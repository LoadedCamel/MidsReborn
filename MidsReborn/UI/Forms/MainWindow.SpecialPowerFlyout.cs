using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls;

namespace Mids_Reborn.UI.Forms
{
    public partial class MainWindow2
    {
        private SpecialPowerFlyout? _specialPowerFlyout;
        private SpecialPowerFlyoutMessageFilter? _specialPowerFlyoutMessageFilter;
        private SpecialPowerCategory? _activeSpecialPowerCategory;
        private Control? _activeSpecialPowerAnchorControl;
        private Rectangle _activeSpecialPowerAnchorBounds;
        private List<IPower>? _activeSpecialPowerSubsetPowers;
        private string? _activeSpecialPowerTitle;
        private string? _activeSpecialPowerChipKey;
        private string? _rememberedPrestigeChipKey;
        private string? _rememberedTempChipKey;

        private bool IsSpecialPowerFlyoutOpen => _specialPowerFlyout is { IsOpen: true };

        private void InitializeSpecialPowerFlyout()
        {
            _specialPowerFlyout = new SpecialPowerFlyout
            {
                Name = "specialPowerFlyout",
                Visible = false
            };

            _specialPowerFlyout.PowerClicked += SpecialPowerFlyout_PowerClicked;
            _specialPowerFlyout.PowerHovered += SpecialPowerFlyout_PowerHovered;
            _specialPowerFlyout.ChipChanged += SpecialPowerFlyout_ChipChanged;
            _specialPowerFlyout.Closed += SpecialPowerFlyout_Closed;

            Controls.Add(_specialPowerFlyout);
            _specialPowerFlyout.UpdateResponsiveLayout(ClientSize);
            _specialPowerFlyout.BringToFront();
            SizeChanged += (_, _) => UpdateSpecialPowerFlyoutLayout(repositionIfOpen: true);

            _specialPowerFlyoutMessageFilter = new SpecialPowerFlyoutMessageFilter(this);
            Application.AddMessageFilter(_specialPowerFlyoutMessageFilter);
            FormClosed += (_, _) =>
            {
                if (_specialPowerFlyoutMessageFilter != null)
                {
                    Application.RemoveMessageFilter(_specialPowerFlyoutMessageFilter);
                }
            };
        }

        private void ToggleSpecialPowerFlyout(Control anchorControl, SpecialPowerCategory category)
        {
            if (anchorControl == null)
            {
                return;
            }

            if (IsSpecialPowerFlyoutOpen &&
                _activeSpecialPowerCategory == category &&
                ReferenceEquals(_activeSpecialPowerAnchorControl, anchorControl))
            {
                CloseSpecialPowerFlyout();
                return;
            }

            ShowSpecialPowerFlyout(anchorControl, category);
        }

        private void ShowSpecialPowerFlyout(
            Control? anchorControl,
            SpecialPowerCategory category,
            IReadOnlyList<IPower>? explicitPowers = null,
            string? title = null,
            string? selectedChipKey = null,
            Rectangle? anchorOverride = null)
        {
            if (_specialPowerFlyout == null || MidsContext.Character?.Archetype == null)
            {
                return;
            }

            var request = CreateSpecialPowerFlyoutRequest(
                anchorControl,
                category,
                explicitPowers,
                title,
                selectedChipKey,
                anchorOverride);

            _activeSpecialPowerCategory = category;
            _activeSpecialPowerAnchorControl = anchorControl;
            _activeSpecialPowerAnchorBounds = request.AnchorBounds;
            _activeSpecialPowerSubsetPowers = explicitPowers?.ToList();
            _activeSpecialPowerTitle = request.Title;
            _activeSpecialPowerChipKey = request.SelectedChipKey;

            CloseLegacySpecialPowerWindows();
            UpdateSpecialPowerButtonStates(category);
            HidePopup();
            UpdateSpecialPowerFlyoutLayout(repositionIfOpen: false);
            _specialPowerFlyout.Open(request);
            PositionSpecialPowerFlyout(request.AnchorBounds);
        }

        private void RefreshActiveSpecialPowerFlyout()
        {
            if (!IsSpecialPowerFlyoutOpen || _activeSpecialPowerCategory == null)
            {
                return;
            }

            ShowSpecialPowerFlyout(
                _activeSpecialPowerAnchorControl,
                _activeSpecialPowerCategory.Value,
                _activeSpecialPowerSubsetPowers,
                _activeSpecialPowerTitle,
                _activeSpecialPowerChipKey,
                _activeSpecialPowerAnchorControl == null ? _activeSpecialPowerAnchorBounds : null);
        }

        private SpecialPowerFlyoutRequest CreateSpecialPowerFlyoutRequest(
            Control? anchorControl,
            SpecialPowerCategory category,
            IReadOnlyList<IPower>? explicitPowers,
            string? title,
            string? selectedChipKey,
            Rectangle? anchorOverride)
        {
            var classId = MidsContext.Character?.Archetype?.Idx ?? -1;
            var alignment = MidsContext.Character?.Alignment ?? Enums.Alignment.Hero;
            var anchorBounds = anchorOverride ?? GetBoundsInFormClient(anchorControl);

            if (category == SpecialPowerCategory.Incarnate)
            {
                var incarnateSets = SpecialPowerCatalog.GetEnabledIncarnateSets(classId);
                var chipOptions = incarnateSets
                    .Select(set => new SpecialPowerChipOption(set.DisplayName, set.DisplayName))
                    .ToList();
                var selectedSet = SpecialPowerChipStateResolver.ResolveSelectedChipKey(
                    selectedChipKey,
                    chipOptions,
                    chipOptions.FirstOrDefault()?.Key);
                var powers = SpecialPowerCatalog.GetIncarnatePowers(selectedSet, classId);

                return new SpecialPowerFlyoutRequest(
                    category,
                    title ?? GetSpecialPowerFlyoutTitle(category),
                    anchorBounds,
                    powers,
                    chipOptions,
                    selectedSet);
            }

            if (category == SpecialPowerCategory.Prestige)
            {
                var prestigeGroups = SpecialPowerCatalog.GetPrestigePowerGroups(classId);
                var chipOptions = prestigeGroups.Count > 1
                    ? prestigeGroups.Select(group => new SpecialPowerChipOption(group.Key, group.Label)).ToList()
                    : [];
                var preferredChipKey = !string.IsNullOrWhiteSpace(selectedChipKey)
                    ? selectedChipKey
                    : _rememberedPrestigeChipKey;
                var resolvedChipKey = SpecialPowerChipStateResolver.ResolveSelectedChipKey(
                    preferredChipKey,
                    chipOptions,
                    chipOptions.FirstOrDefault()?.Key);
                var selectedGroup = prestigeGroups.FirstOrDefault(group =>
                    string.Equals(group.Key, resolvedChipKey, StringComparison.OrdinalIgnoreCase))
                    ?? prestigeGroups.FirstOrDefault();

                return new SpecialPowerFlyoutRequest(
                    category,
                    title ?? GetSpecialPowerFlyoutTitle(category),
                    anchorBounds,
                    selectedGroup?.Powers ?? [],
                    chipOptions,
                    selectedGroup?.Key);
            }

            if (category == SpecialPowerCategory.Temp)
            {
                var tempGroups = SpecialPowerCatalog.GetTempPowerGroups(classId);
                var chipOptions = tempGroups.Count > 1
                    ? tempGroups.Select(group => new SpecialPowerChipOption(group.Key, group.Label)).ToList()
                    : [];
                var preferredChipKey = !string.IsNullOrWhiteSpace(selectedChipKey)
                    ? selectedChipKey
                    : _rememberedTempChipKey;
                var resolvedChipKey = SpecialPowerChipStateResolver.ResolveSelectedChipKey(
                    preferredChipKey,
                    chipOptions,
                    SpecialPowerCatalog.TempPowersChipKey);
                var selectedGroup = tempGroups.FirstOrDefault(group =>
                    string.Equals(group.Key, resolvedChipKey, StringComparison.OrdinalIgnoreCase))
                    ?? tempGroups.FirstOrDefault();

                return new SpecialPowerFlyoutRequest(
                    category,
                    title ?? GetSpecialPowerFlyoutTitle(category),
                    anchorBounds,
                    selectedGroup?.Powers ?? [],
                    chipOptions,
                    selectedGroup?.Key);
            }

            return new SpecialPowerFlyoutRequest(
                category,
                title ?? GetSpecialPowerFlyoutTitle(category),
                anchorBounds,
                SpecialPowerCatalog.GetPowers(category, classId, alignment, explicitPowers));
        }

        private static string GetSpecialPowerFlyoutTitle(SpecialPowerCategory category)
        {
            return category switch
            {
                SpecialPowerCategory.Accolade => "Accolade Powers",
                SpecialPowerCategory.Prestige => "Prestige Powers",
                SpecialPowerCategory.Temp => "Temporary Powers",
                SpecialPowerCategory.Incarnate => "Incarnate Powers",
                SpecialPowerCategory.Subset => "Special Powers",
                _ => "Special Powers"
            };
        }

        private Rectangle GetBoundsInFormClient(Control? control)
        {
            if (control == null)
            {
                return Rectangle.Empty;
            }

            return ToFormClientRect(control.Parent ?? control, control.Bounds);
        }

        private void PositionSpecialPowerFlyout(Rectangle anchorBounds)
        {
            if (_specialPowerFlyout == null)
            {
                return;
            }

            const int margin = 8;
            var flyoutSize = _specialPowerFlyout.Size;

            var x = anchorBounds.Left;
            if (x + flyoutSize.Width > ClientSize.Width - margin)
            {
                x = anchorBounds.Right - flyoutSize.Width;
            }

            x = Math.Max(margin, Math.Min(x, ClientSize.Width - flyoutSize.Width - margin));

            var y = anchorBounds.Bottom + margin;
            if (y + flyoutSize.Height > ClientSize.Height - margin)
            {
                y = anchorBounds.Top - flyoutSize.Height - margin;
            }

            y = Math.Max(MenuBar.Height, Math.Min(y, ClientSize.Height - flyoutSize.Height - margin));

            _specialPowerFlyout.Location = new Point(x, y);
            _specialPowerFlyout.BringToFront();
        }

        private void UpdateSpecialPowerFlyoutLayout(bool repositionIfOpen)
        {
            if (_specialPowerFlyout == null)
            {
                return;
            }

            _specialPowerFlyout.UpdateResponsiveLayout(ClientSize);

            if (!repositionIfOpen || !IsSpecialPowerFlyoutOpen)
            {
                return;
            }

            var anchorBounds = _activeSpecialPowerAnchorControl == null
                ? _activeSpecialPowerAnchorBounds
                : GetBoundsInFormClient(_activeSpecialPowerAnchorControl);

            _activeSpecialPowerAnchorBounds = anchorBounds;
            PositionSpecialPowerFlyout(anchorBounds);
        }

        private void CloseSpecialPowerFlyout()
        {
            _specialPowerFlyout?.CloseFlyout();
        }

        private void CloseLegacySpecialPowerWindows()
        {
            if (fAccolade is { IsDisposed: false, Visible: true })
            {
                fAccolade.Close();
            }

            if (fPrestige is { IsDisposed: false, Visible: true })
            {
                fPrestige.Close();
            }

            if (fTemp is { IsDisposed: false, Visible: true })
            {
                fTemp.Close();
            }

            if (fIncarnate is { IsDisposed: false, Visible: true })
            {
                fIncarnate.Close();
            }
        }

        private void UpdateSpecialPowerButtonStates(SpecialPowerCategory? activeCategory)
        {
            accoladesEx.ToggleState = activeCategory == SpecialPowerCategory.Accolade
                ? MidsVectorButton.States.ToggledOn
                : MidsVectorButton.States.ToggledOff;
            ibPrestigePowersEx.ToggleState = activeCategory == SpecialPowerCategory.Prestige
                ? MidsVectorButton.States.ToggledOn
                : MidsVectorButton.States.ToggledOff;
            incarnatesEx.ToggleState = activeCategory == SpecialPowerCategory.Incarnate
                ? MidsVectorButton.States.ToggledOn
                : MidsVectorButton.States.ToggledOff;
            tempPowersEx.ToggleState = activeCategory == SpecialPowerCategory.Temp
                ? MidsVectorButton.States.ToggledOn
                : MidsVectorButton.States.ToggledOff;
        }

        private bool ShouldKeepSpecialPowerFlyoutOpen(Point screenPoint)
        {
            if (_specialPowerFlyout is null || !_specialPowerFlyout.Visible)
            {
                return false;
            }

            if (_specialPowerFlyout.RectangleToScreen(_specialPowerFlyout.ClientRectangle).Contains(screenPoint))
            {
                return true;
            }

            return _activeSpecialPowerAnchorControl != null &&
                   _activeSpecialPowerAnchorControl.RectangleToScreen(_activeSpecialPowerAnchorControl.ClientRectangle)
                       .Contains(screenPoint);
        }

        private void SpecialPowerFlyout_PowerClicked(object? sender, SpecialPowerSelectionEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Info_Power(e.Power.PowerIndex, -1, false, true);
                return;
            }

            if (_activeSpecialPowerCategory == null || !CanToggleSpecialPower(e.Power, e.ItemState))
            {
                return;
            }

            var mutationCategory = ResolveMutationCategory(_activeSpecialPowerCategory.Value, e.Power);
            if (!ToggleSpecialPowerSelection(e.Power, mutationCategory))
            {
                return;
            }

            PowerModified(true);
            RefreshActiveSpecialPowerFlyout();
            LastClickPlacedSlot = false;
        }

        private void SpecialPowerFlyout_PowerHovered(object? sender, SpecialPowerHoverEventArgs e)
        {
            if (e.Power == null || e.ScreenBounds == Rectangle.Empty)
            {
                HidePopup();
                return;
            }

            var anchorInForm = new Rectangle(PointToClient(e.ScreenBounds.Location), e.ScreenBounds.Size);
            ShowPopup(-1, e.Power.PowerIndex, -1, Point.Empty, anchorInForm, includePowerKindLabel: true);
        }

        private void SpecialPowerFlyout_ChipChanged(object? sender, SpecialPowerChipChangedEventArgs e)
        {
            _activeSpecialPowerChipKey = e.Key;
            if (_activeSpecialPowerCategory == SpecialPowerCategory.Prestige)
            {
                _rememberedPrestigeChipKey = e.Key;
            }
            else if (_activeSpecialPowerCategory == SpecialPowerCategory.Temp)
            {
                _rememberedTempChipKey = e.Key;
            }

            RefreshActiveSpecialPowerFlyout();
        }

        private void SpecialPowerFlyout_Closed(object? sender, EventArgs e)
        {
            _activeSpecialPowerCategory = null;
            _activeSpecialPowerAnchorControl = null;
            _activeSpecialPowerAnchorBounds = Rectangle.Empty;
            _activeSpecialPowerSubsetPowers = null;
            _activeSpecialPowerTitle = null;
            _activeSpecialPowerChipKey = null;
            UpdateSpecialPowerButtonStates(null);
            HidePopup();
        }

        private bool CanToggleSpecialPower(IPower power, MidsItemState itemState)
        {
            var build = MidsContext.Character?.CurrentBuild;
            if (build == null)
            {
                return false;
            }

            if (SpecialPowerBuildIdentityResolver.BuildUsesPower(build, power))
            {
                return true;
            }

            return itemState is not (MidsItemState.Disabled or MidsItemState.Invalid or MidsItemState.Heading);
        }

        private static SpecialPowerCategory ResolveMutationCategory(SpecialPowerCategory category, IPower power)
        {
            if (category != SpecialPowerCategory.Subset)
            {
                return category;
            }

            return power.InherentType switch
            {
                Enums.eGridType.Accolade => SpecialPowerCategory.Accolade,
                Enums.eGridType.Prestige => SpecialPowerCategory.Prestige,
                Enums.eGridType.Incarnate => SpecialPowerCategory.Incarnate,
                _ => SpecialPowerCategory.Temp
            };
        }

        private bool ToggleSpecialPowerSelection(IPower power, SpecialPowerCategory category)
        {
            var build = MidsContext.Character?.CurrentBuild;
            if (build == null)
            {
                return false;
            }

            var existingEntry = SpecialPowerBuildIdentityResolver.FindMatchingEntry(build, power);
            if (existingEntry?.Power != null)
            {
                build.RemovePower(existingEntry.Power);
                return true;
            }

            switch (category)
            {
                case SpecialPowerCategory.Accolade:
                    build.AddPower(power, 49).StatInclude = true;
                    return true;
                case SpecialPowerCategory.Temp:
                    build.AddPower(power, 0).StatInclude = true;
                    return true;
                case SpecialPowerCategory.Incarnate:
                    return ToggleIncarnatePower(build, power);
                case SpecialPowerCategory.Prestige:
                    return AddPrestigePower(build, power);
                case SpecialPowerCategory.Subset:
                    return false;
                default:
                    return false;
            }
        }

        private bool ToggleIncarnatePower(Build build, IPower power)
        {
            var selectedSameSetPowers = build.Powers
                .Where(entry => entry?.Power is not null &&
                                entry.Power.InherentType == Enums.eGridType.Incarnate &&
                                entry.Power.PowerSetID == power.PowerSetID)
                .Select(entry => entry.Power!)
                .ToList();

            var clickedPowerWasSelected = selectedSameSetPowers.Any(existing => SpecialPowerBuildIdentityResolver.Matches(existing, power));
            foreach (var existing in selectedSameSetPowers)
            {
                build.RemovePower(existing);
            }

            if (clickedPowerWasSelected)
            {
                return selectedSameSetPowers.Count > 0;
            }

            build.AddPower(power, 49).StatInclude = true;
            return true;
        }

        private bool AddPrestigePower(Build build, IPower power)
        {
            var toggledOnByDefault =
                (power.AlwaysToggle && power.PowerType == Enums.ePowerType.Toggle) ||
                power.ClickBuff ||
                power.PowerType == Enums.ePowerType.Auto_;

            var entry = build.AddPower(power);
            var buildPowerIndex = build.Powers.FindIndex(candidate =>
                candidate is { Power: not null } && candidate.Power.StaticIndex == entry.Power?.StaticIndex);

            var mutex = buildPowerIndex > -1
                ? MainModule.MidsController.Toon.CurrentBuild.MutexV2(buildPowerIndex)
                : Enums.eMutex.NoConflict;

            entry.StatInclude =
                toggledOnByDefault &&
                (mutex == Enums.eMutex.NoConflict || mutex == Enums.eMutex.NoGroup);

            return true;
        }

        private sealed class SpecialPowerFlyoutMessageFilter : IMessageFilter
        {
            private readonly MainWindow2 _owner;

            public SpecialPowerFlyoutMessageFilter(MainWindow2 owner)
            {
                _owner = owner;
            }

            public bool PreFilterMessage(ref Message m)
            {
                const int WM_LBUTTONDOWN = 0x0201;
                const int WM_RBUTTONDOWN = 0x0204;
                const int WM_MBUTTONDOWN = 0x0207;
                const int WM_NCLBUTTONDOWN = 0x00A1;
                const int WM_NCRBUTTONDOWN = 0x00A4;

                if (m.Msg is not (WM_LBUTTONDOWN or WM_RBUTTONDOWN or WM_MBUTTONDOWN or WM_NCLBUTTONDOWN or WM_NCRBUTTONDOWN))
                {
                    return false;
                }

                if (!_owner.IsSpecialPowerFlyoutOpen)
                {
                    return false;
                }

                var screenPoint = Control.MousePosition;
                if (_owner.ShouldKeepSpecialPowerFlyoutOpen(screenPoint))
                {
                    return false;
                }

                _owner.CloseSpecialPowerFlyout();
                return false;
            }
        }
    }
}

using System.Globalization;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Theming;
using static Mids_Reborn.Core.EnhancementSet;
using static Mids_Reborn.Core.Enums;

namespace Mids_Reborn.UI.Forms
{
    public partial class SetInspector : Form
    {
        private static readonly ePowerSetType[] SlottingPowerSetTypes =
        [
            ePowerSetType.Primary,
            ePowerSetType.Secondary,
            ePowerSetType.Pool,
            ePowerSetType.Ancillary,
            ePowerSetType.Inherent
        ];

        private List<Archetype?>? _archetypeList;
        private static List<EnhancementSet> EnhancementSetList => DatabaseAPI.Database.EnhancementSets;

        private readonly List<ResolvedSetBonusEntry> _bonusIndex = [];
        private List<SetInspectorResult> _setResults = [];
        private SetInspectorResult? _selectedResult;
        private EnhancementSet? _selectedSet;

        private List<IPower>? _baseFilteredPowers;
        private bool _suppressFilterEvents;
        private bool _suppressArchetypeEvents;

        public SetInspector(Form parent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();

            Owner = parent;
            ConfigureDropDowns();
            ApplyTheme();
            if (!DesignMode)
            {
                ThemeManager.ThemeChanged += OnThemeChanged;
            }

            Load += OnLoad;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            CenterToParent();
            ApplyTheme();
            ApplyWindowChrome();
            LayoutControls();
            EnableAtFilters(false);
            setImageList ??= new ImageList
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };

            BuildBonusIndex();
            AssignFilterDropDowns();
            AssignSets();
            SetPowerListMessage("Select an enhancement set and archetype.");
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!IsDisposed && IsHandleCreated)
            {
                LayoutControls();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyWindowChrome();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (!DesignMode)
            {
                ThemeManager.ThemeChanged -= OnThemeChanged;
            }

            base.OnFormClosed(e);
        }

        private ApplicationTheme CurrentAppTheme => DesignMode
            ? ThemeManager.DesignTime
            : ThemeManager.CurrentTheme ?? ThemeManager.DesignTime;

        private void ConfigureDropDowns()
        {
            foreach (var dropDown in new MidsDropDownList[] { cbEffect, cbEffectType, cbEffectStr, cbPvMode, cbVariant })
            {
                dropDown.IconSize = 8;
                dropDown.ItemHeight = 23;
                dropDown.MaxDropDownItems = 12;
                dropDown.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            }

            cbArchetype.IconSize = 20;
            cbArchetype.ItemHeight = 26;
            cbArchetype.MaxDropDownItems = 12;
            cbArchetype.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            cbArchetype.IconProvider = ResolveArchetypeIcon;
        }

        private Bitmap? ResolveArchetypeIcon(object item)
        {
            if (item is not Archetype archetype)
            {
                return null;
            }

            var index = ResolveArchetypeIconIndex(archetype);
            if (index < 0)
            {
                return null;
            }

            AssetManager.Archetypes.TryGetValue(index, out var icon);
            return icon?.Bitmap;
        }

        private static int ResolveArchetypeIconIndex(Archetype archetype)
        {
            if (archetype.Idx >= 0 && archetype.Idx < DatabaseAPI.Database.Classes.Length)
            {
                return archetype.Idx;
            }

            return Array.FindIndex(
                DatabaseAPI.Database.Classes,
                cls => cls != null &&
                       string.Equals(cls.ClassName, archetype.ClassName, StringComparison.OrdinalIgnoreCase));
        }

        private void ApplyTheme()
        {
            var appTheme = CurrentAppTheme;
            var theme = appTheme.DataView;

            BackColor = ResolveColor(theme.Background, Color.Black);
            ForeColor = ResolveColor(theme.Text, Color.WhiteSmoke);
            ApplyListTheme(alvSets, theme);
            ApplyListTheme(alvPowers, theme);
            enhSetInfo1.ApplyTheme(theme);

            cbEffect.Invalidate();
            cbEffectType.Invalidate();
            cbEffectStr.Invalidate();
            cbPvMode.Invalidate();
            cbVariant.Invalidate();
            cbArchetype.Invalidate();
            ApplyWindowChrome();
        }

        private static void ApplyListTheme(Mids_Reborn.UI.Forms.Controls.AdvListView listView, DataViewTheme theme)
        {
            listView.UseAppTheme = true;
            listView.BackColor = ResolveColor(theme.GridRowEven, theme.Card);
            listView.ForeColor = ResolveColor(theme.Text, Color.WhiteSmoke);
            listView.RefreshTheme();
        }

        private void ApplyWindowChrome()
        {
            if (!IsHandleCreated)
            {
                return;
            }

            var theme = CurrentAppTheme.DataView;
            var captionColor = Blend(ResolveColor(theme.HeaderBottom, Color.Black), ResolveColor(theme.Background, Color.Black), 0.28f);
            WinApi.SetDarkMode(Handle, true);
            WinApi.SetWindowCornerPreference(Handle, WinApi.CornerPreference.RoundSmall);
            WinApi.StylizeWindow(
                Handle,
                borderColor: ResolveColor(theme.Border, Color.DodgerBlue),
                captionColor: captionColor,
                textColor: ResolveColor(theme.Text, Color.WhiteSmoke));
        }

        private void LayoutControls()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }

            const int margin = 12;
            const int gap = 10;
            const int filterHeight = 24;
            const int archetypeHeight = 26;
            var contentWidth = Math.Max(1, ClientSize.Width - margin * 2);
            var contentHeight = Math.Max(1, ClientSize.Height - margin * 2);
            var portrait = ClientSize.Width < 980 || ClientSize.Height > ClientSize.Width;

            if (portrait)
            {
                LayoutPortrait(margin, gap, filterHeight, archetypeHeight, contentWidth, contentHeight);
            }
            else
            {
                LayoutLandscape(margin, gap, filterHeight, archetypeHeight, contentWidth, contentHeight);
            }

            ResizeSetColumns();
            ResizePowerColumns();
        }

        private void LayoutLandscape(int margin, int gap, int filterHeight, int archetypeHeight, int contentWidth, int contentHeight)
        {
            var detailWidth = Math.Clamp((int)(contentWidth * 0.43f), 410, 520);
            var leftWidth = Math.Max(520, contentWidth - detailWidth - gap);
            detailWidth = Math.Max(340, contentWidth - leftWidth - gap);
            var filtersTop = margin;
            LayoutFilterRow(margin, filtersTop, leftWidth, filterHeight, singleRow: true);

            var listTop = filtersTop + filterHeight + gap;
            var availableLeftHeight = contentHeight - filterHeight - gap;
            var archetypeTopSpace = archetypeHeight + gap;
            var setHeight = Math.Max(180, (int)((availableLeftHeight - archetypeTopSpace) * 0.52f));
            var powerTop = listTop + setHeight + gap + archetypeHeight + gap;
            var powerHeight = Math.Max(150, margin + contentHeight - powerTop);

            alvSets.SetBounds(margin, listTop, leftWidth, setHeight);
            cbArchetype.SetBounds(margin, listTop + setHeight + gap, Math.Min(190, leftWidth), archetypeHeight);
            alvPowers.SetBounds(margin, powerTop, leftWidth, powerHeight);
            enhSetInfo1.SetBounds(margin + leftWidth + gap, margin, detailWidth, contentHeight);
        }

        private void LayoutPortrait(int margin, int gap, int filterHeight, int archetypeHeight, int contentWidth, int contentHeight)
        {
            var filtersTop = margin;
            LayoutFilterRow(margin, filtersTop, contentWidth, filterHeight, singleRow: false);

            var filterBlockHeight = filterHeight * 2 + gap;
            var listTop = filtersTop + filterBlockHeight + gap;
            var remaining = contentHeight - filterBlockHeight - gap;
            var setHeight = Math.Max(150, (int)(remaining * 0.30f));
            var powersHeight = Math.Max(130, (int)(remaining * 0.26f));
            var detailTop = listTop + setHeight + gap + archetypeHeight + gap + powersHeight + gap;
            var detailHeight = Math.Max(150, margin + contentHeight - detailTop);

            alvSets.SetBounds(margin, listTop, contentWidth, setHeight);
            cbArchetype.SetBounds(margin, listTop + setHeight + gap, Math.Min(220, contentWidth), archetypeHeight);
            alvPowers.SetBounds(margin, listTop + setHeight + gap + archetypeHeight + gap, contentWidth, powersHeight);
            enhSetInfo1.SetBounds(margin, detailTop, contentWidth, detailHeight);
        }

        private void LayoutFilterRow(int left, int top, int width, int height, bool singleRow)
        {
            const int gap = 8;
            if (singleRow)
            {
                var effectWidth = Math.Max(126, (int)(width * 0.25f));
                var typeWidth = Math.Max(118, (int)(width * 0.22f));
                var strengthWidth = Math.Max(102, (int)(width * 0.17f));
                var modeWidth = Math.Max(82, (int)(width * 0.14f));
                var variantWidth = Math.Max(108, width - effectWidth - typeWidth - strengthWidth - modeWidth - gap * 4);

                cbEffect.SetBounds(left, top, effectWidth, height);
                cbEffectType.SetBounds(cbEffect.Right + gap, top, typeWidth, height);
                cbEffectStr.SetBounds(cbEffectType.Right + gap, top, strengthWidth, height);
                cbPvMode.SetBounds(cbEffectStr.Right + gap, top, modeWidth, height);
                cbVariant.SetBounds(cbPvMode.Right + gap, top, variantWidth, height);
                return;
            }

            var firstRowWidth = Math.Max(1, width);
            var effectW = Math.Max(170, (firstRowWidth - gap * 2) / 3);
            var typeW = Math.Max(150, (firstRowWidth - gap * 2) / 3);
            var strengthW = Math.Max(120, firstRowWidth - effectW - typeW - gap * 2);
            cbEffect.SetBounds(left, top, effectW, height);
            cbEffectType.SetBounds(cbEffect.Right + gap, top, typeW, height);
            cbEffectStr.SetBounds(cbEffectType.Right + gap, top, strengthW, height);

            var secondTop = top + height + gap;
            var modeW = Math.Max(120, (width - gap) / 2);
            cbPvMode.SetBounds(left, secondTop, modeW, height);
            cbVariant.SetBounds(cbPvMode.Right + gap, secondTop, Math.Max(120, width - modeW - gap), height);
        }

        private void ResizeSetColumns()
        {
            if (alvSets.Columns.Count < 6)
            {
                return;
            }

            var width = Math.Max(520, alvSets.ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
            alvSets.Columns[1].Width = 62;
            alvSets.Columns[3].Width = 58;
            alvSets.Columns[4].Width = 50;
            alvSets.Columns[5].Width = 86;
            alvSets.Columns[2].Width = Math.Clamp((int)(width * 0.27f), 120, 180);
            alvSets.Columns[0].Width = Math.Max(150, width - alvSets.Columns[1].Width - alvSets.Columns[2].Width - alvSets.Columns[3].Width - alvSets.Columns[4].Width - alvSets.Columns[5].Width - 8);
        }

        private void ResizePowerColumns()
        {
            if (alvPowers.Columns.Count < 3)
            {
                return;
            }

            var width = Math.Max(420, alvPowers.ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
            alvPowers.Columns[0].Width = Math.Clamp((int)(width * 0.38f), 150, 240);
            alvPowers.Columns[1].Width = Math.Clamp((int)(width * 0.22f), 100, 150);
            alvPowers.Columns[2].Width = Math.Max(160, width - alvPowers.Columns[0].Width - alvPowers.Columns[1].Width - 8);
        }

        private void OnThemeChanged()
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke((Action)ApplyTheme);
                return;
            }

            ApplyTheme();
        }

        private static Color ResolveColor(Color value, Color fallback)
        {
            return value.IsEmpty ? fallback : value;
        }

        private static Color Blend(Color first, Color second, float amountSecond)
        {
            amountSecond = Math.Clamp(amountSecond, 0f, 1f);
            var amountFirst = 1f - amountSecond;
            return Color.FromArgb(
                (int)Math.Round(first.A * amountFirst + second.A * amountSecond),
                (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
                (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
                (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
        }

        private void EnableAtFilters(bool aEnable)
        {
            cbArchetype.Enabled = aEnable;
        }

        private void ApplyAtFilter()
        {
            ClearPowerList();
            var genericAt = CreateGenericArchetype();
            var filteredArchetypes = new List<Archetype?> { genericAt };
            if (_selectedResult != null)
            {
                filteredArchetypes.AddRange(DatabaseAPI.Database.Classes
                    .Where(archetype => archetype is { Playable: true } && ArchetypeHasEligiblePowers(archetype, _selectedResult)));
            }

            _archetypeList = filteredArchetypes;
            var selectIdx = 0;
            if (cbArchetype.SelectedIndex > 0)
            {
                var currentAt = cbArchetype.SelectedItem as Archetype;
                var atIdx = currentAt == null
                    ? -1
                    : _archetypeList.FindIndex(at => at != null &&
                                                     string.Equals(at.ClassName, currentAt.ClassName, StringComparison.OrdinalIgnoreCase));
                if (atIdx > 0)
                {
                    selectIdx = atIdx;
                }
            }

            _suppressArchetypeEvents = true;
            cbArchetype.DisplayMember = "DisplayName";
            cbArchetype.ValueMember = null;
            cbArchetype.DataSource = _archetypeList;
            cbArchetype.SelectedIndex = selectIdx;
            _suppressArchetypeEvents = false;
            EnableAtFilters(_archetypeList.Count > 1);

            if (selectIdx > 0)
            {
                PopulatePowersForSelectedArchetype();
            }
            else
            {
                SetPowerListMessage(_archetypeList.Count > 1
                    ? "Select an archetype to view eligible powers."
                    : "No eligible archetypes for this set.");
            }
        }

        private void AssignSets()
        {
            if (!EnhancementSetList.Any()) return;
            BindSetResults(CreateAllSetResults());
        }

        private void AssignFilterDropDowns()
        {
            var effects = _bonusIndex
                .SelectMany(entry => entry.Effects)
                .Where(effect => IsFilterableEffect(effect.EffectType))
                .Select(effect => GetEffectDisplayName(effect.EffectType))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(effectType => effectType, StringComparer.OrdinalIgnoreCase)
                .ToList();

            _suppressFilterEvents = true;
            cbEffect.DataSource = GenerateComboBoxItems(effects, "Select Effect");
            cbEffectType.DataSource = GenerateComboBoxItems([], "Select Type");
            cbEffectType.Enabled = false;
            cbEffectStr.DataSource = GenerateComboBoxItems([], "Select Strength");
            cbEffectStr.Enabled = false;
            cbPvMode.DataSource = GenerateComboBoxItems(["PvE/Any", "PvP", "Any"], "Mode");
            cbPvMode.SelectedIndex = cbPvMode.Items.Count > 1 ? 1 : 0;
            cbVariant.DataSource = GenerateComboBoxItems(["Crafted", "Attuned", "Superior", "Superior Attuned"], "Any Variant");
            cbVariant.SelectedIndex = 0;
            _suppressFilterEvents = false;
        }

        private void BindSetResults(IEnumerable<SetInspectorResult> results)
        {
            _setResults = results.ToList();
            FillImageList(_setResults);
            alvSets.DataSource = _setResults;

            alvSets.ColumnMappings?.Clear();
            alvSets.AddColumnMapping(0, obj => ((SetInspectorResult)obj).Set.DisplayName);
            alvSets.AddColumnMapping(1, obj => ((SetInspectorResult)obj).Set.LevelMin,
                value => (int)value! + 1,
                obj => ((SetInspectorResult)obj!).Set.LevelMax,
                value2 => (int)value2! + 1);
            alvSets.AddColumnMapping(2, obj => ((SetInspectorResult)obj).Set.SetType,
                value => DatabaseAPI.GetSetTypeByIndex((int)value!).Name);
            alvSets.AddColumnMapping(3, obj => ((SetInspectorResult)obj).RequirementText);
            alvSets.AddColumnMapping(4, obj => ((SetInspectorResult)obj).PvModeText);
            alvSets.AddColumnMapping(5, obj => ((SetInspectorResult)obj).VariantText);
        }

        private void FillImageList(IReadOnlyList<SetInspectorResult>? results = null)
        {
            using var extendedBitmap = new ExtendedBitmap(setImageList.ImageSize);
            setImageList.Images.Clear();
            var sourceResults = results ?? CreateAllSetResults();

            for (var index = 0; index < sourceResults.Count; index++)
            {
                var result = sourceResults[index];
                if (result.SetIndex > -1)
                {
                    extendedBitmap.Graphics.Clear(Color.Transparent);
                    var graphics = extendedBitmap.Graphics;
                    var destRect = new Rectangle(0, 0, extendedBitmap.Size.Width, extendedBitmap.Size.Height);

                    AssetManager.DrawEnhancementSet(graphics, destRect, result.SetIndex);

                    if (extendedBitmap.Bitmap != null)
                    {
                        setImageList.Images.Add(extendedBitmap.Bitmap);
                        continue;
                    }
                }

                var images = setImageList.Images;
                var imageSize = setImageList.ImageSize;
                var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
                images.Add(bitmap);
            }
        }

        private async void FillPowerImageList()
        {
            if (_baseFilteredPowers == null) return;
            powerImageList.Images.Clear();

            foreach (var power in _baseFilteredPowers)
            {
                var powerset = power.GetPowerSet();
                if (powerset == null) continue;

                var powersetImage = AssetManager.GetPowersetImage(powerset);
                if (powersetImage?.Bitmap != null)
                {
                    powerImageList.Images.Add(powersetImage.Bitmap);
                }
                else if (AssetManager.UnknownIcon?.Bitmap != null)
                {
                    powerImageList.Images.Add(AssetManager.UnknownIcon.Bitmap);
                }
            }
        }

        private void BuildBonusIndex()
        {
            _bonusIndex.Clear();
            for (var setIndex = 0; setIndex < EnhancementSetList.Count; setIndex++)
            {
                var set = EnhancementSetList[setIndex];
                var variants = GetAvailableVariants(setIndex);

                for (var bonusIndex = 0; bonusIndex < set.Bonus.Length; bonusIndex++)
                {
                    var bonus = set.Bonus[bonusIndex];
                    AddBonusEntry(set, setIndex, variants, bonus, bonusIndex, special: false, pieceIndex: null);
                }

                foreach (var (rawMemberPosition, pieceIndex) in EnumerateSpecialBonusPositions(set, setIndex))
                {
                    if (rawMemberPosition < 0 || rawMemberPosition >= set.SpecialBonus.Length)
                    {
                        continue;
                    }

                    AddBonusEntry(set, setIndex, variants, set.SpecialBonus[rawMemberPosition], rawMemberPosition, special: true, pieceIndex);
                }
            }
        }

        private void AddBonusEntry(
            EnhancementSet set,
            int setIndex,
            IReadOnlyList<SetVariantKind> variants,
            BonusItem bonus,
            int bonusIndex,
            bool special,
            int? pieceIndex)
        {
            var powers = ResolveBonusPowers(bonus);
            if (powers.Count == 0)
            {
                return;
            }

            var pvMode = set.GetEffectiveBonusPvMode(bonusIndex, special);
            var requirement = special ? "Enh" : bonus.Slotted.ToString(CultureInfo.InvariantCulture);
            var source = special
                ? GetSpecialSourceText(setIndex, pieceIndex, bonusIndex)
                : requirement;
            var displayText = set.GetEffectString(bonusIndex, special, true, true, true);
            if (string.IsNullOrWhiteSpace(displayText))
            {
                displayText = string.Join(", ", powers.Select(power => power.DisplayName).Where(text => !string.IsNullOrWhiteSpace(text)));
            }

            var effects = powers
                .SelectMany(power => power.Effects.Select(effect => CreateResolvedEffect(effect, power)))
                .Where(effect => effect.EffectType != eEffectType.None)
                .ToList();
            if (effects.Count == 0)
            {
                return;
            }

            var entry = new ResolvedSetBonusEntry
            {
                Set = set,
                SetIndex = setIndex,
                SetUid = set.Uid ?? string.Empty,
                SetType = set.SetType,
                BonusIndex = bonusIndex,
                Kind = special ? SetBonusKind.Special : SetBonusKind.Standard,
                RequirementText = requirement,
                SourceText = source,
                PvMode = pvMode,
                Variants = variants.ToArray(),
                DisplayText = displayText,
                PowerFullNames = powers.Select(power => power.FullName).Where(name => !string.IsNullOrWhiteSpace(name)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
            };

            foreach (var effect in effects)
            {
                effect.Entry = entry;
                entry.Effects.Add(effect);
            }

            entry.Signature = BuildEntrySignature(entry);
            if (_bonusIndex.Any(existing => existing.SetIndex == entry.SetIndex &&
                                            string.Equals(existing.Signature, entry.Signature, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            _bonusIndex.Add(entry);
        }

        private static IEnumerable<(int RawMemberPosition, int? PieceIndex)> EnumerateSpecialBonusPositions(EnhancementSet set, int setIndex)
        {
            var yielded = new HashSet<int>();
            var projection = DatabaseAPI.GetEnhancementSetProjection(setIndex);
            if (projection.VisiblePieces.Count > 0)
            {
                for (var pieceIndex = 0; pieceIndex < projection.VisiblePieces.Count; pieceIndex++)
                {
                    var rawMemberPosition = DatabaseAPI.GetSpecialRawMemberPositionForSetPiece(setIndex, pieceIndex);
                    if (rawMemberPosition < 0 ||
                        rawMemberPosition >= set.SpecialBonus.Length ||
                        set.SpecialBonus[rawMemberPosition].Index.Length == 0 ||
                        !yielded.Add(rawMemberPosition))
                    {
                        continue;
                    }

                    yield return (rawMemberPosition, pieceIndex);
                }
            }

            if (yielded.Count > 0)
            {
                yield break;
            }

            for (var rawMemberPosition = 0; rawMemberPosition < set.SpecialBonus.Length; rawMemberPosition++)
            {
                if (set.SpecialBonus[rawMemberPosition].Index.Length > 0)
                {
                    yield return (rawMemberPosition, null);
                }
            }
        }

        private static string GetSpecialSourceText(int setIndex, int? pieceIndex, int rawMemberPosition)
        {
            if (!pieceIndex.HasValue)
            {
                return "Enh";
            }

            var projection = DatabaseAPI.GetEnhancementSetProjection(setIndex);
            return pieceIndex.Value >= 0 && pieceIndex.Value < projection.VisiblePieces.Count
                ? projection.VisiblePieces[pieceIndex.Value].DisplayLabel
                : $"Enh {rawMemberPosition + 1}";
        }

        private static IReadOnlyList<IPower> ResolveBonusPowers(BonusItem bonus)
        {
            var powers = new List<IPower>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var powerIndex in bonus.Index)
            {
                if (powerIndex < 0 || powerIndex >= DatabaseAPI.Database.Power.Length)
                {
                    continue;
                }

                AddPower(DatabaseAPI.Database.Power[powerIndex]);
            }

            foreach (var powerName in bonus.Name)
            {
                AddPower(DatabaseAPI.GetPowerByFullName(powerName));
            }

            return powers;

            void AddPower(IPower? power)
            {
                if (power == null)
                {
                    return;
                }

                var key = string.IsNullOrWhiteSpace(power.FullName)
                    ? power.DisplayName
                    : power.FullName;
                if (!string.IsNullOrWhiteSpace(key) && seen.Add(key))
                {
                    powers.Add(power);
                }
            }
        }

        private static ResolvedSetBonusEffect CreateResolvedEffect(IEffect effect, IPower sourcePower)
        {
            var effectType = effect.EffectType;
            var subtypeText = GetSubtypeText(effect, effectType);
            var magnitude = Math.Round(Convert.ToDecimal(effect.MagPercent), 2);
            return new ResolvedSetBonusEffect
            {
                Effect = effect,
                EffectType = effectType,
                DamageType = effect.DamageType,
                MezType = effect.MezType,
                EtModifies = effect.ETModifies,
                SubtypeText = subtypeText,
                Magnitude = magnitude,
                MagnitudeText = FormatMagnitude(effect, GetEffectDisplayName(effectType), subtypeText),
                PowerFullName = string.IsNullOrWhiteSpace(effect.PowerFullName) ? sourcePower.FullName : effect.PowerFullName
            };
        }

        private static string BuildEntrySignature(ResolvedSetBonusEntry entry)
        {
            var effects = entry.Effects
                .Select(effect => $"{effect.EffectType}:{effect.SubtypeText}:{effect.Magnitude.ToString(CultureInfo.InvariantCulture)}")
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase);
            return string.Join("|",
                entry.Kind,
                entry.RequirementText,
                entry.PvMode,
                string.Join(",", entry.PowerFullNames.OrderBy(value => value, StringComparer.OrdinalIgnoreCase)),
                string.Join(",", effects));
        }

        private void CbEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressFilterEvents) return;
            if (cbEffect.SelectedIndex <= 0)
            {
                ResetSetFilter();
                ApplyCurrentFilter();
                return;
            }

            var selectedEffectType = cbEffect.GetItemText(cbEffect.SelectedItem);
            if (!TryParseEffectType(selectedEffectType, out var effectType))
            {
                ResetSetFilter();
                return;
            }

            var subtypeItems = GetSubtypeItems(effectType);

            _suppressFilterEvents = true;
            cbEffectType.DataSource = GenerateComboBoxItems(subtypeItems, subtypeItems.Count > 0 ? "Select Type" : "Any Type");
            cbEffectType.Enabled = subtypeItems.Count > 0;
            cbEffectStr.DataSource = GenerateComboBoxItems([], "Select Strength");
            cbEffectStr.Enabled = false;
            _suppressFilterEvents = false;

            if (subtypeItems.Count == 0)
            {
                PopulateStrengthFilter(autoSelect: true);
            }
            else
            {
                BindSetResults(CreateAllSetResults());
                ClearSelectedSet();
            }
        }

        private void CbEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressFilterEvents) return;
            PopulateStrengthFilter(autoSelect: true);
        }

        private void CbEffectStr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressFilterEvents) return;
            ApplyCurrentFilter();
        }

        private void CbPvMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressFilterEvents) return;
            ApplyCurrentFilter();
        }

        private void CbVariant_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressFilterEvents) return;
            ApplyCurrentFilter();
        }

        private void PopulateStrengthFilter(bool autoSelect)
        {
            if (cbEffect.SelectedIndex <= 0)
            {
                return;
            }

            var filteredEffects = GetEffectsForCurrentFilter(includeStrength: false).ToList();
            var formattedMagnitudes = filteredEffects
                .Select(effect => effect.MagnitudeText)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(ParseMagnitudeForSort)
                .ToList();

            if (formattedMagnitudes.Count > 1)
            {
                formattedMagnitudes.Insert(0, "Any");
            }

            _suppressFilterEvents = true;
            cbEffectStr.DataSource = GenerateComboBoxItems(formattedMagnitudes, "Select Strength");
            cbEffectStr.Enabled = formattedMagnitudes.Count > 0;
            if (autoSelect && formattedMagnitudes.Count > 0)
            {
                cbEffectStr.SelectedIndex = 1;
            }
            _suppressFilterEvents = false;

            if (autoSelect && formattedMagnitudes.Count > 0)
            {
                ApplyCurrentFilter();
            }
        }

        private void ApplyCurrentFilter()
        {
            var variantFilter = GetSelectedVariantFilter();
            if (cbEffect.SelectedIndex <= 0)
            {
                var results = CreateAllSetResults(variantFilter)
                    .Where(result => EntryMatchesVariant(result.Variants, variantFilter));
                BindSetResults(results);
                ClearSelectedSet();
                return;
            }

            var selectedEffects = GetEffectsForCurrentFilter(includeStrength: true).ToList();
            if (selectedEffects.Count == 0)
            {
                BindSetResults([]);
                ClearSelectedSet();
                return;
            }

            var entries = selectedEffects
                .Select(effect => effect.Entry)
                .Where(entry => entry != null)
                .Cast<ResolvedSetBonusEntry>()
                .Distinct()
                .ToList();
            BindSetResults(CreateResultsFromEntries(entries, variantFilter));
            ClearSelectedSet();
        }

        private IEnumerable<ResolvedSetBonusEffect> GetEffectsForCurrentFilter(bool includeStrength)
        {
            var selectedEffectType = cbEffect.GetItemText(cbEffect.SelectedItem);
            if (!TryParseEffectType(selectedEffectType, out var effectType))
            {
                return [];
            }

            if (RequiresSubtype(effectType) && cbEffectType.SelectedIndex <= 0)
            {
                return [];
            }

            var selectedSubtype = cbEffectType.Enabled ? cbEffectType.GetItemText(cbEffectType.SelectedItem) : string.Empty;
            var selectedStrength = cbEffectStr.SelectedIndex > 0 ? cbEffectStr.GetItemText(cbEffectStr.SelectedItem) : string.Empty;
            var variantFilter = GetSelectedVariantFilter();

            var effects = _bonusIndex
                .Where(entry => EntryMatchesPvMode(entry) && EntryMatchesVariant(entry.Variants, variantFilter))
                .SelectMany(entry => entry.Effects)
                .Where(effect => effect.EffectType == effectType && EffectSubtypeMatches(effect, effectType, selectedSubtype));

            if (!includeStrength || string.IsNullOrWhiteSpace(selectedStrength) || selectedStrength == "Any")
            {
                return effects;
            }

            return effects.Where(effect => string.Equals(effect.MagnitudeText, selectedStrength, StringComparison.OrdinalIgnoreCase));
        }

        private List<string> GetSubtypeItems(eEffectType effectType)
        {
            return _bonusIndex
                .Where(entry => EntryMatchesPvMode(entry) && EntryMatchesVariant(entry.Variants, GetSelectedVariantFilter()))
                .SelectMany(entry => entry.Effects)
                .Where(effect => effect.EffectType == effectType)
                .Select(effect => effect.SubtypeText)
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(text => text, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private bool EntryMatchesPvMode(ResolvedSetBonusEntry entry)
        {
            var selectedMode = cbPvMode.GetItemText(cbPvMode.SelectedItem);
            return selectedMode switch
            {
                "PvP" => entry.PvMode == ePvX.PvP,
                "Any" => true,
                _ => entry.PvMode != ePvX.PvP
            };
        }

        private static bool EntryMatchesVariant(IReadOnlyCollection<SetVariantKind> variants, SetVariantKind? selectedVariant)
        {
            return !selectedVariant.HasValue || variants.Contains(selectedVariant.Value);
        }

        private SetVariantKind? GetSelectedVariantFilter()
        {
            return cbVariant.SelectedIndex <= 0
                ? null
                : ParseVariant(cbVariant.GetItemText(cbVariant.SelectedItem));
        }

        private static SetVariantKind? ParseVariant(string text)
        {
            return text switch
            {
                "Crafted" => SetVariantKind.Crafted,
                "Attuned" => SetVariantKind.Attuned,
                "Superior" => SetVariantKind.Superior,
                "Superior Attuned" => SetVariantKind.SuperiorAttuned,
                _ => null
            };
        }

        private static bool TryParseEffectType(string selectedEffectType, out eEffectType effectType)
        {
            if (selectedEffectType == "MaxEnd")
            {
                selectedEffectType = nameof(eEffectType.Endurance);
            }

            return Enum.TryParse(selectedEffectType, out effectType);
        }

        private static bool IsFilterableEffect(eEffectType effectType)
        {
            return effectType is not (eEffectType.Damage or eEffectType.GlobalChanceMod or eEffectType.GrantPower or eEffectType.Null);
        }

        private static string GetEffectDisplayName(eEffectType effectType)
        {
            return effectType is eEffectType.Endurance ? "MaxEnd" : effectType.ToString();
        }

        private static bool RequiresSubtype(eEffectType effectType)
        {
            return effectType is eEffectType.Defense
                or eEffectType.Resistance
                or eEffectType.DamageBuff
                or eEffectType.Enhancement
                or eEffectType.ResEffect
                or eEffectType.Mez
                or eEffectType.MezProtect
                or eEffectType.MezResist;
        }

        private static string GetSubtypeText(IEffect effect, eEffectType effectType)
        {
            return effectType switch
            {
                eEffectType.Defense or eEffectType.Resistance or eEffectType.DamageBuff => effect.DamageType.ToString(),
                eEffectType.Enhancement or eEffectType.ResEffect => effect.ETModifies is eEffectType.Mez or eEffectType.MezProtect or eEffectType.MezResist
                    ? $"{effect.ETModifies} ({effect.MezType})"
                    : effect.ETModifies.ToString(),
                eEffectType.Mez or eEffectType.MezProtect or eEffectType.MezResist => effect.MezType.ToString(),
                _ => string.Empty
            };
        }

        private static bool EffectSubtypeMatches(ResolvedSetBonusEffect effect, eEffectType effectType, string selectedSubtype)
        {
            if (!RequiresSubtype(effectType))
            {
                return true;
            }

            return string.Equals(effect.SubtypeText, selectedSubtype, StringComparison.OrdinalIgnoreCase);
        }

        private static string FormatMagnitude(IEffect effect, string selectedEffectType, string selectedSubtype)
        {
            var magnitude = Math.Round(Convert.ToDecimal(effect.MagPercent), 2);
            if (effect.MezType is eMez.Knockback)
            {
                return $"Mag {Math.Abs(magnitude).ToString(CultureInfo.InvariantCulture)}";
            }

            var unit = selectedEffectType == "HitPoints" || selectedSubtype.Contains("HitPoints", StringComparison.OrdinalIgnoreCase)
                ? " HP"
                : "%";
            return $"{magnitude.ToString(CultureInfo.InvariantCulture)}{unit}";
        }

        private static decimal ParseMagnitudeForSort(string magnitude)
        {
            if (string.Equals(magnitude, "Any", StringComparison.OrdinalIgnoreCase))
            {
                return decimal.MinValue;
            }

            var cleaned = magnitude
                .Replace("Mag ", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" HP", "", StringComparison.OrdinalIgnoreCase)
                .Replace("%", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            return decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
                ? value
                : 0;
        }

        private List<SetInspectorResult> CreateAllSetResults(SetVariantKind? variantFilter = null)
        {
            return EnhancementSetList
                .Select((set, index) => CreateResult(
                    set,
                    index,
                    [],
                    variantFilter))
                .Where(result => EntryMatchesVariant(result.Variants, variantFilter))
                .OrderBy(result => result.Set.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private IEnumerable<SetInspectorResult> CreateResultsFromEntries(IEnumerable<ResolvedSetBonusEntry> entries, SetVariantKind? variantFilter)
        {
            return entries
                .GroupBy(entry => entry.SetIndex)
                .Select(group =>
                {
                    var set = group.First().Set;
                    var matchedEntries = group
                        .GroupBy(entry => entry.Signature, StringComparer.OrdinalIgnoreCase)
                        .Select(entryGroup => entryGroup.First())
                        .ToList();
                    return CreateResult(set, group.Key, matchedEntries, variantFilter);
                })
                .Where(result => EntryMatchesVariant(result.Variants, variantFilter))
                .OrderBy(result => result.Set.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static SetInspectorResult CreateResult(
            EnhancementSet set,
            int setIndex,
            IReadOnlyList<ResolvedSetBonusEntry> matchedEntries,
            SetVariantKind? variantFilter)
        {
            var variants = GetAvailableVariants(setIndex);
            return new SetInspectorResult
            {
                Set = set,
                SetIndex = setIndex,
                MatchedEntries = matchedEntries.ToArray(),
                Variants = variants.ToArray(),
                VariantFilter = variantFilter,
                RepresentativeEnhancementIds = BuildRepresentativeEnhancementIds(set, setIndex, variantFilter),
                RequirementText = FormatRequirementText(matchedEntries),
                PvModeText = FormatPvModeText(matchedEntries),
                VariantText = FormatVariantBadges(variants)
            };
        }

        private static IReadOnlyList<SetVariantKind> GetAvailableVariants(int setIndex)
        {
            return setIndex < 0
                ? Array.Empty<SetVariantKind>()
                : DatabaseAPI.GetAvailableSetVariants(setIndex);
        }

        private static int[] BuildRepresentativeEnhancementIds(EnhancementSet set, int setIndex, SetVariantKind? variantFilter)
        {
            var enhancementIds = new List<int>();
            if (variantFilter.HasValue && setIndex >= 0)
            {
                var projection = DatabaseAPI.GetEnhancementSetProjection(setIndex);
                enhancementIds.AddRange(projection.VisiblePieces
                    .Select(piece => DatabaseAPI.ResolveEnhancementVariantForSetPiece(setIndex, piece.PieceIndex, variantFilter.Value))
                    .Where(IsValidEnhancementId));
            }

            if (enhancementIds.Count == 0)
            {
                enhancementIds.AddRange((set.Enhancements ?? Array.Empty<int>()).Where(IsValidEnhancementId));
            }

            return enhancementIds.Distinct().ToArray();
        }

        private static bool IsValidEnhancementId(int enhancementId)
        {
            return enhancementId >= 0 && enhancementId < DatabaseAPI.Database.Enhancements.Length;
        }

        private static string FormatRequirementText(IReadOnlyList<ResolvedSetBonusEntry> matchedEntries)
        {
            if (matchedEntries.Count == 0)
            {
                return "?";
            }

            var requirements = matchedEntries
                .Select(entry => entry.RequirementText)
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(ParseRequirementForSort)
                .ThenBy(text => text, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            return requirements.Length == 0 ? "-" : string.Join(", ", requirements);
        }

        private static int ParseRequirementForSort(string text)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : int.MaxValue;
        }

        private static string FormatPvModeText(IReadOnlyList<ResolvedSetBonusEntry> matchedEntries)
        {
            if (matchedEntries.Count == 0)
            {
                return "-";
            }

            var modes = matchedEntries
                .Select(entry => FormatPvMode(entry.PvMode))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(mode => mode, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            return modes.Length == 0 ? "-" : string.Join(", ", modes);
        }

        private static string FormatPvMode(ePvX mode)
        {
            return mode switch
            {
                ePvX.PvP => "PvP",
                ePvX.PvE => "PvE",
                _ => "Any"
            };
        }

        private static string FormatVariantBadges(IReadOnlyCollection<SetVariantKind> variants)
        {
            if (variants.Count == 0)
            {
                return "-";
            }

            return string.Join(", ", variants
                .OrderBy(DatabaseAPI.GetSetVariantSortKey)
                .Select(FormatVariantBadge));
        }

        private static string FormatVariantBadge(SetVariantKind variant)
        {
            return variant switch
            {
                SetVariantKind.Crafted => "C",
                SetVariantKind.Attuned => "A",
                SetVariantKind.Superior => "S",
                SetVariantKind.SuperiorAttuned => "SA",
                _ => variant.ToString()
            };
        }

        private static string FormatVariantName(SetVariantKind variant)
        {
            return variant switch
            {
                SetVariantKind.Crafted => "Crafted",
                SetVariantKind.Attuned => "Attuned",
                SetVariantKind.Superior => "Superior",
                SetVariantKind.SuperiorAttuned => "Superior Attuned",
                _ => variant.ToString()
            };
        }

        private void AdvListView1OnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (alvSets.SelectedItems.Count == 0) return;

            var selectedIndex = alvSets.SelectedIndices[0];
            if (selectedIndex < 0 || selectedIndex >= alvSets.Items.Count) return;
            if (alvSets.DataSource is not List<SetInspectorResult> results) return;

            _selectedResult = results[selectedIndex];
            _selectedSet = _selectedResult.Set;
            var setData = BuildSetData(_selectedResult);
            enhSetInfo1.SetInfo(setData);
            ApplyAtFilter();
        }

        private EnhSetInfo.SetData BuildSetData(SetInspectorResult result)
        {
            var set = result.Set;
            var setData = new EnhSetInfo.SetData
            {
                Set = set.DisplayName,
                SetRarity = set.GetEnhancementSetRarity(),
                SetType = DatabaseAPI.GetSetTypeByIndex(set.SetType).Name,
                LevelRange = $"{set.LevelMin + 1} to {set.LevelMax + 1}",
                EnhCount = GetVisibleEnhancementCount(result).ToString(CultureInfo.InvariantCulture),
            };

            foreach (var enhancementText in GetVisibleEnhancementTexts(result))
            {
                setData.Enhancements.Add(enhancementText);
            }

            for (var index = 0; index < set.Bonus.Length; index++)
            {
                var bonus = set.Bonus[index];
                var effectString = set.GetEffectString(index, false, true, true, true);
                if (string.IsNullOrWhiteSpace(effectString)) continue;
                if (set.GetEffectiveBonusPvMode(index, false) is ePvX.PvP) effectString += " (PVP)";
                setData.Bonuses.Add($"({bonus.Slotted}) {effectString}");
            }

            foreach (var specialBonusRow in EnhancementSetSpecialBonusDisplay.BuildRows(set, result.SetIndex))
            {
                foreach (var effectStringRaw in specialBonusRow.EffectStrings)
                {
                    var effectString = effectStringRaw;
                    if (specialBonusRow.PvMode is ePvX.PvP) effectString += " (PVP)";
                    setData.Bonuses.Add($"(Enh) {effectString}");
                }
            }

            var selectedBonuses = result.MatchedEntries
                .Select(entry => entry.DisplayText)
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            setData.Selected = selectedBonuses.FirstOrDefault() ?? string.Empty;
            setData.SelectedBonuses.AddRange(selectedBonuses);
            return setData;
        }

        private static int GetVisibleEnhancementCount(SetInspectorResult result)
        {
            var projection = DatabaseAPI.GetEnhancementSetProjection(result.SetIndex);
            return projection.VisiblePieces.Count > 0
                ? projection.VisiblePieces.Count
                : result.Set.Enhancements.Length;
        }

        private static IEnumerable<string> GetVisibleEnhancementTexts(SetInspectorResult result)
        {
            var projection = DatabaseAPI.GetEnhancementSetProjection(result.SetIndex);
            if (projection.VisiblePieces.Count > 0)
            {
                foreach (var piece in projection.VisiblePieces)
                {
                    var variants = piece.Variants.Keys
                        .OrderBy(DatabaseAPI.GetSetVariantSortKey)
                        .Select(FormatVariantName)
                        .ToArray();
                    yield return variants.Length == 0
                        ? piece.DisplayLabel
                        : $"{piece.DisplayLabel} [{string.Join(", ", variants)}]";
                }

                yield break;
            }

            foreach (var enhancementId in result.Set.Enhancements.Where(IsValidEnhancementId))
            {
                yield return DatabaseAPI.Database.Enhancements[enhancementId].Name;
            }
        }

        private static List<StandardListItem> GenerateComboBoxItems(IEnumerable<string> items, string placeholderText)
        {
            var comboBoxItems = new List<StandardListItem> { new(placeholderText) };
            comboBoxItems.AddRange(items.Select(item => new StandardListItem(item)));
            return comboBoxItems;
        }

        private void CbArchetypeOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_suppressArchetypeEvents) return;
            PopulatePowersForSelectedArchetype();
        }

        private void PopulatePowersForSelectedArchetype()
        {
            switch (cbArchetype.SelectedIndex)
            {
                case < 0:
                    return;
                case 0:
                    SetPowerListMessage(_selectedSet == null
                        ? "Select an enhancement set and archetype."
                        : "Select an archetype to view eligible powers.");
                    return;
            }

            if (_selectedResult == null)
            {
                SetPowerListMessage("Select an enhancement set first.");
                return;
            }

            if (cbArchetype.SelectedItem is not Archetype archetype) return;

            _baseFilteredPowers = GetEligiblePowers(archetype, _selectedResult).ToList();
            if (_baseFilteredPowers.Count == 0)
            {
                SetPowerListMessage("No eligible powers for this archetype and set.");
                return;
            }

            EnableAtFilters(true);
            FillAlvPowers();
        }

        private void FillAlvPowers()
        {
            FillPowerImageList();
            alvPowers.DataSource = _baseFilteredPowers;
            alvPowers.AddColumnMapping(0, obj => ((IPower)obj).GetPowerSet()?.DisplayName);
            alvPowers.AddColumnMapping(1, obj => ((IPower)obj).GetPowerSet()?.SetType);
            alvPowers.AddColumnMapping(2, obj => ((IPower)obj).DisplayName);
            UpdateAlvPowerColumns();
        }

        private void UpdateAlvPowerColumns(bool usePower = false)
        {
            if (usePower)
            {
                alvPowers.Columns[0].Text = @"Power";
                alvPowers.Columns[1].Text = @"Type";
                alvPowers.Columns[2].Text = @"Available At";
            }
            else
            {
                alvPowers.Columns[0].Text = @"Powerset";
                alvPowers.Columns[1].Text = @"Type";
                alvPowers.Columns[2].Text = @"Power";
            }
            alvPowers.Refresh();
        }

        private void ResetSetFilter()
        {
            _suppressFilterEvents = true;
            cbEffectType.DataSource = GenerateComboBoxItems([], "Select Type");
            cbEffectType.Enabled = false;
            cbEffectStr.DataSource = GenerateComboBoxItems([], "Select Strength");
            cbEffectStr.Enabled = false;
            _suppressFilterEvents = false;
        }

        private void ClearSelectedSet()
        {
            _selectedResult = null;
            _selectedSet = null;
            enhSetInfo1.Clear();
            EnableAtFilters(false);
            _archetypeList = [CreateGenericArchetype()];

            _suppressArchetypeEvents = true;
            cbArchetype.DisplayMember = "DisplayName";
            cbArchetype.ValueMember = null;
            cbArchetype.DataSource = _archetypeList;
            cbArchetype.SelectedIndex = 0;
            _suppressArchetypeEvents = false;

            SetPowerListMessage("Select an enhancement set and archetype.");
        }

        private static Archetype CreateGenericArchetype()
        {
            return new Archetype
            {
                DisplayName = "Select Archetype",
                Playable = false
            };
        }

        private void ClearPowerList()
        {
            _baseFilteredPowers = null;
            powerImageList.Images.Clear();
            alvPowers.DataSource = null;
            alvPowers.Items.Clear();
        }

        private void SetPowerListMessage(string message)
        {
            _baseFilteredPowers = null;
            powerImageList.Images.Clear();
            alvPowers.DataSource = null;
            alvPowers.Items.Clear();

            var item = new ListViewItem(message);
            item.SubItems.Add(string.Empty);
            item.SubItems.Add(string.Empty);
            alvPowers.Items.Add(item);
            alvPowers.Refresh();
        }

        private static bool ArchetypeHasEligiblePowers(Archetype archetype, SetInspectorResult result)
        {
            return GetEligiblePowers(archetype, result).Any();
        }

        private static IEnumerable<IPower> GetEligiblePowers(Archetype archetype, SetInspectorResult result)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var powerset in GetAccessiblePowersets(archetype))
            {
                foreach (var power in powerset.Powers.Where(power => IsEligibleSlottingPower(power, archetype, powerset)))
                {
                    if (!PowerCanSlotSet(power, result))
                    {
                        continue;
                    }

                    var key = string.IsNullOrWhiteSpace(power.FullName)
                        ? $"{powerset.FullName}.{power.DisplayName}"
                        : power.FullName;
                    if (seen.Add(key))
                    {
                        yield return power;
                    }
                }
            }
        }

        private static IEnumerable<IPowerset> GetAccessiblePowersets(Archetype archetype)
        {
            return SlottingPowerSetTypes
                .SelectMany(powerSetType => DatabaseAPI.GetPowersetIndexes(archetype, powerSetType))
                .Where(powerSet => powerSet != null)
                .Cast<IPowerset>()
                .Where(powerSet => SlottingPowerSetTypes.Contains(powerSet.SetType))
                .DistinctBy(powerSet => powerSet.nID);
        }

        private static bool IsEligibleSlottingPower(IPower? power, Archetype archetype, IPowerset powerset)
        {
            return power != null &&
                   !power.HiddenPower &&
                   !power.DoNotSave &&
                   !string.IsNullOrWhiteSpace(power.DisplayName) &&
                   SlottingPowerSetTypes.Contains(powerset.SetType) &&
                   power.AllowedForClass(archetype.Idx);
        }

        private static bool PowerCanSlotSet(IPower power, SetInspectorResult result)
        {
            return power.SetTypes.Contains(result.Set.SetType) &&
                   result.RepresentativeEnhancementIds.Any(enhancementId =>
                       DatabaseAPI.ValidateEnhancementForPower(power, enhancementId).IsValid);
        }

        private enum SetBonusKind
        {
            Standard,
            Special
        }

        private sealed class ResolvedSetBonusEntry
        {
            public required EnhancementSet Set { get; init; }
            public required int SetIndex { get; init; }
            public required string SetUid { get; init; }
            public required int SetType { get; init; }
            public required int BonusIndex { get; init; }
            public required SetBonusKind Kind { get; init; }
            public required string RequirementText { get; init; }
            public required string SourceText { get; init; }
            public required ePvX PvMode { get; init; }
            public required SetVariantKind[] Variants { get; init; }
            public required string DisplayText { get; init; }
            public required string[] PowerFullNames { get; init; }
            public string Signature { get; set; } = string.Empty;
            public List<ResolvedSetBonusEffect> Effects { get; } = [];
        }

        private sealed class ResolvedSetBonusEffect
        {
            public required IEffect Effect { get; init; }
            public required eEffectType EffectType { get; init; }
            public required eDamage DamageType { get; init; }
            public required eMez MezType { get; init; }
            public required eEffectType EtModifies { get; init; }
            public required string SubtypeText { get; init; }
            public required decimal Magnitude { get; init; }
            public required string MagnitudeText { get; init; }
            public required string PowerFullName { get; init; }
            public ResolvedSetBonusEntry? Entry { get; set; }
        }

        private sealed class SetInspectorResult
        {
            public required EnhancementSet Set { get; init; }
            public required int SetIndex { get; init; }
            public required ResolvedSetBonusEntry[] MatchedEntries { get; init; }
            public required SetVariantKind[] Variants { get; init; }
            public SetVariantKind? VariantFilter { get; init; }
            public required int[] RepresentativeEnhancementIds { get; init; }
            public required string RequirementText { get; init; }
            public required string PvModeText { get; init; }
            public required string VariantText { get; init; }
        }

        internal class StandardListItem
        {
            public string Text { get; set; }

            public StandardListItem(string text)
            {
                Text = text;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}

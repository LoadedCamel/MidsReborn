using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Skia;
using MRBResourceLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms
{
    public partial class FrmIncarnate : Form
    {
        private enum IncarnateGroup
        {
            Alpha,
            Judgement,
            Interface,
            Lore,
            Destiny,
            Hybrid,
            Genesis,
            Stance,
            Vitae,
            Omega
        }

        private readonly Dictionary<IncarnateGroup, ImageButtonEx> _buttons;
        private readonly MainWindow2? _myParent;
        private bool _locked;
        private IPower?[]? _myPowers;
        private IncarnateGroup _currentIncarnateGroup;
        private const ImageButtonEx.EnabledStates DisabledButtonStyle = ImageButtonEx.EnabledStates.DisabledGloss;

        public FrmIncarnate(ref MainWindow2 iParent)
        {
            Icon = Resources.MRB_Icon_Concept;
            _myParent = iParent;
            _locked = false;
            _currentIncarnateGroup = IncarnateGroup.Alpha;
            _myPowers = DatabaseAPI.Database.Power.Where(e => e != null && e.FullName.StartsWith("Incarnate.")).ToArray();
            InitializeComponent();
            _buttons = new Dictionary<IncarnateGroup, ImageButtonEx>
            {
                { IncarnateGroup.Alpha, BtnAlpha },
                { IncarnateGroup.Judgement, BtnJudgement },
                { IncarnateGroup.Interface, BtnInterface },
                { IncarnateGroup.Lore, BtnLore },
                { IncarnateGroup.Destiny, BtnDestiny },
                { IncarnateGroup.Hybrid, BtnHybrid },
                { IncarnateGroup.Genesis, BtnGenesis },
                { IncarnateGroup.Stance, BtnStance },
                { IncarnateGroup.Vitae, BtnVitae },
                { IncarnateGroup.Omega, BtnOmega }
            };
        }

        private void frmIncarnate_Load(object? sender, EventArgs e)
        {
            // Bug: StartPosition doesn't work, if called from constructor (old way)
            CenterToParent();

            var iPopup = new PopUp.PopupData();
            var index = iPopup.Add();
            iPopup.Sections[index].Add("Click powers to enable/disable them.", PopUp.Colors.Title);
            iPopup.Sections[index]
                .Add("Powers in gray (or your custom 'power disabled' color) cannot be included in your stats.",
                    PopUp.Colors.Text, 0.9f);
            PopInfo.SetPopup(iPopup);
            ChangedScrollFrameContents();
            ToggleButtons();
            EnableButtons();
            UpdateColorTheme();
            UpdateFonts();
            FillLists("Alpha");
        }

        public void UpdateColorTheme()
        {
            SetListColors();

            foreach (var g in _buttons)
            {
                g.Value.UseAlt = MidsContext.Character?.IsHero() == false;
            }

            BtnDone.UseAlt = MidsContext.Character?.IsHero() == false;
        }

        public void UpdateColorTheme(Enums.Alignment alignment)
        {
            SetListColors(alignment);

            foreach (var g in _buttons)
            {
                g.Value.UseAlt = alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
            }

            BtnDone.UseAlt = alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
        }

        private void SetListColors(Enums.Alignment? alignment = null)
        {
            SkPairedList1.SuspendRedraw = true;

            var isHero = alignment == null
                ? MidsContext.Character.IsHero()
                : alignment is Enums.Alignment.Hero or Enums.Alignment.Vigilante or Enums.Alignment.Resistance;

            // ----------- Alignment-specific colors -----------
            SkPairedList1.ScrollBarColor = isHero
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain;

            SkPairedList1.ScrollButtonColor = isHero
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;

            SkPairedList1.UpdateTextColors(EItemState.Selected,
                isHero
                    ? MidsContext.Config.RtFont.ColorPowerTakenHero
                    : MidsContext.Config.RtFont.ColorPowerTakenVillain);

            SkPairedList1.UpdateTextColors(EItemState.SelectedDisabled,
                isHero
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);

            SkPairedList1.HoverColor = isHero
                ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                : MidsContext.Config.RtFont.ColorPowerHighlightVillain;

            // ----------- Others -----------
            SkPairedList1.UpdateTextColors(EItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            SkPairedList1.UpdateTextColors(EItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            // No custom color for headings

            SkPairedList1.SuspendRedraw = false;
            SkPairedList1.Invalidate();
        }

        public void UpdateFonts(Font font)
        {
            UpdateColorTheme();

            SkPairedList1.SuspendRedraw = true;
            SkPairedList1.Font = font;
            foreach (var item in SkPairedList1.Items)
            {
                item.Bold = MidsContext.Config.RtFont.PowersSelectBold;
            }

            SkPairedList1.SuspendRedraw = false;
            SkPairedList1.Invalidate();
        }

        public void UpdateFonts()
        {
            UpdateColorTheme();

            SkPairedList1.Font = new Font(SkPairedList1.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase,
                !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold, GraphicsUnit.Point);
            foreach (var e in SkPairedList1.Items)
            {
                e.Bold = MidsContext.Config.RtFont.PowersSelectBold;
            }
        }

        private void IncarnateGroupButton_Click(object? sender, EventArgs e)
        {
            if (sender is not ImageButtonEx btn)
            {
                return;
            }

            var btnNames = _buttons.Values.Select(b => b.Name);
            if (!btnNames.Contains(btn.Name))
            {
                return;
            }

            var group = _buttons.First(g => g.Value.Name == btn.Name).Key;
            if (group == _currentIncarnateGroup)
            {
                return;
            }

            ToggleButtons(group);
            _currentIncarnateGroup = group;
            FillLists(_currentIncarnateGroup.ToString());
        }

        private void ToggleButtons(IncarnateGroup? newGroup = null)
        {
            var changedGroup = newGroup == null || newGroup != _currentIncarnateGroup;
            Debug.WriteLine($"ToggleButtons({(newGroup == null ? "null" : newGroup)}): changedGroup: {changedGroup}");
            newGroup ??= _currentIncarnateGroup;
            Debug.WriteLine($"  New group: {newGroup}, Current group: {_currentIncarnateGroup}");

            if (!changedGroup)
            {
                return;
            }

            foreach (var g in _buttons)
            {
                if (newGroup == g.Key)
                {
                    g.Value.ToggleState = ImageButtonEx.States.ToggledOn;
                    g.Value.Invalidate();

                    continue;
                }
                
                g.Value.ToggleState = ImageButtonEx.States.ToggledOff;
            }
        }

        private void EnableButtons()
        {
            foreach (var g in _buttons)
            {
                Debug.WriteLine($"EnableButtons({g.Key}): {DatabaseAPI.ServerData.EnabledIncarnates[g.Key.ToString()]}");
                g.Value.EnabledState = DatabaseAPI.ServerData.EnabledIncarnates[g.Key.ToString()]
                    ? ImageButtonEx.EnabledStates.Enabled
                    : DisabledButtonStyle;
            }
        }

        private void BtnDone_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void frmIncarnate_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                _myParent.ibIncarnatePowersEx.ToggleState = ImageButtonEx.States.ToggledOff;
            }

            if (DialogResult == DialogResult.Cancel)
            {
                _myParent.ibIncarnatePowersEx.ToggleState = ImageButtonEx.States.ToggledOff;
            }
        }

        private void ChangedScrollFrameContents()
        {
            VScrollBar1.Value = 0;
            VScrollBar1.Maximum = (int)Math.Round(PopInfo.lHeight * (VScrollBar1.LargeChange / (double)Panel1.Height));
            VScrollBar1_Scroll(VScrollBar1, new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        private void FillLists(string setName)
        {
            var subPowers =
                _myPowers == null
                    ? []
                    : _myPowers
                        .Where(e => e != null &&
                                    !e.FullName.Contains("_silent", StringComparison.InvariantCultureIgnoreCase) &&
                                    e.FullName.StartsWith($"Incarnate.{setName.Replace(' ', '_')}."))
                        .ToArray();

            SkPairedList1.SuspendRedraw = true;
            SkPairedList1.ClearItems();

            foreach (var p in subPowers)
            {
                var state = MidsContext.Character?.CurrentBuild != null &&
                                   !MidsContext.Character.CurrentBuild.PowerUsed(p)
                    ? p.DisplayName != "Nothing"
                        ? EItemState.Enabled
                        : EItemState.Disabled
                    : EItemState.Selected;

                var item = new SkListItem(p.DisplayName, state, -1, -1, -1, p.FullName, EFontFlags.Bold)
                {
                    Bold = MidsContext.Config.RtFont.PairedBold
                };

                if (item.ItemState == EItemState.Invalid)
                {
                    item.Italic = true;
                }

                SkPairedList1.AddItem(item);
            }

            SkPairedList1.SuspendRedraw = false;
            SkPairedList1.Invalidate();
        }

        private void LblLock_Click(object? sender, EventArgs e)
        {
            _locked = false;
            LblLock.Visible = false;
        }

        private void MiniPowerInfo(int pIdx)
        {
            if (_locked)
            {
                return;
            }

            var iPopup = new PopUp.PopupData();
            if (pIdx < 0)
            {
                PopInfo.SetPopup(iPopup);
                ChangedScrollFrameContents();

                return;
            }
            
            var power = new Power(_myPowers[pIdx]);
            power.AbsorbPetEffects();
            power.ApplyGrantPowerEffects();
            var index1 = iPopup.Add();
            var powerTypeSuffix = power.PowerType switch
            {
                Enums.ePowerType.Click when power.ClickBuff => "(Click)",
                Enums.ePowerType.Auto_ => "(Auto)",
                Enums.ePowerType.Toggle => "(Toggle)",
                _ => ""
            };

            iPopup.Sections?[index1].Add(power.DisplayName, PopUp.Colors.Title);
            iPopup.Sections?[index1].Add($"{powerTypeSuffix} {power.DescShort}", PopUp.Colors.Text, 0.9f);
            var powerLongDesc = power.DescLong.Replace("\0", "").Replace("<br>", "\r\n");
            iPopup.Sections?[index1].Add($"{powerTypeSuffix} {powerLongDesc}", PopUp.Colors.Common, 1f, FontStyle.Regular);
            var index2 = iPopup.Add();
            if (power.EndCost > 0)
            {
                iPopup.Sections?[index2].Add("End Cost:", PopUp.Colors.Title,
                    power.ActivatePeriod > 0
                        ? $"{Utilities.FixDP(power.EndCost / power.ActivatePeriod)}/s"
                        : Utilities.FixDP(power.EndCost), PopUp.Colors.Title, 0.9f,
                    FontStyle.Bold, 1);
            }

            if ((power.EntitiesAutoHit == Enums.eEntity.None) | ((power.Range > 20) & power.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt)))
            {
                iPopup.Sections?[index2].Add("Accuracy:", PopUp.Colors.Title,
                    $"{Utilities.FixDP(MidsContext.Config.ScalingToHit * power.Accuracy * 100)}%",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power.RechargeTime > 0)
            {
                iPopup.Sections?[index2].Add("Recharge:", PopUp.Colors.Title,
                    $"{Utilities.FixDP(power.RechargeTime)}s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            var durationEffectId = power.GetDurationEffectID();
            var durationValue = durationEffectId > -1
                ? power.Effects[durationEffectId].Duration
                : 0;
                
            if ((power.PowerType != Enums.ePowerType.Toggle) & (power.PowerType != Enums.ePowerType.Auto_) && durationValue > 0)
            {
                iPopup.Sections?[index2].Add("Duration:", PopUp.Colors.Title, $"{Utilities.FixDP(durationValue)}s",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power.Range > 0)
            {
                iPopup.Sections?[index2].Add("Range:", PopUp.Colors.Title, $"{Utilities.FixDP(power.Range)}ft",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power.Arc > 0)
            {
                iPopup.Sections?[index2].Add("Arc:", PopUp.Colors.Title, $"{Convert.ToString(power.Arc)}°",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }
            else if (power.Radius > 0)
            {
                iPopup.Sections?[index2].Add("Radius:", PopUp.Colors.Title,
                    $"{Convert.ToString(power.Radius, CultureInfo.InvariantCulture)}ft", PopUp.Colors.Title, 0.9f,
                    FontStyle.Bold, 1);
            }

            if (power.CastTime > 0)
            {
                iPopup.Sections?[index2].Add("Cast Time:", PopUp.Colors.Title,
                    $"{Utilities.FixDP(power.CastTime)}s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power.Effects.Length > 0)
            {
                iPopup.Sections?[index2].Add("Effects:", PopUp.Colors.Title);
                foreach (var fx in power.Effects)
                {
                    if (!(((fx.EffectType != Enums.eEffectType.GrantPower) | fx.Absorbed_Effect) &
                          (fx.EffectType != Enums.eEffectType.RevokePower) &
                          (fx.EffectType != Enums.eEffectType.SetMode)))
                    {
                        continue;
                    }

                    var index4 = iPopup.Add();
                    fx.SetPower(power);
                    var strArray = fx.BuildEffectString(false, "", false, false, false, true)
                        .Replace("[", "\r\n")
                        .Replace("\r\n", "^")
                        .Replace("  ", "")
                        .Replace("]", "")
                        .Split('^');
                    for (var index5 = 0; index5 < strArray.Length; index5++)
                    {
                        if (index5 == 0)
                        {
                            iPopup.Sections?[index4].Add(strArray[index5], PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                        }
                        else
                        {
                            iPopup.Sections?[index4].Add(strArray[index5], PopUp.Colors.Disabled, 0.9f, FontStyle.Italic, 2);
                        }
                    }
                }
            }

            PopInfo.SetPopup(iPopup);
            ChangedScrollFrameContents();
        }

        private void PopInfo_MouseEnter(object? sender, EventArgs e)

        {
            if (!ContainsFocus)
            {
                return;
            }

            VScrollBar1.Focus();
        }

        private void PopInfo_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                if (VScrollBar1.Value + VScrollBar1.LargeChange <= VScrollBar1.Maximum)
                {
                    VScrollBar1.Value += VScrollBar1.LargeChange;
                }
            }
            else if (VScrollBar1.Value - VScrollBar1.LargeChange >= VScrollBar1.Minimum)
            {
                VScrollBar1.Value -= VScrollBar1.LargeChange;
            }

            VScrollBar1_Scroll(RuntimeHelpers.GetObjectValue(sender), new ScrollEventArgs(ScrollEventType.EndScroll, VScrollBar1.Value));
        }
        
        private void VScrollBar1_Scroll(object? sender, ScrollEventArgs e)
        {
            PopInfo.ScrollY = (PopInfo.lHeight > Panel1.Height) & (VScrollBar1.Maximum > VScrollBar1.LargeChange)
                ? VScrollBar1.Value / (float)(VScrollBar1.Maximum - VScrollBar1.LargeChange) * (PopInfo.lHeight - Panel1.Height)
                : 0;
        }

        private void SkPairedList1_ItemClick(SkListItem item, MouseButtons button)
        {
            if (button == MouseButtons.Right)
            {
                _locked = false;
                MiniPowerInfo(item.Index);
                LblLock.Visible = true;
                _locked = true;
                
                return;
            }

            if (item.ItemState == EItemState.Disabled)
            {
                return;
            }

            var powerUsed = MidsContext.Character?.CurrentBuild?.PowerUsed(_myPowers[item.Index]) == true;
            SkPairedList1.SuspendRedraw = true;
            for (var i = 0; i < SkPairedList1.Items.Length; i++)
            {
                if (i != item.Index)
                {
                    SkPairedList1.Items[i].ItemState = _myPowers[i].DisplayName != "Nothing"
                        ? EItemState.Enabled
                        : EItemState.Disabled;

                    continue;
                }

                SkPairedList1.Items[i].ItemState = powerUsed ? EItemState.Enabled : EItemState.Selected;
                if (powerUsed)
                {
                    MidsContext.Character?.CurrentBuild?.RemovePower(_myPowers[i]);
                }
                else
                {
                    MidsContext.Character.CurrentBuild.AddPower(_myPowers[item.Index], 49).StatInclude = true;
                }
            }

            SkPairedList1.SuspendRedraw = false;
            SkPairedList1.Invalidate();

            _myParent.PowerModified(true);
            _myParent.DoRefresh();
        }

        private void SkPairedList1_ItemHover(SkListItem item)
        {
            MiniPowerInfo(item.Index);
        }

        private void SkPairedList1_EmptyHover()
        {
            MiniPowerInfo(-1);
        }

        private void SkPairedList1_MouseLeave(object sender, EventArgs e)
        {
            MiniPowerInfo(-1);
        }

        public class CustomPanel : Panel
        {
            protected override Point ScrollToControl(Control activeControl)
            {
                return DisplayRectangle.Location;
            }
        }
    }
}
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
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    public partial class frmPrestige : Form
    {
        private readonly MainWindow2 _myParent;
        private bool _locked;
        private List<IPower?> _myPowers;

        public frmPrestige(MainWindow2 parentForm, List<IPower?> powers)
        {
            Location = new Point(Location.X, Location.Y - 100);
            _locked = false;
            InitializeComponent();
            //var componentResourceManager = new ComponentResourceManager(typeof(frmPrestige));
            Icon = Resources.MRB_Icon_Concept;
            Name = nameof(frmPrestige);
            _myParent = parentForm;
            _myPowers = powers;
        }

        private void frmPrestige_Load(object? sender, EventArgs e)
        {
            CenterToParent();
            UpdateColorTheme();

            var iPopup = new PopUp.PopupData();
            var index = iPopup.Add();
            iPopup.Sections[index].Add("Click powers to enable/disable them.", PopUp.Colors.Title);
            iPopup.Sections[index]
                .Add("Powers in gray (or your custom 'power disabled' color) cannot be included in your stats.",
                    PopUp.Colors.Text, 0.9f);
            PopInfo.SetPopup(iPopup);
            ChangedScrollFrameContents();
            FillLists();
        }

        private void frmPrestige_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                _myParent.ibPrestigePowersEx.ToggleState = ImageButtonEx.States.ToggledOff;
            }

            if (DialogResult == DialogResult.Cancel)
            {
                _myParent.ibPrestigePowersEx.ToggleState = ImageButtonEx.States.ToggledOff;
            }
        }

        public void UpdateColorTheme()
        {
            SetButtonColors();
            SetListColors();
            UpdateFonts();
        }

        public void UpdateColorTheme(Enums.Alignment alignment)
        {
            SetButtonColors(alignment);
            SetListColors(alignment);
        }

        private void SetButtonColors(Enums.Alignment? alignment = null)
        {
            BtnDone.UseAlt = alignment == null
                ? !MidsContext.Character.IsHero() // Automatic
                : alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist; // Manual, parameter-driven
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
            SetListColors();

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
            SetListColors();

            SkPairedList1.Font = new Font(SkPairedList1.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase,
                !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold, GraphicsUnit.Point);
            foreach (var e in SkPairedList1.Items)
            {
                e.Bold = MidsContext.Config.RtFont.PowersSelectBold;
            }
        }

        private void ChangedScrollFrameContents()
        {
            VScrollBar1.Value = 0;
            VScrollBar1.Maximum = (int)Math.Round(PopInfo.lHeight * (VScrollBar1.LargeChange / (double) Panel1.Height));
            VScrollBar1_Scroll(VScrollBar1, new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        private void FillLists()
        {
            _myPowers = _myPowers.OrderBy(x => x?.DisplayName).ToList();
            Debug.WriteLine($"FillLists(): {_myPowers.Count} powers");
            SkPairedList1.SuspendRedraw = true;
            SkPairedList1.ClearItems();
            var message = string.Empty; // Has to be initialized first
            foreach (var p in _myPowers)
            {
                if (p == null)
                {
                    continue;
                }

                var item = new SkListItem(p.DisplayName, MainModule.MidsController.Toon.SkPowerState(p.PowerIndex, ref message), -1, -1, p.PowerIndex, p.FullName, EFontFlags.Bold)
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

        private void lblLock_Click(object sender, EventArgs e)
        {
            _locked = false;
            lblLock.Visible = false;
        }
        
        private void MiniPowerInfo(int pIDX)
        {
            if (_locked)
            {
                return;
            }

            var iPopup = new PopUp.PopupData();
            if (pIDX < 0)
            {
                PopInfo.SetPopup(iPopup);
                ChangedScrollFrameContents();

                return;
            }
            
            var power1 = new Power(_myPowers[pIDX]);
            var index1 = iPopup.Add();
            var str = power1.PowerType switch
            {
                Enums.ePowerType.Click when power1.ClickBuff => "(Click)",
                Enums.ePowerType.Auto_ => "(Auto)",
                Enums.ePowerType.Toggle => "(Toggle)",
                _ => string.Empty
            };

            iPopup.Sections[index1].Add(power1.DisplayName, PopUp.Colors.Title);
            iPopup.Sections[index1].Add($"{str} {power1.DescShort}", PopUp.Colors.Text, 0.9f);
            var index2 = iPopup.Add();
            if (power1.EndCost > 0)
            {
                iPopup.Sections[index2].Add("End Cost:", PopUp.Colors.Title,
                    power1.ActivatePeriod > 0
                        ? $"{Utilities.FixDP(power1.EndCost / power1.ActivatePeriod)}/s"
                        : Utilities.FixDP(power1.EndCost), PopUp.Colors.Title, 0.9f,
                    FontStyle.Bold, 1);
            }

            if ((power1.EntitiesAutoHit == Enums.eEntity.None) | ((power1.Range > 20) & power1.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt)))
            {
                iPopup.Sections[index2].Add("Accuracy:", PopUp.Colors.Title,
                    $"{Utilities.FixDP(MidsContext.Config.ScalingToHit * power1.Accuracy * 100)}%",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power1.RechargeTime > 0)
            {
                iPopup.Sections[index2].Add("Recharge:", PopUp.Colors.Title,
                    $"{Utilities.FixDP(power1.RechargeTime)}s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            var durationEffectId = power1.GetDurationEffectID();
            var iNum = 0f;
            if (durationEffectId > -1)
            {
                iNum = power1.Effects[durationEffectId].Duration;
            }

            if ((power1.PowerType != Enums.ePowerType.Toggle) & (power1.PowerType != Enums.ePowerType.Auto_) && iNum > 0)
            {
                iPopup.Sections[index2].Add("Duration:", PopUp.Colors.Title, $"{Utilities.FixDP(iNum)}s",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power1.Range > 0)
            {
                iPopup.Sections[index2].Add("Range:", PopUp.Colors.Title, $"{Utilities.FixDP(power1.Range)}ft",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power1.Arc > 0)
            {
                iPopup.Sections[index2].Add("Arc:", PopUp.Colors.Title, $"{power1.Arc}°",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }
            else if (power1.Radius > 0)
            {
                iPopup.Sections[index2].Add("Radius:", PopUp.Colors.Title,
                    $"{power1.Radius}ft", PopUp.Colors.Title, 0.9f,
                    FontStyle.Bold, 1);
            }

            if (power1.CastTime > 0)
            {
                iPopup.Sections[index2].Add("Cast Time:", PopUp.Colors.Title,
                    $"{Utilities.FixDP(power1.CastTime)}s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power1.Effects.Length > 0)
            {
                iPopup.Sections[index2].Add("Effects:", PopUp.Colors.Title);
                foreach (var fx in power1.Effects)
                {
                    var index4 = iPopup.Add();
                    fx.SetPower(power1);
                    var strArray = fx.BuildEffectString()
                        .Replace("[", "\r\n")
                        .Replace("\r\n", "^")
                        .Replace("  ", string.Empty)
                        .Replace("]", string.Empty)
                        .Split('^');
                    for (var index5 = 0; index5 < strArray.Length; index5++)
                    {
                        if (index5 == 0)
                        {
                            iPopup.Sections[index4].Add(strArray[index5], PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                        }
                        else
                        {
                            iPopup.Sections[index4].Add(strArray[index5], PopUp.Colors.Disabled, 0.9f, FontStyle.Italic, 2);
                        }
                    }
                }
            }

            PopInfo.SetPopup(iPopup);
            ChangedScrollFrameContents();
        }

        private void PopInfo_MouseEnter(object sender, EventArgs e)
        {
            if (!ContainsFocus)
            {
                return;
            }

            VScrollBar1.Focus();
        }

        private void PopInfo_MouseWheel(object sender, MouseEventArgs e)
        {
            // var ConVal = Convert.ToInt32(Operators.AddObject(VScrollBar1.Value, Interaction.IIf(e.Delta > 0, -1, 1)));
            // if (ConVal != -1)
            // {
            //     VScrollBar1.Value =
            //         Convert.ToInt32(Operators.AddObject(VScrollBar1.Value, Interaction.IIf(e.Delta > 0, -1, 1)));
            //     if (VScrollBar1.Value > VScrollBar1.Maximum - 9)
            //         VScrollBar1.Value = VScrollBar1.Maximum - 9;
            //     VScrollBar1_Scroll(RuntimeHelpers.GetObjectValue(sender),
            //         new ScrollEventArgs(ScrollEventType.EndScroll, 0));
            // }
        }

        private void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (VScrollBar1.Value == -1)
            {
                return;
            }

            PopInfo.ScrollY = VScrollBar1.Value / (float) (VScrollBar1.Maximum - VScrollBar1.LargeChange) * (PopInfo.lHeight - Panel1.Height);
        }

        private void SkPairedList1_ItemClick(SkListItem item, MouseButtons button)
        {
            if (button == MouseButtons.Right)
            {
                _locked = false;
                MiniPowerInfo(item.Index);
                lblLock.Visible = true;
                _locked = true;

                return;
            }

            if (item.ItemState == EItemState.Disabled)
            {
                return;
            }

            if (MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[item.Index]))
            {
                MidsContext.Character.CurrentBuild.RemovePower(_myPowers[item.Index]);
                item.ItemState = EItemState.Enabled;
            }
            else
            {
                // Toggle on if (any):
                // - Toggle default is ON
                // - Is a click buff
                // - Is an auto
                var pToggled =
                    (_myPowers[item.Index].AlwaysToggle & (_myPowers[item.Index].PowerType == Enums.ePowerType.Toggle)) |
                    _myPowers[item.Index].ClickBuff |
                    (_myPowers[item.Index].PowerType == Enums.ePowerType.Auto_);
                var p = MidsContext.Character.CurrentBuild.AddPower(_myPowers[item.Index], 0);
                // Get power index in build powers' list
                var hIdPower = MidsContext.Character.CurrentBuild.Powers.FindIndex(e => e is { Power: not null } && e.Power.StaticIndex == p.Power?.StaticIndex);
                // Check for mutexes
                var eMutex = MainModule.MidsController.Toon.CurrentBuild.MutexV2(hIdPower);
                MidsContext.Character.CurrentBuild.Powers[hIdPower].StatInclude = (eMutex == Enums.eMutex.NoConflict) | (eMutex == Enums.eMutex.NoGroup) && pToggled;

                item.ItemState = EItemState.Selected;
            }

            SkPairedList1.Invalidate();

            _myParent.PowerModified(false);
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

        private void BtnDone_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
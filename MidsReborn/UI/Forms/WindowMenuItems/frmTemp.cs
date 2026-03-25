using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Skia;
using MRBResourceLib;

namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    public partial class frmTemp : Form
    {
        private enum ItemType
        {
            Any,
            Encounters,
            BaseBuffs,
            DayJobs,
            Misc
        }

        private readonly MainWindow2 _myParent;
        private bool _locked;
        private readonly List<IPower?> _myPowers;
        private ItemType _itemFilter;

        public frmTemp(MainWindow2 parentForm, List<IPower?> powersList)
        {
            InitializeComponent();
            _locked = false;
            Icon = Resources.MRB_Icon_Concept;
            Name = nameof(frmTemp);
            _myParent = parentForm;
            _myPowers = powersList;
            _myPowers = _myPowers.OrderBy(x => x?.DisplayName).ToList();
            _itemFilter = ItemType.Any;
        }

        private void frmTemp_Load(object sender, EventArgs e)
        {
            // Bug: StartPosition doesn't work, if called from constructor (old way)
            CenterToParent();
            // PopInfo.ForeColor = BackColor; // ??

            UpdateColorTheme();
            UpdateButtons();

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

        private void UpdateButtons()
        {
            BtnTypeAll.ToggleState = _itemFilter == ItemType.Any
                ? ImageButtonEx.States.ToggledOn
                : ImageButtonEx.States.ToggledOff;
            BtnTypeAll.Invalidate();

            BtnTypeEncounters.ToggleState = _itemFilter == ItemType.Encounters
                ? ImageButtonEx.States.ToggledOn
                : ImageButtonEx.States.ToggledOff;
            BtnTypeEncounters.Invalidate();

            BtnTypeBaseBuffs.ToggleState = _itemFilter == ItemType.BaseBuffs
                ? ImageButtonEx.States.ToggledOn
                : ImageButtonEx.States.ToggledOff;
            BtnTypeBaseBuffs.Invalidate();

            BtnTypeDayJobs.ToggleState = _itemFilter == ItemType.DayJobs
                ? ImageButtonEx.States.ToggledOn
                : ImageButtonEx.States.ToggledOff;
            BtnTypeDayJobs.Invalidate();

            BtnTypeMisc.ToggleState = _itemFilter == ItemType.Misc
                ? ImageButtonEx.States.ToggledOn
                : ImageButtonEx.States.ToggledOff;
            BtnTypeMisc.Invalidate();
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
            VScrollBar1.Maximum = (int)Math.Round(PopInfo.lHeight * (VScrollBar1.LargeChange / (double)Panel1.Height));
            VScrollBar1_Scroll(VScrollBar1, new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        private void FillLists()
        {
            string[] filterPowers = _itemFilter switch
            {
                ItemType.Encounters => [
                    "Temporary_Powers.Temporary_Powers.Anger_Monument",
                    "Temporary_Powers.Temporary_Powers.Warburg_Biological",
                    "Temporary_Powers.Temporary_Powers.Crystal_of_Resilience",
                    "Temporary_Powers.Temporary_Powers.Mayhem_Increase_Movement_Speed",
                    "Temporary_Powers.Temporary_Powers.Mayhem_Increase_Endurance",
                    "Temporary_Powers.Temporary_Powers.Mayhem_Increase_Health",
                    "Temporary_Powers.Temporary_Powers.Heart_of_a_Storm_Elemental",
                    "Temporary_Powers.Temporary_Powers.Mayhem_Flight_Pack",
                    "Temporary_Powers.Temporary_Powers.The_Perfect_Eye",
                    "Temporary_Powers.Temporary_Powers.The_True_Furnace",
                    "Temporary_Powers.Temporary_Powers.Wedding_Band",
                    "Temporary_Powers.Temporary_Powers.Wedding_Band_FB",
                    "Temporary_Powers.Temporary_Powers.Mayhem_Low-G_Pack"
                ],

                ItemType.BaseBuffs => [
                    "Temporary_Powers.Temporary_Powers.Increase_Run_Speed",
                    "Temporary_Powers.Temporary_Powers.Knockback_Increase",
                    "Temporary_Powers.Temporary_Powers.Knockback_Protection",
                    "Temporary_Powers.Temporary_Powers.Lethal_Resistance",
                    "Temporary_Powers.Temporary_Powers.Confusion_Resistance",
                    "Temporary_Powers.Temporary_Powers.Fear_Resistance",
                    "Temporary_Powers.Temporary_Powers.Fire_Resistance",
                    "Temporary_Powers.Temporary_Powers.Sleep_Resistance",
                    "Temporary_Powers.Temporary_Powers.Slow_Resistance",
                    "Temporary_Powers.Temporary_Powers.Smashing_Resistance",
                    "Temporary_Powers.Temporary_Powers.Increase_Attack_Speed",
                    "Temporary_Powers.Temporary_Powers.Increase_Flight_Speed",
                    "Temporary_Powers.Temporary_Powers.Increase_Jump_Speed",
                    "Temporary_Powers.Temporary_Powers.Increase_Perception",
                    "Temporary_Powers.Temporary_Powers.Increase_Recovery"
                ],

                ItemType.Misc => [
                    "Temporary_Powers.Temporary_Powers.Alt_Defense_Amplifier_1_hour",
                    "Temporary_Powers.Temporary_Powers.Alt_Offense_Amplifier_1_hour",
                    "Temporary_Powers.Temporary_Powers.GRHeroAlignmentPower", // Call to Justice
                    "Temporary_Powers.Temporary_Powers.GRVillainAlignmentPower", // Frenzy
                    "Temporary_Powers.Temporary_Powers.Mayhem_Store_Kinetic_Shield",
                    "Temporary_Powers.Temporary_Powers.Cryonite_Armor",
                    "Temporary_Powers.Temporary_Powers.Grant_Invisibility",
                    "Temporary_Powers.Temporary_Powers.Alt_Survival_Amplifier_1_hour",
                    "Temporary_Powers.Temporary_Powers.Soulbound_Armor"
                ],

                _ => []
            };

            SkPairedList1.SuspendRedraw = true;
            SkPairedList1.ClearItems();
            var message = string.Empty; // Has to be initialized first
            
            foreach (var p in _myPowers)
            {
                if (p == null)
                {
                    continue;
                }

                if (_itemFilter == ItemType.DayJobs)
                {
                    if (!p.FullName.StartsWith("Temporary_Powers.Day_Job_Powers."))
                    {
                        continue;
                    }
                }
                else if (filterPowers.Length > 0)
                {
                    if (!filterPowers.Contains(p.FullName))
                    {
                        continue;
                    }
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
            var useAlt = alignment == null
                ? MidsContext.Character?.IsHero() == false // Automatic
                : alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist; // Manual, parameter-driven

            ibClose.UseAlt = useAlt;
            BtnTypeAll.UseAlt = useAlt;
            BtnTypeEncounters.UseAlt = useAlt;
            BtnTypeBaseBuffs.UseAlt = useAlt;
            BtnTypeDayJobs.UseAlt = useAlt;
            BtnTypeMisc.UseAlt = useAlt;
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

        private void ibClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lblLock_Click(object sender, EventArgs e)
        {
            _locked = false;
            lblLock.Visible = false;
        }

        private void SkPairedList1_ItemClick(SkListItem item, MouseButtons button)
        {
            if (button == MouseButtons.Right)
            {
                _locked = false;
                MiniPowerInfo(item.Tag);
                lblLock.Visible = true;
                _locked = true;

                return;
            }

            if (item.ItemState == EItemState.Disabled)
            {
                return;
            }

            var itemIndex = GetPowerIndexFromName(item.Tag);
            if (itemIndex < 0)
            {
                return;
            }

            if (MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[itemIndex]))
            {
                MidsContext.Character.CurrentBuild.RemovePower(_myPowers[itemIndex]);
                item.ItemState = EItemState.Enabled;
            }
            else
            {
                MidsContext.Character.CurrentBuild.AddPower(_myPowers[itemIndex], 0).StatInclude = true;
                item.ItemState = EItemState.Selected;
            }

            SkPairedList1.Invalidate();

            _myParent.PowerModified(false);
            _myParent.DoRefresh();
        }

        private void SkPairedList1_ItemHover(SkListItem item)
        {
            MiniPowerInfo(item.Tag);
        }

        private void SkPairedList1_EmptyHover()
        {
            // Can be enabled if padding between items starts becoming large.
            // As the default value is of 2px, this is disabled to reduce flickering.
            //MiniPowerInfo(-1);
        }

        private void SkPairedList1_MouseLeave(object sender, EventArgs e)
        {
            MiniPowerInfo(-1);
        }

        private int GetPowerIndexFromName(string powerFullName)
        {
            return _myPowers.TryFindIndex(e => e?.FullName == powerFullName);
        }

        private void MiniPowerInfo(string powerFullName)
        {
            MiniPowerInfo(GetPowerIndexFromName(powerFullName));
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

            var power1 = new Power(_myPowers[pIdx]);
            var index1 = iPopup.Add();
            var pwType = power1.PowerType switch
            {
                Enums.ePowerType.Click when power1.ClickBuff => "(Click)",
                Enums.ePowerType.Auto_ => "(Auto)",
                Enums.ePowerType.Toggle => "(Toggle)",
                _ => ""
            };

            iPopup.Sections?[index1].Add(power1.DisplayName, PopUp.Colors.Title);
            iPopup.Sections?[index1].Add($"{pwType} {power1.DescShort}", PopUp.Colors.Text, 0.9f);
            var index2 = iPopup.Add();
            if (power1.EndCost > 0)
            {
                iPopup.Sections?[index2].Add("End Cost:", PopUp.Colors.Title,
                    power1.ActivatePeriod > 0
                        ? $"{Utilities.FixDP(power1.EndCost / power1.ActivatePeriod)}/s"
                        : Utilities.FixDP(power1.EndCost), PopUp.Colors.Title, 0.9f,
                    FontStyle.Bold, 1);
            }

            if ((power1.EntitiesAutoHit == Enums.eEntity.None) | ((power1.Range > 20) & power1.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt)))
            {
                iPopup.Sections?[index2].Add("Accuracy:", PopUp.Colors.Title,
                    $"{Utilities.FixDP(MidsContext.Config.ScalingToHit * power1.Accuracy * 100)}%",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power1.RechargeTime > 0)
            {
                iPopup.Sections?[index2].Add("Recharge:", PopUp.Colors.Title,
                    $"{Utilities.FixDP(power1.RechargeTime)}s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            var durationEffectId = power1.GetDurationEffectID();
            var pwDuration = durationEffectId > -1 ? power1.Effects[durationEffectId].Duration : 0;

            if ((power1.PowerType != Enums.ePowerType.Toggle) & (power1.PowerType != Enums.ePowerType.Auto_) && pwDuration > 0)
            {
                iPopup.Sections?[index2].Add("Duration:", PopUp.Colors.Title, $"{Utilities.FixDP(pwDuration)}s",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power1.Range > 0)
            {
                iPopup.Sections?[index2].Add("Range:", PopUp.Colors.Title, $"{Utilities.FixDP(power1.Range)}ft",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power1.Arc > 0)
            {
                iPopup.Sections?[index2].Add("Arc:", PopUp.Colors.Title, $"{Convert.ToString(power1.Arc)}°",
                    PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }
            else if (power1.Radius > 0)
            {
                iPopup.Sections?[index2].Add("Radius:", PopUp.Colors.Title,
                    $"{Convert.ToString(power1.Radius, CultureInfo.InvariantCulture)}ft", PopUp.Colors.Title, 0.9f,
                    FontStyle.Bold, 1);
            }

            if (power1.CastTime > 0)
            {
                iPopup.Sections?[index2].Add("Cast Time:", PopUp.Colors.Title,
                    $"{Utilities.FixDP(power1.CastTime)}s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            if (power1.Effects.Length > 0)
            {
                iPopup.Sections?[index2].Add("Effects:", PopUp.Colors.Title);
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

            PopInfo.ScrollY = VScrollBar1.Value / (float)(VScrollBar1.Maximum - VScrollBar1.LargeChange) * (PopInfo.lHeight - Panel1.Height);
        }

        private void BtnTypeAll_Click(object sender, EventArgs e)
        {
            if (_itemFilter == ItemType.Any)
            {
                return;
            }

            _itemFilter = ItemType.Any;
            UpdateButtons();
            FillLists();
        }

        private void BtnTypeEncounters_Click(object sender, EventArgs e)
        {
            if (_itemFilter == ItemType.Encounters)
            {
                return;
            }

            _itemFilter = ItemType.Encounters;
            UpdateButtons();
            FillLists();
        }

        private void BtnTypeBaseBuffs_Click(object sender, EventArgs e)
        {
            if (_itemFilter == ItemType.BaseBuffs)
            {
                return;
            }

            _itemFilter = ItemType.BaseBuffs;
            UpdateButtons();
            FillLists();
        }

        private void BtnTypeDayJobs_Click(object sender, EventArgs e)
        {
            if (_itemFilter == ItemType.DayJobs)
            {
                return;
            }

            _itemFilter = ItemType.DayJobs;
            UpdateButtons();
            FillLists();
        }

        private void BtnTypeMisc_Click(object sender, EventArgs e)
        {
            if (_itemFilter == ItemType.Misc)
            {
                return;
            }

            _itemFilter = ItemType.Misc;
            UpdateButtons();
            FillLists();
        }
    }
}
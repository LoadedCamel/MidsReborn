using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmPrestige : Form
    {
        private readonly frmMain _myParent;

        private bool _locked;

        private List<IPower?> _myPowers;
        private ImageButton ibClose;

        private Label lblLock;
        private ListLabel llLeft;
        private ListLabel llRight;
        private Panel Panel1;

        private FrmIncarnate.CustomPanel Panel2;

        private ctlPopUp PopInfo;

        private VScrollBar VScrollBar1;

        public frmPrestige(frmMain iParent, List<IPower?> iPowers)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            CenterToParent();
            Location = new Point(Location.X, Location.Y - 100);
            Load += frmPrestige_Load;
            _locked = false;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmPrestige));
            Icon = Resources.MRB_Icon_Concept;
            Name = nameof(frmPrestige);
            _myParent = iParent;
            _myPowers = iPowers;
            FormClosing += FrmPrestige_FormClosing;
        }

        private void FrmPrestige_FormClosing(object? sender, FormClosingEventArgs e)
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

        public void UpdateFonts(Font font)
        {
            foreach (var llControl in Controls.OfType<ListLabel>())
            {
                llControl.SuspendRedraw = true;
                llControl.UpdateTextColors(ListLabel.LlItemState.Enabled,
                    MidsContext.Config.RtFont.ColorPowerAvailable);
                llControl.UpdateTextColors(ListLabel.LlItemState.Disabled,
                    MidsContext.Config.RtFont.ColorPowerDisabled);
                llControl.UpdateTextColors(ListLabel.LlItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
                llControl.ScrollBarColor = MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenHero
                    : MidsContext.Config.RtFont.ColorPowerTakenVillain;
                llControl.ScrollButtonColor = MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
                llControl.UpdateTextColors(ListLabel.LlItemState.Selected,
                    MidsContext.Character.IsHero()
                        ? MidsContext.Config.RtFont.ColorPowerTakenHero
                        : MidsContext.Config.RtFont.ColorPowerTakenVillain);
                llControl.UpdateTextColors(ListLabel.LlItemState.SelectedDisabled,
                    MidsContext.Character.IsHero()
                        ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                        : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
                llControl.HoverColor = MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                    : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
                var style = !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold;
                llControl.Font = new Font(llControl.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase, style, GraphicsUnit.Point);
                foreach (var e in llControl.Items)
                {
                    e.Bold = MidsContext.Config.RtFont.PowersSelectBold;
                }
                llControl.SuspendRedraw = false;
                llControl.Refresh();
            }
        }

        private void ChangedScrollFrameContents()
        {
            VScrollBar1.Value = 0;
            VScrollBar1.Maximum =
                (int) Math.Round(PopInfo.lHeight * (VScrollBar1.LargeChange / (double) Panel1.Height));
            VScrollBar1_Scroll(VScrollBar1, new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        private void FillLists()
        {
            _myPowers = _myPowers.OrderBy(x => x.DisplayName).ToList();
            llLeft.SuspendRedraw = true;
            llRight.SuspendRedraw = true;
            llLeft.ClearItems();
            llRight.ClearItems();
            var num = _myPowers.Count - 1;
            for (var index = 0; index <= num; ++index)
            {
                var iState = !MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[index])
                    ? !((_myPowers[index].PowerType != Enums.ePowerType.Click) | _myPowers[index].ClickBuff)
                        ? !_myPowers[index].SubIsAltColor ? ListLabel.LlItemState.Disabled :
                        ListLabel.LlItemState.Invalid
                        : ListLabel.LlItemState.Enabled
                    : ListLabel.LlItemState.Selected;
                var iItem = !MidsContext.Config.RtFont.PairedBold
                    ? new ListLabel.ListLabelItem(_myPowers[index].DisplayName, iState)
                    : new ListLabel.ListLabelItem(_myPowers[index].DisplayName, iState, -1, -1, -1, "",
                        ListLabel.LlFontFlags.Bold);
                if (index >= _myPowers.Count / 2.0)
                    llRight.AddItem(iItem);
                else
                    llLeft.AddItem(iItem);
            }

            llLeft.SuspendRedraw = false;
            llRight.SuspendRedraw = false;
            llLeft.Refresh();
            llRight.Refresh();
        }

        private void frmPrestige_Load(object? sender, EventArgs e)
        {
            CenterToParent();
            BackColor = _myParent.BackColor;
            PopInfo.ForeColor = BackColor;
            var llLeft = this.llLeft;
            UpdateLlColours(ref llLeft);
            this.llLeft = llLeft;
            var llRight = this.llRight;
            UpdateLlColours(ref llRight);
            this.llRight = llRight;
            ibClose.IA = _myParent.Drawing.PImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero()
                ? _myParent.Drawing.BxPower[2].Bitmap
                : _myParent.Drawing.BxPower[4].Bitmap;
            ibClose.ImageOn = MidsContext.Character.IsHero()
                ? _myParent.Drawing.BxPower[3].Bitmap
                : _myParent.Drawing.BxPower[5].Bitmap;
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

        private void ibClose_ButtonClicked()
        {
            Close();
        }

        private void lblLock_Click(object sender, EventArgs e)
        {
            _locked = false;
            lblLock.Visible = false;
        }

        private void llLeft_ItemClick(ListLabel.ListLabelItem Item, MouseButtons Button)
        {
            var pIDX = Item.Index;
            if (Button == MouseButtons.Right)
            {
                _locked = false;
                MiniPowerInfo(Item.Index);
                lblLock.Visible = true;
                _locked = true;
            }
            else
            {
                if (Item.ItemState == ListLabel.LlItemState.Disabled)
                    return;
                if (MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[pIDX]))
                {
                    MidsContext.Character.CurrentBuild.RemovePower(_myPowers[pIDX]);
                    Item.ItemState = ListLabel.LlItemState.Enabled;
                }
                else
                {
                    // Toggle on if (any):
                    // - Toggle default is ON
                    // - Is a click buff
                    // - Is an auto
                    var pToggled =
                        (_myPowers[pIDX].AlwaysToggle & _myPowers[pIDX].PowerType == Enums.ePowerType.Toggle) |
                        _myPowers[pIDX].ClickBuff |
                        _myPowers[pIDX].PowerType == Enums.ePowerType.Auto_;
                    var p = MidsContext.Character.CurrentBuild.AddPower(_myPowers[pIDX]);
                    
                    // Get power index in build powers' list
                    var hIDPower = MidsContext.Character.CurrentBuild.Powers.FindIndex(e => e.Power != null && e.Power.StaticIndex == p.Power.StaticIndex);
                    // Check for mutexes
                    var eMutex = MainModule.MidsController.Toon.CurrentBuild.MutexV2(hIDPower);
                    MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = (eMutex == Enums.eMutex.NoConflict) | (eMutex == Enums.eMutex.NoGroup) && pToggled;
                    
                    Item.ItemState = ListLabel.LlItemState.Selected;
                }

                llLeft.Refresh();
                _myParent.PowerModified(true);
                _myParent.DoRefresh();
            }
        }

        private void llLeft_ItemHover(ListLabel.ListLabelItem Item)
        {
            MiniPowerInfo(Item.Index);
        }

        private void llLeft_MouseEnter(object sender, EventArgs e)
        {
            if (!ContainsFocus)
                return;
            Panel2.Focus();
        }

        private void llRight_ItemClick(ListLabel.ListLabelItem Item, MouseButtons Button)
        {
            var pIDX = Item.Index + llLeft.Items.Length;
            if (Button == MouseButtons.Right)
            {
                _locked = false;
                MiniPowerInfo(pIDX);
                lblLock.Visible = true;
                _locked = true;
            }
            else
            {
                if (Item.ItemState == ListLabel.LlItemState.Disabled)
                    return;
                if (MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[pIDX]))
                {
                    MidsContext.Character.CurrentBuild.RemovePower(_myPowers[pIDX]);
                    Item.ItemState = ListLabel.LlItemState.Enabled;
                }
                else
                {
                    MidsContext.Character.CurrentBuild.AddPower(_myPowers[pIDX]).StatInclude = true;
                    Item.ItemState = ListLabel.LlItemState.Selected;
                }

                llRight.Refresh();
                _myParent.PowerModified(false);
                _myParent.DoRefresh();
            }
        }

        private void llRight_ItemHover(ListLabel.ListLabelItem Item)
        {
            MiniPowerInfo(Item.Index + llLeft.Items.Length);
        }

        private void llRight_MouseEnter(object sender, EventArgs e)
        {
            llLeft_MouseEnter(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void MiniPowerInfo(int pIDX)
        {
            if (_locked)
                return;
            IPower? power1 = new Power(_myPowers[pIDX]);
            var iPopup = new PopUp.PopupData();
            if (pIDX < 0)
            {
                PopInfo.SetPopup(iPopup);
                ChangedScrollFrameContents();
            }
            else
            {
                var index1 = iPopup.Add();
                var str = string.Empty;
                switch (power1.PowerType)
                {
                    case Enums.ePowerType.Click:
                        if (power1.ClickBuff) str = "(Click)";
                        break;
                    case Enums.ePowerType.Auto_:
                        str = "(Auto)";
                        break;
                    case Enums.ePowerType.Toggle:
                        str = "(Toggle)";
                        break;
                }

                iPopup.Sections[index1].Add(power1.DisplayName, PopUp.Colors.Title);
                iPopup.Sections[index1].Add(str + " " + power1.DescShort, PopUp.Colors.Text, 0.9f);
                var index2 = iPopup.Add();
                if (power1.EndCost > 0.0)
                {
                    if (power1.ActivatePeriod > 0.0)
                        iPopup.Sections[index2].Add("End Cost:", PopUp.Colors.Title,
                            Utilities.FixDP(power1.EndCost / power1.ActivatePeriod) + "/s", PopUp.Colors.Title, 0.9f,
                            FontStyle.Bold, 1);
                    else
                        iPopup.Sections[index2].Add("End Cost:", PopUp.Colors.Title, Utilities.FixDP(power1.EndCost),
                            PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                }

                if ((power1.EntitiesAutoHit == Enums.eEntity.None) | ((power1.Range > 20.0) &
                                                                      power1.I9FXPresentP(Enums.eEffectType.Mez,
                                                                          Enums.eMez.Taunt)))
                    iPopup.Sections[index2].Add("Accuracy:", PopUp.Colors.Title,
                        Utilities.FixDP((float) (MidsContext.Config.ScalingToHit * (double) power1.Accuracy * 100.0)) + "%",
                        PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                if (power1.RechargeTime > 0.0)
                    iPopup.Sections[index2].Add("Recharge:", PopUp.Colors.Title,
                        Utilities.FixDP(power1.RechargeTime) + "s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                var durationEffectId = power1.GetDurationEffectID();
                var iNum = 0.0f;
                if (durationEffectId > -1)
                    iNum = power1.Effects[durationEffectId].Duration;
                if ((power1.PowerType != Enums.ePowerType.Toggle) & (power1.PowerType != Enums.ePowerType.Auto_) &&
                    iNum > 0.0)
                    iPopup.Sections[index2].Add("Duration:", PopUp.Colors.Title, Utilities.FixDP(iNum) + "s",
                        PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                if (power1.Range > 0.0)
                    iPopup.Sections[index2].Add("Range:", PopUp.Colors.Title, Utilities.FixDP(power1.Range) + "ft",
                        PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                if (power1.Arc > 0)
                    iPopup.Sections[index2].Add("Arc:", PopUp.Colors.Title, Convert.ToString(power1.Arc) + "°",
                        PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                else if (power1.Radius > 0.0)
                    iPopup.Sections[index2].Add("Radius:", PopUp.Colors.Title,
                        Convert.ToString(power1.Radius, CultureInfo.InvariantCulture) + "ft", PopUp.Colors.Title, 0.9f,
                        FontStyle.Bold, 1);
                if (power1.CastTime > 0.0)
                    iPopup.Sections[index2].Add("Cast Time:", PopUp.Colors.Title,
                        Utilities.FixDP(power1.CastTime) + "s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                var power2 = power1;
                if (power2.Effects.Length > 0)
                {
                    iPopup.Sections[index2].Add("Effects:", PopUp.Colors.Title);
                    char[] chArray = {'^'};
                    var num1 = power2.Effects.Length - 1;
                    for (var index3 = 0; index3 <= num1; ++index3)
                    {
                        var index4 = iPopup.Add();
                        power1.Effects[index3].SetPower(power1);
                        var strArray = power1.Effects[index3].BuildEffectString().Replace("[", "\r\n")
                            .Replace("\r\n", "^").Replace("  ", string.Empty).Replace("]", string.Empty).Split(chArray);
                        var num2 = strArray.Length - 1;
                        for (var index5 = 0; index5 <= num2; ++index5)
                            if (index5 == 0)
                                iPopup.Sections[index4].Add(strArray[index5], PopUp.Colors.Effect, 0.9f, FontStyle.Bold,
                                    1);
                            else
                                iPopup.Sections[index4].Add(strArray[index5], PopUp.Colors.Disabled, 0.9f,
                                    FontStyle.Italic, 2);
                    }
                }

                PopInfo.SetPopup(iPopup);
                ChangedScrollFrameContents();
            }
        }

        private void PopInfo_MouseEnter(object sender, EventArgs e)
        {
            if (!ContainsFocus)
                return;
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

        private static void UpdateLlColours(ref ListLabel iList)
        {
            iList.UpdateTextColors(ListLabel.LlItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            iList.UpdateTextColors(ListLabel.LlItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            iList.UpdateTextColors(ListLabel.LlItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            iList.ScrollBarColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain;
            iList.ScrollButtonColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
            iList.UpdateTextColors(ListLabel.LlItemState.Selected,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenHero
                    : MidsContext.Config.RtFont.ColorPowerTakenVillain);
            iList.UpdateTextColors(ListLabel.LlItemState.SelectedDisabled,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
            iList.HoverColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                : MidsContext.Config.RtFont.ColorPowerHighlightVillain;

            var style = !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold;
            iList.Font = new Font(iList.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase, style, GraphicsUnit.Point);
        }

        private void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (VScrollBar1.Value == -1)
                return;
            PopInfo.ScrollY = (float) (VScrollBar1.Value / (double) (VScrollBar1.Maximum - VScrollBar1.LargeChange) *
                                       (PopInfo.lHeight - (double) Panel1.Height));
        }
    }
}
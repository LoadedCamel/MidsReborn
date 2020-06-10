using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using midsControls;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmAccolade : Form
    {
        ImageButton ibClose;

        Label lblLock;
        ListLabelV3 llLeft;
        ListLabelV3 llRight;

        bool _locked;

        readonly frmMain _myParent;

        readonly List<IPower> _myPowers;
        Panel Panel1;

        ctlPopUp PopInfo;

        VScrollBar VScrollBar1;

        private frmIncarnate.CustomPanel Panel2;

        public frmAccolade(frmMain iParent, List<IPower> iPowers)
        {
            Load += frmAccolade_Load;
            _locked = false;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmAccolade));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmAccolade);
            _myParent = iParent;
            _myPowers = iPowers;
        }

        public void UpdateFonts(Font font)
        {
            var llLeft = this.llLeft;
            var llRight = this.llRight;
            llLeft.SuspendRedraw = true;
            llRight.SuspendRedraw = true;
            llLeft.Font = font;
            llRight.Font = font;
            llLeft.UpdateTextColors(ListLabelV3.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            llLeft.UpdateTextColors(ListLabelV3.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            llLeft.UpdateTextColors(ListLabelV3.LLItemState.Selected, MidsContext.Config.RtFont.ColorPowerTaken);
            llLeft.UpdateTextColors(ListLabelV3.LLItemState.SelectedDisabled, MidsContext.Config.RtFont.ColorPowerTakenDark);
            llLeft.UpdateTextColors(ListLabelV3.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            llLeft.HoverColor = MidsContext.Config.RtFont.ColorPowerHighlight;
            llRight.UpdateTextColors(ListLabelV3.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            llRight.UpdateTextColors(ListLabelV3.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            llRight.UpdateTextColors(ListLabelV3.LLItemState.Selected, MidsContext.Config.RtFont.ColorPowerTaken);
            llRight.UpdateTextColors(ListLabelV3.LLItemState.SelectedDisabled, MidsContext.Config.RtFont.ColorPowerTakenDark);
            llRight.UpdateTextColors(ListLabelV3.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            llRight.HoverColor = MidsContext.Config.RtFont.ColorPowerHighlight;
            for (int index = 0; index <= llLeft.Items.Length - 1; ++index)
                llLeft.Items[index].Bold = MidsContext.Config.RtFont.PairedBold;
            for (int index = 0; index <= llRight.Items.Length - 1; ++index)
                llRight.Items[index].Bold = MidsContext.Config.RtFont.PairedBold;
            llLeft.SuspendRedraw = false;
            llRight.SuspendRedraw = false;
            llLeft.Refresh();
            llRight.Refresh();

        }

        void ChangedScrollFrameContents()
        {
            VScrollBar1.Value = 0;
            VScrollBar1.Maximum = (int)Math.Round(PopInfo.lHeight * (VScrollBar1.LargeChange / (double)Panel1.Height));
            VScrollBar1_Scroll(VScrollBar1, new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        void FillLists()
        {
            llLeft.SuspendRedraw = true;
            llRight.SuspendRedraw = true;
            llLeft.ClearItems();
            llRight.ClearItems();
            int num = _myPowers.Count - 1;
            for (int index = 0; index <= num; ++index)
            {
                ListLabelV3.LLItemState iState = !MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[index]) ? (!(_myPowers[index].PowerType != Enums.ePowerType.Click | _myPowers[index].ClickBuff) ? (!_myPowers[index].SubIsAltColour ? ListLabelV3.LLItemState.Disabled : ListLabelV3.LLItemState.Invalid) : ListLabelV3.LLItemState.Enabled) : ListLabelV3.LLItemState.Selected;
                ListLabelV3.ListLabelItemV3 iItem = !MidsContext.Config.RtFont.PairedBold ? new ListLabelV3.ListLabelItemV3(_myPowers[index].DisplayName, iState) : new ListLabelV3.ListLabelItemV3(_myPowers[index].DisplayName, iState, -1, -1, -1, "", ListLabelV3.LLFontFlags.Bold);
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

        void frmAccolade_Load(object sender, EventArgs e)
        {
            BackColor = _myParent.BackColor;
            PopInfo.ForeColor = BackColor;
            ListLabelV3 llLeft = this.llLeft;
            UpdateLlColours(ref llLeft);
            this.llLeft = llLeft;
            ListLabelV3 llRight = this.llRight;
            UpdateLlColours(ref llRight);
            this.llRight = llRight;
            ibClose.IA = _myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero() ? _myParent.Drawing.bxPower[2].Bitmap : _myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = _myParent.Drawing.bxPower[3].Bitmap;
            PopUp.PopupData iPopup = new PopUp.PopupData();
            int index = iPopup.Add();
            iPopup.Sections[index].Add("Click powers to enable/disable them.", PopUp.Colors.Title);
            iPopup.Sections[index].Add("Powers in gray (or your custom 'power disabled' color) cannot be included in your stats.", PopUp.Colors.Text, 0.9f);
            PopInfo.SetPopup(iPopup);
            ChangedScrollFrameContents();
            FillLists();
        }

        void ibClose_ButtonClicked()
        {
            Close();
        }

        void lblLock_Click(object sender, EventArgs e)
        {
            _locked = false;
            lblLock.Visible = false;
        }

        void llLeft_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            if (Button == MouseButtons.Right)
            {
                _locked = false;
                MiniPowerInfo(Item.Index);
                lblLock.Visible = true;
                _locked = true;
            }
            else
            {
                if (Item.ItemState == ListLabelV3.LLItemState.Disabled)
                    return;
                if (MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[Item.Index]))
                {
                    MidsContext.Character.CurrentBuild.RemovePower(_myPowers[Item.Index]);
                    Item.ItemState = ListLabelV3.LLItemState.Enabled;
                }
                else
                {
                    MidsContext.Character.CurrentBuild.AddPower(_myPowers[Item.Index]).StatInclude = true;
                    Item.ItemState = ListLabelV3.LLItemState.Selected;
                }
                llLeft.Refresh();
                _myParent.PowerModified(markModified: false);
            }
        }

        void llLeft_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            MiniPowerInfo(Item.Index);
        }

        void llLeft_MouseEnter(object sender, EventArgs e)
        {
            if (!ContainsFocus)
                return;
            Panel2.Focus();
        }

        void llRight_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            int pIDX = Item.Index + llLeft.Items.Length;
            if (Button == MouseButtons.Right)
            {
                _locked = false;
                MiniPowerInfo(pIDX);
                lblLock.Visible = true;
                _locked = true;
            }
            else
            {
                if (Item.ItemState == ListLabelV3.LLItemState.Disabled)
                    return;
                if (MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[pIDX]))
                {
                    MidsContext.Character.CurrentBuild.RemovePower(_myPowers[pIDX]);
                    Item.ItemState = ListLabelV3.LLItemState.Enabled;
                }
                else
                {
                    MidsContext.Character.CurrentBuild.AddPower(_myPowers[pIDX]).StatInclude = true;
                    Item.ItemState = ListLabelV3.LLItemState.Selected;
                }
                llRight.Refresh();
                _myParent.PowerModified(markModified: false);
            }
        }

        void llRight_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            MiniPowerInfo(Item.Index + llLeft.Items.Length);
        }

        void llRight_MouseEnter(object sender, EventArgs e)
        {
            llLeft_MouseEnter(RuntimeHelpers.GetObjectValue(sender), e);
        }

        void MiniPowerInfo(int pIDX)
        {
            if (_locked)
                return;
            IPower power1 = new Power(_myPowers[pIDX]);
            PopUp.PopupData iPopup = new PopUp.PopupData();
            if (pIDX < 0)
            {
                PopInfo.SetPopup(iPopup);
                ChangedScrollFrameContents();
            }
            else
            {
                int index1 = iPopup.Add();
                string str = string.Empty;
                switch (power1.PowerType)
                {
                    case Enums.ePowerType.Click:
                        if (power1.ClickBuff)
                        {
                            str = "(Click)";
                        }
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
                int index2 = iPopup.Add();
                if (power1.EndCost > 0.0)
                {
                    if (power1.ActivatePeriod > 0.0)
                        iPopup.Sections[index2].Add("End Cost:", PopUp.Colors.Title, Utilities.FixDP(power1.EndCost / power1.ActivatePeriod) + "/s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                    else
                        iPopup.Sections[index2].Add("End Cost:", PopUp.Colors.Title, Utilities.FixDP(power1.EndCost), PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                }
                if (power1.EntitiesAutoHit == Enums.eEntity.None | power1.Range > 20.0 & power1.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt))
                    iPopup.Sections[index2].Add("Accuracy:", PopUp.Colors.Title, Utilities.FixDP((float)(MidsContext.Config.BaseAcc * (double)power1.Accuracy * 100.0)) + "%", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                if (power1.RechargeTime > 0.0)
                    iPopup.Sections[index2].Add("Recharge:", PopUp.Colors.Title, Utilities.FixDP(power1.RechargeTime) + "s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                int durationEffectId = power1.GetDurationEffectID();
                float iNum = 0.0f;
                if (durationEffectId > -1)
                    iNum = power1.Effects[durationEffectId].Duration;
                if (power1.PowerType != Enums.ePowerType.Toggle & power1.PowerType != Enums.ePowerType.Auto_ && iNum > 0.0)
                    iPopup.Sections[index2].Add("Duration:", PopUp.Colors.Title, Utilities.FixDP(iNum) + "s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                if (power1.Range > 0.0)
                    iPopup.Sections[index2].Add("Range:", PopUp.Colors.Title, Utilities.FixDP(power1.Range) + "ft", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                if (power1.Arc > 0)
                    iPopup.Sections[index2].Add("Arc:", PopUp.Colors.Title, Convert.ToString(power1.Arc) + "°", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                else if (power1.Radius > 0.0)
                    iPopup.Sections[index2].Add("Radius:", PopUp.Colors.Title, Convert.ToString(power1.Radius, CultureInfo.InvariantCulture) + "ft", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                if (power1.CastTime > 0.0)
                    iPopup.Sections[index2].Add("Cast Time:", PopUp.Colors.Title, Utilities.FixDP(power1.CastTime) + "s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                IPower power2 = power1;
                if (power2.Effects.Length > 0)
                {
                    iPopup.Sections[index2].Add("Effects:", PopUp.Colors.Title);
                    char[] chArray = { '^' };
                    int num1 = power2.Effects.Length - 1;
                    for (int index3 = 0; index3 <= num1; ++index3)
                    {
                        int index4 = iPopup.Add();
                        power1.Effects[index3].SetPower(power1);
                        string[] strArray = power1.Effects[index3].BuildEffectString().Replace("[", "\r\n").Replace("\r\n", "^").Replace("  ", string.Empty).Replace("]", string.Empty).Split(chArray);
                        int num2 = strArray.Length - 1;
                        for (int index5 = 0; index5 <= num2; ++index5)
                        {
                            if (index5 == 0)
                                iPopup.Sections[index4].Add(strArray[index5], PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                            else
                                iPopup.Sections[index4].Add(strArray[index5], PopUp.Colors.Disabled, 0.9f, FontStyle.Italic, 2);
                        }
                    }
                }
                PopInfo.SetPopup(iPopup);
                ChangedScrollFrameContents();
            }
        }

        void PopInfo_MouseEnter(object sender, EventArgs e)
        {
            if (!ContainsFocus)
                return;
            VScrollBar1.Focus();
        }

        void PopInfo_MouseWheel(object sender, MouseEventArgs e)
        {
            var ConVal = Convert.ToInt32(Operators.AddObject(VScrollBar1.Value, Interaction.IIf(e.Delta > 0, -1, 1)));
            if (ConVal != -1)
            {
                VScrollBar1.Value = Convert.ToInt32(Operators.AddObject(VScrollBar1.Value, Interaction.IIf(e.Delta > 0, -1, 1)));
                if (VScrollBar1.Value > VScrollBar1.Maximum - 9)
                    VScrollBar1.Value = VScrollBar1.Maximum - 9;
                VScrollBar1_Scroll(RuntimeHelpers.GetObjectValue(sender), new ScrollEventArgs(ScrollEventType.EndScroll, 0));
            }
        }

        static void UpdateLlColours(ref ListLabelV3 iList)
        {
            iList.UpdateTextColors(ListLabelV3.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            iList.UpdateTextColors(ListLabelV3.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            iList.UpdateTextColors(ListLabelV3.LLItemState.Selected, MidsContext.Config.RtFont.ColorPowerTaken);
            iList.UpdateTextColors(ListLabelV3.LLItemState.SelectedDisabled, MidsContext.Config.RtFont.ColorPowerTakenDark);
            iList.UpdateTextColors(ListLabelV3.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            iList.HoverColor = MidsContext.Config.RtFont.ColorPowerHighlight;
            iList.Font = new Font(iList.Font.FontFamily, MidsContext.Config.RtFont.PairedBase, FontStyle.Bold, GraphicsUnit.Point);
        }

        void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (VScrollBar1.Value == -1)
                return;
            PopInfo.ScrollY = (float)(VScrollBar1.Value / (double)(VScrollBar1.Maximum - VScrollBar1.LargeChange) * (PopInfo.lHeight - (double)Panel1.Height));
        }
    }
}
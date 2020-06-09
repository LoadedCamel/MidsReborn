
using System;
using System.ComponentModel;
using System.Diagnostics;
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
    public partial class frmIncarnate : Form
    {
        ImageButton alphaBtn;

        ImageButton destinyBtn;

        ImageButton GenesisButton;

        ImageButton hybridBtn;

        ImageButton ibClose;

        ImageButton interfaceBtn;

        ImageButton judgementBtn;

        Label lblLock;

        ImageButton loreBtn;

        ImageButton OmegaButton;
        Panel Panel1;

        ctlPopUp PopInfo;

        ImageButton StanceButton;

        ImageButton VitaeButton;

        VScrollBar VScrollBar1;

        readonly ImageButton[] buttonArray;

        bool Locked;
        readonly frmMain myParent;
        private IPower[] myPowers;
        private CustomPanel Panel2;

        internal ListLabelV3 LLLeft;

        internal ListLabelV3 LLRight;

        public frmIncarnate(ref frmMain iParent)
        {
            Load += frmIncarnate_Load;
            myPowers = new IPower[0];
            Locked = false;
            buttonArray = new ImageButton[10];
            InitializeComponent();
            GenesisButton.ButtonClicked += GenesisButton_ButtonClicked;
            OmegaButton.ButtonClicked += OmegaButton_ButtonClicked;

            // PopInfo events
            PopInfo.MouseWheel += PopInfo_MouseWheel;
            PopInfo.MouseEnter += PopInfo_MouseEnter;

            StanceButton.ButtonClicked += StanceButton_ButtonClicked;
            VScrollBar1.Scroll += VScrollBar1_Scroll;
            VitaeButton.ButtonClicked += VitaeButton_ButtonClicked;
            alphaBtn.ButtonClicked += alphaBtn_ButtonClicked;
            destinyBtn.ButtonClicked += destinyBtn_ButtonClicked;
            hybridBtn.ButtonClicked += hybridBtn_ButtonClicked;
            ibClose.ButtonClicked += ibClose_ButtonClicked;
            interfaceBtn.ButtonClicked += interfaceBtn_ButtonClicked;
            judgementBtn.ButtonClicked += judgementBtn_ButtonClicked;
            lblLock.Click += lblLock_Click;

            // llLeft events
            LLLeft.ItemClick += llLeft_ItemClick;
            LLLeft.MouseEnter += llLeft_MouseEnter;
            LLLeft.ItemHover += llLeft_ItemHover;


            // llRight events
            LLRight.MouseEnter += llRight_MouseEnter;
            LLRight.ItemHover += llRight_ItemHover;
            LLRight.ItemClick += llRight_ItemClick;

            loreBtn.ButtonClicked += loreBtn_ButtonClicked;
            Name = nameof(frmIncarnate);
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmIncarnate));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            myParent = iParent;
            myPowers = DatabaseAPI.GetPowersetByName("Alpha", Enums.ePowerSetType.Incarnate).Powers;
            FormClosing += FrmIncarnate_FormClosing;
        }

        private void FrmIncarnate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                myParent.incarnateButton.Checked = false;
            }
            if (DialogResult == DialogResult.Cancel)
            {
                myParent.incarnateButton.Checked = false;
            }
        }

        void alphaBtn_ButtonClicked()

        {
            ImageButton alphaBtn = this.alphaBtn;
            SetPowerSet("Alpha", ref alphaBtn);
            this.alphaBtn = alphaBtn;
        }

        void ChangedScrollFrameContents()

        {
            VScrollBar1.Value = 0;
            VScrollBar1.Maximum = (int)Math.Round(PopInfo.lHeight * (VScrollBar1.LargeChange / (double)Panel1.Height));
            VScrollBar1_Scroll(VScrollBar1, new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        void destinyBtn_ButtonClicked()

        {
            ImageButton destinyBtn = this.destinyBtn;
            SetPowerSet("Destiny", ref destinyBtn);
            this.destinyBtn = destinyBtn;
        }

        private void FillLists()
        {
            LLLeft.SuspendRedraw = true;
            LLRight.SuspendRedraw = true;
            LLLeft.ClearItems();
            LLRight.ClearItems();
            int[] keys = new int[myPowers.Length - 1 + 1];
            if (myPowers.Length < 2)
            {
                int num = myPowers.Length - 1;
                for (int index = 0; index <= num; ++index)
                    keys[index] = myPowers[index].StaticIndex;
            }
            else if (myPowers[0].DisplayLocation != myPowers[1].DisplayLocation)
            {
                int num = myPowers.Length - 1;
                for (int index = 0; index <= num; ++index)
                    keys[index] = myPowers[index].DisplayLocation;
            }
            else
            {
                int num = myPowers.Length - 1;
                for (int index = 0; index <= num; ++index)
                    keys[index] = myPowers[index].StaticIndex;
            }
            Array.Sort(keys, myPowers);
            int num1 = myPowers.Length - 1;
            for (int index = 0; index <= num1; ++index)
            {
                ListLabelV3.LLItemState iState = !MidsContext.Character.CurrentBuild.PowerUsed(myPowers[index]) ? myPowers[index].DisplayName != "Nothing" ? ListLabelV3.LLItemState.Enabled : ListLabelV3.LLItemState.Disabled : ListLabelV3.LLItemState.Selected;
                ListLabelV3.ListLabelItemV3 iItem = !MidsContext.Config.RtFont.PairedBold ? new ListLabelV3.ListLabelItemV3(myPowers[index].DisplayName, iState) : new ListLabelV3.ListLabelItemV3(myPowers[index].DisplayName, iState, -1, -1, -1, "", ListLabelV3.LLFontFlags.Bold);
                if (index >= myPowers.Length / 2.0)
                    LLRight.AddItem(iItem);
                else
                    LLLeft.AddItem(iItem);
            }
            LLLeft.SuspendRedraw = false;
            LLRight.SuspendRedraw = false;
            LLLeft.Refresh();
            LLRight.Refresh();
        }

        void frmIncarnate_Load(object sender, EventArgs e)

        {
            buttonArray[0] = alphaBtn;
            buttonArray[1] = destinyBtn;
            buttonArray[2] = hybridBtn;
            buttonArray[3] = interfaceBtn;
            buttonArray[4] = judgementBtn;
            buttonArray[5] = loreBtn;
            buttonArray[6] = GenesisButton;
            buttonArray[7] = StanceButton;
            buttonArray[8] = VitaeButton;
            buttonArray[9] = OmegaButton;
            foreach (ImageButton button in buttonArray)
            {
                button.IA = myParent.Drawing.pImageAttributes;
                button.ImageOff = myParent.Drawing.bxPower[2].Bitmap;
                button.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            }
            BackColor = myParent.BackColor;
            PopInfo.ForeColor = BackColor;
            ListLabelV3 llLeft = LLLeft;
            UpdateLLColours(ref llLeft);
            LLLeft = llLeft;
            ListLabelV3 llRight = LLRight;
            UpdateLLColours(ref llRight);
            LLRight = llRight;
            ibClose.IA = myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = myParent.Drawing.bxPower[2].Bitmap;
            ibClose.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            PopUp.PopupData iPopup = new PopUp.PopupData();
            int index = iPopup.Add();
            iPopup.Sections[index].Add("Click powers to enable/disable them.", PopUp.Colors.Title);
            iPopup.Sections[index].Add("Powers in gray (or your custom 'power disabled' color) cannot be included in your stats.", PopUp.Colors.Text, 0.9f);
            PopInfo.SetPopup(iPopup);
            ChangedScrollFrameContents();
            FillLists();
        }

        void GenesisButton_ButtonClicked()

        {
            ImageButton genesisButton = GenesisButton;
            SetPowerSet("Genesis", ref genesisButton);
            GenesisButton = genesisButton;
        }

        void hybridBtn_ButtonClicked()

        {
            ImageButton hybridBtn = this.hybridBtn;
            SetPowerSet("Hybrid", ref hybridBtn);
            this.hybridBtn = hybridBtn;
        }

        void ibClose_ButtonClicked()

        {
            Close();
        }

        [DebuggerStepThrough]

        void interfaceBtn_ButtonClicked()

        {
            ImageButton interfaceBtn = this.interfaceBtn;
            SetPowerSet("Interface", ref interfaceBtn);
            this.interfaceBtn = interfaceBtn;
        }

        void judgementBtn_ButtonClicked()

        {
            ImageButton judgementBtn = this.judgementBtn;
            SetPowerSet("Judgement", ref judgementBtn);
            this.judgementBtn = judgementBtn;
        }

        void lblLock_Click(object sender, EventArgs e)

        {
            Locked = false;
            lblLock.Visible = false;
        }

        void llLeft_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            if (Button == MouseButtons.Right)
            {
                Locked = false;
                miniPowerInfo(Item.Index);
                lblLock.Visible = true;
                Locked = true;
                return;
            }

            if (Item.ItemState == ListLabelV3.LLItemState.Disabled)
                return;
            bool flag = !MidsContext.Character.CurrentBuild.PowerUsed(myPowers[Item.Index]);
            int num1 = LLLeft.Items.Length - 1;
            for (int index = 0; index <= num1; ++index)
            {
                if (LLLeft.Items[index].ItemState == ListLabelV3.LLItemState.Selected)
                    LLLeft.Items[index].ItemState = ListLabelV3.LLItemState.Enabled;
                if (MidsContext.Character.CurrentBuild.PowerUsed(myPowers[index]))
                    MidsContext.Character.CurrentBuild.RemovePower(myPowers[index]);
            }
            int num2 = LLRight.Items.Length - 1;
            for (int index = 0; index <= num2; ++index)
            {
                if (LLRight.Items[index].ItemState == ListLabelV3.LLItemState.Selected)
                    LLRight.Items[index].ItemState = ListLabelV3.LLItemState.Enabled;
                if (MidsContext.Character.CurrentBuild.PowerUsed(myPowers[index + LLLeft.Items.Length]))
                    MidsContext.Character.CurrentBuild.RemovePower(myPowers[index + LLLeft.Items.Length]);
            }
            if (flag)
            {
                MidsContext.Character.CurrentBuild.AddPower(myPowers[Item.Index], 49).StatInclude = true;
                Item.ItemState = ListLabelV3.LLItemState.Selected;
            }
            LLLeft.Refresh();
            LLRight.Refresh();
            myParent.PowerModified(markModified: true);
        }

        void llLeft_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            miniPowerInfo(Item.Index);
        }

        void llLeft_MouseEnter(object sender, EventArgs e)
        {
            if (!ContainsFocus)
                return;
            Panel2.Focus();
        }

        void llRight_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            int pIDX = Item.Index + LLLeft.Items.Length;
            if (Button == MouseButtons.Right)
            {
                Locked = false;
                miniPowerInfo(pIDX);
                lblLock.Visible = true;
                Locked = true;
            }
            else
            {
                if (Item.ItemState == ListLabelV3.LLItemState.Disabled)
                    return;
                bool unused = !MidsContext.Character.CurrentBuild.PowerUsed(myPowers[pIDX]);
                bool hasChanges = false;
                for (int index = 0; index <= LLLeft.Items.Length - 1; ++index)
                {
                    if (LLLeft.Items[index].ItemState == ListLabelV3.LLItemState.Selected)
                        LLLeft.Items[index].ItemState = ListLabelV3.LLItemState.Enabled;
                    if (!MidsContext.Character.CurrentBuild.PowerUsed(myPowers[index]))
                        continue;
                    MidsContext.Character.CurrentBuild.RemovePower(myPowers[index]);
                    hasChanges = true;
                }
                for (int index = 0; index <= LLRight.Items.Length - 1; ++index)
                {
                    if (LLRight.Items[index].ItemState == ListLabelV3.LLItemState.Selected)
                        LLRight.Items[index].ItemState = ListLabelV3.LLItemState.Enabled;
                    if (!MidsContext.Character.CurrentBuild.PowerUsed(myPowers[index + LLLeft.Items.Length]))
                        continue;
                    MidsContext.Character.CurrentBuild.RemovePower(myPowers[index + LLLeft.Items.Length]);
                    hasChanges = true;
                }
                if (unused)
                {
                    MidsContext.Character.CurrentBuild.AddPower(myPowers[pIDX], 49).StatInclude = true;
                    Item.ItemState = ListLabelV3.LLItemState.Selected;
                }
                LLLeft.Refresh();
                LLRight.Refresh();
                myParent.PowerModified(markModified: unused || hasChanges);
            }
        }

        void llRight_ItemHover(ListLabelV3.ListLabelItemV3 Item)

        {
            miniPowerInfo(Item.Index + LLLeft.Items.Length);
        }

        void llRight_MouseEnter(object sender, EventArgs e)

        {
            llLeft_MouseEnter(RuntimeHelpers.GetObjectValue(sender), e);
        }

        void loreBtn_ButtonClicked()

        {
            ImageButton loreBtn = this.loreBtn;
            SetPowerSet("Lore", ref loreBtn);
            this.loreBtn = loreBtn;
        }

        private void miniPowerInfo(int pIDX)
        {
            if (Locked)
                return;
            IPower power1 = new Power(myPowers[pIDX]);
            power1.AbsorbPetEffects();
            power1.ApplyGrantPowerEffects();
            PopUp.PopupData iPopup = new PopUp.PopupData();
            if (pIDX < 0)
            {
                PopInfo.SetPopup(iPopup);
                ChangedScrollFrameContents();
            }
            else
            {
                int index1 = iPopup.Add();
                string str1 = "";
                switch (power1.PowerType)
                {
                    case Enums.ePowerType.Click:
                        if (power1.ClickBuff)
                        {
                            str1 = "(Click)";
                        }
                        break;
                    case Enums.ePowerType.Auto_:
                        str1 = "(Auto)";
                        break;
                    case Enums.ePowerType.Toggle:
                        str1 = "(Toggle)";
                        break;
                }
                iPopup.Sections[index1].Add(power1.DisplayName, PopUp.Colors.Title);
                iPopup.Sections[index1].Add(str1 + " " + power1.DescShort, PopUp.Colors.Text, 0.9f);
                string str2 = power1.DescLong.Replace("<br>", "\r\n");
                iPopup.Sections[index1].Add(str1 + " " + str2, PopUp.Colors.Common, 1f, FontStyle.Regular);
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
                        if (!((power2.Effects[index3].EffectType != Enums.eEffectType.GrantPower | power2.Effects[index3].Absorbed_Effect) &
                              power2.Effects[index3].EffectType != Enums.eEffectType.RevokePower &
                              power2.Effects[index3].EffectType != Enums.eEffectType.SetMode))
                            continue;
                        int index4 = iPopup.Add();
                        power1.Effects[index3].SetPower(power1);
                        string[] strArray = power1.Effects[index3].BuildEffectString().Replace("[", "\r\n").Replace("\r\n", "^").Replace("  ", "").Replace("]", "").Split(chArray);
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

        void OmegaButton_ButtonClicked()

        {
            ImageButton omegaButton = OmegaButton;
            SetPowerSet("Omega", ref omegaButton);
            OmegaButton = omegaButton;
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

        void SetPowerSet(string Setname, ref ImageButton button)

        {
            foreach (ImageButton button1 in buttonArray)
                button1.Checked = false;
            button.Checked = true;
            myPowers = DatabaseAPI.GetPowersetByID(Setname, Enums.ePowerSetType.Incarnate).Powers;
            FillLists();
        }

        void StanceButton_ButtonClicked()

        {
            ImageButton stanceButton = StanceButton;
            SetPowerSet("Stance", ref stanceButton);
            StanceButton = stanceButton;
        }

        private void UpdateLLColours(ref ListLabelV3 iList)
        {
            iList.UpdateTextColors(ListLabelV3.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            iList.UpdateTextColors(ListLabelV3.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            iList.UpdateTextColors(ListLabelV3.LLItemState.Selected, MidsContext.Config.RtFont.ColorPowerTaken);
            iList.UpdateTextColors(ListLabelV3.LLItemState.SelectedDisabled, MidsContext.Config.RtFont.ColorPowerTakenDark);
            iList.UpdateTextColors(ListLabelV3.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            iList.HoverColor = MidsContext.Config.RtFont.ColorPowerHighlight;
            //iList.Font = new Font(iList.Font.FontFamily, MidsContext.Config.RtFont.PairedBase, FontStyle.Bold, GraphicsUnit.Point);
            iList.Font = new Font("Arial", 11.5f, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        void VitaeButton_ButtonClicked()

        {
            ImageButton vitaeButton = VitaeButton;
            SetPowerSet("Vitae", ref vitaeButton);
            VitaeButton = vitaeButton;
        }

        void VScrollBar1_Scroll(object sender, ScrollEventArgs e)

        {
            if (PopInfo.lHeight > (double)Panel1.Height & VScrollBar1.Maximum > VScrollBar1.LargeChange)
                PopInfo.ScrollY = (float)(VScrollBar1.Value / (double)(VScrollBar1.Maximum - VScrollBar1.LargeChange) * (PopInfo.lHeight - (double)Panel1.Height));
            else
                PopInfo.ScrollY = 0.0f;
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
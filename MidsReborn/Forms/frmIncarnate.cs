using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace Mids_Reborn.Forms
{
    public partial class frmIncarnate : Form
    {
        private readonly ImageButton[] buttonArray;
        private readonly frmMain myParent;
        private ImageButton alphaBtn;

        private ImageButton destinyBtn;

        private ImageButton GenesisButton;

        private ImageButton hybridBtn;

        private ImageButton ibClose;

        private ImageButton interfaceBtn;

        private ImageButton judgementBtn;

        private Label lblLock;

        internal ListLabelV3 LLLeft;

        internal ListLabelV3 LLRight;

        private bool Locked;

        private ImageButton loreBtn;
        private IPower[] myPowers;

        private ImageButton OmegaButton;
        private Panel Panel1;
        private CustomPanel Panel2;

        private ctlPopUp PopInfo;

        private ImageButton StanceButton;

        private ImageButton VitaeButton;

        private VScrollBar VScrollBar1;

        public frmIncarnate(ref frmMain iParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            CenterToParent();
            Location = new Point(Location.X, Location.Y - 100);
            Load += frmIncarnate_Load;
            myPowers = Array.Empty<IPower>();
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
            Icon = Resources.MRB_Icon_Concept;
            myParent = iParent;
            myPowers = DatabaseAPI.GetPowersetByName("Alpha", Enums.ePowerSetType.Incarnate).Powers;
            FormClosing += FrmIncarnate_FormClosing;
        }

        private void FrmIncarnate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                myParent.ibIncarnatePowersEx.ToggleState = ImageButtonEx.States.ToggledOff;
            }

            if (DialogResult == DialogResult.Cancel)
            {
                myParent.ibIncarnatePowersEx.ToggleState = ImageButtonEx.States.ToggledOff;
            }
        }

        private void alphaBtn_ButtonClicked()
        {
            var alphaBtn = this.alphaBtn;
            SetPowerSet("Alpha", ref alphaBtn);
            this.alphaBtn = alphaBtn;
        }

        private void ChangedScrollFrameContents()
        {
            VScrollBar1.Value = 0;
            VScrollBar1.Maximum = (int) Math.Round(PopInfo.lHeight * (VScrollBar1.LargeChange / (double) Panel1.Height));
            VScrollBar1_Scroll(VScrollBar1, new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        private void destinyBtn_ButtonClicked()
        {
            var destinyBtn = this.destinyBtn;
            SetPowerSet("Destiny", ref destinyBtn);
            this.destinyBtn = destinyBtn;
        }

        private List<IPower> ParseIncarnate(List<IPower> powerList, string order, string name)
        {
            var pairList = new List<KeyValuePair<int, IPower>>();
            var tList = powerList.FindAll(x => x.DisplayName.Contains(name));
            var outList = new List<IPower>();
            int pos = 0;
            foreach (var power in tList)
            {
                string[] nSplit = power.DisplayName.Split();
                var value = power.DisplayName.Replace($"{nSplit[0]} ", "");
                switch (order)
                {
                    case "Alpha":
                        pos = (int)Enum.Parse(typeof(Enums.eAlphaOrder), value.Replace(" ", "_"));
                        break;
                    case "Judgement":
                        pos = (int)Enum.Parse(typeof(Enums.eJudgementOrder), value.Replace(" ", "_"));
                        break;
                    case "Interface":
                        pos = (int)Enum.Parse(typeof(Enums.eInterfaceOrder), value.Replace(" ", "_"));
                        break;
                    case "Lore":
                        switch (name)
                        {
                            case "Banished Pantheon":
                                value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} ", "");
                                break;
                            case "Knives of Vengeance":
                                value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} {nSplit[2]} ", "");
                                break;
                            case "Polar Lights":
                                value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} ", "");
                                break;
                            case "Robotic Drones":
                                value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} ", "");
                                break;
                            case "Storm Elemental":
                                value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} ", "");
                                break;
                            case "Talons of Vengeance":
                                value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} {nSplit[2]} ", "");
                                break;
                        }
                        pos = (int)Enum.Parse(typeof(Enums.eLoreOrder), value.Replace(" ", "_"));
                        break;
                    case "Destiny":
                        pos = (int)Enum.Parse(typeof(Enums.eDestinyOrder), value.Replace(" ", "_"));
                        break;
                    case "Hybrid":
                        pos = (int)Enum.Parse(typeof(Enums.eHybridOrder), value.Replace(" ", "_"));
                        break;
                }
                pairList.Add(new KeyValuePair<int, IPower>(pos, power));
            }

            var oList = pairList.OrderBy(x => x.Key);
            foreach (var power in oList)
            {
                outList.Add(power.Value);
            }

            return outList;
        }

        private void FillLists(string setName)
        {
            var newPowerList = new List<IPower>();
            switch (setName)
            {
                case "Alpha":
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Agility"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Cardiac"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Intuition"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Musculature"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Nerve"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Resilient"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Spiritual"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Vigor"));
                    break;
                case "Judgement":
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Cryonic"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Ion"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Mighty"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Pyronic"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Void"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Vorpal"));
                    break;
                case "Interface":
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Cognitive"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Degenerative"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Diamagnetic"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Gravitic"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Paralytic"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Preemptive"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Reactive"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Spectral"));
                    break;
                case "Lore":
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Arachnos"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Banished Pantheon"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Carnival"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Cimeroran"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Clockwork"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "IDF"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Knives of Vengeance"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Longbow"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Nemesis"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Phantom"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Polar Lights"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Rikti"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Robotic Drones"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Rularuu"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Seers"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Storm Elemental"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Talons of Vengeance"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Tsoo"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Vanguard"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Warworks"));
                    break;
                case "Destiny":
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Ageless"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Barrier"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Clarion"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Incandescence"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Rebirth"));
                    break;
                case "Hybrid":
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Assault"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Control"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Melee"));
                    newPowerList.AddRange(ParseIncarnate(myPowers.ToList(), setName, "Support"));
                    break;
            }

            myPowers = newPowerList.ToArray();
            LLLeft.SuspendRedraw = true;
            LLRight.SuspendRedraw = true;
            LLLeft.ClearItems();
            LLRight.ClearItems();
            /*var keys = new int[myPowers.Length - 1 + 1];
            if (myPowers.Length < 2)
            {
                var num = myPowers.Length - 1;
                for (var index = 0; index <= num; ++index)
                    keys[index] = myPowers[index].StaticIndex;
            }
            else if (myPowers[0].DisplayLocation != myPowers[1].DisplayLocation)
            {
                var num = myPowers.Length - 1;
                for (var index = 0; index <= num; ++index)
                    keys[index] = myPowers[index].DisplayLocation;
            }
            else
            {
                var num = myPowers.Length - 1;
                for (var index = 0; index <= num; ++index)
                    keys[index] = myPowers[index].StaticIndex;
            }

            Array.Sort(keys, myPowers);*/
            var num1 = myPowers.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                var iState = !MidsContext.Character.CurrentBuild.PowerUsed(myPowers[index]) ? myPowers[index].DisplayName != "Nothing" ? ListLabelV3.LlItemState.Enabled : ListLabelV3.LlItemState.Disabled : ListLabelV3.LlItemState.Selected;
                var iItem = !MidsContext.Config.RtFont.PairedBold ? new ListLabelV3.ListLabelItemV3(myPowers[index].DisplayName, iState) : new ListLabelV3.ListLabelItemV3(myPowers[index].DisplayName, iState, -1, -1, -1, "", ListLabelV3.LlFontFlags.Bold);
                if (index < myPowers.Length / 2)
                {
                    LLLeft.AddItem(iItem);
                }
                else
                {
                    LLRight.AddItem(iItem);
                }
            }

            LLLeft.SuspendRedraw = false;
            LLRight.SuspendRedraw = false;
            LLLeft.Refresh();
            LLRight.Refresh();
        }

        private void frmIncarnate_Load(object sender, EventArgs e)

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
            foreach (var button in buttonArray)
            {
                if (button.Enabled)
                {
                    button.IA = myParent.Drawing.pImageAttributes;
                    button.ImageOff = MidsContext.Character.IsHero()
                        ? myParent.Drawing.bxPower[2].Bitmap
                        : myParent.Drawing.bxPower[4].Bitmap;
                    button.ImageOn = MidsContext.Character.IsHero()
                        ? myParent.Drawing.bxPower[3].Bitmap
                        : myParent.Drawing.bxPower[5].Bitmap;
                }
                else
                {
                    button.IA = myParent.Drawing.pImageAttributes;
                    button.ImageOff = myParent.Drawing.bxPower[1].Bitmap;
                    button.ImageOn = myParent.Drawing.bxPower[1].Bitmap;
                }
            }

            BackColor = myParent.BackColor;
            PopInfo.ForeColor = BackColor;
            var llLeft = LLLeft;
            UpdateLLColours(ref llLeft);
            LLLeft = llLeft;
            var llRight = LLRight;
            UpdateLLColours(ref llRight);
            LLRight = llRight;
            ibClose.IA = myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            var iPopup = new PopUp.PopupData();
            var index = iPopup.Add();
            iPopup.Sections[index].Add("Click powers to enable/disable them.", PopUp.Colors.Title);
            iPopup.Sections[index]
                .Add("Powers in gray (or your custom 'power disabled' color) cannot be included in your stats.",
                    PopUp.Colors.Text, 0.9f);
            PopInfo.SetPopup(iPopup);
            ChangedScrollFrameContents();
            FillLists("Alpha");
        }

        private void GenesisButton_ButtonClicked()
        {
            var genesisButton = GenesisButton;
            SetPowerSet("Genesis", ref genesisButton);
            GenesisButton = genesisButton;
        }

        private void hybridBtn_ButtonClicked()
        {
            var hybridBtn = this.hybridBtn;
            SetPowerSet("Hybrid", ref hybridBtn);
            this.hybridBtn = hybridBtn;
        }

        private void ibClose_ButtonClicked()
        {
            Close();
        }

        [DebuggerStepThrough]
        private void interfaceBtn_ButtonClicked()
        {
            var interfaceBtn = this.interfaceBtn;
            SetPowerSet("Interface", ref interfaceBtn);
            this.interfaceBtn = interfaceBtn;
        }

        private void judgementBtn_ButtonClicked()
        {
            var judgementBtn = this.judgementBtn;
            SetPowerSet("Judgement", ref judgementBtn);
            this.judgementBtn = judgementBtn;
        }

        private void lblLock_Click(object sender, EventArgs e)
        {
            Locked = false;
            lblLock.Visible = false;
        }

        private void llLeft_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            if (Button == MouseButtons.Right)
            {
                Locked = false;
                MiniPowerInfo(Item.Index);
                lblLock.Visible = true;
                Locked = true;
                return;
            }

            if (Item.ItemState == ListLabelV3.LlItemState.Disabled)
                return;
            var flag = !MidsContext.Character.CurrentBuild.PowerUsed(myPowers[Item.Index]);
            var num1 = LLLeft.Items.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                if (LLLeft.Items[index].ItemState == ListLabelV3.LlItemState.Selected)
                    LLLeft.Items[index].ItemState = ListLabelV3.LlItemState.Enabled;
                if (MidsContext.Character.CurrentBuild.PowerUsed(myPowers[index]))
                    MidsContext.Character.CurrentBuild.RemovePower(myPowers[index]);
            }

            var num2 = LLRight.Items.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                if (LLRight.Items[index].ItemState == ListLabelV3.LlItemState.Selected)
                    LLRight.Items[index].ItemState = ListLabelV3.LlItemState.Enabled;
                if (MidsContext.Character.CurrentBuild.PowerUsed(myPowers[index + LLLeft.Items.Length]))
                    MidsContext.Character.CurrentBuild.RemovePower(myPowers[index + LLLeft.Items.Length]);
            }

            if (flag)
            {
                MidsContext.Character.CurrentBuild.AddPower(myPowers[Item.Index], 49).StatInclude = true;
                Item.ItemState = ListLabelV3.LlItemState.Selected;
            }

            LLLeft.Refresh();
            LLRight.Refresh();
            myParent.PowerModified(true);
            myParent.DoRefresh();
        }

        private void llLeft_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            MiniPowerInfo(Item.Index);
        }

        private void llLeft_MouseEnter(object sender, EventArgs e)
        {
            if (!ContainsFocus)
                return;
            Panel2.Focus();
        }

        private void llRight_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            var pIDX = Item.Index + LLLeft.Items.Length;
            if (Button == MouseButtons.Right)
            {
                Locked = false;
                MiniPowerInfo(pIDX);
                lblLock.Visible = true;
                Locked = true;
            }
            else
            {
                if (Item.ItemState == ListLabelV3.LlItemState.Disabled)
                    return;
                var unused = !MidsContext.Character.CurrentBuild.PowerUsed(myPowers[pIDX]);
                var hasChanges = false;
                for (var index = 0; index <= LLLeft.Items.Length - 1; ++index)
                {
                    if (LLLeft.Items[index].ItemState == ListLabelV3.LlItemState.Selected)
                        LLLeft.Items[index].ItemState = ListLabelV3.LlItemState.Enabled;
                    if (!MidsContext.Character.CurrentBuild.PowerUsed(myPowers[index]))
                        continue;
                    MidsContext.Character.CurrentBuild.RemovePower(myPowers[index]);
                    hasChanges = true;
                }

                for (var index = 0; index <= LLRight.Items.Length - 1; ++index)
                {
                    if (LLRight.Items[index].ItemState == ListLabelV3.LlItemState.Selected)
                        LLRight.Items[index].ItemState = ListLabelV3.LlItemState.Enabled;
                    if (!MidsContext.Character.CurrentBuild.PowerUsed(myPowers[index + LLLeft.Items.Length]))
                        continue;
                    MidsContext.Character.CurrentBuild.RemovePower(myPowers[index + LLLeft.Items.Length]);
                    hasChanges = true;
                }

                if (unused)
                {
                    MidsContext.Character.CurrentBuild.AddPower(myPowers[pIDX], 49).StatInclude = true;
                    Item.ItemState = ListLabelV3.LlItemState.Selected;
                }

                LLLeft.Refresh();
                LLRight.Refresh();
                myParent.PowerModified(unused || hasChanges);
                myParent.DoRefresh();
            }
        }

        private void llRight_ItemHover(ListLabelV3.ListLabelItemV3 Item)

        {
            MiniPowerInfo(Item.Index + LLLeft.Items.Length);
        }

        private void llRight_MouseEnter(object sender, EventArgs e)

        {
            llLeft_MouseEnter(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void loreBtn_ButtonClicked()

        {
            var loreBtn = this.loreBtn;
            SetPowerSet("Lore", ref loreBtn);
            this.loreBtn = loreBtn;
        }

        private void MiniPowerInfo(int pIDX)
        {
            if (Locked)
                return;
            var iPopup = new PopUp.PopupData();
            if (pIDX < 0)
            {
                PopInfo.SetPopup(iPopup);
                ChangedScrollFrameContents();
            }
            else
            {
                IPower power1 = new Power(myPowers[pIDX]);
                power1.AbsorbPetEffects();
                power1.ApplyGrantPowerEffects();
                var index1 = iPopup.Add();
                var str1 = "";
                switch (power1.PowerType)
                {
                    case Enums.ePowerType.Click:
                        if (power1.ClickBuff) str1 = "(Click)";
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
                var str2 = power1.DescLong.Replace("<br>", "\r\n");
                iPopup.Sections[index1].Add(str1 + " " + str2, PopUp.Colors.Common, 1f, FontStyle.Regular);
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
                        if (!(((power2.Effects[index3].EffectType != Enums.eEffectType.GrantPower) |
                               power2.Effects[index3].Absorbed_Effect) &
                              (power2.Effects[index3].EffectType != Enums.eEffectType.RevokePower) &
                              (power2.Effects[index3].EffectType != Enums.eEffectType.SetMode)))
                            continue;
                        var index4 = iPopup.Add();
                        power1.Effects[index3].SetPower(power1);
                        var strArray = power1.Effects[index3].BuildEffectString(false, "", false, false, false, true)
                            .Replace("[", "\r\n")
                            .Replace("\r\n", "^")
                            .Replace("  ", "")
                            .Replace("]", "")
                            .Split(chArray);
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

        private void OmegaButton_ButtonClicked()

        {
            var omegaButton = OmegaButton;
            SetPowerSet("Omega", ref omegaButton);
            OmegaButton = omegaButton;
        }

        private void PopInfo_MouseEnter(object sender, EventArgs e)

        {
            if (!ContainsFocus)
                return;
            VScrollBar1.Focus();
        }

        private void PopInfo_MouseWheel(object sender, MouseEventArgs e)
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

        private void SetPowerSet(string Setname, ref ImageButton button)

        {
            foreach (var button1 in buttonArray)
                button1.Checked = false;
            button.Checked = true;
            myPowers = DatabaseAPI.GetPowersetByID(Setname, Enums.ePowerSetType.Incarnate).Powers;
            FillLists(Setname);
        }

        private void StanceButton_ButtonClicked()

        {
            var stanceButton = StanceButton;
            SetPowerSet("Stance", ref stanceButton);
            StanceButton = stanceButton;
        }

        private void UpdateLLColours(ref ListLabelV3 iList)
        {
            iList.UpdateTextColors(ListLabelV3.LlItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            iList.UpdateTextColors(ListLabelV3.LlItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            iList.UpdateTextColors(ListLabelV3.LlItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            iList.ScrollBarColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain;
            iList.ScrollButtonColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
            iList.UpdateTextColors(ListLabelV3.LlItemState.Selected,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenHero
                    : MidsContext.Config.RtFont.ColorPowerTakenVillain);
            iList.UpdateTextColors(ListLabelV3.LlItemState.SelectedDisabled,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
            iList.HoverColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
            var style = !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold;
            iList.Font = new Font(iList.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase, style, GraphicsUnit.Point);
        }

        private void VitaeButton_ButtonClicked()
        {
            var vitaeButton = VitaeButton;
            SetPowerSet("Vitae", ref vitaeButton);
            VitaeButton = vitaeButton;
        }

        private void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if ((PopInfo.lHeight > (double) Panel1.Height) & (VScrollBar1.Maximum > VScrollBar1.LargeChange))
                PopInfo.ScrollY = (float) (VScrollBar1.Value /
                                           (double) (VScrollBar1.Maximum - VScrollBar1.LargeChange) *
                                           (PopInfo.lHeight - (double) Panel1.Height));
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
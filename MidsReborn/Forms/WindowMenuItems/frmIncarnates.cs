using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
    public partial class frmIncarnates : Form
    {
        private readonly ImageButton[] buttonArray;
        private readonly frmMain myParent;
        private IPower?[] myPowers;
        private bool Locked;

        public frmIncarnates(ref frmMain iParent)
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
            loreBtn.ButtonClicked += loreBtn_ButtonClicked;
            Icon = Resources.MRB_Icon_Concept;
            myParent = iParent;
            Parent = myParent;
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
        private void frmIncarnate_Load(object? sender, EventArgs e)
        {
            CenterToParent();
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
                button.IA = myParent.Drawing.PImageAttributes;
                button.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.BxPower[2].Bitmap : myParent.Drawing.BxPower[4].Bitmap;
                button.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.BxPower[3].Bitmap : myParent.Drawing.BxPower[5].Bitmap;
                Debug.WriteLine(button.TextOn);
                Debug.WriteLine(DatabaseAPI.ServerData.EnabledIncarnates[button.TextOn]);
                button.Enabled = DatabaseAPI.ServerData.EnabledIncarnates[button.TextOn];
            }

            BackColor = myParent.BackColor;
            PopInfo.ForeColor = BackColor;
            var plLeft = PLLeft;
            UpdatePLColors(ref plLeft);
            //LLLeft = llLeft;
            var plRight = PLRight;
            UpdatePLColors(ref plRight);
            //LLRight = llRight;
            ibClose.IA = myParent.Drawing.PImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.BxPower[2].Bitmap : myParent.Drawing.BxPower[4].Bitmap;
            ibClose.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.BxPower[3].Bitmap : myParent.Drawing.BxPower[5].Bitmap;
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
        private void FillLists()
        {
            PLLeft.SelectionMode = SelectionMode.None;
            PLRight.SelectionMode = SelectionMode.None;
            var leftBound = new BindingList<IPower?>();
            var rightBound = new BindingList<IPower?>();
            var powersLength = myPowers.Length;
            for (var index = 0; index < powersLength / 2; index++)
            {
                leftBound.Add(myPowers[index]);
            }
            for (var index = powersLength / 2; index < powersLength; index++)
            {
                rightBound.Add(myPowers[index]);
            }
            //Assign Left
            PLLeft.DisplayMember = "DisplayName";
            PLLeft.ValueMember = null;
            PLLeft.DataSource = leftBound;
            //Assign Right
            PLRight.DisplayMember = "DisplayName";
            PLRight.ValueMember = null;
            PLRight.DataSource = rightBound;
            //Reset Selection Modes
            PLLeft.SelectionMode = SelectionMode.One;
            PLRight.SelectionMode = SelectionMode.One;
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
            VScrollBar1.Maximum = (int)Math.Round(PopInfo.lHeight * (VScrollBar1.LargeChange / (double)ctlPanel1.Height));
            VScrollBar1_Scroll(VScrollBar1, new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        private void destinyBtn_ButtonClicked()
        {
            var destinyBtn = this.destinyBtn;
            SetPowerSet("Destiny", ref destinyBtn);
            this.destinyBtn = destinyBtn;
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
        private void loreBtn_ButtonClicked()
        {
            var loreBtn = this.loreBtn;
            SetPowerSet("Lore", ref loreBtn);
            this.loreBtn = loreBtn;
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
            FillLists();
        }

        private void StanceButton_ButtonClicked()
        {
            var stanceButton = StanceButton;
            SetPowerSet("Stance", ref stanceButton);
            StanceButton = stanceButton;
        }
        private void VitaeButton_ButtonClicked()
        {
            var vitaeButton = VitaeButton;
            SetPowerSet("Vitae", ref vitaeButton);
            VitaeButton = vitaeButton;
        }

        private void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if ((PopInfo.lHeight > (double)ctlPanel1.Height) & (VScrollBar1.Maximum > VScrollBar1.LargeChange))
                PopInfo.ScrollY = (float)(VScrollBar1.Value /
                                          (double)(VScrollBar1.Maximum - VScrollBar1.LargeChange) *
                                          (PopInfo.lHeight - (double)ctlPanel1.Height));
            else
                PopInfo.ScrollY = 0.0f;
        }
        private void UpdatePLColors(ref ctlPowerList iList)
        {
            iList.UpdateTextColors(ctlPowerList.ItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            iList.UpdateTextColors(ctlPowerList.ItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            iList.UpdateTextColors(ctlPowerList.ItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            /*iList.ScrollBarColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain;
            iList.ScrollButtonColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;*/
            iList.UpdateTextColors(ctlPowerList.ItemState.Selected,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenHero
                    : MidsContext.Config.RtFont.ColorPowerTakenVillain);
            iList.UpdateTextColors(ctlPowerList.ItemState.SelectedDisabled,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
            /*iList.HoverColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                : MidsContext.Config.RtFont.ColorPowerHighlightVillain;*/
            iList.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold, GraphicsUnit.Pixel);
            iList.Refresh();
            iList.Update();
        }
    }
}

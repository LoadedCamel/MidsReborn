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
    public partial class frmMMPowers : Form
    {
        ImageButton ibClose;

        Label lblLock;
        ListLabelV2 llLeft;
        ListLabelV2 llRight;

        ComboBox cbSelPetPower;
        ComboBox cbSelPets;

        bool _locked;

        readonly frmMain _myParent;

        List<IPower> _myPetPowers;
        List<IPower> _myPowers;


        private List<string> PetPowers { get; }

        Panel Panel1;

        ctlPopUp PopInfo;

        VScrollBar VScrollBar1;

        private frmIncarnate.CustomPanel Panel2;

        public frmMMPowers(frmMain iParent, List<string> PetPowersList)
        {
            Load += frmMMPowers_Load;
            _locked = false;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmMMPowers));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmMMPowers);
            _myParent = iParent;
            PetPowers = PetPowersList;
            FormClosing += FrmMMPowers_FormClosing;
            Panel1.GotFocus += Panel1OnGotFocus;
        }

        private void Panel1OnGotFocus(object sender, EventArgs e)
        {
            VScrollBar1.Focus();
        }

        private void FrmMMPowers_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                _myParent.petsButton.Checked = false;
                _myParent.petWindowFlag = false;
            }
            if (DialogResult == DialogResult.Cancel)
            {
                _myParent.petsButton.Checked = false;
                _myParent.petWindowFlag = false;
            }
        }

        public void UpdateFonts(Font font)
        {
            var llLeft = this.llLeft;
            var llRight = this.llRight;
            llLeft.SuspendRedraw = true;
            llRight.SuspendRedraw = true;
            llLeft.Font = font;
            llRight.Font = font;
            llLeft.UpdateTextColors(ListLabelV2.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            llLeft.UpdateTextColors(ListLabelV2.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            llLeft.UpdateTextColors(ListLabelV2.LLItemState.Selected, MidsContext.Config.RtFont.ColorPowerTaken);
            llLeft.UpdateTextColors(ListLabelV2.LLItemState.SelectedDisabled, MidsContext.Config.RtFont.ColorPowerTakenDark);
            llLeft.UpdateTextColors(ListLabelV2.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            llLeft.HoverColor = MidsContext.Config.RtFont.ColorPowerHighlight;
            llRight.UpdateTextColors(ListLabelV2.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            llRight.UpdateTextColors(ListLabelV2.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            llRight.UpdateTextColors(ListLabelV2.LLItemState.Selected, MidsContext.Config.RtFont.ColorPowerTaken);
            llRight.UpdateTextColors(ListLabelV2.LLItemState.SelectedDisabled, MidsContext.Config.RtFont.ColorPowerTakenDark);
            llRight.UpdateTextColors(ListLabelV2.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
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
            int num = _myPetPowers.Count - 1;
            for (int index = 0; index <= num; ++index)
            {
                ListLabelV2.LLItemState iState = !MidsContext.Character.CurrentBuild.PowerUsed(_myPetPowers[index]) ? (!(_myPetPowers[index].PowerType != Enums.ePowerType.Click | _myPetPowers[index].ClickBuff) ? (!_myPetPowers[index].SubIsAltColour ? ListLabelV2.LLItemState.Disabled : ListLabelV2.LLItemState.Invalid) : ListLabelV2.LLItemState.Enabled) : ListLabelV2.LLItemState.Selected;
                ListLabelV2.ListLabelItemV2 iItem = !MidsContext.Config.RtFont.PairedBold ? new ListLabelV2.ListLabelItemV2(_myPetPowers[index].DisplayName, iState) : new ListLabelV2.ListLabelItemV2(_myPetPowers[index].DisplayName, iState, -1, -1, -1, "", ListLabelV2.LLFontFlags.Bold);
                if (index >= _myPetPowers.Count / 2.0)
                    llRight.AddItem(iItem);
                else
                    llLeft.AddItem(iItem);
            }
            llLeft.SuspendRedraw = false;
            llRight.SuspendRedraw = false;
            llLeft.Refresh();
            llRight.Refresh();
        }

        void frmMMPowers_Load(object sender, EventArgs e)
        {
            BackColor = _myParent.BackColor;
            PopInfo.ForeColor = BackColor;
            ListLabelV2 llLeft = this.llLeft;
            UpdateLlColours(ref llLeft);
            this.llLeft = llLeft;
            ListLabelV2 llRight = this.llRight;
            UpdateLlColours(ref llRight);
            this.llRight = llRight;
            ibClose.IA = _myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = _myParent.Drawing.bxPower[2].Bitmap;
            ibClose.ImageOn = _myParent.Drawing.bxPower[3].Bitmap;
            PopUp.PopupData iPopup = new PopUp.PopupData();
            int index = iPopup.Add();
            iPopup.Sections[index].Add("Click powers to enable/disable them.", PopUp.Colors.Title);
            iPopup.Sections[index].Add("Powers in gray (or your custom 'power disabled' color) cannot be included in your stats.", PopUp.Colors.Text, 0.9f);
            PopInfo.SetPopup(iPopup);
            //ChangedScrollFrameContents();
            UpdatePetPowersCombo(PetPowers);
            UpdatePetsListCombo();
            //FillLists();
        }

        private void UpdatePetPowersCombo(List<string> pPowersList)
        {
            //_myPowers = new List<IPower>();
            
            foreach (var power in pPowersList)
            {
                cbSelPetPower.Items.Add(power);
            }

            cbSelPetPower.SelectedIndex = 0;
        }

        private void UpdatePetsListCombo()
        {
            cbSelPets.Items.Clear();
            var SelectedPet = cbSelPetPower.SelectedItem.ToString();
            switch (SelectedPet)
            {
                case "Summon Wolves":
                    cbSelPets.Items.Add("Howler Wolf");
                    cbSelPets.Items.Add("Alpha Wolf");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Summon Lions":
                    cbSelPets.Items.Add("Lioness");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Summon Dire Wolf":
                    cbSelPets.Items.Add("Dire Wolf");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Summon Demonlings":
                    cbSelPets.Items.Add("Cold Demonling");
                    cbSelPets.Items.Add("Fiery Demonling");
                    cbSelPets.Items.Add("Hellfire Demonling");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Summon Demons":
                    cbSelPets.Items.Add("Ember Demon");
                    cbSelPets.Items.Add("Hellfire Gargoyle");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Hell on Earth":
                    cbSelPets.Items.Add("Living Hellfire");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Summon Demon Prince":
                    cbSelPets.Items.Add("Demon Prince");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Soldiers":
                    cbSelPets.Items.Add("Soldier");
                    cbSelPets.Items.Add("Medic");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Spec Ops":
                    cbSelPets.Items.Add("Spec Ops");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Commando":
                    cbSelPets.Items.Add("Commando");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Zombie Horde":
                    cbSelPets.Items.Add("Zombie");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Grave Knight":
                    cbSelPets.Items.Add("Skeletal Warrior");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Soul Extraction":
                    cbSelPets.Items.Add("Ghost");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Lich":
                    cbSelPets.Items.Add("Lich");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Call Genin":
                    cbSelPets.Items.Add("Genin");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Call Jounin":
                    cbSelPets.Items.Add("Jounin");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Oni":
                    cbSelPets.Items.Add("Oni");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Battle Drones":
                    cbSelPets.Items.Add("Droid");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Protector Bots":
                    cbSelPets.Items.Add("Protector Bot");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Assault Bot":
                    cbSelPets.Items.Add("Assault Bot");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Call Thugs":
                    cbSelPets.Items.Add("Thug");
                    cbSelPets.Items.Add("Arsonist");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Call Enforcer":
                    cbSelPets.Items.Add("Thug Lt");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Gang War":
                    cbSelPets.Items.Add("Thug Posse");
                    cbSelPets.SelectedIndex = 0;
                    break;
                case "Call Bruiser":
                    cbSelPets.Items.Add("Thug Boss");
                    cbSelPets.SelectedIndex = 0;
                    break;
            }
            
        }

        void cbSelPetPower_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbSelPets.Items.Clear();
            UpdatePetsListCombo();
        }

        void cbSelPet_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var pow = DatabaseAPI.NidFromUidPower("Mastermind_Summon.Demon_Summoning.Summon_Demons");
            //var psetp = DatabaseAPI.Database.Power[pow].FXGetDamageValue();
            //MessageBox.Show(psetp.ToString());
            _myPetPowers = new List<IPower>();
            var ent = DatabaseAPI.NidFromUidEntity($"Pets_{cbSelPets.SelectedItem.ToString().Replace(" ", "_")}");
            var pset = DatabaseAPI.Database.Entities[ent].GetNPowerset();
            foreach (var entity in pset)
            {
                var powers = DatabaseAPI.Database.Powersets[entity].Powers;
                foreach (var power in powers)
                {
                    /*Calculates damage effects and outputs as string
                    var dmgval = power.FXGetDamageString();*/

                    _myPetPowers.Add(power);
                }
            }
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

        void llLeft_ItemClick(ListLabelV2.ListLabelItemV2 Item, MouseButtons Button)
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
                if (Item.ItemState == ListLabelV2.LLItemState.Disabled)
                    return;
                if (MidsContext.Character.CurrentBuild.PowerUsed(_myPetPowers[Item.Index]))
                {
                    MidsContext.Character.CurrentBuild.RemovePower(_myPetPowers[Item.Index]);
                    Item.ItemState = ListLabelV2.LLItemState.Enabled;
                }
                else
                {
                    MidsContext.Character.CurrentBuild.AddPower(_myPetPowers[Item.Index]).StatInclude = true;
                    Item.ItemState = ListLabelV2.LLItemState.Selected;
                }
                llLeft.Refresh();
                _myParent.PowerModified(markModified: false);
            }
        }

        void llLeft_ItemHover(ListLabelV2.ListLabelItemV2 Item)
        {
            MiniPowerInfo(Item.Index);
        }

        void llLeft_MouseEnter(object sender, EventArgs e)
        {
            if (!ContainsFocus)
                return;
            Panel2.Focus();
        }

        void llRight_ItemClick(ListLabelV2.ListLabelItemV2 Item, MouseButtons Button)
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
                if (Item.ItemState == ListLabelV2.LLItemState.Disabled)
                    return;
                if (MidsContext.Character.CurrentBuild.PowerUsed(_myPetPowers[pIDX]))
                {
                    MidsContext.Character.CurrentBuild.RemovePower(_myPetPowers[pIDX]);
                    Item.ItemState = ListLabelV2.LLItemState.Enabled;
                }
                else
                {
                    MidsContext.Character.CurrentBuild.AddPower(_myPetPowers[pIDX]).StatInclude = true;
                    Item.ItemState = ListLabelV2.LLItemState.Selected;
                }
                llRight.Refresh();
                _myParent.PowerModified(markModified: false);
            }
        }

        void llRight_ItemHover(ListLabelV2.ListLabelItemV2 Item)
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
            IPower power1 = new Power(_myPetPowers[pIDX]);
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

        static void UpdateLlColours(ref ListLabelV2 iList)
        {
            iList.UpdateTextColors(ListLabelV2.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            iList.UpdateTextColors(ListLabelV2.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            iList.UpdateTextColors(ListLabelV2.LLItemState.Selected, MidsContext.Config.RtFont.ColorPowerTaken);
            iList.UpdateTextColors(ListLabelV2.LLItemState.SelectedDisabled, MidsContext.Config.RtFont.ColorPowerTakenDark);
            iList.UpdateTextColors(ListLabelV2.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            iList.HoverColor = MidsContext.Config.RtFont.ColorPowerHighlight;
            iList.Font = new Font(iList.Font.FontFamily, MidsContext.Config.RtFont.PairedBase, FontStyle.Bold, GraphicsUnit.Point);
        }

        void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            PopInfo.ScrollY = (float)(VScrollBar1.Value / (double)(VScrollBar1.Maximum - VScrollBar1.LargeChange) * (PopInfo.lHeight - (double)Panel1.Height));
        }
    }
}
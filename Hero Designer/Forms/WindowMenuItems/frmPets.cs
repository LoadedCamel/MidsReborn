using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
    public partial class frmPets : Form
    {
        ImageButton ibClose;

        ComboBox cbSelPetPower;
        ComboBox cbSelPets;

        ListLabelV3 llPrimary;
        Label lblPrimary;
        FlowLayoutPanel pnlGFXFlow;
        PictureBox pnlGFX;

        ExtendedBitmap dmBuffer;
        clsDrawX drawing;
        DataView myDataView;
        frmFloatingStats FloatingDataForm;

        readonly frmMain _myParent;

        List<IPower> _myPowers;

        private List<string> PetPowers { get; }

        public frmPets(frmMain iParent, List<string> PetPowersList)
        {
            Load += frmPets_Load;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmPets));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmPets);
            _myParent = iParent;
            PetPowers = PetPowersList;
            FormClosing += FrmPets_FormClosing;
        }

        private void FrmPets_FormClosing(object sender, FormClosingEventArgs e)
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

        void frmPets_Load(object sender, EventArgs e)
        {
            BackColor = _myParent.BackColor;
            ibClose.IA = _myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero() ? _myParent.Drawing.bxPower[2].Bitmap : _myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = _myParent.Drawing.bxPower[3].Bitmap;
            UpdatePetPowersCombo(PetPowers);
            UpdatePetsListCombo();
        }

        private void UpdatePetPowersCombo(List<string> pPowersList)
        {
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
            _myPowers = new List<IPower>();
            var ent = DatabaseAPI.NidFromUidEntity($"Pets_{cbSelPets.SelectedItem.ToString().Replace(" ", "_")}");
            var pset = DatabaseAPI.Database.Entities[ent].GetNPowerset();
            foreach (var entity in pset)
            {
                var powers = DatabaseAPI.Database.Powersets[entity].Powers;
                foreach (var power in powers)
                {
                    _myPowers.Add(power);
                }
            }
        }

        void ibClose_ButtonClicked()
        {
            Close();
        }

    }
}

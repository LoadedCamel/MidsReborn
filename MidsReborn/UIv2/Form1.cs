using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FontAwesome.Sharp;
using Mids_Reborn.Forms;
using Mids_Reborn.My;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.UIv2
{
    public partial class Form1 : Form
    {

        private bool GetPlayableClasses(Archetype a) => a.Playable;
        private frmInitializing _frmInitializing;
        private bool Loading { get; set; }

        private readonly int _panelWidth;
        private bool PanelHidden { get; set; }

        private float OriginalWidth { get; set; }
        private float OriginalHeight { get; set; }

        public Form1()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.ResizeRedraw, true);
            ConfigData.Initialize(MyApplication.GetSerializer());
            Load += Form1_Load;
            Resize += Form1_Resize;
            InitializeComponent();
            _panelWidth = 350;
            PanelHidden = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            float widthRatio = ClientRectangle.Width / OriginalWidth;
            float heightRatio = ClientRectangle.Height / OriginalHeight;
            SizeF scale = new SizeF(widthRatio, heightRatio);
            OriginalHeight = ClientRectangle.Height;
            OriginalWidth = ClientRectangle.Width;

            foreach (Control control in Controls)
            {
                control.Font = new Font(control.Font.FontFamily, control.Font.SizeInPoints * heightRatio * widthRatio);
                control.Scale(scale);
                foreach (Control child in control.Controls)
                {
                    child.Font = new Font(child.Font.FontFamily, child.Font.SizeInPoints * heightRatio * widthRatio);
                    child.Scale(scale);
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Loading = true;
            try
            {
                OriginalWidth = ClientRectangle.Width;
                OriginalHeight = ClientRectangle.Height;
                if (MidsContext.Config.I9.DefaultIOLevel == 27)
                {
                    MidsContext.Config.I9.DefaultIOLevel = 49;
                }

                using frmInitializing iFrm = new frmInitializing();
                _frmInitializing = iFrm;
                _frmInitializing.Show();
                if (!this.IsInDesignMode() && !MidsContext.Config.IsInitialized)
                {
                    MidsContext.Config.CheckForUpdates = false;
                    MidsContext.Config.DefaultSaveFolderOverride = null;
                    MidsContext.Config.CreateDefaultSaveFolder();
                    MidsContext.Config.IsInitialized = true;
                }
                MainModule.MidsController.LoadData(ref _frmInitializing);
                _frmInitializing?.SetMessage("Setting up UI...");

                Show();
                _frmInitializing.Hide();
                _frmInitializing.Close();
                Refresh();
                Loading = false;
                Fill_Initial_Combos();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}\r\n\n{exception.StackTrace}");
            }
            Loading = false;
            cbAT.SelectedItem = cbAT.Items[0];
        }

        private void ButtonMouse_Enter(object sender, EventArgs e)
        {
            IconButton ib = (IconButton) sender;
            ib.IconColor = Color.DodgerBlue;
            ib.FlatAppearance.MouseOverBackColor = Color.FromArgb(170,0,0,0);
            ib.FlatAppearance.MouseDownBackColor = Color.FromArgb(170, 0, 0, 0);
        }

        private void ButtonMouse_Leave(object sender, EventArgs e)
        {
            IconButton ib = (IconButton)sender;
            ib.IconColor = Color.White;
            ib.FlatAppearance.MouseOverBackColor = Color.FromArgb(170, 0, 0, 0);
            ib.FlatAppearance.MouseDownBackColor = Color.FromArgb(170, 0, 0, 0);
        }

        private void MenuGrip_Enter(object sender, EventArgs e)
        {
            IconButton ib = (IconButton)sender;
            ib.IconColor = Color.DodgerBlue;
            ib.FlatAppearance.MouseOverBackColor = Color.Transparent;
            ib.FlatAppearance.MouseDownBackColor = Color.Transparent;
        }

        private void MenuGrip_Leave(object sender, EventArgs e)
        {
            IconButton ib = (IconButton)sender;
            ib.IconColor = Color.White;
            ib.FlatAppearance.MouseOverBackColor = Color.Transparent;
            ib.FlatAppearance.MouseDownBackColor = Color.Transparent;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            //Add confirmation of save event?
            Application.Exit();
        }

        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Normal:
                    WindowState = FormWindowState.Maximized;
                    break;
                case FormWindowState.Maximized:
                    WindowState = FormWindowState.Normal;
                    break;
                case FormWindowState.Minimized:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private int _movX;
        private int _movY;
        private bool _isMoving;

        private void Move_OnMouseDown(object sender, MouseEventArgs e)
        {
            // Assign this method to mouse_Down event of Form or Panel,whatever you want
            _isMoving = true;
            _movX = e.X;
            _movY = e.Y;
        }

        private void Move_OnMouseMove(object sender, MouseEventArgs e)
        {
            // Assign this method to Mouse_Move event of that Form or Panel
            if (_isMoving)
            {
                this.SetDesktopLocation(MousePosition.X - _movX, MousePosition.Y - _movY);
            }
        }

        private void Move_OnMouseUp(object sender, MouseEventArgs e)
        {
            // Assign this method to Mouse_Up event of Form or Panel.
            _isMoving = false;
        }

        private void PowerButton_PaintSlot(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using Pen pen = new Pen(Color.White, 1);
            //Rectangle rectangle = new Rectangle(point, 20, 30, 30);
            //g.DrawEllipse(pen, rectangle);
        }

        private void Fill_Initial_Combos()
        {
            FillArchetypeCombo();
            FillOriginCombo();
            FillPool0Combo();
            FillPool1Combo();
            FillPool2Combo();
            FillPool3Combo();
        }
        private void FillOriginCombo()
        {
            var selectedArchetype = (Archetype) cbAT.SelectedItem;
            cbOrigin.DataSource = selectedArchetype.Origin;
        }

        private void FillArchetypeCombo()
        {
            List<Archetype> playableArchetypes = Array.FindAll(DatabaseAPI.Database.Classes, GetPlayableClasses).ToList();
            cbAT.DisplayMember = "DisplayName";
            cbAT.ValueMember = null;
            cbAT.DataSource = playableArchetypes;
        }
        private void cbAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAT.SelectedIndex < 0)
                return;
            var selectedArchetype = (Archetype)cbAT.Items[cbAT.SelectedIndex];
            if (selectedArchetype.DisplayName.Contains("Arachnos"))
            {
                cbSecondary.SelectedIndexChanged -= cbSecondary_SelectedIndexChanged;
            }
            else
            {
                cbSecondary.SelectedIndexChanged += cbSecondary_SelectedIndexChanged;
            }
            //MidsContext.Character.Archetype = selectedArchetype;
            FillPrimaryCombo(selectedArchetype);
            FillSecondaryCombo(selectedArchetype);
            FillAncillaryCombo(selectedArchetype);
        }

        public void FillPrimaryCombo(Archetype selectedArchetype)
        {
            List<IPowerset> powersets = Array.FindAll(DatabaseAPI.Database.Powersets, p => p.SetType == Enums.ePowerSetType.Primary && p.ATClass.Equals(selectedArchetype.ClassName)).ToList();
            cbPrimary.DisplayMember = "DisplayName";
            cbPrimary.ValueMember = null;
            cbPrimary.DataSource = powersets;
            cbPrimary.SelectedIndex = 0;
        }
        private void cbPrimary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPrimary.SelectedIndex < 0)
                return;
            IPowerset selectedPri;
            var primaryList = new BindingList<IPower>();
            var selectedPowerset = (Powerset) cbPrimary.Items[cbPrimary.SelectedIndex];
            switch (selectedPowerset.DisplayName)
            {
                case "Arachnos Soldier":
                    primaryList = new BindingList<IPower>(selectedPowerset.Powers);
                    cbSecondary.SelectedIndex = 2;
                    break;
                case "Bane Spider Soldier":
                    selectedPri = DatabaseAPI.GetPowersetByName("Arachnos Soldier", Enums.ePowerSetType.Primary);
                    foreach (var power in selectedPri.Powers)
                        primaryList.Add(power);
                    foreach (var power in selectedPowerset.Powers)
                        primaryList.Add(power);
                    cbSecondary.SelectedIndex = 0;
                    break;
                case "Crab Spider Soldier":
                    selectedPri = DatabaseAPI.GetPowersetByName("Arachnos Soldier", Enums.ePowerSetType.Primary);
                    foreach (var power in selectedPri.Powers)
                        primaryList.Add(power);
                    foreach (var power in selectedPowerset.Powers)
                        primaryList.Add(power);
                    cbSecondary.SelectedIndex = 1;
                    break;
                case "Fortunata Training":
                    selectedPri = DatabaseAPI.GetPowersetByName("Widow Training", Enums.ePowerSetType.Primary);
                    foreach (var power in selectedPri.Powers)
                        primaryList.Add(power);
                    foreach (var power in selectedPowerset.Powers)
                        primaryList.Add(power);
                    cbSecondary.SelectedIndex = 0;
                    break;
                case "Night Widow Training":
                    selectedPri = DatabaseAPI.GetPowersetByName("Widow Training", Enums.ePowerSetType.Primary);
                    foreach (var power in selectedPri.Powers)
                        primaryList.Add(power);
                    foreach (var power in selectedPowerset.Powers)
                        primaryList.Add(power);
                    cbSecondary.SelectedIndex = 2;
                    break;
                case "Widow Training":
                    primaryList = new BindingList<IPower>(selectedPowerset.Powers);
                    cbSecondary.SelectedIndex = 1;
                    break;
                default:
                    primaryList = new BindingList<IPower>(selectedPowerset.Powers);
                    break;
            }
            FillPrimaryPowers(primaryList);
        }

        public void FillPrimaryPowers(BindingList<IPower> primaryPowers)
        {
            Primary_Powers.SelectionMode = SelectionMode.None;
            Primary_Powers.DisplayMember = "DisplayName";
            Primary_Powers.ValueMember = null;
            Primary_Powers.DataSource = primaryPowers;
            Primary_Powers.SelectionMode = SelectionMode.MultiSimple;
        }

        public void FillSecondaryCombo(Archetype selectedArchetype)
        {
            List<IPowerset> powersets = Array.FindAll(DatabaseAPI.Database.Powersets, p => p.SetType == Enums.ePowerSetType.Secondary && p.ATClass.Equals(selectedArchetype.ClassName)).ToList();
            cbSecondary.DisplayMember = "DisplayName";
            cbSecondary.ValueMember = null;
            cbSecondary.DataSource = powersets;
            if (cbPrimary.SelectedIndex == 0)
            {
                cbPrimary_SelectedIndexChanged(cbPrimary, new EventArgs());
            }
        }

        private void cbSecondary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSecondary.SelectedIndex < 0)
                return;
            IPowerset selectedSec;
            var secondaryList = new BindingList<IPower>();
            var selectedPowerset = (Powerset) cbSecondary.Items[cbSecondary.SelectedIndex];
            switch (selectedPowerset.DisplayName)
            {
                case "Bane Spider Training":
                    selectedSec =
                        DatabaseAPI.GetPowersetByName("Training and Gadgets", Enums.ePowerSetType.Secondary);
                    foreach (var power in selectedSec.Powers)
                        secondaryList.Add(power);
                    foreach (var power in selectedPowerset.Powers)
                        secondaryList.Add(power);
                    break;
                case "Crab Spider Training":
                    selectedSec =
                        DatabaseAPI.GetPowersetByName("Training and Gadgets", Enums.ePowerSetType.Secondary);
                    foreach (var power in selectedSec.Powers)
                        secondaryList.Add(power);
                    foreach (var power in selectedPowerset.Powers)
                        secondaryList.Add(power);
                    break;
                case "Fortunata Teamwork":
                    selectedSec = DatabaseAPI.GetPowersetByName("Teamwork", Enums.ePowerSetType.Secondary);
                    foreach (var power in selectedSec.Powers)
                        secondaryList.Add(power);
                    foreach (var power in selectedPowerset.Powers)
                        secondaryList.Add(power);
                    break;
                case "Widow Teamwork":
                    selectedSec = DatabaseAPI.GetPowersetByName("Teamwork", Enums.ePowerSetType.Secondary);
                    foreach (var power in selectedSec.Powers)
                        secondaryList.Add(power);
                    foreach (var power in selectedPowerset.Powers)
                        secondaryList.Add(power);
                    break;
                default:
                    secondaryList = new BindingList<IPower>(selectedPowerset.Powers);
                    break;
            }

            FillSecondaryPowers(secondaryList);
        }

        public void FillSecondaryPowers(BindingList<IPower> secondaryPowers)
        {
            Secondary_Powers.SelectionMode = SelectionMode.None;
            Secondary_Powers.DisplayMember = "DisplayName";
            Secondary_Powers.ValueMember = null;
            Secondary_Powers.DataSource = secondaryPowers;
            Secondary_Powers.SelectionMode = SelectionMode.MultiSimple;
        }

        public void FillAncillaryCombo(Archetype selectedArchetype)
        {
            if (selectedArchetype.DisplayName != "Peacebringer" && selectedArchetype.DisplayName != "Warshade")
            {
                var powersets = selectedArchetype.Ancillary.Select(t => DatabaseAPI.Database.Powersets.FirstOrDefault(p => p.SetType == Enums.ePowerSetType.Ancillary && p.nID.Equals(t))).ToList();
                cbAncillary.DisplayMember = "DisplayName";
                cbAncillary.ValueMember = null;
                cbAncillary.DataSource = powersets;
            }
            else
            {
                cbAncillary.DataSource = null;
                cbAncillary.Items.Clear();
            }
        }

        private void cbAncillary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAncillary.SelectedIndex < 0)
            {
                Ancillary_Powers.DataSource = new BindingList<string>();
                Ancillary_Powers.Update();
                Ancillary_Powers.Refresh();
                return;
            }
            var selectedPowerset = (Powerset)cbAncillary.Items[cbAncillary.SelectedIndex];
            FillAncillaryPowers(new BindingList<IPower>(selectedPowerset.Powers));
        }

        public void FillAncillaryPowers(BindingList<IPower> ancillaryPowers)
        {
            Ancillary_Powers.SelectionMode = SelectionMode.None;
            Ancillary_Powers.DisplayMember = "DisplayName";
            Ancillary_Powers.ValueMember = null;
            Ancillary_Powers.DataSource = ancillaryPowers;
            Ancillary_Powers.SelectionMode = SelectionMode.MultiSimple;
        }

        public void FillPool0Combo()
        {
            List<IPowerset> powersets = Array.FindAll(DatabaseAPI.Database.Powersets, p => p.SetType == Enums.ePowerSetType.Pool).ToList();
            cbPool0.DisplayMember = "DisplayName";
            cbPool0.ValueMember = null;
            cbPool0.DataSource = powersets;
            cbPool0.SelectedIndex = 0;
        }

        public void cbPool0_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPool0.SelectedIndex < 0)
                return;
            var selectedPowerset = (Powerset) cbPool0.Items[cbPool0.SelectedIndex];
            FillPool0Powers(new BindingList<IPower>(selectedPowerset.Powers));
        }

        public void FillPool0Powers(BindingList<IPower> poolPowers)
        {
            Pool0_Powers.SelectionMode = SelectionMode.None;
            Pool0_Powers.DisplayMember = "DisplayName";
            Pool0_Powers.ValueMember = null;
            Pool0_Powers.DataSource = poolPowers;
            Pool0_Powers.SelectionMode = SelectionMode.MultiSimple;
        }

        public void FillPool1Combo()
        {
            List<IPowerset> powersets = Array.FindAll(DatabaseAPI.Database.Powersets, p => p.SetType == Enums.ePowerSetType.Pool).ToList();
            cbPool1.DisplayMember = "DisplayName";
            cbPool1.ValueMember = null;
            cbPool1.DataSource = powersets;
            cbPool1.SelectedIndex = 1;
        }

        public void cbPool1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPool1.SelectedIndex < 0)
                return;
            var selectedPowerset = (Powerset)cbPool1.Items[cbPool1.SelectedIndex];
            FillPool1Powers(new BindingList<IPower>(selectedPowerset.Powers));
        }

        public void FillPool1Powers(BindingList<IPower> poolPowers)
        {
            Pool1_Powers.SelectionMode = SelectionMode.None;
            Pool1_Powers.DisplayMember = "DisplayName";
            Pool1_Powers.ValueMember = null;
            Pool1_Powers.DataSource = poolPowers;
            Pool1_Powers.SelectionMode = SelectionMode.MultiSimple;
        }
        public void FillPool2Combo()
        {
            List<IPowerset> powersets = Array.FindAll(DatabaseAPI.Database.Powersets, p => p.SetType == Enums.ePowerSetType.Pool).ToList();
            cbPool2.DisplayMember = "DisplayName";
            cbPool2.ValueMember = null;
            cbPool2.DataSource = powersets;
            cbPool2.SelectedIndex = 2;
        }

        public void cbPool2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPool2.SelectedIndex < 0)
                return;
            var selectedPowerset = (Powerset)cbPool2.Items[cbPool2.SelectedIndex];
            FillPool2Powers(new BindingList<IPower>(selectedPowerset.Powers));
        }

        public void FillPool2Powers(BindingList<IPower> poolPowers)
        {
            Pool2_Powers.SelectionMode = SelectionMode.None;
            Pool2_Powers.DisplayMember = "DisplayName";
            Pool2_Powers.ValueMember = null;
            Pool2_Powers.DataSource = poolPowers;
            Pool2_Powers.SelectionMode = SelectionMode.MultiSimple;
        }
        public void FillPool3Combo()
        {
            List<IPowerset> powersets = Array.FindAll(DatabaseAPI.Database.Powersets, p => p.SetType == Enums.ePowerSetType.Pool).ToList();
            cbPool3.DisplayMember = "DisplayName";
            cbPool3.ValueMember = null;
            cbPool3.DataSource = powersets;
            cbPool3.SelectedIndex = 3;
        }

        public void cbPool3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPool3.SelectedIndex < 0)
                return;
            var selectedPowerset = (Powerset)cbPool3.Items[cbPool3.SelectedIndex];
            FillPool3Powers(new BindingList<IPower>(selectedPowerset.Powers));
        }

        public void FillPool3Powers(BindingList<IPower> poolPowers)
        {
            Pool3_Powers.SelectionMode = SelectionMode.None;
            Pool3_Powers.DisplayMember = "DisplayName";
            Pool3_Powers.ValueMember = null;
            Pool3_Powers.DataSource = poolPowers;
            Pool3_Powers.SelectionMode = SelectionMode.MultiSimple;
        }

        private void cbOrigin_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}

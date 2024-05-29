using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class FrmEntityDetails : Form
    {
        public delegate void PowerIncludeChangedEventHandler(IPower power);
        public event PowerIncludeChangedEventHandler PowerIncludeChanged;

        public delegate void PetViewSliderUpdatedEventHandler();
        public event PetViewSliderUpdatedEventHandler PetViewSliderUpdated;
        
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;
        private const int WM_NCLBUTTONDOWN = 0xA1;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private string _entityUid;
        private List<string> _powers;
        private SummonedEntity? _entityData;
        private List<IPower?>? _powersData;

        private bool DvLocked
        {
            get => petView1.Lock;
            set => petView1.Lock = value;
        }
        private IPower? _dvPowerBase;
        private IPower? _dvPowerEnh;
        private readonly PetInfo _petInfo;

        /// <summary>
        /// Initialize entity data
        /// </summary>
        /// <param name="entityUid">Entity UID</param>
        /// <param name="powers">Entity's powers, as a HashSet</param>
        /// <param name="petInfo">PetInfo instance</param>
        public FrmEntityDetails(string entityUid, HashSet<string> powers, PetInfo petInfo)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
            Closed += OnClosed;
            _entityUid = entityUid;
            _powers = powers.ToList();
            _petInfo = petInfo;
            _petInfo.PowersUpdated += PetInfoOnPowersUpdated;
            InitializeComponent();
            powersCombo1.SelectedPowersIndexChanged += PowersCombo1OnSelectedPowersIndexChanged;
            powersCombo1.MouseDown += PowersCombo1OnMouseDown;
        }

        /// <summary>
        /// Initialize entity data
        /// </summary>
        /// <param name="entityUid">Entity UID</param>
        /// <param name="powers">Entity's powers, as a List</param>
        /// <param name="petInfo">PetInfo instance</param> 
        public FrmEntityDetails(string entityUid, List<string> powers, PetInfo petInfo)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
            Closed += OnClosed;
            _entityUid = entityUid;
            _powers = powers;
            _petInfo = petInfo;
            _petInfo.PowersUpdated += PetInfoOnPowersUpdated;
            InitializeComponent();
            powersCombo1.SelectedPowersIndexChanged += PowersCombo1OnSelectedPowersIndexChanged;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        // This is all you need to allow the window to be movable
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WM_NCLBUTTONDBLCLK)
            {
                message.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref message);

            if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT)
                message.Result = (IntPtr)HTCAPTION;
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            MidsContext.Config.EntityDetailsLocation = Location;
        }

        private void PetInfoOnPowersUpdated(object? sender, EventArgs e)
        {
            var petPower = _petInfo.GetPetPower();
            _dvPowerBase = petPower?.BasePower;
            _dvPowerEnh = petPower?.BuffedPower;
            if (DvLocked) return;
            if (_dvPowerEnh != null) petView1.SetData(_dvPowerBase, _dvPowerEnh);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnTopMost_Click(object sender, EventArgs e)
        {
            switch (btnTopMost.ToggleState)
            {
                case ImageButtonEx.States.ToggledOff:
                    btnTopMost.ToggleState = ImageButtonEx.States.ToggledOn;
                    btnTopMost.Text = btnTopMost.ToggleText.ToggledOn;
                    TopMost = true;
                    break;

                default:
                    btnTopMost.ToggleState = ImageButtonEx.States.ToggledOff;
                    btnTopMost.Text = btnTopMost.ToggleText.ToggledOff;
                    TopMost = false;
                    break;
            }
        }

        private void SetTitleText(string text, bool adjustFontSize = true, float minFontSize = 9, float maxFontSize = 14.25f)
        {
            if (!adjustFontSize)
            {
                lblEntityName.Text = text;

                return;
            }

            const float fontSizeIncrement = 0.5f;
            for (var i = maxFontSize; i > minFontSize; i -= fontSizeIncrement)
            {
                var font = new Font("Segoe UI Variable Display", i, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);
                var textSize = TextRenderer.MeasureText(text, font);
                if (textSize.Width > lblEntityName.Width - 3)
                {
                    continue;
                }

                lblEntityName.Font = font;
                lblEntityName.Text = text;

                return;
            }
        }

        private void frmEntityDetails_Load(object sender, EventArgs e)
        {
            var owner = (frmMain)Owner;

            Location = MidsContext.Config.EntityDetailsLocation == null
                ? new Point(owner.Location.X + 10, owner.Bottom - Height - 10)
                : (Point)MidsContext.Config.EntityDetailsLocation;

            switch (TopMost)
            {
                case true:
                    btnTopMost.ToggleState = ImageButtonEx.States.ToggledOn;
                    btnTopMost.Text = btnTopMost.ToggleText.ToggledOn;
                    break;

                case false:
                    btnTopMost.ToggleState = ImageButtonEx.States.ToggledOff;
                    btnTopMost.Text = btnTopMost.ToggleText.ToggledOff;
                    break;
            }

            // Hide Totals and Enhance tabs
            //petView1.TabsMask = new[] {true, true, false, false};
            //petView1.BackColor = BackColor;
            //petView1.Refresh();

            Text = _entityData != null ? $"Entity Details: {_entityData.DisplayName}" : "Entity Details";

            _entityData = DatabaseAPI.Database.Entities
                .DefaultIfEmpty(null)
                .FirstOrDefault(en => en?.UID == _entityUid);

            _powersData = _powers
                .Select(DatabaseAPI.GetPowerByFullName).Where(p => !p.DisplayName.Contains("Granter"))
                .ToList();

            var entityName = _entityData == null
                ? "Entity Details"
                : $"Entity: {_entityData.DisplayName}";

            SetTitleText(entityName);
            UpdateColorTheme(MidsContext.Character == null ? Enums.Alignment.Hero : MidsContext.Character.Alignment);

            powersCombo1.DisplayMember = "DisplayName";
            powersCombo1.ValueMember = null;
            powersCombo1.DataSource = _powersData;
            powersCombo1.SelectedIndex = 0;

            petView1.SetGraphType(Enums.eDDGraph.Simple, Enums.eDDStyle.TextUnderGraph);
            petView1.UseAlt = MidsContext.Character.IsVillain;
            _petInfo.ExecuteUpdate();
        }

        private void PowersCombo1OnSelectedPowersIndexChanged(object? sender, EventArgs e)
        {
            var power = (IPower)powersCombo1.Items[powersCombo1.SelectedIndex];
            var petPower = _petInfo.GetPetPower(power);
            _dvPowerBase = petPower?.BasePower;
            _dvPowerEnh = petPower?.BuffedPower;

            if (DvLocked)
            {
                return;
            }

            cbPowerInclude.Visible = power.ClickBuff | power.PowerType == Enums.ePowerType.Toggle;
            cbPowerInclude.Checked = MidsContext.Character.CurrentBuild.PowerUsed(power);

            if (_dvPowerEnh != null) petView1.SetData(_dvPowerBase, _dvPowerEnh);
        }

        private void PowersCombo1OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            DvLocked = DvLocked switch
            {
                true => false,
                false => true
            };
            if (_dvPowerEnh != null) petView1.SetData(_dvPowerBase, _dvPowerEnh, false, DvLocked);
        }

        public void UpdateData()
        {
            Text = _entityData != null ? $"Entity Details: {_entityData.DisplayName}" : "Entity Details";

            _petInfo.ExecuteUpdate();
            if (TopMost) BringToFront();
        }

        /// <summary>
        /// Update and refresh data
        /// </summary>
        /// <param name="entityUid">Entity UID</param>
        /// <param name="powers">Entity's powers, as a HashSet</param>
        public void UpdateData(string entityUid, HashSet<string> powers)
        {
            UpdateData(entityUid, powers.ToList());
        }

        /// <summary>
        /// Update and refresh data
        /// </summary>
        /// <param name="entityUid">Entity UID</param>
        /// <param name="powers">Entity's powers, as a List</param>
        public void UpdateData(string entityUid, List<string> powers)
        {
            _entityUid = entityUid;
            _powers = powers;

            _entityData = DatabaseAPI.Database.Entities
                .DefaultIfEmpty(null)
                .FirstOrDefault(en => en?.UID == _entityUid);

            _powersData = _powers
                .Select(DatabaseAPI.GetPowerByFullName)
                .ToList();

            Text = _entityData != null ? $"Entity Details: {_entityData.DisplayName}" : "Entity Details";

            _petInfo.ExecuteUpdate();

            //ListPowers();
            if (TopMost) BringToFront();
        }

        /// <summary>
        /// Update controls color theme from character alignment
        /// </summary>
        /// <param name="cAlignment">Character alignment</param>
        public void UpdateColorTheme(Enums.Alignment cAlignment)
        {
            var charVillain = cAlignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
            btnTopMost.UseAlt = charVillain;
            btnClose.UseAlt = charVillain;
            petView1.UseAlt = charVillain;
        }

        // Bug: DisplayLocation increases + 1 on every use
        // Bug (CurrentBuild.AddPower): power modifications are ignored and have to be set on the resulting PowerEntry after the call
        // Bug: Entity name is appended on every use and change every occurrence of the power (entity details, dataview, database editor...)
        // To be improved: checkbox responsiveness is very slow (1-2s freeze delay)
        private void cbPowerInclude_CheckedChanged(object sender, EventArgs e)
        {
            var power = new Power((IPower)powersCombo1.Items[powersCombo1.SelectedIndex]);

            if (MidsContext.Character?.CurrentBuild?.PowerUsed(power) == false)
            {
                var pe = MidsContext.Character.CurrentBuild.AddPower(power, 49);
                pe.StatInclude = true; // Toggle on
                pe.Power.IncludeFlag = true; // Show in inherents
                pe.Power.InherentType = Enums.eGridType.Pet;
                pe.Power.DisplayName += _entityData == null
                    ? ""
                    : $" ({_entityData.DisplayName})";
                pe.Power.DisplayLocation = (int)MidsContext.Character?.CurrentBuild?.Powers // Pick first available index on the inherents grid
                    .Where(e => e?.Power is { IncludeFlag: true })
                    .Select(e => e?.Power?.DisplayLocation)
                    .Max() + 1;
                //Debug.WriteLine($"- IncludeFlag: {pe.Power.IncludeFlag}, DisplayLocation: {pe.Power.DisplayLocation}, NIDPower: {pe.NIDPower}, NIDPowerset: {pe.NIDPowerset}");
            }
            else
            {
                MidsContext.Character?.CurrentBuild?.RemovePower(power);
            }

            PowerIncludeChanged?.Invoke(power);
        }

        private void borderPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private void petView1_SlotUpdate()
        {
            //PetViewSliderUpdated?.Invoke();
        }
    }
}
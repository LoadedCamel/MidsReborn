using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class FrmEntityDetails : Form
    {
        private string _entityUid;
        private List<string> _powers;
        private readonly int _basePowerHistoryIdx;
        private List<IPower?> _mathPower;
        private List<IPower?> _buffedPower;

        private SummonedEntity? _entityData;
        private List<IPower?>? _powersData;
        private ListLabelV3? _powersList;
        private int _hoveredItemIndex = -1;

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
        /// <param name="basePowerHistoryIdx">PowerEntry index</param>
        /// <param name="petInfo">PetInfo instance</param>
        public FrmEntityDetails(string entityUid, HashSet<string> powers, int basePowerHistoryIdx, PetInfo petInfo)
        {
            _entityUid = entityUid;
            _powers = powers.ToList();
            _basePowerHistoryIdx = basePowerHistoryIdx;
            _petInfo = petInfo;
            _petInfo.PowersUpdated += PetInfoOnPowersUpdated;
            InitializeComponent();
        }

        private void PetInfoOnPowersUpdated(object? sender, EventArgs e)
        {
            var petPower = _petInfo.GetPetPower();
            _dvPowerBase = petPower?.BasePower;
            _dvPowerEnh = petPower?.BuffedPower;
            if (DvLocked) return;
            if (_dvPowerEnh != null) petView1.SetData(_dvPowerBase, _dvPowerEnh);
        }

        /// <summary>
        /// Initialize entity data
        /// </summary>
        /// <param name="entityUid">Entity UID</param>
        /// <param name="powers">Entity's powers, as a List</param>
        /// <param name="basePowerHistoryIdx">PowerEntry index</param>
        /// <param name="petInfo">PetInfo instance</param> 
        public FrmEntityDetails(string entityUid, List<string> powers, int basePowerHistoryIdx, PetInfo petInfo)
        {
            _entityUid = entityUid;
            _powers = powers;
            _basePowerHistoryIdx = basePowerHistoryIdx;
            _petInfo = petInfo;
            _petInfo.PowersUpdated += PetInfoOnPowersUpdated;
            InitializeComponent();
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

        private void frmEntityDetails_Load(object sender, EventArgs e)
        {
            CenterToParent();
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
                .Select(DatabaseAPI.GetPowerByFullName)
                .ToList();

            var buffedPowers = MainModule.MidsController.Toon?.GenerateBuffedPowers(_powersData, _basePowerHistoryIdx);
            if (buffedPowers == null)
            {
                _mathPower = _powersData.Clone();
                _buffedPower = _powersData.Clone();
            }
            else
            {
                _mathPower = buffedPowers.Value.Key;
                _buffedPower = buffedPowers.Value.Value;
            }

            lblEntityName.Text = _entityData == null
                ? "Entity Details"
                : $"Entity: {_entityData.DisplayName}";

            _powersList = new ListLabelV3
            {
                Location = petView1.Location with {X = petView1.Location.X + petView1.Size.Width + 10},
                HighVis = true,
                Size = new Size(180, petView1.Size.Height - 70),
                SizeNormal = new Size(180, petView1.Size.Height - 70),
                BackColor = Color.Black,
                ForeColor = Color.AliceBlue,
                Expandable = false,
                ActualLineHeight = 18,
                HoverColor = Color.DimGray,
                Scrollable = false,
            };

            _powersList.ItemHover += powersList_ItemHover;
            _powersList.MouseDown += powersList_MouseDown;
            Controls.Add(_powersList);

            _powersList.SuspendRedraw = true;
            SetPowersFont();
            SetPowerColors();
            _powersList.SuspendRedraw = false;

            ListPowers();
            _powersList.Size = new Size(180, Math.Min(petView1.Size.Height - 70, Math.Max(120, _powersList.DesiredHeight)));
            _powersList.SizeNormal = new Size(180, Math.Min(petView1.Size.Height - 70, Math.Max(120, _powersList.DesiredHeight)));

            //petView1.SetFontData();
            //petView1.DrawVillain = MidsContext.Character.IsVillain;
            petView1.SetGraphType(Enums.eDDGraph.Simple, Enums.eDDStyle.TextUnderGraph);
            petView1.SetData(_powersData[0], _buffedPower[0]);
        }

        /// <summary>
        /// Set ListLabel font
        /// </summary>
        private void SetPowersFont()
        {
            var loc = _powersList.Location;
            var style = !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold;
            _powersList.Font = new Font(_powersList.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase, style, GraphicsUnit.Point);
            foreach (var it in _powersList.Items)
            {
                it.Bold = MidsContext.Config.RtFont.PowersSelectBold;
            }

            _powersList.Location = new Point(loc.X, loc.Y);
        }

        /// <summary>
        /// Set ListLabel color theme
        /// </summary>
        private void SetPowerColors()
        {
            _powersList.UpdateTextColors(ListLabelV3.LlItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            _powersList.UpdateTextColors(ListLabelV3.LlItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            _powersList.UpdateTextColors(ListLabelV3.LlItemState.Invalid, Color.Red);
            _powersList.ScrollBarColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain;
            _powersList.ScrollButtonColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
            _powersList.UpdateTextColors(ListLabelV3.LlItemState.Selected, MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain);
            _powersList.UpdateTextColors(ListLabelV3.LlItemState.SelectedDisabled, MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
            _powersList.HoverColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
        }

        /// <summary>
        /// Populate ListLabel powers
        /// </summary>
        private void ListPowers()
        {
            _powersList.SuspendRedraw = true;
            _powersList.ClearItems();
            for (var i = 0; i < _powersData.Count; i++)
            {
                if (_powersData[i] == null)
                {
                    var nameChunks = _powers[i].Split('.');
                    _powersList.AddItem(new ListLabelV3.ListLabelItemV3(nameChunks.Length < 1 ? "N/A" : nameChunks[^1], ListLabelV3.LlItemState.Disabled, -1, -1, i, "", ListLabelV3.LlFontFlags.Italic));
                }
                else if (_powersData[i].DisplayName == "Resistance")
                {
                    _powersList.AddItem(new ListLabelV3.ListLabelItemV3(_powersData[i].DisplayName, ListLabelV3.LlItemState.Enabled, -1, _powersData[i].StaticIndex, i));
                }
                else if (new Regex(@"^Self[\s\-_]Destruct", RegexOptions.IgnoreCase).IsMatch(_powersData[i].DisplayName))
                {
                    _powersList.AddItem(new ListLabelV3.ListLabelItemV3(_powersData[i].DisplayName, ListLabelV3.LlItemState.Invalid, -1, _powersData[i].StaticIndex, i, "", ListLabelV3.LlFontFlags.Italic));
                }
                else
                {
                    _powersList.AddItem(new ListLabelV3.ListLabelItemV3(_powersData[i].DisplayName, ListLabelV3.LlItemState.Selected, -1, _powersData[i].StaticIndex, i));
                }
            }

            _powersList.SuspendRedraw = false;
        }

        private void powersList_ItemHover(ListLabelV3.ListLabelItemV3 item)
        {
            if (item.IdxPower < 0)
            {
                return;
            }

            if (item.IdxPower == _hoveredItemIndex)
            {
                return;
            }

            _hoveredItemIndex = item.IdxPower;

            var power = DatabaseAPI.Database.Power
                .DefaultIfEmpty(null)
                .FirstOrDefault(e => e != null && e.StaticIndex == item.IdxPower);

            if (power == null)
            {
                return;
            }

            var petPower = _petInfo.GetPetPower(power);
            _dvPowerBase = petPower?.BasePower;
            _dvPowerEnh = petPower?.BuffedPower;
            

            // dvPowerBase = power.Clone();
            // dvPowerEnh = (item.NIdPower < 0 ? power : BuffedPower[item.NIdPower]).Clone();

            if (DvLocked)
            {
                return;
            }
            
            petView1.SetData(_dvPowerBase, _dvPowerEnh);
        }

        private void powersList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            DvLocked = true;
            petView1.SetData(_dvPowerBase, _dvPowerEnh, false, DvLocked);
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

            var buffedPowers = MainModule.MidsController.Toon?.GenerateBuffedPowers(_powersData, _basePowerHistoryIdx);
            if (buffedPowers == null)
            {
                _mathPower = _powersData.Clone();
                _buffedPower = _powersData.Clone();
            }
            else
            {
                _mathPower = buffedPowers.Value.Key;
                _buffedPower = buffedPowers.Value.Value;
            }

            ListPowers();
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

            _powersList.SuspendRedraw = true;
            SetPowersFont();
            SetPowerColors();
            _powersList.SuspendRedraw = false;

            //petView1.SetFontData();
            //petView1.DrawVillain = charVillain;
        }
    }
}
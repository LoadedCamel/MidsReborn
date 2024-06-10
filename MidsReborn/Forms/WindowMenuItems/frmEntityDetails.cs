using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using static Mids_Reborn.Core.Utils.WinApi;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class FrmEntityDetails : Form
    {
        private string _entityUid;
        private HashSet<string> _powers;
        private SummonedEntity? _entityData;
        private List<IPower?>? _powersData;

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
            Load += OnLoad;
            _entityUid = entityUid;
            _powers = powers;
            _petInfo = petInfo;
            _petInfo.PowersDataUpdated += OnPetInfoPowersDataUpdated;
            InitializeComponent();
            powersCombo1.SelectedIndexChanged += PowersCombo1OnSelectedPowersIndexChanged;
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            if (MidsContext.Config != null)
            {
                MidsContext.Config.EntityDetailsLocation = Location;
            }
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            var owner = (frmMain)Owner;

            Location = MidsContext.Config.EntityDetailsLocation == null
                ? new Point(owner.Location.X + 10, owner.Bottom - Height - 10)
                : (Point)MidsContext.Config.EntityDetailsLocation;

            Text = _entityData != null ? $"Entity Details: {_entityData.DisplayName}" : "Entity Details";

            _entityData = DatabaseAPI.Database.Entities
                .DefaultIfEmpty(null)
                .FirstOrDefault(en => en?.UID == _entityUid);

            _powersData = _powers
                .Select(DatabaseAPI.GetPowerByFullName).Where(p => !p.IsSummonPower)
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

        private void OnPetInfoPowersDataUpdated(object? sender, EventArgs e)
        {
            var petPower = _petInfo.GetPetPower();
            _dvPowerBase = petPower?.BasePower;
            _dvPowerEnh = petPower?.BuffedPower;
            if (_dvPowerEnh != null) petView1.SetData(_dvPowerBase, _dvPowerEnh, false, false, _petInfo.PowerEntryIndex);
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

        private void PowersCombo1OnSelectedPowersIndexChanged(object? sender, EventArgs e)
        {
            if (powersCombo1.Items[powersCombo1.SelectedIndex] is IPower power)
            {
                var petPower = _petInfo.GetPetPower(power);
                _dvPowerBase = petPower?.BasePower;
                _dvPowerEnh = petPower?.BuffedPower;
            }

            if (_dvPowerEnh != null) petView1.SetData(_dvPowerBase, _dvPowerEnh, false, false, _petInfo.PowerEntryIndex);
        }

        public void UpdateData(bool refresh = false)
        {
            Text = _entityData != null ? $"Entity Details: {_entityData.DisplayName}" : "Entity Details";

            if (refresh)
            {
                _petInfo.ExecuteUpdate(out var powers);
                _powers = powers;
                _powersData = _powers
                    .Select(DatabaseAPI.GetPowerByFullName)
                    .ToList();
                powersCombo1.DataSource = null;
                powersCombo1.DataSource = _powersData;
                powersCombo1.ResetBindings();
            }
            else
            {
                _petInfo.ExecuteUpdate();
            }
        }

        /// <summary>
        /// Update and refresh data
        /// </summary>
        /// <param name="entityUid">Entity UID</param>
        /// <param name="powers">Entity's powers, as a List</param>
        public void UpdateData(string entityUid, HashSet<string> powers)
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
        }

        /// <summary>
        /// Update controls color theme from character alignment
        /// </summary>
        /// <param name="cAlignment">Character alignment</param>
        public void UpdateColorTheme(Enums.Alignment cAlignment)
        {
            var charVillain = cAlignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
            petView1.UseAlt = charVillain;
            UpdateStyle();
        }

        private void UpdateStyle()
        {
            bool useVillain;
            if (MidsContext.Character != null && MidsContext.Character.IsHero())
            {
                if (MidsContext.Config != null && MidsContext.Config.DimWindowStyleColors)
                {
                    StylizeWindow(Handle, Color.FromArgb(12, 56, 100), Color.FromArgb(12, 56, 100), Color.WhiteSmoke);
                }
                else
                {
                    StylizeWindow(Handle, Color.DodgerBlue, Color.DodgerBlue, Color.Black);
                }

                useVillain = false;
            }
            else
            {
                if (MidsContext.Config != null && MidsContext.Config.DimWindowStyleColors)
                {
                    StylizeWindow(Handle, Color.FromArgb(100, 0, 0), Color.FromArgb(100, 0, 0), Color.WhiteSmoke);
                }
                else
                {
                    StylizeWindow(Handle, Color.DarkRed, Color.DarkRed, Color.WhiteSmoke);
                }

                useVillain = true;
            }

            powersCombo1.ApplyHeroBorder(useVillain);
        }
    }
}
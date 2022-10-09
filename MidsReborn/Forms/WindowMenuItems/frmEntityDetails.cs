using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;

namespace Forms.WindowMenuItems
{
    public partial class frmEntityDetails : Form
    {
        private string EntityUid;
        private List<string> Powers;
        private int BasePowerHistoryIdx;
        private List<IPower> MathPower;
        private List<IPower> BuffedPower;

        private SummonedEntity? EntityData;
        private List<IPower> PowersData;
        private ListLabelV3 powersList;
        private int HoveredItemIndex = -1;
        private bool dvLocked;
        private IPower? dvPowerBase;
        private IPower? dvPowerEnh;

        /// <summary>
        /// Initialize entity data
        /// </summary>
        /// <param name="entityUid">Entity UID</param>
        /// <param name="powers">Entity's powers, as a HashSet</param>
        public frmEntityDetails(string entityUid, HashSet<string> powers, int basePowerHistoryIdx)
        {
            InitializeComponent();
            EntityUid = entityUid;
            Powers = powers.ToList();
            BasePowerHistoryIdx = basePowerHistoryIdx;
        }

        /// <summary>
        /// Initialize entity data
        /// </summary>
        /// <param name="entityUid">Entity UID</param>
        /// <param name="powers">Entity's powers, as a List</param>
        public frmEntityDetails(string entityUid, List<string> powers, int basePowerHistoryIdx)
        {
            InitializeComponent();
            EntityUid = entityUid;
            Powers = powers;
            BasePowerHistoryIdx = basePowerHistoryIdx;
        }

        
        private void btnClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void btnTopMost_Click(object sender, System.EventArgs e)
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

        private void frmEntityDetails_Load(object sender, System.EventArgs e)
        {
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
            dataView1.TabsMask = new[] {true, true, false, false};
            dataView1.BackColor = BackColor;
            dataView1.Refresh();
            
            Text = EntityData != null ? $"Entity Details: {EntityData.DisplayName}" : "Entity Details";
            
            EntityData = DatabaseAPI.Database.Entities
                .DefaultIfEmpty(null)
                .FirstOrDefault(en => en?.UID == EntityUid);
            
            PowersData = Powers
                .Select(DatabaseAPI.GetPowerByFullName)
                .ToList();

            var buffedPowers = MainModule.MidsController.Toon.GenerateBuffedPowers(PowersData, BasePowerHistoryIdx);
            MathPower = buffedPowers.Key;
            BuffedPower = buffedPowers.Value;

            lblEntityName.Text = EntityData == null
                ? "Entity Details"
                : $"Entity: {EntityData.DisplayName}";

            powersList = new ListLabelV3
            {
                Location = dataView1.Location with {X = dataView1.Location.X + dataView1.Size.Width + 10},
                Size = new Size(180, dataView1.Size.Height - 70),
                SizeNormal = new Size(180, dataView1.Size.Height - 70),
                BackColor = Color.Black,
                Expandable = false,
                ActualLineHeight = 18,
                HoverColor = Color.DimGray,
                Scrollable = false,
            };

            powersList.ItemHover += powersList_ItemHover;
            powersList.MouseDown += powersList_MouseDown;
            Controls.Add(powersList);

            powersList.SuspendRedraw = true;
            SetPowersFont();
            SetPowerColors();
            powersList.SuspendRedraw = false;

            ListPowers();
            powersList.Size = new Size(180, Math.Min(dataView1.Size.Height - 70, Math.Max(120, powersList.DesiredHeight)));
            powersList.SizeNormal = new Size(180, Math.Min(dataView1.Size.Height - 70, Math.Max(120, powersList.DesiredHeight)));

            dataView1.SetFontData();
            dataView1.DrawVillain = MidsContext.Character.IsVillain;
            dataView1.SetGraphType(Enums.eDDGraph.Simple, Enums.eDDStyle.TextUnderGraph);
            dataView1.SetData(PowersData[0], BuffedPower[0]);
        }

        /// <summary>
        /// Set ListLabel font
        /// </summary>
        private void SetPowersFont()
        {
            var loc = powersList.Location;
            var style = !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold;
            powersList.Font = new Font(powersList.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase, style, GraphicsUnit.Point);
            foreach (var it in powersList.Items)
            {
                it.Bold = MidsContext.Config.RtFont.PowersSelectBold;
            }

            powersList.Location = new Point(loc.X, loc.Y);
        }

        /// <summary>
        /// Set ListLabel color theme
        /// </summary>
        private void SetPowerColors()
        {
            powersList.UpdateTextColors(ListLabelV3.LlItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            powersList.UpdateTextColors(ListLabelV3.LlItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            powersList.UpdateTextColors(ListLabelV3.LlItemState.Invalid, Color.Red);
            powersList.ScrollBarColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain;
            powersList.ScrollButtonColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
            powersList.UpdateTextColors(ListLabelV3.LlItemState.Selected, MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain);
            powersList.UpdateTextColors(ListLabelV3.LlItemState.SelectedDisabled, MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
            powersList.HoverColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
        }

        /// <summary>
        /// Populate ListLabel powers
        /// </summary>
        private void ListPowers()
        {
            powersList.SuspendRedraw = true;
            powersList.ClearItems();
            for (var i = 0; i < PowersData.Count; i++)
            {
                if (PowersData[i] == null)
                {
                    var nameChunks = Powers[i].Split('.');
                    powersList.AddItem(new ListLabelV3.ListLabelItemV3(nameChunks.Length < 1 ? "N/A" : nameChunks[^1], ListLabelV3.LlItemState.Disabled, -1, -1, i, "", ListLabelV3.LlFontFlags.Italic));
                }
                else if (PowersData[i].DisplayName == "Resistance")
                {
                    powersList.AddItem(new ListLabelV3.ListLabelItemV3(PowersData[i].DisplayName, ListLabelV3.LlItemState.Enabled, -1, PowersData[i].StaticIndex, i));
                }
                else if (new Regex(@"^Self[\s\-_]Destruct", RegexOptions.IgnoreCase).IsMatch(PowersData[i].DisplayName))
                {
                    powersList.AddItem(new ListLabelV3.ListLabelItemV3(PowersData[i].DisplayName, ListLabelV3.LlItemState.Invalid, -1, PowersData[i].StaticIndex, i, "", ListLabelV3.LlFontFlags.Italic));
                }
                else
                {
                    powersList.AddItem(new ListLabelV3.ListLabelItemV3(PowersData[i].DisplayName, ListLabelV3.LlItemState.Selected, -1, PowersData[i].StaticIndex, i));
                }
            }

            powersList.SuspendRedraw = false;
        }

        private void powersList_ItemHover(ListLabelV3.ListLabelItemV3 item)
        {
            if (item.IdxPower < 0)
            {
                return;
            }

            if (item.IdxPower == HoveredItemIndex)
            {
                return;
            }

            HoveredItemIndex = item.IdxPower;

            var power = DatabaseAPI.Database.Power
                .DefaultIfEmpty(null)
                .FirstOrDefault(e => e != null && e.StaticIndex == item.IdxPower);

            if (power == null)
            {
                return;
            }

            dvPowerBase = power.Clone();
            dvPowerEnh = (item.NIdPower < 0 ? power : BuffedPower[item.NIdPower]).Clone();
            if (dvLocked)
            {
                return;
            }

            dataView1.SetData(dvPowerBase, dvPowerEnh);
        }

        private void powersList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            dvLocked = true;
            dataView1.SetData(dvPowerBase, dvPowerEnh, false, true);
        }

        private void dataView1_Unlock_Click()
        {
            dvLocked = false;
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
            EntityUid = entityUid;
            Powers = powers;

            EntityData = DatabaseAPI.Database.Entities
                .DefaultIfEmpty(null)
                .FirstOrDefault(en => en?.UID == EntityUid);

            PowersData = Powers
                .Select(DatabaseAPI.GetPowerByFullName)
                .ToList();

            Text = EntityData != null ? $"Entity Details: {EntityData.DisplayName}" : "Entity Details";

            var buffedPowers = MainModule.MidsController.Toon.GenerateBuffedPowers(PowersData, BasePowerHistoryIdx);
            MathPower = buffedPowers.Key;
            BuffedPower = buffedPowers.Value;

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

            powersList.SuspendRedraw = true;
            SetPowersFont();
            SetPowerColors();
            powersList.SuspendRedraw = false;

            dataView1.SetFontData();
            dataView1.DrawVillain = charVillain;
        }
    }
}
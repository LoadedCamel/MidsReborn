using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Base.Display;
using Base.Master_Classes;
using midsControls;
using Syncfusion.Windows.Forms.Tools;

namespace Hero_Designer.Forms.WindowMenuItems
{
    public partial class frmTotalsV2 : Form
    {
        private readonly frmMain _myParent;
        private bool _keepOnTop;

        public Control StatControl(string tab, int panel, string type, int control)
        {
            Regex regEx = new Regex(@"^\d+");
            TabPageAdv page = tabControlAdv1.Controls.OfType<TabPageAdv>().First(t => t.Text.Contains(tab));
            List<GradientPanel> gradientList = page.Controls.OfType<GradientPanel>().ToList();
            List<GradientPanel> gradientPanels = gradientList.OrderBy(x => x.Name).ToList();
            List<TableLayoutPanel> tablePanels = gradientPanels[panel - 1].Controls.OfType<TableLayoutPanel>().ToList();

            switch (type)
            {
                case "Bar":
                    List<Control> controls = new List<Control>();
                    for (int rowIndex = 0; rowIndex < tablePanels[0].RowCount; rowIndex++)
                    {
                        Control tControl = tablePanels[0].GetControlFromPosition(2, rowIndex);
                        controls.Add(tControl);
                    }
                    List<ctlLayeredBar> barList = controls.OfType<ctlLayeredBar>().ToList();
                    List<ctlLayeredBar> bars = barList.OrderBy(x => regEx.Match(x.Name).Value).ToList();

                    return bars[control - 1];
                case "Label":
                    controls = new List<Control>();
                    for (int rowIndex = 0; rowIndex < tablePanels[0].RowCount; rowIndex++)
                    {
                        Control tControl = tablePanels[0].GetControlFromPosition(1, rowIndex);
                        controls.Add(tControl);
                    }
                    List<Label> labelList = controls.OfType<Label>().ToList();
                    List<Label> labels = labelList.OrderBy(x => regEx.Match(x.Name).Value).ToList();

                    return labels[control - 1];
            }

            return null;
        }

        private string FormatValue(int formatType, float value)
        {
            return formatType switch
            {
                0 => $"{value:##0.##}%", // Percentage
                1 => $"{value:##0.##}", // Numeric, 2 decimals
                2 => (value > 0 ? "+" : "") + $"{value:##0.##}", // Numeric, 2 decimals, with sign
                _ => $"{value:##0.##}"
            };
        }

        private Label FetchLabel(Enums.eBarType barType)
        {
            return FetchLabel((int)barType);
        }

        private Label FetchLabel(int index)
        {
            return index switch
            {
                0 => label15,
                1 => label16,
                2 => label17,
                3 => label18,
                4 => label19,
                5 => label20,
                6 => label21,
                7 => label22,
                8 => label23,
                9 => label24,

                10 => label33,
                11 => label34,
                12 => label35,
                13 => label36,
                14 => label37,
                15 => label38,
                16 => label39,
                17 => label40,

                18 => label43,
                19 => label44,

                20 => label48,
                21 => label49,
                22 => label50,

                23 => label70,
                24 => label59,
                25 => label58,
                26 => label57,

                27 => label66,
                28 => label54,
                29 => label53,

                30 => label74,
                31 => label67,
                32 => label71,
                33 => label73,
                34 => label76,
                35 => label78,
                36 => label80,

                37 => label89,
                38 => label84,
                39 => label83,
                40 => label82,
                41 => label97,
                42 => label98,
                43 => label99,
                44 => label100,
                45 => label101,
                46 => label102,
                47 => label103,

                48 => label126,
                49 => label121,
                50 => label120,
                51 => label119,
                52 => label127,
                53 => label111,
                54 => label110,
                55 => label109,
                56 => label108,
                57 => label107,
                58 => label106,

                59 => label149,
                60 => label144,
                61 => label143,
                62 => label142,
                63 => label150,
                64 => label134,
                65 => label133,
                _ => label15
            };
        }

        private ctlLayeredBar FetchBar(int index)
        {
            return index switch
            {
                // Defense
                0 => ctlLayeredBar3,
                1 => ctlLayeredBar4,
                2 => ctlLayeredBar5,
                3 => ctlLayeredBar6,
                4 => ctlLayeredBar7,
                5 => ctlLayeredBar8,
                6 => ctlLayeredBar9,
                7 => ctlLayeredBar10,
                8 => ctlLayeredBar11,
                9 => ctlLayeredBar12,

                // Resistance
                10 => ctlLayeredBar13,
                11 => ctlLayeredBar19,
                12 => ctlLayeredBar20,
                13 => ctlLayeredBar21,
                14 => ctlLayeredBar22,
                15 => ctlLayeredBar23,
                16 => ctlLayeredBar24,
                17 => ctlLayeredBar25,

                // Regen/HP/Absorb
                18 => ctlLayeredBar14,
                19 => ctlLayeredBar15,

                // Endurance
                20 => ctlLayeredBar16,
                21 => ctlLayeredBar17,
                22 => ctlLayeredBar18,
                
                // Movement
                23 => ctlLayeredBar51,
                24 => ctlLayeredBar50,
                25 => ctlLayeredBar49,
                26 => ctlLayeredBar48,
                
                // Stealth/Perception
                27 => ctlLayeredBar63,
                28 => ctlLayeredBar60,
                29 => ctlLayeredBar57,
                
                // Misc
                30 => ctlLayeredBar72,
                31 => ctlLayeredBar56,
                32 => ctlLayeredBar68,
                33 => ctlLayeredBar71,
                34 => ctlLayeredBar78,
                35 => ctlLayeredBar81,
                36 => ctlLayeredBar85,
                
                // Status Protection
                37 => ctlLayeredBar97,
                38 => ctlLayeredBar94,
                39 => ctlLayeredBar91,
                40 => ctlLayeredBar88,
                41 => ctlLayeredBar89,
                42 => ctlLayeredBar90,
                43 => ctlLayeredBar92,
                44 => ctlLayeredBar93,
                45 => ctlLayeredBar95,
                46 => ctlLayeredBar96,
                47 => ctlLayeredBar98,
                
                // Status Resistance
                48 => ctlLayeredBar102,
                49 => ctlLayeredBar101,
                50 => ctlLayeredBar100,
                51 => ctlLayeredBar99,
                52 => ctlLayeredBar103,
                53 => ctlLayeredBar104,
                54 => ctlLayeredBar105,
                55 => ctlLayeredBar106,
                56 => ctlLayeredBar107,
                57 => ctlLayeredBar108,
                58 => ctlLayeredBar109,
                
                // Debuff Resistance
                59 => ctlLayeredBar113,
                60 => ctlLayeredBar112,
                61 => ctlLayeredBar111,
                62 => ctlLayeredBar110,
                63 => ctlLayeredBar114,
                64 => ctlLayeredBar115,
                65 => ctlLayeredBar116,
                _ => ctlLayeredBar3
            };
        }

        private ctlLayeredBar FetchBar(Enums.eBarType barType)
        {
            return FetchBar((int)barType);
        }

        private void SetTabPanelColorScheme()
        {
            if (MidsContext.Character.IsHero())
            {
                tabControlAdv1.InactiveTabColor = Color.DodgerBlue;
                tabControlAdv1.TabPanelBackColor = Color.DodgerBlue;
                tabControlAdv1.FixedSingleBorderColor = Color.Goldenrod;
                tabControlAdv1.ActiveTabColor = Color.Goldenrod;
            }
            else
            {
                tabControlAdv1.InactiveTabColor = Color.FromArgb(193, 23, 23);
                tabControlAdv1.TabPanelBackColor = Color.FromArgb(193, 23, 23);
                tabControlAdv1.FixedSingleBorderColor = Color.FromArgb(198, 128, 29);
                tabControlAdv1.ActiveTabColor = Color.FromArgb(198, 128, 29);
            }
        }

        public frmTotalsV2(ref frmMain iParent)
        {
            Load += OnLoad;
            _keepOnTop = true;
            InitializeComponent();
            _myParent = iParent;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            CenterToParent();

            if (!MainModule.MidsController.IsAppInitialized) return;
            switch (MidsContext.Config.SpeedFormat)
            {
                case Enums.eSpeedMeasure.FeetPerSecond:
                    radioButton3.Checked = true;
                    break;
                case Enums.eSpeedMeasure.MetersPerSecond:
                    radioButton4.Checked = true;
                    break;
                case Enums.eSpeedMeasure.KilometersPerHour:
                    radioButton2.Checked = true;
                    break;
                case Enums.eSpeedMeasure.MilesPerHour:
                    radioButton1.Checked = true;
                    break;
                default:
                    radioButton1.Checked = true;
                    break;
            }

            // Tab Foreground Colors don't stick in the designer.
            // Note: default colors will be used anyway.
            // This may only cause issues if a custom
            // Windows theme is in use.
            tabControlAdv1.ActiveTabForeColor = Color.White;
            tabControlAdv1.InActiveTabForeColor = Color.Black;
            SetTabPanelColorScheme();
        }

        private void PbCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void PbClosePaint(object sender, PaintEventArgs e)
        {
            if (_myParent?.Drawing == null) return;

            var iStr = "Close";
            var rectangle = new Rectangle();
            ref var local = ref rectangle;
            var size = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Size
                : _myParent.Drawing.bxPower[4].Size;
            var width = size.Width;
            size = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Size
                : _myParent.Drawing.bxPower[4].Size;
            var height1 = size.Height;
            local = new Rectangle(0, 0, width, height1);
            var destRect = new Rectangle(0, 0, 105, 22);
            using var stringFormat = new StringFormat();
            using var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            extendedBitmap.Graphics.DrawImage(
                MidsContext.Character.IsHero()
                    ? _myParent.Drawing.bxPower[2].Bitmap
                    : _myParent.Drawing.bxPower[4].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                GraphicsUnit.Pixel, _myParent.Drawing.pImageAttributes);
            var height2 = bFont.GetHeight(e.Graphics) + 2;
            var Bounds = new RectangleF(0, (22 - height2) / 2, 105, height2);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        private void PbTopMostClick(object sender, EventArgs e)
        {
            _keepOnTop = !_keepOnTop;
            TopMost = _keepOnTop;
            pbTopMost.Refresh();
        }

        private void PbTopMostPaint(object sender, PaintEventArgs e)
        {
            if (_myParent?.Drawing == null) return;

            var index = 2;
            if (_keepOnTop) index = 3;
            var iStr = "Keep On top";
            var rectangle = new Rectangle(0, 0, _myParent.Drawing.bxPower[index].Size.Width,
                _myParent.Drawing.bxPower[index].Size.Height);
            var destRect = new Rectangle(0, 0, 105, 22);
            using var stringFormat = new StringFormat();
            using var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            if (index == 3)
                extendedBitmap.Graphics.DrawImage(_myParent.Drawing.bxPower[index].Bitmap, destRect, 0, 0,
                    rectangle.Width, rectangle.Height, GraphicsUnit.Pixel);
            else
                extendedBitmap.Graphics.DrawImage(_myParent.Drawing.bxPower[index].Bitmap, destRect, 0, 0,
                    rectangle.Width, rectangle.Height, GraphicsUnit.Pixel, _myParent.Drawing.pImageAttributes);
            var height = bFont.GetHeight(e.Graphics) + 2;
            var Bounds = new RectangleF(0, (22 - height) / 2, 105, height);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        #region frmTotals import
        /*
        private void RbSpeedCheckedChanged(object sender, EventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            if (rbMPH.Checked)
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.MilesPerHour;
            else if (rbKPH.Checked)
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.KilometersPerHour;
            else if (rbFPS.Checked)
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.FeetPerSecond;
            else if (rbMSec.Checked)
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.MetersPerSecond;
            UpdateData();
        }
        */

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized) return;

            MainModule.MidsController.SzFrmTotals.X = Left;
            MainModule.MidsController.SzFrmTotals.Y = Top;
            MainModule.MidsController.SzFrmTotals.Width = Width;
            MainModule.MidsController.SzFrmTotals.Height = Height;
        }
        #endregion

        public void UpdateData()
        {
            string[] damageNames = Enum.GetNames(Enums.eDamage.None.GetType());
            pbClose.Refresh();
            pbTopMost.Refresh();
            Statistics displayStats = MidsContext.Character.DisplayStats;

            #region Bars setup
            FetchBar(Enums.eBarType.DefenseSmashing).ValueMainBar = displayStats.Defense(0);
            FetchBar(Enums.eBarType.DefenseLethal).ValueMainBar = displayStats.Defense(1);
            FetchBar(Enums.eBarType.DefenseFire).ValueMainBar = displayStats.Defense(2);
            FetchBar(Enums.eBarType.DefenseCold).ValueMainBar = displayStats.Defense(3);
            FetchBar(Enums.eBarType.DefenseEnergy).ValueMainBar = displayStats.Defense(4);
            FetchBar(Enums.eBarType.DefenseNegative).ValueMainBar = displayStats.Defense(5);
            FetchBar(Enums.eBarType.DefensePsionic).ValueMainBar = displayStats.Defense(6);
            FetchBar(Enums.eBarType.DefenseMelee).ValueMainBar = displayStats.Defense(7);
            FetchBar(Enums.eBarType.DefenseRanged).ValueMainBar = displayStats.Defense(8);
            FetchBar(Enums.eBarType.DefenseAoE).ValueMainBar = displayStats.Defense(9);

            FetchBar(Enums.eBarType.ResistanceSmashing).SuspendUpdate();
            FetchBar(Enums.eBarType.ResistanceSmashing).ValueMainBar = displayStats.DamageResistance(0, false);
            FetchBar(Enums.eBarType.ResistanceSmashing).ValueOverCap = displayStats.DamageResistance(0, true);
            FetchBar(Enums.eBarType.ResistanceSmashing).ResumeUpdate();
            FetchBar(Enums.eBarType.ResistanceLethal).SuspendUpdate();
            FetchBar(Enums.eBarType.ResistanceLethal).ValueMainBar = displayStats.DamageResistance(1, false);
            FetchBar(Enums.eBarType.ResistanceLethal).ValueOverCap = displayStats.DamageResistance(1, true);
            FetchBar(Enums.eBarType.ResistanceLethal).ResumeUpdate();
            FetchBar(Enums.eBarType.ResistanceFire).SuspendUpdate();
            FetchBar(Enums.eBarType.ResistanceFire).ValueMainBar = displayStats.DamageResistance(2, false);
            FetchBar(Enums.eBarType.ResistanceFire).ValueOverCap = displayStats.DamageResistance(2, true);
            FetchBar(Enums.eBarType.ResistanceFire).ResumeUpdate();
            FetchBar(Enums.eBarType.ResistanceCold).SuspendUpdate();
            FetchBar(Enums.eBarType.ResistanceCold).ValueMainBar = displayStats.DamageResistance(3, false);
            FetchBar(Enums.eBarType.ResistanceCold).ValueOverCap = displayStats.DamageResistance(3, true);
            FetchBar(Enums.eBarType.ResistanceCold).ResumeUpdate();
            FetchBar(Enums.eBarType.ResistanceEnergy).SuspendUpdate();
            FetchBar(Enums.eBarType.ResistanceEnergy).ValueMainBar = displayStats.DamageResistance(4, false);
            FetchBar(Enums.eBarType.ResistanceEnergy).ValueOverCap = displayStats.DamageResistance(4, true);
            FetchBar(Enums.eBarType.ResistanceEnergy).ResumeUpdate();
            FetchBar(Enums.eBarType.ResistanceNegative).SuspendUpdate();
            FetchBar(Enums.eBarType.ResistanceNegative).ValueMainBar = displayStats.DamageResistance(5, false);
            FetchBar(Enums.eBarType.ResistanceNegative).ValueOverCap = displayStats.DamageResistance(5, true);
            FetchBar(Enums.eBarType.ResistanceNegative).ResumeUpdate();
            FetchBar(Enums.eBarType.ResistanceToxic).SuspendUpdate();
            FetchBar(Enums.eBarType.ResistanceToxic).ValueMainBar = displayStats.DamageResistance(6, false);
            FetchBar(Enums.eBarType.ResistanceToxic).ValueOverCap = displayStats.DamageResistance(6, true);
            FetchBar(Enums.eBarType.ResistanceToxic).ResumeUpdate();
            FetchBar(Enums.eBarType.ResistancePsionic).SuspendUpdate();
            FetchBar(Enums.eBarType.ResistancePsionic).ValueMainBar = displayStats.DamageResistance(7, false);
            FetchBar(Enums.eBarType.ResistancePsionic).ValueOverCap = displayStats.DamageResistance(7, true);
            FetchBar(Enums.eBarType.ResistancePsionic).ResumeUpdate();

            FetchBar(Enums.eBarType.Regeneration).SuspendUpdate();
            FetchBar(Enums.eBarType.Regeneration).ValueBase = MidsContext.Character.Archetype.BaseRegen;
            FetchBar(Enums.eBarType.Regeneration).ValueMainBar = displayStats.HealthRegenPercent(false);
            FetchBar(Enums.eBarType.Regeneration).ValueOverCap = displayStats.HealthRegenPercent(true);
            FetchBar(Enums.eBarType.Regeneration).ResumeUpdate();

            FetchBar(Enums.eBarType.MaxHPAbsorb).SuspendUpdate();
            FetchBar(Enums.eBarType.MaxHPAbsorb).ValueBase = MidsContext.Character.Archetype.Hitpoints;
            FetchBar(Enums.eBarType.MaxHPAbsorb).ValueMainBar = displayStats.HealthHitpointsNumeric(false);
            FetchBar(Enums.eBarType.MaxHPAbsorb).ValueOverCap = displayStats.HealthHitpointsNumeric(true);
            FetchBar(Enums.eBarType.MaxHPAbsorb).ValueOverlay1 = Math.Min(displayStats.Absorb, MidsContext.Character.Archetype.Hitpoints);
            FetchBar(Enums.eBarType.MaxHPAbsorb).ResumeUpdate();

            FetchBar(Enums.eBarType.EndRec).SuspendUpdate();
            FetchBar(Enums.eBarType.EndRec).ValueBase = MidsContext.Character.Archetype.BaseRecovery;
            FetchBar(Enums.eBarType.EndRec).ValueMainBar = displayStats.EnduranceRecoveryNumeric;
            FetchBar(Enums.eBarType.EndRec).ValueOverCap = MidsContext.Character.Archetype.RecoveryCap;
            FetchBar(Enums.eBarType.EndRec).ResumeUpdate();

            FetchBar(Enums.eBarType.EndUse).ValueMainBar = displayStats.EnduranceUsage;

            FetchBar(Enums.eBarType.MaxEnd).SuspendUpdate();
            FetchBar(Enums.eBarType.MaxEnd).ValueBase = 100;
            FetchBar(Enums.eBarType.MaxEnd).ValueMainBar = displayStats.EnduranceMaxEnd;
            FetchBar(Enums.eBarType.MaxEnd).ResumeUpdate();

            ///////////////////////////////

            FetchBar(Enums.eBarType.RunSpeed).SuspendUpdate();
            FetchBar(Enums.eBarType.RunSpeed).ValueBase = 21f;
            FetchBar(Enums.eBarType.RunSpeed).ValueMainBar = displayStats.MovementRunSpeed(Enums.eSpeedMeasure.MilesPerHour, false);
            FetchBar(Enums.eBarType.RunSpeed).ValueOverCap = displayStats.MovementRunSpeed(Enums.eSpeedMeasure.MilesPerHour, true);
            FetchBar(Enums.eBarType.RunSpeed).ResumeUpdate();
            FetchBar(Enums.eBarType.JumpSpeed).SuspendUpdate();
            //FetchBar(Enums.eBarType.JumpSpeed).ValueBase = MidsContext.Character.Archetype.BaseJumpSpeed;
            FetchBar(Enums.eBarType.JumpSpeed).ValueMainBar = displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.MilesPerHour, false);
            FetchBar(Enums.eBarType.JumpSpeed).ValueOverCap = displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.MilesPerHour, true);
            FetchBar(Enums.eBarType.JumpSpeed).ResumeUpdate();
            FetchBar(Enums.eBarType.JumpHeight).SuspendUpdate();
            FetchBar(Enums.eBarType.JumpHeight).ValueBase = 4f;
            FetchBar(Enums.eBarType.JumpHeight).ValueMainBar = displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond);
            //FetchBar(Enums.eBarType.JumpHeight).ValueOverCap = displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond, true);
            FetchBar(Enums.eBarType.JumpHeight).ResumeUpdate();
            FetchBar(Enums.eBarType.FlySpeed).SuspendUpdate();
            FetchBar(Enums.eBarType.FlySpeed).ValueBase = 31.5f;
            FetchBar(Enums.eBarType.FlySpeed).ValueMainBar = displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false);
            FetchBar(Enums.eBarType.FlySpeed).ValueOverCap = displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, true);
            FetchBar(Enums.eBarType.FlySpeed).ResumeUpdate();

            ///////////////////////////////

            FetchBar(Enums.eBarType.StealthPvE).SuspendUpdate();
            FetchBar(Enums.eBarType.StealthPvE).ValueBase = 0;
            FetchBar(Enums.eBarType.StealthPvE).ValueMainBar = MidsContext.Character.Totals.StealthPvE; // ???
            FetchBar(Enums.eBarType.StealthPvE).ValueOverCap = 0; //displayStats.Distance(0, Enums.eSpeedMeasure.FeetPerSecond, true); // ???
            FetchBar(Enums.eBarType.StealthPvE).ResumeUpdate();
            FetchBar(Enums.eBarType.StealthPvP).SuspendUpdate();
            FetchBar(Enums.eBarType.StealthPvP).ValueBase = 0;
            FetchBar(Enums.eBarType.StealthPvP).ValueMainBar = MidsContext.Character.Totals.StealthPvP; // ???
            FetchBar(Enums.eBarType.StealthPvP).ValueOverCap = 0; //displayStats.Distance(0, Enums.eSpeedMeasure.FeetPerSecond, true); // ???
            FetchBar(Enums.eBarType.StealthPvP).ResumeUpdate();
            FetchBar(Enums.eBarType.Perception).SuspendUpdate();
            //FetchBar(Enums.eBarType.Perception).ValueBase = MidsContext.Character.Archetype.BasePerception;
            FetchBar(Enums.eBarType.Perception).ValueMainBar = displayStats.Perception(false);
            FetchBar(Enums.eBarType.Perception).ValueOverCap = displayStats.Perception(true);
            FetchBar(Enums.eBarType.Perception).ResumeUpdate();

            ///////////////////////////////

            FetchBar(Enums.eBarType.Haste).SuspendUpdate();
            FetchBar(Enums.eBarType.Haste).ValueBase = 100;
            FetchBar(Enums.eBarType.Haste).ValueMainBar = displayStats.BuffHaste(false);
            FetchBar(Enums.eBarType.Haste).ValueOverCap = displayStats.BuffHaste(true);
            FetchBar(Enums.eBarType.Haste).ResumeUpdate();
            FetchBar(Enums.eBarType.ToHit).ValueMainBar = displayStats.BuffToHit;
            FetchBar(Enums.eBarType.Accuracy).ValueMainBar = displayStats.BuffAccuracy;
            FetchBar(Enums.eBarType.Damage).SuspendUpdate();
            FetchBar(Enums.eBarType.Damage).ValueBase = 100;
            FetchBar(Enums.eBarType.Damage).ValueMainBar = displayStats.BuffDamage(false); // Need to add +100 here ?
            FetchBar(Enums.eBarType.Damage).ValueOverCap = displayStats.BuffDamage(true); // Need to add +100 here ?
            FetchBar(Enums.eBarType.Damage).ResumeUpdate();
            FetchBar(Enums.eBarType.EndRdx).ValueMainBar = displayStats.BuffEndRdx;
            FetchBar(Enums.eBarType.ThreatLevel).SuspendUpdate();
            FetchBar(Enums.eBarType.ThreatLevel).ValueBase = MidsContext.Character.Archetype.BaseThreat;
            FetchBar(Enums.eBarType.ThreatLevel).ValueMainBar = displayStats.ThreatLevel;
            FetchBar(Enums.eBarType.ThreatLevel).ResumeUpdate();
            FetchBar(Enums.eBarType.Elusivity).ValueMainBar = MidsContext.Character.Totals.Elusivity;

            ///////////////////////////////

            FetchBar(Enums.eBarType.MezProtectionHold).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Held];
            FetchBar(Enums.eBarType.MezProtectionStunned).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Stunned];
            FetchBar(Enums.eBarType.MezProtectionSleep).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Sleep];
            FetchBar(Enums.eBarType.MezProtectionImmob).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Immobilized];
            FetchBar(Enums.eBarType.MezProtectionKnockback).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Knockback];
            FetchBar(Enums.eBarType.MezProtectionRepel).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Repel];
            FetchBar(Enums.eBarType.MezProtectionConfuse).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Confused];
            FetchBar(Enums.eBarType.MezProtectionFear).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Terrorized];
            FetchBar(Enums.eBarType.MezProtectionTaunt).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Taunt];
            FetchBar(Enums.eBarType.MezProtectionPlacate).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Placate];
            FetchBar(Enums.eBarType.MezProtectionTeleport).ValueMainBar = MidsContext.Character.Totals.Mez[(int)Enums.eMez.Teleport];

            FetchBar(Enums.eBarType.MezResistanceHold).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Held];
            FetchBar(Enums.eBarType.MezResistanceStunned).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Stunned];
            FetchBar(Enums.eBarType.MezResistanceSleep).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Sleep];
            FetchBar(Enums.eBarType.MezResistanceImmob).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Immobilized];
            FetchBar(Enums.eBarType.MezResistanceKnockback).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Knockback];
            FetchBar(Enums.eBarType.MezResistanceRepel).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Repel];
            FetchBar(Enums.eBarType.MezResistanceConfuse).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Confused];
            FetchBar(Enums.eBarType.MezResistanceFear).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Terrorized];
            FetchBar(Enums.eBarType.MezResistanceTaunt).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Taunt];
            FetchBar(Enums.eBarType.MezResistancePlacate).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Placate];
            FetchBar(Enums.eBarType.MezResistanceTeleport).ValueMainBar = MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Teleport];

            ///////////////////////////////

            FetchBar(Enums.eBarType.DebuffResistanceDefense).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Defense];
            FetchBar(Enums.eBarType.DebuffResistanceEndurance).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Endurance];
            FetchBar(Enums.eBarType.DebuffResistanceRecovery).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Recovery];
            FetchBar(Enums.eBarType.DebuffResistancePerception).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.PerceptionRadius];
            FetchBar(Enums.eBarType.DebuffResistanceToHit).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.ToHit];
            FetchBar(Enums.eBarType.DebuffResistanceRechargeTime).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.RechargeTime];
            FetchBar(Enums.eBarType.DebuffResistanceSpeedRunning).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.SpeedRunning];
            FetchBar(Enums.eBarType.DebuffResistanceRegen).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Regeneration];
            #endregion

            #region Labels setup
            FetchLabel(Enums.eBarType.DefenseSmashing).Text = FormatValue(0, displayStats.Defense(0));
            FetchLabel(Enums.eBarType.DefenseLethal).Text = FormatValue(0, displayStats.Defense(1));
            FetchLabel(Enums.eBarType.DefenseFire).Text = FormatValue(0, displayStats.Defense(2));
            FetchLabel(Enums.eBarType.DefenseCold).Text = FormatValue(0, displayStats.Defense(3));
            FetchLabel(Enums.eBarType.DefenseEnergy).Text = FormatValue(0, displayStats.Defense(4));
            FetchLabel(Enums.eBarType.DefenseNegative).Text = FormatValue(0, displayStats.Defense(5));
            FetchLabel(Enums.eBarType.DefensePsionic).Text = FormatValue(0, displayStats.Defense(6));
            FetchLabel(Enums.eBarType.DefenseMelee).Text = FormatValue(0, displayStats.Defense(7));
            FetchLabel(Enums.eBarType.DefenseRanged).Text = FormatValue(0, displayStats.Defense(8));
            FetchLabel(Enums.eBarType.DefenseAoE).Text = FormatValue(0, displayStats.Defense(9));

            FetchLabel(Enums.eBarType.ResistanceSmashing).Text = FormatValue(0, displayStats.DamageResistance(0, false));
            FetchLabel(Enums.eBarType.ResistanceLethal).Text = FormatValue(0, displayStats.DamageResistance(1, false));
            FetchLabel(Enums.eBarType.ResistanceFire).Text = FormatValue(0, displayStats.DamageResistance(2, false));
            FetchLabel(Enums.eBarType.ResistanceCold).Text = FormatValue(0, displayStats.DamageResistance(3, false));
            FetchLabel(Enums.eBarType.ResistanceEnergy).Text = FormatValue(0, displayStats.DamageResistance(4, false));
            FetchLabel(Enums.eBarType.ResistanceNegative).Text = FormatValue(0, displayStats.DamageResistance(5, false));
            FetchLabel(Enums.eBarType.ResistanceToxic).Text = FormatValue(0, displayStats.DamageResistance(6, false));
            FetchLabel(Enums.eBarType.ResistancePsionic).Text = FormatValue(0, displayStats.DamageResistance(7, false));

            FetchLabel(Enums.eBarType.Regeneration).Text = FormatValue(0, displayStats.HealthRegenPercent(false));
            FetchLabel(Enums.eBarType.MaxHPAbsorb).Text = FormatValue(0, displayStats.HealthHitpointsNumeric(false));

            FetchLabel(Enums.eBarType.EndRec).Text = FormatValue(1, displayStats.EnduranceRecoveryNumeric) + "/s";
            FetchLabel(Enums.eBarType.EndUse).Text = FormatValue(1, displayStats.EnduranceUsage) + "/s";
            FetchLabel(Enums.eBarType.MaxEnd).Text = FormatValue(0, displayStats.EnduranceMaxEnd);

            ///////////////////////////////

            FetchLabel(Enums.eBarType.RunSpeed).Text = FormatValue(1, displayStats.MovementRunSpeed(Enums.eSpeedMeasure.MilesPerHour, false)) + "mph";
            FetchLabel(Enums.eBarType.JumpSpeed).Text = FormatValue(1, displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.MilesPerHour, false)) + "mph";
            FetchLabel(Enums.eBarType.JumpHeight).Text = FormatValue(1, displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond)) + "ft";
            FetchLabel(Enums.eBarType.FlySpeed).Text = FormatValue(1, displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false)) + "mph";

            ///////////////////////////////

            FetchLabel(Enums.eBarType.StealthPvE).Text = FormatValue(1, MidsContext.Character.Totals.StealthPvE) + "ft"; // ???
            FetchLabel(Enums.eBarType.StealthPvP).Text = FormatValue(1, MidsContext.Character.Totals.StealthPvP) + "ft"; // ???
            FetchLabel(Enums.eBarType.Perception).Text = FormatValue(1, displayStats.Perception(false)) + "ft";

            ///////////////////////////////

            FetchLabel(Enums.eBarType.Haste).Text = FormatValue(0, displayStats.BuffHaste(false));
            FetchLabel(Enums.eBarType.ToHit).Text = FormatValue(0, displayStats.BuffToHit);
            FetchLabel(Enums.eBarType.Accuracy).Text = FormatValue(0, displayStats.BuffAccuracy);
            FetchLabel(Enums.eBarType.Damage).Text = FormatValue(0, displayStats.BuffDamage(false)); // Need to add +100 here ?
            FetchLabel(Enums.eBarType.EndRdx).Text = FormatValue(0, displayStats.BuffEndRdx);
            FetchLabel(Enums.eBarType.ThreatLevel).Text = Convert.ToString(displayStats.ThreatLevel, CultureInfo.InvariantCulture); // ???
            FetchLabel(Enums.eBarType.Elusivity).Text = FormatValue(0, MidsContext.Character.Totals.Elusivity);

            ///////////////////////////////

            FetchLabel(Enums.eBarType.MezProtectionHold).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Held]);
            FetchLabel(Enums.eBarType.MezProtectionStunned).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Stunned]);
            FetchLabel(Enums.eBarType.MezProtectionSleep).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Sleep]);
            FetchLabel(Enums.eBarType.MezProtectionImmob).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Immobilized]);
            FetchLabel(Enums.eBarType.MezProtectionKnockback).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Knockback]);
            FetchLabel(Enums.eBarType.MezProtectionRepel).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Repel]);
            FetchLabel(Enums.eBarType.MezProtectionConfuse).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Confused]);
            FetchLabel(Enums.eBarType.MezProtectionFear).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Terrorized]);
            FetchLabel(Enums.eBarType.MezProtectionTaunt).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Taunt]);
            FetchLabel(Enums.eBarType.MezProtectionPlacate).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Placate]);
            FetchLabel(Enums.eBarType.MezProtectionTeleport).Text = FormatValue(1, MidsContext.Character.Totals.Mez[(int)Enums.eMez.Teleport]);

            FetchLabel(Enums.eBarType.MezResistanceHold).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Held]);
            FetchLabel(Enums.eBarType.MezResistanceStunned).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Stunned]);
            FetchLabel(Enums.eBarType.MezResistanceSleep).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Sleep]);
            FetchLabel(Enums.eBarType.MezResistanceImmob).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Immobilized]);
            FetchLabel(Enums.eBarType.MezResistanceKnockback).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Knockback]);
            FetchLabel(Enums.eBarType.MezResistanceRepel).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Repel]);
            FetchLabel(Enums.eBarType.MezResistanceConfuse).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Confused]);
            FetchLabel(Enums.eBarType.MezResistanceFear).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Terrorized]);
            FetchLabel(Enums.eBarType.MezResistanceTaunt).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Taunt]);
            FetchLabel(Enums.eBarType.MezResistancePlacate).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Placate]);
            FetchLabel(Enums.eBarType.MezResistanceTeleport).Text = FormatValue(0, MidsContext.Character.Totals.MezRes[(int)Enums.eMez.Teleport]);

            ///////////////////////////////

            FetchLabel(Enums.eBarType.DebuffResistanceDefense).Text = FormatValue(0, MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Defense]);
            FetchLabel(Enums.eBarType.DebuffResistanceEndurance).Text = FormatValue(0, MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Endurance]);
            FetchLabel(Enums.eBarType.DebuffResistanceRecovery).Text = FormatValue(0, MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Recovery]);
            FetchLabel(Enums.eBarType.DebuffResistancePerception).Text = FormatValue(0, MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.PerceptionRadius]);
            FetchLabel(Enums.eBarType.DebuffResistanceToHit).Text = FormatValue(0, MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.ToHit]);
            FetchLabel(Enums.eBarType.DebuffResistanceRechargeTime).Text = FormatValue(0, MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.RechargeTime]);
            FetchLabel(Enums.eBarType.DebuffResistanceSpeedRunning).Text = FormatValue(0, MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.SpeedRunning]);
            FetchLabel(Enums.eBarType.DebuffResistanceRegen).Text = FormatValue(0, MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Regeneration]);
            #endregion
        }
    }
}
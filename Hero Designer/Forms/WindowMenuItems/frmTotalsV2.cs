using System;
using System.Collections.Generic;
using System.Drawing;
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
        private ctlLayeredBar[] Bars;
        private readonly int BarMaxWidth = 307;

        public Control StatControl(string tab, int panel, string type, int control)
        {
            var regEx = new Regex(@"^\d+");
            var page = tabControlAdv1.Controls.OfType<TabPageAdv>().First(t => t.Text.Contains(tab));
            var gradientList = page.Controls.OfType<GradientPanel>().ToList();
            var gradientPanels = gradientList.OrderBy(x => x.Name).ToList();
            var tablePanels = gradientPanels[panel - 1].Controls.OfType<TableLayoutPanel>().ToList();

            switch (type)
            {
                case "Bar":
                    var controls = new List<Control>();
                    for (var rowIndex = 0; rowIndex < tablePanels[0].RowCount; rowIndex++)
                    {
                        var tControl = tablePanels[0].GetControlFromPosition(2, rowIndex);
                        controls.Add(tControl);
                    }
                    var barList = controls.OfType<ctlLayeredBar>().ToList();
                    var bars = barList.OrderBy(x => regEx.Match(x.Name).Value).ToList();

                    return bars[control - 1];
                case "Label":
                    controls = new List<Control>();
                    for (var rowIndex = 0; rowIndex < tablePanels[0].RowCount; rowIndex++)
                    {
                        var tControl = tablePanels[0].GetControlFromPosition(1, rowIndex);
                        controls.Add(tControl);
                    }
                    var labelList = controls.OfType<Label>().ToList();
                    var labels = labelList.OrderBy(x => regEx.Match(x.Name).Value).ToList();

                    return labels[control - 1];
            }

            return null;
        }

        private ctlLayeredBar FetchBar(int index)
        {
            switch (index)
            {
                // Defense
                case 0: return ctlLayeredBar3;
                case 1: return ctlLayeredBar4;
                case 2: return ctlLayeredBar5;
                case 3: return ctlLayeredBar6;
                case 4: return ctlLayeredBar7;
                case 5: return ctlLayeredBar8;
                case 6: return ctlLayeredBar9;
                case 7: return ctlLayeredBar10;
                case 8: return ctlLayeredBar11;
                case 9: return ctlLayeredBar12;

                // Resistance
                case 10: return ctlLayeredBar13;
                case 11: return ctlLayeredBar19;
                case 12: return ctlLayeredBar20;
                case 13: return ctlLayeredBar21;
                case 14: return ctlLayeredBar22;
                case 15: return ctlLayeredBar23;
                case 16: return ctlLayeredBar24;
                case 17: return ctlLayeredBar25;

                // Regen/HP/Absorb
                case 18: return ctlLayeredBar14;
                case 19: return ctlLayeredBar15;

                // Endurance
                case 20: return ctlLayeredBar16;
                case 21: return ctlLayeredBar17;
                case 22: return ctlLayeredBar18;

                // Movement
                case 23: return ctlLayeredBar51;
                case 24: return ctlLayeredBar50;
                case 25: return ctlLayeredBar49;
                case 26: return ctlLayeredBar48;

                // Stealth/Perception
                case 27: return ctlLayeredBar63;
                case 28: return ctlLayeredBar60;
                case 29: return ctlLayeredBar57;

                // Misc
                case 30: return ctlLayeredBar72;
                case 31: return ctlLayeredBar56;
                case 32: return ctlLayeredBar68;
                case 33: return ctlLayeredBar71;
                case 34: return ctlLayeredBar78;
                case 35: return ctlLayeredBar81;
                case 36: return ctlLayeredBar85;

                // Status Protection
                case 37: return ctlLayeredBar97;
                case 38: return ctlLayeredBar94;
                case 39: return ctlLayeredBar91;
                case 40: return ctlLayeredBar88;
                case 41: return ctlLayeredBar89;
                case 42: return ctlLayeredBar90;
                case 43: return ctlLayeredBar92;
                case 44: return ctlLayeredBar93;
                case 45: return ctlLayeredBar95;
                case 46: return ctlLayeredBar96;
                case 47: return ctlLayeredBar98;

                // Status Resistance
                case 48: return ctlLayeredBar102;
                case 49: return ctlLayeredBar101;
                case 50: return ctlLayeredBar100;
                case 51: return ctlLayeredBar99;
                case 52: return ctlLayeredBar103;
                case 53: return ctlLayeredBar104;
                case 54: return ctlLayeredBar105;
                case 55: return ctlLayeredBar106;
                case 56: return ctlLayeredBar107;
                case 57: return ctlLayeredBar108;
                case 58: return ctlLayeredBar109;
                // Debuff Resistance
                case 59: return ctlLayeredBar113;
                case 60: return ctlLayeredBar112;
                case 61: return ctlLayeredBar111;
                case 62: return ctlLayeredBar110;
                case 63: return ctlLayeredBar114;
                case 64: return ctlLayeredBar115;
                case 65: return ctlLayeredBar116;

                default: return ctlLayeredBar3;
            };
        }

        private ctlLayeredBar FetchBar(Enums.eBarType barType)
        {
            return FetchBar((int) barType);
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

            FetchBar(Enums.eBarType.MezProtectionHold).ValueMainBar = MidsContext.Character.Totals.Mez[(int) Enums.eMez.Held];
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

            FetchBar(Enums.eBarType.MezResistanceHold).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Held];
            FetchBar(Enums.eBarType.MezResistanceStunned).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Stunned];
            FetchBar(Enums.eBarType.MezResistanceSleep).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Sleep];
            FetchBar(Enums.eBarType.MezResistanceImmob).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Immobilized];
            FetchBar(Enums.eBarType.MezResistanceKnockback).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Knockback];
            FetchBar(Enums.eBarType.MezResistanceRepel).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Repel];
            FetchBar(Enums.eBarType.MezResistanceConfuse).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Confused];
            FetchBar(Enums.eBarType.MezResistanceFear).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Terrorized];
            FetchBar(Enums.eBarType.MezResistanceTaunt).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Taunt];
            FetchBar(Enums.eBarType.MezResistancePlacate).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Placate];
            FetchBar(Enums.eBarType.MezResistanceTeleport).ValueMainBar = MidsContext.Character.Totals.MezRes[(int) Enums.eMez.Teleport];

            ///////////////////////////////

            FetchBar(Enums.eBarType.DebuffResistanceDefense).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int) Enums.eEffectType.Defense];
            FetchBar(Enums.eBarType.DebuffResistanceEndurance).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Endurance];
            FetchBar(Enums.eBarType.DebuffResistanceRecovery).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Recovery];
            FetchBar(Enums.eBarType.DebuffResistancePerception).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.PerceptionRadius];
            FetchBar(Enums.eBarType.DebuffResistanceToHit).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.ToHit];
            FetchBar(Enums.eBarType.DebuffResistanceRechargeTime).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.RechargeTime];
            FetchBar(Enums.eBarType.DebuffResistanceSpeedRunning).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.SpeedRunning];
            FetchBar(Enums.eBarType.DebuffResistanceRegen).ValueMainBar = MidsContext.Character.Totals.DebuffRes[(int)Enums.eEffectType.Regeneration];
            #endregion
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using MRBResourceLib;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmTotals : Form
    {
        private bool _keepOnTop;
        private bool _loaded;
        private readonly frmMain _myParent;
        private int _tabPage;

        public frmTotals(ref frmMain iParent)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            FormClosed += FrmTotalsFormClosed;
            Load += FrmTotalsLoad;
            Resize += FrmTotalsResize;
            Move += FrmTotalsMove;
            _tabPage = 0;
            _loaded = false;
            _keepOnTop = true;
            InitializeComponent();
            Name = nameof(frmTotals);
            Icon = Resources.MRB_Icon_Concept;
            _myParent = iParent;
        }

        public bool A_GT_B(float a, float b)
        {
            var num = (double)Math.Abs(a - b);

            return num is >= 1.0000000116861E-07 and > 0;
        }

        private void FrmTotalsFormClosed(object sender, FormClosedEventArgs e)
        {
            _myParent.FloatTotals(false, true);
        }

        private void FrmTotalsLoad(object sender, EventArgs e)
        {
            if (MainModule.MidsController.IsAppInitialized)
            {
                if (Size.Width >= 554)
                {
                    Size = new Size(344, 544);
                }

                switch (MidsContext.Config.SpeedFormat)
                {
                    case Enums.eSpeedMeasure.FeetPerSecond:
                        rbFPS.Checked = true;
                        break;
                    case Enums.eSpeedMeasure.MetersPerSecond:
                        rbMSec.Checked = true;
                        break;
                    case Enums.eSpeedMeasure.KilometersPerHour:
                        rbKPH.Checked = true;
                        break;
                    default:
                        rbMPH.Checked = true;
                        break;
                }
            }

            var yOffset = DatabaseAPI.RealmUsesToxicDef() ? 10 : 0;
            graphDef.Size = new Size(300, 152 + yOffset);
            lblRes.Location = new Point(3, 164 + yOffset);
            graphRes.Location = new Point(15, 183 + yOffset);
            lblRegenRec.Location = new Point(3, 302 + yOffset);
            graphRegen.Location = new Point(15, 321 + yOffset);
            graphHP.Location = new Point(15, 339 + yOffset);
            graphRec.Location = new Point(15, 357 + yOffset);
            graphDrain.Location = new Point(15, 375 + yOffset);
            graphMaxEnd.Location = new Point(15, 394 + yOffset);

            _loaded = true;
            SetFonts();
        }

        private void FrmTotalsMove(object sender, EventArgs e)
        {
            StoreLocation();
        }

        private void FrmTotalsResize(object sender, EventArgs e)
        {
            if (!_loaded)
            {
                return;
            }

            pnlDRHE.Width = ClientSize.Width - pnlDRHE.Left * 2;
            pnlMisc.Width = pnlDRHE.Width;
            graphAcc.Width = pnlDRHE.Width - (graphAcc.Left + 4);
            graphDam.Width = graphAcc.Width;
            graphDef.Width = graphAcc.Width;
            graphDrain.Width = graphAcc.Width;
            graphHaste.Width = graphAcc.Width;
            graphHP.Width = graphAcc.Width;
            graphMaxEnd.Width = graphAcc.Width;
            graphMovement.Width = graphAcc.Width;
            graphRec.Width = graphAcc.Width;
            graphRegen.Width = graphAcc.Width;
            graphRes.Width = graphAcc.Width;
            graphStealth.Width = graphAcc.Width;
            graphToHit.Width = graphAcc.Width;
            graphElusivity.Width = graphAcc.Width;
            graphEndRdx.Width = graphAcc.Width;
            graphThreat.Width = graphAcc.Width;
            graphRange.Width = graphAcc.Width;
            Panel2.Width = graphAcc.Width;
            pbClose.Left = pnlDRHE.Right - pbClose.Width;
            StoreLocation();
        }

        private void PbCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void PbClosePaint(object sender, PaintEventArgs e)
        {
            if (_myParent?.Drawing == null)
            {
                return;
            }

            var iStr = "Close";
            var rectangle = new Rectangle();
            ref var local = ref rectangle;
            var size = _myParent.Drawing.BxPower[2].Size;
            var width = size.Width;
            size = _myParent.Drawing.BxPower[2].Size;
            var height1 = size.Height;
            local = new Rectangle(0, 0, width, height1);
            var destRect = new Rectangle(0, 0, tab0.Width, tab0.Height);
            var stringFormat = new StringFormat();
            var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            extendedBitmap.Graphics.DrawImage(
                MidsContext.Character.IsHero()
                    ? _myParent.Drawing.BxPower[2].Bitmap
                    : _myParent.Drawing.BxPower[4].Bitmap, destRect, 0, 0, rectangle.Width,
                rectangle.Height, GraphicsUnit.Pixel, _myParent.Drawing.PImageAttributes);

            var height2 = bFont.GetHeight(e.Graphics) + 2;
            var bounds = new RectangleF(0f, (tab0.Height - height2) / 2f, tab0.Width, height2);
            var graphics = extendedBitmap.Graphics;
            ClsDrawX.DrawOutlineText(iStr, bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1, graphics);
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
            if (_myParent?.Drawing == null)
            {
                return;
            }

            var index = 2;
            if (_keepOnTop)
            {
                index = 3;
            }

            var iStr = "Keep On Top";
            var rectangle = new Rectangle(0, 0, _myParent.Drawing.BxPower[index].Size.Width,
                _myParent.Drawing.BxPower[index].Size.Height);
            var destRect = new Rectangle(0, 0, tab0.Width, tab0.Height);
            var stringFormat = new StringFormat();
            var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            if (index == 3)
            {
                extendedBitmap.Graphics.DrawImage(MidsContext.Character.IsHero()
                        ? _myParent.Drawing.BxPower[3].Bitmap
                        : _myParent.Drawing.BxPower[5].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                    GraphicsUnit.Pixel);
            }
            else
            {
                extendedBitmap.Graphics.DrawImage(MidsContext.Character.IsHero()
                        ? _myParent.Drawing.BxPower[2].Bitmap
                        : _myParent.Drawing.BxPower[4].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                    GraphicsUnit.Pixel, _myParent.Drawing.PImageAttributes);
            }

            var height = bFont.GetHeight(e.Graphics) + 2;
            var bounds = new RectangleF(0f, (tab0.Height - height) / 2f, tab0.Width, height);
            var graphics = extendedBitmap.Graphics;
            ClsDrawX.DrawOutlineText(iStr, bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        private void RbSpeedCheckedChanged(object sender, EventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            if (rbMPH.Checked)
            {
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.MilesPerHour;
            }
            else if (rbKPH.Checked)
            {
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.KilometersPerHour;
            }
            else if (rbFPS.Checked)
            {
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.FeetPerSecond;
            }
            else if (rbMSec.Checked)
            {
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.MetersPerSecond;
            }
            //UpdateData();
        }

        private static void SetFontDataSingle(ref CtlMultiGraph iGraph)
        {
            //iGraph.Font = new Font(iGraph.Font.FontFamily, MidsContext.Config.RtFont.PairedBase, FontStyle.Bold, GraphicsUnit.Point);
            iGraph.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        private void SetFonts()
        {
            var graphControls = new List<CtlMultiGraph>
            {
                graphAcc, graphDam, graphRange, graphDef, graphDrain, graphHaste, graphHP,
                graphMaxEnd, graphMovement, graphRec, graphRegen, graphRes,
                graphStealth, graphToHit, graphEndRdx, graphThreat,
                graphElusivity, graphSProt, graphSRes, graphSDeb
            };

            foreach (var graphControl in graphControls)
            {
                var g = graphControl;
                SetFontDataSingle(ref g);
            }

            lblDef.Font = graphDef.Font;
            lblMisc.Font = graphDef.Font;
            lblMovement.Font = graphDef.Font;
            lblRegenRec.Font = graphDef.Font;
            lblRes.Font = graphDef.Font;
            lblStealth.Font = graphDef.Font;
            lblSRes.Font = graphDef.Font;
            lblSProt.Font = graphDef.Font;
            lblSDeb.Font = graphDef.Font;
        }

        public void SetLocation()
        {
            var rectangle = new Rectangle();
            pnlMisc.Left = pnlDRHE.Left;
            pnlStatus.Left = pnlDRHE.Left;
            Width -= ClientSize.Width + pnlDRHE.Left * 2 + 320;
            rectangle.X = MainModule.MidsController.SzFrmTotals.X;
            rectangle.Y = MainModule.MidsController.SzFrmTotals.Y;
            rectangle.Width = MainModule.MidsController.SzFrmTotals.Width;
            rectangle.Height = MainModule.MidsController.SzFrmTotals.Height;
            if (rectangle.Width < 1)
            {
                rectangle.Width = Width;
            }

            if (rectangle.Height < 1)
            {
                rectangle.Height = Height;
            }

            if (rectangle.Width < MinimumSize.Width)
            {
                rectangle.Width = MinimumSize.Width;
            }

            if (rectangle.Height < MinimumSize.Height)
            {
                rectangle.Height = MinimumSize.Height;
            }

            if (rectangle.X < 1)
            {
                rectangle.X = _myParent.Left + 8;
            }

            if (rectangle.Y < 32)
            {
                rectangle.Y = _myParent.Top + (_myParent.Height - _myParent.ClientSize.Height) + _myParent.GetPrimaryBottom();
            }

            Top = rectangle.Y;
            Left = rectangle.X;
            Height = rectangle.Height;
            Width = rectangle.Width;
            _loaded = true;
            FrmTotalsResize(this, EventArgs.Empty);
        }

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            MainModule.MidsController.SzFrmTotals.X = Left;
            MainModule.MidsController.SzFrmTotals.Y = Top;
            MainModule.MidsController.SzFrmTotals.Width = Width;
            MainModule.MidsController.SzFrmTotals.Height = Height;
        }

        private void Tab0Click(object sender, EventArgs e)
        {
            TabPageChange(0);
        }

        private void Tab0Paint(object sender, PaintEventArgs e)
        {
            var tab0 = this.tab0;
            TabPaint(ref tab0, e, "Survival", _tabPage == 0);
            this.tab0 = tab0;
        }

        private void Tab1Click(object sender, EventArgs e)
        {
            TabPageChange(1);
        }

        private void Tab1Paint(object sender, PaintEventArgs e)
        {
            var tab1 = this.tab1;
            TabPaint(ref tab1, e, "Misc Buffs", _tabPage == 1);
            this.tab1 = tab1;
        }

        private void Tab2Click(object sender, EventArgs e)
        {
            TabPageChange(2);
        }

        private void Tab2Paint(object sender, PaintEventArgs e)
        {
            var tab2 = this.tab2;
            TabPaint(ref tab2, e, "Status", _tabPage == 2);
            this.tab2 = tab2;
        }

        private void TabPageChange(int index)
        {
            switch (index)
            {
                case 0:
                    pnlDRHE.Visible = true;
                    pnlMisc.Visible = false;
                    pnlStatus.Visible = false;
                    break;
                case 1:
                    pnlDRHE.Visible = false;
                    pnlMisc.Visible = true;
                    pnlStatus.Visible = false;
                    break;
                case 2:
                    pnlDRHE.Visible = false;
                    pnlMisc.Visible = false;
                    pnlStatus.Visible = true;
                    break;
            }

            _tabPage = index;
            tab0.Refresh();
            tab1.Refresh();
            tab2.Refresh();
        }

        private void TabPaint(ref PictureBox iTab, PaintEventArgs e, string iString, bool iState)
        {
            if (_myParent?.Drawing == null)
            {
                return;
            }

            var index = 2;
            if (iState)
            {
                index = 3;
            }

            var rectangle = new Rectangle(0, 0, _myParent.Drawing.BxPower[index].Size.Width,
                _myParent.Drawing.BxPower[index].Size.Height);
            var destRect = new Rectangle(0, 0, iTab.Width, iTab.Height);
            var stringFormat = new StringFormat();
            var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            if (index == 3)
            {
                extendedBitmap.Graphics.DrawImage(MidsContext.Character.IsHero()
                        ? _myParent.Drawing.BxPower[3].Bitmap
                        : _myParent.Drawing.BxPower[5].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                    GraphicsUnit.Pixel);
            }
            else
            {
                extendedBitmap.Graphics.DrawImage(MidsContext.Character.IsHero()
                        ? _myParent.Drawing.BxPower[2].Bitmap
                        : _myParent.Drawing.BxPower[4].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                    GraphicsUnit.Pixel, _myParent.Drawing.PImageAttributes);
            }

            var height = bFont.GetHeight(e.Graphics) + 2;
            var bounds = new RectangleF(0f, (tab0.Height - height) / 2f, tab0.Width, height);
            var graphics = extendedBitmap.Graphics;
            ClsDrawX.DrawOutlineText(iString, bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        private static string FormatSpeed(float iSpeed, Statistics displayStats, Enums.eSpeedMeasure speedFormat, string rateDisp)
        {
            return $"{displayStats.Speed(iSpeed, speedFormat):##0.##}{rateDisp}.";
        }

        private static string FormatDistance(float iSpeed, Statistics displayStats, Enums.eSpeedMeasure speedFormat)
        {
            return $"{displayStats.Distance(iSpeed, speedFormat):##0.##}";
        }

        private void AddGraphMovementItem(string title, Func<Enums.eSpeedMeasure, bool, float> dispStatsF, string tip, Enums.eSpeedMeasure speedFormat, string rateDisp)
        {
            graphMovement.AddItem($"{title}{dispStatsF(speedFormat, false):##0.##}{rateDisp}", dispStatsF(Enums.eSpeedMeasure.FeetPerSecond, false), dispStatsF(Enums.eSpeedMeasure.FeetPerSecond, true), tip);
        }

        public void UpdateData()
        {
            var defDmgNames = Enum.GetNames(Enums.eDamage.None.GetType());
            pbClose.Refresh();
            pbTopMost.Refresh();
            tab0.Refresh();
            tab1.Refresh();
            var displayStats = MidsContext.Character.DisplayStats;
            graphDef.Clear();

            var skipDefVectors = new List<Enums.eDamage>
            {
                DatabaseAPI.RealmUsesToxicDef() ? Enums.eDamage.None : Enums.eDamage.Toxic,
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>().ToList();

            for (var dType = 1; dType < defDmgNames.Length; dType++)
            {
                if (skipDefVectors.Contains(dType))
                {
                    continue;
                }

                var iTip = $"{displayStats.Defense(dType):##0.##}% {defDmgNames[dType]} defense";
                graphDef.AddItem($"{defDmgNames[dType]}| {displayStats.Defense(dType):##0.##}%", Math.Max(0, displayStats.Defense(dType)), 0, iTip);
            }

            graphDef.Max = 100;
            graphDef.Draw();

            graphRes.Clear();
            var str1 = $"{MidsContext.Character.Archetype.DisplayName} resistance cap: {MidsContext.Character.Archetype.ResCap * 100:###0}%";
            for (var dType = 1; dType < 9; dType++)
            {
                if (dType == 9)
                {
                    continue;
                }

                var iTip =
                    MidsContext.Character.TotalsCapped.Res[dType] < MidsContext.Character.Totals.Res[dType]
                        ? $"{displayStats.DamageResistance(dType, true):##0.##}% {defDmgNames[dType]} resistance capped at {displayStats.DamageResistance(dType, false):##0.##}%"
                        : $"{displayStats.DamageResistance(dType, true):##0.##}% {defDmgNames[dType]} resistance. ({str1})";
                graphRes.AddItem($"{defDmgNames[dType]}|{displayStats.DamageResistance(dType, false):##0.##}%", Math.Max(0, displayStats.DamageResistance(dType, false)), Math.Max(0, displayStats.DamageResistance(dType, true)), iTip);
            }

            graphRes.Max = 100f;
            graphRes.Draw();

            var drainTip = "";
            var str2 = $"Time to go from 0-100% end: {Utilities.FixDP(displayStats.EnduranceTimeToFull)}s.";
            if (Math.Abs(displayStats.EnduranceRecoveryPercentage(false) - displayStats.EnduranceRecoveryPercentage(true)) > 0.01)
            {
                str2 += $"\r\nCapped from a total of: {displayStats.EnduranceRecoveryPercentage(true):###0}%.";
            }

            var recTip = $"{str2}\r\nHover the mouse of the End Drain stats for more info.";
            switch (displayStats.EnduranceRecoveryNet)
            {
                case > 0:
                    drainTip = $"Net Endurance Gain (Recovery - Drain): {Utilities.FixDP(displayStats.EnduranceRecoveryNet)}/s.";
                    if (Math.Abs(displayStats.EnduranceRecoveryNet - displayStats.EnduranceRecoveryNumeric) > 0.01)
                    {
                        drainTip += $"\r\nTime to go from 0-100% end (using net gain): {Utilities.FixDP(displayStats.EnduranceTimeToFullNet)}s.";
                    }

                    break;
                case < 0:
                    drainTip = $"With current end drain, you will lose end at a rate of: {Utilities.FixDP(displayStats.EnduranceRecoveryLossNet)}/s.\r\nFrom 100% you would run out of end in: {Utilities.FixDP(displayStats.EnduranceTimeToZero)}s.";
                    break;
            }

            graphMaxEnd.Clear();
            var iTip3 = $"Base Endurance: 100\r\nCurrent Max End: {Utilities.FixDP(displayStats.EnduranceMaxEnd)}";
            if (MidsContext.Character.Totals.EndMax > 0)
            {
                iTip3 += $"\r\nYour maximum endurance has been increased by {Utilities.FixDP(displayStats.EnduranceMaxEnd - 100f)}%";
            }

            graphMaxEnd.AddItem($"Max End|{Utilities.FixDP(displayStats.EnduranceMaxEnd)}%", Math.Max(0, displayStats.EnduranceMaxEnd), 0, iTip3);
            graphMaxEnd.Max = 150;
            graphMaxEnd.MarkerValue = 100;
            graphMaxEnd.Draw();

            graphDrain.Clear();
            graphDrain.AddItem($"EndUse|{MidsContext.Character.Totals.EndUse:##0.##}/s", MidsContext.Character.Totals.EndUse, MidsContext.Character.Totals.EndUse, drainTip);
            graphDrain.Max = 4;
            graphDrain.Draw();

            graphRec.Clear();
            graphRec.AddItem($"EndRec|{displayStats.EnduranceRecoveryPercentage(false):###0}% ({displayStats.EnduranceRecoveryNumeric:##0.##}/s)", Math.Max(0, displayStats.EnduranceRecoveryPercentage(false)), Math.Max(0, displayStats.EnduranceRecoveryPercentage(true)), recTip);
            graphRec.Max = 400;
            graphRec.MarkerValue = 100;
            graphRec.Draw();

            var iTip4 = $"Time to go from 0-100% health: {Utilities.FixDP(displayStats.HealthRegenTimeToFull)}s.\r\nHealth regenerated per second: {Utilities.FixDP(displayStats.HealthRegenHealthPerSec)}%\r\nHitPoints regenerated per second at level 50: {Utilities.FixDP(displayStats.HealthRegenHPPerSec)} HP";
            if (Math.Abs(displayStats.HealthRegenPercent(false) - displayStats.HealthRegenPercent(true)) > 0.01)
            {
                iTip4 += $"\r\nCapped from a total of: {displayStats.HealthRegenPercent(true):###0}%.";
            }

            graphRegen.Clear();
            graphRegen.AddItem($"Regeneration|{displayStats.HealthRegenPercent(false):###0}%", Math.Max(0, displayStats.HealthRegenPercent(false)), Math.Max(0, displayStats.HealthRegenPercent(true)), iTip4);
            graphRegen.Max = graphRegen.GetMaxValue();
            graphRegen.MarkerValue = 100f;
            graphRegen.Draw();

            graphHP.Clear();
            var iTip5 = $"Base HitPoints: {MidsContext.Character.Archetype.Hitpoints}\r\nCurrent HitPoints: {displayStats.HealthHitpointsNumeric(false)}";
            if (Math.Abs(displayStats.HealthHitpointsNumeric(false) - displayStats.HealthHitpointsNumeric(true)) > 0.01)
            {
                iTip5 += $"\r\n(Capped from a total of: {displayStats.HealthHitpointsNumeric(true):###0.##})";
            }

            graphHP.AddItem($"Max HP|{displayStats.HealthHitpointsPercentage:###0.##}%", Math.Max(0, displayStats.HealthHitpointsPercentage), Math.Max(0, displayStats.HealthHitpointsPercentage), iTip5);
            graphHP.Max = (float)(MidsContext.Character.Archetype.HPCap / (double)MidsContext.Character.Archetype.Hitpoints * 100);
            graphHP.MarkerValue = 100f;
            graphHP.Draw();

            graphMovement.Clear();
            var speedFormat = MidsContext.Config.SpeedFormat;
            var rateDisp = "MPH";
            var lengthDisp = " m";
            switch (speedFormat)
            {
                case Enums.eSpeedMeasure.FeetPerSecond:
                    rateDisp = " Ft/Sec";
                    lengthDisp = " ft";
                    break;
                case Enums.eSpeedMeasure.MetersPerSecond:
                    rateDisp = " m/s";
                    lengthDisp = " m";
                    break;
                case Enums.eSpeedMeasure.MilesPerHour:
                    rateDisp = " mph";
                    lengthDisp = " ft";
                    break;
                case Enums.eSpeedMeasure.KilometersPerHour:
                    rateDisp = " km/h";
                    lengthDisp = " m";
                    break;
            }

            var strCap = "This has been capped at the maximum in-game speed.\r\nUncapped speed: ";
            var fltTip = $"Base Flight Speed: {FormatSpeed(Statistics.BaseFlySpeed, displayStats, speedFormat, rateDisp)}"; // 31.5
            var jmpTip2 = $"Base Jump Height: {FormatDistance(Statistics.BaseJumpHeight, displayStats, speedFormat)}"; // 4
            var iTip8 = $"Base Run Speed: {FormatSpeed(Statistics.BaseRunSpeed, displayStats, speedFormat, rateDisp)}"; // 21

            if (A_GT_B(displayStats.MovementFlySpeed(speedFormat, true), displayStats.MovementFlySpeed(speedFormat, false)))
            {
                fltTip += $"\r\n{strCap}{displayStats.Speed(MidsContext.Character.Totals.FlySpd, speedFormat):##0.##}{rateDisp}.";
            }
            else if (Math.Abs(displayStats.MovementFlySpeed(speedFormat, false)) < float.Epsilon)
            {
                fltTip += "\r\nYou have no active flight powers.";
            }

            var jumpTip = $"Base Jump Speed: {FormatSpeed(Statistics.BaseJumpSpeed, displayStats, speedFormat, rateDisp)}"; // 21f
            if (A_GT_B(displayStats.MovementJumpSpeed(speedFormat, true), displayStats.MovementJumpSpeed(speedFormat, false)))
            {
                jumpTip += $"\r\n{strCap}{FormatSpeed(MidsContext.Character.Totals.JumpSpd, displayStats, speedFormat, rateDisp)}";
            }

            if (A_GT_B(displayStats.MovementRunSpeed(speedFormat, true), displayStats.MovementRunSpeed(speedFormat, false)))
            {
                iTip8 += $"\r\n{strCap}{FormatSpeed(MidsContext.Character.Totals.RunSpd, displayStats, speedFormat, rateDisp)}";
            }

            var jmpHtTip = !(speedFormat == Enums.eSpeedMeasure.FeetPerSecond | speedFormat == Enums.eSpeedMeasure.MilesPerHour)
                ? jmpTip2 + " m."
                : jmpTip2 + " ft.";

            AddGraphMovementItem("Run|", displayStats.MovementRunSpeed, iTip8, speedFormat, rateDisp);
            AddGraphMovementItem("Jump|", displayStats.MovementJumpSpeed, jumpTip, speedFormat, rateDisp);
            graphMovement.AddItem($"Jump Height|{displayStats.MovementJumpHeight(speedFormat):##0.##}{lengthDisp}", displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond), displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond), jmpHtTip);
            AddGraphMovementItem("Fly|", displayStats.MovementFlySpeed, fltTip, speedFormat, rateDisp);
            graphMovement.ForcedMax = displayStats.Speed(200f, Enums.eSpeedMeasure.FeetPerSecond);
            graphMovement.Draw();

            graphToHit.Clear();
            graphToHit.AddItem($"ToHit|{displayStats.BuffToHit:##0.##}%", Math.Max(0, displayStats.BuffToHit), 0, "This effect increases the accuracy of all your powers.\r\nToHit values are added together before being multiplied by Accuracy.");
            graphToHit.Max = 100;
            graphToHit.Draw();

            graphAcc.Clear();
            graphAcc.AddItem($"Accuracy|{displayStats.BuffAccuracy:##0.##}%", Math.Max(0, displayStats.BuffAccuracy), 0, "This effect increases the accuracy of all your powers.\r\nAccuracy buffs are usually applied as invention set bonuses.");
            graphAcc.Max = 100;
            graphAcc.Draw();

            graphDam.Clear();
            var str7 = "";
            if (A_GT_B(displayStats.BuffDamage(true), displayStats.BuffDamage(false)))
            {
                str7 = $"\r\n\nDamage Capped from {displayStats.BuffDamage(true)}% to {displayStats.BuffDamage(false)}%";
            }

            graphDam.AddItem($"Damage|{displayStats.BuffDamage(false) - 100:##0.##}%", Math.Max(0, displayStats.BuffDamage(false)), Math.Max(0, displayStats.BuffDamage(true)), $"This effect alters the damage dealt by all your attacks.\r\nAs some powers can reduce your damage output, this bar has your base damage (100%) included.{str7}");
            graphDam.Max = MidsContext.Character.Archetype.DamageCap * 100f;
            graphDam.MarkerValue = 100f;
            graphDam.Draw();

            graphRange.Clear();
            graphRange.AddItem($"Range|{displayStats.RangePercent:##0.##}%", 0, Math.Max(0, displayStats.RangePercent), "This effect increases the range of all your powers.");
            graphRange.Max = 300;
            graphRange.Draw();

            graphHaste.Clear();
            var str8 = "";
            if (A_GT_B(displayStats.BuffHaste(true), displayStats.BuffHaste(false)))
            {
                str8 = $"\r\n\r\nRecharge Speed Capped from {displayStats.BuffHaste(true)}% to {displayStats.BuffHaste(false)}%";
            }

            graphHaste.AddItem($"Haste|{displayStats.BuffHaste(false) - 100:##0.##}%", Math.Max(0, displayStats.BuffHaste(false)), Math.Max(0, displayStats.BuffHaste(true)), $"This effect alters the recharge speed of all your powers.\r\nThe higher the value, the faster the recharge.\r\nAs some powers can slow your recharge, this bar starts with your base recharge (100%) included.{str8}");
            graphHaste.MarkerValue = 100f;
            graphHaste.Max = displayStats.BuffHaste(true) switch
            {
                <= 280 => 300,
                <= 380 => 400,
                _ => 500
            };
            graphHaste.Draw();

            graphEndRdx.Clear();
            graphEndRdx.AddItem($"EndRdx|{displayStats.BuffEndRdx:##0.##}%", displayStats.BuffEndRdx, displayStats.BuffEndRdx, "This effect is applied to powers in addition to endurance reduction enhancements.");
            graphEndRdx.Max = 200;
            graphEndRdx.Draw();

            graphStealth.Clear();
            graphStealth.AddItem($"PvE|{MidsContext.Character.Totals.StealthPvE:##0} ft", MidsContext.Character.Totals.StealthPvE, 0.0f, "This is subtracted from a mob's perception to work out if they can see you.");
            graphStealth.AddItem($"PvE|{MidsContext.Character.Totals.StealthPvP:##0} ft", MidsContext.Character.Totals.StealthPvE, 0.0f, "This is subtracted from a player's perception to work out if they can see you.");
            graphStealth.AddItem($"Perception|{displayStats.Perception(false):###0} ft", displayStats.Perception(false), 0.0f, "This, minus a player's stealth radius, is the distance you can see it.");
            graphStealth.Max = graphStealth.GetMaxValue() * 1.01f;
            graphStealth.Draw();
            var iTip10 = $"This affects how mobs prioritize you as a threat.\r\nLower values make you a less tempting target.\r\nThe {MidsContext.Character.Archetype.DisplayName} base Threat Level of {MidsContext.Character.Archetype.BaseThreat * 100.0:###}% is included in this figure.";
            var nBase = displayStats.ThreatLevel + 200;
            graphThreat.Clear();
            graphThreat.AddItem($"Threat Level|{displayStats.ThreatLevel:##0}%", nBase, 0, iTip10);
            graphThreat.MarkerValue = MidsContext.Character.Archetype.BaseThreat * 100 + 200;
            graphThreat.Max = 800;
            graphThreat.Draw();

            graphElusivity.Clear();
            var sElusivity = MidsContext.Character.Totals.ElusivityMax;
            graphElusivity.AddItem($"Elusivity|{sElusivity * 100:##0.##}%", Math.Max(0, sElusivity * 100), 0, "This effect resists accuracy buffs of enemies attacking you.");
            graphElusivity.Max = 100;
            graphElusivity.Draw();
            if (Math.Abs(graphAcc.Font.Size - MidsContext.Config.RtFont.PairedBase) > float.Epsilon)
            {
                SetFonts();
            }

            var totals = MidsContext.Character.Totals;
            var str9 = "\r\nStatus protection prevents you being affected by a status effect such as" + "\r\na Hold until the magnitude of the effect exceeds that of the protection.";
            var str10 = "\r\nStatus resistance reduces the time you are affected by a status effect such as" + "\r\na Hold. Note that 100% resistance would make a 10s effect last 5s, and not 0s.";
            graphSProt.Clear();
            graphSRes.Clear();
            Enums.eMez[] eMezArray =
            {
                Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep, Enums.eMez.Immobilized, Enums.eMez.Knockback, Enums.eMez.Repel,
                Enums.eMez.Confused, Enums.eMez.Terrorized, Enums.eMez.Taunt, Enums.eMez.Placate, Enums.eMez.Teleport
            };
            var names2 = Enum.GetNames<Enums.eMez>(); // Enum.GetNames(eMezArray[0].GetType());
            var names3 = Enum.GetNames<Enums.eMez>(); // Enum.GetNames(eMezArray[0].GetType());
            names2[2] = "Hold";
            names2[3] = "Immob";
            names2[1] = "Confuse";
            names2[12] = "Fear";
            names3[2] = "Hold";
            names3[1] = "Confuse";
            names3[12] = "Fear (Terrorized)";
            names3[4] = "Knockback and Knockup";
            var sResMax = 5;
            foreach (var e in eMezArray)
            {
                var mezProtection = totals.Mez[(int)e] > 0 ? 0 : Math.Abs(totals.Mez[(int)e]);
                var iTip11 = mezProtection < float.Epsilon
                    ? $"You have no protection from {names3[(int)e]} effects.\r\n{str9}"
                    : $"You have mag {mezProtection:##0.##} protection from {names3[(int)e]} effects.\r\n{str9}";

                graphSProt.AddItem($"{names2[(int)e]}|{mezProtection:##0.##}", mezProtection, 0, iTip11);

                var mezResPercent = 100 / (1 + totals.MezRes[(int)e] / 100);
                var str11 = "";
                if (e != Enums.eMez.Knockback & e != Enums.eMez.Knockup & e != Enums.eMez.Repel & e != Enums.eMez.Teleport)
                {
                    if (totals.MezRes[(int)e] > sResMax)
                    {
                        sResMax = (int)Math.Round(totals.MezRes[(int)e]);
                    }

                    str11 = $"\r\n{names3[(int)e]} effects will last {mezResPercent:##0.##}% of their full duration.\r\n{str10}";
                }
                else if (e == Enums.eMez.Teleport)
                {
                    str11 = $"\r\n{names3[(int)e]} effects will be resisted.\r\n{str10}";
                }
                else
                {
                    str11 = $"\r\n{names3[(int)e]} effects will have {mezResPercent:##0.##}% of their full effect.\r\n{str10}";
                }

                var iTip12 = Math.Abs(totals.MezRes[(int)e]) < float.Epsilon
                    ? $"You have no resistance to {names3[(int)e]} effects.\r\n{str10}"
                    : $"You have {totals.Mez[(int)e]:##0.##}% resistance to {names3[(int)e]} effects.{str11}";

                graphSRes.AddItem($"{names2[(int)e]}|{totals.MezRes[(int)e]:##0.##}%", totals.MezRes[(int)e], 0, iTip12);
            }

            graphSProt.Max = graphSProt.GetMaxValue();
            graphSProt.Draw();

            graphSRes.Max = sResMax;
            graphSRes.Draw();

            graphSDeb.Clear();
            Enums.eEffectType[] eEffectTypeArray =
            {
                Enums.eEffectType.Defense, Enums.eEffectType.Endurance, Enums.eEffectType.Recovery, Enums.eEffectType.PerceptionRadius,
                Enums.eEffectType.ToHit, Enums.eEffectType.RechargeTime, Enums.eEffectType.SpeedRunning, Enums.eEffectType.Regeneration
            };

            for (var index = 0; index < eEffectTypeArray.Length; index++)
            {
                var iTip11 = Math.Abs(totals.DebuffRes[(int)eEffectTypeArray[index]]) < 0.001
                    ? $"You have no resistance to {Enums.GetEffectName(eEffectTypeArray[index])} debuffs."
                    : $"You have {totals.DebuffRes[(int)eMezArray[index]]:##0.##}% resistance to {Enums.GetEffectName(eEffectTypeArray[index])} debuffs.";

                graphSDeb.AddItem($"{Enums.GetEffectName(eEffectTypeArray[index])}|{totals.DebuffRes[(int)eEffectTypeArray[index]]:##0.##}%", totals.DebuffRes[(int)eEffectTypeArray[index]], 0f, iTip11);
            }

            graphSDeb.Max = graphSDeb.GetMaxValue() + 1;
            graphSDeb.Draw();
        }
    }
}
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Base.Display;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using midsControls;

namespace Hero_Designer
{
    public partial class frmTotals : Form
    {
        private readonly frmMain _myParent;


        private bool _keepOnTop;
        private bool _loaded;

        private int _tabPage;
        private ctlMultiGraph graphAcc;
        private ctlMultiGraph graphDam;
        private ctlMultiGraph graphDef;
        private ctlMultiGraph graphDrain;
        private ctlMultiGraph graphElusivity;
        private ctlMultiGraph graphEndRdx;
        private ctlMultiGraph graphHaste;
        private ctlMultiGraph graphHP;
        private ctlMultiGraph graphMaxEnd;
        private ctlMultiGraph graphMovement;
        private ctlMultiGraph graphRec;
        private ctlMultiGraph graphRegen;
        private ctlMultiGraph graphRes;
        private ctlMultiGraph graphSDeb;
        private ctlMultiGraph graphSProt;
        private ctlMultiGraph graphSRes;
        private ctlMultiGraph graphStealth;
        private ctlMultiGraph graphThreat;
        private ctlMultiGraph graphToHit;


        public frmTotals(ref frmMain iParent)
        {
            FormClosed += FrmTotalsFormClosed;
            Load += FrmTotalsLoad;
            Resize += FrmTotalsResize;
            Move += FrmTotalsMove;
            _tabPage = 0;
            _loaded = false;
            _keepOnTop = true;
            InitializeComponent();
            Name = nameof(frmTotals);
            var componentResourceManager = new ComponentResourceManager(typeof(frmTotals));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            _myParent = iParent;
        }

        private bool A_GT_B(float A, float B)
        {
            double num = Math.Abs(A - B);
            return num >= 1.0000000116861E-07 && num > 0.0;
        }

        private void FrmTotalsFormClosed(object sender, FormClosedEventArgs e)
        {
            _myParent.FloatTotals(false);
        }

        private void FrmTotalsLoad(object sender, EventArgs e)
        {
            if (MainModule.MidsController.IsAppInitialized)
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
                return;
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
                return;
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
            var destRect = new Rectangle(0, 0, tab0.Width, tab0.Height);
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
            var height2 = bFont.GetHeight(e.Graphics) + 2f;
            var Bounds = new RectangleF(0.0f, (float) ((tab0.Height - (double) height2) / 2.0), tab0.Width, height2);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1f, graphics);
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
                return;
            var index = 2;
            if (_keepOnTop)
                index = 3;
            var iStr = "Keep On top";
            var rectangle = new Rectangle(0, 0, _myParent.Drawing.bxPower[index].Size.Width,
                _myParent.Drawing.bxPower[index].Size.Height);
            var destRect = new Rectangle(0, 0, tab0.Width, tab0.Height);
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
            var height = bFont.GetHeight(e.Graphics) + 2f;
            var Bounds = new RectangleF(0.0f, (float) ((tab0.Height - (double) height) / 2.0), tab0.Width, height);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1f, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        private string PM(float iValue, string iFormat, string iSuff)
        {
            return (double) iValue >= 0.0
                ? (double) iValue <= 0.0 ? "+0" + iSuff : "+" + Strings.Format(iValue, iFormat) + iSuff
                : Strings.Format(iValue, iFormat) + iSuff;
        }

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

        private static void SetFontDataSingle(ref ctlMultiGraph iGraph)
        {
            //iGraph.Font = new Font(iGraph.Font.FontFamily, MidsContext.Config.RtFont.PairedBase, FontStyle.Bold, GraphicsUnit.Point);
            iGraph.Font = new Font("Arial", 11.5f, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        private void SetFonts()
        {
            var graphAcc = this.graphAcc;
            SetFontDataSingle(ref graphAcc);
            this.graphAcc = graphAcc;
            var graphDam = this.graphDam;
            SetFontDataSingle(ref graphDam);
            this.graphDam = graphDam;
            var graphDef = this.graphDef;
            SetFontDataSingle(ref graphDef);
            this.graphDef = graphDef;
            var graphDrain = this.graphDrain;
            SetFontDataSingle(ref graphDrain);
            this.graphDrain = graphDrain;
            var graphHaste = this.graphHaste;
            SetFontDataSingle(ref graphHaste);
            this.graphHaste = graphHaste;
            var graphHp = graphHP;
            SetFontDataSingle(ref graphHp);
            graphHP = graphHp;
            var graphMaxEnd = this.graphMaxEnd;
            SetFontDataSingle(ref graphMaxEnd);
            this.graphMaxEnd = graphMaxEnd;
            var graphMovement = this.graphMovement;
            SetFontDataSingle(ref graphMovement);
            this.graphMovement = graphMovement;
            var graphRec = this.graphRec;
            SetFontDataSingle(ref graphRec);
            this.graphRec = graphRec;
            var graphRegen = this.graphRegen;
            SetFontDataSingle(ref graphRegen);
            this.graphRegen = graphRegen;
            var graphRes = this.graphRes;
            SetFontDataSingle(ref graphRes);
            this.graphRes = graphRes;
            var graphStealth = this.graphStealth;
            SetFontDataSingle(ref graphStealth);
            this.graphStealth = graphStealth;
            var graphToHit = this.graphToHit;
            SetFontDataSingle(ref graphToHit);
            this.graphToHit = graphToHit;
            var graphEndRdx = this.graphEndRdx;
            SetFontDataSingle(ref graphEndRdx);
            this.graphEndRdx = graphEndRdx;
            var graphThreat = this.graphThreat;
            SetFontDataSingle(ref graphThreat);
            this.graphThreat = graphThreat;
            var graphSprot = graphSProt;
            SetFontDataSingle(ref graphSprot);
            graphSProt = graphSprot;
            var graphSres = graphSRes;
            SetFontDataSingle(ref graphSres);
            graphSRes = graphSres;
            var graphSdeb = graphSDeb;
            SetFontDataSingle(ref graphSdeb);
            graphSDeb = graphSdeb;
            lblDef.Font = this.graphDef.Font;
            lblMisc.Font = this.graphDef.Font;
            lblMovement.Font = this.graphDef.Font;
            lblRegenRec.Font = this.graphDef.Font;
            lblRes.Font = this.graphDef.Font;
            lblStealth.Font = this.graphDef.Font;
            lblSRes.Font = this.graphDef.Font;
            lblSProt.Font = this.graphDef.Font;
            lblSDeb.Font = this.graphDef.Font;
        }

        public void SetLocation()
        {
            var rectangle = new Rectangle();
            pnlMisc.Left = pnlDRHE.Left;
            pnlStatus.Left = pnlDRHE.Left;
            Width = Width - ClientSize.Width + pnlDRHE.Left * 2 + 320;
            rectangle.X = MainModule.MidsController.SzFrmTotals.X;
            rectangle.Y = MainModule.MidsController.SzFrmTotals.Y;
            rectangle.Width = MainModule.MidsController.SzFrmTotals.Width;
            rectangle.Height = MainModule.MidsController.SzFrmTotals.Height;
            if (rectangle.Width < 1)
                rectangle.Width = Width;
            if (rectangle.Height < 1)
                rectangle.Height = Height;
            if (rectangle.Width < MinimumSize.Width)
                rectangle.Width = MinimumSize.Width;
            if (rectangle.Height < MinimumSize.Height)
                rectangle.Height = MinimumSize.Height;
            if (rectangle.X < 1)
                rectangle.X = _myParent.Left + 8;
            if (rectangle.Y < 32)
                rectangle.Y = _myParent.Top + (_myParent.Height - _myParent.ClientSize.Height) +
                              _myParent.GetPrimaryBottom();
            Top = rectangle.Y;
            Left = rectangle.X;
            Height = rectangle.Height;
            Width = rectangle.Width;
            _loaded = true;
            FrmTotalsResize(this, new EventArgs());
        }

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
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
                return;
            var index = 2;
            if (iState)
                index = 3;
            var rectangle = new Rectangle(0, 0, _myParent.Drawing.bxPower[index].Size.Width,
                _myParent.Drawing.bxPower[index].Size.Height);
            var destRect = new Rectangle(0, 0, iTab.Width, iTab.Height);
            using var stringFormat = new StringFormat();
            using var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            if (index == 3)
                extendedBitmap.Graphics.DrawImage(_myParent.Drawing.bxPower[index].Bitmap, destRect, 0, 0,
                    rectangle.Width, rectangle.Height,
                    GraphicsUnit.Pixel);
            else
                extendedBitmap.Graphics.DrawImage(
                    MidsContext.Character.IsHero()
                        ? _myParent.Drawing.bxPower[2].Bitmap
                        : _myParent.Drawing.bxPower[4].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                    GraphicsUnit.Pixel, _myParent.Drawing.pImageAttributes);
            var height = bFont.GetHeight(e.Graphics) + 2f;
            var Bounds = new RectangleF(0.0f, (float) ((tab0.Height - (double) height) / 2.0), tab0.Width, height);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iString, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1f,
                graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        public void UpdateData()
        {
            var names1 = Enum.GetNames(Enums.eDamage.None.GetType());
            pbClose.Refresh();
            pbTopMost.Refresh();
            tab0.Refresh();
            tab1.Refresh();
            var displayStats = MidsContext.Character.DisplayStats;
            graphDef.Clear();
            var num1 = names1.Length - 1;
            for (var dType = 1; dType <= num1; ++dType)
            {
                if (!((dType != 9) & (dType != 7)))
                    continue;
                var iTip = Strings.Format(displayStats.Defense(dType), "##0.##") + "% " + names1[dType] + " defense";
                graphDef.AddItem(names1[dType] + ":|" + Strings.Format(displayStats.Defense(dType), "##0.##") + "%",
                    displayStats.Defense(dType), 0.0f, iTip);
            }

            graphDef.Max = 100f;
            graphDef.Draw();
            var str1 = MidsContext.Character.Archetype.DisplayName + " resistance cap: " +
                       Strings.Format((float) (MidsContext.Character.Archetype.ResCap * 100.0), "###0") + "%";
            graphRes.Clear();
            var dType1 = 1;
            do
            {
                if (dType1 != 9)
                {
                    string iTip;
                    if (MidsContext.Character.TotalsCapped.Res[dType1] <
                        (double) MidsContext.Character.Totals.Res[dType1])
                        iTip = Strings.Format(displayStats.DamageResistance(dType1, true), "##0.##") + "% " +
                               names1[dType1] +
                               " resistance capped at " +
                               Strings.Format(displayStats.DamageResistance(dType1, false), "##0.##") + "%";
                    else
                        iTip = Strings.Format(displayStats.DamageResistance(dType1, true), "##0.##") + "% " +
                               names1[dType1] +
                               " resistance. (" + str1 + ")";
                    graphRes.AddItem(
                        names1[dType1] + ":|" + Strings.Format(displayStats.DamageResistance(dType1, false), "##0.##") +
                        "%",
                        displayStats.DamageResistance(dType1, false), displayStats.DamageResistance(dType1, true),
                        iTip);
                }

                ++dType1;
            } while (dType1 <= 9);

            graphRes.Max = 100f;
            graphRes.Draw();
            var iTip1 = "";
            var str2 = "Time to go from 0-100% end: " + Utilities.FixDP(displayStats.EnduranceTimeToFull) + "s.";
            if (Math.Abs(displayStats.EnduranceRecoveryPercentage(false) -
                         displayStats.EnduranceRecoveryPercentage(true)) > 0.01)
                str2 = str2 + "\r\nCapped from a total of: " +
                       Strings.Format(displayStats.EnduranceRecoveryPercentage(true), "###0") + "%.";
            var iTip2 = str2 + "\r\nHover the mouse over the End Drain stats for more info.";
            if (displayStats.EnduranceRecoveryNet > 0.0)
            {
                iTip1 = "Net Endurance Gain (Recovery - Drain): " + Utilities.FixDP(displayStats.EnduranceRecoveryNet) +
                        "/s.";
                if (Math.Abs(displayStats.EnduranceRecoveryNet - displayStats.EnduranceRecoveryNumeric) > 0.01)
                    iTip1 = iTip1 + "\r\nTime to go from 0-100% end (using net gain): " +
                            Utilities.FixDP(displayStats.EnduranceTimeToFullNet) + "s.";
            }
            else if (displayStats.EnduranceRecoveryNet < 0.0)
            {
                iTip1 = "With current end drain, you will lose end at a rate of: " +
                        Utilities.FixDP(displayStats.EnduranceRecoveryLossNet) +
                        "/s.\r\nFrom 100% you would run out of end in: " +
                        Utilities.FixDP(displayStats.EnduranceTimeToZero) + "s.";
            }

            graphMaxEnd.Clear();
            var iTip3 = "Base Endurance: 100\r\nCurrent Max End: " + Utilities.FixDP(displayStats.EnduranceMaxEnd);
            if (MidsContext.Character.Totals.EndMax > 0.0)
                iTip3 = iTip3 + "\r\nYour maximum endurance has been increased by " +
                        Utilities.FixDP(displayStats.EnduranceMaxEnd - 100f) +
                        "%";
            graphMaxEnd.AddItem("Max End:|" + Utilities.FixDP(displayStats.EnduranceMaxEnd) + "%",
                displayStats.EnduranceMaxEnd, 0.0f, iTip3);
            graphMaxEnd.Max = 150f;
            graphMaxEnd.MarkerValue = 100f;
            graphMaxEnd.Draw();
            graphDrain.Clear();
            graphDrain.AddItem("EndUse:|" + Strings.Format(MidsContext.Character.Totals.EndUse, "##0.##") + "/s",
                MidsContext.Character.Totals.EndUse, MidsContext.Character.Totals.EndUse, iTip1);
            graphDrain.Max = 4f;
            graphDrain.Draw();
            graphRec.Clear();
            graphRec.AddItem(
                "EndRec:|" + Strings.Format(displayStats.EnduranceRecoveryPercentage(false), "###0") + "% (" +
                Strings.Format(displayStats.EnduranceRecoveryNumeric, "##0.##") + "/s)",
                displayStats.EnduranceRecoveryPercentage(false),
                displayStats.EnduranceRecoveryPercentage(true), iTip2);
            graphRec.Max = 400f;
            graphRec.MarkerValue = 100f;
            graphRec.Draw();
            var iTip4 = "Time to go from 0-100% health: " + Utilities.FixDP(displayStats.HealthRegenTimeToFull) +
                        "s.\r\nHealth regenerated per second: " +
                        Utilities.FixDP(displayStats.HealthRegenHealthPerSec) +
                        "%\r\nHitPoints regenerated per second at level 50: " +
                        Utilities.FixDP(displayStats.HealthRegenHPPerSec) + " HP";
            if (Math.Abs(displayStats.HealthRegenPercent(false) - displayStats.HealthRegenPercent(true)) > 0.01)
                iTip4 = iTip4 + "\r\nCapped from a total of: " +
                        Strings.Format(displayStats.HealthRegenPercent(true), "###0") + "%.";
            graphRegen.Clear();
            graphRegen.AddItem("Regeneration:|" + Strings.Format(displayStats.HealthRegenPercent(false), "###0") + "%",
                displayStats.HealthRegenPercent(false), displayStats.HealthRegenPercent(true), iTip4);
            graphRegen.Max = graphRegen.GetMaxValue();
            graphRegen.MarkerValue = 100f;
            graphRegen.Draw();
            graphHP.Clear();
            var iTip5 = "Base HitPoints: " + Convert.ToString(MidsContext.Character.Archetype.Hitpoints) +
                        "\r\nCurrent HitPoints: " +
                        Convert.ToString(displayStats.HealthHitpointsNumeric(false), CultureInfo.InvariantCulture);
            if (Math.Abs(displayStats.HealthHitpointsNumeric(false) - displayStats.HealthHitpointsNumeric(true)) > 0.01)
                iTip5 = iTip5 + "\r\n(Capped from a total of: " +
                        Strings.Format(displayStats.HealthHitpointsNumeric(true), "###0.##") + ")";
            graphHP.AddItem("Max HP:|" + Strings.Format(displayStats.HealthHitpointsPercentage, "###0.##") + "%",
                displayStats.HealthHitpointsPercentage, displayStats.HealthHitpointsPercentage, iTip5);
            graphHP.Max = (float) (MidsContext.Character.Archetype.HPCap /
                (double) MidsContext.Character.Archetype.Hitpoints * 100.0);
            graphHP.MarkerValue = 100f;
            graphHP.Draw();
            graphMovement.Clear();
            var speedFormat = MidsContext.Config.SpeedFormat;
            var rateDisp = " MPH";
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

            string formatSpeed(float iSpeed)
            {
                return Strings.Format(displayStats.Speed(iSpeed, speedFormat), "##0.##") + rateDisp + ".";
            }

            string formatDistance(float iSpeed)
            {
                return Strings.Format(displayStats.Distance(iSpeed, speedFormat), "##0.##");
            }

            var strCap = "This has been capped at the maximum in-game speed.\r\nUncapped speed: ";
            var fltTip = "Base Flight Speed: " + formatSpeed(31.5f);
            var jmpTip2 = "Base Jump Height: " + formatDistance(4f);
            var iTip8 = "Base Run Speed: " + formatSpeed(21f);

            if (A_GT_B(displayStats.MovementFlySpeed(speedFormat, true),
                displayStats.MovementFlySpeed(speedFormat, false)))
                fltTip = fltTip + "\r\n" + strCap +
                         Strings.Format(displayStats.Speed(MidsContext.Character.Totals.FlySpd, speedFormat),
                             "##0.##") + rateDisp + ".";
            else if (Math.Abs(displayStats.MovementFlySpeed(speedFormat, false)) < float.Epsilon)
                fltTip += "\r\nYou have no active flight powers.";
            var jumpTip = "Base Jump Speed: " + formatSpeed(21f);
            if (A_GT_B(displayStats.MovementJumpSpeed(speedFormat, true),
                displayStats.MovementJumpSpeed(speedFormat, false)))
                jumpTip = jumpTip + "\r\n" + strCap + formatSpeed(MidsContext.Character.Totals.JumpSpd);
            if (A_GT_B(displayStats.MovementRunSpeed(speedFormat, true),
                displayStats.MovementRunSpeed(speedFormat, false)))
                iTip8 = iTip8 + "\r\n" + strCap + formatSpeed(MidsContext.Character.Totals.RunSpd);
            var jmpHtTip = !((speedFormat == Enums.eSpeedMeasure.FeetPerSecond) |
                             (speedFormat == Enums.eSpeedMeasure.MilesPerHour))
                ? jmpTip2 + " m."
                : jmpTip2 + " ft.";

            void AddGrphMovement(string title, Func<Enums.eSpeedMeasure, bool, float> dispStatsF, string tip)
            {
                graphMovement.AddItem(title + Strings.Format(dispStatsF(speedFormat, false), "##0.##") + rateDisp,
                    dispStatsF(Enums.eSpeedMeasure.FeetPerSecond, false),
                    dispStatsF(Enums.eSpeedMeasure.FeetPerSecond, true), tip);
            }

            AddGrphMovement("Run:|", displayStats.MovementRunSpeed, iTip8);
            AddGrphMovement("Jump:|", displayStats.MovementJumpSpeed, jumpTip);
            //this.graphMovement.AddItem("Run:|" + Strings.Format(displayStats.MovementRunSpeed(speedFormat, false), "##0.##") + rateDisp, displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, false), displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, true), iTip8);
            //this.graphMovement.AddItem("Jump:|" + Strings.Format(displayStats.MovementJumpSpeed(speedFormat, false), "##0.##") + rateDisp, displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, false), displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, true), jumpTip);
            graphMovement.AddItem(
                "Jump Height:|" + Strings.Format(displayStats.MovementJumpHeight(speedFormat), "##0.##") + lengthDisp,
                displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond),
                displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond), jmpHtTip);
            //this.graphMovement.AddItem("Fly:|" + Strings.Format(displayStats.MovementFlySpeed(speedFormat, false), "##0.##") + rateDisp, displayStats.MovementFlySpeed(Enums.eSpeedMeasure.FeetPerSecond, false), displayStats.MovementFlySpeed(Enums.eSpeedMeasure.FeetPerSecond, true), fltTip);

            //AddGrphMovement("Jump Height:|", (x,_) => displayStats.MovementJumpHeight(x), jmpHtTip);
            AddGrphMovement("Fly:|", displayStats.MovementFlySpeed, fltTip);

            graphMovement.ForcedMax = displayStats.Speed(200f, Enums.eSpeedMeasure.FeetPerSecond);
            graphMovement.Draw();
            graphToHit.Clear();
            graphToHit.AddItem("ToHit:|" + PM(displayStats.BuffToHit, "##0.##", "%"), displayStats.BuffToHit, 0.0f,
                "This effect increases the accuracy of all your powers.\r\nToHit values are added together before being multiplied by Accuracy");
            graphToHit.Max = 100f;
            graphToHit.Draw();
            graphAcc.Clear();
            graphAcc.AddItem("Accuracy:|" + PM(displayStats.BuffAccuracy, "##0.##", "%"), displayStats.BuffAccuracy,
                0.0f,
                "This effect increases the accuracy of all your powers.\r\nAccuracy buffs are usually applied as invention set bonuses.");
            graphAcc.Max = 100f;
            graphAcc.Draw();
            graphDam.Clear();
            var str7 = "";
            if (A_GT_B(displayStats.BuffDamage(true), displayStats.BuffDamage(false)))
                str7 = "\r\n\r\nDamage Capped from " +
                       Convert.ToString(displayStats.BuffDamage(true), CultureInfo.InvariantCulture) + "% to " +
                       Convert.ToString(displayStats.BuffDamage(false), CultureInfo.InvariantCulture) + "%";
            graphDam.AddItem("Damage:|" + PM(displayStats.BuffDamage(false) - 100f, "##0.##", "%"),
                displayStats.BuffDamage(false),
                displayStats.BuffDamage(true),
                "This effect alters the damage dealt by all your attacks.\r\nAs some powers can reduce your damage output, this bar has your base damage (100%) included." +
                str7);
            graphDam.Max = MidsContext.Character.Archetype.DamageCap * 100f;
            graphDam.MarkerValue = 100f;
            graphDam.Draw();
            graphHaste.Clear();
            var str8 = "";
            if (A_GT_B(displayStats.BuffHaste(true), displayStats.BuffHaste(false)))
                str8 = "\r\n\r\nRecharge Speed Capped from " +
                       Convert.ToString(displayStats.BuffHaste(true), CultureInfo.InvariantCulture) + "% to " +
                       Convert.ToString(displayStats.BuffHaste(false), CultureInfo.InvariantCulture) + "%";
            graphHaste.AddItem("Haste:|" + PM(displayStats.BuffHaste(false) - 100f, "##0.##", "%"),
                displayStats.BuffHaste(false),
                displayStats.BuffHaste(true),
                "This effect alters the recharge speed of all your powers.\r\nThe higher the value, the faster the recharge.\r\nAs some powers can slow your recharge, this bar starts with your base recharge (100%) included." +
                str8);
            graphHaste.MarkerValue = 100f;
            var haste = displayStats.BuffHaste(true);
            graphHaste.Max = haste <= 380.0 ? haste <= 280.0 ? 300f : 400f : 500f;
            graphHaste.Draw();
            graphEndRdx.Clear();
            graphEndRdx.AddItem("EndRdx:|" + PM(displayStats.BuffEndRdx, "##0.##", "%"), displayStats.BuffEndRdx,
                displayStats.BuffEndRdx,
                "This effect is applied to powers in addition to endurance reduction enhancements.");
            graphEndRdx.Max = 200f;
            graphEndRdx.Draw();
            graphStealth.Clear();
            graphStealth.AddItem("PvE:|" + Strings.Format(MidsContext.Character.Totals.StealthPvE, "##0") + " ft",
                MidsContext.Character.Totals.StealthPvE, 0.0f,
                "This is subtracted from a critter's perception to work out if they can see you.");
            graphStealth.AddItem("PvP:|" + Strings.Format(MidsContext.Character.Totals.StealthPvP, "##0") + " ft",
                MidsContext.Character.Totals.StealthPvP, 0.0f,
                "This is subtracted from a player's perception to work out if they can see you.");
            graphStealth.AddItem("Perception:|" + Strings.Format(displayStats.Perception(false), "###0") + " ft",
                displayStats.Perception(false), 0.0f,
                "This, minus a player's stealth radius, is the distance you can see it.");
            graphStealth.Max = graphStealth.GetMaxValue() * 1.01f;
            graphStealth.Draw();
            var iTip10 =
                "This affects how critters prioritise you as a threat.\r\nLower values make you a less tempting target.\r\nThe " +
                MidsContext.Character.Archetype.DisplayName + " base Threat Level of " +
                Strings.Format((float) (MidsContext.Character.Archetype.BaseThreat * 100.0), "##0") +
                "% is included in this figure.";
            var nBase = displayStats.ThreatLevel + 200f;
            graphThreat.Clear();
            graphThreat.AddItem("Threat Level:|" + Strings.Format(displayStats.ThreatLevel, "##0") + "%", nBase, 0.0f,
                iTip10);
            graphThreat.MarkerValue = (float) (MidsContext.Character.Archetype.BaseThreat * 100.0 + 200.0);
            graphThreat.Max = 800f;
            graphThreat.Draw();
            graphElusivity.Clear();
            graphElusivity.AddItem(
                "Elusivity:|" + Strings.Format((float) (MidsContext.Character.Totals.Elusivity * 100.0), "##0.##") +
                "%",
                MidsContext.Character.Totals.Elusivity * 100f, 0.0f,
                "This effect resists accuracy buffs of enemies attacking you.");
            graphElusivity.Max = 100f;
            graphElusivity.Draw();
            if (Math.Abs(graphAcc.Font.Size - (double) MidsContext.Config.RtFont.PairedBase) > float.Epsilon)
                SetFonts();
            var totals = MidsContext.Character.Totals;
            var str9 = "\r\nStatus protection prevents you being affected by a status effect such as" +
                       "\r\na Hold until the magnitude of the effect exceeds that of the protection.";
            var str10 = "\r\nStatus resistance reduces the time you are affected by a status effect such as" +
                        "\r\na Hold. Note that 100% resistance would make a 10s effect last 5s, and not 0s.";
            graphSProt.Clear();
            graphSRes.Clear();
            Enums.eMez[] eMezArray =
            {
                Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep, Enums.eMez.Immobilized, Enums.eMez.Knockback,
                Enums.eMez.Repel,
                Enums.eMez.Confused, Enums.eMez.Terrorized, Enums.eMez.Taunt, Enums.eMez.Placate, Enums.eMez.Teleport
            };
            var names2 = Enum.GetNames(eMezArray[0].GetType());
            var names3 = Enum.GetNames(eMezArray[0].GetType());
            names2[2] = "Hold";
            names2[3] = "Immob";
            names2[1] = "Confuse";
            names2[12] = "Fear";
            names3[2] = "Hold";
            names3[1] = "Confuse";
            names3[12] = "Fear (Terrorized)";
            names3[4] = "Knockback and Knockup";
            var num3 = 5;
            var num4 = eMezArray.Length - 1;
            for (var index = 0; index <= num4; ++index)
            {
                string iTip11;
                if (Math.Abs(totals.Mez[(int) eMezArray[index]]) < float.Epsilon)
                    iTip11 = "You have no protection from " + names3[(int) eMezArray[index]] + " effects.\r\n" + str9;
                else
                    iTip11 = "You have mag " +
                             Strings.Format((float) -(double) totals.Mez[(int) eMezArray[index]], "##0.##") +
                             " protection from " + names3[(int) eMezArray[index]] + " effects.\r\n" + str9;
                graphSProt.AddItem(
                    names2[(int) eMezArray[index]] + ":|" +
                    Strings.Format(-totals.Mez[(int) eMezArray[index]], "##0.##"),
                    -totals.Mez[(int) eMezArray[index]], 0.0f, iTip11);
                var num5 = (float) (100.0 / (1.0 + totals.MezRes[(int) eMezArray[index]] / 100.0));
                string str11;
                if ((eMezArray[index] != Enums.eMez.Knockback) & (eMezArray[index] != Enums.eMez.Knockup) &
                    (eMezArray[index] != Enums.eMez.Repel) &
                    (eMezArray[index] != Enums.eMez.Teleport))
                {
                    if (totals.MezRes[(int) eMezArray[index]] > (double) num3)
                        num3 = (int) Math.Round(totals.MezRes[(int) eMezArray[index]]);
                    str11 = "\r\n" + names3[(int) eMezArray[index]] + " effects will last " +
                            Strings.Format(num5, "##0.##") +
                            "% of their full duration.\r\n" + str10;
                }
                else if (eMezArray[index] == Enums.eMez.Teleport)
                {
                    str11 = "\r\n" + names3[(int) eMezArray[index]] + " effects will be resisted.\r\n" + str10;
                }
                else
                {
                    str11 = "\r\n" + names3[(int) eMezArray[index]] + " effects will have " +
                            Strings.Format(num5, "##0.##") +
                            "% of their full effect.\r\n" + str10;
                }

                string iTip12;
                if (Math.Abs(totals.MezRes[(int) eMezArray[index]]) < float.Epsilon)
                    iTip12 = "You have no resistance to " + names3[(int) eMezArray[index]] + " effects.\r\n" + str10;
                else
                    iTip12 = "You have " + Strings.Format(totals.MezRes[(int) eMezArray[index]], "##0.##") +
                             "% resistance to " +
                             names3[(int) eMezArray[index]] + " effects." + str11;
                graphSRes.AddItem(
                    names2[(int) eMezArray[index]] + ":|" +
                    Strings.Format(totals.MezRes[(int) eMezArray[index]], "##0.##") + "%",
                    totals.MezRes[(int) eMezArray[index]], 0.0f, iTip12);
            }

            graphSProt.Max = graphSProt.GetMaxValue();
            graphSProt.Draw();
            graphSRes.Max = num3;
            graphSRes.Draw();
            graphSDeb.Clear();
            Enums.eEffectType[] eEffectTypeArray =
            {
                Enums.eEffectType.Defense, Enums.eEffectType.Endurance, Enums.eEffectType.Recovery,
                Enums.eEffectType.PerceptionRadius,
                Enums.eEffectType.ToHit, Enums.eEffectType.RechargeTime, Enums.eEffectType.SpeedRunning
            };
            var num6 = eEffectTypeArray.Length - 1;
            for (var index = 0; index <= num6; ++index)
            {
                string iTip11;
                if (Math.Abs(totals.DebuffRes[(int) eEffectTypeArray[index]] - 0.0f) < 0.001)
                    iTip11 = "You have no resistance to " + Enums.GetEffectName(eEffectTypeArray[index]) + " debuffs.";
                else
                    iTip11 = "You have " + Strings.Format(totals.DebuffRes[(int) eEffectTypeArray[index]], "##0.##") +
                             "% resistance to " +
                             Enums.GetEffectName(eEffectTypeArray[index]) + " debuffs.";
                graphSDeb.AddItem(
                    Enums.GetEffectName(eEffectTypeArray[index]) + ":|" +
                    Strings.Format(totals.DebuffRes[(int) eEffectTypeArray[index]], "##0.##") + "%",
                    totals.DebuffRes[(int) eEffectTypeArray[index]], 0.0f, iTip11);
            }

            graphSDeb.Max = graphSDeb.GetMaxValue() + 1f;
            graphSDeb.Draw();
        }
    }
}
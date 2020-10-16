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
        private class LayeredBar
        {
            private Panel? BaseVal;
            private Panel? OverCap;
            private Panel Bar;
            private Panel? Overlay1;
            private Panel? Overlay2;
            private Label Value;
            private int MaxWidth;

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;

                BaseVal = null;
                OverCap = null;
                Overlay1 = null;
                Overlay2 = null;
            }

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth,
                ref Panel overCap)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;
                OverCap = overCap;

                BaseVal = null;
                Overlay1 = null;
                Overlay2 = null;
            }

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth, int dummmy,
                ref Panel baseVal)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;
                BaseVal = baseVal;

                OverCap = null;
                Overlay1 = null;
                Overlay2 = null;
            }

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth,
                ref Panel baseVal, ref Panel overCap)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;
                BaseVal = baseVal;
                OverCap = overCap;

                Overlay1 = null;
                Overlay2 = null;
            }

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth,
                ref Panel baseVal, ref Panel overCap, ref Panel overlay1)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;
                BaseVal = baseVal;
                OverCap = overCap;
                Overlay1 = overlay1;

                Overlay2 = null;
            }

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth,
                ref Panel baseVal, ref Panel overCap, ref Panel overlay1, ref Panel overlay2)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;
                BaseVal = baseVal;
                OverCap = overCap;
                Overlay1 = overlay1;
                Overlay2 = overlay2;
            }
        }

        private readonly frmMain _myParent;
        private bool _keepOnTop;
        private LayeredBar[] Bars;
        private readonly int BarMaxWidth = 277;

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
                    var barList = controls.OfType<Panel>().ToList();
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

        private void InitBars()
        {
            
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

            // Tab Foreground Colors don't stick in the designer.
            // Note: default colors will be used anyway.
            // This may only cause issues if a custom
            // Windows theme is in use.
            tabControlAdv1.ActiveTabForeColor = Color.White;
            tabControlAdv1.InActiveTabForeColor = Color.Black;

            InitBars();
            SetTabPanelColorScheme();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            CenterToParent();
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
    }
}
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

        public Control StatControl(string tab, int panel, string type, int control)
        {
            Regex regEx = new Regex(@"^\d+");
            TabPageAdv page = tabControlAdv2.Controls.OfType<TabPageAdv>().First(t => t.Text.Contains(tab));
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

        private void SetTabPanelColorScheme()
        {
            if (MidsContext.Character.IsHero())
            {
                tabControlAdv2.InactiveTabColor = Color.DodgerBlue;
                tabControlAdv2.TabPanelBackColor = Color.DodgerBlue;
                tabControlAdv2.FixedSingleBorderColor = Color.Goldenrod;
                tabControlAdv2.ActiveTabColor = Color.Goldenrod;
            }
            else
            {
                tabControlAdv2.InactiveTabColor = Color.FromArgb(193, 23, 23);
                tabControlAdv2.TabPanelBackColor = Color.FromArgb(193, 23, 23);
                tabControlAdv2.FixedSingleBorderColor = Color.FromArgb(198, 128, 29);
                tabControlAdv2.ActiveTabColor = Color.FromArgb(198, 128, 29);
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
            tabControlAdv2.ActiveTabForeColor = Color.White;
            tabControlAdv2.InActiveTabForeColor = Color.Black;
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
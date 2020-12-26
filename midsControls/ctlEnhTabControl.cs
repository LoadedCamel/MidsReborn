using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;

namespace mrbControls
{
    public sealed partial class ctlEnhTabControl : TabControlAdv
    {
        [Description("Enables Tab Text Outlinning")]
        [Category("Tab Enhancements")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableOutline { get; set; }

        [Description("Active Tab Outline Color")]
        [Category("Tab Enhancements")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ActiveTabOutlineColor { get; set; }

        [Description("Active Tab Outline Width")]
        [Category("Tab Enhancements")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float ActiveTabOutlineWidth { get; set; }

        [Description("Inactive Tab Outline Color")]
        [Category("Tab Enhancements")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color InactiveTabOutlineColor { get; set; }

        [Description("Inactive Tab Outline Width")]
        [Category("Tab Enhancements")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float InactiveTabOutlineWidth { get; set; }

        public ctlEnhTabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            DrawItem += DrawItems;
        }

        private static float NewFontSize(Graphics graphics, Size size, Font font, string str)
        {
            SizeF stringSize = graphics.MeasureString(str, font);
            float wRatio = size.Width / stringSize.Width;
            float hRatio = size.Height / stringSize.Height;
            float ratio = Math.Max(hRatio, wRatio);
            return font.Size * ratio;
        }

        private void DrawItems(object sender, DrawTabEventArgs drawItemInfo)
        {
            using var gp = new GraphicsPath();
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            drawItemInfo.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            drawItemInfo.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            float fontSize;
            Rectangle interiorBounds;
            Rectangle tabBounds;

            if (drawItemInfo.State == DrawItemState.Selected)
            {
                using Pen outline = new Pen(ActiveTabOutlineColor, ActiveTabOutlineWidth) { LineJoin = LineJoin.Round };
                using Brush foreBrush = new SolidBrush(SelectedTab.ForeColor);
                Text = SelectedTab.Text;
                interiorBounds = drawItemInfo.BoundsInterior;
                interiorBounds.Width += TabGap;
                fontSize = NewFontSize(drawItemInfo.Graphics, drawItemInfo.BoundsInterior.Size, ActiveTabFont, Text);
                tabBounds = drawItemInfo.Bounds;
                tabBounds.Width += TabGap;
                drawItemInfo.Graphics.FillRectangle(new SolidBrush(ActiveTabColor), tabBounds);
                gp.AddString(Text, ActiveTabFont.FontFamily, (int)ActiveTabFont.Style, fontSize * 1.1f, interiorBounds, sf);
                if (EnableOutline)
                    drawItemInfo.Graphics.DrawPath(outline, gp);
                drawItemInfo.Graphics.FillPath(foreBrush, gp);
            }
            else
            {
                using Pen outline = new Pen(InactiveTabOutlineColor, InactiveTabOutlineWidth) { LineJoin = LineJoin.Round };
                using Brush foreBrush = new SolidBrush(TabPages[drawItemInfo.Index].ForeColor);
                Text = TabPages[drawItemInfo.Index].Text;
                interiorBounds = drawItemInfo.BoundsInterior;
                interiorBounds.Width += TabGap;
                fontSize = NewFontSize(drawItemInfo.Graphics, drawItemInfo.BoundsInterior.Size, Font, Text);
                tabBounds = drawItemInfo.Bounds;
                tabBounds.Width += TabGap;
                drawItemInfo.Graphics.FillRectangle(new SolidBrush(InactiveTabColor), tabBounds);
                gp.AddString(Text, Font.FontFamily, (int)Font.Style, fontSize * 1.1f, interiorBounds, sf);
                if (EnableOutline)
                    drawItemInfo.Graphics.DrawPath(outline, gp);
                drawItemInfo.Graphics.FillPath(foreBrush, gp);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core;

namespace Mids_Reborn.UIv2.v2Controls
{
    public partial class PowerControl : UserControl
    {
        [DllImport("uxtheme", ExactSpelling = true)]
        private static extern int DrawThemeParentBackground(IntPtr hWnd, IntPtr hdc, ref Rectangle pRect);

        private readonly List<string> _images = I9Gfx.LoadButtons().GetAwaiter().GetResult();

        public enum ButtonType
        {
            EmptyAlt = 0,
            Empty = 1,
            Hero = 2,
            HeroHover = 3,
            Villain = 4,
            VillainHover = 5
        }

        public ButtonType CurrentImage { get; set; }
        public bool Selected { get; set; }

        public PowerControl()
        {
            InitializeComponent();
        }

        private Image ButtonImage(ButtonType type) => Image.FromFile(_images[(int)type]);

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var rec = ClientRectangle;

            var hdc = g.GetHdc();
            DrawThemeParentBackground(Handle, hdc, ref rec);
            g.ReleaseHdc(hdc);


            var powerRect = new Rectangle(0, 0, Width, Height);
            var image = ButtonImage(CurrentImage);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.DrawImage(image, powerRect, 0,0, image.Width, image.Height, GraphicsUnit.Pixel, new ImageAttributes());
            var outlinePen = new Pen(Color.Black, 3) { LineJoin = LineJoin.Round };
            var brush = new SolidBrush(ForeColor);
            var sFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            var gfxPath = new GraphicsPath();
            var textRect = new Rectangle(0, -8, Width, Height);
            gfxPath.AddString(Text, Font.FontFamily, (int)Font.Style, Font.Size, textRect, sFormat);
            outlinePen.LineJoin = LineJoin.Round;
            e.Graphics.DrawPath(outlinePen, gfxPath);
            e.Graphics.FillPath(brush, gfxPath);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }
    }
}

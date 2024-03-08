using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Mids_Reborn.Core;

namespace Mids_Reborn.UIv2.Controls
{
    public partial class PowerControl2 : UserControl
    {
        [DllImport("uxtheme", ExactSpelling = true)]
        private static extern int DrawThemeParentBackground(IntPtr hWnd, IntPtr hdc, ref Rectangle pRect);

        private List<string?> _images = new();

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

        public PowerControl2()
        {
            LoadButtons();
            InitializeComponent();
        }

        private Image ButtonImage(ButtonType type) => Image.FromFile(_images[(int)type] ?? throw new InvalidOperationException());

        private async void LoadButtons()
        {
            _images = await I9Gfx.LoadButtons();
        }

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
            g.DrawImage(image, powerRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, new ImageAttributes());

            // Calculate font size based on control's size
            float fontSize = Math.Min(Width, Height) / 10f; 
            fontSize = Math.Max(fontSize, 10); 

            // Create font with dynamic size
            using Font dynamicFont = new Font(Font.FontFamily, fontSize, FontStyle.Bold);

            // Create outline pen, brush, and format
            var outlinePen = new Pen(Color.Black, 3) { LineJoin = LineJoin.Round };
            var brush = new SolidBrush(ForeColor);
            var sFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };

            // Use GraphicsPath for text
            var gfxPath = new GraphicsPath();
            var textRect = new Rectangle(0, -8, Width, Height);
            gfxPath.AddString(Text, dynamicFont.FontFamily, (int)dynamicFont.Style, e.Graphics.DpiY * dynamicFont.SizeInPoints / 72, textRect, sFormat);
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

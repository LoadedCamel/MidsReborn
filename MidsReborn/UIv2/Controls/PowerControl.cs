using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mids_Reborn.UIv2.Controls
{
    public partial class PowerControl : UserControl
    {
        [DllImport("uxtheme", ExactSpelling = true)]
        private static extern int DrawThemeParentBackground(IntPtr hWnd, IntPtr hdc, ref Rectangle pRect);


        public Image PowerImage { get; set; }
        public List<Image> SlotEnhancements { get; private set; } = new List<Image>();
        public int SlotCount { get; set; } = 0; // Dynamically set based on the power
        private const int MaxSlots = 6;
        private int _baseSlotSize = 32; // Base size for slots
        private int _slotSize; // Actual slot size, adjusted dynamically
        private int _padding; // Dynamic padding, adjusted dynamically

        // Properties for texts
        public string LevelSelected { get; set; } = "(1)";
        public string PowerName { get; set; } = "Power Name";


        public PowerControl()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            var rec = ClientRectangle;

            var hdc = g.GetHdc();
            _ = DrawThemeParentBackground(Handle, hdc, ref rec);
            g.ReleaseHdc(hdc);

            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            DrawPower(e.Graphics);
        }

        private void DrawPower(Graphics graphics)
        {
            // Adjust the drawing based on current sizes and padding
            if (PowerImage != null)
            {
                graphics.DrawImage(PowerImage, new Rectangle(_padding, _padding, Width - 2 * _padding, Height - 2 * _padding - _slotSize));
            }

            // Overlay texts
            DrawTexts(graphics);

            // Draw slots
            DrawSlots(graphics);
        }

        private void DrawTexts(Graphics graphics)
        {
            // Define text styles
            var topLeftTextStyle = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            var topCenterTextStyle = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

            // Calculate positions
            var topLeftPosition = new PointF(_padding * 2, _padding * 2); // Additional padding for the text itself
            var topCenterPosition = new PointF(Width / 2f, _padding * 2); // Centered, with padding from the top

            // Draw Texts
            graphics.DrawString(LevelSelected, this.Font, Brushes.Black, topLeftPosition, topLeftTextStyle);
            graphics.DrawString(PowerName, this.Font, Brushes.Black, topCenterPosition, topCenterTextStyle);
        }

        private void DrawSlots(Graphics graphics)
        {
            int slotX = _padding;
            int slotY = Height - _slotSize - _padding / 2; // Adjust to draw half outside
            for (int i = 0; i < SlotCount; i++)
            {
                Image slotImage = i < SlotEnhancements.Count ? SlotEnhancements[i] : null;
                Rectangle slotRect = new Rectangle(slotX, slotY, _slotSize, _slotSize);
                graphics.DrawEllipse(Pens.Black, slotRect);
                if (slotImage != null)
                {
                    graphics.DrawImage(slotImage, slotRect);
                }
                slotX += _slotSize + _padding; // Adjust for next slot position
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustSizes();
            Invalidate(); // Trigger repaint with new sizes
        }

        private void AdjustSizes()
        {
            // Dynamic calculation for slot size and padding based on control size
            int controlWidth = Width;
            int controlHeight = Height;

            // Example dynamic resizing logic (customize as needed):
            // Adjust slot size relative to control size, maintaining aspect ratio
            _slotSize = Math.Max(_baseSlotSize, controlWidth / 15);
            // Adjust padding based on slot size
            _padding = _slotSize / 8;

            // Ensure slot size and padding adjustments respect minimum values
            _slotSize = Math.Max(_slotSize, 20); // Minimum slot size
            _padding = Math.Max(_padding, 5); // Minimum padding
        }
    }
}

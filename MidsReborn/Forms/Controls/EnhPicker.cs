using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Forms.Controls
{
    public sealed partial class EnhPicker : UserControl
    {
        public EnhPicker()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ContainerControl | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            MinimumSize = new Size(300, 300);
            InitializeComponent();
        }

        public override Font Font { get; set; } = new(@"Microsoft Sans Serif", 10.25f, FontStyle.Bold);

        private void DrawLayout(Graphics gfx)
        {
            var client = ClientRectangle;
            using var pen = new Pen(Color.FromArgb(12,56,100), 2);
            switch (MidsContext.Character?.IsHero())
            {
                case false:
                {
                    pen.Color = Color.FromArgb(100, 0, 0);
                    break;
                }
            }

            var textMeasurement = gfx.MeasureString("Enhancement Picker", Font);
            
            var titleRect = new Rectangle(client.X + 2, client.Y - 2, client.Width - 2, (int)textMeasurement.Height + 10);
            var enhTypeRect = new Rectangle(client.X + 2, client.Y + 36, client.Width - 2, 42);
            var enhSubTypeRect = new Rectangle(client.Right - 42, enhTypeRect.Bottom, 42, client.Height - 38);
            var enhRect = new Rectangle(client.X + 2, enhTypeRect.Bottom, client.Width - 42, client.Height - 38);
            var descriptionRect = new Rectangle(client.X + 2, client.Bottom - 38, client.Width - enhSubTypeRect.Width - 2, client.Height - 2);
            var levelRect = new Rectangle(client.Right - 44, client.Bottom - 38, 42, 36);


            ControlPaint.DrawBorder(gfx, client,
                pen.Color, 2, ButtonBorderStyle.Solid,
                pen.Color, 2, ButtonBorderStyle.Solid,
                pen.Color, 2, ButtonBorderStyle.Solid,
                pen.Color, 2, ButtonBorderStyle.Solid);
            gfx.DrawRectangle(pen, titleRect);
            // gfx.DrawRectangle(pen, enhTypeRect);
            // gfx.DrawRectangle(pen, enhSubTypeRect);
            // gfx.DrawRectangle(pen, enhRect);
            // gfx.DrawRectangle(pen, descriptionRect);
            // gfx.DrawRectangle(pen, levelRect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            DrawLayout(e.Graphics);
        }
    }
}

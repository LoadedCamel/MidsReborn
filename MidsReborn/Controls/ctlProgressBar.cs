using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Mids_Reborn.Controls
{
    public partial class ctlProgressBar : ProgressBar
    {
        public ctlProgressBar()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
        }

        public string StatusText { get; set; }
        public int ItemCount { get; set; }

        protected override void OnPaint(PaintEventArgs pe)
        {
            var rect = ClientRectangle;
            var g = pe.Graphics;
            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            g.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#2C2F33")), rect);
            if (Value > 0)
            {
                var clip = new Rectangle(rect.X, rect.Y, (int) Math.Round((float) Value / (Maximum - 1) * Width), rect.Height);
                var clipGradient = new LinearGradientBrush(clip, Color.DodgerBlue, ColorTranslator.FromHtml("#404EED"), LinearGradientMode.ForwardDiagonal);
                ProgressBarRenderer.DrawHorizontalBar(g, clip);
                g.FillRectangle(clipGradient, clip);
            }

            using var f = new Font(FontFamily.GenericMonospace, 10);
            var size = g.MeasureString($"{StatusText} {Value}%", f);
            var location = new Point((int) (rect.Width / 2.0 - size.Width / 2), (int) (rect.Height / 2.0 - size.Height / 2 + 2));
            g.DrawString(string.Format($"{StatusText} {(int)Math.Round((double)(100 * Value) / Maximum + 1)}%"), f, Brushes.Azure, location);
            
            //base.OnPaint(pe);
        }
    }
}
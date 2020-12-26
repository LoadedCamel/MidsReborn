using System;
using System.Drawing;
using System.Windows.Forms;

namespace mrbControls
{
    public partial class ctlProgressBar : ProgressBar
    {
        public ctlProgressBar()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        public string StatusText { get; set; }

        protected override void OnPaint(PaintEventArgs pe)
        {
            var rect = ClientRectangle;
            var g = pe.Graphics;
            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            if (Value > 0)
            {
                var clip = new Rectangle(rect.X, rect.Y, (int) Math.Round((float) Value / Maximum * rect.Width),
                    rect.Height);
                ProgressBarRenderer.DrawHorizontalBar(g, clip);
            }

            using (var f = new Font(FontFamily.GenericMonospace, 10))
            {
                var size = g.MeasureString($"{StatusText} {Value} %", f);
                var location = new Point((int) (rect.Width / 2.0 - size.Width / 2),
                    (int) (rect.Height / 2.0 - size.Height / 2 + 2));
                g.DrawString(string.Format($"{StatusText} {Value} %"), f, Brushes.Black, location);
            }

            //base.OnPaint(pe);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace midsControls
{
    public partial class ctlProgressBar : ProgressBar
    {

        public string StatusText { get; set; }

        public ctlProgressBar()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint,true);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Rectangle rect = ClientRectangle;
            Graphics g = pe.Graphics;
            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            if (Value > 0)
            {
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round((float)Value/Maximum * rect.Width), rect.Height);
                ProgressBarRenderer.DrawHorizontalBar(g, clip);
            }

            using (Font f = new Font(FontFamily.GenericMonospace, 10))
            {
                SizeF size = g.MeasureString($"{StatusText} {Value} %", f);
                Point location = new Point((int)(rect.Width / 2.0 - size.Width / 2), (int)(rect.Height / 2.0 - size.Height / 2 + 2));
                g.DrawString(string.Format($"{StatusText} {Value} %"), f, Brushes.Black, location);
            }
            //base.OnPaint(pe);
        }
    }
}

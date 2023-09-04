using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Controls
{
    public sealed partial class LabelEx : Label
    {
        public LabelEx()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x20; // EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Paint background with underlying graphics from other controls
            base.OnPaintBackground(e);
            var g = e.Graphics;

            if (Parent == null) return;

            // Take each control in turn
            var index = Parent.Controls.GetChildIndex(this);
            for (var i = Parent.Controls.Count - 1; i > index; i--)
            {
                var c = Parent.Controls[i];

                // Check it's visible and overlaps this control
                if (!c.Bounds.IntersectsWith(Bounds) || !c.Visible) continue;

                // Load appearance of underlying control and redraw it on this background
                var bmp = new Bitmap(c.Width, c.Height, g);
                c.DrawToBitmap(bmp, c.ClientRectangle);
                g.TranslateTransform(c.Left - Left, c.Top - Top);
                g.DrawImageUnscaled(bmp, Point.Empty);
                g.TranslateTransform(Left - c.Left, Top - c.Top);
                bmp.Dispose();
            }
        }
    }
}

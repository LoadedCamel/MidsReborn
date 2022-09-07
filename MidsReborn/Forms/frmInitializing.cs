using System;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.IO_Classes;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;

namespace Mids_Reborn.Forms
{
    public partial class frmInitializing : PerPixelAlpha, IMessager
    {
        public frmInitializing()
        {
            InitializeComponent();
            SelectBitmap(Resources.MRB_Splash_Concept, 100);
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public void SetMessage(string text)
        {
            if (Label1.InvokeRequired)
            {
                void Action()
                {
                    SetMessage(text);
                }

                Invoke((Action) Action);
            }
            else
            {
                if (Label1.Text == text)
                    return;
                Label1.Text = text;
                Label1.Refresh();
                Refresh();
            }
        }
    }

    public sealed class TransparentLabel : Label
    {
        public TransparentLabel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
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
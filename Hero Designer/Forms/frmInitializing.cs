using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Base.IO_Classes;

namespace Hero_Designer.Forms
{
    public partial class frmInitializing : Form, IMessager
    {
        public frmInitializing()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            Label1.Parent = pictureBox1;
            Load += On_Load;
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

        private void On_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            Opacity = 0;
            tmrOp.Enabled = true;
            //Width = BackgroundImage.Width;
            //Height = BackgroundImage.Height;
        }

        private void tmrOp_Tick(object sender, EventArgs e)
        {
            if (Opacity < 1.0)
            {
                Opacity += 0.05;
            }
            else
            {
                tmrOp.Enabled = false;
            }
        }
    }

    // https://stackoverflow.com/questions/5522337/c-sharp-picturebox-transparent-background-doesnt-seem-to-work
    public class TransparentLabel : Label
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

    public class TransparentPictureBox : Control
    {
        private readonly Timer _refresher;
        private Image _image;

        public TransparentPictureBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            _refresher = new Timer();
            _refresher.Tick += TimerOnTick;
            _refresher.Interval = 50;
            _refresher.Enabled = true;
            _refresher.Start();
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

        protected override void OnMove(EventArgs e)
        {
            RecreateHandle();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            if (_image == null) return;

            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.DrawImage(_image, (Width / 2) - (_image.Width / 2), (Height / 2) - (_image.Height / 2));
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //Do not paint background
        }

        //Hack
        public void Redraw()
        {
            RecreateHandle();
        }

        private void TimerOnTick(object source, EventArgs e)
        {
            RecreateHandle();
            _refresher.Stop();
        }

        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                RecreateHandle();
            }
        }
    }
}
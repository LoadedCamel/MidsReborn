using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Mids_Reborn.Controls
{
    public partial class ctlAvatar : PictureBox
    {
        public ctlAvatar()
        {
            InitializeComponent();
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            var g = new GraphicsPath();
            g.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            Region = new Region(g);
            base.OnPaint(pe);
        }
    }
}
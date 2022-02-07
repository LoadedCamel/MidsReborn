using System.Windows.Forms;

namespace mrbControls
{
    public class pnlGFX : PictureBox
    {
        public pnlGFX()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
    }
}
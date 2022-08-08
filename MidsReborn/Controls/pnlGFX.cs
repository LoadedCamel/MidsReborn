using System.Windows.Forms;

namespace Mids_Reborn.Controls
{
    public class pnlGFX : PictureBox
    {
        public pnlGFX()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
    }
}
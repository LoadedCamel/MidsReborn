using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls
{
    public class PanelGfx : PictureBox
    {
        public PanelGfx()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
    }
}
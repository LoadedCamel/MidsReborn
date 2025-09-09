using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls
{
    public class PanelGfx : PictureBox
    {
        public PanelGfx()
        {
            // This setup is correct for enabling double buffering.
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
        }

        // ADD THIS METHOD
        // By overriding OnPaintBackground and leaving it empty, we prevent
        // the base class from erasing the background, which eliminates flickering.
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing.
        }
    }
}
using System.Windows.Forms;

namespace Mids_Reborn.Controls
{
    public partial class ctlPanel : Panel
    {
        public ctlPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.DoubleBuffer|ControlStyles.ContainerControl|ControlStyles.ResizeRedraw|ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }
    }
}

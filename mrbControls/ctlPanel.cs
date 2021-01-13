using System.Windows.Forms;

namespace mrbControls
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

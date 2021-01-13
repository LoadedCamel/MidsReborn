using System.Windows.Forms;

namespace mrbControls
{
    public partial class ctlTablePanel : TableLayoutPanel
    {
        public ctlTablePanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.ResizeRedraw|ControlStyles.ContainerControl,true);
            InitializeComponent();
        }
    }
}

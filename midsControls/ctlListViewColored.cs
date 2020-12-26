using System.Windows.Forms;

namespace mrbControls
{
    public partial class ctlListViewColored : ListView
    {
        public int LostFocusItem { get; set; }

        public ctlListViewColored()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            LostFocusItem = -1;
        }
    }
}

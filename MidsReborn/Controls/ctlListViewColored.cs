using System.Windows.Forms;

namespace Mids_Reborn.Controls
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

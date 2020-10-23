using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace midsControls
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

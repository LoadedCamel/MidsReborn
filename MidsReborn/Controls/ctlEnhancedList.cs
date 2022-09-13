using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Controls
{
    public partial class ctlEnhancedList : ListBox
    {

        public Color AvailableColor { get; set; }
        public Color UnavailableColor { get; set; }
        public Color SelectedColor { get; set; }
        public Color InvalidColor { get; set; }

        public ctlEnhancedList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index >= 0 && e.Index < Items.Count)
            {
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {

                }
            }
        }
    }
}

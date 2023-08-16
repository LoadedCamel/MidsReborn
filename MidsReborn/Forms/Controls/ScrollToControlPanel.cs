using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class ScrollToControlPanel : Panel
    {
        public ScrollToControlPanel()
        {
            InitializeComponent();
        }
        
        protected override Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }
    }
}

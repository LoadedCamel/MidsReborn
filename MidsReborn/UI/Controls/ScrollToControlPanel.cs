namespace Mids_Reborn.UI.Forms.Controls
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

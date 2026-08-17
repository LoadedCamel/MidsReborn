namespace Mids_Reborn.UI.Forms.Controls
{
    public partial class ScrollToControlPanel : Panel
    {
        public ScrollToControlPanel()
        {
            InitializeComponent();
        }
        
        // Prevent scroll position reset when losing focus
        protected override Point ScrollToControl(Control activeControl)
        {
            var loc = DisplayRectangle.Location;
            loc.Offset(new Point(-1 * Padding.Left, -1 * Padding.Bottom));

            return loc;
        }
    }
}

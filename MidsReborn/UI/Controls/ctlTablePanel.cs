namespace Mids_Reborn.UI.Controls
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

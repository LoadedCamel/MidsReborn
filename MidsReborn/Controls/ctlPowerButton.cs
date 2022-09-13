using System;
using System.Windows.Forms;

namespace Mids_Reborn.Controls
{
    public partial class ctlPowerButton : UserControl
    {
        public ctlPowerButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            Load += ctlPowerButton_Load;
        }

        private void ctlPowerButton_Load(object sender, EventArgs e)
        {
            PowerButton.MouseEnter += PowerButton_MouseEnter;
            PowerButton.MouseLeave += PowerButton_MouseLeave;
        }

        private void PowerButton_MouseEnter(object sender, EventArgs e)
        {
            PowerButton.ImageIndex = 1;
        }

        private void PowerButton_MouseLeave(object sender, EventArgs e)
        {
            PowerButton.ImageIndex = 0;
        }
    }
}

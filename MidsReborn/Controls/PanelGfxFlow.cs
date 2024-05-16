using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mids_Reborn.Controls
{
    public partial class PanelGfxFlow : FlowLayoutPanel
    {
        private const int WS_VSCROLL = 0x00200000;
        private const int WS_HSCROLL = 0x00100000;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style &= ~(WS_VSCROLL | WS_HSCROLL); // Remove vertical and horizontal scroll styles
                return cp;
            }
        }

        public PanelGfxFlow()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            // Filter out the WM_NCCALCSIZE message
            if (m.Msg == 0x83) // WM_NCCALCSIZE
            {
                var style = GetWindowLong(Handle, -16);
                if ((style & WS_VSCROLL) == WS_VSCROLL)
                    style &= ~WS_VSCROLL;
                if ((style & WS_HSCROLL) == WS_HSCROLL)
                    style &= ~WS_HSCROLL;
                _ = SetWindowLong(Handle, -16, style);

                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms
{
    public partial class PerPixelAlpha : Form
    {
        protected PerPixelAlpha()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // Add the layered extended style (WS_EX_LAYERED) to this window.
                var createParams = base.CreateParams;
                if(!DesignMode)
                    createParams.ExStyle |= WS_EX_LAYERED;
                return createParams;
            }
        }
        /// <summary>
        /// Let Windows drag this window for us (thinks its hitting the title 
        /// bar of the window)
        /// </summary>
        /// <param name="message"></param>
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WM_NCHITTEST)
            {
                // Tell Windows that the user is on the title bar (caption)
                message.Result = (IntPtr)HTCAPTION;
            }
            else
            {
                base.WndProc(ref message);
            }
        }
        #region Native Methods and Structures

        const int WS_EX_LAYERED = 0x80000;
        const int HTCAPTION = 0x02;
        const int WM_NCHITTEST = 0x84;
        const int ULW_ALPHA = 0x02;
        const byte AC_SRC_OVER = 0x00;
        const byte AC_SRC_ALPHA = 0x01;

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            { this.x = x; this.y = y; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private new struct Size
        {
            public int cx;
            public int cy;

            public Size(int cx, int cy)
            { this.cx = cx; this.cy = cy; }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BlendFunction
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDst,
            ref Point pptDst, ref Size pSize, IntPtr hdcSrc, ref Point pprSrc,
            int crKey, ref BlendFunction pBlend, int dwFlags);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDc);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hObject);

        #endregion
    }
}

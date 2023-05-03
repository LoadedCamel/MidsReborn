using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class PerPixelAlpha : Form
    {
        public PerPixelAlpha()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Load += Initializer_Load;
            InitializeComponent();
        }

        private void Initializer_Load(object? sender, EventArgs e)
        {
            TopMost = true;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap">
        /// 
        /// </param>
        /// <param name="opacity">
        /// Specifies an alpha transparency value to be used on the entire source 
        /// bitmap. The SourceConstantAlpha value is combined with any per-pixel 
        /// alpha values in the source bitmap. The value ranges from 0 to 255. If 
        /// you set SourceConstantAlpha to 0, it is assumed that your image is 
        /// transparent. When you only want to use per-pixel alpha values, set 
        /// the SourceConstantAlpha value to 255 (opaque).
        /// </param>
        public void SelectBitmap(Bitmap bitmap, int opacity = 255)
        {
            // Does this bitmap contain an alpha channel?
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ApplicationException("The bitmap must be 32bpp with alpha-channel.");
            }

            // Get device contexts
            var screenDc = GetDC(IntPtr.Zero);
            var memDc = CreateCompatibleDC(screenDc);
            var hBitmap = IntPtr.Zero;
            var hOldBitmap = IntPtr.Zero;

            try
            {
                // Get handle to the new bitmap and select it into the current 
                // device context.
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                hOldBitmap = SelectObject(memDc, hBitmap);

                // Set parameters for layered window update.
                var newSize = new Size(bitmap.Width, bitmap.Height);
                var sourceLocation = new Point(0, 0);
                var newLocation = new Point(Left, Top);
                var blend = new BlendFunction
                {
                    BlendOp = AC_SRC_OVER,
                    BlendFlags = 0,
                    SourceConstantAlpha = (byte)opacity,
                    AlphaFormat = AC_SRC_ALPHA
                };

                // Update the window.
                UpdateLayeredWindow(
                    Handle,     // Handle to the layered window
                    screenDc,        // Handle to the screen DC
                    ref newLocation, // New screen position of the layered window
                    ref newSize,     // New size of the layered window
                    memDc,           // Handle to the layered window surface DC
                    ref sourceLocation, // Location of the layer in the DC
                    0,               // Color key of the layered window
                    ref blend,       // Transparency of the layered window
                    ULW_ALPHA        // Use blend as the blend function
                    );
            }
            finally
            {
                // Release device context.
                ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    SelectObject(memDc, hOldBitmap);
                    DeleteObject(hBitmap);
                }
                DeleteDC(memDc);
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
        private struct Argb
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
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

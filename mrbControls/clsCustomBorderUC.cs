using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace mrbControls
{
    public static class clsCustomBorderHandlers
    {
        private static readonly Color DefaultBorderColor = Color.Black;

        private enum RedrawWindowFlags : uint
        {
            Invalidate = 0X1,
            InternalPaint = 0X2,
            Erase = 0X4,
            Validate = 0X8,
            NoInternalPaint = 0X10,
            NoErase = 0X20,
            NoChildren = 0X40,
            AllChildren = 0X80,
            UpdateNow = 0X100,
            EraseNow = 0X200,
            Frame = 0X400,
            NoFrame = 0X800
        }
        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("user32.dll")]
        private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);
        [DllImport("user32.dll", EntryPoint = "GetWindowDC", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);
        private const int WM_PAINT = 0XF;
        private const int WM_NCPAINT = 0X85;

        public static void OnResizeRedrawWindow(IntPtr ctlHandle)
        {
            RedrawWindow(ctlHandle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Frame | RedrawWindowFlags.UpdateNow | RedrawWindowFlags.Invalidate);
        }

        public static void WndProcDrawBorder(ref Message m, Control ctl, Color? borderColor)
        {
            if (m.Msg != WM_NCPAINT) return;
            var bc = borderColor ?? DefaultBorderColor;

            var hDc = GetWindowDC(m.HWnd);
            var p = new Pen(bc);
            using (var g = Graphics.FromHdc(hDc))
            {
                g.DrawRectangle(p, new Rectangle(0, 0, ctl.Width - 1, ctl.Height - 1));
                g.DrawRectangle(SystemPens.Window, new Rectangle(1, 1, ctl.Width - 3, ctl.Height - 3));
            }

            ReleaseDC(m.HWnd, hDc);
            p.Dispose();
        }

        public static Color GetDefaultColor()
        {
            return DefaultBorderColor;
        }
    }

    public class clsCustomBorderUC : UserControl
    {
        // https://social.msdn.microsoft.com/Forums/windows/en-US/1711fcb3-29c6-4e80-9aef-6cf8bb6fd215/c-how-do-you-add-a-border-on-a-control
        [Category("Appearance"), Description("Border color")]
        public Color BorderColor = clsCustomBorderHandlers.GetDefaultColor(); // SystemColors.WindowFrame
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            clsCustomBorderHandlers.OnResizeRedrawWindow(Handle);
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            clsCustomBorderHandlers.WndProcDrawBorder(ref m, this, BorderColor);
        }
    }
}
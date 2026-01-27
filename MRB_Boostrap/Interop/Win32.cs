using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace MRB_Boostrap.Interop
{
    internal abstract class Win32
    {
        [DllImport("user32.dll")] internal static extern int SetLayeredWindowAttributes(nint hwnd, int crKey, byte bAlpha, int dwFlags);
        [DllImport("gdi32.dll")] internal static extern nint CreateRoundRectRgn(int left, int top, int right, int bottom, int width, int height);
        [DllImport("user32.dll")] internal static extern int SetWindowRgn(nint hWnd, nint hRgn, bool redraw);
        [DllImport("user32.dll")] internal static extern nint SetTimer(nint hWnd, int nIdEvent, uint uElapse, nint lpTimerFunc);
        [DllImport("user32.dll")] internal static extern bool KillTimer(nint hWnd, int uIdEvent);
        [DllImport("user32.dll")] internal static extern bool GetMessage(out Msg lpMsg, nint hWnd, uint min, uint max);
        [DllImport("user32.dll")] internal static extern bool TranslateMessage(ref Msg lpMsg);
        [DllImport("user32.dll")] internal static extern nint DispatchMessage(ref Msg lpmsg);
        [DllImport("user32.dll")] internal static extern bool PostQuitMessage(int code);
        [DllImport("user32.dll")] internal static extern bool ShowWindow(nint hWnd, int cmdShow);
        [DllImport("user32.dll")] internal static extern bool UpdateWindow(nint hWnd);
        [DllImport("user32.dll")] internal static extern bool InvalidateRect(nint hWnd, nint lpRect, bool erase);
        [DllImport("user32.dll")] internal static extern bool ReleaseCapture();
        [DllImport("user32.dll")] internal static extern nint SendMessage(nint hWnd, int msg, nint wParam, nint lParam);
        [DllImport("user32.dll")] internal static extern bool PostMessage(nint hWnd, int Msg, nint wParam, nint lParam);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)] internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);
        [DllImport("dwmapi.dll")] internal static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins margins);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Margins
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Msg
        {
            public nint hwnd;
            public uint message;
            public nint wParam;
            public nint lParam;
            public uint time;
            public Point pt;
        }

        [Flags]
        public enum MoveFileFlags : uint
        {
            None = 0x0,
            ReplaceExisting = 0x1,
            CopyAllowed = 0x2,
            DelayUntilReboot = 0x4,
            WriteThrough = 0x8,
            CreateHardlink = 0x10,
            FailIfNotTrackable = 0x20
        }
    }
}

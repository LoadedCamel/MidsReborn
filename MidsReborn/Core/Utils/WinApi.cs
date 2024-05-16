using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Mids_Reborn.Core.Utils
{
    internal static class WinApi
    {
        [DllImport("user32")]
        public static extern bool AnimateWindow(IntPtr hWnd, int milliseconds, AnimationFlags flags);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string? pszSubIdList);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [Flags]
        public enum AnimationFlags : int
        {
            Roll = 0x0000,
            Slide = 0x40000,
            Blend = 0x80000,
            Hide = 0x10000,
            Center = 0x00000010,
            HorPositive = 0x00000001,
            HorNegative = 0x00000002,
            VerPositive = 0x00000004,
            VerNegative = 0x00000008,
            Activate = 0x20000
        }

        [DllImport("DwmApi", PreserveSig = false)]
        private static extern int DwmSetWindowAttribute(IntPtr hWnd, WindowAttribute attr, int[] attrValue, int attrSize);

        public enum WindowAttribute : int
        {
            Backdrop = 17,
            DarkMode = 20,
            Corner = 33,
            BorderColor = 34,
            CaptionColor = 35,
            TextColor = 36
        }

        public enum BackdropTypes : int
        {
            MainWindow = 1,
            TransientWindow = 2,
            TabbedWindow = 3
        }

        private static string GetRgb(Color color) => $"{color.B:X2}{color.G:X2}{color.R:X2}";

        public static void StylizeWindow(IntPtr handle, Color borderColor, Color? captionColor = null, Color? textColor = null)
        {
            var border = new[] { int.Parse(GetRgb(borderColor), NumberStyles.HexNumber) };
            _ = DwmSetWindowAttribute(handle, WindowAttribute.BorderColor, border, Marshal.SizeOf<int>());

            if (captionColor != null)
            {
                var caption = new[] { int.Parse(GetRgb((Color)captionColor), NumberStyles.HexNumber) };
                _ = DwmSetWindowAttribute(handle, WindowAttribute.CaptionColor, caption, Marshal.SizeOf<int>());
            }

            if (textColor == null) return;
            var text = new[] { int.Parse(GetRgb((Color)textColor), NumberStyles.HexNumber) };
            _ = DwmSetWindowAttribute(handle, WindowAttribute.TextColor, text, Marshal.SizeOf<int>());
        }

        // Example: Setting the Dark Mode
        public static void SetDarkMode(IntPtr handle, bool isEnabled)
        {
            var value = isEnabled ? 1 : 0;
            _ = DwmSetWindowAttribute(handle, WindowAttribute.DarkMode, new[] { value }, Marshal.SizeOf<int>());
        }

        // Example: Setting Window Corner Preference
        public static void SetWindowCornerPreference(IntPtr handle, int preference)
        {
            _ = DwmSetWindowAttribute(handle, WindowAttribute.Corner, new[] { preference }, Marshal.SizeOf<int>());
        }

        // Example: Setting Window Backdrop Type
        public static void SetWindowBackdropType(IntPtr handle, BackdropTypes backdropType)
        {
            var value = (int)backdropType;
            _ = DwmSetWindowAttribute(handle, WindowAttribute.Backdrop, new[] { value }, Marshal.SizeOf<int>());
        }
    }
}

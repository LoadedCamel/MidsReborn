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

        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hWnd, WindowAttribute attr, int[] attrValue, int attrSize);

        public enum WindowAttribute : int
        {
            BorderColor = 34,
            CaptionColor = 35,
            TextColor = 36,
            BorderThickness = 37
        }

        private static string GetRgb(Color color)
        {
            return $"{color.B:X2}{color.G:X2}{color.R:X2}";
        }

        public static void StylizeWindow(IntPtr handle, Color borderColor, Color? captionColor = null, Color? textColor = null)
        {
            var border = new[] { int.Parse(GetRgb(borderColor), NumberStyles.HexNumber) };
            _ = DwmSetWindowAttribute(handle, WindowAttribute.BorderColor, border, 4);

            if (captionColor != null)
            {
                var caption = new[] { int.Parse(GetRgb((Color)captionColor), NumberStyles.HexNumber) };
                _ = DwmSetWindowAttribute(handle, WindowAttribute.CaptionColor, caption, 4);
            }

            if (textColor == null) return;
            var text = new[] { int.Parse(GetRgb((Color)textColor), NumberStyles.HexNumber) };
            _ = DwmSetWindowAttribute(handle, WindowAttribute.TextColor, text, 4);
        }
    }
}

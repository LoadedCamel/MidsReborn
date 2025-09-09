using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Mids_Reborn.Core.Utils
{
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    internal static partial class WinApi
    {
        #region Library Imports

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool AnimateWindow(IntPtr hWnd, int milliseconds, AnimationFlags flags);

        [LibraryImport("uxtheme.dll")]
        internal static partial int SetWindowTheme(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string pszSubAppName, [MarshalAs(UnmanagedType.LPWStr)] string? pszSubIdList);

        [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
        internal static partial int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool ReleaseCapture();

        [LibraryImport("DwmApi.dll")]
        private static partial int DwmSetWindowAttribute(IntPtr hWnd, WindowAttribute attr, ref int attrValue, int attrSize);

        [LibraryImport("dwmapi.dll")]
        private static partial int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPositionOptions uFlags);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetForegroundWindow(IntPtr hWnd);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool FlashWindowEx(ref FlashWindowInfo fwi);

        [LibraryImport("user32.dll")]
        internal static partial int GetSystemMetrics(SystemMetric smIndex);

        [LibraryImport("user32.dll")]
        private static partial uint GetDpiForWindow(IntPtr hWnd);

        [LibraryImport("user32.dll")]
        private static partial int GetSystemMetricsForDpi(int nIndex, uint dpi);

        [LibraryImport("uxtheme.dll")]
        private static partial int SetWindowThemeAttribute(IntPtr hWnd, WindowThemeAttributeType attribute, ref WindowThemeAttributeOptions options, uint size);

        #endregion

        #region Constants

        public static readonly IntPtr TopMost = new(-1);
        public static readonly IntPtr NotTopMost = new(-2);
        public static readonly IntPtr Top = new(0);
        public static readonly IntPtr Bottom = new(1);

        private const int SmCxSizeFrame = 32;
        private const int SmCySizeFrame = 33;
        private const int SmCxPaddedBorder = 92;
        private const int SmCyPaddedBorder = 93;

        #endregion

        #region Enums

        [Flags]
        public enum AnimationFlags
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

        public enum WindowAttribute
        {
            Backdrop = 17,
            DarkMode = 20,
            Corner = 33,
            BorderColor = 34,
            CaptionColor = 35,
            TextColor = 36,
            BorderThickness = 37
        }

        public enum BackdropTypes
        {
            MainWindow = 1,
            TransientWindow = 2,
            TabbedWindow = 3
        }

        public enum CornerPreference
        {
            Default = 0,
            DoNotRound = 1,
            Round = 2,
            RoundSmall = 3
        }

        [Flags]
        public enum SetWindowPositionOptions : uint
        {
            IgnoreResize = 0x0001,
            IgnoreMove = 0x0002,
            IgnoreZOrder = 0x0004,
            SuppressRedraw = 0x0008,
            DoNotActivate = 0x0010,
            ApplyFrameChanges = 0x0020,
            ShowWindow = 0x0040,
            HideWindow = 0x0080,
            DiscardClientArea = 0x0100,
            IgnoreOwnerZOrder = 0x0200,
            SuppressChangeNotification = 0x0400
        }

        [Flags]
        public enum FlashWindowFlags : uint
        {
            Stop = 0,
            Caption = 1,
            Tray = 2,
            All = 3,
            Timer = 4,
            TimerNotInForeground = 12
        }

        public enum SystemMetric
        {
            PrimaryScreenWidth = 0,
            PrimaryScreenHeight = 1,
            VerticalScrollBarWidth = 2,
            HorizontalScrollBarHeight = 3,
            CaptionHeight = 4,
            WindowBorderWidth = 5,
            WindowBorderHeight = 6,
        }

        internal enum WindowThemeAttributeType : int
        {
            NonClient = 1
        }

        [Flags]
        private enum NonClientAreaOptions : uint
        {
            None = 0,
            NoDrawCaption = 0x00000001,
            NoDrawIcon = 0x00000002,
            NoSysMenu = 0x00000004
        }

        #endregion

        #region Common Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int LeftWidth;
            public int RightWidth;
            public int TopHeight;
            public int BottomHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FlashWindowInfo
        {
            public uint cbSize;
            public IntPtr hwnd;
            public FlashWindowFlags dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowThemeAttributeOptions
        {
            // ReSharper: public fields are intentional for blittable P/Invoke
            public uint Flags; // which options to enable
            public uint Mask;  // which options to change
        }

        #endregion

        #region Helper Methods

        private static string GetRgb(Color color) => $"{color.B:X2}{color.G:X2}{color.R:X2}";

        public static void StylizeWindow(IntPtr handle, Color borderColor, Color? captionColor = null, Color? textColor = null)
        {
            var border = int.Parse(GetRgb(borderColor), NumberStyles.HexNumber);
            _ = DwmSetWindowAttribute(handle, WindowAttribute.BorderColor, ref border, sizeof(int));

            if (captionColor != null)
            {
                var caption = int.Parse(GetRgb((Color)captionColor), NumberStyles.HexNumber);
                _ = DwmSetWindowAttribute(handle, WindowAttribute.CaptionColor, ref caption, sizeof(int));
            }

            if (textColor == null) return;
            var text = int.Parse(GetRgb((Color)textColor), NumberStyles.HexNumber);
            _ = DwmSetWindowAttribute(handle, WindowAttribute.TextColor, ref text, sizeof(int));
        }

        public static void SetDarkMode(IntPtr handle, bool isEnabled)
        {
            var value = isEnabled ? 1 : 0;
            _ = DwmSetWindowAttribute(handle, WindowAttribute.DarkMode, ref value, sizeof(int));
        }

        public static void SetWindowCornerPreference(IntPtr handle, CornerPreference preference)
        {
            var value = (int)preference;
            _ = DwmSetWindowAttribute(handle, WindowAttribute.Corner, ref value, sizeof(int));
        }

        public static void SetWindowBorderSize(IntPtr handle, int size)
        {
            var margins = new MARGINS
            {
                LeftWidth = size,
                RightWidth = size,
                TopHeight = size,
                BottomHeight = size
            };

            DwmExtendFrameIntoClientArea(handle, ref margins);
        }

        public static void SetWindowBackdropType(IntPtr handle, BackdropTypes backdropType)
        {
            var value = (int)backdropType;
            _ = DwmSetWindowAttribute(handle, WindowAttribute.Backdrop, ref value, sizeof(int));
        }

        public static void MakeWindowTopmost(IntPtr handle)
        {
            SetWindowPos(handle, TopMost, 0, 0, 0, 0, SetWindowPositionOptions.IgnoreMove | SetWindowPositionOptions.IgnoreResize);
        }

        public static bool FlashWindowUntilFocus(IntPtr handle)
        {
            FlashWindowInfo info = new FlashWindowInfo
            {
                hwnd = handle,
                dwFlags = FlashWindowFlags.All | FlashWindowFlags.TimerNotInForeground,
                uCount = uint.MaxValue,
                dwTimeout = 0
            };

            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            return FlashWindowEx(ref info);
        }

        public static bool StopFlashingWindow(IntPtr handle)
        {
            FlashWindowInfo info = new FlashWindowInfo
            {
                hwnd = handle,
                dwFlags = FlashWindowFlags.Stop,
            };

            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            return FlashWindowEx(ref info);
        }

        public static void DisableSystemCaptionAndBorder(IntPtr handle)
        {
            var flags = NonClientAreaOptions.NoDrawCaption |
                        NonClientAreaOptions.NoDrawIcon |
                        NonClientAreaOptions.NoSysMenu;

            var opts = new WindowThemeAttributeOptions
            {
                Flags = (uint)flags,
                Mask = (uint)flags
            };

            _ = SetWindowThemeAttribute(handle, WindowThemeAttributeType.NonClient, ref opts,
                (uint)Marshal.SizeOf<WindowThemeAttributeOptions>());
        }

        public static int GetResizeBorderThicknessPx(IntPtr hWnd)
        {
            uint dpi = GetDpiForWindow(hWnd);
            int frameX = GetSystemMetricsForDpi(SmCxSizeFrame, dpi);
            int padX = GetSystemMetricsForDpi(SmCxPaddedBorder, dpi);
            // symmetrical for Y; using X is fine here since we just need a thickness value
            return frameX + padX;
        }

        #endregion
    }
}
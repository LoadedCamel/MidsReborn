using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mids_Reborn.Core.Base.Extensions
{
    internal sealed class ColorDialogExt : ColorDialog
    {
        private const int InitDialog = 0x0110;
        private const uint NoSize = 0x0001;
        private const uint ShowWindow = 0x0040;
        private const uint NoZOrder = 0x0004;
        private const uint Flags = NoSize | NoZOrder | ShowWindow;
        private static readonly IntPtr Top = new(0);

        #region Api Calls

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool SetWindowText(IntPtr hWnd, string? text);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        #endregion

        private Control? Parent { get; set; }
        private Point Location { get; set; }
        private string? Title { get; set; }

        public ColorDialogExt(Control parent, string? title = "Color Selector")
        {
            Parent = parent;
            Title = title;
            FullOpen = true;
        }

        private void CenterToParent()
        {
            if (Parent is null) return;
            var x = Parent.Location.X + (Parent.Width - 450) / 2;
            var y = Parent.Location.Y + (Parent.Height - 300) / 2;
            Location = new Point(x, y);
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            var hookProc = base.HookProc(hWnd, msg, wparam, lparam);
            if (msg != InitDialog) return hookProc;
            CenterToParent();
            if (!string.IsNullOrEmpty(Title))
            {
                SetWindowText(hWnd, Title);
            }

            SetWindowPos(hWnd, Top, Location.X, Location.Y, 0, 0, Flags);
            return hookProc;
        }
    }
}

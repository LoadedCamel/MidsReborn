using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mids_Reborn.Core.Base.Extensions
{
    public static class ListviewExt
    {
        // Mitigate flickering on the ListView control.
        // https://stackoverflow.com/a/42389596
        public static void EnableDoubleBuffer(this ListView lvw)
        {
            lvw
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(lvw, true, null);
        }

        // http://csharphelper.com/blog/2018/03/display-icons-next-to-listview-sub-items-in-c/
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, ref LV_ITEM item_info);

        private struct LV_ITEM
        {
            public UInt32 uiMask;
            public Int32 iItem;
            public Int32 iSubItem;
            public UInt32 uiState;
            public UInt32 uiStateMask;
            public string pszText;
            public Int32 cchTextMax;
            public Int32 iImage;
            public IntPtr lParam;
        }

        public const Int32 LVM_FIRST = 0x1000;
        public const Int32 LVM_SETITEM = LVM_FIRST + 6;
        public const Int32 LVIF_IMAGE = 0x2;

        public const int LVW_FIRST = 0x1000;
        public const int LVM_SETEXTENDEDLISTVIEWSTYLE = LVW_FIRST + 54;
        public const int LVM_GETEXTENDEDLISTVIEWSTYLE = LVW_FIRST + 55;

        public const int LVS_EX_SUBITEMIMAGES = 0x2;

        // Add an icon to a subitem.
        public static void AddIconToSubItem(this ListView lvw, int row, int col, int iconNum)
        {
            var lvi = new LV_ITEM
            {
                iItem = row,          // Row
                iSubItem = col,       // Column
                uiMask = LVIF_IMAGE,  // We're setting the image
                iImage = iconNum      // The image index in the ImageList
            };

            // Send the LVM_SETITEM message.
            SendMessage(lvw.Handle, LVM_SETITEM, 0, ref lvi);
        }

        // Make the ListView display sub-item icons.
        public static void ShowSubItemIcons(this ListView lvw, bool show = true)
        {
            // Get the current style.
            var style = SendMessage(lvw.Handle, LVM_GETEXTENDEDLISTVIEWSTYLE, 0, 0);

            // Show or hide sub-item icons.
            if (show) style |= LVS_EX_SUBITEMIMAGES;
            else style &= (~LVS_EX_SUBITEMIMAGES);

            SendMessage(lvw.Handle, LVM_SETEXTENDEDLISTVIEWSTYLE, 0, style);
        }
    }
}
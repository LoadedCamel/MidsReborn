using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Design.Extensions
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
        private static extern int SendMessage(nint hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int SendMessage(nint hWnd, uint Msg, int wParam, ref LV_ITEM item_info);

        private struct LV_ITEM
        {
            public uint uiMask;
            public int iItem;
            public int iSubItem;
            public uint uiState;
            public uint uiStateMask;
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public nint lParam;
        }

        public const int LVM_FIRST = 0x1000;
        public const int LVM_SETITEM = LVM_FIRST + 6;
        public const int LVIF_IMAGE = 0x2;

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
            else style &= ~LVS_EX_SUBITEMIMAGES;

            SendMessage(lvw.Handle, LVM_SETEXTENDEDLISTVIEWSTYLE, 0, style);
        }
        public static LvKeyboardNavHandler AssignKeyboardNavHandler(this ListView targetControl, bool loopResults = true, long delayTime = 1000)
        {
            return new LvKeyboardNavHandler(targetControl, loopResults, delayTime);
        }

        public class LvKeyboardNavHandler
        {
            private ListView TargetControl;
            private long LastInputTime;
            private long DelayTime;
            private string InputString;
            private bool LoopResults;
            private AddCharResultCode LastResultCode;

            public LvKeyboardNavHandler(ListView targetControl, bool loopResults = true, long delayTime = 1000)
            {
                TargetControl = targetControl;
                LastInputTime = long.MinValue;
                DelayTime = delayTime;
                InputString = "";
                LoopResults = loopResults;
                LastResultCode = AddCharResultCode.None;
            }

            private enum AddCharResultCode
            {
                None,
                OneLetterNoChange,
                Added,
                Replaced
            }

            private AddCharResultCode AddChar(string input)
            {
                var resultCode = AddCharResultCode.None;
                input = input.ToLowerInvariant();

                if (InputString.Length == 1 && InputString.Equals(input, StringComparison.InvariantCultureIgnoreCase))
                {
                    resultCode = AddCharResultCode.OneLetterNoChange;
                }
                else if (LastInputTime <= double.MinValue || Math.Abs(LastInputTime - DateTimeOffset.Now.ToUnixTimeMilliseconds()) < DelayTime)
                {
                    InputString = $"{InputString}{input}";
                    resultCode = AddCharResultCode.Added;
                }
                else
                {
                    InputString = input;
                    resultCode = AddCharResultCode.Replaced;
                }

                LastInputTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                return resultCode;
            }

            private void SelectItem(int oldItem, int newItem)
            {
                TargetControl.Items[oldItem].Focused = false;
                TargetControl.SelectedIndices.Clear();
                TargetControl.SelectedIndices.Add(newItem);
                TargetControl.Items[newItem].EnsureVisible();
                TargetControl.Items[newItem].Focused = true;
            }

            private void SelectItem(int newItem)
            {
                TargetControl.SelectedIndices.Clear();
                TargetControl.SelectedIndices.Add(newItem);
                TargetControl.Items[newItem].EnsureVisible();
                TargetControl.Items[newItem].Focused = true;
            }

            public void ProcessInput(string input, List<string[]> virtualItems, int filterColumn)
            {
                var opCode = AddChar(input);
                if (TargetControl.SelectedIndices.Count <= 0)
                {
                    return;
                }

                var selectedIndex = TargetControl.SelectedIndices[0];

                if (InputString.Length > 1 && virtualItems[selectedIndex][filterColumn].ToLowerInvariant().StartsWith(InputString))
                {
                    return;
                }

                var itemsSlice = virtualItems.GetRange(selectedIndex + 1, virtualItems.Count - selectedIndex - 1);

                var nextItemIndex = itemsSlice.Select(e => e[filterColumn]).Any(e => e.ToLowerInvariant().StartsWith(InputString))
                    ? itemsSlice
                        .Select((e, i) => new KeyValuePair<int, string[]>(i, e))
                        .First(e => e.Value[filterColumn].ToLowerInvariant().StartsWith(InputString))
                        .Key
                    : -1;

                if (nextItemIndex >= 0)
                {
                    SelectItem(selectedIndex, nextItemIndex + selectedIndex + 1);

                    return;
                }

                if (LoopResults)
                {
                    nextItemIndex = virtualItems.Select(e => e[filterColumn]).Any(e => e.ToLowerInvariant().StartsWith(InputString))
                       ? virtualItems
                           .Select((e, i) => new KeyValuePair<int, string[]>(i, e))
                           .First(e => e.Value[filterColumn].ToLowerInvariant().StartsWith(InputString))
                           .Key
                       : -1;

                    if (nextItemIndex >= 0)
                    {
                        SelectItem(selectedIndex, nextItemIndex);

                        return;
                    }
                }

                SelectItem(selectedIndex);
                LastResultCode = opCode;
            }
        }
    }
}
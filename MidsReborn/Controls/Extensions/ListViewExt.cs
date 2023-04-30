using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.Controls.Extensions
{
    public static class ListViewExt
    {
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

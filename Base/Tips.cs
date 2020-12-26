using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mrbBase
{
    public class Tips
    {
        public enum TipType
        {
            TotalsTab,
            FirstPower,
            FirstEnh,

            // nothing from here on out is used, but old config files may expect it to exist, so leave them for migration
            Tip3,
            Tip4,
            Tip5,
            Tip6,
            Tip7,
            Tip8,
            Tip9,
            Tip10,
            Tip11,
            Tip12,
            Tip13,
            Tip14,
            Tip15,
            Tip16,
            Tip17,
            Tip18,
            Tip19,
            Tip20,
            Tip21,
            Tip22,
            Tip23,
            Tip24,
            Tip25,
            Tip26,
            Tip27,
            Tip28,
            Tip29,
            Tip30
        }

        private readonly int[] _tipStatus;

        public Tips()
        {
            _tipStatus = new int[Enum.GetValues(TipType.TotalsTab.GetType()).Length];
            for (var index = 0; index <= _tipStatus.Length - 1; ++index)
                _tipStatus[index] = 1;
        }

        // legacy for migration only
        public Tips(BinaryReader reader)
        {
            _tipStatus = new int[Enum.GetValues(TipType.TotalsTab.GetType()).Length];
            var num = reader.ReadInt32();
            if (num > _tipStatus.Length - 1)
                num = _tipStatus.Length - 1;
            for (var index = 0; index <= num; ++index)
                _tipStatus[index] = reader.ReadInt32() == 0 ? 1 : 0;
        }

        public int[] TipStatus => _tipStatus.Select(x => x).ToArray();

        public void Show(TipType tip)
        {
            if (_tipStatus[(int) tip] == 0)
                return;
            var stringBuilder = new StringBuilder();
            switch (tip)
            {
                case TipType.TotalsTab:
                    stringBuilder.AppendLine(
                        "While viewing the Totals tab, the powers which are being included are highlighted green.");
                    stringBuilder.AppendLine(
                        "Clicking a power will toggle it on or off. Dimmed powers can't be toggled as they have no effect on your totals.");
                    break;
                case TipType.FirstPower:
                    stringBuilder.AppendLine(
                        "If you decide you want to remove a power and replace it with a different one, click on the power name in the power lists");
                    stringBuilder.AppendLine(
                        "that appear on the left of the screen, or Alt+Click on the power bar. Then the next power you select will be placed into");
                    stringBuilder.AppendLine("the empty space.");
                    break;
                case TipType.FirstEnh:
                    stringBuilder.AppendLine("To put an enhancement into a slot, Right-Click on it.");
                    stringBuilder.AppendLine(string.Empty);
                    stringBuilder.AppendLine("To pick up a slot to move it somewhere else, Double-Click it.");
                    stringBuilder.AppendLine("Alternatively, Shift+Clicking enhancement slots will allow you");
                    stringBuilder.AppendLine("to pick up several slots one after the other before placing them again.");
                    stringBuilder.AppendLine(string.Empty);
                    stringBuilder.AppendLine("You can set the level an Invention enhancement is placed at by keying");
                    stringBuilder.AppendLine("the number into the enhancement picker before clicking on the enhancement.");
                    break;
            }

            _tipStatus[(int) tip] = 0;
            stringBuilder.AppendLine("\nThis message should not appear again.");
            MessageBox.Show(stringBuilder.ToString(), "Instructions");
        }
    }
}
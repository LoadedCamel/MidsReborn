using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Core
{
    public class Tips
    {
        public enum TipType
        {
            TotalsTab,
            FirstPower,
            FirstSlot,
            FirstEnhancement,
            PowerToggle,
            ProcToggle,
        }

        // Make _tipStatus private and add a public property for serialization
        //private bool[] _tipStatus;
        private Dictionary<TipType, bool>? _tipShown;

        // Public property for serialization and deserialization
        public Dictionary<TipType, bool>? TipShown
        {
            get => _tipShown;
            set => _tipShown = value ?? InitializeTipStatus();
        }

        public Tips()
        {
            _tipShown = InitializeTipStatus();
        }

        // Initialize all tip statuses to false
        private Dictionary<TipType, bool> InitializeTipStatus()
        {
            var status = new Dictionary<TipType, bool>();
            foreach (TipType tip in Enum.GetValues(typeof(TipType)))
            {
                status[tip] = false;
            }
            return status;
        }

        public void Show(TipType tip)
        {
            if (_tipShown == null || !_tipShown.ContainsKey(tip) || _tipShown[tip]) return;
            var message = GetTipMessage(tip);
            _tipShown[tip] = true; // Mark the tip as shown
            MessageBoxEx.Show($"{message}\n\nThis message should not appear again.\n",@"Tip", MessageBoxEx.MessageBoxExButtons.Okay);
        }

        private string GetTipMessage(TipType tip)
        {
            var stringBuilder = new StringBuilder();
            switch (tip)
            {
                case TipType.TotalsTab:
                    stringBuilder.AppendLine("While viewing the Totals tab, the powers which are being included are lit up.");
                    stringBuilder.AppendLine("Clicking a power will toggle it on or off. Dimmed powers can't be toggled as they have no effect on your totals.");
                    break;
                case TipType.FirstPower:
                    stringBuilder.AppendLine("If you decide you want to remove a power and replace it with a different one, click on the power name in the power lists");
                    stringBuilder.AppendLine("that appear on the left of the screen, or Alt+Click on the power bar. Then the next power you select will be placed into the empty space.");
                    break;
                case TipType.FirstSlot:
                    stringBuilder.AppendLine("To put an enhancement into a slot, Right-Click on it.");
                    stringBuilder.AppendLine("To pick up a slot to move it somewhere else, Double-Click it.");
                    stringBuilder.AppendLine("Alternatively, Shift+Clicking enhancement slots will allow you to pick up several slots one after the other before placing them again.");
                    stringBuilder.AppendLine("You can set the level an Invention enhancement is placed at by keying the number into the enhancement picker before clicking on the enhancement.");
                    break;
                case TipType.FirstEnhancement:
                    stringBuilder.AppendLine("To quickly add the next enhancement from a set into a slot,");
                    stringBuilder.AppendLine("using your mouse Middle-Click on the next slot. This will automatically add the next enhancement from that set");
                    break;
            }
            return stringBuilder.ToString();
        }
    }
}
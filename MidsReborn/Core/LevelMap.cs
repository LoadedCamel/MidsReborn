using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Mids_Reborn.Core
{
    public class LevelMap
    {
        public readonly int Powers;
        public readonly int Slots;

        public LevelMap(IList<string> ioString)
        {
            try
            {
                if (!int.TryParse(ioString[1], out Powers))
                    Powers = 0;
                if (ioString.Count <= 2 || int.TryParse(ioString[2], out Slots))
                    return;
                Slots = 0;
            }
            catch (Exception ex)
            {
                var num = (int) MessageBox.Show("An error has occurred reading level data from database. Error: " +
                                                ex.Message);
                throw;
            }
        }

        public Enums.dmItem LevelType()
        {
            return Powers <= 0 ? Slots <= 0 ? Enums.dmItem.None : Enums.dmItem.Slot : Enums.dmItem.Power;
        }
    }
}
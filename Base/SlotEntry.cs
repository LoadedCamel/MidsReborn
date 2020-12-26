namespace mrbBase
{
    public struct SlotEntry
    {
        public int Level;
        public I9Slot Enhancement;
        public I9Slot FlippedEnhancement;

        public void Assign(SlotEntry slotEntry)
        {
            Level = slotEntry.Level;
            Enhancement = slotEntry.Enhancement.Clone() as I9Slot;
            FlippedEnhancement = slotEntry.FlippedEnhancement.Clone() as I9Slot;
        }

        public void Flip()
        {
            var i9Slot = Enhancement.Clone() as I9Slot;
            Enhancement = FlippedEnhancement.Clone() as I9Slot;
            FlippedEnhancement = i9Slot == null ? new I9Slot() : i9Slot.Clone() as I9Slot;
        }

        public void LoadFromString(string iString, string delimiter)
        {
            var strArray = iString.Split(delimiter.ToCharArray());
            var i9Slot1 = new I9Slot();
            var i9Slot2 = new I9Slot();
            if (strArray.Length > 4)
            {
                i9Slot1.Enh = DatabaseAPI.FindEnhancement(strArray[0], strArray[1], int.Parse(strArray[2]),
                    int.Parse(strArray[3]));
                i9Slot1.RelativeLevel = (Enums.eEnhRelative) int.Parse(strArray[4]);
                i9Slot1.Grade = (Enums.eEnhGrade) int.Parse(strArray[5]);
                i9Slot1.IOLevel = int.Parse(strArray[6]);
                if (i9Slot1.IOLevel > 49)
                    i9Slot1.IOLevel = 49;
                if (strArray.Length > 12)
                {
                    i9Slot2.Enh = DatabaseAPI.FindEnhancement(strArray[7], strArray[8], int.Parse(strArray[9]),
                        int.Parse(strArray[10]));
                    i9Slot2.RelativeLevel = (Enums.eEnhRelative) int.Parse(strArray[11]);
                    i9Slot2.Grade = (Enums.eEnhGrade) int.Parse(strArray[12]);
                    i9Slot2.IOLevel = int.Parse(strArray[13]);
                    if (i9Slot2.IOLevel > 49)
                        i9Slot2.IOLevel = 49;
                }
            }
            else if (strArray.Length > 3)
            {
                i9Slot1.Enh = int.Parse(strArray[0]);
                i9Slot1.RelativeLevel = (Enums.eEnhRelative) int.Parse(strArray[1]);
                i9Slot1.Grade = (Enums.eEnhGrade) int.Parse(strArray[2]);
                i9Slot1.IOLevel = int.Parse(strArray[3]);
                if (i9Slot1.IOLevel > 49)
                    i9Slot1.IOLevel = 49;
            }

            Enhancement = i9Slot1.Clone() as I9Slot;
            FlippedEnhancement = i9Slot2.Clone() as I9Slot;
        }
    }
}
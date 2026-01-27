namespace Mids_Reborn.Core
{
    public class RawSaveResult
    {
        public readonly int Hash;
        public readonly int Length;

        public RawSaveResult(int length, int hash)
        {
            Length = length;
            Hash = hash;
        }
    }

    public class HistoryMap
    {
        internal int HID = -1;
        internal int Level = -1;
        internal int SID = -1;
        internal string Text = string.Empty;
    }
}
namespace mrbBase
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

    public class FHash
    {
        public readonly string Archetype;
        public readonly string Fullname;
        public readonly int Hash;
        public readonly int Length;

        public FHash(string archetype, string fullname, int length, int hash)
        {
            Archetype = archetype;
            Fullname = fullname;
            Hash = hash;
            Length = length;
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
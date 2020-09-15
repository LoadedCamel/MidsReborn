public class RawSaveResult
{
    internal readonly int Hash;
    internal readonly int Length;

    public RawSaveResult(int length, int hash)
    {
        Length = length;
        Hash = hash;
    }
}

public class FHash
{
    internal readonly string Archetype;
    internal readonly string Fullname;
    internal readonly int Hash;
    internal readonly int Length;

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
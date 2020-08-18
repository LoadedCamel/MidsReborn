using System.Collections.Generic;
using System.Linq;

public class BuilderApp
{
    public string Software;
    public string Version;
}

public class RawCharacterInfo
{
    public string Alignment;
    public string Archetype;
    public int Level;
    public string Name;
    public string Origin;
}

public class RawEnhData
{
    public int Boosters;
    public int eData;
    public bool HasCatalyst; // NOT FULLY IMPLEMENTED
    public string InternalName;
    public int Level;
}

public class RawPowerData
{
    public string DisplayName;
    public string FullName;
    public int Level;
    public IPower pData;
    public IPowerset Powerset;
    public List<RawEnhData> Slots;
    public bool Valid;
}

public class UniqueList<T> : List<T>
{
    public new void Add(T item)
    {
        if (!Contains(item)) base.Add(item);
    }

    public void FromList(List<T> listElements)
    {
        Clear();
        AddRange(listElements);
    }
}

public class SlotLevelQueue
{
    private int Index;
    private int Level = -1;

    private int SlotsAtLevel;
    /*private static readonly int[] SlotLevels = new[]
        {
            2, 2, 4, 4, 6, 6, 8, 8, 10, 10, 12, 12, 14, 14, 16, 16, 18, 18, 20, 20, 22, 22, 24, 24, 26, 26, 28, 28,
            30, 30, 30, 32, 32, 32, 33, 33, 33, 35, 35, 35, 36, 36, 36, 38, 38, 38, 39, 39, 39,
            41, 41, 41, 42, 42, 42, 45, 44, 44, 44, 45, 45, 47, 47, 47, 49, 49, 49
        };*/

    public static int GetNumSlotsBeforeLevel(int level)
    {
        return DatabaseAPI.Database.Levels.Take(level).Select(e => e.Slots).Sum();
    }

    public static int GetTotalSlots()
    {
        return DatabaseAPI.Database.Levels.Select(e => e.Slots).Sum();
    }

    public int PickSlot()
    {
        /*if (this.Index >= SlotLevels.Length) return 50;

            return SlotLevels[this.Index++];
            */

        if (Level == 49 && SlotsAtLevel == 0) return 50;

        if (SlotsAtLevel > 0)
        {
            SlotsAtLevel--;
            Index++;

            return Level;
        }

        while (SlotsAtLevel == 0 && Level < 50)
        {
            Level++;
            SlotsAtLevel = DatabaseAPI.Database.Levels[Level].Slots;
        }

        Index++;
        SlotsAtLevel--;
        return Level;
    }

    public bool IsValidNext()
    {
        return Level != 49 || SlotsAtLevel != 0;
    }
}
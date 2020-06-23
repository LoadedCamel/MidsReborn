using System.Collections.Generic;
using System.Linq;

namespace HeroViewer.Base
{
    public class BuilderApp
    {
        public string Software;
        public string Version;
    }

    public class RawCharacterInfo
    {
        public string Name;
        public string Origin;
        public int Level;
        public string Alignment;
        public string Archetype;
    }

    public class RawEnhData
    {
        public string InternalName;
        public int Level;
        public int Boosters;
        public bool HasCatalyst; // NOT FULLY IMPLEMENTED
        public int eData;
    }

    public class RawPowerData
    {
        public string FullName;
        public string DisplayName;
        public int Level;
        public bool Valid;
        public IPowerset Powerset;
        public List<RawEnhData> Slots;
        public IPower pData;
    }

    public class UniqueList<T> : List<T>
    {
        public new void Add(T item)
        {
            if (!Contains(item)) base.Add(item);
        }

        public void FromList(List<T> listElements)
        {
            base.Clear();
            base.AddRange(listElements);
        }
    }

    public class SlotLevelQueue
    {
        private int Index = 0;
        private int Level = -1;
        private int SlotsAtLevel = 0;
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

        public SlotLevelQueue() { }

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
}
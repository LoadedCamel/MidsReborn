using System.Collections.Generic;
using System.Linq;

namespace Mids_Reborn.Core
{
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
        public IPower? pData;
        public IPowerset? Powerset;
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
        private int Level = -1;

        private int SlotsAtLevel;

        public int PickSlot()
        {
            /*if (this.Index >= SlotLevels.Length) return 50;

            return SlotLevels[this.Index++];
            */

            if (Level == 49 && SlotsAtLevel == 0) return 50;

            if (SlotsAtLevel > 0)
            {
                SlotsAtLevel--;

                return Level;
            }

            while (SlotsAtLevel == 0 && Level < 50)
            {
                Level++;
                SlotsAtLevel = DatabaseAPI.Database.Levels[Level].Slots;
            }

            SlotsAtLevel--;
            return Level;
        }
    }
}
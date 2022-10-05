using System.IO;

namespace Mids_Reborn.Core
{
    public interface IEnhancement
    {
        bool HasEnhEffect { get; }

        bool HasPowerEffect { get; }

        Enums.eSchedule Schedule { get; }

        float Probability { get; }

        bool IsModified { get; set; }

        bool IsNew { get; set; }

        int StaticIndex { get; set; }

        string Name { get; set; }

        string ShortName { get; set; }

        string Desc { get; set; }

        Enums.eType TypeID { get; set; }

        //Enums.eSubtype SubTypeID { get; set; }

        int SubTypeID { get; set; }

        int[] ClassID { get; set; }

        string Image { get; set; }

        int ImageIdx { get; set; }

        int nIDSet { get; set; }

        string UIDSet { get; set; }

        Enums.sEffect[] Effect { get; set; }

        float EffectChance { get; set; }

        int LevelMin { get; set; }

        int LevelMax { get; set; }

        bool Unique { get; set; }

        Enums.eEnhMutex MutExID { get; set; }

        Enums.eBuffDebuff BuffMode { get; set; }

        string RecipeName { get; set; }

        int RecipeIDX { get; set; }

        string UID { get; set; }

        bool IsProc { get; set; }

        bool IsScalable { get; set; }

        bool Superior { get; set; }

        string LongName { get; }

        //IPower Power { get; set; }
        IPower GetPower();
        void SetPower(IPower power);

        int CheckAndFixIOLevel(int level);

        void StoreTo(BinaryWriter writer);

        string GetSpecialName();
    }
}
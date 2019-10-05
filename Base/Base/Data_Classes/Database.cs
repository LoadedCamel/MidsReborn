
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Base.Data_Classes
{
    public sealed class Database : IDatabase
    {
        public float Version { get; set; }

        public int Issue { get; set; }

        public DateTime Date { get; set; }

        public IPower[] Power { get; set; }

        public Enums.VersionData PowerVersion { get; set; } = new Enums.VersionData();

        public Enums.VersionData PowerEffectVersion { get; set; } = new Enums.VersionData();

        public Enums.VersionData PowerLevelVersion { get; set; } = new Enums.VersionData();

        public IPowerset[] Powersets { get; set; }

        public Enums.VersionData PowersetVersion { get; set; } = new Enums.VersionData();

        public Archetype[] Classes { get; set; }

        public Enums.VersionData ArchetypeVersion { get; set; } = new Enums.VersionData();

        public IEnhancement[] Enhancements { get; set; }

        public EnhancementSetCollection EnhancementSets { get; set; }

        public Enums.sEnhClass[] EnhancementClasses { get; set; }

        public Recipe[] Recipes { get; set; }

        public DateTime RecipeRevisionDate { get; set; }

        public string RecipeSource1 { get; set; }

        public string RecipeSource2 { get; set; }

        public Salvage[] Salvage { get; set; }

        public List<Origin> Origins { get; set; }

        public IDictionary<string, PowersetGroup> PowersetGroups { get; set; }

        public bool Loading { get; set; }

        public object I9 { get; set; }

        public Enums.VersionData IOAssignmentVersion { get; set; } = new Enums.VersionData();

        public SummonedEntity[] Entities { get; set; } = new SummonedEntity[0];

        public Modifiers AttribMods { get; set; }

        public LevelMap[] Levels { get; set; }

        public int[] Levels_MainPowers { get; set; }

        public List<string> EffectIds { get; set; } = new List<string>();

        public static Database Instance { get; } = new Database();

        public float VersionEnhDb { get; set; }

        public float[][] MultED { get; set; }

        public float[][] MultTO { get; set; }

        public float[][] MultDO { get; set; }

        public float[][] MultSO { get; set; }

        public float[][] MultHO { get; set; }

        public float[][] MultIO { get; set; }

        public string[] SetTypeStringLong { get; set; }

        public string[] SetTypeStringShort { get; set; }

        public string[] EnhGradeStringLong { get; set; }

        public string[] EnhGradeStringShort { get; set; }

        public string[] SpecialEnhStringLong { get; set; }

        public string[] SpecialEnhStringShort { get; set; }

        public string[] MutexList { get; set; }

        public void LoadEntities(BinaryReader reader)
        {
            Entities = new SummonedEntity[reader.ReadInt32() + 1];
            for (int index = 0; index <= Entities.Length - 1; ++index)
                Entities[index] = new SummonedEntity(reader);
        }

        public void StoreEntities(BinaryWriter writer)
        {
            writer.Write(Entities.Length - 1);
            for (int index = 0; index <= Entities.Length - 1; ++index)
                Entities[index].StoreTo(writer);
        }
    }
}

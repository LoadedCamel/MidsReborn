using System;
using System.Collections.Generic;
using System.IO;
using Mids_Reborn.Core.Base;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Core
{
    public interface IDatabase
    {
        string UpdateManifest { get; set; }

        Version Version { get; set; }

        int Issue { get; set; }

        int PageVol { get; set; }

        string PageVolText { get; set; }

        DateTime Date { get; set; }

        IPower?[] Power { get; set; }

        Enums.VersionData PowerVersion { get; set; }

        Enums.VersionData PowerEffectVersion { get; set; }

        Enums.VersionData PowerLevelVersion { get; set; }

        IPowerset?[] Powersets { get; set; }

        Enums.VersionData PowersetVersion { get; set; }

        Archetype?[] Classes { get; set; }

        Enums.VersionData ArchetypeVersion { get; set; }

        IDictionary<string, PowersetGroup> PowersetGroups { get; set; }

        bool Loading { get; set; }

        object I9 { get; set; }

        Enums.VersionData IOAssignmentVersion { get; set; }

        Modifiers AttribMods { get; set; }

        SummonedEntity[] Entities { get; set; }

        LevelMap[] Levels { get; set; }

        int[] Levels_MainPowers { get; set; }

        List<string> EffectIds { get; set; }

        IEnhancement[] Enhancements { get; set; }

        EnhancementSetCollection EnhancementSets { get; set; }

        Enums.sEnhClass[] EnhancementClasses { get; set; }

        Recipe[] Recipes { get; set; }

        DateTime RecipeRevisionDate { get; set; }

        string RecipeSource1 { get; set; }

        string RecipeSource2 { get; set; }

        Salvage[] Salvage { get; set; }

        PowersReplTable ReplTable { get; set; }

        CrypticReplTable? CrypticReplTable { get; set; }

        List<Origin> Origins { get; set; }

        float VersionEnhDb { get; set; }

        float[][] MultED { get; set; }

        float[][] MultTO { get; set; }

        float[][] MultDO { get; set; }

        float[][] MultSO { get; set; }

        float[][] MultHO { get; set; }

        float[][] MultIO { get; set; }

        List<TypeGrade> SetTypes { get; set; }

        List<TypeGrade> EnhancementGrades { get; set; }

        List<TypeGrade> SpecialEnhancements { get; set; }

        string[] SetTypeStringLong { get; set; }

        string[] SetTypeStringShort { get; set; }

        string[] EnhGradeStringLong { get; set; }

        string[] EnhGradeStringShort { get; set; }

        string[] SpecialEnhStringLong { get; set; }

        string[] SpecialEnhStringShort { get; set; }

        string[] MutexList { get; set; }

        void LoadEntities(BinaryReader reader);

        void StoreEntities(BinaryWriter writer);
    }
}
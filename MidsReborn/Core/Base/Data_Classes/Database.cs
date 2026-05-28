using System;
using System.Collections.Generic;
using System.IO;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.Core.Utils;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Base.Data_Classes
{
    public sealed class Database : IDatabase
    {
        public static Database Instance { get; } = new();

        public string UpdateManifest { get; set; }

        public Version Version { get; set; }

        public int Issue { get; set; }

        public int PageVol { get; set; }

        public string PageVolText { get; set; }

        public DateTime Date { get; set; }

        public IPower?[] Power { get; set; }

        public Enums.VersionData PowerVersion { get; set; } = new();

        public Enums.VersionData PowerEffectVersion { get; set; } = new();

        public Enums.VersionData PowerLevelVersion { get; set; } = new();

        public IPowerset?[] Powersets { get; set; }

        public Enums.VersionData PowersetVersion { get; set; } = new();

        public Archetype[]? Classes { get; set; }

        public Enums.VersionData ArchetypeVersion { get; set; } = new();

        public IEnhancement[] Enhancements { get; set; }

        public EnhancementSetCollection EnhancementSets { get; set; }

        public Enums.sEnhClass[] EnhancementClasses { get; set; }

        public Recipe[] Recipes { get; set; }

        public DateTime RecipeRevisionDate { get; set; }

        public string RecipeSource1 { get; set; }

        public string RecipeSource2 { get; set; }

        public Salvage[] Salvage { get; set; }

        public PowersReplTable ReplTable { get; set; }

        public CrypticReplTable? CrypticReplTable { get; set; }

        public List<Origin> Origins { get; set; }

        public IDictionary<string, PowersetGroup> PowersetGroups { get; set; }

        public bool Loading { get; set; }

        public object I9 { get; set; }

        public Enums.VersionData IOAssignmentVersion { get; set; } = new();

        public SummonedEntity[] Entities { get; set; } = new SummonedEntity[0];

        public OmniDatabaseImportSource OmniImportSource { get; set; }

        public OmniDataProviderId DataProviderId { get; set; }

        public PlannerRulesetId PlannerRulesetId { get; set; }

        public int PlannerRulesetVersion { get; set; }

        public bool HasCanonicalOmniPlannerMath { get; set; }

        public bool HasPersistedOmniRuntimeMetadata { get; set; }

        public Dictionary<string, OmniClassAttributeTable> ClassAttributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public BuildProgressionMetadata BuildProgressionMetadata { get; set; } = new();

        public EnhancementImportMetadata EnhancementImportMetadata { get; set; } = new();

        public PowerImportMetadata PowerImportMetadata { get; set; } = new();

        public EntityImportMetadata EntityImportMetadata { get; set; } = new();

        public LevelMap[] Levels { get; set; }

        public int[] Levels_MainPowers { get; set; }

        public List<string> EffectIds { get; set; } = new();

        public float VersionEnhDb { get; set; }

        public float[][] MultED { get; set; }

        public float[][] MultTO { get; set; }

        public float[][] MultDO { get; set; }

        public float[][] MultSO { get; set; }

        public float[][] MultHO { get; set; }

        public float[][] MultIO { get; set; }

        public List<TypeGrade> SetTypes { get; set; }
        
        public List<TypeGrade> EnhancementGrades { get; set; }

        public List<TypeGrade> SpecialEnhancements { get; set; }

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
            for (var index = 0; index < Entities.Length; index++)
            {
                Entities[index] = new SummonedEntity(reader);
            }
        }

        public void StoreEntities(BinaryWriter writer)
        {
            writer.Write(Entities.Length - 1);
            foreach (var ent in Entities)
            {
                ent.StoreTo(writer);
            }
        }

        public void LoadOmniMetadata(BinaryReader reader)
        {
            OmniImportSource = OmniDatabaseImportSource.Unknown;
            DataProviderId = OmniDataProviderId.Unknown;
            PlannerRulesetId = PlannerRulesetId.Legacy;
            PlannerRulesetVersion = 0;
            HasCanonicalOmniPlannerMath = false;
            HasPersistedOmniRuntimeMetadata = false;
            ClassAttributes = new Dictionary<string, OmniClassAttributeTable>(StringComparer.OrdinalIgnoreCase);
            BuildProgressionMetadata = new BuildProgressionMetadata();
            EnhancementImportMetadata = new EnhancementImportMetadata();
            PowerImportMetadata = new PowerImportMetadata();
            EntityImportMetadata = new EntityImportMetadata();
            if (reader.BaseStream.Position >= reader.BaseStream.Length)
            {
                return;
            }

            var marker = reader.ReadString();
            if (!string.Equals(marker, "MRB_OMNI_METADATA", StringComparison.Ordinal))
            {
                return;
            }

            var version = reader.ReadInt32();
            if (version is not 1 and not 2 and not 3 and not 4 and not 5 and not 6 and not 7)
            {
                return;
            }

            var json = reader.ReadString();
            switch (version)
            {
                case 1:
                {
                    var legacy = JsonConvert.DeserializeObject<Dictionary<string, OmniClassAttributeTable>>(json);
                    ClassAttributes = legacy == null
                        ? new Dictionary<string, OmniClassAttributeTable>(StringComparer.OrdinalIgnoreCase)
                        : new Dictionary<string, OmniClassAttributeTable>(legacy, StringComparer.OrdinalIgnoreCase);
                    if (ClassAttributes.Count > 0)
                    {
                        OmniImportSource = OmniDatabaseImportSource.Omni;
                        DataProviderId = OmniDataProviderId.OmniHomecoming;
                    }

                    break;
                }
                case 2:
                {
                    var metadata = JsonConvert.DeserializeObject<OmniDatabaseMetadata>(json) ?? new OmniDatabaseMetadata();
                    OmniImportSource = metadata.ImportSource;
                    DataProviderId = metadata.DataProviderId == OmniDataProviderId.Unknown &&
                                     metadata.ImportSource == OmniDatabaseImportSource.Omni
                        ? OmniDataProviderId.OmniHomecoming
                        : metadata.DataProviderId;
                    PlannerRulesetId = metadata.HasCanonicalOmniPlannerMath
                        ? PlannerRulesetId.Homecoming
                        : PlannerRulesetId.Legacy;
                    PlannerRulesetVersion = metadata.PlannerRulesetVersion;
                    HasCanonicalOmniPlannerMath = metadata.HasCanonicalOmniPlannerMath;
                    HasPersistedOmniRuntimeMetadata = false;
                    ClassAttributes = metadata.ClassAttributes == null
                        ? new Dictionary<string, OmniClassAttributeTable>(StringComparer.OrdinalIgnoreCase)
                        : new Dictionary<string, OmniClassAttributeTable>(metadata.ClassAttributes, StringComparer.OrdinalIgnoreCase);
                    BuildProgressionMetadata = metadata.BuildProgression ?? new BuildProgressionMetadata();
                    break;
                }
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                {
                    var metadata = JsonConvert.DeserializeObject<OmniDatabaseMetadata>(json) ?? new OmniDatabaseMetadata();
                    OmniImportSource = metadata.ImportSource;
                    DataProviderId = metadata.DataProviderId;
                    PlannerRulesetId = metadata.PlannerRulesetId;
                    PlannerRulesetVersion = metadata.PlannerRulesetVersion;
                    HasCanonicalOmniPlannerMath = metadata.HasCanonicalOmniPlannerMath;
                    HasPersistedOmniRuntimeMetadata = version >= 6 && metadata.HasPersistedOmniRuntimeMetadata;
                    ClassAttributes = metadata.ClassAttributes == null
                        ? new Dictionary<string, OmniClassAttributeTable>(StringComparer.OrdinalIgnoreCase)
                        : new Dictionary<string, OmniClassAttributeTable>(metadata.ClassAttributes, StringComparer.OrdinalIgnoreCase);
                    BuildProgressionMetadata = metadata.BuildProgression ?? new BuildProgressionMetadata();
                    EnhancementImportMetadata = metadata.EnhancementImport ?? new EnhancementImportMetadata();
                    PowerImportMetadata = metadata.PowerImport ?? new PowerImportMetadata();
                    EntityImportMetadata = metadata.EntityImport ?? new EntityImportMetadata();
                    break;
                }
            }

            if (PlannerRulesetId == PlannerRulesetId.Legacy && HasCanonicalOmniPlannerMath)
            {
                PlannerRulesetId = PlannerRulesetId.Homecoming;
            }
        }

        public void StoreOmniMetadata(BinaryWriter writer)
        {
            writer.Write("MRB_OMNI_METADATA");
            writer.Write(7);
            writer.Write(JsonConvert.SerializeObject(new OmniDatabaseMetadata
            {
                ImportSource = OmniImportSource,
                DataProviderId = DataProviderId,
                PlannerRulesetId = PlannerRulesetId,
                PlannerRulesetVersion = PlannerRulesetVersion,
                HasCanonicalOmniPlannerMath = HasCanonicalOmniPlannerMath,
                HasPersistedOmniRuntimeMetadata = HasPersistedOmniRuntimeMetadata,
                ClassAttributes = new Dictionary<string, OmniClassAttributeTable>(ClassAttributes, StringComparer.OrdinalIgnoreCase),
                BuildProgression = BuildProgressionMetadata,
                EnhancementImport = EnhancementImportMetadata,
                PowerImport = PowerImportMetadata,
                EntityImport = EntityImportMetadata
            }));
        }

    }
}

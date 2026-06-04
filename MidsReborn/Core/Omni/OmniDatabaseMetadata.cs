using System;
using System.Collections.Generic;
using System.Linq;
using Mids_Reborn.Core;
using Mids_Reborn.Core.PlannerRulesets;
using Newtonsoft.Json.Linq;

namespace Mids_Reborn.Core.Omni;

public enum OmniDatabaseImportSource
{
    Unknown = 0,
    Omni = 1
}

public enum OmniDataProviderId
{
    Unknown = 0,
    OmniHomecoming = 1,
    OmniRebirth = 2,
    OmniThunderspy = 3
}

public enum PlannerRulesetId
{
    Legacy = 0,
    Homecoming = 1,
    Ourodev = 2
}

public sealed class OmniDatabaseMetadata
{
    public OmniDatabaseImportSource ImportSource { get; set; } = OmniDatabaseImportSource.Unknown;
    public OmniDataProviderId DataProviderId { get; set; } = OmniDataProviderId.Unknown;
    public PlannerRulesetId PlannerRulesetId { get; set; } = PlannerRulesetId.Legacy;
    public int PlannerRulesetVersion { get; set; }
    public bool HasCanonicalOmniPlannerMath { get; set; }
    public bool HasPersistedOmniRuntimeMetadata { get; set; }
    public Dictionary<string, OmniClassAttributeTable> ClassAttributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public BuildProgressionMetadata BuildProgression { get; set; } = new();
    public EnhancementImportMetadata EnhancementImport { get; set; } = new();
    public PowerImportMetadata PowerImport { get; set; } = new();
    public EntityImportMetadata EntityImport { get; set; } = new();
}

public sealed class PowerImportMetadata
{
    public Dictionary<string, ImportedPowerSemantics> Powers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ImportedPowerSemantics
{
    public string TargetRequires { get; set; } = string.Empty;
    public List<string> RequiredModes { get; set; } = [];
    public List<string> DisallowedModes { get; set; } = [];
    public List<OmniEffectDefinition> ActivationEffects { get; set; } = [];
    public string ProcEligibility { get; set; } = nameof(ImportedProcEligibilityMode.Default);
    public string ProcAllowance { get; set; } = nameof(ProcAllowanceMode.Default);
    public string ProcEligibilityRaw { get; set; } = string.Empty;
    public bool ProcMainTargetOnly { get; set; }
    public bool ProcIgnoreChainEffect { get; set; }
    public bool ProcIgnoreOverCap { get; set; }
    public bool? StackingLifetime { get; set; }
    public int? MaxPowerLifetime { get; set; }
    public int? MaxPowerLifetimeInGame { get; set; }
    public string BoostInfoJson { get; set; } = string.Empty;
    public List<string> AllowedBoostSetCategories { get; set; } = [];
    public ImportedArchetypeInherentBinding? ArchetypeInherent { get; set; }

    internal ImportedProcPolicy ProcPolicy
    {
        get => new(
            ImportedProcPolicyNormalizer.ParseStoredEligibility(ProcEligibility, ProcAllowance),
            ImportedProcPolicyNormalizer.ParseStoredAllowance(ProcAllowance),
            ProcMainTargetOnly,
            ProcIgnoreChainEffect,
            ProcIgnoreOverCap,
            ProcEligibilityRaw ?? string.Empty);
        set
        {
            ProcEligibility = value.Eligibility.ToString();
            ProcAllowance = value.Allowance.ToString();
            ProcEligibilityRaw = value.RawEligibilityValue ?? string.Empty;
            ProcMainTargetOnly = value.MainTargetOnly;
            ProcIgnoreChainEffect = value.IgnoreChainEffect;
            ProcIgnoreOverCap = value.IgnoreOverCap;
        }
    }

    internal ImportedPowerLifetimeMetadata LifetimeMetadata
    {
        get => new(MaxPowerLifetime, MaxPowerLifetimeInGame);
        set
        {
            MaxPowerLifetime = value.MaxPowerLifetime;
            MaxPowerLifetimeInGame = value.MaxPowerLifetimeInGame;
        }
    }

    internal ImportedBoostPolicyMetadata BoostPolicyMetadata
    {
        get => new(BoostInfoJson, AllowedBoostSetCategories);
        set
        {
            BoostInfoJson = value.RawBoostInfoJson;
            AllowedBoostSetCategories = value.AllowedBoostSetCategories.ToList();
        }
    }
}

public sealed class ImportedArchetypeInherentBinding
{
    public string CatalogKey { get; set; } = string.Empty;
    public OmniDataProviderId ProviderId { get; set; } = OmniDataProviderId.Unknown;
    public string OwningClassName { get; set; } = string.Empty;
    public string PowerFullName { get; set; } = string.Empty;
    public ArchetypeInherentPresentationType PresentationType { get; set; } = ArchetypeInherentPresentationType.InfoOnly;
    public ArchetypeInherentBehaviorModel BehaviorModel { get; set; } = ArchetypeInherentBehaviorModel.None;
    public ArchetypeInherentVisibilityRule VisibilityRule { get; set; } = ArchetypeInherentVisibilityRule.OnlyIfPresentInActiveDatabase;
    public ArchetypeInherentFallbackBehavior FallbackBehavior { get; set; } = ArchetypeInherentFallbackBehavior.InformationalOnly;
}

public sealed class EntityImportMetadata
{
    public Dictionary<string, List<string>> EntityTagsByUid { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class EnhancementImportMetadata
{
    public string SourceRoot { get; set; } = string.Empty;
    public string SourceVersionStamp { get; set; } = string.Empty;
    public JToken? EnhancementDiversification { get; set; }
    public JToken? EnhancementEffectiveness { get; set; }
    public JToken? EnhancementExemplarScaling { get; set; }
    public JToken? EnhancementSetGroups { get; set; }
    public JToken? SetConversions { get; set; }
    public Dictionary<string, string> EnhancementSourceCategories { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnhancementSourceDisplayNames { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnhancementSourceFamilies { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnhancementSourceIcons { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnhancementSetSourceKeys { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnhancementSetSourceGroups { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnhancementSetSourceIcons { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnhancementSetSourceRarityNames { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> SpecialFamilyCrosswalk { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> BoostPowerAliasCrosswalk { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> SetBonusPowerAliasCrosswalk { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnhancementAliasCrosswalk { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnhancementSetAliasCrosswalk { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> RecipeAliasCrosswalk { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> SalvageAliasCrosswalk { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> ClassicEnhancementCanonicalBySourceKey { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> ClassicEnhancementCanonicalByName { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> ClassicEnhancementFoldKeys { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, ClassicEnhancementSourceVariantMetadata> ClassicEnhancementSourcesByName { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, List<string>> ClassicEnhancementSourceNamesByCanonicalName { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> CategoryOnlyBoostPowerFullNames { get; set; } = [];
    public List<string> UnresolvedPowerLinks { get; set; } = [];
    public List<string> UnresolvedIconAliases { get; set; } = [];
    public List<string> UnconsumedBonusRequirements { get; set; } = [];
    public List<string> DeferredPowerPolicies { get; set; } = [];
    public List<string> ClassicEnhancementFoldAudit { get; set; } = [];
}

public sealed class ClassicEnhancementSourceVariantMetadata
{
    public string SourceKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Tier { get; set; } = string.Empty;
    public string NormalizedTier { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Flavor { get; set; } = string.Empty;
    public string PrimaryOrigin { get; set; } = string.Empty;
    public string SecondaryOrigin { get; set; } = string.Empty;
    public bool HasExplicitPrimaryOrigin { get; set; }
    public List<string> CompatibleOrigins { get; set; } = [];
}

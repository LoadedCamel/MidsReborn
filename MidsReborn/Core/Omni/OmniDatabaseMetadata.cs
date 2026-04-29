using System;
using System.Collections.Generic;
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
    public Dictionary<string, OmniClassAttributeTable> ClassAttributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public EnhancementImportMetadata EnhancementImport { get; set; } = new();
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

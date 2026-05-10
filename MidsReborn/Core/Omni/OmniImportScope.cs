namespace Mids_Reborn.Core.Omni;

public sealed class OmniImportScope
{
    private static readonly HashSet<string> RetainedNonPlayableArchetypeNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Class_Minion_Grunt",
        "Class_Lt_Grunt",
        "Class_Boss_Grunt",
        "Class_Boss_Elite",
        "Class_Boss_Archvillain",
        "Class_Boss_PraetorianGrunt",
        "Class_Boss_PraetorianElite",
        "Class_Boss_PraetorianArchvillain",
        "Class_Boss_Monster",
        "Class_Boss_ChallengeArchvillain",
        "Class_Boss_Hamidon",
        "Class_Boss_Mito",
        "Class_Minion_ControllerPets",
        "Class_Minion_Pets",
        "Class_Henchman_Minion",
        "Class_Henchman_Lt",
        "Class_Henchman_Boss",
        "Class_Henchman_Minion_Small",
        "Class_Lt_PraetorianGrunt_Pet",
        "Class_Boss_PraetorianGrunt_Pet"
    };

    private static readonly HashSet<string> ExcludedPowerFullNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Inherent.Inherent.Walk",
        "Inherent.Inherent.HAC_Rez",
        "Inherent.Inherent.HAC__Standard",
        "Prestige.Prestige_Travel.prestige_Pocket_D_VIP_Pass",
        "Prestige.Prestige_Travel.prestige_Base_Teleport",
        "Prestige.Prestige_Travel.prestige_Base_Portal",
        "Prestige.Prestige_Travel.prestige_Mission_Teleport",
        "Prestige.Prestige_Travel.Team_Transporter",
        "Prestige.Prestige_Travel.prestige_Team_Recall",
        "Prestige.Prestige_Utility.Disable_All_Powers",
        "Prestige.Prestige_Utility.prestige_InspirationGrant",
        "Prestige.Prestige_Utility.Only_Affect_Self",
        "Prestige.Prestige_Utility.Portable_Workbench",
        "Prestige.Prestige_Utility.prestige_SelfRez",
        "Prestige.Prestige_Utility.prestige_reveal",
        "Prestige.Prestige_Utility.prestige_Permanent_SelfDestruction"
    };

    private static readonly HashSet<string> ExcludedPowersetFullNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Controller_Control.Wind_Control",
        "Dominator_Control.Wind_Control",
        "Pool.Fitness",
        "Temporary_Powers.Art_Test",
        "Prestige.Combat_Dummy",
        "Prestige.Prestige_Costumes",
        "Prestige.Fun",
        "Prestige.Vanity_Pets"
    };

    private readonly HashSet<string> _includedPowerRoots = new(StringComparer.OrdinalIgnoreCase)
    {
        "boosts",
        "epic",
        "incarnate",
        "incarnate_pets",
        "inherent",
        "kheldian_pets",
        "mastermind_pets",
        "pets",
        "pool",
        "prestige",
        "redirects",
        "set_bonus",
        "temporary_powers",
        "villain_pets",
        "arachnos_soldiers",
        "arachnos_widow",
        "blaster_ranged",
        "blaster_support",
        "brute_defense",
        "brute_melee",
        "controller_buff",
        "controller_control",
        "corruptor_buff",
        "corruptor_ranged",
        "defender_buff",
        "defender_ranged",
        "dominator_assault",
        "dominator_control",
        "mastermind_buff",
        "mastermind_summon",
        "peacebringer_defensive",
        "peacebringer_offensive",
        "scrapper_defense",
        "scrapper_melee",
        "sentinel_defense",
        "sentinel_ranged",
        "stalker_defense",
        "stalker_melee",
        "tanker_defense",
        "tanker_melee",
        "warshade_defensive",
        "warshade_offensive"
    };

    private readonly HashSet<string> _excludedIncarnateRoots = new(StringComparer.OrdinalIgnoreCase)
    {
        "incarnate_alphastrike",
        "incarnate_i20"
    };

    private readonly HashSet<string> _retainedArchetypes = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _retainedPlayableArchetypes = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _retainedEntityClasses = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _retainedEntityIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _retainedPowerFullNames = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _retainedPowerPowersets = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _retainedPowersets = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Enums.ePowerSetType> _categoryTypes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Enums.ePowerSetType> _powersetTypes = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> RetainedClassNames => _retainedArchetypes
        .Concat(_retainedEntityClasses)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();
    public IReadOnlyCollection<string> RetainedPowerFullNames => _retainedPowerFullNames;
    public IReadOnlyCollection<string> RetainedPowersets => _retainedPowersets;
    public IReadOnlyCollection<string> RetainedEntityIds => _retainedEntityIds;

    public OmniImportScope()
    {
        AddCategoryType("Pool", Enums.ePowerSetType.Pool);
        AddCategoryType("Inherent", Enums.ePowerSetType.Inherent);
        AddCategoryType("Epic", Enums.ePowerSetType.Ancillary);
        AddCategoryType("Incarnate", Enums.ePowerSetType.Incarnate);
        AddCategoryType("Redirects", Enums.ePowerSetType.Redirect);
        AddCategoryType("Set_Bonus", Enums.ePowerSetType.SetBonus);
        AddCategoryType("Boosts", Enums.ePowerSetType.Boost);
        AddCategoryType("Temporary_Powers", Enums.ePowerSetType.Temp);
        AddPowersetType("Temporary_Powers.Accolades", Enums.ePowerSetType.Accolade);
        AddCategoryType("Prestige", Enums.ePowerSetType.Temp);
    }

    public Enums.ePowerSetType GetPowersetType(string powersetFullName)
    {
        if (string.IsNullOrWhiteSpace(powersetFullName))
        {
            return Enums.ePowerSetType.None;
        }

        if (_powersetTypes.TryGetValue(powersetFullName, out var powersetType))
        {
            return powersetType;
        }

        var group = powersetFullName.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
        return _categoryTypes.TryGetValue(group, out var categoryType)
            ? categoryType
            : Enums.ePowerSetType.None;
    }

    public bool ShouldRetainArchetype(OmniArchetypeDefinition archetype)
    {
        return archetype != null && ShouldRetainArchetype(archetype.Playable, archetype.InternalName);
    }

    public bool ShouldRetainArchetype(bool playable, string className)
    {
        return playable || IsWhitelistedNonPlayableArchetype(className);
    }

    public bool IsRetainedArchetype(string className)
    {
        return !string.IsNullOrWhiteSpace(className) &&
               _retainedArchetypes.Contains(NormalizeClassName(className));
    }

    public bool IsPlayableArchetype(string className)
    {
        return !string.IsNullOrWhiteSpace(className) &&
               _retainedPlayableArchetypes.Contains(NormalizeClassName(className));
    }

    public bool IsRetainedClass(string className)
    {
        var normalized = NormalizeClassName(className);
        return !string.IsNullOrWhiteSpace(normalized) &&
               (_retainedArchetypes.Contains(normalized) || _retainedEntityClasses.Contains(normalized));
    }

    public bool IsRetainedPowerset(string powersetFullName)
    {
        return !string.IsNullOrWhiteSpace(powersetFullName) &&
               _retainedPowersets.Contains(powersetFullName);
    }

    public bool IsRetainedPowerPowerset(string powersetFullName)
    {
        return !string.IsNullOrWhiteSpace(powersetFullName) &&
               _retainedPowerPowersets.Contains(powersetFullName);
    }

    public bool IsRetainedPower(string powerFullName)
    {
        return !string.IsNullOrWhiteSpace(powerFullName) &&
               _retainedPowerFullNames.Contains(powerFullName);
    }

    public bool IsRetainedEntity(string entityName)
    {
        return !string.IsNullOrWhiteSpace(entityName) &&
               _retainedEntityIds.Contains(entityName);
    }

    public bool IsIncludedPowerRoot(string powersetOrFullName)
    {
        if (string.IsNullOrWhiteSpace(powersetOrFullName))
        {
            return false;
        }

        var root = powersetOrFullName
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? powersetOrFullName;
        return _includedPowerRoots.Contains(root);
    }

    public static bool IsWhitelistedNonPlayableArchetype(string className)
    {
        return !string.IsNullOrWhiteSpace(className) &&
               RetainedNonPlayableArchetypeNames.Contains(NormalizeClassName(className));
    }

    public void AddRetainedArchetype(OmniArchetypeDefinition archetype)
    {
        if (!ShouldRetainArchetype(archetype))
        {
            return;
        }

        var className = NormalizeClassName(archetype.InternalName);
        if (!string.IsNullOrWhiteSpace(className))
        {
            _retainedArchetypes.Add(className);
            if (archetype.Playable)
            {
                _retainedPlayableArchetypes.Add(className);
            }
        }

        if (!archetype.Playable)
        {
            return;
        }

        AddCategoryType(archetype.PrimaryCategory, Enums.ePowerSetType.Primary);
        AddCategoryType(archetype.SecondaryCategory, Enums.ePowerSetType.Secondary);
        AddCategoryType(archetype.PowerPoolCategory, Enums.ePowerSetType.Pool);
        AddCategoryType(archetype.EpicPoolCategory, Enums.ePowerSetType.Ancillary);
        AddPowersets(archetype.PrimaryPowersets, Enums.ePowerSetType.Primary);
        AddPowersets(archetype.SecondaryPowersets, Enums.ePowerSetType.Secondary);
        AddPowersets(archetype.EpicPowersets, Enums.ePowerSetType.Ancillary);
        AddPowersets(archetype.Powersets, Enums.ePowerSetType.None);
    }

    public void AddReferencedEntityClass(string className)
    {
        var normalized = NormalizeClassName(className);
        if (!string.IsNullOrWhiteSpace(normalized))
        {
            _retainedEntityClasses.Add(normalized);
        }
    }

    public void AddReferencedEntityId(string entityName)
    {
        if (!string.IsNullOrWhiteSpace(entityName))
        {
            _retainedEntityIds.Add(entityName);
        }
    }

    public void AddReferencedEntityPowersets(IEnumerable<string>? powersets)
    {
        foreach (var powerset in powersets ?? [])
        {
            if (string.IsNullOrWhiteSpace(powerset) || IsExcludedPowerset(powerset))
            {
                continue;
            }

            _retainedPowersets.Add(powerset);

            var resolvedType = ResolveReferencedEntityPowersetType(powerset);
            if (resolvedType != Enums.ePowerSetType.None)
            {
                _powersetTypes[powerset] = resolvedType;
            }
        }
    }

    public void AddRetainedPower(string powerFullName)
    {
        if (string.IsNullOrWhiteSpace(powerFullName))
        {
            return;
        }

        if (!_retainedPowerFullNames.Add(powerFullName))
        {
            return;
        }

        var fullSetName = FullSetNameFromPowerFullName(powerFullName);
        if (!string.IsNullOrWhiteSpace(fullSetName) && !IsExcludedPowerset(fullSetName))
        {
            _retainedPowerPowersets.Add(fullSetName);
        }
    }

    private void AddPowersets(IEnumerable<string> powersets, Enums.ePowerSetType setType)
    {
        foreach (var powerset in powersets)
        {
            if (!string.IsNullOrWhiteSpace(powerset) && !IsExcludedPowerset(powerset))
            {
                _retainedPowersets.Add(powerset);
                if (setType != Enums.ePowerSetType.None)
                {
                    _powersetTypes[powerset] = setType;
                }
            }
        }
    }

    private Enums.ePowerSetType ResolveReferencedEntityPowersetType(string powersetFullName)
    {
        var existingType = GetPowersetType(powersetFullName);
        if (existingType != Enums.ePowerSetType.None)
        {
            return existingType;
        }

        var root = powersetFullName
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? powersetFullName;
        return IsPetRoot(root)
            ? Enums.ePowerSetType.Pet
            : Enums.ePowerSetType.None;
    }

    private void AddCategoryType(string category, Enums.ePowerSetType setType)
    {
        if (string.IsNullOrWhiteSpace(category) || setType == Enums.ePowerSetType.None)
        {
            return;
        }

        _categoryTypes[category] = setType;
    }

    private void AddPowersetType(string powerset, Enums.ePowerSetType setType)
    {
        if (string.IsNullOrWhiteSpace(powerset) || setType == Enums.ePowerSetType.None)
        {
            return;
        }

        _powersetTypes[powerset] = setType;
    }

    public bool RetainsAnyArchetype(IEnumerable<string>? archetypes)
    {
        var sawArchetype = false;
        foreach (var archetype in archetypes ?? [])
        {
            if (string.IsNullOrWhiteSpace(archetype))
            {
                continue;
            }

            sawArchetype = true;
            if (IsRetainedClass(archetype))
            {
                return true;
            }
        }

        return !sawArchetype;
    }

    public bool IsPowerFileInScope(string relativePath, string powersetFullName, IEnumerable<string>? archetypes = null)
    {
        return IsPowerFileInScope(relativePath, powersetFullName, archetypes, null);
    }

    public bool IsPowerFileInScope(
        string relativePath,
        string powersetFullName,
        IEnumerable<string>? archetypes,
        string? powerFullName)
    {
        var normalized = relativePath.Replace('\\', '/');
        var parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return false;
        }

        var root = parts[0];
        if (_excludedIncarnateRoots.Contains(root))
        {
            return false;
        }

        powerFullName = string.IsNullOrWhiteSpace(powerFullName)
            ? PowerFullNameFromParts(parts)
            : powerFullName;
        if (ExcludedPowerFullNames.Contains(powerFullName))
        {
            return false;
        }

        if (IsExcludedPowerset(powersetFullName) || IsExcludedPowerset(FullSetNameFromPowerFullName(powerFullName)))
        {
            return false;
        }

        var explicitlyRetainedPower = !string.IsNullOrWhiteSpace(powerFullName) &&
                                      _retainedPowerFullNames.Contains(powerFullName);
        var explicitlyRetainedPowerset = !string.IsNullOrWhiteSpace(powersetFullName) &&
                                         _retainedPowersets.Contains(powersetFullName);
        if (IsPetRoot(root) && !explicitlyRetainedPowerset && !explicitlyRetainedPower)
        {
            return false;
        }

        if (!explicitlyRetainedPower && !explicitlyRetainedPowerset && !RetainsAnyArchetype(archetypes))
        {
            return false;
        }

        return _includedPowerRoots.Contains(root) || explicitlyRetainedPowerset || explicitlyRetainedPower;
    }

    public bool IsPowersetFileInScope(string relativePath, string powersetFullName, IEnumerable<string>? archetypes = null)
    {
        var normalized = relativePath.Replace('\\', '/');
        var parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return false;
        }

        var root = parts[0];
        if (_excludedIncarnateRoots.Contains(root))
        {
            return false;
        }

        if (IsExcludedPowerset(powersetFullName))
        {
            return false;
        }

        var explicitlyRetainedPowerPowerset = !string.IsNullOrWhiteSpace(powersetFullName) &&
                                              _retainedPowerPowersets.Contains(powersetFullName);
        var explicitlyRetainedPowerset = !string.IsNullOrWhiteSpace(powersetFullName) &&
                                         _retainedPowersets.Contains(powersetFullName);
        if (IsPetRoot(root) && !explicitlyRetainedPowerset)
        {
            return false;
        }

        if (!explicitlyRetainedPowerset && !explicitlyRetainedPowerPowerset && !RetainsAnyArchetype(archetypes))
        {
            return false;
        }

        return _includedPowerRoots.Contains(root) || explicitlyRetainedPowerset || explicitlyRetainedPowerPowerset;
    }

    public static bool IsExcludedPowerset(string powersetFullName)
    {
        return !string.IsNullOrWhiteSpace(powersetFullName) &&
               ExcludedPowersetFullNames.Contains(powersetFullName);
    }

    public static bool IsExcludedPower(string powerFullName)
    {
        return !string.IsNullOrWhiteSpace(powerFullName) &&
               ExcludedPowerFullNames.Contains(powerFullName);
    }

    private static string PowerFullNameFromParts(IReadOnlyList<string> parts)
    {
        if (parts.Count < 3)
        {
            return string.Empty;
        }

        return $"{parts[^3]}.{parts[^2]}.{Path.GetFileNameWithoutExtension(parts[^1])}";
    }

    private static string FullSetNameFromPowerFullName(string powerFullName)
    {
        if (string.IsNullOrWhiteSpace(powerFullName))
        {
            return string.Empty;
        }

        var lastDot = powerFullName.LastIndexOf('.');
        return lastDot > 0 ? powerFullName[..lastDot] : string.Empty;
    }

    private static bool IsPetRoot(string root)
    {
        return NormalizeToken(root) is
            "incarnatepets" or
            "kheldianpets" or
            "mastermindpets" or
            "pets" or
            "villainpets";
    }

    private static string NormalizeToken(string value)
    {
        return (value ?? string.Empty)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Trim()
            .ToLowerInvariant();
    }

    public static string NormalizeClassName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var name = value.Trim();
        if (name.StartsWith("class_", StringComparison.OrdinalIgnoreCase))
        {
            return "Class_" + name["class_".Length..];
        }

        return name;
    }
}

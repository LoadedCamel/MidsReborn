namespace Mids_Reborn.Core.Omni;

public sealed class OmniImportScope
{
    private static readonly HashSet<string> ExcludedPowerFullNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Inherent.Inherent.Walk",
        "Inherent.Inherent.HAC_Rez",
        "Inherent.Inherent.HAC__Standard"
    };

    private static readonly HashSet<string> ExcludedPowersetFullNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Controller_Control.Wind_Control",
        "Dominator_Control.Wind_Control",
        "Pool.Fitness"
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

    private readonly HashSet<string> _playablePowersets = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Enums.ePowerSetType> _categoryTypes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Enums.ePowerSetType> _powersetTypes = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> PlayablePowersets => _playablePowersets;

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

    public void AddPlayableArchetype(OmniArchetypeDefinition archetype)
    {
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

    private void AddPowersets(IEnumerable<string> powersets, Enums.ePowerSetType setType)
    {
        foreach (var powerset in powersets)
        {
            if (!string.IsNullOrWhiteSpace(powerset) && !IsExcludedPowerset(powerset))
            {
                _playablePowersets.Add(powerset);
                if (setType != Enums.ePowerSetType.None)
                {
                    _powersetTypes[powerset] = setType;
                }
            }
        }
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

    public bool IsPowerFileInScope(string relativePath, string powersetFullName)
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

        var powerFullName = PowerFullNameFromParts(parts);
        if (ExcludedPowerFullNames.Contains(powerFullName))
        {
            return false;
        }

        if (IsExcludedPowerset(powersetFullName) || IsExcludedPowerset(FullSetNameFromPowerFullName(powerFullName)))
        {
            return false;
        }

        return _includedPowerRoots.Contains(root) ||
               (!string.IsNullOrWhiteSpace(powersetFullName) && _playablePowersets.Contains(powersetFullName));
    }

    public bool IsPowersetFileInScope(string relativePath, string powersetFullName)
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

        return _includedPowerRoots.Contains(root) ||
               (!string.IsNullOrWhiteSpace(powersetFullName) && _playablePowersets.Contains(powersetFullName));
    }

    public static bool IsExcludedPowerset(string powersetFullName)
    {
        return !string.IsNullOrWhiteSpace(powersetFullName) &&
               ExcludedPowersetFullNames.Contains(powersetFullName);
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
}

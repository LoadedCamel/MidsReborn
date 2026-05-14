using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

public static class SpecialPowerCatalog
{
    public const string AllChipKey = "all";
    public const string OtherChipKey = "other";
    public const string TempPowersChipKey = "temp_powers";
    public const string DayJobChipKey = "day_job";
    private const string AccoladeDestinationKey = "accolade";
    private const string PrestigeDestinationKey = "prestige";

    private static readonly string[] IncarnateDisplayOrder =
    [
        "Alpha",
        "Judgement",
        "Interface",
        "Lore",
        "Destiny",
        "Hybrid",
        "Genesis",
        "Omega",
        "Stance",
        "Vitae"
    ];

    private static readonly IReadOnlyList<(string Key, string Label)> PrestigeChipDisplayOrder =
    [
        ("combatpets", "Combat Pets"),
        ("prestigeattacks", "Attacks"),
        ("prestigesprints", "Sprints"),
        ("prestigetravel", "Travel"),
        ("prestigeutility", "Utility"),
        (OtherChipKey, "Other")
    ];

    private static readonly IReadOnlyList<(string Key, string Label)> TempChipDisplayOrder =
    [
        (TempPowersChipKey, "Temp Powers"),
        (DayJobChipKey, "Day Job")
    ];

    private static readonly HashSet<string> LegacyBackfillTempPowerDisplayWhitelist = new(StringComparer.OrdinalIgnoreCase)
    {
        "Anger Monument",
        "Arachnos Power Shield",
        "Biological Mutagens",
        "Call to Justice",
        "Cimeroran",
        "Combat Shield",
        "Confusion Resistance",
        "Crey Narcotic",
        "Cryonite Armor",
        "Dimensional Shield",
        "Disorient Resistance",
        "Endurance Discount",
        "Endurance Drain Resistance",
        "Endurance Increase",
        "Energy Resistance",
        "Fear Resistance",
        "Fire Resistance",
        "Health Increase",
        "Heart of a Storm Elemental",
        "Hold Resistance",
        "Immobilize Resistance",
        "Increase Attack Speed",
        "Increase Flight Speed",
        "Increase Jump Speed",
        "Increase Perception",
        "Increase Recovery",
        "Increase Run Speed",
        "Kinetic Shield",
        "Knockback Increase",
        "Knockback Protection",
        "Lethal Resistance",
        "Monument of Iron",
        "Movement Increase",
        "Offense Amplifier",
        "Raptor Pack",
        "Regeneration Increase",
        "Sleep Resistance",
        "Slow Resistance",
        "Smashing Resistance",
        "Survival Amplifier",
        "The Perfect Eye",
        "The True Furnace",
        "Toxic Resistance",
        "Wedding Band",
        "Wedding Band (Echo)",
        "Zero-G Pack"
    };

    private static readonly HashSet<string> LegacyBackfillDayJobPowerDisplayWhitelist = new(StringComparer.OrdinalIgnoreCase)
    {
        "Cimeroran",
        "Cimeroran Citizen",
        "Cold Hand of Death",
        "Combat Shield",
        "Commuter",
        "Duelist",
        "Elite Ranger",
        "Experienced Pilot",
        "Expert Duelist",
        "Frequent Commuter",
        "Frozen Hand of Death",
        "Master Predator",
        "Master Psychologist",
        "Pilot",
        "Predator",
        "Psychologist",
        "Ranger",
        "Time Lord's Boon"
    };

    public static IReadOnlyList<IPower> GetPowers(
        SpecialPowerCategory category,
        int classId,
        bool isHero,
        IEnumerable<IPower?>? explicitPowers = null,
        string? incarnateSetName = null)
    {
        return category switch
        {
            SpecialPowerCategory.Accolade => GetAccoladePowers(classId, isHero),
            SpecialPowerCategory.Prestige => GetPrestigePowers(classId),
            SpecialPowerCategory.Temp => GetTempPowers(classId),
            SpecialPowerCategory.Incarnate => GetIncarnatePowers(incarnateSetName, classId),
            SpecialPowerCategory.Subset => GetSubsetPowers(explicitPowers),
            _ => []
        };
    }

    public static bool CanAssignToSpecialPowerPicker(IPower? power)
    {
        return power is not null &&
               power.IncludeFlag &&
               TryGetSpecialPowerPickerDestination(power, out _, out _);
    }

    public static string GetSpecialPowerPickerDestinationLabel(IPower? power)
    {
        return power is not null &&
               TryGetSpecialPowerPickerDestination(power, out _, out var destinationLabel)
            ? destinationLabel
            : "N/A";
    }

    public static bool ShouldBackfillSpecialPowerPicker(IPower? power)
    {
        if (power is null ||
            power.HiddenPower ||
            !power.IncludeFlag ||
            !TryGetSpecialPowerPickerDestination(power, out var destinationKey, out _))
        {
            return false;
        }

        return destinationKey switch
        {
            AccoladeDestinationKey => IsLegacySelectableSpecialPower(power),
            PrestigeDestinationKey => true,
            TempPowersChipKey => IsLegacySelectableSpecialPower(power) &&
                                 LegacyBackfillTempPowerDisplayWhitelist.Contains(power.DisplayName),
            DayJobChipKey => IsLegacySelectableSpecialPower(power) &&
                             LegacyBackfillDayJobPowerDisplayWhitelist.Contains(power.DisplayName),
            _ => false
        };
    }

    public static IReadOnlyList<IPower> GetAccoladePowers(int classId, bool isHero)
    {
        return FilterPickerPowers(
                DatabaseAPI.Database.Power.OfType<IPower>()
                    .Where(power => MatchesSpecialPowerPicker(power, AccoladeDestinationKey)))
            .OrderBy(power => power.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static IReadOnlyList<IPower> GetPrestigePowers(int classId)
    {
        return GetVisiblePrestigePowers(classId)
            .OrderBy(power => power.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static IReadOnlyList<SpecialPowerChipGroup> GetPrestigePowerGroups(int classId)
    {
        var visiblePowers = GetVisiblePrestigePowers(classId);
        if (visiblePowers.Count == 0)
        {
            return [];
        }

        var grouped = visiblePowers
            .GroupBy(GetPrestigeChipKey)
            .ToDictionary(
                grouping => grouping.Key,
                grouping => (IReadOnlyList<IPower>)grouping
                    .OrderBy(power => power.DisplayName, StringComparer.OrdinalIgnoreCase)
                    .ToList(),
                StringComparer.OrdinalIgnoreCase);

        var groups = new List<SpecialPowerChipGroup>(PrestigeChipDisplayOrder.Count);
        foreach (var (key, label) in PrestigeChipDisplayOrder)
        {
            if (!grouped.TryGetValue(key, out var powers) || powers.Count == 0)
            {
                continue;
            }

            groups.Add(new SpecialPowerChipGroup(key, label, powers));
        }

        return groups;
    }

    public static IReadOnlyList<IPower> GetTempPowers(int classId)
    {
        return GetTempPowerGroups(classId)
            .FirstOrDefault(group => string.Equals(group.Key, TempPowersChipKey, StringComparison.OrdinalIgnoreCase))
            ?.Powers ?? [];
    }

    public static IReadOnlyList<SpecialPowerChipGroup> GetTempPowerGroups(int classId)
    {
        var visiblePowers = FilterPickerPowers(
                DatabaseAPI.Database.Power.OfType<IPower>()
                    .Where(power => MatchesSpecialPowerPicker(power, TempPowersChipKey) ||
                                    MatchesSpecialPowerPicker(power, DayJobChipKey)))
            .ToList();

        if (visiblePowers.Count == 0)
        {
            return [];
        }

        var grouped = new Dictionary<string, IReadOnlyList<IPower>>(StringComparer.OrdinalIgnoreCase)
        {
            [TempPowersChipKey] = visiblePowers
                .Where(power => MatchesSpecialPowerPicker(power, TempPowersChipKey))
                .OrderBy(power => power.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList(),
            [DayJobChipKey] = visiblePowers
                .Where(power => MatchesSpecialPowerPicker(power, DayJobChipKey))
                .OrderBy(power => power.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList()
        };

        var groups = new List<SpecialPowerChipGroup>(TempChipDisplayOrder.Count);
        foreach (var (key, label) in TempChipDisplayOrder)
        {
            if (!grouped.TryGetValue(key, out var powers) || powers.Count == 0)
            {
                continue;
            }

            groups.Add(new SpecialPowerChipGroup(key, label, powers));
        }

        return groups;
    }

    public static IReadOnlyList<IPowerset> GetEnabledIncarnateSets(int classId)
    {
        var enabledLookup = DatabaseAPI.ServerData?.EnabledIncarnates;
        if (enabledLookup == null)
        {
            return [];
        }

        return DatabaseAPI.Database.Powersets
            .OfType<IPowerset>()
            .Where(powerset => powerset.SetType == Enums.ePowerSetType.Incarnate)
            .GroupBy(powerset => powerset.DisplayName, StringComparer.OrdinalIgnoreCase)
            .Select(group => ChooseBestIncarnatePowerset(group, classId))
            .Where(powerset => powerset != null)
            .Where(powerset => enabledLookup.TryGetValue(powerset!.DisplayName, out var enabled) && enabled)
            .Where(powerset => FilterSelectableIncarnatePowers(powerset!.Powers, classId).Any())
            .Cast<IPowerset>()
            .OrderBy(powerset => GetIncarnateOrderIndex(powerset.DisplayName))
            .ThenBy(powerset => powerset.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static IReadOnlyList<IPower> GetIncarnatePowers(string? incarnateSetName, int classId)
    {
        if (string.IsNullOrWhiteSpace(incarnateSetName))
        {
            return [];
        }

        var powerset = FindIncarnatePowerset(incarnateSetName, classId);

        return powerset?.Powers is { Length: > 0 } powers
            ? FilterSelectableIncarnatePowers(powers, classId).ToList()
            : [];
    }

    public static IReadOnlyList<IPower> GetSubsetPowers(IEnumerable<IPower?>? powers)
    {
        return powers?
            .Where(power => power != null)
            .Cast<IPower>()
            .ToList() ?? [];
    }

    private static IEnumerable<IPower> FilterVisiblePowers(IEnumerable<IPower?> powers, int classId)
    {
        return powers
            .Where(power => power is not null &&
                            power.IncludeFlag &&
                            !power.HiddenPower &&
                            (classId < 0 || power.AllowedForClass(classId)))
            .Cast<IPower>();
    }

    private static IEnumerable<IPower> FilterPickerPowers(IEnumerable<IPower?> powers)
    {
        return powers
            .Where(power => power is not null && power.IncludeFlag)
            .Cast<IPower>();
    }

    private static bool IsLegacySelectableSpecialPower(IPower power)
    {
        return power.ClickBuff ||
               power.PowerType == Enums.ePowerType.Auto_ ||
               power.PowerType == Enums.ePowerType.Toggle;
    }

    private static bool IsTempPowerChipMatch(IPower power)
    {
        return power.FullName.StartsWith("Temporary_Powers.Temporary_Powers.", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(power.GroupName, "Temporary_Powers", StringComparison.OrdinalIgnoreCase) &&
               string.Equals(power.SetName, "Temporary_Powers", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(power.GetPowerSet()?.FullName, "Temporary_Powers.Temporary_Powers", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDayJobChipMatch(IPower power)
    {
        return power.FullName.StartsWith("Temporary_Powers.Day_Job_Powers.", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(power.GroupName, "Temporary_Powers", StringComparison.OrdinalIgnoreCase) &&
               string.Equals(power.SetName, "Day_Job_Powers", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(power.GetPowerSet()?.FullName, "Temporary_Powers.Day_Job_Powers", StringComparison.OrdinalIgnoreCase);
    }

    private static IEnumerable<IPower> FilterSelectableIncarnatePowers(IEnumerable<IPower?> powers, int classId)
    {
        return powers
            .Where(power => power is not null &&
                            !power.HiddenPower &&
                            !string.Equals(power.DisplayName, "Nothing", StringComparison.OrdinalIgnoreCase) &&
                            (classId < 0 || power.AllowedForClass(classId)))
            .Cast<IPower>();
    }

    private static bool IsPrestigeFallbackPower(IPower power)
    {
        if (power.InherentType == Enums.eGridType.Prestige)
        {
            return true;
        }

        if (power.FullName.StartsWith("Prestige.", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(power.GroupName, "Prestige", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var powerset = power.GetPowerSet();
        return powerset != null &&
               (powerset.FullName.StartsWith("Prestige.", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(powerset.GroupName, "Prestige", StringComparison.OrdinalIgnoreCase));
    }

    private static List<IPower> GetVisiblePrestigePowers(int classId)
    {
        return FilterPickerPowers(
                DatabaseAPI.Database.Power.OfType<IPower>()
                    .Where(power => MatchesSpecialPowerPicker(power, PrestigeDestinationKey)))
            .ToList();
    }

    private static bool MatchesSpecialPowerPicker(IPower power, string destinationKey)
    {
        return power.ShowInSpecialPowerPicker &&
               TryGetSpecialPowerPickerDestination(power, out var candidateKey, out _) &&
               string.Equals(candidateKey, destinationKey, StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryGetSpecialPowerPickerDestination(
        IPower power,
        out string destinationKey,
        out string destinationLabel)
    {
        destinationKey = string.Empty;
        destinationLabel = string.Empty;

        if (IsAccoladePickerMatch(power))
        {
            destinationKey = AccoladeDestinationKey;
            destinationLabel = "Accolade";
            return true;
        }

        if (IsPrestigeFallbackPower(power))
        {
            destinationKey = PrestigeDestinationKey;
            destinationLabel = "Prestige";
            return true;
        }

        if (power.InherentType == Enums.eGridType.Temp)
        {
            if (IsDayJobChipMatch(power))
            {
                destinationKey = DayJobChipKey;
                destinationLabel = "Day Job";
                return true;
            }

            if (IsTempPowerChipMatch(power))
            {
                destinationKey = TempPowersChipKey;
                destinationLabel = "Temp Powers";
                return true;
            }
        }

        return false;
    }

    private static bool IsAccoladePickerMatch(IPower power)
    {
        return power.InherentType == Enums.eGridType.Accolade ||
               power.FullName.StartsWith("Temporary_Powers.Accolades.", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(power.SetName, "Accolades", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(power.GetPowerSet()?.FullName, "Temporary_Powers.Accolades", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetPrestigeChipKey(IPower power)
    {
        var normalizedSetName = NormalizeName(power.GetPowerSet()?.SetName);
        if (IsKnownPrestigeChipKey(normalizedSetName))
        {
            return normalizedSetName;
        }

        var normalizedPowerSetName = NormalizeName(power.SetName);
        if (IsKnownPrestigeChipKey(normalizedPowerSetName))
        {
            return normalizedPowerSetName;
        }

        return OtherChipKey;
    }

    private static bool IsKnownPrestigeChipKey(string normalizedSetName)
    {
        return normalizedSetName is
            "combatpets" or
            "prestigeattacks" or
            "prestigesprints" or
            "prestigetravel" or
            "prestigeutility";
    }

    private static string NormalizeName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
    }

    private static int GetIncarnateOrderIndex(string displayName)
    {
        var index = Array.FindIndex(
            IncarnateDisplayOrder,
            entry => string.Equals(entry, displayName, StringComparison.OrdinalIgnoreCase));

        return index >= 0 ? index : int.MaxValue;
    }

    private static IPowerset? FindIncarnatePowerset(string incarnateSetName, int classId)
    {
        var candidates = DatabaseAPI.Database.Powersets
            .OfType<IPowerset>()
            .Where(
                powerset => powerset.SetType == Enums.ePowerSetType.Incarnate &&
                            (string.Equals(powerset.DisplayName, incarnateSetName, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(powerset.SetName, incarnateSetName, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(powerset.FullName, incarnateSetName, StringComparison.OrdinalIgnoreCase)));

        return ChooseBestIncarnatePowerset(candidates, classId);
    }

    private static IPowerset? ChooseBestIncarnatePowerset(IEnumerable<IPowerset> candidates, int classId)
    {
        return candidates
            .Select(
                powerset => new
                {
                    Powerset = powerset,
                    SelectableCount = FilterSelectableIncarnatePowers(powerset.Powers, classId).Count(),
                    NonHiddenCount = powerset.Powers.Count(power => power is not null && !power.HiddenPower),
                    AuxiliaryPenalty = IsIncarnateAuxiliaryPowerset(powerset) ? 1 : 0
                })
            .OrderByDescending(entry => entry.SelectableCount)
            .ThenByDescending(entry => entry.NonHiddenCount)
            .ThenBy(entry => entry.AuxiliaryPenalty)
            .ThenBy(entry => entry.Powerset.FullName.Length)
            .Select(entry => entry.Powerset)
            .FirstOrDefault();
    }

    private static bool IsIncarnateAuxiliaryPowerset(IPowerset powerset)
    {
        return powerset.FullName.Contains("_Silent", StringComparison.OrdinalIgnoreCase) ||
               powerset.FullName.Contains("_Rez", StringComparison.OrdinalIgnoreCase) ||
               powerset.FullName.Contains("Lore_Pet_", StringComparison.OrdinalIgnoreCase);
    }
}

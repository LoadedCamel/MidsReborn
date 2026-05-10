using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

public static class SpecialPowerCatalog
{
    public const string AllChipKey = "all";
    public const string OtherChipKey = "other";

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

    private static readonly IReadOnlyList<(string Hero, string Villain)> AccoladeCounterparts =
    [
        ("The Atlas Medallion", "Marshal"),
        ("Portal Jockey", "Born In Battle"),
        ("Task Force Commander", "Invader"),
        ("Freedom Phalanx Reserve", "High Pain Threshold"),
        ("Eye of the Magus", "Demonic Aura"),
        ("Vanguard Medal", "Megalomaniac"),
        ("Geas of the Kind Ones", "Force of Nature")
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

    public static IReadOnlyList<IPower> GetAccoladePowers(int classId, bool isHero)
    {
        var excludedNames = GetOppositeFactionAccoladeNames(isHero);

        return FilterVisiblePowers(
                DatabaseAPI.Database.Power.OfType<IPower>()
                    .Where(power => power.InherentType == Enums.eGridType.Accolade),
                classId)
            .Where(power => !excludedNames.Contains(power.DisplayName))
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
        return FilterVisiblePowers(
                DatabaseAPI.Database.Power.OfType<IPower>()
                    .Where(power => power.InherentType == Enums.eGridType.Temp),
                classId)
            .OrderBy(power => power.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();
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

    private static HashSet<string> GetOppositeFactionAccoladeNames(bool isHero)
    {
        return isHero
            ? AccoladeCounterparts.Select(pair => pair.Villain).ToHashSet(StringComparer.OrdinalIgnoreCase)
            : AccoladeCounterparts.Select(pair => pair.Hero).ToHashSet(StringComparer.OrdinalIgnoreCase);
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
        var classified = DatabaseAPI.Database.Power.OfType<IPower>()
            .Where(power => power.InherentType == Enums.eGridType.Prestige)
            .ToList();

        var source = classified.Count > 0
            ? classified
            : DatabaseAPI.Database.Power.OfType<IPower>()
                .Where(IsPrestigeFallbackPower);

        return FilterVisiblePowers(source, classId).ToList();
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

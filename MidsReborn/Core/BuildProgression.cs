using System;
using System.Collections.Generic;
using System.Linq;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

public enum GrantedSlotAssignmentScope
{
    RestrictedToTargetPower = 0,
    Flexible = 1
}

public enum SlotSourceKind
{
    AutoBase = 0,
    Bought = 1,
    Granted = 2
}

public sealed class CharacterLevelProgressionEntry
{
    public int Level { get; set; }
    public int PowerPickDelta { get; set; }
    public int BoughtSlotDelta { get; set; }
    public int PoolUnlockDelta { get; set; }
    public int EpicUnlockDelta { get; set; }
}

public sealed class PowerOwnedLevelProgressionEntry
{
    public int Level { get; set; }
    public int FreeEnhancementSlotCount { get; set; }
}

public sealed class GrantedSlotRule
{
    public string RuleId { get; set; } = string.Empty;
    public string TargetPowerFullName { get; set; } = string.Empty;
    public List<int> GrantLevels { get; set; } = [];
    public bool ConsumesBoughtSlotBudget { get; set; }
    public GrantedSlotAssignmentScope AssignmentScope { get; set; } = GrantedSlotAssignmentScope.RestrictedToTargetPower;
    public bool CanRemoveInLevelUp { get; set; }
    public bool CanRemoveInRespec { get; set; } = true;
}

public sealed class BuildProgressionMetadata
{
    public int MaxCharacterLevel { get; set; } = 49;
    public List<CharacterLevelProgressionEntry> CharacterLevels { get; set; } = [];
    public List<PowerOwnedLevelProgressionEntry> PowerOwnedLevels { get; set; } = [];
    public List<GrantedSlotRule> GrantedSlotRules { get; set; } = [];

    public bool HasCharacterLevels => CharacterLevels?.Count > 0;

    public static BuildProgressionMetadata CreateLegacy(
        IList<LevelMap> levels,
        ServerData? serverData,
        OmniDataProviderId providerId)
    {
        var metadata = new BuildProgressionMetadata
        {
            MaxCharacterLevel = Math.Max(0, levels.Count - 1),
            PowerOwnedLevels =
            [
                new PowerOwnedLevelProgressionEntry
                {
                    Level = 1,
                    FreeEnhancementSlotCount = 1
                }
            ]
        };

        for (var index = 0; index < levels.Count; index++)
        {
            var row = levels[index];
            metadata.CharacterLevels.Add(new CharacterLevelProgressionEntry
            {
                Level = index + 1,
                // Mids stores one of the two starting level-1 power choices outside LevelMap.
                PowerPickDelta = row.Powers + (index == 0 ? 1 : 0),
                BoughtSlotDelta = row.Slots,
                PoolUnlockDelta = 0,
                EpicUnlockDelta = 0
            });
        }

        if (serverData?.EnableInherentSlotting == true)
        {
            AppendLegacyGrantedRule(
                metadata.GrantedSlotRules,
                providerId,
                "Inherent.Fitness.Health",
                serverData.HealthSlots,
                [serverData.HealthSlot1Level, serverData.HealthSlot2Level]);
            AppendLegacyGrantedRule(
                metadata.GrantedSlotRules,
                providerId,
                "Inherent.Fitness.Stamina",
                serverData.StaminaSlots,
                [serverData.StaminaSlot1Level, serverData.StaminaSlot2Level]);
        }

        return metadata;
    }

    private static void AppendLegacyGrantedRule(
        ICollection<GrantedSlotRule> rules,
        OmniDataProviderId providerId,
        string powerFullName,
        int expectedSlots,
        IEnumerable<int> rawGrantLevels)
    {
        var grantLevels = rawGrantLevels
            .Where(level => level > 0)
            .Distinct()
            .OrderBy(level => level)
            .Take(Math.Max(0, expectedSlots))
            .ToList();
        if (grantLevels.Count == 0)
        {
            return;
        }

        rules.Add(new GrantedSlotRule
        {
            RuleId = $"{providerId}:{powerFullName}",
            TargetPowerFullName = powerFullName,
            GrantLevels = grantLevels,
            ConsumesBoughtSlotBudget = false,
            AssignmentScope = GrantedSlotAssignmentScope.RestrictedToTargetPower,
            CanRemoveInLevelUp = false,
            CanRemoveInRespec = true
        });
    }
}

public sealed class BuildProgressionPolicy
{
    private static readonly IReadOnlyList<PowerOwnedLevelProgressionEntry> DefaultPowerOwnedRows =
    [
        new PowerOwnedLevelProgressionEntry
        {
            Level = 1,
            FreeEnhancementSlotCount = 1
        }
    ];

    private readonly Dictionary<string, GrantedSlotRule> _rulesById;
    private readonly List<PowerOwnedLevelProgressionEntry> _powerOwnedRows;
    private readonly List<CharacterLevelProgressionEntry> _rows;

    public BuildProgressionPolicy(BuildProgressionMetadata metadata)
    {
        Metadata = metadata ?? new BuildProgressionMetadata();
        _rows = (Metadata.CharacterLevels ?? new List<CharacterLevelProgressionEntry>())
            .Where(row => row.Level > 0)
            .OrderBy(row => row.Level)
            .ToList();
        _powerOwnedRows = (Metadata.PowerOwnedLevels ?? new List<PowerOwnedLevelProgressionEntry>())
            .Where(row => row is { Level: > 0, FreeEnhancementSlotCount: > 0 })
            .OrderBy(row => row.Level)
            .ToList();
        _rulesById = (Metadata.GrantedSlotRules ?? new List<GrantedSlotRule>())
            .Where(rule => !string.IsNullOrWhiteSpace(rule.RuleId))
            .GroupBy(rule => rule.RuleId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last(), StringComparer.OrdinalIgnoreCase);
    }

    public BuildProgressionMetadata Metadata { get; }
    public int MaxCharacterLevel => Math.Clamp(Metadata.MaxCharacterLevel, 0, 49);
    public IReadOnlyList<CharacterLevelProgressionEntry> CharacterLevels => _rows;
    public IReadOnlyList<PowerOwnedLevelProgressionEntry> PowerOwnedLevels => _powerOwnedRows;
    public IReadOnlyCollection<GrantedSlotRule> GrantedSlotRules => _rulesById.Values;

    public readonly record struct GrantedSlotGrant(string RuleId, string TargetPowerFullName, int UnlockLevel);

    public LevelMap[] CreateChronologyLevels()
    {
        var levels = new LevelMap[MaxCharacterLevel + 1];
        for (var index = 0; index < levels.Length; index++)
        {
            levels[index] = new LevelMap(0, 0);
        }

        foreach (var row in _rows)
        {
            var levelIndex = row.Level - 1;
            if (levelIndex < 0 || levelIndex >= levels.Length)
            {
                continue;
            }

            var additionalPowerPicks = row.PowerPickDelta - (levelIndex == 0 ? 1 : 0);
            levels[levelIndex] = new LevelMap(Math.Max(0, additionalPowerPicks), Math.Max(0, row.BoughtSlotDelta));
        }

        return levels;
    }

    public int[] CreateMainPowerLevels(LevelMap[] levels)
    {
        var intList = new List<int> { 0 };
        for (var index = 0; index < levels.Length; ++index)
        {
            if (levels[index].Powers > 0)
            {
                intList.Add(index);
            }
        }

        return intList.ToArray();
    }

    public int GetBoughtSlotCountAtLevel(int zeroBasedLevel)
    {
        return SumAtLevel(zeroBasedLevel, row => row.BoughtSlotDelta);
    }

    public int GetNormalPowerPickCountAtLevel(int zeroBasedLevel)
    {
        return SumAtLevel(zeroBasedLevel, row => row.PowerPickDelta);
    }

    public int GetPoolUnlockCountAtLevel(int zeroBasedLevel)
    {
        return SumAtLevel(zeroBasedLevel, row => row.PoolUnlockDelta);
    }

    public int GetEpicUnlockCountAtLevel(int zeroBasedLevel)
    {
        return SumAtLevel(zeroBasedLevel, row => row.EpicUnlockDelta);
    }

    public int GetInitialFreeEnhancementSlotCount()
    {
        return GetFreeEnhancementSlotCountForOwnedLevel(1);
    }

    public int GetFreeEnhancementSlotCountForOwnedLevel(int oneBasedOwnedLevel)
    {
        var ownedLevel = Math.Max(1, oneBasedOwnedLevel);
        IEnumerable<PowerOwnedLevelProgressionEntry> rows = _powerOwnedRows.Count > 0
            ? _powerOwnedRows
            : DefaultPowerOwnedRows;

        return rows
            .Where(row => row.Level <= ownedLevel)
            .Sum(row => Math.Max(0, row.FreeEnhancementSlotCount));
    }

    public int GetFreeEnhancementSlotCountForPowerLevel(int zeroBasedCharacterLevel, int zeroBasedPowerLevel)
    {
        var currentLevel = NormalizeCharacterLevel(zeroBasedCharacterLevel) + 1;
        var powerLevel = Math.Max(0, zeroBasedPowerLevel) + 1;
        return currentLevel < powerLevel
            ? 0
            : GetFreeEnhancementSlotCountForOwnedLevel(currentLevel - powerLevel + 1);
    }

    public int GetGrantedSlotCountAtLevel(int zeroBasedLevel)
    {
        var currentLevel = NormalizeCharacterLevel(zeroBasedLevel) + 1;
        return GrantedSlotRules.Sum(rule => rule.GrantLevels.Count(level => level <= currentLevel));
    }

    public int GetGrantedSlotCountForPower(string? powerFullName, int zeroBasedLevel)
    {
        if (string.IsNullOrWhiteSpace(powerFullName))
        {
            return 0;
        }

        var currentLevel = NormalizeCharacterLevel(zeroBasedLevel) + 1;
        return GrantedSlotRules
            .Where(rule => string.Equals(rule.TargetPowerFullName, powerFullName, StringComparison.OrdinalIgnoreCase))
            .Sum(rule => rule.GrantLevels.Count(level => level <= currentLevel));
    }

    public IEnumerable<GrantedSlotGrant> GetActiveGrantedSlotsForPower(string? powerFullName, int zeroBasedLevel)
    {
        if (string.IsNullOrWhiteSpace(powerFullName))
        {
            return [];
        }

        var currentLevel = NormalizeCharacterLevel(zeroBasedLevel) + 1;
        return GrantedSlotRules
            .Where(rule => string.Equals(rule.TargetPowerFullName, powerFullName, StringComparison.OrdinalIgnoreCase))
            .SelectMany(rule => rule.GrantLevels.Select(level =>
                new GrantedSlotGrant(rule.RuleId, rule.TargetPowerFullName, level)))
            .Where(entry => entry.UnlockLevel <= currentLevel)
            .OrderBy(entry => entry.UnlockLevel)
            .ToList();
    }

    public bool TryGetGrantedSlotRule(string? ruleId, out GrantedSlotRule rule)
    {
        if (!string.IsNullOrWhiteSpace(ruleId) && _rulesById.TryGetValue(ruleId, out rule))
        {
            return true;
        }

        rule = new GrantedSlotRule();
        return false;
    }

    public int GetTotalSlotsAvailableAtLevel(int zeroBasedLevel)
    {
        return GetBoughtSlotCountAtLevel(zeroBasedLevel) + GetGrantedSlotCountAtLevel(zeroBasedLevel);
    }

    public int NormalizeCharacterLevel(int zeroBasedLevel)
    {
        return Math.Clamp(zeroBasedLevel, 0, MaxCharacterLevel);
    }

    private int SumAtLevel(int zeroBasedLevel, Func<CharacterLevelProgressionEntry, int> selector)
    {
        var currentLevel = NormalizeCharacterLevel(zeroBasedLevel) + 1;
        var sum = 0;
        foreach (var row in _rows)
        {
            if (row.Level > currentLevel)
            {
                break;
            }

            sum += selector(row);
        }

        return sum;
    }
}

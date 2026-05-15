using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

public sealed class BuildCombatContextState
{
    public int EnemyRelativeLevel { get; set; } = int.MinValue;
    public Dictionary<string, int> TeamMembers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<ConfigData.TeammateSlot> TeamRoster { get; set; } = [];
    public ConfigData.CombatContext CombatContextSettings { get; set; } = new();
}

internal sealed record TeammateArchetypeDefinition(string Key, string DisplayName);

internal static class CombatContextState
{
    public static BuildCombatContextState CreateDefault()
    {
        var state = new BuildCombatContextState();
        Normalize(state);
        return state;
    }

    public static BuildCombatContextState Clone(BuildCombatContextState? source)
    {
        if (source == null)
        {
            return CreateDefault();
        }

        var clone = new BuildCombatContextState
        {
            EnemyRelativeLevel = source.EnemyRelativeLevel,
            TeamMembers = CloneTeamMembers(source.TeamMembers),
            TeamRoster = CloneTeamRoster(source.TeamRoster),
            CombatContextSettings = CloneCombatContext(source.CombatContextSettings)
        };

        Normalize(clone);
        return clone;
    }

    public static Dictionary<string, int> CloneTeamMembers(IDictionary<string, int>? source)
    {
        var clone = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (source == null)
        {
            return clone;
        }

        foreach (var (key, value) in source)
        {
            clone[key] = value;
        }

        return clone;
    }

    public static List<ConfigData.TeammateSlot> CloneTeamRoster(IEnumerable<ConfigData.TeammateSlot>? source)
    {
        if (source == null)
        {
            return [];
        }

        return source
            .Where(slot => slot != null)
            .Select(slot => new ConfigData.TeammateSlot
            {
                Archetype = slot.Archetype,
                InRange = slot.InRange,
                HpPercent = slot.HpPercent
            })
            .ToList();
    }

    public static ConfigData.CombatContext CloneCombatContext(ConfigData.CombatContext? source)
    {
        source ??= new ConfigData.CombatContext();
        return new ConfigData.CombatContext
        {
            PlayerSettings = new ConfigData.CombatContext.Player
            {
                HpPercent = source.PlayerSettings?.HpPercent ?? 100,
                EndPercent = source.PlayerSettings?.EndPercent ?? 100,
                IsAlive = source.PlayerSettings?.IsAlive ?? true
            },
            TargetSettings = new ConfigData.CombatContext.Target
            {
                HpPercent = source.TargetSettings?.HpPercent ?? 100,
                EndPercent = source.TargetSettings?.EndPercent ?? 100,
                ProfileId = source.TargetSettings?.ProfileId ?? (int)CombatTargetProfileId.Boss,
                Held = source.TargetSettings?.Held ?? false,
                Immobilized = source.TargetSettings?.Immobilized ?? false,
                Stunned = source.TargetSettings?.Stunned ?? false,
                Terrorized = source.TargetSettings?.Terrorized ?? false,
                SleptRecently = source.TargetSettings?.SleptRecently ?? false,
                VulnerabilityActive = source.TargetSettings?.VulnerabilityActive ?? false,
                OpportunityState = source.TargetSettings?.OpportunityState ?? (int)CombatTargetOpportunityState.None
            },
            Defiance = new ConfigData.CombatContext.DefianceSettings
            {
                Contributors = source.Defiance?.Contributors?
                    .Where(item => item != null)
                    .Select(item => new ConfigData.CombatContext.DefianceContributorSelection
                    {
                        SourcePowerFullName = item.SourcePowerFullName ?? string.Empty,
                        ResolvedPowerFullName = item.ResolvedPowerFullName ?? string.Empty,
                        ActiveCount = item.ActiveCount
                    })
                    .ToList() ?? []
            }
        };
    }

    public static BuildCombatContextState? TryGetActiveBuildState()
    {
        return MidsContext.Character?.CurrentBuild?.CombatContextState;
    }

    public static BuildCombatContextState GetActiveOrDefault()
    {
        var state = TryGetActiveBuildState();
        if (state != null)
        {
            Normalize(state);
            return state;
        }

        return CreateDefault();
    }

    public static void Normalize(BuildCombatContextState? state)
    {
        if (state == null)
        {
            return;
        }

        state.CombatContextSettings = CloneCombatContext(state.CombatContextSettings);
        NormalizeDefianceSelections(state.CombatContextSettings.Defiance);

        state.TeamMembers = NormalizeTeamMembers(state.TeamMembers);
        state.TeamRoster = DeriveRosterFromCounts(state.TeamMembers, state.TeamRoster);
        state.EnemyRelativeLevel = ConfigData.ClampEnemyRelativeLevel(
            ConfigData.NormalizeEnemyRelativeLevel(
                state.EnemyRelativeLevel,
                ConfigData.GetLegacyScalingToHitForRelativeLevel(0)));

        if (state.CombatContextSettings.TargetSettings.OpportunityState != (int)CombatTargetOpportunityState.None)
        {
            state.CombatContextSettings.TargetSettings.VulnerabilityActive = true;
        }
    }

    public static void NormalizeActiveBuildState()
    {
        Normalize(TryGetActiveBuildState());
    }

    public static Dictionary<string, int> NormalizeTeamMembers(IDictionary<string, int>? members)
    {
        var normalized = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (members == null)
        {
            return normalized;
        }

        var definitions = GetAvailableTeammateArchetypes();
        var definitionsByKey = definitions
            .ToDictionary(item => item.Key, item => item, StringComparer.OrdinalIgnoreCase);

        foreach (var (rawKey, rawValue) in members)
        {
            var key = ResolveTeammateArchetypeKey(rawKey);
            if (string.IsNullOrWhiteSpace(key) ||
                key.Equals("Any", StringComparison.OrdinalIgnoreCase) ||
                key.Equals(TeamContextDefaults.UnknownArchetype, StringComparison.OrdinalIgnoreCase) ||
                !definitionsByKey.ContainsKey(key))
            {
                continue;
            }

            var clamped = Math.Max(0, rawValue);
            if (clamped <= 0)
            {
                continue;
            }

            normalized[key] = clamped;
        }

        var orderedAndCapped = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var remaining = TeamContextDefaults.MaxTeammates;
        foreach (var definition in definitions)
        {
            if (remaining <= 0 || !normalized.TryGetValue(definition.Key, out var count))
            {
                continue;
            }

            var capped = Math.Min(count, remaining);
            if (capped <= 0)
            {
                continue;
            }

            orderedAndCapped[definition.Key] = capped;
            remaining -= capped;
        }

        return orderedAndCapped;
    }

    public static List<ConfigData.TeammateSlot> DeriveRosterFromCounts(
        IDictionary<string, int>? counts,
        IEnumerable<ConfigData.TeammateSlot>? existingRoster)
    {
        var normalizedCounts = NormalizeTeamMembers(counts);
        var existingByArchetype = CloneTeamRoster(existingRoster)
            .Where(slot => !string.IsNullOrWhiteSpace(slot.Archetype))
            .GroupBy(slot => ResolveTeammateArchetypeKey(slot.Archetype), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.OrdinalIgnoreCase);

        var derived = new List<ConfigData.TeammateSlot>(TeamContextDefaults.MaxTeammates);
        foreach (var definition in GetAvailableTeammateArchetypes())
        {
            if (!normalizedCounts.TryGetValue(definition.Key, out var count) || count <= 0)
            {
                continue;
            }

            existingByArchetype.TryGetValue(definition.Key, out var existingSlots);
            existingSlots ??= [];

            for (var ordinal = 0; ordinal < count && derived.Count < TeamContextDefaults.MaxTeammates; ordinal++)
            {
                ConfigData.TeammateSlot slot;
                if (ordinal < existingSlots.Count)
                {
                    slot = existingSlots[ordinal];
                }
                else
                {
                    slot = new ConfigData.TeammateSlot
                    {
                        Archetype = definition.Key,
                        InRange = true,
                        HpPercent = 100
                    };
                }

                slot.Archetype = definition.Key;
                slot.HpPercent = Math.Max(0, Math.Min(100, slot.HpPercent));
                derived.Add(slot);
            }
        }

        return derived;
    }

    public static Dictionary<string, int> BuildTeamMembersFromRoster(
        IEnumerable<ConfigData.TeammateSlot>? roster,
        bool inRangeOnly)
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (roster == null)
        {
            return counts;
        }

        foreach (var group in roster
                     .Where(slot => slot != null &&
                                    !string.IsNullOrWhiteSpace(slot.Archetype) &&
                                    (!inRangeOnly || slot.InRange))
                     .GroupBy(slot => ResolveTeammateArchetypeKey(slot.Archetype), StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(group.Key))
            {
                continue;
            }

            counts[group.Key] = group.Count();
        }

        return counts;
    }

    public static IReadOnlyList<TeammateArchetypeDefinition> GetAvailableTeammateArchetypes()
    {
        var definitions = new List<TeammateArchetypeDefinition>
        {
            new("Class_Blaster", "Blaster"),
            new("Class_Controller", "Controller"),
            new("Class_Defender", "Defender"),
            new("Class_Scrapper", "Scrapper"),
            new("Class_Tanker", "Tanker"),
            new("Class_Peacebringer", "Peacebringer"),
            new("Class_Warshade", "Warshade")
        };

        switch (DatabaseAPI.Database?.DataProviderId)
        {
            case OmniDataProviderId.OmniHomecoming:
                definitions.Add(new TeammateArchetypeDefinition("Class_Sentinel", "Sentinel"));
                break;
            case OmniDataProviderId.OmniRebirth:
                definitions.Add(new TeammateArchetypeDefinition("Class_Guardian", "Guardian"));
                break;
        }

        definitions.AddRange(
        [
            new TeammateArchetypeDefinition("Class_Brute", "Brute"),
            new TeammateArchetypeDefinition("Class_Stalker", "Stalker"),
            new TeammateArchetypeDefinition("Class_Mastermind", "Mastermind"),
            new TeammateArchetypeDefinition("Class_Dominator", "Dominator"),
            new TeammateArchetypeDefinition("Class_Corruptor", "Corruptor"),
            new TeammateArchetypeDefinition("Class_Arachnos_Soldier", "Arachnos Soldier"),
            new TeammateArchetypeDefinition("Class_Arachnos_Widow", "Arachnos Widow")
        ]);

        return definitions;
    }

    public static int GetDerivedTeamSize(BuildCombatContextState? state)
    {
        if (state == null)
        {
            return 1;
        }

        Normalize(state);
        return Math.Max(1, 1 + state.TeamMembers.Values.Sum());
    }

    public static string ResolveTeammateArchetypeKey(string? rawValue)
    {
        var normalized = ConfigData.NormalizeTeammateArchetype(rawValue);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        if (normalized.Equals("Any", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals(TeamContextDefaults.UnknownArchetype, StringComparison.OrdinalIgnoreCase))
        {
            return normalized;
        }

        foreach (var definition in GetAvailableTeammateArchetypes())
        {
            if (definition.Key.Equals(normalized, StringComparison.OrdinalIgnoreCase) ||
                definition.DisplayName.Equals(normalized, StringComparison.OrdinalIgnoreCase))
            {
                return definition.Key;
            }
        }

        return normalized;
    }

    private static void NormalizeDefianceSelections(ConfigData.CombatContext.DefianceSettings? settings)
    {
        settings ??= new ConfigData.CombatContext.DefianceSettings();
        settings.Contributors ??= [];
        settings.Contributors = settings.Contributors
            .Where(item => item != null &&
                           !string.IsNullOrWhiteSpace(item.SourcePowerFullName) &&
                           item.ActiveCount > 0)
            .GroupBy(item => item.SourcePowerFullName, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.Last())
            .ToList();
    }
}

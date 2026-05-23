using System;
using System.Linq;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

internal static class AssassinationPlanner
{
    private const int MaxFocusStacks = 3;
    private const float FocusStackChanceMagnitude = 0.333f;
    private const float TeamCritPerMemberChanceMagnitude = 0.03f;
    internal const string AssassinsStrikeChanceTag = "ASCrit";
    internal const string TeamCritChanceTag = "ASTeamCrit";

    public static int NormalizeFocusStacks(int value)
    {
        return Math.Max(0, Math.Min(MaxFocusStacks, value));
    }

    public static float GetFocusChanceBonusMagnitude(int focusStacks)
    {
        return NormalizeFocusStacks(focusStacks) * FocusStackChanceMagnitude;
    }

    public static float GetTeamCritChanceBonusMagnitude(int inRangeTeammates)
    {
        // Homecoming source counts the caster as part of TeamSize>radius, so solo
        // Stalkers still receive the base +3% ASTeamCrit modifier from Assassination.
        return (1 + Math.Max(0, inRangeTeammates)) * TeamCritPerMemberChanceMagnitude;
    }

    public static bool SupportsCurrentContext()
    {
        return IsStalkerArchetype();
    }

    public static IReadOnlyDictionary<string, float> BuildSupplementalChanceModifierCatalog(
        Build? build,
        ConfigData.CombatContext.AssassinationSettings? settings)
    {
        var catalog = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        if (!SupportsCurrentContext())
        {
            return catalog;
        }

        var focusChanceBonus = GetFocusChanceBonusMagnitude(settings?.FocusStacks ?? 0);
        if (focusChanceBonus > float.Epsilon)
        {
            catalog[AssassinsStrikeChanceTag] = focusChanceBonus;
        }

        if (ShouldInjectSupplementalTeamCritChance(build))
        {
            var teamCritChanceBonus = GetTeamCritChanceBonusMagnitude(GetInRangeTeammateCount());
            if (teamCritChanceBonus > float.Epsilon)
            {
                catalog[TeamCritChanceTag] = teamCritChanceBonus;
            }
        }

        return catalog;
    }

    public static void Synchronize(Build? build, ConfigData.CombatContext.AssassinationSettings? settings)
    {
        if (build?.Powers == null)
        {
            return;
        }

        var supported = SupportsCurrentContext();
        var focusStacks = supported
            ? NormalizeFocusStacks(settings?.FocusStacks ?? 0)
            : 0;

        if (settings != null)
        {
            settings.FocusStacks = focusStacks;
        }

        SyncPowerEntry(
            build,
            PlannerStateCatalog.AssassinationPowerFullName,
            focusStacks,
            isActive: true,
            forceVisiblePassive: true);

        SyncPowerEntry(
            build,
            PlannerStateCatalog.AssassinsFocusMarker,
            focusStacks,
            isActive: supported && focusStacks > 0);
    }

    private static void SyncPowerEntry(
        Build build,
        string powerFullName,
        int value,
        bool isActive,
        bool forceVisiblePassive = false)
    {
        var powerEntry = build.Powers.FirstOrDefault(entry =>
            entry?.Power?.FullName.Equals(powerFullName, StringComparison.OrdinalIgnoreCase) == true);
        if (powerEntry?.Power == null)
        {
            return;
        }

        powerEntry.VariableValue = value;
        powerEntry.StatInclude = isActive || forceVisiblePassive;
        powerEntry.Power.Active = (isActive || forceVisiblePassive) &&
                                  build.MeetsRequirement(powerEntry.Power, build.GetMaxLevel());
    }

    private static bool IsStalkerArchetype()
    {
        var archetype = MidsContext.Character?.Archetype ?? MidsContext.Archetype;
        if (archetype == null)
        {
            return false;
        }

        return archetype.DisplayName.Equals("Stalker", StringComparison.OrdinalIgnoreCase) ||
               archetype.ClassName.Equals("Class_Stalker", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ShouldInjectSupplementalTeamCritChance(Build? build)
    {
        var assassinationPower = build?.Powers?
            .FirstOrDefault(entry =>
                entry?.Power?.FullName.Equals(PlannerStateCatalog.AssassinationPowerFullName, StringComparison.OrdinalIgnoreCase) == true)?
            .Power;
        if (assassinationPower?.Effects == null)
        {
            return true;
        }

        foreach (var effect in assassinationPower.Effects)
        {
            if (effect.EffectType != Enums.eEffectType.GlobalChanceMod ||
                !string.Equals(effect.Reward, TeamCritChanceTag, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(effect.Expressions.Magnitude) &&
                effect.Expressions.Magnitude.Contains("TeamSize", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static int GetInRangeTeammateCount()
    {
        return MidsContext.Config?.TeamRoster?
            .Count(slot => slot.InRange && !string.IsNullOrWhiteSpace(slot.Archetype)) ?? 0;
    }
}

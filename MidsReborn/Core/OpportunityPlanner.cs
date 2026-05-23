using System;
using System.Linq;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

internal static class OpportunityPlanner
{
    private const int ReadyThreshold = 50;

    public static int NormalizeMeterPercent(int value)
    {
        return Math.Max(0, Math.Min(100, value));
    }

    public static bool IsReady(int meterPercent)
    {
        return NormalizeMeterPercent(meterPercent) >= ReadyThreshold;
    }

    public static bool SupportsCurrentContext()
    {
        return DatabaseAPI.GetDataProviderId() == OmniDataProviderId.OmniHomecoming &&
               IsSentinelArchetype();
    }

    public static void Synchronize(Build? build, ConfigData.CombatContext.OpportunitySettings? settings)
    {
        if (build?.Powers == null)
        {
            return;
        }

        var supported = SupportsCurrentContext();
        var meterPercent = supported
            ? NormalizeMeterPercent(settings?.MeterPercent ?? 0)
            : 0;

        if (settings != null)
        {
            settings.MeterPercent = meterPercent;
        }

        SyncPowerEntry(build, PlannerStateCatalog.OpportunityPowerFullName, meterPercent, isActive: true);
        SyncPowerEntry(build, PlannerStateCatalog.OpportunityMeterPowerFullName, meterPercent, isActive: supported);
    }

    private static void SyncPowerEntry(Build build, string powerFullName, int meterPercent, bool isActive)
    {
        var powerEntry = build.Powers.FirstOrDefault(entry =>
            entry?.Power?.FullName.Equals(powerFullName, StringComparison.OrdinalIgnoreCase) == true);
        if (powerEntry?.Power == null)
        {
            return;
        }

        powerEntry.VariableValue = meterPercent;
        powerEntry.StatInclude = isActive;
        powerEntry.Power.Active = isActive &&
                                  build.MeetsRequirement(powerEntry.Power, build.GetMaxLevel());
    }

    private static bool IsSentinelArchetype()
    {
        var archetype = MidsContext.Character?.Archetype ?? MidsContext.Archetype;
        if (archetype == null)
        {
            return false;
        }

        return archetype.DisplayName.Equals("Sentinel", StringComparison.OrdinalIgnoreCase) ||
               archetype.ClassName.Equals("Class_Sentinel", StringComparison.OrdinalIgnoreCase);
    }
}

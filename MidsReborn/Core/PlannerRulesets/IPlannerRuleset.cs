using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core.PlannerRulesets;

public interface IPlannerRuleset
{
    PlannerRulesetId Id { get; }
    bool UsesCanonicalPlannerMath { get; }
    bool AllowLegacyCrossPowerIncarnatePasses { get; }
    bool AllowRedirectSelectionInAssembly { get; }
    bool AllowPseudoPetAbsorptionInAssembly { get; }
    bool AllowGrantPowerExpansionInAssembly { get; }
    bool AllowExecutePowerExpansionInAssembly { get; }
    bool AllowSubPowerEffectsInAssembly { get; }
    PlannerEffectResolutionContext CreateDefaultResolutionContext();
    PlannerEffectResolutionContext CreateAssemblyExpansionContext(int historyIndex, int stackingOverride);
    void AccumulateBuckets(IPower power, ref Enums.BuffsX buckets, PlannerBucketPass pass);
    bool IncludePvpResistanceBonusInBuckets(PlannerBucketPass pass);
    bool ShouldProcessExecutesInDamageHelpers(IPower power);
    bool ShouldAbsorbPseudoPetEffectsForDamage(IPower power, bool absorbRequested);
    bool EffectMatchesCurrentMode(IEffect effect);
    float GetMinProcChance(float procsPerMinute);
    float GetMaxProcChance(float procsPerMinute);
    float CalculateProcProbability(IPower power, float procsPerMinute, float baseProbability);
    bool SupportsPowerLocalChanceMods { get; }
    bool SupportsRelativeLevelCombatModMath { get; }
    PlannerCombatContext ResolveCombatContext(ConfigData? config);
    int ResolveEffectiveCombatDelta(ConfigData? config);
    float GetCombatModToHitScale(PlannerCombatContext context);
    float GetCombatModAccuracyScale(PlannerCombatContext context);
    float GetCombatModMagnitudeScale(PlannerCombatContext context);
    float GetCombatModDurationScale(PlannerCombatContext context);
    float ApplyChanceModifiers(Character? character, IPower? ownerPower, IEffect procEffect, float probability);
    void ApplyPvpDiminishingReturns(Character.TotalStatistics totals);
    void ApplyFinalCaps(Archetype? archetype, Character.TotalStatistics totals, Character.TotalStatistics totalsCapped,
        int? zeroBasedLevel = null);
    float GetDisplayedBuffHastePercent(Archetype? archetype, Character.TotalStatistics totals,
        Character.TotalStatistics totalsCapped, bool uncapped, int? zeroBasedLevel = null);
    float GetDisplayedBuffDamagePercent(Archetype? archetype, Character.TotalStatistics totals,
        Character.TotalStatistics totalsCapped, bool uncapped, int? zeroBasedLevel = null);
}

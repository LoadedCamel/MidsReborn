using System;
using System.Collections.Generic;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.PlannerRulesets;

namespace Mids_Reborn.Core;

internal sealed class ActorPowerAssemblyContext
{
    public string ClassName { get; init; } = string.Empty;
    public Archetype? Archetype { get; init; }
    public IReadOnlyList<IPower> MathPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> BuffedPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower>? IncludedMathPowers { get; init; }
    public IReadOnlyList<IPower>? IncludedBuffedPowers { get; init; }
    public IReadOnlyList<IPower>? IncludedSelfBuffPowers { get; init; }
    public IReadOnlyList<IPower> EnhancementExternalPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> SelfBuffExternalPowers { get; init; } = Array.Empty<IPower>();
    public float ComputedDefianceMagnitude { get; init; }
    public float ComputedVigilanceDamageMagnitude { get; init; }
    public float ComputedVigilanceEndDiscountMagnitude { get; init; }
    public CosmicBalanceComputedState CosmicBalanceState { get; init; } = new();
    public CosmicBalanceComputedState DarkSustenanceState { get; init; } = new();
    public IPower? ChanceModifierSetBonusPower { get; init; }
    public IReadOnlyList<IPower> SupplementalChanceModifierPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyDictionary<string, float>? SupplementalChanceModifierCatalog { get; init; }
    public bool BuildChanceModifierCatalog { get; init; } = true;
}

internal sealed class ActorPowerAssemblyResult
{
    public string ClassName { get; init; } = string.Empty;
    public Archetype? Archetype { get; init; }
    public IReadOnlyList<IPower> MathPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> BuffedPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> IncludedMathPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> IncludedBuffedPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> IncludedSelfBuffPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> EnhancementExternalPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> SelfBuffExternalPowers { get; init; } = Array.Empty<IPower>();
    public Enums.BuffsX SelfEnhanceBuckets { get; init; }
    public Enums.BuffsX SelfBuffBuckets { get; init; }
    public Dictionary<string, float> ChanceModifierCatalog { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public CalculationContributionSnapshot Contributions { get; init; } = CalculationContributionSnapshot.Empty;
}

internal static class ActorPowerAssembly
{
    public static ActorPowerAssemblyResult Build(ActorPowerAssemblyContext context)
    {
        var ruleset = DatabaseAPI.GetPlannerRuleset();
        var mathPowers = Materialize(context.MathPowers);
        var buffedPowers = Materialize(context.BuffedPowers);
        var includedMathPowers = context.IncludedMathPowers == null
            ? mathPowers
            : Materialize(context.IncludedMathPowers);
        var includedBuffedPowers = context.IncludedBuffedPowers == null
            ? buffedPowers
            : Materialize(context.IncludedBuffedPowers);
        var includedSelfBuffPowers = context.IncludedSelfBuffPowers == null
            ? includedBuffedPowers
            : Materialize(context.IncludedSelfBuffPowers);
        var enhancementExternalPowers = Materialize(context.EnhancementExternalPowers);
        var selfBuffExternalPowers = Materialize(context.SelfBuffExternalPowers);

        var selfEnhance = new Enums.BuffsX();
        var selfBuffs = new Enums.BuffsX();
        selfEnhance.Reset();
        selfBuffs.Reset();
        var contributionCollector = new PlannerContributionCollector();

        foreach (var power in includedMathPowers)
        {
            ContributionCapture.AccumulateBuckets(ruleset, power, ref selfEnhance, PlannerBucketPass.Enhancement, contributionCollector);
        }

        foreach (var power in enhancementExternalPowers)
        {
            ContributionCapture.AccumulateBuckets(ruleset, power, ref selfEnhance, PlannerBucketPass.Enhancement, contributionCollector);
        }

        foreach (var power in includedSelfBuffPowers)
        {
            ContributionCapture.AccumulateBuckets(ruleset, power, ref selfBuffs, PlannerBucketPass.SelfBuff, contributionCollector);
        }

        foreach (var power in selfBuffExternalPowers)
        {
            ContributionCapture.AccumulateBuckets(ruleset, power, ref selfBuffs, PlannerBucketPass.SelfBuff, contributionCollector);
        }

        void AddComputedBucketDelta(
            ContributionBucket bucket,
            int index,
            double value,
            string sourceName,
            string sourceFullName,
            string metricName)
        {
            contributionCollector.AddSyntheticBucketDelta(
                bucket,
                index,
                value,
                sourceName,
                sourceFullName,
                ContributionCategory.ComputedState,
                metricName);
        }

        if (context.ComputedDefianceMagnitude > float.Epsilon)
        {
            foreach (var damageType in DefiancePlanner.ComputedBuffDamageTypes)
            {
                selfBuffs.Damage[(int)damageType] += context.ComputedDefianceMagnitude;
                AddComputedBucketDelta(
                    ContributionBucket.Damage,
                    (int)damageType,
                    context.ComputedDefianceMagnitude,
                    "Defiance",
                    "Planner.Computed.Defiance",
                    $"Damage[{(int)damageType}]");
            }
        }

        if (context.ComputedVigilanceDamageMagnitude > float.Epsilon)
        {
            foreach (var damageType in DefiancePlanner.ComputedBuffDamageTypes)
            {
                selfBuffs.Damage[(int)damageType] += context.ComputedVigilanceDamageMagnitude;
                AddComputedBucketDelta(
                    ContributionBucket.Damage,
                    (int)damageType,
                    context.ComputedVigilanceDamageMagnitude,
                    "Vigilance",
                    "Planner.Computed.Vigilance",
                    $"Damage[{(int)damageType}]");
            }
        }

        if (context.ComputedVigilanceEndDiscountMagnitude > float.Epsilon)
        {
            selfBuffs.Effect[(int)Enums.eStatType.BuffEndRdx] += context.ComputedVigilanceEndDiscountMagnitude;
            AddComputedBucketDelta(
                ContributionBucket.Effect,
                (int)Enums.eStatType.BuffEndRdx,
                context.ComputedVigilanceEndDiscountMagnitude,
                "Vigilance",
                "Planner.Computed.Vigilance",
                $"Effect[{(int)Enums.eStatType.BuffEndRdx}]");
        }

        if (context.CosmicBalanceState.DamageMagnitude > float.Epsilon)
        {
            foreach (var damageType in CosmicBalancePlanner.DamageTypes)
            {
                selfBuffs.Damage[(int)damageType] += context.CosmicBalanceState.DamageMagnitude;
                AddComputedBucketDelta(
                    ContributionBucket.Damage,
                    (int)damageType,
                    context.CosmicBalanceState.DamageMagnitude,
                    "Cosmic Balance",
                    "Planner.Computed.CosmicBalance",
                    $"Damage[{(int)damageType}]");
            }
        }

        if (context.CosmicBalanceState.ResistanceMagnitude > float.Epsilon)
        {
            foreach (var damageType in CosmicBalancePlanner.ResistanceTypes)
            {
                selfBuffs.Resistance[(int)damageType] += context.CosmicBalanceState.ResistanceMagnitude;
                AddComputedBucketDelta(
                    ContributionBucket.Resistance,
                    (int)damageType,
                    context.CosmicBalanceState.ResistanceMagnitude,
                    "Cosmic Balance",
                    "Planner.Computed.CosmicBalance",
                    $"Resistance[{(int)damageType}]");
            }
        }

        if (context.CosmicBalanceState.MezProtectionMagnitude > float.Epsilon)
        {
            foreach (var mezType in CosmicBalancePlanner.MezTypes)
            {
                selfBuffs.StatusProtection[(int)mezType] += context.CosmicBalanceState.MezProtectionMagnitude;
                AddComputedBucketDelta(
                    ContributionBucket.StatusProtection,
                    (int)mezType,
                    context.CosmicBalanceState.MezProtectionMagnitude,
                    "Cosmic Balance",
                    "Planner.Computed.CosmicBalance",
                    $"StatusProtection[{(int)mezType}]");
            }
        }

        if (context.CosmicBalanceState.MezResistanceMagnitude > float.Epsilon)
        {
            foreach (var mezType in CosmicBalancePlanner.MezTypes)
            {
                selfBuffs.StatusResistance[(int)mezType] += context.CosmicBalanceState.MezResistanceMagnitude;
                AddComputedBucketDelta(
                    ContributionBucket.StatusResistance,
                    (int)mezType,
                    context.CosmicBalanceState.MezResistanceMagnitude,
                    "Cosmic Balance",
                    "Planner.Computed.CosmicBalance",
                    $"StatusResistance[{(int)mezType}]");
            }
        }

        if (context.CosmicBalanceState.RechargeSlowResistanceMagnitude > float.Epsilon)
        {
            selfBuffs.DebuffResistance[(int)Enums.eEffectType.RechargeTime] +=
                context.CosmicBalanceState.RechargeSlowResistanceMagnitude;
            AddComputedBucketDelta(
                ContributionBucket.DebuffResistance,
                (int)Enums.eEffectType.RechargeTime,
                context.CosmicBalanceState.RechargeSlowResistanceMagnitude,
                "Cosmic Balance",
                "Planner.Computed.CosmicBalance",
                $"DebuffResistance[{(int)Enums.eEffectType.RechargeTime}]");
        }

        if (context.DarkSustenanceState.DamageMagnitude > float.Epsilon)
        {
            foreach (var damageType in DarkSustenancePlanner.DamageTypes)
            {
                selfBuffs.Damage[(int)damageType] += context.DarkSustenanceState.DamageMagnitude;
                AddComputedBucketDelta(
                    ContributionBucket.Damage,
                    (int)damageType,
                    context.DarkSustenanceState.DamageMagnitude,
                    "Dark Sustenance",
                    "Planner.Computed.DarkSustenance",
                    $"Damage[{(int)damageType}]");
            }
        }

        if (context.DarkSustenanceState.ResistanceMagnitude > float.Epsilon)
        {
            foreach (var damageType in DarkSustenancePlanner.ResistanceTypes)
            {
                selfBuffs.Resistance[(int)damageType] += context.DarkSustenanceState.ResistanceMagnitude;
                AddComputedBucketDelta(
                    ContributionBucket.Resistance,
                    (int)damageType,
                    context.DarkSustenanceState.ResistanceMagnitude,
                    "Dark Sustenance",
                    "Planner.Computed.DarkSustenance",
                    $"Resistance[{(int)damageType}]");
            }
        }

        if (context.DarkSustenanceState.MezProtectionMagnitude > float.Epsilon)
        {
            foreach (var mezType in DarkSustenancePlanner.MezTypes)
            {
                selfBuffs.StatusProtection[(int)mezType] += context.DarkSustenanceState.MezProtectionMagnitude;
                AddComputedBucketDelta(
                    ContributionBucket.StatusProtection,
                    (int)mezType,
                    context.DarkSustenanceState.MezProtectionMagnitude,
                    "Dark Sustenance",
                    "Planner.Computed.DarkSustenance",
                    $"StatusProtection[{(int)mezType}]");
            }
        }

        if (context.DarkSustenanceState.MezResistanceMagnitude > float.Epsilon)
        {
            foreach (var mezType in DarkSustenancePlanner.MezTypes)
            {
                selfBuffs.StatusResistance[(int)mezType] += context.DarkSustenanceState.MezResistanceMagnitude;
                AddComputedBucketDelta(
                    ContributionBucket.StatusResistance,
                    (int)mezType,
                    context.DarkSustenanceState.MezResistanceMagnitude,
                    "Dark Sustenance",
                    "Planner.Computed.DarkSustenance",
                    $"StatusResistance[{(int)mezType}]");
            }
        }

        if (context.DarkSustenanceState.RechargeSlowResistanceMagnitude > float.Epsilon)
        {
            selfBuffs.DebuffResistance[(int)Enums.eEffectType.RechargeTime] +=
                context.DarkSustenanceState.RechargeSlowResistanceMagnitude;
            AddComputedBucketDelta(
                ContributionBucket.DebuffResistance,
                (int)Enums.eEffectType.RechargeTime,
                context.DarkSustenanceState.RechargeSlowResistanceMagnitude,
                "Dark Sustenance",
                "Planner.Computed.DarkSustenance",
                $"DebuffResistance[{(int)Enums.eEffectType.RechargeTime}]");
        }

        var chanceModifierCatalog = context.BuildChanceModifierCatalog
            ? ChanceModifierCatalogBuilder.Build(
                includedSelfBuffPowers.Cast<IPower?>().ToArray(),
                context.ChanceModifierSetBonusPower,
                context.SupplementalChanceModifierPowers,
                requireActive: false)
            : new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        MergeChanceModifierCatalog(chanceModifierCatalog, context.SupplementalChanceModifierCatalog);
        CaptureChanceModifierContributions(
            includedSelfBuffPowers,
            PlannerBucketPass.SelfBuff,
            contributionCollector,
            requireActive: false);
        if (context.ChanceModifierSetBonusPower != null)
        {
            CaptureChanceModifierContributions([context.ChanceModifierSetBonusPower], PlannerBucketPass.SelfBuff, contributionCollector);
        }

        CaptureChanceModifierContributions(
            context.SupplementalChanceModifierPowers,
            PlannerBucketPass.SelfBuff,
            contributionCollector);

        return new ActorPowerAssemblyResult
        {
            ClassName = context.ClassName,
            Archetype = context.Archetype,
            MathPowers = mathPowers,
            BuffedPowers = buffedPowers,
            IncludedMathPowers = includedMathPowers,
            IncludedBuffedPowers = includedBuffedPowers,
            IncludedSelfBuffPowers = includedSelfBuffPowers,
            EnhancementExternalPowers = enhancementExternalPowers,
            SelfBuffExternalPowers = selfBuffExternalPowers,
            SelfEnhanceBuckets = selfEnhance,
            SelfBuffBuckets = selfBuffs,
            Contributions = contributionCollector.ToSnapshot(),
            ChanceModifierCatalog = chanceModifierCatalog
        };
    }

    public static ActorTotalsSnapshot CalculateTotals(ActorPowerAssemblyResult assemblyResult, bool applyPvpDiminishingReturns)
    {
        return ActorTotalsCalculator.Calculate(new ActorTotalsCalculationRequest
        {
            ClassName = assemblyResult.ClassName,
            Archetype = assemblyResult.Archetype,
            SelfEnhance = assemblyResult.SelfEnhanceBuckets,
            SelfBuffs = assemblyResult.SelfBuffBuckets,
            IncludedBuffedPowers = assemblyResult.IncludedBuffedPowers,
            ApplyPvpDiminishingReturns = applyPvpDiminishingReturns
        });
    }

    private static IReadOnlyList<IPower> Materialize(IEnumerable<IPower> powers)
    {
        return powers.ToArray();
    }

    private static void MergeChanceModifierCatalog(
        IDictionary<string, float> target,
        IReadOnlyDictionary<string, float>? supplemental)
    {
        if (supplemental == null)
        {
            return;
        }

        foreach (var (tag, magnitude) in supplemental)
        {
            if (string.IsNullOrWhiteSpace(tag) || Math.Abs(magnitude) <= float.Epsilon)
            {
                continue;
            }

            if (target.TryGetValue(tag, out var existing))
            {
                target[tag] = existing + magnitude;
            }
            else
            {
                target[tag] = magnitude;
            }
        }
    }

    private static void CaptureChanceModifierContributions(
        IEnumerable<IPower> powers,
        PlannerBucketPass pass,
        PlannerContributionCollector collector,
        bool requireActive = true)
    {
        foreach (var power in powers)
        {
            ContributionCapture.RecordChanceModifierContributions(power, pass, collector, requireActive);
        }
    }
}

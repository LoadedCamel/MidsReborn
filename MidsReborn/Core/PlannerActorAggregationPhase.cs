using System.Collections.Generic;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.PlannerRulesets;

namespace Mids_Reborn.Core;

internal sealed class PlannerActorAggregationContext
{
    public string ClassName { get; init; } = string.Empty;
    public Archetype? Archetype { get; init; }
    public IReadOnlyList<IPower> MathPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> BuffedPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower>? IncludedMathPowers { get; init; }
    public IReadOnlyList<IPower>? IncludedBuffedPowers { get; init; }
    public IReadOnlyList<IPower> EnhancementExternalPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> SelfBuffExternalPowers { get; init; } = Array.Empty<IPower>();
    public float ComputedDefianceMagnitude { get; init; }
    public float ComputedVigilanceDamageMagnitude { get; init; }
    public float ComputedVigilanceEndDiscountMagnitude { get; init; }
    public CosmicBalanceComputedState CosmicBalanceState { get; init; } = new();
    public CosmicBalanceComputedState DarkSustenanceState { get; init; } = new();
    public IReadOnlyList<IPower> SupplementalEnhancementSourcePowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> SupplementalSelfBuffSourcePowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> SupplementalChanceModifierPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyCollection<int> SupplementalEnhancementExcludedIndexes { get; init; } = Array.Empty<int>();
    public IPower? ChanceModifierSetBonusPower { get; init; }
    public IReadOnlyDictionary<string, float>? SupplementalChanceModifierCatalog { get; init; }
    public bool BuildChanceModifierCatalog { get; init; } = true;
    public bool ApplyPvpDiminishingReturns { get; init; }
}

internal sealed class PlannerActorAggregationResult
{
    public ActorPowerAssemblyResult ActorAssembly { get; init; } = null!;
    public Enums.BuffsX FinalSelfEnhanceBuckets { get; init; }
    public Enums.BuffsX FinalSelfBuffBuckets { get; init; }
    public ActorTotalsSnapshot? Totals { get; init; }
    public CalculationContributionSnapshot Contributions { get; init; } = CalculationContributionSnapshot.Empty;
    public ActorAggregationSnapshot Aggregation { get; init; } = new();
}

internal static class PlannerActorAggregationPhase
{
    public static PlannerActorAggregationResult Assemble(PlannerActorAggregationContext context)
    {
        var actorAssembly = ActorPowerAssembly.Build(new ActorPowerAssemblyContext
        {
            ClassName = context.ClassName,
            Archetype = context.Archetype,
            MathPowers = context.MathPowers,
            BuffedPowers = context.BuffedPowers,
            IncludedMathPowers = context.IncludedMathPowers,
            IncludedBuffedPowers = context.IncludedBuffedPowers,
            EnhancementExternalPowers = context.EnhancementExternalPowers,
            SelfBuffExternalPowers = context.SelfBuffExternalPowers,
            ComputedDefianceMagnitude = context.ComputedDefianceMagnitude,
            ComputedVigilanceDamageMagnitude = context.ComputedVigilanceDamageMagnitude,
            ComputedVigilanceEndDiscountMagnitude = context.ComputedVigilanceEndDiscountMagnitude,
            CosmicBalanceState = context.CosmicBalanceState,
            DarkSustenanceState = context.DarkSustenanceState,
            ChanceModifierSetBonusPower = context.ChanceModifierSetBonusPower,
            SupplementalChanceModifierPowers = context.SupplementalChanceModifierPowers,
            SupplementalChanceModifierCatalog = context.SupplementalChanceModifierCatalog,
            BuildChanceModifierCatalog = context.BuildChanceModifierCatalog
        });

        return new PlannerActorAggregationResult
        {
            ActorAssembly = actorAssembly,
            FinalSelfEnhanceBuckets = CalculationSnapshotFactory.CloneBuckets(actorAssembly.SelfEnhanceBuckets),
            FinalSelfBuffBuckets = CalculationSnapshotFactory.CloneBuckets(actorAssembly.SelfBuffBuckets),
            Contributions = actorAssembly.Contributions,
            Aggregation = CalculationSnapshotFactory.CreateAggregationSnapshot(
                actorAssembly.IncludedMathPowers,
                actorAssembly.IncludedBuffedPowers,
                actorAssembly.EnhancementExternalPowers,
                actorAssembly.SelfBuffExternalPowers,
                context.SupplementalEnhancementSourcePowers,
                context.SupplementalSelfBuffSourcePowers)
        };
    }

    public static PlannerActorAggregationResult Finalize(PlannerActorAggregationContext context)
    {
        var assembled = Assemble(context);
        var finalSelfEnhance = CalculationSnapshotFactory.CloneBuckets(assembled.FinalSelfEnhanceBuckets);
        var finalSelfBuff = CalculationSnapshotFactory.CloneBuckets(assembled.FinalSelfBuffBuckets);
        var supplementalEnhance = new Enums.BuffsX();
        var supplementalSelfBuffs = new Enums.BuffsX();
        supplementalEnhance.Reset();
        supplementalSelfBuffs.Reset();
        var collector = new PlannerContributionCollector();
        var plannerRuleset = DatabaseAPI.GetPlannerRuleset();

        foreach (var power in context.SupplementalEnhancementSourcePowers)
        {
            ContributionCapture.AccumulateBuckets(
                plannerRuleset,
                power,
                ref supplementalEnhance,
                PlannerBucketPass.Enhancement,
                collector);
        }

        foreach (var power in context.SupplementalSelfBuffSourcePowers)
        {
            ContributionCapture.AccumulateBuckets(
                plannerRuleset,
                power,
                ref supplementalSelfBuffs,
                PlannerBucketPass.SelfBuff,
                collector);
        }

        if (PlannerSupplementalPowerMath.HasSupplementalEnhancementBuckets(supplementalEnhance) ||
            PlannerSupplementalPowerMath.HasSupplementalEnhancementBuckets(supplementalSelfBuffs))
        {
            var oldBuffAcc = finalSelfBuff.Effect[(int)Enums.eStatType.BuffAcc];
            var oldToHit = finalSelfBuff.Effect[(int)Enums.eStatType.ToHit];
            PlannerSupplementalPowerMath.MergeBuffBuckets(ref finalSelfEnhance, supplementalEnhance);
            PlannerSupplementalPowerMath.MergeBuffBuckets(ref finalSelfBuff, supplementalSelfBuffs);

            if (PlannerSupplementalPowerMath.HasSupplementalEnhancementBuckets(supplementalEnhance))
            {
                var excludedIndexes = context.SupplementalEnhancementExcludedIndexes.Count == 0
                    ? null
                    : context.SupplementalEnhancementExcludedIndexes.ToHashSet();

                for (var index = 0; index < context.MathPowers.Count && index < context.BuffedPowers.Count; index++)
                {
                    if (excludedIndexes?.Contains(index) == true)
                    {
                        continue;
                    }

                    var mathPower = context.MathPowers[index];
                    var buffedPower = context.BuffedPowers[index];
                    IPower? mathRef = mathPower;
                    IPower? buffedRef = buffedPower;
                    PlannerSupplementalPowerMath.ApplySupplementalEnhancementBuckets(
                        ref mathRef,
                        ref buffedRef,
                        supplementalEnhance);
                }
            }

            PlannerSupplementalPowerMath.ApplySelfBuffAccuracyPreview(
                context.MathPowers.Cast<IPower?>().ToArray(),
                context.BuffedPowers.Cast<IPower?>().ToArray(),
                finalSelfBuff,
                oldBuffAcc,
                oldToHit);
        }

        var totals = ActorTotalsCalculator.Calculate(new ActorTotalsCalculationRequest
        {
            ClassName = context.ClassName,
            Archetype = context.Archetype,
            SelfEnhance = finalSelfEnhance,
            SelfBuffs = finalSelfBuff,
            IncludedBuffedPowers = assembled.ActorAssembly.IncludedBuffedPowers,
            ApplyPvpDiminishingReturns = context.ApplyPvpDiminishingReturns
        });

        return new PlannerActorAggregationResult
        {
            ActorAssembly = assembled.ActorAssembly,
            FinalSelfEnhanceBuckets = finalSelfEnhance,
            FinalSelfBuffBuckets = finalSelfBuff,
            Totals = totals,
            Contributions = CalculationContributionSnapshot.Merge(
                assembled.Contributions,
                collector.ToSnapshot(),
                ContributionCapture.CreateCapAdjustmentSnapshot(totals)),
            Aggregation = assembled.Aggregation
        };
    }
}

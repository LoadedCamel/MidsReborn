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
    public IReadOnlyList<IPower> EnhancementExternalPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> SelfBuffExternalPowers { get; init; } = Array.Empty<IPower>();
    public float ComputedDefianceMagnitude { get; init; }
    public IPower? ChanceModifierSetBonusPower { get; init; }
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

        foreach (var power in includedBuffedPowers)
        {
            ContributionCapture.AccumulateBuckets(ruleset, power, ref selfBuffs, PlannerBucketPass.SelfBuff, contributionCollector);
        }

        foreach (var power in selfBuffExternalPowers)
        {
            ContributionCapture.AccumulateBuckets(ruleset, power, ref selfBuffs, PlannerBucketPass.SelfBuff, contributionCollector);
        }

        if (context.ComputedDefianceMagnitude > float.Epsilon)
        {
            foreach (var damageType in DefiancePlanner.ComputedBuffDamageTypes)
            {
                selfBuffs.Damage[(int)damageType] += context.ComputedDefianceMagnitude;
            }
        }

        return new ActorPowerAssemblyResult
        {
            ClassName = context.ClassName,
            Archetype = context.Archetype,
            MathPowers = mathPowers,
            BuffedPowers = buffedPowers,
            IncludedMathPowers = includedMathPowers,
            IncludedBuffedPowers = includedBuffedPowers,
            EnhancementExternalPowers = enhancementExternalPowers,
            SelfBuffExternalPowers = selfBuffExternalPowers,
            SelfEnhanceBuckets = selfEnhance,
            SelfBuffBuckets = selfBuffs,
            Contributions = contributionCollector.ToSnapshot(),
            ChanceModifierCatalog = context.BuildChanceModifierCatalog
                ? ChanceModifierCatalogBuilder.Build(buffedPowers.Cast<IPower?>().ToArray(), context.ChanceModifierSetBonusPower)
                : new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase)
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
}

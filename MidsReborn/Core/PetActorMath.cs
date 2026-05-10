using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

internal static class PetActorMath
{
    public static PlannerActorAggregationContext CreateAggregationContext(
        string className,
        IReadOnlyList<IPower> mathPowers,
        IReadOnlyList<IPower> buffedPowers,
        IEnumerable<IPower> externalPowers,
        IReadOnlyList<IPower>? includedMathPowers = null,
        IReadOnlyList<IPower>? includedBuffedPowers = null,
        IReadOnlyList<IPower>? supplementalEnhancementSourcePowers = null,
        IReadOnlyCollection<int>? supplementalEnhancementExcludedIndexes = null)
    {
        var externalPowerList = externalPowers.ToArray();

        return new PlannerActorAggregationContext
        {
            ClassName = className,
            Archetype = DatabaseAPI.GetArchetypeByClassName(className),
            MathPowers = mathPowers,
            BuffedPowers = buffedPowers,
            IncludedMathPowers = includedMathPowers,
            IncludedBuffedPowers = includedBuffedPowers,
            EnhancementExternalPowers = externalPowerList,
            SelfBuffExternalPowers = externalPowerList,
            SupplementalEnhancementSourcePowers = supplementalEnhancementSourcePowers ?? Array.Empty<IPower>(),
            SupplementalSelfBuffSourcePowers = Array.Empty<IPower>(),
            SupplementalEnhancementExcludedIndexes = supplementalEnhancementExcludedIndexes ?? Array.Empty<int>(),
            BuildChanceModifierCatalog = false,
            ApplyPvpDiminishingReturns = false
        };
    }

    public static ActorPowerAssemblyResult Assemble(
        string className,
        IReadOnlyList<IPower> mathPowers,
        IReadOnlyList<IPower> buffedPowers,
        IPower? setBonusPower)
    {
        return Assemble(
            className,
            mathPowers,
            buffedPowers,
            setBonusPower == null ? Array.Empty<IPower>() : new[] { setBonusPower },
            includedMathPowers: null,
            includedBuffedPowers: null);
    }

    public static ActorPowerAssemblyResult Assemble(
        string className,
        IReadOnlyList<IPower> mathPowers,
        IReadOnlyList<IPower> buffedPowers,
        IEnumerable<IPower> externalPowers,
        IReadOnlyList<IPower>? includedMathPowers = null,
        IReadOnlyList<IPower>? includedBuffedPowers = null)
    {
        return PlannerActorAggregationPhase.Assemble(
            CreateAggregationContext(
                className,
                mathPowers,
                buffedPowers,
                externalPowers,
                includedMathPowers,
                includedBuffedPowers)).ActorAssembly;
    }

    public static PlannerActorAggregationResult Finalize(
        string className,
        IReadOnlyList<IPower> mathPowers,
        IReadOnlyList<IPower> buffedPowers,
        IEnumerable<IPower> externalPowers,
        IReadOnlyList<IPower>? includedMathPowers = null,
        IReadOnlyList<IPower>? includedBuffedPowers = null,
        IReadOnlyList<IPower>? supplementalEnhancementSourcePowers = null,
        IReadOnlyCollection<int>? supplementalEnhancementExcludedIndexes = null)
    {
        return PlannerActorAggregationPhase.Finalize(
            CreateAggregationContext(
                className,
                mathPowers,
                buffedPowers,
                externalPowers,
                includedMathPowers,
                includedBuffedPowers,
                supplementalEnhancementSourcePowers,
                supplementalEnhancementExcludedIndexes));
    }

    public static ActorTotalsSnapshot Calculate(ActorPowerAssemblyResult assemblyResult)
    {
        return ActorPowerAssembly.CalculateTotals(assemblyResult, applyPvpDiminishingReturns: false);
    }

    public static ActorTotalsSnapshot Calculate(
        string className,
        IReadOnlyList<IPower> mathPowers,
        IReadOnlyList<IPower> buffedPowers,
        IPower? setBonusPower)
    {
        return Calculate(Assemble(className, mathPowers, buffedPowers, setBonusPower));
    }

    public static ActorTotalsSnapshot Calculate(
        string className,
        IReadOnlyList<IPower> mathPowers,
        IReadOnlyList<IPower> buffedPowers,
        IEnumerable<IPower> externalPowers)
    {
        return Calculate(Assemble(className, mathPowers, buffedPowers, externalPowers));
    }
}

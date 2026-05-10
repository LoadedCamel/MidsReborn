using System;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.PlannerRulesets;

namespace Mids_Reborn.Core;

public sealed class PlannerPowerPipelineResult
{
    public IPower?[] BasePowers { get; internal set; } = Array.Empty<IPower?>();
    public IPower?[] AssembledBasePowers { get; internal set; } = Array.Empty<IPower?>();
    public IPower?[] MathPowers { get; internal set; } = Array.Empty<IPower?>();
    public IPower?[] PreBuffPowers { get; internal set; } = Array.Empty<IPower?>();
    public IPower?[] BuffedPowers { get; internal set; } = Array.Empty<IPower?>();
    public Enums.BuffsX SelfEnhanceBuckets { get; internal set; }
    public Enums.BuffsX SelfBuffBuckets { get; internal set; }
    public PlannerCombatContext CombatContext { get; internal set; } = PlannerCombatContext.Default;
    internal ActorTotalsSnapshot? FinalTotalsSnapshot { get; set; }
    internal ActorCalculationSnapshot? ActorSnapshot { get; set; }
    internal BuildCalculationSnapshot? CalculationSnapshot { get; set; }
    internal ActorPowerAssemblyResult? ActorAssembly { get; set; }
}

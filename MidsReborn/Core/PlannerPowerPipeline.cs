using System.Globalization;
using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.Core.PlannerRulesets;

namespace Mids_Reborn.Core;

internal sealed class PlannerPowerPipeline
{
    private readonly Build _currentBuild;
    private readonly Archetype? _archetype;
    private readonly PlannerBuildRecipientContext? _recipient;
    private readonly IReadOnlyList<IPower> _recipientExternalPowers;
    private IPower?[] _basePowers = Array.Empty<IPower?>();
    private IPower?[] _assembledBasePowers = Array.Empty<IPower?>();
    private IPower?[] _buffedPowers = Array.Empty<IPower?>();
    private IPower?[] _mathPowers = Array.Empty<IPower?>();
    private IPower?[] _preBuffPowers = Array.Empty<IPower?>();
    private Enums.BuffsX _selfBuffs;
    private Enums.BuffsX _selfEnhance;
    private ActorPowerAssemblyResult? _actorAssembly;
    private Dictionary<string, float> _chanceModifierCatalog = new(StringComparer.OrdinalIgnoreCase);
    private PlannerCombatContext _combatContext = PlannerCombatContext.Default;

    private struct FxIdShort
    {
        public Enums.eEffectType EffectType;
        public Enums.eMez MezType;
        public Enums.eEffectType ETModifies;
    }

    public PlannerPowerPipeline(
        Build currentBuild,
        Archetype? archetype,
        PlannerBuildRecipientContext? recipient = null,
        IReadOnlyList<IPower>? recipientExternalPowers = null)
    {
        _currentBuild = currentBuild;
        _archetype = archetype;
        _recipient = recipient;
        _recipientExternalPowers = recipientExternalPowers ?? Array.Empty<IPower>();
        Result = new PlannerPowerPipelineResult();
        SyncResult();
    }

    public PlannerPowerPipelineResult Result { get; }

    public void ExecuteAssemblyPhase()
    {
        _selfBuffs.Reset();
        _selfEnhance.Reset();
        _actorAssembly = null;
        _chanceModifierCatalog = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        _combatContext = PlannerCombatContext.Default;
        _basePowers = new IPower?[_currentBuild.Powers.Count];
        _assembledBasePowers = new IPower?[_currentBuild.Powers.Count];
        _buffedPowers = new IPower?[_currentBuild.Powers.Count];
        _mathPowers = new IPower?[_currentBuild.Powers.Count];
        _preBuffPowers = new IPower?[_currentBuild.Powers.Count];
        GBPA_Pass0_InitializePowerArray();
        _actorAssembly = BuildActorAggregation(CreateActorAggregationContext(
            buildChanceModifierCatalog: _recipient == null)).ActorAssembly;
        if (_recipient == null)
        {
            _chanceModifierCatalog = new Dictionary<string, float>(
                _actorAssembly.ChanceModifierCatalog,
                StringComparer.OrdinalIgnoreCase);
        }

        SyncResult();
    }

    public void ExecuteEnhancementBucketPhase()
    {
        _actorAssembly = BuildActorAggregation(CreateActorAggregationContext(buildChanceModifierCatalog: false)).ActorAssembly;
        _selfEnhance = _actorAssembly.SelfEnhanceBuckets;
        SyncResult();
    }

    public void ExecutePerPowerEnhancementMathPhase()
    {
        Parallel.For(0, _mathPowers.Length, hIDX =>
        {
            if (_mathPowers[hIDX] == null)
            {
                return;
            }

            GBPA_Pass1_EnhancePreED(ref _mathPowers[hIDX], hIDX);
            GBPA_Pass2_ApplyED(ref _mathPowers[hIDX]);
            GBPA_Pass3_EnhancePostED(ref _mathPowers[hIDX], hIDX);
            GBPA_Pass4_Add(ref _mathPowers[hIDX]);
            GBPA_ApplyArchetypeCaps(ref _mathPowers[hIDX]);
            GBPA_Pass5_MultiplyPreBuff(ref _mathPowers[hIDX], ref _buffedPowers[hIDX]);
        });

        for (var index = 0; index < _mathPowers.Length; index++)
        {
            RemoveExactDuplicateAbsorbedEffects(ref _mathPowers[index]);
            RemoveExactDuplicateAbsorbedEffects(ref _buffedPowers[index]);
            _preBuffPowers[index] = _buffedPowers[index] == null ? null : new Power(_buffedPowers[index]);
        }

        SyncResult();
    }

    public void ExecuteSelfBuffBucketPhase()
    {
        _actorAssembly = BuildActorAggregation(CreateActorAggregationContext(buildChanceModifierCatalog: false)).ActorAssembly;
        _selfBuffs = _actorAssembly.SelfBuffBuckets;
        _combatContext = DatabaseAPI.GetPlannerRuleset()
            .ResolveCombatContext(MidsContext.Config);
        SyncResult();
    }

    public void ExecutePostBuffMultiplyPhase()
    {
        Parallel.For(0, _mathPowers.Length, index =>
        {
            if (_mathPowers[index] != null)
            {
                GBPA_Pass6_MultiplyPostBuff(ref _mathPowers[index], ref _buffedPowers[index]);
            }
        });

        for (var index = 0; index < _buffedPowers.Length; index++)
        {
            RemoveExactDuplicateAbsorbedEffects(ref _buffedPowers[index]);
        }

        _actorAssembly = BuildActorAggregation(CreateActorAggregationContext(buildChanceModifierCatalog: false)).ActorAssembly;
        SyncResult();
    }

    public void ExecuteFinalizationPhase()
    {
        if (_recipient != null)
        {
            return;
        }

        var aggregation = BuildActorAggregation(CreateActorAggregationContext(
            buildChanceModifierCatalog: true),
            finalize: true);
        _actorAssembly = aggregation.ActorAssembly;
        _selfEnhance = aggregation.FinalSelfEnhanceBuckets;
        _selfBuffs = aggregation.FinalSelfBuffBuckets;
        _chanceModifierCatalog = new Dictionary<string, float>(
            aggregation.ActorAssembly.ChanceModifierCatalog,
            StringComparer.OrdinalIgnoreCase);
        SyncResult();

        var powerSnapshots = CalculationSnapshotFactory.CreatePowerSnapshots(
            _basePowers,
            _assembledBasePowers,
            _mathPowers,
            _preBuffPowers,
            _buffedPowers);
        var totalsSnapshot = aggregation.Totals!;
        var actorSnapshot = CalculationSnapshotFactory.CreateActorSnapshot(
            CalculationActorKind.Player,
            _archetype?.ClassName ?? DatabaseAPI.ResolveClassName(_archetype),
            _archetype,
            _mathPowers.OfType<IPower>().ToArray(),
            _buffedPowers.OfType<IPower>().ToArray(),
            _selfEnhance,
            _selfBuffs,
            totalsSnapshot,
            _chanceModifierCatalog,
            aggregation.Contributions,
            powerSnapshots,
            aggregation.Aggregation);

        Result.FinalTotalsSnapshot = totalsSnapshot;
        Result.ActorSnapshot = actorSnapshot;
        Result.CalculationSnapshot = CalculationSnapshotFactory.CreateBuildSnapshot(
            _combatContext,
            actorSnapshot,
            powerSnapshots);
    }

    public IPower? AssemblePowerEntry(int nIDPower, int hIDX, int stackingOverride = -1)
    {
        return GBPA_SubPass0_AssemblePowerEntry(nIDPower, hIDX, stackingOverride);
    }

    private void SyncResult()
    {
        Result.BasePowers = _basePowers;
        Result.AssembledBasePowers = _assembledBasePowers;
        Result.MathPowers = _mathPowers;
        Result.PreBuffPowers = _preBuffPowers;
        Result.BuffedPowers = _buffedPowers;
        Result.SelfEnhanceBuckets = _selfEnhance;
        Result.SelfBuffBuckets = _selfBuffs;
        Result.CombatContext = _combatContext;
        Result.ActorAssembly = _actorAssembly;
        Result.FinalTotalsSnapshot = null;
        Result.ActorSnapshot = null;
        Result.CalculationSnapshot = null;
    }

    private bool GBPA_Pass0_InitializePowerArray()
    {
        var plannerRuleset = DatabaseAPI.GetPlannerRuleset();
        for (var hIDX = 0; hIDX < _currentBuild.Powers.Count; hIDX++)
        {
            var powerEntry = _currentBuild.Powers[hIDX];
            if (powerEntry == null || powerEntry.NIDPower <= -1)
            {
                continue;
            }

            if (powerEntry.Power != null)
            {
                powerEntry.Power.Stacks = PlannerStateCatalog.GetMirroredStackValue(powerEntry.Power, powerEntry.VariableValue);
            }

            _basePowers[hIDX] = new Power(DatabaseAPI.Database.Power[powerEntry.NIDPower]);
            if (_recipient != null && _basePowers[hIDX] is Power basePower)
            {
                basePower.OmniDisplayClassName = _recipient.ClassName;
            }

            var assembledPower = GBPA_SubPass0_AssemblePowerEntry(powerEntry.NIDPower, hIDX);
            if (_recipient != null && assembledPower != null)
            {
                assembledPower = OmniPowerRouting.CreatePlannerPower(
                    assembledPower,
                    _recipient,
                    hIDX == _recipient.ActorSourceHistoryIndex);
            }

            _assembledBasePowers[hIDX] = assembledPower == null ? null : new Power(assembledPower);
            _mathPowers[hIDX] = assembledPower == null ? null : new Power(assembledPower);
        }

        if (plannerRuleset.AllowLegacyCrossPowerIncarnatePasses)
        {
            for (var index1 = 0; index1 < _currentBuild.Powers.Count; index1++)
            {
                if (_currentBuild.Powers[index1] == null || _currentBuild.Powers[index1]?.NIDPower <= -1)
                {
                    continue;
                }

                for (var index2 = 0; index2 < _currentBuild.Powers.Count; index2++)
                {
                    if (index1 == index2 || !IsStaticAggregationSourcePowerEntry(index2, allowGlobalBoost: true))
                    {
                        continue;
                    }

                    var effectType = Enums.eEffectType.GrantPower;
                    GBPA_ApplyIncarnateEnhancements(ref _mathPowers[index1], -1, _mathPowers[index2], false, ref effectType);
                }
            }
        }

        for (var hIDX = 0; hIDX < _currentBuild.Powers.Count; hIDX++)
        {
            if (_currentBuild.Powers[hIDX] == null || _currentBuild.Powers[hIDX].NIDPower <= -1 || _mathPowers[hIDX] == null)
            {
                continue;
            }

            GBPA_MultiplyVariable(ref _mathPowers[hIDX], hIDX);
            _buffedPowers[hIDX] = new Power(_mathPowers[hIDX]);
            _buffedPowers[hIDX]?.SetMathMag();
        }

        if (!plannerRuleset.AllowLegacyCrossPowerIncarnatePasses && plannerRuleset.UsesCanonicalPlannerMath)
        {
            GBPA_ApplyCanonicalCrossPowerGlobalBoostEffects();
        }

        return true;
    }

    private IPower? GBPA_SubPass0_AssemblePowerEntry(int nIDPower, int hIDX, int stackingOverride = -1)
    {
        if (nIDPower < 0)
        {
            return null;
        }

        IPower power = new Power(DatabaseAPI.Database.Power[nIDPower]);

        if (stackingOverride > -1)
        {
            power.Stacks = stackingOverride;
        }
        else if (hIDX >= 0 && hIDX < _currentBuild.Powers.Count && _currentBuild.Powers[hIDX] != null)
        {
            power.Stacks = PlannerStateCatalog.GetMirroredStackValue(power, _currentBuild.Powers[hIDX]!.VariableValue);
        }

        return DatabaseAPI.GetPlannerRuleset().UsesCanonicalPlannerMath
            ? AssemblePlannerEffectivePowerEntryOmni(power, hIDX, stackingOverride)
            : AssemblePlannerEffectivePowerEntryLegacy(power, hIDX, stackingOverride);
    }

    private IPower AssemblePlannerEffectivePowerEntryLegacy(IPower power, int hIDX, int stackingOverride)
    {
        power = PlannerEffectResolver.ApplyRedirect(power);
        GBPA_AddEnhFX(ref power, hIDX);
        power = ResolveAssemblyEffects(power, hIDX, stackingOverride,
            allowPseudoPetAbsorption: true,
            allowGrantExpansion: true,
            allowExecuteExpansion: true);
        GBPA_AddSubPowerEffects(ref power, hIDX);

        return power;
    }

    private IPower AssemblePlannerEffectivePowerEntryOmni(IPower power, int hIDX, int stackingOverride)
    {
        var plannerRuleset = DatabaseAPI.GetPlannerRuleset();
        if (plannerRuleset.AllowRedirectSelectionInAssembly)
        {
            power = PlannerEffectResolver.ApplyRedirect(power);
            power.AppliedPowersOverride = true;
        }

        GBPA_AddEnhFX(ref power, hIDX);
        power = ResolveAssemblyEffects(power, hIDX, stackingOverride,
            plannerRuleset.AllowPseudoPetAbsorptionInAssembly,
            plannerRuleset.AllowGrantPowerExpansionInAssembly,
            plannerRuleset.AllowExecutePowerExpansionInAssembly);

        if (plannerRuleset.AllowSubPowerEffectsInAssembly && !power.AppliedSubPowers)
        {
            GBPA_AddSubPowerEffects(ref power, hIDX);
            power.AppliedSubPowers = true;
        }

        return power;
    }

    private static IPower ResolveAssemblyEffects(
        IPower power,
        int historyIndex,
        int stackingOverride,
        bool allowPseudoPetAbsorption,
        bool allowGrantExpansion,
        bool allowExecuteExpansion)
    {
        return PlannerEffectResolver.ResolvePower(power, new PlannerEffectResolutionContext(useRulesetDefaults: false)
        {
            HistoryIndex = historyIndex,
            StackingOverride = stackingOverride,
            ApplyRedirects = false,
            AbsorbPetEffects = allowPseudoPetAbsorption,
            ExpandGrantPowers = allowGrantExpansion,
            ExpandExecutePowers = allowExecuteExpansion,
            IncludeTrace = false,
            MaxExpansionDepth = PlannerEffectResolutionContext.DefaultMaxExpansionDepth
        }).ResolvedPower;
    }

    private void GBPA_AddEnhFX(ref IPower? power, int index)
    {
        PlannerPowerAssembly.AddEnhancementEffects(_currentBuild, ref power, index);
    }

    private static void AddClonedEffectToList(ICollection<IEffect> effectsList, IEffect enhancementEffect, bool isProc, bool isEnhancementEffect = true)
    {
        if (enhancementEffect.Clone() is not IEffect clonedEffect)
        {
            return;
        }

        clonedEffect.isEnhancementEffect = isEnhancementEffect;
        clonedEffect.IgnoreScaling = isProc;
        clonedEffect.ToWho = enhancementEffect.ToWho;
        clonedEffect.Absorbed_Effect = true;
        clonedEffect.Ticks = enhancementEffect.Ticks;
        clonedEffect.Buffable = false;
        effectsList.Add(clonedEffect);
    }

    private bool GBPA_AddSubPowerEffects(ref IPower power, int hIDX)
    {
        return PlannerPowerAssembly.AddSubPowerEffects(_currentBuild, ref power, hIDX);
    }

    private void GBPA_ApplyArchetypeCaps(ref IPower powerMath)
    {
        var rechargeCap = DatabaseAPI.GetClassRechargeCap(_archetype);
        var damageCap = DatabaseAPI.GetClassDamageCap(_archetype);

        if (powerMath.RechargeTime > rechargeCap)
        {
            powerMath.RechargeTime = rechargeCap;
        }

        foreach (var effect in powerMath.Effects)
        {
            if (effect.EffectType == Enums.eEffectType.Damage && effect.Math_Mag > damageCap)
            {
                effect.Math_Mag = damageCap;
            }
        }
    }

    private static void HandleDefaultIncarnateEnh(ref IPower powerMath, IPower sourcePower, IEffect effect, IEffect[] buffedPowerEffects)
    {
        foreach (var targetEffect in powerMath.Effects)
        {
            if (PlannerStrengthSemantics.IgnoresStrength(targetEffect))
            {
                continue;
            }

            if (!GlobalBoostPlannerSemantics.MatchesTargetEffect(powerMath, targetEffect, sourcePower, effect))
            {
                continue;
            }

            var duration = 0f;
            var mag = 0f;
            if (targetEffect.EffectType is Enums.eEffectType.Resistance or Enums.eEffectType.Damage &&
                effect.EffectType == Enums.eEffectType.DamageBuff)
            {
                if (targetEffect.DamageType == effect.DamageType)
                {
                    targetEffect.Math_Mag += effect.Mag;
                }
            }
            else if (targetEffect.EffectType == effect.ETModifies)
            {
                switch (effect.ETModifies)
                {
                    case Enums.eEffectType.Damage:
                        if (targetEffect.DamageType == effect.DamageType)
                        {
                            targetEffect.Math_Mag += effect.Mag;
                        }

                        mag = 0;
                        break;

                    case Enums.eEffectType.Defense:
                        if (targetEffect.DamageType == effect.DamageType)
                        {
                            targetEffect.Math_Mag += effect.Mag;
                        }

                        mag = 0;
                        break;

                    case Enums.eEffectType.Mez:
                        if (effect.MezType == targetEffect.MezType)
                        {
                            for (var mezIndex = 0; mezIndex < Enum.GetValues<Enums.eMez>().Length; mezIndex++)
                            {
                                if (targetEffect.AttribType == Enums.eAttribType.Duration)
                                {
                                    if (targetEffect.MezType == (Enums.eMez)mezIndex)
                                    {
                                        targetEffect.Math_Duration += effect.Mag;
                                    }

                                    duration = 0;
                                    mag = 0;
                                }
                                else if (targetEffect.MezType == (Enums.eMez)mezIndex)
                                {
                                    targetEffect.Math_Mag += effect.Mag;
                                    mag = 0;
                                }
                            }
                        }

                        break;

                    default:
                        if (targetEffect is
                            {
                                EffectType: Enums.eEffectType.Enhancement,
                                ETModifies: Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping or Enums.eEffectType.JumpHeight or Enums.eEffectType.SpeedFlying
                            } ||
                            targetEffect.EffectType is Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping or Enums.eEffectType.JumpHeight or Enums.eEffectType.SpeedFlying)
                        {
                            mag = effect.Mag;
                            break;
                        }

                        mag = effect.Mag;
                        break;
                }

                targetEffect.Math_Mag += mag;
                targetEffect.Math_Duration += duration;
            }
        }
    }

    private static void HandleGrantPowerIncarnate(ref IPower powerMath, IEffect effect, IReadOnlyList<IPower?> buffedPowers, int effectIndex, Archetype? archetype, int hIDX)
    {
        var originalLength = powerMath.Effects.Length;
        powerMath.AbsorbEffects(DatabaseAPI.Database.Power[effect.nSummon], effect.Duration, 0, archetype, 1, true, effectIndex);
        StampAbsorbedCrossPowerEffects(powerMath, originalLength);
        for (var index = originalLength; index < powerMath.Effects.Length; index++)
        {
            var fx = powerMath.Effects[index];
            fx.ToWho = Enums.eToWho.Target;
            fx.Absorbed_Effect = true;
            fx.isEnhancementEffect = effect.isEnhancementEffect;
            if (effect.EffectType != Enums.eEffectType.GrantPower)
            {
                fx.BaseProbability *= effect.BaseProbability;
            }

            fx.Ticks = effect.Ticks;
        }

        if (hIDX <= -1 || buffedPowers[hIDX] == null)
        {
            return;
        }

        var length = buffedPowers[hIDX]!.Effects.Length;
        buffedPowers[hIDX]!.AbsorbEffects(DatabaseAPI.Database.Power[effect.nSummon], effect.Duration, 0, archetype, 1, true, effectIndex);
        StampAbsorbedCrossPowerEffects(buffedPowers[hIDX]!, length);
        for (var index = length; index < buffedPowers[hIDX]!.Effects.Length; index++)
        {
            buffedPowers[hIDX]!.Effects[index].ToWho = effect.ToWho;
            buffedPowers[hIDX]!.Effects[index].Absorbed_Effect = true;
            buffedPowers[hIDX]!.Effects[index].isEnhancementEffect = effect.isEnhancementEffect;
            buffedPowers[hIDX]!.Effects[index].BaseProbability *= effect.BaseProbability;
            buffedPowers[hIDX]!.Effects[index].Ticks = effect.Ticks;
        }
    }

    private static void StampAbsorbedCrossPowerEffects(IPower ownerPower, int startIndex)
    {
        if (ownerPower == null || startIndex < 0 || startIndex >= ownerPower.Effects.Length)
        {
            return;
        }

        for (var index = startIndex; index < ownerPower.Effects.Length; index++)
        {
            var effect = ownerPower.Effects[index];
            effect.SetPower(ownerPower);
            PlannerProcSupport.AppendProcEvaluationLineage(effect, ownerPower, forceActivationRoot: true);
        }
    }

    private void GBPA_ApplyCanonicalCrossPowerGlobalBoostEffects()
    {
        var sourcePowers = _assembledBasePowers;
        for (var targetIndex = 0; targetIndex < _currentBuild.Powers.Count; targetIndex++)
        {
            if (_currentBuild.Powers[targetIndex] == null ||
                _currentBuild.Powers[targetIndex]!.NIDPower <= -1 ||
                _mathPowers[targetIndex] == null ||
                _buffedPowers[targetIndex] == null ||
                !IsCanonicalCrossPowerGlobalBoostProcHostPower(_mathPowers[targetIndex]))
            {
                continue;
            }

            var originalMathCount = _mathPowers[targetIndex]!.Effects.Length;
            var originalBuffedCount = _buffedPowers[targetIndex]!.Effects.Length;
            for (var sourceIndex = 0; sourceIndex < _currentBuild.Powers.Count; sourceIndex++)
            {
                if (targetIndex == sourceIndex ||
                    !IsStaticAggregationSourcePowerEntry(sourceIndex, allowGlobalBoost: true) ||
                    sourcePowers[sourceIndex] == null ||
                    !SourceHasCanonicalCrossPowerGlobalBoostEffects(sourcePowers[sourceIndex]))
                {
                    continue;
                }

                var effectType = Enums.eEffectType.GrantPower;
                GBPA_ApplyIncarnateEnhancements(ref _mathPowers[targetIndex], targetIndex, sourcePowers[sourceIndex], false, ref effectType);
            }

            if (_mathPowers[targetIndex]!.Effects.Length == originalMathCount &&
                _buffedPowers[targetIndex]!.Effects.Length == originalBuffedCount)
            {
                continue;
            }

            _mathPowers[targetIndex]!.SetMathMag();
            _buffedPowers[targetIndex]!.SetMathMag();
        }
    }

    private void GBPA_ApplyCanonicalCrossPowerPowerBoostEnhancements(ref IPower powerMath, int hIDX, bool ignoreED)
    {
        var sourcePowers = _assembledBasePowers;
        for (var sourceIndex = 0; sourceIndex < _currentBuild.Powers.Count; sourceIndex++)
        {
            if (sourceIndex == hIDX ||
                !IsStaticAggregationSourcePowerEntry(sourceIndex, allowGlobalBoost: true) ||
                sourcePowers[sourceIndex] == null ||
                !SourceHasTaggedCrossPowerEnhancementEffects(sourcePowers[sourceIndex], ignoreED))
            {
                continue;
            }

            var effectType = Enums.eEffectType.Enhancement;
            GBPA_ApplyIncarnateEnhancements(
                ref powerMath,
                hIDX,
                sourcePowers[sourceIndex],
                ignoreED,
                ref effectType,
                allowTaggedStandardPowerCarrier: true);
        }
    }

    private static bool SourceHasTaggedCrossPowerEnhancementEffects(IPower? power, bool ignoreED)
    {
        return power?.Effects.Any(effect =>
            effect.EffectClass != Enums.eEffectClass.Ignored &&
            effect.IgnoreED == ignoreED &&
            effect.EffectType is Enums.eEffectType.Enhancement or Enums.eEffectType.DamageBuff &&
            GlobalBoostPlannerSemantics.IsPowerBoostTaggedEnhancementCarrierEffect(effect)) == true;
    }

    private static bool SourceHasCanonicalCrossPowerGlobalBoostEffects(IPower? power)
    {
        if (power == null)
        {
            return false;
        }

        return power.Effects.Any(effect =>
            effect.EffectClass != Enums.eEffectClass.Ignored &&
            effect.IgnoreED == false &&
            effect.EffectType is not (Enums.eEffectType.Enhancement or Enums.eEffectType.DamageBuff) &&
            TryResolveSemanticGlobalBoostSourcePower(power, effect, out _));
    }

    private static bool TryResolveSemanticGlobalBoostSourcePower(IPower ownerPower, IEffect effect, out IPower? semanticSource)
    {
        semanticSource = null;
        if (ownerPower == null || effect == null)
        {
            return false;
        }

        semanticSource = GlobalBoostPlannerSemantics.ResolveSemanticSourcePower(ownerPower, effect);
        return semanticSource is { PowerType: Enums.ePowerType.GlobalBoost };
    }

    private static void StampSemanticGlobalBoostSource(IEffect effect, IPower semanticSource)
    {
        if (effect == null || semanticSource == null)
        {
            return;
        }

        effect.Absorbed_Effect = true;
        effect.Absorbed_PowerType = semanticSource.PowerType;
        if (semanticSource.PowerIndex > -1)
        {
            effect.Absorbed_Power_nID = semanticSource.PowerIndex;
        }

        if (effect.Absorbed_Class_nID < 0 && semanticSource.GetPowerSet() is { nArchetype: >= 0 } powerset)
        {
            effect.Absorbed_Class_nID = powerset.nArchetype;
        }
    }

    private void GBPA_ApplyIncarnateEnhancements(
        ref IPower powerMath,
        int hIDX,
        IPower? power,
        bool ignoreED,
        ref Enums.eEffectType effectType,
        bool allowTaggedStandardPowerCarrier = false)
    {
        if (powerMath == null || power == null || power.Effects.Length == 0 || !powerMath.Slottable)
        {
            return;
        }

        for (var effectIndex = 0; effectIndex < power.Effects.Length; effectIndex++)
        {
            var effect = power.Effects[effectIndex];
            var hasSemanticGlobalBoostSource = TryResolveSemanticGlobalBoostSourcePower(power, effect, out var semanticSourcePower);
            var disqualified = false;
            if (effect.EffectClass == Enums.eEffectClass.Ignored)
            {
                disqualified = true;
            }
            else
            {
                switch (effectType)
                {
                    case Enums.eEffectType.Enhancement when effect.EffectType != Enums.eEffectType.Enhancement && effect.EffectType != Enums.eEffectType.DamageBuff:
                        disqualified = true;
                        break;
                    case Enums.eEffectType.GrantPower when effect.EffectType is Enums.eEffectType.Enhancement or Enums.eEffectType.DamageBuff:
                        disqualified = true;
                        break;
                    default:
                        if (effect.IgnoreED != ignoreED)
                        {
                            disqualified = true;
                        }
                        else if (!hasSemanticGlobalBoostSource &&
                                 power.PowerType != Enums.ePowerType.GlobalBoost &&
                                 (!allowTaggedStandardPowerCarrier ||
                                  !GlobalBoostPlannerSemantics.IsPowerBoostTaggedEnhancementCarrierEffect(effect)) &&
                                 (!effect.Absorbed_Effect || effect.Absorbed_PowerType != Enums.ePowerType.GlobalBoost))
                        {
                            disqualified = true;
                        }
                        else if (effect is { EffectType: Enums.eEffectType.GrantPower, Absorbed_Effect: true })
                        {
                            disqualified = true;
                        }

                        break;
                }
            }

            if (disqualified)
            {
                continue;
            }

            var sourcePower = hasSemanticGlobalBoostSource
                ? semanticSourcePower!
                : effect.Absorbed_Effect & effect.Absorbed_Power_nID > -1
                    ? DatabaseAPI.Database.Power[effect.Absorbed_Power_nID]
                    : power;
            var isAllowed = GlobalBoostPlannerSemantics.IsSourceEffectAllowedForPower(powerMath, sourcePower, effect);
            if (!isAllowed)
            {
                continue;
            }

            if (effectType == Enums.eEffectType.Enhancement &&
                effect.EffectType is Enums.eEffectType.DamageBuff or Enums.eEffectType.Enhancement)
            {
                var includeAccuracy = powerMath.IgnoreEnhancement(Enums.eEnhance.Accuracy);
                var includeRecharge = powerMath.IgnoreEnhancement(Enums.eEnhance.RechargeTime);
                var includeEndDiscount = powerMath.IgnoreEnhancement(Enums.eEnhance.EnduranceDiscount);
                switch (effect.ETModifies)
                {
                    case Enums.eEffectType.Accuracy when includeAccuracy:
                        powerMath.Accuracy += effect.BuffedMag;
                        continue;

                    case Enums.eEffectType.EnduranceDiscount when includeEndDiscount:
                        powerMath.EndCost += effect.BuffedMag;
                        continue;

                    case Enums.eEffectType.InterruptTime:
                        EnhancementPolicyAxes.ApplyInterruptEnhancement(powerMath, effect.BuffedMag);
                        continue;

                    case Enums.eEffectType.Range:
                        EnhancementPolicyAxes.ApplyRangeEnhancement(powerMath, effect.BuffedMag);
                        continue;

                    case Enums.eEffectType.RechargeTime when includeRecharge:
                        powerMath.RechargeTime += effect.BuffedMag;
                        continue;

                    default:
                        if (hIDX > -1 && hIDX < _buffedPowers.Length && _buffedPowers[hIDX] != null)
                        {
                            HandleDefaultIncarnateEnh(ref powerMath, sourcePower, effect, _buffedPowers[hIDX]!.Effects);
                        }

                        break;
                }
            }
            else if (effect.EffectType == Enums.eEffectType.GrantPower)
            {
                HandleGrantPowerIncarnate(ref powerMath, effect, _buffedPowers, effectIndex, _archetype, hIDX);
            }
            else
            {
                var powerMathLength = powerMath.Effects.Length;
                powerMath.AbsorbEffects(power, effect.Duration, 0, _archetype, 1, true, effectIndex, effectIndex);
                StampAbsorbedCrossPowerEffects(powerMath, powerMathLength);
                if (hasSemanticGlobalBoostSource)
                {
                    for (var index = powerMathLength; index < powerMath.Effects.Length; index++)
                    {
                        StampSemanticGlobalBoostSource(powerMath.Effects[index], semanticSourcePower!);
                    }
                }

                if (hIDX <= -1 || hIDX >= _buffedPowers.Length || _buffedPowers[hIDX] == null)
                {
                    continue;
                }

                var length = _buffedPowers[hIDX]!.Effects.Length;
                _buffedPowers[hIDX]!.AbsorbEffects(power, effect.Duration, 0, _archetype, 1, true, effectIndex, effectIndex);
                StampAbsorbedCrossPowerEffects(_buffedPowers[hIDX]!, length);
                for (var index = length; index < _buffedPowers[hIDX]!.Effects.Length; index++)
                {
                    _buffedPowers[hIDX]!.Effects[index].ToWho = effect.ToWho;
                    _buffedPowers[hIDX]!.Effects[index].Absorbed_Effect = true;
                    _buffedPowers[hIDX]!.Effects[index].isEnhancementEffect = effect.isEnhancementEffect;
                    _buffedPowers[hIDX]!.Effects[index].Ticks = effect.Ticks;
                    if (hasSemanticGlobalBoostSource)
                    {
                        StampSemanticGlobalBoostSource(_buffedPowers[hIDX]!.Effects[index], semanticSourcePower!);
                    }
                }
            }
        }
    }

    private bool GBPA_MultiplyVariable(ref IPower power, int hIDX)
    {
        if (power == null || hIDX < 0 || !power.VariableEnabled)
        {
            return false;
        }

        foreach (var effect in power.Effects)
        {
            if (effect.VariableModified && !effect.IgnoreScaling)
            {
                effect.Scale *= _currentBuild.Powers[hIDX].VariableValue;
            }
        }

        return true;
    }

    private FxIdShort[]? GetAllowedEffectsFromEnhance(Enums.eEnhance enhanceType)
    {
        return enhanceType switch
        {
            Enums.eEnhance.Defense =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.Defense, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None },
                new FxIdShort { EffectType = Enums.eEffectType.ResEffect, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.Defense }
            ],
            Enums.eEnhance.Heal =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.Heal, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None },
                new FxIdShort { EffectType = Enums.eEffectType.Absorb, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None },
                new FxIdShort { EffectType = Enums.eEffectType.Regeneration, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None },
                new FxIdShort { EffectType = Enums.eEffectType.HitPoints, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None },
                new FxIdShort { EffectType = Enums.eEffectType.ResEffect, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.Regeneration }
            ],
            Enums.eEnhance.Accuracy =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.Accuracy, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.EnduranceDiscount =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.EnduranceDiscount, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.Endurance =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.Endurance, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.SpeedFlying =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.SpeedFlying, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.Interrupt =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.InterruptTime, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.JumpHeight =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.JumpHeight, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.SpeedJumping =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.SpeedJumping, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.Range =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.Range, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.RechargeTime or Enums.eEnhance.X_RechargeTime =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.RechargeTime, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.Recovery =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.Recovery, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.SpeedRunning =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.SpeedRunning, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.ToHit =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.ToHit, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            Enums.eEnhance.Slow =>
            [
                new FxIdShort { EffectType = Enums.eEffectType.SpeedRunning, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None },
                new FxIdShort { EffectType = Enums.eEffectType.SpeedJumping, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None },
                new FxIdShort { EffectType = Enums.eEffectType.SpeedFlying, MezType = Enums.eMez.None, ETModifies = Enums.eEffectType.None }
            ],
            _ => null
        };
    }

    private FxIdShort[]? GetAllowedEffectsFromEnhance(I9Slot enhancement)
    {
        if (enhancement.Enh < 0)
        {
            return null;
        }

        return DatabaseAPI.Database.Enhancements[enhancement.Enh].Effect
            .SelectMany(effect => GetAllowedEffectsFromEnhance((Enums.eEnhance)effect.Enhance.ID) ?? Array.Empty<FxIdShort>())
            .Distinct()
            .ToArray();
    }

    private bool CheckAllowedFromFx(FxIdShort[]? fxIdList, Enums.eEffectType effectType, Enums.eMez mezType = Enums.eMez.None, Enums.eEffectType etModifies = Enums.eEffectType.None)
    {
        if (fxIdList == null)
        {
            return true;
        }

        var fxId = new FxIdShort { EffectType = effectType, MezType = mezType, ETModifies = etModifies };
        return fxIdList.Contains(fxId);
    }

    private bool GBPA_Pass1_EnhancePreED(ref IPower powerMath, int hIDX)
    {
        if (hIDX < 0 || _currentBuild.Powers[hIDX].NIDPowerset < 0)
        {
            return false;
        }

        powerMath.Accuracy = 0;
        powerMath.EndCost = 0;
        powerMath.InterruptTime = 0;
        powerMath.Range = 0;
        powerMath.RechargeTime = 0;
        foreach (var effect in powerMath.Effects)
        {
            effect.Math_Mag = 0;
            effect.Math_Duration = 0;
        }

        var isAccuracy = DatabaseAPI.Database.Power[_currentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.Accuracy);
        var isRecharge = DatabaseAPI.Database.Power[_currentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.RechargeTime);
        var isEndurance = DatabaseAPI.Database.Power[_currentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.EnduranceDiscount);
        var effectTypeCount = Enum.GetValues(typeof(Enums.eEffectType)).Length;

        for (var index = 0; index < _currentBuild.Powers[hIDX].SlotCount; index++)
        {
            if (!(_currentBuild.Powers[hIDX].Slots[index].Enhancement.Enh > -1 &
                  _currentBuild.Powers[hIDX].Slots[index].Level < MidsContext.Config.ForceLevel))
            {
                continue;
            }

            var enhancement = _currentBuild.Powers[hIDX].Slots[index].Enhancement;
            if (isAccuracy)
            {
                powerMath.Accuracy += enhancement.GetEnhancementEffect(Enums.eEnhance.Accuracy, -1, 1);
            }

            if (isEndurance)
            {
                powerMath.EndCost += enhancement.GetEnhancementEffect(Enums.eEnhance.EnduranceDiscount, -1, 1);
            }

            EnhancementPolicyAxes.ApplyInterruptEnhancement(
                powerMath,
                enhancement.GetEnhancementEffect(Enums.eEnhance.Interrupt, -1, 1));
            EnhancementPolicyAxes.ApplyRangeEnhancement(
                powerMath,
                enhancement.GetEnhancementEffect(Enums.eEnhance.Range, -1, 1));
            if (isRecharge)
            {
                powerMath.RechargeTime += enhancement.GetEnhancementEffect(Enums.eEnhance.RechargeTime, -1, 1);
            }

            for (var effectIndex = 0; effectIndex < powerMath.Effects.Length; effectIndex++)
            {
                if (!powerMath.Effects[effectIndex].Buffable ||
                    PlannerStrengthSemantics.IgnoresStrength(powerMath.Effects[effectIndex]))
                {
                    continue;
                }

                for (var effectTypeIndex = 0; effectTypeIndex < effectTypeCount; effectTypeIndex++)
                {
                    if (powerMath.Effects[effectIndex].EffectType != (Enums.eEffectType)effectTypeIndex)
                    {
                        continue;
                    }

                    var duration = 0f;
                    var effectType = (Enums.eEffectType)effectTypeIndex;
                    var isMapped = Enums.IsEnumValue(Enum.GetName(typeof(Enums.eEffectType), effectType), Enums.eEnhance.None);
                    var specialAccuracy = false;
                    if (!isMapped)
                    {
                        if (powerMath.Effects[effectIndex].EffectType == Enums.eEffectType.Enhancement &&
                            powerMath.Effects[effectIndex].ETModifies == Enums.eEffectType.Accuracy)
                        {
                            isMapped = true;
                            specialAccuracy = true;
                        }
                        else if (powerMath.Effects[effectIndex].EffectType == Enums.eEffectType.ResEffect &&
                                 powerMath.Effects[effectIndex].ETModifies == Enums.eEffectType.Defense)
                        {
                            isMapped = true;
                        }
                    }

                    if (!isMapped)
                    {
                        var allowedEffects = GetAllowedEffectsFromEnhance(enhancement);
                        if (allowedEffects == null ||
                            !CheckAllowedFromFx(allowedEffects,
                                powerMath.Effects[effectIndex].EffectType,
                                powerMath.Effects[effectIndex].MezType,
                                powerMath.Effects[effectIndex].ETModifies))
                        {
                            continue;
                        }
                    }

                    var enhanceType = !specialAccuracy
                        ? effectType switch
                        {
                            Enums.eEffectType.MezProtect => Enums.eEnhance.Mez,
                            Enums.eEffectType.MezResist => Enums.eEnhance.Mez,
                            _ => (Enums.eEnhance)Enums.StringToFlaggedEnum(
                                Enum.GetName(typeof(Enums.eEffectType), effectType),
                                Enums.eEnhance.None)
                        }
                        : Enums.eEnhance.Accuracy;

                    var magnitude = effectType is Enums.eEffectType.Mez or Enums.eEffectType.MezProtect
                        ? enhancement.GetEnhancementEffect(enhanceType, (int)powerMath.Effects[effectIndex].MezType, _buffedPowers[hIDX]!.Effects[effectIndex].Math_Mag)
                        : effectType == Enums.eEffectType.ResEffect && powerMath.Effects[effectIndex].ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Regeneration
                            ? powerMath.Effects[effectIndex].ETModifies switch
                            {
                                Enums.eEffectType.Defense => enhancement.GetEnhancementEffect(Enums.eEnhance.Defense, -1, _buffedPowers[hIDX]!.Effects[effectIndex].Math_Mag),
                                Enums.eEffectType.Regeneration => enhancement.GetEnhancementEffect(Enums.eEnhance.Heal, -1, _buffedPowers[hIDX]!.Effects[effectIndex].Math_Mag)
                            }
                            : enhancement.GetEnhancementEffect(enhanceType, -1, _buffedPowers[hIDX]!.Effects[effectIndex].Math_Mag);

                    if (effectType == Enums.eEffectType.Damage && powerMath.Effects[effectIndex].DamageType == Enums.eDamage.Special)
                    {
                        magnitude = 0;
                    }
                    else if (effectType is Enums.eEffectType.Mez or Enums.eEffectType.MezProtect && powerMath.Effects[effectIndex].AttribType == Enums.eAttribType.Duration)
                    {
                        duration = magnitude;
                        magnitude = 0;
                    }

                    powerMath.Effects[effectIndex].Math_Mag += magnitude;
                    powerMath.Effects[effectIndex].Math_Duration += duration;
                }
            }
        }

        if (DatabaseAPI.GetPlannerRuleset().AllowLegacyCrossPowerIncarnatePasses)
        {
            for (var index = 0; index < _currentBuild.Powers.Count; index++)
            {
                if (!IsStaticAggregationSourcePowerEntry(index, allowGlobalBoost: true))
                {
                    continue;
                }

                var effectType = Enums.eEffectType.Enhancement;
                GBPA_ApplyIncarnateEnhancements(ref powerMath, hIDX, _mathPowers[index], false, ref effectType);
            }
        }
        else if (DatabaseAPI.GetPlannerRuleset().UsesCanonicalPlannerMath)
        {
            GBPA_ApplyCanonicalCrossPowerPowerBoostEnhancements(ref powerMath, hIDX, false);
        }

        return false;
    }

    private static bool GBPA_Pass2_ApplyED(ref IPower powerMath)
    {
        powerMath.Accuracy = Enhancement.ApplyED(Enums.eEnhance.Accuracy, powerMath.Accuracy);
        powerMath.EndCost = Enhancement.ApplyED(Enums.eEnhance.EnduranceDiscount, powerMath.EndCost);
        powerMath.InterruptTime = Enhancement.ApplyED(Enums.eEnhance.Interrupt, powerMath.InterruptTime);
        powerMath.Range = Enhancement.ApplyED(Enums.eEnhance.Range, powerMath.Range);
        powerMath.RechargeTime = Enhancement.ApplyED(Enums.eEnhance.RechargeTime, powerMath.RechargeTime);
        foreach (var effect in powerMath.Effects)
        {
            if (effect.isEnhancementEffect)
            {
                continue;
            }

            for (var index = 0; index < Enum.GetValues<Enums.eEffectType>().Length; index++)
            {
                if (effect.EffectType != (Enums.eEffectType)index)
                {
                    continue;
                }

                var enhanceType = Enums.eEnhance.None;
                var effectType = (Enums.eEffectType)index;
                var isMapped = Enums.IsEnumValue(Enum.GetName(effectType.GetType(), effectType), enhanceType);
                var specialAccuracy = false;
                if (!isMapped)
                {
                    if (effect.EffectType == Enums.eEffectType.Enhancement && effect.ETModifies == Enums.eEffectType.Accuracy)
                    {
                        isMapped = true;
                        specialAccuracy = true;
                    }
                    else if (effect.EffectType == Enums.eEffectType.ResEffect && effect.ETModifies == Enums.eEffectType.Defense)
                    {
                        isMapped = true;
                    }
                }

                if (!isMapped)
                {
                    continue;
                }

                var scheduleEnhance = !specialAccuracy
                    ? effectType switch
                    {
                        Enums.eEffectType.MezProtect => Enums.eEnhance.Mez,
                        Enums.eEffectType.MezResist => Enums.eEnhance.Mez,
                        _ => (Enums.eEnhance)Enums.StringToFlaggedEnum(Enum.GetName(effectType.GetType(), effectType), enhanceType)
                    }
                    : Enums.eEnhance.Accuracy;
                var diversificationMode = effect.buffMode switch
                {
                    Enums.eBuffMode.Buff => Enums.eBuffDebuff.BuffOnly,
                    Enums.eBuffMode.Debuff => Enums.eBuffDebuff.DeBuffOnly,
                    _ => Enums.eBuffDebuff.Any
                };

                if (effectType is Enums.eEffectType.Mez or Enums.eEffectType.MezProtect)
                {
                    effect.Math_Mag = Enhancement.ApplyED(scheduleEnhance, effect.Math_Mag, diversificationMode, (int)effect.MezType);
                    effect.Math_Duration = Enhancement.ApplyED(scheduleEnhance, effect.Math_Duration, diversificationMode, (int)effect.MezType);
                }
                else
                {
                    effect.Math_Mag = !(effectType == Enums.eEffectType.ResEffect && effect.ETModifies == Enums.eEffectType.Defense)
                        ? Enhancement.ApplyED(scheduleEnhance, effect.Math_Mag, diversificationMode)
                        : Enhancement.ApplyED(Enums.eEnhance.Defense, effect.Math_Mag, diversificationMode);
                }
            }
        }

        return true;
    }

    private bool GBPA_Pass3_EnhancePostED(ref IPower powerMath, int hIDX)
    {
        var includeAccuracy = DatabaseAPI.Database.Power[_currentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.Accuracy);
        var includeRecharge = DatabaseAPI.Database.Power[_currentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.RechargeTime);
        var includeEndurance = DatabaseAPI.Database.Power[_currentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.EnduranceDiscount);
        for (var effectTypeIndex = 0; effectTypeIndex < _selfEnhance.Effect.Length; effectTypeIndex++)
        {
            var effectType = (Enums.eEffectType)effectTypeIndex;
            switch (effectType)
            {
                case Enums.eEffectType.Accuracy:
                    if (includeAccuracy)
                    {
                        powerMath.Accuracy += _selfEnhance.Effect[effectTypeIndex];
                    }

                    break;
                case Enums.eEffectType.EnduranceDiscount:
                    if (includeEndurance)
                    {
                        powerMath.EndCost += _selfEnhance.Effect[effectTypeIndex];
                    }

                    break;
                case Enums.eEffectType.InterruptTime:
                    EnhancementPolicyAxes.ApplyInterruptEnhancement(powerMath, _selfEnhance.Effect[effectTypeIndex]);
                    break;
                case Enums.eEffectType.Range:
                    EnhancementPolicyAxes.ApplyRangeEnhancement(powerMath, _selfEnhance.Effect[effectTypeIndex]);
                    break;
                case Enums.eEffectType.RechargeTime:
                    if (includeRecharge)
                    {
                        powerMath.RechargeTime += _selfEnhance.Effect[effectTypeIndex];
                    }

                    break;
                default:
                    for (var index = 0; index < powerMath.Effects.Length; index++)
                    {
                        if (!powerMath.Effects[index].Buffable ||
                            powerMath.Effects[index].EffectType != effectType ||
                            PlannerStrengthSemantics.IgnoresStrength(powerMath.Effects[index]))
                        {
                            continue;
                        }

                        var duration = 0f;
                        var magnitude = 0f;
                        switch (effectType)
                        {
                            case Enums.eEffectType.Damage:
                                if (powerMath.Effects[index].DamageType != Enums.eDamage.None)
                                {
                                    magnitude += _selfEnhance.Damage[(int)powerMath.Effects[index].DamageType];
                                }

                                break;
                            case Enums.eEffectType.Defense:
                                if (powerMath.Effects[index].DamageType != Enums.eDamage.None)
                                {
                                    magnitude += _selfEnhance.Defense[(int)powerMath.Effects[index].DamageType];
                                }

                                break;
                            case Enums.eEffectType.Mez:
                            case Enums.eEffectType.MezProtect:
                                if (powerMath.Effects[index].AttribType == Enums.eAttribType.Duration)
                                {
                                    duration += _selfEnhance.Mez[(int)powerMath.Effects[index].MezType];
                                }
                                else
                                {
                                    magnitude += _selfEnhance.Mez[(int)powerMath.Effects[index].MezType];
                                }

                                break;
                            case Enums.eEffectType.Resistance:
                                if (powerMath.Effects[index].DamageType != Enums.eDamage.None)
                                {
                                    magnitude += _selfEnhance.Resistance[(int)powerMath.Effects[index].DamageType];
                                }

                                break;
                            default:
                                var effect = powerMath.Effects[index];
                                if (effect is
                                    {
                                        EffectType: Enums.eEffectType.Enhancement,
                                        ETModifies: Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping or Enums.eEffectType.JumpHeight or Enums.eEffectType.SpeedFlying
                                    })
                                {
                                    if (_buffedPowers[hIDX]!.Effects[index].Mag > 0)
                                    {
                                        magnitude = _selfEnhance.Effect[(int)effect.ETModifies];
                                    }

                                    if (_buffedPowers[hIDX]!.Effects[index].Mag < 0)
                                    {
                                        magnitude = _selfEnhance.EffectAux[(int)effect.ETModifies];
                                    }

                                    break;
                                }

                                if (effect.EffectType is Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping or Enums.eEffectType.JumpHeight or Enums.eEffectType.SpeedFlying)
                                {
                                    if (_buffedPowers[hIDX]!.Effects[index].Mag > 0)
                                    {
                                        magnitude = _selfEnhance.Effect[(int)effect.EffectType];
                                    }

                                    if (_buffedPowers[hIDX]!.Effects[index].Mag < 0)
                                    {
                                        magnitude = _selfEnhance.EffectAux[(int)effect.EffectType];
                                    }

                                    break;
                                }

                                magnitude = _selfEnhance.Effect[effectTypeIndex];
                                break;
                        }

                        powerMath.Effects[index].Math_Mag += magnitude;
                        powerMath.Effects[index].Math_Duration += duration;
                    }

                    break;
            }
        }

        if (DatabaseAPI.GetPlannerRuleset().AllowLegacyCrossPowerIncarnatePasses)
        {
            for (var index = 0; index < _currentBuild.Powers.Count; index++)
            {
                if (!IsStaticAggregationSourcePowerEntry(index, allowGlobalBoost: true))
                {
                    continue;
                }

                var effectType = Enums.eEffectType.Enhancement;
                GBPA_ApplyIncarnateEnhancements(ref powerMath, hIDX, _mathPowers[index], true, ref effectType);
            }
        }
        else if (DatabaseAPI.GetPlannerRuleset().UsesCanonicalPlannerMath)
        {
            GBPA_ApplyCanonicalCrossPowerPowerBoostEnhancements(ref powerMath, hIDX, true);
        }

        return true;
    }

    private static bool GBPA_Pass4_Add(ref IPower powerMath)
    {
        ++powerMath.EndCost;
        ++powerMath.InterruptTime;
        ++powerMath.Range;
        ++powerMath.RechargeTime;
        foreach (var effect in powerMath.Effects)
        {
            ++effect.Math_Mag;
            ++effect.Math_Duration;
        }

        return true;
    }

    private static void GBPA_Pass5_ResyncEffects(ref IPower powerMath, ref IPower powerBuffed)
    {
        var length = Math.Min(powerMath.Effects.Length, powerBuffed.Effects.Length);
        var mathEffects = powerMath.Effects.ToList();
        var buffedEffects = powerBuffed.Effects.ToList();
        for (var index = 0; index < length; index++)
        {
            if (mathEffects[index].EffectType == buffedEffects[index].EffectType &&
                mathEffects[index].DamageType == buffedEffects[index].DamageType &&
                mathEffects[index].MezType == buffedEffects[index].MezType &&
                mathEffects[index].ETModifies == buffedEffects[index].ETModifies &&
                mathEffects[index].Summon == buffedEffects[index].Summon)
            {
                continue;
            }

            mathEffects.RemoveAt(index);
        }

        powerMath.Effects = mathEffects.ToArray();
    }

    private static bool GBPA_Pass5_MultiplyPreBuff(ref IPower powerMath, ref IPower powerBuffed)
    {
        if (powerBuffed == null)
        {
            return false;
        }

        powerBuffed.EndCost /= powerMath.EndCost;
        powerBuffed.InterruptTime /= powerMath.InterruptTime;
        powerBuffed.Range *= powerMath.Range;
        powerBuffed.RechargeTime /= powerMath.RechargeTime;

        if (powerMath.Effects.Length > powerBuffed.Effects.Length)
        {
            GBPA_Pass5_ResyncEffects(ref powerMath, ref powerBuffed);
        }

        for (var index = 0; index < powerMath.Effects.Length; index++)
        {
            powerBuffed.Effects[index].Math_Mag = powerBuffed.Effects[index].Mag * powerMath.Effects[index].Math_Mag;
            powerBuffed.Effects[index].Math_Duration = powerBuffed.Effects[index].Duration * powerMath.Effects[index].Math_Duration;
        }

        return true;
    }

    private bool GBPA_Pass6_MultiplyPostBuff(ref IPower? powerMath, ref IPower? powerBuffed)
    {
        if (powerMath == null || powerBuffed == null || MidsContext.Config is null)
        {
            return false;
        }

        var plannerRuleset = DatabaseAPI.GetPlannerRuleset();
        var toHit = powerMath.IgnoreBuff(Enums.eEnhance.ToHit) ? _selfBuffs.Effect[(int)Enums.eStatType.ToHit] : 0;
        var accuracy = powerMath.IgnoreBuff(Enums.eEnhance.Accuracy) ? _selfBuffs.Effect[(int)Enums.eStatType.BuffAcc] : 0;
        var combatToHitScale = plannerRuleset.GetCombatModToHitScale(_combatContext);
        var combatAccuracyScale = plannerRuleset.GetCombatModAccuracyScale(_combatContext);
        var combatMagnitudeScale = plannerRuleset.GetCombatModMagnitudeScale(_combatContext);
        var combatDurationScale = plannerRuleset.GetCombatModDurationScale(_combatContext);

        ApplyDisplayedSelfBuffScalars(powerMath, powerBuffed, _selfBuffs);

        var accuracyMultiplier = (1 + powerMath.Accuracy + accuracy) * combatAccuracyScale;
        powerBuffed.Accuracy *= accuracyMultiplier * (combatToHitScale + toHit);
        powerBuffed.AccuracyMult *= accuracyMultiplier;

        if (_combatContext.UsesFullCombatModTables && PowerUsesEnemyCombatContext(powerBuffed))
        {
            foreach (var effect in powerBuffed.Effects)
            {
                if (effect.UseCombatModMagnitude)
                {
                    effect.Math_Mag *= combatMagnitudeScale;
                }

                if (effect.UseCombatModDuration)
                {
                    effect.Math_Duration *= combatDurationScale;
                }
            }
        }

        return true;
    }

    internal static void ApplyDisplayedSelfBuffScalars(IPower powerMath, IPower powerBuffed, Enums.BuffsX selfBuffs)
    {
        if (powerMath.IgnoreBuff(Enums.eEnhance.Damage))
        {
            foreach (var effect in powerBuffed.Effects)
            {
                if (effect.EffectType != Enums.eEffectType.Damage)
                {
                    continue;
                }

                var damageIndex = (int)effect.DamageType;
                if (damageIndex < 0 || damageIndex >= selfBuffs.Damage.Length)
                {
                    continue;
                }

                var damageBuff = selfBuffs.Damage[damageIndex];
                if (Math.Abs(damageBuff) > float.Epsilon &&
                    !PlannerStrengthSemantics.IgnoresStrength(effect))
                {
                    effect.Math_Mag *= 1f + damageBuff;
                }
            }
        }

        for (var effectIndex = 0; effectIndex < powerMath.Effects.Length && effectIndex < powerBuffed.Effects.Length; effectIndex++)
        {
            var mathEffect = powerMath.Effects[effectIndex];
            var buffedEffect = powerBuffed.Effects[effectIndex];
            if (mathEffect.EffectType is not (Enums.eEffectType.Mez or Enums.eEffectType.MezProtect))
            {
                continue;
            }

            var mezIndex = (int)mathEffect.MezType;
            if (mezIndex < 0 || mezIndex >= selfBuffs.Mez.Length)
            {
                continue;
            }

            var mezBuff = selfBuffs.Mez[mezIndex];
            if (Math.Abs(mezBuff) <= float.Epsilon)
            {
                continue;
            }

            if (PlannerStrengthSemantics.IgnoresStrength(mathEffect))
            {
                continue;
            }

            if (mathEffect.AttribType == Enums.eAttribType.Duration)
            {
                buffedEffect.Math_Duration *= 1f + mezBuff;
            }
            else
            {
                buffedEffect.Math_Mag *= 1f + mezBuff;
            }
        }

        if (powerMath.IgnoreBuff(Enums.eEnhance.EnduranceDiscount))
        {
            var endDiscount = selfBuffs.Effect[(int)Enums.eStatType.BuffEndRdx];
            if (Math.Abs(endDiscount) > float.Epsilon)
            {
                var divisor = 1f + endDiscount;
                if (divisor > float.Epsilon)
                {
                    powerBuffed.EndCost /= divisor;
                }
            }
        }

        if (powerMath.IgnoreBuff(Enums.eEnhance.RechargeTime))
        {
            var rechargeBuff = selfBuffs.Effect[(int)Enums.eStatType.Haste];
            var divisor = 1f + rechargeBuff;
            if (Math.Abs(divisor) > float.Epsilon)
            {
                powerBuffed.RechargeTime /= divisor;
            }
        }
    }

    private PlannerActorAggregationResult BuildActorAggregation(PlannerActorAggregationContext context, bool finalize = false)
    {
        return finalize
            ? PlannerActorAggregationPhase.Finalize(context)
            : PlannerActorAggregationPhase.Assemble(context);
    }

    private PlannerActorAggregationContext CreateActorAggregationContext(bool buildChanceModifierCatalog)
    {
        var plannerRuleset = DatabaseAPI.GetPlannerRuleset();
        var includedMathPowers = new List<IPower>();
        var includedBuffedPowers = new List<IPower>();
        var includedSelfBuffPowers = new List<IPower>();
        for (var index = 0; index < _currentBuild.Powers.Count; index++)
        {
            var powerEntry = _currentBuild.Powers[index];
            if (powerEntry == null)
            {
                continue;
            }

            if (!IsStaticAggregationSourcePowerEntry(index))
            {
                continue;
            }

            var sourcePower = powerEntry.Power ?? _buffedPowers[index] ?? _mathPowers[index];
            var excludeManagedComputedPower = _recipient is null or { Kind: PlannerBuildRecipientKind.Player } &&
                                             (VigilancePlanner.IsManagedComputedPower(sourcePower) ||
                                              CosmicBalancePlanner.IsManagedComputedPower(sourcePower) ||
                                              DarkSustenancePlanner.IsManagedComputedPower(sourcePower));

            if (!excludeManagedComputedPower && _mathPowers[index] != null)
            {
                includedMathPowers.Add(_mathPowers[index]!);
            }

            if (!excludeManagedComputedPower && _buffedPowers[index] != null)
            {
                includedBuffedPowers.Add(_buffedPowers[index]!);
            }

            if (!excludeManagedComputedPower)
            {
                var selfBuffSourcePower = _preBuffPowers[index] ?? _buffedPowers[index];
                if (selfBuffSourcePower != null)
                {
                    includedSelfBuffPowers.Add(selfBuffSourcePower);
                }
            }
        }

        var setBonusPower = _recipient == null
            ? _currentBuild.SetBonusVirtualPower
            : _currentBuild.GetSetBonusVirtualPower(_recipient);

        var enhancementExternalPowers = new List<IPower>();
        var selfBuffExternalPowers = new List<IPower>();
        var computedDefianceMagnitude = 0f;
        var computedVigilanceDamageMagnitude = 0f;
        var computedVigilanceEndDiscountMagnitude = 0f;
        var computedCosmicBalanceState = new CosmicBalanceComputedState();
        var computedDarkSustenanceState = new CosmicBalanceComputedState();
        IReadOnlyDictionary<string, float>? supplementalChanceModifierCatalog = null;
        if (setBonusPower != null)
        {
            enhancementExternalPowers.Add(setBonusPower);
            selfBuffExternalPowers.Add(setBonusPower);
        }

        if (_recipient != null && _recipientExternalPowers.Count > 0)
        {
            enhancementExternalPowers.AddRange(_recipientExternalPowers);
            selfBuffExternalPowers.AddRange(_recipientExternalPowers);
        }

        if (plannerRuleset.IncludePvpResistanceBonusInBuckets(PlannerBucketPass.Enhancement) ||
            plannerRuleset.IncludePvpResistanceBonusInBuckets(PlannerBucketPass.SelfBuff))
        {
            var pvpResistIndex = DatabaseAPI.NidFromUidPower("Temporary_Powers.Temporary_Powers.PVP_Resist_Bonus");
            if (pvpResistIndex > -1)
            {
                IPower pvpResistPower = new Power(DatabaseAPI.Database.Power[pvpResistIndex]);
                if (_recipient != null)
                {
                    pvpResistPower = OmniPowerRouting.CreatePlannerPower(pvpResistPower, _recipient) ?? new Power();
                }

                if (plannerRuleset.IncludePvpResistanceBonusInBuckets(PlannerBucketPass.Enhancement))
                {
                    enhancementExternalPowers.Add(pvpResistPower);
                }

                if (plannerRuleset.IncludePvpResistanceBonusInBuckets(PlannerBucketPass.SelfBuff))
                {
                    selfBuffExternalPowers.Add(pvpResistPower);
                }
            }
        }

        if (_recipient is null or { Kind: PlannerBuildRecipientKind.Player })
        {
            computedDefianceMagnitude = DefiancePlanner.Resolve(
                _currentBuild,
                MidsContext.Config?.CombatContextSettings.Defiance).TotalMagnitude;
            computedVigilanceDamageMagnitude = VigilancePlanner.GetComputedDamageBuffMagnitude(MidsContext.Config);
            computedVigilanceEndDiscountMagnitude = VigilancePlanner.GetComputedEnduranceDiscountMagnitude(MidsContext.Config);
            computedCosmicBalanceState = CosmicBalancePlanner.GetComputedState(MidsContext.Config);
            computedDarkSustenanceState = DarkSustenancePlanner.GetComputedState(MidsContext.Config);
            supplementalChanceModifierCatalog = AssassinationPlanner.BuildSupplementalChanceModifierCatalog(
                _currentBuild,
                MidsContext.Config?.CombatContextSettings.Assassination);
        }

        return new PlannerActorAggregationContext
        {
            ClassName = _recipient?.ClassName ?? DatabaseAPI.ResolveClassName(_archetype),
            Archetype = _recipient == null ? _archetype : DatabaseAPI.GetArchetypeByClassName(_recipient.ClassName),
            MathPowers = _mathPowers.OfType<IPower>().ToArray(),
            BuffedPowers = _buffedPowers.OfType<IPower>().ToArray(),
            IncludedMathPowers = includedMathPowers,
            IncludedBuffedPowers = includedBuffedPowers,
            IncludedSelfBuffPowers = includedSelfBuffPowers,
            EnhancementExternalPowers = enhancementExternalPowers,
            SelfBuffExternalPowers = selfBuffExternalPowers,
            ComputedDefianceMagnitude = computedDefianceMagnitude,
            ComputedVigilanceDamageMagnitude = computedVigilanceDamageMagnitude,
            ComputedVigilanceEndDiscountMagnitude = computedVigilanceEndDiscountMagnitude,
            CosmicBalanceState = computedCosmicBalanceState,
            DarkSustenanceState = computedDarkSustenanceState,
            ChanceModifierSetBonusPower = setBonusPower,
            SupplementalChanceModifierCatalog = supplementalChanceModifierCatalog,
            BuildChanceModifierCatalog = buildChanceModifierCatalog,
            ApplyPvpDiminishingReturns = _recipient == null
        };
    }

    private static void RemoveExactDuplicateAbsorbedEffects(ref IPower? power)
    {
        if (power == null || power.Effects.Length <= 1)
        {
            return;
        }

        var absorbedEffects = power.Effects
            .Where(effect => effect.Absorbed_Effect)
            .ToArray();
        if (absorbedEffects.Length <= 1)
        {
            return;
        }

        var reducedAbsorbed = PlannerStackRules.ReducePlannerVisibleCopies(absorbedEffects);
        if (reducedAbsorbed.Count == absorbedEffects.Length)
        {
            return;
        }

        var reducedSet = new HashSet<IEffect>(reducedAbsorbed);
        power.Effects = power.Effects
            .Where(effect => !effect.Absorbed_Effect || reducedSet.Remove(effect))
            .ToArray();
        foreach (var effect in power.Effects)
        {
            effect.SetPower(power);
        }
    }

    private static bool PowerUsesEnemyCombatContext(IPower? power)
    {
        if (power == null)
        {
            return false;
        }

        var hostileTargets = Enums.eEntity.Foe |
                             Enums.eEntity.DeadFoe |
                             Enums.eEntity.DeadOrAliveFoe |
                             Enums.eEntity.FoeRezzingFoe |
                             Enums.eEntity.Villain |
                             Enums.eEntity.DeadVillain |
                             Enums.eEntity.NPC |
                             Enums.eEntity.Any;

        return (power.EntitiesAffected & hostileTargets) != Enums.eEntity.None;
    }

    private static bool IsCanonicalCrossPowerGlobalBoostProcHostPower(IPower? power)
    {
        if (power == null || power.ClickBuff || !PowerUsesEnemyCombatContext(power))
        {
            return false;
        }

        if (power.AttackTypes != Enums.eVector.None)
        {
            return true;
        }

        return power.Effects.Any(effect =>
            effect.EffectType == Enums.eEffectType.Damage &&
            effect.ToWho is not (Enums.eToWho.Self or Enums.eToWho.All));
    }

    private bool IsStaticAggregationSourcePowerEntry(int historyIndex, bool allowGlobalBoost = false)
    {
        if (historyIndex < 0 || historyIndex >= _currentBuild.Powers.Count)
        {
            return false;
        }

        var powerEntry = _currentBuild.Powers[historyIndex];
        if (powerEntry == null || !powerEntry.StatInclude || powerEntry.NIDPower <= -1)
        {
            return false;
        }

        var power = powerEntry.Power ?? _assembledBasePowers[historyIndex] ?? _buffedPowers[historyIndex] ?? _mathPowers[historyIndex];
        if (power == null)
        {
            return false;
        }

        if (!allowGlobalBoost && power.PowerType == Enums.ePowerType.GlobalBoost)
        {
            return false;
        }

        return IsStaticAggregationSourcePower(powerEntry, power);
    }

    private static bool IsStaticAggregationSourcePower(PowerEntry powerEntry, IPower power)
    {
        if (powerEntry == null || power == null)
        {
            return false;
        }

        if (PowerEntry.ShouldForceAutoIncluded(power) ||
            PowerEntry.IsVisiblePlannerModeControl(power))
        {
            return true;
        }

        return power.PowerType switch
        {
            Enums.ePowerType.Auto_ => true,
            Enums.ePowerType.Toggle => true,
            Enums.ePowerType.Click when power.ClickBuff => true,
            _ => false
        };
    }
}

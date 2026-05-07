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
    private IPower?[] _basePowers = Array.Empty<IPower?>();
    private IPower?[] _assembledBasePowers = Array.Empty<IPower?>();
    private IPower?[] _buffedPowers = Array.Empty<IPower?>();
    private IPower?[] _mathPowers = Array.Empty<IPower?>();
    private IPower?[] _preBuffPowers = Array.Empty<IPower?>();
    private Enums.BuffsX _selfBuffs;
    private Enums.BuffsX _selfEnhance;
    private PlannerCombatContext _combatContext = PlannerCombatContext.Default;

    private struct FxIdShort
    {
        public Enums.eEffectType EffectType;
        public Enums.eMez MezType;
        public Enums.eEffectType ETModifies;
    }

    public PlannerPowerPipeline(Build currentBuild, Archetype? archetype, PlannerBuildRecipientContext? recipient = null)
    {
        _currentBuild = currentBuild;
        _archetype = archetype;
        _recipient = recipient;
        Result = new PlannerPowerPipelineResult();
        SyncResult();
    }

    public PlannerPowerPipelineResult Result { get; }

    public void ExecuteAssemblyPhase()
    {
        _selfBuffs.Reset();
        _selfEnhance.Reset();
        _combatContext = PlannerCombatContext.Default;
        _basePowers = new IPower?[_currentBuild.Powers.Count];
        _assembledBasePowers = new IPower?[_currentBuild.Powers.Count];
        _buffedPowers = new IPower?[_currentBuild.Powers.Count];
        _mathPowers = new IPower?[_currentBuild.Powers.Count];
        _preBuffPowers = new IPower?[_currentBuild.Powers.Count];
        GBPA_Pass0_InitializePowerArray();
        SyncResult();
    }

    public void ExecuteEnhancementBucketPhase()
    {
        GenerateBuffData(ref _selfEnhance, PlannerBucketPass.Enhancement);
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
        GenerateBuffData(ref _selfBuffs, PlannerBucketPass.SelfBuff);
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

        SyncResult();
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

            if (powerEntry.Power != null && powerEntry.Power.Stacks < powerEntry.VariableValue)
            {
                powerEntry.Power.Stacks = powerEntry.VariableValue;
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
                    if ((index1 != index2 & _currentBuild.Powers[index2]?.StatInclude & _currentBuild.Powers[index2]?.NIDPower > -1) == false)
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
            power.Stacks = _currentBuild.Powers[hIDX]!.VariableValue;
        }

        return DatabaseAPI.GetPlannerRuleset().UsesCanonicalPlannerMath
            ? AssemblePlannerEffectivePowerEntryOmni(power, hIDX, stackingOverride)
            : AssemblePlannerEffectivePowerEntryLegacy(power, hIDX, stackingOverride);
    }

    private IPower AssemblePlannerEffectivePowerEntryLegacy(IPower power, int hIDX, int stackingOverride)
    {
        power = PlannerEffectResolver.ApplyRedirect(power);
        GBPA_AddEnhFX(ref power, hIDX);
        power.AbsorbPetEffects(hIDX, stackingOverride, pseudoOnly: true);
        PlannerEffectResolver.ExpandEffects(power,
            DatabaseAPI.GetPlannerRuleset().CreateAssemblyExpansionContext(hIDX, stackingOverride));
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

        if (plannerRuleset.AllowPseudoPetAbsorptionInAssembly && !power.AbsorbedPetEffects)
        {
            power.AbsorbPetEffects(hIDX, stackingOverride, pseudoOnly: true);
        }

        PlannerEffectResolver.ExpandEffects(power, plannerRuleset.CreateAssemblyExpansionContext(hIDX, stackingOverride));

        if (plannerRuleset.AllowSubPowerEffectsInAssembly && !power.AppliedSubPowers)
        {
            GBPA_AddSubPowerEffects(ref power, hIDX);
            power.AppliedSubPowers = true;
        }

        return power;
    }

    private void GBPA_AddEnhFX(ref IPower? power, int index)
    {
        if (MidsContext.Config is null || MidsContext.Config.I9.IgnoreEnhFX || index < 0 || power is null)
        {
            return;
        }

        var currentPowerEntry = _currentBuild.Powers[index];
        if (currentPowerEntry?.Power == null)
        {
            return;
        }

        var newEffects = new List<IEffect>();
        foreach (var slotEntry in currentPowerEntry.Slots.Where(slot => slot.Enhancement.Enh >= 0))
        {
            var enhancement = DatabaseAPI.Database.Enhancements[slotEntry.Enhancement.Enh];
            var enhancementPower = enhancement.GetPower();
            if (enhancementPower == null)
            {
                continue;
            }

            if (currentPowerEntry.ProcInclude & enhancement.IsProc)
            {
                continue;
            }

            var enhancementSet = enhancement.GetEnhancementSet();
            if (enhancementSet is null)
            {
                continue;
            }

            foreach (var enhancementEffect in enhancementPower.Effects)
            {
                var shouldAddEffect = false;
                if (enhancementEffect.AffectsPetsOnly() && power.IsSummonPower)
                {
                    var uidEntity = power.Effects.FirstOrDefault(x => x.EffectType == Enums.eEffectType.EntCreate)?.Summon;
                    if (uidEntity != null)
                    {
                        var summon = DatabaseAPI.NidFromUidEntity(uidEntity);
                        var entitySetName = DatabaseAPI.Database.Entities[summon].PowersetFullName.FirstOrDefault();
                        var entitySet = DatabaseAPI.GetPowersetByFullname(entitySetName);
                        if (entitySet != null)
                        {
                            foreach (var entityPower in entitySet.Powers)
                            {
                                if (entityPower == null)
                                {
                                    continue;
                                }

                                shouldAddEffect = entityPower.Effects.Any(e => e.EffectType == enhancementEffect.EffectType);
                                if (!shouldAddEffect)
                                {
                                    continue;
                                }

                                AddClonedEffectToList(newEffects, enhancementEffect, enhancement.IsProc);
                                if (enhancementEffect.EffectType == Enums.eEffectType.GrantPower)
                                {
                                    entityPower.HasGrantPowerEffect = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    var enhancementIndex = DatabaseAPI.TryGetSetRawMemberPositionForEnhancement(slotEntry.Enhancement.Enh, out _, out var rawMemberPosition)
                        ? rawMemberPosition
                        : -1;
                    shouldAddEffect = enhancementIndex >= 0 &&
                                      enhancementSet.SpecialBonus[enhancementIndex].Index.Length <= 0 &&
                                      (enhancement.Effect.All(e => e.Mode != Enums.eEffMode.Enhancement) ||
                                       !Regex.IsMatch(enhancementEffect.ModifierTable, @"^(Melee|Ranged)_Boosts_"));
                }

                if (!shouldAddEffect)
                {
                    continue;
                }

                AddClonedEffectToList(newEffects, enhancementEffect, enhancement.IsProc);
                if (enhancementEffect.EffectType == Enums.eEffectType.GrantPower)
                {
                    power.HasGrantPowerEffect = true;
                }
            }
        }

        if (newEffects.Count > 0)
        {
            power.Effects = power.Effects.Concat(newEffects).ToArray();
        }
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
        if (power.NIDSubPower.Length <= 0 || hIDX < 0)
        {
            return false;
        }

        var length = power.Effects.Length;
        var effectCount = 0;
        for (var index = 0; index < _currentBuild.Powers[hIDX].SubPowers.Length; index++)
        {
            if ((_currentBuild.Powers[hIDX].SubPowers[index].nIDPower > -1) &
                _currentBuild.Powers[hIDX].SubPowers[index].StatInclude)
            {
                effectCount += DatabaseAPI.Database.Power[power.NIDSubPower[index]].Effects.Length;
            }
        }

        var effectArray = new IEffect[power.Effects.Length + effectCount];
        Array.Copy(power.Effects, effectArray, power.Effects.Length);
        power.Effects = effectArray;
        foreach (var subPower in _currentBuild.Powers[hIDX].SubPowers.Where(sp => sp is { nIDPower: > -1, StatInclude: true }))
        {
            for (var index = 0; index < DatabaseAPI.Database.Power[subPower.nIDPower].Effects.Length; index++)
            {
                power.Effects[length] = (IEffect)DatabaseAPI.Database.Power[subPower.nIDPower].Effects[index].Clone();
                power.Effects[length].Absorbed_EffectID = index;
                power.Effects[length].Absorbed_Effect = true;
                power.Effects[length].Absorbed_Power_nID = subPower.nIDPower;
                power.Effects[length].Absorbed_PowerType = DatabaseAPI.Database.Power[subPower.nIDPower].PowerType;
                length++;
            }
        }

        return true;
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

    private static void HandleDefaultIncarnateEnh(ref IPower powerMath, IEffect effect, IEffect[] buffedPowerEffects)
    {
        foreach (var targetEffect in powerMath.Effects)
        {
            if (!targetEffect.Buffable)
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
        powerMath.AbsorbEffects(DatabaseAPI.Database.Power[effect.nSummon], effect.Duration, 0, archetype, 1, true, effectIndex);
        foreach (var fx in powerMath.Effects)
        {
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
        for (var index = length; index < buffedPowers[hIDX]!.Effects.Length; index++)
        {
            buffedPowers[hIDX]!.Effects[index].ToWho = effect.ToWho;
            buffedPowers[hIDX]!.Effects[index].Absorbed_Effect = true;
            buffedPowers[hIDX]!.Effects[index].isEnhancementEffect = effect.isEnhancementEffect;
            buffedPowers[hIDX]!.Effects[index].BaseProbability *= effect.BaseProbability;
            buffedPowers[hIDX]!.Effects[index].Ticks = effect.Ticks;
        }
    }

    private void GBPA_ApplyIncarnateEnhancements(ref IPower powerMath, int hIDX, IPower? power, bool ignoreED, ref Enums.eEffectType effectType)
    {
        if (powerMath == null || power == null || power.Effects.Length == 0 || !powerMath.Slottable)
        {
            return;
        }

        for (var effectIndex = 0; effectIndex < power.Effects.Length; effectIndex++)
        {
            var effect = power.Effects[effectIndex];
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
                        else if (power.PowerType != Enums.ePowerType.GlobalBoost &&
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

            var sourcePower = effect.Absorbed_Effect & effect.Absorbed_Power_nID > -1
                ? DatabaseAPI.Database.Power[effect.Absorbed_Power_nID]
                : power;
            var isAllowed = powerMath.Enhancements.Intersect(sourcePower.Enhancements).Any();
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
                            HandleDefaultIncarnateEnh(ref powerMath, effect, _buffedPowers[hIDX]!.Effects);
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
                powerMath.AbsorbEffects(power, effect.Duration, 0, _archetype, 1, true, effectIndex, effectIndex);
                if (hIDX <= -1 || hIDX >= _buffedPowers.Length || _buffedPowers[hIDX] == null)
                {
                    continue;
                }

                var length = _buffedPowers[hIDX]!.Effects.Length;
                _buffedPowers[hIDX]!.AbsorbEffects(power, effect.Duration, 0, _archetype, 1, true, effectIndex, effectIndex);
                for (var index = length; index < _buffedPowers[hIDX]!.Effects.Length; index++)
                {
                    _buffedPowers[hIDX]!.Effects[index].ToWho = effect.ToWho;
                    _buffedPowers[hIDX]!.Effects[index].Absorbed_Effect = true;
                    _buffedPowers[hIDX]!.Effects[index].isEnhancementEffect = effect.isEnhancementEffect;
                    _buffedPowers[hIDX]!.Effects[index].BaseProbability *= effect.BaseProbability;
                    _buffedPowers[hIDX]!.Effects[index].Ticks = effect.Ticks;
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
                if (!powerMath.Effects[effectIndex].Buffable)
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
                        ? (Enums.eEnhance)Enums.StringToFlaggedEnum(
                            Enum.GetName(typeof(Enums.eEffectType), effectType),
                            Enums.eEnhance.None)
                        : Enums.eEnhance.Accuracy;

                    var magnitude = effectType == Enums.eEffectType.Mez
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
                    else if (effectType == Enums.eEffectType.Mez && powerMath.Effects[effectIndex].AttribType == Enums.eAttribType.Duration)
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
                if (_currentBuild.Powers[index] == null || !(_currentBuild.Powers[index].StatInclude & _currentBuild.Powers[index].NIDPower > -1))
                {
                    continue;
                }

                var effectType = Enums.eEffectType.Enhancement;
                GBPA_ApplyIncarnateEnhancements(ref powerMath, hIDX, _mathPowers[index], false, ref effectType);
            }
        }

        return false;
    }

    private static bool GBPA_Pass2_ApplyED(ref IPower powerMath)
    {
        powerMath.Accuracy = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.Accuracy), powerMath.Accuracy);
        powerMath.EndCost = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.EnduranceDiscount), powerMath.EndCost);
        powerMath.InterruptTime = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.Interrupt), powerMath.InterruptTime);
        powerMath.Range = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.Range), powerMath.Range);
        powerMath.RechargeTime = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.RechargeTime), powerMath.RechargeTime);
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
                    ? (Enums.eEnhance)Enums.StringToFlaggedEnum(Enum.GetName(effectType.GetType(), effectType), enhanceType)
                    : Enums.eEnhance.Accuracy;

                if (effectType == Enums.eEffectType.Mez)
                {
                    effect.Math_Mag = Enhancement.ApplyED(Enhancement.GetSchedule(scheduleEnhance, (int)effect.MezType), effect.Math_Mag);
                    effect.Math_Duration = Enhancement.ApplyED(Enhancement.GetSchedule(scheduleEnhance, (int)effect.MezType), effect.Math_Duration);
                }
                else
                {
                    effect.Math_Mag = !(effectType == Enums.eEffectType.ResEffect && effect.ETModifies == Enums.eEffectType.Defense)
                        ? Enhancement.ApplyED(Enhancement.GetSchedule(scheduleEnhance), effect.Math_Mag)
                        : Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.Defense), effect.Math_Mag);
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
                        if (!powerMath.Effects[index].Buffable || powerMath.Effects[index].EffectType != effectType)
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
                if (_currentBuild.Powers[index] == null || !(_currentBuild.Powers[index].StatInclude & _currentBuild.Powers[index].NIDPower > -1))
                {
                    continue;
                }

                var effectType = Enums.eEffectType.Enhancement;
                GBPA_ApplyIncarnateEnhancements(ref powerMath, hIDX, _mathPowers[index], true, ref effectType);
            }
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
        var toHit = powerMath.IgnoreBuff(Enums.eEnhance.ToHit) ? 0 : _selfBuffs.Effect[(int)Enums.eStatType.ToHit];
        var accuracy = powerMath.IgnoreBuff(Enums.eEnhance.Accuracy) ? 0 : _selfBuffs.Effect[(int)Enums.eStatType.BuffAcc];
        var combatToHitScale = plannerRuleset.GetCombatModToHitScale(_combatContext);
        var combatAccuracyScale = plannerRuleset.GetCombatModAccuracyScale(_combatContext);
        var combatMagnitudeScale = plannerRuleset.GetCombatModMagnitudeScale(_combatContext);
        var combatDurationScale = plannerRuleset.GetCombatModDurationScale(_combatContext);

        powerBuffed.Accuracy = powerBuffed.Accuracy * (1 + powerMath.Accuracy + accuracy) * combatAccuracyScale *
                               (combatToHitScale + toHit);
        powerBuffed.AccuracyMult = powerBuffed.Accuracy * (1 + powerMath.Accuracy + accuracy) * combatAccuracyScale;

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

    private void GenerateBuffData(ref Enums.BuffsX buckets, PlannerBucketPass pass)
    {
        var enhancementPass = pass == PlannerBucketPass.Enhancement;
        var plannerRuleset = DatabaseAPI.GetPlannerRuleset();
        for (var index = 0; index < _currentBuild.Powers.Count; index++)
        {
            var powerEntry = _currentBuild.Powers[index];
            if (powerEntry == null)
            {
                continue;
            }

            if (!(powerEntry.StatInclude & powerEntry.NIDPower > -1) ||
                DatabaseAPI.Database.Power[powerEntry.NIDPower].PowerType == Enums.ePowerType.GlobalBoost)
            {
                continue;
            }

            if (enhancementPass)
            {
                if (_mathPowers[index] == null)
                {
                    continue;
                }

                DatabaseAPI.GetPlannerRuleset().AccumulateBuckets(_mathPowers[index]!, ref buckets, pass);
            }
            else
            {
                if (_buffedPowers[index] == null)
                {
                    continue;
                }

                DatabaseAPI.GetPlannerRuleset().AccumulateBuckets(_buffedPowers[index]!, ref buckets, pass);
            }
        }

        var setBonusPower = _recipient == null
            ? _currentBuild.SetBonusVirtualPower
            : _currentBuild.GetSetBonusVirtualPower(_recipient);
        if (setBonusPower != null)
        {
            DatabaseAPI.GetPlannerRuleset().AccumulateBuckets(setBonusPower, ref buckets, pass);
        }

        if (!plannerRuleset.IncludePvpResistanceBonusInBuckets(pass))
        {
            return;
        }

        var pvpResistIndex = DatabaseAPI.NidFromUidPower("Temporary_Powers.Temporary_Powers.PVP_Resist_Bonus");
        if (pvpResistIndex <= -1)
        {
            return;
        }

        IPower pvpResistPower = new Power(DatabaseAPI.Database.Power[pvpResistIndex]);
        if (_recipient != null)
        {
            pvpResistPower = OmniPowerRouting.CreatePlannerPower(pvpResistPower, _recipient) ?? new Power();
        }

        DatabaseAPI.GetPlannerRuleset().AccumulateBuckets(pvpResistPower, ref buckets, pass);
    }

    private static void RemoveExactDuplicateAbsorbedEffects(ref IPower? power)
    {
        if (power == null || power.Effects.Length <= 1)
        {
            return;
        }

        var filteredEffects = new List<IEffect>(power.Effects.Length);
        var absorbedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var effect in power.Effects)
        {
            if (!effect.Absorbed_Effect)
            {
                filteredEffects.Add(effect);
                continue;
            }

            var key = GetExactAbsorbedEffectKey(effect);
            if (!absorbedKeys.Add(key))
            {
                continue;
            }

            filteredEffects.Add(effect);
        }

        if (filteredEffects.Count == power.Effects.Length)
        {
            return;
        }

        power.Effects = filteredEffects.ToArray();
        foreach (var effect in power.Effects)
        {
            effect.SetPower(power);
        }
    }

    private static string GetExactAbsorbedEffectKey(IEffect effect)
    {
        static string NormalizeConditionSet(AdvancedConditionSet? conditions)
        {
            if (conditions == null || conditions.Rows.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(";",
                conditions.Rows
                    .Select(row => $"{row.EvaluationMode}|{row.Kind}|{row.Link}|{row.Negated}|{AdvancedConditionCompiler.Compile(row)}")
                    .OrderBy(value => value, StringComparer.OrdinalIgnoreCase));
        }

        var tags = string.Join(",",
            effect.EffectTags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase));

        var recurrence = effect.PseudoPetRecurrence is { IsValid: true } info
            ? $"{info.EntityName}|{info.PetPowerName}|{info.SourceUsageTime:0.####}|{info.SourceActivatePeriod:0.####}|{info.EntCreateDuration:0.####}|{info.PetTickInterval:0.####}|{info.SpawnCount}|{info.TicksPerSpawn}|{info.TotalExpectedTicks}"
            : string.Empty;

        return string.Join("|",
            effect.EffectType,
            effect.ToWho,
            effect.PvMode,
            effect.AttribType,
            effect.Aspect,
            effect.DamageType,
            effect.MezType,
            effect.ModifierTable ?? string.Empty,
            effect.Scale.ToString("0.####", CultureInfo.InvariantCulture),
            effect.nMagnitude.ToString("0.####", CultureInfo.InvariantCulture),
            effect.Math_Mag.ToString("0.####", CultureInfo.InvariantCulture),
            effect.nDuration.ToString("0.####", CultureInfo.InvariantCulture),
            effect.Math_Duration.ToString("0.####", CultureInfo.InvariantCulture),
            effect.BaseProbability.ToString("0.####", CultureInfo.InvariantCulture),
            effect.ProcsPerMinute.ToString("0.####", CultureInfo.InvariantCulture),
            effect.DelayedTime.ToString("0.####", CultureInfo.InvariantCulture),
            effect.Ticks.ToString("0.####", CultureInfo.InvariantCulture),
            effect.Absorbed_Interval.ToString("0.####", CultureInfo.InvariantCulture),
            effect.EffectClass,
            effect.Stacking,
            effect.Absorbed_EffectID,
            effect.Absorbed_Power_nID,
            effect.Absorbed_Class_nID,
            effect.OmniSource ?? string.Empty,
            effect.EffectId ?? string.Empty,
            tags,
            recurrence,
            NormalizeConditionSet(effect.AdvancedConditions),
            effect.Summon ?? string.Empty,
            effect.Override ?? string.Empty,
            effect.Reward ?? string.Empty,
            effect.ModeName ?? string.Empty);
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
}

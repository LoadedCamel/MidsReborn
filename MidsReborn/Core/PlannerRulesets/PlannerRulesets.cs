using System;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core.PlannerRulesets;

internal abstract class PlannerRulesetBase : IPlannerRuleset
{
    private static readonly Enums.eDamage[] SupportedDefenseVectors =
    [
        Enums.eDamage.Smashing,
        Enums.eDamage.Lethal,
        Enums.eDamage.Fire,
        Enums.eDamage.Cold,
        Enums.eDamage.Energy,
        Enums.eDamage.Negative,
        Enums.eDamage.Psionic,
        Enums.eDamage.Melee,
        Enums.eDamage.Ranged,
        Enums.eDamage.AoE
    ];

    private static readonly Enums.eDamage[] SupportedDefenseVectorsWithToxic =
    [
        Enums.eDamage.Smashing,
        Enums.eDamage.Lethal,
        Enums.eDamage.Fire,
        Enums.eDamage.Cold,
        Enums.eDamage.Energy,
        Enums.eDamage.Negative,
        Enums.eDamage.Psionic,
        Enums.eDamage.Toxic,
        Enums.eDamage.Melee,
        Enums.eDamage.Ranged,
        Enums.eDamage.AoE
    ];

    public abstract PlannerRulesetId Id { get; }
    public abstract bool UsesCanonicalPlannerMath { get; }
    public virtual bool UsesToxicDefense => false;
    public abstract bool AllowLegacyCrossPowerIncarnatePasses { get; }
    public virtual bool AllowRedirectSelectionInAssembly => true;
    public virtual bool AllowPseudoPetAbsorptionInAssembly => true;
    public virtual bool AllowGrantPowerExpansionInAssembly => true;
    public virtual bool AllowExecutePowerExpansionInAssembly => true;
    public virtual bool AllowSubPowerEffectsInAssembly => true;

    public virtual PlannerEffectResolutionContext CreateDefaultResolutionContext()
    {
        return new PlannerEffectResolutionContext(useRulesetDefaults: false)
        {
            ApplyRedirects = AllowRedirectSelectionInAssembly,
            AbsorbPetEffects = AllowPseudoPetAbsorptionInAssembly,
            ExpandGrantPowers = AllowGrantPowerExpansionInAssembly,
            ExpandExecutePowers = AllowExecutePowerExpansionInAssembly
        };
    }

    public virtual PlannerEffectResolutionContext CreateAssemblyExpansionContext(int historyIndex, int stackingOverride)
    {
        return new PlannerEffectResolutionContext(useRulesetDefaults: false)
        {
            HistoryIndex = historyIndex,
            StackingOverride = stackingOverride,
            ApplyRedirects = false,
            AbsorbPetEffects = false,
            ExpandGrantPowers = AllowGrantPowerExpansionInAssembly,
            ExpandExecutePowers = AllowExecutePowerExpansionInAssembly
        };
    }

    public virtual void AccumulateBuckets(IPower power, ref Enums.BuffsX buckets, PlannerBucketPass pass)
    {
        if (power.PowerType == Enums.ePowerType.GlobalBoost)
        {
            return;
        }

        var enhancementPass = pass == PlannerBucketPass.Enhancement;

        static bool TargetsSelfOrAll(IEffect fx) =>
            fx.ToWho == Enums.eToWho.Self || fx.ToWho == Enums.eToWho.All;

        static bool IsMaxSpeedCap(IEffect fx, Enums.eEffectType mod) =>
            fx.EffectType != Enums.eEffectType.ResEffect && fx.ETModifies == mod && fx.Aspect == Enums.eAspect.Max;

        static bool IsSpeedScalar(Enums.eEffectType mod) =>
            mod == Enums.eEffectType.SpeedRunning || mod == Enums.eEffectType.SpeedFlying ||
            mod == Enums.eEffectType.SpeedJumping || mod == Enums.eEffectType.JumpHeight;

        static bool IsDamageBuffOrEnhancement(IEffect fx) =>
            fx.EffectType == Enums.eEffectType.DamageBuff || fx.EffectType == Enums.eEffectType.Enhancement;

        static bool IsSelfOrAllProcContribution(IEffect fx) =>
            fx.isEnhancementEffect &&
            fx.IsFromProc &&
            (fx.ToWho == Enums.eToWho.Self || fx.ToWho == Enums.eToWho.All);

        static float GetEnhancementPassDamageBuffMagnitude(IEffect fx)
        {
            var magnitudeFactor = Math.Abs(fx.Math_Mag) > float.Epsilon
                ? fx.Math_Mag
                : 1f;
            var magnitude = fx.Mag * magnitudeFactor;
            var tickCopies = PlannerStackRules.GetPlannerVisibleCopyCount(fx, fx.Ticks);
            if (tickCopies > 1)
            {
                magnitude *= tickCopies;
            }

            return magnitude;
        }

        static bool IsGlobalAccuracySource(IPower src) =>
            ReferenceEquals(src, MidsContext.Character.CurrentBuild.SetBonusVirtualPower) ||
            string.Equals(src.FullName, "Mids.SetBonus.Virtual", StringComparison.OrdinalIgnoreCase) ||
            src.PowerType == Enums.ePowerType.GlobalBoost;

        static bool IsSemanticGlobalAccuracySource(IPower src, IEffect fx)
        {
            var semanticSource = GlobalBoostPlannerSemantics.ResolveSemanticSourcePower(src, fx);
            return ReferenceEquals(semanticSource, MidsContext.Character.CurrentBuild.SetBonusVirtualPower) ||
                   string.Equals(semanticSource.FullName, "Mids.SetBonus.Virtual", StringComparison.OrdinalIgnoreCase) ||
                   semanticSource.PowerType == Enums.ePowerType.GlobalBoost;
        }

        var supportedDefenseVectors = UsesToxicDefense
            ? SupportedDefenseVectorsWithToxic
            : SupportedDefenseVectors;

        void ApplyAcrossDefenseVectors(float[] bucket, float value)
        {
            foreach (var damageType in supportedDefenseVectors)
            {
                bucket[(int)damageType] += value;
            }
        }

        var shortFx = new Enums.ShortFX();
        var shortFxSelf = new Enums.ShortFX();

        for (var effectIndex = 0; effectIndex < buckets.Effect.Length; effectIndex++)
        {
            var effectType = (Enums.eEffectType)effectIndex;
            if (effectType == Enums.eEffectType.Damage)
            {
                continue;
            }

            if (enhancementPass && effectType != Enums.eEffectType.DamageBuff)
            {
                shortFx.Assign(power.GetEnhancementMagSum(effectType, -1));
            }
            else
            {
                switch (effectType)
                {
                    case Enums.eEffectType.MaxRunSpeed:
                        shortFx.Assign(power.GetEffectMagSum(Enums.eEffectType.SpeedRunning, false, false, false, true));
                        shortFxSelf.Assign(power.GetEffectMagSum(Enums.eEffectType.MaxRunSpeed, false, true));
                        buckets.Effect[(int)Enums.eStatType.MaxRunSpeed] += shortFxSelf.Sum;
                        break;

                    case Enums.eEffectType.MaxJumpSpeed:
                        shortFx.Assign(power.GetEffectMagSum(Enums.eEffectType.SpeedJumping, false, false, false, true));
                        shortFxSelf.Assign(power.GetEffectMagSum(Enums.eEffectType.MaxJumpSpeed, false, true));
                        buckets.Effect[(int)Enums.eStatType.MaxJumpSpeed] += shortFxSelf.Sum;
                        break;

                    case Enums.eEffectType.MaxFlySpeed:
                        shortFx.Assign(power.GetEffectMagSum(Enums.eEffectType.SpeedFlying, false, false, false, true));
                        shortFxSelf.Assign(power.GetEffectMagSum(Enums.eEffectType.MaxFlySpeed, false, true));
                        buckets.Effect[(int)Enums.eStatType.MaxFlySpeed] += shortFxSelf.Sum;
                        break;

                    default:
                        shortFx.Assign(power.GetEffectMagSum(effectType));
                        break;
                }
            }

            for (var shortFxIndex = 0; shortFxIndex < shortFx.Value.Length; shortFxIndex++)
            {
                var effect = power.Effects[shortFx.Index[shortFxIndex]];
                if (!TargetsSelfOrAll(effect))
                {
                    continue;
                }

                if (IsSelfOrAllProcContribution(effect))
                {
                    continue;
                }

                if (GlobalBoostPlannerSemantics.IsPowerBoostTaggedEnhancementCarrierEffect(effect))
                {
                    continue;
                }

                var value = shortFx.Value[shortFxIndex];
                if (enhancementPass && effectType == Enums.eEffectType.DamageBuff)
                {
                    value = GetEnhancementPassDamageBuffMagnitude(effect);
                }

                if (!enhancementPass && effect.EffectType == Enums.eEffectType.Enhancement &&
                    effect.ETModifies == Enums.eEffectType.Range)
                {
                    buckets.Effect[(int)Enums.eEffectType.Range] += value;
                }

                if (effect.Absorbed_PowerType == Enums.ePowerType.GlobalBoost)
                {
                    continue;
                }

                if (!enhancementPass)
                {
                    switch (effect.EffectType)
                    {
                        case Enums.eEffectType.MezProtect:
                            buckets.StatusProtection[(int)effect.MezType] += value;
                            break;
                        case Enums.eEffectType.MezResist:
                            buckets.StatusResistance[(int)effect.MezType] += value;
                            break;
                        case Enums.eEffectType.ResEffect:
                            buckets.DebuffResistance[(int)effect.ETModifies] += value;
                            break;
                    }
                }

                if (enhancementPass && IsDamageBuffOrEnhancement(effect))
                {
                    switch (effect.ETModifies)
                    {
                        case Enums.eEffectType.Mez:
                            buckets.Mez[(int)effect.MezType] += value;
                            continue;

                        case Enums.eEffectType.Defense:
                            if (effect.DamageType != Enums.eDamage.None)
                            {
                                buckets.Defense[(int)effect.DamageType] += value;
                            }
                            else
                            {
                                ApplyAcrossDefenseVectors(buckets.Defense, value);
                            }
                            continue;

                        case Enums.eEffectType.Resistance:
                            if (effect.DamageType != Enums.eDamage.None)
                            {
                                buckets.Resistance[(int)effect.DamageType] += value;
                            }
                            else
                            {
                                buckets.Effect[effectIndex] += value;
                            }
                            continue;

                        case Enums.eEffectType.Elusivity:
                            if (effect.DamageType != Enums.eDamage.None)
                            {
                                buckets.Elusivity[(int)effect.DamageType] += value;
                            }
                            else
                            {
                                ApplyAcrossDefenseVectors(buckets.Elusivity, value);
                            }
                            continue;

                        default:
                            if (effectType == Enums.eEffectType.DamageBuff)
                            {
                                var semanticSource = GlobalBoostPlannerSemantics.ResolveSemanticSourcePower(power, effect);
                                var damageBuffFlavor = GlobalBoostPlannerSemantics.ClassifyDamageBuffFlavor(semanticSource, effect);
                                if (damageBuffFlavor == GlobalBoostDamageFlavor.Resistance)
                                {
                                    if (effect.DamageType != Enums.eDamage.None)
                                    {
                                        buckets.Resistance[(int)effect.DamageType] += value;
                                    }
                                    else
                                    {
                                        buckets.Effect[(int)Enums.eEffectType.Resistance] += value;
                                    }

                                    continue;
                                }

                                if (DefiancePlanner.IsComputedCurrentBuffEffect(effect))
                                {
                                    foreach (var damageType in DefiancePlanner.ComputedBuffDamageTypes)
                                    {
                                        buckets.Damage[(int)damageType] += value;
                                    }

                                    continue;
                                }

                                var isDefiance =
                                    DefiancePlanner.IsModernContributorEffect(effect) ||
                                    (effect.isEnhancementEffect && effect.EffectClass == Enums.eEffectClass.Tertiary) ||
                                    effect.ValidateConditional("Active", "Defiance");

                                if (!isDefiance)
                                {
                                    buckets.Damage[(int)effect.DamageType] += value;
                                }

                                continue;
                            }

                            if (effect.ETModifies == Enums.eEffectType.Accuracy)
                            {
                                buckets.Effect[(int)Enums.eEffectType.Accuracy] += value;
                                continue;
                            }

                            if (IsSpeedScalar(effect.ETModifies))
                            {
                                if (effect.buffMode != Enums.eBuffMode.Debuff)
                                {
                                    buckets.Effect[(int)effect.ETModifies] += value;
                                }
                                else
                                {
                                    buckets.EffectAux[(int)effect.ETModifies] += value;
                                }

                                continue;
                            }

                            buckets.Effect[effectIndex] += value;
                            continue;
                    }
                }

                if (effect.EffectType == Enums.eEffectType.Endurance && effect.Aspect == Enums.eAspect.Max)
                {
                    buckets.MaxEnd += value;
                    continue;
                }

                if (!enhancementPass && effect.EffectType != Enums.eEffectType.ResEffect &&
                    effect.ETModifies == Enums.eEffectType.Mez)
                {
                    buckets.Mez[(int)effect.MezType] += value;
                    continue;
                }

                if (!enhancementPass && effect.EffectType != Enums.eEffectType.ResEffect &&
                    effect.ETModifies == Enums.eEffectType.MezResist)
                {
                    buckets.MezRes[(int)effect.MezType] += value;
                    continue;
                }

                if (!enhancementPass && effectType == Enums.eEffectType.Defense && effect.DamageType != Enums.eDamage.None)
                {
                    buckets.Defense[(int)effect.DamageType] += value;
                    continue;
                }

                if (!enhancementPass && effectType == Enums.eEffectType.Accuracy)
                {
                    buckets.Effect[(int)Enums.eStatType.BuffAcc] += value;
                    continue;
                }

                if (!enhancementPass && effectType == Enums.eEffectType.Mez && value > 0f)
                {
                    buckets.Mez[(int)effect.MezType] += value;
                    continue;
                }

                if (!enhancementPass && effectType == Enums.eEffectType.Defense)
                {
                    ApplyAcrossDefenseVectors(buckets.Defense, value);
                    continue;
                }

                if (!enhancementPass && effectType == Enums.eEffectType.Resistance && effect.DamageType != Enums.eDamage.None)
                {
                    buckets.Resistance[(int)effect.DamageType] += value;
                    continue;
                }

                if (!enhancementPass && effectType == Enums.eEffectType.Elusivity && effect.DamageType != Enums.eDamage.None)
                {
                    buckets.Elusivity[(int)effect.DamageType] += value;
                    continue;
                }

                if (!enhancementPass && effectType == Enums.eEffectType.Elusivity)
                {
                    ApplyAcrossDefenseVectors(buckets.Elusivity, value);
                    continue;
                }

                if (!enhancementPass && effect.EffectType != Enums.eEffectType.ResEffect &&
                    effect.ETModifies == Enums.eEffectType.Accuracy)
                {
                    if (IsSemanticGlobalAccuracySource(power, effect) || IsGlobalAccuracySource(power))
                    {
                        buckets.Effect[(int)Enums.eStatType.BuffAcc] += value;
                    }
                    else
                    {
                        buckets.Effect[(int)Enums.eStatType.ToHit] += value;
                    }

                    continue;
                }

                if (!enhancementPass && IsMaxSpeedCap(effect, Enums.eEffectType.SpeedRunning))
                {
                    buckets.Effect[(int)Enums.eStatType.MaxRunSpeed] += value;
                    continue;
                }

                if (!enhancementPass && IsMaxSpeedCap(effect, Enums.eEffectType.SpeedFlying))
                {
                    buckets.Effect[(int)Enums.eStatType.MaxFlySpeed] += value;
                    continue;
                }

                if (!enhancementPass && IsMaxSpeedCap(effect, Enums.eEffectType.SpeedJumping))
                {
                    buckets.Effect[(int)Enums.eStatType.MaxJumpSpeed] += value;
                    continue;
                }

                if (!enhancementPass && effectType == Enums.eEffectType.ToHit)
                {
                    if (!(effect.isEnhancementEffect && effect.EffectClass == Enums.eEffectClass.Tertiary))
                    {
                        buckets.Effect[effectIndex] += value;
                    }

                    continue;
                }

                if (!enhancementPass)
                {
                    if (effectIndex == (int)Enums.eStatType.Absorb && effect.DisplayPercentage)
                    {
                        value *= MidsContext.Character.Totals.HPMax;
                    }

                    buckets.Effect[effectIndex] += value;

                    if (IsClickPower(effect.GetPower()) && !effect.BuildEffectString().Contains("From Enh"))
                    {
                        buckets.Effect[effectIndex] -= effect.Mag;
                    }
                }
            }
        }
    }

    public virtual bool IncludePvpResistanceBonusInBuckets(PlannerBucketPass pass)
    {
        return MidsContext.Config.Inc.DisablePvE;
    }

    public virtual bool ShouldProcessExecutesInDamageHelpers(IPower power)
    {
        return !power.AppliedExecutes;
    }

    public virtual bool ShouldAbsorbPseudoPetEffectsForDamage(IPower power, bool absorbRequested)
    {
        return absorbRequested && !power.HasAbsorbedEffects;
    }

    public virtual bool EffectMatchesCurrentMode(IEffect effect)
    {
        return MidsContext.Archetype == null ||
               (effect.PvMode != Enums.ePvX.PvP && !MidsContext.Config.Inc.DisablePvE ||
                effect.PvMode != Enums.ePvX.PvE && MidsContext.Config.Inc.DisablePvE) &&
               (effect.nIDClassName == -1 || effect.nIDClassName == MidsContext.Archetype.Idx);
    }

    public virtual bool SupportsPowerLocalChanceMods => false;
    public virtual bool SupportsRelativeLevelCombatModMath => Id != PlannerRulesetId.Legacy;

    public virtual PlannerCombatContext ResolveCombatContext(ConfigData? config)
    {
        var selectedRelativeLevel = ConfigData.NormalizeEnemyRelativeLevel(
            config?.EnemyRelativeLevel ?? int.MinValue,
            config?.ScalingToHit ?? DatabaseAPI.ServerData.BaseToHit);
        var effectiveRelativeLevel = selectedRelativeLevel;
        var clampedEffectiveRelativeLevel = ConfigData.ClampEnemyRelativeLevel(effectiveRelativeLevel);
        var teamSize = Math.Max(1, config?.TeamSize ?? 1);
        var legacyToHitScale = ConfigData.GetLegacyScalingToHitForRelativeLevel(clampedEffectiveRelativeLevel);
        var diagnostic = string.Empty;
        var toHitScale = legacyToHitScale;
        var accuracyScale = 1f;
        var magnitudeScale = 1f;
        var durationScale = 1f;
        var usesLegacyToHitFallback = true;
        var usesFullCombatModTables = false;

        if (SupportsRelativeLevelCombatModMath)
        {
            if (DatabaseAPI.ServerData.TryGetPlayerCombatModSnapshot(teamSize, effectiveRelativeLevel, out var snapshot))
            {
                if (snapshot.HasToHit)
                {
                    toHitScale = snapshot.ToHit;
                    usesLegacyToHitFallback = false;
                }

                if (snapshot.HasAccuracy)
                {
                    accuracyScale = snapshot.Accuracy;
                }

                if (snapshot.HasMagnitude)
                {
                    magnitudeScale = snapshot.Magnitude;
                }

                if (snapshot.HasDuration)
                {
                    durationScale = snapshot.Duration;
                }

                usesFullCombatModTables = snapshot.HasFullPlannerData;
                if (!snapshot.HasFullPlannerData)
                {
                    diagnostic =
                        "Combat-mod tables are only partially available for this ruleset. Relative-level ToHit is applied, but full relative-level accuracy/magnitude/duration math is not.";
                }
            }
            else
            {
                diagnostic =
                    "Combat-mod tables are not available for this ruleset. Relative-level ToHit is using legacy planner values, and full relative-level accuracy/magnitude/duration math is disabled.";
            }
        }
        else
        {
            diagnostic = "Legacy planner math does not apply relative-level combat-mod tables.";
        }

        return new PlannerCombatContext
        {
            SelectedEnemyRelativeLevel = selectedRelativeLevel,
            EffectiveEnemyRelativeLevel = effectiveRelativeLevel,
            TeamSize = teamSize,
            ToHitScale = toHitScale,
            AccuracyScale = accuracyScale,
            MagnitudeScale = magnitudeScale,
            DurationScale = durationScale,
            UsesLegacyToHitFallback = usesLegacyToHitFallback,
            UsesFullCombatModTables = usesFullCombatModTables,
            Diagnostic = diagnostic
        };
    }

    public virtual int ResolveEffectiveCombatDelta(ConfigData? config)
    {
        return ResolveCombatContext(config).EffectiveEnemyRelativeLevel;
    }

    public virtual float GetCombatModToHitScale(PlannerCombatContext context)
    {
        return context?.ToHitScale ?? DatabaseAPI.ServerData.BaseToHit;
    }

    public virtual float GetCombatModAccuracyScale(PlannerCombatContext context)
    {
        return context?.AccuracyScale ?? 1f;
    }

    public virtual float GetCombatModMagnitudeScale(PlannerCombatContext context)
    {
        return context?.MagnitudeScale ?? 1f;
    }

    public virtual float GetCombatModDurationScale(PlannerCombatContext context)
    {
        return context?.DurationScale ?? 1f;
    }

    public virtual float GetMinProcChance(float procsPerMinute)
    {
        return procsPerMinute > 0 ? procsPerMinute * 0.015f + 0.05f : 0.05f;
    }

    public virtual float GetMaxProcChance(float procsPerMinute)
    {
        return 0.9f;
    }

    public virtual float CalculateProcProbability(IPower power, IEffect procEffect, float procsPerMinute, float baseProbability)
    {
        if (procsPerMinute <= 0 || power == null)
        {
            return baseProbability;
        }

        var areaFactor = PlannerProcSupport.ResolveProcAreaFactor(power, procEffect);
        var probability = procsPerMinute;

        if (power.PowerType == Enums.ePowerType.Click)
        {
            probability *= ResolveProcRechargeDuration(power) + Math.Max(0f, power.CastTimeReal);
        }
        else
        {
            probability *= PlannerProcSupport.ResolveProcSourceActivatePeriod(power, procEffect);
        }

        probability /= 60f * areaFactor;

        return Math.Max(GetMinProcChance(procsPerMinute), Math.Min(GetMaxProcChance(procsPerMinute), probability));
    }

    private static float ResolveProcRechargeDuration(IPower power)
    {
        if (power.BaseRechargeTime <= float.Epsilon)
        {
            return 0f;
        }

        if (power.IgnoreStrength)
        {
            return Math.Max(0f, power.RechargeTime);
        }

        if (power.RechargeTime <= float.Epsilon)
        {
            return power.BaseRechargeTime;
        }

        var totalRechargeScale = power.BaseRechargeTime / power.RechargeTime;
        var globalRecharge = MidsContext.Character?.Totals?.BuffHaste ?? 0f;
        return totalRechargeScale > 1f + globalRecharge
            ? power.BaseRechargeTime / (totalRechargeScale - globalRecharge)
            : power.BaseRechargeTime;
    }

    public virtual float ApplyChanceModifiers(Character? character, IPower? ownerPower, IEffect procEffect, float probability)
    {
        return ChanceModifierSupport.ApplyChanceModifiers(
            character,
            ownerPower,
            procEffect,
            probability,
            SupportsPowerLocalChanceMods);
    }

    public virtual void ApplyPvpDiminishingReturns(Character.TotalStatistics totals)
    {
        if (totals == null || !MidsContext.Config.Inc.DisablePvE)
        {
            return;
        }

        for (var index = 0; index < totals.Def.Length; index++)
        {
            totals.Def[index] = CalculatePvpDefenseDiminishingReturns(totals.Def[index], 1.2f, 1f);
        }
    }

    public virtual void ApplyFinalCaps(Archetype? archetype, Character.TotalStatistics totals,
        Character.TotalStatistics totalsCapped, int? zeroBasedLevel = null)
    {
        if (totals == null || totalsCapped == null)
        {
            return;
        }

        var hitPointCap = DatabaseAPI.GetClassHitPointCap(archetype, zeroBasedLevel);
        var damageCap = DatabaseAPI.GetClassDamageCap(archetype, zeroBasedLevel);
        var rechargeCap = DatabaseAPI.GetClassRechargeCap(archetype, zeroBasedLevel);
        var regenCap = DatabaseAPI.GetClassRegenCap(archetype, zeroBasedLevel);
        var recoveryCap = DatabaseAPI.GetClassRecoveryCap(archetype, zeroBasedLevel);
        var resistanceCap = DatabaseAPI.GetClassResistanceCap(archetype, zeroBasedLevel);
        var perceptionCap = DatabaseAPI.GetClassPerceptionCap(archetype, zeroBasedLevel);

        totalsCapped.BuffDam = Math.Min(totalsCapped.BuffDam, damageCap - 1);
        totalsCapped.BuffHaste = Math.Min(totalsCapped.BuffHaste, rechargeCap - 1);
        totalsCapped.HPRegen = Math.Min(totalsCapped.HPRegen, regenCap - 1);
        totalsCapped.EndRec = Math.Min(totalsCapped.EndRec, recoveryCap - 1);
        for (var index = 0; index < totalsCapped.Res.Length; index++)
        {
            totalsCapped.Res[index] = Math.Min(totalsCapped.Res[index], resistanceCap);
        }

        if (hitPointCap > 0)
        {
            totalsCapped.HPMax = Math.Min(totalsCapped.HPMax, hitPointCap);
            totalsCapped.Absorb = Math.Min(totalsCapped.Absorb, totalsCapped.HPMax);
        }

        totalsCapped.RunSpd = Math.Min(totalsCapped.RunSpd, totals.MaxRunSpd);
        totalsCapped.JumpSpd = Math.Min(totalsCapped.JumpSpd, totals.MaxJumpSpd);
        totalsCapped.FlySpd = Math.Min(totalsCapped.FlySpd, totals.MaxFlySpd);
        totalsCapped.JumpHeight = Math.Min(totalsCapped.JumpHeight, DatabaseAPI.ServerData.MaxJumpHeight);
        totalsCapped.Perception = Math.Min(totalsCapped.Perception, perceptionCap);
    }

    public virtual float GetDisplayedBuffHastePercent(Archetype? archetype, Character.TotalStatistics totals,
        Character.TotalStatistics totalsCapped, bool uncapped, int? zeroBasedLevel = null)
    {
        if (uncapped)
        {
            return (totals.BuffHaste + 1) * 100f;
        }

        var rechargeCap = DatabaseAPI.GetClassRechargeCap(archetype, zeroBasedLevel) * 100f;
        return Math.Min(rechargeCap, (totalsCapped.BuffHaste + 1) * 100f);
    }

    public virtual float GetDisplayedBuffDamagePercent(Archetype? archetype, Character.TotalStatistics totals,
        Character.TotalStatistics totalsCapped, bool uncapped, int? zeroBasedLevel = null)
    {
        if (uncapped)
        {
            return (totals.BuffDam + 1) * 100f;
        }

        var damageCap = DatabaseAPI.GetClassDamageCap(archetype, zeroBasedLevel) * 100f;
        return Math.Min(damageCap, (totalsCapped.BuffDam + 1) * 100f);
    }

    private static float CalculatePvpDefenseDiminishingReturns(float value, float a, float b)
    {
        return value * (float)(1.0 - Math.Abs(Math.Atan(a * value)) * (2.0 / Math.PI) * b);
    }

    private static bool IsClickPower(IPower power)
    {
        return power is { PowerType: Enums.ePowerType.Click, ClickBuff: false };
    }
}

internal sealed class LegacyPlannerRuleset : PlannerRulesetBase
{
    public override PlannerRulesetId Id => PlannerRulesetId.Legacy;
    public override bool UsesCanonicalPlannerMath => false;
    public override bool AllowLegacyCrossPowerIncarnatePasses => true;
}

internal sealed class HomecomingPlannerRuleset : PlannerRulesetBase
{
    public override PlannerRulesetId Id => PlannerRulesetId.Homecoming;
    public override bool UsesCanonicalPlannerMath => true;
    public override bool UsesToxicDefense => true;
    public override bool AllowLegacyCrossPowerIncarnatePasses => false;
    public override bool SupportsPowerLocalChanceMods => true;

    public override bool ShouldProcessExecutesInDamageHelpers(IPower power)
    {
        return false;
    }
}

internal sealed class OurodevPlannerRuleset : PlannerRulesetBase
{
    public override PlannerRulesetId Id => PlannerRulesetId.Ourodev;
    public override bool UsesCanonicalPlannerMath => true;
    public override bool AllowLegacyCrossPowerIncarnatePasses => false;
    public override bool SupportsPowerLocalChanceMods => true;
}

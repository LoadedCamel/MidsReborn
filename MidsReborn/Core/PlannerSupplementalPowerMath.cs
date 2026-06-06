using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.PlannerRulesets;

namespace Mids_Reborn.Core;

internal static class PlannerSupplementalPowerMath
{
    public static bool HasSupplementalEnhancementBuckets(Enums.BuffsX buckets)
    {
        return buckets.Damage.Any(value => Math.Abs(value) > float.Epsilon) ||
               buckets.Defense.Any(value => Math.Abs(value) > float.Epsilon) ||
               buckets.Resistance.Any(value => Math.Abs(value) > float.Epsilon) ||
               buckets.Mez.Any(value => Math.Abs(value) > float.Epsilon) ||
               buckets.Effect.Any(value => Math.Abs(value) > float.Epsilon) ||
               buckets.EffectAux.Any(value => Math.Abs(value) > float.Epsilon);
    }

    public static void MergeBuffBuckets(ref Enums.BuffsX target, Enums.BuffsX source)
    {
        target.MaxEnd += source.MaxEnd;

        for (var index = 0; index < target.Effect.Length && index < source.Effect.Length; index++)
        {
            target.Effect[index] += source.Effect[index];
        }

        for (var index = 0; index < target.EffectAux.Length && index < source.EffectAux.Length; index++)
        {
            target.EffectAux[index] += source.EffectAux[index];
        }

        for (var index = 0; index < target.Mez.Length && index < source.Mez.Length; index++)
        {
            target.Mez[index] += source.Mez[index];
            target.MezRes[index] += source.MezRes[index];
            target.StatusProtection[index] += source.StatusProtection[index];
            target.StatusResistance[index] += source.StatusResistance[index];
        }

        for (var index = 0; index < target.Damage.Length && index < source.Damage.Length; index++)
        {
            target.Damage[index] += source.Damage[index];
            target.Defense[index] += source.Defense[index];
            target.Resistance[index] += source.Resistance[index];
            target.Elusivity[index] += source.Elusivity[index];
        }

        for (var index = 0; index < target.DebuffResistance.Length && index < source.DebuffResistance.Length; index++)
        {
            target.DebuffResistance[index] += source.DebuffResistance[index];
        }
    }

    public static void ApplySelfBuffAccuracyPreview(
        IReadOnlyList<IPower?> mathPowers,
        IReadOnlyList<IPower?> buffedPowers,
        Enums.BuffsX selfBuffs,
        float oldBuffAcc,
        float oldToHit)
    {
        var newBuffAcc = selfBuffs.Effect[(int)Enums.eStatType.BuffAcc];
        var newToHit = selfBuffs.Effect[(int)Enums.eStatType.ToHit];

        if (Math.Abs(newBuffAcc - oldBuffAcc) < float.Epsilon && Math.Abs(newToHit - oldToHit) < float.Epsilon)
        {
            return;
        }

        var toHitScale = MidsContext.Config?.ScalingToHit ?? DatabaseAPI.ServerData.BaseToHit;
        for (var index = 0; index < mathPowers.Count && index < buffedPowers.Count; index++)
        {
            var powerMath = mathPowers[index];
            var powerBuffed = buffedPowers[index];
            if (powerMath == null || powerBuffed == null)
            {
                continue;
            }

            var effectiveOldToHit = powerMath.IgnoreBuff(Enums.eEnhance.ToHit) ? oldToHit : 0f;
            var effectiveNewToHit = powerMath.IgnoreBuff(Enums.eEnhance.ToHit) ? newToHit : 0f;
            var effectiveOldAcc = powerMath.IgnoreBuff(Enums.eEnhance.Accuracy) ? oldBuffAcc : 0f;
            var effectiveNewAcc = powerMath.IgnoreBuff(Enums.eEnhance.Accuracy) ? newBuffAcc : 0f;

            var oldAccuracyFactor = (1f + powerMath.Accuracy + effectiveOldAcc) * (toHitScale + effectiveOldToHit);
            var newAccuracyFactor = (1f + powerMath.Accuracy + effectiveNewAcc) * (toHitScale + effectiveNewToHit);
            if (Math.Abs(oldAccuracyFactor) < float.Epsilon || Math.Abs(newAccuracyFactor) < float.Epsilon)
            {
                continue;
            }

            var ratio = newAccuracyFactor / oldAccuracyFactor;
            powerBuffed.Accuracy *= ratio;

            var oldAccuracyMultFactor = 1f + powerMath.Accuracy + effectiveOldAcc;
            var newAccuracyMultFactor = 1f + powerMath.Accuracy + effectiveNewAcc;
            if (Math.Abs(oldAccuracyMultFactor) < float.Epsilon)
            {
                powerBuffed.AccuracyMult = 0f;
                continue;
            }

            powerBuffed.AccuracyMult *= newAccuracyMultFactor / oldAccuracyMultFactor;
        }
    }

    public static void ApplySupplementalEnhancementBuckets(
        ref IPower? powerMath,
        ref IPower? powerBuffed,
        Enums.BuffsX supplementalEnhance)
    {
        if (powerMath == null || powerBuffed == null)
        {
            return;
        }

        var oldAccuracy = powerMath.Accuracy;
        var oldEndCost = powerMath.EndCost;
        var oldInterruptTime = powerMath.InterruptTime;
        var oldRange = powerMath.Range;
        var oldRechargeTime = powerMath.RechargeTime;

        var allowAccuracy = powerMath.IgnoreEnhancement(Enums.eEnhance.Accuracy);
        var allowRecharge = powerMath.IgnoreEnhancement(Enums.eEnhance.RechargeTime);
        var allowEnduranceDiscount = powerMath.IgnoreEnhancement(Enums.eEnhance.EnduranceDiscount);

        for (var effectIndex = 0; effectIndex < supplementalEnhance.Effect.Length; effectIndex++)
        {
            var effectType = (Enums.eEffectType)effectIndex;
            switch (effectType)
            {
                case Enums.eEffectType.Accuracy:
                    if (allowAccuracy)
                    {
                        powerMath.Accuracy += supplementalEnhance.Effect[effectIndex];
                    }

                    break;

                case Enums.eEffectType.EnduranceDiscount:
                    if (allowEnduranceDiscount)
                    {
                        powerMath.EndCost += supplementalEnhance.Effect[effectIndex];
                    }

                    break;

                case Enums.eEffectType.InterruptTime:
                    EnhancementPolicyAxes.ApplyInterruptEnhancement(powerMath, supplementalEnhance.Effect[effectIndex]);
                    break;

                case Enums.eEffectType.Range:
                    EnhancementPolicyAxes.ApplyRangeEnhancement(powerMath, supplementalEnhance.Effect[effectIndex]);
                    break;

                case Enums.eEffectType.RechargeTime:
                    if (allowRecharge)
                    {
                        powerMath.RechargeTime += supplementalEnhance.Effect[effectIndex];
                    }

                    break;

                default:
                    for (var powerEffectIndex = 0; powerEffectIndex < powerMath.Effects.Length && powerEffectIndex < powerBuffed.Effects.Length; powerEffectIndex++)
                    {
                        if (!powerMath.Effects[powerEffectIndex].Buffable ||
                            powerMath.Effects[powerEffectIndex].EffectType != effectType ||
                            PlannerStrengthSemantics.IgnoresStrength(powerMath.Effects[powerEffectIndex]))
                        {
                            continue;
                        }

                        var mathEffect = powerMath.Effects[powerEffectIndex];
                        var buffedEffect = powerBuffed.Effects[powerEffectIndex];
                        var durationAdjustment = 0f;
                        var magnitudeAdjustment = 0f;

                        switch (effectType)
                        {
                            case Enums.eEffectType.Damage:
                                magnitudeAdjustment = supplementalEnhance.Damage[(int)mathEffect.DamageType];
                                break;

                            case Enums.eEffectType.Defense:
                                magnitudeAdjustment = supplementalEnhance.Defense[(int)mathEffect.DamageType];
                                break;

                            case Enums.eEffectType.Mez:
                            case Enums.eEffectType.MezProtect:
                                if (mathEffect.AttribType == Enums.eAttribType.Duration)
                                {
                                    durationAdjustment = supplementalEnhance.Mez[(int)mathEffect.MezType];
                                }
                                else
                                {
                                    magnitudeAdjustment = supplementalEnhance.Mez[(int)mathEffect.MezType];
                                }

                                break;

                            case Enums.eEffectType.Resistance:
                                magnitudeAdjustment = supplementalEnhance.Resistance[(int)mathEffect.DamageType];
                                break;

                            default:
                                if (mathEffect is
                                    {
                                        EffectType: Enums.eEffectType.Enhancement,
                                        ETModifies: Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping or Enums.eEffectType.JumpHeight or Enums.eEffectType.SpeedFlying
                                    })
                                {
                                    magnitudeAdjustment = buffedEffect.Mag > 0
                                        ? supplementalEnhance.Effect[(int)mathEffect.ETModifies]
                                        : supplementalEnhance.EffectAux[(int)mathEffect.ETModifies];
                                }
                                else if (mathEffect.EffectType is Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping or Enums.eEffectType.JumpHeight or Enums.eEffectType.SpeedFlying)
                                {
                                    magnitudeAdjustment = buffedEffect.Mag > 0
                                        ? supplementalEnhance.Effect[(int)mathEffect.EffectType]
                                        : supplementalEnhance.EffectAux[(int)mathEffect.EffectType];
                                }
                                else
                                {
                                    magnitudeAdjustment = supplementalEnhance.Effect[effectIndex];
                                }

                                break;
                        }

                        mathEffect.Math_Mag += magnitudeAdjustment;
                        mathEffect.Math_Duration += durationAdjustment;
                        buffedEffect.Math_Mag = buffedEffect.Mag * mathEffect.Math_Mag;
                        buffedEffect.Math_Duration = buffedEffect.Duration * mathEffect.Math_Duration;
                    }

                    break;
            }
        }

        if (Math.Abs(oldAccuracy + 1f) > float.Epsilon && Math.Abs(powerMath.Accuracy + 1f) > float.Epsilon)
        {
            var accuracyRatio = (1f + powerMath.Accuracy) / (1f + oldAccuracy);
            powerBuffed.Accuracy *= accuracyRatio;
            powerBuffed.AccuracyMult *= accuracyRatio;
        }

        if (Math.Abs(oldEndCost) > float.Epsilon && Math.Abs(powerMath.EndCost) > float.Epsilon)
        {
            powerBuffed.EndCost *= oldEndCost / powerMath.EndCost;
        }

        if (Math.Abs(oldInterruptTime) > float.Epsilon && Math.Abs(powerMath.InterruptTime) > float.Epsilon)
        {
            powerBuffed.InterruptTime *= oldInterruptTime / powerMath.InterruptTime;
        }

        if (Math.Abs(oldRange) > float.Epsilon)
        {
            powerBuffed.Range *= powerMath.Range / oldRange;
        }

        if (Math.Abs(oldRechargeTime) > float.Epsilon && Math.Abs(powerMath.RechargeTime) > float.Epsilon)
        {
            powerBuffed.RechargeTime *= oldRechargeTime / powerMath.RechargeTime;
        }
    }
}

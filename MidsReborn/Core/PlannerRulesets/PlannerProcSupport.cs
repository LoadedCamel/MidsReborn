using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core.PlannerRulesets;

internal readonly record struct ProcEvaluationLineage(
    float AreaFactorProduct,
    float SourceActivatePeriod,
    float SourceBaseRechargeTime,
    float SourceActivationTime,
    float SourceEndCost,
    float SourceRange,
    string SourcePowerFullName)
{
    public bool IsValid =>
        AreaFactorProduct > float.Epsilon ||
        SourceActivatePeriod > float.Epsilon ||
        SourceBaseRechargeTime > float.Epsilon ||
        SourceActivationTime > float.Epsilon ||
        SourceEndCost > float.Epsilon ||
        SourceRange > float.Epsilon ||
        !string.IsNullOrWhiteSpace(SourcePowerFullName);
}

internal static class PlannerProcSupport
{
    public static bool IsPlannerProcContributionEffect(IEffect? effect)
    {
        if (effect == null)
        {
            return false;
        }

        if (effect.IgnoreScaling || effect.IsFromProc)
        {
            return true;
        }

        if (!effect.isEnhancementEffect)
        {
            return false;
        }

        return effect is Effect concreteEffect &&
               concreteEffect.ProcContributionFlags.HasFlag(ProcContributionFlags.InheritedFromProcWrapper);
    }

    public static bool IsDirectProcDamageEffect(IEffect? effect)
    {
        return effect is { EffectType: Enums.eEffectType.Damage } &&
               IsPlannerProcContributionEffect(effect);
    }

    public static bool IsProcDerivedChanceModifier(IEffect? effect)
    {
        return effect is { EffectType: Enums.eEffectType.GlobalChanceMod } &&
               IsPlannerProcContributionEffect(effect);
    }

    public static void AppendProcEvaluationLineage(
        IEffect effect,
        IPower activationRootPower,
        IPower? expandedFromPower = null,
        bool forceActivationRoot = false)
    {
        if (effect is not Effect concreteEffect || activationRootPower == null)
        {
            return;
        }

        var existing = concreteEffect.ProcEvaluationLineage;
        var areaSourcePower = expandedFromPower;
        var sourceAreaFactor = areaSourcePower?.AoEModifier > float.Epsilon
            ? areaSourcePower.AoEModifier
            : 0f;
        var shouldReplaceRoot = forceActivationRoot || string.IsNullOrWhiteSpace(existing.SourcePowerFullName);
        concreteEffect.ProcEvaluationLineage = new ProcEvaluationLineage(
            sourceAreaFactor > float.Epsilon
                ? existing.AreaFactorProduct > float.Epsilon
                    ? existing.AreaFactorProduct * sourceAreaFactor
                    : sourceAreaFactor
                : existing.AreaFactorProduct,
            shouldReplaceRoot || existing.SourceActivatePeriod <= float.Epsilon
                ? Math.Max(0f, activationRootPower.ActivatePeriod)
                : existing.SourceActivatePeriod,
            shouldReplaceRoot || existing.SourceBaseRechargeTime <= float.Epsilon
                ? Math.Max(0f, activationRootPower.BaseRechargeTime)
                : existing.SourceBaseRechargeTime,
            shouldReplaceRoot || existing.SourceActivationTime <= float.Epsilon
                ? Math.Max(0f, activationRootPower.CastTime)
                : existing.SourceActivationTime,
            shouldReplaceRoot || existing.SourceEndCost <= float.Epsilon
                ? Math.Max(0f, activationRootPower.EndCost)
                : existing.SourceEndCost,
            shouldReplaceRoot || existing.SourceRange <= float.Epsilon
                ? Math.Max(0f, activationRootPower.Range)
                : existing.SourceRange,
            shouldReplaceRoot
                ? activationRootPower.FullName ?? string.Empty
                : existing.SourcePowerFullName);
    }

    public static float ResolveProcAreaFactor(IPower ownerPower, IEffect procEffect)
    {
        var areaFactorProduct = ownerPower.AoEModifier > float.Epsilon
            ? ownerPower.AoEModifier
            : 1f;
        if (procEffect is Effect concreteEffect &&
            concreteEffect.ProcEvaluationLineage.AreaFactorProduct > float.Epsilon)
        {
            areaFactorProduct *= concreteEffect.ProcEvaluationLineage.AreaFactorProduct;
        }

        return areaFactorProduct * 0.75f + 0.25f;
    }

    public static float ResolveProcSourceActivatePeriod(IPower ownerPower, IEffect procEffect)
    {
        if (procEffect is Effect concreteEffect &&
            concreteEffect.ProcEvaluationLineage.SourceActivatePeriod > float.Epsilon)
        {
            return concreteEffect.ProcEvaluationLineage.SourceActivatePeriod;
        }

        return Math.Max(0f, ownerPower?.ActivatePeriod ?? 0f);
    }

    public static float ResolvePowerBaseActivatePeriod(IEffect effect)
    {
        if (effect is Effect concreteEffect &&
            concreteEffect.ProcEvaluationLineage.SourceActivatePeriod > float.Epsilon)
        {
            return concreteEffect.ProcEvaluationLineage.SourceActivatePeriod;
        }

        return Math.Max(0f, effect.GetPower()?.ActivatePeriod ?? 0f);
    }

    public static float ResolvePowerBaseActivationTime(IEffect effect)
    {
        if (effect is Effect concreteEffect &&
            concreteEffect.ProcEvaluationLineage.SourceActivationTime > float.Epsilon)
        {
            return concreteEffect.ProcEvaluationLineage.SourceActivationTime;
        }

        return Math.Max(0f, effect.GetPower()?.CastTime ?? 0f);
    }

    public static float ResolvePowerBaseAreaFactor(IEffect effect)
    {
        return Math.Max(0f, effect.GetPower()?.AoEModifier ?? 0f);
    }

    public static float ResolvePowerBaseRechargeTime(IEffect effect)
    {
        if (effect is Effect concreteEffect &&
            concreteEffect.ProcEvaluationLineage.SourceBaseRechargeTime > float.Epsilon)
        {
            return concreteEffect.ProcEvaluationLineage.SourceBaseRechargeTime;
        }

        return Math.Max(0f, effect.GetPower()?.BaseRechargeTime ?? 0f);
    }

    public static float ResolvePowerBaseEndCost(IEffect effect)
    {
        if (effect is Effect concreteEffect &&
            concreteEffect.ProcEvaluationLineage.SourceEndCost > float.Epsilon)
        {
            return concreteEffect.ProcEvaluationLineage.SourceEndCost;
        }

        return Math.Max(0f, effect.GetPower()?.EndCost ?? 0f);
    }

    public static float ResolvePowerBaseRange(IEffect effect)
    {
        if (effect is Effect concreteEffect &&
            concreteEffect.ProcEvaluationLineage.SourceRange > float.Epsilon)
        {
            return concreteEffect.ProcEvaluationLineage.SourceRange;
        }

        return Math.Max(0f, effect.GetPower()?.Range ?? 0f);
    }

    public static string ResolvePowerBaseFullName(IEffect effect)
    {
        if (effect is Effect concreteEffect &&
            !string.IsNullOrWhiteSpace(concreteEffect.ProcEvaluationLineage.SourcePowerFullName))
        {
            return concreteEffect.ProcEvaluationLineage.SourcePowerFullName;
        }

        return effect.GetPower()?.FullName ?? string.Empty;
    }

    public static string ResolvePowerBaseSetFullName(IEffect effect)
    {
        var fullName = ResolvePowerBaseFullName(effect);
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return string.Empty;
        }

        var lastDotIndex = fullName.LastIndexOf('.');
        return lastDotIndex > 0 ? fullName[..lastDotIndex] : fullName;
    }
}

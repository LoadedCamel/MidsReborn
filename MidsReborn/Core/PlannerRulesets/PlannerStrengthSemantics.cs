using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core.PlannerRulesets;

internal static class PlannerStrengthSemantics
{
    public static bool IgnoresStrength(IEffect? effect)
    {
        if (effect == null)
        {
            return false;
        }

        if (effect.IgnoreStrength || effect.GetPower()?.IgnoreStrength == true)
        {
            return true;
        }

        return PlannerProcSupport.IsPlannerProcContributionEffect(effect);
    }

    public static bool IsStandaloneCarrierDamageEffect(IEffect? effect)
    {
        if (!PlannerProcSupport.IsDirectProcDamageEffect(effect))
        {
            return false;
        }

        var ownerPower = effect.GetPower();
        if (ownerPower == null)
        {
            return false;
        }

        var semanticSource = GlobalBoostPlannerSemantics.ResolveSemanticSourcePower(ownerPower, effect);
        if (semanticSource?.PowerType != Enums.ePowerType.GlobalBoost)
        {
            return false;
        }

        if (string.Equals(ownerPower.FullName, semanticSource.FullName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var rawOwnerPower = DatabaseAPI.GetPowerByFullName(ownerPower.FullName);
        if (rawOwnerPower == null)
        {
            return false;
        }

        return !HasHostileBaseDamage(rawOwnerPower);
    }

    private static bool HasHostileBaseDamage(IPower power)
    {
        return power.Effects.Any(effect =>
            effect.EffectType == Enums.eEffectType.Damage &&
            effect.ToWho is not (Enums.eToWho.Self or Enums.eToWho.All) &&
            effect.EffectClass != Enums.eEffectClass.Ignored &&
            effect.PvXInclude() &&
            effect.CanInclude());
    }
}

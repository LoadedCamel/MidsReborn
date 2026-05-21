using System.Linq;

namespace Mids_Reborn.Core;

internal static class EnhancementProcRules
{
    public static bool IsProcToggleEligible(IEnhancement? enhancement)
    {
        if (enhancement == null)
        {
            return false;
        }

        var boostPower = enhancement.GetPower();
        if (boostPower != null)
        {
            return HasToggleableProcEffect(boostPower);
        }

        return enhancement.IsProc;
    }

    public static bool HasToggleableProcEffect(IPower? boostPower)
    {
        if (boostPower?.Effects == null || boostPower.Effects.Length == 0)
        {
            return false;
        }

        return boostPower.Effects.Any(effect =>
            effect is { Absorbed_Effect: false } &&
            effect.EffectType != Enums.eEffectType.GrantPower &&
            (effect.ProcsPerMinute > 0f || effect.Probability < 1f));
    }
}

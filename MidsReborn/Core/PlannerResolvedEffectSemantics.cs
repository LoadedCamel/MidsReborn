using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core;

[Flags]
internal enum PlannerResolvedEffectKind
{
    None = 0,
    Base = 1 << 0,
    GrantChild = 1 << 1,
    ExecuteChild = 1 << 2,
    PseudoPetChild = 1 << 3,
    DeliveryChild = 1 << 4
}

internal static class PlannerResolvedEffectSemantics
{
    public static PlannerResolvedEffectKind GetResolvedEffectKind(this IEffect effect)
    {
        return effect is Effect concreteEffect
            ? concreteEffect.ResolvedEffectKind
            : PlannerResolvedEffectKind.None;
    }

    public static void SetResolvedEffectKind(this IEffect effect, PlannerResolvedEffectKind kind)
    {
        if (effect is Effect concreteEffect)
        {
            concreteEffect.ResolvedEffectKind = kind;
        }
    }

    public static void AddResolvedEffectKind(this IEffect effect, PlannerResolvedEffectKind kind)
    {
        if (effect is Effect concreteEffect)
        {
            concreteEffect.ResolvedEffectKind |= kind;
        }
    }

    public static bool HasResolvedEffectKind(this IEffect effect, PlannerResolvedEffectKind kind)
    {
        return (effect.GetResolvedEffectKind() & kind) == kind;
    }

    public static void MarkBaseEffects(IEnumerable<IEffect> effects)
    {
        foreach (var effect in effects)
        {
            effect.AddResolvedEffectKind(PlannerResolvedEffectKind.Base);
        }
    }
}

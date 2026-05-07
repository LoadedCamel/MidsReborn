using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

public static class EnhancementEffectMapper
{
    public static Enums.eEnhance MapEnhanceFromEffect(IEffect effect)
    {
        var sourceEffectType = effect.EffectType;
        if ((sourceEffectType == Enums.eEffectType.Enhancement || sourceEffectType == Enums.eEffectType.ResEffect) &&
            effect.ETModifies != Enums.eEffectType.None)
        {
            sourceEffectType = effect.ETModifies;
        }

        return MapEnhanceFromEffectType(sourceEffectType);
    }

    public static Enums.eEnhance MapEnhanceFromEffectType(Enums.eEffectType sourceEffectType)
        => sourceEffectType switch
        {
            Enums.eEffectType.Accuracy => Enums.eEnhance.Accuracy,
            Enums.eEffectType.Damage or Enums.eEffectType.DamageBuff => Enums.eEnhance.Damage,
            Enums.eEffectType.Defense => Enums.eEnhance.Defense,
            Enums.eEffectType.EnduranceDiscount => Enums.eEnhance.EnduranceDiscount,
            Enums.eEffectType.Endurance => Enums.eEnhance.Endurance,
            Enums.eEffectType.Fly or Enums.eEffectType.SpeedFlying or Enums.eEffectType.MaxFlySpeed => Enums.eEnhance.SpeedFlying,
            Enums.eEffectType.Heal => Enums.eEnhance.Heal,
            Enums.eEffectType.HitPoints => Enums.eEnhance.HitPoints,
            Enums.eEffectType.InterruptTime => Enums.eEnhance.Interrupt,
            Enums.eEffectType.JumpHeight => Enums.eEnhance.JumpHeight,
            Enums.eEffectType.SpeedJumping or Enums.eEffectType.MaxJumpSpeed => Enums.eEnhance.SpeedJumping,
            Enums.eEffectType.Mez => Enums.eEnhance.Mez,
            Enums.eEffectType.Range => Enums.eEnhance.Range,
            Enums.eEffectType.RechargeTime => Enums.eEnhance.RechargeTime,
            Enums.eEffectType.Recovery => Enums.eEnhance.Recovery,
            Enums.eEffectType.Regeneration => Enums.eEnhance.Regeneration,
            Enums.eEffectType.Resistance => Enums.eEnhance.Resistance,
            Enums.eEffectType.SpeedRunning or Enums.eEffectType.MaxRunSpeed => Enums.eEnhance.SpeedRunning,
            Enums.eEffectType.ToHit => Enums.eEnhance.ToHit,
            Enums.eEffectType.Slow => Enums.eEnhance.Slow,
            Enums.eEffectType.Absorb => Enums.eEnhance.Absorb,
            _ => Enums.eEnhance.None
        };
}

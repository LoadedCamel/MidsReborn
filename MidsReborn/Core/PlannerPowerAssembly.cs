using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

internal static class PlannerPowerAssembly
{
    public static void AddEnhancementEffects(Build? build, ref IPower? power, int historyIndex)
    {
        if (MidsContext.Config is null || build is null || MidsContext.Config.I9.IgnoreEnhFX || historyIndex < 0 || power is null)
        {
            return;
        }

        var currentPowerEntry = build.Powers[historyIndex];
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

            var isProcEnhancement = EnhancementProcRules.IsProcToggleEligible(enhancement);
            if (currentPowerEntry.ProcInclude & isProcEnhancement)
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
                if (!enhancementEffect.AffectsPetsOnly() && IsLocalBoostEnhancementEffect(enhancementEffect))
                {
                    continue;
                }

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

                                shouldAddEffect = entityPower.Effects.Any(effect => effect.EffectType == enhancementEffect.EffectType);
                                if (!shouldAddEffect)
                                {
                                    continue;
                                }

                                AddClonedEffectToList(newEffects, enhancementEffect, isProcEnhancement, power);
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
                    var hasLinkedSpecialBonus = enhancementIndex >= 0 &&
                                                enhancementSet.SpecialBonus[enhancementIndex].Index.Length > 0;

                    shouldAddEffect = enhancementIndex >= 0 &&
                                      ShouldAbsorbEnhancementEffectIntoHostPower(
                                          enhancement,
                                          enhancementPower,
                                          enhancementEffect,
                                          hasLinkedSpecialBonus);
                }

                if (!shouldAddEffect)
                {
                    continue;
                }

                AddClonedEffectToList(newEffects, enhancementEffect, isProcEnhancement, power);
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

    private static bool IsLocalBoostEnhancementEffect(IEffect enhancementEffect)
    {
        if (enhancementEffect == null)
        {
            return false;
        }

        if (EnhancementEffectMapper.MapEnhanceFromEffect(enhancementEffect) == Enums.eEnhance.None)
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(enhancementEffect.ModifierTable) &&
               enhancementEffect.ModifierTable.Contains("_Boosts_", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ShouldAbsorbEnhancementEffectIntoHostPower(
        IEnhancement enhancement,
        IPower enhancementPower,
        IEffect enhancementEffect,
        bool hasLinkedSpecialBonus)
    {
        if (enhancementEffect == null)
        {
            return false;
        }

        // Unique/global helper pieces now surface their popup FX from linked special/global
        // bonus ownership. Those rows must stay owned by set bonus plumbing instead of being
        // cloned onto the host power as live math.
        if (hasLinkedSpecialBonus || HasLinkedGlobalBonusOwnership(enhancementPower))
        {
            return false;
        }

        return enhancement.Effect.All(effect => effect.Mode != Enums.eEffMode.Enhancement) ||
               !Regex.IsMatch(
                   enhancementEffect.ModifierTable ?? string.Empty,
                   @"^(Melee|Ranged)_Boosts_",
                   RegexOptions.IgnoreCase);
    }

    private static bool HasLinkedGlobalBonusOwnership(IPower enhancementPower)
    {
        return enhancementPower is Power boostPower &&
               boostPower.OmniBoostPolicy.LinkedGlobalBonusPowerNames.Count > 0;
    }

    public static bool AddSubPowerEffects(Build? build, ref IPower power, int historyIndex)
    {
        if (build is null || power.NIDSubPower.Length <= 0 || historyIndex < 0)
        {
            return false;
        }

        var length = power.Effects.Length;
        var effectCount = 0;
        for (var index = 0; index < build.Powers[historyIndex].SubPowers.Length; index++)
        {
            if ((build.Powers[historyIndex].SubPowers[index].nIDPower > -1) &
                build.Powers[historyIndex].SubPowers[index].StatInclude)
            {
                effectCount += DatabaseAPI.Database.Power[power.NIDSubPower[index]].Effects.Length;
            }
        }

        var effectArray = new IEffect[power.Effects.Length + effectCount];
        Array.Copy(power.Effects, effectArray, power.Effects.Length);
        power.Effects = effectArray;
        foreach (var subPower in build.Powers[historyIndex].SubPowers.Where(sp => sp is { nIDPower: > -1, StatInclude: true }))
        {
            for (var index = 0; index < DatabaseAPI.Database.Power[subPower.nIDPower].Effects.Length; index++)
            {
                power.Effects[length] = (IEffect)DatabaseAPI.Database.Power[subPower.nIDPower].Effects[index].Clone();
                power.Effects[length].AddResolvedEffectKind(PlannerResolvedEffectKind.Base);
                power.Effects[length].Absorbed_EffectID = index;
                power.Effects[length].Absorbed_Effect = true;
                power.Effects[length].Absorbed_Power_nID = subPower.nIDPower;
                power.Effects[length].Absorbed_PowerType = DatabaseAPI.Database.Power[subPower.nIDPower].PowerType;
                length++;
            }
        }

        return true;
    }

    private static void AddClonedEffectToList(ICollection<IEffect> effectsList, IEffect enhancementEffect, bool isProc, IPower? ownerPower, bool isEnhancementEffect = true)
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
        clonedEffect.AddResolvedEffectKind(PlannerResolvedEffectKind.Base);
        if (ownerPower != null)
        {
            clonedEffect.SetPower(ownerPower);
        }
        effectsList.Add(clonedEffect);
    }
}

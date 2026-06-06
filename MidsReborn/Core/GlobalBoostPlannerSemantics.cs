using System;
using System.Collections.Generic;
using System.Linq;

namespace Mids_Reborn.Core;

internal enum GlobalBoostDamageFlavor
{
    Unknown,
    Damage,
    Resistance
}

internal static class GlobalBoostPlannerSemantics
{
    internal const string BoostModAllowedTagPrefix = "BoostModAllowed:";
    private const string EnhanceAccuracy = "Enhance Accuracy";
    private const string EnhanceConfuse = "Enhance Confuse";
    private const string EnhanceDamage = "Enhance Damage";
    private const string EnhanceDamageResistance = "Enhance Damage Resistance";
    private const string EnhanceDefense = "Enhance Defense";
    private const string EnhanceDefenseDebuff = "Enhance Defense Debuff";
    private const string EnhanceEnduranceModification = "Enhance Endurance Modification";
    private const string EnhanceFear = "Enhance Fear";
    private const string EnhanceFlyingSpeed = "Enhance Flying Speed";
    private const string EnhanceHeal = "Enhance Heal";
    private const string EnhanceHold = "Enhance Hold";
    private const string EnhanceImmobilization = "Enhance Immobilization";
    private const string EnhanceIntangibility = "Enhance Intangibility";
    private const string EnhanceJump = "Enhance Jump";
    private const string EnhanceKnockback = "Enhance Knockback";
    private const string EnhanceRange = "Enhance Range";
    private const string EnhanceRechargeSpeed = "Enhance Recharge Speed";
    private const string EnhanceRunningSpeed = "Enhance Running Speed";
    private const string EnhanceSleep = "Enhance Sleep";
    private const string EnhanceSlow = "Enhance Slow";
    private const string EnhanceStun = "Enhance Stun";
    private const string EnhanceTaunt = "Enhance Taunt";
    private const string EnhanceToHitBuffs = "Enhance ToHit Buffs";
    private const string EnhanceToHitDebuffs = "Enhance ToHit Debuffs";
    private const string MagicBoost = "magicboost";
    private const string MutationBoost = "mutationboost";
    private const string NaturalBoost = "naturalboost";
    private const string PowerBoostATag = "PowerBoostA";
    private const string PowerBoostBTag = "PowerBoostB";
    private const string ReduceEnduranceCost = "Reduce Endurance Cost";
    private const string ReduceInterruptTime = "Reduce Interrupt Time";
    private const string ScienceBoost = "scienceboost";
    private const string TechnologyBoost = "technologyboost";

    public static bool IsSourceEffectAllowedForPower(IPower targetPower, IPower sourcePower, IEffect sourceEffect)
    {
        if (targetPower == null || sourcePower == null || sourceEffect == null)
        {
            return false;
        }

        sourcePower = ResolveSemanticSourcePower(sourcePower, sourceEffect);
        var requiredLabels = GetRequiredBoostAllowedLabels(sourcePower, sourceEffect, out var hadBoostModAllowedMetadata);
        if (requiredLabels.Count == 0)
        {
            return !hadBoostModAllowedMetadata && HasPowerLevelOverlap(targetPower, sourcePower);
        }

        return requiredLabels.Any(label => PowerAllowsBoostLabel(targetPower, label));
    }

    public static bool IsPowerBoostTaggedEnhancementCarrierEffect(IEffect? effect)
    {
        return effect != null &&
               effect.EffectType is Enums.eEffectType.Enhancement or Enums.eEffectType.DamageBuff &&
               (HasEffectTag(effect, PowerBoostATag) || HasEffectTag(effect, PowerBoostBTag));
    }

    public static bool MatchesTargetEffect(IPower targetPower, IEffect targetEffect, IPower sourcePower, IEffect sourceEffect)
    {
        if (targetPower == null ||
            targetEffect == null ||
            sourcePower == null ||
            sourceEffect == null ||
            !targetEffect.Buffable ||
            !IsSourceEffectAllowedForPower(targetPower, sourcePower, sourceEffect))
        {
            return false;
        }

        sourcePower = ResolveSemanticSourcePower(sourcePower, sourceEffect);
        return sourceEffect.EffectType switch
        {
            Enums.eEffectType.DamageBuff => MatchesDamageBuffTargetEffect(targetEffect, sourcePower, sourceEffect),
            Enums.eEffectType.Enhancement => MatchesEnhancementTargetEffect(targetEffect, sourceEffect),
            _ => targetEffect.EffectType == sourceEffect.EffectType
        };
    }

    public static IPower ResolveSemanticSourcePower(IPower fallbackSourcePower, IEffect sourceEffect)
    {
        if (fallbackSourcePower == null)
        {
            return null!;
        }

        if (sourceEffect == null)
        {
            return fallbackSourcePower;
        }

        if (sourceEffect.Absorbed_Effect && sourceEffect.Absorbed_Power_nID > -1)
        {
            var absorbedPower = DatabaseAPI.Database.Power[sourceEffect.Absorbed_Power_nID];
            if (absorbedPower != null)
            {
                return absorbedPower;
            }
        }

        var omniSourcePower = TryResolveOmniSourcePower(sourceEffect.OmniSource);
        return omniSourcePower ?? fallbackSourcePower;
    }

    internal static IReadOnlyCollection<string> GetRequiredBoostAllowedLabels(
        IPower sourcePower,
        IEffect sourceEffect,
        out bool hadBoostModAllowedMetadata)
    {
        var resolvedFromMetadata = TryResolveBoostModAllowedLabelsFromMetadata(
            sourcePower,
            sourceEffect,
            out var metadataLabels,
            out hadBoostModAllowedMetadata);
        if (resolvedFromMetadata || hadBoostModAllowedMetadata)
        {
            return metadataLabels;
        }

        return GetRequiredBoostAllowedLabelsByEffectSemantics(sourcePower, sourceEffect);
    }

    private static IReadOnlyCollection<string> GetRequiredBoostAllowedLabelsByEffectSemantics(
        IPower sourcePower,
        IEffect sourceEffect,
        bool preferSemanticLabels = false)
    {
        if (sourcePower == null || sourceEffect == null)
        {
            return Array.Empty<string>();
        }

        if (sourceEffect.EffectType == Enums.eEffectType.DamageBuff)
        {
            return ClassifyDamageBuffFlavor(sourcePower, sourceEffect) switch
            {
                GlobalBoostDamageFlavor.Resistance => [EnhanceDamageResistance],
                GlobalBoostDamageFlavor.Damage => [EnhanceDamage],
                _ => preferSemanticLabels
                    ? Array.Empty<string>()
                    : BuildExistingBoostAllowedSubset(sourcePower, EnhanceDamageResistance, EnhanceDamage)
            };
        }

        if (sourceEffect.EffectType != Enums.eEffectType.Enhancement)
        {
            return Array.Empty<string>();
        }

        return sourceEffect.ETModifies switch
        {
            Enums.eEffectType.Accuracy => [EnhanceAccuracy],
            Enums.eEffectType.Absorb => [EnhanceHeal],
            Enums.eEffectType.Defense => ResolveDefenseLabels(sourcePower, sourceEffect, preferSemanticLabels),
            Enums.eEffectType.Endurance => [EnhanceEnduranceModification],
            Enums.eEffectType.EnduranceDiscount => [ReduceEnduranceCost],
            Enums.eEffectType.Heal => [EnhanceHeal],
            Enums.eEffectType.HitPoints => [EnhanceHeal],
            Enums.eEffectType.InterruptTime => [ReduceInterruptTime],
            Enums.eEffectType.JumpHeight => [EnhanceJump],
            Enums.eEffectType.Mez => ResolveMezLabels(sourceEffect),
            Enums.eEffectType.Range => [EnhanceRange],
            Enums.eEffectType.RechargeTime => [EnhanceRechargeSpeed],
            Enums.eEffectType.Recovery => [EnhanceEnduranceModification],
            Enums.eEffectType.Regeneration => [EnhanceHeal],
            Enums.eEffectType.Slow => [EnhanceSlow],
            Enums.eEffectType.SpeedFlying => [EnhanceFlyingSpeed],
            Enums.eEffectType.SpeedJumping => [EnhanceJump],
            Enums.eEffectType.SpeedRunning => [EnhanceRunningSpeed],
            Enums.eEffectType.ToHit => ResolveToHitLabels(sourcePower, sourceEffect, preferSemanticLabels),
            _ => Array.Empty<string>()
        };
    }

    internal static GlobalBoostDamageFlavor ClassifyDamageBuffFlavor(IPower sourcePower, IEffect? sourceEffect = null)
    {
        if (sourcePower == null)
        {
            return GlobalBoostDamageFlavor.Unknown;
        }

        if (TryResolveBoostModAllowedLabelsFromMetadata(sourcePower, sourceEffect, out var metadataLabels, out _))
        {
            var hasResistanceLabel = metadataLabels.Any(label =>
                string.Equals(label, EnhanceDamageResistance, StringComparison.OrdinalIgnoreCase));
            var hasDamageLabel = metadataLabels.Any(label =>
                string.Equals(label, EnhanceDamage, StringComparison.OrdinalIgnoreCase));
            if (hasResistanceLabel && !hasDamageLabel)
            {
                return GlobalBoostDamageFlavor.Resistance;
            }

            if (hasDamageLabel && !hasResistanceLabel)
            {
                return GlobalBoostDamageFlavor.Damage;
            }
        }

        if (sourceEffect != null && IsPowerBoostTaggedEnhancementCarrierEffect(sourceEffect))
        {
            return GlobalBoostDamageFlavor.Unknown;
        }

        var normalizedText = NormalizeLookupKey(
            $"{sourcePower.DisplayName} {sourcePower.DescShort} {sourcePower.DescLong} {sourcePower.FullName}");
        var allowsResistance = PowerAllowsBoostLabel(sourcePower, EnhanceDamageResistance);
        var allowsDamage = PowerAllowsBoostLabel(sourcePower, EnhanceDamage);

        if (normalizedText.Contains("damageresistance", StringComparison.Ordinal) ||
            normalizedText.Contains("resdamage", StringComparison.Ordinal))
        {
            return GlobalBoostDamageFlavor.Resistance;
        }

        if (allowsResistance && !allowsDamage)
        {
            return GlobalBoostDamageFlavor.Resistance;
        }

        if (allowsDamage && !allowsResistance)
        {
            return GlobalBoostDamageFlavor.Damage;
        }

        if (normalizedText.Contains("damage", StringComparison.Ordinal))
        {
            return GlobalBoostDamageFlavor.Damage;
        }

        return GlobalBoostDamageFlavor.Unknown;
    }

    private static bool MatchesDamageBuffTargetEffect(IEffect targetEffect, IPower sourcePower, IEffect sourceEffect)
    {
        if (targetEffect.DamageType != sourceEffect.DamageType)
        {
            return false;
        }

        return ClassifyDamageBuffFlavor(sourcePower, sourceEffect) switch
        {
            GlobalBoostDamageFlavor.Resistance => targetEffect.EffectType == Enums.eEffectType.Resistance,
            GlobalBoostDamageFlavor.Damage => targetEffect.EffectType == Enums.eEffectType.Damage,
            _ => targetEffect.EffectType is Enums.eEffectType.Resistance or Enums.eEffectType.Damage
        };
    }

    private static bool MatchesEnhancementTargetEffect(IEffect targetEffect, IEffect sourceEffect)
    {
        if (sourceEffect.ETModifies == Enums.eEffectType.None)
        {
            return false;
        }

        if (targetEffect.EffectType != sourceEffect.ETModifies)
        {
            return false;
        }

        return sourceEffect.ETModifies switch
        {
            Enums.eEffectType.Defense => sourceEffect.DamageType == Enums.eDamage.None ||
                                         targetEffect.DamageType == sourceEffect.DamageType,
            Enums.eEffectType.Damage or Enums.eEffectType.Resistance => targetEffect.DamageType == sourceEffect.DamageType,
            Enums.eEffectType.Mez => targetEffect.MezType == sourceEffect.MezType,
            _ => true
        };
    }

    private static IReadOnlyCollection<string> ResolveDefenseLabels(
        IPower sourcePower,
        IEffect sourceEffect,
        bool preferSemanticLabels = false)
    {
        if (preferSemanticLabels)
        {
            return sourceEffect.buffMode == Enums.eBuffMode.Debuff
                ? [EnhanceDefenseDebuff]
                : [EnhanceDefense];
        }

        if (PowerAllowsBoostLabel(sourcePower, EnhanceDefenseDebuff) || sourceEffect.buffMode == Enums.eBuffMode.Debuff)
        {
            return [EnhanceDefenseDebuff];
        }

        return PowerAllowsBoostLabel(sourcePower, EnhanceDefense)
            ? [EnhanceDefense]
            : BuildExistingBoostAllowedSubset(sourcePower, EnhanceDefenseDebuff, EnhanceDefense);
    }

    private static IReadOnlyCollection<string> ResolveToHitLabels(
        IPower sourcePower,
        IEffect sourceEffect,
        bool preferSemanticLabels = false)
    {
        if (preferSemanticLabels)
        {
            return sourceEffect.buffMode == Enums.eBuffMode.Debuff
                ? [EnhanceToHitDebuffs]
                : [EnhanceToHitBuffs];
        }

        if (PowerAllowsBoostLabel(sourcePower, EnhanceToHitDebuffs) || sourceEffect.buffMode == Enums.eBuffMode.Debuff)
        {
            return [EnhanceToHitDebuffs];
        }

        return PowerAllowsBoostLabel(sourcePower, EnhanceToHitBuffs)
            ? [EnhanceToHitBuffs]
            : BuildExistingBoostAllowedSubset(sourcePower, EnhanceToHitDebuffs, EnhanceToHitBuffs);
    }

    private static IReadOnlyCollection<string> ResolveMezLabels(IEffect sourceEffect)
    {
        return sourceEffect.MezType switch
        {
            Enums.eMez.Confused => [EnhanceConfuse],
            Enums.eMez.Held => [EnhanceHold],
            Enums.eMez.Immobilized => [EnhanceImmobilization],
            Enums.eMez.Intangible => [EnhanceIntangibility],
            Enums.eMez.Knockback or Enums.eMez.Knockup => [EnhanceKnockback],
            Enums.eMez.Sleep => [EnhanceSleep],
            Enums.eMez.Stunned => [EnhanceStun],
            Enums.eMez.Taunt => [EnhanceTaunt],
            Enums.eMez.Terrorized or Enums.eMez.Afraid => [EnhanceFear],
            _ => Array.Empty<string>()
        };
    }

    private static IReadOnlyCollection<string> BuildExistingBoostAllowedSubset(IPower power, params string[] candidateLabels)
    {
        if (power?.BoostsAllowed == null || power.BoostsAllowed.Length == 0 || candidateLabels.Length == 0)
        {
            return Array.Empty<string>();
        }

        var matches = candidateLabels
            .Where(label => PowerAllowsBoostLabel(power, label))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return matches.Length > 0 ? matches : Array.Empty<string>();
    }

    private static bool TryResolveBoostModAllowedLabelsFromMetadata(
        IPower sourcePower,
        IEffect? sourceEffect,
        out IReadOnlyCollection<string> labels,
        out bool hadBoostModAllowedMetadata)
    {
        hadBoostModAllowedMetadata = false;
        labels = Array.Empty<string>();
        if (sourceEffect == null)
        {
            return false;
        }

        var resolved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var token in EnumerateBoostModAllowedTokens(sourceEffect))
        {
            hadBoostModAllowedMetadata = true;
            foreach (var label in ResolveBoostModAllowedTokenToLabels(sourcePower, sourceEffect, token))
            {
                if (!string.IsNullOrWhiteSpace(label))
                {
                    resolved.Add(label);
                }
            }
        }

        if (resolved.Count == 0)
        {
            return false;
        }

        labels = resolved.ToArray();
        return true;
    }

    private static IEnumerable<string> ResolveBoostModAllowedTokenToLabels(
        IPower sourcePower,
        IEffect sourceEffect,
        string token)
    {
        switch (NormalizeLookupKey(token))
        {
            case "accuracyboost":
                return [EnhanceAccuracy];
            case "interruptboost":
                return [ReduceInterruptTime];
            case "confuseboost":
                return [EnhanceConfuse];
            case "damageboost":
                return [EnhanceDamage];
            case "buffdefenseboost":
                return [EnhanceDefense];
            case "debuffdefenseboost":
                return [EnhanceDefenseDebuff];
            case "recoveryboost":
                return [EnhanceEnduranceModification];
            case "endurancediscountboost":
                return [ReduceEnduranceCost];
            case "fearboost":
                return [EnhanceFear];
            case "speedflyingboost":
                return [EnhanceFlyingSpeed];
            case "healboost":
                return [EnhanceHeal];
            case "holdboost":
                return [EnhanceHold];
            case "immobilizedboost":
                return [EnhanceImmobilization];
            case "intangibleboost":
                return [EnhanceIntangibility];
            case "jumpboost":
                return [EnhanceJump];
            case "knockbackboost":
                return [EnhanceKnockback];
            case "rangeboost":
                return [EnhanceRange];
            case "rechargeboost":
                return [EnhanceRechargeSpeed];
            case "resdamageboost":
                return [EnhanceDamageResistance];
            case "speedrunningboost":
                return [EnhanceRunningSpeed];
            case "sleepboost":
                return [EnhanceSleep];
            case "slowboost":
                return [EnhanceSlow];
            case "stunnedboost":
                return [EnhanceStun];
            case "tauntboost":
                return [EnhanceTaunt];
            case "bufftohitboost":
                return [EnhanceToHitBuffs];
            case "debufftohitboost":
                return [EnhanceToHitDebuffs];
            case ScienceBoost:
            case TechnologyBoost:
            case MutationBoost:
            case MagicBoost:
            case NaturalBoost:
                if (sourceEffect.EffectType == Enums.eEffectType.DamageBuff &&
                    IsPowerBoostTaggedEnhancementCarrierEffect(sourceEffect))
                {
                    return Array.Empty<string>();
                }

                return GetRequiredBoostAllowedLabelsByEffectSemantics(
                    sourcePower,
                    sourceEffect,
                    preferSemanticLabels: true);
            default:
                return Array.Empty<string>();
        }
    }

    private static IEnumerable<string> EnumerateBoostModAllowedTokens(IEffect effect)
    {
        if (effect?.EffectTags == null)
        {
            yield break;
        }

        foreach (var tag in effect.EffectTags)
        {
            if (string.IsNullOrWhiteSpace(tag) ||
                !tag.StartsWith(BoostModAllowedTagPrefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var token = tag[BoostModAllowedTagPrefix.Length..].Trim();
            if (!string.IsNullOrWhiteSpace(token))
            {
                yield return token;
            }
        }
    }

    private static bool HasEffectTag(IEffect effect, string expectedTag)
    {
        return effect?.EffectTags?.Any(tag =>
            string.Equals(tag, expectedTag, StringComparison.OrdinalIgnoreCase)) == true;
    }

    private static bool HasPowerLevelOverlap(IPower targetPower, IPower sourcePower)
    {
        if (targetPower?.BoostsAllowed == null || sourcePower?.BoostsAllowed == null)
        {
            return false;
        }

        var target = targetPower.BoostsAllowed
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(NormalizeLookupKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        return sourcePower.BoostsAllowed
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(NormalizeLookupKey)
            .Any(target.Contains);
    }

    private static bool PowerAllowsBoostLabel(IPower power, string expectedLabel)
    {
        if (power?.BoostsAllowed == null || power.BoostsAllowed.Length == 0 || string.IsNullOrWhiteSpace(expectedLabel))
        {
            return false;
        }

        var expected = NormalizeLookupKey(expectedLabel);
        return power.BoostsAllowed.Any(label =>
            string.Equals(NormalizeLookupKey(label), expected, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeLookupKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return new string(value
            .Where(char.IsLetterOrDigit)
            .Select(char.ToLowerInvariant)
            .ToArray());
    }

    private static IPower? TryResolveOmniSourcePower(string? omniSource)
    {
        if (string.IsNullOrWhiteSpace(omniSource))
        {
            return null;
        }

        var separatorIndex = omniSource.IndexOf(':');
        var powerFullName = separatorIndex > 0
            ? omniSource[..separatorIndex]
            : omniSource;
        return string.IsNullOrWhiteSpace(powerFullName)
            ? null
            : DatabaseAPI.GetPowerByFullName(powerFullName);
    }
}

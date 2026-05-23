using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core.PlannerRulesets;

internal static class ChanceModifierSupport
{
    private const string ChanceModScopePrefix = "chance-mod-scope=";
    private const string GlobalChanceModScope = "global";
    private const string PowerLocalChanceModScope = "power-local";

    public static string ApplyChanceModScope(string omniSource, bool powerLocal)
    {
        var scopeValue = powerLocal ? PowerLocalChanceModScope : GlobalChanceModScope;
        var source = RemoveChanceModScope(omniSource ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(source)
            ? $"{ChanceModScopePrefix}{scopeValue}"
            : $"{source} | {ChanceModScopePrefix}{scopeValue}";
    }

    public static bool IsPowerLocalChanceMod(IEffect? effect)
    {
        return effect != null &&
               effect.OmniSource?.Contains($"{ChanceModScopePrefix}{PowerLocalChanceModScope}",
                   StringComparison.OrdinalIgnoreCase) == true;
    }

    public static float ApplyChanceModifiers(
        Character? character,
        IPower? ownerPower,
        IEffect procEffect,
        float probability,
        bool includePowerLocal)
    {
        if (character?.ModifyEffects != null)
        {
            foreach (var tag in EnumerateChanceTags(procEffect))
            {
                if (character.ModifyEffects.TryGetValue(tag, out var modifier))
                {
                    probability += modifier;
                }
            }
        }

        if (!includePowerLocal || ownerPower?.Effects == null)
        {
            return probability;
        }

        foreach (var effect in ownerPower.Effects)
        {
            if (effect.EffectType != Enums.eEffectType.GlobalChanceMod || !IsPowerLocalChanceMod(effect))
            {
                continue;
            }

            if (!effect.PvXInclude() || !effect.CanInclude() || effect.BaseProbability <= float.Epsilon)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(effect.Reward) || MatchesChanceTag(effect.Reward, procEffect))
            {
                probability += effect.BuffedMag;
            }
        }

        return probability;
    }

    public static bool TryGetChanceModifierScale(
        Character? character,
        IPower? ownerPower,
        string chanceTag,
        bool includePowerLocal,
        out float scale)
    {
        scale = 0;
        var found = false;

        if (includePowerLocal && ownerPower?.Effects != null)
        {
            foreach (var effect in ownerPower.Effects)
            {
                if (effect.EffectType != Enums.eEffectType.GlobalChanceMod ||
                    !IsPowerLocalChanceMod(effect) ||
                    !effect.PvXInclude() ||
                    !effect.CanInclude() ||
                    effect.BaseProbability <= float.Epsilon ||
                    !string.Equals(effect.Reward, chanceTag, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                scale += effect.BuffedMag;
                found = true;
            }
        }

        if (character?.ModifyEffects != null &&
            character.ModifyEffects.TryGetValue(chanceTag, out var globalScale))
        {
            scale += globalScale;
            found = true;
        }

        return found;
    }

    private static bool MatchesChanceTag(string chanceTag, IEffect effect)
    {
        return EnumerateChanceTags(effect)
            .Any(tag => string.Equals(tag, chanceTag, StringComparison.OrdinalIgnoreCase));
    }

    private static IEnumerable<string> EnumerateChanceTags(IEffect effect)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(effect.EffectId) && seen.Add(effect.EffectId))
        {
            yield return effect.EffectId;
        }

        foreach (var tag in effect.EffectTags ?? [])
        {
            if (!string.IsNullOrWhiteSpace(tag) && seen.Add(tag))
            {
                yield return tag;
            }
        }
    }

    private static string RemoveChanceModScope(string omniSource)
    {
        if (string.IsNullOrWhiteSpace(omniSource))
        {
            return string.Empty;
        }

        var segments = omniSource
            .Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Where(segment => !segment.StartsWith(ChanceModScopePrefix, StringComparison.OrdinalIgnoreCase));
        return string.Join(" | ", segments);
    }
}

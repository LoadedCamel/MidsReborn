using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core.PlannerRulesets;

internal static partial class PlannerEffectSourceSupport
{
    public static bool ShouldSuppressFallbackEffect(IEffect? effect)
    {
        if (effect == null || !HasFallbackTag(effect))
        {
            return false;
        }

        var ownerPower = effect.GetPower();
        if (ownerPower?.Effects == null || string.IsNullOrWhiteSpace(effect.OmniSource))
        {
            return false;
        }

        var fallbackKey = NormalizeFallbackComparableSource(effect.OmniSource);
        if (string.IsNullOrWhiteSpace(fallbackKey))
        {
            return false;
        }

        return ownerPower.Effects.Any(peer =>
            !ReferenceEquals(peer, effect) &&
            !HasFallbackTag(peer) &&
            AreEquivalentFallbackPeers(effect, peer) &&
            string.Equals(
                NormalizeFallbackComparableSource(peer.OmniSource),
                fallbackKey,
                StringComparison.OrdinalIgnoreCase));
    }

    public static bool HasFallbackTag(IEffect? effect)
    {
        return effect?.EffectTags.Any(tag =>
            !string.IsNullOrWhiteSpace(tag) &&
            tag.StartsWith("Fallback", StringComparison.OrdinalIgnoreCase)) == true;
    }

    private static string NormalizeFallbackComparableSource(string? omniSource)
    {
        if (string.IsNullOrWhiteSpace(omniSource))
        {
            return string.Empty;
        }

        var normalized = omniSource.Trim();
        normalized = EffectGroupIndexRegex().Replace(normalized, ":effect[*]");
        normalized = ChildGroupIndexRegex().Replace(normalized, ":child[*]");
        return normalized;
    }

    private static bool AreEquivalentFallbackPeers(IEffect left, IEffect right)
    {
        return left.EffectType == right.EffectType &&
               left.EffectClass == right.EffectClass &&
               left.DamageType == right.DamageType &&
               left.MezType == right.MezType &&
               left.ETModifies == right.ETModifies &&
               left.ToWho == right.ToWho &&
               left.PvMode == right.PvMode &&
               left.AttribType == right.AttribType &&
               left.Aspect == right.Aspect &&
               left.Scale.Equals(right.Scale) &&
               left.nMagnitude.Equals(right.nMagnitude) &&
               left.nDuration.Equals(right.nDuration) &&
               left.DelayedTime.Equals(right.DelayedTime) &&
               left.Ticks == right.Ticks &&
               left.BaseProbability.Equals(right.BaseProbability) &&
               left.ProcsPerMinute.Equals(right.ProcsPerMinute) &&
               left.IgnoreScaling == right.IgnoreScaling &&
               left.IgnoreStrength == right.IgnoreStrength &&
               StringEquals(left.Summon, right.Summon) &&
               StringEquals(left.Override, right.Override) &&
               StringEquals(left.RevokedPower, right.RevokedPower) &&
               StringEquals(left.Expressions?.Magnitude, right.Expressions?.Magnitude) &&
               StringEquals(left.Expressions?.Duration, right.Expressions?.Duration) &&
               StringEquals(left.Expressions?.Probability, right.Expressions?.Probability);
    }

    private static bool StringEquals(string? left, string? right)
    {
        return string.Equals(left?.Trim(), right?.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    [GeneratedRegex(@":effect\[\d+\]", RegexOptions.IgnoreCase)]
    private static partial Regex EffectGroupIndexRegex();

    [GeneratedRegex(@":child\[\d+\]", RegexOptions.IgnoreCase)]
    private static partial Regex ChildGroupIndexRegex();
}

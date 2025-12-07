namespace Mids_Reborn.Core;

/// <summary>
/// Law of Fives enforcement for set-bonus effects.
/// Groups effects into "bonus categories" and keeps the strongest 5 per category.
/// Categories map to what CoH caps together (e.g., Recharge, Defense(S/L), Resist(Fire), ToHit, etc.).
/// </summary>
internal static class LawOfFives
{
    private const int MaxPerCategory = 5;

    public static IEffect[] Enforce(IEnumerable<IEffect>? effects)
    {
        if (effects == null) return [];

        // Keep input order stable within strength ties.
        var indexed = effects.Select((fx, idx) => new { fx, idx }).ToArray();

        // Bucket by category → sort by strength desc → take top 5.
        var kept = new List<(int idx, IEffect fx)>(indexed.Length);

        foreach (var grp in indexed.GroupBy(x => BuildLawBucketKey(x.fx)))
        {
            var sorted = grp.OrderByDescending(x => Strength(x.fx))  // strongest first
                            .ThenBy(x => x.idx)                       // stable tie-break
                            .Take(MaxPerCategory);

            kept.AddRange(sorted.Select(s => (s.idx, s.fx)));
        }

        // Rebuild in original order for deterministic downstream behavior.
        return kept.OrderBy(k => k.idx).Select(k => k.fx).ToArray();
    }

    /// <summary>Ranking metric within a category: absolute BuffedMag (rounded to avoid float jitter).</summary>
    private static double Strength(IEffect e)
    {
        var mag = Math.Abs(e.BuffedMag);
        return Math.Round(mag, 8, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Compact key that captures the "bonus category" for Law-of-Fives.
    /// - Defense/Resistance/DamageBuff/Damage → keyed by DamageType vector.
    /// - Mez/MezProtection/MezResistance/Elusivity → keyed by Mez type.
    /// - ModifyAttrib (global modifiers like RechargeTime/EnduranceCost) → keyed by PowerAttribs.
    /// - Otherwise: EffectType + ETModifies + PowerAttribs.
    /// </summary>
    private readonly struct LawBucketKey : IEquatable<LawBucketKey>
    {
        public readonly Enums.eEffectType EffectType;
        public readonly Enums.eEffectType ETModifies;
        public readonly Enums.ePowerAttribs PowerAttrib;
        public readonly Enums.eDamage DamageType;
        public readonly Enums.eMez MezType;

        public LawBucketKey(Enums.eEffectType effectType,
                            Enums.eEffectType etModifies,
                            Enums.ePowerAttribs powerAttrib,
                            Enums.eDamage damageType,
                            Enums.eMez mezType)
        {
            EffectType = effectType;
            ETModifies = etModifies;
            PowerAttrib = powerAttrib;
            DamageType = damageType;
            MezType = mezType;
        }

        public bool Equals(LawBucketKey other) =>
            EffectType == other.EffectType &&
            ETModifies == other.ETModifies &&
            PowerAttrib == other.PowerAttrib &&
            DamageType == other.DamageType &&
            MezType == other.MezType;

        public override bool Equals(object? obj) => obj is LawBucketKey k && Equals(k);

        public override int GetHashCode()
        {
            unchecked
            {
                var h = (int)EffectType;
                h = (h * 397) ^ (int)ETModifies;
                h = (h * 397) ^ (int)PowerAttrib;
                h = (h * 397) ^ (int)DamageType;
                h = (h * 397) ^ (int)MezType;
                return h;
            }
        }
    }

    private static LawBucketKey BuildLawBucketKey(IEffect e)
    {
        var type = e.EffectType;

        switch (type)
        {
            // Vector categories (e.g., Defense(S/L), Resist(Fire))
            case Enums.eEffectType.Defense:
            case Enums.eEffectType.Resistance:
            case Enums.eEffectType.DamageBuff:
            case Enums.eEffectType.Damage:
                return new LawBucketKey(type, Enums.eEffectType.None, Enums.ePowerAttribs.None, e.DamageType, Enums.eMez.None);

            // Mez categories
            case Enums.eEffectType.Mez:
            case Enums.eEffectType.MezResist:
                return new LawBucketKey(type, Enums.eEffectType.None, Enums.ePowerAttribs.None, Enums.eDamage.None, e.MezType);

            case Enums.eEffectType.Elusivity:
                return new LawBucketKey(type, Enums.eEffectType.None, e.PowerAttribs, Enums.eDamage.None, Enums.eMez.None);

            // Global attribute modifiers (e.g., RechargeTime, EnduranceCost)
            case Enums.eEffectType.ModifyAttrib:
                return new LawBucketKey(type, Enums.eEffectType.None, e.PowerAttribs, Enums.eDamage.None, Enums.eMez.None);

            // Fallback: EffectType + ETModifies (+ PowerAttribs if set)
            default:
                return new LawBucketKey(type, e.ETModifies, e.PowerAttribs, Enums.eDamage.None, Enums.eMez.None);
        }
    }
}
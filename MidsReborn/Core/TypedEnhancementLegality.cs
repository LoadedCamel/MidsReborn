using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

public enum TypedEnhancementRestrictionFamily
{
    None = 0,
    Damage,
    Resistance
}

public readonly record struct TypedEnhancementRestriction(
    TypedEnhancementRestrictionFamily Family,
    Enums.eDamage DamageType)
{
    public bool IsValid => Family != TypedEnhancementRestrictionFamily.None &&
                           DamageType != Enums.eDamage.None;

    public override string ToString()
    {
        return $"{Family}:{DamageType}";
    }

    public static bool TryParse(string? value, out TypedEnhancementRestriction restriction)
    {
        restriction = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2 ||
            !Enum.TryParse(parts[0], true, out TypedEnhancementRestrictionFamily family) ||
            !Enum.TryParse(parts[1], true, out Enums.eDamage damageType))
        {
            return false;
        }

        restriction = new TypedEnhancementRestriction(family, damageType);
        return restriction.IsValid;
    }
}

public static class TypedEnhancementLegality
{
    public static TypedEnhancementRestriction[] Normalize(IEnumerable<TypedEnhancementRestriction>? restrictions)
    {
        return (restrictions ?? [])
            .Where(restriction => restriction.IsValid)
            .Distinct()
            .OrderBy(restriction => restriction.Family)
            .ThenBy(restriction => restriction.DamageType)
            .ToArray();
    }

    public static TypedEnhancementRestriction[] Deserialize(IEnumerable<string>? serialized)
    {
        return Normalize((serialized ?? [])
            .Select(value => TypedEnhancementRestriction.TryParse(value, out var restriction)
                ? restriction
                : default)
            .Where(restriction => restriction.IsValid));
    }

    public static string[] Serialize(IEnumerable<TypedEnhancementRestriction>? restrictions)
    {
        return Normalize(restrictions)
            .Select(restriction => restriction.ToString())
            .ToArray();
    }

    public static TypedEnhancementRestriction[] InferRestrictionsFromEffects(IEnumerable<IEffect>? effects)
    {
        if (effects == null)
        {
            return [];
        }

        var inferred = new List<TypedEnhancementRestriction>();
        foreach (var effect in effects.Where(effect => effect != null))
        {
            var effectiveType = GetEffectiveEffectType(effect);
            if (effect.DamageType == Enums.eDamage.None)
            {
                continue;
            }

            switch (effectiveType)
            {
                case Enums.eEffectType.Damage:
                case Enums.eEffectType.DamageBuff:
                    inferred.Add(new TypedEnhancementRestriction(TypedEnhancementRestrictionFamily.Damage, effect.DamageType));
                    break;
                case Enums.eEffectType.Resistance:
                    inferred.Add(new TypedEnhancementRestriction(TypedEnhancementRestrictionFamily.Resistance, effect.DamageType));
                    break;
            }
        }

        return Normalize(inferred);
    }

    public static bool AllowsEnhancement(IPower? power, IEnhancement? enhancement)
    {
        if (power?.TypedEnhancementRestrictions == null ||
            power.TypedEnhancementRestrictions.Length == 0 ||
            enhancement == null)
        {
            return true;
        }

        return !EnhancementMatchesRestrictions(enhancement, power.TypedEnhancementRestrictions);
    }

    public static int[] FilterAllowedEnhancementClassIds(
        IDatabase database,
        IEnumerable<int>? allowedClassIds,
        IEnumerable<TypedEnhancementRestriction>? restrictions)
    {
        var normalizedRestrictions = Normalize(restrictions);
        var classIds = (allowedClassIds ?? [])
            .Distinct()
            .ToArray();
        if (classIds.Length == 0 || normalizedRestrictions.Length == 0 || database.Enhancements == null)
        {
            return classIds;
        }

        var enhancementClasses = database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>();
        var filtered = new List<int>(classIds.Length);
        foreach (var allowedClassId in classIds)
        {
            var classHasBlockedEnhancement = database.Enhancements
                .Where(enhancement => enhancement != null && EnhancementBelongsToClass(enhancement, allowedClassId, enhancementClasses))
                .Any(enhancement => EnhancementMatchesRestrictions(enhancement, normalizedRestrictions));
            if (!classHasBlockedEnhancement)
            {
                filtered.Add(allowedClassId);
            }
        }

        return filtered.ToArray();
    }

    public static bool EffectMatchesRestrictions(IEffect? effect, IEnumerable<TypedEnhancementRestriction>? restrictions)
    {
        if (effect == null)
        {
            return false;
        }

        return Normalize(restrictions)
            .Any(restriction => EffectMatchesRestriction(effect, restriction));
    }

    public static bool EnhancementMatchesRestrictions(
        IEnhancement enhancement,
        IEnumerable<TypedEnhancementRestriction>? restrictions)
    {
        var normalizedRestrictions = Normalize(restrictions);
        if (normalizedRestrictions.Length == 0)
        {
            return false;
        }

        return EnumerateEnhancementEffects(enhancement)
            .Any(effect => normalizedRestrictions.Any(restriction => EffectMatchesRestriction(effect, restriction)));
    }

    private static IEnumerable<IEffect> EnumerateEnhancementEffects(IEnhancement enhancement)
    {
        foreach (var effect in enhancement.GetPower()?.Effects ?? [])
        {
            if (effect != null)
            {
                yield return effect;
            }
        }

        foreach (var effect in enhancement.Effect
                     .Where(effect => effect.Mode == Enums.eEffMode.FX && effect.FX != null)
                     .Select(effect => effect.FX!))
        {
            yield return effect;
        }
    }

    private static bool EnhancementBelongsToClass(
        IEnhancement enhancement,
        int allowedClassId,
        IReadOnlyList<Enums.sEnhClass> enhancementClasses)
    {
        foreach (var classRef in enhancement.ClassID ?? Array.Empty<int>())
        {
            if (classRef == allowedClassId)
            {
                return true;
            }

            if (classRef >= 0 &&
                classRef < enhancementClasses.Count &&
                enhancementClasses[classRef].ID == allowedClassId)
            {
                return true;
            }
        }

        return false;
    }

    private static bool EffectMatchesRestriction(IEffect effect, TypedEnhancementRestriction restriction)
    {
        if (!restriction.IsValid || effect.DamageType != restriction.DamageType)
        {
            return false;
        }

        var effectiveType = GetEffectiveEffectType(effect);
        return restriction.Family switch
        {
            TypedEnhancementRestrictionFamily.Damage => effectiveType is Enums.eEffectType.Damage or Enums.eEffectType.DamageBuff,
            TypedEnhancementRestrictionFamily.Resistance => effectiveType == Enums.eEffectType.Resistance,
            _ => false
        };
    }

    private static Enums.eEffectType GetEffectiveEffectType(IEffect effect)
    {
        return (effect.EffectType is Enums.eEffectType.Enhancement or Enums.eEffectType.ResEffect) &&
               effect.ETModifies != Enums.eEffectType.None
            ? effect.ETModifies
            : effect.EffectType;
    }
}

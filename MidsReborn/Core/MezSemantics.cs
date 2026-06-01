namespace Mids_Reborn.Core;

internal static class MezSemantics
{
    internal static bool IsStatusEffectType(Enums.eEffectType effectType)
    {
        return effectType is Enums.eEffectType.Mez or Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist;
    }

    internal static bool IsStatusProtectionType(Enums.eEffectType effectType)
    {
        return effectType == Enums.eEffectType.MezProtect;
    }

    internal static bool IsStatusResistanceType(Enums.eEffectType effectType)
    {
        return effectType == Enums.eEffectType.MezResist;
    }

    internal static string GetFriendlyMezLabel(Enums.eMez mezType)
    {
        return mezType switch
        {
            Enums.eMez.Confused => "Confusion",
            Enums.eMez.Held => "Hold",
            Enums.eMez.Immobilized => "Immobilization",
            Enums.eMez.Knockback => "Knockback",
            Enums.eMez.Knockup => "Knockup",
            Enums.eMez.OnlyAffectsSelf => "Self Only",
            Enums.eMez.Placate => "Placate",
            Enums.eMez.Repel => "Repel",
            Enums.eMez.Sleep => "Sleep",
            Enums.eMez.Stunned => "Stun",
            Enums.eMez.Taunt => "Taunt",
            Enums.eMez.Terrorized => "Fear",
            Enums.eMez.Untouchable => "Untouchable",
            Enums.eMez.Teleport => "Teleportation",
            Enums.eMez.ToggleDrop => "Toggle Drop",
            Enums.eMez.Afraid => "Afraid",
            Enums.eMez.Avoid => "Avoid",
            Enums.eMez.CombatPhase => "Combat Phase",
            Enums.eMez.Intangible => "Intangible",
            _ => mezType.ToString()
        };
    }

    internal static string GetFriendlyMezShortLabel(Enums.eMez mezType)
    {
        return mezType switch
        {
            Enums.eMez.Confused => "Conf",
            Enums.eMez.Held => "Hold",
            Enums.eMez.Immobilized => "Immob",
            Enums.eMez.Knockback => "KB",
            Enums.eMez.Knockup => "KUp",
            Enums.eMez.OnlyAffectsSelf => "Self",
            Enums.eMez.Placate => "Plac",
            Enums.eMez.Repel => "Repel",
            Enums.eMez.Sleep => "Sleep",
            Enums.eMez.Stunned => "Stun",
            Enums.eMez.Taunt => "Taunt",
            Enums.eMez.Terrorized => "Fear",
            Enums.eMez.Untouchable => "Untch",
            Enums.eMez.Teleport => "TP",
            Enums.eMez.ToggleDrop => "DeTogg",
            Enums.eMez.Afraid => "Afraid",
            Enums.eMez.Avoid => "Avoid",
            Enums.eMez.CombatPhase => "Phase",
            Enums.eMez.Intangible => "Intan",
            _ => mezType.ToString()
        };
    }

    internal static string GetStatusProtectionLabel(Enums.eMez mezType, bool shortForm = false)
    {
        if (mezType == Enums.eMez.None)
        {
            return shortForm ? "Stat Prot" : "Status Protection";
        }

        var baseLabel = shortForm ? GetFriendlyMezShortLabel(mezType) : GetFriendlyMezLabel(mezType);
        return shortForm ? $"{baseLabel} Prot" : $"{baseLabel} Protection";
    }

    internal static string GetStatusResistanceLabel(Enums.eMez mezType, bool shortForm = false)
    {
        if (mezType == Enums.eMez.None)
        {
            return shortForm ? "Stat Res" : "Status Resistance";
        }

        var baseLabel = shortForm ? GetFriendlyMezShortLabel(mezType) : GetFriendlyMezLabel(mezType);
        return shortForm ? $"{baseLabel} Res" : $"{baseLabel} Resistance";
    }

    internal static string NormalizeMezVectorLabels(string vectors)
    {
        if (string.IsNullOrWhiteSpace(vectors))
        {
            return vectors;
        }

        var parts = vectors
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(NormalizeSingleVectorLabel)
            .ToList();

        return parts.Count == 0 ? vectors : string.Join(", ", parts);
    }

    internal static bool MatchesSingleMezVector(Enums.eMez mezType, string vectors)
    {
        if (string.IsNullOrWhiteSpace(vectors))
        {
            return false;
        }

        var normalized = NormalizeSingleVectorLabel(vectors);
        return string.Equals(normalized, GetFriendlyMezLabel(mezType), StringComparison.OrdinalIgnoreCase) ||
               string.Equals(normalized, mezType.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeSingleVectorLabel(string vector)
    {
        if (Enum.TryParse<Enums.eMez>(vector.Trim(), out var mezType))
        {
            return GetFriendlyMezLabel(mezType);
        }

        return vector.Trim();
    }
}

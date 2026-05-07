namespace Mids_Reborn.Core.Omni;

public enum OmniBuildActorKind
{
    Player,
    RealPet,
    PseudoPet
}

public enum OmniPetClassificationReason
{
    Player,
    RealPetGroup,
    CommandablePet,
    PersistentPetWithPowers,
    PersistentBuffablePet,
    PseudoPatchOrTrap,
    PseudoIgnoredExecutionEntity,
    PseudoOneShotOrAnchor,
    PseudoNoPersistentPowers,
    Unknown
}

public sealed class OmniBuildActor
{
    public OmniBuildActorKind Kind { get; set; }
    public bool IsRealPet => Kind == OmniBuildActorKind.RealPet;
    public bool IsPseudoPet => Kind == OmniBuildActorKind.PseudoPet;
    public OmniPetClassificationReason ClassificationReason { get; set; } = OmniPetClassificationReason.Unknown;
    public string EntityName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string OwnerEntityName { get; set; } = string.Empty;
    public string SummonedByPower { get; set; } = string.Empty;
    public List<string> Powersets { get; set; } = [];
    public List<string> Powers { get; set; } = [];
    public string SyntheticPowersetFullName { get; set; } = string.Empty;
    public string SyntheticPowersetDisplayName { get; set; } = string.Empty;
    public Dictionary<string, string> SyntheticPowerAliasesBySource { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public static class OmniPetClassifier
{
    private static readonly string[] RealPetPrefixes =
    [
        "Mastermind_Pets.",
        "Kheldian_Pets.",
        "Villain_Pets.",
        "Incarnate."
    ];

    private static readonly string[] PseudoNameFragments =
    [
        "patch",
        "pseudopet",
        "pseudo_pet",
        "cloud",
        "rain",
        "storm",
        "mine",
        "trap",
        "bomb",
        "anchor",
        "field",
        "location",
        "one_shot",
        "oneshot",
        "ignore"
    ];

    public static OmniBuildActor Classify(OmniEntityDefinition entity, string summonedByPower = "")
    {
        var actor = new OmniBuildActor
        {
            EntityName = entity.InternalName,
            DisplayName = entity.DisplayName,
            ClassName = entity.ClassId,
            SummonedByPower = summonedByPower,
            Powers = entity.PowerIds
                .Select(NormalizePowerId)
                .Where(power => !string.IsNullOrWhiteSpace(power))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
        };

        actor.Powersets = actor.Powers
            .Select(FullSetNameFromPower)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (actor.Powersets.Count == 0)
        {
            actor.Powersets = entity.PowerReferences
                .Select(p => $"{p.PowerCategory}.{p.PowerSet}")
                .Where(p => !string.IsNullOrWhiteSpace(p) && !p.EndsWith(".", StringComparison.Ordinal))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        var loweredName = entity.InternalName.ToLowerInvariant();
        var loweredAi = entity.AiConfig.ToLowerInvariant();
        var hasPseudoName = PseudoNameFragments.Any(f => loweredName.Contains(f) || loweredAi.Contains(f));
        var hasPowers = entity.PowerIds.Count > 0 || entity.PowerReferences.Count > 0;
        var hasRealPetClass = entity.ClassId.Contains("Pet", StringComparison.OrdinalIgnoreCase) ||
                              entity.ClassId.Contains("Henchman", StringComparison.OrdinalIgnoreCase);
        var hasPetAi = loweredAi.StartsWith("pets_", StringComparison.OrdinalIgnoreCase);
        var hasIgnoredAi = loweredAi.Contains("ignore", StringComparison.OrdinalIgnoreCase);
        var hasScopedRealPetPowers = entity.PowerIds
            .Select(NormalizePowerId)
            .Any(power => RealPetPrefixes.Any(prefix => power.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));

        if (hasPseudoName && entity.PetCommandability <= 0)
        {
            actor.Kind = OmniBuildActorKind.PseudoPet;
            actor.ClassificationReason = hasIgnoredAi
                ? OmniPetClassificationReason.PseudoIgnoredExecutionEntity
                : OmniPetClassificationReason.PseudoPatchOrTrap;
            return actor;
        }

        if (entity.PetCommandability > 0)
        {
            actor.Kind = OmniBuildActorKind.RealPet;
            actor.ClassificationReason = OmniPetClassificationReason.CommandablePet;
            return actor;
        }

        if (hasScopedRealPetPowers && hasRealPetClass && hasPetAi && !hasIgnoredAi)
        {
            actor.Kind = OmniBuildActorKind.RealPet;
            actor.ClassificationReason = OmniPetClassificationReason.RealPetGroup;
            return actor;
        }

        if (hasPowers && hasRealPetClass && hasPetAi && !hasIgnoredAi)
        {
            actor.Kind = OmniBuildActorKind.RealPet;
            actor.ClassificationReason = entity.CanZone
                ? OmniPetClassificationReason.PersistentBuffablePet
                : OmniPetClassificationReason.PersistentPetWithPowers;
            return actor;
        }

        actor.Kind = OmniBuildActorKind.PseudoPet;
        actor.ClassificationReason = hasPowers
            ? OmniPetClassificationReason.PseudoOneShotOrAnchor
            : OmniPetClassificationReason.PseudoNoPersistentPowers;

        return actor;
    }

    private static string NormalizePowerId(string powerId)
    {
        const string powerPrefix = "power:";
        var value = powerId.Trim();
        return value.StartsWith(powerPrefix, StringComparison.OrdinalIgnoreCase)
            ? value[powerPrefix.Length..]
            : value;
    }

    private static string FullSetNameFromPower(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 ? $"{parts[0]}.{parts[1]}" : string.Empty;
    }
}

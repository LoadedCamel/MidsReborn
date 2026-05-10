using System.Text.RegularExpressions;

namespace Mids_Reborn.Core.Omni;

[Flags]
internal enum PlannerRecipientFlags
{
    None = 0,
    Player = 1,
    OwnedRealPet = 2
}

internal enum PlannerSelfRequirement
{
    Any = 0,
    SelfOnly = 1,
    NonSelfOnly = 2
}

internal enum PlannerRecipientClauseKind
{
    PlayerRecipient = 0,
    OwnedRealPetRecipient = 1,
    SelfRecipient = 2,
    TargetTag = 3,
    Never = 4
}

internal sealed class PlannerRecipientClause
{
    public AdvancedConditionLink Link { get; set; } = AdvancedConditionLink.And;
    public PlannerRecipientClauseKind Kind { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool Negated { get; set; }

    public PlannerRecipientClause Clone()
    {
        return new PlannerRecipientClause
        {
            Link = Link,
            Kind = Kind,
            Value = Value,
            Negated = Negated
        };
    }
}

internal sealed class PlannerTargetRoutingPolicy
{
    public static readonly PlannerTargetRoutingPolicy Default = new(
        PlannerRecipientFlags.Player | PlannerRecipientFlags.OwnedRealPet,
        PlannerSelfRequirement.Any,
        new AdvancedConditionSet(),
        [],
        new AdvancedConditionSet(),
        new AdvancedConditionSet(),
        string.Empty,
        hasImportedMetadata: false);

    public PlannerTargetRoutingPolicy(
        PlannerRecipientFlags allowedRecipients,
        PlannerSelfRequirement selfRequirement,
        AdvancedConditionSet buildSourceGates,
        IReadOnlyList<PlannerRecipientClause> recipientClauses,
        AdvancedConditionSet deferredTargetRows,
        AdvancedConditionSet deferredSourceRows,
        string originalTargetRequires,
        bool hasImportedMetadata)
    {
        AllowedRecipients = allowedRecipients;
        SelfRequirement = selfRequirement;
        BuildSourceGates = buildSourceGates?.Clone() ?? new AdvancedConditionSet();
        RecipientClauses = recipientClauses?
            .Select(clause => clause.Clone())
            .ToArray() ?? [];
        DeferredTargetRows = deferredTargetRows?.Clone() ?? new AdvancedConditionSet();
        DeferredSourceRows = deferredSourceRows?.Clone() ?? new AdvancedConditionSet();
        OriginalTargetRequires = originalTargetRequires ?? string.Empty;
        HasImportedMetadata = hasImportedMetadata;
    }

    public PlannerRecipientFlags AllowedRecipients { get; }
    public PlannerSelfRequirement SelfRequirement { get; }
    public AdvancedConditionSet BuildSourceGates { get; }
    public IReadOnlyList<PlannerRecipientClause> RecipientClauses { get; }
    public AdvancedConditionSet DeferredTargetRows { get; }
    public AdvancedConditionSet DeferredSourceRows { get; }
    public string OriginalTargetRequires { get; }
    public bool HasImportedMetadata { get; }

    public bool IsDefault =>
        !HasImportedMetadata &&
        string.IsNullOrWhiteSpace(OriginalTargetRequires) &&
        BuildSourceGates.Rows.Count == 0 &&
        RecipientClauses.Count == 0 &&
        DeferredTargetRows.Rows.Count == 0 &&
        DeferredSourceRows.Rows.Count == 0 &&
        AllowedRecipients == (PlannerRecipientFlags.Player | PlannerRecipientFlags.OwnedRealPet) &&
        SelfRequirement == PlannerSelfRequirement.Any;

    public PlannerTargetRoutingPolicy Clone()
    {
        return new PlannerTargetRoutingPolicy(
            AllowedRecipients,
            SelfRequirement,
            BuildSourceGates,
            RecipientClauses,
            DeferredTargetRows,
            DeferredSourceRows,
            OriginalTargetRequires,
            HasImportedMetadata);
    }
}

internal static partial class PlannerConditionRoutingAnalyzer
{
    private static readonly Regex TargetHasTagRegex = new(@"^!?target\.HasTag\?\(([^)]+)\)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex InfixSelfRegex = new(@"^target>entref\s+(eq|==|!=|ne)\s+source>entref$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex ForwardFriendEntityRegex = new(@"^target\.isFriend\?\s+enttype\s+target>\s+([A-Za-z0-9_'""]+)\s+eq\s+&&$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex ReverseFriendEntityRegex = new(@"^enttype\s+target>\s+([A-Za-z0-9_'""]+)\s+eq\s+target\.isFriend\?\s+&&$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex ReverseFoeEntityRegex = new(@"^enttype\s+target>\s+([A-Za-z0-9_'""]+)\s+eq\s+target\.isFriend\?\s+!\s+&&$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static PlannerTargetRoutingPolicy Analyze(string? expression, string ownerFullName = "")
    {
        var original = expression ?? string.Empty;
        if (string.IsNullOrWhiteSpace(original))
        {
            return PlannerTargetRoutingPolicy.Default;
        }

        var builder = new PolicyBuilder(original);
        var rows = OmniExpressionConverter.ToRows(
            original,
            AdvancedConditionEvaluationMode.ReportOnly,
            ownerFullName);

        foreach (var row in rows)
        {
            if (TryAddSupportedRawRouting(row.RawExpression, row.Link, builder))
            {
                continue;
            }

            if (TryAddBuildSourceGate(row, builder))
            {
                continue;
            }

            if (TryAddRecipientClause(row, builder))
            {
                continue;
            }

            builder.AddDeferred(row);
        }

        return builder.Build();
    }

    public static string DescribeRecipientClause(PlannerRecipientClause clause)
    {
        return clause.Kind switch
        {
            PlannerRecipientClauseKind.PlayerRecipient => clause.Negated ? "recipient is not player" : "recipient is player",
            PlannerRecipientClauseKind.OwnedRealPetRecipient => clause.Negated ? "recipient is not owned real pet" : "recipient is owned real pet",
            PlannerRecipientClauseKind.SelfRecipient => clause.Negated ? "recipient is not self" : "recipient is self",
            PlannerRecipientClauseKind.TargetTag => clause.Negated ? $"recipient lacks tag {clause.Value}" : $"recipient has tag {clause.Value}",
            PlannerRecipientClauseKind.Never => "recipient routing blocked",
            _ => clause.Kind.ToString()
        };
    }

    private static bool TryAddBuildSourceGate(AdvancedConditionRow row, PolicyBuilder builder)
    {
        if (row.EvaluationMode != AdvancedConditionEvaluationMode.BuildEvaluated)
        {
            return false;
        }

        if (row.Kind is not (AdvancedConditionKind.SourceMode or AdvancedConditionKind.SourceOwnPower))
        {
            return false;
        }

        builder.AddBuildSourceGate(row);
        return true;
    }

    private static bool TryAddRecipientClause(AdvancedConditionRow row, PolicyBuilder builder)
    {
        if (row.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated)
        {
            return false;
        }

        if (row.Kind == AdvancedConditionKind.TargetEntityType &&
            TryCreateTargetEntityClause(row, out var entityClause))
        {
            builder.AddRecipientClause(entityClause);
            return true;
        }

        if (row.Kind == AdvancedConditionKind.TargetGroup &&
            TryCreateTargetGroupClause(row, out var groupClause))
        {
            builder.AddRecipientClause(groupClause);
            return true;
        }

        return false;
    }

    private static bool TryCreateTargetEntityClause(AdvancedConditionRow row, out PlannerRecipientClause clause)
    {
        clause = new PlannerRecipientClause();
        if (row.Kind != AdvancedConditionKind.TargetEntityType)
        {
            return false;
        }

        if (row.TargetScope == AdvancedConditionTargetScope.Self)
        {
            clause = new PlannerRecipientClause
            {
                Link = row.Link,
                Kind = PlannerRecipientClauseKind.SelfRecipient,
                Negated = row.Operator == AdvancedConditionOperator.NotEquals || row.Negated
            };
            return true;
        }

        if (row.Operator != AdvancedConditionOperator.Equals || row.Negated)
        {
            return false;
        }

        var normalized = NormalizeTargetLiteral(row.Subject, row.Value);
        if (normalized is "player" or "pc")
        {
            clause = new PlannerRecipientClause
            {
                Link = row.Link,
                Kind = PlannerRecipientClauseKind.PlayerRecipient
            };
            return true;
        }

        if (normalized is "critter" or "npc" or "pet" or "pets" or "henchman")
        {
            clause = new PlannerRecipientClause
            {
                Link = row.Link,
                Kind = PlannerRecipientClauseKind.OwnedRealPetRecipient
            };
            return true;
        }

        return false;
    }

    private static bool TryCreateTargetGroupClause(AdvancedConditionRow row, out PlannerRecipientClause clause)
    {
        clause = new PlannerRecipientClause();
        if (row.Kind != AdvancedConditionKind.TargetGroup ||
            row.Operator != AdvancedConditionOperator.Equals ||
            row.Negated)
        {
            return false;
        }

        if (!IsOwnedPetGroup(row.Value))
        {
            return false;
        }

        clause = new PlannerRecipientClause
        {
            Link = row.Link,
            Kind = PlannerRecipientClauseKind.OwnedRealPetRecipient
        };
        return true;
    }

    private static bool TryAddSupportedRawRouting(string rawExpression, AdvancedConditionLink link, PolicyBuilder builder)
    {
        var normalized = NormalizeRaw(rawExpression);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return false;
        }

        var tagMatch = TargetHasTagRegex.Match(normalized);
        if (tagMatch.Success)
        {
            builder.AddRecipientClause(new PlannerRecipientClause
            {
                Link = link,
                Kind = PlannerRecipientClauseKind.TargetTag,
                Value = tagMatch.Groups[1].Value.Trim(),
                Negated = normalized.StartsWith("!", StringComparison.Ordinal)
            });
            return true;
        }

        var selfMatch = InfixSelfRegex.Match(normalized);
        if (selfMatch.Success)
        {
            builder.AddRecipientClause(new PlannerRecipientClause
            {
                Link = link,
                Kind = PlannerRecipientClauseKind.SelfRecipient,
                Negated = IsNotEqualsOperator(selfMatch.Groups[1].Value)
            });
            return true;
        }

        if (TryAddCompositeFriendEntity(link, builder, normalized))
        {
            return true;
        }

        return TryAddRecipientUnionWithoutConnectors(link, builder, normalized);
    }

    private static bool TryAddCompositeFriendEntity(AdvancedConditionLink link, PolicyBuilder builder, string normalized)
    {
        var friendMatch = ForwardFriendEntityRegex.Match(normalized);
        if (!friendMatch.Success)
        {
            friendMatch = ReverseFriendEntityRegex.Match(normalized);
        }

        if (friendMatch.Success)
        {
            if (TryCreateRecipientClauseFromEntityLiteral(friendMatch.Groups[1].Value, link, out var clause))
            {
                builder.AddRecipientClause(clause);
                return true;
            }

            return false;
        }

        var foeMatch = ReverseFoeEntityRegex.Match(normalized);
        if (!foeMatch.Success)
        {
            return false;
        }

        if (TryCreateRecipientClauseFromEntityLiteral(foeMatch.Groups[1].Value, link, out _))
        {
            builder.AddRecipientClause(new PlannerRecipientClause
            {
                Link = link,
                Kind = PlannerRecipientClauseKind.Never
            });
            return true;
        }

        return false;
    }

    private static bool TryAddRecipientUnionWithoutConnectors(AdvancedConditionLink link, PolicyBuilder builder, string normalized)
    {
        if (normalized.Contains("&&", StringComparison.Ordinal) ||
            normalized.Contains("||", StringComparison.Ordinal) ||
            normalized.Contains("!", StringComparison.Ordinal) ||
            normalized.Contains("target.mode?", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("target.ownPower?", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("target.VillainName>", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("target>costume", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var clauses = new List<PlannerRecipientClause>();
        if (ContainsTargetEntityPlayer(normalized))
        {
            clauses.Add(new PlannerRecipientClause
            {
                Link = link,
                Kind = PlannerRecipientClauseKind.PlayerRecipient
            });
        }

        if (ContainsOwnedPetGroup(normalized))
        {
            clauses.Add(new PlannerRecipientClause
            {
                Link = clauses.Count == 0 ? link : AdvancedConditionLink.Or,
                Kind = PlannerRecipientClauseKind.OwnedRealPetRecipient
            });
        }

        if (clauses.Count == 0)
        {
            return false;
        }

        foreach (var clause in clauses)
        {
            builder.AddRecipientClause(clause);
        }

        return true;
    }

    private static bool TryCreateRecipientClauseFromEntityLiteral(string rawValue, AdvancedConditionLink link, out PlannerRecipientClause clause)
    {
        clause = new PlannerRecipientClause();
        var normalized = NormalizeTargetLiteral(rawValue, rawValue);
        if (normalized is "player" or "pc")
        {
            clause = new PlannerRecipientClause
            {
                Link = link,
                Kind = PlannerRecipientClauseKind.PlayerRecipient
            };
            return true;
        }

        if (normalized is "critter" or "npc" or "pet" or "pets" or "henchman")
        {
            clause = new PlannerRecipientClause
            {
                Link = link,
                Kind = PlannerRecipientClauseKind.OwnedRealPetRecipient
            };
            return true;
        }

        return false;
    }

    private static bool ContainsTargetEntityPlayer(string normalized)
    {
        return normalized.Contains("target>enttype eq 'player'", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("target>enttype eq \"player\"", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("enttype target> player eq", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainsOwnedPetGroup(string normalized)
    {
        return normalized.Contains("target>group eq 'Pets'", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("target>group eq \"Pets\"", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("target>group eq 'MastermindPets'", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("target>group eq \"MastermindPets\"", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("group target> Pets eq", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("group target> MastermindPets eq", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsOwnedPetGroup(string value)
    {
        return value.Equals("Pets", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("MastermindPets", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsNotEqualsOperator(string value)
    {
        return value.Equals("!=", StringComparison.Ordinal) ||
               value.Equals("ne", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeTargetLiteral(string subject, string value)
    {
        var candidate = !string.IsNullOrWhiteSpace(subject)
            ? subject
            : value;
        return candidate
            .Trim()
            .Trim('\'', '"')
            .ToLowerInvariant();
    }

    private static string NormalizeRaw(string rawExpression)
    {
        return rawExpression
            .Trim()
            .Replace("  ", " ", StringComparison.Ordinal);
    }

    private sealed class PolicyBuilder
    {
        private readonly string _originalTargetRequires;
        private readonly AdvancedConditionSet _buildSourceGates = new();
        private readonly List<PlannerRecipientClause> _recipientClauses = [];
        private readonly AdvancedConditionSet _deferredTargetRows = new();
        private readonly AdvancedConditionSet _deferredSourceRows = new();

        public PolicyBuilder(string originalTargetRequires)
        {
            _originalTargetRequires = originalTargetRequires;
        }

        public void AddBuildSourceGate(AdvancedConditionRow row)
        {
            var clone = row.Clone();
            clone.Link = _buildSourceGates.Rows.Count == 0 ? AdvancedConditionLink.And : clone.Link;
            _buildSourceGates.Rows.Add(clone);
        }

        public void AddRecipientClause(PlannerRecipientClause clause)
        {
            var clone = clause.Clone();
            clone.Link = _recipientClauses.Count == 0 ? AdvancedConditionLink.And : clone.Link;
            _recipientClauses.Add(clone);
        }

        public void AddDeferred(AdvancedConditionRow row)
        {
            var target = ReferencesSource(row) ? _deferredSourceRows : _deferredTargetRows;
            var clone = row.Clone();
            clone.Link = target.Rows.Count == 0 ? AdvancedConditionLink.And : clone.Link;
            target.Rows.Add(clone);
        }

        public PlannerTargetRoutingPolicy Build()
        {
            var allowedRecipients = DetermineAllowedRecipients(_recipientClauses);
            var selfRequirement = DetermineSelfRequirement(_recipientClauses);
            return new PlannerTargetRoutingPolicy(
                allowedRecipients,
                selfRequirement,
                _buildSourceGates,
                _recipientClauses,
                _deferredTargetRows,
                _deferredSourceRows,
                _originalTargetRequires,
                hasImportedMetadata: true);
        }

        private static PlannerRecipientFlags DetermineAllowedRecipients(IReadOnlyList<PlannerRecipientClause> clauses)
        {
            var flags = PlannerRecipientFlags.Player | PlannerRecipientFlags.OwnedRealPet;

            foreach (var clause in clauses)
            {
                var clauseFlags = clause.Kind switch
                {
                    PlannerRecipientClauseKind.PlayerRecipient => PlannerRecipientFlags.Player,
                    PlannerRecipientClauseKind.OwnedRealPetRecipient => PlannerRecipientFlags.OwnedRealPet,
                    PlannerRecipientClauseKind.SelfRecipient when clause.Negated => PlannerRecipientFlags.OwnedRealPet,
                    PlannerRecipientClauseKind.SelfRecipient => PlannerRecipientFlags.Player,
                    PlannerRecipientClauseKind.Never => PlannerRecipientFlags.None,
                    _ => PlannerRecipientFlags.Player | PlannerRecipientFlags.OwnedRealPet
                };

                flags = clause.Link == AdvancedConditionLink.Or
                    ? flags | clauseFlags
                    : flags & clauseFlags;
            }

            return flags;
        }

        private static PlannerSelfRequirement DetermineSelfRequirement(IReadOnlyList<PlannerRecipientClause> clauses)
        {
            var selfClauses = clauses
                .Where(clause => clause.Kind == PlannerRecipientClauseKind.SelfRecipient)
                .ToList();
            if (selfClauses.Count == 0)
            {
                return PlannerSelfRequirement.Any;
            }

            return selfClauses.Last().Negated
                ? PlannerSelfRequirement.NonSelfOnly
                : PlannerSelfRequirement.SelfOnly;
        }

        private static bool ReferencesSource(AdvancedConditionRow row)
        {
            if (row.Kind is AdvancedConditionKind.SourceMode or AdvancedConditionKind.SourceOwnPower)
            {
                return true;
            }

            var raw = row.RawExpression ?? string.Empty;
            return raw.Contains("source.", StringComparison.OrdinalIgnoreCase) ||
                   raw.Contains("source>", StringComparison.OrdinalIgnoreCase);
        }
    }
}

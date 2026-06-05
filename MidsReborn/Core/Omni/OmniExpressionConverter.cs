using System.Text.RegularExpressions;

namespace Mids_Reborn.Core.Omni;

public static partial class OmniExpressionConverter
{
    public sealed class PowerRequirementConversionResult
    {
        public AdvancedConditionSet Requirements { get; } = new();
        public AdvancedConditionSet ReportOnlyFragments { get; } = new();
        public string RawExpression { get; init; } = string.Empty;

        public bool Success => CanUseAsPowerRequirement(Requirements);
    }

    public static AdvancedConditionSet ToConditionSet(params string?[] expressions)
    {
        return ToConditionSet(AdvancedConditionEvaluationMode.BuildEvaluated, expressions);
    }

    public static AdvancedConditionSet ToConditionSet(
        AdvancedConditionEvaluationMode fallbackMode,
        params string?[] expressions)
    {
        return ToConditionSet(fallbackMode, ownerFullName: string.Empty, expressions);
    }

    public static AdvancedConditionSet ToConditionSet(
        AdvancedConditionEvaluationMode fallbackMode,
        string ownerFullName,
        params string?[] expressions)
    {
        var set = new AdvancedConditionSet();
        foreach (var expression in expressions.Where(e => !string.IsNullOrWhiteSpace(e)))
        {
            var rows = ToRows(expression!, fallbackMode, ownerFullName);
            if (rows.Count == 0)
            {
                rows.Add(AdvancedConditionRow.AdvancedExpression(
                    AdvancedConditionLink.And,
                    expression!,
                    unsupported: true,
                    evaluationMode: fallbackMode));
            }

            foreach (var row in rows)
            {
                row.Link = set.Rows.Count == 0 ? AdvancedConditionLink.And : row.Link;
                set.Rows.Add(row);
            }
        }

        return set;
    }

    public static List<AdvancedConditionRow> ToRows(string expression)
    {
        return ToRows(expression, AdvancedConditionEvaluationMode.BuildEvaluated);
    }

    public static List<AdvancedConditionRow> ToRows(
        string expression,
        AdvancedConditionEvaluationMode fallbackMode)
    {
        return ToRows(expression, fallbackMode, ownerFullName: string.Empty);
    }

    public static List<AdvancedConditionRow> ToRows(
        string expression,
        AdvancedConditionEvaluationMode fallbackMode,
        string ownerFullName)
    {
        expression = NormalizeOuter(expression);
        var rows = new List<AdvancedConditionRow>();

        var orParts = SplitTopLevelOr(expression).ToList();
        if (orParts.Count > 1)
        {
            foreach (var part in orParts)
            {
                foreach (var row in ToRows(part.Trim(), fallbackMode, ownerFullName))
                {
                    row.Link = rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.Or;
                    rows.Add(row);
                }
            }

            return rows;
        }

        var andParts = SplitTopLevelAnd(expression).ToList();
        if (andParts.Count > 1)
        {
            foreach (var part in andParts)
            {
                foreach (var row in ToRows(part.Trim(), fallbackMode, ownerFullName))
                {
                    row.Link = rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.And;
                    rows.Add(row);
                }
            }

            return rows;
        }

        if (TryExtractMixedRows(expression.Trim(), fallbackMode, out var extractedRows))
        {
            return extractedRows;
        }

        return
        [
            ToSingleRow(
                expression.Trim(),
                AdvancedConditionLink.And,
                fallbackMode,
                ownerFullName)
        ];
    }

    public static IReadOnlyList<string> FindUnknownTokens(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return [];
        }

        var knownPatterns = new[]
        {
            OwnPowerRegex(),
            TargetOwnPowerRegex(),
            SourceModeRegex(),
            TargetModeRegex(),
            TargetEntityRegex(),
            SourceArchetypeRegex(),
            ArchetypeRpnRegex(),
            ArchetypeStringRegex(),
            CharacterLevelRegex(),
            OwnPowerNumRegex(),
            DirectPowerNameRegex()
        };

        var normalized = NormalizeOuter(expression);
        if (knownPatterns.Any(regex => regex.IsMatch(normalized)) || IsRuntimeTargetExpression(normalized) || IsReportOnlyExpression(normalized))
        {
            return [];
        }

        return Regex.Matches(expression, @"[@$]?[A-Za-z_][A-Za-z0-9_?.>:-]*")
            .Select(m => m.Value)
            .Where(IsUnknownDiagnosticToken)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(20)
            .ToList();
    }

    private static bool IsUnknownDiagnosticToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token) ||
            KnownWords.Contains(token, StringComparer.OrdinalIgnoreCase) ||
            PowerTokenRegex().IsMatch(token) ||
            DirectPowerSetRegex().IsMatch(token))
        {
            return false;
        }

        var normalized = token.Trim('\'', '"');
        return !(normalized.StartsWith("@Class_", StringComparison.OrdinalIgnoreCase) ||
                 normalized.StartsWith("Class_", StringComparison.OrdinalIgnoreCase) ||
                 normalized.StartsWith("k", StringComparison.Ordinal) ||
                 normalized.StartsWith("cur.k", StringComparison.OrdinalIgnoreCase) ||
                 normalized.StartsWith("source>", StringComparison.OrdinalIgnoreCase) ||
                 normalized.StartsWith("source.", StringComparison.OrdinalIgnoreCase) ||
                 normalized.StartsWith("target>", StringComparison.OrdinalIgnoreCase) ||
                 normalized.StartsWith("target.", StringComparison.OrdinalIgnoreCase) ||
                 normalized.StartsWith("power.base>", StringComparison.OrdinalIgnoreCase) ||
                 normalized.StartsWith("$archetype", StringComparison.OrdinalIgnoreCase) ||
                 normalized.StartsWith("$archtype", StringComparison.OrdinalIgnoreCase));
    }

    public static bool CanUseAsPowerRequirement(AdvancedConditionSet set)
    {
        return set.Rows.All(row =>
            row.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated &&
            !row.Unsupported &&
            row.Kind != AdvancedConditionKind.AdvancedExpression);
    }

    public static bool TryConvertPowerRequirement(string expression, out AdvancedConditionSet set)
    {
        var result = ConvertPowerRequirement(expression);
        set = result.Requirements;
        return result.Success;
    }

    public static PowerRequirementConversionResult ConvertPowerRequirement(string expression)
    {
        var result = new PowerRequirementConversionResult
        {
            RawExpression = expression
        };

        foreach (var row in ConvertPowerRequirementRows(expression, result.ReportOnlyFragments))
        {
            row.Link = result.Requirements.Rows.Count == 0 ? AdvancedConditionLink.And : row.Link;
            result.Requirements.Rows.Add(row);
        }

        if (result.Requirements.Rows.Count == 0 && result.ReportOnlyFragments.Rows.Count > 0)
        {
            return result;
        }

        if (!CanUseAsPowerRequirement(result.Requirements))
        {
            return result;
        }

        return result;
    }

    private static AdvancedConditionRow ToSingleRow(
        string expression,
        AdvancedConditionLink link,
        AdvancedConditionEvaluationMode fallbackMode,
        string ownerFullName = "")
    {
        var normalized = NormalizeOuter(expression);
        var negated = normalized.StartsWith("!", StringComparison.Ordinal);
        if (negated)
        {
            normalized = NormalizeOuter(normalized[1..]);
        }

        if (TryResolveBooleanLiteral(normalized, negated, out var literalValue))
        {
            return AdvancedConditionRow.BooleanLiteral(link, literalValue);
        }

        if (ArchetypeInherentCatalog.TryRewriteTierOneCondition(
                ownerFullName,
                normalized,
                negated,
                link,
                expression,
                out var tierOneRow))
        {
            return tierOneRow;
        }

        var targetOwnPower = TargetOwnPowerRegex().Match(normalized);
        if (targetOwnPower.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetEntityType,
                Subject = "ownPower",
                Value = targetOwnPower.Groups[1].Value,
                TargetScope = AdvancedConditionTargetScope.Unknown,
                Negated = negated,
                RawExpression = expression,
                Unsupported = true,
                EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
            };
        }

        var ownPower = OwnPowerRegex().Match(normalized);
        if (ownPower.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.SourceOwnPower,
                Subject = ownPower.Groups[1].Value,
                Negated = negated,
                RawExpression = expression
            };
        }

        var ownPowerNum = OwnPowerNumRegex().Match(normalized);
        if (ownPowerNum.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.PowerCount,
                Subject = ownPowerNum.Groups[1].Value.Trim(),
                Operator = ParseOperator(ownPowerNum.Groups[2].Value),
                Value = ownPowerNum.Groups[3].Value,
                Negated = negated,
                RawExpression = expression
            };
        }

        var directPower = DirectPowerNameRegex().Match(normalized);
        if (directPower.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.PowerTaken,
                Subject = directPower.Groups[1].Value,
                Value = "true",
                Negated = negated,
                RawExpression = expression
            };
        }

        var directPowerSet = DirectPowerSetRegex().Match(normalized);
        if (directPowerSet.Success && negated)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.PowerCount,
                Subject = directPowerSet.Groups[1].Value,
                Operator = AdvancedConditionOperator.Equals,
                Value = "0",
                RawExpression = expression
            };
        }

        var sourceMode = SourceModeRegex().Match(normalized);
        if (sourceMode.Success)
        {
            return BuildSourceModeRow(link, sourceMode.Groups[1].Value, negated, expression, ownerFullName);
        }

        var sourceModeRpn = SourceModeRpnRegex().Match(normalized);
        if (sourceModeRpn.Success)
        {
            return BuildSourceModeRow(link, sourceModeRpn.Groups[1].Value, negated, expression, ownerFullName);
        }

        var targetMode = TargetModeRegex().Match(normalized);
        if (targetMode.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetMode,
                Subject = OmniModeMapper.Normalize(targetMode.Groups[1].Value),
                Negated = negated,
                RawExpression = expression,
                Unsupported = true,
                EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
            };
        }

        var targetModeRpn = TargetModeRpnRegex().Match(normalized);
        if (targetModeRpn.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetMode,
                Subject = OmniModeMapper.Normalize(targetModeRpn.Groups[1].Value),
                Negated = negated,
                RawExpression = expression,
                Unsupported = true,
                EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
            };
        }

        var targetEntity = TargetEntityRegex().Match(normalized);
        if (targetEntity.Success)
        {
            var targetScope = MapTargetEntityToScope(targetEntity.Groups[2].Value);
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetEntityType,
                Subject = targetEntity.Groups[2].Value.Trim('\'', '"'),
                Value = targetScope == AdvancedConditionTargetScope.Unknown
                    ? targetEntity.Groups[2].Value.Trim('\'', '"')
                    : targetScope.ToString(),
                TargetScope = targetScope,
                Operator = targetEntity.Groups[1].Value.Equals("!=", StringComparison.Ordinal) || negated
                    ? AdvancedConditionOperator.NotEquals
                    : AdvancedConditionOperator.Equals,
                RawExpression = expression,
                Unsupported = true,
                EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
            };
        }

        var reverseTargetEntity = ReverseTargetEntityRegex().Match(normalized);
        if (reverseTargetEntity.Success)
        {
            var targetScope = MapTargetEntityToScope(reverseTargetEntity.Groups[1].Value);
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetEntityType,
                Subject = reverseTargetEntity.Groups[1].Value.Trim('\'', '"'),
                Value = targetScope == AdvancedConditionTargetScope.Unknown
                    ? reverseTargetEntity.Groups[1].Value.Trim('\'', '"')
                    : targetScope.ToString(),
                TargetScope = targetScope,
                Operator = reverseTargetEntity.Groups[2].Value.Equals("!=", StringComparison.Ordinal) ||
                           reverseTargetEntity.Groups[2].Value.Equals("ne", StringComparison.OrdinalIgnoreCase) ||
                           negated
                    ? AdvancedConditionOperator.NotEquals
                    : AdvancedConditionOperator.Equals,
                RawExpression = expression,
                Unsupported = true,
                EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
            };
        }

        var targetArchetype = TargetArchetypeRegex().Match(normalized);
        if (targetArchetype.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetArchetype,
                Subject = "arch",
                Value = targetArchetype.Groups[2].Value.Trim('\'', '"'),
                Operator = targetArchetype.Groups[1].Value.Equals("!=", StringComparison.Ordinal) ||
                           targetArchetype.Groups[1].Value.Equals("ne", StringComparison.OrdinalIgnoreCase) ||
                           negated
                    ? AdvancedConditionOperator.NotEquals
                    : AdvancedConditionOperator.Equals,
                RawExpression = expression,
                EvaluationMode = AdvancedConditionEvaluationMode.BuildEvaluated
            };
        }

        var reverseTargetArchetype = ReverseTargetArchetypeRegex().Match(normalized);
        if (reverseTargetArchetype.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetArchetype,
                Subject = "arch",
                Value = reverseTargetArchetype.Groups[1].Value.Trim('\'', '"'),
                Operator = reverseTargetArchetype.Groups[2].Value.Equals("!=", StringComparison.Ordinal) ||
                           reverseTargetArchetype.Groups[2].Value.Equals("ne", StringComparison.OrdinalIgnoreCase) ||
                           negated
                    ? AdvancedConditionOperator.NotEquals
                    : AdvancedConditionOperator.Equals,
                RawExpression = expression,
                EvaluationMode = AdvancedConditionEvaluationMode.BuildEvaluated
            };
        }

        var friend = TargetFriendRegex().Match(normalized);
        if (friend.Success)
        {
            var scope = negated ? AdvancedConditionTargetScope.Foe : AdvancedConditionTargetScope.Ally;
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetEntityType,
                Subject = "friend",
                Value = scope.ToString(),
                TargetScope = scope,
                Operator = AdvancedConditionOperator.Equals,
                RawExpression = expression,
                Unsupported = true,
                EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
            };
        }

        var targetGroup = TargetGroupRegex().Match(normalized);
        if (targetGroup.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetGroup,
                Subject = "group",
                Value = targetGroup.Groups[2].Value.Trim('\'', '"'),
                Operator = targetGroup.Groups[1].Value.Equals("!=", StringComparison.Ordinal) ||
                           targetGroup.Groups[1].Value.Equals("ne", StringComparison.OrdinalIgnoreCase) ||
                           negated
                    ? AdvancedConditionOperator.NotEquals
                    : AdvancedConditionOperator.Equals,
                RawExpression = expression,
                Unsupported = true,
                EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
            };
        }

        var reverseTargetGroup = ReverseTargetGroupRegex().Match(normalized);
        if (reverseTargetGroup.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetGroup,
                Subject = "group",
                Value = reverseTargetGroup.Groups[1].Value.Trim('\'', '"'),
                Operator = reverseTargetGroup.Groups[2].Value.Equals("!=", StringComparison.Ordinal) ||
                           reverseTargetGroup.Groups[2].Value.Equals("ne", StringComparison.OrdinalIgnoreCase) ||
                           negated
                    ? AdvancedConditionOperator.NotEquals
                    : AdvancedConditionOperator.Equals,
                RawExpression = expression,
                Unsupported = true,
                EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
            };
        }

        var infixSelf = InfixSelfTargetRegex().Match(normalized);
        if (infixSelf.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetEntityType,
                Subject = "self",
                Value = AdvancedConditionTargetScope.Self.ToString(),
                TargetScope = AdvancedConditionTargetScope.Self,
                Operator = infixSelf.Groups[1].Value.Equals("!=", StringComparison.Ordinal) ||
                           infixSelf.Groups[1].Value.Equals("ne", StringComparison.OrdinalIgnoreCase) ||
                           negated
                    ? AdvancedConditionOperator.NotEquals
                    : AdvancedConditionOperator.Equals,
                RawExpression = expression,
                Unsupported = true,
                EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
            };
        }

        var self = SelfTargetRegex().Match(normalized);
        if (self.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetEntityType,
                Subject = "self",
                Value = AdvancedConditionTargetScope.Self.ToString(),
                TargetScope = AdvancedConditionTargetScope.Self,
                Operator = negated ? AdvancedConditionOperator.NotEquals : AdvancedConditionOperator.Equals,
                RawExpression = expression,
                Unsupported = true,
                EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
            };
        }

        var sourceEntity = SourceEntityRegex().Match(normalized);
        if (sourceEntity.Success && IsPlayerLiteral(sourceEntity.Groups[2].Value))
        {
            return AdvancedConditionRow.AdvancedExpression(
                link,
                expression,
                unsupported: true,
                evaluationMode: AdvancedConditionEvaluationMode.ReportOnly);
        }

        var sourceArch = SourceArchetypeRegex().Match(normalized);
        if (sourceArch.Success)
        {
            return CreateSourceArchetypeRow(
                link,
                sourceArch.Groups[1].Value,
                sourceArch.Groups[2].Value,
                negated,
                expression);
        }

        var reverseSourceArch = ReverseSourceArchetypeRegex().Match(normalized);
        if (reverseSourceArch.Success)
        {
            return CreateSourceArchetypeRow(
                link,
                reverseSourceArch.Groups[2].Value,
                reverseSourceArch.Groups[1].Value,
                negated,
                expression);
        }

        var archetypeRpn = ArchetypeRpnRegex().Match(normalized);
        if (archetypeRpn.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.CharacterArchetype,
                Subject = "class",
                Value = CleanArchetypeName(archetypeRpn.Groups[1].Value),
                Operator = ParseTextComparison(archetypeRpn.Groups[2].Value, negated),
                RawExpression = expression
            };
        }

        var archetypeString = ArchetypeStringRegex().Match(normalized);
        if (archetypeString.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.CharacterArchetype,
                Subject = "class",
                Value = CleanArchetypeName(archetypeString.Groups[2].Value),
                Operator = ParseTextComparison(archetypeString.Groups[1].Value, negated),
                RawExpression = expression
            };
        }

        var level = CharacterLevelRegex().Match(normalized);
        if (level.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.CharacterLevel,
                Subject = "Level",
                Operator = ParseOperator(level.Groups[1].Value),
                Value = level.Groups[2].Value,
                Negated = negated,
                RawExpression = expression
            };
        }

        if (IsReportOnlyExpression(normalized))
        {
            return AdvancedConditionRow.AdvancedExpression(
                link,
                expression,
                unsupported: true,
                evaluationMode: AdvancedConditionEvaluationMode.ReportOnly);
        }

        if (IsRuntimeTargetExpression(normalized))
        {
            return AdvancedConditionRow.AdvancedExpression(
                link,
                expression,
                unsupported: true,
                evaluationMode: AdvancedConditionEvaluationMode.RuntimeTargetOnly);
        }

        return AdvancedConditionRow.AdvancedExpression(
            link,
            expression,
            unsupported: true,
                evaluationMode: fallbackMode);
    }

    private static AdvancedConditionRow BuildSourceModeRow(
        AdvancedConditionLink link,
        string rawMode,
        bool negated,
        string expression,
        string ownerFullName)
    {
        var mode = OmniModeMapper.Normalize(rawMode);
        if (mode.Equals("Engaged", StringComparison.OrdinalIgnoreCase) &&
            OmniModeMapper.IsSnipePlannerContext(ownerFullName))
        {
            mode = "FastSnipe";
        }

        if (!OmniModeMapper.IsKnownBuildSourceMode(mode))
        {
            return AdvancedConditionRow.AdvancedExpression(
                link,
                $"source.Mode?({mode})",
                unsupported: true,
                evaluationMode: AdvancedConditionEvaluationMode.RuntimeTargetOnly);
        }

        return new AdvancedConditionRow
        {
            Link = link,
            Kind = AdvancedConditionKind.SourceMode,
            Subject = mode,
            Negated = negated,
            RawExpression = expression
        };
    }

    private static bool TryExtractMixedRows(
        string expression,
        AdvancedConditionEvaluationMode fallbackMode,
        out List<AdvancedConditionRow> rows)
    {
        rows = [];
        var normalized = NormalizeOuter(expression);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return false;
        }

        AddEmbeddedSourceArchetypeRows(normalized, rows);
        if (rows.Count == 0)
        {
            return false;
        }

        if (fallbackMode is AdvancedConditionEvaluationMode.ReportOnly or AdvancedConditionEvaluationMode.RuntimeTargetOnly)
        {
            rows.Add(AdvancedConditionRow.AdvancedExpression(
                rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.And,
                expression,
                unsupported: true,
                evaluationMode: fallbackMode));
        }

        return true;
    }

    private static void AddEmbeddedSourceArchetypeRows(string expression, ICollection<AdvancedConditionRow> rows)
    {
        foreach (Match match in EmbeddedSourceArchetypeRegex().Matches(expression))
        {
            AddDistinctRow(
                rows,
                CreateSourceArchetypeRow(
                    AdvancedConditionLink.And,
                    match.Groups[1].Value,
                    match.Groups[2].Value,
                    negated: false,
                    match.Value));
        }

        foreach (Match match in EmbeddedReverseSourceArchetypeRegex().Matches(expression))
        {
            AddDistinctRow(
                rows,
                CreateSourceArchetypeRow(
                    AdvancedConditionLink.And,
                    match.Groups[2].Value,
                    match.Groups[1].Value,
                    negated: false,
                    match.Value));
        }
    }

    private static void AddDistinctRow(ICollection<AdvancedConditionRow> rows, AdvancedConditionRow candidate)
    {
        if (rows.Any(existing =>
                existing.Kind == candidate.Kind &&
                existing.Operator == candidate.Operator &&
                string.Equals(existing.Subject, candidate.Subject, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(existing.Value, candidate.Value, StringComparison.OrdinalIgnoreCase) &&
                existing.Negated == candidate.Negated))
        {
            return;
        }

        candidate.Link = rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.And;
        rows.Add(candidate);
    }

    private static AdvancedConditionRow CreateSourceArchetypeRow(
        AdvancedConditionLink link,
        string comparison,
        string archetype,
        bool negated,
        string rawExpression)
    {
        return new AdvancedConditionRow
        {
            Link = link,
            Kind = AdvancedConditionKind.CharacterArchetype,
            Subject = "class",
            Value = CleanArchetypeName(archetype),
            Operator = ParseTextComparison(comparison, negated),
            RawExpression = rawExpression
        };
    }

    private static AdvancedConditionOperator ParseOperator(string op)
    {
        return op switch
        {
            "!=" or "<>" => AdvancedConditionOperator.NotEquals,
            ">" => AdvancedConditionOperator.GreaterThan,
            "<" => AdvancedConditionOperator.LessThan,
            ">=" => AdvancedConditionOperator.GreaterThanOrEqual,
            "<=" => AdvancedConditionOperator.LessThanOrEqual,
            _ => AdvancedConditionOperator.Equals
        };
    }

    private static AdvancedConditionOperator ParseTextComparison(string op, bool negated)
    {
        var notEquals = op.Equals("!=", StringComparison.Ordinal) ||
                        op.Equals("<>", StringComparison.Ordinal) ||
                        op.Equals("ne", StringComparison.OrdinalIgnoreCase);
        return notEquals ^ negated
            ? AdvancedConditionOperator.NotEquals
            : AdvancedConditionOperator.Equals;
    }

    private static string CleanArchetypeName(string value)
    {
        return value.Trim().Trim('\'', '"').TrimStart('@');
    }

    private static List<AdvancedConditionRow> ConvertPowerRequirementRows(
        string expression,
        AdvancedConditionSet reportOnlyFragments)
    {
        var normalized = NormalizeOuter(expression);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return [];
        }

        if (IsIgnorablePowerRequirementFragment(normalized))
        {
            AddReportOnlyFragment(reportOnlyFragments, expression);
            return [];
        }

        if (TryConvertPoolRequirementExpression(normalized, reportOnlyFragments, out var poolRows))
        {
            return poolRows;
        }

        var andParts = SplitTopLevelAnd(normalized).ToList();
        if (andParts.Count > 1)
        {
            var rows = new List<AdvancedConditionRow>();
            foreach (var part in andParts)
            {
                foreach (var convertedRow in ConvertPowerRequirementRows(part, reportOnlyFragments))
                {
                    convertedRow.Link = rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.And;
                    rows.Add(convertedRow);
                }
            }

            return rows;
        }

        if (TryConvertArchetypeRpnChain(normalized, out var archetypeRows))
        {
            return archetypeRows;
        }

        if (TryConvertArchetypePatronChain(normalized, reportOnlyFragments, out var patronRows))
        {
            return patronRows;
        }

        if (TryConvertPowerOrMath(normalized, out var powerOrRows))
        {
            return powerOrRows;
        }

        if (TryConvertPowerSetNotList(normalized, out var powerSetRows))
        {
            return powerSetRows;
        }

        var row = ToSingleRow(normalized, AdvancedConditionLink.And, AdvancedConditionEvaluationMode.BuildEvaluated);
        if (row.EvaluationMode == AdvancedConditionEvaluationMode.ReportOnly && IsIgnorablePowerRequirementFragment(normalized))
        {
            AddReportOnlyFragment(reportOnlyFragments, expression);
            return [];
        }

        if (row.Kind == AdvancedConditionKind.PowerTaken &&
            row.Operator == AdvancedConditionOperator.Equals &&
            string.Equals(row.Value, "true", StringComparison.OrdinalIgnoreCase))
        {
            row = CreateRequirementGroupRow(
                row.Subject,
                string.Empty,
                row.Link,
                row.Negated,
                row.RawExpression);
        }

        return [row];
    }

    private static bool TryConvertPoolRequirementExpression(
        string expression,
        AdvancedConditionSet reportOnlyFragments,
        out List<AdvancedConditionRow> rows)
    {
        rows = [];
        var working = StripIgnorableRpnFragments(expression, reportOnlyFragments);
        if (string.IsNullOrWhiteSpace(working) || !PowerTokenRegex().IsMatch(working))
        {
            return false;
        }

        if (TryConvertPowerCountThreshold(working, out rows))
        {
            return true;
        }

        if (TryConvertPoolRpnRequirements(working, out rows))
        {
            return true;
        }

        return false;
    }

    private static string StripIgnorableRpnFragments(string expression, AdvancedConditionSet reportOnlyFragments)
    {
        var working = expression;
        var changed = false;

        working = Regex.Replace(
            working,
            @"(?:^|\s)(?:accesslevel\s+char>|char>accesslevel)\s+\d+\s*(?:>=|>)\s*(?:&&)?(?=\s|$)",
            match =>
            {
                changed = true;
                return " ";
            },
            RegexOptions.IgnoreCase);

        if (changed)
        {
            AddReportOnlyFragment(reportOnlyFragments, expression);
        }

        return Regex.Replace(working, @"\s+", " ").Trim();
    }

    private static bool TryConvertPowerCountThreshold(
        string expression,
        out List<AdvancedConditionRow> rows)
    {
        rows = [];
        var match = PowerCountThresholdRegex().Match(expression);
        if (!match.Success)
        {
            return false;
        }

        var powers = PowerTokenRegex()
            .Matches(match.Groups[1].Value)
            .Select(m => m.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (powers.Count == 0 || !int.TryParse(match.Groups[2].Value, out var threshold))
        {
            return false;
        }

        var requiredCount = threshold + 1;
        if (requiredCount <= 1)
        {
            rows.AddRange(CreateSinglePowerRequirementRows(powers, expression));
            return true;
        }

        if (requiredCount != 2)
        {
            return false;
        }

        for (var i = 0; i < powers.Count; i++)
        {
            for (var j = i + 1; j < powers.Count; j++)
            {
                rows.Add(CreateRequirementGroupRow(
                    powers[i],
                    powers[j],
                    rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.Or,
                    negated: false,
                    expression));
            }
        }

        return rows.Count > 0;
    }

    private static bool TryConvertPoolRpnRequirements(
        string expression,
        out List<AdvancedConditionRow> rows)
    {
        rows = [];
        var tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            return false;
        }

        if (tokens.All(IsPowerToken))
        {
            rows.AddRange(CreateSinglePowerRequirementRows(tokens, expression));
            return rows.Count > 0;
        }

        var consumedAny = false;
        for (var i = 0; i < tokens.Length;)
        {
            if (tokens[i].Equals("&&", StringComparison.Ordinal))
            {
                i++;
                continue;
            }

            if (!IsPowerToken(tokens[i]))
            {
                return false;
            }

            if (i + 1 < tokens.Length && tokens[i + 1].Equals("!", StringComparison.Ordinal))
            {
                rows.Add(CreateRequirementGroupRow(
                    tokens[i],
                    string.Empty,
                    AdvancedConditionLink.And,
                    negated: true,
                    expression));
                consumedAny = true;
                i += 2;
                continue;
            }

            if (i + 2 < tokens.Length && IsPowerToken(tokens[i + 1]) && tokens[i + 2].Equals("&&", StringComparison.Ordinal))
            {
                rows.Add(CreateRequirementGroupRow(
                    tokens[i],
                    tokens[i + 1],
                    rows.Any(r => !r.Negated) ? AdvancedConditionLink.Or : AdvancedConditionLink.And,
                    negated: false,
                    expression));
                consumedAny = true;
                i += 3;
                continue;
            }

            return false;
        }

        return consumedAny && rows.Count > 0;
    }

    private static IEnumerable<AdvancedConditionRow> CreateSinglePowerRequirementRows(
        IEnumerable<string> powers,
        string expression)
    {
        var rows = new List<AdvancedConditionRow>();
        foreach (var power in powers)
        {
            rows.Add(CreateRequirementGroupRow(
                power,
                string.Empty,
                rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.Or,
                negated: false,
                expression));
        }

        return rows;
    }

    private static AdvancedConditionRow CreateRequirementGroupRow(
        string subject,
        string value,
        AdvancedConditionLink link,
        bool negated,
        string expression)
    {
        return new AdvancedConditionRow
        {
            Link = link,
            Kind = AdvancedConditionKind.PowerRequirementGroup,
            Subject = subject,
            Value = value,
            Operator = AdvancedConditionOperator.Equals,
            Negated = negated,
            RawExpression = expression
        };
    }

    private static bool TryResolveBooleanLiteral(string normalizedExpression, bool negated, out bool value)
    {
        value = false;
        if (string.IsNullOrWhiteSpace(normalizedExpression))
        {
            return false;
        }

        var normalized = NormalizeOuter(normalizedExpression);
        if (normalized.Equals("1", StringComparison.Ordinal) ||
            normalized.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            value = !negated;
            return true;
        }

        if (normalized.Equals("0", StringComparison.Ordinal) ||
            normalized.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            value = negated;
            return true;
        }

        return false;
    }

    private static bool IsPowerToken(string token)
    {
        return DirectPowerNameRegex().IsMatch(token);
    }

    private static void AddReportOnlyFragment(AdvancedConditionSet reportOnlyFragments, string expression)
    {
        reportOnlyFragments.Rows.Add(AdvancedConditionRow.AdvancedExpression(
            reportOnlyFragments.Rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.And,
            expression,
            unsupported: true,
            evaluationMode: AdvancedConditionEvaluationMode.ReportOnly));
    }

    private static bool TryConvertArchetypeRpnChain(
        string expression,
        out List<AdvancedConditionRow> rows)
    {
        rows = [];
        var matches = Regex.Matches(expression, @"\$?archetype\s+@?(Class_[A-Za-z0-9_]+)\s+(?:==|eq)", RegexOptions.IgnoreCase);
        if (matches.Count == 0)
        {
            return false;
        }

        var remainder = Regex.Replace(expression, @"\$?archetype\s+@?Class_[A-Za-z0-9_]+\s+(?:==|eq)", string.Empty, RegexOptions.IgnoreCase).Trim();
        if (!string.IsNullOrWhiteSpace(remainder))
        {
            return false;
        }

        foreach (Match match in matches)
        {
            rows.Add(new AdvancedConditionRow
            {
                Link = rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.Or,
                Kind = AdvancedConditionKind.CharacterArchetype,
                Subject = "class",
                Value = CleanArchetypeName(match.Groups[1].Value),
                Operator = AdvancedConditionOperator.Equals,
                RawExpression = expression
            });
        }

        return rows.Count > 0;
    }

    private static bool TryConvertArchetypePatronChain(
        string expression,
        AdvancedConditionSet reportOnlyFragments,
        out List<AdvancedConditionRow> rows)
    {
        rows = [];
        if (!expression.Contains("Owned?", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var withoutOwned = Regex.Replace(expression, @"[A-Za-z0-9_]+Patron\s+Owned\?", string.Empty, RegexOptions.IgnoreCase);
        withoutOwned = Regex.Replace(withoutOwned, @"Beta_AutoLevel50\s+Owned\?", string.Empty, RegexOptions.IgnoreCase);
        withoutOwned = Regex.Replace(withoutOwned, @"\s*&&\s*$", string.Empty, RegexOptions.IgnoreCase).Trim();
        withoutOwned = Regex.Replace(withoutOwned, @"\s+", " ").Trim();

        if (!TryConvertArchetypeRpnChain(withoutOwned, out rows))
        {
            return false;
        }

        AddReportOnlyFragment(reportOnlyFragments, expression);
        return true;
    }

    private static bool TryConvertPowerOrMath(
        string expression,
        out List<AdvancedConditionRow> rows)
    {
        rows = [];
        var match = PowerOrMathRegex().Match(expression);
        if (!match.Success)
        {
            return false;
        }

        var powers = new[] { match.Groups[1].Value, match.Groups[2].Value };
        foreach (var power in powers)
        {
            rows.Add(CreateRequirementGroupRow(
                power,
                string.Empty,
                rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.Or,
                negated: false,
                expression));
        }

        return true;
    }

    private static bool TryConvertPowerSetNotList(
        string expression,
        out List<AdvancedConditionRow> rows)
    {
        rows = [];
        var normalized = expression.Trim();
        if (!normalized.EndsWith("!", StringComparison.Ordinal))
        {
            return false;
        }

        var body = normalized[..^1].Trim();
        var tokens = body.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0 || tokens.Any(token => !DirectPowerSetRegex().IsMatch(token) && !DirectPowerNameRegex().IsMatch(token)))
        {
            return false;
        }

        foreach (var token in tokens)
        {
            if (DirectPowerNameRegex().IsMatch(token))
            {
                rows.Add(CreateRequirementGroupRow(
                    token,
                    string.Empty,
                    AdvancedConditionLink.And,
                    negated: true,
                    expression));
                continue;
            }

            rows.Add(new AdvancedConditionRow
            {
                Link = rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.And,
                Kind = AdvancedConditionKind.PowerCount,
                Subject = token,
                Operator = AdvancedConditionOperator.Equals,
                Value = "0",
                RawExpression = expression
            });
        }

        return true;
    }

    private static bool IsIgnorablePowerRequirementFragment(string expression)
    {
        var normalized = NormalizeOuter(expression);
        return AccessLevelRequirementRegex().IsMatch(normalized) ||
               ReverseAccessLevelRequirementRegex().IsMatch(normalized) ||
               OwnedRequirementRegex().IsMatch(normalized) ||
               normalized.Contains("auth>", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("productOwned?", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("tokenOwned?", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("Preorder:", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("Class_Hyper-Advanced_Clockwork", StringComparison.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> SplitTopLevelAnd(string expression)
    {
        var parts = SplitTopLevelOperator(expression, "&&");
        return parts.Count == 0 ? [expression] : parts.Where(p => !string.IsNullOrWhiteSpace(p));
    }

    private static IEnumerable<string> SplitTopLevelOr(string expression)
    {
        var parts = SplitTopLevelOperator(expression, "||");
        return parts.Count == 0 ? [expression] : parts.Where(p => !string.IsNullOrWhiteSpace(p));
    }

    private static List<string> SplitTopLevelOperator(string expression, string op)
    {
        var parts = new List<string>();
        var depth = 0;
        var start = 0;
        for (var i = 0; i < expression.Length; i++)
        {
            depth += expression[i] switch
            {
                '(' => 1,
                ')' => -1,
                _ => 0
            };

            if (depth != 0 ||
                i + op.Length > expression.Length ||
                !string.Equals(expression.Substring(i, op.Length), op, StringComparison.Ordinal))
            {
                continue;
            }

            parts.Add(expression[start..i].Trim());
            i += op.Length - 1;
            start = i + 1;
        }

        parts.Add(expression[start..].Trim());
        return parts;
    }

    private static bool IsPlayerLiteral(string value)
    {
        return string.Equals(value.Trim('\'', '"'), "player", StringComparison.OrdinalIgnoreCase);
    }

    private static AdvancedConditionTargetScope MapTargetEntityToScope(string value)
    {
        var normalized = value.Trim('\'', '"').ToLowerInvariant();
        return normalized switch
        {
            "player" or "pc" => AdvancedConditionTargetScope.Player,
            "critter" or "npc" => AdvancedConditionTargetScope.Foe,
            "pet" or "pets" or "henchman" => AdvancedConditionTargetScope.Pet,
            "ally" or "friend" or "teammate" => AdvancedConditionTargetScope.Ally,
            "foe" or "enemy" => AdvancedConditionTargetScope.Foe,
            "self" => AdvancedConditionTargetScope.Self,
            _ => AdvancedConditionTargetScope.Unknown
        };
    }

    private static bool IsRuntimeTargetExpression(string expression)
    {
        return RuntimeTargetTokens.Any(token => expression.Contains(token, StringComparison.OrdinalIgnoreCase)) ||
               expression.StartsWith("target>", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains(" target>", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("target.", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("@ToHit", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("@ForceHit", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("distance", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsReportOnlyExpression(string expression)
    {
        return expression.Contains("server_tray_requires", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("confirm_requires", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("highlight_expression", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("power.base>", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("source>enttype", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("source>entref", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("source.EventTimeSince>", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("isPVPMap?", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("char>accesslevel", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("accesslevel char>", StringComparison.OrdinalIgnoreCase) ||
               expression.Contains("Owned?", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeOuter(string expression)
    {
        expression = expression.Trim();
        while (expression.StartsWith("(") && expression.EndsWith(")") && HasBalancedOuterParens(expression))
        {
            expression = expression[1..^1].Trim();
        }

        return expression;
    }

    private static bool HasBalancedOuterParens(string expression)
    {
        var depth = 0;
        for (var i = 0; i < expression.Length; i++)
        {
            depth += expression[i] switch
            {
                '(' => 1,
                ')' => -1,
                _ => 0
            };

            if (depth == 0 && i < expression.Length - 1)
            {
                return false;
            }
        }

        return depth == 0;
    }

    private static readonly string[] KnownWords =
    [
        "eq", "ne", "lt", "gt", "and", "or", "target", "source", "char", "level",
        "combatlevel", "enttype", "arch", "ownPower", "ownPowerNum", "Mode", "true", "false",
        "archetype", "archtype", "accesslevel", "Owned", "auth", "productOwned", "tokenOwned",
        "player", "critter", "friend", "distance"
    ];

    private static readonly string[] RuntimeTargetTokens =
    [
        "target.isFriend?",
        "target.HasTag?",
        "target.EventTimeSince>",
        "target.ownPower?",
        "target>cur.",
        "target>Cur.",
        "target>k",
        "target>arch",
        "target>entref",
        "enttype target>",
        "target>enttype",
        "arch target>"
    ];

    [GeneratedRegex(@"^(?:source\.)?ownPower\?\(([^)]+)\)$", RegexOptions.IgnoreCase)]
    private static partial Regex OwnPowerRegex();

    [GeneratedRegex(@"^target\.ownPower\?\(([^)]+)\)$", RegexOptions.IgnoreCase)]
    private static partial Regex TargetOwnPowerRegex();

    [GeneratedRegex(@"^(?:source\.)?Mode\?\(([^)]+)\)$", RegexOptions.IgnoreCase)]
    private static partial Regex SourceModeRegex();

    [GeneratedRegex(@"^([^\s()]+)\s+(?:source\.)?Mode\?$", RegexOptions.IgnoreCase)]
    private static partial Regex SourceModeRpnRegex();

    [GeneratedRegex(@"^target\.Mode\?\(([^)]+)\)$", RegexOptions.IgnoreCase)]
    private static partial Regex TargetModeRegex();

    [GeneratedRegex(@"^([^\s()]+)\s+target\.Mode\?$", RegexOptions.IgnoreCase)]
    private static partial Regex TargetModeRpnRegex();

    [GeneratedRegex(@"^target>enttype\s+(eq|==|!=|ne)\s+(.+)$", RegexOptions.IgnoreCase)]
    private static partial Regex TargetEntityRegex();

    [GeneratedRegex(@"^enttype\s+target>\s+(.+?)\s+(eq|==|!=|ne)$", RegexOptions.IgnoreCase)]
    private static partial Regex ReverseTargetEntityRegex();

    [GeneratedRegex(@"^target>arch\s+(eq|==|!=|ne)\s+['""]?@?(.+?)['""]?$", RegexOptions.IgnoreCase)]
    private static partial Regex TargetArchetypeRegex();

    [GeneratedRegex(@"^arch\s+target>\s+@?(.+?)\s+(eq|==|!=|ne)$", RegexOptions.IgnoreCase)]
    private static partial Regex ReverseTargetArchetypeRegex();

    [GeneratedRegex(@"^target\.isFriend\?$", RegexOptions.IgnoreCase)]
    private static partial Regex TargetFriendRegex();

    [GeneratedRegex(@"^target>group\s+(eq|==|!=|ne)\s+(.+)$", RegexOptions.IgnoreCase)]
    private static partial Regex TargetGroupRegex();

    [GeneratedRegex(@"^group\s+target>\s+(.+?)\s+(eq|==|!=|ne)$", RegexOptions.IgnoreCase)]
    private static partial Regex ReverseTargetGroupRegex();

    [GeneratedRegex(@"^target>entref\s+(eq|==|!=|ne)\s+source>entref$", RegexOptions.IgnoreCase)]
    private static partial Regex InfixSelfTargetRegex();

    [GeneratedRegex(@"^entref\s+target>\s+entref\s+source>\s+(eq|==|!=|ne)$", RegexOptions.IgnoreCase)]
    private static partial Regex SelfTargetRegex();

    [GeneratedRegex(@"^source>enttype\s+(eq|==|!=|ne)\s+(.+)$", RegexOptions.IgnoreCase)]
    private static partial Regex SourceEntityRegex();

    [GeneratedRegex(@"^(?:source(?:\.owner)?>arch|source\.owner>arch)\s+(eq|==|!=|ne)\s+['""]?@?(.+?)['""]?$", RegexOptions.IgnoreCase)]
    private static partial Regex SourceArchetypeRegex();

    [GeneratedRegex(@"^arch\s+source>\s+['""]?@?(.+?)['""]?\s+(eq|==|!=|ne)$", RegexOptions.IgnoreCase)]
    private static partial Regex ReverseSourceArchetypeRegex();

    [GeneratedRegex(@"(?:source(?:\.owner)?>arch|source\.owner>arch)\s+(eq|==|!=|ne)\s+['""]?@?([A-Za-z0-9_]+)['""]?", RegexOptions.IgnoreCase)]
    private static partial Regex EmbeddedSourceArchetypeRegex();

    [GeneratedRegex(@"arch\s+source>\s+['""]?@?([A-Za-z0-9_]+)['""]?\s+(eq|==|!=|ne)", RegexOptions.IgnoreCase)]
    private static partial Regex EmbeddedReverseSourceArchetypeRegex();

    [GeneratedRegex(@"^\$?archetype\s+@?(Class_[A-Za-z0-9_]+)\s+(eq|==|!=|ne)$", RegexOptions.IgnoreCase)]
    private static partial Regex ArchetypeRpnRegex();

    [GeneratedRegex(@"^['""]?\$?(?:archetype|archtype)['""]?\s+(eq|==|!=|ne)\s+@?(Class_[A-Za-z0-9_]+)$", RegexOptions.IgnoreCase)]
    private static partial Regex ArchetypeStringRegex();

    [GeneratedRegex(@"^(?:char>level|level char>)\s*(>=|<=|>|<|==|eq)\s*(\d+)$", RegexOptions.IgnoreCase)]
    private static partial Regex CharacterLevelRegex();

    [GeneratedRegex(@"^(?:source\.)?ownPowerNum\?\(([^)]+)\)\s*(>=|<=|>|<|==|eq|!=|ne)\s*(\d+)$", RegexOptions.IgnoreCase)]
    private static partial Regex OwnPowerNumRegex();

    [GeneratedRegex(@"^([A-Za-z0-9_]+\.[A-Za-z0-9_]+\.[A-Za-z0-9_]+)$", RegexOptions.IgnoreCase)]
    private static partial Regex DirectPowerNameRegex();

    [GeneratedRegex(@"^([A-Za-z0-9_]+\.[A-Za-z0-9_]+)$", RegexOptions.IgnoreCase)]
    private static partial Regex DirectPowerSetRegex();

    [GeneratedRegex(@"[A-Za-z0-9_]+\.[A-Za-z0-9_]+\.[A-Za-z0-9_]+", RegexOptions.IgnoreCase)]
    private static partial Regex PowerTokenRegex();

    [GeneratedRegex(@"^([A-Za-z0-9_]+\.[A-Za-z0-9_]+\.[A-Za-z0-9_]+)\s+\+\s+([A-Za-z0-9_]+\.[A-Za-z0-9_]+\.[A-Za-z0-9_]+)\s*>\s*0$", RegexOptions.IgnoreCase)]
    private static partial Regex PowerOrMathRegex();

    [GeneratedRegex(@"^((?:[A-Za-z0-9_]+\.[A-Za-z0-9_]+\.[A-Za-z0-9_]+(?:\s*\+\s*|))+[A-Za-z0-9_]+\.[A-Za-z0-9_]+\.[A-Za-z0-9_]+)\s*>\s*(\d+)$", RegexOptions.IgnoreCase)]
    private static partial Regex PowerCountThresholdRegex();

    [GeneratedRegex(@"^char>accesslevel\s*(?:>=|>|<=|<|==|eq|!=|ne)\s*\d+$", RegexOptions.IgnoreCase)]
    private static partial Regex AccessLevelRequirementRegex();

    [GeneratedRegex(@"^accesslevel\s+char>\s+\d+\s*(?:>=|>|<=|<|==|eq|!=|ne)$", RegexOptions.IgnoreCase)]
    private static partial Regex ReverseAccessLevelRequirementRegex();

    [GeneratedRegex(@"^[A-Za-z0-9_]+\s+Owned\?$", RegexOptions.IgnoreCase)]
    private static partial Regex OwnedRequirementRegex();
}

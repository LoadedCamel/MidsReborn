using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

public enum AdvancedConditionLink
{
    And,
    Or
}

public enum AdvancedConditionKind
{
    PowerActive,
    PowerTaken,
    PowerStacks,
    TeamMembers,
    CombatSetting,
    SourceOwnPower,
    SourceMode,
    TargetEntityType,
    TargetMode,
    TargetGroup,
    TargetArchetype,
    CharacterArchetype,
    CharacterLevel,
    PowerRequirementGroup,
    BoostsSlotted,
    AdvancedExpression,
    PowerCount
}

public enum AdvancedConditionOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual
}

public enum AdvancedConditionEvaluationMode
{
    BuildEvaluated,
    RuntimeTargetOnly,
    ReportOnly
}

public enum AdvancedConditionTargetScope
{
    Unknown,
    Self,
    Pet,
    Player,
    Ally,
    Foe
}

public sealed class AdvancedConditionSet
{
    private const int SerializationVersion = 3;

    public List<AdvancedConditionRow> Rows { get; set; } = [];

    public bool Any() => Rows.Count > 0;

    public AdvancedConditionSet Clone()
    {
        return new AdvancedConditionSet
        {
            Rows = Rows.Select(r => r.Clone()).ToList()
        };
    }

    public static AdvancedConditionSet FromLegacyActiveConditionals(List<KeyValue<string, string>>? conditionals)
    {
        var set = new AdvancedConditionSet();
        if (conditionals is not { Count: > 0 })
        {
            return set;
        }

        foreach (var conditional in conditionals)
        {
            var key = conditional.Key ?? string.Empty;
            var link = key.StartsWith("OR ", StringComparison.OrdinalIgnoreCase)
                ? AdvancedConditionLink.Or
                : AdvancedConditionLink.And;

            key = key.Replace("AND ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("OR ", "", StringComparison.OrdinalIgnoreCase);

            var splitAt = key.IndexOf(':');
            if (splitAt < 0)
            {
                set.Rows.Add(AdvancedConditionRow.AdvancedExpression(link, key, unsupported: true));
                continue;
            }

            var conditionType = key[..splitAt];
            var subject = key[(splitAt + 1)..];
            var valueParts = (conditional.Value ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var row = conditionType switch
            {
                "Active" => new AdvancedConditionRow
                {
                    Link = link,
                    Kind = AdvancedConditionKind.PowerActive,
                    Subject = subject,
                    Value = conditional.Value,
                    Operator = AdvancedConditionOperator.Equals
                },
                "Taken" => new AdvancedConditionRow
                {
                    Link = link,
                    Kind = AdvancedConditionKind.PowerTaken,
                    Subject = subject,
                    Value = conditional.Value,
                    Operator = AdvancedConditionOperator.Equals
                },
                "Stacks" => new AdvancedConditionRow
                {
                    Link = link,
                    Kind = AdvancedConditionKind.PowerStacks,
                    Subject = subject,
                    Value = valueParts.Length > 1 ? valueParts[1] : "0",
                    Operator = ParseOperator(valueParts.Length > 0 ? valueParts[0] : "=")
                },
                "Team" => new AdvancedConditionRow
                {
                    Link = link,
                    Kind = AdvancedConditionKind.TeamMembers,
                    Subject = subject,
                    Value = valueParts.Length > 1 ? valueParts[1] : "0",
                    Operator = ParseOperator(valueParts.Length > 0 ? valueParts[0] : "=")
                },
                "Config" => new AdvancedConditionRow
                {
                    Link = link,
                    Kind = AdvancedConditionKind.CombatSetting,
                    Subject = subject,
                    Value = valueParts.Length > 1 ? valueParts[1] : conditional.Value,
                    Operator = ParseOperator(valueParts.Length > 0 ? valueParts[0] : "=")
                },
                _ => AdvancedConditionRow.AdvancedExpression(link, $"{conditionType}:{subject} {conditional.Value}", unsupported: true)
            };

            set.Rows.Add(row);
        }

        return set;
    }

    public List<KeyValue<string, string>> ToLegacyActiveConditionals()
    {
        var conditionals = new List<KeyValue<string, string>>();
        foreach (var row in Rows)
        {
            var prefix = conditionals.Count > 0 && row.Link == AdvancedConditionLink.Or ? "OR " : string.Empty;
            switch (row.Kind)
            {
                case AdvancedConditionKind.PowerActive:
                    conditionals.Add(new KeyValue<string, string>($"{prefix}Active:{row.Subject}", row.Value));
                    break;
                case AdvancedConditionKind.PowerTaken:
                    conditionals.Add(new KeyValue<string, string>($"{prefix}Taken:{row.Subject}", row.Value));
                    break;
                case AdvancedConditionKind.PowerStacks:
                    conditionals.Add(new KeyValue<string, string>($"{prefix}Stacks:{row.Subject}", $"{FormatOperator(row.Operator)} {row.Value}"));
                    break;
                case AdvancedConditionKind.TeamMembers:
                    conditionals.Add(new KeyValue<string, string>($"{prefix}Team:{row.Subject}", $"{FormatOperator(row.Operator)} {row.Value}"));
                    break;
                case AdvancedConditionKind.CombatSetting:
                    conditionals.Add(new KeyValue<string, string>($"{prefix}Config:{row.Subject}", $"{FormatOperator(row.Operator)} {row.Value}"));
                    break;
            }
        }

        return conditionals;
    }

    public static AdvancedConditionSet FromLegacyRequirement(Requirement? requirement)
    {
        var set = new AdvancedConditionSet();
        if (requirement == null)
        {
            return set;
        }

        foreach (var className in requirement.ClassName.Where(IsMeaningfulValue))
        {
            set.Rows.Add(new AdvancedConditionRow
            {
                Link = AdvancedConditionLink.And,
                Kind = AdvancedConditionKind.CharacterArchetype,
                Subject = "class",
                Value = className,
                Operator = AdvancedConditionOperator.Equals
            });
        }

        foreach (var className in requirement.ClassNameNot.Where(IsMeaningfulValue))
        {
            set.Rows.Add(new AdvancedConditionRow
            {
                Link = AdvancedConditionLink.And,
                Kind = AdvancedConditionKind.CharacterArchetype,
                Subject = "class",
                Value = className,
                Operator = AdvancedConditionOperator.Equals,
                Negated = true
            });
        }

        foreach (var powerGroup in requirement.PowerID.Where(IsMeaningfulPowerGroup))
        {
            set.Rows.Add(new AdvancedConditionRow
            {
                Link = AdvancedConditionLink.Or,
                Kind = AdvancedConditionKind.PowerRequirementGroup,
                Subject = NormalizePowerRequirement(powerGroup.ElementAtOrDefault(0)),
                Value = NormalizePowerRequirement(powerGroup.ElementAtOrDefault(1)),
                Operator = AdvancedConditionOperator.Equals
            });
        }

        foreach (var powerGroup in requirement.PowerIDNot.Where(IsMeaningfulPowerGroup))
        {
            set.Rows.Add(new AdvancedConditionRow
            {
                Link = AdvancedConditionLink.And,
                Kind = AdvancedConditionKind.PowerRequirementGroup,
                Subject = NormalizePowerRequirement(powerGroup.ElementAtOrDefault(0)),
                Value = NormalizePowerRequirement(powerGroup.ElementAtOrDefault(1)),
                Operator = AdvancedConditionOperator.Equals,
                Negated = true
            });
        }

        return set;
    }

    public Requirement ToLegacyRequirement()
    {
        var requirement = new Requirement
        {
            ClassName = Rows
                .Where(r => r.Kind == AdvancedConditionKind.CharacterArchetype && !r.Negated)
                .Select(r => r.Value)
                .Where(IsMeaningfulValue)
                .ToArray(),
            ClassNameNot = Rows
                .Where(r => r.Kind == AdvancedConditionKind.CharacterArchetype && r.Negated)
                .Select(r => r.Value)
                .Where(IsMeaningfulValue)
                .ToArray(),
            PowerID = Rows
                .Where(r => r.Kind == AdvancedConditionKind.PowerRequirementGroup && !r.Negated)
                .Select(ToLegacyPowerGroup)
                .Where(IsMeaningfulPowerGroup)
                .ToArray(),
            PowerIDNot = Rows
                .Where(r => r.Kind == AdvancedConditionKind.PowerRequirementGroup && r.Negated)
                .Select(ToLegacyPowerGroup)
                .Where(IsMeaningfulPowerGroup)
                .ToArray()
        };

        return requirement;
    }

    public void StoreTo(BinaryWriter writer)
    {
        writer.Write(SerializationVersion);
        writer.Write(Rows.Count);
        foreach (var row in Rows)
        {
            writer.Write((int)row.Link);
            writer.Write((int)row.Kind);
            writer.Write((int)row.Operator);
            writer.Write(row.Subject ?? string.Empty);
            writer.Write(row.Value ?? string.Empty);
            writer.Write(row.Negated);
            writer.Write(row.RawExpression ?? string.Empty);
            writer.Write(row.Unsupported);
            writer.Write((int)row.EvaluationMode);
            writer.Write((int)row.TargetScope);
        }
    }

    public static AdvancedConditionSet ReadFrom(BinaryReader reader)
    {
        var version = reader.ReadInt32();
        if (version > SerializationVersion)
        {
            throw new InvalidDataException($"Unsupported advanced condition version {version}.");
        }

        var set = new AdvancedConditionSet();
        var rowCount = reader.ReadInt32();
        for (var i = 0; i < rowCount; i++)
        {
            var row = new AdvancedConditionRow
            {
                Link = (AdvancedConditionLink)reader.ReadInt32(),
                Kind = (AdvancedConditionKind)reader.ReadInt32(),
                Operator = (AdvancedConditionOperator)reader.ReadInt32(),
                Subject = reader.ReadString(),
                Value = reader.ReadString(),
                Negated = reader.ReadBoolean(),
                RawExpression = reader.ReadString(),
                Unsupported = reader.ReadBoolean()
            };

            if (version >= 2)
            {
                row.EvaluationMode = (AdvancedConditionEvaluationMode)reader.ReadInt32();
            }

            if (version >= 3)
            {
                row.TargetScope = (AdvancedConditionTargetScope)reader.ReadInt32();
            }

            set.Rows.Add(row);
        }

        return set;
    }

    public static void StoreMarked(BinaryWriter writer, string marker, AdvancedConditionSet? set)
    {
        writer.Write(marker);
        (set ?? new AdvancedConditionSet()).StoreTo(writer);
    }

    public static bool TryReadMarked(BinaryReader reader, string marker, out AdvancedConditionSet set)
    {
        set = new AdvancedConditionSet();
        if (!reader.BaseStream.CanSeek)
        {
            return false;
        }

        var position = reader.BaseStream.Position;
        try
        {
            if (!string.Equals(reader.ReadString(), marker, StringComparison.Ordinal))
            {
                reader.BaseStream.Position = position;
                return false;
            }

            set = ReadFrom(reader);
            return true;
        }
        catch
        {
            reader.BaseStream.Position = position;
            set = new AdvancedConditionSet();
            return false;
        }
    }

    private static string[] ToLegacyPowerGroup(AdvancedConditionRow row)
    {
        return
        [
            NormalizePowerRequirement(row.Subject),
            NormalizePowerRequirement(row.Value)
        ];
    }

    private static bool IsMeaningfulPowerGroup(string[] powerGroup)
    {
        return powerGroup.Any(IsMeaningfulValue);
    }

    private static bool IsMeaningfulValue(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               !string.Equals(value, "Empty", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePowerRequirement(string? value)
    {
        value = value?.Trim() ?? string.Empty;
        return value.StartsWith("!", StringComparison.Ordinal) ? value[1..] : value;
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

    public static string FormatOperator(AdvancedConditionOperator op)
    {
        return op switch
        {
            AdvancedConditionOperator.NotEquals => "!=",
            AdvancedConditionOperator.GreaterThan => ">",
            AdvancedConditionOperator.LessThan => "<",
            AdvancedConditionOperator.GreaterThanOrEqual => ">=",
            AdvancedConditionOperator.LessThanOrEqual => "<=",
            _ => "="
        };
    }
}

public sealed class AdvancedConditionRow
{
    public AdvancedConditionLink Link { get; set; }
    public AdvancedConditionKind Kind { get; set; }
    public AdvancedConditionOperator Operator { get; set; } = AdvancedConditionOperator.Equals;
    public string Subject { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool Negated { get; set; }
    public string RawExpression { get; set; } = string.Empty;
    public bool Unsupported { get; set; }
    public AdvancedConditionEvaluationMode EvaluationMode { get; set; } = AdvancedConditionEvaluationMode.BuildEvaluated;
    public AdvancedConditionTargetScope TargetScope { get; set; } = AdvancedConditionTargetScope.Unknown;

    public AdvancedConditionRow Clone()
    {
        return new AdvancedConditionRow
        {
            Link = Link,
            Kind = Kind,
            Operator = Operator,
            Subject = Subject,
            Value = Value,
            Negated = Negated,
            RawExpression = RawExpression,
            Unsupported = Unsupported,
            EvaluationMode = EvaluationMode,
            TargetScope = TargetScope
        };
    }

    public static AdvancedConditionRow AdvancedExpression(
        AdvancedConditionLink link,
        string expression,
        bool unsupported = false,
        AdvancedConditionEvaluationMode evaluationMode = AdvancedConditionEvaluationMode.BuildEvaluated)
    {
        return new AdvancedConditionRow
        {
            Link = link,
            Kind = AdvancedConditionKind.AdvancedExpression,
            RawExpression = expression,
            Value = expression,
            Unsupported = unsupported,
            EvaluationMode = evaluationMode
        };
    }
}

public static class AdvancedConditionCompiler
{
    public static string Compile(AdvancedConditionSet? set)
    {
        if (set is not { Rows.Count: > 0 })
        {
            return string.Empty;
        }

        return string.Join(" ", set.Rows.Select((row, index) =>
        {
            var link = index == 0 ? string.Empty : row.Link == AdvancedConditionLink.Or ? " || " : " && ";
            var expr = Compile(row);
            return $"{link}{expr}";
        }));
    }

    public static string Compile(AdvancedConditionRow row)
    {
        var expr = row.Kind switch
        {
            AdvancedConditionKind.PowerActive => $"powerActive({row.Subject}) {CompareText(row.Operator)} {BoolAsNumber(row.Value)}",
            AdvancedConditionKind.PowerTaken => $"source.ownPower?({row.Subject}) {CompareText(row.Operator)} {BoolAsNumber(row.Value)}",
            AdvancedConditionKind.PowerStacks => $"powerStacks({row.Subject}) {CompareText(row.Operator)} {row.Value}",
            AdvancedConditionKind.TeamMembers => $"teamMembers({row.Subject}) {CompareText(row.Operator)} {row.Value}",
            AdvancedConditionKind.CombatSetting => $"{row.Subject} {CompareText(row.Operator)} {row.Value}",
            AdvancedConditionKind.SourceOwnPower => $"source.ownPower?({row.Subject})",
            AdvancedConditionKind.SourceMode => $"Source.Mode?({row.Subject})",
            AdvancedConditionKind.TargetEntityType when row.TargetScope != AdvancedConditionTargetScope.Unknown => $"target.scope {CompareText(row.Operator)} '{row.TargetScope}'",
            AdvancedConditionKind.TargetEntityType => $"target>enttype {CompareText(row.Operator)} '{row.Value}'",
            AdvancedConditionKind.TargetMode => $"target.mode?({row.Subject})",
            AdvancedConditionKind.TargetGroup => $"target>group {CompareText(row.Operator)} '{row.Value}'",
            AdvancedConditionKind.TargetArchetype => $"target>arch {CompareText(row.Operator)} '{row.Value}'",
            AdvancedConditionKind.CharacterArchetype => $"char>arch {CompareText(row.Operator)} '{row.Value}'",
            AdvancedConditionKind.CharacterLevel => $"char>level {CompareText(row.Operator)} {row.Value}",
            AdvancedConditionKind.PowerRequirementGroup => CompilePowerRequirementGroup(row),
            AdvancedConditionKind.BoostsSlotted => $"BoostsSlotted>{row.Subject} {CompareText(row.Operator)} {row.Value}",
            AdvancedConditionKind.AdvancedExpression => row.RawExpression,
            AdvancedConditionKind.PowerCount => $"source.ownPowerNum?({row.Subject}) {CompareText(row.Operator)} {row.Value}",
            _ => row.RawExpression
        };

        return row.Negated ? $"!({expr})" : expr;
    }

    private static string CompareText(AdvancedConditionOperator op)
    {
        return op switch
        {
            AdvancedConditionOperator.NotEquals => "!=",
            AdvancedConditionOperator.GreaterThan => ">",
            AdvancedConditionOperator.LessThan => "<",
            AdvancedConditionOperator.GreaterThanOrEqual => ">=",
            AdvancedConditionOperator.LessThanOrEqual => "<=",
            _ => "=="
        };
    }

    private static string BoolAsNumber(string value)
    {
        return bool.TryParse(value, out var boolValue) && boolValue ? "1" : "0";
    }

    private static string CompilePowerRequirementGroup(AdvancedConditionRow row)
    {
        var first = $"source.ownPower?({row.Subject})";
        return string.IsNullOrWhiteSpace(row.Value)
            ? first
            : $"({first} && source.ownPower?({row.Value}))";
    }
}

public static class AdvancedConditionEvaluator
{
    public static bool Evaluate(IEffect effect)
    {
        var set = effect.AdvancedConditions is { Rows.Count: > 0 }
            ? effect.AdvancedConditions
            : AdvancedConditionSet.FromLegacyActiveConditionals(effect.ActiveConditionals);

        return Evaluate(effect, set);
    }

    public static bool Evaluate(IEffect effect, AdvancedConditionSet? set)
    {
        if (set is not { Rows.Count: > 0 })
        {
            return true;
        }

        var result = EvaluateRow(effect, set.Rows[0]);
        for (var i = 1; i < set.Rows.Count; i++)
        {
            var rowResult = EvaluateRow(effect, set.Rows[i]);
            result = set.Rows[i].Link == AdvancedConditionLink.Or
                ? result || rowResult
                : result && rowResult;
        }

        return result;
    }

    public static bool EvaluateRow(IEffect effect, AdvancedConditionRow row)
    {
        if (row.EvaluationMode is AdvancedConditionEvaluationMode.RuntimeTargetOnly or AdvancedConditionEvaluationMode.ReportOnly)
        {
            return true;
        }

        var result = row.Kind switch
        {
            AdvancedConditionKind.PowerActive => EvaluatePowerActive(row),
            AdvancedConditionKind.PowerTaken => EvaluatePowerTaken(row),
            AdvancedConditionKind.PowerStacks => EvaluatePowerStacks(row),
            AdvancedConditionKind.TeamMembers => CompareNumber(GetTeamMembers(row.Subject), row.Operator, ParseNumber(row.Value)),
            AdvancedConditionKind.CombatSetting => EvaluateCombatSetting(row),
            AdvancedConditionKind.SourceOwnPower => EvaluateOwnPower(row.Subject),
            AdvancedConditionKind.SourceMode => EvaluateSourceMode(row),
            AdvancedConditionKind.TargetEntityType => EvaluateTargetEntityType(row),
            AdvancedConditionKind.TargetMode => EvaluateUnsupportedTargetState(row),
            AdvancedConditionKind.TargetGroup => EvaluateTargetGroup(row),
            AdvancedConditionKind.TargetArchetype => EvaluateUnsupportedTargetState(row),
            AdvancedConditionKind.CharacterArchetype => CompareString(GetCharacterArchetype(), row.Operator, row.Value),
            AdvancedConditionKind.CharacterLevel => CompareNumber(MidsContext.Character?.Level ?? 0, row.Operator, ParseNumber(row.Value)),
            AdvancedConditionKind.PowerRequirementGroup => EvaluatePowerRequirementGroup(row),
            AdvancedConditionKind.BoostsSlotted => CompareNumber(0, row.Operator, ParseNumber(row.Value)),
            AdvancedConditionKind.AdvancedExpression => EvaluateAdvancedExpression(row),
            AdvancedConditionKind.PowerCount => EvaluateOwnedPowerCount(row),
            _ => false
        };

        return row.Negated ? !result : result;
    }

    public static bool EvaluatePowerRequirements(IPower? power, int nLevel, int skipIdx = -1, Build? build = null)
    {
        if (power == null || nLevel < 0)
        {
            return false;
        }

        if (nLevel + 1 < power.Level)
        {
            return false;
        }

        var set = power.AdvancedRequirements is { Rows.Count: > 0 }
            ? power.AdvancedRequirements
            : AdvancedConditionSet.FromLegacyRequirement(power.Requires);

        if (set.Rows.Count == 0)
        {
            return true;
        }

        build ??= MidsContext.Character?.CurrentBuild;

        var classIncludes = set.Rows
            .Where(r => r.Kind == AdvancedConditionKind.CharacterArchetype && !r.Negated)
            .ToList();
        if (classIncludes.Count > 0 && !classIncludes.Any(EvaluatePowerRequirementClassRow))
        {
            return false;
        }

        var classExcludes = set.Rows
            .Where(r => r.Kind == AdvancedConditionKind.CharacterArchetype && r.Negated)
            .ToList();
        if (classExcludes.Any(r => CompareString(GetCharacterArchetype(), r.Operator, r.Value)))
        {
            return false;
        }

        var requiredPowerGroups = set.Rows
            .Where(r => r.Kind == AdvancedConditionKind.PowerRequirementGroup && !r.Negated)
            .ToList();
        if (requiredPowerGroups.Count > 0 &&
            !requiredPowerGroups.Any(r => EvaluatePowerRequirementGroup(r, nLevel, skipIdx, build)))
        {
            return false;
        }

        var excludedPowerGroups = set.Rows
            .Where(r => r.Kind == AdvancedConditionKind.PowerRequirementGroup && r.Negated)
            .ToList();
        if (excludedPowerGroups.Any(r => EvaluateExcludedPowerRequirementGroup(r, nLevel, skipIdx, build)))
        {
            return false;
        }

        foreach (var row in set.Rows.Where(r =>
                     r.Kind != AdvancedConditionKind.CharacterArchetype &&
                     r.Kind != AdvancedConditionKind.PowerRequirementGroup))
        {
            if (!EvaluateRowForPower(row))
            {
                return false;
            }
        }

        return true;
    }

    public static void UpdateLegacyValidationFlags(IEffect effect)
    {
        if (effect.ActiveConditionals is not { Count: > 0 })
        {
            return;
        }

        var set = AdvancedConditionSet.FromLegacyActiveConditionals(effect.ActiveConditionals);
        for (var i = 0; i < effect.ActiveConditionals.Count && i < set.Rows.Count; i++)
        {
            effect.ActiveConditionals[i].Validated = EvaluateRow(effect, set.Rows[i]);
        }
    }

    private static bool EvaluatePowerActive(AdvancedConditionRow row)
    {
        var power = DatabaseAPI.GetPowerByFullName(row.Subject);
        var actual = power != null && MidsContext.Character?.CurrentBuild?.PowerActive(power) == true;
        var expected = ParseBool(row.Value);
        return CompareBool(actual, row.Operator, expected);
    }

    private static bool EvaluatePowerTaken(AdvancedConditionRow row)
    {
        var actual = EvaluateOwnPower(row.Subject);
        var expected = ParseBool(row.Value);
        return CompareBool(actual, row.Operator, expected);
    }

    private static bool EvaluateOwnPower(string powerName)
    {
        var power = DatabaseAPI.GetPowerByFullName(powerName);
        return power != null && MidsContext.Character?.CurrentBuild?.PowerUsed(power) == true;
    }

    private static bool EvaluatePowerRequirementClassRow(AdvancedConditionRow row)
    {
        return CompareString(GetCharacterArchetype(), row.Operator, row.Value);
    }

    private static bool EvaluatePowerRequirementGroup(AdvancedConditionRow row)
    {
        return EvaluatePowerRequirementGroup(row, MidsContext.Character?.Level ?? 0, -1, MidsContext.Character?.CurrentBuild);
    }

    private static bool EvaluatePowerRequirementGroup(AdvancedConditionRow row, int nLevel, int skipIdx, Build? build)
    {
        if (build == null)
        {
            return false;
        }

        var nIdSkip = -1;
        if (skipIdx > -1 && skipIdx < build.Powers.Count)
        {
            nIdSkip = build.Powers[skipIdx]?.Power?.PowerIndex ?? -1;
        }

        return PowerIsTakenByLevel(row.Subject, nLevel, nIdSkip, build) &&
               (string.IsNullOrWhiteSpace(row.Value) || PowerIsTakenByLevel(row.Value, nLevel, nIdSkip, build));
    }

    private static bool EvaluateExcludedPowerRequirementGroup(AdvancedConditionRow row, int nLevel, int skipIdx, Build? build)
    {
        if (build == null)
        {
            return false;
        }

        var nIdSkip = -1;
        if (skipIdx > -1 && skipIdx < build.Powers.Count)
        {
            nIdSkip = build.Powers[skipIdx]?.Power?.PowerIndex ?? -1;
        }

        return PowerIsTakenByLevel(row.Subject, nLevel, nIdSkip, build) ||
               PowerIsTakenByLevel(row.Value, nLevel, nIdSkip, build);
    }

    private static bool PowerIsTakenByLevel(string powerName, int nLevel, int nIdSkip, Build build)
    {
        if (string.IsNullOrWhiteSpace(powerName))
        {
            return true;
        }

        var powerId = DatabaseAPI.NidFromUidPower(powerName);
        if (powerId < 0 || powerId == nIdSkip)
        {
            return false;
        }

        var powerIndex = build.FindInToonHistory(powerId);
        return powerIndex >= 0 && build.Powers[powerIndex]?.Level <= nLevel;
    }

    private static bool EvaluateRowForPower(AdvancedConditionRow row)
    {
        if (row.EvaluationMode != AdvancedConditionEvaluationMode.BuildEvaluated)
        {
            return false;
        }

        var result = row.Kind switch
        {
            AdvancedConditionKind.SourceOwnPower or AdvancedConditionKind.PowerTaken => EvaluateOwnPower(row.Subject),
            AdvancedConditionKind.SourceMode => EvaluateSourceMode(row),
            AdvancedConditionKind.CombatSetting => EvaluateCombatSetting(row),
            AdvancedConditionKind.CharacterLevel => CompareNumber(MidsContext.Character?.Level ?? 0, row.Operator, ParseNumber(row.Value)),
            AdvancedConditionKind.PowerCount => EvaluateOwnedPowerCount(row),
            AdvancedConditionKind.AdvancedExpression => EvaluateAdvancedExpression(row),
            _ => EvaluateAdvancedExpression(new AdvancedConditionRow { Unsupported = true })
        };

        return row.Negated ? !result : result;
    }

    private static bool EvaluatePowerStacks(AdvancedConditionRow row)
    {
        var power = DatabaseAPI.GetPowerByFullName(row.Subject);
        if (power == null)
        {
            return false;
        }

        var pe = MidsContext.Character?.CurrentBuild?.Powers
            .DefaultIfEmpty(null)
            .FirstOrDefault(e => e?.Power?.StaticIndex == power.StaticIndex);
        var stacks = Math.Max(power.Stacks, pe?.VariableValue ?? 0);

        return CompareNumber(stacks, row.Operator, ParseNumber(row.Value));
    }

    private static bool EvaluateOwnedPowerCount(AdvancedConditionRow row)
    {
        var build = MidsContext.Character?.CurrentBuild;
        if (build?.Powers == null)
        {
            return false;
        }

        var prefix = (row.Subject ?? string.Empty).Trim().Trim('.');
        if (string.IsNullOrWhiteSpace(prefix))
        {
            return false;
        }

        var count = build.Powers.Count(powerEntry =>
        {
            var fullName = powerEntry?.Power?.FullName;
            return !string.IsNullOrWhiteSpace(fullName) &&
                   (fullName.Equals(prefix, StringComparison.OrdinalIgnoreCase) ||
                    fullName.StartsWith(prefix + ".", StringComparison.OrdinalIgnoreCase));
        });

        return CompareNumber(count, row.Operator, ParseNumber(row.Value));
    }

    private static bool EvaluateCombatSetting(AdvancedConditionRow row)
    {
        var value = GetConfigValue(row.Subject);
        if (bool.TryParse(row.Value, out var boolTarget))
        {
            return CompareBool(value != 0, row.Operator, boolTarget);
        }

        return CompareNumber(value, row.Operator, ParseNumber(row.Value));
    }

    private static bool EvaluateSourceMode(AdvancedConditionRow row)
    {
        var mode = OmniModeMapper.Normalize(row.Subject);
        if (PlannerModeMapper.TryGetPlannerMode(mode, out var plannerMode))
        {
            return plannerMode switch
            {
                PlannerMode.DefensiveAdaptation => MidsContext.Character?.DefensiveAdaptation == true,
                PlannerMode.EfficientAdaptation => MidsContext.Character?.EfficientAdaptation == true,
                PlannerMode.OffensiveAdaptation => MidsContext.Character?.OffensiveAdaptation == true,
                PlannerMode.Domination => MidsContext.Character?.Domination == true,
                PlannerMode.Scourge => MidsContext.Character?.Scourge == true,
                PlannerMode.Containment => MidsContext.Character?.Containment == true,
                PlannerMode.CriticalHit => MidsContext.Character?.CriticalHits == true,
                PlannerMode.Assassination => MidsContext.Character?.Assassination == true,
                PlannerMode.Defiance => MidsContext.Character?.Defiance == true,
                PlannerMode.FastSnipe => MidsContext.Character?.FastSnipe == true,
                PlannerMode.ComboLevel1 => MidsContext.Character?.ActiveComboLevel == 1,
                PlannerMode.ComboLevel2 => MidsContext.Character?.ActiveComboLevel == 2,
                PlannerMode.ComboLevel3 => MidsContext.Character?.ActiveComboLevel == 3,
                PlannerMode.FastMode => MidsContext.Character?.FastModeActive == true,
                PlannerMode.PerfectionOfBody => MidsContext.Character?.PerfectionOfBodyLevel > 0,
                PlannerMode.PerfectionOfMind => MidsContext.Character?.PerfectionOfMindLevel > 0,
                PlannerMode.PerfectionOfSoul => MidsContext.Character?.PerfectionOfSoulLevel > 0,
                PlannerMode.PackMentality => MidsContext.Character?.PackMentality == true,
                _ => false
            };
        }

        var result = mode switch
        {
            "DefensiveAdaptation" => MidsContext.Character?.DefensiveAdaptation == true,
            "EfficientAdaptation" => MidsContext.Character?.EfficientAdaptation == true,
            "OffensiveAdaptation" => MidsContext.Character?.OffensiveAdaptation == true,
            "Domination" => MidsContext.Character?.Domination == true,
            "Scourge" => MidsContext.Character?.Scourge == true,
            "Containment" => MidsContext.Character?.Containment == true,
            "CriticalHit" => MidsContext.Character?.CriticalHits == true,
            "Assassination" => MidsContext.Character?.Assassination == true,
            "FastSnipe" => MidsContext.Character?.FastSnipe == true,
            _ => false
        };

        return result;
    }

    private static bool EvaluateTargetEntityType(AdvancedConditionRow row)
    {
        if (row.TargetScope != AdvancedConditionTargetScope.Unknown)
        {
            return true;
        }

        var value = row.Value.Trim('\'', '"').ToLowerInvariant();
        var targetIsPlayer = MidsContext.Config?.Inc.DisablePvE == true;
        var actual = targetIsPlayer ? "player" : "critter";

        return CompareString(actual, row.Operator, value);
    }

    private static bool EvaluateTargetGroup(AdvancedConditionRow row)
    {
        return EvaluateUnsupportedTargetState(row);
    }

    private static bool EvaluateUnsupportedTargetState(AdvancedConditionRow row)
    {
        return row.EvaluationMode is AdvancedConditionEvaluationMode.RuntimeTargetOnly or AdvancedConditionEvaluationMode.ReportOnly;
    }

    private static bool EvaluateAdvancedExpression(AdvancedConditionRow row)
    {
        if (row.EvaluationMode is AdvancedConditionEvaluationMode.RuntimeTargetOnly or AdvancedConditionEvaluationMode.ReportOnly)
        {
            return true;
        }

        return !row.Unsupported;
    }

    private static int GetConfigValue(string cond)
    {
        var chunks = cond.ToLowerInvariant().Split('.');
        if (chunks.Length != 3 || MidsContext.Config == null)
        {
            return 0;
        }

        var group = MidsContext.Config.CombatContextSettings.GetType()
            .GetProperty(ConfigData.CombatContext.GetConfigChunkName(chunks[1]));

        var groupValue = group?.GetValue(MidsContext.Config.CombatContextSettings);
        var value = groupValue?.GetType()
            .GetProperty(ConfigData.CombatContext.GetConfigChunkName(chunks[2]))?
            .GetValue(groupValue);

        return value switch
        {
            bool b => b ? 1 : 0,
            int i => i,
            _ => 0
        };
    }

    private static int GetTeamMembers(string archetype)
    {
        if (MidsContext.Config?.TeamMembers == null)
        {
            return 0;
        }

        return MidsContext.Config.TeamMembers.TryGetValue(archetype, out var count)
            ? count
            : 0;
    }

    private static string GetCharacterArchetype()
    {
        var archetype = MidsContext.Character?.Archetype;
        if (archetype == null)
        {
            return string.Empty;
        }

        return string.IsNullOrWhiteSpace(archetype.ClassName)
            ? archetype.DisplayName
            : archetype.ClassName;
    }

    private static bool CompareBool(bool actual, AdvancedConditionOperator op, bool expected)
    {
        return op switch
        {
            AdvancedConditionOperator.NotEquals => actual != expected,
            _ => actual == expected
        };
    }

    private static bool CompareString(string actual, AdvancedConditionOperator op, string expected)
    {
        expected = expected.Trim('\'', '"');
        var normalizedActual = NormalizeClassToken(actual);
        var normalizedExpected = NormalizeClassToken(expected);
        return op switch
        {
            AdvancedConditionOperator.NotEquals => !string.Equals(normalizedActual, normalizedExpected, StringComparison.OrdinalIgnoreCase),
            _ => string.Equals(normalizedActual, normalizedExpected, StringComparison.OrdinalIgnoreCase)
        };
    }

    private static string NormalizeClassToken(string value)
    {
        value = value.Trim().Trim('\'', '"');
        return value.StartsWith("Class_", StringComparison.OrdinalIgnoreCase)
            ? value[6..]
            : value;
    }

    private static bool CompareNumber(float actual, AdvancedConditionOperator op, float expected)
    {
        return op switch
        {
            AdvancedConditionOperator.NotEquals => Math.Abs(actual - expected) > float.Epsilon,
            AdvancedConditionOperator.GreaterThan => actual > expected,
            AdvancedConditionOperator.LessThan => actual < expected,
            AdvancedConditionOperator.GreaterThanOrEqual => actual >= expected,
            AdvancedConditionOperator.LessThanOrEqual => actual <= expected,
            _ => Math.Abs(actual - expected) < float.Epsilon
        };
    }

    private static bool ParseBool(string value)
    {
        return bool.TryParse(value, out var boolValue)
            ? boolValue
            : value == "1";
    }

    private static float ParseNumber(string value)
    {
        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
            ? number
            : 0;
    }
}

public static class AdvancedConditionDecompiler
{
    private static readonly Regex TargetModeRegex = new(@"^target\.mode\?\(([^)]+)\)$", RegexOptions.IgnoreCase);
    private static readonly Regex SourceModeRegex = new(@"^Source\.Mode\?\(([^)]+)\)$", RegexOptions.IgnoreCase);
    private static readonly Regex OwnPowerRegex = new(@"^(source\.)?ownPower\?\(([^)]+)\)$", RegexOptions.IgnoreCase);

    public static AdvancedConditionRow FromSimpleExpression(string expression, AdvancedConditionLink link = AdvancedConditionLink.And)
    {
        expression = expression.Trim();
        var negated = expression.StartsWith("!", StringComparison.Ordinal);
        if (negated)
        {
            expression = expression[1..].Trim();
            if (expression.StartsWith("(") && expression.EndsWith(")"))
            {
                expression = expression[1..^1].Trim();
            }
        }

        var targetMode = TargetModeRegex.Match(expression);
        if (targetMode.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.TargetMode,
                Subject = targetMode.Groups[1].Value,
                Negated = negated,
                Unsupported = true
            };
        }

        var sourceMode = SourceModeRegex.Match(expression);
        if (sourceMode.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.SourceMode,
                Subject = sourceMode.Groups[1].Value,
                Negated = negated
            };
        }

        var ownPower = OwnPowerRegex.Match(expression);
        if (ownPower.Success)
        {
            return new AdvancedConditionRow
            {
                Link = link,
                Kind = AdvancedConditionKind.SourceOwnPower,
                Subject = ownPower.Groups[2].Value,
                Negated = negated
            };
        }

        return AdvancedConditionRow.AdvancedExpression(link, expression, unsupported: true);
    }
}

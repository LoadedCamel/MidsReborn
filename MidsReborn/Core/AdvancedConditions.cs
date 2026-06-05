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
    PowerCount,
    BooleanLiteral
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
    private const int SerializationVersion = 4;

    public List<AdvancedConditionRow> Rows { get; set; } = [];

    public bool Any() => Rows.Count > 0;

    public AdvancedConditionSet Clone()
    {
        return new AdvancedConditionSet
        {
            Rows = Rows.Select(r => r.Clone()).ToList()
        };
    }

    public bool AllowsClass(string? className)
    {
        var normalizedClassName = NormalizeClassName(className);
        if (string.IsNullOrWhiteSpace(normalizedClassName))
        {
            return true;
        }

        var includeRows = Rows
            .Where(row => row.Kind == AdvancedConditionKind.CharacterArchetype && !row.Negated)
            .ToList();
        if (includeRows.Count > 0 && !includeRows.Any(row => MatchesClassRow(row, normalizedClassName)))
        {
            return false;
        }

        return !Rows
            .Where(row => row.Kind == AdvancedConditionKind.CharacterArchetype && row.Negated)
            .Any(row => MatchesClassRow(row, normalizedClassName));
    }

    public bool AllowsClass(int classId, IDatabase? database = null)
    {
        if (classId < 0)
        {
            return true;
        }

        database ??= DatabaseAPI.Database;
        if (database?.Classes == null || classId >= database.Classes.Length)
        {
            return true;
        }

        return AllowsClass(database.Classes[classId]?.ClassName ?? string.Empty);
    }

    internal static AdvancedConditionSet FromLegacyRequirement(Requirement? requirement)
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

    internal Requirement ToLegacyRequirement()
    {
        return new Requirement
        {
            ClassName = Rows
                .Where(IsLegacyCompatibleIncludedClassRow)
                .Select(r => NormalizeClassName(r.Value))
                .Where(IsMeaningfulValue)
                .ToArray(),
            ClassNameNot = Rows
                .Where(IsLegacyCompatibleExcludedClassRow)
                .Select(r => NormalizeClassName(r.Value))
                .Where(IsMeaningfulValue)
                .ToArray(),
            PowerID = Rows
                .Where(IsLegacyCompatibleIncludedPowerRequirementRow)
                .Select(ToLegacyPowerGroup)
                .Where(IsMeaningfulPowerGroup)
                .ToArray(),
            PowerIDNot = Rows
                .Where(IsLegacyCompatibleExcludedPowerRequirementRow)
                .Select(ToLegacyPowerGroup)
                .Where(IsMeaningfulPowerGroup)
                .ToArray()
        };
    }

    public IReadOnlyList<string> GetIncludedClassNames()
    {
        return Rows
            .Where(row => row.Kind == AdvancedConditionKind.CharacterArchetype && !row.Negated)
            .Select(row => NormalizeClassName(row.Value))
            .Where(IsMeaningfulValue)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public IReadOnlyList<string> GetExcludedClassNames()
    {
        return Rows
            .Where(row => row.Kind == AdvancedConditionKind.CharacterArchetype && row.Negated)
            .Select(row => NormalizeClassName(row.Value))
            .Where(IsMeaningfulValue)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public string? GetFirstPositiveReferencedPower()
    {
        foreach (var row in Rows)
        {
            if (row.Negated)
            {
                continue;
            }

            foreach (var powerName in EnumerateReferencedPowers(row))
            {
                if (IsMeaningfulValue(powerName))
                {
                    return powerName;
                }
            }
        }

        return null;
    }

    public bool ReferencesPower(string? powerName)
    {
        return RewritePowerReferences(powerName, null);
    }

    public bool RewritePowerReferences(string? powerName, string? replacement)
    {
        if (!IsMeaningfulValue(powerName))
        {
            return false;
        }

        var changed = false;
        foreach (var row in Rows)
        {
            changed |= RewritePowerReference(row, powerName!, replacement);
        }

        return changed;
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
            if (!BinaryMetadataEnvelope.TryConsumeMarker(reader, marker))
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
        if (row.Kind == AdvancedConditionKind.PowerRequirementGroup)
        {
            return
            [
                NormalizePowerRequirement(row.Subject),
                NormalizePowerRequirement(row.Value)
            ];
        }

        return
        [
            NormalizePowerRequirement(row.Subject),
            string.Empty
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

    private static string NormalizeClassName(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    private static bool IsLegacyCompatibleIncludedClassRow(AdvancedConditionRow row)
    {
        return row.Kind == AdvancedConditionKind.CharacterArchetype &&
               !row.Negated &&
               row.Operator == AdvancedConditionOperator.Equals;
    }

    private static bool IsLegacyCompatibleExcludedClassRow(AdvancedConditionRow row)
    {
        return row.Kind == AdvancedConditionKind.CharacterArchetype &&
               row.Negated &&
               row.Operator == AdvancedConditionOperator.Equals;
    }

    private static bool IsLegacyCompatibleIncludedPowerRequirementRow(AdvancedConditionRow row)
    {
        return IsLegacyCompatiblePowerRequirementRow(row) && !row.Negated;
    }

    private static bool IsLegacyCompatibleExcludedPowerRequirementRow(AdvancedConditionRow row)
    {
        return IsLegacyCompatiblePowerRequirementRow(row) && row.Negated;
    }

    private static bool IsLegacyCompatiblePowerRequirementRow(AdvancedConditionRow row)
    {
        if (row.EvaluationMode != AdvancedConditionEvaluationMode.BuildEvaluated ||
            row.Operator != AdvancedConditionOperator.Equals)
        {
            return false;
        }

        return row.Kind switch
        {
            AdvancedConditionKind.PowerRequirementGroup => IsMeaningfulValue(row.Subject) || IsMeaningfulValue(row.Value),
            AdvancedConditionKind.PowerTaken => IsMeaningfulValue(row.Subject) && IsLegacyCompatibleSimplePowerRequirement(row),
            _ => false
        };
    }

    private static bool IsLegacyCompatibleSimplePowerRequirement(AdvancedConditionRow row)
    {
        return row.Kind == AdvancedConditionKind.PowerTaken &&
               (bool.TryParse(row.Value, out var boolValue) ? boolValue : row.Value == "1");
    }

    private static bool MatchesClassRow(AdvancedConditionRow row, string className)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            return false;
        }

        var normalizedValue = NormalizeClassName(row.Value);
        return row.Operator switch
        {
            AdvancedConditionOperator.NotEquals => !string.Equals(normalizedValue, className, StringComparison.OrdinalIgnoreCase),
            _ => string.Equals(normalizedValue, className, StringComparison.OrdinalIgnoreCase)
        };
    }

    private static IEnumerable<string> EnumerateReferencedPowers(AdvancedConditionRow row)
    {
        switch (row.Kind)
        {
            case AdvancedConditionKind.PowerTaken:
            case AdvancedConditionKind.SourceOwnPower:
            case AdvancedConditionKind.PowerCount:
                if (IsMeaningfulValue(row.Subject))
                {
                    yield return NormalizePowerRequirement(row.Subject);
                }

                yield break;

            case AdvancedConditionKind.PowerRequirementGroup:
                if (IsMeaningfulValue(row.Subject))
                {
                    yield return NormalizePowerRequirement(row.Subject);
                }

                if (IsMeaningfulValue(row.Value))
                {
                    yield return NormalizePowerRequirement(row.Value);
                }

                yield break;
        }
    }

    private static bool RewritePowerReference(AdvancedConditionRow row, string powerName, string? replacement)
    {
        var changed = false;
        if (row.Kind is AdvancedConditionKind.PowerTaken or AdvancedConditionKind.SourceOwnPower or AdvancedConditionKind.PowerCount)
        {
            var subject = row.Subject;
            changed |= RewritePowerField(ref subject, powerName, replacement);
            row.Subject = subject;
        }
        else if (row.Kind == AdvancedConditionKind.PowerRequirementGroup)
        {
            var subject = row.Subject;
            var value = row.Value;
            changed |= RewritePowerField(ref subject, powerName, replacement);
            changed |= RewritePowerField(ref value, powerName, replacement);
            row.Subject = subject;
            row.Value = value;
        }

        return changed;
    }

    private static bool RewritePowerField(ref string value, string powerName, string? replacement)
    {
        var normalized = NormalizePowerRequirement(value);
        if (!normalized.Equals(powerName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        value = replacement ?? string.Empty;
        return true;
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

    public static AdvancedConditionRow BooleanLiteral(AdvancedConditionLink link, bool value)
    {
        return new AdvancedConditionRow
        {
            Link = link,
            Kind = AdvancedConditionKind.BooleanLiteral,
            Value = value ? "true" : "false",
            RawExpression = value ? "1" : "0",
            EvaluationMode = AdvancedConditionEvaluationMode.BuildEvaluated
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
            AdvancedConditionKind.TargetGroup when row.Subject.Equals("tag", StringComparison.OrdinalIgnoreCase) => $"target.HasTag?({row.Value})",
            AdvancedConditionKind.TargetGroup => $"target>group {CompareText(row.Operator)} '{row.Value}'",
            AdvancedConditionKind.TargetArchetype => $"target>arch {CompareText(row.Operator)} '{row.Value}'",
            AdvancedConditionKind.CharacterArchetype => $"char>arch {CompareText(row.Operator)} '{row.Value}'",
            AdvancedConditionKind.CharacterLevel => $"char>level {CompareText(row.Operator)} {row.Value}",
            AdvancedConditionKind.PowerRequirementGroup => CompilePowerRequirementGroup(row),
            AdvancedConditionKind.BoostsSlotted => $"BoostsSlotted>{row.Subject} {CompareText(row.Operator)} {row.Value}",
            AdvancedConditionKind.AdvancedExpression => row.RawExpression,
            AdvancedConditionKind.PowerCount => $"source.ownPowerNum?({row.Subject}) {CompareText(row.Operator)} {row.Value}",
            AdvancedConditionKind.BooleanLiteral => BoolAsNumber(row.Value),
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

internal sealed class BuildConditionSnapshot
{
    public HashSet<PlannerMode> ActivePlannerModes { get; } = [];
    public HashSet<string> ActiveSourceModes { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Enums.eModeFlags ActiveSourceModeFlags { get; private set; } = Enums.eModeFlags.None;
    public Dictionary<string, int> StackCounts { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static BuildConditionSnapshot Create(
        Build? build,
        IEnumerable<string>? activeSourceModes = null,
        Enums.eModeFlags activeSourceModeFlags = Enums.eModeFlags.None)
    {
        var snapshot = new BuildConditionSnapshot();
        snapshot.SetSourceModes(activeSourceModes, activeSourceModeFlags);
        if (build?.Powers == null)
        {
            return snapshot;
        }

        foreach (var entry in build.Powers.Where(entry => entry is { Power: not null, StatInclude: true }))
        {
            var power = entry!.Power;
            if (PlannerStateCatalog.IsHiddenPayloadPower(power.FullName))
            {
                continue;
            }

            if (power.VariableEnabled)
            {
                snapshot.StackCounts[power.FullName] = Math.Max(0, entry.VariableValue);
            }

            foreach (var effect in power.Effects.Where(effect => effect?.EffectType == Enums.eEffectType.SetMode))
            {
                if (PlannerModeMapper.TryGetPlannerMode(effect!.ModeName, out var effectMode))
                {
                    snapshot.SetPlannerMode(effectMode);
                }
            }

            if (PlannerModeMapper.TryGetPlannerMode(power.PowerName, out var powerMode) ||
                PlannerModeMapper.TryGetPlannerMode(power.FullName?.Split('.').LastOrDefault(), out powerMode))
            {
                snapshot.SetPlannerMode(powerMode);
            }

            if (PlannerStateCatalog.TryGetImpliedPlannerModes(power.FullName, out var impliedModes))
            {
                foreach (var impliedMode in impliedModes)
                {
                    snapshot.SetPlannerMode(impliedMode);
                }
            }
        }

        DeriveCompatibilityModes(snapshot);
        return snapshot;
    }

    public bool IsPlannerModeActive(PlannerMode mode)
    {
        return ActivePlannerModes.Contains(mode);
    }

    public bool IsSourceModeActive(string? mode)
    {
        return !string.IsNullOrWhiteSpace(mode) &&
               OmniModeMapper.ModeMatches(mode, ActiveSourceModes, ActiveSourceModeFlags);
    }

    public int GetStacks(string powerFullName)
    {
        return StackCounts.TryGetValue(powerFullName, out var stacks) ? stacks : 0;
    }

    private void SetPlannerMode(PlannerMode mode)
    {
        if (PlannerStateCatalog.TryGetExclusiveModeFamily(mode, out var familyModes))
        {
            foreach (var siblingMode in familyModes)
            {
                if (siblingMode != mode)
                {
                    ActivePlannerModes.Remove(siblingMode);
                }
            }
        }

        ActivePlannerModes.Add(mode);
    }

    private void SetSourceModes(IEnumerable<string>? activeSourceModes, Enums.eModeFlags activeSourceModeFlags)
    {
        ActiveSourceModeFlags = activeSourceModeFlags;
        foreach (var mode in OmniModeMapper.NormalizeModeList(activeSourceModes))
        {
            ActiveSourceModes.Add(mode);
            if (OmniModeMapper.TryToFlag(mode, out var flag))
            {
                ActiveSourceModeFlags |= flag;
            }
        }
    }

    private static void DeriveCompatibilityModes(BuildConditionSnapshot snapshot)
    {
        if (snapshot.ActivePlannerModes.Contains(PlannerMode.DominationActive) ||
            snapshot.ActivePlannerModes.Contains(PlannerMode.Domination))
        {
            snapshot.ActivePlannerModes.Add(PlannerMode.DominationActive);
            snapshot.ActivePlannerModes.Add(PlannerMode.Domination);
            snapshot.StackCounts[PlannerStateCatalog.DominationMeterPowerFullName] =
                Math.Max(100, snapshot.GetStacks(PlannerStateCatalog.DominationMeterPowerFullName));
        }

        if (snapshot.GetStacks(PlannerStateCatalog.PackMentalityMarker) > 0)
        {
            snapshot.ActivePlannerModes.Add(PlannerMode.PackMentality);
        }

        var hasBody = snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionOfBody) || MidsContext.Character?.IsStalker == true;
        var hasMind = snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionOfMind);
        var hasSoul = snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionOfSoul);

        if (hasBody)
        {
            snapshot.ActivePlannerModes.Add(PlannerMode.PerfectionOfBody);
            if (snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionLevel1)) snapshot.ActivePlannerModes.Add(PlannerMode.PerfectionOfBody1);
            if (snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionLevel2)) snapshot.ActivePlannerModes.Add(PlannerMode.PerfectionOfBody2);
            if (snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionLevel3)) snapshot.ActivePlannerModes.Add(PlannerMode.PerfectionOfBody3);
        }

        if (hasMind)
        {
            if (snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionLevel1)) snapshot.ActivePlannerModes.Add(PlannerMode.PerfectionOfMind1);
            if (snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionLevel2)) snapshot.ActivePlannerModes.Add(PlannerMode.PerfectionOfMind2);
            if (snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionLevel3)) snapshot.ActivePlannerModes.Add(PlannerMode.PerfectionOfMind3);
        }

        if (hasSoul)
        {
            if (snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionLevel1)) snapshot.ActivePlannerModes.Add(PlannerMode.PerfectionOfSoul1);
            if (snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionLevel2)) snapshot.ActivePlannerModes.Add(PlannerMode.PerfectionOfSoul2);
            if (snapshot.ActivePlannerModes.Contains(PlannerMode.PerfectionLevel3)) snapshot.ActivePlannerModes.Add(PlannerMode.PerfectionOfSoul3);
        }
    }
}

public static class AdvancedConditionEvaluator
{

    public static bool Evaluate(IEffect effect)
    {
        return Evaluate(effect, effect.AdvancedConditions);
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
            if (TryEvaluatePreservedRuntimeRow(effect, row, out var preservedResult))
            {
                return row.Negated ? !preservedResult : preservedResult;
            }

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
            AdvancedConditionKind.TargetArchetype => EvaluateTargetArchetype(row),
            AdvancedConditionKind.CharacterArchetype => CompareString(GetCharacterArchetype(), row.Operator, row.Value),
            AdvancedConditionKind.CharacterLevel => CompareNumber(MidsContext.Character?.Level ?? 0, row.Operator, ParseNumber(row.Value)),
            AdvancedConditionKind.PowerRequirementGroup => EvaluatePowerRequirementGroup(row),
            AdvancedConditionKind.BoostsSlotted => CompareNumber(0, row.Operator, ParseNumber(row.Value)),
            AdvancedConditionKind.AdvancedExpression => EvaluateAdvancedExpression(row),
            AdvancedConditionKind.PowerCount => EvaluateOwnedPowerCount(row),
            AdvancedConditionKind.BooleanLiteral => EvaluateBooleanLiteral(row),
            _ => false
        };

        return row.Negated ? !result : result;
    }

    public static bool EvaluatePowerRequirements(
        IPower? power,
        int nLevel,
        int skipIdx = -1,
        Build? build = null)
    {
        return EvaluatePowerRequirements(
            power,
            nLevel,
            skipIdx,
            build,
            null,
            includeSourceModes: false);
    }

    internal static bool EvaluatePowerRequirements(
        IPower? power,
        int nLevel,
        int skipIdx,
        Build? build,
        BuildConditionSnapshot? snapshot,
        bool includeSourceModes)
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
            : new AdvancedConditionSet();

        if (set.Rows.Count == 0)
        {
            return true;
        }

        build ??= MidsContext.Character?.CurrentBuild;
        snapshot ??= BuildConditionSnapshot.Create(
            build,
            MidsContext.Character?.ActiveSourceModes,
            MidsContext.Character?.ActiveSourceModeFlags ?? Enums.eModeFlags.None);

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

        var tailRows = set.Rows.Where(r =>
                r.Kind != AdvancedConditionKind.CharacterArchetype &&
                r.Kind != AdvancedConditionKind.PowerRequirementGroup &&
                r.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated)
            .ToList();

        if (tailRows.Count == 0)
        {
            return true;
        }

        var result = EvaluateRowForPower(tailRows[0], build, snapshot, includeSourceModes);
        for (var index = 1; index < tailRows.Count; index++)
        {
            var row = tailRows[index];
            var rowResult = EvaluateRowForPower(row, build, snapshot, includeSourceModes);
            result = row.Link == AdvancedConditionLink.Or
                ? result || rowResult
                : result && rowResult;
        }

        return result;
    }

    public static void UpdateLegacyValidationFlags(IEffect effect)
    {
        _ = effect;
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
        if (PlannerStateCatalog.TryEvaluateOwnPowerState(
                powerName,
                mode => MidsContext.Character?.ActivePlannerModes.Contains(mode) == true,
                powerFullName => MidsContext.Character?.PlannerStateStacks.TryGetValue(powerFullName, out var stacks) == true ? stacks : 0,
                MidsContext.Character?.IsStalker == true,
                out var plannerActive))
        {
            return plannerActive;
        }

        return BuildOwnsPower(MidsContext.Character?.CurrentBuild, powerName);
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
               !string.IsNullOrWhiteSpace(row.Value) &&
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

    private static bool EvaluateRowForPower(
        AdvancedConditionRow row,
        Build? build,
        BuildConditionSnapshot snapshot,
        bool includeSourceModes)
    {
        if (row.EvaluationMode != AdvancedConditionEvaluationMode.BuildEvaluated)
        {
            return false;
        }

        if (row.Kind == AdvancedConditionKind.SourceMode &&
            !includeSourceModes &&
            !PlannerModeMapper.TryGetPlannerMode(OmniModeMapper.Normalize(row.Subject), out _))
        {
            return true;
        }

        var result = row.Kind switch
        {
            AdvancedConditionKind.SourceOwnPower or AdvancedConditionKind.PowerTaken => EvaluateOwnPower(row.Subject, build, snapshot),
            AdvancedConditionKind.PowerActive => EvaluatePowerActive(row, build),
            AdvancedConditionKind.SourceMode => EvaluateSourceMode(row, snapshot),
            AdvancedConditionKind.CombatSetting => EvaluateCombatSetting(row),
            AdvancedConditionKind.TargetEntityType => EvaluateTargetEntityType(row),
            AdvancedConditionKind.TargetGroup => EvaluateTargetGroup(row),
            AdvancedConditionKind.TargetArchetype => EvaluateTargetArchetype(row),
            AdvancedConditionKind.CharacterLevel => CompareNumber(MidsContext.Character?.Level ?? 0, row.Operator, ParseNumber(row.Value)),
            AdvancedConditionKind.PowerCount => EvaluateOwnedPowerCount(row, build, snapshot),
            AdvancedConditionKind.PowerStacks => EvaluatePowerStacks(row, build, snapshot),
            AdvancedConditionKind.BooleanLiteral => EvaluateBooleanLiteral(row),
            AdvancedConditionKind.AdvancedExpression => EvaluateAdvancedExpression(row),
            _ => EvaluateAdvancedExpression(new AdvancedConditionRow { Unsupported = true })
        };

        return row.Negated ? !result : result;
    }

    private static bool EvaluatePowerActive(AdvancedConditionRow row, Build? build)
    {
        var power = DatabaseAPI.GetPowerByFullName(row.Subject);
        var expected = ParseBool(row.Value);
        if (power == null || build?.Powers == null)
        {
            return CompareBool(false, row.Operator, expected);
        }

        var entry = build.Powers.FirstOrDefault(candidate => candidate?.Power?.PowerIndex == power.PowerIndex);
        var actual = entry?.StatInclude == true;
        return CompareBool(actual, row.Operator, expected);
    }

    private static bool EvaluateOwnPower(string powerName, Build? build, BuildConditionSnapshot snapshot)
    {
        if (PlannerStateCatalog.TryEvaluateOwnPowerState(
                powerName,
                snapshot.IsPlannerModeActive,
                snapshot.GetStacks,
                MidsContext.Character?.IsStalker == true,
                out var plannerActive))
        {
            return plannerActive;
        }

        return BuildOwnsPower(build, powerName);
    }

    private static bool EvaluatePowerStacks(AdvancedConditionRow row, Build? build, BuildConditionSnapshot snapshot)
    {
        var power = DatabaseAPI.GetPowerByFullName(row.Subject);
        if (power == null)
        {
            return false;
        }

        var stacks = snapshot.GetStacks(power.FullName);
        if (stacks == 0 && build?.Powers != null)
        {
            var pe = build.Powers.FirstOrDefault(entry => entry?.Power?.StaticIndex == power.StaticIndex);
            stacks = Math.Max(power.Stacks, pe?.VariableValue ?? 0);
            if (pe is { StatInclude: false } && PlannerStateCatalog.IsVariablePlannerPower(power.FullName))
            {
                stacks = 0;
            }
        }

        return CompareNumber(stacks, row.Operator, ParseNumber(row.Value));
    }

    private static bool EvaluateOwnedPowerCount(AdvancedConditionRow row, Build? build, BuildConditionSnapshot snapshot)
    {
        if (PlannerStateCatalog.TryEvaluatePlannerPowerCount(
                row.Subject,
                snapshot.IsPlannerModeActive,
                snapshot.GetStacks,
                MidsContext.Character?.IsStalker == true,
                out var plannerCount))
        {
            return CompareNumber(plannerCount, row.Operator, ParseNumber(row.Value));
        }

        if (build?.Powers == null)
        {
            return false;
        }

        var prefix = (row.Subject ?? string.Empty).Trim().Trim('.');
        if (string.IsNullOrWhiteSpace(prefix))
        {
            return false;
        }

        var count = CountOwnedPowers(build, prefix);

        return CompareNumber(count, row.Operator, ParseNumber(row.Value));
    }

    private static bool EvaluateSourceMode(AdvancedConditionRow row, BuildConditionSnapshot snapshot)
    {
        var mode = OmniModeMapper.Normalize(row.Subject);
        if (PlannerModeMapper.TryGetPlannerMode(mode, out var plannerMode))
        {
            return snapshot.IsPlannerModeActive(plannerMode);
        }

        return snapshot.IsSourceModeActive(mode);
    }

    private static bool EvaluateBooleanLiteral(AdvancedConditionRow row)
    {
        return ParseBool(row.Value);
    }

    private static bool EvaluatePowerStacks(AdvancedConditionRow row)
    {
        var power = DatabaseAPI.GetPowerByFullName(row.Subject);
        if (power == null)
        {
            return false;
        }

        if (MidsContext.Character?.PlannerStateStacks.TryGetValue(power.FullName, out var plannerStacks) == true)
        {
            return CompareNumber(plannerStacks, row.Operator, ParseNumber(row.Value));
        }

        var pe = MidsContext.Character?.CurrentBuild?.Powers
            .DefaultIfEmpty(null)
            .FirstOrDefault(e => e?.Power?.StaticIndex == power.StaticIndex);
        var stacks = Math.Max(power.Stacks, pe?.VariableValue ?? 0);
        if (pe is { StatInclude: false } &&
            PlannerStateCatalog.IsVariablePlannerPower(power.FullName))
        {
            stacks = 0;
        }

        return CompareNumber(stacks, row.Operator, ParseNumber(row.Value));
    }

    private static bool EvaluateOwnedPowerCount(AdvancedConditionRow row)
    {
        if (PlannerStateCatalog.TryEvaluatePlannerPowerCount(
                row.Subject,
                mode => MidsContext.Character?.ActivePlannerModes.Contains(mode) == true,
                powerFullName => MidsContext.Character?.PlannerStateStacks.TryGetValue(powerFullName, out var stacks) == true ? stacks : 0,
                MidsContext.Character?.IsStalker == true,
                out var plannerCount))
        {
            return CompareNumber(plannerCount, row.Operator, ParseNumber(row.Value));
        }

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

        var count = CountOwnedPowers(build, prefix);

        return CompareNumber(count, row.Operator, ParseNumber(row.Value));
    }

    private static bool BuildOwnsPower(Build? build, string? powerName)
    {
        return build?.OwnsPowerByFullName(powerName) == true;
    }

    private static int CountOwnedPowers(Build? build, string? powerNamePrefix)
    {
        return build?.CountOwnedPowersByFullNamePrefix(powerNamePrefix) ?? 0;
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
            if (MidsContext.Character?.ActivePlannerModes.Contains(plannerMode) == true)
            {
                return true;
            }

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
                PlannerMode.Insight => MidsContext.Character?.Insight == true,
                PlannerMode.Exhausted => MidsContext.Character?.Exhausted == true,
                PlannerMode.PerfectionLevel1 => MidsContext.Character?.ActivePerfectionLevel == 1,
                PlannerMode.PerfectionLevel2 => MidsContext.Character?.ActivePerfectionLevel == 2,
                PlannerMode.PerfectionLevel3 => MidsContext.Character?.ActivePerfectionLevel == 3,
                PlannerMode.PerfectionOfBody => MidsContext.Character?.IsStalker == true || MidsContext.Character?.PerfectionType == "body",
                PlannerMode.PerfectionOfBody1 => MidsContext.Character?.PerfectionOfBodyLevel == 1,
                PlannerMode.PerfectionOfBody2 => MidsContext.Character?.PerfectionOfBodyLevel == 2,
                PlannerMode.PerfectionOfBody3 => MidsContext.Character?.PerfectionOfBodyLevel == 3,
                PlannerMode.PerfectionOfMind => MidsContext.Character?.IsStalker != true && MidsContext.Character?.PerfectionType == "mind",
                PlannerMode.PerfectionOfMind1 => MidsContext.Character?.PerfectionOfMindLevel == 1,
                PlannerMode.PerfectionOfMind2 => MidsContext.Character?.PerfectionOfMindLevel == 2,
                PlannerMode.PerfectionOfMind3 => MidsContext.Character?.PerfectionOfMindLevel == 3,
                PlannerMode.PerfectionOfSoul => MidsContext.Character?.IsStalker != true && MidsContext.Character?.PerfectionType == "soul",
                PlannerMode.PerfectionOfSoul1 => MidsContext.Character?.PerfectionOfSoulLevel == 1,
                PlannerMode.PerfectionOfSoul2 => MidsContext.Character?.PerfectionOfSoulLevel == 2,
                PlannerMode.PerfectionOfSoul3 => MidsContext.Character?.PerfectionOfSoulLevel == 3,
                PlannerMode.PackMentality => MidsContext.Character?.PackMentality == true,
                _ => false
            };
        }

        return MidsContext.Character != null &&
               OmniModeMapper.ModeMatches(
                   mode,
                   MidsContext.Character.ActiveSourceModes,
                   MidsContext.Character.ActiveSourceModeFlags);
    }

    private static bool EvaluateTargetEntityType(AdvancedConditionRow row)
    {
        var actualScope = MidsContext.Config?.Inc.DisablePvE == true
            ? AdvancedConditionTargetScope.Player
            : AdvancedConditionTargetScope.Foe;

        if (row.TargetScope != AdvancedConditionTargetScope.Unknown)
        {
            return row.Operator switch
            {
                AdvancedConditionOperator.NotEquals => actualScope != row.TargetScope,
                _ => actualScope == row.TargetScope
            };
        }

        var value = row.Value.Trim('\'', '"').ToLowerInvariant();
        var actual = actualScope == AdvancedConditionTargetScope.Player ? "player" : "critter";

        return CompareString(actual, row.Operator, value);
    }

    private static bool TryEvaluatePreservedRuntimeRow(IEffect effect, AdvancedConditionRow row, out bool result)
    {
        result = false;

        if (row.Kind == AdvancedConditionKind.TargetEntityType &&
            CanResolvePlannerTargetScope(row.TargetScope, row.Value))
        {
            result = EvaluateTargetEntityType(row);
            return true;
        }

        if (row.Kind != AdvancedConditionKind.AdvancedExpression)
        {
            return false;
        }

        if (!TryResolvePreservedTargetScope(row.RawExpression, out var scope) ||
            !CanResolvePlannerTargetScope(scope, row.RawExpression))
        {
            return false;
        }

        result = MatchesCurrentPlannerTargetScope(scope);
        return true;
    }

    internal static bool TryDescribePreservedTargetExpression(string? rawExpression, out string description)
    {
        description = string.Empty;
        if (!TryResolvePreservedTargetScope(rawExpression, out var scope))
        {
            return false;
        }

        description = scope switch
        {
            AdvancedConditionTargetScope.Self => "Target is Self",
            AdvancedConditionTargetScope.Pet => "Target is Pet",
            AdvancedConditionTargetScope.Player => "Target is Player",
            AdvancedConditionTargetScope.Ally => "Target is Ally",
            AdvancedConditionTargetScope.Foe => "Target is Foe",
            _ => string.Empty
        };

        return !string.IsNullOrWhiteSpace(description);
    }

    private static bool TryResolvePreservedTargetScope(string? rawExpression, out AdvancedConditionTargetScope scope)
    {
        scope = AdvancedConditionTargetScope.Unknown;
        if (string.IsNullOrWhiteSpace(rawExpression))
        {
            return false;
        }

        var normalized = NormalizeRuntimeExpression(rawExpression);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return false;
        }

        if (normalized.Contains("target>entref eq source>entref", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("entref target> entref source> eq", StringComparison.OrdinalIgnoreCase))
        {
            scope = AdvancedConditionTargetScope.Self;
            return true;
        }

        if (ContainsTargetEntityScope(normalized, "player", "pc"))
        {
            scope = AdvancedConditionTargetScope.Player;
            return true;
        }

        if (ContainsTargetEntityScope(normalized, "critter", "npc", "foe", "enemy"))
        {
            scope = AdvancedConditionTargetScope.Foe;
            return true;
        }

        if (ContainsTargetEntityScope(normalized, "pet", "pets", "henchman"))
        {
            scope = AdvancedConditionTargetScope.Pet;
            return true;
        }

        if (normalized.Contains("target.isfriend? !", StringComparison.OrdinalIgnoreCase))
        {
            scope = AdvancedConditionTargetScope.Foe;
            return true;
        }

        if (normalized.Contains("target.isfriend?", StringComparison.OrdinalIgnoreCase))
        {
            scope = AdvancedConditionTargetScope.Ally;
            return true;
        }

        return false;
    }

    private static bool CanResolvePlannerTargetScope(AdvancedConditionTargetScope scope, string? value)
    {
        return scope switch
        {
            AdvancedConditionTargetScope.Player => true,
            AdvancedConditionTargetScope.Foe => true,
            _ => scope == AdvancedConditionTargetScope.Unknown &&
                 !string.IsNullOrWhiteSpace(value) &&
                 (value.Contains("player", StringComparison.OrdinalIgnoreCase) ||
                  value.Contains("pc", StringComparison.OrdinalIgnoreCase) ||
                  value.Contains("critter", StringComparison.OrdinalIgnoreCase) ||
                  value.Contains("npc", StringComparison.OrdinalIgnoreCase) ||
                  value.Contains("foe", StringComparison.OrdinalIgnoreCase) ||
                  value.Contains("enemy", StringComparison.OrdinalIgnoreCase))
        };
    }

    private static bool MatchesCurrentPlannerTargetScope(AdvancedConditionTargetScope scope)
    {
        var actualScope = MidsContext.Config?.Inc.DisablePvE == true
            ? AdvancedConditionTargetScope.Player
            : AdvancedConditionTargetScope.Foe;

        return actualScope == scope;
    }

    private static bool ContainsTargetEntityScope(string normalized, params string[] candidates)
    {
        foreach (var candidate in candidates)
        {
            if (normalized.Contains($"target>enttype eq '{candidate}'", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains($"target>enttype eq \"{candidate}\"", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains($"target>enttype == '{candidate}'", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains($"target>enttype == \"{candidate}\"", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains($"enttype target> {candidate} eq", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains($"enttype target> {candidate} ==", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string NormalizeRuntimeExpression(string expression)
    {
        return Regex.Replace(expression.Trim(), @"\s+", " ");
    }

    private static bool EvaluateTargetGroup(AdvancedConditionRow row)
    {
        if (row.Subject.Equals("tag", StringComparison.OrdinalIgnoreCase))
        {
            var actual = CombatTargetProfiles.HasTag(
                MidsContext.Config?.CombatContextSettings.TargetSettings.ProfileId ?? (int)CombatTargetProfileId.Minion,
                row.Value);
            return CompareBool(actual, row.Operator, true);
        }

        return EvaluateUnsupportedTargetState(row);
    }

    private static bool EvaluateTargetArchetype(AdvancedConditionRow row)
    {
        var profileId = MidsContext.Config?.CombatContextSettings.TargetSettings.ProfileId ??
                        (int)CombatTargetProfileId.Minion;
        var matches = CombatTargetProfiles.MatchesClass(profileId, row.Value);
        return row.Operator switch
        {
            AdvancedConditionOperator.NotEquals => !matches,
            _ => matches
        };
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
        if (MidsContext.Config == null)
        {
            return 0;
        }

        if (MidsContext.Config.TeamRoster is { Count: > 0 })
        {
            var normalized = ConfigData.NormalizeTeammateArchetype(archetype);
            return MidsContext.Config.TeamRoster.Count(slot =>
                slot.InRange &&
                !string.IsNullOrWhiteSpace(slot.Archetype) &&
                slot.Archetype.Equals(normalized, StringComparison.OrdinalIgnoreCase));
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

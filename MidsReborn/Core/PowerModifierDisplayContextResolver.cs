using System;
using System.Collections.Generic;
using System.Linq;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

internal sealed class PowerModifierDisplayContextCandidate
{
    public PowerModifierDisplayContextCandidate(
        string className,
        string displayName,
        string sourceDescription,
        bool isEntityOwned,
        bool isCurrentCharacter,
        int sourceOrder)
    {
        ClassName = className;
        DisplayName = displayName;
        SourceDescription = sourceDescription;
        IsEntityOwned = isEntityOwned;
        IsCurrentCharacter = isCurrentCharacter;
        SourceOrder = sourceOrder;
    }

    public string ClassName { get; }
    public string DisplayName { get; }
    public string SourceDescription { get; }
    public bool IsEntityOwned { get; }
    public bool IsCurrentCharacter { get; }
    public int SourceOrder { get; }

    public override string ToString()
    {
        var classText = string.IsNullOrWhiteSpace(DisplayName) ||
                        DisplayName.Equals(ClassName, StringComparison.OrdinalIgnoreCase)
            ? ClassName
            : $"{DisplayName} ({ClassName})";

        return string.IsNullOrWhiteSpace(SourceDescription)
            ? classText
            : $"{SourceDescription}: {classText}";
    }
}

internal sealed class PowerModifierDisplayContextResult
{
    public PowerModifierDisplayContextResult(
        PowerModifierDisplayContextCandidate? selected,
        IReadOnlyList<PowerModifierDisplayContextCandidate> candidates,
        bool isAmbiguous,
        string sourceDescription)
    {
        Selected = selected;
        Candidates = candidates;
        IsAmbiguous = isAmbiguous;
        SourceDescription = sourceDescription;
    }

    public PowerModifierDisplayContextCandidate? Selected { get; }
    public IReadOnlyList<PowerModifierDisplayContextCandidate> Candidates { get; }
    public bool IsAmbiguous { get; }
    public string SourceDescription { get; }
    public string SelectedClassName => Selected?.ClassName ?? string.Empty;

    public string DisplayText
    {
        get
        {
            if (Selected == null)
            {
                return "Modifier context: unresolved";
            }

            var prefix = IsAmbiguous ? "Modifier context: ambiguous - " : "Modifier context: ";
            return prefix + Selected;
        }
    }
}

internal static class PowerModifierDisplayContextResolver
{
    public static PowerModifierDisplayContextResult Resolve(IPower? power, string selectedClassName = "")
    {
        if (power == null)
        {
            return CreateCurrentCharacterResult();
        }

        if (!string.IsNullOrWhiteSpace(power.ForcedClass))
        {
            return CreateFixedResult(power.ForcedClass, "Forced class");
        }

        if (power is Power { OmniDisplayClassName: { Length: > 0 } displayClassName } &&
            !string.IsNullOrWhiteSpace(displayClassName))
        {
            return CreateFixedResult(displayClassName, "Display class");
        }

        var entityCandidates = GetEntityCandidates(power).ToArray();
        if (entityCandidates.Length > 0)
        {
            return SelectFromCandidates(power, entityCandidates, selectedClassName, "Entity-owned power");
        }

        var archetypeCandidates = GetArchetypeCandidates(power).ToArray();
        if (archetypeCandidates.Length > 0)
        {
            return SelectFromCandidates(power, archetypeCandidates, selectedClassName, "Archetype-owned power");
        }

        return CreateCurrentCharacterResult();
    }

    public static Power CreateContextualPowerClone(IPower sourcePower, string className)
    {
        var clone = new Power(sourcePower);
        if (!string.IsNullOrWhiteSpace(className))
        {
            clone.OmniDisplayClassName = className;
        }

        return clone;
    }

    public static IEffect CreateContextualEffectClone(IEffect sourceEffect, IPower? sourcePower, string className)
    {
        var clone = (IEffect)sourceEffect.Clone();
        if (sourcePower != null && !string.IsNullOrWhiteSpace(className))
        {
            clone.SetPower(CreateContextualPowerClone(sourcePower, className));
        }

        return clone;
    }

    private static PowerModifierDisplayContextResult CreateFixedResult(string className, string sourceDescription)
    {
        var candidate = CreateCandidate(className, sourceDescription, false, false, 0, allowMissingClassTable: true);
        return new PowerModifierDisplayContextResult(
            candidate,
            candidate == null ? Array.Empty<PowerModifierDisplayContextCandidate>() : new[] { candidate },
            false,
            sourceDescription);
    }

    private static PowerModifierDisplayContextResult CreateCurrentCharacterResult()
    {
        var className = MidsContext.Character?.Archetype?.ClassName ??
                        MidsContext.Archetype?.ClassName ??
                        string.Empty;
        var candidate = CreateCandidate(className, "Current character", false, true, 0, allowMissingClassTable: true);
        return new PowerModifierDisplayContextResult(
            candidate,
            candidate == null ? Array.Empty<PowerModifierDisplayContextCandidate>() : new[] { candidate },
            false,
            "Current character");
    }

    private static PowerModifierDisplayContextResult SelectFromCandidates(
        IPower power,
        IReadOnlyList<PowerModifierDisplayContextCandidate> candidates,
        string selectedClassName,
        string sourceDescription)
    {
        var ordered = candidates
            .OrderBy(candidate => candidate.SourceOrder)
            .ThenBy(candidate => candidate.ClassName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (ordered.Length <= 1 || HaveEquivalentModifiers(power, ordered))
        {
            return new PowerModifierDisplayContextResult(ordered.FirstOrDefault(), ordered.Take(1).ToArray(), false, sourceDescription);
        }

        var selected = ordered.FirstOrDefault(candidate =>
                           candidate.ClassName.Equals(selectedClassName, StringComparison.OrdinalIgnoreCase)) ??
                       ordered[0];

        return new PowerModifierDisplayContextResult(selected, ordered, true, sourceDescription);
    }

    private static IEnumerable<PowerModifierDisplayContextCandidate> GetEntityCandidates(IPower power)
    {
        var powerSetFullName = GetPowerSetFullName(power);
        if (string.IsNullOrWhiteSpace(powerSetFullName))
        {
            yield break;
        }

        var matches = DatabaseAPI.Database.Entities
            .Select((entity, index) => new { Entity = entity, Index = index })
            .Where(pair => pair.Entity != null &&
                           pair.Entity.PowersetFullName.Any(powerset =>
                               powerset.Equals(powerSetFullName, StringComparison.OrdinalIgnoreCase)))
            .GroupBy(pair => pair.Entity.ClassName, StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group.Min(pair => pair.Index));

        foreach (var group in matches)
        {
            var sourceOrder = group.Min(pair => pair.Index);
            var entityNames = group
                .Select(pair => pair.Entity.DisplayName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(3)
                .ToArray();
            var extraCount = group
                .Select(pair => pair.Entity.DisplayName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() - entityNames.Length;
            var source = entityNames.Length switch
            {
                0 => "Entity",
                _ when extraCount > 0 => $"{string.Join(", ", entityNames)} +{extraCount}",
                _ => string.Join(", ", entityNames)
            };

            var candidate = CreateCandidate(group.Key, source, true, false, sourceOrder, allowMissingClassTable: false);
            if (candidate != null)
            {
                yield return candidate;
            }
        }
    }

    private static IEnumerable<PowerModifierDisplayContextCandidate> GetArchetypeCandidates(IPower power)
    {
        var powerset = GetPowerSet(power);
        if (!string.IsNullOrWhiteSpace(powerset?.ATClass))
        {
            var candidate = CreateCandidate(powerset.ATClass, "Powerset archetype", false, false, 0, allowMissingClassTable: false);
            if (candidate != null)
            {
                yield return candidate;
                yield break;
            }
        }

        if (powerset is { nArchetype: >= 0 })
        {
            var candidate = CreateCandidate(
                DatabaseAPI.UidFromNidClass(powerset.nArchetype),
                "Powerset archetype",
                false,
                false,
                0,
                allowMissingClassTable: false);
            if (candidate != null)
            {
                yield return candidate;
                yield break;
            }
        }

        var requiredClasses = power.AdvancedRequirements.GetIncludedClassNames();
        if (requiredClasses.Count == 1)
        {
            var candidate = CreateCandidate(requiredClasses[0], "Single class requirement", false, false, 0, allowMissingClassTable: false);
            if (candidate != null)
            {
                yield return candidate;
            }
        }
    }

    private static PowerModifierDisplayContextCandidate? CreateCandidate(
        string className,
        string sourceDescription,
        bool isEntityOwned,
        bool isCurrentCharacter,
        int sourceOrder,
        bool allowMissingClassTable)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            return null;
        }

        var displayName = className;
        if (DatabaseAPI.TryGetClassAttributeTable(className, out var classAttributes))
        {
            displayName = string.IsNullOrWhiteSpace(classAttributes.DisplayName)
                ? className
                : classAttributes.DisplayName;
        }
        else if (!allowMissingClassTable)
        {
            return null;
        }
        else if (DatabaseAPI.GetArchetypeByClassName(className) is { } archetype &&
                 !string.IsNullOrWhiteSpace(archetype.DisplayName))
        {
            displayName = archetype.DisplayName;
        }

        return new PowerModifierDisplayContextCandidate(
            className,
            displayName,
            sourceDescription,
            isEntityOwned,
            isCurrentCharacter,
            sourceOrder);
    }

    private static bool HaveEquivalentModifiers(IPower power, IReadOnlyList<PowerModifierDisplayContextCandidate> candidates)
    {
        if (candidates.Count <= 1)
        {
            return true;
        }

        var tables = power.Effects
            .Select(effect => effect.ModifierTable)
            .Where(table => !string.IsNullOrWhiteSpace(table))
            .Select(DatabaseAPI.NormalizeModifierTableName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (tables.Length == 0)
        {
            return true;
        }

        var reference = GetModifierValues(candidates[0].ClassName, tables);
        return candidates
            .Skip(1)
            .All(candidate => ValuesEqual(reference, GetModifierValues(candidate.ClassName, tables)));
    }

    private static float[] GetModifierValues(string className, IReadOnlyList<string> tables)
    {
        var level = MidsContext.MathLevelBase;
        return tables
            .Select(table =>
            {
                DatabaseAPI.TryGetClassModifier(className, table, level, out var value);
                return value;
            })
            .ToArray();
    }

    private static bool ValuesEqual(IReadOnlyList<float> left, IReadOnlyList<float> right)
    {
        return left.Count == right.Count &&
               left.Zip(right, (l, r) => Math.Abs(l - r) < 0.00001f).All(equal => equal);
    }

    private static string GetPowerSetFullName(IPower power)
    {
        if (!string.IsNullOrWhiteSpace(power.GroupName) && !string.IsNullOrWhiteSpace(power.SetName))
        {
            return $"{power.GroupName}.{power.SetName}";
        }

        return power.GetPowerSet()?.FullName ?? string.Empty;
    }

    private static IPowerset? GetPowerSet(IPower power)
    {
        var powerSetFullName = GetPowerSetFullName(power);
        if (!string.IsNullOrWhiteSpace(powerSetFullName))
        {
            return DatabaseAPI.Database.Powersets.FirstOrDefault(powerset =>
                powerset != null && powerset.FullName.Equals(powerSetFullName, StringComparison.OrdinalIgnoreCase));
        }

        return power.GetPowerSet();
    }
}

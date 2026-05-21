using System;
using System.Collections.Generic;
using System.Linq;

namespace Mids_Reborn.Core;

public enum EnhancementPolicyAxis
{
    Radius,
    Arc,
    TimeToRoot
}

public static class EnhancementPolicyAxes
{
    public static EnhancementPolicyAxis[] Normalize(IEnumerable<EnhancementPolicyAxis>? axes)
    {
        return axes?
            .Distinct()
            .OrderBy(axis => axis)
            .ToArray() ?? [];
    }

    public static bool TryParse(string? value, out EnhancementPolicyAxis axis)
    {
        axis = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return NormalizeLookupKey(value) switch
        {
            "radius" => TryAssign(EnhancementPolicyAxis.Radius, out axis),
            "arc" => TryAssign(EnhancementPolicyAxis.Arc, out axis),
            "timetoroot" or "roottime" => TryAssign(EnhancementPolicyAxis.TimeToRoot, out axis),
            _ => false
        };
    }

    public static EnhancementPolicyAxis[] Deserialize(IEnumerable<string>? serialized)
    {
        return Normalize(serialized?
            .Select(value => TryParse(value, out var axis) ? axis : (EnhancementPolicyAxis?)null)
            .Where(axis => axis.HasValue)
            .Select(axis => axis!.Value));
    }

    public static string[] Serialize(IEnumerable<EnhancementPolicyAxis>? axes)
    {
        return Normalize(axes)
            .Select(axis => axis.ToString())
            .ToArray();
    }

    public static bool AllowsAxis(IEnumerable<EnhancementPolicyAxis>? blockedAxes, EnhancementPolicyAxis axis)
    {
        if (blockedAxes == null)
        {
            return true;
        }

        return blockedAxes.All(candidate => candidate != axis);
    }

    public static string GetDisplayName(EnhancementPolicyAxis axis)
    {
        return axis switch
        {
            EnhancementPolicyAxis.TimeToRoot => "Root Time",
            _ => axis.ToString()
        };
    }

    public static string[] BuildPolicyNotes(IPower power)
    {
        var notes = new List<string>();

        notes.AddRange(power.IgnoreEnh.Select(enhance =>
            $"{FormatEnhanceDisplayName(enhance)} ignored from slotted enhancement scaling"));
        notes.AddRange(power.IgnoreEnhancementAxes.Select(axis =>
            $"{GetDisplayName(axis)} ignored from slotted enhancement scaling"));
        notes.AddRange(power.Ignore_Buff.Select(enhance =>
            $"{FormatEnhanceDisplayName(enhance)} ignored from global buff scaling"));
        notes.AddRange(power.IgnoreBuffEnhancementAxes.Select(axis =>
            $"{GetDisplayName(axis)} ignored from global buff scaling"));

        return notes
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static string BuildPolicySummary(IPower power)
    {
        return string.Join("; ", BuildPolicyNotes(power));
    }

    public static string BuildPolicyTooltip(IPower power)
    {
        var notes = BuildPolicyNotes(power);
        return notes.Length == 0
            ? string.Empty
            : string.Join(Environment.NewLine, notes.Select(note => $"- {note}"));
    }

    public static string GetStatPolicyTooltip(IPower power, string statLabel)
    {
        var notes = new List<string>();
        switch (statLabel)
        {
            case "Range" when !power.IgnoreEnhancement(Enums.eEnhance.Range):
                notes.Add("Range ignored from slotted enhancement scaling.");
                break;

            case "Radius" when !power.IgnoreEnhancementAxis(EnhancementPolicyAxis.Radius):
                notes.Add("Radius ignored from slotted enhancement scaling.");
                notes.Add("Radius policy is preserved separately from Range so non-cone AoEs can block radius scaling without suppressing cast range.");
                break;

            case "Arc" when !power.IgnoreEnhancementAxis(EnhancementPolicyAxis.Arc):
                notes.Add("Arc ignored from slotted enhancement scaling.");
                notes.Add("Current planner/runtime has no separate slotted Arc source, so this is preserved as policy state and surfaced in the UI.");
                break;

            case "Interrupt" when !power.IgnoreEnhancement(Enums.eEnhance.Interrupt):
                notes.Add("Interrupt ignored from slotted enhancement scaling.");
                break;

            case "Root Time":
                if (!power.IgnoreEnhancementAxis(EnhancementPolicyAxis.TimeToRoot))
                {
                    notes.Add("Root Time ignored from slotted enhancement scaling.");
                }

                if (!power.IgnoreBuffAxis(EnhancementPolicyAxis.TimeToRoot))
                {
                    notes.Add("Root Time ignored from global buff scaling.");
                }

                if (notes.Count > 0)
                {
                    notes.Add("Current planner/runtime has no dedicated Root Time enhancement axis, so this is preserved as policy state and surfaced in the UI.");
                }

                break;
        }

        return notes.Count == 0
            ? string.Empty
            : string.Join(Environment.NewLine, notes.Select(note => $"- {note}"));
    }

    public static void ApplyInterruptEnhancement(IPower power, float delta)
    {
        if (power.IgnoreEnhancement(Enums.eEnhance.Interrupt))
        {
            power.InterruptTime += delta;
        }
    }

    public static void ApplyRangeEnhancement(IPower power, float delta)
    {
        var allowRange = power.IgnoreEnhancement(Enums.eEnhance.Range);
        if (allowRange)
        {
            power.Range += delta;
        }

        if (Math.Abs(power.Radius) <= float.Epsilon)
        {
            return;
        }

        var allowRadius = power.EffectArea == Enums.eEffectArea.Cone || power.Arc > 0
            ? allowRange
            : AllowsAxis(power.IgnoreEnhancementAxes, EnhancementPolicyAxis.Radius);

        if (allowRadius)
        {
            power.Radius += delta;
        }
    }

    private static bool TryAssign(EnhancementPolicyAxis axis, out EnhancementPolicyAxis assignedAxis)
    {
        assignedAxis = axis;
        return true;
    }

    private static string NormalizeLookupKey(string value)
    {
        return value
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Trim()
            .ToLowerInvariant();
    }

    private static string FormatEnhanceDisplayName(Enums.eEnhance enhance)
    {
        return enhance switch
        {
            Enums.eEnhance.EnduranceDiscount => "Endurance Discount",
            Enums.eEnhance.RechargeTime => "Recharge",
            _ => SplitPascalCase(enhance.ToString())
        };
    }

    private static string SplitPascalCase(string value)
    {
        return string.Concat(value.Select((character, index) =>
            index > 0 && char.IsUpper(character) && !char.IsUpper(value[index - 1])
                ? $" {character}"
                : character.ToString()));
    }
}

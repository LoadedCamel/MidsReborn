using System.Globalization;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core.PlannerRulesets;

internal enum ImportedStackMode
{
    Unknown = 0,
    Stack = 1,
    Ignore = 2,
    Extend = 3,
    Replace = 4,
    Overlap = 5,
    StackThenIgnore = 6,
    Refresh = 7,
    RefreshToCount = 8,
    Maximize = 9,
    Suppress = 10,
    Continuous = 11
}

internal enum ImportedCasterStackMode
{
    Unknown = 0,
    Individual = 1,
    Collective = 2
}

internal sealed record ImportedStackPolicy(
    ImportedStackMode Mode,
    ImportedCasterStackMode CasterMode,
    int StackLimit,
    string StackKey,
    string[] SuppressEvents,
    string RawStack,
    string RawCasterStack,
    bool HasImportedMetadata)
{
    public static readonly ImportedStackPolicy Default = new(
        ImportedStackMode.Unknown,
        ImportedCasterStackMode.Unknown,
        0,
        string.Empty,
        [],
        string.Empty,
        string.Empty,
        HasImportedMetadata: false);
}

internal static class ImportedStackPolicyNormalizer
{
    public static ImportedStackPolicy FromTemplate(OmniEffectTemplate template)
    {
        var suppressEvents = template.SuppressEvents
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var stackKey = (template.StackKey ?? string.Empty).Trim();
        var rawStack = (template.Stack ?? string.Empty).Trim();
        var rawCasterStack = (template.CasterStack ?? string.Empty).Trim();
        var stackLimit = template.StackLimit;
        var hasImportedMetadata =
            !string.IsNullOrWhiteSpace(rawStack) ||
            !string.IsNullOrWhiteSpace(rawCasterStack) ||
            stackLimit > 0 ||
            !string.IsNullOrWhiteSpace(stackKey) ||
            suppressEvents.Length > 0;

        if (!hasImportedMetadata)
        {
            return ImportedStackPolicy.Default;
        }

        return new ImportedStackPolicy(
            ParseMode(rawStack),
            ParseCasterMode(rawCasterStack),
            stackLimit > 0 ? stackLimit : 0,
            stackKey,
            suppressEvents,
            rawStack,
            rawCasterStack,
            HasImportedMetadata: true);
    }

    public static Enums.eStacking ToCompatibilityStacking(ImportedStackPolicy policy)
    {
        return policy.HasImportedMetadata && policy.Mode == ImportedStackMode.Stack
            ? Enums.eStacking.Yes
            : Enums.eStacking.No;
    }

    private static ImportedStackMode ParseMode(string? value)
    {
        var normalized = Normalize(value);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return ImportedStackMode.Unknown;
        }

        return normalized switch
        {
            "stack" => ImportedStackMode.Stack,
            "ignore" => ImportedStackMode.Ignore,
            "extend" => ImportedStackMode.Extend,
            "replace" => ImportedStackMode.Replace,
            "overlap" => ImportedStackMode.Overlap,
            "stackthenignore" => ImportedStackMode.StackThenIgnore,
            "refresh" => ImportedStackMode.Refresh,
            "refreshtocount" => ImportedStackMode.RefreshToCount,
            "maximize" => ImportedStackMode.Maximize,
            "suppress" => ImportedStackMode.Suppress,
            "continuous" => ImportedStackMode.Continuous,
            _ => ImportedStackMode.Unknown
        };
    }

    private static ImportedCasterStackMode ParseCasterMode(string? value)
    {
        var normalized = Normalize(value);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return ImportedCasterStackMode.Unknown;
        }

        return normalized switch
        {
            "individual" => ImportedCasterStackMode.Individual,
            "collective" => ImportedCasterStackMode.Collective,
            _ => ImportedCasterStackMode.Unknown
        };
    }

    private static string Normalize(string? value)
    {
        return (value ?? string.Empty)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Trim()
            .ToLowerInvariant();
    }
}

internal static class PlannerStackRules
{
    public static ImportedStackPolicy GetImportedPolicy(IEffect effect)
    {
        return effect is Effect concreteEffect
            ? concreteEffect.StackPolicy
            : ImportedStackPolicy.Default;
    }

    public static bool HasExplicitImportedPolicy(IEffect effect)
    {
        return GetImportedPolicy(effect).HasImportedMetadata;
    }

    public static bool SupportsVariableCopyScaling(IEffect effect)
    {
        if (!HasExplicitImportedPolicy(effect))
        {
            return effect.Stacking == Enums.eStacking.Yes;
        }

        return GetPlannerVisibleCopyCount(effect, 2) > 1;
    }

    public static int GetPlannerVisibleCopyCount(IEffect effect, int requestedCopies)
    {
        var safeRequestedCopies = Math.Max(1, requestedCopies);
        var policy = GetImportedPolicy(effect);
        if (!policy.HasImportedMetadata)
        {
            return effect.Stacking == Enums.eStacking.Yes
                ? safeRequestedCopies
                : 1;
        }

        return policy.Mode switch
        {
            ImportedStackMode.Stack or ImportedStackMode.Continuous or ImportedStackMode.Refresh => safeRequestedCopies,
            ImportedStackMode.StackThenIgnore or ImportedStackMode.RefreshToCount => Math.Min(
                safeRequestedCopies,
                policy.StackLimit > 0 ? policy.StackLimit : 1),
            _ => 1
        };
    }

    public static bool ShouldUseGreatestMagnitudeOnly(IEffect effect)
    {
        if (!HasExplicitImportedPolicy(effect))
        {
            return false;
        }

        var mode = GetImportedPolicy(effect).Mode;
        return mode is ImportedStackMode.Maximize or ImportedStackMode.Suppress;
    }

    public static bool IsSingleContributionForPlannerMath(IEffect effect)
    {
        return GetPlannerVisibleCopyCount(effect, 2) <= 1;
    }

    public static bool ShouldFlagNoStackSameCaster(IEffect effect)
    {
        if (!HasExplicitImportedPolicy(effect))
        {
            return effect.Stacking == Enums.eStacking.No;
        }

        var policy = GetImportedPolicy(effect);
        return policy.CasterMode == ImportedCasterStackMode.Collective || IsSingleContributionForPlannerMath(effect);
    }

    public static string GetDisplayGroupingKey(IEffect effect)
    {
        var policy = GetImportedPolicy(effect);
        if (!policy.HasImportedMetadata)
        {
            return $"legacy:{effect.Stacking}";
        }

        return string.Join("|",
            "imported",
            policy.Mode,
            policy.CasterMode,
            policy.StackLimit,
            policy.StackKey ?? string.Empty,
            string.Join(",", policy.SuppressEvents));
    }

    public static string GetDiagnosticDescription(IEffect effect)
    {
        var policy = GetImportedPolicy(effect);
        if (!policy.HasImportedMetadata)
        {
            return $"compat:{effect.Stacking}";
        }

        var values = new List<string>
        {
            policy.Mode.ToString()
        };
        if (policy.CasterMode != ImportedCasterStackMode.Unknown)
        {
            values.Add($"caster={policy.CasterMode}");
        }

        if (policy.StackLimit > 0)
        {
            values.Add($"limit={policy.StackLimit}");
        }

        if (!string.IsNullOrWhiteSpace(policy.StackKey))
        {
            values.Add($"key={policy.StackKey}");
        }

        if (policy.Mode == ImportedStackMode.Unknown && !string.IsNullOrWhiteSpace(policy.RawStack))
        {
            values.Add($"raw={policy.RawStack}");
        }

        if (policy.SuppressEvents.Length > 0)
        {
            values.Add($"suppress=[{string.Join(", ", policy.SuppressEvents)}]");
        }

        return string.Join(", ", values);
    }

    public static string GetDiagnosticPolicyKey(IEffect effect)
    {
        var policy = GetImportedPolicy(effect);
        if (!policy.HasImportedMetadata)
        {
            return $"legacy:{effect.Stacking}";
        }

        return string.Join("|",
            policy.Mode,
            policy.CasterMode,
            policy.StackLimit,
            policy.StackKey ?? string.Empty,
            policy.RawStack ?? string.Empty,
            policy.RawCasterStack ?? string.Empty,
            string.Join(",", policy.SuppressEvents));
    }

    public static bool ShouldFlagDuplicateLookingGroup(IReadOnlyCollection<IEffect> effects)
    {
        if (effects.Count <= 1)
        {
            return false;
        }

        var first = effects.First();
        var policy = GetImportedPolicy(first);
        return !policy.HasImportedMetadata || policy.Mode == ImportedStackMode.Unknown;
    }

    public static IReadOnlyList<IEffect> ReducePlannerVisibleCopies(IReadOnlyList<IEffect> effects)
    {
        if (effects.Count <= 1)
        {
            return effects;
        }

        var indexedEffects = effects
            .Select((effect, index) => new IndexedEffect(index, effect))
            .ToArray();
        var keptIndexes = new HashSet<int>();

        foreach (var group in indexedEffects.GroupBy(item => GetPlannerContributionGroupKey(item.Effect), StringComparer.OrdinalIgnoreCase))
        {
            var items = group.OrderBy(item => item.Index).ToArray();
            var first = items[0].Effect;
            var policy = GetImportedPolicy(first);
            if (!policy.HasImportedMetadata)
            {
                foreach (var legacyGroup in items.GroupBy(item => GetPlannerExactKey(item.Effect), StringComparer.OrdinalIgnoreCase))
                {
                    keptIndexes.Add(legacyGroup.First().Index);
                }

                continue;
            }

            switch (policy.Mode)
            {
                case ImportedStackMode.Stack:
                case ImportedStackMode.Continuous:
                case ImportedStackMode.Refresh:
                    foreach (var item in items)
                    {
                        keptIndexes.Add(item.Index);
                    }
                    break;

                case ImportedStackMode.StackThenIgnore:
                case ImportedStackMode.RefreshToCount:
                    var limit = policy.StackLimit > 0 ? policy.StackLimit : 1;
                    foreach (var item in items.Take(limit))
                    {
                        keptIndexes.Add(item.Index);
                    }
                    break;

                case ImportedStackMode.Maximize:
                case ImportedStackMode.Suppress:
                    keptIndexes.Add(SelectGreatestMagnitude(items).Index);
                    break;

                default:
                    keptIndexes.Add(items[0].Index);
                    break;
            }
        }

        return indexedEffects
            .Where(item => keptIndexes.Contains(item.Index))
            .OrderBy(item => item.Index)
            .Select(item => item.Effect)
            .ToArray();
    }

    public static string GetPlannerContributionGroupKey(IEffect effect)
    {
        return string.Join("|",
            effect.EffectType,
            effect.ToWho,
            effect.PvMode,
            effect.AttribType,
            effect.Aspect,
            effect.DamageType,
            effect.MezType,
            effect.ModifierTable ?? string.Empty,
            Format(effect.nDuration),
            Format(effect.DelayedTime),
            Format(effect.BaseProbability),
            Format(effect.ProcsPerMinute),
            Format(effect.Ticks),
            Format(effect.Absorbed_Interval),
            effect.EffectClass,
            effect.Absorbed_EffectID,
            effect.Absorbed_Power_nID,
            effect.Absorbed_Class_nID,
            effect.OmniSource ?? string.Empty,
            effect.EffectId ?? string.Empty,
            effect.Summon ?? string.Empty,
            effect.Override ?? string.Empty,
            effect.Reward ?? string.Empty,
            effect.ModeName ?? string.Empty,
            NormalizeConditionSet(effect.AdvancedConditions),
            string.Join(",", GetNormalizedTags(effect)),
            GetDiagnosticPolicyKey(effect));
    }

    public static string GetPlannerExactKey(IEffect effect)
    {
        var recurrence = effect.PseudoPetRecurrence is { IsValid: true } info
            ? $"{info.EntityName}|{info.PetPowerName}|{Format(info.SourceUsageTime)}|{Format(info.SourceActivatePeriod)}|{Format(info.EntCreateDuration)}|{Format(info.PetTickInterval)}|{info.SpawnCount}|{info.TicksPerSpawn}|{info.TotalExpectedTicks}"
            : string.Empty;

        return string.Join("|",
            GetPlannerContributionGroupKey(effect),
            Format(effect.Scale),
            Format(effect.nMagnitude),
            Format(effect.Math_Mag),
            Format(effect.Math_Duration),
            recurrence);
    }

    private static IndexedEffect SelectGreatestMagnitude(IReadOnlyList<IndexedEffect> effects)
    {
        return effects
            .OrderByDescending(item => Math.Abs(item.Effect.BuffedMag))
            .ThenByDescending(item => Math.Abs(item.Effect.Duration))
            .ThenBy(item => item.Index)
            .First();
    }

    private static string NormalizeConditionSet(AdvancedConditionSet? conditions)
    {
        if (conditions == null || conditions.Rows.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(";",
            conditions.Rows
                .Select(row => $"{row.EvaluationMode}|{row.Kind}|{row.Link}|{row.Negated}|{AdvancedConditionCompiler.Compile(row)}")
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase));
    }

    private static IEnumerable<string> GetNormalizedTags(IEffect effect)
    {
        return effect.EffectTags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase);
    }

    private static string Format(float value)
    {
        return value.ToString("0.####", CultureInfo.InvariantCulture);
    }

    private sealed record IndexedEffect(int Index, IEffect Effect);
}

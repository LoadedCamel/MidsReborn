using System;
using System.Collections.Generic;
using System.Linq;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.PlannerRulesets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mids_Reborn.Core.Omni;

internal sealed class ImportedPowerLifetimeMetadata
{
    public static readonly ImportedPowerLifetimeMetadata Default = new(null, null);

    public ImportedPowerLifetimeMetadata(int? maxPowerLifetime, int? maxPowerLifetimeInGame)
    {
        MaxPowerLifetime = maxPowerLifetime;
        MaxPowerLifetimeInGame = maxPowerLifetimeInGame;
    }

    public int? MaxPowerLifetime { get; }
    public int? MaxPowerLifetimeInGame { get; }

    public bool HasValue => MaxPowerLifetime.HasValue || MaxPowerLifetimeInGame.HasValue;

    public ImportedPowerLifetimeMetadata Clone()
    {
        return new ImportedPowerLifetimeMetadata(MaxPowerLifetime, MaxPowerLifetimeInGame);
    }
}

internal sealed class ImportedBoostPolicyMetadata
{
    public static readonly ImportedBoostPolicyMetadata Default = new(string.Empty, []);

    public ImportedBoostPolicyMetadata(string rawBoostInfoJson, IReadOnlyList<string>? allowedBoostSetCategories)
    {
        RawBoostInfoJson = rawBoostInfoJson ?? string.Empty;
        AllowedBoostSetCategories = allowedBoostSetCategories?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];
    }

    public string RawBoostInfoJson { get; }
    public IReadOnlyList<string> AllowedBoostSetCategories { get; }

    public bool HasValue =>
        !string.IsNullOrWhiteSpace(RawBoostInfoJson) ||
        AllowedBoostSetCategories.Count > 0;

    public ImportedBoostPolicyMetadata Clone()
    {
        return new ImportedBoostPolicyMetadata(RawBoostInfoJson, AllowedBoostSetCategories);
    }
}

internal enum ImportedPowerPolicyDiagnosticCategory
{
    AppliedNow = 0,
    PreservedRuntimeOnly = 1,
    DeferredFutureMechanics = 2,
    UnknownInput = 3
}

internal sealed record ImportedPowerPolicyDiagnosticEntry(
    ImportedPowerPolicyDiagnosticCategory Category,
    string FieldName,
    string Detail);

internal static class ImportedPowerPolicyDiagnostics
{
    public static IReadOnlyList<ImportedPowerPolicyDiagnosticEntry> DescribeSource(OmniPowerDefinition power)
    {
        var entries = new List<ImportedPowerPolicyDiagnosticEntry>();
        AppendProcDiagnostics(entries,
            ImportedProcPolicyNormalizer.Normalize(
                power.ProcAllowedValue,
                power.ProcsOnlyOnMainTarget,
                power.ProcIgnoreChainEffect,
                power.ProcIgnoreOverCap,
                out var recognizedProcAllowed),
            recognizedProcAllowed,
            hasProcInput: HasJsonValue(power.ProcAllowedValue) ||
                          power.ProcsOnlyOnMainTarget ||
                          power.ProcIgnoreChainEffect ||
                          power.ProcIgnoreOverCap,
            profile: null);

        AppendLifetimeDiagnostics(entries, BuildLifetimeMetadata(power));
        AppendBoostDiagnostics(entries, BuildBoostPolicyMetadata(power));

        if (power.StackingLifetime != null)
        {
            entries.Add(new ImportedPowerPolicyDiagnosticEntry(
                ImportedPowerPolicyDiagnosticCategory.PreservedRuntimeOnly,
                "stacking_lifetime",
                power.StackingLifetime.Value.ToString()));
        }

        return entries;
    }

    public static IReadOnlyList<ImportedPowerPolicyDiagnosticEntry> DescribeRuntime(Power power, ServerRulesProfile? profile)
    {
        var entries = new List<ImportedPowerPolicyDiagnosticEntry>();
        AppendProcDiagnostics(entries,
            power.ProcPolicy,
            recognizedProcAllowed: power.ProcPolicy.Eligibility != ImportedProcEligibilityMode.Unknown,
            hasProcInput: !power.ProcPolicy.IsDefault,
            profile);
        AppendLifetimeDiagnostics(entries, power.OmniLifetimeMetadata);
        AppendBoostDiagnostics(entries, power.OmniBoostPolicy);

        if (power.OmniStackingLifetime != null)
        {
            entries.Add(new ImportedPowerPolicyDiagnosticEntry(
                ImportedPowerPolicyDiagnosticCategory.PreservedRuntimeOnly,
                "stacking_lifetime",
                power.OmniStackingLifetime.Value.ToString()));
        }

        return entries;
    }

    public static string FormatEntry(string ownerFullName, ImportedPowerPolicyDiagnosticEntry entry)
    {
        return $"{ownerFullName}: [{FormatCategory(entry.Category)}] {entry.FieldName} {entry.Detail}";
    }

    public static ImportedPowerLifetimeMetadata BuildLifetimeMetadata(OmniPowerDefinition power)
    {
        int? maxPowerLifetime = HasJsonValue(power.MaxPowerLifetimeValue)
            ? power.MaxPowerLifetime
            : null;
        int? maxPowerLifetimeInGame = HasJsonValue(power.MaxPowerLifetimeInGameValue)
            ? power.MaxPowerLifetimeInGame
            : null;
        return new ImportedPowerLifetimeMetadata(maxPowerLifetime, maxPowerLifetimeInGame);
    }

    public static ImportedBoostPolicyMetadata BuildBoostPolicyMetadata(OmniPowerDefinition power)
    {
        return new ImportedBoostPolicyMetadata(
            NormalizeJson(power.BoostInfo),
            power.AllowedBoostSetCats);
    }

    internal static string NormalizeJson(JToken? token)
    {
        return HasJsonValue(token)
            ? token!.ToString(Formatting.None)
            : string.Empty;
    }

    private static void AppendProcDiagnostics(
        ICollection<ImportedPowerPolicyDiagnosticEntry> target,
        ImportedProcPolicy policy,
        bool recognizedProcAllowed,
        bool hasProcInput,
        ServerRulesProfile? profile)
    {
        if (!hasProcInput)
        {
            return;
        }

        var category = recognizedProcAllowed
            ? ImportedPowerPolicyDiagnosticCategory.AppliedNow
            : ImportedPowerPolicyDiagnosticCategory.UnknownInput;
        var detail = $"eligibility={policy.Eligibility}, allowance={policy.Allowance}, main_target_only={policy.MainTargetOnly}, ignore_chain_effect={policy.IgnoreChainEffect}, ignore_over_cap={policy.IgnoreOverCap}";
        if (policy.IgnoreOverCap)
        {
            var overCapMode = profile?.ProcOverCapHandling.ToString() ?? ProcOverCapHandlingMode.PreservedOnly.ToString();
            detail += $", over_cap_mode={overCapMode}";
        }

        if (!recognizedProcAllowed && !string.IsNullOrWhiteSpace(policy.RawEligibilityValue))
        {
            detail += $", raw={policy.RawEligibilityValue}";
        }

        target.Add(new ImportedPowerPolicyDiagnosticEntry(category, "proc_allowed", detail));
    }

    private static void AppendLifetimeDiagnostics(
        ICollection<ImportedPowerPolicyDiagnosticEntry> target,
        ImportedPowerLifetimeMetadata metadata)
    {
        if (!metadata.HasValue)
        {
            return;
        }

        if (metadata.MaxPowerLifetime.HasValue)
        {
            target.Add(new ImportedPowerPolicyDiagnosticEntry(
                ImportedPowerPolicyDiagnosticCategory.PreservedRuntimeOnly,
                "max_power_lifetime",
                metadata.MaxPowerLifetime.Value.ToString()));
        }

        if (metadata.MaxPowerLifetimeInGame.HasValue)
        {
            target.Add(new ImportedPowerPolicyDiagnosticEntry(
                ImportedPowerPolicyDiagnosticCategory.PreservedRuntimeOnly,
                "max_power_lifetime_ingame",
                metadata.MaxPowerLifetimeInGame.Value.ToString()));
        }
    }

    private static void AppendBoostDiagnostics(
        ICollection<ImportedPowerPolicyDiagnosticEntry> target,
        ImportedBoostPolicyMetadata metadata)
    {
        if (!metadata.HasValue)
        {
            return;
        }

        if (metadata.AllowedBoostSetCategories.Count > 0)
        {
            target.Add(new ImportedPowerPolicyDiagnosticEntry(
                ImportedPowerPolicyDiagnosticCategory.DeferredFutureMechanics,
                "allowed_boostset_cats",
                string.Join(", ", metadata.AllowedBoostSetCategories)));
        }

        if (!string.IsNullOrWhiteSpace(metadata.RawBoostInfoJson))
        {
            target.Add(new ImportedPowerPolicyDiagnosticEntry(
                ImportedPowerPolicyDiagnosticCategory.DeferredFutureMechanics,
                "boost_info",
                metadata.RawBoostInfoJson));
        }
    }

    private static string FormatCategory(ImportedPowerPolicyDiagnosticCategory category)
    {
        return category switch
        {
            ImportedPowerPolicyDiagnosticCategory.AppliedNow => "applied-now",
            ImportedPowerPolicyDiagnosticCategory.PreservedRuntimeOnly => "preserved-runtime-only",
            ImportedPowerPolicyDiagnosticCategory.DeferredFutureMechanics => "deferred-future-mechanics",
            ImportedPowerPolicyDiagnosticCategory.UnknownInput => "unknown-input",
            _ => category.ToString()
        };
    }

    private static bool HasJsonValue(JToken? token)
    {
        return token != null && token.Type != JTokenType.Null;
    }
}

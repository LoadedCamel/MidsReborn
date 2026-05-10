using System;
using System.Collections.Generic;
using System.Linq;
using Mids_Reborn.Core.Base.Data_Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mids_Reborn.Core.PlannerRulesets;

internal enum ProcAllowanceMode
{
    Default = 0,
    Allow = 1,
    Deny = 2
}

internal enum ImportedProcEligibilityMode
{
    Default = 0,
    All = 1,
    PowerOnly = 2,
    GlobalOnly = 3,
    None = 4,
    Unknown = 5
}

internal enum ProcEligibilityInterpretationMode
{
    CompatibilityProjection = 0,
    OmniTypedEligibility = 1
}

internal enum ProcOverCapHandlingMode
{
    PreservedOnly = 0,
    PlannerAware = 1
}

[Flags]
internal enum ProcContributionFlags
{
    None = 0,
    InheritedFromProcWrapper = 1,
    ChildResultView = 2,
    ChainExpandedView = 4
}

internal sealed record ImportedProcPolicy(
    ImportedProcEligibilityMode Eligibility,
    ProcAllowanceMode Allowance,
    bool MainTargetOnly,
    bool IgnoreChainEffect,
    bool IgnoreOverCap,
    string RawEligibilityValue)
{
    public static readonly ImportedProcPolicy Default = new(
        ImportedProcEligibilityMode.Default,
        ProcAllowanceMode.Default,
        MainTargetOnly: false,
        IgnoreChainEffect: false,
        IgnoreOverCap: false,
        RawEligibilityValue: string.Empty);

    public bool IsDefault =>
        Eligibility == ImportedProcEligibilityMode.Default &&
        Allowance == ProcAllowanceMode.Default &&
        !MainTargetOnly &&
        !IgnoreChainEffect &&
        !IgnoreOverCap &&
        string.IsNullOrWhiteSpace(RawEligibilityValue);
}

internal sealed class ProcRulesPolicy
{
    private readonly IPlannerRuleset _ruleset;
    private readonly ProcEligibilityInterpretationMode _procEligibilityInterpretation;
    private readonly ProcOverCapHandlingMode _procOverCapHandling;
    private readonly bool _supportsPowerLocalChanceMods;

    public ProcRulesPolicy(
        IPlannerRuleset ruleset,
        ProcEligibilityInterpretationMode procEligibilityInterpretation,
        ProcOverCapHandlingMode procOverCapHandling,
        bool supportsPowerLocalChanceMods)
    {
        _ruleset = ruleset;
        _procEligibilityInterpretation = procEligibilityInterpretation;
        _procOverCapHandling = procOverCapHandling;
        _supportsPowerLocalChanceMods = supportsPowerLocalChanceMods;
    }

    public float GetMinProcChance(float procsPerMinute)
    {
        return _ruleset.GetMinProcChance(procsPerMinute);
    }

    public float GetMaxProcChance(float procsPerMinute)
    {
        return _ruleset.GetMaxProcChance(procsPerMinute);
    }

    public float EvaluateProbability(Character? character, IPower? ownerPower, IEffect procEffect, float baseProbability)
    {
        if (!ShouldIncludePlannerEffect(ownerPower, procEffect))
        {
            return 0f;
        }

        var probability = baseProbability;
        if (procEffect.ProcsPerMinute > 0f && ownerPower != null)
        {
            probability = _ruleset.CalculateProcProbability(ownerPower, procEffect.ProcsPerMinute, probability);
        }

        probability = ChanceModifierSupport.ApplyChanceModifiers(
            character,
            ownerPower,
            procEffect,
            probability,
            _supportsPowerLocalChanceMods);

        return Math.Clamp(probability, 0f, 1f);
    }

    public bool TryGetChanceModifierScale(Character? character, IPower? ownerPower, string chanceTag, out float scale)
    {
        return ChanceModifierSupport.TryGetChanceModifierScale(
            character,
            ownerPower,
            chanceTag,
            _supportsPowerLocalChanceMods,
            out scale);
    }

    public bool ShouldIncludePlannerEffect(IPower? ownerPower, IEffect effect)
    {
        if (!IsPlannerProcContributionEffect(effect))
        {
            return true;
        }

        var policy = GetProcPolicy(ownerPower);
        if (ShouldBlockPlannerLocalProcContribution(policy))
        {
            return false;
        }

        var contributionFlags = effect is Effect concreteEffect
            ? concreteEffect.ProcContributionFlags
            : ProcContributionFlags.None;

        if (policy.MainTargetOnly &&
            (effect.Absorbed_Effect ||
             contributionFlags.HasFlag(ProcContributionFlags.ChildResultView)))
        {
            return false;
        }

        if (policy.IgnoreChainEffect &&
            contributionFlags.HasFlag(ProcContributionFlags.ChainExpandedView))
        {
            return false;
        }

        return true;
    }

    internal ProcAllowanceMode GetCompatibilityAllowance(ImportedProcPolicy policy)
    {
        return policy.Allowance;
    }

    internal ProcOverCapHandlingMode GetProcOverCapHandling(ImportedProcPolicy policy)
    {
        return policy.IgnoreOverCap
            ? _procOverCapHandling
            : ProcOverCapHandlingMode.PreservedOnly;
    }

    private static ImportedProcPolicy GetProcPolicy(IPower? ownerPower)
    {
        return ownerPower is Power concretePower
            ? concretePower.ProcPolicy
            : ImportedProcPolicy.Default;
    }

    private bool ShouldBlockPlannerLocalProcContribution(ImportedProcPolicy policy)
    {
        return _procEligibilityInterpretation switch
        {
            ProcEligibilityInterpretationMode.OmniTypedEligibility => policy.Eligibility switch
            {
                ImportedProcEligibilityMode.GlobalOnly => true,
                ImportedProcEligibilityMode.None => true,
                ImportedProcEligibilityMode.Unknown => true,
                _ => policy.Allowance == ProcAllowanceMode.Deny
            },
            _ => policy.Allowance == ProcAllowanceMode.Deny
        };
    }

    private static bool IsPlannerProcContributionEffect(IEffect effect)
    {
        if (!effect.isEnhancementEffect)
        {
            return false;
        }

        if (effect.IgnoreScaling || effect.IsFromProc)
        {
            return true;
        }

        return effect is Effect concreteEffect &&
               concreteEffect.ProcContributionFlags.HasFlag(ProcContributionFlags.InheritedFromProcWrapper);
    }
}

internal static class ImportedProcPolicyNormalizer
{
    public static ImportedProcPolicy Normalize(
        JToken? procAllowedValue,
        bool mainTargetOnly,
        bool ignoreChainEffect,
        bool ignoreOverCap,
        out bool recognizedProcAllowed)
    {
        recognizedProcAllowed = TryNormalizeProcEligibility(procAllowedValue, out var eligibility, out var rawEligibilityValue);
        return new ImportedProcPolicy(
            eligibility,
            ProjectAllowance(eligibility),
            mainTargetOnly,
            ignoreChainEffect,
            ignoreOverCap,
            rawEligibilityValue);
    }

    public static bool TryNormalizeProcAllowed(JToken? procAllowedValue, out ProcAllowanceMode allowance)
    {
        var recognized = TryNormalizeProcEligibility(procAllowedValue, out var eligibility, out _);
        allowance = ProjectAllowance(eligibility);
        return recognized;
    }

    public static ProcAllowanceMode ParseStoredAllowance(string? value)
    {
        return Enum.TryParse<ProcAllowanceMode>(value, ignoreCase: true, out var parsed)
            ? parsed
            : ProcAllowanceMode.Default;
    }

    public static ImportedProcEligibilityMode ParseStoredEligibility(string? value, string? fallbackAllowance = null)
    {
        if (Enum.TryParse<ImportedProcEligibilityMode>(value, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        return ParseStoredAllowance(fallbackAllowance) switch
        {
            ProcAllowanceMode.Allow => ImportedProcEligibilityMode.All,
            ProcAllowanceMode.Deny => ImportedProcEligibilityMode.None,
            _ => ImportedProcEligibilityMode.Default
        };
    }

    public static ProcAllowanceMode ProjectAllowance(ImportedProcEligibilityMode eligibility)
    {
        return eligibility switch
        {
            ImportedProcEligibilityMode.All or ImportedProcEligibilityMode.PowerOnly => ProcAllowanceMode.Allow,
            ImportedProcEligibilityMode.GlobalOnly or ImportedProcEligibilityMode.None or ImportedProcEligibilityMode.Unknown => ProcAllowanceMode.Deny,
            _ => ProcAllowanceMode.Default
        };
    }

    private static bool TryNormalizeProcEligibility(
        JToken? procAllowedValue,
        out ImportedProcEligibilityMode eligibility,
        out string rawEligibilityValue)
    {
        eligibility = ImportedProcEligibilityMode.Default;
        rawEligibilityValue = procAllowedValue?.ToString(Formatting.None) ?? string.Empty;
        if (procAllowedValue == null || procAllowedValue.Type == JTokenType.Null)
        {
            rawEligibilityValue = string.Empty;
            return true;
        }

        return procAllowedValue.Type switch
        {
            JTokenType.Boolean => NormalizeFromBoolean(procAllowedValue.Value<bool>(), out eligibility),
            JTokenType.Integer => NormalizeFromInteger(procAllowedValue.Value<int>(), out eligibility),
            JTokenType.String => NormalizeFromString(procAllowedValue.Value<string>(), out eligibility),
            _ => NormalizeUnknown(out eligibility)
        };
    }

    private static bool NormalizeFromBoolean(bool value, out ImportedProcEligibilityMode eligibility)
    {
        eligibility = value
            ? ImportedProcEligibilityMode.All
            : ImportedProcEligibilityMode.None;
        return true;
    }

    private static bool NormalizeFromInteger(int value, out ImportedProcEligibilityMode eligibility)
    {
        eligibility = value switch
        {
            0 => ImportedProcEligibilityMode.All,
            1 => ImportedProcEligibilityMode.None,
            2 => ImportedProcEligibilityMode.PowerOnly,
            3 => ImportedProcEligibilityMode.GlobalOnly,
            _ => ImportedProcEligibilityMode.Unknown
        };

        return value is >= 0 and <= 3;
    }

    private static bool NormalizeFromString(string? value, out ImportedProcEligibilityMode eligibility)
    {
        eligibility = ImportedProcEligibilityMode.Default;
        var normalized = (value ?? string.Empty).Trim().Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return true;
        }

        if (normalized.Equals("all", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("true", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("allow", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("allowed", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("kall", StringComparison.OrdinalIgnoreCase))
        {
            eligibility = ImportedProcEligibilityMode.All;
            return true;
        }

        if (normalized.Equals("poweronly", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("kpoweronly", StringComparison.OrdinalIgnoreCase))
        {
            eligibility = ImportedProcEligibilityMode.PowerOnly;
            return true;
        }

        if (normalized.Equals("none", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("false", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("deny", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("denied", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("knone", StringComparison.OrdinalIgnoreCase))
        {
            eligibility = ImportedProcEligibilityMode.None;
            return true;
        }

        if (normalized.Equals("globalonly", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("kglobalonly", StringComparison.OrdinalIgnoreCase))
        {
            eligibility = ImportedProcEligibilityMode.GlobalOnly;
            return true;
        }

        if (normalized.Equals("default", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("inherit", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            eligibility = ImportedProcEligibilityMode.Default;
            return true;
        }

        eligibility = ImportedProcEligibilityMode.Unknown;
        return false;
    }

    private static bool NormalizeUnknown(out ImportedProcEligibilityMode eligibility)
    {
        eligibility = ImportedProcEligibilityMode.Unknown;
        return false;
    }
}

internal static class ChanceModifierCatalogBuilder
{
    public static Dictionary<string, float> Build(IReadOnlyList<IPower?> buffedPowers, IPower? setBonusVirtualPower)
    {
        var catalog = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        AddEffects(catalog, buffedPowers);
        AddEffects(catalog, setBonusVirtualPower == null ? [] : [setBonusVirtualPower]);
        return catalog;
    }

    private static void AddEffects(IDictionary<string, float> target, IEnumerable<IPower?> powers)
    {
        foreach (var power in powers.Where(power => power != null))
        {
            foreach (var effect in power!.Effects)
            {
                if (effect.EffectType != Enums.eEffectType.GlobalChanceMod ||
                    string.IsNullOrWhiteSpace(effect.Reward) ||
                    ChanceModifierSupport.IsPowerLocalChanceMod(effect))
                {
                    continue;
                }

                if (target.TryGetValue(effect.Reward, out var existing))
                {
                    target[effect.Reward] = existing + effect.Scale;
                }
                else
                {
                    target[effect.Reward] = effect.Scale;
                }
            }
        }
    }
}

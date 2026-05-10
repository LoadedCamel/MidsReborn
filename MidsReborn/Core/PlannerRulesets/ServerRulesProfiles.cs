using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core.PlannerRulesets;

public enum ServerRulesProfileId
{
    Legacy = 0,
    Homecoming = 1,
    Rebirth = 2,
    Thunderspy = 3
}

public sealed class ServerRulesProfile
{
    internal ServerRulesProfile(
        ServerRulesProfileId id,
        OmniDataProviderId dataProviderId,
        PlannerRulesetId plannerRulesetId,
        ProcEligibilityInterpretationMode procEligibilityInterpretation,
        ProcOverCapHandlingMode procOverCapHandling,
        bool usesToxicDefense,
        bool supportsPowerLocalChanceMods,
        bool usesImportedEnhancementMathWhenAvailable,
        SlottingRulesPolicy slottingRulesPolicy)
    {
        Id = id;
        DataProviderId = dataProviderId;
        PlannerRulesetId = plannerRulesetId;
        ProcEligibilityInterpretation = procEligibilityInterpretation;
        ProcOverCapHandling = procOverCapHandling;
        UsesToxicDefense = usesToxicDefense;
        SupportsPowerLocalChanceMods = supportsPowerLocalChanceMods;
        UsesImportedEnhancementMathWhenAvailable = usesImportedEnhancementMathWhenAvailable;
        SlottingRulesPolicy = slottingRulesPolicy;
        Ruleset = PlannerRulesetResolver.Resolve(plannerRulesetId);
        ProcRulesPolicy = new ProcRulesPolicy(
            Ruleset,
            procEligibilityInterpretation,
            procOverCapHandling,
            supportsPowerLocalChanceMods);
    }

    public ServerRulesProfileId Id { get; }
    public OmniDataProviderId DataProviderId { get; }
    public PlannerRulesetId PlannerRulesetId { get; }
    internal ProcEligibilityInterpretationMode ProcEligibilityInterpretation { get; }
    internal ProcOverCapHandlingMode ProcOverCapHandling { get; }
    public bool UsesToxicDefense { get; }
    public bool SupportsPowerLocalChanceMods { get; }
    public bool UsesImportedEnhancementMathWhenAvailable { get; }
    public bool UsesCanonicalPlannerMath => Ruleset.UsesCanonicalPlannerMath;

    internal IPlannerRuleset Ruleset { get; }
    internal SlottingRulesPolicy SlottingRulesPolicy { get; }
    internal ProcRulesPolicy ProcRulesPolicy { get; }
    internal EnhancementMathPolicy GetEnhancementMathPolicy(EnhancementImportMetadata? metadata)
    {
        return EnhancementMathPolicyResolver.Resolve(this, metadata);
    }

    internal SlottingValidationResult ValidateEnhancementForPower(IPower? power, int enhancementId)
    {
        return SlottingRulesPolicy.ValidatePowerEnhancement(power, enhancementId);
    }

    internal SlottingValidationResult ValidateEnhancementSlot(Build? build, int powerIndex, int slotIndex, int enhancementId)
    {
        return SlottingRulesPolicy.ValidateBuildSlot(build, powerIndex, slotIndex, enhancementId);
    }

    public float GetMinProcChance(float procsPerMinute)
    {
        return ProcRulesPolicy.GetMinProcChance(procsPerMinute);
    }

    public float GetMaxProcChance(float procsPerMinute)
    {
        return ProcRulesPolicy.GetMaxProcChance(procsPerMinute);
    }

    public float EvaluateProcProbability(Character? character, IPower? ownerPower, IEffect procEffect, float baseProbability)
    {
        return ProcRulesPolicy.EvaluateProbability(character, ownerPower, procEffect, baseProbability);
    }

    public bool TryGetChanceModifierScale(Character? character, IPower? ownerPower, string chanceTag, out float scale)
    {
        return ProcRulesPolicy.TryGetChanceModifierScale(character, ownerPower, chanceTag, out scale);
    }

    public bool ShouldIncludePlannerEffect(IPower? ownerPower, IEffect effect)
    {
        return ProcRulesPolicy.ShouldIncludePlannerEffect(ownerPower, effect);
    }
}

internal static class ServerRulesProfileResolver
{
    private static readonly SlottingRulesPolicy DefaultSlottingRulesPolicy = new();

    private static readonly ServerRulesProfile Legacy = new(
        ServerRulesProfileId.Legacy,
        OmniDataProviderId.Unknown,
        PlannerRulesetId.Legacy,
        procEligibilityInterpretation: ProcEligibilityInterpretationMode.CompatibilityProjection,
        procOverCapHandling: ProcOverCapHandlingMode.PreservedOnly,
        usesToxicDefense: false,
        supportsPowerLocalChanceMods: false,
        usesImportedEnhancementMathWhenAvailable: true,
        slottingRulesPolicy: DefaultSlottingRulesPolicy);

    private static readonly ServerRulesProfile Homecoming = new(
        ServerRulesProfileId.Homecoming,
        OmniDataProviderId.OmniHomecoming,
        PlannerRulesetId.Homecoming,
        procEligibilityInterpretation: ProcEligibilityInterpretationMode.OmniTypedEligibility,
        procOverCapHandling: ProcOverCapHandlingMode.PreservedOnly,
        usesToxicDefense: true,
        supportsPowerLocalChanceMods: true,
        usesImportedEnhancementMathWhenAvailable: true,
        slottingRulesPolicy: DefaultSlottingRulesPolicy);

    private static readonly ServerRulesProfile Rebirth = new(
        ServerRulesProfileId.Rebirth,
        OmniDataProviderId.OmniRebirth,
        PlannerRulesetId.Ourodev,
        procEligibilityInterpretation: ProcEligibilityInterpretationMode.OmniTypedEligibility,
        procOverCapHandling: ProcOverCapHandlingMode.PreservedOnly,
        usesToxicDefense: false,
        supportsPowerLocalChanceMods: true,
        usesImportedEnhancementMathWhenAvailable: true,
        slottingRulesPolicy: DefaultSlottingRulesPolicy);

    private static readonly ServerRulesProfile Thunderspy = new(
        ServerRulesProfileId.Thunderspy,
        OmniDataProviderId.OmniThunderspy,
        PlannerRulesetId.Ourodev,
        procEligibilityInterpretation: ProcEligibilityInterpretationMode.OmniTypedEligibility,
        procOverCapHandling: ProcOverCapHandlingMode.PreservedOnly,
        usesToxicDefense: false,
        supportsPowerLocalChanceMods: true,
        usesImportedEnhancementMathWhenAvailable: true,
        slottingRulesPolicy: DefaultSlottingRulesPolicy);

    public static ServerRulesProfile Resolve(OmniDataProviderId providerId, PlannerRulesetId rulesetId)
    {
        return Resolve(ResolveProfileId(providerId, rulesetId));
    }

    public static ServerRulesProfile Resolve(ServerRulesProfileId profileId)
    {
        return profileId switch
        {
            ServerRulesProfileId.Homecoming => Homecoming,
            ServerRulesProfileId.Rebirth => Rebirth,
            ServerRulesProfileId.Thunderspy => Thunderspy,
            _ => Legacy
        };
    }

    internal static OmniDataProviderId DetectProviderIdFromHint(string? hint)
    {
        var normalized = NormalizeProviderHint(hint);
        if (normalized.Contains("homecoming", StringComparison.Ordinal))
        {
            return OmniDataProviderId.OmniHomecoming;
        }

        if (normalized.Contains("rebirth", StringComparison.Ordinal))
        {
            return OmniDataProviderId.OmniRebirth;
        }

        if (normalized.Contains("thunderspy", StringComparison.Ordinal) ||
            normalized.Contains("tspy", StringComparison.Ordinal))
        {
            return OmniDataProviderId.OmniThunderspy;
        }

        return OmniDataProviderId.Unknown;
    }

    internal static PlannerRulesetId ResolvePlannerRulesetId(
        OmniDataProviderId providerId,
        PlannerRulesetId existingRulesetId)
    {
        return providerId switch
        {
            OmniDataProviderId.OmniHomecoming => PlannerRulesetId.Homecoming,
            OmniDataProviderId.OmniRebirth or OmniDataProviderId.OmniThunderspy => PlannerRulesetId.Ourodev,
            _ => existingRulesetId
        };
    }

    internal static int ResolvePlannerRulesetVersion(PlannerRulesetId rulesetId)
    {
        return 3;
    }

    internal static ServerRulesProfileId ResolveProfileId(OmniDataProviderId providerId, PlannerRulesetId rulesetId)
    {
        return providerId switch
        {
            OmniDataProviderId.OmniHomecoming => ServerRulesProfileId.Homecoming,
            OmniDataProviderId.OmniRebirth => ServerRulesProfileId.Rebirth,
            OmniDataProviderId.OmniThunderspy => ServerRulesProfileId.Thunderspy,
            _ => ResolveProfileId(rulesetId)
        };
    }

    private static ServerRulesProfileId ResolveProfileId(PlannerRulesetId rulesetId)
    {
        // Older databases only persisted the shared Ourodev ruleset id, so keep
        // a stable default profile for that family unless provider metadata says otherwise.
        return rulesetId switch
        {
            PlannerRulesetId.Homecoming => ServerRulesProfileId.Homecoming,
            PlannerRulesetId.Ourodev => ServerRulesProfileId.Rebirth,
            _ => ServerRulesProfileId.Legacy
        };
    }

    private static string NormalizeProviderHint(string? hint)
    {
        return (hint ?? string.Empty)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();
    }
}

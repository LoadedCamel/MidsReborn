using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Omni;

public enum OmniReportOverallStatusKind
{
    Healthy,
    HealthyWithKnownUnsupportedOrAuditOnlyItems,
    NeedsManualReview,
    BlockedByNeedsImplementation
}

public enum OmniReportBucketStatusKind
{
    HealthyImplemented,
    ImplementedWithFallback,
    InformationalReportOnly,
    KnownUnsupported,
    NeedsManualReview,
    NeedsImplementation
}

public sealed class OmniReportBucketSummary
{
    public string Key { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;

    [JsonIgnore]
    public OmniReportBucketStatusKind StatusKind { get; init; }

    public string Status => OmniReportLegibility.FormatBucketStatus(StatusKind);
    public string Meaning { get; init; } = string.Empty;
    public bool Actionable { get; init; }
    public string RecommendedAction { get; init; } = string.Empty;
    public string Snapshot { get; init; } = string.Empty;
    public List<string> Highlights { get; } = [];
}

public sealed class OmniReportSummary
{
    [JsonIgnore]
    public OmniReportOverallStatusKind OverallStatusKind { get; init; }

    public string OverallStatus => OmniReportLegibility.FormatOverallStatus(OverallStatusKind);
    public string Overview { get; init; } = string.Empty;
    public List<string> NextActions { get; } = [];
    public List<OmniReportBucketSummary> Imported { get; } = [];
    public List<OmniReportBucketSummary> NotImportedOrBroken { get; } = [];
    public List<OmniReportBucketSummary> NotModeledYet { get; } = [];
    public List<OmniReportBucketSummary> IgnoredByDesign { get; } = [];
    public List<OmniReportBucketSummary> OptionalCleanup { get; } = [];
    public List<OmniReportBucketSummary> Buckets { get; } = [];
}

public sealed class OmniReportEnvelopeSummary
{
    [JsonIgnore]
    public OmniReportOverallStatusKind OverallStatusKind { get; init; }

    public string OverallStatus => OmniReportLegibility.FormatOverallStatus(OverallStatusKind);
    public string Overview { get; init; } = string.Empty;
    public List<string> NextActions { get; } = [];
    public OmniReportSummary DryRun { get; init; } = new();
    public OmniReportSummary Apply { get; init; } = new();
}

internal static class OmniReportLegibility
{
    private static readonly HashSet<string> HarmlessSourceMetadataFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "attrib_cache",
        "display_fullname",
        "display_help",
        "display_short_help",
        "display_name",
        "icon",
        "short_name"
    };

    private static readonly HashSet<string> KnownUnsupportedPlannerPolicyFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "strengths_disallowed",
        "global_strengths_disallowed"
    };

    public static string FormatOverallStatus(OmniReportOverallStatusKind status)
    {
        return status switch
        {
            OmniReportOverallStatusKind.Healthy => "Healthy",
            OmniReportOverallStatusKind.HealthyWithKnownUnsupportedOrAuditOnlyItems => "Healthy with only known unsupported or audit-only items left",
            OmniReportOverallStatusKind.NeedsManualReview => "Needs manual review",
            OmniReportOverallStatusKind.BlockedByNeedsImplementation => "Blocked by unresolved implementation gaps",
            _ => "Healthy"
        };
    }

    public static string FormatBucketStatus(OmniReportBucketStatusKind status)
    {
        return status switch
        {
            OmniReportBucketStatusKind.HealthyImplemented => "Healthy / Implemented",
            OmniReportBucketStatusKind.ImplementedWithFallback => "Implemented With Fallback",
            OmniReportBucketStatusKind.InformationalReportOnly => "Informational / Report-Only",
            OmniReportBucketStatusKind.KnownUnsupported => "Known Unsupported",
            OmniReportBucketStatusKind.NeedsManualReview => "Needs Manual Review",
            OmniReportBucketStatusKind.NeedsImplementation => "Needs Implementation",
            _ => "Healthy / Implemented"
        };
    }

    public static OmniReportSummary BuildSummary(OmniImportReport report)
    {
        var buckets = new List<OmniReportBucketSummary>();

        var ignoredPowerFieldBreakdown = BuildIgnoredPowerFieldBreakdown(report.IgnoredPowerFieldCount, report.IgnoredPowerFieldNameCounts);
        var unsupportedPolicyCount = ignoredPowerFieldBreakdown.UnsupportedPlannerPolicyCount;

        buckets.Add(CreateBucket(
            key: "expressions",
            label: "Expression coverage",
            status: report.UnsupportedBuildExpressionCount > 0 || report.UnknownExpressionTokenCount > 0 || report.UnsupportedPowerRequirementCount > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : report.PowerTargetRequiresManualReviewCount > 0
                    ? OmniReportBucketStatusKind.ImplementedWithFallback
                    : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Build-evaluated expressions are planner-facing support. Runtime-target and report-only expressions are preserved for diagnostics without blocking the import when build-time support is complete.",
            actionable: report.UnsupportedBuildExpressionCount > 0 || report.UnknownExpressionTokenCount > 0 || report.UnsupportedPowerRequirementCount > 0,
            recommendedAction: report.UnsupportedBuildExpressionCount > 0 || report.UnknownExpressionTokenCount > 0 || report.UnsupportedPowerRequirementCount > 0
                ? "Implement the remaining unsupported build-time expressions or requirement forms before treating the report as fully complete."
                : string.Empty,
            snapshot: $"Build-evaluated={report.BuildEvaluatedExpressionCount:n0}, runtime-target={report.RuntimeTargetExpressionCount:n0}, report-only={report.ReportOnlyExpressionCount:n0}, unsupported={report.UnsupportedBuildExpressionCount + report.UnknownExpressionTokenCount + report.UnsupportedPowerRequirementCount:n0}, manual-review={report.PowerTargetRequiresManualReviewCount:n0}",
            highlights:
            [
                "Build-evaluated expressions are implemented planner math.",
                "Runtime-target expressions stay available for runtime-targeted behavior without becoming build-time blockers.",
                "Report-only expressions such as highlight or tray logic are preserved for audit visibility instead of being treated as missing planner support.",
                report.PowerTargetRequiresManualReviewCount > 0
                    ? "When unsupported build expressions and unknown tokens are both zero, the remaining manual-review expression count is audit inventory rather than a sign-off blocker."
                    : string.Empty
            ]));

        buckets.Add(CreateBucket(
            key: "effect-mode-mappings",
            label: "Effect and mode mapping coverage",
            status: report.UnknownEffectMappings.Count > 0 || report.UnknownAttribMappings.Count > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : report.UnknownEffectModeCount > 0 || report.UnknownModeCount > 0
                    ? OmniReportBucketStatusKind.NeedsManualReview
                    : report.ManualClassificationReviewCount > 0 || report.SpecialCaseCompatibilityBridgeCount > 0 || report.SnipeEngagedAliasCount > 0 || report.PowerFieldsMappedWithFallbackCount > 0
                        ? OmniReportBucketStatusKind.ImplementedWithFallback
                        : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Effect templates, mode conditions, and planner-mode payloads are the importer’s core combat-math bridge. Unknown effect or attribute mappings are real implementation gaps; compatibility bridges and aliases are already intentional fallbacks.",
            actionable: report.UnknownEffectMappings.Count > 0 || report.UnknownAttribMappings.Count > 0 || report.UnknownEffectModeCount > 0 || report.UnknownModeCount > 0,
            recommendedAction: report.UnknownEffectMappings.Count > 0 || report.UnknownAttribMappings.Count > 0
                ? "Implement the unresolved effect or attribute mappings so combat behavior does not rely on manual interpretation."
                : report.UnknownEffectModeCount > 0 || report.UnknownModeCount > 0
                    ? "Review the remaining mode/classification entries and either map them explicitly or downgrade them to report-only handling."
                    : string.Empty,
            snapshot: $"Unknown effect mappings={report.UnknownEffectMappings.Count:n0}, unknown attribute mappings={report.UnknownAttribMappings.Count:n0}, unknown modes={report.UnknownModeCount + report.UnknownEffectModeCount:n0}, manual review={report.ManualClassificationReviewCount:n0}, compatibility bridges={report.SpecialCaseCompatibilityBridgeCount:n0}",
            highlights:
            [
                "Compatibility bridges and kEngaged aliases are intentional fallback behavior, not unresolved failures.",
                report.ManualClassificationReviewCount > 0 && report.UnknownEffectModeCount + report.UnknownModeCount == 0
                    ? "Manual classification review counts are retained as audit inventory. When unknown mappings and unknown modes are both zero, they do not block importer sign-off."
                    : string.Empty
            ]));

        buckets.Add(CreateBucket(
            key: "gcm-registry",
            label: "GCM registry coverage",
            status: report.EffectTagMissingFromGcmCount > 0
                ? OmniReportBucketStatusKind.InformationalReportOnly
                : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "GCM counts describe tag-registry coverage. Tags referenced outside gcm.json are registry drift and do not automatically mean a broken power import.",
            actionable: false,
            recommendedAction: "Optional audit: refresh gcm.json or accept the registry drift if those tags are intentionally sourced outside the exported registry.",
            snapshot: $"Imported={report.GcmTagsImportedCount:n0}, already-known={report.GcmTagsAlreadyKnownCount:n0}, would-add={report.GcmTagsWouldAddCount:n0}, registry-drift={report.EffectTagMissingFromGcmCount:n0}",
            highlights:
            [
                "Effect tags referenced outside gcm.json are shown as informational registry drift unless another bucket marks them as implementation-blocking."
            ]));

        buckets.Add(CreateBucket(
            key: "power-field-mappings",
            label: "Power-field mapping coverage",
            status: report.UnknownPowerFieldCount > 0 || report.PowerFieldConflictCount > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : report.PowerFieldsMappedWithFallbackCount > 0 || report.KnownChargeCapacityExtensions > 0
                    ? OmniReportBucketStatusKind.ImplementedWithFallback
                    : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Unknown power fields and field conflicts are the true unresolved field-mapping buckets. Deferred fields and charge-capacity extensions are already known classifications, not mystery failures.",
            actionable: report.UnknownPowerFieldCount > 0 || report.PowerFieldConflictCount > 0,
            recommendedAction: report.UnknownPowerFieldCount > 0 || report.PowerFieldConflictCount > 0
                ? "Implement the remaining power-field mappings or resolve the field conflicts so the report can treat every planner-relevant field as covered."
                : string.Empty,
            snapshot: $"Mapped={report.PowerFieldsMappedCount:n0}, fallback={report.PowerFieldsMappedWithFallbackCount:n0}, conflicts={report.PowerFieldConflictCount:n0}, unknown={report.UnknownPowerFieldCount:n0}, deferred={report.DeferredPowerFieldCount:n0}, charge-capacity extensions={report.KnownChargeCapacityExtensions:n0}",
            highlights:
            [
                "Known charge-capacity extensions stay informational so they do not read like unresolved field conflicts."
            ]));

        buckets.Add(CreateBucket(
            key: "harmless-source-metadata",
            label: "Harmless source/export metadata",
            status: ignoredPowerFieldBreakdown.HarmlessSourceMetadataCount > 0
                ? OmniReportBucketStatusKind.InformationalReportOnly
                : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "These ignored power fields are source-side cache or display metadata that the planner does not use as authoritative combat data.",
            actionable: false,
            recommendedAction: "No importer work is required unless you want to suppress or collapse this audit noise further.",
            snapshot: $"Ignored source/export metadata={ignoredPowerFieldBreakdown.HarmlessSourceMetadataCount:n0}",
            highlights:
            [
                "attrib_cache entries containing names like Lethal_Dmg or Smashing_Dmg are cached source hints, not dropped combat math."
            ]));

        buckets.Add(CreateBucket(
            key: "ui-client-server-metadata",
            label: "UI / client / server behavior not modeled by planner",
            status: ignoredPowerFieldBreakdown.UiClientServerMetadataCount > 0 || report.IgnoredFields.Count > 0
                ? OmniReportBucketStatusKind.InformationalReportOnly
                : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "This bucket covers tray, highlight, confirmation, message, stance, and other client/server presentation metadata that is intentionally visible in the audit but not modeled as planner math.",
            actionable: false,
            recommendedAction: "Optional polish only: collapse or suppress these entries if you want the default report to be quieter.",
            snapshot: $"Ignored power-field UI/client/server metadata={ignoredPowerFieldBreakdown.UiClientServerMetadataCount:n0}, ignored report-only source directives={report.IgnoredFields.Count:n0}",
            highlights:
            [
                "highlight_expression, server_tray_requires, and similar directives are preserved as report-only evidence rather than treated as unsupported build logic."
            ]));

        buckets.Add(CreateBucket(
            key: "enhancement-policy",
            label: "Enhancement-policy coverage",
            status: report.PowerFieldConflictCount > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : unsupportedPolicyCount > 0
                    ? OmniReportBucketStatusKind.KnownUnsupported
                    : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Mapped strengths_disallowed/global_strengths_disallowed rules are implemented. Only explicitly listed unsupported axes such as Radius, Arc, or TimeToRoot remain unmodeled when this bucket is non-zero.",
            actionable: report.PowerFieldConflictCount > 0 || unsupportedPolicyCount > 0,
            recommendedAction: report.PowerFieldConflictCount > 0
                ? "Resolve the remaining enhancement-policy conflicts before relying on the import as the final legality source."
                : unsupportedPolicyCount > 0
                    ? "Optional planner feature work: add the remaining unsupported enhancement-policy axes if they matter for editor legality accuracy."
                    : string.Empty,
            snapshot: $"Known unsupported planner-policy fields={unsupportedPolicyCount:n0}, field conflicts={report.PowerFieldConflictCount:n0}",
            highlights:
            [
                report.PowerFieldConflictCount == 0
                    ? "PowerFieldConflicts=0 means there are no unresolved enhancement-policy blockers in this run."
                    : "Remaining PowerFieldConflicts indicate unresolved enhancement-policy blockers."
            ]));

        buckets.Add(CreateBucket(
            key: "powerset-icons",
            label: "Powerset icon audit",
            status: report.PoolRequirementEvaluationFailureCount > 0
                ? OmniReportBucketStatusKind.NeedsManualReview
                : report.MissingPoolIconAssetCount > 0
                    ? OmniReportBucketStatusKind.InformationalReportOnly
                : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Icon audits are presentation and asset checks. Missing assets or pool requirement evaluation mismatches should be reviewed, but they are not the same thing as broken planner math.",
            actionable: report.MissingPoolIconAssetCount > 0 || report.PoolRequirementEvaluationFailureCount > 0,
            recommendedAction: report.PoolRequirementEvaluationFailureCount > 0
                ? "Review the pool requirement audit details to decide whether the remaining mismatches reflect real editor-content issues."
                : report.MissingPoolIconAssetCount > 0
                    ? "Optional editor-content cleanup: review the missing icon asset/name mismatches and decide whether they are real gaps or naming/reporting noise."
                    : string.Empty,
            snapshot: $"Icon audits={report.PoolPowersetIconAuditCount:n0}, assignments available={report.PoolIconAssignmentsAvailableCount:n0}, missing assets={report.MissingPoolIconAssetCount:n0}, pool requirement failures={report.PoolRequirementEvaluationFailureCount:n0}",
            highlights:
            [
                "This bucket is about UI asset coverage and pickability traces, not missing planner combat support."
            ]));

        buckets.Add(CreateBucket(
            key: "pet-entity-scope",
            label: "Pet, entity, and scoped import coverage",
            status: report.EntityReferencesUnresolved > 0 || report.PetManifestMissingPowerFiles > 0 || report.MissingClassTableReferences > 0 || report.MissingModifierTableReferences > 0
                ? OmniReportBucketStatusKind.NeedsManualReview
                : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "This bucket tells you whether the dry-run scope could resolve the supporting entities, pet manifests, and class tables needed for a clean apply phase.",
            actionable: report.EntityReferencesUnresolved > 0 || report.PetManifestMissingPowerFiles > 0 || report.MissingClassTableReferences > 0 || report.MissingModifierTableReferences > 0,
            recommendedAction: "Review unresolved entities, missing pet-manifest files, and missing table references before treating the scope as fully trustworthy.",
            snapshot: $"Entity references unresolved={report.EntityReferencesUnresolved:n0}, pet manifest missing files={report.PetManifestMissingPowerFiles:n0}, missing class/modifier tables={report.MissingClassTableReferences + report.MissingModifierTableReferences:n0}",
            highlights:
            [
                "When this bucket is healthy, apply-time pet and table integrity work is much less likely to surface new surprises."
            ]));

        buckets.Add(CreateBucket(
            key: "enhancement-reconciliation",
            label: "Enhancement / recipe / salvage reconciliation",
            status: report.EnhancementBoostPowerLinksMissingDryRun + report.EnhancementSetBonusLinksMissingDryRun + report.RecipeRewardLinksMissingDryRun > 0
                    || report.EnhancementAmbiguousMatchesDryRun + report.EnhancementSetAmbiguousMatchesDryRun + report.RecipeAmbiguousMatchesDryRun + report.SalvageAmbiguousMatchesDryRun > 0
                    || report.EnhancementPolicyFilesMissing > 0
                ? OmniReportBucketStatusKind.NeedsManualReview
                : report.EnhancementAliasMatchesDryRun + report.EnhancementFallbackMatchesDryRun + report.RecipeAliasMatchesDryRun + report.RecipeFallbackMatchesDryRun + report.SalvageAliasMatchesDryRun + report.SalvageFallbackMatchesDryRun > 0
                    ? OmniReportBucketStatusKind.ImplementedWithFallback
                    : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Reconciliation covers enhancement, recipe, and salvage identity matching. Alias and fallback matches are expected compatibility paths; missing links, ambiguous matches, and missing policy files are the items worth reviewing.",
            actionable: report.EnhancementBoostPowerLinksMissingDryRun + report.EnhancementSetBonusLinksMissingDryRun + report.RecipeRewardLinksMissingDryRun > 0
                || report.EnhancementAmbiguousMatchesDryRun + report.EnhancementSetAmbiguousMatchesDryRun + report.RecipeAmbiguousMatchesDryRun + report.SalvageAmbiguousMatchesDryRun > 0
                || report.EnhancementPolicyFilesMissing > 0,
            recommendedAction: "Review missing enhancement links, ambiguous matches, and absent enhancement policy files before treating the reconciliation plan as final.",
            snapshot: $"Missing boost/set/reward links={report.EnhancementBoostPowerLinksMissingDryRun + report.EnhancementSetBonusLinksMissingDryRun + report.RecipeRewardLinksMissingDryRun:n0}, ambiguous matches={report.EnhancementAmbiguousMatchesDryRun + report.EnhancementSetAmbiguousMatchesDryRun + report.RecipeAmbiguousMatchesDryRun + report.SalvageAmbiguousMatchesDryRun:n0}, policy files missing={report.EnhancementPolicyFilesMissing:n0}",
            highlights:
            [
                "Would update/create counts are normal planning output, not unresolved failures by themselves."
            ]));

        return FinalizeSummary(buckets);
    }

    public static OmniReportSummary BuildSummary(OmniApplyResult result)
    {
        var buckets = new List<OmniReportBucketSummary>();

        var ignoredPowerFieldBreakdown = BuildIgnoredPowerFieldBreakdown(result.IgnoredPowerFields, result.IgnoredPowerFieldNameCounts);
        var unsupportedPolicyCount = ignoredPowerFieldBreakdown.UnsupportedPlannerPolicyCount;

        buckets.Add(CreateBucket(
            key: "core-import-integrity",
            label: "Core import integrity",
            status: result.PowersMissingFromMids > 0 || result.UnknownEffectMappings > 0 || result.UnknownAttribMappings > 0 || result.SupportPowerLinksUnresolved > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "These are the primary correctness counters for a safe import. Unknown effect or attribute mappings, missing powers, and unresolved support links are real implementation gaps.",
            actionable: result.PowersMissingFromMids > 0 || result.UnknownEffectMappings > 0 || result.UnknownAttribMappings > 0 || result.SupportPowerLinksUnresolved > 0,
            recommendedAction: "Resolve any remaining unknown mappings, missing powers, or unresolved support links before treating the import as final.",
            snapshot: $"Powers missing from Mids={result.PowersMissingFromMids:n0}, unknown effect mappings={result.UnknownEffectMappings:n0}, unknown attribute mappings={result.UnknownAttribMappings:n0}, support links unresolved={result.SupportPowerLinksUnresolved:n0}",
            highlights:
            [
                "Known hidden/stateful and known unsupported effect mappings are tracked separately so they do not masquerade as unresolved correctness failures."
            ]));

        buckets.Add(CreateBucket(
            key: "scoped-pet-epic-integrity",
            label: "Scoped, pet, and epic integrity",
            status: result.OrphanPowersAfterImport > 0 || result.NewOrphanPowersIntroduced > 0 || result.OrphanedScopedOmniPowers > 0
                    || result.PetPowersMissingAfterImport > 0 || result.PetSourceIntegrityFailures > 0
                    || result.EpicPowersMissingAfterImport > 0 || result.EpicPowersetsWithZeroLinkedPowers > 0
                    || result.ScopedPowerEnhancementLegalityUnresolvedAfterRebuild > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : result.AcceptedCanonicalScopedPowerReplacements > 0
                    || result.ManifestOwnedScopedOmniPowers > 0
                    || result.AliasedPowerIdentityRepairs > 0
                    || result.AliasedPowersetIdentityRepairs > 0
                        ? OmniReportBucketStatusKind.ImplementedWithFallback
                        : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "This bucket reflects the final DB graph repair pass: orphan powers, scoped retention, pet integrity, epic linking, and scoped legality rebuilds.",
            actionable: result.OrphanPowersAfterImport > 0 || result.NewOrphanPowersIntroduced > 0 || result.OrphanedScopedOmniPowers > 0
                || result.PetPowersMissingAfterImport > 0 || result.PetSourceIntegrityFailures > 0
                || result.EpicPowersMissingAfterImport > 0 || result.EpicPowersetsWithZeroLinkedPowers > 0
                || result.ScopedPowerEnhancementLegalityUnresolvedAfterRebuild > 0,
            recommendedAction: "Fix any remaining orphan, scoped, pet, epic, or legality-rebuild failures before considering the final DB graph stable.",
            snapshot: $"Orphans after import={result.OrphanPowersAfterImport:n0}, orphaned scoped powers={result.OrphanedScopedOmniPowers:n0}, pet integrity failures={result.PetSourceIntegrityFailures + result.PetPowersMissingAfterImport:n0}, epic integrity failures={result.EpicPowersMissingAfterImport + result.EpicPowersetsWithZeroLinkedPowers:n0}, scoped legality unresolved={result.ScopedPowerEnhancementLegalityUnresolvedAfterRebuild:n0}",
            highlights:
            [
                "Accepted canonical scoped replacements and manifest-owned scoped powers are compatibility/accounting paths, not failures by themselves."
            ]));

        buckets.Add(CreateBucket(
            key: "pseudo-pet-audit",
            label: "Pseudo-pet audit",
            status: result.PseudoPetAbsorptionAuditFailures > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : result.PseudoPetAbsorptionAuditSkipped > 0
                    ? OmniReportBucketStatusKind.InformationalReportOnly
                    : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Pseudo-pet failures are the real unresolved wrapper cases. Skipped/report-only entries are intentionally separated so they no longer read like broken absorption behavior.",
            actionable: result.PseudoPetAbsorptionAuditFailures > 0,
            recommendedAction: "Implement the remaining pseudo-pet absorption cases or explicitly downgrade them to skipped/report-only behavior if they are not planner-visible.",
            snapshot: $"Absorption flags enabled={result.PseudoPetAbsorptionFlagsEnabled:n0}, audit failures={result.PseudoPetAbsorptionAuditFailures:n0}, skipped/report-only={result.PseudoPetAbsorptionAuditSkipped:n0}",
            highlights:
            [
                "PseudoPetAbsorptionAuditSkipped is informational audit noise, not failed combat support."
            ]));

        buckets.Add(CreateBucket(
            key: "effect-mode-mappings",
            label: "Effect and mode mapping coverage",
            status: result.UnknownEffectMappings > 0 || result.UnknownAttribMappings > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : result.UnknownModesPreserved > 0
                    ? OmniReportBucketStatusKind.NeedsManualReview
                    : result.ManualClassificationReviews > 0 || result.SpecialCaseCompatibilityBridges > 0 || result.SnipeEngagedAliases > 0
                        ? OmniReportBucketStatusKind.ImplementedWithFallback
                        : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Apply-time mode coverage reflects final classification and planner payload binding. Unknown mappings are real gaps; preserved unknown modes and manual review items still deserve inspection.",
            actionable: result.UnknownEffectMappings > 0 || result.UnknownAttribMappings > 0 || result.UnknownModesPreserved > 0,
            recommendedAction: result.UnknownEffectMappings > 0 || result.UnknownAttribMappings > 0
                ? "Implement the remaining effect or attribute mappings."
                : result.UnknownModesPreserved > 0
                    ? "Review the remaining preserved unknown-mode entries and either map them explicitly or downgrade them to report-only handling."
                    : string.Empty,
            snapshot: $"Unknown effect mappings={result.UnknownEffectMappings:n0}, unknown attribute mappings={result.UnknownAttribMappings:n0}, manual review={result.ManualClassificationReviews:n0}, unknown modes preserved={result.UnknownModesPreserved:n0}, compatibility bridges={result.SpecialCaseCompatibilityBridges:n0}",
            highlights:
            [
                "Known unsupported effect mappings are reported separately as known limitations, not unresolved apply failures.",
                result.ManualClassificationReviews > 0 && result.UnknownModesPreserved == 0
                    ? "Manual classification review counts are retained as audit inventory. When unknown mappings and preserved unknown modes are both zero, they do not block importer sign-off."
                    : string.Empty
            ]));

        buckets.Add(CreateBucket(
            key: "power-field-mappings",
            label: "Power-field mapping coverage",
            status: result.UnknownPowerFields > 0 || result.PowerFieldConflicts > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : result.PowerFieldsMappedWithFallback > 0 || result.KnownChargeCapacityExtensions > 0
                    ? OmniReportBucketStatusKind.ImplementedWithFallback
                    : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Unknown power fields and field conflicts are the real unresolved field-mapping buckets. Deferred fields and charge-capacity extensions are already known classifications.",
            actionable: result.UnknownPowerFields > 0 || result.PowerFieldConflicts > 0,
            recommendedAction: result.UnknownPowerFields > 0 || result.PowerFieldConflicts > 0
                ? "Resolve the remaining power-field gaps or conflicts before relying on the report as final field-coverage evidence."
                : string.Empty,
            snapshot: $"Mapped={result.PowerFieldsMapped:n0}, fallback={result.PowerFieldsMappedWithFallback:n0}, conflicts={result.PowerFieldConflicts:n0}, unknown={result.UnknownPowerFields:n0}, deferred={result.DeferredPowerFields:n0}, charge-capacity extensions={result.KnownChargeCapacityExtensions:n0}",
            highlights:
            [
                "KnownChargeCapacityExtensions is informational by design so it does not reopen the old Taser-style conflict noise."
            ]));

        buckets.Add(CreateBucket(
            key: "harmless-source-metadata",
            label: "Harmless source/export metadata",
            status: ignoredPowerFieldBreakdown.HarmlessSourceMetadataCount > 0
                ? OmniReportBucketStatusKind.InformationalReportOnly
                : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "These ignored power fields are cached/export metadata rather than authoritative planner math.",
            actionable: false,
            recommendedAction: "Optional report cleanup only.",
            snapshot: $"Ignored source/export metadata={ignoredPowerFieldBreakdown.HarmlessSourceMetadataCount:n0}",
            highlights:
            [
                "attrib_cache values that mention damage types are source hints, not dropped combat support."
            ]));

        buckets.Add(CreateBucket(
            key: "ui-client-server-metadata",
            label: "UI / client / server behavior not modeled by planner",
            status: ignoredPowerFieldBreakdown.UiClientServerMetadataCount > 0
                ? OmniReportBucketStatusKind.InformationalReportOnly
                : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "This is intentionally ignored presentation and interaction metadata, not unresolved planner logic.",
            actionable: false,
            recommendedAction: "Optional report cleanup only.",
            snapshot: $"Ignored UI/client/server metadata={ignoredPowerFieldBreakdown.UiClientServerMetadataCount:n0}",
            highlights:
            [
                "These fields are kept visible in the audit so you can distinguish them from unknown power fields."
            ]));

        buckets.Add(CreateBucket(
            key: "enhancement-policy",
            label: "Enhancement-policy coverage",
            status: result.PowerFieldConflicts > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : unsupportedPolicyCount > 0
                    ? OmniReportBucketStatusKind.KnownUnsupported
                    : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Mapped strengths_disallowed/global_strengths_disallowed rules are implemented. Only explicitly listed unsupported axes remain outside planner legality support when this bucket is non-zero.",
            actionable: result.PowerFieldConflicts > 0 || unsupportedPolicyCount > 0,
            recommendedAction: result.PowerFieldConflicts > 0
                ? "Resolve the remaining enhancement-policy conflicts."
                : unsupportedPolicyCount > 0
                    ? "Optional planner feature work: add the remaining unsupported enhancement-policy axes if they matter to the editor."
                    : string.Empty,
            snapshot: $"Known unsupported planner-policy fields={unsupportedPolicyCount:n0}, field conflicts={result.PowerFieldConflicts:n0}",
            highlights:
            [
                result.PowerFieldConflicts == 0
                    ? "PowerFieldConflicts=0 means there are no unresolved enhancement-policy blockers in this apply run."
                    : "Remaining PowerFieldConflicts are unresolved enhancement-policy blockers."
            ]));

        buckets.Add(CreateBucket(
            key: "powerset-icons",
            label: "Powerset icon audit",
            status: result.PoolIconIntegrityIssues > 0 || result.PoolPowerLinkIntegrityIssues > 0
                    ? OmniReportBucketStatusKind.NeedsImplementation
                    : result.PoolPowersetIconsMissingAssets > 0
                        ? OmniReportBucketStatusKind.InformationalReportOnly
                        : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "This bucket is about icon assets and icon/link integrity inside the editor, not combat-math correctness.",
            actionable: result.PoolIconIntegrityIssues > 0 || result.PoolPowerLinkIntegrityIssues > 0 || result.PoolPowersetIconsMissingAssets > 0,
            recommendedAction: result.PoolIconIntegrityIssues > 0 || result.PoolPowerLinkIntegrityIssues > 0
                ? "Fix the remaining icon or pool-link integrity issues."
                : result.PoolPowersetIconsMissingAssets > 0
                    ? "Optional editor-content cleanup: review the missing icon asset/name mismatches and decide whether they are real gaps or naming/reporting noise."
                    : string.Empty,
            snapshot: $"Icons preserved/assigned/missing={result.PoolPowersetIconsPreserved:n0}/{result.PoolPowersetIconsAssigned:n0}/{result.PoolPowersetIconsMissingAssets:n0}, icon integrity issues={result.PoolIconIntegrityIssues:n0}, pool link integrity issues={result.PoolPowerLinkIntegrityIssues:n0}",
            highlights:
            [
                "Missing asset counts are editor-content cleanup tasks unless accompanied by real icon or link integrity failures."
            ]));

        buckets.Add(CreateBucket(
            key: "enhancement-reconciliation",
            label: "Enhancement / recipe / salvage reconciliation",
            status: result.UnresolvedEnhancementPowerLinks.Count > 0
                    || result.MissingBoostPowersAfterImport > 0
                    || result.MissingSetBonusPowersAfterImport > 0
                    || result.EnhancementClassIdsUnresolved > 0
                    || result.BoostPowerLegalityRepairUnresolved > 0
                    || result.ScopedPowerEnhancementLegalityUnresolvedAfterRebuild > 0
                ? OmniReportBucketStatusKind.NeedsImplementation
                : result.EnhancementAmbiguousMatches + result.EnhancementSetsAmbiguousMatches + result.RecipeAmbiguousMatches + result.SalvageAmbiguousMatches > 0
                    || result.ClassicEnhancementMetadataWarnings > 0
                        ? OmniReportBucketStatusKind.NeedsManualReview
                        : result.EnhancementAliasMatches + result.EnhancementFallbackMatches + result.RecipeAliasMatches + result.RecipeFallbackMatches + result.SalvageAliasMatches + result.SalvageFallbackMatches > 0
                            ? OmniReportBucketStatusKind.ImplementedWithFallback
                            : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "Enhancement, recipe, and salvage reconciliation is where naming, linking, legality, and editor-shape compatibility meet. Alias/fallback matches are deliberate compatibility behavior; unresolved links and missing post-import powers are the real blockers.",
            actionable: result.UnresolvedEnhancementPowerLinks.Count > 0
                || result.MissingBoostPowersAfterImport > 0
                || result.MissingSetBonusPowersAfterImport > 0
                || result.EnhancementClassIdsUnresolved > 0
                || result.BoostPowerLegalityRepairUnresolved > 0
                || result.ScopedPowerEnhancementLegalityUnresolvedAfterRebuild > 0
                || result.EnhancementAmbiguousMatches + result.EnhancementSetsAmbiguousMatches + result.RecipeAmbiguousMatches + result.SalvageAmbiguousMatches > 0
                || result.ClassicEnhancementMetadataWarnings > 0,
            recommendedAction: result.UnresolvedEnhancementPowerLinks.Count > 0
                || result.MissingBoostPowersAfterImport > 0
                || result.MissingSetBonusPowersAfterImport > 0
                || result.EnhancementClassIdsUnresolved > 0
                || result.BoostPowerLegalityRepairUnresolved > 0
                || result.ScopedPowerEnhancementLegalityUnresolvedAfterRebuild > 0
                ? "Fix unresolved enhancement links, legality rebuild gaps, or post-import missing boost/set-bonus powers."
                : result.EnhancementAmbiguousMatches + result.EnhancementSetsAmbiguousMatches + result.RecipeAmbiguousMatches + result.SalvageAmbiguousMatches > 0
                    || result.ClassicEnhancementMetadataWarnings > 0
                    ? "Review the ambiguous enhancement/recipe/salvage matches and classic enhancement metadata warnings."
                    : string.Empty,
            snapshot: $"Unresolved enhancement links={result.UnresolvedEnhancementPowerLinks.Count:n0}, missing boost/set-bonus powers after import={result.MissingBoostPowersAfterImport + result.MissingSetBonusPowersAfterImport:n0}, ambiguous matches={result.EnhancementAmbiguousMatches + result.EnhancementSetsAmbiguousMatches + result.RecipeAmbiguousMatches + result.SalvageAmbiguousMatches:n0}, class-derivation unresolved={result.EnhancementClassIdsUnresolved:n0}",
            highlights:
            [
                "Update/create counts are normal reconciliation output and are not automatically unresolved work."
            ]));

        buckets.Add(CreateBucket(
            key: "known-unsupported-and-deferred",
            label: "Known unsupported or deferred importer items",
            status: result.KnownUnsupportedEffectMappings > 0 || result.DeferredPowerFields > 0
                ? OmniReportBucketStatusKind.KnownUnsupported
                : OmniReportBucketStatusKind.HealthyImplemented,
            meaning: "These items are intentionally classified as known limitations or deferred policy work rather than unresolved import failures.",
            actionable: result.KnownUnsupportedEffectMappings > 0 || result.DeferredPowerFields > 0,
            recommendedAction: "Only prioritize these if you want to expand planner feature coverage; they are not active import blockers.",
            snapshot: $"Known unsupported effect mappings={result.KnownUnsupportedEffectMappings:n0}, deferred power fields={result.DeferredPowerFields:n0}",
            highlights:
            [
                "Known unsupported items stay out of the main blocker list unless they are the only work left."
            ]));

        return FinalizeSummary(buckets);
    }

    public static OmniReportEnvelopeSummary BuildSafeImportSummary(OmniImportReport dryRunReport, OmniApplyResult applyResult)
    {
        var dryRunSummary = BuildSummary(dryRunReport);
        var applySummary = BuildSummary(applyResult);
        var combinedBuckets = dryRunSummary.Buckets.Concat(applySummary.Buckets).ToList();
        var overallStatus = DetermineOverallStatus(combinedBuckets);
        var nextActions = BuildNextActions(combinedBuckets);

        var summary = new OmniReportEnvelopeSummary
        {
            OverallStatusKind = overallStatus,
            Overview = BuildOverview(overallStatus),
            DryRun = dryRunSummary,
            Apply = applySummary
        };
        summary.NextActions.AddRange(nextActions);
        return summary;
    }

    public static void AppendMarkdownSummary(StringBuilder builder, OmniReportSummary summary)
    {
        builder.AppendLine("## Overall Import Result");
        builder.AppendLine();
        builder.AppendLine($"- Status: {summary.OverallStatus}");
        builder.AppendLine($"- Summary: {summary.Overview}");

        builder.AppendLine();
        builder.AppendLine("### Next Work");
        builder.AppendLine();
        if (summary.NextActions.Count == 0)
        {
            builder.AppendLine("- No further action is recommended.");
        }
        else
        {
            foreach (var action in summary.NextActions)
            {
                builder.AppendLine($"- {action}");
            }
        }

        AppendMarkdownBucketSection(
            builder,
            "Imported",
            summary.Imported,
            "No imported coverage buckets were recorded.");

        AppendMarkdownBucketSection(
            builder,
            "Not Imported / Broken",
            summary.NotImportedOrBroken,
            "No missing or broken import buckets remain.");

        AppendMarkdownBucketSection(
            builder,
            "Not Modeled Yet",
            summary.NotModeledYet,
            "No known unsupported coverage gaps remain.");

        AppendMarkdownBucketSection(
            builder,
            "Ignored By Design",
            summary.IgnoredByDesign,
            "No ignored-by-design buckets were recorded.");

        AppendMarkdownBucketSection(
            builder,
            "Optional Cleanup",
            summary.OptionalCleanup,
            "No optional cleanup items were identified.");
    }

    public static void AppendMarkdownAppendixHeader(StringBuilder builder)
    {
        builder.AppendLine();
        builder.AppendLine("## Detailed Appendices");
        builder.AppendLine();
        builder.AppendLine("The sections below preserve the raw audit trails and sampled evidence, grouped so implemented, informational, and actionable details do not compete with each other.");
    }

    public static void AppendMarkdownAppendixGroup(StringBuilder builder, string title)
    {
        builder.AppendLine();
        builder.AppendLine($"### {title}");
        builder.AppendLine();
    }

    public static void AppendIgnoredPowerFieldNarrative(StringBuilder builder, int harmlessCount, int uiClientServerCount, int unsupportedPolicyCount)
    {
        builder.AppendLine("#### Ignored Power Field Meaning");
        builder.AppendLine();
        builder.AppendLine($"- Harmless source/export metadata: {harmlessCount:n0}");
        builder.AppendLine("- These entries include cached or display-oriented fields such as attrib_cache, display_fullname, icon, and short_name.");
        builder.AppendLine("- attrib_cache entries containing names like Lethal_Dmg or Smashing_Dmg are cached source hints, not dropped combat math.");
        builder.AppendLine($"- UI / client / server behavior not modeled by planner: {uiClientServerCount:n0}");
        builder.AppendLine("- These entries cover tray, highlight, message, confirmation, stance, and similar behavior that the planner intentionally does not model.");
        builder.AppendLine($"- Known unsupported planner policy fields: {unsupportedPolicyCount:n0}");
        builder.AppendLine("- These entries come from strengths_disallowed/global_strengths_disallowed cases for unsupported axes such as Radius, Arc, or TimeToRoot.");
    }

    public static (int HarmlessSourceMetadataCount, int UiClientServerMetadataCount, int UnsupportedPlannerPolicyCount) BuildIgnoredPowerFieldBreakdown(
        int totalCount,
        IReadOnlyDictionary<string, int> fieldNameCounts)
    {
        var harmless = SumCounts(fieldNameCounts, HarmlessSourceMetadataFields);
        var unsupportedPolicy = SumCounts(fieldNameCounts, KnownUnsupportedPlannerPolicyFields);
        var uiClientServer = Math.Max(0, totalCount - harmless - unsupportedPolicy);
        return (harmless, uiClientServer, unsupportedPolicy);
    }

    private static OmniReportSummary FinalizeSummary(IEnumerable<OmniReportBucketSummary> buckets)
    {
        var materialized = buckets.ToList();
        var overallStatus = DetermineOverallStatus(materialized);
        var imported = materialized.Where(IsImportedBucket).ToList();
        var notImported = materialized.Where(IsBlockingBucket).ToList();
        var notModeledYet = materialized.Where(IsKnownUnsupportedBucket).ToList();
        var optionalCleanup = materialized.Where(IsOptionalCleanupBucket).ToList();
        var ignoredByDesign = materialized
            .Where(bucket => IsIgnoredByDesignBucket(bucket) && !optionalCleanup.Contains(bucket))
            .ToList();
        var summary = new OmniReportSummary
        {
            OverallStatusKind = overallStatus,
            Overview = BuildOverview(overallStatus)
        };
        summary.Imported.AddRange(imported);
        summary.NotImportedOrBroken.AddRange(notImported);
        summary.NotModeledYet.AddRange(notModeledYet);
        summary.IgnoredByDesign.AddRange(ignoredByDesign);
        summary.OptionalCleanup.AddRange(optionalCleanup);
        summary.Buckets.AddRange(materialized);
        summary.NextActions.AddRange(BuildNextActions(materialized));
        return summary;
    }

    private static OmniReportOverallStatusKind DetermineOverallStatus(IEnumerable<OmniReportBucketSummary> buckets)
    {
        var materialized = buckets.ToList();
        if (materialized.Any(bucket => bucket.StatusKind == OmniReportBucketStatusKind.NeedsImplementation))
        {
            return OmniReportOverallStatusKind.BlockedByNeedsImplementation;
        }

        if (materialized.Any(bucket => bucket.StatusKind == OmniReportBucketStatusKind.NeedsManualReview))
        {
            return OmniReportOverallStatusKind.NeedsManualReview;
        }

        if (materialized.Any(bucket => bucket.StatusKind is OmniReportBucketStatusKind.KnownUnsupported or OmniReportBucketStatusKind.InformationalReportOnly))
        {
            return OmniReportOverallStatusKind.HealthyWithKnownUnsupportedOrAuditOnlyItems;
        }

        return OmniReportOverallStatusKind.Healthy;
    }

    private static string BuildOverview(OmniReportOverallStatusKind overallStatus)
    {
        return overallStatus switch
        {
            OmniReportOverallStatusKind.BlockedByNeedsImplementation => "Some planner-relevant content still did not import or still has unresolved mapping gaps. Fix the items under Not Imported / Broken first.",
            OmniReportOverallStatusKind.NeedsManualReview => "The import is close, but some planner-relevant items still need review before you can treat the import as fully complete.",
            OmniReportOverallStatusKind.HealthyWithKnownUnsupportedOrAuditOnlyItems => "Everything planner-relevant imported cleanly. The remaining items are either not modeled yet, ignored by design, or optional cleanup.",
            _ => "Everything covered by the importer imported cleanly, and no unsupported or ignored follow-up buckets remain."
        };
    }

    private static List<string> BuildNextActions(IEnumerable<OmniReportBucketSummary> buckets)
    {
        var materialized = buckets.ToList();
        var actionable = materialized
            .Where(bucket => bucket.Actionable && IsBlockingBucket(bucket))
            .Select(bucket => bucket.RecommendedAction)
            .Where(action => !string.IsNullOrWhiteSpace(action))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (actionable.Count > 0)
        {
            return actionable;
        }

        var next = new List<string> { "No import correctness blockers remain." };

        if (materialized.Any(bucket => bucket.Key == "enhancement-policy" && bucket.StatusKind == OmniReportBucketStatusKind.KnownUnsupported))
        {
            next.Add("Optional feature work: support unsupported enhancement-policy axes such as Radius, Arc, and TimeToRoot if editor legality accuracy needs them.");
        }

        if (materialized.Any(bucket => bucket.Key == "powerset-icons" && bucket.Actionable))
        {
            next.Add("Optional cleanup: review missing icon asset/name mismatches and decide whether any real editor assets are still missing.");
        }

        if (materialized.Any(bucket => bucket.Key == "known-unsupported-and-deferred" && bucket.StatusKind == OmniReportBucketStatusKind.KnownUnsupported))
        {
            next.Add("Optional feature work: revisit known unsupported effect mappings and deferred power-field handling only if you want broader planner coverage.");
        }

        if (next.Count > 1)
        {
            return next;
        }

        next[0] = "No import correctness blockers remain; only ignored-by-design diagnostics remain.";
        return next;
    }

    private static bool IsBlockingBucket(OmniReportBucketSummary bucket)
    {
        return bucket.StatusKind is OmniReportBucketStatusKind.NeedsImplementation or OmniReportBucketStatusKind.NeedsManualReview;
    }

    private static bool IsImportedBucket(OmniReportBucketSummary bucket)
    {
        return bucket.StatusKind is OmniReportBucketStatusKind.HealthyImplemented or OmniReportBucketStatusKind.ImplementedWithFallback;
    }

    private static bool IsKnownUnsupportedBucket(OmniReportBucketSummary bucket)
    {
        return bucket.StatusKind == OmniReportBucketStatusKind.KnownUnsupported;
    }

    private static bool IsIgnoredByDesignBucket(OmniReportBucketSummary bucket)
    {
        return bucket.StatusKind == OmniReportBucketStatusKind.InformationalReportOnly;
    }

    private static bool IsOptionalCleanupBucket(OmniReportBucketSummary bucket)
    {
        return bucket.Actionable
            && bucket.StatusKind == OmniReportBucketStatusKind.InformationalReportOnly;
    }

    private static OmniReportBucketSummary CreateBucket(
        string key,
        string label,
        OmniReportBucketStatusKind status,
        string meaning,
        bool actionable,
        string recommendedAction,
        string snapshot,
        IEnumerable<string>? highlights = null)
    {
        var bucket = new OmniReportBucketSummary
        {
            Key = key,
            Label = label,
            StatusKind = status,
            Meaning = meaning,
            Actionable = actionable,
            RecommendedAction = recommendedAction,
            Snapshot = snapshot
        };

        if (highlights != null)
        {
            foreach (var highlight in highlights.Where(static value => !string.IsNullOrWhiteSpace(value)))
            {
                bucket.Highlights.Add(highlight);
            }
        }

        return bucket;
    }

    private static void AppendMarkdownBucketSection(
        StringBuilder builder,
        string title,
        IEnumerable<OmniReportBucketSummary> buckets,
        string emptyMessage)
    {
        var materialized = buckets.ToList();

        builder.AppendLine();
        builder.AppendLine($"## {title}");
        builder.AppendLine();

        if (materialized.Count == 0)
        {
            builder.AppendLine($"- {emptyMessage}");
            return;
        }

        foreach (var bucket in materialized)
        {
            builder.AppendLine($"- {bucket.Label} [{bucket.Status}]: {bucket.Snapshot}");
            builder.AppendLine($"  {bucket.Meaning}");

            if (bucket.Actionable && !string.IsNullOrWhiteSpace(bucket.RecommendedAction))
            {
                builder.AppendLine($"  Recommended next step: {bucket.RecommendedAction}");
            }

            foreach (var highlight in bucket.Highlights)
            {
                builder.AppendLine($"  Note: {highlight}");
            }
        }
    }

    private static int SumCounts(IReadOnlyDictionary<string, int> values, IReadOnlySet<string> keys)
    {
        return values
            .Where(pair => keys.Contains(pair.Key))
            .Sum(pair => pair.Value);
    }
}

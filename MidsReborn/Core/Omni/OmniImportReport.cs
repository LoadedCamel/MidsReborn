using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Omni;

public sealed class OmniImportReport
{
    public int ArchetypesRead { get; set; }
    public int PlayableArchetypesRead { get; set; }
    public int RetainedArchetypesRead { get; set; }
    public int SkippedArchetypesRead { get; set; }
    public int ClassAttributeTablesImported { get; set; }
    public int PowersConsidered { get; set; }
    public int PowersInScope { get; set; }
    public int PowersSkippedOutOfScope { get; set; }
    public int PowerRequirements { get; set; }
    public int TargetRequirements { get; set; }
    public int PowerTargetRequiresManualReviewCount { get; set; }
    public int ActivateRequirementsIgnored { get; set; }
    public int RedirectRules { get; set; }
    public int EffectsVisited { get; set; }
    public int EffectTemplatesVisited { get; set; }
    public int EffectRequiresExpressions { get; set; }
    public int JitRequiresExpressions { get; set; }
    public int ChildEffectsVisited { get; set; }
    public int ParamsVisited { get; set; }
    public int EntCreateParams { get; set; }
    public int DynamicMaxTargetExpressions { get; set; }
    public int ChainExpressions { get; set; }
    public int EntitiesRead { get; set; }
    public int EntityReferences { get; set; }
    public int EntityReferencesResolved { get; set; }
    public int EntityReferencesUnresolved { get; set; }
    public int PseudoEntCreatesWithoutEntityDef { get; set; }
    public int RealPetActors { get; set; }
    public int PseudoPetActors { get; set; }
    public int BuildEvaluatedExpressionCount { get; set; }
    public int RuntimeTargetExpressionCount { get; set; }
    public int ReportOnlyExpressionCount { get; set; }
    public int ReportOnlyPowerRequirementFragmentCount { get; set; }
    public int UnsupportedPowerRequirementCount { get; set; }
    public int RawAdvancedExpressionCount { get; set; }
    public int UnsupportedBuildExpressionCount { get; set; }
    public int UnknownExpressionTokenCount { get; set; }
    public int HiddenSupportPowerCount { get; set; }
    public int VisibleGatedPowerCount { get; set; }
    public int RedirectExecutionVariantCount { get; set; }
    public int ClickBuffPowerCount { get; set; }
    public int ManualClassificationReviewCount { get; set; }
    public int SupportHeavyFilesDiscovered { get; set; }
    public int SupportHeavyFilesInScope { get; set; }
    public int TemporaryPowerFilesDiscovered { get; set; }
    public int TemporaryPowerFilesInScope { get; set; }
    public int InherentPowerFilesDiscovered { get; set; }
    public int InherentPowerFilesInScope { get; set; }
    public int ClassInherentPowerCount { get; set; }
    public int PowerLevelModeGateMappedCount { get; set; }
    public int HiddenByPowerLevelModeGateCount { get; set; }
    public int ConditionModeMappedCount { get; set; }
    public int EffectModePayloadMappedCount { get; set; }
    public int UnknownModeCount { get; set; }
    public int SnipeEngagedAliasCount { get; set; }
    public int PlannerInherentPreservedCount { get; set; }
    public int SyntheticPlannerInherentCount { get; set; }
    public int UnknownEffectModeCount { get; set; }
    public int ModeCatalogEntriesLoaded { get; set; }
    public int PlannerModeDiscoveredCount { get; set; }
    public int SpecialCaseCompatibilityBridgeCount { get; set; }
    public int GcmTagsImportedCount { get; set; }
    public int GcmTagsAlreadyKnownCount { get; set; }
    public int GcmTagsWouldAddCount { get; set; }
    public int GcmDuplicateTagCount { get; set; }
    public int GcmBlankTagCount { get; set; }
    public int EffectGroupTagCount { get; set; }
    public int TemplateFilterTagCount { get; set; }
    public int GlobalChanceModsMappedCount { get; set; }
    public int PowerLocalChanceModsMappedCount { get; set; }
    public int UnsupportedEffectFilterCount { get; set; }
    public int EffectTagMissingFromGcmCount { get; set; }
    public int ClassTableFilesRead { get; set; }
    public int RetainedClassTableFiles { get; set; }
    public int SkippedClassTableFiles { get; set; }
    public int CanonicalNamedTablesLoaded { get; set; }
    public int DuplicateCanonicalNamedTables { get; set; }
    public int MissingClassTableReferences { get; set; }
    public int MissingModifierTableReferences { get; set; }
    public int PowerFieldsMappedCount { get; set; }
    public int PowerFieldsMappedWithFallbackCount { get; set; }
    public int PowerFieldConflictCount { get; set; }
    public int KnownChargeCapacityExtensions { get; set; }
    public int DeferredPowerFieldCount { get; set; }
    public int IgnoredPowerFieldCount { get; set; }
    public int UnknownPowerFieldCount { get; set; }
    public int PoolPowersetIconAuditCount { get; set; }
    public int PoolIconAssignmentsAvailableCount { get; set; }
    public int MissingPoolIconAssetCount { get; set; }
    public int SorceryEnflameTraceCount { get; set; }
    public int PoolRequirementEvaluationFailureCount { get; set; }
    public int PvModeInferredFromTargetEntityCount { get; set; }
    public int PvModeInferredFromTableCount { get; set; }
    public int PvModeAmbiguousCount { get; set; }
    public int PvTargetAuditCount { get; set; }
    public int PvTargetMappingMismatchCount { get; set; }
    public int EnhancementDefinitionsDiscovered { get; set; }
    public int EnhancementSetsDiscovered { get; set; }
    public int EnhancementRecordsExcludedByPolicy { get; set; }
    public int ClassicEnhancementSourceVariantsDiscovered { get; set; }
    public int ClassicEnhancementLogicalRecords { get; set; }
    public int ClassicEnhancementVariantsFolded { get; set; }
    public int RecipeLevelFilesDiscovered { get; set; }
    public int SalvageDefinitionsDiscovered { get; set; }
    public int FoldedEnhancementRecipeCount { get; set; }
    public int EnhancementLevelVariantsDiscovered { get; set; }
    public int RecipeLevelVariantsDiscovered { get; set; }
    public int EnhancementShapeCompatibilityFallbacks { get; set; }
    public int EnhancementMalformedRecordsSkipped { get; set; }
    public int StructuredBoostsAllowedParsedCount { get; set; }
    public int RecipeRewardLinksResolvedDryRun { get; set; }
    public int RecipeRewardLinksMissingDryRun { get; set; }
    public int RecipeRecordsExcludedByPolicy { get; set; }
    public string EnhancementRecipeSourceDirectory { get; set; } = string.Empty;
    public int EnhancementBoostPowerLinksResolvedDryRun { get; set; }
    public int EnhancementBoostPowerLinksMissingDryRun { get; set; }
    public int EnhancementBoostPowerLinksAliasDryRun { get; set; }
    public int EnhancementBoostPowerLinksFallbackDryRun { get; set; }
    public int EnhancementSetBonusLinksResolvedDryRun { get; set; }
    public int EnhancementSetBonusLinksMissingDryRun { get; set; }
    public int EnhancementSetBonusLinksAliasDryRun { get; set; }
    public int EnhancementSetBonusLinksFallbackDryRun { get; set; }
    public int EnhancementPolicyFilesPresent { get; set; }
    public int EnhancementPolicyFilesMissing { get; set; }
    public int EnhancementExactMatchesDryRun { get; set; }
    public int EnhancementAliasMatchesDryRun { get; set; }
    public int EnhancementLinkedPowerMatchesDryRun { get; set; }
    public int EnhancementFallbackMatchesDryRun { get; set; }
    public int EnhancementAmbiguousMatchesDryRun { get; set; }
    public int EnhancementWouldCreateDryRun { get; set; }
    public int EnhancementWouldUpdateDryRun { get; set; }
    public int EnhancementCreateLikelyNewDryRun { get; set; }
    public int EnhancementCreateLikelyNewAttunedDryRun { get; set; }
    public int EnhancementCreateLikelyNewSuperiorAttunedDryRun { get; set; }
    public int EnhancementCreateLikelyNewSetBackedDryRun { get; set; }
    public int EnhancementCreateLikelyNewRecipeBackedDryRun { get; set; }
    public int EnhancementCreateLikelyNewClassicDryRun { get; set; }
    public int EnhancementCreateLikelyNewSpecialOriginDryRun { get; set; }
    public int EnhancementCreateLikelyNewInventionDryRun { get; set; }
    public int InventionCraftedVariantsDiscovered { get; set; }
    public int InventionAttunedVariantsDiscovered { get; set; }
    public int InventionSuperiorVariantsDiscovered { get; set; }
    public int InventionSuperiorAttunedVariantsDiscovered { get; set; }
    public int InventionStandaloneCraftedVariantsDiscovered { get; set; }
    public int InventionStandaloneAttunedVariantsDiscovered { get; set; }
    public int InventionStandaloneSuperiorVariantsDiscovered { get; set; }
    public int InventionStandaloneSuperiorAttunedVariantsDiscovered { get; set; }
    public int IoSetFamiliesCraftedOnly { get; set; }
    public int IoSetFamiliesAttunedOnly { get; set; }
    public int IoSetFamiliesSuperiorOnly { get; set; }
    public int IoSetFamiliesSuperiorAttunedOnly { get; set; }
    public int IoSetFamiliesCraftedAndAttuned { get; set; }
    public int IoSetFamiliesCraftedAndSuperior { get; set; }
    public int IoSetFamiliesCraftedAndSuperiorAttuned { get; set; }
    public int IoSetFamiliesAttunedAndSuperior { get; set; }
    public int IoSetFamiliesAttunedAndSuperiorAttuned { get; set; }
    public int IoSetFamiliesSuperiorAndSuperiorAttuned { get; set; }
    public int IoSetFamiliesCraftedAttunedSuperior { get; set; }
    public int IoSetFamiliesCraftedAttunedSuperiorAttuned { get; set; }
    public int IoSetFamiliesCraftedSuperiorSuperiorAttuned { get; set; }
    public int IoSetFamiliesAttunedSuperiorSuperiorAttuned { get; set; }
    public int IoSetFamiliesAllFourVariants { get; set; }
    public int EnhancementCreateShouldMatchBoostPowerDryRun { get; set; }
    public int EnhancementCreateShouldMatchRecipeDryRun { get; set; }
    public int EnhancementCreateShouldMatchSetMembershipDryRun { get; set; }
    public int EnhancementCreateNoPlausibleExistingMatchDryRun { get; set; }
    public int EnhancementSetExactMatchesDryRun { get; set; }
    public int EnhancementSetAliasMatchesDryRun { get; set; }
    public int EnhancementSetFallbackMatchesDryRun { get; set; }
    public int EnhancementSetAmbiguousMatchesDryRun { get; set; }
    public int EnhancementSetWouldCreateDryRun { get; set; }
    public int EnhancementSetWouldUpdateDryRun { get; set; }
    public int RecipeExactMatchesDryRun { get; set; }
    public int RecipeAliasMatchesDryRun { get; set; }
    public int RecipeFallbackMatchesDryRun { get; set; }
    public int RecipeAmbiguousMatchesDryRun { get; set; }
    public int RecipeWouldCreateDryRun { get; set; }
    public int RecipeWouldUpdateDryRun { get; set; }
    public int RecipeCreateLikelyNewDryRun { get; set; }
    public int RecipeCreateShouldMatchRewardIdentityDryRun { get; set; }
    public int RecipeCreateShouldMatchCanonicalStorageIdentityDryRun { get; set; }
    public int RecipeCreateNoPlausibleExistingMatchDryRun { get; set; }
    public int SalvageExactMatchesDryRun { get; set; }
    public int SalvageAliasMatchesDryRun { get; set; }
    public int SalvageFallbackMatchesDryRun { get; set; }
    public int SalvageAmbiguousMatchesDryRun { get; set; }
    public int SalvageWouldCreateDryRun { get; set; }
    public int SalvageWouldUpdateDryRun { get; set; }
    public int BoostPowersetsInScope { get; set; }
    public int SetBonusPowersetsInScope { get; set; }
    public int BoostExplicitPowersetsInScope { get; set; }
    public int BoostDerivedPowersetsInScope { get; set; }
    public int SetBonusExplicitPowersetsInScope { get; set; }
    public int SetBonusDerivedPowersetsInScope { get; set; }
    public int BoostPowersInScope { get; set; }
    public int SetBonusPowersInScope { get; set; }

    public Dictionary<string, int> PetPowersetFilesDiscovered { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> PetPowersetFilesInScope { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> PetPowerFilesDiscovered { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> PetPowerFilesInScope { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> PetPowersLoaded { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> IgnoredPowerFieldOwnerKindCounts { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> IgnoredPowerFieldCategoryCounts { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> IgnoredPowerFieldNameCounts { get; } = new(StringComparer.OrdinalIgnoreCase);
    public int PetManifestRootsPresent { get; set; }
    public int PetManifestRootsMissing { get; set; }
    public int PetManifestPowersets { get; set; }
    public int PetManifestExpectedPowers { get; set; }
    public int PetManifestPowerFiles { get; set; }
    public int PetManifestMissingPowerFiles { get; set; }
    public int EpicPowersetFilesDiscovered { get; set; }
    public int EpicPowersetFilesInScope { get; set; }
    public int EpicPowerFilesDiscovered { get; set; }
    public int EpicPowerFilesInScope { get; set; }
    public int EpicPowersLoaded { get; set; }

    public List<string> ImportedPowersets { get; } = [];
    public List<string> RetainedArchetypes { get; } = [];
    public List<string> SkippedArchetypes { get; } = [];
    public List<string> SkippedPowerGroups { get; } = [];
    public List<string> IgnoredFields { get; } = [];
    public List<string> BuildEvaluatedExpressions { get; } = [];
    public List<string> RuntimeTargetExpressions { get; } = [];
    public List<string> ReportOnlyExpressions { get; } = [];
    public List<string> PowerTargetRequiresNotImported { get; } = [];
    public List<string> PowerTargetRequiresManualReview { get; } = [];
    public List<string> ReportOnlyPowerRequirementFragments { get; } = [];
    public List<string> UnsupportedPowerRequirements { get; } = [];
    public List<string> RawAdvancedExpressions { get; } = [];
    public List<string> UnsupportedBuildExpressions { get; } = [];
    public List<string> UnknownExpressionTokens { get; } = [];
    public List<string> HiddenSupportPowers { get; } = [];
    public List<string> VisibleGatedPowers { get; } = [];
    public List<string> RedirectExecutionVariants { get; } = [];
    public List<string> ClickBuffClassifications { get; } = [];
    public List<string> ManualClassificationReviews { get; } = [];
    public List<string> SupportHeavyFiles { get; } = [];
    public List<string> ClassInherentPowers { get; } = [];
    public List<string> PowerLevelModeGates { get; } = [];
    public List<string> HiddenByPowerLevelModeGates { get; } = [];
    public List<string> ConditionModes { get; } = [];
    public List<string> EffectModePayloads { get; } = [];
    public List<string> UnknownModes { get; } = [];
    public List<string> SnipeEngagedAliases { get; } = [];
    public List<string> PlannerInherentsPreserved { get; } = [];
    public List<string> SyntheticPlannerInherents { get; } = [];
    public List<string> UnknownEffectModes { get; } = [];
    public List<string> PlannerModesDiscovered { get; } = [];
    public List<string> SpecialCaseCompatibilityBridges { get; } = [];
    public List<string> GcmTags { get; } = [];
    public List<string> GcmTagsAlreadyKnown { get; } = [];
    public List<string> GcmTagsWouldAdd { get; } = [];
    public List<string> GcmDuplicateTags { get; } = [];
    public List<string> EffectGroupTags { get; } = [];
    public List<string> TemplateFilterTags { get; } = [];
    public List<string> ChanceModMappings { get; } = [];
    public List<string> UnsupportedEffectFilters { get; } = [];
    public List<string> EffectTagsMissingFromGcm { get; } = [];
    public List<string> ClassTableFiles { get; } = [];
    public List<string> RetainedClassTableFileDetails { get; } = [];
    public List<string> SkippedClassTableFileDetails { get; } = [];
    public List<string> DuplicateNamedTables { get; } = [];
    public List<string> MissingClassTableReferenceDetails { get; } = [];
    public List<string> MissingModifierTableReferenceDetails { get; } = [];
    public List<string> MappedPowerFields { get; } = [];
    public List<string> PowerFieldsMappedWithFallback { get; } = [];
    public List<string> PowerFieldConflicts { get; } = [];
    public List<string> KnownChargeCapacityDetailEntries { get; } = [];
    public List<string> DeferredPowerFields { get; } = [];
    public List<string> IgnoredPowerFields { get; } = [];
    public List<string> UnknownPowerFields { get; } = [];
    public List<string> PoolPowersetIconAudit { get; } = [];
    public List<string> PoolIconAssignments { get; } = [];
    public List<string> MissingPoolIconAssets { get; } = [];
    public List<string> SorceryEnflamePickabilityTrace { get; } = [];
    public List<string> PoolRequirementEvaluationFailures { get; } = [];
    public List<string> PvTargetGatingAudit { get; } = [];
    public List<string> PvTargetMappingMismatches { get; } = [];
    public List<string> PetImportScopeDetails { get; } = [];
    public List<string> PetImportManifestDetails { get; } = [];
    public List<string> EpicImportScopeDetails { get; } = [];
    public List<string> EpicPowersetIdentityDetails { get; } = [];
    public List<string> EpicPowersetPrefixSuffixCandidates { get; } = [];
    public List<string> EpicPowersetDisplayNameCollisions { get; } = [];
    public List<string> EnhancementImportScanDetails { get; } = [];
    public List<string> ClassicEnhancementFoldingDetails { get; } = [];
    public List<string> EnhancementSourceShapeValidation { get; } = [];
    public List<string> EnhancementMissingPolicyFiles { get; } = [];
    public List<string> EnhancementPowerLinkAudit { get; } = [];
    public List<string> EnhancementReconciliationAudit { get; } = [];
    public List<string> EnhancementReconciliationConflicts { get; } = [];
    public List<string> EnhancementCreateValidationSummary { get; } = [];
    public List<string> InventionVariantAudit { get; } = [];
    public List<string> EnhancementIdentityInvestigation { get; } = [];
    public List<string> EnhancementIdentityLikelyNewSamples { get; } = [];
    public List<string> RecipeIdentityInvestigation { get; } = [];
    public List<string> RecipeIdentityLikelyNewSamples { get; } = [];
    public List<string> ScopedBoostSetBonusCoverage { get; } = [];
    public List<string> UnresolvedRedirects { get; } = [];
    public List<string> UnresolvedEntities { get; } = [];
    public List<string> SkippedEntityReferences { get; } = [];
    public List<string> PseudoPetClassifications { get; } = [];
    public List<string> UnknownEffectMappings { get; } = [];
    public List<string> UnknownAttribMappings { get; } = [];
    public List<string> ChildEffectsNotRepresented { get; } = [];
    public List<string> DynamicExpressionsNotEvaluated { get; } = [];
    public List<string> Samples { get; } = [];

    public void AddSample(string sample)
    {
        if (Samples.Count < 100)
        {
            Samples.Add(sample);
        }
    }

    public void AddLimited(ICollection<string> target, string value, int limit = 500)
    {
        if (string.IsNullOrWhiteSpace(value) || target.Count >= limit || target.Contains(value))
        {
            return;
        }

        target.Add(value);
    }

    public void TrimDetails()
    {
        ImportedPowersets.Clear();
        RetainedArchetypes.Clear();
        SkippedArchetypes.Clear();
        SkippedPowerGroups.Clear();
        IgnoredFields.Clear();
        BuildEvaluatedExpressions.Clear();
        RuntimeTargetExpressions.Clear();
        ReportOnlyExpressions.Clear();
        PowerTargetRequiresNotImported.Clear();
        PowerTargetRequiresManualReview.Clear();
        ReportOnlyPowerRequirementFragments.Clear();
        UnsupportedPowerRequirements.Clear();
        RawAdvancedExpressions.Clear();
        UnsupportedBuildExpressions.Clear();
        UnknownExpressionTokens.Clear();
        HiddenSupportPowers.Clear();
        VisibleGatedPowers.Clear();
        RedirectExecutionVariants.Clear();
        ClickBuffClassifications.Clear();
        ManualClassificationReviews.Clear();
        SupportHeavyFiles.Clear();
        ClassInherentPowers.Clear();
        PowerLevelModeGates.Clear();
        HiddenByPowerLevelModeGates.Clear();
        ConditionModes.Clear();
        EffectModePayloads.Clear();
        UnknownModes.Clear();
        SnipeEngagedAliases.Clear();
        PlannerInherentsPreserved.Clear();
        SyntheticPlannerInherents.Clear();
        UnknownEffectModes.Clear();
        PlannerModesDiscovered.Clear();
        SpecialCaseCompatibilityBridges.Clear();
        GcmTags.Clear();
        GcmTagsAlreadyKnown.Clear();
        GcmTagsWouldAdd.Clear();
        GcmDuplicateTags.Clear();
        EffectGroupTags.Clear();
        TemplateFilterTags.Clear();
        ChanceModMappings.Clear();
        UnsupportedEffectFilters.Clear();
        EffectTagsMissingFromGcm.Clear();
        ClassTableFiles.Clear();
        RetainedClassTableFileDetails.Clear();
        SkippedClassTableFileDetails.Clear();
        DuplicateNamedTables.Clear();
        MissingClassTableReferenceDetails.Clear();
        MissingModifierTableReferenceDetails.Clear();
        MappedPowerFields.Clear();
        PowerFieldsMappedWithFallback.Clear();
        PowerFieldConflicts.Clear();
        KnownChargeCapacityDetailEntries.Clear();
        DeferredPowerFields.Clear();
        IgnoredPowerFields.Clear();
        UnknownPowerFields.Clear();
        PoolPowersetIconAudit.Clear();
        PoolIconAssignments.Clear();
        MissingPoolIconAssets.Clear();
        SorceryEnflamePickabilityTrace.Clear();
        PoolRequirementEvaluationFailures.Clear();
        PvTargetGatingAudit.Clear();
        PvTargetMappingMismatches.Clear();
        PetPowersetFilesDiscovered.Clear();
        PetPowersetFilesInScope.Clear();
        PetPowerFilesDiscovered.Clear();
        PetPowerFilesInScope.Clear();
        PetPowersLoaded.Clear();
        IgnoredPowerFieldOwnerKindCounts.Clear();
        IgnoredPowerFieldCategoryCounts.Clear();
        IgnoredPowerFieldNameCounts.Clear();
        PetManifestRootsPresent = 0;
        PetManifestRootsMissing = 0;
        PetManifestPowersets = 0;
        PetManifestExpectedPowers = 0;
        PetManifestPowerFiles = 0;
        PetManifestMissingPowerFiles = 0;
        PetImportScopeDetails.Clear();
        PetImportManifestDetails.Clear();
        EpicImportScopeDetails.Clear();
        EpicPowersetIdentityDetails.Clear();
        EpicPowersetPrefixSuffixCandidates.Clear();
        EpicPowersetDisplayNameCollisions.Clear();
        EnhancementImportScanDetails.Clear();
        ClassicEnhancementFoldingDetails.Clear();
        EnhancementSourceShapeValidation.Clear();
        EnhancementMissingPolicyFiles.Clear();
        EnhancementPowerLinkAudit.Clear();
        ScopedBoostSetBonusCoverage.Clear();
        EnhancementCreateValidationSummary.Clear();
        UnresolvedRedirects.Clear();
        UnresolvedEntities.Clear();
        SkippedEntityReferences.Clear();
        PseudoPetClassifications.Clear();
        UnknownEffectMappings.Clear();
        UnknownAttribMappings.Clear();
        ChildEffectsNotRepresented.Clear();
        DynamicExpressionsNotEvaluated.Clear();
        Samples.Clear();
    }

    [JsonProperty("summary")]
    public OmniReportSummary Summary => OmniReportLegibility.BuildSummary(this);

    public string ToMarkdown()
    {
        var summary = Summary;
        var ignoredPowerFieldBreakdown = OmniReportLegibility.BuildIgnoredPowerFieldBreakdown(IgnoredPowerFieldCount, IgnoredPowerFieldNameCounts);
        var builder = new StringBuilder();

        builder.AppendLine("# Omni Import Dry Run");
        builder.AppendLine();
        OmniReportLegibility.AppendMarkdownSummary(builder, summary);
        OmniReportLegibility.AppendMarkdownAppendixHeader(builder);

        OmniReportLegibility.AppendMarkdownAppendixGroup(builder, "Raw Metric Snapshot");
        builder.AppendLine($"- Archetypes read/playable/retained/skipped: {ArchetypesRead:n0}/{PlayableArchetypesRead:n0}/{RetainedArchetypesRead:n0}/{SkippedArchetypesRead:n0}");
        builder.AppendLine($"- Powers considered/in scope/skipped: {PowersConsidered:n0}/{PowersInScope:n0}/{PowersSkippedOutOfScope:n0}");
        builder.AppendLine($"- Effects/templates/child effects visited: {EffectsVisited:n0}/{EffectTemplatesVisited:n0}/{ChildEffectsVisited:n0}");
        builder.AppendLine($"- Requirements visited/activate ignored: {PowerRequirements:n0}/{ActivateRequirementsIgnored:n0}");
        builder.AppendLine($"- Entities read/real pets/pseudo pets: {EntitiesRead:n0}/{RealPetActors:n0}/{PseudoPetActors:n0}");
        builder.AppendLine($"- GCM imported/already-known/would-add: {GcmTagsImportedCount:n0}/{GcmTagsAlreadyKnownCount:n0}/{GcmTagsWouldAddCount:n0}");
        builder.AppendLine($"- Power fields mapped/fallback/conflicts/unknown: {PowerFieldsMappedCount:n0}/{PowerFieldsMappedWithFallbackCount:n0}/{PowerFieldConflictCount:n0}/{UnknownPowerFieldCount:n0}");
        builder.AppendLine($"- Enhancement files/sets/recipes discovered: {EnhancementDefinitionsDiscovered:n0}/{EnhancementSetsDiscovered:n0}/{FoldedEnhancementRecipeCount:n0}");
        builder.AppendLine($"- Enhancement reconciliation update/create/ambiguous: {EnhancementWouldUpdateDryRun:n0}/{EnhancementWouldCreateDryRun:n0}/{EnhancementAmbiguousMatchesDryRun:n0}");
        builder.AppendLine($"- Recipe reconciliation update/create/ambiguous: {RecipeWouldUpdateDryRun:n0}/{RecipeWouldCreateDryRun:n0}/{RecipeAmbiguousMatchesDryRun:n0}");
        builder.AppendLine($"- Salvage reconciliation update/create/ambiguous: {SalvageWouldUpdateDryRun:n0}/{SalvageWouldCreateDryRun:n0}/{SalvageAmbiguousMatchesDryRun:n0}");
        builder.AppendLine($"- Pet manifest roots present/missing: {PetManifestRootsPresent:n0}/{PetManifestRootsMissing:n0}");

        OmniReportLegibility.AppendMarkdownAppendixGroup(builder, "Needs Implementation / Manual Review");
        AppendSection(builder, "Power Target Requires Not Imported", PowerTargetRequiresNotImported);
        AppendSection(builder, "Power Target Requires Manual Review", PowerTargetRequiresManualReview);
        AppendSection(builder, "Unsupported Build Expressions", UnsupportedBuildExpressions);
        AppendSection(builder, "Unknown Expression Tokens", UnknownExpressionTokens);
        AppendSection(builder, "Unsupported Power Requirements", UnsupportedPowerRequirements);
        AppendSection(builder, "Manual Classification Review", ManualClassificationReviews);
        AppendSection(builder, "Unknown Omni Modes", UnknownModes);
        AppendSection(builder, "Unknown Effect Modes", UnknownEffectModes);
        AppendSection(builder, "Unknown Effect Mappings", UnknownEffectMappings);
        AppendSection(builder, "Unknown Attribute Mappings", UnknownAttribMappings);
        AppendSection(builder, "Unknown Power Fields", UnknownPowerFields);
        AppendSection(builder, "Missing Class Table References", MissingClassTableReferenceDetails);
        AppendSection(builder, "Missing Modifier Table References", MissingModifierTableReferenceDetails);
        AppendSection(builder, "Missing Powerset Icon Assets", MissingPoolIconAssets);
        AppendSection(builder, "Pool Requirement Evaluation Failures", PoolRequirementEvaluationFailures);
        AppendSection(builder, "Enhancement Missing Policy Files", EnhancementMissingPolicyFiles);
        AppendSection(builder, "Enhancement Reconciliation Conflicts", EnhancementReconciliationConflicts);

        OmniReportLegibility.AppendMarkdownAppendixGroup(builder, "Known Unsupported / Deferred");
        AppendSection(builder, "Known Deferred Power Fields", DeferredPowerFields);
        AppendSection(builder, "Known Charge-Capacity Extensions", KnownChargeCapacityDetailEntries);
        AppendSection(builder, "Unsupported EffectFilter Fields", UnsupportedEffectFilters);
        OmniReportLegibility.AppendIgnoredPowerFieldNarrative(
            builder,
            ignoredPowerFieldBreakdown.HarmlessSourceMetadataCount,
            ignoredPowerFieldBreakdown.UiClientServerMetadataCount,
            ignoredPowerFieldBreakdown.UnsupportedPlannerPolicyCount);

        OmniReportLegibility.AppendMarkdownAppendixGroup(builder, "Informational / Report-Only");
        AppendSection(builder, "Build-Evaluated Expressions", BuildEvaluatedExpressions);
        AppendSection(builder, "Runtime-Target Expressions", RuntimeTargetExpressions);
        AppendSection(builder, "Report-Only Expressions", ReportOnlyExpressions);
        AppendSection(builder, "Report-Only Power Requirement Fragments", ReportOnlyPowerRequirementFragments);
        AppendSection(builder, "Raw Advanced Expressions", RawAdvancedExpressions);
        AppendSection(builder, "Ignored Source Directives / Report-Only Metadata", IgnoredFields);
        AppendSection(builder, "Effect Tags Referenced Outside gcm.json", EffectTagsMissingFromGcm);
        AppendSection(builder, "GCM Tags Imported", GcmTags);
        AppendSection(builder, "GCM Tags Already Known", GcmTagsAlreadyKnown);
        AppendSection(builder, "GCM Tags That Would Be Added", GcmTagsWouldAdd);
        AppendSection(builder, "GCM Duplicate Tags", GcmDuplicateTags);
        AppendSection(builder, "Effect Group Tags Preserved", EffectGroupTags);
        AppendSection(builder, "Template / Filter Tags Mapped", TemplateFilterTags);
        AppendSection(builder, "Chance Mod Mappings", ChanceModMappings);
        AppendCountSection(builder, "Ignored Power Field Owner Kinds", IgnoredPowerFieldOwnerKindCounts);
        AppendCountSection(builder, "Ignored Power Field Categories", IgnoredPowerFieldCategoryCounts);
        AppendCountSection(builder, "Ignored Power Fields By Name", IgnoredPowerFieldNameCounts);
        AppendSection(builder, "Ignored Power Field Samples", IgnoredPowerFields);
        AppendSection(builder, "Powerset Icon Audit", PoolPowersetIconAudit);
        AppendSection(builder, "Powerset Icon Assignments", PoolIconAssignments);
        AppendSection(builder, "Sorcery Enflame Pickability Trace", SorceryEnflamePickabilityTrace);
        AppendSection(builder, "PvX Target And Gating Audit", PvTargetGatingAudit);
        AppendSection(builder, "PvX Target Mapping Mismatches", PvTargetMappingMismatches);

        OmniReportLegibility.AppendMarkdownAppendixGroup(builder, "Implemented Coverage / Audit Trails");
        AppendSection(builder, "Pet Import Scope", PetImportScopeDetails);
        AppendSection(builder, "Pet Import Manifest", PetImportManifestDetails);
        AppendSection(builder, "Power Field Mapping Coverage", MappedPowerFields);
        AppendSection(builder, "Mapped With Fallback", PowerFieldsMappedWithFallback);
        AppendSection(builder, "Enhancement Import Scan", EnhancementImportScanDetails);
        AppendSection(builder, "Classic Enhancement Folding", ClassicEnhancementFoldingDetails);
        AppendSection(builder, "Enhancement Source Shape Validation", EnhancementSourceShapeValidation);
        AppendSection(builder, "Boosts And Set Bonus Scope Coverage", ScopedBoostSetBonusCoverage);
        AppendSection(builder, "Enhancement Power Link Audit", EnhancementPowerLinkAudit);
        AppendSection(builder, "Enhancement Reconciliation Audit", EnhancementReconciliationAudit);
        AppendSection(builder, "Enhancement Create Validation", EnhancementCreateValidationSummary);
        AppendSection(builder, "Invention Variant Audit", InventionVariantAudit);
        AppendSection(builder, "Enhancement Identity Investigation", EnhancementIdentityInvestigation);
        AppendSection(builder, "Enhancement Identity Likely New Samples", EnhancementIdentityLikelyNewSamples);
        AppendSection(builder, "Recipe Identity Investigation", RecipeIdentityInvestigation);
        AppendSection(builder, "Recipe Identity Likely New Samples", RecipeIdentityLikelyNewSamples);

        return builder.ToString();
    }

    private string BuildLegacyMarkdown()
    {
        static string WithSample(int count, int sampleCount)
        {
            return count == 0
                ? "0"
                : $"{count} (sampled {sampleCount})";
        }

        var builder = new StringBuilder();
        builder.AppendLine("# Omni Import Dry Run");
        builder.AppendLine();
        builder.AppendLine($"- Archetypes read: {ArchetypesRead}");
        builder.AppendLine($"- Playable archetypes read: {PlayableArchetypesRead}");
        builder.AppendLine($"- Retained archetypes: {WithSample(RetainedArchetypesRead, RetainedArchetypes.Count)}");
        builder.AppendLine($"- Skipped archetypes: {WithSample(SkippedArchetypesRead, SkippedArchetypes.Count)}");
        builder.AppendLine($"- Class attribute tables imported: {ClassAttributeTablesImported}");
        builder.AppendLine($"- Powers considered: {PowersConsidered}");
        builder.AppendLine($"- Powers in scope: {PowersInScope}");
        builder.AppendLine($"- Powers skipped out of scope: {PowersSkippedOutOfScope}");
        builder.AppendLine($"- Power requirements: {PowerRequirements}");
        builder.AppendLine($"- Power target_requires not imported: {WithSample(TargetRequirements, PowerTargetRequiresNotImported.Count)}");
        builder.AppendLine($"- Power target_requires manual review: {WithSample(PowerTargetRequiresManualReviewCount, PowerTargetRequiresManualReview.Count)}");
        builder.AppendLine($"- Activate requirements ignored: {ActivateRequirementsIgnored}");
        builder.AppendLine($"- Redirect rules: {RedirectRules}");
        builder.AppendLine($"- Effects visited: {EffectsVisited}");
        builder.AppendLine($"- Effect templates visited: {EffectTemplatesVisited}");
        builder.AppendLine($"- Effect requires expressions: {EffectRequiresExpressions}");
        builder.AppendLine($"- JIT requires expressions: {JitRequiresExpressions}");
        builder.AppendLine($"- Child effects visited: {ChildEffectsVisited}");
        builder.AppendLine($"- Params visited: {ParamsVisited}");
        builder.AppendLine($"- EntCreate params: {EntCreateParams}");
        builder.AppendLine($"- Dynamic max target expressions: {DynamicMaxTargetExpressions}");
        builder.AppendLine($"- Chain expressions: {ChainExpressions}");
        builder.AppendLine($"- Entity references: {EntityReferences}");
        builder.AppendLine($"- Entity references resolved: {EntityReferencesResolved}");
        builder.AppendLine($"- Entity references unresolved: {EntityReferencesUnresolved}");
        builder.AppendLine($"- Pseudo EntCreate without entity_def: {PseudoEntCreatesWithoutEntityDef}");
        builder.AppendLine($"- Entities read: {EntitiesRead}");
        builder.AppendLine($"- Real pet actors: {RealPetActors}");
        builder.AppendLine($"- Pseudo pet actors: {PseudoPetActors}");
        builder.AppendLine($"- Build-evaluated expressions: {WithSample(BuildEvaluatedExpressionCount, BuildEvaluatedExpressions.Count)}");
        builder.AppendLine($"- Runtime target expressions preserved: {WithSample(RuntimeTargetExpressionCount, RuntimeTargetExpressions.Count)}");
        builder.AppendLine($"- Report-only expressions preserved: {WithSample(ReportOnlyExpressionCount, ReportOnlyExpressions.Count)}");
        builder.AppendLine($"- Report-only power requirement fragments stripped: {WithSample(ReportOnlyPowerRequirementFragmentCount, ReportOnlyPowerRequirementFragments.Count)}");
        builder.AppendLine($"- Unsupported power requirements: {WithSample(UnsupportedPowerRequirementCount, UnsupportedPowerRequirements.Count)}");
        builder.AppendLine($"- Raw advanced expressions: {WithSample(RawAdvancedExpressionCount, RawAdvancedExpressions.Count)}");
        builder.AppendLine($"- Unsupported build expressions: {WithSample(UnsupportedBuildExpressionCount, UnsupportedBuildExpressions.Count)}");
        builder.AppendLine($"- Unknown expression tokens: {WithSample(UnknownExpressionTokenCount, UnknownExpressionTokens.Count)}");
        builder.AppendLine($"- Hidden support powers: {WithSample(HiddenSupportPowerCount, HiddenSupportPowers.Count)}");
        builder.AppendLine($"- Visible gated powers: {WithSample(VisibleGatedPowerCount, VisibleGatedPowers.Count)}");
        builder.AppendLine($"- Redirect execution variants: {WithSample(RedirectExecutionVariantCount, RedirectExecutionVariants.Count)}");
        builder.AppendLine($"- Click-buff classifications: {WithSample(ClickBuffPowerCount, ClickBuffClassifications.Count)}");
        builder.AppendLine($"- Manual classification review: {WithSample(ManualClassificationReviewCount, ManualClassificationReviews.Count)}");
        builder.AppendLine($"- Support-heavy files discovered: {SupportHeavyFilesDiscovered}");
        builder.AppendLine($"- Support-heavy files in scope: {SupportHeavyFilesInScope}");
        builder.AppendLine($"- Temporary power files discovered: {TemporaryPowerFilesDiscovered}");
        builder.AppendLine($"- Temporary power files in scope: {TemporaryPowerFilesInScope}");
        builder.AppendLine($"- Inherent power files discovered: {InherentPowerFilesDiscovered}");
        builder.AppendLine($"- Inherent power files in scope: {InherentPowerFilesInScope}");
        builder.AppendLine($"- Class inherents classified: {WithSample(ClassInherentPowerCount, ClassInherentPowers.Count)}");
        builder.AppendLine($"- Power-level mode gates mapped: {WithSample(PowerLevelModeGateMappedCount, PowerLevelModeGates.Count)}");
        builder.AppendLine($"- Hidden by power-level mode gates: {WithSample(HiddenByPowerLevelModeGateCount, HiddenByPowerLevelModeGates.Count)}");
        builder.AppendLine($"- Effect/redirect mode conditions mapped: {WithSample(ConditionModeMappedCount, ConditionModes.Count)}");
        builder.AppendLine($"- Effect mode payloads mapped: {WithSample(EffectModePayloadMappedCount, EffectModePayloads.Count)}");
        builder.AppendLine($"- kEngaged snipe aliases: {WithSample(SnipeEngagedAliasCount, SnipeEngagedAliases.Count)}");
        builder.AppendLine($"- Planner inherents preserved: {WithSample(PlannerInherentPreservedCount, PlannerInherentsPreserved.Count)}");
        builder.AppendLine($"- Synthetic planner inherents: {WithSample(SyntheticPlannerInherentCount, SyntheticPlannerInherents.Count)}");
        builder.AppendLine($"- Unknown effect modes: {WithSample(UnknownEffectModeCount, UnknownEffectModes.Count)}");
        builder.AppendLine($"- Mode catalog entries loaded: {ModeCatalogEntriesLoaded}");
        builder.AppendLine($"- Planner modes discovered: {WithSample(PlannerModeDiscoveredCount, PlannerModesDiscovered.Count)}");
        builder.AppendLine($"- Unknown Omni modes: {WithSample(UnknownModeCount, UnknownModes.Count)}");
        builder.AppendLine($"- GCM tags imported: {WithSample(GcmTagsImportedCount, GcmTags.Count)}");
        builder.AppendLine($"- GCM tags already in Mids: {WithSample(GcmTagsAlreadyKnownCount, GcmTagsAlreadyKnown.Count)}");
        builder.AppendLine($"- GCM tags that would be added: {WithSample(GcmTagsWouldAddCount, GcmTagsWouldAdd.Count)}");
        builder.AppendLine($"- GCM duplicate tags ignored: {WithSample(GcmDuplicateTagCount, GcmDuplicateTags.Count)}");
        builder.AppendLine($"- GCM blank tags ignored: {GcmBlankTagCount}");
        builder.AppendLine($"- Effect-group tags preserved: {WithSample(EffectGroupTagCount, EffectGroupTags.Count)}");
        builder.AppendLine($"- Template/filter tags mapped: {WithSample(TemplateFilterTagCount, TemplateFilterTags.Count)}");
        builder.AppendLine($"- Global chance mods mapped: {WithSample(GlobalChanceModsMappedCount, ChanceModMappings.Count(detail => detail.Contains("global chance mod", StringComparison.OrdinalIgnoreCase)))}");
        builder.AppendLine($"- Power-local chance mods mapped: {WithSample(PowerLocalChanceModsMappedCount, ChanceModMappings.Count(detail => detail.Contains("power-local chance mod", StringComparison.OrdinalIgnoreCase)))}");
        builder.AppendLine($"- Unsupported EffectFilter fields preserved: {WithSample(UnsupportedEffectFilterCount, UnsupportedEffectFilters.Count)}");
        builder.AppendLine($"- Effect tags missing from gcm.json: {WithSample(EffectTagMissingFromGcmCount, EffectTagsMissingFromGcm.Count)}");
        builder.AppendLine($"- Class table files read: {WithSample(ClassTableFilesRead, ClassTableFiles.Count)}");
        builder.AppendLine($"- Retained class table files: {WithSample(RetainedClassTableFiles, RetainedClassTableFileDetails.Count)}");
        builder.AppendLine($"- Skipped class table files: {WithSample(SkippedClassTableFiles, SkippedClassTableFileDetails.Count)}");
        builder.AppendLine($"- Canonical named tables loaded: {CanonicalNamedTablesLoaded}");
        builder.AppendLine($"- Duplicate canonical named tables: {WithSample(DuplicateCanonicalNamedTables, DuplicateNamedTables.Count)}");
        builder.AppendLine($"- Missing class table references: {WithSample(MissingClassTableReferences, MissingClassTableReferenceDetails.Count)}");
        builder.AppendLine($"- Missing modifier table references: {WithSample(MissingModifierTableReferences, MissingModifierTableReferenceDetails.Count)}");
        builder.AppendLine($"- Power fields mapped: {WithSample(PowerFieldsMappedCount, MappedPowerFields.Count)}");
        builder.AppendLine($"- Power fields mapped with fallback: {WithSample(PowerFieldsMappedWithFallbackCount, PowerFieldsMappedWithFallback.Count)}");
        builder.AppendLine($"- Power field conflicts: {WithSample(PowerFieldConflictCount, PowerFieldConflicts.Count)}");
        builder.AppendLine($"- Known charge-capacity extensions: {WithSample(KnownChargeCapacityExtensions, KnownChargeCapacityDetailEntries.Count)}");
        builder.AppendLine($"- Known deferred power fields: {WithSample(DeferredPowerFieldCount, DeferredPowerFields.Count)}");
        builder.AppendLine($"- Ignored UI/client/server power fields: {IgnoredPowerFieldCount} across {IgnoredPowerFieldOwnerKindCounts.Count} owner kinds / {IgnoredPowerFieldCategoryCounts.Count} categories / {IgnoredPowerFieldNameCounts.Count} unique fields (sampled {IgnoredPowerFields.Count})");
        builder.AppendLine($"- Unknown power fields: {WithSample(UnknownPowerFieldCount, UnknownPowerFields.Count)}");
        builder.AppendLine($"- Powerset icon audits: {WithSample(PoolPowersetIconAuditCount, PoolPowersetIconAudit.Count)}");
        builder.AppendLine($"- Powerset icon assignments available: {WithSample(PoolIconAssignmentsAvailableCount, PoolIconAssignments.Count)}");
        builder.AppendLine($"- Missing powerset icon assets: {WithSample(MissingPoolIconAssetCount, MissingPoolIconAssets.Count)}");
        builder.AppendLine($"- Sorcery Enflame pickability traces: {WithSample(SorceryEnflameTraceCount, SorceryEnflamePickabilityTrace.Count)}");
        builder.AppendLine($"- Pool requirement evaluation failures: {WithSample(PoolRequirementEvaluationFailureCount, PoolRequirementEvaluationFailures.Count)}");
        builder.AppendLine($"- PvMode inferred from target entity expressions: {WithSample(PvModeInferredFromTargetEntityCount, PvTargetGatingAudit.Count)}");
        builder.AppendLine($"- PvMode inferred from modifier tables: {PvModeInferredFromTableCount}");
        builder.AppendLine($"- Ambiguous PvX target expressions preserved as Any: {PvModeAmbiguousCount}");
        builder.AppendLine($"- PvX/target/gating audit entries: {WithSample(PvTargetAuditCount, PvTargetGatingAudit.Count)}");
        builder.AppendLine($"- PvX/target mapping mismatches: {WithSample(PvTargetMappingMismatchCount, PvTargetMappingMismatches.Count)}");
        builder.AppendLine($"- Enhancement files discovered: {EnhancementDefinitionsDiscovered}");
        builder.AppendLine($"- Enhancement sets discovered: {EnhancementSetsDiscovered}");
        builder.AppendLine($"- Classic enhancement variants discovered/logical/folded: {ClassicEnhancementSourceVariantsDiscovered}/{ClassicEnhancementLogicalRecords}/{ClassicEnhancementVariantsFolded}");
        builder.AppendLine($"- Classic enhancement editor rows expected: {ClassicEnhancementSourceVariantsDiscovered}");
        builder.AppendLine($"- Recipe source directory used: {EnhancementRecipeSourceDirectory}");
        builder.AppendLine($"- Recipe files discovered: {RecipeLevelFilesDiscovered}");
        builder.AppendLine($"- Logical enhancement recipes discovered: {FoldedEnhancementRecipeCount}");
        builder.AppendLine($"- Recipe level variants discovered: {RecipeLevelVariantsDiscovered}");
        builder.AppendLine($"- Recipes excluded by policy: {RecipeRecordsExcludedByPolicy}");
        builder.AppendLine($"- Enhancement records excluded by policy: {EnhancementRecordsExcludedByPolicy}");
        builder.AppendLine($"- Salvage definitions discovered: {SalvageDefinitionsDiscovered}");
        builder.AppendLine($"- Enhancement level variants discovered: {EnhancementLevelVariantsDiscovered}");
        builder.AppendLine($"- Structured boosts_allowed parsed successfully: {StructuredBoostsAllowedParsedCount}");
        builder.AppendLine($"- Enhancement records skipped malformed/unsupported: {EnhancementMalformedRecordsSkipped}");
        builder.AppendLine($"- Enhancement shape compatibility fallbacks: {EnhancementShapeCompatibilityFallbacks}");
        builder.AppendLine($"- Enhancement boost power links resolvable: {EnhancementBoostPowerLinksResolvedDryRun}");
        builder.AppendLine($"- Enhancement boost power links missing: {WithSample(EnhancementBoostPowerLinksMissingDryRun, EnhancementPowerLinkAudit.Count)}");
        builder.AppendLine($"- Enhancement boost links via alias/fallback: {EnhancementBoostPowerLinksAliasDryRun}/{EnhancementBoostPowerLinksFallbackDryRun}");
        builder.AppendLine($"- Set bonus power links resolvable: {EnhancementSetBonusLinksResolvedDryRun}");
        builder.AppendLine($"- Set bonus power links missing: {WithSample(EnhancementSetBonusLinksMissingDryRun, EnhancementPowerLinkAudit.Count)}");
        builder.AppendLine($"- Set bonus links via alias/fallback: {EnhancementSetBonusLinksAliasDryRun}/{EnhancementSetBonusLinksFallbackDryRun}");
        builder.AppendLine($"- Recipe reward links resolvable: {RecipeRewardLinksResolvedDryRun}");
        builder.AppendLine($"- Recipe reward links missing: {RecipeRewardLinksMissingDryRun}");
        builder.AppendLine($"- Boost powersets in scope: {BoostPowersetsInScope}");
        builder.AppendLine($"- Boost powers in scope: {BoostPowersInScope}");
        builder.AppendLine($"- Set bonus powersets in scope: {SetBonusPowersetsInScope}");
        builder.AppendLine($"- Set bonus powers in scope: {SetBonusPowersInScope}");
        builder.AppendLine($"- Boost powerset coverage source: {DescribeCoverageSource(BoostExplicitPowersetsInScope, BoostDerivedPowersetsInScope)}");
        builder.AppendLine($"- Set bonus powerset coverage source: {DescribeCoverageSource(SetBonusExplicitPowersetsInScope, SetBonusDerivedPowersetsInScope)}");
        builder.AppendLine($"- Enhancement policy files present: {EnhancementPolicyFilesPresent}");
        builder.AppendLine($"- Enhancement policy files missing: {WithSample(EnhancementPolicyFilesMissing, EnhancementMissingPolicyFiles.Count)}");
        builder.AppendLine($"- Enhancement reconciliation exact/alias/linked/fallback/ambiguous: {EnhancementExactMatchesDryRun}/{EnhancementAliasMatchesDryRun}/{EnhancementLinkedPowerMatchesDryRun}/{EnhancementFallbackMatchesDryRun}/{EnhancementAmbiguousMatchesDryRun}");
        builder.AppendLine($"- Enhancement reconciliation would update/create: {EnhancementWouldUpdateDryRun}/{EnhancementWouldCreateDryRun}");
        builder.AppendLine($"- Enhancement create investigation likely-new/boost/recipe/set/no-plausible: {EnhancementCreateLikelyNewDryRun}/{EnhancementCreateShouldMatchBoostPowerDryRun}/{EnhancementCreateShouldMatchRecipeDryRun}/{EnhancementCreateShouldMatchSetMembershipDryRun}/{EnhancementCreateNoPlausibleExistingMatchDryRun}");
        builder.AppendLine($"- Enhancement likely-new profile attuned/superior/set-backed/recipe-backed/classic/special/invention: {EnhancementCreateLikelyNewAttunedDryRun}/{EnhancementCreateLikelyNewSuperiorAttunedDryRun}/{EnhancementCreateLikelyNewSetBackedDryRun}/{EnhancementCreateLikelyNewRecipeBackedDryRun}/{EnhancementCreateLikelyNewClassicDryRun}/{EnhancementCreateLikelyNewSpecialOriginDryRun}/{EnhancementCreateLikelyNewInventionDryRun}");
        builder.AppendLine($"- Invention variants discovered crafted/attuned/superior/superior-attuned: {InventionCraftedVariantsDiscovered}/{InventionAttunedVariantsDiscovered}/{InventionSuperiorVariantsDiscovered}/{InventionSuperiorAttunedVariantsDiscovered}");
        builder.AppendLine($"- Standalone invention variants crafted/attuned/superior/superior-attuned: {InventionStandaloneCraftedVariantsDiscovered}/{InventionStandaloneAttunedVariantsDiscovered}/{InventionStandaloneSuperiorVariantsDiscovered}/{InventionStandaloneSuperiorAttunedVariantsDiscovered}");
        builder.AppendLine($"- IO set family coverage crafted-only/attuned-only/superior-only/superior-attuned-only: {IoSetFamiliesCraftedOnly}/{IoSetFamiliesAttunedOnly}/{IoSetFamiliesSuperiorOnly}/{IoSetFamiliesSuperiorAttunedOnly}");
        builder.AppendLine($"- IO set family coverage crafted+attuned/crafted+superior-attuned/attuned+superior-attuned/all-three: {IoSetFamiliesCraftedAndAttuned}/{IoSetFamiliesCraftedAndSuperiorAttuned}/{IoSetFamiliesAttunedAndSuperiorAttuned}/{IoSetFamiliesCraftedAttunedSuperiorAttuned}");
        builder.AppendLine($"- Enhancement set reconciliation exact/alias/fallback/ambiguous: {EnhancementSetExactMatchesDryRun}/{EnhancementSetAliasMatchesDryRun}/{EnhancementSetFallbackMatchesDryRun}/{EnhancementSetAmbiguousMatchesDryRun}");
        builder.AppendLine($"- Enhancement set reconciliation would update/create: {EnhancementSetWouldUpdateDryRun}/{EnhancementSetWouldCreateDryRun}");
        builder.AppendLine($"- Recipe reconciliation exact/alias/fallback/ambiguous: {RecipeExactMatchesDryRun}/{RecipeAliasMatchesDryRun}/{RecipeFallbackMatchesDryRun}/{RecipeAmbiguousMatchesDryRun}");
        builder.AppendLine($"- Recipe reconciliation would update/create: {RecipeWouldUpdateDryRun}/{RecipeWouldCreateDryRun}");
        builder.AppendLine($"- Recipe create investigation likely-new/reward/canonical/no-plausible: {RecipeCreateLikelyNewDryRun}/{RecipeCreateShouldMatchRewardIdentityDryRun}/{RecipeCreateShouldMatchCanonicalStorageIdentityDryRun}/{RecipeCreateNoPlausibleExistingMatchDryRun}");
        builder.AppendLine($"- Salvage reconciliation exact/alias/fallback/ambiguous: {SalvageExactMatchesDryRun}/{SalvageAliasMatchesDryRun}/{SalvageFallbackMatchesDryRun}/{SalvageAmbiguousMatchesDryRun}");
        builder.AppendLine($"- Salvage reconciliation would update/create: {SalvageWouldUpdateDryRun}/{SalvageWouldCreateDryRun}");
        builder.AppendLine($"- Epic powerset files discovered: {EpicPowersetFilesDiscovered}");
        builder.AppendLine($"- Epic powerset files in scope: {EpicPowersetFilesInScope}");
        builder.AppendLine($"- Epic power files discovered: {EpicPowerFilesDiscovered}");
        builder.AppendLine($"- Epic power files in scope: {EpicPowerFilesInScope}");
        builder.AppendLine($"- Epic powers loaded: {EpicPowersLoaded}");
        builder.AppendLine($"- Pet powerset files discovered: {FormatCounts(PetPowersetFilesDiscovered)}");
        builder.AppendLine($"- Pet powerset files in scope: {FormatCounts(PetPowersetFilesInScope)}");
        builder.AppendLine($"- Pet power files discovered: {FormatCounts(PetPowerFilesDiscovered)}");
        builder.AppendLine($"- Pet power files in scope: {FormatCounts(PetPowerFilesInScope)}");
        builder.AppendLine($"- Pet powers loaded: {FormatCounts(PetPowersLoaded)}");
        builder.AppendLine($"- Pet manifest roots present: {PetManifestRootsPresent}");
        builder.AppendLine($"- Pet manifest roots missing: {PetManifestRootsMissing}");
        builder.AppendLine($"- Pet manifest powersets: {PetManifestPowersets}");
        builder.AppendLine($"- Pet manifest expected powers: {PetManifestExpectedPowers}");
        builder.AppendLine($"- Pet manifest power files: {PetManifestPowerFiles}");
        builder.AppendLine($"- Pet manifest missing power files: {PetManifestMissingPowerFiles}");
        AppendSection(builder, "Pet Import Scope", PetImportScopeDetails);
        AppendSection(builder, "Pet Import Manifest", PetImportManifestDetails);
        AppendSection(builder, "Power Target Requires Not Imported", PowerTargetRequiresNotImported);
        AppendSection(builder, "Power Target Requires Manual Review", PowerTargetRequiresManualReview);
        AppendSection(builder, "Power Field Mapping Coverage", MappedPowerFields);
        AppendSection(builder, "Mapped With Fallback", PowerFieldsMappedWithFallback);
        AppendSection(builder, "Field Conflicts", PowerFieldConflicts);
        AppendSection(builder, "Known Charge-Capacity Extensions", KnownChargeCapacityDetailEntries);
        AppendSection(builder, "Known Deferred Power Fields", DeferredPowerFields);
        AppendCountSection(builder, "Ignored Power Field Owner Kinds", IgnoredPowerFieldOwnerKindCounts);
        AppendCountSection(builder, "Ignored Power Field Categories", IgnoredPowerFieldCategoryCounts);
        AppendCountSection(builder, "Ignored Power Fields By Name", IgnoredPowerFieldNameCounts);
        AppendSection(builder, "Ignored UI/Client/Server Power Field Samples", IgnoredPowerFields);
        AppendSection(builder, "Unknown Power Fields", UnknownPowerFields);
        AppendSection(builder, "Powerset Icon Audit", PoolPowersetIconAudit);
        AppendSection(builder, "Powerset Icon Assignments", PoolIconAssignments);
        AppendSection(builder, "Missing Powerset Icon Assets", MissingPoolIconAssets);
        AppendSection(builder, "Sorcery Enflame Pickability Trace", SorceryEnflamePickabilityTrace);
        AppendSection(builder, "Pool Requirement Evaluation Failures", PoolRequirementEvaluationFailures);
        AppendSection(builder, "PvX Target And Gating Audit", PvTargetGatingAudit);
        AppendSection(builder, "PvX Target Mapping Mismatches", PvTargetMappingMismatches);
        AppendSection(builder, "Chance Mod Mappings", ChanceModMappings);
        AppendSection(builder, "Enhancement Import Scan", EnhancementImportScanDetails);
        AppendSection(builder, "Classic Enhancement Folding", ClassicEnhancementFoldingDetails);
        AppendSection(builder, "Enhancement Source Shape Validation", EnhancementSourceShapeValidation);
        AppendSection(builder, "Enhancement Missing Policy Files", EnhancementMissingPolicyFiles);
        AppendSection(builder, "Boosts And Set Bonus Scope Coverage", ScopedBoostSetBonusCoverage);
        AppendSection(builder, "Enhancement Power Link Audit", EnhancementPowerLinkAudit);
        AppendSection(builder, "Enhancement Reconciliation Audit", EnhancementReconciliationAudit);
        AppendSection(builder, "Enhancement Reconciliation Conflicts", EnhancementReconciliationConflicts);
        AppendSection(builder, "Enhancement Create Validation", EnhancementCreateValidationSummary);
        AppendSection(builder, "Invention Variant Audit", InventionVariantAudit);
        AppendSection(builder, "Enhancement Identity Investigation", EnhancementIdentityInvestigation);
        AppendSection(builder, "Enhancement Identity Likely New Samples", EnhancementIdentityLikelyNewSamples);
        AppendSection(builder, "Recipe Identity Investigation", RecipeIdentityInvestigation);
        AppendSection(builder, "Recipe Identity Likely New Samples", RecipeIdentityLikelyNewSamples);
        return builder.ToString();
    }

    public string ToPreviewMarkdown(string exportRoot, int classAttributeCount, int actorCount)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Omni Import Preview");
        builder.AppendLine();
        builder.AppendLine($"- Export root: {exportRoot}");
        builder.AppendLine($"- Scoped powers: {PowersInScope:n0} of {PowersConsidered:n0}");
        builder.AppendLine($"- Retained/skipped archetypes: {RetainedArchetypesRead:n0}/{SkippedArchetypesRead:n0}");
        builder.AppendLine($"- Class attributes loaded: {classAttributeCount:n0}");
        builder.AppendLine($"- Retained/skipped class tables: {RetainedClassTableFiles:n0}/{SkippedClassTableFiles:n0}");
        builder.AppendLine($"- Actors classified: {actorCount:n0}");
        builder.AppendLine($"- Effects visited: {EffectsVisited:n0}");
        builder.AppendLine($"- Requirements visited: {PowerRequirements:n0}");
        builder.AppendLine($"- Build/runtime/report-only expressions: {BuildEvaluatedExpressionCount:n0}/{RuntimeTargetExpressionCount:n0}/{ReportOnlyExpressionCount:n0}");
        builder.AppendLine($"- Global/power-local chance mods mapped: {GlobalChanceModsMappedCount:n0}/{PowerLocalChanceModsMappedCount:n0}");
        builder.AppendLine($"- Enhancement files/sets discovered: {EnhancementDefinitionsDiscovered:n0}/{EnhancementSetsDiscovered:n0}");
        builder.AppendLine($"- Classic enhancement variants/logical/folded: {ClassicEnhancementSourceVariantsDiscovered:n0}/{ClassicEnhancementLogicalRecords:n0}/{ClassicEnhancementVariantsFolded:n0}");
        builder.AppendLine($"- Classic enhancement editor rows expected: {ClassicEnhancementSourceVariantsDiscovered:n0}");
        builder.AppendLine($"- Recipe source/files/logical/variants: {EnhancementRecipeSourceDirectory}/{RecipeLevelFilesDiscovered:n0}/{FoldedEnhancementRecipeCount:n0}/{RecipeLevelVariantsDiscovered:n0}");
        builder.AppendLine($"- Recipes excluded by policy: {RecipeRecordsExcludedByPolicy:n0}");
        builder.AppendLine($"- Salvage definitions discovered: {SalvageDefinitionsDiscovered:n0}");
        builder.AppendLine($"- Structured boosts_allowed parsed: {StructuredBoostsAllowedParsedCount:n0}");
        builder.AppendLine($"- Shape fallbacks / malformed skips: {EnhancementShapeCompatibilityFallbacks:n0}/{EnhancementMalformedRecordsSkipped:n0}");
        builder.AppendLine($"- Enhancement boost links resolved/missing: {EnhancementBoostPowerLinksResolvedDryRun:n0}/{EnhancementBoostPowerLinksMissingDryRun:n0}");
        builder.AppendLine($"- Set bonus links resolved/missing: {EnhancementSetBonusLinksResolvedDryRun:n0}/{EnhancementSetBonusLinksMissingDryRun:n0}");
        builder.AppendLine($"- Recipe reward links resolved/missing: {RecipeRewardLinksResolvedDryRun:n0}/{RecipeRewardLinksMissingDryRun:n0}");
        builder.AppendLine($"- Enhancement reconciliation updates/creates/conflicts: {EnhancementWouldUpdateDryRun:n0}/{EnhancementWouldCreateDryRun:n0}/{EnhancementAmbiguousMatchesDryRun:n0}");
        builder.AppendLine($"- Enhancement create investigation likely-new/boost/recipe/set/no-plausible: {EnhancementCreateLikelyNewDryRun:n0}/{EnhancementCreateShouldMatchBoostPowerDryRun:n0}/{EnhancementCreateShouldMatchRecipeDryRun:n0}/{EnhancementCreateShouldMatchSetMembershipDryRun:n0}/{EnhancementCreateNoPlausibleExistingMatchDryRun:n0}");
        builder.AppendLine($"- Enhancement likely-new profile attuned/set-backed/classic/special/invention: {EnhancementCreateLikelyNewAttunedDryRun:n0}/{EnhancementCreateLikelyNewSetBackedDryRun:n0}/{EnhancementCreateLikelyNewClassicDryRun:n0}/{EnhancementCreateLikelyNewSpecialOriginDryRun:n0}/{EnhancementCreateLikelyNewInventionDryRun:n0}");
        builder.AppendLine($"- Invention variants crafted/attuned/superior-attuned: {InventionCraftedVariantsDiscovered:n0}/{InventionAttunedVariantsDiscovered:n0}/{InventionSuperiorAttunedVariantsDiscovered:n0}");
        builder.AppendLine($"- IO set families crafted-only/attuned-only/superior-attuned-only/crafted+attuned/crafted+superior-attuned: {IoSetFamiliesCraftedOnly:n0}/{IoSetFamiliesAttunedOnly:n0}/{IoSetFamiliesSuperiorAttunedOnly:n0}/{IoSetFamiliesCraftedAndAttuned:n0}/{IoSetFamiliesCraftedAndSuperiorAttuned:n0}");
        builder.AppendLine($"- Enhancement set reconciliation updates/creates/conflicts: {EnhancementSetWouldUpdateDryRun:n0}/{EnhancementSetWouldCreateDryRun:n0}/{EnhancementSetAmbiguousMatchesDryRun:n0}");
        builder.AppendLine($"- Recipe reconciliation updates/creates/conflicts: {RecipeWouldUpdateDryRun:n0}/{RecipeWouldCreateDryRun:n0}/{RecipeAmbiguousMatchesDryRun:n0}");
        builder.AppendLine($"- Recipe create investigation likely-new/reward/canonical/no-plausible: {RecipeCreateLikelyNewDryRun:n0}/{RecipeCreateShouldMatchRewardIdentityDryRun:n0}/{RecipeCreateShouldMatchCanonicalStorageIdentityDryRun:n0}/{RecipeCreateNoPlausibleExistingMatchDryRun:n0}");
        builder.AppendLine($"- Boosts powersets/powers in scope: {BoostPowersetsInScope:n0}/{BoostPowersInScope:n0} ({DescribeCoverageSource(BoostExplicitPowersetsInScope, BoostDerivedPowersetsInScope)})");
        builder.AppendLine($"- Set_Bonus powersets/powers in scope: {SetBonusPowersetsInScope:n0}/{SetBonusPowersInScope:n0} ({DescribeCoverageSource(SetBonusExplicitPowersetsInScope, SetBonusDerivedPowersetsInScope)})");
        builder.AppendLine($"- Enhancement policy files present/missing: {EnhancementPolicyFilesPresent:n0}/{EnhancementPolicyFilesMissing:n0}");

        AppendPreviewWarnings(builder, new[]
        {
            FormatWarning("Unsupported power requirements", UnsupportedPowerRequirementCount),
            FormatWarning("Unknown effect mappings", UnknownEffectMappings.Count),
            FormatWarning("Unknown attribute mappings", UnknownAttribMappings.Count),
            FormatWarning("Unresolved redirects", UnresolvedRedirects.Count),
            FormatWarning("Unresolved entities", UnresolvedEntities.Count),
            FormatWarning("Unsupported build expressions", UnsupportedBuildExpressionCount),
            FormatWarning("Unknown expression tokens", UnknownExpressionTokenCount),
            FormatWarning("Enhancement power links missing", EnhancementBoostPowerLinksMissingDryRun + EnhancementSetBonusLinksMissingDryRun),
            FormatWarning("Enhancement reconciliation conflicts", EnhancementAmbiguousMatchesDryRun + EnhancementSetAmbiguousMatchesDryRun + RecipeAmbiguousMatchesDryRun + SalvageAmbiguousMatchesDryRun),
            FormatWarning("Enhancement policy files missing", EnhancementPolicyFilesMissing)
        });

        AppendPreviewSection(builder, "Enhancement Link Audit", EnhancementPowerLinkAudit, 8);
        AppendPreviewSection(builder, "Classic Enhancement Folding", ClassicEnhancementFoldingDetails, 8);
        AppendPreviewSection(builder, "Enhancement Source Shape Validation", EnhancementSourceShapeValidation, 8);
        AppendPreviewSection(builder, "Enhancement Reconciliation Audit", EnhancementReconciliationAudit, 8);
        AppendPreviewSection(builder, "Enhancement Reconciliation Conflicts", EnhancementReconciliationConflicts, 8);
        AppendPreviewSection(builder, "Enhancement Create Validation", EnhancementCreateValidationSummary, 8);
        AppendPreviewSection(builder, "Invention Variant Audit", InventionVariantAudit, 8);
        AppendPreviewSection(builder, "Enhancement Identity Investigation", EnhancementIdentityInvestigation, 8);
        AppendPreviewSection(builder, "Enhancement Identity Likely New Samples", EnhancementIdentityLikelyNewSamples, 6);
        AppendPreviewSection(builder, "Recipe Identity Investigation", RecipeIdentityInvestigation, 8);
        AppendPreviewSection(builder, "Recipe Identity Likely New Samples", RecipeIdentityLikelyNewSamples, 6);
        AppendPreviewSection(builder, "Boosts / Set_Bonus Coverage", ScopedBoostSetBonusCoverage, 8);
        AppendPreviewSection(builder, "Chance Mod Mappings", ChanceModMappings, 6);
        AppendPreviewSection(builder, "Missing Enhancement Policy Files", EnhancementMissingPolicyFiles, 8);
        AppendPreviewSection(builder, "Unknown Effect Mappings", UnknownEffectMappings, 6);
        AppendPreviewSection(builder, "Unknown Attribute Mappings", UnknownAttribMappings, 6);
        AppendPreviewSection(builder, "Representative Samples", Samples, 6);

        return builder.ToString();
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    private static void AppendSection(StringBuilder builder, string title, IReadOnlyCollection<string> values)
    {
        if (values.Count == 0)
        {
            return;
        }

        builder.AppendLine();
        builder.AppendLine($"## {title}");
        builder.AppendLine();
        foreach (var value in values)
        {
            builder.AppendLine($"- {value}");
        }
    }

    private static void AppendCountSection(StringBuilder builder, string title, IReadOnlyDictionary<string, int> values)
    {
        if (values.Count == 0)
        {
            return;
        }

        builder.AppendLine();
        builder.AppendLine($"## {title}");
        builder.AppendLine();
        foreach (var pair in values.OrderByDescending(pair => pair.Value).ThenBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"- {pair.Key}: {pair.Value}");
        }
    }

    private static string? FormatWarning(string label, int count)
    {
        return count > 0 ? $"{label}: {count:n0}" : null;
    }

    private static void AppendPreviewWarnings(StringBuilder builder, IEnumerable<string?> warnings)
    {
        var materialized = warnings
            .Where(static warning => !string.IsNullOrWhiteSpace(warning))
            .Cast<string>()
            .ToList();
        if (materialized.Count == 0)
        {
            return;
        }

        builder.AppendLine();
        builder.AppendLine("## Attention");
        foreach (var warning in materialized)
        {
            builder.AppendLine($"- {warning}");
        }
    }

    private static void AppendPreviewSection(StringBuilder builder, string title, IReadOnlyCollection<string> values, int maxItems)
    {
        if (values.Count == 0)
        {
            return;
        }

        builder.AppendLine();
        builder.AppendLine($"## {title}");
        foreach (var value in values.Take(maxItems))
        {
            builder.AppendLine($"- {value}");
        }

        if (values.Count > maxItems)
        {
            builder.AppendLine($"- ... {values.Count - maxItems:n0} more");
        }
    }

    private static string DescribeCoverageSource(int explicitCount, int derivedCount)
    {
        return (explicitCount, derivedCount) switch
        {
            (> 0, > 0) => $"mixed: explicit={explicitCount:n0}, derived={derivedCount:n0}",
            (> 0, 0) => $"explicit only: {explicitCount:n0}",
            (0, > 0) => $"derived only: {derivedCount:n0}",
            _ => "none"
        };
    }

    private static string FormatCounts(IReadOnlyDictionary<string, int> counts)
    {
        return counts.Count == 0
            ? "0"
            : string.Join(", ", counts.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                .Select(pair => $"{pair.Key}={pair.Value}"));
    }
}

using System.Text;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Omni;

public sealed class OmniApplyResult
{
    public int ClassAttributesStored { get; set; }
    public int ArchetypesRead { get; set; }
    public int PlayableArchetypesRead { get; set; }
    public int RetainedArchetypesRead { get; set; }
    public int SkippedArchetypesRead { get; set; }
    public int ArchetypeSummariesUpdated { get; set; }
    public int PowersetsMatched { get; set; }
    public int PowersetsCreated { get; set; }
    public int PowersetsUpdated { get; set; }
    public int PowersMatched { get; set; }
    public int PowersCreated { get; set; }
    public int PowersUpdated { get; set; }
    public int PowersSkippedNeverAutoUpdate { get; set; }
    public int RequirementsUpdated { get; set; }
    public int RequirementsSkippedNeverAutoUpdateRequirements { get; set; }
    public int RequirementsSkippedUnsupported { get; set; }
    public int EffectsReplaced { get; set; }
    public int RedirectEffectsAdded { get; set; }
    public int PowersMissingFromMids { get; set; }
    public int UnknownEffectMappings { get; set; }
    public int UnknownAttribMappings { get; set; }
    public int KnownHiddenStatefulEffectMappings { get; set; }
    public int KnownUnsupportedEffectMappings { get; set; }
    public int SupportPowersRemoved { get; set; }
    public int SupportPowersetsRemoved { get; set; }
    public int HiddenSupportPowers { get; set; }
    public int VisibleGatedPowers { get; set; }
    public int SupportPowerLinksCreated { get; set; }
    public int SupportPowerLinksUnresolved { get; set; }
    public int RedirectExecutionVariants { get; set; }
    public int ClickBuffClassificationChanges { get; set; }
    public int ManualClassificationReviews { get; set; }
    public int SupportHeavyPowersRecreated { get; set; }
    public int ClassInherentsImported { get; set; }
    public int FormGatedPowersHidden { get; set; }
    public int ModeFlagsAssigned { get; set; }
    public int UnknownModesPreserved { get; set; }
    public int PlannerInherentsPreserved { get; set; }
    public int SyntheticPlannerInherentsCreated { get; set; }
    public int ModeCatalogEntriesLoaded { get; set; }
    public int PlannerModesDiscovered { get; set; }
    public int PlannerModePayloadsAdded { get; set; }
    public int SpecialCaseCompatibilityBridges { get; set; }
    public int SnipeEngagedAliases { get; set; }
    public int GcmTagsRead { get; set; }
    public int GcmTagsAlreadyKnown { get; set; }
    public int GcmTagsAdded { get; set; }
    public int GcmDuplicateTags { get; set; }
    public int GcmBlankTags { get; set; }
    public int EffectGroupTagsPreserved { get; set; }
    public int TemplateFilterTagsMapped { get; set; }
    public int GlobalChanceModsMapped { get; set; }
    public int PowerLocalChanceModsMapped { get; set; }
    public int UnsupportedEffectFilters { get; set; }
    public int ClassTableFilesRead { get; set; }
    public int RetainedClassTableFiles { get; set; }
    public int SkippedClassTableFiles { get; set; }
    public int CanonicalNamedTablesStored { get; set; }
    public int MissingModifierTableReferences { get; set; }
    public int AttackVectorsMapped { get; set; }
    public int ZeroValueTagCarrierEffectsSuppressed { get; set; }
    public int MissingScopedPowersetsRepaired { get; set; }
    public int ScopedPowerSetIdentityRepairs { get; set; }
    public int AliasedPowersetIdentityRepairs { get; set; }
    public int AliasedPowersetDuplicateRemovals { get; set; }
    public int AliasedPowersetIdentityCollisions { get; set; }
    public int AliasedPowerIdentityRepairs { get; set; }
    public int AliasedPowerDuplicateRemovals { get; set; }
    public int AliasedPowerIdentityCollisions { get; set; }
    public int MalformedPowerNameRepairs { get; set; }
    public int MalformedPowerDuplicateRemovals { get; set; }
    public int MalformedPowerNameCollisions { get; set; }
    public int PowersetsReorderedByPowerLevel { get; set; }
    public int ExcludedArchetypesRemoved { get; set; }
    public int ExcludedClassPowersetsRemoved { get; set; }
    public int ExcludedClassPowersRemoved { get; set; }
    public int ExcludedClassEntitiesRemoved { get; set; }
    public int ExcludedPowersetsRemoved { get; set; }
    public int ExcludedPowersRemoved { get; set; }
    public int OrphanPowersBeforeImport { get; set; }
    public int OrphanPowersAfterImport { get; set; }
    public int NewOrphanPowersIntroduced { get; set; }
    public int OrphanedScopedOmniPowers { get; set; }
    public int ExactScopedOmniPowersRetained { get; set; }
    public int AcceptedCanonicalScopedPowerReplacements { get; set; }
    public int ScopedDisplayFallbackRejected { get; set; }
    public int ExcludedScopedOmniPowers { get; set; }
    public int ManifestOwnedScopedOmniPowers { get; set; }
    public int RetainedTemporaryPowersetsTracked { get; set; }
    public int RetainedTemporaryPowersTracked { get; set; }
    public int RetainedTemporaryHiddenPowers { get; set; }
    public int RetainedTemporaryIntegrityFailures { get; set; }
    public int PowersetIdentityChanges { get; set; }
    public int DuplicateCompositePowerIdentities { get; set; }
    public int EpicPowersetsCreated { get; set; }
    public int EpicPowersetsMatched { get; set; }
    public int EpicPowersetsUpdated { get; set; }
    public int EpicPowersCreated { get; set; }
    public int EpicPowersMatched { get; set; }
    public int EpicPowersUpdated { get; set; }
    public int EpicPowersSkipped { get; set; }
    public int EpicPowersLinkedAfterMatchIds { get; set; }
    public int EpicPowersetsWithZeroLinkedPowers { get; set; }
    public int EpicPowersMissingAfterImport { get; set; }
    public int EpicPowersWithMissingPowerset { get; set; }
    public int EpicPowersWithInvalidPowersetId { get; set; }
    public int EpicPowersNotInPowersetArray { get; set; }
    public int EpicPowersHiddenInDbEditor { get; set; }
    public int PetPowersCreated { get; set; }
    public int PetPowersMatched { get; set; }
    public int PetPowersUpdated { get; set; }
    public int PetPowersSkipped { get; set; }
    public int PetPowersRecreated { get; set; }
    public int PetPowersRepairedBeforeLinking { get; set; }
    public int PetPowersLinkedAfterMatchIds { get; set; }
    public int PetPowersMissingAfterImport { get; set; }
    public int PetPowersWithMissingPowerset { get; set; }
    public int PetPowersWithInvalidPowersetId { get; set; }
    public int PetPowersNotInPowersetArray { get; set; }
    public int PetManifestRootsPresent { get; set; }
    public int PetManifestRootsMissing { get; set; }
    public int PetManifestPowersets { get; set; }
    public int PetManifestExpectedPowers { get; set; }
    public int PetManifestPowerFiles { get; set; }
    public int PetManifestMissingPowerFiles { get; set; }
    public int PetSourcePowersetsMatched { get; set; }
    public int PetSourcePowersetsCreated { get; set; }
    public int PetSourcePowersetsUpdated { get; set; }
    public int PetSourcePowersMatched { get; set; }
    public int PetSourcePowersCreated { get; set; }
    public int PetSourcePowersUpdated { get; set; }
    public int PetSourceIntegrityFailures { get; set; }
    public int PetSourcePowersetsWithZeroLinkedPowers { get; set; }
    public int PowerFieldsMapped { get; set; }
    public int PowerFieldsMappedWithFallback { get; set; }
    public int PowerFieldConflicts { get; set; }
    public int KnownChargeCapacityExtensions { get; set; }
    public int DeferredPowerFields { get; set; }
    public int IgnoredPowerFields { get; set; }
    public int UnknownPowerFields { get; set; }
    public int PoolPowersetIconsPreserved { get; set; }
    public int PoolPowersetIconsAssigned { get; set; }
    public int PoolPowersetIconsMissingAssets { get; set; }
    public int PoolIconIntegrityIssues { get; set; }
    public int PoolPowerLinkIntegrityIssues { get; set; }
    public int SorceryEnflameTraceCount { get; set; }
    public int PoolRequirementEvaluationFailures { get; set; }
    public int PseudoPetEntitiesCreated { get; set; }
    public int PseudoPetEntitiesUpdated { get; set; }
    public int RealPetEntitiesCreated { get; set; }
    public int RealPetEntitiesUpdated { get; set; }
    public int PseudoPetAbsorptionFlagsEnabled { get; set; }
    public int PseudoPetAbsorptionAuditFailures { get; set; }
    public int PseudoPetAbsorptionAuditSkipped { get; set; }
    public int PvModeInferredFromTargetEntity { get; set; }
    public int PvModeInferredFromTable { get; set; }
    public int PvModeAmbiguous { get; set; }
    public int PvTargetAuditEntries { get; set; }
    public int PvTargetMappingMismatches { get; set; }
    public int SalvageMatched { get; set; }
    public int SalvageCreated { get; set; }
    public int SalvageUpdated { get; set; }
    public int RecipesMatched { get; set; }
    public int RecipesCreated { get; set; }
    public int RecipesUpdated { get; set; }
    public int EnhancementSetsMatched { get; set; }
    public int EnhancementSetsCreated { get; set; }
    public int EnhancementSetsUpdated { get; set; }
    public int EnhancementsMatched { get; set; }
    public int EnhancementsCreated { get; set; }
    public int EnhancementsUpdated { get; set; }
    public int EnhancementFilesProcessed { get; set; }
    public int EnhancementRecordsExcludedByPolicy { get; set; }
    public int ClassicEnhancementSourceVariantsDiscovered { get; set; }
    public int ClassicEnhancementLogicalRecords { get; set; }
    public int ClassicEnhancementVariantsFolded { get; set; }
    public int ClassicEnhancementEditorRowsExpected { get; set; }
    public int ClassicEnhancementMetadataWarnings { get; set; }
    public int RecipeFilesProcessed { get; set; }
    public int LogicalRecipesProcessed { get; set; }
    public int RecipeLevelVariantsProcessed { get; set; }
    public int RecipeRecordsExcludedByPolicy { get; set; }
    public int EnhancementMalformedRecordsSkipped { get; set; }
    public int EnhancementShapeCompatibilityFallbacks { get; set; }
    public int StructuredBoostsAllowedParsedCount { get; set; }
    public int EnhancementClassIdsDerivedFromEffects { get; set; }
    public int EnhancementClassIdsFallbackUsed { get; set; }
    public int EnhancementClassIdsCategoryOnly { get; set; }
    public int EnhancementClassIdWrapperMismatches { get; set; }
    public int EnhancementClassIdsUnresolved { get; set; }
    public int VectorDefenseTemplateOverrides { get; set; }
    public int VectorResistanceTemplateOverrides { get; set; }
    public int BoostHelperCarriersResolvedFromLinkedBonus { get; set; }
    public int BoostHelperCarriersResolvedLocally { get; set; }
    public int BoostHelperCarriersUnresolved { get; set; }
    public int Boosts20DamageMappingsRemaining { get; set; }
    public int EnhancementBoostPowerLinksResolved { get; set; }
    public int EnhancementBoostPowerLinksMissing { get; set; }
    public int EnhancementBoostPowerLinksAlias { get; set; }
    public int EnhancementBoostPowerLinksFallback { get; set; }
    public int EnhancementSetBonusLinksResolved { get; set; }
    public int EnhancementSetBonusLinksMissing { get; set; }
    public int EnhancementSetBonusLinksAlias { get; set; }
    public int EnhancementSetBonusLinksFallback { get; set; }
    public int RecipeRewardLinksResolved { get; set; }
    public int RecipeRewardLinksMissing { get; set; }
    public int EnhancementIconsPreserved { get; set; }
    public int EnhancementIconsAssigned { get; set; }
    public int EnhancementIconsMissing { get; set; }
    public int EnhancementExactMatches { get; set; }
    public int EnhancementAliasMatches { get; set; }
    public int EnhancementLinkedPowerMatches { get; set; }
    public int EnhancementFallbackMatches { get; set; }
    public int EnhancementAmbiguousMatches { get; set; }
    public int EnhancementSetsExactMatches { get; set; }
    public int EnhancementSetsAliasMatches { get; set; }
    public int EnhancementSetsFallbackMatches { get; set; }
    public int EnhancementSetsAmbiguousMatches { get; set; }
    public int RecipeExactMatches { get; set; }
    public int RecipeAliasMatches { get; set; }
    public int RecipeFallbackMatches { get; set; }
    public int RecipeAmbiguousMatches { get; set; }
    public int SalvageExactMatches { get; set; }
    public int SalvageAliasMatches { get; set; }
    public int SalvageFallbackMatches { get; set; }
    public int SalvageAmbiguousMatches { get; set; }
    public int BoostPowersetsInScope { get; set; }
    public int SetBonusPowersetsInScope { get; set; }
    public int BoostExplicitPowersetsInScope { get; set; }
    public int BoostDerivedPowersetsInScope { get; set; }
    public int SetBonusExplicitPowersetsInScope { get; set; }
    public int SetBonusDerivedPowersetsInScope { get; set; }
    public int BoostPowersInScope { get; set; }
    public int SetBonusPowersInScope { get; set; }
    public int BoostPowersMatched { get; set; }
    public int BoostPowersCreated { get; set; }
    public int BoostPowersUpdated { get; set; }
    public int SetBonusPowersMatched { get; set; }
    public int SetBonusPowersCreated { get; set; }
    public int SetBonusPowersUpdated { get; set; }
    public int MissingBoostPowersAfterImport { get; set; }
    public int MissingSetBonusPowersAfterImport { get; set; }
    public int StrictSetBonusPowersPurged { get; set; }
    public int StrictSetBonusPowersetsPurged { get; set; }
    public int StrictSetBonusGlobalBonusAfterImport { get; set; }
    public int StrictSetBonusSetBonusAfterImport { get; set; }
    public int StrictSetBonusPvpSetBonusAfterImport { get; set; }
    public int StrictSetBonusCrossSiblingMismatchesAfterImport { get; set; }
    public int StrictSetBonusDbOnlyExtrasAfterImport { get; set; }
    public int StrictSetBonusMissingFullNamesAfterImport { get; set; }
    public int ScopedPowerEnhancementLegalityRebuilt { get; set; }
    public int ScopedPowerEnhancementLegalityChanged { get; set; }
    public int ScopedPowerEnhancementLegalityPreserved { get; set; }
    public int ScopedPowerEnhancementLegalityEmptyAfterRebuild { get; set; }
    public int ScopedPowerEnhancementLegalityUnresolvedAfterRebuild { get; set; }
    public int ScopedPowerEnhancementLegalityUnresolvedLabelCount { get; set; }
    public int BoostPowerLegalityRepairInspected { get; set; }
    public int BoostPowerLegalityRepairRebuilt { get; set; }
    public int BoostPowerLegalityRepairPreserved { get; set; }
    public int BoostPowerLegalityRepairUnresolved { get; set; }
    public int SetBonusPowerLegalityRepairInspected { get; set; }
    public int SetBonusPowerLegalityRepairCleared { get; set; }
    public int SetBonusPowerLegalityRepairAlreadyEmpty { get; set; }
    public int BoostSetBonusPowerLegalityRepairChanged { get; set; }
    public int BoostPowerAliasRepairs { get; set; }
    public int SetBonusPowerAliasRepairs { get; set; }
    public int EnhancementAliasRepairs { get; set; }
    public int EnhancementSetAliasRepairs { get; set; }
    public int RecipeAliasRepairs { get; set; }
    public int SalvageAliasRepairs { get; set; }

    public Dictionary<string, int> PetPowersCreatedByRoot { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> PetPowersMatchedByRoot { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> PetPowersUpdatedByRoot { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> PetPowersSkippedByRoot { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> PetPowersRecreatedByRoot { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> PetPowersRepairedByRoot { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> PetPowersLinkedByRoot { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> IgnoredPowerFieldOwnerKindCounts { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> IgnoredPowerFieldCategoryCounts { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> IgnoredPowerFieldNameCounts { get; } = new(StringComparer.OrdinalIgnoreCase);

    public List<string> UpdatedPowers { get; } = [];
    public List<string> CreatedPowersets { get; } = [];
    public List<string> UpdatedPowersets { get; } = [];
    public List<string> CreatedPowers { get; } = [];
    public List<string> SkippedPowers { get; } = [];
    public List<string> MissingPowers { get; } = [];
    public List<string> RequirementSkips { get; } = [];
    public List<string> UnknownEffectMappingDetails { get; } = [];
    public List<string> UnknownAttribMappingDetails { get; } = [];
    public List<string> KnownHiddenStatefulEffectMappingDetails { get; } = [];
    public List<string> KnownUnsupportedEffectMappingDetails { get; } = [];
    public List<string> HiddenSupportPowerDetails { get; } = [];
    public List<string> VisibleGatedPowerDetails { get; } = [];
    public List<string> SupportPowerLinks { get; } = [];
    public List<string> SupportPowerLinkSkips { get; } = [];
    public List<string> RedirectExecutionVariantDetails { get; } = [];
    public List<string> ClickBuffClassificationChangeDetails { get; } = [];
    public List<string> ManualClassificationReviewDetails { get; } = [];
    public List<string> ClassInherentDetails { get; } = [];
    public List<string> FormGatedPowerDetails { get; } = [];
    public List<string> ModeFlagDetails { get; } = [];
    public List<string> UnknownModeDetails { get; } = [];
    public List<string> PlannerInherentDetails { get; } = [];
    public List<string> SyntheticPlannerInherentDetails { get; } = [];
    public List<string> PlannerModeDetails { get; } = [];
    public List<string> PlannerModePayloadDetails { get; } = [];
    public List<string> SpecialCaseCompatibilityBridgeDetails { get; } = [];
    public List<string> SnipeEngagedAliasDetails { get; } = [];
    public List<string> GcmTagsAddedDetails { get; } = [];
    public List<string> ScopedPowerEnhancementLegalityDetails { get; } = [];
    public List<string> ScopedPowerEnhancementLegalityUnknownLabelDetails { get; } = [];
    public List<string> BoostSetBonusPowerLegalityRepairDetails { get; } = [];
    public List<string> ClassicEnhancementPresentationDetails { get; } = [];
    public List<string> GcmTagsAlreadyKnownDetails { get; } = [];
    public List<string> GcmDuplicateTagDetails { get; } = [];
    public List<string> EffectGroupTagDetails { get; } = [];
    public List<string> TemplateFilterTagDetails { get; } = [];
    public List<string> ChanceModMappingDetails { get; } = [];
    public List<string> UnsupportedEffectFilterDetails { get; } = [];
    public List<string> ClassTableFileDetails { get; } = [];
    public List<string> MissingModifierTableReferenceDetails { get; } = [];
    public List<string> AttackVectorDetails { get; } = [];
    public List<string> ZeroValueTagCarrierDetails { get; } = [];
    public List<string> MissingScopedPowersetRepairDetails { get; } = [];
    public List<string> ScopedPowerSetIdentityRepairDetails { get; } = [];
    public List<string> AliasedPowersetIdentityRepairDetails { get; } = [];
    public List<string> AliasedPowersetDuplicateRemovalDetails { get; } = [];
    public List<string> AliasedPowersetIdentityCollisionDetails { get; } = [];
    public List<string> AliasedPowerIdentityRepairDetails { get; } = [];
    public List<string> AliasedPowerDuplicateRemovalDetails { get; } = [];
    public List<string> AliasedPowerIdentityCollisionDetails { get; } = [];
    public List<string> MalformedPowerNameRepairDetails { get; } = [];
    public List<string> MalformedPowerDuplicateRemovalDetails { get; } = [];
    public List<string> MalformedPowerNameCollisionDetails { get; } = [];
    public List<string> PowersetPowerOrderDetails { get; } = [];
    public List<string> ExcludedContentRemovalDetails { get; } = [];
    public List<string> ExcludedClassContentRemovalDetails { get; } = [];
    public List<string> ImportIntegrityAuditDetails { get; } = [];
    public List<string> NewOrphanPowerDetails { get; } = [];
    public List<string> OrphanedScopedOmniPowerDetails { get; } = [];
    public List<string> ScopedDisplayFallbackRejectedDetails { get; } = [];
    public List<string> AcceptedCanonicalScopedPowerReplacementDetails { get; } = [];
    public List<string> ExcludedScopedOmniPowerDetails { get; } = [];
    public List<string> ManifestOwnedScopedOmniPowerDetails { get; } = [];
    public List<string> RetainedTemporaryIntegrityDetails { get; } = [];
    public List<string> PowersetIdentityChangeDetails { get; } = [];
    public List<string> StaffMasteryTraceDetails { get; } = [];
    public List<string> DuplicateCompositePowerIdentityDetails { get; } = [];
    public List<string> EpicImportDetails { get; } = [];
    public List<string> EpicPowersetIdentityDetails { get; } = [];
    public List<string> EpicPowersetPrefixSuffixCandidates { get; } = [];
    public List<string> EpicPowersetDisplayNameCollisions { get; } = [];
    public List<string> EpicPowersetsWithZeroLinkedPowersDetails { get; } = [];
    public List<string> EpicPowersMissingAfterImportDetails { get; } = [];
    public List<string> EpicPowersWithMissingPowersetDetails { get; } = [];
    public List<string> EpicPowersWithInvalidPowersetIdDetails { get; } = [];
    public List<string> EpicPowersNotInPowersetArrayDetails { get; } = [];
    public List<string> EpicPowersHiddenInDbEditorDetails { get; } = [];
    public List<string> PetImportDetails { get; } = [];
    public List<string> PetPowersRepairedBeforeLinkingDetails { get; } = [];
    public List<string> PetPowersMissingAfterImportDetails { get; } = [];
    public List<string> PetPowersWithMissingPowersetDetails { get; } = [];
    public List<string> PetPowersWithInvalidPowersetIdDetails { get; } = [];
    public List<string> PetPowersNotInPowersetArrayDetails { get; } = [];
    public List<string> PetImportManifestDetails { get; } = [];
    public List<string> PetSourceOfTruthUpsertDetails { get; } = [];
    public List<string> PetLinkIntegrityFailureDetails { get; } = [];
    public List<string> PetPowersetsWithZeroLinkedPowersDetails { get; } = [];
    public List<string> MappedPowerFieldDetails { get; } = [];
    public List<string> PowerFieldFallbackDetails { get; } = [];
    public List<string> PowerFieldConflictDetails { get; } = [];
    public List<string> KnownChargeCapacityDetailEntries { get; } = [];
    public List<string> DeferredPowerFieldDetails { get; } = [];
    public List<string> IgnoredPowerFieldDetails { get; } = [];
    public List<string> UnknownPowerFieldDetails { get; } = [];
    public List<string> PoolPowersetIconAuditDetails { get; } = [];
    public List<string> PoolIconAssignmentDetails { get; } = [];
    public List<string> MissingPoolIconAssetDetails { get; } = [];
    public List<string> PoolIconIntegrityDetails { get; } = [];
    public List<string> PoolPowerLinkIntegrityDetails { get; } = [];
    public List<string> SorceryEnflamePickabilityTraceDetails { get; } = [];
    public List<string> PoolRequirementEvaluationFailureDetails { get; } = [];
    public List<string> PseudoPetEntityDetails { get; } = [];
    public List<string> PseudoPetAbsorptionFlagDetails { get; } = [];
    public List<string> PseudoPetAbsorptionAuditDetails { get; } = [];
    public List<string> PseudoPetAbsorptionAuditSkippedDetails { get; } = [];
    public List<string> PvTargetGatingAuditDetails { get; } = [];
    public List<string> PvTargetMappingMismatchDetails { get; } = [];
    public List<string> EnhancementImportDetails { get; } = [];
    public List<string> ClassicEnhancementFoldingDetails { get; } = [];
    public List<string> EnhancementSourceShapeDetails { get; } = [];
    public List<string> UnresolvedEnhancementPowerLinks { get; } = [];
    public List<string> EnhancementIconDetails { get; } = [];
    public List<string> EnhancementClassDerivationDetails { get; } = [];
    public List<string> VectorTemplateSemanticOverrideDetails { get; } = [];
    public List<string> BoostHelperCarrierLinkedBonusDetails { get; } = [];
    public List<string> BoostHelperCarrierLocalResolutionDetails { get; } = [];
    public List<string> BoostHelperCarrierUnresolvedDetails { get; } = [];
    public List<string> Boosts20DamageMappingDetails { get; } = [];
    public List<string> BoostSetBonusImportAuditDetails { get; } = [];
    public List<string> EnhancementReconciliationAuditDetails { get; } = [];
    public List<string> EnhancementReconciliationConflictDetails { get; } = [];
    public List<string> EnhancementNamingReconciliationDetails { get; } = [];

    public void AddLimited(ICollection<string> target, string value, int limit = 500)
    {
        if (string.IsNullOrWhiteSpace(value) || target.Count >= limit || target.Contains(value))
        {
            return;
        }

        target.Add(value);
    }

    [JsonProperty("summary")]
    public OmniReportSummary Summary => OmniReportLegibility.BuildSummary(this);

    public string ToMarkdown()
    {
        var summary = Summary;
        var ignoredPowerFieldBreakdown = OmniReportLegibility.BuildIgnoredPowerFieldBreakdown(IgnoredPowerFields, IgnoredPowerFieldNameCounts);
        var builder = new StringBuilder();

        builder.AppendLine("# Omni Safe Import Apply");
        builder.AppendLine();
        OmniReportLegibility.AppendMarkdownSummary(builder, summary);
        OmniReportLegibility.AppendMarkdownAppendixHeader(builder);

        OmniReportLegibility.AppendMarkdownAppendixGroup(builder, "Raw Metric Snapshot");
        builder.AppendLine($"- Archetypes read/playable/retained/skipped: {ArchetypesRead:n0}/{PlayableArchetypesRead:n0}/{RetainedArchetypesRead:n0}/{SkippedArchetypesRead:n0}");
        builder.AppendLine($"- Powersets matched/created/updated: {PowersetsMatched:n0}/{PowersetsCreated:n0}/{PowersetsUpdated:n0}");
        builder.AppendLine($"- Powers matched/created/updated: {PowersMatched:n0}/{PowersCreated:n0}/{PowersUpdated:n0}");
        builder.AppendLine($"- Requirements updated/skipped unsupported: {RequirementsUpdated:n0}/{RequirementsSkippedUnsupported:n0}");
        builder.AppendLine($"- Retained temp powersets/powers/hidden/failures: {RetainedTemporaryPowersetsTracked:n0}/{RetainedTemporaryPowersTracked:n0}/{RetainedTemporaryHiddenPowers:n0}/{RetainedTemporaryIntegrityFailures:n0}");
        builder.AppendLine($"- Unknown effect/attribute mappings: {UnknownEffectMappings:n0}/{UnknownAttribMappings:n0}");
        builder.AppendLine($"- Orphan powers/scoped missing-detached after import: {OrphanPowersAfterImport:n0}/{OrphanedScopedOmniPowers:n0}");
        builder.AppendLine($"- Pet powers missing/source integrity failures: {PetPowersMissingAfterImport:n0}/{PetSourceIntegrityFailures:n0}");
        builder.AppendLine($"- Power fields mapped/fallback/conflicts/unknown: {PowerFieldsMapped:n0}/{PowerFieldsMappedWithFallback:n0}/{PowerFieldConflicts:n0}/{UnknownPowerFields:n0}");
        builder.AppendLine($"- Powerset icons preserved/assigned/missing: {PoolPowersetIconsPreserved:n0}/{PoolPowersetIconsAssigned:n0}/{PoolPowersetIconsMissingAssets:n0}");
        builder.AppendLine($"- Pseudo-pet absorption enabled/skipped/failures: {PseudoPetAbsorptionFlagsEnabled:n0}/{PseudoPetAbsorptionAuditSkipped:n0}/{PseudoPetAbsorptionAuditFailures:n0}");
        builder.AppendLine($"- Enhancements matched/created/updated: {EnhancementsMatched:n0}/{EnhancementsCreated:n0}/{EnhancementsUpdated:n0}");
        builder.AppendLine($"- Enhancement links resolved/missing: {EnhancementBoostPowerLinksResolved + EnhancementSetBonusLinksResolved + RecipeRewardLinksResolved:n0}/{EnhancementBoostPowerLinksMissing + EnhancementSetBonusLinksMissing + RecipeRewardLinksMissing:n0}");

        OmniReportLegibility.AppendMarkdownAppendixGroup(builder, "Needs Implementation / Manual Review");
        AppendSection(builder, "Missing Powers", MissingPowers);
        AppendSection(builder, "Requirement Skips", RequirementSkips);
        AppendSection(builder, "Manual Classification Review", ManualClassificationReviewDetails);
        AppendSection(builder, "Unknown Modes Preserved", UnknownModeDetails);
        AppendSection(builder, "Import Integrity Audit", ImportIntegrityAuditDetails);
        AppendSection(builder, "New Orphan Powers Introduced", NewOrphanPowerDetails);
        AppendSection(builder, "Missing / Detached Scoped Omni Powers", OrphanedScopedOmniPowerDetails);
        AppendSection(builder, "Scoped Display Fallback Rejected", ScopedDisplayFallbackRejectedDetails);
        AppendSection(builder, "Epic Powersets With Zero Linked Powers", EpicPowersetsWithZeroLinkedPowersDetails);
        AppendSection(builder, "Epic Powers Missing After Import", EpicPowersMissingAfterImportDetails);
        AppendSection(builder, "Pet Powers Missing After Import", PetPowersMissingAfterImportDetails);
        AppendSection(builder, "Pet Link Integrity Failures", PetLinkIntegrityFailureDetails);
        AppendSection(builder, "Field Conflicts", PowerFieldConflictDetails);
        AppendSection(builder, "Unknown Power Fields", UnknownPowerFieldDetails);
        AppendSection(builder, "Missing Powerset Icon Assets", MissingPoolIconAssetDetails);
        AppendSection(builder, "Pool Icon Integrity", PoolIconIntegrityDetails);
        AppendSection(builder, "Pool Power Link Integrity", PoolPowerLinkIntegrityDetails);
        AppendSection(builder, "Pool Requirement Evaluation Failures", PoolRequirementEvaluationFailureDetails);
        AppendSection(builder, "Pseudo-Pet Absorption Audit Failures", PseudoPetAbsorptionAuditDetails);
        AppendSection(builder, "PvX Target Mapping Mismatches", PvTargetMappingMismatchDetails);
        AppendSection(builder, "Unresolved Enhancement Power Links", UnresolvedEnhancementPowerLinks);
        AppendSection(builder, "Enhancement Class Derivation", EnhancementClassDerivationDetails);
        AppendSection(builder, "Scoped Power Enhancement Legality", ScopedPowerEnhancementLegalityDetails);
        AppendSection(builder, "Scoped Power Enhancement Legality Unknown Labels", ScopedPowerEnhancementLegalityUnknownLabelDetails);
        AppendSection(builder, "Boosts And Set Bonus Legality Repair", BoostSetBonusPowerLegalityRepairDetails);
        AppendSection(builder, "Enhancement Reconciliation Conflicts", EnhancementReconciliationConflictDetails);
        AppendSection(builder, "Unknown Effect Mappings", UnknownEffectMappingDetails);
        AppendSection(builder, "Unknown Attribute Mappings", UnknownAttribMappingDetails);

        OmniReportLegibility.AppendMarkdownAppendixGroup(builder, "Known Unsupported / Deferred");
        AppendSection(builder, "Known Charge-Capacity Extensions", KnownChargeCapacityDetailEntries);
        AppendSection(builder, "Known Deferred Power Fields", DeferredPowerFieldDetails);
        AppendSection(builder, "Known Unsupported Effect Mappings", KnownUnsupportedEffectMappingDetails);
        OmniReportLegibility.AppendIgnoredPowerFieldNarrative(
            builder,
            ignoredPowerFieldBreakdown.HarmlessSourceMetadataCount,
            ignoredPowerFieldBreakdown.UiClientServerMetadataCount,
            ignoredPowerFieldBreakdown.UnsupportedPlannerPolicyCount);

        OmniReportLegibility.AppendMarkdownAppendixGroup(builder, "Informational / Report-Only");
        AppendSection(builder, "Hidden Support Powers", HiddenSupportPowerDetails);
        AppendSection(builder, "Visible Gated Powers", VisibleGatedPowerDetails);
        AppendSection(builder, "GCM Tags Added", GcmTagsAddedDetails);
        AppendSection(builder, "GCM Tags Already Known", GcmTagsAlreadyKnownDetails);
        AppendSection(builder, "GCM Duplicate Tags", GcmDuplicateTagDetails);
        AppendSection(builder, "Effect Group Tags Preserved", EffectGroupTagDetails);
        AppendSection(builder, "Template/Filter Tags Mapped", TemplateFilterTagDetails);
        AppendSection(builder, "Chance Mod Mappings", ChanceModMappingDetails);
        AppendCountSection(builder, "Ignored Power Field Owner Kinds", IgnoredPowerFieldOwnerKindCounts);
        AppendCountSection(builder, "Ignored Power Field Categories", IgnoredPowerFieldCategoryCounts);
        AppendCountSection(builder, "Ignored Power Fields By Name", IgnoredPowerFieldNameCounts);
        AppendSection(builder, "Ignored Power Field Samples", IgnoredPowerFieldDetails);
        AppendSection(builder, "Powerset Icon Audit", PoolPowersetIconAuditDetails);
        AppendSection(builder, "Powerset Icon Assignments", PoolIconAssignmentDetails);
        AppendSection(builder, "Sorcery Enflame Pickability Trace", SorceryEnflamePickabilityTraceDetails);
        AppendSection(builder, "Pseudo-Pet Absorption Audit Skipped / Report-Only", PseudoPetAbsorptionAuditSkippedDetails);
        AppendSection(builder, "PvX Target And Gating Audit", PvTargetGatingAuditDetails);
        AppendSection(builder, "Vector Template Semantic Overrides", VectorTemplateSemanticOverrideDetails);
        AppendSection(builder, "Classic Enhancement Presentation", ClassicEnhancementPresentationDetails);

        OmniReportLegibility.AppendMarkdownAppendixGroup(builder, "Implemented Coverage / Audit Trails");
        AppendSection(builder, "Created Powersets", CreatedPowersets);
        AppendSection(builder, "Updated Powersets", UpdatedPowersets);
        AppendSection(builder, "Created Powers", CreatedPowers);
        AppendSection(builder, "Updated Powers", UpdatedPowers);
        AppendSection(builder, "Skipped Powers", SkippedPowers);
        AppendSection(builder, "Support Power Links", SupportPowerLinks);
        AppendSection(builder, "Support Power Link Skips", SupportPowerLinkSkips);
        AppendSection(builder, "Redirect Execution Variants", RedirectExecutionVariantDetails);
        AppendSection(builder, "Click-Buff Classification Changes", ClickBuffClassificationChangeDetails);
        AppendSection(builder, "Class Inherents Imported", ClassInherentDetails);
        AppendSection(builder, "Form-Gated Powers Hidden", FormGatedPowerDetails);
        AppendSection(builder, "Power-Level Mode Gates Assigned", ModeFlagDetails);
        AppendSection(builder, "Planner Inherents Preserved", PlannerInherentDetails);
        AppendSection(builder, "Synthetic Planner Inherents", SyntheticPlannerInherentDetails);
        AppendSection(builder, "Planner Modes Discovered", PlannerModeDetails);
        AppendSection(builder, "Planner Mode Payloads Added", PlannerModePayloadDetails);
        AppendSection(builder, "kEngaged Snipe Aliases", SnipeEngagedAliasDetails);
        AppendSection(builder, "Class Table Files", ClassTableFileDetails);
        AppendSection(builder, "Missing Modifier Table References", MissingModifierTableReferenceDetails);
        AppendSection(builder, "Attack Vectors Mapped", AttackVectorDetails);
        AppendSection(builder, "Zero-Value Tag Carrier Effects Suppressed", ZeroValueTagCarrierDetails);
        AppendSection(builder, "Missing Scoped Powersets Repaired", MissingScopedPowersetRepairDetails);
        AppendSection(builder, "Scoped Power Identity Repairs", ScopedPowerSetIdentityRepairDetails);
        AppendSection(builder, "Aliased Powerset Identity Repairs", AliasedPowersetIdentityRepairDetails);
        AppendSection(builder, "Aliased Duplicate Powersets Removed", AliasedPowersetDuplicateRemovalDetails);
        AppendSection(builder, "Aliased Powerset Identity Collisions", AliasedPowersetIdentityCollisionDetails);
        AppendSection(builder, "Aliased Power Identity Repairs", AliasedPowerIdentityRepairDetails);
        AppendSection(builder, "Aliased Duplicate Powers Removed", AliasedPowerDuplicateRemovalDetails);
        AppendSection(builder, "Aliased Power Identity Collisions", AliasedPowerIdentityCollisionDetails);
        AppendSection(builder, "Malformed Power Name Repairs", MalformedPowerNameRepairDetails);
        AppendSection(builder, "Malformed Duplicate Powers Removed", MalformedPowerDuplicateRemovalDetails);
        AppendSection(builder, "Malformed Power Name Collisions", MalformedPowerNameCollisionDetails);
        AppendSection(builder, "Powersets Reordered By Power Level", PowersetPowerOrderDetails);
        AppendSection(builder, "Excluded Content Removed", ExcludedContentRemovalDetails);
        AppendSection(builder, "Import Integrity Audit", ImportIntegrityAuditDetails);
        AppendSection(builder, "New Orphan Powers Introduced", NewOrphanPowerDetails);
        AppendSection(builder, "Missing / Detached Scoped Omni Powers", OrphanedScopedOmniPowerDetails);
        AppendSection(builder, "Scoped Display Fallback Rejected", ScopedDisplayFallbackRejectedDetails);
        AppendSection(builder, "Accepted Canonical Scoped Power Replacements", AcceptedCanonicalScopedPowerReplacementDetails);
        AppendSection(builder, "Excluded Scoped Omni Powers", ExcludedScopedOmniPowerDetails);
        AppendSection(builder, "Manifest-Owned Scoped Omni Powers", ManifestOwnedScopedOmniPowerDetails);
        AppendSection(builder, "Retained Temporary Power Integrity", RetainedTemporaryIntegrityDetails);
        AppendSection(builder, "Powerset Identity Changes", PowersetIdentityChangeDetails);
        AppendSection(builder, "Staff Mastery Trace", StaffMasteryTraceDetails);
        AppendSection(builder, "Duplicate Composite Power Identities", DuplicateCompositePowerIdentityDetails);
        AppendSection(builder, "Epic Import Details", EpicImportDetails);
        AppendSection(builder, "Epic Powerset Identity Details", EpicPowersetIdentityDetails);
        AppendSection(builder, "Epic Powerset Prefix/Suffix Candidates", EpicPowersetPrefixSuffixCandidates);
        AppendSection(builder, "Epic Powerset Display Name Collisions", EpicPowersetDisplayNameCollisions);
        AppendSection(builder, "Epic Powers With Missing Powerset", EpicPowersWithMissingPowersetDetails);
        AppendSection(builder, "Epic Powers With PowerSetID < 0", EpicPowersWithInvalidPowersetIdDetails);
        AppendSection(builder, "Epic Powers Not In Powerset Array", EpicPowersNotInPowersetArrayDetails);
        AppendSection(builder, "Epic Powers Hidden In DB Editor", EpicPowersHiddenInDbEditorDetails);
        AppendSection(builder, "Pet Import Details", PetImportDetails);
        AppendSection(builder, "Pet Powers Repaired Before Linking", PetPowersRepairedBeforeLinkingDetails);
        AppendSection(builder, "Pet Powers With Missing Powerset", PetPowersWithMissingPowersetDetails);
        AppendSection(builder, "Pet Powers With PowerSetID < 0", PetPowersWithInvalidPowersetIdDetails);
        AppendSection(builder, "Pet Powers Not In Powerset Array", PetPowersNotInPowersetArrayDetails);
        AppendSection(builder, "Pet Import Manifest", PetImportManifestDetails);
        AppendSection(builder, "Pet Source-Of-Truth Upsert", PetSourceOfTruthUpsertDetails);
        AppendSection(builder, "Pet Powersets With Zero Linked Powers", PetPowersetsWithZeroLinkedPowersDetails);
        AppendSection(builder, "Power Field Mapping Coverage", MappedPowerFieldDetails);
        AppendSection(builder, "Mapped With Fallback", PowerFieldFallbackDetails);
        AppendSection(builder, "Pseudo-Pet Entities", PseudoPetEntityDetails);
        AppendSection(builder, "Pseudo-Pet Absorption Flags", PseudoPetAbsorptionFlagDetails);
        AppendSection(builder, "Enhancement Import", EnhancementImportDetails);
        AppendSection(builder, "Classic Enhancement Folding", ClassicEnhancementFoldingDetails);
        AppendSection(builder, "Enhancement Source Shape Validation", EnhancementSourceShapeDetails);
        AppendSection(builder, "Enhancement Icon Decisions", EnhancementIconDetails);
        AppendSection(builder, "Boosts And Set Bonus Import Audit", BoostSetBonusImportAuditDetails);
        AppendSection(builder, "Enhancement Reconciliation Audit", EnhancementReconciliationAuditDetails);
        AppendSection(builder, "Enhancement Naming Reconciliation", EnhancementNamingReconciliationDetails);
        AppendSection(builder, "Known Hidden/Stateful Effect Mappings", KnownHiddenStatefulEffectMappingDetails);

        return builder.ToString();
    }

    private string BuildLegacyMarkdown()
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Omni Safe Import Apply");
        builder.AppendLine();
        builder.AppendLine($"- Class attributes stored: {ClassAttributesStored}");
        builder.AppendLine($"- Archetypes read/playable/retained/skipped: {ArchetypesRead}/{PlayableArchetypesRead}/{RetainedArchetypesRead}/{SkippedArchetypesRead}");
        builder.AppendLine($"- Archetype summaries updated: {ArchetypeSummariesUpdated}");
        builder.AppendLine($"- Powersets matched: {PowersetsMatched}");
        builder.AppendLine($"- Powersets created: {PowersetsCreated}");
        builder.AppendLine($"- Powersets updated: {PowersetsUpdated}");
        builder.AppendLine($"- Powers matched: {PowersMatched}");
        builder.AppendLine($"- Powers created: {PowersCreated}");
        builder.AppendLine($"- Powers updated: {PowersUpdated}");
        builder.AppendLine($"- Powers skipped by NeverAutoUpdate: {PowersSkippedNeverAutoUpdate}");
        builder.AppendLine($"- Requirements updated: {RequirementsUpdated}");
        builder.AppendLine($"- Requirements skipped by NeverAutoUpdateRequirements: {RequirementsSkippedNeverAutoUpdateRequirements}");
        builder.AppendLine($"- Requirements skipped for unsupported expressions: {RequirementsSkippedUnsupported}");
        builder.AppendLine($"- Effects replaced: {EffectsReplaced}");
        builder.AppendLine($"- Redirect effects added: {RedirectEffectsAdded}");
        builder.AppendLine($"- Powers missing from Mids: {PowersMissingFromMids}");
        builder.AppendLine($"- Unknown effect mappings: {UnknownEffectMappings}");
        builder.AppendLine($"- Unknown attribute mappings: {UnknownAttribMappings}");
        builder.AppendLine($"- Known hidden/stateful effect mappings: {KnownHiddenStatefulEffectMappings}");
        builder.AppendLine($"- Known unsupported effect mappings: {KnownUnsupportedEffectMappings}");
        builder.AppendLine($"- Support powersets removed before rebuild: {SupportPowersetsRemoved}");
        builder.AppendLine($"- Support powers removed before rebuild: {SupportPowersRemoved}");
        builder.AppendLine($"- Hidden support powers classified: {HiddenSupportPowers}");
        builder.AppendLine($"- Visible gated powers classified: {VisibleGatedPowers}");
        builder.AppendLine($"- Support power links created: {SupportPowerLinksCreated}");
        builder.AppendLine($"- Support power links unresolved: {SupportPowerLinksUnresolved}");
        builder.AppendLine($"- Redirect execution variants: {RedirectExecutionVariants}");
        builder.AppendLine($"- Click-buff classification changes: {ClickBuffClassificationChanges}");
        builder.AppendLine($"- Manual classification review: {ManualClassificationReviews}");
        builder.AppendLine($"- Support-heavy powers recreated: {SupportHeavyPowersRecreated}");
        builder.AppendLine($"- Class inherents imported: {ClassInherentsImported}");
        builder.AppendLine($"- Form-gated powers hidden: {FormGatedPowersHidden}");
        builder.AppendLine($"- Power-level mode gates assigned: {ModeFlagsAssigned}");
        builder.AppendLine($"- Unknown modes preserved: {UnknownModesPreserved}");
        builder.AppendLine($"- Planner inherents preserved: {PlannerInherentsPreserved}");
        builder.AppendLine($"- Synthetic planner inherents created: {SyntheticPlannerInherentsCreated}");
        builder.AppendLine($"- Mode catalog entries loaded: {ModeCatalogEntriesLoaded}");
        builder.AppendLine($"- Planner modes discovered: {PlannerModesDiscovered}");
        builder.AppendLine($"- Planner mode payloads added: {PlannerModePayloadsAdded}");
        builder.AppendLine($"- kEngaged snipe aliases: {SnipeEngagedAliases}");
        builder.AppendLine($"- GCM tags read: {GcmTagsRead}");
        builder.AppendLine($"- GCM tags already known: {GcmTagsAlreadyKnown}");
        builder.AppendLine($"- GCM tags added: {GcmTagsAdded}");
        builder.AppendLine($"- GCM duplicate tags ignored: {GcmDuplicateTags}");
        builder.AppendLine($"- GCM blank tags ignored: {GcmBlankTags}");
        builder.AppendLine($"- Effect-group tags preserved: {EffectGroupTagsPreserved}");
        builder.AppendLine($"- Template/filter tags mapped: {TemplateFilterTagsMapped}");
        builder.AppendLine($"- Global chance mods mapped: {GlobalChanceModsMapped}");
        builder.AppendLine($"- Power-local chance mods mapped: {PowerLocalChanceModsMapped}");
        builder.AppendLine($"- Unsupported EffectFilter fields preserved: {UnsupportedEffectFilters}");
        builder.AppendLine($"- Class table files read: {ClassTableFilesRead}");
        builder.AppendLine($"- Retained/skipped class table files: {RetainedClassTableFiles}/{SkippedClassTableFiles}");
        builder.AppendLine($"- Canonical named tables stored: {CanonicalNamedTablesStored}");
        builder.AppendLine($"- Missing modifier table references: {MissingModifierTableReferences}");
        builder.AppendLine($"- Attack vectors mapped: {AttackVectorsMapped}");
        builder.AppendLine($"- Zero-value tag carrier effects suppressed: {ZeroValueTagCarrierEffectsSuppressed}");
        builder.AppendLine($"- Missing scoped powersets repaired: {MissingScopedPowersetsRepaired}");
        builder.AppendLine($"- Scoped power identity repairs: {ScopedPowerSetIdentityRepairs}");
        builder.AppendLine($"- Aliased powerset identity repairs: {AliasedPowersetIdentityRepairs}");
        builder.AppendLine($"- Aliased duplicate powersets removed: {AliasedPowersetDuplicateRemovals}");
        builder.AppendLine($"- Aliased powerset identity collisions: {AliasedPowersetIdentityCollisions}");
        builder.AppendLine($"- Aliased power identity repairs: {AliasedPowerIdentityRepairs}");
        builder.AppendLine($"- Aliased duplicate powers removed: {AliasedPowerDuplicateRemovals}");
        builder.AppendLine($"- Aliased power identity collisions: {AliasedPowerIdentityCollisions}");
        builder.AppendLine($"- Malformed power name repairs: {MalformedPowerNameRepairs}");
        builder.AppendLine($"- Malformed duplicate powers removed: {MalformedPowerDuplicateRemovals}");
        builder.AppendLine($"- Malformed power name collisions: {MalformedPowerNameCollisions}");
        builder.AppendLine($"- Powersets reordered by power level: {PowersetsReorderedByPowerLevel}");
        builder.AppendLine($"- Excluded archetypes removed: {ExcludedArchetypesRemoved}");
        builder.AppendLine($"- Excluded class-owned powersets/powers/entities removed: {ExcludedClassPowersetsRemoved}/{ExcludedClassPowersRemoved}/{ExcludedClassEntitiesRemoved}");
        builder.AppendLine($"- Excluded powersets removed: {ExcludedPowersetsRemoved}");
        builder.AppendLine($"- Excluded powers removed: {ExcludedPowersRemoved}");
        builder.AppendLine($"- Orphan powers before import: {OrphanPowersBeforeImport}");
        builder.AppendLine($"- Orphan powers after import: {OrphanPowersAfterImport}");
        builder.AppendLine($"- New orphan powers introduced: {NewOrphanPowersIntroduced}");
        builder.AppendLine($"- Missing / detached scoped Omni powers: {OrphanedScopedOmniPowers}");
        builder.AppendLine($"- Exact retained scoped Omni powers: {ExactScopedOmniPowersRetained}");
        builder.AppendLine($"- Accepted canonical scoped replacements: {AcceptedCanonicalScopedPowerReplacements}");
        builder.AppendLine($"- Scoped display fallback rejected: {ScopedDisplayFallbackRejected}");
        builder.AppendLine($"- Excluded scoped Omni powers: {ExcludedScopedOmniPowers}");
        builder.AppendLine($"- Manifest-owned scoped Omni powers: {ManifestOwnedScopedOmniPowers}");
        builder.AppendLine($"- Powerset identity changes: {PowersetIdentityChanges}");
        builder.AppendLine($"- Duplicate composite power identities: {DuplicateCompositePowerIdentities}");
        builder.AppendLine($"- Epic powersets created: {EpicPowersetsCreated}");
        builder.AppendLine($"- Epic powersets matched: {EpicPowersetsMatched}");
        builder.AppendLine($"- Epic powersets updated: {EpicPowersetsUpdated}");
        builder.AppendLine($"- Epic powers created: {EpicPowersCreated}");
        builder.AppendLine($"- Epic powers matched: {EpicPowersMatched}");
        builder.AppendLine($"- Epic powers updated: {EpicPowersUpdated}");
        builder.AppendLine($"- Epic powers skipped: {EpicPowersSkipped}");
        builder.AppendLine($"- Epic powers linked after MatchIds: {EpicPowersLinkedAfterMatchIds}");
        builder.AppendLine($"- Epic powersets with zero linked powers: {EpicPowersetsWithZeroLinkedPowers}");
        builder.AppendLine($"- Epic powers missing after import: {EpicPowersMissingAfterImport}");
        builder.AppendLine($"- Epic powers with missing powerset: {EpicPowersWithMissingPowerset}");
        builder.AppendLine($"- Epic powers with PowerSetID < 0: {EpicPowersWithInvalidPowersetId}");
        builder.AppendLine($"- Epic powers not in powerset array: {EpicPowersNotInPowersetArray}");
        builder.AppendLine($"- Epic powers hidden in DB editor: {EpicPowersHiddenInDbEditor}");
        builder.AppendLine($"- Pet powers created: {PetPowersCreated} ({FormatCounts(PetPowersCreatedByRoot)})");
        builder.AppendLine($"- Pet powers matched: {PetPowersMatched} ({FormatCounts(PetPowersMatchedByRoot)})");
        builder.AppendLine($"- Pet powers updated: {PetPowersUpdated} ({FormatCounts(PetPowersUpdatedByRoot)})");
        builder.AppendLine($"- Pet powers skipped: {PetPowersSkipped} ({FormatCounts(PetPowersSkippedByRoot)})");
        builder.AppendLine($"- Pet powers recreated: {PetPowersRecreated} ({FormatCounts(PetPowersRecreatedByRoot)})");
        builder.AppendLine($"- Pet powers repaired before linking: {PetPowersRepairedBeforeLinking} ({FormatCounts(PetPowersRepairedByRoot)})");
        builder.AppendLine($"- Pet powers linked after MatchIds: {PetPowersLinkedAfterMatchIds} ({FormatCounts(PetPowersLinkedByRoot)})");
        builder.AppendLine($"- Pet powers missing after import: {PetPowersMissingAfterImport}");
        builder.AppendLine($"- Pet powers with missing powerset: {PetPowersWithMissingPowerset}");
        builder.AppendLine($"- Pet powers with PowerSetID < 0: {PetPowersWithInvalidPowersetId}");
        builder.AppendLine($"- Pet powers not in powerset array: {PetPowersNotInPowersetArray}");
        builder.AppendLine($"- Pet manifest roots present: {PetManifestRootsPresent}");
        builder.AppendLine($"- Pet manifest roots missing: {PetManifestRootsMissing}");
        builder.AppendLine($"- Pet manifest powersets: {PetManifestPowersets}");
        builder.AppendLine($"- Pet manifest expected powers: {PetManifestExpectedPowers}");
        builder.AppendLine($"- Pet manifest power files: {PetManifestPowerFiles}");
        builder.AppendLine($"- Pet manifest missing power files: {PetManifestMissingPowerFiles}");
        builder.AppendLine($"- Pet source powersets matched: {PetSourcePowersetsMatched}");
        builder.AppendLine($"- Pet source powersets created: {PetSourcePowersetsCreated}");
        builder.AppendLine($"- Pet source powersets updated: {PetSourcePowersetsUpdated}");
        builder.AppendLine($"- Pet source powers matched: {PetSourcePowersMatched}");
        builder.AppendLine($"- Pet source powers created: {PetSourcePowersCreated}");
        builder.AppendLine($"- Pet source powers updated: {PetSourcePowersUpdated}");
        builder.AppendLine($"- Pet source integrity failures: {PetSourceIntegrityFailures}");
        builder.AppendLine($"- Pet source powersets with zero linked powers: {PetSourcePowersetsWithZeroLinkedPowers}");
        builder.AppendLine($"- Power fields mapped: {PowerFieldsMapped}");
        builder.AppendLine($"- Power fields mapped with fallback: {PowerFieldsMappedWithFallback}");
        builder.AppendLine($"- Power field conflicts: {PowerFieldConflicts}");
        builder.AppendLine($"- Known charge-capacity extensions: {KnownChargeCapacityExtensions}");
        builder.AppendLine($"- Known deferred power fields: {DeferredPowerFields}");
        builder.AppendLine($"- Ignored UI/client/server power fields: {IgnoredPowerFields} across {IgnoredPowerFieldOwnerKindCounts.Count} owner kinds / {IgnoredPowerFieldCategoryCounts.Count} categories / {IgnoredPowerFieldNameCounts.Count} unique fields (sampled {IgnoredPowerFieldDetails.Count})");
        builder.AppendLine($"- Unknown power fields: {UnknownPowerFields}");
        builder.AppendLine($"- Powerset icons preserved: {PoolPowersetIconsPreserved}");
        builder.AppendLine($"- Powerset icons assigned: {PoolPowersetIconsAssigned}");
        builder.AppendLine($"- Missing powerset icon assets: {PoolPowersetIconsMissingAssets}");
        builder.AppendLine($"- Pool icon integrity issues: {PoolIconIntegrityIssues}");
        builder.AppendLine($"- Pool power link integrity issues: {PoolPowerLinkIntegrityIssues}");
        builder.AppendLine($"- Sorcery Enflame pickability traces: {SorceryEnflameTraceCount}");
        builder.AppendLine($"- Pool requirement evaluation failures: {PoolRequirementEvaluationFailures}");
        builder.AppendLine($"- Pseudo-pet entities created: {PseudoPetEntitiesCreated}");
        builder.AppendLine($"- Pseudo-pet entities updated: {PseudoPetEntitiesUpdated}");
        builder.AppendLine($"- Real pet entities created: {RealPetEntitiesCreated}");
        builder.AppendLine($"- Real pet entities updated: {RealPetEntitiesUpdated}");
        builder.AppendLine($"- Pseudo-pet absorption flags enabled: {PseudoPetAbsorptionFlagsEnabled}");
        builder.AppendLine($"- Pseudo-pet absorption audit skipped/report-only: {PseudoPetAbsorptionAuditSkipped}");
        builder.AppendLine($"- Pseudo-pet absorption audit failures: {PseudoPetAbsorptionAuditFailures}");
        builder.AppendLine($"- PvMode inferred from target entity expressions: {PvModeInferredFromTargetEntity}");
        builder.AppendLine($"- PvMode inferred from modifier tables: {PvModeInferredFromTable}");
        builder.AppendLine($"- Ambiguous PvX target expressions preserved as Any: {PvModeAmbiguous}");
        builder.AppendLine($"- PvX/target/gating audit entries: {PvTargetAuditEntries}");
        builder.AppendLine($"- PvX/target mapping mismatches: {PvTargetMappingMismatches}");
        builder.AppendLine($"- Salvage matched/created/updated: {SalvageMatched}/{SalvageCreated}/{SalvageUpdated}");
        builder.AppendLine($"- Recipes matched/created/updated: {RecipesMatched}/{RecipesCreated}/{RecipesUpdated}");
        builder.AppendLine($"- Enhancement sets matched/created/updated: {EnhancementSetsMatched}/{EnhancementSetsCreated}/{EnhancementSetsUpdated}");
        builder.AppendLine($"- Enhancements matched/created/updated: {EnhancementsMatched}/{EnhancementsCreated}/{EnhancementsUpdated}");
        builder.AppendLine($"- Enhancement files/recipe files/logical recipes/recipe variants: {EnhancementFilesProcessed}/{RecipeFilesProcessed}/{LogicalRecipesProcessed}/{RecipeLevelVariantsProcessed}");
        builder.AppendLine($"- Classic enhancement variants discovered/logical/folded: {ClassicEnhancementSourceVariantsDiscovered}/{ClassicEnhancementLogicalRecords}/{ClassicEnhancementVariantsFolded}");
        builder.AppendLine($"- Classic enhancement editor rows expected: {ClassicEnhancementEditorRowsExpected}");
        builder.AppendLine($"- Classic enhancement metadata warnings: {ClassicEnhancementMetadataWarnings}");
        builder.AppendLine($"- Recipes excluded by policy: {RecipeRecordsExcludedByPolicy}");
        builder.AppendLine($"- Enhancement records excluded by policy: {EnhancementRecordsExcludedByPolicy}");
        builder.AppendLine($"- Structured boosts_allowed parsed: {StructuredBoostsAllowedParsedCount}");
        builder.AppendLine($"- Enhancement class derivation effects/fallback/category-only/mismatch/unresolved: {EnhancementClassIdsDerivedFromEffects}/{EnhancementClassIdsFallbackUsed}/{EnhancementClassIdsCategoryOnly}/{EnhancementClassIdWrapperMismatches}/{EnhancementClassIdsUnresolved}");
        builder.AppendLine($"- Vector defense/resistance template overrides: {VectorDefenseTemplateOverrides}/{VectorResistanceTemplateOverrides}");
        builder.AppendLine($"- Helper carriers linked/local/unresolved: {BoostHelperCarriersResolvedFromLinkedBonus}/{BoostHelperCarriersResolvedLocally}/{BoostHelperCarriersUnresolved}");
        builder.AppendLine($"- Residual Boosts_20 damage mappings: {Boosts20DamageMappingsRemaining}");
        builder.AppendLine($"- Scoped power legality rebuilt/changed/preserved/empty/unresolved/unknown-labels: {ScopedPowerEnhancementLegalityRebuilt}/{ScopedPowerEnhancementLegalityChanged}/{ScopedPowerEnhancementLegalityPreserved}/{ScopedPowerEnhancementLegalityEmptyAfterRebuild}/{ScopedPowerEnhancementLegalityUnresolvedAfterRebuild}/{ScopedPowerEnhancementLegalityUnresolvedLabelCount}");
        builder.AppendLine($"- Boost/Set_Bonus legality repair boosts inspected/rebuilt/preserved/unresolved: {BoostPowerLegalityRepairInspected}/{BoostPowerLegalityRepairRebuilt}/{BoostPowerLegalityRepairPreserved}/{BoostPowerLegalityRepairUnresolved}");
        builder.AppendLine($"- Boost/Set_Bonus legality repair set-bonus inspected/cleared/already-empty/changed: {SetBonusPowerLegalityRepairInspected}/{SetBonusPowerLegalityRepairCleared}/{SetBonusPowerLegalityRepairAlreadyEmpty}/{BoostSetBonusPowerLegalityRepairChanged}");
        builder.AppendLine($"- Shape fallbacks / malformed skips: {EnhancementShapeCompatibilityFallbacks}/{EnhancementMalformedRecordsSkipped}");
        builder.AppendLine($"- Enhancement boost links resolved/missing: {EnhancementBoostPowerLinksResolved}/{EnhancementBoostPowerLinksMissing}");
        builder.AppendLine($"- Enhancement boost links via alias/fallback: {EnhancementBoostPowerLinksAlias}/{EnhancementBoostPowerLinksFallback}");
        builder.AppendLine($"- Set bonus links resolved/missing: {EnhancementSetBonusLinksResolved}/{EnhancementSetBonusLinksMissing}");
        builder.AppendLine($"- Set bonus links via alias/fallback: {EnhancementSetBonusLinksAlias}/{EnhancementSetBonusLinksFallback}");
        builder.AppendLine($"- Recipe reward links resolved/missing: {RecipeRewardLinksResolved}/{RecipeRewardLinksMissing}");
        builder.AppendLine($"- Enhancement icons preserved/assigned/missing: {EnhancementIconsPreserved}/{EnhancementIconsAssigned}/{EnhancementIconsMissing}");
        builder.AppendLine($"- Enhancement reconciliation exact/alias/linked/fallback/ambiguous: {EnhancementExactMatches}/{EnhancementAliasMatches}/{EnhancementLinkedPowerMatches}/{EnhancementFallbackMatches}/{EnhancementAmbiguousMatches}");
        builder.AppendLine($"- Enhancement set reconciliation exact/alias/fallback/ambiguous: {EnhancementSetsExactMatches}/{EnhancementSetsAliasMatches}/{EnhancementSetsFallbackMatches}/{EnhancementSetsAmbiguousMatches}");
        builder.AppendLine($"- Recipe reconciliation exact/alias/fallback/ambiguous: {RecipeExactMatches}/{RecipeAliasMatches}/{RecipeFallbackMatches}/{RecipeAmbiguousMatches}");
        builder.AppendLine($"- Salvage reconciliation exact/alias/fallback/ambiguous: {SalvageExactMatches}/{SalvageAliasMatches}/{SalvageFallbackMatches}/{SalvageAmbiguousMatches}");
        builder.AppendLine($"- Boost powersets in scope: {BoostPowersetsInScope}");
        builder.AppendLine($"- Boost powers in scope: {BoostPowersInScope}");
        builder.AppendLine($"- Set bonus powersets in scope: {SetBonusPowersetsInScope}");
        builder.AppendLine($"- Set bonus powers in scope: {SetBonusPowersInScope}");
        builder.AppendLine($"- Boost powerset coverage source: {DescribeCoverageSource(BoostExplicitPowersetsInScope, BoostDerivedPowersetsInScope)}");
        builder.AppendLine($"- Set bonus powerset coverage source: {DescribeCoverageSource(SetBonusExplicitPowersetsInScope, SetBonusDerivedPowersetsInScope)}");
        builder.AppendLine($"- Boost powers matched/created/updated: {BoostPowersMatched}/{BoostPowersCreated}/{BoostPowersUpdated}");
        builder.AppendLine($"- Set bonus powers matched/created/updated: {SetBonusPowersMatched}/{SetBonusPowersCreated}/{SetBonusPowersUpdated}");
        builder.AppendLine($"- Missing boost powers after import: {MissingBoostPowersAfterImport}");
        builder.AppendLine($"- Missing set bonus powers after import: {MissingSetBonusPowersAfterImport}");
        builder.AppendLine($"- Strict Set_Bonus purge powers/powersets: {StrictSetBonusPowersPurged}/{StrictSetBonusPowersetsPurged}");
        builder.AppendLine($"- Strict Set_Bonus rebuilt counts Global/Set/PvP: {StrictSetBonusGlobalBonusAfterImport}/{StrictSetBonusSetBonusAfterImport}/{StrictSetBonusPvpSetBonusAfterImport}");
        builder.AppendLine($"- Strict Set_Bonus cross-sibling mismatches/extras/missing: {StrictSetBonusCrossSiblingMismatchesAfterImport}/{StrictSetBonusDbOnlyExtrasAfterImport}/{StrictSetBonusMissingFullNamesAfterImport}");
        builder.AppendLine($"- Naming mismatches repaired (boost/set-bonus/enh/set/recipe/salvage): {BoostPowerAliasRepairs}/{SetBonusPowerAliasRepairs}/{EnhancementAliasRepairs}/{EnhancementSetAliasRepairs}/{RecipeAliasRepairs}/{SalvageAliasRepairs}");
        AppendSection(builder, "Created Powersets", CreatedPowersets);
        AppendSection(builder, "Updated Powersets", UpdatedPowersets);
        AppendSection(builder, "Created Powers", CreatedPowers);
        AppendSection(builder, "Updated Powers", UpdatedPowers);
        AppendSection(builder, "Skipped Powers", SkippedPowers);
        AppendSection(builder, "Missing Powers", MissingPowers);
        AppendSection(builder, "Requirement Skips", RequirementSkips);
        AppendSection(builder, "Hidden Support Powers", HiddenSupportPowerDetails);
        AppendSection(builder, "Visible Gated Powers", VisibleGatedPowerDetails);
        AppendSection(builder, "Support Power Links", SupportPowerLinks);
        AppendSection(builder, "Support Power Link Skips", SupportPowerLinkSkips);
        AppendSection(builder, "Redirect Execution Variants", RedirectExecutionVariantDetails);
        AppendSection(builder, "Click-Buff Classification Changes", ClickBuffClassificationChangeDetails);
        AppendSection(builder, "Manual Classification Review", ManualClassificationReviewDetails);
        AppendSection(builder, "Class Inherents Imported", ClassInherentDetails);
        AppendSection(builder, "Form-Gated Powers Hidden", FormGatedPowerDetails);
        AppendSection(builder, "Power-Level Mode Gates Assigned", ModeFlagDetails);
        AppendSection(builder, "Unknown Modes Preserved", UnknownModeDetails);
        AppendSection(builder, "Planner Inherents Preserved", PlannerInherentDetails);
        AppendSection(builder, "Synthetic Planner Inherents", SyntheticPlannerInherentDetails);
        AppendSection(builder, "Planner Modes Discovered", PlannerModeDetails);
        AppendSection(builder, "Planner Mode Payloads Added", PlannerModePayloadDetails);
        AppendSection(builder, "kEngaged Snipe Aliases", SnipeEngagedAliasDetails);
        AppendSection(builder, "GCM Tags Added", GcmTagsAddedDetails);
        AppendSection(builder, "GCM Tags Already Known", GcmTagsAlreadyKnownDetails);
        AppendSection(builder, "GCM Duplicate Tags", GcmDuplicateTagDetails);
        AppendSection(builder, "Effect Group Tags Preserved", EffectGroupTagDetails);
        AppendSection(builder, "Template/Filter Tags Mapped", TemplateFilterTagDetails);
        AppendSection(builder, "Chance Mod Mappings", ChanceModMappingDetails);
        AppendSection(builder, "Unsupported EffectFilter Fields", UnsupportedEffectFilterDetails);
        AppendSection(builder, "Class Table Files", ClassTableFileDetails);
        AppendSection(builder, "Missing Modifier Table References", MissingModifierTableReferenceDetails);
        AppendSection(builder, "Attack Vectors Mapped", AttackVectorDetails);
        AppendSection(builder, "Zero-Value Tag Carrier Effects Suppressed", ZeroValueTagCarrierDetails);
        AppendSection(builder, "Missing Scoped Powersets Repaired", MissingScopedPowersetRepairDetails);
        AppendSection(builder, "Scoped Power Identity Repairs", ScopedPowerSetIdentityRepairDetails);
        AppendSection(builder, "Aliased Powerset Identity Repairs", AliasedPowersetIdentityRepairDetails);
        AppendSection(builder, "Aliased Duplicate Powersets Removed", AliasedPowersetDuplicateRemovalDetails);
        AppendSection(builder, "Aliased Powerset Identity Collisions", AliasedPowersetIdentityCollisionDetails);
        AppendSection(builder, "Aliased Power Identity Repairs", AliasedPowerIdentityRepairDetails);
        AppendSection(builder, "Aliased Duplicate Powers Removed", AliasedPowerDuplicateRemovalDetails);
        AppendSection(builder, "Aliased Power Identity Collisions", AliasedPowerIdentityCollisionDetails);
        AppendSection(builder, "Malformed Power Name Repairs", MalformedPowerNameRepairDetails);
        AppendSection(builder, "Malformed Duplicate Powers Removed", MalformedPowerDuplicateRemovalDetails);
        AppendSection(builder, "Malformed Power Name Collisions", MalformedPowerNameCollisionDetails);
        AppendSection(builder, "Powersets Reordered By Power Level", PowersetPowerOrderDetails);
        AppendSection(builder, "Excluded Content Removed", ExcludedContentRemovalDetails);
        AppendSection(builder, "Import Integrity Audit", ImportIntegrityAuditDetails);
        AppendSection(builder, "New Orphan Powers Introduced", NewOrphanPowerDetails);
        AppendSection(builder, "Missing / Detached Scoped Omni Powers", OrphanedScopedOmniPowerDetails);
        AppendSection(builder, "Scoped Display Fallback Rejected", ScopedDisplayFallbackRejectedDetails);
        AppendSection(builder, "Accepted Canonical Scoped Power Replacements", AcceptedCanonicalScopedPowerReplacementDetails);
        AppendSection(builder, "Excluded Scoped Omni Powers", ExcludedScopedOmniPowerDetails);
        AppendSection(builder, "Manifest-Owned Scoped Omni Powers", ManifestOwnedScopedOmniPowerDetails);
        AppendSection(builder, "Powerset Identity Changes", PowersetIdentityChangeDetails);
        AppendSection(builder, "Staff Mastery Trace", StaffMasteryTraceDetails);
        AppendSection(builder, "Duplicate Composite Power Identities", DuplicateCompositePowerIdentityDetails);
        AppendSection(builder, "Epic Import Details", EpicImportDetails);
        AppendSection(builder, "Epic Powerset Identity Details", EpicPowersetIdentityDetails);
        AppendSection(builder, "Epic Powerset Prefix/Suffix Candidates", EpicPowersetPrefixSuffixCandidates);
        AppendSection(builder, "Epic Powerset Display Name Collisions", EpicPowersetDisplayNameCollisions);
        AppendSection(builder, "Epic Powersets With Zero Linked Powers", EpicPowersetsWithZeroLinkedPowersDetails);
        AppendSection(builder, "Epic Powers Missing After Import", EpicPowersMissingAfterImportDetails);
        AppendSection(builder, "Epic Powers With Missing Powerset", EpicPowersWithMissingPowersetDetails);
        AppendSection(builder, "Epic Powers With PowerSetID < 0", EpicPowersWithInvalidPowersetIdDetails);
        AppendSection(builder, "Epic Powers Not In Powerset Array", EpicPowersNotInPowersetArrayDetails);
        AppendSection(builder, "Epic Powers Hidden In DB Editor", EpicPowersHiddenInDbEditorDetails);
        AppendSection(builder, "Pet Import Details", PetImportDetails);
        AppendSection(builder, "Pet Powers Repaired Before Linking", PetPowersRepairedBeforeLinkingDetails);
        AppendSection(builder, "Pet Powers Missing After Import", PetPowersMissingAfterImportDetails);
        AppendSection(builder, "Pet Powers With Missing Powerset", PetPowersWithMissingPowersetDetails);
        AppendSection(builder, "Pet Powers With PowerSetID < 0", PetPowersWithInvalidPowersetIdDetails);
        AppendSection(builder, "Pet Powers Not In Powerset Array", PetPowersNotInPowersetArrayDetails);
        AppendSection(builder, "Pet Import Manifest", PetImportManifestDetails);
        AppendSection(builder, "Pet Source-Of-Truth Upsert", PetSourceOfTruthUpsertDetails);
        AppendSection(builder, "Pet Link Integrity Failures", PetLinkIntegrityFailureDetails);
        AppendSection(builder, "Pet Powersets With Zero Linked Powers", PetPowersetsWithZeroLinkedPowersDetails);
        AppendSection(builder, "Power Field Mapping Coverage", MappedPowerFieldDetails);
        AppendSection(builder, "Mapped With Fallback", PowerFieldFallbackDetails);
        AppendSection(builder, "Field Conflicts", PowerFieldConflictDetails);
        AppendSection(builder, "Known Charge-Capacity Extensions", KnownChargeCapacityDetailEntries);
        AppendSection(builder, "Known Deferred Power Fields", DeferredPowerFieldDetails);
        AppendCountSection(builder, "Ignored Power Field Owner Kinds", IgnoredPowerFieldOwnerKindCounts);
        AppendCountSection(builder, "Ignored Power Field Categories", IgnoredPowerFieldCategoryCounts);
        AppendCountSection(builder, "Ignored Power Fields By Name", IgnoredPowerFieldNameCounts);
        AppendSection(builder, "Ignored UI/Client/Server Power Field Samples", IgnoredPowerFieldDetails);
        AppendSection(builder, "Unknown Power Fields", UnknownPowerFieldDetails);
        AppendSection(builder, "Powerset Icon Audit", PoolPowersetIconAuditDetails);
        AppendSection(builder, "Powerset Icon Assignments", PoolIconAssignmentDetails);
        AppendSection(builder, "Missing Powerset Icon Assets", MissingPoolIconAssetDetails);
        AppendSection(builder, "Pool Icon Integrity", PoolIconIntegrityDetails);
        AppendSection(builder, "Pool Power Link Integrity", PoolPowerLinkIntegrityDetails);
        AppendSection(builder, "Sorcery Enflame Pickability Trace", SorceryEnflamePickabilityTraceDetails);
        AppendSection(builder, "Pool Requirement Evaluation Failures", PoolRequirementEvaluationFailureDetails);
        AppendSection(builder, "Pseudo-Pet Entities", PseudoPetEntityDetails);
        AppendSection(builder, "Pseudo-Pet Absorption Flags", PseudoPetAbsorptionFlagDetails);
        AppendSection(builder, "Pseudo-Pet Absorption Audit Skipped / Report-Only", PseudoPetAbsorptionAuditSkippedDetails);
        AppendSection(builder, "Pseudo-Pet Absorption Audit Failures", PseudoPetAbsorptionAuditDetails);
        AppendSection(builder, "PvX Target And Gating Audit", PvTargetGatingAuditDetails);
        AppendSection(builder, "PvX Target Mapping Mismatches", PvTargetMappingMismatchDetails);
        AppendSection(builder, "Enhancement Import", EnhancementImportDetails);
        AppendSection(builder, "Classic Enhancement Folding", ClassicEnhancementFoldingDetails);
        AppendSection(builder, "Classic Enhancement Presentation", ClassicEnhancementPresentationDetails);
        AppendSection(builder, "Enhancement Source Shape Validation", EnhancementSourceShapeDetails);
        AppendSection(builder, "Unresolved Enhancement Power Links", UnresolvedEnhancementPowerLinks);
        AppendSection(builder, "Enhancement Icon Decisions", EnhancementIconDetails);
        AppendSection(builder, "Enhancement Class Derivation", EnhancementClassDerivationDetails);
        AppendSection(builder, "Vector Template Semantic Overrides", VectorTemplateSemanticOverrideDetails);
        AppendSection(builder, "Helper Carriers Backfilled From Linked Bonuses", BoostHelperCarrierLinkedBonusDetails);
        AppendSection(builder, "Helper Carriers Resolved Locally", BoostHelperCarrierLocalResolutionDetails);
        AppendSection(builder, "Helper Carriers Left Unresolved", BoostHelperCarrierUnresolvedDetails);
        AppendSection(builder, "Residual Boosts_20 Damage Mappings", Boosts20DamageMappingDetails);
        AppendSection(builder, "Scoped Power Enhancement Legality", ScopedPowerEnhancementLegalityDetails);
        AppendSection(builder, "Scoped Power Enhancement Legality Unknown Labels", ScopedPowerEnhancementLegalityUnknownLabelDetails);
        AppendSection(builder, "Boosts And Set Bonus Legality Repair", BoostSetBonusPowerLegalityRepairDetails);
        AppendSection(builder, "Boosts And Set Bonus Import Audit", BoostSetBonusImportAuditDetails);
        AppendSection(builder, "Enhancement Reconciliation Audit", EnhancementReconciliationAuditDetails);
        AppendSection(builder, "Enhancement Reconciliation Conflicts", EnhancementReconciliationConflictDetails);
        AppendSection(builder, "Enhancement Naming Reconciliation", EnhancementNamingReconciliationDetails);
        AppendSection(builder, "Known Hidden/Stateful Effect Mappings", KnownHiddenStatefulEffectMappingDetails);
        AppendSection(builder, "Known Unsupported Effect Mappings", KnownUnsupportedEffectMappingDetails);
        AppendSection(builder, "Unknown Effect Mappings", UnknownEffectMappingDetails);
        AppendSection(builder, "Unknown Attribute Mappings", UnknownAttribMappingDetails);
        return builder.ToString();
    }

    public string ToPreviewMarkdown()
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Omni Safe Import Preview");
        builder.AppendLine();
        builder.AppendLine($"- Archetypes read/playable/retained/skipped: {ArchetypesRead:n0}/{PlayableArchetypesRead:n0}/{RetainedArchetypesRead:n0}/{SkippedArchetypesRead:n0}");
        builder.AppendLine($"- Retained/skipped class tables: {RetainedClassTableFiles:n0}/{SkippedClassTableFiles:n0}");
        builder.AppendLine($"- Powersets matched/created/updated: {PowersetsMatched:n0}/{PowersetsCreated:n0}/{PowersetsUpdated:n0}");
        builder.AppendLine($"- Powers matched/created/updated: {PowersMatched:n0}/{PowersCreated:n0}/{PowersUpdated:n0}");
        builder.AppendLine($"- Effects replaced: {EffectsReplaced:n0}");
        builder.AppendLine($"- Requirements updated/skipped unsupported: {RequirementsUpdated:n0}/{RequirementsSkippedUnsupported:n0}");
        builder.AppendLine($"- Redirect effects added: {RedirectEffectsAdded:n0}");
        builder.AppendLine($"- Excluded archetypes/powersets/powers/entities removed: {ExcludedArchetypesRemoved:n0}/{ExcludedClassPowersetsRemoved:n0}/{ExcludedClassPowersRemoved:n0}/{ExcludedClassEntitiesRemoved:n0}");
        builder.AppendLine($"- Scoped powers missing-detached/exact-retained/accepted replacements/display-fallback-rejected: {OrphanedScopedOmniPowers:n0}/{ExactScopedOmniPowersRetained:n0}/{AcceptedCanonicalScopedPowerReplacements:n0}/{ScopedDisplayFallbackRejected:n0}");
        builder.AppendLine($"- Scoped powers excluded/manifest-owned: {ExcludedScopedOmniPowers:n0}/{ManifestOwnedScopedOmniPowers:n0}");
        builder.AppendLine($"- Powers missing from Mids: {PowersMissingFromMids:n0}");
        builder.AppendLine($"- Known hidden/stateful effect mappings: {KnownHiddenStatefulEffectMappings:n0}");
        builder.AppendLine($"- Known unsupported effect mappings: {KnownUnsupportedEffectMappings:n0}");
        builder.AppendLine($"- Global/power-local chance mods mapped: {GlobalChanceModsMapped:n0}/{PowerLocalChanceModsMapped:n0}");
        builder.AppendLine($"- Salvage matched/created/updated: {SalvageMatched:n0}/{SalvageCreated:n0}/{SalvageUpdated:n0}");
        builder.AppendLine($"- Recipes matched/created/updated: {RecipesMatched:n0}/{RecipesCreated:n0}/{RecipesUpdated:n0}");
        builder.AppendLine($"- Enhancement sets matched/created/updated: {EnhancementSetsMatched:n0}/{EnhancementSetsCreated:n0}/{EnhancementSetsUpdated:n0}");
        builder.AppendLine($"- Enhancements matched/created/updated: {EnhancementsMatched:n0}/{EnhancementsCreated:n0}/{EnhancementsUpdated:n0}");
        builder.AppendLine($"- Known charge-capacity extensions: {KnownChargeCapacityExtensions:n0}");
        builder.AppendLine($"- Enhancement files/recipe files/logical recipes/recipe variants: {EnhancementFilesProcessed:n0}/{RecipeFilesProcessed:n0}/{LogicalRecipesProcessed:n0}/{RecipeLevelVariantsProcessed:n0}");
        builder.AppendLine($"- Classic enhancement variants/logical/folded: {ClassicEnhancementSourceVariantsDiscovered:n0}/{ClassicEnhancementLogicalRecords:n0}/{ClassicEnhancementVariantsFolded:n0}");
        builder.AppendLine($"- Classic enhancement editor rows expected: {ClassicEnhancementEditorRowsExpected:n0}");
        builder.AppendLine($"- Classic enhancement metadata warnings: {ClassicEnhancementMetadataWarnings:n0}");
        builder.AppendLine($"- Recipes excluded by policy: {RecipeRecordsExcludedByPolicy:n0}");
        builder.AppendLine($"- Structured boosts_allowed parsed: {StructuredBoostsAllowedParsedCount:n0}");
        builder.AppendLine($"- Enhancement class derivation effects/fallback/category-only/mismatch/unresolved: {EnhancementClassIdsDerivedFromEffects:n0}/{EnhancementClassIdsFallbackUsed:n0}/{EnhancementClassIdsCategoryOnly:n0}/{EnhancementClassIdWrapperMismatches:n0}/{EnhancementClassIdsUnresolved:n0}");
        builder.AppendLine($"- Vector defense/resistance template overrides: {VectorDefenseTemplateOverrides:n0}/{VectorResistanceTemplateOverrides:n0}");
        builder.AppendLine($"- Helper carriers linked/local/unresolved: {BoostHelperCarriersResolvedFromLinkedBonus:n0}/{BoostHelperCarriersResolvedLocally:n0}/{BoostHelperCarriersUnresolved:n0}");
        builder.AppendLine($"- Residual Boosts_20 damage mappings: {Boosts20DamageMappingsRemaining:n0}");
        builder.AppendLine($"- Scoped power legality rebuilt/changed/preserved/empty/unresolved/unknown-labels: {ScopedPowerEnhancementLegalityRebuilt:n0}/{ScopedPowerEnhancementLegalityChanged:n0}/{ScopedPowerEnhancementLegalityPreserved:n0}/{ScopedPowerEnhancementLegalityEmptyAfterRebuild:n0}/{ScopedPowerEnhancementLegalityUnresolvedAfterRebuild:n0}/{ScopedPowerEnhancementLegalityUnresolvedLabelCount:n0}");
        builder.AppendLine($"- Boost/Set_Bonus legality repair boosts inspected/rebuilt/preserved/unresolved: {BoostPowerLegalityRepairInspected:n0}/{BoostPowerLegalityRepairRebuilt:n0}/{BoostPowerLegalityRepairPreserved:n0}/{BoostPowerLegalityRepairUnresolved:n0}");
        builder.AppendLine($"- Boost/Set_Bonus legality repair set-bonus inspected/cleared/already-empty/changed: {SetBonusPowerLegalityRepairInspected:n0}/{SetBonusPowerLegalityRepairCleared:n0}/{SetBonusPowerLegalityRepairAlreadyEmpty:n0}/{BoostSetBonusPowerLegalityRepairChanged:n0}");
        builder.AppendLine($"- Shape fallbacks / malformed skips: {EnhancementShapeCompatibilityFallbacks:n0}/{EnhancementMalformedRecordsSkipped:n0}");
        builder.AppendLine($"- Enhancement boost links resolved/missing: {EnhancementBoostPowerLinksResolved:n0}/{EnhancementBoostPowerLinksMissing:n0}");
        builder.AppendLine($"- Set bonus links resolved/missing: {EnhancementSetBonusLinksResolved:n0}/{EnhancementSetBonusLinksMissing:n0}");
        builder.AppendLine($"- Recipe reward links resolved/missing: {RecipeRewardLinksResolved:n0}/{RecipeRewardLinksMissing:n0}");
        builder.AppendLine($"- Enhancement reconciliation updates/creates/conflicts: {EnhancementsUpdated:n0}/{EnhancementsCreated:n0}/{EnhancementAmbiguousMatches:n0}");
        builder.AppendLine($"- Enhancement set reconciliation updates/creates/conflicts: {EnhancementSetsUpdated:n0}/{EnhancementSetsCreated:n0}/{EnhancementSetsAmbiguousMatches:n0}");
        builder.AppendLine($"- Enhancement icons preserved/assigned/missing: {EnhancementIconsPreserved:n0}/{EnhancementIconsAssigned:n0}/{EnhancementIconsMissing:n0}");
        builder.AppendLine($"- Boosts powersets/powers in scope: {BoostPowersetsInScope:n0}/{BoostPowersInScope:n0} ({DescribeCoverageSource(BoostExplicitPowersetsInScope, BoostDerivedPowersetsInScope)})");
        builder.AppendLine($"- Set_Bonus powersets/powers in scope: {SetBonusPowersetsInScope:n0}/{SetBonusPowersInScope:n0} ({DescribeCoverageSource(SetBonusExplicitPowersetsInScope, SetBonusDerivedPowersetsInScope)})");
        builder.AppendLine($"- Boosts powers matched/created/updated: {BoostPowersMatched:n0}/{BoostPowersCreated:n0}/{BoostPowersUpdated:n0}");
        builder.AppendLine($"- Set_Bonus powers matched/created/updated: {SetBonusPowersMatched:n0}/{SetBonusPowersCreated:n0}/{SetBonusPowersUpdated:n0}");
        builder.AppendLine($"- Missing Boosts/Set_Bonus after import: {MissingBoostPowersAfterImport:n0}/{MissingSetBonusPowersAfterImport:n0}");
        builder.AppendLine($"- Strict Set_Bonus purge powers/powersets: {StrictSetBonusPowersPurged:n0}/{StrictSetBonusPowersetsPurged:n0}");
        builder.AppendLine($"- Strict Set_Bonus rebuilt counts Global/Set/PvP: {StrictSetBonusGlobalBonusAfterImport:n0}/{StrictSetBonusSetBonusAfterImport:n0}/{StrictSetBonusPvpSetBonusAfterImport:n0}");
        builder.AppendLine($"- Strict Set_Bonus cross-sibling mismatches/extras/missing: {StrictSetBonusCrossSiblingMismatchesAfterImport:n0}/{StrictSetBonusDbOnlyExtrasAfterImport:n0}/{StrictSetBonusMissingFullNamesAfterImport:n0}");
        builder.AppendLine($"- Naming repairs (boost/set-bonus/enh/set/recipe/salvage): {BoostPowerAliasRepairs:n0}/{SetBonusPowerAliasRepairs:n0}/{EnhancementAliasRepairs:n0}/{EnhancementSetAliasRepairs:n0}/{RecipeAliasRepairs:n0}/{SalvageAliasRepairs:n0}");
        builder.AppendLine($"- Pseudo-pet audit skipped/failures: {PseudoPetAbsorptionAuditSkipped:n0}/{PseudoPetAbsorptionAuditFailures:n0}");

        AppendPreviewWarnings(builder, new[]
        {
            FormatWarning("Unknown effect mappings", UnknownEffectMappings),
            FormatWarning("Unknown attribute mappings", UnknownAttribMappings),
            FormatWarning("Powers missing from Mids", PowersMissingFromMids),
            FormatWarning("Unresolved enhancement power links", UnresolvedEnhancementPowerLinks.Count),
            FormatWarning("Enhancement reconciliation conflicts", EnhancementAmbiguousMatches + EnhancementSetsAmbiguousMatches + RecipeAmbiguousMatches + SalvageAmbiguousMatches),
            FormatWarning("Missing boost powers after import", MissingBoostPowersAfterImport),
            FormatWarning("Missing set bonus powers after import", MissingSetBonusPowersAfterImport),
            FormatWarning("Helper carriers unresolved", BoostHelperCarriersUnresolved),
            FormatWarning("Residual Boosts_20 damage mappings", Boosts20DamageMappingsRemaining),
            FormatWarning("Enhancement class derivation unresolved", EnhancementClassIdsUnresolved),
            FormatWarning("Scoped power legality unresolved", ScopedPowerEnhancementLegalityUnresolvedAfterRebuild + ScopedPowerEnhancementLegalityEmptyAfterRebuild + ScopedPowerEnhancementLegalityUnresolvedLabelCount),
            FormatWarning("Boost/Set_Bonus legality repair unresolved", BoostPowerLegalityRepairUnresolved),
            FormatWarning("Classic enhancement metadata warnings", ClassicEnhancementMetadataWarnings),
            FormatWarning("Import integrity audit entries", ImportIntegrityAuditDetails.Count)
        });

        AppendPreviewSection(builder, "Import Integrity Audit", ImportIntegrityAuditDetails, 8);
        AppendPreviewSection(builder, "Scoped Display Fallback Rejected", ScopedDisplayFallbackRejectedDetails, 8);
        AppendPreviewSection(builder, "Classic Enhancement Folding", ClassicEnhancementFoldingDetails, 8);
        AppendPreviewSection(builder, "Classic Enhancement Presentation", ClassicEnhancementPresentationDetails, 8);
        AppendPreviewSection(builder, "Enhancement Source Shape Validation", EnhancementSourceShapeDetails, 8);
        AppendPreviewSection(builder, "Unresolved Enhancement Power Links", UnresolvedEnhancementPowerLinks, 8);
        AppendPreviewSection(builder, "Enhancement Class Derivation", EnhancementClassDerivationDetails, 8);
        AppendPreviewSection(builder, "Vector Template Semantic Overrides", VectorTemplateSemanticOverrideDetails, 8);
        AppendPreviewSection(builder, "Helper Carriers Backfilled From Linked Bonuses", BoostHelperCarrierLinkedBonusDetails, 8);
        AppendPreviewSection(builder, "Helper Carriers Resolved Locally", BoostHelperCarrierLocalResolutionDetails, 8);
        AppendPreviewSection(builder, "Helper Carriers Left Unresolved", BoostHelperCarrierUnresolvedDetails, 8);
        AppendPreviewSection(builder, "Residual Boosts_20 Damage Mappings", Boosts20DamageMappingDetails, 8);
        AppendPreviewSection(builder, "Scoped Power Enhancement Legality", ScopedPowerEnhancementLegalityDetails, 8);
        AppendPreviewSection(builder, "Boosts And Set Bonus Legality Repair", BoostSetBonusPowerLegalityRepairDetails, 8);
        AppendPreviewSection(builder, "Scoped Power Enhancement Legality Unknown Labels", ScopedPowerEnhancementLegalityUnknownLabelDetails, 8);
        AppendPreviewSection(builder, "Boosts / Set_Bonus Import Audit", BoostSetBonusImportAuditDetails, 8);
        AppendPreviewSection(builder, "Enhancement Reconciliation Audit", EnhancementReconciliationAuditDetails, 8);
        AppendPreviewSection(builder, "Enhancement Reconciliation Conflicts", EnhancementReconciliationConflictDetails, 8);
        AppendPreviewSection(builder, "Enhancement Naming Reconciliation", EnhancementNamingReconciliationDetails, 8);
        AppendPreviewSection(builder, "Chance Mod Mappings", ChanceModMappingDetails, 6);
        AppendPreviewSection(builder, "Known Hidden/Stateful Effect Mappings", KnownHiddenStatefulEffectMappingDetails, 6);
        AppendPreviewSection(builder, "Known Unsupported Effect Mappings", KnownUnsupportedEffectMappingDetails, 6);
        AppendPreviewSection(builder, "Unknown Effect Mappings", UnknownEffectMappingDetails, 6);
        AppendPreviewSection(builder, "Unknown Attribute Mappings", UnknownAttribMappingDetails, 6);

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

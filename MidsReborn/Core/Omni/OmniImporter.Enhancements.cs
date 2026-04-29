using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mids_Reborn.Core.Omni;

public sealed partial class OmniImporter
{
    private void ApplyEnhancementImportStage(IDatabase database, string exportRoot, OmniApplyResult applyResult)
    {
        var enhancementsRoot = Path.Combine(exportRoot, "enhancements");
        var enhancementSetsRoot = Path.Combine(exportRoot, "enhancement_sets");
        var recipesRoot = Path.Combine(exportRoot, "recipes");
        var legacyRecipesRoot = Path.Combine(exportRoot, "base_recipes");
        var salvageRoot = Path.Combine(exportRoot, "salvage");
        if (!Directory.Exists(enhancementsRoot) &&
            !Directory.Exists(enhancementSetsRoot) &&
            !Directory.Exists(recipesRoot) &&
            !Directory.Exists(legacyRecipesRoot) &&
            !Directory.Exists(salvageRoot))
        {
            return;
        }

        var normalizedData = LoadNormalizedEnhancementImportData(exportRoot);
        var metadata = CloneEnhancementImportMetadata(database.EnhancementImportMetadata);
        metadata.SourceRoot = NormalizeRoot(exportRoot);
        metadata.SourceVersionStamp = Path.GetFileName(NormalizeRoot(exportRoot)) ?? string.Empty;
        metadata.EnhancementDiversification = ReadJsonTokenIfExists(Path.Combine(exportRoot, "enhancement_diversification.json")) ?? metadata.EnhancementDiversification;
        metadata.EnhancementEffectiveness = ReadJsonTokenIfExists(Path.Combine(exportRoot, "enhancement_effectiveness.json")) ?? metadata.EnhancementEffectiveness;
        metadata.EnhancementExemplarScaling = ReadJsonTokenIfExists(Path.Combine(exportRoot, "enhancement_exemplar_scaling.json")) ?? metadata.EnhancementExemplarScaling;
        metadata.EnhancementSetGroups = ReadJsonTokenIfExists(Path.Combine(exportRoot, "enhancement_set_groups.json")) ?? metadata.EnhancementSetGroups;
        metadata.SetConversions = ReadJsonTokenIfExists(Path.Combine(exportRoot, "set_conversions.json")) ?? metadata.SetConversions;
        metadata.UnresolvedPowerLinks.Clear();
        metadata.UnresolvedIconAliases.Clear();
        metadata.UnconsumedBonusRequirements.Clear();
        metadata.DeferredPowerPolicies.Clear();
        metadata.ClassicEnhancementFoldAudit.Clear();
        metadata.CategoryOnlyBoostPowerFullNames.Clear();

        var powerLookup = BuildPowerLookup(database);
        var enhancementClassLookup = BuildEnhancementClassLookup(database);

        applyResult.EnhancementFilesProcessed = normalizedData.EnhancementSourceRecordsDiscovered;
        applyResult.RecipeFilesProcessed = normalizedData.Recipes.Count;
        applyResult.LogicalRecipesProcessed = normalizedData.Recipes.Count;
        applyResult.RecipeLevelVariantsProcessed = normalizedData.RecipeLevelVariantsDiscovered;
        applyResult.RecipeRecordsExcludedByPolicy = normalizedData.RecipeRecordsExcludedByPolicy;
        applyResult.EnhancementRecordsExcludedByPolicy = normalizedData.EnhancementRecordsExcludedByPolicy;
        applyResult.StructuredBoostsAllowedParsedCount = normalizedData.StructuredBoostsAllowedParsedCount;
        applyResult.EnhancementShapeCompatibilityFallbacks = normalizedData.ShapeCompatibilityFallbackCount;
        applyResult.ClassicEnhancementSourceVariantsDiscovered = normalizedData.ClassicEnhancementSourceVariantsDiscovered;
        applyResult.ClassicEnhancementLogicalRecords = normalizedData.ClassicEnhancementLogicalRecords;
        applyResult.ClassicEnhancementVariantsFolded = normalizedData.ClassicEnhancementVariantsFolded;
        applyResult.ClassicEnhancementEditorRowsExpected = normalizedData.ClassicEnhancementSourceVariantsDiscovered;
        applyResult.EnhancementMalformedRecordsSkipped =
            normalizedData.EnhancementMalformedRecordsSkipped +
            normalizedData.EnhancementSetMalformedRecordsSkipped +
            normalizedData.RecipeMalformedRecordsSkipped +
            normalizedData.SalvageMalformedRecordsSkipped;
        applyResult.AddLimited(applyResult.EnhancementSourceShapeDetails,
            $"Recipe source directory used: {normalizedData.RecipeSourceDirectoryName}.");
        applyResult.AddLimited(applyResult.EnhancementSourceShapeDetails,
            $"Enhancement source files/logical enhancement records/recipe files/logical recipes/recipe variants: {normalizedData.EnhancementSourceRecordsDiscovered}/{normalizedData.Enhancements.Count}/{normalizedData.Recipes.Count}/{normalizedData.Recipes.Count}/{normalizedData.RecipeLevelVariantsDiscovered}.");
        applyResult.AddLimited(applyResult.EnhancementSourceShapeDetails,
            $"Enhancement records excluded by policy: {normalizedData.EnhancementRecordsExcludedByPolicy}.");
        applyResult.AddLimited(applyResult.EnhancementImportDetails,
            $"Classic enhancement variants discovered/logical/folded: {normalizedData.ClassicEnhancementSourceVariantsDiscovered}/{normalizedData.ClassicEnhancementLogicalRecords}/{normalizedData.ClassicEnhancementVariantsFolded}.");
        applyResult.AddLimited(applyResult.ClassicEnhancementPresentationDetails,
            $"Synthetic classic editor rows expected from import metadata: {normalizedData.ClassicEnhancementSourceVariantsDiscovered}.");
        foreach (var detail in normalizedData.ClassicEnhancementFoldDetails)
        {
            applyResult.AddLimited(applyResult.EnhancementImportDetails, detail);
            applyResult.AddLimited(applyResult.ClassicEnhancementFoldingDetails, detail);
        }
        foreach (var detail in normalizedData.ShapeValidationDetails)
        {
            applyResult.AddLimited(applyResult.EnhancementSourceShapeDetails, detail);
        }

        foreach (var recipe in normalizedData.Recipes)
        {
            if (powerLookup.ContainsKey(recipe.EnhancementReward))
            {
                applyResult.RecipeRewardLinksResolved++;
            }
            else
            {
                applyResult.RecipeRewardLinksMissing++;
                applyResult.AddLimited(applyResult.UnresolvedEnhancementPowerLinks,
                    $"{recipe.Name}: recipe reward power '{recipe.EnhancementReward}' was not present after power import.");
            }
        }

        var reconciliationIndex = new EnhancementImportReconciliationIndex(database, metadata, powerLookup);
        ApplySalvageImport(database, normalizedData.Salvage, metadata, reconciliationIndex, applyResult);
        ApplyRecipeImport(database, normalizedData.Recipes, metadata, reconciliationIndex, applyResult);
        ApplyEnhancementSetImport(database, normalizedData.EnhancementSets, metadata, reconciliationIndex, applyResult);
        ApplyEnhancementImport(database, normalizedData.Enhancements, metadata, enhancementClassLookup, reconciliationIndex, applyResult);
        ValidateClassicEnhancementPresentationMetadata(normalizedData.Enhancements, metadata, applyResult);
        FinalizeEnhancementImport(database, normalizedData.EnhancementSets, metadata, reconciliationIndex, applyResult);

        database.EnhancementImportMetadata = metadata;
    }

    private List<T> LoadJsonDirectory<T>(string directory) where T : class
    {
        if (!Directory.Exists(directory))
        {
            return [];
        }

        var items = new List<T>();
        foreach (var file in Directory.EnumerateFiles(directory, "*.json", SearchOption.TopDirectoryOnly))
        {
            var item = ReadJson<T>(file);
            if (item != null)
            {
                items.Add(item);
            }
        }

        return items;
    }

    private List<OmniEnhancementDefinition> LoadEnhancementDefinitions(string enhancementsRoot)
    {
        if (!Directory.Exists(enhancementsRoot))
        {
            return [];
        }

        var items = new List<OmniEnhancementDefinition>();
        foreach (var categoryDir in Directory.EnumerateDirectories(enhancementsRoot))
        {
            foreach (var file in Directory.EnumerateFiles(categoryDir, "*.json", SearchOption.TopDirectoryOnly))
            {
                var item = ReadJson<OmniEnhancementDefinition>(file);
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        return items;
    }

    private Dictionary<string, OmniSetBonusSetDefinition> LoadSetBonusDefinitions(string exportRoot)
    {
        var path = Path.Combine(exportRoot, "set_bonuses.json");
        if (!File.Exists(path))
        {
            return new Dictionary<string, OmniSetBonusSetDefinition>(StringComparer.OrdinalIgnoreCase);
        }

        var items = ReadJson<List<OmniSetBonusSetDefinition>>(path);
        if (items == null)
        {
            return new Dictionary<string, OmniSetBonusSetDefinition>(StringComparer.OrdinalIgnoreCase);
        }

        return items
            .Where(item => !string.IsNullOrWhiteSpace(item.Name))
            .GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
    }

    private JToken? ReadJsonTokenIfExists(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        return ReadJson<JToken>(path);
    }

    private static EnhancementImportMetadata CloneEnhancementImportMetadata(EnhancementImportMetadata? source)
    {
        source ??= new EnhancementImportMetadata();
        return new EnhancementImportMetadata
        {
            SourceRoot = source.SourceRoot,
            SourceVersionStamp = source.SourceVersionStamp,
            EnhancementDiversification = source.EnhancementDiversification?.DeepClone(),
            EnhancementEffectiveness = source.EnhancementEffectiveness?.DeepClone(),
            EnhancementExemplarScaling = source.EnhancementExemplarScaling?.DeepClone(),
            EnhancementSetGroups = source.EnhancementSetGroups?.DeepClone(),
            SetConversions = source.SetConversions?.DeepClone(),
            EnhancementSourceCategories = new Dictionary<string, string>(source.EnhancementSourceCategories, StringComparer.OrdinalIgnoreCase),
            EnhancementSourceDisplayNames = new Dictionary<string, string>(source.EnhancementSourceDisplayNames, StringComparer.OrdinalIgnoreCase),
            EnhancementSourceFamilies = new Dictionary<string, string>(source.EnhancementSourceFamilies, StringComparer.OrdinalIgnoreCase),
            EnhancementSourceIcons = new Dictionary<string, string>(source.EnhancementSourceIcons, StringComparer.OrdinalIgnoreCase),
            EnhancementSetSourceKeys = new Dictionary<string, string>(source.EnhancementSetSourceKeys, StringComparer.OrdinalIgnoreCase),
            EnhancementSetSourceGroups = new Dictionary<string, string>(source.EnhancementSetSourceGroups, StringComparer.OrdinalIgnoreCase),
            EnhancementSetSourceIcons = new Dictionary<string, string>(source.EnhancementSetSourceIcons, StringComparer.OrdinalIgnoreCase),
            SpecialFamilyCrosswalk = new Dictionary<string, string>(source.SpecialFamilyCrosswalk, StringComparer.OrdinalIgnoreCase),
            BoostPowerAliasCrosswalk = new Dictionary<string, string>(source.BoostPowerAliasCrosswalk, StringComparer.OrdinalIgnoreCase),
            SetBonusPowerAliasCrosswalk = new Dictionary<string, string>(source.SetBonusPowerAliasCrosswalk, StringComparer.OrdinalIgnoreCase),
            EnhancementAliasCrosswalk = new Dictionary<string, string>(source.EnhancementAliasCrosswalk, StringComparer.OrdinalIgnoreCase),
            EnhancementSetAliasCrosswalk = new Dictionary<string, string>(source.EnhancementSetAliasCrosswalk, StringComparer.OrdinalIgnoreCase),
            RecipeAliasCrosswalk = new Dictionary<string, string>(source.RecipeAliasCrosswalk, StringComparer.OrdinalIgnoreCase),
            SalvageAliasCrosswalk = new Dictionary<string, string>(source.SalvageAliasCrosswalk, StringComparer.OrdinalIgnoreCase),
            ClassicEnhancementCanonicalBySourceKey = new Dictionary<string, string>(source.ClassicEnhancementCanonicalBySourceKey, StringComparer.OrdinalIgnoreCase),
            ClassicEnhancementCanonicalByName = new Dictionary<string, string>(source.ClassicEnhancementCanonicalByName, StringComparer.OrdinalIgnoreCase),
            ClassicEnhancementFoldKeys = new Dictionary<string, string>(source.ClassicEnhancementFoldKeys, StringComparer.OrdinalIgnoreCase),
            ClassicEnhancementSourcesByName = source.ClassicEnhancementSourcesByName.ToDictionary(
                pair => pair.Key,
                pair => new ClassicEnhancementSourceVariantMetadata
                {
                    SourceKey = pair.Value.SourceKey,
                    Name = pair.Value.Name,
                    Tier = pair.Value.Tier,
                    NormalizedTier = pair.Value.NormalizedTier,
                    DisplayName = pair.Value.DisplayName,
                    Icon = pair.Value.Icon,
                    Flavor = pair.Value.Flavor,
                    PrimaryOrigin = pair.Value.PrimaryOrigin,
                    SecondaryOrigin = pair.Value.SecondaryOrigin,
                    HasExplicitPrimaryOrigin = pair.Value.HasExplicitPrimaryOrigin,
                    CompatibleOrigins = pair.Value.CompatibleOrigins?.ToList() ?? []
                },
                StringComparer.OrdinalIgnoreCase),
            ClassicEnhancementSourceNamesByCanonicalName = source.ClassicEnhancementSourceNamesByCanonicalName.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToList(),
                StringComparer.OrdinalIgnoreCase),
            CategoryOnlyBoostPowerFullNames = source.CategoryOnlyBoostPowerFullNames.ToList(),
            UnresolvedPowerLinks = source.UnresolvedPowerLinks.ToList(),
            UnresolvedIconAliases = source.UnresolvedIconAliases.ToList(),
            UnconsumedBonusRequirements = source.UnconsumedBonusRequirements.ToList(),
            DeferredPowerPolicies = source.DeferredPowerPolicies.ToList(),
            ClassicEnhancementFoldAudit = source.ClassicEnhancementFoldAudit.ToList()
        };
    }

    private enum EnhancementReconciliationMatchKind
    {
        None = 0,
        Exact = 1,
        Alias = 2,
        LinkedPower = 3,
        Fallback = 4,
        Ambiguous = 5
    }

    private sealed class EntityReconciliationResult<T> where T : class
    {
        public EnhancementReconciliationMatchKind Kind { get; init; }
        public T? Item { get; init; }
        public string MatchedKey { get; init; } = string.Empty;
        public IReadOnlyList<string> CandidateKeys { get; init; } = Array.Empty<string>();
        public bool Found => Item != null && Kind != EnhancementReconciliationMatchKind.Ambiguous;
        public bool IsAmbiguous => Kind == EnhancementReconciliationMatchKind.Ambiguous;
    }

    private sealed class PowerReconciliationResult
    {
        public EnhancementReconciliationMatchKind Kind { get; init; }
        public int Index { get; init; } = -1;
        public IPower? Power { get; init; }
        public string MatchedFullName { get; init; } = string.Empty;
        public IReadOnlyList<string> CandidateKeys { get; init; } = Array.Empty<string>();
        public bool Found => Power != null && Index >= 0 && Kind != EnhancementReconciliationMatchKind.Ambiguous;
        public bool IsAmbiguous => Kind == EnhancementReconciliationMatchKind.Ambiguous;
    }

    private sealed class EnhancementImportReconciliationIndex
    {
        private readonly IDatabase _database;
        private readonly EnhancementImportMetadata _metadata;
        private readonly IReadOnlyDictionary<string, (int Index, IPower Power)> _powerLookup;
        private readonly List<Enhancement> _enhancements;
        private readonly List<EnhancementSet> _sets;
        private readonly List<Recipe> _recipes;
        private readonly List<Salvage> _salvage;
        private readonly Dictionary<string, List<Enhancement>> _enhancementsByCanonicalPower;
        private readonly Dictionary<string, List<Enhancement>> _enhancementsByNormalizedPower;
        private readonly Dictionary<string, List<Enhancement>> _enhancementsByClassicFoldKey;
        private readonly Dictionary<string, List<Enhancement>> _enhancementsByRecipe;
        private readonly Dictionary<string, List<Enhancement>> _enhancementsByTypeAndShortName;
        private readonly Dictionary<string, List<Enhancement>> _enhancementsBySetAndShortName;
        private readonly Dictionary<string, List<Enhancement>> _enhancementsByNormalizedName;
        private readonly Dictionary<string, List<EnhancementSet>> _setsByShortName;
        private readonly Dictionary<string, List<EnhancementSet>> _setsByDisplayName;
        private readonly Dictionary<string, List<EnhancementSet>> _setsByMemberSignature;
        private readonly Dictionary<string, List<EnhancementSet>> _setsByBonusSignature;
        private readonly Dictionary<string, List<EnhancementSet>> _setsByGroupSignature;
        private readonly Dictionary<string, List<Recipe>> _recipesByEnhancement;
        private readonly Dictionary<string, List<Recipe>> _recipesByNormalizedName;
        private readonly Dictionary<string, List<Salvage>> _salvageByNormalizedName;
        private readonly Dictionary<string, List<string>> _powersByNormalizedName;
        private readonly Dictionary<string, List<string>> _powersByDisplayName;
        private readonly Dictionary<string, List<string>> _powersByTokenizedIdentity;

        public EnhancementImportReconciliationIndex(
            IDatabase database,
            EnhancementImportMetadata metadata,
            IReadOnlyDictionary<string, (int Index, IPower Power)> powerLookup)
        {
            _database = database;
            _metadata = metadata;
            _powerLookup = powerLookup;
            _enhancements = (database.Enhancements ?? Array.Empty<IEnhancement>())
                .OfType<Enhancement>()
                .Where(static item => item != null)
                .ToList();
            _sets = (database.EnhancementSets ?? new EnhancementSetCollection())
                .Where(static item => item != null)
                .ToList();
            _recipes = (database.Recipes ?? Array.Empty<Recipe>())
                .Where(static item => item != null)
                .ToList();
            _salvage = (database.Salvage ?? Array.Empty<Salvage>())
                .Where(static item => item != null)
                .ToList();
            _enhancementsByCanonicalPower = GroupByList(_enhancements, item => CanonicalizeOmniFullName(item.GetPower()?.FullName ?? string.Empty));
            _enhancementsByNormalizedPower = GroupByList(_enhancements, item => NormalizeLookupKey(item.GetPower()?.FullName ?? string.Empty));
            _enhancementsByClassicFoldKey = GroupByListMany(
                _enhancements.Where(item => item.TypeID == Enums.eType.Normal),
                BuildExistingClassicEnhancementKeys);
            _enhancementsByRecipe = GroupByList(_enhancements, item => item.RecipeName);
            _enhancementsByTypeAndShortName = GroupByList(_enhancements, item => $"{(int)item.TypeID}|{NormalizeLookupKey(item.ShortName)}");
            _enhancementsBySetAndShortName = GroupByList(_enhancements, item => BuildEnhancementSetShortKey(item.UIDSet, item.ShortName));
            _enhancementsByNormalizedName = GroupByListMany(_enhancements, item => new[]
            {
                NormalizeLookupKey(item.UID),
                NormalizeLookupKey(item.Name),
                NormalizeLookupKey(item.ShortName)
            });
            _setsByShortName = GroupByListMany(_sets, item => new[]
            {
                item.Uid,
                item.ShortName
            });
            _setsByDisplayName = GroupByListMany(_sets, item => new[]
            {
                NormalizeLookupKey(item.DisplayName),
                NormalizeLookupKey(item.ShortName),
                NormalizeLookupKey(item.Uid)
            });
            _setsByMemberSignature = GroupByList(_sets, BuildExistingSetMemberSignature);
            _setsByBonusSignature = GroupByList(_sets, BuildExistingSetBonusSignature);
            _setsByGroupSignature = GroupByList(_sets, BuildExistingSetGroupSignature);
            _recipesByEnhancement = GroupByListMany(_recipes, item => new[]
            {
                item.InternalName,
                item.Enhancement
            });
            _recipesByNormalizedName = GroupByListMany(_recipes, item => new[]
            {
                NormalizeLookupKey(item.InternalName),
                NormalizeLookupKey(item.ExternalName),
                NormalizeLookupKey(item.Enhancement)
            });
            _salvageByNormalizedName = GroupByListMany(_salvage, item => new[]
            {
                item.InternalName,
                item.ExternalName
            });
            _powersByNormalizedName = GroupByPowerKeys(key => NormalizeLookupKey(key));
            _powersByDisplayName = GroupByPowerKeys(key => NormalizeLookupKey(_powerLookup[key].Power.DisplayName));
            _powersByTokenizedIdentity = GroupByPowerKeys(TokenizePowerIdentity);
        }

        public EntityReconciliationResult<Salvage> ResolveSalvage(NormalizedSalvageSource source)
        {
            var exact = _salvage.FirstOrDefault(item => string.Equals(item.InternalName, source.Name, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                return new EntityReconciliationResult<Salvage>
                {
                    Kind = EnhancementReconciliationMatchKind.Exact,
                    Item = exact,
                    MatchedKey = exact.InternalName
                };
            }

            var alias = ResolveAlias<Salvage>(_metadata.SalvageAliasCrosswalk, source.Name);
            if (alias.Found)
            {
                return alias;
            }

            var fallback = UniqueFromCandidates(
                TryGetCandidates(_salvageByNormalizedName, NormalizeLookupKey(source.Name)),
                TryGetCandidates(_salvageByNormalizedName, NormalizeLookupKey(source.DisplayName)));
            return FinalizeFallback(fallback, item => item.InternalName);
        }

        public EntityReconciliationResult<Recipe> ResolveRecipe(NormalizedRecipeSource source)
        {
            var alias = ResolveAlias<Recipe>(_metadata.RecipeAliasCrosswalk, source.Name);
            if (alias.Found)
            {
                return alias;
            }

            var exact = _recipes.FirstOrDefault(item =>
                string.Equals(item.InternalName, source.Name, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(item.InternalName, source.StorageKey, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(item.InternalName, source.SourceKey, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(item.Enhancement, source.EnhancementRewardUid, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                return new EntityReconciliationResult<Recipe>
                {
                    Kind = EnhancementReconciliationMatchKind.Exact,
                    Item = exact,
                    MatchedKey = exact.InternalName
                };
            }

            var fallback = UniqueFromCandidates(
                TryGetCandidates(_recipesByEnhancement, source.EnhancementRewardUid),
                TryGetCandidates(_recipesByNormalizedName, NormalizeLookupKey(source.Name)));
            return FinalizeFallback(fallback, item => item.InternalName);
        }

        public EntityReconciliationResult<EnhancementSet> ResolveEnhancementSet(
            NormalizedEnhancementSetSource source)
        {
            var exact = _sets.FirstOrDefault(item => string.Equals(item.Uid, source.Name, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                return new EntityReconciliationResult<EnhancementSet>
                {
                    Kind = EnhancementReconciliationMatchKind.Exact,
                    Item = exact,
                    MatchedKey = exact.Uid
                };
            }

            var alias = ResolveAlias<EnhancementSet>(_metadata.EnhancementSetAliasCrosswalk, source.Name);
            if (alias.Found)
            {
                return alias;
            }

            var shortName = UniqueFromCandidates(
                TryGetCandidates(_setsByShortName, source.Name),
                TryGetCandidates(_setsByShortName, source.DisplayName));
            if (shortName.Count == 1)
            {
                return new EntityReconciliationResult<EnhancementSet>
                {
                    Kind = EnhancementReconciliationMatchKind.Fallback,
                    Item = shortName[0],
                    MatchedKey = shortName[0].Uid
                };
            }

            if (shortName.Count > 1)
            {
                return Ambiguous(shortName, item => item.Uid);
            }

            var signatureCandidates = UniqueFromCandidates(
                TryGetCandidates(_setsByMemberSignature, BuildSourceSetMemberSignature(source)),
                TryGetCandidates(_setsByBonusSignature, BuildSourceSetBonusSignature(source.Bonuses)),
                TryGetCandidates(_setsByDisplayName, NormalizeLookupKey(source.DisplayName)),
                TryGetCandidates(_setsByDisplayName, NormalizeLookupKey(source.Name)),
                TryGetCandidates(_setsByGroupSignature, NormalizeLookupKey(source.GroupName)));
            return FinalizeFallback(signatureCandidates, item => item.Uid);
        }

        public EntityReconciliationResult<Enhancement> ResolveEnhancement(NormalizedEnhancementSource source)
        {
            var mappedType = MapEnhancementType(source);
            var exact = _enhancements.FirstOrDefault(item => string.Equals(item.UID, source.Name, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                return new EntityReconciliationResult<Enhancement>
                {
                    Kind = EnhancementReconciliationMatchKind.Exact,
                    Item = exact,
                    MatchedKey = exact.UID
                };
            }

            var alias = ResolveAlias<Enhancement>(_metadata.EnhancementAliasCrosswalk, source.Name);
            if (alias.Found)
            {
                return alias;
            }

            if (source.IsClassicOriginVariant && !string.IsNullOrWhiteSpace(source.ClassicFoldKey))
            {
                var classicCandidates = UniqueFromCandidates(TryGetCandidates(_enhancementsByClassicFoldKey, source.ClassicFoldKey));
                if (classicCandidates.Count == 1)
                {
                    return new EntityReconciliationResult<Enhancement>
                    {
                        Kind = EnhancementReconciliationMatchKind.Fallback,
                        Item = classicCandidates[0],
                        MatchedKey = classicCandidates[0].UID
                    };
                }

                if (classicCandidates.Count > 1)
                {
                    return Ambiguous(classicCandidates, item => item.UID);
                }
            }

            var canonicalPowerName = CanonicalizeOmniFullName(source.PowerFullName);
            var linkedCandidates = UniqueFromCandidates(
                TryGetCandidates(_enhancementsByCanonicalPower, canonicalPowerName),
                TryGetCandidates(_enhancementsByNormalizedPower, NormalizeLookupKey(canonicalPowerName)));
            if (linkedCandidates.Count == 1)
            {
                return new EntityReconciliationResult<Enhancement>
                {
                    Kind = EnhancementReconciliationMatchKind.LinkedPower,
                    Item = linkedCandidates[0],
                    MatchedKey = linkedCandidates[0].UID
                };
            }

            if (linkedCandidates.Count > 1)
            {
                return Ambiguous(linkedCandidates, item => item.UID);
            }

            var recipeCandidates = FindEnhancementCandidatesByRecipe(source);
            if (recipeCandidates.Count == 1)
            {
                return new EntityReconciliationResult<Enhancement>
                {
                    Kind = EnhancementReconciliationMatchKind.Fallback,
                    Item = recipeCandidates[0],
                    MatchedKey = recipeCandidates[0].UID
                };
            }

            if (recipeCandidates.Count > 1)
            {
                return Ambiguous(recipeCandidates, item => item.UID);
            }

            var setShortKey = BuildEnhancementSetShortKey(ResolveSetAlias(source.EnhancementSetName), source.Name);
            var fallbackCandidates = UniqueFromCandidates(
                TryGetCandidates(_enhancementsBySetAndShortName, setShortKey),
                TryGetCandidates(_enhancementsByTypeAndShortName, $"{(int)mappedType}|{NormalizeLookupKey(source.Name)}"),
                TryGetCandidates(_enhancementsByNormalizedName, NormalizeLookupKey(source.Name)),
                TryGetCandidates(_enhancementsByNormalizedName, NormalizeLookupKey(source.DisplayName)),
                LegacyEnhancementCandidates(source, mappedType));
            return FinalizeFallback(fallbackCandidates, item => item.UID);
        }

        public PowerReconciliationResult ResolvePower(string sourcePowerFullName, bool setBonusPower)
        {
            var canonical = CanonicalizeOmniFullName(sourcePowerFullName);
            if (_powerLookup.TryGetValue(canonical, out var exact))
            {
                return new PowerReconciliationResult
                {
                    Kind = EnhancementReconciliationMatchKind.Exact,
                    Index = exact.Index,
                    Power = exact.Power,
                    MatchedFullName = canonical
                };
            }

            var aliasLookup = setBonusPower ? _metadata.SetBonusPowerAliasCrosswalk : _metadata.BoostPowerAliasCrosswalk;
            if (aliasLookup.TryGetValue(canonical, out var aliasFullName) && _powerLookup.TryGetValue(aliasFullName, out var alias))
            {
                return new PowerReconciliationResult
                {
                    Kind = EnhancementReconciliationMatchKind.Alias,
                    Index = alias.Index,
                    Power = alias.Power,
                    MatchedFullName = aliasFullName
                };
            }

            var fallbackKeys = UniqueKeys(
                TryGetPowerCandidates(_powersByNormalizedName, NormalizeLookupKey(canonical)),
                TryGetPowerCandidates(_powersByDisplayName, NormalizeLookupKey(canonical)),
                TryGetPowerCandidates(_powersByTokenizedIdentity, TokenizePowerIdentity(canonical)));

            if (fallbackKeys.Count == 1 && _powerLookup.TryGetValue(fallbackKeys[0], out var fallback))
            {
                return new PowerReconciliationResult
                {
                    Kind = EnhancementReconciliationMatchKind.Fallback,
                    Index = fallback.Index,
                    Power = fallback.Power,
                    MatchedFullName = fallbackKeys[0]
                };
            }

            if (fallbackKeys.Count > 1)
            {
                return new PowerReconciliationResult
                {
                    Kind = EnhancementReconciliationMatchKind.Ambiguous,
                    CandidateKeys = fallbackKeys
                };
            }

            return new PowerReconciliationResult
            {
                Kind = EnhancementReconciliationMatchKind.None
            };
        }

        public EnhancementCreateInvestigationResult InvestigateUnmatchedEnhancement(NormalizedEnhancementSource source)
        {
            if (source.IsClassicOriginVariant && !string.IsNullOrWhiteSpace(source.ClassicFoldKey))
            {
                var classicCandidates = UniqueFromCandidates(TryGetCandidates(_enhancementsByClassicFoldKey, source.ClassicFoldKey));
                if (classicCandidates.Count == 1)
                {
                    return new EnhancementCreateInvestigationResult
                    {
                        Kind = EnhancementCreateInvestigationKind.ShouldHaveMatchedBySetMembership,
                        Anchor = source.ClassicFoldKey,
                        CandidateKeys = [classicCandidates[0].UID]
                    };
                }

                if (classicCandidates.Count > 1)
                {
                    return new EnhancementCreateInvestigationResult
                    {
                        Kind = EnhancementCreateInvestigationKind.Ambiguous,
                        Anchor = source.ClassicFoldKey,
                        CandidateKeys = classicCandidates.Select(item => item.UID).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                    };
                }
            }

            var linkedCandidates = UniqueFromCandidates(
                TryGetCandidates(_enhancementsByCanonicalPower, CanonicalizeOmniFullName(source.PowerFullName)),
                TryGetCandidates(_enhancementsByNormalizedPower, NormalizeLookupKey(CanonicalizeOmniFullName(source.PowerFullName))));
            if (linkedCandidates.Count == 1)
            {
                return new EnhancementCreateInvestigationResult
                {
                    Kind = EnhancementCreateInvestigationKind.ShouldHaveMatchedByBoostPower,
                    Anchor = CanonicalizeOmniFullName(source.PowerFullName),
                    CandidateKeys = [linkedCandidates[0].UID]
                };
            }

            if (linkedCandidates.Count > 1)
            {
                return new EnhancementCreateInvestigationResult
                {
                    Kind = EnhancementCreateInvestigationKind.Ambiguous,
                    Anchor = CanonicalizeOmniFullName(source.PowerFullName),
                    CandidateKeys = linkedCandidates.Select(item => item.UID).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                };
            }

            var recipeCandidates = FindEnhancementCandidatesByRecipe(source);
            if (recipeCandidates.Count == 1)
            {
                return new EnhancementCreateInvestigationResult
                {
                    Kind = EnhancementCreateInvestigationKind.ShouldHaveMatchedByRecipe,
                    Anchor = FirstNonEmpty(source.RecipeCanonicalId, source.RecipeStorageKey, source.RecipeKey),
                    CandidateKeys = [recipeCandidates[0].UID]
                };
            }

            if (recipeCandidates.Count > 1)
            {
                return new EnhancementCreateInvestigationResult
                {
                    Kind = EnhancementCreateInvestigationKind.Ambiguous,
                    Anchor = FirstNonEmpty(source.RecipeCanonicalId, source.RecipeStorageKey, source.RecipeKey),
                    CandidateKeys = recipeCandidates.Select(item => item.UID).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                };
            }

            var setCandidates = FindEnhancementCandidatesBySetMembership(source);
            if (setCandidates.Count == 1)
            {
                return new EnhancementCreateInvestigationResult
                {
                    Kind = EnhancementCreateInvestigationKind.ShouldHaveMatchedBySetMembership,
                    Anchor = FirstNonEmpty(source.EnhancementSetCanonicalId, source.EnhancementSetStorageKey, source.EnhancementSetName),
                    CandidateKeys = [setCandidates[0].UID]
                };
            }

            if (setCandidates.Count > 1)
            {
                return new EnhancementCreateInvestigationResult
                {
                    Kind = EnhancementCreateInvestigationKind.Ambiguous,
                    Anchor = FirstNonEmpty(source.EnhancementSetCanonicalId, source.EnhancementSetStorageKey, source.EnhancementSetName),
                    CandidateKeys = setCandidates.Select(item => item.UID).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                };
            }

            var looseCandidates = UniqueFromCandidates(
                TryGetCandidates(_enhancementsByTypeAndShortName, $"{(int)MapEnhancementType(source)}|{NormalizeLookupKey(source.Name)}"),
                TryGetCandidates(_enhancementsByNormalizedName, NormalizeLookupKey(source.Name)),
                TryGetCandidates(_enhancementsByNormalizedName, NormalizeLookupKey(source.DisplayName)),
                LegacyEnhancementCandidates(source, MapEnhancementType(source)));

            if (looseCandidates.Count > 1)
            {
                return new EnhancementCreateInvestigationResult
                {
                    Kind = EnhancementCreateInvestigationKind.Ambiguous,
                    Anchor = "normalized legacy fallback",
                    CandidateKeys = looseCandidates.Select(item => item.UID).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                };
            }

            if (looseCandidates.Count == 1)
            {
                return new EnhancementCreateInvestigationResult
                {
                    Kind = EnhancementCreateInvestigationKind.NoPlausibleExistingMatch,
                    Anchor = "normalized legacy fallback",
                    CandidateKeys = [looseCandidates[0].UID]
                };
            }

            return new EnhancementCreateInvestigationResult
            {
                Kind = HasAnyIdentity(source.PowerFullName, source.RecipeKey, source.RecipeCanonicalId, source.RecipeStorageKey, source.EnhancementSetName, source.EnhancementSetCanonicalId, source.EnhancementSetStorageKey)
                    ? EnhancementCreateInvestigationKind.LikelyNew
                    : EnhancementCreateInvestigationKind.NoPlausibleExistingMatch
            };
        }

        public RecipeCreateInvestigationResult InvestigateUnmatchedRecipe(NormalizedRecipeSource source)
        {
            var rewardCandidates = FindRecipeCandidatesByReward(source);
            if (rewardCandidates.Count == 1)
            {
                return new RecipeCreateInvestigationResult
                {
                    Kind = RecipeCreateInvestigationKind.ShouldHaveMatchedByRewardIdentity,
                    Anchor = FirstNonEmpty(source.EnhancementRewardUid, source.EnhancementReward),
                    CandidateKeys = [rewardCandidates[0].InternalName]
                };
            }

            if (rewardCandidates.Count > 1)
            {
                return new RecipeCreateInvestigationResult
                {
                    Kind = RecipeCreateInvestigationKind.Ambiguous,
                    Anchor = FirstNonEmpty(source.EnhancementRewardUid, source.EnhancementReward),
                    CandidateKeys = rewardCandidates.Select(item => item.InternalName).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                };
            }

            var canonicalCandidates = FindRecipeCandidatesByCanonicalIdentity(source);
            if (canonicalCandidates.Count == 1)
            {
                return new RecipeCreateInvestigationResult
                {
                    Kind = RecipeCreateInvestigationKind.ShouldHaveMatchedByCanonicalStorageIdentity,
                    Anchor = FirstNonEmpty(source.CanonicalId, source.StorageKey, source.SourceKey),
                    CandidateKeys = [canonicalCandidates[0].InternalName]
                };
            }

            if (canonicalCandidates.Count > 1)
            {
                return new RecipeCreateInvestigationResult
                {
                    Kind = RecipeCreateInvestigationKind.Ambiguous,
                    Anchor = FirstNonEmpty(source.CanonicalId, source.StorageKey, source.SourceKey),
                    CandidateKeys = canonicalCandidates.Select(item => item.InternalName).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                };
            }

            var looseCandidates = UniqueFromCandidates(
                TryGetCandidates(_recipesByNormalizedName, NormalizeLookupKey(source.Name)),
                TryGetCandidates(_recipesByNormalizedName, NormalizeLookupKey(source.DisplayName)));

            if (looseCandidates.Count > 1)
            {
                return new RecipeCreateInvestigationResult
                {
                    Kind = RecipeCreateInvestigationKind.Ambiguous,
                    Anchor = "normalized name fallback",
                    CandidateKeys = looseCandidates.Select(item => item.InternalName).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                };
            }

            if (looseCandidates.Count == 1)
            {
                return new RecipeCreateInvestigationResult
                {
                    Kind = RecipeCreateInvestigationKind.NoPlausibleExistingMatch,
                    Anchor = "normalized name fallback",
                    CandidateKeys = [looseCandidates[0].InternalName]
                };
            }

            return new RecipeCreateInvestigationResult
            {
                Kind = HasAnyIdentity(source.EnhancementRewardUid, source.EnhancementReward, source.CanonicalId, source.StorageKey, source.SourceKey)
                    ? RecipeCreateInvestigationKind.LikelyNew
                    : RecipeCreateInvestigationKind.NoPlausibleExistingMatch
            };
        }

        private EntityReconciliationResult<T> ResolveAlias<T>(Dictionary<string, string> crosswalk, string sourceKey) where T : class
        {
            if (!crosswalk.TryGetValue(sourceKey, out var aliasedKey))
            {
                return new EntityReconciliationResult<T>();
            }

            object? item = typeof(T) switch
            {
                var type when type == typeof(Salvage) => _salvage.FirstOrDefault(entry => string.Equals(entry.InternalName, aliasedKey, StringComparison.OrdinalIgnoreCase)),
                var type when type == typeof(Recipe) => _recipes.FirstOrDefault(entry => string.Equals(entry.InternalName, aliasedKey, StringComparison.OrdinalIgnoreCase)),
                var type when type == typeof(EnhancementSet) => _sets.FirstOrDefault(entry => string.Equals(entry.Uid, aliasedKey, StringComparison.OrdinalIgnoreCase)),
                var type when type == typeof(Enhancement) => _enhancements.FirstOrDefault(entry => string.Equals(entry.UID, aliasedKey, StringComparison.OrdinalIgnoreCase)),
                _ => null
            };

            return item is T typed
                ? new EntityReconciliationResult<T>
                {
                    Kind = EnhancementReconciliationMatchKind.Alias,
                    Item = typed,
                    MatchedKey = aliasedKey
                }
                : new EntityReconciliationResult<T>();
        }

        private List<Enhancement> LegacyEnhancementCandidates(NormalizedEnhancementSource source, Enums.eType mappedType)
        {
            var normalizedSource = NormalizeLookupKey(source.Name);
            var normalizedDisplay = NormalizeLookupKey(source.DisplayName);
            return _enhancements
                .Where(item =>
                    item.TypeID == mappedType &&
                    (NormalizeLookupKey(item.UID).Contains(normalizedSource, StringComparison.OrdinalIgnoreCase) ||
                     NormalizeLookupKey(item.ShortName).Contains(normalizedSource, StringComparison.OrdinalIgnoreCase) ||
                     NormalizeLookupKey(item.Name) == normalizedDisplay))
                .Distinct()
                .ToList();
        }

        private static IEnumerable<string> BuildExistingClassicEnhancementKeys(Enhancement enhancement)
        {
            var candidates = new[]
            {
                enhancement.UID,
                enhancement.Name,
                enhancement.ShortName
            };

            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var candidate in candidates)
            {
                var stripped = TryStripClassicOriginPrefix(candidate);
                var normalized = NormalizeLookupKey(stripped ?? candidate);
                if (!string.IsNullOrWhiteSpace(normalized))
                {
                    keys.Add(normalized);
                }
            }

            return keys;
        }

        private string ResolveSetAlias(string? setUid)
        {
            if (string.IsNullOrWhiteSpace(setUid))
            {
                return string.Empty;
            }

            return _metadata.EnhancementSetAliasCrosswalk.TryGetValue(setUid, out var aliased) ? aliased : setUid;
        }

        private List<Enhancement> FindEnhancementCandidatesByRecipe(NormalizedEnhancementSource source)
        {
            return UniqueFromCandidates(
                TryGetCandidates(_enhancementsByRecipe, source.RecipeKey),
                TryGetCandidates(_enhancementsByRecipe, source.RecipeCanonicalId),
                TryGetCandidates(_enhancementsByRecipe, source.RecipeStorageKey));
        }

        private List<Enhancement> FindEnhancementCandidatesBySetMembership(NormalizedEnhancementSource source)
        {
            var normalizedSetKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in new[]
                     {
                         source.EnhancementSetName,
                         source.EnhancementSetCanonicalId,
                         source.EnhancementSetStorageKey,
                         ResolveSetAlias(source.EnhancementSetName),
                         ResolveSetAlias(source.EnhancementSetCanonicalId),
                         ResolveSetAlias(source.EnhancementSetStorageKey)
                     })
            {
                var normalized = NormalizeLookupKey(key);
                if (!string.IsNullOrWhiteSpace(normalized))
                {
                    normalizedSetKeys.Add(normalized);
                }
            }

            var normalizedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in new[] { source.Name, source.DisplayName, source.StorageKey, source.CanonicalId })
            {
                var normalized = NormalizeLookupKey(key);
                if (!string.IsNullOrWhiteSpace(normalized))
                {
                    normalizedNames.Add(normalized);
                }
            }

            return _enhancements
                .Where(item => normalizedSetKeys.Contains(NormalizeLookupKey(item.UIDSet)))
                .Where(item =>
                    normalizedNames.Contains(NormalizeLookupKey(item.UID)) ||
                    normalizedNames.Contains(NormalizeLookupKey(item.ShortName)) ||
                    normalizedNames.Contains(NormalizeLookupKey(item.Name)))
                .Distinct()
                .ToList();
        }

        private List<Recipe> FindRecipeCandidatesByReward(NormalizedRecipeSource source)
        {
            return UniqueFromCandidates(
                TryGetCandidates(_recipesByEnhancement, source.EnhancementRewardUid),
                TryGetCandidates(_recipesByEnhancement, source.EnhancementReward));
        }

        private List<Recipe> FindRecipeCandidatesByCanonicalIdentity(NormalizedRecipeSource source)
        {
            return _recipes
                .Where(item =>
                    string.Equals(item.InternalName, source.CanonicalId, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(item.InternalName, source.StorageKey, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(item.InternalName, source.SourceKey, StringComparison.OrdinalIgnoreCase) ||
                    NormalizeLookupKey(item.InternalName) == NormalizeLookupKey(source.CanonicalId) ||
                    NormalizeLookupKey(item.InternalName) == NormalizeLookupKey(source.StorageKey) ||
                    NormalizeLookupKey(item.InternalName) == NormalizeLookupKey(source.SourceKey))
                .Distinct()
                .ToList();
        }

    private static bool HasAnyIdentity(params string[] values)
    {
        return values.Any(value => !string.IsNullOrWhiteSpace(value));
    }

    private static string FirstNonEmpty(params string[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
    }

        private static EntityReconciliationResult<T> FinalizeFallback<T>(List<T> candidates, Func<T, string> keySelector) where T : class
        {
            if (candidates.Count == 1)
            {
                return new EntityReconciliationResult<T>
                {
                    Kind = EnhancementReconciliationMatchKind.Fallback,
                    Item = candidates[0],
                    MatchedKey = keySelector(candidates[0])
                };
            }

            return candidates.Count > 1
                ? Ambiguous(candidates, keySelector)
                : new EntityReconciliationResult<T>();
        }

        private static EntityReconciliationResult<T> Ambiguous<T>(IEnumerable<T> candidates, Func<T, string> keySelector) where T : class
        {
            return new EntityReconciliationResult<T>
            {
                Kind = EnhancementReconciliationMatchKind.Ambiguous,
                CandidateKeys = candidates.Select(keySelector).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
            };
        }

        private static Dictionary<string, List<T>> GroupByList<T>(IEnumerable<T> items, Func<T, string> keySelector) where T : class
        {
            return items
                .Where(item => item != null)
                .Select(item => new { item, key = keySelector(item) })
                .Where(item => !string.IsNullOrWhiteSpace(item.key))
                .GroupBy(item => item.key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Select(entry => entry.item).Distinct().ToList(), StringComparer.OrdinalIgnoreCase);
        }

        private static Dictionary<string, List<T>> GroupByListMany<T>(IEnumerable<T> items, Func<T, IEnumerable<string>> keySelector) where T : class
        {
            return items
                .Where(item => item != null)
                .SelectMany(item => keySelector(item)
                    .Where(key => !string.IsNullOrWhiteSpace(key))
                    .Select(key => new { item, key }))
                .GroupBy(item => item.key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Select(entry => entry.item).Distinct().ToList(), StringComparer.OrdinalIgnoreCase);
        }

        private Dictionary<string, List<string>> GroupByPowerKeys(Func<string, string> keySelector)
        {
            return _powerLookup.Keys
                .Select(key => new { key, normalized = keySelector(key) })
                .Where(item => !string.IsNullOrWhiteSpace(item.normalized))
                .GroupBy(item => item.normalized, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Select(item => item.key).Distinct(StringComparer.OrdinalIgnoreCase).ToList(), StringComparer.OrdinalIgnoreCase);
        }

        private static List<T> TryGetCandidates<T>(IReadOnlyDictionary<string, List<T>> lookup, string key) where T : class
        {
            return string.IsNullOrWhiteSpace(key) || !lookup.TryGetValue(key, out var candidates)
                ? []
                : candidates.Where(candidate => candidate != null).Distinct().ToList();
        }

        private static T? TryGetUnique<T>(IReadOnlyDictionary<string, List<T>> lookup, string key) where T : class
        {
            var candidates = TryGetCandidates(lookup, key);
            return candidates.Count == 1 ? candidates[0] : null;
        }

        private static List<string> TryGetPowerCandidates(IReadOnlyDictionary<string, List<string>> lookup, string key)
        {
            return string.IsNullOrWhiteSpace(key) || !lookup.TryGetValue(key, out var candidates)
                ? []
                : candidates.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static List<T> UniqueFromCandidates<T>(params IEnumerable<T>[] groups) where T : class
        {
            return groups
                .Where(group => group != null)
                .SelectMany(group => group)
                .Where(item => item != null)
                .Distinct()
                .ToList();
        }

        private static List<string> UniqueKeys(params IEnumerable<string>[] groups)
        {
            return groups
                .Where(group => group != null)
                .SelectMany(group => group)
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string BuildEnhancementSetShortKey(string? setUid, string? shortName)
        {
            return $"{NormalizeLookupKey(setUid ?? string.Empty)}|{NormalizeLookupKey(shortName ?? string.Empty)}";
        }

        private string BuildExistingSetMemberSignature(EnhancementSet set)
        {
            var values = set.Enhancements
                .Where(index => index >= 0 && index < _database.Enhancements.Length)
                .Select(index => _database.Enhancements[index])
                .Where(item => item != null)
                .SelectMany(item => new[]
                {
                    NormalizeLookupKey(item!.UID),
                    NormalizeLookupKey(item.ShortName)
                })
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase);
            return string.Join("|", values);
        }

        private static string BuildSourceSetMemberSignature(NormalizedEnhancementSetSource source)
        {
            return string.Join("|", source.Members
                .Concat(source.AttunedMembers)
                .Concat(source.SuperiorAttunedMembers)
                .Select(member => NormalizeLookupKey(member.Name))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase));
        }

        private static string BuildSourceSetBonusSignature(IEnumerable<NormalizedSetBonusEntry> bonuses)
        {
            return string.Join("|", bonuses
                .SelectMany(GetSetBonusPowerNames)
                .Select(CanonicalizeOmniFullName)
                .Select(NormalizeLookupKey)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase));
        }

        private static string BuildExistingSetBonusSignature(EnhancementSet set)
        {
            return string.Join("|", set.Bonus
                .Concat(set.SpecialBonus ?? Array.Empty<EnhancementSet.BonusItem>())
                .Where(item => item.Name != null)
                .SelectMany(item => item.Name)
                .Select(CanonicalizeOmniFullName)
                .Select(NormalizeLookupKey)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase));
        }

        private string BuildExistingSetGroupSignature(EnhancementSet set)
        {
            if (set.SetType < 0 || _database.SetTypes == null || set.SetType >= _database.SetTypes.Count)
            {
                return string.Empty;
            }

            var type = _database.SetTypes[set.SetType];
            return NormalizeLookupKey(type.Name);
        }
    }

    private enum EnhancementCreateInvestigationKind
    {
        LikelyNew,
        ShouldHaveMatchedByBoostPower,
        ShouldHaveMatchedByRecipe,
        ShouldHaveMatchedBySetMembership,
        Ambiguous,
        NoPlausibleExistingMatch
    }

    private sealed class EnhancementCreateInvestigationResult
    {
        public EnhancementCreateInvestigationKind Kind { get; set; }
        public string Anchor { get; set; } = string.Empty;
        public string[] CandidateKeys { get; set; } = [];
    }

    private enum RecipeCreateInvestigationKind
    {
        LikelyNew,
        ShouldHaveMatchedByRewardIdentity,
        ShouldHaveMatchedByCanonicalStorageIdentity,
        Ambiguous,
        NoPlausibleExistingMatch
    }

    private sealed class RecipeCreateInvestigationResult
    {
        public RecipeCreateInvestigationKind Kind { get; set; }
        public string Anchor { get; set; } = string.Empty;
        public string[] CandidateKeys { get; set; } = [];
    }

    private static string TokenizePowerIdentity(string fullName)
    {
        var canonical = CanonicalizeOmniFullName(fullName);
        var parts = canonical.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var group = NormalizeLookupKey(parts.ElementAtOrDefault(0) ?? string.Empty);
        var set = NormalizeLookupKey(parts.Length > 2 ? parts[1] : string.Empty);
        var power = NormalizeLookupKey(parts.LastOrDefault() ?? string.Empty);
        return $"{group}|{set}|{power}";
    }

    private static Dictionary<string, (int Index, IPower Power)> BuildPowerLookup(IDatabase database)
    {
        return (database.Power ?? [])
            .Select((power, index) => new { power, index })
            .Where(item => item.power != null && !string.IsNullOrWhiteSpace(item.power.FullName))
            .GroupBy(item => CanonicalizeOmniFullName(item.power!.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => (group.First().index, group.First().power!),
                StringComparer.OrdinalIgnoreCase);
    }

    private static Dictionary<string, int> BuildEnhancementClassLookup(IDatabase database)
    {
        var lookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var enhancementClasses = database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>();
        for (var index = 0; index < enhancementClasses.Length; index++)
        {
            var enhClass = enhancementClasses[index];
            AddLookup(lookup, enhClass.Name, index);
            AddLookup(lookup, enhClass.ShortName, index);
            AddLookup(lookup, enhClass.ClassID, index);
        }

        return lookup;
    }

    private static void AddLookup(IDictionary<string, int> lookup, string value, int id)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var normalized = NormalizeLookupKey(value);
        if (!lookup.ContainsKey(normalized))
        {
            lookup[normalized] = id;
        }
    }

    private static string NormalizeLookupKey(string value)
    {
        return Regex.Replace(value ?? string.Empty, @"[^A-Za-z0-9]+", string.Empty).Trim().ToLowerInvariant();
    }

    private static bool TryResolveExistingSalvage(
        IReadOnlyCollection<Salvage> salvageItems,
        string sourceKey,
        string displayName,
        out Salvage? salvage,
        out string matchedKey)
    {
        salvage = salvageItems.FirstOrDefault(item => string.Equals(item.InternalName, sourceKey, StringComparison.OrdinalIgnoreCase));
        if (salvage != null)
        {
            matchedKey = salvage.InternalName;
            return true;
        }

        var normalizedCandidates = salvageItems
            .Where(item => item != null)
            .Where(item =>
                NormalizeLookupKey(item.InternalName) == NormalizeLookupKey(sourceKey) ||
                NormalizeLookupKey(item.ExternalName) == NormalizeLookupKey(displayName))
            .ToList();

        if (normalizedCandidates.Count == 1)
        {
            salvage = normalizedCandidates[0];
            matchedKey = salvage.InternalName;
            return true;
        }

        matchedKey = string.Empty;
        return false;
    }

    private static bool TryResolveExistingRecipe(
        IReadOnlyCollection<Recipe> recipes,
        string sourceKey,
        string displayName,
        out Recipe? recipe,
        out string matchedKey)
    {
        recipe = recipes.FirstOrDefault(item => string.Equals(item.InternalName, sourceKey, StringComparison.OrdinalIgnoreCase));
        if (recipe != null)
        {
            matchedKey = recipe.InternalName;
            return true;
        }

        var normalizedCandidates = recipes
            .Where(item => item != null)
            .Where(item =>
                NormalizeLookupKey(item.InternalName) == NormalizeLookupKey(sourceKey) ||
                NormalizeLookupKey(item.ExternalName) == NormalizeLookupKey(displayName) ||
                NormalizeLookupKey(item.Enhancement) == NormalizeLookupKey(sourceKey))
            .ToList();

        if (normalizedCandidates.Count == 1)
        {
            recipe = normalizedCandidates[0];
            matchedKey = recipe.InternalName;
            return true;
        }

        matchedKey = string.Empty;
        return false;
    }

    private static bool TryResolveExistingEnhancementSet(
        IReadOnlyCollection<EnhancementSet> sets,
        string sourceKey,
        string displayName,
        out EnhancementSet? set,
        out string matchedKey)
    {
        set = sets.FirstOrDefault(item => string.Equals(item.Uid, sourceKey, StringComparison.OrdinalIgnoreCase));
        if (set != null)
        {
            matchedKey = set.Uid;
            return true;
        }

        var normalizedCandidates = sets
            .Where(item => item != null)
            .Where(item =>
                NormalizeLookupKey(item.Uid) == NormalizeLookupKey(sourceKey) ||
                NormalizeLookupKey(item.DisplayName) == NormalizeLookupKey(displayName) ||
                NormalizeLookupKey(item.ShortName) == NormalizeLookupKey(displayName))
            .ToList();

        if (normalizedCandidates.Count == 1)
        {
            set = normalizedCandidates[0];
            matchedKey = set.Uid;
            return true;
        }

        matchedKey = string.Empty;
        return false;
    }

    private static bool TryResolveExistingEnhancement(
        IReadOnlyCollection<Enhancement> enhancements,
        string sourceKey,
        string displayName,
        out Enhancement? enhancement,
        out string matchedKey)
    {
        enhancement = enhancements.FirstOrDefault(item => string.Equals(item.UID, sourceKey, StringComparison.OrdinalIgnoreCase));
        if (enhancement != null)
        {
            matchedKey = enhancement.UID;
            return true;
        }

        var normalizedCandidates = enhancements
            .Where(item => item != null)
            .Where(item =>
                NormalizeLookupKey(item.UID) == NormalizeLookupKey(sourceKey) ||
                NormalizeLookupKey(item.Name) == NormalizeLookupKey(displayName) ||
                NormalizeLookupKey(item.ShortName) == NormalizeLookupKey(displayName))
            .ToList();

        if (normalizedCandidates.Count == 1)
        {
            enhancement = normalizedCandidates[0];
            matchedKey = enhancement.UID;
            return true;
        }

        matchedKey = string.Empty;
        return false;
    }

    private static bool TryResolveLinkedPower(
        string sourcePowerFullName,
        IReadOnlyDictionary<string, (int Index, IPower Power)> powerLookup,
        out (int Index, IPower Power) linkedPower,
        out string matchedFullName)
    {
        var canonical = CanonicalizeOmniFullName(sourcePowerFullName);
        if (powerLookup.TryGetValue(canonical, out linkedPower))
        {
            matchedFullName = canonical;
            return true;
        }

        var normalizedKey = NormalizeLookupKey(canonical);
        var fallbackMatches = powerLookup
            .Where(entry =>
                NormalizeLookupKey(entry.Key) == normalizedKey ||
                NormalizeLookupKey(entry.Value.Power.DisplayName) == normalizedKey)
            .Select(entry => (entry.Key, entry.Value))
            .Distinct()
            .ToList();

        if (fallbackMatches.Count == 1)
        {
            matchedFullName = fallbackMatches[0].Key;
            linkedPower = fallbackMatches[0].Value;
            return true;
        }

        linkedPower = default;
        matchedFullName = string.Empty;
        return false;
    }

    private static void ApplySalvageImport(
        IDatabase database,
        IReadOnlyCollection<NormalizedSalvageSource> salvageDefinitions,
        EnhancementImportMetadata metadata,
        EnhancementImportReconciliationIndex reconciliationIndex,
        OmniApplyResult applyResult)
    {
        if (salvageDefinitions.Count == 0)
        {
            return;
        }

        var salvageList = database.Salvage?.Where(item => item != null).ToList() ?? [];
        var byKey = salvageList
            .GroupBy(item => item.InternalName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var source in salvageDefinitions.Where(source => !string.IsNullOrWhiteSpace(source.Name)))
        {
            var resolution = reconciliationIndex.ResolveSalvage(source);
            RecordEntityResolution(source.Name, resolution, applyResult,
                applyResult.EnhancementReconciliationAuditDetails,
                applyResult.EnhancementReconciliationConflictDetails,
                isEnhancementSet: false,
                isRecipe: false,
                isSalvage: true);
            if (resolution.IsAmbiguous)
            {
                continue;
            }

            var existed = resolution.Found;
            var salvage = resolution.Item;

            if (!existed)
            {
                salvage = new Salvage
                {
                    InternalName = source.Name
                };
                salvageList.Add(salvage);
                byKey[source.Name] = salvage;
                applyResult.SalvageCreated++;
            }
            else
            {
                applyResult.SalvageMatched++;
                if (resolution.Kind == EnhancementReconciliationMatchKind.Alias &&
                    !string.Equals(source.Name, resolution.MatchedKey, StringComparison.OrdinalIgnoreCase))
                {
                    metadata.SalvageAliasCrosswalk[source.Name] = resolution.MatchedKey;
                    applyResult.SalvageAliasRepairs++;
                    applyResult.AddLimited(applyResult.EnhancementNamingReconciliationDetails,
                        $"Salvage {source.Name}: matched existing record '{resolution.MatchedKey}' via preserved alias.");
                }
            }

            var changed = false;
            changed |= AssignIfChanged(ref salvage.InternalName, source.Name);
            changed |= AssignIfChanged(ref salvage.ExternalName, source.DisplayName);
            changed |= AssignIfChanged(ref salvage.Rarity, source.Rarity);
            changed |= AssignIfChanged(ref salvage.Origin, source.Origin);

            if (changed)
            {
                applyResult.SalvageUpdated++;
                applyResult.AddLimited(applyResult.EnhancementImportDetails,
                    $"Salvage {source.Name}: updated from Omni export.");
            }
        }

        database.Salvage = salvageList.OrderBy(item => item.InternalName, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private static void ApplyRecipeImport(
        IDatabase database,
        IReadOnlyCollection<NormalizedRecipeSource> recipeDefinitions,
        EnhancementImportMetadata metadata,
        EnhancementImportReconciliationIndex reconciliationIndex,
        OmniApplyResult applyResult)
    {
        if (recipeDefinitions.Count == 0)
        {
            return;
        }

        var recipes = database.Recipes?.Where(item => item != null).ToList() ?? [];
        var byKey = recipes
            .GroupBy(item => item.InternalName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var source in recipeDefinitions.Where(definition => !string.IsNullOrWhiteSpace(definition.Name)))
        {
            var resolution = reconciliationIndex.ResolveRecipe(source);
            RecordEntityResolution(source.Name, resolution, applyResult,
                applyResult.EnhancementReconciliationAuditDetails,
                applyResult.EnhancementReconciliationConflictDetails,
                isEnhancementSet: false,
                isRecipe: true,
                isSalvage: false);
            if (resolution.IsAmbiguous)
            {
                continue;
            }

            var existed = resolution.Found;
            var recipe = resolution.Item;

            if (!existed)
            {
                recipe = new Recipe
                {
                    InternalName = source.Name
                };
                recipes.Add(recipe);
                byKey[source.Name] = recipe;
                applyResult.RecipesCreated++;
            }
            else
            {
                applyResult.RecipesMatched++;
                if (resolution.Kind == EnhancementReconciliationMatchKind.Alias &&
                    !string.Equals(source.Name, resolution.MatchedKey, StringComparison.OrdinalIgnoreCase))
                {
                    metadata.RecipeAliasCrosswalk[source.Name] = resolution.MatchedKey;
                    applyResult.RecipeAliasRepairs++;
                    applyResult.AddLimited(applyResult.EnhancementNamingReconciliationDetails,
                        $"Recipe {source.Name}: matched existing record '{resolution.MatchedKey}' via preserved alias.");
                }
            }

            var changed = false;
            changed |= AssignIfChanged(ref recipe.InternalName, source.Name);
            changed |= AssignIfChanged(ref recipe.ExternalName, source.DisplayName);
            changed |= AssignIfChanged(ref recipe.Enhancement, source.EnhancementRewardUid);
            changed |= AssignIfChanged(ref recipe.Rarity, source.Rarity);
            recipe.IsVirtual = false;
            recipe.IsHidden = false;
            recipe.IsGeneric = source.IsGeneric;

            var entries = source.LevelVariants
                .OrderBy(item => item.Level)
                .Select(definition => BuildRecipeEntry(definition))
                .ToArray();
            if (!RecipeEntriesEqual(recipe.Item, entries))
            {
                recipe.Item = entries;
                changed = true;
            }

            if (changed)
            {
                applyResult.RecipesUpdated++;
                applyResult.AddLimited(applyResult.EnhancementImportDetails,
                    $"Recipe {source.Name}: normalized {entries.Length} Omni level variants into one Mids recipe.");
            }
        }

        database.Recipes = recipes.OrderBy(item => item.InternalName, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private static Recipe.RecipeEntry BuildRecipeEntry(NormalizedRecipeLevelVariant definition)
    {
        var entry = new Recipe.RecipeEntry
        {
            Level = Math.Max(0, definition.Level - 1),
            BuyCost = definition.BuyFromVendor,
            CraftCost = ParseInt(definition.CreationCost.FirstOrDefault()),
            BuyCostM = 0,
            CraftCostM = 0,
            Salvage = definition.SalvageRequired.Select(item => item.Salvage).ToArray(),
            Count = definition.SalvageRequired.Select(item => item.Amount).ToArray(),
            SalvageIdx = definition.SalvageRequired.Select(_ => -1).ToArray()
        };

        return entry;
    }

    private static bool RecipeEntriesEqual(Recipe.RecipeEntry[] existing, Recipe.RecipeEntry[] replacement)
    {
        if (ReferenceEquals(existing, replacement))
        {
            return true;
        }

        if (existing == null || replacement == null || existing.Length != replacement.Length)
        {
            return false;
        }

        for (var index = 0; index < existing.Length; index++)
        {
            var left = existing[index];
            var right = replacement[index];
            if (left.Level != right.Level ||
                left.BuyCost != right.BuyCost ||
                left.CraftCost != right.CraftCost ||
                !left.Salvage.SequenceEqual(right.Salvage, StringComparer.OrdinalIgnoreCase) ||
                !left.Count.SequenceEqual(right.Count))
            {
                return false;
            }
        }

        return true;
    }

    private static void ApplyEnhancementSetImport(
        IDatabase database,
        IReadOnlyCollection<NormalizedEnhancementSetSource> setDefinitions,
        EnhancementImportMetadata metadata,
        EnhancementImportReconciliationIndex reconciliationIndex,
        OmniApplyResult applyResult)
    {
        var sets = database.EnhancementSets ?? new EnhancementSetCollection();
        var byKey = sets
            .Select((set, index) => new { set, index })
            .Where(item => item.set != null && !string.IsNullOrWhiteSpace(item.set.Uid))
            .GroupBy(item => item.set.Uid, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().set, StringComparer.OrdinalIgnoreCase);

        foreach (var source in setDefinitions.Where(source => !string.IsNullOrWhiteSpace(source.Name)))
        {
            var resolution = reconciliationIndex.ResolveEnhancementSet(source);
            RecordEntityResolution(source.Name, resolution, applyResult,
                applyResult.EnhancementReconciliationAuditDetails,
                applyResult.EnhancementReconciliationConflictDetails,
                isEnhancementSet: true,
                isRecipe: false,
                isSalvage: false);
            if (resolution.IsAmbiguous)
            {
                continue;
            }

            var existed = resolution.Found;
            var set = resolution.Item;

            if (!existed)
            {
                set = new EnhancementSet
                {
                    Uid = source.Name
                };
                sets.Add(set);
                byKey[source.Name] = set;
                applyResult.EnhancementSetsCreated++;
            }
            else
            {
                applyResult.EnhancementSetsMatched++;
                if (resolution.Kind == EnhancementReconciliationMatchKind.Alias &&
                    !string.Equals(source.Name, resolution.MatchedKey, StringComparison.OrdinalIgnoreCase))
                {
                    metadata.EnhancementSetAliasCrosswalk[source.Name] = resolution.MatchedKey;
                    applyResult.EnhancementSetAliasRepairs++;
                    applyResult.AddLimited(applyResult.EnhancementNamingReconciliationDetails,
                        $"Enhancement set {source.Name}: matched existing record '{resolution.MatchedKey}' via preserved alias.");
                }
            }

            metadata.EnhancementSetSourceKeys[source.Name] = source.SourceKey;
            metadata.EnhancementSetSourceGroups[source.Name] = source.GroupName;
            metadata.EnhancementSetSourceIcons[source.Name] = source.Icon;

            var changed = false;
            changed |= AssignIfChanged(ref set.Uid, source.Name);
            changed |= AssignIfChanged(ref set.DisplayName, source.DisplayName);
            changed |= AssignIfChanged(ref set.ShortName, source.Name);
            changed |= AssignIfChanged(ref set.Desc, source.DisplayName);
            if (!string.IsNullOrWhiteSpace(source.Icon) &&
                !string.Equals(set.Image, source.Icon, StringComparison.OrdinalIgnoreCase))
            {
                set.Image = source.Icon;
                changed = true;
                applyResult.AddLimited(applyResult.EnhancementImportDetails,
                    $"Enhancement set {source.Name}: refreshed icon to '{source.Icon}'.");
            }
            {
                var mappedSetType = MapSetType(database, source);
                if (mappedSetType != set.SetType)
                {
                    set.SetType = mappedSetType;
                    changed = true;
                }
            }

            if (TryGetSetLevelBand(metadata.EnhancementSetGroups, source.Name, out var levelMin, out var levelMax) ||
                (source.LevelMin.HasValue && source.LevelMax.HasValue &&
                 TryGetSetLevelBand(source, out levelMin, out levelMax)))
            {
                if (set.LevelMin != levelMin)
                {
                    set.LevelMin = levelMin;
                    changed = true;
                }

                if (set.LevelMax != levelMax)
                {
                    set.LevelMax = levelMax;
                    changed = true;
                }
            }

            var bonusItems = BuildBonusItems(source.Bonuses, source.Name, reconciliationIndex, metadata, applyResult);
            if (!BonusItemsEqual(set.Bonus, bonusItems))
            {
                set.Bonus = bonusItems;
                changed = true;
            }

            if (changed)
            {
                applyResult.EnhancementSetsUpdated++;
                applyResult.AddLimited(applyResult.EnhancementImportDetails,
                    $"Enhancement set {source.Name}: metadata and bonus links refreshed.");
            }
        }
    }

    private static EnhancementSet.BonusItem[] BuildBonusItems(
        IReadOnlyCollection<NormalizedSetBonusEntry> bonuses,
        string setName,
        EnhancementImportReconciliationIndex reconciliationIndex,
        EnhancementImportMetadata metadata,
        OmniApplyResult applyResult)
    {
        var standardBonuses = bonuses
            .Where(bonus => bonus.Kind == NormalizedSetBonusKind.Standard)
            .OrderBy(bonus => bonus.MinimumBoosts)
            .ToArray();
        if (standardBonuses.Length == 0)
        {
            return new EnhancementSet.BonusItem[11];
        }

        var items = new EnhancementSet.BonusItem[Math.Max(11, standardBonuses.Length)];
        for (var index = 0; index < items.Length; index++)
        {
            items[index].Special = -1;
            items[index].AltString = string.Empty;
            items[index].PvMode = Enums.ePvX.Any;
            items[index].Name = Array.Empty<string>();
            items[index].Index = Array.Empty<int>();
        }

        for (var index = 0; index < standardBonuses.Length; index++)
        {
            var source = standardBonuses[index];
            items[index].Special = -1;
            items[index].AltString = source.DisplayName ?? string.Empty;
            items[index].PvMode = Enums.ePvX.Any;
            items[index].Slotted = source.MinimumBoosts;

            var powerNames = GetSetBonusPowerNames(source);
            items[index].Name = powerNames;
            items[index].Index = ResolveSetBonusPowerIndices(
                powerNames,
                setName,
                reconciliationIndex,
                metadata,
                applyResult);
        }

        return items;
    }

    private static string[] GetSetBonusPowerNames(NormalizedSetBonusEntry bonus)
    {
        return bonus.AutoPowers
            .Concat(string.IsNullOrWhiteSpace(bonus.BonusPower) ? [] : [bonus.BonusPower])
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static int[] ResolveSetBonusPowerIndices(
        IEnumerable<string> powerNames,
        string setName,
        EnhancementImportReconciliationIndex reconciliationIndex,
        EnhancementImportMetadata metadata,
        OmniApplyResult applyResult)
    {
        return powerNames
            .Select(powerName =>
            {
                var resolution = reconciliationIndex.ResolvePower(powerName, setBonusPower: true);
                if (resolution.Found)
                {
                    applyResult.EnhancementSetBonusLinksResolved++;
                    var canonicalSourceName = CanonicalizeOmniFullName(powerName);
                    if (resolution.Kind == EnhancementReconciliationMatchKind.Alias)
                    {
                        applyResult.EnhancementSetBonusLinksAlias++;
                    }
                    else if (resolution.Kind == EnhancementReconciliationMatchKind.Fallback)
                    {
                        applyResult.EnhancementSetBonusLinksFallback++;
                    }

                    if (!string.Equals(canonicalSourceName, resolution.MatchedFullName, StringComparison.OrdinalIgnoreCase))
                    {
                        metadata.SetBonusPowerAliasCrosswalk[canonicalSourceName] = resolution.MatchedFullName;
                        applyResult.SetBonusPowerAliasRepairs++;
                        applyResult.AddLimited(applyResult.EnhancementNamingReconciliationDetails,
                            $"{setName}: set bonus power '{canonicalSourceName}' linked through existing '{resolution.MatchedFullName}'.");
                    }

                    return resolution.Index;
                }

                if (resolution.IsAmbiguous)
                {
                    applyResult.EnhancementSetBonusLinksMissing++;
                    applyResult.AddLimited(applyResult.EnhancementReconciliationConflictDetails,
                        $"{setName}: set bonus power '{powerName}' matched multiple existing powers [{string.Join(", ", resolution.CandidateKeys)}].");
                    return -1;
                }

                metadata.UnresolvedPowerLinks.Add(powerName);
                applyResult.EnhancementSetBonusLinksMissing++;
                applyResult.AddLimited(applyResult.UnresolvedEnhancementPowerLinks,
                    $"{setName}: set bonus power '{powerName}' was not present after power import.");
                return -1;
            })
            .ToArray();
    }

    private static EnhancementSet.BonusItem[] BuildSpecialBonusItems(
        EnhancementSet set,
        NormalizedEnhancementSetSource source,
        IDatabase database,
        EnhancementImportReconciliationIndex reconciliationIndex,
        EnhancementImportMetadata metadata,
        OmniApplyResult applyResult)
    {
        var memberCount = Math.Max(set.Enhancements?.Length ?? 0, 0);
        var items = new EnhancementSet.BonusItem[memberCount];
        for (var index = 0; index < items.Length; index++)
        {
            items[index].Special = -1;
            items[index].AltString = string.Empty;
            items[index].PvMode = Enums.ePvX.Any;
            items[index].Slotted = 0;
            items[index].Name = Array.Empty<string>();
            items[index].Index = Array.Empty<int>();
        }

        if (memberCount == 0)
        {
            return items;
        }

        var memberPositions = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < set.Enhancements.Length; index++)
        {
            var enhancementIndex = set.Enhancements[index];
            if (enhancementIndex < 0 || enhancementIndex >= database.Enhancements.Length)
            {
                continue;
            }

            var enhancement = database.Enhancements[enhancementIndex];
            AddIndexedValue(memberPositions, enhancement.UID, index);
            AddIndexedValue(memberPositions, enhancement.ShortName, index);
        }

        foreach (var bonus in source.Bonuses.Where(bonus => bonus.Kind != NormalizedSetBonusKind.Standard))
        {
            var powerNames = GetSetBonusPowerNames(bonus);
            var powerIndexes = ResolveSetBonusPowerIndices(powerNames, source.Name, reconciliationIndex, metadata, applyResult);
            var targetPositions = bonus.TargetEnhancementNames
                .SelectMany(targetName => memberPositions.TryGetValue(targetName, out var positions) ? positions : [])
                .Distinct()
                .OrderBy(position => position)
                .ToArray();

            if (targetPositions.Length == 0)
            {
                foreach (var requirement in bonus.RequiresTokens.Where(requirement => !string.IsNullOrWhiteSpace(requirement)))
                {
                    metadata.UnconsumedBonusRequirements.Add($"{source.Name}: {requirement}");
                }
                continue;
            }

            foreach (var position in targetPositions)
            {
                items[position].Special = -1;
                if (items[position].Slotted == 0)
                {
                    items[position].Slotted = bonus.MinimumBoosts;
                }

                if (string.IsNullOrWhiteSpace(items[position].AltString) && !string.IsNullOrWhiteSpace(bonus.DisplayName))
                {
                    items[position].AltString = bonus.DisplayName;
                }

                items[position].Name = items[position].Name
                    .Concat(powerNames)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                items[position].Index = items[position].Index
                    .Concat(powerIndexes)
                    .Distinct()
                    .ToArray();
            }
        }

        return items;
    }

    private static void AddIndexedValue(Dictionary<string, List<int>> lookup, string value, int index)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!lookup.TryGetValue(value, out var indexes))
        {
            indexes = [];
            lookup[value] = indexes;
        }

        if (!indexes.Contains(index))
        {
            indexes.Add(index);
        }
    }

    private static bool BonusItemsEqual(EnhancementSet.BonusItem[] existing, EnhancementSet.BonusItem[] replacement)
    {
        if (ReferenceEquals(existing, replacement))
        {
            return true;
        }

        if (existing == null || replacement == null || existing.Length != replacement.Length)
        {
            return false;
        }

        for (var index = 0; index < existing.Length; index++)
        {
            if (existing[index].Slotted != replacement[index].Slotted ||
                existing[index].PvMode != replacement[index].PvMode ||
                !existing[index].Name.SequenceEqual(replacement[index].Name, StringComparer.OrdinalIgnoreCase) ||
                !existing[index].Index.SequenceEqual(replacement[index].Index))
            {
                return false;
            }
        }

        return true;
    }

    private static void ApplyEnhancementImport(
        IDatabase database,
        IReadOnlyCollection<NormalizedEnhancementSource> enhancementDefinitions,
        EnhancementImportMetadata metadata,
        IReadOnlyDictionary<string, int> enhancementClassLookup,
        EnhancementImportReconciliationIndex reconciliationIndex,
        OmniApplyResult applyResult)
    {
        var enhancements = database.Enhancements?
            .Where(item => item != null)
            .Select(item => item as Enhancement)
            .Where(item => item != null)
            .Cast<Enhancement>()
            .ToList() ?? [];
        var nextStaticIndex = enhancements.Select(item => item.StaticIndex).DefaultIfEmpty(-1).Max() + 1;
        var byUid = enhancements
            .GroupBy(item => item.UID, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => (Enhancement)group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var source in enhancementDefinitions.Where(source => !string.IsNullOrWhiteSpace(source.Name)))
        {
            var canonicalClassicSource = GetCanonicalClassicSurfaceSource(source);
            var resolution = reconciliationIndex.ResolveEnhancement(source);
            RecordEntityResolution(source.Name, resolution, applyResult,
                applyResult.EnhancementReconciliationAuditDetails,
                applyResult.EnhancementReconciliationConflictDetails,
                isEnhancementSet: false,
                isRecipe: false,
                isSalvage: false);
            if (resolution.IsAmbiguous)
            {
                continue;
            }

            var existed = resolution.Found;
            var enhancement = resolution.Item;
            var preserveExistingClassicIdentity = false;

            if (existed &&
                enhancement != null &&
                source.IsClassicOriginVariant &&
                TryGetLegacyCanonicalClassicUid(enhancement, metadata, out var legacyCanonicalUid) &&
                !string.Equals(legacyCanonicalUid, enhancement.UID, StringComparison.OrdinalIgnoreCase))
            {
                if (byUid.TryGetValue(legacyCanonicalUid, out var canonicalEnhancement))
                {
                    enhancement = canonicalEnhancement;
                }
                else
                {
                    existed = false;
                }
            }

            if (!existed)
            {
                enhancement = new Enhancement
                {
                    UID = canonicalClassicSource.Name,
                    StaticIndex = nextStaticIndex++
                };
                enhancements.Add(enhancement);
                byUid[enhancement.UID] = enhancement;
                applyResult.EnhancementsCreated++;
            }
            else
            {
                applyResult.EnhancementsMatched++;
                preserveExistingClassicIdentity = source.IsClassicOriginVariant;
                if (resolution.Kind == EnhancementReconciliationMatchKind.Alias &&
                    !string.Equals(source.Name, resolution.MatchedKey, StringComparison.OrdinalIgnoreCase))
                {
                    metadata.EnhancementAliasCrosswalk[source.Name] = resolution.MatchedKey;
                    applyResult.EnhancementAliasRepairs++;
                    applyResult.AddLimited(applyResult.EnhancementNamingReconciliationDetails,
                        $"Enhancement {source.Name}: matched existing record '{resolution.MatchedKey}' via preserved alias.");
                }
            }

            foreach (var variant in new[] { source }.Concat(source.FoldedClassicVariants))
            {
                metadata.EnhancementSourceCategories[variant.Name] = variant.EnhancementType;
                metadata.EnhancementSourceDisplayNames[variant.Name] = variant.DisplayName;
                metadata.EnhancementSourceFamilies[variant.Name] = variant.EnhancementFamily;
                metadata.EnhancementSourceIcons[variant.Name] = variant.Icon;

                if (variant.IsClassicOriginVariant && !string.IsNullOrWhiteSpace(variant.ClassicFoldKey))
                {
                    metadata.ClassicEnhancementCanonicalBySourceKey[variant.SourceKey] = canonicalClassicSource.SourceKey;
                    metadata.ClassicEnhancementCanonicalByName[variant.Name] = enhancement.UID;
                    metadata.ClassicEnhancementFoldKeys[variant.SourceKey] = variant.ClassicFoldKey;
                    metadata.ClassicEnhancementSourcesByName[variant.Name] = BuildClassicEnhancementSourceVariantMetadata(variant);
                }
            }

            if (source.IsClassicOriginVariant)
            {
                metadata.ClassicEnhancementSourceNamesByCanonicalName[enhancement.UID] = new[] { source }
                    .Concat(source.FoldedClassicVariants)
                    .Select(item => item.Name)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            if (source.FoldedClassicVariants.Count > 0)
            {
                metadata.ClassicEnhancementFoldAudit.Add(
                    $"{source.Name}: kept {source.EnhancementType}, folded {string.Join(", ", source.FoldedClassicVariants.Select(item => $"{item.EnhancementType}:{item.Name}"))}.");
            }

            if (MapEnhancementType(source) == Enums.eType.SpecialO)
            {
                metadata.SpecialFamilyCrosswalk[source.EnhancementType] =
                    ResolveSpecialSubtypeShortName(source);
            }

            var changed = false;
            var targetUid = preserveExistingClassicIdentity ? enhancement.UID : canonicalClassicSource.Name;
            if (!string.Equals(enhancement.UID, targetUid, StringComparison.Ordinal))
            {
                if (byUid.TryGetValue(enhancement.UID, out var mappedEnhancement) && ReferenceEquals(mappedEnhancement, enhancement))
                {
                    byUid.Remove(enhancement.UID);
                }

                enhancement.UID = targetUid;
                byUid[enhancement.UID] = enhancement;
                changed = true;
            }

            var targetName = preserveExistingClassicIdentity ? enhancement.Name : canonicalClassicSource.DisplayName;
            if (!string.Equals(enhancement.Name, targetName, StringComparison.Ordinal))
            {
                enhancement.Name = targetName;
                changed = true;
            }

            var targetShortName = preserveExistingClassicIdentity ? enhancement.ShortName : canonicalClassicSource.Name;
            if (!string.Equals(enhancement.ShortName, targetShortName, StringComparison.Ordinal))
            {
                enhancement.ShortName = targetShortName;
                changed = true;
            }

            var desc = preserveExistingClassicIdentity
                ? enhancement.Desc
                : !string.IsNullOrWhiteSpace(canonicalClassicSource.DisplayName)
                    ? canonicalClassicSource.DisplayName
                    : enhancement.Desc;
            if (!string.Equals(enhancement.Desc, desc, StringComparison.Ordinal))
            {
                enhancement.Desc = desc;
                changed = true;
            }

            var mappedType = MapEnhancementType(source);
            if (enhancement.TypeID != mappedType)
            {
                enhancement.TypeID = mappedType;
                changed = true;
            }

            var mappedSubtype = MapSpecialSubtype(database, source);
            if (enhancement.SubTypeID != mappedSubtype)
            {
                enhancement.SubTypeID = mappedSubtype;
                changed = true;
            }

            var preferredIcon = source.IsClassicOriginVariant ? canonicalClassicSource.Icon : source.Icon;
            if (!string.IsNullOrWhiteSpace(preferredIcon))
            {
                if (string.Equals(enhancement.Image, preferredIcon, StringComparison.OrdinalIgnoreCase))
                {
                    applyResult.EnhancementIconsPreserved++;
                    applyResult.AddLimited(applyResult.EnhancementIconDetails,
                        $"{source.Name}: kept source-aligned icon '{enhancement.Image}'.");
                }
                else
                {
                    var priorImage = string.IsNullOrWhiteSpace(enhancement.Image) ? "<blank>" : enhancement.Image;
                    enhancement.Image = preferredIcon;
                    changed = true;
                    applyResult.EnhancementIconsAssigned++;
                    applyResult.AddLimited(applyResult.EnhancementIconDetails,
                        $"{source.Name}: refreshed icon to '{preferredIcon}' (was {priorImage}).");
                }
            }
            else if (!string.IsNullOrWhiteSpace(enhancement.Image))
            {
                applyResult.EnhancementIconsPreserved++;
                applyResult.AddLimited(applyResult.EnhancementIconDetails,
                    $"{source.Name}: kept current Mids icon '{enhancement.Image}' because source icon was blank.");
            }
            else
            {
                applyResult.EnhancementIconsMissing++;
                applyResult.AddLimited(applyResult.EnhancementIconDetails,
                    $"{source.Name}: no current or source icon was available.");
            }

            if (!string.Equals(enhancement.UIDSet, source.EnhancementSetName, StringComparison.Ordinal))
            {
                enhancement.UIDSet = source.EnhancementSetName;
                changed = true;
            }
            var levelMin = source.LevelMin;
            var levelMax = source.LevelMax;
            if (enhancement.LevelMin != levelMin)
            {
                enhancement.LevelMin = levelMin;
                changed = true;
            }
            if (enhancement.LevelMax != levelMax)
            {
                enhancement.LevelMax = levelMax;
                changed = true;
            }
            var superior = source.SuperiorAttuned || source.DisplayName.Contains("Superior", StringComparison.OrdinalIgnoreCase);
            if (enhancement.Superior != superior)
            {
                enhancement.Superior = superior;
                changed = true;
            }
            if (mappedType is Enums.eType.InventO or Enums.eType.SetO)
            {
                if (!string.Equals(enhancement.RecipeName, source.RecipeKey, StringComparison.Ordinal))
                {
                    enhancement.RecipeName = source.RecipeKey;
                    changed = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(enhancement.RecipeName))
                {
                    enhancement.RecipeName = string.Empty;
                    changed = true;
                }
            }

            enhancement.RecipeIDX = -1;
            enhancement.BuffMode = enhancement.BuffMode == Enums.eBuffDebuff.Any ? Enums.eBuffDebuff.Any : enhancement.BuffMode;

            var powerResolution = reconciliationIndex.ResolvePower(source.PowerFullName, setBonusPower: false);
            var replacementEffects = Array.Empty<Enums.sEffect>();
            if (powerResolution.Found)
            {
                ((IEnhancement)enhancement).SetPower(powerResolution.Power);
                applyResult.EnhancementBoostPowerLinksResolved++;
                var canonicalSourceName = CanonicalizeOmniFullName(source.PowerFullName);
                if (powerResolution.Kind == EnhancementReconciliationMatchKind.Alias)
                {
                    applyResult.EnhancementBoostPowerLinksAlias++;
                }
                else if (powerResolution.Kind == EnhancementReconciliationMatchKind.Fallback)
                {
                    applyResult.EnhancementBoostPowerLinksFallback++;
                }

                if (!string.Equals(canonicalSourceName, powerResolution.MatchedFullName, StringComparison.OrdinalIgnoreCase))
                {
                    metadata.BoostPowerAliasCrosswalk[canonicalSourceName] = powerResolution.MatchedFullName;
                    applyResult.BoostPowerAliasRepairs++;
                    applyResult.AddLimited(applyResult.EnhancementNamingReconciliationDetails,
                        $"Enhancement {source.Name}: boost power '{canonicalSourceName}' linked through existing '{powerResolution.MatchedFullName}'.");
                }

                replacementEffects = BuildEnhancementEffects(powerResolution.Power, enhancement.TypeID);
                if (!EnhancementEffectsEqual(enhancement.Effect, replacementEffects))
                {
                    enhancement.Effect = replacementEffects;
                    changed = true;
                }

                var isProc = enhancement.Effect.Any(effect => effect.Mode == Enums.eEffMode.FX);
                if (enhancement.IsProc != isProc)
                {
                    enhancement.IsProc = isProc;
                    changed = true;
                }

                if ((!existed || string.IsNullOrWhiteSpace(enhancement.Desc)) &&
                    !string.IsNullOrWhiteSpace(powerResolution.Power.DescLong))
                {
                    if (!string.Equals(enhancement.Desc, powerResolution.Power.DescLong, StringComparison.Ordinal))
                    {
                        enhancement.Desc = powerResolution.Power.DescLong;
                        changed = true;
                    }
                }
            }
            else if (powerResolution.IsAmbiguous)
            {
                applyResult.EnhancementBoostPowerLinksMissing++;
                applyResult.AddLimited(applyResult.EnhancementReconciliationConflictDetails,
                    $"{source.Name}: boost power '{source.PowerFullName}' matched multiple existing powers [{string.Join(", ", powerResolution.CandidateKeys)}].");
            }
            else
            {
                metadata.UnresolvedPowerLinks.Add(source.PowerFullName);
                applyResult.EnhancementBoostPowerLinksMissing++;
                applyResult.AddLimited(applyResult.UnresolvedEnhancementPowerLinks,
                    $"{source.Name}: boost power '{source.PowerFullName}' was not present after power import.");
            }

            var wrapperClassIds = ResolveEnhancementSourceWrapperClassRefs(
                source.BoostsAllowed,
                enhancementClassLookup,
                out var mappedWrapperLabels,
                out var unresolvedWrapperLabels);
            var specificWrapperClassIds = ResolveSpecificEnhancementSourceWrapperClassRefs(
                source.BoostsAllowed,
                enhancementClassLookup);

            var existingClassIds = enhancement.ClassID ?? Array.Empty<int>();
            if (TryResolveEnhancementClassRefsFromEffects(
                    replacementEffects,
                    source.BoostsAllowed.Select(value => value.Name).Where(value => !string.IsNullOrWhiteSpace(value)).ToArray(),
                    enhancementClassLookup,
                    database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>(),
                    out var effectDerivedClassIds,
                    out var unresolvedEffectSemantics))
            {
                var finalClassIds = MergeEffectAndSpecificWrapperClassRefs(
                    effectDerivedClassIds,
                    specificWrapperClassIds,
                    database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>(),
                    enhancementClassLookup);
                applyResult.EnhancementClassIdsDerivedFromEffects++;
                if (wrapperClassIds.Length > 0 && !wrapperClassIds.SequenceEqual(finalClassIds))
                {
                    applyResult.EnhancementClassIdWrapperMismatches++;
                    applyResult.AddLimited(applyResult.EnhancementClassDerivationDetails,
                        $"{source.Name}: linked boost effects -> [{FormatEnhancementClassRefSummary(effectDerivedClassIds, database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>())}] while wrapper boosts_allowed implied [{FormatEnhancementClassRefSummary(wrapperClassIds, database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>())}] from [{string.Join(", ", mappedWrapperLabels)}]; final classes=[{FormatEnhancementClassRefSummary(finalClassIds, database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>())}].");
                }

                if (unresolvedEffectSemantics.Length > 0)
                {
                    applyResult.AddLimited(applyResult.EnhancementClassDerivationDetails,
                        $"{source.Name}: linked boost effects resolved classes [{FormatEnhancementClassRefSummary(finalClassIds, database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>())}] with unmapped effect semantics [{string.Join(", ", unresolvedEffectSemantics)}].");
                    }

                if (!existingClassIds.SequenceEqual(finalClassIds))
                {
                    enhancement.ClassID = finalClassIds;
                    changed = true;
                }
            }
            else if (TryResolveConservativeEnhancementSourceFallbackClassRefs(
                         source.BoostsAllowed,
                         enhancementClassLookup,
                         out var fallbackClassIds,
                         out mappedWrapperLabels,
                         out unresolvedWrapperLabels))
            {
                applyResult.EnhancementClassIdsFallbackUsed++;
                applyResult.AddLimited(applyResult.EnhancementClassDerivationDetails,
                    $"{source.Name}: fell back to wrapper boosts_allowed [{string.Join(", ", mappedWrapperLabels)}] -> [{FormatEnhancementClassRefSummary(fallbackClassIds, database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>())}] because linked boost effects did not produce canonical classes.");

                if (!existingClassIds.SequenceEqual(fallbackClassIds))
                {
                    enhancement.ClassID = fallbackClassIds;
                    changed = true;
                }
            }
            else if (ShouldTreatEnhancementAsCategoryOnly(
                         enhancement,
                         powerResolution.Power,
                         replacementEffects,
                         source.BoostsAllowed,
                         out var categoryOnlyReason,
                         out var categoryOnlyWrapperLabels))
            {
                applyResult.EnhancementClassIdsCategoryOnly++;
                if (powerResolution.Found && !string.IsNullOrWhiteSpace(powerResolution.Power?.FullName))
                {
                    var categoryOnlyBoostFullName = CanonicalizeOmniFullName(powerResolution.Power.FullName);
                    if (!metadata.CategoryOnlyBoostPowerFullNames.Contains(categoryOnlyBoostFullName, StringComparer.OrdinalIgnoreCase))
                    {
                        metadata.CategoryOnlyBoostPowerFullNames.Add(categoryOnlyBoostFullName);
                    }
                }

                applyResult.AddLimited(applyResult.EnhancementClassDerivationDetails,
                    $"{source.Name}: treated as category-only SetO piece; linked boost='{powerResolution.MatchedFullName}', wrapper boosts_allowed=[{FormatSourceBoostAllowedLabels(categoryOnlyWrapperLabels)}], reason={categoryOnlyReason}.");

                if (existingClassIds.Length > 0)
                {
                    enhancement.ClassID = Array.Empty<int>();
                    changed = true;
                }
            }
            else
            {
                applyResult.EnhancementClassIdsUnresolved++;
                var unresolvedSummary = unresolvedEffectSemantics.Length > 0
                    ? string.Join(", ", unresolvedEffectSemantics)
                    : unresolvedWrapperLabels.Length > 0
                        ? string.Join(", ", unresolvedWrapperLabels)
                        : "<none>";
                if (existingClassIds.Length > 0)
                {
                    applyResult.AddLimited(applyResult.EnhancementClassDerivationDetails,
                        $"{source.Name}: preserved existing enhancement classes [{FormatEnhancementClassRefSummary(existingClassIds, database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>())}] because no canonical class derivation source was available; unresolved=[{unresolvedSummary}].");
                }
                else
                {
                    applyResult.AddLimited(applyResult.EnhancementClassDerivationDetails,
                        $"{source.Name}: no canonical enhancement classes could be derived from linked boost effects or wrapper boosts_allowed; unresolved=[{unresolvedSummary}].");
                }
            }

            if (changed)
            {
                applyResult.EnhancementsUpdated++;
                applyResult.AddLimited(applyResult.EnhancementImportDetails,
                    $"Enhancement {source.Name}: metadata refreshed and linked to existing boost power.");
            }
        }

        database.Enhancements = enhancements
            .OrderBy(item => item.StaticIndex)
            .Cast<IEnhancement>()
            .ToArray();
    }

    private static Enums.sEffect[] BuildEnhancementEffects(IPower power, Enums.eType enhancementType)
    {
        if (power?.Effects == null || power.Effects.Length == 0)
        {
            return Array.Empty<Enums.sEffect>();
        }

        var results = new List<Enums.sEffect>();
        foreach (var effect in power.Effects.Where(effect => effect != null))
        {
            var mappedEnhance = MapEnhanceFromEffect(effect);
            if (mappedEnhance == Enums.eEnhance.None)
            {
                if (effect.EffectType != Enums.eEffectType.None)
                {
                    results.Add(new Enums.sEffect
                    {
                        Mode = Enums.eEffMode.FX,
                        BuffMode = Enums.eBuffDebuff.Any,
                        Enhance = new Enums.sTwinID { ID = 0, SubID = -1 },
                        Schedule = Enums.eSchedule.None,
                        Multiplier = 0,
                        FX = effect.Clone() as IEffect
                    });
                }

                continue;
            }

            var schedule = Enhancement.GetSchedule(mappedEnhance, mappedEnhance == Enums.eEnhance.Mez ? (int)effect.MezType : -1);
            results.Add(new Enums.sEffect
            {
                Mode = Enums.eEffMode.Enhancement,
                BuffMode = MapBuffMode(effect),
                Enhance = new Enums.sTwinID
                {
                    ID = (int)mappedEnhance,
                    SubID = mappedEnhance == Enums.eEnhance.Mez ? (int)effect.MezType : -1
                },
                Schedule = schedule,
                Multiplier = EnhancementScheduleMath.NormalizeImportedScaleToMultiplier(
                    enhancementType,
                    schedule,
                    Math.Abs(effect.Scale) > 0.0001f ? effect.Scale : effect.nMagnitude)
            });
        }

        return results.ToArray();
    }

    private static bool TryResolveEnhancementClassRefsFromEffects(
        IReadOnlyCollection<Enums.sEffect>? effects,
        IReadOnlyCollection<string> sourceBoostAllowedLabels,
        IReadOnlyDictionary<string, int> enhancementClassLookup,
        Enums.sEnhClass[] enhancementClasses,
        out int[] classRefs,
        out string[] unresolvedSemantics)
    {
        if (effects == null || effects.Count == 0)
        {
            classRefs = Array.Empty<int>();
            unresolvedSemantics = Array.Empty<string>();
            return false;
        }

        var resolved = new List<int>();
        var seen = new HashSet<int>();
        var unresolved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var effect in effects.Where(effect => effect.Mode == Enums.eEffMode.Enhancement))
        {
            if (TryResolveEnhancementClassRefFromEffect(
                    effect,
                    sourceBoostAllowedLabels,
                    enhancementClassLookup,
                    enhancementClasses,
                    out var classRef,
                    out var unresolvedSemantic))
            {
                if (seen.Add(classRef))
                {
                    resolved.Add(classRef);
                }

                continue;
            }

            if (!string.IsNullOrWhiteSpace(unresolvedSemantic))
            {
                unresolved.Add(unresolvedSemantic);
            }
        }

        classRefs = resolved
            .Distinct()
            .OrderBy(id => id)
            .ToArray();
        unresolvedSemantics = unresolved
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return classRefs.Length > 0;
    }

    private static bool TryResolveEnhancementClassRefFromEffect(
        Enums.sEffect effect,
        IReadOnlyCollection<string> sourceBoostAllowedLabels,
        IReadOnlyDictionary<string, int> enhancementClassLookup,
        Enums.sEnhClass[] enhancementClasses,
        out int classRef,
        out string unresolvedSemantic)
    {
        classRef = -1;
        unresolvedSemantic = DescribeEnhancementEffectSemantic(effect);

        if (!TryResolveEnhancementClassLookupKeyFromEffect(effect, sourceBoostAllowedLabels, out var classKey))
        {
            return false;
        }

        if (!enhancementClassLookup.TryGetValue(NormalizeLookupKey(classKey), out classRef))
        {
            unresolvedSemantic = classKey;
            classRef = -1;
            return false;
        }

        if (sourceBoostAllowedLabels.Count > 0)
        {
            var specificWrapperClassRefs = ResolveSpecificEnhancementSourceWrapperClassRefs(
                sourceBoostAllowedLabels.Select(label => new OmniBoostAllowedRef { Name = label }),
                enhancementClassLookup);
            if (specificWrapperClassRefs.Length > 0)
            {
                var merged = MergeEffectAndSpecificWrapperClassRefs(
                    new[] { classRef },
                    specificWrapperClassRefs,
                    enhancementClasses,
                    enhancementClassLookup);
                if (merged.Length > 0)
                {
                    classRef = merged[0];
                }
            }
        }

        unresolvedSemantic = string.Empty;
        return true;
    }

    private static bool TryResolveEnhancementClassLookupKeyFromEffect(
        Enums.sEffect effect,
        IReadOnlyCollection<string> sourceBoostAllowedLabels,
        out string classKey)
    {
        classKey = string.Empty;
        var enhance = (Enums.eEnhance)effect.Enhance.ID;
        switch (enhance)
        {
            case Enums.eEnhance.Accuracy:
                classKey = "Accuracy_Boost";
                return true;
            case Enums.eEnhance.Damage:
                classKey = "Damage_Boost";
                return true;
            case Enums.eEnhance.Defense:
                classKey = SourceLabelsContain(sourceBoostAllowedLabels, "Enhance Defense DeBuff")
                    ? "Debuff_Defense_Boost"
                    : SourceLabelsContain(sourceBoostAllowedLabels, "Enhance Defense")
                        ? "Buff_Defense_Boost"
                        : effect.BuffMode == Enums.eBuffDebuff.DeBuffOnly
                    ? "Debuff_Defense_Boost"
                    : "Buff_Defense_Boost";
                return true;
            case Enums.eEnhance.EnduranceDiscount:
                classKey = "EnduranceDiscount_Boost";
                return true;
            case Enums.eEnhance.Endurance:
            case Enums.eEnhance.Recovery:
                classKey = "Recovery_Boost";
                return true;
            case Enums.eEnhance.SpeedFlying:
                classKey = "SpeedFlying_Boost";
                return true;
            case Enums.eEnhance.Heal:
            case Enums.eEnhance.HitPoints:
            case Enums.eEnhance.Regeneration:
            case Enums.eEnhance.Absorb:
                classKey = "Heal_Boost";
                return true;
            case Enums.eEnhance.Interrupt:
                classKey = "Interrupt_Boost";
                return true;
            case Enums.eEnhance.JumpHeight:
            case Enums.eEnhance.SpeedJumping:
                classKey = "Jump_Boost";
                return true;
            case Enums.eEnhance.Mez:
                return TryResolveEnhancementClassLookupKeyFromMezSubType(effect.Enhance.SubID, out classKey);
            case Enums.eEnhance.Range:
                classKey = "Range_Boost";
                return true;
            case Enums.eEnhance.RechargeTime:
            case Enums.eEnhance.X_RechargeTime:
                classKey = "Recharge_Boost";
                return true;
            case Enums.eEnhance.Resistance:
                classKey = "Res_Damage_Boost";
                return true;
            case Enums.eEnhance.SpeedRunning:
                classKey = "SpeedRunning_Boost";
                return true;
            case Enums.eEnhance.ToHit:
                classKey = SourceLabelsContain(sourceBoostAllowedLabels, "Enhance ToHit DeBuffs")
                    ? "Debuff_ToHit_Boost"
                    : SourceLabelsContain(sourceBoostAllowedLabels, "Enhance ToHit Buffs")
                        ? "Buff_ToHit_Boost"
                        : effect.BuffMode == Enums.eBuffDebuff.DeBuffOnly
                    ? "Debuff_ToHit_Boost"
                    : "Buff_ToHit_Boost";
                return true;
            case Enums.eEnhance.Slow:
                classKey = "Slow_Boost";
                return true;
            default:
                return false;
        }
    }

    private static bool TryResolveEnhancementClassLookupKeyFromMezSubType(int subId, out string classKey)
    {
        classKey = subId switch
        {
            (int)Enums.eMez.Confused => "Confuse_Boost",
            (int)Enums.eMez.Held => "Hold_Boost",
            (int)Enums.eMez.Immobilized => "Immobilized_Boost",
            (int)Enums.eMez.Knockback or (int)Enums.eMez.Knockup => "Knockback_Boost",
            (int)Enums.eMez.Sleep => "Sleep_Boost",
            (int)Enums.eMez.Stunned => "Stunned_Boost",
            (int)Enums.eMez.Taunt => "Taunt_Boost",
            (int)Enums.eMez.Terrorized or (int)Enums.eMez.Afraid => "Fear_Boost",
            (int)Enums.eMez.Intangible => "Intangible_Boost",
            _ => string.Empty
        };

        return !string.IsNullOrWhiteSpace(classKey);
    }

    private static string DescribeEnhancementEffectSemantic(Enums.sEffect effect)
    {
        var enhanceName = Enum.GetName(typeof(Enums.eEnhance), effect.Enhance.ID) ?? effect.Enhance.ID.ToString();
        if ((Enums.eEnhance)effect.Enhance.ID != Enums.eEnhance.Mez)
        {
            return $"{enhanceName} ({effect.BuffMode})";
        }

        var mezName = Enum.GetName(typeof(Enums.eMez), effect.Enhance.SubID) ?? effect.Enhance.SubID.ToString();
        return $"{enhanceName}:{mezName} ({effect.BuffMode})";
    }

    private static int[] ResolveEnhancementSourceWrapperClassRefs(
        IEnumerable<OmniBoostAllowedRef> boostsAllowed,
        IReadOnlyDictionary<string, int> enhancementClassLookup,
        out string[] mappedLabels,
        out string[] unresolvedLabels)
    {
        var resolved = new List<int>();
        var seen = new HashSet<int>();
        var mapped = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var unresolved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var label in boostsAllowed
                     .Select(value => value?.Name ?? string.Empty)
                     .Where(value => !string.IsNullOrWhiteSpace(value))
                     .Where(value => !IsNonStatBoostPowerCompatibilityTag(value))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var labelResolved = false;
            foreach (var candidate in EnumeratePowerBoostAllowedLookupCandidates(label))
            {
                if (!enhancementClassLookup.TryGetValue(NormalizeLookupKey(candidate), out var classRef))
                {
                    continue;
                }

                labelResolved = true;
                mapped.Add(label);
                if (seen.Add(classRef))
                {
                    resolved.Add(classRef);
                }
            }

            if (!labelResolved)
            {
                unresolved.Add(label);
            }
        }

        mappedLabels = mapped.OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToArray();
        unresolvedLabels = unresolved.OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToArray();
        return resolved
            .Distinct()
            .OrderBy(id => id)
            .ToArray();
    }

    private static int[] ResolveSpecificEnhancementSourceWrapperClassRefs(
        IEnumerable<OmniBoostAllowedRef> boostsAllowed,
        IReadOnlyDictionary<string, int> enhancementClassLookup)
    {
        var labels = boostsAllowed
            .Select(value => value?.Name ?? string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (labels.Count == 0)
        {
            return Array.Empty<int>();
        }

        var filtered = labels
            .Where(label => !IsGenericEnhancementWrapperNoiseLabel(label))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (filtered.Count == 0)
        {
            return Array.Empty<int>();
        }

        var resolved = new List<int>();
        var seen = new HashSet<int>();
        foreach (var label in filtered)
        {
            foreach (var candidate in EnumeratePowerBoostAllowedLookupCandidates(label))
            {
                if (!enhancementClassLookup.TryGetValue(NormalizeLookupKey(candidate), out var classRef))
                {
                    continue;
                }

                if (seen.Add(classRef))
                {
                    resolved.Add(classRef);
                }
            }
        }

        return resolved
            .Distinct()
            .OrderBy(id => id)
            .ToArray();
    }

    private static bool IsGenericEnhancementWrapperNoiseLabel(string label)
    {
        var normalized = NormalizeLookupKey(label);
        return normalized is "enhanceaccuracy" or
               "enhancedamage" or
               "enhanceconfuse" or
               "enhancedefense" or
               "enhancetohitbuffs";
    }

    private static bool ShouldTreatEnhancementAsCategoryOnly(
        Enhancement enhancement,
        IPower? linkedBoostPower,
        IReadOnlyCollection<Enums.sEffect>? replacementEffects,
        IEnumerable<OmniBoostAllowedRef> boostsAllowed,
        out string reason,
        out string[] wrapperLabels)
    {
        reason = string.Empty;
        wrapperLabels = boostsAllowed
            .Select(value => value?.Name ?? string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (enhancement.TypeID != Enums.eType.SetO || linkedBoostPower == null)
        {
            return false;
        }

        var hasEnhancementAspect = replacementEffects?.Any(effect => effect.Mode == Enums.eEffMode.Enhancement) == true;
        if (hasEnhancementAspect)
        {
            return false;
        }

        var hasSupportOnlyShape = replacementEffects?.Any(effect => effect.Mode == Enums.eEffMode.FX) == true ||
                                  (linkedBoostPower.Effects?.Any() ?? false);
        if (!hasSupportOnlyShape)
        {
            return false;
        }

        if (wrapperLabels.Any(label => !IsNonStatBoostPowerCompatibilityTag(label) && !IsGenericEnhancementWrapperNoiseLabel(label)))
        {
            return false;
        }

        var wrapperSummary = FormatSourceBoostAllowedLabels(wrapperLabels);
        reason = wrapperLabels.Length == 0
            ? "no canonical enhancement-aspect effects were present and the linked boost only exposed support/proc-style templates"
            : $"no canonical enhancement-aspect effects were present and wrapper boosts_allowed contained only generic/noise compatibility labels [{wrapperSummary}]";
        return true;
    }

    private static bool TryResolveConservativeEnhancementSourceFallbackClassRefs(
        IEnumerable<OmniBoostAllowedRef> boostsAllowed,
        IReadOnlyDictionary<string, int> enhancementClassLookup,
        out int[] classRefs,
        out string[] mappedLabels,
        out string[] unresolvedLabels)
    {
        var specific = ResolveSpecificEnhancementSourceWrapperClassRefs(boostsAllowed, enhancementClassLookup);
        if (specific.Length > 0)
        {
            classRefs = specific;
            mappedLabels = boostsAllowed
                .Select(value => value?.Name ?? string.Empty)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Where(value => !IsGenericEnhancementWrapperNoiseLabel(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            unresolvedLabels = Array.Empty<string>();
            return true;
        }

        classRefs = ResolveEnhancementSourceWrapperClassRefs(
            boostsAllowed,
            enhancementClassLookup,
            out mappedLabels,
            out unresolvedLabels);

        return classRefs.Length == 1;
    }

    private static int[] MergeEffectAndSpecificWrapperClassRefs(
        IReadOnlyCollection<int> effectClassRefs,
        IReadOnlyCollection<int> specificWrapperClassRefs,
        Enums.sEnhClass[] enhancementClasses,
        IReadOnlyDictionary<string, int> enhancementClassLookup)
    {
        if (effectClassRefs.Count == 0)
        {
            return specificWrapperClassRefs
                .Distinct()
                .OrderBy(id => id)
                .ToArray();
        }

        if (specificWrapperClassRefs.Count == 0)
        {
            return effectClassRefs
                .Distinct()
                .OrderBy(id => id)
                .ToArray();
        }

        var finalKeys = effectClassRefs
            .Select(classRef => GetEnhancementClassKey(classRef, enhancementClasses))
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var specificKeys = specificWrapperClassRefs
            .Select(classRef => GetEnhancementClassKey(classRef, enhancementClasses))
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        ApplySpecificWrapperOverride(finalKeys, specificKeys, "Debuff_Defense_Boost", "Buff_Defense_Boost");
        ApplySpecificWrapperOverride(finalKeys, specificKeys, "Debuff_ToHit_Boost", "Buff_ToHit_Boost");
        ApplySpecificWrapperOverride(finalKeys, specificKeys, "Res_Damage_Boost", "Damage_Boost");

        if (specificKeys.Contains("Slow_Boost"))
        {
            finalKeys.Remove("SpeedRunning_Boost");
            finalKeys.Remove("SpeedFlying_Boost");
            finalKeys.Remove("Jump_Boost");
            finalKeys.Add("Slow_Boost");
        }

        if (specificKeys.Contains("Interrupt_Boost") && !finalKeys.Contains("Interrupt_Boost"))
        {
            finalKeys.Add("Interrupt_Boost");
        }

        if (specificKeys.Contains("Intangible_Boost") && !finalKeys.Contains("Intangible_Boost"))
        {
            finalKeys.Add("Intangible_Boost");
        }

        return finalKeys
            .Select(key => enhancementClassLookup.TryGetValue(NormalizeLookupKey(key), out var classRef) ? classRef : -1)
            .Where(classRef => classRef >= 0)
            .Distinct()
            .OrderBy(classRef => classRef)
            .ToArray();
    }

    private static void ApplySpecificWrapperOverride(
        ISet<string> finalKeys,
        IReadOnlyCollection<string> specificKeys,
        string preferredKey,
        string displacedKey)
    {
        if (!specificKeys.Contains(preferredKey, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        finalKeys.Remove(displacedKey);
        finalKeys.Add(preferredKey);
    }

    private static string GetEnhancementClassKey(int classRef, Enums.sEnhClass[] enhancementClasses)
    {
        return classRef >= 0 && classRef < enhancementClasses.Length
            ? enhancementClasses[classRef].ClassID
            : string.Empty;
    }

    private static bool SourceLabelsContain(IEnumerable<string> sourceBoostAllowedLabels, string expectedLabel)
    {
        return sourceBoostAllowedLabels.Any(label =>
            string.Equals(NormalizeLookupKey(label), NormalizeLookupKey(expectedLabel), StringComparison.OrdinalIgnoreCase));
    }

    private static string FormatSourceBoostAllowedLabels(IReadOnlyCollection<string> labels)
    {
        return labels == null || labels.Count == 0
            ? "<none>"
            : string.Join(", ", labels);
    }

    private static string FormatEnhancementClassRefSummary(
        IReadOnlyCollection<int> classRefs,
        Enums.sEnhClass[] enhancementClasses)
    {
        if (classRefs == null || classRefs.Count == 0)
        {
            return "<empty>";
        }

        return string.Join(", ", classRefs.Select(classRef =>
        {
            if (classRef >= 0 && classRef < enhancementClasses.Length)
            {
                return $"{enhancementClasses[classRef].ID}:{enhancementClasses[classRef].ClassID}";
            }

            return classRef.ToString();
        }));
    }

    private static bool EnhancementEffectsEqual(Enums.sEffect[] existing, Enums.sEffect[] replacement)
    {
        if (ReferenceEquals(existing, replacement))
        {
            return true;
        }

        if (existing == null || replacement == null || existing.Length != replacement.Length)
        {
            return false;
        }

        for (var index = 0; index < existing.Length; index++)
        {
            var left = existing[index];
            var right = replacement[index];
            if (left.Mode != right.Mode ||
                left.BuffMode != right.BuffMode ||
                left.Enhance.ID != right.Enhance.ID ||
                left.Enhance.SubID != right.Enhance.SubID ||
                left.Schedule != right.Schedule ||
                Math.Abs(left.Multiplier - right.Multiplier) > 0.0001f ||
                !string.Equals(left.FX?.PowerFullName, right.FX?.PowerFullName, StringComparison.OrdinalIgnoreCase) ||
                left.FX?.EffectType != right.FX?.EffectType)
            {
                return false;
            }
        }

        return true;
    }

    private static void RecordEntityResolution<T>(
        string sourceKey,
        EntityReconciliationResult<T> resolution,
        OmniApplyResult applyResult,
        ICollection<string> auditDetails,
        ICollection<string> conflictDetails,
        bool isEnhancementSet,
        bool isRecipe,
        bool isSalvage) where T : class
    {
        var entityLabel = isSalvage ? "Salvage" : isRecipe ? "Recipe" : isEnhancementSet ? "Enhancement set" : "Enhancement";

        switch (resolution.Kind)
        {
            case EnhancementReconciliationMatchKind.Exact:
                IncrementResolutionCounter(applyResult, isEnhancementSet, isRecipe, isSalvage, exact: true, alias: false, linked: false, fallback: false, ambiguous: false);
                applyResult.AddLimited(auditDetails, $"{entityLabel} {sourceKey}: matched existing '{resolution.MatchedKey}' by exact identity.");
                break;
            case EnhancementReconciliationMatchKind.Alias:
                IncrementResolutionCounter(applyResult, isEnhancementSet, isRecipe, isSalvage, exact: false, alias: true, linked: false, fallback: false, ambiguous: false);
                applyResult.AddLimited(auditDetails, $"{entityLabel} {sourceKey}: matched existing '{resolution.MatchedKey}' via alias crosswalk.");
                break;
            case EnhancementReconciliationMatchKind.LinkedPower:
                IncrementResolutionCounter(applyResult, isEnhancementSet, isRecipe, isSalvage, exact: false, alias: false, linked: true, fallback: false, ambiguous: false);
                applyResult.AddLimited(auditDetails, $"{entityLabel} {sourceKey}: matched existing '{resolution.MatchedKey}' via linked boost power.");
                break;
            case EnhancementReconciliationMatchKind.Fallback:
                IncrementResolutionCounter(applyResult, isEnhancementSet, isRecipe, isSalvage, exact: false, alias: false, linked: false, fallback: true, ambiguous: false);
                applyResult.AddLimited(auditDetails, $"{entityLabel} {sourceKey}: matched existing '{resolution.MatchedKey}' via fallback identity.");
                break;
            case EnhancementReconciliationMatchKind.Ambiguous:
                IncrementResolutionCounter(applyResult, isEnhancementSet, isRecipe, isSalvage, exact: false, alias: false, linked: false, fallback: false, ambiguous: true);
                applyResult.AddLimited(conflictDetails, $"{entityLabel} {sourceKey}: ambiguous candidates [{string.Join(", ", resolution.CandidateKeys)}].");
                break;
            case EnhancementReconciliationMatchKind.None:
                applyResult.AddLimited(auditDetails, $"{entityLabel} {sourceKey}: no existing match; would create new record.");
                break;
        }
    }

    private static void IncrementResolutionCounter(
        OmniApplyResult applyResult,
        bool isEnhancementSet,
        bool isRecipe,
        bool isSalvage,
        bool exact,
        bool alias,
        bool linked,
        bool fallback,
        bool ambiguous)
    {
        if (isSalvage)
        {
            if (exact) applyResult.SalvageExactMatches++;
            if (alias) applyResult.SalvageAliasMatches++;
            if (fallback) applyResult.SalvageFallbackMatches++;
            if (ambiguous) applyResult.SalvageAmbiguousMatches++;
            return;
        }

        if (isRecipe)
        {
            if (exact) applyResult.RecipeExactMatches++;
            if (alias) applyResult.RecipeAliasMatches++;
            if (fallback) applyResult.RecipeFallbackMatches++;
            if (ambiguous) applyResult.RecipeAmbiguousMatches++;
            return;
        }

        if (isEnhancementSet)
        {
            if (exact) applyResult.EnhancementSetsExactMatches++;
            if (alias) applyResult.EnhancementSetsAliasMatches++;
            if (fallback) applyResult.EnhancementSetsFallbackMatches++;
            if (ambiguous) applyResult.EnhancementSetsAmbiguousMatches++;
            return;
        }

        if (exact) applyResult.EnhancementExactMatches++;
        if (alias) applyResult.EnhancementAliasMatches++;
        if (linked) applyResult.EnhancementLinkedPowerMatches++;
        if (fallback) applyResult.EnhancementFallbackMatches++;
        if (ambiguous) applyResult.EnhancementAmbiguousMatches++;
    }

    private static void RecordEntityResolution<T>(
        string sourceKey,
        EntityReconciliationResult<T> resolution,
        OmniImportReport report,
        bool isEnhancementSet,
        bool isRecipe,
        bool isSalvage) where T : class
    {
        var entityLabel = isSalvage ? "Salvage" : isRecipe ? "Recipe" : isEnhancementSet ? "Enhancement set" : "Enhancement";

        switch (resolution.Kind)
        {
            case EnhancementReconciliationMatchKind.Exact:
                IncrementResolutionCounter(report, isEnhancementSet, isRecipe, isSalvage, exact: true, alias: false, linked: false, fallback: false, ambiguous: false);
                report.AddLimited(report.EnhancementReconciliationAudit, $"{entityLabel} {sourceKey}: would update existing '{resolution.MatchedKey}' by exact identity.");
                break;
            case EnhancementReconciliationMatchKind.Alias:
                IncrementResolutionCounter(report, isEnhancementSet, isRecipe, isSalvage, exact: false, alias: true, linked: false, fallback: false, ambiguous: false);
                report.AddLimited(report.EnhancementReconciliationAudit, $"{entityLabel} {sourceKey}: would update existing '{resolution.MatchedKey}' via alias crosswalk.");
                break;
            case EnhancementReconciliationMatchKind.LinkedPower:
                IncrementResolutionCounter(report, isEnhancementSet, isRecipe, isSalvage, exact: false, alias: false, linked: true, fallback: false, ambiguous: false);
                report.AddLimited(report.EnhancementReconciliationAudit, $"{entityLabel} {sourceKey}: would update existing '{resolution.MatchedKey}' via linked boost power.");
                break;
            case EnhancementReconciliationMatchKind.Fallback:
                IncrementResolutionCounter(report, isEnhancementSet, isRecipe, isSalvage, exact: false, alias: false, linked: false, fallback: true, ambiguous: false);
                report.AddLimited(report.EnhancementReconciliationAudit, $"{entityLabel} {sourceKey}: would update existing '{resolution.MatchedKey}' via fallback identity.");
                break;
            case EnhancementReconciliationMatchKind.Ambiguous:
                IncrementResolutionCounter(report, isEnhancementSet, isRecipe, isSalvage, exact: false, alias: false, linked: false, fallback: false, ambiguous: true);
                report.AddLimited(report.EnhancementReconciliationConflicts, $"{entityLabel} {sourceKey}: ambiguous candidates [{string.Join(", ", resolution.CandidateKeys)}].");
                break;
            case EnhancementReconciliationMatchKind.None:
                report.AddLimited(report.EnhancementReconciliationAudit, $"{entityLabel} {sourceKey}: would create new record.");
                break;
        }
    }

    private static void IncrementResolutionCounter(
        OmniImportReport report,
        bool isEnhancementSet,
        bool isRecipe,
        bool isSalvage,
        bool exact,
        bool alias,
        bool linked,
        bool fallback,
        bool ambiguous)
    {
        if (isSalvage)
        {
            if (exact) report.SalvageExactMatchesDryRun++;
            if (alias) report.SalvageAliasMatchesDryRun++;
            if (fallback) report.SalvageFallbackMatchesDryRun++;
            if (ambiguous) report.SalvageAmbiguousMatchesDryRun++;
            return;
        }

        if (isRecipe)
        {
            if (exact) report.RecipeExactMatchesDryRun++;
            if (alias) report.RecipeAliasMatchesDryRun++;
            if (fallback) report.RecipeFallbackMatchesDryRun++;
            if (ambiguous) report.RecipeAmbiguousMatchesDryRun++;
            return;
        }

        if (isEnhancementSet)
        {
            if (exact) report.EnhancementSetExactMatchesDryRun++;
            if (alias) report.EnhancementSetAliasMatchesDryRun++;
            if (fallback) report.EnhancementSetFallbackMatchesDryRun++;
            if (ambiguous) report.EnhancementSetAmbiguousMatchesDryRun++;
            return;
        }

        if (exact) report.EnhancementExactMatchesDryRun++;
        if (alias) report.EnhancementAliasMatchesDryRun++;
        if (linked) report.EnhancementLinkedPowerMatchesDryRun++;
        if (fallback) report.EnhancementFallbackMatchesDryRun++;
        if (ambiguous) report.EnhancementAmbiguousMatchesDryRun++;
    }

    private static void RecordEnhancementCreateInvestigation(
        NormalizedEnhancementSource source,
        EnhancementCreateInvestigationResult investigation,
        OmniImportReport report)
    {
        switch (investigation.Kind)
        {
            case EnhancementCreateInvestigationKind.LikelyNew:
                report.EnhancementCreateLikelyNewDryRun++;
                report.AddLimited(report.EnhancementIdentityLikelyNewSamples,
                    $"Enhancement {source.Name}: likely new. boost='{CanonicalizeOmniFullName(source.PowerFullName)}', recipe='{FirstNonEmpty(source.RecipeCanonicalId, source.RecipeStorageKey, source.RecipeKey)}', set='{FirstNonEmpty(source.EnhancementSetCanonicalId, source.EnhancementSetStorageKey, source.EnhancementSetName)}'.");
                break;
            case EnhancementCreateInvestigationKind.ShouldHaveMatchedByBoostPower:
                report.EnhancementCreateShouldMatchBoostPowerDryRun++;
                report.AddLimited(report.EnhancementIdentityInvestigation,
                    $"Enhancement {source.Name}: would create, but linked boost power '{CanonicalizeOmniFullName(source.PowerFullName)}' suggests existing '{string.Join(", ", investigation.CandidateKeys)}'.");
                break;
            case EnhancementCreateInvestigationKind.ShouldHaveMatchedByRecipe:
                report.EnhancementCreateShouldMatchRecipeDryRun++;
                report.AddLimited(report.EnhancementIdentityInvestigation,
                    $"Enhancement {source.Name}: would create, but embedded recipe '{investigation.Anchor}' suggests existing '{string.Join(", ", investigation.CandidateKeys)}'.");
                break;
            case EnhancementCreateInvestigationKind.ShouldHaveMatchedBySetMembership:
                report.EnhancementCreateShouldMatchSetMembershipDryRun++;
                report.AddLimited(report.EnhancementIdentityInvestigation,
                    source.IsClassicOriginVariant
                        ? $"Enhancement {source.Name}: would create, but classic fold key '{investigation.Anchor}' suggests existing '{string.Join(", ", investigation.CandidateKeys)}'."
                        : $"Enhancement {source.Name}: would create, but set membership '{investigation.Anchor}' suggests existing '{string.Join(", ", investigation.CandidateKeys)}'.");
                break;
            case EnhancementCreateInvestigationKind.Ambiguous:
                report.AddLimited(report.EnhancementIdentityInvestigation,
                    $"Enhancement {source.Name}: strongest alternate match is ambiguous via '{investigation.Anchor}' -> [{string.Join(", ", investigation.CandidateKeys)}].");
                break;
            case EnhancementCreateInvestigationKind.NoPlausibleExistingMatch:
                report.EnhancementCreateNoPlausibleExistingMatchDryRun++;
                if (investigation.CandidateKeys.Length > 0)
                {
                    report.AddLimited(report.EnhancementIdentityInvestigation,
                        $"Enhancement {source.Name}: no strong identity miss found; only loose fallback via '{investigation.Anchor}' -> '{string.Join(", ", investigation.CandidateKeys)}'.");
                }
                else
                {
                    report.AddLimited(report.EnhancementIdentityInvestigation,
                        $"Enhancement {source.Name}: no plausible existing enhancement match found from boost/recipe/set anchors.");
                }
                break;
        }
    }

    private static void RecordEnhancementCreateValidation(
        NormalizedEnhancementSource source,
        EnhancementCreateInvestigationResult investigation,
        OmniImportReport report)
    {
        if (investigation.Kind != EnhancementCreateInvestigationKind.LikelyNew)
        {
            return;
        }

        if (source.Attuned)
        {
            report.EnhancementCreateLikelyNewAttunedDryRun++;
        }

        if (source.SuperiorAttuned)
        {
            report.EnhancementCreateLikelyNewSuperiorAttunedDryRun++;
        }

        if (!string.IsNullOrWhiteSpace(source.EnhancementSetName) ||
            !string.IsNullOrWhiteSpace(source.EnhancementSetCanonicalId) ||
            !string.IsNullOrWhiteSpace(source.EnhancementSetStorageKey))
        {
            report.EnhancementCreateLikelyNewSetBackedDryRun++;
        }

        if (!string.IsNullOrWhiteSpace(source.RecipeKey) ||
            !string.IsNullOrWhiteSpace(source.RecipeCanonicalId) ||
            !string.IsNullOrWhiteSpace(source.RecipeStorageKey))
        {
            report.EnhancementCreateLikelyNewRecipeBackedDryRun++;
        }

        if (source.IsClassicOriginVariant)
        {
            report.EnhancementCreateLikelyNewClassicDryRun++;
        }

        var mappedType = MapEnhancementType(source);
        if (mappedType == Enums.eType.SpecialO)
        {
            report.EnhancementCreateLikelyNewSpecialOriginDryRun++;
        }
        else if (mappedType is Enums.eType.InventO or Enums.eType.SetO)
        {
            report.EnhancementCreateLikelyNewInventionDryRun++;
        }

        var flags = new List<string>();
        if (source.Attuned)
        {
            flags.Add(source.SuperiorAttuned ? "superior-attuned" : "attuned");
        }

        if (!string.IsNullOrWhiteSpace(source.EnhancementSetName))
        {
            flags.Add($"set={FirstNonEmpty(source.EnhancementSetCanonicalId, source.EnhancementSetStorageKey, source.EnhancementSetName)}");
        }

        if (!string.IsNullOrWhiteSpace(source.RecipeKey))
        {
            flags.Add($"recipe={FirstNonEmpty(source.RecipeCanonicalId, source.RecipeStorageKey, source.RecipeKey)}");
        }

        if (source.IsClassicOriginVariant)
        {
            flags.Add($"classic={source.ClassicFoldKey}");
        }

        flags.Add($"type={source.EnhancementType}");
        report.AddLimited(report.EnhancementIdentityLikelyNewSamples,
            $"Enhancement {source.Name}: {string.Join(", ", flags)}; boost='{CanonicalizeOmniFullName(source.PowerFullName)}'.",
            limit: 80);
    }

    private static void FinalizeEnhancementCreateValidationSummary(OmniImportReport report)
    {
        report.AddLimited(report.EnhancementCreateValidationSummary,
            $"Remaining creates after reconciliation: {report.EnhancementWouldCreateDryRun}.");
        report.AddLimited(report.EnhancementCreateValidationSummary,
            $"Likely-new creates: {report.EnhancementCreateLikelyNewDryRun}; suspicious rescue cases: boost={report.EnhancementCreateShouldMatchBoostPowerDryRun}, recipe={report.EnhancementCreateShouldMatchRecipeDryRun}, set={report.EnhancementCreateShouldMatchSetMembershipDryRun}, no-plausible={report.EnhancementCreateNoPlausibleExistingMatchDryRun}.");
        report.AddLimited(report.EnhancementCreateValidationSummary,
            $"Likely-new profile: attuned={report.EnhancementCreateLikelyNewAttunedDryRun}, superior-attuned={report.EnhancementCreateLikelyNewSuperiorAttunedDryRun}, set-backed={report.EnhancementCreateLikelyNewSetBackedDryRun}, recipe-backed={report.EnhancementCreateLikelyNewRecipeBackedDryRun}, classic={report.EnhancementCreateLikelyNewClassicDryRun}, special-origin={report.EnhancementCreateLikelyNewSpecialOriginDryRun}, invention-or-set={report.EnhancementCreateLikelyNewInventionDryRun}.");
    }

    private static void RecordRecipeCreateInvestigation(
        NormalizedRecipeSource source,
        RecipeCreateInvestigationResult investigation,
        OmniImportReport report)
    {
        switch (investigation.Kind)
        {
            case RecipeCreateInvestigationKind.LikelyNew:
                report.RecipeCreateLikelyNewDryRun++;
                report.AddLimited(report.RecipeIdentityLikelyNewSamples,
                    $"Recipe {source.Name}: likely new. reward='{FirstNonEmpty(source.EnhancementRewardUid, source.EnhancementReward)}', canonical='{FirstNonEmpty(source.CanonicalId, source.StorageKey, source.SourceKey)}', variants={source.LevelVariants.Count}.");
                break;
            case RecipeCreateInvestigationKind.ShouldHaveMatchedByRewardIdentity:
                report.RecipeCreateShouldMatchRewardIdentityDryRun++;
                report.AddLimited(report.RecipeIdentityInvestigation,
                    $"Recipe {source.Name}: would create, but reward identity '{investigation.Anchor}' suggests existing '{string.Join(", ", investigation.CandidateKeys)}'.");
                break;
            case RecipeCreateInvestigationKind.ShouldHaveMatchedByCanonicalStorageIdentity:
                report.RecipeCreateShouldMatchCanonicalStorageIdentityDryRun++;
                report.AddLimited(report.RecipeIdentityInvestigation,
                    $"Recipe {source.Name}: would create, but canonical/storage identity '{investigation.Anchor}' suggests existing '{string.Join(", ", investigation.CandidateKeys)}'.");
                break;
            case RecipeCreateInvestigationKind.Ambiguous:
                report.AddLimited(report.RecipeIdentityInvestigation,
                    $"Recipe {source.Name}: strongest alternate match is ambiguous via '{investigation.Anchor}' -> [{string.Join(", ", investigation.CandidateKeys)}].");
                break;
            case RecipeCreateInvestigationKind.NoPlausibleExistingMatch:
                report.RecipeCreateNoPlausibleExistingMatchDryRun++;
                if (investigation.CandidateKeys.Length > 0)
                {
                    report.AddLimited(report.RecipeIdentityInvestigation,
                        $"Recipe {source.Name}: no strong reward/canonical miss found; only loose fallback via '{investigation.Anchor}' -> '{string.Join(", ", investigation.CandidateKeys)}'.");
                }
                else
                {
                    report.AddLimited(report.RecipeIdentityInvestigation,
                        $"Recipe {source.Name}: no plausible existing recipe match found from reward/canonical anchors.");
                }
                break;
        }
    }

    private static string FirstNonEmpty(params string[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
    }

    private void InspectEnhancementReconciliationDryRun(NormalizedEnhancementImportData normalizedData, OmniImportReport report)
    {
        if (DatabaseAPI.Database == null)
        {
            return;
        }

        var metadata = CloneEnhancementImportMetadata(DatabaseAPI.Database.EnhancementImportMetadata);
        var powerLookup = BuildPowerLookup(DatabaseAPI.Database);
        var reconciliationIndex = new EnhancementImportReconciliationIndex(DatabaseAPI.Database, metadata, powerLookup);

        foreach (var source in normalizedData.Salvage.Where(source => !string.IsNullOrWhiteSpace(source.Name)))
        {
            var resolution = reconciliationIndex.ResolveSalvage(source);
            RecordEntityResolution(source.Name, resolution, report, isEnhancementSet: false, isRecipe: false, isSalvage: true);
            if (resolution.Found)
            {
                report.SalvageWouldUpdateDryRun++;
            }
            else if (!resolution.IsAmbiguous)
            {
                report.SalvageWouldCreateDryRun++;
            }
        }

        foreach (var source in normalizedData.Recipes.Where(source => !string.IsNullOrWhiteSpace(source.Name)))
        {
            var resolution = reconciliationIndex.ResolveRecipe(source);
            RecordEntityResolution(source.Name, resolution, report, isEnhancementSet: false, isRecipe: true, isSalvage: false);
            if (resolution.Found)
            {
                report.RecipeWouldUpdateDryRun++;
            }
            else if (!resolution.IsAmbiguous)
            {
                report.RecipeWouldCreateDryRun++;
                RecordRecipeCreateInvestigation(source, reconciliationIndex.InvestigateUnmatchedRecipe(source), report);
            }
        }

        foreach (var source in normalizedData.EnhancementSets.Where(source => !string.IsNullOrWhiteSpace(source.Name)))
        {
            var resolution = reconciliationIndex.ResolveEnhancementSet(source);
            RecordEntityResolution(source.Name, resolution, report, isEnhancementSet: true, isRecipe: false, isSalvage: false);
            if (resolution.Found)
            {
                report.EnhancementSetWouldUpdateDryRun++;
            }
            else if (!resolution.IsAmbiguous)
            {
                report.EnhancementSetWouldCreateDryRun++;
            }

            foreach (var powerName in source.Bonuses
                         .SelectMany(GetSetBonusPowerNames)
                         .Where(value => !string.IsNullOrWhiteSpace(value))
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var powerResolution = reconciliationIndex.ResolvePower(powerName, setBonusPower: true);
                if (powerResolution.Kind == EnhancementReconciliationMatchKind.Alias)
                {
                    report.EnhancementSetBonusLinksAliasDryRun++;
                }
                else if (powerResolution.Kind == EnhancementReconciliationMatchKind.Fallback)
                {
                    report.EnhancementSetBonusLinksFallbackDryRun++;
                }
                else if (powerResolution.IsAmbiguous)
                {
                    report.AddLimited(report.EnhancementReconciliationConflicts,
                        $"Set bonus power {CanonicalizeOmniFullName(powerName)}: ambiguous existing matches [{string.Join(", ", powerResolution.CandidateKeys)}].");
                }
            }
        }

        foreach (var source in normalizedData.Enhancements.Where(source => !string.IsNullOrWhiteSpace(source.Name)))
        {
            var resolution = reconciliationIndex.ResolveEnhancement(source);
            RecordEntityResolution(source.Name, resolution, report, isEnhancementSet: false, isRecipe: false, isSalvage: false);
            if (resolution.Found)
            {
                report.EnhancementWouldUpdateDryRun++;
            }
            else if (!resolution.IsAmbiguous)
            {
                report.EnhancementWouldCreateDryRun++;
                var investigation = reconciliationIndex.InvestigateUnmatchedEnhancement(source);
                RecordEnhancementCreateInvestigation(source, investigation, report);
                RecordEnhancementCreateValidation(source, investigation, report);
            }

            if (string.IsNullOrWhiteSpace(source.PowerFullName))
            {
                continue;
            }

            var powerResolution = reconciliationIndex.ResolvePower(source.PowerFullName, setBonusPower: false);
            if (powerResolution.Kind == EnhancementReconciliationMatchKind.Alias)
            {
                report.EnhancementBoostPowerLinksAliasDryRun++;
            }
            else if (powerResolution.Kind == EnhancementReconciliationMatchKind.Fallback)
            {
                report.EnhancementBoostPowerLinksFallbackDryRun++;
            }
            else if (powerResolution.IsAmbiguous)
            {
                report.AddLimited(report.EnhancementReconciliationConflicts,
                    $"Boost power {CanonicalizeOmniFullName(source.PowerFullName)}: ambiguous existing matches [{string.Join(", ", powerResolution.CandidateKeys)}].");
            }
        }

        FinalizeEnhancementCreateValidationSummary(report);
    }

    private static Enums.eBuffDebuff MapBuffMode(IEffect effect)
    {
        if (effect == null)
        {
            return Enums.eBuffDebuff.Any;
        }

        if (effect.Scale < 0 || effect.nMagnitude < 0)
        {
            return Enums.eBuffDebuff.DeBuffOnly;
        }

        return Enums.eBuffDebuff.Any;
    }

    private static Enums.eEnhance MapEnhanceFromEffect(IEffect effect)
    {
        if (effect == null)
        {
            return Enums.eEnhance.None;
        }

        var sourceEffectType = effect.EffectType;
        if ((sourceEffectType == Enums.eEffectType.Enhancement || sourceEffectType == Enums.eEffectType.ResEffect) &&
            effect.ETModifies != Enums.eEffectType.None)
        {
            sourceEffectType = effect.ETModifies;
        }

        return sourceEffectType switch
        {
            Enums.eEffectType.Accuracy => Enums.eEnhance.Accuracy,
            Enums.eEffectType.Damage or Enums.eEffectType.DamageBuff => Enums.eEnhance.Damage,
            Enums.eEffectType.Defense => Enums.eEnhance.Defense,
            Enums.eEffectType.EnduranceDiscount => Enums.eEnhance.EnduranceDiscount,
            Enums.eEffectType.Endurance => Enums.eEnhance.Endurance,
            Enums.eEffectType.Fly or Enums.eEffectType.SpeedFlying or Enums.eEffectType.MaxFlySpeed => Enums.eEnhance.SpeedFlying,
            Enums.eEffectType.Heal => Enums.eEnhance.Heal,
            Enums.eEffectType.HitPoints => Enums.eEnhance.HitPoints,
            Enums.eEffectType.InterruptTime => Enums.eEnhance.Interrupt,
            Enums.eEffectType.JumpHeight => Enums.eEnhance.JumpHeight,
            Enums.eEffectType.SpeedJumping or Enums.eEffectType.MaxJumpSpeed => Enums.eEnhance.SpeedJumping,
            Enums.eEffectType.Mez => Enums.eEnhance.Mez,
            Enums.eEffectType.Range => Enums.eEnhance.Range,
            Enums.eEffectType.RechargeTime => Enums.eEnhance.RechargeTime,
            Enums.eEffectType.Recovery => Enums.eEnhance.Recovery,
            Enums.eEffectType.Regeneration => Enums.eEnhance.Regeneration,
            Enums.eEffectType.Resistance => Enums.eEnhance.Resistance,
            Enums.eEffectType.SpeedRunning or Enums.eEffectType.MaxRunSpeed => Enums.eEnhance.SpeedRunning,
            Enums.eEffectType.ToHit => Enums.eEnhance.ToHit,
            Enums.eEffectType.Slow => Enums.eEnhance.Slow,
            Enums.eEffectType.Absorb => Enums.eEnhance.Absorb,
            _ => Enums.eEnhance.None
        };
    }

    private static void FinalizeEnhancementImport(
        IDatabase database,
        IReadOnlyCollection<NormalizedEnhancementSetSource> setDefinitions,
        EnhancementImportMetadata metadata,
        EnhancementImportReconciliationIndex reconciliationIndex,
        OmniApplyResult applyResult)
    {
        NormalizeEnhancementClassReferences(database, applyResult);
        DeactivateExcludedAndFoldedEnhancements(database, metadata, applyResult);

        if (ReferenceEquals(DatabaseAPI.Database, database))
        {
            DatabaseAPI.AssignRecipeSalvageIDs();
            DatabaseAPI.AssignRecipeIDs();
            AssignEnhancementSetMembership(database);
        }
        else
        {
            AssignRecipeSalvageIds(database);
            AssignRecipeIds(database);
            AssignEnhancementSetMembership(database);
        }

        ApplyEnhancementSetSpecialBonuses(database, setDefinitions, metadata, reconciliationIndex, applyResult);

        applyResult.AddLimited(applyResult.EnhancementImportDetails,
            "Enhancement import finalized recipe/salvage links after Omni upsert.");
    }

    private static void DeactivateExcludedAndFoldedEnhancements(
        IDatabase database,
        EnhancementImportMetadata metadata,
        OmniApplyResult applyResult)
    {
        if (database.Enhancements == null || database.Enhancements.Length == 0)
        {
            return;
        }

        var deactivatedClassic = 0;
        var deactivatedExcluded = 0;

        foreach (var enhancement in database.Enhancements.Where(enhancement => enhancement != null))
        {
            var isExcludedOmniHack = IsOmniHackEnhancementRecord(enhancement);
            var isFoldedClassicVariant =
                metadata.ClassicEnhancementCanonicalByName.TryGetValue(enhancement.UID ?? string.Empty, out var canonicalName) &&
                !string.IsNullOrWhiteSpace(canonicalName) &&
                !string.Equals(canonicalName, enhancement.UID, StringComparison.OrdinalIgnoreCase);

            if (!isExcludedOmniHack && !isFoldedClassicVariant)
            {
                continue;
            }

            if (isExcludedOmniHack)
            {
                deactivatedExcluded++;
            }
            else
            {
                deactivatedClassic++;
            }

            enhancement.TypeID = Enums.eType.None;
            enhancement.SubTypeID = 0;
            enhancement.ClassID = Array.Empty<int>();
        }

        if (deactivatedExcluded > 0)
        {
            applyResult.AddLimited(applyResult.EnhancementImportDetails,
                $"Deactivated {deactivatedExcluded} excluded OmniHack enhancement record(s) left from prior imports.");
        }

        if (deactivatedClassic > 0)
        {
            applyResult.AddLimited(applyResult.EnhancementImportDetails,
                $"Deactivated {deactivatedClassic} stale folded classic enhancement variant record(s) left from prior imports.");
        }
    }

    private static bool IsOmniHackEnhancementRecord(IEnhancement enhancement)
    {
        return string.Equals(enhancement.Name, "OmniHack", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(enhancement.UID, "Yins_Omni", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(enhancement.ShortName, "Yins_Omni", StringComparison.OrdinalIgnoreCase);
    }

    private static void ValidateClassicEnhancementPresentationMetadata(
        IEnumerable<NormalizedEnhancementSource> sources,
        EnhancementImportMetadata metadata,
        OmniApplyResult applyResult)
    {
        foreach (var source in sources.Where(source => source.IsClassicOriginVariant))
        {
            var expectedCount = 1 + source.FoldedClassicVariants.Count;
            if (expectedCount <= 1)
            {
                continue;
            }

            var canonicalUid = metadata.ClassicEnhancementCanonicalByName.TryGetValue(source.Name, out var mappedCanonicalUid) &&
                               !string.IsNullOrWhiteSpace(mappedCanonicalUid)
                ? mappedCanonicalUid
                : source.Name;

            if (!metadata.ClassicEnhancementSourceNamesByCanonicalName.TryGetValue(canonicalUid, out var sourceNames))
            {
                applyResult.ClassicEnhancementMetadataWarnings++;
                applyResult.AddLimited(applyResult.ClassicEnhancementPresentationDetails,
                    $"{source.Name}: expected {expectedCount} classic source rows, but no presentation metadata group was recorded for canonical '{canonicalUid}'.");
                continue;
            }

            var actualCount = sourceNames
                .Select(sourceName => metadata.ClassicEnhancementSourcesByName.TryGetValue(sourceName, out var variant) ? variant : null)
                .Count(variant => variant != null);

            if (actualCount >= expectedCount)
            {
                continue;
            }

            applyResult.ClassicEnhancementMetadataWarnings++;
            applyResult.AddLimited(applyResult.ClassicEnhancementPresentationDetails,
                $"{source.Name}: expected {expectedCount} classic source rows, but only {actualCount} presentation variants were recorded.");
        }
    }

    private static NormalizedEnhancementSource GetCanonicalClassicSurfaceSource(NormalizedEnhancementSource source)
    {
        if (!source.IsClassicOriginVariant)
        {
            return source;
        }

        return new[] { source }
            .Concat(source.FoldedClassicVariants)
            .OrderByDescending(static item => IsGenericClassicIdentity(item) ? 1 : 0)
            .ThenByDescending(static item => IsTrainingClassicTier(item.EnhancementType) ? 1 : 0)
            .ThenBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault() ?? source;
    }

    private static bool TryGetLegacyCanonicalClassicUid(
        Enhancement enhancement,
        EnhancementImportMetadata metadata,
        out string canonicalUid)
    {
        canonicalUid = string.Empty;
        return enhancement != null &&
               metadata?.ClassicEnhancementCanonicalByName != null &&
               metadata.ClassicEnhancementCanonicalByName.TryGetValue(enhancement.UID ?? string.Empty, out canonicalUid) &&
               !string.IsNullOrWhiteSpace(canonicalUid);
    }

    private static bool IsTrainingClassicTier(string? enhancementType)
    {
        return string.Equals(enhancementType, "TO", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(enhancementType, "TR", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsGenericClassicIdentity(NormalizedEnhancementSource source)
    {
        var candidate = FirstNonEmpty(source?.SourceKey, source?.StorageKey, source?.Name);
        return candidate.StartsWith("generic_", StringComparison.OrdinalIgnoreCase) ||
               candidate.Contains("_generic_", StringComparison.OrdinalIgnoreCase);
    }

    private static ClassicEnhancementSourceVariantMetadata BuildClassicEnhancementSourceVariantMetadata(
        NormalizedEnhancementSource source)
    {
        var normalizedTier = ClassicEnhancementVariantResolver.NormalizeTier(source.EnhancementType);
        var descriptor = ClassicEnhancementVariantResolver.BuildOriginDescriptor(
            source.Name,
            source.SourceKey,
            string.Empty,
            normalizedTier);

        return new ClassicEnhancementSourceVariantMetadata
        {
            SourceKey = source.SourceKey,
            Name = source.Name,
            Tier = source.EnhancementType,
            NormalizedTier = normalizedTier,
            DisplayName = source.DisplayName,
            Icon = source.Icon,
            Flavor = ClassicEnhancementVariantResolver.BuildFlavor(descriptor, normalizedTier),
            PrimaryOrigin = descriptor.PrimaryOrigin,
            SecondaryOrigin = descriptor.SecondaryOrigin,
            HasExplicitPrimaryOrigin = descriptor.HasExplicitPrimaryOrigin,
            CompatibleOrigins = descriptor.CompatibleOrigins.ToList()
        };
    }

    private static void NormalizeEnhancementClassReferences(IDatabase database, OmniApplyResult applyResult)
    {
        var enhancementClasses = database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>();
        if (enhancementClasses.Length == 0 || database.Enhancements == null)
        {
            return;
        }

        var idToIndex = enhancementClasses
            .Select((enhClass, index) => new { enhClass.ID, index })
            .GroupBy(item => item.ID)
            .ToDictionary(group => group.Key, group => group.First().index);

        var normalizedCount = 0;
        foreach (var enhancement in database.Enhancements.Where(enhancement => enhancement != null))
        {
            var existing = enhancement.ClassID ?? Array.Empty<int>();
            if (existing.Length == 0)
            {
                continue;
            }

            var normalized = existing
                .Select(classRef =>
                {
                    if (classRef >= 0 && classRef < enhancementClasses.Length)
                    {
                        return classRef;
                    }

                    return idToIndex.TryGetValue(classRef, out var index)
                        ? index
                        : -1;
                })
                .Where(index => index >= 0 && index < enhancementClasses.Length)
                .Distinct()
                .OrderBy(index => index)
                .ToArray();

            if (existing.SequenceEqual(normalized))
            {
                continue;
            }

            enhancement.ClassID = normalized;
            normalizedCount++;
        }

        if (normalizedCount > 0)
        {
            applyResult.AddLimited(applyResult.EnhancementImportDetails,
                $"Normalized enhancement class references for {normalizedCount} enhancement records.");
        }
    }

    private static void ApplyEnhancementSetSpecialBonuses(
        IDatabase database,
        IReadOnlyCollection<NormalizedEnhancementSetSource> setDefinitions,
        EnhancementImportMetadata metadata,
        EnhancementImportReconciliationIndex reconciliationIndex,
        OmniApplyResult applyResult)
    {
        if (database.EnhancementSets == null || database.EnhancementSets.Count == 0)
        {
            return;
        }

        var setsByUid = database.EnhancementSets
            .Where(set => set != null && !string.IsNullOrWhiteSpace(set.Uid))
            .ToDictionary(set => set.Uid, StringComparer.OrdinalIgnoreCase);

        foreach (var source in setDefinitions.Where(source => !string.IsNullOrWhiteSpace(source.Name)))
        {
            if (!setsByUid.TryGetValue(source.Name, out var set))
            {
                continue;
            }

            var replacement = BuildSpecialBonusItems(set, source, database, reconciliationIndex, metadata, applyResult);
            if (BonusItemsEqual(set.SpecialBonus, replacement))
            {
                continue;
            }

            set.SpecialBonus = replacement;
            applyResult.EnhancementSetsUpdated++;
            applyResult.AddLimited(applyResult.EnhancementImportDetails,
                $"Enhancement set {source.Name}: refreshed special/member bonus links.");
        }
    }

    private static void AssignRecipeSalvageIds(IDatabase database)
    {
        var salvageByName = (database.Salvage ?? Array.Empty<Salvage>())
            .Select((salvage, index) => new { salvage, index })
            .ToDictionary(item => item.salvage.InternalName, item => item.index, StringComparer.OrdinalIgnoreCase);

        foreach (var recipe in database.Recipes ?? Array.Empty<Recipe>())
        {
            foreach (var entry in recipe.Item ?? Array.Empty<Recipe.RecipeEntry>())
            {
                for (var index = 0; index < entry.Salvage.Length; index++)
                {
                    entry.SalvageIdx[index] = salvageByName.TryGetValue(entry.Salvage[index], out var salvageIdx)
                        ? salvageIdx
                        : -1;
                }
            }
        }
    }

    private static void AssignRecipeIds(IDatabase database)
    {
        var recipeByName = (database.Recipes ?? Array.Empty<Recipe>())
            .Select((recipe, index) => new { recipe, index })
            .ToDictionary(item => item.recipe.InternalName, item => item.index, StringComparer.OrdinalIgnoreCase);

        foreach (var enhancement in database.Enhancements ?? Array.Empty<IEnhancement>())
        {
            enhancement.RecipeIDX = -1;
            if (string.IsNullOrWhiteSpace(enhancement.RecipeName))
            {
                continue;
            }

            if (recipeByName.TryGetValue(enhancement.RecipeName, out var recipeIndex))
            {
                enhancement.RecipeIDX = recipeIndex;
            }
        }
    }

    private static void AssignEnhancementSetMembership(IDatabase database)
    {
        foreach (var set in database.EnhancementSets)
        {
            set.Enhancements = Array.Empty<int>();
        }

        var setIndexes = database.EnhancementSets
            .Select((set, index) => new { set, index })
            .Where(item => !string.IsNullOrWhiteSpace(item.set.Uid))
            .ToDictionary(item => item.set.Uid, item => item.index, StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < database.Enhancements.Length; index++)
        {
            var enhancement = database.Enhancements[index];
            if (enhancement.TypeID != Enums.eType.SetO || string.IsNullOrWhiteSpace(enhancement.UIDSet))
            {
                continue;
            }

            if (!setIndexes.TryGetValue(enhancement.UIDSet, out var setIndex))
            {
                enhancement.nIDSet = -1;
                continue;
            }

            enhancement.nIDSet = setIndex;
            var set = database.EnhancementSets[setIndex];
            Array.Resize(ref set.Enhancements, set.Enhancements.Length + 1);
            set.Enhancements[^1] = index;
        }
    }

    private static Recipe.RecipeRarity MapRecipeRarity(string rarityName)
    {
        return rarityName.Trim().ToLowerInvariant() switch
        {
            "common" => Recipe.RecipeRarity.Common,
            "uncommon" => Recipe.RecipeRarity.Uncommon,
            "rare" => Recipe.RecipeRarity.Rare,
            "very rare" => Recipe.RecipeRarity.UltraRare,
            "ultrarare" => Recipe.RecipeRarity.UltraRare,
            "ultra rare" => Recipe.RecipeRarity.UltraRare,
            "archetype" => Recipe.RecipeRarity.UltraRare,
            "superior archetype" => Recipe.RecipeRarity.UltraRare,
            _ => Recipe.RecipeRarity.Common
        };
    }

    private static Salvage.SalvageOrigin MapSalvageOrigin(IEnumerable<string> workshops)
    {
        var values = workshops.Where(value => !string.IsNullOrWhiteSpace(value)).ToArray();
        var hasTech = values.Any(value => value.Contains("Tech", StringComparison.OrdinalIgnoreCase));
        var hasMagic = values.Any(value => value.Contains("Arcane", StringComparison.OrdinalIgnoreCase) || value.Contains("Magic", StringComparison.OrdinalIgnoreCase));
        return hasTech && !hasMagic
            ? Salvage.SalvageOrigin.Tech
            : hasMagic && !hasTech
                ? Salvage.SalvageOrigin.Magic
                : Salvage.SalvageOrigin.Special;
    }

    private static Enums.eType MapEnhancementType(NormalizedEnhancementSource source)
    {
        if (source == null)
        {
            return Enums.eType.Normal;
        }

        var family = source.EnhancementFamily?.Trim() ?? string.Empty;
        var enhancementType = source.EnhancementType?.Trim() ?? string.Empty;

        if (string.Equals(family, "Classic", StringComparison.OrdinalIgnoreCase) || source.IsClassicOriginVariant)
        {
            return Enums.eType.Normal;
        }

        if (string.Equals(family, "Special", StringComparison.OrdinalIgnoreCase) ||
            enhancementType is "Hamidon" or "Hydra" or "Titan" or "Yin" or "Synthetic" or "DSync")
        {
            return Enums.eType.SpecialO;
        }

        if (!string.IsNullOrWhiteSpace(source.EnhancementSetName) ||
            !string.IsNullOrWhiteSpace(source.EnhancementSetCanonicalId) ||
            !string.IsNullOrWhiteSpace(source.EnhancementSetStorageKey) ||
            string.Equals(enhancementType, "IOSet", StringComparison.OrdinalIgnoreCase))
        {
            return Enums.eType.SetO;
        }

        if (string.Equals(family, "Invention", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(enhancementType, "IO", StringComparison.OrdinalIgnoreCase) ||
            !string.IsNullOrWhiteSpace(source.RecipeKey) ||
            !string.IsNullOrWhiteSpace(source.RecipeCanonicalId) ||
            !string.IsNullOrWhiteSpace(source.RecipeStorageKey) ||
            source.Attuned ||
            source.SuperiorAttuned)
        {
            return Enums.eType.InventO;
        }

        return Enums.eType.Normal;
    }

    private static int MapSpecialSubtype(IDatabase? database, NormalizedEnhancementSource source)
    {
        var shortName = ResolveSpecialSubtypeShortName(source);
        if (database?.SpecialEnhancements != null)
        {
            var match = database.SpecialEnhancements.FirstOrDefault(type =>
                string.Equals(type.ShortName, shortName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(type.Name, shortName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(match.Name))
            {
                return match.Index;
            }
        }

        return shortName switch
        {
            "HO" => 1,
            "HyO" => 2,
            "TnO" => 3,
            "DSyncO" => 4,
            _ => 0
        };
    }

    private static string ResolveSpecialSubtypeShortName(NormalizedEnhancementSource source)
    {
        var enhancementType = source?.EnhancementType?.Trim() ?? string.Empty;
        return enhancementType switch
        {
            "Hamidon" => "HO",
            "Hydra" => "HyO",
            "Titan" => "TnO",
            "DSync" => "DSyncO",
            "Synthetic" => "SynHO",
            "Yin" => "Yin",
            _ => "None"
        };
    }

    private static int MapSetType(IDatabase database, NormalizedEnhancementSetSource source)
    {
        if (database.SetTypes == null || database.SetTypes.Count == 0)
        {
            return 0;
        }

        var candidates = source.ConversionGroups
            .Where(value => value.StartsWith("Category:", StringComparison.OrdinalIgnoreCase))
            .Select(value => value[(value.IndexOf(':') + 1)..].Trim())
            .Concat(string.IsNullOrWhiteSpace(source.GroupName) ? [] : [source.GroupName])
            .SelectMany(value => new[] { value, value.TrimEnd('s') })
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var candidate in candidates)
        {
            var normalized = NormalizeLookupKey(candidate);
            var match = database.SetTypes.FirstOrDefault(type =>
                NormalizeLookupKey(type.Name) == normalized ||
                NormalizeLookupKey(type.ShortName) == normalized);
            if (match.Index != 0 || !string.IsNullOrWhiteSpace(match.Name))
            {
                return match.Index;
            }
        }

        return 0;
    }

    private static bool TryGetSetLevelBand(NormalizedEnhancementSetSource source, out int levelMin, out int levelMax)
    {
        if (!source.LevelMin.HasValue && !source.LevelMax.HasValue)
        {
            levelMin = 0;
            levelMax = 0;
            return false;
        }

        levelMin = Math.Max(0, (source.LevelMin ?? source.LevelMax ?? 1) - 1);
        levelMax = Math.Max(levelMin, (source.LevelMax ?? source.LevelMin ?? 1) - 1);
        return true;
    }

    private static bool TryGetSetLevelBand(JToken? enhancementSetGroups, string setName, out int levelMin, out int levelMax)
    {
        levelMin = 0;
        levelMax = 0;
        if (enhancementSetGroups is not JObject groups)
        {
            return false;
        }

        foreach (var property in groups.Properties())
        {
            if (property.Value is not JArray entries)
            {
                continue;
            }

            foreach (var entry in entries.OfType<JObject>())
            {
                var name = entry.Value<string>("name");
                if (!string.Equals(name, setName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                levelMin = Math.Max(0, (entry.Value<int?>("min_boost_level") ?? 1) - 1);
                levelMax = Math.Max(levelMin, (entry.Value<int?>("max_boost_level") ?? 1) - 1);
                return true;
            }
        }

        return false;
    }

    private static bool AssignIfChanged<T>(ref T field, T value)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        return true;
    }

    private static int ParseInt(string? value)
    {
        return int.TryParse(value, out var parsed) ? parsed : 0;
    }

    private static string GetEnhancementUidFromReward(string enhancementReward)
    {
        if (string.IsNullOrWhiteSpace(enhancementReward))
        {
            return string.Empty;
        }

        var segments = enhancementReward.Split('.', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length == 0 ? string.Empty : segments[^1];
    }

    private static (int LevelMin, int LevelMax) GetEnhancementLevelBand(OmniEnhancementDefinition source)
    {
        if (source.LevelVariants.Count > 0)
        {
            var levelMin = Math.Max(0, source.LevelVariants.Min() - 1);
            var levelMax = Math.Max(levelMin, source.LevelVariants.Max() - 1);
            return (levelMin, levelMax);
        }

        var fallbackMin = Math.Max(0, (source.MinLevel ?? 1) - 1);
        var fallbackMax = Math.Max(fallbackMin, (source.MaxLevel ?? source.MinLevel ?? 1) - 1);
        return (fallbackMin, fallbackMax);
    }
}

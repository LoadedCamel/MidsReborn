using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;

namespace Mids_Reborn.Core.Omni;

public sealed partial class OmniImporter
{
    internal sealed class NormalizedEnhancementImportData
    {
        public string RecipeSourceDirectoryName { get; set; } = string.Empty;
        public int EnhancementSourceRecordsDiscovered { get; set; }
        public int EnhancementRecordsExcludedByPolicy { get; set; }
        public int EnhancementMalformedRecordsSkipped { get; set; }
        public int EnhancementSetMalformedRecordsSkipped { get; set; }
        public int RecipeMalformedRecordsSkipped { get; set; }
        public int SalvageMalformedRecordsSkipped { get; set; }
        public int RecipeLevelVariantsDiscovered { get; set; }
        public int RecipeRewardLinksResolved { get; set; }
        public int RecipeRewardLinksMissing { get; set; }
        public int RecipeRecordsExcludedByPolicy { get; set; }
        public int StructuredBoostsAllowedParsedCount { get; set; }
        public int ShapeCompatibilityFallbackCount { get; set; }
        public int ClassicEnhancementSourceVariantsDiscovered { get; set; }
        public int ClassicEnhancementLogicalRecords { get; set; }
        public int ClassicEnhancementVariantsFolded { get; set; }
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
        public List<string> ShapeValidationDetails { get; } = [];
        public List<string> MalformedRecordDetails { get; } = [];
        public List<string> ClassicEnhancementFoldDetails { get; } = [];
        public List<string> InventionVariantAuditDetails { get; } = [];
        public List<NormalizedEnhancementSource> Enhancements { get; } = [];
        public List<NormalizedEnhancementSetSource> EnhancementSets { get; } = [];
        public List<NormalizedRecipeSource> Recipes { get; } = [];
        public List<NormalizedSalvageSource> Salvage { get; } = [];
        public Dictionary<string, NormalizedSetBonusSource> SetBonusDefinitions { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    internal sealed class NormalizedEnhancementSource
    {
        public string SourceKey { get; set; } = string.Empty;
        public string CanonicalId { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string DisplayFullName { get; set; } = string.Empty;
        public string EnhancementType { get; set; } = string.Empty;
        public string EnhancementFamily { get; set; } = string.Empty;
        public bool Attuned { get; set; }
        public bool SuperiorAttuned { get; set; }
        public string PowerFullName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string SourceFile { get; set; } = string.Empty;
        public string RecipeKey { get; set; } = string.Empty;
        public string RecipeName { get; set; } = string.Empty;
        public string RecipeCanonicalId { get; set; } = string.Empty;
        public string RecipeStorageKey { get; set; } = string.Empty;
        public string EnhancementSetName { get; set; } = string.Empty;
        public string EnhancementSetCanonicalId { get; set; } = string.Empty;
        public string EnhancementSetStorageKey { get; set; } = string.Empty;
        public string EnhancementSetGroupName { get; set; } = string.Empty;
        public int? EnhancementSetMinLevel { get; set; }
        public int? EnhancementSetMaxLevel { get; set; }
        public int LevelMin { get; set; }
        public int LevelMax { get; set; }
        public int MinimumUseLevel { get; set; }
        public int MaximumUseLevel { get; set; }
        public int MinimumSlotLevel { get; set; }
        public int MaximumSlotLevel { get; set; }
        public int MaxBoostLevel { get; set; }
        public bool IsClassicOriginVariant { get; set; }
        public string ClassicFoldKey { get; set; } = string.Empty;
        public List<int> LevelVariants { get; } = [];
        public List<OmniBoostAllowedRef> BoostsAllowed { get; } = [];
        public List<NormalizedEnhancementSource> FoldedClassicVariants { get; } = [];
    }

    internal sealed class NormalizedEnhancementSetSource
    {
        public string SourceKey { get; set; } = string.Empty;
        public string CanonicalId { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int? LevelMin { get; set; }
        public int? LevelMax { get; set; }
        public string BaseVariantSet { get; set; } = string.Empty;
        public string SuperiorVariantSet { get; set; } = string.Empty;
        public List<string> ConversionGroups { get; } = [];
        public List<string> BoostLists { get; } = [];
        public List<NormalizedEnhancementSetMemberSource> Members { get; } = [];
        public List<NormalizedEnhancementSetMemberSource> AttunedMembers { get; } = [];
        public List<NormalizedEnhancementSetMemberSource> SuperiorAttunedMembers { get; } = [];
        public List<NormalizedSetBonusEntry> Bonuses { get; } = [];
    }

    internal sealed class NormalizedEnhancementSetMemberSource
    {
        public string CanonicalId { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Variant { get; set; } = string.Empty;
    }

    internal sealed class NormalizedRecipeSource
    {
        public string SourceKey { get; set; } = string.Empty;
        public string CanonicalId { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string DisplayHelp { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string EnhancementReward { get; set; } = string.Empty;
        public string EnhancementRewardUid { get; set; } = string.Empty;
        public Recipe.RecipeRarity Rarity { get; set; }
        public bool IsGeneric { get; set; }
        public string SourceFile { get; set; } = string.Empty;
        public List<NormalizedRecipeLevelVariant> LevelVariants { get; } = [];
    }

    internal sealed class NormalizedRecipeLevelVariant
    {
        public int Level { get; set; }
        public int BuyFromVendor { get; set; }
        public int SellToVendor { get; set; }
        public List<string> CreationCost { get; } = [];
        public List<OmniRecipeSalvageRequirement> SalvageRequired { get; } = [];
        public List<string> PowerRequired { get; } = [];
        public List<string> VisibleRequires { get; } = [];
        public List<string> CreateRequires { get; } = [];
        public List<string> ReceiveRequires { get; } = [];
        public List<string> NeverReceiveRequires { get; } = [];
        public List<string> AuctionRequires { get; } = [];
    }

    internal sealed class NormalizedSalvageSource
    {
        public string SourceKey { get; set; } = string.Empty;
        public string CanonicalId { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public Recipe.RecipeRarity Rarity { get; set; }
        public Salvage.SalvageOrigin Origin { get; set; }
        public string SourceFile { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
    }

    internal sealed class NormalizedSetBonusSource
    {
        public string CanonicalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<NormalizedSetBonusEntry> Bonuses { get; } = [];
    }

    internal enum NormalizedSetBonusKind
    {
        Standard,
        MemberSpecific,
        GlobalUnique,
        PetSpecial,
        OtherSpecial
    }

    internal sealed class NormalizedSetBonusEntry
    {
        public string DisplayName { get; set; } = string.Empty;
        public int MinimumBoosts { get; set; }
        public int MaximumBoosts { get; set; }
        public string BonusPower { get; set; } = string.Empty;
        public List<string> AutoPowers { get; } = [];
        public List<string> RequiresTokens { get; } = [];
        public List<string> TargetEnhancementNames { get; } = [];
        public NormalizedSetBonusKind Kind { get; set; } = NormalizedSetBonusKind.Standard;
        public Enums.ePvX PvMode { get; set; } = Enums.ePvX.Any;
    }

    internal sealed class InventionVariantFamilyAudit
    {
        public string DisplayName { get; set; } = string.Empty;
        public string[] Variants { get; set; } = [];
    }

    private NormalizedEnhancementImportData LoadNormalizedEnhancementImportData(
        string exportRoot,
        OmniExportManifest? manifest = null,
        IProgress<OmniImportProgress>? progress = null)
    {
        var data = new NormalizedEnhancementImportData();

        var enhancementsRoot = Path.Combine(exportRoot, "enhancements");
        var enhancementSetsRoot = Path.Combine(exportRoot, "enhancement_sets");
        var recipesRoot = ResolveRecipeRoot(exportRoot, data);
        var salvageRoot = Path.Combine(exportRoot, "salvage");
        var recipeFiles = recipesRoot.Equals(Path.Combine(exportRoot, "base_recipes"), StringComparison.OrdinalIgnoreCase)
            ? manifest?.LegacyRecipeFiles
            : manifest?.RecipeFiles;

        if (Directory.Exists(enhancementsRoot))
        {
            var enhancementFiles = manifest?.EnhancementFiles?.Count > 0
                ? manifest.EnhancementFiles
                : Directory.EnumerateDirectories(enhancementsRoot)
                    .SelectMany(categoryDir => EnumerateJsonRecordFiles(categoryDir, recursive: false))
                    .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            var loadedEnhancements = LoadJsonRecordsParallel(
                enhancementFiles,
                file => ReadJsonWithError<OmniEnhancementDefinition>(file));

            foreach (var (file, definition, error) in loadedEnhancements)
            {
                data.EnhancementSourceRecordsDiscovered++;
                if (definition == null || string.IsNullOrWhiteSpace(definition.Name))
                {
                    data.EnhancementMalformedRecordsSkipped++;
                    AddMalformedRecordDetail(data, exportRoot, "Enhancement", file, FormatParseFailure(error));
                    continue;
                }

                if (ShouldExcludeEnhancementDefinition(definition, file))
                {
                    data.EnhancementRecordsExcludedByPolicy++;
                    data.ShapeValidationDetails.Add(
                        $"Enhancement record excluded by policy: {Path.GetFileName(file)} ({FirstNonEmpty(definition.DisplayName, definition.Name)}).");
                    continue;
                }

                var normalized = NormalizeEnhancementSource(definition, file, data);
                if (normalized == null)
                {
                    data.EnhancementMalformedRecordsSkipped++;
                    AddMalformedRecordDetail(data, exportRoot, "Enhancement", file,
                        FormatMissingRequiredFields(
                            "missing required identity or power data",
                            ("name", definition.Name),
                            ("power_full_name", definition.PowerFullName)));
                    continue;
                }

                data.Enhancements.Add(normalized);
                AddTargetedEnhancementClassificationAudit(data, normalized, "normalized");
            }
        }

        if (Directory.Exists(enhancementSetsRoot))
        {
            var loadedSets = LoadJsonRecordsParallel(
                manifest?.EnhancementSetFiles?.Count > 0
                    ? manifest.EnhancementSetFiles
                    : EnumerateJsonRecordFiles(enhancementSetsRoot, recursive: false),
                file => ReadJsonWithError<OmniEnhancementSetDefinition>(file));
            foreach (var (file, definition, error) in loadedSets)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.Name))
                {
                    data.EnhancementSetMalformedRecordsSkipped++;
                    AddMalformedRecordDetail(data, exportRoot, "Enhancement set", file, FormatParseFailure(error));
                    continue;
                }

                var normalizedSet = new NormalizedEnhancementSetSource
                {
                    SourceKey = ChooseSourceKey(definition.CanonicalId?.Value, definition.StorageKey, definition.Name),
                    CanonicalId = definition.CanonicalId?.Value ?? string.Empty,
                    StorageKey = definition.StorageKey,
                    Name = definition.Name,
                    DisplayName = definition.DisplayName,
                    GroupName = definition.GroupName,
                    Icon = definition.Icon,
                    LevelMin = definition.MinLevel,
                    LevelMax = definition.MaxLevel,
                    BaseVariantSet = definition.BaseVariantSet?.Name ?? string.Empty,
                    SuperiorVariantSet = definition.SuperiorVariantSet?.Name ?? string.Empty
                };
                normalizedSet.ConversionGroups.AddRange(definition.ConversionGroups.Where(value => !string.IsNullOrWhiteSpace(value)));
                normalizedSet.BoostLists.AddRange(definition.BoostLists.Where(value => !string.IsNullOrWhiteSpace(value)));
                normalizedSet.Members.AddRange(NormalizeSetMembers(definition.Enhancements, "crafted"));
                normalizedSet.AttunedMembers.AddRange(NormalizeSetMembers(definition.AttunedEnhancements, "attuned"));
                normalizedSet.SuperiorAttunedMembers.AddRange(NormalizeSetMembers(definition.SuperiorAttunedEnhancements, "superior-attuned"));
                normalizedSet.Bonuses.AddRange(NormalizeSetBonuses(
                    definition.Bonuses,
                    normalizedSet.Members.Concat(normalizedSet.AttunedMembers).Concat(normalizedSet.SuperiorAttunedMembers).ToArray(),
                    normalizedSet.GroupName));
                data.EnhancementSets.Add(normalizedSet);
            }
        }

        if (!string.IsNullOrWhiteSpace(recipesRoot) && Directory.Exists(recipesRoot))
        {
            var loadedRecipes = LoadJsonRecordsParallel(
                recipeFiles?.Count > 0
                    ? recipeFiles
                    : EnumerateJsonRecordFiles(recipesRoot, recursive: true),
                file => ReadJsonWithError<OmniRecipeDefinition>(file));
            foreach (var (file, definition, error) in loadedRecipes)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.Name))
                {
                    data.RecipeMalformedRecordsSkipped++;
                    AddMalformedRecordDetail(data, exportRoot, "Recipe", file, FormatParseFailure(error));
                    continue;
                }

                if (ShouldExcludeRecipeDefinition(definition, file))
                {
                    data.RecipeRecordsExcludedByPolicy++;
                    data.ShapeValidationDetails.Add($"Recipe excluded by policy: {definition.Name}.");
                    continue;
                }

                var normalized = NormalizeRecipeSource(definition, file, data);
                if (normalized == null)
                {
                    data.RecipeMalformedRecordsSkipped++;
                    AddMalformedRecordDetail(data, exportRoot, "Recipe", file,
                        FormatMissingRequiredFields(
                            "missing required recipe identity or reward data",
                            ("name", definition.Name),
                            ("enhancement_reward", definition.EnhancementReward)));
                    continue;
                }

                data.Recipes.Add(normalized);
                data.RecipeLevelVariantsDiscovered += normalized.LevelVariants.Count;
            }
        }

        if (Directory.Exists(salvageRoot))
        {
            var loadedSalvage = LoadJsonRecordsParallel(
                manifest?.SalvageFiles?.Count > 0
                    ? manifest.SalvageFiles
                    : EnumerateJsonRecordFiles(salvageRoot, recursive: false),
                file => ReadJsonWithError<OmniSalvageDefinition>(file));
            foreach (var (file, definition, error) in loadedSalvage)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.Name))
                {
                    data.SalvageMalformedRecordsSkipped++;
                    AddMalformedRecordDetail(data, exportRoot, "Salvage", file, FormatParseFailure(error));
                    continue;
                }

                data.Salvage.Add(new NormalizedSalvageSource
                {
                    SourceKey = ChooseSourceKey(definition.RecordId, definition.StorageKey, definition.Name),
                    CanonicalId = definition.RecordId,
                    StorageKey = definition.StorageKey,
                    Name = definition.Name,
                    DisplayName = definition.DisplayName,
                    Icon = definition.Icon,
                    Rarity = MapRecipeRarity(definition.RarityName),
                    Origin = MapSalvageOrigin(definition.Workshops),
                    SourceFile = definition.SourceFile,
                    TypeName = definition.TypeName
                });
            }
        }

        foreach (var item in LoadSetBonusDefinitions(exportRoot).Values)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                continue;
            }

            var normalizedSetBonus = new NormalizedSetBonusSource
            {
                CanonicalId = item.CanonicalId?.Value ?? string.Empty,
                Name = item.Name
            };
            var setSource = data.EnhancementSets.FirstOrDefault(set => string.Equals(set.Name, item.Name, StringComparison.OrdinalIgnoreCase));
            var knownMembers = setSource == null
                ? Array.Empty<NormalizedEnhancementSetMemberSource>()
                : setSource.Members.Concat(setSource.AttunedMembers).Concat(setSource.SuperiorAttunedMembers).ToArray();
            normalizedSetBonus.Bonuses.AddRange(NormalizeSetBonuses(item.Bonuses, knownMembers, setSource?.GroupName ?? string.Empty));
            data.SetBonusDefinitions[item.Name] = normalizedSetBonus;
        }

        MergeSetBonusDefinitions(data);

        FoldClassicEnhancementVariants(data);
        MergeSetIdentityFromEnhancements(data);
        AuditInventionVariantMultiplicity(data);

        return data;
    }

    private List<(string File, T? Value, string? Error)> LoadJsonRecordsParallel<T>(
        IEnumerable<string> files,
        Func<string, (T? Value, string? Error)> loader) where T : class
    {
        var loaded = new ConcurrentBag<(string File, T? Value, string? Error)>();
        Parallel.ForEach(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = GetAdaptiveParallelDegree(3) },
            file =>
            {
                var (value, error) = loader(file);
                loaded.Add((file, value, error));
            });
        return loaded
            .OrderBy(entry => entry.File, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void AddMalformedRecordDetail(
        NormalizedEnhancementImportData data,
        string exportRoot,
        string recordKind,
        string filePath,
        string reason)
    {
        var detail = $"{recordKind} record skipped: {FormatSourceRecordPath(exportRoot, filePath)} - {reason}.";
        data.MalformedRecordDetails.Add(detail);
        data.ShapeValidationDetails.Add(detail);
    }

    private static string FormatParseFailure(string? error)
    {
        return string.IsNullOrWhiteSpace(error)
            ? "could not be parsed; deserializer returned null"
            : $"could not be parsed: {error}";
    }

    private static string FormatMissingRequiredFields(
        string reason,
        params (string Name, string? Value)[] fields)
    {
        var values = string.Join(", ", fields.Select(field =>
        {
            var value = string.IsNullOrWhiteSpace(field.Value) ? "<blank>" : field.Value.Trim();
            return $"{field.Name}='{value}'";
        }));
        return $"{reason} ({values})";
    }

    private static string FormatSourceRecordPath(string exportRoot, string filePath)
    {
        var relativePath = Path.GetRelativePath(exportRoot, filePath);
        return relativePath.StartsWith("..", StringComparison.Ordinal) || Path.IsPathRooted(relativePath)
            ? filePath
            : relativePath;
    }

    private static void FoldClassicEnhancementVariants(NormalizedEnhancementImportData data)
    {
        var classicVariants = data.Enhancements
            .Where(item => item.IsClassicOriginVariant && !string.IsNullOrWhiteSpace(item.ClassicFoldKey))
            .ToList();

        data.ClassicEnhancementSourceVariantsDiscovered = classicVariants.Count;
        if (classicVariants.Count == 0)
        {
            return;
        }

        var foldedEnhancements = new List<NormalizedEnhancementSource>(data.Enhancements.Count);
        var consumedClassicKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var enhancement in data.Enhancements)
        {
            if (!enhancement.IsClassicOriginVariant || string.IsNullOrWhiteSpace(enhancement.ClassicFoldKey))
            {
                foldedEnhancements.Add(enhancement);
                continue;
            }

            if (!consumedClassicKeys.Add(enhancement.ClassicFoldKey))
            {
                continue;
            }

            var group = classicVariants
                .Where(item => string.Equals(item.ClassicFoldKey, enhancement.ClassicFoldKey, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(item => GetClassicRepresentativeRank(item.EnhancementType))
                .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var canonical = group[0];
            foreach (var folded in group.Skip(1))
            {
                canonical.FoldedClassicVariants.Add(folded);
            }

            foldedEnhancements.Add(canonical);
            data.ClassicEnhancementLogicalRecords++;
            data.ClassicEnhancementVariantsFolded += canonical.FoldedClassicVariants.Count;

            if (canonical.FoldedClassicVariants.Count > 0)
            {
                data.ClassicEnhancementFoldDetails.Add(
                    $"{canonical.Name}: kept {canonical.EnhancementType} for fold key '{canonical.ClassicFoldKey}', folded {string.Join(", ", canonical.FoldedClassicVariants.Select(item => $"{item.EnhancementType}:{item.Name}"))}.");
                AddTargetedEnhancementClassificationAudit(data, canonical, "folded");
            }
        }

        data.Enhancements.Clear();
        data.Enhancements.AddRange(foldedEnhancements);
    }

    private static bool IsClassicEnhancementVariant(string enhancementType, string enhancementFamily)
    {
        return (string.Equals(enhancementFamily, "Classic", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(enhancementFamily)) &&
               (string.Equals(enhancementType, "TR", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(enhancementType, "TO", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(enhancementType, "DO", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(enhancementType, "SO", StringComparison.OrdinalIgnoreCase));
    }

    private static string BuildClassicEnhancementFoldKey(NormalizedEnhancementSource source)
    {
        var strippedIdentity = TryStripClassicOriginPrefix(source.Name);
        if (string.IsNullOrWhiteSpace(strippedIdentity))
        {
            strippedIdentity = TryStripClassicOriginPrefix(GetEnhancementUidFromReward(source.PowerFullName));
        }

        strippedIdentity ??= source.Name;
        var identityKey = NormalizeLookupKey(strippedIdentity);
        if (string.IsNullOrWhiteSpace(identityKey))
        {
            identityKey = NormalizeLookupKey(source.Name);
        }
        return identityKey;
    }

    private static string? TryStripClassicOriginPrefix(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var segments = value.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments.Length >= 3 &&
            ClassicEnhancementVariantResolver.TryNormalizeOriginToken(segments[0], out _) &&
            ClassicEnhancementVariantResolver.TryNormalizeOriginToken(segments[1], out _) &&
            !string.Equals(segments[0], segments[1], StringComparison.OrdinalIgnoreCase))
        {
            return string.Join("_", segments.Skip(2));
        }

        if (segments.Length >= 2 &&
            (string.Equals(segments[0], "generic", StringComparison.OrdinalIgnoreCase) ||
             ClassicEnhancementVariantResolver.TryNormalizeOriginToken(segments[0], out _)))
        {
            return string.Join("_", segments.Skip(1));
        }

        return null;
    }

    private static int GetClassicRepresentativeRank(string enhancementType)
    {
        if (string.Equals(enhancementType, "SO", StringComparison.OrdinalIgnoreCase))
        {
            return 3;
        }

        if (string.Equals(enhancementType, "DO", StringComparison.OrdinalIgnoreCase))
        {
            return 2;
        }

        if (string.Equals(enhancementType, "TR", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(enhancementType, "TO", StringComparison.OrdinalIgnoreCase))
        {
            return 1;
        }

        return 0;
    }

    private static bool ShouldExcludeEnhancementDefinition(OmniEnhancementDefinition definition, string filePath)
    {
        return string.Equals(definition.DisplayName, "OmniHack", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(definition.Name, "Yins_Omni", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(definition.StorageKey, "yins_omni", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(definition.CanonicalId?.Value, "enhancements:yin:yins_omni", StringComparison.OrdinalIgnoreCase) ||
               filePath.Contains("yins_omni", StringComparison.OrdinalIgnoreCase);
    }

    private static void AddTargetedEnhancementClassificationAudit(
        NormalizedEnhancementImportData data,
        NormalizedEnhancementSource source,
        string stage)
    {
        if (!ShouldAuditEnhancementClassification(source))
        {
            return;
        }

        var mappedType = MapEnhancementType(source);
        var subtype = mappedType == Enums.eType.SpecialO ? ResolveSpecialSubtypeShortName(source) : "n/a";
        var outcome = stage switch
        {
            "folded" when source.FoldedClassicVariants.Count > 0 =>
                $"kept={source.EnhancementType}, folded={string.Join(", ", source.FoldedClassicVariants.Select(item => item.EnhancementType))}",
            _ => source.IsClassicOriginVariant ? $"classic-fold-key={source.ClassicFoldKey}" : "import"
        };

        data.ShapeValidationDetails.Add(
            $"Enhancement classification audit: {source.Name} ({source.DisplayName}) family={source.EnhancementFamily}, type={source.EnhancementType}, mappedType={mappedType}, mappedSubtype={subtype}, stage={stage}, outcome={outcome}.");
    }

    private static bool ShouldAuditEnhancementClassification(NormalizedEnhancementSource source)
    {
        if (source == null)
        {
            return false;
        }

        var enhancementType = source.EnhancementType?.Trim() ?? string.Empty;
        return source.IsClassicOriginVariant ||
               string.Equals(source.DisplayName, "OmniHack", StringComparison.OrdinalIgnoreCase) ||
               enhancementType is "Hamidon" or "Hydra" or "Titan" or "Yin" or "Synthetic" or "DSync" or "TO" or "TR" or "DO" or "SO";
    }

    private static void MergeSetIdentityFromEnhancements(NormalizedEnhancementImportData data)
    {
        if (data.Enhancements.Count == 0)
        {
            return;
        }

        var byName = data.EnhancementSets
            .Where(set => !string.IsNullOrWhiteSpace(set.Name))
            .ToDictionary(set => set.Name, set => set, StringComparer.OrdinalIgnoreCase);

        foreach (var enhancement in data.Enhancements.Where(item => !string.IsNullOrWhiteSpace(item.EnhancementSetName)))
        {
            if (!byName.TryGetValue(enhancement.EnhancementSetName, out var set))
            {
                set = new NormalizedEnhancementSetSource
                {
                    SourceKey = ChooseSourceKey(enhancement.EnhancementSetCanonicalId, enhancement.EnhancementSetStorageKey, enhancement.EnhancementSetName),
                    CanonicalId = enhancement.EnhancementSetCanonicalId,
                    StorageKey = enhancement.EnhancementSetStorageKey,
                    Name = enhancement.EnhancementSetName,
                    DisplayName = enhancement.EnhancementSetName,
                    GroupName = enhancement.EnhancementSetGroupName,
                    LevelMin = enhancement.EnhancementSetMinLevel,
                    LevelMax = enhancement.EnhancementSetMaxLevel
                };
                data.EnhancementSets.Add(set);
                byName[set.Name] = set;
            }

            if (string.IsNullOrWhiteSpace(set.CanonicalId) && !string.IsNullOrWhiteSpace(enhancement.EnhancementSetCanonicalId))
            {
                set.CanonicalId = enhancement.EnhancementSetCanonicalId;
            }

            if (string.IsNullOrWhiteSpace(set.StorageKey) && !string.IsNullOrWhiteSpace(enhancement.EnhancementSetStorageKey))
            {
                set.StorageKey = enhancement.EnhancementSetStorageKey;
            }

            if (string.IsNullOrWhiteSpace(set.GroupName) && !string.IsNullOrWhiteSpace(enhancement.EnhancementSetGroupName))
            {
                set.GroupName = enhancement.EnhancementSetGroupName;
            }

            if (set.LevelMin == null && enhancement.EnhancementSetMinLevel.HasValue)
            {
                set.LevelMin = enhancement.EnhancementSetMinLevel;
            }

            if (set.LevelMax == null && enhancement.EnhancementSetMaxLevel.HasValue)
            {
                set.LevelMax = enhancement.EnhancementSetMaxLevel;
            }

            var memberIcon = FindSetMemberIcon(set, enhancement.Name);
            if (!string.IsNullOrWhiteSpace(memberIcon))
            {
                enhancement.Icon = memberIcon;
            }
        }
    }

    private static string FindSetMemberIcon(
        NormalizedEnhancementSetSource set,
        string enhancementName)
    {
        if (string.IsNullOrWhiteSpace(enhancementName))
        {
            return string.Empty;
        }

        return set.Members
            .Concat(set.AttunedMembers)
            .Concat(set.SuperiorAttunedMembers)
            .FirstOrDefault(member => string.Equals(member.Name, enhancementName, StringComparison.OrdinalIgnoreCase))
            ?.Icon ?? string.Empty;
    }

    private static void AuditInventionVariantMultiplicity(NormalizedEnhancementImportData data)
    {
        var inventionEnhancements = data.Enhancements
            .Where(item => string.Equals(item.EnhancementFamily, "Invention", StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (inventionEnhancements.Count == 0)
        {
            return;
        }

        foreach (var enhancement in inventionEnhancements)
        {
            switch (ClassifyInventionVariant(enhancement))
            {
                case "crafted":
                    data.InventionCraftedVariantsDiscovered++;
                    break;
                case "attuned":
                    data.InventionAttunedVariantsDiscovered++;
                    break;
                case "superior":
                    data.InventionSuperiorVariantsDiscovered++;
                    break;
                case "superior-attuned":
                    data.InventionSuperiorAttunedVariantsDiscovered++;
                    break;
            }

            if (!HasAnySourceIdentity(enhancement.EnhancementSetCanonicalId, enhancement.EnhancementSetStorageKey, enhancement.EnhancementSetName))
            {
                switch (ClassifyInventionVariant(enhancement))
                {
                    case "crafted":
                        data.InventionStandaloneCraftedVariantsDiscovered++;
                        break;
                    case "attuned":
                        data.InventionStandaloneAttunedVariantsDiscovered++;
                        break;
                    case "superior":
                        data.InventionStandaloneSuperiorVariantsDiscovered++;
                        break;
                    case "superior-attuned":
                        data.InventionStandaloneSuperiorAttunedVariantsDiscovered++;
                        break;
                }
            }
        }

        var ioSetFamilies = inventionEnhancements
            .Where(item => HasAnySourceIdentity(item.EnhancementSetCanonicalId, item.EnhancementSetStorageKey, item.EnhancementSetName))
            .GroupBy(item => FirstNonEmpty(item.EnhancementSetCanonicalId, item.EnhancementSetStorageKey, item.EnhancementSetName), StringComparer.OrdinalIgnoreCase)
            .Select(group => new InventionVariantFamilyAudit
            {
                DisplayName = FirstNonEmpty(group.First().EnhancementSetName, group.Key),
                Variants = group
                    .Select(ClassifyInventionVariant)
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(value => GetInventionVariantRank(value))
                    .ToArray()
            })
            .ToList();

        foreach (var family in ioSetFamilies)
        {
            var signature = string.Join("+", family.Variants);
            switch (signature)
            {
                case "crafted":
                    data.IoSetFamiliesCraftedOnly++;
                    break;
                case "attuned":
                    data.IoSetFamiliesAttunedOnly++;
                    break;
                case "superior":
                    data.IoSetFamiliesSuperiorOnly++;
                    break;
                case "superior-attuned":
                    data.IoSetFamiliesSuperiorAttunedOnly++;
                    break;
                case "crafted+attuned":
                    data.IoSetFamiliesCraftedAndAttuned++;
                    break;
                case "crafted+superior":
                    data.IoSetFamiliesCraftedAndSuperior++;
                    break;
                case "crafted+superior-attuned":
                    data.IoSetFamiliesCraftedAndSuperiorAttuned++;
                    break;
                case "attuned+superior":
                    data.IoSetFamiliesAttunedAndSuperior++;
                    break;
                case "attuned+superior-attuned":
                    data.IoSetFamiliesAttunedAndSuperiorAttuned++;
                    break;
                case "superior+superior-attuned":
                    data.IoSetFamiliesSuperiorAndSuperiorAttuned++;
                    break;
                case "crafted+attuned+superior":
                    data.IoSetFamiliesCraftedAttunedSuperior++;
                    break;
                case "crafted+attuned+superior-attuned":
                    data.IoSetFamiliesCraftedAttunedSuperiorAttuned++;
                    break;
                case "crafted+superior+superior-attuned":
                    data.IoSetFamiliesCraftedSuperiorSuperiorAttuned++;
                    break;
                case "attuned+superior+superior-attuned":
                    data.IoSetFamiliesAttunedSuperiorSuperiorAttuned++;
                    break;
                case "crafted+attuned+superior+superior-attuned":
                    data.IoSetFamiliesAllFourVariants++;
                    break;
            }
        }

        AddVariantCoverageSamples(data, ioSetFamilies, "crafted", "IO set families with crafted only");
        AddVariantCoverageSamples(data, ioSetFamilies, "attuned", "IO set families with attuned only");
        AddVariantCoverageSamples(data, ioSetFamilies, "superior-attuned", "IO set families with superior-attuned only");
        AddVariantCoverageSamples(data, ioSetFamilies, "crafted+attuned", "IO set families with crafted + attuned");
        AddVariantCoverageSamples(data, ioSetFamilies, "crafted+superior-attuned", "IO set families with crafted + superior-attuned");
        AddVariantCoverageSamples(data, ioSetFamilies, "attuned+superior-attuned", "IO set families with attuned + superior-attuned");
        AddVariantCoverageSamples(data, ioSetFamilies, "crafted+attuned+superior-attuned", "IO set families with crafted + attuned + superior-attuned");
    }

    private static void AddVariantCoverageSamples(
        NormalizedEnhancementImportData data,
        IEnumerable<InventionVariantFamilyAudit> ioSetFamilies,
        string signature,
        string label)
    {
        var matches = ioSetFamilies
            .Where(item => string.Equals(string.Join("+", item.Variants), signature, StringComparison.OrdinalIgnoreCase))
            .Select(item => item.DisplayName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToArray();
        if (matches.Length == 0)
        {
            return;
        }

        data.InventionVariantAuditDetails.Add($"{label}: {string.Join(", ", matches)}.");
    }

    private static string ClassifyInventionVariant(NormalizedEnhancementSource source)
    {
        if (source.SuperiorAttuned)
        {
            return "superior-attuned";
        }

        if (source.Attuned)
        {
            return "attuned";
        }

        if (source.Name.StartsWith("Superior_", StringComparison.OrdinalIgnoreCase) ||
            source.DisplayName.Contains("Superior", StringComparison.OrdinalIgnoreCase))
        {
            return "superior";
        }

        return "crafted";
    }

    private static int GetInventionVariantRank(string variant)
    {
        return variant switch
        {
            "crafted" => 1,
            "attuned" => 2,
            "superior" => 3,
            "superior-attuned" => 4,
            _ => 99
        };
    }

    private static bool HasAnySourceIdentity(params string[] values)
    {
        return values.Any(value => !string.IsNullOrWhiteSpace(value));
    }

    private static string ResolveRecipeRoot(string exportRoot, NormalizedEnhancementImportData data)
    {
        var recipesRoot = Path.Combine(exportRoot, "recipes");
        if (Directory.Exists(recipesRoot))
        {
            data.RecipeSourceDirectoryName = "recipes";
            return recipesRoot;
        }

        var legacyRoot = Path.Combine(exportRoot, "base_recipes");
        if (Directory.Exists(legacyRoot))
        {
            data.RecipeSourceDirectoryName = "base_recipes";
            data.ShapeCompatibilityFallbackCount++;
            data.ShapeValidationDetails.Add("Compatibility fallback: using legacy base_recipes/ directory.");
            return legacyRoot;
        }

        data.RecipeSourceDirectoryName = "missing";
        return string.Empty;
    }

    private static IEnumerable<string> EnumerateJsonRecordFiles(string root, bool recursive)
    {
        if (!Directory.Exists(root))
        {
            return [];
        }

        var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        return Directory.EnumerateFiles(root, "*.json", option)
            .Where(file => !string.Equals(Path.GetFileName(file), "index.json", StringComparison.OrdinalIgnoreCase));
    }

    private static NormalizedEnhancementSource? NormalizeEnhancementSource(
        OmniEnhancementDefinition definition,
        string filePath,
        NormalizedEnhancementImportData data)
    {
        var powerFullName = CanonicalizeOmniFullName(definition.PowerFullName);
        if (string.IsNullOrWhiteSpace(definition.Name) || string.IsNullOrWhiteSpace(powerFullName))
        {
            return null;
        }

        var levelVariants = definition.LevelVariants.Where(level => level > 0).Distinct().OrderBy(level => level).ToList();
        var usedCompatibilityLevelFallback = false;
        int levelMin;
        int levelMax;
        if (definition.MinLevel.HasValue || definition.MaxLevel.HasValue)
        {
            levelMin = Math.Max(0, (definition.MinLevel ?? definition.MaxLevel ?? 1) - 1);
            levelMax = Math.Max(levelMin, (definition.MaxLevel ?? definition.MinLevel ?? 1) - 1);
        }
        else if (levelVariants.Count > 0)
        {
            levelMin = Math.Max(0, levelVariants.Min() - 1);
            levelMax = Math.Max(levelMin, levelVariants.Max() - 1);
        }
        else if (definition.MinSlotLevel.HasValue || definition.MaxSlotLevel.HasValue)
        {
            levelMin = Math.Max(0, definition.MinSlotLevel ?? definition.MaxSlotLevel ?? 0);
            levelMax = Math.Max(levelMin, definition.MaxSlotLevel ?? definition.MinSlotLevel ?? levelMin);
            usedCompatibilityLevelFallback = true;
        }
        else
        {
            levelMin = 0;
            levelMax = 0;
            usedCompatibilityLevelFallback = true;
        }

        if (usedCompatibilityLevelFallback)
        {
            data.ShapeCompatibilityFallbackCount++;
        }

        var boostsAllowed = definition.BoostsAllowed
            .Where(item => item != null && (item.Id.HasValue || !string.IsNullOrWhiteSpace(item.Name)))
            .Select(item => new OmniBoostAllowedRef
            {
                Id = item.Id,
                Name = item.Name
            })
            .ToList();

        if (boostsAllowed.Count > 0)
        {
            data.StructuredBoostsAllowedParsedCount++;
        }

        var minimumUseLevel = Math.Max(0, (definition.MinimumUseLevel ?? definition.MinimumUseLevelRaw ?? (levelMin + 1)) - 1);
        var maximumUseLevel = Math.Max(minimumUseLevel, (definition.MaximumUseLevel ?? definition.MaximumUseLevelRaw ?? (levelMax + 1)) - 1);
        var minimumSlotLevel = Math.Max(0, (definition.MinSlotLevel ?? definition.MinSlotLevelRaw ?? (levelMin + 1)) - 1);
        var maximumSlotLevel = Math.Max(minimumSlotLevel, (definition.MaxSlotLevel ?? definition.MaxSlotLevelRaw ?? (levelMax + 1)) - 1);
        var maxBoostLevel = Math.Max(maximumSlotLevel, (definition.MaxBoostLevel ?? (maximumSlotLevel + 1)) - 1);

        var normalized = new NormalizedEnhancementSource
        {
            SourceKey = ChooseSourceKey(definition.CanonicalId?.Value, definition.StorageKey, definition.Name),
            CanonicalId = definition.CanonicalId?.Value ?? string.Empty,
            StorageKey = definition.StorageKey,
            Name = definition.Name,
            DisplayName = definition.DisplayName,
            DisplayFullName = definition.DisplayFullName,
            EnhancementType = definition.EnhancementType,
            EnhancementFamily = definition.EnhancementFamily,
            Attuned = definition.Attuned,
            SuperiorAttuned = definition.SuperiorAttuned,
            PowerFullName = powerFullName,
            Icon = definition.Icon,
            SourceFile = string.IsNullOrWhiteSpace(definition.SourceFile) ? filePath : definition.SourceFile,
            RecipeKey = ChooseSourceKey(definition.Recipe?.CanonicalId?.Value, definition.Recipe?.StorageKey, definition.Recipe?.Name),
            RecipeName = definition.Recipe?.Name ?? string.Empty,
            RecipeCanonicalId = definition.Recipe?.CanonicalId?.Value ?? string.Empty,
            RecipeStorageKey = definition.Recipe?.StorageKey ?? string.Empty,
            EnhancementSetName = definition.EnhancementSet?.Name ?? string.Empty,
            EnhancementSetCanonicalId = definition.EnhancementSet?.CanonicalId?.Value ?? string.Empty,
            EnhancementSetStorageKey = definition.EnhancementSet?.StorageKey ?? string.Empty,
            EnhancementSetGroupName = definition.EnhancementSet?.GroupName ?? string.Empty,
            EnhancementSetMinLevel = definition.EnhancementSet?.MinLevel,
            EnhancementSetMaxLevel = definition.EnhancementSet?.MaxLevel,
            LevelMin = levelMin,
            LevelMax = levelMax,
            MinimumUseLevel = minimumUseLevel,
            MaximumUseLevel = maximumUseLevel,
            MinimumSlotLevel = minimumSlotLevel,
            MaximumSlotLevel = maximumSlotLevel,
            MaxBoostLevel = maxBoostLevel
        };
        normalized.IsClassicOriginVariant = IsClassicEnhancementVariant(normalized.EnhancementType, normalized.EnhancementFamily);
        normalized.ClassicFoldKey = normalized.IsClassicOriginVariant ? BuildClassicEnhancementFoldKey(normalized) : string.Empty;
        normalized.LevelVariants.AddRange(levelVariants);
        normalized.BoostsAllowed.AddRange(boostsAllowed);
        return normalized;
    }

    private static bool ShouldExcludeRecipeDefinition(OmniRecipeDefinition definition, string filePath)
    {
        return ContainsRecipePolicyToken(definition.Name) ||
               ContainsRecipePolicyToken(definition.StorageKey) ||
               ContainsRecipePolicyToken(definition.RecordId) ||
               ContainsRecipePolicyToken(definition.DisplayName) ||
               ContainsRecipePolicyToken(definition.Group) ||
               ContainsRecipePolicyToken(filePath);
    }

    private static bool ContainsRecipePolicyToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.Contains("Workshop", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("Merit_", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("Ticket_", StringComparison.OrdinalIgnoreCase) ||
               value.Contains(":merit_", StringComparison.OrdinalIgnoreCase) ||
               value.Contains(":ticket_", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("_workshop", StringComparison.OrdinalIgnoreCase);
    }

    private static NormalizedRecipeSource? NormalizeRecipeSource(
        OmniRecipeDefinition definition,
        string filePath,
        NormalizedEnhancementImportData data)
    {
        if (string.IsNullOrWhiteSpace(definition.Name) || string.IsNullOrWhiteSpace(definition.EnhancementReward))
        {
            return null;
        }

        var rewardPowerFullName = CanonicalizeOmniFullName(definition.EnhancementReward);
        if (string.IsNullOrWhiteSpace(rewardPowerFullName))
        {
            return null;
        }

        var normalized = new NormalizedRecipeSource
        {
            SourceKey = ChooseSourceKey(definition.RecordId, definition.StorageKey, definition.Name),
            CanonicalId = definition.RecordId,
            StorageKey = definition.StorageKey,
            Group = definition.Group,
            Name = definition.Name,
            DisplayName = definition.DisplayName,
            DisplayHelp = definition.DisplayHelp,
            Icon = definition.Icon,
            EnhancementReward = rewardPowerFullName,
            EnhancementRewardUid = GetEnhancementUidFromReward(rewardPowerFullName),
            Rarity = MapRecipeRarity(definition.RarityName),
            SourceFile = string.IsNullOrWhiteSpace(definition.SourceFile) ? filePath : definition.SourceFile
        };

        var levelVariants = definition.LevelVariants
            .Where(variant => variant != null)
            .ToList();
        if (levelVariants.Count == 0)
        {
            if (definition.Level.HasValue ||
                definition.CreationCost.Count > 0 ||
                definition.SalvageRequired.Count > 0 ||
                definition.BuyFromVendor.HasValue ||
                definition.SellToVendor.HasValue)
            {
                data.ShapeCompatibilityFallbackCount++;
                data.ShapeValidationDetails.Add($"Compatibility fallback: recipe {definition.Name} used legacy top-level level/cost/salvage fields.");
                levelVariants =
                [
                    new OmniRecipeLevelVariant
                    {
                        Level = definition.Level,
                        BuyFromVendor = definition.BuyFromVendor,
                        SellToVendor = definition.SellToVendor,
                        CreationCost = definition.CreationCost,
                        SalvageRequired = definition.SalvageRequired
                    }
                ];
            }
        }

        foreach (var variant in levelVariants)
        {
            var level = Math.Max(1, variant.Level ?? definition.Level ?? 1);
            var normalizedVariant = new NormalizedRecipeLevelVariant
            {
                Level = level,
                BuyFromVendor = variant.BuyFromVendor ?? definition.BuyFromVendor ?? 0,
                SellToVendor = variant.SellToVendor ?? definition.SellToVendor ?? 0
            };
            normalizedVariant.CreationCost.AddRange(variant.CreationCost.Where(value => !string.IsNullOrWhiteSpace(value)));
            normalizedVariant.SalvageRequired.AddRange(variant.SalvageRequired.Where(item => !string.IsNullOrWhiteSpace(item.Salvage)));
            normalizedVariant.PowerRequired.AddRange(variant.PowerRequired.Where(value => !string.IsNullOrWhiteSpace(value)));
            normalizedVariant.VisibleRequires.AddRange(variant.VisibleRequires.Where(value => !string.IsNullOrWhiteSpace(value)));
            normalizedVariant.CreateRequires.AddRange(variant.CreateRequires.Where(value => !string.IsNullOrWhiteSpace(value)));
            normalizedVariant.ReceiveRequires.AddRange(variant.ReceiveRequires.Where(value => !string.IsNullOrWhiteSpace(value)));
            normalizedVariant.NeverReceiveRequires.AddRange(variant.NeverReceiveRequires.Where(value => !string.IsNullOrWhiteSpace(value)));
            normalizedVariant.AuctionRequires.AddRange(variant.AuctionRequires.Where(value => !string.IsNullOrWhiteSpace(value)));
            normalized.LevelVariants.Add(normalizedVariant);
        }

        if (normalized.LevelVariants.Count == 0)
        {
            return null;
        }

        return normalized;
    }

    private static IEnumerable<NormalizedEnhancementSetMemberSource> NormalizeSetMembers(
        IEnumerable<OmniEnhancementSetMemberRef>? members,
        string variant)
    {
        if (members == null)
        {
            yield break;
        }

        foreach (var member in members.Where(member => member != null && !string.IsNullOrWhiteSpace(member.Name)))
        {
            yield return new NormalizedEnhancementSetMemberSource
            {
                CanonicalId = member.CanonicalId?.Value ?? string.Empty,
                StorageKey = member.StorageKey,
                Name = member.Name,
                DisplayName = member.DisplayName,
                Icon = member.Icon,
                Variant = variant
            };
        }
    }

    private static IEnumerable<NormalizedSetBonusEntry> NormalizeSetBonuses(
        IEnumerable<OmniSetBonusDefinition>? bonuses,
        IReadOnlyCollection<NormalizedEnhancementSetMemberSource> knownMembers,
        string groupName)
    {
        if (bonuses == null)
        {
            yield break;
        }

        var memberNames = knownMembers
            .Select(member => member.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var bonus in bonuses.Where(bonus => bonus != null))
        {
            var normalized = new NormalizedSetBonusEntry
            {
                DisplayName = bonus.DisplayName ?? string.Empty,
                MinimumBoosts = bonus.MinimumBoosts,
                MaximumBoosts = bonus.MaximumBoosts,
                BonusPower = bonus.BonusPower ?? string.Empty
            };
            normalized.AutoPowers.AddRange(bonus.AutoPowers.Where(value => !string.IsNullOrWhiteSpace(value)));
            normalized.RequiresTokens.AddRange(bonus.Requires.Where(value => !string.IsNullOrWhiteSpace(value)));
            normalized.TargetEnhancementNames.AddRange(ExtractTargetEnhancementNames(normalized.RequiresTokens, memberNames));
            normalized.Kind = DetermineSetBonusKind(normalized, groupName);
            normalized.PvMode = DetermineSetBonusPvMode(normalized);
            yield return normalized;
        }
    }

    private static void MergeSetBonusDefinitions(NormalizedEnhancementImportData data)
    {
        foreach (var set in data.EnhancementSets)
        {
            if (set.Bonuses.Count > 0)
            {
                continue;
            }

            if (data.SetBonusDefinitions.TryGetValue(set.Name, out var setBonusSource))
            {
                set.Bonuses.AddRange(setBonusSource.Bonuses);
            }
        }
    }

    private static string[] ExtractTargetEnhancementNames(IEnumerable<string> requiresTokens, IEnumerable<string> candidateMemberNames)
    {
        var joined = string.Join(" ", requiresTokens.Where(token => !string.IsNullOrWhiteSpace(token)));
        return candidateMemberNames
            .Where(name =>
                requiresTokens.Any(token => string.Equals(token, name, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(joined) && joined.Contains(name, StringComparison.OrdinalIgnoreCase)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static NormalizedSetBonusKind DetermineSetBonusKind(NormalizedSetBonusEntry bonus, string groupName)
    {
        if (bonus.TargetEnhancementNames.Count > 0)
        {
            var powerNames = bonus.AutoPowers
                .Concat(string.IsNullOrWhiteSpace(bonus.BonusPower) ? [] : [bonus.BonusPower]);
            return powerNames.Any(power => power.Contains(".Global_Bonus.", StringComparison.OrdinalIgnoreCase))
                ? NormalizedSetBonusKind.GlobalUnique
                : IsPetSetGroup(groupName)
                    ? NormalizedSetBonusKind.PetSpecial
                    : NormalizedSetBonusKind.MemberSpecific;
        }

        return NormalizedSetBonusKind.Standard;
    }

    private static bool IsPetSetGroup(string groupName)
    {
        return !string.IsNullOrWhiteSpace(groupName) &&
               groupName.Contains("Pet", StringComparison.OrdinalIgnoreCase);
    }

    private static Enums.ePvX DetermineSetBonusPvMode(NormalizedSetBonusEntry bonus)
    {
        var powerNames = bonus.AutoPowers
            .Concat(string.IsNullOrWhiteSpace(bonus.BonusPower) ? [] : [bonus.BonusPower])
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
        if (powerNames.Length == 0)
        {
            return Enums.ePvX.Any;
        }

        var sawPvpOnlyPower = false;
        var sawNonPvpPower = false;
        foreach (var powerName in powerNames)
        {
            if (powerName.StartsWith("Set_Bonus.PVP_Set_Bonus.", StringComparison.OrdinalIgnoreCase))
            {
                sawPvpOnlyPower = true;
            }
            else
            {
                sawNonPvpPower = true;
            }
        }

        return sawPvpOnlyPower && !sawNonPvpPower ? Enums.ePvX.PvP : Enums.ePvX.Any;
    }

    private static string ChooseSourceKey(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return string.Empty;
    }
}

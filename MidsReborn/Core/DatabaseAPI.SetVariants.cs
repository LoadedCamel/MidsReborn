using System;
using System.Collections.Generic;
using System.Linq;
using Mids_Reborn.Core.Omni;
using Newtonsoft.Json.Linq;

namespace Mids_Reborn.Core
{
    public enum SetVariantKind
    {
        Crafted = 0,
        Attuned = 1,
        Superior = 2,
        SuperiorAttuned = 3
    }

    public sealed class SetPieceProjection
    {
        public int PieceIndex { get; init; }
        public string DisplayLabel { get; init; } = string.Empty;
        public int[] RawMemberPositions { get; init; } = [];
        public IReadOnlyDictionary<SetVariantKind, int> Variants { get; init; } = new Dictionary<SetVariantKind, int>();
    }

    public sealed class EnhancementSetProjection
    {
        public int SetId { get; init; } = -1;
        public IReadOnlyList<SetPieceProjection> VisiblePieces { get; init; } = [];
        public IReadOnlyDictionary<int, int> EnhancementIdToPieceIndex { get; init; } = new Dictionary<int, int>();
        public IReadOnlyDictionary<int, int> EnhancementIdToRawMemberPosition { get; init; } = new Dictionary<int, int>();
        public IReadOnlyList<SetVariantKind> AvailableVariants { get; init; } = [];
    }

    public static partial class DatabaseAPI
    {
        private sealed class SetPieceProjectionBuilder
        {
            public required string DisplayLabel { get; init; }
            public List<int> RawMemberPositions { get; } = [];
            public Dictionary<SetVariantKind, int> Variants { get; } = new();
            public List<int> EnhancementIds { get; } = [];
            public int SortOrder => RawMemberPositions.Count > 0 ? RawMemberPositions.Min() : int.MaxValue;
        }

        private static readonly Dictionary<int, EnhancementSetProjection> SetProjectionCache = new();
        private static readonly Dictionary<int, (int SetId, int PieceIndex)> SetPieceLookupCache = new();
        private static readonly Dictionary<int, (bool Found, Recipe.RecipeRarity Rarity)> ImportedSetRarityCache = new();

        public static EnhancementSetProjection GetEnhancementSetProjection(int setId)
        {
            if (setId < 0 || Database?.EnhancementSets == null || setId >= Database.EnhancementSets.Count)
            {
                return new EnhancementSetProjection { SetId = setId };
            }

            if (SetProjectionCache.TryGetValue(setId, out var cachedProjection))
            {
                return cachedProjection;
            }

            var enhancementSet = Database.EnhancementSets[setId];
            var builders = new List<SetPieceProjectionBuilder>();
            var enhancementIdToRawMemberPosition = new Dictionary<int, int>();

            for (var rawMemberPosition = 0; rawMemberPosition < enhancementSet.Enhancements.Length; rawMemberPosition++)
            {
                var enhancementId = enhancementSet.Enhancements[rawMemberPosition];
                if (enhancementId < 0 || enhancementId >= Database.Enhancements.Length)
                {
                    continue;
                }

                var enhancement = Database.Enhancements[enhancementId];
                var displayLabel = GetSetPieceDisplayLabel(enhancementSet, enhancement, rawMemberPosition);
                var variantKind = GetSetVariantKind(enhancementId);
                var builder = builders.FirstOrDefault(piece =>
                    string.Equals(piece.DisplayLabel, displayLabel, StringComparison.OrdinalIgnoreCase) &&
                    !piece.Variants.ContainsKey(variantKind));

                if (builder == null)
                {
                    builder = new SetPieceProjectionBuilder { DisplayLabel = displayLabel };
                    builders.Add(builder);
                }

                builder.RawMemberPositions.Add(rawMemberPosition);
                builder.EnhancementIds.Add(enhancementId);
                builder.Variants.TryAdd(variantKind, enhancementId);
                enhancementIdToRawMemberPosition[enhancementId] = rawMemberPosition;
            }

            var orderedPieces = builders
                .OrderBy(piece => piece.SortOrder)
                .ThenBy(piece => piece.DisplayLabel, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var visiblePieces = new List<SetPieceProjection>(orderedPieces.Length);
            var enhancementIdToPieceIndex = new Dictionary<int, int>(enhancementIdToRawMemberPosition.Count);
            var availableVariants = new SortedSet<SetVariantKind>(Comparer<SetVariantKind>.Create((left, right) => GetSetVariantSortKey(left).CompareTo(GetSetVariantSortKey(right))));

            for (var pieceIndex = 0; pieceIndex < orderedPieces.Length; pieceIndex++)
            {
                var piece = orderedPieces[pieceIndex];
                foreach (var enhancementId in piece.EnhancementIds)
                {
                    enhancementIdToPieceIndex[enhancementId] = pieceIndex;
                    SetPieceLookupCache[enhancementId] = (setId, pieceIndex);
                }

                foreach (var variant in piece.Variants.Keys)
                {
                    availableVariants.Add(variant);
                }

                visiblePieces.Add(new SetPieceProjection
                {
                    PieceIndex = pieceIndex,
                    DisplayLabel = piece.DisplayLabel,
                    RawMemberPositions = piece.RawMemberPositions.OrderBy(position => position).ToArray(),
                    Variants = new Dictionary<SetVariantKind, int>(piece.Variants)
                });
            }

            var projection = new EnhancementSetProjection
            {
                SetId = setId,
                VisiblePieces = visiblePieces,
                EnhancementIdToPieceIndex = enhancementIdToPieceIndex,
                EnhancementIdToRawMemberPosition = enhancementIdToRawMemberPosition,
                AvailableVariants = availableVariants.ToArray()
            };

            SetProjectionCache[setId] = projection;
            return projection;
        }

        public static bool TryGetSetPieceIndexForEnhancement(int enhancementId, out int setId, out int pieceIndex)
        {
            setId = -1;
            pieceIndex = -1;

            if (enhancementId < 0 || enhancementId >= Database.Enhancements.Length)
            {
                return false;
            }

            if (SetPieceLookupCache.TryGetValue(enhancementId, out var cachedPiece))
            {
                setId = cachedPiece.SetId;
                pieceIndex = cachedPiece.PieceIndex;
                return true;
            }

            var enhancement = Database.Enhancements[enhancementId];
            if (enhancement.nIDSet < 0)
            {
                return false;
            }

            var projection = GetEnhancementSetProjection(enhancement.nIDSet);
            if (!projection.EnhancementIdToPieceIndex.TryGetValue(enhancementId, out pieceIndex))
            {
                return false;
            }

            setId = enhancement.nIDSet;
            SetPieceLookupCache[enhancementId] = (setId, pieceIndex);
            return true;
        }

        public static bool TryGetSetRawMemberPositionForEnhancement(int enhancementId, out int setId, out int rawMemberPosition)
        {
            setId = -1;
            rawMemberPosition = -1;

            if (enhancementId < 0 || enhancementId >= Database.Enhancements.Length)
            {
                return false;
            }

            var enhancement = Database.Enhancements[enhancementId];
            if (enhancement.nIDSet < 0)
            {
                return false;
            }

            var projection = GetEnhancementSetProjection(enhancement.nIDSet);
            if (!projection.EnhancementIdToRawMemberPosition.TryGetValue(enhancementId, out rawMemberPosition))
            {
                return false;
            }

            setId = enhancement.nIDSet;
            return true;
        }

        public static IReadOnlyList<SetVariantKind> GetAvailableSetVariants(int setId)
        {
            return GetEnhancementSetProjection(setId).AvailableVariants;
        }

        public static int ResolveEnhancementVariantForSetPiece(int setId, int pieceIndex, SetVariantKind variantKind)
        {
            var projection = GetEnhancementSetProjection(setId);
            if (pieceIndex < 0 || pieceIndex >= projection.VisiblePieces.Count)
            {
                return -1;
            }

            return projection.VisiblePieces[pieceIndex].Variants.TryGetValue(variantKind, out var enhancementId)
                ? enhancementId
                : -1;
        }

        public static int GetVisibleSetPieceCount(int setId)
        {
            return GetEnhancementSetProjection(setId).VisiblePieces.Count;
        }

        public static bool TryGetEnhancementResolvedRarity(int enhancementId, out Recipe.RecipeRarity rarity)
        {
            rarity = Recipe.RecipeRarity.Common;

            if (enhancementId < 0 || enhancementId >= Database.Enhancements.Length)
            {
                return false;
            }

            var enhancement = Database.Enhancements[enhancementId];
            if (enhancement.nIDSet < 0)
            {
                return TryGetDirectOrBaseRecipeRarity(enhancementId, out rarity);
            }

            if (TryGetImportedEnhancementSetRarity(enhancement.nIDSet, out rarity))
            {
                return true;
            }

            if (TryGetDirectOrBaseRecipeRarity(enhancementId, out rarity))
            {
                return true;
            }

            if (TryGetSetPieceIndexForEnhancement(enhancementId, out var setId, out var pieceIndex))
            {
                var projection = GetEnhancementSetProjection(setId);
                if (pieceIndex >= 0 && pieceIndex < projection.VisiblePieces.Count)
                {
                    foreach (var candidateEnhancementId in projection.VisiblePieces[pieceIndex].Variants
                                 .OrderBy(entry => GetSetVariantSortKey(entry.Key))
                                 .Select(entry => entry.Value))
                    {
                        if (TryGetDirectOrBaseRecipeRarity(candidateEnhancementId, out rarity))
                        {
                            return true;
                        }
                    }
                }
            }

            return TryGetEnhancementSetResolvedRarity(enhancement.nIDSet, out rarity);
        }

        public static bool TryGetEnhancementSetResolvedRarity(int setId, out Recipe.RecipeRarity rarity)
        {
            rarity = Recipe.RecipeRarity.Common;

            if (setId < 0 || setId >= Database.EnhancementSets.Count)
            {
                return false;
            }

            if (TryGetImportedEnhancementSetRarity(setId, out rarity))
            {
                return true;
            }

            var found = false;
            var projection = GetEnhancementSetProjection(setId);

            foreach (var piece in projection.VisiblePieces)
            {
                foreach (var candidateEnhancementId in piece.Variants
                             .OrderBy(entry => GetSetVariantSortKey(entry.Key))
                             .Select(entry => entry.Value))
                {
                    if (!TryGetDirectOrBaseRecipeRarity(candidateEnhancementId, out var candidateRarity))
                    {
                        continue;
                    }

                    if (!found || candidateRarity > rarity)
                    {
                        rarity = candidateRarity;
                    }

                    found = true;
                    break;
                }
            }

            if (found)
            {
                return true;
            }

            foreach (var candidateEnhancementId in Database.EnhancementSets[setId].Enhancements)
            {
                if (!TryGetDirectOrBaseRecipeRarity(candidateEnhancementId, out var candidateRarity))
                {
                    continue;
                }

                if (!found || candidateRarity > rarity)
                {
                    rarity = candidateRarity;
                }

                found = true;
            }

            return found;
        }

        private static bool TryGetImportedEnhancementSetRarity(int setId, out Recipe.RecipeRarity rarity)
        {
            rarity = Recipe.RecipeRarity.Common;

            if (setId < 0 || setId >= Database.EnhancementSets.Count)
            {
                return false;
            }

            if (ImportedSetRarityCache.TryGetValue(setId, out var cached))
            {
                rarity = cached.Rarity;
                return cached.Found;
            }

            var found = TryGetImportedEnhancementSetRarity(Database.EnhancementSets[setId], out rarity);
            ImportedSetRarityCache[setId] = (found, rarity);
            return found;
        }

        public static IReadOnlyList<int> GetOrderedRepeatSetEnhancementCandidates(int setId, int currentEnhancementId, SetVariantKind? variantKind = null)
        {
            if (setId < 0 || currentEnhancementId < 0)
            {
                return [];
            }

            var projection = GetEnhancementSetProjection(setId);
            if (projection.VisiblePieces.Count < 2 ||
                !TryGetSetPieceIndexForEnhancement(currentEnhancementId, out var currentSetId, out var currentPieceIndex) ||
                currentSetId != setId)
            {
                return [];
            }

            var effectiveVariant = variantKind ?? GetSetVariantKind(currentEnhancementId);
            var candidates = new List<int>(projection.VisiblePieces.Count - 1);

            for (var offset = 1; offset < projection.VisiblePieces.Count; offset++)
            {
                var pieceIndex = (currentPieceIndex + offset) % projection.VisiblePieces.Count;
                var enhancementId = ResolveEnhancementVariantForSetPiece(setId, pieceIndex, effectiveVariant);
                if (enhancementId >= 0)
                {
                    candidates.Add(enhancementId);
                }
            }

            return candidates;
        }

        public static int CountDistinctVisibleSetPieces(int setId, IEnumerable<int> enhancementIds)
        {
            if (enhancementIds == null)
            {
                return 0;
            }

            var projection = GetEnhancementSetProjection(setId);
            if (projection.VisiblePieces.Count == 0)
            {
                return 0;
            }

            var distinctPieceIndexes = new HashSet<int>();
            foreach (var enhancementId in enhancementIds)
            {
                if (projection.EnhancementIdToPieceIndex.TryGetValue(enhancementId, out var pieceIndex))
                {
                    distinctPieceIndexes.Add(pieceIndex);
                }
            }

            return distinctPieceIndexes.Count;
        }

        public static bool AreEnhancementsSameSetPiece(int leftEnhancementId, int rightEnhancementId)
        {
            if (leftEnhancementId < 0 || rightEnhancementId < 0)
            {
                return false;
            }

            if (leftEnhancementId == rightEnhancementId)
            {
                return true;
            }

            if (!TryGetSetPieceIndexForEnhancement(leftEnhancementId, out var leftSetId, out var leftPieceIndex) ||
                !TryGetSetPieceIndexForEnhancement(rightEnhancementId, out var rightSetId, out var rightPieceIndex))
            {
                return false;
            }

            return leftSetId == rightSetId && leftPieceIndex == rightPieceIndex;
        }

        public static SetVariantKind GetSetVariantKind(int enhancementId)
        {
            if (enhancementId < 0 || enhancementId >= Database.Enhancements.Length)
            {
                return SetVariantKind.Crafted;
            }

            var enhancement = Database.Enhancements[enhancementId];
            var isSuperior = enhancement.Superior ||
                             (enhancement.UID?.StartsWith("Superior_", StringComparison.OrdinalIgnoreCase) ?? false);
            var isAttuned = enhancement.GetPower()?.BoostUsePlayerLevel == true ||
                            (enhancement.UID?.StartsWith("Attuned_", StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (enhancement.UID?.StartsWith("Superior_Attuned_", StringComparison.OrdinalIgnoreCase) ?? false);

            return (isSuperior, isAttuned) switch
            {
                (true, true) => SetVariantKind.SuperiorAttuned,
                (true, false) => SetVariantKind.Superior,
                (false, true) => SetVariantKind.Attuned,
                _ => SetVariantKind.Crafted
            };
        }

        public static bool IsAttunedSetVariant(int enhancementId)
        {
            var variantKind = GetSetVariantKind(enhancementId);
            return variantKind is SetVariantKind.Attuned or SetVariantKind.SuperiorAttuned;
        }

        public static int GetSpecialRawMemberPositionForSetPiece(int setId, int pieceIndex)
        {
            var projection = GetEnhancementSetProjection(setId);
            if (pieceIndex < 0 || pieceIndex >= projection.VisiblePieces.Count)
            {
                return -1;
            }

            var enhancementSet = Database.EnhancementSets[setId];
            foreach (var rawMemberPosition in projection.VisiblePieces[pieceIndex].RawMemberPositions)
            {
                if (rawMemberPosition >= 0 &&
                    rawMemberPosition < enhancementSet.SpecialBonus.Length &&
                    enhancementSet.SpecialBonus[rawMemberPosition].Index.Length > 0)
                {
                    return rawMemberPosition;
                }
            }

            return -1;
        }

        public static int GetSetVariantSortKey(SetVariantKind variantKind)
        {
            return variantKind switch
            {
                SetVariantKind.Crafted => 0,
                SetVariantKind.Attuned => 1,
                SetVariantKind.Superior => 2,
                SetVariantKind.SuperiorAttuned => 3,
                _ => int.MaxValue
            };
        }

        private static bool TryGetDirectOrBaseRecipeRarity(int enhancementId, out Recipe.RecipeRarity rarity)
        {
            if (TryGetDirectRecipeRarity(enhancementId, out rarity))
            {
                return true;
            }

            rarity = Recipe.RecipeRarity.Common;
            if (enhancementId < 0 || enhancementId >= Database.Enhancements.Length)
            {
                return false;
            }

            var baseUid = GetEnhancementBaseUIDName(Database.Enhancements[enhancementId].UID);
            if (string.IsNullOrWhiteSpace(baseUid))
            {
                return false;
            }

            var baseEnhancementId = GetEnhancementByUIDName(baseUid);
            return baseEnhancementId >= 0 &&
                   baseEnhancementId != enhancementId &&
                   TryGetDirectRecipeRarity(baseEnhancementId, out rarity);
        }

        private static bool TryGetDirectRecipeRarity(int enhancementId, out Recipe.RecipeRarity rarity)
        {
            rarity = Recipe.RecipeRarity.Common;

            if (enhancementId < 0 || enhancementId >= Database.Enhancements.Length)
            {
                return false;
            }

            var recipeIndex = Database.Enhancements[enhancementId].RecipeIDX;
            if (recipeIndex < 0 || recipeIndex >= Database.Recipes.Length)
            {
                return false;
            }

            rarity = Database.Recipes[recipeIndex].Rarity;
            return true;
        }

        private static bool TryGetImportedEnhancementSetRarity(EnhancementSet enhancementSet, out Recipe.RecipeRarity rarity)
        {
            rarity = Recipe.RecipeRarity.Common;

            if (enhancementSet == null)
            {
                return false;
            }

            var metadata = Database?.EnhancementImportMetadata;
            if (metadata == null)
            {
                return false;
            }

            return TryGetImportedEnhancementSetRarityLabel(metadata, enhancementSet, out var rarityLabel) &&
                   TryMapImportedRarityLabel(rarityLabel, out rarity);
        }

        private static bool TryGetImportedEnhancementSetRarityLabel(
            EnhancementImportMetadata metadata,
            EnhancementSet enhancementSet,
            out string rarityLabel)
        {
            rarityLabel = string.Empty;
            foreach (var candidateKey in GetEnhancementSetLookupKeys(enhancementSet))
            {
                if (metadata.EnhancementSetSourceRarityNames.TryGetValue(candidateKey, out rarityLabel) &&
                    !string.IsNullOrWhiteSpace(rarityLabel))
                {
                    return true;
                }
            }

            if (!TryGetImportedEnhancementSetRarityLabel(metadata.EnhancementSetGroups, enhancementSet, out rarityLabel))
            {
                return false;
            }

            foreach (var candidateKey in GetEnhancementSetLookupKeys(enhancementSet))
            {
                metadata.EnhancementSetSourceRarityNames[candidateKey] = rarityLabel;
            }

            return true;
        }

        private static bool TryGetImportedEnhancementSetRarityLabel(
            JToken? enhancementSetGroups,
            EnhancementSet enhancementSet,
            out string rarityLabel)
        {
            rarityLabel = string.Empty;
            if (enhancementSetGroups is not JObject groups)
            {
                return false;
            }

            var lookupKeys = new HashSet<string>(
                GetEnhancementSetLookupKeys(enhancementSet)
                    .Where(candidateKey => !string.IsNullOrWhiteSpace(candidateKey)),
                StringComparer.OrdinalIgnoreCase);
            if (lookupKeys.Count == 0)
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
                    var name = entry.Value<string>("name") ?? string.Empty;
                    var displayName = entry.Value<string>("display_name") ?? string.Empty;
                    if (!lookupKeys.Contains(name) && !lookupKeys.Contains(displayName))
                    {
                        continue;
                    }

                    if (TryGetRarityLabelFromConversions(entry["conversions"], out rarityLabel))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static IEnumerable<string> GetEnhancementSetLookupKeys(EnhancementSet enhancementSet)
        {
            if (!string.IsNullOrWhiteSpace(enhancementSet.Uid))
            {
                yield return enhancementSet.Uid;
            }

            if (!string.IsNullOrWhiteSpace(enhancementSet.DisplayName))
            {
                yield return enhancementSet.DisplayName;
            }

            if (!string.IsNullOrWhiteSpace(enhancementSet.ShortName))
            {
                yield return enhancementSet.ShortName;
            }
        }

        private static bool TryGetRarityLabelFromConversions(JToken? conversionsToken, out string rarityLabel)
        {
            rarityLabel = string.Empty;
            if (conversionsToken is not JArray conversions)
            {
                return false;
            }

            var conversionValues = conversions
                .Values<string>()
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .ToArray();

            if (TryInferRarityLabelFromConversions(conversionValues, out rarityLabel))
            {
                return true;
            }

            foreach (var conversionValue in conversionValues)
            {
                if (!conversionValue.StartsWith("Rarity:", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                rarityLabel = conversionValue[(conversionValue.IndexOf(':') + 1)..].Trim();
                return !string.IsNullOrWhiteSpace(rarityLabel);
            }

            return false;
        }

        private static bool TryInferRarityLabelFromConversions(
            IEnumerable<string> conversionValues,
            out string rarityLabel)
        {
            rarityLabel = string.Empty;
            if (conversionValues == null)
            {
                return false;
            }

            foreach (var conversionValue in conversionValues)
            {
                if (string.Equals(conversionValue, "Category: Universal Damage", StringComparison.OrdinalIgnoreCase))
                {
                    rarityLabel = "Rare";
                    return true;
                }
            }

            return false;
        }

        private static bool TryMapImportedRarityLabel(string rarityLabel, out Recipe.RecipeRarity rarity)
        {
            rarity = Recipe.RecipeRarity.Common;
            if (string.IsNullOrWhiteSpace(rarityLabel))
            {
                return false;
            }

            var normalized = new string(rarityLabel.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
            rarity = normalized switch
            {
                "common" => Recipe.RecipeRarity.Common,
                "uncommon" => Recipe.RecipeRarity.Uncommon,
                "rare" => Recipe.RecipeRarity.Rare,
                "veryrare" => Recipe.RecipeRarity.UltraRare,
                "ultrarare" => Recipe.RecipeRarity.UltraRare,
                "archetype" => Recipe.RecipeRarity.Rare,
                "superiorarchetype" => Recipe.RecipeRarity.UltraRare,
                "winterpackseries" => Recipe.RecipeRarity.Rare,
                "superiorwinterpackseries" => Recipe.RecipeRarity.UltraRare,
                _ => Recipe.RecipeRarity.Common
            };

            return normalized is
                "common" or
                "uncommon" or
                "rare" or
                "veryrare" or
                "ultrarare" or
                "archetype" or
                "superiorarchetype" or
                "winterpackseries" or
                "superiorwinterpackseries";
        }

        private static string GetSetPieceDisplayLabel(EnhancementSet enhancementSet, IEnhancement enhancement, int rawMemberPosition)
        {
            var label = enhancement.Name?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(enhancementSet.DisplayName))
            {
                var prefix = enhancementSet.DisplayName.Trim() + ":";
                if (label.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    label = label[prefix.Length..].TrimStart();
                }
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                label = enhancement.ShortName?.Trim();
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                label = $"Piece {rawMemberPosition + 1}";
            }

            return label;
        }

        private static void ClearSetProjectionCache()
        {
            SetProjectionCache.Clear();
            SetPieceLookupCache.Clear();
            ImportedSetRarityCache.Clear();
        }
    }
}

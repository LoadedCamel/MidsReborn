using System;
using System.Collections.Generic;
using System.Linq;

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
        }
    }
}

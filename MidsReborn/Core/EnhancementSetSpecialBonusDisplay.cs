using System;
using System.Collections.Generic;
using System.Linq;

namespace Mids_Reborn.Core
{
    internal sealed class EnhancementSetSpecialBonusDisplayRow
    {
        public required IReadOnlyList<string> EffectStrings { get; init; }
        public required HashSet<int> PieceIndexes { get; init; }
        public Enums.ePvX PvMode { get; init; }
    }

    internal static class EnhancementSetSpecialBonusDisplay
    {
        internal static IReadOnlyList<EnhancementSetSpecialBonusDisplayRow> BuildRows(EnhancementSet enhancementSet)
        {
            return BuildRows(enhancementSet, ResolveSetId(enhancementSet));
        }

        internal static IReadOnlyList<EnhancementSetSpecialBonusDisplayRow> BuildRows(EnhancementSet enhancementSet, int setId)
        {
            if (enhancementSet == null)
            {
                return Array.Empty<EnhancementSetSpecialBonusDisplayRow>();
            }

            var rows = new List<EnhancementSetSpecialBonusDisplayRow>();
            var rowIndexes = new Dictionary<string, int>(StringComparer.Ordinal);

            if (setId >= 0)
            {
                var projection = DatabaseAPI.GetEnhancementSetProjection(setId);
                for (var pieceIndex = 0; pieceIndex < projection.VisiblePieces.Count; pieceIndex++)
                {
                    var rawMemberPosition = DatabaseAPI.GetSpecialRawMemberPositionForSetPiece(setId, pieceIndex);
                    if (rawMemberPosition < 0)
                    {
                        continue;
                    }

                    AddRow(rows, rowIndexes, enhancementSet, rawMemberPosition, pieceIndex);
                }
            }

            if (rows.Count > 0)
            {
                return rows;
            }

            for (var rawMemberPosition = 0; rawMemberPosition < enhancementSet.SpecialBonus.Length; rawMemberPosition++)
            {
                AddRow(rows, rowIndexes, enhancementSet, rawMemberPosition, rawMemberPosition);
            }

            return rows;
        }

        private static int ResolveSetId(EnhancementSet enhancementSet)
        {
            foreach (var enhancementId in enhancementSet.Enhancements)
            {
                if (enhancementId < 0 || enhancementId >= DatabaseAPI.Database.Enhancements.Length)
                {
                    continue;
                }

                var setId = DatabaseAPI.Database.Enhancements[enhancementId].nIDSet;
                if (setId >= 0)
                {
                    return setId;
                }
            }

            return DatabaseAPI.Database.EnhancementSets.IndexOf(enhancementSet);
        }

        private static void AddRow(
            List<EnhancementSetSpecialBonusDisplayRow> rows,
            Dictionary<string, int> rowIndexes,
            EnhancementSet enhancementSet,
            int rawMemberPosition,
            int pieceIndex)
        {
            if (rawMemberPosition < 0 || rawMemberPosition >= enhancementSet.SpecialBonus.Length)
            {
                return;
            }

            var effectStrings = GetDisplayStrings(enhancementSet, rawMemberPosition);
            if (effectStrings.Count == 0)
            {
                return;
            }

            var effectKey = string.Join(
                "\u001f",
                effectStrings.OrderBy(effectString => effectString, StringComparer.Ordinal));
            if (!rowIndexes.TryGetValue(effectKey, out var rowIndex))
            {
                rows.Add(new EnhancementSetSpecialBonusDisplayRow
                {
                    EffectStrings = effectStrings,
                    PieceIndexes = new HashSet<int> { pieceIndex },
                    PvMode = enhancementSet.SpecialBonus[rawMemberPosition].PvMode
                });
                rowIndexes[effectKey] = rows.Count - 1;
                return;
            }

            rows.ElementAt(rowIndex).PieceIndexes.Add(pieceIndex);
        }

        private static IReadOnlyList<string> GetDisplayStrings(EnhancementSet enhancementSet, int rawMemberPosition)
        {
            var effectStrings = enhancementSet.GetPopupEffectStrings(
                rawMemberPosition,
                true,
                true,
                ShouldCheckStatus(enhancementSet, rawMemberPosition));
            if (effectStrings.Count > 0)
            {
                return effectStrings;
            }

            var fallbackLabels = enhancementSet.GetEnhancementSetLinkedPowers(rawMemberPosition, true)
                .Select(power => NormalizeFallbackLabel(power?.DisplayName, enhancementSet.DisplayName))
                .Where(label => !string.IsNullOrWhiteSpace(label))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return fallbackLabels.Length == 0
                ? Array.Empty<string>()
                : fallbackLabels;
        }

        private static string NormalizeFallbackLabel(string? label, string? setDisplayName)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return string.Empty;
            }

            var normalized = label.Trim();
            if (!string.IsNullOrWhiteSpace(setDisplayName))
            {
                var prefix = $"{setDisplayName.Trim()}:";
                if (normalized.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    normalized = normalized[prefix.Length..].Trim();
                }
            }

            return GroupedFx.FormatPresentationText(normalized);
        }

        private static bool ShouldCheckStatus(EnhancementSet enhancementSet, int rawMemberPosition)
        {
            if (rawMemberPosition < 0 || rawMemberPosition >= enhancementSet.SpecialBonus.Length)
            {
                return false;
            }

            var specialPowers = enhancementSet.GetEnhancementSetLinkedPowers(rawMemberPosition, true);
            return specialPowers is { Count: 1 } &&
                   (specialPowers[0].FullName.Contains("Skin", StringComparison.Ordinal) ||
                    specialPowers[0].FullName.Contains("Aegis", StringComparison.Ordinal));
        }
    }
}

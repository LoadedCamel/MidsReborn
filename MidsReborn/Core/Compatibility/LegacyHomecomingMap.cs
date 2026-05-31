using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mids_Reborn.Core.Compatibility
{
    internal sealed class LegacyHomecomingMap
    {
        private static readonly Lazy<LegacyHomecomingMap> LazyInstance =
            new(() => new LegacyHomecomingMap(AppDataPaths.LegacyLoadMap, strictValidation: false));

        private readonly Dictionary<int, IReadOnlyList<LegacyMapEntry>> _powerEntriesByStaticIndex;
        private readonly Dictionary<int, IReadOnlyList<LegacyMapEntry>> _enhancementEntriesByStaticIndex;
        private readonly Dictionary<string, IReadOnlyList<LegacyMapEntry>> _powerEntriesByFullName;
        private readonly Dictionary<string, IReadOnlyList<LegacyMapEntry>> _powerEntriesByName;
        private readonly Dictionary<string, IReadOnlyList<LegacyMapEntry>> _enhancementEntriesByFullName;
        private readonly Dictionary<string, IReadOnlyList<LegacyMapEntry>> _enhancementEntriesByName;
        private readonly Dictionary<string, IReadOnlyList<LegacyMapEntry>> _enhancementSetEntriesByFullName;
        private readonly Dictionary<string, IReadOnlyList<LegacyMapEntry>> _enhancementSetEntriesByName;
        private readonly Dictionary<int, LegacyEnhancementEncoding> _enhancementEncodingByStaticIndex;

        private LegacyHomecomingMap(string path, bool strictValidation)
        {
            SourcePath = path;
            var entries = LoadEntries(path, strictValidation);
            Entries = entries;
            _powerEntriesByStaticIndex = BuildStaticIndexLookup(entries, LegacyMapKind.Power);
            _enhancementEntriesByStaticIndex = BuildStaticIndexLookup(entries, LegacyMapKind.Enhancement);
            _powerEntriesByFullName = BuildValueLookup(entries, LegacyMapKind.Power, entry => entry.LegacyFullName);
            _powerEntriesByName = BuildValueLookup(entries, LegacyMapKind.Power, entry => entry.LegacyName);
            _enhancementEntriesByFullName = BuildValueLookup(entries, LegacyMapKind.Enhancement, entry => entry.LegacyFullName);
            _enhancementEntriesByName = BuildValueLookup(entries, LegacyMapKind.Enhancement, entry => entry.LegacyName);
            _enhancementSetEntriesByFullName = BuildValueLookup(entries, LegacyMapKind.EnhancementSet, entry => entry.LegacyFullName);
            _enhancementSetEntriesByName = BuildValueLookup(entries, LegacyMapKind.EnhancementSet, entry => entry.LegacyName);
            _enhancementEncodingByStaticIndex = BuildEnhancementEncodingLookup(_enhancementEntriesByStaticIndex);
        }

        public static LegacyHomecomingMap Instance => LazyInstance.Value;

        public LegacyLineage Lineage => LegacyLineage.Homecoming;
        public string SourcePath { get; }
        public IReadOnlyList<LegacyMapEntry> Entries { get; }

        public IReadOnlyList<LegacyMapEntry> GetPowerEntries(int staticIndex)
        {
            return _powerEntriesByStaticIndex.TryGetValue(staticIndex, out var entries)
                ? entries
                : Array.Empty<LegacyMapEntry>();
        }

        public IReadOnlyList<LegacyMapEntry> GetEnhancementEntries(int staticIndex)
        {
            return _enhancementEntriesByStaticIndex.TryGetValue(staticIndex, out var entries)
                ? entries
                : Array.Empty<LegacyMapEntry>();
        }

        public IReadOnlyList<LegacyMapEntry> FindPowerEntries(string? legacyFullName, string? legacyName)
        {
            return CombineMatches(
                FindMatches(_powerEntriesByFullName, legacyFullName),
                FindMatches(_powerEntriesByName, legacyName));
        }

        public IReadOnlyList<LegacyMapEntry> FindEnhancementEntries(string? legacyFullName, string? legacyName)
        {
            return CombineMatches(
                FindMatches(_enhancementEntriesByFullName, legacyFullName),
                FindMatches(_enhancementEntriesByName, legacyName));
        }

        public IReadOnlyList<LegacyMapEntry> FindEnhancementSetEntries(string? legacyFullName, string? legacyName)
        {
            return CombineMatches(
                FindMatches(_enhancementSetEntriesByFullName, legacyFullName),
                FindMatches(_enhancementSetEntriesByName, legacyName));
        }

        public bool TryGetEnhancementEncoding(int staticIndex, out LegacyEnhancementEncoding encoding)
        {
            if (_enhancementEncodingByStaticIndex.TryGetValue(staticIndex, out encoding))
            {
                return true;
            }

            if (!_enhancementEntriesByStaticIndex.TryGetValue(staticIndex, out var entries) || entries.Count == 0)
            {
                encoding = LegacyEnhancementEncoding.None;
                return false;
            }

            if (!TryBuildEnhancementEncoding(entries, out encoding))
            {
                return false;
            }

            _enhancementEncodingByStaticIndex[staticIndex] = encoding;
            return true;
        }

        private static IReadOnlyList<LegacyMapEntry> LoadEntries(string path, bool strictValidation)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The Homecoming legacy load-target map was not found at '{path}'.");
            }

            var entries = new List<LegacyMapEntry>();
            var dedupe = new Dictionary<string, LegacyMapEntry>(StringComparer.OrdinalIgnoreCase);
            var lines = File.ReadAllLines(path);
            for (var index = 0; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                var parts = line.Split('|');
                if (parts.Length != 5)
                {
                    throw new InvalidDataException(
                        $"Legacy load map line {index + 1} in '{path}' does not have 5 pipe-delimited columns.");
                }

                if (!Enum.TryParse(parts[0].Trim(), ignoreCase: true, out LegacyMapKind kind))
                {
                    throw new InvalidDataException(
                        $"Legacy load map line {index + 1} in '{path}' contains an unknown kind '{parts[0]}'.");
                }

                int? staticIndex = null;
                var staticIndexText = parts[1].Trim();
                if (kind == LegacyMapKind.EnhancementSet)
                {
                    if (!string.IsNullOrWhiteSpace(staticIndexText))
                    {
                        throw new InvalidDataException(
                            $"Legacy load map line {index + 1} in '{path}' must leave StaticIndex blank for enhancement sets.");
                    }
                }
                else
                {
                    if (!int.TryParse(staticIndexText, out var parsedStaticIndex))
                    {
                        throw new InvalidDataException(
                            $"Legacy load map line {index + 1} in '{path}' must include a valid static index.");
                    }

                    staticIndex = parsedStaticIndex;
                }

                var rawEntry = new LegacyMapEntry(
                    kind,
                    staticIndex,
                    parts[2].Trim(),
                    parts[3].Trim(),
                    parts[4].Trim(),
                    index + 1);

                var entry = CanonicalizeAndValidateCurrentTarget(rawEntry, path, strictValidation);

                var dedupeKey = string.Join("|",
                    entry.Kind,
                    entry.StaticIndex?.ToString() ?? string.Empty,
                    entry.LegacyName,
                    entry.LegacyFullName);
                if (dedupe.TryGetValue(dedupeKey, out var existingEntry))
                {
                    if (!string.Equals(existingEntry.CurrentTarget, entry.CurrentTarget, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidDataException(
                            $"Legacy load map line {index + 1} in '{path}' conflicts with line {existingEntry.SourceLine} for the same legacy key.");
                    }

                    continue;
                }

                dedupe[dedupeKey] = entry;
                entries.Add(entry);
            }

            return entries;
        }

        private static LegacyMapEntry CanonicalizeAndValidateCurrentTarget(LegacyMapEntry entry, string path, bool strictValidation)
        {
            switch (entry.Kind)
            {
                case LegacyMapKind.Power:
                    if (!LegacyMapResolver.TryCanonicalizeCurrentPower(entry.CurrentTarget, out var currentPowerUid))
                    {
                        if (!strictValidation)
                        {
                            return entry;
                        }

                        throw new InvalidDataException(
                            $"Legacy load map line {entry.SourceLine} in '{path}' targets unknown current power '{entry.CurrentTarget}'.");
                    }

                    return entry with { CurrentTarget = currentPowerUid };
                case LegacyMapKind.Enhancement:
                    if (!LegacyMapResolver.TryCanonicalizeCurrentEnhancement(entry.CurrentTarget, out var currentEnhancementUid))
                    {
                        if (!strictValidation)
                        {
                            return entry;
                        }

                        throw new InvalidDataException(
                            $"Legacy load map line {entry.SourceLine} in '{path}' targets unknown current enhancement '{entry.CurrentTarget}'.");
                    }

                    return entry with { CurrentTarget = currentEnhancementUid };
                case LegacyMapKind.EnhancementSet:
                    if (!LegacyMapResolver.TryCanonicalizeCurrentEnhancementSet(entry.CurrentTarget, out var currentSetUid))
                    {
                        if (!strictValidation)
                        {
                            return entry;
                        }

                        throw new InvalidDataException(
                            $"Legacy load map line {entry.SourceLine} in '{path}' targets unknown current enhancement set '{entry.CurrentTarget}'.");
                    }

                    return entry with { CurrentTarget = currentSetUid };
                default:
                    return entry;
            }
        }

        private static Dictionary<int, IReadOnlyList<LegacyMapEntry>> BuildStaticIndexLookup(
            IEnumerable<LegacyMapEntry> entries,
            LegacyMapKind kind)
        {
            return entries
                .Where(entry => entry.Kind == kind && entry.StaticIndex.HasValue)
                .GroupBy(entry => entry.StaticIndex!.Value)
                .ToDictionary(
                    group => group.Key,
                    group => (IReadOnlyList<LegacyMapEntry>)group
                        .OrderBy(entry => entry.LegacyFullName, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(entry => entry.LegacyName, StringComparer.OrdinalIgnoreCase)
                        .ToArray());
        }

        private static Dictionary<string, IReadOnlyList<LegacyMapEntry>> BuildValueLookup(
            IEnumerable<LegacyMapEntry> entries,
            LegacyMapKind kind,
            Func<LegacyMapEntry, string> selector)
        {
            return entries
                .Where(entry => entry.Kind == kind && !string.IsNullOrWhiteSpace(selector(entry)))
                .GroupBy(selector, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    group => group.Key,
                    group => (IReadOnlyList<LegacyMapEntry>)group
                        .OrderBy(entry => entry.CurrentTarget, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(entry => entry.SourceLine)
                        .ToArray(),
                    StringComparer.OrdinalIgnoreCase);
        }

        private static Dictionary<int, LegacyEnhancementEncoding> BuildEnhancementEncodingLookup(
            IReadOnlyDictionary<int, IReadOnlyList<LegacyMapEntry>> entriesByStaticIndex)
        {
            var lookup = new Dictionary<int, LegacyEnhancementEncoding>();
            foreach (var pair in entriesByStaticIndex)
            {
                if (!TryBuildEnhancementEncoding(pair.Value, out var encoding))
                {
                    continue;
                }

                lookup[pair.Key] = encoding;
            }

            return lookup;
        }

        private static bool TryBuildEnhancementEncoding(
            IReadOnlyList<LegacyMapEntry> entries,
            out LegacyEnhancementEncoding encoding)
        {
            var encodings = entries
                .Select(entry => LegacyMapResolver.TryCanonicalizeCurrentEnhancement(entry.CurrentTarget, out var canonicalEnhancementUid)
                    ? DatabaseAPI.NidFromUidEnh(canonicalEnhancementUid)
                    : -1)
                .Where(index => index >= 0)
                .Select(index => DatabaseAPI.Database.Enhancements[index].TypeID)
                .Select(ToEncoding)
                .Distinct()
                .ToArray();

            if (encodings.Length == 0)
            {
                encoding = LegacyEnhancementEncoding.None;
                return false;
            }

            if (encodings.Length != 1)
            {
                throw new InvalidDataException(
                    $"Legacy enhancement static index {entries[0].StaticIndex} maps to multiple enhancement encodings. The flat map must not mix enhancement storage kinds for a single static index.");
            }

            encoding = encodings[0];
            return true;
        }

        private static LegacyEnhancementEncoding ToEncoding(Enums.eType typeId)
        {
            return typeId switch
            {
                Enums.eType.Normal => LegacyEnhancementEncoding.NormalLike,
                Enums.eType.SpecialO => LegacyEnhancementEncoding.NormalLike,
                Enums.eType.InventO => LegacyEnhancementEncoding.InventLike,
                Enums.eType.SetO => LegacyEnhancementEncoding.InventLike,
                _ => LegacyEnhancementEncoding.None
            };
        }

        private static IReadOnlyList<LegacyMapEntry> FindMatches(
            IReadOnlyDictionary<string, IReadOnlyList<LegacyMapEntry>> lookup,
            string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Array.Empty<LegacyMapEntry>();
            }

            return lookup.TryGetValue(key.Trim(), out var entries)
                ? entries
                : Array.Empty<LegacyMapEntry>();
        }

        private static IReadOnlyList<LegacyMapEntry> CombineMatches(
            IReadOnlyList<LegacyMapEntry> first,
            IReadOnlyList<LegacyMapEntry> second)
        {
            return first
                .Concat(second)
                .GroupBy(entry => $"{entry.Kind}|{entry.StaticIndex}|{entry.LegacyName}|{entry.LegacyFullName}|{entry.CurrentTarget}",
                    StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(entry => entry.CurrentTarget, StringComparer.OrdinalIgnoreCase)
                .ThenBy(entry => entry.SourceLine)
                .ToArray();
        }
    }
}

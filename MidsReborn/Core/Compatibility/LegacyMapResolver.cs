using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mids_Reborn.Core.BuildFile.DataModels;

namespace Mids_Reborn.Core.Compatibility
{
    internal static class LegacyMapResolver
    {
        public static bool TryResolveLegacyPowerTarget(
            LegacyHomecomingMap map,
            int? staticIndex,
            string? legacyFullName,
            IReadOnlyCollection<string> rawPowerSets,
            IReadOnlyCollection<string> mappedCurrentPowerSets,
            string classUid,
            string? parentCurrentPowerUid,
            out string currentTarget,
            out LegacyMapEntry? matchedEntry,
            out string error)
        {
            currentTarget = string.Empty;
            matchedEntry = null;
            error = string.Empty;

            var candidates = new List<LegacyMapEntry>();
            if (staticIndex.HasValue)
            {
                candidates.AddRange(map.GetPowerEntries(staticIndex.Value));
            }

            if (candidates.Count == 0 && !string.IsNullOrWhiteSpace(legacyFullName))
            {
                candidates.AddRange(map.FindPowerEntries(legacyFullName, ExtractLeafName(legacyFullName)));
            }

            if (candidates.Count == 0 && TryCanonicalizeCurrentPower(legacyFullName, out currentTarget))
            {
                return true;
            }

            if (candidates.Count == 0)
            {
                error = staticIndex.HasValue
                    ? $"No Homecoming legacy power mapping exists for static index {staticIndex.Value}."
                    : $"No Homecoming legacy power mapping exists for '{legacyFullName}'.";
                return false;
            }

            var filteredCandidates = CanonicalizePowerCandidates(candidates);
            if (filteredCandidates.Count == 0)
            {
                error = staticIndex.HasValue
                    ? $"No valid current Homecoming power target exists for static index {staticIndex.Value}."
                    : $"No valid current Homecoming power target exists for '{legacyFullName}'.";
                return false;
            }

            filteredCandidates = FilterPowerCandidates(
                filteredCandidates,
                rawPowerSets,
                mappedCurrentPowerSets,
                classUid,
                parentCurrentPowerUid);

            if (filteredCandidates.Count == 1)
            {
                matchedEntry = filteredCandidates[0];
                currentTarget = matchedEntry.CurrentTarget;
                return true;
            }

            error = $"Legacy power reference '{legacyFullName ?? $"static index {staticIndex}"}' is ambiguous. Candidates: {DescribeCandidates(filteredCandidates)}";
            return false;
        }

        public static bool TryResolveNamedPowerTarget(
            LegacyHomecomingMap map,
            string? legacyFullName,
            IReadOnlyCollection<string> rawPowerSets,
            IReadOnlyCollection<string> mappedCurrentPowerSets,
            string classUid,
            string? parentCurrentPowerUid,
            out string currentTarget,
            out LegacyMapEntry? matchedEntry,
            out string error)
        {
            return TryResolvePowerTarget(
                map,
                null,
                legacyFullName,
                rawPowerSets,
                mappedCurrentPowerSets,
                classUid,
                parentCurrentPowerUid,
                out currentTarget,
                out matchedEntry,
                out error);
        }

        public static bool TryResolvePowerTarget(
            LegacyHomecomingMap map,
            int? staticIndex,
            string? legacyFullName,
            IReadOnlyCollection<string> rawPowerSets,
            IReadOnlyCollection<string> mappedCurrentPowerSets,
            string classUid,
            string? parentCurrentPowerUid,
            out string currentTarget,
            out LegacyMapEntry? matchedEntry,
            out string error)
        {
            currentTarget = string.Empty;
            matchedEntry = null;
            error = string.Empty;

            if (TryCanonicalizeCurrentPower(legacyFullName, out currentTarget))
            {
                return true;
            }

            var candidates = new List<LegacyMapEntry>();
            if (!string.IsNullOrWhiteSpace(legacyFullName))
            {
                candidates.AddRange(map.FindPowerEntries(legacyFullName, ExtractLeafName(legacyFullName)));
            }

            if (candidates.Count == 0 && staticIndex.HasValue)
            {
                candidates.AddRange(map.GetPowerEntries(staticIndex.Value));
            }

            if (candidates.Count == 0)
            {
                error = staticIndex.HasValue
                    ? $"No Homecoming legacy power mapping exists for static index {staticIndex.Value}."
                    : $"No Homecoming legacy power mapping exists for '{legacyFullName}'.";
                return false;
            }

            var filteredCandidates = CanonicalizePowerCandidates(candidates);
            if (filteredCandidates.Count == 0)
            {
                error = staticIndex.HasValue
                    ? $"No valid current Homecoming power target exists for static index {staticIndex.Value}."
                    : $"No valid current Homecoming power target exists for '{legacyFullName}'.";
                return false;
            }

            filteredCandidates = FilterPowerCandidates(
                filteredCandidates,
                rawPowerSets,
                mappedCurrentPowerSets,
                classUid,
                parentCurrentPowerUid);

            if (filteredCandidates.Count == 1)
            {
                matchedEntry = filteredCandidates[0];
                currentTarget = matchedEntry.CurrentTarget;
                return true;
            }

            error = $"Legacy power reference '{legacyFullName ?? $"static index {staticIndex}"}' is ambiguous. Candidates: {DescribeCandidates(filteredCandidates)}";
            return false;
        }

        public static bool TryResolveLegacyEnhancementTarget(
            LegacyHomecomingMap map,
            int? staticIndex,
            string? legacyFullName,
            string? legacyDisplayName,
            string currentPowerUid,
            IReadOnlyCollection<string> resolvedSetUids,
            out string currentTarget,
            out LegacyMapEntry? matchedEntry,
            out string error)
        {
            currentTarget = string.Empty;
            matchedEntry = null;
            error = string.Empty;

            var candidates = new List<LegacyMapEntry>();
            if (staticIndex.HasValue)
            {
                candidates.AddRange(map.GetEnhancementEntries(staticIndex.Value));
            }

            if (candidates.Count == 0 && !string.IsNullOrWhiteSpace(legacyFullName))
            {
                candidates.AddRange(map.FindEnhancementEntries(legacyFullName, ExtractLeafName(legacyFullName)));
            }

            if (candidates.Count == 0 && !string.IsNullOrWhiteSpace(legacyDisplayName))
            {
                candidates.AddRange(map.FindEnhancementEntries(legacyDisplayName, legacyDisplayName));
            }

            if (candidates.Count == 0 &&
                !string.IsNullOrWhiteSpace(legacyDisplayName) &&
                TryResolveEnhancementDisplayName(map, legacyDisplayName, currentPowerUid, resolvedSetUids, out currentTarget))
            {
                return true;
            }

            if (candidates.Count == 0 && TryCanonicalizeCurrentEnhancement(legacyFullName, out currentTarget))
            {
                return true;
            }

            if (candidates.Count == 0)
            {
                error = staticIndex.HasValue
                    ? $"No Homecoming legacy enhancement mapping exists for static index {staticIndex.Value}."
                    : $"No Homecoming legacy enhancement mapping exists for '{DescribeLegacyEnhancementReference(legacyFullName, legacyDisplayName, staticIndex)}'.";
                return false;
            }

            var filteredCandidates = CanonicalizeEnhancementCandidates(candidates);
            if (filteredCandidates.Count == 0)
            {
                error = staticIndex.HasValue
                    ? $"No valid current Homecoming enhancement target exists for static index {staticIndex.Value}."
                    : $"No valid current Homecoming enhancement target exists for '{DescribeLegacyEnhancementReference(legacyFullName, legacyDisplayName, staticIndex)}'.";
                return false;
            }

            filteredCandidates = FilterEnhancementCandidates(filteredCandidates, currentPowerUid, resolvedSetUids);

            if (filteredCandidates.Count == 1)
            {
                matchedEntry = filteredCandidates[0];
                currentTarget = matchedEntry.CurrentTarget;
                return true;
            }

            error = $"Legacy enhancement reference '{DescribeLegacyEnhancementReference(legacyFullName, legacyDisplayName, staticIndex)}' is ambiguous. Candidates: {DescribeCandidates(filteredCandidates)}";
            return false;
        }

        public static bool TryResolveNamedEnhancementTarget(
            LegacyHomecomingMap map,
            string? legacyFullName,
            string? legacyDisplayName,
            string currentPowerUid,
            IReadOnlyCollection<string> resolvedSetUids,
            out string currentTarget,
            out LegacyMapEntry? matchedEntry,
            out string error)
        {
            return TryResolveEnhancementTarget(
                map,
                null,
                legacyFullName,
                legacyDisplayName,
                currentPowerUid,
                resolvedSetUids,
                out currentTarget,
                out matchedEntry,
                out error);
        }

        public static bool TryResolveEnhancementTarget(
            LegacyHomecomingMap map,
            int? staticIndex,
            string? legacyFullName,
            string? legacyDisplayName,
            string currentPowerUid,
            IReadOnlyCollection<string> resolvedSetUids,
            out string currentTarget,
            out LegacyMapEntry? matchedEntry,
            out string error)
        {
            currentTarget = string.Empty;
            matchedEntry = null;
            error = string.Empty;

            if (TryCanonicalizeCurrentEnhancement(legacyFullName, out currentTarget))
            {
                return true;
            }

            var candidates = new List<LegacyMapEntry>();
            if (!string.IsNullOrWhiteSpace(legacyFullName))
            {
                candidates.AddRange(map.FindEnhancementEntries(legacyFullName, ExtractLeafName(legacyFullName)));
            }

            if (candidates.Count == 0 && !string.IsNullOrWhiteSpace(legacyDisplayName))
            {
                candidates.AddRange(map.FindEnhancementEntries(legacyDisplayName, legacyDisplayName));
            }

            if (candidates.Count == 0 && staticIndex.HasValue)
            {
                candidates.AddRange(map.GetEnhancementEntries(staticIndex.Value));
            }

            if (candidates.Count == 0 &&
                !string.IsNullOrWhiteSpace(legacyDisplayName) &&
                TryResolveEnhancementDisplayName(map, legacyDisplayName, currentPowerUid, resolvedSetUids, out currentTarget))
            {
                return true;
            }

            if (candidates.Count == 0)
            {
                error = staticIndex.HasValue
                    ? $"No Homecoming legacy enhancement mapping exists for static index {staticIndex.Value}."
                    : $"No Homecoming legacy enhancement mapping exists for '{DescribeLegacyEnhancementReference(legacyFullName, legacyDisplayName, staticIndex)}'.";
                return false;
            }

            var filteredCandidates = CanonicalizeEnhancementCandidates(candidates);
            if (filteredCandidates.Count == 0)
            {
                error = staticIndex.HasValue
                    ? $"No valid current Homecoming enhancement target exists for static index {staticIndex.Value}."
                    : $"No valid current Homecoming enhancement target exists for '{DescribeLegacyEnhancementReference(legacyFullName, legacyDisplayName, staticIndex)}'.";
                return false;
            }

            filteredCandidates = FilterEnhancementCandidates(filteredCandidates, currentPowerUid, resolvedSetUids);

            if (filteredCandidates.Count == 1)
            {
                matchedEntry = filteredCandidates[0];
                currentTarget = matchedEntry.CurrentTarget;
                return true;
            }

            error = $"Legacy enhancement reference '{DescribeLegacyEnhancementReference(legacyFullName, legacyDisplayName, staticIndex)}' is ambiguous. Candidates: {DescribeCandidates(filteredCandidates)}";
            return false;
        }

        public static bool TryResolveCurrentEnhancementSet(
            LegacyHomecomingMap map,
            string? legacySetValue,
            out string currentSetUid)
        {
            currentSetUid = string.Empty;
            if (string.IsNullOrWhiteSpace(legacySetValue))
            {
                return false;
            }

            var mappedEntries = DistinctCanonicalSetTargets(map.FindEnhancementSetEntries(legacySetValue, legacySetValue));
            if (mappedEntries.Length == 1)
            {
                currentSetUid = mappedEntries[0];
                return true;
            }

            return TryCanonicalizeCurrentEnhancementSet(legacySetValue, out currentSetUid);
        }

        public static bool TryCanonicalizeCurrentPower(string? powerUid, out string canonicalPowerUid)
        {
            canonicalPowerUid = string.Empty;
            if (string.IsNullOrWhiteSpace(powerUid))
            {
                return false;
            }

            var powerId = DatabaseAPI.PiDFromUidPower(powerUid);
            if (powerId < 0)
            {
                return false;
            }

            canonicalPowerUid = DatabaseAPI.Database.Power[powerId].FullName;
            return !string.IsNullOrWhiteSpace(canonicalPowerUid);
        }

        public static bool TryCanonicalizeCurrentEnhancement(string? enhancementUid, out string canonicalEnhancementUid)
        {
            canonicalEnhancementUid = string.Empty;
            if (string.IsNullOrWhiteSpace(enhancementUid))
            {
                return false;
            }

            var directUidMatches = DatabaseAPI.Database.Enhancements
                .Where(enhancement => enhancement != null &&
                                      string.Equals(enhancement.UID, enhancementUid, StringComparison.OrdinalIgnoreCase))
                .Select(enhancement => enhancement!.UID)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .ToArray();
            if (directUidMatches.Length == 1)
            {
                canonicalEnhancementUid = directUidMatches[0];
                return true;
            }

            var directNameMatches = DatabaseAPI.Database.Enhancements
                .Where(enhancement => enhancement != null &&
                                      (string.Equals(enhancement.LongName, enhancementUid, StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(enhancement.Name, enhancementUid, StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(enhancement.ShortName, enhancementUid, StringComparison.OrdinalIgnoreCase)))
                .Select(enhancement => enhancement!.UID)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .ToArray();
            if (directNameMatches.Length == 1)
            {
                canonicalEnhancementUid = directNameMatches[0];
                return true;
            }

            var displayUid = DatabaseAPI.GetEnhancementUid(enhancementUid);
            if (!string.IsNullOrWhiteSpace(displayUid))
            {
                canonicalEnhancementUid = displayUid;
                return true;
            }

            var enhancementId = DatabaseAPI.GetEnhancementByUIDName(enhancementUid);
            if (enhancementId < 0)
            {
                var normalized = NormalizeDisplayKey(enhancementUid);
                var normalizedMatches = DatabaseAPI.Database.Enhancements
                    .Where(enhancement => enhancement != null &&
                                          (NormalizeDisplayKey(enhancement.UID) == normalized ||
                                           NormalizeDisplayKey(enhancement.LongName) == normalized ||
                                           NormalizeDisplayKey(enhancement.Name) == normalized ||
                                           NormalizeDisplayKey(enhancement.ShortName) == normalized))
                    .Select(enhancement => enhancement!.UID)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(2)
                    .ToArray();
                if (normalizedMatches.Length != 1)
                {
                    return false;
                }

                canonicalEnhancementUid = normalizedMatches[0];
                return true;
            }

            canonicalEnhancementUid = DatabaseAPI.Database.Enhancements[enhancementId].UID;
            return !string.IsNullOrWhiteSpace(canonicalEnhancementUid);
        }

        public static bool TryCanonicalizeCurrentEnhancementSet(string? setUid, out string canonicalSetUid)
        {
            canonicalSetUid = string.Empty;
            if (string.IsNullOrWhiteSpace(setUid))
            {
                return false;
            }

            var directUidMatches = DatabaseAPI.Database.EnhancementSets
                .Where(set => set != null &&
                              string.Equals(set.Uid, setUid, StringComparison.OrdinalIgnoreCase))
                .Select(set => set!.Uid)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .ToArray();
            if (directUidMatches.Length == 1)
            {
                canonicalSetUid = directUidMatches[0];
                return true;
            }

            var directNameMatches = DatabaseAPI.Database.EnhancementSets
                .Where(set => set != null &&
                              (string.Equals(set.DisplayName, setUid, StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(set.ShortName, setUid, StringComparison.OrdinalIgnoreCase)))
                .Select(set => set!.Uid)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .ToArray();
            if (directNameMatches.Length == 1)
            {
                canonicalSetUid = directNameMatches[0];
                return true;
            }

            var normalized = NormalizeEnhancementSetKey(setUid);
            var normalizedMatches = DatabaseAPI.Database.EnhancementSets
                .Where(set => set != null &&
                              (NormalizeEnhancementSetKey(set.Uid) == normalized ||
                               NormalizeEnhancementSetKey(set.DisplayName) == normalized ||
                               NormalizeEnhancementSetKey(set.ShortName) == normalized))
                .Select(set => set!.Uid)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .ToArray();
            if (normalizedMatches.Length != 1)
            {
                return false;
            }

            canonicalSetUid = normalizedMatches[0];
            return true;
        }

        private static List<LegacyMapEntry> FilterPowerCandidates(
            List<LegacyMapEntry> candidates,
            IReadOnlyCollection<string> rawPowerSets,
            IReadOnlyCollection<string> mappedCurrentPowerSets,
            string classUid,
            string? parentCurrentPowerUid)
        {
            if (candidates.Count <= 1)
            {
                return candidates;
            }

            if (!string.IsNullOrWhiteSpace(parentCurrentPowerUid))
            {
                var subPowerMatches = candidates
                    .Where(candidate => SamePowerFamily(candidate.CurrentTarget, parentCurrentPowerUid))
                    .ToList();
                if (subPowerMatches.Count == 1)
                {
                    return subPowerMatches;
                }

                if (subPowerMatches.Count > 0)
                {
                    candidates = subPowerMatches;
                }
            }

            var exactPowersetMatches = candidates
                .Where(candidate => CandidateMatchesExactPowersetContext(candidate.CurrentTarget, rawPowerSets, mappedCurrentPowerSets))
                .ToList();
            if (exactPowersetMatches.Count == 1)
            {
                return exactPowersetMatches;
            }

            if (exactPowersetMatches.Count > 0)
            {
                candidates = exactPowersetMatches;
            }

            var contextMatches = candidates
                .Where(candidate => CandidateMatchesBuildContext(candidate.CurrentTarget, rawPowerSets, mappedCurrentPowerSets, classUid))
                .ToList();
            if (contextMatches.Count == 1)
            {
                return contextMatches;
            }

            return contextMatches.Count > 0
                ? contextMatches
                : candidates;
        }

        private static bool CandidateMatchesBuildContext(
            string currentTarget,
            IReadOnlyCollection<string> rawPowerSets,
            IReadOnlyCollection<string> mappedCurrentPowerSets,
            string classUid)
        {
            var powerId = DatabaseAPI.PiDFromUidPower(currentTarget);
            if (powerId < 0)
            {
                return false;
            }

            var power = DatabaseAPI.Database.Power[powerId];
            var powerset = power.GetPowerSet();
            if (powerset == null)
            {
                return false;
            }

            if (power.GroupName is "Inherent" or "Temporary_Powers" or "Incarnate" or "Pets" or "Redirects" or "Set_Bonus")
            {
                return true;
            }

            if (powerset.SetType is Enums.ePowerSetType.Pool or Enums.ePowerSetType.Ancillary)
            {
                return MatchesPowerset(powerset.FullName, rawPowerSets, mappedCurrentPowerSets);
            }

            if (powerset.nArchetype > -1)
            {
                var classIndex = DatabaseAPI.NidFromUidClass(classUid);
                if (classIndex > -1 && powerset.nArchetype == classIndex)
                {
                    return MatchesPowerset(powerset.FullName, rawPowerSets, mappedCurrentPowerSets) || powerset.SetType is Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary;
                }
            }

            return MatchesPowerset(powerset.FullName, rawPowerSets, mappedCurrentPowerSets);
        }

        private static bool CandidateMatchesExactPowersetContext(
            string currentTarget,
            IReadOnlyCollection<string> rawPowerSets,
            IReadOnlyCollection<string> mappedCurrentPowerSets)
        {
            var powerId = DatabaseAPI.PiDFromUidPower(currentTarget);
            if (powerId < 0)
            {
                return false;
            }

            var powersetFullName = DatabaseAPI.Database.Power[powerId].GetPowerSet()?.FullName;
            if (string.IsNullOrWhiteSpace(powersetFullName))
            {
                return false;
            }

            return rawPowerSets.Any(raw => string.Equals(raw, powersetFullName, StringComparison.OrdinalIgnoreCase)) ||
                   mappedCurrentPowerSets.Any(mapped => string.Equals(mapped, powersetFullName, StringComparison.OrdinalIgnoreCase));
        }

        private static bool MatchesPowerset(
            string currentPowerset,
            IReadOnlyCollection<string> rawPowerSets,
            IReadOnlyCollection<string> mappedCurrentPowerSets)
        {
            if (rawPowerSets.Any(raw => PowerSetKeysMatch(currentPowerset, raw)))
            {
                return true;
            }

            return mappedCurrentPowerSets.Any(mapped => PowerSetKeysMatch(currentPowerset, mapped));
        }

        private static List<LegacyMapEntry> FilterEnhancementCandidates(
            List<LegacyMapEntry> candidates,
            string currentPowerUid,
            IReadOnlyCollection<string> resolvedSetUids)
        {
            if (candidates.Count <= 1)
            {
                return candidates;
            }

            var powerId = DatabaseAPI.PiDFromUidPower(currentPowerUid);
            if (powerId > -1)
            {
                var currentPower = DatabaseAPI.Database.Power[powerId];
                var validCandidates = candidates
                    .Where(candidate =>
                    {
                        var enhancementId = DatabaseAPI.NidFromUidEnh(candidate.CurrentTarget);
                        return enhancementId >= 0 &&
                               DatabaseAPI.ValidateEnhancementForPower(currentPower, enhancementId).IsValid;
                    })
                    .ToList();

                if (validCandidates.Count == 1)
                {
                    return validCandidates;
                }

                if (validCandidates.Count > 0)
                {
                    candidates = validCandidates;
                }
            }

            if (resolvedSetUids.Count > 0)
            {
                var setMatches = candidates
                    .Where(candidate =>
                    {
                        var enhancementId = DatabaseAPI.NidFromUidEnh(candidate.CurrentTarget);
                        if (enhancementId < 0)
                        {
                            return false;
                        }

                        var setUid = DatabaseAPI.Database.Enhancements[enhancementId].UIDSet;
                        return !string.IsNullOrWhiteSpace(setUid) &&
                               resolvedSetUids.Contains(setUid, StringComparer.OrdinalIgnoreCase);
                    })
                    .ToList();

                if (setMatches.Count == 1)
                {
                    return setMatches;
                }

                if (setMatches.Count > 0)
                {
                    candidates = setMatches;
                }
            }

            return candidates;
        }

        private static bool TryResolveEnhancementDisplayName(
            LegacyHomecomingMap map,
            string legacyDisplayName,
            string currentPowerUid,
            IReadOnlyCollection<string> resolvedSetUids,
            out string currentTarget)
        {
            currentTarget = string.Empty;

            var currentUid = DatabaseAPI.GetEnhancementUid(legacyDisplayName);
            if (TryCanonicalizeCurrentEnhancement(currentUid, out currentTarget))
            {
                return true;
            }

            if (TryCanonicalizeCurrentEnhancement(legacyDisplayName, out currentTarget))
            {
                return true;
            }

            var normalizedDisplayName = NormalizeDisplayKey(legacyDisplayName);
            var directMatches = DatabaseAPI.Database.Enhancements
                .Where(enhancement => enhancement != null &&
                                      (NormalizeDisplayKey(enhancement.LongName) == normalizedDisplayName ||
                                       NormalizeDisplayKey(enhancement.Name) == normalizedDisplayName ||
                                       NormalizeDisplayKey(enhancement.ShortName) == normalizedDisplayName ||
                                       NormalizeDisplayKey(enhancement.UID) == normalizedDisplayName))
                .Select(enhancement => enhancement!.UID)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .ToArray();
            if (directMatches.Length == 1)
            {
                currentTarget = directMatches[0];
                return true;
            }

            var mappedRows = DistinctCanonicalEnhancementTargets(map.FindEnhancementEntries(legacyDisplayName, legacyDisplayName));
            if (mappedRows.Length == 1)
            {
                currentTarget = mappedRows[0];
                return true;
            }

            if (!TrySplitEnhancementDisplayName(legacyDisplayName, out var legacySetName, out var legacyPieceName))
            {
                return false;
            }

            var candidateSetUids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (TryResolveCurrentEnhancementSet(map, legacySetName, out var mappedSetUid))
            {
                candidateSetUids.Add(mappedSetUid);
            }

            foreach (var enhancementSet in DatabaseAPI.Database.EnhancementSets.Where(set => set != null))
            {
                if (NormalizeDisplayKey(enhancementSet!.DisplayName) == NormalizeDisplayKey(legacySetName) ||
                    NormalizeDisplayKey(enhancementSet.ShortName) == NormalizeDisplayKey(legacySetName) ||
                    NormalizeDisplayKey(enhancementSet.Uid) == NormalizeDisplayKey(legacySetName))
                {
                    candidateSetUids.Add(enhancementSet.Uid);
                }
            }

            var mappedPieceCandidates = CanonicalizeEnhancementCandidates(
                map.FindEnhancementEntries(legacyPieceName, legacyPieceName));
            if (mappedPieceCandidates.Count > 0)
            {
                mappedPieceCandidates = FilterEnhancementCandidatesBySetUids(mappedPieceCandidates, candidateSetUids);
                mappedPieceCandidates = FilterEnhancementCandidates(mappedPieceCandidates, currentPowerUid, resolvedSetUids);
                if (mappedPieceCandidates.Count == 1)
                {
                    currentTarget = mappedPieceCandidates[0].CurrentTarget;
                    return true;
                }
            }

            var pieceMatches = DatabaseAPI.Database.Enhancements
                .Where(enhancement =>
                    enhancement != null &&
                    !string.IsNullOrWhiteSpace(enhancement.UIDSet) &&
                    candidateSetUids.Contains(enhancement.UIDSet) &&
                    (NormalizeEnhancementPieceName(enhancement.LongName) == NormalizeDisplayKey(legacyPieceName) ||
                     NormalizeEnhancementPieceName(enhancement.Name) == NormalizeDisplayKey(legacyPieceName) ||
                     NormalizeDisplayKey(enhancement.ShortName) == NormalizeDisplayKey(legacyPieceName)))
                .Select(enhancement => enhancement!.UID)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .ToArray();

            if (pieceMatches.Length == 1)
            {
                currentTarget = pieceMatches[0];
                return true;
            }

            return false;
        }

        private static List<LegacyMapEntry> FilterEnhancementCandidatesBySetUids(
            List<LegacyMapEntry> candidates,
            IReadOnlyCollection<string> candidateSetUids)
        {
            if (candidates.Count <= 1 || candidateSetUids.Count == 0)
            {
                return candidates;
            }

            var filtered = candidates
                .Where(candidate =>
                {
                    var enhancementId = DatabaseAPI.NidFromUidEnh(candidate.CurrentTarget);
                    if (enhancementId < 0)
                    {
                        return false;
                    }

                    var setUid = DatabaseAPI.Database.Enhancements[enhancementId].UIDSet;
                    return !string.IsNullOrWhiteSpace(setUid) &&
                           candidateSetUids.Contains(setUid, StringComparer.OrdinalIgnoreCase);
                })
                .ToList();

            return filtered.Count > 0 ? filtered : candidates;
        }

        private static string DescribeCandidates(IEnumerable<LegacyMapEntry> candidates)
        {
            return string.Join("; ",
                candidates.Select(candidate =>
                    $"[{candidate.Kind} line {candidate.SourceLine}] {candidate.LegacyFullName} -> {candidate.CurrentTarget}"));
        }

        private static string DescribeLegacyEnhancementReference(
            string? legacyFullName,
            string? legacyDisplayName,
            int? staticIndex)
        {
            if (!string.IsNullOrWhiteSpace(legacyFullName))
            {
                return legacyFullName;
            }

            if (!string.IsNullOrWhiteSpace(legacyDisplayName))
            {
                return legacyDisplayName;
            }

            return staticIndex.HasValue ? $"static index {staticIndex.Value}" : string.Empty;
        }

        private static List<LegacyMapEntry> CanonicalizePowerCandidates(IEnumerable<LegacyMapEntry> candidates)
        {
            return candidates
                .Select(candidate =>
                    TryCanonicalizeCurrentPower(candidate.CurrentTarget, out var currentPowerUid)
                        ? candidate with { CurrentTarget = currentPowerUid }
                        : null)
                .Where(candidate => candidate != null)
                .GroupBy(candidate => candidate!.CurrentTarget, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(candidate => candidate!.CurrentTarget, StringComparer.OrdinalIgnoreCase)
                .Cast<LegacyMapEntry>()
                .ToList();
        }

        private static List<LegacyMapEntry> CanonicalizeEnhancementCandidates(IEnumerable<LegacyMapEntry> candidates)
        {
            return candidates
                .Select(candidate =>
                    TryCanonicalizeCurrentEnhancement(candidate.CurrentTarget, out var currentEnhancementUid)
                        ? candidate with { CurrentTarget = currentEnhancementUid }
                        : null)
                .Where(candidate => candidate != null)
                .GroupBy(candidate => candidate!.CurrentTarget, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(candidate => candidate!.CurrentTarget, StringComparer.OrdinalIgnoreCase)
                .Cast<LegacyMapEntry>()
                .ToList();
        }

        private static string[] DistinctCanonicalEnhancementTargets(IEnumerable<LegacyMapEntry> candidates)
        {
            return candidates
                .Select(candidate => TryCanonicalizeCurrentEnhancement(candidate.CurrentTarget, out var currentEnhancementUid)
                    ? currentEnhancementUid
                    : string.Empty)
                .Where(target => !string.IsNullOrWhiteSpace(target))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string[] DistinctCanonicalSetTargets(IEnumerable<LegacyMapEntry> candidates)
        {
            return candidates
                .Select(candidate => TryCanonicalizeCurrentEnhancementSet(candidate.CurrentTarget, out var currentSetUid)
                    ? currentSetUid
                    : string.Empty)
                .Where(target => !string.IsNullOrWhiteSpace(target))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static bool SamePowerFamily(string currentTarget, string parentCurrentPowerUid)
        {
            var currentPrefix = ExtractPowerSetPrefix(currentTarget);
            var parentPrefix = ExtractPowerSetPrefix(parentCurrentPowerUid);
            return !string.IsNullOrWhiteSpace(currentPrefix) &&
                   string.Equals(currentPrefix, parentPrefix, StringComparison.OrdinalIgnoreCase);
        }

        private static string ExtractPowerSetPrefix(string? powerFullName)
        {
            if (string.IsNullOrWhiteSpace(powerFullName))
            {
                return string.Empty;
            }

            var lastDot = powerFullName.LastIndexOf('.');
            return lastDot > 0 ? powerFullName[..lastDot] : string.Empty;
        }

        private static bool PowerSetKeysMatch(string left, string right)
        {
            if (string.Equals(left, right, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return string.Equals(ExtractLeafName(left), ExtractLeafName(right), StringComparison.OrdinalIgnoreCase);
        }

        private static string ExtractLeafName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return string.Empty;
            }

            var trimmed = fullName.Trim();
            var lastDot = trimmed.LastIndexOf('.');
            return lastDot >= 0 && lastDot < trimmed.Length - 1
                ? trimmed[(lastDot + 1)..]
                : trimmed;
        }

        private static bool TrySplitEnhancementDisplayName(string displayName, out string setName, out string pieceName)
        {
            setName = string.Empty;
            pieceName = string.Empty;
            var separatorIndex = displayName.IndexOf(':');
            if (separatorIndex < 0)
            {
                return false;
            }

            setName = displayName[..separatorIndex].Trim();
            pieceName = displayName[(separatorIndex + 1)..].Trim();
            return !string.IsNullOrWhiteSpace(setName) && !string.IsNullOrWhiteSpace(pieceName);
        }

        private static string NormalizeDisplayKey(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return Regex.Replace(value.Trim().ToUpperInvariant(), @"[^A-Z0-9]+", string.Empty);
        }

        private static string NormalizeEnhancementPieceName(string? value)
        {
            if (!TrySplitEnhancementDisplayName(value ?? string.Empty, out _, out var pieceName))
            {
                return NormalizeDisplayKey(value);
            }

            return NormalizeDisplayKey(pieceName);
        }

        private static string NormalizeEnhancementSetKey(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var normalized = value.Trim();
            normalized = Regex.Replace(normalized, @"^(SUPERIOR_ATTUNED_|ATTUNED_|CRAFTED_|SUPERIOR_)", string.Empty, RegexOptions.IgnoreCase);
            return NormalizeDisplayKey(normalized);
        }
    }
}

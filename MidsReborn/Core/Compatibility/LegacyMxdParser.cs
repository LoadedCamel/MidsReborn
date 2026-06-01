using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core.Compatibility
{
    internal enum LegacySaveBinaryFormat
    {
        Current = 0,
        Prior = 1,
        Legacy = 2
    }

    internal sealed class LegacyMxdBuild
    {
        public string LegacyTag { get; init; } = string.Empty;
        public float SaveFormatVersion { get; init; }
        public LegacySaveBinaryFormat SaveFormat { get; init; }
        public bool QualifiedNames { get; init; }
        public bool HasSubPowers { get; init; }
        public string ClassUid { get; init; } = string.Empty;
        public string OriginUid { get; init; } = string.Empty;
        public Enums.Alignment Alignment { get; init; }
        public string CharacterName { get; init; } = string.Empty;
        public List<string> PowerSets { get; init; } = [];
        public int LastPower { get; init; }
        public List<LegacyMxdPowerEntry> PowerEntries { get; init; } = [];
    }

    internal sealed class LegacyMxdPowerEntry
    {
        public int PowerIndex { get; init; }
        public int? SavedStaticIndex { get; init; }
        public string SavedUid { get; init; } = string.Empty;
        public int Level { get; init; }
        public bool StatInclude { get; init; }
        public bool ProcInclude { get; init; }
        public int VariableValue { get; init; }
        public int InherentSlotsUsed { get; init; }
        public List<LegacyMxdSubPowerEntry> SubPowers { get; init; } = [];
        public List<LegacyMxdSlotEntry> Slots { get; init; } = [];
    }

    internal sealed class LegacyMxdSubPowerEntry
    {
        public int SubPowerIndex { get; init; }
        public int? SavedStaticIndex { get; init; }
        public string SavedUid { get; init; } = string.Empty;
        public bool StatInclude { get; init; }
    }

    internal sealed class LegacyMxdSlotEntry
    {
        public int SlotIndex { get; init; }
        public int Level { get; init; }
        public bool IsGranted { get; init; }
        public LegacyMxdEnhancementRef? Enhancement { get; init; }
        public LegacyMxdEnhancementRef? FlippedEnhancement { get; init; }
    }

    internal sealed class LegacyMxdEnhancementRef
    {
        public int? SavedStaticIndex { get; init; }
        public string SavedUid { get; init; } = string.Empty;
        public int? IoLevel { get; init; }
        public int? RelativeLevelRaw { get; init; }
        public int? GradeRaw { get; init; }
    }

    internal static class LegacyMxdParser
    {
        private const string MagicCompressed = "MxDz";
        private const string MagicUncompressed = "MxDu";
        private const string ModernCompressed = "MRBz";
        private const string LegacyCompressed = AppDataPaths.Headers.Save.LegacyCompressed;
        private const string LegacyUncompressed = AppDataPaths.Headers.Save.LegacyUncompressed;
        private const float PriorSaveVersion = 3.10f;
        private const float CurrentSaveVersion = 3.20f;
        private const float LegacyInternalFormatChange1 = 1.29999995231628f;
        private const float LegacyInternalFormatChange2 = 1.39999997615814f;

        private static readonly byte[] MagicNumber =
        {
            Convert.ToByte('M'),
            Convert.ToByte('x'),
            Convert.ToByte('D'),
            Convert.ToByte(12)
        };

        public static bool TryParse(string text, LegacyHomecomingMap compatibilityMap, out LegacyMxdBuild? build, out string error)
        {
            build = null;
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(text))
            {
                error = "The legacy build file was empty.";
                return false;
            }

            if (!TryParseLegacyTag(text, out var legacyTag))
            {
                error = "The legacy build version could not be determined from the file preamble.";
                return false;
            }

            byte[] payloadBytes;
            try
            {
                payloadBytes = ExtractPayloadBytes(text);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            if (TryGetLegacyInternalPayloadText(payloadBytes, out var legacyPayloadText))
            {
                try
                {
                    build = ParseLegacyInternalPayloadText(text, legacyPayloadText, legacyTag);
                    return true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    return false;
                }
            }

            try
            {
                build = ParsePayloadBytes(payloadBytes, legacyTag, compatibilityMap);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static bool TryParseLegacyTag(string text, out string tag)
        {
            tag = string.Empty;
            var match = Regex.Match(
                text,
                @"built using\s+Mids(?:'?\s+Reborn)?\s+(?<version>v?\d+(?:\.\d+)+)",
                RegexOptions.IgnoreCase);

            if (match.Success)
            {
                tag = match.Groups["version"].Value;
                return !string.IsNullOrWhiteSpace(tag);
            }

            match = Regex.Match(
                text,
                @"(?:Hero|Villain|Rogue|Vigilante)\s+Plan\s+by\s+(?:Mids|Pine)(?:'?\s+(?:Hero|Villain))?\s+Designer(?:\s+(?<version>v?\d+(?:\.\d+)+))?",
                RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return false;
            }

            tag = match.Groups["version"].Success && !string.IsNullOrWhiteSpace(match.Groups["version"].Value)
                ? match.Groups["version"].Value
                : "Original Mids";
            return true;
        }

        private sealed record LegacyPlannerPowerListing(
            int Level,
            string DisplayName,
            bool IsInherent,
            bool IsEmpty);

        private static bool TryGetLegacyInternalPayloadText(byte[] payloadBytes, out string payloadText)
        {
            payloadText = string.Empty;
            if (payloadBytes == null || payloadBytes.Length == 0)
            {
                return false;
            }

            payloadText = Encoding.UTF8.GetString(payloadBytes)
                .TrimStart('\uFEFF', '\0', ' ', '\t', '\r', '\n');
            return payloadText.StartsWith(LegacyUncompressed, StringComparison.OrdinalIgnoreCase);
        }

        private static LegacyMxdBuild ParseLegacyInternalPayloadText(string sourceText, string payloadText, string legacyTag)
        {
            var normalizedPayload = payloadText
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace('\r', '\n');
            using var reader = new StringReader(normalizedPayload);

            var versionLine = ReadNextNonEmptyLine(reader);
            if (string.IsNullOrWhiteSpace(versionLine))
            {
                throw new InvalidDataException("The legacy MHD payload did not contain a HeroDataVersion header.");
            }

            var versionParts = versionLine.Split(';');
            if (versionParts.Length < 2 ||
                !string.Equals(versionParts[0], LegacyUncompressed, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("The legacy MHD payload did not start with a valid HeroDataVersion header.");
            }

            if (!float.TryParse(
                    versionParts[1].Trim().Replace(",", ".", StringComparison.Ordinal),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var saveFormatVersion))
            {
                throw new InvalidDataException($"The legacy HeroDataVersion '{versionParts[1]}' could not be parsed.");
            }

            var characterLine = ReadNextNonEmptyLine(reader);
            var characterParts = SplitDelimited(characterLine);
            if (characterParts.Length < 3)
            {
                throw new InvalidDataException("The legacy MHD payload did not contain the expected character identity line.");
            }

            var powerSetLine = ReadNextNonEmptyLine(reader);
            var rawPowerSetNames = SplitDelimited(powerSetLine)
                .Select(part => part.Trim())
                .ToList();
            if (rawPowerSetNames.Count == 0)
            {
                throw new InvalidDataException("The legacy MHD payload did not contain any powerset declarations.");
            }

            var powerDataLine = string.Concat(
                reader.ReadToEnd()
                    .Replace("\r\n", "\n", StringComparison.Ordinal)
                    .Replace('\r', '\n')
                    .Split('\n')
                    .Where(line => !string.IsNullOrWhiteSpace(line)));
            if (string.IsNullOrWhiteSpace(powerDataLine))
            {
                throw new InvalidDataException("The legacy MHD payload did not contain any packed power data.");
            }

            var archetype = DatabaseAPI.GetArchetypeByName(characterParts[2].Trim());
            if (archetype == null)
            {
                throw new InvalidDataException($"The archetype '{characterParts[2]}' from the legacy MHD payload could not be resolved.");
            }

            var resolvedOriginIndex = DatabaseAPI.GetOriginByName(archetype, characterParts[1].Trim());
            if (resolvedOriginIndex < 0 || resolvedOriginIndex >= archetype.Origin.Length)
            {
                resolvedOriginIndex = 0;
            }

            var resolvedPowerSets = ResolveLegacyInternalPowerSets(rawPowerSetNames, archetype.DisplayName);
            var plannerListings = ExtractPlannerPowerListings(sourceText);
            var powerTokens = powerDataLine.Split('|');
            var powerEntries = ParseLegacyInternalPowerEntries(
                powerTokens,
                saveFormatVersion,
                resolvedPowerSets,
                rawPowerSetNames,
                plannerListings,
                archetype.DisplayName);

            var highestMainLevel = plannerListings
                .Where(listing => !listing.IsInherent)
                .Select(listing => listing.Level)
                .DefaultIfEmpty(powerEntries
                    .Select(entry => entry.Level + 1)
                    .DefaultIfEmpty(1)
                    .Max())
                .Max();
            var importedLevel = Math.Clamp(highestMainLevel - 1, 0, Character.MaxLevel);
            var progressionPolicy = DatabaseAPI.GetBuildProgressionPolicy(MidsContext.Config?.DataPath);
            var lastPower = Math.Max(0, progressionPolicy.GetNormalPowerPickCountAtLevel(importedLevel) - 1);

            return new LegacyMxdBuild
            {
                LegacyTag = legacyTag,
                SaveFormatVersion = saveFormatVersion,
                SaveFormat = saveFormatVersion < LegacyInternalFormatChange1
                    ? LegacySaveBinaryFormat.Legacy
                    : saveFormatVersion < LegacyInternalFormatChange2
                        ? LegacySaveBinaryFormat.Prior
                        : LegacySaveBinaryFormat.Current,
                QualifiedNames = true,
                HasSubPowers = saveFormatVersion >= LegacyInternalFormatChange2,
                ClassUid = archetype.ClassName,
                OriginUid = archetype.Origin[resolvedOriginIndex],
                Alignment = archetype.Hero ? Enums.Alignment.Hero : Enums.Alignment.Villain,
                CharacterName = characterParts[0].Trim(),
                PowerSets = resolvedPowerSets,
                LastPower = lastPower,
                PowerEntries = powerEntries
            };
        }

        private static List<LegacyMxdPowerEntry> ParseLegacyInternalPowerEntries(
            IReadOnlyList<string> powerTokens,
            float saveFormatVersion,
            IReadOnlyList<string> resolvedPowerSets,
            IReadOnlyList<string> rawPowerSetNames,
            IReadOnlyList<LegacyPlannerPowerListing> plannerListings,
            string archetypeName)
        {
            var hasSubPowers = saveFormatVersion >= LegacyInternalFormatChange2;
            var entries = new List<LegacyMxdPowerEntry>();
            var offset = 0;
            var powerIndex = 0;
            while (offset < powerTokens.Count)
            {
                if (RemainingTokensAreBlank(powerTokens, offset))
                {
                    break;
                }

                RequireRemainingTokenCount(powerTokens, offset, 6, "power identity");
                var statInclude = ParseLegacyInt(powerTokens[offset]) != 0;
                var isInherentPower = ParseLegacyInt(powerTokens[offset + 1]) != 0;
                var level = ParseLegacyInt(powerTokens[offset + 2]);
                var powerSetIndex = ParseLegacyInt(powerTokens[offset + 3]);
                var localPowerIndex = ParseLegacyInt(powerTokens[offset + 4]);
                var slotCount = Math.Max(ParseLegacyInt(powerTokens[offset + 5]) + 1, 0);
                offset += 6;

                if (isInherentPower && powerSetIndex < 0 && resolvedPowerSets.Count > 2)
                {
                    powerSetIndex = 2;
                }

                var slots = new List<LegacyMxdSlotEntry>(slotCount);
                for (var slotIndex = 0; slotIndex < slotCount; slotIndex++)
                {
                    RequireRemainingTokenCount(powerTokens, offset, 2, $"slot {slotIndex + 1}");
                    var slotToken = powerTokens[offset];
                    var slotLevel = ParseLegacyInt(powerTokens[offset + 1]);
                    offset += 2;

                    ParseLegacyInternalSlot(slotToken, out var enhancement, out var flippedEnhancement);
                    slots.Add(new LegacyMxdSlotEntry
                    {
                        SlotIndex = slotIndex,
                        Level = slotLevel,
                        IsGranted = false,
                        Enhancement = enhancement,
                        FlippedEnhancement = flippedEnhancement
                    });
                }

                RequireRemainingTokenCount(powerTokens, offset, 1, "variable value");
                var variableValue = ParseLegacyInt(powerTokens[offset]);
                offset++;

                var subPowers = new List<LegacyMxdSubPowerEntry>();
                if (hasSubPowers)
                {
                    RequireRemainingTokenCount(powerTokens, offset, 1, "subpower count");
                    var subPowerCount = Math.Max(ParseLegacyInt(powerTokens[offset]) + 1, 0);
                    offset++;
                    subPowers = new List<LegacyMxdSubPowerEntry>(subPowerCount);
                    for (var subPowerIndex = 0; subPowerIndex < subPowerCount; subPowerIndex++)
                    {
                        RequireRemainingTokenCount(powerTokens, offset, 3, $"subpower {subPowerIndex + 1}");
                        var localSubPowerIndex = ParseLegacyInt(powerTokens[offset]);
                        var subPowerSetIndex = ParseLegacyInt(powerTokens[offset + 1]);
                        var subStatInclude = ParseLegacyInt(powerTokens[offset + 2]) != 0;
                        offset += 3;

                        ResolveLegacyInternalPowerIdentity(
                            entryOrdinal: -1,
                            powerSetIndex: subPowerSetIndex,
                            localPowerIndex: localSubPowerIndex,
                            resolvedPowerSets,
                            rawPowerSetNames,
                            plannerListings,
                            archetypeName,
                            out var subPowerUid,
                            out var subPowerStaticIndex);

                        subPowers.Add(new LegacyMxdSubPowerEntry
                        {
                            SubPowerIndex = subPowerIndex,
                            SavedStaticIndex = subPowerStaticIndex,
                            SavedUid = subPowerUid,
                            StatInclude = subStatInclude
                        });
                    }
                }

                ResolveLegacyInternalPowerIdentity(
                    powerIndex,
                    powerSetIndex,
                    localPowerIndex,
                    resolvedPowerSets,
                    rawPowerSetNames,
                    plannerListings,
                    archetypeName,
                    out var savedUid,
                    out var savedStaticIndex);

                entries.Add(new LegacyMxdPowerEntry
                {
                    PowerIndex = powerIndex,
                    SavedStaticIndex = savedStaticIndex,
                    SavedUid = savedUid,
                    Level = level,
                    StatInclude = statInclude,
                    ProcInclude = false,
                    VariableValue = variableValue,
                    InherentSlotsUsed = 0,
                    SubPowers = subPowers,
                    Slots = slots
                });
                powerIndex++;
            }

            return entries;
        }

        private static void ResolveLegacyInternalPowerIdentity(
            int entryOrdinal,
            int powerSetIndex,
            int localPowerIndex,
            IReadOnlyList<string> resolvedPowerSets,
            IReadOnlyList<string> rawPowerSetNames,
            IReadOnlyList<LegacyPlannerPowerListing> plannerListings,
            string archetypeName,
            out string savedUid,
            out int? savedStaticIndex)
        {
            savedUid = string.Empty;
            savedStaticIndex = null;
            if (powerSetIndex < 0 || localPowerIndex < 0 || powerSetIndex >= resolvedPowerSets.Count)
            {
                return;
            }

            var resolvedPowerSet = resolvedPowerSets[powerSetIndex];
            var rawPowerSetName = powerSetIndex < rawPowerSetNames.Count
                ? rawPowerSetNames[powerSetIndex]
                : string.Empty;
            var plannerListing = entryOrdinal >= 0 && entryOrdinal < plannerListings.Count
                ? plannerListings[entryOrdinal]
                : null;
            var plannerDisplayName = plannerListing?.IsEmpty == true
                ? string.Empty
                : plannerListing?.DisplayName ?? string.Empty;

            if (string.Equals(resolvedPowerSet, "Pool.Fitness", StringComparison.OrdinalIgnoreCase) &&
                TryResolveLegacyFitnessPower(plannerDisplayName, localPowerIndex, out var fitnessUid, out var fitnessStaticIndex))
            {
                savedUid = fitnessUid;
                savedStaticIndex = fitnessStaticIndex;
                return;
            }

            if (IsLegacyInherentPowerSetReference(rawPowerSetName, powerSetIndex) &&
                TryResolveInherentPowerUid(plannerDisplayName, out var inherentPowerUid))
            {
                savedUid = inherentPowerUid;
                return;
            }

            if (TryResolvePowerUidFromPowerSet(resolvedPowerSet, plannerDisplayName, localPowerIndex, out var resolvedPowerUid))
            {
                savedUid = resolvedPowerUid;
                return;
            }

            if (!string.IsNullOrWhiteSpace(plannerDisplayName))
            {
                var guessedLeafName = NormalizeLegacyPowerLeafName(plannerDisplayName);
                if (!string.IsNullOrWhiteSpace(guessedLeafName))
                {
                    savedUid = !string.IsNullOrWhiteSpace(resolvedPowerSet)
                        ? $"{resolvedPowerSet}.{guessedLeafName}"
                        : guessedLeafName;
                }
            }

            if (string.IsNullOrWhiteSpace(savedUid) &&
                TryResolveFallbackPowerSet(rawPowerSetName, powerSetIndex, archetypeName, out var fallbackPowerSet) &&
                TryResolvePowerUidFromPowerSet(fallbackPowerSet, plannerDisplayName, localPowerIndex, out resolvedPowerUid))
            {
                savedUid = resolvedPowerUid;
            }
        }

        private static List<string> ResolveLegacyInternalPowerSets(
            IReadOnlyList<string> rawPowerSetNames,
            string archetypeName)
        {
            var resolved = new List<string>(rawPowerSetNames.Count);
            for (var index = 0; index < rawPowerSetNames.Count; index++)
            {
                var rawName = rawPowerSetNames[index]?.Trim() ?? string.Empty;
                resolved.Add(ResolveLegacyInternalPowerSet(rawName, index, archetypeName));
            }

            return resolved;
        }

        private static string ResolveLegacyInternalPowerSet(string rawName, int slotIndex, string archetypeName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return string.Empty;
            }

            if (slotIndex == 2 || rawName.Contains("Inherent", StringComparison.OrdinalIgnoreCase))
            {
                return DatabaseAPI.GetInherentPowerset()?.FullName ?? "Inherent.Inherent";
            }

            if (string.Equals(rawName, "Fitness", StringComparison.OrdinalIgnoreCase))
            {
                return "Pool.Fitness";
            }

            if (TryResolveFallbackPowerSet(rawName, slotIndex, archetypeName, out var resolvedPowerSet))
            {
                return resolvedPowerSet;
            }

            return slotIndex switch
            {
                0 => $"Primary.{NormalizeLegacyPowerLeafName(rawName)}",
                1 => $"Secondary.{NormalizeLegacyPowerLeafName(rawName)}",
                7 => $"Epic.{NormalizeLegacyPowerLeafName(rawName)}",
                _ => $"Pool.{NormalizeLegacyPowerLeafName(rawName)}"
            };
        }

        private static bool TryResolveFallbackPowerSet(string rawName, int slotIndex, string archetypeName, out string resolvedPowerSet)
        {
            resolvedPowerSet = string.Empty;
            IPowerset? powerset = slotIndex switch
            {
                0 or 1 or 7 => DatabaseAPI.GetPowersetByName(rawName, archetypeName, true),
                _ => null
            };

            powerset ??= slotIndex switch
            {
                0 => DatabaseAPI.GetPowersetByName(rawName, Enums.ePowerSetType.Primary),
                1 => DatabaseAPI.GetPowersetByName(rawName, Enums.ePowerSetType.Secondary),
                7 => FindPowerSet(rawName, archetypeName, Enums.ePowerSetType.Ancillary),
                >= 3 and <= 6 => DatabaseAPI.GetPowersetByName(rawName, Enums.ePowerSetType.Pool) ?? FindPowerSet(rawName, archetypeName, Enums.ePowerSetType.Pool),
                _ => null
            };

            if (powerset == null)
            {
                return false;
            }

            resolvedPowerSet = powerset.FullName;
            return !string.IsNullOrWhiteSpace(resolvedPowerSet);
        }

        private static IPowerset? FindPowerSet(string rawName, string archetypeName, Enums.ePowerSetType setType)
        {
            var archetypeIndex = DatabaseAPI.GetArchetypeByName(archetypeName)?.Idx ?? -1;
            return DatabaseAPI.Database.Powersets
                .Where(powerSet => powerSet != null &&
                                   powerSet.SetType == setType &&
                                   string.Equals(powerSet.DisplayName, rawName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(powerSet => setType != Enums.ePowerSetType.Ancillary ||
                                            archetypeIndex < 0 ||
                                            powerSet!.ClassOk(archetypeIndex));
        }

        private static bool TryResolvePowerUidFromPowerSet(
            string resolvedPowerSet,
            string displayName,
            int localPowerIndex,
            out string powerUid)
        {
            powerUid = string.Empty;
            var powerSet = DatabaseAPI.GetPowersetByName(resolvedPowerSet) ??
                           DatabaseAPI.GetPowersetByFullname(resolvedPowerSet);
            if (powerSet == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(displayName))
            {
                var normalizedDisplayName = NormalizeLookupKey(displayName);
                foreach (var powerId in powerSet.Power)
                {
                    if (powerId < 0 || powerId >= DatabaseAPI.Database.Power.Length)
                    {
                        continue;
                    }

                    var power = DatabaseAPI.Database.Power[powerId];
                    if (power == null)
                    {
                        continue;
                    }

                    if (string.Equals(power.DisplayName, displayName, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(power.PowerName, displayName, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(NormalizeLookupKey(power.DisplayName), normalizedDisplayName, StringComparison.Ordinal) ||
                        string.Equals(NormalizeLookupKey(power.PowerName), normalizedDisplayName, StringComparison.Ordinal) ||
                        string.Equals(NormalizeLookupKey(ExtractLeafName(power.FullName)), normalizedDisplayName, StringComparison.Ordinal))
                    {
                        powerUid = power.FullName;
                        return !string.IsNullOrWhiteSpace(powerUid);
                    }
                }
            }

            if (localPowerIndex >= 0 && localPowerIndex < powerSet.Power.Length)
            {
                var fallbackPowerId = powerSet.Power[localPowerIndex];
                if (fallbackPowerId >= 0 && fallbackPowerId < DatabaseAPI.Database.Power.Length)
                {
                    powerUid = DatabaseAPI.Database.Power[fallbackPowerId]?.FullName ?? string.Empty;
                    return !string.IsNullOrWhiteSpace(powerUid);
                }
            }

            return false;
        }

        private static bool IsLegacyInherentPowerSetReference(string rawPowerSetName, int powerSetIndex)
        {
            return powerSetIndex == 2 ||
                   rawPowerSetName.Contains("Inherent", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryResolveInherentPowerUid(string displayName, out string powerUid)
        {
            powerUid = string.Empty;
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return false;
            }

            var normalizedDisplayName = NormalizeLookupKey(displayName);
            var matches = DatabaseAPI.Database.Powersets
                .Where(powerSet => powerSet is { SetType: Enums.ePowerSetType.Inherent })
                .SelectMany(powerSet => powerSet!.Power
                    .Where(powerId => powerId >= 0 && powerId < DatabaseAPI.Database.Power.Length)
                    .Select(powerId => DatabaseAPI.Database.Power[powerId]))
                .Where(power => power != null &&
                               (string.Equals(power.DisplayName, displayName, StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(power.PowerName, displayName, StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(NormalizeLookupKey(power.DisplayName), normalizedDisplayName, StringComparison.Ordinal) ||
                                string.Equals(NormalizeLookupKey(power.PowerName), normalizedDisplayName, StringComparison.Ordinal) ||
                                string.Equals(NormalizeLookupKey(ExtractLeafName(power.FullName)), normalizedDisplayName, StringComparison.Ordinal)))
                .Select(power => power!.FullName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .ToArray();
            if (matches.Length != 1)
            {
                return false;
            }

            powerUid = matches[0];
            return true;
        }

        private static bool TryResolveLegacyFitnessPower(
            string displayName,
            int localPowerIndex,
            out string powerUid,
            out int staticIndex)
        {
            powerUid = string.Empty;
            staticIndex = -1;

            var normalizedDisplayName = NormalizeLookupKey(displayName);
            if (string.Equals(normalizedDisplayName, "QUICK", StringComparison.Ordinal) ||
                string.Equals(normalizedDisplayName, "SWIFT", StringComparison.Ordinal) ||
                localPowerIndex == 0)
            {
                powerUid = "Pool.Fitness.Quick";
                staticIndex = 1797;
                return true;
            }

            if (string.Equals(normalizedDisplayName, "HURDLE", StringComparison.Ordinal) || localPowerIndex == 1)
            {
                powerUid = "Pool.Fitness.Hurdle";
                staticIndex = 1798;
                return true;
            }

            if (string.Equals(normalizedDisplayName, "HEALTH", StringComparison.Ordinal) || localPowerIndex == 2)
            {
                powerUid = "Pool.Fitness.Health";
                staticIndex = 1799;
                return true;
            }

            if (string.Equals(normalizedDisplayName, "STAMINA", StringComparison.Ordinal) || localPowerIndex == 3)
            {
                powerUid = "Pool.Fitness.Stamina";
                staticIndex = 1800;
                return true;
            }

            return false;
        }

        private static void ParseLegacyInternalSlot(
            string slotToken,
            out LegacyMxdEnhancementRef? enhancement,
            out LegacyMxdEnhancementRef? flippedEnhancement)
        {
            enhancement = null;
            flippedEnhancement = null;
            if (string.IsNullOrWhiteSpace(slotToken))
            {
                return;
            }

            var slotParts = slotToken.Split('~');
            if (slotParts.Length > 6)
            {
                enhancement = CreateLegacyInternalEnhancementRef(slotParts, 0);
                if (slotParts.Length > 13)
                {
                    flippedEnhancement = CreateLegacyInternalEnhancementRef(slotParts, 7);
                }

                return;
            }

            if (slotParts.Length > 3)
            {
                var staticIndex = ParseLegacyInt(slotParts[0]);
                if (staticIndex >= 0)
                {
                    enhancement = new LegacyMxdEnhancementRef
                    {
                        SavedStaticIndex = staticIndex,
                        SavedUid = string.Empty,
                        RelativeLevelRaw = ParseLegacyInt(slotParts[1]),
                        GradeRaw = ParseLegacyInt(slotParts[2]),
                        IoLevel = ClampIoLevel(ParseLegacyInt(slotParts[3]))
                    };
                }
            }
        }

        private static LegacyMxdEnhancementRef? CreateLegacyInternalEnhancementRef(IReadOnlyList<string> slotParts, int offset)
        {
            if (slotParts.Count <= offset + 6)
            {
                return null;
            }

            var setName = slotParts[offset].Trim();
            var enhancementName = slotParts[offset + 1].Trim();
            var staticIndex = ParseLegacyInt(slotParts[offset + 3]);
            if (staticIndex < 0 &&
                string.IsNullOrWhiteSpace(setName) &&
                (string.IsNullOrWhiteSpace(enhancementName) ||
                 string.Equals(enhancementName, "Empty", StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            var savedUid = string.IsNullOrWhiteSpace(setName)
                ? enhancementName
                : $"{setName}.{enhancementName}";
            return new LegacyMxdEnhancementRef
            {
                SavedStaticIndex = staticIndex >= 0 ? staticIndex : null,
                SavedUid = savedUid,
                RelativeLevelRaw = ParseLegacyInt(slotParts[offset + 4]),
                GradeRaw = ParseLegacyInt(slotParts[offset + 5]),
                IoLevel = ClampIoLevel(ParseLegacyInt(slotParts[offset + 6]))
            };
        }

        private static List<LegacyPlannerPowerListing> ExtractPlannerPowerListings(string text)
        {
            var listings = new List<LegacyPlannerPowerListing>();
            var normalizedText = text
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace('\r', '\n');
            var lines = normalizedText.Split('\n');
            var section = 0;
            foreach (var line in lines)
            {
                var trimmed = line.TrimEnd();
                if (trimmed.StartsWith("|", StringComparison.Ordinal))
                {
                    break;
                }

                if (Regex.IsMatch(trimmed, @"^-{3,}$"))
                {
                    section++;
                    continue;
                }

                if (section == 0)
                {
                    continue;
                }

                var match = Regex.Match(
                    trimmed,
                    @"^Level\s+(?<level>\d+)\s*:\s*(?<power>[^\t]+?)\s*(?:\t|$)",
                    RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    continue;
                }

                var powerName = match.Groups["power"].Value.Trim();
                listings.Add(new LegacyPlannerPowerListing(
                    int.Parse(match.Groups["level"].Value, CultureInfo.InvariantCulture),
                    powerName,
                    section > 1,
                    string.Equals(powerName, "[Empty]", StringComparison.OrdinalIgnoreCase)));
            }

            return listings;
        }

        private static string ReadNextNonEmptyLine(StringReader reader)
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    return line.Trim();
                }
            }

            return string.Empty;
        }

        private static string[] SplitDelimited(string line)
        {
            return string.IsNullOrWhiteSpace(line)
                ? []
                : line.Split('|', StringSplitOptions.None);
        }

        private static int ParseLegacyInt(string token)
        {
            return int.TryParse(token?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : 0;
        }

        private static int? ClampIoLevel(int level)
        {
            if (level < 0)
            {
                return null;
            }

            return Math.Min(level, 49);
        }

        private static bool RemainingTokensAreBlank(IReadOnlyList<string> tokens, int offset)
        {
            for (var index = offset; index < tokens.Count; index++)
            {
                if (!string.IsNullOrWhiteSpace(tokens[index]))
                {
                    return false;
                }
            }

            return true;
        }

        private static void RequireRemainingTokenCount(IReadOnlyList<string> tokens, int offset, int requiredCount, string context)
        {
            if (offset + requiredCount > tokens.Count)
            {
                throw new InvalidDataException($"The legacy MHD payload ended unexpectedly while reading {context}.");
            }
        }

        private static string NormalizeLegacyPowerLeafName(string displayName)
        {
            return displayName
                .Trim()
                .Replace(" ", "_", StringComparison.Ordinal)
                .Replace("-", "_", StringComparison.Ordinal)
                .Replace("/", "_", StringComparison.Ordinal)
                .Replace("\\", "_", StringComparison.Ordinal);
        }

        private static string NormalizeLookupKey(string value)
        {
            return Regex.Replace(value ?? string.Empty, @"[^A-Z0-9]+", string.Empty, RegexOptions.IgnoreCase)
                .ToUpperInvariant();
        }

        private static LegacyMxdBuild ParsePayloadBytes(byte[] bytes, string legacyTag, LegacyHomecomingMap compatibilityMap)
        {
            using var memoryStream = new MemoryStream(bytes, writable: false);
            using var reader = new BinaryReader(memoryStream, Encoding.UTF8, leaveOpen: false);

            var magicIndex = FindMagicIndex(bytes);
            if (magicIndex < 0)
            {
                throw new InvalidDataException("The legacy payload did not contain the expected magic number.");
            }

            reader.BaseStream.Seek(magicIndex + MagicNumber.Length, SeekOrigin.Begin);
            var saveFormatVersion = reader.ReadSingle();
            var saveFormat = saveFormatVersion switch
            {
                < PriorSaveVersion => LegacySaveBinaryFormat.Legacy,
                < CurrentSaveVersion => LegacySaveBinaryFormat.Prior,
                _ => LegacySaveBinaryFormat.Current
            };

            var qualifiedNames = reader.ReadBoolean();
            var hasSubPowers = reader.ReadBoolean();
            var classUid = reader.ReadString();
            var originUid = reader.ReadString();

            var classIndex = DatabaseAPI.NidFromUidClass(classUid);
            var fallbackAlignment = classIndex >= 0 &&
                                    classIndex < DatabaseAPI.Database.Classes.Length &&
                                    DatabaseAPI.Database.Classes[classIndex]?.Hero == true
                ? Enums.Alignment.Hero
                : Enums.Alignment.Villain;

            var alignment = saveFormatVersion > 1.0f
                ? (Enums.Alignment)reader.ReadInt32()
                : fallbackAlignment;

            var characterName = reader.ReadString();
            var powerSetCount = reader.ReadInt32() + 1;
            var powerSets = new List<string>(powerSetCount);
            for (var index = 0; index < powerSetCount; index++)
            {
                powerSets.Add(reader.ReadString());
            }

            var lastPower = reader.ReadInt32();
            var powerEntryCount = reader.ReadInt32() + 1;
            var powerEntries = new List<LegacyMxdPowerEntry>(Math.Max(powerEntryCount, 0));

            for (var powerIndex = 0; powerIndex < powerEntryCount; powerIndex++)
            {
                int? savedStaticIndex = null;
                var savedUid = string.Empty;
                if (qualifiedNames)
                {
                    savedUid = reader.ReadString();
                }
                else
                {
                    var rawStaticIndex = reader.ReadInt32();
                    if (rawStaticIndex >= 0)
                    {
                        savedStaticIndex = rawStaticIndex;
                    }
                }

                var hasIdentity = savedStaticIndex.HasValue || !string.IsNullOrWhiteSpace(savedUid);
                var level = 0;
                var statInclude = false;
                var procInclude = false;
                var variableValue = 0;
                var inherentSlotsUsed = 0;
                var subPowers = new List<LegacyMxdSubPowerEntry>();

                if (hasIdentity)
                {
                    level = reader.ReadSByte();
                    switch (saveFormat)
                    {
                        case LegacySaveBinaryFormat.Current:
                            statInclude = reader.ReadBoolean();
                            procInclude = reader.ReadBoolean();
                            variableValue = reader.ReadInt32();
                            inherentSlotsUsed = reader.ReadInt32();
                            break;
                        case LegacySaveBinaryFormat.Prior:
                            statInclude = reader.ReadBoolean();
                            procInclude = reader.ReadBoolean();
                            variableValue = reader.ReadInt32();
                            break;
                        case LegacySaveBinaryFormat.Legacy:
                            statInclude = reader.ReadBoolean();
                            variableValue = reader.ReadInt32();
                            break;
                    }

                    if (hasSubPowers)
                    {
                        var subPowerCount = reader.ReadSByte() + 1;
                        subPowers = new List<LegacyMxdSubPowerEntry>(Math.Max(subPowerCount, 0));
                        for (var subPowerIndex = 0; subPowerIndex < subPowerCount; subPowerIndex++)
                        {
                            int? subPowerStaticIndex = null;
                            var subPowerUid = string.Empty;
                            if (qualifiedNames)
                            {
                                subPowerUid = reader.ReadString();
                            }
                            else
                            {
                                var rawSubPowerStaticIndex = reader.ReadInt32();
                                if (rawSubPowerStaticIndex >= 0)
                                {
                                    subPowerStaticIndex = rawSubPowerStaticIndex;
                                }
                            }

                            subPowers.Add(new LegacyMxdSubPowerEntry
                            {
                                SubPowerIndex = subPowerIndex,
                                SavedStaticIndex = subPowerStaticIndex,
                                SavedUid = subPowerUid,
                                StatInclude = reader.ReadBoolean()
                            });
                        }
                    }
                }

                var slotCount = reader.ReadSByte() + 1;
                var slots = new List<LegacyMxdSlotEntry>(Math.Max(slotCount, 0));
                for (var slotIndex = 0; slotIndex < slotCount; slotIndex++)
                {
                    var slotLevel = reader.ReadSByte();
                    var granted = saveFormat == LegacySaveBinaryFormat.Current && reader.ReadBoolean();
                    var enhancement = ReadEnhancement(reader, qualifiedNames, saveFormatVersion, compatibilityMap, legacyTag);
                    var hasFlipped = reader.ReadBoolean();
                    var flippedEnhancement = hasFlipped
                        ? ReadEnhancement(reader, qualifiedNames, saveFormatVersion, compatibilityMap, legacyTag)
                        : null;

                    slots.Add(new LegacyMxdSlotEntry
                    {
                        SlotIndex = slotIndex,
                        Level = slotLevel,
                        IsGranted = granted,
                        Enhancement = enhancement,
                        FlippedEnhancement = flippedEnhancement
                    });
                }

                powerEntries.Add(new LegacyMxdPowerEntry
                {
                    PowerIndex = powerIndex,
                    SavedStaticIndex = savedStaticIndex,
                    SavedUid = savedUid,
                    Level = level,
                    StatInclude = statInclude,
                    ProcInclude = procInclude,
                    VariableValue = variableValue,
                    InherentSlotsUsed = inherentSlotsUsed,
                    SubPowers = subPowers,
                    Slots = slots
                });
            }

            return new LegacyMxdBuild
            {
                LegacyTag = legacyTag,
                SaveFormatVersion = saveFormatVersion,
                SaveFormat = saveFormat,
                QualifiedNames = qualifiedNames,
                HasSubPowers = hasSubPowers,
                ClassUid = classUid,
                OriginUid = originUid,
                Alignment = alignment,
                CharacterName = characterName,
                PowerSets = powerSets,
                LastPower = lastPower,
                PowerEntries = powerEntries
            };
        }

        private static LegacyMxdEnhancementRef? ReadEnhancement(
            BinaryReader reader,
            bool qualifiedNames,
            float saveFormatVersion,
            LegacyHomecomingMap compatibilityMap,
            string legacyTag)
        {
            int? staticIndex = null;
            var savedUid = string.Empty;
            var encoding = LegacyEnhancementEncoding.None;
            if (qualifiedNames)
            {
                savedUid = reader.ReadString();
                if (string.IsNullOrWhiteSpace(savedUid))
                {
                    return null;
                }

                if (!LegacyMapResolver.TryCanonicalizeCurrentEnhancement(savedUid, out var currentEnhancementUid))
                {
                    var candidates = compatibilityMap.FindEnhancementEntries(savedUid, ExtractLeafName(savedUid));
                    var currentTargets = candidates
                        .Select(candidate => candidate.CurrentTarget)
                        .Where(target => !string.IsNullOrWhiteSpace(target))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();
                    if (currentTargets.Length != 1)
                    {
                        throw new InvalidDataException(
                            $"The qualified-name legacy enhancement reference '{savedUid}' could not be resolved.");
                    }

                    currentEnhancementUid = currentTargets[0];
                }

                var enhancementId = DatabaseAPI.NidFromUidEnh(currentEnhancementUid);
                if (enhancementId < 0)
                {
                    throw new InvalidDataException(
                        $"The qualified-name legacy enhancement reference '{savedUid}' targets unknown enhancement '{currentEnhancementUid}'.");
                }

                encoding = DatabaseAPI.Database.Enhancements[enhancementId].TypeID switch
                {
                    Enums.eType.Normal => LegacyEnhancementEncoding.NormalLike,
                    Enums.eType.SpecialO => LegacyEnhancementEncoding.NormalLike,
                    Enums.eType.InventO => LegacyEnhancementEncoding.InventLike,
                    Enums.eType.SetO => LegacyEnhancementEncoding.InventLike,
                    _ => LegacyEnhancementEncoding.None
                };
            }
            else
            {
                var rawStaticIndex = reader.ReadInt32();
                if (rawStaticIndex < 0)
                {
                    return null;
                }

                staticIndex = rawStaticIndex;
                if (!compatibilityMap.TryGetEnhancementEncoding(rawStaticIndex, out encoding))
                {
                    throw new InvalidDataException(
                        $"The legacy enhancement static index {rawStaticIndex} could not be resolved for Homecoming {legacyTag}.");
                }
            }

            int? ioLevel = null;
            int? relativeLevel = null;
            int? grade = null;

            if (!qualifiedNames && staticIndex.HasValue)
            {
                switch (encoding)
                {
                    case LegacyEnhancementEncoding.NormalLike:
                        relativeLevel = reader.ReadSByte();
                        grade = reader.ReadSByte();
                        break;
                    case LegacyEnhancementEncoding.InventLike:
                        ioLevel = reader.ReadSByte();
                        if (saveFormatVersion > 1.0f)
                        {
                            relativeLevel = reader.ReadSByte();
                        }
                        break;
                }
            }

            return new LegacyMxdEnhancementRef
            {
                SavedStaticIndex = staticIndex,
                SavedUid = savedUid,
                IoLevel = ioLevel,
                RelativeLevelRaw = relativeLevel,
                GradeRaw = grade
            };
        }

        private static string ExtractLeafName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var trimmed = value.Trim();
            var lastDot = trimmed.LastIndexOf('.');
            return lastDot >= 0 && lastDot < trimmed.Length - 1
                ? trimmed[(lastDot + 1)..]
                : trimmed;
        }

        private static byte[] ExtractPayloadBytes(string text)
        {
            var normalizedText = text.Replace("||", "|\n|");
            var lines = normalizedText.Split('\n');
            string[] headers = ["ABCD", "0", "0", "0"];
            var header = string.Empty;
            var dataIndex = -1;

            for (var index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                var startIndex = line.IndexOf(MagicUncompressed, StringComparison.Ordinal);
                if (startIndex < 0)
                {
                    startIndex = line.IndexOf(MagicCompressed, StringComparison.Ordinal);
                }

                if (startIndex < 0)
                {
                    startIndex = line.IndexOf(ModernCompressed, StringComparison.OrdinalIgnoreCase);
                }

                if (startIndex < 0)
                {
                    startIndex = line.IndexOf(LegacyUncompressed, StringComparison.OrdinalIgnoreCase);
                }

                if (startIndex < 0)
                {
                    startIndex = line.IndexOf(LegacyCompressed, StringComparison.OrdinalIgnoreCase);
                }

                if (startIndex < 0)
                {
                    continue;
                }

                headers = line[startIndex..].Split(';', StringSplitOptions.None);
                header = headers.Length > 0 ? headers[0] : string.Empty;
                dataIndex = index;
                break;
            }

            if (dataIndex < 0)
            {
                throw new InvalidDataException("Could not locate the legacy MxD payload header.");
            }

            if (lines.Length <= dataIndex + 1)
            {
                if (string.Equals(header, LegacyUncompressed, StringComparison.OrdinalIgnoreCase))
                {
                    return Encoding.UTF8.GetBytes(lines[dataIndex]);
                }

                throw new InvalidDataException("The legacy MxD payload body was empty.");
            }

            if (string.Equals(header, LegacyUncompressed, StringComparison.OrdinalIgnoreCase))
            {
                return Encoding.UTF8.GetBytes(string.Join("\n", lines[dataIndex..]));
            }

            var payload = string.Join("\n", lines[(dataIndex + 1)..]);
            var isHex = headers.Length > 4 && string.Equals(headers[4], "HEX", StringComparison.OrdinalIgnoreCase);
            var encodedBytes = Encoding.ASCII.GetBytes(isHex
                ? UnbreakHex(payload)
                : UnbreakString(payload, true));

            var encodedSize = Convert.ToInt32(headers[3], CultureInfo.InvariantCulture);
            if (encodedBytes.Length < encodedSize)
            {
                throw new InvalidDataException("The encoded legacy MxD payload was truncated.");
            }

            if (encodedBytes.Length > encodedSize)
            {
                Array.Resize(ref encodedBytes, encodedSize);
            }

            var rawBytes = isHex ? HexDecodeBytes(encodedBytes) : UuDecodeBytes(encodedBytes);
            if (string.Equals(header, MagicCompressed, StringComparison.Ordinal) ||
                string.Equals(header, ModernCompressed, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(header, LegacyCompressed, StringComparison.OrdinalIgnoreCase))
            {
                var uncompressedSize = Convert.ToInt32(headers[1], CultureInfo.InvariantCulture);
                rawBytes = DecompressChunk(rawBytes, uncompressedSize);
            }

            return rawBytes;
        }

        private static int FindMagicIndex(IReadOnlyList<byte> bytes)
        {
            for (var index = 0; index <= bytes.Count - MagicNumber.Length; index++)
            {
                var matched = true;
                for (var offset = 0; offset < MagicNumber.Length; offset++)
                {
                    if (bytes[index + offset] == MagicNumber[offset])
                    {
                        continue;
                    }

                    matched = false;
                    break;
                }

                if (matched)
                {
                    return index;
                }
            }

            return -1;
        }

        private static byte[] HexDecodeBytes(byte[] inputBytes)
        {
            var hexString = Encoding.ASCII.GetString(inputBytes);
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        private static byte[] UuDecodeBytes(byte[] inputBytes)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            for (var index = 0; index < inputBytes.Length; index += 4)
            {
                var bytes = inputBytes
                    .Skip(index)
                    .Take(4)
                    .Select(b => b == 96 ? (byte)32 : b)
                    .ToArray();

                var byte1 = ((bytes[0] - 32) << 2) | ((bytes[1] - 32) >> 4);
                var byte2 = ((bytes[1] - 32) & 0xF) << 4 | ((bytes[2] - 32) >> 2);
                var byte3 = ((bytes[2] - 32) & 0x3) << 6 | (bytes[3] - 32);

                writer.Write((byte)byte1);
                writer.Write((byte)byte2);
                writer.Write((byte)byte3);
            }

            return memoryStream.ToArray();
        }

        private static byte[] DecompressChunk(byte[] inputBytes, int expectedLength)
        {
            using var inputStream = new MemoryStream(inputBytes);
            using var zlibStream = new ZLibStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            zlibStream.CopyTo(outputStream);
            var output = outputStream.ToArray();
            Array.Resize(ref output, expectedLength);
            return output;
        }

        private static string UnbreakHex(string inputString)
        {
            var builder = new StringBuilder(inputString.Length);
            foreach (var ch in inputString)
            {
                if (ch is >= 'A' and <= 'Z' or >= '0' and <= '9')
                {
                    builder.Append(ch);
                }
            }

            return builder.ToString();
        }

        private static string UnbreakString(string inputString, bool bookend)
        {
            if (bookend)
            {
                inputString = string.Join(
                    Environment.NewLine,
                    inputString
                        .Split([Environment.NewLine], StringSplitOptions.None)
                        .Select(line => line.Trim('|')));
            }

            return inputString.Replace(Environment.NewLine, string.Empty);
        }
    }
}

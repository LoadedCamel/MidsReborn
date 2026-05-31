using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.BuildFile;
using Mids_Reborn.Core.BuildFile.DataModels;

namespace Mids_Reborn.Core.Compatibility
{
    internal enum LegacyCompatibilityLoadStatus
    {
        NotApplicable = 0,
        Success = 1,
        Failure = 2
    }

    internal static class CompatibilityBuildLoader
    {
        public static bool TryPrepareBuildDataForLoad(
            CharacterBuildData sourceBuild,
            string? sourceName,
            out CharacterBuildData normalizedBuild,
            out CompatibilityLoadSummary? summary,
            out CompatibilityFailureReport? failure)
        {
            return TryPrepareBuildDataForLoad(
                sourceBuild,
                compatibilityMap: null,
                sourceName,
                out normalizedBuild,
                out summary,
                out failure);
        }

        internal static bool TryPrepareBuildDataForLoad(
            CharacterBuildData sourceBuild,
            LegacyHomecomingMap? compatibilityMap,
            string? sourceName,
            out CharacterBuildData normalizedBuild,
            out CompatibilityLoadSummary? summary,
            out CompatibilityFailureReport? failure)
        {
            normalizedBuild = sourceBuild;
            summary = null;
            failure = null;

            if (sourceBuild?.BuiltWith == null ||
                !string.Equals(sourceBuild.BuiltWith.Database, "Homecoming", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (compatibilityMap == null &&
                !LegacyCompatibilityRegistry.TryGetCurrent(out compatibilityMap, out var mapError))
            {
                failure = new CompatibilityFailureReport
                {
                    Title = "Build Conversion Failed",
                    SourceName = sourceName ?? string.Empty,
                    Lineage = LegacyLineage.Homecoming,
                    Summary = mapError
                };
                return false;
            }

            if (compatibilityMap == null)
            {
                return true;
            }

            return TryNormalizeNamedBuild(
                sourceBuild,
                compatibilityMap,
                sourceName,
                sourceBuild.BuiltWith.DatabaseVersion.ToString(),
                out normalizedBuild,
                out summary,
                out failure);
        }

        public static LegacyCompatibilityLoadStatus TryLoadLegacyMxd(
            string text,
            string? sourceName,
            IBuildNotifier notifier)
        {
            if (!LegacyMxdParser.TryParseLegacyTag(text, out var legacyTag))
            {
                return LegacyCompatibilityLoadStatus.NotApplicable;
            }

            if (!LegacyCompatibilityRegistry.TryGetCurrent(out var compatibilityMap, out var mapError))
            {
                notifier.ShowCompatibilityFailure(new CompatibilityFailureReport
                {
                    Title = "Legacy Build Conversion Failed",
                    SourceName = sourceName ?? string.Empty,
                    Lineage = LegacyLineage.Homecoming,
                    LegacyTag = legacyTag,
                    Summary = mapError
                });
                return LegacyCompatibilityLoadStatus.Failure;
            }

            if (compatibilityMap == null)
            {
                return LegacyCompatibilityLoadStatus.NotApplicable;
            }

            if (!LegacyMxdParser.TryParse(text, compatibilityMap, out var parsedBuild, out var parseError) || parsedBuild == null)
            {
                notifier.ShowCompatibilityFailure(
                    BuildParseFailureReport(
                        sourceName,
                        legacyTag,
                        parseError,
                        compatibilityMap));
                return LegacyCompatibilityLoadStatus.Failure;
            }

            if (!TryBuildCurrentCharacterDataFromLegacy(
                    parsedBuild,
                    compatibilityMap,
                    sourceName,
                    out var normalizedBuild,
                    out var summary,
                    out var failure))
            {
                notifier.ShowCompatibilityFailure(failure ?? new CompatibilityFailureReport
                {
                    Title = "Legacy Build Conversion Failed",
                    SourceName = sourceName ?? string.Empty,
                    Lineage = LegacyLineage.Homecoming,
                    LegacyTag = legacyTag,
                    Summary = "The legacy build could not be converted."
                });
                return LegacyCompatibilityLoadStatus.Failure;
            }

            if (!normalizedBuild.LoadBuild())
            {
                return LegacyCompatibilityLoadStatus.Failure;
            }

            if (summary?.HasUserVisibleChanges == true)
            {
                notifier.ShowCompatibilitySummary(summary);
            }

            return LegacyCompatibilityLoadStatus.Success;
        }

        private static bool TryBuildCurrentCharacterDataFromLegacy(
            LegacyMxdBuild parsedBuild,
            LegacyHomecomingMap compatibilityMap,
            string? sourceName,
            out CharacterBuildData normalizedBuild,
            out CompatibilityLoadSummary? summary,
            out CompatibilityFailureReport? failure)
        {
            summary = new CompatibilityLoadSummary
            {
                SourceName = sourceName ?? string.Empty,
                Lineage = compatibilityMap.Lineage,
                LegacyTag = parsedBuild.LegacyTag
            };
            failure = new CompatibilityFailureReport
            {
                Title = "Legacy Build Conversion Failed",
                SourceName = sourceName ?? string.Empty,
                Lineage = compatibilityMap.Lineage,
                LegacyTag = parsedBuild.LegacyTag
            };

            normalizedBuild = new CharacterBuildData
            {
                BuiltWith = new MetaData(
                    MidsContext.AppName,
                    MidsContext.AppFileVersion,
                    DatabaseAPI.DatabaseName,
                    DatabaseAPI.Database.Version),
                Class = parsedBuild.ClassUid,
                Origin = parsedBuild.OriginUid,
                Alignment = parsedBuild.Alignment.ToString(),
                Name = parsedBuild.CharacterName,
                Comment = string.Empty,
                LastPower = parsedBuild.LastPower,
                CombatContext = null
            };

            var resolvedPowerSetMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var powerEntries = new List<PowerData?>(parsedBuild.PowerEntries.Count);

            foreach (var entry in parsedBuild.PowerEntries)
            {
                if (!TryConvertLegacyPowerEntry(
                        parsedBuild,
                        entry,
                        compatibilityMap,
                        resolvedPowerSetMappings,
                        summary,
                        failure,
                        out var convertedPowerEntry))
                {
                    normalizedBuild = new CharacterBuildData();
                    return false;
                }

                powerEntries.Add(convertedPowerEntry);
            }

            normalizedBuild.PowerEntries = powerEntries;
            normalizedBuild.PowerSets = NormalizePowerSets(parsedBuild.PowerSets, resolvedPowerSetMappings, failure);
            if (normalizedBuild.PowerSets.Count != parsedBuild.PowerSets.Count)
            {
                normalizedBuild = new CharacterBuildData();
                failure.Summary = "One or more legacy powersets could not be mapped to the current Homecoming database.";
                return false;
            }

            normalizedBuild.Level = ComputeBuildLevel(powerEntries).ToString();
            failure = failure.HasItems ? failure : null;
            return true;
        }

        private static bool TryNormalizeNamedBuild(
            CharacterBuildData sourceBuild,
            LegacyHomecomingMap compatibilityMap,
            string? sourceName,
            string legacyTag,
            out CharacterBuildData normalizedBuild,
            out CompatibilityLoadSummary? summary,
            out CompatibilityFailureReport? failure)
        {
            summary = new CompatibilityLoadSummary
            {
                SourceName = sourceName ?? string.Empty,
                Lineage = compatibilityMap.Lineage,
                LegacyTag = legacyTag
            };
            failure = new CompatibilityFailureReport
            {
                Title = "Build Conversion Failed",
                SourceName = sourceName ?? string.Empty,
                Lineage = compatibilityMap.Lineage,
                LegacyTag = legacyTag
            };

            var resolvedPowerSetMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var normalizedPowerEntries = new List<PowerData?>(sourceBuild.PowerEntries.Count);
            foreach (var powerEntry in sourceBuild.PowerEntries)
            {
                if (!TryNormalizeNamedPowerEntry(
                        sourceBuild,
                        powerEntry,
                        compatibilityMap,
                        resolvedPowerSetMappings,
                        summary,
                        failure,
                        out var normalizedPowerEntry))
                {
                    normalizedBuild = sourceBuild;
                    return false;
                }

                normalizedPowerEntries.Add(normalizedPowerEntry);
            }

            var normalizedPowerSets = NormalizePowerSets(sourceBuild.PowerSets, resolvedPowerSetMappings, failure);
            if (normalizedPowerSets.Count != sourceBuild.PowerSets.Count)
            {
                normalizedBuild = sourceBuild;
                failure.Summary = "One or more build powersets could not be mapped to the current Homecoming database.";
                return false;
            }

            normalizedBuild = new CharacterBuildData
            {
                BuiltWith = sourceBuild.BuiltWith,
                Level = sourceBuild.Level,
                Class = sourceBuild.Class,
                Origin = sourceBuild.Origin,
                Alignment = sourceBuild.Alignment,
                Name = sourceBuild.Name,
                Comment = sourceBuild.Comment,
                PowerSets = normalizedPowerSets,
                LastPower = sourceBuild.LastPower,
                PowerEntries = normalizedPowerEntries,
                CombatContext = sourceBuild.CombatContext == null
                    ? null
                    : CombatContextState.Clone(sourceBuild.CombatContext)
            };

            failure = failure.HasItems ? failure : null;
            return true;
        }

        private static bool TryConvertLegacyPowerEntry(
            LegacyMxdBuild parsedBuild,
            LegacyMxdPowerEntry entry,
            LegacyHomecomingMap compatibilityMap,
            IDictionary<string, string> resolvedPowerSetMappings,
            CompatibilityLoadSummary summary,
            CompatibilityFailureReport failure,
            out PowerData normalizedPowerEntry)
        {
            normalizedPowerEntry = CreateEmptyPowerData();
            if (!entry.SavedStaticIndex.HasValue && string.IsNullOrWhiteSpace(entry.SavedUid))
            {
                return true;
            }

            if (ShouldIgnoreLegacyPowerReference(compatibilityMap, entry.SavedStaticIndex, entry.SavedUid))
            {
                return true;
            }

            if (!LegacyMapResolver.TryResolveLegacyPowerTarget(
                    compatibilityMap,
                    entry.SavedStaticIndex,
                    entry.SavedUid,
                    parsedBuild.PowerSets,
                    resolvedPowerSetMappings.Values.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                    parsedBuild.ClassUid,
                    null,
                    out var currentPowerUid,
                    out var matchedEntry,
                    out var error))
            {
                failure.Items.Add(new CompatibilityFailureItem
                {
                    Category = "Power",
                    PowerIndex = entry.PowerIndex,
                    StaticIndex = entry.SavedStaticIndex,
                    LegacyFullName = entry.SavedUid,
                    Message = $"Power entry {entry.PowerIndex + 1} could not be mapped. {error}"
                });
                failure.Summary = "One or more legacy powers could not be mapped to the current Homecoming database.";
                return false;
            }

            normalizedPowerEntry = new PowerData
            {
                PowerName = currentPowerUid,
                Level = entry.Level + 1,
                StatInclude = entry.StatInclude,
                ProcInclude = entry.ProcInclude,
                VariableValue = entry.VariableValue,
                InherentSlotsUsed = entry.InherentSlotsUsed
            };

            if (!string.IsNullOrWhiteSpace(entry.SavedUid) &&
                !string.Equals(entry.SavedUid, currentPowerUid, StringComparison.OrdinalIgnoreCase))
            {
                summary.RemappedPowerCount++;
            }

            RegisterPowerSetMapping(resolvedPowerSetMappings, matchedEntry?.LegacyFullName ?? entry.SavedUid, currentPowerUid);

            foreach (var subPower in entry.SubPowers)
            {
                if (!subPower.SavedStaticIndex.HasValue && string.IsNullOrWhiteSpace(subPower.SavedUid))
                {
                    normalizedPowerEntry.SubPowerEntries.Add(new SubPowerData
                    {
                        PowerName = string.Empty,
                        StatInclude = subPower.StatInclude
                    });
                    continue;
                }

                if (ShouldIgnoreLegacyPowerReference(compatibilityMap, subPower.SavedStaticIndex, subPower.SavedUid))
                {
                    normalizedPowerEntry.SubPowerEntries.Add(new SubPowerData
                    {
                        PowerName = string.Empty,
                        StatInclude = subPower.StatInclude
                    });
                    continue;
                }

                if (!LegacyMapResolver.TryResolveLegacyPowerTarget(
                        compatibilityMap,
                        subPower.SavedStaticIndex,
                        subPower.SavedUid,
                        parsedBuild.PowerSets,
                        resolvedPowerSetMappings.Values.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                        parsedBuild.ClassUid,
                        currentPowerUid,
                        out var currentSubPowerUid,
                        out _,
                        out var subError))
                {
                    failure.Items.Add(new CompatibilityFailureItem
                    {
                        Category = "SubPower",
                        PowerIndex = entry.PowerIndex,
                        StaticIndex = subPower.SavedStaticIndex,
                        LegacyFullName = subPower.SavedUid,
                        Message = $"Sub-power {subPower.SubPowerIndex + 1} on power entry {entry.PowerIndex + 1} could not be mapped. {subError}"
                    });
                    failure.Summary = "One or more legacy sub-powers could not be mapped.";
                    return false;
                }

                normalizedPowerEntry.SubPowerEntries.Add(new SubPowerData
                {
                    PowerName = currentSubPowerUid,
                    StatInclude = subPower.StatInclude
                });
            }

            var resolvedSetUids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var slot in entry.Slots)
            {
                if (!TryConvertLegacySlot(
                        slot,
                        compatibilityMap,
                        currentPowerUid,
                        resolvedSetUids,
                        summary,
                        failure,
                        out var normalizedSlot))
                {
                    return false;
                }

                normalizedPowerEntry.SlotEntries.Add(normalizedSlot);
            }

            return true;
        }

        private static bool TryNormalizeNamedPowerEntry(
            CharacterBuildData sourceBuild,
            PowerData? sourcePowerEntry,
            LegacyHomecomingMap compatibilityMap,
            IDictionary<string, string> resolvedPowerSetMappings,
            CompatibilityLoadSummary summary,
            CompatibilityFailureReport failure,
            out PowerData normalizedPowerEntry)
        {
            normalizedPowerEntry = CreateEmptyPowerData();
            if (sourcePowerEntry == null || string.IsNullOrWhiteSpace(sourcePowerEntry.PowerName))
            {
                return true;
            }

            if (ShouldIgnoreLegacyPowerReference(compatibilityMap, legacyStaticIndex: null, sourcePowerEntry.PowerName))
            {
                return true;
            }

            if (!LegacyMapResolver.TryResolveNamedPowerTarget(
                    compatibilityMap,
                    sourcePowerEntry.PowerName,
                    sourceBuild.PowerSets,
                    resolvedPowerSetMappings.Values.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                    sourceBuild.Class,
                    null,
                    out var currentPowerUid,
                    out var matchedEntry,
                    out var error))
            {
                failure.Items.Add(new CompatibilityFailureItem
                {
                    Category = "Power",
                    LegacyFullName = sourcePowerEntry.PowerName,
                    Message = $"Power '{sourcePowerEntry.PowerName}' could not be mapped. {error}"
                });
                failure.Summary = "One or more build powers could not be mapped to the current Homecoming database.";
                return false;
            }

            normalizedPowerEntry = new PowerData
            {
                PowerName = currentPowerUid,
                Level = sourcePowerEntry.Level,
                StatInclude = sourcePowerEntry.StatInclude,
                ProcInclude = sourcePowerEntry.ProcInclude,
                VariableValue = sourcePowerEntry.VariableValue,
                InherentSlotsUsed = sourcePowerEntry.InherentSlotsUsed
            };

            if (!string.Equals(sourcePowerEntry.PowerName, currentPowerUid, StringComparison.OrdinalIgnoreCase))
            {
                summary.RemappedPowerCount++;
            }

            RegisterPowerSetMapping(resolvedPowerSetMappings, matchedEntry?.LegacyFullName ?? sourcePowerEntry.PowerName, currentPowerUid);

            foreach (var subPower in sourcePowerEntry.SubPowerEntries)
            {
                if (string.IsNullOrWhiteSpace(subPower.PowerName))
                {
                    normalizedPowerEntry.SubPowerEntries.Add(new SubPowerData
                    {
                        PowerName = string.Empty,
                        StatInclude = subPower.StatInclude
                    });
                    continue;
                }

                if (ShouldIgnoreLegacyPowerReference(compatibilityMap, legacyStaticIndex: null, subPower.PowerName))
                {
                    normalizedPowerEntry.SubPowerEntries.Add(new SubPowerData
                    {
                        PowerName = string.Empty,
                        StatInclude = subPower.StatInclude
                    });
                    continue;
                }

                if (!LegacyMapResolver.TryResolveNamedPowerTarget(
                        compatibilityMap,
                        subPower.PowerName,
                        sourceBuild.PowerSets,
                        resolvedPowerSetMappings.Values.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                        sourceBuild.Class,
                        currentPowerUid,
                        out var currentSubPowerUid,
                        out _,
                        out var subError))
                {
                    failure.Items.Add(new CompatibilityFailureItem
                    {
                        Category = "SubPower",
                        LegacyFullName = subPower.PowerName,
                        Message = $"Sub-power '{subPower.PowerName}' could not be mapped. {subError}"
                    });
                    failure.Summary = "One or more build sub-powers could not be mapped to the current Homecoming database.";
                    return false;
                }

                normalizedPowerEntry.SubPowerEntries.Add(new SubPowerData
                {
                    PowerName = currentSubPowerUid,
                    StatInclude = subPower.StatInclude
                });
            }

            var resolvedSetUids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var slot in sourcePowerEntry.SlotEntries)
            {
                if (!TryNormalizeNamedSlot(
                        slot,
                        compatibilityMap,
                        currentPowerUid,
                        resolvedSetUids,
                        summary,
                        failure,
                        out var normalizedSlot))
                {
                    return false;
                }

                normalizedPowerEntry.SlotEntries.Add(normalizedSlot);
            }

            return true;
        }

        private static bool TryConvertLegacySlot(
            LegacyMxdSlotEntry sourceSlot,
            LegacyHomecomingMap compatibilityMap,
            string currentPowerUid,
            ISet<string> resolvedSetUids,
            CompatibilityLoadSummary summary,
            CompatibilityFailureReport failure,
            out SlotData normalizedSlot)
        {
            normalizedSlot = new SlotData
            {
                Level = sourceSlot.Level + 1,
                IsInherent = sourceSlot.IsGranted,
                SlotSource = sourceSlot.IsGranted
                    ? SlotSourceKind.Granted.ToString()
                    : sourceSlot.SlotIndex == 0
                        ? SlotSourceKind.AutoBase.ToString()
                        : SlotSourceKind.Bought.ToString()
            };

            if (!TryConvertLegacyEnhancement(
                    sourceSlot.Enhancement,
                    compatibilityMap,
                    currentPowerUid,
                    resolvedSetUids,
                    summary,
                    failure,
                    sourceSlot.SlotIndex,
                    out var enhancement))
            {
                return false;
            }

            if (!TryConvertLegacyEnhancement(
                    sourceSlot.FlippedEnhancement,
                    compatibilityMap,
                    currentPowerUid,
                    resolvedSetUids,
                    summary,
                    failure,
                    sourceSlot.SlotIndex,
                    out var flippedEnhancement))
            {
                return false;
            }

            normalizedSlot.Enhancement = enhancement;
            normalizedSlot.FlippedEnhancement = flippedEnhancement;
            return true;
        }

        private static bool TryNormalizeNamedSlot(
            SlotData sourceSlot,
            LegacyHomecomingMap compatibilityMap,
            string currentPowerUid,
            ISet<string> resolvedSetUids,
            CompatibilityLoadSummary summary,
            CompatibilityFailureReport failure,
            out SlotData normalizedSlot)
        {
            normalizedSlot = new SlotData
            {
                Level = sourceSlot.Level,
                IsInherent = sourceSlot.IsInherent,
                SlotSource = sourceSlot.SlotSource,
                GrantedRuleId = sourceSlot.GrantedRuleId
            };

            if (!TryNormalizeNamedEnhancement(
                    sourceSlot.Enhancement,
                    compatibilityMap,
                    currentPowerUid,
                    resolvedSetUids,
                    summary,
                    failure,
                    out var enhancement))
            {
                return false;
            }

            if (!TryNormalizeNamedEnhancement(
                    sourceSlot.FlippedEnhancement,
                    compatibilityMap,
                    currentPowerUid,
                    resolvedSetUids,
                    summary,
                    failure,
                    out var flippedEnhancement))
            {
                return false;
            }

            normalizedSlot.Enhancement = enhancement;
            normalizedSlot.FlippedEnhancement = flippedEnhancement;
            return true;
        }

        private static bool TryConvertLegacyEnhancement(
            LegacyMxdEnhancementRef? sourceEnhancement,
            LegacyHomecomingMap compatibilityMap,
            string currentPowerUid,
            ISet<string> resolvedSetUids,
            CompatibilityLoadSummary summary,
            CompatibilityFailureReport failure,
            int slotIndex,
            out EnhancementData? enhancementData)
        {
            enhancementData = null;
            if (sourceEnhancement == null)
            {
                return true;
            }

            if (!LegacyMapResolver.TryResolveLegacyEnhancementTarget(
                    compatibilityMap,
                    sourceEnhancement.SavedStaticIndex,
                    sourceEnhancement.SavedUid,
                    null,
                    currentPowerUid,
                    resolvedSetUids.ToArray(),
                    out var currentEnhancementUid,
                    out var matchedEntry,
                    out var error))
            {
                failure.Items.Add(new CompatibilityFailureItem
                {
                    Category = "Enhancement",
                    SlotIndex = slotIndex,
                    StaticIndex = sourceEnhancement.SavedStaticIndex,
                    LegacyFullName = sourceEnhancement.SavedUid,
                    Message = $"Enhancement at slot {slotIndex + 1} could not be mapped. {error}"
                });
                failure.Summary = "One or more legacy enhancements could not be mapped to the current Homecoming database.";
                return false;
            }

            enhancementData = new EnhancementData
            {
                Uid = currentEnhancementUid,
                IoLevel = sourceEnhancement.IoLevel ?? 1,
                RelativeLevel = sourceEnhancement.RelativeLevelRaw.HasValue
                    ? ((Enums.eEnhRelative)sourceEnhancement.RelativeLevelRaw.Value).ToString()
                    : Enums.eEnhRelative.Even.ToString(),
                Grade = sourceEnhancement.GradeRaw.HasValue
                    ? ((Enums.eEnhGrade)sourceEnhancement.GradeRaw.Value).ToString()
                    : Enums.eEnhGrade.None.ToString()
            };

            var sourceIdentity = !string.IsNullOrWhiteSpace(sourceEnhancement.SavedUid)
                ? sourceEnhancement.SavedUid
                : matchedEntry?.LegacyFullName ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(sourceIdentity) &&
                !string.Equals(sourceIdentity, currentEnhancementUid, StringComparison.OrdinalIgnoreCase))
            {
                summary.RemappedEnhancementCount++;
            }

            RegisterResolvedSetUid(resolvedSetUids, currentEnhancementUid);
            return true;
        }

        private static bool TryNormalizeNamedEnhancement(
            EnhancementData? sourceEnhancement,
            LegacyHomecomingMap compatibilityMap,
            string currentPowerUid,
            ISet<string> resolvedSetUids,
            CompatibilityLoadSummary summary,
            CompatibilityFailureReport failure,
            out EnhancementData? normalizedEnhancement)
        {
            normalizedEnhancement = null;
            if (sourceEnhancement == null)
            {
                return true;
            }

            if (!LegacyMapResolver.TryResolveNamedEnhancementTarget(
                    compatibilityMap,
                    sourceEnhancement.Uid,
                    sourceEnhancement.LegacyDisplayName,
                    currentPowerUid,
                    resolvedSetUids.ToArray(),
                    out var currentEnhancementUid,
                    out var matchedEntry,
                    out var error))
            {
                var legacyIdentity = !string.IsNullOrWhiteSpace(sourceEnhancement.Uid)
                    ? sourceEnhancement.Uid
                    : sourceEnhancement.LegacyDisplayName;
                failure.Items.Add(new CompatibilityFailureItem
                {
                    Category = "Enhancement",
                    LegacyFullName = legacyIdentity,
                    Message = $"Enhancement '{legacyIdentity}' could not be mapped. {error}"
                });
                failure.Summary = "One or more build enhancements could not be mapped to the current Homecoming database.";
                return false;
            }

            normalizedEnhancement = new EnhancementData
            {
                Uid = currentEnhancementUid,
                Grade = sourceEnhancement.Grade,
                IoLevel = sourceEnhancement.IoLevel,
                RelativeLevel = sourceEnhancement.RelativeLevel,
                Obtained = sourceEnhancement.Obtained,
                LegacyDisplayName = sourceEnhancement.LegacyDisplayName
            };

            var sourceIdentity = !string.IsNullOrWhiteSpace(sourceEnhancement.Uid)
                ? sourceEnhancement.Uid
                : sourceEnhancement.LegacyDisplayName;
            if (!string.IsNullOrWhiteSpace(sourceIdentity) &&
                !string.Equals(sourceIdentity, currentEnhancementUid, StringComparison.OrdinalIgnoreCase))
            {
                summary.RemappedEnhancementCount++;
            }

            if (matchedEntry != null && string.IsNullOrWhiteSpace(normalizedEnhancement.LegacyDisplayName))
            {
                normalizedEnhancement.LegacyDisplayName = matchedEntry.LegacyName;
            }

            RegisterResolvedSetUid(resolvedSetUids, currentEnhancementUid);
            return true;
        }

        private static List<string> NormalizePowerSets(
            IEnumerable<string> sourcePowerSets,
            IReadOnlyDictionary<string, string> resolvedPowerSetMappings,
            CompatibilityFailureReport failure)
        {
            var normalized = new List<string>();
            foreach (var sourcePowerSet in sourcePowerSets)
            {
                if (string.IsNullOrWhiteSpace(sourcePowerSet))
                {
                    normalized.Add(string.Empty);
                    continue;
                }

                var candidate = ApplyLegacyPowerSetAlias(sourcePowerSet);
                var currentPowerset = DatabaseAPI.GetPowersetByName(candidate);
                if (currentPowerset != null)
                {
                    normalized.Add(currentPowerset.FullName);
                    continue;
                }

                if (resolvedPowerSetMappings.TryGetValue(sourcePowerSet, out var mappedPowerSet) ||
                    resolvedPowerSetMappings.TryGetValue(candidate, out mappedPowerSet))
                {
                    normalized.Add(mappedPowerSet);
                    continue;
                }

                var currentMatches = DatabaseAPI.Database.Powersets
                    .Where(powerSet => powerSet != null &&
                                       string.Equals(ExtractLeafName(powerSet.FullName), ExtractLeafName(candidate), StringComparison.OrdinalIgnoreCase))
                    .Select(powerSet => powerSet!.FullName)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(2)
                    .ToArray();
                if (currentMatches.Length == 1)
                {
                    normalized.Add(currentMatches[0]);
                    continue;
                }

                failure.Items.Add(new CompatibilityFailureItem
                {
                    Category = "Powerset",
                    LegacyFullName = sourcePowerSet,
                    Message = $"Powerset '{sourcePowerSet}' could not be mapped to the current Homecoming database."
                });
            }

            return normalized;
        }

        private static string ApplyLegacyPowerSetAlias(string sourcePowerSet)
        {
            return sourcePowerSet switch
            {
                "Pool.Leadership_beta" => "Pool.Leadership",
                "Blaster_Support.Atomic_Manipulation" => "Blaster_Support.Radiation_Manipulation",
                "Pool.Fitness" => "Pool.Invisibility",
                _ => sourcePowerSet
            };
        }

        private static void RegisterPowerSetMapping(
            IDictionary<string, string> resolvedPowerSetMappings,
            string? legacyPowerUid,
            string currentPowerUid)
        {
            var currentPowerIndex = DatabaseAPI.PiDFromUidPower(currentPowerUid);
            if (currentPowerIndex < 0)
            {
                return;
            }

            var currentPowerSet = DatabaseAPI.Database.Power[currentPowerIndex].GetPowerSet()?.FullName;
            if (string.IsNullOrWhiteSpace(currentPowerSet))
            {
                return;
            }

            resolvedPowerSetMappings[currentPowerSet] = currentPowerSet;

            if (string.IsNullOrWhiteSpace(legacyPowerUid))
            {
                return;
            }

            var legacyPowerSet = GetPowerSetFullName(legacyPowerUid);
            if (!string.IsNullOrWhiteSpace(legacyPowerSet))
            {
                resolvedPowerSetMappings[legacyPowerSet] = currentPowerSet;
            }
        }

        private static void RegisterResolvedSetUid(ISet<string> resolvedSetUids, string currentEnhancementUid)
        {
            var enhancementId = DatabaseAPI.NidFromUidEnh(currentEnhancementUid);
            if (enhancementId < 0)
            {
                return;
            }

            var setUid = DatabaseAPI.Database.Enhancements[enhancementId].UIDSet;
            if (!string.IsNullOrWhiteSpace(setUid))
            {
                resolvedSetUids.Add(setUid);
            }
        }

        private static string GetPowerSetFullName(string? powerUid)
        {
            if (string.IsNullOrWhiteSpace(powerUid))
            {
                return string.Empty;
            }

            var lastDot = powerUid.LastIndexOf('.');
            return lastDot <= 0 ? string.Empty : powerUid[..lastDot];
        }

        private static string ExtractLeafName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return string.Empty;
            }

            var lastDot = fullName.LastIndexOf('.');
            return lastDot >= 0 && lastDot < fullName.Length - 1
                ? fullName[(lastDot + 1)..]
                : fullName;
        }

        private static PowerData CreateEmptyPowerData()
        {
            return new PowerData
            {
                PowerName = string.Empty,
                Level = -1,
                StatInclude = false,
                ProcInclude = false,
                VariableValue = 0,
                InherentSlotsUsed = 0
            };
        }

        private static bool ShouldIgnoreLegacyPowerReference(
            LegacyHomecomingMap compatibilityMap,
            int? legacyStaticIndex,
            string? legacyFullName)
        {
            if (ShouldIgnoreLegacyPowerIdentifier(legacyFullName))
            {
                return true;
            }

            if (!legacyStaticIndex.HasValue)
            {
                return false;
            }

            if (legacyStaticIndex.Value == 11536)
            {
                return true;
            }

            var entries = compatibilityMap.GetPowerEntries(legacyStaticIndex.Value);
            return entries.Count > 0 &&
                   entries.All(entry =>
                       ShouldIgnoreLegacyPowerIdentifier(entry.LegacyFullName) ||
                       ShouldIgnoreLegacyPowerIdentifier(entry.LegacyName));
        }

        private static bool ShouldIgnoreLegacyPowerIdentifier(string? legacyFullName)
        {
            if (string.IsNullOrWhiteSpace(legacyFullName))
            {
                return false;
            }

            var normalized = Regex.Replace(legacyFullName.Trim().ToUpperInvariant(), @"[^A-Z0-9]+", string.Empty);
            if (string.Equals(normalized, "INHERENTINHERENTSPECIALSETBONUSES", StringComparison.Ordinal))
            {
                return true;
            }

            if (!legacyFullName.StartsWith("Temporary_Powers.", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return normalized.StartsWith("TEMPORARYPOWERSTEMPORARYPOWERSHC", StringComparison.Ordinal) ||
                   normalized.Contains("DISGUISE", StringComparison.Ordinal) ||
                   normalized.Contains("COSTUME", StringComparison.Ordinal) ||
                   normalized.Contains("UNIFORM", StringComparison.Ordinal);
        }

        private static CompatibilityFailureReport BuildParseFailureReport(
            string? sourceName,
            string legacyTag,
            string parseError,
            LegacyHomecomingMap compatibilityMap)
        {
            var report = new CompatibilityFailureReport
            {
                Title = "Legacy Build Conversion Failed",
                SourceName = sourceName ?? string.Empty,
                Lineage = LegacyLineage.Homecoming,
                LegacyTag = legacyTag,
                Summary = parseError
            };

            AddStaticIndexFailureDetails(
                parseError,
                report,
                @"legacy enhancement static index (?<index>\d+)",
                LegacyMapKind.Enhancement,
                staticIndex => compatibilityMap.GetEnhancementEntries(staticIndex));

            AddStaticIndexFailureDetails(
                parseError,
                report,
                @"legacy power static index (?<index>\d+)",
                LegacyMapKind.Power,
                staticIndex => compatibilityMap.GetPowerEntries(staticIndex));

            return report;
        }

        private static void AddStaticIndexFailureDetails(
            string parseError,
            CompatibilityFailureReport report,
            string pattern,
            LegacyMapKind kind,
            Func<int, IReadOnlyList<LegacyMapEntry>> entryLookup)
        {
            var match = Regex.Match(parseError ?? string.Empty, pattern, RegexOptions.IgnoreCase);
            if (!match.Success || !int.TryParse(match.Groups["index"].Value, out var staticIndex))
            {
                return;
            }

            var entries = entryLookup(staticIndex);
            if (entries.Count == 0)
            {
                report.Items.Add(new CompatibilityFailureItem
                {
                    Category = kind.ToString(),
                    Message = $"{kind} static index {staticIndex} could not be resolved from the legacy load map.",
                    StaticIndex = staticIndex
                });
                return;
            }

            foreach (var entry in entries)
            {
                var targetText = string.IsNullOrWhiteSpace(entry.CurrentTarget)
                    ? "no current target assigned"
                    : $"current target '{entry.CurrentTarget}'";

                report.Items.Add(new CompatibilityFailureItem
                {
                    Category = kind.ToString(),
                    Message =
                        $"{kind} static index {staticIndex}: legacy '{entry.LegacyFullName}' ({entry.LegacyName}) -> {targetText}.",
                    StaticIndex = staticIndex,
                    LegacyName = entry.LegacyName,
                    LegacyFullName = entry.LegacyFullName,
                    CurrentTarget = entry.CurrentTarget
                });
            }
        }

        private static int ComputeBuildLevel(IEnumerable<PowerData?> powerEntries)
        {
            var maxPowerLevel = powerEntries
                .Where(powerEntry => powerEntry != null && !string.IsNullOrWhiteSpace(powerEntry.PowerName))
                .Select(powerEntry => powerEntry!.Level)
                .DefaultIfEmpty(1)
                .Max();
            return Math.Max(1, maxPowerLevel);
        }
    }
}

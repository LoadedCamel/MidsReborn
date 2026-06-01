using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;
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

            var normalized = TryNormalizeNamedBuild(
                sourceBuild,
                compatibilityMap,
                sourceName,
                sourceBuild.BuiltWith.DatabaseVersion.ToString(),
                out normalizedBuild,
                out summary,
                out failure);
            if (normalized && normalizedBuild != null && summary != null)
            {
                ApplyLegacyFitnessMigrationFromNamedSource(sourceBuild, normalizedBuild, summary);
            }

            return normalized;
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

            var hasLegacyInternalPayload =
                text.Contains(AppDataPaths.Headers.Save.LegacyCompressed, StringComparison.OrdinalIgnoreCase) ||
                text.Contains(AppDataPaths.Headers.Save.LegacyUncompressed, StringComparison.OrdinalIgnoreCase);

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
                if (hasLegacyInternalPayload)
                {
                    var fallbackStatus = TryLoadLegacyPlainTextBuild(text, sourceName, legacyTag, notifier);
                    if (fallbackStatus != LegacyCompatibilityLoadStatus.NotApplicable)
                    {
                        return fallbackStatus;
                    }
                }

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

            ApplyDeclaredPowerSetLayoutFromText(text, normalizedBuild);
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
            ApplyLegacyFitnessMigrationFromParsedBuild(parsedBuild, normalizedBuild, summary);
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

                if (string.Equals(sourcePowerSet, "Pool.Fitness", StringComparison.OrdinalIgnoreCase))
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

        private static void ApplyDeclaredPowerSetLayoutFromText(string text, CharacterBuildData normalizedBuild)
        {
            if (normalizedBuild == null)
            {
                return;
            }

            if (!Regex.IsMatch(text, @"(Primary|Secondary)\s+Power\s+Set\:|Power\s+Pool\:|Ancillary\s+Pool\:", RegexOptions.IgnoreCase))
            {
                return;
            }

            var slottedPowerSets = BuildLegacyPlannerPowerSetLayout(text, normalizedBuild);
            if (slottedPowerSets.Count == 8)
            {
                normalizedBuild.PowerSets = slottedPowerSets;
            }
        }

        private static List<string> BuildLegacyPlannerPowerSetLayout(string text, CharacterBuildData normalizedBuild)
        {
            var slotted = Enumerable.Repeat(string.Empty, 8).ToList();
            slotted[0] = ResolveDeclaredPowerSetFullName(
                TryExtractDeclaredPowerSet(text, "Primary"),
                normalizedBuild,
                Enums.ePowerSetType.Primary,
                fallbackIndex: 0);
            slotted[1] = ResolveDeclaredPowerSetFullName(
                TryExtractDeclaredPowerSet(text, "Secondary"),
                normalizedBuild,
                Enums.ePowerSetType.Secondary,
                fallbackIndex: 1);
            slotted[2] = ResolveInherentPowerSetFullName(normalizedBuild);

            var poolSlot = 3;
            foreach (Match match in Regex.Matches(text, @"Power\s+Pool\:\s*([^\r\n]+)", RegexOptions.IgnoreCase))
            {
                if (poolSlot > 6)
                {
                    break;
                }

                var declaredPool = match.Groups[1].Value.Trim();
                if (string.IsNullOrWhiteSpace(declaredPool) ||
                    string.Equals(declaredPool, "Fitness", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var resolvedPool = ResolveDeclaredPowerSetFullName(
                    declaredPool,
                    normalizedBuild,
                    Enums.ePowerSetType.Pool);
                if (string.IsNullOrWhiteSpace(resolvedPool) ||
                    slotted.Skip(3).Take(poolSlot - 3).Contains(resolvedPool, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                slotted[poolSlot++] = resolvedPool;
            }

            slotted[7] = ResolveDeclaredPowerSetFullName(
                TryExtractDeclaredPowerSet(text, "Ancillary"),
                normalizedBuild,
                Enums.ePowerSetType.Ancillary,
                fallbackIndex: 7);
            return slotted;
        }

        private static string TryExtractDeclaredPowerSet(string text, string label)
        {
            var match = Regex.Match(text, $@"{label}\s+Power\s+Set\:\s*([^\r\n]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            match = Regex.Match(text, $@"{label}\s+Pool\:\s*([^\r\n]+)", RegexOptions.IgnoreCase);
            return match.Success
                ? match.Groups[1].Value.Trim()
                : string.Empty;
        }

        private static string ResolveInherentPowerSetFullName(CharacterBuildData normalizedBuild)
        {
            var existing = normalizedBuild.PowerSets
                .FirstOrDefault(powerSet => !string.IsNullOrWhiteSpace(powerSet) &&
                                            powerSet.StartsWith("Inherent.", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(existing))
            {
                return existing;
            }

            return DatabaseAPI.GetInherentPowerset()?.FullName ?? string.Empty;
        }

        private static string ResolveDeclaredPowerSetFullName(
            string declaredPowerSetName,
            CharacterBuildData normalizedBuild,
            Enums.ePowerSetType setType,
            int fallbackIndex = -1)
        {
            if (string.IsNullOrWhiteSpace(declaredPowerSetName))
            {
                return fallbackIndex >= 0 &&
                       fallbackIndex < normalizedBuild.PowerSets.Count
                    ? normalizedBuild.PowerSets[fallbackIndex]
                    : string.Empty;
            }

            var archetypeName = ResolveArchetypeDisplayName(normalizedBuild.Class);
            var powerset = DatabaseAPI.GetPowersetByName(declaredPowerSetName, archetypeName, true);
            if (powerset?.SetType == setType)
            {
                return powerset.FullName;
            }

            var normalizedKey = NormalizePowerSetLookupKey(declaredPowerSetName);
            foreach (var candidate in EnumerateCandidatePowerSets(normalizedBuild, setType))
            {
                if (string.Equals(NormalizePowerSetLookupKey(candidate.DisplayName), normalizedKey, StringComparison.Ordinal) ||
                    string.Equals(NormalizePowerSetLookupKey(candidate.SetName), normalizedKey, StringComparison.Ordinal) ||
                    string.Equals(NormalizePowerSetLookupKey(ExtractLeafName(candidate.FullName)), normalizedKey, StringComparison.Ordinal))
                {
                    return candidate.FullName;
                }
            }

            return fallbackIndex >= 0 &&
                   fallbackIndex < normalizedBuild.PowerSets.Count
                ? normalizedBuild.PowerSets[fallbackIndex]
                : string.Empty;
        }

        private static IEnumerable<IPowerset> EnumerateCandidatePowerSets(CharacterBuildData normalizedBuild, Enums.ePowerSetType setType)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var powerSetName in normalizedBuild.PowerSets)
            {
                var powerSet = DatabaseAPI.GetPowersetByFullname(powerSetName);
                if (powerSet == null || powerSet.SetType != setType || !seen.Add(powerSet.FullName))
                {
                    continue;
                }

                yield return powerSet;
            }

            foreach (var powerEntry in normalizedBuild.PowerEntries)
            {
                if (string.IsNullOrWhiteSpace(powerEntry?.PowerName))
                {
                    continue;
                }

                var powerId = DatabaseAPI.PiDFromUidPower(powerEntry.PowerName);
                if (powerId < 0)
                {
                    continue;
                }

                var powerSet = DatabaseAPI.Database.Power[powerId].GetPowerSet();
                if (powerSet == null || powerSet.SetType != setType || !seen.Add(powerSet.FullName))
                {
                    continue;
                }

                yield return powerSet;
            }
        }

        private static string NormalizePowerSetLookupKey(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return Regex.Replace(value, @"[^A-Za-z0-9]+", string.Empty)
                .Trim()
                .ToUpperInvariant();
        }

        private static string ResolveArchetypeDisplayName(string classUid)
        {
            var classIndex = DatabaseAPI.NidFromUidClass(classUid);
            return classIndex >= 0 && classIndex < DatabaseAPI.Database.Classes.Length
                ? DatabaseAPI.Database.Classes[classIndex]?.DisplayName ?? string.Empty
                : string.Empty;
        }

        private static string ApplyLegacyPowerSetAlias(string sourcePowerSet)
        {
            return sourcePowerSet switch
            {
                "Pool.Leadership_beta" => "Pool.Leadership",
                "Blaster_Support.Atomic_Manipulation" => "Blaster_Support.Radiation_Manipulation",
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

        private static LegacyCompatibilityLoadStatus TryLoadLegacyPlainTextBuild(
            string text,
            string? sourceName,
            string legacyTag,
            IBuildNotifier notifier)
        {
            var parserInput = !string.IsNullOrWhiteSpace(sourceName) && global::System.IO.File.Exists(sourceName)
                ? sourceName
                : text;
            var parser = new global::Mids_Reborn.PlainTextParser(parserInput);
            var recoveredPowers = parser.Parse();
            if (recoveredPowers == null)
            {
                notifier.ShowCompatibilityFailure(new CompatibilityFailureReport
                {
                    Title = "Legacy Build Conversion Failed",
                    SourceName = sourceName ?? string.Empty,
                    Lineage = LegacyLineage.Homecoming,
                    LegacyTag = legacyTag,
                    Summary = "The original Mids build could not be recovered from its plain-text section."
                });
                return LegacyCompatibilityLoadStatus.Failure;
            }

            if (!TryBuildRecoveredPlainTextBuild(
                    parser.GetCharacterInfo(),
                    parser.GetPowersets(),
                    recoveredPowers,
                    legacyTag,
                    out var recoveredBuild,
                    out var buildError))
            {
                notifier.ShowCompatibilityFailure(new CompatibilityFailureReport
                {
                    Title = "Legacy Build Conversion Failed",
                    SourceName = sourceName ?? string.Empty,
                    Lineage = LegacyLineage.Homecoming,
                    LegacyTag = legacyTag,
                    Summary = buildError
                });
                return LegacyCompatibilityLoadStatus.Failure;
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

            if (!TryNormalizeNamedBuild(
                    recoveredBuild,
                    compatibilityMap,
                    sourceName,
                    legacyTag,
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
                    Summary = "The recovered original Mids build could not be converted."
                });
                return LegacyCompatibilityLoadStatus.Failure;
            }

            if (summary != null)
            {
                ApplyLegacyFitnessMigrationFromFlag(
                    HasLegacyFitnessPoolText(text),
                    normalizedBuild,
                    summary);
            }

            normalizedBuild.BuiltWith = new MetaData(
                MidsContext.AppName,
                MidsContext.AppFileVersion,
                DatabaseAPI.DatabaseName,
                DatabaseAPI.Database.Version);

            ApplyDeclaredPowerSetLayoutFromText(text, normalizedBuild);
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

        private static bool TryBuildRecoveredPlainTextBuild(
            RawCharacterInfo characterInfo,
            UniqueList<string> recoveredPowerSets,
            IReadOnlyList<PowerEntry> recoveredPowers,
            string legacyTag,
            out CharacterBuildData recoveredBuild,
            out string error)
        {
            recoveredBuild = new CharacterBuildData();
            error = string.Empty;

            var archetype = DatabaseAPI.GetArchetypeByName(characterInfo.Archetype);
            if (archetype == null)
            {
                error = $"The archetype '{characterInfo.Archetype}' from the recovered original Mids build could not be resolved.";
                return false;
            }

            var resolvedOriginIndex = DatabaseAPI.GetOriginByName(archetype, characterInfo.Origin);
            if (resolvedOriginIndex < 0 || resolvedOriginIndex >= archetype.Origin.Length)
            {
                resolvedOriginIndex = 0;
            }

            var resolvedPowerSets = ResolveRecoveredPowerSets(recoveredPowerSets, characterInfo.Archetype, recoveredPowers);
            var importedLevel = Math.Clamp(characterInfo.Level - 1, 0, Character.MaxLevel);
            var progressionPolicy = DatabaseAPI.GetBuildProgressionPolicy(MidsContext.Config?.DataPath);
            var lastPower = Math.Max(0, progressionPolicy.GetNormalPowerPickCountAtLevel(importedLevel) - 1);

            recoveredBuild = new CharacterBuildData
            {
                BuiltWith = new MetaData(
                    "Mids' Hero Designer",
                    ParseLegacyVersionSafe(legacyTag),
                    DatabaseAPI.DatabaseName,
                    DatabaseAPI.Database.Version),
                Level = characterInfo.Level.ToString(),
                Class = archetype.ClassName,
                Origin = archetype.Origin[resolvedOriginIndex],
                Alignment = characterInfo.Alignment,
                Name = characterInfo.Name ?? string.Empty,
                Comment = string.Empty,
                PowerSets = resolvedPowerSets,
                LastPower = lastPower
            };

            var mainPowers = recoveredPowers
                .Where(IsRecoveredMainPower)
                .Select(CreatePowerDataFromRecoveredPower)
                .ToList();
            var inherentPowers = recoveredPowers
                .Where(IsRecoveredInherentPower)
                .Select(CreatePowerDataFromRecoveredPower)
                .ToList();

            for (var index = 0; index <= lastPower; index++)
            {
                recoveredBuild.PowerEntries.Add(index < mainPowers.Count
                    ? mainPowers[index]
                    : CreateEmptyPowerData());
            }

            foreach (var inherentPower in inherentPowers)
            {
                recoveredBuild.PowerEntries.Add(inherentPower);
            }

            return true;
        }

        private static List<string> ResolveRecoveredPowerSets(
            IEnumerable<string> recoveredPowerSets,
            string archetypeName,
            IReadOnlyList<PowerEntry> recoveredPowers)
        {
            var psFullNames = recoveredPowerSets
                .Where(entry => !string.IsNullOrWhiteSpace(entry) && !entry.StartsWith("Incarnate.Lore_Pet_", StringComparison.OrdinalIgnoreCase))
                .Select(entry => entry.Contains('.')
                    ? entry
                    : DatabaseAPI.GetPowersetByName(entry, archetypeName)?.FullName)
                .Where(entry => !string.IsNullOrWhiteSpace(entry))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var listPowersetsFull = new UniqueList<string>();
            foreach (var powerset in psFullNames)
            {
                listPowersetsFull.Add(powerset);
            }

            var trunkPowersets = listPowersetsFull
                .Select(entry => DatabaseAPI.GetPowersetByFullname(entry) ?? null)
                .Where(entry => entry is { SetType: Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary, nIDTrunkSet: > -1 })
                .Select(entry => DatabaseAPI.Database.Powersets[entry!.nIDTrunkSet].FullName)
                .ToList();

            var listPowersets = new UniqueList<string>();
            foreach (var powerset in listPowersetsFull)
            {
                if (!trunkPowersets.Contains(powerset, StringComparer.OrdinalIgnoreCase))
                {
                    listPowersets.Add(powerset);
                }
            }

            global::Mids_Reborn.ImportBase.FilterVEATPools(ref listPowersets);
            global::Mids_Reborn.ImportBase.FixUndetectedPowersets(ref listPowersets);
            global::Mids_Reborn.ImportBase.FinalizePowersetsList(ref listPowersets, recoveredPowers.ToList(), trunkPowersets);
            global::Mids_Reborn.ImportBase.PadPowerPools(ref listPowersets);
            global::Mids_Reborn.ImportBase.FilterTempPowersets(ref listPowersets);
            global::Mids_Reborn.ImportBase.SortPowersets(ref listPowersets);
            return listPowersets.ToList();
        }

        private static bool IsRecoveredMainPower(PowerEntry powerEntry)
        {
            var power = powerEntry.Power;
            if (power == null)
            {
                return false;
            }

            if (power.FullName.StartsWith("Incarnate.", StringComparison.OrdinalIgnoreCase) ||
                power.FullName.StartsWith("Temporary_Powers.", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return power.InherentType == Enums.eGridType.None;
        }

        private static bool IsRecoveredInherentPower(PowerEntry powerEntry)
        {
            return powerEntry.Power is { InherentType: not Enums.eGridType.None };
        }

        private static PowerData CreatePowerDataFromRecoveredPower(PowerEntry powerEntry)
        {
            var powerData = new PowerData
            {
                PowerName = powerEntry.Power?.FullName ?? string.Empty,
                Level = powerEntry.Level,
                StatInclude = powerEntry.StatInclude,
                ProcInclude = powerEntry.ProcInclude,
                VariableValue = powerEntry.VariableValue,
                InherentSlotsUsed = powerEntry.InherentSlotsUsed
            };

            foreach (var subPowerEntry in powerEntry.SubPowers)
            {
                var subPowerName = subPowerEntry.nIDPower > -1 &&
                                   subPowerEntry.nIDPower < DatabaseAPI.Database.Power.Length
                    ? DatabaseAPI.Database.Power[subPowerEntry.nIDPower].FullName
                    : string.Empty;
                powerData.SubPowerEntries.Add(new SubPowerData
                {
                    PowerName = subPowerName,
                    StatInclude = subPowerEntry.StatInclude
                });
            }

            foreach (var slot in powerEntry.Slots)
            {
                powerData.SlotEntries.Add(new SlotData
                {
                    Level = slot.Level,
                    IsInherent = slot.IsInherent,
                    SlotSource = slot.Source.ToString(),
                    GrantedRuleId = slot.GrantedRuleId,
                    Enhancement = CreateEnhancementDataFromRecoveredSlot(slot.Enhancement),
                    FlippedEnhancement = CreateEnhancementDataFromRecoveredSlot(slot.FlippedEnhancement)
                });
            }

            return powerData;
        }

        private static EnhancementData? CreateEnhancementDataFromRecoveredSlot(I9Slot slot)
        {
            if (slot == null || slot.Enh < 0 || slot.Enh >= DatabaseAPI.Database.Enhancements.Length)
            {
                return null;
            }

            var enhancement = DatabaseAPI.Database.Enhancements[slot.Enh];
            return new EnhancementData
            {
                Uid = enhancement.UID,
                Obtained = slot.Obtained,
                RelativeLevel = slot.RelativeLevel.ToString(),
                Grade = slot.Grade.ToString(),
                IoLevel = slot.IOLevel
            };
        }

        private static void ApplyLegacyFitnessMigrationFromParsedBuild(
            LegacyMxdBuild parsedBuild,
            CharacterBuildData normalizedBuild,
            CompatibilityLoadSummary summary)
        {
            ApplyLegacyFitnessMigrationFromFlag(
                parsedBuild.PowerSets.Any(powerSet => string.Equals(powerSet, "Pool.Fitness", StringComparison.OrdinalIgnoreCase)),
                normalizedBuild,
                summary);
        }

        private static void ApplyLegacyFitnessMigrationFromNamedSource(
            CharacterBuildData sourceBuild,
            CharacterBuildData normalizedBuild,
            CompatibilityLoadSummary summary)
        {
            ApplyLegacyFitnessMigrationFromFlag(
                sourceBuild.PowerSets.Any(powerSet => string.Equals(powerSet, "Pool.Fitness", StringComparison.OrdinalIgnoreCase)),
                normalizedBuild,
                summary);
        }

        private static void ApplyLegacyFitnessMigrationFromFlag(
            bool hadLegacyFitnessPool,
            CharacterBuildData normalizedBuild,
            CompatibilityLoadSummary summary)
        {
            if (!hadLegacyFitnessPool)
            {
                return;
            }

            summary.RemovedLegacyFitnessPool = true;
            for (var powersetIndex = 0; powersetIndex < normalizedBuild.PowerSets.Count; powersetIndex++)
            {
                if (string.Equals(normalizedBuild.PowerSets[powersetIndex], "Pool.Fitness", StringComparison.OrdinalIgnoreCase))
                {
                    normalizedBuild.PowerSets[powersetIndex] = string.Empty;
                }
            }

            var mainEntryCount = Math.Min(
                normalizedBuild.PowerEntries.Count,
                Math.Max(0, normalizedBuild.LastPower + 1));
            var migratedCount = 0;
            for (var index = 0; index < mainEntryCount; index++)
            {
                var powerEntry = normalizedBuild.PowerEntries[index];
                if (powerEntry == null || !IsFitnessPowerUid(powerEntry.PowerName))
                {
                    continue;
                }

                var migratedPower = ClonePowerData(powerEntry);
                migratedPower.Level = ResolveInherentPowerLevel(migratedPower.PowerName, migratedPower.Level);
                normalizedBuild.PowerEntries[index] = CreateEmptyPowerData();
                AppendOrMergeMigratedInherentPower(normalizedBuild, migratedPower);
                migratedCount++;
            }

            if (migratedCount == 0)
            {
                return;
            }

            summary.MigratedFitnessPowerCount += migratedCount;
        }

        private static void AppendOrMergeMigratedInherentPower(CharacterBuildData normalizedBuild, PowerData migratedPower)
        {
            var existingIndex = normalizedBuild.PowerEntries.FindIndex(entry =>
                entry != null &&
                string.Equals(entry.PowerName, migratedPower.PowerName, StringComparison.OrdinalIgnoreCase));
            if (existingIndex < 0)
            {
                normalizedBuild.PowerEntries.Add(migratedPower);
                return;
            }

            var existing = normalizedBuild.PowerEntries[existingIndex];
            if (existing == null || string.IsNullOrWhiteSpace(existing.PowerName))
            {
                normalizedBuild.PowerEntries[existingIndex] = migratedPower;
                return;
            }

            if (existing.SlotEntries.Count < migratedPower.SlotEntries.Count)
            {
                existing.SlotEntries = migratedPower.SlotEntries
                    .Select(CloneSlotData)
                    .ToList();
            }

            existing.Level = ResolveInherentPowerLevel(existing.PowerName, existing.Level > 0 ? existing.Level : migratedPower.Level);
            existing.StatInclude |= migratedPower.StatInclude;
            existing.ProcInclude |= migratedPower.ProcInclude;
            existing.InherentSlotsUsed = Math.Max(existing.InherentSlotsUsed, migratedPower.InherentSlotsUsed);
        }

        private static bool HasLegacyFitnessPoolText(string text)
        {
            return Regex.IsMatch(
                text,
                @"Power\s+Pool\:\s*Fitness",
                RegexOptions.IgnoreCase);
        }

        private static bool IsFitnessPowerUid(string? powerUid)
        {
            return !string.IsNullOrWhiteSpace(powerUid) &&
                   powerUid.StartsWith("Inherent.Fitness.", StringComparison.OrdinalIgnoreCase);
        }

        private static int ResolveInherentPowerLevel(string powerUid, int fallbackLevel)
        {
            var powerId = DatabaseAPI.PiDFromUidPower(powerUid);
            if (powerId < 0)
            {
                return Math.Max(1, fallbackLevel);
            }

            return Math.Max(1, DatabaseAPI.Database.Power[powerId].Level);
        }

        private static PowerData ClonePowerData(PowerData source)
        {
            return new PowerData
            {
                PowerName = source.PowerName,
                Level = source.Level,
                StatInclude = source.StatInclude,
                ProcInclude = source.ProcInclude,
                VariableValue = source.VariableValue,
                InherentSlotsUsed = source.InherentSlotsUsed,
                SubPowerEntries = source.SubPowerEntries
                    .Select(subPower => new SubPowerData
                    {
                        PowerName = subPower.PowerName,
                        StatInclude = subPower.StatInclude
                    })
                    .ToList(),
                SlotEntries = source.SlotEntries
                    .Select(CloneSlotData)
                    .ToList()
            };
        }

        private static SlotData CloneSlotData(SlotData source)
        {
            return new SlotData
            {
                Level = source.Level,
                IsInherent = source.IsInherent,
                SlotSource = source.SlotSource,
                GrantedRuleId = source.GrantedRuleId,
                Enhancement = CloneEnhancementData(source.Enhancement),
                FlippedEnhancement = CloneEnhancementData(source.FlippedEnhancement)
            };
        }

        private static EnhancementData? CloneEnhancementData(EnhancementData? source)
        {
            if (source == null)
            {
                return null;
            }

            return new EnhancementData
            {
                Uid = source.Uid,
                Grade = source.Grade,
                IoLevel = source.IoLevel,
                RelativeLevel = source.RelativeLevel,
                Obtained = source.Obtained,
                LegacyDisplayName = source.LegacyDisplayName
            };
        }

        private static Version ParseLegacyVersionSafe(string? versionText)
        {
            return Version.TryParse(versionText, out var version)
                ? version
                : new Version(1, 0);
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

            if (ShouldIgnoreLegacyPowerStaticIndex(legacyStaticIndex.Value))
            {
                return true;
            }

            var entries = compatibilityMap.GetPowerEntries(legacyStaticIndex.Value);
            return entries.Count > 0 &&
                   entries.All(entry =>
                       ShouldIgnoreLegacyPowerIdentifier(entry.LegacyFullName) ||
                       ShouldIgnoreLegacyPowerIdentifier(entry.LegacyName));
        }

        private static bool ShouldIgnoreLegacyPowerStaticIndex(int legacyStaticIndex)
        {
            return legacyStaticIndex is 11536 or 3257 or 3258 or 3259;
        }

        private static bool ShouldIgnoreLegacyPowerIdentifier(string? legacyFullName)
        {
            if (string.IsNullOrWhiteSpace(legacyFullName))
            {
                return false;
            }

            if (legacyFullName.StartsWith("Mastermind_Summon.", StringComparison.OrdinalIgnoreCase) &&
                ExtractLeafName(legacyFullName).EndsWith("_H", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var normalized = Regex.Replace(legacyFullName.Trim().ToUpperInvariant(), @"[^A-Z0-9]+", string.Empty);
            if (string.Equals(normalized, "INHERENTINHERENTSPECIALSETBONUSES", StringComparison.Ordinal))
            {
                return true;
            }

            if (normalized is
                "INHERENTINHERENTMXDACCOLADESHERO" or
                "INHERENTINHERENTMXDACCOLADESVILLAIN" or
                "INHERENTINHERENTMXDTEMPS")
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

using System.Text.RegularExpressions;

namespace Mids_Reborn.Core.PlannerRulesets;

internal enum SlottingValidationReason
{
    None = 0,
    MissingPower,
    InvalidSlotIndex,
    InvalidEnhancement,
    EnhancementSuppressed,
    EnhancementClassMismatch,
    SetTypeNotAllowed,
    TypedRestrictionBlocked,
    UniqueConflict,
    MutexConflict,
    DuplicateSetPiece
}

internal readonly record struct SlottingValidationResult(
    bool IsValid,
    SlottingValidationReason Reason,
    int ConflictingEnhancementId = -1,
    int ConflictingPowerIndex = -1,
    int ConflictingSlotIndex = -1)
{
    public static SlottingValidationResult Allowed { get; } = new(true, SlottingValidationReason.None);
}

internal sealed class SlottingRulesPolicy
{
    public SlottingValidationResult ValidatePowerEnhancement(IPower? power, int enhancementId)
    {
        if (power == null)
        {
            return new SlottingValidationResult(false, SlottingValidationReason.MissingPower);
        }

        if (!TryGetEnhancement(enhancementId, out var enhancement))
        {
            return new SlottingValidationResult(false, SlottingValidationReason.InvalidEnhancement);
        }

        if (DatabaseAPI.ShouldSuppressImportedEnhancement(enhancement))
        {
            return new SlottingValidationResult(false, SlottingValidationReason.EnhancementSuppressed);
        }

        var allowed = enhancement.TypeID switch
        {
            Enums.eType.SetO => PowerAllowsEnhancementSet(power, enhancement),
            _ => PowerAllowsEnhancementClass(power, enhancement)
        };

        if (!allowed)
        {
            var reason = enhancement.TypeID == Enums.eType.SetO
                ? SlottingValidationReason.SetTypeNotAllowed
                : SlottingValidationReason.EnhancementClassMismatch;
            return new SlottingValidationResult(false, reason);
        }

        if (!TypedEnhancementLegality.AllowsEnhancement(power, enhancement))
        {
            return new SlottingValidationResult(false, SlottingValidationReason.TypedRestrictionBlocked);
        }

        return SlottingValidationResult.Allowed;
    }

    public SlottingValidationResult ValidateBuildSlot(Build? build, int powerIndex, int slotIndex, int enhancementId)
    {
        if (build == null ||
            powerIndex < 0 ||
            powerIndex >= build.Powers.Count ||
            build.Powers[powerIndex]?.Power == null)
        {
            return new SlottingValidationResult(false, SlottingValidationReason.MissingPower);
        }

        var powerEntry = build.Powers[powerIndex]!;
        if (slotIndex < 0 || slotIndex >= powerEntry.Slots.Length)
        {
            return new SlottingValidationResult(false, SlottingValidationReason.InvalidSlotIndex);
        }

        var compatibility = ValidatePowerEnhancement(powerEntry.Power, enhancementId);
        if (!compatibility.IsValid)
        {
            return compatibility;
        }

        var enhancement = DatabaseAPI.Database.Enhancements[enhancementId];

        if (enhancement.Unique &&
            TryFindPrimaryEnhancementConflict(build, powerIndex, slotIndex,
                existingEnhancementId => existingEnhancementId == enhancementId,
                out var uniqueConflict))
        {
            return new SlottingValidationResult(
                false,
                SlottingValidationReason.UniqueConflict,
                uniqueConflict.enhancementId,
                uniqueConflict.powerIndex,
                uniqueConflict.slotIndex);
        }

        if (enhancement.MutExID != Enums.eEnhMutex.None &&
            TryFindPrimaryEnhancementConflict(build, powerIndex, slotIndex,
                existingEnhancementId => IsMutexConflict(enhancement, DatabaseAPI.Database.Enhancements[existingEnhancementId]),
                out var mutexConflict))
        {
            return new SlottingValidationResult(
                false,
                SlottingValidationReason.MutexConflict,
                mutexConflict.enhancementId,
                mutexConflict.powerIndex,
                mutexConflict.slotIndex);
        }

        if (enhancement.nIDSet > -1 &&
            TryFindPrimaryEnhancementConflict(build, powerIndex, slotIndex,
                existingEnhancementId => DatabaseAPI.AreEnhancementsSameSetPiece(existingEnhancementId, enhancementId),
                out var duplicateSetPiece,
                restrictToPowerIndex: powerIndex))
        {
            return new SlottingValidationResult(
                false,
                SlottingValidationReason.DuplicateSetPiece,
                duplicateSetPiece.enhancementId,
                duplicateSetPiece.powerIndex,
                duplicateSetPiece.slotIndex);
        }

        return SlottingValidationResult.Allowed;
    }

    private static bool PowerAllowsEnhancementClass(IPower power, IEnhancement enhancement)
    {
        if (enhancement.ClassID == null || enhancement.ClassID.Length == 0)
        {
            return false;
        }

        var enhancementClasses = DatabaseAPI.Database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>();
        foreach (var classId in enhancement.ClassID)
        {
            if (classId < 0 || classId >= enhancementClasses.Length)
            {
                continue;
            }

            if (power.Enhancements.Contains(enhancementClasses[classId].ID))
            {
                return true;
            }
        }

        return false;
    }

    private static bool PowerAllowsEnhancementSet(IPower power, IEnhancement enhancement)
    {
        if (enhancement.nIDSet < 0 || enhancement.nIDSet >= DatabaseAPI.Database.EnhancementSets.Count)
        {
            return false;
        }

        var setType = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].SetType;
        return power.SetTypes.Any(allowedSetType => allowedSetType == setType);
    }

    private static bool TryGetEnhancement(int enhancementId, out IEnhancement enhancement)
    {
        enhancement = null!;
        if (enhancementId < 0 || enhancementId >= DatabaseAPI.Database.Enhancements.Length)
        {
            return false;
        }

        enhancement = DatabaseAPI.Database.Enhancements[enhancementId];
        return enhancement != null && enhancement.TypeID != Enums.eType.None;
    }

    private static bool TryFindPrimaryEnhancementConflict(
        Build build,
        int targetPowerIndex,
        int targetSlotIndex,
        Func<int, bool> isConflict,
        out (int enhancementId, int powerIndex, int slotIndex) conflict,
        int? restrictToPowerIndex = null)
    {
        for (var powerIndex = 0; powerIndex < build.Powers.Count; powerIndex++)
        {
            if (restrictToPowerIndex.HasValue && powerIndex != restrictToPowerIndex.Value)
            {
                continue;
            }

            var powerEntry = build.Powers[powerIndex];
            if (powerEntry == null)
            {
                continue;
            }

            for (var slotIndex = 0; slotIndex < powerEntry.Slots.Length; slotIndex++)
            {
                if (powerIndex == targetPowerIndex && slotIndex == targetSlotIndex)
                {
                    continue;
                }

                var enhancementId = powerEntry.Slots[slotIndex].Enhancement.Enh;
                if (enhancementId < 0)
                {
                    continue;
                }

                if (isConflict(enhancementId))
                {
                    conflict = (enhancementId, powerIndex, slotIndex);
                    return true;
                }
            }
        }

        conflict = default;
        return false;
    }

    private static bool IsMutexConflict(IEnhancement candidate, IEnhancement existing)
    {
        if (candidate.MutExID == Enums.eEnhMutex.Stealth)
        {
            return existing.MutExID == Enums.eEnhMutex.Stealth;
        }

        var mutexVersion = NormalizeSuperiorMutexVersion(candidate.UID);
        if (string.IsNullOrWhiteSpace(mutexVersion))
        {
            return false;
        }

        var existingUid = existing.UID ?? string.Empty;
        return candidate.Superior
            ? existingUid.Contains(mutexVersion, StringComparison.OrdinalIgnoreCase)
            : existingUid.Contains($"Superior_Attuned_{mutexVersion}", StringComparison.OrdinalIgnoreCase) ||
              existingUid.Contains($"Superior_Attuned_Superior_{mutexVersion}", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeSuperiorMutexVersion(string? uid)
    {
        return Regex.Replace(uid ?? string.Empty, @"(Attuned_|Superior_)", string.Empty);
    }
}

using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

internal static class CompositePowersetRules
{
    private static readonly Dictionary<string, string> TrunkPowersets = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Widow_Training.Fortunata_Training"] = "Widow_Training.Widow_Training",
        ["Widow_Training.Night_Widow_Training"] = "Widow_Training.Widow_Training",
        ["Teamwork.Fortunata_Teamwork"] = "Teamwork.Teamwork",
        ["Teamwork.Widow_Teamwork"] = "Teamwork.Teamwork",
        ["Training_Gadgets.Bane_Spider_Training"] = "Training_Gadgets.Training_and_Gadgets",
        ["Training_Gadgets.Crab_Spider_Training"] = "Training_Gadgets.Training_and_Gadgets"
    };

    private static readonly HashSet<string> ForcedVisiblePowerFullNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Teamwork.Fortunata_Teamwork.Fate_Sealed",
        "Teamwork.Fortunata_Teamwork.FRT_Fate_Sealed",
        "Teamwork.Widow_Teamwork.Pain_Tolerance",
        "Teamwork.Widow_Teamwork.NW_Pain_Tolerance"
    };

    public static string GetTrunkPowersetFullName(string powersetFullName)
    {
        if (string.IsNullOrWhiteSpace(powersetFullName))
        {
            return string.Empty;
        }

        return TrunkPowersets.TryGetValue(powersetFullName, out var trunkFullName)
            ? trunkFullName
            : string.Empty;
    }

    public static bool ShouldForceVisibleOnImport(string powerFullName)
    {
        return !string.IsNullOrWhiteSpace(powerFullName) &&
               ForcedVisiblePowerFullNames.Contains(powerFullName);
    }

    public static IPowerset[] GetDisplaySections(IPowerset? selectedPowerset)
    {
        if (selectedPowerset == null)
        {
            return [];
        }

        if (selectedPowerset.nIDTrunkSet < 0 ||
            selectedPowerset.nIDTrunkSet >= DatabaseAPI.Database.Powersets.Length)
        {
            return [selectedPowerset];
        }

        var trunkPowerset = DatabaseAPI.Database.Powersets[selectedPowerset.nIDTrunkSet];
        return trunkPowerset == null
            ? [selectedPowerset]
            : [trunkPowerset, selectedPowerset];
    }

    public static string FormatDisplaySectionHeading(string displayName)
    {
        var trimmedName = string.IsNullOrWhiteSpace(displayName) ? string.Empty : displayName.Trim();
        return $"–{trimmedName}–";
    }
}

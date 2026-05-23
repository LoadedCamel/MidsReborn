using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

internal sealed class PetActorExternalPowerSource
{
    public required string SourceName { get; init; }
    public required string SourceFullName { get; init; }
    public required IReadOnlyList<IPower> Powers { get; init; }
}

internal static class PetActorExternalPowerResolver
{
    private const string SupremacyPowerFullName = "Inherent.Inherent.Supremacy";

    public static IReadOnlyList<PetActorExternalPowerSource> GetOwnerExternalSources(
        Toon toon,
        RealPetActorRosterItem rosterItem,
        PlannerBuildRecipientContext recipient,
        IReadOnlyList<PetUpgradeOverlay> availableUpgrades,
        PetActorPreviewState previewState)
    {
        var excludedUpgradePowerNames = new HashSet<string>(
            availableUpgrades.Select(overlay => overlay.UpgradePowerFullName),
            StringComparer.OrdinalIgnoreCase);

        return toon.CurrentBuild.Powers
            .Select((powerEntry, historyIndex) => new { powerEntry, historyIndex })
            .Where(pair => pair.powerEntry?.Power != null)
            .Where(pair => pair.historyIndex != rosterItem.SourceHistoryIndex)
            .Select(pair => pair.powerEntry!.Power!)
            .Where(power => !excludedUpgradePowerNames.Contains(power.FullName))
            .Where(power => previewState.InRange ||
                            !power.FullName.Equals(SupremacyPowerFullName, StringComparison.OrdinalIgnoreCase))
            .Select(power => new
            {
                Power = power,
                RoutedPower = OmniPowerRouting.CreatePlannerPower(power, recipient)
            })
            .Where(pair => pair.RoutedPower != null && pair.RoutedPower.Effects.Length > 0)
            .Where(pair => pair.RoutedPower!.Effects.Any(effect =>
                effect.EffectType is not Enums.eEffectType.EntCreate and
                    not Enums.eEffectType.GrantPower and
                    not Enums.eEffectType.ExecutePower))
            .Select(pair => new PetActorExternalPowerSource
            {
                SourceName = pair.Power.DisplayName,
                SourceFullName = pair.Power.FullName,
                Powers = new[] { pair.RoutedPower! }
            })
            .ToArray();
    }
}

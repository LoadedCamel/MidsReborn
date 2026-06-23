using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core;

public sealed class PetActorPowerTileViewModel
{
    public int PowerIndex { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string PowerKindLabel { get; init; } = string.Empty;
    public string? UpgradeRequiredDisplayName { get; init; }
    public string SourceDescription { get; init; } = string.Empty;
    public bool IsAuto { get; init; }
    public bool IsSelected { get; init; }
    public bool CanTogglePreview { get; init; }
    public bool IsPreviewIncluded { get; init; }
    public IPower BasePower { get; init; } = null!;
}

public sealed class PetActorPowerSectionViewModel
{
    public string Title { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public IReadOnlyList<PetActorPowerTileViewModel> Tiles { get; init; } = [];
}

public sealed class PetActorPowerGridViewModel
{
    public IReadOnlyList<PetActorPowerSectionViewModel> Sections { get; init; } = [];
    public int SelectedPowerIndex { get; init; } = -1;

    public bool HasPowers => Sections.Any(section => section.Tiles.Count > 0);
}

public static class PetActorSheetViewModelBuilder
{
    public static PetActorPowerGridViewModel Build(PetActorSnapshot? snapshot, int selectedPowerIndex)
    {
        if (snapshot == null || snapshot.ResolvedPowers.Count == 0)
        {
            return new PetActorPowerGridViewModel();
        }

        var upgradeLookup = snapshot.ResolvedPowers
            .Where(power => !string.IsNullOrWhiteSpace(power.RequiredUpgradePowerFullName))
            .GroupBy(power => power.Power.FullName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => DatabaseAPI.GetPowerByFullName(group.First().RequiredUpgradePowerFullName!)?.DisplayName,
                StringComparer.OrdinalIgnoreCase);

        var sections = snapshot.ResolvedPowers
            .Select((resolvedPower, index) =>
            {
                upgradeLookup.TryGetValue(resolvedPower.Power.FullName, out var requiredUpgrade);
                return new
                {
                    Power = resolvedPower,
                    Index = index,
                    RequiredUpgrade = requiredUpgrade
                };
            })
            .Where(entry => entry.Power.VisibleInGrid)
            .GroupBy(entry => new SectionKey(
                    GetPowerGroupTitle(entry.Power.Power),
                    string.IsNullOrWhiteSpace(entry.RequiredUpgrade)
                        ? null
                        : $"Unlocked by {entry.RequiredUpgrade}"),
                SectionKeyComparer.Instance)
            .Select(group => new PetActorPowerSectionViewModel
            {
                Title = group.Key.Title,
                Subtitle = group.Key.Subtitle,
                Tiles = group
                    .Select(entry =>
                    {
                        var canTogglePreview = entry.Power.VisibleInGrid &&
                                               entry.Power.Power.PowerType == Enums.ePowerType.Click &&
                                               entry.Power.Power.ClickBuff;
                        return new PetActorPowerTileViewModel
                        {
                            PowerIndex = entry.Index,
                            DisplayName = entry.Power.Power.DisplayName,
                            FullName = entry.Power.Power.FullName,
                            PowerKindLabel = GetPowerKindLabel(entry.Power.Power),
                            UpgradeRequiredDisplayName = entry.RequiredUpgrade,
                            SourceDescription = PetActorPowerResolver.DescribePowerSource(entry.Power),
                            IsAuto = entry.Power.Power.PowerType == Enums.ePowerType.Auto_,
                            IsSelected = entry.Index == selectedPowerIndex,
                            CanTogglePreview = canTogglePreview,
                            IsPreviewIncluded = canTogglePreview &&
                                                snapshot.PreviewState.IsPetClickBuffIncluded(entry.Power.Power.FullName),
                            BasePower = entry.Power.Power
                        };
                    })
                    .ToArray()
            })
            .ToArray();

        return new PetActorPowerGridViewModel
        {
            Sections = sections,
            SelectedPowerIndex = selectedPowerIndex
        };
    }

    private sealed record SectionKey(string Title, string? Subtitle);

    private sealed class SectionKeyComparer : IEqualityComparer<SectionKey>
    {
        public static SectionKeyComparer Instance { get; } = new();

        public bool Equals(SectionKey? x, SectionKey? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return string.Equals(x.Title, y.Title, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(x.Subtitle, y.Subtitle, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(SectionKey obj)
        {
            return HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Title),
                obj.Subtitle == null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Subtitle));
        }
    }

    private static string GetPowerGroupTitle(IPower power)
    {
        return power.GetPowerSet()?.DisplayName
               ?? (!string.IsNullOrWhiteSpace(power.SetName) ? FormatTokenizedName(power.SetName) : "Powers");
    }

    private static string GetPowerKindLabel(IPower power)
    {
        return power.PowerType switch
        {
            Enums.ePowerType.Auto_ => "Auto",
            Enums.ePowerType.Toggle => "Toggle",
            _ => string.Empty
        };
    }

    private static string FormatTokenizedName(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Replace('_', ' ').Trim();
    }
}

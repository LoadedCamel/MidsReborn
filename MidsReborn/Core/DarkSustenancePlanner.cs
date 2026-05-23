using System;
using System.Collections.Generic;
using System.Linq;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

internal static class DarkSustenancePlanner
{
    internal const string DarkSustenancePowerFullName = "Inherent.Inherent.Dark_Sustenance";

    private const int MaxContributorsPerCategory = 2;
    private const float DamageMagnitudePerContributor = 0.2f;
    private const float ResistanceMagnitudePerContributor = 0.1f;
    private const float MezProtectionPerContributor = 1f;
    private const float MezResistancePerContributor = 0.1f;
    private const float RechargeSlowResistancePerContributor = 0.1f;

    private static readonly HashSet<string> HomecomingDamageArchetypes = CreateArchetypeSet(
        "Class_Scrapper",
        "Class_Blaster",
        "Class_Sentinel",
        "Class_Brute",
        "Class_Stalker");

    private static readonly HashSet<string> HomecomingResistanceArchetypes = CreateArchetypeSet(
        "Class_Tanker",
        "Class_Defender",
        "Class_Mastermind",
        "Class_Corruptor");

    private static readonly HashSet<string> RebirthDamageArchetypes = CreateArchetypeSet(
        "Class_Scrapper",
        "Class_Blaster",
        "Class_Brute",
        "Class_Stalker");

    private static readonly HashSet<string> RebirthResistanceArchetypes = CreateArchetypeSet(
        "Class_Tanker",
        "Class_Defender",
        "Class_Mastermind",
        "Class_Guardian",
        "Class_Corruptor");

    private static readonly HashSet<string> ControlArchetypes = CreateArchetypeSet(
        "Class_Controller",
        "Class_Dominator");

    private static readonly HashSet<string> RechargeArchetypes = CreateArchetypeSet(
        "Class_Peacebringer",
        "Class_Warshade",
        "Class_Arachnos_Soldier",
        "Class_Arachnos_Widow");

    private static readonly Enums.eDamage[] ComputedDamageTypes =
    [
        Enums.eDamage.Smashing,
        Enums.eDamage.Negative
    ];

    private static readonly Enums.eDamage[] ComputedResistanceTypes =
    [
        Enums.eDamage.Smashing,
        Enums.eDamage.Lethal,
        Enums.eDamage.Fire,
        Enums.eDamage.Cold,
        Enums.eDamage.Energy,
        Enums.eDamage.Negative,
        Enums.eDamage.Toxic,
        Enums.eDamage.Psionic
    ];

    private static readonly Enums.eMez[] ComputedMezTypes =
    [
        Enums.eMez.Confused,
        Enums.eMez.Terrorized,
        Enums.eMez.Held,
        Enums.eMez.Immobilized,
        Enums.eMez.Stunned,
        Enums.eMez.Sleep
    ];

    internal static IReadOnlyList<Enums.eDamage> DamageTypes => ComputedDamageTypes;
    internal static IReadOnlyList<Enums.eDamage> ResistanceTypes => ComputedResistanceTypes;
    internal static IReadOnlyList<Enums.eMez> MezTypes => ComputedMezTypes;

    public static bool SupportsCurrentContext()
    {
        var archetype = MidsContext.Character?.Archetype ?? MidsContext.Archetype;
        if (archetype == null)
        {
            return false;
        }

        return archetype.DisplayName.Equals("Warshade", StringComparison.OrdinalIgnoreCase) ||
               archetype.ClassName.Equals("Class_Warshade", StringComparison.OrdinalIgnoreCase) ||
               archetype.ClassName.Equals("Class_Shade", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsManagedComputedPower(IPower? power)
    {
        return power != null &&
               !string.IsNullOrWhiteSpace(power.FullName) &&
               power.FullName.Equals(DarkSustenancePowerFullName, StringComparison.OrdinalIgnoreCase);
    }

    public static CosmicBalanceComputedState GetComputedState(ConfigData? config)
    {
        if (!SupportsCurrentContext() || config == null)
        {
            return new CosmicBalanceComputedState();
        }

        var counts = CombatContextState.NormalizeTeamMembers(config.TeamMembers);
        var providerId = DatabaseAPI.GetServerRulesProfile().DataProviderId;
        var damageArchetypes = providerId == OmniDataProviderId.OmniRebirth
            ? RebirthDamageArchetypes
            : HomecomingDamageArchetypes;
        var resistanceArchetypes = providerId == OmniDataProviderId.OmniRebirth
            ? RebirthResistanceArchetypes
            : HomecomingResistanceArchetypes;
        var pvpContext = MidsContext.Config?.Inc.DisablePvE == true;

        var damageCount = CountContributors(counts, damageArchetypes);
        var resistanceCount = CountContributors(counts, resistanceArchetypes);
        var controlCount = CountContributors(counts, ControlArchetypes);
        var rechargeCount = CountContributors(counts, RechargeArchetypes);

        return new CosmicBalanceComputedState
        {
            DamageMagnitude = damageCount * DamageMagnitudePerContributor,
            ResistanceMagnitude = resistanceCount * ResistanceMagnitudePerContributor,
            MezProtectionMagnitude = pvpContext ? 0f : controlCount * MezProtectionPerContributor,
            MezResistanceMagnitude = pvpContext ? controlCount * MezResistancePerContributor : 0f,
            RechargeSlowResistanceMagnitude = rechargeCount * RechargeSlowResistancePerContributor
        };
    }

    private static int CountContributors(
        IReadOnlyDictionary<string, int> counts,
        IReadOnlySet<string> supportedArchetypes)
    {
        if (counts.Count == 0 || supportedArchetypes.Count == 0)
        {
            return 0;
        }

        var total = 0;
        foreach (var (archetype, count) in counts)
        {
            if (!supportedArchetypes.Contains(archetype))
            {
                continue;
            }

            total += Math.Max(0, count);
            if (total >= MaxContributorsPerCategory)
            {
                return MaxContributorsPerCategory;
            }
        }

        return Math.Min(total, MaxContributorsPerCategory);
    }

    private static HashSet<string> CreateArchetypeSet(params string[] classNames)
    {
        return classNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}

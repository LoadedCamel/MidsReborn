using System;
using System.Linq;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

internal static class VigilancePlanner
{
    internal const string VigilancePowerFullName = "Inherent.Inherent.Vigilance";
    internal const string VigilanceEndAdjustmentPowerFullName = "Inherent.Inherent.Vigilance_PerTeamEndAdjustment";

    private const float BaseDamageOffset = 0.4f;
    private const float DamageReductionPerMember = 0.1f;
    private const float MaxDamageMagnitude = 0.3f;
    private const float RebirthBaseTeamEndDiscountPerTeammate = 0.1f;
    private const int RebirthBaseTeamEndDiscountMaxTeammates = 3;

    public static bool SupportsCurrentContext()
    {
        var archetype = MidsContext.Character?.Archetype ?? MidsContext.Archetype;
        if (archetype == null)
        {
            return false;
        }

        return archetype.DisplayName.Equals("Defender", StringComparison.OrdinalIgnoreCase) ||
               archetype.ClassName.Equals("Class_Defender", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsManagedComputedPower(IPower? power)
    {
        if (power == null || string.IsNullOrWhiteSpace(power.FullName))
        {
            return false;
        }

        return power.FullName.Equals(VigilancePowerFullName, StringComparison.OrdinalIgnoreCase) ||
               power.FullName.Equals(VigilanceEndAdjustmentPowerFullName, StringComparison.OrdinalIgnoreCase);
    }

    public static float GetComputedDamageBuffMagnitude(ConfigData? config)
    {
        if (!SupportsCurrentContext() || config == null)
        {
            return 0f;
        }

        var teamSize = Math.Max(1, config.TeamSize);
        return Math.Clamp(BaseDamageOffset - DamageReductionPerMember * teamSize, 0f, MaxDamageMagnitude);
    }

    public static float GetComputedEnduranceDiscountMagnitude(ConfigData? config)
    {
        if (!SupportsCurrentContext() || config == null)
        {
            return 0f;
        }

        var missingHealthContribution = GetPerTeammateMissingHealthContribution(config);
        var baseTeamContribution = GetBaseTeamContribution(config.TeamSize);
        return Math.Max(0f, missingHealthContribution + baseTeamContribution);
    }

    private static float GetPerTeammateMissingHealthContribution(ConfigData config)
    {
        if (config.TeamRoster is not { Count: > 0 } roster)
        {
            return 0f;
        }

        var scale = DatabaseAPI.GetServerRulesProfile().DataProviderId switch
        {
            OmniDataProviderId.OmniRebirth => 0.55f,
            _ => 0.75f
        };

        var total = 0f;
        foreach (var slot in roster)
        {
            if (!slot.InRange || string.IsNullOrWhiteSpace(slot.Archetype))
            {
                continue;
            }

            var clampedHpPercent = Math.Clamp(slot.HpPercent, 0, 100);
            total += (100f - clampedHpPercent) / 100f * scale;
        }

        return total;
    }

    private static float GetBaseTeamContribution(int fullTeamSizeIncludingSelf)
    {
        if (DatabaseAPI.GetServerRulesProfile().DataProviderId != OmniDataProviderId.OmniRebirth)
        {
            return 0f;
        }

        var teammateCount = Math.Max(0, fullTeamSizeIncludingSelf - 1);
        return Math.Min(teammateCount, RebirthBaseTeamEndDiscountMaxTeammates) * RebirthBaseTeamEndDiscountPerTeammate;
    }
}

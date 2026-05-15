using System;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

internal static class PetActorBonusAnalyzer
{
    public static IReadOnlyList<PetAppliedBonusEntry> Build(
        Toon toon,
        RealPetActorRosterItem rosterItem,
        PlannerBuildRecipientContext recipient,
        ActorTotalsSnapshot finalTotals,
        IReadOnlyList<ResolvedPetPower> resolvedPowers,
        IReadOnlyList<PetUpgradeOverlay> availableUpgrades,
        PetActorPreviewState previewState,
        IReadOnlyList<IPower> allMathPowers,
        IReadOnlyList<IPower> allBuffedPowers)
    {
        var entries = new List<PetAppliedBonusEntry>();
        var includedIndexes = resolvedPowers
            .Select((resolvedPower, index) => new { resolvedPower, index })
            .Where(entry => PetActorPowerResolver.ShouldIncludeInTotals(entry.resolvedPower, previewState))
            .Select(entry => entry.index)
            .ToArray();

        var setBonusPower = toon.CurrentBuild.GetSetBonusVirtualPower(recipient);

        foreach (var overlay in availableUpgrades.Where(overlay => previewState.IsUpgradeApplied(overlay.UpgradePowerFullName)))
        {
            var previewWithoutOverlay = previewState.Clone();
            previewWithoutOverlay.Upgrades.AppliedUpgradePowerFullNames.Remove(overlay.UpgradePowerFullName);
            var withoutOverlay = toon.GeneratePetActorSnapshotForAnalysis(rosterItem, previewWithoutOverlay)?.Totals
                                 ?? CalculateSubsetTotals(recipient.ClassName, includedIndexes, allMathPowers, allBuffedPowers, setBonusPower);
            var entry = CreateEntry(
                $"Upgrade: {overlay.UpgradePowerDisplayName}",
                overlay.UpgradePowerFullName,
                PetAppliedBonusSourceType.UpgradeOverlay,
                finalTotals,
                withoutOverlay);
            if (entry != null)
            {
                entries.Add(entry);
            }
        }

        foreach (var pair in resolvedPowers.Select((resolvedPower, index) => new { resolvedPower, index }))
        {
            if (!includedIndexes.Contains(pair.index))
            {
                continue;
            }

            if (pair.resolvedPower.GrantKind == PetGrantKind.Baseline &&
                pair.resolvedPower.Power.PowerType is not (Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle) &&
                !pair.resolvedPower.IsSelfClickBuff)
            {
                continue;
            }

            PetAppliedBonusSourceType? sourceType = pair.resolvedPower.Power.PowerType switch
            {
                Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle => PetAppliedBonusSourceType.PetAutoToggle,
                Enums.ePowerType.Click when pair.resolvedPower.IsSelfClickBuff => PetAppliedBonusSourceType.PetClickBuff,
                _ => null
            };

            if (sourceType == null)
            {
                continue;
            }

            var withoutPower = sourceType == PetAppliedBonusSourceType.PetClickBuff
                ? toon.GeneratePetActorSnapshotForAnalysis(
                    rosterItem,
                    CreatePreviewStateWithoutClickBuff(previewState, pair.resolvedPower.Power.FullName))?.Totals
                  ?? CalculateSubsetTotals(recipient.ClassName, includedIndexes.Where(index => index != pair.index), allMathPowers, allBuffedPowers, setBonusPower)
                : CalculateSubsetTotals(recipient.ClassName, includedIndexes.Where(index => index != pair.index), allMathPowers, allBuffedPowers, setBonusPower);
            var entry = CreateEntry(
                $"{pair.resolvedPower.Power.DisplayName}",
                pair.resolvedPower.Power.FullName,
                sourceType.Value,
                finalTotals,
                withoutPower);
            if (entry != null)
            {
                entries.Add(entry);
            }
        }

        var directExternalBaseline = PetActorMath.Calculate(recipient.ClassName, Array.Empty<IPower>(), Array.Empty<IPower>(), Array.Empty<IPower>());

        foreach (var setBonusSource in toon.CurrentBuild.GetSetBonusPowers(recipient)
                     .GroupBy(power => power.FullName, StringComparer.OrdinalIgnoreCase)
                     .Select(group => new
                     {
                         SourceName = $"Set Bonus: {group.First().DisplayName}",
                         SourceFullName = group.Key,
                         Powers = group.ToArray()
                     }))
        {
            var withSource = PetActorMath.Calculate(recipient.ClassName, Array.Empty<IPower>(), Array.Empty<IPower>(), setBonusSource.Powers);
            var entry = CreateEntry(setBonusSource.SourceName, setBonusSource.SourceFullName, PetAppliedBonusSourceType.SetBonus, withSource, directExternalBaseline);
            if (entry != null)
            {
                entries.Add(entry);
            }
        }

        foreach (var ownerSource in GetOwnerExternalSources(toon, rosterItem, recipient, availableUpgrades, previewState))
        {
            var withSource = PetActorMath.Calculate(recipient.ClassName, Array.Empty<IPower>(), Array.Empty<IPower>(), ownerSource.Powers);
            var entry = CreateEntry(ownerSource.SourceName, ownerSource.SourceFullName, PetAppliedBonusSourceType.OwnerAuraOrBuff, withSource, directExternalBaseline);
            if (entry != null)
            {
                entries.Add(entry);
            }
        }

        return entries
            .OrderBy(entry => entry.SourceType)
            .ThenBy(entry => entry.SourceName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static PetActorPreviewState CreatePreviewStateWithoutClickBuff(PetActorPreviewState previewState, string powerFullName)
    {
        var clone = previewState.Clone();
        clone.ClickBuffs.IncludedPetSelfClickFullNames.Remove(powerFullName);
        return clone;
    }

    private static ActorTotalsSnapshot CalculateSubsetTotals(
        string className,
        IEnumerable<int> includedIndexes,
        IReadOnlyList<IPower> allMathPowers,
        IReadOnlyList<IPower> allBuffedPowers,
        IPower setBonusPower)
    {
        var selectedIndexes = includedIndexes.ToArray();
        var mathPowers = selectedIndexes.Select(index => allMathPowers[index]).ToArray();
        var buffedPowers = selectedIndexes.Select(index => allBuffedPowers[index]).ToArray();
        return PetActorMath.Calculate(className, mathPowers, buffedPowers, setBonusPower);
    }

    private static IEnumerable<(string SourceName, string SourceFullName, IReadOnlyList<IPower> Powers)> GetOwnerExternalSources(
        Toon toon,
        RealPetActorRosterItem rosterItem,
        PlannerBuildRecipientContext recipient,
        IReadOnlyList<PetUpgradeOverlay> availableUpgrades,
        PetActorPreviewState previewState)
    {
        var excludedUpgradePowerNames = new HashSet<string>(
            availableUpgrades.Select(overlay => overlay.UpgradePowerFullName),
            StringComparer.OrdinalIgnoreCase);

        foreach (var entry in toon.CurrentBuild.Powers
                     .Select((powerEntry, historyIndex) => new { powerEntry, historyIndex })
                     .Where(pair => pair.powerEntry?.Power != null))
        {
            if (entry.historyIndex == rosterItem.SourceHistoryIndex)
            {
                continue;
            }

            var power = entry.powerEntry!.Power!;
            if (excludedUpgradePowerNames.Contains(power.FullName))
            {
                continue;
            }

            if (!previewState.InRange &&
                power.FullName.Equals("Inherent.Inherent.Supremacy", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var routedPower = OmniPowerRouting.CreatePlannerPower(power, recipient);
            if (routedPower == null || routedPower.Effects.Length == 0)
            {
                continue;
            }

            if (routedPower.Effects.All(effect =>
                    effect.EffectType is Enums.eEffectType.EntCreate or Enums.eEffectType.GrantPower or Enums.eEffectType.ExecutePower))
            {
                continue;
            }

            yield return (power.DisplayName, power.FullName, new[] { routedPower });
        }
    }

    private static PetAppliedBonusEntry? CreateEntry(
        string sourceName,
        string sourceFullName,
        PetAppliedBonusSourceType sourceType,
        ActorTotalsSnapshot withSource,
        ActorTotalsSnapshot withoutSource)
    {
        var deltas = BuildDeltaList(withSource, withoutSource)
            .Where(delta => Math.Abs(delta.Delta) > 0.01f)
            .ToArray();
        if (deltas.Length == 0)
        {
            return null;
        }

        var summary = string.Join(", ", deltas.Take(3).Select(delta => delta.ToDisplayString()));
        var tooltip = string.Join("\r\n", deltas.Select(delta => delta.ToDisplayString()));

        return new PetAppliedBonusEntry
        {
            SourceName = sourceName,
            SourceFullName = sourceFullName,
            SourceType = sourceType,
            StatDeltas = deltas,
            Summary = summary,
            Tooltip = tooltip
        };
    }

    private static IReadOnlyList<PetAppliedBonusStatDelta> BuildDeltaList(ActorTotalsSnapshot withSource, ActorTotalsSnapshot withoutSource)
    {
        var deltas = new List<PetAppliedBonusStatDelta>();
        void Add(string name, float value, string suffix)
        {
            if (Math.Abs(value) <= 0.01f)
            {
                return;
            }

            deltas.Add(new PetAppliedBonusStatDelta
            {
                StatName = name,
                Delta = value,
                Suffix = suffix
            });
        }

        var withDisplay = withSource.DisplayStats;
        var withoutDisplay = withoutSource.DisplayStats;

        Add("Hit Points", withDisplay.HealthHitpointsNumeric(false) - withoutDisplay.HealthHitpointsNumeric(false), string.Empty);
        Add("Regen", withDisplay.HealthRegenHPPerSec(false) - withoutDisplay.HealthRegenHPPerSec(false), " HP/s");
        Add("Recovery", withDisplay.GetEnduranceRecoveryNumeric(false) - withoutDisplay.GetEnduranceRecoveryNumeric(false), " End/s");
        Add("Absorb", withSource.Totals.Absorb - withoutSource.Totals.Absorb, string.Empty);
        Add("ToHit", withDisplay.BuffToHit - withoutDisplay.BuffToHit, "%");
        Add("Accuracy", withDisplay.BuffAccuracy - withoutDisplay.BuffAccuracy, "%");
        Add("Damage", (withSource.Totals.BuffDam - withoutSource.Totals.BuffDam) * 100f, "%");
        Add("EndRdx", withDisplay.BuffEndRdx - withoutDisplay.BuffEndRdx, "%");
        Add("Recharge", (withSource.Totals.BuffHaste - withoutSource.Totals.BuffHaste) * 100f, "%");
        Add("Range", withDisplay.RangePercent - withoutDisplay.RangePercent, "%");
        Add("Threat", withDisplay.ThreatLevel - withoutDisplay.ThreatLevel, "%");

        foreach (var damageType in new[]
                 {
                     (Label: "Smashing", DamageType: Enums.eDamage.Smashing),
                     (Label: "Lethal", DamageType: Enums.eDamage.Lethal),
                     (Label: "Fire", DamageType: Enums.eDamage.Fire),
                     (Label: "Cold", DamageType: Enums.eDamage.Cold),
                     (Label: "Energy", DamageType: Enums.eDamage.Energy),
                     (Label: "Negative", DamageType: Enums.eDamage.Negative),
                     (Label: "Toxic", DamageType: Enums.eDamage.Toxic),
                     (Label: "Psionic", DamageType: Enums.eDamage.Psionic)
                 })
        {
            Add($"{damageType.Label} Def", withDisplay.Defense((int)damageType.DamageType) - withoutDisplay.Defense((int)damageType.DamageType), "%");
            Add($"{damageType.Label} Res", withDisplay.DamageResistance((int)damageType.DamageType, false) - withoutDisplay.DamageResistance((int)damageType.DamageType, false), "%");
        }

        Add("Run Speed", withSource.Totals.RunSpd - withoutSource.Totals.RunSpd, " ft/s");
        Add("Fly Speed", withSource.Totals.FlySpd - withoutSource.Totals.FlySpd, " ft/s");
        Add("Jump Speed", withSource.Totals.JumpSpd - withoutSource.Totals.JumpSpd, " ft/s");
        Add("Jump Height", withSource.Totals.JumpHeight - withoutSource.Totals.JumpHeight, " ft");
        Add("Perception", withSource.Totals.Perception - withoutSource.Totals.Perception, " ft");

        return deltas;
    }
}

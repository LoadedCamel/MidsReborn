using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core.Omni;

public sealed partial class OmniImporter
{
    private const string VigilanceEndAdjustmentPowerFullName = "Inherent.Inherent.Vigilance_PerTeamEndAdjustment";

    private static void RemodelPlannerStateFamilies(IDatabase database, OmniApplyResult applyResult)
    {
        database.MutexList = PlannerStateCatalog.MergePlannerMutexGroups(
            database.MutexList ?? DatabaseAPI.Database?.MutexList ?? []);

        var powers = database.Power ?? [];
        var powersByName = powers
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => power!.FullName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First()!, StringComparer.OrdinalIgnoreCase);
        var nextStaticIndex = powers
            .Where(power => power != null)
            .Select(power => power!.StaticIndex)
            .DefaultIfEmpty(-2)
            .Max() + 1;

        foreach (var power in powers.Where(power => power != null))
        {
            RewritePlannerStatePower(database, power!, applyResult);
        }

        foreach (var definition in PlannerStateCatalog.Definitions)
        {
            EnsurePlannerStateControl(database, definition, ref powers, powersByName, ref nextStaticIndex, applyResult);
        }

        database.Power = powers;
    }

    private static void RemodelTierOneArchetypeInherents(IDatabase database, OmniApplyResult applyResult)
    {
        foreach (var power in database.Power?.Where(power => power != null) ?? [])
        {
            var changed = false;
            var strippedRows = 0;
            var strippedEffects = 0;

            if (StripTierOneRuntimeRows(power!.AdvancedRequirements, power.FullName, out var rewrittenRequirements, out var removedRequirementRows))
            {
                power.AdvancedRequirements = rewrittenRequirements;
                power.Requires = rewrittenRequirements.ToLegacyRequirement();
                strippedRows += removedRequirementRows;
                changed = true;
            }

            if (ArchetypeInherentCatalog.TryAppendTierOnePowerRequirements(power, power.AdvancedRequirements))
            {
                power.Requires = power.AdvancedRequirements.ToLegacyRequirement();
                changed = true;
            }

            var rewrittenEffects = new List<IEffect>(power.Effects?.Length ?? 0);
            foreach (var effect in power.Effects ?? [])
            {
                if (effect == null)
                {
                    continue;
                }

                if (!StripTierOneRuntimeRows(effect.AdvancedConditions, power.FullName, out var rewrittenConditions, out var removedEffectRows))
                {
                    rewrittenEffects.Add(effect);
                    continue;
                }

                strippedRows += removedEffectRows;
                changed = true;
                if (rewrittenConditions.Rows.Count == 0 && effect.AdvancedConditions.Rows.Count > 0)
                {
                    strippedEffects++;
                    applyResult.AddLimited(
                        applyResult.ImportIntegrityAuditDetails,
                        $"{power.FullName}: removed an active effect branch because tier-1 runtime conditions remained unresolved ({effect.AdvancedConditions.Rows[0].RawExpression}).");
                    continue;
                }

                effect.AdvancedConditions = rewrittenConditions;
                effect.NormalizeConditionState();
                rewrittenEffects.Add(effect);
            }

            if (!changed)
            {
                continue;
            }

            power.Effects = rewrittenEffects.ToArray();
            power.IsModified = true;
            applyResult.AddLimited(
                applyResult.ImportIntegrityAuditDetails,
                $"{power.FullName}: tier-1 inherent remodel stripped {strippedRows} unresolved runtime row(s) and removed {strippedEffects} effect branch(es).");
        }
    }

    private static bool StripTierOneRuntimeRows(
        AdvancedConditionSet? source,
        string ownerFullName,
        out AdvancedConditionSet rewrittenSet,
        out int removedRows)
    {
        rewrittenSet = source?.Clone() ?? new AdvancedConditionSet();
        removedRows = 0;
        if (source is not { Rows.Count: > 0 })
        {
            return false;
        }

        rewrittenSet = new AdvancedConditionSet();
        foreach (var row in source.Rows)
        {
            if ((row.EvaluationMode is AdvancedConditionEvaluationMode.RuntimeTargetOnly or AdvancedConditionEvaluationMode.ReportOnly) &&
                ArchetypeInherentCatalog.ShouldStripUnrewrittenRuntimeExpression(ownerFullName, row.RawExpression))
            {
                removedRows++;
                continue;
            }

            var clone = row.Clone();
            clone.Link = rewrittenSet.Rows.Count == 0 ? AdvancedConditionLink.And : row.Link;
            rewrittenSet.Rows.Add(clone);
        }

        return removedRows > 0;
    }

    private static void RewritePlannerStatePower(IDatabase database, IPower power, OmniApplyResult applyResult)
    {
        var powerChanged = false;
        var strippedStateEffects = 0;
        var rewrittenRows = 0;
        var rewrittenExpressions = 0;

        if (PlannerStateCatalog.TryGetDefinition(power.FullName, out var definition))
        {
            ConfigurePlannerStatePower(database, power, definition, applyResult);
            powerChanged = true;
        }
        else if (TryConfigureImportedStaffFormPower(database, power))
        {
            powerChanged = true;
        }

        if (TryConfigureComputedInherentSupportPower(power))
        {
            powerChanged = true;
        }

        if (RewritePlannerStateConditionSet(power.AdvancedRequirements, power, out var rewrittenRequirements, out var requirementRowChanges))
        {
            power.AdvancedRequirements = rewrittenRequirements;
            power.Requires = rewrittenRequirements.ToLegacyRequirement();
            rewrittenRows += requirementRowChanges;
            powerChanged = true;
        }

        var rewrittenEffects = new List<IEffect>(power.Effects?.Length ?? 0);
        foreach (var effect in power.Effects ?? [])
        {
            if (effect == null)
            {
                continue;
            }

            if (TryStripPlannerStateBookkeeping(effect))
            {
                strippedStateEffects++;
                powerChanged = true;
                continue;
            }

            if (RewritePlannerStateEffect(effect, power, out var rowChanges, out var expressionChanges))
            {
                rewrittenRows += rowChanges;
                rewrittenExpressions += expressionChanges;
                powerChanged = true;
            }

            rewrittenEffects.Add(effect);
        }

        if (!powerChanged)
        {
            return;
        }

        power.Effects = rewrittenEffects.ToArray();
        power.HasGrantPowerEffect = power.Effects.Any(effect => effect?.EffectType == Enums.eEffectType.GrantPower);
        power.IsModified = true;

        applyResult.AddLimited(
            applyResult.ImportIntegrityAuditDetails,
            $"{power.FullName}: planner-state remodel stripped {strippedStateEffects} bookkeeping effect(s), rewrote {rewrittenRows} condition row(s), and rewrote {rewrittenExpressions} expression(s).");
    }

    private static bool RewritePlannerStateConditionSet(
        AdvancedConditionSet? source,
        IPower ownerPower,
        out AdvancedConditionSet rewrittenSet,
        out int rewrittenRows)
    {
        rewrittenRows = 0;
        rewrittenSet = source?.Clone() ?? new AdvancedConditionSet();
        if (source is not { Rows.Count: > 0 })
        {
            return false;
        }

        var changed = false;
        rewrittenSet = new AdvancedConditionSet();
        foreach (var row in source.Rows)
        {
            if (row.Kind == AdvancedConditionKind.SourceOwnPower &&
                PlannerStateCatalog.TryRewriteSourceOwnPowerRow(row, out _, out var replacementRows))
            {
                AppendReplacementRows(rewrittenSet, row, replacementRows);
                rewrittenRows += replacementRows.Count;
                changed = true;
                continue;
            }

            if (row.Kind == AdvancedConditionKind.PowerCount &&
                PlannerStateCatalog.TryRewritePowerCountRow(row, out _, out var rewrittenRow))
            {
                rewrittenRow.Link = rewrittenSet.Rows.Count == 0 ? AdvancedConditionLink.And : row.Link;
                rewrittenSet.Rows.Add(rewrittenRow);
                rewrittenRows++;
                changed = true;
                continue;
            }

            var clonedRow = row.Clone();
            clonedRow.Link = rewrittenSet.Rows.Count == 0 ? AdvancedConditionLink.And : row.Link;
            rewrittenSet.Rows.Add(clonedRow);
        }

        if (!changed)
        {
            rewrittenSet = source.Clone();
        }

        return changed;
    }

    private static void AppendReplacementRows(
        AdvancedConditionSet rewrittenSet,
        AdvancedConditionRow originalRow,
        IReadOnlyList<AdvancedConditionRow> replacementRows)
    {
        if (replacementRows.Count == 0)
        {
            return;
        }

        if (replacementRows.Count == 1)
        {
            var single = replacementRows[0].Clone();
            single.Link = rewrittenSet.Rows.Count == 0 ? AdvancedConditionLink.And : originalRow.Link;
            single.Negated = originalRow.Negated;
            rewrittenSet.Rows.Add(single);
            return;
        }

        for (var index = 0; index < replacementRows.Count; index++)
        {
            var replacement = replacementRows[index].Clone();
            replacement.Link = index == 0
                ? rewrittenSet.Rows.Count == 0 ? AdvancedConditionLink.And : originalRow.Link
                : originalRow.Negated ? AdvancedConditionLink.Or : AdvancedConditionLink.And;
            replacement.Negated = originalRow.Negated;
            rewrittenSet.Rows.Add(replacement);
        }
    }

    private static bool TryStripPlannerStateBookkeeping(IEffect effect)
    {
        if (effect.EffectType is not (Enums.eEffectType.GrantPower or Enums.eEffectType.RevokePower or Enums.eEffectType.ExecutePower) ||
            effect.ToWho == Enums.eToWho.Target)
        {
            return false;
        }

        var targetPowerFullName = effect.EffectType switch
        {
            Enums.eEffectType.GrantPower or Enums.eEffectType.ExecutePower => effect.Summon ?? string.Empty,
            Enums.eEffectType.RevokePower => effect.RevokedPower ?? string.Empty,
            _ => string.Empty
        };

        return PlannerStateCatalog.IsSelfStateMarker(targetPowerFullName);
    }

    private static bool RewritePlannerStateEffect(
        IEffect effect,
        IPower ownerPower,
        out int rewrittenRows,
        out int rewrittenExpressions)
    {
        rewrittenRows = 0;
        rewrittenExpressions = 0;
        var changed = false;

        effect.Expressions ??= new Expressions();

        if (RewritePlannerStateConditionSet(effect.AdvancedConditions, ownerPower, out var rewrittenConditions, out var rowChanges))
        {
            effect.AdvancedConditions = rewrittenConditions;
            effect.NormalizeConditionState();
            rewrittenRows += rowChanges;
            changed = true;
        }

        if (PlannerStateCatalog.TryRewriteNumericExpression(effect.Expressions.Magnitude, out var magnitudeExpression))
        {
            effect.Expressions.Magnitude = magnitudeExpression;
            effect.MagnitudeExpression = magnitudeExpression;
            rewrittenExpressions++;
            changed = true;
        }

        if (PlannerStateCatalog.TryRewriteNumericExpression(effect.Expressions.Duration, out var durationExpression))
        {
            effect.Expressions.Duration = durationExpression;
            rewrittenExpressions++;
            changed = true;
        }

        if (PlannerStateCatalog.TryRewriteNumericExpression(effect.Expressions.Probability, out var probabilityExpression))
        {
            effect.Expressions.Probability = probabilityExpression;
            rewrittenExpressions++;
            changed = true;
        }

        return changed;
    }

    private static void EnsurePlannerStateControl(
        IDatabase database,
        PlannerStateControlDefinition definition,
        ref IPower?[] powers,
        IDictionary<string, IPower> powersByName,
        ref int nextStaticIndex,
        OmniApplyResult applyResult)
    {
        var createdPower = false;
        if (!powersByName.TryGetValue(definition.FullName, out var power))
        {
            if (definition.PresentationType == PlannerStatePresentationType.ReuseImportedPower)
            {
                return;
            }

            power = CreateSyntheticPlannerControl(definition, nextStaticIndex++);
            Array.Resize(ref powers, powers.Length + 1);
            powers[^1] = power;
            powersByName[power.FullName] = power;
            applyResult.SyntheticPlannerInherentsCreated++;
            createdPower = true;
        }

        ConfigurePlannerStatePower(database, power, definition, applyResult);

        if (createdPower)
        {
            applyResult.AddLimited(
                applyResult.SyntheticPlannerInherentDetails,
                $"{power.FullName}: created planner-state control for {definition.DisplayName}.");
        }
    }

    private static Power CreateSyntheticPlannerControl(PlannerStateControlDefinition definition, int staticIndex)
    {
        var power = definition.IsModeControl
            ? CreateSyntheticPlannerPower(definition.Mode, staticIndex)
            : new Power();

        power.FullName = definition.FullName;
        power.GroupName = "Inherent";
        power.SetName = "Inherent";
        power.PowerName = definition.FullName.Split('.').LastOrDefault() ?? definition.DisplayName.Replace(" ", "_", StringComparison.Ordinal);
        power.StaticIndex = staticIndex;
        power.IsNew = true;
        power.IsModified = true;
        power.Effects ??= [];

        return power;
    }

    private static void ConfigurePlannerStatePower(
        IDatabase database,
        IPower power,
        PlannerStateControlDefinition definition,
        OmniApplyResult applyResult)
    {
        power.IncludeFlag = true;
        power.Available = 1;
        power.Level = 1;
        power.ShowInSpecialPowerPicker = false;
        power.DisplayName = definition.DisplayName;
        power.IconName = !string.IsNullOrWhiteSpace(definition.IconName) ? definition.IconName : power.IconName;
        power.MutexAuto = !string.IsNullOrWhiteSpace(definition.MutexGroup);
        power.GroupMembership = string.IsNullOrWhiteSpace(definition.MutexGroup) ? [] : [definition.MutexGroup];
        power.NGroupMembership = ResolveMutexGroupIds(database, power.GroupMembership);

        if (definition.IsHiddenPayload)
        {
            power.HiddenPower = true;
            power.InherentType = Enums.eGridType.None;
            power.PowerType = Enums.ePowerType.Auto_;
            power.AlwaysToggle = true;
            power.VariableEnabled = false;
            power.ShowStatToggle = false;
            power.AdvancedRequirements = BuildHiddenPayloadRequirements(definition);
            power.Requires = power.AdvancedRequirements.ToLegacyRequirement();
            power.DescShort = $"Hidden planner payload for {definition.DisplayName}.";
            power.DescLong = $"Auto-managed hidden planner payload for {definition.DisplayName}.";
            power.IsModified = true;
            return;
        }

        power.HiddenPower = !definition.VisibleInInherentGrid;
        power.InherentType = definition.VisibleInInherentGrid ? definition.VisibleGridType : Enums.eGridType.None;
        if (definition.VisibleInInherentGrid)
        {
            power.GroupName = "Inherent";
            power.SetName = "Inherent";
        }
        power.PowerType = Enums.ePowerType.Auto_;
        var isPassiveComputedInherent = definition.FullName.Equals(PlannerStateCatalog.DefiancePowerFullName, StringComparison.OrdinalIgnoreCase);
        power.AlwaysToggle = definition.IsVariableControl || isPassiveComputedInherent;
        power.ShowStatToggle = definition.VisibleInInherentGrid && !definition.IsVariableControl && !isPassiveComputedInherent;
        if (definition.PresentationType != PlannerStatePresentationType.ReuseImportedPower)
        {
            power.DescShort = $"Planner state for {definition.DisplayName}.";
            power.DescLong = definition.IsVariableControl
                ? $"Use the planner slider to pick the {definition.DisplayName} stack count for Mids calculations."
                : $"Turn this on to make Mids assume {definition.DisplayName} is active.";
        }
        else
        {
            if (string.IsNullOrWhiteSpace(power.DescShort))
            {
                power.DescShort = $"Planner state for {definition.DisplayName}.";
            }

            if (string.IsNullOrWhiteSpace(power.DescLong))
            {
                power.DescLong = definition.IsVariableControl
                    ? $"Use the planner slider to pick the {definition.DisplayName} stack count for Mids calculations."
                    : $"Turn this on to make Mids assume {definition.DisplayName} is active.";
            }
        }
        power.AdvancedRequirements = new AdvancedConditionSet();
        power.Requires = power.AdvancedRequirements.ToLegacyRequirement();
        power.VariableEnabled = definition.IsVariableControl;
        power.VariableMin = definition.VariableMin;
        power.VariableMax = definition.VariableMax;
        power.VariableStart = definition.VariableStart;
        power.VariableName = definition.IsVariableControl ? definition.VariableName : power.VariableName;
        power.VariableDisplayDivisor = definition.VariableDisplayDivisor;
        power.VariableDisplayPrecision = definition.VariableDisplayPrecision;
        power.VariableDisplayStep = definition.VariableDisplayStep;

        if (definition.IsModeControl)
        {
            AddPlannerModePayload(power, definition.Mode, applyResult);
        }

        if (definition.IsVariableControl && definition.ApplyVariableScalingModel)
        {
            ApplyVariableScalingModel(power);
        }

        power.IsModified = true;
    }

    private static bool TryConfigureImportedStaffFormPower(IDatabase database, IPower power)
    {
        if (power.FullName.IndexOf(".Staff_Fighting.Form_of_the_", StringComparison.OrdinalIgnoreCase) < 0)
        {
            return false;
        }

        power.IncludeFlag = true;
        power.HiddenPower = false;
        power.ShowInSpecialPowerPicker = false;
        power.MutexAuto = true;
        power.GroupMembership = [PlannerStateCatalog.StaffFormMutexGroup];
        power.NGroupMembership = ResolveMutexGroupIds(database, power.GroupMembership);
        power.IsModified = true;
        return true;
    }

    private static int[] ResolveMutexGroupIds(IDatabase database, IEnumerable<string>? groupMembership)
    {
        var mutexList = database.MutexList ?? DatabaseAPI.Database?.MutexList ?? [];
        var groups = groupMembership?
            .Where(group => !string.IsNullOrWhiteSpace(group))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];
        if (groups.Length == 0 || mutexList.Length == 0)
        {
            return [];
        }

        var ids = new List<int>(groups.Length);
        foreach (var group in groups)
        {
            var index = Array.FindIndex(mutexList,
                item => string.Equals(item, group, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                ids.Add(index);
            }
        }

        return ids.Distinct().ToArray();
    }

    private static AdvancedConditionSet BuildHiddenPayloadRequirements(PlannerStateControlDefinition definition)
    {
        var requirements = new AdvancedConditionSet();
        switch (definition.FullName)
        {
            case PlannerStateCatalog.PerfectionBody1Marker:
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionOfBody));
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionLevel1));
                break;
            case PlannerStateCatalog.PerfectionBody2Marker:
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionOfBody));
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionLevel2));
                break;
            case PlannerStateCatalog.PerfectionBody3Marker:
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionOfBody));
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionLevel3));
                break;
            case PlannerStateCatalog.PerfectionMind1Marker:
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionOfMind));
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionLevel1));
                break;
            case PlannerStateCatalog.PerfectionMind2Marker:
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionOfMind));
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionLevel2));
                break;
            case PlannerStateCatalog.PerfectionMind3Marker:
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionOfMind));
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionLevel3));
                break;
            case PlannerStateCatalog.PerfectionSoul1Marker:
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionOfSoul));
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionLevel1));
                break;
            case PlannerStateCatalog.PerfectionSoul2Marker:
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionOfSoul));
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionLevel2));
                break;
            case PlannerStateCatalog.PerfectionSoul3Marker:
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionOfSoul));
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.PerfectionLevel3));
                break;
            case PlannerStateCatalog.DominationModePowerFullName:
                requirements.Rows.Add(SourceModeRequirement(PlannerMode.DominationActive));
                break;
        }

        return requirements;
    }

    private static AdvancedConditionRow SourceModeRequirement(PlannerMode mode)
    {
        return new AdvancedConditionRow
        {
            Link = AdvancedConditionLink.And,
            Kind = AdvancedConditionKind.SourceMode,
            Subject = PlannerModeMapper.ToCanonicalName(mode),
            Operator = AdvancedConditionOperator.Equals,
            Value = "true"
        };
    }

    private static void ApplyVariableScalingModel(IPower power)
    {
        foreach (var effect in power.Effects ?? [])
        {
            if (effect == null)
            {
                continue;
            }

            if (effect.EffectType is Enums.eEffectType.GrantPower or
                Enums.eEffectType.ExecutePower or
                Enums.eEffectType.RevokePower or
                Enums.eEffectType.SetMode or
                Enums.eEffectType.UnsetMode or
                Enums.eEffectType.GlobalChanceMod)
            {
                continue;
            }

            effect.VariableModifiedOverride = true;
        }
    }

    private static bool TryConfigureComputedInherentSupportPower(IPower power)
    {
        if (power.FullName.Equals(VigilanceEndAdjustmentPowerFullName, StringComparison.OrdinalIgnoreCase))
        {
            return ConfigureVigilanceEndAdjustmentPower(power);
        }

        return false;
    }

    private static bool ConfigureVigilanceEndAdjustmentPower(IPower power)
    {
        var changed = false;
        foreach (var effect in power.Effects ?? [])
        {
            if (effect == null)
            {
                continue;
            }

            var hadPlayerTargetRows = effect.AdvancedConditions.Rows.Any(row =>
                row.RawExpression.Contains("target>enttype", StringComparison.OrdinalIgnoreCase));
            if (hadPlayerTargetRows)
            {
                effect.AdvancedConditions = new AdvancedConditionSet();
                effect.NormalizeConditionState();
                changed = true;
            }

            effect.Expressions ??= new Expressions();
            if (!string.Equals(effect.Expressions.Magnitude, "cfg>team>vigilanceenddiscount", StringComparison.OrdinalIgnoreCase))
            {
                effect.Expressions.Magnitude = "cfg>team>vigilanceenddiscount";
                effect.MagnitudeExpression = "cfg>team>vigilanceenddiscount";
                effect.AttribType = Enums.eAttribType.Expression;
                changed = true;
            }

            if (effect.PvMode != Enums.ePvX.Any)
            {
                effect.PvMode = Enums.ePvX.Any;
                changed = true;
            }
        }

        return changed;
    }
}

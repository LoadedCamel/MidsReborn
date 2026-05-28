using System.Text.RegularExpressions;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core.Omni;

public sealed partial class OmniImporter
{
    private const string VigilanceEndAdjustmentPowerFullName = "Inherent.Inherent.Vigilance_PerTeamEndAdjustment";
    private const string ScourgeProbabilityExpression = "minmax((50 - cfg>target>hp) / 40, 0, 1)";
    private const string DominationRagePowerFullName = "Inherent.Inherent.Domination_Rage";
    private const string DominationSuppressionTag = "DominationSuppression";
    private const string ScrapperArchetypeClassName = "Class_Scrapper";
    private const string ScrapperCritGenericTag = "ScrapperCrit";
    private const float StandardScrapperCritSmallChance = 0.05f;
    private const float StandardScrapperCritLargeChance = 0.10f;
    private const string MinionProfileClassName = "Class_Minion_Grunt";
    private const string AssassinationFocusProbabilityExpression =
        "minmax(Temporary_Powers.Temporary_Powers.Assassins_Focus>variableVal * 0.333, 0, 1)";

    private enum ScrapperCritVariant
    {
        None,
        Small,
        Large,
        Player,
        GenericCritter,
        GenericPlayer
    }

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

        if (ConfigureAssassinationCritProbability(effect))
        {
            rewrittenExpressions++;
            changed = true;
        }

        return changed;
    }

    private static bool ConfigureAssassinationCritProbability(IEffect effect)
    {
        if (effect == null ||
            effect.EffectType != Enums.eEffectType.Damage ||
            !effect.EffectTags.Contains(AssassinationPlanner.AssassinsStrikeChanceTag, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        effect.Expressions ??= new Expressions();
        if (string.Equals(effect.Expressions.Probability, AssassinationFocusProbabilityExpression, StringComparison.OrdinalIgnoreCase) &&
            Math.Abs(effect.BaseProbability) <= float.Epsilon)
        {
            return false;
        }

        effect.Expressions.Probability = AssassinationFocusProbabilityExpression;
        effect.BaseProbability = 0f;
        return true;
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
        var isPassiveComputedInherent = definition.FullName.Equals(PlannerStateCatalog.DefiancePowerFullName, StringComparison.OrdinalIgnoreCase);
        var isCombatSettingsDrivenInherent =
            definition.FullName.Equals(PlannerStateCatalog.OpportunityPowerFullName, StringComparison.OrdinalIgnoreCase) ||
            definition.FullName.Equals(PlannerStateCatalog.AssassinationPowerFullName, StringComparison.OrdinalIgnoreCase);
        var isDominationActiveInherent = definition.FullName.Equals(PlannerStateCatalog.DominationPowerFullName, StringComparison.OrdinalIgnoreCase);
        var isVisibleModePlannerToggle =
            definition.VisibleInInherentGrid &&
            definition.IsModeControl &&
            !isDominationActiveInherent;
        power.PowerType = isDominationActiveInherent
            ? Enums.ePowerType.Click
            : isVisibleModePlannerToggle
                ? Enums.ePowerType.Toggle
                : Enums.ePowerType.Auto_;
        power.ClickBuff = isDominationActiveInherent || power.ClickBuff;
        power.AlwaysToggle = power.PowerType == Enums.ePowerType.Auto_ &&
                             (definition.IsVariableControl || isPassiveComputedInherent || isCombatSettingsDrivenInherent);
        if (isDominationActiveInherent && power.ModesRequired.HasFlag(Enums.eModeFlags.Domination))
        {
            // In planner math, toggling Domination on already implies the meter was ready.
            power.ModesRequired &= ~Enums.eModeFlags.Domination;
        }
        power.ShowStatToggle = definition.VisibleInInherentGrid &&
                               !definition.IsVariableControl &&
                               !isPassiveComputedInherent &&
                               !isCombatSettingsDrivenInherent;
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
        power.VariableEnabled = definition.IsVariableControl && !isCombatSettingsDrivenInherent;
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

    private static AdvancedConditionRow SourceModeRequirement(PlannerMode mode, bool negated = false)
    {
        return new AdvancedConditionRow
        {
            Link = AdvancedConditionLink.And,
            Negated = negated,
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
        if (ConfigureDominationPlannerSupport(power))
        {
            return true;
        }

        if (power.FullName.Equals(VigilanceEndAdjustmentPowerFullName, StringComparison.OrdinalIgnoreCase))
        {
            return ConfigureVigilanceEndAdjustmentPower(power);
        }

        if (ConfigureScrapperCriticalHitRows(power))
        {
            return true;
        }

        if (ConfigureScourgeDamageRows(power))
        {
            return true;
        }

        if (IsDominatorDominationTaggedPower(power) &&
            ConfigureDominationTaggedRows(power))
        {
            return true;
        }

        return false;
    }

    private static bool ConfigureDominationPlannerSupport(IPower power)
    {
        if (power.FullName.Equals(PlannerStateCatalog.DominationPowerFullName, StringComparison.OrdinalIgnoreCase))
        {
            return StripDominationChancePlumbing(power);
        }

        if (power.FullName.Equals(DominationRagePowerFullName, StringComparison.OrdinalIgnoreCase))
        {
            return GateDominationSuppressionWhileActive(power);
        }

        return false;
    }

    private static bool IsDominatorDominationTaggedPower(IPower power)
    {
        if (power == null)
        {
            return false;
        }

        var fullName = power.FullName ?? string.Empty;
        var groupName = power.GroupName ?? string.Empty;
        var setName = power.SetName ?? string.Empty;

        return fullName.StartsWith("Dominator_", StringComparison.OrdinalIgnoreCase) ||
               groupName.StartsWith("Dominator_", StringComparison.OrdinalIgnoreCase) ||
               setName.StartsWith("Dominator_", StringComparison.OrdinalIgnoreCase) ||
               (fullName.StartsWith("Pets.", StringComparison.OrdinalIgnoreCase) &&
                fullName.Contains("_Dominator", StringComparison.OrdinalIgnoreCase));
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

    private static bool ConfigureScrapperCriticalHitRows(IPower power)
    {
        if (power.Effects is not { Length: > 0 })
        {
            return false;
        }

        var changed = false;
        var rewrittenEffects = new List<IEffect>(power.Effects.Length);
        foreach (var sourceEffect in power.Effects)
        {
            if (sourceEffect is not Effect effect)
            {
                if (sourceEffect != null)
                {
                    rewrittenEffects.Add(sourceEffect);
                }

                continue;
            }

            var variant = ClassifyScrapperCritVariant(effect);
            switch (variant)
            {
                case ScrapperCritVariant.None:
                    rewrittenEffects.Add(effect);
                    break;

                case ScrapperCritVariant.Small:
                    changed |= NormalizeScrapperCritEffect(effect, ScrapperCritVariant.Small);
                    rewrittenEffects.Add(effect);
                    break;

                case ScrapperCritVariant.Large:
                    changed |= NormalizeScrapperCritEffect(effect, ScrapperCritVariant.Large);
                    rewrittenEffects.Add(effect);
                    break;

                case ScrapperCritVariant.Player:
                    changed |= NormalizeScrapperCritEffect(effect, ScrapperCritVariant.Player);
                    rewrittenEffects.Add(effect);
                    break;

                case ScrapperCritVariant.GenericPlayer:
                    changed |= NormalizeScrapperCritEffect(effect, ScrapperCritVariant.Player);
                    rewrittenEffects.Add(effect);
                    break;

                case ScrapperCritVariant.GenericCritter:
                    rewrittenEffects.Add(effect);
                    break;
            }
        }

        if (!changed)
        {
            return false;
        }

        power.Effects = rewrittenEffects.ToArray();
        power.HasGrantPowerEffect = power.Effects.Any(effect => effect?.EffectType == Enums.eEffectType.GrantPower);
        power.IsModified = true;
        return true;
    }

    private static ScrapperCritVariant ClassifyScrapperCritVariant(Effect effect)
    {
        if (!IsScrapperCriticalHitDamageEffect(effect))
        {
            return ScrapperCritVariant.None;
        }

        if (HasEffectTag(effect, "CritSmall"))
        {
            return ScrapperCritVariant.Small;
        }

        if (HasEffectTag(effect, "CritLarge"))
        {
            return ScrapperCritVariant.Large;
        }

        if (HasEffectTag(effect, "CritPlayer"))
        {
            return ScrapperCritVariant.Player;
        }

        if (!TryGetArchetypeCondition(effect, out var archetypeClass) ||
            !archetypeClass.Equals(ScrapperArchetypeClassName, StringComparison.OrdinalIgnoreCase))
        {
            return ScrapperCritVariant.None;
        }

        if (TargetsPlayer(effect))
        {
            return ScrapperCritVariant.GenericPlayer;
        }

        return TargetsCritter(effect)
            ? ScrapperCritVariant.GenericCritter
            : ScrapperCritVariant.None;
    }

    private static bool IsScrapperCriticalHitDamageEffect(Effect effect)
    {
        if (effect == null || effect.EffectType != Enums.eEffectType.Damage)
        {
            return false;
        }

        var modifierTable = effect.ModifierTable ?? string.Empty;
        if (!modifierTable.Contains("InherentDamage", StringComparison.OrdinalIgnoreCase) &&
            !modifierTable.Contains("PvPDamage", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return HasEffectTag(effect, "CritSmall") ||
               HasEffectTag(effect, "CritLarge") ||
               HasEffectTag(effect, "CritPlayer") ||
               HasEffectTag(effect, "ScrapperCrit") ||
               (TryGetArchetypeCondition(effect, out var archetypeClass) &&
                archetypeClass.Equals(ScrapperArchetypeClassName, StringComparison.OrdinalIgnoreCase));
    }

    private static bool NormalizeScrapperCritEffect(Effect effect, ScrapperCritVariant variant)
    {
        var changed = false;
        if (effect.AdvancedConditions == null)
        {
            effect.AdvancedConditions = new AdvancedConditionSet();
            changed = true;
        }

        var rewrittenConditions = CloneNonTargetConditions(effect.AdvancedConditions);
        switch (variant)
        {
            case ScrapperCritVariant.Small:
                changed |= AddTargetArchetypeRequirement(rewrittenConditions, MinionProfileClassName, AdvancedConditionOperator.Equals);
                changed |= EnsurePvMode(effect, Enums.ePvX.PvE);
                changed |= EnsureBaseProbability(effect, StandardScrapperCritSmallChance);
                changed |= EnsureCritTags(effect, "CritSmall");
                break;

            case ScrapperCritVariant.Large:
                changed |= AddTargetArchetypeRequirement(rewrittenConditions, MinionProfileClassName, AdvancedConditionOperator.NotEquals);
                changed |= EnsurePvMode(effect, Enums.ePvX.PvE);
                changed |= EnsureBaseProbability(effect, StandardScrapperCritLargeChance);
                changed |= EnsureCritTags(effect, "CritLarge");
                break;

            case ScrapperCritVariant.Player:
                changed |= AddTargetEntityRequirement(rewrittenConditions, AdvancedConditionTargetScope.Player);
                changed |= EnsurePvMode(effect, Enums.ePvX.PvP);
                changed |= EnsureBaseProbability(effect, StandardScrapperCritSmallChance);
                changed |= EnsureCritTags(effect, "CritPlayer");
                break;
        }

        if (!ConditionSetsEquivalent(effect.AdvancedConditions, rewrittenConditions))
        {
            effect.AdvancedConditions = rewrittenConditions;
            effect.NormalizeConditionState();
            changed = true;
        }

        return changed;
    }

    private static AdvancedConditionSet CloneNonTargetConditions(AdvancedConditionSet source)
    {
        var rewritten = new AdvancedConditionSet();
        if (source?.Rows == null)
        {
            return rewritten;
        }

        foreach (var row in source.Rows)
        {
            if (IsTargetScopedCondition(row))
            {
                continue;
            }

            var clone = row.Clone();
            clone.Link = rewritten.Rows.Count == 0 ? AdvancedConditionLink.And : row.Link;
            rewritten.Rows.Add(clone);
        }

        return rewritten;
    }

    private static bool IsTargetScopedCondition(AdvancedConditionRow row)
    {
        if (row == null)
        {
            return false;
        }

        if (row.Kind is AdvancedConditionKind.TargetEntityType or
            AdvancedConditionKind.TargetArchetype or
            AdvancedConditionKind.TargetGroup or
            AdvancedConditionKind.TargetMode)
        {
            return true;
        }

        return row.EvaluationMode is AdvancedConditionEvaluationMode.RuntimeTargetOnly or AdvancedConditionEvaluationMode.ReportOnly
            && !string.IsNullOrWhiteSpace(row.RawExpression)
            && row.RawExpression.Contains("target", StringComparison.OrdinalIgnoreCase);
    }

    private static bool AddTargetArchetypeRequirement(
        AdvancedConditionSet conditions,
        string className,
        AdvancedConditionOperator op)
    {
        if (conditions.Rows.Any(row =>
                row.Kind == AdvancedConditionKind.TargetArchetype &&
                row.Operator == op &&
                row.Value.Equals(className, StringComparison.OrdinalIgnoreCase) &&
                !row.Negated))
        {
            return false;
        }

        conditions.Rows.Add(new AdvancedConditionRow
        {
            Link = conditions.Rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.And,
            Kind = AdvancedConditionKind.TargetArchetype,
            Subject = "arch",
            Operator = op,
            Value = className,
            Negated = false,
            RawExpression = op == AdvancedConditionOperator.NotEquals
                ? $"target>arch ne '{className}'"
                : $"target>arch eq '{className}'",
            EvaluationMode = AdvancedConditionEvaluationMode.BuildEvaluated
        });

        return true;
    }

    private static bool AddTargetEntityRequirement(
        AdvancedConditionSet conditions,
        AdvancedConditionTargetScope scope)
    {
        if (conditions.Rows.Any(row =>
                row.Kind == AdvancedConditionKind.TargetEntityType &&
                row.TargetScope == scope &&
                row.Operator == AdvancedConditionOperator.Equals &&
                !row.Negated))
        {
            return false;
        }

        conditions.Rows.Add(new AdvancedConditionRow
        {
            Link = conditions.Rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.And,
            Kind = AdvancedConditionKind.TargetEntityType,
            Subject = scope == AdvancedConditionTargetScope.Player ? "player" : "critter",
            Value = scope.ToString(),
            TargetScope = scope,
            Operator = AdvancedConditionOperator.Equals,
            Negated = false,
            RawExpression = scope == AdvancedConditionTargetScope.Player
                ? "target>enttype eq 'player'"
                : "target>enttype eq 'critter'",
            Unsupported = true,
            EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
        });

        return true;
    }

    private static bool EnsurePvMode(Effect effect, Enums.ePvX pvMode)
    {
        if (effect.PvMode == pvMode)
        {
            return false;
        }

        effect.PvMode = pvMode;
        return true;
    }

    private static bool EnsureBaseProbability(Effect effect, float probability)
    {
        if (Approximately(effect.BaseProbability, probability))
        {
            return false;
        }

        effect.BaseProbability = probability;
        return true;
    }

    private static bool EnsureCritTags(Effect effect, string primaryTag)
    {
        var changed = false;
        effect.EffectTags ??= [];
        if (!effect.EffectTags.Contains(primaryTag, StringComparer.OrdinalIgnoreCase))
        {
            effect.EffectTags.Insert(0, primaryTag);
            changed = true;
        }

        if (!effect.EffectTags.Contains(ScrapperCritGenericTag, StringComparer.OrdinalIgnoreCase) &&
            !effect.EffectTags.Any(tag => tag.Contains("ScrapperCrit", StringComparison.OrdinalIgnoreCase)))
        {
            effect.EffectTags.Add(ScrapperCritGenericTag);
            changed = true;
        }

        if (!string.Equals(effect.EffectId, primaryTag, StringComparison.OrdinalIgnoreCase))
        {
            effect.EffectId = primaryTag;
            changed = true;
        }

        return changed;
    }

    private static bool TargetsPlayer(Effect effect)
    {
        return effect.AdvancedConditions?.Rows.Any(row =>
                   row.Kind == AdvancedConditionKind.TargetEntityType &&
                   row.TargetScope == AdvancedConditionTargetScope.Player &&
                   row.Operator == AdvancedConditionOperator.Equals &&
                   !row.Negated) == true;
    }

    private static bool TargetsCritter(Effect effect)
    {
        return effect.AdvancedConditions?.Rows.Any(row =>
                   row.Kind == AdvancedConditionKind.TargetEntityType &&
                   row.TargetScope == AdvancedConditionTargetScope.Foe &&
                   row.Operator == AdvancedConditionOperator.Equals &&
                   !row.Negated) == true;
    }

    private static bool TryGetArchetypeCondition(Effect effect, out string archetypeClass)
    {
        archetypeClass = effect.AdvancedConditions?.Rows
            .FirstOrDefault(row =>
                row.Kind == AdvancedConditionKind.CharacterArchetype &&
                !row.Negated &&
                row.Operator == AdvancedConditionOperator.Equals &&
                !string.IsNullOrWhiteSpace(row.Value))
            ?.Value ?? string.Empty;
        return !string.IsNullOrWhiteSpace(archetypeClass);
    }

    private static bool HasEffectTag(Effect effect, string tag)
    {
        return effect.EffectTags?.Any(value => value.Equals(tag, StringComparison.OrdinalIgnoreCase)) == true;
    }

    private static bool ConditionSetsEquivalent(AdvancedConditionSet left, AdvancedConditionSet right)
    {
        return string.Equals(
            AdvancedConditionCompiler.Compile(left),
            AdvancedConditionCompiler.Compile(right),
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool Approximately(float left, float right)
    {
        return Math.Abs(left - right) <= 0.0001f;
    }

    private static bool ConfigureScourgeDamageRows(IPower power)
    {
        var changed = false;
        foreach (var effect in power.Effects ?? [])
        {
            if (effect == null || effect.EffectType != Enums.eEffectType.Damage)
            {
                continue;
            }

            var rows = effect.AdvancedConditions?.Rows;
            if (rows is not { Count: > 0 })
            {
                continue;
            }

            var hasScourgeHpRoll = rows.Any(row =>
                !string.IsNullOrWhiteSpace(row.RawExpression) &&
                row.RawExpression.Contains("kHitPoints%", StringComparison.OrdinalIgnoreCase) &&
                row.RawExpression.Contains("rand 100", StringComparison.OrdinalIgnoreCase));
            if (!hasScourgeHpRoll)
            {
                continue;
            }

            var rewritten = new AdvancedConditionSet();
            var removedRows = false;
            var forcedPvMode = effect.PvMode;

            foreach (var row in rows)
            {
                var raw = row.RawExpression ?? string.Empty;
                if (TryResolveScourgePvMode(raw, out var rowPvMode))
                {
                    forcedPvMode = rowPvMode;
                }

                if (raw.Contains("kHitPoints%", StringComparison.OrdinalIgnoreCase) &&
                    raw.Contains("rand 100", StringComparison.OrdinalIgnoreCase))
                {
                    removedRows = true;
                    continue;
                }

                if (raw.Contains("target>enttype", StringComparison.OrdinalIgnoreCase) ||
                    raw.Contains("enttype target>", StringComparison.OrdinalIgnoreCase))
                {
                    if (raw.Contains("player", StringComparison.OrdinalIgnoreCase))
                    {
                        forcedPvMode = Enums.ePvX.PvP;
                    }
                    else if (raw.Contains("critter", StringComparison.OrdinalIgnoreCase))
                    {
                        forcedPvMode = Enums.ePvX.PvE;
                    }

                    removedRows = true;
                    continue;
                }

                var clone = row.Clone();
                clone.Link = rewritten.Rows.Count == 0 ? AdvancedConditionLink.And : row.Link;
                rewritten.Rows.Add(clone);
            }

            if (!removedRows)
            {
                continue;
            }

            effect.AdvancedConditions = rewritten;
            effect.NormalizeConditionState();
            effect.Expressions ??= new Expressions();
            effect.Expressions.Probability = ScourgeProbabilityExpression;
            effect.PvMode = forcedPvMode;
            changed = true;
        }

        return changed;
    }

    private static bool ConfigureDominationTaggedRows(IPower power)
    {
        var changed = false;
        foreach (var effect in power.Effects ?? [])
        {
            if (effect == null ||
                !effect.EffectTags.Contains("Domination", StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            effect.AdvancedConditions ??= new AdvancedConditionSet();
            if (!effect.AdvancedConditions.Rows.Any(row =>
                    row.Kind == AdvancedConditionKind.SourceMode &&
                    row.Operator == AdvancedConditionOperator.Equals &&
                    row.Subject.Equals(PlannerModeMapper.ToCanonicalName(PlannerMode.DominationActive), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(row.Value, "true", StringComparison.OrdinalIgnoreCase)))
            {
                effect.AdvancedConditions.Rows.Add(SourceModeRequirement(PlannerMode.DominationActive));
                effect.NormalizeConditionState();
                changed = true;
            }

            effect.Expressions ??= new Expressions();
            if (!string.Equals(effect.Expressions.Probability, "1", StringComparison.OrdinalIgnoreCase) ||
                Math.Abs(effect.BaseProbability - 1f) > float.Epsilon)
            {
                // Live servers gate these rows through Domination-tag chance suppression.
                // For planner snapshots, model them directly as "active while Domination is on".
                effect.Expressions.Probability = "1";
                effect.BaseProbability = 1f;
                changed = true;
            }
        }

        return changed;
    }

    private static bool StripDominationChancePlumbing(IPower power)
    {
        if (power.Effects == null || power.Effects.Length == 0)
        {
            return false;
        }

        var filtered = new List<IEffect>(power.Effects.Length);
        var changed = false;
        foreach (var effect in power.Effects)
        {
            if (effect == null)
            {
                continue;
            }

            var isSuppressionChanceMod = effect.EffectType == Enums.eEffectType.GlobalChanceMod &&
                                         (effect.EffectTags.Contains(DominationSuppressionTag, StringComparer.OrdinalIgnoreCase) ||
                                          string.Equals(effect.Reward, DominationSuppressionTag, StringComparison.OrdinalIgnoreCase));
            var isSuppressionCancel = effect.EffectType == Enums.eEffectType.Null &&
                                      (effect.EffectTags.Contains("Cancel_Effects", StringComparer.OrdinalIgnoreCase) ||
                                       string.Equals(effect.EffectId, "Cancel_Effects", StringComparison.OrdinalIgnoreCase));
            if (isSuppressionChanceMod || isSuppressionCancel)
            {
                changed = true;
                continue;
            }

            filtered.Add(effect);
        }

        if (changed)
        {
            power.Effects = filtered.ToArray();
            power.HasGrantPowerEffect = power.Effects.Any(effect => effect?.EffectType == Enums.eEffectType.GrantPower);
        }

        return changed;
    }

    private static bool GateDominationSuppressionWhileActive(IPower power)
    {
        var changed = false;
        foreach (var effect in power.Effects ?? [])
        {
            if (effect == null ||
                effect.EffectType != Enums.eEffectType.GlobalChanceMod ||
                !effect.EffectTags.Contains(DominationSuppressionTag, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            effect.AdvancedConditions ??= new AdvancedConditionSet();
            if (!effect.AdvancedConditions.Rows.Any(row =>
                    row.Kind == AdvancedConditionKind.SourceMode &&
                    row.Operator == AdvancedConditionOperator.Equals &&
                    row.Negated &&
                    row.Subject.Equals(PlannerModeMapper.ToCanonicalName(PlannerMode.DominationActive), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(row.Value, "true", StringComparison.OrdinalIgnoreCase)))
            {
                effect.AdvancedConditions.Rows.Add(SourceModeRequirement(PlannerMode.DominationActive, negated: true));
                effect.NormalizeConditionState();
                changed = true;
            }
        }

        return changed;
    }

    private static bool TryResolveScourgePvMode(string rawExpression, out Enums.ePvX pvMode)
    {
        if (rawExpression.Contains("player", StringComparison.OrdinalIgnoreCase))
        {
            pvMode = Enums.ePvX.PvP;
            return true;
        }

        if (rawExpression.Contains("critter", StringComparison.OrdinalIgnoreCase))
        {
            pvMode = Enums.ePvX.PvE;
            return true;
        }

        pvMode = Enums.ePvX.Any;
        return false;
    }
}

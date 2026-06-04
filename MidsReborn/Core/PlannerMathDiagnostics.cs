using System.Globalization;
using System.Text;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.Core.PlannerRulesets;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Forms;
using System.Windows.Forms;

namespace Mids_Reborn.Core;

public sealed class PlannerMathDiagnosticOptions
{
    public string? OutputPath { get; init; }
    public IReadOnlyList<string>? PowerFullNames { get; init; }
    public bool IncludeCurrentBuildComparison { get; init; } = true;
    public bool IncludeVerboseEffectRows { get; init; } = true;
}

public sealed class PlannerMathDiagnosticReport
{
    public PlannerMathDiagnosticReport(string powerFullName, string text)
    {
        PowerFullName = powerFullName;
        Text = text;
    }

    public string PowerFullName { get; }
    public string Text { get; }

    public override string ToString()
    {
        return Text;
    }
}

public static class PlannerMathDiagnosticRunner
{
    public static string GenerateReport(PlannerMathDiagnosticOptions? options = null)
    {
        options ??= new PlannerMathDiagnosticOptions();
        var samplePowers = ResolveSamplePowerList(options.PowerFullNames);
        var builder = new StringBuilder();
        builder.AppendLine("# Planner Math Diagnostic Report");
        builder.AppendLine();
        builder.AppendLine($"- Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine($"- Archetype: {MidsContext.Character?.Archetype?.ClassName ?? MidsContext.Archetype?.ClassName ?? "(none)"}");
        builder.AppendLine($"- Character level: {MidsContext.Character?.Level.ToString(CultureInfo.InvariantCulture) ?? "(none)"}");
        builder.AppendLine($"- Math level index: {MidsContext.MathLevelBase}");
        builder.AppendLine($"- Include current build comparison: {options.IncludeCurrentBuildComparison}");
        builder.AppendLine();

        AppendLegacyCallerAudit(builder);

        var missing = samplePowers
            .Where(s => s.Power == null)
            .Select(s => $"{s.Label}: {s.FullName}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (missing.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("## Missing Samples");
            foreach (var item in missing)
            {
                builder.AppendLine($"- {item}");
            }
        }

        if (options.IncludeCurrentBuildComparison)
        {
            try
            {
                global::Mids_Reborn.MainModule.MidsController.Toon?.GenerateBuffedPowerArray();
            }
            catch (Exception ex)
            {
                builder.AppendLine();
                builder.AppendLine("## Current Build Comparison Setup Error");
                builder.AppendLine($"- {Escape(ex.GetType().Name)}: {Escape(ex.Message)}");
            }
        }

        AppendTopLevelAudit(builder, samplePowers.Where(s => s.Power != null).Select(s => s.Power!).ToList());
        AppendKnownValidOddities(builder);

        foreach (var sample in samplePowers.Where(s => s.Power != null))
        {
            builder.AppendLine();
            builder.AppendLine($"## {sample.Label}");
            builder.AppendLine();
            builder.AppendLine(PlannerMathDiagnostics.ResolvePower(sample.Power!, sample.Label, options).Text);
        }

        var report = builder.ToString();
        if (!string.IsNullOrWhiteSpace(options.OutputPath))
        {
            File.WriteAllText(options.OutputPath, report, Encoding.UTF8);
        }

        return report;
    }

    private static List<DiagnosticSamplePower> ResolveSamplePowerList(IReadOnlyList<string>? customPowerFullNames)
    {
        var samples = new List<DiagnosticSamplePower>();
        if (customPowerFullNames is { Count: > 0 })
        {
            foreach (var fullName in customPowerFullNames.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                samples.Add(CreateSample("Custom Sample", fullName));
            }

            return samples;
        }

        AddSample(samples, "Aimed Shot", "Blaster_Ranged.Archery.Aimed_Shot");
        AddSample(samples, "Ranged Shot", "Blaster_Ranged.Archery.Ranged_Shot");
        AddSample(samples, "Rain of Arrows", "Blaster_Ranged.Archery.Rain_of_Arrows");
        AddRedirectSamples(samples, "Ranged Shot Redirect", "Blaster_Ranged.Archery.Ranged_Shot");
        AddSample(samples, "Blaster Defiance", "Inherent.Inherent.Defiance");
        AddSample(samples, "Controller Containment", "Inherent.Inherent.Containment");
        AddSearchSample(samples, "Blood Thirst / GCM", p => ContainsAny(p, "Blood_Thirst", "Blood Thirst"));
        AddSearchSample(samples, "GrantPower Sample", p => p.Effects.Any(e => e.EffectType == Enums.eEffectType.GrantPower));
        AddSearchSample(samples, "ExecutePower Sample", p => p.Effects.Any(e => e.EffectType == Enums.eEffectType.ExecutePower));
        AddSample(samples, "Enflame Usage-Time Pseudo-Pet", "Pool.Sorcery.Enflame");
        AddSearchSample(samples, "Pseudo-Pet Delivery Sample", p => p.Effects.Any(e => e.EffectType == Enums.eEffectType.EntCreate && string.IsNullOrWhiteSpace(e.SummonedEntityName)));
        AddSearchSample(samples, "Kheldian Form-Gated Sample", p => ContainsAny(p, "Bright_Nova", "White_Dwarf", "Dark_Nova", "Black_Dwarf"));
        AddSearchSample(samples, "PPM Proc Sample", p => p.Effects.Any(e => e.ProcsPerMinute > 0));

        return samples
            .GroupBy(s => s.Power?.FullName ?? s.FullName, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();
    }

    private static void AddSample(List<DiagnosticSamplePower> samples, string label, string fullName)
    {
        samples.Add(CreateSample(label, fullName));
    }

    private static void AddRedirectSamples(List<DiagnosticSamplePower> samples, string label, string sourceFullName)
    {
        var source = DatabaseAPI.GetPowerByFullName(sourceFullName);
        if (source == null)
        {
            return;
        }

        foreach (var redirect in source.Effects.Where(e => e.EffectType == Enums.eEffectType.PowerRedirect && e.nOverride >= 0)
                     .Select(e => e.nOverride)
                     .Distinct())
        {
            if (redirect >= 0 && redirect < DatabaseAPI.Database.Power.Length)
            {
                var power = DatabaseAPI.Database.Power[redirect];
                samples.Add(new DiagnosticSamplePower($"{label}: {power.FullName}", power.FullName, power));
            }
        }
    }

    private static void AddSearchSample(List<DiagnosticSamplePower> samples, string label, Func<IPower, bool> predicate)
    {
        var power = DatabaseAPI.Database.Power.FirstOrDefault(p => p != null && predicate(p));
        samples.Add(power == null
            ? new DiagnosticSamplePower(label, "(auto-discovery found no matching power)", null)
            : new DiagnosticSamplePower(label, power.FullName, power));
    }

    private static DiagnosticSamplePower CreateSample(string label, string fullName)
    {
        return new DiagnosticSamplePower(label, fullName, DatabaseAPI.GetPowerByFullName(fullName));
    }

    private static bool ContainsAny(IPower power, params string[] values)
    {
        return values.Any(value =>
            power.FullName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
            power.DisplayName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
            power.PowerName.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private static void AppendLegacyCallerAudit(StringBuilder builder)
    {
        builder.AppendLine("## Legacy Expansion Caller Audit");
        builder.AppendLine("- These direct callers may still disagree with the canonical resolver until migrated:");
        foreach (var caller in LegacyExpansionCallers)
        {
            builder.AppendLine($"  - `{caller}`");
        }
    }

    private static readonly string[] LegacyExpansionCallers =
    [
        "Core/Base/Data_Classes/Power.cs: legacy compatibility wrapper implementations",
        "Core/GroupedFx.cs: local pet absorption display path"
    ];

    private static void AppendTopLevelAudit(StringBuilder builder, IReadOnlyList<IPower> samplePowers)
    {
        var preservedRuntimeGrants = 0;
        var unresolvedWrappers = 0;
        var overlapGroups = 0;
        var duplicateLookingGroups = 0;

        foreach (var power in samplePowers)
        {
            var resolution = PlannerEffectResolver.ResolvePower(new Power(power), new PlannerEffectResolutionContext
            {
                AbsorbPetEffects = power.AbsorbSummonEffects,
                IncludeTrace = true
            });

            preservedRuntimeGrants += resolution.ExpansionEvents.Count(e =>
                e.Kind == PlannerEffectExpansionKind.GrantPower &&
                e.Status == PlannerEffectExpansionStatus.PreservedRuntimeOrTarget);
            unresolvedWrappers += resolution.ExpansionEvents.Count(e =>
                e.Status is PlannerEffectExpansionStatus.PreservedUnresolved or PlannerEffectExpansionStatus.PreservedEmptyTarget or PlannerEffectExpansionStatus.PreservedMaxDepth);
            overlapGroups += CountOverlapGroups(resolution.ResolvedPower.Effects);
            duplicateLookingGroups += CountDuplicateLookingGroups(resolution.ResolvedPower.Effects);
        }

        builder.AppendLine();
        builder.AppendLine("## Actionable Issues");
        builder.AppendLine($"- Exact duplicate-looking effect groups: {duplicateLookingGroups}");
        builder.AppendLine($"- Preserved unresolved wrappers: {unresolvedWrappers}");
        builder.AppendLine();
        builder.AppendLine("## Informational Audit");
        builder.AppendLine($"- Overlapping effect groups: {overlapGroups}");
        builder.AppendLine($"- Preserved runtime/target grants: {preservedRuntimeGrants}");
        builder.AppendLine($"- Legacy caller audit entries: {LegacyExpansionCallers.Length}");
    }

    private static void AppendKnownValidOddities(StringBuilder builder)
    {
        builder.AppendLine();
        builder.AppendLine("## Known Valid Oddities");
        builder.AppendLine("- `Brute_Melee.Savage_Melee.Blood_Thirst` grants `Temporary_Powers.Temporary_Powers.Savage_Melee_Blood_Frenzy_Stalker`; this is known-valid City of Heroes data and is not treated as suspicious.");
    }

    private static int CountOverlapGroups(IEnumerable<IEffect> effects)
    {
        return PlannerMathDiagnostics.CountOverlapGroups(effects);
    }

    private static int CountDuplicateLookingGroups(IEnumerable<IEffect> effects)
    {
        return PlannerMathDiagnostics.CountDuplicateLookingGroups(effects);
    }

    private sealed record DiagnosticSamplePower(string Label, string FullName, IPower? Power);

    internal static string Escape(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Replace("\r", " ", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal);
    }
}

public static class PlannerMathDiagnostics
{
    internal static int CountOverlapGroups(IEnumerable<IEffect> effects)
    {
        return effects
            .Where(IsOverlapRelevant)
            .GroupBy(GetOverlapKey, StringComparer.OrdinalIgnoreCase)
            .Count(g => g.Count() > 1);
    }

    internal static int CountDuplicateLookingGroups(IEnumerable<IEffect> effects)
    {
        return effects
            .Where(IsOverlapRelevant)
            .GroupBy(GetExactDuplicateKey, StringComparer.OrdinalIgnoreCase)
            .Count(g => g.Count() > 1 && PlannerStackRules.ShouldFlagDuplicateLookingGroup(g.Select(item => item).ToArray()));
    }

    public static PlannerMathDiagnosticReport ResolvePower(string powerFullName)
    {
        var power = DatabaseAPI.GetPowerByFullName(powerFullName);
        if (power == null)
        {
            return new PlannerMathDiagnosticReport(powerFullName, $"Power not found: {powerFullName}");
        }

        return ResolvePower(power);
    }

    public static PlannerMathDiagnosticReport ResolvePower(IPower power)
    {
        return ResolvePower(power, power.DisplayName, new PlannerMathDiagnosticOptions());
    }

    public static PlannerMathDiagnosticReport ResolvePower(IPower power, string label, PlannerMathDiagnosticOptions options)
    {
        var raw = new Power(power);
        var resolution = PlannerEffectResolver.ResolvePower(raw, new PlannerEffectResolutionContext
        {
            AbsorbPetEffects = power.AbsorbSummonEffects,
            IncludeTrace = true
        });

        var builder = new StringBuilder();
        AppendRawPowerSection(builder, power, label);
        AppendResolverSection(builder, power, resolution);
        AppendPseudoPetAbsorptionAudit(builder, power, resolution);
        AppendConditionAudit(builder, power, resolution.ResolvedPower);
        AppendExpansionAudit(builder, resolution);
        AppendOverlapAudit(builder, "Raw Overlapping Effects", power.Effects);
        AppendOverlapAudit(builder, "Resolved Overlapping Effects", resolution.ResolvedPower.Effects);
        AppendEffectTable(builder, "Raw Effects", power.Effects, options.IncludeVerboseEffectRows);
        AppendEffectTable(builder, "Resolved Effects", resolution.ResolvedPower.Effects, options.IncludeVerboseEffectRows);
        AppendFocusedDamageGatingAudit(builder, power, resolution.ResolvedPower);
        AppendPickedPowerPipelineAudit(builder, power, options.IncludeCurrentBuildComparison);
        AppendMainUiDisplayPath(builder, power, options.IncludeCurrentBuildComparison);
        AppendWholeSystemPipelineInspection(builder, power, options.IncludeCurrentBuildComparison);
        AppendCurrentBuildComparison(builder, power, resolution.ResolvedPower, options.IncludeCurrentBuildComparison);

        return new PlannerMathDiagnosticReport(power.FullName, builder.ToString());
    }

    private static void AppendRawPowerSection(StringBuilder builder, IPower power, string label)
    {
        builder.AppendLine("### Raw Database Power");
        builder.AppendLine();
        builder.AppendLine($"- Label: {PlannerMathDiagnosticRunner.Escape(label)}");
        builder.AppendLine($"- Full name: `{power.FullName}`");
        builder.AppendLine($"- Display name: {PlannerMathDiagnosticRunner.Escape(power.DisplayName)}");
        builder.AppendLine($"- Power type: `{power.PowerType}`");
        builder.AppendLine($"- Target: `{power.Target}`");
        builder.AppendLine($"- Entities affected: `{power.EntitiesAffected}`");
        builder.AppendLine($"- Attack types: `{power.AttackTypes}`");
        builder.AppendLine($"- Accuracy: {Format(power.Accuracy)}");
        builder.AppendLine($"- Recharge: {Format(power.RechargeTime)}");
        builder.AppendLine($"- Cast time: {Format(power.CastTime)}");
        builder.AppendLine($"- End cost: {Format(power.EndCost)}");
        builder.AppendLine($"- Usage time: {power.UsageTime}");
        builder.AppendLine($"- Activate period: {Format(power.ActivatePeriod)}");
        builder.AppendLine($"- Effect count: {power.Effects.Length}");
        builder.AppendLine($"- Flags: Hidden={power.HiddenPower}, ClickBuff={power.ClickBuff}, AlwaysToggle={power.AlwaysToggle}, Include={power.IncludeFlag}, AbsorbSummonEffects={power.AbsorbSummonEffects}, AbsorbSummonAttributes={power.AbsorbSummonAttributes}");
        builder.AppendLine($"- Modes required: `{power.ModesRequired}`");
        builder.AppendLine($"- Modes disallowed: `{power.ModesDisallowed}`");
        if (power is Power omniPower)
        {
            builder.AppendLine($"- Omni required modes: {FormatList(omniPower.OmniRequiredModesRaw)}");
            builder.AppendLine($"- Omni disallowed modes: {FormatList(omniPower.OmniDisallowedModesRaw)}");
        }
        builder.AppendLine($"- Forced class: `{power.ForcedClass}`");
        builder.AppendLine($"- Modifier tables: {FormatList(power.Effects.Select(e => e.ModifierTable).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase))}");
        builder.AppendLine($"- GCM rewards: {FormatList(power.Effects.Where(e => e.EffectType == Enums.eEffectType.GlobalChanceMod).Select(e => e.Reward).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase))}");
        builder.AppendLine($"- Effect ids/tags: {FormatList(power.Effects.SelectMany(GetEffectTags).Distinct(StringComparer.OrdinalIgnoreCase))}");
        builder.AppendLine();
    }

    private static void AppendResolverSection(StringBuilder builder, IPower sourcePower, PlannerEffectResolution resolution)
    {
        builder.AppendLine("### Resolver Output");
        builder.AppendLine();
        builder.AppendLine($"- Resolved power: `{resolution.ResolvedPower.FullName}`");
        if (resolution.SelectedRedirectPower != null)
        {
            builder.AppendLine($"- Selected redirect: `{resolution.SelectedRedirectPower.FullName}`");
        }
        else
        {
            builder.AppendLine("- Selected redirect: `(none)`");
        }

        var redirectVariants = sourcePower.Effects
            .Where(e => e.EffectType == Enums.eEffectType.PowerRedirect && e.nOverride >= 0)
            .Select(e => FormatRedirectVariant(e, resolution.SelectedRedirectPower))
            .ToList();

        builder.AppendLine($"- Raw effects: {sourcePower.Effects.Length}");
        builder.AppendLine($"- Resolved effects: {resolution.ResolvedPower.Effects.Length}");
        builder.AppendLine($"- GrantPower effects: raw={sourcePower.Effects.Count(e => e.EffectType == Enums.eEffectType.GrantPower)}, resolved={resolution.ResolvedPower.Effects.Count(e => e.EffectType == Enums.eEffectType.GrantPower)}");
        builder.AppendLine($"- ExecutePower effects: raw={sourcePower.Effects.Count(e => e.EffectType == Enums.eEffectType.ExecutePower)}, resolved={resolution.ResolvedPower.Effects.Count(e => e.EffectType == Enums.eEffectType.ExecutePower)}");
        builder.AppendLine($"- EntCreate effects: raw={sourcePower.Effects.Count(e => e.EffectType == Enums.eEffectType.EntCreate)}, resolved={resolution.ResolvedPower.Effects.Count(e => e.EffectType == Enums.eEffectType.EntCreate)}");
        builder.AppendLine();

        if (redirectVariants.Count > 0)
        {
            builder.AppendLine("#### Redirect Variants");
            foreach (var variant in redirectVariants)
            {
                builder.AppendLine($"- {variant}");
            }

            builder.AppendLine();
        }

        builder.AppendLine("#### Trace");
        if (resolution.Trace.Count == 0)
        {
            builder.AppendLine("- No resolver trace entries.");
        }
        else
        {
            foreach (var entry in resolution.Trace)
            {
                builder.AppendLine($"- {entry}");
            }
        }

        builder.AppendLine();
    }

    private static void AppendPseudoPetAbsorptionAudit(StringBuilder builder, IPower sourcePower, PlannerEffectResolution resolution)
    {
        var entCreates = sourcePower.Effects.Where(effect => effect.EffectType == Enums.eEffectType.EntCreate).ToList();
        if (entCreates.Count == 0)
        {
            return;
        }

        builder.AppendLine("### Pseudo-Pet Absorption Audit");
        builder.AppendLine();
        foreach (var effect in entCreates)
        {
            var entity = effect.nSummon >= 0 && effect.nSummon < DatabaseAPI.Database.Entities.Length
                ? DatabaseAPI.Database.Entities[effect.nSummon]
                : null;
            var powersets = entity?.GetNPowerset().ToArray() ?? [];
            var linkedPowers = powersets
                .Where(index => index >= 0 && index < DatabaseAPI.Database.Powersets.Length)
                .Sum(index => DatabaseAPI.Database.Powersets[index].Powers.Length);
            var absorbedEffects = resolution.ResolvedPower.Effects.Count(absorbed => absorbed.Absorbed_Effect);
            var recurrentEffects = resolution.ResolvedPower.Effects
                .Where(absorbed => absorbed.PseudoPetRecurrence is { IsValid: true })
                .Select(absorbed => absorbed.PseudoPetRecurrence!)
                .GroupBy(recurrence => string.Join("|",
                    recurrence.EntityName,
                    recurrence.PetPowerName,
                    Format(recurrence.SourceUsageTime),
                    Format(recurrence.SourceActivatePeriod),
                    Format(recurrence.EntCreateDuration),
                    Format(recurrence.PetTickInterval),
                    recurrence.SpawnCount,
                    recurrence.TicksPerSpawn,
                    recurrence.TotalExpectedTicks))
                .Select(group => group.First())
                .ToList();
            var durationNote = recurrentEffects.Count > 0
                ? "recurrence modeled; EntCreate lifetime kept as per-spawn duration"
                : GetPseudoPetRecurrenceFailure(sourcePower, effect, entity, linkedPowers);
            builder.AppendLine($"- EntCreate `{PlannerMathDiagnosticRunner.Escape(effect.Summon)}` nSummon={effect.nSummon} entityType=`{entity?.EntityType.ToString() ?? "unresolved"}` real={entity?.IsRealPet.ToString() ?? "false"} pseudo={entity?.IsPseudoPet.ToString() ?? "false"}");
            builder.AppendLine($"  UsageTime={sourcePower.UsageTime}, ActivatePeriod={Format(sourcePower.ActivatePeriod)}, EntCreateDuration={Format(effect.Duration)}, EntCreateChance={Format(effect.BaseProbability)}, EntCreateDelay={Format(effect.DelayedTime)}, {durationNote}");
            builder.AppendLine($"  Entity powersets={FormatList(entity?.PowersetFullName ?? [])}, linked pet powers={linkedPowers}, AbsorbSummonEffects={sourcePower.AbsorbSummonEffects}, AbsorbSummonAttributes={sourcePower.AbsorbSummonAttributes}, resolved absorbed effects={absorbedEffects}");
            foreach (var recurrence in recurrentEffects)
            {
                builder.AppendLine($"  Recurrence: entity=`{PlannerMathDiagnosticRunner.Escape(recurrence.EntityName)}`, petPower=`{PlannerMathDiagnosticRunner.Escape(recurrence.PetPowerName)}`, sourceWindow={Format(recurrence.SourceUsageTime)}s, sourcePeriod={Format(recurrence.SourceActivatePeriod)}s, spawnDuration={Format(recurrence.EntCreateDuration)}s, petTick={Format(recurrence.PetTickInterval)}s, spawnCount={recurrence.SpawnCount}, ticksPerSpawn={recurrence.TicksPerSpawn}, totalTicks={recurrence.TotalExpectedTicks}");
            }
        }

        builder.AppendLine();
    }

    private static string GetPseudoPetRecurrenceFailure(IPower sourcePower, IEffect entCreate, SummonedEntity? entity, int linkedPowers)
    {
        if (entity == null)
        {
            return "recurrence not modeled: unresolved entity";
        }

        if (!entity.IsPseudoPet)
        {
            return entity.IsRealPet
                ? "recurrence not modeled: real pet preserved"
                : "recurrence not modeled: entity is not pseudo-pet";
        }

        if (sourcePower.UsageTime <= 0)
        {
            return "recurrence not modeled: missing source usage time";
        }

        if (sourcePower.ActivatePeriod <= 0)
        {
            return "recurrence not modeled: missing source activate period";
        }

        if (entCreate.Duration <= 0)
        {
            return "recurrence not modeled: missing EntCreate duration";
        }

        if (linkedPowers <= 0)
        {
            return "recurrence not modeled: zero linked pet powers";
        }

        return "recurrence not modeled: no absorbed damage effect had a pet tick interval";
    }

    private static void AppendConditionAudit(StringBuilder builder, IPower rawPower, IPower resolvedPower)
    {
        builder.AppendLine("### Condition Audit");
        builder.AppendLine();
        AppendTargetRoutingAudit(builder, rawPower);
        AppendImportedPowerPolicyAudit(builder, rawPower);
        AppendConditionBucket(builder, "Power Requirements", rawPower.AdvancedRequirements.Rows);
        AppendConditionBucket(builder, "Build-Evaluated Effect Conditions", resolvedPower.Effects.SelectMany(e => e.AdvancedConditions.Rows).Where(r => r.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated));
        AppendConditionBucket(builder, "Runtime Target-Only Effect Conditions", resolvedPower.Effects.SelectMany(e => e.AdvancedConditions.Rows).Where(r => r.EvaluationMode == AdvancedConditionEvaluationMode.RuntimeTargetOnly));
        AppendConditionBucket(builder, "Report-Only Effect Conditions", resolvedPower.Effects.SelectMany(e => e.AdvancedConditions.Rows).Where(r => r.EvaluationMode == AdvancedConditionEvaluationMode.ReportOnly));
        AppendConditionBucket(builder, "Unsupported / Advanced Effect Conditions", resolvedPower.Effects
            .SelectMany(e => e.AdvancedConditions.Rows)
            .Where(r => r.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated &&
                        (r.Unsupported || r.Kind == AdvancedConditionKind.AdvancedExpression)));
        AppendWrapperAudit(builder, "Unflattened GrantPower", resolvedPower.Effects.Where(e => e.EffectType == Enums.eEffectType.GrantPower));
        AppendWrapperAudit(builder, "Unflattened ExecutePower", resolvedPower.Effects.Where(e => e.EffectType == Enums.eEffectType.ExecutePower));
        var runtimeRows = resolvedPower.Effects
            .SelectMany(e => e.AdvancedConditions.Rows)
            .Where(r => r.EvaluationMode is AdvancedConditionEvaluationMode.RuntimeTargetOnly or AdvancedConditionEvaluationMode.ReportOnly)
            .ToList();
        AppendRuntimeConditionSummary(builder, runtimeRows);
        AppendConditionBucket(builder, "Runtime-only Conditions Preserved", runtimeRows);
        builder.AppendLine();
    }

    private static void AppendTargetRoutingAudit(StringBuilder builder, IPower rawPower)
    {
        builder.AppendLine("#### Target Routing");
        if (rawPower is not Power power ||
            (power.TargetRoutingPolicy.IsDefault && string.IsNullOrWhiteSpace(power.OmniTargetRequiresRaw)))
        {
            builder.AppendLine("- None.");
            return;
        }

        var policy = power.TargetRoutingPolicy;
        var original = string.IsNullOrWhiteSpace(policy.OriginalTargetRequires)
            ? power.OmniTargetRequiresRaw
            : policy.OriginalTargetRequires;
        builder.AppendLine($"- Raw `target_requires`: {PlannerMathDiagnosticRunner.Escape(string.IsNullOrWhiteSpace(original) ? "(none)" : original)}");
        builder.AppendLine($"- Applied recipient scope: recipients=`{policy.AllowedRecipients}` self=`{policy.SelfRequirement}`");

        if (policy.RecipientClauses.Count == 0)
        {
            builder.AppendLine("- Applied recipient clauses: none.");
        }
        else
        {
            foreach (var group in policy.RecipientClauses
                         .GroupBy(GetRecipientClauseDiagnosticKey, StringComparer.OrdinalIgnoreCase)
                         .Take(20))
            {
                var clause = group.First();
                var countText = group.Count() > 1 ? $" x{group.Count()}" : string.Empty;
                builder.AppendLine($"- Applied recipient clause{countText}: `{clause.Link}` {PlannerConditionRoutingAnalyzer.DescribeRecipientClause(clause)}");
            }
        }

        AppendConditionBucket(builder, "Applied Build Source Gates", policy.BuildSourceGates.Rows);
        AppendConditionBucket(builder, "Deferred Target Routing Fragments", policy.DeferredTargetRows.Rows);
        AppendConditionBucket(builder, "Deferred Source Routing Fragments", policy.DeferredSourceRows.Rows);
    }

    private static void AppendImportedPowerPolicyAudit(StringBuilder builder, IPower rawPower)
    {
        builder.AppendLine("#### Imported Power Policies");
        if (rawPower is not Power power)
        {
            builder.AppendLine("- None.");
            return;
        }

        var entries = ImportedPowerPolicyDiagnostics
            .DescribeRuntime(power, DatabaseAPI.GetServerRulesProfile())
            .Take(20)
            .ToList();
        if (entries.Count == 0)
        {
            builder.AppendLine("- None.");
            return;
        }

        foreach (var entry in entries)
        {
            builder.AppendLine($"- `{entry.Category}` `{entry.FieldName}`: {PlannerMathDiagnosticRunner.Escape(entry.Detail)}");
        }
    }

    private static void AppendConditionBucket(StringBuilder builder, string title, IEnumerable<AdvancedConditionRow> rows)
    {
        var rowList = rows
            .GroupBy(GetConditionDiagnosticKey, StringComparer.OrdinalIgnoreCase)
            .Select(g => new
            {
                Row = g.First(),
                Count = g.Count()
            })
            .Take(50)
            .ToList();
        builder.AppendLine($"#### {title}");
        if (rowList.Count == 0)
        {
            builder.AppendLine("- None.");
            return;
        }

        foreach (var item in rowList)
        {
            var row = item.Row;
            var evaluation = EvaluateConditionForReport(null, row);
            var unsupported = row.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated && row.Unsupported;
            var countText = item.Count > 1 ? $" x{item.Count}" : string.Empty;
            builder.AppendLine($"- `{row.Link}` `{row.Kind}` mode=`{row.EvaluationMode}` eval=`{evaluation}` negated={row.Negated} unsupported={unsupported}{countText}: {PlannerMathDiagnosticRunner.Escape(AdvancedConditionCompiler.Compile(row))}");
        }
    }

    private static void AppendRuntimeConditionSummary(StringBuilder builder, IReadOnlyList<AdvancedConditionRow> rows)
    {
        builder.AppendLine("#### Runtime-only Preserved Summary");
        if (rows.Count == 0)
        {
            builder.AppendLine("- None.");
            return;
        }

        foreach (var group in rows.GroupBy(GetConditionDiagnosticKey, StringComparer.OrdinalIgnoreCase).Take(10))
        {
            builder.AppendLine($"- x{group.Count()} `{group.First().EvaluationMode}` {PlannerMathDiagnosticRunner.Escape(AdvancedConditionCompiler.Compile(group.First()))}");
        }
    }

    private static string GetConditionDiagnosticKey(AdvancedConditionRow row)
    {
        return $"{row.EvaluationMode}|{row.Kind}|{row.Link}|{row.Negated}|{AdvancedConditionCompiler.Compile(row)}";
    }

    private static string GetRecipientClauseDiagnosticKey(PlannerRecipientClause clause)
    {
        return $"{clause.Link}|{clause.Kind}|{clause.Negated}|{clause.Value}";
    }

    private static void AppendExpansionAudit(StringBuilder builder, PlannerEffectResolution resolution)
    {
        builder.AppendLine("### Expansion Audit");
        builder.AppendLine();
        AppendExpansionBucket(builder, "Expanded GrantPower", resolution.ExpansionEvents.Where(e => e.Kind == PlannerEffectExpansionKind.GrantPower && e.Status == PlannerEffectExpansionStatus.Expanded));
        AppendExpansionBucket(builder, "Expanded ExecutePower", resolution.ExpansionEvents.Where(e => e.Kind == PlannerEffectExpansionKind.ExecutePower && e.Status == PlannerEffectExpansionStatus.Expanded));
        AppendExpansionBucket(builder, "Preserved Runtime/Target GrantPower", resolution.ExpansionEvents.Where(e => e.Kind == PlannerEffectExpansionKind.GrantPower && e.Status == PlannerEffectExpansionStatus.PreservedRuntimeOrTarget));
        AppendExpansionBucket(builder, "Preserved Unresolved Wrappers", resolution.ExpansionEvents.Where(e => e.Status is PlannerEffectExpansionStatus.PreservedUnresolved or PlannerEffectExpansionStatus.PreservedEmptyTarget or PlannerEffectExpansionStatus.PreservedMaxDepth));
        builder.AppendLine();
    }

    private static void AppendExpansionBucket(StringBuilder builder, string title, IEnumerable<PlannerEffectExpansionEvent> events)
    {
        var list = events.ToList();
        builder.AppendLine($"#### {title}");
        if (list.Count == 0)
        {
            builder.AppendLine("- None.");
            return;
        }

        foreach (var item in list.Take(50))
        {
            builder.AppendLine($"- `{item.OwnerPower}` -> `{item.TargetPower}` status=`{item.Status}` target=`{item.Target}` chance={Format(item.Chance)} ppm={Format(item.Ppm)} delay={Format(item.Delay)} duration={Format(item.Duration)} conditions={item.ConditionCount} childEffects={item.ChildEffectCount}");
        }
    }

    private static void AppendOverlapAudit(StringBuilder builder, string title, IReadOnlyList<IEffect> effects)
    {
        builder.AppendLine($"### {title}");
        builder.AppendLine();
        var overlapGroups = effects
            .Select((effect, index) => new { Effect = effect, Index = index })
            .Where(item => IsOverlapRelevant(item.Effect))
            .GroupBy(item => GetOverlapKey(item.Effect), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Take(20)
            .ToList();

        if (overlapGroups.Count == 0)
        {
            builder.AppendLine("- None.");
            builder.AppendLine();
            return;
        }

        foreach (var group in overlapGroups)
        {
            var first = group.First().Effect;
            var exactDuplicateGroups = group.GroupBy(item => GetExactDuplicateKey(item.Effect), StringComparer.OrdinalIgnoreCase)
                .Count(g => g.Count() > 1 && PlannerStackRules.ShouldFlagDuplicateLookingGroup(g.Select(item => item.Effect).ToArray()));
            var sources = group
                .Select(i => i.Effect.OmniSource)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(5)
                .ToArray();
            var sourceText = sources.Length == 0 ? string.Empty : $" sources={string.Join("; ", sources.Select(s => $"`{PlannerMathDiagnosticRunner.Escape(s)}`"))}";
            builder.AppendLine($"- {FormatOverlapLabel(first)} count={group.Count()} exactDuplicateLookingGroups={exactDuplicateGroups} indexes={string.Join(", ", group.Select(i => i.Index))}{sourceText}");
        }

        builder.AppendLine();
    }

    private static void AppendWrapperAudit(StringBuilder builder, string title, IEnumerable<IEffect> effects)
    {
        var wrappers = effects.ToList();
        builder.AppendLine($"#### {title}");
        if (wrappers.Count == 0)
        {
            builder.AppendLine("- None.");
            return;
        }

        foreach (var effect in wrappers.Take(50))
        {
            builder.AppendLine($"- {effect.EffectType}: ref=`{FirstNonEmpty(effect.Summon, effect.Override, effect.RevokedPower, effect.nSummon >= 0 ? $"#{effect.nSummon}" : string.Empty)}` target=`{effect.ToWho}` chance={Format(effect.BaseProbability)} ppm={Format(effect.ProcsPerMinute)} conditions={effect.AdvancedConditions.Rows.Count}");
        }
    }

    private static void AppendEffectTable(StringBuilder builder, string title, IReadOnlyList<IEffect> effects, bool verbose)
    {
        builder.AppendLine($"### {title}");
        builder.AppendLine();
        if (effects.Count == 0)
        {
            builder.AppendLine("- No effects.");
            builder.AppendLine();
            return;
        }

        builder.AppendLine("| # | Type | Target | PvX | Attrib | Aspect | Mod Table | Scale | Mag | Math | Duration | Chance | PPM | Delay | Tags/Reward | Source | Conditions | Summary |");
        builder.AppendLine("|---:|---|---|---|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|---|---:|---|");
        var count = verbose ? effects.Count : Math.Min(effects.Count, 40);
        for (var i = 0; i < count; i++)
        {
            var effect = effects[i];
            var modifier = GetModifierAudit(effect);
            var summary = SafeEffectString(effect);
            builder.AppendLine(
                $"| {i} | `{effect.EffectType}` | `{effect.ToWho}` | `{effect.PvMode}` | `{effect.AttribType}` | `{effect.Aspect}` | {modifier} | {Format(effect.Scale)} | {Format(effect.nMagnitude)} | {Format(effect.Math_Mag)} | {Format(effect.nDuration)} | {Format(effect.BaseProbability)} | {Format(effect.ProcsPerMinute)} | {Format(effect.DelayedTime)} | {FormatTagsAndReward(effect)} | {FormatOmniSource(effect)} | {effect.AdvancedConditions.Rows.Count} | {EscapeTable(summary)} |");
        }

        if (count < effects.Count)
        {
            builder.AppendLine($"| ... | | | | | | | | | | | | | | | | | {effects.Count - count} more effects omitted |");
        }

        builder.AppendLine();
    }

    private static void AppendFocusedDamageGatingAudit(StringBuilder builder, IPower rawPower, IPower resolvedPower)
    {
        var isFocusedSample =
            ContainsDiagnosticName(rawPower, "Rain_Of_Arrows", "Rain of Arrows", "RainofArrows") ||
            ContainsDiagnosticName(rawPower, "Enflame", "Pets.Enflame");

        if (!isFocusedSample)
        {
            return;
        }

        builder.AppendLine("### Focused Damage Gating Audit");
        builder.AppendLine();
        builder.AppendLine($"- `FXGetDamageValue`: {Format(resolvedPower.FXGetDamageValue(absorb: false))}");
        builder.AppendLine($"- `FXGetDamageString`: {PlannerMathDiagnosticRunner.Escape(resolvedPower.FXGetDamageString(absorb: false))}");

        var grouped = GroupedFx.AssembleGroupedEffects(resolvedPower, includeDamage: true);
        var groupedPass2 = GroupedFx.AggregateGroupedEffectsPass2(resolvedPower, grouped);
        builder.AppendLine($"- `GroupedFx.AssembleGroupedEffects`: {grouped.Count}");
        builder.AppendLine($"- `GroupedFx.AggregateGroupedEffectsPass2`: {groupedPass2.Count}");
        builder.AppendLine();

        var damageRows = resolvedPower.Effects
            .Select((effect, index) => new { Effect = effect, Index = index })
            .Where(item => item.Effect.EffectType == Enums.eEffectType.Damage)
            .ToList();

        builder.AppendLine("| # | Source | Damage | PvX | Target | Mag | Ticks | Recur | CanInclude | PvXInclude | DamageInclude | Conditions | Math Total |");
        builder.AppendLine("|---:|---|---|---|---|---:|---:|---|---|---|---|---|---:|");
        foreach (var item in damageRows)
        {
            var effect = item.Effect;
            builder.AppendLine(
                $"| {item.Index} | {FormatOmniSource(effect)} | `{effect.DamageType}` | `{effect.PvMode}` | `{effect.ToWho}` | {Format(effect.BuffedMag)} | {Format(Power.GetDamageEffectEffectiveTicks(effect))} | {FormatRecurrence(effect)} | {effect.CanInclude()} | {effect.PvXInclude()} | {Power.ShouldIncludeDamageEffect(effect)} | {EscapeTable(FormatConditionRows(effect))} | {Format(Power.GetDamageEffectTotal(effect, resolvedPower, absolute: false, applyReturnScaling: false))} |");
        }

        var mutuallyExclusiveGroups = damageRows
            .GroupBy(item => string.Join("|",
                item.Effect.EffectType,
                item.Effect.DamageType,
                item.Effect.ToWho,
                item.Effect.PvMode,
                Format(item.Effect.BuffedMag),
                Format(Power.GetDamageEffectEffectiveTicks(item.Effect))))
            .Select(group => new
            {
                Key = group.Key,
                Count = group.Count(),
                ConditionCount = group.Select(item => GetEffectConditionIdentity(item.Effect)).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                Indexes = group.Select(item => item.Index).ToList()
            })
            .Where(group => group.Count > 1 && group.ConditionCount > 1)
            .ToList();

        builder.AppendLine();
        builder.AppendLine("#### Mutually Exclusive Rows Grouped Together");
        if (mutuallyExclusiveGroups.Count == 0)
        {
            builder.AppendLine("- None detected by condition identity.");
        }
        else
        {
            foreach (var group in mutuallyExclusiveGroups)
            {
                builder.AppendLine($"- key=`{PlannerMathDiagnosticRunner.Escape(group.Key)}` rows={string.Join(", ", group.Indexes)} distinctConditionSets={group.ConditionCount}");
            }
        }

        builder.AppendLine();
    }

    private static void AppendCurrentBuildComparison(StringBuilder builder, IPower rawPower, IPower resolverPower, bool include)
    {
        if (!include)
        {
            return;
        }

        builder.AppendLine("### Current Toon Comparison");
        builder.AppendLine();
        var toon = global::Mids_Reborn.MainModule.MidsController.Toon;
        var build = MidsContext.Character?.CurrentBuild;
        if (toon == null || build == null)
        {
            builder.AppendLine("- No active Toon/current build available.");
            builder.AppendLine();
            return;
        }

        var historyIndex = FindActiveBuildHistoryIndex(build, rawPower);
        if (historyIndex < 0)
        {
            builder.AppendLine("- Power is not currently picked in the active build; Toon math comparison skipped.");
            builder.AppendLine();
            return;
        }

        var basePower = toon.GetBasePower(historyIndex);
        var enhancedPower = toon.GetEnhancedPower(historyIndex);
        builder.AppendLine($"- Build history index: {historyIndex}");
        builder.AppendLine($"- Toon base power: `{basePower?.FullName ?? "(none)"}` effects={basePower?.Effects.Length ?? 0}");
        builder.AppendLine($"- Toon enhanced power: `{enhancedPower?.FullName ?? "(none)"}` effects={enhancedPower?.Effects.Length ?? 0}");
        builder.AppendLine($"- Resolver power: `{resolverPower.FullName}` effects={resolverPower.Effects.Length}");
        builder.AppendLine($"- Active source modes: {FormatList(MidsContext.Character.ActiveSourceModes)}");
        builder.AppendLine($"- Active source mode flags: `{MidsContext.Character.ActiveSourceModeFlags}`");
        builder.AppendLine($"- Source mode gate: {PlannerMathDiagnosticRunner.Escape(MidsContext.Character.GetSourceModeGateReason(rawPower))}");
        builder.AppendLine($"- Redirect mismatch: {(!string.Equals(basePower?.FullName, resolverPower.FullName, StringComparison.OrdinalIgnoreCase)).ToString(CultureInfo.InvariantCulture)}");
        builder.AppendLine($"- Effect count mismatch: {((basePower?.Effects.Length ?? -1) != resolverPower.Effects.Length).ToString(CultureInfo.InvariantCulture)}");

        if (basePower != null)
        {
            AppendEffectMismatchSummary(builder, "Base vs Resolver", basePower, resolverPower);
        }

        if (enhancedPower != null)
        {
            AppendEffectMismatchSummary(builder, "Enhanced vs Resolver", enhancedPower, resolverPower);
        }

        builder.AppendLine();
    }

    private static void AppendPickedPowerPipelineAudit(StringBuilder builder, IPower rawPower, bool include)
    {
        if (!include)
        {
            return;
        }

        var toon = global::Mids_Reborn.MainModule.MidsController.Toon;
        var build = MidsContext.Character?.CurrentBuild;
        if (toon == null || build == null)
        {
            return;
        }

        var historyIndex = FindActiveBuildHistoryIndex(build, rawPower);
        if (historyIndex < 0)
        {
            return;
        }

        builder.AppendLine("### Picked Power Pipeline Audit");
        builder.AppendLine();
        builder.AppendLine($"- Build history index: {historyIndex}");

        var databasePower = rawPower.PowerIndex >= 0 && rawPower.PowerIndex < DatabaseAPI.Database.Power.Length
            ? DatabaseAPI.Database.Power[rawPower.PowerIndex]
            : DatabaseAPI.GetPowerByFullName(rawPower.FullName);
        var basePower = toon.GetBasePower(historyIndex);
        var rawBasePower = TryGetToonPowerArrayEntry(toon, "_basePowers", historyIndex);
        var assembledBasePower = TryGetToonPowerArrayEntry(toon, "_assembledBasePowers", historyIndex);
        var enhancedPower = toon.GetEnhancedPower(historyIndex);
        var mathPower = TryGetToonPowerArrayEntry(toon, "_mathPowers", historyIndex);
        var preBuffPower = TryGetToonPowerArrayEntry(toon, "_preBuffPowers", historyIndex);
        var buffedPower = TryGetToonPowerArrayEntry(toon, "_buffedPowers", historyIndex);
        var displaySnapshot = toon.GetDisplayPowerSnapshot(historyIndex, rawPower.PowerIndex);
        var selfEnhance = TryGetToonBuffsXField(toon, "_selfEnhance");
        var selfBuffs = TryGetToonBuffsXField(toon, "_selfBuffs");
        var resolvedEnhanced = enhancedPower == null
            ? null
            : PlannerEffectResolver.ResolvePower(new Power(enhancedPower), new PlannerEffectResolutionContext
            {
                ApplyRedirects = false
            }).ResolvedPower;

        AppendPipelineStage(builder, "Database", databasePower);
        AppendPipelineStage(builder, "GetBasePower", basePower);
        AppendPipelineStage(builder, "_basePowers", rawBasePower);
        AppendPipelineStage(builder, "_assembledBasePowers", assembledBasePower);
        AppendPipelineStage(builder, "_mathPowers", mathPower);
        AppendBucketSummary(builder, "_selfEnhance", selfEnhance);
        AppendPipelineStage(builder, "_preBuffPowers", preBuffPower);
        AppendBucketSummary(builder, "_selfBuffs", selfBuffs);
        AppendPipelineStage(builder, "_buffedPowers", buffedPower);
        AppendPipelineStage(builder, "Display snapshot base", displaySnapshot.BasePower);
        AppendPipelineStage(builder, "Display snapshot enhanced", displaySnapshot.EnhancedPower);
        AppendPipelineStage(builder, "GetEnhancedPower", enhancedPower);
        AppendPipelineStage(builder, "Resolved pEnh", resolvedEnhanced);
        builder.AppendLine($"- display snapshot base resolved for display: {displaySnapshot.BaseWasResolvedForDisplay.ToString(CultureInfo.InvariantCulture)}");
        builder.AppendLine($"- display snapshot enhanced resolved for display: {displaySnapshot.EnhancedWasResolvedForDisplay.ToString(CultureInfo.InvariantCulture)}");
        builder.AppendLine($"- display snapshot base padded/repaired: {displaySnapshot.BaseWasPaddedOrRepaired.ToString(CultureInfo.InvariantCulture)}");
        builder.AppendLine($"- display snapshot enhanced padded/repaired: {displaySnapshot.EnhancedWasPaddedOrRepaired.ToString(CultureInfo.InvariantCulture)}");

        if (enhancedPower != null && resolvedEnhanced != null)
        {
            builder.AppendLine($"- duplicate active damage rows in pEnh: {CountDuplicateActiveDamageRows(enhancedPower)}");
            builder.AppendLine($"- duplicate active damage rows only after ResolvePower: {(CountDuplicateActiveDamageRows(enhancedPower) == 0 && CountDuplicateActiveDamageRows(resolvedEnhanced) > 0).ToString(CultureInfo.InvariantCulture)}");
            builder.AppendLine($"- string/value mismatch in pEnh: {HasDamageStringValueMismatch(enhancedPower).ToString(CultureInfo.InvariantCulture)}");
            builder.AppendLine($"- string/value mismatch after ResolvePower: {HasDamageStringValueMismatch(resolvedEnhanced).ToString(CultureInfo.InvariantCulture)}");
        }

        builder.AppendLine();
    }

    private static void AppendMainUiDisplayPath(StringBuilder builder, IPower rawPower, bool include)
    {
        if (!include)
        {
            return;
        }

        builder.AppendLine("### Main UI Display Path");
        builder.AppendLine();

        var form = Application.OpenForms.OfType<MainWindow2>().FirstOrDefault(window => !window.IsDisposed);
        if (form == null)
        {
            builder.AppendLine("- No open MainWindow2 instance found.");
            builder.AppendLine();
            return;
        }

        var build = MidsContext.Character?.CurrentBuild;
        var historyIndex = build == null ? -1 : FindActiveBuildHistoryIndex(build, rawPower);

        try
        {
            var snapshot = form.InvokeRequired
                ? (DamageDisplayDebugSnapshot?)form.Invoke(new Func<DamageDisplayDebugSnapshot?>(() =>
                    form.CaptureUiDamageSnapshotForPower(rawPower.PowerIndex, historyIndex)))
                : form.CaptureUiDamageSnapshotForPower(rawPower.PowerIndex, historyIndex);

            if (snapshot == null)
            {
                builder.AppendLine("- Live UI snapshot unavailable.");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"- Control type: `{snapshot.ControlType}`");
            builder.AppendLine($"- Base power: `{snapshot.BasePowerFullName}`");
            builder.AppendLine($"- Enhanced power: `{snapshot.EnhancedPowerFullName}`");
            builder.AppendLine($"- Base effect count: {snapshot.BaseEffectCount}");
            builder.AppendLine($"- Enhanced effect count: {snapshot.EnhancedEffectCount}");
            builder.AppendLine($"- Branch: `{snapshot.Branch}`");
            builder.AppendLine($"- baseDamage: {Format(snapshot.BaseDamage)}");
            builder.AppendLine($"- enhancedDamage: {Format(snapshot.EnhancedDamage)}");
            builder.AppendLine($"- baseDamage(absorb:true): {Format(snapshot.BaseDamageAbsorbTrue)}");
            builder.AppendLine($"- enhancedDamage(absorb:true): {Format(snapshot.EnhancedDamageAbsorbTrue)}");
            builder.AppendLine($"- Base FXGetDamageString: `{PlannerMathDiagnosticRunner.Escape(snapshot.BaseDamageString)}`");
            builder.AppendLine($"- Enhanced FXGetDamageString: `{PlannerMathDiagnosticRunner.Escape(snapshot.EnhancedDamageString)}`");
            if (!string.Equals(snapshot.BaseDamageStringAbsorbTrue, snapshot.BaseDamageString, StringComparison.Ordinal) ||
                !string.Equals(snapshot.EnhancedDamageStringAbsorbTrue, snapshot.EnhancedDamageString, StringComparison.Ordinal))
            {
                builder.AppendLine($"- Base FXGetDamageString(absorb:true): `{PlannerMathDiagnosticRunner.Escape(snapshot.BaseDamageStringAbsorbTrue)}`");
                builder.AppendLine($"- Enhanced FXGetDamageString(absorb:true): `{PlannerMathDiagnosticRunner.Escape(snapshot.EnhancedDamageStringAbsorbTrue)}`");
            }

            builder.AppendLine($"- Display text assigned: `{PlannerMathDiagnosticRunner.Escape(snapshot.DisplayText)}`");
            builder.AppendLine($"- Tooltip text assigned: `{PlannerMathDiagnosticRunner.Escape(snapshot.ToolTipText)}`");
            builder.AppendLine($"- Display contains duplicate identical damage terms: {snapshot.DisplayContainsDuplicateIdenticalTerms.ToString(CultureInfo.InvariantCulture)}");
            builder.AppendLine($"- Base has absorbed rows: {snapshot.BaseHasAbsorbedRows.ToString(CultureInfo.InvariantCulture)}");
            builder.AppendLine($"- Enhanced has absorbed rows: {snapshot.EnhancedHasAbsorbedRows.ToString(CultureInfo.InvariantCulture)}");
            builder.AppendLine($"- UI damage row identities: {(snapshot.DamageRowIdentities.Count == 0 ? "(none)" : string.Join(", ", snapshot.DamageRowIdentities.Select(EscapeTable)))}");
            builder.AppendLine($"- UI grouped effect identities: {(snapshot.GroupedEffectIdentities.Count == 0 ? "(none)" : string.Join(", ", snapshot.GroupedEffectIdentities.Select(EscapeTable)))}");
        }
        catch (Exception ex)
        {
            builder.AppendLine($"- Live UI snapshot failed: {PlannerMathDiagnosticRunner.Escape(ex.GetType().Name)}: {PlannerMathDiagnosticRunner.Escape(ex.Message)}");
        }

        builder.AppendLine();
    }

    private sealed record InspectionStage(
        string Name,
        IPower? Power,
        IReadOnlyList<string> DamageRowIdentities,
        IReadOnlyList<string> GroupedEffectIdentities,
        float DamageValue,
        string DamageString,
        string DamageTip,
        int DuplicateDamageRowCount,
        bool HelperMismatch,
        bool DisplayDuplicateTerms,
        bool Cloned,
        bool Resolved,
        bool Absorbed,
        bool Executed,
        bool Redirected,
        bool Rebuilt);

    private static void AppendWholeSystemPipelineInspection(StringBuilder builder, IPower rawPower, bool include)
    {
        if (!include)
        {
            return;
        }

        builder.AppendLine("### Whole-System Pipeline Inspection");
        builder.AppendLine();

        var toon = global::Mids_Reborn.MainModule.MidsController.Toon;
        var build = MidsContext.Character?.CurrentBuild;
        if (toon == null || build == null)
        {
            builder.AppendLine("- Toon/build unavailable.");
            builder.AppendLine();
            return;
        }

        var historyIndex = FindActiveBuildHistoryIndex(build, rawPower);
        if (historyIndex < 0)
        {
            builder.AppendLine("- Power is not currently picked in the active build; whole-system picked inspection skipped.");
            builder.AppendLine();
            return;
        }

        var databasePower = rawPower.PowerIndex >= 0 && rawPower.PowerIndex < DatabaseAPI.Database.Power.Length
            ? DatabaseAPI.Database.Power[rawPower.PowerIndex]
            : DatabaseAPI.GetPowerByFullName(rawPower.FullName);
        var assemblyPower = InvokeAssemblyStage(toon, rawPower.PowerIndex, historyIndex);
        var rawBasePower = TryGetToonPowerArrayEntry(toon, "_basePowers", historyIndex);
        var assembledBasePower = TryGetToonPowerArrayEntry(toon, "_assembledBasePowers", historyIndex);
        var mathPower = TryGetToonPowerArrayEntry(toon, "_mathPowers", historyIndex);
        var preBuffPower = TryGetToonPowerArrayEntry(toon, "_preBuffPowers", historyIndex);
        var buffedPower = TryGetToonPowerArrayEntry(toon, "_buffedPowers", historyIndex);
        var basePower = toon.GetBasePower(historyIndex);
        var enhancedPower = toon.GetEnhancedPower(historyIndex);
        var displaySnapshot = toon.GetDisplayPowerSnapshot(historyIndex, rawPower.PowerIndex);
        var uiSnapshot = CaptureUiSnapshot(rawPower.PowerIndex, historyIndex);

        var stages = new List<InspectionStage>
        {
            CaptureInspectionStage("Raw DB power", databasePower),
            CaptureInspectionStage("_basePowers[hIDX]", rawBasePower),
            CaptureInspectionStage("GBPA_SubPass0_AssemblePowerEntry", assemblyPower, cloned: true, rebuilt: true),
            CaptureInspectionStage("_assembledBasePowers[hIDX]", assembledBasePower, absorbed: assembledBasePower?.AbsorbedPetEffects == true, executed: assembledBasePower?.AppliedExecutes == true, redirected: assembledBasePower?.AppliedPowersOverride == true),
            CaptureInspectionStage("_mathPowers[hIDX]", mathPower, absorbed: mathPower?.AbsorbedPetEffects == true, executed: mathPower?.AppliedExecutes == true, redirected: mathPower?.AppliedPowersOverride == true),
            CaptureInspectionStage("_preBuffPowers[hIDX]", preBuffPower, absorbed: preBuffPower?.AbsorbedPetEffects == true, executed: preBuffPower?.AppliedExecutes == true, redirected: preBuffPower?.AppliedPowersOverride == true),
            CaptureInspectionStage("_buffedPowers[hIDX]", buffedPower, absorbed: buffedPower?.AbsorbedPetEffects == true, executed: buffedPower?.AppliedExecutes == true, redirected: buffedPower?.AppliedPowersOverride == true),
            CaptureInspectionStage("GetBasePower()", basePower, rebuilt: !ReferenceEquals(basePower, rawBasePower)),
            CaptureInspectionStage("GetEnhancedPower()", enhancedPower, rebuilt: !ReferenceEquals(enhancedPower, buffedPower)),
            CaptureInspectionStage("GetDisplayPowerSnapshot().BasePower", displaySnapshot.BasePower, cloned: displaySnapshot.BasePower != null, resolved: displaySnapshot.BaseWasResolvedForDisplay, rebuilt: displaySnapshot.BaseWasPaddedOrRepaired),
            CaptureInspectionStage("GetDisplayPowerSnapshot().EnhancedPower", displaySnapshot.EnhancedPower, cloned: displaySnapshot.EnhancedPower != null, resolved: displaySnapshot.EnhancedWasResolvedForDisplay, rebuilt: displaySnapshot.EnhancedWasPaddedOrRepaired)
        };

        foreach (var stage in stages)
        {
            AppendInspectionStage(builder, stage);
        }

        if (uiSnapshot != null)
        {
            builder.AppendLine("- MidsDataViewNeo input snapshot:");
            builder.AppendLine($"  - Base power: `{uiSnapshot.BasePowerFullName}` effects={uiSnapshot.BaseEffectCount}");
            builder.AppendLine($"  - Enhanced power: `{uiSnapshot.EnhancedPowerFullName}` effects={uiSnapshot.EnhancedEffectCount}");
            builder.AppendLine($"  - Damage rows: {(uiSnapshot.DamageRowIdentities.Count == 0 ? "(none)" : string.Join(", ", uiSnapshot.DamageRowIdentities.Select(EscapeTable)))}");
            builder.AppendLine($"  - Grouped effects: {(uiSnapshot.GroupedEffectIdentities.Count == 0 ? "(none)" : string.Join(", ", uiSnapshot.GroupedEffectIdentities.Select(EscapeTable)))}");
            builder.AppendLine($"  - Display text: `{PlannerMathDiagnosticRunner.Escape(uiSnapshot.DisplayText)}`");
            builder.AppendLine($"  - Tooltip text: `{PlannerMathDiagnosticRunner.Escape(uiSnapshot.ToolTipText)}`");
            builder.AppendLine($"  - Duplicate visible terms: {uiSnapshot.DisplayContainsDuplicateIdenticalTerms.ToString(CultureInfo.InvariantCulture)}");
        }
        else
        {
            builder.AppendLine("- MidsDataViewNeo input snapshot: unavailable");
        }

        AppendFirstDivergenceSummary(builder, stages, uiSnapshot);
        builder.AppendLine();
    }

    private static void AppendInspectionStage(StringBuilder builder, InspectionStage stage)
    {
        if (stage.Power == null)
        {
            builder.AppendLine($"- {stage.Name}: `(none)`");
            return;
        }

        builder.AppendLine($"- {stage.Name}: `{stage.Power.FullName}` effects={stage.Power.Effects.Length} damage={Format(stage.DamageValue)} duplicates={stage.DuplicateDamageRowCount}");
        builder.AppendLine($"  - identity: powerIndex={stage.Power.PowerIndex}, staticIndex={stage.Power.StaticIndex}, level={stage.Power.Level}, stacks={stage.Power.Stacks}");
        builder.AppendLine($"  - flags: cloned={stage.Cloned.ToString(CultureInfo.InvariantCulture)}, resolved={stage.Resolved.ToString(CultureInfo.InvariantCulture)}, absorbed={stage.Absorbed.ToString(CultureInfo.InvariantCulture)}, executed={stage.Executed.ToString(CultureInfo.InvariantCulture)}, redirected={stage.Redirected.ToString(CultureInfo.InvariantCulture)}, rebuilt={stage.Rebuilt.ToString(CultureInfo.InvariantCulture)}");
        builder.AppendLine($"  - helper agreement: mismatch={stage.HelperMismatch.ToString(CultureInfo.InvariantCulture)}, duplicateVisibleTerms={stage.DisplayDuplicateTerms.ToString(CultureInfo.InvariantCulture)}");
        builder.AppendLine($"  - FXGetDamageString: `{PlannerMathDiagnosticRunner.Escape(stage.DamageString)}`");
        builder.AppendLine($"  - GetDamageTip: `{PlannerMathDiagnosticRunner.Escape(stage.DamageTip)}`");
        builder.AppendLine($"  - damage rows: {(stage.DamageRowIdentities.Count == 0 ? "(none)" : string.Join(", ", stage.DamageRowIdentities.Select(EscapeTable)))}");
        builder.AppendLine($"  - grouped effects: {(stage.GroupedEffectIdentities.Count == 0 ? "(none)" : string.Join(", ", stage.GroupedEffectIdentities.Select(EscapeTable)))}");
    }

    private static InspectionStage CaptureInspectionStage(
        string name,
        IPower? power,
        bool cloned = false,
        bool resolved = false,
        bool absorbed = false,
        bool executed = false,
        bool redirected = false,
        bool rebuilt = false)
    {
        if (power == null)
        {
            return new InspectionStage(name, null, Array.Empty<string>(), Array.Empty<string>(), 0f, string.Empty, string.Empty, 0, false, false, cloned, resolved, absorbed, executed, redirected, rebuilt);
        }

        var damageRows = Power.GetIncludedDamageEffects(power)
            .Select(GetExactDuplicateKey)
            .ToArray();
        var groupedRows = GroupedFx.AggregateGroupedEffectsPass2(power, GroupedFx.AssembleGroupedEffects(power))
            .Select(group => BuildGroupedEffectIdentity(group, power))
            .ToArray();
        var damageValue = power.FXGetDamageValue();
        var damageString = power.FXGetDamageString();
        var damageTip = power.GetDamageTip();
        var helperTotal = Power.GetIncludedDamageEffects(power).Sum(effect => Power.GetDamageEffectTotal(effect, power, absolute: true, applyReturnScaling: true));

        return new InspectionStage(
            name,
            power,
            damageRows,
            groupedRows,
            damageValue,
            damageString,
            damageTip,
            damageRows.GroupBy(x => x, StringComparer.OrdinalIgnoreCase).Sum(group => Math.Max(0, group.Count() - 1)),
            Math.Abs(helperTotal - damageValue) > 0.001f,
            DamageDisplayDebugSnapshot.DetectDuplicateIdenticalDamageTerms(damageString),
            cloned,
            resolved,
            absorbed,
            executed,
            redirected,
            rebuilt);
    }

    private static string BuildGroupedEffectIdentity(GroupedFx groupedFx, IPower power)
    {
        var rankedEffects = power.GetRankedEffects(true);
        var rankedIndex = groupedFx.GetRankedEffectIndex(rankedEffects, 0);
        return string.Join("|",
            groupedFx.EffectType,
            groupedFx.DamageType,
            groupedFx.ToWho,
            groupedFx.PvMode,
            groupedFx.NumEffects,
            rankedIndex,
            groupedFx.GetTooltip(power, simple: true));
    }

    private static IPower? InvokeAssemblyStage(object toon, int powerIndex, int historyIndex)
    {
        if (powerIndex < 0)
        {
            return null;
        }

        var method = toon.GetType().GetMethod("GBPA_SubPass0_AssemblePowerEntry", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (method == null)
        {
            return null;
        }

        try
        {
            return method.Invoke(toon, new object[] { powerIndex, historyIndex, -1 }) as IPower;
        }
        catch
        {
            return null;
        }
    }

    private static DamageDisplayDebugSnapshot? CaptureUiSnapshot(int powerIndex, int historyIndex)
    {
        var form = Application.OpenForms.OfType<MainWindow2>().FirstOrDefault(window => !window.IsDisposed);
        if (form == null)
        {
            return null;
        }

        try
        {
            return form.InvokeRequired
                ? (DamageDisplayDebugSnapshot?)form.Invoke(new Func<DamageDisplayDebugSnapshot?>(() => form.CaptureUiDamageSnapshotForPower(powerIndex, historyIndex)))
                : form.CaptureUiDamageSnapshotForPower(powerIndex, historyIndex);
        }
        catch
        {
            return null;
        }
    }

    private static void AppendFirstDivergenceSummary(StringBuilder builder, IReadOnlyList<InspectionStage> stages, DamageDisplayDebugSnapshot? uiSnapshot)
    {
        builder.AppendLine("- First divergence summary:");

        var firstDuplicate = stages.FirstOrDefault(stage => stage.DuplicateDamageRowCount > 0);
        builder.AppendLine(firstDuplicate != null
            ? $"  - First duplicate damage row phase: `{firstDuplicate.Name}`"
            : "  - First duplicate damage row phase: `(none)`");

        var firstHelperMismatch = stages.FirstOrDefault(stage => stage.HelperMismatch);
        builder.AppendLine(firstHelperMismatch != null
            ? $"  - First helper mismatch phase: `{firstHelperMismatch.Name}`"
            : "  - First helper mismatch phase: `(none)`");

        InspectionStage? firstDamageRowSetChange = null;
        for (var i = 1; i < stages.Count; i++)
        {
            if (stages[i - 1].Power == null || stages[i].Power == null)
            {
                continue;
            }

            if (!stages[i - 1].DamageRowIdentities.SequenceEqual(stages[i].DamageRowIdentities, StringComparer.OrdinalIgnoreCase))
            {
                firstDamageRowSetChange = stages[i];
                break;
            }
        }

        builder.AppendLine(firstDamageRowSetChange != null
            ? $"  - First damage-row set change: `{firstDamageRowSetChange.Name}`"
            : "  - First damage-row set change: `(none)`");

        InspectionStage? firstGroupedSetChange = null;
        for (var i = 1; i < stages.Count; i++)
        {
            if (stages[i - 1].Power == null || stages[i].Power == null)
            {
                continue;
            }

            if (!stages[i - 1].GroupedEffectIdentities.SequenceEqual(stages[i].GroupedEffectIdentities, StringComparer.OrdinalIgnoreCase))
            {
                firstGroupedSetChange = stages[i];
                break;
            }
        }

        builder.AppendLine(firstGroupedSetChange != null
            ? $"  - First grouped-effect set change: `{firstGroupedSetChange.Name}`"
            : "  - First grouped-effect set change: `(none)`");

        if (uiSnapshot != null)
        {
            builder.AppendLine(uiSnapshot.DisplayContainsDuplicateIdenticalTerms
                ? "  - MidsDataViewNeo visible text still contains duplicate identical terms."
                : "  - MidsDataViewNeo visible text does not contain duplicate identical terms.");

            var finalStage = stages.LastOrDefault(stage => stage.Power != null);
            if (finalStage != null &&
                (!finalStage.DamageRowIdentities.SequenceEqual(uiSnapshot.DamageRowIdentities, StringComparer.OrdinalIgnoreCase) ||
                 !finalStage.GroupedEffectIdentities.SequenceEqual(uiSnapshot.GroupedEffectIdentities, StringComparer.OrdinalIgnoreCase)))
            {
                builder.AppendLine("  - UI input snapshot diverges from final Toon/display snapshot identities.");
            }
            else
            {
                builder.AppendLine("  - UI input snapshot matches the final inspected power identities.");
            }
        }
    }

    private static void AppendEffectMismatchSummary(StringBuilder builder, string title, IPower left, IPower right)
    {
        var comparable = Math.Min(left.Effects.Length, right.Effects.Length);
        var typeMismatches = 0;
        var modifierMismatches = 0;
        var valueMismatches = 0;
        for (var i = 0; i < comparable; i++)
        {
            if (left.Effects[i].EffectType != right.Effects[i].EffectType)
            {
                typeMismatches++;
            }

            if (!string.Equals(left.Effects[i].ModifierTable, right.Effects[i].ModifierTable, StringComparison.OrdinalIgnoreCase))
            {
                modifierMismatches++;
            }

            if (Math.Abs(left.Effects[i].BuffedMag - right.Effects[i].BuffedMag) > 0.001f)
            {
                valueMismatches++;
            }
        }

        builder.AppendLine($"- {title}: type mismatches={typeMismatches}, modifier-table mismatches={modifierMismatches}, value mismatches={valueMismatches}");
    }

    private static int FindActiveBuildHistoryIndex(Build build, IPower rawPower)
    {
        var powerIndex = rawPower.PowerIndex;
        var fullName = rawPower.FullName;

        var historyIndex = build.Powers.FindIndex(p => p != null && (
            (powerIndex >= 0 && p.NIDPower == powerIndex) ||
            (p.Power != null && p.Power.PowerIndex == powerIndex && powerIndex >= 0) ||
            (p.Power != null && p.Power.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase))));

        return historyIndex;
    }

    private static IPower? TryGetToonPowerArrayEntry(object toon, string fieldName, int historyIndex)
    {
        var field = toon.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field?.GetValue(toon) is not IPower?[] powers || historyIndex < 0 || historyIndex >= powers.Length)
        {
            return null;
        }

        return powers[historyIndex];
    }

    private static Enums.BuffsX? TryGetToonBuffsXField(object toon, string fieldName)
    {
        var field = toon.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field?.GetValue(toon) is Enums.BuffsX buffs)
        {
            return buffs;
        }

        return null;
    }

    private static void AppendPipelineStage(StringBuilder builder, string stageName, IPower? power)
    {
        if (power == null)
        {
            builder.AppendLine($"- {stageName}: `(none)`");
            return;
        }

        var includedDamageRows = GetIncludedDamageRows(power).ToList();
        var duplicateActiveDamageRows = includedDamageRows
            .GroupBy(item => GetExactDuplicateKey(item), StringComparer.OrdinalIgnoreCase)
            .Count(group => group.Count() > 1);

        var damageValue = power.FXGetDamageValue();
        var damageString = power.FXGetDamageString();
        var absorbedDamageValue = power.FXGetDamageValue(absorb: true);
        var absorbedDamageString = power.FXGetDamageString(absorb: true);

        builder.AppendLine(
            $"- {stageName}: `{power.FullName}` effects={power.Effects.Length}, includedDamageRows={includedDamageRows.Count}, duplicateActiveDamageRows={duplicateActiveDamageRows}, FXGetDamageValue={Format(damageValue)}, FXGetDamageString=`{PlannerMathDiagnosticRunner.Escape(damageString)}`");

        if (Math.Abs(absorbedDamageValue - damageValue) > 0.001f ||
            !string.Equals(absorbedDamageString, damageString, StringComparison.Ordinal))
        {
            builder.AppendLine(
                $"  - absorb=True branch: FXGetDamageValue={Format(absorbedDamageValue)}, FXGetDamageString=`{PlannerMathDiagnosticRunner.Escape(absorbedDamageString)}`");
        }

        foreach (var effect in includedDamageRows.Take(6))
        {
            builder.AppendLine(
                $"  - `{effect.DamageType}` mag={Format(effect.BuffedMag)} ticks={Format(Power.GetDamageEffectEffectiveTicks(effect))} pvx=`{effect.PvMode}` target=`{effect.ToWho}` source={FormatOmniSource(effect)} duplicateKey=`{EscapeTable(GetExactDuplicateKey(effect))}`");
        }
    }

    private static void AppendBucketSummary(StringBuilder builder, string stageName, Enums.BuffsX? buffs)
    {
        if (buffs == null)
        {
            builder.AppendLine($"- {stageName}: `(none)`");
            return;
        }

        var data = buffs.Value;
        var topDamage = data.Damage
            .Select((value, index) => new { Value = value, Index = index })
            .Where(item => Math.Abs(item.Value) > 0.0001f)
            .OrderByDescending(item => Math.Abs(item.Value))
            .Take(3)
            .Select(item => $"{(Enums.eDamage)item.Index}={Format(item.Value)}")
            .ToArray();

        builder.AppendLine(
            $"- {stageName}: BuffAcc={Format(data.Effect[(int)Enums.eStatType.BuffAcc])}, ToHit={Format(data.Effect[(int)Enums.eStatType.ToHit])}, Haste={Format(data.Effect[(int)Enums.eStatType.Haste])}, Range={Format(data.Effect[(int)Enums.eStatType.Range])}, HPRegen={Format(data.Effect[(int)Enums.eStatType.HPRegen])}, EndRec={Format(data.Effect[(int)Enums.eStatType.EndRec])}, MaxEnd={Format(data.MaxEnd)}, DamageBuckets={(topDamage.Length == 0 ? "(none)" : string.Join(", ", topDamage))}");
    }

    private static IEnumerable<IEffect> GetIncludedDamageRows(IPower power)
    {
        return Power.GetIncludedDamageEffects(power, absorb: false);
    }

    private static int CountDuplicateActiveDamageRows(IPower power)
    {
        return GetIncludedDamageRows(power)
            .GroupBy(GetExactDuplicateKey, StringComparer.OrdinalIgnoreCase)
            .Where(group => PlannerStackRules.ShouldFlagDuplicateLookingGroup(group.ToArray()))
            .Sum(group => Math.Max(0, group.Count() - 1));
    }

    private static bool HasDamageStringValueMismatch(IPower power)
    {
        var total = GetIncludedDamageRows(power).Sum(effect => Power.GetDamageEffectTotal(effect, power, absolute: true, applyReturnScaling: true));
        return Math.Abs(total - power.FXGetDamageValue()) > 0.001f;
    }

    private static bool ContainsDiagnosticName(IPower power, params string[] values)
    {
        return values.Any(value =>
            power.FullName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
            power.DisplayName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
            power.PowerName.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private static string FormatRecurrence(IEffect effect)
    {
        var recurrence = effect.PseudoPetRecurrence;
        return recurrence is { IsValid: true }
            ? $"{recurrence.TicksPerSpawn}x{recurrence.SpawnCount}/{Format(recurrence.SourceUsageTime)}s"
            : string.Empty;
    }

    private static string GetEffectConditionIdentity(IEffect effect)
    {
        return string.Join(";",
            effect.AdvancedConditions.Rows
                .Select(row => $"{row.EvaluationMode}|{row.Kind}|{row.Link}|{row.Negated}|{AdvancedConditionCompiler.Compile(row)}")
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase));
    }

    private static string FormatRedirectVariant(IEffect effect, IPower? selectedRedirect)
    {
        var target = effect.nOverride >= 0 && effect.nOverride < DatabaseAPI.Database.Power.Length
            ? DatabaseAPI.Database.Power[effect.nOverride]?.FullName ?? effect.Override
            : effect.Override;
        var selected = selectedRedirect != null && string.Equals(selectedRedirect.FullName, target, StringComparison.OrdinalIgnoreCase)
            ? "selected"
            : "inactive";
        var conditions = effect.AdvancedConditions.Rows.Count == 0
            ? "no advanced conditions"
            : FormatConditionRows(effect);

        return $"`{target}` ({selected}) chance={Format(effect.BaseProbability)} conditions={PlannerMathDiagnosticRunner.Escape(conditions)}";
    }

    private static string FormatConditionRows(IEffect effect)
    {
        var parts = new List<string>();
        for (var i = 0; i < effect.AdvancedConditions.Rows.Count; i++)
        {
            var row = effect.AdvancedConditions.Rows[i];
            var link = i == 0 ? string.Empty : row.Link == AdvancedConditionLink.Or ? " OR " : " AND ";
            parts.Add($"{link}{FriendlyConditionText(row)} [{EvaluateConditionForReport(effect, row)}]");
        }

        return string.Concat(parts);
    }

    private static string FriendlyConditionText(AdvancedConditionRow row)
    {
        var subject = row.Subject;
        if (row.Kind == AdvancedConditionKind.SourceMode && PlannerModeMapper.TryGetPlannerMode(subject, out var plannerMode))
        {
            subject = PlannerModeMapper.ToDisplayName(plannerMode);
        }

        return row.Kind switch
        {
            AdvancedConditionKind.SourceMode => $"{(row.Negated ? "Not " : string.Empty)}Mode {subject}",
            AdvancedConditionKind.SourceOwnPower => $"{(row.Negated ? "Does not have power " : "Has power ")}{subject}",
            AdvancedConditionKind.PowerTaken => $"{(row.Negated ? "Power not taken " : "Power taken ")}{subject}",
            AdvancedConditionKind.BooleanLiteral => ParseConditionBooleanLiteral(row) ? "Always true" : "Always false",
            _ => AdvancedConditionCompiler.Compile(row)
        };
    }

    private static bool ParseConditionBooleanLiteral(AdvancedConditionRow row)
    {
        var literalValue = bool.TryParse(row.Value, out var boolValue)
            ? boolValue
            : row.Value == "1";
        return row.Negated ? !literalValue : literalValue;
    }

    private static string EvaluateConditionForReport(IEffect? effect, AdvancedConditionRow row)
    {
        if (row.EvaluationMode == AdvancedConditionEvaluationMode.RuntimeTargetOnly)
        {
            return "non-blocking-runtime";
        }

        if (row.EvaluationMode == AdvancedConditionEvaluationMode.ReportOnly)
        {
            return "non-blocking-report";
        }

        if (effect == null)
        {
            return "not-evaluated";
        }

        try
        {
            return AdvancedConditionEvaluator.EvaluateRow(effect, row) ? "pass" : "fail";
        }
        catch (Exception ex)
        {
            return $"error:{ex.GetType().Name}";
        }
    }

    private static string GetModifierAudit(IEffect effect)
    {
        var className = DatabaseAPI.ResolveModifierClassName(effect);
        var canonical = DatabaseAPI.TryGetClassModifier(className, effect.ModifierTable, MidsContext.MathLevelBase, out var value);
        var source = canonical ? "canonical" : "fallback";
        return $"`{effect.ModifierTable}` {source}:{Format(value)}";
    }

    private static IEnumerable<string> GetEffectTags(IEffect effect)
    {
        if (!string.IsNullOrWhiteSpace(effect.EffectId))
        {
            yield return effect.EffectId;
        }

        foreach (var tag in effect.EffectTags.Where(t => !string.IsNullOrWhiteSpace(t)))
        {
            yield return tag;
        }
    }

    private static string FormatTagsAndReward(IEffect effect)
    {
        var values = GetEffectTags(effect).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (!string.IsNullOrWhiteSpace(effect.Reward))
        {
            values.Add($"reward:{effect.Reward}");
        }

        if (!string.IsNullOrWhiteSpace(effect.ModeName))
        {
            values.Add($"mode:{effect.ModeName}");
        }

        if (!string.IsNullOrWhiteSpace(effect.Summon))
        {
            values.Add($"power:{effect.Summon}");
        }

        if (!string.IsNullOrWhiteSpace(effect.Override))
        {
            values.Add($"override:{effect.Override}");
        }

        return EscapeTable(string.Join(", ", values.Distinct(StringComparer.OrdinalIgnoreCase)));
    }

    private static bool IsOverlapRelevant(IEffect effect)
    {
        return effect.EffectType is not Enums.eEffectType.Null
            and not Enums.eEffectType.NullBool
            and not Enums.eEffectType.DesignerStatus
            and not Enums.eEffectType.PowerRedirect
            and not Enums.eEffectType.GrantPower
            and not Enums.eEffectType.ExecutePower
            and not Enums.eEffectType.EntCreate;
    }

    private static string GetOverlapKey(IEffect effect)
    {
        return string.Join("|",
            effect.EffectType,
            effect.ToWho,
            effect.AttribType,
            effect.Aspect,
            effect.DamageType,
            effect.MezType,
            effect.ModifierTable,
            Format(effect.nDuration),
            Format(effect.DelayedTime),
            NormalizeConditionSet(effect.AdvancedConditions),
            string.Join(",", GetEffectTags(effect).OrderBy(v => v, StringComparer.OrdinalIgnoreCase)),
            PlannerStackRules.GetDiagnosticPolicyKey(effect));
    }

    private static string GetExactDuplicateKey(IEffect effect)
    {
        return PlannerStackRules.GetPlannerExactKey(effect);
    }

    private static string FormatOverlapLabel(IEffect effect)
    {
        var subtype = effect.EffectType switch
        {
            Enums.eEffectType.Damage => effect.DamageType.ToString(),
            Enums.eEffectType.Mez => effect.MezType.ToString(),
            _ => effect.AttribType.ToString()
        };

        return $"`{effect.EffectType}` `{subtype}` target=`{effect.ToWho}` table=`{effect.ModifierTable}` duration={Format(effect.nDuration)} stack=`{EscapeTable(PlannerStackRules.GetDiagnosticDescription(effect))}`";
    }

    private static string FormatOmniSource(IEffect effect)
    {
        return string.IsNullOrWhiteSpace(effect.OmniSource)
            ? string.Empty
            : EscapeTable(effect.OmniSource);
    }

    private static string NormalizeConditionSet(AdvancedConditionSet? conditions)
    {
        if (conditions == null || conditions.Rows.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(";", conditions.Rows.Select(GetConditionDiagnosticKey).OrderBy(v => v, StringComparer.OrdinalIgnoreCase));
    }

    private static string FormatList(IEnumerable<string> values)
    {
        var list = values.Where(v => !string.IsNullOrWhiteSpace(v)).Take(30).ToList();
        return list.Count == 0 ? "(none)" : string.Join(", ", list.Select(v => $"`{v}`"));
    }

    private static string FirstNonEmpty(params string[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
    }

    private static string SafeEffectString(IEffect effect)
    {
        try
        {
            return effect.BuildEffectString(Simple: true, ignoreConditions: true);
        }
        catch (Exception ex)
        {
            return $"Effect string failed: {ex.GetType().Name}: {ex.Message}";
        }
    }

    private static string Format(float value)
    {
        return value.ToString("0.####", CultureInfo.InvariantCulture);
    }

    private static string EscapeTable(string? value)
    {
        return PlannerMathDiagnosticRunner.Escape(value).Replace("|", "\\|", StringComparison.Ordinal);
    }
}

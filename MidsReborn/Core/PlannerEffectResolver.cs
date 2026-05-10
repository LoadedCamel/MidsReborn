using System.Text;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.PlannerRulesets;

namespace Mids_Reborn.Core;

public sealed class PlannerEffectResolutionContext
{
    public const int DefaultMaxExpansionDepth = 5;

    public PlannerEffectResolutionContext(bool useRulesetDefaults = true)
    {
        if (!useRulesetDefaults)
        {
            return;
        }

        var defaults = DatabaseAPI.GetPlannerRuleset().CreateDefaultResolutionContext();
        HistoryIndex = defaults.HistoryIndex;
        StackingOverride = defaults.StackingOverride;
        ApplyRedirects = defaults.ApplyRedirects;
        AbsorbPetEffects = defaults.AbsorbPetEffects;
        ExpandGrantPowers = defaults.ExpandGrantPowers;
        ExpandExecutePowers = defaults.ExpandExecutePowers;
        IncludeTrace = defaults.IncludeTrace;
        MaxExpansionDepth = defaults.MaxExpansionDepth;
    }

    public int HistoryIndex { get; init; } = -1;
    public int StackingOverride { get; init; } = -1;
    public bool ApplyRedirects { get; init; }
    public bool AbsorbPetEffects { get; init; }
    public bool ExpandGrantPowers { get; init; }
    public bool ExpandExecutePowers { get; init; }
    public bool IncludeTrace { get; init; }
    public int MaxExpansionDepth { get; init; } = DefaultMaxExpansionDepth;
}

public sealed class PlannerEffectTraceEntry
{
    public PlannerEffectTraceEntry(string step, string message)
    {
        Step = step;
        Message = message;
    }

    public string Step { get; }
    public string Message { get; }

    public override string ToString()
    {
        return $"{Step}: {Message}";
    }
}

public enum PlannerEffectExpansionKind
{
    GrantPower,
    ExecutePower
}

public enum PlannerEffectExpansionStatus
{
    Expanded,
    PreservedRuntimeOrTarget,
    PreservedUnresolved,
    PreservedEmptyTarget,
    PreservedMaxDepth
}

public sealed class PlannerEffectExpansionEvent
{
    public PlannerEffectExpansionEvent(
        PlannerEffectExpansionKind kind,
        PlannerEffectExpansionStatus status,
        string ownerPower,
        string targetPower,
        Enums.eToWho target,
        float chance,
        float ppm,
        float delay,
        float duration,
        int conditionCount,
        int childEffectCount)
    {
        Kind = kind;
        Status = status;
        OwnerPower = ownerPower;
        TargetPower = targetPower;
        Target = target;
        Chance = chance;
        Ppm = ppm;
        Delay = delay;
        Duration = duration;
        ConditionCount = conditionCount;
        ChildEffectCount = childEffectCount;
    }

    public PlannerEffectExpansionKind Kind { get; }
    public PlannerEffectExpansionStatus Status { get; }
    public string OwnerPower { get; }
    public string TargetPower { get; }
    public Enums.eToWho Target { get; }
    public float Chance { get; }
    public float Ppm { get; }
    public float Delay { get; }
    public float Duration { get; }
    public int ConditionCount { get; }
    public int ChildEffectCount { get; }
}

public sealed class PlannerEffectResolution
{
    public PlannerEffectResolution(
        IPower resolvedPower,
        IPower? selectedRedirectPower,
        IReadOnlyList<PlannerEffectTraceEntry> trace,
        IReadOnlyList<PlannerEffectExpansionEvent> expansionEvents)
    {
        ResolvedPower = resolvedPower;
        SelectedRedirectPower = selectedRedirectPower;
        Trace = trace;
        ExpansionEvents = expansionEvents;
    }

    public IPower ResolvedPower { get; }
    public IPower? SelectedRedirectPower { get; }
    public IReadOnlyList<PlannerEffectTraceEntry> Trace { get; }
    public IReadOnlyList<PlannerEffectExpansionEvent> ExpansionEvents { get; }

    public string FormatTrace()
    {
        if (Trace.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        foreach (var entry in Trace)
        {
            builder.AppendLine(entry.ToString());
        }

        return builder.ToString();
    }
}

public static class PlannerEffectResolver
{
    public static PlannerEffectResolution ResolvePower(IPower sourcePower, PlannerEffectResolutionContext? context = null)
    {
        context ??= new PlannerEffectResolutionContext();
        var trace = new List<PlannerEffectTraceEntry>();
        var expansionEvents = new List<PlannerEffectExpansionEvent>();
        var resolvedPower = new Power(sourcePower);
        IPower? selectedRedirectPower = null;

        if (context.ApplyRedirects && !resolvedPower.AppliedPowersOverride)
        {
            var redirected = ResolveRedirect(resolvedPower, trace);
            if (redirected != null)
            {
                var level = resolvedPower.Level;
                var stacks = resolvedPower.Stacks;
                selectedRedirectPower = redirected;
                resolvedPower = new Power(redirected)
                {
                    Level = level,
                    Stacks = stacks
                };
            }

            resolvedPower.AppliedPowersOverride = true;
        }

        PlannerResolvedEffectSemantics.MarkBaseEffects(resolvedPower.Effects);

        if (context.AbsorbPetEffects && !resolvedPower.AbsorbedPetEffects)
        {
            trace.Add(new PlannerEffectTraceEntry("EntCreate", $"Absorbing pseudo pet effects for {resolvedPower.FullName}."));
            resolvedPower.AbsorbPetEffects(context.HistoryIndex, context.StackingOverride, pseudoOnly: true);
        }

        ExpandEffects(resolvedPower, context, trace, expansionEvents);
        SetEffectPower(resolvedPower);

        return new PlannerEffectResolution(
            resolvedPower,
            selectedRedirectPower,
            context.IncludeTrace ? trace : Array.Empty<PlannerEffectTraceEntry>(),
            context.IncludeTrace ? expansionEvents : Array.Empty<PlannerEffectExpansionEvent>());
    }

    public static IPower ApplyRedirect(IPower sourcePower, List<PlannerEffectTraceEntry>? trace = null)
    {
        var redirected = ResolveRedirect(sourcePower, trace);
        if (redirected == null)
        {
            return sourcePower;
        }

        var level = sourcePower.Level;
        var stacks = sourcePower.Stacks;
        return new Power(redirected)
        {
            Level = level,
            Stacks = stacks
        };
    }

    public static void ExpandEffects(
        IPower power,
        PlannerEffectResolutionContext? context = null,
        List<PlannerEffectTraceEntry>? trace = null,
        List<PlannerEffectExpansionEvent>? expansionEvents = null)
    {
        context ??= new PlannerEffectResolutionContext();

        var expanded = power.Effects.Select(CloneEffect).ToList();
        PlannerResolvedEffectSemantics.MarkBaseEffects(expanded);
        if (context.ExpandGrantPowers)
        {
            expanded = ExpandGrantPowerEffects(power, expanded, context, trace, expansionEvents, 0);
        }

        if (context.ExpandExecutePowers && !power.AppliedExecutes)
        {
            expanded = ExpandExecutePowerEffects(power, expanded, context, trace, expansionEvents, 0);
        }

        power.Effects = expanded.ToArray();
        power.HasGrantPowerEffect = power.Effects.Any(effect => effect.EffectType == Enums.eEffectType.GrantPower);
        power.AppliedExecutes = context.ExpandExecutePowers;
        SetEffectPower(power);
    }

    public static AdvancedConditionSet MergeConditions(AdvancedConditionSet? parent, AdvancedConditionSet? child)
    {
        var merged = new AdvancedConditionSet();
        AppendRows(merged, parent);
        AppendRows(merged, child);
        return merged;
    }

    public static void InheritEffectMetadata(IEffect parent, IEffect child, bool inheritToWho = true, IPower? owner = null)
    {
        if (inheritToWho &&
            child.EffectType != Enums.eEffectType.GrantPower &&
            parent.EffectType == Enums.eEffectType.GrantPower)
        {
            child.ToWho = parent.ToWho;
        }
        else if (inheritToWho &&
                 child.EffectType != Enums.eEffectType.GrantPower &&
                 child.ToWho == Enums.eToWho.Unspecified)
        {
            child.ToWho = parent.ToWho;
        }

        child.isEnhancementEffect = parent.isEnhancementEffect;
        child.BaseProbability = ClampProbability(child.BaseProbability * parent.BaseProbability);
        child.DelayedTime += parent.DelayedTime;
        if (child.PvMode == Enums.ePvX.Any && parent.PvMode != Enums.ePvX.Any)
        {
            child.PvMode = parent.PvMode;
        }
        child.Absorbed_Effect = child.Absorbed_Effect || parent.Absorbed_Effect;
        child.Absorbed_Power_nID = parent.Absorbed_Power_nID > -1 ? parent.Absorbed_Power_nID : child.Absorbed_Power_nID;
        child.Absorbed_PowerType = parent.Absorbed_PowerType;
        child.Absorbed_Class_nID = parent.Absorbed_Class_nID > -1 ? parent.Absorbed_Class_nID : child.Absorbed_Class_nID;

        if (parent.ProcsPerMinute > 0 && child.ProcsPerMinute <= 0)
        {
            child.ProcsPerMinute = parent.ProcsPerMinute;
        }

        if (parent.isEnhancementEffect &&
            (parent.IgnoreScaling || parent.IsFromProc) &&
            child is Effect childEffect)
        {
            childEffect.ProcContributionFlags |= ProcContributionFlags.InheritedFromProcWrapper |
                                                 ProcContributionFlags.ChildResultView;
            if (parent.EffectType == Enums.eEffectType.ExecutePower)
            {
                childEffect.ProcContributionFlags |= ProcContributionFlags.ChainExpandedView;
            }
        }

        if (!string.IsNullOrWhiteSpace(parent.EffectId) && string.IsNullOrWhiteSpace(child.EffectId))
        {
            child.EffectId = parent.EffectId;
        }

        foreach (var tag in parent.EffectTags.Where(tag => !string.IsNullOrWhiteSpace(tag)))
        {
            if (!child.EffectTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                child.EffectTags.Add(tag);
            }
        }

        if (parent.Ticks > 0 && child.Ticks == 0)
        {
            child.Ticks = parent.Ticks;
        }

        if (parent.nDuration > 0)
        {
            child.nDuration = parent.nDuration;
        }

        if (parent.AdvancedConditions is { Rows.Count: > 0 })
        {
            child.AdvancedConditions = MergeConditions(parent.AdvancedConditions, child.AdvancedConditions);
            child.ActiveConditionals = child.AdvancedConditions.ToLegacyActiveConditionals();
        }
        else if (parent.ActiveConditionals is { Count: > 0 })
        {
            var inherited = AdvancedConditionSet.FromLegacyActiveConditionals(parent.ActiveConditionals);
            child.AdvancedConditions = MergeConditions(inherited, child.AdvancedConditions);
            child.ActiveConditionals = child.AdvancedConditions.ToLegacyActiveConditionals();
        }

        if (owner != null &&
            parent.EffectType == Enums.eEffectType.GrantPower &&
            parent.GrantBoosted)
        {
            child.Absorbed_Effect = true;
            child.Absorbed_Power_nID = owner.PowerIndex > -1 ? owner.PowerIndex : child.Absorbed_Power_nID;
            child.Absorbed_PowerType = owner.PowerType;
            if (child.Absorbed_Class_nID < 0 && owner.GetPowerSet() is { nArchetype: >= 0 } powerset)
            {
                child.Absorbed_Class_nID = powerset.nArchetype;
            }
        }
    }

    private static List<IEffect> ExpandGrantPowerEffects(
        IPower owner,
        IReadOnlyList<IEffect> sourceEffects,
        PlannerEffectResolutionContext context,
        List<PlannerEffectTraceEntry>? trace,
        List<PlannerEffectExpansionEvent>? expansionEvents,
        int depth)
    {
        if (depth > context.MaxExpansionDepth)
        {
            trace?.Add(new PlannerEffectTraceEntry("GrantPower", $"{owner.FullName}: max expansion depth reached; preserving remaining grants."));
            foreach (var grant in sourceEffects.Where(e => e.EffectType == Enums.eEffectType.GrantPower))
            {
                expansionEvents?.Add(CreateExpansionEvent(PlannerEffectExpansionKind.GrantPower, PlannerEffectExpansionStatus.PreservedMaxDepth, owner, grant, childEffectCount: 0));
            }

            return sourceEffects.Select(CloneEffect).ToList();
        }

        var expanded = new List<IEffect>();
        foreach (var effect in sourceEffects)
        {
            if (effect.EffectType != Enums.eEffectType.GrantPower)
            {
                expanded.Add(CloneEffect(effect));
                continue;
            }

            if (!IsBuildRelevantGrant(effect))
            {
                trace?.Add(new PlannerEffectTraceEntry("GrantPower", $"{owner.FullName}: preserving target/runtime grant '{FormatPowerReference(effect)}'."));
                expansionEvents?.Add(CreateExpansionEvent(PlannerEffectExpansionKind.GrantPower, PlannerEffectExpansionStatus.PreservedRuntimeOrTarget, owner, effect, childEffectCount: 0));
                expanded.Add(CloneEffect(effect));
                continue;
            }

            if (!TryResolveReferencedPower(effect, out var grantedPower))
            {
                trace?.Add(new PlannerEffectTraceEntry("GrantPower", $"{owner.FullName}: unresolved grant target '{FormatPowerReference(effect)}'; preserving wrapper."));
                expansionEvents?.Add(CreateExpansionEvent(PlannerEffectExpansionKind.GrantPower, PlannerEffectExpansionStatus.PreservedUnresolved, owner, effect, childEffectCount: 0));
                expanded.Add(CloneEffect(effect));
                continue;
            }

            var childEffects = new Power(grantedPower).Effects.Select(CloneEffect).ToList();
            if (childEffects.Count == 0)
            {
                trace?.Add(new PlannerEffectTraceEntry("GrantPower", $"{owner.FullName}: grant target {grantedPower.FullName} has no effects; preserving wrapper."));
                expansionEvents?.Add(CreateExpansionEvent(PlannerEffectExpansionKind.GrantPower, PlannerEffectExpansionStatus.PreservedEmptyTarget, owner, effect, childEffectCount: 0));
                expanded.Add(CloneEffect(effect));
                continue;
            }

            childEffects = ExpandGrantPowerEffects(grantedPower, childEffects, context, trace, expansionEvents, depth + 1);
            childEffects = ExpandExecutePowerEffects(grantedPower, childEffects, context, trace, expansionEvents, depth + 1);
            foreach (var child in childEffects)
            {
                InheritEffectMetadata(effect, child, owner: owner);
                child.AddResolvedEffectKind(PlannerResolvedEffectKind.GrantChild);
                child.SetPower(owner);
                expanded.Add(child);
            }

            trace?.Add(new PlannerEffectTraceEntry("GrantPower", $"{owner.FullName}: expanded grant {grantedPower.FullName} into {childEffects.Count} effects."));
            expansionEvents?.Add(CreateExpansionEvent(PlannerEffectExpansionKind.GrantPower, PlannerEffectExpansionStatus.Expanded, owner, effect, childEffects.Count, grantedPower.FullName));
        }

        return expanded;
    }

    private static List<IEffect> ExpandExecutePowerEffects(
        IPower owner,
        IReadOnlyList<IEffect> sourceEffects,
        PlannerEffectResolutionContext context,
        List<PlannerEffectTraceEntry>? trace,
        List<PlannerEffectExpansionEvent>? expansionEvents,
        int depth)
    {
        if (depth > context.MaxExpansionDepth)
        {
            trace?.Add(new PlannerEffectTraceEntry("ExecutePower", $"{owner.FullName}: max expansion depth reached; preserving remaining executes."));
            foreach (var execute in sourceEffects.Where(e => e.EffectType == Enums.eEffectType.ExecutePower))
            {
                expansionEvents?.Add(CreateExpansionEvent(PlannerEffectExpansionKind.ExecutePower, PlannerEffectExpansionStatus.PreservedMaxDepth, owner, execute, childEffectCount: 0));
            }

            return sourceEffects.Select(CloneEffect).ToList();
        }

        var expanded = new List<IEffect>();
        foreach (var effect in sourceEffects)
        {
            if (effect.EffectType != Enums.eEffectType.ExecutePower)
            {
                expanded.Add(CloneEffect(effect));
                continue;
            }

            if (!TryResolveReferencedPower(effect, out var executedPower))
            {
                trace?.Add(new PlannerEffectTraceEntry("ExecutePower", $"{owner.FullName}: unresolved execute target '{FormatPowerReference(effect)}'; preserving wrapper."));
                expansionEvents?.Add(CreateExpansionEvent(PlannerEffectExpansionKind.ExecutePower, PlannerEffectExpansionStatus.PreservedUnresolved, owner, effect, childEffectCount: 0));
                expanded.Add(CloneEffect(effect));
                continue;
            }

            var childEffects = new Power(executedPower).Effects.Select(CloneEffect).ToList();
            if (childEffects.Count == 0)
            {
                trace?.Add(new PlannerEffectTraceEntry("ExecutePower", $"{owner.FullName}: execute target {executedPower.FullName} has no effects; preserving wrapper."));
                expansionEvents?.Add(CreateExpansionEvent(PlannerEffectExpansionKind.ExecutePower, PlannerEffectExpansionStatus.PreservedEmptyTarget, owner, effect, childEffectCount: 0));
                expanded.Add(CloneEffect(effect));
                continue;
            }

            childEffects = ExpandGrantPowerEffects(executedPower, childEffects, context, trace, expansionEvents, depth + 1);
            childEffects = ExpandExecutePowerEffects(executedPower, childEffects, context, trace, expansionEvents, depth + 1);
            foreach (var child in childEffects)
            {
                InheritEffectMetadata(effect, child);
                child.AddResolvedEffectKind(PlannerResolvedEffectKind.ExecuteChild);
                child.SetPower(owner);
                expanded.Add(child);
            }

            trace?.Add(new PlannerEffectTraceEntry("ExecutePower", $"{owner.FullName}: expanded execute {executedPower.FullName} into {childEffects.Count} effects."));
            expansionEvents?.Add(CreateExpansionEvent(PlannerEffectExpansionKind.ExecutePower, PlannerEffectExpansionStatus.Expanded, owner, effect, childEffects.Count, executedPower.FullName));
        }

        return expanded;
    }

    private static bool IsBuildRelevantGrant(IEffect effect)
    {
        if (effect.ToWho == Enums.eToWho.Target)
        {
            return false;
        }

        return effect.AdvancedConditions.Rows.All(row =>
            row.EvaluationMode != AdvancedConditionEvaluationMode.RuntimeTargetOnly);
    }

    private static bool TryResolveReferencedPower(IEffect effect, out IPower power)
    {
        power = null!;
        if (effect.nSummon >= 0 && effect.nSummon < DatabaseAPI.Database.Power.Length)
        {
            power = DatabaseAPI.Database.Power[effect.nSummon];
            return power != null;
        }

        foreach (var reference in new[] { effect.Summon, effect.RevokedPower, effect.Override }.Where(value => !string.IsNullOrWhiteSpace(value)))
        {
            var resolved = DatabaseAPI.GetPowerByFullName(reference);
            if (resolved != null)
            {
                power = resolved;
                return true;
            }
        }

        return false;
    }

    private static string FormatPowerReference(IEffect effect)
    {
        if (!string.IsNullOrWhiteSpace(effect.Summon))
        {
            return effect.Summon;
        }

        if (!string.IsNullOrWhiteSpace(effect.RevokedPower))
        {
            return effect.RevokedPower;
        }

        if (!string.IsNullOrWhiteSpace(effect.Override))
        {
            return effect.Override;
        }

        return effect.nSummon >= 0 ? $"#{effect.nSummon}" : "(none)";
    }

    private static PlannerEffectExpansionEvent CreateExpansionEvent(
        PlannerEffectExpansionKind kind,
        PlannerEffectExpansionStatus status,
        IPower owner,
        IEffect wrapper,
        int childEffectCount,
        string? resolvedTarget = null)
    {
        return new PlannerEffectExpansionEvent(
            kind,
            status,
            owner.FullName,
            string.IsNullOrWhiteSpace(resolvedTarget) ? FormatPowerReference(wrapper) : resolvedTarget,
            wrapper.ToWho,
            wrapper.BaseProbability,
            wrapper.ProcsPerMinute,
            wrapper.DelayedTime,
            wrapper.nDuration,
            wrapper.AdvancedConditions.Rows.Count,
            childEffectCount);
    }

    private static IEffect CloneEffect(IEffect effect)
    {
        return (IEffect)effect.Clone();
    }

    private static IPower? ResolveRedirect(IPower sourcePower, List<PlannerEffectTraceEntry>? trace)
    {
        if (!sourcePower.HasPowerOverrideEffect)
        {
            return null;
        }

        var candidates = sourcePower.Effects
            .Where(fx => fx.EffectType == Enums.eEffectType.PowerRedirect && fx.nOverride > -1)
            .ToList();

        if (candidates.Count == 0)
        {
            trace?.Add(new PlannerEffectTraceEntry("Redirect", $"{sourcePower.FullName} has no resolvable redirect target."));
            return null;
        }

        var active = candidates
            .Where(fx => fx.PvXInclude() && AdvancedConditionEvaluator.Evaluate(fx))
            .ToList();

        IEffect? selected = null;
        if (active.Count == 1)
        {
            selected = active[0];
        }
        else if (active.Count > 1)
        {
            selected = active[0];
            trace?.Add(new PlannerEffectTraceEntry("Redirect", $"{sourcePower.FullName} matched {active.Count} redirects; using first import-order match {selected.Override}."));
        }
        else
        {
            selected = candidates.FirstOrDefault(fx =>
                fx.AdvancedConditions.Rows.Count == 0 &&
                (fx.ActiveConditionals == null || fx.ActiveConditionals.Count == 0) &&
                fx.SpecialCase == Enums.eSpecialCase.None);

            if (selected == null)
            {
                trace?.Add(new PlannerEffectTraceEntry("Redirect", $"{sourcePower.FullName} has redirects, but none are active in the current planner context."));
                return null;
            }
        }

        var redirectPower = DatabaseAPI.Database.Power[selected.nOverride];
        trace?.Add(new PlannerEffectTraceEntry("Redirect", $"{sourcePower.FullName} resolves to {redirectPower.FullName}."));
        return redirectPower;
    }

    private static void AppendRows(AdvancedConditionSet target, AdvancedConditionSet? source)
    {
        if (source is not { Rows.Count: > 0 })
        {
            return;
        }

        foreach (var row in source.Rows)
        {
            var clone = row.Clone();
            if (target.Rows.Count > 0)
            {
                clone.Link = AdvancedConditionLink.And;
            }

            target.Rows.Add(clone);
        }
    }

    private static float ClampProbability(float value)
    {
        return Math.Max(0, Math.Min(1, value));
    }

    private static void SetEffectPower(IPower power)
    {
        foreach (var effect in power.Effects)
        {
            effect.SetPower(power);
        }
    }
}

using System.Globalization;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.Core.PlannerRulesets;

namespace Mids_Reborn.Core;

internal sealed record DefianceContributorDescriptor(
    string SourcePowerFullName,
    string SourceDisplayName,
    string ResolvedPowerFullName,
    string ResolvedDisplayName,
    float Magnitude,
    float Duration,
    int MaxActiveCount);

internal sealed record DefianceContributorState(
    DefianceContributorDescriptor Descriptor,
    int ActiveCount);

internal sealed class DefianceResolution
{
    public IReadOnlyList<DefianceContributorState> Contributors { get; init; } = Array.Empty<DefianceContributorState>();
    public float TotalMagnitude { get; init; }
    public int ActiveContributorCount { get; init; }
}

internal static class DefiancePlanner
{
    private const string SyntheticPowerFullName = "Planner.Combat.Defiance_Current";
    private const string SyntheticPayloadTag = "PlannerDefianceComputed";

    private static readonly Enums.eDamage[] TypedDamageBuffTypes =
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

    internal static IReadOnlyList<Enums.eDamage> ComputedBuffDamageTypes => TypedDamageBuffTypes;

    public static bool IsModernContributorEffect(IEffect? effect)
    {
        if (effect == null ||
            effect.EffectType != Enums.eEffectType.DamageBuff ||
            effect.ToWho != Enums.eToWho.Self ||
            effect.DamageType == Enums.eDamage.None ||
            effect.DamageType == Enums.eDamage.Special ||
            effect.BuffedMag <= float.Epsilon)
        {
            return false;
        }

        return effect.SpecialCase == Enums.eSpecialCase.Defiance ||
               effect.ValidateConditional("Active", "Defiance") ||
               effect.EffectTags.Contains("Defiance", StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsComputedCurrentBuffEffect(IEffect? effect)
    {
        return effect != null &&
               effect.EffectType == Enums.eEffectType.DamageBuff &&
               effect.EffectTags.Contains(SyntheticPayloadTag, StringComparer.OrdinalIgnoreCase);
    }

    public static DefianceResolution Resolve(Build? build, ConfigData.CombatContext.DefianceSettings? settings)
    {
        var descriptors = Discover(build);
        if (settings != null)
        {
            SynchronizeSelections(settings, descriptors);
        }

        var contributors = new List<DefianceContributorState>(descriptors.Count);
        var totalMagnitude = 0f;
        var activeContributorCount = 0;
        foreach (var descriptor in descriptors)
        {
            var activeCount = settings == null ? 0 : GetActiveCount(settings, descriptor);
            contributors.Add(new DefianceContributorState(descriptor, activeCount));
            if (activeCount <= 0)
            {
                continue;
            }

            activeContributorCount++;
            totalMagnitude += descriptor.Magnitude * activeCount;
        }

        return new DefianceResolution
        {
            Contributors = contributors,
            TotalMagnitude = totalMagnitude,
            ActiveContributorCount = activeContributorCount
        };
    }

    public static IPower? BuildCurrentBuffPower(Build? build, ConfigData.CombatContext.DefianceSettings? settings)
    {
        var resolution = Resolve(build, settings);
        if (resolution.TotalMagnitude <= float.Epsilon)
        {
            return null;
        }

        var power = new Power
        {
            FullName = SyntheticPowerFullName,
            GroupName = "Planner",
            SetName = "Combat",
            PowerName = "Defiance_Current",
            DisplayName = "Defiance",
            HiddenPower = true,
            IncludeFlag = true,
            PowerType = Enums.ePowerType.Auto_,
            Effects = []
        };

        power.Effects =
        [
            new Effect(power)
            {
                PowerFullName = power.FullName,
                UniqueID = 1,
                EffectClass = Enums.eEffectClass.Primary,
                EffectType = Enums.eEffectType.DamageBuff,
                ToWho = Enums.eToWho.Self,
                DamageType = Enums.eDamage.None,
                AttribType = Enums.eAttribType.Expression,
                BaseProbability = 1f,
                Stacking = Enums.eStacking.No,
                EffectTags = [SyntheticPayloadTag],
                MagnitudeExpression = resolution.TotalMagnitude.ToString("0.#######", CultureInfo.InvariantCulture),
                Expressions = new Expressions
                {
                    Magnitude = resolution.TotalMagnitude.ToString("0.#######", CultureInfo.InvariantCulture)
                }
            }
        ];

        foreach (var effect in power.Effects)
        {
            effect.SetPower(power);
        }

        return power;
    }

    public static void SetActiveCount(
        ConfigData.CombatContext.DefianceSettings settings,
        DefianceContributorDescriptor descriptor,
        int requestedCount)
    {
        settings.Contributors ??= [];
        var clampedCount = Math.Max(0, Math.Min(descriptor.MaxActiveCount, requestedCount));
        settings.Contributors.RemoveAll(entry =>
            entry.SourcePowerFullName.Equals(descriptor.SourcePowerFullName, StringComparison.OrdinalIgnoreCase));

        if (clampedCount <= 0)
        {
            return;
        }

        settings.Contributors.Add(new ConfigData.CombatContext.DefianceContributorSelection
        {
            SourcePowerFullName = descriptor.SourcePowerFullName,
            ResolvedPowerFullName = descriptor.ResolvedPowerFullName,
            ActiveCount = clampedCount
        });
    }

    private static List<DefianceContributorDescriptor> Discover(Build? build)
    {
        var descriptors = new List<DefianceContributorDescriptor>();
        if (build?.Powers == null)
        {
            return descriptors;
        }

        for (var historyIndex = 0; historyIndex < build.Powers.Count; historyIndex++)
        {
            var powerEntry = build.Powers[historyIndex];
            if (powerEntry == null ||
                powerEntry.NIDPower < 0)
            {
                continue;
            }

            var sourcePower = GetSourceIdentityPower(powerEntry);
            if (sourcePower == null)
            {
                continue;
            }

            if (sourcePower.FullName.Equals(PlannerStateCatalog.DefiancePowerFullName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var (resolvedPower, resolvedPowerFullName, resolvedDisplayName) = ResolveContributorPower(build, powerEntry, historyIndex);
            var contributorGroups = resolvedPower.Effects
                .Where(IsModernContributorEffect)
                .GroupBy(GetContributorGroupingKey, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.OrderByDescending(effect => Math.Abs(effect.BuffedMag)).First())
                .ToArray();
            if (contributorGroups.Length == 0)
            {
                continue;
            }

            var magnitude = contributorGroups.Sum(effect => effect.BuffedMag);
            if (magnitude <= float.Epsilon)
            {
                continue;
            }

            var duration = contributorGroups.Max(effect => effect.Duration);
            var distinctLimits = contributorGroups
                .Select(GetMaxActiveCount)
                .Distinct()
                .ToArray();
            var maxActiveCount = distinctLimits.Length == 1 ? distinctLimits[0] : 1;

            descriptors.Add(new DefianceContributorDescriptor(
                sourcePower.FullName,
                sourcePower.DisplayName,
                resolvedPowerFullName,
                resolvedDisplayName,
                magnitude,
                duration,
                maxActiveCount));
        }

        return descriptors
            .OrderBy(descriptor => descriptor.SourceDisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IPower? GetSourceIdentityPower(PowerEntry powerEntry)
    {
        if (powerEntry.Power != null)
        {
            return powerEntry.Power;
        }

        return TryGetCanonicalDatabasePower(powerEntry);
    }

    private static (IPower Power, string FullName, string DisplayName) ResolveContributorPower(
        Build build,
        PowerEntry powerEntry,
        int historyIndex)
    {
        var sourcePower = GetSourceIdentityPower(powerEntry)
                         ?? throw new InvalidOperationException("Selected Defiance contributor is missing its source power identity.");
        var canonicalSourcePower = TryGetCanonicalDatabasePower(powerEntry) ?? sourcePower;

        var liveResolvedPower = TryGetLiveResolvedPower(build, historyIndex);
        if (HasContributorEffects(liveResolvedPower))
        {
            return (
                liveResolvedPower!,
                liveResolvedPower!.FullName,
                liveResolvedPower.DisplayName);
        }

        var rawPower = new Power(canonicalSourcePower)
        {
            Level = sourcePower.Level,
            Stacks = sourcePower.Stacks,
            AppliedPowersOverride = false
        };

        var resolution = PlannerEffectResolver.ResolvePower(rawPower, new PlannerEffectResolutionContext
        {
            HistoryIndex = historyIndex,
            ApplyRedirects = true,
            AbsorbPetEffects = false,
            ExpandGrantPowers = false,
            ExpandExecutePowers = false
        });

        var resolvedPower = resolution.ResolvedPower;
        return (
            resolvedPower,
            resolution.SelectedRedirectPower?.FullName ?? resolvedPower.FullName,
            resolution.SelectedRedirectPower?.DisplayName ?? resolvedPower.DisplayName);
    }

    private static IPower? TryGetCanonicalDatabasePower(PowerEntry powerEntry)
    {
        var powerIndex = powerEntry.NIDPower;
        if (powerIndex < 0 || powerIndex >= DatabaseAPI.Database.Power.Length)
        {
            return null;
        }

        return DatabaseAPI.Database.Power[powerIndex];
    }

    private static IPower? TryGetLiveResolvedPower(Build build, int historyIndex)
    {
        if (MidsContext.Character is not Toon toon ||
            !ReferenceEquals(toon.CurrentBuild, build) ||
            historyIndex < 0 ||
            historyIndex >= build.Powers.Count)
        {
            return null;
        }

        return toon.TryGetAssembledBasePower(historyIndex);
    }

    private static bool HasContributorEffects(IPower? power)
    {
        return power?.Effects.Any(IsModernContributorEffect) == true;
    }

    private static string GetContributorGroupingKey(IEffect effect)
    {
        if (!string.IsNullOrWhiteSpace(effect.EffectId))
        {
            return effect.EffectId;
        }

        if (!string.IsNullOrWhiteSpace(effect.OmniSource))
        {
            return effect.OmniSource;
        }

        var stackPolicy = effect is Effect concreteEffect ? concreteEffect.StackPolicy : ImportedStackPolicy.Default;
        return string.Join("|",
            effect.EffectType,
            effect.ToWho,
            effect.BuffedMag.ToString("0.#######", CultureInfo.InvariantCulture),
            effect.Duration.ToString("0.#######", CultureInfo.InvariantCulture),
            stackPolicy.Mode,
            stackPolicy.StackLimit);
    }

    private static int GetMaxActiveCount(IEffect effect)
    {
        if (effect is Effect concreteEffect)
        {
            var policy = concreteEffect.StackPolicy;
            if (policy.HasImportedMetadata)
            {
                return policy.Mode switch
                {
                    ImportedStackMode.Replace or ImportedStackMode.Ignore or ImportedStackMode.Extend or ImportedStackMode.Overlap or ImportedStackMode.Maximize or ImportedStackMode.Suppress => 1,
                    ImportedStackMode.Stack or ImportedStackMode.StackThenIgnore or ImportedStackMode.Refresh or ImportedStackMode.RefreshToCount or ImportedStackMode.Continuous => Math.Max(1, policy.StackLimit > 0 ? policy.StackLimit : 1),
                    _ => Math.Max(1, policy.StackLimit)
                };
            }
        }

        return effect.Stacking == Enums.eStacking.Yes ? 2 : 1;
    }

    private static int GetActiveCount(
        ConfigData.CombatContext.DefianceSettings settings,
        DefianceContributorDescriptor descriptor)
    {
        settings.Contributors ??= [];

        var exact = settings.Contributors.FirstOrDefault(entry =>
            entry.SourcePowerFullName.Equals(descriptor.SourcePowerFullName, StringComparison.OrdinalIgnoreCase) &&
            entry.ResolvedPowerFullName.Equals(descriptor.ResolvedPowerFullName, StringComparison.OrdinalIgnoreCase));
        var sourceFallback = exact ?? settings.Contributors.FirstOrDefault(entry =>
            entry.SourcePowerFullName.Equals(descriptor.SourcePowerFullName, StringComparison.OrdinalIgnoreCase));

        return Math.Max(0, Math.Min(descriptor.MaxActiveCount, sourceFallback?.ActiveCount ?? 0));
    }

    private static void SynchronizeSelections(
        ConfigData.CombatContext.DefianceSettings settings,
        IReadOnlyList<DefianceContributorDescriptor> descriptors)
    {
        settings.Contributors ??= [];

        var synchronized = new List<ConfigData.CombatContext.DefianceContributorSelection>(descriptors.Count);
        foreach (var descriptor in descriptors)
        {
            var activeCount = GetActiveCount(settings, descriptor);
            if (activeCount <= 0)
            {
                continue;
            }

            synchronized.Add(new ConfigData.CombatContext.DefianceContributorSelection
            {
                SourcePowerFullName = descriptor.SourcePowerFullName,
                ResolvedPowerFullName = descriptor.ResolvedPowerFullName,
                ActiveCount = activeCount
            });
        }

        settings.Contributors = synchronized;
    }
}

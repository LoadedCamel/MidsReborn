using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.PlannerRulesets;

namespace Mids_Reborn.Core;

internal enum ContributionCategory
{
    Unknown,
    Enhancement,
    SelfBuff,
    SetBonus,
    Proc,
    GrantChild,
    ExecuteChild,
    PseudoPetFlattened,
    DeliveryChild,
    OwnerInheritedPetBonus,
    CapAdjustment
}

internal enum CalculationActorKind
{
    Player,
    Pet
}

internal sealed record ContributionRecord(
    ContributionKey? Key,
    double Value,
    ContributionChannel Channel,
    ContributionCategory Category,
    ContributionSource Source,
    string MetricName,
    Enums.eEffectType? EffectType,
    Enums.eEffectType? ETModifies,
    Enums.eDamage? DamageType,
    Enums.eMez? MezType,
    Enums.eAspect? Aspect);

internal sealed class CalculationContributionSnapshot
{
    private readonly IReadOnlyList<ContributionRecord> _records;

    public static CalculationContributionSnapshot Empty { get; } = new(Array.Empty<ContributionRecord>());

    public CalculationContributionSnapshot(IEnumerable<ContributionRecord> records)
    {
        _records = records.ToArray();
    }

    public IReadOnlyList<ContributionRecord> Records => _records;

    public IReadOnlyList<ContributionRecord> GetBucketRecords(ContributionBucket bucket, int index)
    {
        return _records
            .Where(record => record.Key is { } key && key.Bucket == bucket && key.Index == index)
            .ToArray();
    }

    public IEnumerable<(ContributionSource Source, ContributionChannel Channel, double Total)> GetSummed(
        ContributionBucket bucket,
        int index)
    {
        return GetBucketRecords(bucket, index)
            .GroupBy(record => (record.Source, record.Channel))
            .Select(group => (group.Key.Source, group.Key.Channel, group.Sum(record => record.Value)));
    }

    public static CalculationContributionSnapshot Merge(params CalculationContributionSnapshot?[] snapshots)
    {
        return new CalculationContributionSnapshot(
            snapshots
                .Where(snapshot => snapshot != null)
                .SelectMany(snapshot => snapshot!.Records));
    }
}

internal sealed class PowerCalculationSnapshot
{
    public int HistoryIndex { get; init; }
    public IPower? BasePower { get; init; }
    public IPower? AssembledBasePower { get; init; }
    public IPower? MathPower { get; init; }
    public IPower? PreBuffPower { get; init; }
    public IPower? BuffedPower { get; init; }
}

internal sealed class ActorAggregationSnapshot
{
    public IReadOnlyList<IPower> IncludedMathPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> IncludedBuffedPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> EnhancementExternalPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> SelfBuffExternalPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> SupplementalEnhancementSourcePowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> SupplementalSelfBuffSourcePowers { get; init; } = Array.Empty<IPower>();
}

internal sealed class ActorCalculationSnapshot
{
    public CalculationActorKind Kind { get; init; }
    public string ClassName { get; init; } = string.Empty;
    public Archetype? Archetype { get; init; }
    public IReadOnlyList<PowerCalculationSnapshot> PowerSnapshots { get; init; } = Array.Empty<PowerCalculationSnapshot>();
    public IReadOnlyList<IPower> MathPowers { get; init; } = Array.Empty<IPower>();
    public IReadOnlyList<IPower> BuffedPowers { get; init; } = Array.Empty<IPower>();
    public Enums.BuffsX SelfEnhanceBuckets { get; init; }
    public Enums.BuffsX SelfBuffBuckets { get; init; }
    public ActorTotalsSnapshot Totals { get; init; } = null!;
    public Dictionary<string, float> ChanceModifierCatalog { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public CalculationContributionSnapshot Contributions { get; init; } = CalculationContributionSnapshot.Empty;
    public ActorAggregationSnapshot Aggregation { get; init; } = new();
}

internal sealed class BuildCalculationSnapshot
{
    public PlannerCombatContext CombatContext { get; init; } = PlannerCombatContext.Default;
    public ActorCalculationSnapshot PlayerActorSnapshot { get; init; } = null!;
    public IReadOnlyList<PowerCalculationSnapshot> PowerSnapshots { get; init; } = Array.Empty<PowerCalculationSnapshot>();
}

internal sealed class PlannerContributionCollector
{
    private readonly List<ContributionRecord> _records = [];

    public void AddBucketDelta(
        ContributionBucket bucket,
        int index,
        double value,
        IPower sourcePower,
        PlannerBucketPass pass,
        ContributionCategory category,
        string metricName)
    {
        if (Math.Abs(value) < 1e-12)
        {
            return;
        }

        var source = BuildSource(sourcePower);
        _records.Add(new ContributionRecord(
            new ContributionKey(bucket, index),
            value,
            pass == PlannerBucketPass.Enhancement ? ContributionChannel.Enhancements : ContributionChannel.Buffs,
            category,
            source,
            metricName,
            null,
            null,
            null,
            null,
            null));
    }

    public void AddCapAdjustment(string metricName, double value)
    {
        if (Math.Abs(value) < 1e-12)
        {
            return;
        }

        _records.Add(new ContributionRecord(
            null,
            value,
            ContributionChannel.Buffs,
            ContributionCategory.CapAdjustment,
            new ContributionSource("Final Caps", "FinalCaps", Enums.ePowerType.Auto_, true, false, false, false),
            metricName,
            null,
            null,
            null,
            null,
            null));
    }

    public void AddChanceModifierDelta(
        string chanceTag,
        double value,
        IPower sourcePower,
        PlannerBucketPass pass,
        ContributionCategory category)
    {
        if (string.IsNullOrWhiteSpace(chanceTag) || Math.Abs(value) < 1e-12)
        {
            return;
        }

        var source = BuildSource(sourcePower);
        _records.Add(new ContributionRecord(
            null,
            value,
            pass == PlannerBucketPass.Enhancement ? ContributionChannel.Enhancements : ContributionChannel.Buffs,
            category,
            source,
            $"ChanceModifier[{chanceTag}]",
            Enums.eEffectType.GlobalChanceMod,
            null,
            null,
            null,
            null));
    }

    public CalculationContributionSnapshot ToSnapshot()
    {
        return _records.Count == 0
            ? CalculationContributionSnapshot.Empty
            : new CalculationContributionSnapshot(_records);
    }

    private static ContributionSource BuildSource(IPower sourcePower)
    {
        var friendlyName = string.IsNullOrWhiteSpace(sourcePower.DisplayName)
            ? sourcePower.FullName
            : sourcePower.DisplayName;
        var isSetBonusVirtual = string.Equals(sourcePower.FullName, "Mids.SetBonus.Virtual", StringComparison.OrdinalIgnoreCase);
        var isPvpResist = string.Equals(sourcePower.FullName, "Temporary_Powers.Temporary_Powers.PVP_Resist_Bonus", StringComparison.OrdinalIgnoreCase);
        return new ContributionSource(
            friendlyName,
            sourcePower.FullName ?? string.Empty,
            sourcePower.PowerType,
            true,
            isSetBonusVirtual,
            isPvpResist,
            sourcePower.GetPowerSet()?.SetType is Enums.ePowerSetType.Incarnate);
    }
}

internal static class CalculationSnapshotFactory
{
    public static BuildCalculationSnapshot CreateBuildSnapshot(
        PlannerCombatContext combatContext,
        ActorCalculationSnapshot playerActorSnapshot,
        IReadOnlyList<PowerCalculationSnapshot> powerSnapshots)
    {
        return new BuildCalculationSnapshot
        {
            CombatContext = combatContext ?? PlannerCombatContext.Default,
            PlayerActorSnapshot = playerActorSnapshot,
            PowerSnapshots = ClonePowerSnapshots(powerSnapshots)
        };
    }

    public static BuildCalculationSnapshot CreateBuildSnapshot(
        PlannerCombatContext combatContext,
        ActorCalculationSnapshot playerActorSnapshot,
        IReadOnlyList<IPower?> basePowers,
        IReadOnlyList<IPower?> assembledBasePowers,
        IReadOnlyList<IPower?> mathPowers,
        IReadOnlyList<IPower?> preBuffPowers,
        IReadOnlyList<IPower?> buffedPowers)
    {
        return CreateBuildSnapshot(
            combatContext,
            playerActorSnapshot,
            CreatePowerSnapshots(basePowers, assembledBasePowers, mathPowers, preBuffPowers, buffedPowers));
    }

    public static ActorCalculationSnapshot CreateActorSnapshot(
        CalculationActorKind kind,
        string className,
        Archetype? archetype,
        IReadOnlyList<IPower> mathPowers,
        IReadOnlyList<IPower> buffedPowers,
        Enums.BuffsX selfEnhance,
        Enums.BuffsX selfBuffs,
        ActorTotalsSnapshot totals,
        Dictionary<string, float>? chanceModifierCatalog,
        CalculationContributionSnapshot? contributions,
        IReadOnlyList<PowerCalculationSnapshot>? powerSnapshots = null,
        ActorAggregationSnapshot? aggregation = null)
    {
        return new ActorCalculationSnapshot
        {
            Kind = kind,
            ClassName = className,
            Archetype = archetype,
            PowerSnapshots = ClonePowerSnapshots(powerSnapshots ?? CreateActorPowerSnapshots(mathPowers, buffedPowers)),
            MathPowers = ClonePowers(mathPowers),
            BuffedPowers = ClonePowers(buffedPowers),
            SelfEnhanceBuckets = CloneBuckets(selfEnhance),
            SelfBuffBuckets = CloneBuckets(selfBuffs),
            Totals = CloneTotalsSnapshot(totals),
            ChanceModifierCatalog = chanceModifierCatalog == null
                ? new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, float>(chanceModifierCatalog, StringComparer.OrdinalIgnoreCase),
            Contributions = contributions ?? CalculationContributionSnapshot.Empty,
            Aggregation = CloneAggregationSnapshot(aggregation)
        };
    }

    public static ActorTotalsSnapshot CloneTotalsSnapshot(ActorTotalsSnapshot snapshot)
    {
        var totals = new Character.TotalStatistics();
        totals.Assign(snapshot.Totals);
        var totalsCapped = new Character.TotalStatistics();
        totalsCapped.Assign(snapshot.TotalsCapped);
        return new ActorTotalsSnapshot
        {
            ClassName = snapshot.ClassName,
            Totals = totals,
            TotalsCapped = totalsCapped,
            DisplayStats = new ActorDisplayStats(snapshot.ClassName, totals, totalsCapped)
        };
    }

    public static Enums.BuffsX CloneBuckets(Enums.BuffsX source)
    {
        return new Enums.BuffsX
        {
            MaxEnd = source.MaxEnd,
            Effect = CloneArray(source.Effect),
            EffectAux = CloneArray(source.EffectAux),
            Mez = CloneArray(source.Mez),
            MezRes = CloneArray(source.MezRes),
            Damage = CloneArray(source.Damage),
            Defense = CloneArray(source.Defense),
            Resistance = CloneArray(source.Resistance),
            Elusivity = CloneArray(source.Elusivity),
            StatusProtection = CloneArray(source.StatusProtection),
            StatusResistance = CloneArray(source.StatusResistance),
            DebuffResistance = CloneArray(source.DebuffResistance)
        };
    }

    public static IReadOnlyList<PowerCalculationSnapshot> CreatePowerSnapshots(
        IReadOnlyList<IPower?> basePowers,
        IReadOnlyList<IPower?> assembledBasePowers,
        IReadOnlyList<IPower?> mathPowers,
        IReadOnlyList<IPower?> preBuffPowers,
        IReadOnlyList<IPower?> buffedPowers)
    {
        var count = new[] { basePowers.Count, assembledBasePowers.Count, mathPowers.Count, preBuffPowers.Count, buffedPowers.Count }.Max();
        var snapshots = new List<PowerCalculationSnapshot>(count);
        for (var index = 0; index < count; index++)
        {
            snapshots.Add(new PowerCalculationSnapshot
            {
                HistoryIndex = index,
                BasePower = ClonePower(basePowers, index),
                AssembledBasePower = ClonePower(assembledBasePowers, index),
                MathPower = ClonePower(mathPowers, index),
                PreBuffPower = ClonePower(preBuffPowers, index),
                BuffedPower = ClonePower(buffedPowers, index)
            });
        }

        return snapshots;
    }

    public static IReadOnlyList<PowerCalculationSnapshot> CreateActorPowerSnapshots(
        IReadOnlyList<IPower> mathPowers,
        IReadOnlyList<IPower> buffedPowers)
    {
        return CreatePowerSnapshots(
            Array.Empty<IPower?>(),
            Array.Empty<IPower?>(),
            mathPowers.Cast<IPower?>().ToArray(),
            mathPowers.Cast<IPower?>().ToArray(),
            buffedPowers.Cast<IPower?>().ToArray());
    }

    public static ActorAggregationSnapshot CreateAggregationSnapshot(
        IReadOnlyList<IPower> includedMathPowers,
        IReadOnlyList<IPower> includedBuffedPowers,
        IReadOnlyList<IPower> enhancementExternalPowers,
        IReadOnlyList<IPower> selfBuffExternalPowers,
        IReadOnlyList<IPower> supplementalEnhancementSourcePowers,
        IReadOnlyList<IPower> supplementalSelfBuffSourcePowers)
    {
        return new ActorAggregationSnapshot
        {
            IncludedMathPowers = ClonePowers(includedMathPowers),
            IncludedBuffedPowers = ClonePowers(includedBuffedPowers),
            EnhancementExternalPowers = ClonePowers(enhancementExternalPowers),
            SelfBuffExternalPowers = ClonePowers(selfBuffExternalPowers),
            SupplementalEnhancementSourcePowers = ClonePowers(supplementalEnhancementSourcePowers),
            SupplementalSelfBuffSourcePowers = ClonePowers(supplementalSelfBuffSourcePowers)
        };
    }

    private static IReadOnlyList<PowerCalculationSnapshot> ClonePowerSnapshots(IEnumerable<PowerCalculationSnapshot> snapshots)
    {
        return snapshots.Select(snapshot => new PowerCalculationSnapshot
        {
            HistoryIndex = snapshot.HistoryIndex,
            BasePower = snapshot.BasePower == null ? null : new Power(snapshot.BasePower),
            AssembledBasePower = snapshot.AssembledBasePower == null ? null : new Power(snapshot.AssembledBasePower),
            MathPower = snapshot.MathPower == null ? null : new Power(snapshot.MathPower),
            PreBuffPower = snapshot.PreBuffPower == null ? null : new Power(snapshot.PreBuffPower),
            BuffedPower = snapshot.BuffedPower == null ? null : new Power(snapshot.BuffedPower)
        }).ToArray();
    }

    private static ActorAggregationSnapshot CloneAggregationSnapshot(ActorAggregationSnapshot? snapshot)
    {
        if (snapshot == null)
        {
            return new ActorAggregationSnapshot();
        }

        return CreateAggregationSnapshot(
            snapshot.IncludedMathPowers,
            snapshot.IncludedBuffedPowers,
            snapshot.EnhancementExternalPowers,
            snapshot.SelfBuffExternalPowers,
            snapshot.SupplementalEnhancementSourcePowers,
            snapshot.SupplementalSelfBuffSourcePowers);
    }

    private static IReadOnlyList<IPower> ClonePowers(IEnumerable<IPower> powers)
    {
        return powers.Select(power => new Power(power)).Cast<IPower>().ToArray();
    }

    private static IPower? ClonePower(IReadOnlyList<IPower?> powers, int index)
    {
        if (index < 0 || index >= powers.Count || powers[index] == null)
        {
            return null;
        }

        return new Power(powers[index]!);
    }

    private static float[] CloneArray(float[] source)
    {
        return source == null ? Array.Empty<float>() : (float[])source.Clone();
    }
}

internal static class ContributionCategoryResolver
{
    public static ContributionCategory Resolve(IPower sourcePower, PlannerBucketPass pass)
    {
        if (string.Equals(sourcePower.FullName, "Mids.SetBonus.Virtual", StringComparison.OrdinalIgnoreCase))
        {
            return ContributionCategory.SetBonus;
        }

        if (sourcePower.Effects.Any(effect => effect.IsFromProc))
        {
            return ContributionCategory.Proc;
        }

        if (sourcePower.Effects.Any(effect => effect.HasResolvedEffectKind(PlannerResolvedEffectKind.PseudoPetChild) ||
                                              effect.PseudoPetRecurrence is { IsValid: true }))
        {
            return ContributionCategory.PseudoPetFlattened;
        }

        if (sourcePower.Effects.Any(effect => effect.HasResolvedEffectKind(PlannerResolvedEffectKind.GrantChild)))
        {
            return ContributionCategory.GrantChild;
        }

        if (sourcePower.Effects.Any(effect => effect.HasResolvedEffectKind(PlannerResolvedEffectKind.ExecuteChild)))
        {
            return ContributionCategory.ExecuteChild;
        }

        if (sourcePower.Effects.Any(effect => effect.HasResolvedEffectKind(PlannerResolvedEffectKind.DeliveryChild)))
        {
            return ContributionCategory.DeliveryChild;
        }

        return pass == PlannerBucketPass.Enhancement
            ? ContributionCategory.Enhancement
            : ContributionCategory.SelfBuff;
    }
}

internal static class ContributionCapture
{
    public static void AccumulateBuckets(
        PlannerRulesets.IPlannerRuleset ruleset,
        IPower power,
        ref Enums.BuffsX buckets,
        PlannerBucketPass pass,
        PlannerContributionCollector collector)
    {
        var before = CalculationSnapshotFactory.CloneBuckets(buckets);
        ruleset.AccumulateBuckets(power, ref buckets, pass);
        RecordDelta(before, buckets, power, pass, collector);
    }

    public static void RecordChanceModifierContributions(
        IPower power,
        PlannerBucketPass pass,
        PlannerContributionCollector collector,
        bool requireActive = true)
    {
        if (power == null)
        {
            return;
        }

        var category = ContributionCategoryResolver.Resolve(power, pass);
        var modifiers = requireActive
            ? ChanceModifierCatalogBuilder.EnumerateActiveChanceModifiers(power)
            : ChanceModifierCatalogBuilder.EnumerateIncludedChanceModifiers(power);
        foreach (var (tag, magnitude) in modifiers)
        {
            collector.AddChanceModifierDelta(tag, magnitude, power, pass, category);
        }
    }

    public static CalculationContributionSnapshot CreateCapAdjustmentSnapshot(ActorTotalsSnapshot totals)
    {
        var collector = new PlannerContributionCollector();
        collector.AddCapAdjustment("Damage Buff", totals.TotalsCapped.BuffDam - totals.Totals.BuffDam);
        collector.AddCapAdjustment("Recharge Buff", totals.TotalsCapped.BuffHaste - totals.Totals.BuffHaste);
        collector.AddCapAdjustment("Hit Points", totals.TotalsCapped.HPMax - totals.Totals.HPMax);
        collector.AddCapAdjustment("Absorb", totals.TotalsCapped.Absorb - totals.Totals.Absorb);
        collector.AddCapAdjustment("Regeneration", totals.TotalsCapped.HPRegen - totals.Totals.HPRegen);
        collector.AddCapAdjustment("Recovery", totals.TotalsCapped.EndRec - totals.Totals.EndRec);
        collector.AddCapAdjustment("Perception", totals.TotalsCapped.Perception - totals.Totals.Perception);
        collector.AddCapAdjustment("Run Speed", totals.TotalsCapped.RunSpd - totals.Totals.RunSpd);
        collector.AddCapAdjustment("Jump Speed", totals.TotalsCapped.JumpSpd - totals.Totals.JumpSpd);
        collector.AddCapAdjustment("Fly Speed", totals.TotalsCapped.FlySpd - totals.Totals.FlySpd);
        collector.AddCapAdjustment("Jump Height", totals.TotalsCapped.JumpHeight - totals.Totals.JumpHeight);

        for (var index = 0; index < totals.Totals.Res.Length; index++)
        {
            collector.AddCapAdjustment($"Resistance[{(Enums.eDamage)index}]", totals.TotalsCapped.Res[index] - totals.Totals.Res[index]);
        }

        return collector.ToSnapshot();
    }

    private static void RecordDelta(
        Enums.BuffsX before,
        Enums.BuffsX after,
        IPower power,
        PlannerBucketPass pass,
        PlannerContributionCollector collector)
    {
        var category = ContributionCategoryResolver.Resolve(power, pass);
        RecordArrayDelta(before.Effect, after.Effect, ContributionBucket.Effect, "Effect", power, pass, category, collector);
        RecordArrayDelta(before.EffectAux, after.EffectAux, ContributionBucket.EffectAux, "EffectAux", power, pass, category, collector);
        RecordArrayDelta(before.Damage, after.Damage, ContributionBucket.Damage, "Damage", power, pass, category, collector);
        RecordArrayDelta(before.Defense, after.Defense, ContributionBucket.Defense, "Defense", power, pass, category, collector);
        RecordArrayDelta(before.Resistance, after.Resistance, ContributionBucket.Resistance, "Resistance", power, pass, category, collector);
        RecordArrayDelta(before.Elusivity, after.Elusivity, ContributionBucket.Elusivity, "Elusivity", power, pass, category, collector);
        RecordArrayDelta(before.Mez, after.Mez, ContributionBucket.Mez, "Mez", power, pass, category, collector);
        RecordArrayDelta(before.MezRes, after.MezRes, ContributionBucket.MezRes, "MezRes", power, pass, category, collector);
        RecordArrayDelta(before.StatusProtection, after.StatusProtection, ContributionBucket.StatusProtection, "StatusProtection", power, pass, category, collector);
        RecordArrayDelta(before.StatusResistance, after.StatusResistance, ContributionBucket.StatusResistance, "StatusResistance", power, pass, category, collector);
        RecordArrayDelta(before.DebuffResistance, after.DebuffResistance, ContributionBucket.DebuffResistance, "DebuffResistance", power, pass, category, collector);

        var maxEndDelta = after.MaxEnd - before.MaxEnd;
        if (Math.Abs(maxEndDelta) > float.Epsilon)
        {
            collector.AddBucketDelta(ContributionBucket.MaxEnd, 0, maxEndDelta, power, pass, category, "MaxEnd");
        }
    }

    private static void RecordArrayDelta(
        IReadOnlyList<float> before,
        IReadOnlyList<float> after,
        ContributionBucket bucket,
        string metricPrefix,
        IPower power,
        PlannerBucketPass pass,
        ContributionCategory category,
        PlannerContributionCollector collector)
    {
        var count = Math.Min(before.Count, after.Count);
        for (var index = 0; index < count; index++)
        {
            var delta = after[index] - before[index];
            if (Math.Abs(delta) <= float.Epsilon)
            {
                continue;
            }

            collector.AddBucketDelta(bucket, index, delta, power, pass, category, $"{metricPrefix}[{index}]");
        }
    }
}

using System.Globalization;
using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.Core.PlannerRulesets;

namespace Mids_Reborn.Core;

internal enum PowerOutcomeFamily
{
    Damage,
    Heal,
    HitPoints,
    Absorb,
    Endurance,
    Recovery,
    Regeneration,
    ToHit,
    Defense,
    Resistance,
    Mez,
    Recharge,
    EndCost,
    Accuracy,
    Range,
    Interrupt,
    RunSpeed,
    FlySpeed,
    JumpSpeed,
    JumpHeight
}

internal enum PowerOutcomeSourceKind
{
    Enhancement,
    SetBonus,
    IncarnateBonus,
    OtherPowerBuff,
    ConditionalBonus,
    Proc,
    ComputedState,
    MiscExternal
}

internal enum PowerOutcomeLineage
{
    Scalar,
    BasePower,
    ConditionalBonus,
    Proc,
    OtherBonusEffect
}

internal sealed record PowerOutcomeSourceDelta(
    PowerOutcomeSourceKind SourceKind,
    string SourceName,
    string SourceFullName,
    double Value);

internal sealed record PowerOutcomeRow(
    string RowKey,
    PowerOutcomeLineage Lineage,
    double AssembledBaseValue,
    double PreBuffValue,
    double FinalValue,
    string SourceName = "",
    string ConditionName = "",
    Enums.eDamage? DamageType = null,
    Enums.eEffectType? EffectType = null,
    Enums.eMez? MezType = null,
    string OmniSource = "",
    string EffectId = "",
    float Probability = 1f,
    float ProcsPerMinute = 0f,
    float EffectiveTicks = 1f,
    float DurationSeconds = 0f,
    bool IsEnhancementEffect = false,
    bool IsProc = false,
    bool IgnoreScaling = false,
    bool IgnoreBuffs = false,
    bool IgnoreEd = false,
    bool CancelOnMiss = false,
    bool RequiresToHitCheck = false,
    bool SuppressWhenMezzed = false,
    int Occurrences = 1);

internal sealed record PowerOutcomeFamilyReceipt(
    PowerOutcomeFamily Family,
    double AssembledBaseTotal,
    double PreBuffTotal,
    double FinalTotal,
    IReadOnlyList<PowerOutcomeRow> Rows,
    IReadOnlyList<PowerOutcomeSourceDelta> SourceDeltas);

internal sealed class PowerOutcomeReceipt
{
    private readonly IReadOnlyDictionary<PowerOutcomeFamily, PowerOutcomeFamilyReceipt> _families;

    public static PowerOutcomeReceipt Empty { get; } = new(Array.Empty<PowerOutcomeFamilyReceipt>());

    public PowerOutcomeReceipt(IEnumerable<PowerOutcomeFamilyReceipt> families)
    {
        Families = families.ToArray();
        _families = Families.ToDictionary(family => family.Family);
    }

    public IReadOnlyList<PowerOutcomeFamilyReceipt> Families { get; }

    public bool TryGetFamily(PowerOutcomeFamily family, out PowerOutcomeFamilyReceipt receipt)
    {
        return _families.TryGetValue(family, out receipt!);
    }
}

internal static class PowerOutcomeReceiptBuilder
{
    private const double Epsilon = 1e-6;
    private const string RechargeRowKey = "scalar:Recharge";
    private const string EndCostRowKey = "scalar:EndCost";
    private const string AccuracyRowKey = "scalar:Accuracy";
    private const string RangeRowKey = "scalar:Range";
    private const string InterruptRowKey = "scalar:Interrupt";

    public static PowerOutcomeReceipt Build(
        IPower? basePower,
        IPower? assembledBasePower,
        IPower? preBuffPower,
        IPower? buffedPower,
        CalculationContributionSnapshot? contributions)
    {
        var hostFullName = ResolveHostFullName(buffedPower, preBuffPower, assembledBasePower, basePower);
        var hostDisplayName = ResolveHostDisplayName(buffedPower, preBuffPower, assembledBasePower, basePower);
        var families = new List<PowerOutcomeFamilyReceipt>(16);

        AddReceiptIfPresent(families, BuildDamageReceipt(assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));

        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.Heal, Enums.eEffectType.Heal, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.HitPoints, Enums.eEffectType.HitPoints, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.Absorb, Enums.eEffectType.Absorb, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.Endurance, Enums.eEffectType.Endurance, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.Recovery, Enums.eEffectType.Recovery, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.Regeneration, Enums.eEffectType.Regeneration, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.ToHit, Enums.eEffectType.ToHit, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildTypedEffectReceipt(PowerOutcomeFamily.Defense, ContributionBucket.Defense, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildTypedEffectReceipt(PowerOutcomeFamily.Resistance, ContributionBucket.Resistance, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildTypedEffectReceipt(PowerOutcomeFamily.Mez, ContributionBucket.Mez, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));

        AddReceiptIfPresent(families, BuildScalarReceipt(PowerOutcomeFamily.Recharge, RechargeRowKey, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildScalarReceipt(PowerOutcomeFamily.EndCost, EndCostRowKey, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildScalarReceipt(PowerOutcomeFamily.Accuracy, AccuracyRowKey, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildScalarReceipt(PowerOutcomeFamily.Range, RangeRowKey, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildScalarReceipt(PowerOutcomeFamily.Interrupt, InterruptRowKey, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.RunSpeed, Enums.eEffectType.SpeedRunning, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.FlySpeed, Enums.eEffectType.SpeedFlying, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.JumpSpeed, Enums.eEffectType.SpeedJumping, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));
        AddReceiptIfPresent(families, BuildEffectReceipt(PowerOutcomeFamily.JumpHeight, Enums.eEffectType.JumpHeight, assembledBasePower, preBuffPower, buffedPower, contributions, hostFullName, hostDisplayName));

        return families.Count == 0
            ? PowerOutcomeReceipt.Empty
            : new PowerOutcomeReceipt(families);
    }

    private static void AddReceiptIfPresent(List<PowerOutcomeFamilyReceipt> families, PowerOutcomeFamilyReceipt? receipt)
    {
        if (receipt == null)
        {
            return;
        }

        families.Add(receipt);
    }

    private static PowerOutcomeFamilyReceipt? BuildDamageReceipt(
        IPower? assembledBasePower,
        IPower? preBuffPower,
        IPower? buffedPower,
        CalculationContributionSnapshot? contributions,
        string hostFullName,
        string hostDisplayName)
    {
        var assembledRows = BuildDamageStageRows(assembledBasePower, hostFullName);
        var preBuffRows = BuildDamageStageRows(preBuffPower, hostFullName);
        var finalRows = BuildDamageStageRows(buffedPower, hostFullName);
        var mergedRows = MergeStageRows(assembledRows, preBuffRows, finalRows);

        if (mergedRows.Count == 0)
        {
            return null;
        }

        var scaledRows = mergedRows
            .Where(row => row.Lineage is PowerOutcomeLineage.BasePower or PowerOutcomeLineage.ConditionalBonus)
            .ToArray();
        var assembledByDamageType = scaledRows
            .GroupBy(row => (int)(row.DamageType ?? Enums.eDamage.None))
            .ToDictionary(group => group.Key, group => group.Sum(row => row.AssembledBaseValue));
        var actualScaledDelta = scaledRows.Sum(row => row.FinalValue - row.AssembledBaseValue);
        var rawExternalSourceDeltas = BuildBucketScaledSourceDeltas(
            contributions,
            ContributionBucket.Damage,
            assembledByDamageType,
            preBuffPower ?? buffedPower ?? assembledBasePower,
            hostFullName,
            hostDisplayName);
        var recognizedExternalDelta = rawExternalSourceDeltas.Sum(delta => delta.Value);
        var sourceDeltas = new List<PowerOutcomeSourceDelta>();

        // Damage slotting is applied directly onto the host power during the per-power
        // enhancement pass, so it does not appear as a standalone actor contribution
        // record the way set bonuses and other buffs do. Treat the remaining scaled
        // delta as "Enhancements in this power" so the tooltip can separate local
        // slotting from outside buffs without inventing a hover-only math pass.
        var localEnhancementDelta = actualScaledDelta - recognizedExternalDelta;
        if (Math.Abs(localEnhancementDelta) > Epsilon)
        {
            if (localEnhancementDelta > 0d)
            {
                sourceDeltas.Add(new PowerOutcomeSourceDelta(
                    PowerOutcomeSourceKind.Enhancement,
                    "In this power",
                    "Power.Local.Enhancements",
                    localEnhancementDelta));
            }
            else
            {
                rawExternalSourceDeltas = NormalizeExternalSourceDeltas(rawExternalSourceDeltas, actualScaledDelta);
            }
        }

        sourceDeltas.AddRange(rawExternalSourceDeltas);

        return new PowerOutcomeFamilyReceipt(
            PowerOutcomeFamily.Damage,
            mergedRows.Sum(row => row.AssembledBaseValue),
            mergedRows.Sum(row => row.PreBuffValue),
            mergedRows.Sum(row => row.FinalValue),
            mergedRows,
            sourceDeltas);
    }

    private static PowerOutcomeFamilyReceipt? BuildEffectReceipt(
        PowerOutcomeFamily family,
        Enums.eEffectType effectType,
        IPower? assembledBasePower,
        IPower? preBuffPower,
        IPower? buffedPower,
        CalculationContributionSnapshot? contributions,
        string hostFullName,
        string hostDisplayName)
    {
        var assembledRows = BuildEffectStageRows(assembledBasePower, effectType);
        var preBuffRows = BuildEffectStageRows(preBuffPower, effectType);
        var finalRows = BuildEffectStageRows(buffedPower, effectType);
        var mergedRows = MergeStageRows(assembledRows, preBuffRows, finalRows);
        if (mergedRows.Count == 0)
        {
            return null;
        }

        var sourceDeltas = new List<PowerOutcomeSourceDelta>();
        var localEnhancementDelta = mergedRows.Sum(row => row.PreBuffValue - row.AssembledBaseValue);
        if (Math.Abs(localEnhancementDelta) > Epsilon)
        {
            sourceDeltas.Add(new PowerOutcomeSourceDelta(
                PowerOutcomeSourceKind.Enhancement,
                "In this power",
                "Power.Local.Enhancements",
                localEnhancementDelta));
        }

        var actualExternalDelta = mergedRows.Sum(row => row.FinalValue - row.PreBuffValue);
        var scaledSourceDeltas = BuildSingleIndexScaledSourceDeltas(
            contributions,
            ContributionBucket.Effect,
            (int)effectType,
            mergedRows.Sum(row => row.PreBuffValue),
            preBuffPower ?? buffedPower ?? assembledBasePower,
            hostFullName,
            hostDisplayName);
        sourceDeltas.AddRange(NormalizeExternalSourceDeltas(scaledSourceDeltas, actualExternalDelta));

        return new PowerOutcomeFamilyReceipt(
            family,
            mergedRows.Sum(row => row.AssembledBaseValue),
            mergedRows.Sum(row => row.PreBuffValue),
            mergedRows.Sum(row => row.FinalValue),
            mergedRows,
            sourceDeltas);
    }

    private static PowerOutcomeFamilyReceipt? BuildTypedEffectReceipt(
        PowerOutcomeFamily family,
        ContributionBucket bucket,
        IPower? assembledBasePower,
        IPower? preBuffPower,
        IPower? buffedPower,
        CalculationContributionSnapshot? contributions,
        string hostFullName,
        string hostDisplayName)
    {
        var assembledRows = BuildTypedEffectStageRows(assembledBasePower, family);
        var preBuffRows = BuildTypedEffectStageRows(preBuffPower, family);
        var finalRows = BuildTypedEffectStageRows(buffedPower, family);
        var mergedRows = MergeStageRows(assembledRows, preBuffRows, finalRows);
        if (mergedRows.Count == 0)
        {
            return null;
        }

        var sourceDeltas = new List<PowerOutcomeSourceDelta>();
        var localEnhancementDelta = mergedRows.Sum(row => row.PreBuffValue - row.AssembledBaseValue);
        if (Math.Abs(localEnhancementDelta) > Epsilon)
        {
            sourceDeltas.Add(new PowerOutcomeSourceDelta(
                PowerOutcomeSourceKind.Enhancement,
                "In this power",
                "Power.Local.Enhancements",
                localEnhancementDelta));
        }

        var actualExternalDelta = mergedRows.Sum(row => row.FinalValue - row.PreBuffValue);
        var preBuffByIndex = mergedRows
            .GroupBy(row => family == PowerOutcomeFamily.Mez
                ? (int)(row.MezType ?? Enums.eMez.None)
                : (int)(row.DamageType ?? Enums.eDamage.None))
            .ToDictionary(group => group.Key, group => group.Sum(row => row.PreBuffValue));
        sourceDeltas.AddRange(NormalizeExternalSourceDeltas(
            BuildBucketScaledSourceDeltas(
                contributions,
                bucket,
                preBuffByIndex,
                preBuffPower ?? buffedPower ?? assembledBasePower,
                hostFullName,
                hostDisplayName),
            actualExternalDelta));

        return new PowerOutcomeFamilyReceipt(
            family,
            mergedRows.Sum(row => row.AssembledBaseValue),
            mergedRows.Sum(row => row.PreBuffValue),
            mergedRows.Sum(row => row.FinalValue),
            mergedRows,
            sourceDeltas);
    }

    private static PowerOutcomeFamilyReceipt? BuildScalarReceipt(
        PowerOutcomeFamily family,
        string rowKey,
        IPower? assembledBasePower,
        IPower? preBuffPower,
        IPower? buffedPower,
        CalculationContributionSnapshot? contributions,
        string hostFullName,
        string hostDisplayName)
    {
        var assembledBaseValue = GetScalarValue(family, assembledBasePower);
        var preBuffValue = GetScalarValue(family, preBuffPower);
        var finalValue = GetScalarValue(family, buffedPower);
        if (Math.Abs(assembledBaseValue) <= Epsilon &&
            Math.Abs(preBuffValue) <= Epsilon &&
            Math.Abs(finalValue) <= Epsilon)
        {
            return null;
        }

        var rows = new[]
        {
            new PowerOutcomeRow(
                rowKey,
                PowerOutcomeLineage.Scalar,
                assembledBaseValue,
                preBuffValue,
                finalValue)
        };

        var sourceDeltas = new List<PowerOutcomeSourceDelta>();
        var localEnhancementDelta = preBuffValue - assembledBaseValue;
        if (Math.Abs(localEnhancementDelta) > Epsilon)
        {
            sourceDeltas.Add(new PowerOutcomeSourceDelta(
                PowerOutcomeSourceKind.Enhancement,
                "In this power",
                "Power.Local.Enhancements",
                localEnhancementDelta));
        }

        var externalSourceDeltas = BuildScalarSourceDeltas(family, preBuffValue, contributions, hostFullName, hostDisplayName);
        sourceDeltas.AddRange(NormalizeExternalSourceDeltas(externalSourceDeltas, finalValue - preBuffValue));

        return new PowerOutcomeFamilyReceipt(
            family,
            assembledBaseValue,
            preBuffValue,
            finalValue,
            rows,
            sourceDeltas);
    }

    private static IReadOnlyList<PowerOutcomeRow> BuildDamageStageRows(IPower? power, string hostFullName)
    {
        if (power == null)
        {
            return Array.Empty<PowerOutcomeRow>();
        }

        var builders = new Dictionary<string, StageRowBuilder>(StringComparer.OrdinalIgnoreCase);
        foreach (var effect in Power.GetIncludedDamageEffects(power))
        {
            var key = GetReceiptRowKey(effect);
            if (!builders.TryGetValue(key, out var builder))
            {
                builder = new StageRowBuilder(key);
                builders[key] = builder;
            }

            builder.AddDamage(effect, effect.GetPower() ?? power, ClassifyDamageLineage(effect, hostFullName));
        }

        return builders.Values
            .Select(builder => builder.ToRow())
            .OrderByDescending(row => Math.Abs(row.FinalValue + row.PreBuffValue + row.AssembledBaseValue))
            .ThenBy(row => row.RowKey, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<PowerOutcomeRow> BuildEffectStageRows(IPower? power, Enums.eEffectType effectType)
    {
        if (power == null)
        {
            return Array.Empty<PowerOutcomeRow>();
        }

        var builders = new Dictionary<string, StageRowBuilder>(StringComparer.OrdinalIgnoreCase);
        foreach (var effect in EnumerateIncludedEffects(power, effectType))
        {
            var key = GetReceiptRowKey(effect);
            if (!builders.TryGetValue(key, out var builder))
            {
                builder = new StageRowBuilder(key);
                builders[key] = builder;
            }

            builder.AddGeneric(effect, GetGenericEffectValue(effect));
        }

        return builders.Values.Select(builder => builder.ToRow()).ToArray();
    }

    private static IReadOnlyList<PowerOutcomeRow> BuildTypedEffectStageRows(IPower? power, PowerOutcomeFamily family)
    {
        if (power == null)
        {
            return Array.Empty<PowerOutcomeRow>();
        }

        var builders = new Dictionary<string, StageRowBuilder>(StringComparer.OrdinalIgnoreCase);
        foreach (var effect in EnumerateIncludedTypedEffects(power, family))
        {
            var key = GetReceiptRowKey(effect);
            if (!builders.TryGetValue(key, out var builder))
            {
                builder = new StageRowBuilder(key);
                builders[key] = builder;
            }

            builder.AddGeneric(effect, GetGenericEffectValue(effect));
        }

        return builders.Values.Select(builder => builder.ToRow()).ToArray();
    }

    private static IReadOnlyList<PowerOutcomeRow> MergeStageRows(
        IReadOnlyList<PowerOutcomeRow> assembledRows,
        IReadOnlyList<PowerOutcomeRow> preBuffRows,
        IReadOnlyList<PowerOutcomeRow> finalRows)
    {
        var merged = new Dictionary<string, PowerOutcomeRow>(StringComparer.OrdinalIgnoreCase);

        static PowerOutcomeRow Merge(
            string key,
            PowerOutcomeRow? assembled,
            PowerOutcomeRow? preBuff,
            PowerOutcomeRow? final)
        {
            var seed = final ?? preBuff ?? assembled ?? throw new InvalidOperationException("Row merge requires at least one stage value.");
            return seed with
            {
                RowKey = key,
                AssembledBaseValue = assembled?.FinalValue ?? 0d,
                PreBuffValue = preBuff?.FinalValue ?? 0d,
                FinalValue = final?.FinalValue ?? 0d
            };
        }

        foreach (var group in new[] { assembledRows, preBuffRows, finalRows })
        {
            foreach (var row in group)
            {
                merged[row.RowKey] = Merge(
                    row.RowKey,
                    assembledRows.FirstOrDefault(item => item.RowKey.Equals(row.RowKey, StringComparison.OrdinalIgnoreCase)),
                    preBuffRows.FirstOrDefault(item => item.RowKey.Equals(row.RowKey, StringComparison.OrdinalIgnoreCase)),
                    finalRows.FirstOrDefault(item => item.RowKey.Equals(row.RowKey, StringComparison.OrdinalIgnoreCase)));
            }
        }

        return merged.Values
            .Where(row => Math.Abs(row.AssembledBaseValue) > Epsilon ||
                          Math.Abs(row.PreBuffValue) > Epsilon ||
                          Math.Abs(row.FinalValue) > Epsilon)
            .OrderByDescending(row => Math.Abs(row.FinalValue) > Epsilon ? Math.Abs(row.FinalValue) : Math.Abs(row.PreBuffValue))
            .ThenBy(row => row.RowKey, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<PowerOutcomeSourceDelta> BuildSingleIndexScaledSourceDeltas(
        CalculationContributionSnapshot? contributions,
        ContributionBucket bucket,
        int index,
        double referenceTotal,
        IPower? hostPower,
        string hostFullName,
        string hostDisplayName,
        bool includeDisplayedPowerEnhancements = false)
    {
        if (contributions == null || Math.Abs(referenceTotal) <= Epsilon)
        {
            return Array.Empty<PowerOutcomeSourceDelta>();
        }

        return BuildBucketScaledSourceDeltas(
            contributions,
            bucket,
            new Dictionary<int, double> { [index] = referenceTotal },
            hostPower,
            hostFullName,
            hostDisplayName,
            includeDisplayedPowerEnhancements);
    }

    private static IReadOnlyList<PowerOutcomeSourceDelta> BuildBucketScaledSourceDeltas(
        CalculationContributionSnapshot? contributions,
        ContributionBucket bucket,
        IReadOnlyDictionary<int, double> referenceTotalsByIndex,
        IPower? hostPower,
        string hostFullName,
        string hostDisplayName,
        bool includeDisplayedPowerEnhancements = false)
    {
        if (contributions == null || referenceTotalsByIndex.Count == 0)
        {
            return Array.Empty<PowerOutcomeSourceDelta>();
        }

        var totals = new Dictionary<(PowerOutcomeSourceKind Kind, string Name, string FullName), double>();
        foreach (var record in contributions.Records)
        {
            if (record.Key is not { } key || key.Bucket != bucket)
            {
                continue;
            }

            if (ShouldSkipSource(record, hostFullName, hostDisplayName, includeDisplayedPowerEnhancements))
            {
                continue;
            }

            var delta = ResolveBucketScaledDelta(
                bucket,
                key.Index,
                record.Value,
                referenceTotalsByIndex,
                hostPower);
            if (Math.Abs(delta) <= Epsilon)
            {
                continue;
            }

            var sourceKind = ClassifySourceKind(record);
            var normalizedName = NormalizeSourceName(record.Source, sourceKind);
            var keyTuple = (sourceKind, normalizedName, record.Source.FullName ?? string.Empty);
            totals[keyTuple] = totals.TryGetValue(keyTuple, out var existing)
                ? existing + delta
                : delta;
        }

        return totals
            .Where(item => Math.Abs(item.Value) > Epsilon)
            .Select(item => new PowerOutcomeSourceDelta(item.Key.Kind, item.Key.Name, item.Key.FullName, item.Value))
            .OrderByDescending(item => Math.Abs(item.Value))
            .ThenBy(item => item.SourceName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<PowerOutcomeSourceDelta> NormalizeExternalSourceDeltas(
        IReadOnlyList<PowerOutcomeSourceDelta> deltas,
        double actualExternalDelta)
    {
        if (Math.Abs(actualExternalDelta) <= Epsilon)
        {
            return Array.Empty<PowerOutcomeSourceDelta>();
        }

        var recognizedTotal = deltas.Sum(delta => delta.Value);
        if (Math.Abs(recognizedTotal - actualExternalDelta) <= 0.001)
        {
            return deltas;
        }

        if (Math.Abs(recognizedTotal) <= Epsilon)
        {
            return
            [
                new PowerOutcomeSourceDelta(
                    PowerOutcomeSourceKind.MiscExternal,
                    "Other external effects",
                    "Power.External.Misc",
                    actualExternalDelta)
            ];
        }

        var normalized = deltas
            .Select(delta => delta with { Value = delta.Value * (actualExternalDelta / recognizedTotal) })
            .ToList();
        var residual = actualExternalDelta - normalized.Sum(delta => delta.Value);
        if (Math.Abs(residual) > 0.0001)
        {
            normalized.Add(new PowerOutcomeSourceDelta(
                PowerOutcomeSourceKind.MiscExternal,
                "Other external effects",
                "Power.External.Misc",
                residual));
        }

        return normalized
            .Where(delta => Math.Abs(delta.Value) > Epsilon)
            .OrderByDescending(delta => Math.Abs(delta.Value))
            .ThenBy(delta => delta.SourceName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<PowerOutcomeSourceDelta> BuildScalarSourceDeltas(
        PowerOutcomeFamily family,
        double preBuffValue,
        CalculationContributionSnapshot? contributions,
        string hostFullName,
        string hostDisplayName)
    {
        if (contributions == null || Math.Abs(preBuffValue) <= Epsilon)
        {
            return Array.Empty<PowerOutcomeSourceDelta>();
        }

        IEnumerable<(ContributionSource Source, ContributionChannel Channel, double Total)> Groups(params (ContributionBucket Bucket, int Index)[] keys)
        {
            return keys.SelectMany(key => contributions.GetSummed(key.Bucket, key.Index));
        }

        var totals = new Dictionary<(PowerOutcomeSourceKind Kind, string Name, string FullName), double>();

        void AddScalarDelta(ContributionSource source, double delta, ContributionCategory? category = null)
        {
            if (Math.Abs(delta) <= Epsilon)
            {
                return;
            }

            var record = new ContributionRecord(
                null,
                0d,
                ContributionChannel.Buffs,
                category ?? ContributionCategory.SelfBuff,
                source,
                string.Empty,
                null,
                null,
                null,
                null,
                null);
            var kind = ClassifySourceKind(record);
            var name = NormalizeSourceName(source, kind);
            var key = (kind, name, source.FullName ?? string.Empty);
            totals[key] = totals.TryGetValue(key, out var existing)
                ? existing + delta
                : delta;
        }

        switch (family)
        {
            case PowerOutcomeFamily.Recharge:
            {
                var items = Groups(
                        (ContributionBucket.Effect, (int)Enums.eStatType.Haste),
                        (ContributionBucket.Effect, (int)Enums.eEffectType.RechargeTime))
                    .GroupBy(item => item.Source)
                    .Select(group => (Source: group.Key, Total: group.Sum(item => item.Total)))
                    .Where(item => !IsDisplayedPowerSource(item.Source, hostFullName, hostDisplayName))
                    .ToArray();
                var total = items.Sum(item => item.Total);
                foreach (var item in items)
                {
                    var withoutSource = total - item.Total;
                    var delta = preBuffValue / Math.Max(1e-9, 1d + total) -
                                preBuffValue / Math.Max(1e-9, 1d + withoutSource);
                    AddScalarDelta(item.Source, delta);
                }

                break;
            }

            case PowerOutcomeFamily.EndCost:
            {
                var items = Groups(
                        (ContributionBucket.Effect, (int)Enums.eStatType.BuffEndRdx),
                        (ContributionBucket.Effect, (int)Enums.eEffectType.EnduranceDiscount))
                    .GroupBy(item => item.Source)
                    .Select(group => (Source: group.Key, Total: group.Sum(item => item.Total)))
                    .Where(item => !IsDisplayedPowerSource(item.Source, hostFullName, hostDisplayName))
                    .ToArray();
                var total = items.Sum(item => item.Total);
                foreach (var item in items)
                {
                    var withoutSource = Math.Max(0d, total - item.Total);
                    var delta = preBuffValue / (1d + total) - preBuffValue / (1d + withoutSource);
                    AddScalarDelta(item.Source, delta);
                }

                break;
            }

            case PowerOutcomeFamily.Accuracy:
            {
                var items = Groups(
                        (ContributionBucket.Effect, (int)Enums.eStatType.BuffAcc),
                        (ContributionBucket.Effect, (int)Enums.eStatType.ToHit))
                    .GroupBy(item => item.Source)
                    .Select(group => (Source: group.Key, Total: group.Sum(item => item.Total)))
                    .Where(item => !IsDisplayedPowerSource(item.Source, hostFullName, hostDisplayName))
                    .ToArray();
                foreach (var item in items)
                {
                    AddScalarDelta(item.Source, preBuffValue * item.Total);
                }

                break;
            }

            case PowerOutcomeFamily.Range:
            {
                var items = Groups((ContributionBucket.Effect, (int)Enums.eEffectType.Range))
                    .GroupBy(item => item.Source)
                    .Select(group => (Source: group.Key, Total: group.Sum(item => item.Total)))
                    .Where(item => !IsDisplayedPowerSource(item.Source, hostFullName, hostDisplayName))
                    .ToArray();
                foreach (var item in items)
                {
                    AddScalarDelta(item.Source, preBuffValue * item.Total);
                }

                break;
            }

            case PowerOutcomeFamily.Interrupt:
            {
                var items = Groups((ContributionBucket.Effect, (int)Enums.eEffectType.InterruptTime))
                    .GroupBy(item => item.Source)
                    .Select(group => (Source: group.Key, Total: group.Sum(item => item.Total)))
                    .Where(item => !IsDisplayedPowerSource(item.Source, hostFullName, hostDisplayName))
                    .ToArray();
                foreach (var item in items)
                {
                    AddScalarDelta(item.Source, -preBuffValue * item.Total);
                }

                break;
            }
        }

        return totals
            .Where(item => Math.Abs(item.Value) > Epsilon)
            .Select(item => new PowerOutcomeSourceDelta(item.Key.Kind, item.Key.Name, item.Key.FullName, item.Value))
            .OrderByDescending(item => Math.Abs(item.Value))
            .ThenBy(item => item.SourceName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IEnumerable<IEffect> EnumerateIncludedEffects(IPower power, Enums.eEffectType effectType)
    {
        return power.Effects.Where(effect =>
            effect.EffectType == effectType &&
            effect.Probability > 0f &&
            effect.EffectClass is not (Enums.eEffectClass.Ignored or Enums.eEffectClass.Special) &&
            effect.CanInclude() &&
            effect.PvXInclude() &&
            (MidsContext.Config.Suppression & effect.Suppression) == Enums.eSuppress.None &&
            effect.ToWho is Enums.eToWho.Target or Enums.eToWho.Self or Enums.eToWho.All);
    }

    private static IEnumerable<IEffect> EnumerateIncludedTypedEffects(IPower power, PowerOutcomeFamily family)
    {
        return power.Effects.Where(effect =>
        {
            if (effect.Probability <= 0f ||
                effect.EffectClass is Enums.eEffectClass.Ignored or Enums.eEffectClass.Special ||
                !effect.CanInclude() ||
                !effect.PvXInclude())
            {
                return false;
            }

            return family switch
            {
                PowerOutcomeFamily.Defense => effect.EffectType == Enums.eEffectType.Defense,
                PowerOutcomeFamily.Resistance => effect.EffectType == Enums.eEffectType.Resistance,
                PowerOutcomeFamily.Mez => effect.EffectType == Enums.eEffectType.Mez,
                _ => false
            };
        });
    }

    private static double GetGenericEffectValue(IEffect effect)
    {
        var visibleCopies = PlannerStackRules.GetPlannerVisibleCopyCount(effect, effect.Ticks);
        return effect.BuffedMag * Math.Max(1d, visibleCopies);
    }

    private static double GetScalarValue(PowerOutcomeFamily family, IPower? power)
    {
        if (power == null)
        {
            return 0d;
        }

        return family switch
        {
            PowerOutcomeFamily.Recharge => power.RechargeTime,
            PowerOutcomeFamily.EndCost => power.PowerType == Enums.ePowerType.Toggle ? power.ToggleCost : power.EndCost,
            PowerOutcomeFamily.Accuracy => power.Accuracy,
            PowerOutcomeFamily.Range => power.Range,
            PowerOutcomeFamily.Interrupt => power.InterruptTime,
            _ => 0d
        };
    }

    private static PowerOutcomeLineage ClassifyDamageLineage(IEffect effect, string hostFullName)
    {
        if (PlannerProcSupport.IsDirectProcDamageEffect(effect))
        {
            return PowerOutcomeLineage.Proc;
        }

        if (IsAttachedBonusDamageEffect(effect))
        {
            return PowerOutcomeLineage.OtherBonusEffect;
        }

        var belongsToHost = IsHostDamageEffect(effect, hostFullName);
        if (!belongsToHost)
        {
            return PowerOutcomeLineage.OtherBonusEffect;
        }

        if (IsConditionalDamageEffect(effect))
        {
            return PowerOutcomeLineage.ConditionalBonus;
        }

        return PowerOutcomeLineage.BasePower;
    }

    private static bool IsConditionalDamageEffect(IEffect effect)
    {
        if (effect is not Effect concreteEffect)
        {
            return false;
        }

        return concreteEffect.HasPlannerModeCondition(PlannerMode.CriticalHit) ||
               concreteEffect.HasPlannerModeCondition(PlannerMode.Assassination) ||
               concreteEffect.HasPlannerModeCondition(PlannerMode.StalkerHidden) ||
               concreteEffect.HasPlannerModeCondition(PlannerMode.Containment) ||
               concreteEffect.HasPlannerModeCondition(PlannerMode.Scourge) ||
               concreteEffect.HasPlannerModeCondition(PlannerMode.Domination) ||
               concreteEffect.HasPlannerModeCondition(PlannerMode.PackMentality) ||
               concreteEffect.HasPlannerModeCondition(PlannerMode.Insight) ||
               concreteEffect.AdvancedConditions?.Rows.Any(row =>
                   row.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated) == true;
    }

    private static bool IsAttachedBonusDamageEffect(IEffect effect)
    {
        if (effect == null || effect.isEnhancementEffect)
        {
            return false;
        }

        var tags = effect.EffectTags ?? [];
        if (tags.Any(tag =>
                tag.Contains("Interface", StringComparison.OrdinalIgnoreCase) ||
                tag.Contains("Doublehit", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return effect.OmniSource.Contains("Interface", StringComparison.OrdinalIgnoreCase) ||
               effect.OmniSource.Contains("Doublehit", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHostDamageEffect(IEffect effect, string hostFullName)
    {
        if (string.IsNullOrWhiteSpace(hostFullName))
        {
            return false;
        }

        var ownerFullName = effect.GetPower()?.FullName ?? string.Empty;
        if (string.Equals(ownerFullName, hostFullName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(effect.OmniSource))
        {
            return false;
        }

        var separatorIndex = effect.OmniSource.IndexOf(':');
        var omniPowerName = separatorIndex >= 0
            ? effect.OmniSource[..separatorIndex]
            : effect.OmniSource;
        return string.Equals(omniPowerName, hostFullName, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetReceiptRowKey(IEffect effect)
    {
        var recurrence = effect.PseudoPetRecurrence is { IsValid: true } info
            ? $"{info.EntityName}|{info.PetPowerName}|{info.SpawnCount}|{info.TicksPerSpawn}|{info.TotalExpectedTicks}"
            : string.Empty;

        return string.Join("|",
            PlannerStackRules.GetPlannerContributionGroupKey(effect),
            recurrence);
    }

    private static bool ShouldSkipSource(
        ContributionRecord record,
        string hostFullName,
        string hostDisplayName,
        bool includeDisplayedPowerEnhancements)
    {
        if (record.Category == ContributionCategory.CapAdjustment)
        {
            return true;
        }

        if (!IsDisplayedPowerSource(record.Source, hostFullName, hostDisplayName))
        {
            return false;
        }

        return !(includeDisplayedPowerEnhancements &&
                 record.Category == ContributionCategory.Enhancement);
    }

    private static double ResolveBucketScaledDelta(
        ContributionBucket bucket,
        int index,
        double magnitude,
        IReadOnlyDictionary<int, double> preBuffTotalsByIndex,
        IPower? hostPower)
    {
        if (bucket != ContributionBucket.Damage)
        {
            return preBuffTotalsByIndex.TryGetValue(index, out var exactPreBuffTotal)
                ? exactPreBuffTotal * magnitude
                : 0d;
        }

        var attackQualifiedTotal = preBuffTotalsByIndex.Values.Sum();
        if (Math.Abs(attackQualifiedTotal) <= Epsilon)
        {
            return 0d;
        }

        return (Enums.eDamage)index switch
        {
            Enums.eDamage.None => attackQualifiedTotal * magnitude,
            Enums.eDamage.Melee => PowerMatchesAttackVector(hostPower, Enums.eVector.Melee_Attack)
                ? attackQualifiedTotal * magnitude
                : 0d,
            Enums.eDamage.Ranged => PowerMatchesAttackVector(hostPower, Enums.eVector.Ranged_Attack)
                ? attackQualifiedTotal * magnitude
                : 0d,
            Enums.eDamage.AoE => PowerMatchesAttackVector(hostPower, Enums.eVector.AOE_Attack)
                ? attackQualifiedTotal * magnitude
                : 0d,
            _ => preBuffTotalsByIndex.TryGetValue(index, out var typedPreBuffTotal)
                ? typedPreBuffTotal * magnitude
                : 0d
        };
    }

    private static bool PowerMatchesAttackVector(IPower? power, Enums.eVector vector)
    {
        return power != null && (power.AttackTypes & vector) != Enums.eVector.None;
    }

    private static bool IsDisplayedPowerSource(ContributionSource source, string hostFullName, string hostDisplayName)
    {
        return (!string.IsNullOrWhiteSpace(hostFullName) &&
                string.Equals(source.FullName, hostFullName, StringComparison.OrdinalIgnoreCase)) ||
               (!string.IsNullOrWhiteSpace(hostDisplayName) &&
                string.Equals(source.Name, hostDisplayName, StringComparison.OrdinalIgnoreCase));
    }

    private static PowerOutcomeSourceKind ClassifySourceKind(ContributionRecord record)
    {
        if (record.Source.IsSetBonusVirtual || record.Category == ContributionCategory.SetBonus)
        {
            return PowerOutcomeSourceKind.SetBonus;
        }

        if (record.Source.IsIncarnate)
        {
            return PowerOutcomeSourceKind.IncarnateBonus;
        }

        if (record.Category == ContributionCategory.ComputedState || IsComputedSource(record.Source))
        {
            return PowerOutcomeSourceKind.ComputedState;
        }

        if (record.Channel == ContributionChannel.Buffs ||
            record.Category is ContributionCategory.SelfBuff or ContributionCategory.GrantChild or ContributionCategory.ExecuteChild or ContributionCategory.DeliveryChild or ContributionCategory.PseudoPetFlattened or ContributionCategory.OwnerInheritedPetBonus)
        {
            return PowerOutcomeSourceKind.OtherPowerBuff;
        }

        if (record.Category == ContributionCategory.Enhancement)
        {
            return PowerOutcomeSourceKind.Enhancement;
        }

        return PowerOutcomeSourceKind.MiscExternal;
    }

    private static bool IsComputedSource(ContributionSource source)
    {
        return source.FullName.StartsWith("Planner.Computed.", StringComparison.OrdinalIgnoreCase) ||
               source.FullName.Equals(PlannerStateCatalog.DefiancePowerFullName, StringComparison.OrdinalIgnoreCase) ||
               source.FullName.Equals(VigilancePlanner.VigilancePowerFullName, StringComparison.OrdinalIgnoreCase) ||
               source.FullName.Equals(VigilancePlanner.VigilanceEndAdjustmentPowerFullName, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeSourceName(ContributionSource source, PowerOutcomeSourceKind kind)
    {
        if (!string.IsNullOrWhiteSpace(source.Name))
        {
            return source.Name.Trim();
        }

        if (kind == PowerOutcomeSourceKind.SetBonus)
        {
            return "Set Bonus Effects";
        }

        return string.IsNullOrWhiteSpace(source.FullName)
            ? "Unknown Source"
            : source.FullName.Trim();
    }

    private static string ResolveHostFullName(params IPower?[] powers)
    {
        return powers.FirstOrDefault(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))?.FullName ?? string.Empty;
    }

    private static string ResolveHostDisplayName(params IPower?[] powers)
    {
        return powers.FirstOrDefault(power => power != null && !string.IsNullOrWhiteSpace(power.DisplayName))?.DisplayName ?? string.Empty;
    }

    private sealed class StageRowBuilder
    {
        public StageRowBuilder(string rowKey)
        {
            RowKey = rowKey;
        }

        public string RowKey { get; }
        public PowerOutcomeLineage Lineage { get; private set; } = PowerOutcomeLineage.Scalar;
        public double Value { get; private set; }
        public string SourceName { get; private set; } = string.Empty;
        public string ConditionName { get; private set; } = string.Empty;
        public Enums.eDamage? DamageType { get; private set; }
        public Enums.eEffectType? EffectType { get; private set; }
        public Enums.eMez? MezType { get; private set; }
        public string OmniSource { get; private set; } = string.Empty;
        public string EffectId { get; private set; } = string.Empty;
        public float Probability { get; private set; } = 1f;
        public float ProcsPerMinute { get; private set; }
        public float EffectiveTicks { get; private set; } = 1f;
        public float DurationSeconds { get; private set; }
        public bool IsEnhancementEffect { get; private set; }
        public bool IsProc { get; private set; }
        public bool IgnoreScaling { get; private set; }
        public bool IgnoreBuffs { get; private set; }
        public bool IgnoreEd { get; private set; }
        public bool CancelOnMiss { get; private set; }
        public bool RequiresToHitCheck { get; private set; }
        public bool SuppressWhenMezzed { get; private set; }
        public int Occurrences { get; private set; }

        public void AddDamage(IEffect effect, IPower power, PowerOutcomeLineage lineage)
        {
            AddGeneric(effect, Power.GetDamageEffectTotal(effect, power, absolute: true, applyReturnScaling: true));
            Lineage = lineage;
            EffectiveTicks = Math.Max(1f, Power.GetDamageEffectEffectiveTicks(effect));
            DurationSeconds = Math.Max(effect.Duration, Math.Max(effect.Absorbed_Duration, effect.DelayedTime));
            IsProc = PlannerProcSupport.IsDirectProcDamageEffect(effect);
            SourceName = string.IsNullOrWhiteSpace(SourceName) ? ResolveDamageSourceName(effect) : SourceName;
            ConditionName = string.IsNullOrWhiteSpace(ConditionName) ? ResolveDamageConditionName(effect) : ConditionName;
        }

        public void AddGeneric(IEffect effect, double value)
        {
            Value += value;
            DamageType ??= effect.DamageType;
            EffectType ??= effect.EffectType;
            MezType ??= effect.MezType;
            OmniSource = string.IsNullOrWhiteSpace(OmniSource) ? effect.OmniSource ?? string.Empty : OmniSource;
            EffectId = string.IsNullOrWhiteSpace(EffectId) ? effect.EffectId ?? string.Empty : EffectId;
            Probability = effect.Probability;
            ProcsPerMinute = effect.ProcsPerMinute;
            IsEnhancementEffect |= effect.isEnhancementEffect;
            IgnoreScaling |= effect.IgnoreScaling;
            IgnoreBuffs |= !effect.Buffable || effect.EffectType == Enums.eEffectType.DamageBuff;
            IgnoreEd |= effect.IgnoreED;
            CancelOnMiss |= effect.CancelOnMiss;
            RequiresToHitCheck |= effect.RequiresToHitCheck;
            SuppressWhenMezzed |= effect.Suppression != Enums.eSuppress.None;
            Occurrences++;
        }

        public PowerOutcomeRow ToRow()
        {
            return new PowerOutcomeRow(
                RowKey,
                Lineage,
                0d,
                0d,
                Value,
                SourceName,
                ConditionName,
                DamageType,
                EffectType,
                MezType,
                OmniSource,
                EffectId,
                Probability,
                ProcsPerMinute,
                EffectiveTicks,
                DurationSeconds,
                IsEnhancementEffect,
                IsProc,
                IgnoreScaling,
                IgnoreBuffs,
                IgnoreEd,
                CancelOnMiss,
                RequiresToHitCheck,
                SuppressWhenMezzed,
                Occurrences);
        }
    }

    private static string ResolveDamageSourceName(IEffect effect)
    {
        if (effect.isEnhancementEffect)
        {
            var enhancementName = effect.Enhancement?.Name?.Trim();
            if (!string.IsNullOrWhiteSpace(enhancementName) &&
                !enhancementName.Equals("New Enhancement", StringComparison.OrdinalIgnoreCase))
            {
                return NormalizeDamageSourceDisplayName(enhancementName);
            }

            var enhancementPowerName = effect.Enhancement?.GetPower()?.DisplayName?.Trim();
            if (!string.IsNullOrWhiteSpace(enhancementPowerName))
            {
                return NormalizeDamageSourceDisplayName(enhancementPowerName);
            }
        }

        if (!string.IsNullOrWhiteSpace(effect.OmniSource))
        {
            var separatorIndex = effect.OmniSource.IndexOf(':');
            var fullName = separatorIndex >= 0
                ? effect.OmniSource[..separatorIndex]
                : effect.OmniSource;
            var powerName = DatabaseAPI.GetPowerByFullName(fullName)?.DisplayName?.Trim();
            if (!string.IsNullOrWhiteSpace(powerName))
            {
                return NormalizeDamageSourceDisplayName(powerName);
            }
        }

        return NormalizeDamageSourceDisplayName(effect.GetPower()?.DisplayName?.Trim() ?? string.Empty);
    }

    private static string ResolveDamageConditionName(IEffect effect)
    {
        if (effect is not Effect concreteEffect)
        {
            return string.Empty;
        }

        if (concreteEffect.HasPlannerModeCondition(PlannerMode.Insight))
        {
            return "Insight";
        }

        if (concreteEffect.HasPlannerModeCondition(PlannerMode.CriticalHit))
        {
            return "Critical";
        }

        if (concreteEffect.HasPlannerModeCondition(PlannerMode.Assassination))
        {
            return "Assassination";
        }

        if (concreteEffect.HasPlannerModeCondition(PlannerMode.StalkerHidden))
        {
            return "Hidden Strike";
        }

        if (concreteEffect.HasPlannerModeCondition(PlannerMode.Containment))
        {
            return "Containment";
        }

        if (concreteEffect.HasPlannerModeCondition(PlannerMode.Scourge))
        {
            return "Scourge";
        }

        if (concreteEffect.HasPlannerModeCondition(PlannerMode.Domination))
        {
            return "Domination";
        }

        if (concreteEffect.HasPlannerModeCondition(PlannerMode.PackMentality))
        {
            return "Pack Mentality";
        }

        if (IsArchetypeCriticalDamageEffect(concreteEffect))
        {
            return "Critical";
        }

        if ((effect.EffectTags ?? []).Any(tag => tag.Contains("Crit", StringComparison.OrdinalIgnoreCase)))
        {
            return "Critical";
        }

        return string.Empty;
    }

    private static bool IsArchetypeCriticalDamageEffect(Effect effect)
    {
        if (effect == null || !effect.ModifierTable.Contains("InherentDamage", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var archetypeClass = effect.AdvancedConditions?.Rows
            .FirstOrDefault(row =>
                row.Kind == AdvancedConditionKind.CharacterArchetype &&
                !row.Negated &&
                row.Operator == AdvancedConditionOperator.Equals &&
                !string.IsNullOrWhiteSpace(row.Value))
            ?.Value;
        if (!string.IsNullOrWhiteSpace(archetypeClass))
        {
            return true;
        }

        return (effect.EffectTags ?? []).Any(tag => tag.Contains("ScrapperCrit", StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeDamageSourceDisplayName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return string.Empty;
        }

        var text = rawName.Trim();
        var colonIndex = text.IndexOf(':');
        if (colonIndex > 0)
        {
            text = text[..colonIndex].Trim();
        }

        text = Regex.Replace(text, @"\bChance\s+(of|for|to)\b.*$", string.Empty, RegexOptions.IgnoreCase).Trim();
        return text;
    }
}

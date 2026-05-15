using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

public enum PetGrantKind
{
    Baseline,
    Grant,
    GrantBoosted
}

public enum PetAppliedBonusSourceType
{
    UpgradeOverlay,
    SetBonus,
    OwnerAuraOrBuff,
    PetAutoToggle,
    PetClickBuff
}

public sealed class ResolvedPetPower
{
    public required IPower Power { get; init; }
    public required int SourceHistoryIndex { get; init; }
    public required PetGrantKind GrantKind { get; init; }
    public string? GrantedByPowerFullName { get; init; }
    public string? RequiredUpgradePowerFullName { get; init; }
    public bool VisibleInGrid { get; init; } = true;
    public bool IncludeInTotalsByDefault { get; init; }
    public bool IsSelfClickBuff { get; init; }
}

public sealed class PetUpgradePreviewState
{
    public HashSet<string> AppliedUpgradePowerFullNames { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public bool DefaultsInitialized { get; init; }

    public PetUpgradePreviewState Clone()
    {
        return new PetUpgradePreviewState
        {
            AppliedUpgradePowerFullNames = new HashSet<string>(AppliedUpgradePowerFullNames, StringComparer.OrdinalIgnoreCase),
            DefaultsInitialized = DefaultsInitialized
        };
    }
}

public sealed class PetClickPreviewState
{
    public HashSet<string> IncludedPetSelfClickFullNames { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    public PetClickPreviewState Clone()
    {
        return new PetClickPreviewState
        {
            IncludedPetSelfClickFullNames = new HashSet<string>(IncludedPetSelfClickFullNames, StringComparer.OrdinalIgnoreCase)
        };
    }
}

public sealed class PetActorPreviewState
{
    public PetUpgradePreviewState Upgrades { get; init; } = new();
    public PetClickPreviewState ClickBuffs { get; init; } = new();
    public bool InRange { get; set; } = true;

    public bool IsUpgradeApplied(string? powerFullName)
    {
        return !string.IsNullOrWhiteSpace(powerFullName) &&
               Upgrades.AppliedUpgradePowerFullNames.Contains(powerFullName);
    }

    public bool IsPetClickBuffIncluded(string? powerFullName)
    {
        return !string.IsNullOrWhiteSpace(powerFullName) &&
               ClickBuffs.IncludedPetSelfClickFullNames.Contains(powerFullName);
    }

    public PetActorPreviewState Clone()
    {
        return new PetActorPreviewState
        {
            Upgrades = Upgrades.Clone(),
            ClickBuffs = ClickBuffs.Clone(),
            InRange = InRange
        };
    }
}

public sealed class PetAppliedBonusStatDelta
{
    public string StatName { get; init; } = string.Empty;
    public float Delta { get; init; }
    public string Suffix { get; init; } = string.Empty;

    public string ToDisplayString()
    {
        var sign = Delta >= 0 ? "+" : string.Empty;
        return $"{StatName} {sign}{Delta:0.##}{Suffix}";
    }
}

public sealed class PetAppliedBonusEntry
{
    public string SourceName { get; init; } = string.Empty;
    public string SourceFullName { get; init; } = string.Empty;
    public PetAppliedBonusSourceType SourceType { get; init; }
    public IReadOnlyList<PetAppliedBonusStatDelta> StatDeltas { get; init; } = [];
    public string Summary { get; init; } = string.Empty;
    public string Tooltip { get; init; } = string.Empty;
}

public sealed class PetUpgradeOverlayCatalog
{
    public Dictionary<string, List<PetUpgradeOverlay>> OverlaysByEntityUid { get; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class PetUpgradeOverlay
{
    public string TargetEntityUid { get; init; } = string.Empty;
    public string UpgradePowerFullName { get; init; } = string.Empty;
    public string UpgradePowerDisplayName { get; init; } = string.Empty;
    public IReadOnlyList<PetUpgradeGrantedPower> GrantedPowers { get; init; } = [];
}

public sealed class PetUpgradeGrantedPower
{
    public string GrantedPowerFullName { get; init; } = string.Empty;
    public PetGrantKind GrantKind { get; init; }
}

public sealed class RealPetActorRosterItem
{
    public string EntityUid { get; init; } = string.Empty;
    public string EntityDisplayName { get; init; } = string.Empty;
    public string EntityClassName { get; init; } = string.Empty;
    public int SourceHistoryIndex { get; init; }
    public string SourcePowerFullName { get; init; } = string.Empty;
    public string SourcePowerDisplayName { get; init; } = string.Empty;
    public int Count { get; set; } = 1;

    public override string ToString()
    {
        return Count > 1
            ? $"{EntityDisplayName} x{Count}"
            : EntityDisplayName;
    }
}

public sealed class ActorTotalsSnapshot
{
    public string ClassName { get; init; } = string.Empty;
    public Character.TotalStatistics Totals { get; init; } = new();
    public Character.TotalStatistics TotalsCapped { get; init; } = new();
    public ActorDisplayStats DisplayStats { get; init; } = null!;
}

public sealed class PetActorSnapshot
{
    public RealPetActorRosterItem RosterItem { get; init; } = new();
    public SummonedEntity Entity { get; init; } = null!;
    public PlannerBuildRecipientContext Recipient { get; init; } = null!;
    public IReadOnlyList<string> ActorTags { get; init; } = [];
    public PetActorPreviewState PreviewState { get; init; } = new();
    public IReadOnlyList<PetUpgradeOverlay> AvailableUpgrades { get; init; } = [];
    public IReadOnlyList<ResolvedPetPower> AvailableSelfClickBuffs { get; init; } = [];
    public IReadOnlyList<ResolvedPetPower> ResolvedPowers { get; init; } = [];
    public IReadOnlyList<IPower> BasePowers { get; init; } = [];
    public IReadOnlyList<IPower> MathPowers { get; init; } = [];
    public IReadOnlyList<IPower> BuffedPowers { get; init; } = [];
    public ActorTotalsSnapshot Totals { get; init; } = null!;
    public IReadOnlyList<PetAppliedBonusEntry> AppliedBonusEntries { get; init; } = [];
    internal ActorCalculationSnapshot? CalculationSnapshot { get; init; }
}

public sealed class ActorDisplayStats
{
    private readonly string _className;
    private readonly Character.TotalStatistics _totals;
    private readonly Character.TotalStatistics _totalsCapped;

    public ActorDisplayStats(string className, Character.TotalStatistics totals, Character.TotalStatistics totalsCapped)
    {
        _className = className;
        _totals = totals;
        _totalsCapped = totalsCapped;
    }

    public float EnduranceMaxEnd => _totals.EndMax + 100f;
    public float EnduranceUsage => _totals.EndUse;
    public float EnduranceRecoveryNumeric => GetEnduranceRecoveryNumeric(false);
    public float EnduranceRecoveryNumericUncapped => GetEnduranceRecoveryNumeric(true);
    public float EnduranceTimeToFull => EnduranceMaxEnd / EnduranceRecoveryNumeric;
    public float EnduranceRecoveryNet => GetEnduranceRecoveryNet(false);
    public float EnduranceRecoveryNetUncapped => GetEnduranceRecoveryNet(true);
    public float EnduranceRecoveryLossNet => (float)-(EnduranceRecoveryNumeric - EnduranceUsage);
    public float EnduranceTimeToZero => EnduranceMaxEnd / (float)-(EnduranceRecoveryNumeric - EnduranceUsage);
    public float EnduranceTimeToFullNet => EnduranceMaxEnd / (EnduranceRecoveryNumeric - EnduranceUsage);
    public float HealthHitpointsNumeric(bool uncapped) => uncapped ? _totals.HPMax : _totalsCapped.HPMax;
    public float HealthHitpointsPercentage => (float)(HealthHitpointsNumeric(false) / DatabaseAPI.GetClassHitPoints(_className) * 100.0);
    public float Absorb => _totals.Absorb;
    public float HealthRegenHealthPerSec => (float)(HealthRegen(false) * DatabaseAPI.GetClassBaseRegen(_className) * Statistics.BaseMagic);
    public float HealthRegenTimeToFull => HealthHitpointsNumeric(false) / HealthRegenHPPerSec(false);
    public float BuffToHit => _totals.BuffToHit * 100f;
    public float BuffAccuracy => _totals.BuffAcc * 100f;
    public float BuffEndRdx => _totals.BuffEndRdx * 100f;
    public float ThreatLevel => (_totals.ThreatLevel + DatabaseAPI.GetClassBaseThreat(_className)) * 100f;
    public float Range => _totals.BuffRange;
    public float RangePercent => _totals.BuffRange * 100f;
    public float BuffDamage(bool uncapped)
    {
        var archetype = DatabaseAPI.GetArchetypeByClassName(_className);
        return DatabaseAPI.GetPlannerRuleset().GetDisplayedBuffDamagePercent(archetype, _totals, _totalsCapped, uncapped);
    }

    public float BuffHaste(bool uncapped)
    {
        if (uncapped)
        {
            return (_totals.BuffHaste + 1) * 100f;
        }

        var rechargeCap = DatabaseAPI.GetClassRechargeCap(_className) * 100f;
        return Math.Min(rechargeCap, (_totalsCapped.BuffHaste + 1) * 100f);
    }

    public float EnduranceRecoveryPercentage(bool uncapped) => EnduranceRecovery(uncapped) * 100f;

    public float GetEnduranceRecoveryNumeric(bool uncapped)
    {
        var maxEnd = uncapped ? _totals.EndMax : _totalsCapped.EndMax;
        return EnduranceRecovery(uncapped) * (DatabaseAPI.GetClassBaseRecovery(_className) * Statistics.BaseMagic) * (maxEnd / 100f + 1f);
    }

    public float GetEnduranceRecoveryNet(bool uncapped) => GetEnduranceRecoveryNumeric(uncapped) - EnduranceUsage;

    public float HealthRegenPercent(bool uncapped) => HealthRegen(uncapped) * 100f;

    public float HealthRegenHPPerSec(bool uncapped)
    {
        return HealthRegen(uncapped) * DatabaseAPI.GetClassBaseRegen(_className) * Statistics.BaseMagic * HealthHitpointsNumeric(uncapped) / 100f;
    }

    public float DamageResistance(int damageType, bool uncapped)
    {
        return (uncapped ? _totals.Res[damageType] : _totalsCapped.Res[damageType]) * 100f;
    }

    public float Defense(int damageType)
    {
        return _totals.Def[damageType] * 100f;
    }

    public float Perception(bool uncapped)
    {
        return uncapped ? _totals.Perception : _totalsCapped.Perception;
    }

    public float MovementRunSpeed(Enums.eSpeedMeasure measure, bool uncapped)
    {
        var speed = uncapped ? _totals.RunSpd : _totalsCapped.RunSpd;
        return ConvertSpeed(speed, measure);
    }

    public float MovementFlySpeed(Enums.eSpeedMeasure measure, bool uncapped)
    {
        var speed = uncapped ? _totals.FlySpd : Math.Min(_totals.FlySpd, _totals.MaxFlySpd);
        return ConvertSpeed(speed, measure);
    }

    public float MovementJumpSpeed(Enums.eSpeedMeasure measure, bool uncapped)
    {
        var speed = uncapped ? _totals.JumpSpd : Math.Min(_totals.JumpSpd, _totals.MaxJumpSpd);
        return ConvertSpeed(speed, measure);
    }

    public float MovementJumpHeight(Enums.eSpeedMeasure measure)
    {
        return measure is Enums.eSpeedMeasure.KilometersPerHour or Enums.eSpeedMeasure.MetersPerSecond
            ? _totalsCapped.JumpHeight * 0.3048f
            : _totalsCapped.JumpHeight;
    }

    public float Distance(float value, Enums.eSpeedMeasure measure)
    {
        return measure switch
        {
            Enums.eSpeedMeasure.MetersPerSecond or Enums.eSpeedMeasure.KilometersPerHour => value * 0.3048f,
            _ => value
        };
    }

    private float EnduranceRecovery(bool uncapped) => uncapped ? _totals.EndRec + 1f : _totalsCapped.EndRec + 1f;
    private float HealthRegen(bool uncapped) => uncapped ? _totals.HPRegen + 1f : _totalsCapped.HPRegen + 1f;

    private static float ConvertSpeed(float speed, Enums.eSpeedMeasure measure)
    {
        return measure switch
        {
            Enums.eSpeedMeasure.FeetPerSecond => speed,
            Enums.eSpeedMeasure.MetersPerSecond => speed * 0.3048f,
            Enums.eSpeedMeasure.MilesPerHour => speed * 0.6818182f,
            Enums.eSpeedMeasure.KilometersPerHour => speed * 1.09728f,
            _ => speed
        };
    }
}

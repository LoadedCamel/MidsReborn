
using Base.Data_Classes;

public class Statistics
{
    // Base speeds
    public const float BaseFlyMax = 21f; //BaseFlySpeed is 1.5x the value of other base speeds
    public const float BaseFlySpeed = 31.5f;
    public const float BaseJumpHeight = 4f;
    public const float BaseJumpSpeed = 21f;
    public const float BaseRunSpeed = 21f;
    // Unenhanced max speeds
    public const float MaxFlySpeed = 86f;
    public const float MaxRunSpeed = 135.67f;
    public const float MaxJumpSpeed = 114.4f;
    // Hard cap speeds
    public const float CapRunSpeed = 135.67f;
    public const float CapJumpSpeed = 114.4f;
    public const float CapFlySpeed = 128.99f;
    // Misc
    internal const float BaseMagic = 1.666667f;
    public const float BasePerception = 500f;
    readonly Character _character;


    public float EnduranceMaxEnd => _character.Totals.EndMax + 100f;

    public float EnduranceRecoveryNumeric
            => (float)(EnduranceRecovery(false) * (double)(_character.Archetype.BaseRecovery * BaseMagic) * (_character.TotalsCapped.EndMax / 100.0 + 1.0));

    public float EnduranceTimeToFull => EnduranceMaxEnd / EnduranceRecoveryNumeric;

    public float EnduranceRecoveryNet => EnduranceRecoveryNumeric - EnduranceUsage;

    public float EnduranceRecoveryLossNet => (float)-(EnduranceRecoveryNumeric - (double)EnduranceUsage);

    public float EnduranceTimeToZero =>EnduranceMaxEnd / (float)-(EnduranceRecoveryNumeric - (double)EnduranceUsage);

    public float EnduranceTimeToFullNet =>EnduranceMaxEnd / (EnduranceRecoveryNumeric - EnduranceUsage);

    public float EnduranceUsage => _character.Totals.EndUse;

    public float HealthRegenHealthPerSec =>(float)(HealthRegen(false) * (double)_character.Archetype.BaseRegen * 1.66666662693024);

    public float HealthRegenHPPerSec =>
        (float)(HealthRegen(false) * (double)_character.Archetype.BaseRegen * 1.66666662693024 * HealthHitpointsNumeric(false)
        / 100.0);

    public float HealthRegenTimeToFull => HealthHitpointsNumeric(false) / HealthRegenHPPerSec;

    public float HealthHitpointsPercentage => (float)(_character.TotalsCapped.HPMax / (double)_character.Archetype.Hitpoints * 100.0);

    public float BuffToHit => _character.Totals.BuffToHit * 100f;

    public float BuffAccuracy => _character.Totals.BuffAcc * 100f;

    public float BuffEndRdx => _character.Totals.BuffEndRdx * 100f;

    public float ThreatLevel => (float)((_character.Totals.ThreatLevel + (double)_character.Archetype.BaseThreat) * 100.0);

    internal Statistics(Character character) => _character = character;

    float EnduranceRecovery(bool uncapped)
        => uncapped ? _character.Totals.EndRec + 1f : _character.TotalsCapped.EndRec + 1f;

    public float EnduranceRecoveryPercentage(bool uncapped)
        => EnduranceRecovery(uncapped) * 100f;

    float HealthRegen(bool uncapped)
        => uncapped ? _character.Totals.HPRegen + 1f : _character.TotalsCapped.HPRegen + 1f;

    public float HealthRegenPercent(bool uncapped)
        => HealthRegen(uncapped) * 100f;

    public float HealthHitpointsNumeric(bool uncapped)
        => uncapped ? _character.Totals.HPMax : _character.TotalsCapped.HPMax;

    public float DamageResistance(int dType, bool uncapped)
        => uncapped ? _character.Totals.Res[dType] * 100f : _character.TotalsCapped.Res[dType] * 100f;

    public float Perception(bool uncapped)
        => uncapped ? _character.Totals.Perception : _character.TotalsCapped.Perception;

    public float Defense(int dType)
        => _character.Totals.Def[dType] * 100f;

    public float Speed(float iSpeed, Enums.eSpeedMeasure unit)
    {
        float num;
        switch (unit)
        {
            case Enums.eSpeedMeasure.FeetPerSecond:
                num = iSpeed;
                break;
            case Enums.eSpeedMeasure.MetersPerSecond:
                num = iSpeed * 0.3048f;
                break;
            case Enums.eSpeedMeasure.MilesPerHour:
                num = iSpeed * 0.6818182f;
                break;
            case Enums.eSpeedMeasure.KilometersPerHour:
                num = iSpeed * 1.09728f;
                break;
            default:
                num = iSpeed;
                break;
        }
        return num;
    }

    public float Distance(float iDist, Enums.eSpeedMeasure unit)
    {
        float num;
        switch (unit)
        {
            case Enums.eSpeedMeasure.FeetPerSecond:
                num = iDist;
                break;
            case Enums.eSpeedMeasure.MetersPerSecond:
                num = iDist * 0.3048f;
                break;
            case Enums.eSpeedMeasure.MilesPerHour:
                num = iDist;
                break;
            case Enums.eSpeedMeasure.KilometersPerHour:
                num = iDist * 0.3048f;
                break;
            default:
                num = iDist;
                break;
        }
        return num;
    }

    public float MovementRunSpeed(Enums.eSpeedMeasure sType, bool uncapped)
    {
        float iSpeed = _character.Totals.RunSpd;
        if (!uncapped && _character.Totals.RunSpd > (double)_character.Totals.MaxRunSpd)
            iSpeed = _character.Totals.MaxRunSpd;
        return Speed(iSpeed, sType);
    }

    public float MovementFlySpeed(Enums.eSpeedMeasure sType, bool uncapped)
    {
        float iSpeed = _character.Totals.FlySpd;
        if (!uncapped && _character.Totals.FlySpd > (double)_character.Totals.MaxFlySpd)
            iSpeed = _character.Totals.MaxFlySpd;
        return Speed(iSpeed, sType);
    }

    public float MovementJumpSpeed(Enums.eSpeedMeasure sType, bool uncapped)
    {
        float iSpeed = _character.Totals.JumpSpd;
        if (!uncapped && _character.Totals.JumpSpd > (double)_character.Totals.MaxJumpSpd)
            iSpeed = _character.Totals.MaxJumpSpd;
        return Speed(iSpeed, sType);
    }

    public float MovementJumpHeight(Enums.eSpeedMeasure sType)
        => sType == Enums.eSpeedMeasure.KilometersPerHour | sType == Enums.eSpeedMeasure.MetersPerSecond ? _character.TotalsCapped.JumpHeight * 0.3048f : _character.TotalsCapped.JumpHeight;

    public float BuffHaste(bool uncapped)
     => !uncapped ? (float)((_character.TotalsCapped.BuffHaste + 1.0) * 100.0) : (float)((_character.Totals.BuffHaste + 1.0) * 100.0);

    public float BuffDamage(bool uncapped)
     => !uncapped ? (float)((_character.TotalsCapped.BuffDam + 1.0) * 100.0) : (float)((_character.Totals.BuffDam + 1.0) * 100.0);
}

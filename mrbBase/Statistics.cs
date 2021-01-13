using mrbBase.Base.Data_Classes;

namespace mrbBase
{
    public class Statistics
    {
        public const float MaxRunSpeed = 135.67f;
        public const float MaxJumpSpeed = 114.4f;
        public const float MaxFlySpeed = 86f;
        public const float CapRunSpeed = 135.67f;
        public const float CapJumpSpeed = 114.4f;
        public const float CapFlySpeed = 128.99f;
        public const float BaseRunSpeed = 21f;
        public const float BaseJumpSpeed = 21f;
        public const float BaseJumpHeight = 4f;
        public const float BaseFlySpeed = 31.5f;
        internal const float BaseMagic = 1.666667f;
        public const float BasePerception = 500f;
        public const float MaxDefenseDebuffRes = 95f;
        public const float MaxGenericDebuffRes = 100f; // All but defense that has a specific value
        private readonly Character _character;

        internal Statistics(Character character)
        {
            _character = character;
        }


        public float EnduranceMaxEnd => _character.Totals.EndMax + 100f;

        public float EnduranceRecoveryNumeric
            => EnduranceRecovery(false) * (_character.Archetype.BaseRecovery * BaseMagic) *
               (_character.TotalsCapped.EndMax / 100 + 1);

        public float EnduranceRecoveryNumericUncapped
            => EnduranceRecovery(true) * (_character.Archetype.BaseRecovery * BaseMagic) *
               (_character.Totals.EndMax / 100 + 1);

        public float EnduranceTimeToFull => EnduranceMaxEnd / EnduranceRecoveryNumeric;

        public float EnduranceRecoveryNet => EnduranceRecoveryNumeric - EnduranceUsage;

        public float EnduranceRecoveryLossNet => (float) -(EnduranceRecoveryNumeric - (double) EnduranceUsage);

        public float EnduranceTimeToZero => EnduranceMaxEnd / (float) -(EnduranceRecoveryNumeric - (double) EnduranceUsage);

        public float EnduranceTimeToFullNet => EnduranceMaxEnd / (EnduranceRecoveryNumeric - EnduranceUsage);

        public float EnduranceUsage => _character.Totals.EndUse;

        public float HealthRegenHealthPerSec =>
            (float) (HealthRegen(false) * (double) _character.Archetype.BaseRegen * 1.66666662693024);

        public float HealthRegenHPPerSec =>
            (float) (HealthRegen(false) * (double) _character.Archetype.BaseRegen * 1.66666662693024 *
                     HealthHitpointsNumeric(false)
                     / 100.0);

        public float HealthRegenTimeToFull => HealthHitpointsNumeric(false) / HealthRegenHPPerSec;

        public float HealthHitpointsPercentage =>
            (float) (_character.TotalsCapped.HPMax / (double) _character.Archetype.Hitpoints * 100.0);

        public float BuffToHit => _character.Totals.BuffToHit * 100f;

        public float BuffAccuracy => _character.Totals.BuffAcc * 100f;

        public float BuffEndRdx => _character.Totals.BuffEndRdx * 100f;

        public float ThreatLevel =>
            (float) ((_character.Totals.ThreatLevel + (double) _character.Archetype.BaseThreat) * 100.0);

        private float EnduranceRecovery(bool uncapped)
        {
            return uncapped ? _character.Totals.EndRec + 1f : _character.TotalsCapped.EndRec + 1f;
        }

        public float EnduranceRecoveryPercentage(bool uncapped)
        {
            return EnduranceRecovery(uncapped) * 100f;
        }

        private float HealthRegen(bool uncapped)
        {
            return uncapped ? _character.Totals.HPRegen + 1f : _character.TotalsCapped.HPRegen + 1f;
        }

        public float HealthRegenPercent(bool uncapped)
        {
            return HealthRegen(uncapped) * 100f;
        }

        public float HealthHitpointsNumeric(bool uncapped)
        {
            return uncapped ? _character.Totals.HPMax : _character.TotalsCapped.HPMax;
        }

        // Zed: No need for this anymore, absorb is always a flat value.
        // Ref: MidsReborn\clsToonX.cs, GBD_Stage method.
        //public float Absorb => (_character.Totals.Absorb < 1 ? _character.Totals.Absorb * _character.Archetype.Hitpoints : _character.Totals.Absorb);
        public float Absorb => _character.Totals.Absorb;

        public float DamageResistance(int dType, bool uncapped)
        {
            return uncapped ? _character.Totals.Res[dType] * 100f : _character.TotalsCapped.Res[dType] * 100f;
        }

        public float Perception(bool uncapped)
        {
            return uncapped ? _character.Totals.Perception : _character.TotalsCapped.Perception;
        }

        public float Defense(int dType)
        {
            return _character.Totals.Def[dType] * 100f;
        }

        public float Speed(float iSpeed, Enums.eSpeedMeasure unit)
        {
            var num = unit switch
            {
                Enums.eSpeedMeasure.FeetPerSecond => iSpeed,
                Enums.eSpeedMeasure.MetersPerSecond => iSpeed * 0.3048f,
                Enums.eSpeedMeasure.MilesPerHour => iSpeed * 0.6818182f,
                Enums.eSpeedMeasure.KilometersPerHour => iSpeed * 1.09728f,
                _ => iSpeed
            };
            return num;
        }

        public float Distance(float iDist, Enums.eSpeedMeasure unit)
        {
            var num = unit switch
            {
                Enums.eSpeedMeasure.FeetPerSecond => iDist,
                Enums.eSpeedMeasure.MetersPerSecond => iDist * 0.3048f,
                Enums.eSpeedMeasure.MilesPerHour => iDist,
                Enums.eSpeedMeasure.KilometersPerHour => iDist * 0.3048f,
                _ => iDist
            };
            return num;
        }

        public float MovementRunSpeed(Enums.eSpeedMeasure sType, bool uncapped)
        {
            var iSpeed = _character.Totals.RunSpd;
            if (!uncapped && _character.Totals.RunSpd > (double) _character.Totals.MaxRunSpd)
                iSpeed = _character.Totals.MaxRunSpd;
            return Speed(iSpeed, sType);
        }

        public float MovementFlySpeed(Enums.eSpeedMeasure sType, bool uncapped)
        {
            var iSpeed = _character.Totals.FlySpd;
            if (!uncapped && _character.Totals.FlySpd > (double) _character.Totals.MaxFlySpd)
                iSpeed = _character.Totals.MaxFlySpd;
            return Speed(iSpeed, sType);
        }

        public float MovementJumpSpeed(Enums.eSpeedMeasure sType, bool uncapped)
        {
            var iSpeed = _character.Totals.JumpSpd;
            if (!uncapped && _character.Totals.JumpSpd > (double) _character.Totals.MaxJumpSpd)
                iSpeed = _character.Totals.MaxJumpSpd;
            return Speed(iSpeed, sType);
        }

        public float MovementJumpHeight(Enums.eSpeedMeasure sType)
        {
            return (sType == Enums.eSpeedMeasure.KilometersPerHour) | (sType == Enums.eSpeedMeasure.MetersPerSecond)
                ? _character.TotalsCapped.JumpHeight * 0.3048f
                : _character.TotalsCapped.JumpHeight;
        }

        public float BuffHaste(bool uncapped)
        {
            return !uncapped
                ? (float) ((_character.TotalsCapped.BuffHaste + 1.0) * 100.0)
                : (float) ((_character.Totals.BuffHaste + 1.0) * 100.0);
        }

        public float BuffDamage(bool uncapped)
        {
            return !uncapped
                ? (float) ((_character.TotalsCapped.BuffDam + 1.0) * 100.0)
                : (float) ((_character.Totals.BuffDam + 1.0) * 100.0);
        }
    }
}
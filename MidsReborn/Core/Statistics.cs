using System;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core
{
    public class Statistics
    {
        public static readonly float MaxRunSpeed = DatabaseAPI.ServerData.MaxRunSpeed;
        public static readonly float MaxJumpSpeed = DatabaseAPI.ServerData.MaxJumpSpeed;
        public static readonly float MaxFlySpeed = DatabaseAPI.ServerData.MaxFlySpeed;
        public static readonly float MaxMaxRunSpeed = DatabaseAPI.ServerData.MaxMaxRunSpeed;
        public static readonly float MaxMaxJumpSpeed = DatabaseAPI.ServerData.MaxMaxJumpSpeed;
        public static readonly float MaxMaxFlySpeed = DatabaseAPI.ServerData.MaxMaxFlySpeed;
        public const float CapRunSpeed = 135.67f;
        public const float CapJumpSpeed = 114.4f;
        public const float CapFlySpeed = 128.99f;
        public static readonly float BaseRunSpeed = DatabaseAPI.ServerData.BaseRunSpeed;
        public static readonly float BaseJumpSpeed = DatabaseAPI.ServerData.BaseJumpSpeed;
        public static readonly float BaseJumpHeight = DatabaseAPI.ServerData.BaseJumpHeight;
        public static readonly float BaseFlySpeed = DatabaseAPI.ServerData.BaseFlySpeed;
        internal const float BaseMagic = 1.666667f;
        public static readonly float BasePerception = DatabaseAPI.ServerData.BasePerception;
        public const float MaxDefenseDebuffRes = 95f;
        public const float MaxGenericDebuffRes = 100f; // All but defense that has a specific value
        public const float MaxHaste = 400f;
        private readonly Character _character;

        internal Statistics(Character character)
        {
            _character = character;
        }


        public float EnduranceMaxEnd => _character.Totals.EndMax + 100f;

        public float EnduranceRecoveryNumeric => EnduranceRecovery(false) * (_character.Archetype.BaseRecovery * BaseMagic) * (_character.TotalsCapped.EndMax / 100 + 1);

        public float EnduranceRecoveryNumericUncapped => EnduranceRecovery(true) * (_character.Archetype.BaseRecovery * BaseMagic) * (_character.Totals.EndMax / 100 + 1);

        public float EnduranceTimeToFull => EnduranceMaxEnd / EnduranceRecoveryNumeric;

        public float EnduranceRecoveryNet => EnduranceRecoveryNumeric - EnduranceUsage;

        public float EnduranceRecoveryLossNet => (float) -(EnduranceRecoveryNumeric - (double) EnduranceUsage);

        public float EnduranceTimeToZero => EnduranceMaxEnd / (float) -(EnduranceRecoveryNumeric - (double) EnduranceUsage);

        public float EnduranceTimeToFullNet => EnduranceMaxEnd / (EnduranceRecoveryNumeric - EnduranceUsage);

        public float EnduranceUsage => _character.Totals.EndUse;

        public float HealthRegenHealthPerSec => (float) (HealthRegen(false) * (double) _character.Archetype.BaseRegen * 1.66666662693024);

        public float HealthRegenHPPerSec => (float) (HealthRegen(false) * (double) _character.Archetype.BaseRegen * 1.66666662693024 * HealthHitpointsNumeric(false) / 100.0);

        public float HealthRegenTimeToFull => HealthHitpointsNumeric(false) / HealthRegenHPPerSec;

        public float HealthHitpointsPercentage => (float) (_character.TotalsCapped.HPMax / (double) _character.Archetype.Hitpoints * 100.0);

        public float BuffToHit => _character.Totals.BuffToHit * 100f;

        public float BuffAccuracy => _character.Totals.BuffAcc * 100f;

        public float BuffEndRdx => _character.Totals.BuffEndRdx * 100f;

        public float ThreatLevel => (float) ((_character.Totals.ThreatLevel + (double) _character.Archetype.BaseThreat) * 100.0);

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

        public float Range => _character.Totals.BuffRange;

        public float RangePercent => _character.Totals.BuffRange * 100;

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
            return unit switch
            {
                Enums.eSpeedMeasure.FeetPerSecond => iSpeed,
                Enums.eSpeedMeasure.MetersPerSecond => iSpeed * 0.3048f,
                Enums.eSpeedMeasure.MilesPerHour => iSpeed * 0.6818182f,
                Enums.eSpeedMeasure.KilometersPerHour => iSpeed * 1.09728f,
                _ => iSpeed
            };
        }

        public float Speed(float iSpeed, Enums.eSpeedMeasure unit, Enums.eSpeedMeasure baseUnit)
        {
            return baseUnit switch
            {
                Enums.eSpeedMeasure.FeetPerSecond => unit switch
                {
                    Enums.eSpeedMeasure.FeetPerSecond => iSpeed,
                    Enums.eSpeedMeasure.MetersPerSecond => iSpeed * 0.3048f,
                    Enums.eSpeedMeasure.MilesPerHour => iSpeed * 0.6818182f,
                    Enums.eSpeedMeasure.KilometersPerHour => iSpeed * 1.09728f,
                    _ => iSpeed
                },

                Enums.eSpeedMeasure.MetersPerSecond => unit switch
                {
                    Enums.eSpeedMeasure.FeetPerSecond => iSpeed * 3.2808399f,
                    Enums.eSpeedMeasure.MetersPerSecond => iSpeed,
                    Enums.eSpeedMeasure.MilesPerHour => iSpeed * 2.23693629f,
                    Enums.eSpeedMeasure.KilometersPerHour => iSpeed * 3.6f,
                    _ => iSpeed
                },

                Enums.eSpeedMeasure.MilesPerHour => unit switch
                {
                    Enums.eSpeedMeasure.FeetPerSecond => iSpeed * 1.46666667f,
                    Enums.eSpeedMeasure.MetersPerSecond => iSpeed * 0.44704f,
                    Enums.eSpeedMeasure.MilesPerHour => iSpeed,
                    Enums.eSpeedMeasure.KilometersPerHour => iSpeed * 1.60934387f,
                    _ => iSpeed
                },

                Enums.eSpeedMeasure.KilometersPerHour => unit switch
                {
                    Enums.eSpeedMeasure.FeetPerSecond => iSpeed * 0.911344488f,
                    Enums.eSpeedMeasure.MetersPerSecond => iSpeed * 0.2777778f,
                    Enums.eSpeedMeasure.MilesPerHour => iSpeed * 0.621371242f,
                    Enums.eSpeedMeasure.KilometersPerHour => iSpeed,
                    _ => iSpeed
                }
            };
        }

        public float Distance(float iDist, Enums.eSpeedMeasure unit)
        {
            return unit switch
            {
                Enums.eSpeedMeasure.FeetPerSecond => iDist,
                Enums.eSpeedMeasure.MetersPerSecond => iDist * 0.3048f,
                Enums.eSpeedMeasure.MilesPerHour => iDist,
                Enums.eSpeedMeasure.KilometersPerHour => iDist * 0.3048f,
                _ => iDist
            };
        }

        public float MovementRunSpeed(Enums.eSpeedMeasure sType, bool uncapped)
        {
            var iSpeed = uncapped
                ? MidsContext.Character?.Totals.RunSpd
                : MidsContext.Character?.TotalsCapped.RunSpd;

            return Speed(iSpeed ?? 0, sType);
        }

        public float MovementRunSpeed(Enums.eSpeedMeasure sType, Enums.eSpeedMeasure baseUnit, bool uncapped)
        {
            var iSpeed = uncapped
                ? MidsContext.Character?.Totals.RunSpd
                : MidsContext.Character?.TotalsCapped.RunSpd;

            return Speed(iSpeed ?? 0, sType, baseUnit);
        }

        public float MovementFlySpeed(Enums.eSpeedMeasure sType, bool uncapped)
        {
            var iSpeed = _character.Totals.FlySpd;
            if (!uncapped && _character.Totals.FlySpd > _character.Totals.MaxFlySpd)
            {
                iSpeed = _character.Totals.MaxFlySpd;
            }

            return Speed(iSpeed, sType);
        }

        public float MovementJumpSpeed(Enums.eSpeedMeasure sType, bool uncapped)
        {
            var iSpeed = _character.Totals.JumpSpd;
            if (!uncapped && _character.Totals.JumpSpd > _character.Totals.MaxJumpSpd)
            {
                iSpeed = _character.Totals.MaxJumpSpd;
            }

            return Speed(iSpeed, sType);
        }

        public float MovementJumpHeight(Enums.eSpeedMeasure sType)
        {
            return sType is Enums.eSpeedMeasure.KilometersPerHour or Enums.eSpeedMeasure.MetersPerSecond
                ? _character.TotalsCapped.JumpHeight * 0.3048f
                : _character.TotalsCapped.JumpHeight;
        }

        public float BuffHaste(bool uncapped)
        {
            return !uncapped
                ? Math.Min(MaxHaste, (_character.TotalsCapped.BuffHaste + 1) * 100)
                : (_character.Totals.BuffHaste + 1) * 100;
        }

        public float BuffDamage(bool uncapped)
        {
            return !uncapped
                ? Math.Min(_character.Archetype?.DamageCap * 100 ?? float.PositiveInfinity, (_character.TotalsCapped.BuffDam + 1) * 100)
                : (_character.Totals.BuffDam + 1) * 100;
        }
    }
}
using FastDeepCloner;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static Mids_Reborn.UI.Forms.WindowMenuItems.frmStats;

namespace Mids_Reborn.Core
{
    public class StatsPowerData
    {
        public struct PowerValueInfo
        {
            public string PowerName;
            public bool PowerTaken;
            public float BaseValue;
            public float? EnhValue;
            public float? UncappedValue;
            public int? Stacks;
            public string UnitSuffix;
            public string Tip;

            public override string ToString()
            {
                return $"<PowerValueInfoExt> {{PowerName={PowerName}, PowerTaken={PowerTaken}, BaseValue={BaseValue}, EnhValue={(EnhValue == null ? "(null)" : EnhValue)}, UncappedValue={(UncappedValue == null ? "(null)" : UncappedValue)}, Stacks={(Stacks == null ? "(null)" : Stacks)}, UnitSuffix={(string.IsNullOrWhiteSpace(UnitSuffix) ? "(empty)" : UnitSuffix)}, Tip={Tip.Replace("\r\n", "\n").Replace("\n", " ")}";
            }
        }

        public struct PowerValueInfoExt
        {
            public string PowerName;
            public bool PowerTaken;
            public IPower? Power;
            public float BaseValue;
            public float? EnhValue;
            public float? UncappedValue;
            public int? Stacks;
            public string UnitSuffix;
            public string Tip;

            public override string ToString()
            {
                return $"<PowerValueInfoExt> {{PowerName={PowerName}, PowerTaken={PowerTaken}, Power={(Power == null ? "(null)" : Power.FullName)}, BaseValue={BaseValue}, EnhValue={(EnhValue == null ? "(null)" : EnhValue)}, UncappedValue={(UncappedValue == null ? "(null)" : UncappedValue)}, Stacks={(Stacks == null ? "(null)" : Stacks)}, UnitSuffix={(string.IsNullOrWhiteSpace(UnitSuffix) ? "(empty)" : UnitSuffix)}, Tip={Tip.Replace("\r\n", "\n").Replace("\n", " ")}";
            }
        }

        public struct BuildMetadata
        {
            public string? Name;
            public string PvMode;
            public string? BuildFile;
            public string Archetype;
            public string[] Powersets;
            public Enums.eSpeedMeasure SpeedFormat;
        }

        public struct TotalStat : IEquatable<TotalStat>
        {
            public string DisplayName;
            public float BaseValue;
            public float? EnhValue;
            public float? UncappedValue;
            public string Tip;

            public bool Equals(TotalStat other)
            {
                return DisplayName.Equals(other.DisplayName) && BaseValue.Equals(other.BaseValue) && Nullable.Equals(EnhValue, other.EnhValue) && Nullable.Equals(UncappedValue, other.UncappedValue) && Tip.Equals(other.Tip);
            }

            public override bool Equals(object? obj)
            {
                return obj is TotalStat other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(BaseValue, EnhValue, UncappedValue);
            }
        }

        public class TotalStats
        {
            public TotalStat[] Defense = [];
            public TotalStat[] Resistance = [];
            public TotalStat[] Health = [];
            public TotalStat[] Endurance = [];
            public TotalStat[] Movement = [];
            public TotalStat[] Stealth = [];
            public TotalStat[] MiscBuffs = [];
            public TotalStat[] StatusProtection = [];
            public TotalStat[] StatusResistance = [];
            public TotalStat[] DebuffResistance = [];
            public TotalStat[] Elusivity = [];
        }

        public enum TotalStatsGroups
        {
            Defense,
            Resistance,
            Health,
            Endurance,
            Movement,
            Stealth,
            MiscBuffs,
            StatusProtection,
            StatusResistance,
            DebuffResistance,
            Elusivity
        }

        public Dictionary<DisplayMode, PowerValueInfo[]>? PowerInfo { get; set; } = new();
        public TotalStats? Totals { get; set; } = new();
        public BuildMetadata Metadata { get; set; }

        #region Metadata helpers

        public void SetMetadata(string? name, string? buildFile, string archetype, string[] powersets, Enums.eSpeedMeasure speedFormat)
        {
            Metadata = new BuildMetadata
            {
                Name = name,
                PvMode = MidsContext.Config.Inc.DisablePvE ? "PvP" : "PvE",
                BuildFile = buildFile,
                Archetype = archetype,
                Powersets = powersets,
                SpeedFormat = MidsContext.Config?.SpeedFormat ?? Enums.eSpeedMeasure.MilesPerHour
            };
        }

        #endregion

        #region Powers infos groups helpers

        public bool AddGroup(DisplayMode displayMode, PowerValueInfo[] pwValues)
        {
            return PowerInfo.TryAdd(displayMode, pwValues);
        }

        public PowerValueInfo[] GetGroupData(DisplayMode displayMode)
        {
            return PowerInfo.TryGetValue(displayMode, out var groupData)
                ? groupData
                : [];
        }

        #endregion

        #region Json Import/Export

        public string ExportToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static StatsPowerData? ImportFromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<StatsPowerData>(jsonString, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        }

        #endregion

        #region Total stats helpers

        public static TotalStats GetTotals(bool forcePvP = false)
        {
            var excludedDefVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.None,
                DatabaseAPI.RealmUsesToxicDef() ? Enums.eDamage.None : Enums.eDamage.Toxic,
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>().ToList();

            var excludedResVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.None,
                Enums.eDamage.Melee,
                Enums.eDamage.Ranged,
                Enums.eDamage.AoE,
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>().ToList();

            var excludedElusivityVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>().ToList();

            Enums.eMez[] mezList =
            [
                Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep, Enums.eMez.Immobilized,
                Enums.eMez.Knockback, Enums.eMez.Repel, Enums.eMez.Confused, Enums.eMez.Terrorized,
                Enums.eMez.Taunt, Enums.eMez.Placate, Enums.eMez.Teleport
            ];

            Enums.eEffectType[] debuffEffectsList =
            [
                Enums.eEffectType.Defense, Enums.eEffectType.Endurance, Enums.eEffectType.Recovery,
                Enums.eEffectType.PerceptionRadius, Enums.eEffectType.ToHit, Enums.eEffectType.RechargeTime,
                Enums.eEffectType.SpeedRunning, Enums.eEffectType.Regeneration
            ];

            var displayStats = MidsContext.Character.DisplayStats;
            var flySpeedValue = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, false);
            var hpBase = MidsContext.Character.Archetype.Hitpoints;
            var mezProtections = MidsContext.Character.Totals.Mez
                .Select(e => e > 0 ? 0 : Math.Abs(e))
                .ToArray();
            var cappedDebuffRes = debuffEffectsList.Select(e => Math.Min(
                    e == Enums.eEffectType.Defense
                        ? Statistics.MaxDefenseDebuffRes
                        : Statistics.MaxGenericDebuffRes,
                    MidsContext.Character.Totals.DebuffRes[(int)e]))
                .ToArray();
            var uncappedDebuffRes = debuffEffectsList.Select(e => MidsContext.Character.Totals.DebuffRes[(int)e]).ToArray();

            return new TotalStats
            {
                Defense = Enum.GetValues<Enums.eDamage>()
                    .Cast<int>()
                    .Where(e => !excludedDefVectors.Contains(e))
                    .Select(e => new TotalStat { DisplayName = $"{(Enums.eDamage)e}", BaseValue = 0, EnhValue = displayStats.Defense(e), UncappedValue = null, Tip = $"{displayStats.Defense(e):##0.##}% {(Enums.eDamage)e} Defense" })
                    .ToArray(),

                Resistance = Enum.GetValues<Enums.eDamage>()
                        .Cast<int>()
                        .Where(e => !excludedResVectors.Contains(e))
                        .Select(e => new TotalStat
                        {
                            DisplayName = $"{(Enums.eDamage)e}",
                            BaseValue = 0,
                            EnhValue = displayStats.DamageResistance(e, false),
                            UncappedValue = displayStats.DamageResistance(e, true),
                            Tip = displayStats.DamageResistance(e, true) > displayStats.DamageResistance(e, false)
                                ? $"{displayStats.DamageResistance(e, true):##0.##}% {(Enums.eDamage)e} Resistance (capped at {displayStats.DamageResistance(e, false):##0.##}%)"
                                : $"{displayStats.DamageResistance(e, false):##0.##}% {(Enums.eDamage)e} Resistance"
                        })
                        .ToArray(),

                Health =
                [
                    new TotalStat
                    {
                        DisplayName = "Regeneration",
                        BaseValue = 100,
                        EnhValue = displayStats.HealthRegenPercent(false),
                        UncappedValue = displayStats.HealthRegenPercent(true),
                        Tip = displayStats.HealthRegenPercent(true) > displayStats.HealthRegenPercent(false)
                            ? $"{displayStats.HealthRegenPercent(true):###0.##}% Regeneration at level 50 (capped at {displayStats.HealthRegenPercent(false):###0.##}%)"
                            : $"{displayStats.HealthRegenPercent(false):###0.##}% Regeneration at level 50"
                    },
                    new TotalStat
                    {
                        DisplayName = "Max HP",
                        BaseValue = hpBase,
                        EnhValue = displayStats.HealthHitpointsNumeric(false),
                        UncappedValue = displayStats.HealthHitpointsNumeric(true),
                        Tip = displayStats.HealthHitpointsNumeric(true) > displayStats.HealthHitpointsNumeric(false)
                            ? $"{displayStats.HealthHitpointsNumeric(true):###0.##} HP (capped at {displayStats.HealthHitpointsNumeric(false):###0.##})"
                            : $"{displayStats.HealthHitpointsNumeric(false):###0.##} HP"
                    },
                    new TotalStat
                    {
                        DisplayName = "Absorb",
                        BaseValue = 0,
                        EnhValue = Math.Min(displayStats.Absorb, hpBase),
                        UncappedValue = displayStats.Absorb,
                        Tip = displayStats.Absorb > hpBase
                            ? $"{displayStats.Absorb:###0.##} Absorb (capped at base HP = {hpBase:###0.##}"
                            : $"{displayStats.Absorb:###0.##} Absorb"
                    }
                ],

                Endurance = [
                    new TotalStat
                    {
                        DisplayName = "End Rec",
                        BaseValue = MidsContext.Character.Archetype.BaseRecovery * displayStats.EnduranceMaxEnd / 60f,
                        EnhValue = displayStats.EnduranceRecoveryNumeric,
                        UncappedValue = displayStats.EnduranceRecoveryNumericUncapped,
                        Tip = displayStats.EnduranceRecoveryNumericUncapped > displayStats.EnduranceRecoveryNumeric
                            ? $"{displayStats.EnduranceRecoveryNumericUncapped:##0.##}/s Endurance Recovery (capped at {displayStats.EnduranceRecoveryNumeric:##0.##}/s)"
                            : $"{displayStats.EnduranceRecoveryNumeric:##0.##}/s Endurance Recovery"
                    },
                    new TotalStat
                    {
                        DisplayName = "End Use",
                        BaseValue = 0,
                        EnhValue = displayStats.EnduranceUsage,
                        UncappedValue = null,
                        Tip = $"Endurance Use: {displayStats.EnduranceUsage:##0.##}/s"

                    },
                    new TotalStat
                    {
                        DisplayName = "Max End",
                        BaseValue = 100,
                        EnhValue = displayStats.EnduranceMaxEnd,
                        UncappedValue = null,
                        Tip = $"Max Endurance: {displayStats.EnduranceMaxEnd}"
                    }
                ],

                Movement =
                [
                    new TotalStat
                    {
                        DisplayName = "Run Speed",
                        BaseValue = displayStats.Speed(Statistics.BaseRunSpeed, MidsContext.Config.SpeedFormat),
                        EnhValue = displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, false),
                        UncappedValue = displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, true),
                        Tip = displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, true) > displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, false)
                            ? $"{displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, true):###0.##} mph Run Speed (capped at {displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, false):###0.##} mph)"
                            : $"{displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, false):###0.##} mph Run Speed"
                    },
                    new TotalStat
                    {
                        DisplayName = "Jump Speed",
                        BaseValue = displayStats.Speed(Statistics.BaseJumpSpeed, Enums.eSpeedMeasure.FeetPerSecond),
                        EnhValue = displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, false),
                        UncappedValue = displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, true),
                        Tip = displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, true) > displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, false)
                            ? $"{displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, true):###0.##} mph Jump Speed (capped at {displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, false):###0.##} mph)"
                            : $"{displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, false):###0.##} mph Jump Speed"
                    },
                    new TotalStat
                    {
                        DisplayName = "Jump Height",
                        BaseValue = displayStats.Distance(Statistics.BaseJumpHeight, MidsContext.Config.SpeedFormat),
                        EnhValue = displayStats.MovementJumpHeight(MidsContext.Config.SpeedFormat),
                        UncappedValue = null,
                        Tip = $"{displayStats.MovementJumpHeight(MidsContext.Config.SpeedFormat):###0.##} ft Jump Height"
                    },
                    new TotalStat
                    {
                        DisplayName = "Fly Speed",
                        BaseValue = flySpeedValue == 0 ? 0 : displayStats.Speed(Statistics.BaseFlySpeed, MidsContext.Config.SpeedFormat),
                        EnhValue = flySpeedValue,
                        UncappedValue = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, true),
                        Tip = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, true) > flySpeedValue
                            ? $"{displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, true):###0.##} mph Fly Speed (capped at {flySpeedValue:###0.##} mph)"
                            : $"{flySpeedValue:###0.##} mph Fly Speed"
                    }
                ],

                Stealth = [
                    new TotalStat
                    {
                        DisplayName = "Stealth Radius (PvE)",
                        BaseValue = 0,
                        EnhValue = displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat),
                        UncappedValue = null,
                        Tip = $"Stealth Radius (PvE): {displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat):####0.##} ft"
                    },
                    new TotalStat
                    {
                        DisplayName = "Stealth Radius (PvP)",
                        BaseValue = 0,
                        EnhValue = displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat),
                        UncappedValue = null,
                        Tip = $"Stealth Radius (PvP): {displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat):####0.##} ft"
                    },
                    new TotalStat
                    {
                        DisplayName = "Perception",
                        BaseValue = displayStats.Distance(Statistics.BasePerception, MidsContext.Config.SpeedFormat),
                        EnhValue = displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat),
                        UncappedValue = displayStats.Distance(displayStats.Perception(true), MidsContext.Config.SpeedFormat),
                        Tip = displayStats.Distance(displayStats.Perception(true), MidsContext.Config.SpeedFormat) > displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat)
                            ? $"{displayStats.Distance(displayStats.Perception(true), MidsContext.Config.SpeedFormat):####0.##} ft Perception (capped at {displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat):####0.##} ft)"
                            : $"{displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat):####0.##} ft Perception"
                    }
                ],
                MiscBuffs = [
                    new TotalStat
                    {
                        DisplayName = "Haste",
                        BaseValue = 100,
                        EnhValue = Math.Max(0, displayStats.BuffHaste(false)),
                        UncappedValue = Math.Max(0, displayStats.BuffHaste(true)),
                        Tip = Math.Max(0, displayStats.BuffHaste(true)) > Math.Max(0, displayStats.BuffHaste(false))
                            ? $"{Math.Max(0, displayStats.BuffHaste(true)):###0.##}% Haste (base: 100%, capped at {Math.Max(0, displayStats.BuffHaste(false)):###0.##}%)"
                            : $"{Math.Max(0, displayStats.BuffHaste(false)):###0.##}% Haste (base: 100%)"
                    },
                    new TotalStat
                    {
                        DisplayName = "ToHit",
                        BaseValue = 0,
                        EnhValue = Math.Max(0, displayStats.BuffToHit),
                        UncappedValue = null,
                        Tip = $"{Math.Max(0, displayStats.BuffToHit):###0.##}% ToHit"
                    },
                    new TotalStat
                    {
                        DisplayName = "Accuracy",
                        BaseValue = 0,
                        EnhValue = Math.Max(0, displayStats.BuffAccuracy),
                        UncappedValue = null,
                        Tip = $"{Math.Max(0, displayStats.BuffAccuracy):###0.##}% Accuracy"
                    },
                    new TotalStat
                    {
                        DisplayName = "Damage",
                        BaseValue = 100,
                        EnhValue = Math.Max(0, displayStats.BuffDamage(false)),
                        UncappedValue = Math.Max(0, displayStats.BuffDamage(true)),
                        Tip = Math.Max(0, displayStats.BuffDamage(true)) > Math.Max(0, displayStats.BuffDamage(false))
                            ? $"{Math.Max(0, displayStats.BuffDamage(true)):###0.##}% Damage Buff (capped at {Math.Max(0, displayStats.BuffDamage(false)):###0.##}%)"
                            : $"{Math.Max(0, displayStats.BuffDamage(false)):###0.##}% Damage Buff"
                    },
                    new TotalStat
                    {
                        DisplayName = "Range",
                        BaseValue = 0,
                        EnhValue = Math.Max(0, displayStats.RangePercent),
                        UncappedValue = null,
                        Tip = $"{Math.Max(0, displayStats.RangePercent):###0.##}% Range"
                    },
                    new TotalStat
                    {
                        DisplayName = "EndRdx",
                        BaseValue = 0,
                        EnhValue = displayStats.BuffEndRdx,
                        UncappedValue = null,
                        Tip = $"{displayStats.BuffEndRdx:###0.##}% Endurance Cost Reduction"
                    },
                    new TotalStat
                    {
                        DisplayName = "Threat",
                        BaseValue = MidsContext.Character.Archetype.BaseThreat * 100,
                        EnhValue = displayStats.ThreatLevel,
                        UncappedValue = null,
                        Tip = $"{displayStats.ThreatLevel:###0.##}% Threat (base: {MidsContext.Character.Archetype.BaseThreat * 100:###0.##}%)"
                    }
                ],

                StatusProtection = mezList
                    .Cast<int>()
                    .Select(e => new TotalStat { DisplayName = $"{(Enums.eMez)e}", BaseValue = 0, EnhValue = mezProtections[e], UncappedValue = null, Tip = $"{mezProtections[e]:####0.##} Protection Magnitude to {(Enums.eMez)e}" })
                    .ToArray(),

                StatusResistance = mezList
                    .Cast<int>()
                    .Select(e => new TotalStat { DisplayName = $"{(Enums.eMez)e}", BaseValue = 0, EnhValue = MidsContext.Character.Totals.MezRes[e], UncappedValue = null, Tip = $"{MidsContext.Character.Totals.MezRes[e]:####0.##}% Resistance to {(Enums.eMez)e}" })
                    .ToArray(),

                DebuffResistance = debuffEffectsList
                    .Select((e, i) => new TotalStat
                    {
                        DisplayName = $"{e}",
                        BaseValue = 0,
                        EnhValue = cappedDebuffRes[i],
                        UncappedValue = uncappedDebuffRes[i],
                        Tip = uncappedDebuffRes[i] > cappedDebuffRes[i]
                            ? $"{uncappedDebuffRes[i]:####0.##}% Resistance to {e} debuffs (capped at {cappedDebuffRes[i]:####0.##}%)"
                            : $"{cappedDebuffRes[i]:####0.##}% Resistance to {e} debuffs"
                    })
                    .ToArray(),
                Elusivity = Enum.GetValues<Enums.eDamage>()
                    .Cast<int>()
                    .Where(e => !excludedElusivityVectors.Contains(e))
                    .Select(e => new TotalStat { DisplayName = $"{((Enums.eDamage)e == Enums.eDamage.None ? "Untyped" : (Enums.eDamage)e)}", BaseValue = 0, EnhValue = (MidsContext.Character.Totals.Elusivity[e]) * 100, UncappedValue = null, Tip = $"{(MidsContext.Character.Totals.Elusivity[e]) * 100:####0.##}% {(Enums.eDamage)e} Damage Elusivity (in {(MidsContext.Config.Inc.DisablePvE ? "PvP" : "PvE")})"})
                    .ToArray()
            };
        }

        public void SetTotals()
        {
            Totals = GetTotals();
        }

        #endregion

        #region frmStats specific helpers

        public static IPower?[][]? GetPowerStatsArray(int powersFilterIdx = 0, DisplayMode statDisplayed = DisplayMode.Damage)
        {
            if ((MainModule.MidsController.Toon == null) | !MainModule.MidsController.IsAppInitialized)
            {
                return null;
            }

            powersFilterIdx = Math.Max(0, powersFilterIdx);

            if ((int)statDisplayed > 13)
            {
                return null;
            }

            // Powers in build
            var powers = new Dictionary<int, KeyValuePair<IPower, IPower>>();
            for (var i = 0; i < MidsContext.Character?.CurrentBuild?.Powers.Count; i++)
            {
                var pBase = MainModule.MidsController.Toon?.GetBasePower(i);
                var pEnh = MainModule.MidsController.Toon?.GetEnhancedPower(i);

                if (pBase == null)
                {
                    continue;
                }

                var baseDmg = pBase.FXGetDamageValue();
                var enhDmg = pEnh?.FXGetDamageValue(pEnh == null) ?? 0;

                powers.Add(i,
                    baseDmg <= enhDmg
                        ? new KeyValuePair<IPower, IPower>(pBase, pEnh)
                        : new KeyValuePair<IPower, IPower>(pEnh, pBase));
            }

            // Filter out zero damage powers (if needed)
            if (statDisplayed is DisplayMode.Damage or DisplayMode.DPA or DisplayMode.DPS or DisplayMode.DPE)
            {
                powers = powers
                .Where(e => (e.Value.Value != null && e.Value.Value.FXGetDamageValue(e.Value.Value == null) > 0) || (e.Value.Key != null && e.Value.Key.FXGetDamageValue() > 0))
                .ToDictionary(e => e.Key, e => e.Value);
            }

            // basePower FullName -> index
            var powersDict = powers
                .Select(e => new KeyValuePair<IPower, int>(e.Value.Key, e.Key))
                .ToDictionary(e => e.Key.FullName, e => e.Value);

            // Selected powersets in build
            var powersets = MainModule.MidsController.Toon?.Powersets
                .Where(e => e != null)
                .ToList();

            // Root Powers
            var rootPowers = new Dictionary<string, string>();
            foreach (var p in powers)
            {
                var basePower = p.Value.Key;
                var enhancedPower = p.Value.Value;
                var rootPowerName = Power.GetRootPowerName(basePower, enhancedPower);

                if (!string.IsNullOrEmpty(rootPowerName))
                {
                    rootPowers.Add(rootPowerName, basePower.FullName);
                }
            }

            var rootPowersNames = rootPowers
                .Select(e => e.Key)
                .ToList();

            // Powers from powersets (use build base + enhanced if available)
            var powersetsPowers = new Dictionary<int, KeyValuePair<IPower, IPower>>();
            var k = 0;
            foreach (var ps in powersets)
            {
                if (ps == null)
                {
                    continue;
                }

                foreach (var p in ps.Powers)
                {
                    if (powersDict.TryGetValue(p.FullName, out var idx1))
                    {
                        powersetsPowers.Add(k++, new KeyValuePair<IPower, IPower>(powers[idx1].Key.Clone(), powers[idx1].Value.Clone()));
                    }
                    else if (rootPowersNames.Contains(p.FullName))
                    {
                        var idx2 = powersDict[rootPowers[p.FullName]];
                        powersetsPowers.Add(k++, new KeyValuePair<IPower, IPower>(powers[idx2].Key.Clone(), powers[idx2].Value.Clone()));
                    }
                    else
                    {
                        var pBase = p.Clone();
                        pBase?.ProcessExecutes();
                        pBase?.AbsorbPetEffects();
                        powersetsPowers.Add(k++, new KeyValuePair<IPower, IPower>(pBase, pBase));
                    }
                }
            }

            // Apply selection filter
            powers = powersFilterIdx switch
            {
                // Primary/Secondary
                1 => powersetsPowers
                    .Where(e => e.Value.Key.FullName.StartsWith("Redirects.") | e.Value.Key.GetPowerSet()?.SetType is Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary)
                    .ToDictionary(e => e.Key, e => e.Value),

                // Primary
                2 => powersetsPowers
                    .Where(e => e.Value.Key.FullName.StartsWith("Redirects.") | (e.Value.Key.GetPowerSet()?.SetType == Enums.ePowerSetType.Primary))
                    .ToDictionary(e => e.Key, e => e.Value),

                // Secondary
                3 => powersetsPowers
                    .Where(e => e.Value.Key.FullName.StartsWith("Redirects.") | (e.Value.Key.GetPowerSet()?.SetType == Enums.ePowerSetType.Secondary))
                    .ToDictionary(e => e.Key, e => e.Value),

                // Epic/Ancillary
                4 => powersetsPowers
                    .Where(e => e.Value.Key.FullName.StartsWith("Redirects.") | (e.Value.Key.GetPowerSet()?.SetType == Enums.ePowerSetType.Ancillary))
                    .ToDictionary(e => e.Key, e => e.Value),

                // Pools
                5 => powersetsPowers
                    .Where(e => e.Value.Key.FullName.StartsWith("Redirects.") | (e.Value.Key.GetPowerSet()?.SetType == Enums.ePowerSetType.Pool))
                    .ToDictionary(e => e.Key, e => e.Value),

                // Powers taken
                6 => powers,

                // All toggles
                7 => powers
                    .Where(e => e.Value.Key.PowerType == Enums.ePowerType.Toggle)
                    .ToDictionary(e => e.Key, e => e.Value),

                // All clicks
                8 => powersetsPowers
                    .Where(e => e.Value.Key.PowerType == Enums.ePowerType.Click)
                    .ToDictionary(e => e.Key, e => e.Value),

                _ => powersetsPowers
            };

            // Filter inherents, prestige, etc.
            // + validate archetype
            var dbPowers = powers
                .Select(e => DatabaseAPI.GetPowerByFullName(e.Value.Key.FullName))
                .Where(e => e is not null && (MidsContext.Character.Archetype == null || e.Requires.ClassOk(MidsContext.Character.Archetype.Idx)));

            // If temp/prestige/accolade, check if they are actually active in build
            dbPowers = dbPowers
                .Where(e =>
                    e.InherentType is not (Enums.eGridType.Temp or Enums.eGridType.Prestige or Enums.eGridType.Accolade) ||
                    MidsContext.Character.CurrentBuild.FindInToonHistory(
                        DatabaseAPI.Database.Power.TryFindIndex(f => f != null && f.StaticIndex == e.StaticIndex)) >= 0);

            var validPowersNames = dbPowers.Select(e => e.FullName);
            powers = powers
                .Where(e => validPowersNames.Contains(e.Value.Key.FullName))
                .ToDictionary(e => e.Key, e => e.Value);


            // Reorder by powerset type, then powerset name, then power static index
            powers = powers.OrderBy(e => (int)e.Value.Key.GetPowerSet().SetType)
                .ThenBy(e => e.Value.Key.GetPowerSet().SetName)
                .ThenBy(e => e.Value.Key.StaticIndex)
                .ToDictionary(e => e.Key, e => e.Value);

            return
            [
                powers.Select(e => e.Value.Key).ToArray(),
                powers.Select(e => e.Value.Value).ToArray()
            ];
        }

        public static PowerValueInfoExt[]? PreparePowersGraph(IPower?[] baseArray, IPower?[] enhArray, bool baseOverride, Enums.GraphStyle graphStyle, DisplayMode statDisplayed = DisplayMode.Damage)
        {
            if ((MainModule.MidsController.Toon == null) | !MainModule.MidsController.IsAppInitialized)
            {
                return null;
            }

            if ((int)statDisplayed > 13)
            {
                return null;
            }

            var funcBase = (IPower? b, IPower? e) => 0f; // dummy initializing func
            var funcEnhanced = (IPower? b, IPower? e) => 0f;
            var funcFilterPower = (IPower? b, IPower? e) => true;
            var unitSuffix = "%";
            var returnValue = ConfigData.EDamageReturn.DPS;
            var returnValuePrev = MidsContext.Config.DamageMath.ReturnValue;

            switch (statDisplayed)
            {
                case DisplayMode.Accuracy:
                    funcBase = (b, e) =>
                    {
                        var baseAccuracy = b.Accuracy;
                        if (baseOverride)
                        {
                            return baseAccuracy * 100;
                        }

                        return MidsContext.Config.ScalingToHit * baseAccuracy * 100;
                    };

                    funcEnhanced = (b, e) =>
                    {
                        var baseAccuracy = b.Accuracy;
                        var enhAccuracy = e.Accuracy;
                        if (baseOverride)
                        {
                            return enhAccuracy * 100;
                        }

                        if (Math.Abs(e.Accuracy - baseAccuracy) < float.Epsilon)
                        {
                            enhAccuracy *= MidsContext.Config.ScalingToHit;
                        }

                        return enhAccuracy * 100;
                    };

                    funcFilterPower = (b, e) =>
                    {
                        if (Math.Abs(b.Accuracy) < float.Epsilon)
                        {
                            return false;
                        }

                        return (b.EntitiesAutoHit == Enums.eEntity.None) | ((b.Range > 20) & b.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt));
                    };

                    break;

                case DisplayMode.Damage:
                    funcBase = (b, e) => b.FXGetDamageValue();
                    funcEnhanced = (b, e) => e.FXGetDamageValue();
                    funcFilterPower = (b, e) => (Math.Abs(b.FXGetDamageValue()) >= float.Epsilon) | (Math.Abs(e.FXGetDamageValue()) >= float.Epsilon);
                    unitSuffix = "";
                    break;

                case DisplayMode.DPA:
                case DisplayMode.DPS:
                case DisplayMode.DPE:
                    returnValue = statDisplayed switch
                    {
                        DisplayMode.DPA => ConfigData.EDamageReturn.DPA,
                        DisplayMode.DPE => ConfigData.EDamageReturn.Numeric,
                        _ => ConfigData.EDamageReturn.DPS
                    };
                    funcBase = (b, e) => b.FXGetDamageValue();
                    funcEnhanced = (b, e) => e.FXGetDamageValue();
                    funcFilterPower = (b, e) => (Math.Abs(b.FXGetDamageValue()) >= float.Epsilon) | (Math.Abs(e.FXGetDamageValue()) >= float.Epsilon);
                    unitSuffix = "";
                    break;

                case DisplayMode.EndUse:
                    funcBase = (b, e) => b.EndCost;
                    funcEnhanced = (b, e) => e.EndCost;
                    funcFilterPower = (b, e) => b.EndCost >= float.Epsilon;
                    unitSuffix = "/s";
                    break;

                case DisplayMode.EndPerSec:
                    funcBase = (b, e) =>
                    {
                        var nBase = b.PowerType switch
                        {
                            Enums.ePowerType.Click when e.RechargeTime + e.CastTime + e.InterruptTime > 0 => b.EndCost /
                                (b.RechargeTime + b.CastTime + b.InterruptTime),
                            Enums.ePowerType.Toggle => b.EndCost / b.ActivatePeriod,
                            _ => b.EndCost
                        };

                        return nBase;
                    };

                    funcEnhanced = (b, e) =>
                    {
                        var nEnh = b.PowerType switch
                        {
                            Enums.ePowerType.Click when e.RechargeTime + e.CastTime + e.InterruptTime > 0 => e.EndCost /
                                (e.RechargeTime + e.CastTime + e.InterruptTime),
                            Enums.ePowerType.Toggle => e.EndCost / e.ActivatePeriod,
                            _ => e.EndCost
                        };

                        return nEnh;
                    };

                    funcFilterPower = (b, e) => Math.Abs(b.EndCost) >= float.Epsilon;
                    unitSuffix = "/s";
                    break;

                case DisplayMode.Healing:
                    funcBase = (b, e) => b.GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                    funcEnhanced = (b, e) => e.GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                    funcFilterPower = (b, e) =>
                        (Math.Abs(b.GetEffectMagSum(Enums.eEffectType.Heal).Sum) >= float.Epsilon) |
                        (Math.Abs(e.GetEffectMagSum(Enums.eEffectType.Heal).Sum) >= float.Epsilon);
                    unitSuffix = "";
                    break;

                case DisplayMode.HPS:
                    funcBase = (b, e) =>
                    {
                        var nBase = b.GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                        switch (b.PowerType)
                        {
                            case Enums.ePowerType.Click when e.RechargeTime + e.CastTime + e.InterruptTime > 0:
                                nBase /= b.RechargeTime + b.CastTime + b.InterruptTime;
                                break;
                            case Enums.ePowerType.Toggle:
                                nBase /= b.ActivatePeriod;
                                break;
                        }

                        return nBase;
                    };

                    funcEnhanced = (b, e) =>
                    {
                        var nEnh = e.GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                        switch (b.PowerType)
                        {
                            case Enums.ePowerType.Click when e.RechargeTime + e.CastTime + e.InterruptTime > 0:
                                nEnh /= e.RechargeTime + e.CastTime + e.InterruptTime;
                                break;
                            case Enums.ePowerType.Toggle:
                                nEnh /= e.ActivatePeriod;
                                break;
                        }

                        return nEnh;
                    };

                    funcFilterPower = (b, e) =>
                        (Math.Abs(b.GetEffectMagSum(Enums.eEffectType.Heal).Sum) >= float.Epsilon) |
                        (Math.Abs(e.GetEffectMagSum(Enums.eEffectType.Heal).Sum) >= float.Epsilon);
                    unitSuffix = "";
                    break;

                case DisplayMode.HPE:
                    funcBase = (b, e) => b.GetEffectMagSum(Enums.eEffectType.Heal).Sum / (e.EndCost > 0 ? b.EndCost : 1);
                    funcEnhanced = (b, e) => e.GetEffectMagSum(Enums.eEffectType.Heal).Sum / (e.EndCost > 0 ? e.EndCost : 1);
                    funcFilterPower = (b, e) =>
                        (Math.Abs(b.GetEffectMagSum(Enums.eEffectType.Heal).Sum) >= float.Epsilon) |
                        (Math.Abs(e.GetEffectMagSum(Enums.eEffectType.Heal).Sum) >= float.Epsilon);
                    unitSuffix = "";
                    break;

                case DisplayMode.EffectDuration:
                    funcBase = (b, e) =>
                    {
                        var durationEffectId = b.GetDurationEffectID();

                        return b.Effects[durationEffectId].Duration;
                    };

                    funcEnhanced = (b, e) =>
                    {
                        var durationEffectId = b.GetDurationEffectID();

                        return e.Effects[durationEffectId].Duration;
                    };

                    funcFilterPower = (b, e) =>
                    {
                        var durationEffectIdBase = b?.GetDurationEffectID();
                        var durationBase = durationEffectIdBase is null or < 0
                            ? 0
                            : b?.Effects[durationEffectIdBase.Value].Duration;

                        var durationEffectIdEnh = e?.GetDurationEffectID();
                        var durationEnh = durationEffectIdEnh is null or < 0
                            ? 0
                            : e?.Effects[durationEffectIdEnh.Value].Duration;

                        return durationEffectIdBase is not (null or < 0) &&
                               durationEffectIdEnh is not (null or < 0) &&
                               b?.Effects[durationEffectIdBase.Value].EffectType is not (Enums.eEffectType.Null or Enums.eEffectType.NullBool or Enums.eEffectType.DesignerStatus) &
                            (Math.Max(durationBase ?? 5000, durationEnh ?? 5000) <= 4000); // Longer effects are considered permanent
                    };

                    unitSuffix = "s";
                    break;

                case DisplayMode.Range:
                    funcBase = (b, e) =>
                    {
                        var nBase = 0f;
                        if (b.Range > 0)
                        {
                            nBase = b.Range;
                        }
                        else if (b.Radius > 0)
                        {
                            nBase = b.Radius;
                        }

                        return nBase;
                    };

                    funcEnhanced = (b, e) =>
                    {
                        var nEnh = 0f;
                        if (b.Range > 0)
                        {
                            nEnh = e.Range;
                        }
                        else if (b.Radius > 0)
                        {
                            nEnh = e.Radius;
                        }

                        return nEnh;
                    };

                    funcFilterPower = (b, e) =>
                    {
                        var nBase = 0f;
                        if (b.Range > 0)
                        {
                            nBase = b.Range;
                        }
                        else if (b.Radius > 0)
                        {
                            nBase = b.Radius;
                        }

                        return Math.Abs(nBase) >= float.Epsilon;
                    };

                    unitSuffix = "ft";
                    break;

                case DisplayMode.RechargeTime:
                    funcBase = (b, e) => b.RechargeTime;
                    funcEnhanced = (b, e) => e.RechargeTime;
                    funcFilterPower = (b, e) => Math.Abs(b.RechargeTime) >= float.Epsilon;
                    unitSuffix = "s";
                    break;

                case DisplayMode.Regeneration:
                    funcBase = (b, e) => b?.GetEffectMagSum(Enums.eEffectType.Regeneration, false, b.Effects.Any(fx => fx is { EffectType: Enums.eEffectType.Regeneration, ToWho: Enums.eToWho.Target, BuffedMag: < 0 })).Sum ?? 0;
                    funcEnhanced = (b, e) => e?.GetEffectMagSum(Enums.eEffectType.Regeneration, false, e.Effects.Any(fx => fx is { EffectType: Enums.eEffectType.Regeneration, ToWho: Enums.eToWho.Target, BuffedMag: < 0 })).Sum ?? 0;
                    funcFilterPower = (b, e) =>
                        (Math.Abs(b?.GetEffectMagSum(Enums.eEffectType.Regeneration, false, b.Effects.Any(fx => fx is { EffectType: Enums.eEffectType.Regeneration, ToWho: Enums.eToWho.Target, BuffedMag: < 0 })).Sum ?? 0) >= float.Epsilon) |
                        (Math.Abs(e?.GetEffectMagSum(Enums.eEffectType.Regeneration, false, e.Effects.Any(fx => fx is { EffectType: Enums.eEffectType.Regeneration, ToWho: Enums.eToWho.Target, BuffedMag: < 0 })).Sum ?? 0) >= float.Epsilon);
                    unitSuffix = "%/s";
                    break;
            }

            if (statDisplayed is DisplayMode.DPA or DisplayMode.DPE or DisplayMode.DPS)
            {
                MidsContext.Config.DamageMath.ReturnValue = returnValue;
            }

            var pwStats = SetGraphValues(baseArray, enhArray, baseOverride, graphStyle, funcBase, funcEnhanced, funcFilterPower, statDisplayed, unitSuffix);

            if (statDisplayed is DisplayMode.DPA or DisplayMode.DPE or DisplayMode.DPS)
            {
                MidsContext.Config.DamageMath.ReturnValue = returnValuePrev;
            }

            return pwStats;
        }

        /// <summary>
        /// Fill graph with values according to viewed stat.
        /// </summary>
        /// <param name="getBaseValue">Lambda function of type f(basePower:IPower?, enhancedPower:IPower?): float to get base value of each element</param>
        /// <param name="getEnhValue">Lambda function of type f(basePower:IPower?, enhancedPower:IPower?): float to get enhanced value of each element</param>
        /// <param name="filterPower">Lambda function of type f(basePower:IPower?, enhancedPower:IPower?): bool to filter each element. If it returns false, element will be ignored.</param>
        /// <param name="statDisplayed">Stat displayed, one of display stat type (<see cref="DisplayMode"/>)</param>
        /// <param name="valueSuffix">Value suffix or unit to use in tooltips.</param>
        private static PowerValueInfoExt[]? SetGraphValues(IPower?[] baseArray, IPower?[] enhArray, bool baseOverride, Enums.GraphStyle graphStyle, Func<IPower?, IPower?, float>? getBaseValue, Func<IPower?, IPower?, float> getEnhValue,
            Func<IPower?, IPower?, bool> filterPower, DisplayMode statDisplayed, string valueSuffix = "%")
        {
            var ret = new List<PowerValueInfoExt>();
            var num1 = 1f;
            for (var index = 0; index < baseArray.Length; index++)
            {
                if (!filterPower(baseArray[index], enhArray[index]))
                {
                    continue;
                }

                var nBase = getBaseValue(baseArray[index], enhArray[index]);
                var nEnh = getEnhValue(baseArray[index], enhArray[index]);
                var displayName = baseArray[index].DisplayName;

                if (num1 < nEnh)
                {
                    num1 = nEnh;
                }

                if (num1 < nBase)
                {
                    num1 = nBase;
                }

                if (baseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                var pe = MidsContext.Character?.CurrentBuild.Powers
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(e => e is { Power: not null } && e.Power.FullName == baseArray[index]?.FullName);

                var tip = string.Empty;
                var stacks = (pe != null) & (baseArray[index]?.VariableEnabled == true) ? pe?.Power?.Stacks : null;
                var stacksTip = $"{(stacks == null ? "" : $" (at {stacks} stack{(stacks == 1 ? "" : "s")})")}";
                var baseHealValue = nBase / MidsContext.Archetype.Hitpoints * 100;
                var enhHealValue = nEnh / MidsContext.Archetype.Hitpoints * 100;

                switch (statDisplayed)
                {
                    case DisplayMode.Accuracy:
                        tip = graphStyle == Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nBase:##0.#}{valueSuffix}{stacksTip}"
                            : $"{displayName}: {nEnh:##0.#}{valueSuffix}{stacksTip}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.#}{valueSuffix})";
                        }

                        break;

                    case DisplayMode.Damage:
                        tip = $"{displayName}\r\n";
                        if (graphStyle == Enums.GraphStyle.baseOnly)
                        {
                            tip += $"{baseArray[index].FXGetDamageString()}";
                        }
                        else
                        {
                            tip += !baseOverride
                                ? $"{enhArray[index].FXGetDamageString()}"
                                : $"{baseArray[index].FXGetDamageString()}";
                        }

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##})";
                        }

                        if (baseArray[index].PowerType == Enums.ePowerType.Toggle)
                        {
                            tip += $"\r\n(Applied every {baseArray[index].ActivatePeriod}s)";
                        }

                        break;

                    case DisplayMode.DPA:
                        tip = baseArray[index].DisplayName;

                        if (graphStyle == Enums.GraphStyle.baseOnly)
                        {
                            tip += $"\r\n{baseArray[index].FXGetDamageString()}";
                        }
                        else
                        {
                            tip += !baseOverride
                                ? $"\r\n{enhArray[index].FXGetDamageString()}"
                                : $"\r\n{baseArray[index].FXGetDamageString()}";
                        }

                        tip += "/s";
                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##})";
                        }

                        break;

                    case DisplayMode.DPS:
                        tip = baseArray[index].DisplayName;

                        if (graphStyle == Enums.GraphStyle.baseOnly)
                        {
                            tip += $"\r\n{baseArray[index].FXGetDamageString()}";
                        }
                        else
                        {
                            tip += !baseOverride
                                ? $"\r\n{enhArray[index].FXGetDamageString()}"
                                : $"\r\n{baseArray[index].FXGetDamageString()}";
                        }

                        tip += "/s";
                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##})";
                        }

                        break;

                    case DisplayMode.DPE:
                        tip = baseArray[index].DisplayName;

                        if (graphStyle == Enums.GraphStyle.baseOnly)
                        {
                            tip += $"\r\n{baseArray[index].FXGetDamageString()}";
                        }
                        else
                        {
                            tip += !baseOverride
                                ? $"\r\n{enhArray[index].FXGetDamageString()}"
                                : $"\r\n{baseArray[index].FXGetDamageString()}";
                        }

                        if (graphStyle == Enums.GraphStyle.baseOnly)
                        {
                            tip += $"\r\nDamage per unit of End: {nBase:##0.##}";
                        }
                        else
                        {
                            tip += $"\r\nDamage per unit of End: {nEnh:##0.##}";
                            if (Math.Abs(nBase - nEnh) > float.Epsilon)
                            {
                                tip += $" ({nBase:##0.##})";
                            }
                        }

                        break;

                    case DisplayMode.EffectDuration:
                        var durationEffectId = baseArray[index].GetDurationEffectID();
                        if (durationEffectId > -1)
                        {
                            tip = enhArray[index].Effects[durationEffectId].EffectType != Enums.eEffectType.Mez
                                ? Enums.GetEffectName(enhArray[index].Effects[durationEffectId].EffectType)
                                : Enums.GetMezName((Enums.eMezShort)enhArray[index].Effects[durationEffectId].MezType);
                            if (enhArray[index].Effects[durationEffectId].Mag < 0)
                            {
                                tip = $"-{tip}";
                            }

                            tip = graphStyle == Enums.GraphStyle.baseOnly
                                ? $"{displayName} ({tip}): {nBase:##0.#}{stacksTip}"
                                : $"{displayName} ({tip}): {nEnh:##0.#}{stacksTip}";

                            if (Math.Abs(nBase - nEnh) > float.Epsilon)
                            {
                                tip += $" ({nBase:##0.#})";
                            }

                            tip += "s";
                        }

                        break;

                    case DisplayMode.EndUse:
                        tip = graphStyle != Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nEnh:##0.##}{stacksTip}"
                            : $"{displayName}: {nBase:##0.##}{stacksTip}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##})";
                        }

                        if (baseArray[index].PowerType == Enums.ePowerType.Toggle)
                        {
                            tip += "\r\n(Per Second)";
                        }

                        break;

                    case DisplayMode.EndPerSec:
                        tip = graphStyle != Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nEnh:##0.##}/s{stacksTip}"
                            : $"{displayName}: {nBase:##0.##}/s{stacksTip}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##}/s)";
                        }

                        break;

                    case DisplayMode.Healing:
                        tip = graphStyle == Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {baseHealValue:##0.#}%{stacksTip}"
                            : $"{displayName}\r\n Enhanced: {enhHealValue:##0.#}% ({nEnh:##0.#} HP){stacksTip}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $"\r\n Base: {baseHealValue:##0.#}% ({nBase:##0.#} HP)";
                        }

                        break;

                    case DisplayMode.HPE:
                        tip = graphStyle == Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nBase:##0.##}%{stacksTip}"
                            : $"{displayName}\r\n Enhanced Heal per unit of End: {enhHealValue:##0.##}% ({nEnh:##0.##} HP){stacksTip}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $"\r\n Base Heal per unit of End: {baseHealValue:##0.##}% ({nBase:##0.##} HP)";
                        }

                        break;

                    case DisplayMode.HPS:
                        tip = graphStyle == Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {baseHealValue:##0.##}%/s{stacksTip}"
                            : $"{displayName}\r\n Enhanced: {enhHealValue:##0.##}%/s ({nEnh:##0.##} HP/s){stacksTip}";

                        if (Math.Abs(nBase - (double)nEnh) > float.Epsilon)
                        {
                            tip += $"\r\n Base: {baseHealValue:##0.#}%/s ({nBase:##0.##} HP/s)";
                        }

                        break;

                    case DisplayMode.Range:
                        tip = graphStyle != Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nEnh:##0.#}{valueSuffix}{stacksTip}"
                            : $"{displayName}: {nBase:##0.#}{valueSuffix}{stacksTip}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.#}{valueSuffix})";
                        }

                        tip += "ft";

                        break;

                    case DisplayMode.RechargeTime:
                        tip = graphStyle != Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nEnh:##0.##}{valueSuffix}{stacksTip}"
                            : $"{displayName}: {nBase:##0.##}{valueSuffix}{stacksTip}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##}{valueSuffix})";
                        }

                        break;

                    case DisplayMode.Regeneration:
                        var maxHp = MidsContext.Character.DisplayStats.HealthHitpointsNumeric(false);
                        var baseRegen = maxHp / 12f * (0.05f + 0.05f * ((nBase - 100) / 100f));
                        var baseRegenPercent = baseRegen / maxHp * 100f;
                        var enhRegen = maxHp / 12f * (0.05f + 0.05f * ((nEnh - 100) / 100f));
                        var enhRegenPercent = enhRegen / maxHp * 100;
                        if (baseOverride)
                        {
                            (baseRegen, enhRegen) = (enhRegen, baseRegen);
                            (baseRegenPercent, enhRegenPercent) = (enhRegenPercent, baseRegenPercent);
                        }

                        if (graphStyle == Enums.GraphStyle.baseOnly)
                        {
                            tip = $"Health regenerated per second: {baseRegenPercent:##0.##}%\r\n Hit Points regenerated per second at level 50: {baseRegen:##0.#} HP/s{stacksTip}";
                        }
                        else if (Math.Abs(nBase - nEnh) < float.Epsilon)
                        {
                            tip = $"{displayName}: {nBase:##0.#}%\r\n Health regenerated per second: {baseRegenPercent:##0.##}%/s\r\n Hit Points regenerated per second at level 50: {baseRegen:##0.#} HP/s{stacksTip}";
                        }
                        else
                        {
                            tip = $"{displayName}: {nEnh:##0.#}% ({nBase:##0.#}%)\r\n Health regenerated per second: {enhRegenPercent:##0.##}%/s ({baseRegenPercent:##0.##})\r\n Hit Points regenerated per second at level 50: {enhRegen:##0.#} HP/s ({baseRegen:##0.##}/s){stacksTip}";
                        }

                        break;
                }

                tip = tip.Trim();

                if (baseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                    if (statDisplayed is DisplayMode.Healing or DisplayMode.HPE or DisplayMode.HPS)
                    {
                        tip = tip.Replace("Enhanced", "Active").Replace("Base", "Alternate");
                    }
                }

                ret.Add(new PowerValueInfoExt
                {
                    PowerName = baseArray[index].FullName,
                    Power = baseArray[index],
                    PowerTaken = pe != null,
                    BaseValue = nBase,
                    EnhValue = nEnh,
                    UncappedValue = null,
                    Stacks = (pe != null) & (baseArray[index]?.VariableEnabled == true) ? pe?.Power?.Stacks : null,
                    UnitSuffix = valueSuffix,
                    Tip = tip
                });
            }

            return ret.ToArray();
        }

        #endregion
    }
}

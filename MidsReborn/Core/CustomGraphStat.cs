using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mids_Reborn.Core
{
    public static class CustomGraphStat
    {
        public enum eCustomGraphStat
        {
            EnhAccuracy,
            EnhEndurance,
            EnhEnduranceDiscount,
            EnhSpeedFlying,
            EnhJumpHeight,
            EnhSpeedJumping,
            EnhMez,
            EnhPerceptionRadius,
            EnhSpeedRunning,
            EnhToHit,
            EnhAbsorb,

            Defense,
            Resistance,
            Regeneration,
            MaxHP,
            Absorb,
            EndRec,
            EndUse,
            MaxEnd,
            SpeedRunning,
            SpeedJumping,
            JumpHeight,
            SpeedFlying,
            StealthPvE,
            StealthPvP,
            PerceptionRadius,
            Recharge,
            ToHit,
            Accuracy,
            Damage,
            Range,
            EndRdx,
            Heal,
            Threat,
            StatusProtection,
            StatusResistance,
            DebuffResistance,
            Elusivity
        }

        public enum eCustomGraphMode
        {
            Single,
            Min,
            Max,
            Average
        }

        /// <summary>
        /// Usable damage vectors (all available), as Enums.eDamage
        /// </summary>
        private static readonly Enums.eDamage[] DamageVectors = Enum.GetValues<Enums.eDamage>();
        /// <summary>
        /// Usable damage vectors (all available), as string
        /// </summary>
        private static readonly string[] DamageVectorsNames = Enum.GetNames<Enums.eDamage>();
        
        /// <summary>
        /// Unused defense vectors
        /// </summary>
        private static readonly int[] ExcludedDefVectors = new[]
        {
            Enums.eDamage.None,
            DatabaseAPI.RealmUsesToxicDef()? Enums.eDamage.None : Enums.eDamage.Toxic,
            Enums.eDamage.Special,
            Enums.eDamage.Unique1,
            Enums.eDamage.Unique2,
            Enums.eDamage.Unique3
        }.Cast<int>().ToArray();
        
        /// <summary>
        /// Unused resistance vectors
        /// </summary>
        private static readonly int[] ExcludedResVectors = new[]
        {
            Enums.eDamage.None,
            Enums.eDamage.Melee,
            Enums.eDamage.Ranged,
            Enums.eDamage.AoE,
            Enums.eDamage.Special,
            Enums.eDamage.Unique1,
            Enums.eDamage.Unique2,
            Enums.eDamage.Unique3
        }.Cast<int>().ToArray();
        
        /// <summary>
        /// Unused elusivity vectors
        /// </summary>
        private static readonly int[] ExcludedElusivityVectors = new[]
        {
            Enums.eDamage.Special,
            Enums.eDamage.Unique1,
            Enums.eDamage.Unique2,
            Enums.eDamage.Unique3
        }.Cast<int>().ToArray();

        /// <summary>
        /// Used mez types
        /// </summary>
        private static readonly Enums.eMez[] MezList =
        [
            Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep, Enums.eMez.Immobilized,
            Enums.eMez.Knockback, Enums.eMez.Repel, Enums.eMez.Confused, Enums.eMez.Terrorized,
            Enums.eMez.Taunt, Enums.eMez.Placate, Enums.eMez.Teleport
        ];

        /// <summary>
        /// Used debuff types, for debuff resistances
        /// </summary>
        private static readonly Enums.eEffectType[] DebuffEffectsList =
        [
            Enums.eEffectType.Defense, Enums.eEffectType.Endurance, Enums.eEffectType.Recovery,
            Enums.eEffectType.PerceptionRadius, Enums.eEffectType.ToHit, Enums.eEffectType.RechargeTime,
            Enums.eEffectType.SpeedRunning, Enums.eEffectType.Regeneration
        ];

        /// <summary>
        /// Create a custom graphs from a set of settings
        /// </summary>
        /// <param name="stat">Statistics type to display</param>
        /// <param name="mode">Display mode (min, max, average, single vector), for multi vectors e.g. defense</param>
        /// <param name="ctlName">Optional specific control name, default is graphCustom + short name of stat</param>
        /// <returns>A CtlMultiGraph object that monitors a single value</returns>
        public static CtlMultiGraph GenerateGraph(eCustomGraphStat stat, eCustomGraphMode mode, string? ctlName = null)
        {
            var settings = Settings.Get(stat);
            var settingsExt = Settings.GraphSettingsExtended.FromGraphSettings(settings, stat, mode);

            return new CtlMultiGraph
            {
                BarsAlignment = CtlMultiGraph.BarAlignment.Left,
                Border = true,
                BorderColor = settings.Appearance.Border,
                Clickable = false,
                ColorAbsorbed = Color.Gainsboro,
                ColorBase = settings.Appearance.Base ?? Color.Black,
                ColorEnh = settings.Appearance.Enhanced ?? Color.Black,
                ColorFadeEnd = settings.Appearance.FadeEnd,
                ColorFadeStart = Color.Black,
                ColorHighlight = settings.Appearance.Highlight,
                ColorLines = Color.Black,
                ColorMarkerInner = Color.Black,
                ColorMarkerOuter = Color.Yellow,
                ColorOvercap = settings.Appearance.Overcap ?? Color.Black,
                DifferentiateColors = false,
                DrawRuler = false,
                Dual = true,
                ForcedMax = 0,
                ForeColor = Color.WhiteSmoke,
                Highlight = true,
                ItemFontSizeOverride = 0,
                ItemHeight = 13,
                Lines = true,
                MarkerValue = 0,
                Max = settings.Max,
                MaxItems = 1,
                Name = ctlName ?? $"graphCustom{settings.ValueNames.ShortName}",
                NegativeAbsorbedColor = Color.SlateGray,
                NegativeBaseColor = Color.Navy,
                NegativeEnhColor = Color.Olive,
                NegativeOvercapColor = Color.DarkMagenta,
                OuterBorder = true,
                Overcap = settings.Style is Settings.GraphStyle.EnhancedWithOvercap or Settings.GraphStyle.ThreeStatsStacked,
                PaddingX = 4,
                PaddingY = 6,
                RulerPos = CtlMultiGraph.RulerPosition.Top,
                ScaleHeight = 32,
                ScaleIndex = settingsExt.ScaleIndex,
                SecondaryLabelPosition = CtlMultiGraph.Alignment.Right,
                ShowScale = false,
                SingleLineLabels = true,
                Size = MidsContext.Config?.UseOldTotalsWindow == true ? new Size(300, 15) : new Size(526, 27),
                Style = settings.Style == Settings.GraphStyle.EnhOnly ? Enums.GraphStyle.enhOnly : Enums.GraphStyle.Stacked,
                Tag = settingsExt,
                TextWidth = MidsContext.Config?.UseOldTotalsWindow == true ? 125 : 187
            };
        }

        /// <summary>
        /// Set up displayed item, values, tooltip
        /// </summary>
        /// <param name="ctl">(Implicit - extension method) Target control</param>
        /// <param name="stat">Statistic type</param>
        /// <param name="mode">Display mode (min, max, average, single vector), for multi vectors e.g. defense</param>
        /// <param name="cfgSettings">Graph settings to apply</param>
        public static void SetGraphItem(this CtlMultiGraph ctl, eCustomGraphStat stat, eCustomGraphMode mode, ConfigData.CustomGraphSettings cfgSettings)
        {
            var displayStats = MidsContext.Character.DisplayStats;
            var atName = MidsContext.Character.Archetype.DisplayName;

            var longName = Names.CustomStatNameLong(stat);
            
            float val;
            string suffix;
            
            var hpValue = displayStats.HealthHitpointsNumeric(false);
            var hpValueUncapped = displayStats.HealthHitpointsNumeric(true);
            var hpBase = MidsContext.Character.Archetype.Hitpoints;
            
            var regenValue = displayStats.HealthRegenPercent(false);
            var regenValueUncapped = displayStats.HealthRegenPercent(true);
            
            var absorbValue = Math.Min(displayStats.Absorb, hpBase);

            var endRecValue = displayStats.EnduranceRecoveryNumeric;
            var endRecValueUncapped = displayStats.EnduranceRecoveryNumericUncapped;
            var endRecBase = MidsContext.Character.Archetype.BaseRecovery * displayStats.EnduranceMaxEnd / 60f;
            const float maxEndBase = 100;

            var movementUnitSpeed = clsConvertibleUnitValue.FormatSpeedUnit(MidsContext.Config.SpeedFormat);
            var movementUnitDistance = clsConvertibleUnitValue.FormatDistanceUnit(MidsContext.Config.SpeedFormat);

            var mezIndex = cfgSettings.MezType switch
            {
                Enums.eMez.Stunned => 1,
                Enums.eMez.Sleep => 2,
                Enums.eMez.Immobilized => 3,
                Enums.eMez.Knockback => 4,
                Enums.eMez.Repel => 5,
                Enums.eMez.Confused => 6,
                Enums.eMez.Terrorized => 7,
                Enums.eMez.Taunt => 8,
                Enums.eMez.Placate => 9,
                Enums.eMez.Teleport => 10,
                _ => 0
            };

            var debuffResIndex = cfgSettings.EffectType switch
            {
                Enums.eEffectType.Endurance => 1,
                Enums.eEffectType.Recovery => 2,
                Enums.eEffectType.PerceptionRadius => 3,
                Enums.eEffectType.ToHit => 4,
                Enums.eEffectType.RechargeTime => 5,
                Enums.eEffectType.SpeedRunning => 6,
                Enums.eEffectType.Regeneration => 7,
                _ => 0
            };

            string mezLabel;
            string mezLabelLong;
            string barLabel;
            string barLabelVector;

            ctl.SuspendLayout();
            ctl.Clear();

            switch (stat)
            {
                case eCustomGraphStat.EnhAccuracy:
                    val = displayStats.Boosts.GetValueOrDefault(Enums.eEffectType.Accuracy, 0);
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, longName)
                    );
                    break;

                case eCustomGraphStat.EnhEndurance:
                    val = displayStats.Boosts.GetValueOrDefault(Enums.eEffectType.Endurance, 0);
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, longName)
                    );
                    break;

                case eCustomGraphStat.EnhEnduranceDiscount:
                    val = displayStats.Boosts.GetValueOrDefault(Enums.eEffectType.EnduranceDiscount, 0);
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, longName)
                    );
                    break;

                case eCustomGraphStat.EnhSpeedFlying:
                    val = displayStats.Boosts.GetValueOrDefault(Enums.eEffectType.SpeedFlying, 0);
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, longName)
                    );
                    break;

                case eCustomGraphStat.EnhJumpHeight:
                    val = displayStats.Boosts.GetValueOrDefault(Enums.eEffectType.JumpHeight, 0);
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, longName)
                    );
                    break;

                case eCustomGraphStat.EnhSpeedJumping:
                    val = displayStats.Boosts.GetValueOrDefault(Enums.eEffectType.SpeedJumping, 0);
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, longName)
                    );
                    break;

                case eCustomGraphStat.EnhMez:
                    val = mode switch
                    {
                        eCustomGraphMode.Single => displayStats.BoostsMez.GetValueOrDefault(cfgSettings.MezType ?? Enums.eMez.Held, 0),
                        eCustomGraphMode.Min => displayStats.BoostsMez.Values.Min(),
                        eCustomGraphMode.Average => displayStats.BoostsMez.Values.Average(),
                        _ => displayStats.BoostsMez.Values.Max()
                    };

                    suffix = mode switch
                    {
                        eCustomGraphMode.Single => $"({cfgSettings.MezType} only)",
                        eCustomGraphMode.Min => "(min value)",
                        eCustomGraphMode.Average => "(average value)",
                        _ => "(max value)"
                    };
                    
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, $"{longName} {suffix}")
                    );
                    break;

                case eCustomGraphStat.EnhPerceptionRadius:
                    val = displayStats.Boosts.GetValueOrDefault(Enums.eEffectType.PerceptionRadius, 0);
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, longName)
                    );
                    break;

                case eCustomGraphStat.EnhSpeedRunning:
                    val = displayStats.Boosts.GetValueOrDefault(Enums.eEffectType.SpeedRunning, 0);
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, longName)
                    );
                    break;

                case eCustomGraphStat.EnhToHit:
                    val = displayStats.Boosts.GetValueOrDefault(Enums.eEffectType.ToHit, 0);
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, longName)
                    );
                    break;

                case eCustomGraphStat.EnhAbsorb:
                    val = displayStats.Boosts.GetValueOrDefault(Enums.eEffectType.Absorb, 0);
                    ctl.AddItemPair(longName,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, longName)
                    );
                    break;

                case eCustomGraphStat.Defense:
                    val = mode switch
                    {
                        eCustomGraphMode.Single => displayStats.Defense(cfgSettings.DamageType != null ? (int)cfgSettings.DamageType : 1),
                        eCustomGraphMode.Min => displayStats.DefenseMin,
                        eCustomGraphMode.Average => displayStats.DefenseAvg,
                        _ => displayStats.DefenseMax
                    };

                    barLabelVector = mode switch
                    {
                        eCustomGraphMode.Average => "Avg",
                        eCustomGraphMode.Min or eCustomGraphMode.Max => $"{mode}",
                        _ => $"{cfgSettings.DamageType}"
                    };

                    barLabel = $"{longName}{(!string.IsNullOrEmpty(barLabelVector) ? $" ({barLabelVector})" : "")}";

                    suffix = mode switch
                    {
                        eCustomGraphMode.Single => $"({cfgSettings.DamageType} only)",
                        eCustomGraphMode.Min => "(min value)",
                        eCustomGraphMode.Average => "(average value)",
                        _ => "(max value)"
                    };

                    ctl.AddItemPair(barLabel,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, $"{longName} {suffix}")
                    );
                    break;

                case eCustomGraphStat.Resistance:
                    val = mode switch
                    {
                        eCustomGraphMode.Single => displayStats.DamageResistance(cfgSettings.DamageType != null ? (int)cfgSettings.DamageType : 1, false),
                        eCustomGraphMode.Min => displayStats.DamageResistanceMin,
                        eCustomGraphMode.Average => displayStats.DamageResistanceAvg,
                        _ => displayStats.DamageResistanceMax
                    };

                    barLabelVector = mode switch
                    {
                        eCustomGraphMode.Average => "Avg",
                        eCustomGraphMode.Min or eCustomGraphMode.Max => $"{mode}",
                        _ => $"{cfgSettings.DamageType}"
                    };

                    barLabel = $"{longName}{(!string.IsNullOrEmpty(barLabelVector) ? $" ({barLabelVector})" : "")}";

                    suffix = mode switch
                    {
                        eCustomGraphMode.Single => $"({cfgSettings.DamageType} only)",
                        eCustomGraphMode.Min => "(min value)",
                        eCustomGraphMode.Average => "(average value)",
                        _ => "(max value)"
                    };

                    ctl.AddItemPair(barLabel,
                        $"{val:##0.##}%",
                        0,
                        val,
                        GenericDataTooltip3(val, 0, val, $"{longName} {suffix}")
                    );
                    break;

                case eCustomGraphStat.Regeneration:
                    const float regenBase = 100;
                    ctl.AddItemPair("Regeneration",
                        $"{regenValue:###0.##}%",
                        Math.Max(0, regenBase),
                        Math.Max(0, regenValue),
                        Math.Max(0, regenValueUncapped),
                        ((regenValueUncapped > regenValue) & (regenValue > 0)
                            ? $"{regenValueUncapped:##0.##}% Regeneration, capped at {regenValue:##0.##}%"
                            : $"{regenValue:##0.##}% Regeneration"
                        ) +
                        $" ({MidsContext.Character.DisplayStats.HealthRegenHPPerSec:##0.##} HP/s)" +
                        (regenBase > 0 ? $"\r\nBase: {regenBase:##0.##}%" : ""));
                    break;

                case eCustomGraphStat.MaxHP:
                    ctl.AddItemPair("Max HP", $"{hpValue:###0.##}",
                        Math.Max(0, hpBase),
                        Math.Max(0, hpValue),
                        Math.Max(0, hpValueUncapped),
                        0,
                        ((hpValueUncapped > hpValue) & (hpValue > 0)
                            ? $"{hpValueUncapped:##0.##} HP, capped at {MidsContext.Character.Archetype.HPCap} HP"
                            : $"{hpValue:##0.##} HP ({atName} HP cap: {MidsContext.Character.Archetype.HPCap} HP)"

                        ) +
                        $"\r\nBase: {hpBase:##0.##} HP");
                    break;

                case eCustomGraphStat.Absorb:
                    ctl.AddItemPair("Max HP", $"{hpValue:###0.##}",
                        0,
                        Math.Min(hpBase, Math.Max(0, absorbValue)),
                        Math.Max(0, absorbValue),
                        0,
                        $"\r\nAbsorb: {absorbValue:##0.##} ({absorbValue / hpBase * 100:##0.##}% of base HP)");
                    break;

                case eCustomGraphStat.EndRec:
                    ctl.AddItemPair("End Rec", $"{endRecValue:##0.##}/s",
                        Math.Max(0, endRecBase),
                        Math.Max(0, endRecValue),
                        Math.Max(0, endRecValueUncapped),
                        ((endRecValueUncapped > endRecValue) & (endRecValue > 0)
                            ? $"{endRecValueUncapped:##0.##}/s End. ({displayStats.EnduranceRecoveryPercentage(true):##0.##}%), capped at {MidsContext.Character.Archetype.RecoveryCap * 100:##0.##}%"
                            : $"{endRecValue:##0.##}/s End. ({displayStats.EnduranceRecoveryPercentage(false):##0.##}%) ({atName} End. recovery cap: {MidsContext.Character.Archetype.RecoveryCap * 100:##0.##}%)"
                        ) +
                        $"\r\nBase: {endRecBase:##0.##}/s");
                    break;

                case eCustomGraphStat.EndUse:
                    ctl.AddItemPair("End Use",
                        $"{displayStats.EnduranceUsage:##0.##}/s",
                        0,
                        displayStats.EnduranceUsage,
                        $"{displayStats.EnduranceUsage:##0.##}/s End. (Net gain: {displayStats.EnduranceRecoveryNet:##0.##}/s)");
                    break;

                case eCustomGraphStat.MaxEnd:
                    ctl.AddItemPair("Max End",
                        $"{displayStats.EnduranceMaxEnd:##0.##}",
                        maxEndBase,
                        displayStats.EnduranceMaxEnd,
                        $"{displayStats.EnduranceMaxEnd:##0.##} Maximum Endurance (base: {maxEndBase:##0.##})");
                    break;

                case eCustomGraphStat.SpeedRunning:
                    var runSpdBase = displayStats.Speed(Statistics.BaseRunSpeed, MidsContext.Config.SpeedFormat);
                    var runSpdValue = displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, false);
                    var runSpdUncapped = displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, true);
                    ctl.AddItemPair("Run Speed",
                        $"{runSpdValue:##0.##} {movementUnitSpeed}",
                        Math.Max(0, runSpdBase),
                        Math.Max(0, runSpdValue),
                        Math.Max(0, runSpdUncapped),
                        ((runSpdUncapped > runSpdValue) & (runSpdValue > 0)
                            ? $"{runSpdUncapped:##0.##} {movementUnitSpeed} Run Speed, capped at {runSpdValue:##0.##} {movementUnitSpeed}"
                            : $"{runSpdValue:##0.##} {movementUnitSpeed} Run Speed"
                        ) +
                        (runSpdBase > 0
                            ? $"\r\nBase: {runSpdBase:##0.##} {movementUnitSpeed}"
                            : "")
                    );
                    break;

                case eCustomGraphStat.SpeedJumping:
                    var jumpSpdBase = displayStats.Speed(Statistics.BaseJumpSpeed, Enums.eSpeedMeasure.FeetPerSecond);
                    var jumpSpdValue = displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, false);
                    var jumpSpdUncapped = displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, true);
                    ctl.AddItemPair("Jump Speed",
                        $"{jumpSpdValue:##0.##} {movementUnitSpeed}",
                        Math.Max(0, jumpSpdBase),
                        Math.Max(0, jumpSpdValue),
                        Math.Max(0, jumpSpdUncapped),
                        ((jumpSpdUncapped > jumpSpdValue) & (jumpSpdValue > 0)
                            ? $"{jumpSpdUncapped:##0.##} {movementUnitSpeed} Jump Speed, capped at {jumpSpdValue:##0.##} {movementUnitSpeed}"
                            : $"{jumpSpdValue:##0.##} {movementUnitSpeed} Jump Speed"
                        ) +
                        (jumpSpdBase > 0
                            ? $"\r\nBase: {jumpSpdBase:##0.##} {movementUnitSpeed}"
                            : "")
                    );
                    break;

                case eCustomGraphStat.JumpHeight:
                    var jumpHeightBase = displayStats.Distance(Statistics.BaseJumpHeight, MidsContext.Config.SpeedFormat);
                    var jumpHeightValue = displayStats.MovementJumpHeight(MidsContext.Config.SpeedFormat);
                    ctl.AddItemPair("Jump Height",
                        $"{jumpHeightValue:##0.##} {movementUnitDistance}",
                        Math.Max(0, jumpHeightBase),
                        Math.Max(0, jumpHeightValue),
                        $"{jumpHeightValue:##0.##} {movementUnitDistance} Jump Height" +
                        (jumpHeightBase > 0
                            ? $"\r\nBase: {jumpHeightBase:##0.##} {movementUnitDistance}"
                            : "")
                    );
                    break;

                case eCustomGraphStat.SpeedFlying:
                    var flySpeedValue = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, false);
                    var flySpeedBase = flySpeedValue == 0
                        ? 0
                        : displayStats.Speed(Statistics.BaseFlySpeed, MidsContext.Config.SpeedFormat);
                    var flySpeedUncapped = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, true);
                    ctl.AddItemPair("Fly Speed",
                        $"{flySpeedValue:##0.##} {movementUnitSpeed}",
                        Math.Max(0, flySpeedBase),
                        Math.Max(0, flySpeedValue),
                        Math.Max(0, flySpeedUncapped),
                        ((flySpeedUncapped > flySpeedValue) & (flySpeedValue > 0)
                            ? $"{flySpeedUncapped:##0.##} {movementUnitSpeed} Fly Speed, capped at {flySpeedValue:##0.##} {movementUnitSpeed}"
                            : $"{flySpeedValue:##0.##} {movementUnitSpeed} Fly Speed"
                        ) +
                        (flySpeedBase > 0
                            ? $"\r\nBase: {flySpeedBase:##0.##} {movementUnitSpeed}"
                            : ""));
                    break;

                case eCustomGraphStat.StealthPvE:
                    ctl.AddItemPair("Stealth (PvE)",
                        $"{displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat):###0.##} {movementUnitDistance}",
                        0,
                        displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat),
                        GenericDataTooltip3(displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat), 0, displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat), "Stealth (PvE)", "", movementUnitDistance));
                    break;

                case eCustomGraphStat.StealthPvP:
                    ctl.AddItemPair("Stealth (PvP)",
                        $"{displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat):###0.##} {movementUnitDistance}",
                        0,
                        displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat),
                        GenericDataTooltip3(displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat), 0, displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat), "Stealth (PvP)", "", movementUnitDistance));
                    break;

                case eCustomGraphStat.PerceptionRadius:
                    ctl.AddItemPair("Perception",
                        $"{displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat):###0.##} {movementUnitDistance}",
                        displayStats.Distance(Statistics.BasePerception, MidsContext.Config.SpeedFormat),
                        displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat),
                        displayStats.Distance(displayStats.Perception(true), MidsContext.Config.SpeedFormat),
                        GenericDataTooltip3(displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat), displayStats.Distance(Statistics.BasePerception, MidsContext.Config.SpeedFormat), displayStats.Distance(displayStats.Perception(true), MidsContext.Config.SpeedFormat), "Perception", "", movementUnitDistance)
                    );
                    break;
                
                case eCustomGraphStat.Recharge:
                    ctl.AddItemPair("Haste",
                        $"{displayStats.BuffHaste(false):##0.##}%",
                        100,
                        Math.Max(0, displayStats.BuffHaste(false)),
                        Math.Max(0, displayStats.BuffHaste(true)),
                        GenericDataTooltip3(displayStats.BuffHaste(false), 100, displayStats.BuffHaste(true), "Haste"));
                    break;

                case eCustomGraphStat.ToHit:
                    ctl.AddItemPair("ToHit",
                        $"{displayStats.BuffToHit:##0.##}%",
                        0,
                        Math.Max(0, displayStats.BuffToHit),
                        GenericDataTooltip3(displayStats.BuffToHit, 0, displayStats.BuffToHit, "ToHit", "%", "", true));
                    break;

                case eCustomGraphStat.Accuracy:
                    ctl.AddItemPair("Accuracy",
                        $"{displayStats.BuffAccuracy:##0.##}%",
                        0,
                        Math.Max(0, displayStats.BuffAccuracy),
                        GenericDataTooltip3(displayStats.BuffAccuracy, 0, displayStats.BuffAccuracy, "Accuracy", "%", "", true));
                    break;

                case eCustomGraphStat.Damage:
                    ctl.AddItemPair("Damage",
                        $"{displayStats.BuffDamage(false):##0.##}%",
                        100,
                        Math.Max(0, displayStats.BuffDamage(false)),
                        Math.Max(0, displayStats.BuffDamage(true)),
                        GenericDataTooltip3(displayStats.BuffDamage(false), 100, displayStats.BuffDamage(true), "Damage")
                    );
                    break;

                case eCustomGraphStat.Range:
                    ctl.AddItemPair("Range",
                        $"{displayStats.RangePercent:##0.##}%",
                        0,
                        Math.Max(0, displayStats.RangePercent),
                        GenericDataTooltip3(displayStats.RangePercent, 0, displayStats.RangePercent, "Range", "%", "", true)
                    );
                    break;

                case eCustomGraphStat.EndRdx:
                    ctl.AddItemPair("EndRdx",
                        $"{displayStats.BuffEndRdx:##0.##}%",
                        0,
                        displayStats.BuffEndRdx,
                        GenericDataTooltip3(displayStats.BuffEndRdx, 0, displayStats.BuffEndRdx, "EndRdx"));
                    break;

                case eCustomGraphStat.Heal:
                    ctl.AddItemPair("Heal",
                        $"{displayStats.BuffHeal:##0.##}%",
                        0,
                        Math.Max(0, displayStats.BuffHeal),
                        GenericDataTooltip3(displayStats.BuffHeal, 0, displayStats.BuffHeal, "Heal", "%", "", true));
                    break;

                case eCustomGraphStat.Threat:
                    ctl.AddItemPair("Threat",
                        $"{displayStats.ThreatLevel:##0.##}",
                        MidsContext.Character.Archetype.BaseThreat * 100,
                        displayStats.ThreatLevel,
                        GenericDataTooltip3(displayStats.ThreatLevel, MidsContext.Character.Archetype.BaseThreat * 100, displayStats.ThreatLevel, "Threat"));
                    break;
                case eCustomGraphStat.StatusProtection:
                    var mezProtections = MidsContext.Character.Totals.Mez
                        .Select(e => e > 0 ? 0 : Math.Abs(e))
                        .ToArray();

                    val = mode switch
                    {
                        eCustomGraphMode.Single => mezProtections[cfgSettings.MezType != null ? mezIndex : 0],
                        eCustomGraphMode.Min => mezProtections.Min(),
                        eCustomGraphMode.Average => mezProtections.Average(),
                        _ => mezProtections.Max()
                    };

                    mezLabel = mode switch
                    {
                        eCustomGraphMode.Single => cfgSettings.MezType != null ? $"{cfgSettings.MezType} Prot." : "Held Prot.",
                        eCustomGraphMode.Min => "Mez Prot. (Min)",
                        eCustomGraphMode.Average => "Mez Prot. (Avg)",
                        _  => "Mez Prot. (Max)"
                    };

                    mezLabelLong = mode switch
                    {
                        eCustomGraphMode.Single => cfgSettings.MezType != null ? $"Status Protection to {cfgSettings.MezType}" : "Status Protection to Held",
                        eCustomGraphMode.Min => "Status Protection (Min)",
                        eCustomGraphMode.Average => "Status Protection (Avg)",
                        _ => "Status Protection (Max)"
                    };

                    ctl.AddItemPair(mezLabel,
                        $"{val:####0.##}",
                        0,
                        val,
                        $"{val:####0.##} {mezLabelLong}");
                    break;

                case eCustomGraphStat.StatusResistance:
                    val = mode switch
                    {
                        eCustomGraphMode.Single => MidsContext.Character.Totals.MezRes[cfgSettings.MezType != null ? mezIndex : 0],
                        eCustomGraphMode.Min => MidsContext.Character.Totals.MezRes.Min(),
                        eCustomGraphMode.Average => MidsContext.Character.Totals.MezRes.Average(),
                        _ => MidsContext.Character.Totals.MezRes.Max()
                    };

                    mezLabel = mode switch
                    {
                        eCustomGraphMode.Single => cfgSettings.MezType != null ? $"{cfgSettings.MezType} Resist." : "Held Resist.",
                        eCustomGraphMode.Min => "Mez Resist. (Min)",
                        eCustomGraphMode.Average => "Mez Resist. (Avg)",
                        _ => "Mez Resist. (Max)"
                    };

                    mezLabelLong = mode switch
                    {
                        eCustomGraphMode.Single => cfgSettings.MezType != null ? $"Status Resistance to {cfgSettings.MezType}" : "Status Resistance to Held",
                        eCustomGraphMode.Min => "Status Resistance (Min)",
                        eCustomGraphMode.Average => "Status Resistance (Avg)",
                        _ => "Status Resistance (Max)"
                    };

                    ctl.AddItemPair(mezLabel,
                        $"{val:####0.##}",
                        0,
                        val,
                        $"{val:####0.##} {mezLabelLong}");
                    break;

                case eCustomGraphStat.DebuffResistance:
                    var cappedDebuffRes = DebuffEffectsList.Select(e => Math.Min(
                            e == Enums.eEffectType.Defense
                                ? Statistics.MaxDefenseDebuffRes
                                : Statistics.MaxGenericDebuffRes,
                            MidsContext.Character.Totals.DebuffRes[(int)e]))
                        .ToList();
                    var uncappedDebuffRes = DebuffEffectsList.Select(e => MidsContext.Character.Totals.DebuffRes[(int)e]).ToList();

                    val = mode switch
                    {
                        eCustomGraphMode.Single => cfgSettings.EffectType != null ? cappedDebuffRes[debuffResIndex] : cappedDebuffRes[0],
                        eCustomGraphMode.Min => cappedDebuffRes.Min(),
                        eCustomGraphMode.Average => cappedDebuffRes.Average(),
                        _ => cappedDebuffRes.Max()
                    };

                    var valUncapped = mode switch
                    {
                        eCustomGraphMode.Single => cfgSettings.EffectType != null ? uncappedDebuffRes[debuffResIndex] : uncappedDebuffRes[0],
                        eCustomGraphMode.Min => uncappedDebuffRes.Min(),
                        eCustomGraphMode.Average => uncappedDebuffRes.Average(),
                        _ => uncappedDebuffRes.Max()
                    };

                    var debuffResLabel = mode switch
                    {
                        eCustomGraphMode.Single => cfgSettings.EffectType != null ? $"{cfgSettings.EffectType} Debuff Res." : "Defense Debuff Res.",
                        eCustomGraphMode.Min => "Debuff Res. (Min)",
                        eCustomGraphMode.Average => "Debuff Res. (Avg)",
                        _ => "Debuff Res. (Max)"
                    };

                    var debuffResLabelLong = mode switch
                    {
                        eCustomGraphMode.Single => cfgSettings.EffectType != null ? $"{cfgSettings.EffectType} Debuff Resistance" : "Defense Debuff Resistance",
                        eCustomGraphMode.Min => "Debuff Resistance (Min)",
                        eCustomGraphMode.Average => "Debuff Resistance (Avg)",
                        _ => "Debuff Resistance (Max)"
                    };

                    ctl.AddItemPair(debuffResLabel,
                        $"{val:##0.##}%",
                        0,
                        Math.Max(0, val),
                        Math.Max(0, valUncapped),
                        GenericDataTooltip3(val, 0, valUncapped, debuffResLabelLong)
                    );
                    break;

                case eCustomGraphStat.Elusivity:
                    val = mode switch
                    {
                        eCustomGraphMode.Single => cfgSettings.DamageType != null ? MidsContext.Character.Totals.Elusivity[(int)cfgSettings.DamageType] : MidsContext.Character.Totals.Elusivity[0],
                        eCustomGraphMode.Min => MidsContext.Character.Totals.Elusivity.Min(),
                        eCustomGraphMode.Average => MidsContext.Character.Totals.Elusivity.Average(),
                        _ => MidsContext.Character.Totals.Elusivity.Max()
                    };

                    val = (val + (MidsContext.Config.Inc.DisablePvE ? 0.4f : 0)) * 100;

                    var elusivityLabel = mode switch
                    {
                        eCustomGraphMode.Single => cfgSettings.DamageType != null ? $"Elusivity({(cfgSettings.DamageType == Enums.eDamage.None ? "Untyped" : $"{cfgSettings.DamageType}")})" : "Elusivity(Untyped)",
                        eCustomGraphMode.Min => "Elusivity (Min)",
                        eCustomGraphMode.Average => "Elusivity (Avg)",
                        _ => "Elusivity (Max)"
                    };

                    ctl.AddItemPair(elusivityLabel,
                        $"{val:##0.##}%",
                        0,
                        Math.Max(0, val),
                        $"{val:##0.##}% {elusivityLabel}");
                    break;
            }

            ctl.ResumeLayout(true);
        }

        /// <summary>
        /// Get view mode associated with specific stat
        /// </summary>
        /// <param name="stat">Statistic name</param>
        /// <param name="settings">Associated config settings</param>
        /// <returns>Damage/Effect/Mez mode for selected statistic, from the provided settings</returns>
        public static eCustomGraphMode GetModeFromStat(eCustomGraphStat stat, ConfigData.CustomGraphSettings? settings)
        {
            if (settings == null)
            {
                return eCustomGraphMode.Single;
            }

            return stat switch
            {
                eCustomGraphStat.DebuffResistance => settings.EffectMode,
                eCustomGraphStat.Defense or eCustomGraphStat.Resistance or eCustomGraphStat.Elusivity => settings.DamageMode,
                _ => settings.MezMode
            };
        }

        /// <summary>
        /// Generate a generic tooltip text for base, main and uncapped values
        /// </summary>
        /// <param name="value">Actual value</param>
        /// <param name="valueBase">Base value</param>
        /// <param name="valueUncapped">Uncapped value</param>
        /// <param name="statName">Statistics name</param>
        /// <param name="percentageSign">Use percentage sign for values</param>
        /// <param name="movementUnit">Movement unit (speed or distance)</param>
        /// <param name="plusSignEnabled">Show + sign for positive values (e.g. for enhancements effects)</param>
        /// <returns>Tooltip string</returns>
        public static string GenericDataTooltip3(float value, float valueBase, float valueUncapped, string statName, string percentageSign = "%", string movementUnit = "", bool plusSignEnabled = false)
        {
            return (valueUncapped > value
                       ? $"{(plusSignEnabled && valueUncapped > 0 ? "+" : "")}{valueUncapped:##0.##}{percentageSign}{movementUnit} {statName}, capped at {(plusSignEnabled && value > 0 ? "+" : "")}{value:##0.##}{percentageSign}"
                       : $"{(plusSignEnabled && value > 0 ? "+" : "")}{value:##0.##}{percentageSign}{movementUnit} {statName}"
                   ) +
                   (valueBase > 0
                       ? $"\r\nBase: {(plusSignEnabled && valueBase > 0 ? "+" : "")}{valueBase:##0.##}{percentageSign}{movementUnit}"
                       : "") +
                   (statName is "Damage" or "Haste" || valueBase > 0
                       ? $"\r\n(Enh: {value - valueBase:##0.##}{percentageSign})"
                       : "");
        }

        public static class Names
        {
            /// <summary>
            /// Stat name to short text converter
            /// </summary>
            /// <param name="stat">Statistic name</param>
            /// <returns>Short name for specified statistic</returns>
            public static string CustomStatNameShort(eCustomGraphStat stat)
            {
                return stat switch
                {
                    eCustomGraphStat.EnhAccuracy => "Enh(Acc)",
                    eCustomGraphStat.EnhEndurance => "Enh(EndMod)",
                    eCustomGraphStat.EnhEnduranceDiscount => "Enh(EndRdx)",
                    eCustomGraphStat.EnhSpeedFlying => "Enh(SpdFly)",
                    eCustomGraphStat.EnhJumpHeight => "Enh(JmpHght)",
                    eCustomGraphStat.EnhSpeedJumping => "Enh(JmpSpd)",
                    eCustomGraphStat.EnhMez => "Enh(Mez)",
                    eCustomGraphStat.EnhPerceptionRadius => "Enh(Percpt)",
                    eCustomGraphStat.EnhSpeedRunning => "Enh(RunSpd)",
                    eCustomGraphStat.EnhToHit => "Enh(ToHit)",
                    eCustomGraphStat.EnhAbsorb => "Enh(Absrb)",
                    eCustomGraphStat.Defense => "Def",
                    eCustomGraphStat.Resistance => "Res",
                    eCustomGraphStat.Regeneration => "Rgn",
                    eCustomGraphStat.MaxHP => "MaxHP",
                    eCustomGraphStat.Absorb => "Absorb",
                    eCustomGraphStat.EndRec => "EndRec",
                    eCustomGraphStat.EndUse => "EndUse",
                    eCustomGraphStat.MaxEnd => "MaxEnd",
                    eCustomGraphStat.SpeedRunning => "RunSpd",
                    eCustomGraphStat.SpeedJumping => "JmpSpd",
                    eCustomGraphStat.JumpHeight => "JmpHght",
                    eCustomGraphStat.SpeedFlying => "SpdFly",
                    eCustomGraphStat.StealthPvE => "Stealth (PvE)",
                    eCustomGraphStat.StealthPvP => "Stealth (PvP)",
                    eCustomGraphStat.PerceptionRadius => "Percpt",
                    eCustomGraphStat.Recharge => "Haste",
                    eCustomGraphStat.ToHit => "ToHit",
                    eCustomGraphStat.Accuracy => "Acc",
                    eCustomGraphStat.Damage => "Dmg",
                    eCustomGraphStat.Range => "Range",
                    eCustomGraphStat.EndRdx => "EndRdx",
                    eCustomGraphStat.Heal => "Heal",
                    eCustomGraphStat.Threat => "Threat",
                    eCustomGraphStat.StatusProtection => "MezProt",
                    eCustomGraphStat.StatusResistance => "MezRes",
                    eCustomGraphStat.DebuffResistance => "DbfRes",
                    eCustomGraphStat.Elusivity => "Elsvt",
                    _ => ""
                };
            }

            /// <summary>
            /// Stat name to long text converter
            /// </summary>
            /// <param name="stat">Statistic name</param>
            /// <returns>Long name for specified statistic</returns>
            public static string CustomStatNameLong(eCustomGraphStat stat)
            {
                return stat switch
                {
                    eCustomGraphStat.EnhAccuracy => "Enhancement(Accuracy)",
                    eCustomGraphStat.EnhEndurance => "Enhancement(EndMod)",
                    eCustomGraphStat.EnhEnduranceDiscount => "Enhancement(EndRdx)",
                    eCustomGraphStat.EnhSpeedFlying => "Enhancement(Speed Flying)",
                    eCustomGraphStat.EnhJumpHeight => "Enhancement(Jump Height)",
                    eCustomGraphStat.EnhSpeedJumping => "Enhancement(Jump Speed)",
                    eCustomGraphStat.EnhMez => "Enhancement(Mez)",
                    eCustomGraphStat.EnhPerceptionRadius => "Enhancement(Perception)",
                    eCustomGraphStat.EnhSpeedRunning => "Enhancement(Speed Running)",
                    eCustomGraphStat.EnhToHit => "Enhancement(ToHit)",
                    eCustomGraphStat.EnhAbsorb => "Enhancement(Absorb)",
                    eCustomGraphStat.Defense => "Defense",
                    eCustomGraphStat.Resistance => "Resistance",
                    eCustomGraphStat.Regeneration => "Regeneration",
                    eCustomGraphStat.MaxHP => "MaxHP",
                    eCustomGraphStat.Absorb => "Absorb",
                    eCustomGraphStat.EndRec => "Endurance Recovery",
                    eCustomGraphStat.EndUse => "Endurance Use",
                    eCustomGraphStat.MaxEnd => "Max Endurance",
                    eCustomGraphStat.SpeedRunning => "Run Speed",
                    eCustomGraphStat.SpeedJumping => "Jump Speed",
                    eCustomGraphStat.JumpHeight => "Jump Height",
                    eCustomGraphStat.SpeedFlying => "Speed Flying",
                    eCustomGraphStat.StealthPvE => "Stealth (PvE)",
                    eCustomGraphStat.StealthPvP => "Stealth (PvP)",
                    eCustomGraphStat.PerceptionRadius => "Perception",
                    eCustomGraphStat.Recharge => "Haste",
                    eCustomGraphStat.ToHit => "ToHit",
                    eCustomGraphStat.Accuracy => "Accuracy",
                    eCustomGraphStat.Damage => "Damage",
                    eCustomGraphStat.Range => "Range",
                    eCustomGraphStat.EndRdx => "Endurance Discount",
                    eCustomGraphStat.Heal => "Heal",
                    eCustomGraphStat.Threat => "Threat",
                    eCustomGraphStat.StatusProtection => "Mez Protection",
                    eCustomGraphStat.StatusResistance => "Mez Resistance",
                    eCustomGraphStat.DebuffResistance => "Debuff Resistance",
                    eCustomGraphStat.Elusivity => "Elusivity",
                    _ => ""
                };
            }
        }

        public static class Colors
        {
            public struct GraphColors
            {
                public Color? Base;
                public Color? Enhanced;
                public Color? Overcap;
                public Color FadeEnd;
                public Color Border;
                public Color Highlight;
            }

            /// <summary>
            /// Get graph highlight (hover) back color.
            /// Static value for all, depending only on using old window style or new.
            /// </summary>
            /// <returns>Graph highlight color</returns>
            private static Color GetHighlightBackColor()
            {
                return MidsContext.Config?.UseOldTotalsWindow == true ? Color.Gray : Color.FromArgb(128, 128, 255);
            }

            /// <summary>
            /// Get color theme for graph, for specific stat
            /// </summary>
            /// <param name="stat">Statistic name</param>
            /// <returns>Colors template containing up to base, enhanced, overcap, fade end, highlight, border colors (nulls allowed). Defaults are greyscale.</returns>
            public static GraphColors GetTemplate(eCustomGraphStat stat)
            {
                var highlightColor = GetHighlightBackColor();

                return stat switch
                {
                    eCustomGraphStat.EnhAccuracy => new GraphColors
                    {
                        Enhanced = Color.FromArgb(247, 203, 0),
                        FadeEnd = Color.FromArgb(71, 58, 0),
                        Border = Color.FromArgb(202, 167, 0),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.EnhEndurance => new GraphColors
                    {
                        Enhanced = Color.FromArgb(26, 76, 221),
                        FadeEnd = Color.FromArgb(9, 24, 71),
                        Border = Color.FromArgb(60, 109, 255),
                        Highlight = highlightColor
                    },
                    
                    eCustomGraphStat.EnhEnduranceDiscount => new GraphColors
                    {
                        Enhanced = Color.FromArgb(65, 146, 255),
                        FadeEnd = Color.FromArgb(38, 86, 153),
                        Border = Color.FromArgb(103, 168, 255),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.EnhSpeedRunning or eCustomGraphStat.EnhSpeedJumping or eCustomGraphStat.EnhSpeedFlying or eCustomGraphStat.EnhJumpHeight => new GraphColors
                    {
                        Enhanced = Color.FromArgb(0, 185, 157),
                        FadeEnd = Color.FromArgb(0, 64, 54),
                        Border = Color.FromArgb(0, 204, 173),
                        Highlight = highlightColor
                    },
                    
                    eCustomGraphStat.EnhMez => new GraphColors
                    {
                        Enhanced = Color.FromArgb(95, 76, 217),
                        FadeEnd = Color.FromArgb(28, 22, 64),
                        Border = Color.FromArgb(155, 146, 217),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.EnhPerceptionRadius => new GraphColors
                    {
                        Enhanced = Color.FromArgb(93, 112, 136),
                        FadeEnd = Color.FromArgb(26, 37, 51),
                        Border = Color.FromArgb(93, 134, 183),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.EnhToHit => new GraphColors
                    {
                        Enhanced = Color.FromArgb(247, 225, 124),
                        FadeEnd = Color.FromArgb(71, 67, 50),
                        Border = Color.FromArgb(201, 185, 101),
                        Highlight = highlightColor
                    },
                    
                    eCustomGraphStat.EnhAbsorb => new GraphColors
                    {
                        Enhanced = Color.FromArgb(182, 182, 182),
                        FadeEnd = Color.FromArgb(51, 51, 51),
                        Border = Color.FromArgb(204, 204, 204),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.Defense => new GraphColors
                    {
                        Enhanced = Color.Magenta,
                        FadeEnd = Color.Purple,
                        Border = Color.BlueViolet,
                        Highlight = highlightColor
                    },
                    
                    eCustomGraphStat.Resistance => new GraphColors
                    {
                        Enhanced = Color.FromArgb(0, 192, 192),
                        Overcap = Color.FromArgb(255, 128, 128),
                        FadeEnd = Color.LightSeaGreen,
                        Border = Color.LightSeaGreen,
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.Regeneration => new GraphColors
                    {
                        Base = Color.FromArgb(51, 204, 51),
                        Enhanced = Color.FromArgb(64, 255, 64),
                        Overcap = Color.FromArgb(28, 111, 28),
                        FadeEnd = Color.FromArgb(150, 251, 150),
                        Border = Color.PaleGreen,
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.MaxHP => new GraphColors
                    {
                        Base = Color.FromArgb(31, 130, 31),
                        Enhanced = Color.FromArgb(44, 180, 44),
                        Overcap = Color.FromArgb(10, 38, 10),
                        FadeEnd = Color.FromArgb(150, 251, 150),
                        Border = Color.PaleGreen,
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.Absorb => new GraphColors
                    {
                        Enhanced = Color.Gainsboro,
                        Overcap = Color.Gray,
                        FadeEnd = Color.FromArgb(109, 213, 109),
                        Border = Color.PaleGreen,
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.EndRec => new GraphColors
                    {
                        Base = Color.FromArgb(24, 114, 204),
                        Enhanced = Color.FromArgb(30, 144, 255),
                        Overcap = Color.FromArgb(13, 63, 112),
                        FadeEnd = Color.FromArgb(65, 105, 224),
                        Border = Color.RoyalBlue,
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.EndUse => new GraphColors
                    {
                        Enhanced = Color.FromArgb(149, 203, 255),
                        FadeEnd = Color.FromArgb(65, 105, 224),
                        Border = Color.RoyalBlue,
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.MaxEnd => new GraphColors
                    {
                        Base = Color.FromArgb(47, 125, 204),
                        Enhanced = Color.FromArgb(59, 158, 255),
                        FadeEnd = Color.FromArgb(65, 105, 224),
                        Border = Color.RoyalBlue,
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.SpeedRunning or eCustomGraphStat.SpeedJumping or eCustomGraphStat.JumpHeight or eCustomGraphStat.SpeedFlying => new GraphColors
                    {
                        Base = Color.FromArgb(0, 140, 94),
                        Enhanced = Color.FromArgb(0, 192, 128),
                        Overcap = Color.FromArgb(0, 48, 32),
                        FadeEnd = Color.FromArgb(0, 127, 95),
                        Border = Color.FromArgb(0, 114, 85),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.StealthPvE or eCustomGraphStat.StealthPvP or eCustomGraphStat.PerceptionRadius => new GraphColors
                    {
                        Base = Color.FromArgb(84, 95, 107),
                        Enhanced = Color.FromArgb(106, 121, 136),
                        Overcap = Color.FromArgb(46, 52, 59),
                        FadeEnd = Color.FromArgb(72, 61, 137),
                        Border = Color.FromArgb(62, 52, 119),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.Recharge => new GraphColors
                    {
                        Base = Color.FromArgb(204, 102, 0),
                        Enhanced = Color.FromArgb(255, 128, 0),
                        Overcap = Color.FromArgb(112, 56, 0),
                        FadeEnd = Color.FromArgb(184, 62, 0),
                        Border = Color.FromArgb(121, 40, 0),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.ToHit => new GraphColors
                    {
                        Enhanced = Color.FromArgb(255, 255, 128),
                        FadeEnd = Color.FromArgb(190, 190, 0),
                        Border = Color.FromArgb(121, 121, 0),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.Accuracy => new GraphColors
                    {
                        Enhanced = Color.Yellow,
                        FadeEnd = Color.FromArgb(123, 123, 0),
                        Border = Color.FromArgb(121, 121, 0),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.Damage => new GraphColors
                    {
                        Base = Color.FromArgb(204, 0, 0),
                        Enhanced = Color.Red,
                        Overcap = Color.FromArgb(112, 0, 0),
                        FadeEnd = Color.FromArgb(120, 61, 61),
                        Border = Color.FromArgb(114, 56, 56),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.Range => new GraphColors
                    {
                        Enhanced = Color.FromArgb(206, 196, 132),
                        FadeEnd = Color.FromArgb(112, 107, 72),
                        Border = Color.FromArgb(103, 98, 66),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.EndRdx => new GraphColors
                    {
                        Enhanced = Color.RoyalBlue,
                        FadeEnd = Color.FromArgb(105, 89, 203),
                        Border = Color.FromArgb(66, 56, 128),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.Heal => new GraphColors
                    {
                        Enhanced = Color.FromArgb(12, 200, 87),
                        FadeEnd = Color.FromArgb(8, 138, 60),
                        Border = Color.FromArgb(8, 138, 60),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.Threat => new GraphColors
                    {
                        Base = Color.FromArgb(113, 86, 168),
                        Enhanced = Color.MediumPurple,
                        FadeEnd = Color.FromArgb(72, 61, 137),
                        Border = Color.FromArgb(131, 112, 255),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.StatusProtection => new GraphColors
                    {
                        Enhanced = Color.FromArgb(255, 128, 0),
                        FadeEnd = Color.FromArgb(255, 128, 0),
                        Border = Color.FromArgb(114, 56, 0),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.StatusResistance => new GraphColors
                    {
                        Enhanced = Color.Yellow,
                        FadeEnd = Color.FromArgb(127, 127, 0),
                        Border = Color.FromArgb(114, 114, 0),
                        Highlight = highlightColor
                    },

                    eCustomGraphStat.DebuffResistance => new GraphColors
                    {
                        Base = Color.FromArgb(0, 190, 190),
                        Enhanced = Color.Cyan,
                        Overcap = Color.FromArgb(0, 90, 127),
                        FadeEnd = Color.FromArgb(0, 127, 127),
                        Border = Color.LightSeaGreen,
                        Highlight = highlightColor
                    },

                    _ => new GraphColors
                    {
                        Base = Color.FromArgb(140, 140, 140),
                        Enhanced = Color.FromArgb(170, 170, 170),
                        Overcap = Color.FromArgb(60, 60, 60),
                        FadeEnd = Color.FromArgb(40, 40, 40),
                        Border = Color.FromArgb(204, 204, 204),
                        Highlight = highlightColor
                    }
                };
            }
        }

        public static class Settings
        {
            private static readonly int[] Scales =
            [
                1,
                2,
                3,
                5,
                10,
                25,
                50,
                75,
                100,
                150,
                225,
                300,
                450,
                600,
                900,
                1200,
                2400,
                3000,
                3600,
                4000
            ];

            public enum GraphStyle
            {
                EnhOnly, // Enh
                BaseVsEnhanced, // Base + Enh
                EnhancedWithOvercap, // Enh + Overcap
                ThreeStatsStacked // Base + Enh + Overcap
            }

            public enum StatUnitType
            {
                Raw,
                Percentage,
                Distance,
                Speed,
                Custom
            }

            public struct StatNames
            {
                public string ShortName;
                public string LongName;
            }

            public struct GraphSettings
            {
                public StatNames ValueNames;
                public GraphStyle Style;
                public Colors.GraphColors Appearance;
                public StatUnitType UnitType;
                public string? UnitSuffix;
                public float Max;

                /// <summary>
                /// Convert a <see cref="GraphSettingsExtended">GraphSettingsExtended struct</see> into a GraphSettings one
                /// </summary>
                /// <param name="s">A GraphSettingsExtended struct object</param>
                public static GraphSettings FromGraphSettingsExtended(GraphSettingsExtended s)
                {
                    return new GraphSettings
                    {
                        ValueNames = new StatNames
                        {
                            LongName = s.ValueNames.LongName,
                            ShortName = s.ValueNames.ShortName
                        },
                        Style = s.Style,
                        Appearance = new Colors.GraphColors
                        {
                            Base = s.Appearance.Base,
                            Border = s.Appearance.Border,
                            Enhanced = s.Appearance.Enhanced,
                            FadeEnd = s.Appearance.FadeEnd,
                            Highlight = s.Appearance.Highlight,
                            Overcap = s.Appearance.Overcap
                        },
                        Max = s.Max,
                        UnitSuffix = s.UnitSuffix,
                        UnitType = s.UnitType,
                    };
                }
            }

            public struct GraphSettingsExtended
            {
                public StatNames ValueNames;
                public GraphStyle Style;
                public Colors.GraphColors Appearance;
                public StatUnitType UnitType;
                public string? UnitSuffix;
                public float Max;
                public int ScaleIndex;
                public eCustomGraphStat Stat;
                public eCustomGraphMode Mode;

                /// <summary>
                /// Converts and expand a <see cref="GraphSettings">GraphSettings struct</see> into a GraphSettingsExpanded one, adding stat name and stat mode
                /// </summary>
                /// <param name="s">A GraphSettings struct object</param>
                /// <param name="stat">Statistic name</param>
                /// <param name="mode">Statistic display mode</param>
                public static GraphSettingsExtended FromGraphSettings(GraphSettings s, eCustomGraphStat stat, eCustomGraphMode mode)
                {
                    return new GraphSettingsExtended
                    {
                        ValueNames = new StatNames
                        {
                            LongName = s.ValueNames.LongName,
                            ShortName = s.ValueNames.ShortName
                        },
                        Style = s.Style,
                        Appearance = new Colors.GraphColors
                        {
                            Base = s.Appearance.Base,
                            Border = s.Appearance.Border,
                            Enhanced = s.Appearance.Enhanced,
                            FadeEnd = s.Appearance.FadeEnd,
                            Highlight = s.Appearance.Highlight,
                            Overcap = s.Appearance.Overcap
                        },
                        Max = s.Max,
                        Mode = mode,
                        ScaleIndex = GetScaleIndex(s.Max),
                        Stat = stat,
                        UnitSuffix = s.UnitSuffix,
                        UnitType = s.UnitType
                    };
                }
            }

            /// <summary>
            /// Get graph scale from its maximum
            /// </summary>
            /// <param name="graphMax">Graph maximum value</param>
            /// <returns>Scale value (see <see cref="Scales">Scales</see>)</returns>
            internal static int GetScaleIndex(float graphMax)
            {
                for (var i = 0; i < Scales.Length; i++)
                {
                    if (graphMax <= Scales[i])
                    {
                        return i;
                    }
                }

                return Scales.Length - 1;
            }

            /// <summary>
            /// Get graph settings from specified statistic
            /// </summary>
            /// <param name="stat">Statistic name</param>
            /// <returns>A GraphSettings struct for matching statistic</returns>
            public static GraphSettings Get(eCustomGraphStat stat)
            {
                var statNames = new StatNames
                {
                    ShortName = Names.CustomStatNameShort(stat),
                    LongName = Names.CustomStatNameLong(stat)
                };

                var colors = Colors.GetTemplate(stat);

                return stat switch
                {
                    eCustomGraphStat.EnhEndurance => new GraphSettings { ValueNames = statNames, Style = GraphStyle.EnhOnly, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 250 },
                    eCustomGraphStat.EnhSpeedRunning or eCustomGraphStat.EnhSpeedJumping or eCustomGraphStat.EnhSpeedFlying or eCustomGraphStat.EnhJumpHeight => new GraphSettings { ValueNames = statNames, Style = GraphStyle.EnhOnly, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 300 },
                    
                    eCustomGraphStat.Defense => new GraphSettings { ValueNames = statNames, Style = GraphStyle.EnhOnly, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 100 },
                    eCustomGraphStat.Resistance => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 100 },
                    eCustomGraphStat.Regeneration => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Custom, UnitSuffix = "%HP/s", Max = 100 },
                    eCustomGraphStat.MaxHP => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Custom, UnitSuffix = "HP", Max = 4000 },
                    eCustomGraphStat.Absorb => new GraphSettings { ValueNames = statNames, Style = GraphStyle.EnhancedWithOvercap, Appearance = colors, UnitType = StatUnitType.Custom, UnitSuffix = "HP", Max = 100 },
                    eCustomGraphStat.EndRec => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Custom, UnitSuffix = "end/s", Max = 10 },
                    eCustomGraphStat.EndUse => new GraphSettings { ValueNames = statNames, Style = GraphStyle.EnhOnly, Appearance = colors, UnitType = StatUnitType.Custom, UnitSuffix = "end/s", Max = 10 },
                    eCustomGraphStat.MaxEnd => new GraphSettings { ValueNames = statNames, Style = GraphStyle.BaseVsEnhanced, Appearance = colors, UnitType = StatUnitType.Raw, Max = 200 },
                    eCustomGraphStat.SpeedRunning or eCustomGraphStat.SpeedJumping or eCustomGraphStat.SpeedFlying => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Speed, Max = 225 },
                    eCustomGraphStat.JumpHeight => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Distance, Max = 225 },
                    eCustomGraphStat.StealthPvE or eCustomGraphStat.StealthPvP => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Distance, Max = 1200 },
                    eCustomGraphStat.PerceptionRadius => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Distance, Max = 1200 },
                    eCustomGraphStat.Recharge => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 400 },
                    eCustomGraphStat.Damage => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 900 },
                    eCustomGraphStat.Range => new GraphSettings { ValueNames = statNames, Style = GraphStyle.EnhOnly, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 300 },
                    eCustomGraphStat.Heal => new GraphSettings { ValueNames = statNames, Style = GraphStyle.EnhOnly, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 300 },
                    eCustomGraphStat.Threat => new GraphSettings { ValueNames = statNames, Style = GraphStyle.BaseVsEnhanced, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 600 },
                    eCustomGraphStat.StatusProtection => new GraphSettings { ValueNames = statNames, Style = GraphStyle.EnhOnly, Appearance = colors, UnitType = StatUnitType.Raw, Max = 50 },
                    eCustomGraphStat.StatusResistance => new GraphSettings { ValueNames = statNames, Style = GraphStyle.EnhOnly, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 600 },
                    eCustomGraphStat.DebuffResistance => new GraphSettings { ValueNames = statNames, Style = GraphStyle.ThreeStatsStacked, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 100 },

                    _ => new GraphSettings { ValueNames = statNames, Style = GraphStyle.EnhOnly, Appearance = colors, UnitType = StatUnitType.Percentage, Max = 100 }
                };
            }
        }
    }
}

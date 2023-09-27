using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Controls.Extensions;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Controls
{
    public partial class ctlCombatTimeline : UserControl
    {
        #region Custom events

        public delegate void CalcEnhancedProgressEventHandler(object sender, float value);
        public event CalcEnhancedProgressEventHandler? CalcEnhancedProgress;

        #endregion

        #region Enums

        private enum ValueSign
        {
            Zero,
            Negative,
            Positive
        }

        private enum BoostType
        {
            Enhancement,
            Power
        }

        public enum ViewProfileType
        {
            Damage,
            Healing,
            Survival
        }

        #endregion

        private struct RechBoost
        {
            public int TimelineIndex;
            public BoostType BoostType;
            public float Duration;
        }

        #region FxIdentifier sub-class

        // Warning: struct/class identifier name used elsewhere
        private class FxIdentifier
        {
            public Enums.eEffectType? EffectType;
            public Enums.eMez? MezType;
            public Enums.eEffectType? ETModifies;
            public Enums.eToWho ToWho;
            public ValueSign? ValueSign;

            public FxIdentifier(Enums.eEffectType? effectType, Enums.eToWho toWho, ValueSign? valueSign)
            {
                EffectType = effectType;
                MezType = null;
                ETModifies = null;
                ToWho = toWho;
                ValueSign = valueSign;
            }

            public FxIdentifier(Enums.eMez? mezType, Enums.eToWho toWho, ValueSign? valueSign)
            {
                EffectType = Enums.eEffectType.Mez;
                MezType = mezType;
                ETModifies = null;
                ToWho = toWho;
                ValueSign = valueSign;
            }

            public FxIdentifier(Enums.eEffectType? effectType, Enums.eEffectType? etModifies, Enums.eToWho toWho, ValueSign? valueSign)
            {
                EffectType = effectType;
                MezType = null;
                ETModifies = etModifies;
                ToWho = toWho;
                ValueSign = valueSign;
            }

            public FxIdentifier(Enums.eEffectType? effectType, Enums.eEffectType? etModifies, Enums.eMez? mezType, Enums.eToWho toWho, ValueSign? valueSign)
            {
                EffectType = effectType;
                MezType = mezType;
                ETModifies = etModifies;
                ToWho = toWho;
                ValueSign = valueSign;
            }

            public override string ToString()
            {
                return $"<ctlCombatTimeline.FxIdentifier> {{EffectType={EffectType}, MezType={MezType}, ETModifies={ETModifies}, ToWho={ToWho}, ValueSign={ValueSign}}}";
            }
        }

        #endregion

        #region ViewProfiles sub-class

        private class ViewProfiles
        {
            // Buff/Debuff profile ?
            public readonly List<FxIdentifier> Damage = new()
            {
                new FxIdentifier(Enums.eEffectType.Damage, Enums.eToWho.Target, null), // ???
                new FxIdentifier(Enums.eEffectType.Mez, Enums.eToWho.Target, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Accuracy, Enums.eToWho.Target, ValueSign.Negative),
                new FxIdentifier(Enums.eEffectType.ToHit, Enums.eToWho.Target, ValueSign.Negative),
                new FxIdentifier(Enums.eEffectType.Regeneration, Enums.eToWho.Target, ValueSign.Negative),
                new FxIdentifier(Enums.eEffectType.Recovery, Enums.eToWho.Target, ValueSign.Negative),
                new FxIdentifier(Enums.eEffectType.PerceptionRadius, Enums.eToWho.Target, ValueSign.Negative),
                new FxIdentifier(Enums.eEffectType.Enhancement, Enums.eToWho.Target, ValueSign.Negative),
                new FxIdentifier(Enums.eEffectType.ToHit, Enums.eToWho.Self, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.DamageBuff, Enums.eToWho.Self, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Mez, Enums.eToWho.Self, ValueSign.Negative),
                new FxIdentifier(Enums.eEffectType.Enhancement, Enums.eToWho.Self, ValueSign.Positive)
            };

            public readonly List<FxIdentifier> Healing = new()
            {
                new FxIdentifier(Enums.eEffectType.Heal, Enums.eToWho.Target, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.HitPoints, Enums.eToWho.Target, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Regeneration, Enums.eToWho.Target, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Recovery, Enums.eToWho.Target, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Mez, Enums.eToWho.Target, ValueSign.Negative),
                new FxIdentifier(Enums.eEffectType.MezResist, Enums.eToWho.Target, ValueSign.Negative),
                new FxIdentifier(Enums.eEffectType.Defense, Enums.eToWho.Target, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Resistance, Enums.eToWho.Target, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.ToHit, Enums.eToWho.Target, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Accuracy, Enums.eToWho.Target, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.DamageBuff, Enums.eToWho.Target, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Enhancement, Enums.eToWho.Target, ValueSign.Positive)
            };

            public readonly List<FxIdentifier> Survival = new()
            {
                new FxIdentifier(Enums.eEffectType.Heal, Enums.eToWho.Self, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Regeneration, Enums.eToWho.Self, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.HitPoints, Enums.eToWho.Self, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Mez, Enums.eToWho.Self, ValueSign.Negative),
                new FxIdentifier(Enums.eEffectType.MezResist, Enums.eToWho.Self, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Defense, Enums.eToWho.Self, ValueSign.Positive),
                new FxIdentifier(Enums.eEffectType.Resistance, Enums.eToWho.Self, ValueSign.Positive)
            };
        }

        #endregion

        #region BuildPowerSlot sub-class

        public class BuildPowerSlot
        {
            public IPower? BasePower;
            public IPower? EnhancedPower;
            public int HistoryIdx;

            public BuildPowerSlot(IPower? basePower = null, int historyIdx = -1)
            {
                BasePower = basePower;
                HistoryIdx = historyIdx;
                EnhancedPower = null;
            }

            /// <summary>
            /// Set enhanced power after it is calculated.
            /// </summary>
            /// <param name="enhancedPower">Enhanced power</param>
            public void SetEnhancedPower(IPower? enhancedPower = null)
            {
                EnhancedPower = enhancedPower;
            }

            public override string ToString()
            {
                return $"<BuildPowerSlot> {{BasePower={(BasePower == null ? "<null>" : BasePower.FullName)}, EnhancedPower={(EnhancedPower == null ? "<null>" : EnhancedPower.FullName)}, HistoryIdx={HistoryIdx}}}";
            }
        }

        #endregion

        #region TimelineItem sub-class

        public class TimelineItem
        {
            public BuildPowerSlot PowerSlot;
            public float Time;

            public TimelineItem(BuildPowerSlot powerSlot, float time)
            {
                PowerSlot = powerSlot;
                Time = time;
            }

            public override string ToString()
            {
                return $"<TimelineItem> {{PowerSlot={PowerSlot}, Time={Time}}}";
            }
        }

        #endregion

        #region ColorTheme sub-class

        public class ColorTheme
        {
            public struct EffectTypeShort
            {
                public Enums.eEffectType EffectType;
                public Enums.eEffectType? ETModifies;
            }

            public readonly Color BackgroundColor = Color.FromArgb(11, 22, 29);
            public readonly Color TextColor = Color.WhiteSmoke;
            public readonly Color ShadowColor = Color.Black;

            private readonly Color Red = Color.FromArgb(255, 58, 131);
            private readonly Color LightPink = Color.FromArgb(235, 147, 154);
            private readonly Color Orange = Color.FromArgb(255, 170, 0);
            private readonly Color Yellow = Color.FromArgb(246, 240, 128);
            private readonly Color Yellow2 = Color.FromArgb(237, 223, 28);
            private readonly Color Green = Color.FromArgb(85, 228, 57);
            private readonly Color Green2 = Color.FromArgb(72, 193, 48);
            private readonly Color Green3 = Color.FromArgb(57, 153, 38);
            private readonly Color Green4 = Color.FromArgb(139, 226, 122);
            private readonly Color Blue = Color.FromArgb(30, 154, 224);
            private readonly Color Blue2 = Color.FromArgb(23, 115, 165);
            private readonly Color Blue3 = Color.FromArgb(17, 89, 128);
            private readonly Color Indigo = Color.FromArgb(63, 72, 204);
            private readonly Color Indigo2 = Color.FromArgb(71, 81, 229);
            private readonly Color Indigo3 = Color.FromArgb(79, 90, 255);
            private readonly Color Indigo4 = Color.FromArgb(104, 114, 255);
            private readonly Color Indigo5 = Color.FromArgb(55, 63, 178);
            private readonly Color Purple = Color.FromArgb(115, 43, 245);
            private readonly Color Purple2 = Color.FromArgb(86, 33, 186);
            private readonly Color LightPurple = Color.FromArgb(171, 102, 255);
            private readonly Color LightPurple2 = Color.FromArgb(203, 160, 255);
            private readonly Color White = Color.FromArgb(248, 248, 248);
            private readonly Color Gray = Color.FromArgb(168, 186, 194);
            private readonly Color Gray2 = Color.FromArgb(146, 161, 168);
            private readonly Color Gray3 = Color.FromArgb(124, 137, 142);
            private readonly Color Gray4 = Color.FromArgb(102, 112, 117);
            private readonly Color Gray5 = Color.FromArgb(190, 210, 219);
            private readonly Color Gray6 = Color.FromArgb(212, 235, 244);
            private readonly Color Gray7 = Color.FromArgb(232, 248, 255);
            private readonly Color Gray8 = Color.FromArgb(244, 251, 255);

            public Dictionary<EffectTypeShort, List<Color>> BuildColorDictionary()
            {
                var effectsList = new List<EffectTypeShort>
                {
                    new() {EffectType = Enums.eEffectType.Accuracy},
                    new() {EffectType = Enums.eEffectType.Damage},
                    new() {EffectType = Enums.eEffectType.DamageBuff},
                    new() {EffectType = Enums.eEffectType.Defense},
                    new() {EffectType = Enums.eEffectType.Endurance},
                    new() {EffectType = Enums.eEffectType.SpeedFlying},
                    new() {EffectType = Enums.eEffectType.Heal},
                    new() {EffectType = Enums.eEffectType.HitPoints},
                    new() {EffectType = Enums.eEffectType.JumpHeight},
                    new() {EffectType = Enums.eEffectType.SpeedJumping},
                    new() {EffectType = Enums.eEffectType.Mez},
                    new() {EffectType = Enums.eEffectType.MezResist},
                    new() {EffectType = Enums.eEffectType.PerceptionRadius},
                    new() {EffectType = Enums.eEffectType.Recovery},
                    new() {EffectType = Enums.eEffectType.Regeneration},
                    new() {EffectType = Enums.eEffectType.ResEffect},
                    new() {EffectType = Enums.eEffectType.Resistance},
                    new() {EffectType = Enums.eEffectType.SpeedRunning},
                    new() {EffectType = Enums.eEffectType.ToHit},
                    new() {EffectType = Enums.eEffectType.Enhancement, ETModifies = Enums.eEffectType.Heal},
                    new() {EffectType = Enums.eEffectType.Enhancement, ETModifies = Enums.eEffectType.Mez},
                    new() {EffectType = Enums.eEffectType.Enhancement, ETModifies = Enums.eEffectType.RechargeTime},
                    new() {EffectType = Enums.eEffectType.Enhancement, ETModifies = Enums.eEffectType.SpeedRunning},
                    new() {EffectType = Enums.eEffectType.Enhancement, ETModifies = Enums.eEffectType.SpeedJumping},
                    new() {EffectType = Enums.eEffectType.Enhancement, ETModifies = Enums.eEffectType.SpeedFlying}
                };

                var ret = effectsList
                    .ToDictionary(fx => fx, fx => new List<Color> {AssignColor(fx.EffectType, false, fx.ETModifies ?? Enums.eEffectType.None)});

                ret.Add(new EffectTypeShort {EffectType = Enums.eEffectType.Enhancement, ETModifies = Enums.eEffectType.None}, new List<Color> { Gray4, Gray3, Gray2, Gray, Gray5, Gray6, Gray7, Gray8 });

                return ret;
            }

            public Color AssignColor(Enums.eEffectType effectType, bool isProc = false,
                Enums.eEffectType etModifies = Enums.eEffectType.None, int index = 0, int maxIndex = 0)
            {
                return isProc
                    ? White
                    : effectType switch
                    {
                        Enums.eEffectType.Accuracy => Yellow2,
                        Enums.eEffectType.Damage => Red,
                        Enums.eEffectType.DamageBuff => LightPink,
                        Enums.eEffectType.Defense => LightPurple,
                        Enums.eEffectType.Endurance => Blue2,
                        Enums.eEffectType.SpeedFlying => Indigo3,
                        Enums.eEffectType.Heal => Green,
                        Enums.eEffectType.HitPoints => Green2,
                        Enums.eEffectType.JumpHeight => Indigo4,
                        Enums.eEffectType.SpeedJumping => Indigo2,
                        Enums.eEffectType.Mez => Purple,
                        Enums.eEffectType.MezResist => Purple2,
                        Enums.eEffectType.PerceptionRadius => Gray5,
                        Enums.eEffectType.Recovery => Blue,
                        Enums.eEffectType.Regeneration => Green3,
                        Enums.eEffectType.ResEffect => Indigo5,
                        Enums.eEffectType.Resistance => LightPurple2,
                        Enums.eEffectType.SpeedRunning => Indigo,
                        Enums.eEffectType.ToHit => Yellow,
                        Enums.eEffectType.Enhancement => etModifies switch
                        {
                            Enums.eEffectType.Heal => Green4,
                            Enums.eEffectType.Mez => Purple2,
                            Enums.eEffectType.RechargeTime => Orange,
                            Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping
                                or Enums.eEffectType.SpeedFlying or Enums.eEffectType.JumpHeight => Blue3,
                            _ => maxIndex < 4
                                ? index switch
                                {
                                    0 => Gray,
                                    1 => Gray2,
                                    2 => Gray3,
                                    _ => Gray4
                                }
                                : index switch
                                {
                                    0 => Gray4,
                                    1 => Gray3,
                                    2 => Gray2,
                                    3 => Gray,
                                    4 => Gray5,
                                    5 => Gray6,
                                    6 => Gray7,
                                    _ => Gray8
                                }
                        },

                        _ => White
                    };
            }
        }

        #endregion

        public List<BuildPowerSlot> Powers { get; set; }
        public List<IPower> UserBoosts { get; set; }
        public ViewProfileType Profile { get; set; }
        public bool UseArcanaTime { get; set; }
        public List<string> BuffsLookup { get; }
        public Size? NaturalSize { get; private set; }
        public List<TimelineItem> Timeline { get; private set; }
        
        private List<string> Boosts;
        private ViewProfiles Profiles;
        private ColorTheme Theme;

        public ctlCombatTimeline()
        {
            Powers = new List<BuildPowerSlot>();
            BuffsLookup = new List<string>();
            Timeline = new List<TimelineItem>();
            UseArcanaTime = true;
            Boosts = new List<string>
            {
                "Boosts.Crafted_Force_Feedback_F.Crafted_Force_Feedback_F", // Force Feedback: Chance for +Recharge
                "Boosts.Crafted_Decimation_F.Crafted_Decimation_F", // Decimation: Chance for Build Up
                "Boosts.Crafted_Gaussians_Synchronized_Firecontrol_F" // Gaussian's Synchronized Fire-Control: Chance for Build Up
            };

            UserBoosts = new List<IPower>();
            Profiles = new ViewProfiles();
            Theme = new ColorTheme();

            InitializeComponent();
        }

        /// <summary>
        /// Calculate power cast time using ArcanaTime formula
        /// </summary>
        /// <param name="castTime">Base cast time</param>
        /// <returns></returns>
        private float CalcArcanaCastTime(float castTime)
        {
            return (float)(Math.Ceiling(castTime / 0.132f) + 1) * 0.132f;
        }

        /// <summary>
        /// Place all powers on the timeline, calculate all enhanced powers
        /// </summary>
        /// <param name="redraw">Triggers a redraw after calculations.</param>
        public void PlacePowers(bool redraw = true)
        {
            if (Powers.Count <= 0)
            {
                return;
            }

            ListPowersToTimeline();
            CalcAllEnhancedPowers();

            if (!redraw)
            {
                return;
            }

            Invalidate();
        }

        /// <summary>
        /// Build temporal line from a list of powers
        /// </summary>
        private void ListPowersToTimeline()
        {
            var time = 0f;
            Timeline = new List<TimelineItem>();

            var k = 0;
            foreach (var power in Powers)
            {
                var previousPowerOccurrences = Timeline
                    .Where(e => e.PowerSlot.BasePower != null && e.PowerSlot.BasePower?.FullName == power.BasePower?.FullName)
                    .ToList();

                var previousOccurrence = previousPowerOccurrences.Count <= 0
                    ? null
                    : previousPowerOccurrences
                        .Select((e, i) => new KeyValuePair<int, TimelineItem>(i, e))
                        .MaxBy(e => e.Key)
                        .Value;

                var ffBoostDuration = DatabaseAPI.GetPowerByFullName("Set_Bonus.Set_Bonus.Force_Feedback")?.Effects
                    .First(e => e.EffectType == Enums.eEffectType.Enhancement & e.ETModifies == Enums.eEffectType.RechargeTime)
                    .Duration;

                if (k > 0)
                {
                    var rechargeBoosts = new List<RechBoost>();
                    for (var i = 0; i < k; i++)
                    {
                        var enhRechBoost = HasBoost(Powers[i]).Any(e => e != null && e.Contains("Force_Feedback_F"));
                        var powerRechBoost = Powers[i].BasePower?.ClickBuff &
                                             Powers[i].BasePower?.Effects
                                                 .Any(e => e.EffectType == Enums.eEffectType.Enhancement & e.ETModifies == Enums.eEffectType.RechargeTime & e.ToWho == Enums.eToWho.Self);

                        if (enhRechBoost)
                        {
                            rechargeBoosts.Add(new RechBoost {TimelineIndex = i, BoostType = BoostType.Enhancement, Duration = ffBoostDuration ?? 0});

                            continue;
                        }

                        if (powerRechBoost != true)
                        {
                            continue;
                        }

                        var powerBoostDuration = Powers[i].EnhancedPower?.Effects
                            .Where(e => e.EffectType == Enums.eEffectType.Enhancement &
                                        e.ETModifies == Enums.eEffectType.RechargeTime &
                                        e.ToWho == Enums.eToWho.Self & e.BuffedMag > 0)
                            .Select(e => e.Duration)
                            .Max();
                        rechargeBoosts.Add(new RechBoost
                            {TimelineIndex = i, BoostType = BoostType.Power, Duration = powerBoostDuration ?? 0});
                    }

                    var p = new TimelineItem(power, time + (previousOccurrence?.PowerSlot.EnhancedPower?.RechargeTime ?? 0));
                    CalcEnhancedPower(ref p, rechargeBoosts);

                    Timeline.Add(p);
                }
                else
                {
                    var p = new TimelineItem(power, time);
                    CalcEnhancedPower(ref p);
                    Timeline.Add(p);
                }

                time += UseArcanaTime ? power.BasePower?.ArcanaCastTime ?? 0 : power.BasePower?.CastTimeBase ?? 0;
                k++;
            }

            Timeline = Timeline.OrderBy(e => e.Time).ToList();
        }

        /// <summary>
        /// Get occurrences of a single power in the timeline
        /// </summary>
        /// <param name="power">Power to look for (from power object)</param>
        /// <returns>List of matching powers in the timeline</returns>
        private List<TimelineItem> GetPowerOccurrences(IPower? power)
        {
            return Timeline
                .Where(e => e.PowerSlot.BasePower?.FullName == power?.FullName)
                .ToList();
        }

        /// <summary>
        /// Get occurrences of a single power in the timeline
        /// </summary>
        /// <param name="powerName">Power to look for (from power full name)</param>
        /// <returns>List of matching powers in the timeline</returns>
        private List<TimelineItem> GetPowerOccurrences(string powerName)
        {
            return Timeline
                .Where(e => e.PowerSlot.BasePower?.FullName == powerName)
                .ToList();
        }

        /// <summary>
        /// Get unique powers present on a timeline
        /// </summary>
        /// <returns>List of powers full name</returns>
        private List<string> GetDistinctPowers()
        {
            return Timeline
                .Select(e => new KeyValuePair<string?, string?>(e.PowerSlot.BasePower?.FullName, e.PowerSlot.BasePower?.DisplayName))
                .OrderBy(e => e.Value)
                .Select(e => e.Key)
                .Cast<string>()
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Check if a power is affected by any boost
        /// </summary>
        /// <param name="powerSlot">Target power</param>
        /// <returns>List of active boosts as power full names</returns>
        private List<string?> HasBoost(BuildPowerSlot powerSlot)
        {
            //var buildPowerEntry = MidsContext.Character.CurrentBuild.Powers[powerSlot.HistoryIdx];
            var buildPowerEntry = GetMatchingPowerEntry(powerSlot.BasePower);
            var slotsUid = buildPowerEntry.Slots
                .Select(e => e.Enhancement.Enh < 0 ? "" : DatabaseAPI.Database.Enhancements[e.Enhancement.Enh].UID)
                .ToList();

            return slotsUid
                .Where(e => !string.IsNullOrEmpty(e))
                .Select(e =>
                    DatabaseAPI.Database.Power
                        .DefaultIfEmpty(new Power { StaticIndex = -1 })
                        .FirstOrDefault(f => f?.FullName.EndsWith(e) == true))
                .Select(e => e == null || e.StaticIndex < 0 || !Boosts.Contains(e.FullName) ? null : e.FullName)
                .ToList();
        }
        
        /// <summary>
        /// Get active boost sources for a power in the timeline
        /// </summary>
        /// <param name="timelineItem">Target power</param>
        /// <returns>List of boosting powers in the timeline, if any</returns>
        private List<TimelineItem>? IsAffectedByBoosts(TimelineItem timelineItem)
        {
            var boostsMaxDuration = Boosts
                .Select(DatabaseAPI.GetPowerByFullName)
                .Select(e => e == null ? 0 : e.Effects.Max(f => f.Duration))
                .ToList();

            var absoluteMaxDuration = boostsMaxDuration.Max();
            var timelineIntervalPowers = Timeline
                .Where(e => (e.Time >= timelineItem.Time - absoluteMaxDuration) & e.Time < timelineItem.Time)
                .ToList();

            // Assume target boosts have 100% proc chance
            var activeBoostsSources = timelineIntervalPowers
                .Select(e => new KeyValuePair<TimelineItem, List<string?>>(e, HasBoost(e.PowerSlot)))
                .Where(e => e.Value.Any(f => f != null))
                .Select(e => e.Key)
                .ToList();

            var userBoostsNames = UserBoosts
                .Select(e => e.FullName)
                .ToList();
            var userBoostSources = UserBoosts.Count <= 0
                ? new List<TimelineItem>()
                : Timeline
                    .Where(e => e.PowerSlot.BasePower != null && userBoostsNames.Contains(e.PowerSlot.BasePower.FullName))
                    .Where(e => e.Time < timelineItem.Time & e.Time + (e.PowerSlot.EnhancedPower == null ? 0 : e.PowerSlot.EnhancedPower.Effects.Max(f => f.Duration)) >= e.Time)
                    .ToList();
            var boostSources = activeBoostsSources
                .Union(userBoostSources)
                .Distinct()
                .ToList();

            return boostSources.Count <= 0 ? null : boostSources;
        }

        /// <summary>
        /// Calculate all enhanced powers
        /// </summary>
        /// <remarks>Can take some time depending on how many powers are on the timeline.</remarks>
        private void CalcAllEnhancedPowers()
        {
            if (Timeline.Count == 0)
            {
                return;
            }

            CalcEnhancedProgress?.Invoke(this, 0);

            for (var i = 0; i < Timeline.Count; i++)
            {
                var p = Timeline[i];
                CalcEnhancedPower(ref p);
                CalcEnhancedProgress?.Invoke(this, i / (float) Timeline.Count * 100);
            }

            RecalcTotals();

            CalcEnhancedProgress?.Invoke(this, 100);
        }

        /// <summary>
        /// Calculate enhanced power from a base one with only boosting powers active
        /// </summary>
        /// <param name="timelinePower">Target power</param>
        /// <param name="recalcStats">Recalculate totals with original activation state when done</param>
        /// <remarks>Warning: possibly very slow</remarks>
        private void CalcEnhancedPower(ref TimelineItem timelinePower, bool recalcStats = false)
        {
            var origProcIncludes = MidsContext.Character.CurrentBuild.Powers
                .Select(e => e?.ProcInclude)
                .ToList();

            var origStatIncludes = MidsContext.Character.CurrentBuild.Powers
                .Select(e => e?.StatInclude)
                .ToList();

            foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
            {
                if (pe == null)
                {
                    continue;
                }

                pe.ProcInclude = false;
            }

            var userBoostNames = UserBoosts
                .Select(e => e.FullName)
                .ToList();

            foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
            {
                if (pe == null)
                {
                    continue;
                }

                if (!userBoostNames.Contains(pe.Power?.FullName))
                {
                    continue;
                }

                pe.StatInclude = false;
            }

            var boostingPowers = IsAffectedByBoosts(timelinePower);
            if (boostingPowers != null)
            {
                foreach (var p in boostingPowers)
                {
                    var pe = GetMatchingPowerEntry(p.PowerSlot.BasePower);

                    if (pe == null)
                    {
                        continue;
                    }

                    pe.ProcInclude = true;
                    pe.StatInclude = true;
                }
            }

            RecalcTotals();

            // Select enhanced power by historyIdx doesn't work.
            var pName = timelinePower.PowerSlot.BasePower == null ? "" : timelinePower.PowerSlot.BasePower.FullName;
            timelinePower.PowerSlot.SetEnhancedPower(MainModule.MidsController.Toon.GetEnhancedPower(timelinePower.PowerSlot.BasePower));

            if (!recalcStats)
            {
                // frmMain.DoRedraw() ?
                return;
            }

            for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                if (MidsContext.Character.CurrentBuild.Powers[i] == null)
                {
                    continue;
                }

                MidsContext.Character.CurrentBuild.Powers[i].ProcInclude = origProcIncludes[i] == true;
                MidsContext.Character.CurrentBuild.Powers[i].StatInclude = origStatIncludes[i] == true;
            }

            RecalcTotals();
        }

        private void CalcEnhancedPower(ref TimelineItem timelinePower, List<RechBoost> rechargeBoosts, bool recalcStats = false)
        {
            var origProcIncludes = MidsContext.Character.CurrentBuild.Powers
                .Select(e => e?.ProcInclude)
                .ToList();

            var origStatIncludes = MidsContext.Character.CurrentBuild.Powers
                .Select(e => e?.StatInclude)
                .ToList();

            foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
            {
                if (pe == null)
                {
                    continue;
                }

                pe.ProcInclude = false;
            }

            var userBoostNames = UserBoosts
                .Select(e => e.FullName)
                .ToList();

            foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
            {
                if (pe == null)
                {
                    continue;
                }

                if (!userBoostNames.Contains(pe.Power?.FullName))
                {
                    continue;
                }

                pe.StatInclude = false;
            }

            var boostingPowers = IsAffectedByBoosts(timelinePower);
            if (boostingPowers != null)
            {
                foreach (var p in boostingPowers)
                {
                    var pe = GetMatchingPowerEntry(p.PowerSlot.BasePower);

                    if (pe == null)
                    {
                        continue;
                    }

                    pe.ProcInclude = true;
                    pe.StatInclude = true;
                }
            }

            foreach (var b in rechargeBoosts)
            {
                if (Timeline[b.TimelineIndex].Time + b.Duration < timelinePower.Time)
                {
                    continue;
                }

                var pe = GetMatchingPowerEntry(Timeline[b.TimelineIndex].PowerSlot.BasePower);

                if (pe == null)
                {
                    continue;
                }

                switch (b.BoostType)
                {
                    case BoostType.Enhancement:
                        pe.ProcInclude = true;
                        break;

                    case BoostType.Power:
                        pe.StatInclude = true;
                        break;
                }
            }

            RecalcTotals();

            // Select enhanced power by historyIdx doesn't work.
            var pName = timelinePower.PowerSlot.BasePower == null ? "" : timelinePower.PowerSlot.BasePower.FullName;
            timelinePower.PowerSlot.SetEnhancedPower(MainModule.MidsController.Toon.GetEnhancedPower(timelinePower.PowerSlot.BasePower));

            if (!recalcStats)
            {
                // frmMain.DoRedraw() ?
                return;
            }

            for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                if (MidsContext.Character.CurrentBuild.Powers[i] == null)
                {
                    continue;
                }

                MidsContext.Character.CurrentBuild.Powers[i].ProcInclude = origProcIncludes[i] == true;
                MidsContext.Character.CurrentBuild.Powers[i].StatInclude = origStatIncludes[i] == true;
            }

            RecalcTotals();
        }

        private PowerEntry? GetMatchingPowerEntry(IPower? power)
        {
            if (power == null)
            {
                return null;
            }

            return MidsContext.Character.CurrentBuild.Powers
                .Where(e => e is { Power: not null })
                .DefaultIfEmpty(null)
                .FirstOrDefault(e => e.Power.FullName == power.FullName);
        }
        
        /// <summary>
        /// Trigger a full character stats re-calculation
        /// </summary>
        private void RecalcTotals()
        {
            if (MainModule.MidsController.Toon == null | !MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            MainModule.MidsController.Toon?.GenerateBuffedPowerArray();
        }

        private List<FxIdentifier> SelectViewProfile(ViewProfileType profile)
        {
            return profile switch
            {
                ViewProfileType.Healing => Profiles.Healing,
                ViewProfileType.Survival => Profiles.Survival,
                _ => Profiles.Damage
            };
        }

        /// <summary>
        /// Filter effects according to profile
        /// </summary>
        /// <param name="gfx">Grouped effects</param>
        /// <param name="power">Target power</param>
        /// <param name="profile">View profile</param>
        /// <returns>Filtered effects list</returns>
        private List<GroupedFx> ApplyViewProfile(IEnumerable<GroupedFx> gfx, IPower power, ViewProfileType profile)
        {
            var profileFilter = SelectViewProfile(profile);

            var gfxList = gfx.ToList();
            return gfxList
                .Select(e => new KeyValuePair<FxIdentifier, GroupedFx>(new FxIdentifier(
                    e.EffectType,
                    e.ETModifies == Enums.eEffectType.None ? null : e.ETModifies,
                    e.MezType == Enums.eMez.None ? null : e.MezType,
                    e.ToWho,
                    e.GetMagSum(power) switch
                    {
                        > 0 => ValueSign.Positive,
                        < 0 => ValueSign.Negative,
                        _ => ValueSign.Zero
                    }), e))
                .Where(e => FilterGfx(e.Key, profileFilter))
                .Select(e => e.Value)
                .ToList();
        }

        private bool FilterGfx(FxIdentifier fxIdentifier, IEnumerable<FxIdentifier> profile)
        {
            return (from e in profile
                let effectTypeCheck = e.EffectType == null | e.EffectType == fxIdentifier.EffectType
                let mezTypeCheck = e.MezType == null | e.MezType == fxIdentifier.MezType
                let etModifies = e.ETModifies == null | e.ETModifies == fxIdentifier.ETModifies
                let toWhoCheck = e.ToWho is Enums.eToWho.All or Enums.eToWho.Unspecified | e.ToWho == fxIdentifier.ToWho
                let valueSignCheck = e.ValueSign == null | e.ValueSign == fxIdentifier.ValueSign
                where effectTypeCheck & mezTypeCheck & etModifies & toWhoCheck & valueSignCheck
                select effectTypeCheck).Any();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var bgColor = Theme.BackgroundColor;
            var textColor = Theme.TextColor;
            var shadowColor = Theme.ShadowColor;

            var distinctPowers = GetDistinctPowers();
            if (distinctPowers.Count <= 0)
            {
                g.Clear(bgColor);

                return;
            }

            Debug.WriteLine($"Distinct powers: {distinctPowers.Count}");
            Debug.WriteLine($"Timeline:\r\n{string.Join("\r\n", Timeline)}");
            Debug.WriteLine("\r\n--------------------------\r\n");

            var powerHeights = distinctPowers
                .Select(e => Timeline
                    .Where(f => f.PowerSlot.EnhancedPower?.FullName == e)
                    .Select(f => ApplyViewProfile(GroupedFx.AssembleGroupedEffects(f.PowerSlot.EnhancedPower), f.PowerSlot.EnhancedPower, Profile).Count))
                .Select(e => e.Max())
                .ToList();
            var powersRows = distinctPowers
                .Select((e, i) => new KeyValuePair<string, int>(e, i))
                .ToDictionary(e => e.Key, e => e.Value);
            var lastPowerDuration = Timeline.Count <= 0 ? 0 : Timeline[^1].PowerSlot.EnhancedPower?.Effects.Max(e => e.Duration);
            var maxTime = Timeline.Count <= 0 ? 1 : Timeline[^1].Time + (lastPowerDuration ?? 0);

            const int padding = 8;
            const int lineThickness = 3;
            const int interlineHeight = 16;
            const float normalTextSize = 11; // px
            const int textGapLeft = 90; // px

            var hScale = (Width - 2 * padding - textGapLeft) / maxTime; // time -> pixels
            var totalHeight = powerHeights.Sum() + Math.Max(0, powerHeights.Count - 1) * interlineHeight;
            var vScale = totalHeight < float.Epsilon ? 1 : (Height - 2 * padding) / totalHeight;
            NaturalSize = new Size((int)Math.Ceiling(maxTime + 2 * padding), totalHeight);

            var font = new Font(new FontFamily("Microsoft Sans Serif"), normalTextSize, FontStyle.Regular, GraphicsUnit.Pixel);

            g.Clear(bgColor);

            var k = 0;
            foreach (var p in Timeline)
            {
                var pIndex = powersRows[p.PowerSlot.EnhancedPower?.FullName];
                var vOffset = padding + pIndex == 0 ? 0 : powerHeights.Take(pIndex - 1).Sum() + interlineHeight * pIndex;

                Debug.WriteLine("\r\n=====================\r\n");
                Debug.WriteLine($"Drawing power {p.PowerSlot.BasePower?.FullName} - pIndex: {pIndex}, hOffset: {vOffset}, EnhancedPower is null: {p.PowerSlot.EnhancedPower == null}");

                if (p.PowerSlot.EnhancedPower == null)
                {
                    continue;
                }

                var vp = 0f;
                
                var gfxRaw = GroupedFx.AssembleGroupedEffects(p.PowerSlot.EnhancedPower, true);
                var gfx = ApplyViewProfile(gfxRaw, p.PowerSlot.EnhancedPower, Profile);
                var genericEnhancements = gfx
                    .Select((e, i) => new KeyValuePair<int, GroupedFx>(i, e))
                    .Where(e => e.Value.EffectType == Enums.eEffectType.Enhancement &
                                e.Value.ETModifies is not (Enums.eEffectType.RechargeTime or Enums.eEffectType.Heal
                                    or Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping
                                    or Enums.eEffectType.SpeedFlying or Enums.eEffectType.JumpHeight
                                    or Enums.eEffectType.Mez))
                    .ToDictionary(e => e.Key, e => e.Value);

                Debug.WriteLine("");
                Debug.WriteLine($"GroupedFX count (filtered with {Profile} profile): {gfx.Count}");

                Debug.WriteLine($"TextRendererExt.DrawOutlineText(g, \"{p.PowerSlot.EnhancedPower.FullName}\", font, <Rectangle>{{0, {vOffset}, {textGapLeft}, {powerHeights[pIndex]}}}, shadowColor, textColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);");
                TextRendererExt.DrawOutlineText(g, p.PowerSlot.EnhancedPower.FullName, font, new Rectangle(0, vOffset, textGapLeft, vOffset - Height - padding), shadowColor, textColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

                // Move damage/heal effects to last elements so they are drawn on top of the others.
                gfx = gfx
                    .OrderBy(f => $"{f.EffectType}")
                    .ThenBy(f => f.EffectType is Enums.eEffectType.Damage or Enums.eEffectType.Heal ? 1 : 0)
                    .ToList();

                for (var i = 0; i < gfx.Count; i++)
                {
                    var index = genericEnhancements.ContainsKey(i)
                        ? genericEnhancements.Keys.TryFindIndex(f => f == i)
                        : 0;
                    var maxIndex = gfx[i].EffectType == Enums.eEffectType.Enhancement
                        ? genericEnhancements.Count
                        : 0;
                    var barColor = Theme.AssignColor(gfx[i].EffectType, gfx[i].EnhancementEffect, gfx[i].ETModifies, index, maxIndex);
                    var linePen = new Pen(new SolidBrush(barColor), lineThickness);

                    if (gfx[i].GetEffectAt(p.PowerSlot.EnhancedPower).Duration < float.Epsilon)
                    {
                        Debug.WriteLine($"gfx[{i}] - DrawEllipse(x:{padding + textGapLeft + p.Time * hScale - 6}, y:{vOffset + vp - 6 - lineThickness / 2f}, w:12, h:12");
                        g.DrawEllipse(linePen, new RectangleF(padding + textGapLeft + p.Time * hScale - 6, vOffset + vp - 6 - lineThickness / 2f, 12, 12));
                    }
                    else
                    {
                        Debug.WriteLine($"gfx[{i}] - DrawLine(x1:{padding + textGapLeft + p.Time * hScale}, y1:{vOffset + vp}, x2:{padding + textGapLeft + (p.Time + gfx[i].GetEffectAt(p.PowerSlot.EnhancedPower).Duration) * hScale}, y2:{vOffset + vp}");
                        g.DrawLine(linePen, padding + textGapLeft + p.Time * hScale, vOffset + vp, padding + textGapLeft + (p.Time + gfx[i].GetEffectAt(p.PowerSlot.EnhancedPower).Duration) * hScale, vOffset + vp);
                    }

                    vp += lineThickness / 2f;
                }
            }
        }
    }
}

using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.PlannerRulesets;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Test;
using System.Text.RegularExpressions;
using FastDeepCloner;

namespace Mids_Reborn.Core
{
    #region List extensions

    public static class ListExtGFx
    {
        public static bool ContainsAll<T>(this List<T> list, IEnumerable<T> elements)
        {
            if (list.Count < 16)
            {
                foreach (var e in elements)
                    if (!list.Contains(e)) return false;
                return true;
            }

            var set = new HashSet<T>(list);
            foreach (var e in elements)
                if (!set.Contains(e)) return false;
            return true;
        }

        public static void AddRangeUnique<T>(this List<T> list, IEnumerable<T> elements)
        {
            var set = new HashSet<T>(list);
            foreach (var e in elements)
                if (set.Add(e)) list.Add(e);
        }
    }

    #endregion

    #region Dictionary extensions

    public static class DictionaryExtGFx
    {
        public static bool ContainsKeyPrefix<T1, T2>(this Dictionary<T1, T2> dict, string prefix, ref string? nameFound) where T1 : notnull
        {
            foreach (var k in dict.Keys)
            {
                var s = $"{k}";
                if (s.StartsWith(prefix, StringComparison.Ordinal))
                {
                    nameFound = s;
                    return true;
                }
            }
            nameFound = null;
            return false;
        }
    }

    #endregion

    /// <summary>
    /// Canonical presentation grouper for power and enhancement effects.
    /// The public surface stays centered on <see cref="GroupedFx"/>, while
    /// the internal pipeline is split across well-defined helper sections that:
    /// 1. Select source effects for presentation.
    /// 2. Group compatible effects into display rows.
    /// 3. Compact vectors such as typed damage and mez into human-readable labels.
    /// 4. Project grouped rows into popup text, paired-list items, and effect-grid items.
    ///
    /// Other classes should treat this type as the only authority for grouped
    /// presentation output and should not apply extra regex or string cleanup
    /// after it returns display text.
    /// </summary>
    public class GroupedFx : ICloneable
    {
        private const float Tolerance = 1e-4f;

        private static bool MagnitudesMatch(float left, float right)
        {
            if (float.IsNaN(left) || float.IsNaN(right))
            {
                return float.IsNaN(left) && float.IsNaN(right);
            }

            if (float.IsInfinity(left) || float.IsInfinity(right))
            {
                return left.Equals(right);
            }

            return Math.Abs(left - right) < Tolerance;
        }

        private static Enums.eEffectType GetPresentationEffectType(IEffect effect)
        {
            return effect.EffectType == Enums.eEffectType.Resistance && effect.MezType != Enums.eMez.None
                ? Enums.eEffectType.MezResist
                : effect.EffectType;
        }

        // ===== Canonical presentation facade =====

        /// <summary>
        /// Controls how grouped presentation rows are joined when the caller
        /// asks for rendered text instead of structured rows.
        /// </summary>
        internal enum GroupedFxLineJoinMode
        {
            MultiLine,
            SingleLine
        }

        /// <summary>
        /// Immutable request for grouped presentation output. Callers specify
        /// the source power plus the subset/filter/joining rules they need,
        /// and the canonical <see cref="GroupedFx"/> pipeline handles the rest.
        /// </summary>
        internal sealed record GroupedFxPresentationRequest(
            IEnumerable<int>? IncludedEffectIds = null,
            Func<GroupedFx, IEffect, bool>? GroupFilter = null,
            bool IncludeDamage = true,
            bool SimpleText = true,
            bool IgnoreConditions = true,
            GroupedFxLineJoinMode LineJoinMode = GroupedFxLineJoinMode.MultiLine);

        /// <summary>
        /// Neutral row model shared by popup text and effect-list projections.
        /// It carries the already-grouped text plus any optional list metadata.
        /// </summary>
        internal sealed record GroupedFxPresentationRow(
            FxId Identifier,
            IReadOnlyList<int> IncludedEffectIds,
            string Label,
            string Text,
            string? Value,
            string? AltValue,
            string? Tooltip,
            bool IsSpecial,
            bool IsConditional,
            bool IsUnique,
            bool IsEnhancementEffect);

        /// <summary>
        /// Canonical grouped presentation result. Rows stay structured for
        /// callers that need metadata, while convenience helpers can render the
        /// rows back into popup/list strings using one join rule.
        /// </summary>
        internal sealed class GroupedFxPresentationResult
        {
            private readonly GroupedFxLineJoinMode _defaultJoinMode;

            internal GroupedFxPresentationResult(
                IReadOnlyList<GroupedFxPresentationRow> rows,
                GroupedFxLineJoinMode defaultJoinMode)
            {
                Rows = rows;
                _defaultJoinMode = defaultJoinMode;
            }

            internal IReadOnlyList<GroupedFxPresentationRow> Rows { get; }

            internal IReadOnlyList<string> ToLines(GroupedFxLineJoinMode? joinModeOverride = null)
            {
                if (Rows.Count == 0)
                {
                    return Array.Empty<string>();
                }

                var joinMode = joinModeOverride ?? _defaultJoinMode;
                var seen = new HashSet<string>(StringComparer.Ordinal);
                var parts = new List<string>();

                foreach (var row in Rows)
                {
                    foreach (var line in SplitPresentationLines(row.Text))
                    {
                        if (seen.Add(line))
                        {
                            parts.Add(line);
                        }
                    }
                }

                if (parts.Count == 0)
                {
                    return Array.Empty<string>();
                }

                return joinMode == GroupedFxLineJoinMode.SingleLine
                    ? new[] { string.Join(", ", parts) }
                    : parts;
            }

            internal string ToText(
                string separator = "\n",
                GroupedFxLineJoinMode? joinModeOverride = null)
            {
                return string.Join(separator, ToLines(joinModeOverride));
            }
        }

        private static readonly Dictionary<string, string> PresentationTokenOverrides = new(StringComparer.Ordinal)
        {
            ["DamageBuff"] = "Damage Buff",
            ["MezProtect"] = "Status Protection",
            ["MezResist"] = "Status Resistance",
            ["RechargeTime"] = "Recharge Speed",
            ["HitPoints"] = "Hit Points",
            ["ToHit"] = "To-Hit",
            ["EndDiscount"] = "Endurance Discount",
            ["EnduranceDiscount"] = "Endurance Discount",
            ["ThreatLevel"] = "Threat Level",
            ["SpeedRunning"] = "Run Speed",
            ["SpeedJumping"] = "Jump Speed",
            ["SpeedFlying"] = "Fly Speed",
            ["JumpHeight"] = "Jump Height",
            ["StealthRadius"] = "Stealth Radius",
            ["StealthRadiusPlayer"] = "Player Stealth Radius",
            ["PerceptionRadius"] = "Perception Radius",
            ["MovementControl"] = "Movement Control",
            ["MovementFriction"] = "Movement Friction",
            ["MaxRunSpeed"] = "Max Run Speed",
            ["MaxJumpSpeed"] = "Max Jump Speed",
            ["MaxFlySpeed"] = "Max Fly Speed"
        };

        private static readonly Dictionary<string, string> PresentationPhraseOverrides = new(StringComparer.Ordinal)
        {
            ["Mez Resist"] = "Status Resistance",
            ["Recharge Time"] = "Recharge Speed",
            ["Slf"] = "Self",
            ["Tgt"] = "Target"
        };

        private static readonly Regex PresentationLabelTokenRegex =
            new(@"\b[A-Z][a-z]+(?:[A-Z][a-z]+)+\b", RegexOptions.Compiled);

        internal static string FormatPresentationText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var normalized = value.Replace("\r\n", "\n", StringComparison.Ordinal);
            var lines = normalized.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = FormatPresentationLine(lines[i]);
            }

            return string.Join("\n", lines);
        }

        internal static IReadOnlyList<string> BuildPopupTooltipLines(
            IPower? power,
            IEnumerable<int>? includedEffects = null,
            Func<GroupedFx, IEffect, bool>? groupFilter = null,
            GroupedFxLineJoinMode lineJoinMode = GroupedFxLineJoinMode.MultiLine,
            bool simpleText = true,
            bool ignoreConditions = true)
        {
            var request = new GroupedFxPresentationRequest(
                IncludedEffectIds: includedEffects,
                GroupFilter: groupFilter,
                SimpleText: simpleText,
                IgnoreConditions: ignoreConditions,
                LineJoinMode: lineJoinMode);

            return BuildPopupPresentation(power, request).ToLines();
        }

        /// <summary>
        /// Runs the canonical presentation pipeline and returns already-grouped,
        /// already-normalized rows for callers that need metadata rather than a
        /// flattened text block.
        /// </summary>
        internal static IReadOnlyList<GroupedFxPresentationRow> BuildPresentationRows(
            IPower? power,
            GroupedFxPresentationRequest request)
        {
            return BuildPopupPresentation(power, request).Rows;
        }

        /// <summary>
        /// Convenience wrapper for callers that only need rendered text.
        /// This still goes through the same row-building pipeline as every
        /// other presentation surface.
        /// </summary>
        internal static string BuildPresentationText(IPower? power, GroupedFxPresentationRequest request)
        {
            return BuildPopupPresentation(power, request).ToText();
        }

        private static int GetPresentationOrder(GroupedFx groupedEffect)
        {
            return groupedEffect.EffectType switch
            {
                // Show typed resistance before companion mez/status resistance
                // when both come from the same source bonus/effect bundle.
                Enums.eEffectType.MezProtect => 1,
                Enums.eEffectType.MezResist => 1,
                _ => 0
            };
        }

        /// <summary>
        /// Canonical row builder used by tooltips, set-bonus popups, and any
        /// other caller that needs grouped effect presentation. The steps are:
        /// 1. Select grouped effects for the requested power/effect subset.
        /// 2. Filter them for the caller's surface-specific needs.
        /// 3. Project each group into a neutral, normalized presentation row.
        /// </summary>
        private static GroupedFxPresentationResult BuildPopupPresentation(
            IPower? power,
            GroupedFxPresentationRequest request)
        {
            if (power == null)
            {
                return new GroupedFxPresentationResult(Array.Empty<GroupedFxPresentationRow>(), request.LineJoinMode);
            }

            var groupedEffects = SelectPresentationGroups(power, request)
                .OrderBy(GetPresentationOrder)
                .ToArray();
            if (groupedEffects.Length == 0)
            {
                return new GroupedFxPresentationResult(Array.Empty<GroupedFxPresentationRow>(), request.LineJoinMode);
            }

            var rows = new List<GroupedFxPresentationRow>(groupedEffects.Length);
            foreach (var groupedEffect in groupedEffects)
            {
                var effect = groupedEffect.GetEffectAt(power);
                if (request.GroupFilter != null && !request.GroupFilter(groupedEffect, effect))
                {
                    continue;
                }

                var rawText = groupedEffect
                    .GetTooltip(power, simple: request.SimpleText, ignoreConditions: request.IgnoreConditions)
                    .Trim();
                if (string.IsNullOrWhiteSpace(rawText))
                {
                    continue;
                }

                rows.Add(new GroupedFxPresentationRow(
                    groupedEffect.FxIdentifier,
                    groupedEffect.IncludedEffects.ToArray(),
                    FormatPresentationText(BuildLabel(groupedEffect, power)),
                    FormatPresentationText(rawText),
                    null,
                    null,
                    FormatPresentationText(rawText),
                    groupedEffect.HasConditions,
                    false,
                    groupedEffect.EnhancementEffect,
                    groupedEffect.EnhancementEffect));
            }

            return new GroupedFxPresentationResult(rows, request.LineJoinMode);
        }

        /// <summary>
        /// Selects the grouped effects that participate in the presentation
        /// request. Callers can either render the entire power or only a
        /// specific subset of effect indices.
        /// </summary>
        private static IEnumerable<GroupedFx> SelectPresentationGroups(
            IPower power,
            GroupedFxPresentationRequest request)
        {
            if (request.IncludedEffectIds == null)
            {
                return AssembleGroupedEffects(power, includeDamage: request.IncludeDamage);
            }

            return AssembleGroupedEffects(power, request.IncludedEffectIds, includeDamage: request.IncludeDamage);
        }

        private static string FormatPresentationLine(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var formatted = value.Replace("EndRec", "Recovery", StringComparison.Ordinal);

            formatted = PresentationLabelTokenRegex.Replace(formatted, match =>
            {
                if (PresentationTokenOverrides.TryGetValue(match.Value, out var replacement))
                {
                    return replacement;
                }

                return Regex.Replace(match.Value, "(?<=[a-z])(?=[A-Z])", " ");
            });

            foreach (var phraseOverride in PresentationPhraseOverrides)
            {
                formatted = Regex.Replace(
                    formatted,
                    $@"\b{Regex.Escape(phraseOverride.Key)}\b",
                    phraseOverride.Value,
                    RegexOptions.CultureInvariant);
            }

            formatted = Regex.Replace(formatted, @"\bRech\b", "Recharge");
            formatted = Regex.Replace(formatted, @"(?<=[A-Za-z])\(", " (");
            formatted = Regex.Replace(formatted, @",\s*", ", ");
            formatted = Regex.Replace(formatted, @"\(([^()]+)\)", match =>
            {
                var inner = Regex.Replace(match.Groups[1].Value, @",\s*", ", ");
                return $"({inner})";
            });

            return Regex.Replace(formatted, @"[ \t]{2,}", " ").Trim();
        }

        private static IEnumerable<string> SplitPresentationLines(string value)
        {
            return FormatPresentationText(value)
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        private struct EnhanceableFxId : IEquatable<EnhanceableFxId>
        {
            public Enums.eEffectType EffectType;
            public Enums.eEffectType? ETModifies;
            public Enums.eMez? MezType;
            public Enums.eToWho ToWho;
            public Enums.ePvX PvMode;

            public bool Equals(EnhanceableFxId other)
            {
                return EffectType == other.EffectType && ETModifies == other.ETModifies && ToWho == other.ToWho && PvMode == other.PvMode;
            }

            public override bool Equals(object? obj)
            {
                return obj is EnhanceableFxId other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine((int)EffectType, ETModifies, (int)ToWho, (int)PvMode);
            }
        }

        public record struct FxId
        {
            public Enums.eEffectType EffectType;
            public Enums.eMez MezType;
            public Enums.eDamage DamageType;
            public Enums.eEffectType ETModifies;
            public Enums.eToWho ToWho;
            public Enums.ePvX PvMode;
            public int SummonId;
            public float Duration;
            public bool IgnoreScaling;

            public override string ToString()
            {
                return $"<FxId> {{Type: {EffectType}, Modifies: {ETModifies}, Mez: {MezType}, Damage: {DamageType}, ToWho: {ToWho}, PvMode: {PvMode}, IgnoreScaling: {IgnoreScaling}}}";
            }
        }

        public struct EnhancedMagSum
        {
            public float Base;
            public float Enhanced;

            public override string ToString()
            {
                return $"<EnhancedMagSum> {{Base: {Base}, Enhanced: {Enhanced}}}";
            }
        }

        private FxId FxIdentifier;
        private string ConditionIdentity;
        private float Mag;
        private string Alias;
        private List<int> IncludedEffects;
        private bool IsEnhancement;
        private bool IsDefianceGroup;
        private IEffect? SingleEffectSource;
        private bool IsAggregated;
        private bool HasMisc;

        public int NumEffects => IncludedEffects.Count;
        public Enums.eEffectType EffectType => FxIdentifier.EffectType;
        public Enums.eEffectType ETModifies => FxIdentifier.ETModifies;
        public Enums.eMez MezType => FxIdentifier.MezType;
        public Enums.eDamage DamageType => FxIdentifier.DamageType;
        public Enums.eToWho ToWho => FxIdentifier.ToWho;
        public Enums.ePvX PvMode => FxIdentifier.PvMode;
        public bool IgnoreScaling => FxIdentifier.IgnoreScaling;
        public bool EnhancementEffect => IsEnhancement;
        public bool HasConditions => !string.IsNullOrWhiteSpace(ConditionIdentity);
        internal IReadOnlyList<int> IncludedEffectIds => IncludedEffects;
        internal bool DefianceTagged => IsDefianceGroup;

        public object Clone()
        {
            return (GroupedFx)MemberwiseClone();
        }

        public GroupedFx(FxId fxIdentifier, float mag, string alias,
            List<int> includedEffects, bool isEnhancement, string? conditionIdentity = null, bool isDefianceGroup = false)
        {
            FxIdentifier = fxIdentifier;
            Mag = mag;
            Alias = alias;
            IncludedEffects = includedEffects;
            IsEnhancement = isEnhancement;
            ConditionIdentity = conditionIdentity ?? string.Empty;
            IsDefianceGroup = isDefianceGroup;
            SingleEffectSource = null;
            IsAggregated = false;
            HasMisc = false;
        }

        public GroupedFx(FxId fxIdentifier, List<GroupedFx> greList)
        {
            FxIdentifier = fxIdentifier;
            Mag = greList[0].Mag;
            Alias = greList[0].Alias;
            IsEnhancement = greList[0].IsEnhancement;
            ConditionIdentity = greList[0].ConditionIdentity;
            IsDefianceGroup = greList.Any(gre => gre.IsDefianceGroup);
            IsAggregated = true;
            SingleEffectSource = null;
            HasMisc = false;

            IncludedEffects = [];
            foreach (var gre in greList)
            {
                IncludedEffects.AddRangeUnique(gre.IncludedEffects);
            }

            if (IncludedEffects.Count <= 1)
            {
                IsAggregated = false;
            }

            IncludedEffects.Sort();
        }

        public GroupedFx(IEffect effect, int fxIndex)
        {
            SingleEffectSource = effect;
            FxIdentifier = new FxId
            {
                DamageType = effect.DamageType,
                EffectType = effect.EffectType,
                ETModifies = effect.ETModifies,
                MezType = effect.MezType,
                ToWho = effect.ToWho,
                PvMode = effect.PvMode,
                IgnoreScaling = effect.IgnoreScaling
            };
            Mag = effect.BuffedMag;
            Alias = "";
            IncludedEffects = [fxIndex];
            IsEnhancement = effect.isEnhancementEffect;
            ConditionIdentity = effect.ConditionIdentity;
            IsDefianceGroup = IsModernDefianceDisplayEffect(effect);
            IsAggregated = false;
            HasMisc = false;
        }

        public GroupedFx()
        {
            SingleEffectSource = null;
            FxIdentifier = new FxId
            {
                DamageType = Enums.eDamage.None,
                EffectType = Enums.eEffectType.None,
                ETModifies = Enums.eEffectType.None,
                MezType = Enums.eMez.None,
                ToWho = Enums.eToWho.Unspecified,
                PvMode = Enums.ePvX.Any,
                IgnoreScaling = false
            };
            Mag = 0;
            Alias = "";
            IncludedEffects = [];
            IsEnhancement = false;
            ConditionIdentity = string.Empty;
            IsDefianceGroup = false;
            IsAggregated = false;
            HasMisc = false;

        }

        public override string ToString()
        {
            return $"<GroupedFx> {{{FxIdentifier}, effects: {IncludedEffects.Count}, Mag: {Mag}, EnhancementFx: {IsEnhancement}, Conditional: {HasConditions}, Defiance: {IsDefianceGroup}, Aggregated: {IsAggregated}, HasMisc: {HasMisc}}}";
        }

        public int GetRankedEffectIndex(IEnumerable<int> rankedEffects, int index)
        {
            if (IncludedEffects.Count <= 0) return -1;

            return rankedEffects.TryFindIndex(e => e == IncludedEffects[index]);
        }

        public IEffect GetEffectAt(IPower power, int index = 0)
        {
            return power.Effects[IncludedEffects[index]];
        }

        // ===== Vector universes =====

        private static readonly Enums.eDamage[] PositionalDefenseVectors =
        [
            Enums.eDamage.Melee,
            Enums.eDamage.Ranged,
            Enums.eDamage.AoE
        ];

        private static readonly Enums.eDamage[] TypedDefenseVectorsWithoutToxic =
        [
            Enums.eDamage.Smashing,
            Enums.eDamage.Lethal,
            Enums.eDamage.Fire,
            Enums.eDamage.Cold,
            Enums.eDamage.Energy,
            Enums.eDamage.Negative,
            Enums.eDamage.Psionic
        ];

        private static readonly Enums.eDamage[] TypedDefenseVectorsWithToxic =
        [
            Enums.eDamage.Smashing,
            Enums.eDamage.Lethal,
            Enums.eDamage.Fire,
            Enums.eDamage.Cold,
            Enums.eDamage.Energy,
            Enums.eDamage.Negative,
            Enums.eDamage.Psionic,
            Enums.eDamage.Toxic
        ];

        private static readonly Enums.eDamage[] AllDefenseVectorsWithoutToxic =
        [
            Enums.eDamage.Smashing,
            Enums.eDamage.Lethal,
            Enums.eDamage.Fire,
            Enums.eDamage.Cold,
            Enums.eDamage.Energy,
            Enums.eDamage.Negative,
            Enums.eDamage.Psionic,
            Enums.eDamage.Melee,
            Enums.eDamage.Ranged,
            Enums.eDamage.AoE
        ];

        private static readonly Enums.eDamage[] AllDefenseVectorsWithToxic =
        [
            Enums.eDamage.Smashing,
            Enums.eDamage.Lethal,
            Enums.eDamage.Fire,
            Enums.eDamage.Cold,
            Enums.eDamage.Energy,
            Enums.eDamage.Negative,
            Enums.eDamage.Psionic,
            Enums.eDamage.Toxic,
            Enums.eDamage.Melee,
            Enums.eDamage.Ranged,
            Enums.eDamage.AoE
        ];

        private static readonly Enums.eDamage[] AllDefenseVectorsExWithoutToxic =
        [
            Enums.eDamage.None,
            Enums.eDamage.Smashing,
            Enums.eDamage.Lethal,
            Enums.eDamage.Fire,
            Enums.eDamage.Cold,
            Enums.eDamage.Energy,
            Enums.eDamage.Negative,
            Enums.eDamage.Psionic,
            Enums.eDamage.Melee,
            Enums.eDamage.Ranged,
            Enums.eDamage.AoE
        ];

        private static readonly Enums.eDamage[] AllDefenseVectorsExWithToxic =
        [
            Enums.eDamage.None,
            Enums.eDamage.Smashing,
            Enums.eDamage.Lethal,
            Enums.eDamage.Fire,
            Enums.eDamage.Cold,
            Enums.eDamage.Energy,
            Enums.eDamage.Negative,
            Enums.eDamage.Psionic,
            Enums.eDamage.Toxic,
            Enums.eDamage.Melee,
            Enums.eDamage.Ranged,
            Enums.eDamage.AoE
        ];

        private static readonly Enums.eDamage[] AllResistanceVectors =
        [
            Enums.eDamage.Smashing,
            Enums.eDamage.Lethal,
            Enums.eDamage.Fire,
            Enums.eDamage.Cold,
            Enums.eDamage.Energy,
            Enums.eDamage.Negative,
            Enums.eDamage.Psionic,
            Enums.eDamage.Toxic
        ];

        private static readonly Enums.eMez[] AllMezVectors =
        [
            Enums.eMez.Immobilized,
            Enums.eMez.Held,
            Enums.eMez.Stunned,
            Enums.eMez.Sleep,
            Enums.eMez.Terrorized,
            Enums.eMez.Confused
        ];

        private static readonly Enums.eEffectType[] AllMovementVectors =
        [
            Enums.eEffectType.SpeedFlying,
            Enums.eEffectType.SpeedJumping,
            Enums.eEffectType.SpeedRunning
        ];

        private static bool UsesToxicDefenseVectors() => DatabaseAPI.RealmUsesToxicDef();

        private static IReadOnlyList<Enums.eDamage> GetAllDefensesEx()
        {
            return UsesToxicDefenseVectors() ? AllDefenseVectorsExWithToxic : AllDefenseVectorsExWithoutToxic;
        }

        private static IReadOnlyList<Enums.eDamage> GetAllDefenses()
        {
            return UsesToxicDefenseVectors() ? AllDefenseVectorsWithToxic : AllDefenseVectorsWithoutToxic;
        }

        private static IReadOnlyList<Enums.eDamage> GetPositionDefenses()
        {
            return PositionalDefenseVectors;
        }

        private static IReadOnlyList<Enums.eDamage> GetTypedDefenses()
        {
            return UsesToxicDefenseVectors() ? TypedDefenseVectorsWithToxic : TypedDefenseVectorsWithoutToxic;
        }

        private static IReadOnlyList<Enums.eDamage> GetAllResistances()
        {
            return AllResistanceVectors;
        }

        private static IReadOnlyList<Enums.eMez> GetAllMez()
        {
            return AllMezVectors;
        }

        private static IReadOnlyList<Enums.eEffectType> GetAllMovement()
        {
            return AllMovementVectors;
        }

        // ===== Stat naming (kept for tooltip transforms) =====

        private IReadOnlyList<IEffect> GetIncludedEffects(IPower power)
        {
            return IncludedEffects
                .Where(index => index >= 0 && index < power.Effects.Length)
                .Select(index => power.Effects[index])
                .ToArray();
        }

        private string GetCanonicalGroupedVector(IPower power)
        {
            if (IncludedEffects.Count == 0)
            {
                return string.Empty;
            }

            var primaryEffect = power.Effects[IncludedEffects[0]];
            var presentationEffectType = GetPresentationEffectType(primaryEffect);

            return presentationEffectType switch
            {
                Enums.eEffectType.Defense or Enums.eEffectType.Elusivity or Enums.eEffectType.Resistance or Enums.eEffectType.DamageBuff =>
                    ComputeVectors(this, power, presentationEffectType),

                Enums.eEffectType.Mez or Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist =>
                    ComputeVectors(this, power, Enums.eEffectType.Mez),

                Enums.eEffectType.Enhancement when primaryEffect.ETModifies is Enums.eEffectType.Defense
                    or Enums.eEffectType.Elusivity
                    or Enums.eEffectType.Resistance =>
                    ComputeVectors(this, power, primaryEffect.ETModifies),

                Enums.eEffectType.Enhancement when primaryEffect.ETModifies is Enums.eEffectType.Mez
                    or Enums.eEffectType.MezResist =>
                    ComputeVectors(this, power, Enums.eEffectType.Mez),

                _ => string.Empty
            };
        }

        public string GetStatName(IPower power)
        {
            var effects = GetIncludedEffects(power);
            if (effects.Count == 0)
            {
                return string.Empty;
            }

            var groupedVector = GetCanonicalGroupedVector(power);
            if (FxIdentifier.EffectType == Enums.eEffectType.ResEffect)
            {
                var modifies = effects
                    .Select(effect => effect.ETModifies)
                    .Distinct()
                    .ToArray();
                groupedVector = modifies.Length == 1 ? $"{modifies[0]}" : string.Empty;
            }

            if (FxIdentifier.EffectType is Enums.eEffectType.SpeedFlying
                or Enums.eEffectType.SpeedJumping
                or Enums.eEffectType.SpeedRunning)
            {
                var effectTypes = effects
                    .Select(effect => effect.EffectType)
                    .Distinct()
                    .ToList();

                if (effectTypes.ContainsAll(GetAllMovement()))
                {
                    return "Slow";
                }

                var movementLabel = $"{effects[0].EffectType}";
                return string.IsNullOrEmpty(groupedVector) ? movementLabel : $"{movementLabel} ({groupedVector})";
            }

            var label = $"{FxIdentifier.EffectType}";
            return string.IsNullOrEmpty(groupedVector) ? label : $"{label} ({groupedVector})";
        }

        // ===== Vector string builder used for labels =====

        private static string ComputeVectors(GroupedFx gre, IPower pEnh, Enums.eEffectType category, bool preferAll = true)
        {
            var fx = gre.IncludedEffects
                .Where(i => i >= 0 && i < pEnh.Effects.Length)
                .Select(i => pEnh.Effects[i])
                .ToList();

            if (fx.Count == 0)
            {
                return string.Empty;
            }

            if (category == Enums.eEffectType.Mez || category == Enums.eEffectType.MezProtect || category == Enums.eEffectType.MezResist)
            {
                var set = fx.Select(e => e.MezType).Distinct().ToList();
                var all = GetAllMez();
                if (preferAll && set.ContainsAll(all))
                {
                    return "All";
                }

                return string.Join(", ", all.Where(set.Contains));
            }

            if (category == Enums.eEffectType.Defense || category == Enums.eEffectType.Elusivity)
            {
                var set = fx.Select(e => e.DamageType).Distinct().ToList();
                var all = GetAllDefenses();
                var typed = GetTypedDefenses();
                var positions = GetPositionDefenses();

                if (preferAll && set.ContainsAll(all))
                {
                    return "All";
                }

                if (set.ContainsAll(typed) && set.All(d => typed.Contains(d)))
                {
                    return "All types";
                }

                if (set.All(d => positions.Contains(d)))
                {
                    return string.Join(", ", positions.Where(set.Contains));
                }

                return string.Join(", ", all.Where(set.Contains));
            }

            if (category == Enums.eEffectType.Resistance || category == Enums.eEffectType.DamageBuff)
            {
                var set = fx.Select(e => e.DamageType)
                    .Distinct()
                    .Where(d => d is not Enums.eDamage.Melee and not Enums.eDamage.Ranged and not Enums.eDamage.AoE and not Enums.eDamage.None)
                    .ToList();
                var all = GetAllResistances();
                if (preferAll && set.ContainsAll(all))
                {
                    return "All";
                }

                return string.Join(", ", all.Where(set.Contains));
            }

            return string.Empty;
        }

        // ===== Compact list helpers (unchanged, still used in tooltip) =====

        private static List<string> CompactVectorsList(IReadOnlyList<string> vectors, Enums.eEffectType effectType, Enums.eEffectType etModifies)
        {
            if (effectType == Enums.eEffectType.DamageBuff)
            {
                var allDamageBuffVectors = GetAllResistances()
                    .Select(e => $"{e}")
                    .ToList();
                var damageVectors = vectors
                    .Where(v => Enum.TryParse<Enums.eDamage>(v, out var damageType) &&
                                GetAllResistances().Contains(damageType))
                    .Distinct()
                    .ToList();

                if (damageVectors.ContainsAll(allDamageBuffVectors))
                {
                    return ["All"];
                }

                return damageVectors.Count > 0
                    ? damageVectors
                    : vectors.Distinct().ToList();
            }

            var allDefensesEx = GetAllDefensesEx()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var allDefenses = GetAllDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var positionDefenses = GetPositionDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var typedDefenses = GetTypedDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var allElusivity = GetAllDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            var positionElusivity = GetPositionDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            var typedElusivity = GetTypedDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            var allResistances = GetAllResistances()
                .ToDictionary(e => $"{e} Resistance", _ => -1);

            var allMez = GetAllMez()
                .ToDictionary(e => $"{e}", _ => -1);

            var keyNameFound = "";
            for (var i = 0; i < vectors.Count; i++)
            {
                if (allDefensesEx.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    allDefensesEx[keyNameFound] = i;
                }

                if (allDefenses.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    allDefenses[keyNameFound] = i;
                }

                if (positionDefenses.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    positionDefenses[keyNameFound] = i;
                }

                if (typedDefenses.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    typedDefenses[keyNameFound] = i;
                }

                if (allElusivity.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    allElusivity[keyNameFound] = i;
                }

                if (positionElusivity.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    positionElusivity[keyNameFound] = i;
                }

                if (typedElusivity.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    typedElusivity[keyNameFound] = i;
                }

                if (allResistances.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    allResistances[keyNameFound] = i;
                }

                if (allMez.ContainsKey(vectors[i]))
                {
                    allMez[vectors[i]] = i;
                }
            }

            var ignoredVectors = new List<int>();
            var cVectors = new List<string>();

            switch (effectType)
            {
                case Enums.eEffectType.Defense:
                case Enums.eEffectType.Enhancement when etModifies == Enums.eEffectType.Defense:
                    if (allDefensesEx.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Defense (All)");
                        ignoredVectors.AddRangeUnique(allDefensesEx.Values.ToList());
                    }
                    else if (allDefenses.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Defense (All)");
                        ignoredVectors.AddRangeUnique(allDefenses.Values.ToList());
                    }
                    else if (typedDefenses.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Defense (All types)");
                        ignoredVectors.AddRangeUnique(typedDefenses.Values.ToList());
                    }
                    else if (typedDefenses.Count(e => e.Value >= 0) == typedDefenses.Count - 1)
                    {
                        var diff = typedDefenses.Select(e => e.Key).Except(vectors.Select(e => e.EndsWith(" Defense") ? e : $"{e} Defense")).First();
                        cVectors.Add($"Defense (All types but {diff.Replace(" Defense", "")})");
                        ignoredVectors.AddRangeUnique(typedDefenses.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
                    }

                    break;

                case Enums.eEffectType.Elusivity:
                case Enums.eEffectType.Enhancement when etModifies == Enums.eEffectType.Elusivity:
                    if (allElusivity.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Elusivity (All)");
                        ignoredVectors.AddRangeUnique(allElusivity.Values.ToList());
                    }
                    else if (typedElusivity.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Elusivity (All types)");
                        ignoredVectors.AddRangeUnique(typedElusivity.Values.ToList());
                    }
                    else if (typedElusivity.Count(e => e.Value >= 0) == typedElusivity.Count - 1)
                    {
                        var diff = typedElusivity.Select(e => e.Key).Except(vectors.Select(e => e.EndsWith(" Elusivity") ? e : $"{e} Elusivity")).First();
                        cVectors.Add($"Elusivity (All types but {diff.Replace(" Elusivity", "")})");
                        ignoredVectors.AddRangeUnique(typedElusivity.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
                    }

                    break;

                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Enhancement when etModifies == Enums.eEffectType.Resistance:
                    if (allResistances.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Resistance (All)");
                        ignoredVectors.AddRangeUnique(allResistances.Values.ToList());
                    }
                    else if (allResistances.Count(e => e.Value >= 0) == allResistances.Count - 1)
                    {
                        var diff = allResistances.Select(e => e.Key).Except(vectors.Select(e => e.EndsWith(" Resistance") ? e : $"{e} Resistance")).First();
                        cVectors.Add($"Resistance (All but {diff.Replace(" Resistance", "")})");
                        ignoredVectors.AddRangeUnique(allResistances.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
                    }

                    break;

                case Enums.eEffectType.Mez:
                case Enums.eEffectType.MezProtect:
                case Enums.eEffectType.Enhancement when etModifies == Enums.eEffectType.Mez:
                    if (allMez.All(e => e.Value >= 0))
                    {
                        cVectors.Add("All");
                        ignoredVectors.AddRangeUnique(allMez.Values.ToList());
                    }

                    break;
            }

            cVectors.AddRange(vectors.Where((_, i) => !ignoredVectors.Contains(i)));

            return CompactVectorsList(cVectors);
        }

        private static List<string> CompactVectorsList(IReadOnlyList<string> vectors)
        {
            // (kept identical to your previous version except we don't collapse to "All positions" anywhere)
            var allDefensesEx = GetAllDefensesEx().ToDictionary(e => $"{e} Defense", _ => -1);
            var allDefenses = GetAllDefenses().ToDictionary(e => $"{e} Defense", _ => -1);
            var typedDefenses = GetTypedDefenses().ToDictionary(e => $"{e} Defense", _ => -1);
            var allElusivity = GetAllDefenses().ToDictionary(e => $"{e} Elusivity", _ => -1);
            var typedElusivity = GetTypedDefenses().ToDictionary(e => $"{e} Elusivity", _ => -1);
            var allResistances = GetAllResistances().ToDictionary(e => $"{e} Resistance", _ => -1);
            var allMez = GetAllMez().ToDictionary(e => $"{e}", _ => -1);

            var keyNameFound = "";
            for (var i = 0; i < vectors.Count; i++)
            {
                if (allDefensesEx.ContainsKeyPrefix(vectors[i], ref keyNameFound)) allDefensesEx[keyNameFound] = i;
                if (allDefenses.ContainsKeyPrefix(vectors[i], ref keyNameFound)) allDefenses[keyNameFound] = i;
                if (typedDefenses.ContainsKeyPrefix(vectors[i], ref keyNameFound)) typedDefenses[keyNameFound] = i;

                if (allElusivity.ContainsKeyPrefix(vectors[i], ref keyNameFound)) allElusivity[keyNameFound] = i;
                if (typedElusivity.ContainsKeyPrefix(vectors[i], ref keyNameFound)) typedElusivity[keyNameFound] = i;

                if (allResistances.ContainsKeyPrefix(vectors[i], ref keyNameFound)) allResistances[keyNameFound] = i;

                if (allMez.ContainsKey(vectors[i])) allMez[vectors[i]] = i;
            }

            var ignoredVectors = new List<int>();
            var cVectors = new List<string>();

            if (allDefensesEx.All(e => e.Value >= 0))
            {
                cVectors.Add("Defense (All)");
                ignoredVectors.AddRangeUnique(allDefensesEx.Values.ToList());
            }
            else if (allDefenses.All(e => e.Value >= 0))
            {
                cVectors.Add("Defense (All)");
                ignoredVectors.AddRangeUnique(allDefenses.Values.ToList());
            }
            else if (typedDefenses.All(e => e.Value >= 0))
            {
                cVectors.Add("Defense (All types)");
                ignoredVectors.AddRangeUnique(typedDefenses.Values.ToList());
            }

            if (allElusivity.All(e => e.Value >= 0))
            {
                cVectors.Add("Elusivity (All)");
                ignoredVectors.AddRangeUnique(allElusivity.Values.ToList());
            }
            else if (typedElusivity.All(e => e.Value >= 0))
            {
                cVectors.Add("Elusivity (All types)");
                ignoredVectors.AddRangeUnique(typedElusivity.Values.ToList());
            }

            if (allResistances.All(e => e.Value >= 0))
            {
                cVectors.Add("Resistance (All)");
                ignoredVectors.AddRangeUnique(allResistances.Values.ToList());
            }

            if (allMez.All(e => e.Value >= 0))
            {
                cVectors.Add("All");
                ignoredVectors.AddRangeUnique(allMez.Values.ToList());
            }

            cVectors.AddRange(vectors.Where((_, i) => !ignoredVectors.Contains(i)));
            return cVectors;
        }

        private static string InvertStringValue(string value)
        {
            return value.StartsWith('-') ? value[1..] : $"-{value}";
        }

        private static string GetGroupedMezLabel(Enums.eEffectType effectType, Enums.eMez mezType, string vectors)
        {
            var normalizedVectors = MezSemantics.NormalizeMezVectorLabels(vectors);
            return effectType switch
            {
                Enums.eEffectType.MezProtect => string.IsNullOrWhiteSpace(normalizedVectors) ||
                                                MezSemantics.MatchesSingleMezVector(mezType, normalizedVectors)
                    ? MezSemantics.GetStatusProtectionLabel(mezType)
                    : $"Status Protection ({normalizedVectors})",
                Enums.eEffectType.MezResist => string.IsNullOrWhiteSpace(normalizedVectors) ||
                                               MezSemantics.MatchesSingleMezVector(mezType, normalizedVectors)
                    ? MezSemantics.GetStatusResistanceLabel(mezType)
                    : $"Status Resistance ({normalizedVectors})",
                _ => normalizedVectors
            };
        }

        private static string ReplaceMezStatusLabel(string baseEffectString, IEffect effect, string vectors)
        {
            if (effect.EffectType == Enums.eEffectType.Resistance && effect.MezType != Enums.eMez.None)
            {
                return Regex.Replace(
                    baseEffectString,
                    @"(?:Resistance|MezResist)\([^)]*\)|Mez Resistance \([^)]*\)|Status Resistance \([^)]*\)",
                    $"Status Resistance ({MezSemantics.NormalizeMezVectorLabels(vectors)})");
            }

            if (effect.EffectType is Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist)
            {
                var currentLabel = effect.EffectType == Enums.eEffectType.MezProtect
                    ? MezSemantics.GetStatusProtectionLabel(effect.MezType)
                    : MezSemantics.GetStatusResistanceLabel(effect.MezType);
                return baseEffectString.Replace(currentLabel, GetGroupedMezLabel(effect.EffectType, effect.MezType, vectors));
            }

            return baseEffectString.Replace(
                    $"{effect.EffectType}({effect.MezType})",
                    $"{effect.EffectType}({MezSemantics.NormalizeMezVectorLabels(vectors)})")
                .Replace(
                    $"Mez Resistance ({effect.MezType})",
                    $"Status Resistance ({MezSemantics.NormalizeMezVectorLabels(vectors)})");
        }

        // ===== Tooltip =====

        /// <summary>
        /// Renders grouped effect text for popup-style consumers.
        /// Callers can optionally scope the output to a subset of effect
        /// indices, but the grouping and normalization always stay owned by
        /// the canonical presentation pipeline.
        /// </summary>
        internal static string BuildPopupTooltipText(IPower? power, IEnumerable<int>? includedEffects = null, Func<GroupedFx, IEffect, bool>? groupFilter = null)
        {
            return BuildPresentationText(
                power,
                new GroupedFxPresentationRequest(
                    IncludedEffectIds: includedEffects,
                    GroupFilter: groupFilter,
                    SimpleText: true,
                    IgnoreConditions: true,
                    LineJoinMode: GroupedFxLineJoinMode.MultiLine));
        }

        public string GetTooltip(IPower power, bool simple = false, bool ignoreConditions = false)
        {
            var statName = GetStatName(power);
            var groupedVector = GetCanonicalGroupedVector(power);
            var primaryEffect = power.Effects[IncludedEffects[0]];
            var primaryPresentationType = GetPresentationEffectType(primaryEffect);
            var vectors = string.Empty;

            if (!string.IsNullOrEmpty(groupedVector))
            {
                vectors = groupedVector;
            }
            else
            {
                var uniqueVectors = new List<string>();
                var vectorsChunks = primaryPresentationType switch
                {
                    Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning => IncludedEffects
                            .Select(e => $"{power.Effects[e].EffectType}")
                            .ToList(),

                    Enums.eEffectType.Mez or Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist => IncludedEffects
                        .Select(e => $"{power.Effects[e].MezType}")
                        .ToList(),

                    Enums.eEffectType.Enhancement when
                        power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Mez
                            or Enums.eEffectType.MezResist => !string.IsNullOrEmpty(groupedVector)
                            ? [$"{groupedVector}"]
                            : IncludedEffects
                                .Select(e => $"{power.Effects[e].MezType}")
                                .ToList(),

                    Enums.eEffectType.Enhancement =>
                        power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Defense
                            or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity &&
                        !string.IsNullOrEmpty(groupedVector)
                            ? [$"{groupedVector} {power.Effects[IncludedEffects[0]].ETModifies}"]
                            : IncludedEffects
                                .Select(e =>
                                    power.Effects[e].ETModifies is Enums.eEffectType.Defense
                                        or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity
                                        ? $"{power.Effects[e].DamageType} {power.Effects[e].ETModifies}"
                                        : $"{power.Effects[e].ETModifies}")
                                .ToList(),

                    Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity
                        or Enums.eEffectType.DamageBuff => !string.IsNullOrEmpty(groupedVector)
                            ? [$"{power.Effects[IncludedEffects[0]].EffectType}({groupedVector})"]
                            : IncludedEffects
                                .Select(e => $"{power.Effects[e].DamageType}")
                                .ToList(),

                    Enums.eEffectType.ResEffect => IncludedEffects
                        .Select(e => $"{power.Effects[e].ETModifies}")
                        .ToList(),

                    _ => []
                };

                uniqueVectors.AddRangeUnique(vectorsChunks);
                uniqueVectors = CompactVectorsList(uniqueVectors, primaryPresentationType, primaryEffect.ETModifies);
                vectors = string.Join(", ", uniqueVectors);
            }

            var maxRange = IsAggregated && IncludedEffects.Count > 1 && IncludedEffects
                .Select(e => power.Effects[e].BuffedMag)
                .Any(e => e != power.Effects[IncludedEffects[0]].BuffedMag)
                ? IncludedEffects.Count
                : 1;

            var tip = "";
            for (var i = 0; i < maxRange; i++)
            {
                var effect = power.Effects[IncludedEffects[i]];
                var presentationEffectType = GetPresentationEffectType(effect);
                var baseEffectString = effect
                    .BuildEffectString(simple, "", false, false, false, simple, false, true, ignoreConditions);

                var fxTip = presentationEffectType switch
                {
                    Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning =>
                        power.Effects[IncludedEffects[i]].BuffedMag < 0
                            ? statName == "Slow"
                                ? InvertStringValue(Regex.Replace(baseEffectString,
                                    @"(SpeedFlying|SpeedJumping|SpeedRunning)",
                                    "Slow"))
                                : Regex.Replace(baseEffectString, @"(SpeedFlying|SpeedJumping|SpeedRunning)", vectors)
                            : Regex.Replace(baseEffectString, @"(SpeedFlying|SpeedJumping|SpeedRunning)", "Movement Speed"),

                    Enums.eEffectType.Mez or Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist =>
                        ReplaceMezStatusLabel(baseEffectString, effect, vectors),

                    Enums.eEffectType.Enhancement when effect.ETModifies is Enums.eEffectType
                            .Mez
                        or Enums.eEffectType.MezResist => baseEffectString.Replace(
                        $"{effect.EffectType}({effect.MezType})",
                        $"{effect.EffectType}({(vectors == "All" && effect.ETModifies == Enums.eEffectType.Mez ? "Mez" : vectors)})"),

                    Enums.eEffectType.Enhancement when
                        effect.ETModifies is Enums.eEffectType.Defense
                            or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity => baseEffectString.Replace(
                            $"{effect.EffectType}({effect.DamageType} {effect.ETModifies})",
                            $"{effect.EffectType}({vectors}{(power.Effects[IncludedEffects[^1]].ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity && vectors.Contains("All") ? $" {power.Effects[IncludedEffects[^1]].ETModifies}" : "")})"),

                    Enums.eEffectType.Enhancement or Enums.eEffectType.ResEffect => baseEffectString.Replace(
                        $"{effect.EffectType}({effect.ETModifies})",
                        $"{effect.EffectType}({vectors})"),

                    Enums.eEffectType.DamageBuff => baseEffectString.Replace(
                            $"{effect.EffectType}({effect.DamageType})",
                            $"{effect.EffectType} ({vectors})"),

                    Enums.eEffectType.Resistance or Enums.eEffectType.Defense or Enums.eEffectType.Elusivity => baseEffectString.Replace(
                            $"{effect.EffectType}({effect.DamageType})",
                            $"{effect.EffectType}({vectors})"),

                    Enums.eEffectType.SilentKill => baseEffectString.Replace("SilentKill", "Self-Destructs")
                        .Replace(" in ", " after ")
                        .Replace(" to Self", "")
                        .Replace(" to Target", ""),

                    _ => baseEffectString
                };

                tip += $"{(string.IsNullOrEmpty(tip) ? "" : "\r\n")}{fxTip}";
            }

            tip = Regex.Replace(tip, @"(?<stat>[0-9A-Za-z\-]+)\(\k<stat>", "$1")
                .Replace("((", "(")
                .Replace("))", ")")
                .Replace("None Defense", "Base Defense")
                .Replace("None Elusivity", "Base Elusivity");
            return FormatPresentationText(tip);
        }

        // ===== Label / Value =====

        private static string BuildLabel(GroupedFx gre, IPower pEnh)
        {
            // Name pet/power for summon/grant/revoke
            if (gre.EffectType == Enums.eEffectType.EntCreate ||
                gre.EffectType == Enums.eEffectType.GrantPower ||
                gre.EffectType == Enums.eEffectType.RevokePower)
            {
                var effIdx = gre.IncludedEffects.FirstOrDefault(i => i >= 0 && i < pEnh.Effects.Length, -1);
                if (effIdx >= 0)
                {
                    var eff = pEnh.Effects[effIdx];
                    string? name = null;

                    if (eff.nSummon > -1 && eff.nSummon < DatabaseAPI.Database.Entities.Length)
                        name = DatabaseAPI.Database.Entities[eff.nSummon]?.DisplayName?.Trim();

                    if (string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(eff.Summon))
                        name = DatabaseAPI.GetPowerByFullName(eff.Summon)?.DisplayName?.Trim() ?? eff.Summon.Trim();

                    if (!string.IsNullOrEmpty(name)) return name;

                    return gre.EffectType switch
                    {
                        Enums.eEffectType.EntCreate => "Summon",
                        Enums.eEffectType.GrantPower => "Grant",
                        Enums.eEffectType.RevokePower => "Revoke",
                        _ => string.Empty
                    };
                }
            }

            // Strength-style for ETModified (Enhancement & ResEffect)
            if (gre.EffectType == Enums.eEffectType.Enhancement || gre.EffectType == Enums.eEffectType.ResEffect)
            {
                var cat = gre.ETModifies;
                string vec = cat switch
                {
                    Enums.eEffectType.Defense or Enums.eEffectType.Elusivity => ComputeVectors(gre, pEnh, cat),
                    Enums.eEffectType.Resistance => ComputeVectors(gre, pEnh, cat),
                    Enums.eEffectType.Mez or Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist => ComputeVectors(gre, pEnh, Enums.eEffectType.Mez),
                    _ => ""
                };

                var core = $"{cat} Strength";
                return string.IsNullOrEmpty(vec) ? core : $"{core} ({vec})";
            }

            // Mez groups should show vectors like others
            if (gre.EffectType == Enums.eEffectType.Mez)
            {
                var vec = ComputeVectors(gre, pEnh, Enums.eEffectType.Mez);
                return string.IsNullOrEmpty(vec) ? "Mez" : $"Mez ({vec})";
            }

            if (gre.EffectType == Enums.eEffectType.MezProtect)
            {
                var vec = ComputeVectors(gre, pEnh, Enums.eEffectType.Mez);
                return GetGroupedMezLabel(Enums.eEffectType.MezProtect, gre.MezType, vec);
            }

            if (gre.EffectType == Enums.eEffectType.MezResist)
            {
                var vec = ComputeVectors(gre, pEnh, Enums.eEffectType.Mez);
                return GetGroupedMezLabel(Enums.eEffectType.MezResist, gre.MezType, vec);
            }

            // Defense / Resistance / Elusivity labels enumerate vectors when not All
            if (gre.EffectType is Enums.eEffectType.Defense or Enums.eEffectType.Elusivity)
            {
                var vec = ComputeVectors(gre, pEnh, gre.EffectType);
                return string.IsNullOrEmpty(vec) ? $"{gre.EffectType}" : (vec == "All" ? $"{gre.EffectType} (All)" : $"{gre.EffectType} ({vec})");
            }

            if (gre.EffectType == Enums.eEffectType.Resistance)
            {
                var vec = ComputeVectors(gre, pEnh, Enums.eEffectType.Resistance);
                return string.IsNullOrEmpty(vec) ? "Resistance" : (vec == "All" ? "Resistance (All)" : $"Resistance ({vec})");
            }

            // Fallbacks
            var statName = gre.GetStatName(pEnh);
            if (!string.IsNullOrEmpty(statName)) return statName;

            return gre.EffectType switch
            {
                _ => gre.EffectType.ToString()
            };
        }

        private static (string value, string? alt, string? tip, bool isSpecial, bool isConditional, bool isUnique) BuildValueAndDecorations(GroupedFx gre, IPower pBase, IPower pEnh, int effectIndex, bool powerInBuild)
        {
            string tip = string.Empty;
            string value = string.Empty;

            if (gre.EffectType == Enums.eEffectType.EntCreate
                || gre.EffectType == Enums.eEffectType.GrantPower
                || gre.EffectType == Enums.eEffectType.RevokePower)
            {
                var eff = pEnh.Effects[Math.Clamp(effectIndex, 0, pEnh.Effects.Length - 1)];

                var count = Math.Max(1, gre.NumEffects);
                value = $"x{count}";

                tip = BuildEntCreateTooltip(gre, pEnh);

                return (value, null,
                    string.IsNullOrWhiteSpace(tip) ? null : tip,
                    eff.InherentSpecial || eff.InherentSpecial2,
                    eff.VariableModified,
                    false);
            }

            var baseMag = gre.GetMagSum(pBase, gre.EffectType is not (Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight));
            var enhMag = gre.GetMagSum(pEnh, gre.EffectType is not (Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight));

            bool asPercent = gre.EffectType is Enums.eEffectType.Defense or Enums.eEffectType.Resistance
                                                  or Enums.eEffectType.DamageBuff or Enums.eEffectType.ToHit
                                                  or Enums.eEffectType.RechargeTime or Enums.eEffectType.Elusivity
                                                  or Enums.eEffectType.Enhancement or Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist;
            bool asMagnitude = gre.EffectType == Enums.eEffectType.Mez;

            string fmt(float v) => asMagnitude
                ? $"Mag {DisplayValueFormatter.FormatMagnitude(v)}"
                : asPercent
                    ? $"{DisplayValueFormatter.FormatPercentFromScale(v)}%"
                    : DisplayValueFormatter.FormatNumber(v);

            value = $"{fmt(enhMag)}";
            var alt = Math.Abs(baseMag - enhMag) > Tolerance ? fmt(baseMag) : null;

            var tipTitle = BuildLabel(gre, pEnh);
            tip = $"{tipTitle}  Base: {fmt(baseMag)}  Enhanced: {fmt(enhMag)}";

            var isSpecial = gre.HasConditions;
            var isConditional = false;
            var isUnique = gre.EnhancementEffect;

            return (value, alt, tip, isSpecial, isConditional, isUnique);
        }

        private static string BuildEntCreateTooltip(GroupedFx gre, IPower pEnh)
        {
            var effects = gre.IncludedEffects
                .Where(i => i >= 0 && i < pEnh.Effects.Length)
                .Select(i => pEnh.Effects[i])
                .OrderBy(e => e.DelayedTime)
                .ToList();

            if (effects.Count == 0) return "Summons";

            var first = effects[0];

            string name = first.SummonedEntityName;
            if (string.IsNullOrWhiteSpace(name))
            {
                if (first.nSummon > -1 && first.nSummon < DatabaseAPI.Database.Entities.Length)
                    name = DatabaseAPI.Database.Entities[first.nSummon]?.DisplayName?.Trim();
                if (string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(first.Summon))
                    name = DatabaseAPI.GetPowerByFullName(first.Summon)?.DisplayName?.Trim() ?? first.Summon.Trim();
                if (string.IsNullOrWhiteSpace(name)) name = "Pet";
            }

            int count = effects.Count;

            bool isVariable = effects.Any(e =>
            {
                var pow = e.GetPower();
                return pow != null && pow.VariableEnabled && (e.VariableModified || e.ToWho == Enums.eToWho.Self) && !e.IgnoreScaling;
            });

            var header = new System.Text.StringBuilder();
            header.Append("Summons ").Append(name);
            if (count > 1) header.Append(" x").Append(count);
            if (isVariable) header.Append(" (Variable)");

            static string Ordinal(int n)
            {
                int m = n % 100;
                if (m is 11 or 12 or 13) return n + "th";
                return (n % 10) switch { 1 => n + "st", 2 => n + "nd", 3 => n + "rd", _ => n + "th" };
            }
            static string When(IEffect e)
            {
                if (e.DelayedTime <= 0) return "immediately";
                var s = e.DelayedTime.ToString("0.#");
                return $"after {s} second" + (Math.Abs(e.DelayedTime - 1.0) < 1e-9 ? "" : "s");
            }

            bool allImmediate = effects.All(e => e.DelayedTime <= 0);
            var body = new System.Text.StringBuilder();

            if (!(count > 1 && allImmediate))
            {
                for (int i = 0; i < effects.Count; i++)
                {
                    var when = When(effects[i]);
                    if (count > 1)
                        body.Append("- ").Append(Ordinal(i + 1)).Append(' ').Append(name).Append(' ').Append(when).AppendLine();
                    else
                        body.Append("- 1 ").Append(name).Append(' ').Append(when).AppendLine();
                }
            }

            bool ignoresBuffs = effects.Any(e => !e.Buffable);
            bool ignoresED = effects.Any(e => e.IgnoreED);
            bool noStackSameCaster = effects.Any(PlannerStackRules.ShouldFlagNoStackSameCaster);

            var flags = new System.Text.StringBuilder();
            if (ignoresBuffs) flags.AppendLine("[Ignores Enhancements & Buffs]");
            if (ignoresED) flags.AppendLine("[Not Affected By ED]");
            if (noStackSameCaster) flags.AppendLine("[Does Not Stack From Same Caster]");

            var tip = new System.Text.StringBuilder();
            tip.AppendLine(header.ToString());
            tip.Append(body);
            if (flags.Length > 0) tip.Append(flags);

            return tip.ToString().TrimEnd();
        }

        // ===== Magnitude =====

        public EnhancedMagSum GetMagSum(IPower pBase, IPower pEnh)
        {
            static float Sum(IReadOnlyList<float> vals)
            {
                if (vals.Count == 0) return 0f;
                bool allNeg = vals.All(v => v < 0);
                return allNeg ? vals.Sum() : vals.Where(v => v > 0).Sum();
            }

            var baseVals = IncludedEffects
                .Where(e => e >= 0 && e < pBase.Effects.Length)
                .Select(e => pBase.Effects[e].BuffedMag)
                .ToList();

            var enhVals = IncludedEffects
                .Where(e => e >= 0 && e < pEnh.Effects.Length)
                .Select(e => pEnh.Effects[e].BuffedMag)
                .ToList();

            return new EnhancedMagSum
            {
                Base = Sum(baseVals),
                Enhanced = Sum(enhVals)
            };
        }

        public float GetMagSum(IPower power, bool ignoreNegs = true)
        {
            if (power?.Effects == null || IncludedEffects.Count <= 0)
            {
                return 0;
            }

            var mags = IncludedEffects
                .Where(e => e >= 0 && e < power.Effects.Length)
                .Select(e => power.Effects[e].BuffedMag)
                .ToList();

            if (mags.Count == 0)
            {
                return 0;
            }

            var firstInBounds = IncludedEffects.FirstOrDefault(e => e >= 0 && e < power.Effects.Length, -1);
            if (firstInBounds >= 0)
            {
                var eff = power.Effects[firstInBounds];
                if (eff.EffectType is Enums.eEffectType.Defense
                    or Enums.eEffectType.Resistance
                    or Enums.eEffectType.DamageBuff
                    or Enums.eEffectType.Elusivity
                    or Enums.eEffectType.Mez
                    or Enums.eEffectType.MezProtect
                    or Enums.eEffectType.MezResist
                    or Enums.eEffectType.ResEffect
                    or Enums.eEffectType.Enhancement)
                {
                    return eff.BuffedMag;
                }
            }

            var allNeg = mags.All(v => v < 0);
            if (HasMisc)
            {
                return allNeg ? mags.Min() : mags.Max();
            }
            
            if (allNeg)
            {
                return mags.Sum();
            }

            return ignoreNegs
                ? mags.Where(v => v > 0).Sum()
                : mags.Sum();
        }

        // ===== Grouping / assembly =====

        private static IEnumerable<KeyValuePair<int, IEffect>> EnumerateIndexedEffects(IPower power)
        {
            return power.Effects.Select((effect, index) => new KeyValuePair<int, IEffect>(index, effect));
        }

        private static List<int> FindEffectIndices(IPower power, Func<IEffect, bool> predicate)
        {
            return EnumerateIndexedEffects(power)
                .Where(entry => predicate(entry.Value))
                .Select(entry => entry.Key)
                .ToList();
        }

        public static List<int> GetSimilarEffects(IPower power, FxId fxIdentifier, float mag,
            bool isDefianceGroup = false, bool enhancementEffect = false, int seedIndex = -1)
        {
            var candidates = fxIdentifier.EffectType switch
            {
                Enums.eEffectType.EntCreate => FindEffectIndices(power, effect =>
                    GetPresentationEffectType(effect) == fxIdentifier.EffectType &&
                    effect.nSummon == fxIdentifier.SummonId &&
                    effect.ToWho == fxIdentifier.ToWho &&
                    (Math.Abs(effect.Duration - fxIdentifier.Duration) < Tolerance || fxIdentifier.Duration == 0) &&
                    effect.isEnhancementEffect == enhancementEffect &&
                    effect.PvMode == fxIdentifier.PvMode &&
                    effect.IgnoreScaling == fxIdentifier.IgnoreScaling),

                Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning => FindEffectIndices(power, effect =>
                    effect.EffectType is Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning &&
                    effect.ToWho == fxIdentifier.ToWho &&
                    MagnitudesMatch(effect.BuffedMag, mag) &&
                    effect.isEnhancementEffect == enhancementEffect &&
                    effect.PvMode == fxIdentifier.PvMode &&
                    effect.IgnoreScaling == fxIdentifier.IgnoreScaling),

                Enums.eEffectType.Enhancement when fxIdentifier.ETModifies is Enums.eEffectType.Mez
                    or Enums.eEffectType.MezResist => FindEffectIndices(power, effect =>
                    GetPresentationEffectType(effect) == fxIdentifier.EffectType &&
                    effect.ETModifies == fxIdentifier.ETModifies &&
                    effect.ToWho == fxIdentifier.ToWho &&
                    MagnitudesMatch(effect.BuffedMag, mag) &&
                    effect.isEnhancementEffect == enhancementEffect &&
                    effect.PvMode == fxIdentifier.PvMode &&
                    effect.IgnoreScaling == fxIdentifier.IgnoreScaling),

                Enums.eEffectType.Enhancement when fxIdentifier.ETModifies is Enums.eEffectType.Defense
                    or Enums.eEffectType.Resistance
                    or Enums.eEffectType.Elusivity => FindEffectIndices(power, effect =>
                    GetPresentationEffectType(effect) == fxIdentifier.EffectType &&
                    effect.ETModifies == fxIdentifier.ETModifies &&
                    effect.ToWho == fxIdentifier.ToWho &&
                    MagnitudesMatch(effect.BuffedMag, mag) &&
                    effect.isEnhancementEffect == enhancementEffect &&
                    effect.PvMode == fxIdentifier.PvMode &&
                    effect.IgnoreScaling == fxIdentifier.IgnoreScaling),

                Enums.eEffectType.Enhancement => FindEffectIndices(power, effect =>
                    GetPresentationEffectType(effect) == fxIdentifier.EffectType &&
                    effect.ETModifies == fxIdentifier.ETModifies &&
                    effect.ToWho == fxIdentifier.ToWho &&
                    MagnitudesMatch(effect.BuffedMag, mag) &&
                    effect.isEnhancementEffect == enhancementEffect &&
                    effect.PvMode == fxIdentifier.PvMode &&
                    effect.IgnoreScaling == fxIdentifier.IgnoreScaling),

                Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist or Enums.eEffectType.Defense or Enums.eEffectType.Resistance
                    or Enums.eEffectType.Elusivity or Enums.eEffectType.ResEffect => FindEffectIndices(power, effect =>
                    GetPresentationEffectType(effect) == fxIdentifier.EffectType &&
                    effect.ToWho == fxIdentifier.ToWho &&
                    MagnitudesMatch(effect.BuffedMag, mag) &&
                    effect.isEnhancementEffect == enhancementEffect &&
                    effect.PvMode == fxIdentifier.PvMode &&
                    effect.IgnoreScaling == fxIdentifier.IgnoreScaling),

                Enums.eEffectType.DamageBuff when isDefianceGroup => FindEffectIndices(power, effect =>
                    effect.EffectType == fxIdentifier.EffectType &&
                    effect.ToWho == fxIdentifier.ToWho &&
                    MagnitudesMatch(effect.BuffedMag, mag) &&
                    IsModernDefianceDisplayEffect(effect) &&
                    effect.isEnhancementEffect == enhancementEffect &&
                    effect.PvMode == fxIdentifier.PvMode &&
                    effect.IgnoreScaling == fxIdentifier.IgnoreScaling),

                Enums.eEffectType.DamageBuff => FindEffectIndices(power, effect =>
                    GetPresentationEffectType(effect) == fxIdentifier.EffectType &&
                    effect.ToWho == fxIdentifier.ToWho &&
                    MagnitudesMatch(effect.BuffedMag, mag) &&
                    !IsModernDefianceDisplayEffect(effect) &&
                    effect.isEnhancementEffect == enhancementEffect &&
                    effect.PvMode == fxIdentifier.PvMode &&
                    effect.IgnoreScaling == fxIdentifier.IgnoreScaling),

                Enums.eEffectType.Damage => FindEffectIndices(power, effect =>
                    GetPresentationEffectType(effect) == fxIdentifier.EffectType &&
                    effect.ToWho == fxIdentifier.ToWho &&
                    MagnitudesMatch(effect.BuffedMag, mag) &&
                    effect.DamageType == fxIdentifier.DamageType &&
                    effect.isEnhancementEffect == enhancementEffect &&
                    effect.PvMode == fxIdentifier.PvMode &&
                    effect.IgnoreScaling == fxIdentifier.IgnoreScaling),

                _ => []
            };

            return candidates
                .Where(index => IsEffectEligibleForGrouping(power.Effects[index]) &&
                                HasSameConditionIdentity(power, seedIndex, index))
                .ToList();
        }

        private static bool IsEffectEligibleForGrouping(IEffect effect)
        {
            return effect.Probability > 0 &&
                   (MidsContext.Config?.Suppression & effect.Suppression) == Enums.eSuppress.None &&
                   (effect.CanInclude() || IsModernDefianceDisplayEffect(effect)) &&
                   effect.PvXInclude() &&
                   effect.EffectClass != Enums.eEffectClass.Ignored;
        }

        private static bool IsModernDefianceDisplayEffect(IEffect effect)
        {
            return DefiancePlanner.IsModernContributorEffect(effect);
        }

        private static bool HasSameConditionIdentity(IPower power, int seedIndex, int candidateIndex)
        {
            if (seedIndex < 0 ||
                seedIndex >= power.Effects.Length ||
                candidateIndex < 0 ||
                candidateIndex >= power.Effects.Length)
            {
                return true;
            }

            return string.Equals(
                GetConditionIdentity(power.Effects[seedIndex]),
                GetConditionIdentity(power.Effects[candidateIndex]),
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasSameGroupConditionIdentity(IPower power, GroupedFx left, GroupedFx right)
        {
            var leftIndex = left.IncludedEffects.FirstOrDefault(index => index >= 0 && index < power.Effects.Length, -1);
            var rightIndex = right.IncludedEffects.FirstOrDefault(index => index >= 0 && index < power.Effects.Length, -1);
            if (leftIndex < 0 || rightIndex < 0)
            {
                return true;
            }

            return string.Equals(
                GetConditionIdentity(power.Effects[leftIndex]),
                GetConditionIdentity(power.Effects[rightIndex]),
                StringComparison.OrdinalIgnoreCase);
        }

        private static string GetConditionIdentity(IEffect effect)
        {
            return effect.ConditionIdentity ?? string.Empty;
        }

        private sealed record GroupAssemblySeed(
            FxId Identifier,
            float Magnitude,
            string Alias,
            bool GroupEnhancementEffect,
            bool SimilarityEnhancementEffect,
            string ConditionIdentity,
            bool IsDefianceGroup);

        public static List<GroupedFx> AssembleGroupedEffects(IPower? power, bool includeDamage = false)
        {
            if (power == null)
            {
                return [];
            }

            var rankedEffects = power.GetRankedEffects(true);
            var initialGroups = BuildInitialGroupedEffects(power, rankedEffects, includeDamage);
            var normalizedGroups = NormalizeSingleEffectGroups(power, initialGroups);
            return AggregateAndFinalizeGroupedEffects(power, normalizedGroups);
        }

        private static List<GroupedFx> BuildInitialGroupedEffects(IPower power, IReadOnlyList<int> rankedEffects, bool includeDamage)
        {
            var ignoredEffects = new HashSet<int>();
            var groupedEffects = new List<GroupedFx>();

            foreach (var effectIndex in rankedEffects)
            {
                if (effectIndex < 0 || ignoredEffects.Contains(effectIndex))
                {
                    continue;
                }

                if (ShouldSkipEffectDuringAssembly(power, effectIndex, includeDamage))
                {
                    continue;
                }

                var seed = BuildGroupingSeed(power, effectIndex);
                if (seed == null)
                {
                    groupedEffects.Add(new GroupedFx(power.Effects[effectIndex], effectIndex));
                    continue;
                }

                var similarEffectIds = GetSimilarEffects(
                    power,
                    seed.Identifier,
                    seed.Magnitude,
                    seed.IsDefianceGroup,
                    seed.SimilarityEnhancementEffect,
                    effectIndex);
                if (similarEffectIds.Count == 0)
                {
                    continue;
                }

                ignoredEffects.UnionWith(similarEffectIds);
                groupedEffects.Add(new GroupedFx(
                    seed.Identifier,
                    seed.Magnitude,
                    seed.Alias,
                    similarEffectIds,
                    seed.GroupEnhancementEffect,
                    seed.ConditionIdentity,
                    seed.IsDefianceGroup));
            }

            return groupedEffects;
        }

        private static bool ShouldSkipEffectDuringAssembly(IPower power, int effectIndex, bool includeDamage)
        {
            if (effectIndex < 0 || effectIndex >= power.Effects.Length)
            {
                return true;
            }

            var effect = power.Effects[effectIndex];
            if (!includeDamage && effect.EffectType == Enums.eEffectType.Damage)
            {
                return true;
            }

            if (effect.EffectType is Enums.eEffectType.Meter
                or Enums.eEffectType.SetMode
                or Enums.eEffectType.UnsetMode
                or Enums.eEffectType.Null
                or Enums.eEffectType.NullBool
                or Enums.eEffectType.GlobalChanceMod
                or Enums.eEffectType.ExecutePower)
            {
                return true;
            }

            if (ShouldHideStatefulMezFromGroupedViews(effect))
            {
                return true;
            }

            if (effect.EffectType == Enums.eEffectType.ResEffect &&
                effect.ETModifies is Enums.eEffectType.Null or Enums.eEffectType.NullBool)
            {
                return true;
            }

            var isDefianceDisplayEffect = IsModernDefianceDisplayEffect(effect);
            if (!(effect.Probability > 0 &&
                  (MidsContext.Config?.Suppression & effect.Suppression) == Enums.eSuppress.None &&
                  (effect.CanInclude() || isDefianceDisplayEffect)))
            {
                return true;
            }

            if (effect.EffectType == Enums.eEffectType.RevokePower &&
                effect.nSummon <= -1 &&
                string.IsNullOrWhiteSpace(effect.Summon))
            {
                return true;
            }

            if (effect.EffectType == Enums.eEffectType.GrantPower &&
                effect.nSummon <= -1)
            {
                return true;
            }

            if ((effect.PvMode == Enums.ePvX.PvP && !MidsContext.Config.Inc.DisablePvE) ||
                (effect.PvMode == Enums.ePvX.PvE && MidsContext.Config.Inc.DisablePvE))
            {
                return true;
            }

            if (effect.HasConditions &&
                !isDefianceDisplayEffect &&
                !effect.ValidateConditional())
            {
                return true;
            }

            if (effect.EffectType == Enums.eEffectType.Mez &&
                effect.MezType is not (Enums.eMez.Teleport or Enums.eMez.Knockback or Enums.eMez.Knockup or Enums.eMez.Repel or Enums.eMez.ToggleDrop) &&
                effect.Duration <= 0)
            {
                return true;
            }

            return false;
        }

        private static GroupAssemblySeed? BuildGroupingSeed(IPower power, int effectIndex)
        {
            var effect = power.Effects[effectIndex];
            var presentationEffectType = GetPresentationEffectType(effect);

            return presentationEffectType switch
            {
                Enums.eEffectType.Damage => new GroupAssemblySeed(
                    CreateGroupedFxIdentifier(effect, Enums.eEffectType.Damage, damageType: effect.DamageType, duration: effect.Duration),
                    effect.BuffedMag,
                    "Damage",
                    effect.isEnhancementEffect,
                    effect.isEnhancementEffect,
                    effect.ConditionIdentity,
                    false),

                Enums.eEffectType.EntCreate => new GroupAssemblySeed(
                    CreateGroupedFxIdentifier(effect, Enums.eEffectType.EntCreate, summonId: effect.nSummon),
                    effect.BuffedMag,
                    "Summon",
                    effect.isEnhancementEffect,
                    effect.isEnhancementEffect,
                    effect.ConditionIdentity,
                    false),

                Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning => new GroupAssemblySeed(
                    CreateGroupedFxIdentifier(effect, effect.EffectType),
                    effect.BuffedMag,
                    "Slow",
                    effect.isEnhancementEffect,
                    effect.isEnhancementEffect,
                    effect.ConditionIdentity,
                    false),

                Enums.eEffectType.DamageBuff => BuildDamageBuffGroupingSeed(effect),

                Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity
                    or Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist or Enums.eEffectType.ResEffect or Enums.eEffectType.Enhancement => new GroupAssemblySeed(
                    CreateGroupedFxIdentifier(effect, presentationEffectType, etModifies: effect.ETModifies),
                    effect.BuffedMag,
                    presentationEffectType == Enums.eEffectType.Enhancement
                        ? $"{presentationEffectType}({effect.ETModifies})"
                        : $"{presentationEffectType}",
                    effect.isEnhancementEffect,
                    effect.isEnhancementEffect,
                    effect.ConditionIdentity,
                    false),

                _ => null
            };
        }

        private static GroupAssemblySeed BuildDamageBuffGroupingSeed(IEffect effect)
        {
            var isDefiance = IsModernDefianceDisplayEffect(effect);

            return new GroupAssemblySeed(
                CreateGroupedFxIdentifier(effect, Enums.eEffectType.DamageBuff),
                effect.BuffedMag,
                isDefiance ? "Defiance" : $"{effect.EffectType}",
                !isDefiance && effect.isEnhancementEffect,
                effect.isEnhancementEffect,
                effect.ConditionIdentity,
                isDefiance);
        }

        private static FxId CreateGroupedFxIdentifier(
            IEffect effect,
            Enums.eEffectType effectType,
            Enums.eEffectType etModifies = Enums.eEffectType.None,
            Enums.eDamage damageType = Enums.eDamage.None,
            Enums.eMez mezType = Enums.eMez.None,
            int summonId = -1,
            float duration = 0f)
        {
            return new FxId
            {
                DamageType = damageType,
                EffectType = effectType,
                ETModifies = etModifies,
                MezType = mezType,
                ToWho = effect.ToWho,
                SummonId = summonId,
                Duration = duration,
                PvMode = effect.PvMode,
                IgnoreScaling = effect.IgnoreScaling
            };
        }

        private static List<GroupedFx> NormalizeSingleEffectGroups(IPower power, IReadOnlyList<GroupedFx> groupedEffects)
        {
            var normalizedGroups = new List<GroupedFx>(groupedEffects.Count);
            var ignoredGroupIndexes = new HashSet<int>();

            for (var index = 0; index < groupedEffects.Count; index++)
            {
                if (ignoredGroupIndexes.Contains(index))
                {
                    continue;
                }

                var groupedEffect = groupedEffects[index];
                if (groupedEffect.NumEffects > 1)
                {
                    normalizedGroups.Add(groupedEffect);
                    continue;
                }

                var similarGroups = groupedEffects
                    .Select((group, groupIndex) => new KeyValuePair<int, GroupedFx>(groupIndex, group))
                    .Where(entry => entry.Value.FxIdentifier.Equals(groupedEffect.FxIdentifier) &&
                                    MagnitudesMatch(entry.Value.Mag, groupedEffect.Mag) &&
                                    entry.Value.EnhancementEffect == groupedEffect.EnhancementEffect &&
                                    entry.Value.DefianceTagged == groupedEffect.DefianceTagged &&
                                    string.Equals(entry.Value.ConditionIdentity, groupedEffect.ConditionIdentity, StringComparison.OrdinalIgnoreCase) &&
                                    HasSameGroupConditionIdentity(power, groupedEffect, entry.Value))
                    .ToList();

                ignoredGroupIndexes.UnionWith(similarGroups.Select(entry => entry.Key));
                normalizedGroups.Add(new GroupedFx(
                    groupedEffect.FxIdentifier,
                    similarGroups.Select(entry => entry.Value).ToList()));
            }

            return normalizedGroups;
        }

        private static List<GroupedFx> AggregateAndFinalizeGroupedEffects(IPower power, List<GroupedFx> groupedEffects)
        {
            var aggregatedGroups = Aggregate(groupedEffects);
            foreach (var groupedEffect in aggregatedGroups)
            {
                if (!groupedEffect.IsAggregated)
                {
                    continue;
                }

                groupedEffect.Mag = groupedEffect.GetMagSum(
                    power,
                    groupedEffect.FxIdentifier.EffectType is not (Enums.eEffectType.SpeedFlying
                        or Enums.eEffectType.SpeedJumping
                        or Enums.eEffectType.SpeedRunning
                        or Enums.eEffectType.JumpHeight));
            }

            return aggregatedGroups
                .Where(groupedEffect => Math.Abs(groupedEffect.Mag) > Tolerance)
                .ToList();
        }

        private static bool ShouldHideStatefulMezFromGroupedViews(IEffect effect)
        {
            return effect is
            {
                EffectType: Enums.eEffectType.Mez,
                MezType: Enums.eMez.OnlyAffectsSelf or Enums.eMez.Untouchable or Enums.eMez.Intangible
            };
        }

        public static List<GroupedFx> AssembleGroupedEffects(IPower? power, IEnumerable<int> includedEffects, bool includeDamage = false)
        {
            if (power == null)
            {
                return [];
            }

            var effectIndexMap = NormalizeIncludedEffectIndices(power, includedEffects);
            if (effectIndexMap.Count == 0)
            {
                return [];
            }

            var subsetPower = BuildEffectSubsetPower(power, effectIndexMap);
            var groupedEffects = AssembleGroupedEffects(subsetPower, includeDamage);
            RemapGroupedEffectIndices(groupedEffects, effectIndexMap);
            return groupedEffects;
        }

        private static List<int> NormalizeIncludedEffectIndices(IPower power, IEnumerable<int> includedEffects)
        {
            return includedEffects
                .Where(index => index >= 0 && index < power.Effects.Length)
                .Distinct()
                .OrderBy(index => index)
                .ToList();
        }

        private static IPower BuildEffectSubsetPower(IPower power, IReadOnlyList<int> effectIndexMap)
        {
            var subsetPower = new Power(power);
            subsetPower.Effects = effectIndexMap
                .Select(index => subsetPower.Effects[index])
                .ToArray();

            foreach (var effect in subsetPower.Effects)
            {
                effect.SetPower(subsetPower);
            }

            return subsetPower;
        }

        private static void RemapGroupedEffectIndices(IEnumerable<GroupedFx> groupedEffects, IReadOnlyList<int> effectIndexMap)
        {
            foreach (var groupedEffect in groupedEffects)
            {
                groupedEffect.IncludedEffects = groupedEffect.IncludedEffects
                    .Where(index => index >= 0 && index < effectIndexMap.Count)
                    .Select(index => effectIndexMap[index])
                    .ToList();
            }
        }

        public static List<GroupedFx> AggregateGroupedEffectsPass2(IPower? power, List<GroupedFx> gre)
        {
            if (power == null)
            {
                return [];
            }

            var ret = new Dictionary<EnhanceableFxId, GroupedFx>();

            foreach (var g in gre)
            {
                var fxId = new EnhanceableFxId
                {
                    EffectType = g.EffectType,
                    ETModifies = g.ETModifies == Enums.eEffectType.None ? null : g.ETModifies,
                    MezType = g.MezType == Enums.eMez.None ? null : g.MezType,
                    PvMode = g.PvMode,
                    ToWho = g.ToWho
                };

                var v = ret.TryAdd(fxId, new GroupedFx
                {
                    FxIdentifier = new FxId
                    {
                        DamageType = Enums.eDamage.None,
                        Duration = 0,
                        EffectType = g.EffectType,
                        ETModifies = g.ETModifies,
                        IgnoreScaling = g.IgnoreScaling,
                        MezType = g.MezType,
                        PvMode = g.PvMode,
                        SummonId = 0,
                        ToWho = g.ToWho
                    },
                    Alias = $"{(g.EffectType == Enums.eEffectType.Enhancement ? $"Enhancement({(g.ETModifies == Enums.eEffectType.Mez ? g.MezType : g.ETModifies)})" : g.EffectType)}",
                    HasMisc = true,
                    IncludedEffects = g.IncludedEffects.Clone(),
                    IsAggregated = true,
                    IsEnhancement = g.IsEnhancement,
                    IsDefianceGroup = g.DefianceTagged,
                    Mag = 0,
                    SingleEffectSource = null,
                    ConditionIdentity = string.Empty
                });

                if (!v)
                {
                    ret[fxId].IncludedEffects = ret[fxId].IncludedEffects.Concat(g.IncludedEffects).Distinct().ToList();
                    ret[fxId].IsDefianceGroup |= g.DefianceTagged;
                }
            }

            var gList = ret.Values.ToList();
            foreach (var g in gList)
            {
                // Sort descending by mag
                g.IncludedEffects.Sort((a, b) =>
                {
                    var fx1 = power.Effects[a];
                    var fx2 = power.Effects[b];

                    var magCompare = fx2.BuffedMag.CompareTo(fx1.BuffedMag);
                    return magCompare != 0 ? magCompare : a.CompareTo(b);
                });
            }

            return gList;
        }

        public static List<KeyValuePair<GroupedFx, PairedListEx.Item>> GenerateListItems(List<GroupedFx> groupedRankedEffects, IPower pBase, IPower pEnh, List<int> rankedEffects, float displayBlockFontSize)
        {
            var ret = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>();
            if (pBase == null || pEnh == null || rankedEffects == null)
            {
                return ret;
            }

            var powerInBuild = MidsContext.Character?.CurrentBuild != null &&
                               MidsContext.Character.CurrentBuild.FindInToonHistory(
                                   DatabaseAPI.Database.Power.TryFindIndex(e => e?.FullName == pBase.FullName)) > -1;

            foreach (var gre in groupedRankedEffects)
            {
                var greIndex = gre.GetRankedEffectIndex(rankedEffects, 0);
                if (greIndex < 0) continue;

                var rankedEffect = FastItemBuilder.GetRankedEffect(rankedEffects.ToArray(), greIndex, pBase, pEnh);
                FinalizeListItem(ref rankedEffect, pBase, pEnh, gre, rankedEffects[greIndex], powerInBuild, displayBlockFontSize);

                ret.Add(new KeyValuePair<GroupedFx, PairedListEx.Item>(gre, rankedEffect));
            }

            return ret;
        }

        public static List<KeyValuePair<GroupedFx, EffectListItem>> GenerateEffectItems(List<GroupedFx> groupedRankedEffects, IPower pBase, IPower pEnh, List<int> rankedEffects)
        {
            var ret = new List<KeyValuePair<GroupedFx, EffectListItem>>();
            if (pBase == null || pEnh == null || rankedEffects == null)
            {
                return ret;
            }

            var powerInBuild = MidsContext.Character?.CurrentBuild != null &&
                               MidsContext.Character.CurrentBuild
                                   .FindInToonHistory(DatabaseAPI.Database.Power.TryFindIndex(e => e?.FullName == pBase.FullName)) > -1;

            foreach (var gre in groupedRankedEffects)
            {
                var greIndex = gre.GetRankedEffectIndex(rankedEffects, 0);
                if (greIndex < 0) continue;

                var label = FormatPresentationText(BuildLabel(gre, pEnh));
                var (value, alt, tip, isSpecial, isConditional, isUnique) =
                    BuildValueAndDecorations(gre, pBase, pEnh, rankedEffects[greIndex], powerInBuild);

                var item = new EffectListItem(
                    label,
                    value,
                    alt,
                    string.IsNullOrWhiteSpace(tip) ? tip : FormatPresentationText(tip),
                    isSpecial,
                    isConditional,
                    isUnique,
                    gre.EnhancementEffect);

                ret.Add(new KeyValuePair<GroupedFx, EffectListItem>(gre, item));
            }

            return ret;
        }

        public static List<KeyValuePair<GroupedFx, EffectListItem>> FilterEffectItemsExt(List<KeyValuePair<GroupedFx, EffectListItem>>? itemsDict, Func<FxId, bool> filter)
        {
            if (itemsDict == null) return new List<KeyValuePair<GroupedFx, EffectListItem>>();
            return itemsDict.Where(e => filter(e.Key.FxIdentifier)).ToList();
        }

        public static List<PairedListEx.Item> FilterListItems(List<KeyValuePair<GroupedFx, PairedListEx.Item>> itemsDict, Func<FxId, bool> filterFunc)
        {
            if (itemsDict == null)
            {
                return [];
            }

            return itemsDict
                .Where(e => filterFunc(e.Key.FxIdentifier))
                .Select(e => e.Value)
                .ToList();
        }

        public static List<KeyValuePair<GroupedFx, PairedListEx.Item>> FilterListItemsExt(List<KeyValuePair<GroupedFx, PairedListEx.Item>>? itemsDict, Func<FxId, bool> filterFunc)
        {
            if (itemsDict == null)
            {
                return new List<KeyValuePair<GroupedFx, PairedListEx.Item>>();
            }

            return itemsDict
                .Where(e => filterFunc(e.Key.FxIdentifier))
                .ToList();
        }

        public static List<PairedListEx.Item> FilterListItems(List<KeyValuePair<GroupedFx, PairedListEx.Item>>? itemsDict)
        {
            if (itemsDict == null)
            {
                return [];
            }

            return itemsDict
                .Select(e => e.Value)
                .ToList();
        }

        public static List<KeyValuePair<GroupedFx, PairedListEx.Item>> FilterListItemsExt(List<KeyValuePair<GroupedFx, PairedListEx.Item>> itemsDict)
        {
            return itemsDict ?? [];
        }

        private static string GeneratePowerDescShort(IPower? power)
        {
            var effectShorts = new List<string>();
            var fxIdList = new List<FxId>();
            var effects = (IEffect[])power.Effects.Clone();
            effects = effects.OrderBy(e => e.ToWho)
                .Where(e => e.EffectType is not (Enums.eEffectType.Null or Enums.eEffectType.NullBool
                                or Enums.eEffectType.Meter or Enums.eEffectType.Damage or Enums.eEffectType.MaxFlySpeed
                                or Enums.eEffectType.MaxJumpSpeed or Enums.eEffectType.MaxRunSpeed
                                or Enums.eEffectType.ExecutePower or Enums.eEffectType.RevokePower
                                or Enums.eEffectType.GlobalChanceMod or Enums.eEffectType.SetMode
                                or Enums.eEffectType.SetCostume) &&
                            e.ETModifies is not (Enums.eEffectType.Null or Enums.eEffectType.NullBool) &&
                            e.ToWho != Enums.eToWho.Unspecified &&
                            Math.Abs(e.BuffedMag) >= Tolerance &&
                            e.PvXInclude() &&
                            e.CanInclude())
                .ToArray();

            for (var i = 0; i < effects.Length; i++)
            {
                var fxIdentifier = effects[i].EffectType switch
                {
                    Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist or Enums.eEffectType.Mez => new FxId
                    {
                        EffectType = effects[i].EffectType,
                        DamageType = Enums.eDamage.None,
                        MezType = effects[i].MezType,
                        ETModifies = Enums.eEffectType.None,
                        ToWho = effects[i].ToWho,
                        Duration = 0,
                        SummonId = -1
                    },

                    Enums.eEffectType.ResEffect or Enums.eEffectType.Enhancement => new FxId
                    {
                        EffectType = effects[i].EffectType,
                        DamageType = Enums.eDamage.None,
                        MezType = Enums.eMez.None,
                        ETModifies = effects[i].ETModifies,
                        ToWho = effects[i].ToWho,
                        Duration = 0,
                        SummonId = -1
                    },

                    _ => new FxId
                    {
                        EffectType = effects[i].EffectType,
                        DamageType = Enums.eDamage.None,
                        MezType = Enums.eMez.None,
                        ETModifies = Enums.eEffectType.None,
                        ToWho = effects[i].ToWho,
                        Duration = 0,
                        SummonId = -1
                    }
                };

                if (fxIdList.Contains(fxIdentifier))
                {
                    continue;
                }

                var toWho = effects.Length == 1 ||
                            (i < effects.Length - 1 && effects[i].ToWho != effects[i + 1].ToWho) ||
                            (i < effects.Length - 1 && effects[i + 1].EffectType is Enums.eEffectType.Mez or Enums.eEffectType.Enhancement) ||
                            (effects.Length > 1 && i == effects.Length - 1)
                    ? effects[i].ToWho switch
                    {
                        Enums.eToWho.Self => " (Self)",
                        Enums.eToWho.Target => " (Target)",
                        Enums.eToWho.All => " (All)",
                        _ => ""
                    }
                    : "";

                var mezType = effects[i].ToWho == Enums.eToWho.Self
                    ? $"{effects[i].MezType}"
                    : effects[i].MezType switch
                    {
                        Enums.eMez.Held => "Hold",
                        Enums.eMez.Stunned => "Stun",
                        Enums.eMez.Confused => "Confuse",
                        Enums.eMez.Immobilized => "Immobilize",
                        Enums.eMez.Terrorized => "Fear",
                        _ => $"{effects[i].MezType}"
                    };

                effectShorts.Add(effects[i].EffectType switch
                {
                    Enums.eEffectType.ResEffect => $"{(effects[i].BuffedMag < 0 ? "-" : "")}{effects[i].EffectType} ({effects[i].ETModifies}){toWho}",
                    Enums.eEffectType.MezProtect => $"{MezSemantics.GetStatusProtectionLabel(effects[i].MezType)}{toWho}",
                    Enums.eEffectType.MezResist => $"{MezSemantics.GetStatusResistanceLabel(effects[i].MezType)}{toWho}",
                    Enums.eEffectType.Mez => $"{effects[i].ToWho} {mezType}",
                    Enums.eEffectType.Enhancement => $"{effects[i].ToWho} {(effects[i].BuffedMag > 0 ? "+" : "-")}{effects[i].ETModifies}",
                    _ => $"{(effects[i].BuffedMag < 0 ? "-" : "")}{effects[i].EffectType}{toWho}"
                });

                fxIdList.Add(fxIdentifier);
            }

            return string.Join(", ", effectShorts);
        }

        public static List<GroupedFx> Aggregate(List<GroupedFx> greList)
        {
            var ret = new List<GroupedFx>();
            var excludedGre = new List<int>();

            for (var i = 0; i < greList.Count; i++)
            {
                if (excludedGre.Contains(i))
                {
                    continue;
                }

                excludedGre.Add(i);
                var includedGre = new List<int> { i };
                for (var j = 0; j < greList.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (excludedGre.Contains(j))
                    {
                        continue;
                    }

                    if (greList[i].FxIdentifier.EffectType is Enums.eEffectType.SpeedFlying
                        or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning
                        or Enums.eEffectType.JumpHeight)
                    {
                        if (!greList[i].FxIdentifier.Equals(greList[j].FxIdentifier) |
                            greList[i].EnhancementEffect != greList[j].EnhancementEffect |
                            greList[i].DefianceTagged != greList[j].DefianceTagged |
                            !string.Equals(greList[i].ConditionIdentity, greList[j].ConditionIdentity, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!greList[i].FxIdentifier.Equals(greList[j].FxIdentifier) |
                            Math.Abs(greList[i].Mag - greList[j].Mag) > Tolerance |
                            greList[i].EnhancementEffect != greList[j].EnhancementEffect |
                            greList[i].DefianceTagged != greList[j].DefianceTagged |
                            !string.Equals(greList[i].ConditionIdentity, greList[j].ConditionIdentity, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }

                    includedGre.Add(j);
                    excludedGre.Add(j);
                }

                ret.Add(new GroupedFx(greList[i].FxIdentifier, includedGre.Select(e => greList[e]).ToList()));
            }

            return ret;
        }

        private GroupedFx CropIncludedEffects(IPower power)
        {
            var gre = (GroupedFx)Clone();
            gre.IncludedEffects = gre.IncludedEffects
                .Where(e => e >= 0 && e < power.Effects.Length)
                .ToList();

            return gre;
        }

        private static void FinalizeListItem(ref PairedListEx.Item rankedEffect, IPower pBase, IPower pEnh, GroupedFx gre, int effectIndex, bool powerInBuild, float displayBlockFontSize)
        {
            if (pBase.Effects.Any(e => e.EffectType == Enums.eEffectType.EntCreate) & pBase.AbsorbSummonEffects)
            {
                pBase = PlannerEffectResolver.ResolvePower(new Power(pBase), new PlannerEffectResolutionContext
                {
                    ApplyRedirects = false,
                    AbsorbPetEffects = true,
                    ExpandGrantPowers = false,
                    ExpandExecutePowers = false
                }).ResolvedPower;
            }

            var effectSource = gre.GetEffectAt(pEnh);
            var effectType = gre.EffectType;
            var greTooltip = !gre.HasMisc
                ? gre.GetTooltip(pEnh)
                : BuildPresentationText(
                    pEnh,
                    new GroupedFxPresentationRequest(
                        IncludedEffectIds: gre.IncludedEffects,
                        IncludeDamage: true,
                        SimpleText: false,
                        IgnoreConditions: false,
                        LineJoinMode: GroupedFxLineJoinMode.MultiLine));

            var magSum = effectType is Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping
                    or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight
                    ? gre.GetMagSum(pEnh, false)
                    : gre.GetMagSum(pEnh);
            var baseMagSum = effectType is Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping
                or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight
                ? gre.CropIncludedEffects(pBase).GetMagSum(pBase, false)
                : gre.CropIncludedEffects(pBase).GetMagSum(pBase);

            var mezDurationDiff = (effectType == Enums.eEffectType.Mez) & (Math.Abs(
                (effectIndex < pBase.Effects.Length ? pBase.Effects[effectIndex].Duration : 0) -
                (effectIndex < pEnh.Effects.Length ? pEnh.Effects[effectIndex].Duration : 0)) > Tolerance);

            var magDiff = false;
            var buffedMagDiff = false;
            if (pEnh.Effects[effectIndex].Buffable)
            {
                magDiff = (Math.Abs((effectIndex < pBase.Effects.Length ? pBase.Effects[effectIndex].BuffedMag : 0) -
                                    (effectIndex < pEnh.Effects.Length ? pEnh.Effects[effectIndex].BuffedMag : 0)) > Tolerance) |
                         (Math.Abs(magSum - baseMagSum) > Tolerance) |
                         mezDurationDiff;
                buffedMagDiff = effectIndex < pEnh.Effects.Length &&
                                Math.Abs(pEnh.Effects[effectIndex].BuffedMag - pEnh.Effects[effectIndex].Mag) >
                                Tolerance;
            }
            else
            {
                var indexedFx = pEnh.Effects.Select((f, i) => new KeyValuePair<int, IEffect>(i, f)).ToList();
                var fxSourceAlt = gre.IncludedEffects
                    .Select(e => indexedFx[e])
                    .DefaultIfEmpty(new KeyValuePair<int, IEffect>(-1, new Effect()))
                    .FirstOrDefault(e => e.Value.Buffable & (e.Key < pBase.Effects.Length))
                    .Key;

                if (fxSourceAlt >= 0)
                {
                    magDiff = (Math.Abs((fxSourceAlt < pBase.Effects.Length ? pBase.Effects[fxSourceAlt].BuffedMag : 0) -
                                        (fxSourceAlt < pEnh.Effects.Length ? pEnh.Effects[fxSourceAlt].BuffedMag : 0)) > Tolerance) |
                              (Math.Abs(magSum - baseMagSum) > Tolerance) |
                              mezDurationDiff;
                    buffedMagDiff = fxSourceAlt < pEnh.Effects.Length &&
                                    Math.Abs(pEnh.Effects[fxSourceAlt].BuffedMag - pEnh.Effects[fxSourceAlt].Mag) >
                                    Tolerance;
                }
            }

            var toWhoShort = effectSource.ToWho switch
            {
                Enums.eToWho.Self => " (Self)",
                Enums.eToWho.Target => " (Target)",
                Enums.eToWho.All => " (All)",
                _ => ""
            };
            string FormatValueWithTarget(double value, bool asPercent)
            {
                var formatted = asPercent
                    ? $"{DisplayValueFormatter.FormatPercentFromScale(value)}%"
                    : DisplayValueFormatter.FormatNumber(value);
                return $"{formatted}{toWhoShort}";
            }

            rankedEffect.UseUniqueColor = effectSource.isEnhancementEffect;
            rankedEffect.UseAlternateColor = !effectSource.isEnhancementEffect &&
                                          magDiff &&
                                          buffedMagDiff | mezDurationDiff &&
                                          gre.IncludedEffects.Select(e => pEnh.Effects[e].Buffable).Any(e => e) &
                                          powerInBuild;

            if (gre.IsAggregated && effectType is Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping
                    or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight)
            {
                rankedEffect.Value = FormatValueWithTarget(magSum, effectSource.DisplayPercentage);
            }

            switch (effectType)
            {
                case Enums.eEffectType.Fly:
                case Enums.eEffectType.MovementControl:
                case Enums.eEffectType.MovementFriction:
                case Enums.eEffectType.StealthRadius:
                case Enums.eEffectType.StealthRadiusPlayer:
                    rankedEffect.Value = FormatValueWithTarget(magSum, effectSource.DisplayPercentage);

                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.Recovery:
                case Enums.eEffectType.Endurance:
                case Enums.eEffectType.Regeneration:
                    rankedEffect.Name = $"{effectType}";
                    rankedEffect.Value = FormatValueWithTarget(magSum, effectSource.DisplayPercentage);

                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.SilentKill when effectSource.ToWho == Enums.eToWho.Self:
                    rankedEffect.Name = "Lifespan";
                    rankedEffect.Value = $"{DisplayValueFormatter.FormatSeconds(Math.Max(effectSource.Duration, Math.Max(effectSource.DelayedTime, effectSource.Absorbed_Duration)))} s";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.EntCreate:
                    rankedEffect.Name = "Summon";
                    rankedEffect.Value = effectSource.nSummon > -1
                        ? DatabaseAPI.Database.Entities[effectSource.nSummon].DisplayName
                        : Regex.Replace(effectSource.Summon, @"^(MastermindPets|Pets|Villain_Pets)_", string.Empty);

                    if (gre.IncludedEffects.Count > 1)
                    {
                        rankedEffect.Value += $" x{gre.IncludedEffects.Count}";
                    }

                    if (effectSource.nSummon > -1)
                    {
                        var entityTooltip = string.Join("\r\n", gre.IncludedEffects
                            .Select(e => pEnh.Effects[e])
                            .OrderBy(e => e.DelayedTime)
                            .Select(e => e.BuildEffectString(false, "", false, false, false, false, false, true)));

                        var entityPowersets = DatabaseAPI.Database.Entities[effectSource.nSummon].GetNPowerset();
                        if (entityPowersets.Count > 0 && entityPowersets[0] > -1)
                        {
                            var entityPowerset = DatabaseAPI.Database.Powersets[entityPowersets[0]];
                            entityTooltip += "\r\n\r\nEntity has the following Powers:";
                            foreach (var p in entityPowerset.Power)
                            {
                                var epShortDesc = GeneratePowerDescShort(DatabaseAPI.Database.Power[p]);
                                entityTooltip += $"\r\n- {DatabaseAPI.Database.Power[p].DisplayName}";
                                if (!string.IsNullOrEmpty(epShortDesc))
                                {
                                    entityTooltip +=
                                        (string.IsNullOrWhiteSpace(DatabaseAPI.Database.Power[p].DescShort) ||
                                         DatabaseAPI.Database.Power[p].DescShort.Equals(
                                             DatabaseAPI.Database.Power[p].DisplayName,
                                             StringComparison.InvariantCultureIgnoreCase)) &&
                                        !string.IsNullOrEmpty(epShortDesc)
                                            ? $" ({epShortDesc})"
                                            : $" ({DatabaseAPI.Database.Power[p].DescShort})";
                                }
                            }

                            entityTooltip += "\r\n\r\nTo see the effects of these Powers, Left-Click on the Entity.";
                        }

                        rankedEffect.EntTag = DatabaseAPI.Database.Entities[effectSource.nSummon];
                        rankedEffect.ToolTip = entityTooltip;
                    }
                    else
                    {
                        rankedEffect.ToolTip = greTooltip;
                    }

                    break;

                case Enums.eEffectType.GrantPower:
                    rankedEffect.Name = "Grant";
                    if (effectSource.nSummon > -1)
                    {
                        rankedEffect.Value = DatabaseAPI.Database.Power[effectSource.nSummon].DisplayName;
                        var mainEffectTip =
                            effectSource.BuildEffectString(false, "", false, false, false, false, false, true);
                        var subEffectsTip = string.Join("\r\n",
                            DatabaseAPI.Database.Power[effectSource.nSummon].Effects
                                .Where(e => e.PvXInclude() && e.CanInclude())
                                .Select(e => e.BuildEffectString(false, "", false, false, false, false, false, true)
                                    .Replace("\r\n", "\n").Replace("\n", " -- ").Replace("  ", " ")));
                        rankedEffect.ToolTip = $"{mainEffectTip}\r\n----------\r\n{subEffectsTip}";
                    }

                    break;

                case Enums.eEffectType.LevelShift:
                    rankedEffect.Name = "LvlShift";
                    rankedEffect.Value = $"{(effectSource.Mag > 0 ? "+" : "")}{DisplayValueFormatter.FormatNumber(effectSource.Mag, 2)}";

                    break;

                case Enums.eEffectType.RevokePower:
                    rankedEffect.Name = "Revoke";
                    rankedEffect.Value = effectSource.nSummon > -1
                        ? DatabaseAPI.Database.Entities[effectSource.nSummon].DisplayName
                        : Regex.Replace(effectSource.Summon, @"^(MastermindPets|Pets|Villain_Pets)_", string.Empty);

                    break;

                case Enums.eEffectType.DamageBuff:
                    var isDefiance = IsModernDefianceDisplayEffect(effectSource);
                    rankedEffect.Name = isDefiance
                        ? "Defiance"
                        : FastItemBuilder.Str.ShortStr(displayBlockFontSize, Enums.GetEffectName(effectSource.EffectType),
                            Enums.GetEffectNameShort(effectSource.EffectType));
                    rankedEffect.Value = $"{DisplayValueFormatter.FormatPercentFromScale(effectSource.BuffedMag)}%";
                    rankedEffect.ToolTip = isDefiance
                        ? effectSource.BuildEffectString(false, "DamageBuff (Defiance)", false, false, false, true)
                        : greTooltip;

                    break;

                case Enums.eEffectType.Mez:
                    if (gre.NumEffects == 1)
                    {
                        rankedEffect.Name = effectSource.MezType switch
                        {
                            Enums.eMez.Teleport => "TP",
                            Enums.eMez.Knockback => "KB",
                            Enums.eMez.Knockup => "KUp",
                            _ => $"{effectSource.MezType}"
                        };
                    }

                    rankedEffect.Value = effectSource.ToWho switch
                    {
                        Enums.eToWho.Target => effectSource.MezType is Enums.eMez.Knockback or Enums.eMez.Knockup or Enums.eMez.Teleport
                            ? $"{DisplayValueFormatter.FormatMagnitude(effectSource.BuffedMag, 2)}{toWhoShort}"
                            : $"{DisplayValueFormatter.FormatSeconds(effectSource.Duration, 2)}s (Mag {DisplayValueFormatter.FormatMagnitude(effectSource.BuffedMag, 2)}){toWhoShort}",

                        Enums.eToWho.Self => $"{DisplayValueFormatter.FormatMagnitude(effectSource.BuffedMag, 2)}{toWhoShort}",

                        Enums.eToWho.All => $"{DisplayValueFormatter.FormatSeconds(effectSource.Duration, 2)}s (Mag {DisplayValueFormatter.FormatMagnitude(effectSource.BuffedMag, 2)}){toWhoShort}",

                        _ => rankedEffect.Value
                    };

                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.MezProtect:
                case Enums.eEffectType.MezResist:
                    rankedEffect.Name = effectType == Enums.eEffectType.MezProtect
                        ? MezSemantics.GetStatusProtectionLabel(effectSource.MezType, shortForm: true)
                        : MezSemantics.GetStatusResistanceLabel(effectSource.MezType, shortForm: true);
                    rankedEffect.Value = FormatValueWithTarget(effectSource.BuffedMag, effectSource.DisplayPercentage);
                    rankedEffect.ToolTip = greTooltip;
                    break;

                case Enums.eEffectType.Translucency:
                    rankedEffect.Name = "Trnslcncy";
                    rankedEffect.Value = FormatValueWithTarget(effectSource.BuffedMag, effectSource.DisplayPercentage);
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.SpeedFlying:
                case Enums.eEffectType.SpeedJumping:
                case Enums.eEffectType.SpeedRunning:
                    if (gre.GetStatName(pEnh) == "Slow")
                    {
                        rankedEffect.Name = "Slow";
                        rankedEffect.Value = InvertStringValue(rankedEffect.Value);
                    }
                    else if ((gre.IncludedEffects.Count > 1) & gre.IncludedEffects.Select(e => pEnh.Effects[e].EffectType).Any(e => e != pEnh.Effects[gre.IncludedEffects[0]].EffectType))
                    {
                        rankedEffect.Name = $"{((gre.IsAggregated ? magSum : effectSource.Mag) < 0 ? "-" : "")}Movement";
                    }

                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Defense:
                case Enums.eEffectType.Elusivity:
                case Enums.eEffectType.Enhancement:
                case Enums.eEffectType.ResEffect:
                    rankedEffect.Name = effectType == Enums.eEffectType.Enhancement
                        ? gre.IncludedEffects.Count > 1
                            ? effectSource.Mag < 0
                                ? "Debuff"
                                : "Enhancement"
                            : $"{(effectSource.Mag < 0 ? "-" : "+")}{effectSource.ETModifies}"
                        : FastItemBuilder.Str.ShortStr(displayBlockFontSize,
                            Enums.GetEffectName(effectSource.EffectType),
                            Enums.GetEffectNameShort(effectSource.EffectType));

                    rankedEffect.Value = FormatValueWithTarget(effectSource.BuffedMag, effectSource.DisplayPercentage);
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.PerceptionRadius:
                    rankedEffect.Name = $"Pceptn{toWhoShort}";
                    rankedEffect.Value = $"{(effectSource.DisplayPercentage ? $"{DisplayValueFormatter.FormatPercentFromScale(magSum)}%" : $"{DisplayValueFormatter.FormatNumber(magSum, 2)}")} ({DisplayValueFormatter.FormatDistance(Statistics.BasePerception * magSum, 2)}ft)";

                    break;

                case Enums.eEffectType.ToHit:
                    rankedEffect.Name = "ToHit";
                    rankedEffect.Value = $"{DisplayValueFormatter.FormatPercentFromScale(gre.Mag)}%{toWhoShort}";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.RechargeTime:
                    rankedEffect.Value = $"{DisplayValueFormatter.FormatPercentFromScale(gre.Mag)}%{toWhoShort}";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.Heal:
                    rankedEffect.Name = $"Heal{toWhoShort}";
                    rankedEffect.Value = effectSource.DisplayPercentage & (effectSource.DisplayPercentageOverride == Enums.eOverrideBoolean.TrueOverride)
                        ? $"{DisplayValueFormatter.FormatPercentFromScale(gre.Mag, 2)}% HP"
                        : $"{DisplayValueFormatter.FormatNumber(gre.Mag, 2)} HP ({DisplayValueFormatter.FormatPercentValue(gre.Mag / MidsContext.Character.DisplayStats.HealthHitpointsNumeric(false) * 100d, 2)}%)";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.MaxRunSpeed:
                case Enums.eEffectType.MaxJumpSpeed:
                case Enums.eEffectType.MaxFlySpeed:
                case Enums.eEffectType.EnduranceDiscount:
                case Enums.eEffectType.ThreatLevel:
                    rankedEffect.Value = $"{DisplayValueFormatter.FormatPercentFromScale(gre.Mag)}%{toWhoShort}";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                default:
                    rankedEffect.Value = effectSource.DisplayPercentage
                        ? $"{DisplayValueFormatter.FormatPercentValue(magSum, 2)}%{toWhoShort}"
                        : $"{DisplayValueFormatter.FormatNumber(magSum, 2)}{toWhoShort}";
                    rankedEffect.Name = FastItemBuilder.Str.ShortStr(displayBlockFontSize, Enums.GetEffectName(effectSource.EffectType),
                        Enums.GetEffectNameShort(effectSource.EffectType));
                    rankedEffect.ToolTip = string.Join("\r\n", pEnh.Effects
                        .Where(e => e.PvXInclude() &&
                                    e.CanInclude() &&
                                    Math.Abs(e.BuffedMag) > Tolerance &&
                                    effectSource.ToWho == e.ToWho &&
                                    effectSource.EffectType == e.EffectType &&
                                    effectSource.MezType == e.MezType &&
                                    effectSource.ETModifies == e.ETModifies &&
                                    (effectSource.PvMode == e.PvMode || e.PvMode == Enums.ePvX.Any) &&
                                    effectSource.IgnoreScaling == e.IgnoreScaling)
                        .Select(e => e.BuildEffectString(false, "", false, false, false, true)));

                    break;
            }

            rankedEffect.Name = FormatPresentationText(rankedEffect.Name);
            rankedEffect.Value = string.IsNullOrWhiteSpace(rankedEffect.Value)
                ? rankedEffect.Value
                : FormatPresentationText(rankedEffect.Value);
            rankedEffect.ToolTip = string.IsNullOrWhiteSpace(rankedEffect.ToolTip)
                ? rankedEffect.ToolTip
                : FormatPresentationText(rankedEffect.ToolTip);
        }
    }
}

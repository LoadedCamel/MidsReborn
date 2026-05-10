using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Newtonsoft.Json.Linq;

namespace Mids_Reborn.Core.PlannerRulesets;

internal readonly struct EnhancementDiversificationInfo
{
    public EnhancementDiversificationInfo(float appliedValue, float firstThreshold, float secondThreshold, float thirdThreshold)
    {
        AppliedValue = appliedValue;
        FirstThreshold = firstThreshold;
        SecondThreshold = secondThreshold;
        ThirdThreshold = thirdThreshold;
    }

    public float AppliedValue { get; }
    public float FirstThreshold { get; }
    public float SecondThreshold { get; }
    public float ThirdThreshold { get; }

    public int Stage =>
        AppliedValue <= 0f
            ? 0
            : RawValueStage;

    public int GetStageForRawValue(float rawValue)
    {
        if (rawValue > ThirdThreshold)
        {
            return 3;
        }

        if (rawValue > SecondThreshold)
        {
            return 2;
        }

        return rawValue > FirstThreshold ? 1 : 0;
    }

    private int RawValueStage =>
        AppliedValue > ThirdThreshold
            ? 3
            : AppliedValue > SecondThreshold
                ? 2
                : AppliedValue > FirstThreshold
                    ? 1
                    : 0;
}

internal sealed class EnhancementMathPolicy
{
    private readonly Dictionary<string, EnhancementDiversificationCurve> _curvesByBoostKey;
    private readonly Dictionary<Enums.eSchedule, EnhancementDiversificationCurve> _curvesBySchedule;
    private readonly Dictionary<int, float> _effectivenessAbove;
    private readonly Dictionary<int, float> _effectivenessBelow;
    private readonly Dictionary<int, float> _effectivenessBoosters;
    private readonly Dictionary<int, EnhancementExemplarEntry> _exemplarEntriesByLevel;
    private EnhancementDiversificationCurve? _defaultCurve;

    internal EnhancementMathPolicy(ServerRulesProfile profile, EnhancementImportMetadata? metadata)
    {
        _curvesByBoostKey = new Dictionary<string, EnhancementDiversificationCurve>(StringComparer.OrdinalIgnoreCase);
        _curvesBySchedule = new Dictionary<Enums.eSchedule, EnhancementDiversificationCurve>();
        _effectivenessAbove = new Dictionary<int, float>();
        _effectivenessBelow = new Dictionary<int, float>();
        _effectivenessBoosters = new Dictionary<int, float>();
        _exemplarEntriesByLevel = new Dictionary<int, EnhancementExemplarEntry>();

        ProfileId = profile.Id;
        UsesImportedEnhancementMathWhenAvailable = profile.UsesImportedEnhancementMathWhenAvailable;

        if (!UsesImportedEnhancementMathWhenAvailable || metadata == null)
        {
            return;
        }

        UsesImportedDiversification = TryLoadDiversification(metadata.EnhancementDiversification);
        UsesImportedEffectiveness = TryLoadEffectiveness(metadata.EnhancementEffectiveness);
        UsesImportedExemplarScaling = TryLoadExemplarScaling(metadata.EnhancementExemplarScaling);
    }

    public ServerRulesProfileId ProfileId { get; }
    public bool UsesImportedEnhancementMathWhenAvailable { get; }
    public bool UsesImportedDiversification { get; }
    public bool UsesImportedEffectiveness { get; }
    public bool UsesImportedExemplarScaling { get; }

    public float GetScheduleScale(I9Slot slot, Enums.eType enhancementType, Enums.eEnhGrade grade, int ioLevel, Enums.eSchedule schedule, bool isSuperior)
    {
        if (schedule is Enums.eSchedule.None or Enums.eSchedule.Multiple)
        {
            return 0f;
        }

        var baseScale = EnhancementScheduleMath.GetRuntimeScheduleBaseScale(enhancementType, grade, ioLevel, schedule);
        if (baseScale <= float.Epsilon)
        {
            return 0f;
        }

        var effectivenessMultiplier = GetRelativeEffectivenessMultiplier(slot.RelativeLevel, enhancementType);
        if (effectivenessMultiplier <= float.Epsilon)
        {
            return 0f;
        }

        var scale = baseScale * effectivenessMultiplier;
        if (isSuperior)
        {
            scale *= 1.25f;
        }

        return scale;
    }

    public float ApplyCurrentExemplarScaling(float magnitude)
    {
        if (magnitude <= float.Epsilon)
        {
            return magnitude;
        }

        var selectedLevel = Math.Max(1, MidsContext.Config?.ForceLevel ?? Character.MaxLevel + 1) - 1;
        var experienceLevel = MidsContext.Character?.Level ?? selectedLevel;
        return ApplyExemplarScaling(magnitude, selectedLevel, experienceLevel);
    }

    public float ApplyExemplarScaling(float magnitude, int combatLevelZeroBased, int experienceLevelZeroBased)
    {
        if (!UsesImportedExemplarScaling ||
            magnitude <= float.Epsilon ||
            combatLevelZeroBased >= experienceLevelZeroBased ||
            !_exemplarEntriesByLevel.TryGetValue(combatLevelZeroBased, out var combatEntry) ||
            !_exemplarEntriesByLevel.TryGetValue(experienceLevelZeroBased, out var experienceEntry))
        {
            return magnitude;
        }

        var scaledMagnitude = magnitude;
        if (combatEntry.PreClamp > float.Epsilon && scaledMagnitude > combatEntry.PreClamp)
        {
            scaledMagnitude = combatEntry.PreClamp;
        }

        if (scaledMagnitude >= combatEntry.Limit && experienceEntry.Weight > float.Epsilon)
        {
            scaledMagnitude *= combatEntry.Weight / experienceEntry.Weight;
        }

        if (combatEntry.PostClamp > float.Epsilon && scaledMagnitude > combatEntry.PostClamp)
        {
            scaledMagnitude = combatEntry.PostClamp;
        }

        return scaledMagnitude;
    }

    public float GetRelativeEffectivenessMultiplier(Enums.eEnhRelative relativeLevel, Enums.eType enhancementType)
    {
        if (relativeLevel == Enums.eEnhRelative.None)
        {
            return 0f;
        }

        var delta = (int)relativeLevel - (int)Enums.eEnhRelative.Even;
        if (UsesImportedEffectiveness)
        {
            if (TryGetImportedEffectivenessMultiplier(delta, enhancementType, out var importedMultiplier))
            {
                return importedMultiplier;
            }
        }

        return GetLegacyRelativeEffectivenessMultiplier(relativeLevel);
    }

    public EnhancementDiversificationInfo EvaluateDiversification(
        Enums.eSchedule schedule,
        float value,
        Enums.eEnhance enhanceType = Enums.eEnhance.None,
        int subType = -1,
        Enums.eBuffDebuff buffMode = Enums.eBuffDebuff.Any)
    {
        var curve = ResolveDiversificationCurve(schedule, enhanceType, subType, buffMode);
        return new EnhancementDiversificationInfo(
            ApplyCurve(curve, value),
            curve.FirstThreshold,
            curve.SecondThreshold,
            curve.ThirdThreshold);
    }

    public float ApplyDiversification(
        Enums.eSchedule schedule,
        float value,
        Enums.eEnhance enhanceType = Enums.eEnhance.None,
        int subType = -1,
        Enums.eBuffDebuff buffMode = Enums.eBuffDebuff.Any)
    {
        return EvaluateDiversification(schedule, value, enhanceType, subType, buffMode).AppliedValue;
    }

    private bool TryLoadDiversification(JToken? token)
    {
        var rules = token?["rules"] as JArray;
        if (rules == null)
        {
            return false;
        }

        EnhancementDiversificationCurve? defaultCurve = null;
        foreach (var ruleToken in rules.OfType<JObject>())
        {
            var curve = TryParseDiversificationCurve(ruleToken);
            if (curve == null)
            {
                continue;
            }

            var isDefault = ruleToken.Value<bool?>("is_default") ?? false;
            var boosts = ruleToken["boosts"] as JArray;
            if (boosts != null)
            {
                foreach (var boostKey in boosts.Values<string>().Where(value => !string.IsNullOrWhiteSpace(value)))
                {
                    _curvesByBoostKey[boostKey] = curve;
                    if (EnhancementBoostClassResolver.TryResolveScheduleForBoostKey(boostKey, out var schedule))
                    {
                        _curvesBySchedule[schedule] = curve;
                    }
                }
            }

            if (isDefault)
            {
                defaultCurve = curve;
            }
        }

        _defaultCurve = defaultCurve;
        return _curvesByBoostKey.Count > 0 || _defaultCurve != null;
    }

    private static EnhancementDiversificationCurve? TryParseDiversificationCurve(JObject ruleToken)
    {
        var attributeRules = ruleToken["attribute_rules"] as JArray;
        var returns = attributeRules?
            .OfType<JObject>()
            .SelectMany(attributeRule => (attributeRule["returns"] as JArray)?.OfType<JObject>() ?? [])
            .Take(3)
            .Select(returnToken => new EnhancementDiversificationBreakpoint(
                returnToken.Value<float?>("start") ?? -1f,
                returnToken.Value<float?>("handicap") ?? -1f,
                returnToken.Value<float?>("basis") ?? -1f))
            .ToArray();

        if (returns is not { Length: 3 } ||
            returns.Any(breakpoint =>
                breakpoint.Start < 0f ||
                breakpoint.Handicap < 0f ||
                breakpoint.Basis < 0f))
        {
            return null;
        }

        return new EnhancementDiversificationCurve(returns);
    }

    private bool TryLoadEffectiveness(JToken? token)
    {
        var loaded = false;
        loaded |= LoadMultiplierTable(token?["above_level"] as JArray, "level_delta", _effectivenessAbove);
        loaded |= LoadMultiplierTable(token?["below_level"] as JArray, "level_delta", _effectivenessBelow);
        loaded |= LoadMultiplierTable(token?["boosters"] as JArray, "combines", _effectivenessBoosters);
        return loaded;
    }

    private static bool LoadMultiplierTable(JArray? entries, string keyName, IDictionary<int, float> destination)
    {
        var loaded = false;
        if (entries == null)
        {
            return false;
        }

        foreach (var entry in entries.OfType<JObject>())
        {
            var index = entry.Value<int?>(keyName);
            var multiplier = entry.Value<float?>("multiplier");
            if (!index.HasValue || !multiplier.HasValue)
            {
                continue;
            }

            destination[index.Value] = multiplier.Value;
            loaded = true;
        }

        return loaded;
    }

    private bool TryLoadExemplarScaling(JToken? token)
    {
        var combatLevels = token?["combat_levels"] as JArray;
        if (combatLevels == null)
        {
            return false;
        }

        var loaded = false;
        foreach (var entry in combatLevels.OfType<JObject>())
        {
            var level = entry.Value<int?>("level");
            var limit = entry.Value<float?>("limit");
            var weight = entry.Value<float?>("weight");
            var preClamp = entry.Value<float?>("pre_clamp");
            var postClamp = entry.Value<float?>("post_clamp");
            if (!level.HasValue || !limit.HasValue || !weight.HasValue || !preClamp.HasValue || !postClamp.HasValue)
            {
                continue;
            }

            _exemplarEntriesByLevel[level.Value - 1] = new EnhancementExemplarEntry(limit.Value, weight.Value, preClamp.Value, postClamp.Value);
            loaded = true;
        }

        return loaded;
    }

    private bool TryGetImportedEffectivenessMultiplier(int delta, Enums.eType enhancementType, out float multiplier)
    {
        multiplier = 1f;
        if (delta == 0)
        {
            return true;
        }

        if (delta < 0)
        {
            return _effectivenessBelow.TryGetValue(-delta, out multiplier);
        }

        var table = enhancementType is Enums.eType.InventO or Enums.eType.SetO
            ? _effectivenessBoosters
            : _effectivenessAbove;
        return table.TryGetValue(delta, out multiplier);
    }

    private static float GetLegacyRelativeEffectivenessMultiplier(Enums.eEnhRelative relativeLevel)
    {
        var delta = (int)relativeLevel - (int)Enums.eEnhRelative.Even;
        if (delta >= 0)
        {
            return (float)(delta * 0.0500000007450581 + 1.0);
        }

        return (float)(1.0 + delta * 0.100000001490116);
    }

    private EnhancementDiversificationCurve ResolveDiversificationCurve(
        Enums.eSchedule schedule,
        Enums.eEnhance enhanceType,
        int subType,
        Enums.eBuffDebuff buffMode)
    {
        if (UsesImportedDiversification)
        {
            if (enhanceType != Enums.eEnhance.None &&
                EnhancementBoostClassResolver.TryResolveBoostKey(enhanceType, subType, buffMode, out var boostKey) &&
                _curvesByBoostKey.TryGetValue(boostKey, out var curve))
            {
                return curve;
            }

            if (_curvesBySchedule.TryGetValue(schedule, out var scheduleCurve))
            {
                return scheduleCurve;
            }

            if (_defaultCurve != null)
            {
                return _defaultCurve;
            }
        }

        return BuildLegacyCurve(schedule);
    }

    private static float ApplyCurve(EnhancementDiversificationCurve curve, float value)
    {
        if (value <= 0f)
        {
            return value;
        }

        if (value <= curve.FirstThreshold)
        {
            return value;
        }

        if (value > curve.ThirdThreshold)
        {
            return curve.Third.Basis + (value - curve.Third.Start) * curve.Third.Handicap;
        }

        if (value > curve.SecondThreshold)
        {
            return curve.Second.Basis + (value - curve.Second.Start) * curve.Second.Handicap;
        }

        return curve.First.Basis + (value - curve.First.Start) * curve.First.Handicap;
    }

    private static EnhancementDiversificationCurve BuildLegacyCurve(Enums.eSchedule schedule)
    {
        var thresholds = GetLegacyThresholds(schedule);
        return new EnhancementDiversificationCurve(
        [
            new EnhancementDiversificationBreakpoint(thresholds.First, 0.9f, thresholds.First),
            new EnhancementDiversificationBreakpoint(thresholds.Second, 0.7f, thresholds.First + (thresholds.Second - thresholds.First) * 0.9f),
            new EnhancementDiversificationBreakpoint(thresholds.Third, 0.15f,
                thresholds.First + (thresholds.Second - thresholds.First) * 0.9f + (thresholds.Third - thresholds.Second) * 0.7f)
        ]);
    }

    private static (float First, float Second, float Third) GetLegacyThresholds(Enums.eSchedule schedule)
    {
        if (DatabaseAPI.Database.MultED is not { Length: > 0 })
        {
            return (0f, 0f, 0f);
        }

        var scheduleIndex = (int)schedule;
        if (scheduleIndex < 0 ||
            scheduleIndex >= DatabaseAPI.Database.MultED.Length ||
            DatabaseAPI.Database.MultED[scheduleIndex] is not { Length: >= 3 } table)
        {
            return (0f, 0f, 0f);
        }

        return (table[0], table[1], table[2]);
    }

    private readonly struct EnhancementDiversificationBreakpoint
    {
        public EnhancementDiversificationBreakpoint(float start, float handicap, float basis)
        {
            Start = start;
            Handicap = handicap;
            Basis = basis;
        }

        public float Start { get; }
        public float Handicap { get; }
        public float Basis { get; }
    }

    private sealed class EnhancementDiversificationCurve
    {
        public EnhancementDiversificationCurve(EnhancementDiversificationBreakpoint[] breakpoints)
        {
            Breakpoints = breakpoints;
        }

        public EnhancementDiversificationBreakpoint[] Breakpoints { get; }
        public EnhancementDiversificationBreakpoint First => Breakpoints[0];
        public EnhancementDiversificationBreakpoint Second => Breakpoints[1];
        public EnhancementDiversificationBreakpoint Third => Breakpoints[2];
        public float FirstThreshold => First.Start;
        public float SecondThreshold => Second.Start;
        public float ThirdThreshold => Third.Start;
    }

    private readonly struct EnhancementExemplarEntry
    {
        public EnhancementExemplarEntry(float limit, float weight, float preClamp, float postClamp)
        {
            Limit = limit;
            Weight = weight;
            PreClamp = preClamp;
            PostClamp = postClamp;
        }

        public float Limit { get; }
        public float Weight { get; }
        public float PreClamp { get; }
        public float PostClamp { get; }
    }
}

internal static class EnhancementBoostClassResolver
{
    public static bool TryResolveBoostKey(
        Enums.eEnhance enhanceType,
        int subType,
        Enums.eBuffDebuff buffMode,
        out string boostKey)
    {
        boostKey = enhanceType switch
        {
            Enums.eEnhance.Accuracy => "Accuracy_Boost",
            Enums.eEnhance.Damage => "Damage_Boost",
            Enums.eEnhance.Defense => buffMode == Enums.eBuffDebuff.DeBuffOnly ? "Debuff_Defense_Boost" : "Buff_Defense_Boost",
            Enums.eEnhance.EnduranceDiscount => "EnduranceDiscount_Boost",
            Enums.eEnhance.Endurance or Enums.eEnhance.Recovery => "Recovery_Boost",
            Enums.eEnhance.SpeedFlying => "SpeedFlying_Boost",
            Enums.eEnhance.Heal or Enums.eEnhance.HitPoints or Enums.eEnhance.Regeneration or Enums.eEnhance.Absorb => "Heal_Boost",
            Enums.eEnhance.Interrupt => "Interrupt_Boost",
            Enums.eEnhance.JumpHeight or Enums.eEnhance.SpeedJumping => "Jump_Boost",
            Enums.eEnhance.Range => "Range_Boost",
            Enums.eEnhance.RechargeTime or Enums.eEnhance.X_RechargeTime => "Recharge_Boost",
            Enums.eEnhance.Resistance => "Res_Damage_Boost",
            Enums.eEnhance.SpeedRunning => "SpeedRunning_Boost",
            Enums.eEnhance.ToHit => buffMode == Enums.eBuffDebuff.DeBuffOnly ? "Debuff_ToHit_Boost" : "Buff_ToHit_Boost",
            Enums.eEnhance.Slow => "Slow_Boost",
            Enums.eEnhance.Mez => ResolveMezBoostKey(subType),
            _ => string.Empty
        };

        return !string.IsNullOrWhiteSpace(boostKey);
    }

    public static bool TryResolveScheduleForBoostKey(string boostKey, out Enums.eSchedule schedule)
    {
        schedule = boostKey switch
        {
            "Range_Boost" or "Buff_Defense_Boost" or "Debuff_Defense_Boost" or "Res_Damage_Boost" or "Buff_ToHit_Boost" or "Debuff_ToHit_Boost"
                => Enums.eSchedule.B,
            "Interrupt_Boost" => Enums.eSchedule.C,
            "Knockback_Boost" => Enums.eSchedule.D,
            _ => Enums.eSchedule.A
        };

        return true;
    }

    private static string ResolveMezBoostKey(int subType)
    {
        return subType switch
        {
            (int)Enums.eMez.Confused => "Confuse_Boost",
            (int)Enums.eMez.Held => "Hold_Boost",
            (int)Enums.eMez.Immobilized => "Immobilized_Boost",
            (int)Enums.eMez.Knockback or (int)Enums.eMez.Knockup => "Knockback_Boost",
            (int)Enums.eMez.Sleep => "Sleep_Boost",
            (int)Enums.eMez.Stunned => "Stunned_Boost",
            (int)Enums.eMez.Taunt => "Taunt_Boost",
            (int)Enums.eMez.Terrorized or (int)Enums.eMez.Afraid => "Fear_Boost",
            (int)Enums.eMez.Intangible => "Intangible_Boost",
            _ => string.Empty
        };
    }
}

internal static class EnhancementMathPolicyResolver
{
    private static readonly object SyncRoot = new();
    private static EnhancementImportMetadata? _cachedMetadata;
    private static ServerRulesProfileId _cachedProfileId;
    private static EnhancementMathPolicy? _cachedPolicy;

    public static EnhancementMathPolicy Resolve(ServerRulesProfile profile, EnhancementImportMetadata? metadata)
    {
        lock (SyncRoot)
        {
            if (_cachedPolicy != null &&
                ReferenceEquals(_cachedMetadata, metadata) &&
                _cachedProfileId == profile.Id)
            {
                return _cachedPolicy;
            }

            _cachedMetadata = metadata;
            _cachedProfileId = profile.Id;
            _cachedPolicy = new EnhancementMathPolicy(profile, metadata);
            return _cachedPolicy;
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mids_Reborn.Core.Omni;

public sealed class OmniArchetypeDefinition
{
    [JsonProperty("internal_name")]
    public string InternalName { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("display_help")]
    public string DisplayHelp { get; set; } = string.Empty;

    [JsonProperty("display_short_help")]
    public string DisplayShortHelp { get; set; } = string.Empty;

    [JsonProperty("primary_category")]
    public string PrimaryCategory { get; set; } = string.Empty;

    [JsonProperty("secondary_category")]
    public string SecondaryCategory { get; set; } = string.Empty;

    [JsonProperty("power_pool_category")]
    public string PowerPoolCategory { get; set; } = string.Empty;

    [JsonProperty("epic_pool_category")]
    public string EpicPoolCategory { get; set; } = string.Empty;

    [JsonProperty("playable")]
    public bool Playable { get; set; }

    [JsonProperty("primary_powersets")]
    public List<string> PrimaryPowersets { get; set; } = [];

    [JsonProperty("secondary_powersets")]
    public List<string> SecondaryPowersets { get; set; } = [];

    [JsonProperty("epic_powersets")]
    public List<string> EpicPowersets { get; set; } = [];

    [JsonProperty("powersets")]
    public List<string> Powersets { get; set; } = [];

    [JsonExtensionData]
    public IDictionary<string, JToken>? ExtensionData { get; set; }
}

public sealed class OmniPowerDefinition
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonProperty("powerset")]
    public string Powerset { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("display_help")]
    public string DisplayHelp { get; set; } = string.Empty;

    [JsonProperty("display_short_help")]
    public string DisplayShortHelp { get; set; } = string.Empty;

    [JsonProperty("available_level")]
    public JToken? AvailableLevelValue { get; set; }

    [JsonIgnore]
    public int AvailableLevel => OmniScalar.AsInt(AvailableLevelValue);

    [JsonProperty("attack_types")]
    public List<string> AttackTypes { get; set; } = [];

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("auto_issue")]
    public bool AutoIssue { get; set; }

    [JsonProperty("auto_issue_keeps_level")]
    public bool AutoIssueKeepsLevel { get; set; }

    [JsonProperty("accuracy")]
    public JToken? AccuracyValue { get; set; }

    [JsonIgnore]
    public float Accuracy => OmniScalar.AsFloat(AccuracyValue, 1f);

    [JsonProperty("effect_area")]
    public string EffectArea { get; set; } = string.Empty;

    [JsonProperty("max_targets_hit")]
    public JToken? MaxTargetsHitValue { get; set; }

    [JsonIgnore]
    public int MaxTargetsHit => OmniScalar.AsInt(MaxTargetsHitValue);

    [JsonProperty("radius")]
    public JToken? RadiusValue { get; set; }

    [JsonIgnore]
    public float Radius => OmniScalar.AsFloat(RadiusValue);

    [JsonProperty("arc")]
    public JToken? ArcValue { get; set; }

    [JsonIgnore]
    public int Arc => OmniScalar.AsInt(ArcValue);

    [JsonProperty("range")]
    public JToken? RangeValue { get; set; }

    [JsonIgnore]
    public float Range => OmniScalar.AsFloat(RangeValue);

    [JsonProperty("range_secondary")]
    public JToken? RangeSecondaryValue { get; set; }

    [JsonIgnore]
    public float RangeSecondary => OmniScalar.AsFloat(RangeSecondaryValue);

    [JsonProperty("activation_time")]
    public JToken? ActivationTimeValue { get; set; }

    [JsonIgnore]
    public float ActivationTime => OmniScalar.AsFloat(ActivationTimeValue);

    [JsonProperty("recharge_time")]
    public JToken? RechargeTimeValue { get; set; }

    [JsonIgnore]
    public float RechargeTime => OmniScalar.AsFloat(RechargeTimeValue);

    [JsonProperty("activate_period")]
    public JToken? ActivatePeriodValue { get; set; }

    [JsonIgnore]
    public float ActivatePeriod => OmniScalar.AsFloat(ActivatePeriodValue);

    [JsonProperty("endurance_cost")]
    public JToken? EnduranceCostValue { get; set; }

    [JsonIgnore]
    public float EnduranceCost => OmniScalar.AsFloat(EnduranceCostValue);

    [JsonProperty("interrupt_time")]
    public JToken? InterruptTimeValue { get; set; }

    [JsonIgnore]
    public float InterruptTime => OmniScalar.AsFloat(InterruptTimeValue);

    [JsonProperty("root_time")]
    public JToken? RootTimeValue { get; set; }

    [JsonIgnore]
    public float RootTime => OmniScalar.AsFloat(RootTimeValue);

    [JsonProperty("number_allowed")]
    public JToken? NumberAllowedValue { get; set; }

    [JsonIgnore]
    public int NumberAllowed => OmniScalar.AsInt(NumberAllowedValue);

    [JsonProperty("number_of_charges")]
    public JToken? NumberOfChargesValue { get; set; }

    [JsonIgnore]
    public int NumberOfCharges => OmniScalar.AsInt(NumberOfChargesValue);

    [JsonProperty("max_num_charges")]
    public JToken? MaxNumberOfChargesValue { get; set; }

    [JsonIgnore]
    public int MaxNumberOfCharges => OmniScalar.AsInt(MaxNumberOfChargesValue);

    [JsonProperty("usage_time")]
    public JToken? UsageTimeValue { get; set; }

    [JsonIgnore]
    public int UsageTime => OmniScalar.AsInt(UsageTimeValue);

    [JsonProperty("max_toggle_on_time")]
    public JToken? MaxToggleOnTimeValue { get; set; }

    [JsonIgnore]
    public int MaxToggleOnTime => OmniScalar.AsInt(MaxToggleOnTimeValue);

    [JsonProperty("power_lifetime")]
    public JToken? PowerLifetimeValue { get; set; }

    [JsonIgnore]
    public int PowerLifetime => OmniScalar.AsInt(PowerLifetimeValue);

    [JsonProperty("power_lifetime_ingame")]
    public JToken? PowerLifetimeInGameValue { get; set; }

    [JsonIgnore]
    public int PowerLifetimeInGame => OmniScalar.AsInt(PowerLifetimeInGameValue);

    [JsonProperty("max_power_lifetime")]
    public JToken? MaxPowerLifetimeValue { get; set; }

    [JsonIgnore]
    public int MaxPowerLifetime => OmniScalar.AsInt(MaxPowerLifetimeValue);

    [JsonProperty("max_power_lifetime_ingame")]
    public JToken? MaxPowerLifetimeInGameValue { get; set; }

    [JsonIgnore]
    public int MaxPowerLifetimeInGame => OmniScalar.AsInt(MaxPowerLifetimeInGameValue);

    [JsonProperty("max_boosts")]
    public JToken? MaxBoostsValue { get; set; }

    [JsonIgnore]
    public string MaxBoosts => OmniScalar.AsString(MaxBoostsValue);

    [JsonProperty("boosts_allowed")]
    public List<string> BoostsAllowed { get; set; } = [];

    [JsonProperty("allowed_boostset_cats")]
    public List<string> AllowedBoostSetCats { get; set; } = [];

    [JsonProperty("archetypes")]
    public List<string> Archetypes { get; set; } = [];

    [JsonProperty("exclusion_groups")]
    public List<string> ExclusionGroups { get; set; } = [];

    [JsonProperty("recharge_groups")]
    public List<string> RechargeGroups { get; set; } = [];

    [JsonProperty("notify_ai_when")]
    public string NotifyAiWhen { get; set; } = string.Empty;

    [JsonProperty("cast_through")]
    public List<string> CastThrough { get; set; } = [];

    [JsonProperty("strengths_disallowed")]
    public List<string> StrengthsDisallowed { get; set; } = [];

    [JsonProperty("global_strengths_disallowed")]
    public List<string> GlobalStrengthsDisallowed { get; set; } = [];

    [JsonProperty("boost_info")]
    public JToken? BoostInfo { get; set; }

    [JsonProperty("proc_allowed")]
    public JToken? ProcAllowedValue { get; set; }

    [JsonIgnore]
    public bool ProcAllowed => OmniScalar.AsBool(ProcAllowedValue);

    [JsonProperty("procs_only_on_main_target")]
    public bool ProcsOnlyOnMainTarget { get; set; }

    [JsonProperty("proc_ignore_chain_effect")]
    public bool ProcIgnoreChainEffect { get; set; }

    [JsonProperty("proc_ignore_over_cap")]
    public bool ProcIgnoreOverCap { get; set; }

    [JsonProperty("targets_affected")]
    public List<string> TargetsAffected { get; set; } = [];

    [JsonProperty("targets_autohit")]
    public List<string> TargetsAutoHit { get; set; } = [];

    [JsonProperty("target_type")]
    public string TargetType { get; set; } = string.Empty;

    [JsonProperty("target_type_secondary")]
    public JToken? TargetTypeSecondaryValue { get; set; }

    [JsonIgnore]
    public string TargetTypeSecondary => OmniScalar.AsString(TargetTypeSecondaryValue);

    [JsonProperty("target_visibility")]
    public string TargetVisibility { get; set; } = string.Empty;

    [JsonProperty("cast_when_dead")]
    public JToken? CastWhenDeadValue { get; set; }

    [JsonIgnore]
    public string CastWhenDead => OmniScalar.AsString(CastWhenDeadValue);

    [JsonProperty("caster_near_ground")]
    public JToken? CasterNearGroundValue { get; set; }

    [JsonIgnore]
    public bool CasterNearGround => OmniScalar.AsBool(CasterNearGroundValue);

    [JsonProperty("target_near_ground")]
    public JToken? TargetNearGroundValue { get; set; }

    [JsonIgnore]
    public bool TargetNearGround => OmniScalar.AsBool(TargetNearGroundValue);

    [JsonProperty("ignore_strength")]
    public bool IgnoreStrength { get; set; }

    [JsonProperty("do_not_save")]
    public bool DoNotSave { get; set; }

    [JsonProperty("show_buff_icon")]
    public bool ShowBuffIcon { get; set; }

    [JsonProperty("show_in_manage")]
    public JToken? ShowInManageValue { get; set; }

    [JsonIgnore]
    public bool ShowInManage => OmniScalar.AsBool(ShowInManageValue, true);

    [JsonProperty("show_in_info")]
    public JToken? ShowInInfoValue { get; set; }

    [JsonIgnore]
    public bool ShowInInfo => OmniScalar.AsBool(ShowInInfoValue, true);

    [JsonProperty("modes_required")]
    public List<string> ModesRequired { get; set; } = [];

    [JsonProperty("modes_disallowed")]
    public List<string> ModesDisallowed { get; set; } = [];

    [JsonProperty("modes_suspended")]
    public List<string> ModesSuspended { get; set; } = [];

    [JsonProperty("requires")]
    public string Requires { get; set; } = string.Empty;

    [JsonProperty("target_requires")]
    public string TargetRequires { get; set; } = string.Empty;

    [JsonProperty("activate_requires")]
    public string ActivateRequires { get; set; } = string.Empty;

    [JsonProperty("server_tray_requires")]
    public string ServerTrayRequires { get; set; } = string.Empty;

    [JsonProperty("confirm_requires")]
    public string ConfirmRequires { get; set; } = string.Empty;

    [JsonProperty("highlight_expression")]
    public string HighlightExpression { get; set; } = string.Empty;

    [JsonProperty("max_targets_expression")]
    public string MaxTargetsExpression { get; set; } = string.Empty;

    [JsonProperty("chain_effect_expr")]
    public string ChainEffectExpression { get; set; } = string.Empty;

    [JsonProperty("chain_target_expr")]
    public string ChainTargetExpression { get; set; } = string.Empty;

    [JsonProperty("effects")]
    public List<OmniEffectDefinition> Effects { get; set; } = [];

    [JsonProperty("activation_effects")]
    public List<OmniEffectDefinition> ActivationEffects { get; set; } = [];

    [JsonProperty("redirect")]
    public List<OmniRedirectDefinition> Redirects { get; set; } = [];

    [JsonExtensionData]
    public IDictionary<string, JToken>? ExtensionData { get; set; }
}

public sealed class OmniPowersetDefinition
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("display_fullname")]
    public string FullName { get; set; } = string.Empty;

    [JsonProperty("display_help")]
    public string DisplayHelp { get; set; } = string.Empty;

    [JsonProperty("display_short_help")]
    public string DisplayShortHelp { get; set; } = string.Empty;

    [JsonProperty("available_level")]
    public JToken? AvailableLevelValue { get; set; }

    [JsonIgnore]
    public int AvailableLevel => OmniScalar.AsInt(AvailableLevelValue);

    [JsonProperty("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonProperty("requires")]
    public string Requires { get; set; } = string.Empty;

    [JsonProperty("archetypes")]
    public List<string> Archetypes { get; set; } = [];

    [JsonProperty("power_names")]
    public List<string> PowerNames { get; set; } = [];

    [JsonProperty("power_category")]
    public string PowerCategory { get; set; } = string.Empty;

    [JsonProperty("powers")]
    public List<OmniPowersetPowerEntry> Powers { get; set; } = [];
}

public sealed class OmniPowersetPowerEntry
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("local_available_level")]
    public JToken? LocalAvailableLevelValue { get; set; }

    [JsonIgnore]
    public int LocalAvailableLevel => OmniScalar.AsInt(LocalAvailableLevelValue);

    [JsonProperty("available_level")]
    public JToken? AvailableLevelValue { get; set; }

    [JsonIgnore]
    public int AvailableLevel => OmniScalar.AsInt(AvailableLevelValue);

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("short_help")]
    public string ShortHelp { get; set; } = string.Empty;
}

public sealed class OmniRedirectDefinition
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("requires")]
    public string Requires { get; set; } = string.Empty;
}

public sealed class OmniEffectDefinition
{
    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = [];

    [JsonProperty("flags")]
    public List<string> Flags { get; set; } = [];

    [JsonProperty("requires_expression")]
    public string RequiresExpression { get; set; } = string.Empty;

    [JsonProperty("chance")]
    public JToken? ChanceValue { get; set; }

    [JsonIgnore]
    public float Chance => OmniScalar.AsFloat(ChanceValue, 1f);

    [JsonProperty("ppm")]
    public JToken? PpmValue { get; set; }

    [JsonIgnore]
    public float Ppm => OmniScalar.AsFloat(PpmValue);

    [JsonProperty("delay")]
    public JToken? DelayValue { get; set; }

    [JsonIgnore]
    public float Delay => OmniScalar.AsFloat(DelayValue);

    [JsonProperty("templates")]
    public List<OmniEffectTemplate> Templates { get; set; } = [];

    [JsonProperty("child_effects")]
    public List<OmniEffectDefinition> ChildEffects { get; set; } = [];
}

public sealed class OmniEffectTemplate
{
    [JsonProperty("attribs")]
    public List<string> Attribs { get; set; } = [];

    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = [];

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("application_type")]
    public string ApplicationType { get; set; } = string.Empty;

    [JsonProperty("aspect")]
    public string Aspect { get; set; } = string.Empty;

    [JsonProperty("target")]
    public string Target { get; set; } = string.Empty;

    [JsonProperty("table")]
    public string Table { get; set; } = string.Empty;

    [JsonProperty("scale")]
    public JToken? ScaleValue { get; set; }

    [JsonIgnore]
    public float Scale => OmniScalar.AsFloat(ScaleValue, 1f);

    [JsonProperty("duration")]
    public JToken? DurationValue { get; set; }

    [JsonIgnore]
    public float Duration => OmniScalar.AsFloat(DurationValue);

    [JsonIgnore]
    public string DurationRaw => OmniScalar.AsString(DurationValue);

    [JsonProperty("magnitude")]
    public JToken? MagnitudeValue { get; set; }

    [JsonIgnore]
    public float Magnitude => OmniScalar.AsFloat(MagnitudeValue);

    [JsonIgnore]
    public string MagnitudeRaw => OmniScalar.AsString(MagnitudeValue);

    [JsonProperty("mode_name")]
    public string? ModeName { get; set; }

    [JsonProperty("duration_expression")]
    public string DurationExpression { get; set; } = string.Empty;

    [JsonProperty("magnitude_expression")]
    public string MagnitudeExpression { get; set; } = string.Empty;

    [JsonProperty("application_period")]
    public JToken? ApplicationPeriodValue { get; set; }

    [JsonIgnore]
    public float ApplicationPeriod => OmniScalar.AsFloat(ApplicationPeriodValue);

    [JsonProperty("stack")]
    public string Stack { get; set; } = string.Empty;

    [JsonProperty("caster_stack")]
    public string CasterStack { get; set; } = string.Empty;

    [JsonProperty("flags")]
    public List<string> Flags { get; set; } = [];

    [JsonProperty("jit_requires")]
    public string JitRequires { get; set; } = string.Empty;

    [JsonProperty("params")]
    public JObject? Params { get; set; }
}

public sealed class OmniEntityDefinition
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("internal_name")]
    public string InternalName { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("class_id")]
    public string ClassId { get; set; } = string.Empty;

    [JsonProperty("ai_config")]
    public string AiConfig { get; set; } = string.Empty;

    [JsonProperty("min_level")]
    public int MinLevel { get; set; }

    [JsonProperty("max_level")]
    public int MaxLevel { get; set; }

    [JsonProperty("can_zone")]
    public bool CanZone { get; set; }

    [JsonProperty("spawn_limit")]
    public int SpawnLimit { get; set; }

    [JsonProperty("spawn_limit_mission")]
    public int SpawnLimitMission { get; set; }

    [JsonProperty("pet_visibility")]
    public int PetVisibility { get; set; }

    [JsonProperty("pet_commandability")]
    public int PetCommandability { get; set; }

    [JsonProperty("power_ids")]
    public List<string> PowerIds { get; set; } = [];

    [JsonProperty("power_references")]
    public List<OmniEntityPowerReference> PowerReferences { get; set; } = [];
}

public sealed class OmniEntityPowerReference
{
    [JsonProperty("power_category")]
    public string PowerCategory { get; set; } = string.Empty;

    [JsonProperty("power_set")]
    public string PowerSet { get; set; } = string.Empty;

    [JsonProperty("power")]
    public string Power { get; set; } = string.Empty;
}

internal static class OmniScalar
{
    public static float AsFloat(JToken? token, float defaultValue = 0f)
    {
        if (token == null || token.Type == JTokenType.Null)
        {
            return defaultValue;
        }

        if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
        {
            return token.Value<float>();
        }

        var text = token.Value<string>()?.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return defaultValue;
        }

        if (float.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        var space = text.IndexOf(' ');
        if (space > 0 &&
            float.TryParse(text[..space], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value))
        {
            return value;
        }

        return defaultValue;
    }

    public static string AsString(JToken? token)
    {
        if (token == null || token.Type == JTokenType.Null)
        {
            return string.Empty;
        }

        return token.Type == JTokenType.String
            ? token.Value<string>() ?? string.Empty
            : token.ToString(Formatting.None);
    }

    public static int AsInt(JToken? token, int defaultValue = 0)
    {
        if (token == null || token.Type == JTokenType.Null)
        {
            return defaultValue;
        }

        if (token.Type == JTokenType.Integer)
        {
            return token.Value<int>();
        }

        if (token.Type == JTokenType.Float)
        {
            return (int)token.Value<float>();
        }

        var text = token.Value<string>()?.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return defaultValue;
        }

        if (int.TryParse(text, System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        if (float.TryParse(text, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var floatValue))
        {
            return (int)floatValue;
        }

        var space = text.IndexOf(' ');
        if (space > 0 &&
            float.TryParse(text[..space], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out floatValue))
        {
            return (int)floatValue;
        }

        return defaultValue;
    }

    public static bool AsBool(JToken? token, bool defaultValue = false)
    {
        if (token == null || token.Type == JTokenType.Null)
        {
            return defaultValue;
        }

        if (token.Type == JTokenType.Boolean)
        {
            return token.Value<bool>();
        }

        if (token.Type == JTokenType.Integer)
        {
            return token.Value<int>() != 0;
        }

        var text = token.Value<string>()?.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return defaultValue;
        }

        if (bool.TryParse(text, out var value))
        {
            return value;
        }

        if (int.TryParse(text, System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out var intValue))
        {
            return intValue != 0;
        }

        return defaultValue;
    }
}

using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.PlannerRulesets;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Mids_Reborn.Core.Omni;

public static class OmniMidsMapper
{
    private enum TemplateSemantic
    {
        None,
        VectorDefense,
        VectorResistance
    }

    private enum ChanceModScope
    {
        Global,
        PowerLocal
    }

    public static Effect CreatePowerRedirectEffect(string sourcePowerFullName, OmniRedirectDefinition redirect)
    {
        var effect = new Effect
        {
            PowerFullName = sourcePowerFullName,
            EffectType = Enums.eEffectType.PowerRedirect,
            EffectClass = Enums.eEffectClass.Primary,
            BaseProbability = 1f,
            Override = redirect.Name,
            AdvancedConditions = OmniExpressionConverter.ToConditionSet(
                AdvancedConditionEvaluationMode.ReportOnly,
                sourcePowerFullName,
                redirect.Requires),
            OmniSource = $"{sourcePowerFullName}:redirect:{redirect.Name}"
        };

        return effect;
    }

    public static IEnumerable<Effect> FlattenEffects(OmniPowerDefinition power, OmniApplyResult? applyResult = null)
    {
        return FlattenEffectGroups(power, power.Effects, "effect", applyResult);
    }

    public static IEnumerable<Effect> FlattenEffectGroups(
        OmniPowerDefinition power,
        IReadOnlyList<OmniEffectDefinition> effectGroups,
        string sourcePrefix,
        OmniApplyResult? applyResult = null)
    {
        var uniqueId = 1;
        for (var effectIndex = 0; effectIndex < effectGroups.Count; effectIndex++)
        {
            foreach (var flattened in FlattenEffect(power, effectGroups[effectIndex], ref uniqueId, applyResult, [], $"{sourcePrefix}[{effectIndex}]"))
            {
                yield return flattened;
            }
        }
    }

    private static IEnumerable<Effect> FlattenEffect(
        OmniPowerDefinition power,
        OmniEffectDefinition source,
        ref int uniqueId,
        OmniApplyResult? applyResult,
        IReadOnlyCollection<string> inheritedTags,
        string sourcePath)
    {
        var effects = new List<Effect>();
        var effectTags = MergeTags(inheritedTags, source.Tags, source.Flags).ToArray();
        if (effectTags.Length > 0)
        {
            applyResult?.AddLimited(applyResult.EffectGroupTagDetails,
                $"{power.FullName}: {string.Join(", ", effectTags)}");
            if (applyResult != null)
            {
                applyResult.EffectGroupTagsPreserved += effectTags.Length;
            }
        }

        for (var templateIndex = 0; templateIndex < source.Templates.Count; templateIndex++)
        {
            var template = source.Templates[templateIndex];
            if (string.IsNullOrWhiteSpace(template.Type))
            {
                applyResult?.AddLimited(applyResult.UnknownEffectMappingDetails, $"{power.FullName} template has no type.");
                if (applyResult != null)
                {
                    applyResult.UnknownEffectMappings++;
                }
            }

            var attribs = template.Attribs.Count == 0 ? [string.Empty] : template.Attribs;
            var templateSemantic = ClassifyTemplateSemantic(power, source, template, effectTags);
            if (applyResult != null && templateSemantic != TemplateSemantic.None)
            {
                switch (templateSemantic)
                {
                    case TemplateSemantic.VectorDefense:
                        applyResult.VectorDefenseTemplateOverrides++;
                        break;
                    case TemplateSemantic.VectorResistance:
                        applyResult.VectorResistanceTemplateOverrides++;
                        break;
                }

                applyResult.AddLimited(
                    applyResult.VectorTemplateSemanticOverrideDetails,
                    $"{power.FullName}: template semantic {DescribeTemplateSemantic(templateSemantic)} over attribs [{string.Join(", ", attribs.Where(static value => !string.IsNullOrWhiteSpace(value)))}] (tags=[{string.Join(", ", effectTags)}], aspect='{template.Aspect}', table='{template.Table}').");
            }

            for (var attribIndex = 0; attribIndex < attribs.Count; attribIndex++)
            {
                var attrib = attribs[attribIndex];
                var normalizedAttrib = Normalize(attrib);
                var mappedType = MapEffectType(power, template.Type, attrib, template.Aspect, template.Target, template.Table, templateSemantic);
                if (mappedType == Enums.eEffectType.None &&
                    IsKnownUnsupportedEffectAttrib(power, source, template, normalizedAttrib, template.Type, effectTags))
                {
                    applyResult?.AddLimited(
                        applyResult.KnownUnsupportedEffectMappingDetails,
                        $"{power.FullName} template type '{template.Type}' attrib '{attrib}' is a known unsupported stateful/scalar attrib and was preserved without coercion.");
                    if (applyResult != null)
                    {
                        applyResult.KnownUnsupportedEffectMappings++;
                    }
                }
                else if (mappedType == Enums.eEffectType.None && !string.IsNullOrWhiteSpace(template.Type))
                {
                    applyResult?.AddLimited(applyResult.UnknownEffectMappingDetails,
                        $"{power.FullName} template type '{template.Type}' attrib '{attrib}' did not map to a Mids effect type.");
                    if (applyResult != null)
                    {
                        applyResult.UnknownEffectMappings++;
                    }
                }

                if (string.IsNullOrWhiteSpace(attrib) && mappedType == Enums.eEffectType.None)
                {
                    applyResult?.AddLimited(applyResult.UnknownAttribMappingDetails, $"{power.FullName} has blank attrib.");
                    if (applyResult != null)
                    {
                        applyResult.UnknownAttribMappings++;
                    }
                }

                var modifierTable = string.IsNullOrWhiteSpace(template.Table)
                    ? "Melee_Ones"
                    : DatabaseAPI.NormalizeModifierTableName(template.Table);
                var pvMode = MapPvMode(mappedType, source.RequiresExpression, template, out var pvModeSource);
                var stackPolicy = ImportedStackPolicyNormalizer.FromTemplate(template);
                var effect = new Effect
                {
                    PowerFullName = power.FullName,
                    UniqueID = uniqueId++,
                    EffectClass = Enums.eEffectClass.Primary,
                    EffectType = mappedType,
                    DamageType = MapDamageType(attrib, template.Table),
                    MezType = MapMezType(attrib, template.Type),
                    ToWho = MapToWho(power, template.Target),
                    Stacking = ImportedStackPolicyNormalizer.ToCompatibilityStacking(stackPolicy),
                    StackPolicy = stackPolicy,
                    SpecialCase = MapSpecialCase(power, template, attrib, mappedType),
                    AttribType = MapAttribType(template.Type),
                    Aspect = MapAspect(template.Aspect),
                    PvMode = pvMode,
                    Scale = template.Scale,
                    nMagnitude = template.Magnitude,
                    nDuration = template.Duration,
                    DelayedTime = source.Delay,
                    BaseProbability = source.Chance <= 0 ? 1f : source.Chance,
                    ProcsPerMinute = source.Ppm,
                    ModifierTable = modifierTable,
                    GrantBoosted = IsGrantBoostedTemplate(template.Type, attrib),
                    EffectId = effectTags.FirstOrDefault() ?? "Ones",
                    EffectTags = effectTags.ToList(),
                    OmniSource = $"{power.FullName}:{sourcePath}:template[{templateIndex}]:attrib[{attribIndex}]={attrib}",
                    AdvancedConditions = OmniExpressionConverter.ToConditionSet(
                        AdvancedConditionEvaluationMode.ReportOnly,
                        power.FullName,
                        source.RequiresExpression,
                        template.JitRequires)
                };
                ApplyCombatModFlags(effect, template);
                TrackPvTargetAudit(power, source, template, effect, pvModeSource, applyResult);
                if (!DatabaseAPI.ModifierTableExists(effect.ModifierTable))
                {
                    applyResult?.AddLimited(applyResult.UnknownAttribMappingDetails,
                        $"{power.FullName} modifier table '{effect.ModifierTable}' did not resolve.");
                    applyResult?.AddLimited(applyResult.MissingModifierTableReferenceDetails,
                        $"{power.FullName}: modifier table '{effect.ModifierTable}' was not found in canonical class tables.");
                    if (applyResult != null)
                    {
                        applyResult.UnknownAttribMappings++;
                        applyResult.MissingModifierTableReferences++;
                    }
                }

                if (IsKnownHiddenStatefulEffect(effect))
                {
                    applyResult?.AddLimited(
                        applyResult.KnownHiddenStatefulEffectMappingDetails,
                        $"{power.FullName} template type '{template.Type}' attrib '{attrib}' mapped to hidden stateful effect {DescribeStatefulEffect(effect)}.");
                    if (applyResult != null)
                    {
                        applyResult.KnownHiddenStatefulEffectMappings++;
                    }
                }

                ApplySupportedExpressions(effect, template);
                ApplyModePayload(effect, template, applyResult);
                ApplyParams(effect, template, power.FullName, applyResult);

                if (IsZeroValueTagCarrier(effect))
                {
                    applyResult?.AddLimited(applyResult.ZeroValueTagCarrierDetails,
                        $"{power.FullName}: suppressed {effect.EffectType} tag carrier [{string.Join(", ", GetEffectTagValues(effect))}]");
                    if (applyResult != null)
                    {
                        applyResult.ZeroValueTagCarrierEffectsSuppressed++;
                    }

                    continue;
                }

                if (effect.EffectType == Enums.eEffectType.GlobalChanceMod)
                {
                    var chanceModScope = GetChanceModScope(template);
                    effect.OmniSource = ChanceModifierSupport.ApplyChanceModScope(
                        effect.OmniSource,
                        chanceModScope == ChanceModScope.PowerLocal);
                    var filterTags = GetEffectFilterTags(template).ToArray();
                    if (filterTags.Length > 0)
                    {
                        foreach (var filterTag in filterTags)
                        {
                            var gcmEffect = (Effect)effect.Clone();
                            gcmEffect.UniqueID = uniqueId++;
                            gcmEffect.Reward = filterTag;
                            effects.Add(gcmEffect);
                            TrackChanceModMapping(applyResult, power.FullName, chanceModScope, filterTag);
                            applyResult?.AddLimited(applyResult.TemplateFilterTagDetails,
                                $"{power.FullName}: {DescribeChanceModScope(chanceModScope)} chance mod filters {filterTag}");
                            if (applyResult != null)
                            {
                                applyResult.TemplateFilterTagsMapped++;
                            }
                        }

                        continue;
                    }

                    TrackChanceModMapping(applyResult, power.FullName, chanceModScope, string.Empty);
                }

                effects.Add(effect);
            }
        }

        for (var childIndex = 0; childIndex < source.ChildEffects.Count; childIndex++)
        {
            foreach (var flattened in FlattenEffect(power, source.ChildEffects[childIndex], ref uniqueId, applyResult, effectTags, $"{sourcePath}:child[{childIndex}]"))
            {
                effects.Add(flattened);
            }
        }

        return effects;
    }

    private static bool IsZeroValueTagCarrier(Effect effect)
    {
        if (effect.EffectType is Enums.eEffectType.SetMode or
            Enums.eEffectType.UnsetMode or
            Enums.eEffectType.GrantPower or
            Enums.eEffectType.ExecutePower or
            Enums.eEffectType.PowerRedirect or
            Enums.eEffectType.EntCreate or
            Enums.eEffectType.Null or
            Enums.eEffectType.GlobalChanceMod)
        {
            return false;
        }

        if (effect.EffectType != Enums.eEffectType.DamageBuff)
        {
            return false;
        }

        if (Math.Abs(effect.Scale) > 0.0001f || Math.Abs(effect.nMagnitude) > 0.0001f || effect.nDuration > 0)
        {
            return false;
        }

        return GetEffectTagValues(effect).Any(tag => !tag.Equals("Ones", StringComparison.OrdinalIgnoreCase));
    }

    private static IEnumerable<string> GetEffectTagValues(Effect effect)
    {
        if (!string.IsNullOrWhiteSpace(effect.EffectId))
        {
            yield return effect.EffectId;
        }

        foreach (var tag in effect.EffectTags.Where(tag => !string.IsNullOrWhiteSpace(tag)))
        {
            yield return tag;
        }
    }

    private static void ApplySupportedExpressions(Effect effect, OmniEffectTemplate template)
    {
        // Import only expressions that the planner can evaluate through the shared
        // Mids expression pipeline. Runtime-only server expressions still fall back
        // to the imported numeric values above.
        var durationCompatible = Expressions.CanEvaluatePlannerExpression(template.DurationExpression);
        var magnitudeCompatible = Expressions.CanEvaluatePlannerExpression(template.MagnitudeExpression);
        if (!durationCompatible && !magnitudeCompatible)
        {
            return;
        }

        if (durationCompatible)
        {
            effect.AttribType = Enums.eAttribType.Expression;
            effect.Expressions.Duration = template.DurationExpression;
        }

        if (magnitudeCompatible)
        {
            effect.AttribType = Enums.eAttribType.Expression;
            effect.Expressions.Magnitude = template.MagnitudeExpression;
        }
    }

    private static void ApplyParams(
        Effect effect,
        OmniEffectTemplate template,
        string powerFullName,
        OmniApplyResult? applyResult)
    {
        if (template.Params == null)
        {
            return;
        }

        var type = template.Params.Value<string>("type") ?? string.Empty;
        switch (type.ToLowerInvariant())
        {
            case "entcreate":
                effect.EffectType = Enums.eEffectType.EntCreate;
                effect.Summon = template.Params.Value<string>("entity_def") ?? string.Empty;
                break;
            case "grantpower":
            case "grantboostedpower":
                effect.EffectType = Enums.eEffectType.GrantPower;
                effect.GrantBoosted |= type.Equals("grantboostedpower", StringComparison.OrdinalIgnoreCase);
                effect.Summon = GetPowerParam(template.Params);
                break;
            case "power" when effect.EffectType == Enums.eEffectType.RevokePower:
                var powerName = GetPowerParam(template.Params);
                effect.RevokedPower = powerName;
                effect.Override = powerName;
                break;
            case "power" when effect.EffectType is Enums.eEffectType.GrantPower or Enums.eEffectType.ExecutePower:
                effect.Summon = GetPowerParam(template.Params);
                break;
            case "effectfilter":
                TrackUnsupportedEffectFilterParams(powerFullName, template.Params, applyResult);
                break;
        }
    }

    private static string GetPowerParam(JToken parameters)
    {
        return parameters["power_names"]?.Values<string>().FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ??
               parameters.Value<string>("power") ??
               parameters.Value<string>("power_name") ??
               parameters.Value<string>("power_full_name") ??
               string.Empty;
    }

    private static bool IsGrantBoostedTemplate(string type, string attrib)
    {
        return Normalize(type) == "grantboostedpower" || Normalize(attrib) == "grantboostedpower";
    }

    private static IEnumerable<string> MergeTags(params IEnumerable<string>?[] tagSets)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tagSet in tagSets)
        {
            foreach (var tag in tagSet ?? [])
            {
                var trimmed = tag.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed) && seen.Add(trimmed))
                {
                    yield return trimmed;
                }
            }
        }
    }

    private static ChanceModScope GetChanceModScope(OmniEffectTemplate template)
    {
        return IsPowerLocalChanceModTemplate(template)
            ? ChanceModScope.PowerLocal
            : ChanceModScope.Global;
    }

    private static bool IsPowerLocalChanceModTemplate(OmniEffectTemplate template)
    {
        return template.Attribs.Any(attrib =>
                   attrib.Equals("Power_Chance_Mod", StringComparison.OrdinalIgnoreCase) ||
                   attrib.Equals("PowerChanceMod", StringComparison.OrdinalIgnoreCase)) ||
               template.Type.Equals("Power_Chance_Mod", StringComparison.OrdinalIgnoreCase) ||
               template.Type.Equals("PowerChanceMod", StringComparison.OrdinalIgnoreCase);
    }

    private static void TrackChanceModMapping(
        OmniApplyResult? applyResult,
        string powerFullName,
        ChanceModScope scope,
        string reward)
    {
        if (applyResult == null)
        {
            return;
        }

        if (scope == ChanceModScope.PowerLocal)
        {
            applyResult.PowerLocalChanceModsMapped++;
        }
        else
        {
            applyResult.GlobalChanceModsMapped++;
        }

        applyResult.AddLimited(
            applyResult.ChanceModMappingDetails,
            $"{powerFullName}: mapped {DescribeChanceModScope(scope)} chance mod reward '{(string.IsNullOrWhiteSpace(reward) ? "*" : reward)}'.");
    }

    private static string DescribeChanceModScope(ChanceModScope scope)
    {
        return scope == ChanceModScope.PowerLocal ? "power-local" : "global";
    }

    private static IEnumerable<string> GetEffectFilterTags(OmniEffectTemplate template)
    {
        if (template.Params != null &&
            string.Equals(template.Params.Value<string>("type"), "EffectFilter", StringComparison.OrdinalIgnoreCase))
        {
            var paramTags = ReadStringArray(template.Params["tags"]).ToArray();
            if (paramTags.Length > 0)
            {
                return paramTags;
            }
        }

        return MergeTags(template.Tags);
    }

    private static IEnumerable<string> ReadStringArray(Newtonsoft.Json.Linq.JToken? token)
    {
        if (token == null)
        {
            yield break;
        }

        if (token.Type == Newtonsoft.Json.Linq.JTokenType.Array)
        {
            foreach (var value in token.Values<string>())
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    yield return value.Trim();
                }
            }

            yield break;
        }

        var scalar = token.Type == Newtonsoft.Json.Linq.JTokenType.String
            ? token.ToString()
            : string.Empty;
        if (!string.IsNullOrWhiteSpace(scalar))
        {
            yield return scalar.Trim();
        }
    }

    private static bool HasMeaningfulEffectFilterToken(Newtonsoft.Json.Linq.JToken? token)
    {
        return ReadStringArray(token).Any();
    }

    private static void TrackUnsupportedEffectFilterParams(
        string powerFullName,
        Newtonsoft.Json.Linq.JObject parameters,
        OmniApplyResult? applyResult)
    {
        if (applyResult == null)
        {
            return;
        }

        var unsupportedKeys = new[]
        {
            "power_names",
            "power_name",
            "powers",
            "powerset_names",
            "powersets",
            "category_names",
            "categories"
        };
        var present = unsupportedKeys
            .Where(key => parameters.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out var token) &&
                          token is { Type: not Newtonsoft.Json.Linq.JTokenType.Null } &&
                          HasMeaningfulEffectFilterToken(token))
            .ToArray();
        if (present.Length == 0)
        {
            return;
        }

        applyResult.UnsupportedEffectFilters++;
        applyResult.AddLimited(applyResult.UnsupportedEffectFilterDetails,
            $"{powerFullName}: EffectFilter has report-only filters {string.Join(", ", present)}");
    }

    private static void ApplyModePayload(Effect effect, OmniEffectTemplate template, OmniApplyResult? applyResult)
    {
        if (effect.EffectType is not (Enums.eEffectType.SetMode or Enums.eEffectType.UnsetMode))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(template.ModeName))
        {
            effect.ModeName = OmniModeMapper.Normalize(template.ModeName);
            if (OmniModeMapper.TryFromModeName(effect.ModeName, out var resolvedModeId, out var resolvedFlag))
            {
                effect.ModeId = resolvedModeId;
                effect.ModeFlag = resolvedFlag;
            }
            else if (OmniModeMapper.TryToFlag(effect.ModeName, out var flag))
            {
                effect.ModeFlag = flag;
            }

            return;
        }

        var modeId = (int)MathF.Round(template.Magnitude);
        effect.ModeId = modeId;
        if (OmniModeMapper.TryFromModeId(modeId, out var mappedName, out var mappedFlag))
        {
            effect.ModeName = mappedName;
            effect.ModeFlag = mappedFlag;
            return;
        }

        effect.ModeName = $"Mode {modeId}";
        applyResult?.AddLimited(applyResult.UnknownModeDetails,
            $"{effect.PowerFullName}: {effect.EffectType} mode id {modeId} was preserved without a known Mids flag.");
        if (applyResult != null)
        {
            applyResult.UnknownModesPreserved++;
        }
    }

    private static Enums.eAspect MapAspect(string aspect)
    {
        return aspect.ToLowerInvariant() switch
        {
            "cur" or "current" => Enums.eAspect.Cur,
            "max" or "maximum" => Enums.eAspect.Max,
            "res" or "resistance" => Enums.eAspect.Res,
            "abs" or "absolute" => Enums.eAspect.Abs,
            "str" or "strength" => Enums.eAspect.Str,
            _ => Enums.eAspect.Str
        };
    }

    private static void TrackPvTargetAudit(
        OmniPowerDefinition power,
        OmniEffectDefinition source,
        OmniEffectTemplate template,
        Effect effect,
        string pvModeSource,
        OmniApplyResult? applyResult)
    {
        if (applyResult == null)
        {
            return;
        }

        var targetsPlayer = TargetsEntity(source.RequiresExpression, "player") || TargetsEntity(template.JitRequires, "player");
        var targetsCritter = TargetsEntity(source.RequiresExpression, "critter") || TargetsEntity(template.JitRequires, "critter");

        switch (pvModeSource)
        {
            case "target entity expression":
                applyResult.PvModeInferredFromTargetEntity++;
                break;
            case "modifier table":
                applyResult.PvModeInferredFromTable++;
                break;
            case "ambiguous target entity expression":
                applyResult.PvModeAmbiguous++;
                break;
        }

        if ((targetsPlayer && effect.PvMode != Enums.ePvX.PvP) ||
            (targetsCritter && effect.PvMode != Enums.ePvX.PvE))
        {
            applyResult.PvTargetMappingMismatches++;
            applyResult.AddLimited(applyResult.PvTargetMappingMismatchDetails,
                $"{effect.OmniSource}: target_enttype player={targetsPlayer} critter={targetsCritter}, PvMode={effect.PvMode}, ToWho={effect.ToWho}, requires='{source.RequiresExpression}', jit='{template.JitRequires}'");
        }

        if (!IsFocusedPvTargetAuditPower(power.FullName))
        {
            return;
        }

        applyResult.PvTargetAuditEntries++;
        applyResult.AddLimited(applyResult.PvTargetGatingAuditDetails,
            $"{effect.OmniSource}: template.target='{template.Target}', targets_affected='{string.Join(", ", power.TargetsAffected)}', requires='{source.RequiresExpression}', jit='{template.JitRequires}', ToWho={effect.ToWho}, PvMode={effect.PvMode} ({pvModeSource}), conditions={FormatConditionRows(effect.AdvancedConditions)}");
    }

    private static bool IsFocusedPvTargetAuditPower(string fullName)
    {
        var normalized = Normalize(fullName);
        return normalized.Contains("enflame", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("rainofarrows", StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatConditionRows(AdvancedConditionSet? conditions)
    {
        if (conditions is not { Rows.Count: > 0 })
        {
            return "none";
        }

        return string.Join("; ", conditions.Rows.Select(row =>
            $"{row.Link}:{row.Kind}:{row.Subject}{row.Operator}{row.Value}:mode={row.EvaluationMode}:unsupported={row.Unsupported}"));
    }

    private static Enums.ePvX MapPvMode(
        Enums.eEffectType mappedType,
        string effectRequires,
        OmniEffectTemplate template,
        out string inferenceSource)
    {
        var targetsPlayer = TargetsEntity(effectRequires, "player") || TargetsEntity(template.JitRequires, "player");
        var targetsCritter = TargetsEntity(effectRequires, "critter") || TargetsEntity(template.JitRequires, "critter");
        if (targetsPlayer && !targetsCritter)
        {
            inferenceSource = "target entity expression";
            return Enums.ePvX.PvP;
        }

        if (targetsCritter && !targetsPlayer)
        {
            inferenceSource = "target entity expression";
            return Enums.ePvX.PvE;
        }

        if (targetsPlayer && targetsCritter)
        {
            inferenceSource = "ambiguous target entity expression";
            return Enums.ePvX.Any;
        }

        var table = Normalize(template.Table);
        if (table.Contains("pvp", StringComparison.OrdinalIgnoreCase))
        {
            inferenceSource = "modifier table";
            return Enums.ePvX.PvP;
        }

        if (table.Contains("pve", StringComparison.OrdinalIgnoreCase))
        {
            inferenceSource = "modifier table";
            return Enums.ePvX.PvE;
        }

        inferenceSource = mappedType == Enums.eEffectType.Damage
            ? "no explicit PvX split"
            : string.Empty;
        return Enums.ePvX.Any;
    }

    private static bool TargetsEntity(string expression, string entity)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return false;
        }

        var escaped = Regex.Escape(entity);
        return Regex.IsMatch(
                   expression,
                   $@"target\s*>\s*enttype\s*(?:eq|==)\s*['""]?{escaped}['""]?",
                   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) ||
               Regex.IsMatch(
                   expression,
                   $@"['""]?{escaped}['""]?\s+target\s*>\s*enttype\s*(?:eq|==)",
                   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    private static Enums.eEffectType MapEffectType(
        OmniPowerDefinition power,
        string type,
        string attrib,
        string aspect,
        string target,
        string table,
        TemplateSemantic templateSemantic)
    {
        var normalizedType = Normalize(type);
        var normalizedAttrib = Normalize(attrib);
        var normalizedAspect = Normalize(aspect);
        var normalizedTable = Normalize(table);
        var normalizedTemplateTarget = Normalize(target);
        var isStrengthAspect = normalizedAspect is "str" or "strength";
        var isResistanceAspect = normalizedAspect is "res" or "resistance";
        var isDamageModifierTable =
            normalizedTable.Contains("buffdmg", StringComparison.OrdinalIgnoreCase) ||
            normalizedTable.Contains("damage", StringComparison.OrdinalIgnoreCase) ||
            normalizedTable.EndsWith("dmg", StringComparison.OrdinalIgnoreCase);
        var isDefenseModifierTable =
            normalizedTable.Contains("buffdef", StringComparison.OrdinalIgnoreCase) ||
            normalizedTable.Contains("defense", StringComparison.OrdinalIgnoreCase) ||
            normalizedTable.EndsWith("def", StringComparison.OrdinalIgnoreCase);
        var isResistanceModifierTable =
            normalizedTable.Contains("resdmg", StringComparison.OrdinalIgnoreCase) ||
            normalizedTable.Contains("resistance", StringComparison.OrdinalIgnoreCase) ||
            normalizedTable.EndsWith("res", StringComparison.OrdinalIgnoreCase);
        var isActualDamageTemplate =
            normalizedAttrib.EndsWith("dmg", StringComparison.OrdinalIgnoreCase) ||
            normalizedAttrib.EndsWith("damage", StringComparison.OrdinalIgnoreCase) ||
            normalizedType == "damage" ||
            (normalizedType == "magnitude" &&
             !normalizedTemplateTarget.Contains("self", StringComparison.OrdinalIgnoreCase) &&
             isDamageModifierTable &&
             !isStrengthAspect &&
             !isResistanceAspect);

        if (normalizedType == "powerredirect")
        {
            return Enums.eEffectType.PowerRedirect;
        }

        if (normalizedType == "grantpower")
        {
            return Enums.eEffectType.GrantPower;
        }

        if (normalizedType == "grantboostedpower")
        {
            return Enums.eEffectType.GrantPower;
        }

        if (normalizedType == "entcreate")
        {
            return Enums.eEffectType.EntCreate;
        }

        if (normalizedAttrib is "grantpower")
        {
            return Enums.eEffectType.GrantPower;
        }

        if (normalizedAttrib is "grantboostedpower")
        {
            return Enums.eEffectType.GrantPower;
        }

        if (normalizedAttrib is "executepower")
        {
            return Enums.eEffectType.ExecutePower;
        }

        if (normalizedAttrib is "createentity")
        {
            return Enums.eEffectType.EntCreate;
        }

        if (normalizedAttrib is "revokepower")
        {
            return Enums.eEffectType.RevokePower;
        }

        if (normalizedType is "globalchancemod" or "powerchancemod")
        {
            return Enums.eEffectType.GlobalChanceMod;
        }

        if (normalizedAttrib is "globalchancemod" or "powerchancemod")
        {
            return Enums.eEffectType.GlobalChanceMod;
        }

        if (normalizedAttrib is "setmode")
        {
            return Enums.eEffectType.SetMode;
        }

        if (normalizedAttrib is "unsetmode")
        {
            return Enums.eEffectType.UnsetMode;
        }

        if (normalizedAttrib is "setcostume")
        {
            return Enums.eEffectType.SetCostume;
        }

        if (normalizedAttrib is "null" or "canceleffects")
        {
            return Enums.eEffectType.Null;
        }

        if (normalizedAttrib is "rechargepower")
        {
            return Enums.eEffectType.RechargePower;
        }

        if (normalizedAttrib is "healdmg" or "heal")
        {
            return Enums.eEffectType.Heal;
        }

        if (templateSemantic == TemplateSemantic.VectorDefense &&
            IsVectorDefenseSemanticAttrib(normalizedAttrib))
        {
            return Enums.eEffectType.Defense;
        }

        if (templateSemantic == TemplateSemantic.VectorResistance &&
            IsVectorResistanceSemanticAttrib(normalizedAttrib))
        {
            return Enums.eEffectType.Resistance;
        }

        if (isDefenseModifierTable)
        {
            return Enums.eEffectType.Defense;
        }

        if (isResistanceModifierTable || isResistanceAspect)
        {
            return Enums.eEffectType.Resistance;
        }

        if (IsDamageCategory(normalizedAttrib))
        {
            if (isStrengthAspect || isDamageModifierTable && normalizedTemplateTarget.Contains("self", StringComparison.OrdinalIgnoreCase))
            {
                return Enums.eEffectType.DamageBuff;
            }

            return isActualDamageTemplate ? Enums.eEffectType.Damage : Enums.eEffectType.None;
        }

        if (normalizedAttrib.EndsWith("dmg", StringComparison.OrdinalIgnoreCase) ||
            normalizedAttrib.EndsWith("damage", StringComparison.OrdinalIgnoreCase))
        {
            if (isStrengthAspect)
            {
                return Enums.eEffectType.DamageBuff;
            }

            return Enums.eEffectType.Damage;
        }

        if (normalizedAttrib.EndsWith("defense", StringComparison.OrdinalIgnoreCase))
        {
            return Enums.eEffectType.Defense;
        }

        if (normalizedAttrib.EndsWith("resistance", StringComparison.OrdinalIgnoreCase) ||
            normalizedAttrib.EndsWith("res", StringComparison.OrdinalIgnoreCase))
        {
            return Enums.eEffectType.Resistance;
        }

        if (IsMezAttrib(normalizedAttrib) || normalizedType == "knock")
        {
            return Enums.eEffectType.Mez;
        }

        return normalizedAttrib switch
        {
            "hitpoints" => Enums.eEffectType.HitPoints,
            "absorb" => Enums.eEffectType.Absorb,
            "viewattributes" => Enums.eEffectType.ViewAttrib,
            "endurance" => Enums.eEffectType.Endurance,
            "endurancecost" or "endurancediscount" => Enums.eEffectType.EnduranceDiscount,
            "interrupttime" => Enums.eEffectType.InterruptTime,
            "tohit" => Enums.eEffectType.ToHit,
            "accuracy" => Enums.eEffectType.Accuracy,
            "defense" => Enums.eEffectType.Defense,
            "damage" => Enums.eEffectType.Damage,
            "damagebuff" => Enums.eEffectType.DamageBuff,
            "elusivitybase" => Enums.eEffectType.Elusivity,
            "levelshift" => Enums.eEffectType.LevelShift,
            "rechargetime" => Enums.eEffectType.RechargeTime,
            "recovery" => Enums.eEffectType.Recovery,
            "regeneration" => Enums.eEffectType.Regeneration,
            "resistance" => Enums.eEffectType.Resistance,
            "range" => Enums.eEffectType.Range,
            "runningspeed" => Enums.eEffectType.SpeedRunning,
            "speedrunning" => Enums.eEffectType.SpeedRunning,
            "flyingspeed" => Enums.eEffectType.SpeedFlying,
            "speedflying" => Enums.eEffectType.SpeedFlying,
            "jumpingspeed" => Enums.eEffectType.SpeedJumping,
            "speedjumping" => Enums.eEffectType.SpeedJumping,
            "fly" => Enums.eEffectType.Fly,
            "jumpheight" => Enums.eEffectType.JumpHeight,
            "perceptionradius" => Enums.eEffectType.PerceptionRadius,
            "stealthradiuspve" => Enums.eEffectType.StealthRadius,
            "stealthradiuspvp" => Enums.eEffectType.StealthRadiusPlayer,
            "threatlevel" => Enums.eEffectType.ThreatLevel,
            "stealthradius" => Enums.eEffectType.StealthRadius,
            "stealthradiusplayer" => Enums.eEffectType.StealthRadiusPlayer,
            "slow" => Enums.eEffectType.Slow,
            "movementcontrol" => Enums.eEffectType.MovementControl,
            "movementfriction" => Enums.eEffectType.MovementFriction,
            "translucency" => Enums.eEffectType.Translucency,
            "rage" => Enums.eEffectType.Rage,
            "onlyaffectsself" => Enums.eEffectType.Mez,
            "untouchable" => Enums.eEffectType.Mez,
            "intangible" => Enums.eEffectType.Mez,
            "teleport" => Enums.eEffectType.Mez,
            "afraid" => Enums.eEffectType.Mez,
            "revoke" or "revokepower" => Enums.eEffectType.RevokePower,
            "reward" => Enums.eEffectType.Reward,
            "setmode" => Enums.eEffectType.SetMode,
            "unsetmode" => Enums.eEffectType.UnsetMode,
            "meter" => Enums.eEffectType.Meter,
            "debtprotection" => Enums.eEffectType.XPDebtProtection,
            "addtoken" => Enums.eEffectType.TokenAdd,
            _ => Enum.TryParse<Enums.eEffectType>(normalizedAttrib, true, out var parsedAttrib)
                ? parsedAttrib
                : Enum.TryParse<Enums.eEffectType>(type, true, out var parsedType)
                    ? parsedType
                    : Enums.eEffectType.None
        };
    }

    private static Enums.eAttribType MapAttribType(string type)
    {
        return Normalize(type) switch
        {
            "duration" => Enums.eAttribType.Duration,
            "expression" => Enums.eAttribType.Expression,
            _ => Enums.eAttribType.Magnitude
        };
    }

    private static void ApplyCombatModFlags(Effect effect, OmniEffectTemplate template)
    {
        effect.UseCombatModMagnitude = HasTemplateFlag(template, "CombatModMagnitude");
        effect.UseCombatModDuration = HasTemplateFlag(template, "CombatModDuration");
    }

    private static bool HasTemplateFlag(OmniEffectTemplate template, string flagName)
    {
        return template.Flags.Any(flag => Normalize(flag).Contains(Normalize(flagName), StringComparison.OrdinalIgnoreCase));
    }

    private static Enums.eSpecialCase MapSpecialCase(
        OmniPowerDefinition power,
        OmniEffectTemplate template,
        string attrib,
        Enums.eEffectType mappedType)
    {
        // New Omni imports use mode payloads/conditions for planner state. Keep
        // SpecialCase as a legacy database compatibility path, not imported truth.
        return Enums.eSpecialCase.None;
    }

    private static Enums.eDamage MapDamageType(string attrib, string table)
    {
        var normalizedAttrib = Normalize(attrib);
        if (normalizedAttrib is "basedefense" or "elusivitybase" or "baseresistance")
        {
            return Enums.eDamage.None;
        }

        switch (normalizedAttrib)
        {
            case "smashing":
                return Enums.eDamage.Smashing;
            case "lethal":
                return Enums.eDamage.Lethal;
            case "fire":
                return Enums.eDamage.Fire;
            case "cold":
                return Enums.eDamage.Cold;
            case "negativeenergy":
            case "negative":
                return Enums.eDamage.Negative;
            case "energy":
                return Enums.eDamage.Energy;
            case "toxic":
                return Enums.eDamage.Toxic;
            case "psionic":
            case "psi":
                return Enums.eDamage.Psionic;
            case "melee":
                return Enums.eDamage.Melee;
            case "ranged":
                return Enums.eDamage.Ranged;
            case "area":
            case "aoe":
                return Enums.eDamage.AoE;
        }

        var combined = $"{normalizedAttrib} {Normalize(table)}";
        if (combined.Contains("smashing", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.Smashing;
        if (combined.Contains("lethal", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.Lethal;
        if (combined.Contains("fire", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.Fire;
        if (combined.Contains("cold", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.Cold;
        if (combined.Contains("negativeenergy", StringComparison.OrdinalIgnoreCase) ||
            combined.Contains("negative", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.Negative;
        if (combined.Contains("energy", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.Energy;
        if (combined.Contains("toxic", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.Toxic;
        if (combined.Contains("psionic", StringComparison.OrdinalIgnoreCase) ||
            combined.Contains("psi", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.Psionic;
        if (combined.Contains("melee", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.Melee;
        if (combined.Contains("ranged", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.Ranged;
        if (combined.Contains("aoe", StringComparison.OrdinalIgnoreCase) ||
            combined.Contains("area", StringComparison.OrdinalIgnoreCase)) return Enums.eDamage.AoE;
        return Enums.eDamage.None;
    }

    private static Enums.eMez MapMezType(string attrib, string type)
    {
        var combined = $"{Normalize(attrib)} {Normalize(type)}";
        if (combined.Contains("confus", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Confused;
        if (combined.Contains("hold", StringComparison.OrdinalIgnoreCase) ||
            combined.Contains("held", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Held;
        if (combined.Contains("immobil", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Immobilized;
        if (combined.Contains("knockback", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Knockback;
        if (combined.Contains("knockup", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Knockup;
        if (combined.Contains("knock", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Knockback;
        if (combined.Contains("placate", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Placate;
        if (combined.Contains("repel", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Repel;
        if (combined.Contains("sleep", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Sleep;
        if (combined.Contains("stun", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Stunned;
        if (combined.Contains("taunt", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Taunt;
        if (combined.Contains("terror", StringComparison.OrdinalIgnoreCase) ||
            combined.Contains("fear", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Terrorized;
        if (combined.Contains("afraid", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Afraid;
        if (combined.Contains("onlyaffectsself", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.OnlyAffectsSelf;
        if (combined.Contains("untouchable", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Untouchable;
        if (combined.Contains("intangible", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Intangible;
        if (combined.Contains("teleport", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.Teleport;
        if (combined.Contains("phase", StringComparison.OrdinalIgnoreCase)) return Enums.eMez.CombatPhase;
        return Enums.eMez.None;
    }

    private static bool IsKnownUnsupportedEffectAttrib(
        OmniPowerDefinition power,
        OmniEffectDefinition source,
        OmniEffectTemplate template,
        string normalizedAttrib,
        string templateType,
        IReadOnlyCollection<string> effectTags)
    {
        if (normalizedAttrib is "scriptnotify" or "avoid" or "evade")
        {
            return true;
        }

        return IsHiddenStatefulVectorTemplate(power, source, template, effectTags) &&
               (IsDamageCategory(normalizedAttrib) ||
                IsVectorDefenseOrResistanceSelectorAttrib(normalizedAttrib));
    }

    private static bool IsKnownHiddenStatefulEffect(Effect effect)
    {
        return effect.EffectType == Enums.eEffectType.Rage ||
               effect is { EffectType: Enums.eEffectType.Mez, MezType: Enums.eMez.OnlyAffectsSelf or Enums.eMez.Untouchable or Enums.eMez.Intangible };
    }

    private static string DescribeStatefulEffect(Effect effect)
    {
        if (effect.EffectType == Enums.eEffectType.Mez)
        {
            return $"{effect.EffectType}:{effect.MezType}";
        }

        return effect.EffectType.ToString();
    }

    private static TemplateSemantic ClassifyTemplateSemantic(
        OmniPowerDefinition power,
        OmniEffectDefinition source,
        OmniEffectTemplate template,
        IReadOnlyCollection<string> effectTags)
    {
        var normalizedAttribs = (template.Attribs.Count == 0 ? [string.Empty] : template.Attribs)
            .Select(Normalize)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
        if (normalizedAttribs.Length == 0 || normalizedAttribs.Any(IsActualDamagePayloadAttrib))
        {
            return TemplateSemantic.None;
        }

        if (!normalizedAttribs.All(IsVectorDefenseOrResistanceSelectorAttrib))
        {
            return TemplateSemantic.None;
        }

        if (IsHiddenStatefulVectorTemplate(power, source, template, effectTags))
        {
            return TemplateSemantic.None;
        }

        if (HasDefenseEvidence(power, source, template, effectTags, normalizedAttribs))
        {
            return TemplateSemantic.VectorDefense;
        }

        if (HasResistanceEvidence(power, source, template, effectTags, normalizedAttribs))
        {
            return TemplateSemantic.VectorResistance;
        }

        var normalizedAspect = Normalize(template.Aspect);
        if (normalizedAttribs.Length > 1)
        {
            if (normalizedAspect is "cur" or "current")
            {
                return TemplateSemantic.VectorDefense;
            }

            if (normalizedAspect is "res" or "resistance")
            {
                return TemplateSemantic.VectorResistance;
            }
        }

        return TemplateSemantic.None;
    }

    private static bool HasPlannerDefenseOrResistanceEvidence(
        OmniPowerDefinition power,
        OmniEffectDefinition source,
        OmniEffectTemplate template,
        IReadOnlyCollection<string> effectTags,
        IReadOnlyCollection<string> normalizedAttribs)
    {
        return HasDefenseEvidence(power, source, template, effectTags, normalizedAttribs) ||
               HasResistanceEvidence(power, source, template, effectTags, normalizedAttribs);
    }

    private static bool HasDefenseEvidence(
        OmniPowerDefinition power,
        OmniEffectDefinition source,
        OmniEffectTemplate template,
        IReadOnlyCollection<string> effectTags,
        IReadOnlyCollection<string> normalizedAttribs)
    {
        var plannerText = GetPlannerSemanticText(power);
        if (normalizedAttribs.Contains("basedefense", StringComparer.OrdinalIgnoreCase) ||
            normalizedAttribs.Contains("defense", StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        if (CollectNormalizedTemplateTags(source, template, effectTags)
            .Contains("defense", StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        var normalizedTable = Normalize(template.Table);
        if (normalizedTable.Contains("buffdef", StringComparison.OrdinalIgnoreCase) ||
            normalizedTable.Contains("defense", StringComparison.OrdinalIgnoreCase) ||
            normalizedTable.EndsWith("def", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return plannerText.Contains("defense", StringComparison.OrdinalIgnoreCase) ||
               plannerText.Contains("def(", StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasResistanceEvidence(
        OmniPowerDefinition power,
        OmniEffectDefinition source,
        OmniEffectTemplate template,
        IReadOnlyCollection<string> effectTags,
        IReadOnlyCollection<string> normalizedAttribs)
    {
        var plannerText = GetPlannerSemanticText(power);
        if (normalizedAttribs.Contains("baseresistance", StringComparer.OrdinalIgnoreCase) ||
            normalizedAttribs.Contains("resistance", StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        var normalizedAspect = Normalize(template.Aspect);
        if (normalizedAspect is "res" or "resistance")
        {
            return true;
        }

        if (CollectNormalizedTemplateTags(source, template, effectTags)
            .Contains("resistance", StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        var normalizedTable = Normalize(template.Table);
        if (normalizedTable.Contains("resdmg", StringComparison.OrdinalIgnoreCase) ||
            normalizedTable.Contains("resistance", StringComparison.OrdinalIgnoreCase) ||
            normalizedTable.EndsWith("res", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return plannerText.Contains("resistance", StringComparison.OrdinalIgnoreCase) ||
               plannerText.Contains("resist", StringComparison.OrdinalIgnoreCase) ||
               plannerText.Contains("res(", StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyCollection<string> CollectNormalizedTemplateTags(
        OmniEffectDefinition source,
        OmniEffectTemplate template,
        IReadOnlyCollection<string> effectTags)
    {
        return MergeTags(source.Tags, source.Flags, template.Tags, effectTags)
            .Select(Normalize)
            .Where(static tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string GetPlannerSemanticText(OmniPowerDefinition power)
    {
        return Normalize(string.Join(" ",
            power.FullName,
            power.DisplayName,
            power.DisplayShortHelp,
            power.DisplayHelp));
    }

    private static bool IsHiddenStatefulVectorTemplate(
        OmniPowerDefinition power,
        OmniEffectDefinition source,
        OmniEffectTemplate template,
        IReadOnlyCollection<string> effectTags)
    {
        var normalizedFlagsAndTags = MergeTags(template.Flags, source.Tags, template.Tags, effectTags)
            .Select(Normalize)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
        if (normalizedFlagsAndTags.Any(flag =>
                flag.Contains("hidefrominfo", StringComparison.OrdinalIgnoreCase) ||
                flag.Contains("placate", StringComparison.OrdinalIgnoreCase) ||
                flag.Contains("scriptnotify", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        var plannerText = GetPlannerSemanticText(power);
        if (IsSupportOnlyStatefulVectorCarrier(power, plannerText))
        {
            return true;
        }

        var hasExplicitPlannerStatSemantic = plannerText.Contains("def(", StringComparison.OrdinalIgnoreCase) ||
                                             plannerText.Contains("res(", StringComparison.OrdinalIgnoreCase) ||
                                             plannerText.Contains("defense", StringComparison.OrdinalIgnoreCase) ||
                                             plannerText.Contains("resistance", StringComparison.OrdinalIgnoreCase);
        var normalizedTable = Normalize(template.Table);
        var sourceAttribs = source.Templates
            .SelectMany(item => item.Attribs)
            .Select(Normalize)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
        var hasStealthRadiusSibling = sourceAttribs.Any(attrib => attrib.StartsWith("stealthradius", StringComparison.OrdinalIgnoreCase));
        var hasTranslucencySibling = sourceAttribs.Contains("translucency", StringComparer.OrdinalIgnoreCase);
        var hasSetModeSibling = sourceAttribs.Contains("setmode", StringComparer.OrdinalIgnoreCase);
        var isStealthCarrierTable = normalizedTable.EndsWith("ones", StringComparison.OrdinalIgnoreCase);
        if (isStealthCarrierTable && hasStealthRadiusSibling && (hasTranslucencySibling || hasSetModeSibling))
        {
            return true;
        }

        return !hasExplicitPlannerStatSemantic &&
               (plannerText.Contains("placate", StringComparison.OrdinalIgnoreCase) ||
                plannerText.Contains("avoid", StringComparison.OrdinalIgnoreCase) ||
                plannerText.Contains("evade", StringComparison.OrdinalIgnoreCase) ||
                plannerText.Contains("stealth", StringComparison.OrdinalIgnoreCase) ||
                plannerText.Contains("hide", StringComparison.OrdinalIgnoreCase) ||
                plannerText.Contains("transluc", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsSupportOnlyStatefulVectorCarrier(OmniPowerDefinition power, string plannerText)
    {
        var normalizedPowerName = Normalize(power.FullName);
        return normalizedPowerName.Contains("battleeuphoria", StringComparison.OrdinalIgnoreCase) ||
               normalizedPowerName.Contains("defianceold", StringComparison.OrdinalIgnoreCase) ||
               normalizedPowerName.Contains("defiancebuff", StringComparison.OrdinalIgnoreCase) ||
               normalizedPowerName.Contains("furybuff", StringComparison.OrdinalIgnoreCase) ||
               normalizedPowerName.Contains("opportunitymeter", StringComparison.OrdinalIgnoreCase) ||
               normalizedPowerName.Contains("primalenergymeter", StringComparison.OrdinalIgnoreCase) ||
               normalizedPowerName.Contains("ragebuff", StringComparison.OrdinalIgnoreCase) ||
               normalizedPowerName.Contains("supremacy", StringComparison.OrdinalIgnoreCase) ||
               normalizedPowerName.Contains("teleportfoe", StringComparison.OrdinalIgnoreCase) ||
               plannerText.Contains("opportunitymeter", StringComparison.OrdinalIgnoreCase) ||
               plannerText.Contains("battleeuphoria", StringComparison.OrdinalIgnoreCase) ||
               plannerText.Contains("primalenergy", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsActualDamagePayloadAttrib(string normalizedAttrib)
    {
        return normalizedAttrib is "damage" or "damagebuff" ||
               normalizedAttrib.EndsWith("dmg", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.EndsWith("damage", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsVectorDefenseOrResistanceSelectorAttrib(string normalizedAttrib)
    {
        return normalizedAttrib is "basedefense" or "baseresistance" or "defense" or "resistance" ||
               IsDamageCategory(normalizedAttrib);
    }

    private static bool IsVectorDefenseSemanticAttrib(string normalizedAttrib)
    {
        return normalizedAttrib is "basedefense" or "defense" || IsDamageCategory(normalizedAttrib);
    }

    private static bool IsVectorResistanceSemanticAttrib(string normalizedAttrib)
    {
        return normalizedAttrib is "baseresistance" or "resistance" || IsDamageCategory(normalizedAttrib);
    }

    private static string DescribeTemplateSemantic(TemplateSemantic templateSemantic)
    {
        return templateSemantic switch
        {
            TemplateSemantic.VectorDefense => "VectorDefense",
            TemplateSemantic.VectorResistance => "VectorResistance",
            _ => "None"
        };
    }

    private static bool IsMezAttrib(string normalizedAttrib)
    {
        return normalizedAttrib.Contains("confus", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("hold", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("held", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("immobil", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("knock", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("placate", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("repel", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("sleep", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("stun", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("taunt", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("afraid", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("teleport", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("terror", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.Contains("fear", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDamageCategory(string normalizedAttrib)
    {
        return normalizedAttrib is
            "smashing" or
            "lethal" or
            "fire" or
            "cold" or
            "energy" or
            "negativeenergy" or
            "negative" or
            "toxic" or
            "psionic" or
            "psi" or
            "melee" or
            "ranged" or
            "area" or
            "aoe";
    }

    private static bool IsDamageAttrib(string attrib)
    {
        var normalizedAttrib = Normalize(attrib);
        return IsDamageCategory(normalizedAttrib) ||
               normalizedAttrib.EndsWith("dmg", StringComparison.OrdinalIgnoreCase) ||
               normalizedAttrib.EndsWith("damage", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsBlasterDefianceDamageStrength(
        OmniPowerDefinition power,
        string attrib,
        string aspect,
        string target,
        string table)
    {
        var normalizedAspect = Normalize(aspect);
        if (normalizedAspect is not ("str" or "strength") || !IsDamageAttrib(attrib))
        {
            return false;
        }

        var normalizedTarget = Normalize(target);
        if (!normalizedTarget.Contains("self", StringComparison.OrdinalIgnoreCase) &&
            !normalizedTarget.Contains("caster", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var normalizedTable = Normalize(table);
        if (!normalizedTable.EndsWith("ones", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return power.FullName.StartsWith("Blaster_", StringComparison.OrdinalIgnoreCase) ||
               power.Archetypes.Any(a => a.Equals("blaster", StringComparison.OrdinalIgnoreCase));
    }

    private static Enums.eToWho MapToWho(OmniPowerDefinition power, string templateTarget)
    {
        var target = Normalize(templateTarget);
        if (target.Contains("caster", StringComparison.OrdinalIgnoreCase) ||
            target.Contains("self", StringComparison.OrdinalIgnoreCase))
        {
            return Enums.eToWho.Self;
        }

        if (target.Contains("all", StringComparison.OrdinalIgnoreCase))
        {
            return Enums.eToWho.All;
        }

        if (target.Contains("target", StringComparison.OrdinalIgnoreCase) ||
            target.Contains("anyaffected", StringComparison.OrdinalIgnoreCase) ||
            target.Contains("affected", StringComparison.OrdinalIgnoreCase))
        {
            return Enums.eToWho.Target;
        }

        return PowerTargetsSelf(power) ? Enums.eToWho.Self : Enums.eToWho.Target;
    }

    private static bool PowerTargetsSelf(OmniPowerDefinition power)
    {
        var target = Normalize(power.TargetType);
        return target.Contains("self", StringComparison.OrdinalIgnoreCase) ||
               target.Contains("caster", StringComparison.OrdinalIgnoreCase) ||
               (power.TargetsAffected.Count > 0 &&
                power.TargetsAffected.All(t =>
                    Normalize(t).Contains("self", StringComparison.OrdinalIgnoreCase) ||
                    Normalize(t).Contains("caster", StringComparison.OrdinalIgnoreCase)));
    }

    private static string Normalize(string value)
    {
        return (value ?? string.Empty)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace(".", string.Empty, StringComparison.Ordinal)
            .Trim()
            .ToLowerInvariant();
    }
}

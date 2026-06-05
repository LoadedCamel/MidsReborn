namespace Mids_Reborn.Core.Omni;

public enum OmniRedirectKind
{
    None,
    ExecutionVariant,
    ModeVariant,
    BuffVariant,
    SummonDelivery,
    Unknown
}

public enum OmniScopedPowerDisposition
{
    MainImportVisible,
    MainImportHidden,
    PetManifestOwned,
    Excluded
}

public sealed class OmniPowerClassification
{
    public Enums.ePowerType PowerType { get; set; } = Enums.ePowerType.Click;
    public bool ClickBuff { get; set; }
    public bool AlwaysToggle { get; set; }
    public bool HiddenPower { get; set; }
    public bool IncludeFlag { get; set; }
    public Enums.eGridType InherentType { get; set; } = Enums.eGridType.None;
    public bool ExecutionOnly { get; set; }
    public bool GrantedSupportPower { get; set; }
    public bool NormalBuildPick { get; set; }
    public OmniRedirectKind RedirectKind { get; set; } = OmniRedirectKind.None;
    public OmniScopedPowerDisposition ScopedDisposition { get; set; } = OmniScopedPowerDisposition.MainImportVisible;
    public float ClassificationConfidence { get; set; } = 0.75f;
    public List<string> Reasons { get; } = [];

    public string Summary(string fullName)
    {
        var reasons = Reasons.Count == 0 ? "no specific reason recorded" : string.Join("; ", Reasons);
        return $"{fullName}: PowerType={PowerType}, ClickBuff={ClickBuff}, HiddenPower={HiddenPower}, IncludeFlag={IncludeFlag}, InherentType={InherentType}, NormalBuildPick={NormalBuildPick}, RedirectKind={RedirectKind}, ScopedDisposition={ScopedDisposition}, Confidence={ClassificationConfidence:0.00} ({reasons})";
    }
}

public sealed class OmniPowerClassifier
{
    private static readonly HashSet<string> VisibleInherentNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Brawl",
        "Sprint",
        "Rest",
        "Health",
        "Hurdle",
        "Swift",
        "Stamina"
    };

    private static readonly HashSet<string> DefaultOffVisibleInherentToggleNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Rest"
    };

    private static readonly HashSet<string> BuildRelevantClassInherents = new(StringComparer.OrdinalIgnoreCase)
    {
        "Assassination",
        "Spider_Conditioning",
        "Widow_Conditioning",
        "Defiance",
        "Containment",
        "Cosmic_Balance",
        "Critical_Hit",
        "Dark_Sustenance",
        "Domination",
        "Rage",
        "Fury",
        "Gauntlet",
        "Inherent_Gauntlet",
        "Scourge",
        "Opportunity",
        "Assassin_Strike_Inherent",
        "Vigilance",
        "Supremacy"
    };

    private static readonly string[] OffensiveAttribFragments =
    [
        "damage",
        "smashing",
        "lethal",
        "fire",
        "cold",
        "energy",
        "negative",
        "psionic",
        "toxic",
        "taunt",
        "knock",
        "hold",
        "sleep",
        "stun",
        "immobil",
        "terror",
        "confuse",
        "placate"
    ];

    private static readonly string[] BuffAttribFragments =
    [
        "defense",
        "resistance",
        "res",
        "damage",
        "dmg",
        "tohit",
        "accuracy",
        "recharge",
        "recovery",
        "regeneration",
        "endurance",
        "elusivity",
        "hit_points",
        "hitpoints",
        "absorb",
        "movement",
        "jump",
        "fly",
        "speed",
        "perception",
        "protection",
        "stealth",
        "range",
        "mez"
    ];

    public OmniPowerClassification Classify(
        OmniPowerDefinition power,
        IReadOnlyDictionary<string, OmniPowerDefinition> scopedPowersByFullName)
    {
        var classification = new OmniPowerClassification
        {
            PowerType = MapPowerType(power.Type),
            HiddenPower = false,
            IncludeFlag = false,
            InherentType = Enums.eGridType.None
        };

        var group = GroupNamePart(power.FullName);
        var set = SetNamePart(power.FullName);
        var name = LastNamePart(power.FullName);
        var isTemporaryPower = group.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase);
        var offensive = IsOffensivePower(power);
        var hasModeRequirement = power.ModesRequired.Count > 0;
        var sameSetPowerRequirement = TryGetSinglePowerRequirement(power, out var parentPowerName) &&
                                      SamePowerset(power.FullName, parentPowerName);

        classification.RedirectKind = ClassifyRedirects(power, scopedPowersByFullName);
        classification.ExecutionOnly = classification.RedirectKind is OmniRedirectKind.ExecutionVariant or OmniRedirectKind.SummonDelivery;
        var visibleInherent = IsVisibleInherent(power, name, set);
        var defaultOffVisibleInherentToggle = visibleInherent &&
                                              MapPowerType(power.Type) == Enums.ePowerType.Toggle &&
                                              DefaultOffVisibleInherentToggleNames.Contains(name);
        var normalBuildPick = IsNormalBuildPickCandidate(
            group,
            power,
            hasModeRequirement,
            sameSetPowerRequirement,
            classification.ExecutionOnly);
        classification.NormalBuildPick = normalBuildPick;
        var grantedInherentType = DetermineGrantedSupportInherentType(
            group,
            power,
            sameSetPowerRequirement,
            visibleInherent,
            isTemporaryPower,
            hasModeRequirement,
            classification.ExecutionOnly);
        classification.GrantedSupportPower =
            (power.AutoIssue && !power.ShowInManage && !visibleInherent) ||
            (power.AutoIssue && !visibleInherent && !group.Equals("Inherent", StringComparison.OrdinalIgnoreCase) && !isTemporaryPower) ||
            (power.AutoIssue && sameSetPowerRequirement) ||
            (hasModeRequirement && power.AutoIssue);

        if (classification.PowerType == Enums.ePowerType.Click)
        {
            classification.ClickBuff = IsClickBuff(power, scopedPowersByFullName, offensive);
        }

        if (offensive)
        {
            classification.ClickBuff = false;
            classification.Reasons.Add("offensive target/effect profile");
        }

        if (classification.RedirectKind == OmniRedirectKind.ExecutionVariant)
        {
            classification.ClickBuff = false;
            classification.Reasons.Add("redirects are execution variants, not buff state");
        }

        if (visibleInherent)
        {
            var classInherent = IsClassInherent(power, name);
            classification.HiddenPower = false;
            classification.GrantedSupportPower = false;
            classification.IncludeFlag = true;
            classification.InherentType = classInherent
                ? Enums.eGridType.Class
                : Enums.eGridType.Inherent;
            if (classInherent)
            {
                classification.PowerType = Enums.ePowerType.Auto_;
                classification.ClickBuff = false;
                classification.AlwaysToggle = true;
            }

            classification.Reasons.Add("visible build inherent");
        }
        else if (group.Equals("Inherent", StringComparison.OrdinalIgnoreCase))
        {
            classification.HiddenPower = true;
            classification.IncludeFlag = false;
            classification.Reasons.Add("non-build inherent support/detail power");
        }

        if (classification.PowerType == Enums.ePowerType.Auto_)
        {
            classification.AlwaysToggle = true;
        }
        else if (classification.PowerType == Enums.ePowerType.Toggle)
        {
            classification.AlwaysToggle = !offensive &&
                                          !classification.GrantedSupportPower &&
                                          !defaultOffVisibleInherentToggle;
            if (defaultOffVisibleInherentToggle)
            {
                classification.Reasons.Add("visible inherent toggle defaults off");
            }
        }

        if (!power.ShowInManage && !isTemporaryPower && !normalBuildPick)
        {
            classification.HiddenPower = !visibleInherent;
            classification.Reasons.Add("show_in_manage=false");
        }
        else if (!power.ShowInManage && normalBuildPick)
        {
            classification.Reasons.Add("show_in_manage=false but normal build powerset pick");
        }

        if (classification.GrantedSupportPower && !visibleInherent)
        {
            classification.HiddenPower = true;
            classification.IncludeFlag = power.AutoIssue && !group.Equals("Inherent", StringComparison.OrdinalIgnoreCase) && !isTemporaryPower;
            classification.InherentType = classification.IncludeFlag
                ? grantedInherentType != Enums.eGridType.None
                    ? grantedInherentType
                    : power.ShowInManage
                        ? Enums.eGridType.Powerset
                        : Enums.eGridType.None
                : Enums.eGridType.None;
            classification.Reasons.Add(classification.InherentType switch
            {
                Enums.eGridType.Power => "auto-issued power-granted support shown in inherent grid",
                Enums.eGridType.Powerset => "auto-issued powerset-granted support shown in inherent grid",
                _ => "auto-issued support/granted power"
            });
        }

        if (hasModeRequirement && IsSupportHeavyGroup(group) && !visibleInherent)
        {
            classification.HiddenPower = true;
            classification.IncludeFlag = false;
            classification.Reasons.Add("mode-gated support power");
        }

        if (isTemporaryPower)
        {
            classification.InherentType = set.Equals("Accolades", StringComparison.OrdinalIgnoreCase)
                ? Enums.eGridType.Accolade
                : Enums.eGridType.Temp;
            classification.HiddenPower = false;
            classification.IncludeFlag = true;
            classification.Reasons.Add("temporary/accolade database power");
        }

        if (IsExplicitlyRetainedTemporaryVisiblePower(group, set, name))
        {
            classification.HiddenPower = false;
            classification.IncludeFlag = true;
            classification.InherentType = Enums.eGridType.Temp;
            classification.GrantedSupportPower = false;
            classification.ExecutionOnly = false;
            classification.NormalBuildPick = false;
            classification.Reasons.Add("retained temporary visible power");
        }

        if (IsExplicitlyRetainedTemporarySupportPower(group, set, name))
        {
            classification.HiddenPower = true;
            classification.IncludeFlag = true;
            classification.InherentType = Enums.eGridType.Temp;
            classification.GrantedSupportPower = true;
            classification.ExecutionOnly = false;
            classification.NormalBuildPick = false;
            classification.ClickBuff = false;
            classification.Reasons.Add("retained temporary support/monitor power");
        }

        if (IsPowersetGrantedTemporaryState(power, group, set, name))
        {
            classification.HiddenPower = true;
            classification.IncludeFlag = true;
            classification.InherentType = Enums.eGridType.Powerset;
            classification.GrantedSupportPower = true;
            classification.ExecutionOnly = false;
            classification.NormalBuildPick = false;
            classification.ClickBuff = false;
            classification.Reasons.Add("powerset-granted temporary state shown in inherent grid");
        }

        if (string.Equals(power.Requires?.Trim(), "0", StringComparison.OrdinalIgnoreCase) &&
            (power.AutoIssue || IsSupportHeavyGroup(group)) &&
            !IsVisibleInherent(power, name, set) &&
            !IsExplicitlyRetainedTemporaryPower(group, set, name))
        {
            classification.HiddenPower = true;
            classification.IncludeFlag = false;
            classification.Reasons.Add("disabled/manual support requirement");
        }

        if (IsRedirectOrPetExecutionGroup(group))
        {
            classification.HiddenPower = true;
            classification.ExecutionOnly = true;
            classification.IncludeFlag = false;
            classification.Reasons.Add("execution/support powerset group");
        }

        if (!classification.HiddenPower && !classification.IncludeFlag && HasBuildPrerequisite(power))
        {
            classification.Reasons.Add("visible gated build pick");
        }

        if (classification.HiddenPower && classification.IncludeFlag && !classification.GrantedSupportPower)
        {
            classification.IncludeFlag = false;
        }

        if (normalBuildPick && classification.HiddenPower)
        {
            classification.HiddenPower = false;
            classification.IncludeFlag = false;
            classification.InherentType = Enums.eGridType.None;
            classification.GrantedSupportPower = false;
            classification.ExecutionOnly = false;
            classification.Reasons.Add("restored visible normal build powerset pick");
        }

        if (CompositePowersetRules.ShouldForceVisibleOnImport(power.FullName))
        {
            classification.HiddenPower = false;
            classification.IncludeFlag = false;
            classification.InherentType = Enums.eGridType.None;
            classification.GrantedSupportPower = false;
            classification.ExecutionOnly = false;
            classification.NormalBuildPick = true;
            classification.Reasons.Add("forced visible composite starter power");
        }

        if (IsPowersetPlannerControl(power, group, name))
        {
            classification.HiddenPower = true;
            classification.IncludeFlag = true;
            classification.InherentType = Enums.eGridType.Power;
            classification.GrantedSupportPower = true;
            classification.ExecutionOnly = false;
            classification.NormalBuildPick = false;
            classification.ClickBuff = false;
            classification.Reasons.Add("power-dependent planner control shown in inherent grid");
        }

        if (IsIncarnateSilentPlannerControl(power, group, set))
        {
            classification.HiddenPower = true;
            classification.IncludeFlag = true;
            classification.InherentType = Enums.eGridType.Incarnate;
            classification.GrantedSupportPower = true;
            classification.ExecutionOnly = false;
            classification.NormalBuildPick = false;
            classification.Reasons.Add("planner-relevant incarnate silent control shown in incarnate grid");
        }

        if (IsIncarnateSocketPlannerControl(power, group, set))
        {
            classification.HiddenPower = true;
            classification.IncludeFlag = true;
            classification.InherentType = Enums.eGridType.Incarnate;
            classification.GrantedSupportPower = true;
            classification.ExecutionOnly = false;
            classification.NormalBuildPick = false;
            classification.ClickBuff = false;
            classification.Reasons.Add("planner-relevant incarnate socket support shown hidden in incarnate grid");
        }

        if (IsVisibleIncarnateBuildChoice(power, classification, group, set, name))
        {
            classification.HiddenPower = false;
            classification.IncludeFlag = true;
            classification.InherentType = Enums.eGridType.Incarnate;
            classification.GrantedSupportPower = false;
            classification.ExecutionOnly = false;
            classification.NormalBuildPick = false;
            if (ShouldForceVisibleIncarnateClickBuff(power, classification, set, offensive))
            {
                classification.ClickBuff = true;
                classification.Reasons.Add("visible incarnate support click treated as click-buff");
            }

            classification.Reasons.Add("visible incarnate build choice shown in incarnate grid");
        }

        if (IsTemporarySilentSupportPower(group, set))
        {
            classification.HiddenPower = true;
            classification.IncludeFlag = true;
            classification.InherentType = Enums.eGridType.Temp;
            classification.GrantedSupportPower = true;
            classification.ExecutionOnly = false;
            classification.NormalBuildPick = false;
            classification.Reasons.Add("planner-relevant temporary silent support/state power retained hidden");
        }

        if (IsRetainedPrestigeVisiblePower(group, set))
        {
            classification.HiddenPower = false;
            classification.IncludeFlag = true;
            classification.InherentType = Enums.eGridType.Prestige;
            classification.GrantedSupportPower = false;
            classification.ExecutionOnly = false;
            classification.NormalBuildPick = false;
            classification.Reasons.Add("retained prestige database power");
        }

        if (!classification.HiddenPower && offensive && classification.PowerType == Enums.ePowerType.Click)
        {
            classification.ClassificationConfidence = 0.95f;
        }
        else if (classification.HiddenPower)
        {
            classification.ClassificationConfidence = 0.85f;
        }

        classification.ScopedDisposition = DetermineScopedDisposition(power, classification, group, set);

        return classification;
    }

    public static Enums.ePowerType MapPowerType(string type)
    {
        return NormalizeName(type) switch
        {
            "auto" or "auto_" => Enums.ePowerType.Auto_,
            "toggle" => Enums.ePowerType.Toggle,
            "boost" => Enums.ePowerType.Boost,
            "inspiration" => Enums.ePowerType.Inspiration,
            "globalboost" => Enums.ePowerType.GlobalBoost,
            _ => Enums.ePowerType.Click
        };
    }

    public static bool TryGetSinglePowerRequirement(OmniPowerDefinition power, out string parentPowerName)
    {
        parentPowerName = string.Empty;
        var requires = power.Requires?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(requires) ||
            requires.Equals("0", StringComparison.OrdinalIgnoreCase) ||
            requires.Equals("1", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!requires.Contains(' ') &&
            !requires.Contains('(') &&
            !requires.Contains(')') &&
            requires.Count(c => c == '.') == 2)
        {
            parentPowerName = requires;
            return true;
        }

        if (TryExtractWrappedOwnPowerRequirement(requires, out parentPowerName))
        {
            return true;
        }

        if (!OmniExpressionConverter.TryConvertPowerRequirement(requires, out var convertedRequirements))
        {
            return false;
        }

        var candidateRows = convertedRequirements.Rows
            .Where(row => !row.Negated &&
                          row.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated)
            .ToList();
        if (candidateRows.Count != 1)
        {
            return false;
        }

        var candidate = candidateRows[0];
        var supportsSingleGroupedRequirement =
            candidate.Kind == AdvancedConditionKind.PowerRequirementGroup &&
            string.IsNullOrWhiteSpace(candidate.Value);
        if (candidate.Kind is not (AdvancedConditionKind.PowerTaken or AdvancedConditionKind.SourceOwnPower) &&
            !supportsSingleGroupedRequirement ||
            string.IsNullOrWhiteSpace(candidate.Subject) ||
            candidate.Subject.Count(c => c == '.') != 2)
        {
            return false;
        }

        parentPowerName = candidate.Subject.Trim();
        return true;
    }

    private static bool IsClickBuff(
        OmniPowerDefinition power,
        IReadOnlyDictionary<string, OmniPowerDefinition> scopedPowersByFullName,
        bool offensive)
    {
        if (offensive || !IsClickLikePowerType(power.Type))
        {
            return false;
        }

        if (HasSustainedBuffSurface(power))
        {
            return true;
        }

        var redirectDestinations = power.Redirects
            .Select(r => r.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => scopedPowersByFullName.TryGetValue(n, out var dest) ? dest : null)
            .Where(dest => dest != null)
            .Cast<OmniPowerDefinition>()
            .ToList();

        return redirectDestinations.Count > 0 &&
               redirectDestinations.All(dest => !IsOffensivePower(dest)) &&
               redirectDestinations.Any(HasSustainedBuffSurface);
    }

    private static OmniRedirectKind ClassifyRedirects(
        OmniPowerDefinition power,
        IReadOnlyDictionary<string, OmniPowerDefinition> scopedPowersByFullName)
    {
        if (power.Redirects.Count == 0)
        {
            return OmniRedirectKind.None;
        }

        var destinations = power.Redirects
            .Select(r => r.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => scopedPowersByFullName.TryGetValue(n, out var dest) ? dest : null)
            .Where(dest => dest != null)
            .Cast<OmniPowerDefinition>()
            .ToList();

        if (destinations.Count == 0)
        {
            if (power.Redirects
                .Select(r => r.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .All(IsAuxExecutionRedirectName))
            {
                return OmniRedirectKind.ExecutionVariant;
            }

            return OmniRedirectKind.Unknown;
        }

        if (destinations.Any(dest => HasEntCreate(dest)))
        {
            return OmniRedirectKind.SummonDelivery;
        }

        if (destinations.All(IsOffensivePower))
        {
            return OmniRedirectKind.ExecutionVariant;
        }

        if (destinations.Any(dest => dest.ModesRequired.Count > 0 || dest.ModesDisallowed.Count > 0))
        {
            return OmniRedirectKind.ModeVariant;
        }

        if (destinations.Any(HasSustainedBuffSurface))
        {
            return OmniRedirectKind.BuffVariant;
        }

        return OmniRedirectKind.Unknown;
    }

    private static bool IsOffensivePower(OmniPowerDefinition power)
    {
        return HasFoeTarget(power) ||
               power.AttackTypes.Any(IsAttackType) ||
               power.Effects.Any(HasOffensiveEffect);
    }

    private static bool IsAuxExecutionRedirectName(string? powerName)
    {
        if (string.IsNullOrWhiteSpace(powerName))
        {
            return false;
        }

        var group = GroupNamePart(powerName);
        return group.EndsWith("_Aux", StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasFoeTarget(OmniPowerDefinition power)
    {
        return IsFoe(power.TargetType) ||
               IsFoe(power.TargetTypeSecondary) ||
               power.TargetsAffected.Any(IsFoe);
    }

    private static bool HasSustainedBuffSurface(OmniPowerDefinition power)
    {
        if (!TargetsSelfOrAlly(power))
        {
            return false;
        }

        return power.Effects.Any(HasSustainedBuffSurface);
    }

    private static bool HasSustainedBuffSurface(OmniEffectDefinition effect)
    {
        return effect.Templates.Any(template => IsSustainedBuffTemplate(effect, template)) ||
               effect.ChildEffects.Any(HasSustainedBuffSurface);
    }

    private static bool IsSustainedBuffTemplate(OmniEffectDefinition effect, OmniEffectTemplate template)
    {
        if (!HasSustainedDuration(template) ||
            IsFoe(template.Target))
        {
            return false;
        }

        var normalizedType = NormalizeName(template.Type);
        var hasExplicitBuffSemantic = HasExplicitBuffSemantic(effect, template);

        // Omni overloads damage-vector selector names for self/ally buff rows, especially
        // resistance and damage-buff templates. Let explicit buff semantics win before the
        // generic offensive-vector heuristic can reject them as attacks.
        if (hasExplicitBuffSemantic &&
            normalizedType is not ("damage" or "knock" or "entcreate"))
        {
            return true;
        }

        if (IsOffensiveTemplate(template))
        {
            return false;
        }

        if (hasExplicitBuffSemantic)
        {
            return true;
        }

        var normalizedAttribs = template.Attribs
            .Select(NormalizeName)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
        if (normalizedAttribs.Length == 0 ||
            normalizedAttribs.All(static value => value is "null" or "canceleffects"))
        {
            return false;
        }

        // Allow sustained non-offensive self/ally attrib mods to participate in planner
        // click-buff state, even when the raw Omni row uses atypical selectors/tables.
        return normalizedType is "attribmod" or "expression";
    }

    private static bool HasSustainedDuration(OmniEffectTemplate template)
    {
        return template.Duration > 0.25f ||
               !string.IsNullOrWhiteSpace(template.DurationExpression);
    }

    private static bool HasExplicitBuffSemantic(OmniEffectDefinition effect, OmniEffectTemplate template)
    {
        var normalizedType = NormalizeName(template.Type);
        var normalizedAspect = NormalizeName(template.Aspect);
        var normalizedTable = NormalizeName(template.Table);
        var normalizedAttribs = template.Attribs
            .Select(NormalizeName)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
        var normalizedTags = effect.Tags
            .Concat(effect.Flags)
            .Concat(template.Tags)
            .Select(NormalizeName)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .ToArray();

        if (normalizedTags.Any(HasBuffFragment))
        {
            return true;
        }

        if (HasBuffFragment(normalizedTable))
        {
            return true;
        }

        if (normalizedAttribs.Any(HasBuffFragment))
        {
            return true;
        }

        if (normalizedAspect is "strength" or "str" or "current" or "cur" or "resistance" or "res")
        {
            return normalizedAttribs.Length > 0 &&
                   !normalizedAttribs.All(static value => value is "null" or "canceleffects");
        }

        return normalizedType is "heal" or "grantpower";
    }

    private static bool HasBuffFragment(string value)
    {
        return BuffAttribFragments.Any(fragment =>
            value.Contains(NormalizeName(fragment), StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsClickLikePowerType(string? type)
    {
        var normalized = NormalizeName(type ?? string.Empty);
        return normalized is "click" or "clickbuff";
    }

    private static bool TargetsSelfOrAlly(OmniPowerDefinition power)
    {
        if (IsSelfOrAlly(power.TargetType) || IsSelfOrAlly(power.TargetTypeSecondary))
        {
            return true;
        }

        return power.TargetsAffected.Count == 0 || power.TargetsAffected.Any(IsSelfOrAlly);
    }

    private static bool HasOffensiveEffect(OmniEffectDefinition effect)
    {
        return effect.Templates.Any(template =>
                   IsFoe(template.Target) && IsOffensiveTemplate(template)) ||
               effect.ChildEffects.Any(HasOffensiveEffect);
    }

    private static bool IsOffensiveTemplate(OmniEffectTemplate template)
    {
        var normalizedType = NormalizeName(template.Type);
        if (normalizedType is "damage" or "knock" or "entcreate")
        {
            return true;
        }

        return OffensiveAttribFragments.Any(fragment =>
            template.Attribs.Any(attrib => NormalizeName(attrib).Contains(NormalizeName(fragment), StringComparison.OrdinalIgnoreCase)) ||
            NormalizeName(template.Table).Contains(NormalizeName(fragment), StringComparison.OrdinalIgnoreCase));
    }

    private static bool HasEntCreate(OmniPowerDefinition power)
    {
        return power.Effects.Any(HasEntCreate);
    }

    private static bool HasEntCreate(OmniEffectDefinition effect)
    {
        return effect.Templates.Any(t =>
                   t.Params != null &&
                   string.Equals(t.Params.Value<string>("type"), "EntCreate", StringComparison.OrdinalIgnoreCase)) ||
               effect.ChildEffects.Any(HasEntCreate);
    }

    private static bool IsVisibleInherent(OmniPowerDefinition power, string name, string set)
    {
        if (!GroupNamePart(power.FullName).Equals("Inherent", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (set.Equals("Fitness", StringComparison.OrdinalIgnoreCase) && VisibleInherentNames.Contains(name))
        {
            return true;
        }

        if (VisibleInherentNames.Contains(name))
        {
            return true;
        }

        return IsClassInherent(power, name);
    }

    private static bool IsPowersetPlannerControl(OmniPowerDefinition power, string group, string name)
    {
        if (!IsPlayableBuildGroup(group) ||
            string.Equals(power.Requires?.Trim(), "0", StringComparison.OrdinalIgnoreCase) ||
            !PlannerModeMapper.TryGetPlannerMode(name, out var mode))
        {
            return false;
        }

        return mode is PlannerMode.DefensiveAdaptation or
            PlannerMode.EfficientAdaptation or
            PlannerMode.OffensiveAdaptation or
            PlannerMode.ComboLevel1 or
            PlannerMode.ComboLevel2 or
            PlannerMode.ComboLevel3 or
            PlannerMode.FastMode or
            PlannerMode.PerfectionOfBody or
            PlannerMode.PerfectionOfMind or
            PlannerMode.PerfectionOfSoul or
            PlannerMode.PackMentality;
    }

    private static bool IsIncarnateSilentPlannerControl(OmniPowerDefinition power, string group, string set)
    {
        if (!group.Equals("Incarnate", StringComparison.OrdinalIgnoreCase) ||
            !set.EndsWith("_Silent", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return power.ShowInManage ||
               power.ModesRequired.Count > 0 ||
               power.ModesDisallowed.Count > 0 ||
               !string.IsNullOrWhiteSpace(power.TargetRequires) ||
               !string.IsNullOrWhiteSpace(power.Requires) ||
               power.Effects.Count > 0;
    }

    private static bool IsIncarnateSocketPlannerControl(OmniPowerDefinition power, string group, string set)
    {
        if (!group.Equals("Incarnate", StringComparison.OrdinalIgnoreCase) ||
            !set.Equals("Socket", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return power.DoNotSave ||
               power.ModesRequired.Count > 0 ||
               power.ModesDisallowed.Count > 0 ||
               !string.IsNullOrWhiteSpace(power.TargetRequires) ||
               !string.IsNullOrWhiteSpace(power.Requires) ||
               power.Effects.Count > 0;
    }

    private static bool IsVisibleIncarnateBuildChoice(
        OmniPowerDefinition power,
        OmniPowerClassification classification,
        string group,
        string set,
        string name)
    {
        if (!group.Equals("Incarnate", StringComparison.OrdinalIgnoreCase) ||
            classification.HiddenPower ||
            string.Equals(name, "Nothing", StringComparison.OrdinalIgnoreCase) ||
            IsIncarnateSilentPlannerControl(power, group, set) ||
            IsIncarnateSocketPlannerControl(power, group, set) ||
            IsPetManifestOwnedScopedPower(power, group, set))
        {
            return false;
        }

        return power.ShowInManage || !power.DoNotSave;
    }

    private static bool ShouldForceVisibleIncarnateClickBuff(
        OmniPowerDefinition power,
        OmniPowerClassification classification,
        string set,
        bool offensive)
    {
        if (classification.PowerType != Enums.ePowerType.Click ||
            classification.HiddenPower ||
            classification.ExecutionOnly ||
            offensive)
        {
            return false;
        }

        // Destiny powers are player-triggered incarnate support states in planner terms,
        // even when their raw exported effect rows look more like delivery/heal/teleport
        // payloads than classic sustained buff templates.
        if (set.Equals("Destiny", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return HasSustainedBuffSurface(power);
    }

    private static bool IsTemporarySilentSupportPower(string group, string set)
    {
        return group.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase) &&
               set.Equals("SilentPowers", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsRetainedPrestigeVisiblePower(string group, string set)
    {
        if (!group.Equals("Prestige", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return NormalizeName(set) is
            "combatpets" or
            "prestigeattacks" or
            "prestigesprints" or
            "prestigetravel" or
            "prestigeutility";
    }

    private static OmniScopedPowerDisposition DetermineScopedDisposition(
        OmniPowerDefinition power,
        OmniPowerClassification classification,
        string group,
        string set)
    {
        if (IsExplicitlyExcludedScopedPower(power, group, set))
        {
            return OmniScopedPowerDisposition.Excluded;
        }

        if (IsPetManifestOwnedScopedPower(power, group, set))
        {
            return OmniScopedPowerDisposition.PetManifestOwned;
        }

        return classification.HiddenPower
            ? OmniScopedPowerDisposition.MainImportHidden
            : OmniScopedPowerDisposition.MainImportVisible;
    }

    private static bool IsExplicitlyExcludedScopedPower(
        OmniPowerDefinition power,
        string group,
        string set)
    {
        if (OmniImportScope.IsExcludedPower(power.FullName))
        {
            return true;
        }

        if (group.Equals("Prestige", StringComparison.OrdinalIgnoreCase))
        {
            return NormalizeName(set) is
                "combatdummy" or
                "prestigecostumes" or
                "fun" or
                "vanitypets";
        }

        return group.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase) &&
               set.Equals("Art_Test", StringComparison.OrdinalIgnoreCase);
    }

    internal static bool IsPetManifestOwnedPowerset(string powersetFullName)
    {
        if (string.IsNullOrWhiteSpace(powersetFullName))
        {
            return false;
        }

        var group = GroupNamePart(powersetFullName);
        if (!group.Equals("Incarnate", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var set = SetNamePart(powersetFullName);
        return set.Equals("Destiny_Silent", StringComparison.OrdinalIgnoreCase) ||
               set.Contains("Lore_Pet", StringComparison.OrdinalIgnoreCase) ||
               set.Equals("Ion_Judgement", StringComparison.OrdinalIgnoreCase) ||
               set.Equals("AntiMatterRayBurn", StringComparison.OrdinalIgnoreCase) ||
               set.Equals("Barrier_Rez", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPetManifestOwnedScopedPower(
        OmniPowerDefinition power,
        string group,
        string set)
    {
        return IsPetManifestOwnedPowerset($"{group}.{set}");
    }

    private static bool IsPetRootName(string group)
    {
        return NormalizeName(group) is
            "incarnatepets" or
            "kheldianpets" or
            "mastermindpets" or
            "pets" or
            "villainpets";
    }

    private static bool IsPowersetGrantedTemporaryState(
        OmniPowerDefinition power,
        string group,
        string set,
        string name)
    {
        if (!group.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase) ||
            !set.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var normalizedName = NormalizeName(name);
        return normalizedName is "savagemeleebloodfrenzystalker" or
            "savagemeleebloodfrenzy" or
            "savagemeleeexhausted" ||
            normalizedName.Contains("bloodfrenzy", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsExplicitlyRetainedTemporaryPower(string group, string set, string name)
    {
        if (!group.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase) ||
            !set.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return NormalizeName(name) is
            "secondwind" or
            "mementomoriexecute" or
            "soultransferexecute" or
            "streetcredmonitor" or
            "streetcred" or
            "mementomori" or
            "revive" or
            "soultransfer";
    }

    private static bool IsExplicitlyRetainedTemporaryVisiblePower(string group, string set, string name)
    {
        if (!group.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase) ||
            !set.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return NormalizeName(name) is
            "secondwind" or
            "revive" or
            "streetcred" or
            "mementomori" or
            "soultransfer";
    }

    private static bool IsExplicitlyRetainedTemporarySupportPower(string group, string set, string name)
    {
        if (!group.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase) ||
            !set.Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return NormalizeName(name) is
            "streetcredmonitor" or
            "mementomoriexecute" or
            "soultransferexecute";
    }

    private static bool IsClassInherent(OmniPowerDefinition power, string name)
    {
        return BuildRelevantClassInherents.Contains(name);
    }

    private static bool HasBuildPrerequisite(OmniPowerDefinition power)
    {
        return !string.IsNullOrWhiteSpace(power.Requires) &&
               !power.Requires.Trim().Equals("0", StringComparison.OrdinalIgnoreCase) &&
               !power.Requires.Trim().Equals("1", StringComparison.OrdinalIgnoreCase);
    }

    private static Enums.eGridType DetermineGrantedSupportInherentType(
        string group,
        OmniPowerDefinition power,
        bool sameSetPowerRequirement,
        bool visibleInherent,
        bool isTemporaryPower,
        bool hasModeRequirement,
        bool executionOnly)
    {
        if (!power.AutoIssue ||
            power.AvailableLevel != 0 ||
            visibleInherent ||
            isTemporaryPower ||
            executionOnly ||
            !IsPlayableBuildGroup(group))
        {
            return Enums.eGridType.None;
        }

        if (sameSetPowerRequirement)
        {
            return Enums.eGridType.Power;
        }

        if (hasModeRequirement)
        {
            return Enums.eGridType.None;
        }

        return Enums.eGridType.Powerset;
    }

    private static bool TryExtractWrappedOwnPowerRequirement(string requires, out string parentPowerName)
    {
        parentPowerName = string.Empty;
        var candidate = requires.Trim();
        while (candidate.StartsWith("!", StringComparison.Ordinal))
        {
            candidate = candidate[1..].TrimStart();
        }

        const string sourceOwnPowerPrefix = "source.ownPower?(";
        const string ownPowerPrefix = "ownPower?(";
        if (candidate.StartsWith(sourceOwnPowerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            candidate = candidate[sourceOwnPowerPrefix.Length..];
        }
        else if (candidate.StartsWith(ownPowerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            candidate = candidate[ownPowerPrefix.Length..];
        }
        else
        {
            return false;
        }

        if (!candidate.EndsWith(")", StringComparison.Ordinal))
        {
            return false;
        }

        candidate = candidate[..^1].Trim();
        if (string.IsNullOrWhiteSpace(candidate) ||
            candidate.Contains(' ') ||
            candidate.Contains('(') ||
            candidate.Contains(')') ||
            candidate.Count(c => c == '.') != 2)
        {
            return false;
        }

        parentPowerName = candidate;
        return true;
    }

    private static bool SamePowerset(string powerFullName, string otherPowerFullName)
    {
        return string.Equals(
            $"{GroupNamePart(powerFullName)}.{SetNamePart(powerFullName)}",
            $"{GroupNamePart(otherPowerFullName)}.{SetNamePart(otherPowerFullName)}",
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSupportHeavyGroup(string group)
    {
        return NormalizeName(group) is
            "inherent" or
            "pets" or
            "temporarypowers" or
            "incarnate" or
            "incarnatepets" or
            "kheldianpets" or
            "mastermindpets" or
            "villainpets";
    }

    private static bool IsRedirectOrPetExecutionGroup(string group)
    {
        return NormalizeName(group) is
            "redirects" or
            "pets" or
            "kheldianpets" or
            "mastermindpets" or
            "villainpets" or
            "incarnatepets";
    }

    private static bool IsNormalBuildPickCandidate(
        string group,
        OmniPowerDefinition power,
        bool hasModeRequirement,
        bool sameSetPowerRequirement,
        bool executionOnly)
    {
        return IsPlayableBuildGroup(group) &&
               !power.AutoIssue &&
               !hasModeRequirement &&
               !sameSetPowerRequirement &&
               !executionOnly &&
               !string.Equals(power.Requires?.Trim(), "0", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPlayableBuildGroup(string group)
    {
        return NormalizeName(group) is
            "arachnossoldiers" or
            "arachnoswidow" or
            "blasterranged" or
            "blastersupport" or
            "brutedefense" or
            "brutemelee" or
            "controllerbuff" or
            "controllercontrol" or
            "corruptorbuff" or
            "corruptorranged" or
            "defenderbuff" or
            "defenderranged" or
            "dominatorassault" or
            "dominatorcontrol" or
            "mastermindbuff" or
            "mastermindsummon" or
            "peacebringeroffensive" or
            "peacebringerdefensive" or
            "scrapperdefense" or
            "scrappermelee" or
            "sentineldefense" or
            "sentinelranged" or
            "stalkerdefense" or
            "stalkermelee" or
            "tankerdefense" or
            "tankermelee" or
            "warshadeoffensive" or
            "warshadedefensive" or
            "pool" or
            "epic";
    }

    private static bool IsAttackType(string attackType)
    {
        return NormalizeName(attackType) is
            "melee" or
            "ranged" or
            "aoe" or
            "areaofeffect" or
            "smashing" or
            "smash" or
            "lethal" or
            "cold" or
            "fire" or
            "energy" or
            "negativeenergy" or
            "negative" or
            "psionic" or
            "psi" or
            "toxic";
    }

    private static bool IsFoe(string value)
    {
        var normalized = NormalizeName(value);
        return normalized.Contains("foe", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("villain", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("enemy", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSelfOrAlly(string value)
    {
        var normalized = NormalizeName(value);
        return string.IsNullOrWhiteSpace(normalized) ||
               normalized.Contains("self", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("caster", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("ally", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("friend", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("teammate", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("player", StringComparison.OrdinalIgnoreCase);
    }

    private static string GroupNamePart(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    private static string SetNamePart(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[1] : string.Empty;
    }

    private static string LastNamePart(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[^1] : string.Empty;
    }

    private static string NormalizeName(string value)
    {
        return (value ?? string.Empty)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("(", string.Empty, StringComparison.Ordinal)
            .Replace(")", string.Empty, StringComparison.Ordinal)
            .Trim()
            .ToLowerInvariant();
    }
}

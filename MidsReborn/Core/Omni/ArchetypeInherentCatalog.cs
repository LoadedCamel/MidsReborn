using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core.Omni;

public enum ArchetypeInherentPresentationType
{
    InfoOnly = 0,
    ComputedFromTargetContext = 1,
    ComputedFromTeamContext = 2,
    ComputedFromPetContext = 3,
    ManualToggle = 4,
    ManualSlider = 5
}

public enum ArchetypeInherentBehaviorModel
{
    None = 0,
    LegacyCompatibility = 1,
    ComputedTargetContext = 2,
    ComputedTeamContext = 3,
    ComputedPetContext = 4,
    ManualToggle = 5,
    ManualSlider = 6
}

public enum ArchetypeInherentVisibilityRule
{
    OnlyIfPresentInActiveDatabase = 0
}

public enum ArchetypeInherentFallbackBehavior
{
    InformationalOnly = 0,
    LegacyCompatibility = 1,
    PreserveImportedData = 2
}

internal enum TierOneArchetypeInherentFamily
{
    None = 0,
    Assassination = 1,
    Opportunity = 2,
    Containment = 3,
    Domination = 4
}

internal sealed class ArchetypeInherentDefinition
{
    public required string CatalogKey { get; init; }
    public required OmniDataProviderId ProviderId { get; init; }
    public required string OwningClassName { get; init; }
    public required string PowerFullName { get; init; }
    public required ArchetypeInherentPresentationType PresentationType { get; init; }
    public required ArchetypeInherentBehaviorModel BehaviorModel { get; init; }
    public required ArchetypeInherentVisibilityRule VisibilityRule { get; init; }
    public required ArchetypeInherentFallbackBehavior FallbackBehavior { get; init; }
}

internal static class ArchetypeInherentCatalog
{
    private static readonly ArchetypeInherentDefinition[] Definitions =
    [
        CreateManualSlider("Homecoming.Stalker.Assassination", OmniDataProviderId.OmniHomecoming, "Class_Stalker", "Inherent.Inherent.Assassination"),
        CreateManualToggle("Homecoming.Controller.Containment", OmniDataProviderId.OmniHomecoming, "Class_Controller", "Inherent.Inherent.Containment"),
        CreateComputedTeam("Homecoming.Peacebringer.CosmicBalance", OmniDataProviderId.OmniHomecoming, "Class_Kheldian", "Inherent.Inherent.Cosmic_Balance"),
        CreatePassiveInfo("Homecoming.Scrapper.CriticalHit", OmniDataProviderId.OmniHomecoming, "Class_Scrapper", "Inherent.Inherent.Critical_Hit"),
        CreateComputedTeam("Homecoming.Warshade.DarkSustenance", OmniDataProviderId.OmniHomecoming, "Class_Shade", "Inherent.Inherent.Dark_Sustenance"),
        CreatePassiveInfo("Homecoming.Blaster.Defiance", OmniDataProviderId.OmniHomecoming, "Class_Blaster", "Inherent.Inherent.Defiance"),
        CreatePassiveInfo("Homecoming.Blaster.DefianceBuff", OmniDataProviderId.OmniHomecoming, "Class_Blaster", "Inherent.Inherent.Defiance_Buff"),
        CreatePassiveInfo("Homecoming.Blaster.DefianceV2", OmniDataProviderId.OmniHomecoming, "Class_Blaster", "Inherent.Inherent.Defiance_v2"),
        CreateManualToggle("Homecoming.Dominator.Domination", OmniDataProviderId.OmniHomecoming, "Class_Dominator", "Inherent.Inherent.Domination"),
        CreateManualSlider("Homecoming.Dominator.DominationMeter", OmniDataProviderId.OmniHomecoming, "Class_Dominator", "Inherent.Inherent.Domination_Meter"),
        CreateManualSlider("Homecoming.Brute.Fury", OmniDataProviderId.OmniHomecoming, "Class_Brute", "Inherent.Inherent.Rage"),
        CreatePassiveInfo("Homecoming.Brute.FuryBuff", OmniDataProviderId.OmniHomecoming, "Class_Brute", "Inherent.Inherent.Rage_Buff"),
        CreatePassiveInfo("Homecoming.Brute.FuryDampen", OmniDataProviderId.OmniHomecoming, "Class_Brute", "Inherent.Inherent.Rage_Dampen"),
        CreatePassiveInfo("Homecoming.Brute.FuryStrengthen", OmniDataProviderId.OmniHomecoming, "Class_Brute", "Inherent.Inherent.Rage_Strengthen"),
        CreatePassiveInfo("Homecoming.Tanker.Gauntlet", OmniDataProviderId.OmniHomecoming, "Class_Tanker", "Inherent.Inherent.Gauntlet"),
        CreateManualSlider("Homecoming.Sentinel.Opportunity", OmniDataProviderId.OmniHomecoming, "Class_Sentinel", "Inherent.Inherent.Opportunity"),
        CreatePassiveInfo("Homecoming.Sentinel.Vulnerability", OmniDataProviderId.OmniHomecoming, "Class_Sentinel", "Inherent.Inherent.Vulnerability"),
        CreateComputedTarget("Homecoming.Corruptor.Scourge", OmniDataProviderId.OmniHomecoming, "Class_Corruptor", "Inherent.Inherent.Scourge"),
        CreateComputedPet("Homecoming.Mastermind.Supremacy", OmniDataProviderId.OmniHomecoming, "Class_Mastermind", "Inherent.Inherent.Supremacy"),
        CreateComputedTeam("Homecoming.Defender.Vigilance", OmniDataProviderId.OmniHomecoming, "Class_Defender", "Inherent.Inherent.Vigilance"),
        CreatePassiveInfo("Homecoming.Widow.Conditioning", OmniDataProviderId.OmniHomecoming, "Class_Arachnos_Widow", "Inherent.Inherent.Widow_Conditioning"),
        CreatePassiveInfo("Homecoming.Soldier.Conditioning", OmniDataProviderId.OmniHomecoming, "Class_Arachnos_Soldier", "Inherent.Inherent.Spider_Conditioning")
    ];

    private static readonly Dictionary<string, ArchetypeInherentDefinition> DefinitionsByKey = Definitions
        .ToDictionary(
            definition => ComposeLookupKey(definition.ProviderId, definition.OwningClassName, definition.PowerFullName),
            definition => definition,
            StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, ArchetypeInherentDefinition[]> DefinitionsByProviderAndPower = Definitions
        .GroupBy(
            definition => ComposeProviderPowerKey(definition.ProviderId, definition.PowerFullName),
            StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
            group => group.Key,
            group => group.ToArray(),
            StringComparer.OrdinalIgnoreCase);

    private static readonly string[] TierOneTokens =
    [
        "source>kMeter",
        "source>cur.kMeter",
        "source>kRage",
        "source>cur.kRage",
        "target>kHeld",
        "target>kSleep",
        "target>kImmobilized",
        "target>kStunned",
        "target>kTerrorized",
        "target.EventTimeSince>Sleep",
        "target>enttype",
        "target.mode?(kOpportunitySustain)",
        "target.ownPower?(Temporary_Powers.Temporary_Powers.Opportunity_Lock)",
        "target.ownPower?(Temporary_Powers.Temporary_Powers.Opportunity)",
        "target.ownPower?(Temporary_Powers.Temporary_Powers.Vulnerability)",
        "Source.Mode?(Domination)",
        "source.Mode?(Domination)",
        "target.HasTag?(IncarnateBoss)"
    ];

    private static readonly Regex AssassinationMeterRegex = new(
        @"^source>(?:cur\.)?kMeter\s*(>=|<=|>|<|==|eq|!=|ne)\s*([0-9]*\.?[0-9]+)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex RageMeterRegex = new(
        @"^source>(?:cur\.)?kRage\s*(>=|<=|>|<|==|eq|!=|ne)\s*([0-9]*\.?[0-9]+)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex TargetStateRegex = new(
        @"^target>(kHeld|kSleep|kImmobilized|kStunned|kTerrorized)\s*(>=|<=|>|<|==|eq|!=|ne)\s*([0-9]*\.?[0-9]+)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex TargetSleepEventRegex = new(
        @"^target\.EventTimeSince>Sleep\s*(>=|<=|>|<|==|eq|!=|ne)\s*([0-9]*\.?[0-9]+)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex TargetModeRegex = new(
        @"^target\.Mode\?\(([^)]+)\)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex TargetOwnPowerRegex = new(
        @"^target\.ownPower\?\(([^)]+)\)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex TargetEntityRegex = new(
        @"^target>enttype\s+(eq|==|!=|ne)\s+['""]?([A-Za-z0-9_]+)['""]?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex TargetTagRegex = new(
        @"^target\.HasTag\?\(([A-Za-z0-9_\-]+)\)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex SourceModeRegex = new(
        @"^(?:source\.)?Mode\?\(([^)]+)\)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    internal static bool TryGetDefinition(
        OmniDataProviderId providerId,
        string owningClassName,
        string powerFullName,
        out ArchetypeInherentDefinition definition)
    {
        return DefinitionsByKey.TryGetValue(
            ComposeLookupKey(providerId, owningClassName, powerFullName),
            out definition!);
    }

    internal static IReadOnlyCollection<ArchetypeInherentDefinition> GetDefinitionsForProvider(OmniDataProviderId providerId)
    {
        return Definitions
            .Where(definition => definition.ProviderId == providerId)
            .ToArray();
    }

    internal static bool TryCreateBinding(
        OmniDataProviderId providerId,
        string powerFullName,
        IEnumerable<string>? owningClassNames,
        out ImportedArchetypeInherentBinding binding)
    {
        binding = null!;
        if (providerId == OmniDataProviderId.Unknown || string.IsNullOrWhiteSpace(powerFullName))
        {
            return false;
        }

        var normalizedPowerFullName = NormalizePowerFullName(powerFullName);
        var distinctClassNames = (owningClassNames ?? [])
            .Where(className => !string.IsNullOrWhiteSpace(className))
            .Select(OmniImportScope.NormalizeClassName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var matches = distinctClassNames
            .Select(className => DefinitionsByKey.TryGetValue(
                ComposeLookupKey(providerId, className, normalizedPowerFullName),
                out var definition)
                ? definition
                : null)
            .Where(definition => definition != null)
            .Distinct()
            .ToArray();

        if (matches.Length == 0 &&
            DefinitionsByProviderAndPower.TryGetValue(
                ComposeProviderPowerKey(providerId, normalizedPowerFullName),
                out var providerPowerMatches) &&
            providerPowerMatches.Length == 1)
        {
            matches = providerPowerMatches;
        }

        if (matches.Length != 1)
        {
            return false;
        }

        binding = CreateBinding(matches[0]!);
        return true;
    }

    internal static bool IsTierOneExpression(string? expression)
    {
        return !string.IsNullOrWhiteSpace(expression) &&
               TierOneTokens.Any(token => expression.Contains(token, StringComparison.OrdinalIgnoreCase));
    }

    internal static bool ShouldStripUnrewrittenRuntimeExpression(string ownerFullName, string? expression)
    {
        if (string.IsNullOrWhiteSpace(ownerFullName) || string.IsNullOrWhiteSpace(expression))
        {
            return false;
        }

        var family = ResolveTierOneFamily(ownerFullName);
        if (family == TierOneArchetypeInherentFamily.None)
        {
            return false;
        }

        return family switch
        {
            TierOneArchetypeInherentFamily.Assassination => ContainsAny(
                expression,
                "source>kMeter",
                "source>cur.kMeter",
                "target>kHeld",
                "target>kSleep",
                "target.EventTimeSince>Sleep",
                "target>enttype"),
            TierOneArchetypeInherentFamily.Opportunity => ContainsAny(
                expression,
                "source>kRage",
                "source>cur.kRage",
                "target.mode?(kOpportunitySustain)",
                "target.ownPower?(Temporary_Powers.Temporary_Powers.Opportunity_Lock)",
                "target.ownPower?(Temporary_Powers.Temporary_Powers.Opportunity)",
                "target.ownPower?(Temporary_Powers.Temporary_Powers.Vulnerability)"),
            TierOneArchetypeInherentFamily.Containment => ContainsAny(
                expression,
                "target>kHeld",
                "target>kImmobilized",
                "target>kStunned",
                "target>kTerrorized",
                "target.EventTimeSince>Sleep",
                "target.HasTag?(IncarnateBoss)"),
            TierOneArchetypeInherentFamily.Domination => ContainsAny(
                expression,
                "source>kRage",
                "source>cur.kRage",
                "Source.Mode?(Domination)",
                "source.Mode?(Domination)"),
            _ => false
        };
    }

    internal static bool TryRewriteTierOneCondition(
        string ownerFullName,
        string normalizedExpression,
        bool negated,
        AdvancedConditionLink link,
        string rawExpression,
        out AdvancedConditionRow rewrittenRow)
    {
        rewrittenRow = null!;
        if (string.IsNullOrWhiteSpace(ownerFullName) || string.IsNullOrWhiteSpace(normalizedExpression))
        {
            return false;
        }

        var family = ResolveTierOneFamily(ownerFullName);
        if (family == TierOneArchetypeInherentFamily.None)
        {
            return false;
        }

        if (family == TierOneArchetypeInherentFamily.Assassination &&
            TryRewriteAssassinationMeter(link, rawExpression, normalizedExpression, negated, out rewrittenRow))
        {
            return true;
        }

        if ((family == TierOneArchetypeInherentFamily.Opportunity || family == TierOneArchetypeInherentFamily.Domination) &&
            TryRewriteRageMeter(family, link, rawExpression, normalizedExpression, negated, out rewrittenRow))
        {
            return true;
        }

        if ((family == TierOneArchetypeInherentFamily.Assassination || family == TierOneArchetypeInherentFamily.Containment) &&
            TryRewriteTargetState(family, link, rawExpression, normalizedExpression, negated, out rewrittenRow))
        {
            return true;
        }

        if (family == TierOneArchetypeInherentFamily.Containment &&
            TryRewriteTargetTag(link, rawExpression, normalizedExpression, negated, out rewrittenRow))
        {
            return true;
        }

        if (family == TierOneArchetypeInherentFamily.Opportunity &&
            TryRewriteOpportunityTargetState(link, rawExpression, normalizedExpression, negated, out rewrittenRow))
        {
            return true;
        }

        if (family == TierOneArchetypeInherentFamily.Assassination &&
            TryRewriteTargetEntity(link, rawExpression, normalizedExpression, negated, out rewrittenRow))
        {
            return true;
        }

        if (family == TierOneArchetypeInherentFamily.Domination &&
            TryRewriteDominationSourceMode(link, rawExpression, normalizedExpression, negated, out rewrittenRow))
        {
            return true;
        }

        return false;
    }

    internal static bool TryAppendTierOnePowerRequirements(IPower power, AdvancedConditionSet requirements)
    {
        if (power == null)
        {
            return false;
        }

        if (power.FullName.Equals(PlannerStateCatalog.DominationModePowerFullName, StringComparison.OrdinalIgnoreCase) &&
            power.ModesRequired.HasFlag(Enums.eModeFlags.Domination))
        {
            if (!requirements.Rows.Any(row =>
                    row.Kind == AdvancedConditionKind.SourceMode &&
                    row.Subject.Equals(PlannerModeMapper.ToCanonicalName(PlannerMode.DominationActive), StringComparison.OrdinalIgnoreCase)))
            {
                requirements.Rows.Add(new AdvancedConditionRow
                {
                    Link = requirements.Rows.Count == 0 ? AdvancedConditionLink.And : AdvancedConditionLink.And,
                    Kind = AdvancedConditionKind.SourceMode,
                    Subject = PlannerModeMapper.ToCanonicalName(PlannerMode.DominationActive),
                    Operator = AdvancedConditionOperator.Equals,
                    Value = "true",
                    RawExpression = "modes_required:Domination"
                });
            }

            power.ModesRequired &= ~Enums.eModeFlags.Domination;
            return true;
        }

        return false;
    }

    private static bool TryRewriteAssassinationMeter(
        AdvancedConditionLink link,
        string rawExpression,
        string normalizedExpression,
        bool negated,
        out AdvancedConditionRow row)
    {
        row = null!;
        var match = AssassinationMeterRegex.Match(normalizedExpression);
        if (!match.Success || !double.TryParse(match.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var threshold))
        {
            return false;
        }

        bool? requiresHidden = match.Groups[1].Value switch
        {
            ">" when threshold <= 0.0001d => true,
            ">=" when threshold >= 0.9d => true,
            "!=" when threshold <= 0.0001d => true,
            "<" when threshold <= 0.9d => false,
            "<=" when threshold < 0.9d => false,
            "==" or "eq" when threshold <= 0.0001d => false,
            _ => null
        };

        if (requiresHidden == null)
        {
            return false;
        }

        row = CreateModeRow(
            PlannerMode.StalkerHidden,
            requiresHidden.Value ^ negated,
            link,
            rawExpression);
        return true;
    }

    private static bool TryRewriteRageMeter(
        TierOneArchetypeInherentFamily family,
        AdvancedConditionLink link,
        string rawExpression,
        string normalizedExpression,
        bool negated,
        out AdvancedConditionRow row)
    {
        row = null!;
        var match = RageMeterRegex.Match(normalizedExpression);
        if (!match.Success)
        {
            return false;
        }

        var sliderPower = family == TierOneArchetypeInherentFamily.Domination
            ? PlannerStateCatalog.DominationMeterPowerFullName
            : PlannerStateCatalog.OpportunityMeterPowerFullName;

        row = new AdvancedConditionRow
        {
            Link = link,
            Kind = AdvancedConditionKind.PowerStacks,
            Subject = sliderPower,
            Operator = ParseOperator(match.Groups[1].Value),
            Value = NormalizeNumericValue(match.Groups[2].Value),
            Negated = negated,
            RawExpression = rawExpression
        };

        return true;
    }

    private static bool TryRewriteTargetState(
        TierOneArchetypeInherentFamily family,
        AdvancedConditionLink link,
        string rawExpression,
        string normalizedExpression,
        bool negated,
        out AdvancedConditionRow row)
    {
        row = null!;

        var stateMatch = TargetStateRegex.Match(normalizedExpression);
        if (stateMatch.Success)
        {
            var subject = stateMatch.Groups[1].Value.ToLowerInvariant() switch
            {
                "kheld" => "cfg.target.held",
                "kimmobilized" => "cfg.target.immobilized",
                "kstunned" => "cfg.target.stunned",
                "kterrorized" => "cfg.target.terrorized",
                "ksleep" => "cfg.target.sleptrecently",
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(subject))
            {
                return false;
            }

            var active = IsTruthyStateComparison(stateMatch.Groups[2].Value, stateMatch.Groups[3].Value);
            row = family == TierOneArchetypeInherentFamily.Containment
                ? CreateModeRow(PlannerMode.Containment, active ^ negated, link, rawExpression)
                : CreateCombatSettingRow(subject, active ^ negated, link, rawExpression);
            return true;
        }

        var sleepMatch = TargetSleepEventRegex.Match(normalizedExpression);
        if (!sleepMatch.Success)
        {
            return false;
        }

        var recentSleep = sleepMatch.Groups[1].Value switch
        {
            "<" or "<=" => true,
            "==" or "eq" => true,
            _ => false
        };

        row = family == TierOneArchetypeInherentFamily.Containment
            ? CreateModeRow(PlannerMode.Containment, recentSleep ^ negated, link, rawExpression)
            : CreateCombatSettingRow("cfg.target.sleptrecently", recentSleep ^ negated, link, rawExpression);
        return true;
    }

    private static bool TryRewriteTargetTag(
        AdvancedConditionLink link,
        string rawExpression,
        string normalizedExpression,
        bool negated,
        out AdvancedConditionRow row)
    {
        row = null!;
        var match = TargetTagRegex.Match(normalizedExpression);
        if (!match.Success)
        {
            return false;
        }

        row = CreateModeRow(PlannerMode.Containment, !negated, link, rawExpression);

        return true;
    }

    private static bool TryRewriteOpportunityTargetState(
        AdvancedConditionLink link,
        string rawExpression,
        string normalizedExpression,
        bool negated,
        out AdvancedConditionRow row)
    {
        row = null!;

        var modeMatch = TargetModeRegex.Match(normalizedExpression);
        if (modeMatch.Success &&
            OmniModeMapper.Normalize(modeMatch.Groups[1].Value).Equals("OpportunitySustain", StringComparison.OrdinalIgnoreCase))
        {
            row = CreateCombatSettingRow(
                "cfg.target.vulnerabilityactive",
                !negated,
                link,
                rawExpression);
            return true;
        }

        var ownPowerMatch = TargetOwnPowerRegex.Match(normalizedExpression);
        if (!ownPowerMatch.Success)
        {
            return false;
        }

        var targetPower = ownPowerMatch.Groups[1].Value.Trim();
        row = targetPower switch
        {
            "Temporary_Powers.Temporary_Powers.Opportunity_Lock" => CreateCombatSettingRow(
                "cfg.target.vulnerabilityactive",
                !negated,
                link,
                rawExpression),
            "Temporary_Powers.Temporary_Powers.Opportunity" => CreateCombatSettingRow(
                "cfg.target.vulnerabilityactive",
                !negated,
                link,
                rawExpression),
            "Temporary_Powers.Temporary_Powers.Vulnerability" => CreateCombatSettingRow(
                "cfg.target.vulnerabilityactive",
                !negated,
                link,
                rawExpression),
            _ => null!
        };

        if (row == null)
        {
            return false;
        }

        return true;
    }

    private static bool TryRewriteTargetEntity(
        AdvancedConditionLink link,
        string rawExpression,
        string normalizedExpression,
        bool negated,
        out AdvancedConditionRow row)
    {
        row = null!;
        var match = TargetEntityRegex.Match(normalizedExpression);
        if (!match.Success)
        {
            return false;
        }

        var scope = match.Groups[2].Value.Trim('\'', '"').Equals("player", StringComparison.OrdinalIgnoreCase)
            ? AdvancedConditionTargetScope.Player
            : match.Groups[2].Value.Trim('\'', '"').Equals("critter", StringComparison.OrdinalIgnoreCase)
                ? AdvancedConditionTargetScope.Foe
                : AdvancedConditionTargetScope.Unknown;
        if (scope == AdvancedConditionTargetScope.Unknown)
        {
            return false;
        }

        row = new AdvancedConditionRow
        {
            Link = link,
            Kind = AdvancedConditionKind.TargetEntityType,
            Subject = match.Groups[2].Value.Trim('\'', '"'),
            Value = scope.ToString(),
            TargetScope = scope,
            Operator = ParseOperator(match.Groups[1].Value),
            Negated = negated,
            RawExpression = rawExpression
        };

        return true;
    }

    private static bool TryRewriteDominationSourceMode(
        AdvancedConditionLink link,
        string rawExpression,
        string normalizedExpression,
        bool negated,
        out AdvancedConditionRow row)
    {
        row = null!;
        var match = SourceModeRegex.Match(normalizedExpression);
        if (!match.Success || !OmniModeMapper.Normalize(match.Groups[1].Value).Equals("Domination", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        row = CreateModeRow(PlannerMode.DominationActive, !negated, link, rawExpression);
        return true;
    }

    private static ArchetypeInherentDefinition CreatePassiveInfo(
        string catalogKey,
        OmniDataProviderId providerId,
        string owningClassName,
        string powerFullName)
    {
        return new ArchetypeInherentDefinition
        {
            CatalogKey = catalogKey,
            ProviderId = providerId,
            OwningClassName = OmniImportScope.NormalizeClassName(owningClassName),
            PowerFullName = NormalizePowerFullName(powerFullName),
            PresentationType = ArchetypeInherentPresentationType.InfoOnly,
            BehaviorModel = ArchetypeInherentBehaviorModel.None,
            VisibilityRule = ArchetypeInherentVisibilityRule.OnlyIfPresentInActiveDatabase,
            FallbackBehavior = ArchetypeInherentFallbackBehavior.InformationalOnly
        };
    }

    private static ArchetypeInherentDefinition CreateManualToggle(
        string catalogKey,
        OmniDataProviderId providerId,
        string owningClassName,
        string powerFullName)
    {
        return new ArchetypeInherentDefinition
        {
            CatalogKey = catalogKey,
            ProviderId = providerId,
            OwningClassName = OmniImportScope.NormalizeClassName(owningClassName),
            PowerFullName = NormalizePowerFullName(powerFullName),
            PresentationType = ArchetypeInherentPresentationType.ManualToggle,
            BehaviorModel = ArchetypeInherentBehaviorModel.ManualToggle,
            VisibilityRule = ArchetypeInherentVisibilityRule.OnlyIfPresentInActiveDatabase,
            FallbackBehavior = ArchetypeInherentFallbackBehavior.LegacyCompatibility
        };
    }

    private static ArchetypeInherentDefinition CreateManualSlider(
        string catalogKey,
        OmniDataProviderId providerId,
        string owningClassName,
        string powerFullName)
    {
        return new ArchetypeInherentDefinition
        {
            CatalogKey = catalogKey,
            ProviderId = providerId,
            OwningClassName = OmniImportScope.NormalizeClassName(owningClassName),
            PowerFullName = NormalizePowerFullName(powerFullName),
            PresentationType = ArchetypeInherentPresentationType.ManualSlider,
            BehaviorModel = ArchetypeInherentBehaviorModel.ManualSlider,
            VisibilityRule = ArchetypeInherentVisibilityRule.OnlyIfPresentInActiveDatabase,
            FallbackBehavior = ArchetypeInherentFallbackBehavior.LegacyCompatibility
        };
    }

    private static ArchetypeInherentDefinition CreateComputedTarget(
        string catalogKey,
        OmniDataProviderId providerId,
        string owningClassName,
        string powerFullName)
    {
        return new ArchetypeInherentDefinition
        {
            CatalogKey = catalogKey,
            ProviderId = providerId,
            OwningClassName = OmniImportScope.NormalizeClassName(owningClassName),
            PowerFullName = NormalizePowerFullName(powerFullName),
            PresentationType = ArchetypeInherentPresentationType.ComputedFromTargetContext,
            BehaviorModel = ArchetypeInherentBehaviorModel.ComputedTargetContext,
            VisibilityRule = ArchetypeInherentVisibilityRule.OnlyIfPresentInActiveDatabase,
            FallbackBehavior = ArchetypeInherentFallbackBehavior.PreserveImportedData
        };
    }

    private static ArchetypeInherentDefinition CreateComputedTeam(
        string catalogKey,
        OmniDataProviderId providerId,
        string owningClassName,
        string powerFullName)
    {
        return new ArchetypeInherentDefinition
        {
            CatalogKey = catalogKey,
            ProviderId = providerId,
            OwningClassName = OmniImportScope.NormalizeClassName(owningClassName),
            PowerFullName = NormalizePowerFullName(powerFullName),
            PresentationType = ArchetypeInherentPresentationType.ComputedFromTeamContext,
            BehaviorModel = ArchetypeInherentBehaviorModel.ComputedTeamContext,
            VisibilityRule = ArchetypeInherentVisibilityRule.OnlyIfPresentInActiveDatabase,
            FallbackBehavior = ArchetypeInherentFallbackBehavior.PreserveImportedData
        };
    }

    private static ArchetypeInherentDefinition CreateComputedPet(
        string catalogKey,
        OmniDataProviderId providerId,
        string owningClassName,
        string powerFullName)
    {
        return new ArchetypeInherentDefinition
        {
            CatalogKey = catalogKey,
            ProviderId = providerId,
            OwningClassName = OmniImportScope.NormalizeClassName(owningClassName),
            PowerFullName = NormalizePowerFullName(powerFullName),
            PresentationType = ArchetypeInherentPresentationType.ComputedFromPetContext,
            BehaviorModel = ArchetypeInherentBehaviorModel.ComputedPetContext,
            VisibilityRule = ArchetypeInherentVisibilityRule.OnlyIfPresentInActiveDatabase,
            FallbackBehavior = ArchetypeInherentFallbackBehavior.PreserveImportedData
        };
    }

    private static ImportedArchetypeInherentBinding CreateBinding(ArchetypeInherentDefinition definition)
    {
        return new ImportedArchetypeInherentBinding
        {
            CatalogKey = definition.CatalogKey,
            ProviderId = definition.ProviderId,
            OwningClassName = definition.OwningClassName,
            PowerFullName = definition.PowerFullName,
            PresentationType = definition.PresentationType,
            BehaviorModel = definition.BehaviorModel,
            VisibilityRule = definition.VisibilityRule,
            FallbackBehavior = definition.FallbackBehavior
        };
    }

    private static AdvancedConditionRow CreateModeRow(
        PlannerMode mode,
        bool shouldBeActive,
        AdvancedConditionLink link,
        string rawExpression)
    {
        return new AdvancedConditionRow
        {
            Link = link,
            Kind = AdvancedConditionKind.SourceMode,
            Subject = PlannerModeMapper.ToCanonicalName(mode),
            Operator = AdvancedConditionOperator.Equals,
            Value = "true",
            Negated = !shouldBeActive,
            RawExpression = rawExpression
        };
    }

    private static AdvancedConditionRow CreateCombatSettingRow(
        string subject,
        bool enabled,
        AdvancedConditionLink link,
        string rawExpression)
    {
        return new AdvancedConditionRow
        {
            Link = link,
            Kind = AdvancedConditionKind.CombatSetting,
            Subject = subject,
            Operator = AdvancedConditionOperator.Equals,
            Value = enabled ? "1" : "0",
            RawExpression = rawExpression
        };
    }

    private static AdvancedConditionRow CreateCombatSettingRow(
        string subject,
        int rawValue,
        AdvancedConditionLink link,
        string rawExpression)
    {
        return new AdvancedConditionRow
        {
            Link = link,
            Kind = AdvancedConditionKind.CombatSetting,
            Subject = subject,
            Operator = AdvancedConditionOperator.Equals,
            Value = rawValue.ToString(CultureInfo.InvariantCulture),
            RawExpression = rawExpression
        };
    }

    private static TierOneArchetypeInherentFamily ResolveTierOneFamily(string ownerFullName)
    {
        if (ownerFullName.StartsWith("Stalker_Melee.", StringComparison.OrdinalIgnoreCase) ||
            (ownerFullName.StartsWith("Villain_Pets.", StringComparison.OrdinalIgnoreCase) &&
             ownerFullName.Contains("_Assassins_Strike.", StringComparison.OrdinalIgnoreCase)))
        {
            return TierOneArchetypeInherentFamily.Assassination;
        }

        if (ownerFullName.StartsWith("Controller_", StringComparison.OrdinalIgnoreCase) ||
            ownerFullName.Equals("Inherent.Inherent.Containment", StringComparison.OrdinalIgnoreCase))
        {
            return TierOneArchetypeInherentFamily.Containment;
        }

        if (ownerFullName.StartsWith("Inherent.Inherent.Opportunity", StringComparison.OrdinalIgnoreCase) ||
            ownerFullName.StartsWith("Temporary_Powers.Temporary_Powers.Opportunity_", StringComparison.OrdinalIgnoreCase) ||
            ownerFullName.Equals("Temporary_Powers.Temporary_Powers.Vulnerability", StringComparison.OrdinalIgnoreCase))
        {
            return TierOneArchetypeInherentFamily.Opportunity;
        }

        if (ownerFullName.StartsWith("Inherent.Inherent.Domination", StringComparison.OrdinalIgnoreCase))
        {
            return TierOneArchetypeInherentFamily.Domination;
        }

        return TierOneArchetypeInherentFamily.None;
    }

    private static bool IsTruthyStateComparison(string @operator, string rawValue)
    {
        if (!double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var threshold))
        {
            return false;
        }

        return @operator switch
        {
            ">" or ">=" => threshold <= 1d,
            "==" or "eq" => threshold > 0d,
            "!=" or "ne" => threshold <= 0d,
            _ => false
        };
    }

    private static bool ContainsAny(string expression, params string[] tokens)
    {
        return tokens.Any(token => expression.Contains(token, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeNumericValue(string rawValue)
    {
        return double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? Math.Round(value).ToString(CultureInfo.InvariantCulture)
            : rawValue;
    }

    private static AdvancedConditionOperator ParseOperator(string op)
    {
        return op switch
        {
            "!=" or "ne" or "<>" => AdvancedConditionOperator.NotEquals,
            ">" => AdvancedConditionOperator.GreaterThan,
            "<" => AdvancedConditionOperator.LessThan,
            ">=" => AdvancedConditionOperator.GreaterThanOrEqual,
            "<=" => AdvancedConditionOperator.LessThanOrEqual,
            _ => AdvancedConditionOperator.Equals
        };
    }

    private static string ComposeLookupKey(
        OmniDataProviderId providerId,
        string owningClassName,
        string powerFullName)
    {
        return string.Join(
            "|",
            ((int)providerId).ToString(),
            OmniImportScope.NormalizeClassName(owningClassName),
            NormalizePowerFullName(powerFullName));
    }

    private static string ComposeProviderPowerKey(OmniDataProviderId providerId, string powerFullName)
    {
        return string.Join(
            "|",
            ((int)providerId).ToString(),
            NormalizePowerFullName(powerFullName));
    }

    private static string NormalizePowerFullName(string powerFullName)
    {
        return (powerFullName ?? string.Empty).Trim();
    }
}

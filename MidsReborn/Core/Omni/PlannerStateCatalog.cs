using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core.Omni;

internal enum PlannerStateFamily
{
    StreetJusticeCombo,
    StaffPerfection,
    AssassinsFocus,
    BloodFrenzy,
    SavageExhausted,
    PackMentality,
    Insight,
    FastMode
}

internal enum PlannerStatePresentationType
{
    ReuseImportedPower,
    SyntheticMarkerControl,
    SyntheticWrapperControl
}

internal enum PlannerStatePayloadType
{
    MarkerOnly,
    RealPayload,
    HiddenPayload
}

internal enum PlannerStateVisibilityRule
{
    AnyPowerInOwningSet,
    SpecificPowerOwned,
    SpecificArchetype
}

internal enum PlannerStateStackMode
{
    None,
    DiscreteMutex,
    VariableCount
}

internal sealed class PlannerStateControlDefinition
{
    public required PlannerStateFamily Family { get; init; }
    public required string FullName { get; init; }
    public required string DisplayName { get; init; }
    public required PlannerStatePresentationType PresentationType { get; init; }
    public required PlannerStatePayloadType PayloadType { get; init; }
    public required PlannerStateVisibilityRule VisibilityRule { get; init; }
    public required PlannerStateStackMode StackMode { get; init; }
    public PlannerMode Mode { get; init; }
    public string IconName { get; init; } = string.Empty;
    public string MutexGroup { get; init; } = string.Empty;
    public string VariableName { get; init; } = "Stacks";
    public int VariableMin { get; init; }
    public int VariableMax { get; init; }
    public int VariableStart { get; init; }
    public string[] OwningSetTokens { get; init; } = [];
    public string[] RequiredPowerNames { get; init; } = [];
    public string[] ArchetypeTokens { get; init; } = [];
    public bool VisibleInInherentGrid { get; init; } = true;

    public bool IsVariableControl => StackMode == PlannerStateStackMode.VariableCount;
    public bool IsModeControl => Mode != PlannerMode.None;
    public bool IsHiddenPayload => PayloadType == PlannerStatePayloadType.HiddenPayload;
}

internal static class PlannerStateCatalog
{
    internal const string ComboLevel1Marker = "Temporary_Powers.Temporary_Powers.Combo_Level_1";
    internal const string ComboLevel2Marker = "Temporary_Powers.Temporary_Powers.Combo_Level_2";
    internal const string ComboLevel3Marker = "Temporary_Powers.Temporary_Powers.Combo_Level_3";

    internal const string PerfectionBody1Marker = "Temporary_Powers.Temporary_Powers.Perfection_of_Body_Level_1";
    internal const string PerfectionBody2Marker = "Temporary_Powers.Temporary_Powers.Perfection_of_Body_Level_2";
    internal const string PerfectionBody3Marker = "Temporary_Powers.Temporary_Powers.Perfection_of_Body_Level_3";
    internal const string PerfectionMind1Marker = "Temporary_Powers.Temporary_Powers.Perfection_of_Mind_Level_1";
    internal const string PerfectionMind2Marker = "Temporary_Powers.Temporary_Powers.Perfection_of_Mind_Level_2";
    internal const string PerfectionMind3Marker = "Temporary_Powers.Temporary_Powers.Perfection_of_Mind_Level_3";
    internal const string PerfectionSoul1Marker = "Temporary_Powers.Temporary_Powers.Perfection_of_Soul_Level_1";
    internal const string PerfectionSoul2Marker = "Temporary_Powers.Temporary_Powers.Perfection_of_Soul_Level_2";
    internal const string PerfectionSoul3Marker = "Temporary_Powers.Temporary_Powers.Perfection_of_Soul_Level_3";
    internal const string StaffPerfectionRedirect = "Redirects.Staff_Melee.Perfection_Level";

    internal const string BloodFrenzyMarker = "Temporary_Powers.Temporary_Powers.Savage_Melee_Blood_Frenzy_Stalker";
    internal const string SavageExhaustedMarker = "Temporary_Powers.Temporary_Powers.Savage_Melee_Exhausted";
    internal const string PackMentalityMarker = "Temporary_Powers.Temporary_Powers.Pack_Mentality";
    internal const string InsightMarker = "Temporary_Powers.Temporary_Powers.Psionic_Melee_Insight";
    internal const string AssassinsFocusMarker = "Temporary_Powers.Temporary_Powers.Assassins_Focus";

    internal const string MomentumPowerFullName = "Inherent.Inherent.Fast_Mode";
    internal const string PerfectionLevel1PowerFullName = "Inherent.Inherent.Perfection_Level_1";
    internal const string PerfectionLevel2PowerFullName = "Inherent.Inherent.Perfection_Level_2";
    internal const string PerfectionLevel3PowerFullName = "Inherent.Inherent.Perfection_Level_3";
    private const string ComboMutexGroup = "Planner.ComboLevel";
    internal const string StaffFormMutexGroup = "Planner.StaffForm";
    private const string StaffPerfectionMutexGroup = "Planner.StaffPerfectionLevel";
    private static readonly string[] PlannerMutexGroups =
    [
        ComboMutexGroup,
        StaffFormMutexGroup,
        StaffPerfectionMutexGroup
    ];

    private static readonly Regex OwnPowerNumRegex = new(
        @"(?:(?:source\.)?ownPowerNum\?\()(?<power>[A-Za-z0-9_\-\.]+)\)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly PlannerStateControlDefinition[] ControlDefinitions =
    [
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StreetJusticeCombo,
            FullName = ComboLevel1Marker,
            DisplayName = "Combo 1",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.MarkerOnly,
            VisibilityRule = PlannerStateVisibilityRule.AnyPowerInOwningSet,
            StackMode = PlannerStateStackMode.DiscreteMutex,
            Mode = PlannerMode.ComboLevel1,
            MutexGroup = ComboMutexGroup,
            OwningSetTokens = ["streetjustice"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StreetJusticeCombo,
            FullName = ComboLevel2Marker,
            DisplayName = "Combo 2",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.MarkerOnly,
            VisibilityRule = PlannerStateVisibilityRule.AnyPowerInOwningSet,
            StackMode = PlannerStateStackMode.DiscreteMutex,
            Mode = PlannerMode.ComboLevel2,
            MutexGroup = ComboMutexGroup,
            OwningSetTokens = ["streetjustice"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StreetJusticeCombo,
            FullName = ComboLevel3Marker,
            DisplayName = "Combo 3",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.MarkerOnly,
            VisibilityRule = PlannerStateVisibilityRule.AnyPowerInOwningSet,
            StackMode = PlannerStateStackMode.DiscreteMutex,
            Mode = PlannerMode.ComboLevel3,
            MutexGroup = ComboMutexGroup,
            OwningSetTokens = ["streetjustice"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionLevel1PowerFullName,
            DisplayName = "Perfection 1",
            PresentationType = PlannerStatePresentationType.SyntheticWrapperControl,
            PayloadType = PlannerStatePayloadType.MarkerOnly,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.DiscreteMutex,
            Mode = PlannerMode.PerfectionLevel1,
            MutexGroup = StaffPerfectionMutexGroup,
            IconName = "stafffighting_perfectionofbodylevel1.png",
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionLevel2PowerFullName,
            DisplayName = "Perfection 2",
            PresentationType = PlannerStatePresentationType.SyntheticWrapperControl,
            PayloadType = PlannerStatePayloadType.MarkerOnly,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.DiscreteMutex,
            Mode = PlannerMode.PerfectionLevel2,
            MutexGroup = StaffPerfectionMutexGroup,
            IconName = "stafffighting_perfectionofbodylevel2.png",
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionLevel3PowerFullName,
            DisplayName = "Perfection 3",
            PresentationType = PlannerStatePresentationType.SyntheticWrapperControl,
            PayloadType = PlannerStatePayloadType.MarkerOnly,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.DiscreteMutex,
            Mode = PlannerMode.PerfectionLevel3,
            MutexGroup = StaffPerfectionMutexGroup,
            IconName = "stafffighting_perfectionofbodylevel3.png",
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionBody1Marker,
            DisplayName = "Perfection of Body Level 1",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.HiddenPayload,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.None,
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"],
            VisibleInInherentGrid = false
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionBody2Marker,
            DisplayName = "Perfection of Body Level 2",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.HiddenPayload,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.None,
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"],
            VisibleInInherentGrid = false
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionBody3Marker,
            DisplayName = "Perfection of Body Level 3",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.HiddenPayload,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.None,
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"],
            VisibleInInherentGrid = false
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionMind1Marker,
            DisplayName = "Perfection of Mind Level 1",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.HiddenPayload,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.None,
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"],
            VisibleInInherentGrid = false
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionMind2Marker,
            DisplayName = "Perfection of Mind Level 2",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.HiddenPayload,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.None,
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"],
            VisibleInInherentGrid = false
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionMind3Marker,
            DisplayName = "Perfection of Mind Level 3",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.HiddenPayload,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.None,
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"],
            VisibleInInherentGrid = false
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionSoul1Marker,
            DisplayName = "Perfection of Soul Level 1",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.HiddenPayload,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.None,
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"],
            VisibleInInherentGrid = false
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionSoul2Marker,
            DisplayName = "Perfection of Soul Level 2",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.HiddenPayload,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.None,
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"],
            VisibleInInherentGrid = false
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.StaffPerfection,
            FullName = PerfectionSoul3Marker,
            DisplayName = "Perfection of Soul Level 3",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.HiddenPayload,
            VisibilityRule = PlannerStateVisibilityRule.SpecificPowerOwned,
            StackMode = PlannerStateStackMode.None,
            OwningSetTokens = ["stafffighting"],
            RequiredPowerNames = ["staffmastery"],
            VisibleInInherentGrid = false
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.AssassinsFocus,
            FullName = AssassinsFocusMarker,
            DisplayName = "Assassin's Focus",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.RealPayload,
            VisibilityRule = PlannerStateVisibilityRule.SpecificArchetype,
            StackMode = PlannerStateStackMode.VariableCount,
            VariableMin = 0,
            VariableMax = 3,
            VariableStart = 0,
            VariableName = "Stacks",
            ArchetypeTokens = ["stalker"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.BloodFrenzy,
            FullName = BloodFrenzyMarker,
            DisplayName = "Blood Frenzy",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.RealPayload,
            VisibilityRule = PlannerStateVisibilityRule.AnyPowerInOwningSet,
            StackMode = PlannerStateStackMode.VariableCount,
            VariableMin = 0,
            VariableMax = 5,
            VariableStart = 0,
            VariableName = "Stacks",
            OwningSetTokens = ["savagemelee"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.SavageExhausted,
            FullName = SavageExhaustedMarker,
            DisplayName = "Exhausted",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.MarkerOnly,
            VisibilityRule = PlannerStateVisibilityRule.AnyPowerInOwningSet,
            StackMode = PlannerStateStackMode.None,
            Mode = PlannerMode.Exhausted,
            OwningSetTokens = ["savagemelee"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.PackMentality,
            FullName = PackMentalityMarker,
            DisplayName = "Pack Mentality",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.RealPayload,
            VisibilityRule = PlannerStateVisibilityRule.AnyPowerInOwningSet,
            StackMode = PlannerStateStackMode.VariableCount,
            VariableMin = 0,
            VariableMax = 10,
            VariableStart = 0,
            VariableName = "Stacks",
            OwningSetTokens = ["beastmastery"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.Insight,
            FullName = InsightMarker,
            DisplayName = "Insight",
            PresentationType = PlannerStatePresentationType.ReuseImportedPower,
            PayloadType = PlannerStatePayloadType.MarkerOnly,
            VisibilityRule = PlannerStateVisibilityRule.AnyPowerInOwningSet,
            StackMode = PlannerStateStackMode.None,
            Mode = PlannerMode.Insight,
            OwningSetTokens = ["psionicmelee"]
        },
        new PlannerStateControlDefinition
        {
            Family = PlannerStateFamily.FastMode,
            FullName = MomentumPowerFullName,
            DisplayName = "Momentum",
            PresentationType = PlannerStatePresentationType.SyntheticMarkerControl,
            PayloadType = PlannerStatePayloadType.MarkerOnly,
            VisibilityRule = PlannerStateVisibilityRule.AnyPowerInOwningSet,
            StackMode = PlannerStateStackMode.None,
            Mode = PlannerMode.FastMode,
            IconName = "titanweapons_buildup.png",
            OwningSetTokens = ["titanweapons"]
        }
    ];

    private static readonly Dictionary<string, PlannerStateControlDefinition> DefinitionsByFullName = ControlDefinitions
        .ToDictionary(definition => definition.FullName, definition => definition, StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, PlannerStateFamily> ModeFamilies = new(StringComparer.OrdinalIgnoreCase)
    {
        [PlannerModeMapper.ToCanonicalName(PlannerMode.ComboLevel1)] = PlannerStateFamily.StreetJusticeCombo,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.ComboLevel2)] = PlannerStateFamily.StreetJusticeCombo,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.ComboLevel3)] = PlannerStateFamily.StreetJusticeCombo,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionLevel1)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionLevel2)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionLevel3)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfBody)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfMind)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfSoul)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfBody1)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfBody2)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfBody3)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfMind1)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfMind2)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfMind3)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfSoul1)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfSoul2)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.PerfectionOfSoul3)] = PlannerStateFamily.StaffPerfection,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.Assassination)] = PlannerStateFamily.AssassinsFocus,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.Exhausted)] = PlannerStateFamily.SavageExhausted,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.Insight)] = PlannerStateFamily.Insight,
        [PlannerModeMapper.ToCanonicalName(PlannerMode.FastMode)] = PlannerStateFamily.FastMode
    };

    private static readonly Dictionary<PlannerMode, PlannerMode[]> ExclusiveModeFamilies = new()
    {
        [PlannerMode.ComboLevel1] = [PlannerMode.ComboLevel1, PlannerMode.ComboLevel2, PlannerMode.ComboLevel3],
        [PlannerMode.ComboLevel2] = [PlannerMode.ComboLevel1, PlannerMode.ComboLevel2, PlannerMode.ComboLevel3],
        [PlannerMode.ComboLevel3] = [PlannerMode.ComboLevel1, PlannerMode.ComboLevel2, PlannerMode.ComboLevel3],
        [PlannerMode.PerfectionLevel1] = [PlannerMode.PerfectionLevel1, PlannerMode.PerfectionLevel2, PlannerMode.PerfectionLevel3],
        [PlannerMode.PerfectionLevel2] = [PlannerMode.PerfectionLevel1, PlannerMode.PerfectionLevel2, PlannerMode.PerfectionLevel3],
        [PlannerMode.PerfectionLevel3] = [PlannerMode.PerfectionLevel1, PlannerMode.PerfectionLevel2, PlannerMode.PerfectionLevel3],
        [PlannerMode.PerfectionOfBody] = [PlannerMode.PerfectionOfBody, PlannerMode.PerfectionOfMind, PlannerMode.PerfectionOfSoul],
        [PlannerMode.PerfectionOfMind] = [PlannerMode.PerfectionOfBody, PlannerMode.PerfectionOfMind, PlannerMode.PerfectionOfSoul],
        [PlannerMode.PerfectionOfSoul] = [PlannerMode.PerfectionOfBody, PlannerMode.PerfectionOfMind, PlannerMode.PerfectionOfSoul]
    };

    private static readonly (string Suffix, PlannerMode[] Modes)[] ImpliedPlannerModesByPowerSuffix =
    [
        (".Street_Justice.Combat_Readiness", [PlannerMode.ComboLevel3])
    ];

    private static readonly HashSet<string> SelfStateMarkers =
    [
        ComboLevel1Marker,
        ComboLevel2Marker,
        ComboLevel3Marker,
        PerfectionBody1Marker,
        PerfectionBody2Marker,
        PerfectionBody3Marker,
        PerfectionMind1Marker,
        PerfectionMind2Marker,
        PerfectionMind3Marker,
        PerfectionSoul1Marker,
        PerfectionSoul2Marker,
        PerfectionSoul3Marker,
        StaffPerfectionRedirect,
        BloodFrenzyMarker,
        SavageExhaustedMarker,
        PackMentalityMarker,
        InsightMarker,
        AssassinsFocusMarker
    ];

    internal static IReadOnlyList<PlannerStateControlDefinition> Definitions => ControlDefinitions;

    internal static string[] MergePlannerMutexGroups(IEnumerable<string>? existingMutexGroups)
    {
        return (existingMutexGroups ?? [])
            .Where(group => !string.IsNullOrWhiteSpace(group))
            .Concat(PlannerMutexGroups)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    internal static IReadOnlyList<PlannerStateControlDefinition> GetDefinitions(PlannerStateFamily family)
    {
        return ControlDefinitions.Where(definition => definition.Family == family).ToArray();
    }

    internal static bool TryGetDefinition(string? powerFullName, out PlannerStateControlDefinition definition)
    {
        definition = null!;
        return !string.IsNullOrWhiteSpace(powerFullName) &&
               DefinitionsByFullName.TryGetValue(powerFullName, out definition);
    }

    internal static bool IsAutoGrantedPlannerStatePower(string? powerFullName)
    {
        return TryGetDefinition(powerFullName, out _);
    }

    internal static bool TryGetExclusiveModeFamily(PlannerMode mode, out PlannerMode[] familyModes)
    {
        return ExclusiveModeFamilies.TryGetValue(mode, out familyModes!);
    }

    internal static bool TryGetImpliedPlannerModes(string? powerFullName, out PlannerMode[] modes)
    {
        modes = [];
        if (string.IsNullOrWhiteSpace(powerFullName))
        {
            return false;
        }

        foreach (var (suffix, mappedModes) in ImpliedPlannerModesByPowerSuffix)
        {
            if (powerFullName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                modes = mappedModes;
                return modes.Length > 0;
            }
        }

        return false;
    }

    internal static bool TryEvaluateOwnPowerState(
        string? powerFullName,
        Func<PlannerMode, bool> isModeActive,
        Func<string, int> getStacks,
        bool isStalker,
        out bool isActive)
    {
        isActive = false;
        if (string.IsNullOrWhiteSpace(powerFullName))
        {
            return false;
        }

        switch (powerFullName)
        {
            case ComboLevel1Marker:
                isActive = isModeActive(PlannerMode.ComboLevel1);
                return true;
            case ComboLevel2Marker:
                isActive = isModeActive(PlannerMode.ComboLevel2);
                return true;
            case ComboLevel3Marker:
                isActive = isModeActive(PlannerMode.ComboLevel3);
                return true;
            case InsightMarker:
                isActive = isModeActive(PlannerMode.Insight);
                return true;
            case SavageExhaustedMarker:
                isActive = isModeActive(PlannerMode.Exhausted);
                return true;
            case AssassinsFocusMarker:
                isActive = getStacks(AssassinsFocusMarker) > 0;
                return true;
            case BloodFrenzyMarker:
            case PackMentalityMarker:
                isActive = getStacks(powerFullName) > 0;
                return true;
            case PerfectionBody1Marker:
                isActive = (isStalker || isModeActive(PlannerMode.PerfectionOfBody)) &&
                           isModeActive(PlannerMode.PerfectionLevel1);
                return true;
            case PerfectionBody2Marker:
                isActive = (isStalker || isModeActive(PlannerMode.PerfectionOfBody)) &&
                           isModeActive(PlannerMode.PerfectionLevel2);
                return true;
            case PerfectionBody3Marker:
                isActive = (isStalker || isModeActive(PlannerMode.PerfectionOfBody)) &&
                           isModeActive(PlannerMode.PerfectionLevel3);
                return true;
            case PerfectionMind1Marker:
                isActive = !isStalker &&
                           isModeActive(PlannerMode.PerfectionOfMind) &&
                           isModeActive(PlannerMode.PerfectionLevel1);
                return true;
            case PerfectionMind2Marker:
                isActive = !isStalker &&
                           isModeActive(PlannerMode.PerfectionOfMind) &&
                           isModeActive(PlannerMode.PerfectionLevel2);
                return true;
            case PerfectionMind3Marker:
                isActive = !isStalker &&
                           isModeActive(PlannerMode.PerfectionOfMind) &&
                           isModeActive(PlannerMode.PerfectionLevel3);
                return true;
            case PerfectionSoul1Marker:
                isActive = !isStalker &&
                           isModeActive(PlannerMode.PerfectionOfSoul) &&
                           isModeActive(PlannerMode.PerfectionLevel1);
                return true;
            case PerfectionSoul2Marker:
                isActive = !isStalker &&
                           isModeActive(PlannerMode.PerfectionOfSoul) &&
                           isModeActive(PlannerMode.PerfectionLevel2);
                return true;
            case PerfectionSoul3Marker:
                isActive = !isStalker &&
                           isModeActive(PlannerMode.PerfectionOfSoul) &&
                           isModeActive(PlannerMode.PerfectionLevel3);
                return true;
            default:
                return false;
        }
    }

    internal static bool TryEvaluatePlannerPowerCount(
        string? powerFullName,
        Func<PlannerMode, bool> isModeActive,
        Func<string, int> getStacks,
        bool isStalker,
        out int count)
    {
        count = 0;
        if (!TryEvaluateOwnPowerState(powerFullName, isModeActive, getStacks, isStalker, out var isActive))
        {
            return false;
        }

        count = powerFullName switch
        {
            BloodFrenzyMarker or PackMentalityMarker or AssassinsFocusMarker => getStacks(powerFullName!),
            _ => isActive ? 1 : 0
        };

        return true;
    }

    internal static bool IsVariablePlannerPower(string? powerFullName)
    {
        return TryGetDefinition(powerFullName, out var definition) &&
               definition.IsVariableControl;
    }

    internal static bool IsSyntheticStackPlannerPower(string? powerFullName)
    {
        return IsVariablePlannerPower(powerFullName);
    }

    internal static bool IsHiddenPayloadPower(string? powerFullName)
    {
        return TryGetDefinition(powerFullName, out var definition) &&
               definition.IsHiddenPayload;
    }

    internal static bool IsSelfStateMarker(string? powerFullName)
    {
        return !string.IsNullOrWhiteSpace(powerFullName) &&
               SelfStateMarkers.Contains(powerFullName);
    }

    internal static bool HasCatalogAutoGrantedPowers(IPowerset? powerset)
    {
        return powerset?.Powers.Any(power => power != null && IsAutoGrantedPlannerStatePower(power.FullName)) == true;
    }

    internal static bool ShouldAutoGrantPower(Build build, IPower? power)
    {
        if (power == null || !TryGetDefinition(power.FullName, out var definition))
        {
            return false;
        }

        return definition.VisibilityRule switch
        {
            PlannerStateVisibilityRule.AnyPowerInOwningSet => HasChosenPowerInOwningSet(build, definition.OwningSetTokens),
            PlannerStateVisibilityRule.SpecificPowerOwned => HasChosenRequiredPower(build, definition.OwningSetTokens, definition.RequiredPowerNames),
            PlannerStateVisibilityRule.SpecificArchetype => MatchesCharacterArchetype(build, definition.ArchetypeTokens),
            _ => false
        };
    }

    internal static bool TryRewriteSourceOwnPowerRow(
        AdvancedConditionRow row,
        out PlannerStateFamily family,
        out List<AdvancedConditionRow> rewrittenRows)
    {
        family = default;
        rewrittenRows = [];
        var subject = row.Subject?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(subject))
        {
            return false;
        }

        switch (subject)
        {
            case ComboLevel1Marker:
                family = PlannerStateFamily.StreetJusticeCombo;
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.ComboLevel1));
                return true;
            case ComboLevel2Marker:
                family = PlannerStateFamily.StreetJusticeCombo;
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.ComboLevel2));
                return true;
            case ComboLevel3Marker:
                family = PlannerStateFamily.StreetJusticeCombo;
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.ComboLevel3));
                return true;
            case InsightMarker:
                family = PlannerStateFamily.Insight;
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.Insight));
                return true;
            case SavageExhaustedMarker:
                family = PlannerStateFamily.SavageExhausted;
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.Exhausted));
                return true;
            case AssassinsFocusMarker:
                family = PlannerStateFamily.AssassinsFocus;
                rewrittenRows.Add(CloneAsPowerStacks(row, AssassinsFocusMarker));
                return true;
            case BloodFrenzyMarker:
                family = PlannerStateFamily.BloodFrenzy;
                rewrittenRows.Add(CloneAsPowerStacks(row, BloodFrenzyMarker));
                return true;
            case PackMentalityMarker:
                family = PlannerStateFamily.PackMentality;
                rewrittenRows.Add(CloneAsPowerStacks(row, PackMentalityMarker));
                return true;
        }

        if (TryRewritePerfectionMarkerRow(row, subject, out family, out rewrittenRows))
        {
            return true;
        }

        return false;
    }

    private static bool TryRewritePerfectionMarkerRow(
        AdvancedConditionRow row,
        string subject,
        out PlannerStateFamily family,
        out List<AdvancedConditionRow> rewrittenRows)
    {
        family = PlannerStateFamily.StaffPerfection;
        rewrittenRows = [];

        switch (subject)
        {
            case PerfectionBody1Marker:
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionOfBody));
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionLevel1));
                return true;
            case PerfectionBody2Marker:
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionOfBody));
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionLevel2));
                return true;
            case PerfectionBody3Marker:
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionOfBody));
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionLevel3));
                return true;
            case PerfectionMind1Marker:
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionOfMind));
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionLevel1));
                return true;
            case PerfectionMind2Marker:
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionOfMind));
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionLevel2));
                return true;
            case PerfectionMind3Marker:
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionOfMind));
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionLevel3));
                return true;
            case PerfectionSoul1Marker:
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionOfSoul));
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionLevel1));
                return true;
            case PerfectionSoul2Marker:
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionOfSoul));
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionLevel2));
                return true;
            case PerfectionSoul3Marker:
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionOfSoul));
                rewrittenRows.Add(CloneAsSourceMode(row, PlannerMode.PerfectionLevel3));
                return true;
            default:
                family = default;
                rewrittenRows = [];
                return false;
        }
    }

    internal static bool TryRewritePowerCountRow(
        AdvancedConditionRow row,
        out PlannerStateFamily family,
        out AdvancedConditionRow rewrittenRow)
    {
        rewrittenRow = row.Clone();
        family = default;
        var subject = row.Subject?.Trim() ?? string.Empty;
        switch (subject)
        {
            case AssassinsFocusMarker:
                family = PlannerStateFamily.AssassinsFocus;
                rewrittenRow.Kind = AdvancedConditionKind.PowerStacks;
                rewrittenRow.Subject = AssassinsFocusMarker;
                return true;
            case BloodFrenzyMarker:
                family = PlannerStateFamily.BloodFrenzy;
                rewrittenRow.Kind = AdvancedConditionKind.PowerStacks;
                rewrittenRow.Subject = BloodFrenzyMarker;
                return true;
            case PackMentalityMarker:
                family = PlannerStateFamily.PackMentality;
                rewrittenRow.Kind = AdvancedConditionKind.PowerStacks;
                rewrittenRow.Subject = PackMentalityMarker;
                return true;
            default:
                return false;
        }
    }

    internal static bool TryRewriteNumericExpression(
        string? expression,
        out string rewrittenExpression,
        ISet<PlannerStateFamily>? referencedFamilies = null)
    {
        rewrittenExpression = expression ?? string.Empty;
        if (string.IsNullOrWhiteSpace(expression))
        {
            return false;
        }

        var rewritten = false;
        rewrittenExpression = OwnPowerNumRegex.Replace(expression, match =>
        {
            var powerName = match.Groups["power"].Value.Trim();
            switch (powerName)
            {
                case AssassinsFocusMarker:
                    referencedFamilies?.Add(PlannerStateFamily.AssassinsFocus);
                    rewritten = true;
                    return $"{AssassinsFocusMarker}>variableVal";
                case BloodFrenzyMarker:
                    referencedFamilies?.Add(PlannerStateFamily.BloodFrenzy);
                    rewritten = true;
                    return $"{BloodFrenzyMarker}>variableVal";
                case PackMentalityMarker:
                    referencedFamilies?.Add(PlannerStateFamily.PackMentality);
                    rewritten = true;
                    return $"{PackMentalityMarker}>variableVal";
                default:
                    return match.Value;
            }
        });

        return rewritten;
    }

    internal static bool TryGetFamilyForSourceMode(string? modeName, out PlannerStateFamily family)
    {
        family = default;
        if (!PlannerModeMapper.TryGetPlannerMode(modeName, out var mode))
        {
            return false;
        }

        var canonicalName = PlannerModeMapper.ToCanonicalName(mode);
        return ModeFamilies.TryGetValue(canonicalName, out family);
    }

    internal static bool TryGetStateFamily(string? powerFullName, out PlannerStateFamily family)
    {
        family = default;
        if (string.IsNullOrWhiteSpace(powerFullName))
        {
            return false;
        }

        if (TryGetDefinition(powerFullName, out var definition))
        {
            family = definition.Family;
            return true;
        }

        if (powerFullName.Equals(StaffPerfectionRedirect, StringComparison.OrdinalIgnoreCase))
        {
            family = PlannerStateFamily.StaffPerfection;
            return true;
        }

        return false;
    }

    internal static string ResolveVariableSourcePower(string? powerFullName)
    {
        return string.IsNullOrWhiteSpace(powerFullName)
            ? string.Empty
            : powerFullName;
    }

    private static bool HasChosenPowerInOwningSet(Build build, IEnumerable<string> setTokens)
    {
        var tokenSet = setTokens
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .Select(NormalizeToken)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (tokenSet.Count == 0)
        {
            return false;
        }

        return build.Powers.Any(entry =>
            entry is { Chosen: true, Power: not null } &&
            tokenSet.Contains(NormalizeToken(entry.Power.SetName)));
    }

    private static bool HasChosenRequiredPower(Build build, IEnumerable<string> setTokens, IEnumerable<string> powerNames)
    {
        var tokenSet = setTokens
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .Select(NormalizeToken)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var requiredNames = powerNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(NormalizeToken)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (tokenSet.Count == 0 || requiredNames.Count == 0)
        {
            return false;
        }

        return build.Powers.Any(entry =>
            entry is { Chosen: true, Power: not null } &&
            tokenSet.Contains(NormalizeToken(entry.Power.SetName)) &&
            requiredNames.Contains(NormalizeToken(entry.Power.PowerName)));
    }

    private static bool MatchesCharacterArchetype(Build build, IEnumerable<string> archetypeTokens)
    {
        var tokenSet = archetypeTokens
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .Select(NormalizeToken)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (tokenSet.Count == 0)
        {
            return false;
        }

        var archetype = MidsContext.Character?.Archetype;
        if (archetype == null)
        {
            return false;
        }

        return tokenSet.Contains(NormalizeToken(archetype.ClassName)) ||
               tokenSet.Contains(NormalizeToken(archetype.DisplayName));
    }

    private static string NormalizeToken(string? value)
    {
        return Regex.Replace(value ?? string.Empty, "[^A-Za-z0-9]", string.Empty)
            .ToLowerInvariant();
    }

    private static AdvancedConditionRow CloneAsSourceMode(AdvancedConditionRow row, PlannerMode mode)
    {
        var clone = row.Clone();
        clone.Kind = AdvancedConditionKind.SourceMode;
        clone.Subject = PlannerModeMapper.ToCanonicalName(mode);
        return clone;
    }

    private static AdvancedConditionRow CloneAsPowerStacks(AdvancedConditionRow row, string powerFullName)
    {
        var clone = row.Clone();
        clone.Kind = AdvancedConditionKind.PowerStacks;
        clone.Subject = powerFullName;
        clone.Operator = AdvancedConditionOperator.GreaterThan;
        clone.Value = "0";
        return clone;
    }
}

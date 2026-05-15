namespace Mids_Reborn.Core;

public enum PlannerMode
{
    None,
    FastSnipe,
    Containment,
    Domination,
    DominationActive,
    Scourge,
    CriticalHit,
    Assassination,
    StalkerHidden,
    Defiance,
    DefensiveAdaptation,
    EfficientAdaptation,
    OffensiveAdaptation,
    ComboLevel1,
    ComboLevel2,
    ComboLevel3,
    FastMode,
    Insight,
    Exhausted,
    PerfectionLevel1,
    PerfectionLevel2,
    PerfectionLevel3,
    PerfectionOfBody,
    PerfectionOfBody1,
    PerfectionOfBody2,
    PerfectionOfBody3,
    PerfectionOfMind,
    PerfectionOfMind1,
    PerfectionOfMind2,
    PerfectionOfMind3,
    PerfectionOfSoul,
    PerfectionOfSoul1,
    PerfectionOfSoul2,
    PerfectionOfSoul3,
    PackMentality
}

public static class PlannerModeMapper
{
    private static readonly Dictionary<string, PlannerMode> Modes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["FastSnipe"] = PlannerMode.FastSnipe,
        ["Fast_Snipe"] = PlannerMode.FastSnipe,
        ["Containment"] = PlannerMode.Containment,
        ["Domination"] = PlannerMode.Domination,
        ["DominationActive"] = PlannerMode.DominationActive,
        ["Domination_Active"] = PlannerMode.DominationActive,
        ["Scourge"] = PlannerMode.Scourge,
        ["CriticalHit"] = PlannerMode.CriticalHit,
        ["Critical_Hit"] = PlannerMode.CriticalHit,
        ["Assassination"] = PlannerMode.Assassination,
        ["StalkerHidden"] = PlannerMode.StalkerHidden,
        ["Stalker_Hidden"] = PlannerMode.StalkerHidden,
        ["FromHide"] = PlannerMode.StalkerHidden,
        ["From_Hide"] = PlannerMode.StalkerHidden,
        ["Defiance"] = PlannerMode.Defiance,
        ["DefensiveAdaptation"] = PlannerMode.DefensiveAdaptation,
        ["Defensive_Adaptation"] = PlannerMode.DefensiveAdaptation,
        ["EfficientAdaptation"] = PlannerMode.EfficientAdaptation,
        ["Efficient_Adaptation"] = PlannerMode.EfficientAdaptation,
        ["OffensiveAdaptation"] = PlannerMode.OffensiveAdaptation,
        ["Offensive_Adaptation"] = PlannerMode.OffensiveAdaptation,
        ["ComboLevel1"] = PlannerMode.ComboLevel1,
        ["Combo_Level_1"] = PlannerMode.ComboLevel1,
        ["ComboLevel2"] = PlannerMode.ComboLevel2,
        ["Combo_Level_2"] = PlannerMode.ComboLevel2,
        ["ComboLevel3"] = PlannerMode.ComboLevel3,
        ["Combo_Level_3"] = PlannerMode.ComboLevel3,
        ["FastMode"] = PlannerMode.FastMode,
        ["Fast_Mode"] = PlannerMode.FastMode,
        ["Insight"] = PlannerMode.Insight,
        ["Exhausted"] = PlannerMode.Exhausted,
        ["Savage_Melee_Exhausted"] = PlannerMode.Exhausted,
        ["PerfectionLevel1"] = PlannerMode.PerfectionLevel1,
        ["Perfection_Level_1"] = PlannerMode.PerfectionLevel1,
        ["PerfectionLevel2"] = PlannerMode.PerfectionLevel2,
        ["Perfection_Level_2"] = PlannerMode.PerfectionLevel2,
        ["PerfectionLevel3"] = PlannerMode.PerfectionLevel3,
        ["Perfection_Level_3"] = PlannerMode.PerfectionLevel3,
        ["PerfectionOfBody"] = PlannerMode.PerfectionOfBody,
        ["FormOfTheBody"] = PlannerMode.PerfectionOfBody,
        ["Form_of_the_Body"] = PlannerMode.PerfectionOfBody,
        ["PerfectionOfBody1"] = PlannerMode.PerfectionOfBody1,
        ["Perfection_of_Body_Level_1"] = PlannerMode.PerfectionOfBody1,
        ["PerfectionOfBody2"] = PlannerMode.PerfectionOfBody2,
        ["Perfection_of_Body_Level_2"] = PlannerMode.PerfectionOfBody2,
        ["PerfectionOfBody3"] = PlannerMode.PerfectionOfBody3,
        ["Perfection_of_Body_Level_3"] = PlannerMode.PerfectionOfBody3,
        ["PerfectionOfMind"] = PlannerMode.PerfectionOfMind,
        ["FormOfTheMind"] = PlannerMode.PerfectionOfMind,
        ["Form_of_the_Mind"] = PlannerMode.PerfectionOfMind,
        ["PerfectionOfMind1"] = PlannerMode.PerfectionOfMind1,
        ["Perfection_of_Mind_Level_1"] = PlannerMode.PerfectionOfMind1,
        ["PerfectionOfMind2"] = PlannerMode.PerfectionOfMind2,
        ["Perfection_of_Mind_Level_2"] = PlannerMode.PerfectionOfMind2,
        ["PerfectionOfMind3"] = PlannerMode.PerfectionOfMind3,
        ["Perfection_of_Mind_Level_3"] = PlannerMode.PerfectionOfMind3,
        ["PerfectionOfSoul"] = PlannerMode.PerfectionOfSoul,
        ["FormOfTheSoul"] = PlannerMode.PerfectionOfSoul,
        ["Form_of_the_Soul"] = PlannerMode.PerfectionOfSoul,
        ["PerfectionOfSoul1"] = PlannerMode.PerfectionOfSoul1,
        ["Perfection_of_Soul_Level_1"] = PlannerMode.PerfectionOfSoul1,
        ["PerfectionOfSoul2"] = PlannerMode.PerfectionOfSoul2,
        ["Perfection_of_Soul_Level_2"] = PlannerMode.PerfectionOfSoul2,
        ["PerfectionOfSoul3"] = PlannerMode.PerfectionOfSoul3,
        ["Perfection_of_Soul_Level_3"] = PlannerMode.PerfectionOfSoul3,
        ["PackMentality"] = PlannerMode.PackMentality,
        ["Pack_Mentality"] = PlannerMode.PackMentality
    };

    public static IReadOnlyList<string> KnownModeNames { get; } = Modes.Keys
        .Concat(Modes.Values.Select(ToCanonicalName))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public static bool TryGetPlannerMode(string? name, out PlannerMode mode)
    {
        mode = PlannerMode.None;
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var cleaned = name.Trim().Trim('\'', '"').Replace(" ", string.Empty);
        return Modes.TryGetValue(cleaned, out mode) && mode != PlannerMode.None;
    }

    public static string ToCanonicalName(PlannerMode mode)
    {
        return mode switch
        {
            PlannerMode.FastSnipe => "FastSnipe",
            PlannerMode.Containment => "Containment",
            PlannerMode.Domination => "Domination",
            PlannerMode.DominationActive => "DominationActive",
            PlannerMode.Scourge => "Scourge",
            PlannerMode.CriticalHit => "CriticalHit",
            PlannerMode.Assassination => "Assassination",
            PlannerMode.StalkerHidden => "StalkerHidden",
            PlannerMode.Defiance => "Defiance",
            PlannerMode.DefensiveAdaptation => "DefensiveAdaptation",
            PlannerMode.EfficientAdaptation => "EfficientAdaptation",
            PlannerMode.OffensiveAdaptation => "OffensiveAdaptation",
            PlannerMode.ComboLevel1 => "ComboLevel1",
            PlannerMode.ComboLevel2 => "ComboLevel2",
            PlannerMode.ComboLevel3 => "ComboLevel3",
            PlannerMode.FastMode => "FastMode",
            PlannerMode.Insight => "Insight",
            PlannerMode.Exhausted => "Exhausted",
            PlannerMode.PerfectionLevel1 => "PerfectionLevel1",
            PlannerMode.PerfectionLevel2 => "PerfectionLevel2",
            PlannerMode.PerfectionLevel3 => "PerfectionLevel3",
            PlannerMode.PerfectionOfBody => "PerfectionOfBody",
            PlannerMode.PerfectionOfBody1 => "PerfectionOfBody1",
            PlannerMode.PerfectionOfBody2 => "PerfectionOfBody2",
            PlannerMode.PerfectionOfBody3 => "PerfectionOfBody3",
            PlannerMode.PerfectionOfMind => "PerfectionOfMind",
            PlannerMode.PerfectionOfMind1 => "PerfectionOfMind1",
            PlannerMode.PerfectionOfMind2 => "PerfectionOfMind2",
            PlannerMode.PerfectionOfMind3 => "PerfectionOfMind3",
            PlannerMode.PerfectionOfSoul => "PerfectionOfSoul",
            PlannerMode.PerfectionOfSoul1 => "PerfectionOfSoul1",
            PlannerMode.PerfectionOfSoul2 => "PerfectionOfSoul2",
            PlannerMode.PerfectionOfSoul3 => "PerfectionOfSoul3",
            PlannerMode.PackMentality => "PackMentality",
            _ => string.Empty
        };
    }

    public static string ToPowerName(PlannerMode mode)
    {
        return mode switch
        {
            PlannerMode.FastSnipe => "Fast_Snipe",
            PlannerMode.CriticalHit => "Critical_Hit",
            PlannerMode.DefensiveAdaptation => "Defensive_Adaptation",
            PlannerMode.EfficientAdaptation => "Efficient_Adaptation",
            PlannerMode.OffensiveAdaptation => "Offensive_Adaptation",
            PlannerMode.ComboLevel1 => "Combo_Level_1",
            PlannerMode.ComboLevel2 => "Combo_Level_2",
            PlannerMode.ComboLevel3 => "Combo_Level_3",
            PlannerMode.FastMode => "Fast_Mode",
            PlannerMode.DominationActive => "Domination_Active",
            PlannerMode.StalkerHidden => "Stalker_Hidden",
            PlannerMode.Insight => "Insight",
            PlannerMode.Exhausted => "Exhausted",
            PlannerMode.PerfectionLevel1 => "Perfection_Level_1",
            PlannerMode.PerfectionLevel2 => "Perfection_Level_2",
            PlannerMode.PerfectionLevel3 => "Perfection_Level_3",
            PlannerMode.PerfectionOfBody => "Form_of_the_Body",
            PlannerMode.PerfectionOfBody1 => "Perfection_of_Body_Level_1",
            PlannerMode.PerfectionOfBody2 => "Perfection_of_Body_Level_2",
            PlannerMode.PerfectionOfBody3 => "Perfection_of_Body_Level_3",
            PlannerMode.PerfectionOfMind => "Form_of_the_Mind",
            PlannerMode.PerfectionOfMind1 => "Perfection_of_Mind_Level_1",
            PlannerMode.PerfectionOfMind2 => "Perfection_of_Mind_Level_2",
            PlannerMode.PerfectionOfMind3 => "Perfection_of_Mind_Level_3",
            PlannerMode.PerfectionOfSoul => "Form_of_the_Soul",
            PlannerMode.PerfectionOfSoul1 => "Perfection_of_Soul_Level_1",
            PlannerMode.PerfectionOfSoul2 => "Perfection_of_Soul_Level_2",
            PlannerMode.PerfectionOfSoul3 => "Perfection_of_Soul_Level_3",
            PlannerMode.PackMentality => "Pack_Mentality",
            _ => ToCanonicalName(mode)
        };
    }

    public static string ToDisplayName(PlannerMode mode)
    {
        return mode switch
        {
            PlannerMode.FastSnipe => "Fast Snipe",
            PlannerMode.CriticalHit => "Critical Hit",
            PlannerMode.DefensiveAdaptation => "Defensive Adaptation",
            PlannerMode.EfficientAdaptation => "Efficient Adaptation",
            PlannerMode.OffensiveAdaptation => "Offensive Adaptation",
            PlannerMode.ComboLevel1 => "Combo 1",
            PlannerMode.ComboLevel2 => "Combo 2",
            PlannerMode.ComboLevel3 => "Combo 3",
            PlannerMode.FastMode => "Fast Mode",
            PlannerMode.DominationActive => "Domination",
            PlannerMode.StalkerHidden => "From Hide",
            PlannerMode.Insight => "Insight",
            PlannerMode.Exhausted => "Exhausted",
            PlannerMode.PerfectionLevel1 => "Perfection 1",
            PlannerMode.PerfectionLevel2 => "Perfection 2",
            PlannerMode.PerfectionLevel3 => "Perfection 3",
            PlannerMode.PerfectionOfBody => "Form of the Body",
            PlannerMode.PerfectionOfBody1 => "Body Perfection 1",
            PlannerMode.PerfectionOfBody2 => "Body Perfection 2",
            PlannerMode.PerfectionOfBody3 => "Body Perfection 3",
            PlannerMode.PerfectionOfMind => "Form of the Mind",
            PlannerMode.PerfectionOfMind1 => "Mind Perfection 1",
            PlannerMode.PerfectionOfMind2 => "Mind Perfection 2",
            PlannerMode.PerfectionOfMind3 => "Mind Perfection 3",
            PlannerMode.PerfectionOfSoul => "Form of the Soul",
            PlannerMode.PerfectionOfSoul1 => "Soul Perfection 1",
            PlannerMode.PerfectionOfSoul2 => "Soul Perfection 2",
            PlannerMode.PerfectionOfSoul3 => "Soul Perfection 3",
            PlannerMode.PackMentality => "Pack Mentality",
            _ => ToCanonicalName(mode)
        };
    }
}

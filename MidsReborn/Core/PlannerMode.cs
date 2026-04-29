namespace Mids_Reborn.Core;

public enum PlannerMode
{
    None,
    FastSnipe,
    Containment,
    Domination,
    Scourge,
    CriticalHit,
    Assassination,
    Defiance,
    DefensiveAdaptation,
    EfficientAdaptation,
    OffensiveAdaptation,
    ComboLevel1,
    ComboLevel2,
    ComboLevel3,
    FastMode,
    PerfectionOfBody,
    PerfectionOfMind,
    PerfectionOfSoul,
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
        ["Scourge"] = PlannerMode.Scourge,
        ["CriticalHit"] = PlannerMode.CriticalHit,
        ["Critical_Hit"] = PlannerMode.CriticalHit,
        ["Assassination"] = PlannerMode.Assassination,
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
        ["PerfectionOfBody"] = PlannerMode.PerfectionOfBody,
        ["FormOfTheBody"] = PlannerMode.PerfectionOfBody,
        ["Form_of_the_Body"] = PlannerMode.PerfectionOfBody,
        ["PerfectionOfMind"] = PlannerMode.PerfectionOfMind,
        ["FormOfTheMind"] = PlannerMode.PerfectionOfMind,
        ["Form_of_the_Mind"] = PlannerMode.PerfectionOfMind,
        ["PerfectionOfSoul"] = PlannerMode.PerfectionOfSoul,
        ["FormOfTheSoul"] = PlannerMode.PerfectionOfSoul,
        ["Form_of_the_Soul"] = PlannerMode.PerfectionOfSoul,
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
            PlannerMode.Scourge => "Scourge",
            PlannerMode.CriticalHit => "CriticalHit",
            PlannerMode.Assassination => "Assassination",
            PlannerMode.Defiance => "Defiance",
            PlannerMode.DefensiveAdaptation => "DefensiveAdaptation",
            PlannerMode.EfficientAdaptation => "EfficientAdaptation",
            PlannerMode.OffensiveAdaptation => "OffensiveAdaptation",
            PlannerMode.ComboLevel1 => "ComboLevel1",
            PlannerMode.ComboLevel2 => "ComboLevel2",
            PlannerMode.ComboLevel3 => "ComboLevel3",
            PlannerMode.FastMode => "FastMode",
            PlannerMode.PerfectionOfBody => "PerfectionOfBody",
            PlannerMode.PerfectionOfMind => "PerfectionOfMind",
            PlannerMode.PerfectionOfSoul => "PerfectionOfSoul",
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
            PlannerMode.PerfectionOfBody => "Form_of_the_Body",
            PlannerMode.PerfectionOfMind => "Form_of_the_Mind",
            PlannerMode.PerfectionOfSoul => "Form_of_the_Soul",
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
            PlannerMode.ComboLevel1 => "Combo Level 1",
            PlannerMode.ComboLevel2 => "Combo Level 2",
            PlannerMode.ComboLevel3 => "Combo Level 3",
            PlannerMode.FastMode => "Fast Mode",
            PlannerMode.PerfectionOfBody => "Form of the Body",
            PlannerMode.PerfectionOfMind => "Form of the Mind",
            PlannerMode.PerfectionOfSoul => "Form of the Soul",
            PlannerMode.PackMentality => "Pack Mentality",
            _ => ToCanonicalName(mode)
        };
    }
}

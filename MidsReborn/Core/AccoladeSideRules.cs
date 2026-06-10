using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

internal static class AccoladeSideRules
{
    private enum AlignmentSide
    {
        Unknown,
        Hero,
        Villain
    }

    private readonly record struct Rule(string FullName, AlignmentSide Side);

    private static readonly IReadOnlyList<Rule> Rules =
    [
        new("Temporary_Powers.Accolades.The_Atlas_Medallion", AlignmentSide.Hero),
        new("Temporary_Powers.Accolades.Portal_Jockey", AlignmentSide.Hero),
        new("Temporary_Powers.Accolades.Task_Force_Commander", AlignmentSide.Hero),
        new("Temporary_Powers.Accolades.Freedom_Phalanx_Reserve", AlignmentSide.Hero),
        new("Temporary_Powers.Accolades.Super_Patriot", AlignmentSide.Hero),
        new("Temporary_Powers.Accolades.Eye_of_the_Magus", AlignmentSide.Hero),
        new("Temporary_Powers.Accolades.Vanguard_Medal", AlignmentSide.Hero),
        new("Temporary_Powers.Accolades.Geas_of_the_Kind_Ones", AlignmentSide.Hero),
        new("Temporary_Powers.Accolades.Marshall", AlignmentSide.Villain),
        new("Temporary_Powers.Accolades.Born_In_Battle", AlignmentSide.Villain),
        new("Temporary_Powers.Accolades.Invader", AlignmentSide.Villain),
        new("Temporary_Powers.Accolades.High_Pain_Threshold", AlignmentSide.Villain),
        new("Temporary_Powers.Accolades.Iron_Man", AlignmentSide.Villain),
        new("Temporary_Powers.Accolades.Demonic_Aura", AlignmentSide.Villain),
        new("Temporary_Powers.Accolades.Megalomaniac", AlignmentSide.Villain),
        new("Temporary_Powers.Accolades.Force_of_Nature", AlignmentSide.Villain)
    ];

    private static readonly IReadOnlyDictionary<string, Rule> RulesByFullName = Rules
        .ToDictionary(rule => rule.FullName, StringComparer.OrdinalIgnoreCase);

    public static bool IsHeroSideAlignment(Enums.Alignment alignment)
        => GetAlignmentSide(alignment) == AlignmentSide.Hero;

    public static bool IsVillainSideAlignment(Enums.Alignment alignment)
        => GetAlignmentSide(alignment) == AlignmentSide.Villain;

    public static bool IsSameAlignmentSide(Enums.Alignment left, Enums.Alignment right)
    {
        var leftSide = GetAlignmentSide(left);
        return leftSide != AlignmentSide.Unknown && leftSide == GetAlignmentSide(right);
    }

    public static IReadOnlyList<IPower> FilterAccoladesForAlignment(IEnumerable<IPower> powers, Enums.Alignment alignment)
    {
        var side = GetAlignmentSide(alignment);
        return side == AlignmentSide.Unknown
            ? powers.ToList()
            : powers.Where(power => IsVisibleForSide(power, side)).ToList();
    }

    private static bool IsVisibleForSide(IPower? power, AlignmentSide side)
    {
        if (power == null || side == AlignmentSide.Unknown)
        {
            return power != null;
        }

        return !RulesByFullName.TryGetValue(power.FullName, out var rule) || rule.Side == side;
    }

    private static AlignmentSide GetAlignmentSide(Enums.Alignment alignment)
    {
        return alignment switch
        {
            Enums.Alignment.Hero or Enums.Alignment.Vigilante => AlignmentSide.Hero,
            Enums.Alignment.Villain or Enums.Alignment.Rogue => AlignmentSide.Villain,
            _ => AlignmentSide.Unknown
        };
    }
}

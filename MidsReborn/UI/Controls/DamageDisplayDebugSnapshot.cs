using System.Text.RegularExpressions;

namespace Mids_Reborn.UI.Controls;

public sealed class DamageDisplayDebugSnapshot
{
    public string ControlType { get; init; } = string.Empty;
    public string BasePowerFullName { get; init; } = string.Empty;
    public string EnhancedPowerFullName { get; init; } = string.Empty;
    public int BaseEffectCount { get; init; }
    public int EnhancedEffectCount { get; init; }
    public float BaseDamage { get; init; }
    public float EnhancedDamage { get; init; }
    public float BaseDamageAbsorbTrue { get; init; }
    public float EnhancedDamageAbsorbTrue { get; init; }
    public string BaseDamageString { get; init; } = string.Empty;
    public string EnhancedDamageString { get; init; } = string.Empty;
    public string BaseDamageStringAbsorbTrue { get; init; } = string.Empty;
    public string EnhancedDamageStringAbsorbTrue { get; init; } = string.Empty;
    public string DisplayText { get; init; } = string.Empty;
    public string ToolTipText { get; init; } = string.Empty;
    public string Branch { get; init; } = string.Empty;
    public bool DisplayContainsDuplicateIdenticalTerms { get; init; }
    public bool BaseHasAbsorbedRows { get; init; }
    public bool EnhancedHasAbsorbedRows { get; init; }
    public IReadOnlyList<string> DamageRowIdentities { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> GroupedEffectIdentities { get; init; } = Array.Empty<string>();

    public static bool DetectDuplicateIdenticalDamageTerms(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var leftSide = text.Split('=')[0];
        var terms = leftSide.Split('+')
            .Select(term => Regex.Replace(term, @"\s+", " ").Trim())
            .Where(term => !string.IsNullOrWhiteSpace(term))
            .ToList();

        return terms.Count > 1 &&
               terms.GroupBy(term => term, StringComparer.OrdinalIgnoreCase).Any(group => group.Count() > 1);
    }
}

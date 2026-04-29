namespace Mids_Reborn.Core.PlannerRulesets;

public sealed class PlannerCombatContext
{
    public static PlannerCombatContext Default { get; } = new();

    public int SelectedEnemyRelativeLevel { get; init; }
    public int EffectiveEnemyRelativeLevel { get; init; }
    public int TeamSize { get; init; } = 1;
    public float ToHitScale { get; init; } = 0.75f;
    public float AccuracyScale { get; init; } = 1f;
    public float MagnitudeScale { get; init; } = 1f;
    public float DurationScale { get; init; } = 1f;
    public bool UsesLegacyToHitFallback { get; init; }
    public bool UsesFullCombatModTables { get; init; }
    public string Diagnostic { get; init; } = string.Empty;
}

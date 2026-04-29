namespace Mids_Reborn.Core;

public sealed class PseudoPetRecurrenceInfo
{
    public string EntityName { get; init; } = string.Empty;
    public string PetPowerName { get; init; } = string.Empty;
    public float SourceUsageTime { get; init; }
    public float SourceActivatePeriod { get; init; }
    public float EntCreateDuration { get; init; }
    public float PetTickInterval { get; init; }
    public int SpawnCount { get; init; }
    public int TicksPerSpawn { get; init; }
    public int TotalExpectedTicks { get; init; }

    public bool IsValid => SourceUsageTime > 0 &&
                           SourceActivatePeriod > 0 &&
                           EntCreateDuration > 0 &&
                           PetTickInterval > 0 &&
                           SpawnCount > 0 &&
                           TicksPerSpawn > 0 &&
                           TotalExpectedTicks > 0;

    public string ToDisplayText()
    {
        return $"{TicksPerSpawn} ticks per {EntCreateDuration:0.###}s spawn, {SpawnCount} spawns over {SourceUsageTime:0.###}s";
    }
}

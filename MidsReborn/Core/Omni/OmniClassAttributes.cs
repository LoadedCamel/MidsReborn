using Newtonsoft.Json;

namespace Mids_Reborn.Core.Omni;

public sealed class OmniClassAttributeTable
{
    public string ClassName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PrimaryCategory { get; set; } = string.Empty;
    public string SecondaryCategory { get; set; } = string.Empty;
    public bool Playable { get; set; }

    public Dictionary<string, float> Base { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, float> Min { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, float[]> Max { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, float[]> MaxMax { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, float> StrengthMin { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, float[]> StrengthMax { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, float> ResistanceMin { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, float[]> ResistanceMax { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, float> DiminishingStrength { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, float> DiminishingCurrent { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, float[]> NamedTables { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [JsonIgnore]
    public bool HasAttributes => Base.Count > 0 || Max.Count > 0 || MaxMax.Count > 0 || NamedTables.Count > 0;

    public float? GetBase(string attribute)
    {
        return Base.TryGetValue(attribute, out var value) ? value : null;
    }

    public float? GetMax(string attribute, int level)
    {
        return GetLeveledValue(Max, attribute, level);
    }

    public float? GetMaxMax(string attribute, int level)
    {
        return GetLeveledValue(MaxMax, attribute, level);
    }

    public float? GetNamedTableValue(string tableName, int level)
    {
        return GetLeveledValue(NamedTables, tableName, level);
    }

    public float? GetNamedTableValueZeroBased(string tableName, int zeroBasedLevel)
    {
        if (!NamedTables.TryGetValue(tableName, out var values) || values.Length == 0)
        {
            return null;
        }

        var index = Math.Clamp(zeroBasedLevel, 0, values.Length - 1);
        return values[index];
    }

    private static float? GetLeveledValue(IReadOnlyDictionary<string, float[]> table, string key, int level)
    {
        if (!table.TryGetValue(key, out var values) || values.Length == 0)
        {
            return null;
        }

        var index = Math.Clamp(level, 1, values.Length) - 1;
        return values[index];
    }
}

namespace Mids_Reborn.Core.Omni;

public sealed record OmniExportFingerprint(
    string NormalizedRoot,
    int RelevantFileCount,
    long LatestWriteUtcTicks,
    long TotalBytes,
    string Signature)
{
    public bool Matches(OmniExportFingerprint? other)
    {
        return other != null &&
               string.Equals(NormalizedRoot, other.NormalizedRoot, StringComparison.OrdinalIgnoreCase) &&
               RelevantFileCount == other.RelevantFileCount &&
               LatestWriteUtcTicks == other.LatestWriteUtcTicks &&
               TotalBytes == other.TotalBytes &&
               string.Equals(Signature, other.Signature, StringComparison.Ordinal);
    }
}

public sealed class OmniExportManifest
{
    public string ExportRoot { get; init; } = string.Empty;
    public OmniExportFingerprint Fingerprint { get; init; } =
        new(string.Empty, 0, 0, 0, string.Empty);

    public IReadOnlyList<string> ArchetypeFiles { get; init; } = [];
    public IReadOnlyList<string> ClassTableFiles { get; init; } = [];
    public IReadOnlyList<string> PowerFiles { get; init; } = [];
    public IReadOnlyList<string> PowersetIndexFiles { get; init; } = [];
    public IReadOnlyList<string> EntityFiles { get; init; } = [];
    public IReadOnlyList<string> TagFiles { get; init; } = [];
    public IReadOnlyList<string> EnhancementFiles { get; init; } = [];
    public IReadOnlyList<string> EnhancementSetFiles { get; init; } = [];
    public IReadOnlyList<string> RecipeFiles { get; init; } = [];
    public IReadOnlyList<string> LegacyRecipeFiles { get; init; } = [];
    public IReadOnlyList<string> SalvageFiles { get; init; } = [];
    public IReadOnlyList<string> PolicyFiles { get; init; } = [];
    public IReadOnlyList<string> RelevantFiles { get; init; } = [];

    public bool Matches(OmniExportManifest? other)
    {
        return other != null && Fingerprint.Matches(other.Fingerprint);
    }
}

namespace Mids_Reborn.Core.Omni;

public sealed class OmniImportSession
{
    public string ExportRoot { get; init; } = string.Empty;
    public OmniExportManifest Manifest { get; set; } = new();
    public OmniImportResult AnalysisResult { get; set; } = new();
    public OmniApplyResult? ApplyResult { get; set; }
    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;
    public string CachedMarkdownReport { get; set; } = string.Empty;
    public string CachedJsonReport { get; set; } = string.Empty;
    public string CachedPreviewReport { get; set; } = string.Empty;

    public bool Matches(string exportRoot, OmniExportFingerprint? fingerprint = null)
    {
        if (!string.Equals(ExportRoot, exportRoot, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return fingerprint == null || Manifest.Fingerprint.Matches(fingerprint);
    }

    public void ClearRenderedArtifacts()
    {
        CachedMarkdownReport = string.Empty;
        CachedJsonReport = string.Empty;
        CachedPreviewReport = string.Empty;
    }
}

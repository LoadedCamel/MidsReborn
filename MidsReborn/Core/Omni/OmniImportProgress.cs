namespace Mids_Reborn.Core.Omni;

public sealed record OmniImportProgress(
    int Percent,
    string Stage,
    string Detail = "",
    int Current = 0,
    int Total = 0)
{
    public int ClampedPercent => Math.Clamp(Percent, 0, 100);

    public string DisplayText
    {
        get
        {
            var text = string.IsNullOrWhiteSpace(Detail) ? Stage : $"{Stage}: {Detail}";
            return Total > 0 ? $"{text} ({Current:n0}/{Total:n0})" : text;
        }
    }
}

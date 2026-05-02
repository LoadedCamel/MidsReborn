namespace Mids_Reborn.UI.Controls;

internal enum DamageSourceSegmentKind
{
    Base,
    Enhanced,
    Proc
}

internal sealed record DamageSourceSegment(DamageSourceSegmentKind Kind, string Label, float Value);

internal sealed record DamageCardPresentation(
    string HeaderText,
    string ModeBadgeText,
    string PrimaryText,
    string SubtitleText,
    string TooltipText,
    IReadOnlyList<DamageSourceSegment> Segments)
{
    public static readonly DamageCardPresentation Empty =
        new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, Array.Empty<DamageSourceSegment>());

    public bool HasContent => !string.IsNullOrWhiteSpace(PrimaryText);
}

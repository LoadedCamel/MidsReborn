namespace Mids_Reborn.UI.Controls.Test.EnhSelector;

internal enum EnhSelectorChipStyle
{
    Default,
    RarityCommon,
    RarityUncommon,
    RarityRare,
    RarityUltraRare
}

internal sealed class EnhSelectorChip
{
    public string Text { get; init; } = string.Empty;
    public EnhSelectorChipStyle Style { get; init; }
    public bool Emphasized { get; init; }
}

internal sealed class EnhSelectorInspectorSnapshot
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string AccentLine { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public IReadOnlyList<EnhSelectorChip> Tags { get; init; } = [];
    public IReadOnlyList<EnhSelectorInspectorSection> Sections { get; init; } = [];
}

internal sealed class EnhSelectorInspectorSection
{
    public string Heading { get; init; } = string.Empty;
    public IReadOnlyList<string> Lines { get; init; } = [];
    public IReadOnlySet<int> SuccessLineIndexes { get; init; } = new HashSet<int>();
    public bool IsWarning { get; init; }
}

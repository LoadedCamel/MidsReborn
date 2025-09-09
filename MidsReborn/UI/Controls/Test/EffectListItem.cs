namespace Mids_Reborn.UI.Controls.Test;

/// <summary>
/// A UI-agnostic description of a single line in the Effects tab.
/// </summary>
public sealed class EffectListItem
{
    public string Label { get; }
    public string Value { get; }
    public string? AltValue { get; }
    public string? ToolTip { get; }
    public bool IsSpecial { get; }
    public bool IsConditional { get; }
    public bool IsUnique { get; }
    public bool FromEnhancement { get; }

    public EffectListItem(
        string label,
        string value,
        string? altValue = null,
        string? toolTip = null,
        bool isSpecial = false,
        bool isConditional = false,
        bool isUnique = false,
        bool fromEnhancement = false)
    {
        Label = label;
        Value = value;
        AltValue = altValue;
        ToolTip = toolTip;
        IsSpecial = isSpecial;
        IsConditional = isConditional;
        IsUnique = isUnique;
        FromEnhancement = fromEnhancement;
    }
}
using System;

namespace Mids_Reborn.UI.Controls;

[Flags]
public enum MidsItemFontStyles
{
    Normal = 0,
    Bold = 1,
    Italic = 2,
    Underline = 4,
    Strikethrough = 8
}

public enum MidsItemState
{
    Enabled,
    Selected,
    Disabled,
    SelectedDisabled,
    Invalid,
    Heading
}

public enum MidsItemAlign
{
    Left,
    Center,
    Right
}

public enum WordwrapMode
{
    Legacy,
    New,
    UseEllipsis
}

/// <summary>
/// Represents a single item in the MidsListView.
/// This is a data-only class.
/// </summary>
public class MidsListViewItem
{
    private MidsItemFontStyles _fontStyle;

    public string Text { get; set; }

    public MidsItemState State { get; set; }

    public MidsItemFontStyles FontStyle
    {
        get => _fontStyle;
        set => _fontStyle = value;
    }

    public MidsItemAlign Alignment { get; set; }

    public object? Tag { get; set; } // For associating custom data

    public bool Bold
    {
        get => (_fontStyle & MidsItemFontStyles.Bold) != 0;
        set => _fontStyle = value
            ? _fontStyle | MidsItemFontStyles.Bold
            : _fontStyle & ~MidsItemFontStyles.Bold;
    }

    public bool Italic
    {
        get => (_fontStyle & MidsItemFontStyles.Italic) != 0;
        set => _fontStyle = value
            ? _fontStyle | MidsItemFontStyles.Italic
            : _fontStyle & ~MidsItemFontStyles.Italic;
    }

    public bool Underline
    {
        get => (_fontStyle & MidsItemFontStyles.Underline) != 0;
        set => _fontStyle = value
            ? _fontStyle | MidsItemFontStyles.Underline
            : _fontStyle & ~MidsItemFontStyles.Underline;
    }

    public bool Strikethrough
    {
        get => (_fontStyle & MidsItemFontStyles.Strikethrough) != 0;
        set => _fontStyle = value
            ? _fontStyle | MidsItemFontStyles.Strikethrough
            : _fontStyle & ~MidsItemFontStyles.Strikethrough;
    }

    // Internal properties used by the control for layout caching
    internal string WrappedText { get; set; }
    internal int CalculatedHeight { get; set; }

    public MidsListViewItem(string text, MidsItemState state = MidsItemState.Enabled, MidsItemFontStyles style = MidsItemFontStyles.Normal, MidsItemAlign align = MidsItemAlign.Left)
    {
        Text = text;
        State = state;
        FontStyle = style;
        Alignment = align;
        WrappedText = text; // Default to unwrapped
    }
}
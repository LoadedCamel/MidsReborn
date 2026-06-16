namespace Mids_Reborn.UI.Controls.Test.EnhSelector;

internal readonly record struct EnhSelectorVisualPalette
{
    public Color BackgroundTop { get; init; }
    public Color BackgroundBottom { get; init; }

    public Color OuterBorder { get; init; }
    public Color InnerHighlight { get; init; }

    public Color TitleTop { get; init; }
    public Color TitleBottom { get; init; }

    public Color PanelTop { get; init; }
    public Color PanelBottom { get; init; }
    public Color PanelBorder { get; init; }

    public Color TabTop { get; init; }
    public Color TabBottom { get; init; }
    public Color TabHoverTop { get; init; }
    public Color TabHoverBottom { get; init; }
    public Color TabSelectedTop { get; init; }
    public Color TabSelectedBottom { get; init; }
    public Color TabBorder { get; init; }

    public Color CardTop { get; init; }
    public Color CardBottom { get; init; }
    public Color CardHoverTop { get; init; }
    public Color CardHoverBottom { get; init; }
    public Color CardSelectedTop { get; init; }
    public Color CardSelectedBottom { get; init; }
    public Color CardBorder { get; init; }
    public Color CardInnerBorder { get; init; }
    public Color CardDisabledOverlay { get; init; }

    public Color ButtonTop { get; init; }
    public Color ButtonBottom { get; init; }
    public Color ButtonHoverTop { get; init; }
    public Color ButtonHoverBottom { get; init; }
    public Color ButtonPressedBottom { get; init; }
    public Color ButtonBorder { get; init; }
    public Color ButtonDisabledTop { get; init; }
    public Color ButtonDisabledBottom { get; init; }
    public Color ButtonDisabledBorder { get; init; }

    public Color SelectionGlow { get; init; }
    public Color SelectionBorder { get; init; }

    public Color Text { get; init; }
    public Color MutedText { get; init; }
    public Color HeaderText { get; init; }
    public Color SectionText { get; init; }
    public Color AccentText { get; init; }
    public Color Divider { get; init; }
    public Color WarningText { get; init; }
    public Color SuccessText { get; init; }
    public Color ValueHighlightText { get; init; }

    public static EnhSelectorVisualPalette Default => new()
    {
        BackgroundTop = Color.FromArgb(8, 26, 43),
        BackgroundBottom = Color.FromArgb(2, 7, 13),

        OuterBorder = Color.FromArgb(116, 63, 124, 176),
        InnerHighlight = Color.FromArgb(24, 130, 190, 230),

        TitleTop = Color.FromArgb(22, 70, 105),
        TitleBottom = Color.FromArgb(6, 22, 38),

        PanelTop = Color.FromArgb(12, 43, 68),
        PanelBottom = Color.FromArgb(3, 12, 22),
        PanelBorder = Color.FromArgb(104, 62, 118, 168),

        TabTop = Color.FromArgb(14, 42, 66),
        TabBottom = Color.FromArgb(5, 17, 31),
        TabHoverTop = Color.FromArgb(22, 62, 94),
        TabHoverBottom = Color.FromArgb(9, 28, 48),
        TabSelectedTop = Color.FromArgb(40, 86, 132),
        TabSelectedBottom = Color.FromArgb(24, 58, 96),
        TabBorder = Color.FromArgb(116, 84, 142, 194),

        CardTop = Color.FromArgb(11, 35, 56),
        CardBottom = Color.FromArgb(4, 15, 28),
        CardHoverTop = Color.FromArgb(19, 56, 86),
        CardHoverBottom = Color.FromArgb(8, 26, 43),
        CardSelectedTop = Color.FromArgb(43, 84, 126),
        CardSelectedBottom = Color.FromArgb(25, 53, 88),
        CardBorder = Color.FromArgb(98, 72, 128, 178),
        CardInnerBorder = Color.FromArgb(18, 115, 166, 208),
        CardDisabledOverlay = Color.FromArgb(84, 0, 0, 0),

        ButtonTop = Color.FromArgb(19, 53, 83),
        ButtonBottom = Color.FromArgb(8, 24, 41),
        ButtonHoverTop = Color.FromArgb(28, 72, 108),
        ButtonHoverBottom = Color.FromArgb(12, 32, 54),
        ButtonPressedBottom = Color.FromArgb(3, 13, 26),
        ButtonBorder = Color.FromArgb(122, 82, 146, 200),
        ButtonDisabledTop = Color.FromArgb(15, 28, 39),
        ButtonDisabledBottom = Color.FromArgb(7, 12, 19),
        ButtonDisabledBorder = Color.FromArgb(76, 58, 82, 96),

        SelectionGlow = Color.FromArgb(72, 182, 235),
        SelectionBorder = Color.FromArgb(126, 210, 255),

        Text = Color.FromArgb(236, 246, 255),
        MutedText = Color.FromArgb(145, 166, 182),
        HeaderText = Color.FromArgb(234, 248, 255),
        SectionText = Color.FromArgb(124, 210, 255),
        AccentText = Color.FromArgb(39, 218, 255),
        Divider = Color.FromArgb(112, 76, 148, 200),
        WarningText = Color.FromArgb(255, 194, 102),
        SuccessText = Color.FromArgb(118, 255, 164),
        ValueHighlightText = Color.FromArgb(255, 224, 92)
    };
}

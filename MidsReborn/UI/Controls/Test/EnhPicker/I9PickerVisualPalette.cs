namespace Mids_Reborn.UI.Controls.Test.EnhPicker
{
    public readonly record struct I9PickerVisualPalette
    {
        public Color BackgroundTop { get; init; }
        public Color BackgroundBottom { get; init; }

        public Color OuterBorder { get; init; }
        public Color InnerHighlight { get; init; }

        public Color HeaderTop { get; init; }
        public Color HeaderBottom { get; init; }

        public Color PanelTop { get; init; }
        public Color PanelBottom { get; init; }
        public Color PanelBorder { get; init; }

        public Color RailTop { get; init; }
        public Color RailBottom { get; init; }
        public Color RailBorder { get; init; }

        public Color TileTop { get; init; }
        public Color TileBottom { get; init; }
        public Color TileHoverTop { get; init; }
        public Color TileHoverBottom { get; init; }
        public Color TileBorder { get; init; }
        public Color TileInnerBorder { get; init; }

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

        public static I9PickerVisualPalette Default => new()
        {
            BackgroundTop = Color.FromArgb(8, 26, 43),
            BackgroundBottom = Color.FromArgb(2, 7, 13),

            OuterBorder = Color.FromArgb(132, 58, 124, 178),
            InnerHighlight = Color.FromArgb(72, 130, 190, 230),

            HeaderTop = Color.FromArgb(22, 70, 105),
            HeaderBottom = Color.FromArgb(6, 22, 38),

            PanelTop = Color.FromArgb(12, 43, 68),
            PanelBottom = Color.FromArgb(3, 12, 22),
            PanelBorder = Color.FromArgb(120, 54, 112, 160),

            RailTop = Color.FromArgb(14, 49, 76),
            RailBottom = Color.FromArgb(3, 12, 22),
            RailBorder = Color.FromArgb(135, 62, 132, 188),

            TileTop = Color.FromArgb(16, 52, 80),
            TileBottom = Color.FromArgb(4, 14, 26),
            TileHoverTop = Color.FromArgb(25, 76, 112),
            TileHoverBottom = Color.FromArgb(6, 23, 42),
            TileBorder = Color.FromArgb(112, 58, 120, 170),
            TileInnerBorder = Color.FromArgb(70, 116, 172, 210),

            ButtonTop = Color.FromArgb(24, 70, 105),
            ButtonBottom = Color.FromArgb(6, 22, 40),
            ButtonHoverTop = Color.FromArgb(36, 94, 134),
            ButtonHoverBottom = Color.FromArgb(10, 34, 58),
            ButtonPressedBottom = Color.FromArgb(3, 13, 26),
            ButtonBorder = Color.FromArgb(140, 74, 144, 205),

            ButtonDisabledTop = Color.FromArgb(15, 28, 39),
            ButtonDisabledBottom = Color.FromArgb(7, 12, 19),
            ButtonDisabledBorder = Color.FromArgb(76, 58, 82, 96),

            SelectionGlow = Color.FromArgb(255, 191, 42),
            SelectionBorder = Color.FromArgb(255, 217, 83),

            Text = Color.FromArgb(236, 246, 255),
            MutedText = Color.FromArgb(145, 166, 182),
            HeaderText = Color.FromArgb(234, 248, 255),
            SectionText = Color.FromArgb(124, 210, 255),
            AccentText = Color.FromArgb(39, 218, 255),

            Divider = Color.FromArgb(112, 76, 148, 200)
        };
    }
}

namespace Mids_Reborn.UI.Controls.Test.EnhPicker
{
    public sealed class I9PickerLayout
    {
        public Rectangle OuterBounds { get; private init; }
        public Rectangle HeaderBox { get; private init; }
        public Rectangle InfoBox { get; private init; }
        public Rectangle TypeStripPanel { get; private init; }
        public Rectangle[] TypeButtons { get; private init; } = [];
        public Rectangle GridPanel { get; private init; }
        public Rectangle GridBounds { get; private init; }
        public Rectangle[] GridSlots { get; private init; } = [];
        public Rectangle RailPanel { get; private init; }
        public Rectangle RailIconViewport { get; private init; }
        public Rectangle[] RailSlots { get; private init; } = [];
        public Rectangle RailScrollbarBounds { get; private init; }
        public Rectangle InfoPanel { get; private init; }
        public Rectangle LevelBoostPanel { get; private init; }
        public Rectangle BoostMinusButton { get; private init; }
        public Rectangle BoostPlusButton { get; private init; }
        public int IconSize { get; private init; }
        public int RailSlotGap { get; private init; }
        public int GridColumns { get; private init; }
        public int GridRows { get; private init; }

        public static I9PickerLayout Empty { get; } = new()
        {
            OuterBounds = Rectangle.Empty,
            HeaderBox = Rectangle.Empty,
            InfoBox = Rectangle.Empty,
            TypeStripPanel = Rectangle.Empty,
            TypeButtons = [],
            GridPanel = Rectangle.Empty,
            GridBounds = Rectangle.Empty,
            GridSlots = [],
            RailPanel = Rectangle.Empty,
            RailIconViewport = Rectangle.Empty,
            RailSlots = [],
            RailScrollbarBounds = Rectangle.Empty,
            InfoPanel = Rectangle.Empty,
            LevelBoostPanel = Rectangle.Empty,
            BoostMinusButton = Rectangle.Empty,
            BoostPlusButton = Rectangle.Empty,
            IconSize = 64,
            RailSlotGap = 8,
            GridColumns = 4,
            GridRows = 5
        };

        public static Size CalculatePreferredSize(I9PickerLayoutOptions options)
        {
            var iconSize = Math.Max(32, options.IconSize);
            var gap = Math.Max(0, options.Gap);
            var margin = Math.Max(0, options.Margin);
            var gridColumns = Math.Max(1, options.GridColumns);
            var gridRows = Math.Max(1, options.GridRows);
            var typeButtonCount = Math.Max(1, options.TypeButtonCount);
            var maxVisibleRailItems = Math.Max(1, options.MaxVisibleRailItems);
            var railGap = Math.Max(0, options.RailSlotGap);

            var gridHeight = gridRows * iconSize + (gridRows - 1) * gap;
            var gradeColumnHeight = maxVisibleRailItems * iconSize + (maxVisibleRailItems - 1) * railGap;
            var tallestContentHeight = Math.Max(gridHeight, gradeColumnHeight);

            var railLeft = margin + (typeButtonCount - 1) * (iconSize + gap);
            railLeft += options.LastTypeButtonExtraSpacing;

            var width = railLeft + iconSize + options.RailExtraWidth + options.RailScrollbarWidth + margin;
            var height = margin +
                         options.HeaderHeight +
                         gap +
                         options.InfoHeight +
                         gap +
                         iconSize +
                         gap * 2 +
                         tallestContentHeight +
                         gap +
                         options.FooterHeight +
                         margin;

            return new Size(width, height);
        }

        public static I9PickerLayout Calculate(Size clientSize, I9PickerLayoutOptions options)
        {
            if (clientSize.Width <= 0 || clientSize.Height <= 0)
                return Empty;

            var margin = Math.Max(0, options.Margin);
            var gap = Math.Max(0, options.Gap);
            var headerHeight = Math.Max(1, options.HeaderHeight);
            var infoHeight = Math.Max(1, options.InfoHeight);
            var footerHeight = Math.Max(1, options.FooterHeight);
            var iconSize = Math.Max(32, options.IconSize);
            var gridColumns = Math.Max(1, options.GridColumns);
            var gridRows = Math.Max(1, options.GridRows);
            var railGap = Math.Max(0, options.RailSlotGap);
            var railWidth = iconSize + options.RailExtraWidth + options.RailScrollbarWidth;
            var preferredSize = CalculatePreferredSize(options);
            var compactContentWidth = Math.Max(0, preferredSize.Width - margin * 2);

            var outer = new Rectangle(0, 0, Math.Max(0, clientSize.Width - 1), Math.Max(0, clientSize.Height - 1));
            var headerBox = new Rectangle(
                margin,
                margin,
                Math.Max(0, Math.Min(clientSize.Width - margin * 2, compactContentWidth)),
                headerHeight);

            var infoBox = new Rectangle(
                margin,
                headerBox.Bottom + gap,
                headerBox.Width,
                infoHeight);

            var typeButtons = new Rectangle[Math.Max(1, options.TypeButtonCount)];
            var typeTop = infoBox.Bottom + gap;
            var typeLeft = margin;

            for (var i = 0; i < typeButtons.Length; i++)
            {
                var x = typeLeft + i * (iconSize + gap);
                if (i == typeButtons.Length - 1)
                {
                    x = typeLeft + i * (iconSize + gap + options.LastTypeButtonExtraSpacing);
                }

                typeButtons[i] = new Rectangle(x, typeTop, iconSize, iconSize);
            }

            var lastTypeButton = typeButtons[^1];
            var typeStripPanel = Rectangle.FromLTRB(
                Math.Max(0, margin - options.TypeStripInset),
                Math.Max(0, typeTop - options.TypeStripInset),
                Math.Min(clientSize.Width - margin, lastTypeButton.Left + railWidth),
                Math.Min(clientSize.Height - margin, typeTop + iconSize + options.TypeStripInset));

            var gridTop = lastTypeButton.Bottom + gap * 2;
            var gridWidth = gridColumns * iconSize + (gridColumns - 1) * gap;
            var gridHeight = gridRows * iconSize + (gridRows - 1) * gap;
            var gridBounds = new Rectangle(margin, gridTop, gridWidth, gridHeight);
            var gridPanel = Rectangle.Inflate(gridBounds, options.GridPanelInset, options.GridPanelInset);

            var gridSlots = new Rectangle[gridColumns * gridRows];
            for (var row = 0; row < gridRows; row++)
            {
                for (var col = 0; col < gridColumns; col++)
                {
                    var index = row * gridColumns + col;
                    gridSlots[index] = new Rectangle(
                        gridBounds.Left + col * (iconSize + gap),
                        gridBounds.Top + row * (iconSize + gap),
                        iconSize,
                        iconSize);
                }
            }

            var railPanel = new Rectangle(lastTypeButton.Left, gridBounds.Top, railWidth, gridHeight);
            var railScrollbarBounds = new Rectangle(
                railPanel.Right - options.RailScrollbarWidth,
                gridBounds.Top,
                options.RailScrollbarWidth,
                gridHeight);
            var railIconViewport = new Rectangle(
                railPanel.Left,
                railPanel.Top,
                Math.Max(0, railScrollbarBounds.Left - railPanel.Left),
                railPanel.Height);
            var railBaseX = railIconViewport.Left + Math.Max(0, (railIconViewport.Width - iconSize) / 2);
            var railOpticalShift = Math.Max(0, options.RailExtraWidth / 4);
            var railMaxX = Math.Max(railIconViewport.Left, railScrollbarBounds.Left - iconSize - 1);
            var railIconX = Math.Min(railBaseX + railOpticalShift, railMaxX);
            var railSlots = new Rectangle[Math.Max(1, options.MaxVisibleRailItems)];
            for (var i = 0; i < railSlots.Length; i++)
            {
                railSlots[i] = new Rectangle(
                    railIconX,
                    gridBounds.Top + i * (iconSize + railGap),
                    iconSize,
                    iconSize);
            }

            var footerTop = gridBounds.Bottom + gap;
            var levelBoostWidth = Math.Max(railWidth, options.LevelBoostMinWidth);
            var levelBoostPanel = new Rectangle(
                railPanel.Right - levelBoostWidth,
                footerTop,
                levelBoostWidth,
                footerHeight);
            var infoPanelRight = Math.Max(margin, levelBoostPanel.Left - gap);
            var infoPanel = Rectangle.FromLTRB(margin, footerTop, infoPanelRight, footerTop + footerHeight);

            var totalButtonWidth = options.BoostButtonWidth * 2 + options.BoostButtonGap;
            var buttonX = levelBoostPanel.Left + (levelBoostPanel.Width - totalButtonWidth) / 2;
            var buttonY = levelBoostPanel.Bottom - options.BoostButtonBottomInset - options.BoostButtonHeight;

            return new I9PickerLayout
            {
                OuterBounds = outer,
                HeaderBox = headerBox,
                InfoBox = infoBox,
                TypeStripPanel = typeStripPanel,
                TypeButtons = typeButtons,
                GridPanel = gridPanel,
                GridBounds = gridBounds,
                GridSlots = gridSlots,
                RailPanel = railPanel,
                RailIconViewport = railIconViewport,
                RailSlots = railSlots,
                RailScrollbarBounds = railScrollbarBounds,
                InfoPanel = infoPanel,
                LevelBoostPanel = levelBoostPanel,
                BoostMinusButton = new Rectangle(buttonX, buttonY, options.BoostButtonWidth, options.BoostButtonHeight),
                BoostPlusButton = new Rectangle(buttonX + options.BoostButtonWidth + options.BoostButtonGap, buttonY, options.BoostButtonWidth, options.BoostButtonHeight),
                IconSize = iconSize,
                RailSlotGap = railGap,
                GridColumns = gridColumns,
                GridRows = gridRows
            };
        }
    }

    public sealed class I9PickerLayoutOptions
    {
        public int Margin { get; set; } = 10;
        public int Gap { get; set; } = 8;
        public int HeaderHeight { get; set; } = 32;
        public int InfoHeight { get; set; } = 60;
        public int TypeButtonCount { get; set; } = 5;
        public int LastTypeButtonExtraSpacing { get; set; } = 2;
        public int TypeStripInset { get; set; } = 2;
        public int GridColumns { get; set; } = 4;
        public int GridRows { get; set; } = 5;
        public int IconSize { get; set; } = 64;
        public int GridPanelInset { get; set; } = 4;
        public int RailExtraWidth { get; set; } = 8;
        public int MaxVisibleRailItems { get; set; } = 5;
        public int RailSlotGap { get; set; } = 8;
        public int RailScrollbarWidth { get; set; } = 14;
        public int FooterHeight { get; set; } = 80;
        public int LevelBoostMinWidth { get; set; } = 100;
        public int BoostButtonWidth { get; set; } = 28;
        public int BoostButtonHeight { get; set; } = 20;
        public int BoostButtonGap { get; set; } = 6;
        public int BoostButtonBottomInset { get; set; } = 6;

        public I9PickerLayoutOptions Clone()
        {
            return new I9PickerLayoutOptions
            {
                Margin = Margin,
                Gap = Gap,
                HeaderHeight = HeaderHeight,
                InfoHeight = InfoHeight,
                TypeButtonCount = TypeButtonCount,
                LastTypeButtonExtraSpacing = LastTypeButtonExtraSpacing,
                TypeStripInset = TypeStripInset,
                GridColumns = GridColumns,
                GridRows = GridRows,
                IconSize = IconSize,
                GridPanelInset = GridPanelInset,
                RailExtraWidth = RailExtraWidth,
                MaxVisibleRailItems = MaxVisibleRailItems,
                RailSlotGap = RailSlotGap,
                RailScrollbarWidth = RailScrollbarWidth,
                FooterHeight = FooterHeight,
                LevelBoostMinWidth = LevelBoostMinWidth,
                BoostButtonWidth = BoostButtonWidth,
                BoostButtonHeight = BoostButtonHeight,
                BoostButtonGap = BoostButtonGap,
                BoostButtonBottomInset = BoostButtonBottomInset
            };
        }
    }
}

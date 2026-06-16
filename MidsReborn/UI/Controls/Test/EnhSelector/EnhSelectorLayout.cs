namespace Mids_Reborn.UI.Controls.Test.EnhSelector;

internal sealed class EnhSelectorLayout
{
    public Rectangle OuterBounds { get; private init; }
    public Rectangle TitleBar { get; private init; }
    public Rectangle CloseButton { get; private init; }
    public Rectangle TabStrip { get; private init; }
    public Rectangle[] TabButtons { get; private init; } = [];
    public Rectangle ContentPanel { get; private init; }
    public Rectangle MainPanel { get; private init; }
    public Rectangle MainHeader { get; private init; }
    public Rectangle MainSubHeader { get; private init; }
    public Rectangle BackButton { get; private init; }
    public Rectangle MainViewport { get; private init; }
    public Rectangle MainScrollbarBounds { get; private init; }
    public Rectangle RailPanel { get; private init; }
    public Rectangle RailViewport { get; private init; }
    public Rectangle RailScrollbarBounds { get; private init; }
    public Rectangle InspectorPanel { get; private init; }
    public Rectangle InspectorHeader { get; private init; }
    public Rectangle InspectorViewport { get; private init; }
    public Rectangle InspectorScrollbarBounds { get; private init; }
    public Rectangle FooterPanel { get; private init; }
    public Rectangle FooterSummaryPanel { get; private init; }
    public Rectangle FooterLevelPanel { get; private init; }
    public Rectangle FooterHintBounds { get; private init; }
    public Rectangle FooterMinusButton { get; private init; }
    public Rectangle FooterPlusButton { get; private init; }
    public int IconSize { get; private init; }
    public int RailWidth { get; private init; }
    public int ScrollbarWidth { get; private init; }

    public static EnhSelectorLayout Empty { get; } = new()
    {
        TabButtons = [],
        OuterBounds = Rectangle.Empty,
        TitleBar = Rectangle.Empty,
        CloseButton = Rectangle.Empty,
        TabStrip = Rectangle.Empty,
        ContentPanel = Rectangle.Empty,
        MainPanel = Rectangle.Empty,
        MainHeader = Rectangle.Empty,
        MainSubHeader = Rectangle.Empty,
        BackButton = Rectangle.Empty,
        MainViewport = Rectangle.Empty,
        MainScrollbarBounds = Rectangle.Empty,
        RailPanel = Rectangle.Empty,
        RailViewport = Rectangle.Empty,
        RailScrollbarBounds = Rectangle.Empty,
        InspectorPanel = Rectangle.Empty,
        InspectorHeader = Rectangle.Empty,
        InspectorViewport = Rectangle.Empty,
        InspectorScrollbarBounds = Rectangle.Empty,
        FooterPanel = Rectangle.Empty,
        FooterSummaryPanel = Rectangle.Empty,
        FooterLevelPanel = Rectangle.Empty,
        FooterHintBounds = Rectangle.Empty,
        FooterMinusButton = Rectangle.Empty,
        FooterPlusButton = Rectangle.Empty,
        IconSize = 48,
        RailWidth = 132,
        ScrollbarWidth = 10
    };

    public static Size CalculatePreferredSize(EnhSelectorLayoutOptions options)
    {
        var width = options.Margin * 2 + options.BaseMainWidth + options.Gap + options.RailWidth + options.Gap + options.InspectorWidth;
        var height = options.Margin * 2 + options.TitleHeight + options.Gap + options.TabHeight + options.Gap + options.ContentHeight + options.Gap + options.FooterHeight;
        return new Size(width, height);
    }

    public static EnhSelectorLayout Calculate(Size clientSize, EnhSelectorLayoutOptions options)
    {
        if (clientSize.Width <= 0 || clientSize.Height <= 0)
            return Empty;

        var outer = new Rectangle(0, 0, Math.Max(0, clientSize.Width - 1), Math.Max(0, clientSize.Height - 1));
        var margin = Math.Max(0, options.Margin);
        var gap = Math.Max(0, options.Gap);
        var contentWidth = Math.Max(0, clientSize.Width - margin * 2);
        var contentHeight = Math.Max(0, clientSize.Height - margin * 2);

        var titleBar = new Rectangle(margin, margin, contentWidth, options.TitleHeight);
        var closeButton = new Rectangle(titleBar.Right - options.CloseButtonSize - 10, titleBar.Top + (titleBar.Height - options.CloseButtonSize) / 2, options.CloseButtonSize, options.CloseButtonSize);

        var tabStrip = new Rectangle(margin, titleBar.Bottom + gap, contentWidth, options.TabHeight);
        var tabButtons = new Rectangle[Math.Max(1, options.TabButtonCount)];
        var tabInsetX = Math.Max(4, options.TabInnerPadding);
        var tabInsetY = Math.Max(3, options.TabInnerPadding - 1);
        var tabButtonsHost = Rectangle.FromLTRB(
            tabStrip.Left + tabInsetX,
            tabStrip.Top + tabInsetY,
            tabStrip.Right - tabInsetX,
            tabStrip.Bottom - tabInsetY);
        var tabGap = gap;
        var tabWidth = Math.Max(80, (tabButtonsHost.Width - tabGap * (tabButtons.Length - 1)) / tabButtons.Length);
        var tabX = tabButtonsHost.Left;
        for (var i = 0; i < tabButtons.Length; i++)
        {
            var width = i == tabButtons.Length - 1 ? tabButtonsHost.Right - tabX : tabWidth;
            tabButtons[i] = new Rectangle(tabX, tabButtonsHost.Top, width, tabButtonsHost.Height);
            tabX += width + tabGap;
        }

        var footerTop = clientSize.Height - margin - options.FooterHeight;
        var footerPanel = new Rectangle(margin, footerTop, contentWidth, options.FooterHeight);
        var footerHintHeight = Math.Clamp(options.FooterHintHeight, 14, Math.Max(14, options.FooterHeight / 4));
        var footerGap = Math.Max(2, gap / 3);
        var footerCardsHeight = Math.Max(58, options.FooterHeight - footerHintHeight - footerGap);
        var footerCardsPanel = new Rectangle(footerPanel.Left, footerPanel.Top, footerPanel.Width, footerCardsHeight);
        var footerHintBounds = new Rectangle(
            footerPanel.Left + Math.Max(2, options.InnerPadding / 2),
            footerCardsPanel.Bottom + footerGap,
            Math.Max(0, footerPanel.Width - Math.Max(4, options.InnerPadding)),
            footerHintHeight);

        var contentTop = tabStrip.Bottom + gap;
        var middleHeight = Math.Max(0, footerPanel.Top - gap - contentTop);
        var contentPanel = new Rectangle(margin, contentTop, contentWidth, middleHeight);

        var railWidth = Math.Max(120, Math.Min(options.RailWidth, Math.Max(120, contentWidth / 6)));
        var inspectorWidth = Math.Max(240, Math.Min(options.InspectorWidth, Math.Max(240, (int)Math.Round(contentWidth * 0.285f))));
        var mainWidth = Math.Max(392, contentWidth - inspectorWidth - railWidth - gap * 2);

        var mainPanel = new Rectangle(contentPanel.Left, contentPanel.Top, mainWidth, middleHeight);
        var railPanel = new Rectangle(mainPanel.Right + gap, contentPanel.Top, railWidth, middleHeight);
        var inspectorPanel = new Rectangle(railPanel.Right + gap, contentPanel.Top, contentPanel.Right - (railPanel.Right + gap), middleHeight);

        var mainHeader = new Rectangle(mainPanel.Left + options.InnerPadding, mainPanel.Top + options.InnerPadding, Math.Max(0, mainPanel.Width - options.InnerPadding * 2), options.MainHeaderHeight);
        var mainSubHeader = new Rectangle(mainHeader.Left, mainHeader.Bottom + gap / 2, mainHeader.Width, options.MainSubHeaderHeight);
        var backButton = new Rectangle(mainHeader.Right - options.BackButtonWidth, mainHeader.Top + (mainHeader.Height - options.BackButtonHeight) / 2, options.BackButtonWidth, options.BackButtonHeight);
        var mainViewportTop = mainSubHeader.Bottom + gap / 2;
        var mainViewport = Rectangle.FromLTRB(mainPanel.Left + options.InnerPadding, mainViewportTop, mainPanel.Right - options.ScrollbarWidth - options.InnerPadding - 2, mainPanel.Bottom - options.InnerPadding);
        var mainScrollbarBounds = Rectangle.FromLTRB(mainViewport.Right + 4, mainViewport.Top, mainPanel.Right - options.InnerPadding, mainViewport.Bottom);

        var railViewport = Rectangle.FromLTRB(railPanel.Left + options.InnerPadding, railPanel.Top + options.InnerPadding, railPanel.Right - options.ScrollbarWidth - options.InnerPadding - 2, railPanel.Bottom - options.InnerPadding);
        var railScrollbarBounds = Rectangle.FromLTRB(railViewport.Right + 4, railViewport.Top, railPanel.Right - options.InnerPadding, railViewport.Bottom);

        var inspectorHeader = new Rectangle(inspectorPanel.Left + options.InnerPadding, inspectorPanel.Top + options.InnerPadding, Math.Max(0, inspectorPanel.Width - options.InnerPadding * 2), options.InspectorHeaderHeight);
        var inspectorViewport = Rectangle.FromLTRB(inspectorPanel.Left + options.InnerPadding, inspectorHeader.Bottom + gap / 2, inspectorPanel.Right - options.ScrollbarWidth - options.InnerPadding - 2, inspectorPanel.Bottom - options.InnerPadding);
        var inspectorScrollbarBounds = Rectangle.FromLTRB(inspectorViewport.Right + 4, inspectorViewport.Top, inspectorPanel.Right - options.InnerPadding, inspectorViewport.Bottom);

        var footerSummaryPanel = Rectangle.FromLTRB(
            footerCardsPanel.Left,
            footerCardsPanel.Top,
            railPanel.Right,
            footerCardsPanel.Bottom);
        var footerLevelPanel = Rectangle.FromLTRB(
            inspectorPanel.Left,
            footerCardsPanel.Top,
            footerCardsPanel.Right,
            footerCardsPanel.Bottom);

        var buttonInsetX = Math.Max(10, options.InnerPadding);
        var buttonInsetBottom = Math.Max(5, options.InnerPadding / 2);
        var buttonTop = footerLevelPanel.Bottom - options.ButtonHeight - buttonInsetBottom;
        var minusButton = new Rectangle(
            footerLevelPanel.Left + buttonInsetX,
            buttonTop,
            options.ButtonWidth,
            options.ButtonHeight);
        var plusButton = new Rectangle(
            footerLevelPanel.Right - options.ButtonWidth - buttonInsetX,
            buttonTop,
            options.ButtonWidth,
            options.ButtonHeight);

        return new EnhSelectorLayout
        {
            OuterBounds = outer,
            TitleBar = titleBar,
            CloseButton = closeButton,
            TabStrip = tabStrip,
            TabButtons = tabButtons,
            ContentPanel = contentPanel,
            MainPanel = mainPanel,
            MainHeader = mainHeader,
            MainSubHeader = mainSubHeader,
            BackButton = backButton,
            MainViewport = mainViewport,
            MainScrollbarBounds = mainScrollbarBounds,
            RailPanel = railPanel,
            RailViewport = railViewport,
            RailScrollbarBounds = railScrollbarBounds,
            InspectorPanel = inspectorPanel,
            InspectorHeader = inspectorHeader,
            InspectorViewport = inspectorViewport,
            InspectorScrollbarBounds = inspectorScrollbarBounds,
            FooterPanel = footerPanel,
            FooterSummaryPanel = footerSummaryPanel,
            FooterLevelPanel = footerLevelPanel,
            FooterHintBounds = footerHintBounds,
            FooterMinusButton = minusButton,
            FooterPlusButton = plusButton,
            IconSize = options.IconSize,
            RailWidth = railWidth,
            ScrollbarWidth = options.ScrollbarWidth
        };
    }
}

internal sealed class EnhSelectorLayoutOptions
{
    public int Margin { get; set; } = 6;
    public int Gap { get; set; } = 6;
    public int TitleHeight { get; set; } = 32;
    public int CloseButtonSize { get; set; } = 24;
    public int TabHeight { get; set; } = 64;
    public int TabButtonCount { get; set; } = 5;
    public int ContentHeight { get; set; } = 500;
    public int FooterHeight { get; set; } = 92;
    public int FooterHintHeight { get; set; } = 20;
    public int BaseMainWidth { get; set; } = 500;
    public int RailWidth { get; set; } = 132;
    public int InspectorWidth { get; set; } = 272;
    public int InnerPadding { get; set; } = 10;
    public int MainHeaderHeight { get; set; } = 32;
    public int MainSubHeaderHeight { get; set; } = 20;
    public int InspectorHeaderHeight { get; set; } = 26;
    public int BackButtonWidth { get; set; } = 74;
    public int BackButtonHeight { get; set; } = 26;
    public int LevelPanelWidth { get; set; } = 168;
    public int ButtonWidth { get; set; } = 28;
    public int ButtonHeight { get; set; } = 22;
    public int IconSize { get; set; } = 48;
    public int ScrollbarWidth { get; set; } = 10;
    public int TabInnerPadding { get; set; } = 4;

    public EnhSelectorLayoutOptions Clone()
    {
        return new EnhSelectorLayoutOptions
        {
            Margin = Margin,
            Gap = Gap,
            TitleHeight = TitleHeight,
            CloseButtonSize = CloseButtonSize,
            TabHeight = TabHeight,
            TabButtonCount = TabButtonCount,
            ContentHeight = ContentHeight,
            FooterHeight = FooterHeight,
            FooterHintHeight = FooterHintHeight,
            BaseMainWidth = BaseMainWidth,
            RailWidth = RailWidth,
            InspectorWidth = InspectorWidth,
            InnerPadding = InnerPadding,
            MainHeaderHeight = MainHeaderHeight,
            MainSubHeaderHeight = MainSubHeaderHeight,
            InspectorHeaderHeight = InspectorHeaderHeight,
            BackButtonWidth = BackButtonWidth,
            BackButtonHeight = BackButtonHeight,
            LevelPanelWidth = LevelPanelWidth,
            ButtonWidth = ButtonWidth,
            ButtonHeight = ButtonHeight,
            IconSize = IconSize,
            ScrollbarWidth = ScrollbarWidth,
            TabInnerPadding = TabInnerPadding
        };
    }
}

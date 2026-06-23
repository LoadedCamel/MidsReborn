using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

public sealed class PowerStatsGrid : Control
{
    #region Types

    public sealed class Row
    {
        public string Label { get; }
        public double BaseValue { get; }
        public double EnhancedValue { get; }
        public string Unit { get; }
        public bool HigherIsBetter { get; }
        public string? Tooltip { get; }               // Optional; can include ED details
        public bool AffectedByEd { get; }             // If true, show ED marker

        // Display options
        public bool HideGainPercent { get; }
        public string? GainUnitOverride { get; }
        public bool NeutralWhenZero { get; }
        public int Band { get; }

        public Row(string? label, double baseValue, double enhancedValue, string? unit, bool higherIsBetter, string? tooltip = null, bool affectedByEd = false, bool hideGainPercent = false, string? gainUnitOverride = null, bool neutralWhenZero = true, int band = -1)
        {
            Label = label ?? string.Empty;
            BaseValue = baseValue;
            EnhancedValue = enhancedValue;
            Unit = unit ?? string.Empty;
            HigherIsBetter = higherIsBetter;
            Tooltip = tooltip;
            AffectedByEd = affectedByEd;
            HideGainPercent = hideGainPercent;
            GainUnitOverride = gainUnitOverride;
            NeutralWhenZero = neutralWhenZero;
            Band = band;
        }
    }

    #endregion

    #region Constants

    private const double Eps = 1e-9;

    private const TextFormatFlags CellFlags =
        TextFormatFlags.VerticalCenter |
        TextFormatFlags.EndEllipsis |
        TextFormatFlags.SingleLine |
        TextFormatFlags.NoPadding;

    private const TextFormatFlags HeaderFlags =
        TextFormatFlags.VerticalCenter |
        TextFormatFlags.SingleLine |
        TextFormatFlags.EndEllipsis;

    private const int LogicalScrollBarWidth = 16;
    private const int LogicalArrowHeight = 16;
    private const int LogicalThumbMinHeight = 20;
    private const int LogicalWheelStepPx = 32;
    private const int LogicalArrowStepPx = 30;
    private const int LogicalPageStepMarginPx = 8;
    private const int MaximumResponsiveColumns = 3;
    private const int MinimumResponsiveHeaderHeight = 10;
    private const int MinimumResponsivePadding = 2;
    private const int MinimumResponsiveRowHeight = 16;
    private const float MinimumResponsiveFontSize = 7.5f;
    private const float MaximumResponsiveFontGrowth = 0.75f;
    private const float ResponsiveFontStep = 0.25f;

    #endregion

    #region Layout types

    private readonly record struct GridLayout(
        int ColumnCount,
        int VisualRows,
        int Padding,
        int HeaderHeight,
        int RowHeight,
        int PairGap,
        int ColumnGap,
        int ItemInset,
        int ContentHeight,
        float FontSize);

    private readonly record struct CellLayout(Rectangle Pair, Rectangle Label, Rectangle Value);

    #endregion

    #region Private fields

    private readonly List<Row> _rows = new();
    private readonly MidsToolTip _tooltip = new()
    {
        ShowAlways = true,
        AutomaticDelay = 200,
        ReshowDelay = 50,
        InitialDelay = 250,
        BackColorTop = Color.FromArgb(20, 54, 87),
        BackColorBottom = Color.FromArgb(28, 80, 128),
        BorderColor = Color.FromArgb(58, 65, 75),
        TitleColor = Color.FromArgb(241, 244, 249),
        TextColor = Color.FromArgb(227, 232, 238),
    };

    // Logical (96-dpi) units; we scale to device pixels at paint time
    private int _headerHeight = 28;
    private int _rowHeight = 28;
    private int _gridPadding = 8;
    private int _colGap = 12;
    private int _hoverItem = -1;

    private int _contentHeight;
    private int _scrollOffset;
    private bool _scrollbarVisible;
    private Rectangle _scrollbarBounds;
    private Rectangle _trackBounds;
    private Rectangle _upArrowRect;
    private Rectangle _downArrowRect;
    private Rectangle _thumbRect;
    private GridLayout _resolvedLayout;
    private bool _draggingThumb;
    private int _dragStartY;
    private bool _hoveringThumb;
    private bool _hoveringUpArrow;
    private bool _hoveringDownArrow;

    // Column width ratio within a single Stat|Value pair.
    private float _wLabel = 0.50f;
    private float _wValue = 0.50f;

    #endregion

    #region Public Properties

    [Browsable(false)]
    public IReadOnlyList<Row> Rows => _rows;

    [DefaultValue(28)]
    public int HeaderHeight
    {
        get => _headerHeight;
        set { _headerHeight = Math.Max(10, value); InvalidateAndRelayout(); }
    }

    [DefaultValue(28)]
    public int RowHeight
    {
        get => _rowHeight;
        set { _rowHeight = Math.Max(18, value); InvalidateAndRelayout(); }
    }

    [DefaultValue(8)]
    public int GridPadding
    {
        get => _gridPadding;
        set { _gridPadding = Math.Max(0, value); InvalidateAndRelayout(); }
    }

    [DefaultValue(12)]
    public int ColumnGap
    {
        get => _colGap;
        set { _colGap = Math.Max(0, value); InvalidateAndRelayout(); }
    }

    #endregion

    #region Private Properties

    private DataViewTheme CurrentTheme
    {
        get
        {
            if (DesignMode)
            {
                return ThemeManager.DesignTime.DataView;
            }
            return ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;
        }
    }

    private ScrollPanelTheme CurrentScrollTheme =>
        DesignMode ? ThemeManager.DesignTime.ScrollPanel : ThemeManager.CurrentTheme?.ScrollPanel ?? ThemeManager.DesignTime.ScrollPanel;

    private double DpiScale => DeviceDpi / 96.0;
    private int ScrollBarWidth => ScalePx(LogicalScrollBarWidth);
    private int ArrowHeight => ScalePx(LogicalArrowHeight);
    private int ThumbMinHeight => ScalePx(LogicalThumbMinHeight);
    private int WheelStepPx => ScalePx(LogicalWheelStepPx);
    private int ArrowStepPx => ScalePx(LogicalArrowStepPx);
    private int PageStepMarginPx => ScalePx(LogicalPageStepMarginPx);
    private int ContentViewportWidth => Math.Max(0, ClientSize.Width - (_scrollbarVisible ? ScrollBarWidth : 0));

    #endregion

    #region Constructor

    public PowerStatsGrid()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.Selectable |
                 ControlStyles.SupportsTransparentBackColor, true);
        TabStop = true;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point);
        if (!DesignMode) ThemeManager.ThemeChanged += ThemeManagerOnThemeChanged;
    }

    private void ThemeManagerOnThemeChanged()
    {
        _tooltip.BackColorTop = CurrentTheme.HeaderTop;
        _tooltip.BackColorBottom = CurrentTheme.HeaderBottom;
        _tooltip.BorderColor = CurrentTheme.Border;
        _tooltip.TextColor = CurrentTheme.Text;
        _tooltip.TitleColor = CurrentTheme.Text;
        Invalidate();
    }

    #endregion

    #region Dispose

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ThemeManager.ThemeChanged -= ThemeManagerOnThemeChanged;
            _tooltip.Dispose();
        }

        base.Dispose(disposing);
    }

    #endregion

    #region Public API

    public void SetRows(IEnumerable<Row> rows)
    {
        _rows.Clear();
        _rows.AddRange(rows);
        _hoverItem = -1;
        _tooltip.Hide(this);
        AutoSizeHeight();
        RelayoutAndScrollbar();
        Invalidate();
    }

    public void Clear()
    {
        _rows.Clear();
        _hoverItem = -1;
        _tooltip.Hide(this);
        AutoSizeHeight();
        _scrollOffset = 0;
        RelayoutAndScrollbar();
        Invalidate();
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var width = proposedSize.Width > 0 ? proposedSize.Width : Math.Max(ScalePx(220), Width);
        return new Size(width, MeasureNaturalContentHeight(width));
    }

    internal int GetFullyVisiblePreferredHeight(int proposedWidth)
    {
        if (_rows.Count == 0)
        {
            return 0;
        }

        var width = proposedWidth > 0 ? proposedWidth : Math.Max(ScalePx(220), Width);
        var layout = ResolveMinimumVisibleLayout(width);
        return layout.ContentHeight;
    }

    #endregion

    #region Private Methods

    private int ScalePx(int px) => (int)Math.Round(px * DpiScale);

    private void InvalidateAndRelayout()
    {
        AutoSizeHeight();
        RelayoutAndScrollbar();
        Invalidate();
    }

    #endregion

    #region Overrides

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        RelayoutAndScrollbar();
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        AutoSizeHeight(); // recompute to reflect scaled row/header
        RelayoutAndScrollbar();
        Invalidate();
    }

    #endregion

    #region Layout helpers

    private void AutoSizeHeight()
    {
        var preferredHeight = MeasureNaturalContentHeight(Math.Max(ScalePx(220), Width));
        if (Dock != DockStyle.Fill && Height != preferredHeight)
        {
            Height = preferredHeight;
        }
    }

    private void RelayoutAndScrollbar()
    {
        var viewportWidth = Math.Max(1, ClientSize.Width);
        var viewportHeight = Math.Max(0, ClientSize.Height);

        _resolvedLayout = ResolveResponsiveLayout(viewportWidth, viewportHeight, fitToHeight: true);
        _contentHeight = _resolvedLayout.ContentHeight;
        _scrollbarVisible = _contentHeight > ClientSize.Height && ClientSize.Height > 0;

        if (_scrollbarVisible)
        {
            var scrolledViewportWidth = Math.Max(1, viewportWidth - ScrollBarWidth);
            _resolvedLayout = ResolveResponsiveLayout(scrolledViewportWidth, viewportHeight, fitToHeight: true);
            _contentHeight = _resolvedLayout.ContentHeight;
            _scrollbarVisible = _contentHeight > ClientSize.Height && ClientSize.Height > 0;
        }

        ClampScrollOffset();
        ComputeScrollbarBounds();
    }

    private int MeasureNaturalContentHeight(int proposedWidth)
        => ResolveResponsiveLayout(Math.Max(1, proposedWidth), 0, fitToHeight: false).ContentHeight;

    private GridLayout ResolveResponsiveLayout(int viewportWidth, int viewportHeight, bool fitToHeight)
    {
        var baseFontSize = GetBaseFontSize();
        var naturalColumnCount = ResolveNaturalColumnCount(viewportWidth, baseFontSize);
        var natural = BuildLayout(viewportWidth, viewportHeight, naturalColumnCount, baseFontSize, allowVerticalFit: false);

        if (!fitToHeight || viewportHeight <= 0)
        {
            return natural;
        }

        if (natural.ContentHeight <= viewportHeight)
        {
            return ResolveRoomyLayout(viewportWidth, viewportHeight, natural);
        }

        return ResolveCompactContentLayout(viewportWidth, viewportHeight);
    }

    private GridLayout ResolveRoomyLayout(int viewportWidth, int viewportHeight, GridLayout fallback)
    {
        var baseFontSize = GetBaseFontSize();
        var maxFontSize = Math.Min(11.0f, baseFontSize + MaximumResponsiveFontGrowth);
        if (maxFontSize <= baseFontSize + 0.05f)
        {
            return fallback;
        }

        for (float fontSize = maxFontSize; fontSize > baseFontSize + 0.05f; fontSize -= ResponsiveFontStep)
        {
            var candidate = BuildLayout(viewportWidth, viewportHeight, fallback.ColumnCount, fontSize, allowVerticalFit: false);
            if (candidate.ContentHeight > viewportHeight)
            {
                continue;
            }

            using var font = CreateSizedFont(fontSize);
            if (IsWidthLegible(viewportWidth, candidate, font, strict: false))
            {
                return candidate;
            }
        }

        return fallback;
    }

    private GridLayout ResolveCompactContentLayout(int viewportWidth, int viewportHeight)
    {
        var baseFontSize = GetBaseFontSize();
        var minFontSize = Math.Min(baseFontSize, MinimumResponsiveFontSize);
        GridLayout? bestFit = null;
        GridLayout? bestFallback = null;
        var hasHeightLimit = viewportHeight > 0;

        foreach (var columnCount in GetCompactColumnCandidates(viewportWidth, baseFontSize))
        {
            for (float fontSize = baseFontSize; fontSize >= minFontSize - 0.05f; fontSize -= ResponsiveFontStep)
            {
                var candidate = BuildLayout(viewportWidth, viewportHeight, columnCount, fontSize, allowVerticalFit: hasHeightLimit);
                using var font = CreateSizedFont(fontSize);
                if (!IsWidthLegible(viewportWidth, candidate, font, strict: false))
                {
                    continue;
                }

                var fits = !hasHeightLimit || candidate.ContentHeight <= viewportHeight;
                if (fits)
                {
                    if (bestFit is null || IsBetterCompactFit(candidate, bestFit.Value))
                    {
                        bestFit = candidate;
                    }
                }
                else if (bestFallback is null || IsBetterCompactFallback(candidate, bestFallback.Value, viewportHeight))
                {
                    bestFallback = candidate;
                }
            }
        }

        return bestFit ?? bestFallback ?? BuildLayout(viewportWidth, viewportHeight, 1, minFontSize, allowVerticalFit: hasHeightLimit);
    }

    private GridLayout ResolveMinimumVisibleLayout(int viewportWidth)
    {
        var baseFontSize = GetBaseFontSize();
        var minFontSize = Math.Min(baseFontSize, MinimumResponsiveFontSize);
        GridLayout? best = null;

        foreach (var columnCount in GetCompactColumnCandidates(viewportWidth, baseFontSize))
        {
            for (float fontSize = baseFontSize; fontSize >= minFontSize - 0.05f; fontSize -= ResponsiveFontStep)
            {
                var candidate = BuildLayout(viewportWidth, 0, columnCount, fontSize, allowVerticalFit: false);
                using var font = CreateSizedFont(fontSize);
                if (!IsWidthLegible(viewportWidth, candidate, font, strict: false))
                {
                    continue;
                }

                if (best is null ||
                    candidate.ContentHeight < best.Value.ContentHeight ||
                    candidate.ContentHeight == best.Value.ContentHeight && candidate.FontSize > best.Value.FontSize + 0.05f)
                {
                    best = candidate;
                }
            }
        }

        return best ?? BuildLayout(viewportWidth, 0, 1, minFontSize, allowVerticalFit: false);
    }

    private bool IsBetterCompactFit(GridLayout candidate, GridLayout current)
    {
        if (candidate.FontSize > current.FontSize + 0.05f)
        {
            return true;
        }

        if (Math.Abs(candidate.FontSize - current.FontSize) <= 0.05f && candidate.ContentHeight < current.ContentHeight)
        {
            return true;
        }

        return Math.Abs(candidate.FontSize - current.FontSize) <= 0.05f
               && candidate.ContentHeight == current.ContentHeight
               && Math.Abs(candidate.ColumnCount - 2) < Math.Abs(current.ColumnCount - 2);
    }

    private static bool IsBetterCompactFallback(GridLayout candidate, GridLayout current, int viewportHeight)
    {
        var candidateOverflow = Math.Max(0, candidate.ContentHeight - viewportHeight);
        var currentOverflow = Math.Max(0, current.ContentHeight - viewportHeight);
        if (candidateOverflow != currentOverflow)
        {
            return candidateOverflow < currentOverflow;
        }

        return candidate.FontSize > current.FontSize + 0.05f;
    }

    private GridLayout BuildLayout(int viewportWidth, int viewportHeight, int columnCount, float fontSize, bool allowVerticalFit)
    {
        columnCount = Math.Clamp(columnCount, 1, Math.Max(1, Math.Min(MaximumResponsiveColumns, _rows.Count)));
        using var font = CreateSizedFont(fontSize);

        var baseFontSize = GetBaseFontSize();
        var fontScale = fontSize / Math.Max(1f, baseFontSize);
        var chromeScale = Math.Clamp(fontScale, 0.55f, 1.15f);

        int basePadding = ScalePx(_gridPadding);
        int minPadding = ScalePx(MinimumResponsivePadding);
        int padding = Math.Clamp((int)Math.Round(basePadding * chromeScale), minPadding, Math.Max(minPadding, basePadding + ScalePx(1)));

        int baseHeader = ScalePx(_headerHeight);
        int minHeader = ScalePx(MinimumResponsiveHeaderHeight);
        int headerHeight = Math.Clamp((int)Math.Round(baseHeader * Math.Clamp(fontScale, 0.70f, 1.10f)), minHeader, Math.Max(minHeader, baseHeader + ScalePx(2)));

        int textHeight = MeasureSingleLineHeight(font);
        int minRowHeight = Math.Max(ScalePx(MinimumResponsiveRowHeight), textHeight + ScalePx(2));
        int baseRowHeight = ScalePx(_rowHeight);
        int rowHeight = Math.Max(minRowHeight, (int)Math.Round(baseRowHeight * chromeScale));

        int visualRows = _rows.Count == 0 ? 0 : (_rows.Count + columnCount - 1) / columnCount;
        if (allowVerticalFit && viewportHeight > 0 && visualRows > 0)
        {
            var compactPadding = padding;
            var compactHeader = headerHeight;
            var availableRowsHeight = viewportHeight - (compactPadding * 2) - compactHeader;
            if (availableRowsHeight < visualRows * minRowHeight)
            {
                compactPadding = minPadding;
                compactHeader = minHeader;
                availableRowsHeight = viewportHeight - (compactPadding * 2) - compactHeader;
            }

            padding = compactPadding;
            headerHeight = compactHeader;
            if (availableRowsHeight > 0)
            {
                rowHeight = Math.Clamp(availableRowsHeight / visualRows, minRowHeight, rowHeight);
            }
        }

        int pairGap = Math.Max(ScalePx(4), (int)Math.Round(ScalePx(_colGap) * Math.Clamp(fontScale, 0.55f, 1.0f)));
        int columnGap = Math.Max(ScalePx(6), (int)Math.Round(ScalePx(_colGap + 8) * Math.Clamp(fontScale, 0.45f, 1.0f)));
        int itemInset = Math.Max(ScalePx(2), (int)Math.Round(ScalePx(6) * Math.Clamp(fontScale, 0.50f, 1.0f)));
        int contentHeight = padding + headerHeight + (visualRows * rowHeight) + padding;

        return new GridLayout(
            columnCount,
            visualRows,
            padding,
            headerHeight,
            rowHeight,
            pairGap,
            columnGap,
            itemInset,
            contentHeight,
            fontSize);
    }

    private int ResolveNaturalColumnCount(int viewportWidth, float fontSize)
    {
        if (_rows.Count <= 1)
        {
            return 1;
        }

        var candidate = BuildLayout(viewportWidth, 0, 2, fontSize, allowVerticalFit: false);
        using var font = CreateSizedFont(fontSize);
        return IsWidthLegible(viewportWidth, candidate, font, strict: false) ? 2 : 1;
    }

    private IEnumerable<int> GetCompactColumnCandidates(int viewportWidth, float baseFontSize)
    {
        int maxColumns = Math.Max(1, Math.Min(MaximumResponsiveColumns, _rows.Count));
        var minFontSize = Math.Min(baseFontSize, MinimumResponsiveFontSize);
        for (int columnCount = maxColumns; columnCount >= 1; columnCount--)
        {
            var candidate = BuildLayout(viewportWidth, 0, columnCount, minFontSize, allowVerticalFit: false);
            using var font = CreateSizedFont(minFontSize);
            if (columnCount == 1 || IsWidthLegible(viewportWidth, candidate, font, strict: false))
            {
                yield return columnCount;
            }
        }
    }

    private bool IsWidthLegible(int viewportWidth, GridLayout layout, Font font, bool strict)
    {
        int pairWidth = GetPairWidth(viewportWidth, layout);
        if (pairWidth <= 0)
        {
            return false;
        }

        int desiredWidth = MeasureDesiredLabelWidth(font) + MeasureDesiredValueWidth(font) + layout.PairGap + layout.ItemInset;
        if (strict)
        {
            return pairWidth >= desiredWidth;
        }

        int minimumReadableWidth = Math.Max(
            ScalePx(86),
            MeasureTextWidth("Accuracy:", font) + MeasureTextWidth("00.0 %", font) + layout.PairGap + layout.ItemInset);

        return pairWidth >= Math.Min(desiredWidth, minimumReadableWidth);
    }

    private int GetPairWidth(int viewportWidth, GridLayout layout)
    {
        int innerWidth = Math.Max(0, viewportWidth - (layout.Padding * 2));
        int totalGap = Math.Max(0, layout.ColumnCount - 1) * layout.ColumnGap;
        return Math.Max(1, (innerWidth - totalGap) / Math.Max(1, layout.ColumnCount));
    }

    private float GetBaseFontSize() => Math.Max(1f, Font.SizeInPoints);

    private Font CreateSizedFont(float sizeInPoints)
        => new(Font.FontFamily, sizeInPoints, Font.Style, GraphicsUnit.Point);

    private static int MeasureSingleLineHeight(Font font)
        => TextRenderer.MeasureText(
            "Hg",
            font,
            new Size(int.MaxValue, int.MaxValue),
            TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Height;

    private static int MeasureTextWidth(string text, Font font)
        => TextRenderer.MeasureText(
            text,
            font,
            new Size(int.MaxValue, int.MaxValue),
            TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;

    private void ClampScrollOffset()
    {
        int max = Math.Max(0, _contentHeight - ClientSize.Height);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, max);
    }

    private void SetScrollOffset(int value)
    {
        int max = Math.Max(0, _contentHeight - ClientSize.Height);
        int clamped = Math.Clamp(value, 0, max);
        if (clamped == _scrollOffset)
        {
            return;
        }

        var oldThumb = _thumbRect;
        _scrollOffset = clamped;
        if (_scrollbarVisible)
        {
            ComputeScrollbarBounds();
        }

        Invalidate(new Rectangle(0, 0, ContentViewportWidth, ClientSize.Height));
        if (_scrollbarVisible)
        {
            Invalidate(_scrollbarBounds);
            if (!oldThumb.IsEmpty) Invalidate(oldThumb);
            if (!_thumbRect.IsEmpty) Invalidate(_thumbRect);
        }
    }

    private void ScrollBy(int deltaPx) => SetScrollOffset(_scrollOffset + deltaPx);

    private void ScrollPage(int direction)
    {
        int page = Math.Max(0, ClientSize.Height - PageStepMarginPx);
        ScrollBy(direction * page);
    }

    private CellLayout[] GetCells(Rectangle bounds, GridLayout layout, Font font)
    {
        int columnCount = Math.Max(1, layout.ColumnCount);
        int totalGap = (columnCount - 1) * layout.ColumnGap;
        int pairWidth = Math.Max(1, (bounds.Width - totalGap) / columnCount);
        int contentWidth = Math.Min(bounds.Width, (pairWidth * columnCount) + totalGap);
        int startX = bounds.X + Math.Max(0, (bounds.Width - contentWidth) / 2);
        var cells = new CellLayout[columnCount];

        for (int i = 0; i < columnCount; i++)
        {
            var pairBounds = new Rectangle(startX + (i * (pairWidth + layout.ColumnGap)), bounds.Y, pairWidth, bounds.Height);
            var (label, value) = GetPairColumns(pairBounds, layout.PairGap, layout.ItemInset, font);
            cells[i] = new CellLayout(pairBounds, label, value);
        }

        return cells;
    }

    private (Rectangle label, Rectangle value) GetPairColumns(Rectangle pairBounds, int pairGap, int itemInset, Font font)
    {
        int pairInnerWidth = Math.Max(0, pairBounds.Width - pairGap);
        int minValueWidth = MeasureDesiredValueWidth(font);
        int desiredLabelWidth = MeasureDesiredLabelWidth(font);
        float totalWeight = Math.Max(0.01f, _wLabel + _wValue);
        int fallbackLabelWidth = (int)Math.Floor(pairInnerWidth * (_wLabel / totalWeight));
        int maxLabelWidth = Math.Max(0, pairInnerWidth - minValueWidth);
        int wLabel = Math.Min(Math.Max(desiredLabelWidth, fallbackLabelWidth), maxLabelWidth);
        int wValue = Math.Max(0, pairInnerWidth - wLabel);

        var label = new Rectangle(
            pairBounds.X + itemInset,
            pairBounds.Y,
            Math.Max(0, wLabel - itemInset),
            pairBounds.Height);

        var value = new Rectangle(
            pairBounds.X + wLabel + pairGap,
            pairBounds.Y,
            Math.Max(0, wValue - itemInset),
            pairBounds.Height);

        return (label, value);
    }

    private Rectangle GetVisualRowBounds(int index, GridLayout layout)
    {
        return new Rectangle(
            layout.Padding,
            layout.Padding + layout.HeaderHeight + (index * layout.RowHeight),
            Math.Max(1, ContentViewportWidth - (layout.Padding * 2)),
            layout.RowHeight);
    }

    private Rectangle GetItemBounds(int index)
    {
        if (index < 0 || index >= _rows.Count)
        {
            return Rectangle.Empty;
        }

        RelayoutAndScrollbar();
        var layout = _resolvedLayout;
        int columnCount = Math.Max(1, layout.ColumnCount);
        int visualRow = index / columnCount;
        int column = index % columnCount;
        using var font = CreateSizedFont(layout.FontSize);
        var cells = GetCells(GetVisualRowBounds(visualRow, layout), layout, font);
        var bounds = column < cells.Length ? cells[column].Pair : Rectangle.Empty;
        bounds.Offset(0, -_scrollOffset);
        return bounds;
    }

    #endregion

    #region Paint

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        RelayoutAndScrollbar();

        var g = e.Graphics;
        g.Clear(BackColor);

        var layout = _resolvedLayout;

        var t = CurrentTheme;

        var contentClip = new Rectangle(0, 0, ContentViewportWidth, ClientSize.Height);
        using var rowFont = CreateSizedFont(layout.FontSize);
        var state = g.Save();
        try
        {
            g.SetClip(contentClip);

            var rcHeader = new Rectangle(
                layout.Padding,
                layout.Padding - _scrollOffset,
                Math.Max(1, ContentViewportWidth - (layout.Padding * 2)),
                layout.HeaderHeight);
            if (rcHeader.Width > 0 && rcHeader.Height > 0 && rcHeader.Bottom >= 0)
            {
                using var headerBrush = new LinearGradientBrush(rcHeader, t.HeaderTop, t.HeaderBottom, 90f);
                g.FillRectangle(headerBrush, rcHeader);
            }

            for (int i = 0; i < layout.VisualRows; i++)
            {
                var rc = GetVisualRowBounds(i, layout);
                rc.Offset(0, -_scrollOffset);
                if (rc.Bottom < 0 || rc.Top > ClientSize.Height)
                {
                    continue;
                }

                var cells = GetCells(rc, layout, rowFont);
                for (int column = 0; column < cells.Length; column++)
                {
                    int itemIndex = (i * layout.ColumnCount) + column;
                    if (itemIndex >= _rows.Count)
                    {
                        continue;
                    }

                    var cell = cells[column];
                    DrawRowItem(g, _rows[itemIndex], itemIndex, cell.Pair, cell.Label, cell.Value, rowFont, t);
                }

                using var pen = new Pen(t.GridRowLine);
                g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);
            }
        }
        finally
        {
            g.Restore(state);
        }

        if (_scrollbarVisible)
        {
            DrawScrollbar(g);
        }
    }

    private void DrawRowItem(Graphics g, Row row, int itemIndex, Rectangle pairBounds, Rectangle labelBounds, Rectangle valueBounds, Font font, DataViewTheme theme)
    {
        if (itemIndex == _hoverItem)
        {
            using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
            g.FillRectangle(hov, pairBounds);
        }

        string statLabel = GetDisplayLabel(row);
        using var affectedLabelFont = row.AffectedByEd
            ? new Font("Segoe UI Symbol", font.SizeInPoints, font.Style, GraphicsUnit.Point)
            : null;
        var labelFont = affectedLabelFont ?? font;
        TextRenderer.DrawText(g, statLabel, labelFont, labelBounds, Color.FromArgb(200, theme.Accent), Color.Transparent, CellFlags | TextFormatFlags.Right);

        var (valueText, valueColor) = BuildValueCell(row, theme);
        TextRenderer.DrawText(g, valueText, font, valueBounds, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Left);
    }

    private void DrawScrollbar(Graphics g)
    {
        var theme = CurrentScrollTheme;

        using (var trackPen = new Pen(theme.Track, Math.Max(1, ScalePx(2))))
        {
            int cx = _scrollbarBounds.Left + _scrollbarBounds.Width / 2;
            g.DrawLine(trackPen, cx, _trackBounds.Top, cx, _trackBounds.Bottom);
        }

        using (var arrowBrush = new SolidBrush(_hoveringUpArrow || _hoveringDownArrow ? theme.Hover : theme.Bar))
        {
            var up = _upArrowRect;
            Point[] upArrow =
            [
                new(up.Left + up.Width / 2, up.Top + up.Height / 4),
                new(up.Left + ScalePx(3), up.Bottom - ScalePx(4)),
                new(up.Right - ScalePx(3), up.Bottom - ScalePx(4))
            ];
            g.FillPolygon(arrowBrush, upArrow);

            var down = _downArrowRect;
            Point[] downArrow =
            [
                new(down.Left + down.Width / 2, down.Bottom - down.Height / 4),
                new(down.Left + ScalePx(3), down.Top + ScalePx(4)),
                new(down.Right - ScalePx(3), down.Top + ScalePx(4))
            ];
            g.FillPolygon(arrowBrush, downArrow);
        }

        using var thumbBrush = new SolidBrush(_hoveringThumb ? theme.Hover : theme.Bar);
        g.FillRectangle(thumbBrush, _thumbRect);
    }

    private int MeasureDesiredLabelWidth(Font font)
    {
        var max = 0;
        foreach (var row in _rows)
        {
            using var affectedLabelFont = row.AffectedByEd
                ? new Font("Segoe UI Symbol", font.SizeInPoints, font.Style, GraphicsUnit.Point)
                : null;
            var labelFont = affectedLabelFont ?? font;
            var measured = TextRenderer.MeasureText(
                GetDisplayLabel(row),
                labelFont,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;
            max = Math.Max(max, measured);
        }

        // Reserve the drawable text width plus the same inset removed from the label rect.
        return Math.Max(ScalePx(56), max + ScalePx(6));
    }

    private int MeasureDesiredValueWidth(Font font)
    {
        var theme = CurrentTheme;
        var max = 0;
        foreach (var row in _rows)
        {
            var (valueText, _) = BuildValueCell(row, theme);
            var measured = TextRenderer.MeasureText(
                valueText,
                font,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;
            max = Math.Max(max, measured);
        }

        return Math.Max(ScalePx(58), max + ScalePx(4));
    }

    private static string GetDisplayLabel(Row row)
    {
        var compactLabel = row.Label.StartsWith("Accuracy (per activation)", StringComparison.Ordinal)
            ? row.Label.Replace("Accuracy (per activation)", "Accuracy", StringComparison.Ordinal)
            : row.Label;

        var statLabel = compactLabel.TrimEnd(':') + ":";
        return row.AffectedByEd ? statLabel + "  ⓔ" : statLabel;
    }

    private static (string text, Color color) BuildValueCell(Row row, DataViewTheme theme)
    {
        string enhanced = FormatNorm(row.EnhancedValue, row.Unit);
        bool changed = Math.Abs(row.EnhancedValue - row.BaseValue) >= Eps;
        var text = enhanced;

        if (!changed && row.NeutralWhenZero)
        {
            return (text, theme.GridNeutral);
        }

        var improved = row.HigherIsBetter
            ? row.EnhancedValue > row.BaseValue
            : row.EnhancedValue < row.BaseValue;

        var color = row.Band switch
        {
            1 when improved => theme.GridBandMid,
            2 when improved => theme.GridBandHigh,
            _ when improved => theme.GridBandLow,
            _ => theme.GridNeutral
        };

        return (text, color);
    }

    private static string FormatNorm(double v, string unit, bool sign = false)
    {
        // Normalize tiny values to zero to avoid "-0.000"
        if (Math.Abs(v) < 1e-9) v = 0.0;

        string num = Math.Abs(v) >= 100 ? v.ToString("0")
            : Math.Abs(v) >= 10 ? v.ToString("0.0")
            : Math.Abs(v) >= 1 ? v.ToString("0.00")
            : v.ToString("0.000");

        if (sign)
        {
            if (v > 0) num = "+" + num;
            else if (v < 0) /* leave minus */ ;
            else            /* v == 0 */ ; // no sign for zero
        }

        return string.IsNullOrEmpty(unit) ? num : $"{num} {unit}";
    }

    private static string FormatNormSigned(double v, string unit)
    {
        // Wrapper for convenience when we want sign for nonzero but never "+-0.000"
        return FormatNorm(v, unit, sign: true);
    }

    #endregion

    #region Scrollbar geometry

    private void ComputeScrollbarBounds()
    {
        if (!_scrollbarVisible)
        {
            _scrollbarBounds = Rectangle.Empty;
            _trackBounds = Rectangle.Empty;
            _upArrowRect = Rectangle.Empty;
            _downArrowRect = Rectangle.Empty;
            _thumbRect = Rectangle.Empty;
            return;
        }

        _scrollbarBounds = new Rectangle(
            Math.Max(0, ClientSize.Width - ScrollBarWidth),
            0,
            ScrollBarWidth,
            ClientSize.Height);

        _upArrowRect = new Rectangle(_scrollbarBounds.X, _scrollbarBounds.Y, _scrollbarBounds.Width, ArrowHeight);
        _downArrowRect = new Rectangle(_scrollbarBounds.X, _scrollbarBounds.Bottom - ArrowHeight, _scrollbarBounds.Width, ArrowHeight);
        _trackBounds = Rectangle.FromLTRB(
            _scrollbarBounds.Left,
            _upArrowRect.Bottom,
            _scrollbarBounds.Right,
            _downArrowRect.Top);

        int scrollMax = Math.Max(1, _contentHeight - ClientSize.Height);
        int trackHeight = Math.Max(0, _trackBounds.Height);
        int thumbHeight = Math.Max(ThumbMinHeight, (int)Math.Round((double)ClientSize.Height / Math.Max(1, _contentHeight) * trackHeight));
        thumbHeight = Math.Min(thumbHeight, trackHeight);

        int available = Math.Max(0, trackHeight - thumbHeight);
        int thumbY = _trackBounds.Top;
        if (available > 0)
        {
            double ratio = (double)_scrollOffset / scrollMax;
            thumbY = _trackBounds.Top + (int)Math.Round(available * ratio);
        }

        int inset = Math.Max(1, (int)Math.Round(_scrollbarBounds.Width * 0.25));
        _thumbRect = new Rectangle(
            _scrollbarBounds.Left + inset,
            thumbY,
            Math.Max(1, _scrollbarBounds.Width - (inset * 2)),
            thumbHeight);
    }

    #endregion

    #region Interaction (hover tooltips)

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);

        if (!_scrollbarVisible)
        {
            return;
        }

        int lines = SystemInformation.MouseWheelScrollLines;
        if (lines <= 0)
        {
            lines = 3;
        }

        int steps = e.Delta / 120 * lines;
        if (steps != 0)
        {
            ScrollBy(-steps * WheelStepPx);
        }
    }

    protected override bool IsInputKey(Keys keyData)
    {
        return keyData is Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End
               || base.IsInputKey(keyData);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!_scrollbarVisible)
        {
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.Up:
                ScrollBy(-ArrowStepPx);
                e.Handled = true;
                break;
            case Keys.Down:
                ScrollBy(ArrowStepPx);
                e.Handled = true;
                break;
            case Keys.PageUp:
                ScrollPage(-1);
                e.Handled = true;
                break;
            case Keys.PageDown:
                ScrollPage(1);
                e.Handled = true;
                break;
            case Keys.Home:
                SetScrollOffset(0);
                e.Handled = true;
                break;
            case Keys.End:
                SetScrollOffset(int.MaxValue);
                e.Handled = true;
                break;
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (!Focused)
        {
            Focus();
        }

        if (!_scrollbarVisible || !_scrollbarBounds.Contains(e.Location))
        {
            UpdateHoverAndTooltip(e.Location);
            return;
        }

        if (_thumbRect.Contains(e.Location))
        {
            _draggingThumb = true;
            _dragStartY = e.Y - _thumbRect.Y;
            Capture = true;
            return;
        }

        if (_upArrowRect.Contains(e.Location))
        {
            ScrollBy(-ArrowStepPx);
            return;
        }

        if (_downArrowRect.Contains(e.Location))
        {
            ScrollBy(ArrowStepPx);
            return;
        }

        if (_trackBounds.Contains(e.Location))
        {
            ScrollPage(e.Y < _thumbRect.Top ? -1 : 1);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_scrollbarVisible)
        {
            bool oldHoverThumb = _hoveringThumb;
            bool oldHoverUp = _hoveringUpArrow;
            bool oldHoverDown = _hoveringDownArrow;

            _hoveringThumb = _thumbRect.Contains(e.Location);
            _hoveringUpArrow = _upArrowRect.Contains(e.Location);
            _hoveringDownArrow = _downArrowRect.Contains(e.Location);

            if (_draggingThumb)
            {
                int trackTop = _trackBounds.Top;
                int trackHeight = Math.Max(0, _trackBounds.Height);
                int thumbHeight = _thumbRect.Height;
                int available = Math.Max(0, trackHeight - thumbHeight);
                if (available > 0 && _contentHeight > ClientSize.Height)
                {
                    int newThumbY = Math.Clamp(e.Y - _dragStartY, trackTop, trackTop + available);
                    double ratio = (double)(newThumbY - trackTop) / available;
                    int newScroll = (int)Math.Round(ratio * (_contentHeight - ClientSize.Height));
                    SetScrollOffset(newScroll);
                }
            }
            else if (oldHoverThumb != _hoveringThumb || oldHoverUp != _hoveringUpArrow || oldHoverDown != _hoveringDownArrow)
            {
                Invalidate(_scrollbarBounds);
            }
        }

        if (!_scrollbarVisible || !_scrollbarBounds.Contains(e.Location))
        {
            UpdateHoverAndTooltip(e.Location);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (_draggingThumb)
        {
            _draggingThumb = false;
            Capture = false;
        }
    }

    private void UpdateHoverAndTooltip(Point location)
    {
        int itemIdx = HitTestItem(location);
        if (itemIdx != _hoverItem)
        {
            _hoverItem = itemIdx;
            Invalidate(new Rectangle(0, 0, ContentViewportWidth, ClientSize.Height));

            if (itemIdx >= 0)
            {
                var row = _rows[itemIdx];
                if (!string.IsNullOrWhiteSpace(row.Tooltip))
                {
                    var rc = GetItemBounds(itemIdx);
                    _tooltip.ToolTipTitle = row.Label;
                    _tooltip.Show(row.Tooltip, this, rc.Left + 24, rc.Bottom);
                }
                else
                {
                    _tooltip.ToolTipTitle = string.Empty;
                    _tooltip.Hide(this);
                }
            }
            else
            {
                _tooltip.ToolTipTitle = string.Empty;
                _tooltip.Hide(this);
            }
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (_hoveringThumb || _hoveringUpArrow || _hoveringDownArrow)
        {
            _hoveringThumb = false;
            _hoveringUpArrow = false;
            _hoveringDownArrow = false;
            Invalidate(_scrollbarBounds);
        }

        _hoverItem = -1;
        _tooltip.ToolTipTitle = string.Empty;
        _tooltip.Hide(this);
        Invalidate();
    }

    private int HitTestItem(Point p)
    {
        if (_resolvedLayout.ColumnCount <= 0)
        {
            RelayoutAndScrollbar();
        }

        if (_scrollbarVisible && p.X >= ContentViewportWidth)
        {
            return -1;
        }

        var contentPoint = new Point(p.X, p.Y + _scrollOffset);
        var layout = _resolvedLayout;
        using var font = CreateSizedFont(layout.FontSize);
        for (int i = 0; i < layout.VisualRows; i++)
        {
            var cells = GetCells(GetVisualRowBounds(i, layout), layout, font);
            for (int column = 0; column < cells.Length; column++)
            {
                int itemIndex = (i * layout.ColumnCount) + column;
                if (itemIndex < _rows.Count && cells[column].Pair.Contains(contentPoint))
                {
                    return itemIndex;
                }
            }
        }

        return -1;
    }

    #endregion
}

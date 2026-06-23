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
        set { _headerHeight = Math.Max(16, value); Invalidate(); }
    }

    [DefaultValue(28)]
    public int RowHeight
    {
        get => _rowHeight;
        set { _rowHeight = Math.Max(18, value); Invalidate(); }
    }

    [DefaultValue(8)]
    public int GridPadding
    {
        get => _gridPadding;
        set { _gridPadding = Math.Max(0, value); Invalidate(); }
    }

    [DefaultValue(12)]
    public int ColumnGap
    {
        get => _colGap;
        set { _colGap = Math.Max(0, value); Invalidate(); }
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
        return new Size(width, MeasureContentHeight());
    }

    #endregion

    #region Private Methods

    private int ScalePx(int px) => (int)Math.Round(px * DpiScale);

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
        var preferredHeight = MeasureContentHeight();
        if (Dock != DockStyle.Fill && Height != preferredHeight)
        {
            Height = preferredHeight;
        }
    }

    private int MeasureContentHeight()
    {
        int gp = ScalePx(_gridPadding);
        int hh = ScalePx(_headerHeight);
        int rh = ScalePx(_rowHeight);
        int visualRows = (_rows.Count + 1) / 2;
        return gp + hh + (visualRows * rh) + gp;
    }

    private void RelayoutAndScrollbar()
    {
        _contentHeight = MeasureContentHeight();
        _scrollbarVisible = _contentHeight > ClientSize.Height && ClientSize.Height > 0;
        ClampScrollOffset();
        ComputeScrollbarBounds();
    }

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

    private (Rectangle leftPair, Rectangle leftLabel, Rectangle leftValue, Rectangle rightPair, Rectangle rightLabel, Rectangle rightValue) GetColumns(Rectangle bounds)
    {
        int pairGap = ScalePx(_colGap);
        int desiredCenterGap = ScalePx(_colGap + 16);
        int itemInset = ScalePx(6);
        int desiredLabelWidth = MeasureDesiredLabelWidth();
        int desiredValueWidth = MeasureDesiredValueWidth();
        int availableForPairs = Math.Max(2, bounds.Width - desiredCenterGap);
        int pairWidth = Math.Max(
            1,
            Math.Min(
                availableForPairs / 2,
                Math.Max(ScalePx(190), desiredLabelWidth + desiredValueWidth + pairGap + (itemInset * 2))));

        int centerGap = Math.Min(desiredCenterGap, Math.Max(ScalePx(6), bounds.Width - (pairWidth * 2)));

        int contentWidth = Math.Min(bounds.Width, pairWidth * 2 + centerGap);
        int startX = bounds.X + Math.Max(0, (bounds.Width - contentWidth) / 2);

        var leftPair = new Rectangle(startX, bounds.Y, pairWidth, bounds.Height);
        var rightPair = new Rectangle(leftPair.Right + centerGap, bounds.Y, pairWidth, bounds.Height);

        var left = GetPairColumns(leftPair, pairGap, itemInset);
        var right = GetPairColumns(rightPair, pairGap, itemInset);

        return (leftPair, left.label, left.value, rightPair, right.label, right.value);
    }

    private (Rectangle label, Rectangle value) GetPairColumns(Rectangle pairBounds, int pairGap, int itemInset)
    {
        int pairInnerWidth = Math.Max(0, pairBounds.Width - pairGap);
        int minValueWidth = MeasureDesiredValueWidth();
        int desiredLabelWidth = MeasureDesiredLabelWidth();
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

    private Rectangle GetVisualRowBounds(int index)
    {
        int gp = ScalePx(_gridPadding);
        int hh = ScalePx(_headerHeight);
        int rh = ScalePx(_rowHeight);
        return new Rectangle(gp, gp + hh + index * rh, Math.Max(1, ContentViewportWidth - gp * 2), rh);
    }

    private Rectangle GetItemBounds(int index)
    {
        if (index < 0 || index >= _rows.Count)
        {
            return Rectangle.Empty;
        }

        int visualRow = index / 2;
        bool isRight = (index % 2) == 1;
        var layout = GetColumns(GetVisualRowBounds(visualRow));
        var bounds = isRight ? layout.rightPair : layout.leftPair;
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

        int gp = ScalePx(_gridPadding);
        int hh = ScalePx(_headerHeight);
        int rh = ScalePx(_rowHeight);

        var t = CurrentTheme;

        var contentClip = new Rectangle(0, 0, ContentViewportWidth, ClientSize.Height);
        var state = g.Save();
        try
        {
            g.SetClip(contentClip);

            var rcHeader = new Rectangle(gp, gp - _scrollOffset, Math.Max(1, ContentViewportWidth - gp * 2), hh);
            if (rcHeader.Width > 0 && rcHeader.Height > 0 && rcHeader.Bottom >= 0)
            {
                using var headerBrush = new LinearGradientBrush(rcHeader, t.HeaderTop, t.HeaderBottom, 90f);
                g.FillRectangle(headerBrush, rcHeader);
            }

            int visualRows = (_rows.Count + 1) / 2;
            for (int i = 0; i < visualRows; i++)
            {
                int leftIndex = i * 2;
                int rightIndex = leftIndex + 1;
                var rc = GetVisualRowBounds(i);
                rc.Offset(0, -_scrollOffset);
                if (rc.Bottom < 0 || rc.Top > ClientSize.Height)
                {
                    continue;
                }

                var rcc = GetColumns(rc);

                if (leftIndex < _rows.Count)
                {
                    DrawRowItem(g, _rows[leftIndex], leftIndex, rcc.leftPair, rcc.leftLabel, rcc.leftValue, t);
                }

                if (rightIndex < _rows.Count)
                {
                    DrawRowItem(g, _rows[rightIndex], rightIndex, rcc.rightPair, rcc.rightLabel, rcc.rightValue, t);
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

    private void DrawRowItem(Graphics g, Row row, int itemIndex, Rectangle pairBounds, Rectangle labelBounds, Rectangle valueBounds, DataViewTheme theme)
    {
        if (itemIndex == _hoverItem)
        {
            using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
            g.FillRectangle(hov, pairBounds);
        }

        string statLabel = GetDisplayLabel(row);
        using var affectedLabelFont = row.AffectedByEd
            ? new Font("Segoe UI Symbol", Font.Size, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont)
            : null;
        var labelFont = affectedLabelFont ?? Font;
        TextRenderer.DrawText(g, statLabel, labelFont, labelBounds, Color.FromArgb(200, theme.Accent), Color.Transparent, CellFlags | TextFormatFlags.Right);

        var (valueText, valueColor) = BuildValueCell(row, theme);
        TextRenderer.DrawText(g, valueText, Font, valueBounds, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Left);
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

    private int MeasureDesiredLabelWidth()
    {
        var max = 0;
        foreach (var row in _rows)
        {
            using var affectedLabelFont = row.AffectedByEd
                ? new Font("Segoe UI Symbol", Font.Size, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont)
                : null;
            var labelFont = affectedLabelFont ?? Font;
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

    private int MeasureDesiredValueWidth()
    {
        var theme = CurrentTheme;
        var max = 0;
        foreach (var row in _rows)
        {
            var (valueText, _) = BuildValueCell(row, theme);
            var measured = TextRenderer.MeasureText(
                valueText,
                Font,
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
        if (_scrollbarVisible && p.X >= ContentViewportWidth)
        {
            return -1;
        }

        var contentPoint = new Point(p.X, p.Y + _scrollOffset);
        int visualRows = (_rows.Count + 1) / 2;
        for (int i = 0; i < visualRows; i++)
        {
            int leftIndex = i * 2;
            int rightIndex = leftIndex + 1;
            var layout = GetColumns(GetVisualRowBounds(i));

            if (leftIndex < _rows.Count && layout.leftPair.Contains(contentPoint))
            {
                return leftIndex;
            }

            if (rightIndex < _rows.Count && layout.rightPair.Contains(contentPoint))
            {
                return rightIndex;
            }
        }

        return -1;
    }

    #endregion
}

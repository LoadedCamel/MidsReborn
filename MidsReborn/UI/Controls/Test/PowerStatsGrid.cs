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

    private double DpiScale => DeviceDpi / 96.0;

    #endregion

    #region Constructor

    public PowerStatsGrid()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint |
                 ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        TabStop = false;
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
        Invalidate();
    }

    public void Clear()
    {
        _rows.Clear();
        _hoverItem = -1;
        _tooltip.Hide(this);
        AutoSizeHeight();
        Invalidate();
    }

    #endregion

    #region Private Methods

    private int ScalePx(int px) => (int)Math.Round(px * DpiScale);

    #endregion

    #region Overrides

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        AutoSizeHeight(); // recompute to reflect scaled row/header
        Invalidate();
    }

    #endregion

    #region Layout helpers

    private void AutoSizeHeight()
    {
        int gp = ScalePx(_gridPadding);
        int hh = ScalePx(_headerHeight);
        int rh = ScalePx(_rowHeight);
        int visualRows = (_rows.Count + 1) / 2;
        int h = gp + hh + (visualRows * rh) + gp;
        Height = h;
    }

    private (Rectangle leftPair, Rectangle leftLabel, Rectangle leftValue, Rectangle rightPair, Rectangle rightLabel, Rectangle rightValue) GetColumns(Rectangle bounds)
    {
        int pairGap = ScalePx(_colGap);
        int centerGap = ScalePx(_colGap + 6);
        int itemInset = ScalePx(4);

        int wAvail = Math.Max(0, bounds.Width - centerGap);
        int leftPairWidth = wAvail / 2;
        int rightPairWidth = wAvail - leftPairWidth;

        var leftPair = new Rectangle(bounds.X, bounds.Y, leftPairWidth, bounds.Height);
        var rightPair = new Rectangle(leftPair.Right + centerGap, bounds.Y, rightPairWidth, bounds.Height);

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
        return new Rectangle(gp, gp + hh + index * rh, Width - gp * 2, rh);
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
        return isRight ? layout.rightPair : layout.leftPair;
    }

    #endregion

    #region Paint

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.Clear(BackColor);

        int gp = ScalePx(_gridPadding);
        int hh = ScalePx(_headerHeight);
        int rh = ScalePx(_rowHeight);

        var t = CurrentTheme;

        // Header
        var rcHeader = new Rectangle(gp, gp, Width - gp * 2, hh);
        using (var headerBrush = new LinearGradientBrush(rcHeader, t.HeaderTop, t.HeaderBottom, 90f))
            g.FillRectangle(headerBrush, rcHeader);

        var cols = GetColumns(rcHeader);
        // Rows
        int visualRows = (_rows.Count + 1) / 2;
        for (int i = 0; i < visualRows; i++)
        {
            int leftIndex = i * 2;
            int rightIndex = leftIndex + 1;
            var rc = GetVisualRowBounds(i);
            var rcc = GetColumns(rc);

            if (leftIndex < _rows.Count)
            {
                DrawRowItem(g, _rows[leftIndex], leftIndex, rcc.leftPair, rcc.leftLabel, rcc.leftValue, t);
            }

            if (rightIndex < _rows.Count)
            {
                DrawRowItem(g, _rows[rightIndex], rightIndex, rcc.rightPair, rcc.rightLabel, rcc.rightValue, t);
            }

            // Row separator
            using var pen = new Pen(t.GridRowLine);
            g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);
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

    #region Interaction (hover tooltips)

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        int itemIdx = HitTestItem(e.Location);
        if (itemIdx != _hoverItem)
        {
            _hoverItem = itemIdx;
            Invalidate(); // hover band

            _tooltip.Hide(this);
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
                }
            }
            else
            {
                _tooltip.ToolTipTitle = string.Empty;
            }
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoverItem = -1;
        _tooltip.ToolTipTitle = string.Empty;
        _tooltip.Hide(this);
        Invalidate();
    }

    private int HitTestItem(Point p)
    {
        int visualRows = (_rows.Count + 1) / 2;
        for (int i = 0; i < visualRows; i++)
        {
            int leftIndex = i * 2;
            int rightIndex = leftIndex + 1;
            var layout = GetColumns(GetVisualRowBounds(i));

            if (leftIndex < _rows.Count && layout.leftPair.Contains(p))
            {
                return leftIndex;
            }

            if (rightIndex < _rows.Count && layout.rightPair.Contains(p))
            {
                return rightIndex;
            }
        }

        return -1;
    }

    #endregion
}

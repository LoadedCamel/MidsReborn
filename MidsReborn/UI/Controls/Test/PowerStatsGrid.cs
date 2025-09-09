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
        TextFormatFlags.NoPadding;

    private const TextFormatFlags HeaderFlags =
        TextFormatFlags.VerticalCenter |
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
    private int _hoverRow = -1;

    // Column widths as fractions (sum <= 1). We compute device px each paint.
    private float _wLabel = 0.30f;
    private float _wBase = 0.16f;
    private float _wEnh = 0.22f;
    private float _wDelta = 0.16f;
    private float _wDeltaPct = 0.16f;

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

    #region Public API

    public void SetRows(IEnumerable<Row> rows)
    {
        _rows.Clear();
        _rows.AddRange(rows);
        _hoverRow = -1;
        _tooltip.Hide(this);
        AutoSizeHeight();
        Invalidate();
    }

    public void Clear()
    {
        _rows.Clear();
        _hoverRow = -1;
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
        int h = gp + hh + (_rows.Count * rh) + gp;
        // Only grow if needed (don’t shrink layout unexpectedly)
        Height = Math.Max(Height, h);
    }

    private (Rectangle rcLabel, Rectangle rcBase, Rectangle rcEnh, Rectangle rcDelta, Rectangle rcDeltaPct) GetColumns(Rectangle bounds)
    {
        int gap = ScalePx(_colGap);
        int totalGap = gap * 4;

        int wAvail = bounds.Width - totalGap;
        int wLabel = (int)Math.Floor(wAvail * _wLabel);
        int wBase = (int)Math.Floor(wAvail * _wBase);
        int wEnh = (int)Math.Floor(wAvail * _wEnh);
        int wDelta = (int)Math.Floor(wAvail * _wDelta);
        int wPct = Math.Max(0, wAvail - (wLabel + wBase + wEnh + wDelta + 5)); // remainder

        int x = bounds.X;
        var c0 = new Rectangle(x + 5, bounds.Y, wLabel, bounds.Height); x += wLabel + gap;
        var c1 = new Rectangle(x, bounds.Y, wBase, bounds.Height); x += wBase + gap;
        var c2 = new Rectangle(x, bounds.Y, wEnh, bounds.Height); x += wEnh + gap;
        var c3 = new Rectangle(x, bounds.Y, wDelta, bounds.Height); x += wDelta + gap;
        var c4 = new Rectangle(x, bounds.Y, wPct, bounds.Height);

        return (c0, c1, c2, c3, c4);
    }

    private Rectangle GetRowBounds(int index)
    {
        int gp = ScalePx(_gridPadding);
        int hh = ScalePx(_headerHeight);
        int rh = ScalePx(_rowHeight);
        return new Rectangle(gp, gp + hh + index * rh, Width - gp * 2, rh);
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
        TextRenderer.DrawText(g, "Stat", Font, cols.rcLabel, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Left);
        TextRenderer.DrawText(g, "Base", Font, cols.rcBase, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Right);
        TextRenderer.DrawText(g, "Enhanced", Font, cols.rcEnh, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Right);
        TextRenderer.DrawText(g, "Gain", Font, cols.rcDelta, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Right);
        TextRenderer.DrawText(g, "Gain%", Font, cols.rcDeltaPct, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Right);

        // Rows
        for (int i = 0; i < _rows.Count; i++)
        {
            var row = _rows[i];
            var rc = GetRowBounds(i);
            var rcc = GetColumns(rc);

            // zebra + hover
            using (var zebra = new SolidBrush(i % 2 == 0 ? t.GridRowEven : t.GridRowOdd))
                g.FillRectangle(zebra, rc);

            if (i == _hoverRow)
            {
                using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
                g.FillRectangle(hov, rc);
            }

            var label = row.Label + (row.AffectedByEd ? "  ⓔ" : "");
            TextRenderer.DrawText(g, label, Font, rcc.rcLabel, Color.FromArgb(200, t.Accent), Color.Transparent, CellFlags | TextFormatFlags.Left);

            // Numbers already scaled for display (e.g., Accuracy passed in as 86.6, unit "%")
            var baseTxt = FormatNorm(row.BaseValue, row.Unit);
            var enhTxt = FormatNorm(row.EnhancedValue, row.Unit);

            // Gain semantics: positive when beneficial (lower-is-better rows invert the sign)
            double rawDelta = row.EnhancedValue - row.BaseValue;
            bool improved = row.HigherIsBetter ? rawDelta > 0.0 : rawDelta < 0.0;

            double gain = row.HigherIsBetter ? rawDelta : -rawDelta;
            if (Math.Abs(gain) < 1e-9) gain = 0.0;

            string gainUnit = row.GainUnitOverride ?? row.Unit;

            // Neutral if no change, or both ~0
            bool noChange = Math.Abs(rawDelta) < Eps || (Math.Abs(row.BaseValue) < Eps && Math.Abs(row.EnhancedValue) < Eps);

            var bandLow = t.GridBandLow;
            var bandMid = t.GridBandMid;
            var bandHigh = t.GridBandHigh;
            var neutral = t.GridNeutral;

            Color valueColor = bandLow;
            switch (row.Band)
            {
                case -1: valueColor = (noChange && row.NeutralWhenZero) ? neutral : (improved ? bandLow : neutral); break;
                case 0: valueColor = (noChange && row.NeutralWhenZero) ? neutral : (improved ? bandLow : neutral); break;
                case 1: valueColor = (noChange && row.NeutralWhenZero) ? neutral : (improved ? bandMid : neutral); break;
                case 2: valueColor = (noChange && row.NeutralWhenZero) ? neutral : (improved ? bandHigh : neutral); break;
            }

            // Base / Enhanced
            TextRenderer.DrawText(g, baseTxt, Font, rcc.rcBase, neutral, Color.Transparent, CellFlags | TextFormatFlags.Right);
            TextRenderer.DrawText(g, enhTxt, Font, rcc.rcEnh, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Right);

            // Gain text — show dash when unchanged
            string gainTxt = (noChange && row.NeutralWhenZero) ? "—" : FormatNormSigned(gain, gainUnit);
            string gainPctTxt = "—";
            if (!row.HideGainPercent && !noChange && Math.Abs(row.BaseValue) >= 1e-9)
            {
                double gainPct = (gain / Math.Abs(row.BaseValue)) * 100.0;
                gainPctTxt = FormatNorm(gainPct, "%", sign: false);
            }

            // Gain / Gain%
            TextRenderer.DrawText(g, gainTxt, Font, rcc.rcDelta, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Right);
            TextRenderer.DrawText(g, gainPctTxt, Font, rcc.rcDeltaPct, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Right);

            // Row separator
            using var pen = new Pen(Color.FromArgb(24, 255, 255, 255));
            g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);
        }
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

        int rowIdx = HitTestRow(e.Location);
        if (rowIdx != _hoverRow)
        {
            _hoverRow = rowIdx;
            Invalidate(); // hover band

            _tooltip.Hide(this);
            if (rowIdx >= 0)
            {
                var row = _rows[rowIdx];
                if (!string.IsNullOrWhiteSpace(row.Tooltip))
                {
                    var rc = GetRowBounds(rowIdx);
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
        _hoverRow = -1;
        _tooltip.ToolTipTitle = string.Empty;
        _tooltip.Hide(this);
        Invalidate();
    }

    private int HitTestRow(Point p)
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            if (GetRowBounds(i).Contains(p)) return i;
        }
        return -1;
    }

    #endregion
}
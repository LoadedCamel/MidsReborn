using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

public class PowerEffectsGrid : Control
{
    #region Types

    public abstract class Row
    {
        public string Label { get; }
        public string? Tooltip { get; }

        protected Row(string? label, string? tooltip = null)
        {
            Label = label ?? string.Empty;
            Tooltip = tooltip;
        }
    }

    public sealed class NumericRow : Row
    {
        // Fully formatted texts (already scaled/with units), mirroring PowerStatsGrid display contract.
        public string BaseText { get; }
        public string EnhancedText { get; }
        public string GainText { get; }
        public string GainPctText { get; }
        public bool HigherIsBetter { get; }
        public bool AffectedByEd { get; }
        public bool NeutralWhenZero { get; }
        public bool HideGainPercent { get; }
        public int Band { get; }

        public NumericRow(string label,
                          string baseText, string enhancedText, string gainText, string gainPctText,
                          bool higherIsBetter = true, bool affectedByEd = false,
                          bool neutralWhenZero = true, bool hideGainPercent = false,
                          int band = -1, string? tooltip = null)
            : base(label, tooltip)
        {
            BaseText = baseText;
            EnhancedText = enhancedText;
            GainText = gainText;
            GainPctText = gainPctText;
            HigherIsBetter = higherIsBetter;
            AffectedByEd = affectedByEd;
            NeutralWhenZero = neutralWhenZero;
            HideGainPercent = hideGainPercent;
            Band = band;
        }
    }

    public sealed class MezRow : Row
    {
        // Magnitude channel
        public double? BaseMagnitude { get; }
        public double? EnhancedMagnitude { get; }
        public bool HigherIsBetterMagnitude { get; }
        public bool AffectedByEdMagnitude { get; }
        public bool NeutralWhenZeroMagnitude { get; }
        public bool HideGainPercentMagnitude { get; }
        public int BandMagnitude { get; }

        // Duration channel (seconds)
        public double? BaseDuration { get; }
        public double? EnhancedDuration { get; }
        public bool HigherIsBetterDuration { get; }
        public bool AffectedByEdDuration { get; }
        public bool NeutralWhenZeroDuration { get; }
        public bool HideGainPercentDuration { get; }
        public int BandDuration { get; }

        public MezRow(string label,
                      double? baseMagnitude, double? enhancedMagnitude,
                      double? baseDuration, double? enhancedDuration,
                      bool higherIsBetterMagnitude = true,
                      bool higherIsBetterDuration = true,
                      bool affectedByEdMagnitude = false,
                      bool affectedByEdDuration = false,
                      bool neutralWhenZeroMagnitude = true,
                      bool neutralWhenZeroDuration = true,
                      bool hideGainPercentMagnitude = false,
                      bool hideGainPercentDuration = false,
                      int bandMagnitude = -1,
                      int bandDuration = -1,
                      string? tooltip = null)
            : base(label, tooltip)
        {
            BaseMagnitude = baseMagnitude;
            EnhancedMagnitude = enhancedMagnitude;
            BaseDuration = baseDuration;
            EnhancedDuration = enhancedDuration;

            HigherIsBetterMagnitude = higherIsBetterMagnitude;
            HigherIsBetterDuration = higherIsBetterDuration;

            AffectedByEdMagnitude = affectedByEdMagnitude;
            AffectedByEdDuration = affectedByEdDuration;

            NeutralWhenZeroMagnitude = neutralWhenZeroMagnitude;
            NeutralWhenZeroDuration = neutralWhenZeroDuration;

            HideGainPercentMagnitude = hideGainPercentMagnitude;
            HideGainPercentDuration = hideGainPercentDuration;

            BandMagnitude = bandMagnitude;
            BandDuration = bandDuration;
        }
    }

    public sealed class DescriptorRow : Row
    {
        public string Tag { get; }
        public string Description { get; }

        public DescriptorRow(string label, string tag, string? description, string? tooltip = null)
            : base(label, tooltip)
        {
            Tag = string.IsNullOrWhiteSpace(tag) ? "Descriptor" : tag;
            Description = description ?? string.Empty;
        }
    }

    public sealed class Group
    {
        public string Title { get; }
        public IReadOnlyList<Row> Rows { get; }
        public Group(string? title, IReadOnlyList<Row>? rows)
        {
            Title = title ?? string.Empty;
            Rows = rows ?? Array.Empty<Row>();
        }
    }

    private enum RenderKind { GroupHeader, NumericRow, MezLabel, MezMag, MezDur, DescriptorRow }

    private sealed class RenderEntry
    {
        public RenderKind Kind { get; }
        public Rectangle Bounds { get; }
        public int GroupIndex { get; }
        public int RowIndex { get; }
        public Row? Row { get; }
        public string Title { get; }

        private RenderEntry(RenderKind kind, Rectangle bounds, int gi, int ri, Row? row, string title)
        {
            Kind = kind; Bounds = bounds; GroupIndex = gi; RowIndex = ri; Row = row; Title = title;
        }

        public static RenderEntry GHeader(int gi, Rectangle r, string title) => new(RenderKind.GroupHeader, r, gi, -1, null, title);
        public static RenderEntry Num(int gi, int ri, Rectangle r, Row row) => new(RenderKind.NumericRow, r, gi, ri, row, string.Empty);
        public static RenderEntry MezLbl(int gi, int ri, Rectangle r, Row row) => new(RenderKind.MezLabel, r, gi, ri, row, string.Empty);
        public static RenderEntry MezMagRow(int gi, int ri, Rectangle r, Row row) => new(RenderKind.MezMag, r, gi, ri, row, string.Empty);
        public static RenderEntry MezDurRow(int gi, int ri, Rectangle r, Row row) => new(RenderKind.MezDur, r, gi, ri, row, string.Empty);
        public static RenderEntry Desc(int gi, int ri, Rectangle r, Row row) => new(RenderKind.DescriptorRow, r, gi, ri, row, string.Empty);
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

    private readonly List<Group> _groups = new();
    private readonly List<RenderEntry> _layout = new();

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

    // Logical (96-dpi) layout; scaled at paint time
    private int _headerHeight = 28;      // Top column header (same as PowerStatsGrid)
    private int _rowHeight = 28;         // Numeric row height
    private int _gridPadding = 8;
    private int _colGap = 12;

    // Grouped extras
    private int _groupHeaderHeight = 32;
    private int _mezLabelHeight = 24;
    private int _mezChildHeight = 28;    // per Magnitude / Duration child row
    private int _descriptorRowHeight = 56;
    private int _mezIndent = 18;

    private int _hoverIndex = -1;

    // Column width fractions (sum <= 1) like PowerStatsGrid
    private float _wLabel = 0.40f;
    private float _wBase = 0.16f;
    private float _wEnh = 0.20f;
    private float _wDelta = 0.16f;
    private float _wDeltaPct = 0.16f;

    #endregion

    #region Public Properties

    [Browsable(false)]
    public IReadOnlyList<Group> Groups => _groups;

    [DefaultValue(28)]
    public int HeaderHeight { get => _headerHeight; set { _headerHeight = Math.Max(16, value); InvalidateAndRelayout(); } }

    [DefaultValue(28)]
    public int RowHeight { get => _rowHeight; set { _rowHeight = Math.Max(18, value); InvalidateAndRelayout(); } }

    [DefaultValue(8)]
    public int GridPadding { get => _gridPadding; set { _gridPadding = Math.Max(0, value); InvalidateAndRelayout(); } }

    [DefaultValue(12)]
    public int ColumnGap { get => _colGap; set { _colGap = Math.Max(0, value); InvalidateAndRelayout(); } }

    [DefaultValue(32)]
    public int GroupHeaderHeight { get => _groupHeaderHeight; set { _groupHeaderHeight = Math.Max(20, value); InvalidateAndRelayout(); } }

    [DefaultValue(24)]
    public int MezLabelHeight { get => _mezLabelHeight; set { _mezLabelHeight = Math.Max(18, value); InvalidateAndRelayout(); } }

    [DefaultValue(28)]
    public int MezChildHeight { get => _mezChildHeight; set { _mezChildHeight = Math.Max(18, value); InvalidateAndRelayout(); } }

    [DefaultValue(56)]
    public int DescriptorRowHeight { get => _descriptorRowHeight; set { _descriptorRowHeight = Math.Max(24, value); InvalidateAndRelayout(); } }

    [DefaultValue(18)]
    public int MezIndent { get => _mezIndent; set { _mezIndent = Math.Max(0, value); InvalidateAndRelayout(); } }

    #endregion

    #region Private Properties

    private DataViewTheme CurrentTheme
    {
        get
        {
            if (DesignMode) return ThemeManager.DesignTime.DataView;
            return ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;
        }
    }

    private double DpiScale => DeviceDpi / 96.0;

    #endregion

    #region Constructor

    public PowerEffectsGrid()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint | 
                 ControlStyles.Selectable |
                 ControlStyles.SupportsTransparentBackColor, true);

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

    public void SetGroups(IEnumerable<Group> groups)
    {
        _groups.Clear();
        if (groups is IReadOnlyList<Group> iList) _groups.AddRange(iList);
        else _groups.AddRange(groups);

        _hoverIndex = -1;
        _tooltip.Hide(this);

        RebuildLayout();
        AutoSizeHeight();
        Invalidate();
    }

    public void Clear()
    {
        _groups.Clear();
        _layout.Clear();
        _hoverIndex = -1;
        _tooltip.Hide(this);
        AutoSizeHeight();
        Invalidate();
    }

    #endregion

    #region Private Methods

    private int ScalePx(int px) => (int)Math.Round(px * DpiScale);

    private void InvalidateAndRelayout()
    {
        RebuildLayout();
        AutoSizeHeight();
        Invalidate();
    }

    private static bool IsZero(double v) => Math.Abs(v) < Eps;

    private static string FmtVal(double v, string unit, bool sign = false)
    {
        if (Math.Abs(v) < 1e-9) v = 0.0;
        string num = Math.Abs(v) >= 100 ? v.ToString("0")
                   : Math.Abs(v) >= 10 ? v.ToString("0.0")
                   : Math.Abs(v) >= 1 ? v.ToString("0.00")
                   : v.ToString("0.000");
        if (sign)
        {
            if (v > 0) num = "+" + num;
            else if (v < 0) { /* keep minus */ }
        }
        return string.IsNullOrEmpty(unit) ? num : $"{num} {unit}";
    }

    private static (string baseTxt, string enhTxt, string gainTxt, string pctTxt, bool noChange, bool improved)
        ComputeMezChannel(double? baseVal, double? enhVal, string unit, bool higherIsBetter, bool hidePct)
    {
        if (!baseVal.HasValue && !enhVal.HasValue)
            return ("—", "—", "—", "—", true, false);

        double b = baseVal ?? 0.0;
        double e = enhVal ?? 0.0;

        string baseTxt = baseVal.HasValue ? FmtVal(b, unit, sign: false) : "—";
        string enhTxt = enhVal.HasValue ? FmtVal(e, unit, sign: false) : "—";

        double rawDelta = e - b;
        bool improved = higherIsBetter ? rawDelta > 0.0 : rawDelta < 0.0;
        double gain = higherIsBetter ? rawDelta : -rawDelta;
        if (Math.Abs(gain) < 1e-9) gain = 0.0;

        string gainTxt = FmtVal(gain, unit, sign: true);
        bool noChange = Math.Abs(rawDelta) < Eps || (IsZero(b) && IsZero(e));

        string pctTxt = "—";
        if (!hidePct && !noChange && !IsZero(b))
        {
            double pct = (gain / Math.Abs(b)) * 100.0;
            pctTxt = FmtVal(pct, "%", sign: false);
        }

        return (baseTxt, enhTxt, gainTxt, pctTxt, noChange, improved);
    }

    private int HitTest(Point p)
    {
        for (int i = 0; i < _layout.Count; i++)
        {
            if (_layout[i].Bounds.Contains(p)) return i;
        }
        return -1;
    }

    #endregion

    #region Overrides

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        InvalidateAndRelayout();
    }

    #endregion

    #region Layout helpers

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

    private void RebuildLayout()
    {
        _layout.Clear();

        int gp = ScalePx(_gridPadding);
        int hh = ScalePx(_headerHeight);
        int y = gp;

        // For each group: header, then rows. Stripe alternates per visible row row.
        for (int gi = 0; gi < _groups.Count; gi++)
        {
            var g = _groups[gi];

            // Group header
            var ghr = new Rectangle(gp, y, Width - gp * 2, ScalePx(_groupHeaderHeight));
            _layout.Add(RenderEntry.GHeader(gi, ghr, g.Title));
            y += ghr.Height;

            for (int ri = 0; ri < g.Rows.Count; ri++)
            {
                var row = g.Rows[ri];

                switch (row)
                {
                    case NumericRow:
                        {
                            var r = new Rectangle(gp, y, Width - gp * 2, ScalePx(_rowHeight));
                            _layout.Add(RenderEntry.Num(gi, ri, r, row));
                            y += r.Height;
                            break;
                        }

                    case MezRow:
                        {
                            var rLbl = new Rectangle(gp, y, Width - gp * 2, ScalePx(_mezLabelHeight));
                            _layout.Add(RenderEntry.MezLbl(gi, ri, rLbl, row));
                            y += rLbl.Height;

                            int indent = ScalePx(_mezIndent);
                            var rMag = new Rectangle(gp + indent, y, Width - gp * 2 - indent, ScalePx(_mezChildHeight));
                            _layout.Add(RenderEntry.MezMagRow(gi, ri, rMag, row));
                            y += rMag.Height;

                            var rDur = new Rectangle(gp + indent, y, Width - gp * 2 - indent, ScalePx(_mezChildHeight));
                            _layout.Add(RenderEntry.MezDurRow(gi, ri, rDur, row));
                            y += rDur.Height;
                            break;
                        }

                    case DescriptorRow:
                        {
                            var r = new Rectangle(gp, y, Width - gp * 2, ScalePx(_descriptorRowHeight));
                            _layout.Add(RenderEntry.Desc(gi, ri, r, row));
                            y += r.Height;
                            break;
                        }
                }
            }
        }
    }

    private void AutoSizeHeight()
    {
        int gp = ScalePx(_gridPadding);
        int hh = ScalePx(_headerHeight);

        int total = gp + hh;
        foreach (var e in _layout)
            total += e.Bounds.Height;
        total += gp;

        Height = Math.Max(Height, total);
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
        var t = CurrentTheme;

        // Top header (same style as PowerStatsGrid)
        var rcHeader = new Rectangle(gp, gp, Width - gp * 2, hh);
        // using (var headerBrush = new LinearGradientBrush(rcHeader, t.GridHeaderTop, t.GridHeaderBottom, 90f))
        //     g.FillRectangle(headerBrush, rcHeader);
        //
        // var colsH = GetColumns(rcHeader);
        // TextRenderer.DrawText(g, "Effect", Font, colsH.rcLabel, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Left);
        // TextRenderer.DrawText(g, "Base", Font, colsH.rcBase, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Right);
        // TextRenderer.DrawText(g, "Enhanced", Font, colsH.rcEnh, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Right);
        // TextRenderer.DrawText(g, "Gain", Font, colsH.rcDelta, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Right);
        // TextRenderer.DrawText(g, "Gain%", Font, colsH.rcDeltaPct, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Right);

        // Rows
        bool stripeEven = true; // start zebra after header
        for (int i = 0; i < _layout.Count; i++)
        {
            var entry = _layout[i];
            var rc = entry.Bounds;

            switch (entry.Kind)
            {
                case RenderKind.GroupHeader:
                    {
                        using (var b = new LinearGradientBrush(rc, t.GridHeaderTop, t.GridHeaderBottom, 90f))
                            g.FillRectangle(b, rc);

                        var cols = GetColumns(rc);
                        TextRenderer.DrawText(g, entry.Title, Font, cols.rcLabel, t.Text, Color.Transparent, HeaderFlags | TextFormatFlags.Left);
                        if (!entry.Title.Contains("Grant"))
                        {
                            TextRenderer.DrawText(g, "Gain%", Font, cols.rcDeltaPct, t.GridNeutral, Color.Transparent,
                                HeaderFlags | TextFormatFlags.Right);
                            TextRenderer.DrawText(g, "Gain", Font, cols.rcDelta, t.GridNeutral, Color.Transparent,
                                HeaderFlags | TextFormatFlags.Right);
                            TextRenderer.DrawText(g, "Enhanced", Font, cols.rcEnh, t.Text, Color.Transparent,
                                HeaderFlags | TextFormatFlags.Right);
                            TextRenderer.DrawText(g, "Base", Font, cols.rcBase, t.GridNeutral, Color.Transparent,
                                HeaderFlags | TextFormatFlags.Right);
                        }

                        stripeEven = true; // reset zebra at group boundary
                        break;
                    }

                case RenderKind.NumericRow:
                    {
                        var row = (NumericRow)entry.Row!;
                        var cols = GetColumns(rc);

                        using (var zebra = new SolidBrush(stripeEven ? t.GridRowEven : t.GridRowOdd))
                            g.FillRectangle(zebra, rc);
                        if (i == _hoverIndex)
                        {
                            using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
                            g.FillRectangle(hov, rc);
                        }

                        // Label + ED badge
                        var label = row.Label + (row.AffectedByEd ? "  ⓔ" : "");
                        TextRenderer.DrawText(g, label, Font, cols.rcLabel, Color.FromArgb(200, t.Accent), Color.Transparent, CellFlags | TextFormatFlags.Left);

                        // Band color parity
                        bool noChange = row.GainText == "—" || (string.IsNullOrWhiteSpace(row.GainText) && string.IsNullOrWhiteSpace(row.GainPctText));
                        var bandLow = t.GridBandLow; var bandMid = t.GridBandMid; var bandHigh = t.GridBandHigh; var neutral = t.GridNeutral;
                        bool improved = !string.IsNullOrWhiteSpace(row.GainText) && row.GainText.TrimStart().StartsWith("+");

                        Color valueColor = bandLow;
                        switch (row.Band)
                        {
                            case -1:
                            case 0: valueColor = (noChange && row.NeutralWhenZero) ? neutral : (improved ? bandLow : neutral); break;
                            case 1: valueColor = (noChange && row.NeutralWhenZero) ? neutral : (improved ? bandMid : neutral); break;
                            case 2: valueColor = (noChange && row.NeutralWhenZero) ? neutral : (improved ? bandHigh : neutral); break;
                        }

                        TextRenderer.DrawText(g, row.BaseText, Font, cols.rcBase, neutral, Color.Transparent, CellFlags | TextFormatFlags.Right);
                        TextRenderer.DrawText(g, row.EnhancedText, Font, cols.rcEnh, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Right);

                        string gainTxt = (noChange && row.NeutralWhenZero) ? "—" : row.GainText;
                        string gainPctTx = (row.HideGainPercent || (noChange && row.NeutralWhenZero)) ? "—" : row.GainPctText;

                        TextRenderer.DrawText(g, gainTxt, Font, cols.rcDelta, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Right);
                        TextRenderer.DrawText(g, gainPctTx, Font, cols.rcDeltaPct, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Right);

                        using var pen = new Pen(Color.FromArgb(24, 255, 255, 255));
                        g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);

                        stripeEven = !stripeEven;
                        break;
                    }

                case RenderKind.MezLabel:
                    {
                        var row = (MezRow)entry.Row!;
                        using (var zebra = new SolidBrush(t.GridRowOdd))
                            g.FillRectangle(zebra, rc);
                        if (i == _hoverIndex)
                        {
                            using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
                            g.FillRectangle(hov, rc);
                        }

                        bool anyEd = row.AffectedByEdMagnitude || row.AffectedByEdDuration;
                        var label = row.Label + (anyEd ? "  ⓔ" : "");

                        var cols = GetColumns(rc);
                        TextRenderer.DrawText(g, label, Font, cols.rcLabel, t.Text, Color.Transparent, CellFlags | TextFormatFlags.Left);

                        using var pen = new Pen(Color.FromArgb(24, 255, 255, 255));
                        g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);
                        break;
                    }

                case RenderKind.MezMag:
                case RenderKind.MezDur:
                    {
                        bool isMag = entry.Kind == RenderKind.MezMag;
                        var row = (MezRow)entry.Row!;
                        var cols = GetColumns(rc);

                        using (var zebra = new SolidBrush(stripeEven ? t.GridRowEven : t.GridRowOdd))
                            g.FillRectangle(zebra, rc);
                        if (i == _hoverIndex)
                        {
                            using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
                            g.FillRectangle(hov, rc);
                        }

                        // Channel caption
                        string cap = isMag ? "Magnitude" : "Duration";
                        TextRenderer.DrawText(g, cap, Font, cols.rcLabel, t.GridNeutral, Color.Transparent, CellFlags | TextFormatFlags.Left);

                        // Channel values (rename locals to avoid PaintEventArgs 'e')
                        double? bVal = isMag ? row.BaseMagnitude : row.BaseDuration;
                        double? enhVal = isMag ? row.EnhancedMagnitude : row.EnhancedDuration;
                        bool hib = isMag ? row.HigherIsBetterMagnitude : row.HigherIsBetterDuration;
                        bool neutralZero = isMag ? row.NeutralWhenZeroMagnitude : row.NeutralWhenZeroDuration;
                        bool hidePct = isMag ? row.HideGainPercentMagnitude : row.HideGainPercentDuration;
                        int band = isMag ? row.BandMagnitude : row.BandDuration;

                        var (baseTxt, enhTxt, gainTxt, pctTxt, noChange, improved) =
                            ComputeMezChannel(bVal, enhVal, isMag ? "" : "s", hib, hidePct);

                        // Band color parity
                        var bandLow = t.GridBandLow; var bandMid = t.GridBandMid; var bandHigh = t.GridBandHigh; var neutral = t.GridNeutral;
                        Color valueColor = bandLow;
                        switch (band)
                        {
                            case -1:
                            case 0: valueColor = (noChange && neutralZero) ? neutral : (improved ? bandLow : neutral); break;
                            case 1: valueColor = (noChange && neutralZero) ? neutral : (improved ? bandMid : neutral); break;
                            case 2: valueColor = (noChange && neutralZero) ? neutral : (improved ? bandHigh : neutral); break;
                        }

                        TextRenderer.DrawText(g, baseTxt, Font, cols.rcBase, neutral, Color.Transparent, CellFlags | TextFormatFlags.Right);
                        TextRenderer.DrawText(g, enhTxt, Font, cols.rcEnh, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Right);
                        TextRenderer.DrawText(g, (noChange && neutralZero) ? "—" : gainTxt, Font, cols.rcDelta, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Right);
                        TextRenderer.DrawText(g, (hidePct || (noChange && neutralZero)) ? "—" : pctTxt, Font, cols.rcDeltaPct, valueColor, Color.Transparent, CellFlags | TextFormatFlags.Right);

                        using var pen = new Pen(Color.FromArgb(24, 255, 255, 255));
                        g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);

                        stripeEven = !stripeEven;
                        break;
                    }

                case RenderKind.DescriptorRow:
                    {
                        var row = (DescriptorRow)entry.Row!;
                        var cols = GetColumns(rc);

                        using (var zebra = new SolidBrush(stripeEven ? t.GridRowEven : t.GridRowOdd))
                            g.FillRectangle(zebra, rc);
                        if (i == _hoverIndex)
                        {
                            using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
                            g.FillRectangle(hov, rc);
                        }

                        // Tag chip
                        var chipH = Math.Min(rc.Height - 10, ScalePx(24));
                        var chipW = Math.Max(ScalePx(72), TextRenderer.MeasureText(row.Tag, Font).Width + ScalePx(14));
                        var chip = new Rectangle(rc.X + 10, rc.Y + (rc.Height - chipH) / 2, chipW, chipH);
                        using (var path = Rounded(chip, 6))
                        using (var sb = new SolidBrush(t.Chip))
                        using (var pn = new Pen(t.Border))
                        {
                            g.FillPath(sb, path);
                            g.DrawPath(pn, path);
                        }
                        TextRenderer.DrawText(g, row.Tag, Font,
                                              new Rectangle(chip.X + 8, chip.Y + 2, chip.Width - 12, chip.Height - 4),
                                              t.Text, Color.Transparent, CellFlags | TextFormatFlags.Left);

                        // Label + description
                        int textX = chip.Right + 8;
                        var rcTitle = new Rectangle(textX, rc.Y + 6, cols.rcLabel.Right, 20);
                        var rcDesc = new Rectangle(textX, rcTitle.Bottom, cols.rcLabel.Right, rc.Bottom - rcTitle.Bottom - 6);
                        TextRenderer.DrawText(g, row.Label, Font, rcTitle, t.Text, Color.Transparent, CellFlags | TextFormatFlags.Left);
                        if (!string.IsNullOrWhiteSpace(row.Description))
                            TextRenderer.DrawText(g, row.Description, Font, rcDesc, t.Muted, Color.Transparent, CellFlags | TextFormatFlags.Left);

                        using var pen = new Pen(Color.FromArgb(24, 255, 255, 255));
                        g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);

                        stripeEven = !stripeEven;
                        break;
                    }
            }
        }
    }


    private static GraphicsPath Rounded(Rectangle r, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        if (d <= 0) { path.AddRectangle(r); path.CloseFigure(); return path; }
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    #endregion

    #region Interaction (hover tooltips)

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        int hit = HitTest(e.Location);
        if (hit != _hoverIndex)
        {
            _hoverIndex = hit;
            Invalidate();

            _tooltip.Hide(this);
            if (hit >= 0)
            {
                var entry = _layout[hit];
                string? tip = entry.Row switch
                {
                    NumericRow nr => nr.Tooltip,
                    MezRow mr => mr.Tooltip,
                    DescriptorRow dr => dr.Tooltip ?? dr.Description,
                    _ => null
                };

                if (!string.IsNullOrWhiteSpace(tip))
                {
                    _tooltip.ToolTipTitle = entry.Row switch
                    {
                        NumericRow nr => nr.Label,
                        MezRow mr => mr.Label,
                        DescriptorRow dr => dr.Label,
                        _ => string.Empty
                    };
                    _tooltip.Show(tip, this, entry.Bounds.Left + 24, entry.Bounds.Bottom);
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
        _hoverIndex = -1;
        _tooltip.ToolTipTitle = string.Empty;
        _tooltip.Hide(this);
        Invalidate();
    }

    #endregion
}
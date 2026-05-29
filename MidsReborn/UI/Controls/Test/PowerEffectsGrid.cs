using System.ComponentModel;
using System.Drawing.Drawing2D;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls.Test
{
    public class PowerEffectsGrid : Control
    {
        #region Types

        public abstract class Row
        {
            public string Label { get; }
            public string? Tooltip { get; }
            public IReadOnlyList<string> ContextChips { get; }

            protected Row(string? label, string? tooltip = null, IEnumerable<string>? contextChips = null)
            {
                Label = label ?? string.Empty;
                Tooltip = tooltip;
                ContextChips = contextChips?
                    .Where(static c => !string.IsNullOrWhiteSpace(c))
                    .Select(static c => c.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray() ?? Array.Empty<string>();
            }
        }

        public sealed class NumericRow : Row
        {
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
                              int band = -1, string? tooltip = null, IEnumerable<string>? contextChips = null)
                : base(label, tooltip, contextChips)
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
            public double? BaseMagnitude { get; }
            public double? EnhancedMagnitude { get; }
            public bool HigherIsBetterMagnitude { get; }
            public bool AffectedByEdMagnitude { get; }
            public bool NeutralWhenZeroMagnitude { get; }
            public bool HideGainPercentMagnitude { get; }
            public int BandMagnitude { get; }

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
                          string? tooltip = null,
                          IEnumerable<string>? contextChips = null)
                : base(label, tooltip, contextChips)
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

            public DescriptorRow(string label, string tag, string? description, string? tooltip = null,
                IEnumerable<string>? contextChips = null)
                : base(label, tooltip, contextChips)
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

        // Scrollbar constants (logical @96dpi);
        private const int LogicalScrollBarWidth = 16;
        private const int LogicalArrowHeight = 16;
        private const int LogicalThumbMinHeight = 20;
        private const int LogicalWheelStepPx = 32;
        private const int LogicalArrowStepPx = 30;
        private const int LogicalPageStepMarginPx = 8;

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

        // Logical (96-dpi) grid metrics
        private int _headerHeight = 28;
        private int _rowHeight = 28;
        private int _gridPadding = 8;
        private int _colGap = 12;

        // Grouped extras
        private int _groupHeaderHeight = 32;
        private int _mezLabelHeight = 24;
        private int _mezChildHeight = 28;
        private int _descriptorRowHeight = 56;
        private int _mezIndent = 18;

        private int _hoverIndex = -1;

        // Column width fractions
        private float _wLabel = 0.46f;

        // Scrolling state
        private int _contentHeight;                 // total content height in pixels (device)
        private int _scrollOffset;                  // current vertical scroll offset
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

        private DataViewTheme CurrentTheme =>
            DesignMode ? ThemeManager.DesignTime.DataView : (ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView);

        private ScrollPanelTheme CurrentScrollTheme =>
            DesignMode ? ThemeManager.DesignTime.ScrollPanel : (ThemeManager.CurrentTheme?.ScrollPanel ?? ThemeManager.DesignTime.ScrollPanel);

        private double DpiScale => DeviceDpi / 96.0;
        private float DpiScaleF => DeviceDpi / 96f;

        private int ScrollBarWidth => (int)Math.Round(LogicalScrollBarWidth * DpiScale);
        private int ArrowHeight => (int)Math.Round(LogicalArrowHeight * DpiScale);
        private int ThumbMinHeight => (int)Math.Round(LogicalThumbMinHeight * DpiScale);
        private int WheelStepPx => (int)Math.Round(LogicalWheelStepPx * DpiScale);
        private int ArrowStepPx => (int)Math.Round(LogicalArrowStepPx * DpiScale);
        private int PageStepMarginPx => (int)Math.Round(LogicalPageStepMarginPx * DpiScale);

        private int ContentViewportWidth => Math.Max(0, ClientSize.Width - (_scrollbarVisible ? ScrollBarWidth : 0));
        private int ChipHeight => Math.Max(ScalePx(18), Font.Height + ScalePx(4));
        private int ChipGap => ScalePx(4);
        private int ChipTextPaddingX => ScalePx(5);
        private int TargetChipHeight(Font chipFont) => Math.Max(ScalePx(15), chipFont.Height + ScalePx(2));
        private int TargetChipTextPaddingX => ScalePx(4);
        private int ChipMeasureSlack => ScalePx(2);

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

            // Enable keyboard scroll
            TabStop = true;

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

        public void SetGroups(IEnumerable<Group> groups)
        {
            _groups.Clear();
            if (groups is IReadOnlyList<Group> iList) _groups.AddRange(iList);
            else _groups.AddRange(groups);

            _hoverIndex = -1;
            _tooltip.Hide(this);

            RelayoutAndScrollbar();
            Invalidate();
        }

        public void Clear()
        {
            _groups.Clear();
            _layout.Clear();
            _hoverIndex = -1;
            _tooltip.Hide(this);
            _contentHeight = 0;
            _scrollOffset = 0;
            _scrollbarVisible = false;
            Invalidate();
        }

        public void ScrollToTop()
        {
            SetScrollOffset(0);
        }

        #endregion

        #region Layout / Scrolling helpers

        private int ScalePx(int px) => (int)Math.Round(px * DpiScale);

        private void InvalidateAndRelayout()
        {
            RelayoutAndScrollbar();
            Invalidate();
        }

        private void RelayoutAndScrollbar()
        {
            // First pass: layout with full width (no bar) to measure total height.
            RebuildLayout(ContentWidthForLayout(withScrollbar: false), out int totalHeightNoBar);

            // Determine if a bar is needed.
            _contentHeight = totalHeightNoBar;
            bool need = _contentHeight > ClientSize.Height;

            // rebuild layout with reduced width to avoid painting under the bar.
            if (need)
            {
                RebuildLayout(ContentWidthForLayout(withScrollbar: true), out int totalHeightWithBar);
                _contentHeight = totalHeightWithBar;
            }

            _scrollbarVisible = need;
            ClampScrollOffset();
            ComputeScrollbarBounds();
        }

        private int ContentWidthForLayout(bool withScrollbar)
        {
            int gp = ScalePx(_gridPadding);
            int w = ClientSize.Width - (withScrollbar ? ScrollBarWidth : 0);
            return Math.Max(0, w - gp * 2);
        }

        private void ClampScrollOffset()
        {
            int max = Math.Max(0, _contentHeight - ClientSize.Height);
            if (_scrollOffset > max) _scrollOffset = max;
            if (_scrollOffset < 0) _scrollOffset = 0;
        }

        private void SetScrollOffset(int value)
        {
            int max = Math.Max(0, _contentHeight - ClientSize.Height);
            int clamped = Math.Max(0, Math.Min(value, max));
            if (clamped == _scrollOffset) return;

            var oldThumb = _thumbRect;          // invalidate the old area
            _scrollOffset = clamped;

            if (_scrollbarVisible)
                ComputeScrollbarBounds();       // <— recompute thumb based on new offset

            // Invalidate content viewport and the bar/thumb areas (old + new to avoid ghosting)
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RelayoutAndScrollbar();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            RelayoutAndScrollbar();
        }

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);
            RelayoutAndScrollbar();
        }

        #endregion

        #region Layout building

        private (Rectangle rcLabel, Rectangle rcValue) GetTwoColumns(Rectangle bounds)
        {
            int gap = ScalePx(_colGap);
            int wAvail = bounds.Width - gap;

            int wLabel = (int)Math.Floor(wAvail * _wLabel);
            int wValue = Math.Max(0, wAvail - wLabel);

            int x = bounds.X;
            var c0 = new Rectangle(x + 5, bounds.Y, wLabel, bounds.Height); x += wLabel + gap;
            var c1 = new Rectangle(x, bounds.Y, wValue, bounds.Height);

            return (c0, c1);
        }

        private void RebuildLayout(int innerWidth, out int totalHeight)
        {
            _layout.Clear();

            int gp = ScalePx(_gridPadding);
            int y = gp;

            // Each entry uses "innerWidth"; position at gp and restrict width to content viewport.
            for (int gi = 0; gi < _groups.Count; gi++)
            {
                var g = _groups[gi];

                // Group header
                var ghr = new Rectangle(gp, y, innerWidth, ScalePx(_groupHeaderHeight));
                _layout.Add(RenderEntry.GHeader(gi, ghr, g.Title));
                y += ghr.Height;

                for (int ri = 0; ri < g.Rows.Count; ri++)
                {
                    var row = g.Rows[ri];

                    switch (row)
                    {
                        case NumericRow:
                            {
                                var chips = GetSecondaryChips(row.Label, row.ContextChips);

                                int h = ScalePx(_rowHeight);
                                if (chips.Length > 0)
                                    h += ChipHeight + ScalePx(3);

                                var r = new Rectangle(gp, y, innerWidth, h);
                                _layout.Add(RenderEntry.Num(gi, ri, r, row));
                                y += r.Height;
                                break;
                            }

                        case MezRow:
                            {
                                var chips = GetSecondaryChips(row.Label, row.ContextChips);
                                int h = ScalePx(_mezLabelHeight);
                                if (chips.Length > 0)
                                    h += ChipHeight + ScalePx(3);

                                var rLbl = new Rectangle(gp, y, innerWidth, h);
                                _layout.Add(RenderEntry.MezLbl(gi, ri, rLbl, row));
                                y += rLbl.Height;
                                break;
                            }

                        case DescriptorRow:
                            {
                                var r = new Rectangle(gp, y, innerWidth, ScalePx(_descriptorRowHeight));
                                _layout.Add(RenderEntry.Desc(gi, ri, r, row));
                                y += r.Height;
                                break;
                            }
                    }
                }
            }

            totalHeight = y + gp;
        }

        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_scrollbarVisible)
                ComputeScrollbarBounds();

            base.OnPaint(e);

            var g = e.Graphics;
            var theme = CurrentTheme;

            // Clip to content viewport (exclude scrollbar strip)
            var contentClip = new Rectangle(0, 0, ContentViewportWidth, ClientSize.Height);
            using (var rgn = new Region(contentClip))
            {
                var prevClip = g.Clip;
                g.Clip = rgn;

                bool stripeEven = true;
                for (int i = 0; i < _layout.Count; i++)
                {
                    var entry = _layout[i];
                    var rc = entry.Bounds;

                    if (rc.Bottom < _scrollOffset || rc.Top > _scrollOffset + ClientSize.Height)
                        continue;

                    rc.Offset(0, -_scrollOffset);

                    switch (entry.Kind)
                    {
                        case RenderKind.GroupHeader:
                            {
                                using (var b = new LinearGradientBrush(rc, theme.GridHeaderTop, theme.GridHeaderBottom, 90f))
                                    g.FillRectangle(b, rc);

                                var cols = GetTwoColumns(rc);
                                // Left: group title acts like "Stat" caption region
                                TextRenderer.DrawText(g, string.IsNullOrWhiteSpace(entry.Title) ? "Stat" : entry.Title,
                                                      Font, cols.rcLabel, theme.Text, Color.Transparent,
                                                      HeaderFlags | TextFormatFlags.Left);
                                // Right: always "Value"
                                TextRenderer.DrawText(g, "Value", Font, cols.rcValue, theme.Text, Color.Transparent,
                                                      HeaderFlags | TextFormatFlags.Right);
                                stripeEven = true;
                                break;
                            }

                        case RenderKind.NumericRow:
                            {
                                var row = (NumericRow)entry.Row!;
                                var cols = GetTwoColumns(rc);

                                if (i == _hoverIndex)
                                {
                                    using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
                                    g.FillRectangle(hov, rc);
                                }

                                DrawLabelWithChips(g, row.Label, row.AffectedByEd, cols.rcLabel, theme,
                                    Color.FromArgb(200, theme.Accent), row.ContextChips);

                                // Compose Value cell = Enhanced + inline (Gain) + optional (%)
                                bool noChange = row.GainText == "—" ||
                                                (string.IsNullOrWhiteSpace(row.GainText) && string.IsNullOrWhiteSpace(row.GainPctText));

                                bool improved = !string.IsNullOrWhiteSpace(row.GainText) &&
                                                row.GainText.TrimStart().StartsWith("+");

                                var neutral = theme.GridNeutral;
                                var bandLow = theme.GridBandLow;
                                var bandMid = theme.GridBandMid;
                                var bandHigh = theme.GridBandHigh;

                                Color valueColor;
                                switch (row.Band)
                                {
                                    case 1: valueColor = (noChange && row.NeutralWhenZero) ? neutral : (improved ? bandMid : neutral); break;
                                    case 2: valueColor = (noChange && row.NeutralWhenZero) ? neutral : (improved ? bandHigh : neutral); break;
                                    default: valueColor = (noChange && row.NeutralWhenZero) ? neutral : (improved ? bandLow : neutral); break;
                                }

                                string valueText = BuildInlineValueText(row.EnhancedText,
                                                                        row.GainText,
                                                                        row.GainPctText,
                                                                        row.HideGainPercent,
                                                                        row.NeutralWhenZero,
                                                                        noChange);

                                TextRenderer.DrawText(g, valueText, Font, cols.rcValue, valueColor,
                                                      Color.Transparent, CellFlags | TextFormatFlags.Right);

                                using var pen = new Pen(theme.GridRowLine);
                                g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);

                                stripeEven = !stripeEven;
                                break;
                            }

                        case RenderKind.MezLabel:
                            {
                                var row = (MezRow)entry.Row!;
                                if (i == _hoverIndex)
                                {
                                    using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
                                    g.FillRectangle(hov, rc);
                                }

                                bool anyEd = row.AffectedByEdMagnitude || row.AffectedByEdDuration;
                                var label = row.Label + (anyEd ? "  ⓔ" : "");
                                var cols = GetTwoColumns(rc);

                                DrawLabelWithChips(g, label, false, cols.rcLabel, theme, theme.Text, row.ContextChips);
                                TextRenderer.DrawText(g, BuildMezSummary(row), Font, cols.rcValue, theme.GridNeutral, Color.Transparent,
                                                      CellFlags | TextFormatFlags.Right);

                                using var pen = new Pen(theme.GridRowLine);
                                g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);
                                break;
                            }

                        case RenderKind.MezMag:
                        case RenderKind.MezDur:
                            {
                                bool isMag = entry.Kind == RenderKind.MezMag;
                                var row = (MezRow)entry.Row!;
                                var cols = GetTwoColumns(rc);

                                if (i == _hoverIndex)
                                {
                                    using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
                                    g.FillRectangle(hov, rc);
                                }

                                string cap = isMag ? "Magnitude" : "Duration";
                                TextRenderer.DrawText(g, cap, Font, cols.rcLabel, theme.GridNeutral, Color.Transparent,
                                                      CellFlags | TextFormatFlags.Left);

                                double? bVal = isMag ? row.BaseMagnitude : row.BaseDuration;
                                double? eVal = isMag ? row.EnhancedMagnitude : row.EnhancedDuration;
                                bool hib = isMag ? row.HigherIsBetterMagnitude : row.HigherIsBetterDuration;
                                bool neutralZero = isMag ? row.NeutralWhenZeroMagnitude : row.NeutralWhenZeroDuration;
                                bool hidePct = isMag ? row.HideGainPercentMagnitude : row.HideGainPercentDuration;
                                int band = isMag ? row.BandMagnitude : row.BandDuration;

                                var (baseTxt, enhTxt, gainTxt, pctTxt, noChange, improved) =
                                    ComputeMezChannel(bVal, eVal, isMag ? "" : "s", hib, hidePct);

                                var neutral = theme.GridNeutral;
                                var bandLow = theme.GridBandLow;
                                var bandMid = theme.GridBandMid;
                                var bandHigh = theme.GridBandHigh;

                                Color valueColor;
                                switch (band)
                                {
                                    case 1: valueColor = (noChange && neutralZero) ? neutral : (improved ? bandMid : neutral); break;
                                    case 2: valueColor = (noChange && neutralZero) ? neutral : (improved ? bandHigh : neutral); break;
                                    default: valueColor = (noChange && neutralZero) ? neutral : (improved ? bandLow : neutral); break;
                                }

                                string valueText = BuildInlineValueText(enhTxt, gainTxt, pctTxt, hidePct, neutralZero, noChange);

                                TextRenderer.DrawText(g, valueText, Font, cols.rcValue, valueColor,
                                                      Color.Transparent, CellFlags | TextFormatFlags.Right);

                                using var pen = new Pen(theme.GridRowLine);
                                g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);

                                stripeEven = !stripeEven;
                                break;
                            }

                        case RenderKind.DescriptorRow:
                            {
                                var row = (DescriptorRow)entry.Row!;
                                var cols = GetTwoColumns(rc);

                                if (i == _hoverIndex)
                                {
                                    using var hov = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
                                    g.FillRectangle(hov, rc);
                                }

                                DrawLabelWithChips(g, row.Label, false, cols.rcLabel, theme, theme.Text,
                                    row.ContextChips);

                                // Value uses description if available; else the tag
                                var valueText = !string.IsNullOrWhiteSpace(row.Description)
                                                    ? row.Description
                                                    : row.Tag;

                                TextRenderer.DrawText(g, valueText, Font, cols.rcValue,
                                                      theme.GridNeutral, Color.Transparent,
                                                      CellFlags | TextFormatFlags.Right);

                                using var pen = new Pen(theme.GridRowLine);
                                g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);

                                stripeEven = !stripeEven;
                                break;
                            }
                    }
                }

                g.Clip = prevClip;
            }

            if (_scrollbarVisible)
                DrawScrollbar(e.Graphics);
        }

        private static string BuildInlineValueText(string enhanced, string gain, string pct, bool hideGainPercent, bool neutralWhenZero, bool noChange)
        {
            // Start with the primary value (Enhanced).
            string text = string.IsNullOrWhiteSpace(enhanced) ? "—" : enhanced;

            // Gain (absolute)
            if (!(noChange && neutralWhenZero) && !string.IsNullOrWhiteSpace(gain) && gain != "—")
            {
                text += $"  ({gain})";
            }

            // Percent
            if (!hideGainPercent && !(noChange && neutralWhenZero) && !string.IsNullOrWhiteSpace(pct) && pct != "—")
            {
                text += $"  ({pct})";
            }

            return text;
        }

        private static (string Main, string[] Chips) SplitLabelChips(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) return (string.Empty, []);

            int open = label.IndexOf(" (", StringComparison.Ordinal);
            int close = label.EndsWith(")", StringComparison.Ordinal) ? label.LastIndexOf(')') : -1;
            if (open <= 0 || close <= open) return (label, []);

            string head = label.Substring(0, open);
            string inside = label.Substring(open + 2, close - (open + 2)); // no parentheses

            if (inside.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                if (head.StartsWith("Defense", StringComparison.OrdinalIgnoreCase)) return ("Base Defense", []);
                if (head.StartsWith("Resistance", StringComparison.OrdinalIgnoreCase)) return ("Base Resistance", []);
                return ($"Base {head}", []);
            }

            var chips = inside
                .Split([',', '/'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            return (head, chips);
        }

        private static string[] GetSecondaryChips(string label, IReadOnlyList<string>? contextChips)
        {
            var (_, labelChips) = SplitLabelChips(label);
            var merged = labelChips
                .Concat(contextChips ?? Array.Empty<string>())
                .Where(static chip => !string.IsNullOrWhiteSpace(chip))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return merged;
        }

        private void DrawLabelWithChips(Graphics g, string label, bool affectedByEd, Rectangle bounds,
            DataViewTheme theme, Color labelColor, IReadOnlyList<string>? contextChips = null)
        {
            var (main, _) = SplitLabelChips(label);
            var chips = GetSecondaryChips(label, contextChips);
            if (affectedByEd)
            {
                main += "  ⓔ";
            }

            if (chips.Length == 0)
            {
                TextRenderer.DrawText(g, main, Font, bounds, labelColor, Color.Transparent,
                    CellFlags | TextFormatFlags.Left);
                return;
            }

            var mainRect = new Rectangle(bounds.X, bounds.Y + ScalePx(2), bounds.Width, Font.Height + ScalePx(2));
            TextRenderer.DrawText(g, main, Font, mainRect, labelColor, Color.Transparent,
                TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);

            var chipBounds = new Rectangle(bounds.X, mainRect.Bottom + ScalePx(1), bounds.Width,
                Math.Max(0, bounds.Bottom - mainRect.Bottom - ScalePx(1)));
            DrawChipRow(g, chips, chipBounds, theme);
        }

        private readonly record struct ChipMeasure(string Text, int Width);

        private ChipMeasure[] MeasureChips(Graphics g, IEnumerable<string> chips)
            => MeasureChips(g, chips, Font, ChipTextPaddingX);

        private ChipMeasure[] MeasureChips(Graphics g, IEnumerable<string> chips, Font font, int textPaddingX)
        {
            return chips
                .Where(static c => !string.IsNullOrWhiteSpace(c))
                .Select(c =>
                {
                    var textSize = TextRenderer.MeasureText(g, c, font, new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.NoPadding);
                    return new ChipMeasure(c, textSize.Width + textPaddingX * 2 + ChipMeasureSlack);
                })
                .ToArray();
        }

        private void DrawChipRow(Graphics g, IEnumerable<string> chips, Rectangle bounds, DataViewTheme theme)
            => DrawChipRow(g, MeasureChips(g, chips), bounds, theme);

        private void DrawChipRow(Graphics g, IReadOnlyList<ChipMeasure> chips, Rectangle bounds, DataViewTheme theme)
            => DrawChipRow(g, chips, bounds, theme, Font, ChipHeight, Color.FromArgb(48, theme.Accent),
                Color.FromArgb(128, theme.Accent), theme.Text, ChipTextPaddingX);

        private void DrawChipRow(Graphics g, IReadOnlyList<ChipMeasure> chips, Rectangle bounds, DataViewTheme theme,
            Font chipFont, int chipHeight, Color backColor, Color borderColor, Color textColor, int textPaddingX)
        {
            if (chips.Count == 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            int x = bounds.X;
            int y = bounds.Y + Math.Max(0, (bounds.Height - chipHeight) / 2);

            using var chipBack = new SolidBrush(backColor);
            using var chipBorder = new Pen(borderColor);

            foreach (var chip in chips)
            {
                var chipRect = new Rectangle(x, y, chip.Width, chipHeight);
                if (chipRect.Right > bounds.Right)
                {
                    break;
                }

                using (var path = RoundedRect(chipRect, ScalePx(4)))
                {
                    g.FillPath(chipBack, path);
                    g.DrawPath(chipBorder, path);
                }

                var textBounds = Rectangle.Inflate(chipRect, -textPaddingX, 0);
                TextRenderer.DrawText(g, chip.Text, chipFont, textBounds, textColor, Color.Transparent,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPadding);

                x = chipRect.Right + ChipGap;
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            var d = Math.Max(1, radius * 2);
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void DrawScrollbar(Graphics g)
        {
            var theme = CurrentScrollTheme;

            // Entire strip already reserved; draw centered track
            using (var trackPen = new Pen(theme.Track, Math.Max(1, (int)Math.Round(2 * DpiScale))))
            {
                int cx = _scrollbarBounds.Left + _scrollbarBounds.Width / 2;
                g.DrawLine(trackPen, cx, _trackBounds.Top, cx, _trackBounds.Bottom);
            }

            using (var arrowBrush = new SolidBrush(_hoveringUpArrow || _hoveringDownArrow ? theme.Hover : theme.Bar))
            {
                var up = _upArrowRect;
                Point[] upArrow =
                {
                    new(up.Left + up.Width/2, up.Top + up.Height/4),
                    new(up.Left + (int)Math.Round(3 * DpiScale),   up.Bottom - (int)Math.Round(4 * DpiScale)),
                    new(up.Right - (int)Math.Round(3 * DpiScale),  up.Bottom - (int)Math.Round(4 * DpiScale))
                };
                g.FillPolygon(arrowBrush, upArrow);

                var dn = _downArrowRect;
                Point[] downArrow =
                {
                    new(dn.Left + dn.Width/2, dn.Bottom - dn.Height/4),
                    new(dn.Left + (int)Math.Round(3 * DpiScale),   dn.Top + (int)Math.Round(4 * DpiScale)),
                    new(dn.Right - (int)Math.Round(3 * DpiScale),  dn.Top + (int)Math.Round(4 * DpiScale))
                };
                g.FillPolygon(arrowBrush, downArrow);
            }

            using (var thumbBrush = new SolidBrush(_hoveringThumb ? theme.Hover : theme.Bar))
            {
                g.FillRectangle(thumbBrush, _thumbRect);
            }
        }

        #endregion

        #region Format helpers

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

        private static string BuildMezSummary(MezRow row)
        {
            var magnitude = row.EnhancedMagnitude ?? row.BaseMagnitude;
            var duration = row.EnhancedDuration ?? row.BaseDuration;

            if (magnitude.HasValue && duration.HasValue && duration.Value > Eps)
            {
                return $"Mag {FmtVal(magnitude.Value, string.Empty)} for {FmtVal(duration.Value, "s")}";
            }

            if (magnitude.HasValue)
            {
                return $"Mag {FmtVal(magnitude.Value, string.Empty)}";
            }

            if (duration.HasValue && duration.Value > Eps)
            {
                return FmtVal(duration.Value, "s");
            }

            return "—";
        }

        #endregion

        #region Hit testing & interaction

        private int HitTestContent(Point viewPoint)
        {
            // Convert viewport point - content space by adding scroll offset
            var contentPt = new Point(viewPoint.X, viewPoint.Y + _scrollOffset);

            // Don't consider clicks in the scrollbar strip
            if (_scrollbarVisible && viewPoint.X >= ContentViewportWidth)
                return -1;

            for (int i = 0; i < _layout.Count; i++)
            {
                if (_layout[i].Bounds.Contains(contentPt)) return i;
            }
            return -1;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!_scrollbarVisible) return;

            int lines = SystemInformation.MouseWheelScrollLines;
            if (lines <= 0) lines = 3;

            int steps = e.Delta / 120 * lines;
            if (steps != 0) ScrollBy(-steps * WheelStepPx);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData is Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End
                || base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!_scrollbarVisible) return;

            switch (e.KeyCode)
            {
                case Keys.Up: ScrollBy(-ArrowStepPx); e.Handled = true; break;
                case Keys.Down: ScrollBy(ArrowStepPx); e.Handled = true; break;
                case Keys.PageUp: ScrollPage(-1); e.Handled = true; break;
                case Keys.PageDown: ScrollPage(+1); e.Handled = true; break;
                case Keys.Home: SetScrollOffset(0); e.Handled = true; break;
                case Keys.End: SetScrollOffset(int.MaxValue); e.Handled = true; break;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Focus for keyboard scrolling
            if (!Focused) Focus();

            if (_scrollbarVisible && _scrollbarBounds.Contains(e.Location))
            {
                // Scrollbar interaction
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
                    if (e.Y < _thumbRect.Top) ScrollPage(-1);
                    else if (e.Y > _thumbRect.Bottom) ScrollPage(+1);
                    return;
                }
            }

            // Otherwise content interaction - just update hover/tooltips immediately
            UpdateHoverAndTooltip(e.Location);
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
                        int newThumbY = e.Y - _dragStartY;
                        newThumbY = Math.Max(trackTop, Math.Min(newThumbY, trackTop + available));

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

            // Update content hover unless we're over the bar
            if (!(_scrollbarVisible && _scrollbarBounds.Contains(e.Location)))
                UpdateHoverAndTooltip(e.Location);
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

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveringThumb || _hoveringUpArrow || _hoveringDownArrow)
            {
                _hoveringThumb = _hoveringUpArrow = _hoveringDownArrow = false;
                Invalidate(_scrollbarBounds);
            }

            _hoverIndex = -1;
            _tooltip.ToolTipTitle = string.Empty;
            _tooltip.Hide(this);
            Invalidate();
        }

        private void UpdateHoverAndTooltip(Point viewPt)
        {
            int hit = HitTestContent(viewPt);
            if (hit != _hoverIndex)
            {
                _hoverIndex = hit;
                Invalidate(new Rectangle(0, 0, ContentViewportWidth, ClientSize.Height));

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

                        // Anchor near the visible position of the row (convert content - view space)
                        var anchor = entry.Bounds;
                        var viewAnchor = new Point(anchor.Left + (int)Math.Round(24 * DpiScale),
                                                   anchor.Bottom - _scrollOffset);
                        // Keep tooltip inside the content viewport width
                        int safeX = Math.Min(viewAnchor.X, Math.Max(0, ContentViewportWidth - (int)Math.Round(200 * DpiScale)));
                        _tooltip.Show(tip, this, safeX, Math.Max(0, viewAnchor.Y));
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
                Math.Max(1, _scrollbarBounds.Width - 2 * inset),
                thumbHeight);
        }

        #endregion
    }
}

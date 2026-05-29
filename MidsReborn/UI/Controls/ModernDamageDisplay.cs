using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Mids_Reborn.UI.Controls.Test;
using Mids_Reborn.UI.Theming;
using static Mids_Reborn.Core.Enums;

namespace Mids_Reborn.UI.Controls
{
    public sealed partial class ModernDamageDisplay : UserControl
    {
        #region Fields (legacy)

        private float _baseValue = 100f;
        private float _enhancedValue = 196f;

        private float _maxEnhanced = 207f;
        private float _highestBase = 200f;
        private float _highestEnhanced = 414f;

        private MDmgGraphType _graphType = MDmgGraphType.Stacked;
        private MDmgDisplayStyle _style = MDmgDisplayStyle.TextUnderGraph;
        private HorizontalAlignment _textAlign = HorizontalAlignment.Center;

        private string _text = "Smashing(120.2), Fire(6.69x1), Energy(45.32x1), Negative(47.21), Psionic(841.2) = 1060.6 (225.2)";
        private Color _textColor = Color.Black;

        private Color _bgStart = Color.Lime;
        private Color _bgEnd = Color.Yellow;
        private Color _baseStart = Color.Blue;
        private Color _baseEnd = Color.LightBlue;
        private Color _enhStart = Color.Blue;
        private Color _enhEnd = Color.Red;

        private int _paddingH = 3;
        private int _paddingV = 6;
        private int _barCornerRadius = 6;
        private int _barHeight = 18;
        private int _graphTextSpacing = 2;

        #endregion

        #region Fields (compact card)

        private const float ZeroTol = 1e-4f;

        private const int CardInset = 1;
        private const int CardPaddingX = 8;
        private const int CardPaddingY = 6;
        private const int CardTopGap = 3;
        private const int CardValueGap = 1;
        private const int CardGraphGap = 5;
        private const int CardLegendGap = 4;
        private const int CardLegendDot = 8;
        private const int CardLegendItemGap = 14;
        private const int CardLegendTextGap = 5;

        private readonly MidsToolTip _toolTip;
        private string? _lastTip;
        private bool _tooltipVisible;
        private Point _lastMouseLocation = new(int.MinValue, int.MinValue);
        private Point _lastTooltipAnchor = new(int.MinValue, int.MinValue);

        private bool _useCompactCard;
        private bool _showGraph = true;
        private DamageCardPresentation _presentation = DamageCardPresentation.Empty;

        #endregion

        #region Designer properties (legacy)

        [Category("Data")]
        [DefaultValue(100f)]
        public float BaseValue
        {
            get => _baseValue;
            set
            {
                _baseValue = value;
                Invalidate();
            }
        }

        [Category("Data")]
        [DefaultValue(196f)]
        public float EnhancedValue
        {
            get => _enhancedValue;
            set
            {
                _enhancedValue = value;
                Invalidate();
            }
        }

        [Category("Data")]
        [Description("Maximum enhanced baseline used by some scaling branches.")]
        [DefaultValue(207f)]
        public float MaxEnhancedValue
        {
            get => _maxEnhanced;
            set
            {
                _maxEnhanced = value;
                Invalidate();
            }
        }

        [Category("Data")]
        [Description("Highest observed base value for scaling the base bar.")]
        [DefaultValue(200f)]
        public float HighestBaseValue
        {
            get => _highestBase;
            set
            {
                _highestBase = value;
                Invalidate();
            }
        }

        [Category("Data")]
        [Description("Highest observed enhanced value for scaling the enhanced bar.")]
        [DefaultValue(414f)]
        public float HighestEnhancedValue
        {
            get => _highestEnhanced;
            set
            {
                _highestEnhanced = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0, 0, 0")]
        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0, 0, 255")]
        public Color BaseGradientStart
        {
            get => _baseStart;
            set
            {
                _baseStart = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "173, 216, 230")]
        public Color BaseGradientEnd
        {
            get => _baseEnd;
            set
            {
                _baseEnd = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0, 0, 255")]
        public Color EnhancedGradientStart
        {
            get => _enhStart;
            set
            {
                _enhStart = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "255, 0, 0")]
        public Color EnhancedGradientEnd
        {
            get => _enhEnd;
            set
            {
                _enhEnd = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0, 255, 0")]
        public Color BackgroundGradientStart
        {
            get => _bgStart;
            set
            {
                _bgStart = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "255, 255, 0")]
        public Color BackgroundGradientEnd
        {
            get => _bgEnd;
            set
            {
                _bgEnd = value;
                Invalidate();
            }
        }

        [Category("Layout")]
        [DefaultValue(3)]
        public int PaddingH
        {
            get => _paddingH;
            set
            {
                _paddingH = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Layout")]
        [DefaultValue(6)]
        public int PaddingV
        {
            get => _paddingV;
            set
            {
                _paddingV = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Layout")]
        [DefaultValue(18)]
        [Description("Fixed bar height when Style = TextUnderGraph.")]
        public int BarHeight
        {
            get => _barHeight;
            set
            {
                _barHeight = Math.Max(1, value);
                Invalidate();
            }
        }

        [Category("Layout")]
        [DefaultValue(2)]
        [Description("Vertical spacing between the graph (bars) and the text band when Style = TextUnderGraph.")]
        public int GraphTextSpacing
        {
            get => _graphTextSpacing;
            set
            {
                _graphTextSpacing = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(6)]
        [Description("Corner radius used for the filled bars.")]
        public int BarCornerRadius
        {
            get => _barCornerRadius;
            set
            {
                _barCornerRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment TextAlign
        {
            get => _textAlign;
            set
            {
                _textAlign = value;
                Invalidate();
            }
        }

        [Category("Behavior")]
        [DefaultValue(MDmgGraphType.Stacked)]
        public MDmgGraphType GraphType
        {
            get => _graphType;
            set
            {
                _graphType = value;
                Invalidate();
            }
        }

        [Category("Behavior")]
        [DefaultValue(MDmgDisplayStyle.TextUnderGraph)]
        public MDmgDisplayStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("The text to display for this control.")]
        public override string Text
        {
            get => _text;
            set
            {
                _text = value;
                Invalidate();
            }
        }

        #endregion

        #region Designer properties (compact card)

        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Renders the control as a compact Neo card instead of the legacy layered graph.")]
        public bool UseCompactCard
        {
            get => _useCompactCard;
            set
            {
                if (_useCompactCard == value)
                {
                    return;
                }

                _useCompactCard = value;
                ApplyTooltipTheme();
                Invalidate();
            }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Shows the compact card graph and legend when UseCompactCard is enabled.")]
        public bool ShowGraph
        {
            get => _showGraph;
            set
            {
                if (_showGraph == value)
                {
                    return;
                }

                _showGraph = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        internal DamageCardPresentation Presentation
        {
            get => _presentation;
            set
            {
                _presentation = value ?? DamageCardPresentation.Empty;
                SyncLegacyFieldsFromPresentation();
                ToolTipText = _presentation.TooltipText ?? string.Empty;
                Invalidate();
            }
        }

        #endregion

        #region Tooltip

        [Browsable(false)]
        public string ToolTipText
        {
            get => _lastTip ?? string.Empty;
            set
            {
                var tip = value ?? string.Empty;
                if (_lastTip == tip)
                {
                    return;
                }

                _lastTip = tip;
                if (string.IsNullOrWhiteSpace(tip))
                {
                    HideTooltip();
                }
                else if (_tooltipVisible)
                {
                    ShowTooltipAtPreferredAnchor(force: true);
                }
            }
        }

        public void SetTip(string tip) => ToolTipText = tip;

        #endregion

        #region Construction

        public ModernDamageDisplay()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.ResizeRedraw,
                true);

            DoubleBuffered = true;
            _toolTip = new MidsToolTip
            {
                ShowAlways = true,
                ToolTipTitle = string.Empty,
                InitialDelay = 350,
                ReshowDelay = 100,
                AutoPopDelay = 20000
            };

            InitializeComponent();
            BackColor = Color.Transparent;
            ApplyTooltipTheme();

            if (!DesignMode)
            {
                ThemeManager.ThemeChanged += ThemeManagerOnThemeChanged;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            ApplyTooltipTheme();
            Invalidate();
        }

        #endregion

        #region Public API

        public void Draw() => Invalidate();

        public void Clear()
        {
            _presentation = DamageCardPresentation.Empty;
            _baseValue = 0f;
            _enhancedValue = 0f;
            _maxEnhanced = 0f;
            _highestBase = 0f;
            _highestEnhanced = 0f;
            _text = string.Empty;
            ToolTipText = string.Empty;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _lastMouseLocation = e.Location;
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            ShowTooltipAtPreferredAnchor(force: true);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            HideTooltip();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!Visible)
            {
                HideTooltip();
            }
        }

        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_useCompactCard)
            {
                DrawCompactCard(e);
                return;
            }

            DrawLegacy(e);
        }

        private void DrawLegacy(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            var full = ClientRectangle;
            if (full.Width <= 0 || full.Height <= 0)
            {
                return;
            }

            var content = new Rectangle(
                full.X + _paddingH,
                full.Y + _paddingV,
                Math.Max(1, full.Width - _paddingH * 2),
                Math.Max(1, full.Height - _paddingV * 2));
            if (content.Width <= 0 || content.Height <= 0)
            {
                return;
            }

            Rectangle innerGraph;
            Rectangle textRect;
            switch (_style)
            {
                case MDmgDisplayStyle.TextUnderGraph:
                    {
                        var maxBarHeight = Math.Max(1, content.Height - _graphTextSpacing - 1);
                        var barHeight = Math.Min(_barHeight, maxBarHeight);
                        innerGraph = new Rectangle(content.X, content.Y, content.Width, barHeight);

                        var textY = innerGraph.Bottom + _graphTextSpacing;
                        var textHeight = Math.Max(0, content.Bottom - textY);
                        textRect = textHeight > 0
                            ? new Rectangle(content.X, textY, content.Width, textHeight)
                            : Rectangle.Empty;
                        break;
                    }

                case MDmgDisplayStyle.TextOnGraph:
                    innerGraph = content;
                    textRect = content;
                    break;

                case MDmgDisplayStyle.TextOnly:
                default:
                    innerGraph = Rectangle.Empty;
                    textRect = content;
                    break;
            }

            if (innerGraph.Width > 0 && innerGraph.Height > 0)
            {
                using var bg = new LinearGradientBrush(innerGraph, _bgStart, _bgEnd, 0f);
                g.FillRectangle(bg, innerGraph);

                var hasAnyValue = !IsExactlyZero(_baseValue) || !IsExactlyZero(_enhancedValue);
                if (hasAnyValue)
                {
                    var hiEnhanced = _highestEnhanced;
                    var hiBase = _highestBase;

                    if (NearlyZero(_maxEnhanced))
                    {
                        _maxEnhanced = Math.Max(1f, _baseValue * 2f);
                    }

                    if (NearlyZero(hiEnhanced))
                    {
                        hiEnhanced = Math.Max(1f, _baseValue * 2f);
                    }

                    if (NearlyZero(hiBase))
                    {
                        hiBase = Math.Max(1f, _baseValue * 2f);
                    }

                    var trackWidth = innerGraph.Width;
                    switch (_graphType)
                    {
                        case MDmgGraphType.Layered:
                            {
                                var baseWidth = trackWidth * Clamp01(_baseValue / hiEnhanced);
                                var enhancedWidth = trackWidth * Clamp01(_enhancedValue / hiEnhanced);
                                DrawLayered(g, innerGraph, baseWidth, enhancedWidth);
                                break;
                            }
                        case MDmgGraphType.Stacked:
                            {
                                var baseWidth = trackWidth * Clamp01(_baseValue / hiBase);
                                var enhancedWidth = trackWidth * Clamp01(_enhancedValue / hiEnhanced);
                                DrawSplit(g, innerGraph, baseWidth, enhancedWidth);
                                break;
                            }
                        case MDmgGraphType.BaseOnly:
                            DrawSingle(g, innerGraph, trackWidth * Clamp01(_baseValue / hiBase), true);
                            break;
                        case MDmgGraphType.EnhancedOnly:
                            DrawSingle(g, innerGraph, trackWidth * Clamp01(_enhancedValue / hiEnhanced), false);
                            break;
                    }
                }
            }

            if (textRect.Width > 0 && textRect.Height > 0)
            {
                DrawLegacyText(g, textRect);
            }
        }

        private void DrawCompactCard(PaintEventArgs e)
        {
            if (!_presentation.HasContent)
            {
                return;
            }

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var outer = Rectangle.Inflate(ClientRectangle, -CardInset, -CardInset);
            if (outer.Width <= 0 || outer.Height <= 0)
            {
                return;
            }

            var theme = CurrentTheme;
            using var cardPath = CreateRoundedRectPath(outer, ScalePx(10));
            using var cardFill = new LinearGradientBrush(
                outer,
                Blend(theme.Card, theme.Background, 0.12f),
                Blend(theme.Background, theme.Card, 0.28f),
                LinearGradientMode.Vertical);
            using var cardBorder = new Pen(Blend(theme.Border, theme.Text, 0.08f));

            g.FillPath(cardFill, cardPath);
            g.DrawPath(cardBorder, cardPath);

            var content = Rectangle.Inflate(outer, -ScalePx(CardPaddingX), -ScalePx(CardPaddingY));
            if (content.Width <= 0 || content.Height <= 0)
            {
                return;
            }

            var segments = _presentation.Segments.Where(segment => segment.Value > ZeroTol).ToArray();
            var drawGraph = _showGraph && segments.Length > 0;
            var hasSubtitle = !string.IsNullOrWhiteSpace(_presentation.SubtitleText);
            var hasPrimary = !string.IsNullOrWhiteSpace(_presentation.PrimaryText);
            var compactTextLayout = !hasSubtitle;
            var fullMode = content.Height >= ScalePx(hasSubtitle ? 68 : 58);
            var condensedMode = !fullMode && content.Height >= ScalePx(hasSubtitle ? 54 : 48);
            var minimalMode = !fullMode && !condensedMode;
            var widthScale = Math.Clamp(content.Width / 360f, 0.78f, 1.08f);

            var headerSize = (fullMode ? (compactTextLayout ? 7.4f : 8.0f) : condensedMode ? (compactTextLayout ? 7.0f : 7.5f) : 6.7f) * widthScale;
            var badgeSize = (fullMode ? (compactTextLayout ? 6.6f : 7.1f) : condensedMode ? (compactTextLayout ? 6.3f : 6.7f) : 6.0f) * widthScale;
            var primarySize = (fullMode ? (compactTextLayout ? 15.2f : 16.2f) : condensedMode ? (compactTextLayout ? 13.3f : 14.0f) : 11.9f) * widthScale;
            var subtitleSize = (fullMode ? 7.5f : condensedMode ? 6.8f : 6.5f) * widthScale;
            var legendSize = (fullMode ? (compactTextLayout ? 6.3f : 7.0f) : condensedMode ? (compactTextLayout ? 6.0f : 6.5f) : 5.8f) * widthScale;

            var showLegend = drawGraph;
            var showSubtitle = hasSubtitle && (!minimalMode || !drawGraph);
            var topGap = ScalePx(fullMode ? (compactTextLayout ? 1 : CardTopGap) : 1);
            var valueGap = ScalePx(fullMode ? CardValueGap : 0);
            var graphGap = ScalePx(fullMode ? (compactTextLayout ? 3 : CardGraphGap) : 2);
            var badgePaddingX = ScalePx(fullMode ? (compactTextLayout ? 10 : 14) : 8);
            var badgePaddingY = ScalePx(fullMode ? (compactTextLayout ? 3 : 6) : 2);
            var legendGap = showLegend ? ScalePx(fullMode ? 3 : 2) : 0;
            var barHeight = drawGraph ? ScalePx(fullMode ? (compactTextLayout ? 10 : 11) : 8) : 0;
            var minBarHeight = drawGraph ? ScalePx(fullMode ? 7 : 6) : 0;

            using var headerFont = CreateAutoFitFont(g, _presentation.HeaderText, FontStyle.Bold, headerSize, 6.8f, Math.Max(120, content.Width / 2));
            using var badgeFont = CreateAutoFitFont(g, _presentation.ModeBadgeText, FontStyle.Bold, badgeSize, 6.2f, Math.Max(80, content.Width / 3));
            using var primaryFont = CreateAutoFitFont(g, _presentation.PrimaryText, FontStyle.Bold, primarySize, 11.6f, content.Width);
            using var subtitleFont = CreateAutoFitFont(g, _presentation.SubtitleText, FontStyle.Regular, subtitleSize, 6.2f, content.Width);
            using var legendFont = CreateAutoFitFont(g, "Enhanced", FontStyle.Regular, legendSize, 6.2f, content.Width);

            var headerFlags = TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
            var centeredFlags = TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;

            var topRowHeight = Math.Max(
                TextRenderer.MeasureText(g, _presentation.HeaderText, headerFont, new Size(content.Width, int.MaxValue), headerFlags).Height,
                string.IsNullOrWhiteSpace(_presentation.ModeBadgeText)
                    ? 0
                    : TextRenderer.MeasureText(g, _presentation.ModeBadgeText, badgeFont, new Size(content.Width, int.MaxValue), headerFlags).Height + badgePaddingY);

            var primaryHeight = TextRenderer.MeasureText(g, _presentation.PrimaryText, primaryFont, new Size(content.Width, int.MaxValue), headerFlags).Height;
            var subtitleHeight = showSubtitle
                ? TextRenderer.MeasureText(g, _presentation.SubtitleText, subtitleFont, new Size(content.Width, int.MaxValue), headerFlags).Height
                : 0;
            var legendHeight = showLegend
                ? Math.Max(ScalePx(CardLegendDot), TextRenderer.MeasureText(g, "Enhanced", legendFont, new Size(content.Width, int.MaxValue), headerFlags).Height)
                : 0;
            var topRowRect = new Rectangle(content.Left, content.Top, content.Width, topRowHeight);

            var badgeRect = Rectangle.Empty;
            if (!string.IsNullOrWhiteSpace(_presentation.ModeBadgeText))
            {
                var badgeTextSize = TextRenderer.MeasureText(g, _presentation.ModeBadgeText, badgeFont, new Size(content.Width, int.MaxValue), headerFlags);
                badgeRect = new Rectangle(
                    Math.Max(content.Left, content.Right - badgeTextSize.Width - badgePaddingX),
                    content.Top,
                    badgeTextSize.Width + badgePaddingX,
                    Math.Max(topRowHeight, badgeTextSize.Height + badgePaddingY));
            }

            var headerRect = new Rectangle(
                content.Left,
                content.Top,
                badgeRect == Rectangle.Empty ? content.Width : Math.Max(0, badgeRect.Left - content.Left - ScalePx(8)),
                topRowHeight);

            if (!string.IsNullOrWhiteSpace(_presentation.HeaderText))
            {
                TextRenderer.DrawText(g, _presentation.HeaderText, headerFont, headerRect, theme.Text, headerFlags);
            }

            if (badgeRect != Rectangle.Empty)
            {
                DrawBadge(g, badgeRect, badgeFont, _presentation.ModeBadgeText, theme);
            }

            if (!hasPrimary)
            {
                return;
            }

            var primaryTop = topRowRect.Bottom + topGap;
            if (!drawGraph)
            {
                var textOnlyHeight = primaryHeight + (showSubtitle ? valueGap + subtitleHeight : 0);
                primaryTop += Math.Max(0, (content.Bottom - primaryTop - textOnlyHeight) / 2);
            }

            var primaryRect = new Rectangle(content.Left, primaryTop, content.Width, primaryHeight);
            Rectangle subtitleRect = Rectangle.Empty;
            if (showSubtitle)
            {
                subtitleRect = new Rectangle(content.Left, primaryRect.Bottom + valueGap, content.Width, subtitleHeight);
            }

            TextRenderer.DrawText(g, _presentation.PrimaryText, primaryFont, primaryRect, theme.Text, headerFlags);
            if (showSubtitle)
            {
                TextRenderer.DrawText(g, _presentation.SubtitleText, subtitleFont, subtitleRect, Blend(theme.Muted, theme.Text, 0.18f), headerFlags);
            }

            if (!drawGraph)
            {
                return;
            }

            var barTop = (showSubtitle ? subtitleRect.Bottom : primaryRect.Bottom) + graphGap;
            if (showSubtitle && barTop + minBarHeight + (showLegend ? legendGap + legendHeight : 0) > content.Bottom)
            {
                showSubtitle = false;
                subtitleRect = Rectangle.Empty;
                barTop = primaryRect.Bottom + Math.Max(ScalePx(1), graphGap - ScalePx(1));
            }

            if (showLegend)
            {
                var availableBarSpace = content.Bottom - barTop;
                var maxBarHeight = availableBarSpace - legendGap - legendHeight;
                if (maxBarHeight < minBarHeight)
                {
                    legendGap = ScalePx(1);
                    maxBarHeight = availableBarSpace - legendGap - legendHeight;
                }

                if (maxBarHeight < minBarHeight)
                {
                    maxBarHeight = Math.Max(ScalePx(4), maxBarHeight);
                }

                if (maxBarHeight <= 0)
                {
                    return;
                }

                barHeight = Math.Min(barHeight, maxBarHeight);
            }
            else if (barTop + barHeight > content.Bottom)
            {
                return;
            }

            var barRect = new Rectangle(content.Left, barTop, content.Width, barHeight);
            DrawCompactBar(g, barRect, segments, theme);

            if (showLegend)
            {
                var legendRect = new Rectangle(content.Left, barRect.Bottom + legendGap, content.Width, legendHeight);
                DrawCompactLegend(g, legendRect, segments, legendFont, theme, centeredFlags);
            }
        }

        #endregion

        #region Compact helpers

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

        private void ThemeManagerOnThemeChanged()
        {
            ApplyTooltipTheme();
            Invalidate();
        }

        private void ApplyTooltipTheme()
        {
            _toolTip.ToolTipTitle = string.Empty;

            var theme = CurrentTheme;
            _toolTip.BackColorTop = Blend(theme.HeaderTop, theme.Card, 0.35f);
            _toolTip.BackColorBottom = Blend(theme.HeaderBottom, theme.Background, 0.45f);
            _toolTip.BorderColor = theme.Border;
            _toolTip.TitleColor = theme.Text;
            _toolTip.TextColor = theme.Text;
            _toolTip.ContentFont = Font;
            _toolTip.MaxWidth = 520;
            if (_tooltipVisible)
            {
                ShowTooltipAtPreferredAnchor(force: true);
            }
        }

        private void ShowTooltipAtPreferredAnchor(bool force = false)
        {
            if (string.IsNullOrWhiteSpace(_lastTip) || !Visible || !Enabled)
            {
                HideTooltip();
                return;
            }

            var baseLocation = _lastMouseLocation.X == int.MinValue
                ? new Point(Math.Max(0, Width / 2), Math.Max(0, Height / 2))
                : _lastMouseLocation;

            var preferredAnchor = new Point(
                baseLocation.X + ScalePx(14),
                baseLocation.Y + ScalePx(18));

            if (!force &&
                _tooltipVisible &&
                Math.Abs(preferredAnchor.X - _lastTooltipAnchor.X) < ScalePx(6) &&
                Math.Abs(preferredAnchor.Y - _lastTooltipAnchor.Y) < ScalePx(6))
            {
                return;
            }

            _toolTip.ShowClamped(this, _lastTip!, preferredAnchor, _toolTip.AutoPopDelay);
            _tooltipVisible = true;
            _lastTooltipAnchor = preferredAnchor;
        }

        private void HideTooltip()
        {
            _toolTip.Hide(this);
            _tooltipVisible = false;
            _lastTooltipAnchor = new Point(int.MinValue, int.MinValue);
        }

        private void SyncLegacyFieldsFromPresentation()
        {
            _text = _presentation.PrimaryText ?? string.Empty;

            var baseSegment = _presentation.Segments.FirstOrDefault(segment => segment.Kind == DamageSourceSegmentKind.Base);
            var enhancedSegment = _presentation.Segments.FirstOrDefault(segment => segment.Kind == DamageSourceSegmentKind.Enhanced);
            var procSegment = _presentation.Segments.FirstOrDefault(segment => segment.Kind == DamageSourceSegmentKind.Proc);

            _baseValue = Math.Max(0f, baseSegment?.Value ?? 0f);
            _enhancedValue = Math.Max(0f, _baseValue + (enhancedSegment?.Value ?? 0f) + (procSegment?.Value ?? 0f));
            _maxEnhanced = _enhancedValue;
        }

        private void DrawBadge(Graphics graphics, Rectangle bounds, Font font, string text, DataViewTheme theme)
        {
            using var path = CreateRoundedRectPath(bounds, Math.Max(1, bounds.Height / 2));
            using var fill = new LinearGradientBrush(bounds, theme.HeaderTop, theme.HeaderBottom, LinearGradientMode.Vertical);
            using var border = new Pen(Blend(theme.Border, theme.Accent, 0.28f));

            graphics.FillPath(fill, path);
            graphics.DrawPath(border, path);

            TextRenderer.DrawText(graphics, text, font, bounds, theme.Text,
                TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis |
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void DrawCompactBar(Graphics graphics, Rectangle bounds, IReadOnlyList<DamageSourceSegment> segments, DataViewTheme theme)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            using var trackPath = CreateRoundedRectPath(bounds, Math.Max(1, bounds.Height / 2));
            using var trackFill = new SolidBrush(Blend(theme.Rail, theme.Background, 0.28f));
            using var trackBorder = new Pen(Blend(theme.Border, theme.Background, 0.18f));

            graphics.FillPath(trackFill, trackPath);
            graphics.DrawPath(trackBorder, trackPath);

            var total = Math.Max(ZeroTol, segments.Sum(segment => segment.Value));
            var previousClip = graphics.Clip;
            graphics.SetClip(trackPath);

            var startX = bounds.Left;
            var drawnWidth = 0;
            for (var index = 0; index < segments.Count; index++)
            {
                var segment = segments[index];
                var width = index == segments.Count - 1
                    ? bounds.Width - drawnWidth
                    : Math.Max(0, (int)Math.Round(bounds.Width * (segment.Value / total)));
                if (width <= 0)
                {
                    continue;
                }

                var segmentRect = new Rectangle(startX, bounds.Top, width, bounds.Height);
                var colors = GetCompactSegmentColors(segment.Kind);
                using var fill = new LinearGradientBrush(segmentRect, colors.Start, colors.End, LinearGradientMode.Horizontal);
                graphics.FillRectangle(fill, segmentRect);

                startX += width;
                drawnWidth += width;
            }

            graphics.Clip = previousClip;
        }

        private void DrawCompactLegend(Graphics graphics, Rectangle bounds, IReadOnlyList<DamageSourceSegment> segments, Font font, DataViewTheme theme, TextFormatFlags textFlags)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            var dotSize = ScalePx(CardLegendDot);
            var totalWidth = 0;
            var measured = new List<(DamageSourceSegment Segment, int LabelWidth)>(segments.Count);
            foreach (var segment in segments)
            {
                var labelWidth = TextRenderer.MeasureText(graphics, segment.Label, font, new Size(int.MaxValue, bounds.Height), textFlags).Width;
                measured.Add((segment, labelWidth));
                totalWidth += dotSize + ScalePx(CardLegendTextGap) + labelWidth;
            }

            totalWidth += Math.Max(0, measured.Count - 1) * ScalePx(CardLegendItemGap);

            var x = bounds.Left + Math.Max(0, (bounds.Width - totalWidth) / 2);
            foreach (var item in measured)
            {
                var y = bounds.Top + Math.Max(0, (bounds.Height - dotSize) / 2);
                var dotRect = new Rectangle(x, y, dotSize, dotSize);
                var colors = GetCompactSegmentColors(item.Segment.Kind);
                using var fill = new LinearGradientBrush(dotRect, colors.Start, colors.End, LinearGradientMode.Vertical);
                using var border = new Pen(Blend(theme.Border, theme.Background, 0.18f));

                graphics.FillEllipse(fill, dotRect);
                graphics.DrawEllipse(border, dotRect);

                var textRect = new Rectangle(dotRect.Right + ScalePx(CardLegendTextGap), bounds.Top, item.LabelWidth, bounds.Height);
                TextRenderer.DrawText(graphics, item.Segment.Label, font, textRect, Blend(theme.Muted, theme.Text, 0.12f),
                    textFlags | TextFormatFlags.Left);

                x = textRect.Right + ScalePx(CardLegendItemGap);
            }
        }

        private static (Color Start, Color End) GetCompactSegmentColors(DamageSourceSegmentKind kind)
        {
            return kind switch
            {
                DamageSourceSegmentKind.Base => (Color.FromArgb(170, 54, 40), Color.FromArgb(194, 74, 58)),
                DamageSourceSegmentKind.Enhanced => (Color.FromArgb(209, 101, 30), Color.FromArgb(229, 126, 37)),
                DamageSourceSegmentKind.Proc => (Color.FromArgb(230, 177, 56), Color.FromArgb(242, 201, 76)),
                _ => (Color.SlateBlue, Color.SlateBlue)
            };
        }

        private Font CreateAutoFitFont(Graphics graphics, string text, FontStyle style, float preferredSize, float minSize, int maxWidth)
        {
            var safeText = string.IsNullOrWhiteSpace(text) ? " " : text;
            for (var size = preferredSize; size >= minSize; size -= 0.35f)
            {
                var candidate = new Font(Font.FontFamily, size, style, GraphicsUnit.Point);
                var measured = TextRenderer.MeasureText(graphics, safeText, candidate,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
                if (measured.Width <= maxWidth)
                {
                    return candidate;
                }

                candidate.Dispose();
            }

            return new Font(Font.FontFamily, minSize, style, GraphicsUnit.Point);
        }

        private int ScalePx(int value)
        {
            return Math.Max(1, (int)Math.Round(value * DeviceDpi / 96f));
        }

        #endregion

        #region Legacy drawing helpers

        private void DrawLayered(Graphics g, Rectangle bounds, float baseWidth, float enhancedWidth)
        {
            var baseRect = new Rectangle(bounds.X, bounds.Y, RoundW(baseWidth), bounds.Height);
            var enhancedRect = new Rectangle(bounds.X, bounds.Y, RoundW(enhancedWidth), bounds.Height);

            if (enhancedRect.Width >= baseRect.Width)
            {
                FillRoundedBar(g, enhancedRect, _enhStart, _enhEnd, _barCornerRadius);
                FillStandardBar(g, baseRect, _baseStart, _baseEnd);
            }
            else
            {
                FillRoundedBar(g, baseRect, _baseStart, _baseEnd, _barCornerRadius);
                FillStandardBar(g, enhancedRect, _enhStart, _enhEnd);
            }
        }

        private void DrawSplit(Graphics g, Rectangle bounds, float baseWidth, float enhancedWidth)
        {
            var half = Math.Max(1, bounds.Height / 2);

            var baseRect = new Rectangle(bounds.X, bounds.Y, RoundW(baseWidth), half);
            var enhancedRect = new Rectangle(bounds.X, bounds.Y + half, RoundW(enhancedWidth), bounds.Height - half);

            FillRoundedBar(g, baseRect, _baseStart, _baseEnd, _barCornerRadius);
            FillRoundedBar(g, enhancedRect, _enhStart, _enhEnd, _barCornerRadius);
        }

        private void DrawSingle(Graphics g, Rectangle bounds, float width, bool isBase)
        {
            var rect = new Rectangle(bounds.X, bounds.Y, RoundW(width), bounds.Height);
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            if (isBase)
            {
                FillRoundedBar(g, rect, _baseStart, _baseEnd, _barCornerRadius);
            }
            else
            {
                FillRoundedBar(g, rect, _enhStart, _enhEnd, _barCornerRadius);
            }
        }

        private static int RoundW(float width) => Math.Max(0, (int)Math.Round(width));

        private static void FillStandardBar(Graphics g, Rectangle rect, Color startColor, Color endColor)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            using var brush = new LinearGradientBrush(rect, startColor, endColor, 0f);
            g.FillRectangle(brush, rect);
        }

        private static void FillRoundedBar(Graphics g, Rectangle rect, Color startColor, Color endColor, int radius)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            var actualRadius = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            using var brush = new LinearGradientBrush(rect, startColor, endColor, 0f);

            if (actualRadius <= 0)
            {
                g.FillRectangle(brush, rect);
                return;
            }

            using var path = CreateRoundedRectPath(rect, actualRadius);
            g.FillPath(brush, path);
        }

        private static bool NearlyZero(float value) => Math.Abs(value) < ZeroTol;
        private static bool IsExactlyZero(float value) => value == 0f;
        private static float Clamp01(float value) => value < 0f ? 0f : (value > 1f ? 1f : value);

        #endregion

        #region Text rendering

        private void DrawLegacyText(Graphics g, Rectangle bounds)
        {
            if (string.IsNullOrEmpty(_text) || bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            var flags = _textAlign switch
            {
                HorizontalAlignment.Left => TextFormatFlags.Left,
                HorizontalAlignment.Center => TextFormatFlags.HorizontalCenter,
                HorizontalAlignment.Right => TextFormatFlags.Right,
                _ => TextFormatFlags.HorizontalCenter
            };

            flags |= TextFormatFlags.NoPrefix
                     | TextFormatFlags.WordBreak
                     | TextFormatFlags.TextBoxControl
                     | TextFormatFlags.NoPadding;

            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            TextRenderer.DrawText(g, _text, Font, bounds, _textColor, Color.Transparent, flags);
        }

        #endregion

        #region Shared helpers

        private static GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                path.CloseFigure();
                return path;
            }

            var diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.StartFigure();
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color Blend(Color first, Color second, float amountSecond)
        {
            amountSecond = Math.Clamp(amountSecond, 0f, 1f);
            var amountFirst = 1f - amountSecond;

            return Color.FromArgb(
                (int)Math.Round(first.A * amountFirst + second.A * amountSecond),
                (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
                (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
                (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
        }

        #endregion
    }
}

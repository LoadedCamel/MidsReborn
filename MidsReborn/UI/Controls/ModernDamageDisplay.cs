using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using static Mids_Reborn.Core.Enums;

namespace Mids_Reborn.UI.Controls
{
    public sealed partial class ModernDamageDisplay : UserControl
    {
        #region Fields (data/state)

        private float _baseValue = 100f;
        private float _enhancedValue = 196f;

        // Scaling tracks
        private float _maxEnhanced = 207f;
        private float _highestBase = 200f;
        private float _highestEnhanced = 414f;

        // Defaults
        private MDmgGraphType _graphType = MDmgGraphType.Stacked;
        private MDmgDisplayStyle _style = MDmgDisplayStyle.TextUnderGraph;
        private HorizontalAlignment _textAlign = HorizontalAlignment.Center;

        private string _text = "Smashing(120.2), Fire(6.69x1), Energy(45.32x1), Negative(47.21), Psionic(841.2) = 1060.6 (225.2)";
        private Color _textColor = Color.Black;

        // Gradients
        private Color _bgStart = Color.Lime;
        private Color _bgEnd = Color.Yellow;
        private Color _baseStart = Color.Blue;
        private Color _baseEnd = Color.LightBlue;
        private Color _enhStart = Color.Blue;
        private Color _enhEnd = Color.Red;

        // Layout knobs
        private int _paddingH = 3;
        private int _paddingV = 6;            // vertical interior padding
        private int _barCornerRadius = 6;

        // Fixed bar height & gap between graph and text
        private int _barHeight = 18;
        private int _graphTextSpacing = 2;

        // Tooltip support (instantiated in InitializeComponent)
        private ToolTip _toolTip = null!;
        private string? _lastTip;

        private const float ZeroTol = 1e-4f;

        #endregion

        #region Designer properties

        [Category("Data")]
        [DefaultValue(100f)]
        public float BaseValue
        {
            get => _baseValue;
            set { _baseValue = value; Invalidate(); }
        }

        [Category("Data")]
        [DefaultValue(196f)]
        public float EnhancedValue
        {
            get => _enhancedValue;
            set { _enhancedValue = value; Invalidate(); }
        }

        [Category("Data")]
        [Description("Maximum enhanced baseline used by some scaling branches.")]
        [DefaultValue(207f)]
        public float MaxEnhancedValue
        {
            get => _maxEnhanced;
            set { _maxEnhanced = value; Invalidate(); }
        }

        [Category("Data")]
        [Description("Highest observed base value for scaling the base bar.")]
        [DefaultValue(200f)]
        public float HighestBaseValue
        {
            get => _highestBase;
            set { _highestBase = value; Invalidate(); }
        }

        [Category("Data")]
        [Description("Highest observed enhanced value for scaling the enhanced bar.")]
        [DefaultValue(414f)]
        public float HighestEnhancedValue
        {
            get => _highestEnhanced;
            set { _highestEnhanced = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0, 0, 0")]
        public Color TextColor
        {
            get => _textColor;
            set { _textColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0, 0, 255")]
        public Color BaseGradientStart
        {
            get => _baseStart;
            set { _baseStart = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "173, 216, 230")]
        public Color BaseGradientEnd
        {
            get => _baseEnd;
            set { _baseEnd = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0, 0, 255")]
        public Color EnhancedGradientStart
        {
            get => _enhStart;
            set { _enhStart = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "255, 0, 0")]
        public Color EnhancedGradientEnd
        {
            get => _enhEnd;
            set { _enhEnd = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0, 255, 0")]
        public Color BackgroundGradientStart
        {
            get => _bgStart;
            set { _bgStart = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "255, 255, 0")]
        public Color BackgroundGradientEnd
        {
            get => _bgEnd;
            set { _bgEnd = value; Invalidate(); }
        }

        [Category("Layout")]
        [DefaultValue(3)]
        public int PaddingH
        {
            get => _paddingH;
            set { _paddingH = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout")]
        [DefaultValue(6)]
        public int PaddingV
        {
            get => _paddingV;
            set { _paddingV = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout")]
        [DefaultValue(18)]
        [Description("Fixed bar height when Style = TextUnderGraph.")]
        public int BarHeight
        {
            get => _barHeight;
            set { _barHeight = Math.Max(1, value); Invalidate(); }
        }

        [Category("Layout")]
        [DefaultValue(2)]
        [Description("Vertical spacing between the graph (bars) and the text band when Style = TextUnderGraph.")]
        public int GraphTextSpacing
        {
            get => _graphTextSpacing;
            set { _graphTextSpacing = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(6)]
        [Description("Corner radius used for the filled bars.")]
        public int BarCornerRadius
        {
            get => _barCornerRadius;
            set { _barCornerRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment TextAlign
        {
            get => _textAlign;
            set { _textAlign = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(MDmgGraphType.Stacked)]
        public MDmgGraphType GraphType
        {
            get => _graphType;
            set { _graphType = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(MDmgDisplayStyle.TextUnderGraph)]
        public MDmgDisplayStyle Style
        {
            get => _style;
            set { _style = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The text to display for this control.")]
        public override string Text
        {
            get => _text;
            set { _text = value; Invalidate(); }
        }

        #endregion

        #region Tooltip

        [Browsable(false)]
        public string ToolTipText
        {
            get => _lastTip ?? string.Empty;
            set
            {
                if (_lastTip == value) return;
                _lastTip = value;
                if (IsHandleCreated)
                    _toolTip.SetToolTip(this, value);
            }
        }

        public void SetTip(string tip) => ToolTipText = tip;

        #endregion

        #region Construction

        public ModernDamageDisplay()
        {
            BackColor = Color.Transparent;
            InitializeComponent();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.ResizeRedraw,
                true);

            _toolTip = new ToolTip
            {
                AutoPopDelay = 20000,
                InitialDelay = 350,
                ReshowDelay = 100
            };
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!string.IsNullOrEmpty(_lastTip))
                _toolTip.SetToolTip(this, _lastTip);
        }

        #endregion

        #region Public API

        public void Draw() => Invalidate();

        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            var full = ClientRectangle;
            if (full.Width <= 0 || full.Height <= 0) return;

            // One padded content rect for everything
            var content = new Rectangle(
                x: full.X + _paddingH,
                y: full.Y + _paddingV,
                width: Math.Max(1, full.Width - _paddingH * 2),
                height: Math.Max(1, full.Height - _paddingV * 2)
            );
            if (content.Width <= 0 || content.Height <= 0) return;

            Rectangle innerGraph = Rectangle.Empty;
            Rectangle textRect = Rectangle.Empty;

            switch (_style)
            {
                case MDmgDisplayStyle.TextUnderGraph:
                    {
                        int maxBarH = Math.Max(1, content.Height - _graphTextSpacing - 1);
                        int barH = Math.Min(_barHeight, maxBarH);

                        innerGraph = new Rectangle(content.X, content.Y, content.Width, barH);

                        int textY = innerGraph.Bottom + _graphTextSpacing;
                        int textH = Math.Max(0, content.Bottom - textY);
                        textRect = textH > 0
                            ? new Rectangle(content.X, textY, content.Width, textH)
                            : Rectangle.Empty;
                        break;
                    }

                case MDmgDisplayStyle.TextOnGraph:
                    {
                        innerGraph = content;
                        textRect = content;
                        break;
                    }

                case MDmgDisplayStyle.TextOnly:
                default:
                    {
                        innerGraph = Rectangle.Empty;
                        textRect = content;
                        break;
                    }
            }

            // Background behind the graph region
            if (innerGraph.Width > 0 && innerGraph.Height > 0)
            {
                using var bg = new LinearGradientBrush(innerGraph, _bgStart, _bgEnd, 0f);
                g.FillRectangle(bg, innerGraph);

                // Bars
                bool hasAnyValue = !IsExactlyZero(_baseValue) || !IsExactlyZero(_enhancedValue);
                if (hasAnyValue)
                {
                    float hiEnhanced = _highestEnhanced;
                    float hiBase = _highestBase;

                    if (NearlyZero(_maxEnhanced)) _maxEnhanced = Math.Max(1f, _baseValue * 2f);
                    if (NearlyZero(hiEnhanced)) hiEnhanced = Math.Max(1f, _baseValue * 2f);
                    if (NearlyZero(hiBase)) hiBase = Math.Max(1f, _baseValue * 2f);

                    float trackWidth = innerGraph.Width;

                    switch (_graphType)
                    {
                        case MDmgGraphType.Layered:
                            {
                                float baseW = trackWidth * Clamp01(_baseValue / hiEnhanced);
                                float enhW = trackWidth * Clamp01(_enhancedValue / hiEnhanced);
                                DrawLayered(g, innerGraph, baseW, enhW);
                                break;
                            }
                        case MDmgGraphType.Stacked:
                            {
                                float baseW = trackWidth * Clamp01(_baseValue / hiBase);
                                float enhW = trackWidth * Clamp01(_enhancedValue / hiEnhanced);
                                DrawSplit(g, innerGraph, baseW, enhW);
                                break;
                            }
                        case MDmgGraphType.BaseOnly:
                            {
                                float w = trackWidth * Clamp01(_baseValue / hiBase);
                                DrawSingle(g, innerGraph, w, isBase: true);
                                break;
                            }
                        case MDmgGraphType.EnhancedOnly:
                            {
                                float w = trackWidth * Clamp01(_enhancedValue / hiEnhanced);
                                DrawSingle(g, innerGraph, w, isBase: false);
                                break;
                            }
                    }
                }
            }

            // Text
            if (textRect.Width > 0 && textRect.Height > 0)
                DrawText(g, textRect);
        }

        #endregion

        #region Drawing helpers

        private void DrawLayered(Graphics g, Rectangle bounds, float baseWidth, float enhWidth)
        {
            Rectangle baseRect = new(bounds.X, bounds.Y, RoundW(baseWidth), bounds.Height);
            Rectangle enhRect = new(bounds.X, bounds.Y, RoundW(enhWidth), bounds.Height);

            if (enhRect.Width >= baseRect.Width)
            {
                FillRoundedBar(g, enhRect, _enhStart, _enhEnd, _barCornerRadius);
                FillStandardBar(g, baseRect, _baseStart, _baseEnd);
            }
            else
            {
                FillRoundedBar(g, baseRect, _baseStart, _baseEnd, _barCornerRadius);
                FillStandardBar(g, enhRect, _enhStart, _enhEnd);
            }
        }

        private void DrawSplit(Graphics g, Rectangle bounds, float baseWidth, float enhWidth)
        {
            int half = Math.Max(1, bounds.Height / 2);

            Rectangle baseRect = new(bounds.X, bounds.Y, RoundW(baseWidth), half);
            Rectangle enhRect = new(bounds.X, bounds.Y + half, RoundW(enhWidth), bounds.Height - half);

            FillRoundedBar(g, baseRect, _baseStart, _baseEnd, _barCornerRadius);
            FillRoundedBar(g, enhRect, _enhStart, _enhEnd, _barCornerRadius);
        }

        private void DrawSingle(Graphics g, Rectangle bounds, float width, bool isBase)
        {
            Rectangle rect = new(bounds.X, bounds.Y, RoundW(width), bounds.Height);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            if (isBase) FillRoundedBar(g, rect, _baseStart, _baseEnd, _barCornerRadius);
            else FillRoundedBar(g, rect, _enhStart, _enhEnd, _barCornerRadius);
        }

        private static int RoundW(float w) => Math.Max(0, (int)Math.Round(w));

        private static void FillStandardBar(Graphics g, Rectangle rect, Color c1, Color c2)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            using var brush = new LinearGradientBrush(rect, c1, c2, 0f);
            g.FillRectangle(brush, rect);
        }

        private void FillRoundedBar(Graphics g, Rectangle rect, Color c1, Color c2, int radius)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            int r = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            using var brush = new LinearGradientBrush(rect, c1, c2, 0f);

            if (r <= 0)
            {
                g.FillRectangle(brush, rect);
                return;
            }

            using var path = CreateRoundedRectPath(rect, r);
            g.FillPath(brush, path);
        }

        private static GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            int l = rect.Left, t = rect.Top, r = rect.Right - 1, b = rect.Bottom - 1;

            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(Rectangle.FromLTRB(l, t, r + 1, b + 1));
                path.CloseFigure();
                return path;
            }

            int rr = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);

            path.StartFigure();
            path.AddLine(l, t, r - rr + 1, t);                                   // top
            path.AddArc(r - rr * 2 + 1, t, rr * 2, rr * 2, 270, 90);             // TR
            path.AddLine(r + 1, t + rr, r + 1, b - rr + 1);                       // right
            path.AddArc(r - rr * 2 + 1, b - rr * 2 + 1, rr * 2, rr * 2, 0, 90);   // BR
            path.AddLine(r - rr + 1, b + 1, l, b + 1);                            // bottom
            path.AddLine(l, b + 1, l, t);                                         // left
            path.CloseFigure();
            return path;
        }

        private static bool NearlyZero(float v) => Math.Abs(v) < ZeroTol;
        private static bool IsExactlyZero(float v) => v == 0f;
        private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);

        #endregion

        #region Text rendering

        private void DrawText(Graphics g, Rectangle bounds)
        {
            if (string.IsNullOrEmpty(_text) || bounds.Width <= 0 || bounds.Height <= 0) return;

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
    }
}

using System.ComponentModel;
using System.Drawing.Drawing2D;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls
{
    public sealed partial class MidsPopupDisplay : UserControl
    {
        #region Nested types

        public enum PlacementBias
        {
            Auto,
            PreferBelow,
            PreferAbove,
            PreferLeft,
            PreferRight,
            Point // anchor.X, anchor.Y treated as a point (no size)
        }

        #endregion

        #region Fields

        // Rendering flags to emulate: Top/Left, no clipping to rect (like StringFormatFlags.NoClip)
        private const TextFormatFlags BaseFlags =
            TextFormatFlags.Top |
            TextFormatFlags.Left |
            TextFormatFlags.NoClipping;

        private const TextFormatFlags MeasureTight =
            TextFormatFlags.Top | TextFormatFlags.Left | TextFormatFlags.NoClipping |
            TextFormatFlags.TextBoxControl;

        private const int ColumnGap = 8;
        private const int MinimumReadableWidth = 300;
        private const int MaximumReadableWidth = 560;
        private const float MaxClientWidthFraction = 0.65f;
        private const int ChromePadding = 5;
        private const int BorderWidth = 2;
        private const int CornerRadius = 7;
        private const float MinimumContentScale = 0.95f;
        private const float MaximumContentScale = 1.18f;

        private PopUp.PopupData _popupData;
        private I9Picker.EnhUniqueStatus? _enhUniqueStatus;

        private bool _wrapColumnsThisLayout;
        private float _columnPosition = 0.5f; // 0..1; relative to content width
        private bool _columnRight;
        private int _internalPadding = 3;
        private int _sectionPadding = 8;
        private int _scrollY;
        private float _contentScale = 1f;
        private Font? _baseFont;
        private Font? _scaledFont;
        private bool _applyingScaledFont;

        #endregion

        #region Public state

        public int EIdx = -1;
        public int HIdx = -1;
        public int PIdx = -1;
        public int PsIdx = -1;

        #endregion

        #region Events

        public event EventHandler? Opened;
        public event EventHandler? Closed;

        #endregion

        #region Constructor

        public MidsPopupDisplay()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.LightYellow;
            ForeColor = Color.Black;
            BackColorChanged += (_, _) => Invalidate();
            ForeColorChanged += (_, _) => Invalidate();
            FontChanged += (_, _) =>
            {
                CaptureBaseFont();
                Invalidate();
            };
            SizeChanged += (_, _) => UpdateRoundedRegion();

            if (!DesignMode)
            {
                ThemeManager.ThemeChanged += ThemeManagerOnThemeChanged;
                ApplyTheme();
            }
        }

        #endregion

        #region Properties

        public bool HasContent { get; private set; }
        public bool IsOpen => Visible && HasContent;

        private DataViewTheme CurrentTheme =>
            DesignMode
                ? ThemeManager.DesignTime.DataView
                : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

        private float DpiScale => DeviceDpi / 96f;
        private float EffectiveScale => DpiScale * _contentScale;
        private int ColumnGapPx => ScalePx(ColumnGap);
        private int MinimumReadableWidthPx => ScalePx(MinimumReadableWidth);
        private int MaximumReadableWidthPx => ScalePx(MaximumReadableWidth);
        private int ChromePaddingPx => ScalePx(ChromePadding);
        private int BorderWidthPx => Math.Max(1, ScalePx(BorderWidth));
        private int CornerRadiusPx => Math.Max(1, ScalePx(CornerRadius));
        private int InternalPaddingPx => ScalePx(_internalPadding);
        private int SectionPaddingPx => ScalePx(_sectionPadding);
        private int ContentInset => Math.Max(0, ChromePaddingPx + BorderWidthPx);

        [Browsable(true)]
        [DefaultValue(0)]
        public int ScrollY
        {
            get => _scrollY;
            set
            {
                if (_scrollY == value) return;
                _scrollY = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DefaultValue(0.5f)]
        public float ColumnPosition
        {
            get => _columnPosition;
            set
            {
                var v = Math.Max(0f, Math.Min(1f, value));
                if (Math.Abs(v - _columnPosition) < float.Epsilon) return;
                _columnPosition = v;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DefaultValue(1f)]
        public float ContentScale
        {
            get => _contentScale;
            set
            {
                var clamped = Math.Clamp(value, MinimumContentScale, MaximumContentScale);
                if (Math.Abs(clamped - _contentScale) < 0.001f) return;

                _contentScale = clamped;
                ApplyScaledFont();
                UpdateRoundedRegion();
                Invalidate();
            }
        }

        [Browsable(true)]
        [DefaultValue(false)]
        public bool ColumnRight
        {
            get => _columnRight;
            set
            {
                if (_columnRight == value) return;
                _columnRight = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DefaultValue(8)]
        public int SectionPadding
        {
            get => _sectionPadding;
            set
            {
                if (_sectionPadding == value) return;
                _sectionPadding = Math.Max(0, value);
                Invalidate();
            }
        }

        [Browsable(true)]
        [DefaultValue(3)]
        public int InternalPadding
        {
            get => _internalPadding;
            set
            {
                if (_internalPadding == value) return;
                _internalPadding = Math.Max(0, value);
                Invalidate();
            }
        }

        #endregion

        #region Public API

        public void SetPopup(PopUp.PopupData data, I9Picker.EnhUniqueStatus? status = null)
        {
            _popupData = data;
            _enhUniqueStatus = status;
            HasContent = data.Sections is { Length: > 0 } &&
                         Array.Exists(data.Sections, s => s.Content is { Length: > 0 });
            Invalidate();
        }

        /// <summary>
        /// Convenience wrapper that only shows when there is content, and reports success.
        /// </summary>
        public bool TryShowAt(Rectangle anchor, PlacementBias bias = PlacementBias.Auto, int margin = 8)
        {
            if (!HasContent)
            {
                Visible = false;
                return false;
            }
            ShowAt(anchor, bias, margin);
            return Visible; // ShowAt clamps/sets Visible = true when it can place.
        }

        public void HidePopup()
        {
            Visible = false;
            EIdx = HIdx = PIdx = PsIdx = -1;
        }

        #endregion

        #region Private API

        private void ShowAt(Rectangle anchor, PlacementBias bias = PlacementBias.Auto, int margin = 8)
        {
            // 1) Initial natural measurement (no column wrapping)
            using (var g = CreateGraphics())
            {
                var desired = MeasureContent(g, maxWidth: null);
                Width = desired.Width;
                Height = desired.Height;
            }

            var host = FindForm();
            if (host == null)
            {
                Location = new Point(anchor.Right + margin, anchor.Top);
                Visible = true;
                BringToFront();
                return;
            }

            var client = host.ClientRectangle;

            if (bias == PlacementBias.Point)
                anchor = new Rectangle(anchor.X, anchor.Y, 1, 1);

            var popupWidthCap = GetPopupWidthCap(client, margin);
            if (Width > popupWidthCap)
            {
                EnsureMeasuredToFit(popupWidthCap);
            }

            // 2) Auto-pick side with most space
            if (bias == PlacementBias.Auto)
            {
                int below = client.Bottom - (anchor.Bottom + margin);
                int above = (anchor.Top - margin) - client.Top;
                int right = client.Right - (anchor.Right + margin);
                int left = (anchor.Left - margin) - client.Left;

                int best = Math.Max(Math.Max(below, above), Math.Max(right, left));
                bias = best == below ? PlacementBias.PreferBelow
                     : best == above ? PlacementBias.PreferAbove
                     : best == right ? PlacementBias.PreferRight
                     : PlacementBias.PreferLeft;
            }

            // 3) If too wide for the chosen side, re-measure with a width cap
            var avail = GetAvailableWidth(client, anchor, bias, margin);
            if (Width > avail) EnsureMeasuredToFit(GetWidthCap(client, Math.Min(avail, popupWidthCap), margin));

            // 4) Compute preferred location using SAFE clamping (never throws)
            Point Preferred(Rectangle c, Rectangle a, PlacementBias b, int m, int w, int h)
            {
                return b switch
                {
                    PlacementBias.PreferBelow => new Point(
                        ClampSafe(a.Left, c.Left + m, c.Right - w - m),
                        a.Bottom + m),
                    PlacementBias.PreferAbove => new Point(
                        ClampSafe(a.Left, c.Left + m, c.Right - w - m),
                        a.Top - h - m),
                    PlacementBias.PreferRight => new Point(
                        a.Right + m,
                        ClampSafe(a.Top, c.Top + m, c.Bottom - h - m)),
                    PlacementBias.PreferLeft => new Point(
                        a.Left - w - m,
                        ClampSafe(a.Top, c.Top + m, c.Bottom - h - m)),
                    _ => new Point(
                        ClampSafe(a.Left, c.Left + m, c.Right - w - m),
                        a.Bottom + m)
                };
            }

            var loc = Preferred(client, anchor, bias, margin, Width, Height);

            // 5) Auto-flip if off-screen, and re-measure for the flipped side as needed
            bool OffscreenX(Point p) => p.X < client.Left + margin || p.X + Width > client.Right - margin;
            bool OffscreenY(Point p) => p.Y < client.Top + margin || p.Y + Height > client.Bottom - margin;

            var flipped = bias;
            if ((bias is PlacementBias.PreferRight or PlacementBias.PreferLeft) && OffscreenX(loc))
                flipped = (bias == PlacementBias.PreferRight) ? PlacementBias.PreferLeft : PlacementBias.PreferRight;
            else if ((bias is PlacementBias.PreferBelow or PlacementBias.PreferAbove) && OffscreenY(loc))
                flipped = (bias == PlacementBias.PreferBelow) ? PlacementBias.PreferAbove : PlacementBias.PreferBelow;

            if (flipped != bias)
            {
                var availFlip = GetAvailableWidth(client, anchor, flipped, margin);
                if (Width > availFlip) EnsureMeasuredToFit(GetWidthCap(client, Math.Min(availFlip, popupWidthCap), margin));
                loc = Preferred(client, anchor, flipped, margin, Width, Height);
                bias = flipped;
            }

            // 6) Final SAFE clamp (handles extreme small windows/margins)
            loc.X = ClampSafe(loc.X, client.Left + margin, client.Right - Width - margin);
            loc.Y = ClampSafe(loc.Y, client.Top + margin, client.Bottom - Height - margin);

            Location = loc;
            Visible = true;
            BringToFront();
            host.Invalidate();
        }

        #endregion

        #region Overrides

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _popupData = default;
            _popupData.Init();
            ApplyTheme();
            UpdateRoundedRegion();
            Invalidate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible) Opened?.Invoke(this, EventArgs.Empty);
            else Closed?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            var theme = CurrentTheme;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Parent?.BackColor ?? theme.Background);

            var shellBounds = ClientRectangle;
            shellBounds.Width -= 1;
            shellBounds.Height -= 1;
            if (shellBounds.Width <= 0 || shellBounds.Height <= 0)
            {
                return;
            }

            using var shellPath = RoundedRect(shellBounds, CornerRadiusPx);
            using var fill = new SolidBrush(theme.Background);
            using var border = new Pen(Blend(theme.Border, theme.Accent, 0.35f), BorderWidthPx);
            using var innerBorder = new Pen(Color.FromArgb(120, theme.GridHeaderBorder));

            g.FillPath(fill, shellPath);
            g.DrawPath(border, shellPath);

            var innerBounds = Rectangle.Inflate(shellBounds, -BorderWidthPx, -BorderWidthPx);
            if (innerBounds.Width > 0 && innerBounds.Height > 0)
            {
                using var innerPath = RoundedRect(innerBounds, Math.Max(1, CornerRadiusPx - BorderWidthPx));
                g.DrawPath(innerBorder, innerPath);
            }

            DrawContent(g);
        }

        #endregion

        #region Core drawing

        private void ThemeManagerOnThemeChanged()
        {
            ApplyTheme();
            Invalidate();
        }

        private void ApplyTheme()
        {
            var theme = CurrentTheme;
            BackColor = theme.Background;
            ForeColor = Blend(theme.Border, theme.Accent, 0.35f);
        }

        private int ScalePx(int logicalPixels)
        {
            if (logicalPixels <= 0)
            {
                return 0;
            }

            return Math.Max(1, (int)Math.Round(logicalPixels * EffectiveScale));
        }

        private void CaptureBaseFont()
        {
            if (_applyingScaledFont)
            {
                return;
            }

            _baseFont?.Dispose();
            _baseFont = (Font)Font.Clone();
        }

        private void ApplyScaledFont()
        {
            var baseFont = _baseFont ?? Font;
            var scaledSize = Math.Max(6f, baseFont.Size * _contentScale);
            var nextFont = new Font(baseFont.FontFamily, scaledSize, baseFont.Style, baseFont.Unit,
                baseFont.GdiCharSet, baseFont.GdiVerticalFont);
            var previousScaledFont = _scaledFont;

            _applyingScaledFont = true;
            try
            {
                _scaledFont = nextFont;
                Font = nextFont;
            }
            finally
            {
                _applyingScaledFont = false;
            }

            previousScaledFont?.Dispose();
        }

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);
            UpdateRoundedRegion();
            Invalidate();
        }

        private void UpdateRoundedRegion()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            using var path = RoundedRect(new Rectangle(0, 0, Width, Height), CornerRadiusPx);
            var oldRegion = Region;
            Region = new Region(path);
            oldRegion?.Dispose();
        }

        private void DrawContent(Graphics g)
        {
            if (_popupData.Sections is null) return;

            float y = 0f;
            var originalColPos = _columnPosition;
            var originalColRight = _columnRight;

            if (_popupData.CustomSet)
            {
                _columnPosition = _popupData.ColPos;
                _columnRight = _popupData.ColRight;
            }

            int maxWidth = -1;

            foreach (var section in _popupData.Sections)
            {
                if (section.Content is null)
                    continue;

                foreach (var line in section.Content)
                {
                    // ---- layout (indent in pixels) ----
                    int indentPx = (int)Math.Round(line.Indent * Font.Size);
                    int p = ContentInset + InternalPaddingPx;
                    int x = p + indentPx;
                    int yTop = (int)Math.Round(y + p - _scrollY);
                    int w = Math.Max(0, Width - (2 * p + indentPx));
                    int h = Height; // initial; we’ll set precise height from measurement

                    var layout = new Rectangle(x, yTop, w, h);

                    // ---- wrapping: default wrap; column lines are single-line ----
                    var flags = BaseFlags | (line.HasColumn ? TextFormatFlags.SingleLine : TextFormatFlags.WordBreak);

                    string text = string.IsNullOrWhiteSpace(line.Text) ? "Null String" : line.Text;

                    // Measure bounded by width, with effectively unbounded height
                    var measured = TextRenderer.MeasureText(g, text, Font, new Size(layout.Width, int.MaxValue), flags);
                    var lineHeight = measured.Height;

                    // Track widest unbounded width (like original)
                    var unbounded = TextRenderer.MeasureText(g, line.Text, Font);
                    maxWidth = maxWidth == -1 ? unbounded.Width : Math.Max(maxWidth, unbounded.Width);

                    // Column text (same baseline rect; right/left aligned as requested)
                    if (line.HasColumn)
                    {
                        int colX = p + (int)Math.Round((Width - 2 * p) * _columnPosition);
                        int colW = Math.Max(0, Width - colX - p);
                        var colRect = new Rectangle(colX, layout.Y, colW, Height);

                        var colSingleWidth = TextRenderer.MeasureText(g, line.TextColumn ?? string.Empty, Font, Size.Empty, MeasureTight).Width;
                        var colFlags = (!_wrapColumnsThisLayout || colSingleWidth <= colW)
                            ? (BaseFlags | TextFormatFlags.SingleLine)
                            : (BaseFlags | TextFormatFlags.WordBreak);

                        var colMeasured = TextRenderer.MeasureText(g, line.TextColumn ?? string.Empty, Font, new Size(colW, int.MaxValue), colFlags);
                        lineHeight = Math.Max(lineHeight, colMeasured.Height);
                    }

                    // Final height (+1 line spacing as in original)
                    layout.Height = lineHeight + 1;

                    // Draw main text
                    TextRenderer.DrawText(g, line.Text, Font, layout, line.Color, flags);

                    if (line.HasColumn)
                    {
                        int colX = p + (int)Math.Round((Width - 2 * p) * _columnPosition);
                        int colW = Math.Max(0, Width - colX - p);
                        var colRect = new Rectangle(colX, layout.Y, colW, layout.Height);
                        var colSingleWidth = TextRenderer.MeasureText(g, line.TextColumn ?? string.Empty, Font, Size.Empty, MeasureTight).Width;
                        var colFlags = (!_wrapColumnsThisLayout || colSingleWidth <= colW)
                            ? (BaseFlags | TextFormatFlags.SingleLine)
                            : (BaseFlags | TextFormatFlags.WordBreak);

                        TextRenderer.DrawText(g, line.TextColumn, Font, colRect, line.ColorColumn, colFlags);
                    }

                    y += lineHeight + 1;
                }

                y += SectionPaddingPx;
            }

            _columnPosition = originalColPos;
            _columnRight = originalColRight;

            // Trailing status tag (top-right), like original
            if (_enhUniqueStatus is { } status && (status.InMain || status.InAlternate))
            {
                string msg = status.InMain ? "[Used]" : "[In Alternate]";
                Color color = status.InMain ? Color.Cyan : Color.MediumPurple;

                var tag = TextRenderer.MeasureText(g, msg, Font);
                var p = ContentInset + InternalPaddingPx;
                var rect = new Rectangle(Width - p - tag.Width, p, tag.Width, tag.Height);

                TextRenderer.DrawText(g, msg, Font, rect, color,
                    TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoClipping | TextFormatFlags.SingleLine);
            }
        }

        private static int ClampSafe(int value, int min, int max) => (min <= max) ? Math.Clamp(value, min, max) : min;

        private static int GetAvailableWidth(Rectangle client, Rectangle anchor, PlacementBias bias, int margin)
        {
            return bias switch
            {
                PlacementBias.PreferRight => Math.Max(0, (client.Right - margin) - (anchor.Right + margin)),
                PlacementBias.PreferLeft => Math.Max(0, (anchor.Left - margin) - (client.Left + margin)),
                _ => Math.Max(0, client.Width - 2 * margin),
            };
        }

        private int GetPopupWidthCap(Rectangle client, int margin)
        {
            var clientCap = Math.Max(1, client.Width - 2 * margin);
            var fractionCap = Math.Max(1, (int)Math.Round(client.Width * MaxClientWidthFraction));
            var cap = Math.Min(clientCap, Math.Min(MaximumReadableWidthPx, fractionCap));

            return Math.Max(Math.Min(MinimumReadableWidthPx, clientCap), cap);
        }

        private int GetWidthCap(Rectangle client, int availableWidth, int margin)
        {
            var clientCap = Math.Max(1, client.Width - 2 * margin);
            if (availableWidth >= MinimumReadableWidthPx || clientCap < MinimumReadableWidthPx)
            {
                return Math.Max(1, Math.Min(availableWidth, clientCap));
            }

            return Math.Min(MinimumReadableWidthPx, clientCap);
        }

        private void EnsureMeasuredToFit(int availableWidth)
        {
            // Cap cannot be negative; also don’t let it be 0 (TextRenderer needs some width).
            int cap = Math.Max(1, availableWidth);
            using var g = CreateGraphics();
            var desired = MeasureContent(g, maxWidth: cap);   // this enables constrained reflow/wrapping
            Width = desired.Width;
            Height = desired.Height;
        }

        private Size MeasureContent(Graphics g, int? maxWidth = null)
        {
            if (_popupData.Sections is null) return Size;

            var savedPos = _columnPosition;
            var savedRight = _columnRight;
            if (_popupData.CustomSet)
            {
                _columnPosition = _popupData.ColPos;
                _columnRight = _popupData.ColRight;
            }

            int p = ContentInset + InternalPaddingPx;
            float cp = Math.Clamp(_columnPosition, 0f, 0.99f); // avoid singularity at 1.0

            // ---------- PASS 1: natural width (no column wrapping) ----------
            int requiredWidth = 2 * p;

            foreach (var section in _popupData.Sections)
            {
                if (section.Content is null) continue;

                foreach (var line in section.Content)
                {
                    int indentPx = (int)Math.Round(line.Indent * Font.Size);

                    string main = string.IsNullOrWhiteSpace(line.Text) ? "Null String" : line.Text;
                    int mainW = TextRenderer.MeasureText(g, main, Font, Size.Empty, MeasureTight).Width;
                    requiredWidth = Math.Max(requiredWidth, 2 * p + indentPx + mainW);

                    if (line.HasColumn)
                    {
                        string col = line.TextColumn ?? string.Empty;
                        int colW = TextRenderer.MeasureText(g, col, Font, Size.Empty, MeasureTight).Width;

                        int needMainBeforeColumn = 2 * p + (int)Math.Ceiling((indentPx + mainW + ColumnGapPx) / cp);
                        // width needed so that single-line column fits at ColumnPosition
                        int needCol = (int)Math.Ceiling((colW + 2 * p - 2 * p * cp) / (1f - cp));
                        requiredWidth = Math.Max(requiredWidth, Math.Max(needMainBeforeColumn, needCol));
                    }
                }
            }

            // Status tag
            if (_enhUniqueStatus is { } st && (st.InMain || st.InAlternate))
            {
                string msg = st.InMain ? "[Used]" : "[In Alternate]";
                int tagW = TextRenderer.MeasureText(g, msg, Font, Size.Empty, MeasureTight).Width;
                requiredWidth = Math.Max(requiredWidth, 2 * p + tagW);
            }

            // If no cap or we already fit, compute height with this width
            int finalWidth = requiredWidth;

            _wrapColumnsThisLayout = false;

            if (maxWidth.HasValue && requiredWidth > maxWidth.Value)
            {
                // We must constrain to available width and allow column wrapping when needed
                finalWidth = Math.Max(maxWidth.Value, 2 * p + 1);
                _wrapColumnsThisLayout = true;
            }

            // ---------- PASS 2: height at finalWidth ----------
            float y = 0f;

            foreach (var section in _popupData.Sections)
            {
                if (section.Content is null) continue;

                foreach (var line in section.Content)
                {
                    int indentPx = (int)Math.Round(line.Indent * Font.Size);
                    int usable = Math.Max(0, finalWidth - (2 * p + indentPx));

                    // main text
                    var mainFlags = BaseFlags | (line.HasColumn ? TextFormatFlags.SingleLine : TextFormatFlags.WordBreak);
                    string main = string.IsNullOrWhiteSpace(line.Text) ? "Null String" : line.Text;
                    Size mainSz = TextRenderer.MeasureText(g, main, Font, new Size(usable, int.MaxValue), mainFlags);

                    // column text (only measured if present)
                    int lineHeight = mainSz.Height;
                    if (line.HasColumn)
                    {
                        // column region width for finalWidth
                        int colX = p + (int)Math.Round((finalWidth - 2 * p) * cp);
                        int colW = Math.Max(0, finalWidth - colX - p);

                        var colSingleW = TextRenderer.MeasureText(g, line.TextColumn ?? string.Empty, Font, Size.Empty, MeasureTight).Width;

                        // If constrained and column does not fit single-line, wrap the column text.
                        var colFlags = (!_wrapColumnsThisLayout || colSingleW <= colW)
                            ? (BaseFlags | TextFormatFlags.SingleLine)
                            : (BaseFlags | TextFormatFlags.WordBreak);

                        Size colSz = TextRenderer.MeasureText(g, line.TextColumn ?? string.Empty, Font, new Size(colW, int.MaxValue), colFlags);
                        lineHeight = Math.Max(lineHeight, colSz.Height);
                    }

                    y += lineHeight + 1;
                }

                y += SectionPaddingPx;
            }

            // restore
            _columnPosition = savedPos;
            _columnRight = savedRight;

            return new Size(finalWidth, (int)Math.Ceiling(y) + p);
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return path;
            }

            radius = Math.Max(1, Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2));
            var diameter = radius * 2;
            var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color Blend(Color first, Color second, float amountSecond)
        {
            amountSecond = Math.Clamp(amountSecond, 0f, 1f);
            var amountFirst = 1f - amountSecond;

            return Color.FromArgb(
                255,
                (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
                (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
                (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
        }

        #endregion
    }
}

using System.ComponentModel;
using Mids_Reborn.Core.Base.Display;

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

        private PopUp.PopupData _popupData;
        private I9Picker.EnhUniqueStatus? _enhUniqueStatus;

        private bool _wrapColumnsThisLayout;
        private float _columnPosition = 0.5f; // 0..1; relative to content width
        private bool _columnRight;
        private int _internalPadding = 3;
        private int _sectionPadding = 8;
        private int _scrollY;

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
            FontChanged += (_, _) => Invalidate();
        }

        #endregion

        #region Properties

        public bool HasContent { get; private set; }
        public bool IsOpen => Visible && HasContent;

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
            if (Width > avail) EnsureMeasuredToFit(GetWidthCap(client, avail, margin));

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
                if (Width > availFlip) EnsureMeasuredToFit(GetWidthCap(client, availFlip, margin));
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
            e.Graphics.Clear(BackColor);

            // Optional border like the old control
            using (var pen = new Pen(ForeColor))
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));

            DrawContent(e.Graphics);
        }

        #endregion

        #region Core drawing

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
                    int x = _internalPadding + indentPx;
                    int yTop = (int)Math.Round(y + _internalPadding - _scrollY);
                    int w = Math.Max(0, Width - (2 * _internalPadding + indentPx));
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
                        int colX = _internalPadding + (int)Math.Round((Width - 2 * _internalPadding) * _columnPosition);
                        int colW = Math.Max(0, Width - colX - _internalPadding);
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
                        int colX = _internalPadding + (int)Math.Round((Width - 2 * _internalPadding) * _columnPosition);
                        int colW = Math.Max(0, Width - colX - _internalPadding);
                        var colRect = new Rectangle(colX, layout.Y, colW, layout.Height);
                        var colSingleWidth = TextRenderer.MeasureText(g, line.TextColumn ?? string.Empty, Font, Size.Empty, MeasureTight).Width;
                        var colFlags = (!_wrapColumnsThisLayout || colSingleWidth <= colW)
                            ? (BaseFlags | TextFormatFlags.SingleLine)
                            : (BaseFlags | TextFormatFlags.WordBreak);

                        TextRenderer.DrawText(g, line.TextColumn, Font, colRect, line.ColorColumn, colFlags);
                    }

                    y += lineHeight + 1;
                }

                y += _sectionPadding;
            }

            Height = (int)Math.Round(y);

            _columnPosition = originalColPos;
            _columnRight = originalColRight;

            // Trailing status tag (top-right), like original
            if (_enhUniqueStatus is { } status && (status.InMain || status.InAlternate))
            {
                string msg = status.InMain ? "[Used]" : "[In Alternate]";
                Color color = status.InMain ? Color.Cyan : Color.MediumPurple;

                var tag = TextRenderer.MeasureText(g, msg, Font);
                var rect = new Rectangle(Width - _internalPadding - tag.Width, _internalPadding, tag.Width, tag.Height);

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

        private static int GetWidthCap(Rectangle client, int availableWidth, int margin)
        {
            var clientCap = Math.Max(1, client.Width - 2 * margin);
            if (availableWidth >= MinimumReadableWidth || clientCap < MinimumReadableWidth)
            {
                return Math.Max(1, Math.Min(availableWidth, clientCap));
            }

            return Math.Min(MinimumReadableWidth, clientCap);
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

            int p = _internalPadding;
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

                        int needMainBeforeColumn = 2 * p + (int)Math.Ceiling((indentPx + mainW + ColumnGap) / cp);
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

                y += _sectionPadding;
            }

            // restore
            _columnPosition = savedPos;
            _columnRight = savedRight;

            return new Size(finalWidth, (int)Math.Ceiling(y));
        }

        #endregion
    }
}

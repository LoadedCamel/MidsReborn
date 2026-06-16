using System.ComponentModel;
using Mids_Reborn.Core.Base.Display;

namespace Mids_Reborn.UI.Controls
{
    [DesignerCategory("Code")]
    public sealed class MidsPopup : UserControl
    {
        #region Constants (logical @96 DPI)
        private const int SectionPaddingLogical = 8;
        private const int InternalPaddingLogical = 3;
        private const int ColumnGapLogical = 6;
        private const int ScreenMarginLogical = 8;   // clamp margin inside host bounds
        private const int AnchorGapLogical = 10;   // space between popup and anchor rect
        private const float MaxWorkingAreaFraction = 0.60f;
        private const int MaxWidthSamples = 16;
        private const int MinWidthClampLogical = 180;
        private const int MaxWidthClampLogical = 500; // your chosen hard cap
        private const int GridStepsFallback = 24;
        private const float CGuardMin = 0.05f;
        private const float CGuardMax = 0.95f;
        #endregion

        #region Enums
        public enum PlacementBias
        {
            Auto,        // below -> above -> right -> left
            PreferBelow,
            PreferAbove,
            PreferRight,
            PreferLeft
        }

        public enum PlacementSide { Below, Above, Right, Left, Point }
        #endregion

        #region Fields
        private int _sectionPadding;
        private int _internalPadding;
        private int _columnGap;
        private int _screenMargin;
        private int _anchorGap;

        private PopUp.PopupData _data;
        private PickerControl.EnhUniqueStatus? _enhUniqueStatus;

        private readonly Dictionary<(float mult, FontStyle style), Font> _fontCache = new();
        private float _currentSplitC = 0.5f; // 0..1
        private float _layoutHeight;

        private readonly TextFormatFlags _oneLineNoPad =
            TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;
        #endregion

        #region Public surface (+ legacy indices)
        [Browsable(false)] public int HIdx { get; set; } = -1;
        [Browsable(false)] public int EIdx { get; set; } = -1;
        [Browsable(false)] public int PIdx { get; set; } = -1;
        [Browsable(false)] public int PsIdx { get; set; } = -1;

        [DefaultValue(true)]
        public bool MouseTransparent { get; set; } = true;

        [Browsable(false)]
        public float ContentHeight => _layoutHeight;

        /// <summary>Right-column text alignment (values). Left labels stay left-aligned.</summary>
        [Browsable(true), Category("Layout")]
        public bool ColumnRight { get; set; } = false;

        /// <summary>Raised before showing. Set e.Cancel = true to veto showing.</summary>
        public event EventHandler<CancelEventArgs>? BeforeShow;

        /// <summary>Raised after the popup has been shown (positioned and made visible).</summary>
        public event EventHandler? AfterShow;

        /// <summary>Raised when the popup is hidden via HidePopup().</summary>
        public event EventHandler? Hidden;
        #endregion

        #region Ctor
        public MidsPopup()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint
                   | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;

            // DPI-scaled paddings/gaps
            _sectionPadding = Scale(SectionPaddingLogical);
            _internalPadding = Scale(InternalPaddingLogical);
            _columnGap = Scale(ColumnGapLogical);
            _screenMargin = Scale(ScreenMarginLogical);
            _anchorGap = Scale(AnchorGapLogical);

            _data = default;
            _data.Init();
        }
        #endregion

        #region Public API (data + hide)
        public void SetPopup(PopUp.PopupData popup, PickerControl.EnhUniqueStatus? enhUniqueStatus = null)
        {
            _data = popup;
            _enhUniqueStatus = enhUniqueStatus;
            Invalidate();
        }

        public void SetPopup(PopUp.PopupData popup, bool inMain, bool inAlternate)
        {
            _data = popup;
            _enhUniqueStatus = new PickerControl.EnhUniqueStatus { InMain = inMain, InAlternate = inAlternate };
            Invalidate();
        }

        /// <summary>Hide the popup (no-op if already hidden).</summary>
        public void HidePopup()
        {
            if (Visible)
            {
                Visible = false;
                Hidden?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>Hide and reset legacy indices (handy for older callers).</summary>
        public void HidePopupSafe()
        {
            HidePopup();
            HIdx = EIdx = PIdx = PsIdx = -1;
        }
        #endregion

        #region Show / Reposition API (client-rect based, bias + auto-flip)
        /// <summary>
        /// Show next to a HOST-CLIENT rectangle using a placement bias; auto-flips side if it wouldn’t fit.
        /// Constrains inside the host's client area (form or panel).
        /// </summary>
        public bool TryShowAtClientRect(Control? host, Rectangle clientRect, PlacementBias bias = PlacementBias.PreferBelow)
        {
            if (host is null || Parent is null) return false;
            if (!HasRenderableContent()) { HidePopup(); return false; }

            var veto = new CancelEventArgs(false);
            BeforeShow?.Invoke(this, veto);
            if (veto.Cancel) { HidePopup(); return false; }

            // Bounds & anchor in SCREEN coordinates
            var bounds = host.RectangleToScreen(host.ClientRectangle);
            var anchor = host.RectangleToScreen(clientRect);

            // Measure/size once for available width
            using (var g = CreateGraphics())
            {
                var minWidth = Math.Max(Scale(MinWidthClampLogical), NaturalMinWidthForColumns(g));
                var hardMin = Math.Min(minWidth, Math.Max(bounds.Width - _screenMargin * 2, 50));
                var hardMax = Math.Min(Scale(MaxWidthClampLogical), Math.Max(bounds.Width - _screenMargin * 2, hardMin));
                hardMax = Math.Min(hardMax, (int)(bounds.Width * MaxWorkingAreaFraction));

                var (bestW, bestH, bestC) = FindBestWidthAndSplit(g, hardMin, hardMax);
                Size = new Size(bestW, bestH);
                _currentSplitC = bestC;
            }

            // Place with bias (auto-flips if needed), add outward gap, then clamp in host bounds.
            var (basePos, side) = ComputePlacement(anchor, Size, bounds, _screenMargin, _anchorGap, bias);
            var gap = Scale(AnchorGapLogical);
            var withGap = ApplyOutwardOffset(basePos, Size, side, new Point(gap, gap));
            var clamped = ClampToBounds(withGap, Size, bounds, _screenMargin);

            Location = Parent.PointToClient(clamped);
            BringToFront();
            Visible = true;
            Invalidate();
            AfterShow?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Reposition next to a HOST-CLIENT rectangle using the same rules (no re-measure / no repaint churn).
        /// </summary>
        public void RepositionAtClientRect(Control? host, Rectangle clientRect, PlacementBias bias = PlacementBias.PreferBelow)
        {
            if (!Visible || host is null || Parent is null) return;

            var bounds = host.RectangleToScreen(host.ClientRectangle);
            var anchor = host.RectangleToScreen(clientRect);

            var (basePos, side) = ComputePlacement(anchor, Size, bounds, _screenMargin, _anchorGap, bias);
            var gap = Scale(AnchorGapLogical);
            var withGap = ApplyOutwardOffset(basePos, Size, side, new Point(gap, gap));
            var clamped = ClampToBounds(withGap, Size, bounds, _screenMargin);

            Location = Parent.PointToClient(clamped);
            BringToFront();
        }

        /// <summary>
        /// Screen-rect variant (e.g., if you already have screen coords). Constrains to the screen’s working area.
        /// </summary>
        public bool TryShowAt(Rectangle anchorScreen, PlacementBias bias = PlacementBias.PreferBelow, Control? constrainToHost = null)
        {
            if (Parent is null) return false;
            if (!HasRenderableContent()) { HidePopup(); return false; }

            var veto = new CancelEventArgs(false);
            BeforeShow?.Invoke(this, veto);
            if (veto.Cancel) { HidePopup(); return false; }

            var bounds = constrainToHost?.RectangleToScreen(constrainToHost.ClientRectangle) ?? Screen.FromRectangle(anchorScreen).WorkingArea;

            using (var g = CreateGraphics())
            {
                var minWidth = Math.Max(Scale(MinWidthClampLogical), NaturalMinWidthForColumns(g));
                var hardMin = Math.Min(minWidth, Math.Max(bounds.Width - _screenMargin * 2, 50));
                var hardMax = Math.Min(Scale(MaxWidthClampLogical), Math.Max(bounds.Width - _screenMargin * 2, hardMin));
                hardMax = Math.Min(hardMax, (int)(bounds.Width * MaxWorkingAreaFraction));

                var (bestW, bestH, bestC) = FindBestWidthAndSplit(g, hardMin, hardMax);
                Size = new Size(bestW, bestH);
                _currentSplitC = bestC;
            }

            var (basePos, side) = ComputePlacement(anchorScreen, Size, bounds, _screenMargin, _anchorGap, bias);
            var gap = Scale(AnchorGapLogical);
            var withGap = ApplyOutwardOffset(basePos, Size, side, new Point(gap, gap));
            var clamped = ClampToBounds(withGap, Size, bounds, _screenMargin);

            Location = Parent.PointToClient(clamped);
            BringToFront();
            Visible = true;
            Invalidate();
            AfterShow?.Invoke(this, EventArgs.Empty);
            return true;
        }
        #endregion

        #region Painting
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (BackColor.A == 255)
            {
                using var b = new SolidBrush(BackColor);
                e.Graphics.FillRectangle(b, ClientRectangle);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using var buffer = BufferedGraphicsManager.Current.Allocate(e.Graphics, ClientRectangle);
            var g = buffer.Graphics;

            g.Clear(BackColor);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // If size changed externally, keep split optimal for current width.
            _currentSplitC = OptimizeSplitForHeight(g, ClientSize.Width, _currentSplitC);

            var totalHeight = DrawContents(g, ClientSize.Width, _currentSplitC);
            if ((int)Math.Round(totalHeight) != Height)
                Height = (int)Math.Round(totalHeight);

            using (var pen = new Pen(ForeColor))
            {
                var r = ClientRectangle; r.Width -= 1; r.Height -= 1;
                if (r.Width > 0 && r.Height > 0) g.DrawRectangle(pen, r);
            }

            buffer.Render(e.Graphics);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            RefreshDpiScaledMetrics();
        }

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);
            RefreshDpiScaledMetrics();
        }

        private void RefreshDpiScaledMetrics()
        {
            ClearFontCache();

            _sectionPadding = Scale(SectionPaddingLogical);
            _internalPadding = Scale(InternalPaddingLogical);
            _columnGap = Scale(ColumnGapLogical);
            _screenMargin = Scale(ScreenMarginLogical);
            _anchorGap = Scale(AnchorGapLogical);

            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) ClearFontCache();
            base.Dispose(disposing);
        }
        #endregion

        #region Content guard
        private bool HasRenderableContent()
        {
            if (_data.Sections == null || _data.Sections.Length == 0) return false;

            foreach (var section in _data.Sections)
            {
                if (section.Content == null || section.Content.Length == 0) continue;
                foreach (var sv in section.Content)
                {
                    if (!string.IsNullOrWhiteSpace(sv.Text)) return true;
                    if (!string.IsNullOrWhiteSpace(sv.TextColumn)) return true;
                }
            }
            return false;
        }
        #endregion

        #region Measurement + Auto-tuning
        private (double height, long overflow) ScoreFor(Graphics g, int width, float c)
        {
            // Height as you already compute:
            double h = ComputeHeightForC(g, width, c);

            // Overflow: how many pixels would each side still need (if negative -> 0).
            long overflow = 0;

            if (_data.Sections != null)
            {
                int pad = _internalPadding;
                int innerLeft = pad;
                int innerRight = Math.Max(0, width - pad);
                int innerWidth = Math.Max(0, innerRight - innerLeft);
                c = Math.Clamp(c, CGuardMin, CGuardMax);
                int columnX = innerLeft + (int)Math.Round(innerWidth * c);

                foreach (var section in _data.Sections)
                {
                    if (section?.Content == null) continue;

                    foreach (var sv in section.Content)
                    {
                        if (!sv.HasColumn) continue;

                        var font = GetLineFont(sv.Size, sv.Format);

                        // Required single-line widths
                        int reqLeft = TextRenderer.MeasureText(g, string.IsNullOrEmpty(sv.Text) ? " " : sv.Text, font, Size.Empty, _oneLineNoPad).Width;
                        int reqRight = TextRenderer.MeasureText(g, string.IsNullOrEmpty(sv.TextColumn) ? " " : sv.TextColumn, font, Size.Empty, _oneLineNoPad).Width;

                        int indentPx = sv.Indent * Font.Height;
                        int leftAvail = Math.Max(1, (columnX - _columnGap) - (innerLeft + indentPx));
                        int rightAvail = Math.Max(1, innerRight - (columnX + _columnGap));

                        overflow += Math.Max(0, reqLeft - leftAvail);
                        overflow += Math.Max(0, reqRight - rightAvail);
                    }
                }
            }

            return (h, overflow);
        }

        private (int width, int height, float c) FindBestWidthAndSplit(Graphics g, int hardMin, int hardMax)
        {
            if (hardMax <= hardMin)
            {
                var cSingle = OptimizeSplitForHeight(g, hardMin, 0.5f);
                var hSingle = (int)Math.Ceiling(ComputeHeightForC(g, hardMin, cSingle));
                return (hardMin, Math.Max(hSingle, 1), cSingle);
            }

            int samples = Math.Clamp(MaxWidthSamples, 6, 48);
            var results = new List<(int w, int h, float c)>(samples);

            for (int i = 0; i < samples; i++)
            {
                int w = hardMin + (int)Math.Round((hardMax - hardMin) * (i / (double)(samples - 1)));
                float c = OptimizeSplitForHeight(g, w, _currentSplitC);
                int h = (int)Math.Ceiling(ComputeHeightForC(g, w, c));
                results.Add((w, Math.Max(h, 1), c));
            }

            // Prefer minimal AREA (prevents tall, skinny popups), then lower height, then narrower width
            var byArea = results
                .OrderBy(t => (long)t.w * t.h)
                .ThenBy(t => t.h)
                .ThenBy(t => t.w)
                .First();

            return byArea;
        }

        private int NaturalMinWidthForColumns(Graphics g)
        {
            var rows = EnumerateTwoColumnRows(g).ToList();
            if (rows.Count == 0)
                return Scale(MinWidthClampLogical);

            var candidates = new HashSet<float>();
            foreach (var r in rows)
            {
                float c = r.A / (r.A + r.B);
                candidates.Add(Math.Clamp(c, CGuardMin, CGuardMax));
            }

            if (candidates.Count < 6)
            {
                for (int i = 0; i <= GridStepsFallback; i += 4)
                {
                    float c = (float)i / GridStepsFallback;
                    candidates.Add(Math.Clamp(c, CGuardMin, CGuardMax));
                }
            }

            int bestInner = int.MaxValue;
            foreach (var c in candidates)
            {
                int inner = 0;
                foreach (var r in rows)
                {
                    int reqLeft = (int)Math.Ceiling(r.A / c);
                    int reqRight = (int)Math.Ceiling(r.B / (1.0 - c));
                    inner = Math.Max(inner, Math.Max(reqLeft, reqRight));
                }
                if (inner < bestInner) bestInner = inner;
            }

            return bestInner + _internalPadding * 2;
        }

        private float OptimizeSplitForHeight(Graphics g, int width, float startC)
        {
            var candidates = BuildSplitCandidatesForHeight(g, width);
            if (candidates.Count == 0)
            {
                for (int i = 0; i <= GridStepsFallback; i++)
                    candidates.Add(Math.Clamp((float)i / GridStepsFallback, CGuardMin, CGuardMax));
            }

            double bestH = double.MaxValue;
            long bestOverflow = long.MaxValue;
            float bestC = Math.Clamp(startC, CGuardMin, CGuardMax);

            foreach (var c in candidates)
            {
                var (height, overflow) = ScoreFor(g, width, c);
                if (height < bestH || (Math.Abs(height - bestH) <= 0.5 && overflow < bestOverflow))
                {
                    bestH = height;
                    bestOverflow = overflow;
                    bestC = c;
                }
            }
            return bestC;
        }

        private HashSet<float> BuildSplitCandidatesForHeight(Graphics g, int width)
        {
            var set = new HashSet<float>();
            var rows = EnumerateTwoColumnRows(g).ToList();

            if (rows.Count == 0) { set.Add(0.5f); return set; }

            foreach (var r in rows)
            {
                float c = r.A / (r.A + r.B);
                c = Math.Clamp(c, CGuardMin, CGuardMax);
                set.Add(c);
            }

            const float delta = 0.03f;
            var extra = set.ToList();
            foreach (var c in extra)
            {
                set.Add(Math.Clamp(c - delta, CGuardMin, CGuardMax));
                set.Add(Math.Clamp(c + delta, CGuardMin, CGuardMax));
            }

            set.Add(0.40f); set.Add(0.50f); set.Add(0.60f);
            return set;
        }

        private struct TwoColRow { public int A; public int B; }

        private IEnumerable<TwoColRow> EnumerateTwoColumnRows(Graphics g)
        {
            if (_data.Sections == null) yield break;

            foreach (var section in _data.Sections)
            {
                if (section?.Content == null) continue;
                foreach (var sv in section.Content)
                {
                    if (!sv.HasColumn) continue;

                    var font = GetLineFont(sv.Size, sv.Format);
                    var flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

                    int wLeft = TextRenderer.MeasureText(g, string.IsNullOrEmpty(sv.Text) ? " " : sv.Text, font, Size.Empty, flags).Width;
                    int wRight = TextRenderer.MeasureText(g, string.IsNullOrEmpty(sv.TextColumn) ? " " : sv.TextColumn, font, Size.Empty, flags).Width;
                    int indentPx = sv.Indent * BaseIndentPx;

                    yield return new TwoColRow
                    {
                        A = wLeft + _columnGap + indentPx,
                        B = wRight + _columnGap
                    };
                }
            }
        }
        #endregion

        #region Height & Drawing
        private double ComputeHeightForC(Graphics g, int width, float c)
        {
            if (_data.Sections == null || _data.Sections.Length == 0) return 0;

            int pad = _internalPadding;
            int sectionPad = _sectionPadding;

            int innerLeft = pad;
            int innerRight = Math.Max(0, width - pad);
            int innerWidth = Math.Max(0, innerRight - innerLeft);
            c = Math.Clamp(c, CGuardMin, CGuardMax);

            int columnX = innerLeft + (int)Math.Round(innerWidth * c);
            double y = 0;

            foreach (var section in _data.Sections)
            {
                if (section?.Content == null || section.Content.Length == 0) continue;

                foreach (var sv in section.Content)
                {
                    int indentPx = sv.Indent * BaseIndentPx;
                    int leftX = innerLeft + indentPx;
                    int leftW = Math.Max(1, (columnX - _columnGap) - leftX);

                    var font = GetLineFont(sv.Size, sv.Format);

                    var flags = TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.NoPrefix;
                    flags |= sv.HasColumn ? (TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis)
                                          : TextFormatFlags.WordBreak;

                    var leftSize = MeasureText(g, sv.Text, font, leftW, flags);
                    y += leftSize.Height + 1;
                }
                y += sectionPad;
            }

            if (_enhUniqueStatus is { } status && (status.InMain || status.InAlternate))
            {
                var f = GetLineFont(1f, FontStyle.Bold);
                var sz = TextRenderer.MeasureText(g, "[Used]", f, Size.Empty, TextFormatFlags.NoPadding);
                y = Math.Max(y, pad + sz.Height + pad);
            }

            return y;
        }

        private float DrawContents(Graphics g, int width, float c)
        {
            if (_data.Sections == null || _data.Sections.Length == 0) { _layoutHeight = 0; return 0; }

            int pad = _internalPadding;
            int sectionPad = _sectionPadding;

            int innerLeft = pad;
            int innerRight = Math.Max(0, width - pad);
            int innerWidth = Math.Max(0, innerRight - innerLeft);
            c = Math.Clamp(c, CGuardMin, CGuardMax);
            int columnX = innerLeft + (int)Math.Round(innerWidth * c);

            float y = 0f;
            g.SetClip(ClientRectangle);

            foreach (var section in _data.Sections)
            {
                if (section?.Content == null || section.Content.Length == 0) continue;

                foreach (var sv in section.Content)
                {
                    int indentPx = sv.Indent * BaseIndentPx;
                    float topY = y + pad;

                    int leftX = innerLeft + indentPx;
                    int leftW = Math.Max(1, (columnX - _columnGap) - leftX);
                    var leftRect = new Rectangle(leftX, (int)Math.Round(topY), leftW, int.MaxValue);

                    var font = GetLineFont(sv.Size, sv.Format);

                    var flags = TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.NoPrefix;
                    flags |= sv.HasColumn ? (TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis)
                                          : TextFormatFlags.WordBreak;

                    var leftSize = MeasureText(g, sv.Text, font, leftRect.Width, flags);
                    var leftDrawRect = new Rectangle(leftRect.X, leftRect.Y, leftRect.Width, leftSize.Height);

                    if (!string.IsNullOrWhiteSpace(sv.Text) && leftDrawRect.Width > 0 && leftDrawRect.Height > 0)
                        TextRenderer.DrawText(g, sv.Text, font, leftDrawRect, sv.Color, flags);

                    if (sv.HasColumn)
                    {
                        int rightX = columnX + _columnGap;
                        int rightW = Math.Max(1, innerRight - rightX);
                        var rightRect = new Rectangle(rightX, (int)Math.Round(topY), rightW, leftSize.Height);

                        var colFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix
                                     | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis
                                     | (ColumnRight ? TextFormatFlags.Right : TextFormatFlags.Left);

                        if (!string.IsNullOrWhiteSpace(sv.TextColumn) && rightRect.Width > 0 && rightRect.Height > 0)
                            TextRenderer.DrawText(g, sv.TextColumn, font, rightRect, sv.ColorColumn, colFlags);
                    }

                    y += leftSize.Height + 1;
                }

                y += sectionPad;
            }

            if (_enhUniqueStatus is { } status2 && (status2.InMain || status2.InAlternate))
            {
                var text = status2.InMain ? "[Used]" : "[In Alternate]";
                var color = status2.InMain ? Color.Cyan : Color.MediumPurple;
                var f = GetLineFont(1f, FontStyle.Bold);
                var sz = TextRenderer.MeasureText(g, text, f, Size.Empty, TextFormatFlags.NoPadding);
                int rx = Math.Max(pad, width - pad - sz.Width);
                int ry = pad;
                TextRenderer.DrawText(g, text, f, new Rectangle(rx, ry, sz.Width, sz.Height), color, TextFormatFlags.NoPadding);
            }

            _layoutHeight = y;
            return y;
        }
        #endregion

        #region Placement helpers
        private static (Point pos, PlacementSide side) ComputePlacement(Rectangle anchor, Size popupSize, Rectangle bounds, int margin, int gap, PlacementBias bias)
        {
            if (anchor.Width == 0 && anchor.Height == 0)
                return (ClampToBounds(new Point(anchor.X + gap, anchor.Y + gap), popupSize, bounds, margin),
                        PlacementSide.Point);

            Func<Point> below = () => new(anchor.Left, anchor.Bottom + gap);
            Func<Point> above = () => new(anchor.Left, anchor.Top - gap - popupSize.Height);
            Func<Point> right = () => new(anchor.Right + gap, anchor.Top);
            Func<Point> left = () => new(anchor.Left - gap - popupSize.Width, anchor.Top);

            Func<bool> fitsBelow = () => anchor.Bottom + gap + popupSize.Height <= bounds.Bottom - margin;
            Func<bool> fitsAbove = () => anchor.Top - gap - popupSize.Height >= bounds.Top + margin;
            Func<bool> fitsRight = () => anchor.Right + gap + popupSize.Width <= bounds.Right - margin;
            Func<bool> fitsLeft = () => anchor.Left - gap - popupSize.Width >= bounds.Left + margin;

            (Func<Point> pos, Func<bool> fit, PlacementSide side)[] order = bias switch
            {
                PlacementBias.PreferBelow => new[] { (below, fitsBelow, PlacementSide.Below),
                                                     (above, fitsAbove, PlacementSide.Above),
                                                     (right, fitsRight, PlacementSide.Right),
                                                     (left,  fitsLeft,  PlacementSide.Left) },
                PlacementBias.PreferAbove => new[] { (above, fitsAbove, PlacementSide.Above),
                                                     (below, fitsBelow, PlacementSide.Below),
                                                     (right, fitsRight, PlacementSide.Right),
                                                     (left,  fitsLeft,  PlacementSide.Left) },
                PlacementBias.PreferRight => new[] { (right, fitsRight, PlacementSide.Right),
                                                     (left,  fitsLeft,  PlacementSide.Left),
                                                     (below, fitsBelow, PlacementSide.Below),
                                                     (above, fitsAbove, PlacementSide.Above) },
                PlacementBias.PreferLeft => new[] { (left,  fitsLeft,  PlacementSide.Left),
                                                     (right, fitsRight, PlacementSide.Right),
                                                     (below, fitsBelow, PlacementSide.Below),
                                                     (above, fitsAbove, PlacementSide.Above) },
                _ /* Auto */              => new[] { (below, fitsBelow, PlacementSide.Below),
                                                     (above, fitsAbove, PlacementSide.Above),
                                                     (right, fitsRight, PlacementSide.Right),
                                                     (left,  fitsLeft,  PlacementSide.Left) }
            };

            foreach (var c in order)
                if (c.fit()) return (c.pos(), c.side);

            var first = order[0];
            return (ClampToBounds(first.pos(), popupSize, bounds, margin), first.side);
        }

        private static Point ApplyOutwardOffset(Point pos, Size size, PlacementSide side, Point magnitude)
        {
            // magnitude is always positive; push away from the anchor on the chosen side
            return side switch
            {
                PlacementSide.Above => new Point(pos.X, pos.Y - Math.Abs(magnitude.Y)),
                PlacementSide.Below => new Point(pos.X, pos.Y + Math.Abs(magnitude.Y)),
                PlacementSide.Right => new Point(pos.X + Math.Abs(magnitude.X), pos.Y),
                PlacementSide.Left => new Point(pos.X - Math.Abs(magnitude.X), pos.Y),
                _ /* Point */       => new Point(pos.X + Math.Abs(magnitude.X), pos.Y + Math.Abs(magnitude.Y))
            };
        }

        private static Point ClampToBounds(Point p, Size s, Rectangle bounds, int margin)
        {
            int x = Math.Max(bounds.Left + margin, Math.Min(p.X, bounds.Right - margin - s.Width));
            int y = Math.Max(bounds.Top + margin, Math.Min(p.Y, bounds.Bottom - margin - s.Height));
            return new Point(x, y);
        }
        #endregion

        #region DPI, text, fonts
        private float ScaleFactor => DeviceDpi / 96f;
        private int Scale(int px) => (int)Math.Round(px * ScaleFactor);

        private int BaseIndentPx
        {
            get
            {
                float px = (Font.Unit == GraphicsUnit.Pixel)
                    ? Font.Size
                    : (Font.SizeInPoints * DeviceDpi / 72f);
                return Math.Max(1, (int)Math.Round(px));
            }
        }

        private Size MeasureText(Graphics g, string? text, Font font, int width, TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text)) text = " ";
            var proposed = new Size(Math.Max(0, width), int.MaxValue);
            return TextRenderer.MeasureText(g, text, font, proposed, flags);
        }

        private Font GetLineFont(float sizeMultiplier, FontStyle style)
        {
            float mult = Math.Max(0.1f, sizeMultiplier);

            // Base font size in PIXELS at the control’s DPI
            float basePx = (Font.Unit == GraphicsUnit.Pixel)
                ? Font.Size
                : (Font.SizeInPoints * DeviceDpi / 72f);

            float pxSize = Math.Max(1f, basePx * mult);

            var key = (mult, style);
            if (_fontCache.TryGetValue(key, out var cached))
                return cached;

            var f = new Font(Font.FontFamily, pxSize, style, GraphicsUnit.Pixel);
            _fontCache[key] = f;
            return f;
        }

        private void ClearFontCache()
        {
            foreach (var kv in _fontCache)
            {
                try { kv.Value.Dispose(); } catch { /* ignore */ }
            }
            _fontCache.Clear();
        }
        #endregion

        #region Hit-test transparency (optional, prevents hover flicker)
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTTRANSPARENT = -1;

            if (MouseTransparent && m.Msg == WM_NCHITTEST)
            {
                // Route mouse to whatever is behind us (canvas), avoiding MouseLeave flicker.
                m.Result = (IntPtr)HTTRANSPARENT;
                return;
            }

            base.WndProc(ref m);
        }
        #endregion
    }
}

using System.ComponentModel;
using System.Runtime.InteropServices;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls
{
    [DesignerCategory("Code")]
    public sealed class MidsRichTextView : Control
    {
        #region Win32 (RichEdit print + GDI viewport)
        private const int WM_PRINTCLIENT = 0x0318;
        private const int PRF_CLIENT = 0x00000004;
        private const int PRF_ERASEBKGND = 0x00000008;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern nint SendMessage(nint hWnd, int msg, nint wParam, nint lParam);

        [DllImport("gdi32.dll")]
        private static extern bool SetViewportOrgEx(nint hdc, int x, int y, nint lpPoint);
        #endregion

        #region DPI + metrics
        private const int LogicalScrollBarWidth = 16;
        private const int LogicalArrowHeight = 16;
        private const int LogicalThumbMinHeight = 20;
        private const int LogicalWheelStepPx = 32;
        private const int LogicalArrowStepPx = 30;
        private const int LogicalPageStepMarginPx = 8;

        private float DpiScale => DeviceDpi / 96f;
        private int ScalePx(int v) => (int)Math.Round(v * DpiScale);

        private int BarW => ScalePx(LogicalScrollBarWidth);
        private int ArrowH => ScalePx(LogicalArrowHeight);
        private int ThumbMin => ScalePx(LogicalThumbMinHeight);
        private int WheelStep => ScalePx(LogicalWheelStepPx);
        private int ArrowStep => ScalePx(LogicalArrowStepPx);
        private int PageMargin => ScalePx(LogicalPageStepMarginPx);
        private int ViewportHeight => Math.Max(1, ClientSize.Height - Padding.Vertical);
        #endregion

        #region Fields
        private readonly RichTextBox _rtb; // hidden RichEdit host
        private int _contentHeight;        // total content height (px)
        private int _scrollY;              // viewport top (px)

        // scrollbar geometry
        private Rectangle _sbBounds, _track, _upRect, _dnRect, _thumb;
        private bool _hoverThumb, _hoverUp, _hoverDn, _dragThumb;
        private int _dragOffsetY;

        private bool _themeHooked;
        private Action? _themeChangedHandler;
        #endregion

        #region Theme + design helpers
        private static bool InDesign(IComponent? c)
            => LicenseManager.UsageMode == LicenseUsageMode.Designtime
               || c is Control ctrl && ctrl.Site is not null && ctrl.Site.DesignMode;

        private ScrollPanelTheme Theme =>
            InDesign(this) ? ThemeManager.DesignTime.ScrollPanel
                           : ThemeManager.CurrentTheme?.ScrollPanel ?? ThemeManager.DesignTime.ScrollPanel;
        #endregion

        #region Public properties
        [Category("Appearance")]
        public override Color BackColor
        {
            get => base.BackColor;
            set { base.BackColor = value; _rtb.BackColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set { base.ForeColor = value; _rtb.ForeColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public override Font Font
        {
            get => base.Font;
            set { base.Font = value; _rtb.Font = value; RecalcFromLayout(); }
        }

        [Category("Appearance")]
        [DefaultValue(BorderStyle.FixedSingle)]
        public BorderStyle BorderStyle { get; set; } = BorderStyle.FixedSingle;

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool DetectUrls
        {
            get => _rtb.DetectUrls;
            set { _rtb.DetectUrls = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool WordWrap
        {
            get => _rtb.WordWrap;
            set { _rtb.WordWrap = value; RecalcFromLayout(); }
        }

        [Browsable(true)]
        public override string Text
        {
            get => _rtb.Text;
            set { _rtb.Text = value; RecalcFromLayout(); }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Rtf
        {
            get => _rtb.Rtf;
            set { _rtb.Rtf = value; RecalcFromLayout(); }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool WheelScrollEnabled { get; set; } = true;

        [Browsable(false)]
        public int ContentHeight => _contentHeight;
        #endregion

        public MidsRichTextView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            base.BackColor = Color.White;

            _rtb = new RichTextBox
            {
                BorderStyle = BorderStyle.None,
                Font = Font,
                ScrollBars = RichTextBoxScrollBars.None, // we render/scroll ourselves
                DetectUrls = false,
                WordWrap = true,
                Multiline = true,
                ReadOnly = true,
                TabStop = false,
                Visible = false // hidden; used for layout & paint
            };
            Controls.Add(_rtb);

            _rtb.ContentsResized += (_, e) =>
            {
                // Height RTB requires to layout all content (float→int)
                RecalcAndApplyHeight((int)Math.Ceiling((double)e.NewRectangle.Height));
            };

            if (!InDesign(this))
            {
                _themeChangedHandler = Invalidate;
                ThemeManager.ThemeChanged += _themeChangedHandler;
                _themeHooked = true;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Safe to BeginInvoke now
            _rtb.TextChanged += (_, _) => BeginInvoke(RecalcFromLayout);
            _rtb.FontChanged += (_, _) => BeginInvoke(RecalcFromLayout);
            SizeChanged += (_, _) => BeginInvoke(() => { LayoutEngineRects(); RecalcFromLayout(); });

            // Ensure child handle exists for WM_PRINTCLIENT
            _ = _rtb.Handle;

            LayoutEngineRects();
            RecalcFromLayout();
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            LayoutEngineRects();
            RecalcFromLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _themeHooked && _themeChangedHandler is not null)
            {
                ThemeManager.ThemeChanged -= _themeChangedHandler;
                _themeHooked = false; _themeChangedHandler = null;
            }
            base.Dispose(disposing);
        }

        #region Layout + metrics
        private void LayoutEngineRects()
        {
            // Always reserve the bar strip so wrapping is stable (no reflow pop-in)
            int sbw = BarW;
            var viewport = ContentViewportRect();

            // Width fixed to printable area; height will be set to content in RecalcAndApplyHeight
            _rtb.Bounds = new Rectangle(0, 0, Math.Max(1, viewport.Width - sbw),
                                        Math.Max(viewport.Height, _contentHeight));

            // Scrollbar geometry
            _sbBounds = new Rectangle(viewport.Right - BarW, viewport.Top, BarW, viewport.Height);
            _upRect = new Rectangle(_sbBounds.Left, _sbBounds.Top, _sbBounds.Width, ArrowH);
            _dnRect = new Rectangle(_sbBounds.Left, _sbBounds.Bottom - ArrowH, _sbBounds.Width, ArrowH);
            _track = Rectangle.FromLTRB(_sbBounds.Left, _upRect.Bottom, _sbBounds.Right, _dnRect.Top);

            ComputeThumb();
            Invalidate();
        }

        private void RecalcFromLayout()
        {
            // Fallback calc works even when ContentsResized doesn’t fire
            int last = Math.Max(0, _rtb.TextLength);           // after last char
            var pt = _rtb.GetPositionFromCharIndex(last);    // client Y of last char
            int line = Math.Max(1, _rtb.Font.Height);
            int needed = Math.Max(line, pt.Y + line);

            RecalcAndApplyHeight(needed);
        }

        private void RecalcAndApplyHeight(int desired)
        {
            _contentHeight = Math.Max(1, desired);

            // CRITICAL: make the hidden RTB as tall as the content so there is something to scroll
            int newH = Math.Max(ViewportHeight, _contentHeight);
            if (_rtb.Height != newH) _rtb.Height = newH;

            ClampScroll();
            ComputeThumb();
            Invalidate();
        }

        private bool NeedsScrollbar() => _contentHeight > ViewportHeight + 1;

        private void ClampScroll()
        {
            int max = Math.Max(0, _contentHeight - ViewportHeight);
            if (_scrollY > max) _scrollY = max;
            if (_scrollY < 0) _scrollY = 0;
        }

        private void ComputeThumb()
        {
            if (_track.Height <= 0)
            {
                _thumb = Rectangle.Empty;
                return;
            }

            int visible = ViewportHeight;
            int total = Math.Max(1, _contentHeight);
            int trackH = _track.Height;

            int thumbH = Math.Min(trackH, Math.Max(ThumbMin, (int)Math.Round((double)visible / total * trackH)));
            int avail = Math.Max(0, trackH - thumbH);

            int maxScroll = Math.Max(0, _contentHeight - visible);
            int thumbY = _track.Top;

            if (avail > 0 && maxScroll > 0)
            {
                double r = (double)_scrollY / maxScroll;
                thumbY = _track.Top + (int)Math.Round(avail * r);
            }

            int inset = Math.Max(1, (int)Math.Round(_sbBounds.Width * 0.25));
            _thumb = new Rectangle(_sbBounds.Left + inset, thumbY, Math.Max(1, _sbBounds.Width - inset * 2), thumbH);
        }

        private Rectangle ContentViewportRect()
        {
            var rect = new Rectangle(
                Padding.Left,
                Padding.Top,
                Math.Max(1, ClientSize.Width - Padding.Horizontal),
                Math.Max(1, ClientSize.Height - Padding.Vertical));

            return rect;
        }
        #endregion

        #region Paint (WM_PRINTCLIENT + viewport shift)
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(BackColor);
            var viewport = ContentViewportRect();

            // Shift the HDC upwards by _scrollY so RichEdit paints the scrolled viewport
            var oldClip = g.Clip.Clone();
            g.SetClip(new Rectangle(viewport.Left, viewport.Top, Math.Max(1, viewport.Width - BarW), viewport.Height));
            nint hdc = g.GetHdc();
            try
            {
                SetViewportOrgEx(hdc, viewport.Left, viewport.Top - _scrollY, nint.Zero);
                SendMessage(_rtb.Handle, WM_PRINTCLIENT, hdc, PRF_CLIENT | PRF_ERASEBKGND);
                SetViewportOrgEx(hdc, 0, 0, nint.Zero);
            }
            finally
            {
                g.ReleaseHdc(hdc);
                g.Clip = oldClip;
                oldClip.Dispose();
            }

            DrawScrollbar(g);
            DrawBorder(g);
        }

        private void DrawScrollbar(Graphics g)
        {
            var t = Theme;

            using (var trackPen = new Pen(t.Track, Math.Max(1, ScalePx(2))))
            {
                int cx = _sbBounds.Left + _sbBounds.Width / 2;
                g.DrawLine(trackPen, cx, _track.Top, cx, _track.Bottom);
            }

            using (var arrowBrush = new SolidBrush(_hoverUp || _hoverDn ? t.Hover : t.Bar))
            {
                // Up
                Point[] up =
                {
                    new(_upRect.Left + _upRect.Width/2, _upRect.Top + _upRect.Height/4),
                    new(_upRect.Left + ScalePx(3), _upRect.Bottom - ScalePx(4)),
                    new(_upRect.Right - ScalePx(3), _upRect.Bottom - ScalePx(4))
                };
                g.FillPolygon(arrowBrush, up);

                // Down
                Point[] dn =
                {
                    new(_dnRect.Left + _dnRect.Width/2, _dnRect.Bottom - _dnRect.Height/4),
                    new(_dnRect.Left + ScalePx(3), _dnRect.Top + ScalePx(4)),
                    new(_dnRect.Right - ScalePx(3), _dnRect.Top + ScalePx(4))
                };
                g.FillPolygon(arrowBrush, dn);
            }

            using var thumbBrush = new SolidBrush(_hoverThumb ? t.Hover : t.Bar);
            g.FillRectangle(thumbBrush, _thumb);
        }

        private void DrawBorder(Graphics g)
        {
            if (BorderStyle == BorderStyle.None) return;
            using var pen = new Pen(SystemColors.WindowFrame);
            var r = ClientRectangle;
            r.Width -= 1;
            r.Height -= 1;
            g.DrawRectangle(pen, r);
        }
        #endregion

        #region Input (wheel / keys / mouse)
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!WheelScrollEnabled || !NeedsScrollbar()) return;

            int lines = SystemInformation.MouseWheelScrollLines;
            if (lines <= 0) lines = 3;
            int steps = e.Delta / 120 * lines;
            if (steps != 0) ScrollByPixels(-steps * WheelStep);
        }

        protected override bool IsInputKey(Keys keyData) =>
            keyData is Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End
            || base.IsInputKey(keyData);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!NeedsScrollbar()) return;

            switch (e.KeyCode)
            {
                case Keys.Up: ScrollByPixels(-ArrowStep); e.Handled = true; break;
                case Keys.Down: ScrollByPixels(ArrowStep); e.Handled = true; break;
                case Keys.PageUp: ScrollPage(-1); e.Handled = true; break;
                case Keys.PageDown: ScrollPage(+1); e.Handled = true; break;
                case Keys.Home: _scrollY = 0; ClampScroll(); ComputeThumb(); Invalidate(); e.Handled = true; break;
                case Keys.End:
                    _scrollY = Math.Max(0, _contentHeight - ClientSize.Height);
                    ClampScroll(); ComputeThumb(); Invalidate(); e.Handled = true;
                    break;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!NeedsScrollbar()) return;

            bool oh = _hoverThumb, ou = _hoverUp, od = _hoverDn;
            _hoverThumb = _thumb.Contains(e.Location);
            _hoverUp = _upRect.Contains(e.Location);
            _hoverDn = _dnRect.Contains(e.Location);

            if (_dragThumb)
            {
                int avail = Math.Max(0, _track.Height - _thumb.Height);
                int newTop = Math.Max(_track.Top, Math.Min(e.Y - _dragOffsetY, _track.Top + avail));
                double ratio = avail > 0 ? (double)(newTop - _track.Top) / avail : 0d;
                int maxScroll = Math.Max(0, _contentHeight - ClientSize.Height);
                _scrollY = (int)Math.Round(ratio * maxScroll);
                ClampScroll();
                ComputeThumb();
                Invalidate(_sbBounds);
            }
            else if (oh != _hoverThumb || ou != _hoverUp || od != _hoverDn)
            {
                Invalidate(_sbBounds);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            if (_thumb.Contains(e.Location))
            {
                _dragThumb = true;
                _dragOffsetY = e.Y - _thumb.Top;
                Capture = true;
            }
            else if (_upRect.Contains(e.Location))
            {
                ScrollByPixels(-ArrowStep);
            }
            else if (_dnRect.Contains(e.Location))
            {
                ScrollByPixels(ArrowStep);
            }
            else if (_track.Contains(e.Location))
            {
                if (e.Y < _thumb.Top) ScrollPage(-1);
                else if (e.Y > _thumb.Bottom) ScrollPage(+1);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_dragThumb) { _dragThumb = false; Capture = false; }
        }

        private void ScrollByPixels(int delta)
        {
            _scrollY += delta;
            ClampScroll();
            ComputeThumb();
            Invalidate(); // repaint content + bar
        }

        private void ScrollPage(int dir)
        {
            int page = Math.Max(0, ViewportHeight - PageMargin);
            ScrollByPixels(dir * page);
        }
        #endregion
    }
}

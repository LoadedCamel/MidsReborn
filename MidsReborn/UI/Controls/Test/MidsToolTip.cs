using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

public class MidsToolTip : ToolTip
{
    #region Constants

    private const int DefaultCornerRadius = 8;
    private const int DefaultPadding = 8;
    private const int DefaultMaxWidth = 360;
    private const int TitleGap = 6;
    private const int OutlineThickness = 1;

    #endregion

    #region Instance Fields

    private int _cornerRadius = DefaultCornerRadius;
    private int _padding = DefaultPadding;
    private int _maxWidth = DefaultMaxWidth;

    private Color _backColorTop = Color.FromArgb(16,18,21);
    private Color _backColorBottom = Color.FromArgb(11,13,16);
    private Color _borderColor = Color.FromArgb(58,65,75);
    private Color _titleColor = Color.FromArgb(241,244,249);
    private Color _textColor = Color.FromArgb(227,232,238);

    private Font _contentFont = new Font("Segoe UI", 9f);
    private Font? _titleFont;

    private string? _activeText;                 // last text passed to Show(...)
    private IWin32Window? _activeWindow;         // last window passed to Show(...)
    private bool _inPopup;                       // guard re-entrancy

    #endregion

    #region Public Properties

    [Category("Appearance")]
    [DefaultValue(DefaultCornerRadius)]
    public int CornerRadius
    {
        get => _cornerRadius;
        set => _cornerRadius = Math.Max(0, value);
    }

    [Category("Layout")]
    [DefaultValue(DefaultPadding)]
    public int PaddingLogical
    {
        get => _padding;
        set => _padding = Math.Max(0, value);
    }

    [Category("Layout")]
    [DefaultValue(DefaultMaxWidth)]
    public int MaxWidth
    {
        get => _maxWidth;
        set => _maxWidth = Math.Max(120, value);
    }

    [Category("Appearance")]
    public Color BackColorTop
    {
        get => _backColorTop;
        set => _backColorTop = value;
    }

    [Category("Appearance")]
    public Color BackColorBottom
    {
        get => _backColorBottom;
        set => _backColorBottom = value;
    }

    [Category("Appearance")]
    public Color BorderColor
    {
        get => _borderColor;
        set => _borderColor = value;
    }

    [Category("Appearance")]
    public Color TitleColor
    {
        get => _titleColor;
        set => _titleColor = value;
    }

    [Category("Appearance")]
    public Color TextColor
    {
        get => _textColor;
        set => _textColor = value;
    }

    /// <summary>
    /// Font used for the body text.
    /// </summary>
    [Category("Appearance")]
    public Font ContentFont
    {
        get => _contentFont;
        set => _contentFont = value;
    }

    /// <summary>
    /// Optional override for the title font. If null, uses bold of <see cref="ContentFont"/>.
    /// </summary>
    [Category("Appearance")]
    public Font? TitleFont
    {
        get => _titleFont;
        set => _titleFont = value;
    }

    #endregion

    #region Constructor

    public MidsToolTip()
    {
        OwnerDraw = true;
        IsBalloon = false;
        UseAnimation = false;
        UseFading = false;

        // good UX defaults
        AutomaticDelay = 200;
        AutoPopDelay = 6000;
        InitialDelay = 600;
        ReshowDelay = 150;

        Draw += OnDraw;
        Popup += OnPopupMeasure;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public new bool OwnerDraw
    {
        get => true;
        set { base.OwnerDraw = true; } // ignore attempts to disable
    }

    #endregion

    #region Show/Hide (cache dynamic text)

    // Cache the text for every Show(...) overload so Popup can size properly.

    public new void Show(string text, IWin32Window window)
        {
            _activeText = text;
            _activeWindow = window;
            base.Show(text, window);
        }

        public new void Show(string text, IWin32Window window, int duration)
        {
            _activeText = text;
            _activeWindow = window;
            base.Show(text, window, duration);
        }

        public new void Show(string text, IWin32Window window, Point point)
        {
            _activeText = text;
            _activeWindow = window;
            base.Show(text, window, point);
        }

        public new void Show(string text, IWin32Window window, Point point, int duration)
        {
            _activeText = text;
            _activeWindow = window;
            base.Show(text, window, point, duration);
        }

        public new void Show(string text, IWin32Window window, int x, int y)
        {
            _activeText = text;
            _activeWindow = window;
            base.Show(text, window, x, y);
        }

        public new void Show(string text, IWin32Window window, int x, int y, int duration)
        {
            _activeText = text;
            _activeWindow = window;
            base.Show(text, window, x, y, duration);
        }

    public new void Hide(IWin32Window window)
    {
        if (ReferenceEquals(window, _activeWindow))
        {
            _activeText = null;
                _activeWindow = null;
            }
            base.Hide(window);
        }

    /// <summary>Convenience for client-point shows.</summary>
    public void ShowAt(Control relativeTo, string text, Point clientLocation, int durationMs = 4000)
        => Show(text, relativeTo, clientLocation, durationMs);

    public void ShowClamped(Control relativeTo, string text, Point preferredClientLocation, int durationMs = 4000)
    {
        if (relativeTo == null)
        {
            return;
        }

        var tooltipSize = MeasureTooltip(relativeTo, text);
        var preferredScreenPoint = relativeTo.PointToScreen(preferredClientLocation);
        var workingArea = Screen.FromControl(relativeTo).WorkingArea;
        var dpi = relativeTo.DeviceDpi / 96f;
        var margin = Math.Max(8, (int)Math.Round(8 * dpi));

        var clampedScreenX = Math.Max(
            workingArea.Left + margin,
            Math.Min(preferredScreenPoint.X, workingArea.Right - tooltipSize.Width - margin));

        var clampedScreenY = Math.Max(
            workingArea.Top + margin,
            Math.Min(preferredScreenPoint.Y, workingArea.Bottom - tooltipSize.Height - margin));

        var clampedClientPoint = relativeTo.PointToClient(new Point(clampedScreenX, clampedScreenY));
        Show(text, relativeTo, clampedClientPoint.X, clampedClientPoint.Y, durationMs);
    }

    #endregion

    #region Popup (measure)

        private void OnPopupMeasure(object? sender, PopupEventArgs e)
        {
            if (_inPopup) return; // guard (defensive)
            _inPopup = true;

            try
            {
            e.ToolTipSize = MeasureTooltip(e.AssociatedControl);
        }
        finally
        {
            _inPopup = false;
        }
        }

        #endregion

        #region Draw (render)

        private void OnDraw(object? sender, DrawToolTipEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float dpi = e.AssociatedControl.DeviceDpi / 96f;
            int pad = (int)Math.Round(_padding * dpi);
            int corner = (int)Math.Round(_cornerRadius * dpi);
            int outline = Math.Max(1, (int)Math.Round(OutlineThickness * dpi));
            int titleGapPx = (int)Math.Round(TitleGap * dpi);

            var rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);

            using var path = CreateRoundedRectPath(rect, corner);
            using var bg = new LinearGradientBrush(rect, BackColorTop, BackColorBottom, LinearGradientMode.Vertical);
            using var pen = new Pen(BorderColor, outline);

            g.FillPath(bg, path);
            g.DrawPath(pen, path);

            // Content area
            var content = Rectangle.Inflate(rect, -pad, -pad);
            int x = content.Left;
            int y = content.Top;

            string? title = string.IsNullOrWhiteSpace(ToolTipTitle) ? null : ToolTipTitle;
            using var titleFont = title is null ? null : (TitleFont ?? new Font(ContentFont, FontStyle.Bold));

            if (title is not null && titleFont is not null)
            {
                var titleFlags = TextFormatFlags.NoPrefix
                               | TextFormatFlags.EndEllipsis
                               | TextFormatFlags.TextBoxControl
                               | TextFormatFlags.PreserveGraphicsClipping
                               | TextFormatFlags.PreserveGraphicsTranslateTransform;

                var titleRect = new Rectangle(x, y, content.Right - x, titleFont.Height + 2);
                TextRenderer.DrawText(g, title, titleFont, titleRect, TitleColor, titleFlags);
                y += titleRect.Height + titleGapPx;
            }

            var bodyFlags = TextFormatFlags.NoPadding
                          | TextFormatFlags.NoPrefix
                          | TextFormatFlags.WordBreak
                          | TextFormatFlags.TextBoxControl
                          | TextFormatFlags.PreserveGraphicsClipping
                          | TextFormatFlags.PreserveGraphicsTranslateTransform;

            var bodyRect = new Rectangle(x, y, content.Right, content.Bottom);
            TextRenderer.DrawText(g, e.ToolTipText, ContentFont, bodyRect, TextColor, bodyFlags);
        }

        #endregion

    #region Helpers

    private Size MeasureTooltip(Control associatedControl, string? overrideBody = null)
    {
        string body = !string.IsNullOrEmpty(overrideBody)
            ? overrideBody
            : !string.IsNullOrEmpty(_activeText)
                ? _activeText!
                : (GetToolTip(associatedControl) ?? string.Empty);

        string? title = string.IsNullOrWhiteSpace(ToolTipTitle) ? null : ToolTipTitle;
        using var titleFont = title is null ? null : (TitleFont ?? new Font(ContentFont, FontStyle.Bold));
        using var g = associatedControl.CreateGraphics();

        float dpi = associatedControl.DeviceDpi / 96f;
        int pad = (int)Math.Round(_padding * dpi);
        int corner = (int)Math.Round(_cornerRadius * dpi);
        int outline = Math.Max(1, (int)Math.Round(OutlineThickness * dpi));
        int maxW = (int)Math.Round(_maxWidth * dpi);
        int titleGapPx = (int)Math.Round(TitleGap * dpi);
        int screenLimitedWidth = Math.Max(120, Screen.FromControl(associatedControl).WorkingArea.Width - pad * 4);
        maxW = Math.Min(maxW, screenLimitedWidth);

        var flags = TextFormatFlags.NoPadding
                  | TextFormatFlags.NoPrefix
                  | TextFormatFlags.TextBoxControl
                  | TextFormatFlags.PreserveGraphicsClipping
                  | TextFormatFlags.PreserveGraphicsTranslateTransform;

        Size bodySize = TextRenderer.MeasureText(g, body, ContentFont, new Size(maxW, int.MaxValue),
            flags | TextFormatFlags.WordBreak);

        Size titleSize = Size.Empty;
        if (title is not null && titleFont is not null)
        {
            titleSize = TextRenderer.MeasureText(g, title, titleFont, new Size(maxW, int.MaxValue),
                (flags & ~TextFormatFlags.WordBreak) | TextFormatFlags.EndEllipsis);
        }

        int textBlockWidth = Math.Min(maxW, Math.Max(bodySize.Width, titleSize.Width));
        int width = pad + textBlockWidth + pad + outline;
        int height = pad
                   + (titleSize.Height > 0 ? titleSize.Height + titleGapPx : 0)
                   + bodySize.Height
                   + pad + outline;

        width = Math.Max(width, corner * 2 + pad * 2);
        height = Math.Max(height, corner * 2 + pad * 2);
        return new Size(width, height);
    }

    private static GraphicsPath CreateRoundedRectPath(Rectangle bounds, int radius)
    {
            int r = Math.Max(0, radius);
            var path = new GraphicsPath();
            if (r == 0)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            int d = r * 2;
            var arc = new Rectangle(bounds.Left, bounds.Top, d, d);

            path.AddArc(arc, 180, 90);              // TL
            arc.X = bounds.Right - d;
            path.AddArc(arc, 270, 90);              // TR
            arc.Y = bounds.Bottom - d;
            path.AddArc(arc, 0, 90);                // BR
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);               // BL
            path.CloseFigure();
            return path;
        }

        #endregion
}

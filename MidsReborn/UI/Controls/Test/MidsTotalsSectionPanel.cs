using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

[DesignerCategory("Code")]
internal sealed class MidsTotalsSectionPanel : Panel
{
    private const int DefaultCornerRadius = 7;
    private const int DefaultHeaderHeight = 24;
    private const int DefaultHeaderInset = 8;
    private const int DefaultContentInset = 6;
    private const int DefaultSectionGap = 8;

    private float _uiScale = 1f;
    private string _title = string.Empty;
    private string _metaText = string.Empty;
    private MidsTotalsGlyph? _titleIcon;
    private Color _titleIconColor = Color.Transparent;
    private Color _titleColor = Color.Empty;
    private Color _metaColor = Color.Empty;
    private Control? _contentControl;

    private DataViewTheme CurrentTheme =>
        DesignMode
            ? ThemeManager.DesignTime.DataView
            : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

    public float UiScale
    {
        get => _uiScale;
        set
        {
            var clamped = Math.Clamp(value, 0.82f, 1.25f);
            if (Math.Abs(_uiScale - clamped) < 0.01f) return;
            _uiScale = clamped;
            PerformLayout();
            Invalidate();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) return;
            _title = value;
            Invalidate();
        }
    }

    public string MetaText
    {
        get => _metaText;
        set
        {
            if (_metaText == value) return;
            _metaText = value;
            Invalidate();
        }
    }

    public MidsTotalsGlyph? TitleIcon
    {
        get => _titleIcon;
        set
        {
            if (_titleIcon == value) return;
            _titleIcon = value;
            Invalidate();
        }
    }

    public Color TitleIconColor
    {
        get => _titleIconColor;
        set
        {
            if (_titleIconColor == value) return;
            _titleIconColor = value;
            Invalidate();
        }
    }

    public Color TitleColor
    {
        get => _titleColor;
        set
        {
            if (_titleColor == value) return;
            _titleColor = value;
            Invalidate();
        }
    }

    public Color MetaColor
    {
        get => _metaColor;
        set
        {
            if (_metaColor == value) return;
            _metaColor = value;
            Invalidate();
        }
    }

    public Control? ContentControl
    {
        get => _contentControl;
        set
        {
            if (ReferenceEquals(_contentControl, value)) return;

            if (_contentControl != null)
            {
                Controls.Remove(_contentControl);
                _contentControl.SizeChanged -= ContentControl_SizeChanged;
            }

            _contentControl = value;
            if (_contentControl != null)
            {
                _contentControl.BackColor = Color.Transparent;
                _contentControl.SizeChanged += ContentControl_SizeChanged;
                Controls.Add(_contentControl);
                _contentControl.BringToFront();
            }

            PerformLayout();
            Invalidate();
        }
    }

    public MidsTotalsSectionPanel()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint |
            ControlStyles.SupportsTransparentBackColor,
            true);

        BackColor = Color.Transparent;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Margin = Padding.Empty;
        TabStop = false;
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var width = proposedSize.Width > 0 ? proposedSize.Width : Width;
        var contentWidth = Math.Max(1, width - (ScalePx(DefaultContentInset) * 2));
        var contentSize = _contentControl?.GetPreferredSize(new Size(contentWidth, 0)) ?? Size.Empty;
        var height = ScalePx(DefaultHeaderHeight) + ScalePx(DefaultContentInset) + contentSize.Height + ScalePx(DefaultContentInset);
        return new Size(width, height);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        PaintParentBackground(e.Graphics);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        var theme = CurrentTheme;

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var bounds = ClientRectangle;
        if (bounds.Width <= 1 || bounds.Height <= 1)
        {
            return;
        }

        bounds.Inflate(-1, -1);
        bounds.Width -= 1;
        bounds.Height -= 1;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        var radius = Math.Min(ScalePx(DefaultCornerRadius), Math.Max(2, Math.Min(bounds.Width, bounds.Height) / 2));
        using var shellPath = RoundedRect(bounds, radius);
        using var fillBrush = new LinearGradientBrush(
            bounds,
            Blend(theme.Card, theme.Background, 0.06f),
            Blend(theme.Background, theme.Card, 0.03f),
            LinearGradientMode.Vertical);
        using var borderPen = new Pen(Color.FromArgb(148, Blend(theme.Border, theme.TabActiveBottom, 0.28f)));

        g.FillPath(fillBrush, shellPath);
        g.DrawPath(borderPen, shellPath);

        var inner = Rectangle.Inflate(bounds, -1, -1);
        if (inner.Width > 0 && inner.Height > 0)
        {
            using var innerPath = RoundedRect(inner, Math.Max(1, radius - 1));
            using var innerPen = new Pen(Color.FromArgb(78, theme.GridHeaderBorder));
            g.DrawPath(innerPen, innerPath);
        }

        var headerHeight = ScalePx(DefaultHeaderHeight);
        var headerRect = new Rectangle(bounds.Left + 1, bounds.Top + 1, bounds.Width - 2, Math.Max(1, headerHeight - 1));
        var state = g.Save();
        using (var headerBrush = new LinearGradientBrush(
                   headerRect,
                   Blend(theme.HeaderTop, theme.Card, 0.82f),
                   Blend(theme.HeaderBottom, theme.Background, 0.82f),
                   LinearGradientMode.Vertical))
        {
            g.SetClip(shellPath);
            g.FillRectangle(headerBrush, headerRect);
        }
        g.Restore(state);

        var headerBottom = headerRect.Bottom;
        using var dividerPen = new Pen(Color.FromArgb(112, theme.GridHeaderBorder));
        g.DrawLine(dividerPen, bounds.Left + ScalePx(6), headerBottom, bounds.Right - ScalePx(6), headerBottom);

        DrawHeader(g, bounds, theme);
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);

        var contentInset = ScalePx(DefaultContentInset);
        var headerHeight = ScalePx(DefaultHeaderHeight);

        if (_contentControl != null)
        {
            var contentBounds = new Rectangle(
                contentInset,
                headerHeight + contentInset,
                Math.Max(1, ClientSize.Width - (contentInset * 2)),
                0);
            var preferred = _contentControl.GetPreferredSize(new Size(contentBounds.Width, 0));
            contentBounds.Height = preferred.Height;
            _contentControl.Bounds = contentBounds;

            var desiredHeight = contentBounds.Bottom + contentInset;
            if (Height != desiredHeight)
            {
                Height = desiredHeight;
            }
        }
    }

    private void ContentControl_SizeChanged(object? sender, EventArgs e)
    {
        PerformLayout();
    }

    private void DrawHeader(Graphics g, Rectangle bounds, DataViewTheme theme)
    {
        var inset = ScalePx(DefaultHeaderInset);
        var titleArea = new Rectangle(
            bounds.Left + inset,
            bounds.Top,
            Math.Max(0, bounds.Width - inset * 2),
            ScalePx(DefaultHeaderHeight));

        var iconSize = ScalePx(13);
        var titleLeft = titleArea.Left;
        if (_titleIcon.HasValue)
        {
            var iconY = titleArea.Top + (titleArea.Height - iconSize) / 2;
            var accent = _titleIconColor == Color.Transparent ? theme.Accent : _titleIconColor;
            MidsTotalsIconCache.Draw(g, new Rectangle(titleLeft, iconY, iconSize, iconSize), _titleIcon.Value, accent);
            titleLeft += iconSize + ScalePx(5);
        }

        using var titleFont = new Font(Font.FontFamily, Math.Max(7.9f, Font.Size - 0.55f), FontStyle.Bold, GraphicsUnit.Point);
        using var metaFont = new Font(Font.FontFamily, Math.Max(7.1f, Font.Size - 0.80f), FontStyle.Regular, GraphicsUnit.Point);
        var titleColor = _titleColor.IsEmpty ? theme.Text : _titleColor;
        var metaColor = _metaColor.IsEmpty ? Blend(theme.Muted, theme.GridNeutral, 0.35f) : _metaColor;
        var metaWidth = string.IsNullOrWhiteSpace(_metaText)
            ? 0
            : TextRenderer.MeasureText(_metaText, metaFont, new Size(int.MaxValue, titleArea.Height),
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;

        var metaRect = new Rectangle(
            Math.Max(titleLeft, titleArea.Right - metaWidth),
            titleArea.Top,
            metaWidth,
            titleArea.Height);
        var titleRect = new Rectangle(
            titleLeft,
            titleArea.Top,
            Math.Max(0, titleArea.Width - (titleLeft - titleArea.Left) - metaWidth - ScalePx(8)),
            titleArea.Height);

        TextRenderer.DrawText(
            g,
            _title,
            titleFont,
            titleRect,
            titleColor,
            TextFormatFlags.EndEllipsis | TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter);

        if (!string.IsNullOrWhiteSpace(_metaText))
        {
            TextRenderer.DrawText(
                g,
                _metaText,
                metaFont,
                metaRect,
                metaColor,
                TextFormatFlags.EndEllipsis | TextFormatFlags.Right | TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter);
        }
    }

    private void PaintParentBackground(Graphics g)
    {
        if (Parent is null)
        {
            using var fallback = new SolidBrush(SystemColors.Control);
            g.FillRectangle(fallback, ClientRectangle);
            return;
        }

        var state = g.Save();
        try
        {
            g.TranslateTransform(-Left, -Top);
            using var pe = new PaintEventArgs(g, new Rectangle(Parent.Location, Parent.Size));
            InvokePaintBackground(Parent, pe);
        }
        finally
        {
            g.Restore(state);
        }
    }

    private int ScalePx(int value) => Math.Max(1, (int)Math.Round(value * _uiScale));

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0)
        {
            path.AddRectangle(bounds);
            path.CloseFigure();
            return path;
        }

        int diameter = radius * 2;
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

    private static Color Blend(Color a, Color b, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        float inverse = 1f - amount;
        return Color.FromArgb(
            (int)Math.Round(a.A * inverse + b.A * amount),
            (int)Math.Round(a.R * inverse + b.R * amount),
            (int)Math.Round(a.G * inverse + b.G * amount),
            (int)Math.Round(a.B * inverse + b.B * amount));
    }
}

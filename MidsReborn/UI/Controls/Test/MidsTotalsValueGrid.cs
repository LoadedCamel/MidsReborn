using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

[DesignerCategory("Code")]
internal sealed class MidsTotalsValueGrid : Control
{
    private const int CardHeight = 44;
    private const int Gap = 4;
    private const int Insets = 0;
    private const int IconSize = 12;

    private readonly List<TotalsValueMetric> _metrics = [];
    private readonly ToolTip _toolTip = new();
    private float _uiScale = 1f;
    private int _hoveredIndex = -1;

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
            Invalidate();
        }
    }

    public MidsTotalsValueGrid()
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

    public void SetMetrics(IEnumerable<TotalsValueMetric> metrics)
    {
        _metrics.Clear();
        _metrics.AddRange(metrics);
        Invalidate();
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var width = proposedSize.Width > 0 ? proposedSize.Width : Math.Max(ScalePx(320), Width);
        return new Size(width, MeasureHeight());
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        var index = HitTestCard(e.Location);
        if (_hoveredIndex == index) return;
        _hoveredIndex = index;
        var tip = index >= 0 && index < _metrics.Count ? _metrics[index].Tooltip : string.Empty;
        _toolTip.SetToolTip(this, tip);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoveredIndex = -1;
        _toolTip.SetToolTip(this, string.Empty);
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        if (Parent != null)
        {
            var state = pevent.Graphics.Save();
            try
            {
                pevent.Graphics.TranslateTransform(-Left, -Top);
                using var parentArgs = new PaintEventArgs(pevent.Graphics, Parent.ClientRectangle);
                InvokePaintBackground(Parent, parentArgs);
            }
            finally
            {
                pevent.Graphics.Restore(state);
            }
        }
        else
        {
            base.OnPaintBackground(pevent);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        var theme = CurrentTheme;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using var labelFont = new Font(Font.FontFamily, Math.Max(5.7f, Font.Size - 2.25f), FontStyle.Bold, GraphicsUnit.Point);
        using var valueFont = new Font(Font.FontFamily, Math.Max(8.15f, Font.Size - 0.55f), FontStyle.Bold, GraphicsUnit.Point);

        for (var i = 0; i < _metrics.Count; i++)
        {
            var rect = GetCardRect(i);
            if (rect.Width <= 0 || rect.Height <= 0) continue;

            using var cardPath = RoundedRect(rect, ScalePx(8));
            using var fillBrush = new LinearGradientBrush(
                rect,
                Blend(theme.Card, theme.Background, 0.12f),
                Blend(theme.Background, theme.Card, 0.04f),
                LinearGradientMode.Vertical);
            using var borderPen = new Pen(Color.FromArgb(136, theme.GridHeaderBorder));
            g.FillPath(fillBrush, cardPath);
            g.DrawPath(borderPen, cardPath);

            var metric = _metrics[i];
            var inset = ScalePx(6);
            var iconSize = ScalePx(IconSize);
            var iconRect = new Rectangle(rect.Left + inset, rect.Top + ScalePx(9), iconSize, iconSize);
            using var iconBack = new SolidBrush(Color.FromArgb(16, metric.AccentColor));
            using var iconStroke = new Pen(Color.FromArgb(84, metric.AccentColor));
            g.FillEllipse(iconBack, iconRect);
            g.DrawEllipse(iconStroke, iconRect);
            MidsTotalsIconCache.Draw(g, Rectangle.Inflate(iconRect, -ScalePx(1), -ScalePx(1)), metric.Icon, metric.AccentColor);

            var textLeft = iconRect.Right + ScalePx(4);
            var labelRect = new Rectangle(textLeft, rect.Top + ScalePx(5), Math.Max(1, rect.Right - textLeft - inset), ScalePx(12));
            var valueRect = new Rectangle(textLeft, labelRect.Bottom, labelRect.Width, ScalePx(18));

            TextRenderer.DrawText(
                g,
                metric.Label.ToUpperInvariant(),
                labelFont,
                labelRect,
                Blend(theme.Muted, Color.White, 0.14f),
                TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);
            TextRenderer.DrawText(
                g,
                metric.Value,
                valueFont,
                valueRect,
                metric.AccentColor,
                TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);
        }
    }

    private Rectangle GetCardRect(int index)
    {
        var inset = ScalePx(Insets);
        var gap = ScalePx(Gap);
        var cardHeight = ScalePx(CardHeight);
        var columnCount = 3;
        var row = index / columnCount;
        var column = index % columnCount;
        var availableWidth = Math.Max(1, ClientSize.Width - inset * 2 - gap * (columnCount - 1));
        var cardWidth = Math.Max(ScalePx(86), availableWidth / columnCount);
        var x = inset + column * (cardWidth + gap);
        var y = inset + row * (cardHeight + gap);
        return new Rectangle(x, y, cardWidth, cardHeight);
    }

    private int HitTestCard(Point location)
    {
        for (var i = 0; i < _metrics.Count; i++)
        {
            if (GetCardRect(i).Contains(location))
            {
                return i;
            }
        }

        return -1;
    }

    private int MeasureHeight()
    {
        if (_metrics.Count == 0)
        {
            return 0;
        }

        var inset = ScalePx(Insets);
        var gap = ScalePx(Gap);
        var cardHeight = ScalePx(CardHeight);
        var rows = (int)Math.Ceiling(_metrics.Count / 3d);
        return inset * 2 + rows * cardHeight + (rows - 1) * gap;
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

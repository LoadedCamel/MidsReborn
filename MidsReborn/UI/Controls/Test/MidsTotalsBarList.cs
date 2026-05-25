using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

[DesignerCategory("Code")]
internal sealed class MidsTotalsBarList : Control
{
    private const int RowHeight = 18;
    private const int RowGap = 2;
    private const int Insets = 0;
    private const int IconSize = 12;

    private readonly List<TotalsBarMetric> _metrics = [];
    private readonly ToolTip _toolTip = new();
    private float _uiScale = 1f;
    private float _scaleMax = 100f;
    private string? _activeTooltip;

    public enum FillPalette
    {
        Defense,
        Resistance
    }

    public float UiScale
    {
        get => _uiScale;
        set
        {
            var clamped = Math.Clamp(value, 0.90f, 1.25f);
            if (Math.Abs(_uiScale - clamped) < 0.01f) return;
            _uiScale = clamped;
            Invalidate();
        }
    }

    public float ScaleMax
    {
        get => _scaleMax;
        set
        {
            var clamped = Math.Max(1f, value);
            if (Math.Abs(_scaleMax - clamped) < 0.01f) return;
            _scaleMax = clamped;
            Invalidate();
        }
    }

    public FillPalette Palette { get; set; } = FillPalette.Defense;

    private DataViewTheme CurrentTheme =>
        DesignMode
            ? ThemeManager.DesignTime.DataView
            : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

    public MidsTotalsBarList()
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

    public void SetMetrics(IEnumerable<TotalsBarMetric> metrics)
    {
        _metrics.Clear();
        _metrics.AddRange(metrics);
        Invalidate();
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var width = proposedSize.Width > 0 ? proposedSize.Width : Math.Max(ScalePx(160), Width);
        return new Size(width, MeasureHeight());
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        var row = HitTestRow(e.Location);
        var tooltip = row >= 0 && row < _metrics.Count ? _metrics[row].Tooltip : string.Empty;
        if (tooltip == _activeTooltip) return;
        _activeTooltip = tooltip;
        _toolTip.SetToolTip(this, tooltip);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _activeTooltip = null;
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
                InvokePaint(Parent, parentArgs);
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

        using var labelFont = new Font(Font.FontFamily, Math.Max(6.45f, Font.Size - 1.8f), FontStyle.Bold, GraphicsUnit.Point);
        using var valueFont = new Font(Font.FontFamily, Math.Max(6.9f, Font.Size - 1.35f), FontStyle.Bold, GraphicsUnit.Point);

        var inset = ScalePx(Insets);
        var rowHeight = ScalePx(RowHeight);
        var rowGap = ScalePx(RowGap);
        var iconSize = ScalePx(IconSize);
        var rowWidth = Math.Max(1, ClientSize.Width - inset * 2);
        var labelWidth = MeasureLabelWidth(labelFont, rowWidth);
        var valueWidth = Math.Min(ScalePx(48), Math.Max(ScalePx(38), rowWidth / 5));
        var barGap = ScalePx(2);
        var barWidth = Math.Max(ScalePx(46), rowWidth - labelWidth - valueWidth - iconSize - barGap * 3);

        for (var i = 0; i < _metrics.Count; i++)
        {
            var metric = _metrics[i];
            var rowRect = new Rectangle(inset, inset + i * (rowHeight + rowGap), rowWidth, rowHeight);
            var iconRect = new Rectangle(rowRect.Left, rowRect.Top + (rowHeight - iconSize) / 2, iconSize, iconSize);
            var labelRect = new Rectangle(iconRect.Right + barGap, rowRect.Top, labelWidth, rowHeight);
            var barRect = new Rectangle(labelRect.Right + barGap, rowRect.Top + ScalePx(4), barWidth, Math.Max(7, ScalePx(9)));
            var valueRect = new Rectangle(barRect.Right + barGap, rowRect.Top, valueWidth, rowHeight);

            MidsTotalsIconCache.Draw(g, iconRect, metric.Icon, metric.AccentColor);

            TextRenderer.DrawText(
                g,
                metric.Label,
                labelFont,
                labelRect,
                theme.Text,
                TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);

            DrawRail(g, barRect, theme, metric);

            TextRenderer.DrawText(
                g,
                metric.Value,
                valueFont,
                valueRect,
                theme.Text,
                TextFormatFlags.Right | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
        }
    }

    private void DrawRail(Graphics g, Rectangle rect, DataViewTheme theme, TotalsBarMetric metric)
    {
        using var railPath = RoundedRect(rect, Math.Max(4, rect.Height / 2));
        using var railBrush = new LinearGradientBrush(
            rect,
            Color.FromArgb(42, 49, 67),
            Color.FromArgb(27, 31, 43),
            LinearGradientMode.Vertical);
        using var railPen = new Pen(Color.FromArgb(118, theme.GridHeaderBorder));

        g.FillPath(railBrush, railPath);
        g.DrawPath(railPen, railPath);

        var fillRatio = Math.Clamp(metric.BarValue / Math.Max(1f, _scaleMax), 0f, 1f);
        var fillWidth = Math.Max(0, (int)Math.Round((rect.Width - 4) * fillRatio));
        if (fillWidth > 0)
        {
            var fillRect = new Rectangle(rect.Left + 1, rect.Top + 1, fillWidth, Math.Max(2, rect.Height - 2));
            using var fillPath = RoundedRect(fillRect, Math.Max(2, fillRect.Height / 2));
            var (fillTop, fillBottom) = GetPaletteColors(theme);
            using var fillBrush = new LinearGradientBrush(fillRect, fillTop, fillBottom, LinearGradientMode.Vertical);
            g.FillPath(fillBrush, fillPath);
        }

        var markerRatio = Math.Clamp(metric.MarkerValue / Math.Max(1f, _scaleMax), 0f, 1f);
        var markerX = rect.Left + 1 + (int)Math.Round((rect.Width - 2) * markerRatio);
        using var markerPen = new Pen(Color.FromArgb(224, 232, 184, 74), Math.Max(1, ScalePx(2)));
        g.DrawLine(markerPen, markerX, rect.Top - ScalePx(1), markerX, rect.Bottom + ScalePx(1));
    }

    private (Color top, Color bottom) GetPaletteColors(DataViewTheme theme)
    {
        return Palette switch
        {
            FillPalette.Resistance => (Color.FromArgb(86, 216, 230), Color.FromArgb(44, 170, 188)),
            _ => (Color.FromArgb(214, 62, 226), Color.FromArgb(156, 36, 192))
        };
    }

    private int HitTestRow(Point location)
    {
        var inset = ScalePx(Insets);
        var rowHeight = ScalePx(RowHeight);
        var rowGap = ScalePx(RowGap);
        for (var i = 0; i < _metrics.Count; i++)
        {
            var rowRect = new Rectangle(inset, inset + i * (rowHeight + rowGap), Math.Max(1, ClientSize.Width - inset * 2), rowHeight);
            if (rowRect.Contains(location))
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
        var rowHeight = ScalePx(RowHeight);
        var rowGap = ScalePx(RowGap);
        return inset * 2 + _metrics.Count * rowHeight + (_metrics.Count - 1) * rowGap;
    }

    private int ScalePx(int value) => Math.Max(1, (int)Math.Round(value * _uiScale));

    private int MeasureLabelWidth(Font labelFont, int rowWidth)
    {
        var max = ScalePx(38);
        foreach (var metric in _metrics)
        {
            var measured = TextRenderer.MeasureText(
                metric.Label,
                labelFont,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;
            max = Math.Max(max, measured);
        }

        return Math.Min(ScalePx(48), Math.Max(ScalePx(36), max + ScalePx(1)));
    }

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

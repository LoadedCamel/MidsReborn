using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

[DesignerCategory("Code")]
internal sealed class MidsTotalsQuickChip : Control
{
    private const int CornerRadius = 8;
    private const int Insets = 7;
    private const int IconSize = 15;

    private readonly ToolTip _toolTip = new();
    private float _uiScale = 1f;
    private TotalsQuickMetric _metric;

    private DataViewTheme CurrentTheme =>
        DesignMode
            ? ThemeManager.DesignTime.DataView
            : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

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

    public TotalsQuickMetric Metric
    {
        get => _metric;
        set
        {
            _metric = value;
            _toolTip.SetToolTip(this, value.Tooltip);
            Invalidate();
        }
    }

    public MidsTotalsQuickChip()
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
        Size = new Size(84, 50);
        TabStop = false;
    }

    public override Size GetPreferredSize(Size proposedSize)
        => new(Math.Max(ScalePx(78), proposedSize.Width), ScalePx(50));

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

        var bounds = ClientRectangle;
        bounds.Inflate(-1, -1);
        bounds.Width -= 1;
        bounds.Height -= 1;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        using var shellPath = RoundedRect(bounds, ScalePx(CornerRadius));
        using var fillBrush = new LinearGradientBrush(
            bounds,
            Color.FromArgb(14, 21, 31),
            Color.FromArgb(8, 13, 21),
            LinearGradientMode.Vertical);
        using var borderPen = new Pen(Color.FromArgb(120, 64, 96, 132));
        g.FillPath(fillBrush, shellPath);
        g.DrawPath(borderPen, shellPath);

        var inset = ScalePx(Insets);
        var iconSize = ScalePx(IconSize);
        var iconRect = new Rectangle(bounds.Left + inset, bounds.Top + ScalePx(23), iconSize, iconSize);
        using var iconBack = new SolidBrush(Color.FromArgb(12, _metric.AccentColor));
        using var iconStroke = new Pen(Color.FromArgb(148, _metric.AccentColor));
        g.FillEllipse(iconBack, iconRect);
        g.DrawEllipse(iconStroke, iconRect);
        var glyphRect = Rectangle.Inflate(iconRect, -ScalePx(2), -ScalePx(2));
        MidsTotalsIconCache.Draw(g, glyphRect, _metric.Icon, _metric.AccentColor);

        var labelRect = new Rectangle(bounds.Left + inset, bounds.Top + ScalePx(3), Math.Max(1, bounds.Width - inset * 2), ScalePx(11));
        var valueLeft = iconRect.Right + ScalePx(6);
        var valueWidth = Math.Max(1, bounds.Right - valueLeft - inset);
        var valueRect = new Rectangle(valueLeft, iconRect.Top - ScalePx(1), valueWidth, ScalePx(17));
        var detailRect = new Rectangle(valueLeft, valueRect.Bottom - ScalePx(1), valueWidth, ScalePx(10));

        using var labelFont = new Font(Font.FontFamily, Math.Max(6.1f, Font.Size - 2.0f), FontStyle.Bold, GraphicsUnit.Point);
        using var valueFont = new Font(Font.FontFamily, Math.Max(10.1f, Font.Size + 0.55f), FontStyle.Bold, GraphicsUnit.Point);
        using var detailFont = new Font(Font.FontFamily, Math.Max(6.1f, Font.Size - 1.95f), FontStyle.Regular, GraphicsUnit.Point);

        TextRenderer.DrawText(
            g,
            _metric.Label.ToUpperInvariant(),
            labelFont,
            labelRect,
            Blend(theme.Muted, Color.White, 0.18f),
            TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);

        TextRenderer.DrawText(
            g,
            _metric.Value,
            valueFont,
            valueRect,
            _metric.AccentColor,
            TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);

        if (!string.IsNullOrWhiteSpace(_metric.Detail))
        {
            TextRenderer.DrawText(
                g,
                _metric.Detail,
                detailFont,
                detailRect,
                theme.Muted,
                TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
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

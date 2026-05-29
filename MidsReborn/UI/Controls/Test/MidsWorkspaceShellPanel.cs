using System.ComponentModel;
using System.Drawing.Drawing2D;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls.Test;

[DesignerCategory("Code")]
internal sealed class MidsWorkspaceShellPanel : Panel
{
    private const int DefaultCornerRadius = 8;
    private const int DefaultBorderThickness = 1;

    private int _cornerRadius = DefaultCornerRadius;
    private int _borderThickness = DefaultBorderThickness;
    private bool _showInnerBorder = true;

    private DataViewTheme CurrentTheme =>
        DesignMode
            ? ThemeManager.DesignTime.DataView
            : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

    public int CornerRadius
    {
        get => _cornerRadius;
        set
        {
            value = Math.Max(0, value);
            if (_cornerRadius == value) return;
            _cornerRadius = value;
            Invalidate();
        }
    }

    public int BorderThickness
    {
        get => _borderThickness;
        set
        {
            value = Math.Max(0, value);
            if (_borderThickness == value) return;
            _borderThickness = value;
            Invalidate();
        }
    }

    public bool ShowInnerBorder
    {
        get => _showInnerBorder;
        set
        {
            if (_showInnerBorder == value) return;
            _showInnerBorder = value;
            Invalidate();
        }
    }

    public MidsWorkspaceShellPanel()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint |
            ControlStyles.SupportsTransparentBackColor,
            true);

        BackColor = Color.Transparent;
        Padding = new Padding(8);
        TabStop = false;
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

        var stroke = Math.Max(1, _borderThickness);
        bounds.Inflate(-stroke / 2, -stroke / 2);
        bounds.Width -= 1;
        bounds.Height -= 1;

        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        var radius = Math.Min(Math.Max(1, _cornerRadius), Math.Max(2, Math.Min(bounds.Width, bounds.Height) / 2));
        using var shellPath = RoundedRect(bounds, radius);
        using var fillBrush = new LinearGradientBrush(
            bounds,
            Blend(theme.Card, theme.Background, 0.16f),
            theme.Background,
            LinearGradientMode.Vertical);
        using var borderPen = new Pen(Blend(theme.Border, theme.TabActiveBottom, 0.50f), stroke);

        g.FillPath(fillBrush, shellPath);
        g.DrawPath(borderPen, shellPath);

        if (_showInnerBorder)
        {
            var inner = Rectangle.Inflate(bounds, -1, -1);
            if (inner.Width > 0 && inner.Height > 0)
            {
                using var innerPath = RoundedRect(inner, Math.Max(1, radius - 1));
                using var innerPen = new Pen(Color.FromArgb(100, theme.GridHeaderBorder));
                g.DrawPath(innerPen, innerPath);
            }
        }

        base.OnPaint(e);
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

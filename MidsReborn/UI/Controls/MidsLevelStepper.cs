using System.ComponentModel;
using System.Drawing.Drawing2D;
using Mids_Reborn.Core.Theming;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

[DefaultEvent(nameof(ValueChanged))]
[DesignerCategory("Code")]
public sealed class MidsLevelStepper : Control
{
    private enum HotZone
    {
        None,
        Decrement,
        Increment
    }

    private int _minimum = 1;
    private int _maximum = 50;
    private int _value = 50;
    private HotZone _hoveredZone;
    private HotZone _pressedZone;
    private bool _themeHooked;
    private Action? _themeChangedHandler;

    public event EventHandler? ValueChanged;

    [DefaultValue(1)]
    public int Minimum
    {
        get => _minimum;
        set
        {
            if (_minimum == value)
            {
                return;
            }

            _minimum = value;
            if (_maximum < _minimum)
            {
                _maximum = _minimum;
            }

            Value = Math.Clamp(_value, _minimum, _maximum);
            Invalidate();
        }
    }

    [DefaultValue(50)]
    public int Maximum
    {
        get => _maximum;
        set
        {
            if (_maximum == value)
            {
                return;
            }

            _maximum = Math.Max(value, _minimum);
            Value = Math.Clamp(_value, _minimum, _maximum);
            Invalidate();
        }
    }

    [DefaultValue(50)]
    public int Value
    {
        get => _value;
        set
        {
            var normalized = Math.Clamp(value, _minimum, _maximum);
            if (_value == normalized)
            {
                return;
            }

            _value = normalized;
            Invalidate();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private DropDownListTheme CurrentTheme
    {
        get
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return ThemeManager.DesignTime.DropDownList;
            }

            return ThemeManager.CurrentTheme?.DropDownList ?? ThemeManager.DesignTime.DropDownList;
        }
    }

    public MidsLevelStepper()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor |
            ControlStyles.UserPaint,
            true);

        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
        Font = new Font("Segoe UI Semibold", 9.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        ForeColor = Color.White;
        Size = new Size(82, 28);
        TabStop = true;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (_themeHooked || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            return;
        }

        _themeChangedHandler = Invalidate;
        ThemeManager.ThemeChanged += _themeChangedHandler;
        _themeHooked = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _themeHooked && _themeChangedHandler is not null)
        {
            ThemeManager.ThemeChanged -= _themeChangedHandler;
            _themeHooked = false;
            _themeChangedHandler = null;
        }

        base.Dispose(disposing);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoveredZone = HotZone.None;
        _pressedZone = HotZone.None;
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        var hovered = HitTest(e.Location);
        if (_hoveredZone == hovered)
        {
            return;
        }

        _hoveredZone = hovered;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        Focus();
        _pressedZone = HitTest(e.Location);
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        var hit = HitTest(e.Location);
        var pressed = _pressedZone;
        _pressedZone = HotZone.None;

        if (hit == pressed)
        {
            switch (hit)
            {
                case HotZone.Decrement:
                    StepValue(-1);
                    break;
                case HotZone.Increment:
                    StepValue(1);
                    break;
            }
        }

        Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        if (e.Delta > 0)
        {
            StepValue(1);
        }
        else if (e.Delta < 0)
        {
            StepValue(-1);
        }
    }

    protected override bool IsInputKey(Keys keyData)
    {
        return keyData switch
        {
            Keys.Left or Keys.Right or Keys.Up or Keys.Down => true,
            _ => base.IsInputKey(keyData)
        };
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        switch (e.KeyCode)
        {
            case Keys.Left:
            case Keys.Down:
                StepValue(-1);
                e.Handled = true;
                break;
            case Keys.Right:
            case Keys.Up:
                StepValue(1);
                e.Handled = true;
                break;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var theme = CurrentTheme;
        var bounds = new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1));
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        int radius = Math.Min(7, Math.Max(2, bounds.Height / 2));
        using var outerPath = CreateRoundedRect(bounds, radius);
        using (var fillBrush = new LinearGradientBrush(bounds, theme.GradientTop, theme.GradientBottom, 90f))
        {
            e.Graphics.FillPath(fillBrush, outerPath);
        }

        DrawZone(e.Graphics, DecrementBounds, HotZone.Decrement, theme, roundLeft: true, roundRight: false);
        DrawZone(e.Graphics, IncrementBounds, HotZone.Increment, theme, roundLeft: false, roundRight: true);

        using (var borderPen = new Pen(theme.Border))
        {
            e.Graphics.DrawPath(borderPen, outerPath);
        }

        using (var clipRegion = new Region(outerPath))
        {
            e.Graphics.SetClip(clipRegion, CombineMode.Replace);
            using var dividerPen = new Pen(Color.FromArgb(120, theme.Border));
            e.Graphics.DrawLine(dividerPen, DecrementBounds.Right, 4, DecrementBounds.Right, Height - 5);
            e.Graphics.DrawLine(dividerPen, IncrementBounds.Left, 4, IncrementBounds.Left, Height - 5);
            e.Graphics.ResetClip();
        }

        DrawSymbol(e.Graphics, DecrementBounds, "-", CanDecrement ? theme.ForeColor : Color.FromArgb(110, theme.ForeColor));
        DrawSymbol(e.Graphics, IncrementBounds, "+", CanIncrement ? theme.ForeColor : Color.FromArgb(110, theme.ForeColor));
        DrawValue(e.Graphics, ValueBounds, theme.ForeColor);

        if (Focused)
        {
            var focusRect = Rectangle.Inflate(bounds, -2, -2);
            using var focusPath = CreateRoundedRect(focusRect, Math.Max(2, radius - 2));
            using var focusPen = new Pen(Color.FromArgb(170, theme.HoverBorder));
            e.Graphics.DrawPath(focusPen, focusPath);
        }
    }

    private Rectangle DecrementBounds
    {
        get
        {
            int buttonWidth = Math.Max(20, Math.Min(24, Width / 4));
            return new Rectangle(0, 0, buttonWidth, Height - 1);
        }
    }

    private Rectangle IncrementBounds
    {
        get
        {
            int buttonWidth = DecrementBounds.Width;
            return new Rectangle(Width - 1 - buttonWidth, 0, buttonWidth, Height - 1);
        }
    }

    private Rectangle ValueBounds
        => Rectangle.FromLTRB(DecrementBounds.Right, 0, IncrementBounds.Left, Height - 1);

    private bool CanDecrement => _value > _minimum;
    private bool CanIncrement => _value < _maximum;

    private void StepValue(int delta)
    {
        if (delta < 0 && !CanDecrement || delta > 0 && !CanIncrement)
        {
            return;
        }

        Value += delta;
    }

    private HotZone HitTest(Point location)
    {
        if (DecrementBounds.Contains(location))
        {
            return HotZone.Decrement;
        }

        if (IncrementBounds.Contains(location))
        {
            return HotZone.Increment;
        }

        return HotZone.None;
    }

    private void DrawZone(Graphics graphics, Rectangle bounds, HotZone zone, DropDownListTheme theme, bool roundLeft, bool roundRight)
    {
        bool isHovered = zone == _hoveredZone;
        bool isPressed = zone == _pressedZone;
        bool isDisabled = zone == HotZone.Decrement ? !CanDecrement : !CanIncrement;
        if (!isHovered && !isPressed && !isDisabled)
        {
            return;
        }

        var top = isPressed
            ? Blend(theme.HoverGradientBottom, theme.GradientBottom, 0.30f)
            : isHovered
                ? theme.HoverGradientTop
                : Blend(theme.GradientTop, Color.Black, 0.12f);
        var bottom = isPressed
            ? Blend(theme.HoverGradientTop, theme.GradientTop, 0.30f)
            : isHovered
                ? theme.HoverGradientBottom
                : Blend(theme.GradientBottom, Color.Black, 0.12f);

        using var zonePath = CreateSegmentPath(bounds, 7, roundLeft, roundRight);
        using var zoneBrush = new LinearGradientBrush(bounds, top, bottom, 90f);
        graphics.FillPath(zoneBrush, zonePath);
    }

    private void DrawSymbol(Graphics graphics, Rectangle bounds, string symbol, Color color)
    {
        var textRect = Rectangle.Inflate(bounds, -1, -1);
        TextRenderer.DrawText(
            graphics,
            symbol,
            Font,
            new Rectangle(textRect.X, textRect.Y + 1, textRect.Width, textRect.Height),
            Color.FromArgb(150, Color.Black),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        TextRenderer.DrawText(
            graphics,
            symbol,
            Font,
            textRect,
            color,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    private void DrawValue(Graphics graphics, Rectangle bounds, Color color)
    {
        var text = _value.ToString();
        var textRect = Rectangle.Inflate(bounds, -2, -1);
        TextRenderer.DrawText(
            graphics,
            text,
            Font,
            new Rectangle(textRect.X, textRect.Y + 1, textRect.Width, textRect.Height),
            Color.FromArgb(150, Color.Black),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        TextRenderer.DrawText(
            graphics,
            text,
            Font,
            textRect,
            color,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    private static GraphicsPath CreateRoundedRect(Rectangle bounds, int radius)
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

    private static GraphicsPath CreateSegmentPath(Rectangle bounds, int radius, bool roundLeft, bool roundRight)
    {
        var path = new GraphicsPath();
        int clampedRadius = Math.Min(radius, Math.Max(2, bounds.Height / 2));

        if (!roundLeft && !roundRight)
        {
            path.AddRectangle(bounds);
            path.CloseFigure();
            return path;
        }

        int diameter = clampedRadius * 2;
        if (roundLeft)
        {
            var leftArc = new Rectangle(bounds.Left, bounds.Top, diameter, diameter);
            path.AddArc(leftArc, 180, 90);
        }
        else
        {
            path.AddLine(bounds.Left, bounds.Top, bounds.Left, bounds.Top);
        }

        path.AddLine(bounds.Left + (roundLeft ? clampedRadius : 0), bounds.Top, bounds.Right - (roundRight ? clampedRadius : 0), bounds.Top);

        if (roundRight)
        {
            var rightArc = new Rectangle(bounds.Right - diameter, bounds.Top, diameter, diameter);
            path.AddArc(rightArc, 270, 90);
        }
        else
        {
            path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Top);
        }

        path.AddLine(bounds.Right, bounds.Top + (roundRight ? clampedRadius : 0), bounds.Right, bounds.Bottom - (roundRight ? clampedRadius : 0));

        if (roundRight)
        {
            var rightArc = new Rectangle(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter);
            path.AddArc(rightArc, 0, 90);
        }
        else
        {
            path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom);
        }

        path.AddLine(bounds.Right - (roundRight ? clampedRadius : 0), bounds.Bottom, bounds.Left + (roundLeft ? clampedRadius : 0), bounds.Bottom);

        if (roundLeft)
        {
            var leftArc = new Rectangle(bounds.Left, bounds.Bottom - diameter, diameter, diameter);
            path.AddArc(leftArc, 90, 90);
        }
        else
        {
            path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Bottom);
        }

        path.CloseFigure();
        return path;
    }

    private static Color Blend(Color first, Color second, float amountSecond)
    {
        amountSecond = Math.Clamp(amountSecond, 0f, 1f);
        float amountFirst = 1f - amountSecond;
        return Color.FromArgb(
            (int)Math.Round(first.A * amountFirst + second.A * amountSecond),
            (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
            (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
            (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
    }
}

using System.ComponentModel;
using System.Globalization;
using System.Drawing.Drawing2D;
using Mids_Reborn.Core.Theming;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

[DefaultEvent(nameof(ValueChanged))]
[DesignerCategory("Code")]
public sealed class MidsLevelStepper : Control, ILiveResizeMetricsAware
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
    private bool _suppressEditorLostFocusCommit;
    private bool _themeHooked;
    private bool _liveResizeMetricsFrozen;
    private bool _stepperLayoutValid;
    private Action? _themeChangedHandler;
    private StepperLayoutKey _stepperLayoutKey;
    private StepperLayout _stepperLayout;
    private readonly TextBox _editor;

    private readonly record struct FontSignature(string FamilyName, float SizeInPoints, FontStyle Style, byte GdiCharSet);
    private readonly record struct StepperLayoutKey(int ClientHeight, FontSignature Font, int Dpi);
    private readonly record struct StepperLayout(
        int Radius,
        int DividerTop,
        int DividerBottom,
        int ShadowOffsetY,
        int SymbolInflateX,
        int SymbolInflateY,
        int ValueInflateX,
        int ValueInflateY,
        int FocusInset,
        int FocusRadiusDelta);

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
            if (!_editor.Focused)
            {
                _editor.Text = _value.ToString(CultureInfo.InvariantCulture);
            }

            Invalidate();
            if (IsHandleCreated && Visible)
            {
                Update();
            }

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

        _editor = new TextBox
        {
            BorderStyle = BorderStyle.None,
            HideSelection = false,
            Margin = Padding.Empty,
            Multiline = false,
            TabStop = false,
            TextAlign = HorizontalAlignment.Center,
            Visible = false
        };
        _editor.KeyDown += Editor_KeyDown;
        _editor.KeyPress += Editor_KeyPress;
        _editor.LostFocus += Editor_LostFocus;
        Controls.Add(_editor);

        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
        Font = new Font("Segoe UI Semibold", 9.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        ForeColor = Color.White;
        Size = new Size(82, 28);
        TabStop = true;

        UpdateEditorConstraints();
        ApplyEditorTheme();
        UpdateEditorBounds();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (_themeHooked || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            return;
        }

        _themeChangedHandler = HandleThemeChanged;
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

        if (disposing)
        {
            _editor.KeyDown -= Editor_KeyDown;
            _editor.KeyPress -= Editor_KeyPress;
            _editor.LostFocus -= Editor_LostFocus;
        }

        base.Dispose(disposing);
    }

    public void BeginLiveResizeMetrics()
    {
        _liveResizeMetricsFrozen = true;
        Invalidate();
    }

    public void EndLiveResizeMetrics()
    {
        if (!_liveResizeMetricsFrozen)
        {
            return;
        }

        _liveResizeMetricsFrozen = false;
        Invalidate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        _stepperLayoutValid = false;
        if (_editor is null)
        {
            return;
        }

        _editor.Font = Font;
        UpdateEditorBounds();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        if (_editor is null)
        {
            return;
        }

        UpdateEditorBounds();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoveredZone = HotZone.None;
        _pressedZone = HotZone.None;
        if (!_editor.Focused)
        {
            Cursor = Cursors.Hand;
        }
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        var hovered = HitTest(e.Location);
        Cursor = hovered == HotZone.None && ValueBounds.Contains(e.Location) && !_editor.Visible
            ? Cursors.IBeam
            : Cursors.Hand;
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
        var hit = HitTest(e.Location);
        if (hit == HotZone.None && ValueBounds.Contains(e.Location))
        {
            BeginValueEdit(selectAll: true);
            return;
        }

        CommitValueEdit(keepFocusOnStepper: false);
        _pressedZone = hit;
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
        if (_editor.Visible)
        {
            return;
        }

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
            case Keys.Enter:
            case Keys.F2:
                BeginValueEdit(selectAll: true);
                e.Handled = true;
                break;
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

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        base.OnKeyPress(e);
        if (_editor.Visible)
        {
            return;
        }

        if (char.IsDigit(e.KeyChar))
        {
            BeginValueEdit(e.KeyChar.ToString(CultureInfo.InvariantCulture));
            e.Handled = true;
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

        var layout = GetStepperLayout(e.Graphics);
        int radius = layout.Radius;
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
            e.Graphics.DrawLine(dividerPen, DecrementBounds.Right, layout.DividerTop, DecrementBounds.Right, layout.DividerBottom);
            e.Graphics.DrawLine(dividerPen, IncrementBounds.Left, layout.DividerTop, IncrementBounds.Left, layout.DividerBottom);
            e.Graphics.ResetClip();
        }

        DrawSymbol(e.Graphics, DecrementBounds, layout, "-", CanDecrement ? theme.ForeColor : Color.FromArgb(110, theme.ForeColor));
        DrawSymbol(e.Graphics, IncrementBounds, layout, "+", CanIncrement ? theme.ForeColor : Color.FromArgb(110, theme.ForeColor));
        if (!_editor.Visible)
        {
            DrawValue(e.Graphics, ValueBounds, layout, theme.ForeColor);
        }

        if (Focused || _editor.Focused)
        {
            var focusRect = Rectangle.Inflate(bounds, -layout.FocusInset, -layout.FocusInset);
            using var focusPath = CreateRoundedRect(focusRect, Math.Max(2, radius - layout.FocusRadiusDelta));
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
        CommitValueEdit(keepFocusOnStepper: false);
        if (delta < 0 && !CanDecrement || delta > 0 && !CanIncrement)
        {
            return;
        }

        Value += delta;
    }

    private void BeginValueEdit(string? text = null, bool selectAll = false)
    {
        ApplyEditorTheme();
        UpdateEditorConstraints();
        UpdateEditorBounds();
        _editor.Text = text ?? _value.ToString(CultureInfo.InvariantCulture);
        _editor.Visible = true;
        _editor.BringToFront();
        _editor.Focus();
        if (selectAll)
        {
            _editor.SelectAll();
        }
        else
        {
            _editor.SelectionStart = _editor.TextLength;
        }

        Cursor = Cursors.IBeam;
        Invalidate();
    }

    private void CommitValueEdit(bool keepFocusOnStepper = true)
    {
        if (!_editor.Visible)
        {
            return;
        }

        var text = _editor.Text.Trim();
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            Value = parsed;
        }
        else
        {
            _editor.Text = _value.ToString(CultureInfo.InvariantCulture);
        }

        EndValueEdit(keepFocusOnStepper);
    }

    private void CancelValueEdit(bool keepFocusOnStepper = true)
    {
        if (!_editor.Visible)
        {
            return;
        }

        _editor.Text = _value.ToString(CultureInfo.InvariantCulture);
        EndValueEdit(keepFocusOnStepper);
    }

    private void EndValueEdit(bool keepFocusOnStepper)
    {
        _suppressEditorLostFocusCommit = true;
        try
        {
            _editor.Visible = false;
            if (keepFocusOnStepper && IsHandleCreated && CanFocus)
            {
                Focus();
            }
        }
        finally
        {
            _suppressEditorLostFocusCommit = false;
        }

        Cursor = Cursors.Hand;
        Invalidate();
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

    private void DrawSymbol(Graphics graphics, Rectangle bounds, StepperLayout layout, string symbol, Color color)
    {
        var textRect = Rectangle.Inflate(bounds, -layout.SymbolInflateX, -layout.SymbolInflateY);
        TextRenderer.DrawText(
            graphics,
            symbol,
            Font,
            new Rectangle(textRect.X, textRect.Y + layout.ShadowOffsetY, textRect.Width, textRect.Height),
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

    private void DrawValue(Graphics graphics, Rectangle bounds, StepperLayout layout, Color color)
    {
        var text = _value.ToString(CultureInfo.InvariantCulture);
        var textRect = Rectangle.Inflate(bounds, -layout.ValueInflateX, -layout.ValueInflateY);
        TextRenderer.DrawText(
            graphics,
            text,
            Font,
            new Rectangle(textRect.X, textRect.Y + layout.ShadowOffsetY, textRect.Width, textRect.Height),
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

    private StepperLayout GetStepperLayout(Graphics? graphics = null)
    {
        var key = new StepperLayoutKey(
            ClientSize.Height,
            CreateFontSignature(Font),
            graphics is not null ? (int)Math.Round(graphics.DpiY) : DeviceDpi);

        if (_stepperLayoutValid && _stepperLayoutKey == key)
        {
            return _stepperLayout;
        }

        int radius = Math.Min(7, Math.Max(2, Math.Max(1, Height - 1) / 2));
        _stepperLayout = new StepperLayout(
            radius,
            4,
            Math.Max(4, Height - 5),
            1,
            1,
            1,
            2,
            1,
            2,
            2);
        _stepperLayoutKey = key;
        _stepperLayoutValid = true;
        return _stepperLayout;
    }

    private static FontSignature CreateFontSignature(Font font)
        => new(font.FontFamily.Name, font.SizeInPoints, font.Style, font.GdiCharSet);

    private void HandleThemeChanged()
    {
        ApplyEditorTheme();
        Invalidate();
    }

    private void ApplyEditorTheme()
    {
        if (_editor.IsDisposed)
        {
            return;
        }

        var theme = CurrentTheme;
        _editor.BackColor = Blend(theme.GradientTop, theme.GradientBottom, 0.5f);
        _editor.ForeColor = theme.ForeColor;
        _editor.Font = Font;
    }

    private void UpdateEditorBounds()
    {
        if (_editor.IsDisposed)
        {
            return;
        }

        var valueBounds = Rectangle.Inflate(ValueBounds, -4, -4);
        int editorHeight = Math.Min(valueBounds.Height, Math.Max(16, Font.Height + 4));
        int editorY = valueBounds.Y + Math.Max(0, (valueBounds.Height - editorHeight) / 2);
        _editor.Bounds = new Rectangle(
            Math.Max(0, valueBounds.X),
            Math.Max(0, editorY),
            Math.Max(8, valueBounds.Width),
            Math.Max(1, editorHeight));
    }

    private void UpdateEditorConstraints()
    {
        var digitLength = Math.Max(
            _minimum.ToString(CultureInfo.InvariantCulture).Length,
            _maximum.ToString(CultureInfo.InvariantCulture).Length);
        _editor.MaxLength = Math.Max(1, digitLength);
    }

    private void Editor_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Enter:
                CommitValueEdit();
                e.Handled = true;
                e.SuppressKeyPress = true;
                break;
            case Keys.Escape:
                CancelValueEdit();
                e.Handled = true;
                e.SuppressKeyPress = true;
                break;
        }
    }

    private void Editor_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
        {
            return;
        }

        bool canInsertNegativeSign = e.KeyChar == '-'
            && _minimum < 0
            && _editor.SelectionStart == 0
            && !_editor.Text.Contains('-', StringComparison.Ordinal);
        if (canInsertNegativeSign)
        {
            return;
        }

        e.Handled = true;
    }

    private void Editor_LostFocus(object? sender, EventArgs e)
    {
        if (_suppressEditorLostFocusCommit || !_editor.Visible)
        {
            return;
        }

        CommitValueEdit(keepFocusOnStepper: false);
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

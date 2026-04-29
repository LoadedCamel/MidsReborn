using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls;

[DefaultEvent(nameof(ValueChanged))]
public class MidsTrackBar : Control
{
    private const TextFormatFlags FormatFlags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;

    private int _minimum = 1;
    private int _maximum = 10;
    private int _value = 1;
    private int _horizontalPadding = 8;
    private int _trackThickness = 6;
    private int _thumbDiameter = 14;
    private int _textGap = 6;
    private int _trackGap = 8;
    private string _valueTextFormat = "{0}";

    public event EventHandler? ValueChanged;

    [Category("Behavior")]
    [DefaultValue(1)]
    public int Minimum
    {
        get => _minimum;
        set
        {
            if (value == _minimum) return;
            _minimum = value;
            if (_maximum < _minimum) _maximum = _minimum;
            Value = Clamp(_value);
            Invalidate();
        }
    }

    [Category("Behavior")]
    [DefaultValue(10)]
    public int Maximum
    {
        get => _maximum;
        set
        {
            if (value == _maximum) return;
            _maximum = value;
            if (_maximum < _minimum) _minimum = _maximum;
            Value = Clamp(_value);
            Invalidate();
        }
    }

    [Category("Behavior")]
    [DefaultValue(1)]
    public int Value
    {
        get => _value;
        set
        {
            var v = Clamp(value);
            if (v == _value) return;
            _value = v;
            ValueChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
    }

    [Category("Behavior")]
    [DefaultValue(1)]
    public int SmallChange { get; set; } = 1;

    [Category("Behavior")]
    [DefaultValue(5)]
    public int LargeChange { get; set; } = 5;

    [Category("Layout")]
    [DefaultValue(8)]
    public int HorizontalPadding
    {
        get => _horizontalPadding;
        set
        {
            _horizontalPadding = Math.Max(0, value);
            Invalidate();
        }
    }

    [Category("Appearance")]
    [DefaultValue(6)]
    public int TrackThickness
    {
        get => _trackThickness;
        set
        {
            _trackThickness = Math.Max(2, value);
            Invalidate();
        }
    }

    [Category("Appearance")]
    [DefaultValue(14)]
    public int ThumbDiameter
    {
        get => _thumbDiameter;
        set
        {
            _thumbDiameter = Math.Max(6, value);
            Invalidate();
        }
    }

    [Category("Focus")]
    [DefaultValue(true)]
    public new bool ShowFocusCues { get; set; } = true;

    [Category("Appearance")]
    [DefaultValue(typeof(ContentAlignment), "MiddleLeft")]
    public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleLeft;

    [Category("Appearance")]
    [DefaultValue(true)]
    public bool ShowText { get; set; } = true;

    [Category("Appearance")]
    [DefaultValue(typeof(ContentAlignment), "MiddleRight")]
    public ContentAlignment ValueAlign { get; set; } = ContentAlignment.MiddleRight;

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool ShowValue { get; set; }

    [Category("Layout")]
    [DefaultValue(6)]
    public int TextGap
    {
        get => _textGap;
        set
        {
            _textGap = Math.Max(0, value);
            Invalidate();
        }
    }

    [Category("Data")]
    [DefaultValue("{0}")]
    public string ValueTextFormat
    {
        get => _valueTextFormat;
        set
        {
            _valueTextFormat = string.IsNullOrWhiteSpace(value) ? "{0}" : value;
            Invalidate();
        }
    }

    [Category("Layout")]
    [DefaultValue(8)]
    public int TrackGap
    {
        get => _trackGap;
        set
        {
            _trackGap = Math.Max(0, value);
            Invalidate();
        }
    }

    private DataViewTheme CurrentTheme
    {
        get
        {
            if (DesignMode)
            {
                return ThemeManager.DesignTime.DataView;
            }

            return ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;
        }
    }

    private float DpiScale => DeviceDpi / 96f;

    public MidsTrackBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.Transparent;
        ForeColor = Color.WhiteSmoke;
        Size = new Size(180, 28);
        Cursor = Cursors.Hand;
        TabStop = true;

        if (!DesignMode)
        {
            ThemeManager.ThemeChanged += Invalidate;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !DesignMode)
        {
            ThemeManager.ThemeChanged -= Invalidate;
        }

        base.Dispose(disposing);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            SetFromPoint(e.Location);
        }

        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            SetFromPoint(e.Location);
        }

        base.OnMouseMove(e);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (!Enabled)
        {
            base.OnMouseWheel(e);
            return;
        }

        Value += e.Delta > 0 ? SmallChange : -SmallChange;
        base.OnMouseWheel(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!Enabled)
        {
            base.OnKeyDown(e);
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.Left:
            case Keys.Down:
                Value -= SmallChange;
                e.Handled = true;
                break;
            case Keys.Right:
            case Keys.Up:
                Value += SmallChange;
                e.Handled = true;
                break;
            case Keys.PageDown:
                Value -= LargeChange;
                e.Handled = true;
                break;
            case Keys.PageUp:
                Value += LargeChange;
                e.Handled = true;
                break;
            case Keys.Home:
                Value = Minimum;
                e.Handled = true;
                break;
            case Keys.End:
                Value = Maximum;
                e.Handled = true;
                break;
        }

        base.OnKeyDown(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        Invalidate();
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Invalidate();
    }

    protected override Size DefaultSize => new(200, 32);

    public override Size GetPreferredSize(Size proposedSize)
    {
        var labelSize = GetLabelSizeForLayout();
        var valueSize = GetValueSizeForLayout();

        var textBlockWidth = 0;
        if (!labelSize.IsEmpty) textBlockWidth += labelSize.Width;
        if (!valueSize.IsEmpty)
        {
            if (!labelSize.IsEmpty) textBlockWidth += ScalePx(_textGap);
            textBlockWidth += valueSize.Width;
        }

        const int minPreferredTrack = 120;
        var width = Padding.Horizontal + textBlockWidth + (textBlockWidth > 0 ? ScalePx(_trackGap) : 0)
                    + ScalePx(_horizontalPadding) * 2 + minPreferredTrack;

        var textHeight = Math.Max(labelSize.Height, valueSize.Height);
        var railHeight = Math.Max(ScalePx(_thumbDiameter) + ScalePx(4), ScalePx(_trackThickness));
        var height = Padding.Vertical + Math.Max(textHeight, railHeight);

        return new Size(width, height);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var content = GetContentRect();
        var label = ShowText && !string.IsNullOrEmpty(Text) ? Text : string.Empty;
        var valueText = ShowValue ? FormatValueText(_value) : string.Empty;
        var labelSize = GetLabelSizeForLayout();
        var valueSize = GetValueSizeForLayout();

        var x = content.Left;
        var labelRect = Rectangle.Empty;
        var valueRect = Rectangle.Empty;

        if (!labelSize.IsEmpty)
        {
            labelRect = new Rectangle(x, CenterY(content, labelSize.Height), labelSize.Width, labelSize.Height);
            x += labelSize.Width;
        }

        if (!valueSize.IsEmpty)
        {
            if (!labelSize.IsEmpty) x += ScalePx(_textGap);
            valueRect = new Rectangle(x, CenterY(content, valueSize.Height), valueSize.Width, valueSize.Height);
            x += valueSize.Width;
        }

        if (!labelSize.IsEmpty || !valueSize.IsEmpty)
        {
            x += ScalePx(_trackGap);
        }

        var thickness = ScalePx(_trackThickness);
        var pad = ScalePx(_horizontalPadding);
        var trackOuterLeft = x;
        var trackOuterWidth = Math.Max(0, content.Right - trackOuterLeft);
        var trackInnerWidth = Math.Max(1, trackOuterWidth - pad * 2);

        var track = new Rectangle(
            trackOuterLeft + pad,
            content.Top + (content.Height - thickness) / 2,
            trackInnerWidth,
            thickness);

        var thumb = GetThumbRect(track);

        using (var railBrush = new SolidBrush(Enabled ? CurrentTheme.Rail : Color.FromArgb(120, CurrentTheme.Rail)))
        {
            g.FillRectangle(railBrush, track);
        }

        var fill = new Rectangle(track.Left, track.Top, Math.Max(1, thumb.Left + thumb.Width / 2 - track.Left), track.Height);
        using (var fillBrush = new SolidBrush(Enabled ? CurrentTheme.RailFill : Color.FromArgb(140, CurrentTheme.RailFill)))
        {
            g.FillRectangle(fillBrush, fill);
        }

        using var thumbBrush = new SolidBrush(Enabled ? CurrentTheme.Thumb : Color.FromArgb(160, CurrentTheme.Thumb));
        using var thumbPen = new Pen(CurrentTheme.ThumbBorder);
        g.FillEllipse(thumbBrush, thumb);
        g.DrawEllipse(thumbPen, thumb);

        if (Focused && ShowFocusCues)
        {
            var focus = Rectangle.Inflate(track, ScalePx(2), ScalePx(2));
            using var focusPen = new Pen(Color.FromArgb(90, 255, 255, 255)) { DashStyle = DashStyle.Dot };
            g.DrawRectangle(focusPen, focus);
        }

        if (!labelRect.IsEmpty && !string.IsNullOrEmpty(label))
        {
            TextRenderer.DrawText(g, label, Font, labelRect, Enabled ? CurrentTheme.Text : SystemColors.GrayText, FormatFlags);
        }

        if (!valueRect.IsEmpty && !string.IsNullOrEmpty(valueText))
        {
            TextRenderer.DrawText(g, valueText, Font, valueRect, Enabled ? CurrentTheme.ValueText : SystemColors.GrayText, FormatFlags);
        }
    }

    private Size GetLabelSizeForLayout()
    {
        return !(ShowText && !string.IsNullOrEmpty(Text))
            ? Size.Empty
            : TextRenderer.MeasureText(Text, Font, new Size(int.MaxValue, int.MaxValue), FormatFlags);
    }

    private Size GetValueSizeForLayout()
    {
        if (!ShowValue)
        {
            return Size.Empty;
        }

        var measure = FormatValueText(GetValueMeasureSample());
        return string.IsNullOrEmpty(measure)
            ? Size.Empty
            : TextRenderer.MeasureText(measure, Font, new Size(int.MaxValue, int.MaxValue), FormatFlags);
    }

    private int GetValueMeasureSample() => _value >= 1 && _value <= 9 ? 50 : _value;

    private int ScalePx(int value) => (int)Math.Round(value * DpiScale);

    private Rectangle GetContentRect()
    {
        var r = ClientRectangle;
        return new Rectangle(
            r.Left + Padding.Left,
            r.Top + Padding.Top,
            Math.Max(0, r.Width - Padding.Horizontal),
            Math.Max(0, r.Height - Padding.Vertical));
    }

    private Rectangle GetThumbRect(Rectangle track)
    {
        var diameter = ScalePx(_thumbDiameter);
        if (_maximum <= _minimum || track.Width <= 1)
        {
            return new Rectangle(track.Left - diameter / 2, track.Top + (track.Height - diameter) / 2, diameter, diameter);
        }

        var pct = (_value - _minimum) / (float)(_maximum - _minimum);
        var centerX = track.Left + (int)Math.Round(track.Width * pct);
        var x = centerX - diameter / 2;
        var y = track.Top + (track.Height - diameter) / 2;
        return new Rectangle(x, y, diameter, diameter);
    }

    private static int CenterY(Rectangle rect, int height) => rect.Top + (rect.Height - height) / 2;

    private string FormatValueText(int value)
    {
        try
        {
            return string.Format(ValueTextFormat, value);
        }
        catch
        {
            return value.ToString();
        }
    }

    private int Clamp(int value)
    {
        return Math.Max(_minimum, Math.Min(_maximum, value));
    }

    private void SetFromPoint(Point clientPoint)
    {
        Focus();

        var content = GetContentRect();
        var labelSize = GetLabelSizeForLayout();
        var valueSize = GetValueSizeForLayout();

        var x = content.Left;
        if (!labelSize.IsEmpty) x += labelSize.Width;
        if (!valueSize.IsEmpty)
        {
            if (!labelSize.IsEmpty) x += ScalePx(_textGap);
            x += valueSize.Width;
        }

        if (!labelSize.IsEmpty || !valueSize.IsEmpty)
        {
            x += ScalePx(_trackGap);
        }

        var pad = ScalePx(_horizontalPadding);
        var trackLeft = x + pad;
        var trackRight = content.Right - pad;
        var trackWidth = Math.Max(1, trackRight - trackLeft);

        var px = Math.Max(trackLeft, Math.Min(trackRight, clientPoint.X));
        var t = (px - trackLeft) / (float)trackWidth;
        var newValue = _minimum + (int)Math.Round((_maximum - _minimum) * t);
        Value = Clamp(newValue);
    }
}

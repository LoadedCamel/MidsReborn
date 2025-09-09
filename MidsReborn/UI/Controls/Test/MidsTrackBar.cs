using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

[DefaultEvent(nameof(ValueChanged))]
public sealed class MidsTrackBar : Control
{
    #region Constants

    private const TextFormatFlags FormatFlags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;

    #endregion

    #region Fields

    private int _minimum = 1;
    private int _maximum = 10;
    private int _value = 1;

    // Geometry (logical pixels; scaled by DPI at draw time)
    private int _horizontalPadding = 8;   // left/right space before the rail starts
    private int _trackThickness = 6;   // rail height
    private int _thumbDiameter = 14;  // circular thumb

    private int _textGap = 6;           // [Text] <gap> [Value]
    private int _trackGap = 8;          // [Text Value] <gap> [Track]

    private string _valueTextFormat = "{0}";

    #endregion

    #region Events

    public event EventHandler? ValueChanged;

    #endregion

    #region Public Properties

    [Category("Behavior")]
    [DefaultValue(1)]
    [Description("Low end of the selectable range.")]
    public int Minimum
    {
        get => _minimum;
        set
        {
            if (value == _minimum) return;
            _minimum = value;
            if (_maximum < _minimum) _maximum = _minimum;
            Value = Clamp(_value);       // coerces and repaints
            Invalidate();
        }
    }

    [Category("Behavior")]
    [DefaultValue(10)]
    [Description("High end of the selectable range.")]
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
    [Description("Current value of the slider.")]
    public int Value
    {
        get => _value;
        set
        {
            int v = Clamp(value);
            if (v == _value) return;
            _value = v;
            ValueChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
    }

    [Category("Behavior")]
    [DefaultValue(1)]
    [Description("Keyboard arrow-step size.")]
    public int SmallChange { get; set; } = 1;

    [Category("Behavior")]
    [DefaultValue(5)]
    [Description("Keyboard PageUp/PageDown step size.")]
    public int LargeChange { get; set; } = 5;

    [Category("Layout")]
    [DefaultValue(8)]
    [Description("Inner left/right padding applied inside the track drawing area (logical pixels).")]
    public int HorizontalPadding
    {
        get => _horizontalPadding;
        set { _horizontalPadding = Math.Max(0, value); Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(6)]
    [Description("Track thickness (logical pixels).")]
    public int TrackThickness
    {
        get => _trackThickness;
        set { _trackThickness = Math.Max(2, value); Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(14)]
    [Description("Thumb diameter (logical pixels).")]
    public int ThumbDiameter
    {
        get => _thumbDiameter;
        set { _thumbDiameter = Math.Max(6, value); Invalidate(); }
    }

    [Category("Focus")]
    [DefaultValue(true)]
    [Description("If true, draws a subtle focus outline when the control has focus.")]
    public new bool ShowFocusCues { get; set; } = true;

    [Category("Appearance")]
    [DefaultValue(typeof(ContentAlignment), "MiddleLeft")]
    [Description("Text placement around the rail.")]
    public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleLeft;

    [Category("Appearance")]
    [DefaultValue(true)]
    [Description("Show or hide the text.")]
    public bool ShowText { get; set; } = true;

    [Category("Appearance")]
    [DefaultValue(typeof(ContentAlignment), "MiddleRight")]
    [Description("Value text placement around the rail, CheckBox.CheckAlign-style.")]
    public ContentAlignment ValueAlign { get; set; } = ContentAlignment.MiddleRight;

    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("Show or hide the formatted value text.")]
    public bool ShowValue { get; set; } = false;

    [Category("Layout")]
    [DefaultValue(6)]
    [Description("Gap, in logical pixels, between the label text and value text in the left text block.")]
    public int TextGap
    {
        get => _textGap;
        set { _textGap = Math.Max(0, value); Invalidate(); }
    }

    [Category("Data")]
    [DefaultValue("{0}")]
    [Description("Format string for the value text (e.g. \"{0}%\", \"Lv {0}\").")]
    public string ValueTextFormat
    {
        get => _valueTextFormat;
        set { _valueTextFormat = string.IsNullOrWhiteSpace(value) ? "{0}" : value; Invalidate(); }
    }

    [Category("Layout")]
    [DefaultValue(8)]
    [Description("Gap, in logical pixels, between the combined text block and the slider rail.")]
    public int TrackGap
    {
        get => _trackGap;
        set { _trackGap = Math.Max(0, value); Invalidate(); }
    }

    #endregion

    #region Private Properties

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

    #endregion

    #region Constructor

    public MidsTrackBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint |
                 ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.Transparent;
        ForeColor = Color.WhiteSmoke;
        Size = new Size(180, 28);
        Cursor = Cursors.Hand;
        TabStop = true;
    }

    #endregion

    #region Overrides

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
            SetFromPoint(e.Location);
        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
            SetFromPoint(e.Location);
        base.OnMouseMove(e);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (!Enabled) { base.OnMouseWheel(e); return; }
        int delta = e.Delta > 0 ? SmallChange : -SmallChange;
        Value += delta;
        base.OnMouseWheel(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!Enabled) { base.OnKeyDown(e); return; }

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
        Size labelSize = GetLabelSizeForLayout();
        Size valueSize = GetValueSizeForLayout();

        int textBlockWidth = 0;
        if (!labelSize.IsEmpty)
            textBlockWidth += labelSize.Width;
        if (!valueSize.IsEmpty)
        {
            if (!labelSize.IsEmpty) textBlockWidth += ScalePx(_textGap);
            textBlockWidth += valueSize.Width;
        }

        // A reasonable minimum rail width for AutoSize scenarios
        const int minPreferredTrack = 120;
        int w = Padding.Horizontal + textBlockWidth + (textBlockWidth > 0 ? ScalePx(_trackGap) : 0)
                + ScalePx(_horizontalPadding) * 2 + minPreferredTrack;

        int textHeight = Math.Max(labelSize.Height, valueSize.Height);
        int railHeight = Math.Max(ScalePx(_thumbDiameter) + ScalePx(4), ScalePx(_trackThickness));
        int h = Padding.Vertical + Math.Max(textHeight, railHeight);

        return new Size(w, h);
    }

    #endregion

    #region Drawing

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Layout
        var content = GetContentRect();

        // 1) Compute left text block: [Text] [TextGap] [Value]
        string label = (ShowText && !string.IsNullOrEmpty(Text)) ? Text : string.Empty;
        string valueStr = (ShowValue ? FormatValueText(_value) : string.Empty);

        Size labelSize = GetLabelSizeForLayout();
        Size valueSize = GetValueSizeForLayout();

        int x = content.Left;
        Rectangle labelRect = Rectangle.Empty;
        Rectangle valueRect = Rectangle.Empty;

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

        // 2) Gap between text block and track
        if (!labelSize.IsEmpty || !valueSize.IsEmpty)
            x += ScalePx(_trackGap);

        // 3) Track rectangle fills the remainder (with inner HorizontalPadding)
        int th = ScalePx(_trackThickness);
        int pad = ScalePx(_horizontalPadding);
        int trackOuterLeft = x;
        int trackOuterWidth = Math.Max(0, content.Right - trackOuterLeft);
        int trackInnerWidth = Math.Max(1, trackOuterWidth - pad * 2);

        var track = new Rectangle(
            trackOuterLeft + pad,
            content.Top + (content.Height - th) / 2,
            trackInnerWidth,
            th);

        var thumb = GetThumbRect(track);

        // Rail (background)
        using (var railBrush = new SolidBrush(Enabled ? CurrentTheme.Rail : Color.FromArgb(120, CurrentTheme.Rail)))
            g.FillRectangle(railBrush, track);

        // Filled portion
        var fill = new Rectangle(track.Left, track.Top,
                                 Math.Max(1, thumb.Left + thumb.Width / 2 - track.Left),
                                 track.Height);
        using (var fillBrush = new SolidBrush(Enabled ? CurrentTheme.RailFill : Color.FromArgb(140, CurrentTheme.RailFill)))
            g.FillRectangle(fillBrush, fill);

        // Thumb
        using var tb = new SolidBrush(Enabled ? CurrentTheme.Thumb : Color.FromArgb(160, CurrentTheme.Thumb));
        using var pb = new Pen(CurrentTheme.ThumbBorder);
        g.FillEllipse(tb, thumb);
        g.DrawEllipse(pb, thumb);

        // Focus ring
        if (Focused && ShowFocusCues)
        {
            var focus = Rectangle.Inflate(track, ScalePx(2), ScalePx(2));
            using var focusPen = new Pen(Color.FromArgb(90, 255, 255, 255));
            focusPen.DashStyle = DashStyle.Dot;
            g.DrawRectangle(focusPen, focus);
        }

        // Draw texts

        if (!labelRect.IsEmpty && !string.IsNullOrEmpty(label))
        {
            TextRenderer.DrawText(g, label, Font, labelRect, Enabled ? CurrentTheme.Text : SystemColors.GrayText, FormatFlags);
        }

        if (!valueRect.IsEmpty && !string.IsNullOrEmpty(valueStr))
        {
            TextRenderer.DrawText(g, valueStr, Font, valueRect, Enabled ? CurrentTheme.ValueText : SystemColors.GrayText, FormatFlags);
        }
    }

    #endregion

    #region Measurement helpers (jitter-free value width)

    private Size GetLabelSizeForLayout()
    {
        if (!(ShowText && !string.IsNullOrEmpty(Text))) return Size.Empty;
        return TextRenderer.MeasureText(Text, Font, new Size(int.MaxValue, int.MaxValue), FormatFlags);
    }

    // Measure value text using a stable 2-digit sample when current value is 1–9.
    private Size GetValueSizeForLayout()
    {
        if (!ShowValue) return Size.Empty;

        string measure = FormatValueText(GetValueMeasureSample());
        if (string.IsNullOrEmpty(measure)) return Size.Empty;

        return TextRenderer.MeasureText(measure, Font, new Size(int.MaxValue, int.MaxValue), FormatFlags);
    }

    private int GetValueMeasureSample()
    {
        // When current value is single-digit (1–9), reserve space for two digits by measuring "50".
        // Otherwise, measure the actual value width.
        if (_value >= 1 && _value <= 9) return 50;
        return _value;
    }

    #endregion

    #region Geometry & Value math

    private int ScalePx(int v) => (int)Math.Round(v * DpiScale);

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
        int dia = ScalePx(_thumbDiameter);
        if (_maximum <= _minimum || track.Width <= 1)
            return new Rectangle(track.Left - dia / 2, track.Top + (track.Height - dia) / 2, dia, dia);

        float pct = (_value - _minimum) / (float)(_maximum - _minimum);
        int cx = track.Left + (int)Math.Round(track.Width * pct);
        int x = cx - dia / 2;
        int y = track.Top + (track.Height - dia) / 2;
        return new Rectangle(x, y, dia, dia);
    }

    private void SetFromPoint(Point p)
    {
        // Recompute layout to derive the current track rect
        var content = GetContentRect();

        // Mirror the layout logic minimally to get the current track bounds:
        string label = (ShowText && !string.IsNullOrEmpty(Text)) ? Text : string.Empty;
        string valueStr = (ShowValue ? FormatValueText(_value) : string.Empty);

        Size labelSize = GetLabelSizeForLayout();
        Size valueSize = GetValueSizeForLayout();

        int x = content.Left;
        if (!labelSize.IsEmpty) x += labelSize.Width;
        if (!valueSize.IsEmpty)
        {
            if (!labelSize.IsEmpty) x += ScalePx(_textGap);
            x += valueSize.Width;
        }
        if (!labelSize.IsEmpty || !valueSize.IsEmpty) x += ScalePx(_trackGap);

        int pad = ScalePx(_horizontalPadding);
        int trackOuterLeft = x;
        int trackOuterWidth = Math.Max(0, content.Right - trackOuterLeft);
        int trackInnerWidth = Math.Max(1, trackOuterWidth - pad * 2);

        var track = new Rectangle(
            trackOuterLeft + pad,
            content.Top,
            trackInnerWidth,
            content.Height);

        if (track.Width <= 1 || _maximum <= _minimum) return;

        float pct = Math.Max(0f, Math.Min(1f, (p.X - track.Left) / (float)track.Width));
        int newVal = _minimum + (int)Math.Round(pct * (_maximum - _minimum));
        Value = newVal;
    }

    private int Clamp(int v) => Math.Max(_minimum, Math.Min(_maximum, v));

    private static int CenterY(Rectangle outer, int height) => outer.Top + (outer.Height - height) / 2;

    private static Size MeasureSingleLine(string text, Font font)
    {
        // Tight single-line measurement (no wrapping)
        return TextRenderer.MeasureText(text, font, new Size(int.MaxValue, int.MaxValue),
            TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
    }

    private string FormatValueText(int v)
    {
        try
        {
            return string.Format(_valueTextFormat, v);
        }
        catch
        {
            return v.ToString();
        }
    }

    #endregion
}
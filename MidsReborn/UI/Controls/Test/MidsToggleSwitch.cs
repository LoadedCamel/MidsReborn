using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

[DefaultEvent(nameof(CheckedChanged))]
public sealed class MidsToggleSwitch : Control
{
    #region Constants

    private const float RailAspect = 2.1f;

    #endregion

    #region Instance Fields

    private bool _checked;
    private int _cornerRadius = 8;
    private int _fillInset = 2;
    private int _thumbPadding = 3;

    // Text / layout
    private LabelPlacement _labelPlacement = LabelPlacement.Left;
    private int _contentGap = 6;
    private ContentAlignment _textAlign = ContentAlignment.MiddleLeft;
    private bool _useMnemonic = true;

    // When 0, switch height is derived from Font; otherwise, explicit logical px (DPI scaled).
    private int _explicitSwitchHeight;

    #endregion

    #region Enums

    public enum LabelPlacement
    {
        Left,
        Right,
        Above,
        Below
    }

    #endregion

    #region Events

    public event EventHandler<bool>? CheckedChanged;

    #endregion

    #region Public Properties

    [Category("Behavior")]
    [DefaultValue(false)]
    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked == value) return;
            _checked = value;
            Invalidate();
            CheckedChanged?.Invoke(this, value);
        }
    }

    [Category("Appearance")]
    [DefaultValue(8)]
    [Description("Corner radius of the rail, in logical pixels (scaled by DPI).")]
    public int CornerRadius
    {
        get => _cornerRadius;
        set { _cornerRadius = Math.Max(0, value); Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(2)]
    [Description("Inset used for the inner checked fill, in logical pixels (scaled by DPI).")]
    public int FillInset
    {
        get => _fillInset;
        set { _fillInset = Math.Max(0, value); Invalidate(); }
    }

    [Category("Layout")]
    [DefaultValue(3)]
    [Description("Padding between rail edge and thumb, in logical pixels (scaled by DPI).")]
    public int ThumbPadding
    {
        get => _thumbPadding;
        set { _thumbPadding = Math.Max(0, value); Invalidate(); }
    }

    [Category("Focus")]
    [DefaultValue(true)]
    [Description("If true, draws a subtle focus outline when the control has focus.")]
    public new bool ShowFocusCues { get; set; } = true;

    [Category("Layout")]
    [DefaultValue(LabelPlacement.Left)]
    [Description("Where to place the text relative to the switch.")]
    public LabelPlacement Placement
    {
        get => _labelPlacement;
        set { _labelPlacement = value; RelayoutAndRedraw(); }
    }

    [Category("Layout")]
    [DefaultValue(6)]
    [Description("Gap between the switch and its label text, in logical pixels (scaled by DPI).")]
    public int ContentGap
    {
        get => _contentGap;
        set { _contentGap = Math.Max(0, value); RelayoutAndRedraw(); }
    }

    [Category("Appearance")]
    [DefaultValue(typeof(ContentAlignment), nameof(ContentAlignment.MiddleLeft))]
    [Description("Text alignment inside the text layout rectangle.")]
    public ContentAlignment TextAlign
    {
        get => _textAlign;
        set { _textAlign = value; Invalidate(); }
    }

    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Enables keyboard mnemonics using '&' in Text.")]
    public bool UseMnemonic
    {
        get => _useMnemonic;
        set { _useMnemonic = value; Invalidate(); }
    }

    [Category("Layout")]
    [DefaultValue(0)]
    [Description("Explicit switch height in logical pixels (scaled by DPI). If 0, height derives from the Font.")]
    public int SwitchHeight
    {
        get => _explicitSwitchHeight;
        set { _explicitSwitchHeight = Math.Max(0, value); RelayoutAndRedraw(); }
    }

    #endregion

    #region Private/Protected Properties

    private float DpiScale => DeviceDpi / 96f;

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

    #endregion

    #region Constructor

    public MidsToggleSwitch()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.Transparent;
        Size = new Size(64, 28);
        Cursor = Cursors.Hand;
        TabStop = true;
    }

    #endregion

    #region Overrides

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
        {
            Checked = !Checked;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode is Keys.Space or Keys.Enter)
        {
            Checked = !Checked; e.Handled = true;
        }
    }

    protected override bool ProcessMnemonic(char charCode)
    {
        if (UseMnemonic && IsMnemonic(charCode, Text))
        {
            Checked = !Checked;
            return true;
        }
        return base.ProcessMnemonic(charCode);
    }

    protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
    protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }
    protected override void OnEnabledChanged(EventArgs e) { base.OnEnabledChanged(e); Invalidate(); }
    protected override void OnFontChanged(EventArgs e) { base.OnFontChanged(e); RelayoutAndRedraw(); }
    protected override void OnTextChanged(EventArgs e) { base.OnTextChanged(e); RelayoutAndRedraw(); }
    protected override void OnPaddingChanged(EventArgs e) { base.OnPaddingChanged(e); RelayoutAndRedraw(); }
    protected override void OnRightToLeftChanged(EventArgs e) { base.OnRightToLeftChanged(e); Invalidate(); }

    public override Size GetPreferredSize(Size proposedSize)
    {
        // Compute desired size as switch glyph + gap + measured text, depending on placement.
        using var g = CreateGraphics();
        var (switchSize, _) = GetSwitchMinSizeAndRail(g);

        var textSize = MeasureTextSize(g, proposedSize.Width <= 0 ? int.MaxValue : proposedSize.Width);
        int gap = (string.IsNullOrEmpty(Text) ? 0 : ScalePx(ContentGap));

        Size total;
        switch (EffectivePlacement)
        {
            case LabelPlacement.Left:
            case LabelPlacement.Right:
                total = new Size(
                    Padding.Horizontal + switchSize.Width + gap + textSize.Width,
                    Padding.Vertical + Math.Max(switchSize.Height, textSize.Height));
                break;

            case LabelPlacement.Above:
            case LabelPlacement.Below:
            default:
                total = new Size(
                    Padding.Horizontal + Math.Max(switchSize.Width, textSize.Width),
                    Padding.Vertical + switchSize.Height + gap + textSize.Height);
                break;
        }
        // Avoid returning 0 sized in designer
        total.Width = Math.Max(total.Width, 32);
        total.Height = Math.Max(total.Height, 20);
        return total;
    }

    #endregion

    #region Drawing

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var client = ClientRectangle;

        if (client.Width <= 1 || client.Height <= 1) return;

        // Compute layout
        var layout = ComputeLayout(g, client);

        // --- Draw rail in layout.SwitchRect ---
        DrawSwitch(g, layout.SwitchRect);

        // --- Draw text in layout.TextRect ---
        if (!string.IsNullOrEmpty(Text) && layout.TextRect.Width > 0 && layout.TextRect.Height > 0)
            DrawText(g, layout.TextRect);

        // --- Focus ring around combined area ---
        if (Focused && ShowFocusCues)
        {
            var focusRect = Rectangle.Inflate(Rectangle.Union(layout.SwitchRect, layout.TextRect), -ScalePx(1), -ScalePx(1));
            ControlPaint.DrawFocusRectangle(g, focusRect);
        }
    }

    private void DrawSwitch(Graphics g, Rectangle r)
    {
        // If there's not enough room, bail gracefully.
        if (r.Width <= 2 || r.Height <= 2) return;

        // Rail
        using (var railPath = Capsule(r, ScalePx(CornerRadius)))
        using (var railBrush = new SolidBrush(Enabled ? CurrentTheme.Rail : Color.FromArgb(120, CurrentTheme.Rail)))
        {
            g.FillPath(railBrush, railPath);
        }

        // Checked fill (inset capsule)
        if (Checked)
        {
            var fill = Rectangle.Inflate(r, -ScalePx(FillInset), -ScalePx(FillInset));
            using var fillPath = Capsule(fill, Math.Max(1, ScalePx(CornerRadius - FillInset)));
            using var fillBrush = new SolidBrush(Enabled ? CurrentTheme.RailFill : Color.FromArgb(140, CurrentTheme.RailFill));
            g.FillPath(fillBrush, fillPath);
        }

        // Thumb
        int pad = ScalePx(ThumbPadding);
        int thumb = Math.Max(1, r.Height - pad * 2);
        int cx = Checked ? r.Right - pad - thumb : r.Left + pad;
        var tRect = new Rectangle(cx, r.Top + pad, thumb, thumb);

        using var thumbBrush = new SolidBrush(Enabled ? CurrentTheme.Thumb : Color.FromArgb(160, CurrentTheme.Thumb));
        using var thumbPen = new Pen(CurrentTheme.ThumbBorder);
        g.FillEllipse(thumbBrush, tRect);
        g.DrawEllipse(thumbPen, tRect);
    }

    private void DrawText(Graphics g, Rectangle textRect)
    {
        // TextRenderer handles ClearType + DPI correctly in WinForms.
        var flags = TextFormatFlags.NoPadding |
                    TextFormatFlags.EndEllipsis |
                    TextFormatFlags.PreserveGraphicsTranslateTransform |
                    TextFormatFlags.SingleLine;

        if (!UseMnemonic) flags |= TextFormatFlags.NoPrefix;
        if (RightToLeft == RightToLeft.Yes) flags |= TextFormatFlags.RightToLeft;

        flags |= _textAlign switch
        {
            ContentAlignment.TopLeft => TextFormatFlags.Top | TextFormatFlags.Left,
            ContentAlignment.TopCenter => TextFormatFlags.Top | TextFormatFlags.HorizontalCenter,
            ContentAlignment.TopRight => TextFormatFlags.Top | TextFormatFlags.Right,
            ContentAlignment.MiddleLeft => TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            ContentAlignment.MiddleCenter => TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            ContentAlignment.MiddleRight => TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
            ContentAlignment.BottomLeft => TextFormatFlags.Bottom | TextFormatFlags.Left,
            ContentAlignment.BottomCenter => TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter,
            ContentAlignment.BottomRight => TextFormatFlags.Bottom | TextFormatFlags.Right,
            _ => TextFormatFlags.VerticalCenter | TextFormatFlags.Left
        };

        Color color = Enabled ? CurrentTheme.Text : SystemColors.GrayText;
        TextRenderer.DrawText(g, Text, Font, textRect, color, flags);
    }

    #endregion

    #region Helpers

    private void RelayoutAndRedraw()
    {
        // 1) If AutoSize, compute preferred size and apply it.
        if (AutoSize)
        {
            var desired = GetPreferredSize(Size.Empty);
            if (desired != Size)
                Size = desired;              // Triggers Layout/Resize events
        }

        // 2) Repaint using the fresh geometry.
        Invalidate();                        // coalesced; do NOT call Refresh() here
    }

    private int ScalePx(int v) => (int)Math.Round(v * DpiScale);

    private static GraphicsPath Capsule(Rectangle r, int radiusPx)
    {
        // Clamp radius so arcs never invert.
        int rad = Math.Max(1, Math.Min(radiusPx, Math.Min(r.Width, r.Height) / 2));
        int d = rad * 2;

        var p = new GraphicsPath();
        p.StartFigure();
        p.AddArc(r.Left, r.Top, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    private LabelPlacement EffectivePlacement
    {
        get
        {
            if (RightToLeft == RightToLeft.Yes)
            {
                // Mirror left/right when RTL is active, which matches typical WinForms checkbox behavior.
                if (_labelPlacement == LabelPlacement.Left) return LabelPlacement.Right;
                if (_labelPlacement == LabelPlacement.Right) return LabelPlacement.Left;
            }
            return _labelPlacement;
        }
    }

    private (Size MinSize, int RailHeight) GetSwitchMinSizeAndRail(Graphics g)
    {
        // Derive a pleasant rail height:
        //  - If explicit: use it (scaled)
        //  - Else: base it on font height (~1.25x) to match checkbox text proportions
        int fontH = FontHeightFor(g);
        int railH = _explicitSwitchHeight > 0 ? ScalePx(_explicitSwitchHeight)
                                              : Math.Max(ScalePx(18), (int)Math.Round(fontH * 1.25));

        // Calculate rail length via aspect ratio (plus implicit pixel room for outline/anti-alias).
        int railW = Math.Max(railH + 4, (int)Math.Round(railH * RailAspect));

        return (new Size(railW, railH), railH);
    }

    private int FontHeightFor(Graphics g)
    {
        // Using Font.Height is fine, but GetHeight(Graphics) returns a float with DPI;
        // we round it for consistency with TextRenderer metrics.
        return (int)Math.Round(Font.GetHeight(g));
    }

    private Size MeasureTextSize(Graphics g, int maxWidth)
    {
        if (string.IsNullOrEmpty(Text)) return Size.Empty;

        var flags = TextFormatFlags.NoPadding |
                    TextFormatFlags.PreserveGraphicsTranslateTransform |
                    TextFormatFlags.WordBreak; // allow multi-line above/below if width constrained

        if (!UseMnemonic) flags |= TextFormatFlags.NoPrefix;
        if (RightToLeft == RightToLeft.Yes) flags |= TextFormatFlags.RightToLeft;

        // Provide a large height to measure likely size, constrain width depending on placement.
        Size proposed;
        switch (EffectivePlacement)
        {
            case LabelPlacement.Above:
            case LabelPlacement.Below:
                // For stacked layout, prefer wrapping to our width (or proposed width).
                int w = Math.Max(32, maxWidth <= 0 ? ClientSize.Width : maxWidth);
                proposed = new Size(w, int.MaxValue);
                break;

            default:
                proposed = new Size(int.MaxValue, int.MaxValue);
                break;
        }

        return TextRenderer.MeasureText(g, Text, Font, proposed, flags);
    }

    private int MeasureTextSingleLineWidth(Graphics g)
    {
        if (string.IsNullOrEmpty(Text)) return 0;
        var flags = TextFormatFlags.NoPadding |
                    TextFormatFlags.SingleLine |
                    TextFormatFlags.PreserveGraphicsTranslateTransform;

        if (!UseMnemonic) flags |= TextFormatFlags.NoPrefix;
        if (RightToLeft == RightToLeft.Yes) flags |= TextFormatFlags.RightToLeft;

        return TextRenderer.MeasureText(g, Text, Font, Size.Empty, flags).Width;
    }

    private (Rectangle SwitchRect, Rectangle TextRect) ComputeLayout(Graphics g, Rectangle client)
    {
        var content = Rectangle.Inflate(client, -Padding.Left, -Padding.Top);
        content.Width -= Padding.Right;
        content.Height -= Padding.Bottom;

        var (switchMinSize, _) = GetSwitchMinSizeAndRail(g);
        int gap = string.IsNullOrEmpty(Text) ? 0 : ScalePx(ContentGap);

        Rectangle switchRect, textRect;

        // Ensure we don't exceed available bounds vertically
        switchMinSize.Height = Math.Min(switchMinSize.Height, Math.Max(0, content.Height));

        switch (EffectivePlacement)
        {
            case LabelPlacement.Left:
                {
                    // [Text][gap][Switch]
                    int textIdeal = MeasureTextSingleLineWidth(g);
                    int minTotal = switchMinSize.Width + gap + textIdeal;

                    int switchW, textW;
                    if (content.Width <= minTotal)
                    {
                        // Not enough room for ideal text width: keep minimal rail; ellipsize text if needed
                        switchW = switchMinSize.Width;
                        textW = Math.Max(0, content.Width - switchW - gap);
                    }
                    else
                    {
                        // Give text its full width; surplus goes to the rail (pill grows)
                        textW = textIdeal;
                        switchW = switchMinSize.Width + (content.Width - minTotal);
                    }

                    textRect = new Rectangle(content.Left, content.Top, textW, content.Height);
                    switchRect = new Rectangle(
                        textRect.Right + (textW > 0 ? gap : 0),
                        content.Top + (content.Height - switchMinSize.Height) / 2,
                        switchW, switchMinSize.Height);
                    break;
                }

            case LabelPlacement.Right:
                {
                    // [Switch][gap][Text]
                    int textIdeal = MeasureTextSingleLineWidth(g);
                    int minTotal = switchMinSize.Width + gap + textIdeal;

                    int switchW, textW;
                    if (content.Width <= minTotal)
                    {
                        // Not enough room: keep minimal rail; ellipsize text
                        switchW = switchMinSize.Width;
                        textW = Math.Max(0, content.Width - switchW - gap);
                    }
                    else
                    {
                        // Give text its full width; surplus grows the rail
                        textW = textIdeal;
                        switchW = switchMinSize.Width + (content.Width - minTotal);
                    }

                    switchRect = new Rectangle(
                        content.Left,
                        content.Top + (content.Height - switchMinSize.Height) / 2,
                        switchW, switchMinSize.Height);

                    int textX = switchRect.Right + (textW > 0 ? gap : 0);
                    textRect = new Rectangle(textX, content.Top, Math.Max(0, content.Right - textX), content.Height);
                    // Clamp to the ideal width so text doesn't stretch more than needed
                    if (textRect.Width > textW) textRect.Width = textW;
                    break;
                }

            case LabelPlacement.Above:
                {
                    // [Text]
                    //   gap
                    // [Switch]
                    int textH = MeasureTextSize(g, content.Width).Height;
                    textH = Math.Min(textH, Math.Max(0, content.Height));
                    textRect = new Rectangle(content.Left, content.Top, content.Width, textH);
                    int switchY = textRect.Bottom + gap;
                    int remainingH = Math.Max(0, content.Bottom - switchY);
                    int railH = Math.Min(switchMinSize.Height, remainingH);
                    switchRect = new Rectangle(
                        content.Left + (content.Width - switchMinSize.Width) / 2,
                        switchY + (remainingH - railH) / 2,
                        switchMinSize.Width, railH);
                    break;
                }

            case LabelPlacement.Below:
            default:
                {
                    // [Switch]
                    //   gap
                    // [Text]
                    int switchY = content.Top;
                    switchRect = new Rectangle(
                        content.Left + (content.Width - switchMinSize.Width) / 2,
                        switchY,
                        switchMinSize.Width, switchMinSize.Height);

                    int textY = switchRect.Bottom + gap;
                    textRect = new Rectangle(content.Left, textY, content.Width, Math.Max(0, content.Bottom - textY));
                    break;
                }
        }

        return (switchRect, textRect);
    }

    #endregion
}
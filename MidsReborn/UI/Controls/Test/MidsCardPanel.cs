using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

/// <summary>
/// CSS-like Card control for WinForms: rounded corners, soft drop shadow, optional gradient fill, and border.
/// Designed for .NET 6–8, DPI-aware, double-buffered, and container-friendly (inherits Panel).
/// </summary>
[DesignerCategory("Code")]
public sealed class MidsCardPanel : Panel
{
    #region Constants

    private const int DefaultCornerRadius = 12;
    private const int DefaultShadowBlur = 12;      // visual blur steps (not Gaussian)
    private const int DefaultShadowSpread = 2;     // like CSS 'spread-radius'
    private const int DefaultShadowOffsetY = 6;    // shadow offset downward
    private const int DefaultBorderThickness = 1;
    private const SmoothingMode PaintSmoothing = SmoothingMode.AntiAlias;

    #endregion

    #region Instance Fields

    private int _cornerRadius = DefaultCornerRadius;
    private int _shadowBlur = DefaultShadowBlur;
    private int _shadowSpread = DefaultShadowSpread;
    private int _shadowOffsetX;
    private int _shadowOffsetY = DefaultShadowOffsetY;
    private Color _shadowColor = Color.FromArgb(90, Color.Black);

    private bool _useGradientBackground;
    private Color _gradientColor1 = Color.White;
    private Color _gradientColor2 = Color.Black;
    private float _gradientAngle = 90f;

    private Color _cardBackColor = Color.Silver;
    private Color _borderColor = Color.FromArgb(220, 220, 220);
    private int _borderThickness = DefaultBorderThickness;

    private bool _hoverElevates = true;
    private int _hoverExtraOffsetY;
    private int _hoverExtraBlur = 4;

    private bool _pressedFlattens = true;
    private int _pressedReduceOffsetY = -3;
    private int _pressedReduceBlur = -6;

    private bool _isHovering;
    private bool _isPressed;

    private bool _clipToCard = true;
    private Region? _cachedRegion;

    #endregion

    #region Properties

    [Browsable(false)]
    private float DpiScale => DeviceDpi / 96f;

    [Category("Appearance")]
    [Description("Corner radius in logical pixels (DPI scaled).")]
    [DefaultValue(DefaultCornerRadius)]
    public int CornerRadius
    {
        get => _cornerRadius;
        set
        {
            value = Math.Max(0, value);
            if (_cornerRadius == value) return;
            _cornerRadius = value;
            UpdateCardRegion();
            Invalidate();
        }
    }

    [Category("Shadow")]
    [Description("Shadow blur (visual steps). Higher = softer/larger shadow.")]
    [DefaultValue(DefaultShadowBlur)]
    public int ShadowBlur
    {
        get => _shadowBlur;
        set { _shadowBlur = Math.Max(0, value); Invalidate(); }
    }

    [Category("Shadow")]
    [Description("Shadow spread: expands shadow outward (like CSS spread-radius).")]
    [DefaultValue(DefaultShadowSpread)]
    public int ShadowSpread
    {
        get => _shadowSpread;
        set { _shadowSpread = Math.Max(0, value); Invalidate(); }
    }

    [Category("Shadow")]
    [Description("Shadow horizontal offset (like CSS offset-x).")]
    [DefaultValue(0)]
    public int ShadowOffsetX
    {
        get => _shadowOffsetX;
        set { _shadowOffsetX = value; Invalidate(); }
    }

    [Category("Shadow")]
    [Description("Shadow vertical offset (like CSS offset-y).")]
    [DefaultValue(DefaultShadowOffsetY)]
    public int ShadowOffsetY
    {
        get => _shadowOffsetY;
        set { _shadowOffsetY = value; Invalidate(); }
    }

    [Category("Shadow")]
    [Description("Shadow color (alpha respected).")]
    public Color ShadowColor
    {
        get => _shadowColor;
        set { _shadowColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Card fill color when gradient is disabled.")]
    public Color CardBackColor
    {
        get => _cardBackColor;
        set { _cardBackColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Enable gradient background fill.")]
    [DefaultValue(false)]
    public bool UseGradientBackground
    {
        get => _useGradientBackground;
        set { _useGradientBackground = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Gradient start color (when UseGradientBackground = true).")]
    public Color GradientColor1
    {
        get => _gradientColor1;
        set { _gradientColor1 = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Gradient end color (when UseGradientBackground = true).")]
    public Color GradientColor2
    {
        get => _gradientColor2;
        set { _gradientColor2 = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Gradient angle in degrees (when UseGradientBackground = true).")]
    [DefaultValue(90f)]
    public float GradientAngle
    {
        get => _gradientAngle;
        set { _gradientAngle = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Border color around the card.")]
    public Color BorderColor
    {
        get => _borderColor;
        set { _borderColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Border thickness in logical pixels (DPI scaled).")]
    [DefaultValue(DefaultBorderThickness)]
    public int BorderThickness
    {
        get => _borderThickness;
        set
        {
            value = Math.Max(0, value);
            if (_borderThickness == value) return;
            _borderThickness = value;
            UpdateCardRegion();
            PerformLayout(); // Dock/Anchor recompute against new DisplayRectangle
            Invalidate();
        }
    }

    [Category("Interaction")]
    [Description("If true, hovering slightly increases elevation (larger/softer shadow).")]
    [DefaultValue(true)]
    public bool HoverElevates
    {
        get => _hoverElevates;
        set { _hoverElevates = value; Invalidate(); }
    }

    [Category("Interaction")]
    [Description("Extra vertical offset applied on hover (visual lift).")]
    [DefaultValue(0)]
    public int HoverExtraOffsetY
    {
        get => _hoverExtraOffsetY;
        set { _hoverExtraOffsetY = value; Invalidate(); }
    }

    [Category("Interaction")]
    [Description("Extra blur steps applied on hover (softens shadow).")]
    [DefaultValue(4)]
    public int HoverExtraBlur
    {
        get => _hoverExtraBlur;
        set { _hoverExtraBlur = value; Invalidate(); }
    }

    [Category("Interaction")]
    [Description("If true, pressed state reduces elevation (tighter/closer shadow).")]
    [DefaultValue(true)]
    public bool PressedFlattens
    {
        get => _pressedFlattens;
        set { _pressedFlattens = value; Invalidate(); }
    }

    [Category("Interaction")]
    [Description("Offset adjustment while pressed (usually negative to push shadow up).")]
    [DefaultValue(-3)]
    public int PressedReduceOffsetY
    {
        get => _pressedReduceOffsetY;
        set { _pressedReduceOffsetY = value; Invalidate(); }
    }

    [Category("Interaction")]
    [Description("Blur adjustment while pressed (usually negative to tighten the shadow).")]
    [DefaultValue(-6)]
    public int PressedReduceBlur
    {
        get => _pressedReduceBlur;
        set { _pressedReduceBlur = value; Invalidate(); }
    }

    [Category("Layout")]
    [Description("Clip children to the rounded card shape.")]
    [DefaultValue(true)]
    public bool ClipToCard
    {
        get => _clipToCard;
        set
        {
            if (_clipToCard == value) return;
            _clipToCard = value;
            UpdateCardRegion();
            Invalidate();
        }
    }

    /// <summary>
    /// The layout rectangle that Dock/Anchor uses. We make it the painted card area:
    /// client minus border minus padding.
    /// </summary>
    public override Rectangle DisplayRectangle
    {
        get
        {
            int border = Scale(_borderThickness);
            var r = new Rectangle(
                border + Padding.Left,
                border + Padding.Top,
                Math.Max(0, Width - (border * 2) - Padding.Horizontal),
                Math.Max(0, Height - (border * 2) - Padding.Vertical)
            );
            return r;
        }
    }

    /// <summary>Use Padding as the content inset (default 4) so children don’t ride the border.</summary>
    [Category("Layout")]
    public new Padding Padding
    {
        get => base.Padding;
        set
        {
            if (base.Padding == value) return;
            base.Padding = value;
            PerformLayout();
            Invalidate();
        }
    }

    #endregion

    public MidsCardPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint |
                 ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.Transparent;        // paint parent's background for nice blending
        base.Padding = new Padding(4);       // sensible default content inset
        TabStop = false;
    }

    #region Protected/Internal Methods

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            // Smooth scrolling/less flicker without WS_EX_TRANSPARENT side-effects.
            return cp;
        }
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        UpdateCardRegion();
    }

    protected override void OnPaddingChanged(EventArgs e)
    {
        base.OnPaddingChanged(e);
        PerformLayout();
        Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateCardRegion();
        Invalidate();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        PaintParentBackground(e.Graphics);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = PaintSmoothing;

        int radius = Scale(_cornerRadius);
        int spread = Scale(_shadowSpread);
        int blurSteps = Math.Max(0, _shadowBlur);
        int border = Scale(_borderThickness);
        var offset = GetEffectiveShadowOffset();
        int effBlur = GetEffectiveBlur(blurSteps);

        // Shadow
        if (effBlur > 0 || spread > 0 || offset.X != 0 || offset.Y != 0)
        {
            var shadowRect = ClientRectangle;
            shadowRect = Rectangle.Inflate(shadowRect, -Scale(8), -Scale(8)); // safe margin
            shadowRect.Offset(offset);
            shadowRect.Inflate(spread, spread);

            DrawSoftShadow(g, shadowRect, radius, effBlur, _shadowColor);
        }

        // Card body
        var cardRect = ClientRectangle;
        if (border > 0) cardRect = Rectangle.Inflate(cardRect, -border, -border);

        using (var path = RoundedRect(cardRect, radius))
        {
            if (_useGradientBackground)
            {
                using var lg = new LinearGradientBrush(cardRect, _gradientColor1, _gradientColor2, _gradientAngle, true);
                g.FillPath(lg, path);
            }
            else
            {
                using var br = new SolidBrush(_cardBackColor);
                g.FillPath(br, path);
            }

            if (border > 0)
            {
                using var pen = new Pen(_borderColor, border);
                g.DrawPath(pen, path);
            }
        }

        // Important: paint children after we draw the card
        base.OnPaint(e);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _isHovering = true; Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _isHovering = false; _isPressed = false; Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left) { _isPressed = true; Invalidate(); }
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left) { _isPressed = false; Invalidate(); }
        base.OnMouseUp(e);
    }

    #endregion

    #region Private Methods

    private Point GetEffectiveShadowOffset()
    {
        int ox = _shadowOffsetX;
        int oy = _shadowOffsetY;

        if (_hoverElevates && _isHovering)
            oy += _hoverExtraOffsetY;

        if (_pressedFlattens && _isPressed)
            oy += _pressedReduceOffsetY;

        return new Point(Scale(ox), Scale(oy));
    }

    private int GetEffectiveBlur(int baseBlur)
    {
        int b = baseBlur;
        if (_hoverElevates && _isHovering) b += _hoverExtraBlur;
        if (_pressedFlattens && _isPressed) b += _pressedReduceBlur;
        return Math.Max(0, b);
    }

    private void PaintParentBackground(Graphics g)
    {
        if (Parent == null)
        {
            using var br = new SolidBrush(BackColor.IsEmpty ? SystemColors.Control : BackColor);
            g.FillRectangle(br, ClientRectangle);
            return;
        }

        // Paint the parent's background into our surface (mimics true transparency)
        var state = g.Save();
        try
        {
            var translate = new Point(-Left, -Top);
            g.TranslateTransform(translate.X, translate.Y);
            var pe = new PaintEventArgs(g, new Rectangle(Parent.Location, Parent.Size));
            InvokePaintBackground(Parent, pe);
            InvokePaint(Parent, pe);
        }
        finally
        {
            g.Restore(state);
        }
    }

    private void UpdateCardRegion()
    {
        // Manage a rounded Region so children are clipped to the card outline
        if (!IsHandleCreated) return;

        _cachedRegion?.Dispose();
        if (!_clipToCard || _cornerRadius <= 0)
        {
            Region = null;
            return;
        }

        using var path = RoundedRect(ClientRectangle, Scale(_cornerRadius));
        _cachedRegion = new Region(path);
        Region = _cachedRegion;
    }

    private void DrawSoftShadow(Graphics g, Rectangle rect, int cornerRadius, int blurSteps, Color color)
    {
        if (rect.Width <= 0 || rect.Height <= 0 || blurSteps <= 0) return;

        int layers = blurSteps;
        int maxInflate = Math.Max(1, (int)Math.Round(blurSteps * 0.8));
        for (int i = layers; i >= 1; i--)
        {
            float t = i / (float)layers;
            int inflate = (int)Math.Round(maxInflate * (1 - t)) + 1;
            int alpha = (int)(color.A * t * t);
            if (alpha <= 0) continue;

            var r = Rectangle.Inflate(rect, inflate, inflate);
            int rr = Math.Max(0, cornerRadius + inflate);

            using var path = RoundedRect(r, rr);
            using var br = new SolidBrush(Color.FromArgb(alpha, color));
            g.FillPath(br, path);
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

        int d = radius * 2;
        var arc = new Rectangle(bounds.Location, new Size(d, d));

        // TL
        path.AddArc(arc, 180, 90);
        // TR
        arc.X = bounds.Right - d;
        path.AddArc(arc, 270, 90);
        // BR
        arc.Y = bounds.Bottom - d;
        path.AddArc(arc, 0, 90);
        // BL
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();
        return path;
    }

    private int Scale(int logicalPx) => (int)Math.Round(logicalPx * DpiScale);

    #endregion
}
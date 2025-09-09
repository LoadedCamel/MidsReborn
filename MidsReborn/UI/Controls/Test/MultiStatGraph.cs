using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.ComponentModel;

namespace Mids_Reborn.UI.Controls.Test;

[DesignerCategory("Code")]
[ToolboxItem(true)]
[DefaultEvent(nameof(BarClick))]
public sealed class MultiStatGraph : SKControl
{
    #region Constants

    private static readonly float[] DefaultScales =
    [
        1, 2, 3, 5, 10, 25, 50, 75, 100, 150, 200, 250,
        300, 400, 500, 600, 700, 800, 900, 1000, 1500,
        2000, 2500, 3000, 3500, 4000
    ];

    #endregion

    #region Instance Fields

    private readonly MidsToolTip _toolTip = new()
    {
        ShowAlways = true, 
        AutomaticDelay = 200, 
        ReshowDelay = 50,
        BackColorTop = Color.FromArgb(16, 18, 21),
        BackColorBottom = Color.FromArgb(11, 13, 16),
        BorderColor = Color.FromArgb(58, 65, 75),
        TitleColor = Color.FromArgb(241, 244, 249),
        TextColor = Color.FromArgb(227, 232, 238),
    };

    private readonly List<GraphItem> _items = [];
    private readonly List<float> _perItemScales = [];

    private readonly List<Color> _baseColors = [];
    private readonly List<Color> _enhColors = [];
    private readonly List<Color> _overcapColors = [];

    private bool _created;
    private bool _designSeeded;
    private int _seedCount = 12;
    private bool _useParentBackColor;
    
    private bool _inBatch;
    private bool _clickable;
    private bool _drawRuler;
    private bool _drawHighlight = true;
    private bool _overcap = true;
    private bool _singleLineLabels;
    private bool _drawBarOutlines;

    private int _itemHeight = 18;
    private int _textWidth = 72;
    private int _paddingX = 6;
    private int _paddingY = 4;

    private int _hoverIndex = -1;
    private int _lastMouseX;
    private int _lastMouseY;
    private float _lastBarValue = float.NaN;
    private bool _dragging;

    // Row highlight invalidation helper
    private Rectangle _hoverRowRect = Rectangle.Empty;

    private float _forcedMax; // 0 means auto
    private int _scaleIndex = 8; // 100 by default
    private float _markerValue;

    private Alignment _secondaryLabelPosition = Alignment.Right;
    private GraphStyle _style = GraphStyle.Normal;
    private RulerPosition _rulerPos = RulerPosition.Top;

    private float _fontSizeOverride; // device px override (0 = use Font)

    private Color _colorBase = Color.SteelBlue;
    private Color _colorEnh = Color.MediumSeaGreen;
    private Color _colorOvercap = Color.MediumPurple;
    private Color _colorAbsorbed = Color.FromArgb(160, 245, 245, 245);
    private Color _colorLines = Color.FromArgb(96, 255, 255, 255);
    private Color _colorHighlight = Color.FromArgb(56, 255, 255, 255);
    private Color _colorMarkerInner = Color.White;
    private Color _colorMarkerOuter = Color.Black;
    private Color _borderColor = Color.FromArgb(180, 80, 80, 80);
    private Color _outerBorderColor = Color.FromArgb(220, 40, 40, 40);
    private Color _gradientStart = Color.FromArgb(64, 0, 0, 0);
    private Color _gradientEnd = Color.FromArgb(16, 0, 0, 0);

    private bool _border = true;
    private bool _outerBorder = true;

    // dynamic layout values (computed each paint)
    private float _effectiveTextWidth;

    // parent hooks for UseParentBackColor
    private Control? _hookedParent;

    #endregion

    #region Properties

    [Browsable(false)]
    public IReadOnlyList<GraphItem> Items => _items;

    [Category("Layout"), DefaultValue(18)]
    public int ItemHeight
    {
        get => _itemHeight;
        set { if (value < 8) value = 8; _itemHeight = value; Invalidate(); }
    }

    [Category("Layout"), DefaultValue(72)]
    public int TextWidth
    {
        get => _textWidth;
        set { _textWidth = Math.Max(24, value); Invalidate(); }
    }

    [Category("Layout"), DefaultValue(6)]
    public int PaddingX
    {
        get => _paddingX;
        set { _paddingX = Math.Max(0, value); Invalidate(); }
    }

    [Category("Layout"), DefaultValue(4)]
    public int PaddingY
    {
        get => _paddingY;
        set { _paddingY = Math.Max(0, value); Invalidate(); }
    }

    [Category("Behavior"), DefaultValue(false)]
    public bool Clickable
    {
        get => _clickable;
        set { _clickable = value; }
    }

    [Category("Behavior"), DefaultValue(true)]
    public bool Highlight
    {
        get => _drawHighlight;
        set { _drawHighlight = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "SteelBlue")]
    public Color ColorBase
    {
        get => _colorBase;
        set { _colorBase = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "MediumSeaGreen")]
    public Color ColorEnh
    {
        get => _colorEnh;
        set { _colorEnh = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "MediumPurple")]
    public Color ColorOvercap
    {
        get => _colorOvercap;
        set { _colorOvercap = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "245, 245, 245, 160")]
    public Color ColorAbsorbed
    {
        get => _colorAbsorbed;
        set { _colorAbsorbed = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "255, 255, 255, 96")]
    public Color ColorLines
    {
        get => _colorLines;
        set { _colorLines = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "255, 255, 255, 56")]
    public Color ColorHighlight
    {
        get => _colorHighlight;
        set { _colorHighlight = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "White")]
    public Color ColorMarkerInner
    {
        get => _colorMarkerInner;
        set { _colorMarkerInner = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "Black")]
    public Color ColorMarkerOuter
    {
        get => _colorMarkerOuter;
        set { _colorMarkerOuter = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(true)]
    public bool Border
    {
        get => _border;
        set { _border = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(true)]
    public bool OuterBorder
    {
        get => _outerBorder;
        set { _outerBorder = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "180, 80, 80, 80")]
    public Color BorderColor
    {
        get => _borderColor;
        set { _borderColor = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "220, 40, 40, 40")]
    public Color OuterBorderColor
    {
        get => _outerBorderColor;
        set { _outerBorderColor = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "64, 0, 0, 0")]
    public Color ColorFadeStart
    {
        get => _gradientStart;
        set { _gradientStart = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(typeof(Color), "16, 0, 0, 0")]
    public Color ColorFadeEnd
    {
        get => _gradientEnd;
        set { _gradientEnd = value; Invalidate(); }
    }

    [Category("Layout"), DefaultValue(false)]
    public bool SingleLineLabels
    {
        get => _singleLineLabels;
        set { _singleLineLabels = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(false)]
    public bool DrawBarOutlines
    {
        get => _drawBarOutlines;
        set { _drawBarOutlines = value; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(true)]
    public bool Overcap
    {
        get => _overcap;
        set { _overcap = value; Invalidate(); }
    }

    [Category("Behavior"), DefaultValue(GraphStyle.Normal)]
    public GraphStyle Style
    {
        get => _style;
        set { _style = value; Invalidate(); }
    }

    [Category("Behavior"), DefaultValue(RulerPosition.Top)]
    public RulerPosition RulerPos
    {
        get => _rulerPos;
        set { _rulerPos = value; Invalidate(); }
    }

    [Category("Behavior"), DefaultValue(false)]
    public bool DrawRuler
    {
        get => _drawRuler;
        set { _drawRuler = value; Invalidate(); }
    }

    [Category("Data"), DefaultValue(8)]
    public int ScaleIndex
    {
        get => _scaleIndex;
        set { _scaleIndex = Math.Clamp(value, 0, DefaultScales.Length - 1); Invalidate(); }
    }

    [Browsable(false)]
    public float ScaleValue
    {
        get
        {
            if (_forcedMax > 0) return _forcedMax;
            return DefaultScales[Math.Clamp(_scaleIndex, 0, DefaultScales.Length - 1)];
        }
    }

    [Category("Data"), DefaultValue(0f)]
    public float ForcedMax
    {
        get => _forcedMax;
        set { _forcedMax = value <= 0 ? 0 : value; Invalidate(); }
    }

    [Category("Data"), DefaultValue(0f)]
    public float MarkerValue
    {
        get => _markerValue;
        set { _markerValue = Math.Max(0, value); Invalidate(); }
    }

    [Category("Layout"), DefaultValue(0f)]
    [Description("Override the font size in device pixels. 0 = use control's Font.SizeInPoints.")]
    public float ItemFontSizeOverride
    {
        get => _fontSizeOverride;
        set { _fontSizeOverride = Math.Max(0, value); Invalidate(); }
    }

    [Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public List<Color> BaseBarColors => _baseColors;

    [Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public List<Color> EnhBarColors => _enhColors;

    [Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public List<Color> OvercapColors => _overcapColors;

    [Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public List<float> PerItemScales => _perItemScales;

    [Category("Layout"), DefaultValue(Alignment.Right)]
    public Alignment SecondaryLabelPosition
    {
        get => _secondaryLabelPosition;
        set { _secondaryLabelPosition = value; Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool UseParentBackColor
    {
        get => _useParentBackColor;
        set { _useParentBackColor = value; Invalidate(); }
    }

    [Category("Design"), DefaultValue(12)]
    public int DesignerSampleCount
    {
        get => _seedCount;
        set
        {
            var v = Math.Max(1, value);
            if (_seedCount == v) return;
            _seedCount = v; 

            if (IsInDesignMode)
            {
                if (_designSeeded || _items.Count == 0)
                {
                    SeedDesignTimeData(_seedCount);
                    _designSeeded = true;
                }
            }
            Invalidate();
        }
    }

    [Browsable(false)]
    public int ContentHeight
    {
        get
        {
            var rows = _items.Count;
            if (rows == 0) return 0;
            var topOff = DrawRuler && RulerPos is RulerPosition.Top or RulerPosition.Both ? RulerPixelHeight : 0;
            var bottomOff = DrawRuler && RulerPos is RulerPosition.Bottom or RulerPosition.Both ? RulerPixelHeight : 0;
            return topOff + bottomOff + rows * (_itemHeight + _paddingY) + _paddingY;
        }
    }

    [Browsable(false)]
    public float Max
    {
        get => ScaleValue; // read current scale
        set
        {
            ForcedMax = 0; // ensure we’re using the canned scales
            var scales = DefaultScales; // existing list
            _scaleIndex = Math.Clamp(Array.FindIndex(scales, s => s >= value), 0, scales.Length - 1);
            if (_scaleIndex < 0) _scaleIndex = scales.Length - 1; // if value > largest
            Invalidate();
        }
    }

    private int RulerPixelHeight => (int)Math.Round(15 * DpiScale);

    private float DpiScale => DeviceDpi / 96f;

    private float CurrentFontPx =>
        _fontSizeOverride > 0
            ? _fontSizeOverride
            : Font.SizeInPoints * (DeviceDpi / 72f); // 1pt = 1/72 inch

    // Tunable gaps (scaled by DPI)
    private float Label1ToValueGap => 6f * DpiScale;  // space between Name1 and Name2
    private float ValueToBarGap => 8f * DpiScale;  // space between Name2 and the bar seam
    private float NamePadLeft => 3f * DpiScale;
    private float NamePadRight => 9f * DpiScale;

    private bool IsInDesignMode =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime
        || (Site?.DesignMode ?? false);

    #endregion

    #region Events

    public event Action<float>? BarClick;

    #endregion

    #region Constructor

    public MultiStatGraph()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

        SizeChanged += (_, _) => Invalidate();
        ForeColor = Color.White;
        BackColor = Color.Black;
    }

    #endregion

    #region Public Methods

    public void BeginUpdate() => _inBatch = true;

    public void EndUpdate()
    {
        _inBatch = false;
        Invalidate();
    }

    public void Clear()
    {
        _items.Clear();
        _perItemScales.Clear();
        if (!_inBatch) Invalidate();
    }

    public void AddItem(string name, float baseValue, float enhValue, string? tip = null)
        => AddItem(name, null, baseValue, enhValue, 0, 0, tip);

    public void AddItem(string name, string? name2, float baseValue, float enhValue, string? tip = null)
        => AddItem(name, name2, baseValue, enhValue, 0, 0, tip);

    public void AddItemPair(string name, string name2, float baseValue, float enhValue, float overcap, string? tip = null)
        => AddItem(name, name2, baseValue, enhValue, overcap, 0, tip);

    public void AddItem(string? name, string? name2, float baseValue, float enhValue, float overcap, float absorbed, string? tip)
    {
        _items.Add(new GraphItem
        {
            Name = name ?? string.Empty,
            Name2 = name2 ?? string.Empty,
            ValueBase = baseValue,
            ValueEnh = enhValue,
            ValueOvercap = overcap,
            ValueAbsorbed = absorbed,
            Tip = tip ?? string.Empty
        });

        if (!_inBatch) Invalidate();
    }

    public float GetMaxValue()
    {
        if (_items.Count == 0) return 0;
        float max = 0;
        foreach (var it in _items)
        {
            max = Math.Max(max, it.ValueBase);
            max = Math.Max(max, it.ValueEnh);
            max = Math.Max(max, it.ValueOvercap);
        }
        return max;
    }

    [Browsable(false)]
    public void Draw() => Invalidate();

    #endregion

    #region Protected Overrides

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var w = e.Info.Width;
        var h = e.Info.Height;
        if (w <= 0 || h <= 0) return;

        var backColor = ResolveEffectiveBackColor();
        canvas.Clear(backColor.ToSKColor());

        // If we're inheriting the parent background, skip our gradient overlay
        bool drawGradient = !_useParentBackColor;

        // --- fonts/paints (unchanged crisp settings) ---
        var fontPx = CurrentFontPx;
        using var typeface = ResolveTypeface(Font);
        using var textFont = new SKFont(typeface, fontPx);
        textFont.Edging = SKFontEdging.SubpixelAntialias;

        if (fontPx <= 12f * DpiScale) textFont.Hinting = SKFontHinting.Full;

        using var textFill = new SKPaint();
        textFill.IsAntialias = true;
        textFill.Color = ForeColor.ToSKColor();
        textFill.Style = SKPaintStyle.Fill;

        using var textStroke = new SKPaint();
        textStroke.IsAntialias = true;
        textStroke.Color = SKColors.Black;
        textStroke.Style = SKPaintStyle.Stroke;
        textStroke.StrokeWidth = Math.Max(1f, 3f * (fontPx / 14f));
        textStroke.StrokeJoin = SKStrokeJoin.Round;
        textStroke.StrokeCap = SKStrokeCap.Round;

        // --- MEASURE PASS ---
        _effectiveTextWidth = ComputeEffectiveTextWidth(textFont, out var maxLabel1, out var maxLabel2);

        // --- background gradient seam aligns to *effective* label width ---
        if (drawGradient)
        {
            using (var bg = new SKPaint())
            {
                bg.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(_effectiveTextWidth, 0),
                    new SKPoint(w, 0),
                    [_gradientStart.ToSKColor(), _gradientStart.ToSKColor(), _gradientEnd.ToSKColor()],
                    [0f, 0f, 1f],
                    SKShaderTileMode.Clamp);
                bg.Style = SKPaintStyle.Fill;
                bg.IsAntialias = true;
                canvas.DrawRect(new SKRect(0, 0, w, h), bg);
            }
        }

        // Graph area begins after the effective label column
        var drawArea = new SKRect(_effectiveTextWidth, 0, w - 1, h - 1);

        // Ruler (use effective seam too)
        var topOff = 0; var bottomOff = 0;
        if (_drawRuler)
        {
            RenderRulerStrips(canvas, drawArea);
            switch (_rulerPos)
            {
                case RulerPosition.Both: topOff = bottomOff = RulerPixelHeight; break;
                case RulerPosition.Bottom: bottomOff = RulerPixelHeight; break;
                default: topOff = RulerPixelHeight; break;
            }
        }

        drawArea = new SKRect(drawArea.Left, drawArea.Top + topOff, drawArea.Right, drawArea.Bottom - bottomOff);

        // --- VERTICAL CENTERING of the rows block ---
        var rowsTop = ComputeRowsBlockTop(drawArea);
        var rowSpan = _itemHeight + _paddingY;

        // Hover highlight uses the full row slot (content + gap), but we’ll center bar/text inside
        if (_drawHighlight && _hoverIndex >= 0 && _hoverIndex < _items.Count)
        {
            var ySlotTop = rowsTop + _hoverIndex * rowSpan;

            using var hl = new SKPaint();
            hl.Color = _colorHighlight.ToSKColor();
            hl.Style = SKPaintStyle.Fill;
            hl.IsAntialias = true;

            canvas.DrawRect(new SKRect(0, ySlotTop, w, ySlotTop + rowSpan), hl);
        }

        // Precompute inner row content rect vertical bounds (bar + text are centered inside slot)
        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];

            // Full row slot (for highlight & hit-test)
            var ySlotTop = rowsTop + i * rowSpan;

            // Inner content rect (center ItemHeight within the slot)
            var innerTop = ySlotTop + (rowSpan - _itemHeight) / 2f;
            var innerBottom = innerTop + _itemHeight;

            // --- label/value rects inside the fixed label column ---
            var labelColumnLeft = NamePadLeft;
            var labelColumnRight = _effectiveTextWidth - NamePadRight;
            var labelRect = new SKRect(labelColumnLeft, innerTop, labelColumnRight, innerBottom);

            // Name|Name2 split or Name + Name2
            string label1, label2;
            var sep = item.Name?.IndexOf('|') ?? -1;
            if (sep >= 0) { label1 = item.Name![..sep]; label2 = item.Name![(sep + 1)..]; }
            else { label1 = item.Name ?? string.Empty; label2 = item.Name2 ?? string.Empty; }

            bool hasLabel2 = !string.IsNullOrWhiteSpace(label2) && !string.Equals(label2, label1, StringComparison.Ordinal);

            // SINGLE LINE: Name1 left, Name2 right, guaranteed min gap by the measured column widths
            if (_style != GraphStyle.Twin || _singleLineLabels)
            {
                // Name1
                DrawTextLeft(canvas, textFont, textFill, textStroke, labelRect, label1);

                // Name2 (closer to bar): right-aligned with a small gap to bar
                if (hasLabel2)
                {
                    var valueRect = new SKRect(
                        labelColumnRight - (maxLabel2 + ValueToBarGap), // reserve measured width + fixed gap to bar
                        innerTop,
                        labelColumnRight,
                        innerBottom);
                    DrawTextRight(canvas, textFont, textFill, textStroke, valueRect, label2);
                }
            }
            else
            {
                // TWIN MODE: stack, keep Name2 toward the bar with alignment control
                var half = (int)Math.Round(_itemHeight / 2f);
                var r1 = new SKRect(labelRect.Left, labelRect.Top, labelRect.Right, labelRect.Top + half);
                var r2 = new SKRect(labelRect.Left, labelRect.Top + half, labelRect.Right, labelRect.Bottom);

                DrawTextLeft(canvas, textFont, textFill, textStroke, r1, label1);

                if (hasLabel2)
                {
                    if (_secondaryLabelPosition == Alignment.Right)
                        DrawTextRight(canvas, textFont, textFill, textStroke, r2, label2);
                    else
                        DrawTextLeft(canvas, textFont, textFill, textStroke, r2, label2, leftInset: maxLabel1 + Label1ToValueGap);
                }
            }

            // --- bar rect takes the remainder after the label column + PaddingX ---
            var barsRect = new SKRect(_effectiveTextWidth + _paddingX, innerTop, drawArea.Right - _paddingX, innerBottom);

            var rowScale = (_perItemScales.Count == _items.Count) ? Math.Max(1e-6f, _perItemScales[i]) : ScaleValue;
            DrawRowBars(canvas, barsRect, item, rowScale, i);

            // Marker per row
            if (_markerValue > 0)
            {
                var x = barsRect.Left + (barsRect.Width * (_markerValue / rowScale));
                x = Math.Clamp(x, barsRect.Left, barsRect.Right);
                using var penOuter = new SKPaint();
                penOuter.Style = SKPaintStyle.Stroke;
                penOuter.Color = _colorMarkerOuter.ToSKColor();
                penOuter.StrokeWidth = 2;
                penOuter.IsAntialias = true;

                using var penInner = new SKPaint();
                penInner.Style = SKPaintStyle.Stroke;
                penInner.Color = _colorMarkerInner.ToSKColor();
                penInner.StrokeWidth = 1;
                penInner.IsAntialias = true;

                canvas.DrawLine(x, barsRect.Top, x, barsRect.Bottom, penOuter);
                canvas.DrawLine(x, barsRect.Top, x, barsRect.Bottom, penInner);
            }
        }

        // Borders (use effective label seam)
        if (_border)
        {
            using var pen = new SKPaint();
            pen.Style = SKPaintStyle.Stroke;
            pen.Color = _borderColor.ToSKColor();
            pen.StrokeWidth = 1;
            pen.IsAntialias = true;

            var inner = new SKRect(_effectiveTextWidth, topOff, w - 1, h - 1 - bottomOff);
            canvas.DrawRect(inner, pen);
        }
        if (_outerBorder)
        {
            using var pen = new SKPaint();
            pen.Style = SKPaintStyle.Stroke;
            pen.Color = _outerBorderColor.ToSKColor();
            pen.StrokeWidth = 1;
            pen.IsAntialias = true;

            canvas.DrawRect(new SKRect(0.5f, 0.5f, w - 0.5f, h - 0.5f), pen);
        }
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();

        if (IsInDesignMode && _items.Count == 0)
        {
            SeedDesignTimeData(DesignerSampleCount);
            Invalidate(); // force a paint in the designer
        }
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        Invalidate();
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);

        if (_hookedParent != null)
            _hookedParent.BackColorChanged -= Parent_BackColorChanged;

        _hookedParent = Parent;

        if (_hookedParent != null)
            _hookedParent.BackColorChanged += Parent_BackColorChanged;

        if (_useParentBackColor) Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        // Early-out if pointer didn't actually move
        if (e.X == _lastMouseX && e.Y == _lastMouseY) return;
        _lastMouseX = e.X; _lastMouseY = e.Y;

        base.OnMouseMove(e);

        // Hit-test once; use returned rect to avoid recomputing geometry
        var (row, rowRect) = HitTest(e.Location);
        if (row != _hoverIndex)
        {
            var oldRect = _hoverRowRect;

            _hoverIndex = row;
            _hoverRowRect = row >= 0 ? rowRect : Rectangle.Empty;

            if (row >= 0 && row < _items.Count && !string.IsNullOrEmpty(_items[row].Tip))
                _toolTip.Show(_items[row].Tip, this, e.Location + new Size(16, 16), 3000);
            else
                _toolTip.Hide(this);

            // Targeted repaint: old hover region and new hover region only
            if (!oldRect.IsEmpty) Invalidate(oldRect);
            if (!_hoverRowRect.IsEmpty) Invalidate(_hoverRowRect);
        }

        // Scrub: only emit when value actually changes (with small tolerance)
        if (_clickable && _dragging && e.Button == MouseButtons.Left)
        {
            var val = ValueAtPoint(e.Location);
            if (!ApproximatelyEqual(val, _lastBarValue))
            {
                _lastBarValue = val;
                BarClick?.Invoke(val);
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (!_clickable || e.Button != MouseButtons.Left) return;

        _dragging = true;
        _lastBarValue = ValueAtPoint(e.Location);
        BarClick?.Invoke(_lastBarValue);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _dragging = false;
        _lastBarValue = float.NaN;
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        // Reset hover + debouncing state
        if (_hoverIndex != -1 || !_hoverRowRect.IsEmpty)
        {
            var oldRect = _hoverRowRect;
            _hoverIndex = -1;
            _hoverRowRect = Rectangle.Empty;
            if (!oldRect.IsEmpty) Invalidate(oldRect);
        }
        _toolTip.Hide(this);
        _lastMouseX = int.MinValue;
        _lastMouseY = int.MinValue;
        _lastBarValue = float.NaN;
    }

    #endregion

    #region Private Methods

    private void Parent_BackColorChanged(object? sender, EventArgs e)
    {
        if (_useParentBackColor) Invalidate();
    }

    private void SeedDesignTimeData(int count)
    {
        BeginUpdate();
        Clear();
        ForcedMax = 100;

        // Generate up to 'count' demo rows
        var n = Math.Max(3, count); // keep at least a few rows
        for (int i = 0; i < n; i++)
        {
            var baseVal = 20 + (i * 5) % 60;
            var enhVal = baseVal + 8;
            var over = enhVal + 10;
            AddItemPair($"Stat {i + 1}", $"{baseVal}%", baseVal, enhVal, over, "Designer sample");
        }

        EndUpdate();
    }

    private void RenderRulerStrips(SKCanvas canvas, SKRect graphArea)
    {
        var tickCount = 10;
        var rulerH = RulerPixelHeight;

        var topY = 0f;
        var botY = Height - rulerH;

        // Text setup (SkiaSharp 3.x)
        var fontPx = Math.Max(10f * DpiScale, CurrentFontPx * 0.85f);
        using var typeface = ResolveTypeface(Font);
        using var font = new SKFont(typeface, fontPx);
        font.Edging = SKFontEdging.SubpixelAntialias;
        if (fontPx <= 12f * DpiScale) font.Hinting = SKFontHinting.Full;
        using var ticPen = new SKPaint();
        ticPen.Color = _colorLines.ToSKColor();
        ticPen.Style = SKPaintStyle.Stroke;
        ticPen.StrokeWidth = 1f;
        ticPen.IsAntialias = true;
        using var textFill = new SKPaint();
        textFill.Color = ForeColor.ToSKColor();
        textFill.IsAntialias = true;
        textFill.Style = SKPaintStyle.Fill;
        using var textStroke = new SKPaint();
        textStroke.Color = SKColors.Black;
        textStroke.IsAntialias = true;
        textStroke.Style = SKPaintStyle.Stroke;
        textStroke.StrokeWidth = Math.Max(1, 2f * (fontPx / 14f));

        void DrawStrip(float y, bool bottom)
        {
            var baseY = bottom ? y : y + rulerH;

            // baseline line
            canvas.DrawLine(graphArea.Left, baseY, graphArea.Right, baseY, ticPen);

            for (int i = 0; i <= tickCount; i++)
            {
                var t = i / (float)tickCount;
                var x = graphArea.Left + t * (graphArea.Width - _paddingX * 2) + _paddingX;

                var ticTop = bottom ? baseY - 6 : baseY - rulerH + 6;
                var ticBottom = bottom ? baseY : baseY - rulerH;
                canvas.DrawLine(x, ticTop, x, ticBottom, ticPen);

                var v = ScaleValue * t;
                var label = ((int)Math.Round(v)).ToString();
                DrawBlobAt(canvas, font, textFill, textStroke, label, x, bottom ? baseY - 2 : baseY - rulerH + fontPx + 2, SKTextAlign.Center);
            }
        }

        if (_rulerPos is RulerPosition.Top or RulerPosition.Both)
            DrawStrip(topY, bottom: false);
        if (_rulerPos is RulerPosition.Bottom or RulerPosition.Both)
            DrawStrip(botY, bottom: true);
    }

    private void DrawRowBars(SKCanvas canvas, SKRect barsRect, GraphItem item, float scale, int rowIndex)
    {
        // Calculate widths
        float maxW = Math.Max(0, barsRect.Width);
        if (maxW <= 0 || scale <= 1e-6f) return;

        float wBase = Math.Clamp(maxW * (item.ValueBase / scale), 0, maxW);
        float wEnh = Math.Clamp(maxW * (item.ValueEnh / scale), 0, maxW);
        float wOver = Math.Clamp(maxW * (item.ValueOvercap / scale), 0, maxW);
        float wAbs = Math.Clamp(maxW * (item.ValueAbsorbed / scale), 0, maxW);

        var rectBase = new SKRect(barsRect.Left, barsRect.Top, barsRect.Left + wBase, barsRect.Bottom);
        var rectEnh = new SKRect(barsRect.Left, barsRect.Top, barsRect.Left + wEnh, barsRect.Bottom);
        var rectOver = new SKRect(barsRect.Left, barsRect.Top, barsRect.Left + wOver, barsRect.Bottom);
        var rectAbs = new SKRect(barsRect.Left, barsRect.Top, barsRect.Left + wAbs, barsRect.Bottom);

        var baseColor = (_baseColors.Count > 0) ? _baseColors[rowIndex % _baseColors.Count] : _colorBase;
        var enhColor = (_enhColors.Count > 0) ? _enhColors[rowIndex % _enhColors.Count] : _colorEnh;
        var overColor = (_overcapColors.Count > 0) ? _overcapColors[rowIndex % _overcapColors.Count] : _colorOvercap;

        using var pFill = new SKPaint();
        pFill.IsAntialias = true;
        pFill.Style = SKPaintStyle.Fill;
        using var pLine = new SKPaint();
        pLine.IsAntialias = true;
        pLine.Style = SKPaintStyle.Stroke;
        pLine.Color = _colorLines.ToSKColor();
        pLine.StrokeWidth = 1;

        // Draw order: big first so small stays visible
        if (_overcap && wOver > Math.Max(wBase, wEnh))
        {
            pFill.Color = overColor.ToSKColor();
            canvas.DrawRect(rectOver, pFill);
            if (_drawBarOutlines) canvas.DrawRect(rectOver, pLine);
        }

        if (_style != GraphStyle.EnhOnly)
        {
            pFill.Color = baseColor.ToSKColor();
            canvas.DrawRect(rectBase, pFill);
            if (_drawBarOutlines) canvas.DrawRect(rectBase, pLine);
        }

        if (_style != GraphStyle.BaseOnly)
        {
            pFill.Color = enhColor.ToSKColor();
            canvas.DrawRect(rectEnh, pFill);
            if (_drawBarOutlines) canvas.DrawRect(rectEnh, pLine);
        }

        if (_overcap && wOver > 0 && wOver <= Math.Max(wBase, wEnh))
        {
            pFill.Color = overColor.ToSKColor();
            canvas.DrawRect(rectOver, pFill);
            if (_drawBarOutlines) canvas.DrawRect(rectOver, pLine);
        }

        if (item.ValueAbsorbed > 0 && wAbs > 0)
        {
            using var pAbs = new SKPaint();
            pAbs.IsAntialias = true;
            pAbs.Color = _colorAbsorbed.ToSKColor();
            pAbs.Style = SKPaintStyle.Fill;
            canvas.DrawRect(rectAbs, pAbs);
        }
    }

    private void DrawTextLeft(SKCanvas c, SKFont font, SKPaint fill, SKPaint stroke, SKRect rect, string? text, float leftInset = 0)
    {
        if (string.IsNullOrEmpty(text)) return;

        font.MeasureText(text);
        var m = font.Metrics;
        var baselineY = MathF.Round(rect.MidY + ((m.Descent - m.Ascent) / 2f) - m.Descent);
        var x = MathF.Round(rect.Left + leftInset);

        using var blob = SKTextBlob.Create(text, font);
        c.DrawText(blob, x, baselineY, stroke);
        c.DrawText(blob, x, baselineY, fill);
    }

    private void DrawTextRight(SKCanvas c, SKFont font, SKPaint fill, SKPaint stroke, SKRect rect, string? text)
    {
        if (string.IsNullOrEmpty(text)) return;

        var width = font.MeasureText(text);
        var m = font.Metrics;
        var baselineY = MathF.Round(rect.MidY + ((m.Descent - m.Ascent) / 2f) - m.Descent);
        var x = MathF.Round(rect.Right - width);

        using var blob = SKTextBlob.Create(text, font);
        c.DrawText(blob, x, baselineY, stroke);
        c.DrawText(blob, x, baselineY, fill);
    }

    private void DrawBlobAt(SKCanvas c, SKFont font, SKPaint fill, SKPaint stroke, string text, float x, float baselineY, SKTextAlign align)
    {
        if (string.IsNullOrEmpty(text)) return;

        var width = font.MeasureText(text);
        var drawX = align switch
        {
            SKTextAlign.Center => x - width / 2f,
            SKTextAlign.Right => x - width,
            _ => x
        };

        drawX = MathF.Round(drawX);
        baselineY = MathF.Round(baselineY);

        using var blob = SKTextBlob.Create(text, font);
        c.DrawText(blob, drawX, baselineY, stroke);
        c.DrawText(blob, drawX, baselineY, fill);
    }

    private (int Row, Rectangle RowRect) HitTest(Point pt)
    {
        if (_items.Count == 0) return (-1, Rectangle.Empty);

        // use current font to recompute effective seam (fast)
        using var tf = ResolveTypeface(Font);
        using var f = new SKFont(tf, CurrentFontPx);
        var seam = ComputeEffectiveTextWidth(f, out _, out _);

        var rulerH = DrawRuler ? RulerPixelHeight : 0;
        var topOff = DrawRuler && (RulerPos is RulerPosition.Top or RulerPosition.Both) ? rulerH : 0;
        var bottomOff = DrawRuler && (RulerPos is RulerPosition.Bottom or RulerPosition.Both) ? rulerH : 0;

        var drawArea = SKRect.Create(seam, topOff, Width - seam, Height - topOff - bottomOff);
        var rowsTop = ComputeRowsBlockTop(drawArea);
        var rowSpan = _itemHeight + _paddingY;

        for (int i = 0; i < _items.Count; i++)
        {
            var ySlotTop = rowsTop + i * rowSpan;
            var r = Rectangle.FromLTRB(0, (int)Math.Floor(ySlotTop), Width, (int)Math.Ceiling(ySlotTop + rowSpan));
            if (r.Contains(pt)) return (i, r);
        }
        return (-1, Rectangle.Empty);
    }

    private float ValueAtPoint(Point pt)
    {
        if (_items.Count == 0 || Width <= 0) return 0;

        using var tf = ResolveTypeface(Font);
        using var f = new SKFont(tf, CurrentFontPx);
        var seam = ComputeEffectiveTextWidth(f, out _, out _);

        var drawLeft = seam + _paddingX;
        var drawRight = Width - _paddingX;
        var graphW = Math.Max(0, drawRight - drawLeft);
        if (graphW <= 0) return 0;

        var (row, _) = HitTest(pt);
        if (row < 0) return 0;

        var scale = (_perItemScales.Count == _items.Count) ? Math.Max(1e-6f, _perItemScales[row]) : ScaleValue;
        var x = Math.Clamp(pt.X - drawLeft, 0, graphW);
        return Math.Max(0, (x / graphW) * scale);
    }


    private static SKTypeface ResolveTypeface(Font? winFont)
    {
        var family = winFont?.FontFamily.Name ?? "Segoe UI";

        var weight = (winFont != null && winFont.Style.HasFlag(FontStyle.Bold))
            ? SKFontStyleWeight.Bold
            : SKFontStyleWeight.Normal;

        var slant = (winFont != null && winFont.Style.HasFlag(FontStyle.Italic))
            ? SKFontStyleSlant.Italic
            : SKFontStyleSlant.Upright;

        // prefer FromFamilyName with an explicit SKFontStyle
        var style = new SKFontStyle(weight, SKFontStyleWidth.Normal, slant);
        var tf = SKTypeface.FromFamilyName(family, style);

        return tf ?? SKTypeface.Default;
    }

    // Measure pass: compute the effective label column width for this font/data
    private float ComputeEffectiveTextWidth(SKFont font, out float maxLabel1, out float maxLabel2)
    {
        float m1 = 0, m2 = 0;
        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            // split "Name|Name2" or use Name / Name2
            string label1, label2;
            var sep = item.Name?.IndexOf('|') ?? -1;
            if (sep >= 0) { label1 = item.Name![..sep]; label2 = item.Name![(sep + 1)..]; }
            else { label1 = item.Name ?? string.Empty; label2 = item.Name2 ?? string.Empty; }

            m1 = Math.Max(m1, font.MeasureText(label1));
            if (!string.IsNullOrWhiteSpace(label2))
                m2 = Math.Max(m2, font.MeasureText(label2));
        }

        maxLabel1 = m1; maxLabel2 = m2;

        // how much is needed: pads + label1 + gap + label2 + gap to bar + right pad
        var needed = NamePadLeft + m1 + (m2 > 0 ? Label1ToValueGap + m2 : 0) + ValueToBarGap + NamePadRight;

        // honor the configured TextWidth as a minimum (legacy behavior)
        return Math.Max(_textWidth, needed);
    }

    // Given the graph draw area and rows count, return the top Y that centers the rows block
    private float ComputeRowsBlockTop(SKRect drawArea)
    {
        var rows = _items.Count;
        if (rows <= 0) return drawArea.Top;

        var rowSpan = _itemHeight + _paddingY;                 // full row slot (content + gap)
        var contentH = _paddingY + rows * rowSpan;             // what we actually draw
        var extra = drawArea.Height - contentH;
        return drawArea.Top + (extra > 0 ? extra / 2f : 0f);   // center the block if there is room
    }

    private static bool ApproximatelyEqual(float a, float b, float eps = 0.01f)
        => Math.Abs(a - b) <= eps;

    private Color ResolveEffectiveBackColor()
    {
        if (!_useParentBackColor) return BackColor;

        // Walk up until we hit a non-transparent BackColor
        Control? c = Parent;
        while (c != null && c.BackColor == Color.Transparent)
            c = c.Parent;

        // If nothing found, fall back to our own BackColor or a sane default
        return c?.BackColor ?? BackColor;
    }

    #endregion

    #region Nested Types

    public enum GraphStyle
    {
        Normal = 0,
        BaseOnly = 1,
        EnhOnly = 2,
        Twin = 3
    }

    public enum RulerPosition
    {
        Top = 0,
        Bottom = 1,
        Both = 2
    }

    public enum Alignment
    {
        Left = 0,
        Right = 1
    }

    public sealed class GraphItem
    {
        public string? Name { get; set; } = string.Empty;
        public string? Name2 { get; set; } = string.Empty;
        public float ValueBase { get; set; }
        public float ValueEnh { get; set; }
        public float ValueOvercap { get; set; }
        public float ValueAbsorbed { get; set; }
        public string? Tip { get; set; } = string.Empty;
    }

    #endregion
}
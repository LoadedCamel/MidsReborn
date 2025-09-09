// NuGet at runtime: SkiaSharp, SkiaSharp.Views.Desktop
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls
{
    [DefaultEvent(nameof(BarClick))]
    public class MultiGraph : UserControl
    {
        #region Enums / Delegates

        public enum GraphStyle { Overlay, BaseOnly, EnhOnly, Twin }
        public enum RulerPosition { Top, Bottom, Both }
        public enum Alignment { Left, Right }
        public enum SamplePreset { None = 0, Defense = 1, Resistance = 2 }
        public enum SecondaryLabelKind { None, Name2, ValueBase, ValueEnh, ValueOvercap, ValueAbsorbed }

        public delegate void BarClickEventHandler(float value);
        public event BarClickEventHandler? BarClick;

        #endregion

        #region Constants

        private const int RULER_H = 15;

        #endregion

        #region Fields

        // Appearance
        private Color _colorBase = Color.RoyalBlue;
        private Color _colorEnh = Color.Goldenrod;
        private Color _colorOvercap = Color.DimGray;
        private Color _colorAbsorbed = Color.Gainsboro;
        private Color _colorLines = Color.Black;
        private Color _colorHighlight = Color.FromArgb(128, 128, 255);
        private Color _colorMarkerInner = Color.Black;
        private Color _colorMarkerOuter = Color.Yellow;
        private Color _borderColor = Color.Black;
        private Color _fadeStart = Color.Black;
        private Color _fadeEnd = Color.Red;
        private Color _rulerColor = Color.FromArgb(192, 192, 255);
        private Color _secondaryLabelColor = Color.Empty;

        // Layout
        private int _itemHeight = 12;
        private int _rowGap = 6;
        private int _textWidth = 120;           // manual fallback width if AutoTextWidth=false
        private int _scaleHeight = 32;
        private GraphStyle _style = GraphStyle.Overlay;

        // Scale / ruler
        private bool _drawRuler = true;
        private RulerPosition _rulerPos = RulerPosition.Top;
        private float _scaleValue = 100f;
        private int _forcedMax = 0;
        private readonly List<float> _scales = new();

        // Behavior
        private bool _overcap = false;
        private bool _lines = false;
        private bool _border = true;
        private bool _outerBorder = false;
        private bool _highlight = true;
        private bool _clickable = false;
        private bool _differentiate = false;

        // Marker / fonts
        private float _markerValue = 0f;
        private float _fontPxOverride = 0f;

        // Samples
        private bool _showDesignSamples = true;
        private bool _useSamplesAtRuntime = false;
        private int _sampleCount = 11;
        private SamplePreset _samplePreset = SamplePreset.Defense;

        // Secondary label (numbers / name2)
        private SecondaryLabelKind _secondaryKind = SecondaryLabelKind.ValueEnh;
        private int _secondaryDecimals = 2;
        private string _secondarySuffix = "%";
        private bool _secondaryRightAlign = true;

        // Name/value spacing and alignment
        private int _labelValuePadding = 8;                // used only when SecondaryRightAlign=false (inline)
        private Alignment _labelAlignment = Alignment.Left;
        private int _labelValueMinGapSpaces = 2;           // when SecondaryRightAlign=true, enforce gap = spaces * spaceWidth
        private bool _autoTextWidth = true;                // compute text column from content
        private int _autoTextWidthMin = 80;                // clamp
        private int _autoTextWidthMax = 500;               // clamp

        // Data
        private readonly List<Item> _items = new();
        private readonly List<float> _perItemScales = new();
        private readonly List<Color> _baseBarColors = new(), _enhBarColors = new(), _overcapColors = new();

        // Runtime / design-time plumbing
        private SKControl? _sk;
        private readonly ToolTip _tip = new() { AutoPopDelay = 10000, InitialDelay = 500, ReshowDelay = 100 };
        private bool _noDraw;
        private int _hoverRow = -1;

        private readonly List<Item> _empty = new();

        #endregion

        #region Properties (invalidate on set)

        [Category("Appearance")] public Color ColorBase { get => _colorBase; set { if (_colorBase == value) return; _colorBase = value; InvalidateAll(); } }
        [Category("Appearance")] public Color ColorEnh { get => _colorEnh; set { if (_colorEnh == value) return; _colorEnh = value; InvalidateAll(); } }
        [Category("Appearance")] public Color ColorOvercap { get => _colorOvercap; set { if (_colorOvercap == value) return; _colorOvercap = value; InvalidateAll(); } }
        [Category("Appearance")] public Color ColorAbsorbed { get => _colorAbsorbed; set { if (_colorAbsorbed == value) return; _colorAbsorbed = value; InvalidateAll(); } }
        [Category("Appearance")] public Color ColorLines { get => _colorLines; set { if (_colorLines == value) return; _colorLines = value; InvalidateAll(); } }
        [Category("Appearance")] public Color ColorHighlight { get => _colorHighlight; set { if (_colorHighlight == value) return; _colorHighlight = value; InvalidateAll(); } }
        [Category("Appearance")] public Color ColorMarkerInner { get => _colorMarkerInner; set { if (_colorMarkerInner == value) return; _colorMarkerInner = value; InvalidateAll(); } }
        [Category("Appearance")] public Color ColorMarkerOuter { get => _colorMarkerOuter; set { if (_colorMarkerOuter == value) return; _colorMarkerOuter = value; InvalidateAll(); } }
        [Category("Appearance")] public Color BorderColor { get => _borderColor; set { if (_borderColor == value) return; _borderColor = value; InvalidateAll(); } }
        [Category("Appearance")][Description("Left-to-right background gradient start color.")] public Color ColorFadeStart { get => _fadeStart; set { if (_fadeStart == value) return; _fadeStart = value; InvalidateAll(); } }
        [Category("Appearance")][Description("Left-to-right background gradient end color.")] public Color ColorFadeEnd { get => _fadeEnd; set { if (_fadeEnd == value) return; _fadeEnd = value; InvalidateAll(); } }
        [Category("Appearance")] public Color RulerColor { get => _rulerColor; set { if (_rulerColor == value) return; _rulerColor = value; InvalidateAll(); } }
        [Category("Appearance")][Description("Color for secondary labels; Empty uses ForeColor.")] public Color SecondaryLabelColor { get => _secondaryLabelColor; set { _secondaryLabelColor = value; InvalidateAll(); } }

        [Category("Layout")] public int ItemHeight { get => _itemHeight; set { value = Math.Max(1, value); if (_itemHeight == value) return; _itemHeight = value; InvalidateAll(); } }
        [Category("Layout")] public int RowGap { get => _rowGap; set { value = Math.Max(0, value); if (_rowGap == value) return; _rowGap = value; InvalidateAll(); } }
        [Category("Layout")][Description("Fallback width of the text column when AutoTextWidth=false.")] public int TextWidth { get => _textWidth; set { value = Math.Max(0, value); if (_textWidth == value) return; _textWidth = value; InvalidateAll(); } }
        [Category("Layout")] public GraphStyle Style { get => _style; set { if (_style == value) return; _style = value; InvalidateAll(); } }

        [Category("Ruler")] public bool DrawRuler { get => _drawRuler; set { if (_drawRuler == value) return; _drawRuler = value; InvalidateAll(); } }
        [Category("Ruler")] public RulerPosition RulerPos { get => _rulerPos; set { if (_rulerPos == value) return; _rulerPos = value; InvalidateAll(); } }

        [Browsable(false)] public float ScaleValue { get => _scaleValue; private set { if (Math.Abs(_scaleValue - value) < float.Epsilon) return; _scaleValue = Math.Max(1f, value); InvalidateAll(); } }
        [Category("Ruler")]
        [Description("Optional fixed maximum. Set to 0 to auto-pick from data.")]
        public int ForcedMax { get => _forcedMax; set { if (_forcedMax == value) return; _forcedMax = value; if (_forcedMax > 0) ScaleValue = _forcedMax; else Max = GetMaxValue(); } }
        [Category("Ruler")][Description("Choose a scale from the internal scale list.")] public int ScaleIndex { get => WhichScale(ScaleValue); set { if (value >= 0 && value < _scales.Count) ScaleValue = _scales[value]; } }

        [Category("Scale Band")] public bool ShowScale { get => _showScale; set { if (_showScale == value) return; _showScale = value; InvalidateAll(); } }
        private bool _showScale = false;
        [Category("Scale Band")] public int ScaleHeight { get => _scaleHeight; set { value = Math.Max(0, value); if (_scaleHeight == value) return; _scaleHeight = value; InvalidateAll(); } }

        [Category("Behavior")] public bool Overcap { get => _overcap; set { if (_overcap == value) return; _overcap = value; InvalidateAll(); } }
        [Category("Behavior")] public bool Lines { get => _lines; set { if (_lines == value) return; _lines = value; InvalidateAll(); } }
        [Category("Behavior")] public bool Border { get => _border; set { if (_border == value) return; _border = value; InvalidateAll(); } }
        [Category("Behavior")] public bool OuterBorder { get => _outerBorder; set { if (_outerBorder == value) return; _outerBorder = value; InvalidateAll(); } }
        [Category("Behavior")] public bool Highlight { get => _highlight; set { if (_highlight == value) return; _highlight = value; InvalidateAll(); } }
        [Category("Behavior")] public bool Clickable { get => _clickable; set { if (_clickable == value) return; _clickable = value; InvalidateAll(); } }
        [Category("Behavior")][Description("Differentiate bar colors per item using the corresponding color lists.")] public bool DifferentiateColors { get => _differentiate; set { if (_differentiate == value) return; _differentiate = value; InvalidateAll(); } }

        [Category("Marker")] public float MarkerValue { get => _markerValue; set { if (Math.Abs(_markerValue - value) < float.Epsilon) return; _markerValue = value; InvalidateAll(); } }
        [Category("Fonts")][Description("Optional override for Skia text size in device pixels; 0 uses ItemHeight.")] public float ItemFontSizeOverride { get => _fontPxOverride; set { if (Math.Abs(_fontPxOverride - value) < float.Epsilon) return; _fontPxOverride = Math.Max(0f, value); InvalidateAll(); } }

        // Samples
        [Category("Samples")]
        [Description("Show curated dummy items in the designer when the list is empty.")]
        public bool ShowDesignTimeSamples { get => _showDesignSamples; set { if (_showDesignSamples == value) return; _showDesignSamples = value; InvalidateAll(); } }

        [Category("Samples")]
        [Description("Use the dummy items even at runtime (ignores real data while true).")]
        public bool UseSamplesAtRuntime { get => _useSamplesAtRuntime; set { if (_useSamplesAtRuntime == value) return; _useSamplesAtRuntime = value; InvalidateAll(); } }

        [Category("Samples")]
        [Description("Select 'Defense' (no cap) or 'Resistance' (cap at 90%).")]
        public SamplePreset Samples { get => _samplePreset; set { if (_samplePreset == value) return; _samplePreset = value; InvalidateAll(); } }

        [Category("Samples")]
        [Description("How many sample rows to show (top-N for the chosen preset).")]
        public int SampleCount { get => _sampleCount; set { value = Math.Max(1, value); if (_sampleCount == value) return; _sampleCount = value; InvalidateAll(); } }

        // Secondary label controls
        [Category("Labels")]
        [Description("What to render as the right-side label in the text column.")]
        public SecondaryLabelKind SecondaryLabel { get => _secondaryKind; set { if (_secondaryKind == value) return; _secondaryKind = value; InvalidateAll(); } }

        [Category("Labels")]
        [Description("Decimals for numeric secondary labels (maximum).")]
        public int SecondaryDecimals { get => _secondaryDecimals; set { value = Math.Max(0, value); if (_secondaryDecimals == value) return; _secondaryDecimals = value; InvalidateAll(); } }

        [Category("Labels")]
        [Description("Text appended after numeric secondary labels (e.g., %).")]
        public string SecondarySuffix { get => _secondarySuffix; set { _secondarySuffix = value ?? ""; InvalidateAll(); } }

        [Category("Labels")]
        [Description("Right-align secondary labels inside the text column.")]
        public bool SecondaryRightAlign { get => _secondaryRightAlign; set { if (_secondaryRightAlign == value) return; _secondaryRightAlign = value; InvalidateAll(); } }

        [Category("Labels")]
        [Description("Gap (in pixels) between item name and inline value (used when SecondaryRightAlign=false).")]
        public int LabelValuePadding { get => _labelValuePadding; set { value = Math.Max(0, value); if (_labelValuePadding == value) return; _labelValuePadding = value; InvalidateAll(); } }

        [Category("Labels")]
        [Description("Primary item label alignment within the text column.")]
        public Alignment LabelAlignment { get => _labelAlignment; set { if (_labelAlignment == value) return; _labelAlignment = value; InvalidateAll(); } }

        [Category("Labels")]
        [Description("Minimum gap, in space characters, enforced between the name and the right-aligned value.")]
        public int LabelValueMinGapSpaces { get => _labelValueMinGapSpaces; set { value = Math.Max(0, value); if (_labelValueMinGapSpaces == value) return; _labelValueMinGapSpaces = value; InvalidateAll(); } }

        [Category("Layout")]
        [Description("When true, the text column width is computed from the longest Name + gap + Value, so nothing overlaps.")]
        public bool AutoTextWidth { get => _autoTextWidth; set { if (_autoTextWidth == value) return; _autoTextWidth = value; InvalidateAll(); } }

        [Category("Layout")][Description("Minimum auto-computed text width.")] public int AutoTextWidthMin { get => _autoTextWidthMin; set { _autoTextWidthMin = Math.Max(0, value); InvalidateAll(); } }
        [Category("Layout")][Description("Maximum auto-computed text width.")] public int AutoTextWidthMax { get => _autoTextWidthMax; set { _autoTextWidthMax = Math.Max(_autoTextWidthMin, value); InvalidateAll(); } }

        // Lists exposed (mutate then InvalidateAll)
        [Browsable(false)] public int ItemCount => _items.Count;
        [Browsable(false)] public int ContentHeight => CalcContentHeight();
        [Browsable(false)] public List<float> PerItemScales => _perItemScales;
        [Browsable(false)] public List<Color> BaseBarColors => _baseBarColors;
        [Browsable(false)] public List<Color> EnhBarColors => _enhBarColors;
        [Browsable(false)] public List<Color> OvercapColors => _overcapColors;

        // Legacy "Max" setter
        [Browsable(false)] public float Max { get => _scaleValue; set { SetBestScale(value); } }

        #endregion

        #region Item model + data operations

        public sealed class Item
        {
            public string Name { get; }
            public string Name2 { get; }
            public float ValueBase { get; }
            public float ValueEnh { get; }
            public float ValueOvercap { get; }
            public float ValueAbsorbed { get; }
            public string Tip { get; }

            public Item(string name, float valueBase, float valueEnh, string tip = "")
            { Name = name; Name2 = ""; ValueBase = valueBase; ValueEnh = valueEnh; ValueOvercap = valueEnh; ValueAbsorbed = 0; Tip = tip; }

            public Item(string name, string name2, float valueBase, float valueEnh, float valueOvercap = 0, float valueAbsorbed = 0, string tip = "")
            { Name = name; Name2 = name2; ValueBase = valueBase; ValueEnh = valueEnh; ValueOvercap = valueOvercap; ValueAbsorbed = valueAbsorbed; Tip = tip; }
        }

        public void Clear() { _items.Clear(); InvalidateAll(); }
        public void AddItem(string sName, float nBase, float nEnh, string iTip = "") { _items.Add(new Item(sName, nBase, nEnh, iTip)); InvalidateAll(); }
        public void AddItem(string sName, float nBase, float nEnh, float nOvercap, string iTip = "") { _items.Add(new Item(sName, sName, nBase, nEnh, nOvercap, 0, iTip)); InvalidateAll(); }
        public void AddItem(string sName, float nBase, float nEnh, float nOvercap, float nAbsorbed, string iTip = "") { _items.Add(new Item(sName, sName, nBase, nEnh, nOvercap, nAbsorbed, iTip)); InvalidateAll(); }
        public void AddItemPair(string sName, string sName2, float nBase, float nEnh, string iTip = "") { _items.Add(new Item(sName, sName2, nBase, nEnh, nEnh, 0, iTip)); InvalidateAll(); }
        public void AddItemPair(string sName, string sName2, float nBase, float nEnh, float nOvercap, string iTip = "") { _items.Add(new Item(sName, sName2, nBase, nEnh, nOvercap, 0, iTip)); InvalidateAll(); }
        public void AddItemPair(string sName, string sName2, float nBase, float nEnh, float nOvercap, float nAbsorbed, string iTip = "") { _items.Add(new Item(sName, sName2, nBase, nEnh, nOvercap, nAbsorbed, iTip)); InvalidateAll(); }

        public void BeginUpdate() => _noDraw = true;
        public void EndUpdate() { _noDraw = false; InvalidateAll(); }

        #endregion

        #region ctor / lifecycle

        public MultiGraph()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            FillScales();

            BackColorChanged += (_, __) => InvalidateAll();
            ForeColorChanged += (_, __) => InvalidateAll();
            SizeChanged += (_, __) => InvalidateAll();
            FontChanged += (_, __) => InvalidateAll();

            MouseMove += Wrapper_MouseMove;
            MouseLeave += Wrapper_MouseLeave;
            MouseDown += Wrapper_MouseDown;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (!IsInDesignMode(this))
            {
                _sk = new SKControl { Dock = DockStyle.Fill, BackColor = BackColor };
                _sk.PaintSurface += Sk_PaintSurface;
                _sk.MouseMove += Sk_MouseMove;
                _sk.MouseLeave += Sk_MouseLeave;
                _sk.MouseDown += Sk_MouseDown;
                Controls.Add(_sk);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_sk != null)
                {
                    _sk.PaintSurface -= Sk_PaintSurface;
                    _sk.MouseMove -= Sk_MouseMove;
                    _sk.MouseLeave -= Sk_MouseLeave;
                    _sk.MouseDown -= Sk_MouseDown;
                    _sk.Dispose(); _sk = null;
                }
                _tip.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Design-time GDI+ preview

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!IsInDesignMode(this) || _noDraw) return;

            var g = e.Graphics;
            g.Clear(BackColor);

            using (var lg = new System.Drawing.Drawing2D.LinearGradientBrush(
                       new Rectangle(0, 0, Math.Max(1, Width), 1), _fadeStart, _fadeEnd, 0f))
                g.FillRectangle(lg, ClientRectangle);

            int leftCol = GetEffectiveTextWidth_GDI(g, ClientRectangle, EffectiveItems(true));

            var (drawRect, scaleBand, topRuler, bottomRuler) = ComputeRects(ClientRectangle, leftCol);

            if (_showScale) DrawScaleBand_GDI(g, scaleBand);
            if (_drawRuler)
            {
                if (_rulerPos != RulerPosition.Bottom) DrawRuler_GDI(g, topRuler, true);
                if (_rulerPos != RulerPosition.Top) DrawRuler_GDI(g, bottomRuler, false);
            }

            if (_highlight && _hoverRow >= 0)
            {
                var rowRect = RowRect(drawRect, _hoverRow);
                using var hl = new SolidBrush(Color.FromArgb(128, _colorHighlight));
                g.FillRectangle(hl, new Rectangle(0, rowRect.Top, Width, rowRect.Height));
            }

            DrawBars_GDI(g, drawRect, leftCol);

            if (_border) using (var pen = new Pen(_colorLines)) g.DrawRectangle(pen, drawRect);
            if (_outerBorder) using (var pen = new Pen(_borderColor)) g.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
        }

        #endregion

        #region Runtime Skia renderer

        private void Sk_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_noDraw) return;

            var canvas = e.Surface.Canvas;
            var info = e.Info;
            var client = new Rectangle(0, 0, info.Width, info.Height);

            using (var shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(info.Width, 0),
                       new[] { ToSK(_fadeStart), ToSK(_fadeEnd) }, new float[] { 0f, 1f }, SKShaderTileMode.Clamp))
            using (var bg = new SKPaint { Shader = shader, Style = SKPaintStyle.Fill })
                canvas.DrawRect(new SKRect(0, 0, info.Width, info.Height), bg);

            using var pText = new SKPaint { Color = ToSK(ForeColor), IsAntialias = true, TextSize = CurrentFontPx };
            using var pTextSec = new SKPaint { Color = ToSK(_secondaryLabelColor.IsEmpty ? ForeColor : _secondaryLabelColor), IsAntialias = true, TextSize = CurrentFontPx };

            int leftCol = GetEffectiveTextWidth_SK(pText, pTextSec, client, EffectiveItems(false));

            var (drawRect, scaleBand, topRuler, bottomRuler) = ComputeRects(client, leftCol);
            if (_showScale) DrawScaleBand_SK(canvas, scaleBand);
            if (_drawRuler)
            {
                if (_rulerPos != RulerPosition.Bottom) DrawRuler_SK(canvas, topRuler, true);
                if (_rulerPos != RulerPosition.Top) DrawRuler_SK(canvas, bottomRuler, false);
            }

            if (_highlight && _hoverRow >= 0)
            {
                var row = RowRect(drawRect, _hoverRow);
                using var hl = new SKPaint { Color = new SKColor(_colorHighlight.R, _colorHighlight.G, _colorHighlight.B, 128), Style = SKPaintStyle.Fill };
                canvas.DrawRect(new SKRect(0, row.Top, info.Width, row.Bottom), hl);
            }

            DrawBars_SK(canvas, drawRect, client, pText, pTextSec, leftCol);

            if (_border)
            {
                using var pen = new SKPaint { Color = ToSK(_colorLines), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true };
                canvas.DrawRect(ToSKRect(drawRect), pen);
            }
            if (_outerBorder)
            {
                using var pen = new SKPaint { Color = ToSK(_borderColor), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true };
                canvas.DrawRect(new SKRect(0, 0, info.Width - 1, info.Height - 1), pen);
            }
        }

        #endregion

        #region Shared geometry / helpers

        private (Rectangle drawRect, Rectangle scaleBand, Rectangle topRuler, Rectangle bottomRuler) ComputeRects(Rectangle client, int textWidth)
        {
            int left = Math.Min(client.Right, Math.Max(0, textWidth));
            int top = client.Top;
            int bottom = client.Bottom;

            Rectangle scaleBand = Rectangle.Empty;
            if (_showScale)
            {
                int h = Math.Max(0, _scaleHeight);
                scaleBand = Rectangle.FromLTRB(left, top, client.Right, Math.Min(client.Bottom, top + h));
                top += h;
            }

            Rectangle topRuler = Rectangle.Empty, bottomRuler = Rectangle.Empty;
            if (_drawRuler)
            {
                if (_rulerPos != RulerPosition.Bottom) { topRuler = Rectangle.FromLTRB(left, top, client.Right, Math.Min(client.Bottom, top + RULER_H)); top += RULER_H; }
                if (_rulerPos != RulerPosition.Top) { bottomRuler = Rectangle.FromLTRB(left, Math.Max(client.Top, bottom - RULER_H), client.Right, bottom); bottom -= RULER_H; }
            }

            var drawRect = Rectangle.FromLTRB(left, top, client.Right, bottom);
            return (drawRect, scaleBand, topRuler, bottomRuler);
        }

        private Rectangle RowRect(Rectangle drawRect, int rowIndex)
        {
            int rowH = _itemHeight + _rowGap;
            int y = drawRect.Top + rowIndex * rowH;
            return new Rectangle(0, y - _rowGap / 2, Width, _itemHeight + _rowGap);
        }

        private float CurrentFontPx => _fontPxOverride > 0 ? _fontPxOverride : Math.Max(10f, _itemHeight);
        private static SKColor ToSK(Color c) => new SKColor(c.R, c.G, c.B, c.A);
        private static SKRect ToSKRect(Rectangle r) => new SKRect(r.Left, r.Top, r.Right, r.Bottom);
        private static bool BaseLineOffset(string text) => string.IsNullOrEmpty(text) ? false : Regex.IsMatch(text, @"[ypqgjQY]");
        private float EffectiveScaleFor(int i) => _perItemScales.Count == _items.Count && _items.Count > 0 ? Math.Max(1f, _perItemScales[i]) : Math.Max(1f, _scaleValue);

        // Smart % formatting
        private string FormatSecondaryNumeric(float v)
        {
            bool isPercent = _secondarySuffix.Trim() == "%";
            if (isPercent)
            {
                float rounded = (float)Math.Round(v);
                if (Math.Abs(v - rounded) < 0.0005f)
                    return rounded.ToString("0", CultureInfo.InvariantCulture) + _secondarySuffix;
            }
            string pattern = _secondaryDecimals <= 0 ? "0" : "0." + new string('#', _secondaryDecimals);
            string s = v.ToString(pattern, CultureInfo.InvariantCulture);
            return s + _secondarySuffix;
        }

        private string GetSecondaryText(Item it) =>
            _secondaryKind switch
            {
                SecondaryLabelKind.None => string.Empty,
                SecondaryLabelKind.Name2 => it.Name2 ?? string.Empty,
                SecondaryLabelKind.ValueBase => FormatSecondaryNumeric(it.ValueBase),
                SecondaryLabelKind.ValueEnh => FormatSecondaryNumeric(it.ValueEnh),
                SecondaryLabelKind.ValueOvercap => FormatSecondaryNumeric(it.ValueOvercap),
                SecondaryLabelKind.ValueAbsorbed => FormatSecondaryNumeric(it.ValueAbsorbed),
                _ => string.Empty
            };

        #endregion

        #region Auto text width measurement

        private int GetEffectiveTextWidth_GDI(Graphics g, Rectangle client, List<Item> items)
        {
            if (!_autoTextWidth) return _textWidth;

            if (items.Count == 0) return Math.Min(_autoTextWidthMax, Math.Max(_autoTextWidthMin, _textWidth));

            float spaceW = g.MeasureString(" ", Font).Width;
            float gap = _secondaryRightAlign ? _labelValueMinGapSpaces * spaceW : _labelValuePadding;
            float leftPad = 3f, rightPad = 2f;

            float maxValue = 0f, maxNeeded = 0f, maxName = 0f;

            foreach (var it in items)
            {
                string name = it.Name ?? string.Empty;
                string sec = GetSecondaryText(it);
                float nameW = g.MeasureString(name, Font).Width;
                float secW = string.IsNullOrEmpty(sec) ? 0f : g.MeasureString(sec, Font).Width;

                maxName = Math.Max(maxName, nameW);
                maxValue = Math.Max(maxValue, secW);

                float needed = _secondaryRightAlign
                    ? leftPad + nameW + gap + secW + rightPad
                    : leftPad + nameW + gap + secW + rightPad;

                maxNeeded = Math.Max(maxNeeded, needed);
            }

            // Also ensure room for the widest value when right-aligned and names are short.
            float minForRightAlignedValues = _secondaryRightAlign
                ? leftPad + maxName + _labelValueMinGapSpaces * spaceW + maxValue + rightPad
                : maxNeeded;

            int width = (int)Math.Ceiling(Math.Max(maxNeeded, minForRightAlignedValues));
            width = Math.Max(_autoTextWidthMin, Math.Min(_autoTextWidthMax, width));
            width = Math.Min(width, client.Width - 10); // keep some room for bars
            return Math.Max(0, width);
        }

        private int GetEffectiveTextWidth_SK(SKPaint pText, SKPaint pTextSec, Rectangle client, List<Item> items)
        {
            if (!_autoTextWidth) return _textWidth;

            if (items.Count == 0) return Math.Min(_autoTextWidthMax, Math.Max(_autoTextWidthMin, _textWidth));

            var rSpace = new SKRect(); pText.MeasureText(" ", ref rSpace);
            float spaceW = rSpace.Width;
            float gap = _secondaryRightAlign ? _labelValueMinGapSpaces * spaceW : _labelValuePadding;
            float leftPad = 3f, rightPad = 2f;

            float maxValue = 0f, maxNeeded = 0f, maxName = 0f;

            foreach (var it in items)
            {
                string name = it.Name ?? string.Empty;
                string sec = GetSecondaryText(it);

                var rn = new SKRect(); pText.MeasureText(name, ref rn);
                var rs = new SKRect(); if (!string.IsNullOrEmpty(sec)) pTextSec.MeasureText(sec, ref rs);

                maxName = Math.Max(maxName, rn.Width);
                maxValue = Math.Max(maxValue, rs.Width);

                float needed = _secondaryRightAlign
                    ? leftPad + rn.Width + gap + rs.Width + rightPad
                    : leftPad + rn.Width + gap + rs.Width + rightPad;

                maxNeeded = Math.Max(maxNeeded, needed);
            }

            float minForRightAlignedValues = _secondaryRightAlign
                ? leftPad + maxName + _labelValueMinGapSpaces * spaceW + maxValue + rightPad
                : maxNeeded;

            int width = (int)Math.Ceiling(Math.Max(maxNeeded, minForRightAlignedValues));
            width = Math.Max(_autoTextWidthMin, Math.Min(_autoTextWidthMax, width));
            width = Math.Min(width, client.Width - 10);
            return Math.Max(0, width);
        }

        #endregion

        #region GDI+ drawing

        private void DrawScaleBand_GDI(Graphics g, Rectangle band)
        {
            if (band.Width <= 0 || band.Height <= 0) return;
            using var fill = new SolidBrush(BackColor);
            using var pen = new Pen(ForeColor, 1);
            using var text = new SolidBrush(ForeColor);
            g.FillRectangle(fill, band);
            g.DrawLine(pen, band.Left, band.Bottom - 1, band.Right, band.Bottom - 1);

            const int stops = 10;
            for (int i = 0; i <= stops; i++)
            {
                int x = band.Left + i * band.Width / stops;
                g.DrawLine(pen, x, band.Bottom - 1, x, band.Bottom - 6);

                float v = _scaleValue * i / stops;
                string s = i == 0 ? "0" : v.ToString(v >= 10 ? "0" : v >= 5 ? "0.0" : "0.00", CultureInfo.InvariantCulture);
                var sz = g.MeasureString(s, Font);
                float tx = i < stops ? x - sz.Width / 2f : x - sz.Width - 1;
                g.DrawString(s, Font, text, tx, band.Bottom - sz.Height - 2);
            }
        }

        private void DrawRuler_GDI(Graphics g, Rectangle ruler, bool top)
        {
            if (ruler.Width <= 0 || ruler.Height <= 0) return;
            using var pen = new Pen(_rulerColor, 1);
            using var text = new SolidBrush(_rulerColor);

            int y = top ? ruler.Top + 1 : ruler.Bottom - 1;
            g.DrawLine(pen, ruler.Left, y, ruler.Right, y);

            const int stops = 10;
            for (int i = 0; i <= stops; i++)
            {
                int x = ruler.Left + i * ruler.Width / stops;
                if (top) g.DrawLine(pen, x, y, x, y + 4); else g.DrawLine(pen, x, y, x, y - 4);

                float v = _scaleValue * i / stops;
                string s = i == 0 ? "0" : v.ToString(v >= 10 ? "0" : v >= 5 ? "0.0" : "0.00", CultureInfo.InvariantCulture);
                var sz = g.MeasureString(s, Font);
                float tx = i < stops ? x - sz.Width / 2f : x - sz.Width - 1;
                float ty = top ? y + 2 : y - sz.Height - 2;
                g.DrawString(s, Font, text, tx, ty);
            }
        }

        private void DrawBars_GDI(Graphics g, Rectangle drawRect, int leftColWidth)
        {
            var items = EffectiveItems(IsInDesignMode(this));
            if (items.Count == 0) return;

            using var bBase = new SolidBrush(_colorBase);
            using var bEnh = new SolidBrush(_colorEnh);
            using var bOver = new SolidBrush(_colorOvercap);
            using var bAbs = new SolidBrush(_colorAbsorbed);
            using var pen = new Pen(_colorLines, 1);
            using var textPrimary = new SolidBrush(ForeColor);
            using var textSecondary = new SolidBrush(_secondaryLabelColor.IsEmpty ? ForeColor : _secondaryLabelColor);

            float spaceW = g.MeasureString(" ", Font).Width;
            float gap = _secondaryRightAlign ? _labelValueMinGapSpaces * spaceW : _labelValuePadding;
            float leftPad = 3f, rightPad = 2f;

            var rowH = _itemHeight + _rowGap;
            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                int y = drawRect.Top + i * rowH;

                string name = it.Name ?? "";
                string sec = GetSecondaryText(it);

                var nameSize = g.MeasureString(name, Font);
                var secSize = string.IsNullOrEmpty(sec) ? SizeF.Empty : g.MeasureString(sec, Font);

                // label column rect
                var labelRect = new Rectangle(0, y, Math.Min(leftColWidth, Width), _itemHeight);

                // Secondary X (right aligned by default)
                float secX = _secondaryRightAlign
                    ? labelRect.Right - rightPad - secSize.Width
                    : leftPad + nameSize.Width + gap;

                // Name X (alignment) with enforced gap when right-aligned secondary
                float nameRightLimit = _secondaryRightAlign ? secX - gap : labelRect.Left + leftPad + nameSize.Width;
                float nameX;
                if (_labelAlignment == Alignment.Right && _secondaryRightAlign)
                    nameX = Math.Max(leftPad, nameRightLimit - nameSize.Width);
                else if (_labelAlignment == Alignment.Right && !_secondaryRightAlign)
                    nameX = Math.Max(leftPad, labelRect.Right - rightPad - nameSize.Width - secSize.Width - gap);
                else
                    nameX = labelRect.Left + leftPad;

                float baselineOffset = BaseLineOffset(name) ? Font.Size / 4f : 0f;
                g.DrawString(name, Font, textPrimary, nameX, y - baselineOffset);
                if (!string.IsNullOrEmpty(sec))
                    g.DrawString(sec, Font, textSecondary, secX, y - baselineOffset);

                // bar widths
                float scale = Math.Max(1f, EffectiveScaleFor(i));
                int wBase = (int)Math.Round(drawRect.Width * (it.ValueBase / scale));
                int wEnh = (int)Math.Round(drawRect.Width * (it.ValueEnh / scale));
                int wOver = (int)Math.Round(drawRect.Width * (it.ValueOvercap / scale));
                int wAbs = (int)Math.Round(drawRect.Width * (it.ValueAbsorbed / scale));

                Color cBase = _differentiate && _baseBarColors.Count > 0 ? _baseBarColors[i % _baseBarColors.Count] : _colorBase;
                Color cEnh = _differentiate && _enhBarColors.Count > 0 ? _enhBarColors[i % _enhBarColors.Count] : _colorEnh;
                Color cOver = _differentiate && _overcapColors.Count > 0 ? _overcapColors[i % _overcapColors.Count] : _colorOvercap;

                using var bBaseDyn = new SolidBrush(cBase);
                using var bEnhDyn = new SolidBrush(cEnh);
                using var bOverDyn = new SolidBrush(cOver);

                if (_style == GraphStyle.Twin)
                {
                    int half = (int)Math.Round(_itemHeight / 2.0);
                    var rBase = new Rectangle(drawRect.Left, y, Math.Max(0, wBase), half);
                    var rEnh = new Rectangle(drawRect.Left, y + half, Math.Max(0, wEnh), half);

                    if (_overcap && it.ValueOvercap > 0) g.FillRectangle(bOverDyn, new Rectangle(drawRect.Left, y, Math.Max(0, wOver), half));
                    g.FillRectangle(bBaseDyn, rBase);
                    g.FillRectangle(bEnhDyn, rEnh);
                    if (it.ValueAbsorbed > 0) g.FillRectangle(bAbs, new Rectangle(drawRect.Left, y + half, Math.Max(0, wAbs), half));
                    if (_lines) { g.DrawRectangle(pen, rBase); g.DrawRectangle(pen, rEnh); }
                }
                else if (_style == GraphStyle.EnhOnly)
                {
                    var rEnh = new Rectangle(drawRect.Left, y, Math.Max(0, wEnh), _itemHeight);
                    g.FillRectangle(bEnhDyn, rEnh);
                    if (_lines) g.DrawRectangle(pen, rEnh);
                }
                else if (_style == GraphStyle.BaseOnly)
                {
                    var rBase = new Rectangle(drawRect.Left, y, Math.Max(0, wBase), _itemHeight);
                    g.FillRectangle(bBaseDyn, rBase);
                    if (_lines) g.DrawRectangle(pen, rBase);
                }
                else // Overlay
                {
                    if (_overcap && it.ValueOvercap > 0)
                        g.FillRectangle(bOverDyn, new Rectangle(drawRect.Left, y, Math.Max(0, wOver), _itemHeight));

                    bool baseFirst = wBase >= wEnh;
                    var rBig = new Rectangle(drawRect.Left, y, Math.Max(0, baseFirst ? wBase : wEnh), _itemHeight);
                    var rSmall = new Rectangle(drawRect.Left, y, Math.Max(0, baseFirst ? wEnh : wBase), _itemHeight);
                    var bigB = baseFirst ? bBaseDyn : bEnhDyn;
                    var smallB = baseFirst ? bEnhDyn : bBaseDyn;

                    g.FillRectangle(bigB, rBig);
                    g.FillRectangle(smallB, rSmall);

                    if (_lines) { g.DrawRectangle(pen, rBig); g.DrawRectangle(pen, rSmall); }
                    if (it.ValueAbsorbed > 0) g.FillRectangle(bAbs, new Rectangle(drawRect.Left, y, Math.Max(0, wAbs), _itemHeight));
                }

                // Marker
                if (_markerValue > 0f)
                {
                    int mx = drawRect.Left + (int)Math.Round(drawRect.Width * (_markerValue / scale));
                    using var m2 = new Pen(_colorMarkerOuter, 3);
                    using var m1 = new Pen(_colorMarkerInner, 1);
                    g.DrawLine(m2, mx, y, mx, y + _itemHeight);
                    g.DrawLine(m1, mx, y, mx, y + _itemHeight);
                }
            }
        }

        #endregion

        #region Skia drawing

        private void DrawScaleBand_SK(SKCanvas canvas, Rectangle band)
        {
            if (band.Width <= 0 || band.Height <= 0) return;
            using var fill = new SKPaint { Color = ToSK(BackColor), Style = SKPaintStyle.Fill };
            using var pen = new SKPaint { Color = ToSK(ForeColor), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true };
            using var text = new SKPaint { Color = ToSK(ForeColor), IsAntialias = true, TextSize = CurrentFontPx };
            canvas.DrawRect(ToSKRect(band), fill);
            canvas.DrawLine(band.Left, band.Bottom - 1, band.Right, band.Bottom - 1, pen);

            const int stops = 10;
            for (int i = 0; i <= stops; i++)
            {
                float x = band.Left + i * band.Width / (float)stops;
                canvas.DrawLine(x, band.Bottom - 1, x, band.Bottom - 6, pen);
                float v = _scaleValue * i / stops;
                string s = i == 0 ? "0" : v.ToString(v >= 10 ? "0" : v >= 5 ? "0.0" : "0.00", CultureInfo.InvariantCulture);
                var bounds = new SKRect(); text.MeasureText(s, ref bounds);
                float tx = i < stops ? x - bounds.Width / 2f : x - bounds.Width - 1;
                canvas.DrawText(s, tx, band.Bottom - 3, text);
            }
        }

        private void DrawRuler_SK(SKCanvas canvas, Rectangle ruler, bool top)
        {
            if (ruler.Width <= 0 || ruler.Height <= 0) return;
            using var pen = new SKPaint { Color = ToSK(_rulerColor), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true };
            using var text = new SKPaint { Color = ToSK(_rulerColor), IsAntialias = true, TextSize = Math.Max(10f, CurrentFontPx - 2f) };

            float y = top ? ruler.Top + 1 : ruler.Bottom - 1;
            canvas.DrawLine(ruler.Left, y, ruler.Right, y, pen);

            const int stops = 10;
            for (int i = 0; i <= stops; i++)
            {
                float x = ruler.Left + i * ruler.Width / (float)stops;
                if (top) canvas.DrawLine(x, y, x, y + 4, pen); else canvas.DrawLine(x, y, x, y - 4, pen);

                float v = _scaleValue * i / stops;
                string s = i == 0 ? "0" : v.ToString(v >= 10 ? "0" : v >= 5 ? "0.0" : "0.00", CultureInfo.InvariantCulture);
                var bounds = new SKRect(); text.MeasureText(s, ref bounds);
                float tx = i < stops ? x - bounds.Width / 2f : x - bounds.Width - 1;
                float ty = top ? y + 12 : y - 4;
                canvas.DrawText(s, tx, ty, text);
            }
        }

        private void DrawBars_SK(SKCanvas canvas, Rectangle drawRect, Rectangle client, SKPaint pText, SKPaint pTextSec, int leftColWidth)
        {
            var items = EffectiveItems(false);
            if (items.Count == 0) return;

            using var pBase = new SKPaint { Color = ToSK(_colorBase), Style = SKPaintStyle.Fill, IsAntialias = true };
            using var pEnh = new SKPaint { Color = ToSK(_colorEnh), Style = SKPaintStyle.Fill, IsAntialias = true };
            using var pOver = new SKPaint { Color = ToSK(_colorOvercap), Style = SKPaintStyle.Fill, IsAntialias = true };
            using var pAbs = new SKPaint { Color = ToSK(_colorAbsorbed), Style = SKPaintStyle.Fill, IsAntialias = true };
            using var pLine = new SKPaint { Color = ToSK(_colorLines), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true };

            var spaceRect = new SKRect(); pText.MeasureText(" ", ref spaceRect);
            float spaceW = spaceRect.Width;
            float gap = _secondaryRightAlign ? _labelValueMinGapSpaces * spaceW : _labelValuePadding;
            float leftPad = 3f, rightPad = 2f;

            var rowH = _itemHeight + _rowGap;
            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                float y = drawRect.Top + i * rowH;

                string name = it.Name ?? "";
                string sec = GetSecondaryText(it);

                var rn = new SKRect(); pText.MeasureText(name, ref rn);
                var rs = new SKRect(); if (!string.IsNullOrEmpty(sec)) pTextSec.MeasureText(sec, ref rs);

                var labelRect = new Rectangle(0, (int)y, Math.Min(leftColWidth, client.Width), _itemHeight);

                // secondary X
                float secX = _secondaryRightAlign
                    ? labelRect.Right - rightPad - rs.Width
                    : leftPad + rn.Width + gap;

                // name X
                float nameRightLimit = _secondaryRightAlign ? secX - gap : labelRect.Left + leftPad + rn.Width;
                float nameX;
                if (_labelAlignment == Alignment.Right && _secondaryRightAlign)
                    nameX = Math.Max(leftPad, nameRightLimit - rn.Width);
                else if (_labelAlignment == Alignment.Right && !_secondaryRightAlign)
                    nameX = Math.Max(leftPad, labelRect.Right - rightPad - rn.Width - rs.Width - gap);
                else
                    nameX = labelRect.Left + leftPad;

                float baselineOffset = BaseLineOffset(name) ? CurrentFontPx / 4f : 0f;
                canvas.DrawText(name, nameX, y + _itemHeight - baselineOffset, pText);
                if (!string.IsNullOrEmpty(sec))
                    canvas.DrawText(sec, secX, y + _itemHeight - baselineOffset, pTextSec);

                // bar widths
                float scale = Math.Max(1f, EffectiveScaleFor(i));
                float wBase = drawRect.Width * (it.ValueBase / scale);
                float wEnh = drawRect.Width * (it.ValueEnh / scale);
                float wOver = drawRect.Width * (it.ValueOvercap / scale);
                float wAbs = drawRect.Width * (it.ValueAbsorbed / scale);

                var cBase = _differentiate && _baseBarColors.Count > 0 ? ToSK(_baseBarColors[i % _baseBarColors.Count]) : pBase.Color;
                var cEnh = _differentiate && _enhBarColors.Count > 0 ? ToSK(_enhBarColors[i % _enhBarColors.Count]) : pEnh.Color;
                var cOver = _differentiate && _overcapColors.Count > 0 ? ToSK(_overcapColors[i % _overcapColors.Count]) : pOver.Color;

                if (_style == GraphStyle.Twin)
                {
                    float half = _itemHeight / 2f;
                    var rBase = new SKRect(drawRect.Left, y, drawRect.Left + Math.Max(0, wBase), y + half);
                    var rEnh = new SKRect(drawRect.Left, y + half, drawRect.Left + Math.Max(0, wEnh), y + _itemHeight);

                    if (_overcap && it.ValueOvercap > 0)
                        canvas.DrawRect(new SKRect(drawRect.Left, y, drawRect.Left + Math.Max(0, wOver), y + half), new SKPaint { Color = cOver, Style = SKPaintStyle.Fill, IsAntialias = true });

                    canvas.DrawRect(rBase, new SKPaint { Color = cBase, Style = SKPaintStyle.Fill, IsAntialias = true });
                    canvas.DrawRect(rEnh, new SKPaint { Color = cEnh, Style = SKPaintStyle.Fill, IsAntialias = true });
                    if (it.ValueAbsorbed > 0)
                        canvas.DrawRect(new SKRect(drawRect.Left, y + half, drawRect.Left + Math.Max(0, wAbs), y + _itemHeight), pAbs);

                    if (_lines) { canvas.DrawRect(rBase, pLine); canvas.DrawRect(rEnh, pLine); }
                }
                else if (_style == GraphStyle.EnhOnly)
                {
                    var rEnh = new SKRect(drawRect.Left, y, drawRect.Left + Math.Max(0, wEnh), y + _itemHeight);
                    canvas.DrawRect(rEnh, new SKPaint { Color = cEnh, Style = SKPaintStyle.Fill, IsAntialias = true });
                    if (_lines) canvas.DrawRect(rEnh, pLine);
                }
                else if (_style == GraphStyle.BaseOnly)
                {
                    var rBase = new SKRect(drawRect.Left, y, drawRect.Left + Math.Max(0, wBase), y + _itemHeight);
                    canvas.DrawRect(rBase, new SKPaint { Color = cBase, Style = SKPaintStyle.Fill, IsAntialias = true });
                    if (_lines) canvas.DrawRect(rBase, pLine);
                }
                else // Overlay
                {
                    if (_overcap && it.ValueOvercap > 0)
                        canvas.DrawRect(new SKRect(drawRect.Left, y, drawRect.Left + Math.Max(0, wOver), y + _itemHeight), new SKPaint { Color = cOver, Style = SKPaintStyle.Fill, IsAntialias = true });

                    bool baseFirst = wBase >= wEnh;
                    var rBig = new SKRect(drawRect.Left, y, drawRect.Left + Math.Max(0, baseFirst ? wBase : wEnh), y + _itemHeight);
                    var rSmall = new SKRect(drawRect.Left, y, drawRect.Left + Math.Max(0, baseFirst ? wEnh : wBase), y + _itemHeight);
                    var bigPaint = new SKPaint { Color = baseFirst ? cBase : cEnh, Style = SKPaintStyle.Fill, IsAntialias = true };
                    var smallPaint = new SKPaint { Color = baseFirst ? cEnh : cBase, Style = SKPaintStyle.Fill, IsAntialias = true };

                    canvas.DrawRect(rBig, bigPaint);
                    canvas.DrawRect(rSmall, smallPaint);

                    if (_lines) { canvas.DrawRect(rBig, pLine); canvas.DrawRect(rSmall, pLine); }
                    if (it.ValueAbsorbed > 0)
                        canvas.DrawRect(new SKRect(drawRect.Left, y, drawRect.Left + Math.Max(0, wAbs), y + _itemHeight), pAbs);
                }

                // Marker
                if (_markerValue > 0f)
                {
                    float mx = drawRect.Left + drawRect.Width * (_markerValue / scale);
                    using var m2 = new SKPaint { Color = ToSK(_colorMarkerOuter), Style = SKPaintStyle.Stroke, StrokeWidth = 3, IsAntialias = true };
                    using var m1 = new SKPaint { Color = ToSK(_colorMarkerInner), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true };
                    canvas.DrawLine(mx, y, mx, y + _itemHeight, m2);
                    canvas.DrawLine(mx, y, mx, y + _itemHeight, m1);
                }
            }
        }

        #endregion

        #region Hit-testing / hover / click

        private void Wrapper_MouseMove(object? s, MouseEventArgs e) { if (!IsInDesignMode(this)) return; UpdateHover(e.Location, ClientRectangle); }
        private void Wrapper_MouseLeave(object? s, EventArgs e) { if (!IsInDesignMode(this)) return; ClearHover(); }
        private void Wrapper_MouseDown(object? s, MouseEventArgs e) { if (!IsInDesignMode(this)) return; if (_clickable && e.Button == MouseButtons.Left) BarClick?.Invoke(GetValueAtPoint(e.X, e.Y, ClientRectangle)); }
        private void Sk_MouseMove(object? s, MouseEventArgs e) => UpdateHover(e.Location, _sk?.ClientRectangle ?? Rectangle.Empty);
        private void Sk_MouseLeave(object? s, EventArgs e) => ClearHover();
        private void Sk_MouseDown(object? s, MouseEventArgs e) { if (!_clickable || e.Button != MouseButtons.Left || _sk == null) return; BarClick?.Invoke(GetValueAtPoint(e.X, e.Y, _sk.ClientRectangle)); }

        private void UpdateHover(Point p, Rectangle rect)
        {
            int row = RowAtPoint(p.Y, rect);
            if (row == _hoverRow) return;
            _hoverRow = row;
            if (_hoverRow >= 0) _tip.SetToolTip(this, EffectiveItems(IsInDesignMode(this))[_hoverRow].Tip ?? string.Empty);
            else _tip.SetToolTip(this, "");
            InvalidateAll();
        }

        private void ClearHover()
        {
            if (_hoverRow != -1)
            {
                _hoverRow = -1;
                _tip.SetToolTip(this, "");
                InvalidateAll();
            }
        }

        private float GetValueAtPoint(int x, int y, Rectangle client)
        {
            var (drawRect, _, _, _) = ComputeRects(client, _autoTextWidth ? _textWidth : _textWidth);
            if (drawRect.Width <= 0) return 0f;
            int lx = Math.Max(drawRect.Left, Math.Min(drawRect.Right, x));
            float rel = (lx - drawRect.Left) / (float)drawRect.Width;
            return Math.Max(0f, Math.Min(_scaleValue, _scaleValue * rel));
        }

        private int RowAtPoint(int y, Rectangle client)
        {
            var left = _textWidth; // approximate for hit-test; not critical
            var (drawRect, _, _, _) = ComputeRects(client, left);
            int rowH = _itemHeight + _rowGap;
            if (y < drawRect.Top || y > drawRect.Bottom) return -1;
            int idx = (y - drawRect.Top) / rowH;
            var items = EffectiveItems(IsInDesignMode(this));
            return idx >= 0 && idx < items.Count ? idx : -1;
        }

        #endregion

        #region Scales / samples / utils

        private void FillScales()
        {
            _scales.Clear();
            _scales.AddRange(new float[] { 1, 2, 3, 5, 10, 25, 50, 75, 100, 150, 225, 300, 450, 600, 900, 1200, 2400, 3000, 3600, 4000 });
        }

        public float GetMaxValue()
        {
            if (_items.Count == 0) { ScaleValue = 100; return 100; }
            float m = Math.Max(_items.Max(e => e.ValueBase), _items.Max(e => e.ValueEnh));
            return m;
        }

        private void SetBestScale(float value)
        {
            if (_scales.Count == 0) { ScaleValue = value; return; }
            foreach (var s in _scales) { if (s >= value) { ScaleValue = s; return; } }
            ScaleValue = _scales[^1];
        }

        private int WhichScale(float val)
        {
            for (int i = 0; i < _scales.Count; i++) if (Math.Abs(_scales[i] - val) < float.Epsilon) return i;
            return _scales.Count - 1;
        }

        private List<Item> EffectiveItems(bool designTime)
        {
            if (!designTime && _useSamplesAtRuntime) return BuildSamples();
            if (designTime && _showDesignSamples) return BuildSamples();
            return _items.Count > 0 ? _items : designTime ? BuildSamples() : _empty;
        }

        private List<Item> BuildSamples()
        {
            switch (_samplePreset)
            {
                case SamplePreset.Defense:
                    {
                        string[] names = { "Smashing", "Lethal", "Fire", "Cold", "Energy", "Negative", "Toxic", "Psionic", "Melee", "Ranged", "AoE" };
                        float[] enh = { 52.69f, 52.69f, 56.44f, 56.44f, 42.69f, 42.69f, 42.69f, 42.69f, 74f, 69.87f, 80.49f };
                        float[] bas = { 52.69f, 52.69f, 56.44f, 56.44f, 42.69f, 42.69f, 42.69f, 42.69f, 74f, 69.87f, 80.49f };
                        int n = Math.Max(1, Math.Min(_sampleCount, names.Length));
                        var list = new List<Item>(n);
                        for (int i = 0; i < n; i++)
                            list.Add(new Item(names[i], names[i], bas[i], enh[i], enh[i], 0, $"{names[i]}: {enh[i]:0.00}%"));
                        if (_forcedMax <= 0) SetBestScale(enh.Take(n).Max());
                        return list;
                    }

                case SamplePreset.Resistance:
                    {
                        const float CAP = 90f;
                        string[] names = { "Smashing", "Lethal", "Fire", "Cold", "Energy", "Negative", "Toxic", "Psionic" };
                        float[] baseVals = { 30f, 50f, 80f, 70f, 60f, 75f, 50f, 70f };
                        float[] rawEnh = { 45f, 72f, 112f, 95f, 88f, 90f, 63f, 100f };
                        int n = Math.Max(1, Math.Min(_sampleCount, names.Length));
                        var list = new List<Item>(n);
                        for (int i = 0; i < n; i++)
                        {
                            float eff = Math.Min(rawEnh[i], CAP);
                            float over = rawEnh[i];
                            list.Add(new Item(names[i], names[i], baseVals[i], eff, over, 0,
                                $"{names[i]}: raw {rawEnh[i]:0.00}%, effective {eff:0.00}% (cap {CAP:0}% )"));
                        }
                        if (_forcedMax <= 0) SetBestScale(rawEnh.Take(n).Max());
                        return list;
                    }

                case SamplePreset.None:
                default:
                    {
                        int rows = Math.Max(1, _sampleCount);
                        var rnd = new Random(1337);
                        var list = new List<Item>(rows);
                        for (int i = 0; i < rows; i++)
                        {
                            float b = 30f + (float)rnd.NextDouble() * 40f;
                            float e = b + (float)rnd.NextDouble() * 25f;
                            list.Add(new Item($"Sample {i + 1}", $"Sample {i + 1}", b, e, e, 0, $"Sample {i + 1}: {e:0.00}%"));
                        }
                        if (_forcedMax <= 0) SetBestScale(list.Max(x => x.ValueEnh));
                        return list;
                    }
            }
        }

        private void InvalidateAll() { if (_noDraw) return; Invalidate(); _sk?.Invalidate(); }
        private static bool IsInDesignMode(Control c) => LicenseManager.UsageMode == LicenseUsageMode.Designtime || (c.Site?.DesignMode ?? false);

        private int CalcContentHeight()
        {
            var client = ClientRectangle;
            // approximate; used by designer layout only
            int left = _autoTextWidth ? _textWidth : _textWidth;
            var (drawRect, _, _, _) = ComputeRects(client, left);
            var items = EffectiveItems(IsInDesignMode(this));
            if (items.Count == 0) return 0;
            int rowH = _itemHeight + _rowGap;
            int rows = items.Count;
            return drawRect.Top + rows * rowH + _rowGap;
        }

        #endregion
    }
}

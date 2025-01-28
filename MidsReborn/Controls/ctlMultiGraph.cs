using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mids_Reborn.Controls.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using static Mids_Reborn.Core.Enums;

namespace Mids_Reborn.Controls
{
   [DefaultEvent("BarClick")]
    public class CtlMultiGraph : UserControl
    {
        public delegate void BarClickEventHandler(float value);

        public enum RulerPosition
        {
            Top,
            Bottom,
            Both
        }

        public int ContentHeight => CalcContentHeight();

        private IContainer? Components;
        private bool _dualName;
        private List<GraphItem> _items;
        private bool _loaded;
        private int _nameWidth;
        private bool _noDraw;
        private int _oldMouseX;
        private int _oldMouseY;
        private Color _pBaseColor;
        private Color _pBlendColor1;
        private Color _pBlendColor2;
        private bool _pBorder;
        private bool _pDrawLines;
        private bool _pDrawRuler;
        private RulerPosition _pRulerPosition;
        private Color _pEnhColor;
        private int _pForcedMax;
        private int _pHighlight;
        private Color _pHighlightColor;
        private int _pItemHeight;
        private Color _pLineColor;
        private Color _pMarkerColor;
        private Color _pMarkerColor2;
        private int _pScaleHeight;
        private bool _pShowHighlight;
        private bool _pShowScale;
        private GraphStyle _pStyle;
        private List<float> _scales;
        private int _xPadding;
        private int _yPadding;
        private bool _useDifferentiateColors;
        private bool _displayOuterBorder;
        private int _maxItemsCapacity;
        private float _fontSize;
        private float _fontSizeOverride;
        private bool _singleLineLabels;
        private const float Name2XPadding = 9;

        public CtlMultiGraph()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            MouseLeave += ctlMultiGraph_MouseLeave;
            MouseDown += ctlMultiGraph_MouseDown;
            Load += ctlMultiGraph_Load;
            BackColorChanged += ctlMultiGraph_BackColorChanged;
            SizeChanged += ctlMultiGraph_SizeChanged;
            Paint += ctlMultiGraph_Paint;
            FontChanged += ctlMultiGraph_FontChanged;
            ForeColorChanged += ctlMultiGraph_ForeColorChanged;
            Resize += ctlMultiGraph_Resize;
            MouseMove += ctlMultiGraph_MouseMove;
            _scales = new List<float>();
            _items = new List<GraphItem>();
            _pBaseColor = Color.Blue;
            _pEnhColor = Color.Yellow;
            _pBlendColor1 = Color.Black;
            _pBlendColor2 = Color.Red;
            _pLineColor = Color.Black;
            _pStyle = 0;
            _pDrawLines = false;
            _pDrawRuler = false;
            _pRulerPosition = RulerPosition.Top;
            _pBorder = true;
            ScaleValue = 100f;
            _yPadding = 5;
            _xPadding = 4;
            _oldMouseX = 0;
            _oldMouseY = 0;
            _pItemHeight = 8;
            _nameWidth = 72;
            _loaded = false;
            _pShowScale = false;
            _pScaleHeight = 32;
            _pHighlight = -1;
            _pHighlightColor = Color.FromArgb(128, 128, 255);
            _noDraw = false;
            _dualName = false;
            MarkerValue = 0f;
            _pMarkerColor = Color.Black;
            _pMarkerColor2 = Color.Yellow;
            _pForcedMax = 0;
            Clickable = false;
            _maxItemsCapacity = 60;
            _fontSize = _pItemHeight;
            _fontSizeOverride = 0;
            _singleLineLabels = true;
            
            InitializeComponent();
            FillScales();
        }

        [field: AccessedThroughProperty("tTip")]
        protected virtual ToolTip? Tip
        {
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public bool SingleLineLabels
        {
            get => _singleLineLabels;
            set
            {
                _singleLineLabels = value;
                if (DesignMode) Draw();
            }
        }

        public Color ColorBase
        {
            get => _pBaseColor;
            set
            {
                _pBaseColor = value;
                if (DesignMode) Draw();
            }
        }

        public Color ColorEnh
        {
            get => _pEnhColor;
            set
            {
                _pEnhColor = value;
                if (DesignMode) Draw();
            }
        }

        public Color ColorFadeStart
        {
            get => _pBlendColor1;
            set
            {
                _pBlendColor1 = value;
                if (DesignMode) Draw();
            }
        }

        public Color ColorFadeEnd
        {
            get => _pBlendColor2;
            set
            {
                _pBlendColor2 = value;
                if (DesignMode) Draw();
            }
        }

        public Color ColorLines
        {
            get => _pLineColor;
            set
            {
                _pLineColor = value;
                if (DesignMode) Draw();
            }
        }

        public Color ColorHighlight
        {
            get => _pHighlightColor;
            set
            {
                _pHighlightColor = value;
                if (DesignMode) Draw();
            }
        }

        public Color ColorMarkerInner
        {
            get => _pMarkerColor;
            set
            {
                _pMarkerColor = value;
                if (DesignMode) Draw();
            }
        }

        public float MarkerValue { get; set; }

        public Color ColorMarkerOuter
        {
            get => _pMarkerColor2;
            set
            {
                _pMarkerColor2 = value;
                if (DesignMode) Draw();
            }
        }

        public float Max
        {
            get => ScaleValue;
            set
            {
                SetBestScale(value);
                if (DesignMode) Draw();
            }
        }

        public float PaddingX
        {
            get => _xPadding;
            set
            {
                _xPadding = checked((int) Math.Round(value));
                if (DesignMode) Draw();
            }
        }

        public float PaddingY
        {
            get => _yPadding;
            set
            {
                _yPadding = checked((int) Math.Round(value));
                if (DesignMode) Draw();
            }
        }

        public int TextWidth
        {
            get => _nameWidth;
            set
            {
                _nameWidth = value;
                if (DesignMode) Draw();
            }
        }

        public int ItemHeight
        {
            get => _pItemHeight;
            set
            {
                _pItemHeight = value;
                _fontSize = value;
                if (DesignMode) Draw();
            }
        }

        public bool Lines
        {
            get => _pDrawLines;
            set
            {
                _pDrawLines = value;
                if (DesignMode) Draw();
            }
        }

        public bool Border
        {
            get => _pBorder;
            set
            {
                _pBorder = value;
                if (DesignMode) Draw();
            }
        }

        public bool ShowScale
        {
            get => _pShowScale;
            set
            {
                _pShowScale = value;
                if (DesignMode) Draw();
            }
        }

        public bool Highlight
        {
            get => _pShowHighlight;
            set
            {
                _pShowHighlight = value;
                if (DesignMode) Draw();
            }
        }

        public int ScaleHeight
        {
            get => _pScaleHeight;
            set
            {
                _pScaleHeight = value;
                if (DesignMode) Draw();
            }
        }

        public bool Dual
        {
            get => _dualName;
            set
            {
                _dualName = value;
                if (DesignMode) Draw();
            }
        }

        public GraphStyle Style
        {
            get => _pStyle;
            set
            {
                _pStyle = value;
                if (DesignMode) Draw();
            }
        }

        public int ScaleIndex
        {
            get => WhichScale(ScaleValue);
            set
            {
                if ((value > -1) & (value < _scales.Count))
                {
                    ScaleValue = _scales[value];
                }

                if (DesignMode) Draw();
            }
        }

        public float ScaleValue { get; private set; }

        public int ItemCount => _items.Count;

        public int ScaleCount => _scales.Count;

        public float ForcedMax
        {
            get => _pForcedMax;
            set
            {
                _pForcedMax = checked((int) Math.Round(value));
                if (_pForcedMax > 0)
                {
                    ScaleValue = _pForcedMax;
                }
                else
                {
                    Max = GetMaxValue();
                }

                if (DesignMode) Draw();
            }
        }

        public bool DrawRuler
        {
            get => _pDrawRuler;
            set
            {
                _pDrawRuler = value;
                if (DesignMode) Draw();
            }
        }

        public RulerPosition RulerPos
        {
            get => _pRulerPosition;
            set
            {
                _pRulerPosition = value;
                if (DesignMode) Draw();
            }
        }

        public bool Clickable { get; set; }

        public Color BorderColor { get; set; } = Color.Black;

        public List<Color> BaseBarColors { get; set; } = new();
        
        public List<Color> EnhBarColors { get; set; } = new();

        public List<Color> OvercapColors { get; set; } = new();

        public bool DifferentiateColors
        {
            get => _useDifferentiateColors;
            set
            {
                _useDifferentiateColors = value;
                if (DesignMode) Draw();
            }
        }

        public bool Overcap { get; set; } = false;
        
        public Color ColorOvercap { get; set; } = Color.Black;

        public Color ColorAbsorbed { get; set; } = Color.Gainsboro;

        public bool OuterBorder
        {
            get => _displayOuterBorder;
            set
            {
                _displayOuterBorder = value;
                if (DesignMode) Draw();
            }
        }

        public int MaxItems
        {
            get => _maxItemsCapacity;
            set
            {
                _maxItemsCapacity = value;
                if (DesignMode) Draw();
            }
        }

        public float ItemFontSizeOverride
        {
            get => _fontSizeOverride;
            set
            {
                _fontSizeOverride = value;
                if (DesignMode) Draw();
            }
        }

        public List<float> PerItemScales { get; set; } = new();

        public event BarClickEventHandler? BarClick;

        protected override void Dispose(bool disposing)
        {
            if (disposing) Components?.Dispose();

            base.Dispose(disposing);
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            Components = new Container();
            Tip = new ToolTip(Components)
            {
                AutoPopDelay = 10000, InitialDelay = 500, ReshowDelay = 100
            };

            Name = "CtlMultiGraph";
            Size = new Size(332, 156);
        }

        private void ctlMultiGraph_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
            {
                var rng = new Random();
                Clear();
                for (var i = 0 ; i < _maxItemsCapacity ; i++)
                {
                    var max = (int) (PerItemScales.Count >= _maxItemsCapacity
                        ? PerItemScales[i]
                        : ScaleValue);
                    var values = new List<int> {rng.Next(0, max), rng.Next(0, max), rng.Next(0, (int) (1.5 * max))};
                    values.Sort();
                    if (Overcap)
                    {
                        AddItemPair($"value {i}", $"value {i}b",
                            values[0],
                            values[1],
                            values[2],
                            $"{i}");
                    }
                    else
                    {
                        AddItemPair($"value {i}", $"value {i}b",
                            values[0],
                            values[1],
                            $"{i}");
                    }
                }

                Draw();
            }

            _loaded = true;
        }   

        public void AddItem(string sName, float nBase, float nEnh, string iTip = "")
        {
            _items.Add(new GraphItem(sName, nBase, nEnh, iTip));
        }

        public void AddItem(string sName, float nBase, float nEnh, float nOvercap, string iTip = "")
        {
            _items.Add(new GraphItem(sName, sName, nBase, nEnh, nOvercap, iTip));
        }

        public void AddItem(string sName, float nBase, float nEnh, float nOvercap, float nAbsorbed, string iTip = "")
        {
            _items.Add(new GraphItem(sName, sName, nBase, nEnh, nOvercap, nAbsorbed, iTip));
        }

        public void AddItemPair(string sName, string sName2, float nBase, float nEnh, string iTip = "")
        {
            _items.Add(new GraphItem(sName, sName2, nBase, nEnh, iTip));
        }

        public void AddItemPair(string sName, string sName2, float nBase, float nEnh, float nOvercap, string iTip = "")
        {
            _items.Add(new GraphItem(sName, sName2, nBase, nEnh, nOvercap, iTip));
        }

        public void AddItemPair(string sName, string sName2, float nBase, float nEnh, float nOvercap, float nAbsorbed, string iTip = "")
        {
            _items.Add(new GraphItem(sName, sName2, nBase, nEnh, nOvercap, nAbsorbed, iTip));
        }

        public void Clear()
        {
            _items = new List<GraphItem>();
        }

        /// <summary>
        /// Check if text contains letters going below text base line (on a non-monospaced font)
        /// </summary>
        /// <param name="text">Input string</param>
        /// <returns>true if it contains one of those letters: y, p, q, g, j, Q, Y, false otherwise</returns>
        private static bool BaseLineOffset(string text)
        {
            return Regex.IsMatch(text, @"[ypqgjQY]");
        }

        public void Draw()
        {
            if (_noDraw)
            {
                return;
            }

            using var s = SKSurface.Create(new SKImageInfo(Width, Height));
            s.Canvas.Clear(new SKColor(BackColor.R, BackColor.G, BackColor.B));

            var drawArea = new SKRect(_nameWidth, 0, Width - 1, Height - 1);
            var fullArea = new SKRect(0, 0, Width, Height);
            var fontSize = _fontSizeOverride > 0 ? _fontSizeOverride : _fontSize;
            using var bgGradientBrush = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(drawArea.Left, drawArea.Top), new SKPoint(drawArea.Right, drawArea.Top),
                    new[] {_pBlendColor1.ToSKColor(), _pBlendColor1.ToSKColor(), _pBlendColor2.ToSKColor()},
                    new[] {0, _nameWidth / (float) Width, 1f},
                    SKShaderTileMode.Clamp
                ),
                Style = SKPaintStyle.Fill
            };

            using var fgBrush = new SKPaint
            {
                Color = ForeColor.ToSKColor()
            };

            using var lineBrush = new SKPaint
            {
                IsAntialias = true,
                Color = _pLineColor.ToSKColor(),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var borderBrush = new SKPaint
            {
                IsAntialias = true,
                Color = BorderColor.ToSKColor(),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            s.Canvas.DrawRect(fullArea, bgGradientBrush);

            var rulerYoffsetTop = 0;
            var rulerYoffsetBottom = 0;
            const int rulerHeight = 15;
            if (_pDrawRuler)
            {
                DrawRulerLines(s, drawArea);
                switch (_pRulerPosition)
                {
                    case RulerPosition.Both:
                        rulerYoffsetTop = rulerHeight;
                        rulerYoffsetBottom = rulerHeight;
                        break;

                    case RulerPosition.Bottom:
                        rulerYoffsetBottom = rulerHeight;
                        break;

                    default:
                        rulerYoffsetTop = rulerHeight;
                        break;
                }

                drawArea = new SKRect(drawArea.Left, drawArea.Top + rulerYoffsetTop, drawArea.Right, drawArea.Bottom - rulerYoffsetBottom);
            }
            
            //DrawBackground(s, drawArea);
            DrawHighlight(s, drawArea);

            /*var textPaint = new SKPaint
            {
                IsAntialias = true,
                Color = ForeColor.ToSKColor(),
                TextSize = fontSize,
            };*/

            const float nameXPadding = 3;
            for (var i = 0; i < _items.Count; i++)
            {
                // Data sent from some components might contain nulls (e.g. frmCompare)
                if (string.IsNullOrWhiteSpace(_items[i].Name) & string.IsNullOrWhiteSpace(_items[i].Name2))
                {
                    continue;
                }

                var ny = _yPadding + i * (_pItemHeight + _yPadding);
                var textRect = new SKRect(nameXPadding,
                    drawArea.Top + ny - _yPadding / 2f - (BaseLineOffset(_items[i].Name) ? fontSize / 4f : 0),
                    Math.Max(Name2XPadding, _nameWidth) - Name2XPadding,
                    drawArea.Top + ny + _pItemHeight + _yPadding / 2f);
                var textRect2 = textRect with { Top = drawArea.Top + ny - _yPadding / 2f };
                var separatorTextIndex = _items[i].Name.IndexOf('|', StringComparison.Ordinal);
                var label1 = _items[i].Name;
                var label2 = _items[i].Name2;
                if (separatorTextIndex >= 0)
                {
                    var combinedLabel = _items[i].Name;
                    label1 = combinedLabel[..separatorTextIndex];
                    label2 = combinedLabel[(separatorTextIndex + 1)..];
                }

                if (!_singleLineLabels)
                {
                    var num = Style == GraphStyle.Twin
                        ? (int)Math.Round(_pItemHeight / 2f)
                        : _pItemHeight;
                    var ny2 = ny + (Style == GraphStyle.Twin ? num : 0);

                    textRect = new SKRect(textRect.Left, drawArea.Top + ny, textRect.Right, drawArea.Top + ny + num);
                    textRect2 = new SKRect(textRect2.Left, drawArea.Top + ny2, textRect2.Right, drawArea.Top + ny2 + num);
                }

                s.Canvas.DrawOutlineText(label1, textRect, ForeColor.ToSKColor(), eHTextAlign.Left, eVTextAlign.Middle, 255, fontSize, 3, true);
                if (!string.IsNullOrWhiteSpace(label2) && label1 != label2)
                {
                    s.Canvas.DrawOutlineText(label2, textRect2, ForeColor.ToSKColor(), eHTextAlign.Left, eVTextAlign.Middle, 255, fontSize, 3, true);
                }

                if (Overcap)
                {
                    DrawOvercap(s, i, drawArea, ny);
                }

                switch (_pStyle)
                {
                    case GraphStyle.baseOnly:
                        DrawBase(s, i, drawArea, ny);
                        break;
                    case GraphStyle.enhOnly:
                        DrawEnh(s, i, drawArea, ny);
                        break;
                    default:
                        if (_items[i].ValueBase > _items[i].ValueEnh)
                        {
                            DrawBase(s, i, drawArea, ny);
                            DrawEnh(s, i, drawArea, ny);
                        }
                        else
                        {
                            DrawEnh(s, i, drawArea, ny);
                            DrawBase(s, i, drawArea, ny);
                        }

                        break;
                }

                if (_items[i].ValueAbsorbed <= 0)
                {
                    continue;
                }

                DrawAbsorbed(s, i, drawArea, ny);
            }

            if (_pBorder)
            {
                s.Canvas.DrawRect(drawArea, lineBrush);
            }

            if (_displayOuterBorder)
            {
                s.Canvas.DrawRect(fullArea, borderBrush);
            }

            BackgroundImage = s.Snapshot().ToBitmap();
        }

        private int CalcContentHeight()
        {
            var contentHeight = 0;
            var drawArea = new SKRect(_nameWidth, 0, Width - 1, Height - 1);
            var fontSize = _fontSizeOverride > 0 ? _fontSizeOverride : _fontSize;
            var rulerYoffsetTop = 0;
            var rulerYoffsetBottom = 0;
            const int rulerHeight = 15;
            if (_pDrawRuler)
            {
                switch (_pRulerPosition)
                {
                    case RulerPosition.Both:
                        rulerYoffsetTop = rulerHeight;
                        rulerYoffsetBottom = rulerHeight;
                        break;

                    case RulerPosition.Bottom:
                        rulerYoffsetBottom = rulerHeight;
                        break;

                    default:
                        rulerYoffsetTop = rulerHeight;
                        break;
                }

                drawArea = new SKRect(drawArea.Left, drawArea.Top + rulerYoffsetTop, drawArea.Right,
                    drawArea.Bottom - rulerYoffsetBottom);
            }

            const float nameXPadding = 3;
            if (_items.Count == 0)
            {
                return 0;
            }

            for (var i = 0; i < _items.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(_items[i].Name) & string.IsNullOrWhiteSpace(_items[i].Name2))
                {
                    continue;
                }

                var ny = _yPadding + i * (_pItemHeight + _yPadding);
                var textRect = new SKRect(nameXPadding,
                    drawArea.Top + ny - _yPadding / 2f - (BaseLineOffset(_items[i].Name) ? fontSize / 4f : 0),
                    Math.Max(Name2XPadding, _nameWidth) - Name2XPadding,
                    drawArea.Top + ny + _pItemHeight + _yPadding / 2f);

                var itemScale = PerItemScales.Count == _items.Count && _items.Count > 0 ? PerItemScales[i] : ScaleValue;
                var barWidth = (int)Math.Round(drawArea.Width * (_items[i].ValueBase / itemScale));
                var barRect = new SKRect(drawArea.Left, drawArea.Top + ny, drawArea.Left + barWidth, drawArea.Top + ny + _pItemHeight);

                contentHeight = (int)Math.Ceiling(Math.Max(barRect.Bottom, textRect.Bottom));
            }

            return contentHeight;
        }

        /// <summary>
        /// Draw ruler lines and numbers, above and/or below graph.
        /// Lines and text are using dark outlines to enhance visibility.
        /// </summary>
        /// <param name="s">Skia Surface to draw on</param>
        /// <param name="bounds">Draw area limits</param>
        private void DrawRulerLines(SKSurface s, SKRect bounds)
        {
            const int nbStops = 10;
            using var linePaint = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(192, 192, 255),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var shadowPaint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var textPaint = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(192, 192, 255)
            };

            /*var values = Items.Select(e => e.ValueEnh < 0 ? Math.Min(e.ValueBase, e.ValueEnh) : Math.Max(e.ValueBase, e.ValueEnh)).ToList();
            var vMin = values.Count > 0 ? Math.Min(values.Min(), 0) : 0;
            var vMax = values.Count <= 0
                ? 0
                : PForcedMax > 0
                    ? Math.Min(PForcedMax, values.Max())
                    : values.Max();
            */

            var vMin = 0f;
            var vMax = ScaleValue;

            if (_pRulerPosition != RulerPosition.Bottom)
            {
                s.Canvas.DrawLine(bounds.Left, 0, bounds.Right, 0, shadowPaint);
                s.Canvas.DrawLine(bounds.Left, 2, bounds.Right, 2, shadowPaint);
                for (var i = 0; i <= nbStops; i++)
                {
                    var x = bounds.Left + i * bounds.Width / nbStops;
                    s.Canvas.DrawLine(x - 1, 1, x - 1, 4, shadowPaint);
                    s.Canvas.DrawLine(x + 1, 1, x + 1, 4, shadowPaint);
                }

                s.Canvas.DrawLine(bounds.Left, 1, bounds.Right, 1, linePaint);

                textPaint.TextSize = 10;

                for (var i = 0; i <= nbStops; i++)
                {
                    var x = bounds.Left + i * bounds.Width / nbStops;
                    var v = (int) Math.Round(vMin + i * (vMax - vMin) / nbStops);
                    var textRect = new SKRect();
                    textPaint.MeasureText($"{v}", ref textRect);
                    s.Canvas.DrawLine(x, 1, x, 4, linePaint);
                    s.Canvas.DrawOutlineText($"{v}",
                        i < nbStops
                            ? new SKRect(x - textRect.Width / 2f, 7, x + textRect.Width / 2f, 15)
                            : new SKRect(x - textRect.Width - 1, 7, x - 1, 15), new SKColor(192, 192, 255),
                        eHTextAlign.Center, eVTextAlign.Top, 255, 10, 3, true);
                }
            }

            if (_pRulerPosition != RulerPosition.Top)
            {
                s.Canvas.DrawLine(bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom, shadowPaint);
                s.Canvas.DrawLine(bounds.Left, bounds.Bottom - 2, bounds.Right, bounds.Bottom - 2, shadowPaint);
                for (var i = 0; i <= nbStops; i++)
                {
                    var x = bounds.Left + i * bounds.Width / nbStops;
                    s.Canvas.DrawLine(x - 1, bounds.Bottom - 1, x - 1, bounds.Bottom - 4, linePaint);
                    s.Canvas.DrawLine(x + 1, bounds.Bottom - 1, x + 1, bounds.Bottom - 4, linePaint);
                }

                s.Canvas.DrawLine(bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1, linePaint);

                for (var i = 0; i <= nbStops; i++)
                {
                    var x = bounds.Left + i * bounds.Width / nbStops;
                    var v = (int) Math.Round(vMin + i * (vMax - vMin) / nbStops);
                    var textRect = new SKRect();
                    textPaint.MeasureText($"{v}", ref textRect);
                    s.Canvas.DrawLine(x, bounds.Bottom - 1, x, bounds.Bottom - 4, linePaint);
                    s.Canvas.DrawOutlineText($"{v}", new SKRect(x - (i < nbStops ? 0 : 1), bounds.Bottom - 15, textRect.Width, bounds.Bottom - 7), new SKColor(192, 192, 255), i < nbStops ? eHTextAlign.Center : eHTextAlign.Right, eVTextAlign.Bottom, 255, 10, 3, true);
                }
            }
        }

        private void DrawHighlight(SKSurface s, SKRect bounds)
        {
            checked
            {
                if (!_pShowHighlight | (_pHighlight == -1))
                {
                    return;
                }

                using var fillBrush = new SKPaint
                {
                    Color = new SKColor(_pHighlightColor.R, _pHighlightColor.G, _pHighlightColor.B, 128),
                    Style = SKPaintStyle.Fill
                };

                var ny = _yPadding + _pHighlight * (_pItemHeight + _yPadding);
                var drawRect = new SKRect(0, bounds.Top + ny - _yPadding / 2f, Width, bounds.Top + ny + _pItemHeight + _yPadding / 2f);

                s.Canvas.DrawRect(drawRect, fillBrush);
            }
        }

        private void DrawBackground(SKSurface s, SKRect bounds)
        {
            checked
            {
                if (!_pShowScale)
                {
                    return;
                }

                using var fillBrush = new SKPaint
                {
                    Color = ForeColor.ToSKColor(),
                    Style = SKPaintStyle.Fill
                };

                using var fillBgBrush = new SKPaint
                {
                    Color = BackColor.ToSKColor(),
                    Style = SKPaintStyle.Fill
                };

                using var lineBrush = new SKPaint
                {
                    IsAntialias = true,
                    Color = ForeColor.ToSKColor(),
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1,
                    StrokeCap = SKStrokeCap.Butt
                };

                using var textPaint = new SKPaint
                {
                    IsAntialias = true,
                    Color = ForeColor.ToSKColor()
                };

                var rect = new SKRect(bounds.Left, bounds.Top, bounds.Width, _pScaleHeight);
                bounds.Top += rect.Height;
                bounds.Bottom -= rect.Height;
                s.Canvas.DrawRect(rect, fillBgBrush);
                s.Canvas.DrawLine(rect.Left, rect.Top + rect.Height, rect.Left + rect.Width, rect.Top + rect.Height, lineBrush);
                
                var num = (int) Math.Round(rect.Width / 10f);
                var num2 = (int) Math.Round(Font.Size + 1f);
                var num3 = _nameWidth;
                var num4 = (int) Math.Round(rect.Top + rect.Height / 5f * 4);
                for (var i = 0; i < 11; i++)
                {
                    num3 = Math.Max(num3, Size.Width - 1);

                    s.Canvas.DrawLine(num3, num4, num3, rect.Top + rect.Height, lineBrush);
                    var layoutRectangle = new SKRect((float) (num3 - num / 2.0), num4 - num2, num, num2);
                    if (i == 10)
                    {
                        layoutRectangle.Left = num3 - num;
                    }

                    var itemScale = PerItemScales.Count == _items.Count && _items.Count > 0 ? PerItemScales[i] : ScaleValue;
                    var formattedValue = Max switch
                    {
                        >= 100f => $"{itemScale / 10f * i:#,##0}",
                        >= 10f => $"{itemScale / 10f * i:#0}",
                        >= 5f => $"{itemScale / 10f * i:#0.#}",
                        _ => $"{itemScale / 10f * i:#0.##}",
                    };

                    s.Canvas.DrawTextShort(i > 0 ? formattedValue : "0", _fontSize, layoutRectangle.Left, layoutRectangle.Top, textPaint);
                    num3 += num;
                }
            }
        }

        private void DrawBase(SKSurface s, int index, SKRect bounds, int ny)
        {
            BaseBarColors ??= new List<Color>();
            PerItemScales ??= new List<float>();

            using var fillPaint = new SKPaint
            {
                Color = BaseBarColors.Count > 0
                    ? BaseBarColors[index % BaseBarColors.Count].ToSKColor()
                    : _pBaseColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            using var linePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var markerPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pMarkerColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var marker2Paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pMarkerColor2.ToSKColor(),
                StrokeWidth = 3,
                StrokeCap = SKStrokeCap.Butt
            };

            using var clickableMarkerPaint6 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pMarkerColor2.ToSKColor(),
                StrokeWidth = 6,
                StrokeCap = SKStrokeCap.Butt
            };

            using var clickableMarkerPaint2 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pMarkerColor.ToSKColor(),
                StrokeWidth = 2,
                StrokeCap = SKStrokeCap.Butt
            };

            using var clickableMarkerPaint1 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pMarkerColor2.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            checked
            {
                var itemScale = PerItemScales.Count == _items.Count && _items.Count > 0
                    ? PerItemScales[index]
                    : ScaleValue;
                var width = (int) Math.Round(bounds.Width * (_items[index].ValueBase / itemScale));
                var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width,
                    bounds.Top + ny + (Style == GraphStyle.Twin ? (int) Math.Round(_pItemHeight / 2f) : _pItemHeight));
                s.Canvas.DrawRect(rect, fillPaint);
                if (_pDrawLines)
                {
                    s.Canvas.DrawRect(rect, linePaint);
                }

                if (MarkerValue > 0 & Math.Abs(MarkerValue - _items[index].ValueBase) > float.Epsilon)
                {
                    var markerY = (int) Math.Round(rect.Left + bounds.Width * (MarkerValue / itemScale));
                    s.Canvas.DrawLine(markerY, rect.Top + 1, markerY, rect.Bottom, marker2Paint);
                    s.Canvas.DrawLine(markerY, rect.Top + 1, markerY, rect.Bottom, markerPaint);
                }

                if (!Clickable)
                {
                    return;
                }

                s.Canvas.DrawLine(rect.Right, rect.Top + 1, rect.Right, rect.Bottom, clickableMarkerPaint6);
                s.Canvas.DrawLine(rect.Right, rect.Top + 1, rect.Right, rect.Bottom, clickableMarkerPaint2);
                s.Canvas.DrawLine(rect.Right - 1, rect.Top, rect.Right + 1, rect.Top, clickableMarkerPaint1);
                s.Canvas.DrawLine(rect.Right - 1, rect.Bottom, rect.Right + 1, rect.Bottom, clickableMarkerPaint1);
            }
        }

        private void DrawEnh(SKSurface s, int index, SKRect bounds, int ny)
        {
            EnhBarColors ??= new List<Color>();
            PerItemScales ??= new List<float>();

            using var fillBrush = new SKPaint
            {
                Color = EnhBarColors.Count > 0
                    ? EnhBarColors[index % EnhBarColors.Count].ToSKColor()
                    : _pEnhColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            using var lineBrush = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var markerBrush = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pMarkerColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var markerBrush2 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pMarkerColor2.ToSKColor(),
                StrokeWidth = 3,
                StrokeCap = SKStrokeCap.Butt
            };

            /*using var textBrush = new SKPaint
            {
                IsAntialias = true,
                Color = ForeColor.ToSKColor()
            };*/

            checked
            {
                var itemScale = PerItemScales.Count == _items.Count && _items.Count > 0 ? PerItemScales[index] : ScaleValue;
                var width = (int) Math.Round(bounds.Width * (_items[index].ValueEnh / itemScale));
                var num = _pItemHeight;
                if (Style == GraphStyle.Twin)
                {
                    num = (int) Math.Round(num / 2.0);
                    ny += num;
                }

                var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width, bounds.Top + ny + num);
                s.Canvas.DrawRect(rect, fillBrush);
                if (_pDrawLines)
                {
                    s.Canvas.DrawRect(rect, lineBrush);
                }

                if ((MarkerValue > 0f) & (Math.Abs(MarkerValue - _items[index].ValueEnh) > float.Epsilon))
                {
                    var num2 = (int) Math.Round(rect.Left + bounds.Width * (MarkerValue / itemScale));
                    s.Canvas.DrawLine(num2, rect.Top + 1, num2, rect.Bottom, markerBrush2);
                    s.Canvas.DrawLine(num2, rect.Top + 1, num2, rect.Bottom, markerBrush);
                }
            }
        }

        private void DrawOvercap(SKSurface s, int index, SKRect bounds, int ny)
        {
            OvercapColors ??= new List<Color>();
            PerItemScales ??= new List<float>();

            using var fillPaint = new SKPaint
            {
                Color = OvercapColors.Count > 0
                    ? OvercapColors[index % OvercapColors.Count].ToSKColor()
                    : ColorOvercap.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            using var linePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            var itemScale = PerItemScales.Count == _items.Count && _items.Count > 0 ? PerItemScales[index] : ScaleValue;
            var width = (int) Math.Round(bounds.Width * (_items[index].ValueOvercap / itemScale));
            var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width,
                bounds.Top + ny + (Style == GraphStyle.Twin ? (int) Math.Round(_pItemHeight / 2f) : _pItemHeight));
            s.Canvas.DrawRect(rect, fillPaint);
            if (_pDrawLines)
            {
                s.Canvas.DrawRect(rect, linePaint);
            }
        }

        private void DrawAbsorbed(SKSurface s, int index, SKRect bounds, int ny)
        {
            using var fillPaint = new SKPaint
            {
                Color = ColorAbsorbed.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            using var linePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _pLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            var itemScale = PerItemScales.Count == _items.Count && _items.Count > 0 ? PerItemScales[index] : ScaleValue;
            var width = (int) Math.Round(bounds.Width * (_items[index].ValueAbsorbed / itemScale));
            var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width,
                bounds.Top + ny + (Style == GraphStyle.Twin ? (int) Math.Round(_pItemHeight / 2f) : _pItemHeight));
            s.Canvas.DrawRect(rect, fillPaint);
            if (_pDrawLines)
            {
                s.Canvas.DrawRect(rect, linePaint);
            }
        }

        private void ctlMultiGraph_BackColorChanged(object? sender, EventArgs e)
        {
            Draw();
        }

        private void ctlMultiGraph_SizeChanged(object? sender, EventArgs e)
        {
            Draw();
        }

        private void ctlMultiGraph_Paint(object? sender, PaintEventArgs e)
        {
        }

        private void ctlMultiGraph_FontChanged(object? sender, EventArgs e)
        {
            Draw();
        }

        private void ctlMultiGraph_ForeColorChanged(object? sender, EventArgs e)
        {
            Draw();
        }

        private void ctlMultiGraph_Resize(object? sender, EventArgs e)
        {
            Draw();
        }

        private void ctlMultiGraph_MouseMove(object? sender, MouseEventArgs e)
        {
            checked
            {
                if (Clickable & e.Button == MouseButtons.Left)
                {
                    var valueAtXy = GetValueAtXy(e.X, e.Y);
                    var barClickEvent = BarClick;
                    barClickEvent?.Invoke(valueAtXy);
                }
                else if (!(e.X == _oldMouseX & e.Y == _oldMouseY))
                {
                    _oldMouseX = e.X;
                    _oldMouseY = e.Y;
                    var prevHighlight = _pHighlight;
                    var rectangle = new Rectangle(0, 0, Width - 1, Height - 1);
                    if (_pShowScale)
                    {
                        rectangle.Height -= _pScaleHeight;
                        rectangle.Y += _pScaleHeight;
                    }

                    var drawArea = new SKRect(_nameWidth, _pDrawRuler & _pRulerPosition != RulerPosition.Bottom ? 15 : 0, Width - 1, Height - (_pDrawRuler & _pRulerPosition != RulerPosition.Top ? 1 : 16));
                    for (var i = 0 ; i < _items.Count ; i++)
                    {
                        //var num4 = (int) Math.Round(YPadding / 2.0 + checked(i * (PItemHeight + YPadding)));
                        //var rectangle2 = new Rectangle(rectangle.Left, rectangle.Top + num4, width, height);
                        var ny = _yPadding + i * (_pItemHeight + _yPadding);
                        var rectangle2 = new SKRect(0, (int) Math.Round(drawArea.Top + ny - _yPadding / 2f), Width, (int) Math.Round(drawArea.Top + ny + _pItemHeight + _yPadding / 2f));
                        if ((e.X >= rectangle2.Left) & (e.X <= rectangle2.Right) &&
                            (e.Y >= rectangle2.Top) & (e.Y <= rectangle2.Bottom))
                        {
                            _pHighlight = i;
                            Tip?.SetToolTip(this, _items[i].Tip);
                            if (prevHighlight == _pHighlight)
                            {
                                return;
                            }

                            Draw();
                            
                            return;
                        }
                    }

                    _pHighlight = -1;
                    Tip?.SetToolTip(this, "");
                    if (prevHighlight != _pHighlight)
                    {
                        Draw();
                    }
                }
            }
        }

        private void ctlMultiGraph_MouseLeave(object? sender, EventArgs e)
        {
            var num = _pHighlight;
            _pHighlight = -1;
            if (num == _pHighlight)
            {
                return;
            }

            Draw();
        }

        public void BeginUpdate()
        {
            _noDraw = true;
        }

        public void EndUpdate()
        {
            _noDraw = false;
            Draw();
        }

        private void FillScales()
        {
            _scales = new List<float>
            {
                1,
                2,
                3,
                5,
                10,
                25,
                50,
                75,
                100,
                150,
                225,
                300,
                450,
                600,
                900,
                1200,
                2400,
                3000,
                3600,
                4000
            };
        }

        public float GetMaxValue()
        {
            checked
            {
                if (_scales.Count >= 1 && _items.Count >= 1)
                {
                    return Math.Max(
                        _items.Select(e => e.ValueBase).Max(),
                        _items.Select(e => e.ValueEnh).Max());
                }

                ScaleValue = 100;
                    
                return 100;
            }
        }

        private void SetBestScale(float value)
        {
            checked
            {
                if (_scales.Count < 1)
                {
                    ScaleValue = value;
                }
                else
                {
                    foreach (var scale in _scales)
                    {
                        if (scale < value)
                        {
                            continue;
                        }

                        ScaleValue = scale;
                        
                        return;
                    }

                    ScaleValue = _scales[^1];
                }
            }
        }

        private int WhichScale(float iVal)
        {
            checked
            {
                for (var i = 0; i < _scales.Count; i++)
                {
                    if (Math.Abs(_scales[i] - iVal) < float.Epsilon)
                    {
                        return i;
                    }
                }

                return _scales.Count - 1;
            }
        }

        private void ctlMultiGraph_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!(Clickable & (e.Button == MouseButtons.Left)))
            {
                return;
            }

            var valueAtXy = GetValueAtXy(e.X, e.Y);
            BarClick?.Invoke(valueAtXy);
        }

        private float GetValueAtXy(int iX, int iY)
        {
            checked
            {
                var rectangle = new Rectangle(0, 0, Width - 1, Height - 1);
                if (_pShowScale)
                {
                    rectangle.Height -= _pScaleHeight;
                    rectangle.Y += _pScaleHeight;
                }

                var num = rectangle.Width;
                var height = ItemHeight + _yPadding;
                if (!_items.Select((t, i) => (int) Math.Round(_yPadding / 2.0 + checked(i * (_pItemHeight + _yPadding))))
                        .Select(num4 => new Rectangle(rectangle.Left, rectangle.Top + num4, num, height))
                        .Any(rectangle2 =>
                            iX >= rectangle2.X && iY >= rectangle2.Y & iY <= rectangle2.Y + rectangle2.Height |
                            _items.Count == 1))
                {
                    return 0;
                }

                num -= TextWidth;
                
                return iX > TextWidth ? (iX - TextWidth) / (float) num * ScaleValue : 0;
            }
        }

        private class GraphItem
        {
            public string Name;
            public string Name2;
            public readonly string Tip;
            public readonly float ValueBase;
            public readonly float ValueEnh;
            public readonly float ValueOvercap;
            public readonly float ValueAbsorbed;

            public GraphItem(string statName, float valueBase, float valueEnh, string tip = "")
            {
                ValueBase = valueBase;
                ValueEnh = valueEnh;
                ValueOvercap = ValueEnh;
                ValueAbsorbed = 0;
                Name = statName;
                Name2 = "";
                Tip = tip;
            }

            public GraphItem(string statName, float value, string tip = "")
            {
                ValueBase = value;
                ValueEnh = value;
                ValueOvercap = ValueEnh;
                ValueAbsorbed = 0;
                Name = statName;
                Name2 = "";
                Tip = tip;
            }

            public GraphItem(string statName, string statValue, float valueBase, float valueEnh, string tip = "")
            {
                ValueBase = valueBase;
                ValueEnh = valueEnh;
                ValueAbsorbed = 0;
                Name = statName;
                Name2 = statValue;
                Tip = tip;
            }

            public GraphItem(string statName, string statValue, float valueBase, float valueEnh, float valueOvercap, string tip = "")
            {
                ValueBase = valueBase;
                ValueEnh = valueEnh;
                ValueOvercap = valueOvercap;
                ValueAbsorbed = 0;
                Name = statName;
                Name2 = statValue;
                Tip = tip;
            }

            public GraphItem(string statName, string statValue, float valueBase, float valueEnh, float valueOvercap, float valueAbsorbed, string tip = "")
            {
                ValueBase = valueBase;
                ValueEnh = valueEnh;
                ValueOvercap = valueOvercap;
                ValueAbsorbed = valueAbsorbed;
                Name = statName;
                Name2 = statValue;
                Tip = tip;
            }
        }
    }
}
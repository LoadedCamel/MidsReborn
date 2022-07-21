using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mrbBase;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace mrbControls
{
    [DefaultEvent("BarClick")]
    public class ctlMultiGraph : UserControl
    {
        public delegate void BarClickEventHandler(float value);

        private IContainer Components;
        private bool DualName;
        private List<GraphItem> Items;
        private bool Loaded;
        private int NameWidth;
        private bool NoDraw;
        private int OldMouseX;
        private int OldMouseY;
        private Color PBaseColor;
        private Color PBlendColor1;
        private Color PBlendColor2;
        private bool PBorder;
        private bool PDrawLines;
        private Color PEnhColor;
        private int PForcedMax;
        private int PHighlight;
        private Color PHighlightColor;
        private int PItemHeight;
        private Color PLineColor;
        private Color PMarkerColor;
        private Color PMarkerColor2;
        private int PScaleHeight;
        private bool PShowHighlight;
        private bool PShowScale;
        private Enums.GraphStyle PStyle;
        private List<float> Scales;
        private int XPadding;
        private int YPadding;
        private bool UseDifferentiateColors;
        private bool DisplayOuterBorder;
        private int MaxItemsCapacity;
        private float FontSize;
        private float FontSizeOverride;
        private const float Name2XPadding = 9;

        public ctlMultiGraph()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            MouseLeave += ctlMultiGraph_MouseLeave;
            MouseDown += ctlMultiGraph_MouseDown;
            MouseUp += ctlMultiGraph_MouseUp;
            Load += ctlMultiGraph_Load;
            BackColorChanged += ctlMultiGraph_BackColorChanged;
            SizeChanged += ctlMultiGraph_SizeChanged;
            Paint += ctlMultiGraph_Paint;
            FontChanged += ctlMultiGraph_FontChanged;
            ForeColorChanged += ctlMultiGraph_ForeColorChanged;
            Resize += ctlMultiGraph_Resize;
            MouseMove += ctlMultiGraph_MouseMove;
            Scales = new List<float>();
            Items = new List<GraphItem>();
            PBaseColor = Color.Blue;
            PEnhColor = Color.Yellow;
            PBlendColor1 = Color.Black;
            PBlendColor2 = Color.Red;
            PLineColor = Color.Black;
            PStyle = 0;
            PDrawLines = false;
            PBorder = true;
            ScaleValue = 100f;
            YPadding = 5;
            XPadding = 4;
            OldMouseX = 0;
            OldMouseY = 0;
            PItemHeight = 8;
            NameWidth = 72;
            Loaded = false;
            PShowScale = false;
            PScaleHeight = 32;
            PHighlight = -1;
            PHighlightColor = Color.FromArgb(128, 128, 255);
            NoDraw = false;
            DualName = false;
            MarkerValue = 0f;
            PMarkerColor = Color.Black;
            PMarkerColor2 = Color.Yellow;
            PForcedMax = 0;
            Clickable = false;
            MaxItemsCapacity = 60;
            FontSize = PItemHeight;
            FontSizeOverride = 0;
            
            InitializeComponent();
            FillScales();
        }

        [field: AccessedThroughProperty("tTip")]
        protected virtual ToolTip tTip
        {
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public Color ColorBase
        {
            get => PBaseColor;
            set
            {
                PBaseColor = value;
                Draw();
            }
        }

        public Color ColorEnh
        {
            get => PEnhColor;
            set
            {
                PEnhColor = value;
                Draw();
            }
        }

        public Color ColorFadeStart
        {
            get => PBlendColor1;
            set
            {
                PBlendColor1 = value;
                Draw();
            }
        }

        public Color ColorFadeEnd
        {
            get => PBlendColor2;
            set
            {
                PBlendColor2 = value;
                Draw();
            }
        }

        public Color ColorLines
        {
            get => PLineColor;
            set
            {
                PLineColor = value;
                Draw();
            }
        }

        public Color ColorHighlight
        {
            get => PHighlightColor;
            set
            {
                PHighlightColor = value;
                Draw();
            }
        }

        public Color ColorMarkerInner
        {
            get => PMarkerColor;
            set
            {
                PMarkerColor = value;
                Draw();
            }
        }

        public float MarkerValue { get; set; }

        public Color ColorMarkerOuter
        {
            get => PMarkerColor2;
            set
            {
                PMarkerColor2 = value;
                Draw();
            }
        }

        public float Max
        {
            get => ScaleValue;
            set
            {
                SetBestScale(value);
                Draw();
            }
        }

        public float PaddingX
        {
            get => XPadding;
            set
            {
                XPadding = checked((int) Math.Round(value));
                Draw();
            }
        }

        public float PaddingY
        {
            get => YPadding;
            set
            {
                YPadding = checked((int) Math.Round(value));
                Draw();
            }
        }

        public int TextWidth
        {
            get => NameWidth;
            set
            {
                NameWidth = value;
                Draw();
            }
        }

        public int ItemHeight
        {
            get => PItemHeight;
            set
            {
                PItemHeight = value;
                FontSize = value;
                Draw();
            }
        }

        public bool Lines
        {
            get => PDrawLines;
            set
            {
                PDrawLines = value;
                Draw();
            }
        }

        public bool Border
        {
            get => PBorder;
            set
            {
                PBorder = value;
                Draw();
            }
        }

        public bool ShowScale
        {
            get => PShowScale;
            set
            {
                PShowScale = value;
                Draw();
            }
        }

        public bool Highlight
        {
            get => PShowHighlight;
            set
            {
                PShowHighlight = value;
                Draw();
            }
        }

        public int ScaleHeight
        {
            get => PScaleHeight;
            set
            {
                PScaleHeight = value;
                Draw();
            }
        }

        public bool Dual
        {
            get => DualName;
            set
            {
                DualName = value;
                Draw();
            }
        }

        public Enums.GraphStyle Style
        {
            get => PStyle;
            set
            {
                PStyle = value;
                Draw();
            }
        }

        public int ScaleIndex
        {
            get => WhichScale(ScaleValue);
            set
            {
                if ((value > -1) & (value < Scales.Count))
                {
                    ScaleValue = Scales[value];
                }

                Draw();
            }
        }

        public float ScaleValue { get; private set; }

        public int ItemCount => Items.Count;

        public int ScaleCount => Scales.Count;

        public float ForcedMax
        {
            get => PForcedMax;
            set
            {
                PForcedMax = checked((int) Math.Round(value));
                if (PForcedMax > 0)
                {
                    ScaleValue = PForcedMax;
                }
                else
                {
                    Max = GetMaxValue();
                }

                Draw();
            }
        }

        public bool Clickable { get; set; }

        public Color BorderColor { get; set; } = Color.Black;

        public List<Color> BaseBarColors { get; set; } = new();
        
        public List<Color> EnhBarColors { get; set; } = new();

        public List<Color> OvercapColors { get; set; } = new();

        public bool DifferentiateColors
        {
            get => UseDifferentiateColors;
            set
            {
                UseDifferentiateColors = value;
                Draw();
            }
        }

        public bool Overcap { get; set; } = false;
        
        public Color ColorOvercap { get; set; } = Color.Black;

        public Color ColorAbsorbed { get; set; } = Color.Gainsboro;

        public bool OuterBorder
        {
            get => DisplayOuterBorder;
            set
            {
                DisplayOuterBorder = value;
                Draw();
            }
        }

        public int MaxItems
        {
            get => MaxItemsCapacity;
            set
            {
                MaxItemsCapacity = value;
                Draw();
            }
        }

        public float ItemFontSizeOverride
        {
            get => FontSizeOverride;
            set
            {
                FontSizeOverride = value;
                Draw();
            }
        }

        public List<float> PerItemScales { get; set; } = new();

        public event BarClickEventHandler BarClick;

        protected override void Dispose(bool disposing)
        {
            if (disposing) Components?.Dispose();

            base.Dispose(disposing);
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            Components = new Container();
            tTip = new ToolTip(Components)
            {
                AutoPopDelay = 10000, InitialDelay = 500, ReshowDelay = 100
            };

            Name = "ctlMultiGraph";
            Size = new Size(332, 156);
        }

        private void ctlMultiGraph_Load(object sender, EventArgs e)
        {
            if (DesignMode)
            {
                var rng = new Random();
                Clear();
                for (var i = 0 ; i < MaxItemsCapacity ; i++)
                {
                    var max = (int) (PerItemScales.Count >= MaxItemsCapacity
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
            }

            Loaded = true;
            Draw();
        }

        public void AddItem(string sName, float nBase, float nEnh, string iTip = "")
        {
            Items.Add(new GraphItem(sName, nBase, nEnh, iTip));
        }

        public void AddItem(string sName, float nBase, float nEnh, float nOvercap, string iTip = "")
        {
            Items.Add(new GraphItem(sName, sName, nBase, nEnh, nOvercap, iTip));
        }

        public void AddItem(string sName, float nBase, float nEnh, float nOvercap, float nAbsorbed, string iTip = "")
        {
            Items.Add(new GraphItem(sName, sName, nBase, nEnh, nOvercap, nAbsorbed, iTip));
        }

        public void AddItemPair(string sName, string sName2, float nBase, float nEnh, string iTip = "")
        {
            Items.Add(new GraphItem(sName, sName2, nBase, nEnh, iTip));
        }

        public void AddItemPair(string sName, string sName2, float nBase, float nEnh, float nOvercap, string iTip = "")
        {
            Items.Add(new GraphItem(sName, sName2, nBase, nEnh, nOvercap, iTip));
        }

        public void AddItemPair(string sName, string sName2, float nBase, float nEnh, float nOvercap, float nAbsorbed, string iTip = "")
        {
            Items.Add(new GraphItem(sName, sName2, nBase, nEnh, nOvercap, nAbsorbed, iTip));
        }

        public void Clear()
        {
            Items = new List<GraphItem>();
        }

        private bool BaseLineOffset(string text)
        {
            return Regex.IsMatch(text, @"[ypqgjQY]");
        }

        public void Draw()
        {
            if (NoDraw)
            {
                return;
            }

            if (!Loaded)
            {
                return;
            }

            using var s = SKSurface.Create(new SKImageInfo(Width, Height));
            s.Canvas.Clear(new SKColor(BackColor.R, BackColor.G, BackColor.B));

            var drawArea = new SKRect(NameWidth, 0, Width - 1, Height - 1);
            var fullArea = new SKRect(0, 0, Width, Height);
            var fontSize = FontSizeOverride > 0 ? FontSizeOverride : FontSize;
            using var bgGradientBrush = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(drawArea.Left, drawArea.Top), new SKPoint(drawArea.Right, drawArea.Top),
                    new[] {PBlendColor1.ToSKColor(), PBlendColor1.ToSKColor(), PBlendColor2.ToSKColor()},
                    new[] {0, NameWidth / (float) Width, 1f},
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
                Color = PLineColor.ToSKColor(),
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

            //DrawBackground(s, drawArea);
            DrawHighlight(s, drawArea);

            var textPaint = new SKPaint
            {
                IsAntialias = true,
                Color = ForeColor.ToSKColor(),
                TextSize = fontSize,
            };

            const float nameXPadding = 3;
            for (var i = 0; i < Items.Count; i++)
            {
                // Data sent from some components might contain nulls (e.g. frmCompare)
                if (string.IsNullOrWhiteSpace(Items[i].Name) & string.IsNullOrWhiteSpace(Items[i].Name2))
                {
                    continue;
                }

                var ny = YPadding + i * (PItemHeight + YPadding);
                var textRect = new SKRect(nameXPadding,
                    drawArea.Top + ny - YPadding / 2f - (BaseLineOffset(Items[i].Name) ? fontSize / 4f : 0),
                    Math.Max(Name2XPadding, NameWidth) - Name2XPadding,
                    drawArea.Top + ny + PItemHeight + YPadding / 2f);
                var separatorTextIndex = Items[i].Name.IndexOf("|", StringComparison.Ordinal);
                var label1 = Items[i].Name;
                var label2 = Items[i].Name2;
                if (separatorTextIndex >= 0)
                {
                    var combinedLabel = Items[i].Name;
                    label1 = combinedLabel[..separatorTextIndex];
                    label2 = combinedLabel[(separatorTextIndex + 1)..];
                }

                // Both texts have to be drawn on the same horizontal line.
                var label1Coords = s.Canvas.DrawTextShort($"{label1}", fontSize, textRect, textPaint, Enums.eHTextAlign.Left);
                if (!string.IsNullOrWhiteSpace(label2) && label1 != label2)
                {
                    s.Canvas.DrawTextShort(label2, fontSize, textRect.Right, label1Coords.Y, textPaint, Enums.eHTextAlign.Right, Enums.eVTextAlign.Middle, false, true);
                }

                if (Overcap)
                {
                    DrawOvercap(s, i, drawArea, ny);
                }

                switch (PStyle)
                {
                    case Enums.GraphStyle.baseOnly:
                        DrawBase(s, i, drawArea, ny);
                        break;
                    case Enums.GraphStyle.enhOnly:
                        DrawEnh(s, i, drawArea, ny);
                        break;
                    default:
                        if (Items[i].ValueBase > Items[i].ValueEnh)
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

                if (Items[i].ValueAbsorbed > 0)
                {
                    DrawAbsorbed(s, i, drawArea, ny);
                }
            }

            if (PBorder)
            {
                s.Canvas.DrawRect(drawArea, lineBrush);
            }

            if (DisplayOuterBorder)
            {
                s.Canvas.DrawRect(fullArea, borderBrush);
            }

            BackgroundImage = s.Snapshot().ToBitmap();
        }

        private void DrawHighlight(SKSurface s, SKRect bounds)
        {
            checked
            {
                if (!PShowHighlight | (PHighlight == -1))
                {
                    return;
                }

                using var fillBrush = new SKPaint
                {
                    Color = new SKColor(PHighlightColor.R, PHighlightColor.G, PHighlightColor.B, 128),
                    Style = SKPaintStyle.Fill
                };

                var ny = YPadding + PHighlight * (PItemHeight + YPadding);
                var drawRect = new SKRect(0, bounds.Top + ny - YPadding / 2f, Width, bounds.Top + ny + PItemHeight + YPadding / 2f);

                s.Canvas.DrawRect(drawRect, fillBrush);
            }
        }

        private void DrawBackground(SKSurface s, SKRect bounds)
        {
            checked
            {
                if (!PShowScale)
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

                var rect = new SKRect(bounds.Left, bounds.Top, bounds.Width, PScaleHeight);
                bounds.Top += rect.Height;
                bounds.Bottom -= rect.Height;
                s.Canvas.DrawRect(rect, fillBgBrush);
                s.Canvas.DrawLine(rect.Left, rect.Top + rect.Height, rect.Left + rect.Width, rect.Top + rect.Height, lineBrush);
                
                var num = (int) Math.Round(rect.Width / 10f);
                var num2 = (int) Math.Round(Font.Size + 1f);
                var num3 = NameWidth;
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

                    var itemScale = PerItemScales.Count == Items.Count && Items.Count > 0 ? PerItemScales[i] : ScaleValue;
                    var formattedValue = Max switch
                    {
                        >= 100f => $"{itemScale / 10f * i:#,##0}",
                        >= 10f => $"{itemScale / 10f * i:#0}",
                        >= 5f => $"{itemScale / 10f * i:#0.#}",
                        _ => $"{itemScale / 10f * i:#0.##}",
                    };

                    s.Canvas.DrawTextShort(i > 0 ? formattedValue : "0", FontSize, layoutRectangle.Left, layoutRectangle.Top, textPaint);
                    num3 += num;
                }
            }
        }

        private void DrawBase(SKSurface s, int index, SKRect bounds, int ny)
        {
            using var fillPaint = new SKPaint
            {
                Color = BaseBarColors.Count > 0
                    ? BaseBarColors[index % BaseBarColors.Count].ToSKColor()
                    : PBaseColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            using var linePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var markerPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var marker2Paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor2.ToSKColor(),
                StrokeWidth = 3,
                StrokeCap = SKStrokeCap.Butt
            };

            using var clickableMarkerPaint6 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor2.ToSKColor(),
                StrokeWidth = 6,
                StrokeCap = SKStrokeCap.Butt
            };

            using var clickableMarkerPaint2 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor.ToSKColor(),
                StrokeWidth = 2,
                StrokeCap = SKStrokeCap.Butt
            };

            using var clickableMarkerPaint1 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor2.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            checked
            {
                var itemScale = PerItemScales.Count == Items.Count && Items.Count > 0 ? PerItemScales[index] : ScaleValue;
                var width = (int) Math.Round(bounds.Width * (Items[index].ValueBase / itemScale));
                var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width, bounds.Top + ny + (Style == Enums.GraphStyle.Twin ? (int) Math.Round(PItemHeight / 2f) : PItemHeight));
                s.Canvas.DrawRect(rect, fillPaint);
                if (PDrawLines)
                {
                    s.Canvas.DrawRect(rect, linePaint);
                }

                if ((MarkerValue > 0) & (Math.Abs(MarkerValue - Items[index].ValueBase) > float.Epsilon))
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
            using var fillBrush = new SKPaint
            {
                Color = EnhBarColors.Count > 0
                    ? EnhBarColors[index % EnhBarColors.Count].ToSKColor()
                    : PEnhColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            using var lineBrush = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var markerBrush = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var markerBrush2 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor2.ToSKColor(),
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
                var itemScale = PerItemScales.Count == Items.Count && Items.Count > 0 ? PerItemScales[index] : ScaleValue;
                var width = (int) Math.Round(bounds.Width * (Items[index].ValueEnh / itemScale));
                var num = PItemHeight;
                if (Style == Enums.GraphStyle.Twin)
                {
                    num = (int) Math.Round(num / 2.0);
                    ny += num;
                }

                var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width, bounds.Top + ny + num);
                s.Canvas.DrawRect(rect, fillBrush);
                if (PDrawLines)
                {
                    s.Canvas.DrawRect(rect, lineBrush);
                }

                if ((MarkerValue > 0f) & (Math.Abs(MarkerValue - Items[index].ValueEnh) > float.Epsilon))
                {
                    var num2 = (int) Math.Round(rect.Left + bounds.Width * (MarkerValue / itemScale));
                    s.Canvas.DrawLine(num2, rect.Top + 1, num2, rect.Bottom, markerBrush2);
                    s.Canvas.DrawLine(num2, rect.Top + 1, num2, rect.Bottom, markerBrush);
                }
            }
        }

        private void DrawOvercap(SKSurface s, int index, SKRect bounds, int ny)
        {
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
                Color = PLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            var itemScale = PerItemScales.Count == Items.Count && Items.Count > 0 ? PerItemScales[index] : ScaleValue;
            var width = (int) Math.Round(bounds.Width * (Items[index].ValueOvercap / itemScale));
            var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width,
                bounds.Top + ny + (Style == Enums.GraphStyle.Twin ? (int) Math.Round(PItemHeight / 2f) : PItemHeight));
            s.Canvas.DrawRect(rect, fillPaint);
            if (PDrawLines)
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
                Color = PLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            var itemScale = PerItemScales.Count == Items.Count && Items.Count > 0 ? PerItemScales[index] : ScaleValue;
            var width = (int) Math.Round(bounds.Width * (Items[index].ValueBase / itemScale));
            var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width,
                bounds.Top + ny + (Style == Enums.GraphStyle.Twin ? (int) Math.Round(PItemHeight / 2f) : PItemHeight));
            s.Canvas.DrawRect(rect, fillPaint);
            if (PDrawLines)
            {
                s.Canvas.DrawRect(rect, linePaint);
            }
        }

        private void ctlMultiGraph_BackColorChanged(object sender, EventArgs e)
        {
            Draw();
        }

        private void ctlMultiGraph_SizeChanged(object sender, EventArgs e)
        {
            Draw();
        }

        private void ctlMultiGraph_Paint(object sender, PaintEventArgs e)
        {
        }

        private void ctlMultiGraph_FontChanged(object sender, EventArgs e)
        {
            Draw();
        }

        private void ctlMultiGraph_ForeColorChanged(object sender, EventArgs e)
        {
            Draw();
        }

        private void ctlMultiGraph_Resize(object sender, EventArgs e)
        {
            Draw();
        }

        private void ctlMultiGraph_MouseMove(object sender, MouseEventArgs e)
        {
            checked
            {
                if (Clickable & e.Button == MouseButtons.Left)
                {
                    var valueAtXY = GetValueAtXY(e.X, e.Y);
                    var barClickEvent = BarClick;
                    barClickEvent?.Invoke(valueAtXY);
                }
                else if (!(e.X == OldMouseX & e.Y == OldMouseY))
                {
                    OldMouseX = e.X;
                    OldMouseY = e.Y;
                    var num = PHighlight;
                    var rectangle = new Rectangle(0, 0, Width - 1, Height - 1);
                    if (PShowScale)
                    {
                        rectangle.Height -= PScaleHeight;
                        rectangle.Y += PScaleHeight;
                    }

                    var width = rectangle.Width;
                    var height = ItemHeight + YPadding;
                    for (var i = 0 ; i < Items.Count ; i++)
                    {
                        var num4 = (int) Math.Round(YPadding / 2.0 + checked(i * (PItemHeight + YPadding)));
                        var rectangle2 = new Rectangle(rectangle.Left, rectangle.Top + num4, width, height);
                        if ((e.X >= rectangle2.X) & (e.X <= rectangle2.X + rectangle2.Width) &&
                            (e.Y >= rectangle2.Y) & (e.Y <= rectangle2.Y + rectangle2.Height))
                        {
                            PHighlight = i;
                            tTip.SetToolTip(this, Items[i].Tip);
                            if (num == PHighlight)
                            {
                                return;
                            }

                            Draw();
                            
                            return;
                        }
                    }

                    PHighlight = -1;
                    tTip.SetToolTip(this, "");
                    if (num != PHighlight)
                    {
                        Draw();
                    }
                }
            }
        }

        private void ctlMultiGraph_MouseLeave(object sender, EventArgs e)
        {
            var num = PHighlight;
            PHighlight = -1;
            if (num == PHighlight)
            {
                return;
            }

            Draw();
        }

        public void BeginUpdate()
        {
            NoDraw = true;
        }

        public void EndUpdate()
        {
            NoDraw = false;
            Draw();
        }

        private void FillScales()
        {
            Scales = new List<float>
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
                if (Scales.Count >= 1 && Items.Count >= 1)
                {
                    return Math.Max(
                        Items.Select(e => e.ValueBase).Max(),
                        Items.Select(e => e.ValueEnh).Max());
                }

                ScaleValue = 100;
                    
                return 100;
            }
        }

        private void SetBestScale(float value)
        {
            checked
            {
                if (Scales.Count < 1)
                {
                    ScaleValue = value;
                }
                else
                {
                    foreach (var scale in Scales)
                    {
                        if (scale < value)
                        {
                            continue;
                        }

                        ScaleValue = scale;
                        
                        return;
                    }

                    ScaleValue = Scales[^1];
                }
            }
        }

        private int WhichScale(float iVal)
        {
            checked
            {
                for (var i = 0; i < Scales.Count; i++)
                {
                    if (Math.Abs(Scales[i] - iVal) < float.Epsilon)
                    {
                        return i;
                    }
                }

                return Scales.Count - 1;
            }
        }

        private void ctlMultiGraph_MouseDown(object sender, MouseEventArgs e)
        {
            if (!(Clickable & (e.Button == MouseButtons.Left)))
            {
                return;
            }

            var valueAtXY = GetValueAtXY(e.X, e.Y);
            BarClick?.Invoke(valueAtXY);
        }

        private float GetValueAtXY(int iX, int iY)
        {
            checked
            {
                var rectangle = new Rectangle(0, 0, Width - 1, Height - 1);
                if (PShowScale)
                {
                    rectangle.Height -= PScaleHeight;
                    rectangle.Y += PScaleHeight;
                }

                var num = rectangle.Width;
                var height = ItemHeight + YPadding;
                if (!Items.Select((t, i) => (int) Math.Round(YPadding / 2.0 + checked(i * (PItemHeight + YPadding))))
                        .Select(num4 => new Rectangle(rectangle.Left, rectangle.Top + num4, num, height))
                        .Any(rectangle2 =>
                            iX >= rectangle2.X && iY >= rectangle2.Y & iY <= rectangle2.Y + rectangle2.Height |
                            Items.Count == 1))
                {
                    return 0;
                }

                num -= TextWidth;
                
                return iX > TextWidth ? (iX - TextWidth) / (float) num * ScaleValue : 0;
            }
        }

        private void ctlMultiGraph_MouseUp(object sender, MouseEventArgs e)
        {
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
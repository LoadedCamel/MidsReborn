using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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
            checked
            {
                if (Items.Count < 1 && !IsInDesignMode(this))
                {
                    var num = 0;
                    var rng = new Random();
                    do
                    {
                        AddItemPair($"value {num}", $"value {num}b",
                            rng.Next(0, 101),
                            rng.Next(0, 101),
                            $"{num}");
                        num++;
                    } while (num <= 60);
                }

                Loaded = true;
                Draw();
            }
        }

        public static bool IsInDesignMode(Control c)
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime || c?.Site?.DesignMode == true;
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

        public void Draw()
        {
            checked
            {
                if (NoDraw) return;
                if (!Loaded) return;

                using var s = SKSurface.Create(new SKImageInfo(Width, Height));
                s.Canvas.Clear(new SKColor(BackColor.R, BackColor.G, BackColor.B));

                var drawArea = new SKRect(NameWidth, 0, Width - 1, Height - 1);
                var fullArea = new SKRect(0, 0, Width, Height);
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

                using var textBrush = new SKPaint
                {
                    IsAntialias = true,
                    Color = ForeColor.ToSKColor()
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

                DrawBackground(s, drawArea);
                DrawHighlight(s, drawArea);

                var textPaint = new SKPaint
                {
                    IsAntialias = true,
                    Color = ForeColor.ToSKColor()
                };

                for (var i = 0; i < Items.Count; i++)
                {
                    var y = YPadding + i * (PItemHeight + YPadding);
                    if (!DualName | Items[i].Name == Items[i].Name2)
                    {
                        var text = Items[i].Name;
                        if (text == "")
                        {
                            text = Items[i].Name2;
                        }

                        if (text != "" && !text.Contains(":"))
                        {
                            text += ":";
                        }

                        var separatorTextIndex = text.IndexOf("|", StringComparison.Ordinal);
                        if (separatorTextIndex < 0)
                        {
                            s.Canvas.DrawText(SKTextBlob.Create(text, new SKFont(SKTypeface.Default, Font.Size)), 0, y + Font.Size / 2 + 1, textBrush);
                        }
                        else
                        {
                            s.Canvas.DrawText(SKTextBlob.Create(text[..separatorTextIndex], new SKFont(SKTypeface.Default, Font.Size)), 0, y + Font.Size / 2 + 1, textBrush);

                            var textRect = new SKRect();
                            textBrush.MeasureText(text[(separatorTextIndex + 1)..], ref textRect);
                            s.Canvas.DrawText(SKTextBlob.Create(text[(separatorTextIndex + 1)..], new SKFont(SKTypeface.Default, Font.Size)), NameWidth - 5 - textRect.Width, y + Font.Size / 2 + 1, textBrush);
                        }
                    }

                    if (!(DualName & Items[i].Name != "" & Items[i].Name != Items[i].Name2))
                    {
                        return;
                    }

                    var width = (int)Math.Round(drawArea.Width * (Items[i].ValueBase / ScaleValue));
                    var rect = new SKRect(drawArea.Left, drawArea.Top + y, drawArea.Left + width, drawArea.Top + y + (Style == 0 ? (int) Math.Round(PItemHeight / 2f) : PItemHeight));
                    var layoutRectangle = new SKRect(0, rect.Top, Width - drawArea.Width - XPadding, rect.Height);
                    s.Canvas.DrawText(SKTextBlob.Create($"{Items[i].Name}:", new SKFont(SKTypeface.Default, Font.Size)), layoutRectangle.Left, layoutRectangle.Top, textPaint);

                    if (Overcap)
                    {
                        DrawOvercap(s, i, drawArea, y);
                    }


                    switch (PStyle)
                    {
                        case Enums.GraphStyle.baseOnly:
                            DrawBase(s, i, drawArea, y);
                            break;
                        case Enums.GraphStyle.enhOnly:
                            DrawEnh(s, i, drawArea, y);
                            break;
                        default:
                            if (Items[i].ValueBase > Items[i].ValueEnh)
                            {
                                DrawBase(s, i, drawArea, y);
                                DrawEnh(s, i, drawArea, y);
                            }
                            else
                            {
                                DrawEnh(s, i, drawArea, y);
                                DrawBase(s, i, drawArea, y);
                            }

                            break;
                    }

                    if (Items[i].ValueAbsorbed > 0)
                    {
                        DrawAbsorbed(s, i, drawArea, y);
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

                var drawRect = new SKRect(bounds.Left, bounds.Top + PHighlight * (PItemHeight + YPadding), Width - 1, bounds.Top + PHighlight * (PItemHeight + YPadding) + PItemHeight + YPadding);
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

                    var formattedValue = Max switch
                    {
                        >= 100f => $"{ScaleValue / 10f * i:#,##0}",
                        >= 10f => $"{ScaleValue / 10f * i:#0}",
                        >= 5f => $"{ScaleValue / 10f * i:#0.#}",
                        _ => $"{ScaleValue / 10f * i:#0.##}",
                    };

                    s.Canvas.DrawText(SKTextBlob.Create(i > 0 ? formattedValue : "0", new SKFont(SKTypeface.Default, 11)), layoutRectangle.Left, layoutRectangle.Top, textPaint);
                    num3 += num;
                } //while (num5 <= 10);
            }
        }

        private void DrawBase(SKSurface s, int index, SKRect bounds, int ny)
        {
            var fillPaint = new SKPaint
            {
                Color = BaseBarColors.Count > 0
                    ? BaseBarColors[index % BaseBarColors.Count].ToSKColor()
                    : PBaseColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            var linePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            var markerPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            var marker2Paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor2.ToSKColor(),
                StrokeWidth = 3,
                StrokeCap = SKStrokeCap.Butt
            };

            var clickableMarkerPaint6 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor2.ToSKColor(),
                StrokeWidth = 6,
                StrokeCap = SKStrokeCap.Butt
            };

            var clickableMarkerPaint2 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor.ToSKColor(),
                StrokeWidth = 2,
                StrokeCap = SKStrokeCap.Butt
            };

            var clickableMarkerPaint1 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor2.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            checked
            {
                var width = (int) Math.Round(bounds.Width * (Items[index].ValueBase / ScaleValue));
                var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width, bounds.Top + ny + (Style == 0 ? (int) Math.Round(PItemHeight / 2f) : PItemHeight));
                s.Canvas.DrawRect(rect, fillPaint);
                if (PDrawLines)
                {
                    s.Canvas.DrawRect(rect, linePaint);
                }

                if ((MarkerValue > 0) & (Math.Abs(MarkerValue - Items[index].ValueBase) > float.Epsilon))
                {
                    var markerY = (int) Math.Round(rect.Left + bounds.Width * (MarkerValue / ScaleValue));
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
            var fillBrush = new SKPaint
            {
                Color = EnhBarColors.Count > 0
                    ? EnhBarColors[index % EnhBarColors.Count].ToSKColor()
                    : PEnhColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            var lineBrush = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            var markerBrush = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            var markerBrush2 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PMarkerColor2.ToSKColor(),
                StrokeWidth = 3,
                StrokeCap = SKStrokeCap.Butt
            };

            /*var textBrush = new SKPaint
            {
                IsAntialias = true,
                Color = ForeColor.ToSKColor()
            };*/

            checked
            {
                var width = (int) Math.Round(bounds.Width * (Items[index].ValueEnh / ScaleValue));
                var num = PItemHeight;
                if (Style == 0)
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
                    var num2 = (int) Math.Round(rect.Left + bounds.Width * (MarkerValue / ScaleValue));
                    s.Canvas.DrawLine(num2, rect.Top + 1, num2, rect.Bottom, markerBrush2);
                    s.Canvas.DrawLine(num2, rect.Top + 1, num2, rect.Bottom, markerBrush);
                }

                if (!(DualName & Items[index].Name2 != "" & Items[index].Name != Items[index].Name2))
                {
                    return;
                }

                //var layoutRectangle = new SKRect(0, rect.Top, Width - bounds.Width - XPadding, rect.Height);
                //s.Canvas.DrawText(SKTextBlob.Create($"{Items[index].Name}:", new SKFont(SKTypeface.Default, Font.Size)), layoutRectangle.Left, layoutRectangle.Top, textBrush); // ???
            }
        }

        private void DrawOvercap(SKSurface s, int index, SKRect bounds, int ny)
        {
            var fillPaint = new SKPaint
            {
                Color = OvercapColors.Count > 0
                    ? OvercapColors[index % OvercapColors.Count].ToSKColor()
                    : ColorOvercap.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            var linePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            var textPaint = new SKPaint
            {
                IsAntialias = true,
                Color = ForeColor.ToSKColor()
            };

            checked
            {
                var width = (int) Math.Round(bounds.Width * (Items[index].ValueBase / ScaleValue));
                var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width, bounds.Top + ny + (Style == 0 ? (int) Math.Round(PItemHeight / 2f) : PItemHeight));
                s.Canvas.DrawRect(rect, fillPaint);
                if (PDrawLines)
                {
                    s.Canvas.DrawRect(rect, linePaint);
                }

                if (!(DualName & Items[index].Name != "" & Items[index].Name != Items[index].Name2))
                {
                    return;
                }

                var layoutRectangle = new SKRect(0, rect.Top, Width - bounds.Width - XPadding, rect.Height);
                // s.Canvas.DrawText(SKTextBlob.Create($"{Items[index].Name}:", new SKFont(SKTypeface.Default, Font.Size)), layoutRectangle.Left, layoutRectangle.Top, textPaint);
            }
        }

        private void DrawAbsorbed(SKSurface s, int index, SKRect bounds, int ny)
        {
            var fillPaint = new SKPaint
            {
                Color = ColorAbsorbed.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            var linePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PLineColor.ToSKColor(),
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            var textPaint = new SKPaint
            {
                IsAntialias = true,
                Color = ForeColor.ToSKColor()
            };

            checked
            {
                var width = (int) Math.Round(bounds.Width * (Items[index].ValueBase / ScaleValue));
                var rect = new SKRect(bounds.Left, bounds.Top + ny, bounds.Left + width, bounds.Top + ny + (Style == 0 ? (int) Math.Round(PItemHeight / 2f) : PItemHeight));
                s.Canvas.DrawRect(rect, fillPaint);
                if (PDrawLines)
                {
                    s.Canvas.DrawRect(rect, linePaint);
                }

                if (!(DualName & Items[index].Name != "" & Items[index].Name != Items[index].Name2))
                {
                    return;
                }

                var layoutRectangle = new SKRect(0, rect.Top, Width - bounds.Width - XPadding, rect.Height);
                // s.Canvas.DrawText(SKTextBlob.Create($"{Items[index].Name}:", new SKFont(SKTypeface.Default, Font.Size)), layoutRectangle.Left, layoutRectangle.Top, textPaint);
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
            Scales = new List<float>();
            AddScale(1f);
            AddScale(2f);
            AddScale(3f);
            AddScale(5f);
            AddScale(10f);
            AddScale(25f);
            AddScale(50f);
            AddScale(75f);
            AddScale(100f);
            AddScale(150f);
            AddScale(225f);
            AddScale(300f);
            AddScale(450f);
            AddScale(600f);
            AddScale(900f);
            AddScale(1200f);
            AddScale(2400f);
        }

        private void AddScale(float iValue)
        {
            Scales.Add(iValue);
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

                ScaleValue = 100f;
                    
                return 100f;
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
                for (var i = 0; i < Items.Count; i++)
                {
                    var num4 = (int) Math.Round(YPadding / 2.0 + checked(i * (PItemHeight + YPadding)));
                    var rectangle2 = new Rectangle(rectangle.Left, rectangle.Top + num4, num, height);
                    if (iX < rectangle2.X || !(((iY >= rectangle2.Y) & (iY <= rectangle2.Y + rectangle2.Height)) |
                                               (Items.Count == 1)))
                        continue;
                    num -= TextWidth;
                    float result;
                    if (iX > TextWidth)
                    {
                        iX -= TextWidth;
                        result = (float) (iX / (double) num * ScaleValue);
                    }
                    else
                    {
                        result = 0f;
                    }

                    return result;
                }

                return 0f;
            }
        }

        private void ctlMultiGraph_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private class GraphItem
        {
            public readonly string Name;
            public readonly string Name2;
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
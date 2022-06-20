using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using mrbBase;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Mids_Reborn.Forms.Controls
{
    public partial class SKDamageGraph : UserControl, IDrawLock
    {
        private readonly Enums.eDDText _TextStyle;
        private float _BaseVal;
        private float _EnhancedVal;
        private float _HighestBase; // Not implemented
        private float _HighestEnhanced; // Not implemented
        private float _MaxEnhanced; // Not implemented
        private Enums.eDDAlign _TextAlign; // Not implemented
        private SKColor _FadeBackEnd;
        private SKColor _FadeBackStart;
        private SKColor _FadeBaseEnd;
        private SKColor _FadeBaseStart;
        private SKColor _FadeEnhEnd;
        private SKColor _FadeEnhStart;
        private Enums.eDDGraph _Graph; // Not implemented
        private SKSize _Padding;
        private string _String;
        private Enums.eDDStyle _GraphStyle;
        private SKColor _TextColor;
        private bool _DrawLock;

        public SKDamageGraph()
        {
            _GraphStyle = Enums.eDDStyle.TextUnderGraph;
            _TextStyle = Enums.eDDText.ActualValues;
            _Graph = Enums.eDDGraph.Both; 
            _FadeBackStart = SKColors.Lime;
            _FadeBackEnd = SKColors.Yellow;
            _FadeBaseStart = SKColors.Blue;
            _FadeBaseEnd = SKColors.LightBlue;
            _FadeEnhStart = SKColors.Blue;
            _FadeEnhEnd = SKColors.Red;
            _TextColor = SKColors.WhiteSmoke;
            _Padding = new SKSize(3, 6);
            _TextAlign = Enums.eDDAlign.Center;
            _BaseVal = 100f;
            _EnhancedVal = 196f;
            _MaxEnhanced = 207f;
            _HighestBase = 200f;
            _HighestEnhanced = 414f;
            _String = "196 (100)";

            Load += SKDamageGraph_Load;
            Resize += SKDamageGraph_Resize;

            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel1, new object[] { true });
        }

        private Color ToColor(SKColor color)
        {
            return Color.FromArgb(_FadeBackStart.Red, _FadeBackStart.Green, _FadeBackStart.Blue);
        }

        private SKColor ToSKColor(Color color)
        {
            return new SKColor(color.R, color.G, color.B);
        }

        public int PaddingH
        {
            get => (int) _Padding.Width;
            set
            {
                if (!(value >= 0 & value * 2 < Width - 5))
                {
                    return;
                }

                _Padding = new SKSize(value, _Padding.Height);
                
                Draw();
            }
        }

        public int PaddingV
        {
            get => (int) _Padding.Height;
            set
            {
                if (!(value >= 0 & value * 2 < Height - 5))
                {
                    return;
                }

                _Padding = new SKSize(_Padding.Width, value);

                Draw();
            }
        }

        public Color ColorBackStart
        {
            get => ToColor(_FadeBackStart);
            set
            {
                _FadeBackStart = ToSKColor(value);

                Draw();
            }
        }

        public Color ColorBackEnd
        {
            get => ToColor(_FadeBackEnd);
            set
            {
                _FadeBackEnd = ToSKColor(value);

                Draw();
            }
        }

        public Color ColorBaseStart
        {
            get => ToColor(_FadeBaseStart);
            set
            {
                _FadeBaseStart = ToSKColor(value);

                Draw();
            }
        }

        public Color ColorBaseEnd
        {
            get => ToColor(_FadeBaseEnd);
            set
            {
                _FadeBaseEnd = ToSKColor(value);

                Draw();
            }
        }

        public Color ColorEnhStart
        {
            get => ToColor(_FadeEnhStart);
            set
            {
                _FadeEnhStart = ToSKColor(value);

                Draw();
            }
        }

        public Color ColorEnhEnd
        {
            get => ToColor(_FadeEnhEnd);
            set
            {
                _FadeEnhEnd = ToSKColor(value);

                Draw();
            }
        }

        public Color TextColor
        {
            get => ToColor(_TextColor);
            set
            {
                _TextColor = ToSKColor(value);

                Draw();
            }
        }

        // /////////////////////////////////////////

        public SKColor SKColorBackStart
        {
            get => _FadeBackStart;
            set
            {
                _FadeBackStart = value;

                Draw();
            }
        }

        public SKColor SKColorBackEnd
        {
            get => _FadeBackEnd;
            set
            {
                _FadeBackEnd = value;

                Draw();
            }
        }

        public SKColor SKColorBaseStart
        {
            get => _FadeBaseStart;
            set
            {
                _FadeBaseStart = value;

                Draw();
            }
        }

        public SKColor SKColorBaseEnd
        {
            get => _FadeBaseEnd;
            set
            {
                _FadeBaseEnd = value;

                Draw();
            }
        }

        public SKColor SKColorEnhStart
        {
            get => _FadeEnhStart;
            set
            {
                _FadeEnhStart = value;

                Draw();
            }
        }

        public SKColor SKColorEnhEnd
        {
            get => _FadeEnhEnd;
            set
            {
                _FadeEnhEnd = value;

                Draw();
            }
        }

        public SKColor SKTextColor
        {
            get => _TextColor;
            set
            {
                _TextColor = value;

                Draw();
            }
        }

        // /////////////////////////////////////////

        public Enums.eDDAlign TextAlign
        {
            get => _TextAlign;
            set
            {
                _TextAlign = value;

                Draw();
            }
        }

        public Enums.eDDStyle Style
        {
            get => _GraphStyle;
            set
            {
                _GraphStyle = value;

                Draw();
            }
        }

        public Enums.eDDGraph GraphType
        {
            get => _Graph;
            set
            {
                _Graph = value;

                Draw();
            }
        }

        public float nBaseVal
        {
            get => _BaseVal;
            set
            {
                _BaseVal = value;

                Draw();
            }
        }

        public float nEnhVal
        {
            get => _EnhancedVal;
            set
            {
                _EnhancedVal = value;

                Draw();
            }
        }

        public float nMaxEnhVal
        {
            get => _MaxEnhanced;
            set
            {
                _MaxEnhanced = value;

                Draw();
            }
        }

        public float nHighBase
        {
            get => _HighestBase;
            set
            {
                _HighestBase = value;

                Draw();
            }
        }

        public float nHighEnh
        {
            get => _HighestEnhanced;
            set
            {
                _HighestEnhanced = value;

                Draw();
            }
        }

        public override string Text
        {
            get => _String;
            set
            {
                _String = value;

                Draw();
            }
        }

        // /////////////////////////////////////////

        public void LockDraw()
        {
            _DrawLock = true;
        }

        public void UnlockDraw(bool redraw = true)
        {
            _DrawLock = false;
            if (!redraw)
            {
                return;
            }

            Draw();
        }

        private void SKDamageGraph_Load(object sender, EventArgs e)
        {
            Draw();
        }

        private void SKDamageGraph_Resize(object sender, EventArgs e)
        {
            Draw();
        }

        // https://stackoverflow.com/questions/3961278/word-wrap-a-string-in-multiple-lines
        // https://dev.to/brianelete/text-wrap-with-skiasharp-5722
        private static List<string> WrapText(string text, SKCanvas canvas, SKSize canvasSize, SKFont font, SKSize padding)
        {
            var wrappedLines = new List<string>();
            var actualLine = new StringBuilder();
            var actualWidth = 0f;
            var words = text.Split(new[] { " " }, StringSplitOptions.None);
            using var paint = new SKPaint(font)
            {
                IsAntialias = true
            };

            foreach (var word in words)
            {
                var wSpace = $"{word} ";
                var w = paint.MeasureText(wSpace);
                actualWidth += w;

                if (actualWidth > canvasSize.Width - 2 * padding.Width)
                {
                    wrappedLines.Add(actualLine.ToString());
                    actualLine.Clear();
                    actualWidth = w;
                }

                actualLine.Append(wSpace);
            }

            if (actualLine.Length > 0)
            {
                wrappedLines.Add(actualLine.ToString());
            }

            return wrappedLines;
        }

        private SKImage PaintSurface()
        {
            const int barHeight = 10;
            const int paddingH = 3;
            const int paddingV = 3;
            const float fontSize = 12f;
            const float strokeWidth = 5f;

            using var s = SKSurface.Create(new SKImageInfo(Width, Height));
            s.Canvas.Clear(SKColors.Black);

            // Draw background
            // Black to dark red horizontal gradient
            if (_GraphStyle != Enums.eDDStyle.Text)
            {
                using var bgPaint = new SKPaint
                {
                    Shader = SKShader.CreateLinearGradient(
                        new SKPoint(0, 0), new SKPoint(Width, 0),
                        new[] { SKColors.Black, new SKColor(64, 0, 0) },
                        new float[] { 0, 1 },
                        SKShaderTileMode.Clamp
                    )
                };

                s.Canvas.DrawRect(new SKRect(0, 0, Width, 2 * barHeight), bgPaint);
            }

            // Draw graph bars
            var scaleFactor = Math.Abs(_BaseVal - _EnhancedVal) < float.Epsilon
              ? _EnhancedVal < float.Epsilon
                ? 1
                : Width / _EnhancedVal
              : Width / Math.Max(500, Math.Max(_BaseVal, _EnhancedVal));
            var baseDmgWidth = (int) Math.Round(_BaseVal * scaleFactor);
            var enhDmgWidth = (int) Math.Round(_EnhancedVal * scaleFactor);
            using var baseDmgPaint = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                new SKPoint(0, 0), new SKPoint(baseDmgWidth, 0),
                new[] { _FadeBaseStart, _FadeBaseEnd },
                new float[] { 0, 1 },
                SKShaderTileMode.Clamp
              )
            };

            using var enhDmgPaint = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                new SKPoint(0, 0), new SKPoint(enhDmgWidth, 0),
                new[] { _FadeEnhStart, _FadeEnhEnd },
                new float[] { 0, 1 },
                SKShaderTileMode.Clamp
              )
            };

            if (_GraphStyle == Enums.eDDStyle.TextUnderGraph)
            {
                s.Canvas.DrawRect(new SKRect(0, 0, baseDmgWidth, barHeight), baseDmgPaint);
                s.Canvas.DrawRect(new SKRect(0, barHeight, enhDmgWidth, 2 * barHeight), enhDmgPaint);
            }
            else if (_GraphStyle is Enums.eDDStyle.TextOnGraph or Enums.eDDStyle.Graph)
            {
                s.Canvas.DrawRect(new SKRect(0, 0, baseDmgWidth, Height / 2f), baseDmgPaint);
                s.Canvas.DrawRect(new SKRect(0, Height / 2f, enhDmgWidth, Height), enhDmgPaint);
            }

            if (_GraphStyle == Enums.eDDStyle.Graph)
            {
                return s.Snapshot();
            }

            // Damage Text
            using var textFont = new SKFont(SKTypeface.Default, fontSize);
            using var textPaint = new SKPaint(textFont)
            {
                IsAntialias = true,
                IsStroke = false,
                Color = SKColors.WhiteSmoke,
                TextAlign = SKTextAlign.Center
            };

            var textLines = WrapText(_String,
              s.Canvas,
              new SKSize(Width, Height),
              textFont,
              new SKSize(paddingH, paddingV));

            var infoText = string.Join("\r\n", textLines);
            var y = _GraphStyle == Enums.eDDStyle.TextUnderGraph
                ? barHeight + Height / 2f
                : Height / 2f;

            var textBounds = new SKRect();
            textPaint.MeasureText(infoText, ref textBounds);
            
            using var textPath = textPaint.GetTextPath(infoText, (Width - textBounds.Width - strokeWidth) / 2f, y);
            using var outlinePath = new SKPath();
            textPaint.GetFillPath(textPath, outlinePath);

            using var outlinePaint = new SKPaint
            {
                StrokeCap = SKStrokeCap.Round,
                StrokeMiter = 0, /* Avoid spikes artifacts around sharp edges */
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                Color = SKColors.Black,
                TextAlign = SKTextAlign.Center
            };

            s.Canvas.DrawPath(outlinePath, outlinePaint);
            s.Canvas.DrawText(infoText, new SKPoint(Width / 2f, y), textPaint);

            return s.Snapshot();
        }

        // /////////////////////////////////////////

        public void Draw()
        {
            if (_DrawLock)
            {
                return;
            }

            panel1.BackgroundImage = PaintSurface().ToBitmap();
        }
    }
}
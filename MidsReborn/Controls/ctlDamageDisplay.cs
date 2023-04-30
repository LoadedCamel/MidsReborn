using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Controls
{
    public class ctlDamageDisplay : UserControl
    {
        private readonly Enums.eDDText pText;
        private ExtendedBitmap? bxBuffer;
        private IContainer components;
        private Graphics myGFX;

        [AccessedThroughProperty("myTip")] private ToolTip myTip;
        private float nBase;

        private float nEnhanced;
        private float nHighestBase;
        private float nHighestEnhanced;
        private float nMaxEnhanced;
        private Enums.eDDAlign pAlign;
        private Color pFadeBackEnd;
        private Color pFadeBackStart;
        private Color pFadeBaseEnd;
        private Color pFadeBaseStart;
        private Color pFadeEnhEnd;
        private Color pFadeEnhStart;
        private Enums.eDDGraph pGraph;
        private int phPadding;
        private string pString;
        private Enums.eDDStyle pStyle;
        private Color pTextColor;
        private int pvPadding;

        public ctlDamageDisplay()
        {
            BackColorChanged += ctlDamageDisplay_BackColorChanged;
            Load += ctlDamageDisplay_Load;
            Paint += ctlDamageDisplay_Paint;
            pStyle = (Enums.eDDStyle) 3;
            pText = 0;
            pGraph = (Enums.eDDGraph) 2;
            pFadeBackStart = Color.Lime;
            pFadeBackEnd = Color.Yellow;
            pFadeBaseStart = Color.Blue;
            pFadeBaseEnd = Color.LightBlue;
            pFadeEnhStart = Color.Blue;
            pFadeEnhEnd = Color.Red;
            pTextColor = Color.Black;
            pvPadding = 6;
            phPadding = 3;
            pAlign = (Enums.eDDAlign) 1;
            nBase = 100f;
            nEnhanced = 196f;
            nMaxEnhanced = 207f;
            nHighestBase = 200f;
            nHighestEnhanced = 414f;
            pString = "196 (100)";
            InitializeComponent();
        }

        public int PaddingV
        {
            get => pvPadding;
            set
            {
                if (!((value >= 0) & checked(value * 2 < Height - 5)))
                    return;
                pvPadding = value;
                Draw();
            }
        }

        public int PaddingH
        {
            get => phPadding;
            set
            {
                if (!((value >= 0) & checked(value * 2 < Width - 5)))
                    return;
                phPadding = value;
                Draw();
            }
        }

        public Color ColorBackStart
        {
            get => pFadeBackStart;
            set
            {
                pFadeBackStart = value;
                Draw();
            }
        }

        public Color ColorBackEnd
        {
            get => pFadeBackEnd;
            set
            {
                pFadeBackEnd = value;
                Draw();
            }
        }

        public Color ColorBaseStart
        {
            get => pFadeBaseStart;
            set
            {
                pFadeBaseStart = value;
                Draw();
            }
        }

        public Color ColorBaseEnd
        {
            get => pFadeBaseEnd;
            set
            {
                pFadeBaseEnd = value;
                Draw();
            }
        }

        public Color ColorEnhStart
        {
            get => pFadeEnhStart;
            set
            {
                pFadeEnhStart = value;
                Draw();
            }
        }

        public Color ColorEnhEnd
        {
            get => pFadeEnhEnd;
            set
            {
                pFadeEnhEnd = value;
                Draw();
            }
        }

        public Color TextColor
        {
            get => pTextColor;
            set
            {
                pTextColor = value;
                Draw();
            }
        }

        public Enums.eDDAlign TextAlign
        {
            get => pAlign;
            set
            {
                pAlign = value;
                Draw();
            }
        }

        public Enums.eDDStyle Style
        {
            get => pStyle;
            set
            {
                pStyle = value;
                Draw();
            }
        }

        public Enums.eDDGraph GraphType
        {
            get => pGraph;
            set
            {
                pGraph = value;
                Draw();
            }
        }

        public float nBaseVal
        {
            get => nBase;
            set
            {
                nBase = value;
                Draw();
            }
        }

        public float nEnhVal
        {
            get => nEnhanced;
            set
            {
                nEnhanced = value;
                Draw();
            }
        }

        public float nMaxEnhVal
        {
            get => nMaxEnhanced;
            set
            {
                nMaxEnhanced = value;
                Draw();
            }
        }

        public float nHighBase
        {
            get => nHighestBase;
            set
            {
                nHighestBase = value;
                Draw();
            }
        }

        public float nHighEnh
        {
            get => nHighestEnhanced;
            set
            {
                nHighestEnhanced = value;
                Draw();
            }
        }

        public override string Text
        {
            get => pString;
            set
            {
                pString = value;
                Draw();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();

            base.Dispose(disposing);
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            components = new Container();
            myTip = new ToolTip(components)
            {
                AutoPopDelay = 20000, InitialDelay = 350, ReshowDelay = 100
            };

            Name = "ctlDamageDisplay";
            Size = new Size(312, 104);
        }

        private void ctlDamageDisplay_Load(object sender, EventArgs e)
        {
            myGFX = CreateGraphics();
            bxBuffer = new ExtendedBitmap(Width, Height);
            Draw();
        }

        private void FullUpdate()
        {
            myGFX = CreateGraphics();
            bxBuffer = new ExtendedBitmap(Width, Height);
            Draw();
        }

        private void ctlDamageDisplay_Paint(object sender, PaintEventArgs e)
        {
            if (bxBuffer != null)
                myGFX.DrawImage(bxBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            Draw();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            Draw();
        }

        protected override void OnResize(EventArgs e)
        {
            FullUpdate();
        }

        private void DrawGraph()
        {
            var height = Font.GetHeight(bxBuffer.Graphics);
            var rectangle = new Rectangle(0, 0, Width, Height - 5);
            var rect = new Rectangle(0, 0, Width, Height - 15);
            checked
            {
                if (pStyle == (Enums.eDDStyle) 3)
                {
                    rect.Height = (int) Math.Round(rect.Height - height);
                }

                var rectangle2 = new Rectangle(phPadding, pvPadding, Width - phPadding * 2, rect.Height - pvPadding * 2 - 20);
                var brush = new LinearGradientBrush(rect, pFadeBackStart, pFadeBackEnd, 0f);
                rect.Height -= 20;
                bxBuffer.Graphics.FillRectangle(brush, rect);
                if (Math.Abs(nBase) < float.Epsilon & Math.Abs(nEnhanced) < float.Epsilon)
                {
                    return;
                }

                unchecked
                {
                    if (Math.Abs(nMaxEnhanced) < float.Epsilon)
                    {
                        nMaxEnhanced = nBase * 2;
                    }

                    if (Math.Abs(nHighestEnhanced) < float.Epsilon)
                    {
                        nHighestEnhanced = nBase * 2;
                    }

                    if (Math.Abs(nHighestBase) < float.Epsilon)
                    {
                        nHighestBase = nBase * 2;
                    }
                }

                if (pGraph == 0)
                {
                    var num = (int) Math.Round(nBase / nMaxEnhanced * rectangle2.Width);
                    var brush2 = new SolidBrush(pFadeBaseStart);
                    var rect2 = rectangle2 with { Width = num };
                    bxBuffer.Graphics.FillRectangle(brush2, rect2);
                    var width = (int) Math.Round(nEnhanced / nMaxEnhanced * rectangle2.Width - num);
                    rect2 = rectangle2 with { X = rectangle2.X + num, Width = width };
                    Rectangle rect3 = default;
                    rect3.X = rectangle2.X + num;
                    rect3.Y = rectangle2.Y;
                    rect3.Width = rectangle2.Width - num > 0
                        ? rectangle2.Width - num
                        : 1;

                    rect3.Height = rectangle2.Height;
                    brush = new LinearGradientBrush(rect3, pFadeEnhStart, pFadeEnhEnd, 0f);
                    bxBuffer.Graphics.FillRectangle(brush, rect2);
                }
                else
                {
                    switch (pGraph)
                    {
                        case Enums.eDDGraph.Stacked: //(Enums.eDDGraph) 3:
                        {
                            var num = (int) Math.Round((Math.Abs(nHighestEnhanced) < float.Epsilon ? 0 : nBase / nHighestEnhanced) * rectangle2.Width);
                            var rect2 = rectangle2 with {Width = (int) Math.Round(nBase / nHighestBase * rectangle2.Width)};
                            if (rect2.Width < 1)
                            {
                                rect2.Width = 1;
                            }

                            brush = new LinearGradientBrush(rect2, pFadeBaseStart, pFadeBaseEnd, 0f);
                            rect2 = rectangle2 with {Width = num};
                            bxBuffer.Graphics.FillRectangle(brush, rect2);
                            var width = (int) Math.Round((Math.Abs(nHighestEnhanced) < float.Epsilon ? 0 : (nEnhanced - nBase) / nHighestEnhanced) * rectangle2.Width);
                            rect2 = rectangle2 with {X = rectangle2.X + num, Width = width};
                            if (rect2.Width < 1)
                            {
                                rect2.Width = 1;
                            }

                            brush = new LinearGradientBrush(rectangle2, pFadeEnhStart, pFadeEnhEnd, 0f);
                            bxBuffer.Graphics.FillRectangle(brush, rect2);
                            break;
                        }
                        case Enums.eDDGraph.Both: //(Enums.eDDGraph) 2:
                        {
                            var num2 = (int) Math.Round(rectangle2.Height / 2.0);
                            var num = (int) Math.Round((Math.Abs(nHighestEnhanced) < float.Epsilon ? 0 : nBase / nHighestEnhanced) * rectangle2.Width);
                            var rect2 = rectangle2 with {Width = (int) Math.Round(0.5 * rectangle2.Width), Height = num2};
                            if (rect2.Width < 1)
                            {
                                rect2.Width = 1;
                            }

                            brush = new LinearGradientBrush(rectangle2, pFadeBaseStart, pFadeBaseEnd, 0f);
                            rect2 = rectangle2 with {Width = num, Height = num2};
                            if (rect2.Width < 1)
                            {
                                rect2.Width = 1;
                            }

                            bxBuffer.Graphics.FillRectangle(brush, rect2);
                            var width = (int) Math.Round((Math.Abs(nHighestEnhanced) < float.Epsilon ? 0 : nEnhanced / nHighestEnhanced) * rectangle2.Width);
                            rect2 = new Rectangle(rectangle2.X, num2 + rectangle2.Y, width, num2);
                            if (rect2.Width < 1)
                            {
                                rect2.Width = 1;
                            }

                            brush = new LinearGradientBrush(rectangle2, pFadeEnhStart, pFadeEnhEnd, 0f);
                            bxBuffer.Graphics.FillRectangle(brush, rect2);
                            break;
                        }
                        case Enums.eDDGraph.Enhanced: //(Enums.eDDGraph) 1:
                        {
                            var num = (int) Math.Round((Math.Abs(nHighestEnhanced) < float.Epsilon ? 0 : nEnhanced / nHighestEnhanced) * rectangle2.Width);
                            var rect2 = rectangle2 with {Width = num};
                            if (rect2.Width < 1)
                            {
                                rect2.Width = 1;
                            }

                            var rectangle3 = rectangle2 with {X = rectangle2.X + num, Width = rectangle2.Width - num};
                            brush = new LinearGradientBrush(rectangle3, pFadeEnhStart, pFadeEnhEnd, 0f);
                            bxBuffer.Graphics.FillRectangle(brush, rect2);
                            break;
                        }
                    }
                }

                switch (pStyle)
                {
                    case Enums.eDDStyle.TextOnGraph: //(Enums.eDDStyle) 2:
                        DrawText(rectangle2);
                        break;
                    case Enums.eDDStyle.TextUnderGraph: //(Enums.eDDStyle) 3:
                    {
                        var rectangle3 = rectangle2 with {Y = rectangle2.Y + rectangle2.Height, Height = rectangle.Height - (rectangle2.Y + rectangle2.Height)};
                        DrawText(rectangle3);
                        break;
                    }
                }
            }
        }

        // Token: 0x06000077 RID: 119 RVA: 0x00007244 File Offset: 0x00005444
        private void DrawText(Rectangle bounds)
        {
            var layoutRectangle = new RectangleF(0f, 0f, 0f, 0f);
            var stringFormat = new StringFormat();
            var height = Font.GetHeight(myGFX) + 10;
            if (bxBuffer?.Graphics == null) return;
            bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            layoutRectangle.X = checked(bounds.X + phPadding);
            layoutRectangle.Y = pStyle == Enums.eDDStyle.TextUnderGraph
                ? bounds.Y + (bounds.Height - height) / 2f + 2f + 2f
                : bounds.Y + (bounds.Height - height) / 2f - 1f - 10f;

            layoutRectangle.Width = checked(bounds.Width - phPadding * 2);
            layoutRectangle.Height = bounds.Height;
            Brush brush = new SolidBrush(pTextColor);
            stringFormat.Alignment = pAlign switch
            {
                Enums.eDDAlign.Left => StringAlignment.Near,
                Enums.eDDAlign.Center => StringAlignment.Center,
                Enums.eDDAlign.Right => StringAlignment.Far,
                _ => stringFormat.Alignment
            };

            /*var eDdText = pText;
            if (eDdText == 0)
            {
            }*/

            var txtSizeF = bxBuffer.Graphics.MeasureString(pString, Font);
            bxBuffer.Graphics.TextRenderingHint = txtSizeF.Width > layoutRectangle.Width
                ? TextRenderingHint.ClearTypeGridFit
                : TextRenderingHint.AntiAliasGridFit;
            //Font font = new Font(Font.Name, Font.Size * (layoutRectangle.Width / sizeF.Width), Font.Style, GraphicsUnit.Point);
            var smlFont = new Font("Segoe UI", 9.25f, FontStyle.Bold, GraphicsUnit.Point);
            var font = txtSizeF.Width > layoutRectangle.Width ? smlFont : Font;
            bxBuffer.Graphics.MeasureString(pString, font,
                new SizeF(layoutRectangle.Width, layoutRectangle.Height), stringFormat,
                out _, out var linesFilled);
            layoutRectangle.Y -= linesFilled switch
            {
                0 => 0,
                1 => 0,
                2 => font.GetHeight(myGFX) / 2 - 3,
                _ => font.GetHeight(myGFX) - 3
            };

            if (linesFilled > 3)
            {
                font = smlFont;
            }

            bxBuffer.Graphics.DrawString(pString, font, brush, layoutRectangle, stringFormat);
        }

        // Token: 0x06000078 RID: 120 RVA: 0x00007474 File Offset: 0x00005674
        public void Draw()
        {
            if (bxBuffer == null)
                return;
            var rectangle = new Rectangle(0, 2, Width, Height);
            Brush brush = new SolidBrush(BackColor);
            bxBuffer.Graphics?.FillRectangle(brush, rectangle);
            if (pStyle != 0)
                DrawGraph();
            else
                DrawText(rectangle);
            //bxBuffer.Graphics.SetClip(rectangle2);
            if (bxBuffer.Bitmap != null) myGFX.DrawImageUnscaled(bxBuffer.Bitmap, 0, 0);
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00007505 File Offset: 0x00005705
        public void SetTip(string iTip)
        {
            myTip.SetToolTip(this, iTip);
        }

        // Token: 0x0600007A RID: 122 RVA: 0x00007516 File Offset: 0x00005716
        private void ctlDamageDisplay_BackColorChanged(object sender, EventArgs e)
        {
            Draw();
        }
    }
}
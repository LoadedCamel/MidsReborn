using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Display;

namespace mrbControls
{
    public class ctlDamageDisplay : UserControl
    {
        // Token: 0x0400002A RID: 42
        private readonly Enums.eDDText pText;

        // Token: 0x04000027 RID: 39
        private ExtendedBitmap bxBuffer;

        // Token: 0x04000025 RID: 37
        private IContainer components;

        // Token: 0x04000028 RID: 40
        private Graphics myGFX;

        // Token: 0x04000026 RID: 38
        [AccessedThroughProperty("myTip")] private ToolTip myTip;

        // Token: 0x04000036 RID: 54
        private float nBase;

        // Token: 0x04000037 RID: 55
        private float nEnhanced;

        // Token: 0x04000039 RID: 57
        private float nHighestBase;

        // Token: 0x0400003A RID: 58
        private float nHighestEnhanced;

        // Token: 0x04000038 RID: 56
        private float nMaxEnhanced;

        // Token: 0x04000035 RID: 53
        private Enums.eDDAlign pAlign;

        // Token: 0x0400002D RID: 45
        private Color pFadeBackEnd;

        // Token: 0x0400002C RID: 44
        private Color pFadeBackStart;

        // Token: 0x0400002F RID: 47
        private Color pFadeBaseEnd;

        // Token: 0x0400002E RID: 46
        private Color pFadeBaseStart;

        // Token: 0x04000031 RID: 49
        private Color pFadeEnhEnd;

        // Token: 0x04000030 RID: 48
        private Color pFadeEnhStart;

        // Token: 0x0400002B RID: 43
        private Enums.eDDGraph pGraph;

        // Token: 0x04000034 RID: 52
        private int phPadding;

        // Token: 0x0400003B RID: 59
        private string pString;

        // Token: 0x04000029 RID: 41
        private Enums.eDDStyle pStyle;

        // Token: 0x04000032 RID: 50
        private Color pTextColor;

        // Token: 0x04000033 RID: 51
        private int pvPadding;

        // Token: 0x0600006D RID: 109 RVA: 0x0000689C File Offset: 0x00004A9C
        public ctlDamageDisplay()
        {
            BackColorChanged += ctlDamageDisplay_BackColorChanged;
            Load += ctlDamageDisplay_Load;
            Paint += ctlDamageDisplayt_Paint;
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

        // Token: 0x17000013 RID: 19
        // (get) Token: 0x06000051 RID: 81 RVA: 0x00006634 File Offset: 0x00004834
        // (set) Token: 0x06000052 RID: 82 RVA: 0x0000664C File Offset: 0x0000484C
        public Color ColorBaseStart
        {
            get => pFadeBaseStart;
            set
            {
                pFadeBaseStart = value;
                Draw();
            }
        }

        // Token: 0x17000014 RID: 20
        // (get) Token: 0x06000053 RID: 83 RVA: 0x00006660 File Offset: 0x00004860
        // (set) Token: 0x06000054 RID: 84 RVA: 0x00006678 File Offset: 0x00004878
        public Color ColorBaseEnd
        {
            get => pFadeBaseEnd;
            set
            {
                pFadeBaseEnd = value;
                Draw();
            }
        }

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x06000055 RID: 85 RVA: 0x0000668C File Offset: 0x0000488C
        // (set) Token: 0x06000056 RID: 86 RVA: 0x000066A4 File Offset: 0x000048A4
        public Color ColorEnhStart
        {
            get => pFadeEnhStart;
            set
            {
                pFadeEnhStart = value;
                Draw();
            }
        }

        // Token: 0x17000016 RID: 22
        // (get) Token: 0x06000057 RID: 87 RVA: 0x000066B8 File Offset: 0x000048B8
        // (set) Token: 0x06000058 RID: 88 RVA: 0x000066D0 File Offset: 0x000048D0
        public Color ColorEnhEnd
        {
            get => pFadeEnhEnd;
            set
            {
                pFadeEnhEnd = value;
                Draw();
            }
        }

        // Token: 0x17000017 RID: 23
        // (get) Token: 0x06000059 RID: 89 RVA: 0x000066E4 File Offset: 0x000048E4
        // (set) Token: 0x0600005A RID: 90 RVA: 0x000066FC File Offset: 0x000048FC
        public Color TextColor
        {
            get => pTextColor;
            set
            {
                pTextColor = value;
                Draw();
            }
        }

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x0600005B RID: 91 RVA: 0x00006710 File Offset: 0x00004910
        // (set) Token: 0x0600005C RID: 92 RVA: 0x00006728 File Offset: 0x00004928
        public Enums.eDDAlign TextAlign
        {
            get => pAlign;
            set
            {
                pAlign = value;
                Draw();
            }
        }

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x0600005D RID: 93 RVA: 0x0000673C File Offset: 0x0000493C
        // (set) Token: 0x0600005E RID: 94 RVA: 0x00006754 File Offset: 0x00004954
        public Enums.eDDStyle Style
        {
            get => pStyle;
            set
            {
                pStyle = value;
                Draw();
            }
        }

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x0600005F RID: 95 RVA: 0x00006768 File Offset: 0x00004968
        // (set) Token: 0x06000060 RID: 96 RVA: 0x00006780 File Offset: 0x00004980
        public Enums.eDDGraph GraphType
        {
            get => pGraph;
            set
            {
                pGraph = value;
                Draw();
            }
        }

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x06000061 RID: 97 RVA: 0x00006794 File Offset: 0x00004994
        // (set) Token: 0x06000062 RID: 98 RVA: 0x000067AC File Offset: 0x000049AC
        public float nBaseVal
        {
            get => nBase;
            set
            {
                nBase = value;
                Draw();
            }
        }

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x06000063 RID: 99 RVA: 0x000067C0 File Offset: 0x000049C0
        // (set) Token: 0x06000064 RID: 100 RVA: 0x000067D8 File Offset: 0x000049D8
        public float nEnhVal
        {
            get => nEnhanced;
            set
            {
                nEnhanced = value;
                Draw();
            }
        }

        // Token: 0x1700001D RID: 29
        // (get) Token: 0x06000065 RID: 101 RVA: 0x000067EC File Offset: 0x000049EC
        // (set) Token: 0x06000066 RID: 102 RVA: 0x00006804 File Offset: 0x00004A04
        public float nMaxEnhVal
        {
            get => nMaxEnhanced;
            set
            {
                nMaxEnhanced = value;
                Draw();
            }
        }

        // Token: 0x1700001E RID: 30
        // (get) Token: 0x06000067 RID: 103 RVA: 0x00006818 File Offset: 0x00004A18
        // (set) Token: 0x06000068 RID: 104 RVA: 0x00006830 File Offset: 0x00004A30
        public float nHighBase
        {
            get => nHighestBase;
            set
            {
                nHighestBase = value;
                Draw();
            }
        }

        // Token: 0x1700001F RID: 31
        // (get) Token: 0x06000069 RID: 105 RVA: 0x00006844 File Offset: 0x00004A44
        // (set) Token: 0x0600006A RID: 106 RVA: 0x0000685C File Offset: 0x00004A5C
        public float nHighEnh
        {
            get => nHighestEnhanced;
            set
            {
                nHighestEnhanced = value;
                Draw();
            }
        }

        // Token: 0x17000020 RID: 32
        // (get) Token: 0x0600006B RID: 107 RVA: 0x00006870 File Offset: 0x00004A70
        // (set) Token: 0x0600006C RID: 108 RVA: 0x00006888 File Offset: 0x00004A88
        public override string Text
        {
            get => pString;
            set
            {
                pString = value;
                Draw();
            }
        }

        // Token: 0x0600006E RID: 110 RVA: 0x000069AC File Offset: 0x00004BAC
        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();

            base.Dispose(disposing);
        }

        // Token: 0x0600006F RID: 111 RVA: 0x000069E4 File Offset: 0x00004BE4
        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            components = new Container();
            myTip = new ToolTip(components)
            {
                AutoPopDelay = 20000, InitialDelay = 350, ReshowDelay = 100
            };
            Name = "ctlDamageDisplay";
            var size = new Size(312, 104);
            Size = size;
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00006A62 File Offset: 0x00004C62
        private void ctlDamageDisplay_Load(object sender, EventArgs e)
        {
            myGFX = CreateGraphics();
            bxBuffer = new ExtendedBitmap(Width, Height);
            Draw();
        }

        // Token: 0x06000071 RID: 113 RVA: 0x00006A8F File Offset: 0x00004C8F
        private void FullUpdate()
        {
            myGFX = CreateGraphics();
            bxBuffer = new ExtendedBitmap(Width, Height);
            Draw();
        }

        // Token: 0x06000072 RID: 114 RVA: 0x00006ABC File Offset: 0x00004CBC
        private void ctlDamageDisplayt_Paint(object sender, PaintEventArgs e)
        {
            if (bxBuffer != null)
                myGFX.DrawImage(bxBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        // Token: 0x06000073 RID: 115 RVA: 0x00006B01 File Offset: 0x00004D01
        protected override void OnFontChanged(EventArgs e)
        {
            Draw();
        }

        // Token: 0x06000074 RID: 116 RVA: 0x00006B0B File Offset: 0x00004D0B
        protected override void OnForeColorChanged(EventArgs e)
        {
            Draw();
        }

        // Token: 0x06000075 RID: 117 RVA: 0x00006B15 File Offset: 0x00004D15
        protected override void OnResize(EventArgs e)
        {
            FullUpdate();
        }

        // Token: 0x06000076 RID: 118 RVA: 0x00006B20 File Offset: 0x00004D20
        private void DrawGraph()
        {
            var height = Font.GetHeight(bxBuffer.Graphics);
            var rectangle = new Rectangle(0, 0, Width, Height - 5);
            var rect = new Rectangle(0, 0, Width, Height - 15);
            checked
            {
                if (pStyle == (Enums.eDDStyle) 3) rect.Height = (int) Math.Round(rect.Height - height);

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
                    if (Math.Abs(nMaxEnhanced) < float.Epsilon) nMaxEnhanced = nBase * 2f;

                    if (Math.Abs(nHighestEnhanced) < float.Epsilon) nHighestEnhanced = nBase * 2f;

                    if (Math.Abs(nHighestBase) < float.Epsilon) nHighestBase = nBase * 2f;
                }

                if (pGraph == 0)
                {
                    /*var num = (int) Math.Round(nBase / nMaxEnhanced * rectangle2.Width);
                    var brush2 = new SolidBrush(pFadeBaseStart);
                    var rect2 = new Rectangle(rectangle2.X, rectangle2.Y, num, rectangle2.Height);
                    bxBuffer.Graphics.FillRectangle(brush2, rect2);
                    var width = (int) Math.Round(nEnhanced / nMaxEnhanced * rectangle2.Width - num);
                    rect2 = new Rectangle(rectangle2.X + num, rectangle2.Y, width, rectangle2.Height);
                    Rectangle rect3 = default;
                    rect3.X = rectangle2.X + num;
                    rect3.Y = rectangle2.Y;
                    if (rectangle2.Width - num > 0)
                        rect3.Width = rectangle2.Width - num;
                    else
                        rect3.Width = 1;

                    rect3.Height = rectangle2.Height;
                    brush = new LinearGradientBrush(rect3, pFadeEnhStart, pFadeEnhEnd, 0f);
                    bxBuffer.Graphics.FillRectangle(brush, rect2);*/
                }
                else
                {
                    switch (pGraph)
                    {
                        case (Enums.eDDGraph) 3:
                        {
                            var num = (int) Math.Round(nBase / nHighestEnhanced * rectangle2.Width);
                            var rect2 = new Rectangle(rectangle2.X, rectangle2.Y,
                                (int) Math.Round(nBase / nHighestBase * rectangle2.Width), rectangle2.Height);
                            if (rect2.Width < 1) rect2.Width = 1;

                            brush = new LinearGradientBrush(rect2, pFadeBaseStart, pFadeBaseEnd, 0f);
                            rect2 = new Rectangle(rectangle2.X, rectangle2.Y, num, rectangle2.Height);
                            bxBuffer.Graphics.FillRectangle(brush, rect2);
                            var width = (int) Math.Round((nEnhanced - nBase) / nHighestEnhanced * rectangle2.Width);
                            rect2 = new Rectangle(rectangle2.X + num, rectangle2.Y, width, rectangle2.Height);
                            if (rect2.Width < 1) rect2.Width = 1;

                            brush = new LinearGradientBrush(rectangle2, pFadeEnhStart, pFadeEnhEnd, 0f);
                            bxBuffer.Graphics.FillRectangle(brush, rect2);
                            break;
                        }
                        case (Enums.eDDGraph) 2:
                        {
                            var num2 = (int) Math.Round(rectangle2.Height / 2.0);
                            var num = (int) Math.Round(nBase / nHighestEnhanced * rectangle2.Width);
                            var rect2 = new Rectangle(rectangle2.X, rectangle2.Y,
                                (int) Math.Round(0.5 * rectangle2.Width), num2);
                            if (rect2.Width < 1) rect2.Width = 1;

                            brush = new LinearGradientBrush(rectangle2, pFadeBaseStart, pFadeBaseEnd, 0f);
                            rect2 = new Rectangle(rectangle2.X, rectangle2.Y, num, num2);
                            if (rect2.Width < 1) rect2.Width = 1;

                            bxBuffer.Graphics.FillRectangle(brush, rect2);
                            var width = (int) Math.Round(nEnhanced / nHighestEnhanced * rectangle2.Width);
                            rect2 = new Rectangle(rectangle2.X, num2 + rectangle2.Y, width, num2);
                            if (rect2.Width < 1) rect2.Width = 1;

                            brush = new LinearGradientBrush(rectangle2, pFadeEnhStart, pFadeEnhEnd, 0f);
                            bxBuffer.Graphics.FillRectangle(brush, rect2);
                            break;
                        }
                        case (Enums.eDDGraph) 1:
                        {
                            var num = (int) Math.Round(nEnhanced / nHighestEnhanced * rectangle2.Width);
                            var rect2 = new Rectangle(rectangle2.X, rectangle2.Y, num, rectangle2.Height);
                            if (rect2.Width < 1) rect2.Width = 1;

                            var rectangle3 = new Rectangle(rectangle2.X + num, rectangle2.Y, rectangle2.Width - num,
                                rectangle2.Height);
                            brush = new LinearGradientBrush(rectangle3, pFadeEnhStart, pFadeEnhEnd, 0f);
                            bxBuffer.Graphics.FillRectangle(brush, rect2);
                            break;
                        }
                    }
                }

                switch (pStyle)
                {
                    case (Enums.eDDStyle) 2:
                        DrawText(rectangle2);
                        break;
                    case (Enums.eDDStyle) 3:
                    {
                        var rectangle3 = new Rectangle(rectangle2.X, rectangle2.Y + rectangle2.Height, rectangle2.Width,
                            rectangle.Height - (rectangle2.Y + rectangle2.Height));
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
            bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            layoutRectangle.X = checked(bounds.X + phPadding);
            if (pStyle == Enums.eDDStyle.TextUnderGraph)
                layoutRectangle.Y = bounds.Y + (bounds.Height - height) / 2f + 2f + 2f;
            else
                layoutRectangle.Y = bounds.Y + (bounds.Height - height) / 2f - 1f - 10f;

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
            var smlFont = new Font("Arial", 9.25f, FontStyle.Bold, GraphicsUnit.Point);
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
            if (linesFilled > 3) font = smlFont;
            bxBuffer.Graphics.DrawString(pString, font, brush, layoutRectangle, stringFormat);
        }

        // Token: 0x06000078 RID: 120 RVA: 0x00007474 File Offset: 0x00005674
        public void Draw()
        {
            if (bxBuffer == null)
                return;
            var rectangle = new Rectangle(0, 2, Width, Height);
            Brush brush = new SolidBrush(BackColor);
            bxBuffer.Graphics.FillRectangle(brush, rectangle);
            if (pStyle != 0)
                DrawGraph();
            else
                DrawText(rectangle);
            //bxBuffer.Graphics.SetClip(rectangle2);
            myGFX.DrawImageUnscaled(bxBuffer.Bitmap, 0, 0);
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
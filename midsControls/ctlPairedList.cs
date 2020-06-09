using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Display;
using Microsoft.VisualBasic.CompilerServices;

namespace midsControls
{
    // Token: 0x0200000F RID: 15
    public class ctlPairedList : UserControl
    {
        // Token: 0x14000002 RID: 2
        // (add) Token: 0x060000D6 RID: 214 RVA: 0x00009577 File Offset: 0x00007777
        // (remove) Token: 0x060000D7 RID: 215 RVA: 0x00009591 File Offset: 0x00007791
        public event ItemClickEventHandler ItemClick;

        // Token: 0x14000003 RID: 3
        // (add) Token: 0x060000D8 RID: 216 RVA: 0x000095AB File Offset: 0x000077AB
        // (remove) Token: 0x060000D9 RID: 217 RVA: 0x000095C5 File Offset: 0x000077C5
        public event ItemHoverEventHandler ItemHover;

        // Token: 0x1700003D RID: 61
        // (get) Token: 0x060000DA RID: 218 RVA: 0x000095E0 File Offset: 0x000077E0
        // (set) Token: 0x060000DB RID: 219 RVA: 0x000095F8 File Offset: 0x000077F8
        [field: AccessedThroughProperty("myTip")]
        protected virtual ToolTip myTip
        {
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        // Token: 0x1700003E RID: 62
        // (get) Token: 0x060000DC RID: 220 RVA: 0x00009604 File Offset: 0x00007804
        // (set) Token: 0x060000DD RID: 221 RVA: 0x0000961C File Offset: 0x0000781C
        public int Columns
        {
            get => myColumns;
            set
            {
                if (!(value >= 0 & value < 10))
                    return;
                myColumns = value;
                Draw();
            }
        }

        // Token: 0x1700003F RID: 63
        // (get) Token: 0x060000DE RID: 222 RVA: 0x00009650 File Offset: 0x00007850
        // (set) Token: 0x060000DF RID: 223 RVA: 0x00009668 File Offset: 0x00007868
        public int ValueWidth
        {
            get => myValueWidth;
            set
            {
                if (myColumns < 1)
                {
                    myColumns = 1;
                }

                if (value > 0 & value < 100)
                {
                    myValueWidth = value;
                }
            }
        }

        // Token: 0x17000040 RID: 64
        // (get) Token: 0x060000E0 RID: 224 RVA: 0x000096AC File Offset: 0x000078AC
        // (set) Token: 0x060000E1 RID: 225 RVA: 0x000096C4 File Offset: 0x000078C4
        public bool DoHighlight
        {
            get => Highlightable;
            set
            {
                Highlightable = value;
                Draw();
            }
        }

        // Token: 0x17000041 RID: 65
        // (get) Token: 0x060000E2 RID: 226 RVA: 0x000096D8 File Offset: 0x000078D8
        // (set) Token: 0x060000E3 RID: 227 RVA: 0x000096F0 File Offset: 0x000078F0
        public Color HighlightColor
        {
            get => myHighlightColor;
            set => myHighlightColor = value;
        }

        // Token: 0x17000042 RID: 66
        // (get) Token: 0x060000E4 RID: 228 RVA: 0x000096FC File Offset: 0x000078FC
        // (set) Token: 0x060000E5 RID: 229 RVA: 0x00009714 File Offset: 0x00007914
        public Color HighlightTextColor
        {
            get => myHighlightTextColor;
            set => myHighlightTextColor = value;
        }

        // Token: 0x17000043 RID: 67
        // (get) Token: 0x060000E6 RID: 230 RVA: 0x00009720 File Offset: 0x00007920
        // (set) Token: 0x060000E7 RID: 231 RVA: 0x00009738 File Offset: 0x00007938
        public Color NameColor
        {
            get => NameColour;
            set => NameColour = value;
        }

        // Token: 0x17000044 RID: 68
        // (get) Token: 0x060000E8 RID: 232 RVA: 0x00009744 File Offset: 0x00007944
        // (set) Token: 0x060000E9 RID: 233 RVA: 0x0000975C File Offset: 0x0000795C
        public Color ItemColor
        {
            get => ValueColor;
            set => ValueColor = value;
        }

        // Token: 0x17000045 RID: 69
        // (get) Token: 0x060000EA RID: 234 RVA: 0x00009768 File Offset: 0x00007968
        // (set) Token: 0x060000EB RID: 235 RVA: 0x00009780 File Offset: 0x00007980
        public Color ItemColorAlt
        {
            get => ValueColorA;
            set => ValueColorA = value;
        }

        // Token: 0x17000046 RID: 70
        // (get) Token: 0x060000EC RID: 236 RVA: 0x0000978C File Offset: 0x0000798C
        // (set) Token: 0x060000ED RID: 237 RVA: 0x000097A4 File Offset: 0x000079A4
        public Color ItemColorSpecial
        {
            get => ValueColorB;
            set => ValueColorB = value;
        }

        // Token: 0x17000047 RID: 71
        // (get) Token: 0x060000EE RID: 238 RVA: 0x000097B0 File Offset: 0x000079B0
        // (set) Token: 0x060000EF RID: 239 RVA: 0x000097C8 File Offset: 0x000079C8
        public Color ItemColorSpecCase
        {
            get => ValueColorC;
            set => ValueColorC = value;
        }

        // Token: 0x17000048 RID: 72
        // (get) Token: 0x060000F0 RID: 240 RVA: 0x000097D4 File Offset: 0x000079D4
        public int ItemCount => MyItems.Length;

        // Token: 0x17000049 RID: 73
        // (get) Token: 0x060000F1 RID: 241 RVA: 0x000097F0 File Offset: 0x000079F0
        // (set) Token: 0x060000F2 RID: 242 RVA: 0x00009808 File Offset: 0x00007A08
        public bool ForceBold
        {
            get => myForceBold;
            set
            {
                myForceBold = value;
                Draw();
            }
        }

        // Token: 0x060000F3 RID: 243 RVA: 0x0000981C File Offset: 0x00007A1C
        public ctlPairedList()
        {
            FontChanged += ctlPairedList_FontChanged;
            Load += ctlPairedList_Load;
            Paint += ctlPairedList_Paint;
            MouseDown += Listlabel_MouseDown;
            MouseMove += Listlabel_MouseMove;
            MouseLeave += Listlabel_MouseLeave;
            BackColorChanged += ctlPairedList_BackColorChanged;
            ValueColorD = Color.Aqua;
            LinePadding = 2;
            myForceBold = false;
            InitializeComponent();
        }

        // Token: 0x060000F4 RID: 244 RVA: 0x000098D8 File Offset: 0x00007AD8
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        // Token: 0x060000F5 RID: 245 RVA: 0x00009910 File Offset: 0x00007B10
        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            components = new Container();
            myTip = new ToolTip(components);
            Name = "ctlPairedList";
        }

        // Token: 0x060000F6 RID: 246 RVA: 0x0000993C File Offset: 0x00007B3C
        public void SetUnique()
        {
            if (MyItems.Length > 0)
            {
                MyItems[checked(MyItems.Length - 1)].UniqueColour = true;
            }
        }

        // Token: 0x060000F7 RID: 247 RVA: 0x00009978 File Offset: 0x00007B78
        public bool IsSpecialColor()
        {
            return MyItems[MyItems.Length - 1].VerySpecialColour;
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x000099AE File Offset: 0x00007BAE
        private void ctlPairedList_FontChanged(object sender, EventArgs e)
        {
            Draw();
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x000099B8 File Offset: 0x00007BB8
        private void ctlPairedList_Load(object sender, EventArgs e)
        {
            SetLineHeight();
            CurrentHighlight = -1;
            MyItems = new ItemPair[0];
            myGFX = CreateGraphics();
            bxBuffer = new ExtendedBitmap(Width, Height);
            bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            AddItem(new ItemPair("Item 1:", "Value", false));
            AddItem(new ItemPair("Item 2:", "Alternate", true));
            AddItem(new ItemPair("Item 3:", "1000", false));
            AddItem(new ItemPair("Item 4:", "1,000", false));
            AddItem(new ItemPair("1234567890:", "12345678901234567890", false));
            AddItem(new ItemPair("1234567890:", "12345678901234567890", true));
            AddItem(new ItemPair("1 2 3 4 5 6 7 8 9 0:", "1 2 3 4 5 6 7 8 9 0", false));
            AddItem(new ItemPair("1 2 3 4 5 6 7 8 9 0:", "1 2 3 4 5 6 7 8 9 0", true));
            Draw();
        }

        // Token: 0x060000FA RID: 250 RVA: 0x00009ADA File Offset: 0x00007CDA
        private void FullUpdate()
        {
            SetLineHeight();
            myGFX = CreateGraphics();
            bxBuffer = new ExtendedBitmap(Width, Height);
            Draw();
        }

        // Token: 0x060000FB RID: 251 RVA: 0x00009B10 File Offset: 0x00007D10
        public void Draw()
        {
            checked
            {
                if (bxBuffer == null)
                    return;
                bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Rectangle rect = new Rectangle(0, 0, Width, Height);
                RectangleF rectangleF = new RectangleF(0f, 0f, 0f, 0f);
                StringFormat stringFormat = new StringFormat();
                FontStyle newStyle = FontStyle.Bold;
                FontStyle newStyle2 = FontStyle.Regular;
                if (myForceBold)
                {
                    newStyle2 = FontStyle.Bold;
                    newStyle = FontStyle.Bold;
                }

                //Font font = new Font(Font, newStyle);
                Font font = new Font("Arial", 10.5f, newStyle, GraphicsUnit.Pixel);
                //Font font2 = new Font(Font, newStyle2);
                Font font2 = new Font("Arial", 10.5f, newStyle2, GraphicsUnit.Pixel);
                rectangleF.X = 0f;
                if (myColumns < 1)
                {
                    myColumns = 1;
                }

                int num = (int) Math.Round(Width / (double) myColumns);
                int num2 = (int) Math.Round(num * (myValueWidth / 100f));
                int num3 = num - num2;
                rectangleF.Height = LineHeight;
                bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Brush brush = new SolidBrush(BackColor);
                bxBuffer.Graphics.FillRectangle(brush, rect);
                if (MyItems == null)
                {
                    myGFX.DrawImageUnscaled(bxBuffer.Bitmap, 0, 0);
                }

                if (MyItems != null && MyItems.Length < 1)
                {
                    myGFX.DrawImageUnscaled(bxBuffer.Bitmap, 0, 0);
                }

                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                if (MyItems != null)
                {
                    int num7 = MyItems.Length - 1;
                    for (int i = num6; i <= num7; i++)
                    {
                        PointF location = new PointF(num * num5 - 9, rectangleF.Height * num4 + checked(LinePadding * num4));
                        rectangleF.Location = location;
                        rectangleF.Width = num3;
                        stringFormat.Alignment = StringAlignment.Far;
                        stringFormat.Trimming = StringTrimming.None;
                        stringFormat.FormatFlags = StringFormatFlags.NoWrap;
                        if (Highlightable & CurrentHighlight == i)
                        {
                            brush = new SolidBrush(myHighlightColor);
                            bxBuffer.Graphics.FillRectangle(brush, rectangleF);
                            brush = new SolidBrush(myHighlightTextColor);
                        }
                        else
                        {
                            brush = new SolidBrush(NameColour);
                        }

                        string text = MyItems[i].Name;
                        if (Operators.CompareString(text, "", false) != 0 & !text.EndsWith(":"))
                        {
                            text += ":";
                        }

                        bxBuffer.Graphics.DrawString(text, font, brush, rectangleF, stringFormat);
                        location = new PointF(num * num5 + num3 - 10, rectangleF.Height * num4 + checked(LinePadding * num4));
                        rectangleF.Location = location;
                        rectangleF.Width = num2 + 12;
                        if (Highlightable & CurrentHighlight == i)
                        {
                            brush = new SolidBrush(myHighlightColor);
                            bxBuffer.Graphics.FillRectangle(brush, rectangleF);
                            brush = new SolidBrush(myHighlightTextColor);
                        }
                        else if (MyItems[i].UniqueColour)
                        {
                            brush = new SolidBrush(ValueColorD);
                        }
                        else if (MyItems[i].AlternateColour)
                        {
                            brush = new SolidBrush(ValueColorA);
                        }
                        else if (MyItems[i].ProbColour)
                        {
                            brush = new SolidBrush(ValueColorB);
                        }
                        else
                        {
                            brush = new SolidBrush(ValueColor);
                        }

                        stringFormat.Alignment = StringAlignment.Near;
                        stringFormat.LineAlignment = StringAlignment.Near;
                        bxBuffer.Graphics.DrawString(MyItems[i].Value, font2, brush, rectangleF, stringFormat);
                        num5++;
                        if (num5 < myColumns)
                            continue;
                        num5 = 0;
                        num4++;
                    }
                }

                myGFX.DrawImageUnscaled(bxBuffer.Bitmap, 0, 0);
            }
        }

        // Token: 0x060000FC RID: 252 RVA: 0x00009F80 File Offset: 0x00008180
        private void ctlPairedList_Paint(object sender, PaintEventArgs e)
        {
            if (bxBuffer != null)
            {
                myGFX.DrawImage(bxBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            }
        }

        // Token: 0x060000FD RID: 253 RVA: 0x00009FC5 File Offset: 0x000081C5
        protected override void OnFontChanged(EventArgs e)
        {
            SetLineHeight();
            Draw();
        }

        // Token: 0x060000FE RID: 254 RVA: 0x00009FD6 File Offset: 0x000081D6
        protected override void OnForeColorChanged(EventArgs e)
        {
            Draw();
        }

        // Token: 0x060000FF RID: 255 RVA: 0x00009FE0 File Offset: 0x000081E0
        protected override void OnResize(EventArgs e)
        {
            FullUpdate();
        }

        // Token: 0x06000100 RID: 256 RVA: 0x00009FEC File Offset: 0x000081EC
        private void SetLineHeight()
        {
            Font font = new Font(Font, Font.Style);
            LineHeight = checked((int) Math.Round(font.GetHeight() + LinePadding));
        }

        // Token: 0x06000101 RID: 257 RVA: 0x0000A02C File Offset: 0x0000822C
        public void AddItem(ItemPair iItem)
        {
            checked
            {
                MyItems = (ItemPair[]) Utils.CopyArray(MyItems, new ItemPair[MyItems.Length + 1]);
                MyItems[MyItems.Length - 1] = new ItemPair(iItem);
            }
        }

        // Token: 0x06000102 RID: 258 RVA: 0x0000A078 File Offset: 0x00008278
        public void Clear(bool Redraw = false)
        {
            MyItems = (ItemPair[]) Utils.CopyArray(MyItems, new ItemPair[0]);
            if (Redraw)
            {
                Draw();
            }
        }

        // Token: 0x06000103 RID: 259 RVA: 0x0000A0B4 File Offset: 0x000082B4
        private void Listlabel_MouseDown(object sender, MouseEventArgs e)
        {
            int num = 0;
            int num2 = 0;
            int num3 = -1;
            Rectangle rectangle = default;
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Height = LineHeight;
            checked
            {
                rectangle.Width = (int) Math.Round(Width / (double) myColumns);
                int num4 = 0;
                int num5 = MyItems.Length - 1;
                for (int i = num4; i <= num5; i++)
                {
                    rectangle.X = rectangle.Width * num2;
                    rectangle.Y = (rectangle.Height + LinePadding) * num;
                    if ((e.Y >= rectangle.Y & e.Y <= rectangle.Height + rectangle.Y) &&
                        (e.X >= rectangle.X & e.X <= rectangle.Width + rectangle.X))
                    {
                        num3 = i;
                        break;
                    }

                    num2++;
                    if (num2 < myColumns)
                        continue;
                    num2 = 0;
                    num++;
                }

                if (num3 <= -1)
                    return;
                ItemClickEventHandler itemClickEvent = ItemClick;
                itemClickEvent?.Invoke(num3, e.Button);
            }
        }

        // Token: 0x06000104 RID: 260 RVA: 0x0000A228 File Offset: 0x00008428
        private void Listlabel_MouseMove(object sender, MouseEventArgs e)
        {
            int num = 0;
            int num2 = 0;
            int num3 = -1;
            Rectangle rectangle = default;
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Height = LineHeight;
            checked
            {
                rectangle.Width = (int) Math.Round(Width / (double) myColumns);
                int num4 = 0;
                int num5 = MyItems.Length - 1;
                for (int i = num4; i <= num5; i++)
                {
                    rectangle.X = rectangle.Width * num2;
                    rectangle.Y = (rectangle.Height + LinePadding) * num;
                    if ((e.Y >= rectangle.Y & e.Y <= rectangle.Height + rectangle.Y) &&
                        (e.X >= rectangle.X & e.X <= rectangle.Width + rectangle.X))
                    {
                        num3 = i;
                        break;
                    }

                    num2++;
                    if (num2 < myColumns)
                        continue;
                    num2 = 0;
                    num++;
                }

                if (CurrentHighlight == num3)
                    return;
                CurrentHighlight = num3;
                if (Highlightable)
                {
                    Draw();
                }

                if (num3 > -1)
                {
                    ItemHover?.Invoke(this, num3, MyItems[num3].TagID);
                }
            }
        }

        // Token: 0x06000105 RID: 261 RVA: 0x0000A3D4 File Offset: 0x000085D4
        private void Listlabel_MouseLeave(object sender, EventArgs e)
        {
            CurrentHighlight = -1;
            if (Highlightable)
            {
                Draw();
            }

            myTip.SetToolTip(this, "");
        }

        // Token: 0x06000106 RID: 262 RVA: 0x0000A411 File Offset: 0x00008611
        public void SetTip(string iTip)
        {
            myTip.SetToolTip(this, iTip);
        }

        // Token: 0x06000107 RID: 263 RVA: 0x0000A422 File Offset: 0x00008622
        private void ctlPairedList_BackColorChanged(object sender, EventArgs e)
        {
            Draw();
        }

        // Token: 0x04000064 RID: 100
        private ItemPair[] MyItems;

        // Token: 0x04000065 RID: 101
        private int LineHeight;

        // Token: 0x04000066 RID: 102
        private ExtendedBitmap bxBuffer;

        // Token: 0x04000067 RID: 103
        private Graphics myGFX;

        // Token: 0x04000068 RID: 104
        private Color NameColour;

        // Token: 0x04000069 RID: 105
        private Color ValueColor;

        // Token: 0x0400006A RID: 106
        private Color ValueColorA;

        // Token: 0x0400006B RID: 107
        private Color ValueColorB;

        // Token: 0x0400006C RID: 108
        private Color ValueColorC;

        // Token: 0x0400006D RID: 109
        public Color ValueColorD;

        // Token: 0x0400006E RID: 110
        private Color myHighlightColor;

        // Token: 0x0400006F RID: 111
        private Color myHighlightTextColor;

        // Token: 0x04000070 RID: 112
        private int myColumns;

        // Token: 0x04000071 RID: 113
        private readonly int LinePadding;

        // Token: 0x04000072 RID: 114
        private int myValueWidth;

        // Token: 0x04000073 RID: 115
        private bool Highlightable;

        // Token: 0x04000074 RID: 116
        private int CurrentHighlight;

        // Token: 0x04000075 RID: 117
        private bool myForceBold;

        // Token: 0x04000076 RID: 118
        private IContainer components;

        // Token: 0x04000077 RID: 119

        // Token: 0x02000010 RID: 16
        public class ItemPair
        {
            // Token: 0x06000108 RID: 264 RVA: 0x0000A42C File Offset: 0x0000862C
            public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability, bool iSpecialCase, string iTip)
            {
                Name = iName;
                Value = iValue;
                AlternateColour = iAlternate;
                ProbColour = iProbability;
                VerySpecialColour = iSpecialCase;
                TagID.Add(-1, 0f);
                SpecialTip = iTip;
            }

            // Token: 0x06000109 RID: 265 RVA: 0x0000A484 File Offset: 0x00008684
            public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability = false, bool iSpecialCase = false,
                int iTagID = -1)
            {
                Name = iName;
                Value = iValue;
                AlternateColour = iAlternate;
                ProbColour = iProbability;
                VerySpecialColour = iSpecialCase;
                TagID.Add(iTagID, 0f);
                SpecialTip = "";
            }

            // Token: 0x0600010A RID: 266 RVA: 0x0000A4E0 File Offset: 0x000086E0
            public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability, bool iSpecialCase, Enums.ShortFX iTagID)
            {
                Name = iName;
                Value = iValue;
                AlternateColour = iAlternate;
                ProbColour = iProbability;
                VerySpecialColour = iSpecialCase;
                TagID.Assign(iTagID);
                SpecialTip = "";
            }

            // Token: 0x0600010B RID: 267 RVA: 0x0000A534 File Offset: 0x00008734
            public ItemPair(ItemPair iItem)
            {
                Name = iItem.Name;
                Value = iItem.Value;
                AlternateColour = iItem.AlternateColour;
                ProbColour = iItem.ProbColour;
                VerySpecialColour = iItem.VerySpecialColour;
                TagID.Assign(iItem.TagID);
                SpecialTip = iItem.SpecialTip;
            }

            // Token: 0x0400007A RID: 122
            public string Name;

            // Token: 0x0400007B RID: 123
            public string Value;

            // Token: 0x0400007C RID: 124
            public readonly bool AlternateColour;

            // Token: 0x0400007D RID: 125
            public readonly bool ProbColour;

            // Token: 0x0400007E RID: 126
            public readonly bool VerySpecialColour;

            // Token: 0x0400007F RID: 127
            public bool UniqueColour;

            // Token: 0x04000080 RID: 128
            public Enums.ShortFX TagID;

            // Token: 0x04000081 RID: 129
            private readonly string SpecialTip;
        }

        // Token: 0x02000011 RID: 17
        // (Invoke) Token: 0x0600010D RID: 269
        public delegate void ItemClickEventHandler(int Index, MouseButtons Button);

        // Token: 0x02000012 RID: 18
        // (Invoke) Token: 0x06000111 RID: 273
        public delegate void ItemHoverEventHandler(object Sender, int Index, Enums.ShortFX TagID);
    }
}

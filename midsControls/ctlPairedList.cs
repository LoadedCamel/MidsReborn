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
    
    public class ctlPairedList : UserControl
    {
        public delegate void ItemClickEventHandler(int Index, MouseButtons Button);
        public delegate void ItemHoverEventHandler(object Sender, int Index, Enums.ShortFX TagID);
        private readonly int LinePadding;
        private ExtendedBitmap bxBuffer;
        private IContainer components;
        private int CurrentHighlight;
        private bool Highlightable;
        private int LineHeight;
        private int myColumns;
        private bool myForceBold;
        private Graphics myGFX;
        private ItemPair[] MyItems;
        private int myValueWidth;
        public Color ValueColorD;

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
        
        [field: AccessedThroughProperty("myTip")]
        protected virtual ToolTip myTip
        {
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }
        
        public int Columns
        {
            get => myColumns;
            set
            {
                if (!((value >= 0) & (value < 10)))
                    return;
                myColumns = value;
                Draw();
            }
        }
        
        public int ValueWidth
        {
            get => myValueWidth;
            set
            {
                if (myColumns < 1) myColumns = 1;

                if ((value > 0) & (value < 100)) myValueWidth = value;
            }
        }
        
        public bool DoHighlight
        {
            get => Highlightable;
            set
            {
                Highlightable = value;
                Draw();
            }
        }

        public Color HighlightColor { get; set; }
        public Color HighlightTextColor { get; set; }
        public Color NameColor { get; set; }
        public Color ItemColor { get; set; }
        public Color ItemColorAlt { get; set; }
        public Color ItemColorSpecial { get; set; }
        public Color ItemColorSpecCase { get; set; }
        public int ItemCount => MyItems.Length;

        public bool ForceBold
        {
            get => myForceBold;
            set
            {
                myForceBold = value;
                Draw();
            }
        }

        public event ItemClickEventHandler ItemClick;
        public event ItemHoverEventHandler ItemHover;
        
        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();

            base.Dispose(disposing);
        }
        
        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            components = new Container();
            myTip = new ToolTip(components);
            Name = "ctlPairedList";
        }
        
        public void SetUnique()
        {
            if (MyItems.Length > 0) MyItems[checked(MyItems.Length - 1)].UniqueColour = true;
        }
        
        public bool IsSpecialColor()
        {
            return MyItems[MyItems.Length - 1].VerySpecialColour;
        }

        private void ctlPairedList_FontChanged(object sender, EventArgs e)
        {
            Draw();
        }

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
        
        private void FullUpdate()
        {
            SetLineHeight();
            myGFX = CreateGraphics();
            bxBuffer = new ExtendedBitmap(Width, Height);
            Draw();
        }
        
        public void Draw()
        {
            checked
            {
                if (bxBuffer == null)
                    return;
                bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                var rect = new Rectangle(0, 0, Width, Height);
                var rectangleF = new RectangleF(0f, 0f, 0f, 0f);
                var stringFormat = new StringFormat();
                var newStyle = FontStyle.Bold;
                var newStyle2 = FontStyle.Regular;
                if (myForceBold)
                {
                    newStyle2 = FontStyle.Bold;
                    newStyle = FontStyle.Bold;
                }
                
                var font = new Font("Arial", 10.5f, newStyle, GraphicsUnit.Pixel);
                
                var font2 = new Font("Arial", 10.5f, newStyle2, GraphicsUnit.Pixel);
                rectangleF.X = 0f;
                if (myColumns < 1) myColumns = 1;

                var num = (int) Math.Round(Width / (double) myColumns);
                var num2 = (int) Math.Round(num * (myValueWidth / 100f));
                var num3 = num - num2;
                rectangleF.Height = LineHeight;
                bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Brush brush = new SolidBrush(BackColor);
                bxBuffer.Graphics.FillRectangle(brush, rect);
                if (MyItems == null) myGFX.DrawImageUnscaled(bxBuffer.Bitmap, 0, 0);

                if (MyItems != null && MyItems.Length < 1) myGFX.DrawImageUnscaled(bxBuffer.Bitmap, 0, 0);

                var num4 = 0;
                var num5 = 0;
                var num6 = 0;
                if (MyItems != null)
                {
                    var num7 = MyItems.Length - 1;
                    for (var i = num6; i <= num7; i++)
                    {
                        var location = new PointF(num * num5 - 9, rectangleF.Height * num4 + checked(LinePadding * num4));
                        rectangleF.Location = location;
                        rectangleF.Width = num3;
                        stringFormat.Alignment = StringAlignment.Far;
                        stringFormat.Trimming = StringTrimming.None;
                        stringFormat.FormatFlags = StringFormatFlags.NoWrap;
                        if (Highlightable & (CurrentHighlight == i))
                        {
                            brush = new SolidBrush(HighlightColor);
                            bxBuffer.Graphics.FillRectangle(brush, rectangleF);
                            brush = new SolidBrush(HighlightTextColor);
                        }
                        else
                        {
                            brush = new SolidBrush(NameColor);
                        }

                        var text = MyItems[i].Name;
                        if ((Operators.CompareString(text, "", false) != 0) & !text.EndsWith(":")) text += ":";

                        bxBuffer.Graphics.DrawString(text, font, brush, rectangleF, stringFormat);
                        location = new PointF(num * num5 + num3 - 10, rectangleF.Height * num4 + checked(LinePadding * num4));
                        rectangleF.Location = location;
                        rectangleF.Width = num2 + 12;
                        if (Highlightable & (CurrentHighlight == i))
                        {
                            brush = new SolidBrush(HighlightColor);
                            bxBuffer.Graphics.FillRectangle(brush, rectangleF);
                            brush = new SolidBrush(HighlightTextColor);
                        }
                        else if (MyItems[i].UniqueColour)
                        {
                            brush = new SolidBrush(ValueColorD);
                        }
                        else if (MyItems[i].AlternateColour)
                        {
                            brush = new SolidBrush(ItemColorAlt);
                        }
                        else if (MyItems[i].ProbColour)
                        {
                            brush = new SolidBrush(ItemColorSpecial);
                        }
                        else
                        {
                            brush = new SolidBrush(ItemColor);
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
        
        private void ctlPairedList_Paint(object sender, PaintEventArgs e)
        {
            if (bxBuffer != null)
                myGFX.DrawImage(bxBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }
        
        protected override void OnFontChanged(EventArgs e)
        {
            SetLineHeight();
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
        
        private void SetLineHeight()
        {
            var font = new Font(Font, Font.Style);
            LineHeight = checked((int) Math.Round(font.GetHeight() + LinePadding));
        }
        
        public void AddItem(ItemPair iItem)
        {
            checked
            {
                MyItems = (ItemPair[]) Utils.CopyArray(MyItems, new ItemPair[MyItems.Length + 1]);
                MyItems[MyItems.Length - 1] = new ItemPair(iItem);
            }
        }
        
        public void Clear(bool Redraw = false)
        {
            MyItems = (ItemPair[]) Utils.CopyArray(MyItems, new ItemPair[0]);
            if (Redraw) Draw();
        }
        
        private void Listlabel_MouseDown(object sender, MouseEventArgs e)
        {
            var num = 0;
            var num2 = 0;
            var num3 = -1;
            Rectangle rectangle = default;
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Height = LineHeight;
            checked
            {
                rectangle.Width = (int) Math.Round(Width / (double) myColumns);
                var num4 = 0;
                var num5 = MyItems.Length - 1;
                for (var i = num4; i <= num5; i++)
                {
                    rectangle.X = rectangle.Width * num2;
                    rectangle.Y = (rectangle.Height + LinePadding) * num;
                    if ((e.Y >= rectangle.Y) & (e.Y <= rectangle.Height + rectangle.Y) &&
                        (e.X >= rectangle.X) & (e.X <= rectangle.Width + rectangle.X))
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
                var itemClickEvent = ItemClick;
                itemClickEvent?.Invoke(num3, e.Button);
            }
        }
        
        private void Listlabel_MouseMove(object sender, MouseEventArgs e)
        {
            var num = 0;
            var num2 = 0;
            var num3 = -1;
            Rectangle rectangle = default;
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Height = LineHeight;
            checked
            {
                rectangle.Width = (int) Math.Round(Width / (double) myColumns);
                var num4 = 0;
                var num5 = MyItems.Length - 1;
                for (var i = num4; i <= num5; i++)
                {
                    rectangle.X = rectangle.Width * num2;
                    rectangle.Y = (rectangle.Height + LinePadding) * num;
                    if ((e.Y >= rectangle.Y) & (e.Y <= rectangle.Height + rectangle.Y) &&
                        (e.X >= rectangle.X) & (e.X <= rectangle.Width + rectangle.X))
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
                if (Highlightable) Draw();

                if (num3 > -1) ItemHover?.Invoke(this, num3, MyItems[num3].TagID);
            }
        }
        
        private void Listlabel_MouseLeave(object sender, EventArgs e)
        {
            CurrentHighlight = -1;
            if (Highlightable) Draw();

            myTip.SetToolTip(this, "");
        }
        
        public void SetTip(string iTip)
        {
            myTip.SetToolTip(this, iTip);
        }
        
        private void ctlPairedList_BackColorChanged(object sender, EventArgs e)
        {
            Draw();
        }

        public class ItemPair
        {
            public readonly bool AlternateColour;
            public readonly bool ProbColour;
            private readonly string SpecialTip;
            public readonly bool VerySpecialColour;
            public string Name;
            public Enums.ShortFX TagID;
            public bool UniqueColour;
            public string Value;
            
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
            
            public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability = false, bool iSpecialCase = false, int iTagID = -1)
            {
                Name = iName;
                Value = iValue;
                AlternateColour = iAlternate;
                ProbColour = iProbability;
                VerySpecialColour = iSpecialCase;
                TagID.Add(iTagID, 0f);
                SpecialTip = "";
            }
            
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
        }
    }
}
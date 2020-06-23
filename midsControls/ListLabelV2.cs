using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Base.Display;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace midsControls
{
    public class ListLabelV2 : UserControl
    {
        public delegate void ItemClickEventHandler(ListLabelItemV2 Item, MouseButtons Button);

        public delegate void ItemHoverEventHandler(ListLabelItemV2 Item);

        public delegate void EmptyHoverEventHandler();

        public delegate void ExpandChangedEventHandler(bool Expanded);

        public event ItemClickEventHandler ItemClick;
        public event ItemHoverEventHandler ItemHover;
        public event EmptyHoverEventHandler EmptyHover;
        public event ExpandChangedEventHandler ExpandChanged;

        public bool isExpanded => Expanded;

        public Color TextColor
        {
            get
            {
                Color result;
                if (Item.ItemState < LLItemState.Enabled | Item.ItemState > LLItemState.Heading)
                {
                    result = Color.Black;
                }
                else
                {
                    result = Colours[(int) Item.ItemState];
                }

                return result;
            }
            set
            {
                if (Item.ItemState < LLItemState.Enabled | Item.ItemState > LLItemState.Heading)
                    return;
                Colours[(int) Item.ItemState] = value;
                Draw();
            }
        }

        public void UpdateTextColors(LLItemState State, Color color)
        {
            if (State < LLItemState.Enabled | State > LLItemState.Heading)
                return;
            Colours[(int) State] = color;
            Draw();
        }

        public ListLabelItemV2 Item
        {
            // ReSharper disable once FunctionRecursiveOnAllPaths
            get
            {
                if (Item.Index < 0 | Item.Index > checked(Items.Length - 1))
                    return new ListLabelItemV2();
                return Items[Item.Index];
            }
            set
            {
                if (Item.Index < 0 | Item.Index > checked(Items.Length - 1))
                    return;
                Items[Item.Index] = new ListLabelItemV2(value);
                Draw();
            }
        }

        public override Color BackColor
        {
            get => bgColor;
            set
            {
                bgColor = value;
                Draw();
            }
        }

        public Color HoverColor
        {
            get => hvrColor;
            set
            {
                hvrColor = value;
                Draw();
            }
        }

        public int PaddingX
        {
            get => xPadding;
            set
            {
                if (!(value >= 0 & checked(value * 2 < Width - 5)))
                    return;
                xPadding = value;
                Draw();
            }
        }

        public int PaddingY
        {
            get => yPadding;
            set
            {
                if (!(value >= 0 & value < Height / 3.0))
                    return;
                yPadding = value;
                SetLineHeight();
                Draw();
            }
        }

        public bool HighVis
        {
            get => TextOutline;
            set
            {
                TextOutline = value;
                Draw();
            }
        }

        public int HoverID => HoverIndex;

        public bool SuspendRedraw
        {
            get => DisableRedraw;
            set
            {
                DisableRedraw = value;
                if (value)
                    return;
                if (Expanded)
                {
                    RecomputeExpand();
                }

                if (Expanded)
                    return;
                Recalculate();
                Draw();
            }
        }

        public bool Scrollable
        {
            get => canScroll;
            set
            {
                canScroll = value;
                Draw();
            }
        }

        public bool Expandable
        {
            get => canExpand;
            set
            {
                canExpand = value;
                Draw();
            }
        }

        public Size SizeNormal
        {
            get => szNormal;
            set
            {
                szNormal = value;
                Expand(Expanded);
                Draw();
            }
        }

        // Token: 0x1700006F RID: 111
        // (get) Token: 0x060001EA RID: 490 RVA: 0x00011710 File Offset: 0x0000F910
        // (set) Token: 0x060001EB RID: 491 RVA: 0x00011728 File Offset: 0x0000F928
        public int MaxHeight
        {
            get => expandMaxY;
            set
            {
                if (value < szNormal.Height)
                    return;
                if (value > 2000)
                    return;
                expandMaxY = value;
                Draw();
            }
        }

        // Token: 0x17000070 RID: 112
        // (get) Token: 0x060001EC RID: 492 RVA: 0x00011770 File Offset: 0x0000F970
        // (set) Token: 0x060001ED RID: 493 RVA: 0x00011788 File Offset: 0x0000F988
        public int ScrollBarWidth
        {
            get => ScrollWidth;
            set
            {
                if (value > 0 & value < Width / 2.0)
                {
                    ScrollWidth = value;
                }

                Recalculate();
                Draw();
            }
        }

        // Token: 0x17000071 RID: 113
        // (get) Token: 0x060001EE RID: 494 RVA: 0x000117D0 File Offset: 0x0000F9D0
        // (set) Token: 0x060001EF RID: 495 RVA: 0x000117E8 File Offset: 0x0000F9E8
        public Color ScrollBarColor
        {
            get => scBarColor;
            set
            {
                scBarColor = value;
                Draw();
            }
        }

        // Token: 0x17000072 RID: 114
        // (get) Token: 0x060001F0 RID: 496 RVA: 0x000117FC File Offset: 0x0000F9FC
        // (set) Token: 0x060001F1 RID: 497 RVA: 0x00011814 File Offset: 0x0000FA14
        public Color ScrollButtonColor
        {
            get => scButtonColor;
            set
            {
                scButtonColor = value;
                Draw();
            }
        }

        // Token: 0x17000073 RID: 115
        // (get) Token: 0x060001F2 RID: 498 RVA: 0x00011828 File Offset: 0x0000FA28
        public int ContentHeight => Height;

        // Token: 0x17000074 RID: 116
        // (get) Token: 0x060001F3 RID: 499 RVA: 0x00011840 File Offset: 0x0000FA40
        public int DesiredHeight => checked(GetTotalLineCount() * LineHeight);

        // Token: 0x17000075 RID: 117
        // (get) Token: 0x060001F4 RID: 500 RVA: 0x00011860 File Offset: 0x0000FA60
        public int ActualLineHeight => LineHeight;

        // Token: 0x060001F5 RID: 501 RVA: 0x00011878 File Offset: 0x0000FA78
        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    Components?.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Token: 0x060001F6 RID: 502 RVA: 0x000118C8 File Offset: 0x0000FAC8
        [DebuggerStepThrough]
        void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Inherit;
            DoubleBuffered = true;
            Name = "ListLabelV2";
            Size size = new Size(150, 210);
            Size = size;
            ResumeLayout(false);
        }

        // Token: 0x060001F7 RID: 503 RVA: 0x0001191B File Offset: 0x0000FB1B
        void ListLabelV2_FontChanged(object sender, EventArgs e)
        {
            Recalculate();
            Draw();
        }

        // Token: 0x060001F8 RID: 504 RVA: 0x0001192D File Offset: 0x0000FB2D
        void ListLabelV2_Load(object sender, EventArgs e)
        {
            szNormal = Size;
            DisableRedraw = true;
            InitBuffer();
            Recalculate();
            FillDefaultItems();
            DisableRedraw = false;
        }

        // Token: 0x060001F9 RID: 505 RVA: 0x00011960 File Offset: 0x0000FB60
        void FillDefaultItems()
        {
            ClearItems();
            // Bunch o' items that will never be shown
            // Except maybe if you manage to go under the map.
            /*
            AddItem(new ListLabelItemV2("Header Item", LLItemState.Heading, -1, -1, -1, "", LLFontFlags.Bold, LLTextAlign.Center));
            AddItem(new ListLabelItemV2("Enabled", LLItemState.Enabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Disabled Item", LLItemState.Disabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Selected Item", LLItemState.Selected, -1, -1, -1, "", LLFontFlags.Bold | LLFontFlags.Italic));
            AddItem(new ListLabelItemV2("SD Item", LLItemState.SelectedDisabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Invalid Item", LLItemState.Invalid, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Automatic multiline Item", LLItemState.Enabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Scrollable", LLItemState.Heading, -1, -1, -1, "", LLFontFlags.Bold, LLTextAlign.Center));
            AddItem(new ListLabelItemV2("Item 1", LLItemState.Enabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 2", LLItemState.Selected, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 3", LLItemState.Selected, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 4", LLItemState.Selected, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 5", LLItemState.Disabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 6", LLItemState.Enabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 7", LLItemState.Enabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 8", LLItemState.Selected, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 9", LLItemState.Enabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 10", LLItemState.Disabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 11", LLItemState.Selected, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 12", LLItemState.Selected, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 13", LLItemState.Invalid, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 14", LLItemState.Enabled, -1, -1, -1, "", LLFontFlags.Bold));
            AddItem(new ListLabelItemV2("Item 15", LLItemState.Enabled, -1, -1, -1, "", LLFontFlags.Bold));
            */
            Draw();
        }

        // Token: 0x060001FA RID: 506 RVA: 0x00011C00 File Offset: 0x0000FE00
        public void AddItem(ListLabelItemV2 iItem)
        {
            DisableEvents = true;
            int num = Items.Length;
            Items = (ListLabelItemV2[]) Utils.CopyArray(Items, new ListLabelItemV2[checked(num + 1)]);
            Items[num] = iItem;
            Items[num].Index = num;
            WrapString(num);
            GetScrollSteps();
            DisableEvents = false;
        }

        // Token: 0x060001FB RID: 507 RVA: 0x00011C69 File Offset: 0x0000FE69
        public void ClearItems()
        {
            Items = new ListLabelItemV2[0];
            ScrollOffset = 0;
            HoverIndex = -1;
        }

        void SetLineHeight()
        {
            Font font = new Font(Font, Font.Style);
            LineHeight = checked((int) Math.Round(font.GetHeight() + checked(yPadding * 2)));
            VisibleLineCount = GetVisibleLineCount();
        }

        void Recalculate(bool expanded = false)
        {
            checked
            {
                if (Items.Length == 0)
                    return;
                InitBuffer();
                if (AutoSize)
                {
                    if (AutoSizeMode == AutoSizeMode.GrowAndShrink)
                    {
                        Height = DesiredHeight;
                    }
                    else if (DesiredHeight > SizeNormal.Height)
                    {
                        Height = DesiredHeight;
                    }
                    else
                    {
                        Height = SizeNormal.Height;
                    }
                }

                Rectangle bRect = new Rectangle(xPadding, 0, Width - xPadding * 2, Height);
                RecalcLines(bRect);
                if (ScrollSteps > 0 | Expanded)
                {
                    bRect = new Rectangle(xPadding, 0, Width - xPadding * 2, Height - (ScrollWidth + yPadding));
                    RecalcLines(bRect);
                }

                if (expanded || ScrollSteps <= 0)
                    return;
                int num = 0;
                if (canExpand)
                {
                    num = ScrollWidth + yPadding;
                }

                bRect = new Rectangle(xPadding, 0, Width - (xPadding * 2 + ScrollWidth), Height - num);
                RecalcLines(bRect);
            }
        }

        void RecalcLines(Rectangle bRect)
        {
            TextArea = bRect;
            SetLineHeight();
            int num = 0;
            checked
            {
                int num2 = Items.Length - 1;
                for (int i = num; i <= num2; i++)
                {
                    WrapString(i);
                }

                GetTotalLineCount();
                GetScrollSteps();
            }
        }

        void WrapString(int Index)
        {
            checked
            {
                if (Operators.CompareString(Items[Index].Text, "", false) == 0)
                {
                    return;
                }
                InitBuffer();
                int num = 1;
                if (Items[Index].Text.Contains(" "))
                {
                    string[] array = Items[Index].Text.Split(" ".ToCharArray());
                    StringFormat stringFormat = new StringFormat(StringFormatFlags.NoWrap);
                    Font font = new Font(Font, (FontStyle)Items[Index].FontFlags);
                    string str = "";
                    if (Items[Index].ItemState == LLItemState.Heading)
                    {
                        str = "~  ~";
                    }

                    string text = array[0];
                    int num2 = 1;
                    int num3 = array.Length - 1;
                    for (int i = num2; i <= num3; i++)
                    {
                        Graphics graphics = bxBuffer.Graphics;
                        string text2 = text + " " + array[i] + str;
                        Font font2 = font;
                        SizeF layoutArea = new SizeF(1024f, Height);
                        if (Math.Ceiling(graphics.MeasureString(text2, font2, layoutArea, stringFormat).Width) > TextArea.Width)
                        {
                            if (Items[Index].ItemState == LLItemState.Heading)
                            {
                                text = text + " ~\r\n~ " + array[i];
                            }
                            else
                            {
                                text = text + "\r\n " + array[i];
                            }

                            num++;
                        }
                        else
                        {
                            text = text + " " + array[i];
                        }
                    }
                    Items[Index].WrappedText = text;
                }
                else
                {
                    Items[Index].WrappedText = Items[Index].Text;
                }

                if (Items[Index].ItemState == LLItemState.Heading)
                {
                    Items[Index].WrappedText = "~ " + Items[Index].WrappedText + " ~";
                }

                Items[Index].LineCount = num;
                Items[Index].ItemHeight = num * (LineHeight - yPadding * 2) + yPadding * 2;
            }
        }

        void InitBuffer()
        {
            if (DisableRedraw)
                return;
            if ((Width == 0 | Height == 0) & bxBuffer == null)
                return;
            if (bxBuffer == null)
            {
                bxBuffer = new ExtendedBitmap(Width, Height);
            }

            if (bxBuffer.Size.Width != Width | bxBuffer.Size.Height != Height)
            {
                bxBuffer.Dispose();
                bxBuffer = null;
                GC.Collect();
                if (Height == 0 | Width == 0)
                {
                    return;
                }

                bxBuffer = new ExtendedBitmap(Width, Height);
            }

            bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            bxBuffer.Graphics.TextContrast = 0;
        }

        int GetRealTotalHeight()
        {
            int num = 0;
            int num2 = 0;
            checked
            {
                int num3 = Items.Length - 1;
                for (int i = num2; i <= num3; i++)
                {
                    num += Items[i].ItemHeight;
                }

                return num;
            }
        }

        void Draw()
        {
            checked
            {
                if (DisableRedraw)
                    return;
                if (!Visible)
                    return;
                InitBuffer();
                if (Width == 0 | Height == 0)
                    return;
                bxBuffer.Graphics.Clear(Expanded ? Color.Black : BackColor);
                int scrollOffset = ScrollOffset;
                int num = Items.Length - 1;
                for (int i = scrollOffset; i <= num; i++)
                {
                    DrawItem(i);
                }

                DrawScrollBar();
                DrawExpandButton();
                Graphics graphics = CreateGraphics();
                graphics.DrawImageUnscaled(bxBuffer.Bitmap, 0, 0);
            }
        }

        protected void DrawItem(int Index)
        {
            checked
            {
                if (Index < 0)
                    return;
                if (Index < ScrollOffset)
                    return;
                if (Index > Items.Length - 1)
                    return;
                int num = 0;
                int scrollOffset = ScrollOffset;
                int num2 = Index - 1;
                for (int i = scrollOffset; i <= num2; i++)
                {
                    num += Items[i].ItemHeight;
                    if (num > Height)
                    {
                        return;
                    }
                }

                int height = Items[Index].ItemHeight;
                if (Items[Index].LineCount == 1)
                {
                    if (num + Items[Index].ItemHeight > TextArea.Height)
                    {
                        return;
                    }
                }
                else if (num + Items[Index].ItemHeight > TextArea.Height)
                {
                    if (num + LineHeight > TextArea.Height)
                    {
                        return;
                    }

                    height = LineHeight - yPadding;
                }

                Rectangle rectangle = new Rectangle(TextArea.Left, num, TextArea.Width, height);
                StringFormat stringFormat = new StringFormat();
                switch (Items[Index].TextAlign)
                {
                    case LLTextAlign.Left:
                        stringFormat.Alignment = StringAlignment.Near;
                        break;
                    case LLTextAlign.Center:
                        stringFormat.Alignment = StringAlignment.Center;
                        break;
                    case LLTextAlign.Right:
                        stringFormat.Alignment = StringAlignment.Far;
                        break;
                }

                FontStyle fontStyle = FontStyle.Regular;
                if (Items[Index].Bold)
                {
                    fontStyle++;
                }

                if (Items[Index].Italic)
                {
                    fontStyle += 2;
                }

                if (Items[Index].Underline)
                {
                    fontStyle += 4;
                }

                if (Items[Index].Strikethrough)
                {
                    fontStyle += 8;
                }

                Font font = new Font(Font, fontStyle);
                if (Index == HoverID && HighlightOn[(int) Items[Index].ItemState])
                {
                    SolidBrush brush = new SolidBrush(hvrColor);
                    bxBuffer.Graphics.FillRectangle(brush, rectangle);
                }

                SolidBrush brush2 = new SolidBrush(Color.Black);
                if (TextOutline)
                {
                    Rectangle r = rectangle;
                    r.X--;
                    bxBuffer.Graphics.DrawString(Items[Index].WrappedText, font, brush2, r, stringFormat);
                    r.Y--;
                    bxBuffer.Graphics.DrawString(Items[Index].WrappedText, font, brush2, r, stringFormat);
                    r.X++;
                    bxBuffer.Graphics.DrawString(Items[Index].WrappedText, font, brush2, r, stringFormat);
                    r.X++;
                    bxBuffer.Graphics.DrawString(Items[Index].WrappedText, font, brush2, r, stringFormat);
                    r.Y++;
                    bxBuffer.Graphics.DrawString(Items[Index].WrappedText, font, brush2, r, stringFormat);
                    r.Y++;
                    bxBuffer.Graphics.DrawString(Items[Index].WrappedText, font, brush2, r, stringFormat);
                    r.X--;
                    bxBuffer.Graphics.DrawString(Items[Index].WrappedText, font, brush2, r, stringFormat);
                    r.X--;
                    bxBuffer.Graphics.DrawString(Items[Index].WrappedText, font, brush2, r, stringFormat);
                }

                brush2 = new SolidBrush(Colours[(int) Items[Index].ItemState]);
                bxBuffer.Graphics.DrawString(Items[Index].WrappedText, font, brush2, rectangle, stringFormat);
            }
        }

        int GetVisibleLineCount()
        {
            int result;
            if (!canScroll)
            {
                ScrollSteps = 0;
                result = 0;
            }
            else if (Expanded)
            {
                result = GetTotalLineCount();
            }
            else
            {
                result = checked((int) Math.Round(Conversion.Int(TextArea.Height / (double) LineHeight)));
            }

            return result;
        }

        int GetTotalLineCount()
        {
            int num = 0;
            int num2 = 0;
            checked
            {
                int num3 = Items.Length - 1;
                for (int i = num2; i <= num3; i++)
                {
                    num += Items[i].LineCount;
                }

                return num;
            }
        }

        int GetScrollSteps()
        {
            checked
            {
                if (!canScroll)
                {
                    ScrollSteps = 0;
                    return 0;
                }

                int num = 0;
                int wrapCount = 0;
                int num3 = 0;
                int num4 = Items.Length - 1;
                for (int i = num3; i <= num4; i++)
                {
                    num += Items[i].LineCount;
                    if (num > VisibleLineCount)
                    {
                        wrapCount++;
                    }
                }

                if (wrapCount > 0)
                {
                    wrapCount++;
                }

                ScrollSteps = wrapCount;
                if (ScrollSteps <= 1)
                {
                    ScrollSteps = 0;
                }

                return ScrollSteps;
            }
        }

        void DrawScrollBar()
        {
            if (ScrollWidth < 1 | !canScroll | ScrollSteps < 1)
                return;
            SolidBrush brush;
            Pen pen = new Pen(scBarColor);
            Pen pen2 = new Pen(Color.FromArgb(96, 255, 255, 255), 1f);
            Pen pen3 = new Pen(Color.FromArgb(128, 0, 0, 0), 1f);
            PointF[] array;
            Rectangle rect;
            checked
            {
                bxBuffer.Graphics.DrawLine(pen, (int) Math.Round(TextArea.Right + ScrollWidth / 2.0), yPadding + ScrollWidth,
                    (int) Math.Round(TextArea.Right + ScrollWidth / 2.0), Height - (ScrollWidth + yPadding));
                brush = new SolidBrush(scButtonColor);
                array = new PointF[3];
                if (ScrollSteps > 0)
                {
                    int num = Height - (yPadding + ScrollWidth) * 2 - yPadding * 2;
                    int y = (int) Math.Round(checked(ScrollWidth + yPadding * 2) + num / (double) ScrollSteps * ScrollOffset);
                    int height = (int) Math.Round(num / (double) ScrollSteps);
                    rect = new Rectangle(TextArea.Right, y, ScrollWidth, height);
                    bxBuffer.Graphics.FillRectangle(brush, rect);
                    bxBuffer.Graphics.DrawLine(pen2, rect.Left, rect.Top, rect.Left, rect.Bottom);
                    bxBuffer.Graphics.DrawLine(pen2, rect.Left + 1, rect.Top, rect.Right, rect.Top);
                    bxBuffer.Graphics.DrawLine(pen3, rect.Right, rect.Top, rect.Right, rect.Bottom);
                    bxBuffer.Graphics.DrawLine(pen3, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
                }

                rect = new Rectangle(TextArea.Right, yPadding + ScrollWidth, ScrollWidth, Height - (ScrollWidth + yPadding) * 2);
                array[0] = new PointF(rect.Left, rect.Top);
                array[1] = new PointF(rect.Right, rect.Top);
            }

            array[2] = new PointF(rect.Left + rect.Width / 2f, yPadding);
            bxBuffer.Graphics.FillPolygon(brush, array);
            bxBuffer.Graphics.DrawLine(pen2, array[0], array[2]);
            bxBuffer.Graphics.DrawLine(pen3, array[0], array[1]);
            array[0] = new PointF(rect.Left, rect.Bottom);
            array[1] = new PointF(rect.Right, rect.Bottom);
            array[2] = new PointF(rect.Left + rect.Width / 2f, checked(Height - yPadding));
            bxBuffer.Graphics.FillPolygon(brush, array);
            bxBuffer.Graphics.DrawLine(pen2, array[0], array[1]);
            bxBuffer.Graphics.DrawLine(pen3, array[2], array[1]);
        }

        void DrawExpandButton()
        {
            if (!canExpand | (!Expanded & ScrollSteps < 1))
                return;
            Pen pen = new Pen(scBarColor);
            Pen pen2 = new Pen(Color.FromArgb(96, 255, 255, 255), 1f);
            Pen pen3 = new Pen(Color.FromArgb(128, 0, 0, 0), 1f);
            var brush = new SolidBrush(scButtonColor);
            PointF[] array = new PointF[3];
            Rectangle rectangle = checked(new Rectangle((int) Math.Round(Width / 3.0), Height - (ScrollWidth + yPadding),
                (int) Math.Round(Width / 3.0), ScrollWidth - yPadding));
            if (Expanded)
            {
                array[0] = new PointF(rectangle.Left, rectangle.Bottom);
                array[1] = new PointF(rectangle.Right, rectangle.Bottom);
                array[2] = new PointF(rectangle.Left + rectangle.Width / 2f, rectangle.Top);
                bxBuffer.Graphics.FillPolygon(brush, array);
                bxBuffer.Graphics.DrawLine(pen2, array[0], array[2]);
                bxBuffer.Graphics.DrawLine(pen3, array[0], array[1]);
                checked
                {
                    bxBuffer.Graphics.DrawLine(pen, 0, 0, 0, Height - 1);
                    bxBuffer.Graphics.DrawLine(pen, 0, Height - 1, Width - 1, Height - 1);
                    bxBuffer.Graphics.DrawLine(pen, Width - 1, Height, Width - 1, 0);
                }
            }
            else
            {
                array[0] = new PointF(rectangle.Left, rectangle.Top);
                array[1] = new PointF(rectangle.Right, rectangle.Top);
                array[2] = new PointF(rectangle.Left + rectangle.Width / 2f, rectangle.Bottom);
                bxBuffer.Graphics.FillPolygon(brush, array);
                bxBuffer.Graphics.DrawLine(pen2, array[0], array[1]);
                bxBuffer.Graphics.DrawLine(pen3, array[2], array[1]);
            }
        }

        int GetItemAtY(int Y)
        {
            int num = 0;
            checked
            {
                int result;
                if (Y > Height)
                {
                    result = -1;
                }
                else
                {
                    int scrollOffset = ScrollOffset;
                    int num2 = Items.Length - 1;
                    for (int i = scrollOffset; i <= num2; i++)
                    {
                        num += Items[i].ItemHeight;
                        if (Y < num)
                        {
                            return i;
                        }
                    }

                    result = -1;
                }

                return result;
            }
        }

        // Token: 0x0600020A RID: 522 RVA: 0x00013248 File Offset: 0x00011448
        eMouseTarget GetMouseTarget(int X, int Y)
        {
            checked
            {
                eMouseTarget result;
                if (X >= TextArea.Left & X <= TextArea.Right & Y <= TextArea.Bottom)
                {
                    result = eMouseTarget.Item;
                }
                else if (X >= TextArea.Left & X <= TextArea.Right & Y > TextArea.Bottom)
                {
                    result = eMouseTarget.ExpandButton;
                }
                else if (X > TextArea.Right & Y <= ScrollWidth + yPadding)
                {
                    result = eMouseTarget.UpButton;
                }
                else if (X > TextArea.Right & Y >= Height - (ScrollWidth + yPadding))
                {
                    result = eMouseTarget.DownButton;
                }
                else if (!(X > TextArea.Right & ScrollSteps > 0))
                {
                    result = eMouseTarget.None;
                }
                else
                {
                    int num = Height - (yPadding + ScrollWidth) * 2 - yPadding * 2;
                    int num2 = (int) Math.Round(checked(ScrollWidth + yPadding * 2) + num / (double) ScrollSteps * ScrollOffset);
                    int num3 = (int) Math.Round(num / (double) ScrollSteps);
                    if (Y < num2)
                    {
                        result = eMouseTarget.ScrollBarUp;
                    }
                    else if (Y > num2 + num3)
                    {
                        result = eMouseTarget.ScrollBarDown;
                    }
                    else
                    {
                        result = eMouseTarget.ScrollBlock;
                    }
                }

                return result;
            }
        }

        // Token: 0x0600020B RID: 523 RVA: 0x000133FC File Offset: 0x000115FC
        void ListLabelV2_MouseDown(object sender, MouseEventArgs e)
        {
            checked
            {
                if (e.Button == MouseButtons.Left & ModifierKeys == (Keys.Shift | Keys.Control | Keys.Alt))
                {
                    DisableEvents = false;
                    DisableRedraw = false;
                    Recalculate();
                    Draw();
                    Graphics graphics = CreateGraphics();
                    Pen powderBlue = Pens.PowderBlue;
                    Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                    graphics.DrawRectangle(powderBlue, rect);
                }
                else if (!DisableEvents)
                {
                    int num;
                    if (ScrollSteps / 3.0 < 5.0)
                    {
                        num = (int) Math.Round(ScrollSteps / 3.0);
                    }
                    else
                    {
                        num = 5;
                    }

                    switch (GetMouseTarget(e.X, e.Y))
                    {
                        case eMouseTarget.Item:
                        {
                            int itemAtY = GetItemAtY(e.Y);
                            if (itemAtY > -1)
                            {
                                int num2 = 0;
                                int scrollOffset = ScrollOffset;
                                int num3 = itemAtY - 1;
                                for (int i = scrollOffset; i <= num3; i++)
                                {
                                    num2 += Items[i].ItemHeight;
                                }

                                if ((num2 + Items[itemAtY].ItemHeight >= e.Y & num2 + Items[itemAtY].ItemHeight <= TextArea.Height) |
                                    (Items[itemAtY].LineCount > 1 & num2 + LineHeight >= e.Y & num2 + LineHeight <= TextArea.Height))
                                {
                                    ItemClick?.Invoke(Items[itemAtY], e.Button);
                                }
                            }

                            break;
                        }
                        case eMouseTarget.UpButton:
                            if (ScrollSteps > 0 & ScrollOffset > 0)
                            {
                                ScrollOffset--;
                                Draw();
                            }

                            break;
                        case eMouseTarget.DownButton:
                            if (ScrollSteps > 0 & ScrollOffset + 1 < ScrollSteps)
                            {
                                ScrollOffset++;
                                Draw();
                            }

                            break;
                        case eMouseTarget.ScrollBarUp:
                            if (ScrollSteps > 0)
                            {
                                ScrollOffset -= num;
                                if (ScrollOffset < 0)
                                {
                                    ScrollOffset = 0;
                                }

                                Draw();
                            }

                            break;
                        case eMouseTarget.ScrollBarDown:
                            if (ScrollSteps > 0)
                            {
                                ScrollOffset += num;
                                if (ScrollOffset >= ScrollSteps)
                                {
                                    ScrollOffset = ScrollSteps - 1;
                                }

                                Draw();
                            }

                            break;
                        case eMouseTarget.ScrollBlock:
                            if (ScrollSteps > 0)
                            {
                                DragMode = true;
                            }

                            break;
                        case eMouseTarget.ExpandButton:
                        {
                            if (!Expanded)
                            {
                                Expanded = true;
                                ScrollOffset = 0;
                                RecomputeExpand();
                            }
                            else
                            {
                                DisableRedraw = true;
                                Height = szNormal.Height;
                                Expanded = false;
                                Recalculate();
                                DisableRedraw = false;
                                Draw();
                            }

                            ExpandChanged?.Invoke(Expanded);
                            break;
                        }
                    }
                }
            }
        }

        // Token: 0x0600020C RID: 524 RVA: 0x000137F0 File Offset: 0x000119F0
        void Expand(bool State)
        {
            if (State)
            {
                Expanded = true;
                ScrollOffset = 0;
                RecomputeExpand();
            }
            else
            {
                DisableRedraw = true;
                Height = szNormal.Height;
                Expanded = false;
                Recalculate();
                DisableRedraw = false;
                Draw();
            }

            ExpandChanged?.Invoke(Expanded);
        }

        // Token: 0x0600020D RID: 525 RVA: 0x00013878 File Offset: 0x00011A78
        void RecomputeExpand()
        {
            if (!Expanded)
                return;
            int num = expandMaxY;
            Recalculate(true);
            int num2 = checked(GetRealTotalHeight() + ScrollWidth + yPadding * 3);
            if (num2 < num)
            {
                num = num2;
            }

            DisableRedraw = true;
            Height = num;
            Recalculate();
            DisableRedraw = false;
            Draw();
        }

        // Token: 0x0600020E RID: 526 RVA: 0x000138F0 File Offset: 0x00011AF0
        void ListLabelV2_MouseLeave(object sender, EventArgs e)
        {
            Cursor = System.Windows.Forms.Cursors.Default;
            LastMouseMovetarget = eMouseTarget.None;
            HoverIndex = -1;
            Draw();
            EmptyHover?.Invoke();
        }

        // Token: 0x0600020F RID: 527 RVA: 0x00013938 File Offset: 0x00011B38
        void ListLabelV2_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor cursor = System.Windows.Forms.Cursors.Default;
            eMouseTarget mouseTarget = GetMouseTarget(e.X, e.Y);
            bool flag = false;
            checked
            {
                if (!DragMode)
                {
                    EmptyHoverEventHandler emptyHoverEvent;
                    switch (mouseTarget)
                    {
                        case eMouseTarget.Item:
                        {
                            int itemAtY = GetItemAtY(e.Y);
                            if (itemAtY <= -1)
                            {
                                if (HoverIndex != -1)
                                {
                                    flag = true;
                                }

                                HoverIndex = -1;
                                EmptyHover?.Invoke();
                                goto IL_3EA;
                            }

                            int num = 0;
                            int scrollOffset = ScrollOffset;
                            int num2 = itemAtY - 1;
                            for (int i = scrollOffset; i <= num2; i++)
                            {
                                num += Items[i].ItemHeight;
                            }

                            if (!((num + Items[itemAtY].ItemHeight >= e.Y & num + Items[itemAtY].ItemHeight <= TextArea.Height) |
                                  (Items[itemAtY].LineCount > 1 & num + LineHeight >= e.Y & num + LineHeight <= TextArea.Height)))
                            {
                                if (HoverIndex != -1)
                                {
                                    flag = true;
                                }

                                HoverIndex = -1;
                                EmptyHover?.Invoke();
                                goto IL_3EA;
                            }

                            cursor = Cursors[(int) Items[itemAtY].ItemState];
                            HoverIndex = itemAtY;
                            Draw();
                            ItemHover?.Invoke(Items[itemAtY]);
                            goto IL_3EA;
                        }
                        case eMouseTarget.UpButton:
                            if (LastMouseMovetarget != mouseTarget)
                            {
                                Draw();
                            }

                            emptyHoverEvent = EmptyHover;
                            emptyHoverEvent?.Invoke();
                            goto IL_3EA;
                        case eMouseTarget.DownButton:
                            if (LastMouseMovetarget != mouseTarget)
                            {
                                Draw();
                            }

                            emptyHoverEvent = EmptyHover;
                            emptyHoverEvent?.Invoke();
                            goto IL_3EA;
                        case eMouseTarget.ExpandButton:
                            HoverIndex = -1;
                            emptyHoverEvent = EmptyHover;
                            emptyHoverEvent?.Invoke();
                            goto IL_3EA;
                    }

                    emptyHoverEvent = EmptyHover;
                    emptyHoverEvent?.Invoke();
                }
                else if (e.Button == MouseButtons.None)
                {
                    DragMode = false;
                }
                else
                {
                    cursor = System.Windows.Forms.Cursors.SizeNS;
                    int num3 = Height - (yPadding + ScrollWidth) * 2 - yPadding * 2;
                    int num4 = (int) Math.Round(checked(ScrollWidth + yPadding * 2) + num3 / (double) ScrollSteps * ScrollOffset);
                    int num5 = (int) Math.Round(num3 / (double) ScrollSteps);
                    if (e.Y < num4 & ScrollOffset > 0)
                    {
                        ScrollOffset--;
                        Draw();
                    }
                    else if (e.Y > num4 + num5 & ScrollOffset + 1 < ScrollSteps)
                    {
                        ScrollOffset++;
                        Draw();
                    }

                    EmptyHoverEventHandler emptyHoverEvent = EmptyHover;
                    emptyHoverEvent?.Invoke();
                }

                IL_3EA:
                if (flag)
                {
                    Draw();
                }

                Cursor = cursor;
                LastMouseMovetarget = mouseTarget;
            }
        }

        // Token: 0x06000210 RID: 528 RVA: 0x00013D54 File Offset: 0x00011F54
        void ListLabelV2_MouseUp(object sender, MouseEventArgs e)
        {
            DragMode = false;
            if (Cursor == System.Windows.Forms.Cursors.SizeNS)
            {
                Cursor = System.Windows.Forms.Cursors.Default;
            }
        }

        // Token: 0x06000211 RID: 529 RVA: 0x00013D90 File Offset: 0x00011F90
        void ListLabelV2_Paint(object sender, PaintEventArgs e)
        {
            if (bxBuffer == null)
            {
                Draw();
            }

            if (bxBuffer != null)
            {
                e.Graphics.DrawImage(bxBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            }
        }

        // Token: 0x06000212 RID: 530 RVA: 0x00013DEE File Offset: 0x00011FEE
        void ListLabelV2_Resize(object sender, EventArgs e)
        {
            ScrollOffset = 0;
            Recalculate();
            Draw();
        }

        // Token: 0x06000213 RID: 531 RVA: 0x00013E10 File Offset: 0x00012010
        public ListLabelV2()
        {
            MouseLeave += ListLabelV2_MouseLeave;
            MouseMove += ListLabelV2_MouseMove;
            MouseUp += ListLabelV2_MouseUp;
            Paint += ListLabelV2_Paint;
            Resize += ListLabelV2_Resize;
            FontChanged += ListLabelV2_FontChanged;
            Load += ListLabelV2_Load;
            MouseDown += ListLabelV2_MouseDown;
            bxBuffer = null;
            Items = new ListLabelItemV2[0];
            Colours = new[]
            {
                Color.LightBlue, Color.LightGreen, Color.LightGray, Color.DarkGreen, Color.Red, Color.Orange
            };
            Cursors = new[]
            {
                System.Windows.Forms.Cursors.Hand, System.Windows.Forms.Cursors.Hand, System.Windows.Forms.Cursors.Default,
                System.Windows.Forms.Cursors.Hand, System.Windows.Forms.Cursors.Hand, System.Windows.Forms.Cursors.Default
            };
            HighlightOn = new[]
            {
                true, true, true, true, true, true, false
            };
            bgColor = Color.Black;
            hvrColor = Color.WhiteSmoke;
            TextOutline = true;
            xPadding = 1;
            yPadding = 1;
            LineHeight = 8;
            HoverIndex = -1;
            DisableRedraw = true;
            DisableEvents = false;
            canScroll = true;
            ScrollOffset = 0;
            canExpand = false;
            Expanded = false;
            szNormal = Size;
            expandMaxY = 400;
            TextArea = new Rectangle(0, 0, Width, Height);
            ScrollWidth = 8;
            scBarColor = Color.FromArgb(128, 96, 192);
            scButtonColor = Color.FromArgb(96, 0, 192);
            ScrollSteps = 0;
            DragMode = false;
            LastMouseMovetarget = eMouseTarget.None;
            VisibleLineCount = 0;
            InitializeComponent();
        }

        // Token: 0x040000D7 RID: 215
        public IContainer Components { get; }

        // Token: 0x040000D8 RID: 216
        ExtendedBitmap bxBuffer;

        // Token: 0x040000D9 RID: 217
        public ListLabelItemV2[] Items;

        // Token: 0x040000DA RID: 218
        protected Color[] Colours;

        // Token: 0x040000DB RID: 219
        protected Cursor[] Cursors;

        // Token: 0x040000DC RID: 220
        protected bool[] HighlightOn;

        // Token: 0x040000DD RID: 221
        protected Color bgColor;

        // Token: 0x040000DE RID: 222
        protected Color hvrColor;

        // Token: 0x040000DF RID: 223
        protected bool TextOutline;

        // Token: 0x040000E0 RID: 224
        protected int xPadding;

        // Token: 0x040000E1 RID: 225
        protected int yPadding;

        // Token: 0x040000E2 RID: 226
        protected int LineHeight;

        // Token: 0x040000E3 RID: 227
        protected int HoverIndex;

        // Token: 0x040000E4 RID: 228
        protected bool DisableRedraw;

        // Token: 0x040000E5 RID: 229
        protected bool DisableEvents;

        // Token: 0x040000E6 RID: 230
        protected bool canScroll;

        // Token: 0x040000E7 RID: 231
        protected int ScrollOffset;

        // Token: 0x040000E8 RID: 232
        protected bool canExpand;

        // Token: 0x040000E9 RID: 233
        protected bool Expanded;

        // Token: 0x040000EA RID: 234
        protected Size szNormal;

        // Token: 0x040000EB RID: 235
        protected int expandMaxY;

        // Token: 0x040000EC RID: 236
        protected Rectangle TextArea;

        // Token: 0x040000ED RID: 237
        protected int ScrollWidth;

        // Token: 0x040000EE RID: 238
        protected Color scBarColor;

        // Token: 0x040000EF RID: 239
        protected Color scButtonColor;

        // Token: 0x040000F0 RID: 240
        protected int ScrollSteps;

        // Token: 0x040000F1 RID: 241
        protected bool DragMode;

        // Token: 0x040000F2 RID: 242
        eMouseTarget LastMouseMovetarget;

        // Token: 0x040000F3 RID: 243
        int VisibleLineCount;

        // Token: 0x0200001F RID: 31
        public enum LLItemState
        {
            // Token: 0x040000F9 RID: 249
            Enabled,

            // Token: 0x040000FA RID: 250
            Selected,

            // Token: 0x040000FB RID: 251
            Disabled,

            // Token: 0x040000FC RID: 252
            SelectedDisabled,

            // Token: 0x040000FD RID: 253
            Invalid,

            // Token: 0x040000FE RID: 254
            Heading
        }

        // Token: 0x02000020 RID: 32
        [Flags]
        public enum LLFontFlags
        {
            // Token: 0x04000100 RID: 256
            Normal = 0,

            // Token: 0x04000101 RID: 257
            Bold = 1,

            // Token: 0x04000102 RID: 258
            Italic = 2,

            // Token: 0x04000103 RID: 259
            Underline = 4,

            // Token: 0x04000104 RID: 260
            Strikethrough = 8
        }

        // Token: 0x02000021 RID: 33
        public enum LLTextAlign
        {
            // Token: 0x04000106 RID: 262
            Left,

            // Token: 0x04000107 RID: 263
            Center,

            // Token: 0x04000108 RID: 264
            Right
        }

        // Token: 0x02000022 RID: 34
        enum eMouseTarget
        {
            // Token: 0x0400010A RID: 266
            None,

            // Token: 0x0400010B RID: 267
            Item,

            // Token: 0x0400010C RID: 268
            UpButton,

            // Token: 0x0400010D RID: 269
            DownButton,

            // Token: 0x0400010E RID: 270
            ScrollBarUp,

            // Token: 0x0400010F RID: 271
            ScrollBarDown,

            // Token: 0x04000110 RID: 272
            ScrollBlock,

            // Token: 0x04000111 RID: 273
            ExpandButton
        }

        // Token: 0x02000023 RID: 35
        public class ListLabelItemV2
        {
            // Token: 0x17000076 RID: 118
            // (get) Token: 0x06000214 RID: 532 RVA: 0x00014084 File Offset: 0x00012284
            // (set) Token: 0x06000215 RID: 533 RVA: 0x0001409C File Offset: 0x0001229C
            public string Text
            {
                get => Txt;
                set => Txt = value;
            }

            // Token: 0x17000077 RID: 119
            // (get) Token: 0x06000216 RID: 534 RVA: 0x000140A8 File Offset: 0x000122A8
            // (set) Token: 0x06000217 RID: 535 RVA: 0x000140C0 File Offset: 0x000122C0
            public LLItemState ItemState
            {
                get => State;
                set => State = value;
            }

            // Token: 0x17000078 RID: 120
            // (get) Token: 0x06000218 RID: 536 RVA: 0x000140CC File Offset: 0x000122CC
            // (set) Token: 0x06000219 RID: 537 RVA: 0x000140EC File Offset: 0x000122EC
            public bool Bold
            {
                get => (FontFlags & LLFontFlags.Bold) > LLFontFlags.Normal;
                set
                {
                    checked
                    {
                        if (value)
                        {
                            if ((~FontFlags & LLFontFlags.Bold) > LLFontFlags.Normal)
                            {
                                FontFlags++;
                            }
                        }
                        else if ((FontFlags & LLFontFlags.Bold) > LLFontFlags.Normal)
                        {
                            FontFlags--;
                        }
                    }
                }
            }

            // Token: 0x17000079 RID: 121
            // (get) Token: 0x0600021A RID: 538 RVA: 0x00014150 File Offset: 0x00012350
            // (set) Token: 0x0600021B RID: 539 RVA: 0x00014170 File Offset: 0x00012370
            public bool Italic
            {
                get => (FontFlags & LLFontFlags.Italic) > LLFontFlags.Normal;
                set
                {
                    checked
                    {
                        if (value)
                        {
                            if ((~FontFlags & LLFontFlags.Italic) > LLFontFlags.Normal)
                            {
                                FontFlags += 2;
                            }
                        }
                        else if ((FontFlags & LLFontFlags.Italic) > LLFontFlags.Normal)
                        {
                            FontFlags -= 2;
                        }
                    }
                }
            }

            // Token: 0x1700007A RID: 122
            // (get) Token: 0x0600021C RID: 540 RVA: 0x000141D4 File Offset: 0x000123D4
            // (set) Token: 0x0600021D RID: 541 RVA: 0x000141F4 File Offset: 0x000123F4
            public bool Underline
            {
                get => (FontFlags & LLFontFlags.Underline) > LLFontFlags.Normal;
                set
                {
                    checked
                    {
                        if (value)
                        {
                            if ((~FontFlags & LLFontFlags.Underline) > LLFontFlags.Normal)
                            {
                                FontFlags += 4;
                            }
                        }
                        else if ((FontFlags & LLFontFlags.Underline) > LLFontFlags.Normal)
                        {
                            FontFlags -= 4;
                        }
                    }
                }
            }

            // Token: 0x1700007B RID: 123
            // (get) Token: 0x0600021E RID: 542 RVA: 0x00014258 File Offset: 0x00012458
            // (set) Token: 0x0600021F RID: 543 RVA: 0x00014278 File Offset: 0x00012478
            public bool Strikethrough
            {
                get => (FontFlags & LLFontFlags.Strikethrough) > LLFontFlags.Normal;
                set
                {
                    checked
                    {
                        if (value)
                        {
                            if ((~FontFlags & LLFontFlags.Strikethrough) > LLFontFlags.Normal)
                            {
                                FontFlags += 8;
                            }
                        }
                        else if ((FontFlags & LLFontFlags.Strikethrough) > LLFontFlags.Normal)
                        {
                            FontFlags -= 8;
                        }
                    }
                }
            }

            // Token: 0x1700007C RID: 124
            // (get) Token: 0x06000220 RID: 544 RVA: 0x000142DC File Offset: 0x000124DC
            // (set) Token: 0x06000221 RID: 545 RVA: 0x000142F4 File Offset: 0x000124F4
            public LLTextAlign TextAlign
            {
                get => Alignment;
                set => Alignment = value;
            }

            // Token: 0x06000222 RID: 546 RVA: 0x00014300 File Offset: 0x00012500
            public ListLabelItemV2()
            {
                Txt = "";
                WrappedText = "";
                State = LLItemState.Enabled;
                FontFlags = LLFontFlags.Normal;
                Alignment = LLTextAlign.Left;
                nIDSet = -1;
                IDXPower = -1;
                nIDPower = -1;
                sTag = "";
                LineCount = 1;
                ItemHeight = 1;
                Index = -1;
            }

            // Token: 0x06000223 RID: 547 RVA: 0x00014378 File Offset: 0x00012578
            public ListLabelItemV2(string iText, LLItemState iState, int inIDSet = -1, int iIDXPower = -1, int inIDPower = -1,
                string iStringTag = "", LLFontFlags iFont = LLFontFlags.Normal, LLTextAlign iAlign = LLTextAlign.Left)
            {
                Txt = "";
                WrappedText = "";
                State = LLItemState.Enabled;
                FontFlags = LLFontFlags.Normal;
                Alignment = LLTextAlign.Left;
                nIDSet = -1;
                IDXPower = -1;
                nIDPower = -1;
                sTag = "";
                LineCount = 1;
                ItemHeight = 1;
                Index = -1;
                Txt = iText;
                State = iState;
                nIDSet = inIDSet;
                IDXPower = iIDXPower;
                nIDPower = inIDPower;
                sTag = iStringTag;
                FontFlags = iFont;
                Alignment = iAlign;
            }

            // Token: 0x06000224 RID: 548 RVA: 0x0001442C File Offset: 0x0001262C
            public ListLabelItemV2(ListLabelItemV2 Template)
            {
                Txt = "";
                WrappedText = "";
                State = LLItemState.Enabled;
                FontFlags = LLFontFlags.Normal;
                Alignment = LLTextAlign.Left;
                nIDSet = -1;
                IDXPower = -1;
                nIDPower = -1;
                sTag = "";
                LineCount = 1;
                ItemHeight = 1;
                Index = -1;
                Txt = Template.Txt;
                State = Template.State;
                FontFlags = Template.FontFlags;
                Alignment = Template.Alignment;
                LineCount = Template.LineCount;
                ItemHeight = Template.ItemHeight;
                nIDSet = Template.nIDSet;
                IDXPower = Template.IDXPower;
                nIDPower = Template.nIDPower;
                sTag = Template.sTag;
            }

            // Token: 0x04000112 RID: 274
            protected string Txt;

            // Token: 0x04000113 RID: 275
            public string WrappedText;

            // Token: 0x04000114 RID: 276
            protected LLItemState State;

            // Token: 0x04000115 RID: 277
            public LLFontFlags FontFlags;

            // Token: 0x04000116 RID: 278
            protected LLTextAlign Alignment;

            // Token: 0x04000117 RID: 279
            public int nIDSet;

            // Token: 0x04000118 RID: 280
            public int IDXPower;

            // Token: 0x04000119 RID: 281
            public int nIDPower;

            // Token: 0x0400011A RID: 282
            protected string sTag;

            // Token: 0x0400011B RID: 283
            public int LineCount;

            // Token: 0x0400011C RID: 284
            public int ItemHeight;

            // Token: 0x0400011D RID: 285
            public int Index;
        }
    }
}

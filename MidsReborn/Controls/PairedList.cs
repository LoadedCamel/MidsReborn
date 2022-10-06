using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;

namespace Mids_Reborn.Controls
{
    public partial class PairedList : UserControl
    {
        public delegate void ItemClickEventHandler(ItemPair? item, MouseEventArgs e);
        public delegate void ItemHoverEventHandler(object? sender, Enums.ShortFX tagId, string tooltip = "");
        public delegate void ItemOutEventHandler(object? sender);

        public event ItemClickEventHandler? ItemClick;
        public event ItemHoverEventHandler? ItemHover;
        public event ItemOutEventHandler? ItemOut;

        private ExtendedBitmap? _bxBuffer;
        private int _currentHighlight;
        private bool _highlightable;
        private int _lineHeight;
        private int _myColumns;
        private bool _myForceBold;
        private Graphics? _myGfx;
        private List<ItemPair>? _myItems;
        private int _myValueWidth;
        public Color ValueColorD;

        public Color HighlightColor { get; set; }
        public Color HighlightTextColor { get; set; }
        public Color NameColor { get; set; }
        public Color ItemColor { get; set; }
        public Color ItemColorAlt { get; set; }
        public Color ItemColorSpecial { get; set; }
        public Color ItemColorSpecCase { get; set; }
        public int ItemCount => _myItems?.Count ?? 0;

        public bool ForceBold
        {
            get => _myForceBold;
            set
            {
                _myForceBold = value;
                Draw();
            }
        }
        public int Columns
        {
            get => _myColumns;
            set
            {
                if (!((value >= 0) & (value < 10)))
                    return;
                _myColumns = value;
                Draw();
            }
        }
        public int ValueWidth
        {
            get => _myValueWidth;
            set
            {
                if (_myColumns < 1) _myColumns = 1;

                if ((value > 0) & (value < 100)) _myValueWidth = value;
            }
        }
        public bool DoHighlight
        {
            get => _highlightable;
            set
            {
                _highlightable = value;
                Draw();
            }
        }

        public int LinePadding { get; set; }

        public PairedList()
        {
            FontChanged += ctlPairedList_FontChanged;
            Load += ctlPairedList_Load;
            Paint += ctlPairedList_Paint;
            MouseDown += ListLabel_MouseDown;
            MouseMove += ListLabel_MouseMove;
            MouseLeave += ListLabel_MouseLeave;
            BackColorChanged += ctlPairedList_BackColorChanged;
            ValueColorD = Color.Aqua;
            LinePadding = 2;
            _myForceBold = false;
            InitializeComponent();
        }
        private void ctlPairedList_Load(object sender, EventArgs e)
        {
            SetLineHeight();
            _currentHighlight = -1;
            _myItems = new List<ItemPair>();
            _myGfx = CreateGraphics();
            _bxBuffer = new ExtendedBitmap(Width, Height);
            _bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
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
        public void SetUnique()
        {
            if (_myItems is { Count: > 0 }) _myItems[^1].UniqueColor = true;
        }

        public bool IsSpecialColor()
        {
            return _myItems != null && _myItems[^1].VerySpecialColor;
        }
        private void ctlPairedList_FontChanged(object sender, EventArgs e)
        {
            Draw();
        }
        private void FullUpdate()
        {
            SetLineHeight();
            _myGfx = CreateGraphics();
            _bxBuffer = new ExtendedBitmap(Width, Height);
            Draw();
        }

        public void Draw()
        {
            checked
            {
                if (_bxBuffer == null)
                    return;

                if (_myGfx == null) return;
                _myGfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                _myGfx.CompositingMode = CompositingMode.SourceOver;
                _myGfx.CompositingQuality = CompositingQuality.HighQuality;
                _myGfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                _myGfx.SmoothingMode = SmoothingMode.HighQuality;

                _bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                _bxBuffer.Graphics.CompositingMode = CompositingMode.SourceOver;
                _bxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                _bxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                _bxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                var rect = new Rectangle(0, 0, Width, Height);

                var rectangleF = new RectangleF(0, 0, 0, 0);
                var stringFormat = new StringFormat();
                var newStyle = FontStyle.Bold;
                var newStyle2 = FontStyle.Regular;
                if (_myForceBold)
                {
                    newStyle2 = FontStyle.Bold;
                    newStyle = FontStyle.Bold;
                }

                var font = new Font(Font.FontFamily, Font.Size, newStyle, GraphicsUnit.Pixel);
                var font2 = new Font(Font.FontFamily, Font.Size, newStyle2, GraphicsUnit.Pixel);

                rectangleF.X = 0f;
                if (_myColumns < 1) _myColumns = 1;

                rectangleF.Height = _lineHeight;
                _bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Brush brush = new SolidBrush(BackColor);
                _bxBuffer.Graphics.FillRectangle(brush, rect);
                if (_myItems == null) _myGfx.DrawImageUnscaled(_bxBuffer.Bitmap, 0, 0);

                if (_myItems is { Count: < 1 }) _myGfx.DrawImageUnscaled(_bxBuffer.Bitmap, 0, 0);

                var num4 = 0;
                var num5 = 0;
                if (_myItems != null)
                {
                    for (var i = 0; i < _myItems.Count; i++)
                    {
                        //Names
                        var text = _myItems[i]?.Name;
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            if (!text.EndsWith(":"))
                            {
                                text += ":";
                            }
                        }

                        var stringM = _bxBuffer.Graphics.MeasureString(text, font);

                        var columnWidth = (int)Math.Round(Width / (double)_myColumns);
                        var scaleWidth = (int)Math.Round(columnWidth * (_myValueWidth / 125f));
                        var controlWidth = columnWidth - scaleWidth;

                        var location = new PointF(columnWidth * num5, rectangleF.Height * num4 + checked(LinePadding * num4));
                        rectangleF.Location = location;
                        rectangleF.Width = stringM.Width;
                        stringFormat.Alignment = StringAlignment.Far;
                        stringFormat.LineAlignment = StringAlignment.Center;
                        stringFormat.Trimming = StringTrimming.None;
                        stringFormat.FormatFlags = StringFormatFlags.NoWrap;
                        if (_highlightable & (_currentHighlight == i))
                        {
                            brush = new SolidBrush(HighlightColor);
                            _bxBuffer.Graphics.FillRectangle(brush, rectangleF);
                            brush = new SolidBrush(HighlightTextColor);
                        }
                        else
                        {
                            brush = new SolidBrush(NameColor);
                        }

                        _bxBuffer.Graphics.DrawString(text, font, brush, rectangleF, stringFormat);

                        //Values
                        var value = _myItems[i].Value;
                        var stringM2 = _bxBuffer.Graphics.MeasureString(value, font2);
                        var location2 = new PointF(location.X * num5 + stringM.Width, rectangleF.Height * num4 + checked(LinePadding * num4));
                        _myItems[i].SetBounds(new RectangleF(location.X, location.Y, rectangleF.Width + stringM2.Width, rectangleF.Height));
                        rectangleF.Location = location2;
                        rectangleF.Width = stringM2.Width;
                        stringFormat.Alignment = StringAlignment.Near;
                        stringFormat.LineAlignment = StringAlignment.Center;
                        if (_highlightable & (_currentHighlight == i))
                        {
                            brush = new SolidBrush(HighlightColor);
                            _bxBuffer.Graphics.FillRectangle(brush, rectangleF);
                            brush = new SolidBrush(HighlightTextColor);
                        }
                        else if (_myItems[i].UniqueColor)
                        {
                            brush = new SolidBrush(ValueColorD);
                        }
                        else if (_myItems[i].AlternateColor)
                        {
                            brush = new SolidBrush(ItemColorAlt);
                        }
                        else if (_myItems[i].ProbColor)
                        {
                            brush = new SolidBrush(ItemColorSpecial);
                        }
                        else
                        {
                            brush = new SolidBrush(ItemColor);
                        }


                        _bxBuffer.Graphics.DrawString(value, font2, brush, rectangleF, stringFormat);
                        num5++;
                        if (num5 < _myColumns)
                            continue;
                        num5 = 0;
                        num4++;
                    }
                }

                _myGfx.DrawImageUnscaled(_bxBuffer.Bitmap, 0, 0);
            }
        }
        
        private void ListLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (_myItems == null) return;
            foreach (var item in _myItems.Where(item => e.X >= item.Bounds.Left && e.X <= item.Bounds.Right && e.Y >= item.Bounds.Top && e.Y <= item.Bounds.Bottom))
            {
                ItemClick?.Invoke(item, e);
                break;
            }
        }

        private void ListLabel_MouseMove(object sender, MouseEventArgs e)
        {
            var num = 0;
            var num2 = 0;
            var num3 = -1;
            Rectangle rectangle = default;
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Height = _lineHeight;
            checked
            {
                rectangle.Width = (int)Math.Round(Width / (double)_myColumns);
                if (_myItems != null)
                    for (var i = 0; i < _myItems.Count; i++)
                    {
                        rectangle.X = rectangle.Width * num2;
                        rectangle.Y = (rectangle.Height + LinePadding) * num;
                        if ((e.Y >= rectangle.Y) & (e.Y <= rectangle.Height + rectangle.Y) && (e.X >= rectangle.X) & (e.X <= rectangle.Width + rectangle.X))
                        {
                            num3 = i;
                            break;
                        }

                        num2++;
                        if (num2 < _myColumns)
                        {
                            continue;
                        }

                        num2 = 0;
                        num++;
                    }

                if (_currentHighlight == num3)
                {
                    return;
                }

                _currentHighlight = num3;
                if (_highlightable)
                {
                    Draw();
                }

                if (num3 <= -1)
                {
                    ItemOut?.Invoke(this);

                    return;
                }


                var specialTip = _myItems?[num3].SpecialTip;
                if (specialTip != null)
                    ItemHover?.Invoke(this, _myItems[num3].TagId, specialTip);
            }
        }

        private void ListLabel_MouseLeave(object sender, EventArgs e)
        {
            _currentHighlight = -1;
            if (_highlightable) Draw();

            myTip.SetToolTip(this, "");
        }

        private void ctlPairedList_Paint(object sender, PaintEventArgs e)
        {
            if (_bxBuffer == null) return;
            _myGfx?.DrawImage(_bxBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
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
            _lineHeight = checked((int)Math.Round(font.GetHeight() + LinePadding));
        }

        public void AddItem(ItemPair iItem)
        {
            try
            {
                _myItems?.Add(new ItemPair(iItem));
            }
            catch (Exception)
            {
                Debug.WriteLine("PairedList.AddItem(): Illegal item add call");
            }
        }

        public ItemPair? GetItemAt(int index)
        {
            if (_myItems == null) return null;
            if (!_myItems.Any()) return null;
            if (index < 0 || index >= _myItems.Count) throw new ArgumentOutOfRangeException(nameof(index));
            return _myItems[index];
        }

        public void Clear(bool redraw = false)
        {
            _myItems?.Clear();
            if (redraw) Draw();
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
            public bool AlternateColor { get; set; }
            public bool ProbColor { get; set; }
            public string? SpecialTip { get; set; }
            public bool VerySpecialColor { get; set; }
            public string? Name { get; set; }
            public Enums.ShortFX TagId { get; set; }
            public bool UniqueColor { get; set; }
            public string? Value { get; set; }
            public SummonedEntity? EntTag { get; set; }
            internal RectangleF Bounds { get; private set; }

            internal void SetBounds(RectangleF rect)
            {
                Bounds = rect;
            }

            public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability, bool iSpecialCase, string iTip)
            {
                Name = iName;
                Value = iValue;
                AlternateColor = iAlternate;
                ProbColor = iProbability;
                VerySpecialColor = iSpecialCase;
                TagId.Add(-1, 0f);
                SpecialTip = iTip;
                
            }

            public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability, bool iSpecialCase, string iTip, SummonedEntity entTag)
            {
                Name = iName;
                Value = iValue;
                AlternateColor = iAlternate;
                ProbColor = iProbability;
                VerySpecialColor = iSpecialCase;
                TagId.Add(-1, 0f);
                SpecialTip = iTip;
                EntTag = entTag;
            }

            public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability = false, bool iSpecialCase = false, int iTagId = -1)
            {
                Name = iName;
                Value = iValue;
                AlternateColor = iAlternate;
                ProbColor = iProbability;
                VerySpecialColor = iSpecialCase;
                TagId.Add(iTagId, 0f);
            }

            public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability, bool iSpecialCase, Enums.ShortFX iTagId)
            {
                Name = iName;
                Value = iValue;
                AlternateColor = iAlternate;
                ProbColor = iProbability;
                VerySpecialColor = iSpecialCase;
                TagId.Assign(iTagId);
            }

            public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability, bool iSpecialCase, Enums.ShortFX iTagId, string iTip)
            {
                Name = iName;
                Value = iValue;
                AlternateColor = iAlternate;
                ProbColor = iProbability;
                VerySpecialColor = iSpecialCase;
                TagId.Assign(iTagId);
                SpecialTip = iTip;
            }

            public ItemPair(ItemPair iItem)
            {
                Name = iItem.Name;
                Value = iItem.Value;
                AlternateColor = iItem.AlternateColor;
                ProbColor = iItem.ProbColor;
                VerySpecialColor = iItem.VerySpecialColor;
                TagId.Assign(iItem.TagId);
                SpecialTip = iItem.SpecialTip;
                EntTag = iItem.EntTag;
            }
        }
    }
}

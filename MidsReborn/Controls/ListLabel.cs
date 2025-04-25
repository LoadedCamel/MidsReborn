using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Controls
{
    public partial class ListLabel : UserControl
    {
        public delegate void EmptyHoverEventHandler();
        public delegate void ExpandChangedEventHandler(bool expanded);
        public delegate void ItemClickEventHandler(ListLabelItem item, MouseButtons button);
        public delegate void ItemHoverEventHandler(ListLabelItem item);

        [Flags]
        public enum LlFontFlags
        {
            Normal = 0,
            Bold = 1,
            Italic = 2,
            Underline = 4,
            Strikethrough = 8
        }

        public enum LlItemState
        {
            Enabled,
            Selected,
            Disabled,
            SelectedDisabled,
            Invalid,
            Heading
        }

        public enum LlTextAlign
        {
            Left,
            Center,
            Right
        }

        private Color[] _colors;
        private Cursor[] _cursors;
        private bool[] _highlightOn;
        private Color _bgColor;
        private Color _hvrColor;

        private ExtendedBitmap? _bxBuffer;

        private bool _canExpand;
        private bool _canScroll;
        private bool _disableEvents;
        private bool _disableRedraw;
        private bool _dragMode;
        private int _expandMaxY;

        public ListLabelItem[] Items => _items.ToArray();
        private List<ListLabelItem> _items;

        private EMouseTarget _lastMouseMoveTarget;
        private Color _scBarColor;
        private Color _scButtonColor;
        private int _scrollOffset;
        private int _scrollSteps;
        private int _scrollWidth;
        private Size _szNormal;
        private Rectangle _textArea;
        private bool _textOutline;
        private int _visibleLineCount;

        private int _xPadding;
        private int _yPadding;

        private bool IsDesignMode =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            Site is { DesignMode: true };

        public ListLabel()
        {
            _bxBuffer = null;
            _items = [];
            _colors =
            [
                Color.LightBlue, Color.LightGreen, Color.LightGray, Color.DarkGreen, Color.Red, Color.Orange
            ];
            _cursors =
            [
                Cursors.Hand, Cursors.Hand,
                Cursors.Default,
                Cursors.Hand, Cursors.Hand,
                Cursors.Default
            ];
            _highlightOn =
            [
                true, true, true, true, true, true, false
            ];
            _bgColor = Color.Black;
            _hvrColor = Color.WhiteSmoke;
            _textOutline = true;
            _xPadding = 1;
            _yPadding = 1;
            ActualLineHeight = 8;
            HoverId = -1;
            _disableRedraw = true;
            _disableEvents = false;
            _canScroll = true;
            _scrollOffset = 0;
            _canExpand = false;
            IsExpanded = false;
            _szNormal = Size;
            _expandMaxY = 400;
            _textArea = new Rectangle(0, 0, Width, Height);
            _scrollWidth = 8;
            _scBarColor = Color.FromArgb(128, 96, 192);
            _scButtonColor = Color.FromArgb(96, 0, 192);
            _scrollSteps = 0;
            _dragMode = false;
            _lastMouseMoveTarget = EMouseTarget.None;
            _visibleLineCount = 0;
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!IsDesignMode)
            {
                SafeInitialize(); // move this here instead of constructor
            }
        }

        private void SafeInitialize()
        {
            MouseLeave += ListLabelV3_MouseLeave;
            MouseMove += ListLabelV3_MouseMove;
            MouseUp += ListLabelV3_MouseUp;
            Resize += ListLabelV3_Resize;
            FontChanged += ListLabelV3_FontChanged;
            Load += ListLabelV3_Load;
            MouseDown += ListLabelV3_MouseDown;
            MouseWheel += ListLabelV3_MouseWheel;
        }

        public bool IsExpanded { get; set; }

        public override Color BackColor
        {
            get => _bgColor;
            set
            {
                _bgColor = value;
                Draw();
            }
        }

        public Color HoverColor
        {
            get => _hvrColor;
            set
            {
                _hvrColor = value;
                Draw();
            }
        }

        public int PaddingX
        {
            get => _xPadding;
            set
            {
                if (!(value >= 0 & checked(value * 2 < Width - 5)))
                    return;
                _xPadding = value;
                Draw();
            }
        }

        public int PaddingY
        {
            get => _yPadding;
            set
            {
                if (!(value >= 0 & value < Height / 3f))
                    return;
                _yPadding = value;
                SetLineHeight();
                Draw();
            }
        }

        public bool HighVis
        {
            get => _textOutline;
            set
            {
                _textOutline = value;
                Draw();
            }
        }

        private int HoverId { get; set; }

        public bool SuspendRedraw
        {
            get => _disableRedraw;
            set
            {
                _disableRedraw = value;
                if (value)
                    return;
                if (IsExpanded) RecomputeExpand();

                if (IsExpanded)
                    return;
                Recalculate();
                Draw();
            }
        }

        public bool Scrollable
        {
            get => _canScroll;
            set
            {
                _canScroll = value;
                Draw();
            }
        }

        public bool Expandable
        {
            get => _canExpand;
            set
            {
                _canExpand = value;
                Draw();
            }
        }

        public Size SizeNormal
        {
            get => _szNormal;
            set
            {
                _szNormal = value;
                Expand(IsExpanded);
                Draw();
            }
        }

        public int MaxHeight
        {
            get => _expandMaxY;
            set
            {
                if (value < _szNormal.Height)
                    return;
                if (value > 2000)
                    return;
                _expandMaxY = value;
                Draw();
            }
        }

        public int ScrollBarWidth
        {
            get => _scrollWidth;
            set
            {
                if (value > 0 & value < Width / 2f) _scrollWidth = value;

                Recalculate();
                Draw();
            }
        }

        public Color ScrollBarColor
        {
            get => _scBarColor;
            set
            {
                _scBarColor = value;
                Draw();
            }
        }

        public Color ScrollButtonColor
        {
            get => _scButtonColor;
            set
            {
                _scButtonColor = value;
                Draw();
            }
        }

        public int ContentHeight => Height;
        private const int MultilineTextInterline = 4;

        public int DesiredHeight => _items
            .Select(item => item.WrappedText.Split("\r\n").Length)
            .Select(lines => lines * ActualLineHeight + (lines > 1 ? (lines - 1) * MultilineTextInterline : 0))
            .Sum();

        public int ActualLineHeight { get; set; }

        public event ItemClickEventHandler? ItemClick;
        public event ItemHoverEventHandler? ItemHover;
        public event EmptyHoverEventHandler? EmptyHover;
        public event ExpandChangedEventHandler? ExpandChanged;

        private int GetRealTotalHeight()
        {
            return _items.Sum(e => e.ItemHeight);
        }

        private void FillDefaultItems()
        {
            ClearItems();
            AddItem(new ListLabelItem("Header Item", LlItemState.Heading, -1, -1, -1, "", LlFontFlags.Bold, LlTextAlign.Center));
            AddItem(new ListLabelItem("Enabled", LlItemState.Enabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Disabled Item", LlItemState.Disabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Selected Item", LlItemState.Selected, -1, -1, -1, "", LlFontFlags.Bold | LlFontFlags.Italic));
            AddItem(new ListLabelItem("SD Item", LlItemState.SelectedDisabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Invalid Item", LlItemState.Invalid, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Automatic multiline Item", LlItemState.Enabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Scrollable", LlItemState.Heading, -1, -1, -1, "", LlFontFlags.Bold, LlTextAlign.Center));
            AddItem(new ListLabelItem("Item 1", LlItemState.Enabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 2", LlItemState.Selected, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 3", LlItemState.Selected, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 4", LlItemState.Selected, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 5", LlItemState.Disabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 6", LlItemState.Enabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 7", LlItemState.Enabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 8", LlItemState.Selected, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 9", LlItemState.Enabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 10", LlItemState.Disabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 11", LlItemState.Selected, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 12", LlItemState.Selected, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 13", LlItemState.Invalid, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 14", LlItemState.Enabled, -1, -1, -1, "", LlFontFlags.Bold));
            AddItem(new ListLabelItem("Item 15", LlItemState.Enabled, -1, -1, -1, "", LlFontFlags.Bold));
            Draw();
        }

        public void AddItem(ListLabelItem iItem)
        {
            _disableEvents = true;
            if (iItem.Index < 0)
            {
                iItem.Index = _items.Count;
            }

            _items.Add(iItem);
            WrapString(_items.Count - 1);
            GetScrollSteps();
            _disableEvents = false;
        }

        public void ClearItems()
        {
            _items = [];
            _scrollOffset = 0;
            HoverId = -1;
        }

        private void SetLineHeight()
        {
            var font = new Font(Font, Font.Style);
            ActualLineHeight = checked((int)Math.Round(font.GetHeight() + checked(_yPadding * 2)));
            _visibleLineCount = GetVisibleLineCount();
        }

        private void Recalculate()
        {
            if (_items.Count == 0) return;

            InitBuffer();

            if (AutoSize)
            {
                Height = AutoSizeMode == AutoSizeMode.GrowAndShrink
                    ? DesiredHeight
                    : Math.Max(DesiredHeight, _szNormal.Height);
            }

            var bRect = new Rectangle(_xPadding, 0, Width - _xPadding * 2, Height);
            RecalcLines(bRect);

            if (_scrollSteps > 0 || IsExpanded)
            {
                bRect = new Rectangle(_xPadding, 0, Width - _xPadding * 2 - _scrollWidth, Height);
                RecalcLines(bRect);
            }
        }

        private void RecalcLines(Rectangle bRect)
        {
            _textArea = bRect;
            SetLineHeight();
            checked
            {
                for (var i = 0; i < _items.Count; i++) WrapString(i);

                GetTotalLineCount();
                GetScrollSteps();
            }
        }

        private void WrapString(int index)
        {
            if (string.IsNullOrEmpty(_items[index].Text))
            {
                return;
            }

            InitBuffer();
            var num = 1;
            var strWords = _items[index].Text.Split(' ');
            var stringFormat = new StringFormat(StringFormatFlags.NoWrap);
            var font = MidsContext.Config.PowerListsWordwrapMode is Enums.WordwrapMode.Legacy
                ? new Font(Font, (FontStyle)_items[index].FontFlags)
                : new Font(Font.FontFamily, Font.Size, (FontStyle)_items[index].FontFlags, GraphicsUnit.Point);
            var str = _items[index].ItemState == LlItemState.Heading ? "~  ~" : "";
            var text = strWords[0];
            var layoutArea = new SizeF(1024f, Height);

            if (!_items[index].Text.Contains(' '))
            {
                // UseEllipsis mode needs some parsing here.
                if (MidsContext.Config.PowerListsWordwrapMode is not Enums.WordwrapMode.UseEllipsis)
                {
                    _items[index].WrappedText = _items[index].Text;
                }
                else
                {
                    text = "";
                    var fullFit = true;
                    for (var i = 0; i < _items[index].Text.Length; i++)
                    {
                        var text2 = $"{text}{((i == 0) ? " " : "")}{_items[index].Text.Substring(i, 1)}";
                        var tw = (int)Math.Ceiling(_bxBuffer.Graphics.MeasureString(_items[index].ItemState == LlItemState.Heading ? $"~ {text2.TrimEnd()}... ~" : $"{text2.TrimEnd()}...", font, layoutArea, stringFormat).Width);
                        if (tw <= _textArea.Width)
                        {
                            text = text2;
                        }
                        else
                        {
                            fullFit = false;
                            break;
                        }
                    }

                    if (!fullFit)
                    {
                        text = $"{text.TrimEnd()}...";
                    }

                    _items[index].WrappedText = _items[index].ItemState == LlItemState.Heading
                        ? $"~ {text} ~"
                        : text;
                }
            }
            else
            {
                switch (MidsContext.Config.PowerListsWordwrapMode)
                {
                    case Enums.WordwrapMode.Legacy:
                    case Enums.WordwrapMode.New:
                        for (var i = 1; i < strWords.Length; i++)
                        {
                            var text2 = $"{text} {strWords[i]}{str}";

                            if (Math.Ceiling(_bxBuffer.Graphics.MeasureString(text2, font, layoutArea, stringFormat).Width) > _textArea.Width)
                            {
                                text = _items[index].ItemState == LlItemState.Heading
                                    ? $"{text} ~\r\n~ {strWords[i]}"
                                    : $"{text}\r\n {strWords[i]}";
                                num++;
                            }
                            else
                            {
                                text = $"{text} {strWords[i]}";
                            }
                        }

                        break;

                    case Enums.WordwrapMode.UseEllipsis:
                        text = "";
                        int k;
                        var fullFit = true;
                        strWords = _items[index].Text.Trim(" ~".ToCharArray()).Split(' ');
                        for (k = 0; k < strWords.Length; k++)
                        {
                            var text2 = $"{text}{((k == 0) ? "" : " ")}{strWords[k]}";
                            var tw = (int)Math.Ceiling(_bxBuffer.Graphics.MeasureString(_items[index].ItemState == LlItemState.Heading ? $"~ {text2}... ~" : text2, font, layoutArea, stringFormat).Width);
                            if (tw <= _textArea.Width)
                            {
                                text = text2;
                            }
                            else
                            {
                                fullFit = false;
                                break;
                            }
                        }

                        if (!fullFit)
                        {
                            fullFit = true;
                            for (var i = 0; i < strWords[k].Length; i++)
                            {
                                var text2 = $"{text}{((i == 0) ? " " : "")}{strWords[k].Substring(i, 1)}";
                                var tw = (int)Math.Ceiling(_bxBuffer.Graphics.MeasureString(_items[index].ItemState == LlItemState.Heading ? $"~ {text2.TrimEnd()}... ~" : $"{text2.TrimEnd()}...", font, layoutArea, stringFormat).Width);
                                if (tw <= _textArea.Width)
                                {
                                    text = text2;
                                }
                                else
                                {
                                    fullFit = false;
                                    break;
                                }
                            }

                            if (!fullFit)
                            {
                                text = $"{text.TrimEnd()}...";
                            }
                        }

                        if (_items[index].ItemState == LlItemState.Heading)
                        {
                            text = $"~ {text} ~";
                        }

                        break;
                }

                _items[index].WrappedText = text;
            }

            _items[index].WrappedText = _items[index].WrappedText.Replace("  ", " ");
            if (_items[index].ItemState == LlItemState.Heading & _items[index].WrappedText[0] != '~')
            {
                _items[index].WrappedText = $"~ {_items[index].WrappedText} ~";
            }

            _items[index].LineCount = num;
            _items[index].ItemHeight = num * (ActualLineHeight - _yPadding * 2) + _yPadding * 2;
        }

        private void InitBuffer()
        {
            if (_disableRedraw) return;
            _bxBuffer ??= new ExtendedBitmap(Width, Height);

            if (_bxBuffer.Size.Width != Width | _bxBuffer.Size.Height != Height)
            {
                _bxBuffer.Dispose();
                _bxBuffer = null;
                GC.Collect();
                if (Height == 0 | Width == 0) return;

                _bxBuffer = new ExtendedBitmap(Width, Height);
            }

            _bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            _bxBuffer.Graphics.TextContrast = 0;
            _bxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            _bxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _bxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        private int GetItemAtY(int y)
        {
            var num = 0;
            checked
            {
                int result;
                if (y > Height)
                {
                    result = -1;
                }
                else
                {
                    for (var i = _scrollOffset; i < _items.Count; i++)
                    {
                        num += _items[i].ItemHeight;
                        if (y < num) return i;
                    }

                    result = -1;
                }

                return result;
            }
        }

        private EMouseTarget GetMouseTarget(int x, int y)
        {
            checked
            {
                EMouseTarget result;
                if (x >= _textArea.Left & x <= _textArea.Right & y <= _textArea.Bottom)
                {
                    result = EMouseTarget.Item;
                }
                else if (x >= _textArea.Left & x <= _textArea.Right & y > _textArea.Bottom)
                {
                    result = EMouseTarget.ExpandButton;
                }
                else if (x > _textArea.Right & y <= _scrollWidth + _yPadding)
                {
                    result = EMouseTarget.UpButton;
                }
                else if (x > _textArea.Right & y >= Height - (_scrollWidth + _yPadding))
                {
                    result = EMouseTarget.DownButton;
                }
                else if (!(x > _textArea.Right & _scrollSteps > 0))
                {
                    result = EMouseTarget.None;
                }
                else
                {
                    var num = Height - (_yPadding + _scrollWidth) * 2 - _yPadding * 2;
                    var num2 = (int)Math.Round(checked(_scrollWidth + _yPadding * 2) +
                                                num / (double)_scrollSteps * _scrollOffset);
                    var num3 = (int)Math.Round(num / (double)_scrollSteps);
                    if (y < num2)
                        result = EMouseTarget.ScrollBarUp;
                    else if (y > num2 + num3)
                        result = EMouseTarget.ScrollBarDown;
                    else
                        result = EMouseTarget.ScrollBlock;
                }

                return result;
            }
        }

        private void ListLabelV3_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (e.Delta > 0 & _scrollSteps > 0 & _scrollOffset > 0)
            {
                _scrollOffset--;
                Draw();
            }
            else if (e.Delta < 0 & _scrollOffset + 1 < _scrollSteps)
            {
                _scrollOffset++;
                Draw();
            }
        }

        private void ListLabelV3_MouseDown(object? sender, MouseEventArgs e)
        {
            checked
            {
                if (e.Button == MouseButtons.Left & ModifierKeys == (Keys.Shift | Keys.Control | Keys.Alt))
                {
                    _disableEvents = false;
                    _disableRedraw = false;
                    Recalculate();
                    Draw();
                    var graphics = CreateGraphics();
                    var powderBlue = Pens.PowderBlue;
                    var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                    graphics.DrawRectangle(powderBlue, rect);
                }
                else if (!_disableEvents)
                {
                    var num = _scrollSteps / 3f < 5 ? (int)Math.Round(_scrollSteps / 3f) : 5;

                    switch (GetMouseTarget(e.X, e.Y))
                    {
                        case EMouseTarget.Item:
                            {
                                var itemAtY = GetItemAtY(e.Y);
                                if (itemAtY > -1)
                                {
                                    var num2 = 0;
                                    for (var i = _scrollOffset; i < itemAtY; i++) num2 += _items[i].ItemHeight;

                                    if (num2 + _items[itemAtY].ItemHeight >= e.Y &
                                        num2 + _items[itemAtY].ItemHeight <= _textArea.Height |
                                        _items[itemAtY].LineCount > 1 & num2 + ActualLineHeight >= e.Y &
                                        num2 + ActualLineHeight <= _textArea.Height)
                                        ItemClick?.Invoke(_items[itemAtY], e.Button);
                                }

                                break;
                            }
                        case EMouseTarget.UpButton:
                            if (_scrollSteps > 0 & _scrollOffset > 0)
                            {
                                _scrollOffset--;
                                Draw();
                            }

                            break;
                        case EMouseTarget.DownButton:
                            if (_scrollSteps > 0 & _scrollOffset + 1 < _scrollSteps)
                            {
                                _scrollOffset++;
                                Draw();
                            }

                            break;
                        case EMouseTarget.ScrollBarUp:
                            if (_scrollSteps > 0)
                            {
                                _scrollOffset -= num;
                                if (_scrollOffset < 0) _scrollOffset = 0;

                                Draw();
                            }

                            break;
                        case EMouseTarget.ScrollBarDown:
                            if (_scrollSteps > 0)
                            {
                                _scrollOffset += num;
                                if (_scrollOffset >= _scrollSteps) _scrollOffset = _scrollSteps - 1;

                                Draw();
                            }

                            break;
                        case EMouseTarget.ScrollBlock:
                            if (_scrollSteps > 0) _dragMode = true;

                            break;
                        case EMouseTarget.ExpandButton:
                            {
                                if (!IsExpanded)
                                {
                                    IsExpanded = true;
                                    _scrollOffset = 0;
                                    RecomputeExpand();
                                }
                                else
                                {
                                    _disableRedraw = true;
                                    Height = _szNormal.Height;
                                    IsExpanded = false;
                                    Recalculate();
                                    _disableRedraw = false;
                                    Draw();
                                }

                                ExpandChanged?.Invoke(IsExpanded);
                                break;
                            }
                    }
                }
            }
        }

        private void Expand(bool state)
        {
            if (state)
            {
                IsExpanded = true;
                _scrollOffset = 0;
                RecomputeExpand();
            }
            else
            {
                _disableRedraw = true;
                Height = _szNormal.Height;
                IsExpanded = false;
                Recalculate();
                _disableRedraw = false;
                Draw();
            }

            ExpandChanged?.Invoke(IsExpanded);
        }

        private void RecomputeExpand()
        {
            if (!IsExpanded) return;

            var maxHeight = Math.Min(GetRealTotalHeight() + _scrollWidth + _yPadding * 3, _expandMaxY);

            _disableRedraw = true;
            Height = maxHeight;
            Recalculate();
            _disableRedraw = false;

            Draw();
        }

        private void ListLabelV3_MouseLeave(object? sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            _lastMouseMoveTarget = EMouseTarget.None;
            HoverId = -1;
            Draw();
            EmptyHover?.Invoke();
        }

        private void ListLabelV3_MouseMove(object? sender, MouseEventArgs e)
        {
            var cursor = Cursors.Default;
            var mouseTarget = GetMouseTarget(e.X, e.Y);
            var redraw = false;
            checked
            {
                if (!_dragMode)
                {
                    var skipEmptyHoverEventCall = false;
                    switch (mouseTarget)
                    {
                        case EMouseTarget.Item:
                            {
                                var itemAtY = GetItemAtY(e.Y);
                                if (itemAtY <= -1)
                                {
                                    if (HoverId != -1)
                                    {
                                        redraw = true;
                                    }

                                    HoverId = -1;
                                    EmptyHover?.Invoke();

                                    break;
                                }

                                var num = 0;
                                for (var i = _scrollOffset; i < itemAtY; i++) num += _items[i].ItemHeight;

                                if (!(num + _items[itemAtY].ItemHeight >= e.Y &
                                      num + _items[itemAtY].ItemHeight <= _textArea.Height |
                                      _items[itemAtY].LineCount > 1 & num + ActualLineHeight >= e.Y &
                                      num + ActualLineHeight <= _textArea.Height))
                                {
                                    if (HoverId != -1) redraw = true;

                                    HoverId = -1;
                                    EmptyHover?.Invoke();
                                    break;
                                }

                                cursor = _cursors[(int)_items[itemAtY].ItemState];
                                HoverId = itemAtY;
                                Draw();
                                ItemHover?.Invoke(_items[itemAtY]);
                                skipEmptyHoverEventCall = true;

                                break;
                            }
                        case EMouseTarget.UpButton:
                            if (_lastMouseMoveTarget != mouseTarget)
                            {
                                Draw();
                            }

                            break;
                        case EMouseTarget.DownButton:
                            if (_lastMouseMoveTarget != mouseTarget)
                            {
                                Draw();
                            }

                            break;
                        case EMouseTarget.ExpandButton:
                            HoverId = -1;

                            break;
                    }

                    if (!skipEmptyHoverEventCall)
                    {
                        var emptyHoverEvent = EmptyHover;
                        emptyHoverEvent?.Invoke();
                    }
                }
                else if (e.Button == MouseButtons.None)
                {
                    _dragMode = false;
                }
                else
                {
                    cursor = Cursors.SizeNS;
                    var num3 = Height - (_yPadding + _scrollWidth) * 2 - _yPadding * 2;
                    var num4 = (int)Math.Round(_scrollWidth + _yPadding * 2 + num3 / (double)_scrollSteps * _scrollOffset);
                    var num5 = (int)Math.Round(num3 / (double)_scrollSteps);
                    if (e.Y < num4 & _scrollOffset > 0)
                    {
                        _scrollOffset--;
                        Draw();
                    }
                    else if (e.Y > num4 + num5 & _scrollOffset + 1 < _scrollSteps)
                    {
                        _scrollOffset++;
                        Draw();
                    }

                    var emptyHoverEvent = EmptyHover;
                    emptyHoverEvent?.Invoke();
                }

                if (redraw)
                {
                    Draw();
                }

                Cursor = cursor;
                _lastMouseMoveTarget = mouseTarget;
            }
        }

        private void ListLabelV3_MouseUp(object? sender, MouseEventArgs e)
        {
            _dragMode = false;
            if (Cursor == Cursors.SizeNS) Cursor = Cursors.Default;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (IsDesignMode)
            {
                using var backgroundBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 255)); // Semi-transparent blue
                using var textBrush = new SolidBrush(Color.White);
                using var borderPen = new Pen(Color.DarkBlue, 2);
                using var font = new Font(Font.FontFamily, 10, FontStyle.Bold);

                var graphics = e.Graphics;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.FillRectangle(backgroundBrush, ClientRectangle);
                graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

                var textSize = graphics.MeasureString(Name, font);
                var textPosition = new PointF((Width - textSize.Width) / 2, (Height - textSize.Height) / 2);

                graphics.DrawString(Name, font, textBrush, textPosition);

                return; // Exit early to prevent normal drawing in designer
            }

            if (_bxBuffer == null)
            {
                Draw();

                return;
            }

            if (_bxBuffer.Bitmap != null)
                e.Graphics.DrawImage(_bxBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        private void ListLabelV3_Resize(object? sender, EventArgs e)
        {
            _scrollOffset = 0;
            Recalculate();
            Draw();
        }

        public void UpdateTextColors(LlItemState state, Color color)
        {
            if (state < LlItemState.Enabled | state > LlItemState.Heading)
            {
                return;
            }

            _colors[(int)state] = color;
            Draw();
        }

        private void ListLabelV3_FontChanged(object? sender, EventArgs e)
        {
            Recalculate();
            Draw();
        }

        private void ListLabelV3_Load(object? sender, EventArgs e)
        {
            _szNormal = Size;
            _disableRedraw = true;
            InitBuffer();
            Recalculate();
            FillDefaultItems();
            _disableRedraw = false;
        }

        private void Draw()
        {
            if (IsDisposed) return;

            checked
            {
                if (_disableRedraw) return;
                if (!Visible) return;
                InitBuffer();
                if (Width == 0 | Height == 0) return;
                _bxBuffer.Graphics.Clear(IsExpanded ? Color.Black : BackColor);
                for (var i = _scrollOffset; i < _items.Count; i++)
                {
                    DrawItem(i);
                }

                DrawScrollBar();
                DrawExpandButton();
                var graphics = CreateGraphics();
                if (_bxBuffer.Bitmap != null) graphics.DrawImageUnscaled(_bxBuffer.Bitmap, 0, 0);
            }
        }

        private void DrawItem(int index)
        {
            checked
            {
                if (index < 0)
                {
                    return;
                }

                if (index < _scrollOffset)
                {
                    return;
                }

                if (index > _items.Count - 1)
                {
                    return;
                }

                var num = 0;
                for (var i = _scrollOffset; i < index; i++)
                {
                    num += _items[i].ItemHeight;
                    if (num > Height)
                    {
                        return;
                    }
                }

                var height = _items[index].ItemHeight;
                if (_items[index].LineCount == 1)
                {
                    if (num + _items[index].ItemHeight > _textArea.Height)
                    {
                        return;
                    }
                }
                else if (num + _items[index].ItemHeight > _textArea.Height)
                {
                    if (num + ActualLineHeight > _textArea.Height)
                    {
                        return;
                    }

                    height = ActualLineHeight - _yPadding;
                }

                var rectangle = new Rectangle(_textArea.Left, num, _textArea.Width, height);
                var stringFormat = new StringFormat();
                stringFormat.Alignment = _items[index].TextAlign switch
                {
                    LlTextAlign.Left => StringAlignment.Near,
                    LlTextAlign.Center => StringAlignment.Center,
                    LlTextAlign.Right => StringAlignment.Far,
                    _ => stringFormat.Alignment
                };

                var fontStyle = FontStyle.Regular;
                if (_items[index].Bold) fontStyle |= FontStyle.Bold;
                if (_items[index].Italic) fontStyle |= FontStyle.Italic;
                if (_items[index].Underline) fontStyle |= FontStyle.Underline;
                if (_items[index].Strikethrough) fontStyle |= FontStyle.Strikeout;

                var font = new Font(Fonts.Family("Noto Sans"), Font.Size, fontStyle);
                if (index == HoverId && _highlightOn[(int)_items[index].ItemState])
                {
                    var brush = new SolidBrush(_hvrColor);
                    _bxBuffer.Graphics.FillRectangle(brush, rectangle);
                }

                using var outlineBrush = new SolidBrush(Color.Black);
                var textLines = _items[index].WrappedText.Split("\r\n");
                foreach (var tLine in textLines)
                {
                    var tl = tLine.Trim();
                    if (_textOutline)
                    {
                        var r = rectangle;

                        r.X--;
                        _bxBuffer.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.Y--;
                        _bxBuffer.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.X++;
                        _bxBuffer.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.X++;
                        _bxBuffer.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.Y++;
                        _bxBuffer.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.Y++;
                        _bxBuffer.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.X--;
                        _bxBuffer.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.X--;
                        _bxBuffer.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                    }

                    _bxBuffer.Graphics.DrawString(tl, font, new SolidBrush(_colors[(int)_items[index].ItemState]), rectangle, stringFormat);
                    rectangle.Offset(0, (int)Math.Ceiling(font.Size) + MultilineTextInterline);
                }
            }
        }

        private int GetVisibleLineCount()
        {
            if (!_canScroll)
            {
                _scrollSteps = 0;

                return 0;
            }

            var heightSum = 0;
            var visibleItems = 0;
            foreach (var item in _items)
            {
                heightSum += item.ItemHeight;
                if (heightSum > Height)
                {
                    break;
                }

                visibleItems += item.LineCount;
            }

            return IsExpanded
                ? GetTotalLineCount()
                : visibleItems;
        }

        private int GetTotalLineCount()
        {
            return _items.Sum(e => e.LineCount);
        }

        private void GetScrollSteps()
        {
            checked
            {
                if (!_canScroll)
                {
                    _scrollSteps = 0;
                    return;
                }

                var num = 0;
                var wrapCount = 0;
                foreach (var e in _items)
                {
                    num += e.LineCount;
                    if (num > _visibleLineCount) wrapCount++;
                }

                // Zed: add an extra scroll step to ensure the last element is always visible
                if (wrapCount > 0) wrapCount++;
                _scrollSteps = wrapCount <= 1 ? 0 : wrapCount + 1;
            }
        }

        private void DrawScrollBar()
        {
            if (_scrollWidth < 1 | !_canScroll | _scrollSteps < 1)
                return;
            SolidBrush brush;
            var pen = new Pen(_scBarColor);
            var pen2 = new Pen(Color.FromArgb(96, 255, 255, 255), 1f);
            var pen3 = new Pen(Color.FromArgb(128, 0, 0, 0), 1f);
            PointF[] array;
            Rectangle rect;
            checked
            {
                _bxBuffer.Graphics.DrawLine(pen, (int)Math.Round(_textArea.Right + _scrollWidth / 2f),
                    _yPadding + _scrollWidth,
                    (int)Math.Round(_textArea.Right + _scrollWidth / 2f), Height - (_scrollWidth + _yPadding));
                brush = new SolidBrush(_scButtonColor);
                array = new PointF[3];
                if (_scrollSteps > 0)
                {
                    var num = Height - (_yPadding + _scrollWidth) * 2 - _yPadding * 2;
                    var y = (int)Math.Round(checked(_scrollWidth + _yPadding * 2) +
                                             num / (double)_scrollSteps * _scrollOffset);
                    var height = (int)Math.Round(num / (double)_scrollSteps);
                    rect = new Rectangle(_textArea.Right, y, _scrollWidth, height);
                    _bxBuffer.Graphics.FillRectangle(brush, rect);
                    _bxBuffer.Graphics.DrawLine(pen2, rect.Left, rect.Top, rect.Left, rect.Bottom);
                    _bxBuffer.Graphics.DrawLine(pen2, rect.Left + 1, rect.Top, rect.Right, rect.Top);
                    _bxBuffer.Graphics.DrawLine(pen3, rect.Right, rect.Top, rect.Right, rect.Bottom);
                    _bxBuffer.Graphics.DrawLine(pen3, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
                }

                rect = new Rectangle(_textArea.Right, _yPadding + _scrollWidth, _scrollWidth,
                    Height - (_scrollWidth + _yPadding) * 2);
                array[0] = new PointF(rect.Left, rect.Top);
                array[1] = new PointF(rect.Right, rect.Top);
            }

            array[2] = new PointF(rect.Left + rect.Width / 2f, _yPadding);
            _bxBuffer.Graphics.FillPolygon(brush, array);
            _bxBuffer.Graphics.DrawLine(pen2, array[0], array[2]);
            _bxBuffer.Graphics.DrawLine(pen3, array[0], array[1]);
            array[0] = new PointF(rect.Left, rect.Bottom);
            array[1] = new PointF(rect.Right, rect.Bottom);
            array[2] = new PointF(rect.Left + rect.Width / 2f, checked(Height - _yPadding));
            _bxBuffer.Graphics.FillPolygon(brush, array);
            _bxBuffer.Graphics.DrawLine(pen2, array[0], array[1]);
            _bxBuffer.Graphics.DrawLine(pen3, array[2], array[1]);
        }

        private void DrawExpandButton()
        {
            if (!_canExpand | !IsExpanded & _scrollSteps < 1)
                return;
            var pen = new Pen(_scBarColor);
            var pen2 = new Pen(Color.FromArgb(96, 255, 255, 255), 1f);
            var pen3 = new Pen(Color.FromArgb(128, 0, 0, 0), 1f);
            var brush = new SolidBrush(_scButtonColor);
            var array = new PointF[3];
            var rectangle = checked(new Rectangle((int)Math.Round(Width / 3f), Height - (_scrollWidth + _yPadding),
                (int)Math.Round(Width / 3.0), _scrollWidth - _yPadding));
            if (IsExpanded)
            {
                array[0] = new PointF(rectangle.Left, rectangle.Bottom);
                array[1] = new PointF(rectangle.Right, rectangle.Bottom);
                array[2] = new PointF(rectangle.Left + rectangle.Width / 2f, rectangle.Top);
                _bxBuffer.Graphics.FillPolygon(brush, array);
                _bxBuffer.Graphics.DrawLine(pen2, array[0], array[2]);
                _bxBuffer.Graphics.DrawLine(pen3, array[0], array[1]);
                checked
                {
                    _bxBuffer.Graphics.DrawLine(pen, 0, 0, 0, Height - 1);
                    _bxBuffer.Graphics.DrawLine(pen, 0, Height - 1, Width - 1, Height - 1);
                    _bxBuffer.Graphics.DrawLine(pen, Width - 1, Height, Width - 1, 0);
                }
            }
            else
            {
                array[0] = new PointF(rectangle.Left, rectangle.Top);
                array[1] = new PointF(rectangle.Right, rectangle.Top);
                array[2] = new PointF(rectangle.Left + rectangle.Width / 2f, rectangle.Bottom);
                _bxBuffer.Graphics.FillPolygon(brush, array);
                _bxBuffer.Graphics.DrawLine(pen2, array[0], array[1]);
                _bxBuffer.Graphics.DrawLine(pen3, array[2], array[1]);
            }
        }

        private enum EMouseTarget
        {
            None,
            Item,
            UpButton,
            DownButton,
            ScrollBarUp,
            ScrollBarDown,
            ScrollBlock,
            ExpandButton
        }

        public class ListLabelItem
        {
            public readonly int IdxPower;
            public readonly int NIdPower;
            public readonly int NIdSet;
            private readonly string _sTag;
            public LlFontFlags FontFlags;
            public int Index;
            public int ItemHeight;
            public int LineCount;
            public string WrappedText;

            public ListLabelItem()
            {
                Text = "";
                WrappedText = "";
                ItemState = LlItemState.Enabled;
                FontFlags = LlFontFlags.Normal;
                TextAlign = LlTextAlign.Left;
                NIdSet = -1;
                IdxPower = -1;
                NIdPower = -1;
                _sTag = "";
                LineCount = 1;
                ItemHeight = 1;
                Index = -1;
            }

            public ListLabelItem(string iText, LlItemState iState, int inIdSet = -1, int iIdxPower = -1, int inIdPower = -1, string iStringTag = "", LlFontFlags iFont = LlFontFlags.Normal, LlTextAlign iAlign = LlTextAlign.Left)
            {
                Text = "";
                WrappedText = "";
                ItemState = LlItemState.Enabled;
                FontFlags = LlFontFlags.Normal;
                TextAlign = LlTextAlign.Left;
                NIdSet = -1;
                IdxPower = -1;
                NIdPower = -1;
                _sTag = "";
                LineCount = 1;
                ItemHeight = 1;
                Index = -1;
                Text = iText;
                ItemState = iState;
                NIdSet = inIdSet;
                IdxPower = iIdxPower;
                NIdPower = inIdPower;
                _sTag = iStringTag;
                FontFlags = iFont;
                TextAlign = iAlign;
            }

            public ListLabelItem(ListLabelItem template)
            {
                Text = "";
                WrappedText = "";
                ItemState = LlItemState.Enabled;
                FontFlags = LlFontFlags.Normal;
                TextAlign = LlTextAlign.Left;
                NIdSet = -1;
                IdxPower = -1;
                NIdPower = -1;
                _sTag = "";
                LineCount = 1;
                ItemHeight = 1;
                Index = -1;
                Text = template.Text;
                ItemState = template.ItemState;
                FontFlags = template.FontFlags;
                TextAlign = template.TextAlign;
                LineCount = template.LineCount;
                ItemHeight = template.ItemHeight;
                NIdSet = template.NIdSet;
                IdxPower = template.IdxPower;
                NIdPower = template.NIdPower;
                _sTag = template._sTag;
            }

            public string Text { get; set; }

            public LlItemState ItemState { get; set; }

            public bool Bold
            {
                get => (FontFlags & LlFontFlags.Bold) > LlFontFlags.Normal;
                set
                {
                    checked
                    {
                        if (value)
                        {
                            if ((~FontFlags & LlFontFlags.Bold) > LlFontFlags.Normal) FontFlags++;
                        }
                        else if ((FontFlags & LlFontFlags.Bold) > LlFontFlags.Normal)
                        {
                            FontFlags--;
                        }
                    }
                }
            }

            public bool Italic
            {
                get => (FontFlags & LlFontFlags.Italic) > LlFontFlags.Normal;
                set
                {
                    checked
                    {
                        if (value)
                        {
                            if ((~FontFlags & LlFontFlags.Italic) > LlFontFlags.Normal) FontFlags += 2;
                        }
                        else if ((FontFlags & LlFontFlags.Italic) > LlFontFlags.Normal)
                        {
                            FontFlags -= 2;
                        }
                    }
                }
            }

            public bool Underline
            {
                get => (FontFlags & LlFontFlags.Underline) > LlFontFlags.Normal;
                set
                {
                    checked
                    {
                        if (value)
                        {
                            if ((~FontFlags & LlFontFlags.Underline) > LlFontFlags.Normal) FontFlags += 4;
                        }
                        else if ((FontFlags & LlFontFlags.Underline) > LlFontFlags.Normal)
                        {
                            FontFlags -= 4;
                        }
                    }
                }
            }

            public bool Strikethrough
            {
                get => (FontFlags & LlFontFlags.Strikethrough) > LlFontFlags.Normal;
                set
                {
                    checked
                    {
                        if (value)
                        {
                            if ((~FontFlags & LlFontFlags.Strikethrough) > LlFontFlags.Normal) FontFlags += 8;
                        }
                        else if ((FontFlags & LlFontFlags.Strikethrough) > LlFontFlags.Normal)
                        {
                            FontFlags -= 8;
                        }
                    }
                }
            }

            public LlTextAlign TextAlign { get; set; }
        }
    }
}
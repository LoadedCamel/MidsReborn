using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.UI.Controls
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

        private BufferedGraphics? _buffer;
        private BufferedGraphicsContext? _bufferContext;

        private List<ListLabelItem> _items;

        private Color[] _colors;
        private Cursor[] _cursors;
        private bool[] _highlightOn;
        private Color _bgColor;
        private Color _hvrColor;

        private bool _canExpand;
        private bool _canScroll;
        private bool _disableEvents;
        private bool _disableRedraw;
        private bool _dragMode;
        private int _expandMaxY;

        private Rectangle _expandButtonRect;
        private Rectangle _scrollUpButtonRect;
        private Rectangle _scrollDownButtonRect;
        private Rectangle _scrollThumbRect;
        private Rectangle _scrollRailRect;

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

        public ListLabelItem[] Items => _items.ToArray();

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
                if (!_canExpand)
                {
                    Expand(true); // Force full expansion
                }
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

        private void InitBuffer()
        {
            if (_disableRedraw || Width <= 0 || Height <= 0)
                return;

            _buffer?.Dispose(); // Dispose the previous buffer if it exists

            _bufferContext ??= BufferedGraphicsManager.Current;

            // Allocate a new buffer for the current size
            _buffer = _bufferContext.Allocate(CreateGraphics(), ClientRectangle);

            // Set high-quality rendering options
            _buffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _buffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _buffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _buffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!IsDesignMode)
            {
                SafeInitialize(); // move this here instead of constructor
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _buffer?.Dispose();
            _buffer = null;
            base.OnHandleDestroyed(e);
        }

        private void SafeInitialize()
        {
            MouseLeave += ListLabel_MouseLeave;
            MouseMove += ListLabel_MouseMove;
            MouseUp += ListLabel_MouseUp;
            Resize += ListLabel_Resize;
            FontChanged += ListLabel_FontChanged;
            Load += ListLabel_Load;
            MouseDown += ListLabel_MouseDown;
            MouseWheel += ListLabel_MouseWheel;
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
                return;

            if (_buffer is null)
                InitBuffer();

            if (_buffer?.Graphics is not { } g)
                return;

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
                        var text2 = $"{text}{((i == 0) ? " " : "")}{_items[index].Text[i]}";
                        var measure = _items[index].ItemState == LlItemState.Heading
                            ? $"~ {text2.TrimEnd()}... ~"
                            : $"{text2.TrimEnd()}...";

                        var tw = (int)Math.Ceiling(g.MeasureString(measure, font, layoutArea, stringFormat).Width);
                        if (tw <= _textArea.Width)
                            text = text2;
                        else
                        {
                            fullFit = false;
                            break;
                        }
                    }

                    if (!fullFit)
                        text = $"{text.TrimEnd()}...";

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
                            if (Math.Ceiling(g.MeasureString(text2, font, layoutArea, stringFormat).Width) > _textArea.Width)
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
                            var tw = (int)Math.Ceiling(g.MeasureString(
                                _items[index].ItemState == LlItemState.Heading
                                    ? $"~ {text2}... ~"
                                    : text2, font, layoutArea, stringFormat).Width);

                            if (tw <= _textArea.Width)
                                text = text2;
                            else
                            {
                                fullFit = false;
                                break;
                            }
                        }

                        if (!fullFit && k < strWords.Length)
                        {
                            fullFit = true;
                            for (var i = 0; i < strWords[k].Length; i++)
                            {
                                var text2 = $"{text}{((i == 0) ? " " : "")}{strWords[k][i]}";
                                var tw = (int)Math.Ceiling(g.MeasureString(
                                    _items[index].ItemState == LlItemState.Heading
                                        ? $"~ {text2.TrimEnd()}... ~"
                                        : $"{text2.TrimEnd()}...", font, layoutArea, stringFormat).Width);

                                if (tw <= _textArea.Width)
                                    text = text2;
                                else
                                {
                                    fullFit = false;
                                    break;
                                }
                            }

                            if (!fullFit)
                                text = $"{text.TrimEnd()}...";
                        }

                        if (_items[index].ItemState == LlItemState.Heading)
                            text = $"~ {text} ~";

                        break;
                }

                _items[index].WrappedText = text;
            }

            _items[index].WrappedText = _items[index].WrappedText.Replace("  ", " ");
            if (_items[index].ItemState == LlItemState.Heading && !_items[index].WrappedText.StartsWith("~"))
            {
                _items[index].WrappedText = $"~ {_items[index].WrappedText} ~";
            }

            _items[index].LineCount = num;
            _items[index].ItemHeight = num * (ActualLineHeight - _yPadding * 2) + _yPadding * 2;
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
            if (_canExpand && _expandButtonRect.Contains(x, y))
                return EMouseTarget.ExpandButton;

            if (_scrollSteps > 0)
            {
                if (_scrollUpButtonRect.Contains(x, y)) return EMouseTarget.UpButton;
                if (_scrollDownButtonRect.Contains(x, y)) return EMouseTarget.DownButton;
                if (_scrollThumbRect.Contains(x, y)) return EMouseTarget.ScrollBlock;

                if (_scrollRailRect.Contains(x, y))
                {
                    if (y < _scrollThumbRect.Top) return EMouseTarget.ScrollBarUp;
                    if (y > _scrollThumbRect.Bottom) return EMouseTarget.ScrollBarDown;
                }
            }

            if (_textArea.Contains(x, y))
                return EMouseTarget.Item;

            return EMouseTarget.None;
        }

        private void ListLabel_MouseWheel(object? sender, MouseEventArgs e)
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

        private void ListLabel_MouseDown(object? sender, MouseEventArgs e)
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

        private void ListLabel_MouseLeave(object? sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            _lastMouseMoveTarget = EMouseTarget.None;
            HoverId = -1;
            Draw();
            EmptyHover?.Invoke();
        }

        private void ListLabel_MouseMove(object? sender, MouseEventArgs e)
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

        private void ListLabel_MouseUp(object? sender, MouseEventArgs e)
        {
            _dragMode = false;
            if (Cursor == Cursors.SizeNS) Cursor = Cursors.Default;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DesignMode)
            {
                DrawDesignTimePlaceholder(e.Graphics);
                return;
            }

            if (_buffer is null)
            {
                Draw(); // ensures buffer is initialized and populated
            }

            _buffer?.Render(e.Graphics);
        }

        private void DrawDesignTimePlaceholder(Graphics g)
        {
            using var bgBrush = new SolidBrush(Color.FromArgb(80, 0, 120, 215));
            g.FillRectangle(bgBrush, ClientRectangle);

            using var borderPen = new Pen(Color.Gray, 1);
            borderPen.DashStyle = DashStyle.Dash;
            g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

            string label = string.IsNullOrWhiteSpace(Name)
                ? "ListLabel (Design Time)"
                : $"{Name}";

            using var font = new Font("Segoe UI", 9f, FontStyle.Italic);
            using var textBrush = new SolidBrush(Color.White);

            SizeF textSize = g.MeasureString(label, font);
            PointF center = new((Width - textSize.Width) / 2f, (Height - textSize.Height) / 2f);
            g.DrawString(label, font, textBrush, center);
        }

        private void ListLabel_Resize(object? sender, EventArgs e)
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

        private void ListLabel_FontChanged(object? sender, EventArgs e)
        {
            Recalculate();
            Draw();
        }

        private void ListLabel_Load(object? sender, EventArgs e)
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
            if (IsDisposed || _disableRedraw || !Visible || Width <= 0 || Height <= 0)
                return;

            InitBuffer(); // ensures _buffer is created and sized correctly

            if (_buffer is null)
                return;

            var g = _buffer.Graphics;
            g.Clear(IsExpanded ? Color.Black : BackColor);

            // Draw all items from scroll offset
            for (var i = _scrollOffset; i < _items.Count; i++)
            {
                DrawItem(i); // Must now use _buffer.Graphics internally
            }

            DrawScrollBar();     // Draw scrollbar onto _buffer.Graphics
            DrawExpandButton();  // Draw expand button onto _buffer.Graphics

            // Push the completed buffer to the screen
            using var screenGraphics = CreateGraphics();
            _buffer.Render(screenGraphics);
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
                    _buffer?.Graphics.FillRectangle(brush, rectangle);
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
                        _buffer?.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.Y--;
                        _buffer?.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.X++;
                        _buffer?.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.X++;
                        _buffer?.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.Y++;
                        _buffer?.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.Y++;
                        _buffer?.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.X--;
                        _buffer?.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                        r.X--;
                        _buffer?.Graphics.DrawString(tl, font, outlineBrush, r, stringFormat);
                    }

                    _buffer?.Graphics.DrawString(tl, font, new SolidBrush(_colors[(int)_items[index].ItemState]), rectangle, stringFormat);
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

            var pen = new Pen(_scBarColor);
            var pen2 = new Pen(Color.FromArgb(96, 255, 255, 255));
            var pen3 = new Pen(Color.FromArgb(128, 0, 0, 0));
            var brush = new SolidBrush(_scButtonColor);

            int barX = _textArea.Right;
            int upY = _yPadding;
            int downY = Height - _scrollWidth - _yPadding;
            int centerX = barX + _scrollWidth / 2;

            // ▲ Up arrow
            _scrollUpButtonRect = new Rectangle(barX, upY, _scrollWidth, _scrollWidth);
            PointF[] upTriangle =
            [
                new(centerX, upY),
                new(barX, upY + _scrollWidth),
                new(barX + _scrollWidth, upY + _scrollWidth)
            ];
            _buffer?.Graphics.FillPolygon(brush, upTriangle);
            _buffer?.Graphics.DrawPolygon(pen3, upTriangle);

            // ▼ Down arrow
            _scrollDownButtonRect = new Rectangle(barX, downY, _scrollWidth, _scrollWidth);
            PointF[] downTriangle =
            [
                new(centerX, downY + _scrollWidth),
                new(barX, downY),
                new(barX + _scrollWidth, downY)
            ];
            _buffer?.Graphics.FillPolygon(brush, downTriangle);
            _buffer?.Graphics.DrawPolygon(pen3, downTriangle);

            // Scroll thumb
            int scrollAreaHeight = Height - (_yPadding + _scrollWidth) * 2 - _yPadding * 2;
            int thumbTop = (int)Math.Round(_scrollWidth + _yPadding * 2 +
                                           scrollAreaHeight * (_scrollOffset / (double)_scrollSteps));
            int thumbHeight = Math.Max(10, (int)Math.Round(scrollAreaHeight / (double)_scrollSteps));
            _scrollThumbRect = new Rectangle(barX, thumbTop, _scrollWidth, thumbHeight);

            _buffer?.Graphics.FillRectangle(brush, _scrollThumbRect);
            _buffer?.Graphics.DrawLine(pen2, _scrollThumbRect.Left, _scrollThumbRect.Top, _scrollThumbRect.Left,
                _scrollThumbRect.Bottom);
            _buffer?.Graphics.DrawLine(pen2, _scrollThumbRect.Left + 1, _scrollThumbRect.Top, _scrollThumbRect.Right,
                _scrollThumbRect.Top);
            _buffer?.Graphics.DrawLine(pen3, _scrollThumbRect.Right, _scrollThumbRect.Top, _scrollThumbRect.Right,
                _scrollThumbRect.Bottom);
            _buffer?.Graphics.DrawLine(pen3, _scrollThumbRect.Left + 1, _scrollThumbRect.Bottom, _scrollThumbRect.Right - 1,
                _scrollThumbRect.Bottom);

            // Scroll rail
            int railTop = _scrollUpButtonRect.Bottom + _yPadding;
            int railBottom = _scrollDownButtonRect.Top - _yPadding;
            _scrollRailRect = new Rectangle(barX, railTop, _scrollWidth, railBottom - railTop);
            _buffer?.Graphics.DrawLine(pen, centerX, railTop, centerX, railBottom);
        }

        private void DrawExpandButton()
        {
            if (!_canExpand | !IsExpanded & _scrollSteps < 1)
                return;

            var pen = new Pen(_scBarColor);
            var pen2 = new Pen(Color.FromArgb(96, 255, 255, 255));
            var pen3 = new Pen(Color.FromArgb(128, 0, 0, 0));
            var brush = new SolidBrush(_scButtonColor);

            _expandButtonRect = new Rectangle(
                (int)(Width / 3f),
                Height - (_scrollWidth + _yPadding),
                (int)(Width / 3.0),
                _scrollWidth - _yPadding
            );

            PointF[] triangle = IsExpanded
                ? [new PointF(_expandButtonRect.Left, _expandButtonRect.Bottom),
                    new PointF(_expandButtonRect.Right, _expandButtonRect.Bottom),
                    new PointF(_expandButtonRect.Left + _expandButtonRect.Width / 2f, _expandButtonRect.Top)]
                : [new PointF(_expandButtonRect.Left, _expandButtonRect.Top),
                    new PointF(_expandButtonRect.Right, _expandButtonRect.Top),
                    new PointF(_expandButtonRect.Left + _expandButtonRect.Width / 2f, _expandButtonRect.Bottom)];

            _buffer?.Graphics.FillPolygon(brush, triangle);
            _buffer?.Graphics.DrawLine(pen2, triangle[0], triangle[2]);
            _buffer?.Graphics.DrawLine(pen3, triangle[2], triangle[1]);

            _buffer?.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
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
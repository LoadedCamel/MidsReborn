using Mids_Reborn.Core.Utils;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls.Skia;

[DesignerCategory("Code")]
[ToolboxItem(true)]
public class SkPairedList : SKControl
{
    #region Events
    public delegate void ItemClickEventHandler(SkListItem item, MouseButtons button);
    public delegate void ItemHoverEventHandler(SkListItem item);
    public delegate void EmptyHoverEventHandler();

    public event ItemClickEventHandler? ItemClick;
    public event ItemHoverEventHandler? ItemHover;
    public event EmptyHoverEventHandler? EmptyHover;
    #endregion

    #region Enums
    private enum SkMouseTarget
    {
        None,
        Item,
        UpArrow,
        DownArrow,
        ScrollThumb,
        ScrollTrackUp,
        ScrollTrackDown
    }
    #endregion

    #region Structs

    private struct ItemContainerInfo
    {
        public int MinX;
        public int MaxX;
        public int ItemWidth;
    }
    #endregion

    #region Private fields
    private SkMouseTarget _lastMouseTarget = SkMouseTarget.None;

    private readonly List<SkListItem> _items = [];

    private const int ScrollBarGutter = 4;
    private const int MultilineTextInterline = 4;
    private const int BottomVisualPadding = 10;

    private int _scrollBarWidth = 11;
    private int _scrollOffset;
    private int _scrollSteps;
    private int _hoverIndex = -1;
    private int _dragStartY;
    private int _dragScrollOffset;
    // Bug: Cannot modify paddingX, paddingY from a form designer, default value takes priority
    private int _paddingX = 4;
    private int _paddingY = 1;
    private int _visibleLineCount;
    private int _selectedIndex = -1;
    private int _columns = 2;

    private bool _suspendRedraw;
    private bool _scrollable = true;
    private bool _draggingThumb;
    private bool _highVis = true;
    private bool _hoveringArrowUp;
    private bool _hoveringArrowDown;
    private bool _hoveringExpandToggle;
    private bool _pressingArrowUp;
    private bool _pressingArrowDown;

    private bool _autoColumns = false;
    private int _minColumnWidth = 180;

    private SKRect _textArea;
    private SKRect _thumbRect;
    private SKRect _arrowUpRect;
    private SKRect _arrowDownRect;

    private Color _scrollBarColor = Color.FromArgb(128, 96, 192);
    private Color _scrollButtonColor = Color.FromArgb(96, 0, 192);
    private Color _hoverColor = Color.WhiteSmoke;
    private Color _bgColor = Color.Black;

    private readonly bool[] _highlightOn =
    [
        true, true, true, true, true, true  // Matches Enabled through Heading
    ];

    private static readonly Color[] StateColors =
    [
        Color.LightBlue, Color.LightGreen, Color.Gray,
        Color.DarkGreen, Color.Red, Color.Orange
    ];

    private bool IsInDesignMode =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
        DesignMode || Site?.DesignMode == true;

    public override Font Font =>
        IsInDesignMode
            ? base.Font
            : new Font(Fonts.Family(@"Noto Sans"), base.Font.Size, base.Font.Style, GraphicsUnit.Pixel);

    public float FontSize
    {
        set
        {
            Font = new Font(Font.FontFamily, value, Font.Style, GraphicsUnit.Pixel);
            OnFontChanged(this, EventArgs.Empty);
        }
    }
    #endregion

    public SkPairedList()
    {
        if (IsInDesignMode)
        {
            return;
        }

        MouseMove += OnMouseMove;
        MouseLeave += OnMouseLeave;
        MouseDown += OnMouseDown;
        MouseUp += OnMouseUp;
        MouseWheel += OnMouseWheel;
        Resize += OnResize;
        FontChanged += OnFontChanged;
        PaintSurface += OnPaintSurface;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (IsInDesignMode)
        {
            return;
        }

        SuspendRedraw = true;
        RecalculateLayout();
        SuspendRedraw = false;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        SuspendRedraw = true;
        RecalculateLayout();
        SuspendRedraw = false;
    }

    #region Designer-visible properties
    [Browsable(true)]
    public int Columns
    {
        get
        {
            if (!_autoColumns)
            {
                return _columns;
            }

            var containerInfo = CalcItemContainerInfo(true);

            return Math.Max(2, (int)Math.Floor((containerInfo.MaxX - containerInfo.MinX) / (float)(_minColumnWidth + PaddingX)));
        }
        set
        {
            if (value < 2)
            {
                return;
            }

            _columns = value;
            RecalculateLayout();
            Invalidate();
        }
    }

    [Browsable(true)]
    public bool Scrollable
    {
        get => _scrollable;
        set
        {
            _scrollable = value;
            Invalidate();
        }
    }

    [Browsable(true)]
    public bool SuspendRedraw
    {
        get => _suspendRedraw;
        set
        {
            _suspendRedraw = value;
            if (value)
            {
                return;
            }

            RecalculateLayout(); // Just reflow
            Invalidate(); // Trigger paint
        }
    }

    [Browsable(true)]
    public bool HighVis
    {
        get => _highVis;
        set
        {
            _highVis = value;
            Invalidate();
        }
    }

    [Browsable(true)]
    public Color HoverColor
    {
        get => _hoverColor;
        set
        {
            _hoverColor = value;
            Invalidate();
        }
    }

    [Browsable(true)]
    public int PaddingX
    {
        get => _paddingX;
        set
        {
            if (value < 0 || value * 2 >= Width - 5)
            {
                return;
            }

            _paddingX = value;
            Invalidate(); // Just redraw
        }
    }

    [Browsable(true)]
    public int PaddingY
    {
        get => _paddingY;
        set
        {
            if (value < 0 || !(value < Height / 3.0))
            {
                return;
            }

            _paddingY = value;
            SetLineHeight(); // Only update line height
            Invalidate();    // Redraw only
        }
    }

    [Browsable(true)]
    public override Color BackColor
    {
        get => _bgColor;
        set
        {
            _bgColor = value;
            Invalidate();
        }
    }

    [Browsable(true)]
    public int ScrollBarWidth
    {
        get => _scrollBarWidth;
        set
        {
            if (value <= 0 || value >= Width / 2)
            {
                return;
            }

            _scrollBarWidth = value;
            RecalculateLayout();
            Invalidate();
        }
    }

    [Browsable(true)]
    public Color ScrollBarColor
    {
        get => _scrollBarColor;
        set
        {
            _scrollBarColor = value;
            Invalidate();
        }
    }

    [Browsable(true)]
    public Color ScrollButtonColor
    {
        get => _scrollButtonColor;
        set
        {
            _scrollButtonColor = value;
            Invalidate();
        }
    }

    [Browsable(true)]
    public bool AutoColumns
    {
        get => _autoColumns;
        set
        {
            _autoColumns = value;
            Invalidate();
        }
    }

    [Browsable(true)]
    public int MinColumnWidth
    {
        get => _minColumnWidth;
        set
        {
            _minColumnWidth = value;
            Invalidate();
        }
    }
    #endregion

    #region Other public properties
    [Browsable(false)]
    public SkListItem[] Items => _items.ToArray();

    [Browsable(false)]
    public SkListItem SelectedItem
    {
        get => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : new SkListItem();
        set
        {
            if (_selectedIndex < 0 || _selectedIndex >= _items.Count)
            {
                return;
            }

            _items[_selectedIndex] = new SkListItem(value);
            Invalidate();
        }
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value < 0 || value >= _items.Count)
            {
                return;
            }

            _selectedIndex = value;
            Invalidate();
        }
    }

    [Browsable(false)]
    public int ActualLineHeight { get; set; } = 8;
    #endregion

    #region Items operations
    public void AddItem(SkListItem item)
    {
        item.Index = _items.Count;
        _items.Add(item);
        WrapText(item);
        _scrollSteps = GetScrollSteps();

        if (!SuspendRedraw)
        {
            Invalidate();
        }
    }

    public void ClearItems()
    {
        foreach (var item in _items)
        {
            item.Dispose();
        }

        _items.Clear();
        _hoverIndex = -1;
        _scrollOffset = 0;
        _scrollSteps = 0;

        if (!SuspendRedraw)
        {
            Invalidate();
        }
    }
    #endregion

    public void UpdateTextColors(EItemState state, Color color)
    {
        if ((int)state < 0 || (int)state >= StateColors.Length)
        {
            return;
        }

        StateColors[(int)state] = color;
        Invalidate();
    }

    private void RecalculateLayout()
    {
        if (_items.Count == 0)
        {
            return;
        }

        var fullRect = new SKRect(_paddingX, 0, Width - _paddingX * 2, Height - BottomVisualPadding);
        RecalculateLines(fullRect);

        var totalContentHeight = GetRealTotalHeight();
        var needsScroll = totalContentHeight > Height - BottomVisualPadding;

        _scrollSteps = needsScroll ? GetScrollSteps() : 0;

        if (needsScroll)
        {
            var scrollHeight = Height - (_scrollBarWidth + _paddingY + BottomVisualPadding);
            var scrollRect = new SKRect(_paddingX, 0, Width - _paddingX * 2, scrollHeight);
            RecalculateLines(scrollRect);
        }

        if (_scrollSteps > 0)
        {
            var narrowRect = new SKRect(_paddingX, 0, Width - (_paddingX * 2 + _scrollBarWidth), Height - BottomVisualPadding);
            RecalculateLines(narrowRect);
        }

        if (!SuspendRedraw)
        {
            Invalidate();
        }
    }

    private ItemContainerInfo CalcItemContainerInfo(bool realColumns = false)
    {
        var minX = PaddingX;
        var maxX = _scrollable
            ? (int)_textArea.Right - ScrollBarGutter
            : Width - PaddingX;
        var columns = realColumns ? _columns : Columns; // Prevent infinite recursion when calculating Columns
        var itemWidth = (int)Math.Round((maxX - minX - PaddingX * (columns - 1)) / (float)columns);

        return new ItemContainerInfo
        {
            MinX = minX,
            MaxX = maxX,
            ItemWidth = itemWidth
        };
    }

    private void RecalculateLines(SKRect rect)
    {
        _textArea = rect;

        SetLineHeight();

        foreach (var item in _items)
        {
            WrapText(item);
        }

        GetTotalLineCount();
        _scrollSteps = GetScrollSteps();
        Invalidate();
    }

    private void SetLineHeight()
    {
        var tempItem = new SkListItem { FontFlags = (EFontFlags)(int)Font.Style };
        var skFont = tempItem.GetOrCreateFont(Font);
        var metrics = skFont.Metrics;

        // True total height = |Ascent| + |Descent| + |Leading|
        var ascent = Math.Abs(metrics.Ascent);
        var descent = Math.Abs(metrics.Descent);
        var leading = Math.Abs(metrics.Leading);

        var fullHeight = ascent + descent + leading;

        // Apply vertical padding
        ActualLineHeight = (int)Math.Ceiling(fullHeight) + PaddingY * 2;

        _visibleLineCount = GetVisibleLineCount();
    }

    private void WrapText(SkListItem item)
    {
        if (string.IsNullOrEmpty(item.Text))
        {
            return;
        }

        var baseText = item.Text.Trim("~ ".ToCharArray());

        var font = item.GetOrCreateFont(Font);
        var containerInfo = CalcItemContainerInfo();
        var availableTotalWidth = containerInfo.MaxX - containerInfo.MinX;
        var maxItemWidth = (availableTotalWidth - PaddingX * Columns) / Columns;

        // Apply prefix/suffix only once if heading
        baseText = item.ItemState == EItemState.Heading
            ? $"~_{baseText}_~"
            : baseText;

        var words = baseText.Split(' ');
        var lines = new List<string>();
        var currentLine = words[0];

        for (var i = 1; i < words.Length; i++)
        {
            var testLine = $"{currentLine} {words[i]}";
            if (font.MeasureText(testLine) > maxItemWidth)
            {
                lines.Add(currentLine.Replace("~_", "~ ").Replace("_~", " ~"));
                currentLine = words[i];
            }
            else
            {
                currentLine = testLine;
            }
        }

        // Last line
        lines.Add(currentLine.Replace("~_", "~ ").Replace("_~", " ~"));

        item.WrappedText = string.Join('\n', lines);
        item.LineCount = lines.Count;

        // Layout math
        item.ItemHeight = lines.Count * (ActualLineHeight - PaddingY * 2) + PaddingY * 2;
    }

    private int GetLineHeight(int n, bool isLineNumber = false)
    {
        var lineLowerBound = isLineNumber
            ? n * Columns
            : n - n % Columns; // Most left-side cell on same line
        var lineUpperBound = Math.Max(0, Math.Min(_items.Count, lineLowerBound + Columns) - 1);
        if (_items.Count <= 0 || lineLowerBound < 0 || lineLowerBound >= _items.Count || lineUpperBound >= _items.Count || lineLowerBound > lineUpperBound)
        {
            return 0;
        }
        
        var lineHeight = lineLowerBound == lineUpperBound
            ? _items[lineLowerBound].ItemHeight
            : _items[lineLowerBound..lineUpperBound].Select(e => e.ItemHeight).Max();

        return lineHeight;
    }

    private int GetRealTotalHeight() => _items.Sum(e => e.ItemHeight); // NOTE: PaddingY not included

    private int GetTotalLineCount()
    {
        var lines = 0;
        for (var i = 0; i < _items.Count; i += Columns)
        {
            lines += GetLineHeight(i);;
        }

        return lines;
    }

    private int GetVisibleLineCount()
    {
        if (!_scrollable)
        {
            _scrollSteps = 0;

            return 0;
        }

        var y = PaddingY;
        var visibleLines = 0;

        for (var i = 0; i < _items.Count; i += Columns)
        {
            var lineItemsHeight = GetLineHeight(i);
            var nextY = y + lineItemsHeight;
            if (nextY > Height - BottomVisualPadding)
            {
                break; // Cut-off detected
            }

            visibleLines += lineItemsHeight;
            y = nextY + PaddingY;
        }

        return visibleLines;
    }

    private int GetScrollSteps()
    {
        if (!_scrollable)
        {
            _scrollSteps = 0;
            return 0;
        }

        var lineSum = 0;
        var wrapCount = 0;

        for (var i = 0; i < _items.Count; i += Columns)
        {
            lineSum += GetLineHeight(i);
            if (lineSum > _visibleLineCount)
            {
                wrapCount++;
            }
        }

        // Add one more scroll page if needed
        if (wrapCount > 0)
        {
            wrapCount++;
        }

        _scrollSteps = wrapCount <= 1 ? 0 : wrapCount;

        return _scrollSteps;
    }

    private static SKColor ToSkColor(Color c) => new(c.R, c.G, c.B, c.A);

    #region Paint-specific event handlers
    protected override void OnPaint(PaintEventArgs e)
    {
        if (IsInDesignMode)
        {
            using var background = new SolidBrush(Color.FromArgb(255, 30, 30, 30));
            e.Graphics.FillRectangle(background, ClientRectangle);

            using var pen = new Pen(Color.DodgerBlue);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            using var font = new Font("Segoe UI", 9, FontStyle.Bold);
            var designerLabel = $"{(string.IsNullOrWhiteSpace(Name) ? $"{GetType().Name}" : $"{Name}\r\n{GetType().Name}")} (Design)";
            e.Graphics.DrawString(designerLabel, font, Brushes.LightGray, Width / 2f, Height / 2f, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            return;
        }

        base.OnPaint(e); // Will crash the designer if this is called
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        if (SuspendRedraw || !IsHandleCreated || Width <= 0 || Height <= 0)
        {
            return;
        }

        var canvas = e.Surface.Canvas;
        canvas.Clear(ToSkColor(BackColor));

        //var width = e.BackendRenderTarget.Width;
        var containerInfo = CalcItemContainerInfo();
        var height = e.Info.Height;
        var x = containerInfo.MinX;
        var y = PaddingY;
        var n = _scrollOffset * Columns;
        //var row = _scrollOffset;
        var col = 0;
        while (n < _items.Count)
        {
            if (y > height)
            {
                break;
            }

            DrawItem(canvas, _items[n], x, y, containerInfo.ItemWidth);
            
            n++;
            col++;
            x = containerInfo.MinX + (containerInfo.ItemWidth + PaddingX) * col;

            if (col < Columns)
            {
                continue;
            }

            x = containerInfo.MinX;
            //row++;
            col = 0;
            y += GetLineHeight(n - 1) + PaddingY;
        }

        DrawScrollbar(canvas);
    }

    private void DrawItem(SKCanvas canvas, SkListItem item, float x, float y, int width)
    {
        var lines = item.WrappedText?.Split('\n') ?? [];
        if (lines.Length == 0)
        {
            return;
        }

        var font = item.GetOrCreateFont(Font);
        var color = ToSkColor(StateColors[(int)item.ItemState]);
        var metrics = font.Metrics;
        var textHeight = font.Size;

        x = item.TextAlign switch
        {
            ETextAlign.Center => x + width / 2f,
            ETextAlign.Right => x + width,
            _ => x
        };

        if (item.Index == _hoverIndex && _highlightOn[(int)item.ItemState])
        {
            var col = _hoverIndex % Columns;
            var containerInfo = CalcItemContainerInfo();
            // Expand hover rectangle a bit on the left
            // Expand hover rectangle a bit on the right (except for right-most column, if scrollable)
            var hx = containerInfo.MinX - 4 + (containerInfo.ItemWidth + PaddingX) * col;
            var hoverRect = new SKRect(hx, y, hx + containerInfo.ItemWidth + ((col < Columns - 1) | !_scrollable ? 4 : 0), y + item.ItemHeight);
            using var bg = new SKPaint { Color = ToSkColor(_hoverColor), Style = SKPaintStyle.Fill };
            canvas.DrawRect(hoverRect, bg);
        }

        var lineY = y + PaddingY;
        var interlineSpacing = font.Size + MultilineTextInterline;

        foreach (var line in lines)
        {
            var text = line.Trim();

            if (_highVis)
            {
                using var outline = new SKPaint
                {
                    Color = SKColors.Black,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1,
                    IsAntialias = true
                };

                for (var dx = -1; dx <= 1; dx++)
                {
                    for (var dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        canvas.DrawText(text, x + dx, lineY + textHeight + dy, font, outline);
                    }
                }
            }

            using var paint = new SKPaint { Color = color, IsAntialias = true };
            canvas.DrawText(text, new SKPoint(x, lineY + textHeight), item.TextAlign == ETextAlign.Center ? SKTextAlign.Center : SKTextAlign.Left, font, paint);

            if (item.FontFlags.HasFlag(EFontFlags.Underline))
            {
                var underlineY = lineY + textHeight + (metrics.UnderlinePosition ?? 1);
                using var ul = new SKPaint
                {
                    Color = color,
                    StrokeWidth = metrics.UnderlineThickness ?? 1,
                    IsAntialias = true
                };
                var measured = font.MeasureText(text);
                canvas.DrawLine(x, underlineY, x + measured, underlineY, ul);
            }

            if (item.FontFlags.HasFlag(EFontFlags.Strikethrough))
            {
                var strikeY = lineY + textHeight / 2f;
                using var sl = new SKPaint
                {
                    Color = color,
                    StrokeWidth = 1,
                    IsAntialias = true
                };
                var measured = font.MeasureText(text);
                canvas.DrawLine(x, strikeY, x + measured, strikeY, sl);
            }

            lineY += interlineSpacing;
        }
    }

    private void DrawScrollbar(SKCanvas canvas)
    {
        if (_scrollBarWidth < 1 || !_scrollable || _scrollSteps < 1)
        {
            return;
        }

        var trackLeft = (int)(_textArea.Right + _scrollBarWidth / 2f);
        var trackTop = PaddingY + _scrollBarWidth;
        var trackBottom = Height - (_scrollBarWidth + PaddingY);

        using var trackPen = new SKPaint
        {
            Color = ToSkColor(_scrollBarColor),
            StrokeWidth = 1,
            IsAntialias = false
        };
        canvas.DrawLine(trackLeft, trackTop, trackLeft, trackBottom, trackPen);

        // Base brush for arrows and thumb
        using var fillBrush = new SKPaint
        {
            Color = ToSkColor(_scrollButtonColor),
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        // Scroll thumb
        var usableHeight = Height - (_scrollBarWidth + PaddingY) * 2 - PaddingY * 2;
        var thumbY = (int)Math.Round(_scrollBarWidth + PaddingY * 2 + usableHeight / (double)_scrollSteps * _scrollOffset);
        var thumbHeight = (int)Math.Round(usableHeight / (double)_scrollSteps);

        _thumbRect = new SKRect(_textArea.Right, thumbY, _textArea.Right + _scrollBarWidth, thumbY + thumbHeight);
        canvas.DrawRect(_thumbRect, fillBrush);

        // Thumb shading
        using var highlight = new SKPaint { Color = new SKColor(255, 255, 255, 96), StrokeWidth = 1 };
        using var shadow = new SKPaint { Color = new SKColor(0, 0, 0, 128), StrokeWidth = 1 };

        canvas.DrawLine(_thumbRect.Left, _thumbRect.Top, _thumbRect.Left, _thumbRect.Bottom, highlight);
        canvas.DrawLine(_thumbRect.Left + 1, _thumbRect.Top, _thumbRect.Right, _thumbRect.Top, highlight);
        canvas.DrawLine(_thumbRect.Right, _thumbRect.Top, _thumbRect.Right, _thumbRect.Bottom, shadow);
        canvas.DrawLine(_thumbRect.Left + 1, _thumbRect.Bottom, _thumbRect.Right - 1, _thumbRect.Bottom, shadow);

        // Arrow track region
        var arrowTrack = new SKRect(
            _textArea.Right,
            PaddingY + _scrollBarWidth,
            _textArea.Right + _scrollBarWidth,
            Height - (_scrollBarWidth + PaddingY)
        );

        // Up arrow
        var upArrow = new SKPoint[]
        {
            new(_textArea.Right, arrowTrack.Top),
            new(_textArea.Right + _scrollBarWidth, arrowTrack.Top),
            new(_textArea.Right + _scrollBarWidth / 2f, PaddingY)
        };

        _arrowUpRect = new SKRect(upArrow[0].X, PaddingY, upArrow[1].X, arrowTrack.Top);
        canvas.DrawVertices(SKVertexMode.Triangles, upArrow, null, fillBrush);

        // Arrow highlight and shadow
        canvas.DrawLine(upArrow[0], upArrow[2], highlight);
        canvas.DrawLine(upArrow[0], upArrow[1], shadow);

        // Down arrow
        var downArrow = new SKPoint[]
        {
            new(_textArea.Right, arrowTrack.Bottom),
            new(_textArea.Right + _scrollBarWidth, arrowTrack.Bottom),
            new(_textArea.Right + _scrollBarWidth / 2f, Height - PaddingY)
        };

        _arrowDownRect = new SKRect(downArrow[0].X, arrowTrack.Bottom, downArrow[1].X, Height - PaddingY);
        canvas.DrawVertices(SKVertexMode.Triangles, downArrow, null, fillBrush);

        // Arrow highlight and shadow
        canvas.DrawLine(downArrow[0], downArrow[1], highlight);
        canvas.DrawLine(downArrow[2], downArrow[1], shadow);
    }
    #endregion

    private int GetItemAtXY(int x, int y)
    {
        var containerInfo = CalcItemContainerInfo();
        var targetX = containerInfo.MinX;
        var targetY = PaddingY;
        var n = _scrollOffset * Columns;
        var row = _scrollOffset;
        var col = 0;
        while (n < _items.Count)
        {
            var rect = new Rectangle(targetX, targetY, containerInfo.ItemWidth, _items[n].ItemHeight);
            if (rect.Contains(x, y))
            {
                return n;
            }

            n++;
            col++;
            targetX = containerInfo.MinX + (containerInfo.ItemWidth + PaddingX) * col;

            if (col < Columns)
            {
                continue;
            }

            targetX = containerInfo.MinX;
            row++;
            col = 0;
            targetY += GetLineHeight(n - 1) + PaddingY;
        }

        return -1;
    }

    private SkMouseTarget GetMouseTarget(int x, int y)
    {
        var pt = new SKPoint(x, y);

        if (_arrowUpRect.Contains(pt))
        {
            return SkMouseTarget.UpArrow;
        }

        if (_arrowDownRect.Contains(pt))
        {
            return SkMouseTarget.DownArrow;
        }

        if (_thumbRect.Contains(pt))
        {
            return SkMouseTarget.ScrollThumb;
        }

        if (pt.X >= _thumbRect.Left && pt.X <= _thumbRect.Right)
        {
            if (pt.Y < _thumbRect.Top)
            {
                return SkMouseTarget.ScrollTrackUp;
            }

            if (pt.Y > _thumbRect.Bottom)
            {
                return SkMouseTarget.ScrollTrackDown;
            }
        }

        // Item region
        var itemIndex = GetItemAtXY(x, y);

        return itemIndex != -1 ? SkMouseTarget.Item : SkMouseTarget.None;
    }

    #region Misc. event handlers
    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        var target = GetMouseTarget(e.X, e.Y);

        if (e.Button == MouseButtons.Left &&
            ModifierKeys == (Keys.Shift | Keys.Control | Keys.Alt))
        {
            _suspendRedraw = false;
            _hoverIndex = -1;
            RecalculateLayout();
            Invalidate();

            using var g = CreateGraphics();
            using var p = new Pen(Color.PowderBlue);
            g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);

            return;
        }

        var jump = _scrollSteps <= 0 ? 0 : Math.Min(5, Math.Max(1, _scrollSteps / 3));

        switch (target)
        {
            case SkMouseTarget.UpArrow:
                if (_scrollOffset > 0)
                {
                    _scrollOffset--;
                }

                break;

            case SkMouseTarget.DownArrow:
                if (_scrollOffset + 1 < _scrollSteps)
                {
                    _scrollOffset++;
                }

                break;

            case SkMouseTarget.ScrollTrackUp:
                _scrollOffset = Math.Max(0, _scrollOffset - jump);
                break;

            case SkMouseTarget.ScrollTrackDown:
                _scrollOffset = Math.Min(_scrollSteps - 1, _scrollOffset + jump);
                break;

            case SkMouseTarget.ScrollThumb:
                _draggingThumb = true;
                _dragStartY = e.Y;
                _dragScrollOffset = _scrollOffset;
                break;

            case SkMouseTarget.Item:
                var index = GetItemAtXY(e.X, e.Y);
                if (index >= 0 && index < _items.Count)
                {
                    if (_selectedIndex != index)
                    {
                        _selectedIndex = index;
                        Invalidate();
                    }
                    ItemClick?.Invoke(_items[index], e.Button);
                }

                break;
        }

        Invalidate();
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        var mouseTarget = GetMouseTarget(e.X, e.Y);
        var cursor = Cursors.Default;

        if (_draggingThumb)
        {
            Cursor = Cursors.Hand; // Cursors.SizeNS

            var deltaY = e.Y - _dragStartY;
            var trackHeight = Height - PaddingY * 2 - _thumbRect.Height;
            var scrollRatio = deltaY / trackHeight;

            _scrollOffset = Math.Clamp(_dragScrollOffset + (int)(scrollRatio * _scrollSteps), 0, Math.Max(_scrollSteps - 1, 0));
            Invalidate();
            
            return;
        }

        var invalidate = false;

        switch (mouseTarget)
        {
            case SkMouseTarget.Item:
                var itemIndex = GetItemAtXY(e.X, e.Y);
                if (itemIndex != -1 && itemIndex != _hoverIndex)
                {
                    _hoverIndex = itemIndex;
                    ItemHover?.Invoke(_items[_hoverIndex]);
                    invalidate = true;
                }
                cursor = Cursors.Hand;
                break;

            case SkMouseTarget.UpArrow:
                if (!_hoveringArrowUp)
                {
                    _hoveringArrowUp = true;
                    invalidate = true;
                }
                break;

            case SkMouseTarget.DownArrow:
                if (!_hoveringArrowDown)
                {
                    _hoveringArrowDown = true;
                    invalidate = true;
                }
                break;

            case SkMouseTarget.ScrollTrackUp:
            case SkMouseTarget.ScrollTrackDown:
            case SkMouseTarget.ScrollThumb:
                cursor = Cursors.Hand; // Cursors.SizeNS
                break;

            default:
                if (_hoverIndex != -1)
                {
                    _hoverIndex = -1;
                    EmptyHover?.Invoke();
                    invalidate = true;
                }

                if (_hoveringArrowUp || _hoveringArrowDown || _hoveringExpandToggle)
                {
                    _hoveringArrowUp = _hoveringArrowDown = _hoveringExpandToggle = false;
                    invalidate = true;
                }
                break;
        }

        if (_lastMouseTarget != mouseTarget)
        {
            _lastMouseTarget = mouseTarget;

            // Only redraw if hover state visually changes
            if (mouseTarget == SkMouseTarget.Item || _hoverIndex != -1)
            {
                invalidate = true;
            }
        }

        if (invalidate)
        {
            Invalidate();
        }

        Cursor = cursor;
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        _draggingThumb = false;
        _pressingArrowUp = false;
        _pressingArrowDown = false;

        // Restore default cursor if needed
        if (Cursor == Cursors.Hand)
        {
            Cursor = Cursors.Default;
        }

        Invalidate();
    }

    private void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        if (!_scrollable || _scrollSteps <= 0)
        {
            return;
        }

        var stepSize = Math.Min(5, Math.Max(1, _scrollSteps / 3));

        _scrollOffset = e.Delta switch
        {
            > 0 when _scrollOffset > 0 => Math.Max(0, _scrollOffset - stepSize),
            < 0 when _scrollOffset + 1 < _scrollSteps => Math.Min(_scrollSteps - 1, _scrollOffset + stepSize),
            _ => _scrollOffset
        };

        _hoverIndex = -1; // Reset hover on scroll
        EmptyHover?.Invoke();
        Invalidate();
    }

    private void OnMouseLeave(object? sender, EventArgs e)
    {
        var changed = _hoverIndex != -1 || _hoveringArrowUp || _hoveringArrowDown || _pressingArrowUp || _pressingArrowDown;

        _hoverIndex = -1;
        _hoveringArrowUp = false;
        _hoveringArrowDown = false;
        _pressingArrowUp = false;
        _pressingArrowDown = false;

        _lastMouseTarget = SkMouseTarget.None;
        Cursor = Cursors.Default;

        EmptyHover?.Invoke();

        if (changed)
        {
            Invalidate();
        }
    }

    private void OnResize(object? sender, EventArgs e)
    {
        _scrollOffset = 0;
        RecalculateLayout();
        Invalidate();
    }

    private void OnFontChanged(object? sender, EventArgs e)
    {
        RecalculateLayout();
        Invalidate();
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose each SkListItem to release SKFont resources
            foreach (var item in _items)
            {
                item.Dispose();
            }

            _items.Clear();

            // Detach event handlers since we're managing lifetime
            MouseMove -= OnMouseMove;
            MouseLeave -= OnMouseLeave;
            MouseDown -= OnMouseDown;
            MouseUp -= OnMouseUp;
            MouseWheel -= OnMouseWheel;
            Resize -= OnResize;
            FontChanged -= OnFontChanged;
            PaintSurface -= OnPaintSurface;
        }

        base.Dispose(disposing);
    }
}
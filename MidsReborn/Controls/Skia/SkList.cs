using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Controls.Skia;

[DesignerCategory("Code")]
[ToolboxItem(true)]
public class SkList : SKGLControl
{
    public delegate void ItemClickEventHandler(SkListItem item, MouseButtons button);
    public delegate void ItemHoverEventHandler(SkListItem item);
    public delegate void EmptyHoverEventHandler();
    public delegate void ExpandChangedEventHandler(bool expanded);

    public event ItemClickEventHandler? ItemClick;
    public event ItemHoverEventHandler? ItemHover;
    public event EmptyHoverEventHandler? EmptyHover;
    public event ExpandChangedEventHandler? ExpandChanged;

    private enum SkMouseTarget
    {
        None,
        Item,
        ExpandButton,
        UpArrow,
        DownArrow,
        ScrollThumb,
        ScrollTrackUp,
        ScrollTrackDown
    }

    private SkMouseTarget _lastMouseTarget = SkMouseTarget.None;

    private readonly List<SkListItem> _items = [];

    private const int MultilineTextInterline = 4;
    private const int BottomVisualPadding = 10;

    private int _scrollBarWidth = 11;
    private int _scrollOffset;
    private int _scrollSteps;
    private int _hoverIndex = -1;
    private int _dragStartY;
    private int _dragScrollOffset;
    private int _paddingX = 4;
    private int _paddingY = 2;
    private int _expandMaxY = 400;
    private int _visibleLineCount;
    private int _selectedIndex = -1;

    private bool _canExpand = true;
    private bool _isExpanded;
    private bool _suspendRedraw;
    private bool _scrollable = true;
    private bool _draggingThumb;
    private bool _highVis = true;
    private bool _autoSize;
    private bool _hoveringArrowUp;
    private bool _hoveringArrowDown;
    private bool _hoveringExpandToggle;
    private bool _pressingArrowUp;
    private bool _pressingArrowDown;

    private AutoSizeMode _autoSizeMode = AutoSizeMode.GrowOnly;

    private SKRect _textArea;
    private SKRect _expandToggleBounds;
    private SKRect _thumbRect;
    private SKRect _arrowUpRect;
    private SKRect _arrowDownRect;

    private Size _sizeNormal;

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

    public SkList()
    {
        if (!IsInDesignMode)
        {
            _sizeNormal = Size;

            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            Resize += OnResize;
            FontChanged += OnFontChanged;
            Load += OnLoad;
            PaintSurface += OnPaintSurfaceGL;
        }
    }

    private bool IsInDesignMode =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
        DesignMode || Site?.DesignMode == true;

    public override Font Font =>
        IsInDesignMode
            ? base.Font
            : new Font(Fonts.Family(@"Noto Sans"), base.Font.Size, base.Font.Style, GraphicsUnit.Pixel);

    [Browsable(true)]
    public override bool AutoSize
    {
        get => _autoSize;
        set
        {
            _autoSize = value;
            RecalculateLayout(_isExpanded);
        }
    }

    [Browsable(true)]
    public AutoSizeMode AutoSizeMode
    {
        get => _autoSizeMode;
        set
        {
            _autoSizeMode = value;
            RecalculateLayout(_isExpanded);
        }
    }

    [Browsable(true)]
    public bool Expandable
    {
        get => _canExpand;
        set
        {
            _canExpand = value;
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
            if (!value)
            {
                if (_isExpanded)
                {
                    RecomputeExpand(); // Handles height + layout
                }
                else
                {
                    RecalculateLayout(); // Just reflow
                }

                Invalidate(); // Trigger paint
            }
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
            if (value >= 0 && value * 2 < Width - 5)
            {
                _paddingX = value;
                Invalidate(); // Just redraw
            }
        }
    }

    [Browsable(true)]
    public int PaddingY
    {
        get => _paddingY;
        set
        {
            if (value >= 0 && value < Height / 3.0)
            {
                _paddingY = value;
                SetLineHeight(); // Only update line height
                Invalidate();    // Redraw only
            }
        }
    }

    [Browsable(true)]
    public int MaxHeight
    {
        get => _expandMaxY;
        set
        {
            if (value < _sizeNormal.Height || value > 2000)
                return;

            _expandMaxY = value;
            Invalidate();
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
                return;

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
    public Size SizeNormal
    {
        get => _sizeNormal;
        set
        {
            _sizeNormal = value;
            ToggleExpand(IsExpanded);
            Invalidate();
        }
    }

    [Browsable(false)]
    public SkListItem[] Items => _items.ToArray();

    [Browsable(false)]
    public SkListItem SelectedItem
    {
        get => (_selectedIndex >= 0 && _selectedIndex < _items.Count) ? _items[_selectedIndex] : new SkListItem();
        set
        {
            if (_selectedIndex >= 0 && _selectedIndex < _items.Count)
            {
                _items[_selectedIndex] = new SkListItem(value);
                Invalidate();
            }
        }
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value >= 0 && value < _items.Count)
            {
                _selectedIndex = value;
                Invalidate();
            }
        }
    }

    [Browsable(false)]
    public int DesiredHeight => _items.Sum(item =>
    {
        int lines = item.LineCount;
        int height = lines * ActualLineHeight;
        if (lines > 1)
            height += (lines - 1) * MultilineTextInterline;

        return height + PaddingY;
    });

    [Browsable(true)]
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            ToggleExpand(value);
        }
    }

    [Browsable(false)]
    public int ActualLineHeight { get; set; } = 8;

    public void AddItem(SkListItem item)
    {
        item.Index = _items.Count;
        _items.Add(item);
        WrapText(item);
        _scrollSteps = GetScrollSteps();

        if (!SuspendRedraw)
            Invalidate();
    }

    public void ClearItems()
    {
        foreach (var item in _items)
            item.Dispose();

        _items.Clear();
        _hoverIndex = -1;
        _scrollOffset = 0;
        _scrollSteps = 0;

        if (!SuspendRedraw)
            Invalidate();
    }

    public void UpdateTextColors(EItemState state, Color color)
    {
        if ((int)state < 0 || (int)state >= StateColors.Length) return;
        StateColors[(int)state] = color;
        Invalidate();
    }

    private void ToggleExpand(bool expand)
    {
        _suspendRedraw = true;

        if (expand)
        {
            _isExpanded = true;
            _scrollOffset = 0;
            RecomputeExpand(); // sets Height and calls RecalculateLayout(true)
        }
        else
        {
            Height = _sizeNormal.Height;
            _isExpanded = false;
            RecalculateLayout(); // shrinks back to normal layout
        }

        _suspendRedraw = false;

        ExpandChanged?.Invoke(_isExpanded);
        Invalidate(); // triggers repaint
    }

    private void RecomputeExpand()
    {
        if (!_isExpanded) return;

        _suspendRedraw = true;

        // First pass to estimate content height
        RecalculateLayout(true);

        int contentHeight = GetRealTotalHeight();
        int targetHeight = contentHeight + ScrollBarWidth + PaddingY * 3 + 20;

        // Enforce max expansion height
        if (targetHeight > _expandMaxY)
            targetHeight = _expandMaxY;

        Height = targetHeight;

        // Second pass to recalculate layout with correct height
        RecalculateLayout(true);

        _suspendRedraw = false;

        Invalidate();
    }

    private void RecalculateLayout(bool expanded = false)
    {
        if (_items.Count == 0) return;

        if (AutoSize)
        {
            if (AutoSizeMode == AutoSizeMode.GrowAndShrink || DesiredHeight > SizeNormal.Height)
                Height = DesiredHeight;
            else
                Height = SizeNormal.Height;
        }
        else if (Name == "llAncillary" || Name.StartsWith("llPool"))
        {
            Height = 18 * _items.Count;
        }

        var fullRect = new SKRect(_paddingX, 0, Width - _paddingX * 2, Height - BottomVisualPadding);
        RecalculateLines(fullRect);

        int totalContentHeight = GetRealTotalHeight();
        bool needsScroll = totalContentHeight > Height - BottomVisualPadding;

        _scrollSteps = needsScroll ? GetScrollSteps() : 0;

        if (needsScroll || expanded)
        {
            int scrollHeight = Height - (_scrollBarWidth + _paddingY + BottomVisualPadding);
            var scrollRect = new SKRect(_paddingX, 0, Width - _paddingX * 2, scrollHeight);
            RecalculateLines(scrollRect);
        }

        if (expanded || _scrollSteps > 0)
        {
            int bottomOffset = Expandable ? (_scrollBarWidth + _paddingY + BottomVisualPadding) : BottomVisualPadding;
            var narrowRect = new SKRect(_paddingX, 0, Width - (_paddingX * 2 + _scrollBarWidth), Height - bottomOffset);
            RecalculateLines(narrowRect);
        }

        if (!SuspendRedraw)
        {
            Invalidate();
        }
    }

    private void RecalculateLines(SKRect rect)
    {
        _textArea = rect;

        SetLineHeight();

        foreach (var item in _items)
            WrapText(item);

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
        float ascent = Math.Abs(metrics.Ascent);
        float descent = Math.Abs(metrics.Descent);
        float leading = Math.Abs(metrics.Leading);

        float fullHeight = ascent + descent + leading;

        // Apply vertical padding
        ActualLineHeight = (int)Math.Ceiling(fullHeight) + PaddingY * 2;

        _visibleLineCount = GetVisibleLineCount();
    }

    private void WrapText(SkListItem item)
    {
        if (string.IsNullOrEmpty(item.Text))
            return;

        string baseText = item.Text.Trim("~ ".ToCharArray());
        string prefix = item.ItemState == EItemState.Heading ? "~ " : "";
        string suffix = item.ItemState == EItemState.Heading ? " ~" : "";

        var font = item.GetOrCreateFont(Font);
        int availableWidth = (int)_textArea.Width;

        var words = baseText.Split(' ');
        var lines = new List<string>();
        string currentLine = words[0];

        for (int i = 1; i < words.Length; i++)
        {
            string testLine = currentLine + " " + words[i];
            if (font.MeasureText(testLine) > availableWidth)
            {
                lines.Add(currentLine);
                currentLine = words[i];
            }
            else
            {
                currentLine = testLine;
            }
        }

        lines.Add(currentLine); // last line

        // Apply prefix/suffix only once if heading
        if (item.ItemState == EItemState.Heading)
        {
            lines[0] = prefix + lines[0];
            lines[^1] = lines[^1] + suffix;
        }

        item.WrappedText = string.Join('\n', lines);
        item.LineCount = lines.Count;

        // Layout math
        item.ItemHeight = lines.Count * (ActualLineHeight - PaddingY * 2) + PaddingY * 2;
    }

    private int GetRealTotalHeight() => _items.Sum(e => e.ItemHeight);

    private int GetTotalLineCount() => _items.Sum(i => i.LineCount);

    private int GetVisibleLineCount()
    {
        if (!_scrollable)
        {
            _scrollSteps = 0;
            return 0;
        }

        if (IsExpanded)
            return GetTotalLineCount();

        int y = PaddingY;
        int visibleLines = 0;

        foreach (var item in _items)
        {
            int nextY = y + item.ItemHeight;
            if (nextY > Height - BottomVisualPadding)
                break; // Cut-off detected

            visibleLines += item.LineCount;
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

        int lineSum = 0;
        int wrapCount = 0;

        foreach (var item in _items)
        {
            lineSum += item.LineCount;
            if (lineSum > _visibleLineCount)
            {
                wrapCount++;
            }
        }

        // Add one more scroll page if needed
        if (wrapCount > 0)
            wrapCount++;

        _scrollSteps = wrapCount <= 1 ? 0 : wrapCount;

        return _scrollSteps;
    }

    private static SKColor ToSkColor(Color c) => new(c.R, c.G, c.B, c.A);

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (IsInDesignMode)
        {
            using var background = new SolidBrush(Color.FromArgb(255, 30, 30, 30));
            e.Graphics.FillRectangle(background, ClientRectangle);

            using var pen = new Pen(Color.DodgerBlue);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            using var font = new Font("Segoe UI", 10, FontStyle.Bold);
            var label = "SkList (Design)";
            var size = e.Graphics.MeasureString(label, font);
            e.Graphics.DrawString(label, font, Brushes.LightGray, (Width - size.Width) / 2, (Height - size.Height) / 2);
        }
    }

    private void OnPaintSurfaceGL(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        if (SuspendRedraw || !IsHandleCreated || Width <= 0 || Height <= 0)
            return;

        var canvas = e.Surface.Canvas;
        canvas.Clear(IsExpanded ? SKColors.Black : ToSkColor(BackColor));

        int width = e.BackendRenderTarget.Width;
        int height = e.BackendRenderTarget.Height;

        int y = PaddingY;
        foreach (var item in _items.Skip(_scrollOffset))
        {
            if (y > height)
                break;

            DrawItem(canvas, item, y, width);
            y += item.ItemHeight + PaddingY;
        }

        DrawScrollbar(canvas);
        if (Expandable)
            DrawExpandToggle(canvas);
    }

    private void DrawItem(SKCanvas canvas, SkListItem item, float y, int width)
    {
        var lines = item.WrappedText?.Split('\n') ?? [];
        if (lines.Length == 0) return;

        var font = item.GetOrCreateFont(Font);
        var color = ToSkColor(StateColors[(int)item.ItemState]);
        var metrics = font.Metrics;
        float textHeight = font.Size;

        float x = item.TextAlign switch
        {
            ETextAlign.Center => width / 2f,
            ETextAlign.Right => width - PaddingX - ScrollBarWidth,
            _ => PaddingX
        };

        if (item.Index == _hoverIndex && _highlightOn[(int)item.ItemState])
        {
            var hoverRect = new SKRect(0, y, width - ScrollBarWidth, y + item.ItemHeight);
            using var bg = new SKPaint { Color = ToSkColor(_hoverColor), Style = SKPaintStyle.Fill };
            canvas.DrawRect(hoverRect, bg);
        }

        float lineY = y + PaddingY;
        float interlineSpacing = font.Size + MultilineTextInterline;

        foreach (var line in lines)
        {
            string text = line.Trim();

            if (_highVis)
            {
                using var outline = new SKPaint
                {
                    Color = SKColors.Black,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1,
                    IsAntialias = true
                };

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        canvas.DrawText(text, x + dx, lineY + textHeight + dy, font, outline);
                    }
                }
            }

            using var paint = new SKPaint { Color = color, IsAntialias = true };
            canvas.DrawText(text, x, lineY + textHeight, font, paint);

            if (item.FontFlags.HasFlag(EFontFlags.Underline))
            {
                float underlineY = lineY + textHeight + (metrics.UnderlinePosition ?? 1);
                using var ul = new SKPaint
                {
                    Color = color,
                    StrokeWidth = metrics.UnderlineThickness ?? 1,
                    IsAntialias = true
                };
                float measured = font.MeasureText(text);
                canvas.DrawLine(x, underlineY, x + measured, underlineY, ul);
            }

            if (item.FontFlags.HasFlag(EFontFlags.Strikethrough))
            {
                float strikeY = lineY + textHeight / 2f;
                using var sl = new SKPaint
                {
                    Color = color,
                    StrokeWidth = 1,
                    IsAntialias = true
                };
                float measured = font.MeasureText(text);
                canvas.DrawLine(x, strikeY, x + measured, strikeY, sl);
            }

            lineY += interlineSpacing;
        }
    }

    private void DrawScrollbar(SKCanvas canvas)
    {
        if (_scrollBarWidth < 1 || !_scrollable || _scrollSteps < 1)
            return;

        int trackLeft = (int)(_textArea.Right + _scrollBarWidth / 2f);
        int trackTop = PaddingY + _scrollBarWidth;
        int trackBottom = Height - (_scrollBarWidth + PaddingY);

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
        int usableHeight = Height - (_scrollBarWidth + PaddingY) * 2 - PaddingY * 2;
        int thumbY = (int)Math.Round((_scrollBarWidth + PaddingY * 2) + usableHeight / (double)_scrollSteps * _scrollOffset);
        int thumbHeight = (int)Math.Round(usableHeight / (double)_scrollSteps);

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

    private void DrawExpandToggle(SKCanvas canvas)
    {
        if (!Expandable || (!_isExpanded && _scrollSteps < 1) || Name?.Contains("llPool", StringComparison.OrdinalIgnoreCase) == true)
            return;

        float toggleWidth = Width / 3f;
        float toggleHeight = ScrollBarWidth - PaddingY;
        float toggleLeft = (Width - toggleWidth) / 2f;
        float toggleTop = Height - (ScrollBarWidth + PaddingY);
        float toggleRight = toggleLeft + toggleWidth;
        float toggleBottom = toggleTop + toggleHeight;

        _expandToggleBounds = new SKRect(toggleLeft, toggleTop, toggleRight, toggleBottom);

        var points = new SKPoint[3];
        if (_isExpanded)
        {
            points[0] = new SKPoint(toggleLeft, toggleBottom);
            points[1] = new SKPoint(toggleRight, toggleBottom);
            points[2] = new SKPoint((toggleLeft + toggleRight) / 2f, toggleTop);
        }
        else
        {
            points[0] = new SKPoint(toggleLeft, toggleTop);
            points[1] = new SKPoint(toggleRight, toggleTop);
            points[2] = new SKPoint((toggleLeft + toggleRight) / 2f, toggleBottom);
        }

        using var fill = new SKPaint
        {
            Color = ToSkColor(_scrollButtonColor),
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawVertices(SKVertexMode.Triangles, points, null, fill);

        // Pen2 (highlight)
        using var highlight = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 96),
            StrokeWidth = 1,
            IsAntialias = true
        };

        // Pen3 (shadow)
        using var shadow = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 128),
            StrokeWidth = 1,
            IsAntialias = true
        };

        // Outline triangle edges
        canvas.DrawLine(points[0], points[2], highlight);
        canvas.DrawLine(points[0], points[1], shadow);

        if (_isExpanded)
        {
            // Border around the entire control (bottom + sides)
            using var outline = new SKPaint
            {
                Color = ToSkColor(_scrollBarColor),
                StrokeWidth = 1,
                IsAntialias = false
            };

            canvas.DrawLine(0, 0, 0, Height - 1, outline);
            canvas.DrawLine(0, Height - 1, Width - 1, Height - 1, outline);
            canvas.DrawLine(Width - 1, Height - 1, Width - 1, 0, outline);
        }
    }

    private int GetItemAtY(int y)
    {
        int currentY = PaddingY;

        foreach (var item in _items.Skip(_scrollOffset))
        {
            if (y >= currentY && y < currentY + item.ItemHeight)
                return item.Index;

            currentY += item.ItemHeight + PaddingY;
        }

        return -1;
    }

    private SkMouseTarget GetMouseTarget(int x, int y)
    {
        var pt = new SKPoint(x, y);

        if (_arrowUpRect.Contains(pt))
            return SkMouseTarget.UpArrow;

        if (_arrowDownRect.Contains(pt))
            return SkMouseTarget.DownArrow;

        if (_thumbRect.Contains(pt))
            return SkMouseTarget.ScrollThumb;

        if (_expandToggleBounds.Contains(pt))
            return SkMouseTarget.ExpandButton;

        if (pt.X >= _thumbRect.Left && pt.X <= _thumbRect.Right)
        {
            if (pt.Y < _thumbRect.Top)
                return SkMouseTarget.ScrollTrackUp;
            if (pt.Y > _thumbRect.Bottom)
                return SkMouseTarget.ScrollTrackDown;
        }

        // Item region
        int itemIndex = GetItemAtY(y);
        return itemIndex != -1 ? SkMouseTarget.Item : SkMouseTarget.None;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        _sizeNormal = Size;
        SuspendRedraw = true;
        RecalculateLayout();
        SuspendRedraw = false;
    }

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

        int jump = _scrollSteps <= 0 ? 0 : Math.Min(5, Math.Max(1, _scrollSteps / 3));

        switch (target)
        {
            case SkMouseTarget.UpArrow:
                if (_scrollOffset > 0)
                    _scrollOffset--;
                break;

            case SkMouseTarget.DownArrow:
                if (_scrollOffset + 1 < _scrollSteps)
                    _scrollOffset++;
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

            case SkMouseTarget.ExpandButton:
                ToggleExpand(!_isExpanded);
                break;

            case SkMouseTarget.Item:
                int index = GetItemAtY(e.Y);
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

            case SkMouseTarget.None:
            default:
                break;
        }

        Invalidate();
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        var pt = new SKPoint(e.X, e.Y);
        var mouseTarget = GetMouseTarget(e.X, e.Y);
        Cursor cursor = Cursors.Default;

        if (_draggingThumb)
        {
            Cursor = Cursors.SizeNS;

            int deltaY = e.Y - _dragStartY;
            float trackHeight = Height - PaddingY * 2 - _thumbRect.Height;
            float scrollRatio = deltaY / trackHeight;

            _scrollOffset = Math.Clamp(_dragScrollOffset + (int)(scrollRatio * _scrollSteps), 0, Math.Max(_scrollSteps - 1, 0));
            Invalidate();
            return;
        }

        bool invalidate = false;

        switch (mouseTarget)
        {
            case SkMouseTarget.Item:
                int itemIndex = GetItemAtY(e.Y);
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

            case SkMouseTarget.ExpandButton:
                if (!_hoveringExpandToggle)
                {
                    _hoveringExpandToggle = true;
                    invalidate = true;
                }
                break;

            case SkMouseTarget.ScrollTrackUp:
            case SkMouseTarget.ScrollTrackDown:
            case SkMouseTarget.ScrollThumb:
                cursor = Cursors.SizeNS;
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
                invalidate = true;
        }

        if (invalidate)
            Invalidate();

        Cursor = cursor;
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        _draggingThumb = false;
        _pressingArrowUp = false;
        _pressingArrowDown = false;

        // Restore default cursor if needed
        if (Cursor == Cursors.SizeNS)
            Cursor = Cursors.Default;

        Invalidate();
    }

    private void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        if (!_scrollable || _scrollSteps <= 0) return;

        int stepSize = Math.Min(5, Math.Max(1, _scrollSteps / 3));

        if (e.Delta > 0 && _scrollOffset > 0)
        {
            _scrollOffset = Math.Max(0, _scrollOffset - stepSize);
        }
        else if (e.Delta < 0 && _scrollOffset + 1 < _scrollSteps)
        {
            _scrollOffset = Math.Min(_scrollSteps - 1, _scrollOffset + stepSize);
        }

        _hoverIndex = -1; // Reset hover on scroll
        EmptyHover?.Invoke();
        Invalidate();
    }

    private void OnMouseLeave(object? sender, EventArgs e)
    {
        bool changed = _hoverIndex != -1 || _hoveringArrowUp || _hoveringArrowDown || _pressingArrowUp || _pressingArrowDown;

        _hoverIndex = -1;
        _hoveringArrowUp = false;
        _hoveringArrowDown = false;
        _pressingArrowUp = false;
        _pressingArrowDown = false;

        _lastMouseTarget = SkMouseTarget.None;
        Cursor = Cursors.Default;

        EmptyHover?.Invoke();

        if (changed)
            Invalidate();
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose each SkListItem to release SKFont resources
            foreach (var item in _items)
                item.Dispose();

            _items.Clear();

            // Detach event handlers since we're managing lifetime
            MouseMove -= OnMouseMove;
            MouseLeave -= OnMouseLeave;
            MouseDown -= OnMouseDown;
            MouseUp -= OnMouseUp;
            MouseWheel -= OnMouseWheel;
            Resize -= OnResize;
            FontChanged -= OnFontChanged;
            Load -= OnLoad;
            PaintSurface -= OnPaintSurfaceGL;
        }

        base.Dispose(disposing);
    }
}
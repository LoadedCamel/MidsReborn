using Microsoft.DotNet.DesignTools.Designers;
using System.ComponentModel;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

#region Designer

public sealed class MidsVScrollPanelDesigner : ParentControlDesigner
{
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        if (component is MidsVScrollPanel panel)
        {
            // Treat ContentPanel as the design surface so children drop into it.
            EnableDesignMode(panel.ContentPanel, nameof(MidsVScrollPanel.ContentPanel));
        }
    }
}

#endregion

/// <summary>
/// A vertically scrollable container with a custom-drawn scrollbar.
/// Designed specifically for use in Mids Reborn.
/// </summary>
[Designer(typeof(MidsVScrollPanelDesigner))]
[DesignerCategory("Code")]
public sealed class MidsVScrollPanel : Panel
{
    #region Constants and DPI Scaling

    private const int LogicalScrollBarWidth = 16;
    private const int LogicalArrowHeight = 16;
    private const int LogicalThumbMinHeight = 20;
    private const int LogicalContentBottomPadding = 10;

    private const int LogicalWheelStepPx = 32;          // base per "line" of wheel
    private const int LogicalArrowStepPx = 30;          // per arrow click
    private const int LogicalPageStepMarginPx = 8;      // page step keeps a little margin visible

    private float DpiScale => DeviceDpi / 96f;
    private int Scale(int value) => (int)Math.Round(value * DpiScale);

    private int ScrollBarWidth => Scale(LogicalScrollBarWidth);
    private int ArrowHeight => Scale(LogicalArrowHeight);
    private int ThumbMinHeight => Scale(LogicalThumbMinHeight);
    private int ContentBottomPadding => Scale(LogicalContentBottomPadding);
    private int WheelStepPx => Scale(LogicalWheelStepPx);
    private int ArrowStepPx => Scale(LogicalArrowStepPx);
    private int PageStepMarginPx => Scale(LogicalPageStepMarginPx);

    #endregion

    #region Fields

    private readonly Panel _contentPanel;

    private int _scrollOffset;
    private Rectangle _scrollbarBounds;    // entire scrollbar strip
    private Rectangle _trackBounds;        // interior track (without arrows)
    private Rectangle _upArrowRect;
    private Rectangle _downArrowRect;
    private Rectangle _thumbRect;

    private bool _draggingThumb;
    private int _dragStartY;

    private bool _hoveringThumb;
    private bool _hoveringUpArrow;
    private bool _hoveringDownArrow;

    // Cache during a layout/paint cycle
    private int _cachedContentHeight;

    // Theme hookup guard
    private bool _themeHooked;
    private Action? _themeChangedHandler;

    #endregion

    #region Theme (expects ThemeManager/ScrollPanelTheme in your project)

    private static bool IsInDesignModeSafe(IComponent? c)
        => LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
           c is Control ctrl && ctrl.Site is not null && ctrl.Site.DesignMode;

    private ScrollPanelTheme CurrentTheme
    {
        get
        {
            if (IsInDesignModeSafe(this))
                return ThemeManager.DesignTime.ScrollPanel;

            return ThemeManager.CurrentTheme?.ScrollPanel ?? ThemeManager.DesignTime.ScrollPanel;
        }
    }

    #endregion

    #region Constructor / Dispose

    public MidsVScrollPanel()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint,
            true);

        DoubleBuffered = true;

        _contentPanel = new Panel
        {
            Location = Point.Empty,
            AutoScroll = false,
            AutoSize = false,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        base.Controls.Add(_contentPanel);

        if (!IsInDesignModeSafe(this))
        {
            MinimumSize = new Size(Scale(64), Scale(64));
        }
        else
        {
            // Keep design-time predictable without mutating runtime theme globally.
            ThemeManager.DesignTimeThemeName = "Hero";
        }

        TabStop = true; // to receive keyboard scrolling
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _themeHooked && _themeChangedHandler is not null)
        {
            ThemeManager.ThemeChanged -= _themeChangedHandler;
            _themeHooked = false;
            _themeChangedHandler = null;
        }

        base.Dispose(disposing);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (!_themeHooked && !IsInDesignModeSafe(this))
        {
            _themeChangedHandler = Invalidate;
            ThemeManager.ThemeChanged += _themeChangedHandler;
            _themeHooked = true;
        }
    }

    #endregion

    #region Exposed Content Panel

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Panel ContentPanel => _contentPanel;

    // Route "Controls" into ContentPanel for consumer ergonomics
    [Browsable(false)]
    public new ControlCollection Controls => _contentPanel.Controls;

    #endregion

    #region Public API

    public void ScrollToTop()
    {
        SetScrollOffset(0);
    }

    #endregion

    #region Layout & Scroll Management

    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);
        LayoutContentAndScrollbar();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        LayoutContentAndScrollbar();
    }

    private void LayoutContentAndScrollbar()
    {
        _cachedContentHeight = GetContentHeight();

        // Clamp offset to new bounds
        int maxScroll = Math.Max(0, _cachedContentHeight - ClientSize.Height);
        _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, maxScroll));

        // Reserve space for the scrollbar (if needed) before measuring child layout that depends on width
        int contentWidth = ClientSize.Width - (NeedsScrollbar(_cachedContentHeight) ? ScrollBarWidth : 0);
        _contentPanel.Width = Math.Max(0, contentWidth);

        // Height of inner panel equals measured content height
        if (IsInDesignModeSafe(this))
            _contentPanel.Height = Math.Max(_cachedContentHeight, ClientSize.Height);
        else
            _contentPanel.Height = _cachedContentHeight;

        // Position inner panel by scroll offset
        _contentPanel.Location = new Point(0, -_scrollOffset);

        // Compute scrollbar rectangles
        ComputeScrollbarBounds();
        Invalidate();
    }

    private int GetContentHeight()
    {
        int maxBottom = 0;
        foreach (Control ctrl in _contentPanel.Controls)
        {
            if (!ctrl.Visible) continue;
            maxBottom = Math.Max(maxBottom, ctrl.Bottom);
        }
        return maxBottom + ContentBottomPadding;
    }

    private static bool NeedsScrollbar(int contentHeight, int visibleHeight)
        => contentHeight > visibleHeight;

    private bool NeedsScrollbar(int contentHeight)
        => NeedsScrollbar(contentHeight, ClientSize.Height);

    private void SetScrollOffset(int value)
    {
        int maxScroll = Math.Max(0, _cachedContentHeight - ClientSize.Height);
        int clamped = Math.Max(0, Math.Min(value, maxScroll));
        if (clamped == _scrollOffset) return;

        _scrollOffset = clamped;
        // Reposition panel without forcing another full layout
        _contentPanel.Location = new Point(0, -_scrollOffset);
        Invalidate(_scrollbarBounds);
    }

    private void ScrollBy(int deltaPx)
        => SetScrollOffset(_scrollOffset + deltaPx);

    private void ScrollPage(int direction)
    {
        // One page is the visible height minus a small margin so the user keeps context.
        int page = Math.Max(0, ClientSize.Height - PageStepMarginPx);
        ScrollBy(direction * page);
    }

    private void ComputeScrollbarBounds()
    {
        if (!NeedsScrollbar(_cachedContentHeight))
        {
            _scrollbarBounds = Rectangle.Empty;
            _trackBounds = Rectangle.Empty;
            _upArrowRect = Rectangle.Empty;
            _downArrowRect = Rectangle.Empty;
            _thumbRect = Rectangle.Empty;
            return;
        }

        // Entire scrollbar strip on the right
        _scrollbarBounds = new Rectangle(
            x: Math.Max(0, ClientSize.Width - ScrollBarWidth),
            y: 0,
            width: ScrollBarWidth,
            height: ClientSize.Height);

        // Arrows occupy top/bottom ArrowHeight each
        _upArrowRect = new Rectangle(_scrollbarBounds.X, _scrollbarBounds.Y, _scrollbarBounds.Width, ArrowHeight);
        _downArrowRect = new Rectangle(_scrollbarBounds.X, _scrollbarBounds.Bottom - ArrowHeight, _scrollbarBounds.Width, ArrowHeight);

        // Track between arrows
        _trackBounds = Rectangle.FromLTRB(
            _scrollbarBounds.Left,
            _upArrowRect.Bottom,
            _scrollbarBounds.Right,
            _downArrowRect.Top);

        // Thumb size & position
        int scrollMax = Math.Max(1, _cachedContentHeight - ClientSize.Height);
        int trackHeight = Math.Max(0, _trackBounds.Height);

        int thumbHeight = Math.Max(ThumbMinHeight, (int)Math.Round((double)ClientSize.Height / Math.Max(1, _cachedContentHeight) * trackHeight));
        thumbHeight = Math.Min(thumbHeight, trackHeight);

        int available = Math.Max(0, trackHeight - thumbHeight);
        int thumbY = _trackBounds.Top;
        if (available > 0)
        {
            double ratio = (double)_scrollOffset / scrollMax;
            thumbY = _trackBounds.Top + (int)Math.Round(available * ratio);
        }

        // Make the thumb a little inset horizontally for aesthetics
        int inset = Math.Max(1, (int)Math.Round(_scrollbarBounds.Width * 0.25));
        _thumbRect = new Rectangle(
            _scrollbarBounds.Left + inset,
            thumbY,
            Math.Max(1, _scrollbarBounds.Width - 2 * inset),
            thumbHeight);
    }

    #endregion

    #region Input Handling

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);

        if (!NeedsScrollbar(_cachedContentHeight)) return;

        int lines = SystemInformation.MouseWheelScrollLines;
        if (lines <= 0) lines = 3;

        // e.Delta is multiples of 120; negative delta scrolls down
        int steps = e.Delta / 120 * lines;
        if (steps != 0)
        {
            ScrollBy(-steps * WheelStepPx);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (!NeedsScrollbar(_cachedContentHeight)) return;
        if (!_scrollbarBounds.Contains(e.Location)) return;

        Focus(); // enable keyboard scrolling after click

        if (_thumbRect.Contains(e.Location))
        {
            _draggingThumb = true;
            _dragStartY = e.Y - _thumbRect.Y;
            Capture = true;
            return;
        }

        if (_upArrowRect.Contains(e.Location))
        {
            ScrollBy(-ArrowStepPx);
            return;
        }

        if (_downArrowRect.Contains(e.Location))
        {
            ScrollBy(ArrowStepPx);
            return;
        }

        // Click in track -> page up/down
        if (_trackBounds.Contains(e.Location))
        {
            if (e.Y < _thumbRect.Top)
                ScrollPage(-1);
            else if (e.Y > _thumbRect.Bottom)
                ScrollPage(+1);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (!NeedsScrollbar(_cachedContentHeight)) return;

        bool oldHoverThumb = _hoveringThumb;
        bool oldHoverUp = _hoveringUpArrow;
        bool oldHoverDown = _hoveringDownArrow;

        _hoveringThumb = _thumbRect.Contains(e.Location);
        _hoveringUpArrow = _upArrowRect.Contains(e.Location);
        _hoveringDownArrow = _downArrowRect.Contains(e.Location);

        if (_draggingThumb)
        {
            // Map mouse Y to track space
            int trackTop = _trackBounds.Top;
            int trackHeight = Math.Max(0, _trackBounds.Height);
            int thumbHeight = _thumbRect.Height;

            int available = Math.Max(0, trackHeight - thumbHeight);
            if (available > 0 && _cachedContentHeight > ClientSize.Height)
            {
                int newThumbY = e.Y - _dragStartY;
                newThumbY = Math.Max(trackTop, Math.Min(newThumbY, trackTop + available));

                double ratio = (double)(newThumbY - trackTop) / available;
                int newScroll = (int)Math.Round(ratio * (_cachedContentHeight - ClientSize.Height));
                SetScrollOffset(newScroll);
            }
        }
        else if (oldHoverThumb != _hoveringThumb || oldHoverUp != _hoveringUpArrow || oldHoverDown != _hoveringDownArrow)
        {
            Invalidate(_scrollbarBounds);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (_draggingThumb)
        {
            _draggingThumb = false;
            Capture = false;
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);

        if (_hoveringThumb || _hoveringUpArrow || _hoveringDownArrow)
        {
            _hoveringThumb = _hoveringUpArrow = _hoveringDownArrow = false;
            Invalidate(_scrollbarBounds);
        }
    }

    protected override bool IsInputKey(Keys keyData)
    {
        // Allow arrow/Page/Home/End to be handled
        return keyData is Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End
            || base.IsInputKey(keyData);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!NeedsScrollbar(_cachedContentHeight)) return;

        switch (e.KeyCode)
        {
            case Keys.Up: ScrollBy(-ArrowStepPx); e.Handled = true; break;
            case Keys.Down: ScrollBy(ArrowStepPx); e.Handled = true; break;
            case Keys.PageUp: ScrollPage(-1); e.Handled = true; break;
            case Keys.PageDown: ScrollPage(+1); e.Handled = true; break;
            case Keys.Home: SetScrollOffset(0); e.Handled = true; break;
            case Keys.End: SetScrollOffset(int.MaxValue); e.Handled = true; break;
        }
    }

    #endregion

    #region Drawing and Rendering

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (_scrollbarBounds.IsEmpty) return;
        DrawScrollbar(e.Graphics);
    }

    private void DrawScrollbar(Graphics g)
    {
        var theme = CurrentTheme;

        // Track
        using (var trackPen = new Pen(theme.Track, Math.Max(1, Scale(2))))
        {
            // Draw a centered track line
            int cx = _scrollbarBounds.Left + _scrollbarBounds.Width / 2;
            g.DrawLine(trackPen, cx, _trackBounds.Top, cx, _trackBounds.Bottom);
        }

        // Arrows
        using (var arrowBrush = new SolidBrush(_hoveringUpArrow || _hoveringDownArrow ? theme.Hover : theme.Bar))
        {
            // Up triangle centered in up arrow rect
            var up = _upArrowRect;
            Point[] upArrow =
            {
                new(up.Left + up.Width/2, up.Top + up.Height/4),
                new(up.Left + Scale(3),   up.Bottom - Scale(4)),
                new(up.Right - Scale(3),  up.Bottom - Scale(4))
            };
            g.FillPolygon(arrowBrush, upArrow);

            // Down triangle centered in down arrow rect
            var dn = _downArrowRect;
            Point[] downArrow =
            {
                new(dn.Left + dn.Width/2, dn.Bottom - dn.Height/4),
                new(dn.Left + Scale(3),   dn.Top + Scale(4)),
                new(dn.Right - Scale(3),  dn.Top + Scale(4))
            };
            g.FillPolygon(arrowBrush, downArrow);
        }

        // Thumb
        using (var thumbBrush = new SolidBrush(_hoveringThumb ? theme.Hover : theme.Bar))
        {
            g.FillRectangle(thumbBrush, _thumbRect);
        }
    }

    #endregion
}
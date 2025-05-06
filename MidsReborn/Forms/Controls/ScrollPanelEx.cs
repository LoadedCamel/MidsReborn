using Mids_Reborn.Controls.Designer;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls;

[Designer(typeof(ScrollPanelExDesigner))]
public sealed class ScrollPanelEx : Panel
{
    private const int ScrollBarWidth = 16;
    private const int ArrowHeight = 16;

    private readonly Panel _innerPanel;
    private int _scrollOffset;
    private Rectangle _upArrowRect;
    private Rectangle _downArrowRect;
    private Rectangle _thumbRect;
    private bool _draggingThumb;
    private int _dragStartY;

    private bool _initialized = false;

    private bool IsDesignMode =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
        DesignMode || Site?.DesignMode == true;

    internal Panel InnerPanel => _innerPanel;

    public ScrollPanelEx()
    {
        DoubleBuffered = true;
        AutoScroll = false;

        _innerPanel = new Panel
        {
            Location = Point.Empty,
            Size = new Size(Width - ScrollBarWidth, 0), // width is dynamic, height will grow
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            AutoSize = false,
            AutoScroll = false
        };
        base.Controls.Add(_innerPanel);

        Resize += (_, _) =>
        {
            _innerPanel.Width = Width - ScrollBarWidth;
            SetScrollOffset(_scrollOffset);
            Invalidate();
        };
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        if (e.Control != _innerPanel)
        {
            base.Controls.Remove(e.Control);
            _innerPanel.Controls.Add(e.Control);
        }
        base.OnControlAdded(e);
    }

    public new ControlCollection Controls => _innerPanel.Controls;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!IsDesignMode)
        {
            BeginInvoke(() =>
            {
                SetScrollOffset(0); // Trigger layout after controls are in
            });
        }
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);

        if (!_initialized && Visible && !IsDesignMode)
        {
            _initialized = true;

            BeginInvoke(() =>
            {
                // Force all children to layout and then calculate scroll
                _innerPanel.PerformLayout();
                PerformLayout();
                SetScrollOffset(0);
            });
        }
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        ScrollBy(-Math.Sign(e.Delta) * SystemInformation.MouseWheelScrollLines * 10);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (_thumbRect.Contains(e.Location))
        {
            _draggingThumb = true;
            _dragStartY = e.Y - _thumbRect.Y;
            Capture = true;
        }
        else if (_upArrowRect.Contains(e.Location))
        {
            ScrollBy(-30);
        }
        else if (_downArrowRect.Contains(e.Location))
        {
            ScrollBy(30);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _draggingThumb = false;
        Capture = false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (_draggingThumb)
        {
            int trackHeight = ClientSize.Height - ArrowHeight * 2;
            int contentHeight = GetContentHeight();
            int availableThumbTrack = trackHeight - _thumbRect.Height;

            int newThumbY = e.Y - _dragStartY - ArrowHeight;
            newThumbY = Math.Max(0, Math.Min(newThumbY, availableThumbTrack));

            float ratio = (float)newThumbY / availableThumbTrack;
            int newScroll = (int)(ratio * (contentHeight - ClientSize.Height));
            SetScrollOffset(newScroll);
        }
    }

    private void ScrollBy(int delta)
    {
        SetScrollOffset(_scrollOffset + delta);
    }

    private void SetScrollOffset(int value)
    {
        if (IsDesignMode)
            return;

        int contentHeight = GetContentHeight();

        _innerPanel.Height = contentHeight;

        int maxScroll = Math.Max(0, GetContentHeight() - ClientSize.Height);
        int clamped = Math.Max(0, Math.Min(value, maxScroll));

        if (clamped == _scrollOffset)
            return;

        _scrollOffset = clamped;
        _innerPanel.Height = contentHeight;
        _innerPanel.Top = -_scrollOffset;

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e); // paints background and border
        DrawCustomScrollbar(e.Graphics);
    }

    private void DrawCustomScrollbar(Graphics g)
    {
        int contentHeight = GetContentHeight();
        int visibleHeight = ClientSize.Height;

        if (contentHeight <= visibleHeight)
            return;

        int trackX = ClientSize.Width - ScrollBarWidth / 2 + 3;
        int trackHeight = visibleHeight - ArrowHeight * 2;

        int thumbHeight = Math.Max(20, (int)((float)visibleHeight / contentHeight * trackHeight));
        int scrollMax = contentHeight - visibleHeight;
        float scrollPercent = (float)_scrollOffset / scrollMax;
        int thumbY = ArrowHeight + (int)((trackHeight - thumbHeight) * scrollPercent);

        int thumbX = trackX - ScrollBarWidth / 4;
        int thumbWidth = ScrollBarWidth / 2;

        using Pen trackPen = new(Color.FromArgb(64, 120, 255), 2);
        using SolidBrush arrowBrush = new(Color.FromArgb(0, 122, 255));
        using SolidBrush thumbBrush = new(Color.FromArgb(0, 122, 255));

        // Track
        g.DrawLine(trackPen, trackX, ArrowHeight, trackX, ClientSize.Height - ArrowHeight);

        // Up arrow
        _upArrowRect = new Rectangle(trackX - 6, 0, 12, ArrowHeight);
        Point[] upArrow = {
            new(trackX, 4),
            new(trackX - 6, ArrowHeight - 4),
            new(trackX + 6, ArrowHeight - 4)
        };
        g.FillPolygon(arrowBrush, upArrow);

        // Down arrow
        _downArrowRect = new Rectangle(trackX - 6, ClientSize.Height - ArrowHeight, 12, ArrowHeight);
        Point[] downArrow = {
            new(trackX, ClientSize.Height - 4),
            new(trackX - 6, ClientSize.Height - ArrowHeight + 4),
            new(trackX + 6, ClientSize.Height - ArrowHeight + 4)
        };
        g.FillPolygon(arrowBrush, downArrow);

        // Thumb
        _thumbRect = new Rectangle(thumbX, thumbY, thumbWidth, thumbHeight);
        g.FillRectangle(thumbBrush, _thumbRect);
    }

    private int GetContentHeight()
    {
        int maxBottom = 0;
        foreach (Control ctrl in _innerPanel.Controls)
        {
            if (!ctrl.Visible) continue;
            maxBottom = Math.Max(maxBottom, ctrl.Bottom);
        }
        return maxBottom + 10;
    }
}

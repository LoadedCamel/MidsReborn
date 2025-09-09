using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms.Controls;

[DesignerCategory("Code")]
public sealed class ScrollPanelEx : Panel
{
    private const int ScrollBarWidth = 16;
    private const int ArrowHeight = 16;
    private readonly Panel _contentPanel;

    private int _scrollOffset;
    private Rectangle _upArrowRect;
    private Rectangle _downArrowRect;
    private Rectangle _thumbRect;
    private bool _draggingThumb;
    private int _dragStartY;

    public ScrollPanelEx()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

        _contentPanel = new Panel
        {
            Location = Point.Empty,
            AutoScroll = false,
            AutoSize = false,
            BackColor = Color.Transparent
        };
        base.Controls.Add(_contentPanel);

        Resize += (_, _) =>
        {
            LayoutContent();
            Invalidate();
        };
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Panel ContentPanel => _contentPanel;

    public new ControlCollection Controls => _contentPanel.Controls;

    public void ScrollToTop()
    {
        SetScrollOffset(1);
        SetScrollOffset(0);
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

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (_draggingThumb)
        {
            int trackHeight = Height - ArrowHeight * 2;
            int contentHeight = GetContentHeight();
            int availableTrack = trackHeight - _thumbRect.Height;

            int newThumbY = e.Y - _dragStartY - ArrowHeight;
            newThumbY = Math.Max(0, Math.Min(newThumbY, availableTrack));

            float ratio = (float)newThumbY / availableTrack;
            int newScroll = (int)(ratio * (contentHeight - Height));
            SetScrollOffset(newScroll);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _draggingThumb = false;
        Capture = false;
    }

    private void ScrollBy(int delta)
    {
        SetScrollOffset(_scrollOffset + delta);
    }

    private void SetScrollOffset(int value)
    {
        int contentHeight = GetContentHeight();
        int maxScroll = Math.Max(0, contentHeight - Height);
        int clamped = Math.Max(0, Math.Min(value, maxScroll));

        if (clamped == _scrollOffset) return;

        _scrollOffset = clamped;
        LayoutContent();
        Invalidate();
    }

    private void LayoutContent()
    {
        _contentPanel.Location = new Point(0, -_scrollOffset);
        _contentPanel.Width = Width - (NeedsScrollbar() ? ScrollBarWidth : 0);
        _contentPanel.Height = GetContentHeight();
    }

    private int GetContentHeight()
    {
        int maxBottom = 0;
        foreach (Control ctrl in _contentPanel.Controls)
        {
            if (!ctrl.Visible) continue;
            maxBottom = Math.Max(maxBottom, ctrl.Bottom);
        }
        return maxBottom + 10;
    }

    private bool NeedsScrollbar()
    {
        return GetContentHeight() > Height;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (NeedsScrollbar())
            DrawScrollbar(e.Graphics);
    }

    private void DrawScrollbar(Graphics g)
    {
        int contentHeight = GetContentHeight();
        int visibleHeight = Height;

        int trackX = Width - ScrollBarWidth / 2 + 3;
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

        // Scrollbar track
        g.DrawLine(trackPen, trackX, ArrowHeight, trackX, Height - ArrowHeight);

        // Up arrow
        _upArrowRect = new Rectangle(trackX - 6, 0, 12, ArrowHeight);
        Point[] upArrow =
        [
            new(trackX, 4),
            new(trackX - 6, ArrowHeight - 4),
            new(trackX + 6, ArrowHeight - 4)
        ];
        g.FillPolygon(arrowBrush, upArrow);

        // Down arrow
        _downArrowRect = new Rectangle(trackX - 6, Height - ArrowHeight, 12, ArrowHeight);
        Point[] downArrow =
        [
            new(trackX, Height - 4),
            new(trackX - 6, Height - ArrowHeight + 4),
            new(trackX + 6, Height - ArrowHeight + 4)
        ];
        g.FillPolygon(arrowBrush, downArrow);

        // Thumb
        _thumbRect = new Rectangle(thumbX, thumbY, thumbWidth, thumbHeight);
        g.FillRectangle(thumbBrush, _thumbRect);
    }
}

using Mids_Reborn.UI.Design.Designer;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls;

[Designer(typeof(ScrollPanelExDesigner))]
public sealed class ScrollPanelEx : Panel
{
    private const int ScrollBarWidth = 16;
    private const int ArrowHeight = 16;
    private const int ScrollBarXOffset = -3;

    private int _scrollOffset;
    private Rectangle _upArrowRect;
    private Rectangle _downArrowRect;
    private Rectangle _thumbRect;
    private bool _draggingThumb;
    private int _dragStartY;

    private bool _initialized;
    private bool _useAlt;

    private bool IsDesignMode =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
        DesignMode || Site?.DesignMode == true;

    internal Panel InnerPanel { get; }

    public bool UseAlt
    {
        get => _useAlt;
        set
        {
            _useAlt = value;
            Refresh();
        }
    }

    public ScrollPanelEx()
    {
        DoubleBuffered = true;
        AutoScroll = false;

        InnerPanel = new Panel
        {
            Location = Point.Empty,
            Size = new Size(Width - ScrollBarWidth, 0), // width is dynamic, height will grow
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            AutoSize = false,
            AutoScroll = false
        };
        base.Controls.Add(InnerPanel);

        Resize += (_, _) =>
        {
            InnerPanel.Width = Width - ScrollBarWidth;
            SetScrollOffset(_scrollOffset);
            Invalidate();
        };
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        if (e.Control != InnerPanel)
        {
            base.Controls.Remove(e.Control);
            InnerPanel.Controls.Add(e.Control);
        }
        base.OnControlAdded(e);
    }

    public new ControlCollection Controls => InnerPanel.Controls;

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
                InnerPanel.PerformLayout();
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
        if (!_draggingThumb)
        {
            return;
        }

        var trackHeight = ClientSize.Height - ArrowHeight * 2;
        var contentHeight = GetContentHeight();
        var availableThumbTrack = trackHeight - _thumbRect.Height;

        var newThumbY = e.Y - _dragStartY - ArrowHeight;
        newThumbY = Math.Max(0, Math.Min(newThumbY, availableThumbTrack));

        var ratio = (float)newThumbY / availableThumbTrack;
        var newScroll = (int)(ratio * (contentHeight - ClientSize.Height));
        SetScrollOffset(newScroll);
    }

    private void ScrollBy(int delta)
    {
        SetScrollOffset(_scrollOffset + delta);
    }

    private void SetScrollOffset(int value)
    {
        if (IsDesignMode)
            return;

        var contentHeight = GetContentHeight();

        InnerPanel.Height = contentHeight;

        var maxScroll = Math.Max(0, GetContentHeight() - ClientSize.Height);
        var clamped = Math.Max(0, Math.Min(value, maxScroll));

        if (clamped == _scrollOffset)
        {
            return;
        }

        _scrollOffset = clamped;
        InnerPanel.Height = contentHeight;
        InnerPanel.Top = -_scrollOffset;

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e); // paints background and border

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        e.Graphics.CompositingMode = CompositingMode.SourceOver;
        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        DrawCustomScrollbar(e.Graphics);
    }

    private void DrawCustomScrollbar(Graphics g)
    {
        var contentHeight = GetContentHeight();
        var visibleHeight = ClientSize.Height;

        if (contentHeight <= visibleHeight)
        {
            return;
        }

        var trackX = ClientSize.Width - ScrollBarWidth / 2 + 3 + ScrollBarXOffset;
        var trackHeight = visibleHeight - ArrowHeight * 2;

        var thumbHeight = Math.Max(20, (int)((float)visibleHeight / contentHeight * trackHeight));
        var scrollMax = contentHeight - visibleHeight;
        var scrollPercent = (float)_scrollOffset / scrollMax;
        var thumbY = ArrowHeight + (int)((trackHeight - thumbHeight) * scrollPercent);

        var thumbX = trackX - ScrollBarWidth / 4;
        var thumbWidth = ScrollBarWidth / 2;

        using Pen trackPen = new(!_useAlt ? Color.FromArgb(64, 120, 255) : Color.FromArgb(191, 74, 56), 2);
        using SolidBrush arrowBrush = new(!_useAlt ? Color.FromArgb(0, 122, 255) : Color.FromArgb(128, 0, 0));
        using SolidBrush thumbBrush = new(!_useAlt ? Color.FromArgb(0, 122, 255) : Color.FromArgb(128, 0, 0));

        // Track
        g.DrawLine(trackPen, trackX, ArrowHeight, trackX, ClientSize.Height - ArrowHeight);

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
        _downArrowRect = new Rectangle(trackX - 6, ClientSize.Height - ArrowHeight, 12, ArrowHeight);
        Point[] downArrow =
        [
            new(trackX, ClientSize.Height - 4),
            new(trackX - 6, ClientSize.Height - ArrowHeight + 4),
            new(trackX + 6, ClientSize.Height - ArrowHeight + 4)
        ];
        g.FillPolygon(arrowBrush, downArrow);

        // Thumb
        _thumbRect = new Rectangle(thumbX, thumbY, thumbWidth, thumbHeight);
        g.FillRectangle(thumbBrush, _thumbRect);
    }

    private int GetContentHeight()
    {
        var maxBottom = (from Control ctrl in InnerPanel.Controls where ctrl.Visible select ctrl.Bottom)
            .Prepend(0)
            .Max();
        
        return maxBottom + 10;
    }
}

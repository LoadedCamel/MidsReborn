using Mids_Reborn.UI.Renderer;
using System.ComponentModel;

namespace Mids_Reborn.UI.Controls;

/// <summary>
/// Double-buffered, scroll-friendly host that blits BuildRenderer's off-screen buffer.
/// Keeps BuildRenderer unchanged (ExtendedBitmap pattern) but integrates cleanly with WinForms painting.
/// </summary>
public sealed class MidsBufferedImagePanel : Control
{
    #region Instance Fields
    private bool _suspendAutoSize;
    private bool _useTransparentBackground = true;
    private BuildRenderer? _renderer;
    #endregion

    #region Properties
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BuildRenderer? Renderer
    {
        get => _renderer;
        set
        {
            _renderer = value;
            if (IsHandleCreated && _renderer != null)
            {
                _renderer.ReInit(this);
                if (AutoSizeContent) ResizeToContent();
                Invalidate();
            }
        }
    }

    /// <summary>
    /// When true (default) sets Height to the renderer's full content height so a parent scroll panel can scroll this control.
    /// </summary>
    [DefaultValue(true)]
    public bool AutoSizeContent { get; set; } = true;

    /// <summary>
    /// Allows turning off the parent-background composite if you don't need transparency (tiny perf win).
    /// </summary>
    [DefaultValue(true)]
    public bool UseTransparentBackground
    {
        get => _useTransparentBackground;
        set
        {
            _useTransparentBackground = value;
            BackColor = value ? Color.Transparent : SystemColors.Control;
            Invalidate();
        }
    }

    [DefaultValue(true)]
    public bool ManageRendererOnSize { get; set; } = true;

    #endregion

    #region Constructors
    public MidsBufferedImagePanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint
               | ControlStyles.UserPaint
               | ControlStyles.OptimizedDoubleBuffer
               | ControlStyles.ResizeRedraw
               | ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.Transparent;
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// Force reinitialization of BuildRenderer with this canvas.
    /// </summary>
    public void Reinit()
    {
        Renderer?.ReInit(this);
        Invalidate();
    }

    /// <summary>
    /// Force BuildRenderer to rebuild its buffer and repaint.
    /// </summary>
    public void RequestFullRedraw()
    {
        if (Renderer == null)
        {
            Invalidate();
            return;
        }

        Renderer.FullRedraw();

        // IMPORTANT: if the renderer’s required height changed (e.g., first pick),
        // update our Height so the parent can scroll/display the new content.
        if (AutoSizeContent && !_suspendAutoSize)
            ResizeToContent();

        Invalidate();
    }

    /// <summary>
    /// Ask BuildRenderer for total content height and set our Height accordingly (so the parent can scroll us).
    /// </summary>
    public void ResizeToContent()
    {
        if (Renderer == null) return;
        var size = Renderer.GetRequiredDrawingArea();
        if (size.Height <= 0) return;

        try
        {
            _suspendAutoSize = true;
            Height = size.Height;
        }
        finally
        {
            _suspendAutoSize = false;
        }

        (Parent as MidsVScrollPanel)?.RecalculateLayout();
    }

    /// <summary>Returns the scaled rectangle for the power button at history index <paramref name="hIdx"/> in panel client coords.</summary>
    public Rectangle GetPowerButtonRect(int hIdx) => Renderer?.GetPowerButtonRect(hIdx) ?? Rectangle.Empty;

    /// <summary>Returns the scaled rectangle covering the power button + its slot row (if any) in panel client coords.</summary>
    public Rectangle GetPowerAreaRect(int hIdx) => Renderer?.GetPowerAreaRect(hIdx) ?? Rectangle.Empty;

    /// <summary>Returns the scaled rectangle for a specific enhancement slot under a power in panel client coords.</summary>
    public Rectangle GetEnhancementSlotRect(int hIdx, int slotIndex) => Renderer?.GetEnhancementSlotRect(hIdx, slotIndex) ?? Rectangle.Empty;

    /// <summary>Returns all enhancement slot rectangles for a power (left → right) in panel client coords.</summary>
    public IReadOnlyList<Rectangle> GetEnhancementSlotRects(int hIdx) => Renderer is null ? [] : Renderer.GetEnhancementSlotRects(hIdx);

    /// <summary>Rectangle for the “+1 new slot” hover (if available) in panel client coords; empty if not applicable.</summary>
    public Rectangle GetNewSlotHoverRect(int hIdx) => Renderer?.GetNewSlotHoverRect(hIdx) ?? Rectangle.Empty;

    /// <summary>Convenience: power button rect in *screen* coordinates.</summary>
    public Rectangle GetPowerButtonRectOnScreen(int hIdx)
    {
        var r = GetPowerButtonRect(hIdx);
        return r.IsEmpty ? Rectangle.Empty : RectangleToScreen(r);
    }

    /// <summary>Convenience: power area rect in *screen* coordinates.</summary>
    public Rectangle GetPowerAreaRectOnScreen(int hIdx)
    {
        var r = GetPowerAreaRect(hIdx);
        return r.IsEmpty ? Rectangle.Empty : RectangleToScreen(r);
    }

    /// <summary>Convenience: enhancement slot rect in *screen* coordinates.</summary>
    public Rectangle GetEnhancementSlotRectOnScreen(int hIdx, int slotIndex)
    {
        var r = GetEnhancementSlotRect(hIdx, slotIndex);
        return r.IsEmpty ? Rectangle.Empty : RectangleToScreen(r);
    }

    /// <summary>Convenience: new-slot hover rect in *screen* coordinates.</summary>
    public Rectangle GetNewSlotHoverRectOnScreen(int hIdx)
    {
        var r = GetNewSlotHoverRect(hIdx);
        return r.IsEmpty ? Rectangle.Empty : RectangleToScreen(r);
    }

    /// <summary>
    /// Power hit-test in client coordinates. Returns the power history index (hIdx), or -1 if none.
    /// Delegates to BuildRenderer.WhichSlot(int x, int y).
    /// </summary>
    public int WhichSlot(Point clientPt) => Renderer?.WhichSlot(clientPt.X, clientPt.Y) ?? -1;

    /// <summary>
    /// Power hit-test in client coordinates. Returns the power history index (hIdx), or -1 if none.
    /// Delegates to BuildRenderer.WhichSlot(int x, int y).
    /// </summary>
    public int WhichSlot(int x, int y) => Renderer?.WhichSlot(x, y) ?? -1;

    /// <summary>
    /// Power hit-test in *screen* coordinates. Returns hIdx or -1.
    /// </summary>
    public int WhichSlotOnScreen(Point screenPt) => WhichSlot(PointToClient(screenPt));

    /// <summary>
    /// Enhancement hit-test in client coordinates. Returns slot index (sIdx) under the cursor, or -1.
    /// Delegates to BuildRenderer.WhichEnh(int x, int y).
    /// </summary>
    public int WhichEnh(Point clientPt) => Renderer?.WhichEnh(clientPt.X, clientPt.Y) ?? -1;

    /// <summary>
    /// Enhancement hit-test in client coordinates. Returns slot index (sIdx) under the cursor, or -1.
    /// Delegates to BuildRenderer.WhichEnh(int x, int y).
    /// </summary>
    public int WhichEnh(int x, int y) => Renderer?.WhichEnh(x, y) ?? -1;

    /// <summary>
    /// Enhancement hit-test in *screen* coordinates. Returns sIdx or -1.
    /// </summary>
    public int WhichEnhOnScreen(Point screenPt) => WhichEnh(PointToClient(screenPt));

    /// <summary>
    /// Convenience: returns (hIdx, sIdx) if pointing at an enhancement slot; otherwise (-1, -1).
    /// Uses the renderer's own WhichSlot/WhichEnh so results match render-time geometry exactly.
    /// </summary>
    public (int hIdx, int sIdx) WhichEnhPair(Point clientPt)
    {
        if (Renderer is null) return (-1, -1);
        int h = Renderer.WhichSlot(clientPt.X, clientPt.Y);
        if (h < 0) return (-1, -1);
        int s = Renderer.WhichEnh(clientPt.X, clientPt.Y);
        return s >= 0 ? (h, s) : (-1, -1);
    }

    /// <summary>
    /// Screen-space convenience for (hIdx, sIdx).
    /// </summary>
    public (int hIdx, int sIdx) WhichEnhPairOnScreen(Point screenPt) => WhichEnhPair(PointToClient(screenPt));

    /// <summary>
    /// Does the given client-space point lie within the given rectangle?
    /// </summary>
    public bool IsWithinBounds(Point clientPt, Rectangle bounds) => bounds.Contains(clientPt);

    #endregion

    #region Protected/Internal Methods
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        // Initialize renderer with this host so DPI/width are correct
        Renderer?.ReInit(this);
        if (AutoSizeContent) ResizeToContent();
        Invalidate();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);

        if (Renderer != null && ManageRendererOnSize)
        {
            Renderer.ReInit(this);
            if (AutoSizeContent && !_suspendAutoSize)
                ResizeToContent();
        }

        Invalidate();
    }

    // Proper “transparent” compositing: paint the parent behind us when transparent.
    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        if (!_useTransparentBackground || BackColor.A == 255 || Parent is null)
        {
            base.OnPaintBackground(pevent);
            return;
        }

        var g = pevent.Graphics;
        var state = g.Save();
        try
        {
            g.TranslateTransform(-Left, -Top);
            using var args = new PaintEventArgs(g, Parent.ClientRectangle);

            InvokePaintBackground(Parent, args);
        }
        finally
        {
            g.Restore(state);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // Blit only the visible slice from BuildRenderer's large off-screen bitmap.
        var bmp = Renderer?.BxBuffer?.Bitmap;
        if (bmp is null) return;

        var src = e.ClipRectangle;
        if (src.Width <= 0 || src.Height <= 0)
            src = ClientRectangle;

        var bmpBounds = new Rectangle(Point.Empty, bmp.Size);
        var srcClamped = Rectangle.Intersect(src, bmpBounds);
        if (srcClamped.IsEmpty) return;

        // Draw at natural (unscaled) coordinates; WinForms clip ensures we only touch what's needed.
        e.Graphics.DrawImage(
            bmp,
            destRect: srcClamped,
            srcX: srcClamped.X,
            srcY: srcClamped.Y,
            srcWidth: srcClamped.Width,
            srcHeight: srcClamped.Height,
            srcUnit: GraphicsUnit.Pixel);
    }
    #endregion
}
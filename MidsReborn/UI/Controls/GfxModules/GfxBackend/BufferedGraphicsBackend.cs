using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.UI.Controls.GfxModules.GfxBackend;

public class BufferedGraphicsBackend : IGfxBackend
{
    public readonly ClsDrawX.GfxBackendType BackendType = ClsDrawX.GfxBackendType.BufferedGraphics;

    internal BufferedGraphics? BxBuffer;
    internal BufferedGraphicsContext? BxBufferContext;

    internal Graphics? _gTarget;
    internal Control _cTarget;
    internal Font _defaultFont;
    internal Color _backColor;
    
    private Size _bufferSize;

    internal float ScaleValue { get; set; } = 2f;

    public void PrepareTarget(Control ctl)
    {
        if (ctl == null || ctl.IsDisposed || ctl.Width <= 0 || ctl.Height <= 0)
        {
            return;
        }

        BxBuffer?.Dispose(); // Dispose the previous buffer if it exists

        BxBufferContext ??= BufferedGraphicsManager.Current;

        // Allocate a new buffer for the current size
        BxBuffer = BxBufferContext.Allocate(ctl.CreateGraphics(), ctl.ClientRectangle);

        // Set high-quality rendering options
        BxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        BxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        BxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        BxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        BxBuffer.Graphics.CompositingMode = CompositingMode.SourceOver;
        BxBuffer.Graphics.PageUnit = GraphicsUnit.Pixel;

        _cTarget = ctl;
        _bufferSize = ctl.ClientRectangle.Size;
    }

    public void ReInit(Control ctl)
    {
        if (ctl.IsDisposed)
        {
            return;
        }

        BxBuffer?.Dispose(); // Dispose the previous buffer if it exists

        BxBufferContext ??= BufferedGraphicsManager.Current;

        // Allocate a new buffer for the current size
        BxBuffer = BxBufferContext.Allocate(ctl.CreateGraphics(), ctl.ClientRectangle);

        //_gTarget = ctl.CreateGraphics();
        BxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        BxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        BxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        BxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        BxBuffer.Graphics.CompositingMode = CompositingMode.SourceOver;
        BxBuffer.Graphics.PageUnit = GraphicsUnit.Pixel;
        _cTarget = ctl;
        _bufferSize = ctl.ClientRectangle.Size;
        //DefaultFont = new Font(iTarget.Font.FontFamily, iTarget.Font.Size, FontStyle.Bold, iTarget.Font.Unit);
        _defaultFont = new Font(Fonts.Family("Noto Sans"), 12.25f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
        _backColor = ctl.BackColor;
    }

    public void Refresh(Rectangle clip)
    {
        BxBuffer?.Render(_cTarget.CreateGraphics());
    }

    public void Blank()
    {
        BxBuffer?.Graphics.Clear(_backColor);
    }

    public void Blank(Color color)
    {
        BxBuffer?.Graphics.Clear(color);
    }

    public void SetGraphicsQuality()
    {
        if (BxBuffer == null)
        {
            return;
        }

        BxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        BxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        BxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        BxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        BxBuffer.Graphics.CompositingMode = CompositingMode.SourceOver;
        BxBuffer.Graphics.PageUnit = GraphicsUnit.Pixel;
    }

    public void ResetTarget()
    {
    }

    private void Output()
    {
        BxBuffer?.Render(_cTarget.CreateGraphics());
    }

    public void Output(Rectangle destRect, Rectangle srcRect, GraphicsUnit iUnit)
    {
        Output();
    }

    public void OutputUnscaled(Point location)
    {
        Output();
    }

    public void OutputUnscaled()
    {
        Output();
    }

    public Size GetBufferSize() => _bufferSize;
}
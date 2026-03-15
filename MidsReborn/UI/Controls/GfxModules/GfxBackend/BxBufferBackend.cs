using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls.GfxModules.GfxBackend;

public abstract class BxBufferBackend : IGfxBackend
{
    public readonly ClsDrawX.GfxBackendType BackendType = ClsDrawX.GfxBackendType.BxBuffer;
    
    // Graphics object of target drawing surface (The panel)
    internal Graphics _gTarget;
    internal Control _cTarget;
    internal Font _defaultFont;
    internal Color _backColor;
    
    internal float ScaleValue { get; set; } = 2f;

    // Surface to draw on before combining to display
    public ExtendedBitmap? BxBuffer;

    public void PrepareTarget(Control ctl)
    {
        _gTarget = ctl.CreateGraphics();
        _gTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
        _gTarget.CompositingQuality = CompositingQuality.HighQuality;
        _gTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
        _gTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        _gTarget.SmoothingMode = SmoothingMode.HighQuality;
        _gTarget.CompositingMode = CompositingMode.SourceOver;
        _gTarget.PageUnit = GraphicsUnit.Pixel;
    }

    public void ReInit(Control ctl)
    {
        if (ctl.IsDisposed)
        {
            return;
        }

        _gTarget = ctl.CreateGraphics();
        _gTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
        _gTarget.CompositingQuality = CompositingQuality.HighQuality;
        _gTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
        _gTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        _gTarget.SmoothingMode = SmoothingMode.HighQuality;
        _gTarget.CompositingMode = CompositingMode.SourceOver;
        _gTarget.PageUnit = GraphicsUnit.Pixel;
        _cTarget = ctl;
        //DefaultFont = new Font(iTarget.Font.FontFamily, iTarget.Font.Size, FontStyle.Bold, iTarget.Font.Unit);
        _defaultFont = new Font(Fonts.Family("Noto Sans"), 12.25f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
        _backColor = ctl.BackColor;
    }

    public void Refresh(Rectangle clip)
    {
        Output(clip, clip, GraphicsUnit.Pixel);
    }

    public void Blank()
    {
        BxBuffer?.Graphics?.Clear(_backColor);
    }

    public void Blank(Color color)
    {
        BxBuffer?.Graphics?.Clear(color);
    }

    public void SetGraphicsQuality()
    {
        if (BxBuffer?.Graphics == null)
        {
            return;
        }

        BxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        BxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        BxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        BxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        BxBuffer.Graphics.CompositingMode = CompositingMode.SourceOver;
    }

    public void ResetTarget()
    {
        if (BxBuffer?.Graphics != null)
        {
            BxBuffer.Graphics.TextRenderingHint = ScaleValue > 1.125
                ? TextRenderingHint.SystemDefault
                : TextRenderingHint.ClearTypeGridFit;
        }

        _gTarget.Dispose();

        if (_cTarget.IsDisposed)
        {
            return;
        }

        _gTarget = _cTarget.CreateGraphics();
        _gTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
        _gTarget.CompositingQuality = CompositingQuality.HighQuality;
        _gTarget.CompositingMode = CompositingMode.SourceOver;
        _gTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
        _gTarget.SmoothingMode = SmoothingMode.HighQuality;
    }

    public void Output(Rectangle destRect, Rectangle srcRect, GraphicsUnit iUnit)
    {
        _gTarget.DrawImage(BxBuffer.Bitmap, destRect, srcRect, iUnit);
    }

    public void OutputUnscaled(Point location)
    {
        _gTarget.DrawImageUnscaled(BxBuffer.Bitmap, location);
    }

    public void OutputUnscaled()
    {
        _gTarget.DrawImageUnscaled(BxBuffer.Bitmap, new Point(0, 0));
    }

    public Size GetBufferSize()
    {
        return BxBuffer?.Size ?? new Size(0, 0);
    }
}
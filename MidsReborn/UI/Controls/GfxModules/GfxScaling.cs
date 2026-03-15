using System;
using System.Drawing;
using System.Drawing.Text;

namespace Mids_Reborn.UI.Controls.GfxModules;

public static class GfxScaling
{
    internal static float FontScale(this ClsDrawX drawX, float iSz)
    {
        return Math.Min(drawX.ScaleDown(iSz) * 1.1f, iSz);
    }

    public static void SetScaling(this ClsDrawX drawX, Size iSize)
    {
        var origScaling = drawX.Scaling;
        var origScaleValue = drawX.ScaleValue;

        if ((iSize.Width < 10) | (iSize.Height < 10))
        {
            return;
        }

        var drawingArea = drawX.GetDrawingArea();
        var needsScaling = drawingArea.Width > iSize.Width || drawingArea.Height > iSize.Height;
        drawX.Scaling = needsScaling;

        if (needsScaling)
        {
            // Calculate scale factor based on DPI and control size
            var dpiScale = Math.Max(drawX.BxBuffer.Graphics.DpiX, drawX.BxBuffer.Graphics.DpiY) / 96f;
            drawX.ScaleValue = Math.Max(
                drawingArea.Width / (float)iSize.Width,
                drawingArea.Height / (float)iSize.Height
            ) * dpiScale;

            drawX.SetGraphicsQuality(); // Refactored method to set graphics quality

            if (drawX.ScaleValue != origScaleValue)
            {
                drawX.FullRedraw();
            }
        }
        else
        {
            drawX.BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            drawX.ScaleValue = 1f;

            drawX.ResetTarget();

            if (origScaling != drawX.Scaling || origScaleValue != drawX.ScaleValue)
            {
                drawX.FullRedraw();
            }
        }
    }

    public static int ScaleDown(this ClsDrawX drawX, int iValue)
    {
        // Take a full size value and convert it to a scaled value
        // for co-ordinates, etc.
        return !drawX.Scaling
            ? iValue
            : (int)Math.Round(iValue / drawX.ScaleValue);
    }

    public static float ScaleDown(this ClsDrawX drawX, float iValue)
    {
        // Take a full size value and convert it to a scaled value
        // for co-ordinates, etc.
        return !drawX.Scaling
            ? iValue
            : iValue / drawX.ScaleValue;
    }

    public static Rectangle ScaleDown(this ClsDrawX drawX, Rectangle iValue)
    {
        // Take a full size value and convert it to a scaled value
        // for co-ordinates, etc.
        if (!drawX.Scaling)
        {
            return iValue;
        }

        return new Rectangle(
            (int)Math.Round(iValue.X / drawX.ScaleValue),
            (int)Math.Round(iValue.Y / drawX.ScaleValue),
            (int)Math.Round(iValue.Width / drawX.ScaleValue),
            (int)Math.Round(iValue.Height / drawX.ScaleValue)
        );
    }

    public static RectangleF ScaleDown(this ClsDrawX drawX, RectangleF iValue)
    {
        // Take a full size value and convert it to a scaled value
        // for co-ordinates, etc.
        if (!drawX.Scaling)
        {
            return iValue;
        }

        return new RectangleF(
            (float)Math.Round(iValue.X / drawX.ScaleValue),
            (float)Math.Round(iValue.Y / drawX.ScaleValue),
            (float)Math.Round(iValue.Width / drawX.ScaleValue),
            (float)Math.Round(iValue.Height / drawX.ScaleValue)
        );
    }

    public static int ScaleUp(this ClsDrawX drawX, int iValue)
    {
        // Take a full size value and convert it to a scaled value
        // for co-ordinates, etc.
        return !drawX.Scaling
            ? iValue
            : checked((int)Math.Round(iValue * drawX.ScaleValue));
    }
}
using System;
using SkiaSharp;
using static mrbBase.Enums;

namespace mrbControls
{
    public static class SKCanvasExt
    {
        public static void DrawOutlineText(this SKCanvas canvas, string text, SKPoint location, SKColor textColor,
            SKTextAlign textAlign = SKTextAlign.Left, byte opacity = 0xFF, float fontSize = 12f, float strokeWidth = 3f)
        {
            using var textFont = new SKFont(SKTypeface.Default, fontSize);
            using var textPaint = new SKPaint(textFont)
            {
                IsAntialias = true,
                IsStroke = false,
                Color = new SKColor(textColor.Red, textColor.Green, textColor.Blue, opacity),
                TextAlign = textAlign,
            };

            var textBounds = new SKRect();
            textPaint.MeasureText(text, ref textBounds);

            using var textPath = textAlign switch
            {
                SKTextAlign.Center => textPaint.GetTextPath(text, location.X - textBounds.Width / 2f, location.Y),
                SKTextAlign.Right => textPaint.GetTextPath(text, location.X - textBounds.Width - 0.5f, location.Y),
                _ => textPaint.GetTextPath(text, location.X, location.Y)
            };

            using var outlinePath = new SKPath();
            textPaint.GetFillPath(textPath, outlinePath);

            using var outlinePaint = new SKPaint
            {
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                StrokeMiter = 0, /* Avoid spikes artifacts around sharp edges */
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                Color = new SKColor(0, 0, 0, opacity)
            };

            canvas.DrawPath(outlinePath, outlinePaint);
            canvas.DrawText(text, location, textPaint);
        }

        public static SKPoint DrawTextShort(this SKCanvas canvas, string text, float fontSize, float x, float y,
            SKPaint textPaint, eHTextAlign hTextAlign = eHTextAlign.Left,
            eVTextAlign vTextAlign = eVTextAlign.Middle, bool ignoreOffsetX = false,
            bool ignoreOffsetY = false)
        {
            if (text == null)
            {
                return new SKPoint(-1, -1);
            }

            using var font = new SKFont(SKTypeface.Default, fontSize);
            var textRect = new SKRect();
            textPaint.MeasureText(text, ref textRect);
            var xOffset = hTextAlign switch
            {
                _ when ignoreOffsetX => 0,
                eHTextAlign.Left => textRect.Left,
                eHTextAlign.Right => textRect.Width, 
                eHTextAlign.Center => textRect.Width / 2f,
                _ => 0
            };

            var yOffset = vTextAlign switch
            {
                _ when ignoreOffsetY => 0,
                eVTextAlign.Top => textRect.Top,
                eVTextAlign.Middle => -textRect.Height / 2f,
                _ => 0
            };

            var ret = new SKPoint(x - xOffset, y - yOffset);

            canvas.DrawText(SKTextBlob.Create(text, font), ret.X, ret.Y, textPaint);

            return ret;
        }

        public static SKPoint DrawTextShort(this SKCanvas canvas, string text, float fontSize, SKRect layoutRect,
            SKPaint textPaint, eHTextAlign hTextAlign, eVTextAlign vTextAlign = eVTextAlign.Middle)
        {
            if (text == null)
            {
                return new SKPoint(-1, -1);
            }

            using var font = new SKFont(SKTypeface.Default, fontSize);
            var textRect = new SKRect();
            var lw = Math.Abs(layoutRect.Width);
            var lh = Math.Abs(layoutRect.Height);
            textPaint.MeasureText(text, ref textRect);

            var xOffset = hTextAlign switch
            {
                eHTextAlign.Left => textRect.Left,
                eHTextAlign.Right => textRect.Width,
                eHTextAlign.Center => textRect.Width / 2f,
                _ => 0
            };

            var yOffset = vTextAlign switch
            {
                eVTextAlign.Top => textRect.Top,
                eVTextAlign.Middle => -textRect.Height / 2f,
                _ => 0
            };

            var tx = hTextAlign switch
            {
                eHTextAlign.Right => layoutRect.Right - xOffset,
                eHTextAlign.Center => layoutRect.Left + lw / 2f - xOffset,
                _ => layoutRect.Left
            };

            var ty = vTextAlign switch
            {
                eVTextAlign.Top => layoutRect.Top - yOffset,
                eVTextAlign.Middle => layoutRect.Top + lh / 2f - yOffset,
                _ => layoutRect.Bottom
            };

            canvas.DrawText(SKTextBlob.Create(text, font), tx, ty, textPaint);

            return new SKPoint(tx, ty);
        }
    }
}
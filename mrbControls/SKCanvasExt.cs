using System.Diagnostics;
using SkiaSharp;

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
                SKTextAlign.Center => textPaint.GetTextPath(text, location.X - textBounds.Width / 2f, location.Y), // location.Y - textBounds.Height / 2f ?
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

        public static void DrawTextShort(this SKCanvas canvas, string text, float fontSize, float x, float y, SKPaint textPaint)
        {
            if (text == null)
            {
                return;
            }

            canvas.DrawText(SKTextBlob.Create(text, new SKFont(SKTypeface.Default, fontSize)), x, y, textPaint);
        }
    }
}
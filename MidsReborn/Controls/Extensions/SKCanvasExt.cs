using System;
using Mids_Reborn.Core.Base.Master_Classes;
using SkiaSharp;
using static Mids_Reborn.Core.Enums;

namespace Mids_Reborn.Controls.Extensions
{
    public static class SKCanvasExt
    {
        /// <summary>
        /// Draw outlined text on a skia canvas.
        /// This overload is just a shorthand to process vertical alignment.
        /// </summary>
        /// <param name="canvas">Skia canvas to draw on</param>
        /// <param name="text">Input string</param>
        /// <param name="layoutRect">Rectangle on canvas in which to draw. If using any text alignment but left and top, text will be aligned according to it.</param>
        /// <param name="textColor">Text color</param>
        /// <param name="hAlign">Text horizontal alignment (Left, Center, Right)</param>
        /// <param name="vAlign">Text vertical alignment (Top, Middle, Bottom)</param>
        /// <param name="opacity">Text opacity (0: transparent, 255: opaque)</param>
        /// <param name="fontSize">Text font size, in pixels</param>
        /// <param name="strokeWidth">Shadow stroke width (unused when oldSchoolStyle == true), starting from the text edge included. Effective size of stroke is strokeWidth - 2</param>
        /// <param name="oldSchoolStyle">When set, mimic the old school behavior by drawing black texts around location. Those at 1 px distance are at full opacity, those farther away are drawn with reduced opacity.</param>
        public static void DrawOutlineText(this SKCanvas canvas, string text, SKRect layoutRect, SKColor textColor,
            eHTextAlign hAlign = eHTextAlign.Left, eVTextAlign vAlign = eVTextAlign.Middle, byte opacity = 0xFF,
            float fontSize = 12f, float strokeWidth = 3f, bool oldSchoolStyle = false)
        {
            using var font = new SKFont(SKTypeface.Default, fontSize);
            using var textPaint = new SKPaint
            {
                IsAntialias = true,
                Color = textColor,
                TextSize = fontSize
            };

            var lw = Math.Abs(layoutRect.Width);
            var lh = Math.Abs(layoutRect.Height);
            var textRect = new SKRect();
            textPaint.MeasureText(text, ref textRect);
            var xOffset = hAlign switch
            {
                eHTextAlign.Left => textRect.Left,
                eHTextAlign.Right => textRect.Width,
                eHTextAlign.Center => textRect.Width / 2f,
                _ => 0
            };

            var yOffset = vAlign switch
            {
                eVTextAlign.Top => textRect.Top,
                eVTextAlign.Middle => -textRect.Height / 2f,
                _ => 0
            };

            var tx = hAlign switch
            {
                eHTextAlign.Right => layoutRect.Right - xOffset,
                eHTextAlign.Center => layoutRect.Left + lw / 2f - xOffset,
                _ => layoutRect.Left
            };

            var ty = vAlign switch
            {
                eVTextAlign.Top => layoutRect.Top - yOffset,
                eVTextAlign.Middle => layoutRect.Top + lh / 2f - yOffset,
                _ => layoutRect.Bottom
            };

            var ret = new SKPoint(tx, ty);

            DrawOutlineText(canvas, text, ret, textColor, hAlign, opacity, fontSize, strokeWidth, oldSchoolStyle);
        }

        /// <summary>
        /// Draw outlined text on a skia canvas.
        /// This overload is just a shorthand to process vertical alignment.
        /// </summary>
        /// <remarks>
        /// Made private, direct draw with an anchor as a SKPoint has unreliable effects.
        /// </remarks>
        /// <param name="canvas">Skia canvas to draw on</param>
        /// <param name="text">Input string</param>
        /// <param name="location">Position on canvas</param>
        /// <param name="textColor">Text color</param>
        /// <param name="textAlign">Text horizontal alignment (Left, Center, Right)</param>
        /// <param name="opacity">Text opacity (0: transparent, 255: opaque)</param>
        /// <param name="fontSize">Text font size, in pixels</param>
        /// <param name="strokeWidth">Shadow stroke width (unused when oldSchoolStyle == true), starting from the text edge included. Effective size of stroke is strokeWidth - 2</param>
        /// <param name="oldSchoolStyle">When set, mimic the old school behavior by drawing black texts around location. Those at 1 px distance are at full opacity, those farther away are drawn with reduced opacity.</param>
        private static void DrawOutlineText(this SKCanvas canvas, string text, SKPoint location, SKColor textColor,
            eHTextAlign textAlign = eHTextAlign.Left, byte opacity = 0xFF, float fontSize = 12f, float strokeWidth = 3f, bool oldSchoolStyle = false)
        {
            var textAlignSk = textAlign switch
            {
                eHTextAlign.Left => SKTextAlign.Left,
                eHTextAlign.Right => SKTextAlign.Right,
                _ => SKTextAlign.Center
            };

            using var textFont = new SKFont(SKTypeface.Default, fontSize);
            using var textPaint = new SKPaint(textFont)
            {
                IsAntialias = true,
                IsStroke = false,
                Color = new SKColor(textColor.Red, textColor.Green, textColor.Blue, opacity)
            };

            var textBounds = new SKRect();
            textPaint.MeasureText(text, ref textBounds);

            /*using var textPath = textAlign switch
            {
                eHTextAlign.Center => textPaint.GetTextPath(text, location.X - textBounds.Width / 2f, location.Y),
                eHTextAlign.Right => textPaint.GetTextPath(text, location.X - textBounds.Width - 0.5f, location.Y),
                _ => textPaint.GetTextPath(text, location.X, location.Y)
            };*/

            using var textPath = textPaint.GetTextPath(text, location.X, location.Y);

            if (!oldSchoolStyle)
            {
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

                return;
            }

            var shadowPaint = new SKPaint(textFont)
            {
                IsAntialias = true,
                IsStroke = false,
                Color = SKColors.Black // new SKColor(0, 0, 0, opacity)
            };

            var shadowPaintDim = new SKPaint(textFont)
            {
                IsAntialias = true,
                IsStroke = false,
                Color = new SKColor(0, 0, 0, 64)
            };

            const int radius = 2;
            const int outerRadius = 2;
            for (var x = -radius; x <= radius; x++)
            {
                for (var y = -radius; y <= radius; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    var p = new SKPoint(location.X + x, location.Y + y);
                    if (Math.Abs(x) >= outerRadius || Math.Abs(y) >= outerRadius)
                    {
                        canvas.DrawText(text, p, shadowPaintDim);
                    }
                    else
                    {
                        canvas.DrawText(text, p, shadowPaint);
                    }
                }
            }

            canvas.DrawText(text, location, textPaint);

            return;
        }

        /// <summary>
        /// Shorthand to draw text on a canvas.
        /// </summary>
        /// <param name="canvas">Skia canvas to draw on</param>
        /// <param name="text">Input text</param>
        /// <param name="fontSize">Font size (pixels)</param>
        /// <param name="x">Anchor point X (pixels)</param>
        /// <param name="y">Anchor point Y (pixels)</param>
        /// <param name="textPaint">SKPaint style used to draw text.</param>
        /// <param name="hTextAlign">Horizontal text alignment (Left, Center, Right)</param>
        /// <param name="vTextAlign">Vertical text alignment (Top, Middle, Bottom)</param>
        /// <param name="ignoreOffsetX">Ignore X adjustments (e.g. pre-centered text)</param>
        /// <param name="ignoreOffsetY">Ignore Y adjustments (e.g. pre-centered text)</param>
        /// <returns>>Top-left corner point of where text was drawn</returns>
        /// <remarks>textPaint must have at least those fields set: IsAntialias = true (font smoothing), Color and TextSize.
        /// Do not set Shader or TextAlign.</remarks>
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

        /// <summary>
        /// Shorthand to draw text on a canvas.
        /// </summary>
        /// <param name="canvas">Skia canvas to draw on</param>
        /// <param name="text">Input text</param>
        /// <param name="fontSize">Font size (pixels)</param>
        /// <param name="layoutRect">Rectangle on canvas in which to draw. If using any text alignment but left and top, text will be aligned according to it.</param>
        /// <param name="textPaint">SKPaint style used to draw text.</param>
        /// <param name="hTextAlign">Horizontal text alignment (Left, Center, Right)</param>
        /// <param name="vTextAlign">Vertical text alignment (Top, Middle, Bottom)</param>
        /// <returns>>Top-left corner point of where text was drawn</returns>
        /// <remarks>textPaint must have at least those fields set: IsAntialias = true (font smoothing), Color and TextSize.
        /// Do not set Shader or TextAlign.</remarks>
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

        /// <summary>
        /// Generate color transform matrix to change from hero (blue) to villain (red) theme.
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns>SKColorFilter with the color matrix filter according to current character alignment.
        /// For hero this is the identity matrix.</returns>
        /// <remarks>IMPORTANT: matrix coefficients differs from what was used for system.drawing AND from the "channel mixer" filter from graphics software.
        /// This has been set through trial and error and result isn't a 100% match.</remarks>
        public static SKColorFilter HeroVillainColorMatrix(this SKCanvas canvas)
        {
            return MidsContext.Character.IsHero()
                ? SKColorFilter.CreateColorMatrix(new[]
                {
                    1f,     0,     0,      0,      0,
                    0,      1f,    0,      0,      0,
                    0,      0,     1f,     0,      0,
                    0,      0,     0,      1f,     0
                })
                : SKColorFilter.CreateColorMatrix(new[]
                {
                    0.85f,  0,     0,      0,      0,
                    0,      0.6f,  0,      0,      0,
                    0.175f, 0,     0,      0,      0,
                    0,      0,     0,      1f,     0
                });
        }
    }
}
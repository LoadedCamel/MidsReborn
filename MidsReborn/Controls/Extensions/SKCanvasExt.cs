using System;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using Mids_Reborn.Core.Base.Master_Classes;
using SkiaSharp;
using static Mids_Reborn.Core.Enums;

namespace Mids_Reborn.Controls.Extensions
{
    public static class SKCanvasExt
    {
        public static void DrawOutlineText(this SKCanvas canvas, string text, SKPoint location, SKColor textColor,
            SKTextAlign hAlign = SKTextAlign.Left, VerticalAlignment vAlign = VerticalAlignment.Top,
            byte opacity = 0xFF, float fontSize = 12f, float strokeWidth = 3f, bool oldSchoolStyle = false)
        {
            using var textFont = new SKFont(SKTypeface.Default, fontSize);
            using var textPaint = new SKPaint(textFont)
            {
                IsAntialias = true,
                IsStroke = false,
                Color = new SKColor(textColor.Red, textColor.Green, textColor.Blue, opacity),
                TextAlign = hAlign,
            };

            var textBounds = new SKRect();
            textPaint.MeasureText(text, ref textBounds);

            // Align to real bottom rather than base line.
            var vOffset = (Regex.IsMatch(text, @"[ypqgjQY]") ? -fontSize / 4f : 0) + vAlign switch
            {
                VerticalAlignment.Center => textBounds.Height / 2f,
                VerticalAlignment.Bottom => textBounds.Height,
                _ => 0
            };

            canvas.DrawOutlineText(text, new SKPoint(location.X, location.Y + vOffset), textColor, hAlign, opacity, fontSize, strokeWidth, oldSchoolStyle);
        }

        public static void DrawOutlineText(this SKCanvas canvas, string text, SKPoint location, SKColor textColor,
            SKTextAlign textAlign = SKTextAlign.Left, byte opacity = 0xFF, float fontSize = 12f, float strokeWidth = 3f, bool oldSchoolStyle = false)
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
                Color = SKColors.Black, // new SKColor(0, 0, 0, opacity),
                TextAlign = textAlign
            };

            var shadowPaintDim = new SKPaint(textFont)
            {
                IsAntialias = true,
                IsStroke = false,
                Color = new SKColor(0, 0, 0, 64),
                TextAlign = textAlign
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
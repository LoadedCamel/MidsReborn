using SkiaSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Mids_Reborn.Core.Theming;

public static class HueShift
{
    public static Bitmap AdjustHueAndSaturation(Bitmap sourceBitmap, Color targetColor)
    {
        using var ms = new MemoryStream();
        sourceBitmap.Save(ms, ImageFormat.Png);
        ms.Seek(0, SeekOrigin.Begin);

        using var skBitmap = SKBitmap.Decode(ms);
        if (skBitmap == null) return sourceBitmap;

        float sourceHue = GetAverageHue(skBitmap);
        float sourceSaturation = GetAverageSaturation(skBitmap);

        float targetHue = targetColor.GetHue();
        float targetSaturation = targetColor.GetSaturation();

        float hueShift = GetHueShiftDegrees(sourceHue, targetHue);
        float saturationScale = GetSaturationScale(sourceSaturation, targetSaturation);

        using var surface = SKSurface.Create(skBitmap.Info);
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        using var paint = new SKPaint();
        paint.ColorFilter = SKColorFilter.CreateColorMatrix(CreateHueSaturationMatrix(hueShift, saturationScale));

        canvas.DrawBitmap(skBitmap, 0, 0, paint);

        using var resultImage = surface.Snapshot();
        return SkImageToBitmap(resultImage);
    }

    private static float[] CreateHueSaturationMatrix(float hueDegrees, float saturationScale)
    {
        float hue = hueDegrees * (float)Math.PI / 180f;
        float cos = (float)Math.Cos(hue);
        float sin = (float)Math.Sin(hue);

        const float lumR = 0.213f;
        const float lumG = 0.715f;
        const float lumB = 0.072f;

        float r1 = lumR + cos * (1 - lumR) - sin * lumR;
        float g1 = lumG - cos * lumG - sin * lumG;
        float b1 = lumB - cos * lumB + sin * (1 - lumB);

        float r2 = lumR - cos * lumR + sin * 0.143f;
        float g2 = lumG + cos * (1 - lumG) + sin * 0.140f;
        float b2 = lumB - cos * lumB - sin * 0.283f;

        float r3 = lumR - cos * lumR - sin * (1 - lumR);
        float g3 = lumG - cos * lumG + sin * lumG;
        float b3 = lumB + cos * (1 - lumB) + sin * lumB;

        return
        [
            lumR + (r1 - lumR) * saturationScale, lumG + (g1 - lumG) * saturationScale, lumB + (b1 - lumB) * saturationScale, 0, 0,
                lumR + (r2 - lumR) * saturationScale, lumG + (g2 - lumG) * saturationScale, lumB + (b2 - lumB) * saturationScale, 0, 0,
                lumR + (r3 - lumR) * saturationScale, lumG + (g3 - lumG) * saturationScale, lumB + (b3 - lumB) * saturationScale, 0, 0,
                0, 0, 0, 1, 0
        ];
    }

    private static float GetAverageHue(SKBitmap bitmap)
    {
        long total = 0;
        float hueSum = 0;
        for (int y = 0; y < bitmap.Height; y++)
            for (int x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                if (color.Alpha < 128) continue;
                hueSum += Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue).GetHue();
                total++;
            }
        return total > 0 ? hueSum / total : 0;
    }

    private static float GetAverageSaturation(SKBitmap bitmap)
    {
        long total = 0;
        float saturationSum = 0;
        for (int y = 0; y < bitmap.Height; y++)
            for (int x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                if (color.Alpha < 128) continue;
                saturationSum += Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue).GetSaturation();
                total++;
            }
        return total > 0 ? saturationSum / total : 1f;
    }

    private static float GetHueShiftDegrees(float sourceHue, float targetHue)
    {
        float delta = targetHue - sourceHue;
        if (delta > 180) delta -= 360;
        if (delta < -180) delta += 360;
        return delta;
    }

    private static float GetSaturationScale(float current, float target)
    {
        if (Math.Abs(current) < 0.001f) return 1f;
        return target / current;
    }

    private static Bitmap SkImageToBitmap(SKImage skImage)
    {
        using var data = skImage.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = new MemoryStream(data.ToArray());
        return new Bitmap(stream);
    }
}
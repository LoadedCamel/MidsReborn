using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor;

internal static class DbEditorIconLayout
{
    public const int MinimumIconSize = 32;
    public const int GridPadding = 4;

    public static Rectangle GetAspectFitBounds(Size sourceSize, Rectangle bounds)
    {
        if (sourceSize.Width <= 0 || sourceSize.Height <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return bounds;
        }

        var scale = Math.Min((float)bounds.Width / sourceSize.Width, (float)bounds.Height / sourceSize.Height);
        var width = Math.Max(1, (int)Math.Round(sourceSize.Width * scale));
        var height = Math.Max(1, (int)Math.Round(sourceSize.Height * scale));
        var x = bounds.X + (bounds.Width - width) / 2;
        var y = bounds.Y + (bounds.Height - height) / 2;

        return new Rectangle(x, y, width, height);
    }

    public static void DrawImageAspectFit(Graphics graphics, Image image, Rectangle bounds)
    {
        if (graphics == null || image == null || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.DrawImage(image, GetAspectFitBounds(image.Size, bounds));
    }
}

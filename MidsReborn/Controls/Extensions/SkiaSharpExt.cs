using System;
using System.Drawing;
using System.IO;
using SkiaSharp;

namespace Mids_Reborn.Controls.Extensions
{
    public static class SkiaSharpExt
    {
        /// <summary>
        /// Converts a System.Drawing.Color to a SkiaSharp.SKColor.
        /// </summary>
        /// <param name="color">The System.Drawing.Color to convert.</param>
        /// <returns>The corresponding SKColor.</returns>
        public static SKColor ToSKColor(this Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Converts a SkiaSharp.SKColor to a System.Drawing.Color.
        /// </summary>
        /// <param name="skColor">The SKColor to convert.</param>
        /// <returns>The corresponding System.Drawing.Color.</returns>
        public static Color ToDrawingColor(this SKColor skColor)
        {
            return Color.FromArgb(skColor.Alpha, skColor.Red, skColor.Green, skColor.Blue);
        }

        /// <summary>
        /// Converts an SKImage to a System.Drawing.Bitmap.
        /// </summary>
        /// <param name="image">The SKImage to convert.</param>
        /// <returns>A Bitmap representation of the SKImage.</returns>
        public static Bitmap ToBitmap(this SKImage image)
        {
            using var encoded = image.Encode();
            if (encoded == null)
                throw new InvalidOperationException("Failed to encode the SKImage.");

            var data = encoded.ToArray();
            using var stream = new MemoryStream(data);
            return new Bitmap(stream);
        }

        /// <summary>
        /// Converts an Bitmap to a SKImage.
        /// </summary>
        /// <param name="bitmap">The Bitmap to convert.</param>
        /// <returns>A SKImage representation of the Bitmap.</returns>
        public static SKImage ToSKImage(this Bitmap bitmap)
        {
            using var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);
            return SKImage.FromEncodedData(stream);
        }
    }
}

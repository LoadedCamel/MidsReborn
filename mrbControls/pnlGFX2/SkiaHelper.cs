using System.Drawing;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace mrbControls.pnlGFX2
{
    public static class SkiaHelper
    {
        // https://github.com/mono/SkiaSharp/issues/1633#issue-809649575
        public static SKBitmap ToSKBitmap(Bitmap bitmap)
        {
            var info = new SKImageInfo(bitmap.Width, bitmap.Height);
            var skiaBitmap = new SKBitmap(info);
            using var pixmap = skiaBitmap.PeekPixels();
            bitmap.ToSKPixmap(pixmap);

            return skiaBitmap;
        }

        public static SKImage ToSKImage(Bitmap bitmap)
        {
            var info = new SKImageInfo(bitmap.Width, bitmap.Height);
            var image = SKImage.Create(info);
            using var pixmap = image.PeekPixels();
            bitmap.ToSKPixmap(pixmap);

            return image;
        }
    }
}
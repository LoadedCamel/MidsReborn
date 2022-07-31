using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using mrbBase.Base.Master_Classes;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace mrbControls.Extensions
{
    public static class ControlExt
    {
        public static void DrawImageButton(this Control target, clsDrawX drawing, Color backColor, string label, bool active = false)
        {
            if (drawing == null)
            {
                return;
            }

            using var s = SKSurface.Create(new SKImageInfo(target.Width, target.Height));

            s.Canvas.Clear(backColor.ToSKColor());
            var bgBitmapIndex = active
                ? MidsContext.Character.IsHero()
                    ? 3
                    : 5
                : MidsContext.Character.IsHero()
                    ? 2
                    : 4;

            using var bgBitmap = drawing.bxPower[bgBitmapIndex].Bitmap.ToSKImage();
            using var colorFilter = s.Canvas.HeroVillainColorMatrix();

            s.Canvas.DrawImage(bgBitmap,
                new SKRect(0, 0, drawing.bxPower[bgBitmapIndex].Size.Width, drawing.bxPower[bgBitmapIndex].Size.Height),
                new SKRect(0, 0, target.Width, target.Height),
                new SKPaint { ColorFilter = colorFilter });

            s.Canvas.DrawOutlineText(label, new SKPoint(target.Width / 2f, target.Height / 2f), SKColors.WhiteSmoke, SKTextAlign.Center, VerticalAlignment.Center, 255, 13, 3, true);

            target.BackgroundImage = s.Snapshot().ToBitmap();
        }
    }
}

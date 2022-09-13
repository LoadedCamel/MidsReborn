using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using static Mids_Reborn.Core.Enums;

namespace Mids_Reborn.Controls.Extensions
{
    public static class ControlExt
    {
        /// <summary>
        /// Transforms a compatible control (usually Panel or PictureBox but can be any) into an image button.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="drawing">clsDrawX link, needed to fetch pre-loaded button background images</param>
        /// <param name="backColor">Color to fill the background with</param>
        /// <param name="label">Button text</param>
        /// <param name="active">'Pressed' state for the button</param>
        /// <remarks>Graphics once drawn are sent to the control BackgroundImage bitmap.</remarks>
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

            s.Canvas.DrawOutlineText(label, new SKRect(0, 0, target.Width, target.Height), SKColors.WhiteSmoke, eHTextAlign.Center, eVTextAlign.Middle, 255, 13, 3, true);

            target.BackgroundImage = s.Snapshot().ToBitmap();
        }
    }
}

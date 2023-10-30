using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Controls.Extensions
{
    internal static class TextRendererExt
    {
        public static void DrawOutlineText(Graphics graphics, string? text, Font font, Rectangle bounds, Color outlineColor, Color foreColor, TextFormatFlags flags)
        {
            bounds.Offset(-1, 0);
            TextRenderer.DrawText(graphics, text, font, bounds, outlineColor, flags | TextFormatFlags.NoPadding);
            bounds.Offset(2, 0);
            TextRenderer.DrawText(graphics, text, font, bounds, outlineColor, flags | TextFormatFlags.NoPadding);
            bounds.Offset(-1, -1);
            TextRenderer.DrawText(graphics, text, font, bounds, outlineColor, flags | TextFormatFlags.NoPadding);
            bounds.Offset(0, 2);
            TextRenderer.DrawText(graphics, text, font, bounds, outlineColor, flags | TextFormatFlags.NoPadding);
            bounds.Offset(-1, -2);
            TextRenderer.DrawText(graphics, text, font, bounds, outlineColor, flags | TextFormatFlags.NoPadding);
            bounds.Offset(2, 0);
            TextRenderer.DrawText(graphics, text, font, bounds, outlineColor, flags | TextFormatFlags.NoPadding);
            bounds.Offset(-2, 2);
            TextRenderer.DrawText(graphics, text, font, bounds, outlineColor, flags | TextFormatFlags.NoPadding);
            bounds.Offset(2, 0);
            TextRenderer.DrawText(graphics, text, font, bounds, outlineColor, flags | TextFormatFlags.NoPadding);
            bounds.Offset(-1, -1);
            TextRenderer.DrawText(graphics, text, font, bounds, foreColor, flags | TextFormatFlags.NoPadding);
        }

        public static void DrawTextWithSubColor(IDeviceContext dc, string text, string subText, Font font, Point pt, Color textColor, Color subTextColor)
        {
            // Initialize the current position where text will be drawn
            var currentPoint = pt;

            // Split the text around the substring
            var parts = text.Split(new[] { subText }, StringSplitOptions.None);

            // Measure the width of a single space character
            var singleSpaceWidth = TextRenderer.MeasureText(" ", font).Width;

            // Draw each part
            for (var i = 0; i < parts.Length; i++)
            {
                // Draw the main text part
                TextRenderer.DrawText(dc, parts[i], font, currentPoint, textColor);

                // Update currentPoint for the next draw
                var partSize = TextRenderer.MeasureText(parts[i], font);
                currentPoint.X += partSize.Width;

                // If there's a highlighted part following this main part, draw it
                if (i >= parts.Length - 1) continue;
                TextRenderer.DrawText(dc, subText, font, currentPoint, subTextColor);

                // Update currentPoint for the next draw, but limit the space to a single character's width
                currentPoint.X += TextRenderer.MeasureText(subText, font).Width - (partSize.Width - singleSpaceWidth);
            }
        }
    }
}

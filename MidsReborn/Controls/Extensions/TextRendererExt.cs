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
    }
}

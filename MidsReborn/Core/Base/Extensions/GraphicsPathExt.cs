using System.Drawing;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.Core.Base.Extensions
{
    public static class GraphicsPathExt
    {
        public static void AddRoundRectangle(this GraphicsPath path, Rectangle rectangle, int radius)
        {
            var diameter = radius * 2;
            var size = new Size(diameter, diameter);
            var arc = new Rectangle(rectangle.Location, size);

            path.AddArc(arc, 180, 90); // Top-left corner
            arc.X = rectangle.Right - diameter;
            path.AddArc(arc, 270, 90); // Top-right corner
            arc.Y = rectangle.Bottom - diameter;
            path.AddArc(arc, 0, 90);   // Bottom-right corner
            arc.X = rectangle.Left;
            path.AddArc(arc, 90, 90);  // Bottom-left corner
            path.CloseFigure();
        }
    }
}

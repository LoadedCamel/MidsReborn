using System.Drawing;

namespace Mids_Reborn.UI.Design.Extensions;

public static class DrawingExt
{
    public static Rectangle ToRectangle(this RectangleF rectangleF)
    {
        return new Rectangle((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);
    }
}
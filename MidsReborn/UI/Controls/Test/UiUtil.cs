using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

internal static class UiUtil
{
    public static GraphicsPath Capsule(Rectangle r, int radius)
    {
        int d = Math.Max(1, radius) * 2;
        var p = new GraphicsPath();
        p.StartFigure();
        p.AddArc(r.Left, r.Top, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }
}
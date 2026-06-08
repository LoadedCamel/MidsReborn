using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls;

public class MidsLogoPanel : Panel
{
    private readonly Bitmap? _originalLogo;

    public MidsLogoPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);

        try
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Assets", "mids_resurgence.png");
            if (File.Exists(path))
            {
                _originalLogo = new Bitmap(path);
            }
        }
        catch
        {
            // Fails silently if the logo can't be loaded, preventing a crash.
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // 1. First, check for null in case the logo file failed to load.
        if (_originalLogo == null)
        {
            // If we are in the designer, draw a helpful placeholder to show what this control is.
            if (DesignMode)
            {
                using (var brush = new SolidBrush(Color.FromArgb(50, 50, 50)))
                {
                    e.Graphics.FillRectangle(brush, ClientRectangle);
                }
                TextRenderer.DrawText(e.Graphics, "MidsLogoPanel", Font, ClientRectangle, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            return; 
        }

        Graphics g = e.Graphics;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        Rectangle destRect = new Rectangle(0, 0, Width, Height);
        g.DrawImage(_originalLogo, destRect, 0, 0, _originalLogo.Width, _originalLogo.Height, GraphicsUnit.Pixel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _originalLogo?.Dispose();
        }
        base.Dispose(disposing);
    }
}
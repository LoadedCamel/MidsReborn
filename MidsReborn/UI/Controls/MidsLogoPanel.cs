using Mids_Reborn.Core.Theming;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

public class MidsLogoPanel : Panel
{
    private readonly Bitmap? _originalLogo;
    private readonly Dictionary<string, Bitmap> _themedLogoCache = new();

    public MidsLogoPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);

        try
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png");
            if (File.Exists(path))
            {
                _originalLogo = new Bitmap(path);
            }
        }
        catch
        {
            // Fails silently if the logo can't be loaded, preventing a crash.
        }

        if (!DesignMode)
        {
            ThemeManager.ThemeChanged += OnThemeChanged;
        }
    }

    private void OnThemeChanged()
    {
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // 1. First, handle the fatal error case where the logo file failed to load.
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

        // 2. Get the NAME of the current theme to use as the cache key.
        string currentThemeName = DesignMode
            ? ThemeManager.DesignTimeThemeName
            : ThemeManager.CurrentTheme?.Name ?? ThemeManager.DesignTimeThemeName;

        // 3. Get the HEADER THEME object to access its colors.
        var headerTheme = DesignMode
            ? ThemeManager.DesignTime.Header
            : ThemeManager.CurrentTheme?.Header ?? ThemeManager.DesignTime.Header;

        Bitmap? logoToDraw;

        // 4. Check if a themed logo already exists in our cache.
        if (_themedLogoCache.TryGetValue(currentThemeName, out var cachedLogo))
        {
            logoToDraw = cachedLogo;
        }
        else
        {
            // 5. If not in cache, generate it using the expensive SkiaSharp process.
            logoToDraw = HueShift.AdjustHueAndSaturation(_originalLogo, headerTheme.LogoTargetColor);
            // 6. Add the newly generated bitmap to the cache for next time.
            _themedLogoCache[currentThemeName] = logoToDraw;
        }

        Graphics g = e.Graphics;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        Rectangle destRect = new Rectangle(0, 0, Width, Height);
        g.DrawImage(logoToDraw, destRect, 0, 0, logoToDraw.Width, logoToDraw.Height, GraphicsUnit.Pixel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ThemeManager.ThemeChanged -= OnThemeChanged;

            // Important: Dispose all the bitmaps we created and cached.
            foreach (var logo in _themedLogoCache.Values)
            {
                logo.Dispose();
            }
            _themedLogoCache.Clear();
            _originalLogo?.Dispose();
        }
        base.Dispose(disposing);
    }
}
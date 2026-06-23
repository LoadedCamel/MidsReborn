using System.ComponentModel;
using System.Drawing.Text;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

[DesignerCategory("Code")]
public sealed class PetActorIconView : Control
{
    private Image? _iconImage;
    private string _fallbackText = string.Empty;

    public Image? IconImage
    {
        get => _iconImage;
        set
        {
            if (ReferenceEquals(_iconImage, value))
            {
                return;
            }

            _iconImage = value;
            Invalidate();
        }
    }

    public string FallbackText
    {
        get => _fallbackText;
        set
        {
            var safeValue = value ?? string.Empty;
            if (string.Equals(_fallbackText, safeValue, StringComparison.Ordinal))
            {
                return;
            }

            _fallbackText = safeValue;
            Invalidate();
        }
    }

    public PetActorIconView()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint |
            ControlStyles.SupportsTransparentBackColor,
            true);

        BackColor = Color.Transparent;
        Margin = Padding.Empty;
        Size = new Size(98, 98);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        PaintParentBackground(e.Graphics);
        var bounds = ClientRectangle;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        if (_iconImage != null)
        {
            DrawImageContain(e.Graphics, _iconImage, Rectangle.Inflate(bounds, -2, -2));
            return;
        }

        DrawFallbackText(e.Graphics, Rectangle.Inflate(bounds, -2, -2), CurrentTheme);
    }

    private void DrawFallbackText(Graphics graphics, Rectangle bounds, DataViewTheme theme)
    {
        var token = string.IsNullOrWhiteSpace(_fallbackText)
            ? "PET"
            : new string(_fallbackText
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(part => char.ToUpperInvariant(part[0]))
                .ToArray());

        using var font = new Font("Segoe UI", Math.Max(10f, bounds.Width / 4.8f), FontStyle.Bold, GraphicsUnit.Pixel);
        TextRenderer.DrawText(
            graphics,
            token,
            font,
            bounds,
            theme.ValueText,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    private static void DrawImageContain(Graphics graphics, Image image, Rectangle bounds)
    {
        var widthScale = bounds.Width / (float)image.Width;
        var heightScale = bounds.Height / (float)image.Height;
        var scale = Math.Min(widthScale, heightScale);
        var drawWidth = image.Width * scale;
        var drawHeight = image.Height * scale;
        var drawX = bounds.X + (bounds.Width - drawWidth) / 2f;
        var drawY = bounds.Y + (bounds.Height - drawHeight) / 2f;
        graphics.DrawImage(image, drawX, drawY, drawWidth, drawHeight);
    }

    private void PaintParentBackground(Graphics g)
    {
        if (Parent == null)
        {
            g.Clear(Color.Transparent);
            return;
        }

        var state = g.Save();
        try
        {
            g.TranslateTransform(-Left, -Top);
            using var paintArgs = new PaintEventArgs(g, new Rectangle(Parent.Location, Parent.Size));
            InvokePaintBackground(Parent, paintArgs);
        }
        finally
        {
            g.Restore(state);
        }
    }

    private DataViewTheme CurrentTheme => DesignMode
        ? ThemeManager.DesignTime.DataView
        : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;
}

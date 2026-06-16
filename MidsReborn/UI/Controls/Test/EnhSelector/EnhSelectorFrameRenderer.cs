using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Mids_Reborn.Core.Base.Display;

namespace Mids_Reborn.UI.Controls.Test.EnhSelector;

internal static class EnhSelectorFrameRenderer
{
    public static void ApplyDefaultQuality(Graphics graphics)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
    }

    public static void DrawOuterFrame(Graphics graphics, Rectangle bounds, EnhSelectorVisualPalette palette, int radius)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        using var path = CreateRoundedRectanglePath(Rectangle.Inflate(bounds, -1, -1), radius);
        using (var fill = new LinearGradientBrush(bounds, palette.BackgroundTop, palette.BackgroundBottom, LinearGradientMode.Vertical))
        {
            graphics.FillPath(fill, path);
        }

        using (var border = new Pen(palette.OuterBorder, 1f))
        {
            graphics.DrawPath(border, path);
        }

        var inner = Rectangle.Inflate(bounds, -3, -3);
        using var innerPath = CreateRoundedRectanglePath(inner, Math.Max(2, radius - 2));
        if (palette.InnerHighlight.A > 0)
        {
            using var highlight = new Pen(palette.InnerHighlight, 1f);
            graphics.DrawPath(highlight, innerPath);
        }
    }

    public static void DrawPanel(Graphics graphics, Rectangle bounds, EnhSelectorVisualPalette palette, int radius)
    {
        DrawPanel(graphics, bounds, palette.PanelTop, palette.PanelBottom, palette.PanelBorder, palette.InnerHighlight, radius);
    }

    public static void DrawPanel(
        Graphics graphics,
        Rectangle bounds,
        Color top,
        Color bottom,
        Color borderColor,
        Color innerHighlight,
        int radius)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var outer = Rectangle.Inflate(bounds, -1, -1);
        using var path = CreateRoundedRectanglePath(outer, radius);

        using (var fill = new LinearGradientBrush(outer, top, bottom, LinearGradientMode.Vertical))
        {
            graphics.FillPath(fill, path);
        }

        using (var border = new Pen(borderColor, 1f))
        {
            graphics.DrawPath(border, path);
        }

        if (innerHighlight.A > 0)
        {
            var inner = Rectangle.Inflate(outer, -2, -2);
            using var innerPath = CreateRoundedRectanglePath(inner, Math.Max(2, radius - 2));
            using var highlight = new Pen(innerHighlight, 1f);
            graphics.DrawPath(highlight, innerPath);
        }
    }

    public static void DrawCard(
        Graphics graphics,
        Rectangle bounds,
        EnhSelectorVisualPalette palette,
        bool selected,
        bool hovered,
        bool disabled,
        int radius = 8)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var top = selected ? palette.CardSelectedTop : hovered ? palette.CardHoverTop : palette.CardTop;
        var bottom = selected ? palette.CardSelectedBottom : hovered ? palette.CardHoverBottom : palette.CardBottom;
        DrawPanel(graphics, bounds, top, bottom, palette.CardBorder, palette.CardInnerBorder, radius);

        if (selected)
        {
            DrawSelectionGlow(graphics, Rectangle.Inflate(bounds, -1, -1), radius, palette.SelectionGlow, palette.SelectionBorder);
        }

        if (disabled)
        {
            using var overlay = new SolidBrush(palette.CardDisabledOverlay);
            using var path = CreateRoundedRectanglePath(Rectangle.Inflate(bounds, -1, -1), radius);
            graphics.FillPath(overlay, path);
        }
    }

    public static void DrawTab(
        Graphics graphics,
        Rectangle bounds,
        EnhSelectorVisualPalette palette,
        bool selected,
        bool hovered,
        int radius = 9)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var top = selected ? palette.TabSelectedTop : hovered ? palette.TabHoverTop : palette.TabTop;
        var bottom = selected ? palette.TabSelectedBottom : hovered ? palette.TabHoverBottom : palette.TabBottom;
        var border = selected ? Color.FromArgb(164, palette.SelectionBorder) : palette.TabBorder;
        DrawPanel(graphics, bounds, top, bottom, border, Color.Transparent, radius);
    }

    public static void DrawSmallButton(
        Graphics graphics,
        Rectangle bounds,
        EnhSelectorVisualPalette palette,
        string text,
        Font font,
        bool hovered,
        bool pressed,
        bool enabled)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var top = enabled ? hovered ? palette.ButtonHoverTop : palette.ButtonTop : palette.ButtonDisabledTop;
        var bottom = enabled ? pressed ? palette.ButtonPressedBottom : hovered ? palette.ButtonHoverBottom : palette.ButtonBottom : palette.ButtonDisabledBottom;
        var border = enabled ? palette.ButtonBorder : palette.ButtonDisabledBorder;

        DrawPanel(graphics, bounds, top, bottom, border, palette.InnerHighlight, 6);

        using var brush = new SolidBrush(enabled ? palette.Text : palette.MutedText);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        var textRect = pressed ? new Rectangle(bounds.X, bounds.Y + 1, bounds.Width, bounds.Height) : bounds;
        graphics.DrawString(text, font, brush, textRect, format);
    }

    public static void DrawCloseButton(Graphics graphics, Rectangle bounds, EnhSelectorVisualPalette palette, bool hovered, bool pressed)
    {
        DrawSmallButton(graphics, bounds, palette, "X", SystemFonts.CaptionFont, hovered, pressed, true);
    }

    public static void DrawChip(Graphics graphics, Rectangle bounds, EnhSelectorVisualPalette palette, string text, Font font, bool emphasized)
    {
        DrawChip(
            graphics,
            bounds,
            palette,
            new EnhSelectorChip
            {
                Text = text,
                Emphasized = emphasized
            },
            font);
    }

    public static void DrawChip(Graphics graphics, Rectangle bounds, EnhSelectorVisualPalette palette, EnhSelectorChip chip, Font font)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var (top, bottom, border, textColor) = ResolveChipColors(chip, palette);
        DrawPanel(graphics, bounds, top, bottom, border, Color.Transparent, 6);
        DrawText(graphics, chip.Text, font, textColor, bounds, ContentAlignment.MiddleCenter, StringTrimming.EllipsisCharacter);
    }

    public static void DrawCenteredText(Graphics graphics, string text, Font font, Color color, Rectangle bounds)
    {
        using var brush = new SolidBrush(color);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        graphics.DrawString(text, font, brush, bounds, format);
    }

    public static void DrawText(
        Graphics graphics,
        string text,
        Font font,
        Color color,
        Rectangle bounds,
        ContentAlignment alignment = ContentAlignment.MiddleLeft,
        StringTrimming trimming = StringTrimming.EllipsisWord)
    {
        using var brush = new SolidBrush(color);
        using var format = CreateStringFormat(alignment, trimming);
        graphics.DrawString(text, font, brush, bounds, format);
    }

    public static void DrawDivider(Graphics graphics, Rectangle bounds, EnhSelectorVisualPalette palette, string label, Font font)
    {
        using var linePen = new Pen(palette.Divider, 1f);
        using var textBrush = new SolidBrush(palette.SectionText);

        var textSize = graphics.MeasureString(label, font);
        var centerY = bounds.Top + bounds.Height / 2;
        var textX = bounds.Left + (bounds.Width - textSize.Width) / 2f;
        const int gap = 16;

        graphics.DrawLine(linePen, bounds.Left, centerY, (int)textX - gap, centerY);
        graphics.DrawLine(linePen, (int)(textX + textSize.Width) + gap, centerY, bounds.Right, centerY);

        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        graphics.DrawString(label, font, textBrush, bounds, format);
    }

    public static void DrawEmptyEnhancementSocket(Graphics graphics, Rectangle bounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var palette = CreateDefaultSocketPalette();
        var drawRect = bounds;
        var socketRect = GetSocketRect(drawRect, 0.10f, 2);
        var scale = Math.Max(0.5f, Math.Min(drawRect.Width, drawRect.Height) / 64f);
        var shadowOffset = Math.Max(1, (int)Math.Round(scale));

        var shadowRect = drawRect;
        shadowRect.Offset(shadowOffset, shadowOffset);
        shadowRect.Inflate(shadowOffset, shadowOffset);

        using (var shadowPath = new GraphicsPath())
        {
            shadowPath.AddEllipse(shadowRect);
            using var shadowBrush = new PathGradientBrush(shadowPath)
            {
                CenterColor = Color.FromArgb(54, 0, 0, 0),
                CenterPoint = new PointF(
                    shadowRect.X + shadowRect.Width * 0.52f,
                    shadowRect.Y + shadowRect.Height * 0.55f),
                SurroundColors = Enumerable.Repeat(Color.FromArgb(0, 0, 0, 0), shadowPath.PathPoints.Length).ToArray(),
                FocusScales = new PointF(0.38f, 0.38f)
            };
            graphics.FillPath(shadowBrush, shadowPath);
        }

        DrawIconWell(graphics, drawRect, palette, socketRect, scale);
        DrawIconWellOverlay(graphics, drawRect, palette, socketRect, scale);
    }

    public static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();

        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            path.AddRectangle(Rectangle.Empty);
            return path;
        }

        var diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
        if (diameter <= 0)
        {
            path.AddRectangle(bounds);
            path.CloseFigure();
            return path;
        }

        var arc = new Rectangle(bounds.Left, bounds.Top, diameter, diameter);
        path.AddArc(arc, 180, 90);

        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);

        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();
        return path;
    }

    private static void DrawSelectionGlow(Graphics graphics, Rectangle bounds, int radius, Color glowColor, Color borderColor)
    {
        using var glowPath = CreateRoundedRectanglePath(bounds, radius);
        using var glowPen = new Pen(Color.FromArgb(180, glowColor), 2f);
        using var borderPen = new Pen(borderColor, 1.25f);
        graphics.DrawPath(glowPen, glowPath);
        graphics.DrawPath(borderPen, glowPath);
    }

    private static void DrawIconWellOverlay(Graphics graphics, Rectangle wellRect, SocketPalette palette, Rectangle socketRect, float scale)
    {
        if (wellRect.Width <= 0 || wellRect.Height <= 0 || socketRect.Width <= 0 || socketRect.Height <= 0)
            return;

        using var socketPath = new GraphicsPath();
        socketPath.AddEllipse(socketRect);

        var cavityShadow = Blend(palette.FillBottom, Color.Black, 0.82f);
        var deepShadow = Blend(cavityShadow, Color.Black, 0.26f);
        var upperShadow = Blend(palette.InnerShadow, Color.Black, 0.64f);

        var upperLeftShadeState = graphics.Save();
        graphics.SetClip(socketPath, CombineMode.Intersect);
        using (var upperLeftShadeBrush = new LinearGradientBrush(
                   new PointF(socketRect.Left, socketRect.Top),
                   new PointF(socketRect.Right, socketRect.Bottom),
                   Color.FromArgb(72, upperShadow),
                   Color.FromArgb(0, upperShadow)))
        {
            graphics.FillPath(upperLeftShadeBrush, socketPath);
        }

        graphics.Restore(upperLeftShadeState);

        var aoWidth = Math.Max(2.0f, scale * 2.2f);
        var ambientOcclusionState = graphics.Save();
        graphics.SetClip(socketPath, CombineMode.Intersect);
        using (var aoPen = new Pen(Color.FromArgb(60, upperShadow), aoWidth))
        {
            graphics.DrawEllipse(aoPen, socketRect);
        }

        graphics.Restore(ambientOcclusionState);

        var upperArcState = graphics.Save();
        graphics.SetClip(socketPath, CombineMode.Intersect);
        graphics.SetClip(new Rectangle(
            socketRect.X,
            socketRect.Y,
            Math.Max(2, (int)Math.Round(socketRect.Width * 0.68f)),
            Math.Max(2, (int)Math.Round(socketRect.Height * 0.62f))),
            CombineMode.Intersect);
        using (var upperArcPen = new Pen(Color.FromArgb(92, Blend(upperShadow, Color.Black, 0.24f)), Math.Max(2.2f, scale * 2.4f)))
        {
            graphics.DrawEllipse(upperArcPen, socketRect);
        }

        graphics.Restore(upperArcState);

        var innerOcclusionRect = DeflateRect(socketRect, Math.Max(1, (int)Math.Round(scale)));
        if (innerOcclusionRect.Width > 0 && innerOcclusionRect.Height > 0)
        {
            var innerOcclusionState = graphics.Save();
            graphics.SetClip(socketPath, CombineMode.Intersect);
            graphics.SetClip(new Rectangle(
                innerOcclusionRect.X,
                innerOcclusionRect.Y,
                Math.Max(2, (int)Math.Round(innerOcclusionRect.Width * 0.76f)),
                Math.Max(2, (int)Math.Round(innerOcclusionRect.Height * 0.72f))),
                CombineMode.Intersect);
            using (var innerOcclusionPen = new Pen(Color.FromArgb(70, deepShadow), Math.Max(1.2f, scale * 1.45f)))
            {
                graphics.DrawEllipse(innerOcclusionPen, innerOcclusionRect);
            }

            graphics.Restore(innerOcclusionState);
        }

        var innerStrokeRect = DeflateRect(socketRect, Math.Max(1, (int)Math.Round(scale)));
        if (innerStrokeRect.Width > 0 && innerStrokeRect.Height > 0)
        {
            using var socketInnerShadowStroke = new Pen(Color.FromArgb(56, Blend(palette.OuterStroke, Color.Black, 0.24f)));
            graphics.DrawEllipse(socketInnerShadowStroke, innerStrokeRect);
        }
    }

    private static void DrawIconWell(Graphics graphics, Rectangle wellRect, SocketPalette palette, Rectangle socketRect, float scale)
    {
        if (wellRect.Width <= 0 || wellRect.Height <= 0)
            return;

        var ringRect = DeflateRect(wellRect, Math.Max(2, (int)Math.Round(scale * 2)));
        var midRingRect = DeflateRect(wellRect, Math.Max(3, (int)Math.Round(scale * 3)));
        var innerBevelRect = DeflateRect(wellRect, Math.Max(4, (int)Math.Round(scale * 4)));

        if (ringRect.Width <= 0 || ringRect.Height <= 0)
            return;

        using var ringPath = new GraphicsPath();
        ringPath.AddEllipse(ringRect);
        using var ringBrush = CreateThreeStopBrush(ringRect, palette.RimTop, Blend(palette.RimTop, palette.RimBottom, 0.45f), palette.RimBottom);
        graphics.FillPath(ringBrush, ringPath);

        if (midRingRect.Width > 0 && midRingRect.Height > 0)
        {
            using var midRingPath = new GraphicsPath();
            midRingPath.AddEllipse(midRingRect);
            using var midRingBrush = CreateThreeStopBrush(
                midRingRect,
                Blend(palette.RimTop, Color.White, 0.20f),
                Blend(palette.RimTop, palette.FillTop, 0.28f),
                Blend(palette.RimBottom, Color.Black, 0.10f));
            graphics.FillPath(midRingBrush, midRingPath);
        }

        if (innerBevelRect.Width > 0 && innerBevelRect.Height > 0)
        {
            using var innerBevelPath = new GraphicsPath();
            innerBevelPath.AddEllipse(innerBevelRect);
            using var innerBevelBrush = CreateThreeStopBrush(
                innerBevelRect,
                Blend(palette.RimTop, palette.FillTop, 0.16f),
                Blend(palette.RimBottom, palette.FillBottom, 0.34f),
                Blend(palette.RimBottom, Color.Black, 0.36f));
            graphics.FillPath(innerBevelBrush, innerBevelPath);
        }

        if (socketRect.Width <= 0 || socketRect.Height <= 0)
            return;

        using var socketPath = new GraphicsPath();
        socketPath.AddEllipse(socketRect);
        var cavityBase = Blend(palette.FillBottom, Color.Black, 0.66f);
        using (var socketBaseBrush = new SolidBrush(cavityBase))
        {
            graphics.FillPath(socketBaseBrush, socketPath);
        }

        using (var cavityDepthBrush = new PathGradientBrush(socketPath)
        {
            CenterColor = Color.FromArgb(216, Color.Black),
            CenterPoint = new PointF(socketRect.X + socketRect.Width * 0.50f, socketRect.Y + socketRect.Height * 0.50f),
            SurroundColors = Enumerable.Repeat(Color.FromArgb(0, Color.Black), socketPath.PathPoints.Length).ToArray(),
            FocusScales = new PointF(0.28f, 0.28f)
        })
        {
            graphics.FillPath(cavityDepthBrush, socketPath);
        }

        var ringHighlightState = graphics.Save();
        graphics.SetClip(new Rectangle(ringRect.X, ringRect.Y, ringRect.Width, Math.Max(2, ringRect.Height / 2)), CombineMode.Intersect);
        using (var ringHighlightPen = new Pen(Color.FromArgb(88, palette.HighlightStroke), Math.Max(1.0f, scale)))
        {
            graphics.DrawPath(ringHighlightPen, ringPath);
        }

        graphics.Restore(ringHighlightState);
    }

    private static SocketPalette CreateDefaultSocketPalette()
    {
        return CreateSocketPalette(
            Color.FromArgb(20, 96, 232),
            Color.FromArgb(104, 188, 255),
            Color.FromArgb(6, 52, 156),
            Color.FromArgb(132, 226, 255));
    }

    private static SocketPalette CreateSocketPalette(Color rimBase, Color fillTop, Color fillBottom, Color highlightBase)
    {
        var outerStroke = Blend(rimBase, Color.Black, 0.52f);
        var rimTop = Blend(rimBase, Color.White, 0.18f);
        var rimBottom = Blend(rimBase, Color.Black, 0.22f);
        var fillMid = Blend(fillTop, fillBottom, 0.56f);
        var glossTop = Color.FromArgb(92, highlightBase);
        var glossBottom = Color.FromArgb(0, highlightBase);
        var innerShadow = Color.FromArgb(104, Blend(fillBottom, Color.Black, 0.58f));
        var highlightStroke = Color.FromArgb(122, Blend(highlightBase, Color.White, 0.14f));
        return new SocketPalette(outerStroke, rimTop, rimBottom, fillTop, fillMid, fillBottom, glossTop, glossBottom, innerShadow, highlightStroke);
    }

    private static Rectangle GetSocketRect(Rectangle outerRect, float insetRatio, int minInset)
    {
        var inset = Math.Max(minInset, (int)Math.Round(outerRect.Width * insetRatio));
        return DeflateRect(outerRect, inset);
    }

    private static Rectangle DeflateRect(Rectangle rect, int amount)
    {
        return new Rectangle(
            rect.X + amount,
            rect.Y + amount,
            Math.Max(0, rect.Width - amount * 2),
            Math.Max(0, rect.Height - amount * 2));
    }

    private static LinearGradientBrush CreateThreeStopBrush(Rectangle bounds, Color top, Color middle, Color bottom)
    {
        var brush = new LinearGradientBrush(bounds, top, bottom, 90f);
        brush.InterpolationColors = new ColorBlend
        {
            Colors = [top, middle, bottom],
            Positions = [0f, 0.52f, 1f]
        };
        return brush;
    }

    private static Color Blend(Color first, Color second, float amountSecond)
    {
        amountSecond = Math.Clamp(amountSecond, 0f, 1f);
        var amountFirst = 1f - amountSecond;
        return Color.FromArgb(
            (int)Math.Round(first.A * amountFirst + second.A * amountSecond),
            (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
            (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
            (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
    }

    private static (Color Top, Color Bottom, Color Border, Color Text) ResolveChipColors(EnhSelectorChip chip, EnhSelectorVisualPalette palette)
    {
        return chip.Style switch
        {
            EnhSelectorChipStyle.RarityCommon => (
                Color.FromArgb(34, 56, 82),
                Color.FromArgb(16, 28, 44),
                Color.FromArgb(180, PopUp.Colors.Common),
                palette.Text),
            EnhSelectorChipStyle.RarityUncommon => (
                Color.FromArgb(86, 86, 18),
                Color.FromArgb(54, 50, 10),
                Color.FromArgb(196, PopUp.Colors.Uncommon),
                Color.FromArgb(255, 252, 228)),
            EnhSelectorChipStyle.RarityRare => (
                Color.FromArgb(112, 62, 12),
                Color.FromArgb(70, 34, 8),
                Color.FromArgb(204, PopUp.Colors.Rare),
                Color.FromArgb(255, 242, 228)),
            EnhSelectorChipStyle.RarityUltraRare => (
                Color.FromArgb(86, 40, 112),
                Color.FromArgb(50, 20, 74),
                Color.FromArgb(204, PopUp.Colors.UltraRare),
                Color.FromArgb(250, 238, 255)),
            _ => chip.Emphasized
                ? (
                    Color.FromArgb(31, 72, 114),
                    Color.FromArgb(21, 52, 85),
                    Color.FromArgb(136, 170, 212, 246),
                    palette.Text)
                : (
                    Color.FromArgb(12, 33, 54),
                    Color.FromArgb(6, 18, 33),
                    Color.FromArgb(88, 82, 132, 176),
                    palette.HeaderText)
        };
    }

    private static StringFormat CreateStringFormat(ContentAlignment alignment, StringTrimming trimming)
    {
        var format = new StringFormat
        {
            Trimming = trimming,
            FormatFlags = StringFormatFlags.LineLimit
        };

        format.Alignment = alignment switch
        {
            ContentAlignment.TopLeft or ContentAlignment.MiddleLeft or ContentAlignment.BottomLeft => StringAlignment.Near,
            ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter => StringAlignment.Center,
            _ => StringAlignment.Far
        };

        format.LineAlignment = alignment switch
        {
            ContentAlignment.TopLeft or ContentAlignment.TopCenter or ContentAlignment.TopRight => StringAlignment.Near,
            ContentAlignment.MiddleLeft or ContentAlignment.MiddleCenter or ContentAlignment.MiddleRight => StringAlignment.Center,
            _ => StringAlignment.Far
        };

        return format;
    }

    private readonly record struct SocketPalette(
        Color OuterStroke,
        Color RimTop,
        Color RimBottom,
        Color FillTop,
        Color FillMid,
        Color FillBottom,
        Color GlossTop,
        Color GlossBottom,
        Color InnerShadow,
        Color HighlightStroke);
}

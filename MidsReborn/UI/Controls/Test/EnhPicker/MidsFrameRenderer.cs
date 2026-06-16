using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Mids_Reborn.UI.Controls.Test.EnhPicker
{
    public static class MidsFrameRenderer
    {
        public static void ApplyDefaultQuality(Graphics graphics)
        {
            ArgumentNullException.ThrowIfNull(graphics);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        public static void DrawOuterFrame(Graphics graphics, Rectangle bounds, I9PickerVisualPalette palette, int radius)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            using var path = CreateRoundedRectanglePath(Rectangle.Inflate(bounds, -1, -1), radius);

            using (var fill = new LinearGradientBrush(
                       bounds,
                       palette.BackgroundTop,
                       palette.BackgroundBottom,
                       LinearGradientMode.Vertical))
            {
                graphics.FillPath(fill, path);
            }

            using (var border = new Pen(palette.OuterBorder, 1.35f))
            {
                graphics.DrawPath(border, path);
            }

            var inner = Rectangle.Inflate(bounds, -3, -3);
            using var innerPath = CreateRoundedRectanglePath(inner, Math.Max(2, radius - 2));

            using (var highlight = new Pen(palette.InnerHighlight, 1f))
            {
                graphics.DrawPath(highlight, innerPath);
            }
        }

        public static void DrawPanel(Graphics graphics, Rectangle bounds, I9PickerVisualPalette palette, int radius)
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

            var inner = Rectangle.Inflate(outer, -2, -2);
            using var innerPath = CreateRoundedRectanglePath(inner, Math.Max(2, radius - 2));
            using var highlight = new Pen(Color.FromArgb(
                Math.Min(56, innerHighlight.A == 0 ? 56 : innerHighlight.A),
                innerHighlight.R,
                innerHighlight.G,
                innerHighlight.B), 1f);

            graphics.DrawPath(highlight, innerPath);
        }

        public static void DrawIconTile(
            Graphics graphics,
            Rectangle bounds,
            I9PickerVisualPalette palette,
            bool selected,
            bool hovered,
            bool disabled,
            int radius = 8)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            var outer = Rectangle.Inflate(bounds, -1, -1);
            using var path = CreateRoundedRectanglePath(outer, radius);

            if (disabled)
            {
                using var overlay = new SolidBrush(Color.FromArgb(72, 0, 0, 0));
                graphics.FillPath(overlay, path);
            }

            if (selected)
            {
                DrawSelectionGlow(graphics, outer, radius, palette.SelectionGlow, palette.SelectionBorder);
            }
            else if (hovered)
            {
                using var hoverPen = new Pen(Color.FromArgb(180, palette.SelectionBorder), 1.25f);
                graphics.DrawPath(hoverPen, path);
            }
        }

        public static void DrawSmallButton(
            Graphics graphics,
            Rectangle bounds,
            I9PickerVisualPalette palette,
            string text,
            Font font,
            bool hovered,
            bool pressed,
            bool enabled)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            var top = enabled
                ? hovered ? palette.ButtonHoverTop : palette.ButtonTop
                : palette.ButtonDisabledTop;

            var bottom = enabled
                ? pressed ? palette.ButtonPressedBottom : palette.ButtonBottom
                : palette.ButtonDisabledBottom;

            DrawPanel(
                graphics,
                bounds,
                top,
                bottom,
                enabled ? palette.ButtonBorder : palette.ButtonDisabledBorder,
                enabled ? palette.InnerHighlight : Color.FromArgb(35, 70, 90, 105),
                5);

            using var brush = new SolidBrush(enabled ? palette.Text : palette.MutedText);
            using var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var textRect = pressed
                ? new Rectangle(bounds.X, bounds.Y + 1, bounds.Width, bounds.Height)
                : bounds;

            graphics.DrawString(text, font, brush, textRect, format);
        }

        public static void DrawCenteredText(
            Graphics graphics,
            string text,
            Font font,
            Color color,
            Rectangle bounds)
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

        public static void DrawDivider(Graphics graphics, Rectangle bounds, I9PickerVisualPalette palette, string label, Font font)
        {
            using var linePen = new Pen(palette.Divider, 1f);
            using var textBrush = new SolidBrush(palette.SectionText);

            var textSize = graphics.MeasureString(label, font);
            var centerY = bounds.Top + bounds.Height / 2;
            var textX = bounds.Left + (bounds.Width - textSize.Width) / 2f;
            var gap = 16;

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

        private static void DrawSelectionGlow(
            Graphics graphics,
            Rectangle bounds,
            int radius,
            Color glow,
            Color border)
        {
            using var glowPath = CreateRoundedRectanglePath(bounds, radius);

            using (var glowPen = new Pen(Color.FromArgb(58, glow), 3f))
            {
                graphics.DrawPath(glowPen, glowPath);
            }

            using (var borderPen = new Pen(border, 1.45f))
            {
                graphics.DrawPath(borderPen, glowPath);
            }
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
                ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter => StringAlignment.Center,
                ContentAlignment.TopRight or ContentAlignment.MiddleRight or ContentAlignment.BottomRight => StringAlignment.Far,
                _ => StringAlignment.Near
            };

            format.LineAlignment = alignment switch
            {
                ContentAlignment.TopLeft or ContentAlignment.TopCenter or ContentAlignment.TopRight => StringAlignment.Near,
                ContentAlignment.BottomLeft or ContentAlignment.BottomCenter or ContentAlignment.BottomRight => StringAlignment.Far,
                _ => StringAlignment.Center
            };

            return format;
        }

    }
}

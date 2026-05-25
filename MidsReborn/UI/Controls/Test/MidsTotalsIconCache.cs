using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Mids_Reborn.UI.Controls.Test;

internal static class MidsTotalsIconCache
{
    private static readonly Dictionary<string, Bitmap> Cache = new(StringComparer.Ordinal);

    public static void Draw(Graphics g, Rectangle bounds, MidsTotalsGlyph glyph, Color color)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        var state = g.Save();
        try
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            DrawGlyph(g, bounds, glyph, color);
        }
        finally
        {
            g.Restore(state);
        }
    }

    public static Bitmap Get(MidsTotalsGlyph glyph, Color color, int size)
    {
        size = Math.Max(8, size);
        var key = $"{(int)glyph}:{color.ToArgb()}:{size}";
        if (Cache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var bitmap = new Bitmap(size, size, PixelFormat.Format32bppPArgb);
        using var g = Graphics.FromImage(bitmap);
        g.Clear(Color.Transparent);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        DrawGlyph(g, new Rectangle(0, 0, size, size), glyph, color);
        Cache[key] = bitmap;
        return bitmap;
    }

    private static void DrawGlyph(Graphics g, Rectangle bounds, MidsTotalsGlyph glyph, Color color)
    {
        var inner = RectangleF.Inflate(bounds, -Math.Max(1, bounds.Width * 0.12f), -Math.Max(1, bounds.Height * 0.12f));
        using var fill = new SolidBrush(color);
        using var soft = new SolidBrush(Color.FromArgb(180, Blend(color, Color.White, 0.18f)));
        using var pen = new Pen(color, Math.Max(1.4f, bounds.Width / 10f))
        {
            LineJoin = LineJoin.Round,
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };
        using var thinPen = new Pen(color, Math.Max(1f, bounds.Width / 16f))
        {
            LineJoin = LineJoin.Round,
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };

        switch (glyph)
        {
            case MidsTotalsGlyph.QuickRecharge:
                DrawBolt(g, inner, fill);
                break;
            case MidsTotalsGlyph.QuickRecovery:
                DrawPlus(g, inner, fill);
                break;
            case MidsTotalsGlyph.QuickRegen:
                DrawHeart(g, inner, fill);
                break;
            case MidsTotalsGlyph.QuickEndDrain:
                DrawDroplet(g, inner, fill);
                break;
            case MidsTotalsGlyph.SectionDefense:
            case MidsTotalsGlyph.StatEndRdx:
                DrawShield(g, inner, pen, false);
                break;
            case MidsTotalsGlyph.SectionResistance:
                DrawShield(g, inner, pen, true);
                break;
            case MidsTotalsGlyph.SectionCore:
            case MidsTotalsGlyph.StatToHit:
            case MidsTotalsGlyph.StatAccuracy:
                DrawTarget(g, inner, thinPen);
                break;
            case MidsTotalsGlyph.DamageSmashing:
                DrawBurst(g, inner, fill, 8, 0.58f);
                break;
            case MidsTotalsGlyph.DamageLethal:
                DrawSkull(g, inner, fill);
                break;
            case MidsTotalsGlyph.DamageEnergy:
                DrawSpark(g, inner, fill);
                break;
            case MidsTotalsGlyph.DamageNegative:
                DrawSwirl(g, inner, pen);
                break;
            case MidsTotalsGlyph.DamageToxic:
                DrawToxic(g, inner, pen);
                break;
            case MidsTotalsGlyph.DamagePsionic:
                DrawPsi(g, inner, pen);
                break;
            case MidsTotalsGlyph.DamageFire:
                DrawFlame(g, inner, fill);
                break;
            case MidsTotalsGlyph.DamageCold:
                DrawSnowflake(g, inner, thinPen);
                break;
            case MidsTotalsGlyph.DamageMelee:
                DrawCrossedBlades(g, inner, thinPen);
                break;
            case MidsTotalsGlyph.DamageRanged:
                DrawTarget(g, inner, thinPen);
                break;
            case MidsTotalsGlyph.DamageAoE:
            case MidsTotalsGlyph.StatDamage:
                DrawBurst(g, inner, fill, 10, 0.68f);
                break;
            case MidsTotalsGlyph.StatRange:
                DrawRange(g, inner, thinPen, soft);
                break;
            case MidsTotalsGlyph.StatThreat:
                DrawThreat(g, inner, pen);
                break;
            default:
                g.FillEllipse(fill, inner);
                break;
        }
    }

    private static void DrawBolt(Graphics g, RectangleF rect, Brush fill)
    {
        using var path = new GraphicsPath();
        path.AddPolygon(
        [
            new PointF(rect.Left + rect.Width * 0.60f, rect.Top),
            new PointF(rect.Left + rect.Width * 0.24f, rect.Top + rect.Height * 0.48f),
            new PointF(rect.Left + rect.Width * 0.48f, rect.Top + rect.Height * 0.48f),
            new PointF(rect.Left + rect.Width * 0.32f, rect.Bottom),
            new PointF(rect.Right - rect.Width * 0.10f, rect.Top + rect.Height * 0.40f),
            new PointF(rect.Left + rect.Width * 0.58f, rect.Top + rect.Height * 0.40f)
        ]);
        g.FillPath(fill, path);
    }

    private static void DrawPlus(Graphics g, RectangleF rect, Brush fill)
    {
        var arm = rect.Width * 0.22f;
        var cx = rect.Left + rect.Width / 2f;
        var cy = rect.Top + rect.Height / 2f;
        g.FillRectangle(fill, cx - arm / 2f, rect.Top + rect.Height * 0.10f, arm, rect.Height * 0.80f);
        g.FillRectangle(fill, rect.Left + rect.Width * 0.10f, cy - arm / 2f, rect.Width * 0.80f, arm);
    }

    private static void DrawHeart(Graphics g, RectangleF rect, Brush fill)
    {
        using var path = new GraphicsPath();
        var midX = rect.Left + rect.Width / 2f;
        var topY = rect.Top + rect.Height * 0.28f;
        path.AddBezier(midX, rect.Bottom - rect.Height * 0.08f, rect.Right, rect.Top + rect.Height * 0.50f, rect.Right - rect.Width * 0.02f, rect.Top, midX, topY);
        path.AddBezier(midX, topY, rect.Left + rect.Width * 0.02f, rect.Top, rect.Left, rect.Top + rect.Height * 0.50f, midX, rect.Bottom - rect.Height * 0.08f);
        g.FillPath(fill, path);
    }

    private static void DrawDroplet(Graphics g, RectangleF rect, Brush fill)
    {
        using var path = new GraphicsPath();
        var top = new PointF(rect.Left + rect.Width * 0.50f, rect.Top);
        path.AddBezier(top, new PointF(rect.Right, rect.Top + rect.Height * 0.38f), new PointF(rect.Right - rect.Width * 0.02f, rect.Bottom), new PointF(rect.Left + rect.Width * 0.50f, rect.Bottom));
        path.AddBezier(new PointF(rect.Left + rect.Width * 0.50f, rect.Bottom), new PointF(rect.Left + rect.Width * 0.02f, rect.Bottom), new PointF(rect.Left, rect.Top + rect.Height * 0.38f), top);
        g.FillPath(fill, path);
    }

    private static void DrawShield(Graphics g, RectangleF rect, Pen pen, bool split)
    {
        using var path = new GraphicsPath();
        path.AddPolygon(
        [
            new PointF(rect.Left + rect.Width * 0.18f, rect.Top + rect.Height * 0.12f),
            new PointF(rect.Right - rect.Width * 0.18f, rect.Top + rect.Height * 0.12f),
            new PointF(rect.Right - rect.Width * 0.18f, rect.Top + rect.Height * 0.46f),
            new PointF(rect.Left + rect.Width * 0.50f, rect.Bottom - rect.Height * 0.06f),
            new PointF(rect.Left + rect.Width * 0.18f, rect.Top + rect.Height * 0.46f)
        ]);
        g.DrawPath(pen, path);
        if (split)
        {
            g.DrawLine(pen, rect.Left + rect.Width * 0.50f, rect.Top + rect.Height * 0.16f, rect.Left + rect.Width * 0.50f, rect.Bottom - rect.Height * 0.16f);
        }
    }

    private static void DrawTarget(Graphics g, RectangleF rect, Pen pen)
    {
        g.DrawEllipse(pen, rect);
        var inner = RectangleF.Inflate(rect, -rect.Width * 0.26f, -rect.Height * 0.26f);
        g.DrawEllipse(pen, inner);
        var cx = rect.Left + rect.Width / 2f;
        var cy = rect.Top + rect.Height / 2f;
        g.DrawLine(pen, cx, rect.Top, cx, rect.Top + rect.Height * 0.22f);
        g.DrawLine(pen, cx, rect.Bottom, cx, rect.Bottom - rect.Height * 0.22f);
        g.DrawLine(pen, rect.Left, cy, rect.Left + rect.Width * 0.22f, cy);
        g.DrawLine(pen, rect.Right, cy, rect.Right - rect.Width * 0.22f, cy);
    }

    private static void DrawBurst(Graphics g, RectangleF rect, Brush fill, int points, float innerScale)
    {
        using var path = new GraphicsPath();
        var cx = rect.Left + rect.Width / 2f;
        var cy = rect.Top + rect.Height / 2f;
        var outer = rect.Width / 2f;
        var inner = outer * innerScale;
        var pts = new PointF[points * 2];
        for (var i = 0; i < pts.Length; i++)
        {
            var angle = (-90 + (360f / pts.Length) * i) * (float)Math.PI / 180f;
            var radius = i % 2 == 0 ? outer : inner;
            pts[i] = new PointF(cx + (float)Math.Cos(angle) * radius, cy + (float)Math.Sin(angle) * radius);
        }
        path.AddPolygon(pts);
        g.FillPath(fill, path);
    }

    private static void DrawSkull(Graphics g, RectangleF rect, Brush fill)
    {
        using var path = new GraphicsPath(FillMode.Alternate);
        var head = new RectangleF(rect.Left + rect.Width * 0.10f, rect.Top + rect.Height * 0.02f, rect.Width * 0.80f, rect.Height * 0.62f);
        path.AddEllipse(head);
        path.AddEllipse(rect.Left + rect.Width * 0.24f, rect.Top + rect.Height * 0.22f, rect.Width * 0.16f, rect.Height * 0.16f);
        path.AddEllipse(rect.Left + rect.Width * 0.60f, rect.Top + rect.Height * 0.22f, rect.Width * 0.16f, rect.Height * 0.16f);
        path.AddPie(rect.Left + rect.Width * 0.40f, rect.Top + rect.Height * 0.34f, rect.Width * 0.20f, rect.Height * 0.18f, 0, 180);
        g.FillPath(fill, path);
        g.FillRectangle(fill, rect.Left + rect.Width * 0.22f, rect.Top + rect.Height * 0.56f, rect.Width * 0.56f, rect.Height * 0.18f);
    }

    private static void DrawSpark(Graphics g, RectangleF rect, Brush fill)
    {
        using var path = new GraphicsPath();
        path.AddPolygon(
        [
            new PointF(rect.Left + rect.Width * 0.50f, rect.Top),
            new PointF(rect.Left + rect.Width * 0.66f, rect.Top + rect.Height * 0.34f),
            new PointF(rect.Right, rect.Top + rect.Height * 0.50f),
            new PointF(rect.Left + rect.Width * 0.66f, rect.Top + rect.Height * 0.66f),
            new PointF(rect.Left + rect.Width * 0.50f, rect.Bottom),
            new PointF(rect.Left + rect.Width * 0.34f, rect.Top + rect.Height * 0.66f),
            new PointF(rect.Left, rect.Top + rect.Height * 0.50f),
            new PointF(rect.Left + rect.Width * 0.34f, rect.Top + rect.Height * 0.34f)
        ]);
        g.FillPath(fill, path);
    }

    private static void DrawSwirl(Graphics g, RectangleF rect, Pen pen)
    {
        g.DrawArc(pen, rect, 35, 300);
        var inner = RectangleF.Inflate(rect, -rect.Width * 0.28f, -rect.Height * 0.28f);
        g.DrawArc(pen, inner, 220, 280);
    }

    private static void DrawToxic(Graphics g, RectangleF rect, Pen pen)
    {
        var cx = rect.Left + rect.Width / 2f;
        var cy = rect.Top + rect.Height / 2f;
        var r = rect.Width * 0.14f;
        g.DrawEllipse(pen, cx - r, cy - r, r * 2, r * 2);
        var orbit = rect.Width * 0.28f;
        for (var i = 0; i < 3; i++)
        {
            var angle = (-90 + i * 120) * Math.PI / 180d;
            var ox = cx + (float)Math.Cos(angle) * orbit * 0.58f;
            var oy = cy + (float)Math.Sin(angle) * orbit * 0.58f;
            g.DrawArc(pen, ox - orbit * 0.42f, oy - orbit * 0.42f, orbit * 0.84f, orbit * 0.84f, -30 + i * 120, 120);
        }
    }

    private static void DrawPsi(Graphics g, RectangleF rect, Pen pen)
    {
        var cx = rect.Left + rect.Width / 2f;
        g.DrawArc(pen, rect.Left + rect.Width * 0.14f, rect.Top + rect.Height * 0.08f, rect.Width * 0.32f, rect.Height * 0.52f, 250, 220);
        g.DrawArc(pen, rect.Left + rect.Width * 0.54f, rect.Top + rect.Height * 0.08f, rect.Width * 0.32f, rect.Height * 0.52f, 70, 220);
        g.DrawLine(pen, cx, rect.Top + rect.Height * 0.28f, cx, rect.Bottom);
    }

    private static void DrawFlame(Graphics g, RectangleF rect, Brush fill)
    {
        using var path = new GraphicsPath();
        path.AddBezier(new PointF(rect.Left + rect.Width * 0.48f, rect.Top), new PointF(rect.Right, rect.Top + rect.Height * 0.28f), new PointF(rect.Right, rect.Bottom), new PointF(rect.Left + rect.Width * 0.54f, rect.Bottom));
        path.AddBezier(new PointF(rect.Left + rect.Width * 0.54f, rect.Bottom), new PointF(rect.Left + rect.Width * 0.20f, rect.Bottom), new PointF(rect.Left, rect.Top + rect.Height * 0.46f), new PointF(rect.Left + rect.Width * 0.48f, rect.Top));
        g.FillPath(fill, path);
    }

    private static void DrawSnowflake(Graphics g, RectangleF rect, Pen pen)
    {
        var cx = rect.Left + rect.Width / 2f;
        var cy = rect.Top + rect.Height / 2f;
        var r = rect.Width * 0.40f;
        for (var i = 0; i < 3; i++)
        {
            var angle = i * 60 * Math.PI / 180d;
            var dx = (float)Math.Cos(angle) * r;
            var dy = (float)Math.Sin(angle) * r;
            g.DrawLine(pen, cx - dx, cy - dy, cx + dx, cy + dy);
        }
    }

    private static void DrawCrossedBlades(Graphics g, RectangleF rect, Pen pen)
    {
        g.DrawLine(pen, rect.Left + rect.Width * 0.20f, rect.Bottom - rect.Height * 0.18f, rect.Right - rect.Width * 0.16f, rect.Top + rect.Height * 0.18f);
        g.DrawLine(pen, rect.Left + rect.Width * 0.16f, rect.Top + rect.Height * 0.18f, rect.Right - rect.Width * 0.20f, rect.Bottom - rect.Height * 0.18f);
    }

    private static void DrawRange(Graphics g, RectangleF rect, Pen pen, Brush fill)
    {
        var y = rect.Top + rect.Height * 0.72f;
        g.DrawArc(pen, rect.Left + rect.Width * 0.18f, rect.Top + rect.Height * 0.18f, rect.Width * 0.64f, rect.Height * 0.64f, 200, 120);
        g.DrawLine(pen, rect.Left + rect.Width * 0.20f, y, rect.Right - rect.Width * 0.16f, y);
        using var path = new GraphicsPath();
        path.AddPolygon(
        [
            new PointF(rect.Right - rect.Width * 0.18f, y),
            new PointF(rect.Right - rect.Width * 0.34f, y - rect.Height * 0.10f),
            new PointF(rect.Right - rect.Width * 0.34f, y + rect.Height * 0.10f)
        ]);
        g.FillPath(fill, path);
    }

    private static void DrawThreat(Graphics g, RectangleF rect, Pen pen)
    {
        g.DrawArc(pen, rect.Left + rect.Width * 0.12f, rect.Top + rect.Height * 0.18f, rect.Width * 0.76f, rect.Height * 0.66f, 15, 150);
        g.DrawArc(pen, rect.Left + rect.Width * 0.12f, rect.Top + rect.Height * 0.18f, rect.Width * 0.76f, rect.Height * 0.66f, 195, 150);
        g.DrawLine(pen, rect.Left + rect.Width * 0.30f, rect.Top + rect.Height * 0.52f, rect.Left + rect.Width * 0.44f, rect.Top + rect.Height * 0.60f);
        g.DrawLine(pen, rect.Right - rect.Width * 0.30f, rect.Top + rect.Height * 0.52f, rect.Right - rect.Width * 0.44f, rect.Top + rect.Height * 0.60f);
    }

    private static Color Blend(Color a, Color b, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        float inverse = 1f - amount;
        return Color.FromArgb(
            (int)Math.Round(a.A * inverse + b.A * amount),
            (int)Math.Round(a.R * inverse + b.R * amount),
            (int)Math.Round(a.G * inverse + b.G * amount),
            (int)Math.Round(a.B * inverse + b.B * amount));
    }
}

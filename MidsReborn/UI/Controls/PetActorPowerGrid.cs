using System.Drawing.Drawing2D;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Renderer;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

public sealed class PetActorPowerGrid : Control
{
    private sealed record LayoutTile(Rectangle Bounds, Rectangle? ToggleBounds, PetActorPowerTileViewModel Tile);
    private sealed record LayoutSection(
        string Title,
        string? Subtitle,
        Rectangle HeaderBounds,
        Rectangle? SubtitleBounds,
        IReadOnlyList<LayoutTile> Tiles);

    private readonly List<LayoutSection> _sections = [];
    private PetActorPowerGridViewModel? _viewModel;

    public event EventHandler<PetActorPowerTileViewModel>? PowerSelected;
    public event EventHandler<PetActorPowerTileViewModel>? PreviewToggleClicked;

    public PetActorPowerGrid()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        BackColor = Color.Black;
        Dock = DockStyle.Top;
    }

    public void SetViewModel(PetActorPowerGridViewModel? viewModel)
    {
        _viewModel = viewModel;
        RebuildLayout();
        Invalidate();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        RebuildLayout();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var dataTheme = CurrentDataViewTheme;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(dataTheme.Background);

        if (_viewModel == null || !_viewModel.HasPowers)
        {
            DrawEmptyState(e.Graphics, dataTheme);
            return;
        }

        using var sectionFont = new Font("Segoe UI", 10f, FontStyle.Bold);
        using var subtitleFont = new Font("Segoe UI", 8.5f, FontStyle.Regular);

        foreach (var section in _sections)
        {
            TextRenderer.DrawText(
                e.Graphics,
                section.Title,
                sectionFont,
                section.HeaderBounds,
                dataTheme.ValueText,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if (section.SubtitleBounds is Rectangle subtitleBounds && !string.IsNullOrWhiteSpace(section.Subtitle))
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    section.Subtitle,
                    subtitleFont,
                    subtitleBounds,
                    dataTheme.Muted,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            foreach (var tile in section.Tiles)
            {
                DrawTile(e.Graphics, tile.Bounds, tile.Tile, CurrentPowerSlotTheme, dataTheme);
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        foreach (var tile in _sections.SelectMany(section => section.Tiles))
        {
            if (!tile.Bounds.Contains(e.Location))
            {
                continue;
            }

            if (tile.ToggleBounds is Rectangle toggleBounds && toggleBounds.Contains(e.Location))
            {
                PreviewToggleClicked?.Invoke(this, tile.Tile);
                break;
            }

            PowerSelected?.Invoke(this, tile.Tile);
            break;
        }
    }

    private void RebuildLayout()
    {
        _sections.Clear();

        if (_viewModel == null || !_viewModel.HasPowers || ClientSize.Width <= 0)
        {
            Height = Math.Max(64, ClientSize.Height);
            NotifyScrollParent();
            return;
        }

        const int outerPadding = 8;
        const int sectionGap = 14;
        const int rowGap = 12;
        const int headerHeight = 22;
        const int subtitleHeight = 16;
        const int tileHeight = 42;
        const int sectionMinWidth = 250;

        var populatedSections = _viewModel.Sections.Where(section => section.Tiles.Count > 0).ToArray();
        if (populatedSections.Length == 0)
        {
            Height = Math.Max(64, ClientSize.Height);
            NotifyScrollParent();
            return;
        }

        var usableWidth = Math.Max(sectionMinWidth, ClientSize.Width - (outerPadding * 2));
        var maxColumns = Math.Max(1, (usableWidth + sectionGap) / (sectionMinWidth + sectionGap));
        var columns = Math.Min(populatedSections.Length, maxColumns);
        var sectionWidth = Math.Max(sectionMinWidth, (usableWidth - ((columns - 1) * sectionGap)) / columns);

        var x = outerPadding;
        var y = outerPadding;
        var currentColumn = 0;
        var currentRowHeight = 0;

        foreach (var section in populatedSections)
        {
            var sectionHeight = headerHeight + 4;
            Rectangle? subtitleBounds = null;
            if (!string.IsNullOrWhiteSpace(section.Subtitle))
            {
                subtitleBounds = new Rectangle(x, y + headerHeight, sectionWidth, subtitleHeight);
                sectionHeight += subtitleHeight + 4;
            }

            var headerBounds = new Rectangle(x, y, sectionWidth, headerHeight);
            var tileY = y + sectionHeight;
            var tiles = new List<LayoutTile>(section.Tiles.Count);
            foreach (var tile in section.Tiles)
            {
                var tileBounds = new Rectangle(x, tileY, sectionWidth, tileHeight);
                tiles.Add(new LayoutTile(tileBounds, GetPreviewToggleBounds(tileBounds, tile), tile));
                tileY += tileHeight + rowGap;
            }

            sectionHeight = tileY - y;
            if (tiles.Count > 0)
            {
                sectionHeight -= rowGap;
            }

            _sections.Add(new LayoutSection(section.Title, section.Subtitle, headerBounds, subtitleBounds, tiles));
            currentRowHeight = Math.Max(currentRowHeight, sectionHeight);

            currentColumn++;
            if (currentColumn >= columns)
            {
                currentColumn = 0;
                x = outerPadding;
                y += currentRowHeight + sectionGap;
                currentRowHeight = 0;
            }
            else
            {
                x += sectionWidth + sectionGap;
            }
        }

        if (currentColumn > 0)
        {
            y += currentRowHeight + outerPadding;
        }
        else
        {
            y += outerPadding;
        }

        Height = Math.Max(120, y);
        NotifyScrollParent();
    }

    private static void DrawTile(Graphics g, Rectangle bounds, PetActorPowerTileViewModel tile, PowerSlotTheme slotTheme, DataViewTheme dataTheme)
    {
        var state = ResolveState(tile);
        DrawPowerSlot(g, bounds, state, slotTheme);

        var badgeWidth = 0;
        if (!string.IsNullOrWhiteSpace(tile.PowerKindLabel))
        {
            badgeWidth = DrawKindBadge(g, bounds, tile.PowerKindLabel, dataTheme);
        }

        var toggleWidth = 0;
        if (tile.CanTogglePreview)
        {
            var toggleBounds = GetPreviewToggleBounds(bounds, tile);
            if (toggleBounds is Rectangle statToggleBounds)
            {
                DrawPreviewToggle(g, statToggleBounds, tile.IsPreviewIncluded);
                toggleWidth = statToggleBounds.Width + 12;
            }
        }

        var isBold = MidsContext.Config?.RtFont.PowersBold ?? true;
        var textScaleFactor = SystemFonts.MessageBoxFont.SizeInPoints / 9f;
        var petGridFontSize = 12.5f * textScaleFactor;
        using var titleFont = new Font("Segoe UI", petGridFontSize, isBold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
        using var textBrush = !MidsContext.Character.IsHero() ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);

        var textBounds = new RectangleF
        {
            X = bounds.X + 10,
            Y = bounds.Y + 4,
            Width = bounds.Width - 20 - badgeWidth - toggleWidth,
            Height = bounds.Height - 8
        };

        if (MidsContext.Config?.EnhanceVisibility ?? true)
        {
            BuildRenderer.DrawOutlineText(tile.DisplayName, textBounds, Color.WhiteSmoke, Color.Black, titleFont, 3f, g, false, true);
        }
        else
        {
            using var stringFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(tile.DisplayName, titleFont, textBrush, textBounds, stringFormat);
        }
    }

    private static Enums.ePowerState ResolveState(PetActorPowerTileViewModel tile)
    {
        if (tile.IsSelected)
        {
            return Enums.ePowerState.Open;
        }

        return Enums.ePowerState.Used;
    }

    private static int DrawKindBadge(Graphics g, Rectangle bounds, string kindLabel, DataViewTheme dataTheme)
    {
        var badgeText = kindLabel.ToUpperInvariant();
        using var badgeFont = new Font("Segoe UI", 7.25f, FontStyle.Bold);
        var textSize = TextRenderer.MeasureText(g, badgeText, badgeFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
        var badgeHeight = 18;
        var badgeWidth = Math.Max(44, textSize.Width + 14);
        var badgeBounds = new Rectangle(
            bounds.Right - badgeWidth - 10,
            bounds.Y + (bounds.Height - badgeHeight) / 2,
            badgeWidth,
            badgeHeight);

        using var badgePath = CreateRoundedRect(badgeBounds, badgeHeight / 2);
        using var badgeBrush = new SolidBrush(Color.FromArgb(165, 18, 24, 31));
        using var badgePen = new Pen(Color.FromArgb(140, dataTheme.GridHeaderBorder));
        g.FillPath(badgeBrush, badgePath);
        g.DrawPath(badgePen, badgePath);

        TextRenderer.DrawText(
            g,
            badgeText,
            badgeFont,
            badgeBounds,
            dataTheme.ValueText,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        return badgeWidth + 12;
    }

    private static Rectangle? GetPreviewToggleBounds(Rectangle bounds, PetActorPowerTileViewModel tile)
    {
        if (!tile.CanTogglePreview)
        {
            return null;
        }

        const int toggleSize = 17;
        const int padding = 8;
        var y = bounds.Top + (bounds.Height - toggleSize) / 2;
        var x = bounds.Right - toggleSize - padding - (toggleSize / 2);
        return new Rectangle(x, y, toggleSize, toggleSize);
    }

    private static void DrawPreviewToggle(Graphics g, Rectangle toggleBounds, bool included)
    {
        var centerOffset = new PointF(-0.25f, -0.33f);
        using var gradientBrush = CreateToggleBrush(
            toggleBounds,
            centerOffset,
            included ? Color.FromArgb(96, 255, 96) : Color.FromArgb(96, 96, 96),
            included ? Color.FromArgb(0, 32, 0) : Color.FromArgb(0, 0, 0));
        using var borderPen = new Pen(Color.FromArgb(120, 0, 0, 0));
        g.FillEllipse(gradientBrush, toggleBounds);
        g.DrawEllipse(borderPen, toggleBounds);
    }

    private static PathGradientBrush CreateToggleBrush(Rectangle bounds, PointF centerOffset, Color centerColor, Color outerColor)
    {
        var centerX = bounds.Left + bounds.Width * 0.5f;
        var centerY = bounds.Top + bounds.Height * 0.5f;
        var graphicsPath = new GraphicsPath();
        graphicsPath.AddEllipse(bounds);

        var colors = new Color[graphicsPath.PathPoints.Length];
        Array.Fill(colors, outerColor);

        var brush = new PathGradientBrush(graphicsPath)
        {
            CenterColor = centerColor,
            SurroundColors = colors,
            CenterPoint = new PointF(
                centerX + (centerOffset.X + centerOffset.X * (bounds.Width * 0.5f)),
                centerY + (centerOffset.Y + centerOffset.Y * (bounds.Height * 0.5f)))
        };

        return brush;
    }

    private static void DrawPowerSlot(Graphics g, Rectangle bounds, Enums.ePowerState state, PowerSlotTheme theme)
    {
        using var path = new GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, bounds.Height, bounds.Height, 90, 180);
        path.AddArc(bounds.Right - bounds.Height, bounds.Y, bounds.Height, bounds.Height, 270, 180);
        path.CloseFigure();

        switch (state)
        {
            case Enums.ePowerState.Disabled:
                using (var fillBrush = new SolidBrush(theme.DisabledFill))
                using (var borderPen = new Pen(Color.FromArgb(60, 60, 60)))
                {
                    g.FillPath(fillBrush, path);
                    g.DrawPath(borderPen, path);
                }
                break;

            case Enums.ePowerState.Empty:
                using (var fillBrush = new SolidBrush(theme.EmptyFill))
                using (var borderPen = new Pen(theme.Border))
                {
                    g.FillPath(fillBrush, path);
                    g.DrawPath(borderPen, path);
                }
                break;

            case Enums.ePowerState.Open:
                using (var borderBrush = new SolidBrush(theme.OpenBorder))
                {
                    g.FillPath(borderBrush, path);
                }

                var innerBounds = bounds;
                innerBounds.Inflate(-3, -3);
                using (var innerPath = new GraphicsPath())
                {
                    innerPath.AddArc(innerBounds.X, innerBounds.Y, innerBounds.Height, innerBounds.Height, 90, 180);
                    innerPath.AddArc(innerBounds.Right - innerBounds.Height, innerBounds.Y, innerBounds.Height, innerBounds.Height, 270, 180);
                    innerPath.CloseFigure();
                    using var fillBrush = new LinearGradientBrush(bounds, theme.GradientTop, theme.GradientBottom, 90f);
                    g.FillPath(fillBrush, innerPath);
                }
                break;

            case Enums.ePowerState.Used:
            default:
                using (var fillBrush = new LinearGradientBrush(bounds, theme.GradientTop, theme.GradientBottom, 90f))
                using (var borderPen = new Pen(theme.Border, 1.5f))
                {
                    g.FillPath(fillBrush, path);
                    g.DrawPath(borderPen, path);
                }
                break;
        }
    }

    private void DrawEmptyState(Graphics g, DataViewTheme theme)
    {
        using var font = new Font("Segoe UI", 10f, FontStyle.Bold);
        TextRenderer.DrawText(
            g,
            "No powers are available.",
            font,
            ClientRectangle,
            theme.Muted,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
    }

    private void NotifyScrollParent()
    {
        if (Parent?.Parent is MidsVScrollPanel scrollPanel)
        {
            scrollPanel.RecalculateLayout();
        }
    }

    private static GraphicsPath CreateRoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private DataViewTheme CurrentDataViewTheme => DesignMode
        ? ThemeManager.DesignTime.DataView
        : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

    private PowerSlotTheme CurrentPowerSlotTheme => DesignMode
        ? ThemeManager.DesignTime.PowerSlot
        : ThemeManager.CurrentTheme?.PowerSlot ?? ThemeManager.DesignTime.PowerSlot;
}

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Renderer;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

public sealed class PetActorPowerGrid : Control
{
    private const float MinimumResponsiveUiScale = 0.90f;
    private const float IconWellContentFill = 0.92f;
    private const byte IconVisibleAlphaThreshold = 8;
    private readonly record struct PowerSlotPalette(
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
    private sealed record LayoutTile(Rectangle Bounds, Rectangle? ToggleBounds, PetActorPowerTileViewModel Tile);
    private sealed record LayoutSection(
        string Title,
        string? Subtitle,
        Rectangle HeaderBounds,
        Rectangle? SubtitleBounds,
        IReadOnlyList<LayoutTile> Tiles);

    private readonly List<LayoutSection> _sections = [];
    private readonly Dictionary<Bitmap, Rectangle> _iconVisibleBoundsCache = [];
    private PetActorPowerGridViewModel? _viewModel;
    private float _uiScale = 1f;
    private float EffectiveScale => _uiScale * (DeviceDpi / 96f);

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

    public float UiScale
    {
        get => _uiScale;
        set
        {
            var clamped = Math.Clamp(value, MinimumResponsiveUiScale, 1.25f);
            if (Math.Abs(_uiScale - clamped) < 0.01f)
            {
                return;
            }

            _uiScale = clamped;
            RebuildLayout();
            Invalidate();
        }
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
        ConfigureGraphics(e.Graphics);
        e.Graphics.Clear(dataTheme.Background);

        var effectiveViewModel = GetEffectiveViewModel();
        if (effectiveViewModel == null || !effectiveViewModel.HasPowers)
        {
            DrawEmptyState(e.Graphics, dataTheme);
            return;
        }

        using var sectionFont = CreateScaledFont(10f, FontStyle.Bold);
        using var subtitleFont = CreateScaledFont(8.5f, FontStyle.Regular);
        var sectionTitleColor = Color.FromArgb(129, 243, 48);

        foreach (var section in _sections)
        {
            TextRenderer.DrawText(
                e.Graphics,
                section.Title,
                sectionFont,
                section.HeaderBounds,
                sectionTitleColor,
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
        var effectiveViewModel = GetEffectiveViewModel();

        if (effectiveViewModel == null || !effectiveViewModel.HasPowers || ClientSize.Width <= 0)
        {
            Height = Math.Max(ScalePx(64), ClientSize.Height);
            NotifyScrollParent();
            return;
        }

        var outerPadding = ScalePx(8);
        var sectionGap = ScalePx(14);
        var rowGap = ScaleLogical(18);
        var headerHeight = ScaleLogical(22);
        var subtitleHeight = ScaleLogical(18);
        var tileHeight = ScaleLogical(30);
        var sectionMinWidth = ScalePx(280);
        var sectionPreferredWidth = ScalePx(420);
        var sectionMaxWidth = ScalePx(760);

        var populatedSections = effectiveViewModel.Sections.Where(section => section.Tiles.Count > 0).ToArray();
        if (populatedSections.Length == 0)
        {
            Height = Math.Max(ScalePx(64), ClientSize.Height);
            NotifyScrollParent();
            return;
        }

        var usableWidth = Math.Max(sectionMinWidth, ClientSize.Width - (outerPadding * 2));
        var maxColumns = Math.Max(1, (usableWidth + sectionGap) / (sectionPreferredWidth + sectionGap));
        var columns = Math.Min(populatedSections.Length, maxColumns);
        var y = outerPadding;
        for (var rowStart = 0; rowStart < populatedSections.Length; rowStart += columns)
        {
            var sectionsInRow = Math.Min(columns, populatedSections.Length - rowStart);
            var sectionWidth = sectionsInRow == 1
                ? Math.Min(sectionMaxWidth, Math.Max(sectionMinWidth, usableWidth))
                : Math.Min(sectionMaxWidth, Math.Max(sectionMinWidth, (usableWidth - ((sectionsInRow - 1) * sectionGap)) / sectionsInRow));
            var rowWidth = sectionsInRow * sectionWidth + ((sectionsInRow - 1) * sectionGap);
            var x = outerPadding + Math.Max(0, (usableWidth - rowWidth) / 2);
            var rowHeight = 0;

            for (var rowIndex = 0; rowIndex < sectionsInRow; rowIndex++)
            {
                var section = populatedSections[rowStart + rowIndex];
                var sectionHeight = headerHeight + ScalePx(4);
                Rectangle? subtitleBounds = null;
                if (!string.IsNullOrWhiteSpace(section.Subtitle))
                {
                    subtitleBounds = new Rectangle(x, y + headerHeight, sectionWidth, subtitleHeight);
                    sectionHeight += subtitleHeight + ScalePx(4);
                }

                var headerBounds = new Rectangle(x, y, sectionWidth, headerHeight);
                var tileY = y + sectionHeight;
                var tiles = new List<LayoutTile>(section.Tiles.Count);
                foreach (var tile in section.Tiles)
                {
                    var tileBounds = new Rectangle(x, tileY, sectionWidth, tileHeight);
                    tiles.Add(new LayoutTile(tileBounds, tile.CanTogglePreview ? GetIndicatorBounds(tileBounds, _uiScale) : null, tile));
                    tileY += tileHeight + rowGap;
                }

                sectionHeight = tileY - y;
                if (tiles.Count > 0)
                {
                    sectionHeight -= rowGap;
                }

                _sections.Add(new LayoutSection(section.Title, section.Subtitle, headerBounds, subtitleBounds, tiles));
                rowHeight = Math.Max(rowHeight, sectionHeight);
                x += sectionWidth + sectionGap;
            }

            y += rowHeight + sectionGap;
        }

        Height = Math.Max(ScalePx(120), y + outerPadding - sectionGap);
        NotifyScrollParent();
    }

    private PetActorPowerGridViewModel? GetEffectiveViewModel()
    {
        if (_viewModel != null && _viewModel.HasPowers)
        {
            return _viewModel;
        }

        return IsInDesignMode ? CreateDesignTimeViewModel() : null;
    }

    private static PetActorPowerGridViewModel CreateDesignTimeViewModel()
    {
        return new PetActorPowerGridViewModel
        {
            SelectedPowerIndex = 2,
            Sections =
            [
                new PetActorPowerSectionViewModel
                {
                    Title = "Beast Alpha Wolf",
                    Tiles =
                    [
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 0,
                            DisplayName = "Resistance",
                            FullName = "Sample.Resistance",
                            PowerKindLabel = "Auto",
                            IsAuto = true,
                            BasePower = null!
                        },
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 1,
                            DisplayName = "Super Leap",
                            FullName = "Sample.SuperLeap",
                            PowerKindLabel = "Toggle",
                            BasePower = null!
                        },
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 2,
                            DisplayName = "Vicious Bite",
                            FullName = "Sample.ViciousBite",
                            IsSelected = true,
                            BasePower = null!
                        },
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 3,
                            DisplayName = "Wild Charge",
                            FullName = "Sample.WildCharge",
                            BasePower = null!
                        }
                    ]
                },
                new PetActorPowerSectionViewModel
                {
                    Title = "Beast Alpha Wolf 2",
                    Subtitle = "Unlocked by Train Beasts",
                    Tiles =
                    [
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 4,
                            DisplayName = "Howl",
                            FullName = "Sample.Howl",
                            BasePower = null!
                        },
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 5,
                            DisplayName = "Maiming Bite",
                            FullName = "Sample.MaimingBite",
                            BasePower = null!
                        }
                    ]
                }
            ]
        };
    }

    private void DrawTile(Graphics g, Rectangle bounds, PetActorPowerTileViewModel tile, PowerSlotTheme slotTheme, DataViewTheme dataTheme)
    {
        var state = ResolveState(tile);
        var palette = GetPowerSlotPalette(state, slotTheme);
        var iconWellBounds = GetIconWellRect(bounds);
        DrawVectorPowerSlot(g, bounds, state, slotTheme, tile.IsSelected);
        DrawPowerIcon(g, iconWellBounds, tile, palette);
        DrawIconWellOverlay(g, iconWellBounds, palette);

        var indicatorWidth = 0;
        var showIndicator = CanShowIndicator(tile);
        if (showIndicator)
        {
            var statIndicatorBounds = GetIndicatorBounds(bounds, _uiScale);
            DrawStateIndicator(g, statIndicatorBounds, ShouldHighlightIndicator(tile));
            indicatorWidth = statIndicatorBounds.Width + ScalePx(12);
        }

        var isBold = MidsContext.Config?.RtFont.PowersBold ?? true;
        var textScaleFactor = SystemFonts.MessageBoxFont.SizeInPoints / 9f;
        var petGridFontSize = 11.25f * textScaleFactor * _uiScale;
        using var titleFont = new Font("Segoe UI", petGridFontSize, isBold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
        var textColor = ResolveColor(CurrentPowerSlotTheme.ForeColor, Color.WhiteSmoke);
        if (state is Enums.ePowerState.Empty or Enums.ePowerState.Open)
        {
            textColor = Blend(textColor, Color.White, 0.12f);
        }

        var textBounds = new RectangleF
        {
            X = bounds.X + Math.Max(ScaleLogical(12), iconWellBounds.Right - bounds.X + ScaleLogical(1)),
            Y = bounds.Y,
            Width = Math.Max(1, bounds.Width - Math.Max(ScaleLogical(12), iconWellBounds.Right - bounds.X + ScaleLogical(1)) - Math.Max(ScaleLogical(12), indicatorWidth + ScaleLogical(6))),
            Height = bounds.Height - ScaleLogical(3)
        };

        var outlineThickness = MidsContext.Config?.EnhanceVisibility ?? true
            ? Math.Max(2.4f, EffectiveScale * 2.6f)
            : Math.Max(1.9f, EffectiveScale * 2.1f);
        DrawPowerLabelText(g, tile.DisplayName, textBounds, textColor, titleFont, outlineThickness);
    }

    private static Enums.ePowerState ResolveState(PetActorPowerTileViewModel tile)
    {
        return Enums.ePowerState.Used;
    }

    private static void ConfigureGraphics(Graphics g)
    {
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        g.SmoothingMode = SmoothingMode.HighQuality;
    }

    private static Rectangle GetIndicatorBounds(Rectangle bounds, float uiScale)
    {
        var toggleSize = ScalePx(19, uiScale);
        var padding = ScalePx(10, uiScale);
        var y = bounds.Top + (bounds.Height - toggleSize) / 2;
        var x = bounds.Right - toggleSize - padding;
        return new Rectangle(x, y, toggleSize, toggleSize);
    }

    private static void DrawStateIndicator(Graphics g, Rectangle toggleBounds, bool included)
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

    private void DrawPowerIcon(Graphics g, Rectangle iconWellRect, PetActorPowerTileViewModel tile, PowerSlotPalette palette)
    {
        var image = ResolvePowerImage(tile.BasePower);
        if (image != null)
        {
            var socketRect = GetIconSocketRect(iconWellRect);
            var drawBounds = GetSocketIconDestinationRect(image, socketRect);
            using var clipPath = new GraphicsPath();
            clipPath.AddEllipse(socketRect);
            var state = g.Save();
            g.SetClip(clipPath);
            g.DrawImage(image, drawBounds);
            g.Restore(state);
            return;
        }

        var initials = string.IsNullOrWhiteSpace(tile.DisplayName)
            ? "P"
            : new string(tile.DisplayName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(part => char.ToUpperInvariant(part[0]))
                .ToArray());
        using var font = new Font("Segoe UI", Math.Max(7f, 8.5f * _uiScale), FontStyle.Bold, GraphicsUnit.Pixel);
        TextRenderer.DrawText(
            g,
            initials,
            font,
            GetIconSocketRect(iconWellRect),
            CurrentDataViewTheme.ValueText,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    private static Image? ResolvePowerImage(IPower? power)
    {
        if (power == null)
        {
            return null;
        }

        var icon = AssetManager.GetPowerImage(power);
        if (icon != AssetManager.UnknownIcon && icon?.Bitmap != null)
        {
            return icon.Bitmap;
        }

        var powersetIcon = AssetManager.GetPowersetImage(power);
        return powersetIcon == AssetManager.UnknownIcon
            ? null
            : powersetIcon.Bitmap;
    }

    private static bool ShouldHighlightIndicator(PetActorPowerTileViewModel tile)
    {
        if (tile.CanTogglePreview)
        {
            return tile.IsPreviewIncluded;
        }

        return tile.IsAuto || string.Equals(tile.PowerKindLabel, "Toggle", StringComparison.OrdinalIgnoreCase);
    }

    private static bool CanShowIndicator(PetActorPowerTileViewModel tile)
    {
        return tile.CanTogglePreview ||
               tile.IsAuto ||
               string.Equals(tile.PowerKindLabel, "Toggle", StringComparison.OrdinalIgnoreCase);
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

    private void DrawPowerLabelText(Graphics g, string text, RectangleF bounds, Color fillColor, Font baseFont, float outlineThickness)
    {
        if (string.IsNullOrWhiteSpace(text) || bounds.Width <= 0f || bounds.Height <= 0f)
        {
            return;
        }

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        using var powerFont = CreatePowerLabelFont(baseFont);
        using var format = new StringFormat(StringFormatFlags.NoWrap)
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        var emSize = powerFont.SizeInPoints * g.DpiY / 72f;
        using var textPath = new GraphicsPath();
        textPath.AddString(text, powerFont.FontFamily, (int)powerFont.Style, emSize, bounds, format);
        ApplyPowerLabelShear(textPath, bounds);

        var actualFillColor = Blend(fillColor, Color.White, 0.18f);

        using (var shadowPath = (GraphicsPath)textPath.Clone())
        using (var shadowMatrix = new Matrix())
        using (var shadowBrush = new SolidBrush(Color.FromArgb(182, 0, 0, 0)))
        {
            shadowMatrix.Translate(Math.Max(1f, EffectiveScale * 1.05f), Math.Max(1f, EffectiveScale));
            shadowPath.Transform(shadowMatrix);
            g.FillPath(shadowBrush, shadowPath);
        }

        using (var outlinePen = new Pen(Color.FromArgb(240, 0, 0, 0), outlineThickness + Math.Max(0.2f, EffectiveScale * 0.15f)))
        {
            outlinePen.LineJoin = LineJoin.Round;
            g.DrawPath(outlinePen, textPath);
        }

        using (var fillBrush = new SolidBrush(actualFillColor))
        {
            g.FillPath(fillBrush, textPath);
        }

        var highlightState = g.Save();
        g.SetClip(new RectangleF(
            bounds.X,
            bounds.Y,
            bounds.Width,
            Math.Max(2f, bounds.Height * 0.44f)));
        using (var highlightBrush = new SolidBrush(Color.FromArgb(56, 255, 255, 255)))
        {
            g.FillPath(highlightBrush, textPath);
        }
        g.Restore(highlightState);
    }

    private void ApplyPowerLabelShear(GraphicsPath textPath, RectangleF bounds)
    {
        if (textPath.PointCount == 0)
        {
            return;
        }

        using var shearMatrix = new Matrix();
        const float shear = -0.18f;
        var xCompensation = Math.Max(0.5f, bounds.Height * 0.10f);
        shearMatrix.Translate(-bounds.X, -bounds.Y, MatrixOrder.Append);
        shearMatrix.Shear(shear, 0f, MatrixOrder.Append);
        shearMatrix.Translate(bounds.X + xCompensation, bounds.Y, MatrixOrder.Append);
        textPath.Transform(shearMatrix);
    }

    private Font CreatePowerLabelFont(Font baseFont)
    {
        var size = baseFont.Size + Math.Max(0.15f, EffectiveScale * 0.20f);
        const FontStyle style = FontStyle.Bold;

        foreach (var familyName in new[] { "Arial Black", "Arial", "Tahoma", baseFont.FontFamily.Name })
        {
            try
            {
                return new Font(familyName, size, style, GraphicsUnit.Pixel);
            }
            catch
            {
                // try next family
            }
        }

        return new Font(baseFont.FontFamily, size, style, GraphicsUnit.Pixel);
    }

    private void DrawVectorPowerSlot(Graphics g, Rectangle bounds, Enums.ePowerState state, PowerSlotTheme theme, bool emphasized)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        g.SmoothingMode = SmoothingMode.AntiAlias;

        var palette = GetPowerSlotPalette(state, theme);
        var iconWellRect = GetIconWellRect(bounds);
        var bodyRect = GetPowerBodyRect(bounds, iconWellRect);
        var mergedBounds = Rectangle.Union(bodyRect, iconWellRect);
        var rimBounds = DeflateRect(bodyRect, 1);
        var rimWellRect = DeflateRect(iconWellRect, 1);
        var mergedRimBounds = Rectangle.Union(rimBounds, rimWellRect);
        var faceBounds = DeflateRect(bodyRect, Math.Max(1, ScaleLogical(1)));
        var glossBounds = new Rectangle(faceBounds.X, faceBounds.Y, faceBounds.Width, Math.Max(2, faceBounds.Height / 2 + 1));
        var shadowBounds = new Rectangle(faceBounds.X, faceBounds.Y + faceBounds.Height / 2, faceBounds.Width, Math.Max(2, faceBounds.Height / 2 + 1));
        using var outerPath = CreateMergedPowerSlotPath(bodyRect, iconWellRect);
        using var outerBrush = new SolidBrush(palette.OuterStroke);
        g.FillPath(outerBrush, outerPath);

        if (rimBounds.Width <= 0 || rimBounds.Height <= 0 || rimWellRect.Width <= 0 || rimWellRect.Height <= 0)
        {
            return;
        }

        using var rimPath = CreateMergedPowerSlotPath(rimBounds, rimWellRect);
        using var rimBrush = CreateThreeStopBrush(mergedRimBounds, palette.RimTop, Blend(palette.RimTop, palette.RimBottom, 0.45f), palette.RimBottom);
        g.FillPath(rimBrush, rimPath);

        if (faceBounds.Width <= 0 || faceBounds.Height <= 0)
        {
            return;
        }

        using var facePath = CreateCapsulePath(faceBounds);
        using var faceBrush = CreateThreeStopBrush(faceBounds, palette.FillTop, palette.FillMid, palette.FillBottom);
        g.FillPath(faceBrush, facePath);

        var glossState = g.Save();
        g.SetClip(new Rectangle(faceBounds.X, faceBounds.Y, faceBounds.Width, Math.Max(2, faceBounds.Height / 2 + 1)), CombineMode.Intersect);
        using (var glossBrush = CreateThreeStopBrush(
                   glossBounds,
                   palette.GlossTop,
                   Blend(palette.GlossTop, palette.GlossBottom, 0.55f),
                   palette.GlossBottom))
        {
            g.FillPath(glossBrush, facePath);
        }
        g.Restore(glossState);

        var shadowState = g.Save();
        g.SetClip(new Rectangle(faceBounds.X, faceBounds.Y + faceBounds.Height / 2, faceBounds.Width, Math.Max(2, faceBounds.Height / 2 + 1)), CombineMode.Intersect);
        using (var shadowBrush = new LinearGradientBrush(
                   shadowBounds,
                   Color.FromArgb(0, palette.InnerShadow),
                   palette.InnerShadow,
                   90f))
        {
            g.FillPath(shadowBrush, facePath);
        }
        g.Restore(shadowState);

        var highlightState = g.Save();
        g.SetClip(new Rectangle(mergedRimBounds.X, mergedRimBounds.Y, mergedRimBounds.Width, Math.Max(2, mergedRimBounds.Height / 2)), CombineMode.Intersect);
        using (var highlightPen = new Pen(palette.HighlightStroke, Math.Max(1.1f, EffectiveScale)))
        {
            highlightPen.LineJoin = LineJoin.Round;
            g.DrawPath(highlightPen, rimPath);
        }
        g.Restore(highlightState);

        using var faceOutlinePen = new Pen(Color.FromArgb(92, palette.HighlightStroke));
        using var outerOutlinePen = new Pen(Color.FromArgb(124, palette.OuterStroke));
        g.DrawPath(faceOutlinePen, facePath);
        g.DrawPath(outerOutlinePen, outerPath);

        if (emphasized)
        {
            var selectionAccent = GetPowerSelectionAccentColor(CurrentApplicationTheme, theme);
            using var selectionGlowPen = new Pen(Color.FromArgb(82, selectionAccent), Math.Max(1.8f, EffectiveScale * 1.8f));
            using var selectionStrokePen = new Pen(Color.FromArgb(184, Blend(selectionAccent, Color.White, 0.18f)), Math.Max(1.0f, EffectiveScale * 1.05f));
            selectionGlowPen.LineJoin = LineJoin.Round;
            selectionStrokePen.LineJoin = LineJoin.Round;
            g.DrawPath(selectionGlowPen, outerPath);
            g.DrawPath(selectionStrokePen, rimPath);
        }

        var topLineBounds = DeflateRect(mergedRimBounds, Math.Max(1, ScaleLogical(1)));
        if (topLineBounds.Width > 0 && topLineBounds.Height > 0)
        {
            var lineState = g.Save();
            g.SetClip(new Rectangle(topLineBounds.X, topLineBounds.Y, topLineBounds.Width, Math.Max(2, topLineBounds.Height / 3)), CombineMode.Intersect);
            using var topLinePen = new Pen(Color.FromArgb(140, Blend(palette.HighlightStroke, Color.White, 0.25f)), Math.Max(1f, EffectiveScale));
            g.DrawPath(topLinePen, rimPath);
            g.Restore(lineState);
        }

        DrawIconWell(g, iconWellRect, palette);
    }

    private PowerSlotPalette GetPowerSlotPalette(Enums.ePowerState state, PowerSlotTheme theme)
    {
        var gradientTop = ResolveColor(theme.GradientTop, Color.FromArgb(84, 140, 220));
        var gradientBottom = ResolveColor(theme.GradientBottom, Color.FromArgb(18, 72, 138));
        var border = ResolveColor(theme.Border, gradientBottom);
        var openBorder = ResolveColor(theme.OpenBorder, Blend(border, Color.White, 0.35f));
        var emptyFill = ResolveColor(theme.EmptyFill, Color.FromArgb(54, 58, 68));
        var disabledFill = ResolveColor(theme.DisabledFill, Color.FromArgb(36, 38, 44));

        return state switch
        {
            Enums.ePowerState.Disabled => CreatePowerSlotPalette(
                Blend(disabledFill, border, 0.10f),
                Blend(disabledFill, Color.White, 0.08f),
                Blend(disabledFill, Color.Black, 0.40f),
                Blend(disabledFill, Color.White, 0.10f)),
            Enums.ePowerState.Empty => CreatePowerSlotPalette(
                Blend(emptyFill, border, 0.24f),
                Blend(emptyFill, gradientTop, 0.18f),
                Blend(emptyFill, Color.Black, 0.28f),
                Blend(emptyFill, Color.White, 0.16f)),
            Enums.ePowerState.Open => CreatePowerSlotPalette(
                border,
                Blend(gradientTop, Color.White, 0.10f),
                Blend(gradientBottom, Color.Black, 0.05f),
                Blend(openBorder, gradientTop, 0.28f)),
            Enums.ePowerState.Used => CreatePowerSlotPalette(
                Blend(border, Color.Black, 0.08f),
                Blend(gradientTop, Color.White, 0.06f),
                Blend(gradientBottom, Color.Black, 0.09f),
                Blend(openBorder, gradientTop, 0.22f)),
            _ => CreatePowerSlotPalette(
                Blend(emptyFill, border, 0.24f),
                Blend(emptyFill, gradientTop, 0.18f),
                Blend(emptyFill, Color.Black, 0.28f),
                Blend(emptyFill, Color.White, 0.16f))
        };
    }

    private Color GetPowerSelectionAccentColor(ApplicationTheme applicationTheme, PowerSlotTheme slotTheme)
    {
        var menuAccent = applicationTheme.MenuStrip.AccentColor;
        var hoverBorder = applicationTheme.DropDownList.HoverBorder;
        var openBorder = slotTheme.OpenBorder;

        return ResolveColor(menuAccent,
            ResolveColor(hoverBorder,
                ResolveColor(openBorder, Color.Gold)));
    }

    private static PowerSlotPalette CreatePowerSlotPalette(Color rimBase, Color fillTop, Color fillBottom, Color highlightBase)
    {
        var outerStroke = Blend(rimBase, Color.Black, 0.52f);
        var rimTop = Blend(rimBase, Color.White, 0.18f);
        var rimBottom = Blend(rimBase, Color.Black, 0.22f);
        var fillMid = Blend(fillTop, fillBottom, 0.56f);
        var glossTop = Color.FromArgb(92, highlightBase);
        var glossBottom = Color.FromArgb(0, highlightBase);
        var innerShadow = Color.FromArgb(104, Blend(fillBottom, Color.Black, 0.58f));
        var highlightStroke = Color.FromArgb(122, Blend(highlightBase, Color.White, 0.14f));
        return new PowerSlotPalette(outerStroke, rimTop, rimBottom, fillTop, fillMid, fillBottom, glossTop, glossBottom, innerShadow, highlightStroke);
    }

    private void DrawIconWellOverlay(Graphics g, Rectangle wellRect, PowerSlotPalette palette, Rectangle? customSocketRect = null)
    {
        if (wellRect.Width <= 0 || wellRect.Height <= 0)
        {
            return;
        }

        var socketRect = customSocketRect ?? GetIconSocketRect(wellRect);
        if (socketRect.Width <= 0 || socketRect.Height <= 0)
        {
            return;
        }

        using var socketPath = new GraphicsPath();
        socketPath.AddEllipse(socketRect);

        var cavityShadow = Blend(palette.FillBottom, Color.Black, 0.82f);
        var deepShadow = Blend(cavityShadow, Color.Black, 0.26f);
        var upperShadow = Blend(palette.InnerShadow, Color.Black, 0.64f);

        var upperLeftShadeState = g.Save();
        g.SetClip(socketPath, CombineMode.Intersect);
        using (var upperLeftShadeBrush = new LinearGradientBrush(
                   new PointF(socketRect.Left, socketRect.Top),
                   new PointF(socketRect.Right, socketRect.Bottom),
                   Color.FromArgb(72, upperShadow),
                   Color.FromArgb(0, upperShadow)))
        {
            g.FillPath(upperLeftShadeBrush, socketPath);
        }
        g.Restore(upperLeftShadeState);

        var aoWidth = Math.Max(2.0f, EffectiveScale * 2.2f);
        var ambientOcclusionState = g.Save();
        g.SetClip(socketPath, CombineMode.Intersect);
        using (var aoPen = new Pen(Color.FromArgb(60, upperShadow), aoWidth))
        {
            g.DrawEllipse(aoPen, socketRect);
        }
        g.Restore(ambientOcclusionState);

        var upperArcState = g.Save();
        g.SetClip(socketPath, CombineMode.Intersect);
        g.SetClip(new Rectangle(
            socketRect.X,
            socketRect.Y,
            Math.Max(2, (int)Math.Round(socketRect.Width * 0.68f)),
            Math.Max(2, (int)Math.Round(socketRect.Height * 0.62f))),
            CombineMode.Intersect);
        using (var upperArcPen = new Pen(Color.FromArgb(92, Blend(upperShadow, Color.Black, 0.24f)), Math.Max(2.2f, EffectiveScale * 2.4f)))
        {
            g.DrawEllipse(upperArcPen, socketRect);
        }
        g.Restore(upperArcState);

        var innerOcclusionRect = DeflateRect(socketRect, Math.Max(1, ScaleLogical(1)));
        if (innerOcclusionRect.Width > 0 && innerOcclusionRect.Height > 0)
        {
            var innerOcclusionState = g.Save();
            g.SetClip(socketPath, CombineMode.Intersect);
            g.SetClip(new Rectangle(
                innerOcclusionRect.X,
                innerOcclusionRect.Y,
                Math.Max(2, (int)Math.Round(innerOcclusionRect.Width * 0.76f)),
                Math.Max(2, (int)Math.Round(innerOcclusionRect.Height * 0.72f))),
                CombineMode.Intersect);
            using (var innerOcclusionPen = new Pen(Color.FromArgb(70, deepShadow), Math.Max(1.2f, EffectiveScale * 1.45f)))
            {
                g.DrawEllipse(innerOcclusionPen, innerOcclusionRect);
            }
            g.Restore(innerOcclusionState);
        }

        var innerStrokeRect = DeflateRect(socketRect, Math.Max(1, ScaleLogical(1)));
        if (innerStrokeRect.Width > 0 && innerStrokeRect.Height > 0)
        {
            using var socketInnerShadowStroke = new Pen(Color.FromArgb(56, Blend(palette.OuterStroke, Color.Black, 0.24f)));
            g.DrawEllipse(socketInnerShadowStroke, innerStrokeRect);
        }
    }

    private void DrawIconWell(Graphics g, Rectangle wellRect, PowerSlotPalette palette, Rectangle? customSocketRect = null)
    {
        if (wellRect.Width <= 0 || wellRect.Height <= 0)
        {
            return;
        }

        var ringRect = DeflateRect(wellRect, Math.Max(2, ScaleLogical(2)));
        var midRingRect = DeflateRect(wellRect, Math.Max(3, ScaleLogical(3)));
        var innerBevelRect = DeflateRect(wellRect, Math.Max(4, ScaleLogical(4)));
        var socketRect = customSocketRect ?? GetIconSocketRect(wellRect);

        if (ringRect.Width <= 0 || ringRect.Height <= 0)
        {
            return;
        }

        using var ringPath = new GraphicsPath();
        ringPath.AddEllipse(ringRect);
        using var ringBrush = CreateThreeStopBrush(ringRect, palette.RimTop, Blend(palette.RimTop, palette.RimBottom, 0.45f), palette.RimBottom);
        g.FillPath(ringBrush, ringPath);

        if (midRingRect.Width > 0 && midRingRect.Height > 0)
        {
            using var midRingPath = new GraphicsPath();
            midRingPath.AddEllipse(midRingRect);
            using var midRingBrush = CreateThreeStopBrush(
                midRingRect,
                Blend(palette.RimTop, Color.White, 0.20f),
                Blend(palette.RimTop, palette.FillTop, 0.28f),
                Blend(palette.RimBottom, Color.Black, 0.10f));
            g.FillPath(midRingBrush, midRingPath);
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
            g.FillPath(innerBevelBrush, innerBevelPath);
        }

        if (socketRect.Width <= 0 || socketRect.Height <= 0)
        {
            return;
        }

        using var socketPath = new GraphicsPath();
        socketPath.AddEllipse(socketRect);
        var cavityBase = Blend(palette.FillBottom, Color.Black, 0.66f);
        using (var socketBaseBrush = new SolidBrush(cavityBase))
        {
            g.FillPath(socketBaseBrush, socketPath);
        }

        using (var cavityDepthBrush = new PathGradientBrush(socketPath)
        {
            CenterColor = Color.FromArgb(216, Color.Black),
            CenterPoint = new PointF(socketRect.X + socketRect.Width * 0.50f, socketRect.Y + socketRect.Height * 0.50f),
            SurroundColors = Enumerable.Repeat(Color.FromArgb(0, Color.Black), socketPath.PathPoints.Length).ToArray(),
            FocusScales = new PointF(0.28f, 0.28f)
        })
        {
            g.FillPath(cavityDepthBrush, socketPath);
        }

        var ringHighlightState = g.Save();
        g.SetClip(new Rectangle(ringRect.X, ringRect.Y, ringRect.Width, Math.Max(2, ringRect.Height / 2)), CombineMode.Intersect);
        using (var ringHighlightPen = new Pen(Color.FromArgb(88, palette.HighlightStroke), Math.Max(1.0f, EffectiveScale)))
        {
            g.DrawPath(ringHighlightPen, ringPath);
        }
        g.Restore(ringHighlightState);
    }

    private void DrawEmptyState(Graphics g, DataViewTheme theme)
    {
        using var font = CreateScaledFont(10f, FontStyle.Bold);
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

    private Font CreateScaledFont(float baseSize, FontStyle style, GraphicsUnit unit = GraphicsUnit.Point)
        => new("Segoe UI", Math.Max(6f, baseSize * _uiScale), style, unit);

    private int ScalePx(int value) => ScalePx(value, _uiScale);
    private int ScaleLogical(int value) => Math.Max(1, (int)Math.Round(value * EffectiveScale));

    private static int ScalePx(int value, float uiScale)
        => Math.Max(1, (int)Math.Round(value * uiScale));

    private static Rectangle GetIconWellRect(Rectangle powerRect)
    {
        var extra = Math.Max(8, (int)Math.Round(powerRect.Height * 0.42f));
        var diameter = Math.Max(1, powerRect.Height + extra);
        var y = powerRect.Y - extra / 2;
        return new Rectangle(powerRect.X, y, diameter, diameter);
    }

    private static Rectangle GetPowerBodyRect(Rectangle powerRect, Rectangle iconWellRect)
    {
        var overlapStart = (int)Math.Round(iconWellRect.Width * 0.44f);
        var bodyLeft = iconWellRect.Left + overlapStart;
        return new Rectangle(
            bodyLeft,
            powerRect.Y,
            Math.Max(1, powerRect.Right - bodyLeft),
            powerRect.Height);
    }

    private static Rectangle GetSocketRect(Rectangle outerRect, float insetRatio, int minInset)
    {
        var inset = Math.Max(minInset, (int)Math.Round(outerRect.Width * insetRatio));
        return Rectangle.Inflate(outerRect, -inset, -inset);
    }

    private static Rectangle GetIconSocketRect(Rectangle iconWellRect)
        => GetSocketRect(iconWellRect, 0.16f, 3);

    private Rectangle GetVisibleBitmapBounds(Bitmap bitmap)
    {
        if (_iconVisibleBoundsCache.TryGetValue(bitmap, out var cachedBounds))
        {
            return cachedBounds;
        }

        var fullBounds = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        if (bitmap.Width <= 0 || bitmap.Height <= 0)
        {
            return fullBounds;
        }

        try
        {
            var left = bitmap.Width;
            var top = bitmap.Height;
            var right = -1;
            var bottom = -1;

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, y).A <= IconVisibleAlphaThreshold)
                    {
                        continue;
                    }

                    left = Math.Min(left, x);
                    top = Math.Min(top, y);
                    right = Math.Max(right, x);
                    bottom = Math.Max(bottom, y);
                }
            }

            cachedBounds = right >= left && bottom >= top
                ? Rectangle.FromLTRB(left, top, right + 1, bottom + 1)
                : fullBounds;
        }
        catch
        {
            cachedBounds = fullBounds;
        }

        _iconVisibleBoundsCache[bitmap] = cachedBounds;
        return cachedBounds;
    }

    private Rectangle GetSocketIconDestinationRect(Image image, Rectangle socketRect)
    {
        if (image.Width <= 0 || image.Height <= 0 || socketRect.Width <= 0 || socketRect.Height <= 0)
        {
            return socketRect;
        }

        var visibleBounds = image is Bitmap bitmap
            ? GetVisibleBitmapBounds(bitmap)
            : new Rectangle(0, 0, image.Width, image.Height);

        if (visibleBounds.Width <= 0 || visibleBounds.Height <= 0)
        {
            visibleBounds = new Rectangle(0, 0, image.Width, image.Height);
        }

        var targetWidth = socketRect.Width * IconWellContentFill;
        var targetHeight = socketRect.Height * IconWellContentFill;
        var scale = Math.Min(targetWidth / visibleBounds.Width, targetHeight / visibleBounds.Height);
        scale = Math.Max(scale, 0.01f);

        var destWidth = image.Width * scale;
        var destHeight = image.Height * scale;
        var socketCenterX = socketRect.X + socketRect.Width / 2f;
        var socketCenterY = socketRect.Y + socketRect.Height / 2f;
        var visibleCenterX = visibleBounds.X + visibleBounds.Width / 2f;
        var visibleCenterY = visibleBounds.Y + visibleBounds.Height / 2f;
        var x = socketCenterX - visibleCenterX * scale;
        var y = socketCenterY - visibleCenterY * scale;

        var left = (int)Math.Round(x);
        var top = (int)Math.Round(y);
        var right = (int)Math.Round(x + destWidth);
        var bottom = (int)Math.Round(y + destHeight);
        return Rectangle.FromLTRB(left, top, Math.Max(left + 1, right), Math.Max(top + 1, bottom));
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

    private static Rectangle DeflateRect(Rectangle rect, int amount)
    {
        return new Rectangle(
            rect.X + amount,
            rect.Y + amount,
            Math.Max(0, rect.Width - amount * 2),
            Math.Max(0, rect.Height - amount * 2));
    }

    private static Color ResolveColor(Color candidate, Color fallback)
    {
        return candidate.IsEmpty ? fallback : candidate;
    }

    private static GraphicsPath CreateCapsulePath(Rectangle bounds)
    {
        var path = new GraphicsPath();
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return path;
        }

        if (bounds.Width <= bounds.Height)
        {
            path.AddEllipse(bounds);
            return path;
        }

        path.AddArc(bounds.X, bounds.Y, bounds.Height, bounds.Height, 90, 180);
        path.AddArc(bounds.Right - bounds.Height, bounds.Y, bounds.Height, bounds.Height, 270, 180);
        path.CloseFigure();
        return path;
    }

    private static GraphicsPath CreateMergedPowerSlotPath(Rectangle bodyBounds, Rectangle wellBounds)
    {
        var path = new GraphicsPath
        {
            FillMode = FillMode.Winding
        };

        if (bodyBounds.Width > 0 && bodyBounds.Height > 0)
        {
            using var bodyPath = CreateCapsulePath(bodyBounds);
            path.AddPath(bodyPath, false);
        }

        if (wellBounds.Width > 0 && wellBounds.Height > 0)
        {
            path.AddEllipse(wellBounds);
        }

        return path;
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

    private ApplicationTheme CurrentApplicationTheme => DesignMode
        ? ThemeManager.DesignTime
        : ThemeManager.CurrentTheme ?? ThemeManager.DesignTime;

    private PowerSlotTheme CurrentPowerSlotTheme => DesignMode
        ? ThemeManager.DesignTime.PowerSlot
        : ThemeManager.CurrentTheme?.PowerSlot ?? ThemeManager.DesignTime.PowerSlot;

    private bool IsInDesignMode =>
        DesignMode ||
        LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
        AppDomain.CurrentDomain.FriendlyName.Contains("devenv", StringComparison.OrdinalIgnoreCase) ||
        AppDomain.CurrentDomain.FriendlyName.Contains("designtoolsserver", StringComparison.OrdinalIgnoreCase) ||
        Process.GetCurrentProcess().ProcessName.Contains("devenv", StringComparison.OrdinalIgnoreCase) ||
        Process.GetCurrentProcess().ProcessName.Contains("designtoolsserver", StringComparison.OrdinalIgnoreCase) ||
        Process.GetCurrentProcess().ProcessName.Contains("xdesproc", StringComparison.OrdinalIgnoreCase);
}

using System.ComponentModel;
using System.Diagnostics;
using Mids_Reborn.Core;
using Mids_Reborn.UI.Theming;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls;

public sealed class PetActorRibbonItem
{
    public RealPetActorRosterItem Actor { get; init; } = null!;
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public Image? IconImage { get; init; }
    public bool IsSelected { get; init; }
}

public sealed class PetActorRibbon : Control
{
    private const float MinimumResponsiveUiScale = 0.90f;
    private readonly List<(Rectangle Bounds, PetActorRibbonItem Item)> _itemBounds = [];
    private IReadOnlyList<PetActorRibbonItem> _items = [];
    private readonly PetActorRibbonItem[] _designTimeItems =
    [
        new()
        {
            Actor = new RealPetActorRosterItem
            {
                EntityUid = "Alpha_Howler_Wolf",
                EntityDisplayName = "Alpha Howler Wolf",
                SourceHistoryIndex = 0,
                SourcePowerDisplayName = "Summon Wolves",
                Count = 1
            },
            Title = "Alpha Howler Wolf",
            Subtitle = "Summon Wolves",
            IsSelected = true
        },
        new()
        {
            Actor = new RealPetActorRosterItem
            {
                EntityUid = "Howler_Wolf",
                EntityDisplayName = "Howler Wolf",
                SourceHistoryIndex = 1,
                SourcePowerDisplayName = "Summon Wolves",
                Count = 2
            },
            Title = "Howler Wolf",
            Subtitle = "Summon Wolves",
            IsSelected = false
        }
    ];
    private float _uiScale = 1f;

    public event EventHandler<PetActorRibbonItem>? ItemSelected;

    public PetActorRibbon()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        BackColor = Color.Black;
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

    public void SetItems(IReadOnlyList<PetActorRibbonItem> items)
    {
        _items = items ?? [];
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

        var theme = CurrentTheme;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(theme.Background);
        _itemBounds.Clear();

        var items = _items.Count == 0 && IsInDesignMode
            ? _designTimeItems
            : _items;

        if (items.Count == 0)
        {
            return;
        }

        var itemHeight = ScalePx(58);
        var gap = ScalePx(8);
        var minItemWidth = ScalePx(170);
        var idealItemWidth = ScalePx(220);
        var availableWidth = Math.Max(0, ClientSize.Width - ScalePx(8));
        var computedWidth = items.Count == 0
            ? idealItemWidth
            : (availableWidth - ((items.Count - 1) * gap)) / items.Count;
        var itemWidth = Math.Max(minItemWidth, Math.Min(idealItemWidth, computedWidth));
        var x = ScalePx(4);
        var y = ScalePx(4);

        foreach (var item in items)
        {
            var bounds = new Rectangle(x, y, itemWidth, itemHeight);
            DrawItem(e.Graphics, bounds, item, theme);
            _itemBounds.Add((bounds, item));
            x += itemWidth + gap;
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        foreach (var (bounds, item) in _itemBounds)
        {
            if (!bounds.Contains(e.Location))
            {
                continue;
            }

            ItemSelected?.Invoke(this, item);
            break;
        }
    }

    private void RebuildLayout()
    {
        Invalidate();
    }

    private void DrawItem(Graphics g, Rectangle bounds, PetActorRibbonItem item, DataViewTheme theme)
    {
        var cardColor = item.IsSelected ? theme.HeaderBottom : theme.Card;
        var borderColor = item.IsSelected ? theme.Accent : theme.Border;
        var titleColor = item.IsSelected ? theme.ValueText : theme.Text;
        var subTitleColor = item.IsSelected ? Color.FromArgb(230, theme.Text) : theme.Muted;
        var iconSize = ScalePx(38);
        var iconBounds = new Rectangle(bounds.X + ScalePx(10), bounds.Y + ScalePx(10), iconSize, iconSize);

        using var path = CreateRoundedRect(bounds, ScalePx(6));
        using var fill = new LinearGradientBrush(bounds,
            item.IsSelected ? theme.HeaderTop : theme.GridHeaderTop,
            cardColor,
            LinearGradientMode.Vertical);
        using var borderPen = new Pen(borderColor, item.IsSelected ? 2f : 1f);
        g.FillPath(fill, path);
        g.DrawPath(borderPen, path);

        DrawIcon(g, iconBounds, item.IconImage, item.Title, theme);

        var rightInset = ScalePx(68);
        var titleBounds = new Rectangle(iconBounds.Right + ScalePx(10), bounds.Y + ScalePx(7), bounds.Width - iconBounds.Width - rightInset, ScalePx(22));
        var subtitleBounds = new Rectangle(iconBounds.Right + ScalePx(10), bounds.Y + ScalePx(29), bounds.Width - iconBounds.Width - rightInset, ScalePx(18));
        using var titleFont = CreateScaledFont(10f, FontStyle.Bold);
        using var subtitleFont = CreateScaledFont(8.5f, FontStyle.Regular);

        TextRenderer.DrawText(
            g,
            item.Title,
            titleFont,
            titleBounds,
            titleColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

        TextRenderer.DrawText(
            g,
            item.Subtitle,
            subtitleFont,
            subtitleBounds,
            subTitleColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

        if (item.Actor.Count > 1)
        {
            var badgeBounds = new Rectangle(bounds.Right - ScalePx(46), bounds.Y + ScalePx(10), ScalePx(34), ScalePx(18));
            using var badgeBrush = new SolidBrush(theme.ChipActive);
            using var badgePen = new Pen(theme.GridHeaderBorder);
            using var badgePath = CreateRoundedRect(badgeBounds, ScalePx(8));
            using var badgeFont = CreateScaledFont(8f, FontStyle.Bold);
            g.FillPath(badgeBrush, badgePath);
            g.DrawPath(badgePen, badgePath);
            TextRenderer.DrawText(
                g,
                $"x{item.Actor.Count}",
                badgeFont,
                badgeBounds,
                theme.ValueText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    private void DrawIcon(Graphics g, Rectangle bounds, Image? image, string title, DataViewTheme theme)
    {
        if (image != null)
        {
            var widthScale = bounds.Width / (float)image.Width;
            var heightScale = bounds.Height / (float)image.Height;
            var scale = Math.Min(widthScale, heightScale);
            var drawWidth = image.Width * scale;
            var drawHeight = image.Height * scale;
            var drawX = bounds.X + (bounds.Width - drawWidth) / 2f;
            var drawY = bounds.Y + (bounds.Height - drawHeight) / 2f;
            g.DrawImage(image, drawX, drawY, drawWidth, drawHeight);
            return;
        }

        using var fallbackPath = CreateRoundedRect(bounds, ScalePx(8));
        using var fallbackBrush = new SolidBrush(Color.FromArgb(26, theme.GridHeaderTop));
        using var fallbackPen = new Pen(Color.FromArgb(120, theme.GridHeaderBorder));
        g.FillPath(fallbackBrush, fallbackPath);
        g.DrawPath(fallbackPen, fallbackPath);

        var initials = string.IsNullOrWhiteSpace(title)
            ? "P"
            : new string(title
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(part => char.ToUpperInvariant(part[0]))
                .ToArray());
        using var font = CreateScaledFont(9f, FontStyle.Bold);
        TextRenderer.DrawText(
            g,
            initials,
            font,
            bounds,
            theme.ValueText,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
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

    private Font CreateScaledFont(float baseSize, FontStyle style)
        => new("Segoe UI", Math.Max(6f, baseSize * _uiScale), style, GraphicsUnit.Point);

    private int ScalePx(int value)
        => Math.Max(1, (int)Math.Round(value * _uiScale));

    private DataViewTheme CurrentTheme => DesignMode
        ? ThemeManager.DesignTime.DataView
        : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

    private bool IsInDesignMode =>
        DesignMode ||
        LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
        AppDomain.CurrentDomain.FriendlyName.Contains("devenv", StringComparison.OrdinalIgnoreCase) ||
        AppDomain.CurrentDomain.FriendlyName.Contains("designtoolsserver", StringComparison.OrdinalIgnoreCase) ||
        Process.GetCurrentProcess().ProcessName.Contains("devenv", StringComparison.OrdinalIgnoreCase) ||
        Process.GetCurrentProcess().ProcessName.Contains("designtoolsserver", StringComparison.OrdinalIgnoreCase) ||
        Process.GetCurrentProcess().ProcessName.Contains("xdesproc", StringComparison.OrdinalIgnoreCase);
}

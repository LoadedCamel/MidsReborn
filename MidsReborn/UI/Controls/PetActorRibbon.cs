using Mids_Reborn.Core;
using Mids_Reborn.UI.Theming;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls;

public sealed class PetActorRibbonItem
{
    public RealPetActorRosterItem Actor { get; init; } = null!;
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public bool IsSelected { get; init; }
}

public sealed class PetActorRibbon : Control
{
    private readonly List<(Rectangle Bounds, PetActorRibbonItem Item)> _itemBounds = [];
    private IReadOnlyList<PetActorRibbonItem> _items = [];

    public event EventHandler<PetActorRibbonItem>? ItemSelected;

    public PetActorRibbon()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        BackColor = Color.Black;
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

        if (_items.Count == 0)
        {
            return;
        }

        const int itemHeight = 58;
        const int gap = 8;
        const int minItemWidth = 170;
        const int idealItemWidth = 220;
        var availableWidth = Math.Max(0, ClientSize.Width - 8);
        var computedWidth = _items.Count == 0
            ? idealItemWidth
            : (availableWidth - ((_items.Count - 1) * gap)) / _items.Count;
        var itemWidth = Math.Max(minItemWidth, Math.Min(idealItemWidth, computedWidth));
        var x = 4;
        var y = 4;

        foreach (var item in _items)
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

    private static void DrawItem(Graphics g, Rectangle bounds, PetActorRibbonItem item, DataViewTheme theme)
    {
        var cardColor = item.IsSelected ? theme.HeaderBottom : theme.Card;
        var borderColor = item.IsSelected ? theme.Accent : theme.Border;
        var titleColor = item.IsSelected ? theme.ValueText : theme.Text;
        var subTitleColor = item.IsSelected ? Color.FromArgb(230, theme.Text) : theme.Muted;

        using var path = CreateRoundedRect(bounds, 6);
        using var fill = new LinearGradientBrush(bounds,
            item.IsSelected ? theme.HeaderTop : theme.GridHeaderTop,
            cardColor,
            LinearGradientMode.Vertical);
        using var borderPen = new Pen(borderColor, item.IsSelected ? 2f : 1f);
        g.FillPath(fill, path);
        g.DrawPath(borderPen, path);

        var titleBounds = new Rectangle(bounds.X + 12, bounds.Y + 7, bounds.Width - 58, 22);
        var subtitleBounds = new Rectangle(bounds.X + 12, bounds.Y + 29, bounds.Width - 58, 18);
        using var titleFont = new Font("Segoe UI", 10f, FontStyle.Bold);
        using var subtitleFont = new Font("Segoe UI", 8.5f, FontStyle.Regular);

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
            var badgeBounds = new Rectangle(bounds.Right - 46, bounds.Y + 10, 34, 18);
            using var badgeBrush = new SolidBrush(theme.ChipActive);
            using var badgePen = new Pen(theme.GridHeaderBorder);
            using var badgePath = CreateRoundedRect(badgeBounds, 8);
            using var badgeFont = new Font("Segoe UI", 8f, FontStyle.Bold);
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

    private DataViewTheme CurrentTheme => DesignMode
        ? ThemeManager.DesignTime.DataView
        : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;
}

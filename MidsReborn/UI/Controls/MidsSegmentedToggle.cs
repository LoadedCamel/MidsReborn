using System.ComponentModel;
using System.Drawing.Drawing2D;
using Mids_Reborn.Core.Theming;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

[DefaultEvent("SelectedIndexChanged")]
[DesignerCategory("Code")]
public sealed class MidsSegmentedToggle : Control
{
    private static readonly Padding DefaultSegmentPadding = new(1, 1, 1, 1);
    private readonly List<string> _items = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _pressedIndex = -1;
    private bool _themeHooked;
    private Action? _themeChangedHandler;

    public event EventHandler? SelectedIndexChanged;

    [Browsable(false)]
    public IReadOnlyList<string> Items => _items;

    [DefaultValue(-1)]
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            int normalized = NormalizeSelectedIndex(value);
            if (_selectedIndex == normalized)
            {
                return;
            }

            _selectedIndex = normalized;
            Invalidate();
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [DefaultValue(7)]
    public int CornerRadius { get; set; } = 7;

    [Browsable(false)]
    public Padding SegmentPadding { get; set; } = DefaultSegmentPadding;

    private ButtonTheme CurrentTheme
    {
        get
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return ThemeManager.DesignTime.Button;
            }

            return ThemeManager.CurrentTheme?.Button ?? ThemeManager.DesignTime.Button;
        }
    }

    private SegmentedToggleTheme CurrentSegmentedToggleTheme
    {
        get
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return ThemeManager.DesignTime.SegmentedToggle ?? new SegmentedToggleTheme();
            }

            return ThemeManager.CurrentTheme?.SegmentedToggle
                ?? ThemeManager.DesignTime.SegmentedToggle
                ?? new SegmentedToggleTheme();
        }
    }

    public MidsSegmentedToggle()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor |
            ControlStyles.UserPaint,
            true);

        BackColor = Color.Transparent;
        Font = new Font("Noto Sans SemiBold", 9.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        ForeColor = Color.White;
        Cursor = Cursors.Hand;
        Size = new Size(180, 30);
        SetItems("One", "Two");
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (_themeHooked || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            return;
        }

        _themeChangedHandler = Invalidate;
        ThemeManager.ThemeChanged += _themeChangedHandler;
        _themeHooked = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _themeHooked && _themeChangedHandler is not null)
        {
            ThemeManager.ThemeChanged -= _themeChangedHandler;
            _themeHooked = false;
            _themeChangedHandler = null;
        }

        base.Dispose(disposing);
    }

    public void SetItems(params string[] items)
    {
        _items.Clear();
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                continue;
            }

            _items.Add(item.Trim());
        }

        _selectedIndex = NormalizeSelectedIndex(_selectedIndex);
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoveredIndex = -1;
        _pressedIndex = -1;
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        int hoveredIndex = HitTest(e.Location);
        if (_hoveredIndex == hoveredIndex)
        {
            return;
        }

        _hoveredIndex = hoveredIndex;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        _pressedIndex = HitTest(e.Location);
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        int hitIndex = HitTest(e.Location);
        int pressedIndex = _pressedIndex;
        _pressedIndex = -1;

        if (hitIndex >= 0 && hitIndex == pressedIndex)
        {
            SelectedIndex = hitIndex;
        }
        else
        {
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var theme = CurrentTheme;
        var segmentedTheme = CurrentSegmentedToggleTheme;
        var outerBounds = new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1));
        if (_items.Count == 0 || outerBounds.Width <= 0 || outerBounds.Height <= 0)
        {
            return;
        }

        int radius = Math.Min(CornerRadius, Math.Max(2, Height / 2));
        using var outerPath = CreateRoundedRect(outerBounds, radius);

        Color wellTop = segmentedTheme.WellTop;
        Color wellBottom = segmentedTheme.WellBottom;
        using (var baseBrush = new LinearGradientBrush(outerBounds, wellTop, wellBottom, 90f))
        {
            e.Graphics.FillPath(baseBrush, outerPath);
        }

        using (var baseBorderPen = new Pen(Blend(segmentedTheme.Divider, wellBottom, 0.52f), 1f))
        {
            e.Graphics.DrawPath(baseBorderPen, outerPath);
        }

        using (var clipRegion = new Region(outerPath))
        {
            e.Graphics.SetClip(clipRegion, CombineMode.Replace);

            using (var dividerPen = new Pen(Color.FromArgb(34, segmentedTheme.Divider)))
            {
                for (int index = 1; index < _items.Count; index++)
                {
                    int x = GetSegmentBounds(index).Left;
                    e.Graphics.DrawLine(dividerPen, x, 5, x, Height - 6);
                }
            }

            for (int index = 0; index < _items.Count; index++)
            {
                var segmentBounds = GetSegmentPaintBounds(index);
                if (segmentBounds.Width <= 0 || segmentBounds.Height <= 0)
                {
                    continue;
                }

                bool isSelected = index == _selectedIndex;
                bool isPressed = index == _pressedIndex;
                bool isHovered = index == _hoveredIndex;
                if (!isSelected && !isPressed && !isHovered)
                {
                    continue;
                }

                using var segmentPath = CreateRoundedRect(segmentBounds, Math.Min(radius - 1, Math.Max(2, segmentBounds.Height / 2)));
                var topColor = isSelected
                    ? segmentedTheme.SelectedTop
                    : isPressed
                        ? Blend(segmentedTheme.SelectedTop, wellTop, 0.76f)
                        : Blend(segmentedTheme.SelectedTop, wellTop, 0.88f);
                var bottomColor = isSelected
                    ? segmentedTheme.SelectedBottom
                    : isPressed
                        ? Blend(segmentedTheme.SelectedBottom, wellBottom, 0.76f)
                        : Blend(segmentedTheme.SelectedBottom, wellBottom, 0.88f);

                using var fillBrush = new LinearGradientBrush(segmentBounds, topColor, bottomColor, 90f);
                e.Graphics.FillPath(fillBrush, segmentPath);

                if (isSelected)
                {
                    using var selectedPen = new Pen(segmentedTheme.SelectedBorder, Math.Max(1.05f, theme.ToggledBorderWidth - 0.7f));
                    e.Graphics.DrawPath(selectedPen, segmentPath);
                }
            }
        }

        e.Graphics.ResetClip();

        for (int index = 0; index < _items.Count; index++)
        {
            bool isSelected = index == _selectedIndex;
            DrawSegmentText(
                e.Graphics,
                _items[index],
                Rectangle.Inflate(GetSegmentBounds(index), -4, -1),
                isSelected ? segmentedTheme.SelectedText : segmentedTheme.UnselectedText,
                isSelected ? segmentedTheme.SelectedTextOutline : segmentedTheme.UnselectedTextOutline,
                theme.TextOutlineWidth);
        }
    }

    private int NormalizeSelectedIndex(int index)
    {
        if (_items.Count == 0)
        {
            return -1;
        }

        if (index < 0)
        {
            return 0;
        }

        return Math.Min(index, _items.Count - 1);
    }

    private int HitTest(Point point)
    {
        for (int index = 0; index < _items.Count; index++)
        {
            if (GetSegmentBounds(index).Contains(point))
            {
                return index;
            }
        }

        return -1;
    }

    private Rectangle GetSegmentBounds(int index)
    {
        int left = (Width * index) / _items.Count;
        int right = (Width * (index + 1)) / _items.Count;
        return new Rectangle(left, 0, Math.Max(1, right - left), Height);
    }

    private Rectangle GetSegmentPaintBounds(int index)
    {
        var bounds = GetSegmentBounds(index);
        int topInset = SegmentPadding.Top + 1;
        int bottomInset = SegmentPadding.Bottom + 2;
        int leftInset = index == 0 ? SegmentPadding.Left + 1 : Math.Max(SegmentPadding.Left, 2);
        int rightInset = index == _items.Count - 1 ? SegmentPadding.Right + 1 : Math.Max(SegmentPadding.Right, 2);
        return Rectangle.FromLTRB(
            bounds.Left + leftInset,
            bounds.Top + topInset,
            Math.Max(bounds.Left + leftInset + 1, bounds.Right - rightInset),
            Math.Max(bounds.Top + topInset + 1, bounds.Bottom - bottomInset));
    }

    private void DrawSegmentText(Graphics graphics, string text, Rectangle bounds, Color fillColor, Color outlineColor, float outlineWidth)
    {
        using var path = new GraphicsPath();
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        float emSize = Font.SizeInPoints * graphics.DpiY / 72f;
        path.AddString(text, Font.FontFamily, (int)Font.Style, emSize, bounds, format);

        using (var outlinePen = new Pen(outlineColor, outlineWidth) { LineJoin = LineJoin.Round })
        {
            graphics.DrawPath(outlinePen, path);
        }

        using var textBrush = new SolidBrush(fillColor);
        graphics.FillPath(textBrush, path);
    }

    private static GraphicsPath CreateRoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        int diameter = radius * 2;
        var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

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

    private static Color Blend(Color a, Color b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return Color.FromArgb(
            (int)Math.Round(a.A + (b.A - a.A) * t),
            (int)Math.Round(a.R + (b.R - a.R) * t),
            (int)Math.Round(a.G + (b.G - a.G) * t),
            (int)Math.Round(a.B + (b.B - a.B) * t));
    }
}

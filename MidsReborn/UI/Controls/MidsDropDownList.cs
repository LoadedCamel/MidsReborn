using Mids_Reborn.Core.Theming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

[DefaultProperty(nameof(Items))]
[DefaultEvent(nameof(SelectedIndexChanged))]
[DesignerCategory("Code")]
public class MidsDropDownList : ComboBox, ILiveResizeMetricsAware
{
    #region Constants

    private const int IconPadding = 4;

    #endregion

    #region Private Fields

    private string? _lockedText;
    private bool _isLocked;
    private bool _isHovering;
    private int _iconSize = 16;
    private int? _baseIconSize;
    private int? _baseItemHeight;
    private int? _baseControlHeight;
    private readonly Dictionary<object, Bitmap?> _itemIcons = new();
    private string? _placeholderText;

    private IBindingList? _boundList;
    private bool _liveResizeMetricsFrozen;
    private float? _pendingUiScale;
    private bool _closedSurfaceLayoutValid;
    private ClosedSurfaceLayoutKey _closedSurfaceLayoutKey;
    private ClosedSurfaceLayout _closedSurfaceLayout;

    private readonly record struct FontSignature(string FamilyName, float SizeInPoints, FontStyle Style, byte GdiCharSet);
    private readonly record struct ClosedSurfaceLayoutKey(
        int ClientHeight,
        FontSignature Font,
        int IconSize,
        int ItemHeight,
        int Dpi,
        bool Locked,
        bool Placeholder);
    private readonly record struct ClosedSurfaceLayout(
        int TextHeight,
        int TextY,
        int IconY,
        int ArrowTop,
        int PlaceholderLeft);

    #endregion

    #region Public Properties

    [Category("Behavior")]
    [Description("Determines whether the ComboBox is locked from user interaction.")]
    [DefaultValue(false)]
    public bool IsLocked
    {
        get => _isLocked;
        set
        {
            _isLocked = value;
            _isHovering = false;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Target icon size to draw the item icons.")]
    [DefaultValue(16)]
    public int IconSize
    {
        get => _iconSize;
        set
        {
            _iconSize = Math.Max(8, Math.Min(64, value));
            Invalidate();
        }
    }

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public Dictionary<object, Bitmap?> ItemIcons => _itemIcons;

    [Category("Appearance")]
    [Description("Optional placeholder text shown when no item is selected.")]
    public string? PlaceholderText
    {
        get => _placeholderText;
        set
        {
            _placeholderText = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public string? LockedText => _lockedText;

    [Category("Data")]
    [Description("Callback that returns a Bitmap for a given bound item. Used to auto-populate ItemIcons.")]
    public Func<object, Bitmap?>? IconProvider { get; set; }

    #endregion

    #region Private Properties

    private bool IsInteracting => Focused || DroppedDown || Capture;
    private DropDownListTheme CurrentTheme
    {
        get
        {
            if (DesignMode)
            {
                return ThemeManager.DesignTime.DropDownList;
            }
            return ThemeManager.CurrentTheme?.DropDownList ?? ThemeManager.DesignTime.DropDownList;
        }
    }

    #endregion

    #region Constructors

    public MidsDropDownList()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        DrawMode = DrawMode.OwnerDrawFixed;
        DropDownStyle = ComboBoxStyle.DropDownList;
        IntegralHeight = false;
        AutoSize = false;

        if (!DesignMode) ThemeManager.ThemeChanged += Invalidate;
    }

    #endregion

    #region Public Methods

    public void Lock(string? text = null, bool clear = false)
    {
        _lockedText = text;
        IsLocked = true;
        if (clear)
        {
            SelectedItem = null;
            DataSource = null; 
            DisposeItemIcons();
            RefreshIcons(); 
        }

        Invalidate();
    }

    public void Unlock()
    {
        IsLocked = false;
        _lockedText = null;
        Invalidate();
    }

    public void ApplyUiScale(float scale)
    {
        if (_liveResizeMetricsFrozen)
        {
            _pendingUiScale = scale;
            return;
        }

        ApplyUiScaleCore(scale);
    }

    public void BeginLiveResizeMetrics()
    {
        _liveResizeMetricsFrozen = true;
        _pendingUiScale = null;
    }

    public void EndLiveResizeMetrics()
    {
        if (!_liveResizeMetricsFrozen)
        {
            return;
        }

        _liveResizeMetricsFrozen = false;
        if (_pendingUiScale is float pendingScale)
        {
            _pendingUiScale = null;
            ApplyUiScaleCore(pendingScale);
        }
        else
        {
            Invalidate();
        }
    }

    #endregion

    #region DataSource Methods

    protected override void OnDataSourceChanged(EventArgs e)
    {
        base.OnDataSourceChanged(e);
        DetachListChanged();
        AttachListChanged();
        RefreshIcons();      // rebuild icons for new data
        Invalidate();
    }

    protected override void OnDisplayMemberChanged(EventArgs e)
    {
        base.OnDisplayMemberChanged(e);
        Invalidate();
    }

    protected override void OnValueMemberChanged(EventArgs e)
    {
        base.OnValueMemberChanged(e);
        Invalidate();
    }

    private void AttachListChanged()
    {
        // If DataSource is an IBindingList, listen for changes to rebuild icons.
        if (DataSource is IBindingList bl)
        {
            _boundList = bl;
            _boundList.ListChanged += OnListChanged;
        }
        else
        {
            _boundList = null;
        }
    }

    private void DetachListChanged()
    {
        if (_boundList != null)
        {
            _boundList.ListChanged -= OnListChanged;
            _boundList = null;
        }
    }

    private void OnListChanged(object? sender, ListChangedEventArgs e)
    {
        // Full rebuild is simplest and safe; Items reflects DataSource state.
        RefreshIcons();
        Invalidate();
    }

    /// <summary>
    /// Rebuilds ItemIcons from current Items using IconProvider (if set).
    /// If IconProvider is null, leaves existing ItemIcons as-is.
    /// </summary>
    public void RefreshIcons()
    {
        DisposeItemIcons();
        if (IconProvider is null) return;

        // Note: Items enumerates display objects regardless of DataSource or manual adding.
        foreach (var obj in Items.Cast<object>())
        {
            try
            {
                var bmp = IconProvider(obj);
                // Store even null results to avoid repeated calls for missing icons
                _itemIcons[obj] = bmp != null ? new Bitmap(bmp) : null;
            }
            catch (ArgumentException)
            {
                _itemIcons[obj] = null;
            }
            catch (ObjectDisposedException)
            {
                _itemIcons[obj] = null;
            }
            catch
            {
                // swallow provider exceptions for robustness—control should still render
            }
        }
    }

    #endregion

    #region Drawing

    protected override void OnPaint(PaintEventArgs e)
    {
        if (DropDownStyle != ComboBoxStyle.DropDownList)
        {
            base.OnPaint(e);
            return;
        }

        Graphics g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        Rectangle rect = ClientRectangle;
        bool hasPlaceholder = SelectedItem is null && !string.IsNullOrWhiteSpace(PlaceholderText);
        var layout = GetClosedSurfaceLayout(g, Font, locked: false, hasPlaceholder);

        bool locked = _isLocked;
        bool hovering = _isHovering && !_isLocked && !IsInteracting;

        var theme = CurrentTheme;
        Color topColor = locked ? theme.GradientTop : hovering ? theme.HoverGradientTop : theme.GradientTop;
        Color bottomColor = locked ? theme.GradientBottom : hovering ? theme.HoverGradientBottom : theme.GradientBottom;
        Color arrowColor = hovering ? theme.HoverArrow : theme.Arrow;
        Color borderColor = hovering ? theme.HoverBorder : theme.Border;

        using (var brush = new LinearGradientBrush(rect, topColor, bottomColor, LinearGradientMode.Vertical))
            g.FillRectangle(brush, rect);

        if (locked && !string.IsNullOrWhiteSpace(_lockedText))
        {
            Font? lockedFont = null;
            var drawFont = Font;
            if ((Font.Style & FontStyle.Bold) == 0)
            {
                lockedFont = new Font(Font, Font.Style | FontStyle.Bold);
                drawFont = lockedFont;
            }

            var lockedLayout = GetClosedSurfaceLayout(g, drawFont, locked: true, hasPlaceholder: false);

            var selectedItem = SelectedIndex >= 0 && SelectedIndex < Items.Count
                ? Items[SelectedIndex]
                : SelectedItem;
            Bitmap? selectedIcon = null;
            var hasIcon = selectedItem != null &&
                          _itemIcons.TryGetValue(selectedItem, out selectedIcon) &&
                          selectedIcon != null;

            Size textMeasure = TextRenderer.MeasureText(g, _lockedText, drawFont, Size.Empty, TextFormatFlags.NoPadding);
            var contentWidth = Math.Max(0, rect.Width - IconPadding * 2);
            var iconBlockWidth = hasIcon ? IconSize + IconPadding : 0;
            var availableTextWidth = Math.Max(0, contentWidth - iconBlockWidth);
            var desiredTextWidth = Math.Min(textMeasure.Width, availableTextWidth);
            var totalWidth = hasIcon
                ? Math.Min(contentWidth, iconBlockWidth + desiredTextWidth)
                : Math.Min(contentWidth, desiredTextWidth);
            var contentLeft = rect.Left + Math.Max(IconPadding, (rect.Width - totalWidth) / 2);
            var textLeft = contentLeft;

            if (hasIcon)
            {
                var iconRect = new Rectangle(contentLeft, rect.Top + lockedLayout.IconY, IconSize, IconSize);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                DrawIconIfValid(g, selectedIcon!, iconRect);
                textLeft = iconRect.Right + IconPadding;
            }

            var textRect = new Rectangle(
                textLeft,
                lockedLayout.TextY,
                Math.Max(0, rect.Right - IconPadding - textLeft),
                lockedLayout.TextHeight);

            var color = Color.FromArgb(200, theme.ForeColor);
            var flags = hasIcon
                ? TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis
                : TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis;
            TextRenderer.DrawText(g, _lockedText, drawFont, textRect, color, flags);
            lockedFont?.Dispose();

            ControlPaint.DrawBorder(g, rect, borderColor, ButtonBorderStyle.Solid);
            // Arrow is intentionally not drawn when locked (matches your current behavior)
            return;
        }

        if (SelectedItem != null)
        {
            var selectedItem = SelectedIndex >= 0 && SelectedIndex < Items.Count
                ? Items[SelectedIndex]
                : SelectedItem;
            Rectangle iconRect = new Rectangle(rect.Left + IconPadding, rect.Top + layout.IconY, IconSize, IconSize);

            Rectangle textRect = new Rectangle(
                iconRect.Right + IconPadding,
                layout.TextY,
                rect.Right - iconRect.Right - IconPadding * 2,
                layout.TextHeight);

            if (selectedItem != null && _itemIcons.TryGetValue(selectedItem, out var icon) && icon != null)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                DrawIconIfValid(g, icon, iconRect);
            }

            TextRenderer.DrawText(
                g,
                GetItemText(selectedItem),
                Font,
                textRect,
                theme.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }
        else if (hasPlaceholder)
        {
            Rectangle textRect = new Rectangle(
                rect.Left + layout.PlaceholderLeft,
                layout.TextY,
                rect.Right - layout.PlaceholderLeft - IconPadding,
                layout.TextHeight);

            // Outline color (black)
            Color outlineColor = Color.Black;

            // Fill color (semi-transparent theme foreground)
            Color fillColor = Color.FromArgb(160, theme.ForeColor);

            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis;

            // Draw 1px outline using surrounding offset copies
            var offsets = new[]
            {
                new Point(-1, 0), new Point(1, 0),
                new Point(0, -1), new Point(0, 1),
                new Point(-1, -1), new Point(-1, 1),
                new Point(1, -1), new Point(1, 1)
            };

            foreach (var offset in offsets)
            {
                var outlineRect = new Rectangle(
                    textRect.X + offset.X,
                    textRect.Y + offset.Y,
                    textRect.Width,
                    textRect.Height);

                TextRenderer.DrawText(g, PlaceholderText, Font, outlineRect, outlineColor, flags);
            }

            // Draw the fill text on top
            TextRenderer.DrawText(g, PlaceholderText, Font, textRect, fillColor, flags);
        }

        ControlPaint.DrawBorder(g, rect, borderColor, ButtonBorderStyle.Solid);

        if (!locked)
        {
            Rectangle arrowRect = new Rectangle(rect.Right - 18, rect.Top + layout.ArrowTop, 10, 5);
            Point[] arrowPoints =
            [
                new(arrowRect.Left, arrowRect.Top),
                new(arrowRect.Right, arrowRect.Top),
                new(arrowRect.Left + arrowRect.Width / 2, arrowRect.Bottom)
            ];
            using var arrowBrush = new SolidBrush(arrowColor);
            g.FillPolygon(arrowBrush, arrowPoints);
        }
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= Items.Count)
        {
            base.OnDrawItem(e);
            return;
        }

        object? item = Items[e.Index];
        Rectangle bounds = e.Bounds;
        Graphics g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        bool isFocused = (e.State & DrawItemState.Focus) == DrawItemState.Focus;

        var theme = CurrentTheme;
        Color backColor = isSelected ? theme.DropDownSelectionBackColor : theme.DropDownBackColor;
        Color textColor = isSelected ? theme.DropDownSelectionForeColor : theme.ForeColor;

        using (var backBrush = new SolidBrush(backColor))
            g.FillRectangle(backBrush, bounds);

        // Icon layout
        Rectangle iconRect = new Rectangle(
            bounds.Left + IconPadding,
            bounds.Top + (bounds.Height - IconSize) / 2,
            IconSize,
            IconSize);

        Size textSize = TextRenderer.MeasureText(g, "Mg", Font, Size.Empty, TextFormatFlags.NoPadding);
        int textY = bounds.Top + (bounds.Height - textSize.Height) / 2 - 1;

        Rectangle textRect = new Rectangle(
            iconRect.Right + IconPadding,
            textY,
            bounds.Right - iconRect.Right - IconPadding * 2,
            textSize.Height);

        // Icon draw
        if (item != null && _itemIcons.TryGetValue(item, out var icon) && icon != null)
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            DrawIconIfValid(g, icon, iconRect);
        }

        // Text draw
        TextRenderer.DrawText(
            g,
            GetItemText(item) ?? string.Empty,
            Font,
            textRect,
            textColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

        if (isFocused)
        {
            using var pen = new Pen(theme.FocusBorder, 1);
            pen.DashStyle = DashStyle.Solid;
            var focusRect = bounds;
            focusRect.Inflate(-1, -1);
            g.DrawRectangle(pen, focusRect);
        }
    }

    #endregion

    #region Overridden Methods

    protected override void WndProc(ref Message m)
    {
        const int leftButtonDown = 0x0201;
        const int leftButtonDblClick = 0x0203;
        const int keyDown = 0x0100;

        if (_isLocked)
        {
            if (m.Msg is leftButtonDown or leftButtonDblClick or keyDown)
                return; // Swallow input when locked
        }

        base.WndProc(ref m);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        if (_isLocked) return;
        base.OnMouseEnter(e);
        _isHovering = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        if (_isLocked) return;
        base.OnMouseLeave(e);
        _isHovering = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (_isLocked) return;
        base.OnMouseDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (_isLocked)
        {
            e.Handled = true;
            return;
        }
        base.OnKeyDown(e);
    }

    protected override void OnDropDown(EventArgs e)
    {
        if (_isLocked) return;
        base.OnDropDown(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DetachListChanged();
            DisposeItemIcons();
            if (!DesignMode)
            {
                ThemeManager.ThemeChanged -= Invalidate;
            }
        }

        base.Dispose(disposing);
    }

    private void DisposeItemIcons()
    {
        foreach (var icon in _itemIcons.Values.Where(icon => icon != null))
        {
            icon.Dispose();
        }

        _itemIcons.Clear();
    }

    private static void DrawIconIfValid(Graphics graphics, Image icon, Rectangle bounds)
    {
        try
        {
            if (icon.Width <= 0 || icon.Height <= 0)
            {
                return;
            }

            graphics.DrawImage(icon, bounds);
        }
        catch (ArgumentException)
        {
            // Shared icon caches can refresh while a dropdown still has an older bitmap reference.
        }
        catch (ObjectDisposedException)
        {
            // Draw text-only instead of letting a stale icon crash the UI.
        }
    }

    private void ApplyUiScaleCore(float scale)
    {
        _baseIconSize ??= IconSize;
        _baseItemHeight ??= ItemHeight;
        _baseControlHeight ??= Height;

        int iconSize = Math.Clamp((int)Math.Round(_baseIconSize.Value * scale), 8, 64);
        int itemHeight = Math.Max(12, (int)Math.Round(_baseItemHeight.Value * scale));
        int verticalPadding = Math.Max(8, (int)Math.Round(10f * scale));
        int controlHeight = Math.Max(
            (int)Math.Round(_baseControlHeight.Value * scale),
            itemHeight + verticalPadding);

        if (IconSize == iconSize && ItemHeight == itemHeight && Height == controlHeight)
        {
            return;
        }

        IconSize = iconSize;
        ItemHeight = itemHeight;
        Height = controlHeight;
        Invalidate();
    }

    private ClosedSurfaceLayout GetClosedSurfaceLayout(Graphics graphics, Font drawFont, bool locked, bool hasPlaceholder)
    {
        var key = new ClosedSurfaceLayoutKey(
            ClientSize.Height,
            CreateFontSignature(drawFont),
            IconSize,
            ItemHeight,
            DeviceDpi,
            locked,
            hasPlaceholder);

        if (_closedSurfaceLayoutValid && _closedSurfaceLayoutKey == key)
        {
            return _closedSurfaceLayout;
        }

        Size textSize = TextRenderer.MeasureText(graphics, "Mg", drawFont, Size.Empty, TextFormatFlags.NoPadding);
        _closedSurfaceLayout = new ClosedSurfaceLayout(
            textSize.Height,
            ClientRectangle.Top + (ClientRectangle.Height - textSize.Height) / 2 - 1,
            ClientRectangle.Top + (ClientRectangle.Height - IconSize) / 2,
            ClientRectangle.Top + ClientRectangle.Height / 2 - 2,
            IconPadding);
        _closedSurfaceLayoutKey = key;
        _closedSurfaceLayoutValid = true;
        return _closedSurfaceLayout;
    }

    private static FontSignature CreateFontSignature(Font font)
        => new(font.FontFamily.Name, font.SizeInPoints, font.Style, font.GdiCharSet);

    #endregion
}

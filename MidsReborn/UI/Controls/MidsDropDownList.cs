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
    private const int ClosedSurfaceArrowReservedWidth = 18;
    private const int ClosedSurfaceTextSafetyPadding = 8;
    private const int PopupTextSafetyPadding = 8;

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
    private readonly Action _themeChangedHandler;
    private ToolStripDropDown? _customDropDownHost;
    private ToolStripControlHost? _customDropDownControlHost;
    private DropDownPopupList? _customDropDownList;
    private bool _focusOwnerAfterCustomDropDownClose;
    private bool _suppressNextCustomDropDownToggle;

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

    private bool IsInteracting => Focused || IsCustomDropDownVisible || Capture;
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
    private ScrollPanelTheme CurrentScrollTheme
    {
        get
        {
            if (DesignMode)
            {
                return ThemeManager.DesignTime.ScrollPanel;
            }

            return ThemeManager.CurrentTheme?.ScrollPanel ?? ThemeManager.DesignTime.ScrollPanel;
        }
    }
    private bool IsCustomDropDownOpen => _customDropDownHost is { Visible: true };
    private bool IsCustomDropDownVisible => DroppedDown || IsCustomDropDownOpen;

    #endregion

    #region Constructors

    public MidsDropDownList()
    {
        _themeChangedHandler = HandleThemeChanged;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        DrawMode = DrawMode.OwnerDrawFixed;
        DropDownStyle = ComboBoxStyle.DropDownList;
        IntegralHeight = false;
        AutoSize = false;

        if (!DesignMode) ThemeManager.ThemeChanged += _themeChangedHandler;
    }

    #endregion

    #region Public Methods

    public void Lock(string? text = null, bool clear = false)
    {
        HideCustomDropDown();
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

    public int GetPreferredContentWidth(bool includeAllItems = false)
    {
        int widestText = 0;
        bool hasIcon = false;
        using var bitmap = new Bitmap(1, 1);
        using Graphics graphics = Graphics.FromImage(bitmap);

        if (includeAllItems)
        {
            foreach (object item in Items)
            {
                widestText = Math.Max(widestText, MeasureItemTextWidth(graphics, item, Font));
                if (!hasIcon &&
                    _itemIcons.TryGetValue(item, out var icon) &&
                    icon != null)
                {
                    hasIcon = true;
                }
            }
        }
        else
        {
            object? selectedItem = SelectedIndex >= 0 && SelectedIndex < Items.Count
                ? Items[SelectedIndex]
                : SelectedItem;
            if (selectedItem != null)
            {
                widestText = MeasureItemTextWidth(graphics, selectedItem, Font);
                hasIcon = _itemIcons.TryGetValue(selectedItem, out var icon) && icon != null;
            }
        }

        if (widestText <= 0)
        {
            string? fallbackText = !string.IsNullOrWhiteSpace(_lockedText)
                ? _lockedText
                : !string.IsNullOrWhiteSpace(Text)
                    ? Text
                    : PlaceholderText;
            if (!string.IsNullOrWhiteSpace(fallbackText))
            {
                widestText = MeasureRawTextWidth(graphics, fallbackText, Font);
            }
        }

        int horizontalChrome = IconPadding * 3 + ClosedSurfaceArrowReservedWidth + ClosedSurfaceTextSafetyPadding;
        if (hasIcon)
        {
            horizontalChrome += IconSize + IconPadding;
        }

        return Math.Max(1, horizontalChrome + widestText);
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
        HideCustomDropDown();
        DetachListChanged();
        AttachListChanged();
        RefreshIcons();      // rebuild icons for new data
        Invalidate();
    }

    protected override void OnDisplayMemberChanged(EventArgs e)
    {
        base.OnDisplayMemberChanged(e);
        HideCustomDropDown();
        Invalidate();
    }

    protected override void OnValueMemberChanged(EventArgs e)
    {
        base.OnValueMemberChanged(e);
        HideCustomDropDown();
        Invalidate();
    }

    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        base.OnSelectedIndexChanged(e);
        _customDropDownList?.SyncCommittedSelection(SelectedIndex);
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

        InvalidateCustomDropDownVisuals();
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

            var textLeft = rect.Left + IconPadding;

            if (hasIcon)
            {
                var iconRect = new Rectangle(rect.Left + IconPadding, rect.Top + lockedLayout.IconY, IconSize, IconSize);
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
            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
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
        const int leftButtonUp = 0x0202;
        const int leftButtonDblClick = 0x0203;
        const int keyDown = 0x0100;
        const int sysKeyDown = 0x0104;

        if (_isLocked)
        {
            if (m.Msg is leftButtonDown or leftButtonUp or leftButtonDblClick or keyDown or sysKeyDown)
                return; // Swallow input when locked
        }

        if (m.Msg is leftButtonDown or leftButtonDblClick)
        {
            Focus();
            if (_suppressNextCustomDropDownToggle)
            {
                _suppressNextCustomDropDownToggle = false;
                return;
            }

            ToggleCustomDropDown();
            return;
        }

        if (m.Msg == leftButtonUp)
        {
            return;
        }

        if ((m.Msg is keyDown or sysKeyDown) && TryHandleCustomDropDownKey((Keys)(nint)m.WParam, isSystemKey: m.Msg == sysKeyDown))
        {
            return;
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

        if (IsCustomDropDownOpen)
        {
            return;
        }

        BeginInvoke((MethodInvoker)(() =>
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (DroppedDown)
            {
                DroppedDown = false;
            }

            ShowCustomDropDown();
        }));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            HideCustomDropDown();
            DetachListChanged();
            DisposeItemIcons();
            if (_customDropDownHost != null)
            {
                _customDropDownHost.Closed -= CustomDropDownHost_Closed;
                _customDropDownHost.Dispose();
                _customDropDownHost = null;
            }

            if (_customDropDownList != null)
            {
                _customDropDownList.CommitRequested -= CustomDropDownList_CommitRequested;
                _customDropDownList.CancelRequested -= CustomDropDownList_CancelRequested;
                _customDropDownList.Dispose();
                _customDropDownList = null;
            }

            _customDropDownControlHost = null;
            if (!DesignMode)
            {
                ThemeManager.ThemeChanged -= _themeChangedHandler;
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

    private void HandleThemeChanged()
    {
        Invalidate();
        InvalidateCustomDropDownVisuals();
    }

    private void EnsureCustomDropDown()
    {
        if (_customDropDownHost != null && _customDropDownControlHost != null && _customDropDownList != null)
        {
            return;
        }

        _customDropDownList = new DropDownPopupList(this);
        _customDropDownList.CommitRequested += CustomDropDownList_CommitRequested;
        _customDropDownList.CancelRequested += CustomDropDownList_CancelRequested;

        _customDropDownControlHost = new ToolStripControlHost(_customDropDownList)
        {
            AutoSize = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        _customDropDownHost = new ToolStripDropDown
        {
            AutoClose = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            DropShadowEnabled = false
        };
        _customDropDownHost.Items.Add(_customDropDownControlHost);
        _customDropDownHost.Closed += CustomDropDownHost_Closed;
    }

    private void ToggleCustomDropDown()
    {
        if (IsCustomDropDownOpen)
        {
            HideCustomDropDown(focusOwner: false);
            return;
        }

        ShowCustomDropDown();
    }

    private void ShowCustomDropDown()
    {
        if (_isLocked || !IsHandleCreated || ItemHeight <= 0 || Items.Count == 0 || IsCustomDropDownOpen)
        {
            return;
        }

        EnsureCustomDropDown();
        if (_customDropDownList == null || _customDropDownControlHost == null || _customDropDownHost == null)
        {
            return;
        }

        int preferredHeight = _customDropDownList.GetPreferredPopupHeight(MaxDropDownItems);
        int minimumHeight = _customDropDownList.GetMinimumPopupHeight();
        if (preferredHeight <= 0)
        {
            return;
        }

        Rectangle workingArea = Screen.FromControl(this).WorkingArea;
        Point screenOrigin = PointToScreen(Point.Empty);
        int belowSpace = Math.Max(0, workingArea.Bottom - (screenOrigin.Y + Height));
        int aboveSpace = Math.Max(0, screenOrigin.Y - workingArea.Top);

        bool showAbove = belowSpace < preferredHeight && aboveSpace > belowSpace;
        int availableHeight = showAbove ? aboveSpace : belowSpace;
        if (availableHeight < minimumHeight)
        {
            showAbove = aboveSpace > belowSpace;
            availableHeight = Math.Max(aboveSpace, belowSpace);
        }

        int popupHeight = Math.Max(minimumHeight, Math.Min(preferredHeight, Math.Max(availableHeight, minimumHeight)));

        int popupWidth = _customDropDownList.GetPreferredPopupWidth(Width, MaxDropDownItems, popupHeight);
        popupWidth = Math.Min(popupWidth, workingArea.Width);
        _customDropDownList.PrepareForOpen(SelectedIndex, popupWidth, MaxDropDownItems, popupHeight);
        _customDropDownControlHost.Size = _customDropDownList.Size;

        int popupScreenLeft = Math.Max(workingArea.Left, Math.Min(screenOrigin.X, workingArea.Right - _customDropDownList.Width));
        int popupOffsetX = popupScreenLeft - screenOrigin.X;
        Point location = new(popupOffsetX, showAbove ? -_customDropDownList.Height : Height);
        _customDropDownHost.Show(this, location);

        BeginInvoke((MethodInvoker)(() =>
        {
            if (_customDropDownList is { IsDisposed: false })
            {
                _customDropDownList.Focus();
            }
        }));
        Invalidate();
    }

    private void HideCustomDropDown(bool focusOwner = false)
    {
        if (_customDropDownHost is not { Visible: true })
        {
            return;
        }

        _focusOwnerAfterCustomDropDownClose = focusOwner;
        _customDropDownHost.Close();
    }

    private void InvalidateCustomDropDownVisuals()
    {
        if (_customDropDownList == null)
        {
            return;
        }

        _customDropDownList.RefreshFromOwner();
        _customDropDownList.Invalidate();
    }

    private bool TryHandleCustomDropDownKey(Keys keyData, bool isSystemKey)
    {
        Keys key = keyData & Keys.KeyCode;
        bool altPressed = isSystemKey || (ModifierKeys & Keys.Alt) == Keys.Alt;

        if (key == Keys.F4 || (altPressed && key is Keys.Down or Keys.Up))
        {
            if (IsCustomDropDownOpen)
            {
                HideCustomDropDown(focusOwner: true);
            }
            else
            {
                ShowCustomDropDown();
            }

            return true;
        }

        return false;
    }

    private void CustomDropDownList_CommitRequested(object? sender, int selectedIndex)
    {
        HideCustomDropDown(focusOwner: true);
        if (selectedIndex >= -1 && selectedIndex < Items.Count)
        {
            SelectedIndex = selectedIndex;
        }
    }

    private void CustomDropDownList_CancelRequested(object? sender, EventArgs e)
    {
        HideCustomDropDown(focusOwner: true);
    }

    private void CustomDropDownHost_Closed(object? sender, ToolStripDropDownClosedEventArgs e)
    {
        _customDropDownList?.EndSession();

        if (e.CloseReason == ToolStripDropDownCloseReason.AppClicked &&
            RectangleToScreen(ClientRectangle).Contains(Cursor.Position))
        {
            _suppressNextCustomDropDownToggle = true;
            BeginInvoke((MethodInvoker)(() => _suppressNextCustomDropDownToggle = false));
        }

        if (_focusOwnerAfterCustomDropDownClose && !IsDisposed && IsHandleCreated)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                if (!IsDisposed)
                {
                    Focus();
                }
            }));
        }

        _focusOwnerAfterCustomDropDownClose = false;
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

    private static int MeasureRawTextWidth(Graphics graphics, string text, Font font)
        => TextRenderer.MeasureText(graphics, text, font, Size.Empty, TextFormatFlags.NoPadding).Width;

    private int MeasureItemTextWidth(Graphics graphics, object item, Font font)
        => MeasureRawTextWidth(graphics, GetItemText(item) ?? string.Empty, font);

    #endregion

    #region Custom Drop Down

    private sealed class DropDownPopupList : Control
    {
        private const int BorderThickness = 1;
        private const int LogicalScrollBarWidth = 10;
        private const int LogicalTrackGap = 4;
        private const int LogicalThumbMinHeight = 10;

        private readonly MidsDropDownList _owner;

        private int _topIndex;
        private int _activeIndex = -1;
        private int _committedIndex = -1;
        private int _visibleItemCount;
        private bool _needsScrollbar;

        private Rectangle _contentBounds;
        private Rectangle _scrollbarBounds;
        private Rectangle _upArrowRect;
        private Rectangle _downArrowRect;
        private Rectangle _trackBounds;
        private Rectangle _thumbRect;

        private bool _hoveringThumb;
        private bool _hoveringUpArrow;
        private bool _hoveringDownArrow;
        private bool _draggingThumb;
        private int _dragStartY;

        internal event EventHandler<int>? CommitRequested;
        internal event EventHandler? CancelRequested;

        private float DpiScale => DeviceDpi / 96f;
        private int ScrollBarWidth => Math.Max(8, (int)Math.Round(LogicalScrollBarWidth * DpiScale));
        private int TrackGap => Math.Max(1, (int)Math.Round(LogicalTrackGap * DpiScale));
        private int ThumbMinHeight => Math.Max(6, (int)Math.Round(LogicalThumbMinHeight * DpiScale));
        private int RowHeight => Math.Max(1, _owner.ItemHeight);
        private int ItemCount => _owner.Items.Count;
        private int MaxTopIndex => Math.Max(0, ItemCount - _visibleItemCount);

        internal DropDownPopupList(MidsDropDownList owner)
        {
            _owner = owner;

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);

            TabStop = true;
        }

        internal int GetMinimumPopupHeight()
            => BorderThickness * 2 + RowHeight;

        internal int GetPreferredPopupHeight(int maxVisibleItems)
        {
            if (ItemCount <= 0)
            {
                return 0;
            }

            int visibleItems = Math.Max(1, Math.Min(ItemCount, Math.Max(1, maxVisibleItems)));
            return BorderThickness * 2 + visibleItems * RowHeight;
        }

        internal int GetPreferredPopupWidth(int ownerWidth, int maxVisibleItems, int maxPopupHeight)
        {
            int width = Math.Max(1, ownerWidth);
            if (ItemCount <= 0 || !_owner.IsHandleCreated)
            {
                return width;
            }

            int allowedRowsByHeight = Math.Max(1, (Math.Max(0, maxPopupHeight - BorderThickness * 2)) / RowHeight);
            int visibleItems = Math.Max(1, Math.Min(ItemCount, Math.Min(Math.Max(1, maxVisibleItems), allowedRowsByHeight)));
            bool needsScrollbar = ItemCount > visibleItems;

            int widestText = 0;
            using Graphics g = _owner.CreateGraphics();
            foreach (object item in _owner.Items)
            {
                string text = _owner.GetItemText(item) ?? string.Empty;
                Size size = TextRenderer.MeasureText(g, text, _owner.Font, Size.Empty, TextFormatFlags.NoPadding);
                widestText = Math.Max(widestText, size.Width);
            }

            int horizontalChrome = BorderThickness * 2 + IconPadding * 3 + _owner.IconSize + PopupTextSafetyPadding;
            if (needsScrollbar)
            {
                horizontalChrome += ScrollBarWidth;
            }

            return Math.Max(width, horizontalChrome + widestText);
        }

        internal void PrepareForOpen(int selectedIndex, int width, int maxVisibleItems, int maxPopupHeight)
        {
            if (ItemCount <= 0)
            {
                return;
            }

            int allowedRowsByHeight = Math.Max(1, (Math.Max(0, maxPopupHeight - BorderThickness * 2)) / RowHeight);
            _visibleItemCount = Math.Max(1, Math.Min(ItemCount, Math.Min(Math.Max(1, maxVisibleItems), allowedRowsByHeight)));
            _committedIndex = selectedIndex;
            _activeIndex = selectedIndex >= 0 && selectedIndex < ItemCount ? selectedIndex : 0;

            EnsureActiveIndexVisible();

            Size = new Size(Math.Max(1, width), BorderThickness * 2 + _visibleItemCount * RowHeight);
            RecalculateLayout();
            Invalidate();
        }

        internal void RefreshFromOwner()
        {
            if (_visibleItemCount <= 0)
            {
                return;
            }

            if (ItemCount <= 0)
            {
                _topIndex = 0;
                _activeIndex = -1;
                _committedIndex = -1;
                _visibleItemCount = 0;
                RecalculateLayout();
                return;
            }

            int visibleRows = Math.Max(1, (Math.Max(0, Height - BorderThickness * 2)) / RowHeight);
            _visibleItemCount = Math.Max(1, Math.Min(ItemCount, visibleRows));
            _committedIndex = Math.Min(_committedIndex, ItemCount - 1);
            _activeIndex = _activeIndex < 0 ? 0 : Math.Min(_activeIndex, ItemCount - 1);
            EnsureActiveIndexVisible();
            RecalculateLayout();
        }

        internal void SyncCommittedSelection(int selectedIndex)
        {
            if (ItemCount <= 0)
            {
                _committedIndex = -1;
                _activeIndex = -1;
                _topIndex = 0;
                Invalidate();
                return;
            }

            _committedIndex = selectedIndex;
            _activeIndex = selectedIndex >= 0 && selectedIndex < ItemCount ? selectedIndex : _activeIndex;
            if (_activeIndex < 0)
            {
                _activeIndex = 0;
            }

            EnsureActiveIndexVisible();
            Invalidate();
        }

        internal void EndSession()
        {
            _draggingThumb = false;
            _hoveringThumb = false;
            _hoveringUpArrow = false;
            _hoveringDownArrow = false;
            Capture = false;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            return key is Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End or Keys.Enter or Keys.Escape or Keys.Tab
                || base.IsInputKey(keyData);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = _owner.CurrentTheme;
            using (var backgroundBrush = new SolidBrush(theme.DropDownBackColor))
            {
                g.FillRectangle(backgroundBrush, ClientRectangle);
            }

            DrawItems(g, theme);

            if (_needsScrollbar)
            {
                DrawScrollbar(g);
            }

            using var borderPen = new Pen(theme.Border);
            g.DrawRectangle(borderPen, 0, 0, Math.Max(0, Width - 1), Math.Max(0, Height - 1));
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!_needsScrollbar)
            {
                return;
            }

            int lines = SystemInformation.MouseWheelScrollLines;
            if (lines <= 0)
            {
                lines = 3;
            }

            int direction = Math.Sign(-e.Delta);
            if (direction != 0)
            {
                ScrollBy(direction * lines);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            if (_needsScrollbar && _thumbRect.Contains(e.Location))
            {
                _draggingThumb = true;
                _dragStartY = e.Y - _thumbRect.Y;
                Capture = true;
                return;
            }

            if (_needsScrollbar && _upArrowRect.Contains(e.Location))
            {
                ScrollBy(-1);
                return;
            }

            if (_needsScrollbar && _downArrowRect.Contains(e.Location))
            {
                ScrollBy(1);
                return;
            }

            if (_needsScrollbar && _trackBounds.Contains(e.Location))
            {
                if (e.Y < _thumbRect.Top)
                {
                    ScrollBy(-_visibleItemCount);
                }
                else if (e.Y > _thumbRect.Bottom)
                {
                    ScrollBy(_visibleItemCount);
                }

                return;
            }

            int itemIndex = HitTestItemIndex(e.Location);
            if (itemIndex >= 0)
            {
                SetActiveIndex(itemIndex);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bool oldHoverThumb = _hoveringThumb;
            bool oldHoverUp = _hoveringUpArrow;
            bool oldHoverDown = _hoveringDownArrow;

            _hoveringThumb = _needsScrollbar && _thumbRect.Contains(e.Location);
            _hoveringUpArrow = _needsScrollbar && _upArrowRect.Contains(e.Location);
            _hoveringDownArrow = _needsScrollbar && _downArrowRect.Contains(e.Location);

            if (_draggingThumb)
            {
                DragThumbTo(e.Y);
                return;
            }

            int itemIndex = HitTestItemIndex(e.Location);
            if (itemIndex >= 0 && itemIndex != _activeIndex)
            {
                SetActiveIndex(itemIndex);
                return;
            }

            if (oldHoverThumb != _hoveringThumb || oldHoverUp != _hoveringUpArrow || oldHoverDown != _hoveringDownArrow)
            {
                Invalidate(_scrollbarBounds);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_draggingThumb)
            {
                _draggingThumb = false;
                Capture = false;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!_hoveringThumb && !_hoveringUpArrow && !_hoveringDownArrow)
            {
                return;
            }

            _hoveringThumb = false;
            _hoveringUpArrow = false;
            _hoveringDownArrow = false;
            if (!_scrollbarBounds.IsEmpty)
            {
                Invalidate(_scrollbarBounds);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            int itemIndex = HitTestItemIndex(e.Location);
            if (itemIndex >= 0)
            {
                CommitRequested?.Invoke(this, itemIndex);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Up:
                    MoveActiveIndex(-1);
                    e.Handled = true;
                    break;
                case Keys.Down:
                    MoveActiveIndex(1);
                    e.Handled = true;
                    break;
                case Keys.PageUp:
                    MoveActiveIndex(-Math.Max(1, _visibleItemCount));
                    e.Handled = true;
                    break;
                case Keys.PageDown:
                    MoveActiveIndex(Math.Max(1, _visibleItemCount));
                    e.Handled = true;
                    break;
                case Keys.Home:
                    SetActiveIndex(0);
                    e.Handled = true;
                    break;
                case Keys.End:
                    SetActiveIndex(Math.Max(0, ItemCount - 1));
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    if (_activeIndex >= 0)
                    {
                        CommitRequested?.Invoke(this, _activeIndex);
                    }

                    e.Handled = true;
                    break;
                case Keys.Escape:
                    CancelRequested?.Invoke(this, EventArgs.Empty);
                    e.Handled = true;
                    break;
                case Keys.Tab:
                    if (_activeIndex >= 0)
                    {
                        CommitRequested?.Invoke(this, _activeIndex);
                    }
                    else
                    {
                        CancelRequested?.Invoke(this, EventArgs.Empty);
                    }

                    e.Handled = true;
                    break;
            }
        }

        private void DrawItems(Graphics g, DropDownListTheme theme)
        {
            if (ItemCount <= 0 || _visibleItemCount <= 0)
            {
                return;
            }

            int lastVisibleIndex = Math.Min(ItemCount, _topIndex + _visibleItemCount);
            for (int itemIndex = _topIndex; itemIndex < lastVisibleIndex; itemIndex++)
            {
                int slot = itemIndex - _topIndex;
                Rectangle itemRect = new(
                    _contentBounds.Left,
                    _contentBounds.Top + slot * RowHeight,
                    _contentBounds.Width,
                    RowHeight);

                bool isActive = itemIndex == _activeIndex;
                Color backColor = isActive ? theme.DropDownSelectionBackColor : theme.DropDownBackColor;
                Color textColor = isActive ? theme.DropDownSelectionForeColor : theme.ForeColor;

                using (var itemBrush = new SolidBrush(backColor))
                {
                    g.FillRectangle(itemBrush, itemRect);
                }

                object item = _owner.Items[itemIndex];
                Rectangle iconRect = new Rectangle(
                    itemRect.Left + IconPadding,
                    itemRect.Top + (itemRect.Height - _owner.IconSize) / 2,
                    _owner.IconSize,
                    _owner.IconSize);

                Rectangle textRect = new Rectangle(
                    iconRect.Right + IconPadding,
                    itemRect.Top,
                    Math.Max(0, itemRect.Right - iconRect.Right - IconPadding * 2),
                    itemRect.Height);

                if (_owner._itemIcons.TryGetValue(item, out var icon) && icon != null)
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    DrawIconIfValid(g, icon, iconRect);
                }

                TextRenderer.DrawText(
                    g,
                    _owner.GetItemText(item) ?? string.Empty,
                    _owner.Font,
                    textRect,
                    textColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                if (isActive)
                {
                    using var focusPen = new Pen(theme.FocusBorder);
                    var focusRect = itemRect;
                    focusRect.Width = Math.Max(0, focusRect.Width - 1);
                    focusRect.Height = Math.Max(0, focusRect.Height - 1);
                    g.DrawRectangle(focusPen, focusRect);
                }
            }
        }

        private void DrawScrollbar(Graphics g)
        {
            if (_scrollbarBounds.IsEmpty)
            {
                return;
            }

            var theme = _owner.CurrentScrollTheme;
            int centerX = _scrollbarBounds.Left + _scrollbarBounds.Width / 2;
            float arrowMargin = _scrollbarBounds.Width * 0.08f;

            using (var arrowBrush = new SolidBrush((_hoveringUpArrow || _hoveringDownArrow) ? theme.Hover : theme.Bar))
            {
                var upTriangle = new PointF[]
                {
                    new(_upArrowRect.Left + _upArrowRect.Width / 2f, _upArrowRect.Top + arrowMargin),
                    new(_upArrowRect.Left + arrowMargin, _upArrowRect.Bottom - arrowMargin),
                    new(_upArrowRect.Right - arrowMargin, _upArrowRect.Bottom - arrowMargin)
                };
                g.FillPolygon(arrowBrush, upTriangle);

                var downTriangle = new PointF[]
                {
                    new(_downArrowRect.Left + _downArrowRect.Width / 2f, _downArrowRect.Bottom - arrowMargin),
                    new(_downArrowRect.Left + arrowMargin, _downArrowRect.Top + arrowMargin),
                    new(_downArrowRect.Right - arrowMargin, _downArrowRect.Top + arrowMargin)
                };
                g.FillPolygon(arrowBrush, downTriangle);
            }

            using (var trackPen = new Pen(theme.Track, Math.Max(1, (int)Math.Round(2 * DpiScale))))
            {
                trackPen.StartCap = LineCap.Round;
                trackPen.EndCap = LineCap.Round;
                g.DrawLine(trackPen, centerX, _trackBounds.Top, centerX, _trackBounds.Bottom);
            }

            using var thumbBrush = new SolidBrush(_hoveringThumb ? theme.Hover : theme.Bar);
            g.FillRectangle(thumbBrush, _thumbRect);
        }

        private void RecalculateLayout()
        {
            _needsScrollbar = ItemCount > _visibleItemCount;
            _contentBounds = new Rectangle(
                BorderThickness,
                BorderThickness,
                Math.Max(0, Width - BorderThickness * 2 - (_needsScrollbar ? ScrollBarWidth : 0)),
                Math.Max(0, Height - BorderThickness * 2));

            if (!_needsScrollbar)
            {
                _scrollbarBounds = Rectangle.Empty;
                _upArrowRect = Rectangle.Empty;
                _downArrowRect = Rectangle.Empty;
                _trackBounds = Rectangle.Empty;
                _thumbRect = Rectangle.Empty;
                return;
            }

            int scrollBarX = Width - BorderThickness - ScrollBarWidth;
            _scrollbarBounds = new Rectangle(scrollBarX, BorderThickness, ScrollBarWidth, Math.Max(0, Height - BorderThickness * 2));
            _upArrowRect = new Rectangle(scrollBarX, BorderThickness, ScrollBarWidth, ScrollBarWidth);
            _downArrowRect = new Rectangle(scrollBarX, Height - BorderThickness - ScrollBarWidth, ScrollBarWidth, ScrollBarWidth);
            _trackBounds = Rectangle.FromLTRB(
                scrollBarX,
                _upArrowRect.Bottom + TrackGap,
                scrollBarX + ScrollBarWidth,
                _downArrowRect.Top - TrackGap);

            int trackHeight = Math.Max(0, _trackBounds.Height);
            int thumbHeight = Math.Max(ThumbMinHeight, (int)Math.Round((double)_visibleItemCount / Math.Max(1, ItemCount) * trackHeight));
            thumbHeight = Math.Min(trackHeight, thumbHeight);

            int available = Math.Max(0, trackHeight - thumbHeight);
            int thumbY = _trackBounds.Top;
            if (available > 0 && MaxTopIndex > 0)
            {
                double ratio = (double)_topIndex / MaxTopIndex;
                thumbY = _trackBounds.Top + (int)Math.Round(available * ratio);
            }

            _thumbRect = new Rectangle(scrollBarX, thumbY, ScrollBarWidth, thumbHeight);
        }

        private int HitTestItemIndex(Point location)
        {
            if (!_contentBounds.Contains(location) || _visibleItemCount <= 0)
            {
                return -1;
            }

            int row = (location.Y - _contentBounds.Top) / RowHeight;
            if (row < 0 || row >= _visibleItemCount)
            {
                return -1;
            }

            int itemIndex = _topIndex + row;
            return itemIndex >= 0 && itemIndex < ItemCount ? itemIndex : -1;
        }

        private void MoveActiveIndex(int delta)
        {
            if (ItemCount <= 0)
            {
                return;
            }

            int startIndex = _activeIndex >= 0 ? _activeIndex : 0;
            SetActiveIndex(Math.Max(0, Math.Min(ItemCount - 1, startIndex + delta)));
        }

        private void SetActiveIndex(int index)
        {
            if (index < 0 || index >= ItemCount || index == _activeIndex)
            {
                return;
            }

            _activeIndex = index;
            EnsureActiveIndexVisible();
            Invalidate();
        }

        private void EnsureActiveIndexVisible()
        {
            if (_activeIndex < 0 || _visibleItemCount <= 0)
            {
                _topIndex = 0;
                return;
            }

            if (_activeIndex < _topIndex)
            {
                _topIndex = _activeIndex;
            }
            else if (_activeIndex >= _topIndex + _visibleItemCount)
            {
                _topIndex = _activeIndex - _visibleItemCount + 1;
            }

            _topIndex = Math.Max(0, Math.Min(_topIndex, MaxTopIndex));
            RecalculateLayout();
        }

        private void ScrollBy(int deltaRows)
        {
            if (!_needsScrollbar)
            {
                return;
            }

            int nextTopIndex = Math.Max(0, Math.Min(MaxTopIndex, _topIndex + deltaRows));
            if (nextTopIndex == _topIndex)
            {
                return;
            }

            _topIndex = nextTopIndex;
            RecalculateLayout();
            Invalidate();
        }

        private void DragThumbTo(int mouseY)
        {
            if (_trackBounds.IsEmpty || _thumbRect.IsEmpty)
            {
                return;
            }

            int available = Math.Max(0, _trackBounds.Height - _thumbRect.Height);
            if (available <= 0 || MaxTopIndex <= 0)
            {
                return;
            }

            int newThumbY = mouseY - _dragStartY;
            newThumbY = Math.Max(_trackBounds.Top, Math.Min(newThumbY, _trackBounds.Top + available));
            double ratio = (double)(newThumbY - _trackBounds.Top) / available;
            _topIndex = (int)Math.Round(ratio * MaxTopIndex);
            _topIndex = Math.Max(0, Math.Min(_topIndex, MaxTopIndex));
            RecalculateLayout();
            Invalidate();
        }
    }

    #endregion
}

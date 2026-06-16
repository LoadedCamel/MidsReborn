using System.ComponentModel;
using System.Drawing.Imaging;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Renderer;

namespace Mids_Reborn.UI.Controls.Test.EnhPicker;

[ToolboxItem(true)]
[DesignerCategory("Code")]
[DefaultEvent(nameof(EnhancementPicked))]
public sealed class I9Picker : Control
{
    public struct EnhUniqueStatus
    {
        public bool InMain;
        public bool InAlternate;
    }

    public enum SetPickerStage
    {
        SetFamilyGrid,
        SetVariantGrid,
        SetEnhancementGrid
    }

    public delegate void EnhancementPickedEventHandler(I9Slot e);
    public delegate void EnhancementSelectionCancelledEventHandler();
    public delegate void HoverEnhancementEventHandler(int e, EnhUniqueStatus? enhUniqueStatus);
    public delegate void HoverSetEventHandler(int e);
    public delegate void MovedEventHandler(Rectangle oldBounds, Rectangle newBounds);

    private const int IconSize = 64;
    private const int IconSpacing = 8;
    private const int PaddingOuter = 10;
    private const int HeaderBoxHeight = IconSize / 2;
    private const int InfoBoxHeight = IconSize - 4;
    private const int FooterBoxHeight = IconSize + 16;
    private const int CornerRadius = 8;
    private const int TypeIconCount = 5;
    private const int EnhGridCols = 4;
    private const int EnhGridRows = 5;
    private const int MaxVisibleGradeIcons = 5;
    private const int RailExtraWidth = 8;
    private const int IconInset = 2;
    private const int RailScrollbarWidth = 14;
    private const int RailScrollbarInset = 3;
    private const int RailScrollbarThumbMinHeight = 24;

    private readonly I9PickerLayoutOptions _layoutOptions = new()
    {
        Margin = PaddingOuter,
        Gap = IconSpacing,
        HeaderHeight = HeaderBoxHeight,
        InfoHeight = InfoBoxHeight,
        TypeButtonCount = TypeIconCount,
        LastTypeButtonExtraSpacing = 2,
        GridColumns = EnhGridCols,
        GridRows = EnhGridRows,
        IconSize = IconSize,
        MaxVisibleRailItems = MaxVisibleGradeIcons,
        RailExtraWidth = RailExtraWidth,
        RailSlotGap = IconSpacing,
        RailScrollbarWidth = RailScrollbarWidth,
        FooterHeight = FooterBoxHeight,
        LevelBoostMinWidth = 100,
        BoostButtonWidth = 30,
        BoostButtonHeight = 22,
        BoostButtonGap = 6,
        BoostButtonBottomInset = 6
    };

    private I9PickerLayout _layout = I9PickerLayout.Empty;
    private EnhSelectorModel _model = new();

    private string? _hoverTitle;
    private string? _hoverInfo;
    private string? _hoverText;

    private int _hoverEnhIndex = -1;
    private int _hoverSetIndex = -1;
    private int _hoverHeaderIndex = -1;

    private int _powerId = -1;
    private int[] _normalEnhs = [];
    private int[] _inventionEnhs = [];
    private Enums.eType _lastTab = Enums.eType.Normal;
    private Enums.eEnhGrade _lastGrade = Enums.eEnhGrade.SingleO;
    private int _lastSpecial = 1;
    private int _lastSet;
    private int _initialEnhancementId = -1;
    private int[] _slotted = [];

    private readonly List<(Rectangle Bounds, int Index)> _enhancementRects = [];
    private readonly List<(Rectangle Bounds, int Index)> _gradeRects = [];
    private readonly List<(Rectangle Bounds, int Index)> _headerRects = [];

    private Rectangle _lvlMinusRect = Rectangle.Empty;
    private Rectangle _lvlPlusRect = Rectangle.Empty;
    private Rectangle _railScrollbarBounds = Rectangle.Empty;
    private Rectangle _railScrollbarTrackBounds = Rectangle.Empty;
    private Rectangle _railScrollbarThumbRect = Rectangle.Empty;

    private int _scrollOffset;
    private bool _railDraggingThumb;
    private int _railDragStartY;
    private bool _railHoveringThumb;

    private bool _minusHovered;
    private bool _plusHovered;
    private bool _minusPressed;
    private bool _plusPressed;

    private Rectangle _oldBounds;
    private Size _autoPreferredSize;
    private float _uiScale = 1f;

    public I9Picker()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.Selectable |
            ControlStyles.SupportsTransparentBackColor,
            true);

        BackColor = Color.Transparent;
        ForeColor = I9PickerVisualPalette.Default.Text;
        Font = new Font("Segoe UI", 9f, FontStyle.Regular);

        var preferredSize = I9PickerLayout.CalculatePreferredSize(CreateResolvedLayoutOptions());
        _autoPreferredSize = preferredSize;
        Size = preferredSize;
        MinimumSize = preferredSize;
        TabStop = true;
        _oldBounds = Bounds;
    }

    public event EnhancementPickedEventHandler? EnhancementPicked;
    public event EnhancementSelectionCancelledEventHandler? EnhancementSelectionCancelled;
    public event HoverEnhancementEventHandler? HoverEnhancement;
    public event HoverSetEventHandler? HoverSet;
    public event MovedEventHandler? Moved;

    [Browsable(false)]
    public EnhSelectorState View => _model.View;

    [Browsable(false)]
    public int LastLevel { get; internal set; }

    [Browsable(false)]
    public Size BasePreferredSize => I9PickerLayout.CalculatePreferredSize(_layoutOptions.Clone());

    [Category("Mids Layout")]
    [DefaultValue(1f)]
    public float UiScale
    {
        get => _uiScale;
        set
        {
            var next = Math.Clamp(value, 0.5f, 1f);
            if (Math.Abs(_uiScale - next) < 0.001f)
                return;

            _uiScale = next;
            RefreshScaledSize();
        }
    }

    [Category("Mids Layout")]
    [DefaultValue(64)]
    public int PickerIconSize
    {
        get => _layoutOptions.IconSize;
        set
        {
            var next = Math.Clamp(value, 32, 64);
            if (_layoutOptions.IconSize == next)
                return;

            _layoutOptions.IconSize = next;
            RefreshScaledSize();
        }
    }

    [Category("Mids Layout")]
    [DefaultValue(86)]
    public int RailWidth
    {
        get => _layoutOptions.RailExtraWidth + _layoutOptions.RailScrollbarWidth + _layoutOptions.IconSize;
        set
        {
            var next = Math.Clamp(value, 54, 180);
            var nextExtra = Math.Max(0, next - _layoutOptions.IconSize - _layoutOptions.RailScrollbarWidth);
            if (_layoutOptions.RailExtraWidth == nextExtra)
                return;

            _layoutOptions.RailExtraWidth = nextExtra;
            RefreshScaledSize();
        }
    }

    [Category("Mids Layout")]
    [DefaultValue(80)]
    public int FooterHeight
    {
        get => _layoutOptions.FooterHeight;
        set
        {
            var next = Math.Clamp(value, 64, 160);
            if (_layoutOptions.FooterHeight == next)
                return;

            _layoutOptions.FooterHeight = next;
            RefreshScaledSize();
        }
    }

    private bool InDesigner => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    private static I9PickerVisualPalette Palette => I9PickerVisualPalette.Default;

    public void SetData(int iPower, I9Slot iSlot, int[] slotted)
    {
        if (InDesigner)
            return;

        var validPowerId = IsValidPowerId(iPower) ? iPower : -1;
        var initialEnhancementId = IsValidEnhancementId(iSlot.Enh) ? iSlot.Enh : -1;

        _powerId = validPowerId;
        _hoverTitle = validPowerId > -1
            ? $"Enhancing: {DatabaseAPI.Database.Power[validPowerId]?.DisplayName}"
            : "Enhancing";

        _slotted = slotted?.Where(IsValidEnhancementId).ToArray() ?? [];
        _initialEnhancementId = initialEnhancementId;

        _model = new EnhSelectorModel();

        _normalEnhs = GetValidEnhancements(validPowerId, Enums.eType.Normal).ToArray();
        _inventionEnhs = GetValidEnhancements(validPowerId, Enums.eType.InventO).ToArray();

        _model.SetTypes = GetValidSetTypes(validPowerId);
        _model.NoGrades =
        [
            (int)Enums.eEnhGrade.TrainingO,
            (int)Enums.eEnhGrade.DualO,
            (int)Enums.eEnhGrade.SingleO
        ];
        _model.SpecialTypes = GetOrderedSpecialTypes();

        _model.Initial.GradeId = _lastGrade;
        _model.Initial.RelLevel = Enums.eEnhRelative.Even;
        _model.Initial.SpecialId = _lastSpecial > 0 ? _lastSpecial : 1;

        if (initialEnhancementId > -1)
        {
            var enh = DatabaseAPI.Database.Enhancements[initialEnhancementId];

            _hoverText = enh.Desc;

            _model.Initial.TabId = enh.TypeID;
            _model.Initial.GradeId = iSlot.Grade;
            _model.Initial.RelLevel = iSlot.RelativeLevel;
            _model.Initial.IoLevel = iSlot.IOLevel + 1;
            _model.Initial.SpecialId = enh.SubTypeID;
            _model.Initial.RelLevel = ValidateRelativeLevel(_model.Initial.RelLevel, _model.Initial.TabId, initialEnhancementId);

            if (enh.TypeID == Enums.eType.SetO &&
                enh.nIDSet >= 0 &&
                enh.nIDSet < DatabaseAPI.Database.EnhancementSets.Count)
            {
                var setType = DatabaseAPI.Database.EnhancementSets[enh.nIDSet].SetType;

                _model.Initial.SetTypeId = SetTypeToId(setType);
                _model.SetIds = GetSets(setType);
                _model.Initial.SetId = enh.nIDSet;
                _model.Initial.SetVariant = DatabaseAPI.GetSetVariantKind(initialEnhancementId);
                _model.Initial.SetStage = SetPickerStage.SetEnhancementGrid;
            }
        }
        else
        {
            _hoverText = string.Empty;
            ApplyLastUsedStateForEmptySlot();
        }

        _model.View = new EnhSelectorState(_model.Initial);

        if (_model.View.TabId == Enums.eType.None)
        {
            _model.View.TabId = _lastTab;
        }

        SetActiveEnhancements(validPowerId, initialEnhancementId, _normalEnhs, _inventionEnhs);

        _scrollOffset = 0;
        _hoverEnhIndex = -1;
        _hoverSetIndex = -1;
        _hoverHeaderIndex = -1;

        Invalidate();
    }

    public int CheckAndReturnIoLevel(int enhancementId = -1)
    {
        var ioMax = 50;
        var ioMin = 10;
        var fixedLevel = _model.View.IoLevel;
        var resolvedEnhancementId = enhancementId;

        if (!IsValidEnhancementId(resolvedEnhancementId) &&
            _model.View.PickerId > -1 &&
            _model.View.PickerId < _model.EnhancementIds.Length)
        {
            resolvedEnhancementId = _model.EnhancementIds[_model.View.PickerId];
        }

        switch (_model.View.TabId)
        {
            case Enums.eType.InventO:
                if (TryGetEnhancementDisplayLevelRange(resolvedEnhancementId, out var enhIoMin, out var enhIoMax))
                {
                    ioMax = enhIoMax;
                    ioMin = enhIoMin;
                }

                break;

            case Enums.eType.SetO:
                if (TryGetEnhancementDisplayLevelRange(resolvedEnhancementId, out var setIoMin, out var setIoMax))
                {
                    ioMax = setIoMax;
                    ioMin = setIoMin;
                }
                else if (_model.View.SetId > -1 && _model.View.SetTypeId > -1)
                {
                    var set = DatabaseAPI.Database.EnhancementSets.ElementAtOrDefault(_model.View.SetId);
                    if (set is not null)
                    {
                        ioMax = set.LevelMax + 1;
                        ioMin = set.LevelMin + 1;
                    }
                }

                break;
        }

        fixedLevel = Math.Clamp(fixedLevel, ioMin, ioMax);

        if (_model.View.TabId == Enums.eType.InventO)
        {
            if (ioMax > 50)
                ioMax = 50;

            fixedLevel = Enhancement.GranularLevelZb(fixedLevel - 1, ioMin - 1, ioMax - 1) + 1;
        }

        return fixedLevel;
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        if (BackColor.A == 255)
        {
            using var brush = new SolidBrush(BackColor);
            pevent.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        _layout = I9PickerLayout.Calculate(ClientSize, CreateResolvedLayoutOptions());

        _enhancementRects.Clear();
        _gradeRects.Clear();
        _headerRects.Clear();
        _lvlMinusRect = Rectangle.Empty;
        _lvlPlusRect = Rectangle.Empty;
        _railScrollbarBounds = Rectangle.Empty;
        _railScrollbarTrackBounds = Rectangle.Empty;
        _railScrollbarThumbRect = Rectangle.Empty;

        var g = e.Graphics;

        MidsFrameRenderer.ApplyDefaultQuality(g);

        DrawOuterFrame(g);
        DrawHeaderBox(g);
        DrawTopInfoBox(g);
        DrawTypeStrip(g);
        DrawTypeButtons(g);
        DrawGridPanel(g);
        DrawRailPanel(g);
        DrawInfoPanel(g);
        DrawLevelBoostPanel(g);

        if (Focused)
        {
            var focus = Rectangle.Inflate(_layout.OuterBounds, -4, -4);
            ControlPaint.DrawFocusRectangle(g, focus);
        }
    }

    protected override void OnMove(EventArgs e)
    {
        base.OnMove(e);

        if (_oldBounds != Bounds)
        {
            var old = _oldBounds;
            _oldBounds = Bounds;
            Moved?.Invoke(old, Bounds);
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        if (ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)
            return;

        using var path = MidsFrameRenderer.CreateRoundedRectanglePath(ClientRectangle, 9);
        Region?.Dispose();
        Region = new Region(path);
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        RefreshScaledSize();
    }

    protected override bool IsInputKey(Keys keyData)
    {
        return keyData is Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.Add or Keys.Subtract or Keys.Oemplus or Keys.OemMinus
               || base.IsInputKey(keyData);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);

        var itemCount = GetVisibleRailItemCount();
        if (itemCount <= MaxVisibleGradeIcons)
            return;

        var step = _layout.IconSize + _layout.RailSlotGap;
        _scrollOffset -= Math.Sign(e.Delta) * step;
        _scrollOffset = ClampRailScrollOffset(_scrollOffset, itemCount, step);

        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        var pt = e.Location;
        var changed = false;

        if (_railDraggingThumb)
        {
            DragRailScrollbar(e.Y);
            return;
        }

        var oldMinusHovered = _minusHovered;
        var oldPlusHovered = _plusHovered;

        _minusHovered = _lvlMinusRect.Contains(pt) && CanDecreaseBoost();
        _plusHovered = _lvlPlusRect.Contains(pt) && CanIncreaseBoost();

        if (oldMinusHovered != _minusHovered || oldPlusHovered != _plusHovered)
            changed = true;

        var thumbHovered = _railScrollbarThumbRect.Contains(pt);
        if (_railHoveringThumb != thumbHovered)
        {
            _railHoveringThumb = thumbHovered;
            changed = true;
        }

        if (!_railScrollbarBounds.IsEmpty && _railScrollbarBounds.Contains(pt))
        {
            ClearHoverStateIfNeeded();
            Cursor = _railScrollbarThumbRect.Contains(pt) ? Cursors.Hand : Cursors.Default;

            if (changed)
                Invalidate();

            return;
        }

        foreach (var (rect, index) in _headerRects)
        {
            if (!rect.Contains(pt))
                continue;

            Cursor = Cursors.Hand;

            if (_hoverHeaderIndex != index)
            {
                _hoverHeaderIndex = index;
                _hoverEnhIndex = -1;
                _hoverSetIndex = -1;

                _hoverInfo = GetHeaderHoverInfo(index);
                _hoverText = string.Empty;

                Invalidate();
            }

            return;
        }

        foreach (var (rect, index) in _enhancementRects)
        {
            if (!rect.Contains(pt))
                continue;

            Cursor = Cursors.Hand;

            if (_hoverEnhIndex == index)
                return;

            _hoverEnhIndex = index;
            _hoverSetIndex = -1;
            _hoverHeaderIndex = -1;

            HandleGridHover(index);

            Invalidate();
            return;
        }

        foreach (var (rect, index) in _gradeRects)
        {
            if (!rect.Contains(pt))
                continue;

            Cursor = Cursors.Hand;

            if (_hoverSetIndex == index)
                return;

            _hoverSetIndex = index;
            _hoverEnhIndex = -1;
            _hoverHeaderIndex = -1;

            HandleRailHover(index);

            Invalidate();
            return;
        }

        if (_minusHovered || _plusHovered)
        {
            Cursor = Cursors.Hand;

            if (changed)
                Invalidate();

            return;
        }

        Cursor = Cursors.Default;

        if (!ClientRectangle.Contains(pt))
        {
            EnhancementSelectionCancelled?.Invoke();
            return;
        }

        if (_hoverHeaderIndex != -1 || _hoverEnhIndex != -1 || _hoverSetIndex != -1 || changed)
        {
            _hoverHeaderIndex = -1;
            _hoverEnhIndex = -1;
            _hoverSetIndex = -1;
            SetHoverText(string.Empty, string.Empty);
            Invalidate();
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        Focus();

        var pt = e.Location;

        if (e.Button == MouseButtons.Right)
        {
            EnhancementSelectionCancelled?.Invoke();
            return;
        }

        if (TryHandleRailScrollbarMouseDown(e))
            return;

        if (e.Button != MouseButtons.Left)
            return;

        foreach (var (rect, index) in _headerRects)
        {
            if (!rect.Contains(pt))
                continue;

            HandleHeaderClick(index);
            return;
        }

        foreach (var (rect, index) in _gradeRects)
        {
            if (!rect.Contains(pt))
                continue;

            HandleRailClick(index);
            return;
        }

        foreach (var (rect, index) in _enhancementRects)
        {
            if (!rect.Contains(pt))
                continue;

            HandleGridClick(index);
            return;
        }

        if (_lvlMinusRect.Contains(pt) && CanDecreaseBoost())
        {
            _minusPressed = true;
            Invalidate(_lvlMinusRect);
            return;
        }

        if (_lvlPlusRect.Contains(pt) && CanIncreaseBoost())
        {
            _plusPressed = true;
            Invalidate(_lvlPlusRect);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (_railDraggingThumb)
        {
            _railDraggingThumb = false;
            Capture = false;
            Invalidate(_railScrollbarBounds);
            return;
        }

        var wasMinus = _minusPressed;
        var wasPlus = _plusPressed;

        _minusPressed = false;
        _plusPressed = false;

        if (wasMinus && _lvlMinusRect.Contains(e.Location) && CanDecreaseBoost())
        {
            AdjustRelativeLevel(-1);
        }

        if (wasPlus && _lvlPlusRect.Contains(e.Location) && CanIncreaseBoost())
        {
            AdjustRelativeLevel(+1);
        }

        if (wasMinus || wasPlus)
            Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);

        if (_railDraggingThumb)
        {
            _railDraggingThumb = false;
            Capture = false;
        }

        _minusHovered = false;
        _plusHovered = false;
        _minusPressed = false;
        _plusPressed = false;
        _railHoveringThumb = false;

        Cursor = Cursors.Default;
        Invalidate();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        switch (e.KeyCode)
        {
            case Keys.Add:
            case Keys.Oemplus:
                if (CanIncreaseBoost())
                {
                    AdjustRelativeLevel(+1);
                    e.Handled = true;
                }

                break;

            case Keys.Subtract:
            case Keys.OemMinus:
                if (CanDecreaseBoost())
                {
                    AdjustRelativeLevel(-1);
                    e.Handled = true;
                }

                break;

            case Keys.Enter:
                if (_hoverEnhIndex >= 0 && _hoverEnhIndex < _model.EnhancementIds.Length)
                {
                    var enhId = _model.EnhancementIds[_hoverEnhIndex];
                    var slot = CreateSlotForEnh(enhId, _hoverEnhIndex);

                    if (slot is not null)
                    {
                        PersistLastStateForCurrentView();
                        EnhancementPicked?.Invoke(slot);
                    }

                    e.Handled = true;
                }

                break;

            case Keys.Escape:
                EnhancementSelectionCancelled?.Invoke();
                e.Handled = true;
                break;
        }

        Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Region?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void DrawOuterFrame(Graphics g)
    {
        MidsFrameRenderer.DrawOuterFrame(g, _layout.OuterBounds, Palette, CornerRadius + 1);
    }

    private void DrawHeaderBox(Graphics g)
    {
        MidsFrameRenderer.DrawPanel(
            g,
            _layout.HeaderBox,
            Palette.HeaderTop,
            Palette.HeaderBottom,
            Palette.PanelBorder,
            Palette.InnerHighlight,
            CornerRadius);

        using var titleFont = CreateScaledFont("Segoe UI", 9.75f, FontStyle.Bold, 7f);
        var title = InDesigner ? "Enhancing: Aimed Shot" : _hoverTitle ?? "Enhancing";
        MidsFrameRenderer.DrawCenteredText(g, title, titleFont, Palette.HeaderText, _layout.HeaderBox);
    }

    private void DrawTopInfoBox(Graphics g)
    {
        MidsFrameRenderer.DrawPanel(
            g,
            _layout.InfoBox,
            Palette.PanelTop,
            Palette.PanelBottom,
            Palette.PanelBorder,
            Palette.InnerHighlight,
            CornerRadius);

        using var infoFont = CreateScaledFont("Segoe UI", 8.75f, FontStyle.Bold, 6.5f);
        var info = InDesigner ? "Normal Enhancements" : GetSectionTitle();
        MidsFrameRenderer.DrawCenteredText(g, info, infoFont, Palette.AccentText, _layout.InfoBox);
    }

    private void DrawTypeStrip(Graphics g)
    {
        MidsFrameRenderer.DrawPanel(
            g,
            _layout.TypeStripPanel,
            Palette.RailTop,
            Palette.RailBottom,
            Palette.RailBorder,
            Palette.InnerHighlight,
            CornerRadius);
    }

    private void DrawTypeButtons(Graphics g)
    {
        using var fallbackFont = CreateScaledFont("Segoe UI Symbol", 18f, FontStyle.Bold, 10f);

        for (var i = 0; i < _layout.TypeButtons.Length && i < TypeIconCount; i++)
        {
            var rect = _layout.TypeButtons[i];
            var tab = HeaderIndexToType(i);
            var selected = !InDesigner && _model.View.TabId == tab || InDesigner && i == 1;
            var hovered = _hoverHeaderIndex == i;

            _headerRects.Add((rect, i));
            MidsFrameRenderer.DrawIconTile(g, rect, Palette, selected, hovered, false, CornerRadius);

            if (!InDesigner &&
                AssetManager.EnhTypes.TryGetValue(i, out var iconBitmap) &&
                iconBitmap?.Bitmap is not null)
            {
                g.DrawImage(iconBitmap.Bitmap, IconContentRect(rect));
            }
            else
            {
                var glyph = i switch
                {
                    0 => "×",
                    1 => "◆",
                    2 => "◈",
                    3 => "✦",
                    4 => "▣",
                    _ => "?"
                };

                MidsFrameRenderer.DrawCenteredText(g, glyph, fallbackFont, selected ? Palette.SelectionBorder : Palette.AccentText, rect);
            }
        }
    }

    private void DrawGridPanel(Graphics g)
    {
        MidsFrameRenderer.DrawPanel(g, _layout.GridPanel, Palette, CornerRadius);

        if (InDesigner)
        {
            DrawDesignGrid(g);
            return;
        }

        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid)
        {
            DrawSetFamilyGrid(g);
            return;
        }

        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid)
        {
            DrawSetVariantGrid(g);
            return;
        }

        DrawEnhancementGrid(g);
    }

    private void DrawDesignGrid(Graphics g)
    {
        using var textFont = CreateScaledFont("Segoe UI", 8.25f, FontStyle.Bold, 6.5f);

        for (var i = 0; i < _layout.GridSlots.Length; i++)
        {
            var rect = _layout.GridSlots[i];
            var selected = i == 2;
            var hovered = _hoverEnhIndex == i;

            _enhancementRects.Add((rect, i));
            MidsFrameRenderer.DrawIconTile(g, rect, Palette, selected, hovered, false, 8);

            var label = i switch
            {
                0 => "ACC",
                1 => "DMG",
                2 => "RCH",
                3 => "END",
                4 => "RNG",
                5 => "DEF",
                6 => "RES",
                7 => "HEAL",
                8 => "HOLD",
                9 => "SLOW",
                _ => $"{i + 1}"
            };

            MidsFrameRenderer.DrawCenteredText(g, label, textFont, selected ? Palette.SelectionBorder : Palette.Text, rect);
        }
    }

    private void DrawSetFamilyGrid(Graphics g)
    {
        for (var i = 0; i < _layout.GridSlots.Length && i < _model.SetIds.Length; i++)
        {
            var rect = _layout.GridSlots[i];

            _enhancementRects.Add((rect, i));

            var hovered = _hoverEnhIndex == i;
            MidsFrameRenderer.DrawIconTile(g, rect, Palette, false, hovered, false, 8);

            var setId = _model.SetIds[i];
            AssetManager.DrawEnhancementSet(g, IconContentRect(rect), setId);
        }
    }

    private void DrawSetVariantGrid(Graphics g)
    {
        for (var i = 0; i < _layout.GridSlots.Length && i < _model.SetVariants.Length; i++)
        {
            var rect = _layout.GridSlots[i];

            _enhancementRects.Add((rect, i));

            var selected = _model.View.SetVariant == _model.SetVariants[i];
            var hovered = _hoverEnhIndex == i;

            MidsFrameRenderer.DrawIconTile(g, rect, Palette, selected, hovered, false, 8);
            AssetManager.DrawEnhancementSetVariant(g, IconContentRect(rect), _model.View.SetId, _model.SetVariants[i]);
        }
    }

    private void DrawEnhancementGrid(Graphics g)
    {
        for (var i = 0; i < _layout.GridSlots.Length && i < _model.EnhancementIds.Length; i++)
        {
            var rect = _layout.GridSlots[i];

            _enhancementRects.Add((rect, i));

            var selected = _model.View.PickerId == i;
            var hovered = _hoverEnhIndex == i;
            var disabled = IsEnhancementGrayed(i);

            MidsFrameRenderer.DrawIconTile(g, rect, Palette, selected, hovered, disabled, 8);

            var enhId = _model.EnhancementIds[i];
            var iconIndex = GetEnhImageIndex(enhId);

            if (iconIndex < 0 || iconIndex >= AssetManager.Enhancements.Count)
                continue;

            using var attributes = GetImageAttributes(disabled);
            AssetManager.DrawEnhancementAt(
                g,
                IconContentRect(rect),
                iconIndex,
                enhId,
                _model.View.TabId,
                _model.View.GradeId,
                attributes);
        }
    }

    private void DrawRailPanel(Graphics g)
    {
        MidsFrameRenderer.DrawPanel(
            g,
            _layout.RailPanel,
            Palette.RailTop,
            Palette.RailBottom,
            Palette.RailBorder,
            Palette.InnerHighlight,
            CornerRadius);

        if (InDesigner)
        {
            DrawDesignRail(g);
            return;
        }

        DrawGradeColumn(g);
    }

    private void DrawDesignRail(Graphics g)
    {
        using var font = CreateScaledFont("Segoe UI", 8.25f, FontStyle.Bold, 6.5f);
        var labels = new[] { "TO", "DO", "SO", "HO", "SHO" };
        var clipState = g.Save();
        g.SetClip(_layout.RailIconViewport);

        for (var i = 0; i < _layout.RailSlots.Length && i < labels.Length; i++)
        {
            var rect = _layout.RailSlots[i];

            _gradeRects.Add((rect, i));

            var selected = i == 2;
            var hovered = _hoverSetIndex == i;

            MidsFrameRenderer.DrawIconTile(g, rect, Palette, selected, hovered, false, 8);
            MidsFrameRenderer.DrawCenteredText(g, labels[i], font, selected ? Palette.SelectionBorder : Palette.Text, rect);
        }

        g.Restore(clipState);

        DrawRailScrollbar(g, labels.Length);
    }

    private void DrawGradeColumn(Graphics g)
    {
        var tabId = _model.View.TabId;
        int[]? indices = null;
        Dictionary<int, ExtendedBitmap>? sourceDictionary = null;
        var loopStart = 0;

        if (tabId == Enums.eType.Normal)
        {
            indices = _model.NoGrades;
            sourceDictionary = AssetManager.EnhGrades;
        }
        else if (tabId == Enums.eType.SpecialO)
        {
            indices = _model.SpecialTypes;
            sourceDictionary = AssetManager.EnhSpecials;
            loopStart = 1;
        }
        else if (tabId == Enums.eType.SetO)
        {
            indices = _model.SetTypes;
            sourceDictionary = AssetManager.SetTypes;
        }

        if (indices is null || sourceDictionary is null)
        {
            DrawRailScrollbar(g, 0);
            return;
        }

        var itemCount = Math.Max(0, indices.Length - loopStart);
        var step = _layout.IconSize + _layout.RailSlotGap;

        _scrollOffset = ClampRailScrollOffset(_scrollOffset, itemCount, step);

        var maxScrollRows = Math.Max(0, itemCount - MaxVisibleGradeIcons);
        var scrollRows = Math.Min(step <= 0 ? 0 : _scrollOffset / step, maxScrollRows);
        var clipState = g.Save();
        g.SetClip(_layout.RailIconViewport);

        for (var i = 0; i < _layout.RailSlots.Length; i++)
        {
            var dataIndex = loopStart + scrollRows + i;
            if (dataIndex < loopStart || dataIndex >= indices.Length)
                continue;

            var rect = _layout.RailSlots[i];

            _gradeRects.Add((rect, dataIndex));

            var selected = IsSelectorIndexSelected(tabId, indices, dataIndex);
            var hovered = _hoverSetIndex == dataIndex;
            var iconKey = indices[dataIndex];

            MidsFrameRenderer.DrawIconTile(g, rect, Palette, selected, hovered, false, 8);

            if (sourceDictionary.TryGetValue(iconKey, out var iconToDraw) && iconToDraw?.Bitmap is not null)
            {
                if (tabId == Enums.eType.Normal)
                {
                    var gradeBorder = AssetManager.ToGfxGrade(Enums.eType.Normal, (Enums.eEnhGrade)iconKey);
                    if (AssetManager.TryGetBorderBitmap(gradeBorder, out var borderImage) && borderImage?.Bitmap is not null)
                    {
                        g.DrawImage(borderImage.Bitmap, IconContentRect(rect));
                    }
                }

                g.DrawImage(iconToDraw.Bitmap, IconContentRect(rect));
            }
        }

        g.Restore(clipState);

        DrawRailScrollbar(g, itemCount);
    }

    private void DrawRailScrollbar(Graphics g, int itemCount)
    {
        _railScrollbarBounds = Rectangle.Empty;
        _railScrollbarTrackBounds = Rectangle.Empty;
        _railScrollbarThumbRect = Rectangle.Empty;

        if (_layout.RailScrollbarBounds.IsEmpty || itemCount <= MaxVisibleGradeIcons)
        {
            _railHoveringThumb = false;
            return;
        }

        var bounds = _layout.RailScrollbarBounds;
        var step = _layout.IconSize + _layout.RailSlotGap;

        _railScrollbarBounds = bounds;
        _railScrollbarTrackBounds = Rectangle.FromLTRB(
            bounds.Left + RailScrollbarInset,
            bounds.Top + RailScrollbarInset,
            bounds.Right - RailScrollbarInset,
            bounds.Bottom - RailScrollbarInset);

        var scrollMax = Math.Max(0, (itemCount - MaxVisibleGradeIcons) * step);
        var trackHeight = Math.Max(0, _railScrollbarTrackBounds.Height);
        var thumbHeight = Math.Max(
            RailScrollbarThumbMinHeight,
            (int)Math.Round((double)MaxVisibleGradeIcons / itemCount * trackHeight));

        thumbHeight = Math.Min(thumbHeight, trackHeight);

        var available = Math.Max(0, trackHeight - thumbHeight);
        var thumbY = _railScrollbarTrackBounds.Top;

        if (available > 0 && scrollMax > 0)
        {
            var ratio = (double)_scrollOffset / scrollMax;
            thumbY = _railScrollbarTrackBounds.Top + (int)Math.Round(available * ratio);
        }

        var inset = Math.Max(1, bounds.Width / 4);

        _railScrollbarThumbRect = new Rectangle(
            bounds.Left + inset,
            thumbY,
            Math.Max(1, bounds.Width - inset * 2),
            thumbHeight);

        using var trackPen = new Pen(Color.FromArgb(95, Palette.Divider), 2f);
        var centerX = bounds.Left + bounds.Width / 2;

        g.DrawLine(trackPen, centerX, _railScrollbarTrackBounds.Top, centerX, _railScrollbarTrackBounds.Bottom);

        using var thumbBrush = new SolidBrush(_railHoveringThumb || _railDraggingThumb
            ? Palette.AccentText
            : Color.FromArgb(170, Palette.AccentText));

        g.FillRectangle(thumbBrush, _railScrollbarThumbRect);
    }

    private void DrawInfoPanel(Graphics g)
    {
        MidsFrameRenderer.DrawPanel(g, _layout.InfoPanel, Palette, CornerRadius);

        using var infoFont = CreateScaledFont("Segoe UI", 9f, FontStyle.Regular, 6.75f);

        var text = InDesigner
            ? "Choose a grade, then select an enhancement."
            : _hoverText ?? string.Empty;

        var textRect = Rectangle.Inflate(_layout.InfoPanel, -ScalePaintMetric(12, 6), -ScalePaintMetric(8, 4));
        MidsFrameRenderer.DrawText(g, text, infoFont, Palette.Text, textRect, ContentAlignment.MiddleLeft);
    }

    private void DrawLevelBoostPanel(Graphics g)
    {
        MidsFrameRenderer.DrawPanel(g, _layout.LevelBoostPanel, Palette, CornerRadius);

        using var labelFont = CreateScaledFont("Segoe UI", 8.25f, FontStyle.Regular, 6.75f);
        using var valueFont = CreateScaledFont("Segoe UI", 11.25f, FontStyle.Bold, 9f);
        using var boostFont = CreateScaledFont("Segoe UI", 8.75f, FontStyle.Bold, 7.5f);
        using var buttonFont = CreateScaledFont("Segoe UI", 10.5f, FontStyle.Bold, 8.5f);

        var panel = _layout.LevelBoostPanel;
        var relValue = InDesigner ? 3 : _model.View.RelLevel.ToInt();
        var boostCount = Math.Max(0, relValue);
        var maxBoost = GetMaxBoostCountForCurrentSelection();
        var level = InDesigner ? 50 : CheckAndReturnIoLevel();
        var topInset = ScalePaintMetric(6, 3);
        var labelHeight = ScalePaintMetric(11, 9);
        var valueHeight = ScalePaintMetric(16, 12);
        var dividerInset = ScalePaintMetric(10, 7);
        var dividerGap = ScalePaintMetric(2, 1);
        var boostTopGap = ScalePaintMetric(2, 1);
        var valueGap = ScalePaintMetric(1, 1);
        var contentBottomGap = ScalePaintMetric(2, 1);

        _lvlMinusRect = _layout.BoostMinusButton;
        _lvlPlusRect = _layout.BoostPlusButton;
        var levelLabelRect = new Rectangle(panel.Left, panel.Top + topInset, panel.Width, labelHeight);
        var levelValueRect = new Rectangle(panel.Left, levelLabelRect.Bottom + valueGap, panel.Width, valueHeight);

        MidsFrameRenderer.DrawCenteredText(g, "Level", labelFont, Palette.AccentText, levelLabelRect);
        MidsFrameRenderer.DrawCenteredText(g, $"{level}{GetRelativeLevelLabelForDisplay(_model.View.RelLevel)}", valueFont, Palette.Text, levelValueRect);

        var dividerY = levelValueRect.Bottom + dividerGap;
        using (var dividerPen = new Pen(Color.FromArgb(105, Palette.Divider), 1f))
        {
            g.DrawLine(dividerPen, panel.Left + dividerInset, dividerY, panel.Right - dividerInset, dividerY);
        }

        var boostLineTop = dividerY + boostTopGap;
        var boostLineBottom = Math.Max(boostLineTop, _lvlMinusRect.Top - contentBottomGap);
        var boostLineRect = Rectangle.FromLTRB(panel.Left, boostLineTop, panel.Right, boostLineBottom);

        MidsFrameRenderer.DrawCenteredText(g, $"Boost {boostCount} / {maxBoost}", boostFont, Palette.Text, boostLineRect);

        MidsFrameRenderer.DrawSmallButton(
            g,
            _lvlMinusRect,
            Palette,
            "−",
            buttonFont,
            _minusHovered,
            _minusPressed,
            CanDecreaseBoost());

        MidsFrameRenderer.DrawSmallButton(
            g,
            _lvlPlusRect,
            Palette,
            "+",
            buttonFont,
            _plusHovered,
            _plusPressed,
            CanIncreaseBoost());
    }

    private void HandleHeaderClick(int index)
    {
        var newTabId = HeaderIndexToType(index);

        if (newTabId == Enums.eType.SetO && _model.View.TabId == Enums.eType.SetO && StepBackSetSelection())
        {
            Invalidate();
            return;
        }

        _model.View.TabId = newTabId;
        _model.View.RelLevel = ValidateRelativeLevel(_model.View.RelLevel, _model.View.TabId, -1);

        if (newTabId != Enums.eType.None)
        {
            _lastTab = newTabId;
        }

        if (newTabId == Enums.eType.None)
        {
            EnhancementPicked?.Invoke(new I9Slot());
            return;
        }

        ResetSetSelection();
        _hoverSetIndex = -1;
        _scrollOffset = 0;

        SetActiveEnhancements(_powerId, -1, _normalEnhs, _inventionEnhs);
        Invalidate();
    }

    private void HandleRailClick(int index)
    {
        switch (_model.View.TabId)
        {
            case Enums.eType.Normal:
                if (index >= 0 && index < _model.NoGrades.Length)
                {
                    _model.View.GradeId = (Enums.eEnhGrade)_model.NoGrades[index];
                    _lastGrade = _model.View.GradeId;
                }

                break;

            case Enums.eType.SpecialO:
                if (index >= 0 && index < _model.SpecialTypes.Length)
                {
                    _model.View.SpecialId = _model.SpecialTypes[index];
                    _lastSpecial = _model.View.SpecialId;
                    SetActiveEnhancements(_powerId, -1, _normalEnhs, _inventionEnhs);
                }

                break;

            case Enums.eType.SetO:
                if (index >= 0 && index < _model.SetTypes.Length)
                {
                    _lastSet = index;
                    EnterSetType(index);
                }

                break;
        }

        Invalidate();
    }

    private void HandleGridClick(int index)
    {
        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid)
        {
            if (index < 0 || index >= _model.SetIds.Length)
                return;

            _model.View.SetId = _model.SetIds[index];
            _model.View.SetVariant = null;
            OpenSelectedSetFamily(-1);
            Invalidate();
            return;
        }

        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid)
        {
            if (index < 0 || index >= _model.SetVariants.Length)
                return;

            _model.View.SetVariant = _model.SetVariants[index];
            SetActiveEnhancements(_powerId, -1, _normalEnhs, _inventionEnhs);
            Invalidate();
            return;
        }

        if (index < 0 || index >= _model.EnhancementIds.Length)
            return;

        _model.View.PickerId = index;

        var enhId = _model.EnhancementIds[index];
        var slot = CreateSlotForEnh(enhId, index);

        if (slot is not null)
        {
            PersistLastStateForCurrentView();
            EnhancementPicked?.Invoke(slot);
        }

        Invalidate();
    }

    private void HandleGridHover(int index)
    {
        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid)
        {
            if (index >= _model.SetIds.Length)
                return;

            var setId = _model.SetIds[index];
            var setData = DatabaseAPI.Database.EnhancementSets[setId];
            var setTypeName = DatabaseAPI.GetSetTypeByIndex(setData.SetType).Name;

            SetHoverText(
                $"{setData.DisplayName}\nType: {setTypeName}     Level Range: {setData.LevelMin + 1}-{setData.LevelMax + 1}",
                "Click to view enhancements in this set.");

            RaiseHoverSetEvent(setId);
            return;
        }

        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid)
        {
            if (index >= _model.SetVariants.Length || _model.View.SetId <= -1)
                return;

            var variant = _model.SetVariants[index];

            SetHoverText(
                $"{DatabaseAPI.Database.EnhancementSets[_model.View.SetId].DisplayName} - {GetSetVariantDisplayName(variant)}",
                "Click to view the pieces in this variant.");

            return;
        }

        if (index >= _model.EnhancementIds.Length)
            return;

        var enhId = _model.EnhancementIds[index];

        SetHoverText(GetDisplayNameForEnhancement(enhId), GetHoverTextForEnhancement(enhId));
        RaiseHoverEnhancementEvent(enhId);
    }

    private void HandleRailHover(int index)
    {
        switch (_model.View.TabId)
        {
            case Enums.eType.Normal:
                if (index >= 0 && index < _model.NoGrades.Length)
                {
                    var grade = (Enums.eEnhGrade)_model.NoGrades[index];
                    _hoverInfo = DatabaseAPI.Database.EnhGradeStringLong[(int)grade];
                    _hoverText = string.Empty;
                }

                break;

            case Enums.eType.SpecialO:
                if (index >= 0 && index < _model.SpecialTypes.Length)
                {
                    var special = DatabaseAPI.GetSpecialEnhByIndex(_model.SpecialTypes[index]);
                    _hoverInfo = special.Name;
                    _hoverText = special.Description;
                }

                break;

            case Enums.eType.SetO:
                if (index >= 0 && index < _model.SetTypes.Length)
                {
                    var setType = DatabaseAPI.GetSetTypeByIndex(_model.SetTypes[index]);
                    _hoverInfo = setType.Name;
                    _hoverText = "Click to view sets in this category.";
                }

                break;
        }
    }

    private bool TryHandleRailScrollbarMouseDown(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || _railScrollbarBounds.IsEmpty || !_railScrollbarBounds.Contains(e.Location))
            return false;

        Focus();

        var itemCount = GetVisibleRailItemCount();
        if (itemCount <= MaxVisibleGradeIcons)
            return true;

        if (_railScrollbarThumbRect.Contains(e.Location))
        {
            _railDraggingThumb = true;
            _railDragStartY = e.Y - _railScrollbarThumbRect.Y;
            Capture = true;
            Invalidate(_railScrollbarBounds);
            return true;
        }

        if (_railScrollbarTrackBounds.Contains(e.Location))
        {
            var pageStep = MaxVisibleGradeIcons * (_layout.IconSize + _layout.RailSlotGap);

            if (e.Y < _railScrollbarThumbRect.Top)
            {
                _scrollOffset = ClampRailScrollOffset(_scrollOffset - pageStep, itemCount, _layout.IconSize + _layout.RailSlotGap);
            }
            else if (e.Y > _railScrollbarThumbRect.Bottom)
            {
                _scrollOffset = ClampRailScrollOffset(_scrollOffset + pageStep, itemCount, _layout.IconSize + _layout.RailSlotGap);
            }

            Invalidate();
            return true;
        }

        return true;
    }

    private void DragRailScrollbar(int mouseY)
    {
        if (_railScrollbarTrackBounds.IsEmpty || _railScrollbarThumbRect.IsEmpty)
            return;

        var itemCount = GetVisibleRailItemCount();
        var step = _layout.IconSize + _layout.RailSlotGap;
        var maxScroll = Math.Max(0, (itemCount - MaxVisibleGradeIcons) * step);
        var available = Math.Max(0, _railScrollbarTrackBounds.Height - _railScrollbarThumbRect.Height);

        if (available <= 0 || maxScroll <= 0)
            return;

        var newThumbY = mouseY - _railDragStartY;

        newThumbY = Math.Max(
            _railScrollbarTrackBounds.Top,
            Math.Min(newThumbY, _railScrollbarTrackBounds.Top + available));

        var ratio = (double)(newThumbY - _railScrollbarTrackBounds.Top) / available;

        _scrollOffset = ClampRailScrollOffset((int)Math.Round(ratio * maxScroll), itemCount, step);

        Invalidate();
    }

    private void ClearHoverStateIfNeeded()
    {
        if (_hoverHeaderIndex == -1 && _hoverEnhIndex == -1 && _hoverSetIndex == -1)
            return;

        _hoverHeaderIndex = -1;
        _hoverEnhIndex = -1;
        _hoverSetIndex = -1;
        _hoverInfo = string.Empty;
        _hoverText = string.Empty;

        Invalidate();
    }

    private void SetHoverText(string? info, string? text)
    {
        _hoverInfo = info;
        _hoverText = text;
    }

    private bool CanDecreaseBoost()
    {
        if (InDesigner)
            return true;

        return _model.View.RelLevel.ToInt() > GetMinBoostCountForCurrentSelection();
    }

    private bool CanIncreaseBoost()
    {
        if (InDesigner)
            return true;

        return _model.View.RelLevel.ToInt() < GetMaxBoostCountForCurrentSelection();
    }

    private int GetMinBoostCountForCurrentSelection()
    {
        return _model.View.TabId switch
        {
            Enums.eType.Normal => -3,
            Enums.eType.SpecialO => -3,
            Enums.eType.InventO or Enums.eType.SetO => 0,
            _ => -3
        };
    }

    private int GetMaxBoostCountForCurrentSelection()
    {
        if (InDesigner)
            return 5;

        return _model.View.TabId switch
        {
            Enums.eType.Normal => 3,
            Enums.eType.SpecialO => 3,
            Enums.eType.InventO or Enums.eType.SetO => 5,
            _ => 5
        };
    }

    private void AdjustRelativeLevel(int delta)
    {
        var current = _model.View.RelLevel.ToInt();
        var newRaw = current + delta;

        var enhId = -1;

        if (_hoverEnhIndex >= 0 && _hoverEnhIndex < _model.EnhancementIds.Length)
        {
            enhId = _model.EnhancementIds[_hoverEnhIndex];
        }
        else if (_model.View.PickerId >= 0 && _model.View.PickerId < _model.EnhancementIds.Length)
        {
            enhId = _model.EnhancementIds[_model.View.PickerId];
        }

        _model.View.RelLevel = ValidateRelativeLevel(newRaw.ToEnhRelative(), _model.View.TabId, enhId);
    }

    private Enums.eEnhRelative ValidateRelativeLevel(Enums.eEnhRelative current, Enums.eType tabId, int enhId)
    {
        if (enhId > -1)
        {
            if (tabId == Enums.eType.SetO && DatabaseAPI.IsAttunedSetVariant(enhId))
                return Enums.eEnhRelative.Even;

            if (tabId != Enums.eType.SetO && (_model.HasCatalyst(enhId) || _model.IsNaturallyAttuned(enhId)))
                return Enums.eEnhRelative.Even;
        }

        var value = current.ToInt();

        var clamped = tabId switch
        {
            Enums.eType.Normal => Math.Clamp(value, -3, 3),
            Enums.eType.SpecialO => Math.Clamp(value, -3, 3),
            Enums.eType.InventO or Enums.eType.SetO => Math.Clamp(value, 0, 5),
            _ => Math.Clamp(value, -3, 5)
        };

        return clamped.ToEnhRelative();
    }

    private string GetRelativeLevelLabelForDisplay(Enums.eEnhRelative relLevel)
    {
        var value = relLevel.ToInt();

        return value switch
        {
            > 0 => $" +{value}",
            < 0 => $" {value}",
            _ => string.Empty
        };
    }

    private string GetRelativeLevelLabel(Enums.eEnhRelative relLevel)
    {
        var value = relLevel.ToInt();

        return value switch
        {
            > 0 => $"+{value}",
            < 0 => $"{value}",
            _ => string.Empty
        };
    }

    private I9Slot? CreateSlotForEnh(int enhId, int index)
    {
        if (IsEnhancementGrayed(index))
            return default;

        var slot = new I9Slot
        {
            Enh = enhId,
            RelativeLevel = _model.View.RelLevel
        };

        if (_model.View.TabId is Enums.eType.InventO or Enums.eType.SetO)
            slot.IOLevel = _model.View.IoLevel - 1;

        if (_model.View.TabId == Enums.eType.Normal)
            slot.Grade = _model.View.GradeId;

        return slot;
    }

    private void SetActiveEnhancements(int iPower, int initialEnhId, int[] normalEnhs, int[] inventionEnhs)
    {
        int[] enhIdList;

        var tab = _model.View.TabId;
        var specialSubType = _model.View.SpecialId;

        _model.EnhancementNames.Clear();
        _model.EnhancementDescriptions.Clear();
        _model.VisiblePieceIndexes = [];
        _model.SetVariants = [];

        switch (tab)
        {
            case Enums.eType.Normal:
                enhIdList = normalEnhs;
                break;

            case Enums.eType.InventO:
                enhIdList = inventionEnhs;
                break;

            case Enums.eType.SpecialO:
                enhIdList = GetValidEnhancements(iPower, tab, specialSubType).ToArray();
                break;

            case Enums.eType.SetO:
                ConfigureSetSelection(initialEnhId);
                enhIdList = _model.EnhancementIds;
                break;

            default:
                enhIdList = [];
                break;
        }

        if (tab != Enums.eType.SetO)
        {
            _model.EnhancementIds = enhIdList;
        }

        if (tab != Enums.eType.SetO && enhIdList.Length > 0 && enhIdList[0] > -1)
        {
            _model.EnhancementNames = enhIdList.ToDictionary(id => id, id => DatabaseAPI.Database.Enhancements[id].Name);
            _model.EnhancementDescriptions = enhIdList.ToDictionary(id => id, id => DatabaseAPI.Database.Enhancements[id].ShortName);
        }

        if (tab != Enums.eType.SetO && initialEnhId > -1 && _model.Initial.TabId == _model.View.TabId)
        {
            _model.Initial.PickerId = Array.IndexOf(enhIdList, initialEnhId);
            _model.View.PickerId = _model.Initial.PickerId;
        }

        var focusEnhancementId = _model.View.PickerId >= 0 && _model.View.PickerId < enhIdList.Length
            ? enhIdList[_model.View.PickerId]
            : enhIdList.FirstOrDefault(-1);

        _model.View.RelLevel = ValidateRelativeLevel(_model.View.RelLevel, _model.View.TabId, focusEnhancementId);
    }

    private void ConfigureSetSelection(int initialEnhId)
    {
        _model.EnhancementIds = [];
        _model.VisiblePieceIndexes = [];
        _model.View.PickerId = -1;

        if (_model.View.SetTypeId < 0 || _model.View.SetTypeId >= _model.SetTypes.Length)
        {
            _model.View.SetStage = SetPickerStage.SetFamilyGrid;
            _model.View.SetId = -1;
            _model.View.SetVariant = null;
            return;
        }

        _model.SetIds = GetSets(_model.SetTypes[_model.View.SetTypeId]);

        if (_model.View.SetId < 0 || !_model.SetIds.Contains(_model.View.SetId))
        {
            _model.View.SetId = -1;
            _model.View.SetVariant = null;
            _model.View.SetStage = SetPickerStage.SetFamilyGrid;
            return;
        }

        var setId = _model.View.SetId;
        var variants = GetOrderedSetVariants(setId);

        _model.SetVariants = variants;

        if (variants.Length == 1)
        {
            _model.View.SetVariant = variants[0];
            _model.View.SetStage = SetPickerStage.SetEnhancementGrid;
        }
        else
        {
            if (!_model.View.SetVariant.HasValue || !variants.Contains(_model.View.SetVariant.Value))
            {
                if (initialEnhId > -1 && DatabaseAPI.Database.Enhancements[initialEnhId].nIDSet == setId)
                {
                    _model.View.SetVariant = DatabaseAPI.GetSetVariantKind(initialEnhId);
                }
                else
                {
                    _model.View.SetVariant = null;
                }
            }

            _model.View.SetStage = _model.View.SetVariant.HasValue
                ? SetPickerStage.SetEnhancementGrid
                : SetPickerStage.SetVariantGrid;

            if (_model.View.SetStage == SetPickerStage.SetVariantGrid)
                return;
        }

        if (!_model.View.SetVariant.HasValue)
            return;

        var projection = DatabaseAPI.GetEnhancementSetProjection(setId);
        var visiblePieces = projection.VisiblePieces
            .Select(piece => new
            {
                piece.PieceIndex,
                piece.DisplayLabel,
                EnhancementId = DatabaseAPI.ResolveEnhancementVariantForSetPiece(setId, piece.PieceIndex, _model.View.SetVariant.Value)
            })
            .Where(piece => piece.EnhancementId >= 0)
            .ToArray();

        _model.EnhancementIds = visiblePieces.Select(piece => piece.EnhancementId).ToArray();
        _model.VisiblePieceIndexes = visiblePieces.Select(piece => piece.PieceIndex).ToArray();
        _model.EnhancementNames = visiblePieces.ToDictionary(piece => piece.EnhancementId, piece => piece.DisplayLabel);
        _model.EnhancementDescriptions = visiblePieces.ToDictionary(piece => piece.EnhancementId, piece => DatabaseAPI.Database.Enhancements[piece.EnhancementId].ShortName);

        if (initialEnhId > -1 &&
            DatabaseAPI.TryGetSetPieceIndexForEnhancement(initialEnhId, out var initialSetId, out var initialPieceIndex) &&
            initialSetId == setId)
        {
            _model.View.PickerId = Array.IndexOf(_model.VisiblePieceIndexes, initialPieceIndex);
        }
    }

    private void ApplyLastUsedStateForEmptySlot()
    {
        _model.Initial.TabId = _lastTab;
        _model.Initial.IoLevel = LastLevel > 0 ? LastLevel : MidsContext.Config.I9.DefaultIOLevel + 1;
        _model.Initial.SetTypeId = -1;
        _model.Initial.SetId = -1;
        _model.Initial.SetVariant = null;
        _model.Initial.SetStage = SetPickerStage.SetFamilyGrid;

        if (_lastTab == Enums.eType.SetO && _lastSet >= 0 && _lastSet < _model.SetTypes.Length)
        {
            _model.Initial.SetTypeId = _lastSet;
            _model.SetIds = GetSets(_model.SetTypes[_lastSet]);
        }
    }

    private void ResetSetSelection()
    {
        _model.View.SetTypeId = -1;
        _model.View.SetId = -1;
        _model.View.SetVariant = null;
        _model.View.SetStage = SetPickerStage.SetFamilyGrid;
        _model.View.PickerId = -1;

        _model.SetIds = [];
        _model.SetVariants = [];
        _model.EnhancementIds = [];
        _model.VisiblePieceIndexes = [];
    }

    private void EnterSetType(int setTypeIndex)
    {
        _model.View.SetTypeId = setTypeIndex;
        _model.View.SetId = -1;
        _model.View.SetVariant = null;
        _model.View.SetStage = SetPickerStage.SetFamilyGrid;
        _model.View.PickerId = -1;

        _model.SetIds = setTypeIndex >= 0 && setTypeIndex < _model.SetTypes.Length
            ? GetSets(_model.SetTypes[setTypeIndex])
            : [];

        _model.SetVariants = [];
        _model.EnhancementIds = [];
        _model.VisiblePieceIndexes = [];
    }

    private bool StepBackSetSelection()
    {
        if (_model.View.TabId != Enums.eType.SetO)
            return false;

        switch (_model.View.SetStage)
        {
            case SetPickerStage.SetEnhancementGrid when _model.SetVariants.Length > 1:
                _model.View.SetStage = SetPickerStage.SetVariantGrid;
                _model.View.SetVariant = null;
                _model.View.PickerId = -1;
                _model.EnhancementIds = [];
                _model.VisiblePieceIndexes = [];
                return true;

            case SetPickerStage.SetEnhancementGrid:
                _model.View.SetStage = SetPickerStage.SetFamilyGrid;
                _model.View.SetId = -1;
                _model.View.SetVariant = null;
                _model.View.PickerId = -1;
                _model.SetVariants = [];
                _model.EnhancementIds = [];
                _model.VisiblePieceIndexes = [];
                return true;

            case SetPickerStage.SetVariantGrid:
                _model.View.SetStage = SetPickerStage.SetFamilyGrid;
                _model.View.SetId = -1;
                _model.View.SetVariant = null;
                _model.View.PickerId = -1;
                _model.SetVariants = [];
                _model.EnhancementIds = [];
                _model.VisiblePieceIndexes = [];
                return true;

            case SetPickerStage.SetFamilyGrid when _model.View.SetTypeId > -1:
                ResetSetSelection();
                return true;

            default:
                return false;
        }
    }

    private void OpenSelectedSetFamily(int initialEnhId)
    {
        if (initialEnhId > -1 && DatabaseAPI.Database.Enhancements[initialEnhId].nIDSet == _model.View.SetId)
        {
            _model.View.SetVariant = DatabaseAPI.GetSetVariantKind(initialEnhId);
        }

        SetActiveEnhancements(_powerId, initialEnhId, _normalEnhs, _inventionEnhs);
    }

    private void PersistLastStateForCurrentView()
    {
        _lastTab = _model.View.TabId;

        switch (_model.View.TabId)
        {
            case Enums.eType.Normal:
                _lastGrade = _model.View.GradeId;
                break;

            case Enums.eType.SpecialO:
                _lastSpecial = _model.View.SpecialId;
                break;

            case Enums.eType.SetO:
                _lastSet = _model.View.SetTypeId;
                break;
        }
    }

    private EnhUniqueStatus ComputeUniqueStatusForEnhancement(int enhId)
    {
        var enhData = DatabaseAPI.Database.Enhancements[enhId];
        int? powerStatic = _powerId >= 0 ? DatabaseAPI.Database.Power[_powerId]?.StaticIndex : null;

        bool inMain;
        bool inAlternate;

        if (enhData.TypeID == Enums.eType.SetO && DatabaseAPI.TryGetSetPieceIndexForEnhancement(enhId, out _, out _))
        {
            bool SamePower(PowerEntry? powerEntry) =>
                powerStatic is null || powerEntry?.Power?.StaticIndex == powerStatic;

            inMain = (_slotted?.Any(s => DatabaseAPI.AreEnhancementsSameSetPiece(s, enhId)) == true)
                     || MidsContext.Character.CurrentBuild.Powers
                         .Where(p => p is { Power.Slottable: true } && SamePower(p))
                         .Any(p => p?.Slots.Any(s => DatabaseAPI.AreEnhancementsSameSetPiece(s.Enhancement.Enh, enhId)) == true);

            inAlternate = (_slotted?.Any(s => DatabaseAPI.AreEnhancementsSameSetPiece(s, enhId)) == true)
                          || MidsContext.Character.CurrentBuild.Powers
                              .Where(p => p is { Power.Slottable: true } && SamePower(p))
                              .Any(p => p?.Slots.Any(s => DatabaseAPI.AreEnhancementsSameSetPiece(s.FlippedEnhancement.Enh, enhId)) == true);
        }
        else if (enhData.Unique)
        {
            inMain = (_slotted?.Any(s => s == enhId) == true)
                     || MidsContext.Character.CurrentBuild.Powers
                         .Where(p => p is { Power.Slottable: true })
                         .Any(p => p?.Slots.Any(s => s.Enhancement.Enh == enhId) == true);

            inAlternate = (_slotted?.Any(s => s == enhId) == true)
                          || MidsContext.Character.CurrentBuild.Powers
                              .Where(p => p is { Power.Slottable: true })
                              .Any(p => p?.Slots.Any(s => s.FlippedEnhancement.Enh == enhId) == true);
        }
        else
        {
            bool SamePower(PowerEntry? powerEntry) =>
                powerStatic is null || powerEntry?.Power?.StaticIndex == powerStatic;

            inMain = (_slotted?.Any(s => s == enhId) == true)
                     || MidsContext.Character.CurrentBuild.Powers
                         .Where(p => p is { Power.Slottable: true } && SamePower(p))
                         .Any(p => p?.Slots.Any(s => s.Enhancement.Enh == enhId) == true);

            inAlternate = (_slotted?.Any(s => s == enhId) == true)
                          || MidsContext.Character.CurrentBuild.Powers
                              .Where(p => p is { Power.Slottable: true } && SamePower(p))
                              .Any(p => p?.Slots.Any(s => s.FlippedEnhancement.Enh == enhId) == true);
        }

        return new EnhUniqueStatus
        {
            InMain = inMain,
            InAlternate = inAlternate
        };
    }

    private void RaiseHoverEnhancementEvent(int enhId)
    {
        HoverEnhancement?.Invoke(enhId, ComputeUniqueStatusForEnhancement(enhId));
    }

    private void RaiseHoverSetEvent(int setId)
    {
        HoverSet?.Invoke(setId);
    }

    private bool IsEnhancementGrayed(int index)
    {
        var enhId = _model.EnhancementIds.ElementAtOrDefault(index);
        if (enhId < 0)
            return false;

        var enh = DatabaseAPI.Database.Enhancements[enhId];

        if (enh.TypeID == Enums.eType.SetO && DatabaseAPI.TryGetSetPieceIndexForEnhancement(enhId, out _, out _))
        {
            var ignoredMatches = DatabaseAPI.AreEnhancementsSameSetPiece(_initialEnhancementId, enhId) ? 1 : 0;
            var currentPowerMatches = _slotted?.Count(slottedEnhId => DatabaseAPI.AreEnhancementsSameSetPiece(slottedEnhId, enhId)) ?? 0;

            return currentPowerMatches > ignoredMatches;
        }

        if (!enh.Unique)
            return false;

        var ignoredUniqueMatches = _initialEnhancementId == enhId ? 1 : 0;

        var uniqueMatches = MidsContext.Character.CurrentBuild.Powers
            .Where(p => p is not null)
            .SelectMany(p => p!.Slots)
            .Count(s => s.Enhancement.Enh == enhId);

        return uniqueMatches > ignoredUniqueMatches;
    }

    private static ImageAttributes GetImageAttributes(bool gray)
    {
        if (!gray)
            return new ImageAttributes();

        var colorMatrix = new ColorMatrix(BuildRenderer.HeroMatrix);

        for (var row = 0; row <= 2; row++)
        {
            for (var column = 0; column <= 2; column++)
            {
                colorMatrix[row, column] /= 2f;
            }
        }

        var attributes = new ImageAttributes();
        attributes.SetColorMatrix(colorMatrix);

        return attributes;
    }

    private bool IsSelectorIndexSelected(Enums.eType tabId, int[] indices, int dataIndex)
    {
        if (dataIndex < 0 || dataIndex >= indices.Length)
            return false;

        return tabId switch
        {
            Enums.eType.Normal => _model.View.GradeId == (Enums.eEnhGrade)indices[dataIndex],
            Enums.eType.SpecialO => _model.View.SpecialId == indices[dataIndex],
            Enums.eType.SetO => _model.View.SetTypeId == dataIndex,
            _ => false
        };
    }

    private int GetVisibleRailItemCount()
    {
        return _model.View.TabId switch
        {
            Enums.eType.Normal => _model.NoGrades.Length,
            Enums.eType.SpecialO => Math.Max(0, _model.SpecialTypes.Length - 1),
            Enums.eType.SetO => _model.SetTypes.Length,
            _ => 0
        };
    }

    private static int ClampRailScrollOffset(int offset, int itemCount, int step)
    {
        var maxScroll = Math.Max(0, (itemCount - MaxVisibleGradeIcons) * step);
        offset = Math.Clamp(offset, 0, maxScroll);

        return step > 0 ? offset / step * step : offset;
    }

    private static bool TryGetEnhancementDisplayLevelRange(int enhancementId, out int ioMin, out int ioMax)
    {
        ioMin = 10;
        ioMax = 50;

        if (!IsValidEnhancementId(enhancementId))
            return false;

        var enhancement = DatabaseAPI.Database.Enhancements.ElementAtOrDefault(enhancementId);
        if (enhancement is null)
            return false;

        if (enhancement.TypeID == Enums.eType.SetO &&
            enhancement.nIDSet > -1 &&
            DatabaseAPI.IsAttunedSetVariant(enhancementId))
        {
            var boostPower = enhancement.GetPower() as Power;
            ioMin = enhancement.LevelMin + 1;
            ioMax = (boostPower?.ImportedMaxBoostLevelZeroBased ?? enhancement.LevelMax) + 1;
        }
        else
        {
            ioMin = enhancement.LevelMin + 1;
            ioMax = enhancement.LevelMax + 1;
        }

        return true;
    }

    private static List<int> GetValidEnhancements(int powerIndex, Enums.eType type, int subType = 0)
    {
        return !IsValidPowerId(powerIndex)
            ? []
            : DatabaseAPI.Database.Power[powerIndex].GetValidEnhancements(type, subType);
    }

    private static int[] GetValidSetTypes(int powerIndex)
    {
        return !IsValidPowerId(powerIndex)
            ? []
            : DatabaseAPI.Database.Power[powerIndex].SetTypes.ToArray();
    }

    private static int[] GetOrderedSpecialTypes()
    {
        var desiredOrder = new[]
        {
            "None",
            "HO",
            "SynHO",
            "HyO",
            "TnO",
            "DSyncO",
            "Yin"
        };

        var orderLookup = desiredOrder
            .Select((shortName, index) => new { shortName, index })
            .ToDictionary(x => x.shortName, x => x.index, StringComparer.OrdinalIgnoreCase);

        return DatabaseAPI.Database.SpecialEnhancements
            .OrderBy(x => orderLookup.TryGetValue(x.ShortName, out var index) ? index : int.MaxValue)
            .ThenBy(x => x.Index)
            .Select(x => x.Index)
            .ToArray();
    }

    private static int[] GetSets(int setType)
    {
        return DatabaseAPI.Database.EnhancementSets
            .Select((set, index) => new { set, index })
            .Where(x => x.set.SetType == setType)
            .Select(x => x.index)
            .ToArray();
    }

    private static SetVariantKind[] GetOrderedSetVariants(int setId)
    {
        return DatabaseAPI.GetAvailableSetVariants(setId)
            .OrderBy(DatabaseAPI.GetSetVariantSortKey)
            .ToArray();
    }

    private static string GetSetVariantDisplayName(SetVariantKind variantKind)
    {
        return variantKind switch
        {
            SetVariantKind.Attuned => "Attuned",
            SetVariantKind.Superior => "Superior",
            SetVariantKind.SuperiorAttuned => "Superior Attuned",
            _ => "Crafted"
        };
    }

    private int SetTypeToId(int setType)
    {
        for (var i = 0; i < _model.SetTypes.Length; i++)
        {
            if (setType == _model.SetTypes[i])
                return i;
        }

        return -1;
    }

    private static bool IsValidPowerId(int powerId)
    {
        return DatabaseAPI.Database?.Power is not null &&
               powerId >= 0 &&
               powerId < DatabaseAPI.Database.Power.Length &&
               DatabaseAPI.Database.Power[powerId] is not null;
    }

    private static bool IsValidEnhancementId(int enhancementId)
    {
        return DatabaseAPI.Database?.Enhancements is not null &&
               enhancementId >= 0 &&
               enhancementId < DatabaseAPI.Database.Enhancements.Length;
    }

    private int GetEnhImageIndex(int enhId)
    {
        if (!IsValidEnhancementId(enhId))
            return -1;

        return DatabaseAPI.Database.Enhancements[enhId].ImageIdx;
    }

    private string GetDisplayNameForEnhancement(int enhId)
    {
        if (!IsValidEnhancementId(enhId))
            return string.Empty;

        var enhancement = DatabaseAPI.Database.Enhancements[enhId];

        return _model.EnhancementNames.TryGetValue(enhId, out var canonicalName)
            ? canonicalName
            : enhancement.Name;
    }

    private string GetHoverTextForEnhancement(int enhId)
    {
        if (!IsValidEnhancementId(enhId))
            return string.Empty;

        var enhancement = DatabaseAPI.Database.Enhancements[enhId];

        return _model.EnhancementDescriptions.GetValueOrDefault(enhId, enhancement.ShortName ?? string.Empty);
    }

    private static Rectangle IconContentRect(Rectangle rect)
    {
        return Rectangle.Inflate(rect, -IconInset, -IconInset);
    }

    private I9PickerLayoutOptions CreateResolvedLayoutOptions()
    {
        var resolved = _layoutOptions.Clone();
        var scale = Math.Clamp(_uiScale, 0.5f, 1f);

        resolved.IconSize = Math.Clamp((int)Math.Round(_layoutOptions.IconSize * scale), 32, 64);
        resolved.Margin = ScaleMetric(_layoutOptions.Margin, scale, 5);
        resolved.Gap = ScaleMetric(_layoutOptions.Gap, scale, 4);
        resolved.HeaderHeight = ScaleMetric(_layoutOptions.HeaderHeight, scale, 20);
        resolved.InfoHeight = ScaleMetric(_layoutOptions.InfoHeight, scale, 32);
        resolved.LastTypeButtonExtraSpacing = ScaleMetric(_layoutOptions.LastTypeButtonExtraSpacing, scale, 1);
        resolved.TypeStripInset = ScaleMetric(_layoutOptions.TypeStripInset, scale, 1);
        resolved.GridPanelInset = ScaleMetric(_layoutOptions.GridPanelInset, scale, 2);
        resolved.RailExtraWidth = ScaleMetric(_layoutOptions.RailExtraWidth, scale, 4);
        resolved.RailSlotGap = ScaleMetric(_layoutOptions.RailSlotGap, scale, 4);
        resolved.RailScrollbarWidth = ScaleMetric(_layoutOptions.RailScrollbarWidth, scale, 10);
        resolved.FooterHeight = ScaleMetric(_layoutOptions.FooterHeight, scale, 76);
        resolved.LevelBoostMinWidth = ScaleMetric(_layoutOptions.LevelBoostMinWidth, scale, 88);
        resolved.BoostButtonWidth = ScaleMetric(_layoutOptions.BoostButtonWidth, scale, 22);
        resolved.BoostButtonHeight = ScaleMetric(_layoutOptions.BoostButtonHeight, scale, 18);
        resolved.BoostButtonGap = ScaleMetric(_layoutOptions.BoostButtonGap, scale, 4);
        resolved.BoostButtonBottomInset = ScaleMetric(_layoutOptions.BoostButtonBottomInset, scale, 4);

        return resolved;
    }

    private void RefreshScaledSize()
    {
        var preferredSize = I9PickerLayout.CalculatePreferredSize(CreateResolvedLayoutOptions());
        var shouldAutoResize = Size.IsEmpty || Size == _autoPreferredSize;

        _autoPreferredSize = preferredSize;
        MinimumSize = preferredSize;

        if (shouldAutoResize)
        {
            Size = preferredSize;
        }
        else
        {
            Invalidate();
        }
    }

    private static int ScaleMetric(int value, float scale, int minimum)
    {
        return Math.Max(minimum, (int)Math.Round(value * scale));
    }

    private Font CreateScaledFont(string family, float baseSize, FontStyle style, float minimumSize)
    {
        var scale = Math.Clamp(_layout.IconSize / (float)IconSize, 0.5f, 1f);
        return new Font(family, Math.Max(minimumSize, baseSize * scale), style);
    }

    private int ScalePaintMetric(int baseValue, int minimum)
    {
        var scale = Math.Clamp(_layout.IconSize / (float)IconSize, 0.5f, 1f);
        return Math.Max(minimum, (int)Math.Round(baseValue * scale));
    }

    private static Enums.eType HeaderIndexToType(int index)
    {
        return index switch
        {
            0 => Enums.eType.None,
            1 => Enums.eType.Normal,
            2 => Enums.eType.InventO,
            3 => Enums.eType.SpecialO,
            4 => Enums.eType.SetO,
            _ => Enums.eType.Normal
        };
    }

    private static string GetHeaderLabel(int index)
    {
        return index switch
        {
            0 => "None",
            1 => "Normal",
            2 => "IO",
            3 => "Special",
            4 => "Sets",
            _ => string.Empty
        };
    }

    private static string GetHeaderHoverInfo(int index)
    {
        return HeaderIndexToType(index) switch
        {
            Enums.eType.None => "No Enhancement",
            Enums.eType.Normal => "Normal Enhancements",
            Enums.eType.InventO => "Invention Origin (IO)",
            Enums.eType.SpecialO => "Special Enhancements",
            Enums.eType.SetO => "Invention Sets",
            _ => string.Empty
        };
    }

    private string GetSectionTitle()
    {
        if (InDesigner)
            return "Normal Enhancements";

        if (!string.IsNullOrWhiteSpace(_hoverInfo))
            return _hoverInfo;

        if (_model.View.TabId == Enums.eType.SetO)
        {
            return _model.View.SetStage switch
            {
                SetPickerStage.SetFamilyGrid when _model.View.SetTypeId >= 0 && _model.View.SetTypeId < _model.SetTypes.Length
                    => DatabaseAPI.GetSetTypeByIndex(_model.SetTypes[_model.View.SetTypeId]).Name,

                SetPickerStage.SetVariantGrid when _model.View.SetId >= 0
                    => DatabaseAPI.Database.EnhancementSets[_model.View.SetId].DisplayName,

                SetPickerStage.SetEnhancementGrid when _model.View.SetId >= 0
                    => DatabaseAPI.Database.EnhancementSets[_model.View.SetId].DisplayName,

                _ => "Invention Sets"
            };
        }

        return _model.View.TabId switch
        {
            Enums.eType.None => "No Enhancement",
            Enums.eType.Normal => "Normal Enhancements",
            Enums.eType.InventO => "Invention Origin Enhancements",
            Enums.eType.SpecialO => "Special Enhancements",
            Enums.eType.SetO => "Invention Sets",
            _ => "Enhancements"
        };
    }

    private sealed class EnhSelectorModel
    {
        public EnhSelectorState View { get; set; } = new();
        public EnhSelectorState Initial { get; set; } = new();

        public int[] EnhancementIds { get; set; } = [];
        public int[] SetIds { get; set; } = [];
        public int[] VisiblePieceIndexes { get; set; } = [];
        public int[] NoGrades { get; set; } = [];
        public int[] SpecialTypes { get; set; } = [];
        public int[] SetTypes { get; set; } = [];
        public SetVariantKind[] SetVariants { get; set; } = [];

        public Dictionary<int, string> EnhancementNames { get; set; } = new();
        public Dictionary<int, string> EnhancementDescriptions { get; set; } = new();

        public bool HasCatalyst(int enhId) =>
            IsValidEnhancementId(enhId) &&
            DatabaseAPI.EnhHasCatalyst(DatabaseAPI.Database.Enhancements[enhId].UID);

        public bool IsNaturallyAttuned(int enhId) =>
            IsValidEnhancementId(enhId) &&
            DatabaseAPI.EnhIsNaturallyAttuned(enhId);
    }

    public sealed class EnhSelectorState
    {
        public Enums.eType TabId { get; set; } = Enums.eType.Normal;
        public Enums.eEnhGrade GradeId { get; set; } = Enums.eEnhGrade.SingleO;
        public Enums.eEnhRelative RelLevel { get; set; } = Enums.eEnhRelative.Even;
        public int PickerId { get; set; } = -1;
        public int IoLevel { get; set; } = MidsContext.Config.I9.DefaultIOLevel + 1;
        public int SpecialLevel { get; set; } = -1;
        public int SetTypeId { get; set; } = -1;
        public int SetId { get; set; } = -1;
        public int SpecialId { get; set; } = -1;
        public SetVariantKind? SetVariant { get; set; }
        public SetPickerStage SetStage { get; set; } = SetPickerStage.SetFamilyGrid;

        public EnhSelectorState()
        {
        }

        public EnhSelectorState(EnhSelectorState source)
        {
            ArgumentNullException.ThrowIfNull(source);

            TabId = source.TabId;
            GradeId = source.GradeId;
            RelLevel = source.RelLevel;
            PickerId = source.PickerId;
            IoLevel = source.IoLevel;
            SpecialLevel = source.SpecialLevel;
            SetTypeId = source.SetTypeId;
            SetId = source.SetId;
            SpecialId = source.SpecialId;
            SetVariant = source.SetVariant;
            SetStage = source.SetStage;
        }
    }
}

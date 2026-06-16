using System.ComponentModel;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Renderer;

namespace Mids_Reborn.UI.Controls.Test.EnhSelector;

[ToolboxItem(true)]
[DesignerCategory("Code")]
[DefaultEvent(nameof(EnhancementPicked))]
public sealed class EnhSelector : Control
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

    private enum ScrollArea
    {
        None,
        Main,
        Rail,
        Inspector
    }

    private const int DefaultIconSize = 48;
    private const int MinIconSize = 32;
    private const int MaxIconSize = 64;
    private const int MainGridColumns = 4;
    private const int MainGridVisibleRows = 5;
    private const float MinReadablePaintScale = 0.94f;

    private readonly EnhSelectorLayoutOptions _layoutOptions = new()
    {
        IconSize = DefaultIconSize,
        RailWidth = 132,
        FooterHeight = 92
    };

    private EnhSelectorLayout _layout = EnhSelectorLayout.Empty;
    private EnhSelectorModel _model = new();

    private readonly List<(Rectangle Bounds, int Index)> _headerRects = [];
    private readonly List<(Rectangle Bounds, int Index)> _mainItemRects = [];
    private readonly List<(Rectangle Bounds, int Index)> _railItemRects = [];

    private string? _hoverTitle;
    private int _hoverMainIndex = -1;
    private int _hoverRailIndex = -1;
    private int _hoverHeaderIndex = -1;

    private int _powerId = -1;
    private int[] _normalEnhs = [];
    private int[] _inventionEnhs = [];
    private int[] _slotted = [];
    private int _initialEnhancementId = -1;
    private int _setFamilyCount;

    private Enums.eType _lastTab = Enums.eType.Normal;
    private Enums.eEnhGrade _lastGrade = Enums.eEnhGrade.SingleO;
    private int _lastSpecial = 1;
    private int _lastSet;
    private readonly Dictionary<int, RememberedSetSelection> _rememberedSetSelectionsByPower = new();

    private int _mainScrollOffset;
    private int _railScrollOffset;
    private int _inspectorScrollOffset;
    private ScrollBarVisualState _mainScrollbar = ScrollBarVisualState.Empty;
    private ScrollBarVisualState _railScrollbar = ScrollBarVisualState.Empty;
    private ScrollBarVisualState _inspectorScrollbar = ScrollBarVisualState.Empty;
    private ScrollArea _dragScrollArea = ScrollArea.None;
    private int _dragThumbGrabOffsetY;

    private bool _minusHovered;
    private bool _plusHovered;
    private bool _minusPressed;
    private bool _plusPressed;
    private bool _closeHovered;
    private bool _backHovered;

    private Rectangle _oldBounds;
    private Size _autoPreferredSize;
    private float _uiScale = 1f;

    private bool InDesigner => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
    private static EnhSelectorVisualPalette Palette => EnhSelectorVisualPalette.Default;
    private static readonly Regex PercentHighlightRegex = new(@"(?:\+|-)?\d+(?:\.\d+)?%", RegexOptions.Compiled);

    public EnhSelector()
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
        ForeColor = Palette.Text;
        Font = new Font("Segoe UI", 9f, FontStyle.Regular);

        var preferredSize = EnhSelectorLayout.CalculatePreferredSize(CreateResolvedLayoutOptions());
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
    public Size BasePreferredSize => EnhSelectorLayout.CalculatePreferredSize(_layoutOptions.Clone());

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
    [DefaultValue(DefaultIconSize)]
    public int PickerIconSize
    {
        get => _layoutOptions.IconSize;
        set
        {
            var next = Math.Clamp(value, MinIconSize, MaxIconSize);
            if (_layoutOptions.IconSize == next)
                return;

            _layoutOptions.IconSize = next;
            RefreshScaledSize();
        }
    }

    [Category("Mids Layout")]
    [DefaultValue(132)]
    public int RailWidth
    {
        get => _layoutOptions.RailWidth;
        set
        {
            var next = Math.Clamp(value, 96, 220);
            if (_layoutOptions.RailWidth == next)
                return;

            _layoutOptions.RailWidth = next;
            RefreshScaledSize();
        }
    }

    [Category("Mids Layout")]
    [DefaultValue(92)]
    public int FooterHeight
    {
        get => _layoutOptions.FooterHeight;
        set
        {
            var next = Math.Clamp(value, 88, 200);
            if (_layoutOptions.FooterHeight == next)
                return;

            _layoutOptions.FooterHeight = next;
            RefreshScaledSize();
        }
    }

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
        _setFamilyCount = _model.SetTypes.Sum(setType => GetSets(setType).Length);
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

            RememberSetSelectionForPower(validPowerId, initialEnhancementId);
        }
        else
        {
            ApplyLastUsedStateForEmptySlot();
        }

        _model.View = new EnhSelectorState(_model.Initial);
        if (_model.View.TabId == Enums.eType.None)
            _model.View.TabId = _lastTab;

        SetActiveEnhancements(validPowerId, initialEnhancementId, _normalEnhs, _inventionEnhs);

        _mainScrollOffset = 0;
        _railScrollOffset = 0;
        _inspectorScrollOffset = 0;
        _hoverMainIndex = -1;
        _hoverRailIndex = -1;
        _hoverHeaderIndex = -1;
        _dragScrollArea = ScrollArea.None;
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

        _layout = EnhSelectorLayout.Calculate(ClientSize, CreateResolvedLayoutOptions());
        _headerRects.Clear();
        _mainItemRects.Clear();
        _railItemRects.Clear();

        var g = e.Graphics;
        EnhSelectorFrameRenderer.ApplyDefaultQuality(g);

        DrawOuterFrame(g);
        DrawTitleBar(g);
        DrawTabs(g);
        DrawContent(g);
        DrawFooter(g);
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

        using var path = EnhSelectorFrameRenderer.CreateRoundedRectanglePath(ClientRectangle, 10);
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

        var pt = PointToClient(MousePosition);
        var target = ResolveScrollArea(pt);
        if (target == ScrollArea.None)
            return;

        var delta = -Math.Sign(e.Delta) * GetScrollStep(target);
        AdjustScroll(target, delta);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_dragScrollArea != ScrollArea.None)
        {
            DragScrollbar(e.Y);
            return;
        }

        var pt = e.Location;
        UpdateButtonHoverStates(pt);

        if (UpdateScrollbarHover(pt))
            return;

        foreach (var (rect, index) in _headerRects)
        {
            if (!rect.Contains(pt))
                continue;

            Cursor = Cursors.Hand;
            if (_hoverHeaderIndex != index)
            {
                _hoverHeaderIndex = index;
                _hoverMainIndex = -1;
                _hoverRailIndex = -1;
                Invalidate();
            }

            return;
        }

        foreach (var (rect, index) in _mainItemRects)
        {
            if (!rect.Contains(pt))
                continue;

            Cursor = Cursors.Hand;
            if (_hoverMainIndex != index)
            {
                _hoverMainIndex = index;
                _hoverRailIndex = -1;
                _hoverHeaderIndex = -1;
                Invalidate();
            }

            return;
        }

        foreach (var (rect, index) in _railItemRects)
        {
            if (!rect.Contains(pt))
                continue;

            Cursor = Cursors.Hand;
            if (_hoverRailIndex != index)
            {
                _hoverRailIndex = index;
                _hoverMainIndex = -1;
                _hoverHeaderIndex = -1;
                Invalidate();
            }

            return;
        }

        if (_closeHovered || _backHovered || _minusHovered || _plusHovered)
        {
            Cursor = Cursors.Hand;
            Invalidate();
            return;
        }

        Cursor = Cursors.Default;
        if (_hoverHeaderIndex != -1 || _hoverMainIndex != -1 || _hoverRailIndex != -1)
        {
            _hoverHeaderIndex = -1;
            _hoverMainIndex = -1;
            _hoverRailIndex = -1;
            Invalidate();
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();

        if (e.Button == MouseButtons.Right)
        {
            EnhancementSelectionCancelled?.Invoke();
            return;
        }

        if (TryHandleScrollbarMouseDown(e))
            return;

        if (e.Button != MouseButtons.Left)
            return;

        if (_layout.CloseButton.Contains(e.Location))
        {
            EnhancementSelectionCancelled?.Invoke();
            return;
        }

        if (_layout.BackButton.Contains(e.Location) && ShouldShowBackButton())
        {
            if (StepBackSetSelection())
                Invalidate();
            return;
        }

        foreach (var (rect, index) in _headerRects)
        {
            if (rect.Contains(e.Location))
            {
                HandleHeaderClick(index);
                return;
            }
        }

        foreach (var (rect, index) in _railItemRects)
        {
            if (rect.Contains(e.Location))
            {
                HandleRailClick(index);
                return;
            }
        }

        foreach (var (rect, index) in _mainItemRects)
        {
            if (rect.Contains(e.Location))
            {
                HandleMainItemClick(index);
                return;
            }
        }

        if (_layout.FooterMinusButton.Contains(e.Location) && CanDecreaseBoost())
        {
            _minusPressed = true;
            Invalidate(_layout.FooterMinusButton);
            return;
        }

        if (_layout.FooterPlusButton.Contains(e.Location) && CanIncreaseBoost())
        {
            _plusPressed = true;
            Invalidate(_layout.FooterPlusButton);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (_dragScrollArea != ScrollArea.None)
        {
            _dragScrollArea = ScrollArea.None;
            Capture = false;
            Invalidate();
            return;
        }

        var wasMinus = _minusPressed;
        var wasPlus = _plusPressed;
        _minusPressed = false;
        _plusPressed = false;

        if (wasMinus && _layout.FooterMinusButton.Contains(e.Location) && CanDecreaseBoost())
        {
            AdjustRelativeLevel(-1);
        }

        if (wasPlus && _layout.FooterPlusButton.Contains(e.Location) && CanIncreaseBoost())
        {
            AdjustRelativeLevel(+1);
        }

        if (wasMinus || wasPlus)
            Invalidate();
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);

        if (e.Button != MouseButtons.Left)
            return;

        foreach (var (rect, index) in _headerRects)
        {
            if (!rect.Contains(e.Location))
                continue;

            if (index == 0)
            {
                CommitEmptySelection();
            }

            return;
        }

        foreach (var (rect, index) in _mainItemRects)
        {
            if (!rect.Contains(e.Location))
                continue;

            HandleMainItemDoubleClick(index);
            return;
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _closeHovered = false;
        _backHovered = false;
        _minusHovered = false;
        _plusHovered = false;
        _minusPressed = false;
        _plusPressed = false;
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
                if (_model.View.TabId == Enums.eType.None)
                {
                    CommitEmptySelection();
                    e.Handled = true;
                }
                else if (_hoverMainIndex >= 0)
                {
                    e.Handled = HandleMainItemEnter(_hoverMainIndex, commitFinalSelection: true);
                }
                else if (_model.View.PickerId >= 0)
                {
                    e.Handled = HandleMainItemEnter(_model.View.PickerId, commitFinalSelection: true);
                }
                else if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid && _model.View.SetId >= 0)
                {
                    var familyIndex = Array.IndexOf(_model.SetIds, _model.View.SetId);
                    e.Handled = HandleMainItemEnter(familyIndex, commitFinalSelection: true);
                }
                else if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid && _model.View.SetVariant.HasValue)
                {
                    var variantIndex = Array.IndexOf(_model.SetVariants, _model.View.SetVariant.Value);
                    e.Handled = HandleMainItemEnter(variantIndex, commitFinalSelection: true);
                }

                if (e.Handled)
                {
                    e.Handled = true;
                }

                break;

            case Keys.Escape:
                EnhancementSelectionCancelled?.Invoke();
                e.Handled = true;
                break;
        }

        if (e.Handled)
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
        EnhSelectorFrameRenderer.DrawOuterFrame(g, _layout.OuterBounds, Palette, 10);
    }

    private void DrawTitleBar(Graphics g)
    {
        using var titleFont = CreateScaledFont("Segoe UI", 13f, FontStyle.Bold, 9f);
        var title = InDesigner ? "Enhancing: Aimed Shot" : _hoverTitle ?? "Enhancing";
        var titleRect = Rectangle.FromLTRB(_layout.TitleBar.Left + 16, _layout.TitleBar.Top, _layout.CloseButton.Left - 12, _layout.TitleBar.Bottom);
        EnhSelectorFrameRenderer.DrawText(g, title, titleFont, Palette.HeaderText, titleRect, ContentAlignment.MiddleLeft, StringTrimming.EllipsisCharacter);

        _closeHovered = _layout.CloseButton.Contains(PointToClient(MousePosition));
        EnhSelectorFrameRenderer.DrawCloseButton(g, _layout.CloseButton, Palette, _closeHovered, false);
    }

    private void DrawTabs(Graphics g)
    {
        using var labelFont = CreateScaledFont("Segoe UI", 11f, FontStyle.Bold, 8.5f);

        for (var i = 0; i < _layout.TabButtons.Length; i++)
        {
            var rect = _layout.TabButtons[i];
            var selected = !InDesigner && _model.View.TabId == HeaderIndexToType(i) || InDesigner && i == 4;
            var hovered = _hoverHeaderIndex == i;

            _headerRects.Add((rect, i));
            EnhSelectorFrameRenderer.DrawTab(g, rect, Palette, selected, hovered, 9);

            var iconSize = Math.Min(
                GetUniformBandIconSize(),
                Math.Max(24, Math.Min(rect.Height - ScalePaintMetric(12, 8), rect.Width / 3)));
            var iconRect = new Rectangle(rect.Left + ScalePaintMetric(14, 10), rect.Top + (rect.Height - iconSize) / 2, iconSize, iconSize);
            DrawHeaderIcon(g, i, iconRect);

            var textLeft = iconRect.Right + ScalePaintMetric(12, 8);
            var labelText = GetHeaderLabel(i);
            var labelRect = Rectangle.FromLTRB(
                textLeft,
                rect.Top + ScalePaintMetric(6, 4),
                rect.Right - ScalePaintMetric(12, 8),
                rect.Bottom - ScalePaintMetric(6, 4));
            EnhSelectorFrameRenderer.DrawText(
                g,
                labelText,
                labelFont,
                selected ? Palette.Text : Palette.HeaderText,
                labelRect,
                ContentAlignment.MiddleLeft,
                StringTrimming.EllipsisCharacter);
        }
    }

    private void DrawContent(Graphics g)
    {
        EnhSelectorFrameRenderer.DrawPanel(g, _layout.MainPanel, Palette, 10);
        EnhSelectorFrameRenderer.DrawPanel(g, _layout.RailPanel, Palette, 10);
        EnhSelectorFrameRenderer.DrawPanel(g, _layout.InspectorPanel, Palette, 10);

        DrawMainPanel(g);
        DrawRailPanel(g);
        DrawInspectorPanel(g);
    }

    private void DrawMainPanel(Graphics g)
    {
        using var headerFont = CreateScaledFont("Segoe UI", 11.25f, FontStyle.Bold, 8.5f);
        using var subHeaderFont = CreateScaledFont("Segoe UI", 9.25f, FontStyle.Regular, 7.25f);
        using var chipFont = CreateScaledFont("Segoe UI", 8.5f, FontStyle.Bold, 6.5f);

        EnhSelectorFrameRenderer.DrawText(g, GetMainHeaderText(), headerFont, Palette.HeaderText, _layout.MainHeader, ContentAlignment.MiddleLeft, StringTrimming.EllipsisCharacter);

        if (ShouldShowBackButton())
        {
            _backHovered = _layout.BackButton.Contains(PointToClient(MousePosition));
            EnhSelectorFrameRenderer.DrawSmallButton(g, _layout.BackButton, Palette, "Back", chipFont, _backHovered, false, true);

            var actionChipText = GetMainHeaderActionChipText();
            if (!string.IsNullOrWhiteSpace(actionChipText))
            {
                var actionChipWidth = Math.Max(74, ScalePaintMetric(18, 12) + TextRenderer.MeasureText(actionChipText, chipFont).Width);
                var actionChipRect = new Rectangle(
                    _layout.BackButton.Left - ScalePaintMetric(10, 6) - actionChipWidth,
                    _layout.BackButton.Top,
                    actionChipWidth,
                    _layout.BackButton.Height);
                if (actionChipRect.Left > _layout.MainHeader.Left + ScalePaintMetric(120, 80))
                {
                    EnhSelectorFrameRenderer.DrawChip(g, actionChipRect, Palette, actionChipText, chipFont, true);
                }
            }
        }

        var subHeaderText = GetMainSubHeaderText();
        if (!string.IsNullOrWhiteSpace(subHeaderText))
        {
            EnhSelectorFrameRenderer.DrawText(g, subHeaderText, subHeaderFont, Palette.AccentText, _layout.MainSubHeader, ContentAlignment.MiddleLeft, StringTrimming.EllipsisCharacter);
        }

        var chips = GetMainContextChips();
        if (chips.Count > 0)
        {
            var chipGap = ScalePaintMetric(8, 6);
            var widths = chips
                .Select(chip => Math.Max(72, ScalePaintMetric(18, 12) + TextRenderer.MeasureText(chip.Text, chipFont).Width))
                .ToArray();
            var totalWidth = widths.Sum() + chipGap * Math.Max(0, widths.Length - 1);
            var x = Math.Max(_layout.MainSubHeader.Left, _layout.MainSubHeader.Right - totalWidth);
            var availableLeft = _layout.MainSubHeader.Left + TextRenderer.MeasureText(subHeaderText, subHeaderFont).Width + ScalePaintMetric(18, 12);
            x = Math.Max(x, availableLeft);

            foreach (var chip in chips)
            {
                var chipWidth = Math.Max(72, ScalePaintMetric(18, 12) + TextRenderer.MeasureText(chip.Text, chipFont).Width);
                var chipRect = new Rectangle(x, _layout.MainSubHeader.Top, chipWidth, _layout.MainSubHeader.Height);
                EnhSelectorFrameRenderer.DrawChip(g, chipRect, Palette, chip.Text, chipFont, chip.Emphasized);
                x += chipWidth + chipGap;
                if (x >= _layout.MainSubHeader.Right)
                    break;
            }
        }

        DrawMainItems(g);
    }

    private void DrawMainItems(Graphics g)
    {
        _mainItemRects.Clear();
        var items = BuildMainItemLayouts(out var contentHeight);
        var viewport = _layout.MainViewport;
        _mainScrollOffset = ClampScrollOffset(_mainScrollOffset, contentHeight, viewport.Height);

        var clipState = g.Save();
        g.SetClip(viewport);

        foreach (var item in items)
        {
            var translated = new Rectangle(item.Bounds.X, item.Bounds.Y - _mainScrollOffset, item.Bounds.Width, item.Bounds.Height);
            if (translated.Bottom < viewport.Top || translated.Top > viewport.Bottom)
                continue;

            var isSelected = item.Index >= 0 && IsMainItemSelected(item.Index);
            var isHovered = item.Index >= 0 && _hoverMainIndex == item.Index;
            var isDisabled = item.Kind == MainItemKind.Enhancement && item.Index >= 0 && IsEnhancementGrayed(item.Index);

            if (item.Index >= 0)
            {
                _mainItemRects.Add((translated, item.Index));
            }

            EnhSelectorFrameRenderer.DrawCard(g, translated, Palette, isSelected, isHovered, isDisabled, 10);

            DrawMainItemContent(g, translated, item, isSelected, isDisabled);
        }

        g.Restore(clipState);
        _mainScrollbar = DrawScrollbar(g, ScrollArea.Main, _layout.MainScrollbarBounds, viewport.Height, contentHeight, _mainScrollOffset);
    }

    private void DrawRailPanel(Graphics g)
    {
        _railItemRects.Clear();
        var items = BuildRailItemLayouts(out var contentHeight);
        var viewport = _layout.RailViewport;
        _railScrollOffset = ClampScrollOffset(_railScrollOffset, contentHeight, viewport.Height);

        using var sectionFont = CreateScaledFont("Segoe UI", 9f, FontStyle.Bold, 7f);
        var headerRect = new Rectangle(
            _layout.RailPanel.Left + ScalePaintMetric(10, 6),
            _layout.RailPanel.Top + ScalePaintMetric(10, 6),
            _layout.RailPanel.Width - ScalePaintMetric(20, 12),
            ScalePaintMetric(22, 16));
        EnhSelectorFrameRenderer.DrawText(g, GetRailHeaderText(), sectionFont, Palette.SectionText, headerRect, ContentAlignment.MiddleCenter, StringTrimming.EllipsisCharacter);

        var clipState = g.Save();
        g.SetClip(viewport);

        foreach (var item in items)
        {
            var translated = new Rectangle(item.Bounds.X, item.Bounds.Y - _railScrollOffset, item.Bounds.Width, item.Bounds.Height);
            if (translated.Bottom < viewport.Top || translated.Top > viewport.Bottom)
                continue;

            var selected = IsRailItemSelected(item.DataIndex);
            var hovered = _hoverRailIndex == item.DataIndex;

            _railItemRects.Add((translated, item.DataIndex));
            EnhSelectorFrameRenderer.DrawCard(g, translated, Palette, selected, hovered, false, 9);
            DrawRailItemContent(g, translated, item, selected);
        }

        g.Restore(clipState);
        _railScrollbar = DrawScrollbar(g, ScrollArea.Rail, _layout.RailScrollbarBounds, viewport.Height, contentHeight, _railScrollOffset);
    }

    private void DrawInspectorPanel(Graphics g)
    {
        using var headerFont = CreateScaledFont("Segoe UI", 10.25f, FontStyle.Bold, 7.75f);
        using var titleFont = CreateScaledFont("Segoe UI", 12.5f, FontStyle.Bold, 9.5f);
        using var subtitleFont = CreateScaledFont("Segoe UI", 9.5f, FontStyle.Regular, 7.25f);
        using var sectionFont = CreateScaledFont("Segoe UI", 9.1f, FontStyle.Bold, 7f);
        using var lineFont = CreateScaledFont("Segoe UI", 8.9f, FontStyle.Regular, 6.9f);
        using var tagFont = CreateScaledFont("Segoe UI", 8.1f, FontStyle.Bold, 6.2f);

        EnhSelectorFrameRenderer.DrawText(g, "Selection Details", headerFont, Palette.SectionText, _layout.InspectorHeader, ContentAlignment.MiddleLeft, StringTrimming.EllipsisCharacter);

        var snapshot = BuildInspectorSnapshot();
        var viewport = _layout.InspectorViewport;
        var bodyWidth = Math.Max(40, viewport.Width - ScalePaintMetric(4, 2));
        var contentHeight = MeasureInspectorSnapshot(g, snapshot, bodyWidth, titleFont, subtitleFont, sectionFont, lineFont, tagFont);
        _inspectorScrollOffset = ClampScrollOffset(_inspectorScrollOffset, contentHeight, viewport.Height);

        var clipState = g.Save();
        g.SetClip(viewport);
        RenderInspectorSnapshot(g, snapshot, viewport, _inspectorScrollOffset, titleFont, subtitleFont, sectionFont, lineFont, tagFont);
        g.Restore(clipState);

        _inspectorScrollbar = DrawScrollbar(g, ScrollArea.Inspector, _layout.InspectorScrollbarBounds, viewport.Height, contentHeight, _inspectorScrollOffset);
    }

    private void DrawFooter(Graphics g)
    {
        EnhSelectorFrameRenderer.DrawPanel(g, _layout.FooterSummaryPanel, Palette, 10);
        EnhSelectorFrameRenderer.DrawPanel(g, _layout.FooterLevelPanel, Palette, 10);

        DrawFooterSummary(g);
        DrawFooterLevelPanel(g);
        DrawFooterHint(g);
    }

    private void DrawFooterSummary(Graphics g)
    {
        using var titleFont = CreateScaledFont("Segoe UI", 10.75f, FontStyle.Bold, 8.25f);
        using var bodyFont = CreateScaledFont("Segoe UI", 9.25f, FontStyle.Regular, 7.1f);
        using var detailFont = CreateScaledFont("Segoe UI", 8.75f, FontStyle.Regular, 6.8f);
        using var chipFont = CreateScaledFont("Segoe UI", 8f, FontStyle.Bold, 6.2f);

        var summary = BuildFooterSummary();
        var bounds = Rectangle.Inflate(_layout.FooterSummaryPanel, -ScalePaintMetric(12, 8), -ScalePaintMetric(8, 5));
        var iconSize = GetResponsiveSummaryIconSize(bounds, GetSummaryBandIconSize());
        var iconRect = summary.VisualKind == FooterVisualKind.None
            ? Rectangle.Empty
            : new Rectangle(bounds.Left, bounds.Top + ScalePaintMetric(4, 3), iconSize, iconSize);
        var textLeft = iconRect.IsEmpty ? bounds.Left : iconRect.Right + ScalePaintMetric(14, 10);
        var textBounds = Rectangle.FromLTRB(textLeft, bounds.Top, bounds.Right, bounds.Bottom);
        var visibleTags = summary.Tags?.Take(4).ToArray() ?? [];
        var hasTags = visibleTags.Length > 0;
        var chipHeight = hasTags ? ScalePaintMetric(17, 13) : 0;
        var chipGap = hasTags ? ScalePaintMetric(8, 6) : 0;
        var inlineChipGap = ScalePaintMetric(8, 6);
        var chipSpacing = ScalePaintMetric(6, 4);
        var titleMeasuredWidth = Math.Min(textBounds.Width, MeasureSingleLineTextWidth(g, summary.Title, titleFont) + ScalePaintMetric(2, 2));
        var inlineChipWidths = new List<(EnhSelectorChip Tag, int Width)>();
        var overflowChips = new List<EnhSelectorChip>();
        var inlineChipGroupWidth = 0;

        foreach (var tag in visibleTags)
        {
            var chipWidth = Math.Max(58, TextRenderer.MeasureText(tag.Text, chipFont).Width + ScalePaintMetric(14, 10));
            inlineChipWidths.Add((tag, chipWidth));
            inlineChipGroupWidth += chipWidth + (inlineChipWidths.Count > 1 ? chipSpacing : 0);
        }

        var inlineChips = new List<(EnhSelectorChip Tag, Rectangle Bounds)>();
        var canInlineAllChips = inlineChipWidths.Count > 0 &&
                                titleMeasuredWidth + inlineChipGap + inlineChipGroupWidth <= textBounds.Width;
        if (canInlineAllChips)
        {
            var chipX = textBounds.Left + titleMeasuredWidth + inlineChipGap;
            foreach (var (tag, width) in inlineChipWidths)
            {
                inlineChips.Add((tag, new Rectangle(chipX, 0, width, chipHeight)));
                chipX += width + chipSpacing;
            }
        }
        else
        {
            overflowChips.AddRange(visibleTags);
        }

        var hasOverflowChipRow = overflowChips.Count > 0;
        var textBottom = hasOverflowChipRow ? bounds.Bottom - chipHeight - chipGap : bounds.Bottom;
        var availableTextBounds = Rectangle.FromLTRB(textBounds.Left, textBounds.Top, textBounds.Right, textBottom);
        var lineGap = ScalePaintMetric(3, 2);
        var titleHeight = titleFont.Height + ScalePaintMetric(2, 1);
        var bodyHeight = string.IsNullOrWhiteSpace(summary.Body) ? 0 : bodyFont.Height + ScalePaintMetric(2, 1);
        var detailHeight = string.IsNullOrWhiteSpace(summary.Detail) ? 0 : detailFont.Height + ScalePaintMetric(2, 1);
        var stackedHeight = titleHeight;
        var canShowBody = bodyHeight > 0 &&
                          availableTextBounds.Height >= titleHeight + lineGap + bodyHeight;
        var canShowDetail = detailHeight > 0 &&
                            availableTextBounds.Height >=
                            (canShowBody
                                ? titleHeight + lineGap + bodyHeight + lineGap + detailHeight
                                : titleHeight + lineGap + detailHeight);

        if (canShowBody)
            stackedHeight += lineGap + bodyHeight;
        if (canShowDetail)
            stackedHeight += lineGap + detailHeight;

        var textTop = availableTextBounds.Top;
        if (iconRect.IsEmpty && !hasTags && availableTextBounds.Height > stackedHeight)
        {
            textTop += (availableTextBounds.Height - stackedHeight) / 2;
        }

        var titleRect = Rectangle.FromLTRB(
            availableTextBounds.Left,
            textTop,
            inlineChips.Count > 0
                ? Math.Max(availableTextBounds.Left, inlineChips[0].Bounds.Left - inlineChipGap)
                : availableTextBounds.Right,
            textTop + titleHeight);
        var bodyRect = Rectangle.Empty;
        var detailRect = Rectangle.Empty;
        var nextTop = titleRect.Bottom;
        if (canShowBody)
        {
            nextTop += lineGap;
            bodyRect = new Rectangle(availableTextBounds.Left, nextTop, availableTextBounds.Width, bodyHeight);
            nextTop = bodyRect.Bottom;
        }

        if (canShowDetail)
        {
            nextTop += lineGap;
            detailRect = new Rectangle(availableTextBounds.Left, nextTop, availableTextBounds.Width, Math.Min(detailHeight, Math.Max(0, availableTextBounds.Bottom - nextTop)));
        }

        if (!iconRect.IsEmpty)
        {
            DrawFooterSummaryIcon(g, iconRect, summary);
        }

        EnhSelectorFrameRenderer.DrawText(g, summary.Title, titleFont, Palette.HeaderText, titleRect, ContentAlignment.MiddleLeft, StringTrimming.EllipsisCharacter);
        if (inlineChips.Count > 0)
        {
            foreach (var (tag, inlineBounds) in inlineChips)
            {
                var placed = new Rectangle(inlineBounds.X, titleRect.Top + Math.Max(0, (titleRect.Height - chipHeight) / 2), inlineBounds.Width, chipHeight);
                EnhSelectorFrameRenderer.DrawChip(g, placed, Palette, tag, chipFont);
            }
        }

        if (!bodyRect.IsEmpty)
        {
            EnhSelectorFrameRenderer.DrawText(g, summary.Body, bodyFont, Palette.Text, bodyRect, ContentAlignment.MiddleLeft, StringTrimming.EllipsisWord);
        }

        if (!detailRect.IsEmpty)
        {
            EnhSelectorFrameRenderer.DrawText(g, summary.Detail, detailFont, Palette.AccentText, detailRect, ContentAlignment.MiddleLeft, StringTrimming.EllipsisWord);
        }

        if (hasOverflowChipRow)
        {
            var chipX = textBounds.Left;
            var chipY = bounds.Bottom - chipHeight;
            foreach (var tag in overflowChips)
            {
                var chipWidth = Math.Max(58, TextRenderer.MeasureText(tag.Text, chipFont).Width + ScalePaintMetric(14, 10));
                var chipRect = new Rectangle(chipX, chipY, chipWidth, chipHeight);
                EnhSelectorFrameRenderer.DrawChip(g, chipRect, Palette, tag, chipFont);
                chipX += chipRect.Width + chipSpacing;
                if (chipX >= bounds.Right)
                    break;
            }
        }
    }

    private void DrawFooterLevelPanel(Graphics g)
    {
        using var labelFont = CreateScaledFont("Segoe UI", 8.5f, FontStyle.Regular, 6.5f);
        using var valueFont = CreateScaledFont("Segoe UI", 16.5f, FontStyle.Bold, 10.5f);
        using var boostFont = CreateScaledFont("Segoe UI", 8.75f, FontStyle.Bold, 6.75f);
        using var buttonFont = CreateScaledFont("Segoe UI", 11.5f, FontStyle.Bold, 8f);

        var panel = _layout.FooterLevelPanel;
        var contentBounds = Rectangle.Inflate(panel, -ScalePaintMetric(8, 6), -ScalePaintMetric(6, 4));
        var hasSelection = InDesigner || HasSelectedEnhancement();
        var relValue = hasSelection ? _model.View.RelLevel.ToInt() : 0;
        var boostCount = Math.Max(0, relValue);
        var maxBoost = hasSelection ? GetMaxBoostCountForCurrentSelection() : 0;
        var level = hasSelection ? CheckAndReturnIoLevel() : 0;
        var topContentBottom = Math.Max(contentBounds.Top, _layout.FooterMinusButton.Top - ScalePaintMetric(4, 3));
        var topContentRect = Rectangle.FromLTRB(contentBounds.Left, contentBounds.Top, contentBounds.Right, topContentBottom);
        var labelHeight = ScalePaintMetric(14, 10);
        var labelRect = new Rectangle(topContentRect.Left, topContentRect.Top, topContentRect.Width, Math.Min(labelHeight, topContentRect.Height));
        var valueRect = Rectangle.FromLTRB(topContentRect.Left, labelRect.Bottom, topContentRect.Right, topContentRect.Bottom);
        var boostRect = Rectangle.FromLTRB(
            _layout.FooterMinusButton.Right + ScalePaintMetric(8, 6),
            _layout.FooterMinusButton.Top,
            _layout.FooterPlusButton.Left - ScalePaintMetric(8, 6),
            _layout.FooterMinusButton.Bottom);
        var labelColor = hasSelection ? Palette.AccentText : Palette.MutedText;
        var valueColor = hasSelection ? Palette.Text : Palette.MutedText;
        var boostColor = hasSelection ? Palette.Text : Palette.MutedText;

        EnhSelectorFrameRenderer.DrawCenteredText(g, hasSelection ? "LVL" : "N/A", labelFont, labelColor, labelRect);
        EnhSelectorFrameRenderer.DrawCenteredText(g, hasSelection ? $"{level}{GetRelativeLevelLabelForDisplay(_model.View.RelLevel)}" : "--", valueFont, valueColor, valueRect);
        EnhSelectorFrameRenderer.DrawCenteredText(g, hasSelection ? $"Boost {boostCount} / {maxBoost}" : "Boost --", boostFont, boostColor, boostRect);

        _minusHovered = _layout.FooterMinusButton.Contains(PointToClient(MousePosition)) && CanDecreaseBoost();
        _plusHovered = _layout.FooterPlusButton.Contains(PointToClient(MousePosition)) && CanIncreaseBoost();

        EnhSelectorFrameRenderer.DrawSmallButton(g, _layout.FooterMinusButton, Palette, "-", buttonFont, _minusHovered, _minusPressed, CanDecreaseBoost());
        EnhSelectorFrameRenderer.DrawSmallButton(g, _layout.FooterPlusButton, Palette, "+", buttonFont, _plusHovered, _plusPressed, CanIncreaseBoost());
    }

    private void DrawFooterHint(Graphics g)
    {
        var text = GetFooterHintText();
        if (string.IsNullOrWhiteSpace(text) || _layout.FooterHintBounds.IsEmpty)
            return;

        using var hintFont = CreateScaledFont("Segoe UI", 9.1f, FontStyle.Regular, 7.1f);
        EnhSelectorFrameRenderer.DrawText(
            g,
            text,
            hintFont,
            Palette.Text,
            _layout.FooterHintBounds,
            ContentAlignment.MiddleLeft,
            StringTrimming.EllipsisCharacter);
    }

    private void DrawHeaderIcon(Graphics g, int index, Rectangle rect)
    {
        if (!InDesigner && index == 0 && AssetManager.EmptySlot?.Bitmap is not null)
        {
            DrawEmptySlotIcon(g, rect);
            return;
        }

        if (!InDesigner &&
            AssetManager.EnhTypes.TryGetValue(index, out var iconBitmap) &&
            iconBitmap?.Bitmap is not null)
        {
            g.DrawImage(iconBitmap.Bitmap, rect);
            return;
        }

        using var fallbackFont = CreateScaledFont("Segoe UI", index == 0 ? 14f : 11f, FontStyle.Bold, 8f);
        var glyph = index switch
        {
            0 => "O",
            1 => "OR",
            2 => "IO",
            3 => "SP",
            4 => "ST",
            _ => "?"
        };

        EnhSelectorFrameRenderer.DrawCenteredText(g, glyph, fallbackFont, Palette.AccentText, rect);
    }

    private void DrawFooterSummaryIcon(Graphics g, Rectangle bounds, FooterSummary summary)
    {
        switch (summary.VisualKind)
        {
            case FooterVisualKind.Enhancement when IsValidEnhancementId(summary.DataId):
            {
                var enhancement = DatabaseAPI.Database.Enhancements[summary.DataId];
                using var attributes = GetImageAttributes(false);
                AssetManager.DrawEnhancementAt(
                    g,
                    bounds,
                    GetEnhImageIndex(summary.DataId),
                    summary.DataId,
                    enhancement.TypeID,
                    summary.EnhancementGrade ?? _model.View.GradeId,
                    attributes);
                break;
            }

            case FooterVisualKind.SetFamily when summary.DataId >= 0:
                AssetManager.DrawEnhancementSet(g, bounds, summary.DataId);
                break;

            case FooterVisualKind.SetVariant when summary.DataId >= 0 && summary.Variant.HasValue:
                AssetManager.DrawEnhancementSetVariant(g, bounds, summary.DataId, summary.Variant.Value);
                break;

            case FooterVisualKind.Empty:
                DrawEmptySlotIcon(g, bounds);
                break;
        }
    }

    private static void DrawEmptySlotIcon(Graphics g, Rectangle bounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        EnhSelectorFrameRenderer.DrawEmptyEnhancementSocket(g, bounds);
    }

    private void DrawMainItemContent(Graphics g, Rectangle bounds, MainItemLayout item, bool selected, bool disabled)
    {
        if (item.Kind == MainItemKind.Placeholder)
            return;

        using var titleFont = CreateScaledFont("Segoe UI", 8.85f, FontStyle.Bold, 7.25f);
        using var subFont = CreateScaledFont("Segoe UI", 8.35f, FontStyle.Regular, 6.9f);

        var horizontalPadding = ScalePaintMetric(10, 8);
        var topPadding = ScalePaintMetric(6, 4);
        var bottomPadding = ScalePaintMetric(6, 4);
        var iconGap = ScalePaintMetric(4, 2);
        var innerWidth = Math.Max(32, bounds.Width - horizontalPadding * 2);
        var innerHeight = Math.Max(32, bounds.Height - topPadding - bottomPadding);
        var titleBlockHeight = titleFont.Height + ScalePaintMetric(2, 1);
        var subtitleBlockHeight = subFont.Height + ScalePaintMetric(2, 1);
        var detailBlockHeight = subFont.Height + ScalePaintMetric(2, 1);
        var titleLineCount = GetMainTitleLineCount(item);
        var mandatoryTitleHeight = titleBlockHeight * titleLineCount;
        var bandTargetIconSize = GetMainGridBandIconSize();
        var iconSize = GetResponsiveGridIconSize(bounds, innerWidth, innerHeight, mandatoryTitleHeight, iconGap, bandTargetIconSize);
        var iconRect = new Rectangle(
            bounds.Left + (bounds.Width - iconSize) / 2,
            bounds.Top + topPadding,
            iconSize,
            iconSize);
        var textTop = iconRect.Bottom + iconGap;
        var textRect = Rectangle.FromLTRB(
            bounds.Left + horizontalPadding,
            textTop,
            bounds.Right - horizontalPadding,
            bounds.Bottom - bottomPadding);

        switch (item.Kind)
        {
            case MainItemKind.SetFamily:
                AssetManager.DrawEnhancementSet(g, iconRect, item.DataId);
                break;
            case MainItemKind.SetVariant:
                AssetManager.DrawEnhancementSetVariant(g, iconRect, _model.View.SetId, (SetVariantKind)item.DataId);
                break;
            case MainItemKind.Enhancement:
                if (item.DataId >= 0 && item.DataId < AssetManager.Enhancements.Count)
                {
                    using var attributes = GetImageAttributes(disabled);
                    AssetManager.DrawEnhancementAt(
                        g,
                        iconRect,
                        GetEnhImageIndex(item.DataId),
                        item.DataId,
                        _model.View.TabId,
                        _model.View.GradeId,
                        attributes);
                }

                break;
            case MainItemKind.Placeholder:
                return;
        }

        var remainingHeight = Math.Max(0, textRect.Height - mandatoryTitleHeight);
        var allowSubtitleForBand = GetScaleBand() >= 0.75f;
        if (GetScaleBand() < 1f &&
            item.Kind is MainItemKind.SetFamily or MainItemKind.SetVariant)
        {
            allowSubtitleForBand = false;
        }

        if (GetScaleBand() < 1f &&
            item.Kind == MainItemKind.Enhancement &&
            _model.View.TabId == Enums.eType.SetO)
        {
            allowSubtitleForBand = false;
        }

        var allowDetailForBand = GetScaleBand() >= 1f;
        var showSubtitle = allowSubtitleForBand &&
                           !string.IsNullOrWhiteSpace(item.Subtitle) &&
                           remainingHeight >= subtitleBlockHeight + ScalePaintMetric(2, 1);
        if (showSubtitle)
        {
            remainingHeight -= subtitleBlockHeight;
        }

        var showDetail = allowDetailForBand &&
                         !string.IsNullOrWhiteSpace(item.Detail) &&
                         remainingHeight >= detailBlockHeight + ScalePaintMetric(2, 1);
        var subtitleHeight = showSubtitle ? subtitleBlockHeight : 0;
        var titleRect = new Rectangle(textRect.Left, textRect.Top, textRect.Width, Math.Max(0, Math.Min(mandatoryTitleHeight, textRect.Height)));
        var line2Rect = new Rectangle(textRect.Left, titleRect.Bottom, textRect.Width, Math.Max(0, Math.Min(subtitleHeight, textRect.Bottom - titleRect.Bottom)));
        var line3Rect = new Rectangle(textRect.Left, line2Rect.Bottom, textRect.Width, Math.Max(0, textRect.Bottom - line2Rect.Bottom));

        var titleText = FormatMainCardTitle(item.Title, titleLineCount > 1);
        EnhSelectorFrameRenderer.DrawText(g, titleText, titleFont, selected ? Palette.Text : Palette.HeaderText, titleRect, ContentAlignment.TopCenter, StringTrimming.EllipsisCharacter);
        if (showSubtitle && !string.IsNullOrWhiteSpace(item.Subtitle))
        {
            EnhSelectorFrameRenderer.DrawText(g, item.Subtitle, subFont, Palette.Text, line2Rect, ContentAlignment.TopCenter, StringTrimming.EllipsisCharacter);
        }

        if (showDetail && !string.IsNullOrWhiteSpace(item.Detail))
        {
            var color = disabled ? Palette.WarningText : Palette.MutedText;
            EnhSelectorFrameRenderer.DrawText(g, item.Detail, subFont, color, line3Rect, ContentAlignment.TopCenter, StringTrimming.EllipsisCharacter);
        }
    }

    private void DrawRailItemContent(Graphics g, Rectangle bounds, RailItemLayout item, bool selected)
    {
        using var textFont = CreateScaledFont("Segoe UI", 8.1f, FontStyle.Bold, 6.5f);
        var horizontalPadding = ScalePaintMetric(8, 6);
        var topPadding = ScalePaintMetric(8, 5);
        var bottomPadding = ScalePaintMetric(8, 4);
        var reservedTextHeight = textFont.Height * 2 + ScalePaintMetric(8, 6);
        var iconSize = GetResponsiveRailIconSize(bounds, horizontalPadding, topPadding, bottomPadding, reservedTextHeight, GetRailBandIconSize());
        var iconRect = new Rectangle(
            bounds.Left + (bounds.Width - iconSize) / 2,
            bounds.Top + topPadding,
            iconSize,
            iconSize);
        var textRect = Rectangle.FromLTRB(bounds.Left + horizontalPadding, iconRect.Bottom + ScalePaintMetric(7, 5), bounds.Right - horizontalPadding, bounds.Bottom - bottomPadding);

        switch (_model.View.TabId)
        {
            case Enums.eType.Normal:
                if (AssetManager.EnhGrades.TryGetValue(item.IconKey, out var gradeIcon) && gradeIcon?.Bitmap is not null)
                {
                    var gradeBorder = AssetManager.ToGfxGrade(Enums.eType.Normal, (Enums.eEnhGrade)item.IconKey);
                    if (AssetManager.TryGetBorderBitmap(gradeBorder, out var borderImage) && borderImage?.Bitmap is not null)
                    {
                        g.DrawImage(borderImage.Bitmap, iconRect);
                    }

                    g.DrawImage(gradeIcon.Bitmap, iconRect);
                }

                break;

            case Enums.eType.SpecialO:
                if (AssetManager.EnhSpecials.TryGetValue(item.IconKey, out var specialIcon) && specialIcon?.Bitmap is not null)
                {
                    g.DrawImage(specialIcon.Bitmap, iconRect);
                }

                break;

            case Enums.eType.SetO:
                if (AssetManager.SetTypes.TryGetValue(item.IconKey, out var setIcon) && setIcon?.Bitmap is not null)
                {
                    g.DrawImage(setIcon.Bitmap, iconRect);
                }

                break;
        }

        var labelText = FormatCompactLabel(item.Label, allowMultiline: true, minimumLength: 10);
        EnhSelectorFrameRenderer.DrawText(g, labelText, textFont, selected ? Palette.Text : Palette.MutedText, textRect, ContentAlignment.TopCenter, StringTrimming.EllipsisWord);
    }

    private ScrollBarVisualState DrawScrollbar(Graphics g, ScrollArea area, Rectangle bounds, int viewportHeight, int contentHeight, int offset)
    {
        if (bounds.IsEmpty)
            return ScrollBarVisualState.Empty;

        var state = ScrollBarVisualState.Create(bounds, viewportHeight, contentHeight, offset);
        if (!state.Scrollable)
            return state;

        using var trackPen = new Pen(Color.FromArgb(95, Palette.Divider), 2f);
        var centerX = bounds.Left + bounds.Width / 2;
        g.DrawLine(trackPen, centerX, state.Track.Top, centerX, state.Track.Bottom);

        var hovered = area switch
        {
            ScrollArea.Main => state.Thumb.Contains(PointToClient(MousePosition)),
            ScrollArea.Rail => state.Thumb.Contains(PointToClient(MousePosition)),
            ScrollArea.Inspector => state.Thumb.Contains(PointToClient(MousePosition)),
            _ => false
        };

        using var thumbBrush = new SolidBrush(_dragScrollArea == area || hovered ? Palette.AccentText : Color.FromArgb(170, Palette.AccentText));
        g.FillRectangle(thumbBrush, state.Thumb);
        return state;
    }

    private IReadOnlyList<MainItemLayout> BuildMainItemLayouts(out int contentHeight)
    {
        var items = BuildMainItems();
        var viewport = _layout.MainViewport;
        var columns = MainGridColumns;
        var rowsVisible = MainGridVisibleRows;
        var horizontalInset = ScalePaintMetric(10, 8);
        var verticalInset = ScalePaintMetric(4, 2);
        var usableWidth = Math.Max(0, viewport.Width - horizontalInset * 2);
        var usableHeight = Math.Max(0, viewport.Height - verticalInset * 2);
        var desiredGap = GetMainGridGap();
        var minGap = ScalePaintMetric(6, 4);
        var gap = Math.Max(minGap, Math.Min(desiredGap, (usableWidth - columns * ScalePaintMetric(80, 60)) / Math.Max(1, columns - 1)));
        var rawCardWidth = (usableWidth - gap * (columns - 1)) / columns;
        var cardWidth = Math.Max(ScalePaintMetric(80, 60), rawCardWidth);
        if (cardWidth * columns + gap * (columns - 1) > usableWidth)
        {
            cardWidth = Math.Max(ScalePaintMetric(72, 56), (usableWidth - gap * (columns - 1)) / columns);
        }

        var cardHeight = Math.Max(78, (usableHeight - gap * (rowsVisible - 1)) / rowsVisible);
        var usedWidth = cardWidth * columns + gap * Math.Max(0, columns - 1);
        var originX = viewport.Left + horizontalInset + Math.Max(0, (usableWidth - usedWidth) / 2);
        var originY = viewport.Top + verticalInset;
        var layouts = new List<MainItemLayout>(items.Count);

        for (var i = 0; i < items.Count; i++)
        {
            var row = i / columns;
            var col = i % columns;
            var rect = new Rectangle(
                originX + col * (cardWidth + gap),
                originY + row * (cardHeight + gap),
                cardWidth,
                cardHeight);
            layouts.Add(items[i] with { Bounds = rect, Index = i });
        }

        var rows = items.Count == 0 ? 0 : (int)Math.Ceiling(items.Count / (double)columns);
        var measuredHeight = rows == 0 ? 0 : verticalInset * 2 + rows * cardHeight + Math.Max(0, rows - 1) * gap;
        contentHeight = Math.Max(viewport.Height, measuredHeight);
        return layouts;
    }

    private IReadOnlyList<MainItemLayout> BuildMainItems()
    {
        if (InDesigner)
        {
            return
            [
                new MainItemLayout(MainItemKind.SetFamily, 0, 0, "Decimation", "Levels 25 - 40", "6 pieces"),
                new MainItemLayout(MainItemKind.SetFamily, 1, 1, "Thunderstrike", "Levels 20 - 35", "6 pieces"),
                new MainItemLayout(MainItemKind.SetFamily, 2, 2, "Positron's Blast", "Levels 25 - 40", "6 pieces"),
                new MainItemLayout(MainItemKind.SetFamily, 3, 3, "Devastation", "Levels 22 - 35", "6 pieces")
            ];
        }

        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid)
        {
            return _model.SetIds.Select((setId, index) =>
            {
                var set = DatabaseAPI.Database.EnhancementSets[setId];
                var pieceCount = DatabaseAPI.GetEnhancementSetProjection(setId).VisiblePieces.Count;
                var range = $"{set.LevelMin + 1}-{set.LevelMax + 1}";
                return new MainItemLayout(
                    MainItemKind.SetFamily,
                    index,
                    setId,
                    set.DisplayName,
                    $"Lv {range}",
                    $"{pieceCount} Pieces");
            }).ToArray();
        }

        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid)
        {
            return _model.SetVariants.Select((variant, index) =>
                new MainItemLayout(
                    MainItemKind.SetVariant,
                    index,
                    (int)variant,
                    GetSetVariantDisplayName(variant),
                    DatabaseAPI.Database.EnhancementSets[_model.View.SetId].DisplayName,
                    $"{GetVisiblePieceCount(_model.View.SetId)} Pieces")).ToArray();
        }

        return _model.EnhancementIds.Select((enhId, index) =>
        {
            var title = GetDisplayNameForEnhancement(enhId);
            var subtitle = GetSecondaryLabelForEnhancement(enhId);
            if (string.Equals(title, subtitle, StringComparison.OrdinalIgnoreCase))
            {
                subtitle = string.Empty;
            }

            var detail = GetTertiaryLabelForEnhancement(enhId, index);
            return new MainItemLayout(MainItemKind.Enhancement, index, enhId, title, subtitle, detail);
        }).ToArray();
    }

    private IReadOnlyList<RailItemLayout> BuildRailItemLayouts(out int contentHeight)
    {
        var viewport = _layout.RailViewport;
        var items = BuildRailItems();
        var headerOffset = ScalePaintMetric(30, 22);
        var gap = ScalePaintMetric(10, 6);
        var cardHeight = Math.Max(
            Math.Max(74, ScalePaintMetric(86, 64)),
            GetUniformBandIconSize() + ScalePaintMetric(54, 38));
        var layouts = new List<RailItemLayout>(items.Count);

        for (var i = 0; i < items.Count; i++)
        {
            var rect = new Rectangle(
                viewport.Left,
                viewport.Top + headerOffset + i * (cardHeight + gap),
                viewport.Width,
                cardHeight);
            layouts.Add(items[i] with { Bounds = rect });
        }

        var measuredHeight = items.Count == 0 ? 0 : headerOffset + items.Count * cardHeight + Math.Max(0, items.Count - 1) * gap;
        contentHeight = Math.Max(viewport.Height, measuredHeight);
        return layouts;
    }

    private IReadOnlyList<RailItemLayout> BuildRailItems()
    {
        if (InDesigner)
        {
            return
            [
                new RailItemLayout(0, 0, "Ranged Damage"),
                new RailItemLayout(1, 1, "Melee Damage"),
                new RailItemLayout(2, 2, "Debuff Damage")
            ];
        }

        return _model.View.TabId switch
        {
            Enums.eType.Normal => _model.NoGrades.Select((grade, index) => new RailItemLayout(index, grade, DatabaseAPI.Database.EnhGradeStringLong[grade])).ToArray(),
            Enums.eType.SpecialO => _model.SpecialTypes.Skip(1).Select((special, index) =>
            {
                var dataIndex = index + 1;
                var specialEnh = DatabaseAPI.GetSpecialEnhByIndex(_model.SpecialTypes[dataIndex]);
                return new RailItemLayout(dataIndex, _model.SpecialTypes[dataIndex], specialEnh.Name);
            }).ToArray(),
            Enums.eType.SetO => _model.SetTypes.Select((setType, index) => new RailItemLayout(index, setType, DatabaseAPI.GetSetTypeByIndex(setType).Name)).ToArray(),
            _ => []
        };
    }

    private EnhSelectorInspectorSnapshot BuildInspectorSnapshot()
    {
        if (InDesigner)
        {
            return new EnhSelectorInspectorSnapshot
            {
                Title = "Decimation",
                Subtitle = "Ranged Damage Set",
                AccentLine = "Browse a set family, then drill into pieces without relying on popups.",
                Tags = Chips("Sets", "Persistent Details"),
                Sections =
                [
                    new EnhSelectorInspectorSection
                    {
                        Heading = "Flow",
                        Lines =
                        [
                            "Set type rail -> set family grid -> variant grid when needed -> set piece grid.",
                            "Footer keeps level and boost controls in view."
                        ]
                    }
                ]
            };
        }

        if (_hoverHeaderIndex >= 0)
            return BuildHeaderSnapshot(_hoverHeaderIndex);

        if (_hoverRailIndex >= 0)
            return BuildRailSnapshot(_hoverRailIndex);

        if (_hoverMainIndex >= 0)
            return BuildMainHoverSnapshot(_hoverMainIndex);

        if (_model.View.TabId == Enums.eType.SetO)
        {
            return _model.View.SetStage switch
            {
                SetPickerStage.SetEnhancementGrid when _model.View.PickerId >= 0 && _model.View.PickerId < _model.EnhancementIds.Length
                    => BuildEnhancementSnapshot(_model.EnhancementIds[_model.View.PickerId], _model.View.PickerId, true),
                SetPickerStage.SetVariantGrid when _model.View.SetId >= 0
                    => BuildSetVariantStageSnapshot(_model.View.SetId),
                SetPickerStage.SetFamilyGrid when _model.View.SetTypeId >= 0 && _model.View.SetTypeId < _model.SetTypes.Length
                    => BuildSetTypeSnapshot(_model.View.SetTypeId),
                _ => BuildTabSnapshot(_model.View.TabId)
            };
        }

        if (_model.View.PickerId >= 0 && _model.View.PickerId < _model.EnhancementIds.Length)
            return BuildEnhancementSnapshot(_model.EnhancementIds[_model.View.PickerId], _model.View.PickerId, true);

        return BuildTabSnapshot(_model.View.TabId);
    }

    private EnhSelectorInspectorSnapshot BuildHeaderSnapshot(int index)
    {
        var headerTag = GetHeaderCountText(index);
        return new EnhSelectorInspectorSnapshot
        {
            Title = GetHeaderHoverInfo(index),
            Subtitle = GetHeaderLabel(index),
            AccentLine = index == 0
                ? "Clear the current slot without leaving the picker."
                : HeaderIndexToType(index) switch
                {
                    Enums.eType.Normal => $"{_normalEnhs.Length} compatible origin enhancements for this power.",
                    Enums.eType.InventO => $"{_inventionEnhs.Length} compatible IO enhancements for this power.",
                    Enums.eType.SpecialO => $"{Math.Max(0, _model.SpecialTypes.Length - 1)} special enhancement categories for this power.",
                    Enums.eType.SetO => $"{_setFamilyCount} set families across {_model.SetTypes.Length} set categories.",
                    _ => string.Empty
                },
            Tags = string.IsNullOrWhiteSpace(headerTag) ? [] : Chips(headerTag),
            Sections =
            [
                new EnhSelectorInspectorSection
                {
                    Heading = "Current View",
                    Lines =
                    [
                        index == 0
                            ? "Empty-slot selection remains available until you commit or close the picker."
                            : "Select a category to refresh the main pane with matching enhancements."
                    ]
                }
            ]
        };
    }

    private EnhSelectorInspectorSnapshot BuildRailSnapshot(int dataIndex)
    {
        return _model.View.TabId switch
        {
            Enums.eType.Normal => BuildGradeSnapshot(dataIndex),
            Enums.eType.SpecialO => BuildSpecialSnapshot(dataIndex),
            Enums.eType.SetO => BuildSetTypeSnapshot(dataIndex),
            _ => BuildTabSnapshot(_model.View.TabId)
        };
    }

    private EnhSelectorInspectorSnapshot BuildGradeSnapshot(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex >= _model.NoGrades.Length)
            return BuildTabSnapshot(_model.View.TabId);

        var grade = (Enums.eEnhGrade)_model.NoGrades[dataIndex];
        var gradeName = DatabaseAPI.Database.EnhGradeStringLong[(int)grade];
        return new EnhSelectorInspectorSnapshot
        {
            Title = gradeName,
            Subtitle = "Origin Enhancement Grade",
            AccentLine = $"{_normalEnhs.Length} compatible origin enhancements at this grade.",
            Tags = Chips("Origin"),
            Sections =
            [
                new EnhSelectorInspectorSection
                {
                    Heading = "Available",
                    Lines = [$"{_normalEnhs.Length} compatible origin enhancements for this power."]
                }
            ]
        };
    }

    private EnhSelectorInspectorSnapshot BuildSpecialSnapshot(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex >= _model.SpecialTypes.Length)
            return BuildTabSnapshot(_model.View.TabId);

        var special = DatabaseAPI.GetSpecialEnhByIndex(_model.SpecialTypes[dataIndex]);
        return new EnhSelectorInspectorSnapshot
        {
            Title = special.Name,
            Subtitle = "Special Enhancement Type",
            AccentLine = special.Description,
            Tags = Chips("Special"),
            Sections =
            [
                new EnhSelectorInspectorSection
                {
                    Heading = "Available",
                    Lines = [$"{GetValidEnhancements(_powerId, Enums.eType.SpecialO, _model.SpecialTypes[dataIndex]).Count} matching enhancements for this power."]
                }
            ]
        };
    }

    private EnhSelectorInspectorSnapshot BuildSetTypeSnapshot(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex >= _model.SetTypes.Length)
            return BuildTabSnapshot(_model.View.TabId);

        var setType = _model.SetTypes[dataIndex];
        var label = DatabaseAPI.GetSetTypeByIndex(setType).Name;
        var sets = GetSets(setType)
            .Take(5)
            .Select(setId => DatabaseAPI.Database.EnhancementSets[setId].DisplayName)
            .ToArray();

        return new EnhSelectorInspectorSnapshot
        {
            Title = label,
            Subtitle = "Set Category",
            AccentLine = $"{GetSets(setType).Length} set families in this category.",
            Tags = Chips($"{GetSets(setType).Length} sets"),
            Sections =
            [
                new EnhSelectorInspectorSection
                {
                    Heading = "Examples",
                    Lines = sets.Length > 0 ? sets : ["No visible sets in this category."]
                }
            ]
        };
    }

    private EnhSelectorInspectorSnapshot BuildSetVariantStageSnapshot(int setId)
    {
        var set = DatabaseAPI.Database.EnhancementSets[setId];
        return new EnhSelectorInspectorSnapshot
        {
            Title = set.DisplayName,
            Subtitle = DatabaseAPI.GetSetTypeByIndex(set.SetType).Name,
            AccentLine = $"{GetOrderedSetVariants(setId).Length} variants available for this set.",
            Tags = Chips(GetOrderedSetVariants(setId).Select(GetSetVariantDisplayName).ToArray()),
            Sections =
            [
                new EnhSelectorInspectorSection
                {
                    Heading = "Level Range",
                    Lines = [$"{set.LevelMin + 1} to {set.LevelMax + 1}"]
                }
            ]
        };
    }

    private EnhSelectorInspectorSnapshot BuildMainHoverSnapshot(int index)
    {
        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid)
        {
            if (index >= 0 && index < _model.SetIds.Length)
                return BuildSetFamilySnapshot(_model.SetIds[index]);
        }

        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid)
        {
            if (_model.View.SetId >= 0 && index >= 0 && index < _model.SetVariants.Length)
                return BuildVariantSnapshot(_model.View.SetId, _model.SetVariants[index]);
        }

        if (index >= 0 && index < _model.EnhancementIds.Length)
            return BuildEnhancementSnapshot(_model.EnhancementIds[index], index, false);

        return BuildTabSnapshot(_model.View.TabId);
    }

    private EnhSelectorInspectorSnapshot BuildSetFamilySnapshot(int setId)
    {
        var set = DatabaseAPI.Database.EnhancementSets[setId];
        var projection = DatabaseAPI.GetEnhancementSetProjection(setId);
        var variants = GetOrderedSetVariants(setId).Select(GetSetVariantDisplayName).ToArray();
        var bonusesSection = BuildStandardBonusSection(setId, set, maxLines: 3);

        var sections = new List<EnhSelectorInspectorSection>();
        if (bonusesSection is not null)
        {
            sections.Add(bonusesSection);
        }
        else
        {
            sections.Add(new EnhSelectorInspectorSection
            {
                Heading = "Set Bonuses",
                Lines = ["No set bonuses available."]
            });
        }

        return new EnhSelectorInspectorSnapshot
        {
            Title = set.DisplayName,
            Subtitle = DatabaseAPI.GetSetTypeByIndex(set.SetType).Name,
            AccentLine = $"Levels {set.LevelMin + 1}-{set.LevelMax + 1} | {projection.VisiblePieces.Count} pieces",
            Tags = variants.Length > 0 ? Chips(variants) : Chips("No variants"),
            Sections = sections
        };
    }

    private EnhSelectorInspectorSnapshot BuildVariantSnapshot(int setId, SetVariantKind variantKind)
    {
        var set = DatabaseAPI.Database.EnhancementSets[setId];
        return new EnhSelectorInspectorSnapshot
        {
            Title = GetSetVariantDisplayName(variantKind),
            Subtitle = set.DisplayName,
            AccentLine = $"{DatabaseAPI.GetSetTypeByIndex(set.SetType).Name} | {FormatVisiblePieceCount(setId)}",
            Tags = Chips(DatabaseAPI.GetSetTypeByIndex(set.SetType).Name),
            Sections =
            [
                new EnhSelectorInspectorSection
                {
                    Heading = "Level Range",
                    Lines = [$"{set.LevelMin + 1} to {set.LevelMax + 1}"]
                },
                new EnhSelectorInspectorSection
                {
                    Heading = "Pieces",
                    Lines = [FormatVisiblePieceCount(setId)]
                }
            ]
        };
    }

    private EnhSelectorInspectorSnapshot BuildEnhancementSnapshot(int enhId, int index, bool fromSelection)
    {
        if (!IsValidEnhancementId(enhId))
            return BuildTabSnapshot(_model.View.TabId);

        var enhancement = DatabaseAPI.Database.Enhancements[enhId];
        var title = GetDisplayNameForEnhancement(enhId);
        var subtitle = GetInspectorSubtitleForEnhancement(enhId);
        var description = GetResolvedEnhancementDescription(enhId);
        var scheduleSummary = BuildScheduleSummary(enhId);
        var tags = new List<EnhSelectorChip>();

        if (TryGetEnhancementRarityChip(enhId, out var rarityChip))
            tags.Add(rarityChip);

        if (enhancement.Unique)
            tags.Add(Chip("Unique"));

        if (_model.HasCatalyst(enhId))
            tags.Add(Chip("Catalyst"));

        if (_model.IsNaturallyAttuned(enhId) || (enhancement.TypeID == Enums.eType.SetO && DatabaseAPI.IsAttunedSetVariant(enhId)))
            tags.Add(Chip("Attuned"));

        if (tags.Count == 0 && TryGetEnhancementDisplayLevelRange(enhId, out var ioMin, out var ioMax))
            tags.Add(Chip($"Lv {ioMin}-{ioMax}"));

        var sections = new List<EnhSelectorInspectorSection>();
        var detailLines = new List<string>();

        if (TryGetEnhancementRarityLabel(enhId, out var rarityLabel))
            detailLines.Add($"Rarity: {rarityLabel}");

        if (TryGetEnhancementDisplayLevelRange(enhId, out var detailMin, out var detailMax))
            detailLines.Add($"Level Range: {detailMin}-{detailMax}");

        if (!string.IsNullOrWhiteSpace(scheduleSummary))
            detailLines.Add(scheduleSummary);

        if (detailLines.Count > 0)
        {
            sections.Add(new EnhSelectorInspectorSection
            {
                Heading = "Details",
                Lines = detailLines
            });
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            sections.Add(new EnhSelectorInspectorSection
            {
                Heading = "Description",
                Lines = [description]
            });
        }

        if (enhancement.TypeID == Enums.eType.SetO && enhancement.nIDSet >= 0)
        {
            sections.AddRange(BuildSetEnhancementSections(enhId));
        }

        var disabledReason = GetDisabledReason(index);
        if (!string.IsNullOrWhiteSpace(disabledReason))
        {
            sections.Add(new EnhSelectorInspectorSection
            {
                Heading = "Unavailable",
                Lines = [disabledReason],
                IsWarning = true
            });
        }

        return new EnhSelectorInspectorSnapshot
        {
            Title = title,
            Subtitle = subtitle,
            AccentLine = BuildEnhancementInspectorAccentLine(enhId, index),
            Note = string.Empty,
            Tags = tags,
            Sections = sections
        };
    }

    private IReadOnlyList<EnhSelectorInspectorSection> BuildSetEnhancementSections(int enhId)
    {
        var setId = DatabaseAPI.Database.Enhancements[enhId].nIDSet;
        var set = DatabaseAPI.Database.EnhancementSets[setId];
        var sections = new List<EnhSelectorInspectorSection>();
        var overviewLines = new List<string>
        {
            $"Set Type: {DatabaseAPI.GetSetTypeByIndex(set.SetType).Name}",
            $"Set Level Range: {set.LevelMin + 1} to {set.LevelMax + 1}"
        };

        if (DatabaseAPI.TryGetSetPieceIndexForEnhancement(enhId, out _, out var pieceIndex))
        {
            overviewLines.Add($"Piece: {pieceIndex + 1} of {DatabaseAPI.GetEnhancementSetProjection(setId).VisiblePieces.Count}");
        }

        sections.Add(new EnhSelectorInspectorSection
        {
            Heading = "Set Overview",
            Lines = overviewLines
        });

        var pieceLines = DatabaseAPI.GetEnhancementSetProjection(setId).VisiblePieces
            .Select(piece => piece.DisplayLabel)
            .ToArray();
        sections.Add(new EnhSelectorInspectorSection
        {
            Heading = "Set Pieces",
            Lines = pieceLines
        });

        var bonusesSection = BuildStandardBonusSection(setId, set, maxLines: int.MaxValue);
        if (bonusesSection is not null)
        {
            sections.Add(bonusesSection);
        }

        if (DatabaseAPI.TryGetSetPieceIndexForEnhancement(enhId, out _, out var specialPieceIndex))
        {
            var specialLines = EnhancementSetSpecialBonusDisplay.BuildRows(set, setId)
                .Where(row => row.PieceIndexes.Contains(specialPieceIndex))
                .SelectMany(row => row.EffectStrings.Select(effect => row.PvMode is Enums.ePvX.PvP ? $"{effect} (PVP)" : effect))
                .ToArray();
            if (specialLines.Length > 0)
            {
                sections.Add(new EnhSelectorInspectorSection
                {
                    Heading = "Enhancement Bonuses",
                    Lines = specialLines
                });
            }
        }

        return sections;
    }

    private EnhSelectorInspectorSnapshot BuildTabSnapshot(Enums.eType tabId)
    {
        var tabTag = GetHeaderCountText(TypeToHeaderIndex(tabId));
        var title = tabId switch
        {
            Enums.eType.None => "Empty Slot",
            Enums.eType.Normal => "Origin Enhancements",
            Enums.eType.InventO => "Invention Origin Enhancements",
            Enums.eType.SpecialO => "Special Enhancements",
            Enums.eType.SetO => "Invention Sets",
            _ => "Enhancements"
        };

        var countLine = tabId switch
        {
            Enums.eType.Normal => $"{_normalEnhs.Length} compatible origin enhancements.",
            Enums.eType.InventO => $"{_inventionEnhs.Length} compatible IO enhancements.",
            Enums.eType.SpecialO => $"{Math.Max(0, _model.SpecialTypes.Length - 1)} special enhancement categories.",
            Enums.eType.SetO => $"{_setFamilyCount} set families across {_model.SetTypes.Length} categories.",
            _ => "Select Empty, then double-click it to clear the slot."
        };

        return new EnhSelectorInspectorSnapshot
        {
            Title = title,
            Subtitle = tabId == Enums.eType.None ? "Clear the current slot" : "Current category",
            AccentLine = tabId switch
            {
                Enums.eType.None => "The picker stays open until you close it or commit an empty slot.",
                Enums.eType.SetO => $"{_setFamilyCount} set families across {_model.SetTypes.Length} set categories.",
                Enums.eType.SpecialO => $"{Math.Max(0, _model.SpecialTypes.Length - 1)} special enhancement categories for this power.",
                Enums.eType.InventO => $"{_inventionEnhs.Length} compatible IO enhancements for this power.",
                Enums.eType.Normal => $"{_normalEnhs.Length} compatible origin enhancements for this power.",
                _ => "Browse compatible enhancements for this power."
            },
            Tags = string.IsNullOrWhiteSpace(tabTag) ? [] : Chips(tabTag),
            Sections =
            [
                new EnhSelectorInspectorSection
                {
                    Heading = "Current View",
                    Lines = [countLine]
                },
                new EnhSelectorInspectorSection
                {
                    Heading = tabId == Enums.eType.SetO ? "Set Flow" : "Selection",
                    Lines =
                    [
                        tabId switch
                        {
                            Enums.eType.None => "Double-click Empty or press Enter to clear the current slot.",
                            Enums.eType.SetO => "Choose a set type, open a set family, then select a final set piece to commit.",
                            _ => "Single-click selects an enhancement. Double-click or press Enter to commit the current selection."
                        }
                    ]
                }
            ]
        };
    }

    private FooterSummary BuildFooterSummary()
    {
        if (InDesigner)
        {
            return new FooterSummary(
                "Decimation: Accuracy/Damage",
                "Set | Ranged Damage | Decimation | Crafted",
                "Levels 25-40",
                FooterVisualKind.Enhancement,
                0,
                null,
                Enums.eEnhGrade.SingleO,
                Chips("SmashingDmg", "LethalDmg"));
        }

        if (_hoverMainIndex >= 0)
        {
            if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid && _hoverMainIndex < _model.SetIds.Length)
            {
                var setId = _model.SetIds[_hoverMainIndex];
                var set = DatabaseAPI.Database.EnhancementSets[setId];
                return new FooterSummary(
                    set.DisplayName,
                    $"{DatabaseAPI.GetSetTypeByIndex(set.SetType).Name} | Levels {set.LevelMin + 1}-{set.LevelMax + 1}",
                    $"{GetVisiblePieceCount(setId)} visible pieces",
                    FooterVisualKind.SetFamily,
                    setId,
                    null,
                    null,
                    Chips($"{DatabaseAPI.GetEnhancementSetProjection(setId).VisiblePieces.Count} pieces"));
            }

            if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid && _model.View.SetId >= 0 && _hoverMainIndex < _model.SetVariants.Length)
            {
                var variant = _model.SetVariants[_hoverMainIndex];
                return new FooterSummary(
                    GetSetVariantDisplayName(variant),
                    DatabaseAPI.Database.EnhancementSets[_model.View.SetId].DisplayName,
                    $"{FormatVisiblePieceCount(_model.View.SetId)}",
                    FooterVisualKind.SetVariant,
                    _model.View.SetId,
                    variant,
                    null,
                    Chips(FormatVisiblePieceCount(_model.View.SetId)));
            }

            if (_hoverMainIndex < _model.EnhancementIds.Length)
            {
                var enhId = _model.EnhancementIds[_hoverMainIndex];
                return BuildEnhancementFooterSummary(enhId, selected: false);
            }
        }

        if (_model.View.PickerId >= 0 && _model.View.PickerId < _model.EnhancementIds.Length)
        {
            var enhId = _model.EnhancementIds[_model.View.PickerId];
            return BuildEnhancementFooterSummary(enhId, selected: true);
        }

        return _model.View.TabId switch
        {
            Enums.eType.SetO when _model.View.SetStage == SetPickerStage.SetFamilyGrid && _model.View.SetTypeId < 0 => new FooterSummary("Browse set families", $"{_setFamilyCount} set families are available across {_model.SetTypes.Length} categories.", "Choose a set category to continue browsing."),
            Enums.eType.SetO when _model.View.SetStage == SetPickerStage.SetFamilyGrid => new FooterSummary("Browse set families", $"{_model.SetIds.Length} families are available in the selected set category.", "Preview pieces and bonuses for any matching set family."),
            Enums.eType.SetO when _model.View.SetStage == SetPickerStage.SetVariantGrid && _model.View.SetId >= 0 => new FooterSummary("Choose a variant", $"{GetOrderedSetVariants(_model.View.SetId).Length} variants are available for this set.", "Open a variant to view the final piece grid."),
            Enums.eType.SetO when _model.View.SetStage == SetPickerStage.SetVariantGrid => new FooterSummary("Choose a variant", "Set variants are available for the current set.", "Open a variant to view the final piece grid."),
            Enums.eType.SetO when _model.View.SetId >= 0 => new FooterSummary("Browse set pieces", $"{GetVisiblePieceCount(_model.View.SetId)} visible pieces are available in this set.", "Inspect a piece before committing it."),
            Enums.eType.SetO => new FooterSummary("Browse set pieces", "Set pieces are available for the current selection.", "Inspect pieces from the current set."),
            Enums.eType.None => new FooterSummary("Empty Slot", "Current slot is ready to clear.", "Preview the empty-slot state before committing it.", FooterVisualKind.Empty),
            Enums.eType.SpecialO => new FooterSummary("Browse special enhancements", $"{Math.Max(0, _model.SpecialTypes.Length - 1)} special categories are available for this power.", "Choose a category or enhancement to view its details."),
            Enums.eType.InventO => new FooterSummary("Browse IO enhancements", $"{_inventionEnhs.Length} compatible IO enhancements are available for this power.", "Inspect a compatible IO enhancement."),
            _ => new FooterSummary("Browse enhancements", $"{_normalEnhs.Length} compatible origin enhancements are available for this power.", "Inspect a compatible enhancement.")
        };
    }

    private FooterSummary BuildEnhancementFooterSummary(int enhId, bool selected)
    {
        var title = GetDisplayNameForEnhancement(enhId);
        var body = BuildFooterBodyForEnhancement(enhId);
        var detail = BuildFooterDetailForEnhancement(enhId, selected);
        var tags = BuildFooterTagsForEnhancement(enhId);
        return new FooterSummary(
            title,
            body,
            detail,
            FooterVisualKind.Enhancement,
            enhId,
            null,
            _model.View.GradeId,
            tags);
    }

    private string BuildFooterBodyForEnhancement(int enhId)
    {
        if (!IsValidEnhancementId(enhId))
            return string.Empty;

        var enhancement = DatabaseAPI.Database.Enhancements[enhId];
        return enhancement.TypeID switch
        {
            Enums.eType.SetO when enhancement.nIDSet >= 0
                => $"Set | {DatabaseAPI.GetSetTypeByIndex(DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].SetType).Name} | {DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName}",
            Enums.eType.SpecialO
                => $"Special | {GetDisplayNameForEnhancement(enhId)}",
            Enums.eType.InventO
                => $"IO | {GetSecondaryLabelForEnhancement(enhId)}",
            Enums.eType.Normal
                => $"Origin | {DatabaseAPI.Database.EnhGradeStringLong[(int)_model.View.GradeId]}",
            _ => GetSecondaryLabelForEnhancement(enhId)
        };
    }

    private string BuildFooterDetailForEnhancement(int enhId, bool selected)
    {
        if (!IsValidEnhancementId(enhId))
            return string.Empty;

        var index = Array.IndexOf(_model.EnhancementIds, enhId);
        var disabledReason = GetDisabledReason(index);
        if (!string.IsNullOrWhiteSpace(disabledReason))
            return disabledReason;

        if (TryGetEnhancementDisplayLevelRange(enhId, out var ioMin, out var ioMax))
        {
            if (_model.View.TabId is Enums.eType.InventO or Enums.eType.SetO)
                return $"Levels {ioMin}-{ioMax} | Invention Level: {CheckAndReturnIoLevel(enhId)}";

            return $"Levels {ioMin}-{ioMax}";
        }

        return string.Empty;
    }

    private string GetFooterHintText()
    {
        return _model.View.TabId switch
        {
            Enums.eType.None => "Single-click Empty to inspect. Double-click Empty to clear the slot.",
            Enums.eType.SetO when _model.View.SetStage == SetPickerStage.SetFamilyGrid && _model.View.SetTypeId < 0
                => "Choose a set category to continue browsing matching set families.",
            Enums.eType.SetO when _model.View.SetStage == SetPickerStage.SetFamilyGrid
                => "Single-click a set family to inspect it and continue into its pieces.",
            Enums.eType.SetO when _model.View.SetStage == SetPickerStage.SetVariantGrid
                => "Single-click a variant to open its set pieces.",
            Enums.eType.SetO
                => "Single-click to inspect. Double-click to slot. Esc to cancel.",
            Enums.eType.SpecialO or Enums.eType.InventO or Enums.eType.Normal
                => "Single-click to inspect. Double-click to slot. Esc to cancel.",
            _ => string.Empty
        };
    }

    private IReadOnlyList<EnhSelectorChip> BuildFooterTagsForEnhancement(int enhId)
    {
        var tags = new List<EnhSelectorChip>();
        var enhancement = IsValidEnhancementId(enhId) ? DatabaseAPI.Database.Enhancements[enhId] : null;

        if (TryGetEnhancementRarityChip(enhId, out var rarityChip))
        {
            tags.Add(rarityChip);
        }

        if (enhancement is not null && enhancement.Unique)
        {
            tags.Add(Chip("Unique"));
        }

        if (_model.HasCatalyst(enhId))
        {
            tags.Add(Chip("Catalyst"));
        }

        if (_model.IsNaturallyAttuned(enhId) || (IsValidEnhancementId(enhId) && DatabaseAPI.IsAttunedSetVariant(enhId)))
        {
            tags.Add(Chip("Attuned"));
        }

        if (tags.Count == 0 && TryGetEnhancementDisplayLevelRange(enhId, out var ioMin, out var ioMax))
        {
            tags.Add(Chip($"Levels {ioMin}-{ioMax}"));
        }

        return tags;
    }

    private string GetInspectorSubtitleForEnhancement(int enhId)
    {
        if (!IsValidEnhancementId(enhId))
            return string.Empty;

        var enhancement = DatabaseAPI.Database.Enhancements[enhId];
        var title = GetDisplayNameForEnhancement(enhId);
        var subtitle = enhancement.TypeID switch
        {
            Enums.eType.SetO when enhancement.nIDSet >= 0 => DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName,
            Enums.eType.SpecialO => DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID).Name,
            Enums.eType.InventO => "Invention Origin",
            Enums.eType.Normal => DatabaseAPI.Database.EnhGradeStringLong[(int)_model.View.GradeId],
            _ => string.Empty
        };

        return string.Equals(subtitle, title, StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : subtitle;
    }

    private bool TryGetEnhancementRarityChip(int enhId, out EnhSelectorChip chip)
    {
        chip = Chip(string.Empty);
        if (!TryGetEnhancementRarityLabel(enhId, out var rarityLabel, out var rarity))
            return false;

        chip = Chip(rarityLabel, style: GetChipStyleForRarity(rarity));
        return true;
    }

    private bool TryGetEnhancementRarityLabel(int enhId, out string rarityLabel)
    {
        if (TryGetEnhancementRarityLabel(enhId, out rarityLabel, out _))
            return true;

        rarityLabel = string.Empty;
        return false;
    }

    private bool TryGetEnhancementRarityLabel(int enhId, out string rarityLabel, out Recipe.RecipeRarity rarity)
    {
        rarityLabel = string.Empty;
        rarity = Recipe.RecipeRarity.Common;

        if (!IsValidEnhancementId(enhId) || !DatabaseAPI.TryGetEnhancementResolvedRarity(enhId, out rarity))
            return false;

        rarityLabel = FormatRarityLabel(rarity);
        return true;
    }

    private static string FormatRarityLabel(Recipe.RecipeRarity rarity)
    {
        return rarity switch
        {
            Recipe.RecipeRarity.UltraRare => "Ultra Rare",
            _ => rarity.ToString()
        };
    }

    private static EnhSelectorChipStyle GetChipStyleForRarity(Recipe.RecipeRarity rarity)
    {
        return rarity switch
        {
            Recipe.RecipeRarity.Uncommon => EnhSelectorChipStyle.RarityUncommon,
            Recipe.RecipeRarity.Rare => EnhSelectorChipStyle.RarityRare,
            Recipe.RecipeRarity.UltraRare => EnhSelectorChipStyle.RarityUltraRare,
            _ => EnhSelectorChipStyle.RarityCommon
        };
    }

    private static EnhSelectorChip Chip(string text, bool emphasized = false, EnhSelectorChipStyle style = EnhSelectorChipStyle.Default)
    {
        return new EnhSelectorChip
        {
            Text = text,
            Emphasized = emphasized,
            Style = style
        };
    }

    private static IReadOnlyList<EnhSelectorChip> Chips(params string[] texts)
    {
        return texts
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .Select(text => Chip(text))
            .ToArray();
    }

    private string BuildEnhancementInspectorAccentLine(int enhId, int index)
    {
        var disabledReason = GetDisabledReason(index);
        if (!string.IsNullOrWhiteSpace(disabledReason))
            return disabledReason;

        if (_model.View.TabId is Enums.eType.InventO or Enums.eType.SetO)
        {
            return $"Invention Level: {CheckAndReturnIoLevel(enhId)}";
        }

        if (_model.View.TabId == Enums.eType.Normal)
        {
            return $"Grade: {DatabaseAPI.Database.EnhGradeStringLong[(int)_model.View.GradeId]}";
        }

        if (TryGetEnhancementDisplayLevelRange(enhId, out var ioMin, out var ioMax))
            return $"Levels {ioMin}-{ioMax}";

        return string.Empty;
    }

    private string GetMainHeaderText()
    {
        if (InDesigner)
            return "Set: Decimation [ 0 / 6 ]";

        if (_model.View.TabId == Enums.eType.SetO)
        {
            return _model.View.SetStage switch
                {
                SetPickerStage.SetFamilyGrid when _model.View.SetTypeId >= 0 && _model.View.SetTypeId < _model.SetTypes.Length
                    => $"Set Type: {DatabaseAPI.GetSetTypeByIndex(_model.SetTypes[_model.View.SetTypeId]).Name}",
                SetPickerStage.SetVariantGrid when _model.View.SetId >= 0
                    => $"Set: {DatabaseAPI.Database.EnhancementSets[_model.View.SetId].DisplayName}",
                SetPickerStage.SetEnhancementGrid when _model.View.SetId >= 0 && _model.View.SetTypeId >= 0 && _model.View.SetTypeId < _model.SetTypes.Length
                    => $"Set: {DatabaseAPI.Database.EnhancementSets[_model.View.SetId].DisplayName} [ {GetSlottedPieceCountForSet(_model.View.SetId)} / {GetVisiblePieceCount(_model.View.SetId)} ]",
                _ => "Set Enhancements"
            };
        }

        return GetHeaderHoverInfo(TypeToHeaderIndex(_model.View.TabId));
    }

    private string GetMainSubHeaderText()
    {
        if (InDesigner)
            return "6 visible pieces in this set.";

        if (_model.View.TabId == Enums.eType.SetO)
        {
            return _model.View.SetStage switch
            {
                SetPickerStage.SetFamilyGrid when _model.View.SetTypeId < 0 => $"{_setFamilyCount} set families available. Please choose a category.",
                SetPickerStage.SetFamilyGrid => $"{_model.SetIds.Length} set families available in this category.",
                SetPickerStage.SetVariantGrid when _model.View.SetId >= 0 => $"{GetOrderedSetVariants(_model.View.SetId).Length} variants available for this set.",
                SetPickerStage.SetEnhancementGrid when _model.View.SetId >= 0 && _model.View.SetVariant.HasValue
                    => $"{DatabaseAPI.GetSetTypeByIndex(DatabaseAPI.Database.EnhancementSets[_model.View.SetId].SetType).Name} | Variant: {GetSetVariantDisplayName(_model.View.SetVariant.Value)}",
                SetPickerStage.SetEnhancementGrid when _model.View.SetId >= 0
                    => $"{DatabaseAPI.GetSetTypeByIndex(DatabaseAPI.Database.EnhancementSets[_model.View.SetId].SetType).Name} | {GetVisiblePieceCount(_model.View.SetId)} visible pieces",
                _ => string.Empty
            };
        }

        return _model.View.TabId switch
        {
            Enums.eType.None => "Empty slot selection remains open until you commit or close it.",
            Enums.eType.Normal => $"{_normalEnhs.Length} compatible origin enhancements for this power.",
            Enums.eType.InventO => $"{_inventionEnhs.Length} compatible IO enhancements for this power.",
            Enums.eType.SpecialO => $"{Math.Max(0, _model.SpecialTypes.Length - 1)} special enhancement categories for this power.",
            _ => string.Empty
        };
    }

    private List<(string Text, bool Emphasized)> GetMainContextChips()
    {
        return [];
    }

    private string GetMainHeaderActionChipText()
    {
        if (InDesigner)
            return "Attuned";

        if (_model.View.TabId == Enums.eType.SetO &&
            _model.View.SetStage == SetPickerStage.SetEnhancementGrid &&
            _model.View.SetVariant.HasValue)
        {
            return GetSetVariantDisplayName(_model.View.SetVariant.Value);
        }

        return string.Empty;
    }

    private string GetRailHeaderText()
    {
        if (InDesigner)
            return "Set Type";

        return _model.View.TabId switch
        {
            Enums.eType.Normal => "Grade",
            Enums.eType.SpecialO => "Special Type",
            Enums.eType.SetO => "Set Type",
            _ => "Filter"
        };
    }

    private bool ShouldShowBackButton()
    {
        return _model.View.TabId == Enums.eType.SetO &&
               (_model.View.SetStage != SetPickerStage.SetFamilyGrid || _model.View.SetTypeId > -1);
    }

    private string GetHeaderCountText(int index)
    {
        return index switch
        {
            0 => string.Empty,
            1 => $"({_normalEnhs.Length})",
            2 => $"({_inventionEnhs.Length})",
            3 => $"({Math.Max(0, _model.SpecialTypes.Length - 1)})",
            4 => $"({_setFamilyCount})",
            _ => string.Empty
        };
    }

    private static string GetHeaderLabel(int index)
    {
        return index switch
        {
            0 => "Empty",
            1 => "Origin",
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
            Enums.eType.None => "Empty Slot",
            Enums.eType.Normal => "Origin Enhancements",
            Enums.eType.InventO => "Invention Origin (IO)",
            Enums.eType.SpecialO => "Special Enhancements",
            Enums.eType.SetO => "Invention Sets",
            _ => string.Empty
        };
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

    private static int TypeToHeaderIndex(Enums.eType type)
    {
        return type switch
        {
            Enums.eType.None => 0,
            Enums.eType.Normal => 1,
            Enums.eType.InventO => 2,
            Enums.eType.SpecialO => 3,
            Enums.eType.SetO => 4,
            _ => 1
        };
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
            _lastTab = newTabId;

        if (newTabId == Enums.eType.None)
        {
            ResetSetSelection();
            _model.View.PickerId = -1;
            _mainScrollOffset = 0;
            _railScrollOffset = 0;
            _inspectorScrollOffset = 0;
            Invalidate();
            return;
        }

        ResetSetSelection();
        _hoverRailIndex = -1;
        _mainScrollOffset = 0;
        _railScrollOffset = 0;
        _inspectorScrollOffset = 0;
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

        _mainScrollOffset = 0;
        _inspectorScrollOffset = 0;
        Invalidate();
    }

    private void HandleMainItemClick(int index)
    {
        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid)
        {
            if (index < 0 || index >= _model.SetIds.Length)
                return;

            _model.View.SetId = _model.SetIds[index];
            _model.View.SetVariant = null;
            OpenSelectedSetFamily(-1);
            _mainScrollOffset = 0;
            _inspectorScrollOffset = 0;
            Invalidate();
            return;
        }

        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid)
        {
            if (index < 0 || index >= _model.SetVariants.Length)
                return;

            _model.View.SetVariant = _model.SetVariants[index];
            SetActiveEnhancements(_powerId, -1, _normalEnhs, _inventionEnhs);
            _mainScrollOffset = 0;
            _inspectorScrollOffset = 0;
            Invalidate();
            return;
        }

        if (index < 0 || index >= _model.EnhancementIds.Length)
            return;

        _model.View.PickerId = index;
        Invalidate();
    }

    private void HandleMainItemDoubleClick(int index)
    {
        if (index < 0 || index >= _model.EnhancementIds.Length)
            return;

        if (IsEnhancementGrayed(index))
        {
            _model.View.PickerId = index;
            Invalidate();
            return;
        }

        _model.View.PickerId = index;
        CommitEnhancementSelection(index);
    }

    private bool HandleMainItemEnter(int index, bool commitFinalSelection)
    {
        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid)
        {
            if (index < 0 || index >= _model.SetIds.Length)
                return false;

            HandleMainItemClick(index);
            return true;
        }

        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid)
        {
            if (index < 0 || index >= _model.SetVariants.Length)
                return false;

            HandleMainItemClick(index);
            return true;
        }

        if (index < 0 || index >= _model.EnhancementIds.Length)
            return false;

        _model.View.PickerId = index;
        if (commitFinalSelection)
        {
            return CommitEnhancementSelection(index);
        }

        Invalidate();
        return true;
    }

    private bool CommitEnhancementSelection(int index)
    {
        if (index < 0 || index >= _model.EnhancementIds.Length)
            return false;

        var enhId = _model.EnhancementIds[index];
        var slot = CreateSlotForEnh(enhId, index);
        if (slot is null)
        {
            Invalidate();
            return false;
        }

        PersistLastStateForCurrentView();
        EnhancementPicked?.Invoke(slot);
        return true;
    }

    private void CommitEmptySelection()
    {
        EnhancementPicked?.Invoke(new I9Slot());
    }

    private bool TryHandleScrollbarMouseDown(MouseEventArgs e)
    {
        var target = GetScrollbarAreaAtPoint(e.Location);
        if (target == ScrollArea.None)
            return false;

        Focus();
        var state = GetScrollbarState(target);
        if (!state.Scrollable)
            return true;

        if (state.Thumb.Contains(e.Location))
        {
            _dragScrollArea = target;
            _dragThumbGrabOffsetY = e.Y - state.Thumb.Y;
            Capture = true;
            Invalidate(state.Bounds);
            return true;
        }

        if (state.Track.Contains(e.Location))
        {
            var delta = e.Y < state.Thumb.Top ? -state.ViewportHeight : state.ViewportHeight;
            AdjustScroll(target, delta);
            return true;
        }

        return true;
    }

    private void DragScrollbar(int mouseY)
    {
        var state = GetScrollbarState(_dragScrollArea);
        if (!state.Scrollable)
            return;

        var available = Math.Max(0, state.Track.Height - state.Thumb.Height);
        var maxScroll = Math.Max(0, state.ContentHeight - state.ViewportHeight);
        if (available <= 0 || maxScroll <= 0)
            return;

        var newThumbY = mouseY - _dragThumbGrabOffsetY;
        newThumbY = Math.Max(state.Track.Top, Math.Min(newThumbY, state.Track.Top + available));
        var ratio = (double)(newThumbY - state.Track.Top) / available;
        var offset = (int)Math.Round(ratio * maxScroll);
        SetScrollOffset(_dragScrollArea, offset);
        Invalidate();
    }

    private bool UpdateScrollbarHover(Point pt)
    {
        var area = GetScrollbarAreaAtPoint(pt);
        if (area == ScrollArea.None)
            return false;

        Cursor = Cursors.Hand;
        if (_hoverHeaderIndex != -1 || _hoverMainIndex != -1 || _hoverRailIndex != -1)
        {
            _hoverHeaderIndex = -1;
            _hoverMainIndex = -1;
            _hoverRailIndex = -1;
            Invalidate();
        }

        return true;
    }

    private void UpdateButtonHoverStates(Point pt)
    {
        var oldClose = _closeHovered;
        var oldBack = _backHovered;
        var oldMinus = _minusHovered;
        var oldPlus = _plusHovered;

        _closeHovered = _layout.CloseButton.Contains(pt);
        _backHovered = ShouldShowBackButton() && _layout.BackButton.Contains(pt);
        _minusHovered = _layout.FooterMinusButton.Contains(pt) && CanDecreaseBoost();
        _plusHovered = _layout.FooterPlusButton.Contains(pt) && CanIncreaseBoost();

        if (oldClose != _closeHovered || oldBack != _backHovered || oldMinus != _minusHovered || oldPlus != _plusHovered)
            Invalidate();
    }

    private ScrollArea ResolveScrollArea(Point pt)
    {
        if (_layout.MainViewport.Contains(pt) || _mainScrollbar.Bounds.Contains(pt))
            return ScrollArea.Main;
        if (_layout.RailViewport.Contains(pt) || _railScrollbar.Bounds.Contains(pt))
            return ScrollArea.Rail;
        if (_layout.InspectorViewport.Contains(pt) || _inspectorScrollbar.Bounds.Contains(pt))
            return ScrollArea.Inspector;
        return ScrollArea.None;
    }

    private ScrollArea GetScrollbarAreaAtPoint(Point pt)
    {
        if (_mainScrollbar.Bounds.Contains(pt))
            return ScrollArea.Main;
        if (_railScrollbar.Bounds.Contains(pt))
            return ScrollArea.Rail;
        if (_inspectorScrollbar.Bounds.Contains(pt))
            return ScrollArea.Inspector;
        return ScrollArea.None;
    }

    private ScrollBarVisualState GetScrollbarState(ScrollArea area)
    {
        return area switch
        {
            ScrollArea.Main => _mainScrollbar,
            ScrollArea.Rail => _railScrollbar,
            ScrollArea.Inspector => _inspectorScrollbar,
            _ => ScrollBarVisualState.Empty
        };
    }

    private int GetScrollStep(ScrollArea area)
    {
        return area switch
        {
            ScrollArea.Main => ScalePaintMetric(108, 64),
            ScrollArea.Rail => ScalePaintMetric(92, 64),
            ScrollArea.Inspector => ScalePaintMetric(120, 72),
            _ => 0
        };
    }

    private void AdjustScroll(ScrollArea area, int delta)
    {
        switch (area)
        {
            case ScrollArea.Main:
                _mainScrollOffset = ClampScrollOffset(_mainScrollOffset + delta, _mainScrollbar.ContentHeight, _mainScrollbar.ViewportHeight);
                break;
            case ScrollArea.Rail:
                _railScrollOffset = ClampScrollOffset(_railScrollOffset + delta, _railScrollbar.ContentHeight, _railScrollbar.ViewportHeight);
                break;
            case ScrollArea.Inspector:
                _inspectorScrollOffset = ClampScrollOffset(_inspectorScrollOffset + delta, _inspectorScrollbar.ContentHeight, _inspectorScrollbar.ViewportHeight);
                break;
        }

        Invalidate();
    }

    private void SetScrollOffset(ScrollArea area, int offset)
    {
        switch (area)
        {
            case ScrollArea.Main:
                _mainScrollOffset = ClampScrollOffset(offset, _mainScrollbar.ContentHeight, _mainScrollbar.ViewportHeight);
                break;
            case ScrollArea.Rail:
                _railScrollOffset = ClampScrollOffset(offset, _railScrollbar.ContentHeight, _railScrollbar.ViewportHeight);
                break;
            case ScrollArea.Inspector:
                _inspectorScrollOffset = ClampScrollOffset(offset, _inspectorScrollbar.ContentHeight, _inspectorScrollbar.ViewportHeight);
                break;
        }
    }

    private bool CanDecreaseBoost()
    {
        return InDesigner || HasSelectedEnhancement() && _model.View.RelLevel.ToInt() > GetMinBoostCountForCurrentSelection();
    }

    private bool CanIncreaseBoost()
    {
        return InDesigner || HasSelectedEnhancement() && _model.View.RelLevel.ToInt() < GetMaxBoostCountForCurrentSelection();
    }

    private bool HasSelectedEnhancement()
    {
        return _model.View.PickerId >= 0 &&
               _model.View.PickerId < _model.EnhancementIds.Length &&
               IsValidEnhancementId(_model.EnhancementIds[_model.View.PickerId]);
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
        if (_hoverMainIndex >= 0 && _hoverMainIndex < _model.EnhancementIds.Length)
        {
            enhId = _model.EnhancementIds[_hoverMainIndex];
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

        if (TryGetRememberedSetSelectionForCurrentPower(out var rememberedSelection))
        {
            var rememberedSetTypeId = SetTypeToId(rememberedSelection.SetType);
            if (rememberedSetTypeId >= 0)
            {
                _model.Initial.TabId = Enums.eType.SetO;
                _model.Initial.SetTypeId = rememberedSetTypeId;
                _model.Initial.SetId = rememberedSelection.SetId;
                _model.Initial.SetVariant = rememberedSelection.SetVariant;
                _model.Initial.SetStage = SetPickerStage.SetEnhancementGrid;
                _model.SetIds = GetSets(_model.SetTypes[rememberedSetTypeId]);
                return;
            }
        }

        if (_lastTab == Enums.eType.SetO && _lastSet >= 0 && _lastSet < _model.SetTypes.Length)
        {
            _model.Initial.SetTypeId = _lastSet;
            _model.SetIds = GetSets(_model.SetTypes[_lastSet]);
        }
    }

    private bool TryGetRememberedSetSelectionForCurrentPower(out RememberedSetSelection selection)
    {
        selection = default;

        if (_powerId < 0)
            return false;

        if (_rememberedSetSelectionsByPower.TryGetValue(_powerId, out var rememberedSelection) &&
            IsRememberedSetSelectionValid(rememberedSelection))
        {
            selection = rememberedSelection;
            return true;
        }

        foreach (var slottedEnhancementId in _slotted.Reverse())
        {
            if (!TryBuildRememberedSetSelection(slottedEnhancementId, out var slottedSelection) ||
                !IsRememberedSetSelectionValid(slottedSelection))
            {
                continue;
            }

            _rememberedSetSelectionsByPower[_powerId] = slottedSelection;
            selection = slottedSelection;
            return true;
        }

        return false;
    }

    private bool IsRememberedSetSelectionValid(RememberedSetSelection selection)
    {
        if (selection.SetId < 0 ||
            selection.SetId >= DatabaseAPI.Database.EnhancementSets.Count)
        {
            return false;
        }

        var setTypeId = SetTypeToId(selection.SetType);
        return setTypeId >= 0 && GetSets(_model.SetTypes[setTypeId]).Contains(selection.SetId);
    }

    private bool TryBuildRememberedSetSelection(int enhancementId, out RememberedSetSelection selection)
    {
        selection = default;

        if (!IsValidEnhancementId(enhancementId))
            return false;

        var enhancement = DatabaseAPI.Database.Enhancements[enhancementId];
        if (enhancement.TypeID != Enums.eType.SetO ||
            enhancement.nIDSet < 0 ||
            enhancement.nIDSet >= DatabaseAPI.Database.EnhancementSets.Count)
        {
            return false;
        }

        selection = new RememberedSetSelection(
            DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].SetType,
            enhancement.nIDSet,
            DatabaseAPI.GetSetVariantKind(enhancementId));
        return true;
    }

    private void RememberSetSelectionForPower(int powerId, int enhancementId)
    {
        if (powerId < 0)
            return;

        if (TryBuildRememberedSetSelection(enhancementId, out var selection))
        {
            _rememberedSetSelectionsByPower[powerId] = selection;
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
                if (_powerId >= 0 &&
                    _model.View.SetTypeId >= 0 &&
                    _model.View.SetTypeId < _model.SetTypes.Length &&
                    _model.View.SetId >= 0)
                {
                    _rememberedSetSelectionsByPower[_powerId] = new RememberedSetSelection(
                        _model.SetTypes[_model.View.SetTypeId],
                        _model.View.SetId,
                        _model.View.SetVariant);
                }
                break;
        }
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

    private string GetDisabledReason(int index)
    {
        var enhId = _model.EnhancementIds.ElementAtOrDefault(index);
        if (enhId < 0)
            return string.Empty;

        var enh = DatabaseAPI.Database.Enhancements[enhId];
        if (enh.TypeID == Enums.eType.SetO && DatabaseAPI.TryGetSetPieceIndexForEnhancement(enhId, out _, out _))
        {
            var ignoredMatches = DatabaseAPI.AreEnhancementsSameSetPiece(_initialEnhancementId, enhId) ? 1 : 0;
            var currentPowerMatches = _slotted?.Count(slottedEnhId => DatabaseAPI.AreEnhancementsSameSetPiece(slottedEnhId, enhId)) ?? 0;
            if (currentPowerMatches > ignoredMatches)
                return "Another slot in this power already uses the same set piece.";
        }

        if (enh.Unique)
        {
            var ignoredUniqueMatches = _initialEnhancementId == enhId ? 1 : 0;
            var uniqueMatches = MidsContext.Character.CurrentBuild.Powers
                .Where(p => p is not null)
                .SelectMany(p => p!.Slots)
                .Count(s => s.Enhancement.Enh == enhId);
            if (uniqueMatches > ignoredUniqueMatches)
                return "This unique enhancement is already slotted elsewhere.";
        }

        return string.Empty;
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

    private bool IsMainItemSelected(int index)
    {
        if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid)
            return index >= 0 && index < _model.SetVariants.Length && _model.View.SetVariant == _model.SetVariants[index];

        return _model.View.PickerId == index;
    }

    private bool IsRailItemSelected(int dataIndex)
    {
        return _model.View.TabId switch
        {
            Enums.eType.Normal => dataIndex >= 0 && dataIndex < _model.NoGrades.Length && _model.View.GradeId == (Enums.eEnhGrade)_model.NoGrades[dataIndex],
            Enums.eType.SpecialO => dataIndex >= 0 && dataIndex < _model.SpecialTypes.Length && _model.View.SpecialId == _model.SpecialTypes[dataIndex],
            Enums.eType.SetO => _model.View.SetTypeId == dataIndex,
            _ => false
        };
    }

    private int GetMainMinimumCardWidth()
    {
        return (_model.View.TabId, _model.View.SetStage) switch
        {
            (Enums.eType.SetO, SetPickerStage.SetFamilyGrid) => ScalePaintMetric(250, 180),
            (Enums.eType.SetO, SetPickerStage.SetVariantGrid) => ScalePaintMetric(220, 160),
            (Enums.eType.SetO, SetPickerStage.SetEnhancementGrid) => ScalePaintMetric(220, 160),
            _ => ScalePaintMetric(250, 180)
        };
    }

    private int GetMainCardHeight()
    {
        return (_model.View.TabId, _model.View.SetStage) switch
        {
            (Enums.eType.SetO, SetPickerStage.SetFamilyGrid) => ScalePaintMetric(114, 86),
            (Enums.eType.SetO, SetPickerStage.SetVariantGrid) => ScalePaintMetric(92, 72),
            _ => ScalePaintMetric(102, 78)
        };
    }

    private string GetSecondaryLabelForEnhancement(int enhId)
    {
        if (!IsValidEnhancementId(enhId))
            return string.Empty;

        var enhancement = DatabaseAPI.Database.Enhancements[enhId];
        var displayName = GetDisplayNameForEnhancement(enhId);
        return enhancement.TypeID switch
        {
            Enums.eType.SetO when enhancement.nIDSet >= 0 => $"{DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName} | {enhancement.ShortName}",
            Enums.eType.SpecialO => string.Equals(enhancement.ShortName, displayName, StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : enhancement.ShortName ?? string.Empty,
            Enums.eType.InventO => enhancement.ShortName ?? string.Empty,
            Enums.eType.Normal => DatabaseAPI.Database.EnhGradeStringLong[(int)_model.View.GradeId],
            _ => enhancement.ShortName ?? string.Empty
        };
    }

    private string GetTertiaryLabelForEnhancement(int enhId, int index)
    {
        if (!IsValidEnhancementId(enhId))
            return string.Empty;

        if (IsEnhancementGrayed(index))
            return GetDisabledReason(index);

        if (TryGetEnhancementDisplayLevelRange(enhId, out var ioMin, out var ioMax))
            return $"Levels {ioMin}-{ioMax}";

        return string.Empty;
    }

    private string GetResolvedEnhancementDescription(int enhId)
    {
        if (!IsValidEnhancementId(enhId))
            return string.Empty;

        var slot = new I9Slot
        {
            Enh = enhId,
            Grade = _model.View.GradeId,
            RelativeLevel = _model.View.RelLevel,
            IOLevel = CheckAndReturnIoLevel(enhId) - 1
        };
        return slot.GetResolvedEnhancementDescription();
    }

    private string BuildScheduleSummary(int enhId)
    {
        if (!IsValidEnhancementId(enhId))
            return string.Empty;

        var enhancement = DatabaseAPI.Database.Enhancements[enhId];
        var scheduleValues = enhancement.Effect
            .Where(effect => effect.Mode == Enums.eEffMode.Enhancement && effect.Schedule != Enums.eSchedule.None)
            .Select(effect => new
            {
                effect.Schedule,
                Value = GetScheduleValue(enhId, effect)
            })
            .GroupBy(item => new
            {
                item.Schedule,
                RoundedValue = (float)Math.Round(item.Value, 5)
            })
            .Select(group => $"{group.Key.Schedule} ({DisplayValueFormatter.FormatPercentFromScale(group.First().Value)}%)")
            .ToArray();

        return scheduleValues.Length switch
        {
            0 => string.Empty,
            1 => $"Schedule: {scheduleValues[0]}",
            _ => $"Schedules: {string.Join(", ", scheduleValues)}"
        };
    }

    private float GetScheduleValue(int enhancementId, Enums.sEffect effect)
    {
        if (!IsValidEnhancementId(enhancementId))
            return 0f;

        var enhancement = DatabaseAPI.Database.Enhancements[enhancementId];
        var grade = _model.View.GradeId;
        var relativeLevel = _model.View.RelLevel;
        if (grade < Enums.eEnhGrade.None)
            grade = Enums.eEnhGrade.None;
        if (grade > Enums.eEnhGrade.SingleO)
            grade = Enums.eEnhGrade.SingleO;
        if (relativeLevel < Enums.eEnhRelative.None)
            relativeLevel = Enums.eEnhRelative.None;
        if (relativeLevel > Enums.eEnhRelative.PlusFive)
            relativeLevel = Enums.eEnhRelative.PlusFive;

        var slot = new I9Slot
        {
            Enh = enhancementId,
            Grade = grade,
            RelativeLevel = relativeLevel,
            IOLevel = Math.Max(0, CheckAndReturnIoLevel(enhancementId) - 1)
        };

        var scheduleMult = DatabaseAPI.GetEnhancementMathPolicy().GetScheduleScale(
            slot,
            enhancement.TypeID,
            grade,
            ResolveEffectiveIoLevel(enhancementId, enhancement.TypeID, slot.IOLevel),
            effect.Schedule,
            enhancement.Superior);
        if (Math.Abs(effect.Multiplier) > float.Epsilon)
            scheduleMult *= NormalizeClassicOrSpecialMultiplier(enhancement.TypeID, effect.Schedule, effect.Multiplier);
        return scheduleMult;
    }

    private static int ResolveEffectiveIoLevel(int enhancementId, Enums.eType enhancementType, int ioLevel)
    {
        var resolvedIoLevel = Math.Clamp(ioLevel, 0, DatabaseAPI.Database.MultIO.Length - 1);
        if (enhancementType is not (Enums.eType.InventO or Enums.eType.SetO) ||
            !IsValidEnhancementId(enhancementId))
        {
            return resolvedIoLevel;
        }

        var enhancement = DatabaseAPI.Database.Enhancements[enhancementId];
        var boostPower = enhancement.GetPower();
        var runtimeBoostPower = boostPower as Power;
        var usesPlayerLevel = runtimeBoostPower?.UsesPlayerLevelForBoostMath ?? boostPower?.BoostUsePlayerLevel == true;
        if (!usesPlayerLevel)
        {
            var minimumLevel = enhancement.LevelMin;
            var maximumLevel = enhancement.LevelMax;
            if (runtimeBoostPower?.AllowsBoostersForBoostMath == true &&
                runtimeBoostPower.ImportedMaxBoostLevelZeroBased.HasValue)
            {
                var unboostedLookupCap = Math.Max(minimumLevel, runtimeBoostPower.ImportedMaxBoostLevelZeroBased.Value - 1);
                maximumLevel = Math.Min(maximumLevel, unboostedLookupCap);
            }

            maximumLevel = Math.Max(minimumLevel, maximumLevel);
            return Math.Clamp(
                resolvedIoLevel,
                Math.Max(0, minimumLevel),
                Math.Min(DatabaseAPI.Database.MultIO.Length - 1, maximumLevel));
        }

        var configuredLevel = Math.Max(1, MidsContext.Config?.ForceLevel ?? Character.MaxLevel + 1) - 1;
        var experienceLevel = MidsContext.Character?.Level ?? -1;
        var resolvedPlayerLevel = Math.Max(Math.Max(0, configuredLevel), experienceLevel);

        var effectiveMaximumLevel = enhancement.LevelMax;
        if (boostPower is Power boostPolicyPower &&
            boostPolicyPower.ImportedMaxBoostLevelZeroBased.HasValue)
        {
            effectiveMaximumLevel = boostPolicyPower.ImportedMaxBoostLevelZeroBased.Value;
        }
        else if (enhancement.TypeID == Enums.eType.SetO && enhancement.nIDSet > -1)
        {
            effectiveMaximumLevel = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].LevelMax;
        }

        return Math.Clamp(
            Math.Min(resolvedPlayerLevel, effectiveMaximumLevel),
            0,
            DatabaseAPI.Database.MultIO.Length - 1);
    }

    private float NormalizeClassicOrSpecialMultiplier(Enums.eType enhancementType, Enums.eSchedule schedule, float multiplier)
    {
        if (enhancementType is not (Enums.eType.Normal or Enums.eType.SpecialO) ||
            schedule is Enums.eSchedule.None or Enums.eSchedule.Multiple)
        {
            return multiplier;
        }

        var scheduleIndex = (int)schedule;
        if (scheduleIndex < 0 || scheduleIndex > 3)
            return multiplier;

        var candidates = enhancementType == Enums.eType.SpecialO
            ? new[]
            {
                DatabaseAPI.Database.MultHO is { Length: > 0 } && DatabaseAPI.Database.MultHO[0].Length > scheduleIndex
                    ? DatabaseAPI.Database.MultHO[0][scheduleIndex]
                    : 0f
            }
            : new[]
            {
                DatabaseAPI.Database.MultTO is { Length: > 0 } && DatabaseAPI.Database.MultTO[0].Length > scheduleIndex
                    ? DatabaseAPI.Database.MultTO[0][scheduleIndex]
                    : 0f,
                DatabaseAPI.Database.MultDO is { Length: > 0 } && DatabaseAPI.Database.MultDO[0].Length > scheduleIndex
                    ? DatabaseAPI.Database.MultDO[0][scheduleIndex]
                    : 0f,
                DatabaseAPI.Database.MultSO is { Length: > 0 } && DatabaseAPI.Database.MultSO[0].Length > scheduleIndex
                    ? DatabaseAPI.Database.MultSO[0][scheduleIndex]
                    : 0f
            };

        return candidates.Any(candidate => candidate > float.Epsilon && Math.Abs(Math.Abs(multiplier) - candidate) < 0.02f)
            ? Math.Sign(multiplier == 0 ? 1 : multiplier)
            : multiplier;
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
        return IsValidEnhancementId(enhId) ? DatabaseAPI.Database.Enhancements[enhId].ImageIdx : -1;
    }

    private string GetDisplayNameForEnhancement(int enhId)
    {
        if (!IsValidEnhancementId(enhId))
            return string.Empty;

        var enhancement = DatabaseAPI.Database.Enhancements[enhId];
        if (_model.EnhancementNames.TryGetValue(enhId, out var canonicalName) &&
            !string.IsNullOrWhiteSpace(canonicalName))
        {
            return canonicalName;
        }

        if (enhancement.TypeID == Enums.eType.SpecialO)
        {
            var specialEnh = DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID);
            if (!string.IsNullOrWhiteSpace(specialEnh.Name))
                return specialEnh.Name;

            var resolved = enhancement.GetSpecialName();
            if (!string.IsNullOrWhiteSpace(resolved))
                return resolved;
        }

        return enhancement.Name;
    }

    private EnhSelectorInspectorSection? BuildStandardBonusSection(int setId, EnhancementSet set, int maxLines)
    {
        var lines = new List<string>();
        var successIndexes = new HashSet<int>();
        var slottedPieceCount = GetSlottedPieceCountForSet(setId);
        for (var index = 0; index < set.Bonus.Length; index++)
        {
            var bonus = set.Bonus[index];
            var effectString = set.GetEffectString(index, false, true, true, true);
            if (string.IsNullOrWhiteSpace(effectString))
                continue;

            if (set.GetEffectiveBonusPvMode(index, false) is Enums.ePvX.PvP)
                effectString += " (PVP)";

            lines.Add($"({bonus.Slotted}) {effectString}");
            if (bonus.Slotted <= slottedPieceCount)
                successIndexes.Add(lines.Count - 1);

            if (lines.Count >= maxLines)
                break;
        }

        return lines.Count == 0
            ? null
            : new EnhSelectorInspectorSection
            {
                Heading = "Set Bonuses",
                Lines = lines,
                SuccessLineIndexes = successIndexes
            };
    }

    private string FormatVisiblePieceCount(int setId)
    {
        var visiblePieceCount = GetVisiblePieceCount(setId);
        return $"{visiblePieceCount} visible pieces";
    }

    private int GetVisiblePieceCount(int setId)
    {
        if (setId < 0 || setId >= DatabaseAPI.Database.EnhancementSets.Count)
            return 0;

        return DatabaseAPI.GetEnhancementSetProjection(setId).VisiblePieces.Count;
    }

    private int GetSlottedPieceCountForSet(int setId)
    {
        if (setId < 0 || _slotted.Length == 0)
            return 0;

        return _slotted.Count(enhId =>
            IsValidEnhancementId(enhId) &&
            DatabaseAPI.Database.Enhancements[enhId].nIDSet == setId);
    }

    private int MeasureInspectorSnapshot(
        Graphics g,
        EnhSelectorInspectorSnapshot snapshot,
        int width,
        Font titleFont,
        Font subtitleFont,
        Font sectionFont,
        Font lineFont,
        Font tagFont)
    {
        var y = 0;
        y += MeasureTextBlock(g, snapshot.Title, titleFont, width);
        y += ScalePaintMetric(4, 2);

        if (!string.IsNullOrWhiteSpace(snapshot.Subtitle))
        {
            y += MeasureTextBlock(g, snapshot.Subtitle, subtitleFont, width);
            y += ScalePaintMetric(6, 4);
        }

        if (!string.IsNullOrWhiteSpace(snapshot.AccentLine))
        {
            y += MeasureTextBlock(g, snapshot.AccentLine, subtitleFont, width);
            y += ScalePaintMetric(8, 4);
        }

        if (snapshot.Tags.Count > 0)
        {
            y += ScalePaintMetric(26, 18);
            y += ScalePaintMetric(8, 4);
        }

        foreach (var section in snapshot.Sections)
        {
            y += MeasureTextBlock(g, section.Heading, sectionFont, width);
            y += ScalePaintMetric(4, 2);
            for (var lineIndex = 0; lineIndex < section.Lines.Count; lineIndex++)
            {
                var line = section.Lines[lineIndex];
                y += ShouldHighlightPercentageInline(section, lineIndex, line)
                    ? MeasureHighlightedTextBlock(g, line, lineFont, width)
                    : MeasureTextBlock(g, line, lineFont, width);
                y += ScalePaintMetric(4, 2);
            }

            y += ScalePaintMetric(10, 6);
        }

        if (!string.IsNullOrWhiteSpace(snapshot.Note))
        {
            y += MeasureTextBlock(g, snapshot.Note, lineFont, width);
            y += ScalePaintMetric(6, 4);
        }

        return y;
    }

    private void RenderInspectorSnapshot(
        Graphics g,
        EnhSelectorInspectorSnapshot snapshot,
        Rectangle viewport,
        int scrollOffset,
        Font titleFont,
        Font subtitleFont,
        Font sectionFont,
        Font lineFont,
        Font tagFont)
    {
        var left = viewport.Left;
        var width = viewport.Width;
        var y = viewport.Top - scrollOffset;
        using var dividerPen = new Pen(Color.FromArgb(110, Palette.Divider), 1f);

        var titleHeight = MeasureTextBlock(g, snapshot.Title, titleFont, width);
        EnhSelectorFrameRenderer.DrawText(g, snapshot.Title, titleFont, Palette.HeaderText, new Rectangle(left, y, width, titleHeight), ContentAlignment.TopLeft, StringTrimming.EllipsisWord);
        y += titleHeight + ScalePaintMetric(4, 2);

        if (!string.IsNullOrWhiteSpace(snapshot.Subtitle))
        {
            var subtitleHeight = MeasureTextBlock(g, snapshot.Subtitle, subtitleFont, width);
            EnhSelectorFrameRenderer.DrawText(g, snapshot.Subtitle, subtitleFont, Palette.Text, new Rectangle(left, y, width, subtitleHeight), ContentAlignment.TopLeft, StringTrimming.EllipsisWord);
            y += subtitleHeight + ScalePaintMetric(6, 4);
        }

        if (!string.IsNullOrWhiteSpace(snapshot.AccentLine))
        {
            var accentHeight = MeasureTextBlock(g, snapshot.AccentLine, subtitleFont, width);
            EnhSelectorFrameRenderer.DrawText(g, snapshot.AccentLine, subtitleFont, Palette.AccentText, new Rectangle(left, y, width, accentHeight), ContentAlignment.TopLeft, StringTrimming.EllipsisWord);
            y += accentHeight + ScalePaintMetric(8, 4);
        }

        if (snapshot.Tags.Count > 0)
        {
            var chipX = left;
            var chipHeight = ScalePaintMetric(20, 15);
            foreach (var tag in snapshot.Tags.Where(tag => !string.IsNullOrWhiteSpace(tag.Text)))
            {
                var chipWidth = Math.Max(58, TextRenderer.MeasureText(tag.Text, tagFont).Width + ScalePaintMetric(14, 10));
                var chipRect = new Rectangle(chipX, y, Math.Min(chipWidth, width), chipHeight);
                EnhSelectorFrameRenderer.DrawChip(g, chipRect, Palette, tag, tagFont);
                chipX += chipRect.Width + ScalePaintMetric(6, 4);
                if (chipX > left + width - 48)
                    break;
            }

            y += chipHeight + ScalePaintMetric(8, 4);
        }

        for (var sectionIndex = 0; sectionIndex < snapshot.Sections.Count; sectionIndex++)
        {
            var section = snapshot.Sections[sectionIndex];
            if (sectionIndex > 0)
            {
                var dividerY = y - ScalePaintMetric(4, 2);
                g.DrawLine(dividerPen, left, dividerY, left + width - 2, dividerY);
            }

            var headingHeight = MeasureTextBlock(g, section.Heading, sectionFont, width);
            EnhSelectorFrameRenderer.DrawText(g, section.Heading, sectionFont, Palette.SectionText, new Rectangle(left, y, width, headingHeight), ContentAlignment.TopLeft, StringTrimming.EllipsisWord);
            y += headingHeight + ScalePaintMetric(4, 2);

            for (var lineIndex = 0; lineIndex < section.Lines.Count; lineIndex++)
            {
                var line = section.Lines[lineIndex];
                var highlightInline = ShouldHighlightPercentageInline(section, lineIndex, line);
                var lineHeight = highlightInline
                    ? MeasureHighlightedTextBlock(g, line, lineFont, width)
                    : MeasureTextBlock(g, line, lineFont, width);
                var color = section.IsWarning
                    ? Palette.WarningText
                    : section.SuccessLineIndexes.Contains(lineIndex)
                        ? Palette.SuccessText
                        : Palette.Text;
                var lineBounds = new Rectangle(left, y, width, lineHeight);
                if (highlightInline)
                {
                    DrawHighlightedTextBlock(g, line, lineFont, lineBounds, color, Palette.ValueHighlightText);
                }
                else
                {
                    EnhSelectorFrameRenderer.DrawText(g, line, lineFont, color, lineBounds, ContentAlignment.TopLeft, StringTrimming.EllipsisWord);
                }
                y += lineHeight + ScalePaintMetric(4, 2);
            }

            y += ScalePaintMetric(10, 6);
        }

        if (!string.IsNullOrWhiteSpace(snapshot.Note))
        {
            var noteHeight = MeasureTextBlock(g, snapshot.Note, lineFont, width);
            EnhSelectorFrameRenderer.DrawText(g, snapshot.Note, lineFont, Palette.MutedText, new Rectangle(left, y, width, noteHeight), ContentAlignment.TopLeft, StringTrimming.EllipsisWord);
        }
    }

    private int MeasureTextBlock(Graphics g, string text, Font font, int width)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var measured = g.MeasureString(text, font, width);
        return Math.Max(font.Height, (int)Math.Ceiling(measured.Height));
    }

    private bool ShouldHighlightPercentageInline(EnhSelectorInspectorSection section, int lineIndex, string line)
    {
        if (section.IsWarning ||
            section.SuccessLineIndexes.Contains(lineIndex) ||
            string.IsNullOrWhiteSpace(line) ||
            !PercentHighlightRegex.IsMatch(line))
        {
            return false;
        }

        return !string.Equals(section.Heading, "Set Bonuses", StringComparison.OrdinalIgnoreCase);
    }

    private int MeasureHighlightedTextBlock(Graphics g, string text, Font font, int width)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var lineCount = 1;
        var currentWidth = 0f;
        foreach (var token in TokenizeHighlightedText(text))
        {
            if (token.Text == "\n")
            {
                lineCount++;
                currentWidth = 0f;
                continue;
            }

            var tokenWidth = MeasureInlineTokenWidth(token.Text, font);
            if (!token.IsWhitespace && currentWidth > 0f && currentWidth + tokenWidth > width)
            {
                lineCount++;
                currentWidth = 0f;
            }
            else if (token.IsWhitespace && currentWidth + tokenWidth > width)
            {
                lineCount++;
                currentWidth = 0f;
                continue;
            }

            currentWidth += tokenWidth;
        }

        return Math.Max(font.Height, lineCount * font.Height);
    }

    private void DrawHighlightedTextBlock(Graphics g, string text, Font font, Rectangle bounds, Color baseColor, Color highlightColor)
    {
        if (string.IsNullOrWhiteSpace(text) || bounds.Width <= 0 || bounds.Height <= 0)
            return;

        const TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;
        var x = bounds.Left;
        var y = bounds.Top;
        var maxRight = bounds.Right;
        var lineHeight = font.Height;

        foreach (var token in TokenizeHighlightedText(text))
        {
            if (token.Text == "\n")
            {
                x = bounds.Left;
                y += lineHeight;
                continue;
            }

            var tokenWidth = (int)Math.Ceiling(MeasureInlineTokenWidth(token.Text, font));
            if (!token.IsWhitespace && x > bounds.Left && x + tokenWidth > maxRight)
            {
                x = bounds.Left;
                y += lineHeight;
            }
            else if (token.IsWhitespace && x + tokenWidth > maxRight)
            {
                x = bounds.Left;
                y += lineHeight;
                continue;
            }

            TextRenderer.DrawText(
                g,
                token.Text,
                font,
                new Point(x, y),
                token.IsHighlighted ? highlightColor : baseColor,
                flags);

            x += tokenWidth;
        }
    }

    private float MeasureInlineTokenWidth(string text, Font font)
    {
        if (string.IsNullOrEmpty(text))
            return 0f;

        return TextRenderer.MeasureText(
            text,
            font,
            new Size(int.MaxValue, int.MaxValue),
            TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine).Width;
    }

    private static IEnumerable<(string Text, bool IsHighlighted, bool IsWhitespace)> TokenizeHighlightedText(string text)
    {
        var position = 0;
        foreach (Match match in PercentHighlightRegex.Matches(text))
        {
            if (match.Index > position)
            {
                foreach (var token in TokenizePlainText(text[position..match.Index], false))
                    yield return token;
            }

            yield return (match.Value, true, false);
            position = match.Index + match.Length;
        }

        if (position < text.Length)
        {
            foreach (var token in TokenizePlainText(text[position..], false))
                yield return token;
        }
    }

    private static IEnumerable<(string Text, bool IsHighlighted, bool IsWhitespace)> TokenizePlainText(string text, bool highlighted)
    {
        if (string.IsNullOrEmpty(text))
            yield break;

        var buffer = new StringBuilder();
        bool? whitespace = null;

        foreach (var ch in text)
        {
            if (ch == '\r')
                continue;

            if (ch == '\n')
            {
                if (buffer.Length > 0)
                {
                    yield return (buffer.ToString(), highlighted, whitespace ?? false);
                    buffer.Clear();
                    whitespace = null;
                }

                yield return ("\n", false, false);
                continue;
            }

            var isWhitespace = char.IsWhiteSpace(ch);
            if (whitespace.HasValue && whitespace.Value != isWhitespace)
            {
                yield return (buffer.ToString(), highlighted, whitespace.Value);
                buffer.Clear();
            }

            whitespace = isWhitespace;
            buffer.Append(ch);
        }

        if (buffer.Length > 0)
            yield return (buffer.ToString(), highlighted, whitespace ?? false);
    }

    private static int ClampScrollOffset(int offset, int contentHeight, int viewportHeight)
    {
        return Math.Clamp(offset, 0, Math.Max(0, contentHeight - viewportHeight));
    }

    private EnhSelectorLayoutOptions CreateResolvedLayoutOptions()
    {
        var resolved = _layoutOptions.Clone();
        var scaleBand = GetScaleBand();
        var railBase = Math.Max(120, _layoutOptions.RailWidth);

        if (scaleBand <= 0.5f)
        {
            resolved.IconSize = 32;
            resolved.Margin = 6;
            resolved.Gap = 6;
            resolved.TitleHeight = 22;
            resolved.CloseButtonSize = 22;
            resolved.TabHeight = 56;
            resolved.ContentHeight = 370;
            resolved.FooterHeight = 84;
            resolved.FooterHintHeight = 16;
            resolved.BaseMainWidth = 470;
            resolved.RailWidth = Math.Max(124, railBase - 8);
            resolved.InspectorWidth = 256;
            resolved.InnerPadding = 10;
            resolved.MainHeaderHeight = 28;
            resolved.MainSubHeaderHeight = 16;
            resolved.InspectorHeaderHeight = 22;
            resolved.BackButtonWidth = 68;
            resolved.BackButtonHeight = 24;
            resolved.LevelPanelWidth = 160;
            resolved.ButtonWidth = 26;
            resolved.ButtonHeight = 22;
            resolved.ScrollbarWidth = 10;
            resolved.TabInnerPadding = 4;
            return resolved;
        }

        if (scaleBand <= 0.75f)
        {
            resolved.IconSize = 48;
            resolved.Margin = 6;
            resolved.Gap = 6;
            resolved.TitleHeight = 24;
            resolved.CloseButtonSize = 24;
            resolved.TabHeight = 64;
            resolved.ContentHeight = 522;
            resolved.FooterHeight = 90;
            resolved.FooterHintHeight = 16;
            resolved.BaseMainWidth = 520;
            resolved.RailWidth = Math.Max(132, railBase);
            resolved.InspectorWidth = 264;
            resolved.InnerPadding = 10;
            resolved.MainHeaderHeight = 28;
            resolved.MainSubHeaderHeight = 16;
            resolved.InspectorHeaderHeight = 24;
            resolved.BackButtonWidth = 74;
            resolved.BackButtonHeight = 26;
            resolved.LevelPanelWidth = 168;
            resolved.ButtonWidth = 28;
            resolved.ButtonHeight = 22;
            resolved.ScrollbarWidth = 10;
            resolved.TabInnerPadding = 5;
            return resolved;
        }

        resolved.IconSize = 64;
        resolved.Margin = 6;
        resolved.Gap = 6;
        resolved.TitleHeight = 26;
        resolved.CloseButtonSize = 26;
        resolved.TabHeight = 78;
        resolved.ContentHeight = 660;
        resolved.FooterHeight = 98;
        resolved.FooterHintHeight = 18;
        resolved.BaseMainWidth = 596;
        resolved.RailWidth = Math.Max(140, railBase);
        resolved.InspectorWidth = 296;
        resolved.InnerPadding = 11;
        resolved.MainHeaderHeight = 30;
        resolved.MainSubHeaderHeight = 18;
        resolved.InspectorHeaderHeight = 26;
        resolved.BackButtonWidth = 78;
        resolved.BackButtonHeight = 28;
        resolved.LevelPanelWidth = 176;
        resolved.ButtonWidth = 30;
        resolved.ButtonHeight = 24;
        resolved.ScrollbarWidth = 12;
        resolved.TabInnerPadding = 6;
        return resolved;
    }

    private void RefreshScaledSize()
    {
        var preferredSize = EnhSelectorLayout.CalculatePreferredSize(CreateResolvedLayoutOptions());
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
        var scale = GetPaintScale();
        return new Font(family, Math.Max(minimumSize, baseSize * scale), style);
    }

    private int ScalePaintMetric(int baseValue, int minimum)
    {
        var scale = GetPaintScale();
        return Math.Max(minimum, (int)Math.Round(baseValue * scale));
    }

    private static int MeasureSingleLineTextWidth(Graphics graphics, string text, Font font)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        using var format = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None,
            FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit
        };

        return (int)Math.Ceiling(graphics.MeasureString(text, font, int.MaxValue, format).Width);
    }

    private float GetPaintScale()
    {
        var rawScale = Math.Clamp(_uiScale, 0.5f, 1f);
        var t = (rawScale - 0.5f) / 0.5f;
        return MinReadablePaintScale + (1f - MinReadablePaintScale) * Math.Clamp(t, 0f, 1f);
    }

    private int GetMainGridGap()
    {
        return ScalePaintMetric(10, 6);
    }

    private int GetMainCardHorizontalInset(int cellWidth)
    {
        var inset = GetScaleBand() switch
        {
            <= 0.5f => ScalePaintMetric(2, 2),
            <= 0.75f => ScalePaintMetric(4, 3),
            _ => ScalePaintMetric(6, 4)
        };

        return Math.Min(inset, Math.Max(2, cellWidth / 10));
    }

    private int GetMainCardVerticalInset(int cellHeight)
    {
        var inset = GetScaleBand() switch
        {
            <= 0.5f => ScalePaintMetric(2, 2),
            <= 0.75f => ScalePaintMetric(3, 2),
            _ => ScalePaintMetric(4, 3)
        };

        return Math.Min(inset, Math.Max(2, cellHeight / 12));
    }

    private int GetMainTitleLineCount(MainItemLayout item)
    {
        if (GetScaleBand() < 1f &&
            item.Kind is MainItemKind.SetFamily or MainItemKind.SetVariant)
        {
            return 1;
        }

        if (item.Kind is MainItemKind.SetFamily or MainItemKind.SetVariant)
            return 2;

        if (GetScaleBand() < 1f &&
            item.Kind == MainItemKind.Enhancement &&
            _model.View.TabId == Enums.eType.SetO)
        {
            return 1;
        }

        if (item.Kind == MainItemKind.Enhancement &&
            _model.View.TabId == Enums.eType.SetO &&
            _model.View.SetStage == SetPickerStage.SetEnhancementGrid)
        {
            return 2;
        }

        return 1;
    }

    private int GetResponsiveGridIconSize(Rectangle bounds, int innerWidth, int innerHeight, int mandatoryTitleHeight, int iconGap, int targetIconSize)
    {
        var widthLimit = Math.Max(MinIconSize, innerWidth - ScalePaintMetric(8, 4));
        var heightLimit = Math.Max(MinIconSize, innerHeight - mandatoryTitleHeight - iconGap);
        var size = Math.Min(targetIconSize, Math.Min(widthLimit, heightLimit));
        return Math.Clamp(size, MinIconSize, MaxIconSize);
    }

    private int GetResponsiveRailIconSize(Rectangle bounds, int horizontalPadding, int topPadding, int bottomPadding, int reservedTextHeight, int targetIconSize)
    {
        var maxWidth = Math.Max(28, bounds.Width - horizontalPadding * 2);
        var maxHeight = Math.Max(28, bounds.Height - topPadding - bottomPadding - reservedTextHeight);
        var size = Math.Min(targetIconSize, Math.Min(maxWidth, maxHeight));
        return Math.Max(28, size);
    }

    private int GetResponsiveSummaryIconSize(Rectangle bounds, int targetIconSize)
    {
        var maxHeight = Math.Max(28, bounds.Height - ScalePaintMetric(6, 4));
        var size = Math.Min(targetIconSize, maxHeight);
        return Math.Max(28, size);
    }

    private float GetScaleBand()
    {
        if (_uiScale >= 0.875f)
            return 1f;

        if (_uiScale >= 0.625f)
            return 0.75f;

        return 0.5f;
    }

    private int GetMainGridBandIconSize()
    {
        return GetScaleBand() switch
        {
            <= 0.5f => 32,
            <= 0.75f => 48,
            _ => 64
        };
    }

    private int GetUniformBandIconSize()
    {
        return GetMainGridBandIconSize();
    }

    private int GetHeaderBandIconSize()
    {
        return GetUniformBandIconSize();
    }

    private int GetRailBandIconSize()
    {
        return GetUniformBandIconSize();
    }

    private int GetSummaryBandIconSize()
    {
        return GetUniformBandIconSize();
    }

    private static string FormatMainCardTitle(string title, bool allowMultiline)
    {
        return FormatCompactLabel(title, allowMultiline, 18);
    }

    private static string FormatCompactLabel(string text, bool allowMultiline, int minimumLength)
    {
        if (!allowMultiline || string.IsNullOrWhiteSpace(text) || text.Length <= minimumLength)
            return text;

        var midpoint = text.Length / 2;
        var separators = new[] { '/', ' ' };
        var breakIndex = -1;
        var bestDistance = int.MaxValue;

        foreach (var separator in separators)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] != separator)
                    continue;

                var distance = Math.Abs(i - midpoint);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    breakIndex = i;
                }
            }
        }

        if (breakIndex <= 3 || breakIndex >= text.Length - 4)
            return text;

        return text[breakIndex] == '/'
            ? string.Concat(text.AsSpan(0, breakIndex + 1), "\n", text.AsSpan(breakIndex + 1))
            : string.Concat(text.AsSpan(0, breakIndex), "\n", text.AsSpan(breakIndex + 1));
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

    private readonly record struct RememberedSetSelection(
        int SetType,
        int SetId,
        SetVariantKind? SetVariant);

    private enum FooterVisualKind
    {
        None,
        Enhancement,
        SetFamily,
        SetVariant,
        Empty
    }

    private readonly record struct FooterSummary(
        string Title,
        string Body,
        string Detail = "",
        FooterVisualKind VisualKind = FooterVisualKind.None,
        int DataId = -1,
        SetVariantKind? Variant = null,
        Enums.eEnhGrade? EnhancementGrade = null,
        IReadOnlyList<EnhSelectorChip>? Tags = null);

    private enum MainItemKind
    {
        Enhancement,
        SetFamily,
        SetVariant,
        Placeholder
    }

    private readonly record struct MainItemLayout(
        MainItemKind Kind,
        int Index,
        int DataId,
        string Title,
        string Subtitle,
        string Detail)
    {
        public Rectangle Bounds { get; init; }
    }

    private readonly record struct RailItemLayout(
        int DataIndex,
        int IconKey,
        string Label)
    {
        public Rectangle Bounds { get; init; }
    }

    private readonly record struct ScrollBarVisualState(
        Rectangle Bounds,
        Rectangle Track,
        Rectangle Thumb,
        int ViewportHeight,
        int ContentHeight,
        int Offset)
    {
        public static ScrollBarVisualState Empty { get; } = new(Rectangle.Empty, Rectangle.Empty, Rectangle.Empty, 0, 0, 0);
        public bool Scrollable => ContentHeight > ViewportHeight && Bounds.Width > 0 && Bounds.Height > 0;

        public static ScrollBarVisualState Create(Rectangle bounds, int viewportHeight, int contentHeight, int offset)
        {
            if (bounds.IsEmpty || contentHeight <= viewportHeight || viewportHeight <= 0)
            {
                return new ScrollBarVisualState(bounds, Rectangle.Empty, Rectangle.Empty, viewportHeight, contentHeight, offset);
            }

            var inset = Math.Max(1, bounds.Width / 4);
            var track = Rectangle.FromLTRB(bounds.Left + inset, bounds.Top + 4, bounds.Right - inset, bounds.Bottom - 4);
            var scrollMax = Math.Max(0, contentHeight - viewportHeight);
            var thumbHeight = Math.Max(24, (int)Math.Round((double)viewportHeight / contentHeight * track.Height));
            thumbHeight = Math.Min(thumbHeight, track.Height);
            var available = Math.Max(0, track.Height - thumbHeight);
            var thumbY = track.Top;
            if (available > 0 && scrollMax > 0)
            {
                var ratio = (double)offset / scrollMax;
                thumbY = track.Top + (int)Math.Round(available * ratio);
            }

            var thumb = new Rectangle(track.Left, thumbY, Math.Max(2, track.Width), thumbHeight);
            return new ScrollBarVisualState(bounds, track, thumb, viewportHeight, contentHeight, offset);
        }
    }
}

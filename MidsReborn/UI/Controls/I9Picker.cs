using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.UI.Renderer;
using Mids_Reborn.UI.Theming;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.UI.Controls
{
    [ToolboxItem(false)]
    public sealed class I9Picker : Control
    {
        #region Struct

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

        #endregion

        #region Constants

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

        #endregion

        #region Fields

        private readonly BufferedGraphicsContext _context;
        private BufferedGraphics? _buffer;

        private EnhSelectorModel _model = new();

        private string? _hoverTitle;
        private string? _hoverInfo;
        private string? _hoverText;

        private int _hoverEnhIndex = -1;
        private int _hoverSetIndex = -1;
        private int _hoverHeaderIndex = -1;

        private int _powerId;
        private int[] _normalEnhs = [];
        private int[] _inventionEnhs = [];
        private Enums.eType _lastTab = Enums.eType.Normal;
        private Enums.eEnhGrade _lastGrade = Enums.eEnhGrade.SingleO;
        private int _lastSpecial = 1;
        private int _lastSet;
        private int _initialEnhancementId = -1;

        private int[] _slotted = [];

        public int LastLevel { get; internal set; }

        private readonly List<(Rectangle Bounds, int Index)> _enhancementRects = [];
        private readonly List<(Rectangle Bounds, int Index)> _gradeRects = [];
        private readonly List<(Rectangle Bounds, int Index)> _headerRects = [];

        private Rectangle _levelBoxRect = Rectangle.Empty;
        private Rectangle _lvlPlusRect = Rectangle.Empty;
        private Rectangle _lvlMinusRect = Rectangle.Empty;
        private Rectangle _railScrollbarBounds = Rectangle.Empty;
        private Rectangle _railScrollbarTrackBounds = Rectangle.Empty;
        private Rectangle _railScrollbarThumbRect = Rectangle.Empty;

        private int _scrollOffset;
        private bool _railDraggingThumb;
        private int _railDragStartY;
        private bool _railHoveringThumb;
        private bool _themeHooked;
        private Action? _themeChangedHandler;

        #endregion

        #region Events

        public delegate void EnhancementPickedEventHandler(I9Slot e);
        public delegate void EnhancementSelectionCancelledEventHandler();
        public delegate void HoverEnhancementEventHandler(int e, EnhUniqueStatus? enhUniqueStatus);
        public delegate void HoverSetEventHandler(int e);
        public delegate void MovedEventHandler(Rectangle oldBounds, Rectangle newBounds);

        public event EnhancementPickedEventHandler? EnhancementPicked;
        public event EnhancementSelectionCancelledEventHandler? EnhancementSelectionCancelled;
        public event HoverEnhancementEventHandler? HoverEnhancement;
        public event HoverSetEventHandler? HoverSet;
        public event MovedEventHandler? Moved;

        #endregion

        #region Constructor

        public I9Picker()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            _context = BufferedGraphicsManager.Current;
            Resize += (_, _) => RecreateBuffer();
            int maxHeight = CalculateMaxHeight();
            Size = new Size(CalculateMaxWidth(), maxHeight);
            MouseWheel += I9Picker_MouseWheel;
            MouseMove += I9Picker_MouseMove;
            MouseDown += I9Picker_MouseDown;
            MouseUp += I9Picker_MouseUp;
            MouseLeave += I9Picker_MouseLeave;
            KeyDown += I9Picker_KeyDown;
            TabStop = true;
            Focus();
        }

        #endregion

        #region Public Properties

        public EnhSelectorState View => _model.View;

        #endregion

        #region Theme

        private DataViewTheme CurrentTheme
        {
            get
            {
                if (DesignMode)
                {
                    return ThemeManager.DesignTime.DataView;
                }

                return ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;
            }
        }

        private I9PickerPalette CurrentPalette => I9PickerPalette.From(CurrentTheme, CurrentButtonTheme);

        private ButtonTheme CurrentButtonTheme
        {
            get
            {
                if (DesignMode)
                {
                    return ThemeManager.DesignTime.Button;
                }

                return ThemeManager.CurrentTheme?.Button ?? ThemeManager.DesignTime.Button;
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

        #endregion

        #region Public Methods

        public void SetData(int iPower, I9Slot iSlot, int[] slotted)
        {
            var validPowerId = IsValidPowerId(iPower) ? iPower : -1;
            var initialEnhancementId = IsValidEnhancementId(iSlot.Enh) ? iSlot.Enh : -1;

            // 0. Store the power ID
            _powerId = validPowerId;
            _hoverTitle = validPowerId > -1
                ? $"Enhancing: {DatabaseAPI.Database.Power[validPowerId].DisplayName}"
                : "Enhancing";
            _slotted = slotted?.Where(IsValidEnhancementId).ToArray() ?? [];
            _initialEnhancementId = initialEnhancementId;

            // 1. Reset the model to ensure a clean state
            _model = new EnhSelectorModel();

            // 2. Fetch all possible enhancements and types for the power
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

            // 3. Determine the initial state based on the provided slot
            // Start with last-used or default values
            _model.Initial.GradeId = _lastGrade;
            _model.Initial.RelLevel = Enums.eEnhRelative.Even;
            _model.Initial.SpecialId = _lastSpecial > 0 ? _lastSpecial : 1;

            // If the slot is already filled, override defaults with its data
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

                // Correctly handle pre-selected sets
                if (enh.TypeID == Enums.eType.SetO &&
                    enh.nIDSet >= 0 &&
                    enh.nIDSet < DatabaseAPI.Database.EnhancementSets.Count)
                {
                    int setType = DatabaseAPI.Database.EnhancementSets[enh.nIDSet].SetType;
                    _model.Initial.SetTypeId = SetTypeToId(setType);
                    _model.SetIds = GetSets(setType);
                    _model.Initial.SetId = enh.nIDSet;
                    _model.Initial.SetVariant = DatabaseAPI.GetSetVariantKind(initialEnhancementId);
                    _model.Initial.SetStage = SetPickerStage.SetEnhancementGrid;
                }
            }
            else // Otherwise, the slot is empty
            {
                _hoverText = "";

                ApplyLastUsedStateForEmptySlot();
            }

            // 4. Set the final View state that the user will interact with
            // Assumes a copy constructor exists on EnhSelectorState
            _model.View = new EnhSelectorState(_model.Initial);

            // If opening on an empty slot, default the view to the last used tab for better UX
            if (_model.View.TabId == Enums.eType.None)
            {
                _model.View.TabId = _lastTab;
            }

            // 5. Load the active data into the model based on the final view state
            SetActiveEnhancements(validPowerId, initialEnhancementId, _normalEnhs, _inventionEnhs);

            // 6. Trigger a redraw of the control
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
                    {
                        if (TryGetEnhancementDisplayLevelRange(resolvedEnhancementId, out var enhIoMin, out var enhIoMax))
                        {
                            ioMax = enhIoMax;
                            ioMin = enhIoMin;
                        }

                        break;
                    }
                case Enums.eType.SetO:
                    {
                        if (TryGetEnhancementDisplayLevelRange(resolvedEnhancementId, out var enhIoMin, out var enhIoMax))
                        {
                            ioMax = enhIoMax;
                            ioMin = enhIoMin;
                        }
                        else if (_model.View.SetId > -1 && _model.View.SetTypeId > -1)
                        {
                            var setList = DatabaseAPI.Database.EnhancementSets;
                            var setId = _model.View.SetId;
                            var set = setList.ElementAtOrDefault(setId);
                            if (set is not null)
                            {
                                ioMax = set.LevelMax + 1;
                                ioMin = set.LevelMin + 1;
                            }
                        }

                        break;
                    }
            }

            // Clamp level to valid range
            fixedLevel = Math.Clamp(fixedLevel, ioMin, ioMax);

            // Special IO level snapping logic for InventO
            if (_model.View.TabId == Enums.eType.InventO)
            {
                if (ioMax > 50) ioMax = 50;
                fixedLevel = Enhancement.GranularLevelZb(fixedLevel - 1, ioMin - 1, ioMax - 1) + 1;
            }

            return fixedLevel;
        }

        private static bool TryGetEnhancementDisplayLevelRange(int enhancementId, out int ioMin, out int ioMax)
        {
            ioMin = 10;
            ioMax = 50;
            if (!IsValidEnhancementId(enhancementId))
            {
                return false;
            }

            var enhancement = DatabaseAPI.Database.Enhancements.ElementAtOrDefault(enhancementId);
            if (enhancement is null)
            {
                return false;
            }

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

        #endregion

        #region Transparency

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Don't paint background — allows transparency
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using var path = RoundedRect(ClientRectangle, CornerRadius);
            Region = new Region(path);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (_themeHooked || DesignMode)
            {
                return;
            }

            _themeChangedHandler = Invalidate;
            ThemeManager.ThemeChanged += _themeChangedHandler;
            _themeHooked = true;
        }

        #endregion

        #region Event Handlers

        private void I9Picker_MouseWheel(object? sender, MouseEventArgs e)
        {
            int listLength = GetVisibleRailItemCount();
            if (listLength <= MaxVisibleGradeIcons)
            {
                return;
            }

            int step = IconSize + IconSpacing;
            _scrollOffset -= Math.Sign(e.Delta) * step;
            _scrollOffset = ClampRailScrollOffset(_scrollOffset, listLength);
            Invalidate();
        }

        private void I9Picker_MouseMove(object? sender, MouseEventArgs e)
        {
            var pt = e.Location;

            bool thumbHovered = _railScrollbarThumbRect.Contains(pt);
            bool railHoverChanged = _railHoveringThumb != thumbHovered;
            _railHoveringThumb = thumbHovered;

            if (_railDraggingThumb)
            {
                DragRailScrollbar(e.Y);
                return;
            }

            if (!_railScrollbarBounds.IsEmpty && _railScrollbarBounds.Contains(pt))
            {
                ClearHoverStateIfNeeded();
                if (railHoverChanged)
                {
                    Invalidate(_railScrollbarBounds);
                }

                return;
            }

            foreach (var (rect, i) in _headerRects)
            {
                if (rect.Contains(pt))
                {
                    if (_hoverHeaderIndex != i)
                    {
                        _hoverHeaderIndex = i;
                        _hoverEnhIndex = -1;
                        _hoverSetIndex = -1;
                        var info = (Enums.eType)i switch
                        {
                            Enums.eType.None => "No Enhancement",
                            Enums.eType.Normal => "Normal Enhancements",
                            Enums.eType.InventO => "Invention Origin (IO)",
                            Enums.eType.SpecialO => "Special Enhancements",
                            Enums.eType.SetO => "Invention Sets",
                            _ => ""
                        };
                        _hoverInfo = info;
                        _hoverText = string.Empty;
                        Invalidate();
                    }
                    return;
                }
            }

            foreach (var (rect, i2) in _enhancementRects)
            {
                if (!rect.Contains(pt)) continue;

                if (_hoverEnhIndex == i2) return;

                _hoverEnhIndex = i2;
                _hoverSetIndex = -1;
                _hoverHeaderIndex = -1;

                if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid)
                {
                    // Hovering a SET tile (not yet inside the set). Raise HoverSet with the concrete setId.
                    if (i2 < _model.SetIds.Length)
                    {
                        int setId = _model.SetIds[i2];
                        var setData = DatabaseAPI.Database.EnhancementSets[setId];
                        var setTypeName = DatabaseAPI.GetSetTypeByIndex(setData.SetType).Name;
                        var info = $"{setData.DisplayName}\nType: {setTypeName}     Level Range: {setData.LevelMin + 1}-{setData.LevelMax + 1}";
                        SetHoverText(info, "Click to view enhancements in this set.");
                        RaiseHoverSetEvent(setId);
                    }
                }
                else if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid)
                {
                    if (i2 < _model.SetVariants.Length && _model.View.SetId > -1)
                    {
                        var variantKind = _model.SetVariants[i2];
                        SetHoverText($"{DatabaseAPI.Database.EnhancementSets[_model.View.SetId].DisplayName} - {GetSetVariantDisplayName(variantKind)}", "Click to view the pieces in this variant.");
                    }
                }
                else
                {
                    // Hovering an enhancement tile in any tab
                    if (i2 < _model.EnhancementIds.Length)
                    {
                        int enhId = _model.EnhancementIds[i2];
                        SetHoverText(GetDisplayNameForEnhancement(enhId), GetHoverTextForEnhancement(enhId));
                        RaiseHoverEnhancementEvent(enhId);
                    }
                }

                Invalidate();
                return;
            }

            foreach (var (rect, i3) in _gradeRects)
            {
                if (rect.Contains(pt))
                {
                    if (_hoverSetIndex == i3) return;
                    _hoverSetIndex = i3;
                    _hoverEnhIndex = -1;
                    _hoverHeaderIndex = -1;

                    // Look up correct info based on the active tab
                    switch (_model.View.TabId)
                    {
                        case Enums.eType.Normal:
                            var grade = (Enums.eEnhGrade)_model.NoGrades[i3];
                            _hoverInfo = DatabaseAPI.Database.EnhGradeStringLong[(int)grade];
                            _hoverText = "";
                            break;
                        case Enums.eType.SpecialO:
                            var special = DatabaseAPI.GetSpecialEnhByIndex(_model.SpecialTypes[i3]);
                            _hoverInfo = special.Name;
                            _hoverText = special.Description;
                            break;
                        case Enums.eType.SetO:
                            // This column ALWAYS shows Set Types, so we ALWAYS look up Set Type info.
                            var setType = DatabaseAPI.GetSetTypeByIndex(_model.SetTypes[i3]);
                            _hoverInfo = setType.Name;
                            _hoverText = "Click to view sets in this category.";
                            break;
                    }

                    Invalidate();
                    return;
                }
            }

            if (railHoverChanged)
            {
                Invalidate(_railScrollbarBounds);
            }

            if (!ClientRectangle.Contains(pt))
            {
                EnhancementSelectionCancelled?.Invoke();
                return;
            }

            // Clear hover if not over anything
            if (_hoverHeaderIndex != -1 || _hoverEnhIndex != -1 || _hoverSetIndex != -1)
            {
                _hoverHeaderIndex = -1;
                _hoverEnhIndex = -1;
                _hoverSetIndex = -1;
                SetHoverText(string.Empty, string.Empty);
                Invalidate();
            }
        }

        private void I9Picker_MouseDown(object? sender, MouseEventArgs e)
        {
            var pt = e.Location;

            // Check for cancel click (e.g., 'X' button or edge logic if you implement it)
            // For now assume right-click cancels
            if (e.Button == MouseButtons.Right)
            {
                EnhancementSelectionCancelled?.Invoke();
                return;
            }

            if (TryHandleRailScrollbarMouseDown(e))
            {
                return;
            }

            // 1. Header click = change tab
            for (int i = 0; i < _headerRects.Count; i++)
            {
                if (_headerRects[i].Bounds.Contains(pt))
                {
                    _hoverHeaderIndex = i;

                    // Set TabId based on clicked index (same order used in drawing headers)
                    var newTabId = i switch
                    {
                        0 => Enums.eType.None,
                        1 => Enums.eType.Normal,
                        2 => Enums.eType.InventO,
                        3 => Enums.eType.SpecialO,
                        4 => Enums.eType.SetO,
                        _ => _model.View.TabId
                    };

                    if (newTabId == Enums.eType.SetO && _model.View.TabId == Enums.eType.SetO && StepBackSetSelection())
                    {
                        Invalidate();
                        return;
                    }

                    _model.View.TabId = newTabId;

                    _model.View.RelLevel = ValidateRelativeLevel(_model.View.RelLevel, _model.View.TabId, -1);

                    if (newTabId != Enums.eType.None)
                    {
                        _lastTab = newTabId; // persist last-tab immediately upon tab switch
                    }

                    if (newTabId == Enums.eType.None)
                    {
                        EnhancementPicked?.Invoke(new I9Slot()); // Invoke with a new, empty slot.
                        return;
                    }

                    // Reset selection state
                    ResetSetSelection();
                    _hoverSetIndex = -1;
                    _scrollOffset = 0;

                    SetActiveEnhancements(_powerId, -1, _normalEnhs, _inventionEnhs);
                    Invalidate(); // Redraw with updated state
                    return;
                }
            }

            // 2. Set column click = update grade/setType/setId
            foreach (var (rect, index) in _gradeRects)
            {
                if (rect.Contains(pt))
                {
                    switch (_model.View.TabId)
                    {
                        case Enums.eType.Normal:
                            _model.View.GradeId = (Enums.eEnhGrade)_model.NoGrades[index];
                            _lastGrade = _model.View.GradeId;
                            break;
                        case Enums.eType.SpecialO:
                            _model.View.SpecialId = _model.SpecialTypes[index];
                            _lastSpecial = _model.View.SpecialId;
                            SetActiveEnhancements(_powerId, -1, _normalEnhs, _inventionEnhs);
                            break;
                        case Enums.eType.SetO:
                            _lastSet = index;
                            EnterSetType(index);
                            break;
                    }
                    // The grid will now redraw showing the sets.
                    Invalidate();
                    return;
                }
            }

            // Enhancement click
            foreach (var (rect, index) in _enhancementRects)
            {
                if (rect.Contains(pt))
                {
                    if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid)
                    {
                        _model.View.SetId = _model.SetIds[index];
                        _model.View.SetVariant = null;
                        OpenSelectedSetFamily(-1);
                        Invalidate();
                    }
                    else if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid)
                    {
                        _model.View.SetVariant = _model.SetVariants[index];
                        SetActiveEnhancements(_powerId, -1, _normalEnhs, _inventionEnhs);
                        Invalidate();
                    }
                    else
                    {
                        // This is a final enhancement selection.
                        int enhId = _model.EnhancementIds[index];
                        var slot = CreateSlotForEnh(enhId, index);
                        if (slot != null)
                        {
                            PersistLastStateForCurrentView();
                            EnhancementPicked?.Invoke(slot);
                        }
                    }
                    return;
                }
            }

            // LVL click
            if (_lvlMinusRect.Contains(pt))
            {
                AdjustRelativeLevel(-1);
                Invalidate();
                return;
            }

            if (_lvlPlusRect.Contains(pt))
            {
                AdjustRelativeLevel(+1);
                Invalidate();
            }
        }

        private void I9Picker_MouseUp(object? sender, MouseEventArgs e)
        {
            if (_railDraggingThumb)
            {
                _railDraggingThumb = false;
                Capture = false;
                Invalidate(_railScrollbarBounds);
            }
        }

        private void I9Picker_MouseLeave(object? sender, EventArgs e)
        {
            if (_railDraggingThumb)
            {
                _railDraggingThumb = false;
                Capture = false;
            }

            if (_railHoveringThumb)
            {
                _railHoveringThumb = false;
                if (!_railScrollbarBounds.IsEmpty)
                {
                    Invalidate(_railScrollbarBounds);
                }
            }
        }

        private void I9Picker_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Add:
                case Keys.Oemplus:
                    AdjustRelativeLevel(+1);
                    break;

                case Keys.Subtract:
                case Keys.OemMinus:
                    AdjustRelativeLevel(-1);
                    break;

                case Keys.Enter:
                    if (_hoverEnhIndex >= 0 && _hoverEnhIndex < _model.EnhancementIds.Length)
                    {
                        int enhId = _model.EnhancementIds[_hoverEnhIndex];
                        var slot = CreateSlotForEnh(enhId, _hoverEnhIndex);
                        if (slot != null)
                        {
                            PersistLastStateForCurrentView();
                            EnhancementPicked?.Invoke(slot);
                        }
                    }
                    break;

                case Keys.Escape:
                    EnhancementSelectionCancelled?.Invoke();
                    break;
            }

            Invalidate();
        }

        #endregion

        #region Rendering

        protected override void OnPaint(PaintEventArgs e)
        {
            _enhancementRects.Clear();
            _gradeRects.Clear();
            _headerRects.Clear();
            _lvlPlusRect = Rectangle.Empty;
            _lvlMinusRect = Rectangle.Empty;
            _railScrollbarBounds = Rectangle.Empty;
            _railScrollbarTrackBounds = Rectangle.Empty;
            _railScrollbarThumbRect = Rectangle.Empty;

            if (_buffer == null)
            {
                RecreateBuffer();
            }

            var g = _buffer?.Graphics;

            g.Clear(Color.Transparent);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            DrawOuterFrame(g);
            DrawHeaderBox(g, out var headerBoxRect);
            DrawInfoBox(g, headerBoxRect, out var infoBoxRect);
            DefineHeaderRects(infoBoxRect, out var lastHeaderIcon);
            DrawSelectorRailPanels(g, infoBoxRect, lastHeaderIcon);
            DrawTypeIcons(g);
            DrawEnhancementGrid(g, lastHeaderIcon.Bottom + IconSpacing * 2, out var enhGridBounds);
            DrawGradeColumn(g, lastHeaderIcon.X, enhGridBounds.Top, enhGridBounds.Bottom);
            DrawFooterBox(g, out var leftTextRect);
            DrawLevelBox(g, leftTextRect);

            _buffer?.Render(e.Graphics);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_themeHooked && _themeChangedHandler is not null)
                {
                    ThemeManager.ThemeChanged -= _themeChangedHandler;
                    _themeHooked = false;
                    _themeChangedHandler = null;
                }

                _buffer?.Dispose();
                Region?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void RecreateBuffer()
        {
            _buffer?.Dispose();
            if (Width > 0 && Height > 0)
            {
                _buffer = _context.Allocate(CreateGraphics(), ClientRectangle);
            }

            Invalidate();
        }

        #endregion

        #region Drawing

        private void DrawOuterFrame(Graphics g)
        {
            var bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            var palette = CurrentPalette;
            using var path = RoundedRect(bounds, CornerRadius);
            using var fill = new LinearGradientBrush(bounds, palette.BackgroundTop, palette.BackgroundBottom, LinearGradientMode.Vertical);
            using var pen = new Pen(palette.Border, 1.5f);
            g.FillPath(fill, path);
            g.DrawPath(pen, path);
        }

        private void DrawHeaderBox(Graphics g, out Rectangle rect)
        {
            int totalWidth = Width - PaddingOuter * 2;
            rect = new Rectangle(PaddingOuter, PaddingOuter, totalWidth, HeaderBoxHeight);
            var palette = CurrentPalette;
            DrawRoundedBox(g, rect, CornerRadius, palette.HeaderTop, palette.HeaderBottom, palette.Border);

            using var font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            using var textBrush = new SolidBrush(palette.Text);

            string title = _hoverTitle ?? "Enhancing...";
            g.DrawString(title, font, textBrush, rect,
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        private void DrawInfoBox(Graphics g, Rectangle headerRect, out Rectangle infoBoxRect)
        {
            int y = headerRect.Bottom + IconSpacing;
            infoBoxRect = new Rectangle(PaddingOuter, y, headerRect.Width, InfoBoxHeight);
            var palette = CurrentPalette;

            DrawRoundedBox(g, infoBoxRect, CornerRadius, palette.PanelTop, palette.PanelBottom, palette.Border);

            using var font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            using var textBrush = new SolidBrush(palette.Text);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            string info = _hoverInfo ?? string.Empty;
            g.DrawString(info, font, textBrush, infoBoxRect, sf);
        }

        private void DrawSelectorRailPanels(Graphics g, Rectangle infoBoxRect, Rectangle lastHeaderIcon)
        {
            int top = infoBoxRect.Bottom + IconSpacing - 2;
            int left = PaddingOuter - 2;
            int railLeft = lastHeaderIcon.X - 3;
            int railRight = lastHeaderIcon.Right + RailExtraWidth + RailScrollbarWidth + 3;
            int topRailBottom = top + IconSize + IconSpacing;
            int railBottom = top + IconSize + IconSpacing * 2 +
                             MaxVisibleGradeIcons * IconSize + (MaxVisibleGradeIcons - 1) * IconSpacing + PaddingOuter / 2;
            var palette = CurrentPalette;

            var topRail = Rectangle.FromLTRB(left, top, railRight, topRailBottom);
            var rightRail = Rectangle.FromLTRB(railLeft, top, railRight, railBottom);

            DrawRoundedBox(g, topRail, CornerRadius, palette.SelectorTop, palette.SelectorBottom, palette.SelectorBorder);
            DrawRoundedBox(g, rightRail, CornerRadius, palette.SelectorTop, palette.SelectorBottom, palette.SelectorBorder);

            using var separator = new Pen(Color.FromArgb(170, palette.RailBorder), 1f);
            using var softSeparator = new Pen(Color.FromArgb(70, palette.RailBorder), 1f);
            g.DrawLine(separator, railLeft - 3, topRailBottom + 1, railLeft - 3, railBottom - 2);
            g.DrawLine(softSeparator, railLeft - 1, topRailBottom + 1, railLeft - 1, railBottom - 2);
        }



        private void DrawTypeIcons(Graphics g)
        {
            foreach (var (bounds, index) in _headerRects)
            {
                bool selected = _model.View.TabId == (Enums.eType)index;
                bool hovered = _hoverHeaderIndex == index;

                if (AssetManager.EnhTypes.TryGetValue(index, out var iconBitmap))
                {
                    if (iconBitmap?.Bitmap != null)
                    {
                        // Draw the entire individual icon. No clipping is needed.
                        g.DrawImage(iconBitmap.Bitmap, IconContentRect(bounds));
                    }
                }

                DrawIconFrame(g, bounds, selected, hovered);
            }
        }

        private void DrawEnhancementGrid(Graphics g, int top, out Rectangle gridBounds)
        {
            int left = PaddingOuter;
            int width = EnhGridCols * IconSize + (EnhGridCols - 1) * IconSpacing;
            int height = EnhGridRows * IconSize + (EnhGridRows - 1) * IconSpacing;
            gridBounds = new Rectangle(left, top, width, height);
            _enhancementRects.Clear();

            if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetFamilyGrid && _model.View.SetTypeId > -1)
            {
                for (int i = 0; i < _model.SetIds.Length; i++)
                {
                    int col = i % EnhGridCols;
                    int row = i / EnhGridCols;
                    int x = gridBounds.Left + col * (IconSize + IconSpacing);
                    int y = gridBounds.Top + row * (IconSize + IconSpacing);
                    var rect = new Rectangle(x, y, IconSize, IconSize);

                    _enhancementRects.Add((rect, i)); // Use the grid's rects list for sets temporarily
                    bool hovered = _hoverEnhIndex == i;

                    var setId = _model.SetIds[i];
                    AssetManager.DrawEnhancementSet(g, IconContentRect(rect), setId);

                    DrawIconFrame(g, rect, false, hovered);
                }
                return; // Stop here to prevent drawing enhancements underneath
            }

            if (_model.View.TabId == Enums.eType.SetO && _model.View.SetStage == SetPickerStage.SetVariantGrid && _model.View.SetId > -1)
            {
                for (int i = 0; i < _model.SetVariants.Length; i++)
                {
                    int col = i % EnhGridCols;
                    int row = i / EnhGridCols;
                    int x = gridBounds.Left + col * (IconSize + IconSpacing);
                    int y = gridBounds.Top + row * (IconSize + IconSpacing);
                    var rect = new Rectangle(x, y, IconSize, IconSize);

                    _enhancementRects.Add((rect, i));
                    bool selected = _model.View.SetVariant == _model.SetVariants[i];
                    bool hovered = _hoverEnhIndex == i;

                    AssetManager.DrawEnhancementSetVariant(g, IconContentRect(rect), _model.View.SetId, _model.SetVariants[i]);
                    DrawIconFrame(g, rect, selected, hovered);
                }
                return;
            }

            for (int row = 0; row < EnhGridRows; row++)
            {
                for (int col = 0; col < EnhGridCols; col++)
                {
                    int index = row * EnhGridCols + col;
                    if (index >= _model.EnhancementIds.Length)
                        continue;

                    int x = gridBounds.Left + col * (IconSize + IconSpacing);
                    int y = gridBounds.Top + row * (IconSize + IconSpacing);
                    var rect = new Rectangle(x, y, IconSize, IconSize);

                    _enhancementRects.Add((rect, index));
                    bool selected = _model.View.PickerId == index;
                    bool hovered = _hoverEnhIndex == index;

                    int enhId = _model.EnhancementIds[index];
                    int iconIndex = GetEnhImageIndex(enhId);
                    if (iconIndex >= 0 && iconIndex < AssetManager.Enhancements.Count)
                    {
                        bool disabled = IsEnhancementGrayed(index);
                        using var attr = GetImageAttributes(disabled);
                        AssetManager.DrawEnhancementAt(g, IconContentRect(rect), iconIndex, enhId, _model.View.TabId, _model.View.GradeId, attr);
                        DrawIconFrame(g, rect, selected, hovered, disabled);
                    }
                }
            }
        }

        private void DrawGradeColumn(Graphics g, int columnLeft, int topEnh, int bottomEnh)
        {
            int top = topEnh;
            int bottom = bottomEnh;
            int availableHeight = bottom - top;
            int iconLaneWidth = IconSize + RailExtraWidth;
            int step = IconSize + IconSpacing;

            var clipRect = new Rectangle(columnLeft - 1, top, iconLaneWidth + 2, availableHeight);
            Region oldClip = g.Clip;
            g.SetClip(clipRect);

            _gradeRects.Clear();

            var tabId = _model.View.TabId;
            int[]? indices = null;
            Dictionary<int, ExtendedBitmap>? sourceDictionary = null;

            if (tabId == Enums.eType.Normal)
            {
                indices = _model.NoGrades;
                sourceDictionary = AssetManager.EnhGrades;
            }
            else if (tabId == Enums.eType.SpecialO)
            {
                indices = _model.SpecialTypes;
                sourceDictionary = AssetManager.EnhSpecials;
            }
            else if (tabId == Enums.eType.SetO)
            {
                indices = _model.SetTypes;
                sourceDictionary = AssetManager.SetTypes;
            }

            int loopStart = tabId == Enums.eType.SpecialO ? 1 : 0;
            int itemCount = Math.Max(0, (indices?.Length ?? 0) - loopStart);
            _scrollOffset = ClampRailScrollOffset(_scrollOffset, itemCount);
            int maxScrollRows = Math.Max(0, itemCount - MaxVisibleGradeIcons);
            int scrollRows = Math.Min(_scrollOffset / step, maxScrollRows);
            if (indices != null && sourceDictionary != null)
            {
                for (int i = 0; i < MaxVisibleGradeIcons; i++)
                {
                    int dataIndex = loopStart + i + scrollRows;
                    int y = top + i * step;
                    var rect = new Rectangle(columnLeft, y, IconSize, IconSize);

                    if (dataIndex >= loopStart && dataIndex < indices.Length)
                    {
                        _gradeRects.Add((rect, dataIndex));

                        bool selected = IsSelectorIndexSelected(tabId, indices, dataIndex);
                        bool hovered = _hoverSetIndex == dataIndex;

                        int iconKey = indices[dataIndex];
                        if (sourceDictionary.TryGetValue(iconKey, out var iconToDraw) && iconToDraw?.Bitmap != null)
                        {
                            if (tabId == Enums.eType.Normal)
                            {
                                var gradeBorder = AssetManager.ToGfxGrade(Enums.eType.Normal, (Enums.eEnhGrade)iconKey);
                                if (AssetManager.TryGetBorderBitmap(gradeBorder, out var borderImage) && borderImage?.Bitmap != null)
                                {
                                    g.DrawImage(borderImage.Bitmap, IconContentRect(rect));
                                }
                            }

                            g.DrawImage(iconToDraw.Bitmap, IconContentRect(rect));
                        }

                        DrawIconFrame(g, rect, selected, hovered);
                    }
                }
            }

            g.Clip = oldClip;
            DrawRailScrollbar(g, new Rectangle(columnLeft + iconLaneWidth, topEnh, RailScrollbarWidth, availableHeight), itemCount, step);
        }

        private void DrawFooterBox(Graphics g, out Rectangle leftTextBox)
        {
            int y = Height - PaddingOuter - FooterBoxHeight;
            int leftTextWidth = Width - IconSize - PaddingOuter * 3;

            leftTextBox = new Rectangle(PaddingOuter, y, leftTextWidth, FooterBoxHeight);
            var palette = CurrentPalette;

            DrawRoundedBox(g, leftTextBox, CornerRadius, palette.PanelTop, palette.PanelBottom, palette.Border);

            using var font = new Font("Segoe UI", 9f);
            using var textBrush = new SolidBrush(palette.Text);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisWord
            };

            string footer = _hoverText ?? string.Empty;
            var textRect = Rectangle.Inflate(leftTextBox, -PaddingOuter, -PaddingOuter / 2);
            g.DrawString(footer, font, textBrush, textRect, sf);
        }

        private void DrawLevelBox(Graphics g, Rectangle leftTextBox)
        {
            int y = Height - PaddingOuter - FooterBoxHeight;
            _levelBoxRect = new Rectangle(leftTextBox.Right + PaddingOuter, y, IconSize, FooterBoxHeight);
            var palette = CurrentPalette;

            DrawRoundedBox(g, _levelBoxRect, CornerRadius,
                palette.LevelTop,
                palette.LevelBottom,
                palette.LevelBorder);

            using var fontTitle = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            using var fontValue = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var textBrush = new SolidBrush(palette.LevelText);
            using var lockBrush = new SolidBrush(palette.MutedText);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            var titleRect = new Rectangle(_levelBoxRect.X, _levelBoxRect.Y, _levelBoxRect.Width, _levelBoxRect.Height / 3);
            var valueRect = new Rectangle(_levelBoxRect.X, _levelBoxRect.Y + titleRect.Height, _levelBoxRect.Width, titleRect.Height);

            g.DrawString("LVL", fontTitle, textBrush, titleRect, sf);


            string ioLevelText;

            if (_model.View.TabId is Enums.eType.SpecialO)
            {
                int rel = _model.View.RelLevel.ToInt(); // Convert enum to int
                int finalLevel = _model.View.IoLevel + rel;
                ioLevelText = finalLevel.ToString();
            }
            else
            {
                ioLevelText = _model.View.IoLevel.ToString();
                string relLevelText = GetRelativeLevelLabel(_model.View.RelLevel);

                if (!string.IsNullOrEmpty(relLevelText))
                {
                    ioLevelText += $" {relLevelText}";
                }
            }

            g.DrawString(ioLevelText, fontValue, textBrush, valueRect, sf);

            // Determine if enhancement is adjustable
            bool showAdjustmentButtons = false;
            bool isCatalyst = false;
            bool isAttuned = false;

            if (_model.View.PickerId >= 0 && _model.View.PickerId < _model.EnhancementIds.Length)
            {
                int enhId = _model.EnhancementIds[_model.View.PickerId];
                isCatalyst = _model.HasCatalyst(enhId);
                isAttuned = _model.IsNaturallyAttuned(enhId);
            }

            // Show buttons unless locked
            if (!isCatalyst && !isAttuned)
            {
                showAdjustmentButtons = true;
            }

            int thirdHeight = _levelBoxRect.Height / 3;
            _lvlMinusRect = new Rectangle(_levelBoxRect.X, _levelBoxRect.Bottom - thirdHeight, _levelBoxRect.Width / 2, thirdHeight);
            _lvlPlusRect = new Rectangle(_levelBoxRect.X + _levelBoxRect.Width / 2, _levelBoxRect.Bottom - thirdHeight, _levelBoxRect.Width / 2, thirdHeight);

            if (showAdjustmentButtons)
            {
                g.DrawString("-", fontValue, textBrush, _lvlMinusRect, sf);
                g.DrawString("+", fontValue, textBrush, _lvlPlusRect, sf);
            }
            else
            {
                using var lockFont = new Font("Segoe MDL2 Assets", 10.5f, FontStyle.Bold); // Modern icon font
                g.DrawString("\uE72E", lockFont, lockBrush, _lvlMinusRect, sf); // Unicode for 'Lock'
                g.DrawString("\uE72E", lockFont, lockBrush, _lvlPlusRect, sf);
            }
        }


        #endregion

        #region Drawing Helpers

        private void DefineHeaderRects(Rectangle previousRect, out Rectangle lastHeaderIcon)
        {
            int top = previousRect.Bottom + IconSpacing;
            int left = PaddingOuter;
            lastHeaderIcon = Rectangle.Empty;

            for (int i = 0; i < TypeIconCount; i++)
            {
                var x = left + i * (IconSize + IconSpacing);
                if (i == TypeIconCount - 1)
                {
                    x = left + i * (IconSize + IconSpacing + 2); // Slight adjustment for spacing
                }

                var rect = new Rectangle(x, top, IconSize, IconSize);
                _headerRects.Add((rect, i));

                if (i == TypeIconCount - 1)
                {
                    lastHeaderIcon = rect;
                }
            }
        }

        private void DrawIconFrame(Graphics g, Rectangle rect, bool selected, bool hovered, bool disabled = false)
        {
            var palette = CurrentPalette;

            if (disabled)
            {
                using var disabledPath = RoundedRect(rect, 7);
                using var disabledBrush = new SolidBrush(Color.FromArgb(72, palette.BackgroundTop));
                g.FillPath(disabledBrush, disabledPath);
            }

            if (!selected && !hovered)
            {
                return;
            }

            var borderRect = Rectangle.Inflate(rect, -1, -1);
            using var path = RoundedRect(borderRect, 7);
            using var pen = new Pen(selected ? palette.Accent : Color.FromArgb(190, palette.Accent), selected ? 2f : 1.4f);
            g.DrawPath(pen, path);
        }

        private bool IsSelectorIndexSelected(Enums.eType tabId, int[] indices, int dataIndex)
        {
            if (dataIndex < 0 || dataIndex >= indices.Length)
            {
                return false;
            }

            return tabId switch
            {
                Enums.eType.Normal => _model.View.GradeId == (Enums.eEnhGrade)indices[dataIndex],
                Enums.eType.SpecialO => _model.View.SpecialId == indices[dataIndex],
                Enums.eType.SetO => _model.View.SetTypeId == dataIndex,
                _ => false
            };
        }

        private static Rectangle IconContentRect(Rectangle rect)
        {
            return Rectangle.Inflate(rect, -IconInset, -IconInset);
        }


        private static void DrawRoundedBox(Graphics g, Rectangle rect, int radius, Color innerColor, Color outerColor, Color border)
        {
            using var path = RoundedRect(rect, radius);
            using var fill = new LinearGradientBrush(rect, outerColor, innerColor, LinearGradientMode.Vertical);
            using var pen = new Pen(border, 1.0f);
            g.FillPath(fill, path);
            g.DrawPath(pen, path);
        }

        private void DrawRailScrollbar(Graphics g, Rectangle bounds, int itemCount, int step)
        {
            _railScrollbarBounds = Rectangle.Empty;
            _railScrollbarTrackBounds = Rectangle.Empty;
            _railScrollbarThumbRect = Rectangle.Empty;

            if (itemCount <= MaxVisibleGradeIcons || bounds.Width <= 0 || bounds.Height <= 0)
            {
                _railHoveringThumb = false;
                return;
            }

            _railScrollbarBounds = bounds;
            _railScrollbarTrackBounds = Rectangle.FromLTRB(
                bounds.Left + RailScrollbarInset,
                bounds.Top + RailScrollbarInset,
                bounds.Right - RailScrollbarInset,
                bounds.Bottom - RailScrollbarInset);

            int scrollMax = Math.Max(0, (itemCount - MaxVisibleGradeIcons) * step);
            int trackHeight = Math.Max(0, _railScrollbarTrackBounds.Height);
            int thumbHeight = Math.Max(
                RailScrollbarThumbMinHeight,
                (int)Math.Round((double)MaxVisibleGradeIcons / itemCount * trackHeight));
            thumbHeight = Math.Min(thumbHeight, trackHeight);

            int available = Math.Max(0, trackHeight - thumbHeight);
            int thumbY = _railScrollbarTrackBounds.Top;
            if (available > 0 && scrollMax > 0)
            {
                double ratio = (double)_scrollOffset / scrollMax;
                thumbY = _railScrollbarTrackBounds.Top + (int)Math.Round(available * ratio);
            }

            int inset = Math.Max(1, bounds.Width / 4);
            _railScrollbarThumbRect = new Rectangle(
                bounds.Left + inset,
                thumbY,
                Math.Max(1, bounds.Width - inset * 2),
                thumbHeight);

            var theme = CurrentScrollTheme;
            using var trackPen = new Pen(theme.Track, 2f);
            int centerX = bounds.Left + bounds.Width / 2;
            g.DrawLine(trackPen, centerX, _railScrollbarTrackBounds.Top, centerX, _railScrollbarTrackBounds.Bottom);

            using var thumbBrush = new SolidBrush(_railHoveringThumb || _railDraggingThumb ? theme.Hover : theme.Bar);
            g.FillRectangle(thumbBrush, _railScrollbarThumbRect);
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion

        #region Other Helpers

        private EnhUniqueStatus ComputeUniqueStatusForEnhancement(int enhId)
        {
            var enhData = DatabaseAPI.Database.Enhancements[enhId];
            // If power index not known, comparisons limited to global uniqueness checks
            int? powerStatic = (_powerId >= 0) ? DatabaseAPI.Database.Power[_powerId]?.StaticIndex : null;

            bool InMain, InAlternate;

            if (enhData.TypeID == Enums.eType.SetO && DatabaseAPI.TryGetSetPieceIndexForEnhancement(enhId, out _, out _))
            {
                bool SamePower(PowerEntry? p) =>
                    powerStatic is null || p?.Power?.StaticIndex == powerStatic;

                InMain = (_slotted?.Any(s => DatabaseAPI.AreEnhancementsSameSetPiece(s, enhId)) == true)
                         || MidsContext.Character.CurrentBuild.Powers
                             .Where(p => p is { Power.Slottable: true } && SamePower(p))
                             .Any(p => p?.Slots.Any(s => DatabaseAPI.AreEnhancementsSameSetPiece(s.Enhancement.Enh, enhId)) == true);

                InAlternate = (_slotted?.Any(s => DatabaseAPI.AreEnhancementsSameSetPiece(s, enhId)) == true)
                              || MidsContext.Character.CurrentBuild.Powers
                                  .Where(p => p is { Power.Slottable: true } && SamePower(p))
                                  .Any(p => p?.Slots.Any(s => DatabaseAPI.AreEnhancementsSameSetPiece(s.FlippedEnhancement.Enh, enhId)) == true);
            }
            else if (enhData.Unique)
            {
                InMain = (_slotted?.Any(s => s == enhId) == true)
                         || MidsContext.Character.CurrentBuild.Powers
                             .Where(p => p is { Power.Slottable: true })
                             .Any(p => p?.Slots.Any(s => s.Enhancement.Enh == enhId) == true);

                InAlternate = (_slotted?.Any(s => s == enhId) == true)
                              || MidsContext.Character.CurrentBuild.Powers
                                  .Where(p => p is { Power.Slottable: true })
                                  .Any(p => p?.Slots.Any(s => s.FlippedEnhancement.Enh == enhId) == true);
            }
            else
            {
                // For non-unique, scope to the same power when powerStatic is available
                bool SamePower(PowerEntry? p) =>
                    powerStatic is null || p?.Power?.StaticIndex == powerStatic;

                InMain = (_slotted?.Any(s => s == enhId) == true)
                         || MidsContext.Character.CurrentBuild.Powers
                             .Where(p => p is { Power.Slottable: true } && SamePower(p))
                             .Any(p => p?.Slots.Any(s => s.Enhancement.Enh == enhId) == true);

                InAlternate = (_slotted?.Any(s => s == enhId) == true)
                              || MidsContext.Character.CurrentBuild.Powers
                                  .Where(p => p is { Power.Slottable: true } && SamePower(p))
                                  .Any(p => p?.Slots.Any(s => s.FlippedEnhancement.Enh == enhId) == true);
            }

            return new EnhUniqueStatus { InMain = InMain, InAlternate = InAlternate };
        }

        private void RaiseHoverEnhancementEvent(int enhId)
        {
            var status = ComputeUniqueStatusForEnhancement(enhId);
            HoverEnhancement?.Invoke(enhId, status);
        }

        private void RaiseHoverSetEvent(int setId)
        {
            HoverSet?.Invoke(setId);
        }

        // Persist “last used” state whenever a final selection is made
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

        private int CalculateMaxHeight()
        {
            // 1. Calculate the grid's maximum possible height using the constant.
            int maxGridHeight = EnhGridRows * IconSize + (EnhGridRows - 1) * IconSpacing;

            // 2. Calculate the fixed height of the Grade Column.
            int gradeColumnHeight = MaxVisibleGradeIcons * IconSize + (MaxVisibleGradeIcons - 1) * IconSpacing;

            // 3. Determine the tallest content area needed.
            int tallestContentHeight = Math.Max(maxGridHeight, gradeColumnHeight);

            // 4. Sum all components to get the final total height.
            int totalHeight = PaddingOuter +
                              HeaderBoxHeight +
                              IconSpacing +
                              InfoBoxHeight +
                              IconSpacing +
                              IconSize + // For the Type Icons
                              IconSpacing * 2 +
                              tallestContentHeight +
                              IconSpacing +
                              FooterBoxHeight +
                              PaddingOuter;

            return totalHeight;
        }

        private static int CalculateMaxWidth()
        {
            var railLeft = PaddingOuter + (TypeIconCount - 1) * (IconSize + IconSpacing + 2);

            return railLeft + IconSize + RailExtraWidth + RailScrollbarWidth + PaddingOuter;
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

        private static int ClampRailScrollOffset(int offset, int itemCount)
        {
            int step = IconSize + IconSpacing;
            int maxScroll = Math.Max(0, (itemCount - MaxVisibleGradeIcons) * step);
            offset = Math.Clamp(offset, 0, maxScroll);
            return step > 0 ? offset / step * step : offset;
        }

        private void ClearHoverStateIfNeeded()
        {
            if (_hoverHeaderIndex != -1 || _hoverEnhIndex != -1 || _hoverSetIndex != -1)
            {
                _hoverHeaderIndex = -1;
                _hoverEnhIndex = -1;
                _hoverSetIndex = -1;
                _hoverInfo = string.Empty;
                _hoverText = string.Empty;
                Invalidate();
            }
        }

        private bool TryHandleRailScrollbarMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _railScrollbarBounds.IsEmpty || !_railScrollbarBounds.Contains(e.Location))
            {
                return false;
            }

            Focus();
            int itemCount = GetVisibleRailItemCount();
            if (itemCount <= MaxVisibleGradeIcons)
            {
                return true;
            }

            if (_railScrollbarThumbRect.Contains(e.Location))
            {
                _railDraggingThumb = true;
                _railDragStartY = e.Y - _railScrollbarThumbRect.Y;
                Capture = true;
                return true;
            }

            if (_railScrollbarTrackBounds.Contains(e.Location))
            {
                int pageStep = MaxVisibleGradeIcons * (IconSize + IconSpacing);
                if (e.Y < _railScrollbarThumbRect.Top)
                {
                    _scrollOffset = ClampRailScrollOffset(_scrollOffset - pageStep, itemCount);
                }
                else if (e.Y > _railScrollbarThumbRect.Bottom)
                {
                    _scrollOffset = ClampRailScrollOffset(_scrollOffset + pageStep, itemCount);
                }

                Invalidate();
                return true;
            }

            return true;
        }

        private void DragRailScrollbar(int mouseY)
        {
            if (_railScrollbarTrackBounds.IsEmpty || _railScrollbarThumbRect.IsEmpty)
            {
                return;
            }

            int itemCount = GetVisibleRailItemCount();
            int step = IconSize + IconSpacing;
            int maxScroll = Math.Max(0, (itemCount - MaxVisibleGradeIcons) * step);
            int available = Math.Max(0, _railScrollbarTrackBounds.Height - _railScrollbarThumbRect.Height);
            if (available <= 0 || maxScroll <= 0)
            {
                return;
            }

            int newThumbY = mouseY - _railDragStartY;
            newThumbY = Math.Max(_railScrollbarTrackBounds.Top, Math.Min(newThumbY, _railScrollbarTrackBounds.Top + available));
            double ratio = (double)(newThumbY - _railScrollbarTrackBounds.Top) / available;
            _scrollOffset = ClampRailScrollOffset((int)Math.Round(ratio * maxScroll), itemCount);
            Invalidate();
        }

        private void SetHoverText(string? info, string? text)
        {
            _hoverInfo = info;
            _hoverText = text;
            Invalidate();
        }

        private Origin.Grade GetGradeForEnhancement(int enhId)
        {
            return _model.View.TabId switch
            {
                Enums.eType.Normal => _model.View.GradeId switch
                {
                    Enums.eEnhGrade.TrainingO => Origin.Grade.TrainingO,
                    Enums.eEnhGrade.DualO => Origin.Grade.DualO,
                    _ => Origin.Grade.SingleO
                },
                Enums.eType.InventO => Origin.Grade.IO,
                Enums.eType.SpecialO => Origin.Grade.HO,
                Enums.eType.SetO => Origin.Grade.SetO,
                _ => Origin.Grade.None
            };
        }

        private int GetEnhImageIndex(int enhId)
        {
            if (enhId < 0 || enhId >= DatabaseAPI.Database.Enhancements.Length)
                return -1;

            var enh = DatabaseAPI.Database.Enhancements[enhId];

            return enh.ImageIdx;
        }

        private string GetDisplayNameForEnhancement(int enhId)
        {
            if (enhId < 0 || enhId >= DatabaseAPI.Database.Enhancements.Length)
            {
                return string.Empty;
            }

            var enhancement = DatabaseAPI.Database.Enhancements[enhId];
            return _model.EnhancementNames.TryGetValue(enhId, out var canonicalName)
                ? canonicalName
                : enhancement.Name;
        }

        private string GetHoverTextForEnhancement(int enhId)
        {
            if (enhId < 0 || enhId >= DatabaseAPI.Database.Enhancements.Length)
            {
                return string.Empty;
            }

            var enhancement = DatabaseAPI.Database.Enhancements[enhId];
            return _model.EnhancementDescriptions.GetValueOrDefault(enhId, enhancement.ShortName ?? string.Empty);
        }

        private bool IsEnhancementGrayed(int index)
        {
            int enhId = _model.EnhancementIds.ElementAtOrDefault(index);
            if (enhId < 0) return false;

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
            for (int r = 0; r <= 2; r++)
            {
                for (int c = 0; c <= 2; c++)
                {
                    if (r != 4) colorMatrix[r, c] /= 2f;
                }
            }

            var attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            return attributes;
        }

        private I9Slot? CreateSlotForEnh(int enhId, int index)
        {
            // Block creation of an invalid slot (grayed)
            if (IsEnhancementGrayed(index))
                return default;

            var slot = new I9Slot
            {
                Enh = enhId,
                RelativeLevel = _model.View.RelLevel
            };

            // IO + Set-level logic
            if (_model.View.TabId is Enums.eType.InventO or Enums.eType.SetO)
                slot.IOLevel = _model.View.IoLevel - 1;

            // Grade logic for Normal tab
            if (_model.View.TabId == Enums.eType.Normal)
                slot.Grade = _model.View.GradeId;

            return slot;
        }

        private void AdjustRelativeLevel(int delta)
        {
            var current = _model.View.RelLevel.ToInt();
            var newRaw = current + delta;

            int enhId = -1;

            // Prefer hovered enhancement
            if (_hoverEnhIndex >= 0 && _hoverEnhIndex < _model.EnhancementIds.Length)
                enhId = _model.EnhancementIds[_hoverEnhIndex];
            else if (_model.View.PickerId >= 0 && _model.View.PickerId < _model.EnhancementIds.Length)
                enhId = _model.EnhancementIds[_model.View.PickerId];

            // Use helper to apply proper bounds per type and catalyst/attunement
            var validated = ValidateRelativeLevel(newRaw.ToEnhRelative(), _model.View.TabId, enhId);
            _model.View.RelLevel = validated;
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

            int val = current.ToInt(); // convert enum to -3 to +5

            int clamped = tabId switch
            {
                Enums.eType.Normal => Math.Clamp(val, -3, 3),
                Enums.eType.SpecialO => Math.Clamp(val, -3, 3),
                Enums.eType.InventO or Enums.eType.SetO => Math.Clamp(val, 0, 5),
                _ => Math.Clamp(val, -3, 5)
            };

            return clamped.ToEnhRelative();
        }

        private string GetRelativeLevelLabel(Enums.eEnhRelative relLevel)
        {
            int value = relLevel.ToInt(); // Logical level: -3 to +5
            return value switch
            {
                > 0 => $"+{value}",
                < 0 => $"{value}",
                _ => string.Empty
            };
        }

        private static int[] GetValidSetTypes(int iPowerIdx)
        {
            return !IsValidPowerId(iPowerIdx) ? [] : DatabaseAPI.Database.Power[iPowerIdx].SetTypes.ToArray();
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

        private static int[] GetSets(int iSetType)
        {
            return DatabaseAPI.Database.EnhancementSets
                .Select((set, index) => new { set, index })
                .Where(e => e.set.SetType == iSetType)
                .Select(e => e.index)
                .ToArray();
        }

        private static List<int> GetValidEnhancements(int iPowerIdx, Enums.eType iType, int iSubType = 0)
        {
            return !IsValidPowerId(iPowerIdx) ? [] : DatabaseAPI.Database.Power[iPowerIdx].GetValidEnhancements(iType, iSubType);
        }

        private static bool IsValidPowerId(int powerId)
        {
            return DatabaseAPI.Database?.Power != null &&
                   powerId >= 0 &&
                   powerId < DatabaseAPI.Database.Power.Length &&
                   DatabaseAPI.Database.Power[powerId] != null;
        }

        private static bool IsValidEnhancementId(int enhancementId)
        {
            return DatabaseAPI.Database?.Enhancements != null &&
                   enhancementId >= 0 &&
                   enhancementId < DatabaseAPI.Database.Enhancements.Length;
        }

        private int SetTypeToId(int iSetType)
        {
            for (var i = 0; i < _model.SetTypes.Length; i++)
            {
                if (iSetType == _model.SetTypes[i])
                {
                    return i;
                }
            }
            return -1;
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
            {
                return false;
            }

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
                {
                    return;
                }
            }

            if (!_model.View.SetVariant.HasValue)
            {
                return;
            }

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
                // Repopulate the dictionaries only if we have enhancements to display.
                _model.EnhancementNames = enhIdList.ToDictionary(id => id, id => DatabaseAPI.Database.Enhancements[id].Name);
                _model.EnhancementDescriptions = enhIdList.ToDictionary(id => id, id => DatabaseAPI.Database.Enhancements[id].ShortName);
            }

            // Find the PickerId if the initial enhancement is in the current view
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

        #endregion

        #region Nested Types

        private sealed record I9PickerPalette(
            Color BackgroundTop,
            Color BackgroundBottom,
            Color PanelTop,
            Color PanelBottom,
            Color Border,
            Color HeaderTop,
            Color HeaderBottom,
            Color SelectorTop,
            Color SelectorBottom,
            Color SelectorBorder,
            Color RailBorder,
            Color Accent,
            Color Text,
            Color MutedText,
            Color LevelTop,
            Color LevelBottom,
            Color LevelBorder,
            Color LevelText)
        {
            public static I9PickerPalette From(DataViewTheme dataView, ButtonTheme button)
            {
                return new I9PickerPalette(
                    dataView.Background,
                    dataView.Card,
                    dataView.Card,
                    dataView.Background,
                    dataView.Border,
                    dataView.HeaderTop,
                    dataView.HeaderBottom,
                    Color.FromArgb(130, dataView.GridHeaderTop),
                    Color.FromArgb(100, dataView.GridHeaderBottom),
                    Color.FromArgb(150, dataView.GridHeaderBorder),
                    dataView.GridHeaderBorder,
                    dataView.Accent,
                    dataView.Text,
                    dataView.Muted,
                    button.GradientTop,
                    button.GradientBottom,
                    button.Border,
                    button.ForeColor);
            }
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

            public int MaxLevel { get; set; } = 50;
            public int MinLevel { get; set; } = 1;

            public bool HasCatalyst(int enhId) =>
                DatabaseAPI.EnhHasCatalyst(DatabaseAPI.Database.Enhancements[enhId].UID);

            public bool IsNaturallyAttuned(int enhId) =>
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

            public EnhSelectorState() { }

            public EnhSelectorState(EnhSelectorState iCl)
            {
                TabId = iCl.TabId;
                PickerId = iCl.PickerId;
                SetTypeId = iCl.SetTypeId;
                GradeId = iCl.GradeId;
                SpecialId = iCl.SpecialId;
                SetId = iCl.SetId;
                IoLevel = iCl.IoLevel;
                SpecialLevel = iCl.SpecialLevel;
                RelLevel = iCl.RelLevel;
                SetVariant = iCl.SetVariant;
                SetStage = iCl.SetStage;
            }
        }

        #endregion
    }
}

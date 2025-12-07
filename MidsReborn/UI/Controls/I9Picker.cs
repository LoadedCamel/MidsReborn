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

        #endregion

        #region Constants

        private const int IconSize = 48;
        private const int IconSpacing = 6;
        private const int PaddingOuter = 8;
        private const int HeaderBoxHeight = 25;
        private const int InfoBoxHeight = 50;
        private const int FooterBoxHeight = 60;
        private const int CornerRadius = 10;
        private const int TypeIconCount = 5;
        private const int EnhGridCols = 4;
        private const int EnhGridRows = 5;
        private const int MaxVisibleGradeIcons = 4;
        private const int ArrowHeight = 12;

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
        private Enums.eEnhRelative _lastRelativeLevel = Enums.eEnhRelative.Even;
        private int _lastSpecial = 1;
        private int _lastSet;

        private int[] _slotted = [];

        public int LastLevel { get; internal set; }

        private readonly List<(Rectangle Bounds, int Index)> _enhancementRects = [];
        private readonly List<(Rectangle Bounds, int Index)> _gradeRects = [];
        private readonly List<(Rectangle Bounds, int Index)> _headerRects = [];

        private Rectangle _levelBoxRect = Rectangle.Empty;
        private Rectangle _lvlPlusRect = Rectangle.Empty;
        private Rectangle _lvlMinusRect = Rectangle.Empty;

        private int _scrollOffset;

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
            Size = new Size(287, maxHeight);
            MouseWheel += I9Picker_MouseWheel;
            MouseMove += I9Picker_MouseMove;
            MouseDown += I9Picker_MouseDown;
            KeyDown += I9Picker_KeyDown;
            TabStop = true;
            Focus();
        }

        #endregion

        #region Public Properties

        public EnhSelectorState View => _model.View;

        #endregion

        #region Public Methods

        public void SetData(int iPower, I9Slot iSlot, int[] slotted)
        {
            // 0. Store the power ID
            _powerId = iPower;
            _hoverTitle = $"Enhancing: {DatabaseAPI.Database.Power[iPower].DisplayName}";
            _slotted = slotted;

            // 1. Reset the model to ensure a clean state
            _model = new EnhSelectorModel();

            // 2. Fetch all possible enhancements and types for the power
            _normalEnhs = GetValidEnhancements(iPower, Enums.eType.Normal).ToArray();
            _inventionEnhs = GetValidEnhancements(iPower, Enums.eType.InventO).ToArray();
            _model.SetTypes = GetValidSetTypes(iPower);
            _model.NoGrades = (int[])Enum.GetValues(typeof(Enums.eEnhGrade));
            _model.SpecialTypes = DatabaseAPI.Database.SpecialEnhancements.Select(x => x.Index).ToArray();

            // 3. Determine the initial state based on the provided slot
            // Start with last-used or default values
            _model.Initial.GradeId = _lastGrade;
            _model.Initial.RelLevel = _lastRelativeLevel;
            _model.Initial.SpecialId = _lastSpecial > 0 ? _lastSpecial : 1;

            // If the slot is already filled, override defaults with its data
            if (iSlot.Enh > -1)
            {
                var enh = DatabaseAPI.Database.Enhancements[iSlot.Enh];
                _hoverText = enh.Desc;

                _model.Initial.TabId = enh.TypeID;
                _model.Initial.GradeId = iSlot.Grade;
                _model.Initial.RelLevel = iSlot.RelativeLevel;
                _model.Initial.IoLevel = iSlot.IOLevel + 1;
                _model.Initial.SpecialId = enh.SubTypeID;

                _model.Initial.RelLevel = ValidateRelativeLevel(_model.Initial.RelLevel, _model.Initial.TabId, iSlot.Enh);

                // Correctly handle pre-selected sets
                if (enh.TypeID == Enums.eType.SetO)
                {
                    int setType = DatabaseAPI.Database.EnhancementSets[enh.nIDSet].SetType;
                    _model.Initial.SetTypeId = SetTypeToId(setType);

                    // IMPORTANT: Populate SetIds ONLY with sets of the correct type
                    _model.SetIds = GetSets(setType);

                    // Find the local index of the specific set within that type
                    _model.Initial.SetId = Array.IndexOf(_model.SetIds, enh.nIDSet);
                }
            }
            else // Otherwise, the slot is empty
            {
                _hoverText = "";

                _model.Initial.TabId = Enums.eType.None;
                _model.Initial.IoLevel = LastLevel > 0 ? LastLevel : MidsContext.Config.I9.DefaultIOLevel + 1;
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
            SetActiveEnhancements(iPower, iSlot.Enh, _normalEnhs, _inventionEnhs);

            // 6. Trigger a redraw of the control
            Invalidate();
        }

        public int CheckAndReturnIoLevel()
        {
            var ioMax = 50;
            var ioMin = 10;
            var fixedLevel = _model.View.IoLevel;

            switch (_model.View.TabId)
            {
                case Enums.eType.InventO:
                    {
                        if (_model.Initial.TabId == _model.View.TabId &&
                            _model.Initial.PickerId == _model.View.PickerId &&
                            _model.View.PickerId > -1)
                        {
                            var enh = DatabaseAPI.Database.Enhancements.ElementAtOrDefault(_model.EnhancementIds[_model.View.PickerId]);
                            if (enh is not null)
                            {
                                ioMax = enh.LevelMax + 1;
                                ioMin = enh.LevelMin + 1;
                            }
                        }

                        break;
                    }
                case Enums.eType.SetO when _model.View.SetId > -1 && _model.View.SetTypeId > -1:
                    {
                        var setList = DatabaseAPI.Database.EnhancementSets;
                        var setId = _model.SetIds.ElementAtOrDefault(_model.View.SetId);
                        var set = setList.ElementAtOrDefault(setId);
                        if (set is not null)
                        {
                            ioMax = set.LevelMax + 1;
                            ioMin = set.LevelMin + 1;
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

        #endregion

        #region Event Handlers

        private void I9Picker_MouseWheel(object? sender, MouseEventArgs e)
        {
            int listLength = 0;
            switch (_model.View.TabId)
            {
                case Enums.eType.Normal:
                    listLength = _model.NoGrades.Length;
                    break;
                case Enums.eType.SpecialO:
                    listLength = _model.SpecialTypes.Length;
                    break;
                case Enums.eType.SetO:
                    // If a type isn't selected yet, we are viewing the list of types.
                    listLength = _model.View.SetTypeId < 0 ? _model.SetTypes.Length : _model.SetIds.Length;
                    break;
            }

            // Now, use the correct length in the guard clause.
            if (listLength <= MaxVisibleGradeIcons)
            {
                return; // This now correctly prevents scrolling.
            }

            int step = IconSize + IconSpacing;
            int maxScroll = (listLength - MaxVisibleGradeIcons) * step;
            _scrollOffset -= Math.Sign(e.Delta) * step;
            _scrollOffset = Math.Clamp(_scrollOffset, 0, maxScroll);
            _scrollOffset = _scrollOffset / step * step; // snap
            Invalidate();
        }

        private void I9Picker_MouseMove(object? sender, MouseEventArgs e)
        {
            var pt = e.Location;

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

                if (_model.View.TabId == Enums.eType.SetO && _model.View.SetId == -1)
                {
                    // Hovering a SET tile (not yet inside the set). Raise HoverSet with the concrete setId.
                    if (i2 < _model.SetIds.Length)
                    {
                        int setId = _model.SetIds[i2];
                        var setData = DatabaseAPI.Database.EnhancementSets[setId];
                        var info = $"{setData.DisplayName}\nType: {Enum.GetName(typeof(Enums.eSetType), setData.SetType)}     Level Range: {setData.LevelMin + 1}-{setData.LevelMax + 1}";
                        SetHoverText(info, "Click to view enhancements in this set.");
                        RaiseHoverSetEvent(setId);
                    }
                }
                else
                {
                    // Hovering an enhancement tile in any tab
                    if (i2 < _model.EnhancementIds.Length)
                    {
                        int enhId = _model.EnhancementIds[i2];
                        if (_model.EnhancementNames.TryGetValue(enhId, out var name))
                        {
                            SetHoverText(name, _model.EnhancementDescriptions.GetValueOrDefault(enhId, ""));
                        }
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
                            var grade = (Enums.eEnhGrade)i3;
                            _hoverInfo = grade.ToString().Replace("O", " Origin");
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
                    _model.View.SetId = -1;
                    _model.View.SetTypeId = -1;
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
                            _model.View.GradeId = (Enums.eEnhGrade)index;
                            _lastGrade = _model.View.GradeId;
                            break;
                        case Enums.eType.SpecialO:
                            _model.View.SpecialId = _model.SpecialTypes[index];
                            _lastSpecial = _model.View.SpecialId;
                            SetActiveEnhancements(_powerId, -1, _normalEnhs, _inventionEnhs);
                            break;
                        case Enums.eType.SetO:
                            _model.View.SetTypeId = index;
                            _lastSet = index;
                            _model.View.SetId = -1;
                            _model.SetIds = GetSets(_model.SetTypes[index]);
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
                    if (_model.View.TabId == Enums.eType.SetO && _model.View.SetId == -1)
                    {
                        // We are selecting a SET from the grid.
                        _model.View.SetId = index; // The 'index' corresponds to the set's position
                        // Now load the enhancements for the selected set.
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
            DrawLShapedPanel(g, infoBoxRect, lastHeaderIcon); // NEW: after header icon rectangles
            DrawTypeIcons(g, infoBoxRect);
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
                _buffer?.Dispose();
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
            using var path = RoundedRect(bounds, CornerRadius);
            using var pen = new Pen(Color.FromArgb(60, 120, 220), 1.5f);
            g.DrawPath(pen, path);
        }

        private void DrawHeaderBox(Graphics g, out Rectangle rect)
        {
            int totalWidth = PaddingOuter + TypeIconCount * (IconSize + IconSpacing) - IconSpacing;
            rect = new Rectangle(PaddingOuter, PaddingOuter, totalWidth, HeaderBoxHeight);
            DrawRoundedBox(g, rect, CornerRadius,
                Color.FromArgb(45, 45, 55),
                Color.FromArgb(30, 30, 35),
                Color.FromArgb(90, 140, 255));

            using var font = new Font("Segoe UI", 9.5f, FontStyle.Bold);

            string title = _hoverTitle ?? "Enhancing...";
            g.DrawString(title, font, Brushes.White, rect,
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        private void DrawInfoBox(Graphics g, Rectangle headerRect, out Rectangle infoBoxRect)
        {
            int y = headerRect.Bottom + IconSpacing;
            infoBoxRect = new Rectangle(PaddingOuter, y, headerRect.Width, InfoBoxHeight);

            DrawRoundedBox(g, infoBoxRect, CornerRadius,
                Color.FromArgb(38, 38, 44),
                Color.FromArgb(30, 30, 35),
                Color.FromArgb(85, 85, 100));

            using var font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            string info = _hoverInfo ?? string.Empty;
            g.DrawString(info, font, Brushes.White, infoBoxRect, sf);
        }

        private void DrawLShapedPanel(Graphics g, Rectangle infoBoxRect, Rectangle lastHeaderIcon)
        {
            int top = infoBoxRect.Bottom + IconSpacing;
            int left = PaddingOuter;
            int headerHeight = IconSize;

            int lVerticalX = lastHeaderIcon.X; // Where vertical L column starts
            int lVerticalWidth = IconSize;

            int lVerticalHeight = EnhGridRows * IconSize + EnhGridRows * IconSpacing;

            // Header bar rectangle
            Rectangle headerRect = new Rectangle(left, top, lVerticalX + IconSize - left, headerHeight);

            // Grade column rectangle
            Rectangle gradeRect = new Rectangle(lVerticalX, top, lVerticalWidth, lVerticalHeight);

            using var path = new GraphicsPath();

            // Header top bar: round top-left and top-right corners
            path.AddArc(headerRect.Left, headerRect.Top, CornerRadius * 2, CornerRadius * 2, 180, 90);
            path.AddArc(headerRect.Right - CornerRadius * 2, headerRect.Top, CornerRadius * 2, CornerRadius * 2, 270, 90);
            path.AddLine(headerRect.Right, headerRect.Bottom, gradeRect.Right, gradeRect.Bottom - CornerRadius);

            // Grade column: round bottom-right corner only
            path.AddArc(gradeRect.Right - CornerRadius * 2, gradeRect.Bottom - CornerRadius * 2, CornerRadius * 2, CornerRadius * 2, 0, 90);
            path.AddLine(gradeRect.Right - CornerRadius, gradeRect.Bottom, gradeRect.Left, gradeRect.Bottom);
            path.AddLine(gradeRect.Left, gradeRect.Bottom, gradeRect.Left, headerRect.Bottom);
            path.AddLine(gradeRect.Left, headerRect.Bottom, headerRect.Left, headerRect.Bottom);
            path.CloseFigure();

            using var fill = new LinearGradientBrush(headerRect,
                Color.FromArgb(38, 38, 44),
                Color.FromArgb(38, 38, 44),
                LinearGradientMode.Vertical);

            using var border = new Pen(Color.FromArgb(85, 85, 100), 1.0f);

            g.FillPath(fill, path);
            g.DrawPath(border, path);
        }



        private void DrawTypeIcons(Graphics g, Rectangle previousRect)
        {
            int top = previousRect.Bottom + IconSpacing;
            int left = PaddingOuter;

            foreach (var (bounds, index) in _headerRects)
            {
                // Determine if a glow is needed and what color it should be.
                Color? glow = null;
                if (_model.View.TabId == (Enums.eType)index) // Prominent glow for selected
                {
                    glow = Color.FromArgb(220, 255, 225, 100);
                }
                else if (_hoverHeaderIndex == index) // Soft glow for hovered
                {
                    glow = Color.FromArgb(150, 255, 215, 0);
                }

                if (AssetManager.EnhTypes.TryGetValue(index, out var iconBitmap))
                {
                    if (iconBitmap?.Bitmap != null)
                    {
                        // Draw the entire individual icon. No clipping is needed.
                        g.DrawImage(iconBitmap.Bitmap, bounds);
                    }
                }
            }
        }

        private void DrawEnhancementGrid(Graphics g, int top, out Rectangle gridBounds)
        {
            int left = PaddingOuter;
            int width = EnhGridCols * IconSize + (EnhGridCols - 1) * IconSpacing;
            int height = EnhGridRows * IconSize + (EnhGridRows - 1) * IconSpacing;
            gridBounds = new Rectangle(left, top, width, height);
            _enhancementRects.Clear();

            if (_model.View.TabId == Enums.eType.SetO && _model.View.SetId == -1 && _model.View.SetTypeId > -1)
            {
                for (int i = 0; i < _model.SetIds.Length; i++)
                {
                    int col = i % EnhGridCols;
                    int row = i / EnhGridCols;
                    int x = gridBounds.Left + col * (IconSize + IconSpacing);
                    int y = gridBounds.Top + row * (IconSize + IconSpacing);
                    var rect = new Rectangle(x, y, IconSize, IconSize);

                    _enhancementRects.Add((rect, i)); // Use the grid's rects list for sets temporarily

                    // 1. Look up the specific "SetO" border from the Borders dictionary.
                    var borderKey = new Point(AssetManager.OriginIndex, (int)Origin.Grade.SetO);
                    if (AssetManager.Borders.TryGetValue(borderKey, out var borderImage) && borderImage?.Bitmap != null)
                    {
                        g.DrawImage(borderImage.Bitmap, rect);
                    }

                    // 2. Look up the specific Set icon from the Sets dictionary.
                    var setId = _model.SetIds[i];
                    if (AssetManager.Sets.TryGetValue(setId, out var setImage) && setImage?.Bitmap != null)
                    {
                        g.DrawImage(setImage.Bitmap, rect);
                    }
                }
                return; // Stop here to prevent drawing enhancements underneath
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
                    

                    int enhId = _model.EnhancementIds[index];
                    int iconIndex = GetEnhImageIndex(enhId);
                    if (iconIndex >= 0 && iconIndex < AssetManager.Enhancements.Count)
                    {
                        var grade = GetGradeForEnhancement(enhId);
                        var attr = GetImageAttributes(IsEnhancementGrayed(index));
                        AssetManager.DrawEnhancementAt(g, rect, iconIndex, grade, attr);
                    }
                }
            }
        }

        private void DrawGradeColumn(Graphics g, int columnLeft, int topEnh, int bottomEnh)
        {
            // Use the grid's boundaries directly for the icon area to ensure alignment.
            int top = topEnh;
            int bottom = bottomEnh;
            int availableHeight = bottom - top;

            // Set a clipping region to ensure icons don't draw outside their area during scroll.
            var clipRect = new Rectangle(columnLeft, top, IconSize + 1, availableHeight);
            Region oldClip = g.Clip;
            g.SetClip(clipRect);

            _gradeRects.Clear();

            var tabId = _model.View.TabId;
            int[]? indices = null;
            Dictionary<int, ExtendedBitmap>? sourceDictionary = null;

            // Determine which set of icons to display in the column.
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
                // When the "Set" tab is active, this column always shows the Set Types.
                indices = _model.SetTypes;
                sourceDictionary = AssetManager.SetTypes;
            }

            if (indices != null && sourceDictionary != null)
            {
                // For Normal/Special types, skip the first "None" entry.
                int loopStart = tabId is Enums.eType.Normal or Enums.eType.SpecialO ? 1 : 0;
                int itemCount = indices.Length - loopStart;

                // Always loop four times to draw four boxes for a consistent UI.
                for (int i = 0; i < MaxVisibleGradeIcons; i++)
                {
                    // The actual index into our data array, accounting for scrolling.
                    int dataIndex = loopStart + i + _scrollOffset / (IconSize + IconSpacing);
                    int y = top + i * (IconSize + IconSpacing);
                    var rect = new Rectangle(columnLeft, y, IconSize, IconSize);

                    // Check if a real item exists to be drawn in this slot.
                    if (i < itemCount)
                    {
                        // A real item exists: Draw it fully and make it clickable.
                        _gradeRects.Add((rect, dataIndex));

                        Color? glow = null;
                        if (_hoverSetIndex == dataIndex)
                        {
                            glow = Color.FromArgb(150, 255, 215, 0);
                        }

                        // Look up the specific icon from the correct dictionary
                        int iconKey = indices[dataIndex];
                        if (sourceDictionary.TryGetValue(iconKey, out var iconToDraw) && iconToDraw?.Bitmap != null)
                        {
                            // Draw the entire individual icon; no source rectangle needed.
                            g.DrawImage(iconToDraw.Bitmap, rect);
                        }
                    }
                    else
                    {
                        // No real item: Draw an empty placeholder box that is NOT clickable.
                        //DrawIconBox(g, rect);
                    }
                }
            }

            g.Clip = oldClip;

            // Determine if scroll arrows are needed.
            int finalItemCount = indices != null ? indices.Length - (tabId is Enums.eType.Normal or Enums.eType.SpecialO ? 1 : 0) : 0;
            bool scrollable = finalItemCount > MaxVisibleGradeIcons;

            if (scrollable)
            {
                // Draw arrows ABOVE and BELOW the aligned icon area.
                if (_scrollOffset > 0)
                {
                    int upArrowY = topEnh - ArrowHeight / 2 - IconSpacing;
                    DrawArrow(g, new Point(columnLeft + IconSize / 2, upArrowY), true);
                }

                int maxScrollOffset = (finalItemCount - MaxVisibleGradeIcons) * (IconSize + IconSpacing);
                if (_scrollOffset < maxScrollOffset)
                {
                    int downArrowY = bottomEnh + ArrowHeight / 2 + IconSpacing;
                    DrawArrow(g, new Point(columnLeft + IconSize / 2, downArrowY), false);
                }
            }
        }

        private void DrawFooterBox(Graphics g, out Rectangle leftTextBox)
        {
            int totalWidth = PaddingOuter + TypeIconCount * (IconSize + IconSpacing + 2);
            int y = Height - PaddingOuter - FooterBoxHeight + 10;
            int leftTextWidth = totalWidth - IconSize - PaddingOuter * 3;

            leftTextBox = new Rectangle(PaddingOuter, y, leftTextWidth, FooterBoxHeight - PaddingOuter);

            DrawRoundedBox(g, leftTextBox, CornerRadius,
                Color.FromArgb(38, 38, 44),
                Color.FromArgb(30, 30, 35),
                Color.FromArgb(85, 85, 100));

            using var font = new Font("Segoe UI", 8.5f);

            string footer = _hoverText ?? string.Empty;
            g.DrawString(footer, font, Brushes.White, leftTextBox,
                new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
        }

        private void DrawLevelBox(Graphics g, Rectangle leftTextBox)
        {
            int y = Height - PaddingOuter - FooterBoxHeight + 10;
            _levelBoxRect = new Rectangle(leftTextBox.Right + PaddingOuter, y, IconSize, FooterBoxHeight - PaddingOuter);

            DrawRoundedBox(g, _levelBoxRect, CornerRadius,
                Color.FromArgb(60, 100, 150),
                Color.FromArgb(40, 60, 90),
                Color.FromArgb(120, 170, 255));

            using var fontTitle = new Font("Segoe UI", 8.25f, FontStyle.Bold);
            using var fontValue = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            var titleRect = new Rectangle(_levelBoxRect.X, _levelBoxRect.Y, _levelBoxRect.Width, _levelBoxRect.Height / 3);
            var valueRect = new Rectangle(_levelBoxRect.X, _levelBoxRect.Y + titleRect.Height, _levelBoxRect.Width, titleRect.Height);

            g.DrawString("LVL", fontTitle, Brushes.White, titleRect, sf);


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

            g.DrawString(ioLevelText, fontValue, Brushes.White, valueRect, sf);

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
                g.DrawString("-", fontValue, Brushes.White, _lvlMinusRect, sf);
                g.DrawString("+", fontValue, Brushes.White, _lvlPlusRect, sf);
            }
            else
            {
                using var lockFont = new Font("Segoe MDL2 Assets", 10, FontStyle.Bold); // Modern icon font
                g.DrawString("\uE72E", lockFont, Brushes.Gray, _lvlMinusRect, sf); // Unicode for 'Lock'
                g.DrawString("\uE72E", lockFont, Brushes.Gray, _lvlPlusRect, sf);
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

        private static void DrawIconBox(Graphics g, Rectangle rect, bool darker = false, Color? glowColor = null)
        {
            // If a glow color is provided, draw the glow effect first.
            if (glowColor.HasValue)
            {
                using var glowPath = RoundedRect(rect, 6);
                using var glowPen = new Pen(Color.FromArgb(200, glowColor.Value), 4f);
                g.DrawPath(glowPen, glowPath);
            }

            // Draw the solid icon box on top of the glow.
            using var path = RoundedRect(rect, 6);
            using var bg = new SolidBrush(darker ? Color.FromArgb(50, 65, 85) : Color.FromArgb(60, 60, 70));
            using var pen = new Pen(Color.FromArgb(100, 130, 180), 1.1f);
            g.FillPath(bg, path);
            g.DrawPath(pen, path);
        }

        private static void DrawHeaderIcon(Graphics g, Dictionary<int, ExtendedBitmap> icons, Rectangle destination, int index)
        {
            if (icons.TryGetValue(index, out var bitmap) && bitmap.Bitmap is not null)
            {
                g.DrawImage(bitmap.Bitmap, destination);
            }
        }

        private static void DrawShieldPlaceholder(Graphics g, Rectangle rect)
        {
            using var path = RoundedRect(rect, 6);
            using var bg = new SolidBrush(Color.FromArgb(45, 45, 55));
            using var pen = new Pen(Color.FromArgb(110, 110, 130), 1.0f);
            g.FillPath(bg, path);
            g.DrawPath(pen, path);
        }

        private static void DrawRoundedBox(Graphics g, Rectangle rect, int radius, Color innerColor, Color outerColor, Color border)
        {
            using var path = RoundedRect(rect, radius);
            using var fill = new LinearGradientBrush(rect, outerColor, innerColor, LinearGradientMode.Vertical);
            using var pen = new Pen(border, 1.0f);
            g.FillPath(fill, path);
            g.DrawPath(pen, path);
        }

        private static void DrawArrow(Graphics g, Point center, bool up)
        {
            Point[] pts = up
                ? [new Point(center.X, center.Y - 4), new Point(center.X - 5, center.Y + 4), new Point(center.X + 5, center.Y + 4)
                ]
                : [new Point(center.X, center.Y + 4), new Point(center.X - 5, center.Y - 4), new Point(center.X + 5, center.Y - 4)
                ];
            using var brush = new SolidBrush(Color.White);
            g.FillPolygon(brush, pts);
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

            if (enhData.Unique)
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
                    _lastRelativeLevel = _model.View.RelLevel;
                    break;

                case Enums.eType.SpecialO:
                    _lastSpecial = _model.View.SpecialId;
                    _lastRelativeLevel = _model.View.RelLevel;
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

        private bool IsEnhancementGrayed(int index)
        {
            int enhId = _model.EnhancementIds.ElementAtOrDefault(index);
            if (enhId < 0) return false;

            var enh = DatabaseAPI.Database.Enhancements[enhId];

            if (!enh.Unique)
                return false;

            // Check if it's already slotted in any build slot
            bool isAlreadyUsed = MidsContext.Character.CurrentBuild.Powers
                .Where(p => p is not null)
                .SelectMany(p => p!.Slots)
                .Any(s => s.Enhancement.Enh == enhId);

            return isAlreadyUsed;
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
            if (enhId > -1 && (_model.HasCatalyst(enhId) || _model.IsNaturallyAttuned(enhId)))
                return Enums.eEnhRelative.Even; // lock to 0

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
            return iPowerIdx < 0 ? [] : DatabaseAPI.Database.Power[iPowerIdx].SetTypes.ToArray();
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
            return iPowerIdx < 0 ? [] : DatabaseAPI.Database.Power[iPowerIdx].GetValidEnhancements(iType, iSubType);
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

        private void SetActiveEnhancements(int iPower, int initialEnhId, int[] normalEnhs, int[] inventionEnhs)
        {
            int[] enhIdList;
            var tab = _model.View.TabId;
            var specialSubType = _model.View.SpecialId;

            _model.EnhancementNames.Clear();
            _model.EnhancementDescriptions.Clear();

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
                case Enums.eType.SetO when _model.View.SetId > -1:
                    var setId = _model.SetIds[_model.View.SetId];
                    enhIdList = DatabaseAPI.Database.EnhancementSets[setId].Enhancements;
                    break;
                default:
                    enhIdList = [];
                    break;
            }

            _model.EnhancementIds = enhIdList;

            if (enhIdList.Length > 0 && enhIdList[0] > -1)
            {
                // Repopulate the dictionaries only if we have enhancements to display.
                _model.EnhancementNames = enhIdList.ToDictionary(id => id, id => DatabaseAPI.Database.Enhancements[id].Name);
                _model.EnhancementDescriptions = enhIdList.ToDictionary(id => id, id => DatabaseAPI.Database.Enhancements[id].ShortName);
            }

            // Find the PickerId if the initial enhancement is in the current view
            if (initialEnhId > -1 && _model.Initial.TabId == _model.View.TabId)
            {
                _model.Initial.PickerId = Array.IndexOf(enhIdList, initialEnhId);
                _model.View.PickerId = _model.Initial.PickerId;
            }
        }

        #endregion

        #region Nested Types

        private sealed class EnhSelectorModel
        {
            public EnhSelectorState View { get; set; } = new();
            public EnhSelectorState Initial { get; set; } = new();

            public int[] EnhancementIds { get; set; } = [];
            public int[] SetIds { get; set; } = [];
            public int[] NoGrades { get; set; } = [];
            public int[] SpecialTypes { get; set; } = [];
            public int[] SetTypes { get; set; } = [];

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
            }
        }

        #endregion
    }
}

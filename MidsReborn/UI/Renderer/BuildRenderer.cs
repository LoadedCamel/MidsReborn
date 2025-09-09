using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Theming;
using Mids_Reborn.Core.Utils;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.UI.Theming;
using static Mids_Reborn.Core.Enums;

namespace Mids_Reborn.UI.Renderer
{
    public class BuildRenderer
    {
        public float MasterScale { get; set; } = 1.0f;
        private float _dpiScale = 1f;
        private float EffectiveScale => MasterScale * _dpiScale;

        private const int BasePaddingX = 15;
        private const int BasePaddingY = 32;
        private const int BaseOffsetX = 30;
        private const int BaseOffsetY = 23;
        private const int BaseOffsetInherent = 10;

        private readonly Size _baseSzPower = new(184, 30);
        private readonly Size _baseSzSlot = new Size(32, 32);

        private int _calculatedCellWidth;
        private int _calculatedIconXOffset;


        public Size SzPower => new Size(ScaleLogical(_baseSzPower.Width), ScaleLogical(_baseSzPower.Height));

        public Size SzSlot => new(ScaleLogical(_baseSzSlot.Width), ScaleLogical(_baseSzSlot.Height));

        private int PaddingY => ScaleLogical(BasePaddingY);
        public int OffsetY => ScaleLogical(BaseOffsetY);

        private int PaddingX => ScaleLogical(BasePaddingX);
        private int OffsetX => ScaleLogical(BaseOffsetX);
        private int OffsetInherent => ScaleLogical(BaseOffsetInherent);

        private float _currentFontSize = 11.25f;
        private Font _defaultFont;

        // Same size as target drawing area
        private Size _szBuffer;

        // Surface to draw on before combining to display
        public ExtendedBitmap? BxBuffer;

        // List of disabled, empty, filled, waiting
        public readonly List<ExtendedBitmap> BxPower;

        // The unplaced enhancement slot image
        public ExtendedBitmap? BxNewSlot;

        // Column variables
        private const int VcPowers = 24;
        private int _vcCols;
        private int _vcRowsPowers;
        private eColumnStacking _ColumnStackingMode = eColumnStacking.None;
        private Dictionary<int, Point> ColumnsPowersLayout;
        private int LayoutColumns = 0;
        private bool HasNullColumn = false;

        private bool HasHeaders => _ColumnStackingMode != eColumnStacking.None;

        // Recoloring variables
        private ColorMatrix? _pColorMatrix;
        public ImageAttributes? PImageAttributes;

        public static readonly float[][] HeroMatrix =
        [
            [
                1f, 0f, 0f, 0f, 0f
            ],
            [
                0f, 1f, 0f, 0f, 0f
            ],
            [
                0f, 0f, 1f, 0f, 0f
            ],
            [
                0f, 0f, 0f, 1f, 0f
            ],
            [
                0f, 0f, 0f, 0f, 1f
            ]
        ];
        private static readonly float[][] VillainMatrix =
        [
            [
                0.45f, 0, 0, 0, 0
            ],
            [
                0, 0.35f, 0, 0, 0
            ],
            [
                0.75f, 0, 0, 0.175f, 0
            ],
            [
                0, 0, 0, 1f, 0
            ],
            [
                0, 0, 0, 0, 1f
            ]
        ];
        private const int IcoOffset = 32;
        private Color _backColor;
        private Control _cTarget;
        public int Highlight;
        public eInterfaceMode InterfaceMode;

        private bool IsInDesignMode =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            AppDomain.CurrentDomain.FriendlyName.Contains("devenv");

        public BuildRenderer(Control targetControl)
        {
            InterfaceMode = eInterfaceMode.Normal;
            _vcCols = 6;
            _vcRowsPowers = 24;
            BxPower = [];
            _cTarget = targetControl;
            _backColor = targetControl.BackColor;
            _defaultFont = new Font(Fonts.Family("Noto Sans"), _currentFontSize);

            InitColumns = MidsContext.Config.Columns;
            ColorSwitch();
            Initialize(); // Load images + buffer setup
        }

        private PowerSlotTheme CurrentTheme
        {
            get
            {
                if (IsInDesignMode)
                {
                    return ThemeManager.DesignTime.PowerSlot;
                }
                return ThemeManager.CurrentTheme?.PowerSlot ?? ThemeManager.DesignTime.PowerSlot;
            }
        }

        public static bool EpicColumns => MidsContext.Character is { Archetype.ClassType: eClassType.HeroEpic };

        private int PoolColumns => _ColumnStackingMode switch
        {
            eColumnStacking.Vertical => 1,
            eColumnStacking.Horizontal => MidsContext.Character.CurrentBuild.Powers
                .Where(e => e is { Power: not null })
                .Select(e => e?.Power?.GetPowerSet()?.FullName)
                .Distinct()
                .Count(e => e != null && e.StartsWith("Pool.") | e.StartsWith("Epic.")),
            _ => 0
        };

        public int Columns
        {
            set => MiniSetCol(value);
        }

        public eColumnStacking ColumnStackingMode
        {
            set => _ColumnStackingMode = value;
            get => _ColumnStackingMode;
        }

        private int InitColumns
        {
            init
            {
                if (value == _vcCols)
                {
                    return;
                }

                if (value < 2 | value > 6)
                {
                    return;
                }

                _vcCols = value;
                _vcRowsPowers = VcPowers / _vcCols;
            }
        }

        private void Initialize()
        {
            // Get power button images directly from the AssetManager's pre-loaded cache
            BxPower.AddRange(AssetManager.Buttons);

            if (BxPower.Count == 0)
                throw new InvalidOperationException("No power button images found in AssetManager.");

            // Get the slot image directly from the AssetManager's cache
            BxNewSlot = AssetManager.NewSlot;
            if (BxNewSlot == null)
                throw new InvalidOperationException("Failed to load new slot image from AssetManager.");

            // Create drawing buffer using required size
            _szBuffer = new Size(_cTarget.Width, GetRequiredDrawingArea().Height);
            BxBuffer = new ExtendedBitmap(_szBuffer);

            ConfigureGraphics(BxBuffer.Graphics);
            InitDpi();
        }

        private static void ConfigureGraphics(Graphics? g)
        {
            if (g == null) return;

            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingMode = CompositingMode.SourceOver;
            g.PageUnit = GraphicsUnit.Pixel;
        }

        private int ScaleLogical(int value) => (int)(value * EffectiveScale);

        private void InitDpi()
        {
            using var g = _cTarget.CreateGraphics();
            _dpiScale = g.DpiX / 96f;
        }

        private int Scale(int value)
        {
            using var g = _cTarget.CreateGraphics();
            return (int)(value * g.DpiX / 96f); // 96 is baseline DPI
        }

        public void UpdateFontScale(float masterScale)
        {
            // 1. Define our application's desired base font size.
            const float appBaseSize = 11.25f;

            // 2. Define the standard system font size at 100% text scaling.
            const float standardSystemFontSize = 9.0f;

            // 3. Get the system's current UI font size to get the text scale factor.
            float currentSystemFontSize = SystemFonts.MessageBoxFont.SizeInPoints;
            float textScaleFactor = currentSystemFontSize / standardSystemFontSize;

            // 4. Calculate the final size using both the text scale and the window resize scale.
            _currentFontSize = appBaseSize * textScaleFactor * masterScale;
        }

        private int GetCellHeight(bool isInherent = false)
        {
            var height = SzPower.Height + OffsetY + SzSlot.Height;
            if (isInherent) height += OffsetInherent;
            return height + PaddingY;
        }

        public void ReInit(Control target)
        {
            if (target.IsDisposed)
                return;

            _cTarget = target;
            _backColor = target.BackColor;

            // Skip when minimized/zero width to avoid bad buffers
            if (target.ClientSize.Width <= 0)
                return;

            // Re-read DPI when the control/monitor context changes
            InitDpi();

            var required = GetRequiredDrawingArea();
            var newSize = new Size(target.ClientSize.Width, required.Height);

            // Only (re)allocate if size actually changed
            if (BxBuffer == null || BxBuffer.Size != newSize)
                BxBuffer = new ExtendedBitmap(newSize);

            ConfigureGraphics(BxBuffer.Graphics);

            // Update layout (e.g., rows/columns per stacking)
            if (_ColumnStackingMode != eColumnStacking.None)
            {
                GetPowersLayout();
            }

            FullRedraw();
        }

        private void DrawSplit()
        {
            if (BxBuffer?.Graphics == null || MidsContext.Character?.CurrentBuild == null)
                return;

            // --- Find the bottom boundary of the main power grid ---
            // This is now based on the grid's defined number of rows, not which powers are chosen.
            int lastRowIndex = _ColumnStackingMode == eColumnStacking.None && _vcCols == 5 ? 4 : _vcRowsPowers - 1;
            if (lastRowIndex < 0) lastRowIndex = 0;
            int mainGridBottom = CRtoXy(0, lastRowIndex).Y + SzPower.Height;

            // --- Find the top boundary of the inherent power grid ---
            int inherentGridTop = int.MaxValue;
            for (int i = VcPowers; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                var p = MidsContext.Character.CurrentBuild.Powers[i];
                if (p != null && p.Chosen)
                {
                    int powerTop = PowerPosition(p).Y;
                    if (powerTop < inherentGridTop)
                    {
                        inherentGridTop = powerTop;
                    }
                }
            }

            if (inherentGridTop == int.MaxValue)
            {
                int firstInherentRow = _ColumnStackingMode == eColumnStacking.None && _vcCols == 5 ? 5 : _vcRowsPowers;
                inherentGridTop = CRtoXy(0, firstInherentRow).Y;
            }

            // --- Calculate the midpoint and draw ---
            int y = mainGridBottom + (inherentGridTop - mainGridBottom) / 2 + ScaleLogical(30);

            using var pen = new Pen(Color.Goldenrod, 2f);
            using var font = new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            var brush = MidsContext.Character.IsHero() ? Brushes.DodgerBlue : Brushes.Red;

            string label = "Inherent Powers";
            SizeF textSize = BxBuffer.Graphics.MeasureString(label, font);
            float textY = y + 2f;
            float textX = (BxBuffer.Size.Width - textSize.Width) / 2f;

            BxBuffer.Graphics.DrawLine(pen, 2, y, BxBuffer.Size.Width, y);
            BxBuffer.Graphics.DrawString(label, font, brush, textX, textY);
        }

        private Dictionary<int, Point> LayoutToGridPos(List<List<int>> powersLayout)
        {
            var ret = new Dictionary<int, Point>();
            for (var i = 0; i < powersLayout.Count; i++)
            {
                for (var j = 0; j < powersLayout[i].Count; j++)
                {
                    ret.Add(powersLayout[i][j], new Point(i, j));
                }
            }

            return ret;
        }

        private void DrawPowers()
        {
            var powers = MidsContext.Character.CurrentBuild.Powers;

            for (int i = 0; i < powers.Count; i++)
            {
                var power = powers[i];
                if (power == null)
                    continue;

                bool isHighlighted = Highlight == i;

                // Define what needs to be drawn. A power should be drawn if it's
                // chosen, part of the Incarnate system, or currently highlighted.
                bool shouldDraw = isHighlighted || power.Chosen ||
                                  power.Power != null && (power.Power.GroupName == "Incarnate" || power.Power.IncludeFlag);

                if (!shouldDraw)
                    continue;

                // Create a reference to pass to the drawing method.
                var slotToDraw = power;

                // The 'isHighlighted' flag serves the role of the original 'singleDraw'.
                DrawPowerSlot(ref slotToDraw, isHighlighted);

                // A struct was passed by ref, so we ensure the main list is updated if needed.
                powers[i] = slotToDraw;
            }

            DrawSplit();
        }

        private float FontScale(float iSz)
        {
            return Math.Min(iSz * 1.1f, iSz);
        }

        private static int IndexFromLevel()
        {
            return MidsContext.Character.CurrentBuild.Powers.FindIndex(pow =>
                pow?.Level == MidsContext.Character.RequestedLevel);
        }

        private void DrawEnhancementLevel(SlotEntry slot, Font font, Graphics g, ref RectangleF rect)
        {
            var enh = slot.Enhancement;
            var enhType = DatabaseAPI.Database.Enhancements[enh.Enh].TypeID;

            var iValue2 = rect;
            iValue2.Y -= ScaleLogical(5);
            iValue2.Height = _defaultFont.GetHeight(BxBuffer.Graphics);

            string displayText = string.Empty;
            Color textColor = Color.White;

            if (enhType == eType.InventO || enhType == eType.SetO)
            {
                var relativeString = string.Empty;
                string enhInternalName = DatabaseAPI.Database.Enhancements[enh.Enh].UID;

                bool isCatalyst = DatabaseAPI.EnhHasCatalyst(enhInternalName) ||
                                  enhInternalName.ToLowerInvariant().Contains("overwhelming_force");

                // Catalyst enhancements always draw their base level and do not show a relative level
                if (!isCatalyst &&
                    enh.RelativeLevel > eEnhRelative.Even &&
                    MidsContext.Config.ShowEnhRel)
                {
                    relativeString = GetRelativeString(enh.RelativeLevel, false);
                }

                if (!string.IsNullOrEmpty(relativeString))
                {
                    iValue2.Width += ScaleLogical(10);
                    iValue2.X -= ScaleLogical(5);
                }

                if (!isCatalyst)
                {
                    // Show IO level + relative (or just IO level)
                    displayText = (MidsContext.Config.I9.HideIOLevels
                        ? string.Empty
                        : (enh.IOLevel + 1).ToString()) + relativeString;

                    textColor = Color.Cyan;
                }
                else
                {
                    return; // Do not draw anything for catalysts
                }
            }
            else if (enhType == eType.Normal)
            {
                // Always show the relative level if enabled
                displayText = GetRelativeString(enh.RelativeLevel, MidsContext.Config.ShowRelSymbols);

                // Add fixed level prefix if configured
                if (MidsContext.Config.ShowSoLevels)
                {
                    displayText = "50" + displayText;
                }

                // Widen area if needed
                if (MidsContext.Config.ShowSoLevels &&
                    enh.RelativeLevel != eEnhRelative.None &&
                    enh.RelativeLevel != eEnhRelative.Even &&
                    enh.RelativeLevel != eEnhRelative.PlusThree)
                {
                    iValue2.Width += ScaleLogical(10);
                    iValue2.X -= ScaleLogical(5);
                }

                // +3 SOs don't exist legitimately, just show hardcoded level
                if (enh.RelativeLevel == eEnhRelative.PlusThree)
                {
                    displayText = "53";
                }

                textColor = slot.Enhancement.RelativeLevel switch
                {
                    0 => Color.Red,
                    < eEnhRelative.Even => Color.Yellow,
                    > eEnhRelative.Even => Color.FromArgb(0, 255, 0),
                    _ => Color.White
                };
            }
            else if (enhType == eType.SpecialO)
            {
                // Special enhancements can either show combined or split level depending on config
                if (!MidsContext.Config.ShowSoLevels)
                {
                    // Combined display: IO + Rel + 1
                    int combined = enh.IOLevel + enh.RelativeLevel.ToInt() + 1;
                    displayText = combined.ToString();
                }
                else
                {
                    // Split display: base + relative (e.g., 50 +2)
                    string baseLevel = (enh.IOLevel + 1).ToString();
                    string rel = GetRelativeString(enh.RelativeLevel, MidsContext.Config.ShowRelSymbols);

                    displayText = !string.IsNullOrEmpty(rel) ? $"{baseLevel} {rel}" : baseLevel;
                }

                if (!string.IsNullOrEmpty(displayText) &&
                    enh.RelativeLevel != eEnhRelative.None &&
                    enh.RelativeLevel != eEnhRelative.Even)
                {
                    iValue2.Width += ScaleLogical(10);
                    iValue2.X -= ScaleLogical(5);
                }

                textColor = slot.Enhancement.RelativeLevel switch
                {
                    0 => Color.Red,
                    < eEnhRelative.Even => Color.Yellow,
                    > eEnhRelative.Even => Color.FromArgb(0, 255, 0),
                    _ => Color.White
                };
            }

            if (!string.IsNullOrEmpty(displayText))
            {
                var outlineColor = Color.Black;
                float outlineSpace = ScaleLogical(3);
                g = BxBuffer.Graphics;
                DrawOutlineText(displayText, iValue2, textColor, outlineColor, font, outlineSpace, g);
            }
        }

        public Point DrawPowerSlot(ref PowerEntry? powerEntry, bool singleDraw = false)
        {
            if (powerEntry == null || BxBuffer?.Graphics == null)
                return Point.Empty;

            // 1. Initialize all variables. This is fine.
            var drawVars = InitializeDrawVariables(powerEntry, singleDraw);

            // 2. Determine the power's state (Used, Open, etc.).
            UpdatePowerState(ref drawVars, singleDraw);

            // 3. Draw the main vector button. This is the MOST IMPORTANT step to do first.
            //    It calculates and stores the button's final, correct rectangle in 'drawVars.PowerRect'.
            UpdateImageAttributes(ref drawVars); // This calls DrawPowerImage internally

            // 4. NOW that we know the button's true rectangle, calculate the positions
            //    of the enhancement slots relative to it.
            UpdateSlotLocation(ref drawVars);

            // 5. Finally, draw the enhancement slots and the text, which will now be correctly aligned.
            DrawPowerComponents(drawVars);

            // The 'location' still refers to the top-left of the cell, which is fine to return.
            return drawVars.Location;
        }

        private void UpdateSlotLocation(ref DrawVariables drawVars)
        {
            Rectangle powerRect = drawVars.PowerRect;

            // Define a small, fixed indentation from the left edge of the button.
            const int indent = 8;

            // The X position is now simply the button's left edge plus our indent.
            int slotX = powerRect.X + ScaleLogical(indent);
            int slotY = powerRect.Y + OffsetY;

            drawVars.SlotLocation = new Point(slotX, slotY);
        }

        private void UpdatePowerState(ref DrawVariables drawVars, bool singleDraw)
        {
            if (InterfaceMode == eInterfaceMode.PowerToggle)
                return;

            var power = drawVars.PowerEntry.Power;

            if (power != null)
            {
                // This is the condition for showing the "new slot" hover.
                bool isValidForOpen = drawVars.SlotCheck > -1
                                      && drawVars.CanPlaceSlot
                                      && power.Slottable
                                      && drawVars.PowerEntry.PowerSet != null
                                      && drawVars.PowerEntry.Slots.Length < 6;

                // If it's highlighted ("singleDraw") and can have a slot,
                // change its appearance to the "Open" button style.
                if (singleDraw && isValidForOpen)
                {
                    drawVars.PowerState = ePowerState.Open;
                    // The early 'return' that was here previously was the bug.
                    // By removing it, the code can continue to DrawNewSlotHover.
                }
                else if (drawVars.PowerEntry.Chosen && !drawVars.CanPlaceSlot && Highlight == MidsContext.Character.CurrentBuild.Powers.IndexOf(drawVars.PowerEntry))
                {
                    drawVars.PowerState = ePowerState.Open;
                }
            }
            else
            {
                // If it's the next power to be picked by level, show it open.
                if (MidsContext.Character.CurrentBuild.Powers.IndexOf(drawVars.PowerEntry) == IndexFromLevel())
                {
                    drawVars.PowerState = ePowerState.Open;
                }
            }
        }

        private void UpdateImageAttributes(ref DrawVariables drawVars)
        {
            bool toggling = InterfaceMode == eInterfaceMode.PowerToggle;
            bool grey = drawVars.PowerEntry.Level >= MidsContext.Config.ForceLevel;
            ImageAttributes? imageAttr = null;

            // Toggle Mode
            if (toggling)
            {
                // Reset power state if it was Open
                if (drawVars.PowerState == ePowerState.Open)
                    drawVars.PowerState = ePowerState.Empty;

                if (drawVars.PowerEntry.StatInclude && drawVars.PowerState == ePowerState.Used)
                {
                    drawVars.PowerState = ePowerState.Open;
                    imageAttr = GreySlot(grey, true);
                }
                else if (drawVars.PowerEntry.CanIncludeForStats())
                {
                    imageAttr = GreySlot(grey);
                }
                else
                {
                    imageAttr = GreySlot(true); // Always grey
                }
            }
            else
            {
                imageAttr = GreySlot(grey);
            }

            // Draw slot background image
            drawVars.PowerRect = DrawPowerImage(
                drawVars.PowerEntry,
                drawVars.Location,
                drawVars.PowerState,
                toggling,
                imageAttr,
                grey
            );
        }

        private void DrawPowerComponents(DrawVariables drawVars)
        {
            // Draw stat/proc toggle dots
            DrawToggles(drawVars.PowerEntry, drawVars.PowerRect, drawVars.Pen2);

            // Draw enhancement slot icons (or empty slots)
            DrawSlotsAndEnhancements(drawVars.PowerEntry, drawVars.PowerRect, drawVars.Pen, drawVars.Font);

            // Draw new-slot hover effect (+1 slot)
            DrawNewSlotHover(drawVars.PowerEntry, drawVars.PowerRect, drawVars.SlotLocation, drawVars.Font, drawVars.PowerState, drawVars.SlotCheck, drawVars.DrawNewSlot);

            // Draw text label: (level) Name
            DrawPowerText(drawVars.PowerEntry, drawVars.PowerRect, drawVars.Font, drawVars.PowerState);
        }

        private DrawVariables InitializeDrawVariables(PowerEntry? powerEntry, bool singleDraw)
        {
            var isBold = MidsContext.Config.RtFont.PowersBold;
            
            var font = new Font(_defaultFont.FontFamily, _currentFontSize, isBold ? FontStyle.Bold : FontStyle.Regular,
                GraphicsUnit.Pixel);

            var drawVars = new DrawVariables
            {
                PowerEntry = powerEntry!,
                Pen = new Pen(Color.FromArgb(128, 0, 0, 0), 1f),
                Pen2 = new Pen(Color.Black),
                Text = string.Empty,
                Text2 = string.Empty,
                FontStyle = isBold ? FontStyle.Bold : FontStyle.Regular,
                Font = font,
                StringFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
                {
                    Trimming = StringTrimming.None,
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                },
                RectangleF = new RectangleF(0, 0, 0, 0),
                SlotCheck = MidsContext.Character.SlotCheck(powerEntry),
                PowerState = powerEntry!.State,
                CanPlaceSlot = MidsContext.Character.CanPlaceSlot,
                DrawNewSlot = powerEntry.Power is not null
                              && powerEntry.State != ePowerState.Empty
                              && MidsContext.Character.CanPlaceSlot
                              && powerEntry.Slots.Length < 6
                              && singleDraw
                              && powerEntry.Power.Slottable
                              && InterfaceMode != eInterfaceMode.PowerToggle,
                SingleDraw = singleDraw
            };

            // Positioning
            var baseLocation = PowerPosition(powerEntry);
            if (_ColumnStackingMode != eColumnStacking.None)
                baseLocation = baseLocation with { Y = baseLocation.Y + 18 };

            drawVars.Location = baseLocation;
            drawVars.SlotLocation = default;

            return drawVars;
        }

        private Rectangle DrawPowerImage(PowerEntry? iSlot, Point location, ePowerState ePowerState, bool toggling, ImageAttributes? imageAttr, bool grey)
        {
            // The 'location' is the top-left of the entire cell.
            const int horizontalPadding = 10; // 5px on each side

            // Calculate the dynamic width and centered position for the button.
            int dynamicWidth = _calculatedCellWidth - ScaleLogical(25 + horizontalPadding);
            int buttonX = location.X + (_calculatedCellWidth - dynamicWidth) / 2;
            var powerRect = new Rectangle(buttonX, location.Y, dynamicWidth, SzPower.Height);

            // --- CORRECTED LOGIC FOR TOGGLE MODE ---
            var effectiveState = ePowerState; // Start with the power's actual state.

            if (toggling && iSlot != null)
            {
                // In toggle mode, if a power is included in stats, draw it
                // with the "Open" appearance (thick border) for visual feedback.
                if (iSlot.StatInclude && iSlot.State == ePowerState.Used)
                {
                    effectiveState = ePowerState.Open;
                }
                // If it was going to be drawn as "Open" for another reason (like being the
                // next available slot), but we're in toggle mode, just draw it as "Empty".
                else if (effectiveState == ePowerState.Open)
                {
                    effectiveState = ePowerState.Empty;
                }
            }

            // Select the theme based on alignment.
            var theme = CurrentTheme;

            // Call the vector drawing method with the final, effective state.
            DrawVectorPowerSlot(BxBuffer.Graphics, powerRect, effectiveState, theme);

            return powerRect;
        }

        private void DrawToggles(PowerEntry powerEntry, Rectangle powerRect, Pen pen)
        {
            // Define size and spacing for the dots
            int toggleSize = ScaleLogical(15);
            int padding = ScaleLogical(8); // Space from the edge and between dots

            // Calculate the centered Y position, which is the same for all dots
            int y = powerRect.Top + (powerRect.Height - toggleSize) / 2;

            bool canShowStatToggle = powerEntry.CanIncludeForStats();
            bool canShowProcToggle = powerEntry.HasProc();

            // Case 1: Both toggles are visible
            if (canShowStatToggle && canShowProcToggle)
            {
                // Draw the right-most dot (Stat/Power Toggle)
                int statToggleX = powerRect.Right - toggleSize - padding;
                var statToggleRect = new Rectangle(statToggleX, y, toggleSize, toggleSize);
                var statCenter = new PointF(-0.25f, -0.33f);
                var statBrush = powerEntry.StatInclude
                    ? MakePathBrush(statToggleRect, statCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0))
                    : MakePathBrush(statToggleRect, statCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
                BxBuffer.Graphics.FillEllipse(statBrush, statToggleRect);
                BxBuffer.Graphics.DrawEllipse(pen, statToggleRect);

                // Draw the dot to its left (Proc Toggle)
                int procToggleX = statToggleRect.Left - toggleSize - padding;
                var procToggleRect = new Rectangle(procToggleX, y, toggleSize, toggleSize);
                var pCenter = new PointF(-0.25f, -0.33f);
                var procBrush = !powerEntry.ProcInclude
                    ? MakePathBrush(procToggleRect, pCenter, Color.FromArgb(251, 255, 97), Color.FromArgb(91, 91, 0))
                    : MakePathBrush(procToggleRect, pCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
                BxBuffer.Graphics.FillEllipse(procBrush, procToggleRect);
                BxBuffer.Graphics.DrawEllipse(pen, procToggleRect);
            }
            // Case 2: Only the Stat toggle is visible
            else if (canShowStatToggle)
            {
                int statToggleX = powerRect.Right - toggleSize - padding;
                var statToggleRect = new Rectangle(statToggleX, y, toggleSize, toggleSize);
                var statCenter = new PointF(-0.25f, -0.33f);
                var statBrush = powerEntry.StatInclude
                    ? MakePathBrush(statToggleRect, statCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0))
                    : MakePathBrush(statToggleRect, statCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
                BxBuffer.Graphics.FillEllipse(statBrush, statToggleRect);
                BxBuffer.Graphics.DrawEllipse(pen, statToggleRect);
            }
            // Case 3: Only the Proc toggle is visible
            else if (canShowProcToggle)
            {
                int procToggleX = powerRect.Right - toggleSize - padding;
                var procToggleRect = new Rectangle(procToggleX, y, toggleSize, toggleSize);
                var pCenter = new PointF(-0.25f, -0.33f);
                var procBrush = !powerEntry.ProcInclude
                    ? MakePathBrush(procToggleRect, pCenter, Color.FromArgb(251, 255, 97), Color.FromArgb(91, 91, 0))
                    : MakePathBrush(procToggleRect, pCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
                BxBuffer.Graphics.FillEllipse(procBrush, procToggleRect);
                BxBuffer.Graphics.DrawEllipse(pen, procToggleRect);
            }
        }

        /*private int ComputeSlotEdge(Rectangle powerRect, int slotCount)
        {
            int spacing = ScaleLogical(2);
            int indent = ScaleLogical(5); // keep consistent with draw/hit-test
            int rightPad = ScaleLogical(10);

            int available = powerRect.Width - indent - rightPad;
            if (slotCount <= 0 || available <= 0) return SzSlot.Width;

            // max size per slot so they all fit with gaps
            int maxEdgeToFit = (available - (slotCount - 1) * spacing) / slotCount;

            // clamp between a readable minimum and the logical base size
            int logicalBase = ScaleLogical(_baseSzSlot.Width);
            int minReadable = ScaleLogical(26); // tweak if you want even smaller
            return Math.Clamp(maxEdgeToFit, minReadable, logicalBase);
        }*/

        private int ComputeSlotEdge(Rectangle powerRect)
        {
            // Always calculate the size based on the maximum possible number of slots (6).
            const int maxSlots = 6;

            // Define constants for padding and spacing.
            int indent = ScaleLogical(5);
            int rightPad = ScaleLogical(10);
            int spacing = ScaleLogical(2);

            // Calculate the total horizontal space available for slots within the power button.
            int availableWidth = powerRect.Width - indent - rightPad;

            // If there's no space, return a default fallback size.
            if (availableWidth <= 0)
            {
                return ScaleLogical(26); // Return the minimum readable size
            }

            // Calculate the size for each slot as if there were always six.
            int edgeForSixSlots = (availableWidth - (maxSlots - 1) * spacing) / maxSlots;

            // Define the absolute min/max bounds for the slot size.
            int minReadable = ScaleLogical(26);
            int logicalBase = ScaleLogical(_baseSzSlot.Width); // Don't allow slots to be bigger than their base asset size.

            // Clamp the result to ensure it stays within the desired min/max range.
            return Math.Clamp(edgeForSixSlots, minReadable, logicalBase);
        }

        private void DrawSlotsAndEnhancements(PowerEntry powerEntry, Rectangle powerRect, Pen pen, Font font)
        {
            if (powerEntry.Slots.Length == 0) return;

            // 1. Calculate the starting X coordinate to left-align the slots
            //    with a small indent relative to the power button.
            const int indent = 5;
            int startX = powerRect.X + ScaleLogical(indent);
            int startY = powerRect.Y + OffsetY;

            // 2. Loop through and draw each slot at the correct, calculated position.
            for (var i = 0; i < powerEntry.Slots.Length; i++)
            {
                var slot = powerEntry.Slots[i];

                int edge = ComputeSlotEdge(powerRect);
                var spacing = ScaleLogical(2);
                // Create a new, clean rectangle for each slot.
                var slotRect = new RectangleF(
                    startX + i * (edge + spacing), // Add 2px spacing between slots
                    startY,
                    edge,
                    edge);

                
                SolidBrush solidBrush;
                if (slot.Enhancement.Enh < 0)
                {
                    // Use the .Bitmap property of the ExtendedBitmap from AssetManager
                    var sourceImage = AssetManager.EmptySlot.Bitmap;
                    if (sourceImage is null) continue; // Safety check

                    var destRect = new Rectangle(
                        (int)slotRect.X,
                        (int)slotRect.Y,
                        (int)slotRect.Width,
                        (int)slotRect.Height);

                    var srcRect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);

                    BxBuffer.Graphics.DrawImage(sourceImage, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, PImageAttributes);

                    if (MidsContext.Config.CalcEnhLevel == 0 | slot.Level > MidsContext.Config.ForceLevel |
                        InterfaceMode == eInterfaceMode.PowerToggle & !powerEntry.StatInclude |
                        !powerEntry.AllowFrontLoading & slot.Level < powerEntry.Level)
                    {
                        solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                        BxBuffer.Graphics.FillEllipse(solidBrush, slotRect);
                        BxBuffer.Graphics.DrawEllipse(pen, slotRect);
                    }
                }
                else
                {
                    // Controls if powers or slots are greyed out
                    if (IsInDesignMode) continue;

                    var enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                    var clipRect3 = new Rectangle((int)slotRect.X, (int)slotRect.Y, (int)slotRect.Width, (int)slotRect.Height);
                    AssetManager.DrawEnhancementAt(BxBuffer.Graphics, clipRect3, enhancement.ImageIdx, AssetManager.ToGfxGrade(enhancement.TypeID, slot.Enhancement.Grade));

                    if (slot.Enhancement.RelativeLevel == 0 | slot.Level > MidsContext.Config.ForceLevel |
                        InterfaceMode == eInterfaceMode.PowerToggle & !powerEntry.StatInclude |
                        !powerEntry.AllowFrontLoading & slot.Level < powerEntry.Level |
                        MidsContext.EnhCheckMode & !slot.Enhancement.Obtained)
                    {
                        solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                        var iValue3 = slotRect;
                        iValue3.Inflate(1f, 1f);
                        BxBuffer.Graphics.FillEllipse(solidBrush, iValue3);
                    }

                    if (slot.Enhancement.Enh > -1)
                        DrawEnhancementLevel(slot, font, BxBuffer.Graphics, ref slotRect);
                }

                if (!MidsContext.Config.ShowSlotLevels) continue;

                var powerTextRect = new RectangleF(
                    slotRect.X,                                  // Align horizontally with the slot icon.
                    slotRect.Bottom + 2,                         // Position it 2 pixels below the icon.
                    slotRect.Width,                              // Use the icon's width for centering.
                    _defaultFont.GetHeight(BxBuffer.Graphics)
                );

                DrawOutlineText(
                    Convert.ToString(slot.Level + 1),
                    powerTextRect,
                    Color.FromArgb(0, 255, 0),
                    Color.FromArgb(192, 0, 0, 0),
                    font,
                    2f,
                    BxBuffer.Graphics);
            }
        }

        private void DrawNewSlotHover(PowerEntry powerEntry, Rectangle powerRect, Point slotLocation, Font font, ePowerState powerState, int slotCheck, bool drawNewSlot)
        {
            if (slotCheck > -1 && powerState is not ePowerState.Empty && drawNewSlot)
            {
                int spacing = ScaleLogical(2);
                int edge = ComputeSlotEdge(powerRect);

                var slotHoverRect = new RectangleF(
                    slotLocation.X + (edge + spacing) * powerEntry.Slots.Length,
                    slotLocation.Y,
                    edge,
                    edge);

                var sourceImage = AssetManager.NewSlot.Bitmap;
                if (sourceImage is null) return;

                var srcRect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);

                BxBuffer.Graphics.DrawImage(sourceImage, slotHoverRect, srcRect, GraphicsUnit.Pixel);

                // center the number vertically within the (scaled) edge
                var textRect = slotHoverRect;
                textRect.Height = _defaultFont.GetHeight(BxBuffer.Graphics);
                textRect.Y += (edge - textRect.Height) / 2f;

                DrawOutlineText(Convert.ToString(slotCheck + 1), textRect,
                    Color.FromArgb(0, 255, 255), Color.FromArgb(192, 0, 0, 0),
                    font, 2f, BxBuffer.Graphics);
            }
        }

        private void DrawPowerText(PowerEntry powerEntry, Rectangle powerRect, Font font, ePowerState powerState)
        {
            // Determine the text content and color based on the power's state
            string text;
            SolidBrush textBrush;

            ePowerState displayState = powerEntry.State == ePowerState.Empty && powerState == ePowerState.Open ? powerState : powerEntry.State;

            switch (displayState)
            {
                case ePowerState.Empty:
                case ePowerState.Open:
                    textBrush = new SolidBrush(Color.WhiteSmoke);
                    text = $"({powerEntry.Level + 1})";
                    break;

                case ePowerState.Used:
                default:
                    textBrush = !MidsContext.Character.IsHero() ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                    text = $"({powerEntry.Level + 1}) {powerEntry.Name}";
                    break;
            }

            // Adjust for toggle mode visuals
            if (InterfaceMode == eInterfaceMode.PowerToggle && textBrush.Color == Color.Black && !powerEntry.CanIncludeForStats())
            {
                textBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            }

            // Define the rectangle for the text, indented inside the button
            var textRect = new RectangleF
            {
                X = powerRect.X + ScaleLogical(10),
                Y = powerRect.Y + ScaleLogical(4),
                Width = powerRect.Width - ScaleLogical(20), // Use the button's dynamic width
                Height = powerRect.Height - ScaleLogical(8)
            };

            // Draw the text
            using var stringFormat = new StringFormat { FormatFlags = StringFormatFlags.NoWrap, LineAlignment = StringAlignment.Center };

            if (MidsContext.Config.EnhanceVisibility)
            {
                DrawOutlineText(text, textRect, Color.WhiteSmoke, Color.Black, font, 3f, BxBuffer.Graphics, false, true);
            }
            else
            {
                BxBuffer.Graphics.DrawString(text, font, textBrush, textRect, stringFormat);
            }

            textBrush.Dispose();
        }

        private void DrawVectorPowerSlot(Graphics g, Rectangle bounds, ePowerState state, PowerSlotTheme theme)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;

            using var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, bounds.Height, bounds.Height, 90, 180);
            path.AddArc(bounds.Right - bounds.Height, bounds.Y, bounds.Height, bounds.Height, 270, 180);
            path.CloseFigure();

            switch (state)
            {
                case ePowerState.Disabled: // pSlot0.png
                    g.FillPath(new SolidBrush(theme.DisabledFill), path);
                    using (var pen = new Pen(Color.FromArgb(60, 60, 60)))
                        g.DrawPath(pen, path);
                    break;

                case ePowerState.Empty: // pSlot1.png
                    g.FillPath(new SolidBrush(theme.EmptyFill), path);
                    break;

                case ePowerState.Open: // pSlot3.png and pSlot5.png
                    g.FillPath(new SolidBrush(theme.OpenBorder), path);
                    Rectangle innerBounds = bounds;
                    innerBounds.Inflate(-3, -3); // Create the thick border effect
                    using (var innerPath = new GraphicsPath())
                    {
                        innerPath.AddArc(innerBounds.X, innerBounds.Y, innerBounds.Height, innerBounds.Height, 90, 180);
                        innerPath.AddArc(innerBounds.Right - innerBounds.Height, innerBounds.Y, innerBounds.Height, innerBounds.Height, 270, 180);
                        innerPath.CloseFigure();
                        using var fillBrush = new LinearGradientBrush(bounds, theme.GradientTop, theme.GradientBottom, 90f);
                        g.FillPath(fillBrush, innerPath);
                    }
                    break;

                case ePowerState.Used: // pSlot2.png and pSlot4.png
                default:
                    using (var fillBrush = new LinearGradientBrush(bounds, theme.GradientTop, theme.GradientBottom, 90f))
                    {
                        g.FillPath(fillBrush, path);
                    }
                    using (var borderPen = new Pen(theme.Border, 1.5f))
                    {
                        g.DrawPath(borderPen, path);
                    }
                    break;
            }
        }

        public void GetPowersLayout()
        {
            var powers = MidsContext.Character.CurrentBuild.Powers;
            var powersLayout = new List<List<int>>();
            var nullColumn = new List<int>();

            // Default fallback to 3 columns
            if (MidsContext.Config.Columns < 2)
                MidsContext.Config.Columns = 3;

            if (_ColumnStackingMode == eColumnStacking.Horizontal)
            {
                var pools = new Dictionary<string, int>();

                powersLayout.Add([]); // Primary
                powersLayout.Add([]); // Secondary
                var epicColumn = new List<int>();

                for (int i = 0; i < powers.Count; i++)
                {
                    var p = powers[i];

                    if (p?.Power == null)
                    {
                        nullColumn.Add(i);
                        continue;
                    }

                    switch (p.Power.GetPowerSet()?.SetType)
                    {
                        case ePowerSetType.Primary:
                            powersLayout[0].Add(i);
                            break;

                        case ePowerSetType.Secondary:
                            powersLayout[1].Add(i);
                            break;

                        case ePowerSetType.Pool:
                            string? setName = p.Power.GetPowerSet()?.FullName;
                            if (!string.IsNullOrEmpty(setName))
                            {
                                if (!pools.TryGetValue(setName, out int col))
                                {
                                    col = powersLayout.Count;
                                    powersLayout.Add([]);
                                    pools[setName] = col;
                                }
                                powersLayout[col].Add(i);
                            }
                            break;

                        case ePowerSetType.Ancillary:
                            epicColumn.Add(i);
                            break;
                    }
                }

                if (epicColumn.Count > 0)
                    powersLayout.Add(epicColumn);
            }
            else if (_ColumnStackingMode == eColumnStacking.Vertical)
            {
                // Primary, Secondary, Pools+Epics
                powersLayout.Add([]);
                powersLayout.Add([]);
                powersLayout.Add([]);

                for (int i = 0; i < powers.Count; i++)
                {
                    var p = powers[i];

                    if (p?.Power == null)
                    {
                        nullColumn.Add(i);
                        continue;
                    }

                    var type = p.Power.GetPowerSet()?.SetType;
                    int col = type switch
                    {
                        ePowerSetType.Primary => 0,
                        ePowerSetType.Secondary => 1,
                        ePowerSetType.Pool or ePowerSetType.Ancillary => 2,
                        _ => -1
                    };

                    if (col >= 0)
                        powersLayout[col].Add(i);
                }
            }

            // Add null column last if it exists
            if (nullColumn.Count > 0)
                powersLayout.Add(nullColumn);

            // Final layout state
            ColumnsPowersLayout = LayoutToGridPos(powersLayout);
            Columns = LayoutColumns;
            HasNullColumn = nullColumn.Count > 0;

            if (_ColumnStackingMode == eColumnStacking.None)
            {
                _vcCols = MidsContext.Config.Columns;
                _vcRowsPowers = VcPowers / _vcCols;
            }
            else
            {
                _vcCols = LayoutColumns;
                _vcRowsPowers = powersLayout.Max(col => col.Count);
            }
        }

        private PathGradientBrush MakePathBrush(Rectangle iRect, PointF iCenter, Color iColor1, Color icolor2)
        {
            var num = (float)(iRect.Left + iRect.Width * 0.5);
            var num2 = (float)(iRect.Top + iRect.Height * 0.5);
            var graphicsPath = new GraphicsPath();
            graphicsPath.AddEllipse(iRect);
            PathGradientBrush pathGradientBrush;
            PathGradientBrush pathGradientBrush2;
            checked
            {
                var array = new Color[graphicsPath.PathPoints.GetUpperBound(0) + 1];
                var lowerBound = graphicsPath.PathPoints.GetLowerBound(0);
                var upperBound = graphicsPath.PathPoints.GetUpperBound(0);
                for (var i = lowerBound; i <= upperBound; i++)
                {
                    array[i] = icolor2;
                }

                pathGradientBrush = new PathGradientBrush(graphicsPath)
                {
                    CenterColor = iColor1,
                    SurroundColors = array
                };
                pathGradientBrush2 = pathGradientBrush;
            }

            var centerPoint = new PointF((float)(num + (iCenter.X + iCenter.X * (iRect.Width * 0.5))), (float)(num2 + (iCenter.Y + iCenter.Y * (iRect.Height * 0.5))));
            pathGradientBrush2.CenterPoint = centerPoint;
            
            return pathGradientBrush;
        }

        public void FullRedraw()
        {
            if (_cTarget == null || _cTarget.IsDisposed)
                return;

            // Apply layout config
            if (_ColumnStackingMode != MidsContext.Config.ColumnStackingMode)
                _ColumnStackingMode = MidsContext.Config.ColumnStackingMode;

            ColorSwitch();
            _backColor = _cTarget.BackColor;

            // Validate or resize buffer
            var desiredSize = GetRequiredDrawingArea();
            if (BxBuffer == null || BxBuffer.Size != desiredSize)
            {
                BxBuffer = new ExtendedBitmap(desiredSize);
            }

            // Clear + set rendering quality
            if (BxBuffer.Graphics != null)
            {
                ConfigureGraphics(BxBuffer.Graphics);
                BxBuffer.Graphics.Clear(_backColor);
            }

            // Prep header variables
            InitHeadersVariables();

            // Calculate stacked layout if needed
            if (_ColumnStackingMode != eColumnStacking.None)
                GetPowersLayout();

            // Draw power icons and enhancement overlays
            DrawPowers();

            // Draw headers if layout is stacked
            if (_ColumnStackingMode != eColumnStacking.None)
                DrawHeaders();
        }

        public int GetMinimumRequiredWidth()
        {
            // Define the smallest acceptable gap between power icons.
            const int minimumPadding = 50;

            // Calculate the total minimum width needed.
            // This is the width of all power icons plus the minimum gap between each one.
            int minimumWidth = _vcCols * SzPower.Width + (_vcCols - 1) * ScaleLogical(minimumPadding);

            return minimumWidth;
        }

        private void InitHeadersVariables()
        {
            if (_ColumnStackingMode != eColumnStacking.Horizontal)
            {
                HasNullColumn = false;
                LayoutColumns = 0;

                return;
            }

            var ps = GetDistinctPowersets();
            HasNullColumn = (ps.Count != 0 && ps[^1] == null && ps.Any(e => e != null)) | (ps.Count == 1 && ps[0] == null);
            LayoutColumns = ps.Count;
        }

        private List<IPowerset?> GetDistinctPowersets()
        {
            return ColumnsPowersLayout
                .DistinctBy(e => e.Value.X)
                .Select(e => MidsContext.Character?.CurrentBuild?.Powers[e.Key]?.Power?.GetPowerSet())
                .ToList();
        }

        private void DrawHeaders()
        {
            if (_ColumnStackingMode == eColumnStacking.None)
            {
                return;
            }

            using var textFont = new Font(Fonts.Family("Noto Sans"), 9f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            var powerSets = GetDistinctPowersets();

            int y = ScaleLogical(2);
            int iconSize = ScaleLogical(16);
            var k = 1;

            switch (_ColumnStackingMode)
            {
                case eColumnStacking.Horizontal:
                    for (var i = 0; i < LayoutColumns; i++)
                    {
                        var hLabel = i switch
                        {
                            _ when i >= powerSets.Count => HasNullColumn ? "Unaffected Powers" : "",
                            _ when powerSets[i] is null => HasNullColumn ? "Unaffected Powers" : "",
                            0 => $"Pri.: {powerSets[i].DisplayName}",
                            1 => $"Sec.: {powerSets[i].DisplayName}",
                            >= 2 and <= 7 => i >= powerSets.Count
                                ? HasNullColumn
                                    ? "Unaffected Powers"
                                    : ""
                                : $"{(powerSets[i].SetType == ePowerSetType.Pool ? $"Pool {k++}" : MidsContext.Character?.IsHero() == false ? "Ancillary" : "Epic")}: {powerSets[i].DisplayName}",
                            _ when HasNullColumn => "Unaffected Powers",
                            _ => ""
                        };

                        Bitmap? psImg = null;
                        if (hLabel.Contains("Unaffected Powers"))
                        {
                            psImg = new Bitmap(AssetManager.UnknownIcon.Bitmap);
                        }
                        else if (i < powerSets.Count && powerSets[i] != null)
                        {
                            // Use the new helper method to get the image
                            var extendedBitmap = AssetManager.GetPowersetImage(powerSets[i]);
                            psImg = extendedBitmap.Bitmap;
                        }

                        // Unaffected powers will be drawn at 3rd column or farther no matter what.
                        var powerPos = CRtoXy(hLabel.Contains("Unaffected Powers") ? Math.Max(2, i) : i, 0);
                        var iconOffset = psImg == null ? 0 : 2 + iconSize;
                        var x = powerPos.X + 4;

                        if (psImg != null)
                        {
                            BxBuffer?.Graphics?.DrawImage(psImg, new Point(x, y));
                        }

                        TextRenderer.DrawText(BxBuffer.Graphics, hLabel, textFont, new Point(x + iconOffset, y), Color.WhiteSmoke);
                    }

                    break;

                case eColumnStacking.Vertical:
                    var texts = new[] {
                        "Primary",
                        "Secondary",
                        MidsContext.Character?.IsHero() == false ? "Pools/Ancillary" : "Pools/Epic"
                    };

                    for (var i = 0; i < 3; i++)
                    {
                        var powerPos = CRtoXy(i, 0);
                        var x = powerPos.X + 4;
                        TextRenderer.DrawText(BxBuffer.Graphics, texts[i], textFont, new Point(x, y), Color.WhiteSmoke);
                    }

                    break;
            }
        }

        private int GetVisualIdx(int powerIndex)
        {
            var nidPowerset = MidsContext.Character.CurrentBuild.Powers[powerIndex] != null
                ? MidsContext.Character.CurrentBuild.Powers[powerIndex].NIDPowerset
                : -1;
            var idxPower = MidsContext.Character.CurrentBuild.Powers[powerIndex] != null
                ? MidsContext.Character.CurrentBuild.Powers[powerIndex].IDXPower
                : -1;

            var isInherent = powerIndex > 23;

            if (nidPowerset > -1)
            {
                if (DatabaseAPI.Database.Powersets[nidPowerset].SetType == ePowerSetType.Inherent & isInherent)
                {
                    return DatabaseAPI.Database.Powersets[nidPowerset].Powers[idxPower].LocationIndex;
                }
                
                var vIdx = -1;
                for (var i = 0; i <= powerIndex; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i]?.NIDPowerset > -1)
                    {
                        if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType != ePowerSetType.Inherent | !isInherent)
                        {
                            vIdx++;
                        }
                    }
                    else
                    {
                        vIdx++;
                    }
                }

                return vIdx;
            }
            else
            {
                var vIdx = -1;
                for (var i = 0; i <= powerIndex; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i]?.NIDPowerset > -1)
                    {
                        if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType != ePowerSetType.Inherent)
                        {
                            vIdx++;
                        }
                    }
                    else
                    {
                        vIdx++;
                    }
                }

                return vIdx;
            }
        }

        public static void DrawOutlineText(string text, RectangleF bounds, Color fillColor, Color outlineColor, Font font, float outlineThickness, Graphics g, bool smallMode = false, bool leftAlign = false)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using var format = new StringFormat(StringFormatFlags.NoWrap);
            format.LineAlignment = StringAlignment.Near;
            format.Alignment = leftAlign ? StringAlignment.Near : StringAlignment.Center;

            float emSize = font.SizeInPoints * g.DpiY / 72f;

            using var path = new GraphicsPath();
            path.AddString(text, font.FontFamily, (int)font.Style, emSize, bounds, format);

            using var outlinePen = new Pen(outlineColor, outlineThickness);
            outlinePen.LineJoin = LineJoin.Round;
            g.DrawPath(outlinePen, path);

            using var brush = new SolidBrush(fillColor);
            g.FillPath(brush, path);
        }

        public eToggleType WhichToggle(int powerIndex, int clickX, int clickY)
        {
            if (powerIndex < 0) return eToggleType.None;

            var powerEntry = MidsContext.Character.CurrentBuild.Powers[powerIndex];
            if (powerEntry?.IDXPower < 0) return eToggleType.None;

            // Get the exact, dynamic rectangle of the power button
            Point cellLocation = PowerPosition(powerEntry);
            const int horizontalPadding = 10;
            int dynamicWidth = _calculatedCellWidth - ScaleLogical(25 + horizontalPadding);
            int buttonX = cellLocation.X + (_calculatedCellWidth - dynamicWidth) / 2;
            var powerRect = new Rectangle(buttonX, cellLocation.Y, dynamicWidth, SzPower.Height);

            // Replicate the exact positioning logic from DrawToggles
            int toggleSize = ScaleLogical(15);
            int padding = ScaleLogical(4);
            int y = powerRect.Top + (powerRect.Height - toggleSize) / 2;

            bool canShowStatToggle = powerEntry.CanIncludeForStats();
            bool canShowProcToggle = powerEntry.HasProc();

            // Check for Stat toggle first (it's the right-most)
            if (canShowStatToggle)
            {
                int statToggleX = powerRect.Right - toggleSize - padding;
                var statToggleRect = new Rectangle(statToggleX, y, toggleSize, toggleSize);
                if (statToggleRect.Contains(clickX, clickY))
                {
                    return eToggleType.Stat;
                }
            }

            // Check for Proc toggle
            if (canShowProcToggle)
            {
                Rectangle procToggleRect;
                if (canShowStatToggle)
                {
                    // It's the second button from the right
                    int statToggleX = powerRect.Right - toggleSize - padding;
                    int procToggleX = statToggleX - toggleSize - padding;
                    procToggleRect = new Rectangle(procToggleX, y, toggleSize, toggleSize);
                }
                else
                {
                    // It's the only button, on the far right
                    int procToggleX = powerRect.Right - toggleSize - padding;
                    procToggleRect = new Rectangle(procToggleX, y, toggleSize, toggleSize);
                }

                if (procToggleRect.Contains(clickX, clickY))
                {
                    return eToggleType.Proc;
                }
            }

            return eToggleType.None;
        }

        public int WhichSlot(int x, int y)
        {
            for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                var powerEntry = MidsContext.Character.CurrentBuild.Powers[i];
                if (powerEntry == null) continue;

                // This part is correct: It finds the button's visual location and size.
                Point cellLocation = PowerPosition(powerEntry);
                const int horizontalPadding = 10;
                int dynamicWidth = _calculatedCellWidth - ScaleLogical(25 + horizontalPadding);
                int buttonX = cellLocation.X + (_calculatedCellWidth - dynamicWidth) / 2;
                var powerRect = new Rectangle(buttonX, cellLocation.Y, dynamicWidth, SzPower.Height);

                // First, check if the click is within the button's horizontal bounds.
                bool isHorizontallyInside = x >= powerRect.Left && x < powerRect.Right;

                if (isHorizontallyInside)
                {
                    // CORRECTED LOGIC: Check two separate vertical areas.

                    // Area 1: Is the click inside the main power button?
                    bool inPowerButton = y >= powerRect.Top && y < powerRect.Bottom;

                    // Area 2: Is the click inside the enhancement slot row?
                    int slotAreaTop = powerRect.Top + OffsetY;
                    bool inEnhancementArea = y >= slotAreaTop && y < slotAreaTop + SzSlot.Height;

                    // If the click is in either valid area, we have a match.
                    if (inPowerButton || inEnhancementArea)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int WhichEnh(int x, int y)
        {
            // 1. Find which power the cursor is over. This part was correct.
            int oPower = WhichSlot(x, y);
            if (oPower <= -1)
            {
                return -1;
            }

            // 2. Get the specific power entry and its cell's top-left coordinates.
            var powerEntry = MidsContext.Character.CurrentBuild.Powers[oPower];
            if (powerEntry?.Slots.Length == 0)
            {
                return -1; // No slots to click.
            }
            Point cellLocation = PowerPosition(powerEntry);

            // 3. Replicate the exact calculation used to determine the power button's rectangle during drawing.
            // This is the crucial step to align hit detection with the visual layout.
            const int horizontalPadding = 10;
            int dynamicWidth = _calculatedCellWidth - ScaleLogical(25 + horizontalPadding);
            int buttonX = cellLocation.X + (_calculatedCellWidth - dynamicWidth) / 2;
            var powerRect = new Rectangle(buttonX, cellLocation.Y, dynamicWidth, SzPower.Height);

            int edge = ComputeSlotEdge(powerRect);
            int spacing = ScaleLogical(2);

            // 4. Calculate the starting X and Y coordinates of the first enhancement slot,
            // mirroring the logic from the DrawSlotsAndEnhancements method.
            const int indent = 5;
            int startX = powerRect.X + ScaleLogical(indent); //
            int startY = powerRect.Y + OffsetY; //

            // 5. Check if the cursor's Y-coordinate is within the vertical bounds of the slots.
            if (y < startY || y >= startY + edge)
            {
                return -1;
            }

            // 6. Calculate the total width of the enhancement slot row for a horizontal bounds check.
            int totalSlotRowWidth = powerEntry.Slots.Length * (edge + spacing);
            if (x < startX || x >= startX + totalSlotRowWidth)
            {
                return -1;
            }

            // 7. Calculate the cursor's position relative to the start of the slots, then divide
            // by the width of a single slot plus its spacing to find the index.
            int relativeX = x - startX;
            int slotAndGapWidth = edge + spacing;
            int index = relativeX / slotAndGapWidth;

            // 8. Final validation to ensure the calculated index is within the array's bounds.
            if (index >= 0 && index < powerEntry.Slots.Length)
            {
                return index;
            }

            return -1;
        }

        public bool HighlightSlot(int idx, bool force = false)
        {
            if (MidsContext.Character.CurrentBuild.Powers.Count < 1)
            {
                return false;
            }

            if (Highlight == idx && !force)
            {
                return false;
            }

            // 1. Update the state. This is the only variable that needs to change.
            Highlight = idx;

            // 2. Redraw the entire scene to our off-screen buffer.
            // The FullRedraw() method will now automatically:
            //    - Draw the previously highlighted power in its normal state.
            //    - Draw the newly highlighted power (if any) in its highlighted state.
            FullRedraw();

            // 3. Invalidate the control. This tells Windows that the control's appearance
            //    has changed and it needs to trigger a Paint event. The Paint event
            //    will then draw our updated BxBuffer to the screen.
            _cTarget?.Invalidate();

            return true; // Return true to indicate a change occurred.
        }

        private void Blank()
        {
            BxBuffer?.Graphics.Clear(_backColor);
        }

        public void ColorSwitch()
        {
            /*bool useHeroColors = true;
            if (MidsContext.Character != null)
                useHeroColors = MidsContext.Character.IsHero();
            if (MidsContext.Config.DisableVillainColors)
                useHeroColors = true;
            VillainColor = !useHeroColors;*/
            _pColorMatrix = new ColorMatrix(HeroMatrix);
            PImageAttributes ??= new ImageAttributes();
            PImageAttributes.SetColorMatrix(_pColorMatrix);
        }

        public static ImageAttributes GetRecolorIa(bool hero)
        {
            var colorMatrix = new ColorMatrix(HeroMatrix);
            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);
            return imageAttributes;
        }

        private ImageAttributes? GreySlot(bool grey, bool bypassIa = false)
        {
            if (!grey) return bypassIa ? new ImageAttributes() : PImageAttributes;

            checked
            {
                var colorMatrix = new ColorMatrix(HeroMatrix);
                var r = 0;
                do
                {
                    var c = 0;
                    do
                    {
                        if (!bypassIa) colorMatrix[r, c] = _pColorMatrix[r, c];

                        colorMatrix[r, c] = (float)(colorMatrix[r, c] / 1.5);

                        c++;
                    } while (c <= 2);

                    r++;
                } while (r <= 2);

                var imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);
                return imageAttributes;
            }
        }

        private ImageAttributes? Desaturate(bool grey, bool bypassIa = false)
        {
            var tMm = new ColorMatrix([
                [
                    0.299f, 0.299f, 0.299f, 0f, 0f
                ],
                [
                    0.587f, 0.587f, 0.587f, 0f, 0f
                ],
                [
                    0.114f, 0.114f, 0.114f, 0f, 0f
                ],
                [
                    0, 0, 0, 1f, 0
                ],
                [
                    0, 0, 0, 0, 1f
                ]
            ]);
            var tCm = new ColorMatrix(HeroMatrix);
            var r = 0;
            checked
            {
                do
                {
                    var c = 0;
                    do
                    {
                        //controls shading of inherents
                        if (!bypassIa) tCm[r, c] = (_pColorMatrix[r, c] + tMm[r, c]) / 2f;

                        if (grey) tCm[r, c] = (float)(tCm[r, c] / 1.5);

                        c++;
                    } while (c <= 2);

                    r++;
                } while (r <= 2);

                var imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(tCm);
                return imageAttributes;
            }
        }

        public Rectangle GetPowerButtonRect(int hIdx)
        {
            var powers = MidsContext.Character?.CurrentBuild?.Powers;
            if (powers is null || hIdx < 0 || hIdx >= powers.Count) return Rectangle.Empty;

            var powerEntry = powers[hIdx];
            // Match PowerBounds’ “chosen/not chosen” location choice.
            var location = !powerEntry.Chosen && powerEntry.Power != null
                ? PowerPosition(hIdx)
                : PowerPosition(GetVisualIdx(hIdx));

            // Must mirror DrawPowerImage’s dynamic width & centering.
            const int horizontalPadding = 10;                           // keep in sync with DrawPowerImage
            int dynamicWidth = _calculatedCellWidth - ScaleLogical(25 + horizontalPadding);
            if (dynamicWidth < 1) dynamicWidth = 1;                     // guard

            int buttonX = location.X + (_calculatedCellWidth - dynamicWidth) / 2;
            return new Rectangle(buttonX, location.Y, dynamicWidth, SzPower.Height);
        }

        public Rectangle GetPowerAreaRect(int hIdx)
        {
            var btn = GetPowerButtonRect(hIdx);
            if (btn.IsEmpty) return Rectangle.Empty;

            var powers = MidsContext.Character.CurrentBuild.Powers;
            int slotCount = powers[hIdx]?.Slots?.Length ?? 0;

            if (slotCount <= 0)
                return btn;

            int edge = ComputeSlotEdge(btn);                            // same fit logic as drawing
            int totalHeight = btn.Height + OffsetY + edge;
            return new Rectangle(btn.X, btn.Y, btn.Width, totalHeight);
        }

        public Rectangle GetEnhancementSlotRect(int hIdx, int slotIndex)
        {
            var powers = MidsContext.Character?.CurrentBuild?.Powers;
            if (powers is null || hIdx < 0 || hIdx >= powers.Count) return Rectangle.Empty;

            var entry = powers[hIdx];
            var btn = GetPowerButtonRect(hIdx);
            if (btn.IsEmpty || entry is null) return Rectangle.Empty;

            int count = entry.Slots?.Length ?? 0;
            if (slotIndex < 0 || slotIndex >= count) return Rectangle.Empty;

            // Must mirror DrawSlotsAndEnhancements layout precisely.
            int indent = ScaleLogical(5);
            int spacing = ScaleLogical(2);
            int edge = ComputeSlotEdge(btn);

            int startX = btn.X + indent;
            int y = btn.Y + OffsetY;
            int x = startX + slotIndex * (edge + spacing);

            return new Rectangle(x, y, edge, edge);
        }

        public IReadOnlyList<Rectangle> GetEnhancementSlotRects(int hIdx)
        {
            var powers = MidsContext.Character?.CurrentBuild?.Powers;
            if (powers is null || hIdx < 0 || hIdx >= powers.Count) return Array.Empty<Rectangle>();

            var entry = powers[hIdx];
            var btn = GetPowerButtonRect(hIdx);
            if (btn.IsEmpty || entry is null) return Array.Empty<Rectangle>();

            int count = entry.Slots?.Length ?? 0;
            if (count <= 0) return Array.Empty<Rectangle>();

            int indent = ScaleLogical(5);
            int spacing = ScaleLogical(2);
            int edge = ComputeSlotEdge(btn);

            int startX = btn.X + indent;
            int y = btn.Y + OffsetY;

            var result = new Rectangle[count];
            for (int i = 0; i < count; i++)
            {
                int x = startX + i * (edge + spacing);
                result[i] = new Rectangle(x, y, edge, edge);
            }

            return result;
        }

        public Rectangle GetNewSlotHoverRect(int hIdx)
        {
            var powers = MidsContext.Character?.CurrentBuild?.Powers;
            if (powers is null || hIdx < 0 || hIdx >= powers.Count) return Rectangle.Empty;

            var entry = powers[hIdx];
            if (entry?.Power is null || entry.State == ePowerState.Empty) return Rectangle.Empty;

            // This mirrors the gating used where DrawNewSlotHover is called.
            bool canShow = MidsContext.Character.CanPlaceSlot
                           && entry.Slots.Length < 6
                           && InterfaceMode != eInterfaceMode.PowerToggle;

            if (!canShow) return Rectangle.Empty;

            var btn = GetPowerButtonRect(hIdx);
            if (btn.IsEmpty) return Rectangle.Empty;

            int indent = ScaleLogical(5);
            int spacing = ScaleLogical(2);
            int edge = ComputeSlotEdge(btn);

            int startX = btn.X + indent;
            int y = btn.Y + OffsetY;

            int x = startX + (edge + spacing) * entry.Slots.Length;
            return new Rectangle(x, y, edge, edge);
        }

        public Rectangle PowerBoundsUnscaled(int hIdx)
        {
            if (hIdx < 0 || hIdx >= MidsContext.Character.CurrentBuild.Powers.Count)
                return new Rectangle(0, 0, 1, 1);

            var powerEntry = MidsContext.Character.CurrentBuild.Powers[hIdx];
            var location = !powerEntry.Chosen && powerEntry.Power != null
                ? PowerPosition(hIdx)
                : PowerPosition(GetVisualIdx(hIdx));

            return new Rectangle(location.X, location.Y, SzPower.Width, OffsetY + SzSlot.Height);
        }

        public bool WithinPowerBar(Rectangle pBounds, Point e)
        {
            pBounds.Height = SzPower.Height;
            return e.X >= pBounds.Left && e.X < pBounds.Right && e.Y >= pBounds.Top && e.Y < pBounds.Bottom;
        }

        public Point PowerPosition(int powerEntryIdx)
        {
            var powers = MidsContext.Character.CurrentBuild.Powers;
            if (powerEntryIdx < 0 || powerEntryIdx >= powers.Count)
                return Point.Empty;

            return PowerPosition(powers[powerEntryIdx]);
        }

        public Point PowerPosition(PowerEntry? powerEntry, int displayLocation = -1)
        {
            var cr = PowerPositionCr(powerEntry, displayLocation);
            return CRtoXy(cr.X, cr.Y);
        }

        private int[][] GetInherentGrid()
        {
            switch (_vcCols)
            {
                case 2:
                    if (MidsContext.Character.Archetype.ClassType == eClassType.HeroEpic)
                        return
                        [
                            [
                                0, 1
                            ],
                            [
                                2, 3
                            ],
                            [
                                4, 5
                            ],
                            [
                                6, 7
                            ],
                            [
                                8, 9
                            ],
                            [
                                10, 11
                            ],
                            [
                                12, 13
                            ],
                            [
                                14, 15
                            ],
                            [
                                16, 17
                            ],
                            [
                                18, 19
                            ],
                            [
                                20, 21
                            ],
                            [
                                22, 23
                            ],
                            [
                                24, 25
                            ],
                            [
                                26, 27
                            ],
                            [
                                28, 29
                            ],
                            [
                                30, 31
                            ],
                            [
                                32, 33
                            ],
                            [
                                34, 35
                            ],
                            [
                                36, 37
                            ],
                            [
                                38, 39
                            ],
                            [
                                40, 41
                            ],
                            [
                                42, 43
                            ],
                            [
                                44, 45
                            ],
                            [
                                46, 47
                            ],
                            [
                                48, 49
                            ],
                            [
                                50, 51
                            ],
                            [
                                52, 53
                            ],
                            [
                                54, 55
                            ],
                            [
                                56, 57
                            ],
                            [
                                58, 59
                            ]
                        ];

                    return
                    [
                        [
                            0, 1
                        ],
                        [
                            2, 3
                        ],
                        [
                            4, 5
                        ],
                        [
                            6, 7
                        ],
                        [
                            8, 9
                        ],
                        [
                            10, 11
                        ],
                        [
                            12, 13
                        ],
                        [
                            14, 15
                        ],
                        [
                            16, 17
                        ],
                        [
                            18, 19
                        ],
                        [
                            20, 21
                        ],
                        [
                            22, 23
                        ],
                        [
                            24, 25
                        ],
                        [
                            26, 27
                        ],
                        [
                            28, 29
                        ],
                        [
                            30, 31
                        ],
                        [
                            32, 33
                        ],
                        [
                            34, 35
                        ],
                        [
                            36, 37
                        ],
                        [
                            38, 39
                        ],
                        [
                            40, 41
                        ],
                        [
                            42, 43
                        ],
                        [
                            44, 45
                        ],
                        [
                            46, 47
                        ],
                        [
                            48, 49
                        ],
                        [
                            50, 51
                        ],
                        [
                            52, 53
                        ],
                        [
                            54, 55
                        ],
                        [
                            56, 57
                        ],
                        [
                            58, 59
                        ]
                    ];
                case 4:
                    if (MidsContext.Character.Archetype.ClassType == eClassType.HeroEpic)
                        return
                        [
                            [
                                0, 1, 2, 3
                            ],
                            [
                                4, 5, 6, 7
                            ],
                            [
                                8, 9, 10, 11
                            ],
                            [
                                12, 13, 14, 15
                            ],
                            [
                                16, 17, 18, 19
                            ],
                            [
                                20, 21, 22, 23
                            ],
                            [
                                24, 25, 26, 27
                            ],
                            [
                                28, 29, 30, 31
                            ],
                            [
                                32, 33, 34, 35
                            ],
                            [
                                36, 37, 38, 39
                            ],
                            [
                                40, 41, 42, 43
                            ],
                            [
                                44, 45, 46, 47
                            ],
                            [
                                48, 49, 50, 51
                            ],
                            [
                                52, 53, 54, 55
                            ],
                            [
                                56, 57, 58, 59
                            ]
                        ];

                    return
                    [
                        [
                            0, 1, 2, 3
                        ],
                        [
                            4, 5, 6, 7
                        ],
                        [
                            8, 9, 10, 11
                        ],
                        [
                            12, 13, 14, 15
                        ],
                        [
                            16, 17, 18, 19
                        ],
                        [
                            20, 21, 22, 23
                        ],
                        [
                            24, 25, 26, 27
                        ],
                        [
                            28, 29, 30, 31
                        ],
                        [
                            32, 33, 34, 35
                        ],
                        [
                            36, 37, 38, 39
                        ],
                        [
                            40, 41, 42, 43
                        ],
                        [
                            44, 45, 46, 47
                        ],
                        [
                            48, 49, 50, 51
                        ],
                        [
                            52, 53, 54, 55
                        ],
                        [
                            56, 57, 58, 59
                        ]
                    ];
                case 5:
                    if (MidsContext.Character.Archetype.ClassType == eClassType.HeroEpic)
                        return
                        [
                            [
                                0, 1, 2, 3, 4
                            ],
                            [
                                5, 6, 7, 8, 9
                            ],
                            [
                                10, 11, 12, 13, 14
                            ],
                            [
                                15, 16, 17, 18, 19
                            ],
                            [
                                20, 21, 22, 23, 24
                            ],
                            [
                                25, 26, 27, 28, 29
                            ],
                            [
                                30, 31, 32, 33, 34
                            ],
                            [
                                35, 36, 37, 38, 39
                            ],
                            [
                                40, 41, 42, 43, 44
                            ],
                            [
                                45, 46, 47, 48, 49
                            ],
                            [
                                50, 51, 52, 53, 54
                            ],
                            [
                                55, 56, 57, 58, 59
                            ]
                        ];

                    return
                    [
                        [
                            0, 1, 2, 3, 4
                        ],
                        [
                            5, 6, 7, 8, 9
                        ],
                        [
                            10, 11, 12, 13, 14
                        ],
                        [
                            15, 16, 17, 18, 19
                        ],
                        [
                            20, 21, 22, 23, 24
                        ],
                        [
                            25, 26, 27, 28, 29
                        ],
                        [
                            30, 31, 32, 33, 34
                        ],
                        [
                            35, 36, 37, 38, 39
                        ],
                        [
                            40, 41, 42, 43, 44
                        ],
                        [
                            45, 46, 47, 48, 49
                        ],
                        [
                            50, 51, 52, 53, 54
                        ],
                        [
                            55, 56, 57, 58, 59
                        ]
                    ];
                case 6:
                    if (MidsContext.Character.Archetype.ClassType == eClassType.HeroEpic)
                        return
                        [
                            [
                                0, 1, 2, 3, 4, 5
                            ],
                            [
                                6, 7, 8, 9, 10, 11
                            ],
                            [
                                12, 13, 14, 15, 16, 17
                            ],
                            [
                                18, 19, 20, 21, 22, 23
                            ],
                            [
                                24, 25, 26, 27, 28, 29
                            ],
                            [
                                30, 31, 32, 33, 34, 35
                            ],
                            [
                                36, 37, 38, 39, 40, 41
                            ],
                            [
                                42, 43, 44, 45, 46, 47
                            ],
                            [
                                48, 49, 50, 51, 52, 53
                            ],
                            [
                                54, 55, 56, 57, 58, 59
                            ]
                        ];

                    return
                    [
                        [
                            0, 1, 2, 3, 4, 5
                        ],
                        [
                            6, 7, 8, 9, 10, 11
                        ],
                        [
                            12, 13, 14, 15, 16, 17
                        ],
                        [
                            18, 19, 20, 21, 22, 23
                        ],
                        [
                            24, 25, 26, 27, 28, 29
                        ],
                        [
                            30, 31, 32, 33, 34, 35
                        ],
                        [
                            36, 37, 38, 39, 40, 41
                        ],
                        [
                            42, 43, 44, 45, 46, 47
                        ],
                        [
                            48, 49, 50, 51, 52, 53
                        ],
                        [
                            54, 55, 56, 57, 58, 59
                        ]
                    ];
            }

            if (MidsContext.Character.Archetype.ClassType == eClassType.HeroEpic)
                return
                [
                    [
                        0, 1, 2
                    ],
                    [
                        3, 4, 5
                    ],
                    [
                        6, 7, 8
                    ],
                    [
                        9, 10, 11
                    ],
                    [
                        12, 13, 14
                    ],
                    [
                        15, 16, 17
                    ],
                    [
                        18, 19, 20
                    ],
                    [
                        21, 22, 23
                    ],
                    [
                        24, 25, 26
                    ],
                    [
                        27, 28, 29
                    ],
                    [
                        30, 31, 32
                    ],
                    [
                        33, 34, 35
                    ],
                    [
                        36, 37, 38
                    ],
                    [
                        39, 40, 41
                    ],
                    [
                        42, 43, 44
                    ],
                    [
                        45, 46, 47
                    ],
                    [
                        48, 49, 50
                    ],
                    [
                        51, 52, 53
                    ],
                    [
                        54, 55, 56
                    ],
                    [
                        57, 58, 59
                    ]
                ];

            return
            [
                [
                    0, 1, 2
                ],
                [
                    3, 4, 5
                ],
                [
                    6, 7, 8
                ],
                [
                    9, 10, 11
                ],
                [
                    12, 13, 14
                ],
                [
                    15, 16, 17
                ],
                [
                    18, 19, 20
                ],
                [
                    21, 22, 23
                ],
                [
                    24, 25, 26
                ],
                [
                    27, 28, 29
                ],
                [
                    30, 31, 32
                ],
                [
                    33, 34, 35
                ],
                [
                    36, 37, 38
                ],
                [
                    39, 40, 41
                ],
                [
                    42, 43, 44
                ],
                [
                    45, 46, 47
                ],
                [
                    48, 49, 50
                ],
                [
                    51, 52, 53
                ],
                [
                    54, 55, 56
                ],
                [
                    57, 58, 59
                ]
            ];
        }

        private Point PowerPositionCr(PowerEntry? powerEntry, int displayLocation = -1)
        {
            if (powerEntry == null)
                return Point.Empty;

            var powers = MidsContext.Character.CurrentBuild.Powers;
            int powerIdx = powers.IndexOf(powerEntry);

            if (powerIdx == -1)
            {
                for (int i = 0; i < powers.Count; i++)
                {
                    if (powers[i] == null) continue;
                    if (powers[i].Power.PowerIndex == powerEntry.Power.PowerIndex &&
                        powers[i].Level == powerEntry.Level)
                    {
                        powerIdx = i;
                        break;
                    }
                }
            }

            int iRow = 0;
            int iCol = 0;

            // Inherent power not chosen
            if (!powerEntry.Chosen)
            {
                if (displayLocation == -1 && powerEntry.Power != null)
                    displayLocation = powerEntry.Power.DisplayLocation;

                if (displayLocation <= -1)
                    return new Point(iCol, iRow);

                var inherentGrid = GetInherentGrid();
                iRow = _vcRowsPowers;

                for (int r = 0; r < inherentGrid.Length; r++)
                {
                    for (int c = 0; c < inherentGrid[r].Length; c++)
                    {
                        if (inherentGrid[r][c] != displayLocation)
                            continue;

                        // Row math differs for 5-column mode
                        iRow += _vcCols == 5 ? r + 2 : r + 1;
                        iCol = c;
                        return new Point(iCol, iRow);
                    }
                }

                return new Point(iCol, iRow); // fallback
            }

            // Main powers
            if (powerIdx > -1)
            {
                switch (_ColumnStackingMode)
                {
                    case eColumnStacking.Horizontal:
                    case eColumnStacking.Vertical:
                        if (ColumnsPowersLayout.TryGetValue(powerIdx, out var p))
                            return p;
                        break;

                    default:
                        if (_vcCols == 5)
                        {
                            iCol = (int)Math.Floor((double)powerIdx / _vcCols);
                            iRow = powerIdx % _vcCols;
                        }
                        else
                        {
                            for (int i = 1; i <= _vcCols; i++)
                            {
                                if (powerIdx < _vcRowsPowers * i)
                                {
                                    iCol = i - 1;
                                    iRow = powerIdx - _vcRowsPowers * iCol;
                                    break;
                                }
                            }
                        }
                        return new Point(iCol, iRow);
                }
            }

            return new Point(iCol, iRow);
        }

        private Point CRtoXy(int col, int row, bool ignorePadding = false)
        {
            // This now returns the top-left coordinate of the CELL.
            int x = col * _calculatedCellWidth;

            // The Y calculation remains correct.
            int y = row * (SzPower.Height + ScaleLogical(2) + SzSlot.Height);
            if (row >= _vcRowsPowers)
                y += OffsetInherent;
            if (_ColumnStackingMode != eColumnStacking.None)
                y += (int)Math.Round(SzPower.Height / 2f);

            return new Point(x, y);
        }

        public void UpdateLayout(int panelWidth)
        {
            if (_vcCols <= 0) return;

            // Calculate the total width available for each column's cell
            _calculatedCellWidth = panelWidth / _vcCols;
        }

        public Size GetDrawingArea()
        {
            var result = (Size)PowerPosition(VcPowers - 1);
            checked
            {
                result.Width += SzPower.Width;
                result.Height = result.Height + SzPower.Height + PaddingY;
                for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i] != null && (MidsContext.Character.CurrentBuild.Powers[i].Power == null || MidsContext.Character.CurrentBuild.Powers[i].Chosen && i > MidsContext.Character.CurrentBuild.LastPower))
                        continue;
                    var size = new Size(result.Width, PowerPosition(i).Y + SzPower.Height + PaddingY);
                    if (size.Height > result.Height) result.Height = size.Height;

                    if (size.Width > result.Width) result.Width = size.Width;
                }

                return result;
            }
        }

        public Size GetRequiredDrawingArea()
        {
            if (_cTarget is null)
                return Size.Empty;

            // Establish a sane layout baseline even when nothing is chosen yet
            if (_vcCols <= 0)
                _vcCols = Math.Max(1, MidsContext.Config.Columns);

            if (_vcRowsPowers <= 0)
                _vcRowsPowers = (int)Math.Ceiling((double)VcPowers / _vcCols);

            // Core cell height used by both the main grid and the inherent row(s)
            int cellCore = SzPower.Height + ScaleLogical(2) + SzSlot.Height;

            // --- Main grid height (24 picks across _vcCols/_vcRowsPowers) ---
            int mainRows = Math.Max(1, _vcRowsPowers);
            int height = mainRows * cellCore;

            // Stacked layouts reserve extra header room (matches CRtoXy’s Y-offset)
            if (_ColumnStackingMode != eColumnStacking.None)
                height += (int)Math.Round(SzPower.Height / 2f);

            // Gap between main grid and the inherent grid
            height += OffsetInherent;

            // --- Inherent grid height ---
            // Even on a brand-new toon every Archetype has inherent powers,
            // so show at least one inherent row. If we can detect more, account for them.
            int inherentRows = 1;
            var powers = MidsContext.Character.CurrentBuild.Powers;
            if (powers is { Count: > 0 })
            {
                // Count distinct CR rows whose Y is below the inherent split (>= _vcRowsPowers)
                // This is resilient even when IDXPower < 0.
                var rows = new HashSet<int>();
                for (int i = 0; i < powers.Count; i++)
                {
                    var cr = PowerPositionCr(powers[i], -1);
                    if (cr.Y >= _vcRowsPowers)
                        rows.Add(cr.Y);
                }
                if (rows.Count > 0) inherentRows = rows.Count;
            }

            height += inherentRows * cellCore;

            // Bottom padding
            height += PaddingY + SzSlot.Height;

            // Width follows the hosting control’s client width
            int width = Math.Max(1, _cTarget.ClientSize.Width);
            return new Size(width, Math.Max(1, height));
        }

        private void MiniSetCol(int cols)
        {
            if (cols == _vcCols)
            {
                return;
            }

            if (cols < 2 | cols > 6)
            {
                return;
            }

            _vcCols = cols;
            _vcRowsPowers = VcPowers / _vcCols;
        }

        private class DrawVariables
        {
            public PowerEntry PowerEntry { get; set; }
            public Pen Pen { get; set; }
            public Pen Pen2 { get; set; }
            public RectangleF RectangleF { get; set; }
            public StringFormat StringFormat { get; set; }
            public FontStyle FontStyle { get; set; }
            public Font Font { get; set; }
            public int SlotCheck { get; set; }
            public ePowerState PowerState { get; set; }
            public bool CanPlaceSlot { get; set; }
            public bool DrawNewSlot { get; set; }
            public Point Location { get; set; }
            public Rectangle PowerRect { get; set; }
            public Rectangle ToggleRect { get; set; }
            public Rectangle ProcRect { get; set; }
            public Point SlotLocation { get; set; }
            public string Text { get; set; }
            public string Text2 { get; set; }
            public bool SingleDraw { get; set; }
        }
    }
}
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.Core.Theming;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Windows.ApplicationModel;
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
        private const int BasePowerSlotOverlap = 8;
        private const int BaseEnhancementSlotStartGap = 2;
        private const int BaseEnhancementSlotGap = 0;
        private const int BaseEnhancementSlotRightPad = 2;
        private const int BaseOffsetInherent = 10;
        private const float IconWellContentFill = 0.92f;
        private const byte IconVisibleAlphaThreshold = 8;

        private readonly Size _baseSzPower = new(184, 30);
        private readonly Size _baseSzSlot = new Size(32, 32);

        private int _calculatedCellWidth;
        private int _calculatedIconXOffset;
        private readonly Dictionary<Bitmap, Rectangle> _iconVisibleBoundsCache = new();
        private readonly Dictionary<int, BuildPowerGeometry> _geometryCache = [];
        private bool _geometryCacheDirty = true;

#if DEBUG
        private int _debugFullRedrawCount;
        private int _debugBufferReallocationCount;
#endif


        public Size SzPower => new Size(ScaleLogical(_baseSzPower.Width), ScaleLogical(_baseSzPower.Height));

        public Size SzSlot => new(ScaleLogical(_baseSzSlot.Width), ScaleLogical(_baseSzSlot.Height));

        private int PaddingY => ScaleLogical(BasePaddingY);
        private int PowerSlotOverlap => ScaleLogical(BasePowerSlotOverlap);
        private int EnhancementSlotStartGap => ScaleLogical(BaseEnhancementSlotStartGap);
        private int EnhancementSlotSpacing => Math.Max(0, ScaleLogical(BaseEnhancementSlotGap));
        private int EnhancementSlotRightPad => ScaleLogical(BaseEnhancementSlotRightPad);
        private int SlotLevelBandHeight => MidsContext.Config.ShowSlotLevels
            ? Math.Max(ScaleLogical(10), (int)Math.Ceiling(_currentFontSize) + ScaleLogical(2))
            : 0;
        public int OffsetY => Math.Max(0, SzPower.Height - PowerSlotOverlap);

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
        public int SelectedPowerIndex = -1;
        public eInterfaceMode InterfaceMode;

        private bool IsInDesignMode =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            AppDomain.CurrentDomain.FriendlyName.Contains("devenv");

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

        private ApplicationTheme CurrentApplicationTheme
        {
            get
            {
                if (IsInDesignMode)
                {
                    return ThemeManager.DesignTime;
                }

                return ThemeManager.CurrentTheme ?? ThemeManager.DesignTime;
            }
        }

        internal enum BuildHitArea
        {
            None,
            PowerBody,
            EnhancementSlot,
            NewSlot,
            StatToggle,
            ProcToggle
        }

        internal readonly record struct BuildHitTestResult(
            BuildHitArea Area,
            int PowerIndex,
            int EnhancementIndex,
            eToggleType ToggleType,
            Rectangle AnchorRect)
        {
            public static BuildHitTestResult None { get; } =
                new(BuildHitArea.None, -1, -1, eToggleType.None, Rectangle.Empty);

            public bool HasPower => PowerIndex >= 0;
        }

        private sealed class BuildPowerGeometry
        {
            public required int PowerIndex { get; init; }
            public required Rectangle PowerRect { get; init; }
            public required Rectangle PowerAreaRect { get; init; }
            public required Rectangle IconWellRect { get; init; }
            public required Rectangle SlotHitRect { get; init; }
            public required Rectangle[] EnhancementSlotRects { get; init; }
            public required Rectangle NewSlotRect { get; init; }
            public required Rectangle StatToggleRect { get; init; }
            public required Rectangle ProcToggleRect { get; init; }
        }

        private readonly record struct PowerSlotPalette(
            Color OuterStroke,
            Color RimTop,
            Color RimBottom,
            Color FillTop,
            Color FillMid,
            Color FillBottom,
            Color GlossTop,
            Color GlossBottom,
            Color InnerShadow,
            Color HighlightStroke);

        public BuildRenderer(Control targetControl)
        {
            InterfaceMode = eInterfaceMode.Normal;
            _vcCols = 6;
            _vcRowsPowers = 24;
            BxPower = [];
            _cTarget = targetControl;
            _backColor = targetControl.BackColor;
            _defaultFont = new Font("Segoe UI", _currentFontSize);

            InitColumns = MidsContext.Config.Columns;
            ColorSwitch();
            Initialize(); // Load images + buffer setup
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
            var height = SzPower.Height + OffsetY + SzSlot.Height + SlotLevelBandHeight;
            if (isInherent) height += OffsetInherent;
            return height + PaddingY;
        }

        private Point GetCellLocation(PowerEntry powerEntry)
        {
            var location = PowerPosition(powerEntry);
            if (_ColumnStackingMode != eColumnStacking.None)
            {
                location = location with { Y = location.Y + 18 };
            }

            return location;
        }

        private Rectangle GetPowerButtonRect(Point cellLocation)
        {
            const int interColumnGap = 7;
            int dynamicWidth = Math.Max(1, _calculatedCellWidth - ScaleLogical(interColumnGap));
            int buttonX = cellLocation.X + (_calculatedCellWidth - dynamicWidth) / 2;
            return new Rectangle(buttonX, cellLocation.Y, dynamicWidth, SzPower.Height);
        }

        private static Rectangle UnionNonEmpty(Rectangle left, Rectangle right)
        {
            if (left.IsEmpty) return right;
            if (right.IsEmpty) return left;
            return Rectangle.Union(left, right);
        }

        private bool CanOfferNewSlot(PowerEntry powerEntry)
        {
            return powerEntry.Power is { Slottable: true }
                   && powerEntry.State != ePowerState.Empty
                   && MidsContext.Character.CanPlaceSlot
                   && MidsContext.Character.SlotCheck(powerEntry) >= 0
                   && powerEntry.Slots.Length < 6
                   && InterfaceMode != eInterfaceMode.PowerToggle;
        }

        private (Rectangle StatToggleRect, Rectangle ProcToggleRect) GetToggleRects(PowerEntry powerEntry, Rectangle powerRect)
        {
            int toggleSize = ScaleLogical(15);
            int padding = ScaleLogical(8);
            int y = powerRect.Top + (powerRect.Height - toggleSize) / 2;

            Rectangle statToggleRect = Rectangle.Empty;
            Rectangle procToggleRect = Rectangle.Empty;

            bool canShowStatToggle = powerEntry.CanIncludeForStats();
            bool canShowProcToggle = powerEntry.HasProc();

            if (canShowStatToggle)
            {
                int statToggleX = powerRect.Right - toggleSize - padding;
                statToggleRect = new Rectangle(statToggleX, y, toggleSize, toggleSize);
            }

            if (canShowProcToggle)
            {
                int procToggleX = canShowStatToggle
                    ? statToggleRect.Left - toggleSize - padding
                    : powerRect.Right - toggleSize - padding;
                procToggleRect = new Rectangle(procToggleX, y, toggleSize, toggleSize);
            }

            return (statToggleRect, procToggleRect);
        }

        private Rectangle GetIconWellRect(Rectangle powerRect)
        {
            int extra = Math.Max(8, (int)Math.Round(powerRect.Height * 0.42f));
            int diameter = Math.Max(1, powerRect.Height + extra);
            int y = powerRect.Y - extra / 2;
            return new Rectangle(powerRect.X, y, diameter, diameter);
        }

        private Rectangle GetPowerBodyRect(Rectangle powerRect, Rectangle iconWellRect)
        {
            int overlapStart = (int)Math.Round(iconWellRect.Width * 0.44f);
            int bodyLeft = iconWellRect.Left + overlapStart;
            return new Rectangle(
                bodyLeft,
                powerRect.Y,
                Math.Max(1, powerRect.Right - bodyLeft),
                powerRect.Height);
        }

        private int GetEnhancementSlotStartX(Rectangle powerRect, Rectangle iconWellRect)
        {
            int legacyIndent = powerRect.X + ScaleLogical(5);
            Rectangle iconSocketRect = GetIconSocketRect(iconWellRect);
            int iconAnchoredStart = iconSocketRect.Right + EnhancementSlotStartGap;
            return Math.Max(legacyIndent, iconAnchoredStart);
        }

        private static Rectangle GetSocketRect(Rectangle outerRect, float insetRatio, int minInset)
        {
            int inset = Math.Max(minInset, (int)Math.Round(outerRect.Width * insetRatio));
            return DeflateRect(outerRect, inset);
        }

        private Rectangle GetIconSocketRect(Rectangle iconWellRect)
        {
            return GetSocketRect(iconWellRect, 0.16f, 3);
        }

        private Rectangle GetVisibleBitmapBounds(Bitmap bitmap)
        {
            if (_iconVisibleBoundsCache.TryGetValue(bitmap, out Rectangle cachedBounds))
            {
                return cachedBounds;
            }

            Rectangle fullBounds = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            if (bitmap.Width <= 0 || bitmap.Height <= 0)
            {
                return fullBounds;
            }

            try
            {
                int left = bitmap.Width;
                int top = bitmap.Height;
                int right = -1;
                int bottom = -1;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        if (bitmap.GetPixel(x, y).A <= IconVisibleAlphaThreshold)
                        {
                            continue;
                        }

                        left = Math.Min(left, x);
                        top = Math.Min(top, y);
                        right = Math.Max(right, x);
                        bottom = Math.Max(bottom, y);
                    }
                }

                if (right >= left && bottom >= top)
                {
                    cachedBounds = Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
                }
                else
                {
                    cachedBounds = fullBounds;
                }
            }
            catch
            {
                cachedBounds = fullBounds;
            }

            _iconVisibleBoundsCache[bitmap] = cachedBounds;
            return cachedBounds;
        }

        private Rectangle GetSocketIconDestinationRect(Bitmap bitmap, Rectangle socketRect)
        {
            if (bitmap.Width <= 0 || bitmap.Height <= 0 || socketRect.Width <= 0 || socketRect.Height <= 0)
            {
                return socketRect;
            }

            Rectangle visibleBounds = GetVisibleBitmapBounds(bitmap);
            if (visibleBounds.Width <= 0 || visibleBounds.Height <= 0)
            {
                visibleBounds = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            }

            float targetWidth = socketRect.Width * IconWellContentFill;
            float targetHeight = socketRect.Height * IconWellContentFill;
            float scale = Math.Min(targetWidth / visibleBounds.Width, targetHeight / visibleBounds.Height);
            scale = Math.Max(scale, 0.01f);

            float destWidth = bitmap.Width * scale;
            float destHeight = bitmap.Height * scale;
            float socketCenterX = socketRect.X + socketRect.Width / 2f;
            float socketCenterY = socketRect.Y + socketRect.Height / 2f;
            float visibleCenterX = visibleBounds.X + visibleBounds.Width / 2f;
            float visibleCenterY = visibleBounds.Y + visibleBounds.Height / 2f;
            float x = socketCenterX - visibleCenterX * scale;
            float y = socketCenterY - visibleCenterY * scale;

            int left = (int)Math.Round(x);
            int top = (int)Math.Round(y);
            int right = (int)Math.Round(x + destWidth);
            int bottom = (int)Math.Round(y + destHeight);
            return Rectangle.FromLTRB(left, top, Math.Max(left + 1, right), Math.Max(top + 1, bottom));
        }

        private int GetPowerTopInset()
        {
            Rectangle samplePowerRect = new Rectangle(0, 0, SzPower.Width, SzPower.Height);
            Rectangle iconWellRect = GetIconWellRect(samplePowerRect);
            return Math.Max(0, -iconWellRect.Top + Math.Max(1, ScaleLogical(1)));
        }

        private void MarkGeometryCacheDirty()
        {
            _geometryCacheDirty = true;
        }

        private void ResetGeometryCacheIfNeeded()
        {
            if (!_geometryCacheDirty)
            {
                return;
            }

            _geometryCache.Clear();
            _geometryCacheDirty = false;
        }

        private BuildPowerGeometry CreatePowerGeometry(int powerIndex, PowerEntry powerEntry)
        {
            Rectangle powerRect = GetPowerButtonRect(GetCellLocation(powerEntry));
            Rectangle iconWellRect = GetIconWellRect(powerRect);
            Rectangle bodyRect = GetPowerBodyRect(powerRect, iconWellRect);
            var enhancementSlotRects = Array.Empty<Rectangle>();
            Rectangle powerAreaRect = powerRect;
            Rectangle slotHitRect = Rectangle.Empty;
            Rectangle newSlotRect = Rectangle.Empty;
            bool canOfferNewSlot = CanOfferNewSlot(powerEntry);

            if (powerEntry.Slots.Length > 0 || canOfferNewSlot)
            {
                int startX = GetEnhancementSlotStartX(powerRect, iconWellRect);
                var (edge, spacing) = ComputeEnhancementSlotLayout(powerRect, startX);
                int y = powerRect.Y + OffsetY;

                if (powerEntry.Slots.Length > 0)
                {
                    enhancementSlotRects = new Rectangle[powerEntry.Slots.Length];
                    for (int i = 0; i < enhancementSlotRects.Length; i++)
                    {
                        enhancementSlotRects[i] = new Rectangle(
                            startX + i * (edge + spacing),
                            y,
                            edge,
                            edge);
                    }
                }

                if (canOfferNewSlot)
                {
                    newSlotRect = new Rectangle(
                        startX + (edge + spacing) * powerEntry.Slots.Length,
                        y,
                        edge,
                        edge);
                }

                int slotHitCount = powerEntry.Slots.Length + (newSlotRect.IsEmpty ? 0 : 1);
                if (slotHitCount > 0)
                {
                    int slotBandWidth = slotHitCount * edge + Math.Max(0, slotHitCount - 1) * spacing;
                    slotHitRect = new Rectangle(startX, y, slotBandWidth, edge);
                }

                Rectangle slotBandRect = Rectangle.Empty;
                if (enhancementSlotRects.Length > 0)
                {
                    slotBandRect = enhancementSlotRects[0];
                    slotBandRect = Rectangle.Union(slotBandRect, enhancementSlotRects[^1]);
                }

                slotBandRect = UnionNonEmpty(slotBandRect, newSlotRect);
                if (MidsContext.Config.ShowSlotLevels && enhancementSlotRects.Length > 0)
                {
                    var slotLevelBandRect = new Rectangle(
                        startX,
                        y + edge,
                        enhancementSlotRects.Length * edge + Math.Max(0, enhancementSlotRects.Length - 1) * spacing,
                        SlotLevelBandHeight);
                    slotBandRect = UnionNonEmpty(slotBandRect, slotLevelBandRect);
                }
                powerAreaRect = UnionNonEmpty(powerAreaRect, slotBandRect);
            }

            powerAreaRect = UnionNonEmpty(powerAreaRect, iconWellRect);
            powerAreaRect = UnionNonEmpty(powerAreaRect, bodyRect);

            var (statToggleRect, procToggleRect) = GetToggleRects(powerEntry, powerRect);

            return new BuildPowerGeometry
            {
                PowerIndex = powerIndex,
                PowerRect = powerRect,
                PowerAreaRect = powerAreaRect,
                IconWellRect = iconWellRect,
                SlotHitRect = slotHitRect,
                EnhancementSlotRects = enhancementSlotRects,
                NewSlotRect = newSlotRect,
                StatToggleRect = statToggleRect,
                ProcToggleRect = procToggleRect
            };
        }

        private bool TryGetPowerEntry(int powerIndex, out PowerEntry? powerEntry)
        {
            powerEntry = null;

            var powers = MidsContext.Character?.CurrentBuild?.Powers;
            if (powers is null || powerIndex < 0 || powerIndex >= powers.Count)
            {
                return false;
            }

            powerEntry = powers[powerIndex];
            if (powerEntry == null)
            {
                return false;
            }

            if (ShouldSuppressHiddenSupportPower(powerEntry))
            {
                return false;
            }

            return true;
        }

        private bool TryBuildPowerGeometry(int powerIndex, out BuildPowerGeometry? geometry)
        {
            geometry = null;
            ResetGeometryCacheIfNeeded();

            if (_geometryCache.TryGetValue(powerIndex, out geometry))
            {
                return true;
            }

            if (!TryGetPowerEntry(powerIndex, out var powerEntry))
            {
                return false;
            }

            geometry = CreatePowerGeometry(powerIndex, powerEntry!);
            _geometryCache[powerIndex] = geometry;
            return true;
        }

        private static bool ShouldSuppressHiddenSupportPower(PowerEntry? powerEntry)
        {
            return powerEntry is
            {
                Chosen: false,
                Power:
                {
                    HiddenPower: true,
                    InherentType: eGridType.None
                }
            };
        }

        private static bool IsEffectivelyStatIncluded(PowerEntry? powerEntry)
        {
            if (powerEntry?.Power == null)
            {
                return false;
            }

            if (PowerEntry.ShouldForceAutoIncluded(powerEntry.Power))
            {
                return true;
            }

            if (PlannerStateCatalog.TryGetDefinition(powerEntry.Power.FullName, out var definition) &&
                definition.IsModeControl)
            {
                return MidsContext.Character?.ActivePlannerModes.Contains(definition.Mode) == true;
            }

            return powerEntry.StatInclude;
        }

        internal BuildHitTestResult HitTest(Point clientPoint) => HitTest(clientPoint.X, clientPoint.Y);

        internal BuildHitTestResult HitTest(int x, int y)
        {
            var powers = MidsContext.Character?.CurrentBuild?.Powers;
            if (powers is null)
            {
                return BuildHitTestResult.None;
            }

            var point = new Point(x, y);
            for (int i = 0; i < powers.Count; i++)
            {
                if (!TryBuildPowerGeometry(i, out var geometry))
                {
                    continue;
                }

                if (!geometry.StatToggleRect.IsEmpty && geometry.StatToggleRect.Contains(point))
                {
                    return new BuildHitTestResult(BuildHitArea.StatToggle, i, -1, eToggleType.Stat, geometry.PowerRect);
                }

                if (!geometry.ProcToggleRect.IsEmpty && geometry.ProcToggleRect.Contains(point))
                {
                    return new BuildHitTestResult(BuildHitArea.ProcToggle, i, -1, eToggleType.Proc, geometry.PowerRect);
                }

                for (int slotIndex = 0; slotIndex < geometry.EnhancementSlotRects.Length; slotIndex++)
                {
                    if (geometry.EnhancementSlotRects[slotIndex].Contains(point))
                    {
                        return new BuildHitTestResult(
                            BuildHitArea.EnhancementSlot,
                            i,
                            slotIndex,
                            eToggleType.None,
                            geometry.EnhancementSlotRects[slotIndex]);
                    }
                }

                if (!geometry.NewSlotRect.IsEmpty && geometry.NewSlotRect.Contains(point))
                {
                    return new BuildHitTestResult(BuildHitArea.NewSlot, i, -1, eToggleType.None, geometry.NewSlotRect);
                }

                if (geometry.PowerAreaRect.Contains(point) ||
                    (!geometry.SlotHitRect.IsEmpty && geometry.SlotHitRect.Contains(point)))
                {
                    return new BuildHitTestResult(BuildHitArea.PowerBody, i, -1, eToggleType.None, geometry.PowerRect);
                }
            }

            return BuildHitTestResult.None;
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
            {
                BxBuffer = new ExtendedBitmap(newSize);
#if DEBUG
                _debugBufferReallocationCount++;
                Debug.WriteLine($"[BuildRenderer] buffer realloc #{_debugBufferReallocationCount} -> {newSize.Width}x{newSize.Height}");
#endif
            }

            ConfigureGraphics(BxBuffer.Graphics);

            // Update layout (e.g., rows/columns per stacking)
            if (_ColumnStackingMode != eColumnStacking.None)
            {
                GetPowersLayout();
            }

            MarkGeometryCacheDirty();
            FullRedraw();
        }

        internal void ApplyLiveResize(Control target, int panelWidth, float masterScale)
        {
            if (target.IsDisposed || panelWidth <= 0)
            {
                return;
            }

            _cTarget = target;
            _backColor = target.BackColor;

            InitDpi();

            MasterScale = masterScale;
            UpdateFontScale(masterScale);
            UpdateLayout(panelWidth);

            var required = GetRequiredDrawingArea();
            var newSize = new Size(panelWidth, required.Height);
            if (BxBuffer == null || BxBuffer.Size != newSize)
            {
                BxBuffer = new ExtendedBitmap(newSize);
#if DEBUG
                _debugBufferReallocationCount++;
                Debug.WriteLine($"[BuildRenderer] live buffer realloc #{_debugBufferReallocationCount} -> {newSize.Width}x{newSize.Height}");
#endif
            }

            ConfigureGraphics(BxBuffer.Graphics);

            if (_ColumnStackingMode != eColumnStacking.None)
            {
                GetPowersLayout();
            }

            MarkGeometryCacheDirty();
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

            using var pen = new Pen(Color.Goldenrod, Math.Max(1.5f, EffectiveScale * 1.6f));
            using var font = new Font("Segoe UI", 14f, FontStyle.Bold, GraphicsUnit.Pixel);

            string label = "Inherent Powers";
            SizeF textSize = BxBuffer.Graphics.MeasureString(label, font);
            float lineGap = ScaleLogical(12);
            float textHeight = font.GetHeight(BxBuffer.Graphics) + ScaleLogical(2);
            float textX = (BxBuffer.Size.Width - textSize.Width) / 2f;
            float textY = y - textHeight / 2f - ScaleLogical(1);
            var textBounds = new RectangleF(textX, textY, textSize.Width, textHeight);

            float leftLineEnd = textBounds.Left - lineGap;
            float rightLineStart = textBounds.Right + lineGap;

            if (leftLineEnd > 2f)
            {
                BxBuffer.Graphics.DrawLine(pen, 2f, y, leftLineEnd, y);
            }

            if (rightLineStart < BxBuffer.Size.Width)
            {
                BxBuffer.Graphics.DrawLine(pen, rightLineStart, y, BxBuffer.Size.Width, y);
            }

            Color headerFill = MidsContext.Character.IsHero()
                ? Color.FromArgb(166, 224, 255)
                : Color.FromArgb(255, 178, 178);
            DrawOutlineText(label, textBounds, headerFill, Color.FromArgb(228, 0, 0, 0), font,
                Math.Max(2.4f, EffectiveScale * 2.1f), BxBuffer.Graphics);
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

                if (ShouldSuppressHiddenSupportPower(power))
                    continue;

                if (!ShouldDrawPower(i, power))
                    continue;

                var slotToDraw = power;
                DrawPowerSlot(ref slotToDraw, IsPowerEmphasized(i));
                powers[i] = slotToDraw;
            }

            DrawSplit();
        }

        private bool IsPowerEmphasized(int powerIndex)
        {
            return Highlight == powerIndex || SelectedPowerIndex == powerIndex;
        }

        private static bool IsIncarnateOrIncluded(PowerEntry? powerEntry)
        {
            return powerEntry?.Power != null &&
                   (powerEntry.Power.GroupName == "Incarnate" || powerEntry.Power.IncludeFlag);
        }

        private bool ShouldDrawPower(int powerIndex, PowerEntry? powerEntry)
        {
            return powerEntry != null &&
                   (IsPowerEmphasized(powerIndex) || powerEntry.Chosen || IsIncarnateOrIncluded(powerEntry));
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

            var drawVars = InitializeDrawVariables(powerEntry, singleDraw);
            UpdatePowerState(ref drawVars, singleDraw);
            UpdateImageAttributes(ref drawVars);
            DrawPowerComponents(drawVars);
            return drawVars.Location;
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
                if (drawVars.IsHovered && isValidForOpen)
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

            TryBuildPowerGeometry(drawVars.PowerIndex, out var geometry);
            drawVars.Geometry = geometry ?? CreatePowerGeometry(drawVars.PowerIndex, drawVars.PowerEntry);
            drawVars.PowerRect = drawVars.Geometry.PowerRect;

            DrawPowerImage(
                drawVars.PowerEntry,
                drawVars.PowerRect,
                drawVars.PowerState,
                toggling,
                imageAttr,
                grey,
                drawVars.SingleDraw
            );
        }

        private void DrawPowerComponents(DrawVariables drawVars)
        {
            DrawToggles(drawVars.Geometry, drawVars.Pen2);
            DrawSlotsAndEnhancements(drawVars.PowerEntry, drawVars.Geometry, drawVars.Pen, drawVars.Font);
            DrawNewSlotHover(drawVars.Geometry, drawVars.Font, drawVars.PowerState, drawVars.SlotCheck, drawVars.DrawNewSlot);
            DrawPowerText(drawVars.PowerEntry, drawVars.PowerRect, drawVars.Font, drawVars.PowerState);
        }

        private DrawVariables InitializeDrawVariables(PowerEntry? powerEntry, bool singleDraw)
        {
            var isBold = MidsContext.Config.RtFont.PowersBold;
            
            var font = new Font(_defaultFont.FontFamily, _currentFontSize, isBold ? FontStyle.Bold : FontStyle.Regular,
                GraphicsUnit.Pixel);

            var drawVars = new DrawVariables
            {
                PowerIndex = MidsContext.Character.CurrentBuild.Powers.IndexOf(powerEntry),
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
                IsHovered = Highlight == MidsContext.Character.CurrentBuild.Powers.IndexOf(powerEntry),
                DrawNewSlot = powerEntry.Power is not null
                              && powerEntry.State != ePowerState.Empty
                              && MidsContext.Character.CanPlaceSlot
                              && powerEntry.Slots.Length < 6
                              && Highlight == MidsContext.Character.CurrentBuild.Powers.IndexOf(powerEntry)
                              && powerEntry.Power.Slottable
                              && InterfaceMode != eInterfaceMode.PowerToggle,
                SingleDraw = singleDraw
            };

            drawVars.Location = GetCellLocation(powerEntry);
            return drawVars;
        }

        private void DrawPowerImage(PowerEntry? iSlot, Rectangle powerRect, ePowerState ePowerState, bool toggling, ImageAttributes? imageAttr, bool grey, bool emphasized)
        {
            var effectiveState = ePowerState;
            var statIncluded = IsEffectivelyStatIncluded(iSlot);

            if (toggling && iSlot != null)
            {
                if (statIncluded && iSlot.State == ePowerState.Used)
                {
                    effectiveState = ePowerState.Open;
                }
                else if (effectiveState == ePowerState.Open)
                {
                    effectiveState = ePowerState.Empty;
                }
            }

            var theme = CurrentTheme;
            PowerSlotPalette palette = GetPowerSlotPalette(effectiveState, theme);
            DrawVectorPowerSlot(BxBuffer.Graphics, powerRect, effectiveState, theme, emphasized);
            Rectangle iconWellRect = GetIconWellRect(powerRect);
            DrawPowerIcon(iSlot, iconWellRect, grey, palette);
            DrawIconWellOverlay(BxBuffer.Graphics, iconWellRect, palette);
        }

        private void DrawToggles(BuildPowerGeometry geometry, Pen pen)
        {
            var powerEntry = MidsContext.Character.CurrentBuild.Powers[geometry.PowerIndex];
            if (powerEntry == null)
            {
                return;
            }

            if (!geometry.StatToggleRect.IsEmpty)
            {
                var statCenter = new PointF(-0.25f, -0.33f);
                using var statBrush = IsEffectivelyStatIncluded(powerEntry)
                    ? MakePathBrush(geometry.StatToggleRect, statCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0))
                    : MakePathBrush(geometry.StatToggleRect, statCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
                BxBuffer.Graphics.FillEllipse(statBrush, geometry.StatToggleRect);
                BxBuffer.Graphics.DrawEllipse(pen, geometry.StatToggleRect);
            }

            if (!geometry.ProcToggleRect.IsEmpty)
            {
                var procCenter = new PointF(-0.25f, -0.33f);
                using var procBrush = !powerEntry.ProcInclude
                    ? MakePathBrush(geometry.ProcToggleRect, procCenter, Color.FromArgb(251, 255, 97), Color.FromArgb(91, 91, 0))
                    : MakePathBrush(geometry.ProcToggleRect, procCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
                BxBuffer.Graphics.FillEllipse(procBrush, geometry.ProcToggleRect);
                BxBuffer.Graphics.DrawEllipse(pen, geometry.ProcToggleRect);
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

        private (int Edge, int Spacing) ComputeEnhancementSlotLayout(Rectangle powerRect, int startX)
        {
            const int maxSlots = 6;
            int availableWidth = powerRect.Right - startX - EnhancementSlotRightPad;
            if (availableWidth <= 0)
            {
                return (1, EnhancementSlotSpacing);
            }

            int spacing = EnhancementSlotSpacing;
            int logicalBase = ScaleLogical(_baseSzSlot.Width);
            int edge = Math.Max(1, (availableWidth - (maxSlots - 1) * spacing) / maxSlots);
            edge = Math.Min(edge, logicalBase);
            return (edge, spacing);
        }

        private void DrawSlotsAndEnhancements(PowerEntry powerEntry, BuildPowerGeometry geometry, Pen pen, Font font)
        {
            if (powerEntry.Slots.Length == 0) return;

            using var slotLevelFont = new Font(
                font.FontFamily,
                Math.Max(8f, font.Size - 1.5f),
                font.Style,
                GraphicsUnit.Pixel);

            for (var i = 0; i < powerEntry.Slots.Length; i++)
            {
                var slot = powerEntry.Slots[i];
                var slotRect = geometry.EnhancementSlotRects[i];
                var slotRectF = new RectangleF(slotRect.X, slotRect.Y, slotRect.Width, slotRect.Height);

                
                SolidBrush solidBrush;
                if (slot.Enhancement.Enh < 0)
                {
                    DrawEmptyEnhancementSlot(BxBuffer.Graphics, slotRect);

                    var invalidByLevelPath = MidsContext.Config.BuildMode == Enums.dmModes.LevelUp &&
                                             !powerEntry.AllowFrontLoading &&
                                             slot.Source == SlotSourceKind.Bought &&
                                             slot.Level < powerEntry.Level;
                    if (MidsContext.Config.CalcEnhLevel == 0 | slot.Level > MidsContext.Config.ForceLevel |
                        InterfaceMode == eInterfaceMode.PowerToggle & !powerEntry.StatInclude |
                        invalidByLevelPath)
                    {
                        solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                        BxBuffer.Graphics.FillEllipse(solidBrush, slotRectF);
                        BxBuffer.Graphics.DrawEllipse(pen, slotRectF);
                    }
                }
                else
                {
                    // Controls if powers or slots are greyed out
                    if (IsInDesignMode) continue;

                    var enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                    var clipRect3 = slotRect;
                    AssetManager.DrawEnhancementAt(BxBuffer.Graphics, clipRect3, enhancement.ImageIdx, slot.Enhancement.Enh, enhancement.TypeID, slot.Enhancement.Grade);

                    var invalidByLevelPath = MidsContext.Config.BuildMode == Enums.dmModes.LevelUp &&
                                             !powerEntry.AllowFrontLoading &&
                                             slot.Source == SlotSourceKind.Bought &&
                                             slot.Level < powerEntry.Level;
                    if (slot.Enhancement.RelativeLevel == 0 | slot.Level > MidsContext.Config.ForceLevel |
                        InterfaceMode == eInterfaceMode.PowerToggle & !powerEntry.StatInclude |
                        invalidByLevelPath |
                        MidsContext.EnhCheckMode & !slot.Enhancement.Obtained)
                    {
                        solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                        var iValue3 = slotRectF;
                        iValue3.Inflate(1f, 1f);
                        BxBuffer.Graphics.FillEllipse(solidBrush, iValue3);
                    }

                    if (slot.Enhancement.Enh > -1)
                        DrawEnhancementLevel(slot, font, BxBuffer.Graphics, ref slotRectF);
                }

                if (!MidsContext.Config.ShowSlotLevels) continue;

                float slotLevelHeight = slotLevelFont.GetHeight(BxBuffer.Graphics);
                var powerTextRect = new RectangleF(
                    slotRectF.X,
                    slotRectF.Bottom + ScaleLogical(1),
                    slotRectF.Width,
                    slotLevelHeight
                );

                DrawOutlineText(
                    Convert.ToString(slot.Level + 1),
                    powerTextRect,
                    Color.White,
                    Color.FromArgb(236, 0, 0, 0),
                    slotLevelFont,
                    2f,
                    BxBuffer.Graphics);
            }
        }

        private static PowerSlotPalette GetDefaultEnhancementSlotPalette()
        {
            return CreatePowerSlotPalette(
                Color.FromArgb(20, 96, 232),
                Color.FromArgb(104, 188, 255),
                Color.FromArgb(6, 52, 156),
                Color.FromArgb(132, 226, 255));
        }

        private PowerSlotPalette GetEmptyEnhancementSlotPalette(PowerSlotTheme theme)
        {
            if (theme == null)
            {
                return GetDefaultEnhancementSlotPalette();
            }

            Color border = ResolveColor(theme.Border, Color.FromArgb(32, 88, 182));
            Color openBorder = ResolveColor(theme.OpenBorder, Blend(border, Color.White, 0.32f));
            Color gradientTop = ResolveColor(theme.GradientTop, border);
            Color gradientBottom = ResolveColor(theme.GradientBottom, Blend(border, Color.Black, 0.42f));
            Color emptyFill = ResolveColor(theme.EmptyFill, Blend(gradientBottom, Color.Black, 0.18f));

            Color rimBase = Blend(border, openBorder, 0.40f);
            Color fillTop = Blend(gradientTop, Color.White, 0.16f);
            Color fillBottom = Blend(gradientBottom, emptyFill, 0.45f);
            Color highlightBase = Blend(openBorder, gradientTop, 0.28f);
            return CreatePowerSlotPalette(rimBase, fillTop, fillBottom, highlightBase);
        }

        private PowerSlotPalette GetNewSlotHoverPalette(ApplicationTheme theme)
        {
            if (theme == null)
            {
                return CreatePowerSlotPalette(
                    Color.FromArgb(196, 154, 42),
                    Color.FromArgb(62, 78, 112),
                    Color.FromArgb(14, 24, 44),
                    Color.FromArgb(255, 240, 188));
            }

            PowerSlotTheme slotTheme = theme.PowerSlot ?? ThemeManager.DesignTime.PowerSlot;
            PowerSlotPalette emptyPalette = GetEmptyEnhancementSlotPalette(slotTheme);

            Color accent = ResolveColor(theme.MenuStrip.AccentColor,
                ResolveColor(theme.DropDownList.HoverBorder,
                    ResolveColor(theme.DataView.Accent, Color.FromArgb(196, 154, 42))));
            Color accentLight = ResolveColor(theme.MenuStrip.AccentLightColor,
                ResolveColor(theme.DropDownList.HoverBorder, Blend(accent, Color.White, 0.38f)));

            if (accentLight.GetBrightness() < 0.22f)
            {
                accentLight = Blend(accent, Color.White, 0.42f);
            }

            Color hoverTop = ResolveColor(slotTheme.HoverGradientTop,
                Blend(ResolveColor(slotTheme.GradientTop, accent), accentLight, 0.16f));
            Color hoverBottom = ResolveColor(slotTheme.HoverGradientBottom,
                Blend(ResolveColor(slotTheme.GradientBottom, accent), accent, 0.14f));
            Color rimBase = Blend(ResolveColor(slotTheme.Border, accent), accent, 0.72f);
            Color fillTop = Blend(emptyPalette.FillTop, hoverTop, 0.38f);
            Color fillBottom = Blend(emptyPalette.FillBottom, hoverBottom, 0.34f);
            Color highlightBase = Blend(accentLight, hoverTop, 0.24f);
            return CreatePowerSlotPalette(rimBase, fillTop, fillBottom, highlightBase);
        }

        private void DrawVectorEnhancementSocket(Graphics g, Rectangle slotRect, PowerSlotPalette palette)
        {
            if (slotRect.Width <= 0 || slotRect.Height <= 0)
            {
                return;
            }

            Rectangle drawRect = slotRect;
            Rectangle socketRect = GetSocketRect(drawRect, 0.10f, 2);
            Rectangle shadowRect = drawRect;
            shadowRect.Offset(Math.Max(1, ScaleLogical(1)), Math.Max(1, ScaleLogical(1)));
            shadowRect.Inflate(Math.Max(1, ScaleLogical(1)), Math.Max(1, ScaleLogical(1)));

            using (var shadowPath = new GraphicsPath())
            {
                shadowPath.AddEllipse(shadowRect);
                using var shadowBrush = new PathGradientBrush(shadowPath)
                {
                    CenterColor = Color.FromArgb(54, 0, 0, 0),
                    CenterPoint = new PointF(
                        shadowRect.X + shadowRect.Width * 0.52f,
                        shadowRect.Y + shadowRect.Height * 0.55f),
                    SurroundColors = Enumerable.Repeat(Color.FromArgb(0, 0, 0, 0), shadowPath.PathPoints.Length).ToArray(),
                    FocusScales = new PointF(0.38f, 0.38f)
                };
                g.FillPath(shadowBrush, shadowPath);
            }

            DrawIconWell(g, drawRect, palette, socketRect);
            DrawIconWellOverlay(g, drawRect, palette, socketRect);
        }

        private void DrawEmptyEnhancementSlot(Graphics g, Rectangle slotRect)
        {
            var palette = GetEmptyEnhancementSlotPalette(CurrentTheme);
            DrawVectorEnhancementSocket(g, slotRect, palette);
        }

        private void DrawNewSlotHover(BuildPowerGeometry geometry, Font font, ePowerState powerState, int slotCheck, bool drawNewSlot)
        {
            if (slotCheck > -1 && powerState is not ePowerState.Empty && drawNewSlot && !geometry.NewSlotRect.IsEmpty)
            {
                var palette = GetNewSlotHoverPalette(CurrentApplicationTheme);
                DrawVectorEnhancementSocket(BxBuffer.Graphics, geometry.NewSlotRect, palette);

                var textRect = new RectangleF(
                    geometry.NewSlotRect.X,
                    geometry.NewSlotRect.Y,
                    geometry.NewSlotRect.Width,
                    geometry.NewSlotRect.Height);
                textRect.Height = _defaultFont.GetHeight(BxBuffer.Graphics);
                textRect.Y += (geometry.NewSlotRect.Height - textRect.Height) / 2f;

                Color accentLight = ResolveColor(CurrentApplicationTheme.MenuStrip.AccentLightColor,
                    ResolveColor(CurrentApplicationTheme.PowerSlot.ForeColor, Color.WhiteSmoke));
                if (accentLight.GetBrightness() < 0.22f)
                {
                    accentLight = Blend(accentLight, Color.White, 0.58f);
                }

                DrawOutlineText(Convert.ToString(slotCheck + 1), textRect,
                    accentLight, Color.FromArgb(216, 0, 0, 0),
                    font, 2f, BxBuffer.Graphics);
            }
        }

        private void DrawPowerText(PowerEntry powerEntry, Rectangle powerRect, Font font, ePowerState powerState)
        {
            string text;
            bool omitLevelPrefix = ShouldOmitPowerLevelPrefix(powerEntry);
            string powerName = powerEntry.Name ?? string.Empty;

            ePowerState displayState = powerEntry.State == ePowerState.Empty && powerState == ePowerState.Open ? powerState : powerEntry.State;

            switch (displayState)
            {
                case ePowerState.Empty:
                case ePowerState.Open:
                    text = omitLevelPrefix && !string.IsNullOrWhiteSpace(powerName)
                        ? powerName
                        : $"({powerEntry.Level + 1})";
                    break;

                case ePowerState.Used:
                default:
                    text = omitLevelPrefix
                        ? powerName
                        : $"({powerEntry.Level + 1}) {powerName}";
                    break;
            }

            var textColor = CurrentTheme.ForeColor.IsEmpty ? Color.WhiteSmoke : CurrentTheme.ForeColor;
            if (displayState is ePowerState.Empty or ePowerState.Open)
            {
                textColor = Blend(textColor, Color.White, 0.12f);
            }

            if (InterfaceMode == eInterfaceMode.PowerToggle && !powerEntry.CanIncludeForStats())
            {
                textColor = Color.FromArgb(168, Blend(textColor, Color.Black, 0.30f));
            }

            var (statToggleRect, procToggleRect) = GetToggleRects(powerEntry, powerRect);
            Rectangle toggleBounds = UnionNonEmpty(statToggleRect, procToggleRect);
            int leftPadding = ScaleLogical(12);
            int rightPadding = ScaleLogical(12);
            Rectangle iconWellRect = GetIconWellRect(powerRect);

            leftPadding = Math.Max(leftPadding, iconWellRect.Right - powerRect.X + ScaleLogical(1));

            if (!toggleBounds.IsEmpty)
            {
                rightPadding = Math.Max(rightPadding, powerRect.Right - toggleBounds.Left + ScaleLogical(6));
            }

            var textRect = new RectangleF
            {
                X = powerRect.X + leftPadding,
                Y = powerRect.Y,
                Width = Math.Max(1, powerRect.Width - leftPadding - rightPadding),
                Height = powerRect.Height - ScaleLogical(3)
            };

            float outlineThickness = MidsContext.Config.EnhanceVisibility
                ? Math.Max(2.4f, EffectiveScale * 2.6f)
                : Math.Max(1.9f, EffectiveScale * 2.1f);
            DrawPowerLabelText(text, textRect, textColor, font, outlineThickness);
        }

        private bool ShouldOmitPowerLevelPrefix(PowerEntry powerEntry)
        {
            var powers = MidsContext.Character?.CurrentBuild?.Powers;
            if (powers == null || powers.Count == 0)
            {
                return false;
            }

            int powerIndex = powers.IndexOf(powerEntry);
            if (powerIndex == -1 && powerEntry.Power != null)
            {
                for (int i = 0; i < powers.Count; i++)
                {
                    if (powers[i] == null)
                    {
                        continue;
                    }

                    if (powers[i].Power != null &&
                        powers[i].Power.PowerIndex == powerEntry.Power.PowerIndex &&
                        powers[i].Level == powerEntry.Level)
                    {
                        powerIndex = i;
                        break;
                    }
                }
            }

            if (powerIndex >= VcPowers && powerIndex > -1)
            {
                return true;
            }

            var position = PowerPositionCr(powerEntry, powerEntry.Power?.DisplayLocation ?? -1);
            return position.Y >= _vcRowsPowers;
        }

        private void DrawPowerLabelText(string text, RectangleF bounds, Color fillColor, Font baseFont, float outlineThickness)
        {
            if (BxBuffer?.Graphics == null || string.IsNullOrWhiteSpace(text) || bounds.Width <= 0f || bounds.Height <= 0f)
            {
                return;
            }

            BxBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            using var powerFont = CreatePowerLabelFont(baseFont);
            using var format = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

            float emSize = powerFont.SizeInPoints * BxBuffer.Graphics.DpiY / 72f;
            using var textPath = new GraphicsPath();
            textPath.AddString(text, powerFont.FontFamily, (int)powerFont.Style, emSize, bounds, format);
            ApplyPowerLabelShear(textPath, bounds);

            Color actualFillColor = Blend(fillColor, Color.White, 0.18f);

            using (var shadowPath = (GraphicsPath)textPath.Clone())
            using (var shadowMatrix = new Matrix())
            using (var shadowBrush = new SolidBrush(Color.FromArgb(182, 0, 0, 0)))
            {
                shadowMatrix.Translate(Math.Max(1f, EffectiveScale * 1.05f), Math.Max(1f, EffectiveScale));
                shadowPath.Transform(shadowMatrix);
                BxBuffer.Graphics.FillPath(shadowBrush, shadowPath);
            }

            using (var outlinePen = new Pen(Color.FromArgb(240, 0, 0, 0), outlineThickness + Math.Max(0.2f, EffectiveScale * 0.15f)))
            {
                outlinePen.LineJoin = LineJoin.Round;
                BxBuffer.Graphics.DrawPath(outlinePen, textPath);
            }

            using (var fillBrush = new SolidBrush(actualFillColor))
            {
                BxBuffer.Graphics.FillPath(fillBrush, textPath);
            }

            var highlightState = BxBuffer.Graphics.Save();
            BxBuffer.Graphics.SetClip(new RectangleF(
                bounds.X,
                bounds.Y,
                bounds.Width,
                Math.Max(2f, bounds.Height * 0.44f)));
            using (var highlightBrush = new SolidBrush(Color.FromArgb(56, 255, 255, 255)))
            {
                BxBuffer.Graphics.FillPath(highlightBrush, textPath);
            }
            BxBuffer.Graphics.Restore(highlightState);
        }

        private void ApplyPowerLabelShear(GraphicsPath textPath, RectangleF bounds)
        {
            if (textPath.PointCount == 0)
            {
                return;
            }

            using var shearMatrix = new Matrix();
            float shear = -0.18f;
            float xCompensation = Math.Max(0.5f, bounds.Height * 0.10f);
            shearMatrix.Translate(-bounds.X, -bounds.Y, MatrixOrder.Append);
            shearMatrix.Shear(shear, 0f, MatrixOrder.Append);
            shearMatrix.Translate(bounds.X + xCompensation, bounds.Y, MatrixOrder.Append);
            textPath.Transform(shearMatrix);
        }

        private Font CreatePowerLabelFont(Font baseFont)
        {
            float size = baseFont.Size + Math.Max(0.15f, EffectiveScale * 0.20f);
            const FontStyle style = FontStyle.Bold;

            foreach (string familyName in new[] { "Arial Black", "Arial", "Tahoma", baseFont.FontFamily.Name })
            {
                try
                {
                    return new Font(familyName, size, style, GraphicsUnit.Pixel);
                }
                catch
                {
                    // Try the next family.
                }
            }

            return new Font(baseFont.FontFamily, size, style, GraphicsUnit.Pixel);
        }

        private void DrawVectorPowerSlot(Graphics g, Rectangle bounds, ePowerState state, PowerSlotTheme theme, bool emphasized)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;

            PowerSlotPalette palette = GetPowerSlotPalette(state, theme);
            Rectangle iconWellRect = GetIconWellRect(bounds);
            Rectangle bodyRect = GetPowerBodyRect(bounds, iconWellRect);
            Rectangle mergedBounds = Rectangle.Union(bodyRect, iconWellRect);
            Rectangle rimBounds = DeflateRect(bodyRect, 1);
            Rectangle rimWellRect = DeflateRect(iconWellRect, 1);
            Rectangle mergedRimBounds = Rectangle.Union(rimBounds, rimWellRect);
            Rectangle faceBounds = DeflateRect(bodyRect, Math.Max(1, ScaleLogical(1)));
            Rectangle glossBounds = new(faceBounds.X, faceBounds.Y, faceBounds.Width, Math.Max(2, faceBounds.Height / 2 + 1));
            Rectangle shadowBounds = new(faceBounds.X, faceBounds.Y + faceBounds.Height / 2, faceBounds.Width, Math.Max(2, faceBounds.Height / 2 + 1));

            using var outerPath = CreateMergedPowerSlotPath(bodyRect, iconWellRect);
            using var outerBrush = new SolidBrush(palette.OuterStroke);
            g.FillPath(outerBrush, outerPath);

            if (rimBounds.Width <= 0 || rimBounds.Height <= 0 || rimWellRect.Width <= 0 || rimWellRect.Height <= 0)
            {
                return;
            }

            using var rimPath = CreateMergedPowerSlotPath(rimBounds, rimWellRect);
            using var rimBrush = CreateThreeStopBrush(mergedRimBounds, palette.RimTop, Blend(palette.RimTop, palette.RimBottom, 0.45f), palette.RimBottom);
            g.FillPath(rimBrush, rimPath);

            if (faceBounds.Width <= 0 || faceBounds.Height <= 0)
            {
                return;
            }

            using var facePath = CreateCapsulePath(faceBounds);
            using var faceBrush = CreateThreeStopBrush(faceBounds, palette.FillTop, palette.FillMid, palette.FillBottom);
            g.FillPath(faceBrush, facePath);

            var glossState = g.Save();
            g.SetClip(new Rectangle(faceBounds.X, faceBounds.Y, faceBounds.Width, Math.Max(2, faceBounds.Height / 2 + 1)), CombineMode.Intersect);
            using (var glossBrush = CreateThreeStopBrush(glossBounds, palette.GlossTop, Blend(palette.GlossTop, palette.GlossBottom, 0.55f), palette.GlossBottom))
            {
                g.FillPath(glossBrush, facePath);
            }
            g.Restore(glossState);

            var shadowState = g.Save();
            g.SetClip(new Rectangle(faceBounds.X, faceBounds.Y + faceBounds.Height / 2, faceBounds.Width, Math.Max(2, faceBounds.Height / 2 + 1)), CombineMode.Intersect);
            using (var shadowBrush = new LinearGradientBrush(shadowBounds,
                       Color.FromArgb(0, palette.InnerShadow),
                       palette.InnerShadow,
                       90f))
            {
                g.FillPath(shadowBrush, facePath);
            }
            g.Restore(shadowState);

            var highlightState = g.Save();
            g.SetClip(new Rectangle(mergedRimBounds.X, mergedRimBounds.Y, mergedRimBounds.Width, Math.Max(2, mergedRimBounds.Height / 2)), CombineMode.Intersect);
            using (var highlightPen = new Pen(palette.HighlightStroke, Math.Max(1.1f, EffectiveScale)))
            {
                highlightPen.LineJoin = LineJoin.Round;
                g.DrawPath(highlightPen, rimPath);
            }
            g.Restore(highlightState);

            using var faceOutlinePen = new Pen(Color.FromArgb(92, palette.HighlightStroke));
            using var outerOutlinePen = new Pen(Color.FromArgb(124, palette.OuterStroke));
            g.DrawPath(faceOutlinePen, facePath);
            g.DrawPath(outerOutlinePen, outerPath);

            if (emphasized)
            {
                Color selectionAccent = GetPowerSelectionAccentColor(CurrentApplicationTheme, theme);
                using var selectionGlowPen = new Pen(Color.FromArgb(82, selectionAccent), Math.Max(1.8f, EffectiveScale * 1.8f));
                using var selectionStrokePen = new Pen(Color.FromArgb(184, Blend(selectionAccent, Color.White, 0.18f)), Math.Max(1.0f, EffectiveScale * 1.05f));
                selectionGlowPen.LineJoin = LineJoin.Round;
                selectionStrokePen.LineJoin = LineJoin.Round;
                g.DrawPath(selectionGlowPen, outerPath);
                g.DrawPath(selectionStrokePen, rimPath);
            }

            Rectangle topLineBounds = DeflateRect(mergedRimBounds, Math.Max(1, ScaleLogical(1)));
            if (topLineBounds.Width > 0 && topLineBounds.Height > 0)
            {
                var lineState = g.Save();
                g.SetClip(new Rectangle(topLineBounds.X, topLineBounds.Y, topLineBounds.Width, Math.Max(2, topLineBounds.Height / 3)), CombineMode.Intersect);
                using var topLinePen = new Pen(Color.FromArgb(140, Blend(palette.HighlightStroke, Color.White, 0.25f)), Math.Max(1f, EffectiveScale));
                g.DrawPath(topLinePen, rimPath);
                g.Restore(lineState);
            }

            DrawIconWell(g, iconWellRect, palette);
        }

        private void DrawPowerIcon(PowerEntry? powerEntry, Rectangle iconWellRect, bool grey, PowerSlotPalette palette)
        {
            if (BxBuffer?.Graphics == null || powerEntry?.Power == null || iconWellRect.Width <= 0 || iconWellRect.Height <= 0)
            {
                return;
            }

            var icon = AssetManager.GetPowerImage(powerEntry.Power);
            if (icon == AssetManager.UnknownIcon)
            {
                icon = AssetManager.GetPowersetImage(powerEntry.Power);
            }

            var bitmap = icon?.Bitmap;
            if (bitmap == null || icon == AssetManager.UnknownIcon)
            {
                return;
            }

            Rectangle socketRect = GetIconSocketRect(iconWellRect);
            if (socketRect.Width <= 0 || socketRect.Height <= 0)
            {
                return;
            }

            Rectangle destRect = GetSocketIconDestinationRect(bitmap, socketRect);
            using var clipPath = new GraphicsPath();
            clipPath.AddEllipse(socketRect);

            GraphicsState priorState = BxBuffer.Graphics.Save();
            BxBuffer.Graphics.SetClip(clipPath);

            if (grey)
            {
                using var greyIa = Desaturate(true, true);
                BxBuffer.Graphics.DrawImage(bitmap, destRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, greyIa);
            }
            else
            {
                BxBuffer.Graphics.DrawImage(bitmap, destRect);
            }

            BxBuffer.Graphics.Restore(priorState);

            if (grey)
            {
                using var dimmer = new SolidBrush(Color.FromArgb(72, 0, 0, 0));
                BxBuffer.Graphics.FillEllipse(dimmer, socketRect);
            }
        }

        private void DrawIconWellOverlay(Graphics g, Rectangle wellRect, PowerSlotPalette palette, Rectangle? customSocketRect = null)
        {
            if (wellRect.Width <= 0 || wellRect.Height <= 0)
            {
                return;
            }

            Rectangle socketRect = customSocketRect ?? GetIconSocketRect(wellRect);
            if (socketRect.Width <= 0 || socketRect.Height <= 0)
            {
                return;
            }

            using var socketPath = new GraphicsPath();
            socketPath.AddEllipse(socketRect);

            Color cavityShadow = Blend(palette.FillBottom, Color.Black, 0.82f);
            Color deepShadow = Blend(cavityShadow, Color.Black, 0.26f);
            Color upperShadow = Blend(palette.InnerShadow, Color.Black, 0.64f);

            var upperLeftShadeState = g.Save();
            g.SetClip(socketPath, CombineMode.Intersect);
            using (var upperLeftShadeBrush = new LinearGradientBrush(
                       new PointF(socketRect.Left, socketRect.Top),
                       new PointF(socketRect.Right, socketRect.Bottom),
                       Color.FromArgb(72, upperShadow),
                       Color.FromArgb(0, upperShadow)))
            {
                g.FillPath(upperLeftShadeBrush, socketPath);
            }
            g.Restore(upperLeftShadeState);

            float aoWidth = Math.Max(2.0f, EffectiveScale * 2.2f);
            var ambientOcclusionState = g.Save();
            g.SetClip(socketPath, CombineMode.Intersect);
            using (var aoPen = new Pen(Color.FromArgb(60, upperShadow), aoWidth))
            {
                g.DrawEllipse(aoPen, socketRect);
            }
            g.Restore(ambientOcclusionState);

            var upperArcState = g.Save();
            g.SetClip(socketPath, CombineMode.Intersect);
            g.SetClip(new Rectangle(
                socketRect.X,
                socketRect.Y,
                Math.Max(2, (int)Math.Round(socketRect.Width * 0.68f)),
                Math.Max(2, (int)Math.Round(socketRect.Height * 0.62f))),
                CombineMode.Intersect);
            using (var upperArcPen = new Pen(Color.FromArgb(92, Blend(upperShadow, Color.Black, 0.24f)), Math.Max(2.2f, EffectiveScale * 2.4f)))
            {
                g.DrawEllipse(upperArcPen, socketRect);
            }
            g.Restore(upperArcState);

            Rectangle innerOcclusionRect = DeflateRect(socketRect, Math.Max(1, ScaleLogical(1)));
            if (innerOcclusionRect.Width > 0 && innerOcclusionRect.Height > 0)
            {
                var innerOcclusionState = g.Save();
                g.SetClip(socketPath, CombineMode.Intersect);
                g.SetClip(new Rectangle(
                    innerOcclusionRect.X,
                    innerOcclusionRect.Y,
                    Math.Max(2, (int)Math.Round(innerOcclusionRect.Width * 0.76f)),
                    Math.Max(2, (int)Math.Round(innerOcclusionRect.Height * 0.72f))),
                    CombineMode.Intersect);
                using (var innerOcclusionPen = new Pen(Color.FromArgb(70, deepShadow), Math.Max(1.2f, EffectiveScale * 1.45f)))
                {
                    g.DrawEllipse(innerOcclusionPen, innerOcclusionRect);
                }
                g.Restore(innerOcclusionState);
            }

            Rectangle innerStrokeRect = DeflateRect(socketRect, Math.Max(1, ScaleLogical(1)));
            if (innerStrokeRect.Width > 0 && innerStrokeRect.Height > 0)
            {
                using var socketInnerShadowStroke = new Pen(Color.FromArgb(56, Blend(palette.OuterStroke, Color.Black, 0.24f)));
                g.DrawEllipse(socketInnerShadowStroke, innerStrokeRect);
            }
        }

        private void DrawIconWell(Graphics g, Rectangle wellRect, PowerSlotPalette palette, Rectangle? customSocketRect = null)
        {
            if (wellRect.Width <= 0 || wellRect.Height <= 0)
            {
                return;
            }

            Rectangle ringRect = DeflateRect(wellRect, Math.Max(2, ScaleLogical(2)));
            Rectangle midRingRect = DeflateRect(wellRect, Math.Max(3, ScaleLogical(3)));
            Rectangle innerBevelRect = DeflateRect(wellRect, Math.Max(4, ScaleLogical(4)));
            Rectangle socketRect = customSocketRect ?? GetIconSocketRect(wellRect);

            if (ringRect.Width <= 0 || ringRect.Height <= 0)
            {
                return;
            }

            using var ringPath = new GraphicsPath();
            ringPath.AddEllipse(ringRect);
            using var ringBrush = CreateThreeStopBrush(ringRect, palette.RimTop, Blend(palette.RimTop, palette.RimBottom, 0.45f), palette.RimBottom);
            g.FillPath(ringBrush, ringPath);

            if (midRingRect.Width > 0 && midRingRect.Height > 0)
            {
                using var midRingPath = new GraphicsPath();
                midRingPath.AddEllipse(midRingRect);
                using var midRingBrush = CreateThreeStopBrush(
                    midRingRect,
                    Blend(palette.RimTop, Color.White, 0.20f),
                    Blend(palette.RimTop, palette.FillTop, 0.28f),
                    Blend(palette.RimBottom, Color.Black, 0.10f));
                g.FillPath(midRingBrush, midRingPath);
            }

            if (innerBevelRect.Width > 0 && innerBevelRect.Height > 0)
            {
                using var innerBevelPath = new GraphicsPath();
                innerBevelPath.AddEllipse(innerBevelRect);
                using var innerBevelBrush = CreateThreeStopBrush(
                    innerBevelRect,
                    Blend(palette.RimTop, palette.FillTop, 0.16f),
                    Blend(palette.RimBottom, palette.FillBottom, 0.34f),
                    Blend(palette.RimBottom, Color.Black, 0.36f));
                g.FillPath(innerBevelBrush, innerBevelPath);
            }

            if (socketRect.Width <= 0 || socketRect.Height <= 0)
            {
                return;
            }

            using var socketPath = new GraphicsPath();
            socketPath.AddEllipse(socketRect);
            Color cavityBase = Blend(palette.FillBottom, Color.Black, 0.66f);
            using (var socketBaseBrush = new SolidBrush(cavityBase))
            {
                g.FillPath(socketBaseBrush, socketPath);
            }

            using (var cavityDepthBrush = new PathGradientBrush(socketPath)
            {
                CenterColor = Color.FromArgb(216, Color.Black),
                CenterPoint = new PointF(socketRect.X + socketRect.Width * 0.50f, socketRect.Y + socketRect.Height * 0.50f),
                SurroundColors = Enumerable.Repeat(Color.FromArgb(0, Color.Black), socketPath.PathPoints.Length).ToArray(),
                FocusScales = new PointF(0.28f, 0.28f)
            })
            {
                g.FillPath(cavityDepthBrush, socketPath);
            }

            var ringHighlightState = g.Save();
            g.SetClip(new Rectangle(ringRect.X, ringRect.Y, ringRect.Width, Math.Max(2, ringRect.Height / 2)), CombineMode.Intersect);
            using (var ringHighlightPen = new Pen(Color.FromArgb(88, palette.HighlightStroke), Math.Max(1.0f, EffectiveScale)))
            {
                g.DrawPath(ringHighlightPen, ringPath);
            }
            g.Restore(ringHighlightState);

        }

        private PowerSlotPalette GetPowerSlotPalette(ePowerState state, PowerSlotTheme theme)
        {
            Color gradientTop = ResolveColor(theme.GradientTop, Color.FromArgb(84, 140, 220));
            Color gradientBottom = ResolveColor(theme.GradientBottom, Color.FromArgb(18, 72, 138));
            Color border = ResolveColor(theme.Border, gradientBottom);
            Color openBorder = ResolveColor(theme.OpenBorder, Blend(border, Color.White, 0.35f));
            Color emptyFill = ResolveColor(theme.EmptyFill, Color.FromArgb(54, 58, 68));
            Color disabledFill = ResolveColor(theme.DisabledFill, Color.FromArgb(36, 38, 44));

            return state switch
            {
                ePowerState.Disabled => CreatePowerSlotPalette(
                    Blend(disabledFill, border, 0.10f),
                    Blend(disabledFill, Color.White, 0.08f),
                    Blend(disabledFill, Color.Black, 0.40f),
                    Blend(disabledFill, Color.White, 0.10f)),
                ePowerState.Empty => CreatePowerSlotPalette(
                    Blend(emptyFill, border, 0.24f),
                    Blend(emptyFill, gradientTop, 0.18f),
                    Blend(emptyFill, Color.Black, 0.28f),
                    Blend(emptyFill, Color.White, 0.16f)),
                ePowerState.Open => CreatePowerSlotPalette(
                    border,
                    Blend(gradientTop, Color.White, 0.10f),
                    Blend(gradientBottom, Color.Black, 0.05f),
                    Blend(openBorder, gradientTop, 0.28f)),
                ePowerState.Used => CreatePowerSlotPalette(
                    Blend(border, Color.Black, 0.08f),
                    Blend(gradientTop, Color.White, 0.06f),
                    Blend(gradientBottom, Color.Black, 0.09f),
                    Blend(openBorder, gradientTop, 0.22f)),
                _ => CreatePowerSlotPalette(
                    Blend(emptyFill, border, 0.24f),
                    Blend(emptyFill, gradientTop, 0.18f),
                    Blend(emptyFill, Color.Black, 0.28f),
                    Blend(emptyFill, Color.White, 0.16f))
            };
        }

        private Color GetPowerSelectionAccentColor(ApplicationTheme applicationTheme, PowerSlotTheme slotTheme)
        {
            Color menuAccent = applicationTheme == null ? Color.Empty : applicationTheme.MenuStrip.AccentColor;
            Color hoverBorder = applicationTheme == null ? Color.Empty : applicationTheme.DropDownList.HoverBorder;
            Color openBorder = slotTheme == null ? Color.Empty : slotTheme.OpenBorder;

            return ResolveColor(menuAccent,
                ResolveColor(hoverBorder,
                    ResolveColor(openBorder, Color.Gold)));
        }

        private static PowerSlotPalette CreatePowerSlotPalette(Color rimBase, Color fillTop, Color fillBottom, Color highlightBase)
        {
            Color outerStroke = Blend(rimBase, Color.Black, 0.52f);
            Color rimTop = Blend(rimBase, Color.White, 0.18f);
            Color rimBottom = Blend(rimBase, Color.Black, 0.22f);
            Color fillMid = Blend(fillTop, fillBottom, 0.56f);
            Color glossTop = Color.FromArgb(92, highlightBase);
            Color glossBottom = Color.FromArgb(0, highlightBase);
            Color innerShadow = Color.FromArgb(104, Blend(fillBottom, Color.Black, 0.58f));
            Color highlightStroke = Color.FromArgb(122, Blend(highlightBase, Color.White, 0.14f));
            return new PowerSlotPalette(outerStroke, rimTop, rimBottom, fillTop, fillMid, fillBottom, glossTop, glossBottom, innerShadow, highlightStroke);
        }

        private static Rectangle DeflateRect(Rectangle rect, int amount)
        {
            return new Rectangle(
                rect.X + amount,
                rect.Y + amount,
                Math.Max(0, rect.Width - amount * 2),
                Math.Max(0, rect.Height - amount * 2));
        }

        private static GraphicsPath CreateCapsulePath(Rectangle bounds)
        {
            var path = new GraphicsPath();
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return path;
            }

            if (bounds.Width <= bounds.Height)
            {
                path.AddEllipse(bounds);
                return path;
            }

            path.AddArc(bounds.X, bounds.Y, bounds.Height, bounds.Height, 90, 180);
            path.AddArc(bounds.Right - bounds.Height, bounds.Y, bounds.Height, bounds.Height, 270, 180);
            path.CloseFigure();
            return path;
        }

        private static GraphicsPath CreateMergedPowerSlotPath(Rectangle bodyBounds, Rectangle wellBounds)
        {
            var path = new GraphicsPath
            {
                FillMode = FillMode.Winding
            };

            if (bodyBounds.Width > 0 && bodyBounds.Height > 0)
            {
                using var bodyPath = CreateCapsulePath(bodyBounds);
                path.AddPath(bodyPath, false);
            }

            if (wellBounds.Width > 0 && wellBounds.Height > 0)
            {
                path.AddEllipse(wellBounds);
            }

            return path;
        }

        private static LinearGradientBrush CreateThreeStopBrush(Rectangle bounds, Color top, Color middle, Color bottom)
        {
            var brush = new LinearGradientBrush(bounds, top, bottom, 90f);
            brush.InterpolationColors = new ColorBlend
            {
                Colors = [top, middle, bottom],
                Positions = [0f, 0.52f, 1f]
            };
            return brush;
        }

        private static Rectangle GetAspectFitBounds(Size sourceSize, Rectangle bounds)
        {
            if (sourceSize.Width <= 0 || sourceSize.Height <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                return bounds;
            }

            float scale = Math.Min((float)bounds.Width / sourceSize.Width, (float)bounds.Height / sourceSize.Height);
            int width = Math.Max(1, (int)Math.Round(sourceSize.Width * scale));
            int height = Math.Max(1, (int)Math.Round(sourceSize.Height * scale));
            int x = bounds.X + (bounds.Width - width) / 2;
            int y = bounds.Y + (bounds.Height - height) / 2;
            return new Rectangle(x, y, width, height);
        }

        private static Color ResolveColor(Color candidate, Color fallback)
        {
            return candidate.IsEmpty ? fallback : candidate;
        }

        private static Color Blend(Color first, Color second, float amountSecond)
        {
            amountSecond = Math.Clamp(amountSecond, 0f, 1f);
            float amountFirst = 1f - amountSecond;
            return Color.FromArgb(
                (int)Math.Round(first.A * amountFirst + second.A * amountSecond),
                (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
                (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
                (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
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
#if DEBUG
                _debugBufferReallocationCount++;
                Debug.WriteLine($"[BuildRenderer] redraw buffer realloc #{_debugBufferReallocationCount} -> {desiredSize.Width}x{desiredSize.Height}");
#endif
            }

            // Clear + set rendering quality
            if (BxBuffer.Graphics != null)
            {
                ConfigureGraphics(BxBuffer.Graphics);
                BxBuffer.Graphics.Clear(_backColor);
            }

            MarkGeometryCacheDirty();
            ResetGeometryCacheIfNeeded();

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

#if DEBUG
            _debugFullRedrawCount++;
            Debug.WriteLine($"[BuildRenderer] full redraw #{_debugFullRedrawCount} size={desiredSize.Width}x{desiredSize.Height}");
#endif
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

            using var textFont = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
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
            var hit = HitTest(clickX, clickY);
            return hit.PowerIndex == powerIndex ? hit.ToggleType : eToggleType.None;
        }

        public int WhichSlot(int x, int y)
        {
            return HitTest(x, y).PowerIndex;
        }

        public int WhichEnh(int x, int y)
        {
            var hit = HitTest(x, y);
            return hit.Area == BuildHitArea.EnhancementSlot ? hit.EnhancementIndex : -1;
        }

        private Rectangle GetPowerRenderBounds(int powerIndex)
        {
            if (!TryBuildPowerGeometry(powerIndex, out var geometry))
            {
                return Rectangle.Empty;
            }

            return GetPowerRenderBounds(geometry!);
        }

        private Rectangle GetPowerRenderBounds(BuildPowerGeometry geometry)
        {
            var bounds = geometry.PowerAreaRect;
            if (!geometry.NewSlotRect.IsEmpty)
            {
                bounds = UnionNonEmpty(bounds, geometry.NewSlotRect);
            }

            int pad = Math.Max(2, ScaleLogical(2));
            bounds.Inflate(pad, pad);

            if (BxBuffer?.Bitmap != null)
            {
                bounds = Rectangle.Intersect(bounds, new Rectangle(Point.Empty, BxBuffer.Bitmap.Size));
            }

            return bounds;
        }

        private void ClearRenderBounds(Rectangle bounds)
        {
            if (bounds.IsEmpty || BxBuffer?.Graphics == null)
            {
                return;
            }

            using var backBrush = new SolidBrush(_backColor);
            BxBuffer.Graphics.FillRectangle(backBrush, bounds);
        }

        private void RedrawPowerRegion(int powerIndex)
        {
            if (powerIndex < 0)
            {
                return;
            }

            if (!TryBuildPowerGeometry(powerIndex, out var geometry) || geometry == null)
            {
                return;
            }

            ClearRenderBounds(GetPowerRenderBounds(geometry));

            if (!TryGetPowerEntry(powerIndex, out var powerEntry) || !ShouldDrawPower(powerIndex, powerEntry))
            {
                return;
            }

            var slotToDraw = powerEntry!;
            DrawPowerSlot(ref slotToDraw, IsPowerEmphasized(powerIndex));
            MidsContext.Character.CurrentBuild.Powers[powerIndex] = slotToDraw;
        }

        private void InvalidatePowerRegions(params int[] powerIndices)
        {
            if (_cTarget is MidsBufferedImagePanel bufferedPanel)
            {
                bufferedPanel.InvalidatePowerRegions(powerIndices);
                return;
            }

            foreach (int powerIndex in powerIndices.Distinct())
            {
                if (powerIndex < 0)
                {
                    continue;
                }

                var bounds = GetPowerRenderBounds(powerIndex);
                if (!bounds.IsEmpty)
                {
                    _cTarget?.Invalidate(bounds);
                }
            }
        }

        private bool HoverTransitionNeedsFullRedraw(int powerIndex)
        {
            return powerIndex >= 0 &&
                   TryGetPowerEntry(powerIndex, out var powerEntry) &&
                   powerEntry != null &&
                   CanOfferNewSlot(powerEntry);
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

            int oldHighlight = Highlight;
            Highlight = idx;

            if (BxBuffer?.Graphics == null || _geometryCacheDirty ||
                HoverTransitionNeedsFullRedraw(oldHighlight) ||
                HoverTransitionNeedsFullRedraw(idx))
            {
                FullRedraw();
                _cTarget?.Invalidate();
                return true;
            }

            RedrawPowerRegion(oldHighlight);
            if (idx != oldHighlight)
            {
                RedrawPowerRegion(idx);
            }

            InvalidatePowerRegions(oldHighlight, idx);

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
            return TryBuildPowerGeometry(hIdx, out var geometry)
                ? geometry!.PowerRect
                : Rectangle.Empty;
        }

        public Rectangle GetPowerAreaRect(int hIdx)
        {
            return TryBuildPowerGeometry(hIdx, out var geometry)
                ? geometry!.PowerAreaRect
                : Rectangle.Empty;
        }

        public Rectangle GetEnhancementSlotRect(int hIdx, int slotIndex)
        {
            if (!TryBuildPowerGeometry(hIdx, out var geometry))
                return Rectangle.Empty;

            return slotIndex >= 0 && slotIndex < geometry!.EnhancementSlotRects.Length
                ? geometry.EnhancementSlotRects[slotIndex]
                : Rectangle.Empty;
        }

        public IReadOnlyList<Rectangle> GetEnhancementSlotRects(int hIdx)
        {
            return TryBuildPowerGeometry(hIdx, out var geometry)
                ? geometry!.EnhancementSlotRects
                : Array.Empty<Rectangle>();
        }

        public Rectangle GetNewSlotHoverRect(int hIdx)
        {
            return TryBuildPowerGeometry(hIdx, out var geometry)
                ? geometry!.NewSlotRect
                : Rectangle.Empty;
        }

        public Rectangle PowerBoundsUnscaled(int hIdx)
        {
            if (hIdx < 0 || hIdx >= MidsContext.Character.CurrentBuild.Powers.Count)
                return new Rectangle(0, 0, 1, 1);

            var powerEntry = MidsContext.Character.CurrentBuild.Powers[hIdx];
            if (ShouldSuppressHiddenSupportPower(powerEntry))
                return Rectangle.Empty;

            var location = !powerEntry.Chosen && powerEntry.Power != null
                ? PowerPosition(hIdx)
                : PowerPosition(GetVisualIdx(hIdx));

            return new Rectangle(location.X, location.Y, SzPower.Width, OffsetY + SzSlot.Height + SlotLevelBandHeight);
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

            int y = ignorePadding ? 0 : GetPowerTopInset();
            y += row * (SzPower.Height + ScaleLogical(2) + SzSlot.Height + SlotLevelBandHeight);
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
            MarkGeometryCacheDirty();
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
                    if (ShouldSuppressHiddenSupportPower(MidsContext.Character.CurrentBuild.Powers[i]))
                        continue;

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
            int cellCore = SzPower.Height + ScaleLogical(2) + SzSlot.Height + SlotLevelBandHeight;

            // --- Main grid height (24 picks across _vcCols/_vcRowsPowers) ---
            int mainRows = Math.Max(1, _vcRowsPowers);
            int height = GetPowerTopInset() + mainRows * cellCore;

            // Stacked layouts reserve extra header room (matches CRtoXy�s Y-offset)
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
                    if (ShouldSuppressHiddenSupportPower(powers[i]))
                        continue;

                    var cr = PowerPositionCr(powers[i], -1);
                    if (cr.Y >= _vcRowsPowers)
                        rows.Add(cr.Y);
                }
                if (rows.Count > 0) inherentRows = rows.Count;
            }

            height += inherentRows * cellCore;

            // Bottom padding
            height += PaddingY + SzSlot.Height;

            // Width follows the hosting control�s client width
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
            MarkGeometryCacheDirty();
        }

        private class DrawVariables
        {
            public PowerEntry PowerEntry { get; set; }
            public int PowerIndex { get; set; }
            public Pen Pen { get; set; }
            public Pen Pen2 { get; set; }
            public RectangleF RectangleF { get; set; }
            public StringFormat StringFormat { get; set; }
            public FontStyle FontStyle { get; set; }
            public Font Font { get; set; }
            public int SlotCheck { get; set; }
            public ePowerState PowerState { get; set; }
            public bool CanPlaceSlot { get; set; }
            public bool IsHovered { get; set; }
            public bool DrawNewSlot { get; set; }
            public Point Location { get; set; }
            public BuildPowerGeometry Geometry { get; set; }
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

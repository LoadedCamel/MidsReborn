using FastDeepCloner;
using FontAwesome.Sharp;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls.Test;
using Mids_Reborn.UI.Renderer;
using Mids_Reborn.UI.Theming;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Mids_Reborn.UI.Controls
{
    public enum MidsDataViewNeoPresentationMode
    {
        CharacterBuild,
        ActorReadOnly
    }

    public partial class MidsDataViewNeo : UserControl
    {
        #region Constants

        private readonly record struct TabDescriptor(string Title, int PageIndex);
        private readonly TabDescriptor[] _allTabs =
        [
            new("INFO", 0),
            new("EFFECTS", 1),
            new("TOTALS", 2),
            new("ENHANCE", 3),
            new("BONUSES", 4)
        ];
        private const int TabPaddingX = 16;
        private const int TabHeight = 24;
        private const int TabSpacing = 4;
        private const int CornerRadius = 4;
        private const int FrameBorderWidth = 2;
        private const int HeaderChromeHeight = 34;
        private const int ContentInset = 8;
        private const int InfoDescriptionDividerHeight = 2;
        private const int InfoShortDescriptionMaxLines = 2;
        private const int InfoLongDescriptionMinLines = 1;
        private const int HeaderOuterInset = 6;
        private const int HeaderActionGap = 3;
        private const int TotalsSectionGap = 10;
        private const int EM_SETMARGINS = 0xD3;
        private const int EM_SETRECT = 0xB3;
        private const int EC_LEFTMARGIN = 0x1;
        private const int EC_RIGHTMARGIN = 0x2;

        #endregion

        #region Win32

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern nint SendMessage(nint hWnd, int msg, nint wParam, nint lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern nint SendMessage(nint hWnd, int msg, nint wParam, ref NativeRect lParam);

        #endregion

        #region Structs

        private struct ItemPairGroup
        {
            public string Label;
            public Func<GroupedFx.FxId, bool> Filter;
            public List<PairedListEx.Item> ItemPairs;
        }

        private struct ItemPairGroupEx
        {
            public string Label;
            public Func<GroupedFx.FxId, bool> Filter;
            public List<KeyValuePair<GroupedFx, PairedListEx.Item>> ItemPairsEx;
        }

        #endregion

        #region Fields

        private int _hoveredTabIndex = -1;
        private int _selectedTabIndex;
        private float _uiScale = 1f;
        private readonly Dictionary<Control, float> _baseFontSizes = new();
        private readonly Dictionary<Control, int> _baseHeights = new();

        private Form? _floatingHostForm;
        private bool _isDocked = true;

        // Docking/parent data for reliable redocking
        private Control? _originalParent;
        private DockStyle _originalDock;
        private int _originalIndex;
        private Rectangle _originalBounds;

        private readonly Color[] _tabSelectedColors =
        [
            Color.FromArgb(55, 115, 220), // INFO (Blue)
            Color.FromArgb(70, 175, 115), // EFFECTS (Green)
            Color.FromArgb(220, 180, 60), // TOTALS (Gold)
            Color.FromArgb(180, 80, 200)  // ENHANCE (Purple)
        ];


        private bool bFloating;
        private ExtendedBitmap? bxFlip;
        private bool _updatingPowerScaler;
        private bool _powerScalerPreviewApplying;
        private readonly System.Windows.Forms.Timer _powerScalerDragTimer = new() { Interval = 33 };
        private int _pendingPowerScaleValue = -1;
        private int HistoryIDX;
        private bool _isLocked;
        private IPower? pBase;
        private IPower? pEnh;
        private IPower? _effectsComparisonBase;
        private IPower? rootPowerBase;
        private IPower? rootPowerEnh;
        private ActorTotalsSnapshot? _actorTotalsSnapshot;
        private ActorCalculationSnapshot? _actorCalculationSnapshot;
        private CalculationContributionSnapshot? _displayContributions;
        private string? _actorPowerSourceDescription;
        private IReadOnlyList<PetAppliedBonusEntry> _actorAppliedBonuses = [];
        private int pLastScaleVal;
        private List<GroupedFx> GroupedRankedEffects = [];
        private List<KeyValuePair<GroupedFx, PairedListEx.Item>> EffectsItemPairs = [];
        private MidsDataViewNeoPresentationMode _presentationMode = MidsDataViewNeoPresentationMode.CharacterBuild;
        private Page? _bonusesView;
        private PowerEffectsGrid? _bonusesGrid;
        private Panel? _totalsStackHost;
        private MidsTotalsSectionPanel? _quickReadSection;
        private MidsTotalsSectionPanel? _defenseSection;
        private MidsTotalsSectionPanel? _resistanceSection;
        private MidsTotalsSectionPanel? _coreMiscSection;
        private MidsTotalsQuickStrip? _quickReadStrip;
        private MidsTotalsBarList? _defenseBarListLeft;
        private MidsTotalsBarList? _defenseBarListRight;
        private MidsTotalsBarList? _resistanceBarListLeft;
        private MidsTotalsBarList? _resistanceBarListRight;
        private MidsTotalsValueGrid? _coreMiscGrid;
        private MidsTotalsDualColumnHost? _defenseListsHost;
        private MidsTotalsDualColumnHost? _resistanceListsHost;

        public PetInfo PetInfo;

#if DEBUG
        private int _debugApplyUiScaleCount;
#endif

        #endregion

        #region Events

        /// <summary>Fires when the selected tab changes.</summary>
        public event EventHandler<int>? TabChanged;

        public delegate void FloatChangeEventHandler();
        public delegate void SlotFlipEventHandler(int powerIndex);
        public delegate void SlotUpdateEventHandler(IPower? power, int val);
        public delegate void LockStateChangedEventHandler(object? sender, bool locked);
        public delegate void EntityDetailsEventHandler(string entityUid, HashSet<string> powers, int basePowerHistoryIdx, PetInfo petInfo);

        public event FloatChangeEventHandler? FloatChange;
        public event SlotFlipEventHandler? SlotFlip;
        public event SlotUpdateEventHandler? SlotUpdate;
        public event LockStateChangedEventHandler? LockStateChanged;
        public event EntityDetailsEventHandler EntityDetails;

        #endregion

        #region Properties

        public bool IsLocked
        {
            get => _isLocked;
            set => SetLock(value, true);
        }

        public MidsDataViewNeoPresentationMode PresentationMode => _presentationMode;

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

        private IReadOnlyList<TabDescriptor> VisibleTabs => _presentationMode == MidsDataViewNeoPresentationMode.ActorReadOnly
            ? _allTabs.Where(tab => tab.PageIndex != 3).ToArray()
            : _allTabs.Where(tab => tab.PageIndex != 4).ToArray();

        private bool HeaderActionsVisible => _presentationMode != MidsDataViewNeoPresentationMode.ActorReadOnly;

        #endregion

        #region Constructor

        public MidsDataViewNeo()
        {
            InitializeComponent();
            InitializeBonusesPage();
            InitializeTotalsPage();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            // Ensure the header panel itself is double-buffered (prevents flicker)
            WinFormsBuffering.Enable(headerPanel);
            ApplyShellLayout();
            ApplyShellTheme();
            infoSDesc.HandleCreated += (_, _) => ApplyShortDescriptionMargins();
            infoSDesc.SizeChanged += (_, _) => ApplyShortDescriptionMargins();

            
            LockButton.Click += LockButton_Click;
            ApplyLockVisuals();

            _selectedTabIndex = 0;
            SelectTab(_selectedTabIndex);
            dvPages.SelectedIndexChanged += DvPages_SelectedIndexChanged;
            midsTrackBar1.ValueChanged += MidsTrackBar_ValueChanged;
            midsTrackBar1.InteractionCompleted += MidsTrackBar_InteractionCompleted;
            _powerScalerDragTimer.Tick += PowerScalerDragTimer_Tick;
            enhanceView.Resize += EnhanceView_Resize;
            pnlEnhActive.SizeChanged += EnhancementPanel_SizeChanged;
            pnlEnhInactive.SizeChanged += EnhancementPanel_SizeChanged;
            pnlEnhActive.Paint += pnlEnhActive_Paint;
            pnlEnhInactive.Paint += pnlEnhInactive_Paint;
            pnlEnhActive.MouseClick += pnlEnhActive_MouseClick;
            pnlEnhInactive.MouseClick += pnlEnhInactive_MouseClick;
            pnlEnhActive.MouseMove += pnlEnhActive_MouseMove;
            pnlEnhInactive.MouseMove += pnlEnhInactive_MouseMove;
            LayoutEnhancementPage();
            totalViewScrollPanel.AvailableClientWidthChanged += TotalViewScrollPanel_AvailableClientWidthChanged;
            totalViewScrollPanel.ContentPanel.SizeChanged += TotalViewContentPanel_SizeChanged;

            PetInfo = new PetInfo();
            if (!DesignMode) ThemeManager.ThemeChanged += ThemeManagerOnThemeChanged;
        }

        private void InitializeBonusesPage()
        {
            _bonusesView = new Page
            {
                AccessibleRole = AccessibleRole.None,
                Anchor = AnchorStyles.None,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                ForeColor = Color.WhiteSmoke,
                Name = "bonusesView",
                Size = infoView.Size,
                Title = "Actor Bonuses"
            };

            _bonusesGrid = new PowerEffectsGrid
            {
                BackColor = Color.FromArgb(1, 7, 15),
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Margin = new Padding(0),
                Name = "bonusesGrid",
                GridPadding = Math.Max(6, ContentInset),
                GroupHeaderHeight = 30,
                DescriptorRowHeight = 34
            };

            _bonusesView.Controls.Add(_bonusesGrid);
            dvPages.Controls.Add(_bonusesView);
            dvPages.Pages.Add(_bonusesView);
        }

        private void InitializeTotalsPage()
        {
            _totalsStackHost = new Panel
            {
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                Location = Point.Empty,
                Size = totalViewScrollPanel.ContentPanel.ClientSize
            };

            _quickReadStrip = new MidsTotalsQuickStrip();
            _defenseBarListLeft = new MidsTotalsBarList { Palette = MidsTotalsBarList.FillPalette.Defense };
            _defenseBarListRight = new MidsTotalsBarList { Palette = MidsTotalsBarList.FillPalette.Defense };
            _resistanceBarListLeft = new MidsTotalsBarList { Palette = MidsTotalsBarList.FillPalette.Resistance };
            _resistanceBarListRight = new MidsTotalsBarList { Palette = MidsTotalsBarList.FillPalette.Resistance };
            _coreMiscGrid = new MidsTotalsValueGrid();

            _defenseListsHost = new MidsTotalsDualColumnHost(_defenseBarListLeft, _defenseBarListRight);
            _resistanceListsHost = new MidsTotalsDualColumnHost(_resistanceBarListLeft, _resistanceBarListRight);

            _quickReadSection = new MidsTotalsSectionPanel
            {
                Title = "Quick Read",
                ContentControl = _quickReadStrip
            };
            _defenseSection = new MidsTotalsSectionPanel
            {
                Title = "Defense",
                TitleIcon = MidsTotalsGlyph.SectionDefense,
                TitleIconColor = Color.FromArgb(140, 190, 255),
                MetaText = "softcap: 45%",
                ContentControl = _defenseListsHost
            };
            _resistanceSection = new MidsTotalsSectionPanel
            {
                Title = "Resistance",
                TitleIcon = MidsTotalsGlyph.SectionResistance,
                TitleIconColor = Color.FromArgb(145, 235, 245),
                ContentControl = _resistanceListsHost
            };
            _coreMiscSection = new MidsTotalsSectionPanel
            {
                Title = "Core / Misc",
                TitleIcon = MidsTotalsGlyph.SectionCore,
                TitleIconColor = Color.FromArgb(235, 210, 120),
                ContentControl = _coreMiscGrid
            };

            ApplyTotalsSectionTheme(CurrentTheme);

            totalViewScrollPanel.ContentPanel.Controls.Clear();
            totalViewScrollPanel.ContentPanel.Controls.Add(_totalsStackHost);
            _totalsStackHost.Controls.Add(_quickReadSection);
            _totalsStackHost.Controls.Add(_defenseSection);
            _totalsStackHost.Controls.Add(_resistanceSection);
            _totalsStackHost.Controls.Add(_coreMiscSection);
        }

        #endregion

        #region Theme

        private void ThemeManagerOnThemeChanged()
        {
            ApplyShellTheme();
            Invalidate(true);
        }

        public void RefreshResponsiveLayout()
        {
            UpdateInfoDescriptionLayout();
            LayoutTotalsSections();
            LayoutEnhancementPage();
            headerPanel.Invalidate();
            totalViewScrollPanel.Invalidate();
            if (enhanceView.Visible)
            {
                enhanceView.Invalidate();
            }
        }

        public void ApplyUiScale(float scale)
        {
            scale = Math.Clamp(scale, 0.90f, 1.25f);
            if (Math.Abs(scale - _uiScale) < 0.01f) return;

            _uiScale = scale;

            SuspendLayout();
            ApplyShellLayout();
            ApplyFontScale(this, scale);
            ScaleHeight(titlePanel, scale);
            ScaleHeight(sliderHost, scale);
            ScaleHeight(infoDamageDisplay, scale);
            ScaleHeight(pnlEnhActive, scale);
            ScaleHeight(pnlEnhInactive, scale);
            ScaleHeight(enhanceSubtitlePanel, scale);
            ApplyTotalsUiScale(scale);

            var buttonSize = Math.Max(22, ScalePx(29));
            DockButton.Width = buttonSize;
            LockButton.Width = buttonSize;
            DockButton.IconSize = Math.Max(18, ScalePx(24));
            LockButton.IconSize = Math.Max(18, ScalePx(24));

            RefreshResponsiveLayout();
            ResumeLayout(performLayout: true);
            headerPanel.Invalidate();
            Invalidate();

#if DEBUG
            _debugApplyUiScaleCount++;
            Debug.WriteLine($"[MidsDataViewNeo] ApplyUiScale #{_debugApplyUiScaleCount} scale={scale:F3}");
#endif
        }

        private int ScalePx(int value) => Math.Max(1, (int)Math.Round(value * _uiScale));

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RefreshResponsiveLayout();
        }

        private void ScaleHeight(Control control, float scale)
        {
            if (!_baseHeights.TryGetValue(control, out var height))
            {
                height = control.Height;
                _baseHeights[control] = height;
            }

            control.Height = Math.Max(1, (int)Math.Round(height * scale));
        }

        private void ApplyFontScale(Control root, float scale)
        {
            foreach (var control in EnumerateControls(root))
            {
                if (control.Font is null) continue;
                if (!_baseFontSizes.TryGetValue(control, out var baseSize))
                {
                    baseSize = control.Font.Size;
                    _baseFontSizes[control] = baseSize;
                }

                var scaledSize = Math.Max(6f, baseSize * scale);
                if (Math.Abs(control.Font.Size - scaledSize) < 0.05f) continue;

                control.Font = new Font(control.Font.FontFamily, scaledSize, control.Font.Style, control.Font.Unit,
                    control.Font.GdiCharSet, control.Font.GdiVerticalFont);
            }
        }

        private static IEnumerable<Control> EnumerateControls(Control root)
        {
            yield return root;
            foreach (Control child in root.Controls)
            {
                foreach (var descendant in EnumerateControls(child))
                {
                    yield return descendant;
                }
            }
        }

        #endregion

        #region Paint (Shell)

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            var theme = CurrentTheme;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var bounds = ClientRectangle;
            var stroke = ScalePx(FrameBorderWidth);
            bounds.Inflate(-stroke / 2, -stroke / 2);
            bounds.Width -= 1;
            bounds.Height -= 1;

            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            var radius = ScalePx(6);
            var headerHeight = Math.Min(ScalePx(HeaderChromeHeight), Math.Max(1, bounds.Height));
            using var framePath = RoundedRect(bounds, radius);
            using var frameBack = new SolidBrush(theme.Background);
            using var frameBorder = new Pen(Blend(theme.Border, theme.TabActiveBottom, 0.55f), stroke);

            g.FillPath(frameBack, framePath);

            var headerRect = new Rectangle(bounds.Left, bounds.Top, bounds.Width, headerHeight);
            using (var previousClip = g.Clip.Clone())
            using (var headerBrush = new LinearGradientBrush(headerRect, theme.HeaderTop, theme.HeaderBottom, LinearGradientMode.Vertical))
            {
                g.SetClip(framePath);
                g.FillRectangle(headerBrush, headerRect);
                g.Clip = previousClip;
            }

            g.DrawPath(frameBorder, framePath);

            var inner = Rectangle.Inflate(bounds, -1, -1);
            if (inner.Width > 0 && inner.Height > 0)
            {
                using var innerPath = RoundedRect(inner, ScalePx(5));
                using var innerBorder = new Pen(Color.FromArgb(110, theme.GridHeaderBorder));
                g.DrawPath(innerBorder, innerPath);
            }
        }

        private void ApplyShellTheme()
        {
            var theme = CurrentTheme;

            BackColor = theme.Background;
            ForeColor = theme.Text;

            headerPanel.BackColor = Color.Transparent;
            titlePanel.BackColor = theme.Background;
            title.ForeColor = theme.Text;

            dvPages.BackColor = theme.Background;
            infoView.BackColor = theme.Background;
            effectView.BackColor = theme.Background;
            totalView.BackColor = theme.Background;
            enhanceView.BackColor = theme.Background;
            if (_bonusesView != null)
            {
                _bonusesView.BackColor = theme.Background;
            }

            infoSDesc.BackColor = theme.Background;
            infoSDesc.ForeColor = theme.Text;
            infoLDesc.BackColor = theme.Background;
            infoLDesc.ForeColor = theme.Text;
            infoDescDivider.BackColor = theme.Background;
            infoDescDivider.Invalidate();
            powerStatsGrid.BackColor = theme.Background;
            effectsGrid.BackColor = theme.Background;

            totalViewScrollPanel.BackColor = theme.Background;
            totalViewScrollPanel.ContentPanel.BackColor = theme.Background;
            coreDataList.BackColor = theme.Background;

            ApplyPairedListTheme(infoDataList, theme);
            ApplyPairedListTheme(coreDataList, theme);
            ApplyPairedListTheme(enhDataList, theme);
            _bonusesGrid?.Invalidate();
            ApplyTotalsSectionTheme(theme);
            ApplyEnhanceSurfaceTheme(theme);
            _quickReadSection?.Invalidate();
            _defenseSection?.Invalidate();
            _resistanceSection?.Invalidate();
            _coreMiscSection?.Invalidate();
            _quickReadStrip?.Invalidate();
            _defenseBarListLeft?.Invalidate();
            _defenseBarListRight?.Invalidate();
            _resistanceBarListLeft?.Invalidate();
            _resistanceBarListRight?.Invalidate();
            _coreMiscGrid?.Invalidate();

            sliderHost.BackColor = theme.Background;

            ConfigureHeaderActionButton(LockButton, theme);
            ConfigureHeaderActionButton(DockButton, theme);

            headerPanel.Invalidate();
        }

        private void ApplyEnhanceSurfaceTheme(DataViewTheme theme)
        {
            var background = theme.Background;

            enhanceView.BackColor = background;
            enhanceSubtitlePanel.BackColor = background;
            subTitle.BackColor = background;
            enhDataList.BackColor = background;
            pnlEnhActive.BackColor = background;
            pnlEnhInactive.BackColor = background;

            ResetFlipBuffer();
            enhanceView.Invalidate(true);
            enhDataList.Invalidate();
        }

        private void ConfigureHeaderActionButton(Button button, DataViewTheme theme)
        {
            button.BackColor = Color.Transparent;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = Blend(theme.TabActiveBottom, theme.Background, 0.35f);
            button.FlatAppearance.MouseOverBackColor = Blend(theme.TabInactiveTop, theme.TabActiveTop, 0.35f);
            button.UseVisualStyleBackColor = false;
        }

        private Rectangle HeaderActionBounds(Control button)
        {
            var inset = ScalePx(HeaderActionGap);
            var height = Math.Min(ScalePx(TabHeight), Math.Max(1, headerPanel.ClientSize.Height - inset * 2));
            var top = Math.Max(inset, (headerPanel.ClientSize.Height - height) / 2);
            return new Rectangle(button.Left + inset, top, Math.Max(1, button.Width - inset * 2), height);
        }

        private void ApplyShellLayout()
        {
            var frameInset = ScalePx(FrameBorderWidth);
            Padding = new Padding(frameInset, 0, frameInset, frameInset);
            headerPanel.Height = ScalePx(HeaderChromeHeight);

            var horizontalInset = ScalePx(ContentInset);
            title.Padding = new Padding(horizontalInset + ScalePx(2), 0, 0, 0);
            infoSDesc.Padding = new Padding(horizontalInset, 0, horizontalInset, 0);
            infoLDesc.Padding = new Padding(horizontalInset, 0, horizontalInset, 0);
            infoDescDivider.Height = Math.Max(4, ScalePx(6));
            ApplyShortDescriptionMargins();

            powerStatsGrid.GridPadding = Math.Max(6, ContentInset);
            effectsGrid.GridPadding = Math.Max(6, ContentInset);
            coreDataList.Padding = new Padding(horizontalInset, 0, horizontalInset, 0);
            enhDataList.Padding = new Padding(horizontalInset, 0, horizontalInset, 0);
            if (_bonusesGrid != null)
            {
                _bonusesGrid.GridPadding = Math.Max(6, ContentInset);
            }
            UpdateInfoDescriptionLayout();
            LayoutTotalsSections();
        }

        private void ApplyTotalsUiScale(float scale)
        {
            if (_quickReadSection == null || _defenseSection == null || _resistanceSection == null || _coreMiscSection == null)
            {
                return;
            }

            _quickReadSection.UiScale = scale;
            _defenseSection.UiScale = scale;
            _resistanceSection.UiScale = scale;
            _coreMiscSection.UiScale = scale;
            if (_quickReadStrip != null) _quickReadStrip.UiScale = scale;
            if (_defenseListsHost != null) _defenseListsHost.UiScale = scale;
            if (_resistanceListsHost != null) _resistanceListsHost.UiScale = scale;
            if (_defenseBarListLeft != null) _defenseBarListLeft.UiScale = scale;
            if (_defenseBarListRight != null) _defenseBarListRight.UiScale = scale;
            if (_resistanceBarListLeft != null) _resistanceBarListLeft.UiScale = scale;
            if (_resistanceBarListRight != null) _resistanceBarListRight.UiScale = scale;
            if (_coreMiscGrid != null) _coreMiscGrid.UiScale = scale;
        }

        private void ApplyTotalsSectionTheme(DataViewTheme theme)
        {
            if (_quickReadSection != null)
            {
                _quickReadSection.TitleColor = Color.FromArgb(150, 215, 255);
                _quickReadSection.MetaColor = Blend(theme.Muted, theme.GridNeutral, 0.30f);
            }

            var sectionTitle = Color.FromArgb(235, 220, 172);
            var sectionMeta = Blend(theme.Muted, theme.GridNeutral, 0.30f);

            if (_defenseSection != null)
            {
                _defenseSection.TitleColor = sectionTitle;
                _defenseSection.MetaColor = sectionMeta;
            }

            if (_resistanceSection != null)
            {
                _resistanceSection.TitleColor = sectionTitle;
                _resistanceSection.MetaColor = sectionMeta;
            }

            if (_coreMiscSection != null)
            {
                _coreMiscSection.TitleColor = sectionTitle;
                _coreMiscSection.MetaColor = sectionMeta;
            }
        }

        private void UpdateInfoDescriptionLayout()
        {
            if (infoSDesc == null || infoLDesc == null || infoView == null || infoDamageDisplay == null)
            {
                return;
            }

            var shortHeight = MeasureShortDescriptionHeight();
            var hasLongDescription = !string.IsNullOrWhiteSpace(infoLDesc.Text);
            var reservedDividerHeight = shortHeight > 0 && hasLongDescription ? Math.Max(4, ScalePx(6)) : 0;
            var targetDamageHeight = CalculateInfoDamageDisplayHeight();
            var longHeight = CalculateLongDescriptionHeight(shortHeight, reservedDividerHeight, targetDamageHeight);
            var dividerVisible = shortHeight > 0 && longHeight > 0;
            var dividerHeight = dividerVisible ? reservedDividerHeight : 0;

            if (!dividerVisible && reservedDividerHeight > 0)
            {
                longHeight = CalculateLongDescriptionHeight(shortHeight, 0, targetDamageHeight);
            }

            if (infoSDesc.Height != shortHeight)
            {
                infoSDesc.Height = shortHeight;
            }
            else
            {
                ApplyShortDescriptionMargins();
            }

            if (infoDescDivider.Visible != dividerVisible)
            {
                infoDescDivider.Visible = dividerVisible;
            }

            if (infoDescDivider.Height != dividerHeight)
            {
                infoDescDivider.Height = dividerHeight;
            }

            if (infoLDesc.Height != longHeight)
            {
                infoLDesc.Height = longHeight;
            }

            if (infoDamageDisplay.Height != targetDamageHeight)
            {
                infoDamageDisplay.Height = targetDamageHeight;
            }

            infoView.PerformLayout();
        }

        private int CalculateInfoDamageDisplayHeight()
        {
            var graphEnabled = infoDamageDisplay.ShowGraph;
            var minHeight = graphEnabled ? ScalePx(82) : ScalePx(50);
            var maxHeight = graphEnabled ? ScalePx(116) : ScalePx(72);
            var proportionalHeight = graphEnabled
                ? (int)Math.Round(infoView.ClientSize.Height * 0.22f)
                : (int)Math.Round(infoView.ClientSize.Height * 0.16f);

            return Math.Clamp(proportionalHeight, minHeight, maxHeight);
        }

        private int CalculateLongDescriptionHeight(int shortHeight, int dividerHeight, int damageHeight)
        {
            if (string.IsNullOrWhiteSpace(infoLDesc.Text))
            {
                return 0;
            }

            int minLongHeight = Math.Max(ScalePx(22), infoLDesc.Font.Height * InfoLongDescriptionMinLines + ScalePx(6));
            int desiredLongHeight = Math.Max(minLongHeight, infoLDesc.ContentHeight + ScalePx(4));
            int sliderHeight = sliderHost.Visible ? sliderHost.Height : 0;
            int preferredStatsHeight = CalculatePowerStatsPreferredHeight();

            int availableForLong = infoView.ClientSize.Height
                                   - shortHeight
                                   - dividerHeight
                                   - sliderHeight
                                   - damageHeight
                                   - preferredStatsHeight;

            if (availableForLong < minLongHeight)
            {
                return 0;
            }

            return Math.Min(desiredLongHeight, availableForLong);
        }

        private int CalculatePowerStatsPreferredHeight()
        {
            if (powerStatsGrid.Rows.Count == 0)
            {
                return 0;
            }

            double dpiScale = DeviceDpi / 96.0;
            int ScaleGridPx(int value) => Math.Max(1, (int)Math.Round(value * dpiScale));

            int gp = ScaleGridPx(powerStatsGrid.GridPadding);
            int hh = ScaleGridPx(powerStatsGrid.HeaderHeight);
            int rh = ScaleGridPx(powerStatsGrid.RowHeight);
            int visualRows = (powerStatsGrid.Rows.Count + 1) / 2;

            return gp + hh + (visualRows * rh) + gp;
        }

        private void ApplyShortDescriptionMargins()
        {
            if (infoSDesc == null || !infoSDesc.IsHandleCreated)
            {
                return;
            }

            var horizontalInset = Math.Max(0, ScalePx(ContentInset));
            var packedMargins = PackRichEditMargins(horizontalInset, horizontalInset);
            SendMessage(infoSDesc.Handle, EM_SETMARGINS, EC_LEFTMARGIN | EC_RIGHTMARGIN, packedMargins);

            var formatRect = new NativeRect
            {
                Left = horizontalInset,
                Top = 0,
                Right = Math.Max(horizontalInset + 1, infoSDesc.ClientSize.Width - horizontalInset),
                Bottom = Math.Max(1, infoSDesc.ClientSize.Height)
            };

            SendMessage(infoSDesc.Handle, EM_SETRECT, 0, ref formatRect);
            infoSDesc.Invalidate();
        }

        private static nint PackRichEditMargins(int left, int right)
        {
            left = Math.Clamp(left, 0, ushort.MaxValue);
            right = Math.Clamp(right, 0, ushort.MaxValue);
            return (nint)((right << 16) | left);
        }

        private int MeasureShortDescriptionHeight()
        {
            var text = infoSDesc.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            var contentWidth = Math.Max(1, infoSDesc.ClientSize.Width - infoSDesc.Padding.Horizontal - ScalePx(4));
            var lineHeight = TextRenderer.MeasureText("Ag", infoSDesc.Font, new Size(contentWidth, int.MaxValue),
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Height;
            var measured = TextRenderer.MeasureText(text, infoSDesc.Font, new Size(contentWidth, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Height;

            var minHeight = lineHeight + ScalePx(4);
            var maxHeight = lineHeight * InfoShortDescriptionMaxLines + ScalePx(4);
            return Math.Clamp(measured + ScalePx(4), minHeight, maxHeight);
        }

        private static void ApplyPairedListTheme(PairedListEx list, DataViewTheme theme)
        {
            list.ShowRuntimeSamples = false;
            list.ItemColor = theme.Muted;
            list.ValueColor = theme.Text;
            list.ValueAlternateColor = theme.GridBandLow;
            list.ValueConditionColor = theme.GridBandHigh;
            list.ValueSpecialColor = theme.GridBandMid;
            list.HighlightColor = theme.ChipActive;
            list.HighlightTextColor = theme.Text;
            list.Invalidate();
        }

        private void TotalViewScrollPanel_AvailableClientWidthChanged(object? sender, int availableWidth)
        {
            LayoutTotalsSections();
        }

        private void TotalViewContentPanel_SizeChanged(object? sender, EventArgs e)
        {
            LayoutTotalsSections();
        }

        private void LayoutTotalsSections()
        {
            if (_totalsStackHost == null || _quickReadSection == null || _defenseSection == null || _resistanceSection == null || _coreMiscSection == null)
            {
                return;
            }

            var outerInset = Math.Max(0, ScalePx(ContentInset));
            var gap = ScalePx(TotalsSectionGap);
            var width = Math.Max(1, totalViewScrollPanel.AvailableClientWidth - outerInset * 2);
            var y = outerInset;

            LayoutTotalsSection(_quickReadSection, width, outerInset, ref y, gap);
            LayoutTotalsSection(_defenseSection, width, outerInset, ref y, gap);
            LayoutTotalsSection(_resistanceSection, width, outerInset, ref y, gap);
            LayoutTotalsSection(_coreMiscSection, width, outerInset, ref y, gap);

            _totalsStackHost.Bounds = new Rectangle(0, 0, totalViewScrollPanel.ContentPanel.ClientSize.Width, y + outerInset);
        }

        private static void LayoutTotalsSection(MidsTotalsSectionPanel section, int width, int left, ref int y, int gap)
        {
            var preferred = section.GetPreferredSize(new Size(width, 0));
            section.Bounds = new Rectangle(left, y, width, preferred.Height);
            y = section.Bottom + gap;
        }

        #endregion

        #region Paint (Header)

        private void InfoDescDivider_Paint(object? sender, PaintEventArgs e)
        {
            var bounds = infoDescDivider.ClientRectangle;
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            var inset = ScalePx(ContentInset + 4);
            var y = bounds.Height / 2;

            using var pen = new Pen(CurrentTheme.GridRowLine);
            e.Graphics.DrawLine(
                pen,
                inset,
                y,
                Math.Max(inset, bounds.Width - inset),
                y);
        }

        private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
        {
            // DO NOT call base.OnPaint(e) here: this is an event handler, not an override.
            var g = e.Graphics;

            // Quality settings (cheap here, header is small)
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var hr = headerPanel.ClientRectangle;
            if (hr.Width <= 0 || hr.Height <= 0)
            {
                return;
            }

            var theme = CurrentTheme;
            using (var bottomPen = new Pen(theme.GridHeaderBorder))
            {
                g.DrawLine(bottomPen, hr.Left, hr.Bottom - 1, hr.Right, hr.Bottom - 1);
            }

            var visibleTabs = VisibleTabs;
            var tabRects = GetVisibleTabRects(visibleTabs);

            if (HeaderActionsVisible)
            {
                DrawHeaderActionWell(g, HeaderActionBounds(LockButton), theme, LockButton.ClientRectangle.Contains(LockButton.PointToClient(Cursor.Position)));
                DrawHeaderActionWell(g, HeaderActionBounds(DockButton), theme, DockButton.ClientRectangle.Contains(DockButton.PointToClient(Cursor.Position)));
            }

            // Draw tabs
            using var hoverBrush = new SolidBrush(Blend(theme.TabInactiveTop, theme.TabActiveTop, 0.24f));
            using var outlineColor = new SolidBrush(Color.Black); // for outline method
            using var font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);

            for (int i = 0; i < visibleTabs.Count; i++)
            {
                var rect = tabRects[i];
                var tab = visibleTabs[i];

                using var path = RoundedRect(rect, ScalePx(CornerRadius));

                if (tab.PageIndex == _selectedTabIndex)
                {
                    //using var selectedBrush = new SolidBrush(_tabSelectedColors[i]);
                    using var selectedBrush = new LinearGradientBrush(rect, theme.TabActiveTop, theme.TabActiveBottom, LinearGradientMode.Vertical);
                    g.FillPath(selectedBrush, path);
                }
                else if (i == _hoveredTabIndex)
                {
                    g.FillPath(hoverBrush, path);
                }
                else
                {
                    using var inactiveBrush = new LinearGradientBrush(rect, theme.TabInactiveTop, theme.TabInactiveBottom, LinearGradientMode.Vertical);
                    g.FillPath(inactiveBrush, path);
                }

                using var tabBorder = new Pen(tab.PageIndex == _selectedTabIndex ? Blend(theme.Border, theme.TabActiveTop, 0.45f) : theme.TabBorder);
                g.DrawPath(tabBorder, path);

                var textColor = tab.PageIndex == _selectedTabIndex ? theme.Text : Blend(theme.Muted, theme.Text, 0.24f);
                DrawTextWithOutline(g, tab.Title, font, rect, textColor, Color.Black);
            }
        }

        private void DrawHeaderActionWell(Graphics g, Rectangle bounds, DataViewTheme theme, bool hovered)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            using var path = RoundedRect(bounds, ScalePx(CornerRadius));
            var top = hovered ? Blend(theme.TabInactiveTop, theme.TabActiveTop, 0.28f) : theme.TabInactiveTop;
            var bottom = hovered ? Blend(theme.TabInactiveBottom, theme.TabActiveBottom, 0.22f) : theme.TabInactiveBottom;
            using var fill = new LinearGradientBrush(bounds, top, bottom, LinearGradientMode.Vertical);
            using var border = new Pen(theme.TabBorder);
            g.FillPath(fill, path);
            g.DrawPath(border, path);
        }

        private Rectangle[] GetVisibleTabRects(IReadOnlyList<TabDescriptor> visibleTabs)
        {
            if (visibleTabs.Count == 0)
            {
                return Array.Empty<Rectangle>();
            }

            var tabSpacing = ScalePx(TabSpacing);
            var outerInset = ScalePx(HeaderOuterInset);
            var actionWidth = HeaderActionsVisible ? DockButton.Width + LockButton.Width + tabSpacing : 0;
            var availableWidth = Math.Max(0, headerPanel.ClientSize.Width - actionWidth - (outerInset * 2) - (tabSpacing * Math.Max(0, visibleTabs.Count - 1)));
            var tabTop = Math.Max(ScalePx(2), (headerPanel.ClientSize.Height - ScalePx(TabHeight)) / 2);

            using var font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            return ComputeTabRects(availableWidth, visibleTabs, font, new Point(outerInset, tabTop), ScalePx(TabHeight), tabSpacing, ScalePx(TabPaddingX));
        }

        private static Rectangle[] ComputeTabRects(int totalWidth, IReadOnlyList<TabDescriptor> tabs, Font font, Point origin, int height, int spacing, int paddingX)
        {
            if (tabs.Count == 0)
            {
                return Array.Empty<Rectangle>();
            }

            var preferredWidths = new int[tabs.Count];
            int preferredTotal = 0;

            for (int i = 0; i < tabs.Count; i++)
            {
                int measuredWidth = TextRenderer.MeasureText(
                    tabs[i].Title,
                    font,
                    new Size(int.MaxValue, height),
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix).Width;
                preferredWidths[i] = Math.Max(height, measuredWidth + paddingX);
                preferredTotal += preferredWidths[i];
            }

            var allocatedWidths = new int[tabs.Count];
            if (preferredTotal <= totalWidth)
            {
                Array.Copy(preferredWidths, allocatedWidths, tabs.Count);

                int extra = totalWidth - preferredTotal;
                int extraPerTab = extra / tabs.Count;
                int extraRemainder = extra % tabs.Count;
                for (int i = 0; i < tabs.Count; i++)
                {
                    allocatedWidths[i] += extraPerTab + (i < extraRemainder ? 1 : 0);
                }
            }
            else
            {
                double scale = preferredTotal > 0 ? (double)totalWidth / preferredTotal : 1d;
                var fractional = new (int Index, double Fraction)[tabs.Count];
                int allocatedTotal = 0;

                for (int i = 0; i < tabs.Count; i++)
                {
                    double scaledWidth = preferredWidths[i] * scale;
                    int width = Math.Max(height, (int)Math.Floor(scaledWidth));
                    allocatedWidths[i] = width;
                    allocatedTotal += width;
                    fractional[i] = (i, scaledWidth - width);
                }

                int remainder = Math.Max(0, totalWidth - allocatedTotal);
                foreach (var candidate in fractional.OrderByDescending(item => item.Fraction).ThenBy(item => item.Index))
                {
                    if (remainder <= 0)
                    {
                        break;
                    }

                    allocatedWidths[candidate.Index]++;
                    remainder--;
                }
            }

            var rects = new Rectangle[tabs.Count];
            int x = origin.X;

            for (int i = 0; i < tabs.Count; i++)
            {
                rects[i] = new Rectangle(x, origin.Y, allocatedWidths[i], height);
                x += allocatedWidths[i] + spacing;
            }

            return rects;
        }

        private static void DrawTextWithOutline(Graphics g, string text, Font font, Rectangle bounds, Color foreColor, Color outlineColor)
        {
            var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

            // simple 1px outline
            const int off = 1;
            for (int dx = -off; dx <= off; dx++)
            {
                for (int dy = -off; dy <= off; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    var shadowRect = new Rectangle(bounds.X + dx, bounds.Y + dy, bounds.Width, bounds.Height);
                    TextRenderer.DrawText(g, text, font, shadowRect, outlineColor, flags);
                }
            }

            TextRenderer.DrawText(g, text, font, bounds, foreColor, flags);
        }

        private static Color Blend(Color first, Color second, float amountSecond)
        {
            amountSecond = Math.Clamp(amountSecond, 0f, 1f);
            var amountFirst = 1f - amountSecond;

            return Color.FromArgb(
                255,
                (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
                (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
                (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
        }

        #endregion

        #region Mouse (Header)

        private void HeaderPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            // Ignore hover when over either right-side button
            if (HeaderActionsVisible && (DockButton.Bounds.Contains(e.Location) || LockButton.Bounds.Contains(e.Location)))
            {
                if (_hoveredTabIndex != -1)
                {
                    _hoveredTabIndex = -1;
                    headerPanel.Invalidate();
                }
                return;
            }

            var visibleTabs = VisibleTabs;
            var tabRects = GetVisibleTabRects(visibleTabs);

            int newHovered = -1;
            for (int i = 0; i < tabRects.Length; i++)
            {
                if (tabRects[i].Contains(e.Location))
                {
                    newHovered = i;
                    break;
                }
            }

            if (newHovered != _hoveredTabIndex)
            {
                _hoveredTabIndex = newHovered;
                headerPanel.Invalidate();
            }
        }

        private void HeaderPanel_MouseLeave(object? sender, EventArgs e)
        {
            if (_hoveredTabIndex != -1)
            {
                _hoveredTabIndex = -1;
                headerPanel.Invalidate();
            }
        }

        private void HeaderPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            if (_hoveredTabIndex >= 0 && _hoveredTabIndex != _selectedTabIndex)
            {
                SelectTab(_hoveredTabIndex);
            }
        }

        #endregion

        #region Public API

        public void SetPresentationMode(MidsDataViewNeoPresentationMode mode)
        {
            if (_presentationMode == mode)
            {
                return;
            }

            _presentationMode = mode;
            ApplyPresentationMode();
        }

        public void SelectTab(int index)
        {
            var tabs = VisibleTabs;
            if (index < 0 || index >= tabs.Count)
            {
                return;
            }

            var actualPageIndex = tabs[index].PageIndex;
            if (actualPageIndex == _selectedTabIndex)
                return;

            if (_selectedTabIndex != actualPageIndex)
            {
                _selectedTabIndex = actualPageIndex;
                dvPages.SelectedIndex = _selectedTabIndex;
                headerPanel.Invalidate();
                RefreshSelectedTabHeader();
                TabChanged?.Invoke(this, _selectedTabIndex);
            }
        }

        private void RefreshSelectedTabHeader()
        {
            if (titlePanel != null)
            {
                titlePanel.Visible = _selectedTabIndex != 2;
            }

            if (_selectedTabIndex == 2)
            {
                title.Text = string.Empty;
                subTitle.Text = string.Empty;
                DisplayTotals();
                return;
            }

            if (_selectedTabIndex == 3)
            {
                DisplayEdFigures();
                return;
            }

            if (_selectedTabIndex == 4)
            {
                DisplayBonuses();
                return;
            }

            if (pBase != null)
            {
                DisplayInfo();
            }
        }

        private void ApplyPresentationMode()
        {
            var actorReadOnly = _presentationMode == MidsDataViewNeoPresentationMode.ActorReadOnly;
            DockButton.Visible = !actorReadOnly;
            LockButton.Visible = !actorReadOnly;
            if (actorReadOnly)
            {
                procToggle.Visible = false;
            }

            if ((actorReadOnly && _selectedTabIndex == 3) || (!actorReadOnly && _selectedTabIndex == 4))
            {
                _selectedTabIndex = 0;
                dvPages.SelectedIndex = 0;
            }

            headerPanel.Invalidate();
            RefreshSelectedTabHeader();
        }

        public void SetData(IPower? basePower, IPower? enhancedPower, bool noLevel = false, bool locked = false, int iHistoryIdx = -1)
        {
            if (basePower == null)
            {
                if (!_isLocked)
                {
                    Clear();
                }

                return;
            }

            SetLock(locked, false);

            PowerDisplaySnapshot snapshot;
            if (_presentationMode != MidsDataViewNeoPresentationMode.ActorReadOnly &&
                MainModule.MidsController.Toon != null)
            {
                snapshot = MainModule.MidsController.Toon.GetDisplayPowerSnapshot(iHistoryIdx, basePower.PowerIndex);
            }
            else
            {
                snapshot = new PowerDisplaySnapshot(
                    new Power(basePower),
                    enhancedPower == null ? new Power(basePower) { PowerIndex = -1 } : new Power(enhancedPower),
                    string.IsNullOrEmpty(Power.GetRootPowerName(iHistoryIdx, basePower, enhancedPower))
                        ? null
                        : DatabaseAPI.GetPowerByFullName(Power.GetRootPowerName(iHistoryIdx, basePower, enhancedPower)),
                    string.IsNullOrEmpty(Power.GetRootPowerName(iHistoryIdx, basePower, enhancedPower))
                        ? null
                        : MainModule.MidsController.Toon?.GetEnhancedPower(iHistoryIdx),
                    iHistoryIdx,
                    false,
                    false,
                    false,
                    false)
                {
                    ActorCalculationSnapshot = _presentationMode == MidsDataViewNeoPresentationMode.ActorReadOnly
                        ? _actorCalculationSnapshot
                        : MainModule.MidsController.Toon?.LastCalculationSnapshot?.PlayerActorSnapshot,
                    PowerCalculationSnapshot = _presentationMode == MidsDataViewNeoPresentationMode.ActorReadOnly
                        ? (_actorCalculationSnapshot?.PowerSnapshots.Count > iHistoryIdx && iHistoryIdx >= 0
                            ? _actorCalculationSnapshot.PowerSnapshots[iHistoryIdx]
                            : null)
                        : (MainModule.MidsController.Toon?.LastCalculationSnapshot?.PowerSnapshots.Count > iHistoryIdx && iHistoryIdx >= 0
                            ? MainModule.MidsController.Toon.LastCalculationSnapshot.PowerSnapshots[iHistoryIdx]
                            : null),
                    ContributionSnapshot = _presentationMode == MidsDataViewNeoPresentationMode.ActorReadOnly
                        ? _actorCalculationSnapshot?.Contributions
                        : MainModule.MidsController.Toon?.LastCalculationSnapshot?.PlayerActorSnapshot.Contributions
                };
            }

            SetData(snapshot, noLevel, locked);
        }

        public void SetActorData(
            IPower? basePower,
            IPower? enhancedPower,
            string actorClassName,
            ActorTotalsSnapshot? actorTotals,
            int iHistoryIdx = -1,
            string? powerSourceDescription = null,
            IReadOnlyList<PetAppliedBonusEntry>? appliedBonuses = null)
        {
            SetActorDataInternal(basePower, enhancedPower, actorClassName, actorTotals, iHistoryIdx, powerSourceDescription, appliedBonuses, null);
        }

        internal void SetActorDataInternal(
            IPower? basePower,
            IPower? enhancedPower,
            string actorClassName,
            ActorTotalsSnapshot? actorTotals,
            int iHistoryIdx,
            string? powerSourceDescription,
            IReadOnlyList<PetAppliedBonusEntry>? appliedBonuses,
            ActorCalculationSnapshot? actorCalculationSnapshot)
        {
            SetPresentationMode(MidsDataViewNeoPresentationMode.ActorReadOnly);
            _actorTotalsSnapshot = actorTotals;
            _actorCalculationSnapshot = actorCalculationSnapshot;
            _actorPowerSourceDescription = powerSourceDescription;
            _actorAppliedBonuses = appliedBonuses ?? [];

            if (basePower == null)
            {
                Clear();
                return;
            }

            var baseClone = new Power(basePower)
            {
                OmniDisplayClassName = actorClassName
            };
            var enhancedClone = enhancedPower == null
                ? new Power(basePower)
                {
                    PowerIndex = -1,
                    OmniDisplayClassName = actorClassName
                }
                : new Power(enhancedPower)
                {
                    OmniDisplayClassName = actorClassName
                };

            var snapshot = new PowerDisplaySnapshot(
                baseClone,
                enhancedClone,
                string.IsNullOrEmpty(Power.GetRootPowerName(iHistoryIdx, baseClone, enhancedClone))
                    ? null
                    : DatabaseAPI.GetPowerByFullName(Power.GetRootPowerName(iHistoryIdx, baseClone, enhancedClone)),
                null,
                iHistoryIdx,
                false,
                false,
                false,
                false)
            {
                ActorCalculationSnapshot = actorCalculationSnapshot,
                PowerCalculationSnapshot = actorCalculationSnapshot?.PowerSnapshots.Count > iHistoryIdx && iHistoryIdx >= 0
                    ? actorCalculationSnapshot.PowerSnapshots[iHistoryIdx]
                    : null,
                ContributionSnapshot = actorCalculationSnapshot?.Contributions
            };
            SetData(snapshot, false, false);
        }

        private int GetDisplayedBaseHitPoints()
        {
            if (_presentationMode == MidsDataViewNeoPresentationMode.ActorReadOnly && _actorTotalsSnapshot != null)
            {
                return DatabaseAPI.GetClassHitPoints(_actorTotalsSnapshot.ClassName);
            }

            return DatabaseAPI.GetClassHitPoints(MidsContext.Archetype);
        }

        public void SetData(PowerDisplaySnapshot snapshot, bool noLevel = false, bool locked = false)
        {
            SetLock(locked, false);
            if (_presentationMode != MidsDataViewNeoPresentationMode.ActorReadOnly)
            {
                _actorTotalsSnapshot = null;
                _actorCalculationSnapshot = null;
                _displayContributions = null;
                _actorPowerSourceDescription = null;
                _actorAppliedBonuses = [];
            }

            _actorCalculationSnapshot = snapshot.ActorCalculationSnapshot;
            _displayContributions = snapshot.ContributionSnapshot ?? snapshot.ActorCalculationSnapshot?.Contributions;

            pBase = snapshot.BasePower == null ? null : new Power(snapshot.BasePower);
            pEnh = snapshot.EnhancedPower == null ? null : new Power(snapshot.EnhancedPower);
            _effectsComparisonBase = snapshot.PowerCalculationSnapshot?.BasePower == null
                ? (pBase == null ? null : new Power(pBase))
                : new Power(snapshot.PowerCalculationSnapshot.BasePower);
            rootPowerBase = snapshot.RootPowerBase;
            rootPowerEnh = snapshot.RootPowerEnh;
            HistoryIDX = snapshot.HistoryIndex;

            if (pBase == null)
            {
                if (!_isLocked)
                {
                    Clear();
                }

                return;
            }

            if (pEnh == null)
            {
                pEnh = new Power(pBase)
                {
                    PowerIndex = -1
                };
            }

            GroupedRankedEffects = GroupedFx.AssembleGroupedEffects(pEnh);
            GroupedRankedEffects = GroupedFx.AggregateGroupedEffectsPass2(pEnh, GroupedRankedEffects);

            // Compare effect rows against the raw base power, not the assembled display base.
            // The assembled base already contains absorbed local enhancement effects, which
            // makes host-power enhancement rows appear unchanged in the Neo effects grid.
            var effectComparisonBase = snapshot.PowerCalculationSnapshot?.BasePower ?? pBase;
            EffectsItemPairs = GroupedFx.GenerateListItems(
                GroupedRankedEffects,
                effectComparisonBase ?? pBase,
                pEnh,
                pEnh.GetRankedEffects(true).ToList(),
                infoDataList.Font.Size);

            SetDamageTip();
            DisplayData(noLevel);
        }

        public void Clear()
        {
            pBase = null;
            pEnh = null;
            _effectsComparisonBase = null;
            rootPowerBase = null;
            rootPowerEnh = null;
            _actorTotalsSnapshot = null;
            _actorCalculationSnapshot = null;
            _actorPowerSourceDescription = null;
            _actorAppliedBonuses = [];
            _bonusesGrid?.Clear();
            HistoryIDX = -1;
            GroupedRankedEffects.Clear();
            EffectsItemPairs.Clear();

            title.Text = string.Empty;
            subTitle.Text = string.Empty;
            infoSDesc.Clear();
            infoLDesc.Text = string.Empty;
            UpdateInfoDescriptionLayout();
            powerStatsGrid.Clear();
            effectsGrid.Clear();
            enhDataList.Clear(true);
            coreDataList.Clear(true);
            sliderHost.Visible = false;
            infoDamageDisplay.Clear();

            ClearEnhancementPanels();
        }

        public void SetEnhancement(I9Slot iEnh, int iLevel = -1)
        {
            if ((_isLocked & _selectedTabIndex != 3) || iLevel < 0)
            {
                return;
            }

            string str1;
            if (iEnh.Enh > -1)
            {
                var enhancement = DatabaseAPI.Database.Enhancements[iEnh.Enh];
                str1 = enhancement.TypeID == Enums.eType.Normal
                    ? DatabaseAPI.GetEnhancementDisplayName(iEnh, includeFlavor: true)
                    : enhancement.LongName;
                if (enhancement.TypeID != Enums.eType.Normal && str1.Length > 38 & iLevel > -1)
                {
                    str1 = DatabaseAPI.GetEnhancementNameShortWSet(iEnh.Enh);
                }
            }
            else
            {
                str1 = pBase?.DisplayName ?? string.Empty;
                if (pBase != null)
                {
                    infoSDesc.Rtf = RTF.StartRTF(infoSDesc.Font) + pBase.DescShort + "\r\n" +
                                     RTF.Color(RTF.ElementID.Faded) +
                                     "Shift+Click to move slot. Right-Click to place enh." + RTF.EndRTF();
                    UpdateInfoDescriptionLayout();
                }
            }

            if (iLevel > -1 & !MidsContext.Config.ShowSlotLevels)
            {
                str1 += $" (Slot Level {iLevel + 1})";
            }

            title.Text = str1;
            if (_selectedTabIndex > 1 || iEnh.Enh < 0)
            {
                UpdateInfoDescriptionLayout();
                return;
            }

            var iStr1 = string.Empty;
            var effectPrefixRtf = string.Empty;
            if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.InventO | DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO)
            {
                iStr1 = $"{RTF.Color(RTF.ElementID.Invention)}Invention Level: {iEnh.IOLevel + 1}{Enums.GetRelativeString(iEnh.RelativeLevel, false)}{RTF.Color(RTF.ElementID.Text)}";
            }

            switch (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID)
            {
                case Enums.eType.SetO:
                    if (DatabaseAPI.Database.Enhancements[iEnh.Enh].Unique)
                    {
                        iStr1 += $"{RTF.Color(RTF.ElementID.Warning)} (Unique) {RTF.Color(RTF.ElementID.Text)}";
                    }

                    if (DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance is < 1 and > 0)
                    {

                        effectPrefixRtf += $"{RTF.Color(RTF.ElementID.Enhancement)}{DisplayValueFormatter.FormatPercentFromScale(DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance, 2)}% chance of ";
                    }

                    break;

                case Enums.eType.SpecialO:
                    iStr1 += RTF.Color(RTF.ElementID.Enhancement) + "Hamidon/Synthetic Hamidon Origin Enhancement";
                    break;

                default:
                    if (iStr1 != string.Empty)
                    {
                        iStr1 += " - ";
                    }

                    iStr1 += GetEnhancementStringRtf(iEnh);
                    break;
            }

            string iStr2;
            if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO)
            {
                iStr2 = effectPrefixRtf + GetEnhancementStringLongRtf(iEnh) + RTF.Crlf() + EnhancementSetCollection.GetSetInfoLongRTF(DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet);
            }
            else
            {
                var str3 = DatabaseAPI.Database.Enhancements[iEnh.Enh].Desc;
                if (!string.IsNullOrEmpty(str3))
                {
                    str3 = RTF.ToRTF(str3) + RTF.Crlf();
                }

                iStr2 = effectPrefixRtf + str3 + GetEnhancementStringLongRtf(iEnh);
            }

            infoSDesc.Rtf = RTF.StartRTF(infoSDesc.Font) + iStr1 + RTF.Crlf() +
                             RTF.Color(RTF.ElementID.Faded) +
                             "Shift+Click to move slot. Right-Click to place enh." + RTF.EndRTF();
            infoLDesc.Rtf = RTF.StartRTF(infoLDesc.Font) + iStr2 + RTF.EndRTF();
            UpdateInfoDescriptionLayout();
        }

        public void SetEnhancementPicker(I9Slot iEnh)
        {
            if (iEnh.Enh < 0)
            {
                title.Text = "No Enhancement";
                infoSDesc.Clear();
                infoLDesc.Text = string.Empty;
                UpdateInfoDescriptionLayout();
                return;
            }

            title.Text = DatabaseAPI.GetEnhancementDisplayName(iEnh, includeFlavor: true);

            var effectPrefixRtf = string.Empty;
            var iStr1 = string.Empty;
            if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID is Enums.eType.InventO or Enums.eType.SetO)
            {
                iStr1 = $"{RTF.Color(RTF.ElementID.Invention)}Invention Level: {iEnh.IOLevel + 1}{Enums.GetRelativeString(iEnh.RelativeLevel, false)}{RTF.Color(RTF.ElementID.Text)}";
            }


            switch (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID)
            {
                case Enums.eType.SetO:
                    if (DatabaseAPI.Database.Enhancements[iEnh.Enh].Unique)
                    {
                        iStr1 += $"{RTF.Color(RTF.ElementID.Warning)} (Unique) {RTF.Color(RTF.ElementID.Text)}";
                    }

                    if (DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance is < 1 and > 0)
                    {
                        effectPrefixRtf +=
                            $"{RTF.Color(RTF.ElementID.Enhancement)}{DisplayValueFormatter.FormatPercentFromScale(DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance, 2)}% chance of ";
                    }

                    break;

                case Enums.eType.SpecialO:
                    iStr1 += RTF.Color(RTF.ElementID.Enhancement) + "Hamidon/Synthetic Hamidon Origin Enhancement" + RTF.Color(RTF.ElementID.Text);
                    break;

                default:
                    if (iStr1 != string.Empty)
                    {
                        iStr1 += " - ";
                    }

                    iStr1 += GetEnhancementStringRtf(iEnh);
                    break;
            }

            string iStr2;
            if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO)
            {
                // Fix strange white "-2" showing at the end of the enhancement long text
                /*iStr2 = str1 + GetEnhancementStringLongRTF(iEnh) + RTF.Size(RTF.SizeID.Tiny) + "\r\n" +
                        EnhancementSetCollection.GetSetInfoLongRTF(DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet);*/

                iStr2 = effectPrefixRtf + GetEnhancementStringLongRtf(iEnh) + RTF.Crlf() +
                        EnhancementSetCollection.GetSetInfoLongRTF(DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet);
            }
            else
            {
                var str2 = DatabaseAPI.Database.Enhancements[iEnh.Enh].Desc;
                if (!string.IsNullOrEmpty(str2))
                {
                    str2 = RTF.ToRTF(str2) + RTF.Crlf();
                }

                iStr2 = effectPrefixRtf + str2 + GetEnhancementStringLongRtf(iEnh);
            }

            infoSDesc.Rtf = RTF.StartRTF(infoSDesc.Font) + iStr1 + RTF.Crlf() + RTF.EndRTF();
            infoLDesc.Rtf = RTF.StartRTF(infoLDesc.Font) + iStr2 + RTF.EndRTF();
            UpdateInfoDescriptionLayout();
        }

        public void DisplayTotals()
        {
            var actorMode = _presentationMode == MidsDataViewNeoPresentationMode.ActorReadOnly && _actorTotalsSnapshot != null;
            if ((!actorMode && MidsContext.Character == null) ||
                _quickReadStrip == null ||
                _defenseBarListLeft == null ||
                _defenseBarListRight == null ||
                _resistanceBarListLeft == null ||
                _resistanceBarListRight == null ||
                _coreMiscGrid == null ||
                _defenseSection == null ||
                _resistanceSection == null)
            {
                return;
            }

            var actorDisplayStats = actorMode ? _actorTotalsSnapshot!.DisplayStats : null;
            var totals = actorMode ? _actorTotalsSnapshot!.Totals : MidsContext.Character.Totals;
            var totalsCapped = actorMode ? _actorTotalsSnapshot!.TotalsCapped : MidsContext.Character.TotalsCapped;
            string FormatPercentValue(float value, int maxDecimal = 2) => $"{DisplayValueFormatter.FormatPercentValue(value, maxDecimal)}%";
            string FormatPercentScale(float value, int maxDecimal = 2) => $"{DisplayValueFormatter.FormatPercentFromScale(value, maxDecimal)}%";
            string FormatSignedPercentValue(float value, int maxDecimal = 0) =>
                $"{(value > 0 ? "+" : string.Empty)}{DisplayValueFormatter.FormatPercentValue(value, maxDecimal)}%";
            var resistanceCapValue = actorMode
                ? DatabaseAPI.GetClassResistanceCap(_actorTotalsSnapshot!.ClassName)
                : DatabaseAPI.GetClassResistanceCap(MidsContext.Character.Archetype);
            float GetDefense(int damageType) => actorMode ? actorDisplayStats!.Defense(damageType) : MidsContext.Character.DisplayStats.Defense(damageType);
            float GetResistance(int damageType, bool uncapped) => actorMode
                ? actorDisplayStats!.DamageResistance(damageType, uncapped)
                : MidsContext.Character.DisplayStats.DamageResistance(damageType, uncapped);
            float GetRecoveryPct() => actorMode ? actorDisplayStats!.EnduranceRecoveryPercentage(false) : MidsContext.Character.DisplayStats.EnduranceRecoveryPercentage(false);
            float GetRecoveryNumeric() => actorMode ? actorDisplayStats!.EnduranceRecoveryNumeric : MidsContext.Character.DisplayStats.EnduranceRecoveryNumeric;
            float GetEndUsage() => actorMode ? actorDisplayStats!.EnduranceUsage : MidsContext.Character.DisplayStats.EnduranceUsage;
            float GetEndTimeToFull() => actorMode ? actorDisplayStats!.EnduranceTimeToFull : MidsContext.Character.DisplayStats.EnduranceTimeToFull;
            float GetEndRecoveryNet() => actorMode ? actorDisplayStats!.EnduranceRecoveryNet : MidsContext.Character.DisplayStats.EnduranceRecoveryNet;
            float GetEndRecoveryLossNet() => actorMode ? actorDisplayStats!.EnduranceRecoveryLossNet : MidsContext.Character.DisplayStats.EnduranceRecoveryLossNet;
            float GetEndTimeToZero() => actorMode ? actorDisplayStats!.EnduranceTimeToZero : MidsContext.Character.DisplayStats.EnduranceTimeToZero;
            float GetEndTimeToFullNet() => actorMode ? actorDisplayStats!.EnduranceTimeToFullNet : MidsContext.Character.DisplayStats.EnduranceTimeToFullNet;
            float GetHealthRegenTimeToFull() => actorMode ? actorDisplayStats!.HealthRegenTimeToFull : MidsContext.Character.DisplayStats.HealthRegenTimeToFull;
            float GetHealthRegenPercent() => actorMode ? actorDisplayStats!.HealthRegenPercent(false) : MidsContext.Character.DisplayStats.HealthRegenPercent(false);
            float GetHealthRegenHealthPerSec() => actorMode ? actorDisplayStats!.HealthRegenHealthPerSec : MidsContext.Character.DisplayStats.HealthRegenHealthPerSec;
            float GetHealthRegenHpPerSec() => actorMode ? actorDisplayStats!.HealthRegenHPPerSec(false) : MidsContext.Character.DisplayStats.HealthRegenHPPerSec;
            float GetBuffToHit() => actorMode ? actorDisplayStats!.BuffToHit : MidsContext.Character.DisplayStats.BuffToHit;
            float GetBuffAccuracy() => actorMode ? actorDisplayStats!.BuffAccuracy : MidsContext.Character.DisplayStats.BuffAccuracy;
            float GetBuffDamage() => actorMode ? actorDisplayStats!.BuffDamage(false) : MidsContext.Character.DisplayStats.BuffDamage(false);
            float GetBuffEndRdx() => actorMode ? actorDisplayStats!.BuffEndRdx : MidsContext.Character.DisplayStats.BuffEndRdx;
            float GetBuffHaste() => actorMode ? actorDisplayStats!.BuffHaste(false) : MidsContext.Character.DisplayStats.BuffHaste(false);
            float GetRangePercent() => actorMode ? actorDisplayStats!.RangePercent : MidsContext.Character.DisplayStats.RangePercent;
            float GetThreatLevel() => actorMode ? actorDisplayStats!.ThreatLevel : MidsContext.Character.DisplayStats.ThreatLevel;
            var iTip1 = string.Empty;
            var iTip2 = $"Time to go from 0-100% end: {DisplayValueFormatter.FormatSeconds(GetEndTimeToFull())}s.\r\nHover the mouse over the End Drain stats for more info.";
            switch (GetEndRecoveryNet())
            {
                case > 0:
                    {
                        iTip1 = $"Net Endurance Gain (Recovery - Drain): {DisplayValueFormatter.FormatRate(GetEndRecoveryNet())}/s.";
                        if (Math.Abs(GetEndRecoveryNet() - GetRecoveryNumeric()) > float.Epsilon)
                        {
                            iTip1 += $"\r\nTime to go from 0-100% end (using net gain): {DisplayValueFormatter.FormatSeconds(GetEndTimeToFullNet())}s.";
                        }

                        break;
                    }
                case < 0:
                    iTip1 = $"With current end drain, you will lose end at a rate of: {DisplayValueFormatter.FormatRate(GetEndRecoveryLossNet())}/s.\r\nFrom 100% you would run out of end in: {DisplayValueFormatter.FormatSeconds(GetEndTimeToZero())}s.";
                    break;
            }

            var iTip3 = $"Time to go from 0-100% health: {DisplayValueFormatter.FormatSeconds(GetHealthRegenTimeToFull())}s.\r\nHealth regenerated per second: {FormatPercentValue(GetHealthRegenHealthPerSec())}\r\nHitPoints regenerated per second at level 50: {DisplayValueFormatter.FormatRate(GetHealthRegenHpPerSec())} HP";
            var damageNames = Enum.GetNames(typeof(Enums.eDamage));
            var defenseLeft = BuildBarMetrics(
                [
                    Enums.eDamage.Smashing,
                    Enums.eDamage.Lethal,
                    Enums.eDamage.Energy,
                    Enums.eDamage.Negative,
                    Enums.eDamage.Toxic,
                    Enums.eDamage.Psionic
                ],
                true,
                damage =>
                {
                    var damageIndex = (int)damage;
                    return (
                        Math.Max(0, GetDefense(damageIndex)),
                        45f,
                        $"{FormatPercentValue(GetDefense(damageIndex))} {damageNames[damageIndex]} defense");
                },
                FormatPercentValue,
                45f);

            if (!DatabaseAPI.RealmUsesToxicDef())
            {
                defenseLeft = defenseLeft.Where(metric => metric.Label != nameof(Enums.eDamage.Toxic)).ToList();
            }

            var defenseRight = BuildBarMetrics(
                [
                    Enums.eDamage.Fire,
                    Enums.eDamage.Cold,
                    Enums.eDamage.Melee,
                    Enums.eDamage.Ranged,
                    Enums.eDamage.AoE
                ],
                true,
                damage =>
                {
                    var damageIndex = (int)damage;
                    return (
                        Math.Max(0, GetDefense(damageIndex)),
                        45f,
                        $"{FormatPercentValue(GetDefense(damageIndex))} {damageNames[damageIndex]} defense");
                },
                FormatPercentValue,
                45f);

            var resistanceLeft = BuildBarMetrics(
                [
                    Enums.eDamage.Smashing,
                    Enums.eDamage.Lethal,
                    Enums.eDamage.Energy,
                    Enums.eDamage.Negative
                ],
                false,
                damage =>
                {
                    var damageIndex = (int)damage;
                    var uncapped = Math.Max(0, GetResistance(damageIndex, true));
                    var capped = Math.Max(0, GetResistance(damageIndex, false));
                    var tooltip = totalsCapped.Res[damageIndex] < totals.Res[damageIndex]
                        ? $"{FormatPercentValue(uncapped)} {damageNames[damageIndex]} resistance capped at {FormatPercentValue(capped)}"
                        : $"{FormatPercentValue(uncapped)} {damageNames[damageIndex]} resistance. (cap: {FormatPercentScale(resistanceCapValue, 0)})";
                    return (capped, resistanceCapValue * 100f, tooltip);
                },
                FormatPercentValue,
                resistanceCapValue * 100f);

            var resistanceRight = BuildBarMetrics(
                [
                    Enums.eDamage.Fire,
                    Enums.eDamage.Cold,
                    Enums.eDamage.Toxic,
                    Enums.eDamage.Psionic
                ],
                false,
                damage =>
                {
                    var damageIndex = (int)damage;
                    var uncapped = Math.Max(0, GetResistance(damageIndex, true));
                    var capped = Math.Max(0, GetResistance(damageIndex, false));
                    var tooltip = totalsCapped.Res[damageIndex] < totals.Res[damageIndex]
                        ? $"{FormatPercentValue(uncapped)} {damageNames[damageIndex]} resistance capped at {FormatPercentValue(capped)}"
                        : $"{FormatPercentValue(uncapped)} {damageNames[damageIndex]} resistance. (cap: {FormatPercentScale(resistanceCapValue, 0)})";
                    return (capped, resistanceCapValue * 100f, tooltip);
                },
                FormatPercentValue,
                resistanceCapValue * 100f);

            _defenseSection.MetaText = "softcap: 45%";
            _resistanceSection.MetaText = $"cap: {FormatPercentScale(resistanceCapValue, 0)}";

            _quickReadStrip.SetMetrics(
            [
                new TotalsQuickMetric("Recharge", FormatSignedPercentValue(GetBuffHaste() - 100, 0), null, "The recharge time of this actor's powers is being altered by this effect.\r\nThe higher the value, the faster the recharge.", MidsTotalsGlyph.QuickRecharge, Color.FromArgb(187, 111, 255)),
                new TotalsQuickMetric("Recovery", FormatPercentValue(GetRecoveryPct(), 0), null, iTip2, MidsTotalsGlyph.QuickRecovery, Color.FromArgb(135, 200, 255)),
                new TotalsQuickMetric("Regen", FormatPercentValue(GetHealthRegenPercent(), 0), null, iTip3, MidsTotalsGlyph.QuickRegen, Color.FromArgb(154, 222, 100)),
                new TotalsQuickMetric("End Drain", $"{DisplayValueFormatter.FormatRate(GetEndUsage())}/s", null, iTip1, MidsTotalsGlyph.QuickEndDrain, Color.FromArgb(255, 185, 96))
            ]);

            _defenseBarListLeft.ScaleMax = 100f;
            _defenseBarListRight.ScaleMax = 100f;
            _resistanceBarListLeft.ScaleMax = 100f;
            _resistanceBarListRight.ScaleMax = 100f;
            _defenseBarListLeft.SetMetrics(defenseLeft);
            _defenseBarListRight.SetMetrics(defenseRight);
            _resistanceBarListLeft.SetMetrics(resistanceLeft);
            _resistanceBarListRight.SetMetrics(resistanceRight);

            _coreMiscGrid.SetMetrics(
            [
                new TotalsValueMetric("To Hit", FormatPercentValue(GetBuffToHit(), 0), "This effect is increasing the accuracy of all powers on this actor.", MidsTotalsGlyph.StatToHit, Color.FromArgb(210, 210, 210)),
                new TotalsValueMetric("Accuracy", FormatSignedPercentValue(GetBuffAccuracy(), 0), "This effect is increasing the accuracy scale of this actor's powers.", MidsTotalsGlyph.StatAccuracy, Color.FromArgb(210, 210, 210)),
                new TotalsValueMetric("Damage", FormatSignedPercentValue(GetBuffDamage() - 100, 0), "This effect is modifying the outgoing damage of this actor's attack powers.", MidsTotalsGlyph.StatDamage, Color.FromArgb(255, 190, 70)),
                new TotalsValueMetric("EndRdx", FormatPercentValue(GetBuffEndRdx(), 0), "The end cost of all powers on this actor is being reduced by this effect.\r\nThis is applied like an end-reduction enhancement.", MidsTotalsGlyph.StatEndRdx, Color.FromArgb(125, 185, 255)),
                new TotalsValueMetric("Range", FormatSignedPercentValue(GetRangePercent(), 0), "This effect is modifying the range of this actor's powers.", MidsTotalsGlyph.StatRange, Color.FromArgb(210, 210, 210)),
                new TotalsValueMetric("Threat", FormatPercentValue(GetThreatLevel(), 0), "This shows the actor's current threat modifier.", MidsTotalsGlyph.StatThreat, Color.FromArgb(255, 120, 90))
            ]);

            LayoutTotalsSections();

            List<TotalsBarMetric> BuildBarMetrics(
                IReadOnlyList<Enums.eDamage> damages,
                bool defense,
                Func<Enums.eDamage, (float value, float marker, string tooltip)> getter,
                Func<float, int, string> formatter,
                float defaultMarker)
            {
                var list = new List<TotalsBarMetric>(damages.Count);
                foreach (var damage in damages)
                {
                    var (value, marker, tooltip) = getter(damage);
                    var (icon, color) = GetDamageIcon(damage, defense);
                    list.Add(new TotalsBarMetric(
                        GetDamageDisplayName(damage),
                        formatter(value, 1),
                        value,
                        marker <= 0 ? defaultMarker : marker,
                        tooltip,
                        icon,
                        color));
                }

                return list;
            }
        }

        private static string GetDamageDisplayName(Enums.eDamage damage) => damage switch
        {
            Enums.eDamage.AoE => "AoE",
            _ => damage.ToString()
        };

        private static (MidsTotalsGlyph icon, Color color) GetDamageIcon(Enums.eDamage damage, bool defense)
        {
            return damage switch
            {
                Enums.eDamage.Smashing => (MidsTotalsGlyph.DamageSmashing, Color.FromArgb(184, 154, 255)),
                Enums.eDamage.Lethal => (MidsTotalsGlyph.DamageLethal, Color.FromArgb(234, 234, 234)),
                Enums.eDamage.Fire => (MidsTotalsGlyph.DamageFire, Color.FromArgb(255, 118, 77)),
                Enums.eDamage.Cold => (MidsTotalsGlyph.DamageCold, Color.FromArgb(170, 230, 255)),
                Enums.eDamage.Energy => (MidsTotalsGlyph.DamageEnergy, Color.FromArgb(170, 205, 255)),
                Enums.eDamage.Negative => (MidsTotalsGlyph.DamageNegative, Color.FromArgb(196, 136, 255)),
                Enums.eDamage.Toxic => (MidsTotalsGlyph.DamageToxic, Color.FromArgb(170, 245, 112)),
                Enums.eDamage.Psionic => (MidsTotalsGlyph.DamagePsionic, Color.FromArgb(176, 132, 255)),
                Enums.eDamage.Melee => (MidsTotalsGlyph.DamageMelee, Color.FromArgb(225, 208, 190)),
                Enums.eDamage.Ranged => (MidsTotalsGlyph.DamageRanged, Color.FromArgb(205, 175, 255)),
                Enums.eDamage.AoE => (MidsTotalsGlyph.DamageAoE, Color.FromArgb(255, 160, 112)),
                _ => (defense ? MidsTotalsGlyph.SectionDefense : MidsTotalsGlyph.SectionResistance, Color.FromArgb(190, 200, 255))
            };
        }

        public void DisplayBonuses()
        {
            if (_bonusesGrid == null)
            {
                return;
            }

            _bonusesGrid.Clear();
            if (_presentationMode != MidsDataViewNeoPresentationMode.ActorReadOnly)
            {
                return;
            }

            if (_actorAppliedBonuses.Count == 0)
            {
                _bonusesGrid.SetGroups(new[]
                {
                    new PowerEffectsGrid.Group(
                        "Applied Bonuses",
                        new PowerEffectsGrid.Row[]
                        {
                            new PowerEffectsGrid.DescriptorRow(
                                "Status",
                                "None",
                                "No named actor bonuses are currently active.")
                        })
                });
                return;
            }

            var orderedBonuses = _actorAppliedBonuses
                .OrderBy(item => item.SourceType)
                .ThenBy(item => item.SourceName, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var groups = new List<PowerEffectsGrid.Group>(orderedBonuses.Length);
            foreach (var entry in orderedBonuses)
            {
                var rows = entry.StatDeltas
                    .Select(delta => new PowerEffectsGrid.DescriptorRow(
                        delta.StatName,
                        "Bonus",
                        delta.Delta >= 0 ? $"+{delta.Delta:0.##}{delta.Suffix}" : $"{delta.Delta:0.##}{delta.Suffix}",
                        entry.Tooltip))
                    .Cast<PowerEffectsGrid.Row>()
                    .ToArray();
                groups.Add(new PowerEffectsGrid.Group(entry.SourceName, rows));
            }

            _bonusesGrid.SetGroups(groups);
        }

        public void FlipStage(int Index, int Enh1, int Enh2, float State, int PowerID, Enums.eEnhGrade Grade1, Enums.eEnhGrade Grade2)
        {
            if (pBase == null)
            {
                if (_selectedTabIndex == 3)
                {
                    title.Text = string.Empty;
                }

                subTitle.Text = string.Empty;
                ClearEnhancementPanels();
                enhDataList.Clear(true);
                return;
            }

            EnsureFlipBuffer();
            if (bxFlip?.Graphics == null)
            {
                return;
            }

            using var solidBrush1 = new SolidBrush(enhDataList.BackColor);
            using var solidBrush2 = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            if (!MatchesDisplayedPower(PowerID))
            {
                return;
            }

            var rectangle1 = new Rectangle();
            ref var local1 = ref rectangle1;
            var size = bxFlip.Size;
            var x = size.Width - 188 + 30 * Index;
            size = bxFlip.Size;
            var y1 = (int)Math.Round((size.Height / 2.0 - 30.0) / 2.0);
            local1 = new Rectangle(x, y1, 30, 30);
            var destRect = rectangle1;
            bxFlip.Graphics.FillRectangle(solidBrush1, rectangle1);
            var rectangle2 = new Rectangle((int)Math.Round(rectangle1.X + (30.0 - 30.0 * State) / 2.0), rectangle1.Y,
                (int)Math.Round(30.0 * State), 30);
            Graphics graphics;
            if (Enh1 > -1)
            {
                graphics = bxFlip.Graphics;
                AssetManager.DrawFlippingEnhancement(graphics, rectangle1, State,
                    DatabaseAPI.Database.Enhancements[Enh1].ImageIdx,
                    AssetManager.ToGfxGrade(DatabaseAPI.Database.Enhancements[Enh1].TypeID, Grade1));
            }
            else
            {
                bxFlip.Graphics.DrawImage(AssetManager.EmptySlot.Bitmap, rectangle2);
            }

            pnlEnhActive.Invalidate(destRect);
            ref var local2 = ref rectangle1;
            double y2 = rectangle1.Y;
            size = bxFlip.Size;
            var num1 = size.Height / 2.0;
            var num2 = (int)Math.Round(y2 + num1);
            local2.Y = num2;
            bxFlip.Graphics.FillRectangle(solidBrush1, rectangle1);
            rectangle2 = new Rectangle((int)Math.Round(rectangle1.X + (30.0 - 30.0 * State) / 2.0), rectangle1.Y,
                (int)Math.Round(30.0 * State), 30);
            if (Enh2 > -1)
            {
                graphics = bxFlip.Graphics;
                AssetManager.DrawFlippingEnhancement(graphics, rectangle1, State,
                    DatabaseAPI.Database.Enhancements[Enh2].ImageIdx,
                    AssetManager.ToGfxGrade(DatabaseAPI.Database.Enhancements[Enh2].TypeID, Grade2));
            }
            else
            {
                bxFlip.Graphics.DrawImage(AssetManager.EmptySlot.Bitmap, rectangle2);
            }

            rectangle2.Inflate(2, 2);
            bxFlip.Graphics.FillEllipse(solidBrush2, rectangle2);
            pnlEnhInactive.Invalidate(destRect);
        }

        public void SetSetPicker(int iSet)
        {
            if (iSet < 0)
            {
                title.Text = "No Enhancement";
                infoSDesc.Clear();
                infoLDesc.Text = string.Empty;
                UpdateInfoDescriptionLayout();
            }
            else
            {
                title.Text = DatabaseAPI.Database.EnhancementSets[iSet].DisplayName;
                var str1 = DatabaseAPI.GetSetTypeByIndex(DatabaseAPI.Database.EnhancementSets[iSet].SetType).Name;

                var str2 = DatabaseAPI.Database.EnhancementSets[iSet].LevelMin !=
                           DatabaseAPI.Database.EnhancementSets[iSet].LevelMax
                    ? $"{DatabaseAPI.Database.EnhancementSets[iSet].LevelMin + 1} to {DatabaseAPI.Database.EnhancementSets[iSet].LevelMax + 1}"
                    : $"{DatabaseAPI.Database.EnhancementSets[iSet].LevelMin + 1}";
                infoSDesc.Rtf = RTF.StartRTF(infoSDesc.Font) + RTF.ToRTF($"{str1}, levels {str2}") + RTF.EndRTF();
                infoLDesc.Rtf = $"{RTF.StartRTF(infoLDesc.Font)}{EnhancementSetCollection.GetSetInfoLongRTF(iSet)}{RTF.EndRTF()}";
                UpdateInfoDescriptionLayout();
            }
        }

        public void SetGraphType(Enums.MDmgGraphType graphType, Enums.MDmgDisplayStyle graphStyle)
        {
            infoDamageDisplay.ShowGraph = graphStyle != Enums.MDmgDisplayStyle.TextOnly;
            UpdateInfoDescriptionLayout();
        }

        #endregion

        #region Private Methods

        private void DvPages_SelectedIndexChanged(object? sender, int pageIndex)
        {
            // Reflect FormPages selection into header state and raise external event
            if (pageIndex < 0 || pageIndex >= _allTabs.Length)
                return;

            if (_selectedTabIndex != pageIndex)
            {
                _selectedTabIndex = pageIndex;
                headerPanel.Invalidate();
                TabChanged?.Invoke(this, pageIndex);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();

            if (bounds.Width <= 0 || bounds.Height <= 0)
                return path; // empty, safe

            // Clamp radius to fit within the rectangle
            int r = Math.Max(0, Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2));
            if (r == 0)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            int d = r * 2;

            // careful with Right/Bottom – subtract diameter to avoid negative sizes
            path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static string FormatActorClassName(string className)
        {
            return string.IsNullOrWhiteSpace(className)
                ? "Actor"
                : className.Replace('_', ' ').Trim();
        }

        private static PairedListEx.Item BuildEdItem(int index, float[] value, Enums.eSchedule[] schedule, string name, float[] afterEd)
        {
            var flag1 = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][0];
            var flag2 = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][1];
            var iSpecialCase = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][2];
            PairedListEx.Item itemPair;
            if (value[index] < 0)
            {
                itemPair = new PairedListEx.Item(string.Empty, string.Empty, false, false, false, string.Empty);
            }
            else
            {
                var iName = $"{name}:";
                var num1 = value[index] * 100f;
                var num2 = Enhancement.ApplyED(schedule[index], value[index]) * 100f;
                var num3 = num2 + afterEd[index] * 100f;
                var num4 = (float)Math.Round(num1 - (double)num2, 3);

                var str1 = $"{DisplayValueFormatter.FormatPercentValue(num1, 2)} %";
                var str2 = $"{DisplayValueFormatter.FormatPercentValue(num4, 2)} %";
                var str3 = $"{DisplayValueFormatter.FormatPercentValue(num3, 2)} %";
                var str4 = $"Total Effect: {DisplayValueFormatter.FormatPercentValue(num1 + afterEd[index] * 100, 2)}%\r\nWith ED Applied: {str3}\r\n\r\n";
                string iValue;
                string iTip;
                if (num4 > 0)
                {
                    iValue = $"{str3} (Pre-ED: {DisplayValueFormatter.FormatPercentValue(num1 + afterEd[index] * 100, 2)}%)";
                    if (afterEd[index] > 0)
                    {
                        str4 += $"Amount from pre-ED sources: {str1}\r\n";
                    }

                    iTip = $"{str4} ED reduction: {str2} ({DisplayValueFormatter.FormatPercentValue(num4 / (double)num1 * 100, 2)}% of total)\r\n";
                    if (iSpecialCase)
                    {
                        iTip = $"{iTip} The highest level of ED reduction is being applied.\r\nThreshold: {DisplayValueFormatter.FormatPercentValue(DatabaseAPI.Database.MultED[(int)schedule[index]][2] * 100, 2)} %\r\n";
                    }
                    else if (flag2)
                    {
                        iTip = $"{iTip} The middle level of ED reduction is being applied.\r\nThreshold: {DisplayValueFormatter.FormatPercentValue(DatabaseAPI.Database.MultED[(int)schedule[index]][1] * 100, 2)} %\r\n";
                    }
                    else if (flag1)
                    {
                        iTip = $"{iTip} The lowest level of ED reduction is being applied.\r\nThreshold: {DisplayValueFormatter.FormatPercentValue(DatabaseAPI.Database.MultED[(int)schedule[index]][0] * 100, 2)} %\r\n";
                    }

                    if (afterEd[index] > 0)
                    {
                        iTip = $"{iTip} Amount from post-ED sources: {DisplayValueFormatter.FormatPercentValue(afterEd[index] * 100, 2)} %\r\n";
                    }
                }
                else
                {
                    iValue = str3;
                    if (afterEd[index] > 0)
                    {
                        str4 = $"{str4} Amount from post-ED sources: {DisplayValueFormatter.FormatPercentValue(afterEd[index] * 100, 2)} %\r\n";
                    }

                    iTip = $"{str4}This effect has not been affected by ED.\r\n";
                }

                itemPair = new PairedListEx.Item(iName, iValue, flag2 & !iSpecialCase, flag1 & !flag2, iSpecialCase, iTip);
            }

            return itemPair;
        }

        private void DisplayEdFigures()
        {
            if (pBase == null)
            {
                if (_selectedTabIndex == 3)
                {
                    title.Text = string.Empty;
                }

                subTitle.Text = string.Empty;
                enhDataList.Clear(true);
                ClearEnhancementPanels();
                return;
            }

            title.Text = pBase.DisplayName;
            enhDataList.Clear();
            if (MidsContext.Character == null)
            {
                if (_selectedTabIndex == 3)
                {
                    title.Text = string.Empty;
                }

                subTitle.Text = string.Empty;
                ClearEnhancementPanels();
                enhDataList.Redraw();

                return;
            }

            var buildHistoryIdx = ResolveDisplayedBuildHistoryIndex(allowPowerLookup: false);
            if (buildHistoryIdx < 0)
            {
                if (_selectedTabIndex == 3)
                {
                    title.Text = string.Empty;
                }

                subTitle.Text = string.Empty;
                ClearEnhancementPanels();
                enhDataList.Redraw();

                return;
            }

            var eEnhs = Enum.GetValues(typeof(Enums.eEnhance)).Length;
            var buffs = new float[eEnhs];
            var debuffs = new float[eEnhs];
            var buffDebuffs = new float[eEnhs];
            var buffsSchedule = new Enums.eSchedule[eEnhs];
            var debuffsSchedule = new Enums.eSchedule[eEnhs];
            var buffsDebuffsSchedule = new Enums.eSchedule[eEnhs];
            var buffsAfterEd = new float[eEnhs];
            var debuffsAfterEd = new float[eEnhs];
            var buffsDebuffsAfterEd = new float[eEnhs];
            var mezBuffs = new float[Enum.GetValues(typeof(Enums.eMez)).Length];
            var mezSchedule = new Enums.eSchedule[eEnhs];
            var mezAfterED = new float[eEnhs];

            Array.Fill(buffs, 0);
            Array.Fill(debuffs, 0);
            Array.Fill(buffDebuffs, 0);
            Array.Fill(mezBuffs, 0);

            for (var i = 0; i < buffs.Length; i++)
            {
                buffsSchedule[i] = Enhancement.GetSchedule((Enums.eEnhance)i);
                debuffsSchedule[i] = buffsSchedule[i];
                buffsDebuffsSchedule[i] = buffsSchedule[i];
            }

            debuffsSchedule[(int)Enums.eEnhance.Defense] = Enums.eSchedule.A; // 3
            for (var tSub = 0; tSub < mezBuffs.Length; tSub++)
            {
                mezSchedule[tSub] = Enhancement.GetSchedule(Enums.eEnhance.Mez, tSub);
            }

            var buildPower = MidsContext.Character.CurrentBuild.Powers[buildHistoryIdx];
            if (!HasSlottedEnhancements(buildPower))
            {
                if (_selectedTabIndex == 3)
                {
                    title.Text = string.Empty;
                }

                subTitle.Text = string.Empty;
                ClearEnhancementPanels();
                enhDataList.Redraw();
                return;
            }

            subTitle.Text = "Enhancement Values";

            for (var i = 0; i < buildPower?.SlotCount; i++)
            {
                var slot = buildPower.Slots[i];
                if (slot.Enhancement.Enh <= -1)
                {
                    continue;
                }

                var slotEnh = slot.Enhancement.Enh;
                for (var se = 0; se < DatabaseAPI.Database.Enhancements[slotEnh].Effect.Length; se++)
                {
                    var effect = DatabaseAPI.Database.Enhancements[slotEnh].Effect;
                    if (effect[se].Mode != Enums.eEffMode.Enhancement)
                    {
                        continue;
                    }

                    if (effect[se].Enhance.ID == 12)
                    {
                        mezBuffs[effect[se].Enhance.SubID] += slot.Enhancement.GetEnhancementEffect(Enums.eEnhance.Mez, effect[se].Enhance.SubID, 1);
                    }
                    else
                    {
                        switch (DatabaseAPI.Database.Enhancements[slotEnh].Effect[se].BuffMode)
                        {
                            case Enums.eBuffDebuff.BuffOnly:
                                buffs[effect[se].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[se].Enhance.ID, -1, 1);
                                break;

                            case Enums.eBuffDebuff.DeBuffOnly:
                                if (effect[se].Enhance.ID is not 6 and not 11 and not 19)
                                {
                                    debuffs[effect[se].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[se].Enhance.ID, -1, -1);
                                }

                                break;
                            default:
                                buffDebuffs[effect[se].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[se].Enhance.ID, -1, 1);
                                break;
                        }
                    }
                }
            }

            foreach (var p in MidsContext.Character.CurrentBuild.Powers)
            {
                if (p == null)
                {
                    continue;
                }

                if (!p.StatInclude)
                {
                    continue;
                }

                IPower power1 = PlannerEffectResolver.ResolvePower(new Power(p.Power), new PlannerEffectResolutionContext
                {
                    AbsorbPetEffects = true
                }).ResolvedPower;
                foreach (var effect in power1.Effects)
                {
                    if (power1.PowerType != Enums.ePowerType.GlobalBoost & (!effect.Absorbed_Effect | effect.Absorbed_PowerType != Enums.ePowerType.GlobalBoost))
                    {
                        continue;
                    }

                    if (effect.Absorbed_Effect & effect.Absorbed_Power_nID > -1)
                    {
                        power1 = DatabaseAPI.Database.Power[effect.Absorbed_Power_nID];
                    }

                    var eBuffDebuff = Enums.eBuffDebuff.Any;
                    var flag = false;
                    if (MidsContext.Character.CurrentBuild.Powers[buildHistoryIdx] == null)
                    {
                        continue;
                    }

                    foreach (var b in buildPower.Power.BoostsAllowed)
                    {
                        if (power1 != null && power1.BoostsAllowed.Any(e => b == e))
                        {
                            if (b.Contains("Buff"))
                            {
                                eBuffDebuff = Enums.eBuffDebuff.BuffOnly;
                            }

                            if (b.Contains("Debuff"))
                            {
                                eBuffDebuff = Enums.eBuffDebuff.DeBuffOnly;
                            }

                            flag = true;
                        }

                        if (flag)
                        {
                            break;
                        }
                    }

                    if (!flag)
                    {
                        continue;
                    }

                    switch (effect.EffectType)
                    {
                        case Enums.eEffectType.Enhancement:
                            switch (effect.ETModifies)
                            {
                                case Enums.eEffectType.Defense:
                                    if (effect.DamageType == Enums.eDamage.Smashing)
                                    {
                                        if (effect.IgnoreED)
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    buffsAfterEd[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;

                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    debuffsAfterEd[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;

                                                default:
                                                    buffsDebuffsAfterEd[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    buffs[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;

                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    debuffs[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;

                                                default:
                                                    buffDebuffs[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case Enums.eEffectType.Mez:
                                    if (effect.IgnoreED)
                                    {
                                        mezAfterED[(int)effect.MezType] += effect.BuffedMag;
                                        break;
                                    }

                                    mezBuffs[(int)effect.MezType] += effect.BuffedMag;
                                    break;

                                default:
                                    var rechargeBuffIndex = effect.ETModifies != Enums.eEffectType.RechargeTime
                                        ? Convert.ToInt32(Enum.Parse(typeof(Enums.eEnhance), effect.ETModifies.ToString()))
                                        : (int)Enums.eEnhance.RechargeTime; // 14
                                    if (effect.IgnoreED)
                                    {
                                        buffsDebuffsAfterEd[rechargeBuffIndex] += effect.BuffedMag;
                                        break;
                                    }

                                    buffDebuffs[rechargeBuffIndex] += effect.BuffedMag;
                                    break;
                            }

                            break;
                        default:
                            {
                                if (effect.EffectType == Enums.eEffectType.DamageBuff & effect.DamageType == Enums.eDamage.Smashing)
                                {
                                    switch (effect.IgnoreED)
                                    {
                                        case true:
                                            {
                                                foreach (var b in power1?.BoostsAllowed)
                                                {
                                                    if (b.StartsWith("Res_Damage"))
                                                    {
                                                        buffsDebuffsAfterEd[(int)Enums.eEnhance.Resistance] += effect.BuffedMag; // 18
                                                        break;
                                                    }

                                                    if (!b.StartsWith("Damage"))
                                                    {
                                                        continue;
                                                    }

                                                    buffsDebuffsAfterEd[(int)Enums.eEnhance.Damage] += effect.BuffedMag; // 2
                                                    break;
                                                }

                                                break;
                                            }
                                        default:
                                            {
                                                foreach (var b in power1?.BoostsAllowed)
                                                {
                                                    if (b.StartsWith("Res_Damage"))
                                                    {
                                                        buffDebuffs[(int)Enums.eEnhance.Resistance] += effect.BuffedMag;
                                                        break;
                                                    }

                                                    if (!b.StartsWith("Damage"))
                                                    {
                                                        continue;
                                                    }

                                                    buffDebuffs[(int)Enums.eEnhance.Damage] += effect.BuffedMag;
                                                    break;
                                                }

                                                break;
                                            }
                                    }
                                }

                                break;
                            }
                    }
                }
            }

            var zeroedEnhanceBuffs = new[]
            {
                Enums.eEnhance.HitPoints, Enums.eEnhance.Regeneration, Enums.eEnhance.Recovery // 8, 16, 17
            }.Cast<int>();

            foreach (var buff in zeroedEnhanceBuffs)
            {
                buffs[buff] = 0;
                debuffs[buff] = 0;
                buffDebuffs[buff] = 0;
            }

            var liBuffsDebuffs = new List<List<PairedListEx.Item>>
            {
                new(),
                new(),
                new()
            };

            for (var i = 0; i < buffs.Length; i++)
            {
                if (buffs[i] > 0)
                {
                    liBuffsDebuffs[0].Add(BuildEdItem(i, buffs, buffsSchedule, Enum.GetName(typeof(Enums.eEnhance), i), buffsAfterEd));
                }

                if (debuffs[i] > 0)
                {
                    liBuffsDebuffs[1].Add(BuildEdItem(i, debuffs, debuffsSchedule, $"{Enum.GetName(typeof(Enums.eEnhance), i)} Debuff", debuffsAfterEd));
                }

                if (buffDebuffs[i] > 0)
                {
                    liBuffsDebuffs[2].Add(BuildEdItem(i, buffDebuffs, buffsDebuffsSchedule, Enum.GetName(typeof(Enums.eEnhance), i), buffsDebuffsAfterEd));
                }
            }

            for (var i = 0; i < liBuffsDebuffs.Count; i++)
            {
                if (liBuffsDebuffs[i].Count <= 0)
                {
                    continue;
                }

                var slowIdx = liBuffsDebuffs[i].TryFindIndex(e => e.Name is "Slow:");
                if (slowIdx >= 0 & slowIdx <= liBuffsDebuffs.Count)
                {
                    var slowValue = liBuffsDebuffs[i][slowIdx].Value;
                    liBuffsDebuffs[i] = liBuffsDebuffs[i]
                        .Where(e => !(e.Name is "SpeedFlying:" or "SpeedJumping:" or "SpeedRunning:" & (e.Value != null && e.Value == slowValue)))
                        .ToList();
                }

                for (var j = 0; j < liBuffsDebuffs[i].Count; j++)
                {
                    enhDataList.AddItem(liBuffsDebuffs[i][j]);
                    if (enhDataList.IsSpecialColor())
                    {
                        enhDataList.SetUnique();
                    }
                }
            }

            enhDataList.Redraw();
            DisplayFlippedEnhancements();
        }

        private static bool HasEnhancementCarrier(IPower p, Enums.eEffectType modifies)
        {
            if (p?.Effects == null || p.Effects.Length == 0) return false;
            for (int i = 0; i < p.Effects.Length; i++)
                if (p.Effects[i].EffectType == Enums.eEffectType.Enhancement &&
                    p.Effects[i].ETModifies == modifies)
                    return true;
            return false;
        }

        private static bool HasSlottedEnhancements(PowerEntry? power)
        {
            if (power?.Slots == null)
            {
                return false;
            }

            for (var i = 0; i < power.SlotCount; i++)
            {
                if (power.Slots[i].Enhancement.Enh > -1 || power.Slots[i].FlippedEnhancement.Enh > -1)
                {
                    return true;
                }
            }

            return false;
        }

        private static (bool HasAny, bool HasNonProc, bool HasProc) GetActiveSlotDamageComposition(PowerEntry? power)
        {
            if (power?.Slots == null)
            {
                return (false, false, false);
            }

            var hasAny = false;
            var hasNonProc = false;
            var hasProc = false;

            for (var i = 0; i < power.SlotCount; i++)
            {
                var enhancementId = power.Slots[i].Enhancement.Enh;
                if (enhancementId <= -1)
                {
                    continue;
                }

                hasAny = true;

                var enhancement = DatabaseAPI.Database.Enhancements[enhancementId];
                if (EnhancementProcRules.IsProcToggleEligible(enhancement))
                {
                    hasProc = true;
                }
                else
                {
                    hasNonProc = true;
                }
            }

            return (hasAny, hasNonProc, hasProc);
        }

        private PowerEntry? GetDisplayedBuildPowerEntry()
        {
            var build = MidsContext.Character?.CurrentBuild;
            if (build == null)
            {
                return null;
            }

            var historyIndex = ResolveDisplayedBuildHistoryIndex();
            if (historyIndex < 0 || historyIndex >= build.Powers.Count)
            {
                return null;
            }

            return build.Powers[historyIndex];
        }

        private int ResolveDisplayedBuildHistoryIndex(bool allowPowerLookup = true)
        {
            var build = MidsContext.Character?.CurrentBuild;
            if (build == null)
            {
                return -1;
            }

            if (HistoryIDX >= 0 && HistoryIDX < build.Powers.Count && build.Powers[HistoryIDX] != null)
            {
                return HistoryIDX;
            }

            if (!allowPowerLookup)
            {
                return -1;
            }

            var powerBase = rootPowerBase ?? pBase;
            return powerBase == null ? -1 : build.FindInToonHistory(powerBase.PowerIndex);
        }

        private bool MatchesDisplayedPower(int powerId)
        {
            if (powerId < 0)
            {
                return false;
            }

            if (pBase?.PowerIndex == powerId || rootPowerBase?.PowerIndex == powerId)
            {
                return true;
            }

            var build = MidsContext.Character?.CurrentBuild;
            var historyIdx = ResolveDisplayedBuildHistoryIndex();
            return build != null &&
                   historyIdx >= 0 &&
                   historyIdx < build.Powers.Count &&
                   build.Powers[historyIdx]?.NIDPower == powerId;
        }

        private List<PowerStatsGrid.Row> BuildCanonicalStatRows(IPower pBase, IPower pEnh)
        {
            const double eps = 1e-6;
            var rows = new List<PowerStatsGrid.Row>();
            bool isToggle = pBase.PowerType == Enums.ePowerType.Toggle;
            bool isAuto = pBase.PowerType == Enums.ePowerType.Auto_;

            // --- Shape: Arc/Radius + Range ---
            if (pBase.Arc > eps)
            {
                rows.Add(new PowerStatsGrid.Row(
                    "Arc",
                    pBase.Arc,
                    pEnh.Arc,
                    "°",
                    higherIsBetter: false,
                    tooltip: EnhancementPolicyAxes.GetStatPolicyTooltip(pBase, "Arc")));
                if (pBase.Range > eps || pEnh.Range > eps)
                    rows.Add(new PowerStatsGrid.Row(
                        "Range",
                        pBase.Range,
                        pEnh.Range,
                        "ft",
                        higherIsBetter: true,
                        tooltip: EnhancementPolicyAxes.GetStatPolicyTooltip(pBase, "Range")));
            }
            else if (pBase.Radius > eps)
            {
                rows.Add(new PowerStatsGrid.Row(
                    "Radius",
                    pBase.Radius,
                    pEnh.Radius,
                    "ft",
                    higherIsBetter: true,
                    tooltip: EnhancementPolicyAxes.GetStatPolicyTooltip(pBase, "Radius")));
                if (pBase.Range > eps || pEnh.Range > eps)
                    rows.Add(new PowerStatsGrid.Row(
                        "Range",
                        pBase.Range,
                        pEnh.Range,
                        "ft",
                        higherIsBetter: true,
                        tooltip: EnhancementPolicyAxes.GetStatPolicyTooltip(pBase, "Range")));
            }
            else if (pBase.Range > eps || pEnh.Range > eps)
            {
                rows.Add(new PowerStatsGrid.Row(
                    "Range",
                    pBase.Range,
                    pEnh.Range,
                    "ft",
                    higherIsBetter: true,
                    tooltip: EnhancementPolicyAxes.GetStatPolicyTooltip(pBase, "Range")));
            }

            // --- Timing ---
            if (!isAuto)
            {
                if (!isToggle && (pBase.CastTime > eps || pEnh.CastTime > eps))
                    rows.Add(new PowerStatsGrid.Row("Cast Time", pBase.CastTime, pEnh.CastTime, "s", higherIsBetter: false,
                        tooltip: $"CastTime: {pEnh.CastTimeBase:0.###}s\nArcana CastTime: {pEnh.ArcanaCastTime:0.###}s"));

                if (!isToggle && (pBase.InterruptTime > eps || pEnh.InterruptTime > eps))
                    rows.Add(new PowerStatsGrid.Row(
                        "Interrupt",
                        pBase.InterruptTime,
                        pEnh.InterruptTime,
                        "s",
                        higherIsBetter: false,
                        tooltip: EnhancementPolicyAxes.GetStatPolicyTooltip(pBase, "Interrupt")));

                if (isToggle && (pBase.ActivatePeriod > eps || pEnh.ActivatePeriod > eps))
                    rows.Add(new PowerStatsGrid.Row("Activate", pBase.ActivatePeriod, pEnh.ActivatePeriod, "s", higherIsBetter: false,
                        tooltip: "The effects of this toggle are applied at this interval."));
            }

            // Recharge (lower is better)
            if (pBase.RechargeTime > eps || pEnh.RechargeTime > eps)
                rows.Add(new PowerStatsGrid.Row("Recharge", pBase.RechargeTime, pEnh.RechargeTime, "s", higherIsBetter: false,
                    affectedByEd: HasEnhancementCarrier(pEnh, Enums.eEffectType.RechargeTime)));

            // Endurance cost
            if (isToggle)
            {
                if (pBase.ToggleCost > eps || pEnh.ToggleCost > eps)
                    rows.Add(new PowerStatsGrid.Row("End Cost", pBase.ToggleCost, pEnh.ToggleCost, "/s", higherIsBetter: false,
                        affectedByEd: HasEnhancementCarrier(pEnh, Enums.eEffectType.EnduranceDiscount),
                        tooltip: "Per-second endurance upkeep for a running toggle."));
            }
            else
            {
                if (pBase.EndCost > eps || pEnh.EndCost > eps)
                    rows.Add(new PowerStatsGrid.Row("End Cost", pBase.EndCost, pEnh.EndCost, "End", higherIsBetter: false,
                        affectedByEd: HasEnhancementCarrier(pEnh, Enums.eEffectType.EnduranceDiscount)));
            }

            // Accuracy (Real Numbers style multiplier, higher is better)
            double baseAcc = MidsContext.Config.ScalingToHit * pBase.Accuracy;
            double enhAcc = MidsContext.Config.ScalingToHit * pEnh.Accuracy;

            // Show Accuracy only if autohit caveats or a to-hit check is present (matches prior UI logic)
            bool requiresToHit = pBase.Effects.Any(t => t.RequiresToHitCheck);
            bool entitiesAutoHit = pBase.EntitiesAutoHit == Enums.eEntity.None ||
                pBase.Effects.Where(e => e.EffectType == Enums.eEffectType.EntCreate)
                             .SelectMany(e =>
                             {
                                 var ent = DatabaseAPI.Database.Entities.ElementAtOrDefault(e.nSummon);
                                 if (ent == null) return [];
                                 var nps = ent.GetNPowerset();
                                 if (nps.Count == 0) return [];
                                 var psidx = nps[0];
                                 var ps = DatabaseAPI.Database.Powersets.ElementAtOrDefault(psidx);
                                 return ps?.Powers ?? [];
                             })
                             .Any(p => p?.EntitiesAutoHit == Enums.eEntity.None);

            bool showAcc = entitiesAutoHit || requiresToHit || pBase.Range > 20 ||
                           pBase.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt);

            if (showAcc && (baseAcc > eps || enhAcc > eps))
            {
                var star = (pBase.EntitiesAutoHit != Enums.eEntity.None && requiresToHit) ? "*" : "";
                var tip = $"Accuracy multiplier without other buffs (Real Numbers style): {pBase.AccuracyMult:0.00}x" +
                          (star.Length > 0 ? "\n* Autohit power with at least one effect that requires a ToHit roll." : "");
                rows.Add(new PowerStatsGrid.Row($"Accuracy (per activation){star}",
                    baseAcc, enhAcc, "", higherIsBetter: true,
                    affectedByEd: HasEnhancementCarrier(pEnh, Enums.eEffectType.Accuracy), tooltip: tip));
            }

            // Optional: Duration when discoverable (e.g., mez duration as a canonical timing stat)
            var durId = pBase.GetDurationEffectID();
            if (durId > -1 &&
                pBase.Effects[durId].Duration <= 9999 &&
                pEnh.Effects[durId].Duration <= 9999)
            {
                var d1 = pBase.Effects[durId].Duration;
                var d2 = pEnh.Effects[durId].Duration;
                if (d1 <= float.Epsilon && d2 > float.Epsilon)
                {
                    d1 = d2;
                }

                if (Math.Max(d1, d2) > float.Epsilon)
                {
                rows.Add(new PowerStatsGrid.Row("Duration", d1, d2, "s", higherIsBetter: true));
                }
            }

            return rows;
        }

        private DamageCardPresentation BuildDamageCardPresentation(IPower basePower, IPower enhancedPower)
        {
            var baseSummary = Power.GetDamageBreakdown(basePower, basePower.PowerIndex > -1 && enhancedPower.PowerIndex > -1);
            var enhancedSummary = Power.GetDamageBreakdown(enhancedPower);
            var historyIndex = ResolveDisplayedBuildHistoryIndex();
            var powerSnapshot = TryResolveDisplayedPowerCalculationSnapshot(historyIndex);
            var buildPowerEntry = GetDisplayedBuildPowerEntry();
            var slotComposition = GetActiveSlotDamageComposition(buildPowerEntry);
            var procsExcludedForPower = buildPowerEntry?.ProcInclude == true;
            var canUseBuildSlotComposition = buildPowerEntry != null;

            if (_presentationMode == MidsDataViewNeoPresentationMode.ActorReadOnly && _actorTotalsSnapshot != null)
            {
                var actorHpMax = Math.Max(1f, _actorTotalsSnapshot.Totals.HPMax);
                baseSummary = baseSummary.ScaleToDisplayMultiplier(actorHpMax);
                enhancedSummary = enhancedSummary.ScaleToDisplayMultiplier(actorHpMax);
            }

            if (!baseSummary.HasDamageEffects && !enhancedSummary.HasDamageEffects)
            {
                return new DamageCardPresentation(
                    HeaderText: string.Empty,
                    ModeBadgeText: "Non-Damage",
                    PrimaryText: string.Empty,
                    SubtitleText: string.Empty,
                    TooltipText: string.Empty,
                    Segments: Array.Empty<DamageSourceSegment>());
            }

            if (basePower.NIDSubPower.Length > 0 &&
                baseSummary.DisplayedTotal <= float.Epsilon &&
                enhancedSummary.DisplayedTotal <= float.Epsilon)
            {
                return DamageCardPresentation.Empty;
            }

            var noProcCurrent = Math.Max(0f, enhancedSummary.TotalExcludingProc);
            var currentValue = Math.Max(0f, enhancedSummary.DisplayedTotal);
            var rawBaseValue = Math.Max(0f, baseSummary.DisplayedTotal);

            float baseValue;
            float enhancedValue;
            float procValue;

            if (!canUseBuildSlotComposition)
            {
                baseValue = rawBaseValue;
                enhancedValue = Math.Max(0f, noProcCurrent - baseValue);
                procValue = Math.Max(0f, currentValue - noProcCurrent);
            }
            else if (!slotComposition.HasAny)
            {
                baseValue = currentValue;
                enhancedValue = 0f;
                procValue = 0f;
            }
            else if (!slotComposition.HasNonProc)
            {
                baseValue = noProcCurrent;
                enhancedValue = 0f;
                procValue = procsExcludedForPower ? 0f : Math.Max(0f, currentValue - noProcCurrent);
            }
            else
            {
                baseValue = rawBaseValue;
                enhancedValue = Math.Max(0f, noProcCurrent - baseValue);
                procValue = procsExcludedForPower ? 0f : Math.Max(0f, currentValue - noProcCurrent);
            }

            var segments = new List<DamageSourceSegment>(3);
            if (baseValue > float.Epsilon)
            {
                segments.Add(new DamageSourceSegment(DamageSourceSegmentKind.Base, "Base", baseValue));
            }

            if (enhancedValue > float.Epsilon)
            {
                segments.Add(new DamageSourceSegment(DamageSourceSegmentKind.Enhanced, "Enhanced", enhancedValue));
            }

            if (procValue > float.Epsilon)
            {
                segments.Add(new DamageSourceSegment(DamageSourceSegmentKind.Proc, "Proc", procValue));
            }

            return new DamageCardPresentation(
                HeaderText: "Damage",
                ModeBadgeText: GetDamageModeLabel(),
                PrimaryText: DisplayValueFormatter.FormatNumber(currentValue),
                SubtitleText: BuildDamageSubtitleText(enhancedSummary),
                TooltipText: BuildSharedDamageTooltipText(powerSnapshot?.OutcomeReceipt, enhancedPower),
                Segments: segments);
        }

        private static string BuildDamageSubtitleText(DamageBreakdownSummary summary)
        {
            if (!summary.HasPercentDamage)
            {
                return string.Empty;
            }

            return $"{DisplayValueFormatter.FormatPercentValue(summary.PercentOfTargetHpTotal, 2)}% target HP";
        }

        private static string GetDamageModeLabel()
        {
            var chanceLabel = MidsContext.Config.DamageMath.Calculate switch
            {
                ConfigData.EDamageMath.Average => "Average",
                ConfigData.EDamageMath.Max => "Maximum",
                ConfigData.EDamageMath.Minimum => "Minimum",
                _ => "Average"
            };

            var returnLabel = MidsContext.Config.DamageMath.ReturnValue switch
            {
                ConfigData.EDamageReturn.DPS => "DPS",
                ConfigData.EDamageReturn.DPA => "DPA",
                _ => "Damage"
            };

            return $"{chanceLabel} {returnLabel}";
        }

        private static string BuildSharedDamageTooltipText(
            PowerOutcomeReceipt? outcomeReceipt,
            IPower displayedPower)
        {
            // ModernDamageDisplay and MidsToolTip only render the popup.
            // The shared damage formatter owns the actual tooltip wording.
            return Power.BuildDamageTip(displayedPower, outcomeReceipt);
        }

        private PowerCalculationSnapshot? TryResolveDisplayedPowerCalculationSnapshot(int historyIndex)
        {
            if (historyIndex < 0)
            {
                return null;
            }

            var snapshots = MainModule.MidsController.Toon?.LastCalculationSnapshot?.PowerSnapshots;
            return snapshots != null && historyIndex < snapshots.Count
                ? snapshots[historyIndex]
                : null;
        }

        private void RefreshDamageCardPresentation(IPower? basePower = null, IPower? enhancedPower = null)
        {
            var sourceBase = basePower ?? pBase;
            var sourceEnhanced = enhancedPower ?? pEnh ?? sourceBase;
            if (sourceBase == null || sourceEnhanced == null)
            {
                infoDamageDisplay.Clear();
                return;
            }

            var presentation = BuildDamageCardPresentation(sourceBase, sourceEnhanced);
            if (presentation.HasContent)
            {
                infoDamageDisplay.Presentation = presentation;
            }
            else
            {
                infoDamageDisplay.Clear();
            }
        }

        private void DisplayInfo(bool noLevel = false, int iEnhLvl = -1)
        {
            if (pBase == null)
            {
                powerStatsGrid.Clear();
                infoSDesc.Clear();
                infoLDesc.Text = string.Empty;
                UpdateInfoDescriptionLayout();
                title.Text = string.Empty;
                subTitle.Text = string.Empty;
                infoDamageDisplay.Clear();
                return;
            }

            var enhancedPower = (pEnh == null || pEnh.PowerIndex == -1) ? pBase : pEnh;

            title.Text = !noLevel && pBase.Level > 0
                ? $"[{(rootPowerBase?.Level ?? pBase.Level)}] {pBase.DisplayName}"
                : pBase.DisplayName;
            if (iEnhLvl > -1) title.Text += $" (Slot Level {iEnhLvl + 1})";
            subTitle.Text = _presentationMode == MidsDataViewNeoPresentationMode.ActorReadOnly && !string.IsNullOrWhiteSpace(_actorPowerSourceDescription)
                ? $"Power Source: {_actorPowerSourceDescription}"
                : "Enhancement Values";

            var longInfo = Regex.Replace(pBase.DescLongFormatted.Trim().Replace("\0", ""), @"[ \t]{2,}", " ");
            var shortDescription = pBase.DescShort.Trim();
            if (_presentationMode == MidsDataViewNeoPresentationMode.ActorReadOnly && !string.IsNullOrWhiteSpace(_actorPowerSourceDescription))
            {
                shortDescription = $"Power Source: {_actorPowerSourceDescription}\r\n{shortDescription}";
            }

            var sharedRechargeSummary = BuildSharedRechargeInfoSummary(pBase);
            if (!string.IsNullOrWhiteSpace(sharedRechargeSummary))
            {
                shortDescription = string.IsNullOrWhiteSpace(shortDescription)
                    ? sharedRechargeSummary
                    : $"{sharedRechargeSummary}\r\n{shortDescription}";
            }

            var statRows = PowerCanonicalStats.BuildRows(pBase, enhancedPower, _displayContributions, HistoryIDX);
            infoSDesc.Rtf = RTF.FormatMarkupDocument(shortDescription, infoSDesc.Font);
            infoLDesc.Rtf = RTF.FormatMarkupDocument(longInfo, infoLDesc.Font);
            powerStatsGrid.SetRows(statRows);
            UpdateInfoDescriptionLayout();

            RefreshDamageCardPresentation(pBase, enhancedPower);

            SetPowerScaler();
        }

        private void DisplayData(bool noLevel = false, int iEnhLevel = -1)
        {
            if (IsDisposed)
            {
                // Occurs when trying to load a build made for a different database
                // and auto switch
                return;
            }

            infoDamageDisplay.ShowGraph = !MidsContext.Config.DisableDataDamageGraph;
            UpdateInfoDescriptionLayout();

            //lblLock.Visible = Lock & (_selectedTabIndex != 2);
            DisplayInfo(noLevel, iEnhLevel);
            DisplayEffects(noLevel, iEnhLevel);
            DisplayBonuses();
            DisplayEdFigures();
        }

        private void DisplayEffects(bool noLevel = false, int iEnhLvl = -1)
        {
            if (pBase == null) return;

            // --- Build UI-agnostic effect items (uses the same ranked effects pipeline) ---
            // GroupedRankedEffects is already set earlier in the flow when powers change 
            var enh = pEnh ?? pBase;
            var effectBase = _effectsComparisonBase ?? pBase;
            var rankedSafe = GetRankedEffectsSafe(enh);

            // --- Build Effect groups for the PowerEffectsGrid ---
            // PowerEffects maps items into (Defense/Resistance, Heal/Endurance, Status, Buff/Debuff, Movement, Special, Descriptors)
            var groups = PowerEffects.Build(effectBase, enh, GroupedRankedEffects, rankedSafe);

            // --- Push into the grid ---
            effectsGrid.SetGroups(groups);
        }

        private static List<int> GetRankedEffectsSafe(IPower power)
        {
            var ranked = power?.GetRankedEffects(true)?.ToList();
            if (ranked == null || ranked.Count == 0)
            {
                var n = power?.Effects?.Length ?? 0;
                ranked = Enumerable.Range(0, n).ToList();
            }
            return ranked;
        }

        private static string BuildSharedRechargeInfoSummary(IPower power)
        {
            var groups = power.RechargeGroups
                .Where(group => !string.IsNullOrWhiteSpace(group))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (groups.Length == 0)
            {
                return string.Empty;
            }

            var linkedPowers = (DatabaseAPI.Database?.Power ?? Array.Empty<IPower>())
                .Where(other => other != null &&
                                !string.Equals(other.FullName, power.FullName, StringComparison.OrdinalIgnoreCase) &&
                                other.RechargeGroups.Any(group => groups.Contains(group, StringComparer.OrdinalIgnoreCase)))
                .Select(other => other.DisplayName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .Take(5)
                .ToArray();

            return linkedPowers.Length > 0
                ? $"Shared Recharge: {string.Join(", ", linkedPowers)}"
                : $"Shared Recharge Group{(groups.Length == 1 ? "" : "s")}: {string.Join(", ", groups.Select(group => group.Replace('_', ' ').Trim()))}";
        }

        private void DisplayFlippedEnhancements()
        {
            using var pen = enhDataList.BackColor.B <= 10
                ? new Pen(Color.FromArgb(byte.MaxValue, 0, 0))
                : new Pen(Color.FromArgb(0, 0, byte.MaxValue));

            EnsureFlipBuffer();
            if (bxFlip?.Graphics == null)
            {
                return;
            }

            bxFlip.Graphics.Clear(enhDataList.BackColor);
            if (pBase == null)
            {
                RedrawFlip();
                return;
            }

            var build = MidsContext.Character?.CurrentBuild;
            if (build == null)
            {
                RedrawFlip();
                return;
            }

            var inToonHistory = ResolveDisplayedBuildHistoryIndex(allowPowerLookup: false);
            if (inToonHistory < 0 || !HasSlottedEnhancements(build.Powers[inToonHistory]))
            {
                RedrawFlip();
            }
            else
            {
                bxFlip.Graphics.DrawRectangle(pen, 0, 0, pnlEnhActive.Width - 1, pnlEnhInactive.Height - 1);
                bxFlip.Graphics.DrawRectangle(pen, 0, pnlEnhInactive.Height, pnlEnhActive.Width - 1, pnlEnhInactive.Height - 1);
                using var format = new StringFormat();
                var num1 = bxFlip.Size.Width - 188;
                var rectangle1 = new Rectangle();
                ref var local1 = ref rectangle1;
                var width = num1;
                var size = bxFlip.Size;
                var height = (int)Math.Round(size.Height / 2.0);
                local1 = new Rectangle(-4, 0, width, height);
                using var solidBrush1 = new SolidBrush(enhDataList.ItemColor);
                format.Alignment = StringAlignment.Far;
                format.LineAlignment = StringAlignment.Center;
                bxFlip.Graphics.DrawString("Active Slotting:", pnlEnhActive.Font, solidBrush1, rectangle1, format);
                rectangle1.Y += rectangle1.Height;
                bxFlip.Graphics.DrawString("Alternate:", pnlEnhActive.Font, solidBrush1, rectangle1, format);
                //ImageAttributes recolorIa = clsDrawX.GetRecolorIa(MidsContext.Character.IsHero());
                using var solidBrush2 = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                var power = build.Powers[inToonHistory];
                for (var index = 0; index < power.SlotCount; index++)
                {
                    var iDest = new Rectangle();
                    ref var local2 = ref iDest;
                    var x1 = num1 + 30 * index;
                    size = bxFlip.Size;
                    var y1 = (int)Math.Round((size.Height / 2.0 - 30) / 2.0);
                    local2 = new Rectangle(x1, y1, 30, 30);
                    var rectangle2 = new Rectangle();
                    ref var local3 = ref rectangle2;
                    var x2 = num1 + 30 * index;
                    size = bxFlip.Size;
                    var num3 = size.Height / 2.0;
                    size = bxFlip.Size;
                    var num4 = (size.Height / 2.0 - 30) / 2.0;
                    var y2 = (int)Math.Round(num3 + num4);
                    local3 = new Rectangle(x2, y2, 30, 30);
                    Rectangle destRect;
                    if (power.Slots[index].Enhancement.Enh > -1)
                    {
                        var graphics1 = bxFlip.Graphics;
                        AssetManager.DrawEnhancementAt(graphics1, iDest, DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].ImageIdx, power.Slots[index].Enhancement.Enh, DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].TypeID, power.Slots[index].Enhancement.Grade);
                        DrawEnhancementLevelOverlay(bxFlip.Graphics, iDest, power.Slots[index].Enhancement);
                    }
                    else
                    {
                        destRect = iDest with { Width = 30, Height = 30 };
                        bxFlip.Graphics.DrawImage(AssetManager.EmptySlot.Bitmap, destRect);
                    }

                    if (power.Slots[index].FlippedEnhancement.Enh > -1)
                    {
                        var graphics1 = bxFlip.Graphics;
                        AssetManager.DrawEnhancementAt(graphics1, rectangle2, DatabaseAPI.Database.Enhancements[power.Slots[index].FlippedEnhancement.Enh].ImageIdx, power.Slots[index].FlippedEnhancement.Enh, DatabaseAPI.Database.Enhancements[power.Slots[index].FlippedEnhancement.Enh].TypeID, power.Slots[index].FlippedEnhancement.Grade);
                    }
                    else
                    {
                        destRect = rectangle2 with { Width = 30, Height = 30 };
                        bxFlip.Graphics.DrawImage(AssetManager.EmptySlot.Bitmap, destRect);
                    }

                    rectangle2.Inflate(2, 2);
                    bxFlip.Graphics.FillEllipse(solidBrush2, rectangle2);
                    DrawEnhancementLevelOverlay(bxFlip.Graphics, local3, power.Slots[index].FlippedEnhancement);
                }

                RedrawFlip();
            }
        }

        private void DrawEnhancementLevelOverlay(Graphics graphics, Rectangle slotBounds, I9Slot enhancement)
        {
            if (enhancement.Enh < 0)
            {
                return;
            }

            var enhancementDef = DatabaseAPI.Database.Enhancements[enhancement.Enh];
            string overlayText;
            Color overlayColor;

            if (!MidsContext.Config.I9.HideIOLevels &&
                enhancementDef.TypeID is Enums.eType.SetO or Enums.eType.InventO)
            {
                overlayText = $"{enhancement.IOLevel + 1}";
                overlayColor = Color.Cyan;
            }
            else if (MidsContext.Config.ShowEnhRel &&
                     enhancementDef.TypeID is Enums.eType.Normal or Enums.eType.SpecialO)
            {
                overlayText = Enums.GetRelativeString(enhancement.RelativeLevel, MidsContext.Config.ShowRelSymbols);
                overlayColor = GetEnhancementRelativeLevelColor(enhancement.RelativeLevel);
            }
            else
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(overlayText))
            {
                return;
            }

            var fontSize = Math.Max(6f, pnlEnhActive.Font.SizeInPoints - 3f);
            using var overlayFont = new Font(pnlEnhActive.Font.FontFamily, fontSize, FontStyle.Bold, GraphicsUnit.Point);
            var overlayBounds = new RectangleF(
                slotBounds.X,
                slotBounds.Y - ScalePx(1),
                slotBounds.Width,
                Math.Max(ScalePx(9), overlayFont.GetHeight(graphics) + 1));

            BuildRenderer.DrawOutlineText(overlayText, overlayBounds, overlayColor,
                Color.FromArgb(160, 0, 0, 0), overlayFont, 0.85f, graphics);
        }

        private static Color GetEnhancementRelativeLevelColor(Enums.eEnhRelative relativeLevel)
        {
            if (relativeLevel == Enums.eEnhRelative.None)
            {
                return Color.Red;
            }

            if (relativeLevel < Enums.eEnhRelative.Even)
            {
                return Color.Yellow;
            }

            return relativeLevel == Enums.eEnhRelative.Even
                ? Color.White
                : Color.FromArgb(0, byte.MaxValue, byte.MaxValue);
        }

        private string GetToWhoShort(IEffect fx)
        {
            return fx.ToWho switch
            {
                Enums.eToWho.Target => " (Tgt)",
                Enums.eToWho.Self => " (Self)",
                _ => ""
            };
        }

        private List<IEffect[]> SwapExtraEffects(IEffect[] baseEffects, IEffect[] enhEffects)
        {
            var enhFxList = enhEffects.ToList();
            for (var i = enhEffects.Length; i < baseEffects.Length; i++)
            {
                enhFxList.Add((IEffect)baseEffects[i].Clone());
            }

            var baseFxList = new List<IEffect>();
            for (var i = 0; i < enhEffects.Length; i++)
            {
                baseFxList.Add((IEffect)baseEffects[i].Clone());
            }

            baseEffects = baseFxList.ToArray();
            enhEffects = enhFxList.ToArray();

            return [baseEffects, enhEffects];
        }

        private List<Enums.ShortFX[]> SwapExtraEffects(Enums.ShortFX[] baseEffects, Enums.ShortFX[] enhEffects)
        {
            var enhFxList = enhEffects.ToList();
            for (var i = enhEffects.Length; i < baseEffects.Length; i++)
            {
                enhFxList.Add((Enums.ShortFX)baseEffects[i].Clone());
            }

            var baseFxList = new List<Enums.ShortFX>();
            for (var i = 0; i < enhEffects.Length; i++)
            {
                baseFxList.Add((Enums.ShortFX)baseEffects[i].Clone());
            }

            baseEffects = baseFxList.ToArray();
            enhEffects = enhFxList.ToArray();

            return [baseEffects, enhEffects];
        }

        private static string ConvertNewlinesToRtf(string str)
        {
            return str
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", RTF.Crlf());
        }

        private static string GetEnhancementStringLongRtf(I9Slot iEnh)
        {
            var str = iEnh.GetEnhancementStringLong();
            if (!string.IsNullOrEmpty(str))
            {
                str = RTF.Color(RTF.ElementID.Enhancement) + RTF.Italic(ConvertNewlinesToRtf(str)) + RTF.Color(RTF.ElementID.Text);
            }

            return str;
        }

        private static string GetEnhancementStringRtf(I9Slot iEnh)
        {
            var str = iEnh.GetEnhancementString();
            if (!string.IsNullOrEmpty(str))
            {
                str = RTF.Color(RTF.ElementID.Enhancement) + ConvertNewlinesToRtf(str) + RTF.Color(RTF.ElementID.Text);
            }

            return str;
        }

        private PairedListEx.Item GetRankedEffect(int[] index, int id)
        {
            var title = string.Empty;
            var shortFxBase = new Enums.ShortFX();
            var shortFxEnh = new Enums.ShortFX();
            var tag2 = new Enums.ShortFX();
            var suffix = string.Empty;
            var enhancedPower = pEnh ?? pBase;
            var fx = pEnh != null && index[id] < pEnh.Effects.Length
                ? pEnh.Effects[index[id]]
                : index[id] < pBase.Effects.Length
                    ? pBase.Effects[index[id]]
                        : null;

            var fx2 = id <= 0
                ? null
                : pEnh != null && index[id - 1] < pEnh.Effects.Length
                    ? pEnh.Effects[index[id - 1]]
                    : index[id - 1] < pBase.Effects.Length
                        ? pBase.Effects[index[id - 1]]
                        : null;

            if (fx == null)
            {
                return FastItemBuilder.Fi.FastItem("", 0f, 0f, string.Empty);
            }

            if (index[id] > -1)
            {
                var flag = false;
                var onlySelf = fx.ToWho == Enums.eToWho.Self;
                var onlyTarget = fx.ToWho == Enums.eToWho.Target;
                if (id > 0)
                {
                    flag = (fx.EffectType == fx2.EffectType) &
                           (fx.ToWho == Enums.eToWho.Self) &
                           (fx2.ToWho == Enums.eToWho.Self) &
                           (fx.ToWho == Enums.eToWho.Target);
                }

                if (fx.DelayedTime > 5)
                {
                    flag = true;
                }

                var names = Enum.GetNames(typeof(Enums.eEffectTypeShort));
                if (fx.EffectType == Enums.eEffectType.Enhancement)
                {
                    title = fx.ETModifies switch
                    {
                        Enums.eEffectType.EnduranceDiscount => "+EndRdx",
                        Enums.eEffectType.RechargeTime => "+Rechg",
                        Enums.eEffectType.Mez => fx.MezType == Enums.eMez.None
                            ? "+Effects"
                            : $"Enh({Enum.GetName(Enums.eMezShort.None.GetType(), fx.MezType)})",
                        Enums.eEffectType.Defense => "Enh(Def)",
                        Enums.eEffectType.Resistance => "Enh(Res)",
                        _ => FastItemBuilder.Str.CapString(Enum.GetName(fx.ETModifies.GetType(), fx.ETModifies), 7)
                    };

                    shortFxBase.Assign(pBase.GetEffectMagSum(fx.EffectType,
                        fx.ETModifies, fx.DamageType,
                        fx.MezType, false, onlySelf, onlyTarget));

                    shortFxEnh.Assign(enhancedPower.GetEffectMagSum(enhancedPower.Effects[index[id]].EffectType,
                        enhancedPower.Effects[index[id]].ETModifies, enhancedPower.Effects[index[id]].DamageType,
                        enhancedPower.Effects[index[id]].MezType, false, onlySelf, onlyTarget));
                }
                else
                {
                    title = fx.EffectType switch
                    {
                        Enums.eEffectType.Mez => Enums.GetMezName((Enums.eMezShort)fx.MezType),
                        Enums.eEffectType.MezProtect => MezSemantics.GetStatusProtectionLabel(fx.MezType),
                        Enums.eEffectType.MezResist => MezSemantics.GetStatusResistanceLabel(fx.MezType),
                        _ => names[(int)fx.EffectType]
                    };
                }

                var temp = string.Empty;
                switch (fx.EffectType)
                {
                    case Enums.eEffectType.HitPoints:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf, onlyTarget));
                        tag2.Assign(shortFxBase);
                        var baseHitPoints = GetDisplayedBaseHitPoints();
                        shortFxBase.Sum = (float)(shortFxBase.Sum / (double)baseHitPoints * 100);
                        shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)baseHitPoints * 100);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Heal:
                        if (fx.BuffedMag <= 1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget));
                            var healBaseHitPoints = GetDisplayedBaseHitPoints();
                            shortFxBase.Sum = (float)(shortFxBase.Sum / (double)healBaseHitPoints * 100);
                            shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)healBaseHitPoints * 100);
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Absorb:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf, onlyTarget));
                        var absorbPercent = pBase.Effects
                            .Where(e => e.EffectType == Enums.eEffectType.Absorb)
                            .Any(e => e.DisplayPercentage);
                        tag2.Assign(shortFxBase);
                        suffix = absorbPercent ? "%" : "";
                        break;
                    case Enums.eEffectType.Endurance:
                        if (fx.BuffedMag < -0.01 && fx.BuffedMag > -1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf, onlyTarget));
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Regeneration:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf, onlyTarget));
                        shortFxEnh.Sum *= 100;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Null:
                        if (fx.BuffedMag < 1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf, onlyTarget));
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.ToHit:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Fly:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Recovery:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Mez when fx.Duration > float.Epsilon &&
                                                     fx.MezType is not (Enums.eMez.Knockback or Enums.eMez.Knockup or Enums.eMez.Repel or Enums.eMez.Teleport):
                    {
                        var baseEffect = index[id] < pBase.Effects.Length ? pBase.Effects[index[id]] : fx;
                        var enhancedEffect = index[id] < enhancedPower.Effects.Length ? enhancedPower.Effects[index[id]] : fx;
                        var useAlternate = Math.Abs(baseEffect.Duration - enhancedEffect.Duration) > 0.01f ||
                                           Math.Abs(baseEffect.BuffedMag - enhancedEffect.BuffedMag) > 0.01f;
                        var mezTip = enhancedPower.BuildTooltipStringAllVectorsEffects(
                            enhancedEffect.EffectType,
                            enhancedEffect.ETModifies,
                            enhancedEffect.DamageType,
                            enhancedEffect.MezType);
                        var mezValue = $"{DisplayValueFormatter.FormatSeconds(enhancedEffect.Duration, 2)}s (Mag {DisplayValueFormatter.FormatMagnitude(enhancedEffect.BuffedMag, 2)}){suffix}";
                        return new PairedListEx.Item(title, mezValue, useAlternate, fx.Probability < 1, fx.HasConditions, mezTip);
                    }
                    case Enums.eEffectType.Mez when fx.MezType is Enums.eMez.Taunt or Enums.eMez.Placate:
                        shortFxBase.Add(index[id], fx.Duration);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].Duration);
                        tag2.Assign(shortFxBase);
                        suffix = "s";
                        break;

                    // Set list of effects below that are treated as percentages
                    // Base and enhanced values will be multiplied by 100
                    case Enums.eEffectType.DamageBuff:
                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.ResEffect:
                    case Enums.eEffectType.Enhancement:
                    case Enums.eEffectType.MezProtect:
                    case Enums.eEffectType.MezResist:
                    case Enums.eEffectType.RechargeTime:
                    case Enums.eEffectType.SpeedFlying:
                    case Enums.eEffectType.SpeedRunning:
                    case Enums.eEffectType.SpeedJumping:
                    case Enums.eEffectType.JumpHeight:
                    case Enums.eEffectType.PerceptionRadius:
                    case Enums.eEffectType.Meter:
                    case Enums.eEffectType.Range:
                    case Enums.eEffectType.MaxFlySpeed:
                    case Enums.eEffectType.MaxRunSpeed:
                    case Enums.eEffectType.MaxJumpSpeed:
                    case Enums.eEffectType.Jumppack:
                    case Enums.eEffectType.GlobalChanceMod:
                        if (fx.EffectType != Enums.eEffectType.Enhancement)
                        {
                            shortFxBase.Add(index[id], fx.BuffedMag);
                            shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].BuffedMag);
                        }

                        shortFxBase.Multiply();
                        shortFxEnh.Multiply();

                        tag2.Assign(enhancedPower.GetEffectMagSum(fx.EffectType, false, onlySelf, onlyTarget));
                        break;
                    case Enums.eEffectType.SilentKill:
                        shortFxBase.Add(index[id], fx.Absorbed_Duration);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].Absorbed_Duration);
                        tag2.Assign(shortFxBase);
                        break;
                    default:
                        shortFxBase.Add(index[id], fx.BuffedMag);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].BuffedMag);
                        tag2.Assign(shortFxBase);
                        break;
                }

                if (fx.DisplayPercentage)
                {
                    suffix = "%";
                }

                suffix += fx.ToWho switch
                {
                    Enums.eToWho.Target => " (Tgt)",
                    Enums.eToWho.Self => " (Self)",
                    _ => ""
                };

                if (flag)
                {
                    return FastItemBuilder.Fi.FastItem("", 0f, 0f, string.Empty);
                }
            }

            for (var fxIndex = 0; fxIndex < shortFxEnh.Index.Length; fxIndex++)
            {
                var sFxIdx = shortFxEnh.Index[fxIndex];
                if (sFxIdx >= pBase.Effects.Length & sFxIdx >= pEnh.Effects.Length)
                {
                    continue;
                }

                var effect = sFxIdx < pBase.Effects.Length
                    ? pBase.Effects[sFxIdx]
                    : pEnh.Effects[sFxIdx];

                if (sFxIdx <= -1 || !effect.DisplayPercentage)
                {
                    continue;
                }

                if (shortFxEnh.Value[fxIndex] > 1)
                {
                    continue;
                }

                switch (effect.EffectType)
                {
                    case Enums.eEffectType.Absorb:
                        //Fixes the Absorb display to correctly show the percentage
                        shortFxEnh.Sum = float.Parse(shortFxEnh.Sum.ToString("P", CultureInfo.InvariantCulture).Replace("%", ""));
                        break;
                    case Enums.eEffectType.ToHit:
                        //Fixes the ToHit display to correctly show the percentage
                        if (effect.Stacking == Enums.eStacking.Yes)
                        {
                            var overage = fx.Ticks * 0.05f;
                            shortFxEnh.Sum -= overage;
                            shortFxEnh.Sum /= 2;
                        }

                        break;
                    default:
                        shortFxEnh.ReSum();
                        break;
                }

                break;
            }

            // shortFxEnh.Index.Length == 0 will occur if all effects of the same kind
            // have non validated conditionals.
            // E.g. -Recovery on Kick if Cross Punch has not been picked.
            var tip = shortFxEnh.Index.Length <= 0
                ? ""
                : pEnh.BuildTooltipStringAllVectorsEffects(pEnh.Effects[shortFxEnh.Index[0]].EffectType,
                pEnh.Effects[shortFxEnh.Index[0]].ETModifies, pEnh.Effects[shortFxEnh.Index[0]].DamageType,
                pEnh.Effects[shortFxEnh.Index[0]].MezType);

            if (fx.HasConditions)
            {
                return FastItemBuilder.Fi.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, fx.HasConditions, tip);
            }

            return FastItemBuilder.Fi.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, false, tip);
        }

        private static bool IsMezEffect(string iStr)
        {
            var names = Enum.GetNames(Enums.eMez.None.GetType());
            var num = names.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (string.Equals(iStr, names[index], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

            return false;
        }

        private void RedrawFlip()
        {
            pnlEnhActive.Invalidate();
            pnlEnhInactive.Invalidate();
        }

        private void ClearEnhancementPanels()
        {
            EnsureFlipBuffer();
            if (bxFlip?.Graphics != null)
            {
                bxFlip.Graphics.Clear(CurrentTheme.Background);
            }

            RedrawFlip();
        }

        private void EnsureFlipBuffer()
        {
            var width = Math.Max(1, pnlEnhActive.Width);
            var height = Math.Max(1, pnlEnhActive.Height + pnlEnhInactive.Height);

            if (bxFlip?.Size == new Size(width, height))
            {
                return;
            }

            bxFlip?.Dispose();
            bxFlip = new ExtendedBitmap(width, height);
        }

        private void DisposeFlipBuffer()
        {
            bxFlip?.Dispose();
            bxFlip = null;
        }

        private void ResetFlipBuffer()
        {
            DisposeFlipBuffer();
            RedrawFlip();
        }

        private void DrawFlipPanel(Graphics graphics, bool inactive)
        {
            graphics.Clear(CurrentTheme.Background);

            if (bxFlip == null)
            {
                DisplayFlippedEnhancements();
            }

            if (bxFlip?.Bitmap == null)
            {
                return;
            }

            var target = inactive ? pnlEnhInactive : pnlEnhActive;
            var sourceY = inactive ? pnlEnhActive.Height : 0;
            var srcRect = new Rectangle(0, sourceY, target.Width, target.Height);
            var destRect = new Rectangle(Point.Empty, target.Size);
            graphics.DrawImage(bxFlip.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        private void SetDamageTip()
        {
            RefreshDamageCardPresentation();
        }

        private Power? GetPowerRedirectParent(IPower pSrc)
        {
            var pSrcRedirectParent = new Power
            {
                FullName = ""
            };
            foreach (var power in DatabaseAPI.Database.Power)
            {
                if (power is null) continue;
                var powerset = power.GetPowerSet();
                if (powerset is null) continue;
                if (powerset.SetType != Enums.ePowerSetType.Primary & powerset.SetType != Enums.ePowerSetType.Secondary & powerset.SetType != Enums.ePowerSetType.Pool & powerset.SetType != Enums.ePowerSetType.Ancillary)
                {
                    continue;
                }

                foreach (var fx in power.Effects)
                {
                    if (fx.EffectType != Enums.eEffectType.PowerRedirect)
                    {
                        continue;
                    }

                    if (fx.Override != pSrc.FullName)
                    {
                        continue;
                    }

                    pSrcRedirectParent = new Power(DatabaseAPI.GetPowerByFullName(power.FullName));
                }
            }

            return pSrcRedirectParent.FullName == "" ? null : pSrcRedirectParent;
        }

        private void SetPowerScaler()
        {
            if (pBase == null || HistoryIDX < 0 || MidsContext.Character?.CurrentBuild == null)
            {
                sliderHost.Visible = false;
                return;
            }

            if (pBase.VariableEnabled)
            {
                var str = string.IsNullOrEmpty(pBase.VariableName) ? "Targets" : pBase.VariableName;
                var currentValue = MidsContext.Character.CurrentBuild.Powers[HistoryIDX].VariableValue;
                currentValue = Math.Clamp(currentValue, pBase.VariableMin, pBase.VariableMax);

                _updatingPowerScaler = true;
                try
                {
                    var rawDisplayStep = Math.Max(1, (int)Math.Round(
                        pBase.VariableDisplayStep * pBase.VariableDisplayDivisor,
                        MidpointRounding.AwayFromZero));
                    midsTrackBar1.Text = $"{str}:";
                    midsTrackBar1.Minimum = pBase.VariableMin;
                    midsTrackBar1.Maximum = pBase.VariableMax;
                    midsTrackBar1.DisplayDivisor = pBase.VariableDisplayDivisor;
                    midsTrackBar1.DisplayPrecision = pBase.VariableDisplayPrecision;
                    midsTrackBar1.DisplayStep = pBase.VariableDisplayStep;
                    midsTrackBar1.SmallChange = rawDisplayStep;
                    midsTrackBar1.LargeChange = pBase.VariableDisplayPrecision > 0
                        ? rawDisplayStep * 10
                        : Math.Max(rawDisplayStep, (pBase.VariableMax - pBase.VariableMin) / 10);
                    midsTrackBar1.ValueTextFormat = "{0}";
                    midsTrackBar1.ShowValue = true;
                    midsTrackBar1.Value = currentValue;
                    pLastScaleVal = currentValue;
                    sliderHost.Visible = true;
                }
                finally
                {
                    _updatingPowerScaler = false;
                }
            }
            else
            {
                sliderHost.Visible = false;
            }
        }

        private bool SFxCheck(Enums.ShortFX isFx)
        {
            return isFx.Index != null && isFx.Index.Any(t => pBase?.Effects.Length > t & t > -1 && pBase?.Effects[t].isEnhancementEffect == true);
        }

        private string ShortStr(string full, string brief)
        {
            return infoDataList.Font.Size <= 100f / full.Length ? full : brief;
        }

        private int MiniGetEnhIndex(int iX, int iY)
        {
            if (bxFlip == null) return -1;
            var num1 = bxFlip.Size.Width - 188;
            var build = MidsContext.Character?.CurrentBuild;
            if (build == null)
            {
                return -1;
            }

            var inToonHistory = ResolveDisplayedBuildHistoryIndex(allowPowerLookup: false);
            if (inToonHistory < 0)
            {
                return -1;
            }

            for (var index = 0; index < build.Powers[inToonHistory].SlotCount; index++)
            {
                var rectangle = new Rectangle(num1 + 30 * index, (int)Math.Round((bxFlip.Size.Height / 2f - 30) / 2f), 30, 30);
                if ((iX > rectangle.X) & (iX < rectangle.X + rectangle.Width) &&
                    (iY > rectangle.Y) & (iY < rectangle.Y + rectangle.Height))
                {
                    return index;
                }
            }

            return -1;
        }

        private bool SplitFX_AddToList(ref Enums.ShortFX baseSfx, ref Enums.ShortFX enhSfx, ref PairedListEx iList, string specialTitle = "")
        {
            if (!baseSfx.Present)
            {
                return false;
            }

            var shortFxArray1 = Power.SplitFX(ref baseSfx, ref pBase);
            var shortFxArray2 = Power.SplitFX(ref enhSfx, ref pEnh);
            if (shortFxArray2.Length < shortFxArray1.Length)
            {
                var swappedFx = SwapExtraEffects(shortFxArray1, shortFxArray2);
                shortFxArray1 = (Enums.ShortFX[])swappedFx[0].Clone();
                shortFxArray2 = (Enums.ShortFX[])swappedFx[1].Clone();
            }

            for (var index = 0; index < shortFxArray1.Length; index++)
            {
                if (!shortFxArray1[index].Present)
                {
                    continue;
                }

                var Suffix = string.Empty;
                var num2 = shortFxArray1[index].Value[0];
                var num3 = index < shortFxArray2.Length
                    ? shortFxArray2[index].Value[0]
                    : shortFxArray2[index - 1].Value[0];
                if (pEnh.Effects[shortFxArray1[index].Index[0]].DisplayPercentage)
                {
                    Suffix = "%";
                    var effect = pEnh.Effects[shortFxArray1[index].Index[0]];
                    if ((effect.EffectType == Enums.eEffectType.Heal |
                         effect.EffectType == Enums.eEffectType.Endurance |
                         effect.EffectType == Enums.eEffectType.Damage) &
                        pEnh.Effects[shortFxArray1[index].Index[0]].Aspect == Enums.eAspect.Cur)
                    {
                        num2 *= 100;
                        num3 *= 100;
                    }
                }
                else
                {
                    switch (pEnh.Effects[shortFxArray1[index].Index[0]].EffectType)
                    {
                        case Enums.eEffectType.Heal:
                        case Enums.eEffectType.HitPoints:
                            Suffix = " HP";
                            break;
                    }
                }

                var title = Enums.GetEffectNameShort(pEnh.Effects[shortFxArray1[index].Index[0]].EffectType);
                if (specialTitle != string.Empty)
                {
                    title = specialTitle;
                }

                var s1 = num2;
                var s2 = num3;
                if ((pEnh.Effects[shortFxArray1[index].Index[0]].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                {
                    s1 = 0;
                    s2 = 0;
                }

                iList.AddItem(FastItemBuilder.Fi.FastItem(title, s1, s2, Suffix, false, false, pEnh.Effects[shortFxArray1[index].Index[0]].Probability < 1.0, pEnh.Effects[shortFxArray1[index].Index[0]].HasConditions, Power.SplitFXGroupTip(ref shortFxArray1[index], ref pEnh, false)));
                if (pEnh.Effects[shortFxArray1[index].Index[0]].isEnhancementEffect)
                {
                    iList.SetUnique();
                }
            }

            return true;
        }

        #endregion

        #region Dock/Undock

        private void DockButton_Click(object? sender, EventArgs e)
        {
            if (_isDocked)
                Undock();
            else
                Redock();
        }

        private void Undock()
        {
            if (!_isDocked) return;

            // Remember original placement info for a reliable round-trip
            _originalParent = Parent;
            _originalDock = Dock;
            _originalBounds = Bounds;
            _originalIndex = _originalParent?.Controls.IndexOf(this) ?? -1;

            _floatingHostForm = new Form
            {
                Text = @"Data View",
                Size = new Size(Math.Max(400, Width), Math.Max(300, Height)),
                StartPosition = FormStartPosition.Manual,
                Location = PointToScreen(Point.Empty),
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                BackColor = BackColor
            };

            // Move into floating form
            _originalParent?.Controls.Remove(this);
            _floatingHostForm.Controls.Add(this);
            Dock = DockStyle.Fill;

            // Toggle icon to indicate current state
            DockButton.IconChar = IconChar.Anchor;
            DockButton.IconColor = Color.Silver;

            // When the toolwindow closes, re-dock automatically
            _floatingHostForm.FormClosing += FloatingHostForm_FormClosing;

            _floatingHostForm.Show();
            _isDocked = false;
        }

        private void FloatingHostForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Return to original parent on close (unless app is exiting)
            Redock();
        }

        private void Redock()
        {
            if (_isDocked)
                return;

            // Detach from floating form
            if (_floatingHostForm != null)
            {
                _floatingHostForm.FormClosing -= FloatingHostForm_FormClosing;
                _floatingHostForm.Controls.Remove(this);
                _floatingHostForm.Close();
                _floatingHostForm.Dispose();
                _floatingHostForm = null;
            }

            // Restore to original parent/position if still available
            if (_originalParent != null && !_originalParent.IsDisposed)
            {
                if (_originalIndex >= 0 && _originalIndex <= _originalParent.Controls.Count)
                {
                    _originalParent.Controls.Add(this);
                    _originalParent.Controls.SetChildIndex(this, _originalIndex);
                }
                else
                {
                    _originalParent.Controls.Add(this);
                }

                Dock = _originalDock;
                if (_originalDock == DockStyle.None)
                    Bounds = _originalBounds;
            }

            // Toggle icon back to locked anchor
            DockButton.IconChar = IconChar.UpRightFromSquare;
            DockButton.IconColor = Color.Silver;

            _isDocked = true;
        }

        #endregion

        #region Lock/Unlock

        private void ApplyLockVisuals()
        {
            LockButton.IconChar = _isLocked ? IconChar.Lock : IconChar.Unlock;
            LockButton.IconColor = _isLocked ? Color.Red : Color.LimeGreen;
            LockButton.Invalidate();
        }

        private void SetLock(bool locked, bool raiseEvent)
        {
            if (_isLocked == locked) return;
            _isLocked = locked;
            ApplyLockVisuals();
            if (raiseEvent)
                LockStateChanged?.Invoke(this, _isLocked);
        }

        public void ToggleLock() => SetLock(!_isLocked, true);

        private void LockButton_Click(object? sender, EventArgs e)
        {
            ToggleLock();
        }

        #endregion

        #region Lifecycle

        private void MidsDataView_Resize(object? sender, EventArgs e)
        {
            headerPanel.Invalidate();
            LayoutEnhancementPage();
            ResetFlipBuffer();
        }

        private void EnhanceView_Resize(object? sender, EventArgs e)
        {
            LayoutEnhancementPage();
        }

        private void EnhancementPanel_SizeChanged(object? sender, EventArgs e)
        {
            ResetFlipBuffer();
        }

        private void LayoutEnhancementPage()
        {
            if (enhanceView.ClientSize.Width <= 0 || enhanceView.ClientSize.Height <= 0)
            {
                return;
            }

            const int slotPanelHeight = 50;
            const int gap = 6;

            var width = enhanceView.ClientSize.Width;
            var subtitleBottom = enhanceSubtitlePanel.Bottom;
            var inactiveTop = Math.Max(subtitleBottom, enhanceView.ClientSize.Height - slotPanelHeight - 3);
            var activeTop = Math.Max(subtitleBottom, inactiveTop - slotPanelHeight - gap);
            var listTop = subtitleBottom + 1;
            var listHeight = Math.Max(0, activeTop - listTop - gap);

            enhDataList.SetBounds(0, listTop, width, listHeight);
            pnlEnhActive.SetBounds(0, activeTop, width, slotPanelHeight);
            pnlEnhInactive.SetBounds(0, inactiveTop, width, slotPanelHeight);
        }

        private void Fx_ListItemClick(object? sender, PairedListEx.Item? item, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (item?.EntTag == null)
            {
                return;
            }

            var allPowers = item.EntTag.GetPowers();
            var currentBuildPowers = MidsContext.Character.CurrentBuild.Powers
                .Where(pe => pe?.Power != null)
                .Select(pe => pe?.Power)
                .ToHashSet();

            var filteredPowers = allPowers
                .Where(powerPair => powerPair.Value == null || currentBuildPowers.Contains(powerPair.Value))
                .Select(powerPair => powerPair.Key)
                .ToList();

            if (pBase != null)
            {
                PetInfo = new PetInfo(item.EntTag, HistoryIDX, pBase);
            }

            var hashsetPowers = filteredPowers.Select(x => x.FullName).ToHashSet();

            EntityDetails?.Invoke(item.EntTag.UID, hashsetPowers, HistoryIDX, PetInfo);
        }

        private void PairedList_Hover(object? sender, int index, Enums.ShortFX tag, string tooltip)
        {
            var str1 = string.Empty;
            if (tag.Present)
            {
                var selectedEffects = tag.Index
                    .Where(t => t >= 0 && pEnh != null && t < pEnh.Effects.Length)
                    .ToArray();

                if (selectedEffects.Length > 0 && pEnh != null)
                {
                    str1 = GroupedFx.BuildPopupTooltipText(new Power(pEnh), selectedEffects);
                }

                if (string.IsNullOrWhiteSpace(str1))
                {
                    str1 = tooltip;
                }
            }
            else if (string.IsNullOrWhiteSpace(tooltip))
            {
                str1 = string.Empty;
            }
            else
            {
                str1 = tooltip;
            }

            if (!string.IsNullOrWhiteSpace(str1))
            {
                //dvToolTip.SetToolTip((Control)sender, str1);
            }
            else
            {
                //dvToolTip.SetToolTip((Control)sender, string.Empty);
            }
        }

        private void PairedList_ItemOut(object sender)
        {
            //dvToolTip.SetToolTip((Control)sender, string.Empty);
        }

        private void pnlEnhActive_MouseClick(object sender, MouseEventArgs e)
        {
            var build = MidsContext.Character?.CurrentBuild;

            if (build == null || e.Button != MouseButtons.Left)
            {
                return;
            }

            var inToonHistory = ResolveDisplayedBuildHistoryIndex(allowPowerLookup: false);
            if (inToonHistory <= -1)
            {
                return;
            }

            var slotFlip = SlotFlip;
            slotFlip?.Invoke(inToonHistory);
        }

        private void pnlEnhActive_MouseMove(object sender, MouseEventArgs e)
        {
            var build = MidsContext.Character?.CurrentBuild;
            if (build == null)
            {
                return;
            }

            var inToonHistory = ResolveDisplayedBuildHistoryIndex(allowPowerLookup: false);
            if (inToonHistory <= -1)
            {
                return;
            }

            var enhIndex = MiniGetEnhIndex(e.X, e.Y);
            if (enhIndex <= -1)
            {
                return;
            }

            SetEnhancement(build.Powers[inToonHistory].Slots[enhIndex].Enhancement,
                build.Powers[inToonHistory].Slots[enhIndex].Level);
        }

        private void pnlEnhActive_Paint(object sender, PaintEventArgs e)
        {
            DrawFlipPanel(e.Graphics, inactive: false);
        }

        private void pnlEnhInactive_MouseClick(object sender, MouseEventArgs e)
        {
            var build = MidsContext.Character?.CurrentBuild;

            if (build == null || e.Button != MouseButtons.Left)
            {
                return;
            }

            var inToonHistory = ResolveDisplayedBuildHistoryIndex(allowPowerLookup: false);
            if (inToonHistory <= -1)
            {
                return;
            }

            var slotFlip = SlotFlip;
            slotFlip?.Invoke(inToonHistory);
        }

        private void pnlEnhInactive_MouseMove(object sender, MouseEventArgs e)
        {
            var build = MidsContext.Character?.CurrentBuild;
            if (build == null)
            {
                return;
            }

            var inToonHistory = ResolveDisplayedBuildHistoryIndex(allowPowerLookup: false);
            if (inToonHistory <= -1)
            {
                return;
            }

            var enhIndex = MiniGetEnhIndex(e.X, e.Y);
            if (enhIndex <= -1)
            {
                return;
            }

            SetEnhancement(build.Powers[inToonHistory].Slots[enhIndex].FlippedEnhancement,
                build.Powers[inToonHistory].Slots[enhIndex].Level);
        }

        private void pnlEnhInactive_Paint(object sender, PaintEventArgs e)
        {
            DrawFlipPanel(e.Graphics, inactive: true);
        }

        private void MidsTrackBar_ValueChanged(object? sender, EventArgs e)
        {
            if (_updatingPowerScaler)
            {
                return;
            }

            if (midsTrackBar1.IsInteracting)
            {
                midsTrackBar1.Invalidate();
                midsTrackBar1.Update();
                _pendingPowerScaleValue = midsTrackBar1.Value;
                if (!_powerScalerDragTimer.Enabled)
                {
                    _powerScalerDragTimer.Start();
                }
                return;
            }

            PowerScaler_BarClick(midsTrackBar1.Value, notifyHost: true);
        }

        private void MidsTrackBar_InteractionCompleted(object? sender, EventArgs e)
        {
            _powerScalerDragTimer.Stop();
            if (_pendingPowerScaleValue > -1)
            {
                var pending = _pendingPowerScaleValue;
                _pendingPowerScaleValue = -1;
                PowerScaler_BarClick(pending, notifyHost: true);
            }
        }

        private void PowerScalerDragTimer_Tick(object? sender, EventArgs e)
        {
            if (_powerScalerPreviewApplying)
            {
                return;
            }

            if (_pendingPowerScaleValue > -1)
            {
                var pending = _pendingPowerScaleValue;
                _pendingPowerScaleValue = -1;

                try
                {
                    _powerScalerPreviewApplying = true;
                    midsTrackBar1.Update();
                    PowerScaler_BarClick(pending, notifyHost: false);
                }
                finally
                {
                    _powerScalerPreviewApplying = false;
                }
            }

            if (!midsTrackBar1.IsInteracting && _pendingPowerScaleValue < 0)
            {
                _powerScalerDragTimer.Stop();
            }
        }

        private void PowerScaler_BarClick(float val, bool notifyHost)
        {
            if (pBase == null || HistoryIDX < 0 || MidsContext.Character?.CurrentBuild == null)
            {
                return;
            }

            var num = (int)Math.Round(val);
            if (num < pBase.VariableMin)
            {
                num = pBase.VariableMin;
            }

            if (num > pBase.VariableMax)
            {
                num = pBase.VariableMax;
            }

            MidsContext.Character.CurrentBuild.Powers[HistoryIDX].VariableValue = num;
            
            if (num == pLastScaleVal)
            {
                if (notifyHost)
                {
                    SlotUpdate?.Invoke(pBase, num);
                }
                return;
            }

            pLastScaleVal = num;
            MainModule.MidsController.Toon?.GenerateBuffedPowerArray();
            RefreshCurrentPowerDisplaySnapshot(updateStatRows: notifyHost);

            if (notifyHost)
            {
                SlotUpdate?.Invoke(pBase, num);
            }
        }

        private void RefreshCurrentPowerDisplaySnapshot(bool updateStatRows)
        {
            if (HistoryIDX < 0)
            {
                return;
            }

            var snapshot = MainModule.MidsController.Toon?.GetDisplayPowerSnapshot(HistoryIDX);
            if (snapshot == null)
            {
                return;
            }

            pBase = snapshot.BasePower == null ? null : new Power(snapshot.BasePower);
            pEnh = snapshot.EnhancedPower == null ? null : new Power(snapshot.EnhancedPower);
            _effectsComparisonBase = snapshot.PowerCalculationSnapshot?.BasePower == null
                ? (pBase == null ? null : new Power(pBase))
                : new Power(snapshot.PowerCalculationSnapshot.BasePower);
            rootPowerBase = snapshot.RootPowerBase;
            rootPowerEnh = snapshot.RootPowerEnh;

            if (pBase == null)
            {
                infoDamageDisplay.Clear();
                return;
            }

            if (pEnh == null)
            {
                pEnh = new Power(pBase)
                {
                    PowerIndex = -1
                };
            }

            if (updateStatRows)
            {
                var statRows = PowerCanonicalStats.BuildRows(pBase, pEnh, _displayContributions, HistoryIDX);
                powerStatsGrid.SetRows(statRows);
            }
            RefreshDamageCardPresentation(pBase, pEnh);
        }

        #endregion
    }
}

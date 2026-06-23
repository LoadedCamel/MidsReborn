using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Test;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Forms.WindowMenuItems;

public sealed partial class FrmPetActorDetails : Form
{
    private const float MinimumResponsiveUiScale = 0.90f;
    private const float MaximumResponsiveUiScale = 1.25f;
    private const int BaselineFormWidth = 1280;
    private const int BaselineFormHeight = 800;
    private const float UiScaleIntensity = 0.35f;

    private readonly Toon _toon;
    private PetActorRibbon _actorRibbon = null!;
    private MidsVScrollPanel _powerGridScrollPanel = null!;
    private PetActorPowerGrid _powerGrid = null!;
    private PetActorDetailView _detailView = null!;
    private Panel _headerPanel = null!;
    private PetActorIconView _headerIcon = null!;
    private TableLayoutPanel _summaryMetrics = null!;
    private TableLayoutPanel _previewStatePanel = null!;
    private FlowLayoutPanel _upgradeFlow = null!;
    private FlowLayoutPanel _proximityFlow = null!;
    private SplitContainer _bodySplit = null!;
    private SplitContainer _detailTotalsSplit = null!;
    private Panel _emptyStatePanel = null!;
    private TableLayoutPanel _rootLayout = null!;
    private TableLayoutPanel _headerLayout = null!;
    private TableLayoutPanel _ribbonHost = null!;
    private TableLayoutPanel _powerPanel = null!;
    private Panel _contentHost = null!;
    private TableLayoutPanel _totalsHost = null!;
    private Label _nameLabel = null!;
    private Label _subTitleLabel = null!;
    private Label _classLabel = null!;
    private Label _countLabel = null!;
    private Label _tagsLabel = null!;
    private Label _emptyLabel = null!;
    private Panel _totalsScrollPanel = null!;
    private TableLayoutPanel _totalsStack = null!;
    private PowerStatsGrid _coreGrid = null!;
    private PowerStatsGrid _combatGrid = null!;
    private PowerStatsGrid _movementGrid = null!;
    private PowerStatsGrid _defenseGrid = null!;
    private PowerStatsGrid _statusGrid = null!;
    private Label _coreLabel = null!;
    private Label _combatLabel = null!;
    private Label _movementLabel = null!;
    private Label _defenseLabel = null!;
    private Label _statusLabel = null!;
    private readonly Dictionary<string, (Label Title, Label Value)> _metricLabels = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<RealPetActorRosterItem> _roster = [];
    private readonly Dictionary<string, PetActorPreviewState> _previewStatesByActor = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Control, float> _baseFontSizes = new();

    private PetActorSnapshot? _currentSnapshot;
    private string? _selectedEntityUid;
    private int _selectedSourceHistoryIndex = -1;
    private int _selectedPowerIndex = -1;
    private int _snapshotLoadVersion;
    private int _bonusLoadQueuedVersion = -1;
    private bool _startupRevealQueued;
    private bool _startupRevealCompleted;
    private bool _bonusEntriesPending;
    private bool _snapshotRefreshQueued;
    private bool _suppressPreviewStateEvents;
    private bool _forceDesignPreview;
    private bool _designTimeSampleApplied;
    private float _uiScale = 1f;

    private sealed record ShellPanelMetadata(Color AccentColor);
    private sealed record ShellBadgeSpec(MidsTotalsGlyph Glyph, Color AccentColor, Color GlowColor);

    private bool InDesigner => IsInDesigner();

    private DataViewTheme CurrentTheme => DesignMode
        ? ThemeManager.DesignTime.DataView
        : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

    private static bool IsInDesignerProcess()
    {
        static bool containsDesignHostName(string? value)
            => !string.IsNullOrWhiteSpace(value) &&
               (value.Contains("devenv", StringComparison.OrdinalIgnoreCase) ||
                value.Contains("designtoolsserver", StringComparison.OrdinalIgnoreCase) ||
                value.Contains("xdesproc", StringComparison.OrdinalIgnoreCase));

        return containsDesignHostName(AppDomain.CurrentDomain.FriendlyName) ||
               containsDesignHostName(Process.GetCurrentProcess().ProcessName);
    }

    private bool IsInDesigner()
    {
        return _forceDesignPreview ||
               DesignMode ||
               LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
               Site?.DesignMode == true ||
               IsInDesignerProcess();
    }

    public FrmPetActorDetails()
        : this(new Toon(), null, -1)
    {
        _forceDesignPreview = true;
        ApplyDesignTimeSampleIfNeeded();
    }

    public FrmPetActorDetails(Toon toon, string? initialEntityUid = null, int initialSourceHistoryIndex = -1)
    {
        _toon = toon;
        _selectedEntityUid = initialEntityUid;
        _selectedSourceHistoryIndex = initialSourceHistoryIndex;

        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
        InitializeComponent();
        InitializeShellContent();
        ApplyDesignTimeSampleIfNeeded();
    }

    private void InitializeShellContent()
    {
        _detailView.BonusesTabActivated += DetailViewOnBonusesTabActivated;
        _summaryMetrics.Controls.Clear();
        AddMetric("hp", "Hit Points");
        AddMetric("regen", "Regen");
        AddMetric("recovery", "Recovery");
        AddMetric("defense", "Peak Def");
        AddMetric("resistance", "Peak Res");

        ConfigurePreviewStateFlow(_upgradeFlow);
        ConfigurePreviewStateFlow(_proximityFlow);
        _previewStatePanel.Controls.Clear();
        _previewStatePanel.Controls.Add(CreatePreviewStateGroup("Applied Upgrades", _upgradeFlow), 0, 0);
        _previewStatePanel.Controls.Add(CreatePreviewStateGroup("Pet Context", _proximityFlow), 1, 0);

        ConfigureSectionLabel(_coreLabel, "Core and Sustain");
        ConfigureSectionLabel(_combatLabel, "Combat and Utility");
        ConfigureSectionLabel(_movementLabel, "Movement");
        ConfigureSectionLabel(_defenseLabel, "Defense and Resistance");
        ConfigureSectionLabel(_statusLabel, "Status and Debuff Resistances");

        ConfigureTotalsGrid(_coreGrid);
        ConfigureTotalsGrid(_combatGrid);
        ConfigureTotalsGrid(_movementGrid);
        ConfigureTotalsGrid(_defenseGrid);
        ConfigureTotalsGrid(_statusGrid);

        _totalsStack.Controls.Clear();
        AddTotalsSection(_coreLabel, _coreGrid);
        AddTotalsSection(_combatLabel, _combatGrid);
        AddTotalsSection(_movementLabel, _movementGrid);
        AddTotalsSection(_defenseLabel, _defenseGrid);
        AddTotalsSection(_statusLabel, _statusGrid);

        _bodySplit.Panel1.Paint -= BodyPaneOnPaint;
        _bodySplit.Panel1.Paint += BodyPaneOnPaint;
        _bodySplit.Panel2.Paint -= BodyPaneOnPaint;
        _bodySplit.Panel2.Paint += BodyPaneOnPaint;
    }

    private void PopulateDesignTimeSample()
    {
        var alphaWolf = new RealPetActorRosterItem
        {
            EntityUid = "Alpha_Howler_Wolf",
            EntityDisplayName = "Alpha Howler Wolf",
            EntityClassName = "Class Henchman Minion",
            SourceHistoryIndex = 0,
            SourcePowerFullName = "Beast_Mastery.Summon_Wolves.Summon_Wolves",
            SourcePowerDisplayName = "Summon Wolves",
            Count = 1
        };
        var howlerWolf = new RealPetActorRosterItem
        {
            EntityUid = "Howler_Wolf",
            EntityDisplayName = "Howler Wolf",
            EntityClassName = "Class Henchman Minion",
            SourceHistoryIndex = 1,
            SourcePowerFullName = "Beast_Mastery.Summon_Wolves.Summon_Wolves",
            SourcePowerDisplayName = "Summon Wolves",
            Count = 2
        };

        _headerIcon.IconImage = PetActorIconCatalog.GetIcon(alphaWolf);
        _headerIcon.FallbackText = alphaWolf.EntityDisplayName;
        _nameLabel.Text = alphaWolf.EntityDisplayName;
        _subTitleLabel.Text = $"Summoned from {alphaWolf.SourcePowerDisplayName}";
        _classLabel.Text = $"Actor Class: {alphaWolf.EntityClassName}";
        _countLabel.Text = "1 Active";
        _tagsLabel.Text = "Actor Tags: Beast | Detonator | FullHenchman | SetBonusShare";

        SetMetric("hp", "578");
        SetMetric("regen", "4.82 HP/s");
        SetMetric("recovery", "1.67 End/s");
        SetMetric("defense", "0% Smashing");
        SetMetric("resistance", "28.68% Smashing");

        _upgradeFlow.Controls.Clear();
        _upgradeFlow.Controls.Add(CreatePreviewToggle("Train Beasts", true, "Sample.TrainBeasts", UpgradeToggleOnCheckedChanged));
        _proximityFlow.Controls.Clear();
        _proximityFlow.Controls.Add(CreatePreviewToggle("In Range of Owner", true, "__pet_in_range__", PetInRangeToggleOnCheckedChanged));

        _actorRibbon.SetItems(
        [
            new PetActorRibbonItem
            {
                Actor = alphaWolf,
                Title = alphaWolf.EntityDisplayName,
                Subtitle = alphaWolf.SourcePowerDisplayName,
                IconImage = PetActorIconCatalog.GetIcon(alphaWolf),
                IsSelected = true
            },
            new PetActorRibbonItem
            {
                Actor = howlerWolf,
                Title = howlerWolf.EntityDisplayName,
                Subtitle = howlerWolf.SourcePowerDisplayName,
                IconImage = PetActorIconCatalog.GetIcon(howlerWolf),
                IsSelected = false
            }
        ]);

        _detailView.SetDesignTimeSample();
        _powerGrid.SetViewModel(BuildDesignTimePowerGridViewModel());
        _actorRibbon.Enabled = true;
        _powerGrid.Enabled = true;
        _detailView.Enabled = true;
        _summaryMetrics.Enabled = true;
        ShowContentState();
    }

    private static PetActorPowerGridViewModel BuildDesignTimePowerGridViewModel()
    {
        return new PetActorPowerGridViewModel
        {
            SelectedPowerIndex = 2,
            Sections =
            [
                new PetActorPowerSectionViewModel
                {
                    Title = "Beast Alpha Wolf",
                    Tiles =
                    [
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 0,
                            DisplayName = "Resistance",
                            FullName = "Sample.Resistance",
                            PowerKindLabel = "Auto",
                            IsAuto = true,
                            IsSelected = false,
                            BasePower = null!
                        },
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 1,
                            DisplayName = "Super Leap",
                            FullName = "Sample.SuperLeap",
                            PowerKindLabel = "Toggle",
                            IsAuto = false,
                            IsSelected = false,
                            BasePower = null!
                        },
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 2,
                            DisplayName = "Vicious Bite",
                            FullName = "Sample.ViciousBite",
                            PowerKindLabel = string.Empty,
                            IsAuto = false,
                            IsSelected = true,
                            BasePower = null!
                        },
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 3,
                            DisplayName = "Wild Charge",
                            FullName = "Sample.WildCharge",
                            PowerKindLabel = string.Empty,
                            IsAuto = false,
                            IsSelected = false,
                            BasePower = null!
                        }
                    ]
                },
                new PetActorPowerSectionViewModel
                {
                    Title = "Beast Alpha Wolf 2",
                    Subtitle = "Unlocked by Train Beasts",
                    Tiles =
                    [
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 4,
                            DisplayName = "Howl",
                            FullName = "Sample.Howl",
                            PowerKindLabel = string.Empty,
                            IsAuto = false,
                            IsSelected = false,
                            BasePower = null!
                        },
                        new PetActorPowerTileViewModel
                        {
                            PowerIndex = 5,
                            DisplayName = "Maiming Bite",
                            FullName = "Sample.MaimingBite",
                            PowerKindLabel = string.Empty,
                            IsAuto = false,
                            IsSelected = false,
                            BasePower = null!
                        }
                    ]
                }
            ]
        };
    }

    public void UpdateData(bool refresh = false)
    {
        UpdateData(_selectedEntityUid, _selectedSourceHistoryIndex);
    }

    public void UpdateData(string? entityUid, int sourceHistoryIndex = -1)
    {
        if (InDesigner)
        {
            ApplyDesignTimeSampleIfNeeded();
            return;
        }

        _selectedEntityUid = entityUid;
        _selectedSourceHistoryIndex = sourceHistoryIndex;

        _roster.Clear();
        _roster.AddRange(_toon.GetRealPetActorRoster());
        RebuildRibbon();

        var hasActors = _roster.Count > 0;
        _actorRibbon.Enabled = hasActors;
        _powerGrid.Enabled = hasActors;
        _detailView.Enabled = hasActors;
        _summaryMetrics.Enabled = hasActors;

        if (!hasActors)
        {
            ShowEmptyState();
            return;
        }

        var selected = ResolveSelectedActor(entityUid, sourceHistoryIndex) ?? _roster[0];
        SetSelectedActor(selected);
        LoadActorSnapshot(selected);
        ShowContentState();
    }

    public void UpdateColorTheme(Enums.Alignment alignment)
    {
        var theme = CurrentTheme;
        BackColor = theme.Background;
        ForeColor = theme.Text;
        _headerPanel.BackColor = theme.Card;
        _summaryMetrics.BackColor = theme.Background;
        _previewStatePanel.BackColor = theme.Background;
        _bodySplit.BackColor = theme.Background;
        _detailTotalsSplit.BackColor = theme.Background;
        _powerGridScrollPanel.BackColor = theme.Background;
        _powerGridScrollPanel.ContentPanel.BackColor = theme.Background;
        _totalsScrollPanel.BackColor = theme.Background;
        _totalsStack.BackColor = theme.Background;
        _emptyStatePanel.BackColor = theme.Background;
        _emptyLabel.ForeColor = theme.Text;
        _nameLabel.ForeColor = theme.ValueText;
        _countLabel.ForeColor = theme.ValueText;
        _subTitleLabel.ForeColor = Color.FromArgb(232, theme.Text);
        _classLabel.ForeColor = Color.FromArgb(245, 239, 225);
        _tagsLabel.ForeColor = theme.Muted;

        foreach (var panel in _summaryMetrics.Controls.OfType<Panel>())
        {
            panel.BackColor = theme.Card;
            ApplyShellLabelColors(panel, theme);
            panel.Invalidate();
        }

        foreach (var panel in _previewStatePanel.Controls.OfType<Panel>())
        {
            panel.BackColor = theme.Card;
            ApplyShellLabelColors(panel, theme);
            panel.Invalidate();
        }

        foreach (var flow in EnumerateControls(_previewStatePanel, new HashSet<Control>()).OfType<FlowLayoutPanel>())
        {
            flow.BackColor = Color.Transparent;
            flow.Invalidate();
        }

        foreach (var checkBox in EnumerateControls(_previewStatePanel, new HashSet<Control>()).OfType<CheckBox>())
        {
            checkBox.ForeColor = theme.Text;
            checkBox.BackColor = checkBox is PreviewStateToggle ? Color.Transparent : theme.Card;
            checkBox.Invalidate();
        }

        foreach (var label in EnumerateControls(_previewStatePanel, new HashSet<Control>()).OfType<Label>())
        {
            label.BackColor = Color.Transparent;
        }

        foreach (var label in _ribbonHost.Controls.OfType<Label>())
        {
            label.ForeColor = theme.Accent;
        }

        foreach (var label in _powerPanel.Controls.OfType<Label>())
        {
            label.ForeColor = theme.Accent;
        }

        foreach (var label in _totalsHost.Controls.OfType<Label>())
        {
            label.ForeColor = theme.Accent;
        }

        _actorRibbon.Invalidate();
        _powerGrid.Invalidate();
        _detailView.Invalidate(true);
        _headerIcon.Invalidate();
        _bodySplit.Panel1.Invalidate();
        _bodySplit.Panel2.Invalidate();
        ApplyWindowChrome();
        Invalidate(true);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (InDesigner)
        {
            return;
        }

        ApplyWindowChrome();
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        ApplyDesignTimeSampleIfNeeded();
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        if (InDesigner)
        {
            ApplyDesignTimeSampleIfNeeded();
            return;
        }

        RestoreWindowBounds();
        ConstrainToWorkingArea();

        if (MidsContext.Character != null)
        {
            MidsContext.Character.AlignmentChanged += CharacterOnAlignmentChanged;
        }

        ThemeManager.ThemeChanged += OnThemeChanged;
        UpdateColorTheme(MidsContext.Character?.Alignment ?? Enums.Alignment.Hero);
        ApplySplitLayout();
        ApplyResponsiveScale();
        Opacity = 0d;
        BeginInvoke(new Action(() =>
        {
            ApplySplitLayout();
            ApplyResponsiveScale();
            UpdateData(_selectedEntityUid, _selectedSourceHistoryIndex);
            QueueStartupReveal();
        }));
    }

    private void OnFormSizeChanged(object? sender, EventArgs e)
    {
        ApplySplitLayout();
        ApplyResponsiveScale();
    }

    private void OnFormClosed(object? sender, EventArgs e)
    {
        if (MidsContext.Config != null)
        {
            var boundsToPersist = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
            MidsContext.Config.PetActorDetailsLocation = boundsToPersist.Location;
            MidsContext.Config.PetActorDetailsSize = boundsToPersist.Size;
        }

        if (MidsContext.Character != null)
        {
            MidsContext.Character.AlignmentChanged -= CharacterOnAlignmentChanged;
        }

        ThemeManager.ThemeChanged -= OnThemeChanged;
    }

    private void QueueStartupReveal()
    {
        if (_startupRevealQueued || _startupRevealCompleted)
        {
            return;
        }

        _startupRevealQueued = true;
        BeginInvoke(new Action(() =>
        {
            if (IsDisposed || !IsHandleCreated || _startupRevealCompleted)
            {
                _startupRevealQueued = false;
                return;
            }

            _startupRevealQueued = false;
            _startupRevealCompleted = true;
            ApplySplitLayout();
            ApplyResponsiveScale();
            Invalidate(true);
            Update();
            Opacity = 1d;
        }));
    }

    private void OnThemeChanged()
    {
        UpdateColorTheme(MidsContext.Character?.Alignment ?? Enums.Alignment.Hero);
    }

    private void CharacterOnAlignmentChanged(object? sender, Enums.Alignment alignment)
    {
        UpdateColorTheme(alignment);
    }

    private void ApplyDesignTimeSampleIfNeeded()
    {
        if (!InDesigner || _designTimeSampleApplied || IsDisposed)
        {
            return;
        }

        _designTimeSampleApplied = true;
        UpdateColorTheme(Enums.Alignment.Hero);
        PopulateDesignTimeSample();
        ApplyResponsiveScale();
        ApplySplitLayout();
    }

    private void RestoreWindowBounds()
    {
        var screen = Owner != null ? Screen.FromControl(Owner) : Screen.FromPoint(Location);
        var workingArea = screen.WorkingArea;
        var config = MidsContext.Config;

        Size = config?.PetActorDetailsSize is { } savedSize
            ? savedSize
            : GetPreferredInitialSize(workingArea);

        if (config?.PetActorDetailsLocation is { } savedLocation)
        {
            Location = savedLocation;
            return;
        }

        if (Owner != null)
        {
            var ownerBounds = Owner.WindowState == FormWindowState.Normal
                ? Owner.Bounds
                : workingArea;
            Location = new Point(
                ownerBounds.Left + Math.Max(24, (ownerBounds.Width - Width) / 2),
                ownerBounds.Top + Math.Max(24, (ownerBounds.Height - Height) / 2));
            return;
        }

        Location = new Point(
            workingArea.Left + Math.Max(24, (workingArea.Width - Width) / 2),
            workingArea.Top + Math.Max(24, (workingArea.Height - Height) / 2));
    }

    private Size GetPreferredInitialSize(Rectangle workingArea)
    {
        var referenceBounds = Owner switch
        {
            { WindowState: FormWindowState.Normal } owner => owner.Bounds,
            { } => workingArea,
            null => workingArea
        };

        var preferredWidth = Math.Max(BaselineFormWidth, (int)Math.Round(referenceBounds.Width * 0.82f));
        var preferredHeight = Math.Max(BaselineFormHeight, (int)Math.Round(referenceBounds.Height * 0.84f));
        var maxWidth = Math.Max(MinimumSize.Width, workingArea.Width - 24);
        var maxHeight = Math.Max(MinimumSize.Height, workingArea.Height - 24);

        return new Size(
            Math.Clamp(preferredWidth, MinimumSize.Width, maxWidth),
            Math.Clamp(preferredHeight, MinimumSize.Height, maxHeight));
    }

    private void ConstrainToWorkingArea()
    {
        var screen = Owner != null ? Screen.FromControl(Owner) : Screen.FromPoint(Location);
        var workingArea = screen.WorkingArea;
        var width = Math.Min(Math.Max(MinimumSize.Width, Width), workingArea.Width);
        var height = Math.Min(Math.Max(MinimumSize.Height, Height), workingArea.Height);
        var x = Math.Max(workingArea.Left, Math.Min(Left, workingArea.Right - width));
        var y = Math.Max(workingArea.Top, Math.Min(Top, workingArea.Bottom - height));
        Bounds = new Rectangle(x, y, width, height);
    }

    private void ApplyWindowChrome()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        var theme = CurrentTheme;
        WinApi.SetDarkMode(Handle, true);
        WinApi.SetWindowCornerPreference(Handle, WinApi.CornerPreference.Round);
        WinApi.StylizeWindow(
            Handle,
            borderColor: theme.Border,
            captionColor: Color.FromArgb(12, 26, 44),
            textColor: Color.WhiteSmoke);
    }

    private void HeaderPanelOnPaint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        var theme = CurrentTheme;
        var bounds = _headerPanel.ClientRectangle;
        if (bounds.Width <= 1 || bounds.Height <= 1)
        {
            return;
        }

        g.SmoothingMode = SmoothingMode.AntiAlias;
        var outer = Rectangle.Inflate(bounds, -1, -1);
        DrawShellSurface(
            g,
            outer,
            Blend(theme.HeaderTop, Color.Black, 0.18f),
            Blend(theme.HeaderBottom, Color.Black, 0.20f),
            Blend(theme.Border, theme.Accent, 0.26f),
            Blend(theme.GridHeaderBorder, Color.White, 0.12f),
            ScalePx(12));

        var glossBounds = Rectangle.Inflate(outer, -1, -1);
        glossBounds.Height = Math.Max(8, glossBounds.Height / 3);
        using (var glossBrush = new LinearGradientBrush(
                   glossBounds,
                   Color.FromArgb(58, 255, 255, 255),
                   Color.FromArgb(0, 255, 255, 255),
                   LinearGradientMode.Vertical))
        using (var glossPath = CreateRoundedRect(glossBounds, ScalePx(10)))
        {
            g.FillPath(glossBrush, glossPath);
        }

        var dividerX = _headerIcon.Right + ScalePx(12);
        if (dividerX > outer.Left + ScalePx(40) && dividerX < outer.Right - ScalePx(40))
        {
            using var dividerPen = new Pen(Color.FromArgb(120, Blend(theme.Accent, Color.White, 0.24f)));
            g.DrawLine(dividerPen, dividerX, outer.Top + ScalePx(14), dividerX, outer.Bottom - ScalePx(14));
        }
    }

    private void BodyPaneOnPaint(object? sender, PaintEventArgs e)
    {
        if (sender is not Control pane)
        {
            return;
        }

        var theme = CurrentTheme;
        var bounds = Rectangle.Inflate(pane.ClientRectangle, -1, -1);
        if (bounds.Width <= 1 || bounds.Height <= 1)
        {
            return;
        }

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        DrawShellSurface(
            e.Graphics,
            bounds,
            Blend(theme.Card, theme.HeaderTop, 0.10f),
            Blend(theme.Card, Color.Black, 0.14f),
            Color.FromArgb(170, Blend(theme.Border, theme.Accent, 0.22f)),
            Color.FromArgb(84, Blend(theme.Accent, Color.White, 0.18f)),
            ScalePx(12));
    }

    private void ApplySplitLayout()
    {
        if (_bodySplit.IsDisposed)
        {
            return;
        }

        var available = _bodySplit.ClientSize.Width - _bodySplit.SplitterWidth;
        var panel1Min = ScaleMetric(500, 440);
        var panel2Min = ScaleMetric(520, 460);
        var preferredRightRatio = available <= ScaleMetric(1220, 1080)
            ? 0.50f
            : available <= ScaleMetric(1380, 1220)
                ? 0.48f
                : 0.46f;
        var preferredRightWidth = Math.Clamp((int)Math.Round(available * preferredRightRatio), panel2Min, ScaleMetric(820, 680));
        var preferredDistance = Math.Max(panel1Min, available - preferredRightWidth);
        ApplySplitLayout(_bodySplit, preferredDistance, panel1Min, panel2Min);
        _detailView.RefreshResponsiveLayout();
    }

    private static void ApplySplitLayout(SplitContainer split, int preferredDistance, int panel1Min, int panel2Min)
    {
        if (split.IsDisposed)
        {
            return;
        }

        var available = split.Orientation == Orientation.Vertical
            ? split.ClientSize.Width - split.SplitterWidth
            : split.ClientSize.Height - split.SplitterWidth;
        if (available <= 0)
        {
            return;
        }

        var clampedPanel1Min = Math.Max(0, Math.Min(panel1Min, available));
        var remainingForPanel2 = Math.Max(0, available - clampedPanel1Min);
        var clampedPanel2Min = Math.Max(0, Math.Min(panel2Min, remainingForPanel2));
        clampedPanel1Min = Math.Max(0, Math.Min(clampedPanel1Min, available - clampedPanel2Min));

        var minDistance = clampedPanel1Min;
        var maxDistance = Math.Max(minDistance, available - clampedPanel2Min);
        var clampedDistance = Math.Clamp(preferredDistance, minDistance, maxDistance);

        split.Panel1MinSize = 0;
        split.Panel2MinSize = 0;
        split.SplitterDistance = clampedDistance;
        split.Panel1MinSize = clampedPanel1Min;
        split.Panel2MinSize = clampedPanel2Min;
    }

    private void SummaryMetricPanelOnPaint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel)
        {
            return;
        }

        var theme = CurrentTheme;
        var bounds = panel.ClientRectangle;
        if (bounds.Width <= 1 || bounds.Height <= 1)
        {
            return;
        }

        var metadata = panel.Tag as ShellPanelMetadata;
        var accent = metadata?.AccentColor ?? theme.Accent;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        DrawShellSurface(
            e.Graphics,
            Rectangle.Inflate(bounds, -1, -1),
            Blend(theme.Card, theme.HeaderTop, 0.12f),
            Blend(theme.Card, Color.Black, 0.18f),
            Color.FromArgb(170, Blend(theme.Border, accent, 0.20f)),
            Color.FromArgb(82, Blend(accent, Color.White, 0.24f)),
            ScalePx(10),
            drawGloss: false);
    }

    private void AddMetric(string key, string title)
    {
        var theme = CurrentTheme;
        var badgeSpec = GetMetricBadgeSpec(key);
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 8, 0),
            Padding = new Padding(0),
            Tag = new ShellPanelMetadata(badgeSpec.AccentColor)
        };
        panel.Paint += SummaryMetricPanelOnPaint;

        var badge = CreateShellBadge(badgeSpec, 52);
        badge.Dock = DockStyle.Left;
        badge.Margin = Padding.Empty;

        var textHost = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(0, 8, 10, 8)
        };
        textHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        textHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        textHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var titleLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(232, theme.Text),
            Tag = "metric-title",
            Text = title,
            TextAlign = ContentAlignment.MiddleLeft
        };
        var valueLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(248, 244, 236),
            Tag = "metric-value",
            Text = "--",
            TextAlign = ContentAlignment.MiddleLeft
        };

        textHost.Controls.Add(titleLabel, 0, 0);
        textHost.Controls.Add(valueLabel, 0, 1);
        panel.Controls.Add(textHost);
        panel.Controls.Add(badge);
        _summaryMetrics.Controls.Add(panel);
        _metricLabels[key] = (titleLabel, valueLabel);
    }

    private static Label CreateSectionLabel(string text)
    {
        return new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            Text = text,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(2, 0, 0, 0)
        };
    }

    private static void ConfigureSectionLabel(Label label, string text)
    {
        label.Dock = DockStyle.Fill;
        label.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        label.Padding = new Padding(2, 0, 0, 0);
        label.Text = text;
        label.TextAlign = ContentAlignment.MiddleLeft;
    }

    private void ApplyResponsiveScale()
    {
        var scale = ResolveUiScale();
        if (Math.Abs(scale - _uiScale) < 0.01f)
        {
            _detailView.RefreshResponsiveLayout();
            return;
        }

        _uiScale = scale;

        SuspendLayout();

        _rootLayout.Padding = new Padding(ScalePx(8));
        _rootLayout.RowStyles[0].Height = ScaleMetric(112, 90);
        _rootLayout.RowStyles[1].Height = ScaleMetric(82, 66);
        _rootLayout.RowStyles[2].Height = ScaleMetric(80, 64);
        _rootLayout.RowStyles[3].Height = ScaleMetric(88, 72);

        _headerPanel.Margin = new Padding(0, 0, 0, ScalePx(8));
        _headerPanel.Padding = new Padding(ScalePx(16), ScalePx(12), ScalePx(16), ScalePx(12));
        _headerLayout.ColumnStyles[0].Width = ScaleMetric(136, 100);
        _headerLayout.ColumnStyles[2].Width = ScaleMetric(320, 220);
        _headerLayout.RowStyles[0].Height = ScaleMetric(34, 24);
        _headerLayout.RowStyles[1].Height = ScaleMetric(24, 18);
        _headerIcon.Margin = new Padding(0, 0, ScalePx(14), 0);

        _summaryMetrics.Margin = new Padding(0, 0, 0, ScalePx(8));
        _previewStatePanel.Margin = new Padding(0, 0, 0, ScalePx(8));
        _ribbonHost.Margin = new Padding(0, 0, 0, ScalePx(8));
        _ribbonHost.RowStyles[0].Height = ScaleMetric(24, 18);
        _bodySplit.SplitterWidth = ScaleMetric(12, 8);
        _bodySplit.Panel1.Padding = new Padding(ScalePx(3), ScalePx(2), ScalePx(5), ScalePx(3));
        _bodySplit.Panel2.Padding = new Padding(ScalePx(5), ScalePx(2), ScalePx(3), ScalePx(3));
        _powerPanel.RowStyles[0].Height = ScaleMetric(28, 20);
        _powerPanel.Padding = new Padding(ScalePx(10), ScalePx(6), ScalePx(10), ScalePx(10));

        foreach (var panel in _summaryMetrics.Controls.OfType<Panel>())
        {
            panel.Margin = new Padding(0, 0, ScalePx(8), 0);
            foreach (var badge in panel.Controls.OfType<ShellGlyphBadge>())
            {
                badge.Width = ScaleMetric(52, 40);
            }
        }

        foreach (var panel in _previewStatePanel.Controls.OfType<Panel>())
        {
            panel.Margin = new Padding(0, 0, ScalePx(8), 0);
            foreach (var badge in panel.Controls.OfType<ShellGlyphBadge>())
            {
                badge.Width = ScaleMetric(48, 38);
            }
        }

        ApplyFontScale(this, scale, _detailView, _actorRibbon, _powerGrid);

        _actorRibbon.UiScale = scale;
        _powerGrid.UiScale = scale;
        _detailView.UiScale = scale;
        _detailView.Margin = Padding.Empty;

        ResumeLayout(performLayout: true);
        Invalidate(true);
    }

    private float ResolveUiScale()
    {
        var widthRatio = Math.Max(0.01f, ClientSize.Width / (float)BaselineFormWidth);
        var heightRatio = Math.Max(0.01f, ClientSize.Height / (float)BaselineFormHeight);
        var blendedRatio = widthRatio * 0.70f + heightRatio * 0.30f;
        var rawScale = 1f + ((blendedRatio - 1f) * UiScaleIntensity);
        return Math.Clamp(rawScale, MinimumResponsiveUiScale, MaximumResponsiveUiScale);
    }

    private void ApplyFontScale(Control root, float scale, params Control[] skippedRoots)
    {
        var skipped = new HashSet<Control>(skippedRoots);
        foreach (var control in EnumerateControls(root, skipped))
        {
            if (control.Font is null)
            {
                continue;
            }

            if (!_baseFontSizes.TryGetValue(control, out var baseSize))
            {
                baseSize = control.Font.Size;
                _baseFontSizes[control] = baseSize;
            }

            var scaledSize = Math.Max(6f, baseSize * scale);
            if (Math.Abs(control.Font.Size - scaledSize) < 0.05f)
            {
                continue;
            }

            control.Font = new Font(
                control.Font.FontFamily,
                scaledSize,
                control.Font.Style,
                control.Font.Unit,
                control.Font.GdiCharSet,
                control.Font.GdiVerticalFont);
        }
    }

    private static IEnumerable<Control> EnumerateControls(Control root, ISet<Control> skipped)
    {
        yield return root;
        foreach (Control child in root.Controls)
        {
            if (skipped.Contains(child))
            {
                continue;
            }

            foreach (var descendant in EnumerateControls(child, skipped))
            {
                yield return descendant;
            }
        }
    }

    private int ScalePx(int value)
        => Math.Max(1, (int)Math.Round(value * _uiScale));

    private int ScaleMetric(int baseValue, int minimum)
        => Math.Max(minimum, (int)Math.Round(baseValue * _uiScale));

    private static ShellBadgeSpec GetMetricBadgeSpec(string key)
    {
        return key.ToLowerInvariant() switch
        {
            "hp" => new ShellBadgeSpec(MidsTotalsGlyph.SectionDefense, Color.FromArgb(74, 174, 255), Color.FromArgb(26, 82, 178)),
            "regen" => new ShellBadgeSpec(MidsTotalsGlyph.QuickRegen, Color.FromArgb(66, 242, 126), Color.FromArgb(16, 110, 62)),
            "recovery" => new ShellBadgeSpec(MidsTotalsGlyph.QuickRecharge, Color.FromArgb(255, 206, 72), Color.FromArgb(142, 88, 20)),
            "defense" => new ShellBadgeSpec(MidsTotalsGlyph.SectionDefense, Color.FromArgb(82, 204, 255), Color.FromArgb(22, 86, 168)),
            "resistance" => new ShellBadgeSpec(MidsTotalsGlyph.SectionResistance, Color.FromArgb(198, 112, 255), Color.FromArgb(86, 36, 126)),
            _ => new ShellBadgeSpec(MidsTotalsGlyph.SectionCore, Color.FromArgb(180, 198, 228), Color.FromArgb(48, 66, 98))
        };
    }

    private static ShellBadgeSpec GetPreviewBadgeSpec(string title)
    {
        return title.Equals("Pet Context", StringComparison.OrdinalIgnoreCase)
            ? new ShellBadgeSpec(MidsTotalsGlyph.StatToHit, Color.FromArgb(212, 220, 232), Color.FromArgb(44, 66, 94))
            : new ShellBadgeSpec(MidsTotalsGlyph.StatDamage, Color.FromArgb(206, 214, 224), Color.FromArgb(60, 66, 82));
    }

    private ShellGlyphBadge CreateShellBadge(ShellBadgeSpec spec, int baseSize)
    {
        return new ShellGlyphBadge
        {
            BadgeGlyph = spec.Glyph,
            AccentColor = spec.AccentColor,
            GlowColor = spec.GlowColor,
            BackColor = Color.Transparent,
            Width = baseSize,
            Margin = Padding.Empty
        };
    }

    private void ApplyShellLabelColors(Control root, DataViewTheme theme)
    {
        foreach (var label in EnumerateControls(root, new HashSet<Control>()).OfType<Label>())
        {
            switch (label.Tag as string)
            {
                case "metric-title":
                    label.ForeColor = Color.FromArgb(232, theme.Text);
                    break;
                case "metric-value":
                    label.ForeColor = Color.FromArgb(248, 244, 236);
                    break;
                case "group-title":
                    label.ForeColor = Color.FromArgb(236, theme.Text);
                    break;
            }
        }
    }

    private void DrawShellSurface(Graphics g, Rectangle bounds, Color topColor, Color bottomColor, Color borderColor, Color innerHighlight, int radius, bool drawGloss = true)
    {
        if (bounds.Width <= 1 || bounds.Height <= 1)
        {
            return;
        }

        using var outerPath = CreateRoundedRect(bounds, radius);
        using var fillBrush = new LinearGradientBrush(bounds, topColor, bottomColor, LinearGradientMode.Vertical);
        using var borderPen = new Pen(borderColor);
        g.FillPath(fillBrush, outerPath);
        g.DrawPath(borderPen, outerPath);

        var innerBounds = Rectangle.Inflate(bounds, -2, -2);
        if (innerBounds.Width > 2 && innerBounds.Height > 2)
        {
            using var innerPath = CreateRoundedRect(innerBounds, Math.Max(4, radius - 2));
            using var innerPen = new Pen(innerHighlight);
            g.DrawPath(innerPen, innerPath);

            if (drawGloss)
            {
                var glossBounds = new Rectangle(innerBounds.X, innerBounds.Y, innerBounds.Width, Math.Max(8, innerBounds.Height / 2));
                using var glossPath = CreateRoundedRect(glossBounds, Math.Max(4, radius - 2));
                using var glossBrush = new LinearGradientBrush(
                    glossBounds,
                    Color.FromArgb(44, 255, 255, 255),
                    Color.FromArgb(0, 255, 255, 255),
                    LinearGradientMode.Vertical);
                g.FillPath(glossBrush, glossPath);
            }
        }
    }

    private static GraphicsPath CreateRoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        var diameter = Math.Max(2, radius * 2);
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static Color Blend(Color first, Color second, float amountSecond)
    {
        amountSecond = Math.Clamp(amountSecond, 0f, 1f);
        var amountFirst = 1f - amountSecond;
        return Color.FromArgb(
            (int)Math.Round(first.A * amountFirst + second.A * amountSecond),
            (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
            (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
            (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
    }

    private Panel CreatePreviewStateGroup(string title, FlowLayoutPanel flow)
    {
        var theme = CurrentTheme;
        var badgeSpec = GetPreviewBadgeSpec(title);
        var host = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 8, 0),
            Padding = Padding.Empty,
            Tag = new ShellPanelMetadata(badgeSpec.AccentColor)
        };
        host.Paint += SummaryMetricPanelOnPaint;

        var badge = CreateShellBadge(badgeSpec, 48);
        badge.Dock = DockStyle.Left;
        badge.Margin = Padding.Empty;

        var contentHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = new Padding(0, 7, 10, 8)
        };

        var label = CreateSectionLabel(title);
        label.Dock = DockStyle.Top;
        label.Height = 18;
        label.Font = new Font("Segoe UI", 8.75f, FontStyle.Bold);
        label.ForeColor = Color.FromArgb(236, theme.Text);
        label.Tag = "group-title";

        flow.Dock = DockStyle.Fill;
        contentHost.Controls.Add(flow);
        contentHost.Controls.Add(label);
        host.Controls.Add(contentHost);
        host.Controls.Add(badge);
        return host;
    }

    private static FlowLayoutPanel CreatePreviewStateFlow()
    {
        return new ShellFlowLayoutPanel
        {
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0),
            Padding = new Padding(0, 4, 0, 0),
            BackColor = Color.Transparent
        };
    }

    private static void ConfigurePreviewStateFlow(FlowLayoutPanel flow)
    {
        flow.AutoScroll = true;
        flow.FlowDirection = FlowDirection.LeftToRight;
        flow.WrapContents = true;
        flow.Margin = new Padding(0);
        flow.Padding = new Padding(0, 4, 0, 0);
        flow.BackColor = Color.Transparent;
    }

    private static PowerStatsGrid CreateTotalsGrid()
    {
        return new PowerStatsGrid
        {
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 10),
            BackColor = Color.Black
        };
    }

    private static void ConfigureTotalsGrid(PowerStatsGrid grid)
    {
        grid.Dock = DockStyle.Top;
        grid.Margin = new Padding(0, 0, 0, 10);
        grid.BackColor = Color.Black;
    }

    private void AddTotalsSection(Label label, PowerStatsGrid grid)
    {
        label.Margin = new Padding(0, 0, 0, 4);
        grid.Margin = new Padding(0, 0, 0, 12);
        _totalsStack.Controls.Add(label);
        _totalsStack.Controls.Add(grid);
    }

    private void ActorRibbonOnItemSelected(object? sender, PetActorRibbonItem item)
    {
        SetSelectedActor(item.Actor);
        LoadActorSnapshot(item.Actor);
    }

    private void PowerGridOnPowerSelected(object? sender, PetActorPowerTileViewModel tile)
    {
        _selectedPowerIndex = tile.PowerIndex;
        RefreshPowerSurface();
    }

    private void RebuildRibbon()
    {
        var items = _roster
            .OrderBy(actor => actor.SourcePowerDisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(actor => actor.EntityDisplayName, StringComparer.OrdinalIgnoreCase)
            .Select(actor => new PetActorRibbonItem
            {
                Actor = actor,
                Title = actor.EntityDisplayName,
                Subtitle = actor.SourcePowerDisplayName,
                IconImage = PetActorIconCatalog.GetIcon(actor),
                IsSelected = IsSelectedActor(actor)
            })
            .ToArray();

        _actorRibbon.SetItems(items);
    }

    private RealPetActorRosterItem? ResolveSelectedActor(string? entityUid, int sourceHistoryIndex)
    {
        if (!string.IsNullOrWhiteSpace(entityUid))
        {
            var direct = _roster.FirstOrDefault(actor =>
                actor.EntityUid.Equals(entityUid, StringComparison.OrdinalIgnoreCase) &&
                (sourceHistoryIndex < 0 || actor.SourceHistoryIndex == sourceHistoryIndex));
            if (direct != null)
            {
                return direct;
            }
        }

        if (!string.IsNullOrWhiteSpace(_selectedEntityUid))
        {
            return _roster.FirstOrDefault(actor =>
                actor.EntityUid.Equals(_selectedEntityUid, StringComparison.OrdinalIgnoreCase) &&
                actor.SourceHistoryIndex == _selectedSourceHistoryIndex);
        }

        return null;
    }

    private bool IsSelectedActor(RealPetActorRosterItem actor)
    {
        return !string.IsNullOrWhiteSpace(_selectedEntityUid)
               && actor.EntityUid.Equals(_selectedEntityUid, StringComparison.OrdinalIgnoreCase)
               && actor.SourceHistoryIndex == _selectedSourceHistoryIndex;
    }

    private void SetSelectedActor(RealPetActorRosterItem actor)
    {
        _selectedEntityUid = actor.EntityUid;
        _selectedSourceHistoryIndex = actor.SourceHistoryIndex;
        RebuildRibbon();
    }

    private void LoadActorSnapshot(RealPetActorRosterItem actor, string? preserveSelectedPowerFullName = null)
    {
        var loadVersion = ++_snapshotLoadVersion;
        _bonusEntriesPending = true;
        _bonusLoadQueuedVersion = -1;
        _currentSnapshot = _toon.GeneratePetActorSnapshotForAnalysis(actor, GetPreviewState(actor));
        if (_currentSnapshot == null)
        {
            ShowEmptyState();
            return;
        }

        _previewStatesByActor[GetPreviewStateKey(actor)] = _currentSnapshot.PreviewState.Clone();
        _selectedPowerIndex = ResolveSelectedPowerIndex(_currentSnapshot, preserveSelectedPowerFullName);
        Text = $"Pet Actor Sheet - {_currentSnapshot.RosterItem.EntityDisplayName}";
        PopulateHeader();
        PopulateSummary();
        PopulatePreviewControls();
        RefreshPowerSurface();
        if (_detailView.IsBonusesTabActive)
        {
            QueueAppliedBonusLoad(loadVersion, actor);
        }
    }

    private void QueueAppliedBonusLoad(int loadVersion, RealPetActorRosterItem actor)
    {
        if (IsDisposed || !IsHandleCreated || _currentSnapshot == null || !_bonusEntriesPending)
        {
            return;
        }

        if (_bonusLoadQueuedVersion == loadVersion)
        {
            return;
        }

        _bonusLoadQueuedVersion = loadVersion;
        BeginInvoke(new Action(() =>
        {
            if (_bonusLoadQueuedVersion == loadVersion)
            {
                _bonusLoadQueuedVersion = -1;
            }

            if (IsDisposed || loadVersion != _snapshotLoadVersion || _currentSnapshot == null)
            {
                return;
            }

            if (!_currentSnapshot.RosterItem.EntityUid.Equals(actor.EntityUid, StringComparison.OrdinalIgnoreCase) ||
                _currentSnapshot.RosterItem.SourceHistoryIndex != actor.SourceHistoryIndex)
            {
                return;
            }

            Invalidate(true);
            Update();

            var entries = PetActorBonusAnalyzer.Build(
                _toon,
                _currentSnapshot.RosterItem,
                _currentSnapshot.Recipient,
                _currentSnapshot.Totals,
                _currentSnapshot.ResolvedPowers,
                _currentSnapshot.AvailableUpgrades,
                _currentSnapshot.PreviewState,
                _currentSnapshot.MathPowers,
                _currentSnapshot.BuffedPowers);

            if (IsDisposed || loadVersion != _snapshotLoadVersion || _currentSnapshot == null)
            {
                return;
            }

            _currentSnapshot.AppliedBonusEntries = entries;
            _detailView.SetBonusEntries(entries);
            _bonusEntriesPending = false;
        }));
    }

    private void DetailViewOnBonusesTabActivated(object? sender, EventArgs e)
    {
        if (_currentSnapshot == null || !_bonusEntriesPending)
        {
            return;
        }

        QueueAppliedBonusLoad(_snapshotLoadVersion, _currentSnapshot.RosterItem);
    }

    private static int ResolveDefaultPowerIndex(PetActorSnapshot snapshot)
    {
        for (var index = 0; index < snapshot.ResolvedPowers.Count; index++)
        {
            if (snapshot.ResolvedPowers[index].VisibleInGrid &&
                snapshot.ResolvedPowers[index].Power.PowerType != Enums.ePowerType.Auto_)
            {
                return index;
            }
        }

        for (var index = 0; index < snapshot.ResolvedPowers.Count; index++)
        {
            if (snapshot.ResolvedPowers[index].VisibleInGrid)
            {
                return index;
            }
        }

        return snapshot.ResolvedPowers.Count > 0 ? 0 : -1;
    }

    private static int ResolveSelectedPowerIndex(PetActorSnapshot snapshot, string? preferredPowerFullName)
    {
        if (!string.IsNullOrWhiteSpace(preferredPowerFullName))
        {
            var matchedIndex = snapshot.ResolvedPowers
                .Select((resolvedPower, index) => new { resolvedPower, index })
                .FirstOrDefault(entry => entry.resolvedPower.Power.FullName.Equals(preferredPowerFullName, StringComparison.OrdinalIgnoreCase))
                ?.index ?? -1;
            if (matchedIndex >= 0)
            {
                return matchedIndex;
            }
        }

        return ResolveDefaultPowerIndex(snapshot);
    }

    private void RefreshPowerSurface()
    {
        if (_currentSnapshot == null)
        {
            _powerGrid.SetViewModel(null);
            _detailView.Clear();
            return;
        }

        if (_selectedPowerIndex < 0 || _selectedPowerIndex >= _currentSnapshot.ResolvedPowers.Count)
        {
            _selectedPowerIndex = ResolveDefaultPowerIndex(_currentSnapshot);
        }

        var gridViewModel = PetActorSheetViewModelBuilder.Build(_currentSnapshot, _selectedPowerIndex);
        _powerGrid.SetViewModel(gridViewModel);

        if (_selectedPowerIndex < 0
            || _selectedPowerIndex >= _currentSnapshot.ResolvedPowers.Count
            || _selectedPowerIndex >= _currentSnapshot.BasePowers.Count
            || _selectedPowerIndex >= _currentSnapshot.BuffedPowers.Count)
        {
            _detailView.Clear();
            return;
        }

        _detailView.SetSnapshot(_currentSnapshot, _selectedPowerIndex);
    }

    private void PopulatePreviewControls()
    {
        _suppressPreviewStateEvents = true;
        try
        {
            _upgradeFlow.Controls.Clear();
            _proximityFlow.Controls.Clear();

            if (_currentSnapshot == null)
            {
                return;
            }

            if (_currentSnapshot.AvailableUpgrades.Count == 0)
            {
                _upgradeFlow.Controls.Add(CreatePreviewStatePlaceholder("No upgrade overlays available."));
            }
            else
            {
                foreach (var overlay in _currentSnapshot.AvailableUpgrades)
                {
                    var checkBox = CreatePreviewToggle(
                        overlay.UpgradePowerDisplayName,
                        _currentSnapshot.PreviewState.IsUpgradeApplied(overlay.UpgradePowerFullName),
                        overlay.UpgradePowerFullName,
                        UpgradeToggleOnCheckedChanged);
                    _upgradeFlow.Controls.Add(checkBox);
                }
            }

            var inRangeToggle = CreatePreviewToggle(
                "In Range of Owner",
                _currentSnapshot.PreviewState.InRange,
                "__pet_in_range__",
                PetInRangeToggleOnCheckedChanged);
            _proximityFlow.Controls.Add(inRangeToggle);
        }
        finally
        {
            _suppressPreviewStateEvents = false;
        }
    }

    private CheckBox CreatePreviewToggle(string text, bool isChecked, string fullName, EventHandler handler)
    {
        var theme = CurrentTheme;
        var checkBox = new PreviewStateToggle
        {
            Checked = isChecked,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = theme.Text,
            BackColor = Color.Transparent,
            Margin = new Padding(0, 0, 18, 4),
            Tag = fullName,
            Text = text,
            UseVisualStyleBackColor = false
        };
        checkBox.CheckedChanged += handler;
        return checkBox;
    }

    private static Label CreatePreviewStatePlaceholder(string text)
    {
        return new Label
        {
            AutoSize = true,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
            ForeColor = Color.Silver,
            Margin = new Padding(0, 2, 0, 0),
            Text = text
        };
    }

    private void UpgradeToggleOnCheckedChanged(object? sender, EventArgs e)
    {
        if (_suppressPreviewStateEvents || _currentSnapshot == null || sender is not CheckBox checkBox || checkBox.Tag is not string fullName)
        {
            return;
        }

        var state = GetMutablePreviewState(_currentSnapshot.RosterItem);
        if (checkBox.Checked)
        {
            state.Upgrades.AppliedUpgradePowerFullNames.Add(fullName);
        }
        else
        {
            state.Upgrades.AppliedUpgradePowerFullNames.Remove(fullName);
        }

        QueueRefreshCurrentActorSnapshot();
    }

    private void PetInRangeToggleOnCheckedChanged(object? sender, EventArgs e)
    {
        if (_suppressPreviewStateEvents || _currentSnapshot == null || sender is not CheckBox checkBox)
        {
            return;
        }

        var state = GetMutablePreviewState(_currentSnapshot.RosterItem);
        state.InRange = checkBox.Checked;
        _currentSnapshot.PreviewState.InRange = checkBox.Checked;
        QueueRefreshCurrentActorSnapshot();
    }

    private void RefreshCurrentActorSnapshot()
    {
        if (_currentSnapshot == null)
        {
            return;
        }

        var selectedPowerFullName = _selectedPowerIndex >= 0 && _selectedPowerIndex < _currentSnapshot.ResolvedPowers.Count
            ? _currentSnapshot.ResolvedPowers[_selectedPowerIndex].Power.FullName
            : null;
        LoadActorSnapshot(_currentSnapshot.RosterItem, selectedPowerFullName);
    }

    private void QueueRefreshCurrentActorSnapshot(bool refreshPowerSurfaceFirst = false)
    {
        if (_currentSnapshot == null || IsDisposed || !IsHandleCreated)
        {
            return;
        }

        if (refreshPowerSurfaceFirst)
        {
            RefreshPowerSurface();
            Invalidate(true);
            Update();
        }

        if (_snapshotRefreshQueued)
        {
            return;
        }

        _snapshotRefreshQueued = true;
        BeginInvoke(new Action(() =>
        {
            _snapshotRefreshQueued = false;
            if (IsDisposed || !IsHandleCreated || _currentSnapshot == null)
            {
                return;
            }

            RefreshCurrentActorSnapshot();
        }));
    }

    private void PowerGridOnPreviewToggleClicked(object? sender, PetActorPowerTileViewModel tile)
    {
        if (_currentSnapshot == null || !tile.CanTogglePreview)
        {
            return;
        }

        var state = GetMutablePreviewState(_currentSnapshot.RosterItem);
        if (state.ClickBuffs.IncludedPetSelfClickFullNames.Contains(tile.FullName))
        {
            state.ClickBuffs.IncludedPetSelfClickFullNames.Remove(tile.FullName);
        }
        else
        {
            state.ClickBuffs.IncludedPetSelfClickFullNames.Add(tile.FullName);
        }

        if (_currentSnapshot.PreviewState.IsPetClickBuffIncluded(tile.FullName))
        {
            _currentSnapshot.PreviewState.ClickBuffs.IncludedPetSelfClickFullNames.Remove(tile.FullName);
        }
        else
        {
            _currentSnapshot.PreviewState.ClickBuffs.IncludedPetSelfClickFullNames.Add(tile.FullName);
        }

        QueueRefreshCurrentActorSnapshot(refreshPowerSurfaceFirst: true);
    }

    private PetActorPreviewState GetPreviewState(RealPetActorRosterItem actor)
    {
        return _previewStatesByActor.TryGetValue(GetPreviewStateKey(actor), out var state)
            ? state.Clone()
            : new PetActorPreviewState();
    }

    private PetActorPreviewState GetMutablePreviewState(RealPetActorRosterItem actor)
    {
        var key = GetPreviewStateKey(actor);
        if (!_previewStatesByActor.TryGetValue(key, out var state))
        {
            state = new PetActorPreviewState();
            _previewStatesByActor[key] = state;
        }

        return state;
    }

    private static string GetPreviewStateKey(RealPetActorRosterItem actor)
    {
        return $"{actor.EntityUid}|{actor.SourceHistoryIndex}";
    }

    private void PopulateHeader()
    {
        if (_currentSnapshot == null)
        {
            return;
        }

        var roster = _currentSnapshot.RosterItem;
        _headerIcon.IconImage = PetActorIconCatalog.GetIcon(roster);
        _headerIcon.FallbackText = roster.EntityDisplayName;
        _nameLabel.Text = roster.EntityDisplayName;
        _subTitleLabel.Text = $"Summoned from {roster.SourcePowerDisplayName}";
        _classLabel.Text = $"Actor Class: {FormatTokenizedName(roster.EntityClassName)}";
        _countLabel.Text = roster.Count > 1 ? $"{roster.Count} Active" : "1 Active";

        var tags = _currentSnapshot.ActorTags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(FormatTokenizedName)
            .OrderBy(tag => tag)
            .ToArray();

        _tagsLabel.Text = tags.Length > 0
            ? $"Actor Tags: {string.Join(" | ", tags)}"
            : "Actor Tags: None";
    }

    private void PopulateSummary()
    {
        if (_currentSnapshot == null)
        {
            ClearMetrics();
            return;
        }

        var stats = _currentSnapshot.Totals.DisplayStats;
        var (bestDefenseName, bestDefenseValue) = GetPeakDamageValue(damageType => stats.Defense((int)damageType));
        var (bestResName, bestResValue) = GetPeakDamageValue(damageType => stats.DamageResistance((int)damageType, false));

        SetMetric("hp", $"{stats.HealthHitpointsNumeric(false):0.##}");
        SetMetric("regen", $"{stats.HealthRegenHPPerSec(false):0.##} HP/s");
        SetMetric("recovery", $"{stats.GetEnduranceRecoveryNumeric(false):0.##} End/s");
        SetMetric("defense", $"{bestDefenseValue:0.##}% {bestDefenseName}");
        SetMetric("resistance", $"{bestResValue:0.##}% {bestResName}");
    }

    private void PopulateTotals()
    {
        if (_currentSnapshot == null)
        {
            ClearTotals();
            return;
        }

        var stats = _currentSnapshot.Totals.DisplayStats;
        var speedFormat = MidsContext.Config?.SpeedFormat ?? Enums.eSpeedMeasure.MilesPerHour;

        var coreRows = new[]
        {
            new PowerStatsGrid.Row("Hit Points", stats.HealthHitpointsNumeric(false), stats.HealthHitpointsNumeric(false), string.Empty, true, hideGainPercent: true),
            new PowerStatsGrid.Row("Absorb", stats.Absorb, stats.Absorb, string.Empty, true, hideGainPercent: true),
            new PowerStatsGrid.Row("Regen", stats.HealthRegenHPPerSec(false), stats.HealthRegenHPPerSec(false), " HP/s", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Recovery", stats.GetEnduranceRecoveryNumeric(false), stats.GetEnduranceRecoveryNumeric(false), " End/s", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Net End", stats.GetEnduranceRecoveryNet(false), stats.GetEnduranceRecoveryNet(false), " End/s", true, hideGainPercent: true),
            new PowerStatsGrid.Row("End Use", stats.EnduranceUsage, stats.EnduranceUsage, " End/s", false, hideGainPercent: true),
        };

        var combatRows = new[]
        {
            new PowerStatsGrid.Row("ToHit", stats.BuffToHit, stats.BuffToHit, "%", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Accuracy", stats.BuffAccuracy, stats.BuffAccuracy, "%", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Damage", stats.BuffDamage(false) - 100f, stats.BuffDamage(false) - 100f, "%", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Recharge", _currentSnapshot.Totals.Totals.BuffHaste * 100f, _currentSnapshot.Totals.Totals.BuffHaste * 100f, "%", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Range", stats.RangePercent, stats.RangePercent, "%", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Threat", stats.ThreatLevel, stats.ThreatLevel, "%", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Perception", stats.Perception(false), stats.Perception(false), " ft", true, hideGainPercent: true),
        };

        var movementRows = new[]
        {
            new PowerStatsGrid.Row("Run Speed", stats.MovementRunSpeed(speedFormat, false), stats.MovementRunSpeed(speedFormat, false), $" {GetSpeedUnit(speedFormat)}", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Fly Speed", stats.MovementFlySpeed(speedFormat, false), stats.MovementFlySpeed(speedFormat, false), $" {GetSpeedUnit(speedFormat)}", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Jump Speed", stats.MovementJumpSpeed(speedFormat, false), stats.MovementJumpSpeed(speedFormat, false), $" {GetSpeedUnit(speedFormat)}", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Jump Height", stats.MovementJumpHeight(speedFormat), stats.MovementJumpHeight(speedFormat), $" {GetDistanceUnit(speedFormat)}", true, hideGainPercent: true),
        };

        var defenseRows = GetTrackedDamageTypes()
            .SelectMany(pair => new[]
            {
                new PowerStatsGrid.Row($"{pair.Label} Defense", stats.Defense((int)pair.DamageType), stats.Defense((int)pair.DamageType), "%", true, hideGainPercent: true),
                new PowerStatsGrid.Row($"{pair.Label} Resistance", stats.DamageResistance((int)pair.DamageType, false), stats.DamageResistance((int)pair.DamageType, false), "%", true, hideGainPercent: true)
            })
            .ToArray();

        var statusRows = new List<PowerStatsGrid.Row>();
        AddIndexedRows(statusRows, "Protection", _currentSnapshot.Totals.Totals.Mez, Enum.GetNames<Enums.eMez>(), string.Empty);
        AddIndexedRows(statusRows, "Mez Resist", _currentSnapshot.Totals.Totals.MezRes, Enum.GetNames<Enums.eMez>(), "%");
        AddIndexedRows(statusRows, "Debuff Resist", _currentSnapshot.Totals.Totals.DebuffRes, Enum.GetNames<Enums.eEffectType>(), "%");

        SetGridRows(_coreLabel, _coreGrid, coreRows);
        SetGridRows(_combatLabel, _combatGrid, combatRows);
        SetGridRows(_movementLabel, _movementGrid, movementRows);
        SetGridRows(_defenseLabel, _defenseGrid, defenseRows);
        SetGridRows(_statusLabel, _statusGrid, statusRows);
    }

    private void ShowEmptyState()
    {
        if (InDesigner && _designTimeSampleApplied)
        {
            ShowContentState();
            return;
        }

        _currentSnapshot = null;
        _selectedPowerIndex = -1;
        _bonusEntriesPending = false;
        _bonusLoadQueuedVersion = -1;
        _emptyStatePanel.Visible = true;
        _bodySplit.Visible = false;
        _nameLabel.Text = "No real pet actors";
        _subTitleLabel.Text = "This build does not currently have any persistent summon actors.";
        _classLabel.Text = string.Empty;
        _countLabel.Text = string.Empty;
        _tagsLabel.Text = string.Empty;
        _headerIcon.IconImage = null;
        _headerIcon.FallbackText = "Pet";
        _actorRibbon.SetItems(Array.Empty<PetActorRibbonItem>());
        _upgradeFlow.Controls.Clear();
        _powerGrid.SetViewModel(null);
        _detailView.Clear();
        ClearMetrics();
        ClearTotals();
    }

    private void ShowContentState()
    {
        _emptyStatePanel.Visible = false;
        _bodySplit.Visible = true;
    }

    private void ClearMetrics()
    {
        foreach (var (_, labels) in _metricLabels)
        {
            labels.Value.Text = "--";
        }
    }

    private void SetMetric(string key, string value)
    {
        if (_metricLabels.TryGetValue(key, out var labels))
        {
            labels.Value.Text = value;
        }
    }

    private void ClearTotals()
    {
        SetGridRows(_coreLabel, _coreGrid, Array.Empty<PowerStatsGrid.Row>());
        SetGridRows(_combatLabel, _combatGrid, Array.Empty<PowerStatsGrid.Row>());
        SetGridRows(_movementLabel, _movementGrid, Array.Empty<PowerStatsGrid.Row>());
        SetGridRows(_defenseLabel, _defenseGrid, Array.Empty<PowerStatsGrid.Row>());
        SetGridRows(_statusLabel, _statusGrid, Array.Empty<PowerStatsGrid.Row>());
    }

    private void SetGridRows(Label sectionLabel, PowerStatsGrid grid, IEnumerable<PowerStatsGrid.Row> rows)
    {
        var rowArray = rows.ToArray();
        sectionLabel.Visible = rowArray.Length > 0;
        grid.Visible = rowArray.Length > 0;
        grid.BackColor = CurrentTheme.Background;
        grid.Height = 1;
        if (rowArray.Length == 0)
        {
            grid.Clear();
            return;
        }

        grid.SetRows(rowArray);
    }

    private static void AddIndexedRows(List<PowerStatsGrid.Row> rows, string prefix, IReadOnlyList<float> values, IReadOnlyList<string> names, string suffix)
    {
        for (var index = 0; index < values.Count && index < names.Count; index++)
        {
            if (Math.Abs(values[index]) < float.Epsilon || string.Equals(names[index], "None", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            rows.Add(new PowerStatsGrid.Row($"{prefix}: {FormatTokenizedName(names[index])}", values[index], values[index], suffix, true, hideGainPercent: true));
        }
    }

    private static string FormatTokenizedName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Replace('_', ' ').Trim();
    }

    private static string GetSpeedUnit(Enums.eSpeedMeasure measure)
    {
        return measure switch
        {
            Enums.eSpeedMeasure.FeetPerSecond => "ft/s",
            Enums.eSpeedMeasure.MetersPerSecond => "m/s",
            Enums.eSpeedMeasure.KilometersPerHour => "km/h",
            _ => "mph"
        };
    }

    private static string GetDistanceUnit(Enums.eSpeedMeasure measure)
    {
        return measure is Enums.eSpeedMeasure.MetersPerSecond or Enums.eSpeedMeasure.KilometersPerHour
            ? "m"
            : "ft";
    }

    private static IEnumerable<(string Label, Enums.eDamage DamageType)> GetTrackedDamageTypes()
    {
        var trackedNames = new[]
        {
            "Smashing",
            "Lethal",
            "Fire",
            "Cold",
            "Energy",
            "Negative",
            "Toxic",
            "Psionic"
        };

        foreach (var trackedName in trackedNames)
        {
            if (Enum.TryParse<Enums.eDamage>(trackedName, out var damageType))
            {
                yield return (trackedName, damageType);
            }
        }
    }

    private (string Label, float Value) GetPeakDamageValue(Func<Enums.eDamage, float> selector)
    {
        var bestLabel = "Smashing";
        var bestValue = float.MinValue;

        foreach (var pair in GetTrackedDamageTypes())
        {
            var value = selector(pair.DamageType);
            if (value <= bestValue)
            {
                continue;
            }

            bestValue = value;
            bestLabel = pair.Label;
        }

        return (bestLabel, bestValue == float.MinValue ? 0f : bestValue);
    }

    private sealed class ShellGlyphBadge : Control
    {
        public MidsTotalsGlyph BadgeGlyph { get; init; }
        public Color AccentColor { get; init; } = Color.WhiteSmoke;
        public Color GlowColor { get; init; } = Color.FromArgb(36, 52, 78);

        public ShellGlyphBadge()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            Dock = DockStyle.Left;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var outerInset = Math.Max(6, (int)Math.Round(Math.Min(ClientRectangle.Width, ClientRectangle.Height) * 0.12f));
            var paddedBounds = Rectangle.Inflate(ClientRectangle, -outerInset, -outerInset);
            var badgeSize = Math.Min(paddedBounds.Width, paddedBounds.Height);
            if (badgeSize <= 2)
            {
                return;
            }

            var bounds = new Rectangle(
                paddedBounds.X + (paddedBounds.Width - badgeSize) / 2,
                paddedBounds.Y + (paddedBounds.Height - badgeSize) / 2,
                badgeSize,
                badgeSize);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            using var outerPath = new GraphicsPath();
            outerPath.AddEllipse(bounds);

            using var ringBrush = new LinearGradientBrush(bounds, Blend(GlowColor, Color.White, 0.10f), Blend(GlowColor, Color.Black, 0.28f), LinearGradientMode.Vertical);
            using var ringPen = new Pen(Color.FromArgb(180, Blend(AccentColor, Color.White, 0.10f)));
            e.Graphics.FillPath(ringBrush, outerPath);
            e.Graphics.DrawPath(ringPen, outerPath);

            var innerBounds = Rectangle.Inflate(bounds, -3, -3);
            using var innerPath = new GraphicsPath();
            innerPath.AddEllipse(innerBounds);
            using var innerBrush = new LinearGradientBrush(innerBounds, Blend(AccentColor, Color.White, 0.18f), GlowColor, LinearGradientMode.Vertical);
            using var innerPen = new Pen(Color.FromArgb(150, Blend(AccentColor, Color.Black, 0.18f)));
            e.Graphics.FillPath(innerBrush, innerPath);
            e.Graphics.DrawPath(innerPen, innerPath);

            var glyphPadding = Math.Max(2, innerBounds.Width / 9);
            var glyphBounds = Rectangle.Inflate(innerBounds, -glyphPadding, -glyphPadding);
            MidsTotalsIconCache.Draw(e.Graphics, glyphBounds, BadgeGlyph, Color.FromArgb(248, 248, 244));
        }
    }

    private sealed class ShellFlowLayoutPanel : FlowLayoutPanel
    {
        public ShellFlowLayoutPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            PaintParentBackground(e.Graphics);
        }

        private void PaintParentBackground(Graphics g)
        {
            if (Parent == null)
            {
                g.Clear(Color.Transparent);
                return;
            }

            var state = g.Save();
            try
            {
                g.TranslateTransform(-Left, -Top);
                using var paintArgs = new PaintEventArgs(g, new Rectangle(Parent.Location, Parent.Size));
                InvokePaintBackground(Parent, paintArgs);
                InvokePaint(Parent, paintArgs);
            }
            finally
            {
                g.Restore(state);
            }
        }
    }

    private sealed class PreviewStateToggle : CheckBox
    {
        public PreviewStateToggle()
        {
            AutoSize = true;
            BackColor = Color.Transparent;
            FlatStyle = FlatStyle.Flat;
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var textSize = TextRenderer.MeasureText(Text ?? string.Empty, Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            return new Size(textSize.Width + 34, Math.Max(22, textSize.Height + 4));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            PaintParentBackground(e.Graphics);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var indicatorSize = Math.Max(14, Height - 6);
            var indicatorBounds = new Rectangle(1, (Height - indicatorSize) / 2, indicatorSize, indicatorSize);
            using var outerBrush = new LinearGradientBrush(indicatorBounds, Color.FromArgb(18, 34, 54), Color.FromArgb(6, 12, 20), LinearGradientMode.Vertical);
            using var outerPen = new Pen(Color.FromArgb(148, 48, 72, 98));
            e.Graphics.FillEllipse(outerBrush, indicatorBounds);
            e.Graphics.DrawEllipse(outerPen, indicatorBounds);

            var fillBounds = Rectangle.Inflate(indicatorBounds, -2, -2);
            var onColor = Color.FromArgb(54, 214, 92);
            var offColor = Color.FromArgb(72, 78, 88);
            using var fillBrush = new LinearGradientBrush(
                fillBounds,
                Checked ? Blend(onColor, Color.White, 0.14f) : Blend(offColor, Color.White, 0.08f),
                Checked ? Blend(onColor, Color.Black, 0.16f) : Blend(offColor, Color.Black, 0.18f),
                LinearGradientMode.Vertical);
            e.Graphics.FillEllipse(fillBrush, fillBounds);

            if (Checked)
            {
                using var checkPen = new Pen(Color.FromArgb(248, 250, 244), Math.Max(1.6f, indicatorSize / 6f))
                {
                    StartCap = LineCap.Round,
                    EndCap = LineCap.Round,
                    LineJoin = LineJoin.Round
                };
                var a = new PointF(fillBounds.Left + fillBounds.Width * 0.22f, fillBounds.Top + fillBounds.Height * 0.56f);
                var b = new PointF(fillBounds.Left + fillBounds.Width * 0.44f, fillBounds.Top + fillBounds.Height * 0.76f);
                var c = new PointF(fillBounds.Left + fillBounds.Width * 0.78f, fillBounds.Top + fillBounds.Height * 0.28f);
                e.Graphics.DrawLines(checkPen, [a, b, c]);
            }

            var textBounds = new Rectangle(indicatorBounds.Right + 8, 0, Width - indicatorBounds.Right - 8, Height);
            TextRenderer.DrawText(
                e.Graphics,
                Text ?? string.Empty,
                Font,
                textBounds,
                Enabled ? ForeColor : Color.Gray,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);
        }

        private void PaintParentBackground(Graphics g)
        {
            if (Parent == null)
            {
                g.Clear(Color.Transparent);
                return;
            }

            var state = g.Save();
            try
            {
                g.TranslateTransform(-Left, -Top);
                using var paintArgs = new PaintEventArgs(g, new Rectangle(Parent.Location, Parent.Size));
                InvokePaintBackground(Parent, paintArgs);
                InvokePaint(Parent, paintArgs);
            }
            finally
            {
                g.Restore(state);
            }
        }
    }
}

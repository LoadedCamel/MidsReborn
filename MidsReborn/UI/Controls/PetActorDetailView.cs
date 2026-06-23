using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Net;
using System.Text.RegularExpressions;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls.Test;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

[DesignerCategory("Code")]
public sealed class PetActorDetailView : UserControl
{
    private const float MinimumResponsiveUiScale = 0.90f;
    private const int BaseTabRowHeight = 40;
    private const int BaseContentInset = 4;
    private const int BaseSectionGap = 8;
    private const int BaseDamageHeight = 124;

    private enum DetailTab
    {
        Info,
        Effects,
        Totals,
        Bonuses
    }

    private readonly TableLayoutPanel _rootLayout;
    private readonly TableLayoutPanel _tabStrip;
    private readonly Dictionary<DetailTab, PetActorTabButton> _tabButtons = new();
    private readonly Panel _pageHost;
    private readonly MidsVScrollPanel _infoScrollPanel;
    private readonly Panel _infoStackHost;
    private readonly MidsTotalsSectionPanel _overviewSection;
    private readonly MidsTotalsSectionPanel _statsSection;
    private readonly MidsTotalsSectionPanel _damageSection;
    private readonly TableLayoutPanel _overviewContent;
    private readonly Label _descriptionLabel;
    private readonly PowerStatsGrid _statsGrid;
    private readonly FixedHeightHost _damageHost;
    private readonly ModernDamageDisplay _damageDisplay;
    private readonly PowerEffectsGrid _effectsGrid;
    private readonly MidsVScrollPanel _totalsScrollPanel;
    private readonly Panel _totalsStackHost;
    private readonly MidsTotalsQuickStrip _quickReadStrip;
    private readonly MidsTotalsBarList _defenseBarListLeft;
    private readonly MidsTotalsBarList _defenseBarListRight;
    private readonly MidsTotalsBarList _resistanceBarListLeft;
    private readonly MidsTotalsBarList _resistanceBarListRight;
    private readonly MidsTotalsDualColumnHost _defenseListsHost;
    private readonly MidsTotalsDualColumnHost _resistanceListsHost;
    private readonly MidsTotalsValueGrid _coreMiscGrid;
    private readonly MidsTotalsSectionPanel _quickReadSection;
    private readonly MidsTotalsSectionPanel _defenseSection;
    private readonly MidsTotalsSectionPanel _resistanceSection;
    private readonly MidsTotalsSectionPanel _coreMiscSection;
    private readonly MidsVScrollPanel _bonusesScrollPanel;
    private readonly PetActorBonusesView _bonusesView;
    private readonly Dictionary<Control, float> _baseFontSizes = new();

    private PetActorSnapshot? _snapshot;
    private IPower? _basePower;
    private IPower? _enhancedPower;
    private DetailTab _activeTab = DetailTab.Info;
    private int _selectedHistoryIndex = -1;
    private float _uiScale = 1f;
    private float _requestedUiScale = 1f;
    private bool _applyingUiScale;

    public event EventHandler? BonusesTabActivated;

    public bool IsBonusesTabActive => _activeTab == DetailTab.Bonuses;

    public PetActorDetailView()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint,
            true);

        BackColor = Color.Black;
        Margin = Padding.Empty;

        _rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44f));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        _tabStrip = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            Margin = new Padding(0, 0, 0, 8),
            Padding = Padding.Empty,
            BackColor = Color.Transparent
        };
        for (var index = 0; index < 4; index++)
        {
            _tabStrip.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        }

        AddTabButton(DetailTab.Info, "Info");
        AddTabButton(DetailTab.Effects, "Effects");
        AddTabButton(DetailTab.Totals, "Totals");
        AddTabButton(DetailTab.Bonuses, "Bonuses");

        _pageHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Margin = Padding.Empty
        };

        _infoScrollPanel = new MidsVScrollPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Margin = Padding.Empty
        };
        _infoStackHost = new Panel
        {
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            Location = Point.Empty,
            Size = _infoScrollPanel.ContentPanel.ClientSize
        };
        _infoScrollPanel.ContentPanel.Controls.Add(_infoStackHost);
        _infoScrollPanel.AvailableClientWidthChanged += (_, _) => LayoutInfoSections();
        _infoScrollPanel.ContentPanel.SizeChanged += (_, _) => LayoutInfoSections();

        _overviewContent = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 1,
            Dock = DockStyle.Top,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        _overviewContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        _descriptionLabel = CreateOverviewLabel(9f, FontStyle.Regular);
        _overviewContent.Controls.Add(_descriptionLabel, 0, 0);

        _overviewSection = new MidsTotalsSectionPanel
        {
            Title = "Overview",
            ContentControl = _overviewContent
        };

        _statsGrid = new PowerStatsGrid
        {
            Dock = DockStyle.Top,
            Margin = Padding.Empty,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", 10.25f, FontStyle.Regular, GraphicsUnit.Point)
        };
        _statsSection = new MidsTotalsSectionPanel
        {
            Title = "Power Stats",
            TitleIcon = MidsTotalsGlyph.SectionCore,
            TitleIconColor = Color.FromArgb(235, 210, 120),
            ContentControl = _statsGrid
        };

        _damageDisplay = new ModernDamageDisplay
        {
            Dock = DockStyle.Fill,
            UseCompactCard = true,
            ShowGraph = true,
            Margin = Padding.Empty,
            MinimumSize = new Size(0, 168),
            Size = new Size(240, 168)
        };
        _damageHost = new FixedHeightHost(_damageDisplay, 168);
        _damageSection = new MidsTotalsSectionPanel
        {
            Title = "Damage",
            ContentControl = _damageHost
        };
        _infoStackHost.Controls.Add(_overviewSection);
        _infoStackHost.Controls.Add(_statsSection);
        _infoStackHost.Controls.Add(_damageSection);

        _effectsGrid = new PowerEffectsGrid
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = Color.Transparent,
            GroupHeaderHeight = 30,
            DescriptorRowHeight = 38
        };

        _totalsScrollPanel = new MidsVScrollPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Margin = Padding.Empty
        };
        _totalsStackHost = new Panel
        {
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            Location = Point.Empty,
            Size = _totalsScrollPanel.ContentPanel.ClientSize
        };
        _totalsScrollPanel.ContentPanel.Controls.Add(_totalsStackHost);
        _totalsScrollPanel.AvailableClientWidthChanged += (_, _) => LayoutTotalsSections();
        _totalsScrollPanel.ContentPanel.SizeChanged += (_, _) => LayoutTotalsSections();

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

        _totalsStackHost.Controls.Add(_quickReadSection);
        _totalsStackHost.Controls.Add(_defenseSection);
        _totalsStackHost.Controls.Add(_resistanceSection);
        _totalsStackHost.Controls.Add(_coreMiscSection);

        _bonusesScrollPanel = new MidsVScrollPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Margin = Padding.Empty
        };
        _bonusesView = new PetActorBonusesView
        {
            Dock = DockStyle.Top,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };
        _bonusesScrollPanel.ContentPanel.Controls.Add(_bonusesView);
        _bonusesScrollPanel.AvailableClientWidthChanged += (_, _) => LayoutBonusesSection();
        _bonusesScrollPanel.ContentPanel.SizeChanged += (_, _) => LayoutBonusesSection();

        _pageHost.Controls.Add(_bonusesScrollPanel);
        _pageHost.Controls.Add(_totalsScrollPanel);
        _pageHost.Controls.Add(_effectsGrid);
        _pageHost.Controls.Add(_infoScrollPanel);

        _rootLayout.Controls.Add(_tabStrip, 0, 0);
        _rootLayout.Controls.Add(_pageHost, 0, 1);
        Controls.Add(_rootLayout);

        SizeChanged += OnHostSizeChanged;
        ApplyUiScale(1f);
        ApplyActiveTab();
        Clear();
        if (IsInDesignMode)
        {
            SetDesignTimeSample();
        }
    }

    public float UiScale
    {
        get => _uiScale;
        set => ApplyUiScale(value);
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        if (IsInDesignMode && _snapshot == null && string.IsNullOrWhiteSpace(_descriptionLabel.Text))
        {
            SetDesignTimeSample();
        }
    }

    public void Clear()
    {
        _snapshot = null;
        _basePower = null;
        _enhancedPower = null;
        _selectedHistoryIndex = -1;

        _overviewSection.Title = "Overview";
        _descriptionLabel.Text = "Select a pet power to inspect its details.";
        _statsGrid.Clear();
        _effectsGrid.Clear();
        _damageDisplay.Clear();
        _damageSection.MetaText = string.Empty;
        SetTotals(null);
        _bonusesView.Clear();
        LayoutInfoSections();
    }

    public void SetSnapshot(PetActorSnapshot? snapshot, int selectedPowerIndex)
    {
        _snapshot = snapshot;
        _basePower = null;
        _enhancedPower = null;
        _selectedHistoryIndex = -1;

        if (snapshot == null)
        {
            Clear();
            return;
        }

        if (selectedPowerIndex < 0 ||
            selectedPowerIndex >= snapshot.ResolvedPowers.Count ||
            selectedPowerIndex >= snapshot.BasePowers.Count ||
            selectedPowerIndex >= snapshot.BuffedPowers.Count)
        {
            _overviewSection.Title = "Overview";
            _descriptionLabel.Text = "No power selected.";
            _statsGrid.Clear();
            _effectsGrid.Clear();
            _damageDisplay.Clear();
            _damageSection.MetaText = string.Empty;
            SetTotals(snapshot.Totals);
            _bonusesView.SetEntries(snapshot.AppliedBonusEntries);
            LayoutBonusesSection();
            LayoutInfoSections();
            return;
        }

        _basePower = snapshot.BasePowers[selectedPowerIndex];
        _enhancedPower = snapshot.BuffedPowers[selectedPowerIndex];
        _selectedHistoryIndex = snapshot.ResolvedPowers[selectedPowerIndex].SourceHistoryIndex;
        RefreshActiveTabContent();
    }

    public void SetBonusEntries(IReadOnlyList<PetAppliedBonusEntry> entries)
    {
        _bonusesView.SetEntries(entries);
        LayoutBonusesSection();
        if (_activeTab == DetailTab.Bonuses)
        {
            Invalidate(true);
        }
    }

    public void SetDesignTimeSample()
    {
        _snapshot = null;
        _basePower = null;
        _enhancedPower = null;
        _selectedHistoryIndex = -1;
        _overviewSection.Title = "Overview - Vicious Bite";
        _descriptionLabel.Text =
            "Melee, Light DMG(Lethal), Foe Minor DoT(Lethal)" +
            Environment.NewLine +
            Environment.NewLine +
            "The beast delivers a quick bite to their target causing Light Lethal damage and Minor Lethal damage over time." +
            Environment.NewLine +
            Environment.NewLine +
            "Damage: Light." +
            Environment.NewLine +
            "Recharge: Very Fast.";

        _statsGrid.SetRows(
        [
            new PowerStatsGrid.Row("Range", 7.0, 7.0, " ft", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Cast Time", 1.27, 1.27, " s", false, hideGainPercent: true),
            new PowerStatsGrid.Row("Root Time", 1.27, 1.27, " s", false, hideGainPercent: true),
            new PowerStatsGrid.Row("Recharge", 3.0, 3.0, " s", false, hideGainPercent: true),
            new PowerStatsGrid.Row("End Cost", 4.37, 4.37, " End", false, hideGainPercent: true),
            new PowerStatsGrid.Row("Accuracy", 95.0, 95.0, "%", true, hideGainPercent: true),
            new PowerStatsGrid.Row("Duration", 3.10, 3.10, " s", true, hideGainPercent: true)
        ]);

        _effectsGrid.SetGroups(
        [
            new PowerEffectsGrid.Group(
                "Defense / Resistance",
                [
                    new PowerEffectsGrid.NumericRow(
                        "Resistance",
                        "18.68%",
                        "28.68%",
                        "+10.00%",
                        "+53.5%",
                        tooltip: "Resistance to Smashing, Lethal, and Cold damage.",
                        contextChips: ["Smashing", "Lethal", "Cold", "Self"]),
                    new PowerEffectsGrid.NumericRow(
                        "Defense",
                        "0%",
                        "11.21%",
                        "+11.21%",
                        string.Empty,
                        tooltip: "Defense against melee, ranged, and AoE attacks.",
                        contextChips: ["Melee", "Ranged", "AoE", "Self"])
                ]),
            new PowerEffectsGrid.Group(
                "Status",
                [
                    new PowerEffectsGrid.MezRow(
                        "Confuse",
                        -2.0,
                        -2.0,
                        10.3,
                        10.3,
                        higherIsBetterMagnitude: false,
                        higherIsBetterDuration: true,
                        hideGainPercentMagnitude: true,
                        hideGainPercentDuration: true,
                        tooltip: "Applies Confuse to the target.",
                        contextChips: ["Foe", "Confuse"])
                ]),
            new PowerEffectsGrid.Group(
                "Buff / Debuff",
                [
                    new PowerEffectsGrid.DescriptorRow(
                        "Run Speed Strength",
                        "Self",
                        "Increases the Alpha Howler Wolf's movement speed while active.",
                        tooltip: "Movement speed bonus.")
                ])
        ]);

        _damageDisplay.ShowGraph = true;
        var designDamagePresentation = new DamageCardPresentation(
            HeaderText: "Damage",
            ModeBadgeText: "Average Activation",
            PrimaryText: "21.18",
            SubtitleText: "91.3% immediate | 8.7% DoT",
            TooltipText: "Immediate Light Lethal damage with a minor Damage Over Time component.",
            Segments:
            [
                new DamageSourceSegment(DamageSourceSegmentKind.Base, "Immediate (Lethal)", 19.34f),
                new DamageSourceSegment(DamageSourceSegmentKind.Proc, "Damage Over Time (Lethal)", 1.84f)
            ]);
        _damageDisplay.Presentation = designDamagePresentation with
        {
            HeaderText = string.Empty,
            ModeBadgeText = string.Empty
        };
        _damageSection.MetaText = "Average Activation";

        _quickReadSection.MetaText = string.Empty;
        _defenseSection.MetaText = "softcap: 45%";
        _resistanceSection.MetaText = "cap: 90%";

        _quickReadStrip.SetMetrics(
        [
            new TotalsQuickMetric("Recharge", "0%", null, "Recharge speed modifier.", MidsTotalsGlyph.QuickRecharge, Color.FromArgb(187, 111, 255)),
            new TotalsQuickMetric("Recovery", "100%", null, "Endurance recovery modifier.", MidsTotalsGlyph.QuickRecovery, Color.FromArgb(135, 200, 255)),
            new TotalsQuickMetric("Regen", "200%", null, "Health regeneration modifier.", MidsTotalsGlyph.QuickRegen, Color.FromArgb(154, 222, 100)),
            new TotalsQuickMetric("End Drain", "0/s", null, "Endurance usage per second.", MidsTotalsGlyph.QuickEndDrain, Color.FromArgb(255, 185, 96))
        ]);

        _defenseBarListLeft.ScaleMax = 100f;
        _defenseBarListRight.ScaleMax = 100f;
        _resistanceBarListLeft.ScaleMax = 100f;
        _resistanceBarListRight.ScaleMax = 100f;

        _defenseBarListLeft.SetMetrics(
        [
            new TotalsBarMetric("Smashing", "0%", 0f, 45f, "Smashing defense.", MidsTotalsGlyph.DamageSmashing, Color.FromArgb(214, 62, 226)),
            new TotalsBarMetric("Lethal", "0%", 0f, 45f, "Lethal defense.", MidsTotalsGlyph.DamageLethal, Color.FromArgb(214, 62, 226)),
            new TotalsBarMetric("Energy", "0%", 0f, 45f, "Energy defense.", MidsTotalsGlyph.DamageEnergy, Color.FromArgb(214, 62, 226)),
            new TotalsBarMetric("Negative", "0%", 0f, 45f, "Negative defense.", MidsTotalsGlyph.DamageNegative, Color.FromArgb(214, 62, 226)),
            new TotalsBarMetric("Toxic", "0%", 0f, 45f, "Toxic defense.", MidsTotalsGlyph.DamageToxic, Color.FromArgb(214, 62, 226)),
            new TotalsBarMetric("Psionic", "0%", 0f, 45f, "Psionic defense.", MidsTotalsGlyph.DamagePsionic, Color.FromArgb(214, 62, 226))
        ]);

        _defenseBarListRight.SetMetrics(
        [
            new TotalsBarMetric("Fire", "0%", 0f, 45f, "Fire defense.", MidsTotalsGlyph.DamageFire, Color.FromArgb(214, 62, 226)),
            new TotalsBarMetric("Cold", "0%", 0f, 45f, "Cold defense.", MidsTotalsGlyph.DamageCold, Color.FromArgb(214, 62, 226)),
            new TotalsBarMetric("Melee", "11.2%", 11.2f, 45f, "Melee defense.", MidsTotalsGlyph.DamageMelee, Color.FromArgb(214, 62, 226)),
            new TotalsBarMetric("Ranged", "21.2%", 21.2f, 45f, "Ranged defense.", MidsTotalsGlyph.DamageRanged, Color.FromArgb(214, 62, 226)),
            new TotalsBarMetric("AoE", "11.2%", 11.2f, 45f, "AoE defense.", MidsTotalsGlyph.DamageAoE, Color.FromArgb(214, 62, 226))
        ]);

        _resistanceBarListLeft.SetMetrics(
        [
            new TotalsBarMetric("Smashing", "28.7%", 28.7f, 90f, "Smashing resistance.", MidsTotalsGlyph.DamageSmashing, Color.FromArgb(86, 216, 230)),
            new TotalsBarMetric("Lethal", "28.7%", 28.7f, 90f, "Lethal resistance.", MidsTotalsGlyph.DamageLethal, Color.FromArgb(86, 216, 230)),
            new TotalsBarMetric("Energy", "10%", 10f, 90f, "Energy resistance.", MidsTotalsGlyph.DamageEnergy, Color.FromArgb(86, 216, 230)),
            new TotalsBarMetric("Negative", "10%", 10f, 90f, "Negative resistance.", MidsTotalsGlyph.DamageNegative, Color.FromArgb(86, 216, 230))
        ]);

        _resistanceBarListRight.SetMetrics(
        [
            new TotalsBarMetric("Fire", "10%", 10f, 90f, "Fire resistance.", MidsTotalsGlyph.DamageFire, Color.FromArgb(86, 216, 230)),
            new TotalsBarMetric("Cold", "28.7%", 28.7f, 90f, "Cold resistance.", MidsTotalsGlyph.DamageCold, Color.FromArgb(86, 216, 230)),
            new TotalsBarMetric("Toxic", "10%", 10f, 90f, "Toxic resistance.", MidsTotalsGlyph.DamageToxic, Color.FromArgb(86, 216, 230)),
            new TotalsBarMetric("Psionic", "10%", 10f, 90f, "Psionic resistance.", MidsTotalsGlyph.DamagePsionic, Color.FromArgb(86, 216, 230))
        ]);

        _coreMiscGrid.SetMetrics(
        [
            new TotalsValueMetric("To Hit", "10%", "To-hit modifier.", MidsTotalsGlyph.StatToHit, Color.FromArgb(210, 210, 210)),
            new TotalsValueMetric("Accuracy", "0%", "Accuracy modifier.", MidsTotalsGlyph.StatAccuracy, Color.FromArgb(210, 210, 210)),
            new TotalsValueMetric("Damage", "+27%", "Damage modifier.", MidsTotalsGlyph.StatDamage, Color.FromArgb(255, 190, 70)),
            new TotalsValueMetric("EndRdx", "0%", "Endurance reduction modifier.", MidsTotalsGlyph.StatEndRdx, Color.FromArgb(125, 185, 255)),
            new TotalsValueMetric("Range", "0%", "Range modifier.", MidsTotalsGlyph.StatRange, Color.FromArgb(210, 210, 210)),
            new TotalsValueMetric("Threat", "300%", "Threat modifier.", MidsTotalsGlyph.StatThreat, Color.FromArgb(255, 120, 90))
        ]);

        _bonusesView.SetEntries(
        [
            new PetAppliedBonusEntry
            {
                SourceName = "Pack Mentality",
                SourceFullName = "Sample.PackMentality",
                SourceType = PetAppliedBonusSourceType.SetBonus,
                Summary = "Damage +2%",
                Tooltip = "Pack Mentality grants a small damage bonus.",
                StatDeltas =
                [
                    new PetAppliedBonusStatDelta { StatName = "Damage", Delta = 2f, Suffix = "%" }
                ]
            },
            new PetAppliedBonusEntry
            {
                SourceName = "Supremacy",
                SourceFullName = "Sample.Supremacy",
                SourceType = PetAppliedBonusSourceType.OwnerAuraOrBuff,
                Summary = "Regen +2.41 HP/s, ToHit +10%, Damage +25%",
                Tooltip = "Supremacy improves damage, to-hit, regeneration, and resistances.",
                StatDeltas =
                [
                    new PetAppliedBonusStatDelta { StatName = "Regen", Delta = 2.41f, Suffix = " HP/s" },
                    new PetAppliedBonusStatDelta { StatName = "ToHit", Delta = 10f, Suffix = "%" },
                    new PetAppliedBonusStatDelta { StatName = "Damage", Delta = 25f, Suffix = "%" },
                    new PetAppliedBonusStatDelta { StatName = "Smashing Res", Delta = 10f, Suffix = "%" },
                    new PetAppliedBonusStatDelta { StatName = "Lethal Res", Delta = 10f, Suffix = "%" }
                ]
            }
        ]);

        LayoutInfoSections();
        LayoutTotalsSections();
        LayoutBonusesSection();
        ApplyActiveTab();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.Clear(CurrentTheme.Background);
    }

    private void AddTabButton(DetailTab tab, string text)
    {
        var button = new PetActorTabButton
        {
            Dock = DockStyle.Fill,
            Text = text,
            Margin = tab == DetailTab.Bonuses ? Padding.Empty : new Padding(0, 0, 8, 0)
        };
        button.Click += (_, _) =>
        {
            var previous = _activeTab;
            _activeTab = tab;
            ApplyActiveTab();
            if (_activeTab == DetailTab.Bonuses && previous != DetailTab.Bonuses)
            {
                BonusesTabActivated?.Invoke(this, EventArgs.Empty);
            }
        };

        _tabButtons[tab] = button;
        _tabStrip.Controls.Add(button, _tabButtons.Count - 1, 0);
    }

    private void PopulateOverview()
    {
        if (_basePower == null)
        {
            return;
        }

        _overviewSection.Title = BuildOverviewTitle(_basePower.DisplayName);

        var descriptionParts = new List<string>();
        var sharedRechargeSummary = BuildSharedRechargeInfoSummary(_basePower);
        if (!string.IsNullOrWhiteSpace(sharedRechargeSummary))
        {
            descriptionParts.Add(sharedRechargeSummary);
        }

        foreach (var part in new[]
                 {
                     NormalizeMarkupText(_basePower.DescShort),
                     NormalizeMarkupText(_basePower.DescLongFormatted)
                 }
                 .Where(part => !string.IsNullOrWhiteSpace(part))
                 .Distinct(StringComparer.Ordinal))
        {
            descriptionParts.Add(part);
        }

        _descriptionLabel.Text = descriptionParts.Count == 0
            ? "No descriptive text is available for this power yet."
            : string.Join(Environment.NewLine + Environment.NewLine, descriptionParts);
    }

    private void PopulateStats()
    {
        if (_basePower == null)
        {
            _statsGrid.Clear();
            return;
        }

        var enhancedPower = _enhancedPower ?? _basePower;
        var contributionSnapshot = _snapshot?.CalculationSnapshot?.Contributions;
        var rows = PowerCanonicalStats.BuildRows(_basePower, enhancedPower, contributionSnapshot, _selectedHistoryIndex);
        _statsGrid.SetRows(rows);
    }

    private void PopulateEffects()
    {
        if (_basePower == null)
        {
            _effectsGrid.Clear();
            return;
        }

        var enhancedPower = _enhancedPower ?? _basePower;
        var groupedRankedEffects = GroupedFx.AggregateGroupedEffectsPass2(
            enhancedPower,
            GroupedFx.AssembleGroupedEffects(enhancedPower));
        var rankedEffects = enhancedPower.GetRankedEffects(true)?.ToList()
                            ?? Enumerable.Range(0, enhancedPower.Effects.Length).ToList();
        var groups = PowerEffects.Build(_basePower, enhancedPower, groupedRankedEffects, rankedEffects);
        _effectsGrid.SetGroups(groups);
    }

    private void PopulateDamage()
    {
        if (_basePower == null)
        {
            _damageDisplay.Clear();
            _damageSection.MetaText = string.Empty;
            return;
        }

        var enhancedPower = _enhancedPower ?? _basePower;
        var presentation = BuildDamagePresentation(enhancedPower);
        _damageDisplay.ShowGraph = !MidsContext.Config.DisableDataDamageGraph;
        _damageDisplay.Presentation = presentation with
        {
            HeaderText = string.Empty,
            ModeBadgeText = string.Empty
        };
        _damageSection.MetaText = presentation.ModeBadgeText;
    }

    private DamageCardPresentation BuildDamagePresentation(IPower power)
    {
        var totalSummary = Power.GetDamageBreakdown(power);
        if (!totalSummary.HasDamageEffects)
        {
            return new DamageCardPresentation(
                HeaderText: "Damage",
                ModeBadgeText: "Non-Damage",
                PrimaryText: "0",
                SubtitleText: string.Empty,
                TooltipText: string.Empty,
                Segments: Array.Empty<DamageSourceSegment>());
        }

        var immediateSummary = GetFilteredDamageBreakdown(power, effect => !IsDamageOverTime(effect));
        var dotSummary = GetFilteredDamageBreakdown(power, IsDamageOverTime);

        var immediateValue = Math.Max(0f, immediateSummary.DisplayedTotal);
        var dotValue = Math.Max(0f, dotSummary.DisplayedTotal);
        var totalValue = Math.Max(0f, totalSummary.DisplayedTotal);

        var segments = new List<DamageSourceSegment>(2);
        if (immediateValue > float.Epsilon)
        {
            segments.Add(new DamageSourceSegment(
                DamageSourceSegmentKind.Base,
                BuildDamageLegendLabel("Immediate", immediateSummary),
                immediateValue));
        }

        if (dotValue > float.Epsilon)
        {
            segments.Add(new DamageSourceSegment(
                DamageSourceSegmentKind.Proc,
                BuildDamageLegendLabel("Damage Over Time", dotSummary),
                dotValue));
        }

        var subtitle = BuildDamageSubtitle(totalSummary, immediateValue, dotValue);
        return new DamageCardPresentation(
            HeaderText: "Damage",
            ModeBadgeText: BuildDamageModeLabel(),
            PrimaryText: DisplayValueFormatter.FormatNumber(totalValue),
            SubtitleText: subtitle,
            TooltipText: Power.BuildDamageTip(power),
            Segments: segments);
    }

    private static DamageBreakdownSummary GetFilteredDamageBreakdown(IPower power, Func<IEffect, bool> predicate)
    {
        var clone = new Power(power)
        {
            Effects = power.Effects.Where(effect => predicate(effect)).ToArray()
        };
        return Power.GetDamageBreakdown(clone);
    }

    private static bool IsDamageOverTime(IEffect effect)
    {
        if (!Power.ShouldIncludeDamageEffect(effect))
        {
            return false;
        }

        return Power.GetDamageEffectEffectiveTicks(effect) > 1.001f ||
               effect.Duration > 0.001f ||
               effect.DelayedTime > 0.001f;
    }

    private static string BuildDamageSubtitle(DamageBreakdownSummary totalSummary, float immediateValue, float dotValue)
    {
        var parts = new List<string>();
        var total = Math.Max(0.0001f, totalSummary.DisplayedTotal);
        if (immediateValue > float.Epsilon && dotValue > float.Epsilon)
        {
            parts.Add($"{DisplayValueFormatter.FormatPercentValue(immediateValue / total * 100f, 1)}% immediate");
            parts.Add($"{DisplayValueFormatter.FormatPercentValue(dotValue / total * 100f, 1)}% DoT");
        }
        else if (dotValue > float.Epsilon)
        {
            parts.Add("100% DoT");
        }
        else
        {
            parts.Add("100% immediate");
        }

        if (totalSummary.HasPercentDamage)
        {
            parts.Add($"{DisplayValueFormatter.FormatPercentValue(totalSummary.PercentOfTargetHpTotal, 2)}% target HP");
        }

        return string.Join(" | ", parts);
    }

    private static string BuildDamageLegendLabel(string prefix, DamageBreakdownSummary summary)
    {
        if (summary.ByType.Count == 0)
        {
            return prefix;
        }

        if (summary.ByType.Count == 1)
        {
            return $"{prefix} ({summary.ByType[0].Label})";
        }

        var dominant = summary.ByType
            .OrderByDescending(item => Math.Abs(item.DisplayedTotal))
            .First();
        return $"{prefix} ({dominant.Label})";
    }

    private static string BuildDamageModeLabel()
    {
        var chanceLabel = MidsContext.Config.DamageMath.Calculate switch
        {
            ConfigData.EDamageMath.Average => "Average",
            ConfigData.EDamageMath.Max => "Maximum",
            ConfigData.EDamageMath.Minimum => "Minimum",
            _ => "Average"
        };
        return $"{chanceLabel} Activation";
    }

    private void SetTotals(ActorTotalsSnapshot? totalsSnapshot)
    {
        if (totalsSnapshot == null)
        {
            _quickReadStrip.SetMetrics(Array.Empty<TotalsQuickMetric>());
            _defenseBarListLeft.SetMetrics(Array.Empty<TotalsBarMetric>());
            _defenseBarListRight.SetMetrics(Array.Empty<TotalsBarMetric>());
            _resistanceBarListLeft.SetMetrics(Array.Empty<TotalsBarMetric>());
            _resistanceBarListRight.SetMetrics(Array.Empty<TotalsBarMetric>());
            _coreMiscGrid.SetMetrics(Array.Empty<TotalsValueMetric>());
            LayoutTotalsSections();
            return;
        }

        var actorDisplayStats = totalsSnapshot.DisplayStats;
        var totals = totalsSnapshot.Totals;
        var totalsCapped = totalsSnapshot.TotalsCapped;
        var resistanceCapValue = DatabaseAPI.GetClassResistanceCap(totalsSnapshot.ClassName);
        string FormatPercentValue(float value, int maxDecimal = 2) => $"{DisplayValueFormatter.FormatPercentValue(value, maxDecimal)}%";
        string FormatPercentScale(float value, int maxDecimal = 2) => $"{DisplayValueFormatter.FormatPercentFromScale(value, maxDecimal)}%";
        string FormatSignedPercentValue(float value, int maxDecimal = 0) =>
            $"{(value > 0 ? "+" : string.Empty)}{DisplayValueFormatter.FormatPercentValue(value, maxDecimal)}%";
        float GetDefense(int damageType) => actorDisplayStats.Defense(damageType);
        float GetResistance(int damageType, bool uncapped) => actorDisplayStats.DamageResistance(damageType, uncapped);

        var endTip = $"Time to go from 0-100% end: {DisplayValueFormatter.FormatSeconds(actorDisplayStats.EnduranceTimeToFull)}s.";
        var regenTip = $"Time to go from 0-100% health: {DisplayValueFormatter.FormatSeconds(actorDisplayStats.HealthRegenTimeToFull)}s.\r\n" +
                       $"HitPoints regenerated per second at level 50: {DisplayValueFormatter.FormatRate(actorDisplayStats.HealthRegenHPPerSec(false))} HP";
        var endDrainTip = actorDisplayStats.EnduranceRecoveryNet switch
        {
            > 0 => $"Net Endurance Gain: {DisplayValueFormatter.FormatRate(actorDisplayStats.EnduranceRecoveryNet)}/s",
            < 0 => $"Net Endurance Loss: {DisplayValueFormatter.FormatRate(actorDisplayStats.EnduranceRecoveryLossNet)}/s",
            _ => "Net endurance is neutral."
        };

        _quickReadSection.MetaText = string.Empty;
        _defenseSection.MetaText = "softcap: 45%";
        _resistanceSection.MetaText = $"cap: {FormatPercentScale(resistanceCapValue, 0)}";

        _quickReadStrip.SetMetrics(
        [
            new TotalsQuickMetric("Recharge", FormatSignedPercentValue(actorDisplayStats.BuffHaste(false) - 100f, 0), null, "Recharge speed modifier.", MidsTotalsGlyph.QuickRecharge, Color.FromArgb(187, 111, 255)),
            new TotalsQuickMetric("Recovery", FormatPercentValue(actorDisplayStats.EnduranceRecoveryPercentage(false), 0), null, endTip, MidsTotalsGlyph.QuickRecovery, Color.FromArgb(135, 200, 255)),
            new TotalsQuickMetric("Regen", FormatPercentValue(actorDisplayStats.HealthRegenPercent(false), 0), null, regenTip, MidsTotalsGlyph.QuickRegen, Color.FromArgb(154, 222, 100)),
            new TotalsQuickMetric("End Drain", $"{DisplayValueFormatter.FormatRate(actorDisplayStats.EnduranceUsage)}/s", null, endDrainTip, MidsTotalsGlyph.QuickEndDrain, Color.FromArgb(255, 185, 96))
        ]);

        _defenseBarListLeft.ScaleMax = 100f;
        _defenseBarListRight.ScaleMax = 100f;
        _resistanceBarListLeft.ScaleMax = 100f;
        _resistanceBarListRight.ScaleMax = 100f;

        _defenseBarListLeft.SetMetrics(BuildBarMetrics(
            [Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Toxic, Enums.eDamage.Psionic],
            true,
            damage =>
            {
                var index = (int)damage;
                return (Math.Max(0, GetDefense(index)), 45f, $"{FormatPercentValue(GetDefense(index))} {damage} defense");
            },
            FormatPercentValue,
            45f));

        _defenseBarListRight.SetMetrics(BuildBarMetrics(
            [Enums.eDamage.Fire, Enums.eDamage.Cold, Enums.eDamage.Melee, Enums.eDamage.Ranged, Enums.eDamage.AoE],
            true,
            damage =>
            {
                var index = (int)damage;
                return (Math.Max(0, GetDefense(index)), 45f, $"{FormatPercentValue(GetDefense(index))} {damage} defense");
            },
            FormatPercentValue,
            45f));

        _resistanceBarListLeft.SetMetrics(BuildBarMetrics(
            [Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Energy, Enums.eDamage.Negative],
            false,
            damage =>
            {
                var index = (int)damage;
                var uncapped = Math.Max(0, GetResistance(index, true));
                var capped = Math.Max(0, GetResistance(index, false));
                var tip = totalsCapped.Res[index] < totals.Res[index]
                    ? $"{FormatPercentValue(uncapped)} {damage} resistance capped at {FormatPercentValue(capped)}"
                    : $"{FormatPercentValue(uncapped)} {damage} resistance. (cap: {FormatPercentScale(resistanceCapValue, 0)})";
                return (capped, resistanceCapValue * 100f, tip);
            },
            FormatPercentValue,
            resistanceCapValue * 100f));

        _resistanceBarListRight.SetMetrics(BuildBarMetrics(
            [Enums.eDamage.Fire, Enums.eDamage.Cold, Enums.eDamage.Toxic, Enums.eDamage.Psionic],
            false,
            damage =>
            {
                var index = (int)damage;
                var uncapped = Math.Max(0, GetResistance(index, true));
                var capped = Math.Max(0, GetResistance(index, false));
                var tip = totalsCapped.Res[index] < totals.Res[index]
                    ? $"{FormatPercentValue(uncapped)} {damage} resistance capped at {FormatPercentValue(capped)}"
                    : $"{FormatPercentValue(uncapped)} {damage} resistance. (cap: {FormatPercentScale(resistanceCapValue, 0)})";
                return (capped, resistanceCapValue * 100f, tip);
            },
            FormatPercentValue,
            resistanceCapValue * 100f));

        _coreMiscGrid.SetMetrics(
        [
            new TotalsValueMetric("To Hit", FormatPercentValue(actorDisplayStats.BuffToHit, 0), "To-hit modifier.", MidsTotalsGlyph.StatToHit, Color.FromArgb(210, 210, 210)),
            new TotalsValueMetric("Accuracy", FormatSignedPercentValue(actorDisplayStats.BuffAccuracy, 0), "Accuracy modifier.", MidsTotalsGlyph.StatAccuracy, Color.FromArgb(210, 210, 210)),
            new TotalsValueMetric("Damage", FormatSignedPercentValue(actorDisplayStats.BuffDamage(false) - 100f, 0), "Damage modifier.", MidsTotalsGlyph.StatDamage, Color.FromArgb(255, 190, 70)),
            new TotalsValueMetric("EndRdx", FormatPercentValue(actorDisplayStats.BuffEndRdx, 0), "Endurance reduction modifier.", MidsTotalsGlyph.StatEndRdx, Color.FromArgb(125, 185, 255)),
            new TotalsValueMetric("Range", FormatSignedPercentValue(actorDisplayStats.RangePercent, 0), "Range modifier.", MidsTotalsGlyph.StatRange, Color.FromArgb(210, 210, 210)),
            new TotalsValueMetric("Threat", FormatPercentValue(actorDisplayStats.ThreatLevel, 0), "Threat modifier.", MidsTotalsGlyph.StatThreat, Color.FromArgb(255, 120, 90))
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
                if (!DatabaseAPI.RealmUsesToxicDef() && defense && damage == Enums.eDamage.Toxic)
                {
                    continue;
                }

                var (value, marker, tooltip) = getter(damage);
                var (icon, color) = GetDamageIcon(damage, defense);
                list.Add(new TotalsBarMetric(
                    damage == Enums.eDamage.AoE ? "AoE" : damage.ToString(),
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

    private void ApplyActiveTab()
    {
        foreach (var (tab, button) in _tabButtons)
        {
            button.Selected = tab == _activeTab;
        }

        _infoScrollPanel.Visible = _activeTab == DetailTab.Info;
        _effectsGrid.Visible = _activeTab == DetailTab.Effects;
        _totalsScrollPanel.Visible = _activeTab == DetailTab.Totals;
        _bonusesScrollPanel.Visible = _activeTab == DetailTab.Bonuses;
        RefreshActiveTabContent();
        RefreshResponsiveLayout();
        Invalidate(true);
    }

    private void RefreshActiveTabContent()
    {
        switch (_activeTab)
        {
            case DetailTab.Info:
                if (_basePower == null)
                {
                    _overviewSection.Title = "Overview";
                    _descriptionLabel.Text = _snapshot == null
                        ? "Select a pet power to inspect its details."
                        : "No power selected.";
                    _statsGrid.Clear();
                    _damageDisplay.Clear();
                    _damageSection.MetaText = string.Empty;
                }
                else
                {
                    PopulateOverview();
                    PopulateStats();
                    PopulateDamage();
                }

                LayoutInfoSections();
                break;

            case DetailTab.Effects:
                if (_basePower == null)
                {
                    _effectsGrid.Clear();
                }
                else
                {
                    PopulateEffects();
                }

                break;

            case DetailTab.Totals:
                SetTotals(_snapshot?.Totals);
                break;

            case DetailTab.Bonuses:
                _bonusesView.SetEntries(_snapshot?.AppliedBonusEntries ?? Array.Empty<PetAppliedBonusEntry>());
                LayoutBonusesSection();
                break;
        }
    }

    private void OnHostSizeChanged(object? sender, EventArgs e)
    {
        RefreshResponsiveLayout();
    }

    public void ApplyUiScale(float scale)
    {
        ApplyUiScale(scale, force: false);
    }

    public void RefreshResponsiveLayout()
    {
        ApplyUiScale(_requestedUiScale, force: true);
    }

    private void ApplyUiScale(float scale, bool force)
    {
        if (_applyingUiScale)
        {
            return;
        }

        _requestedUiScale = ClampUiScale(scale);
        _applyingUiScale = true;
        try
        {
            ApplyUiScaleCore(_requestedUiScale, force);
        }
        finally
        {
            _applyingUiScale = false;
        }
    }

    private void ApplyUiScaleCore(float scale, bool force)
    {
        scale = ClampUiScale(scale);
        if (!force && Math.Abs(scale - _uiScale) < 0.01f)
        {
            return;
        }

        _uiScale = scale;

        SuspendLayout();

        _rootLayout.RowStyles[0].Height = ScaleMetric(BaseTabRowHeight, scale, 34);
        _tabStrip.Margin = new Padding(0, 0, 0, ScaleMetric(BaseSectionGap, scale, 4));

        var buttonIndex = 0;
        foreach (var button in _tabButtons.Values)
        {
            button.UiScale = scale;
            button.Margin = buttonIndex == _tabButtons.Count - 1
                ? Padding.Empty
                : new Padding(0, 0, ScaleMetric(BaseSectionGap, scale, 4), 0);
            buttonIndex++;
        }

        ApplyFontScale(this, scale);

        _overviewSection.UiScale = scale;
        _statsSection.UiScale = scale;
        _damageSection.UiScale = scale;
        _quickReadSection.UiScale = scale;
        _defenseSection.UiScale = scale;
        _resistanceSection.UiScale = scale;
        _coreMiscSection.UiScale = scale;

        _statsGrid.HeaderHeight = ScaleMetric(28, scale, 22);
        _statsGrid.RowHeight = ScaleMetric(27, scale, 22);
        _statsGrid.GridPadding = ScaleMetric(6, scale, 4);

        _effectsGrid.HeaderHeight = ScaleMetric(28, scale, 20);
        _effectsGrid.RowHeight = ScaleMetric(28, scale, 20);
        _effectsGrid.GroupHeaderHeight = ScaleMetric(30, scale, 22);
        _effectsGrid.MezLabelHeight = ScaleMetric(24, scale, 18);
        _effectsGrid.MezChildHeight = ScaleMetric(28, scale, 20);
        _effectsGrid.DescriptorRowHeight = ScaleMetric(38, scale, 28);
        _effectsGrid.GridPadding = ScaleMetric(8, scale, 6);

        _quickReadStrip.UiScale = scale;
        _defenseListsHost.UiScale = scale;
        _resistanceListsHost.UiScale = scale;
        _defenseBarListLeft.UiScale = scale;
        _defenseBarListRight.UiScale = scale;
        _resistanceBarListLeft.UiScale = scale;
        _resistanceBarListRight.UiScale = scale;
        _coreMiscGrid.UiScale = scale;
        _bonusesView.UiScale = scale;

        _descriptionLabel.Margin = new Padding(0, 0, 0, ScaleMetric(6, scale, 4));

        var damageHeight = ScaleMetric(BaseDamageHeight, scale, 132);
        _damageHost.FixedHeight = damageHeight;
        _damageDisplay.MinimumSize = new Size(0, damageHeight);
        _damageDisplay.Size = new Size(Math.Max(_damageDisplay.Width, 240), damageHeight);

        LayoutInfoSections();
        LayoutTotalsSections();
        LayoutBonusesSection();

        ResumeLayout(performLayout: true);
        Invalidate(true);
    }

    private void LayoutInfoSections()
    {
        var outerInset = ScaleMetric(BaseContentInset, _uiScale, 2);
        var gap = ScaleMetric(BaseSectionGap, _uiScale, 4);
        var width = Math.Max(1, _infoScrollPanel.AvailableClientWidth - outerInset * 2);
        var contentWidth = Math.Max(1, width - ScaleMetric(14, _uiScale, 10));

        _descriptionLabel.MaximumSize = new Size(contentWidth, 0);

        var y = outerInset;
        LayoutSection(_overviewSection, width, outerInset, ref y, gap);
        LayoutSection(_statsSection, width, outerInset, ref y, gap);
        LayoutSection(_damageSection, width, outerInset, ref y, gap);

        _infoStackHost.Bounds = new Rectangle(0, 0, _infoScrollPanel.ContentPanel.ClientSize.Width, y + outerInset);
        _infoScrollPanel.ContentPanel.Invalidate();
    }

    private void LayoutTotalsSections()
    {
        var outerInset = ScaleMetric(BaseContentInset, _uiScale, 2);
        var gap = ScaleMetric(BaseSectionGap, _uiScale, 4);
        var width = Math.Max(1, _totalsScrollPanel.AvailableClientWidth - outerInset * 2);
        var y = outerInset;

        LayoutSection(_quickReadSection, width, outerInset, ref y, gap);
        LayoutSection(_defenseSection, width, outerInset, ref y, gap);
        LayoutSection(_resistanceSection, width, outerInset, ref y, gap);
        LayoutSection(_coreMiscSection, width, outerInset, ref y, gap);

        _totalsStackHost.Bounds = new Rectangle(0, 0, _totalsScrollPanel.ContentPanel.ClientSize.Width, y + outerInset);
    }

    private void LayoutBonusesSection()
    {
        var inset = ScaleMetric(BaseContentInset, _uiScale, 2);
        var width = Math.Max(1, _bonusesScrollPanel.AvailableClientWidth - inset * 2);
        var preferred = _bonusesView.GetPreferredSize(new Size(width, 0));
        _bonusesView.Bounds = new Rectangle(inset, inset, width, preferred.Height);
    }

    private static void LayoutSection(MidsTotalsSectionPanel section, int width, int left, ref int y, int gap)
    {
        var preferred = section.GetPreferredSize(new Size(width, 0));
        section.Bounds = new Rectangle(left, y, width, preferred.Height);
        y = section.Bottom + gap;
    }

    private static Label CreateOverviewLabel(float fontSize, FontStyle fontStyle)
    {
        return new Label
        {
            AutoSize = true,
            MaximumSize = new Size(720, 0),
            Margin = new Padding(0, 0, 0, 8),
            Font = new Font("Segoe UI", fontSize, fontStyle),
            ForeColor = Color.WhiteSmoke
        };
    }

    private static string NormalizeMarkupText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalized = text.Replace("\0", string.Empty);
        normalized = Regex.Replace(normalized, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
        normalized = Regex.Replace(normalized, @"</p\s*>", "\n\n", RegexOptions.IgnoreCase);
        normalized = Regex.Replace(normalized, @"<p\s*>", string.Empty, RegexOptions.IgnoreCase);
        normalized = Regex.Replace(normalized, @"<li\s*>", "• ", RegexOptions.IgnoreCase);
        normalized = Regex.Replace(normalized, @"</li\s*>", "\n", RegexOptions.IgnoreCase);
        normalized = Regex.Replace(normalized, @"<[^>]+>", string.Empty, RegexOptions.IgnoreCase);
        normalized = WebUtility.HtmlDecode(normalized);
        normalized = normalized.Replace("\u00E2\u20AC\u00A2 ", "- ");
        normalized = normalized.Replace("\u00C3\u00A2\u00E2\u201A\u00AC\u00C2\u00A2 ", "- ");
        normalized = normalized.Replace("â€¢ ", "- ");
        normalized = normalized.Replace("\r\n", "\n").Replace('\r', '\n');
        normalized = Regex.Replace(normalized, @"[ \t]+\n", "\n");
        normalized = Regex.Replace(normalized, @"[ \t]{2,}", " ");
        normalized = Regex.Replace(normalized, @"\n{3,}", "\n\n");
        return normalized.Replace("\n", Environment.NewLine).Trim();
    }

    private void ApplyFontScale(Control root, float scale)
    {
        foreach (var control in EnumerateControls(root))
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

    private static float ClampUiScale(float scale) => Math.Clamp(scale, MinimumResponsiveUiScale, 1.25f);

    private static int ScaleMetric(int value, float scale, int minimum)
        => Math.Max(minimum, (int)Math.Round(value * scale));

    private static string BuildOverviewTitle(string? powerName)
    {
        return string.IsNullOrWhiteSpace(powerName)
            ? "Overview"
            : $"Overview - {powerName.Trim()}";
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
            : $"Shared Recharge Group{(groups.Length == 1 ? string.Empty : "s")}: {string.Join(", ", groups.Select(group => group.Replace('_', ' ').Trim()))}";
    }

    private static string FormatTokenizedName(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Replace('_', ' ').Trim();
    }

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

    private DataViewTheme CurrentTheme => DesignMode
        ? ThemeManager.DesignTime.DataView
        : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

    private bool IsInDesignMode =>
        DesignMode ||
        LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
        AppDomain.CurrentDomain.FriendlyName.Contains("devenv", StringComparison.OrdinalIgnoreCase) ||
        AppDomain.CurrentDomain.FriendlyName.Contains("designtoolsserver", StringComparison.OrdinalIgnoreCase) ||
        Process.GetCurrentProcess().ProcessName.Contains("devenv", StringComparison.OrdinalIgnoreCase) ||
        Process.GetCurrentProcess().ProcessName.Contains("designtoolsserver", StringComparison.OrdinalIgnoreCase) ||
        Process.GetCurrentProcess().ProcessName.Contains("xdesproc", StringComparison.OrdinalIgnoreCase);

    private sealed class FixedHeightHost : Panel
    {
        private int _height;

        public FixedHeightHost(Control child, int height)
        {
            _height = height;
            Margin = Padding.Empty;
            Padding = Padding.Empty;
            BackColor = Color.Transparent;
            Controls.Add(child);
        }

        public int FixedHeight
        {
            get => _height;
            set
            {
                var clamped = Math.Max(1, value);
                if (_height == clamped)
                {
                    return;
                }

                _height = clamped;
                PerformLayout();
                Invalidate();
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var width = proposedSize.Width > 0 ? proposedSize.Width : Width;
            return new Size(width, _height);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (Controls.Count > 0)
            {
                Controls[0].Bounds = new Rectangle(0, 0, ClientSize.Width, _height);
            }

            Height = _height;
        }
    }

    private sealed class PetActorTabButton : Control
    {
        private bool _selected;
        private float _uiScale = 1f;

        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected == value)
                {
                    return;
                }

                _selected = value;
                Invalidate();
            }
        }

        public float UiScale
        {
            get => _uiScale;
            set
            {
                var clamped = Math.Clamp(value, 0.82f, 1.25f);
                if (Math.Abs(_uiScale - clamped) < 0.01f)
                {
                    return;
                }

                _uiScale = clamped;
                Invalidate();
            }
        }

        public PetActorTabButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var theme = CurrentTheme;
            var bounds = Rectangle.Inflate(ClientRectangle, -1, -1);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = RoundedRect(bounds, ScalePx(8));
            using var fillBrush = new LinearGradientBrush(
                bounds,
                _selected ? theme.HeaderTop : theme.GridHeaderTop,
                _selected ? theme.HeaderBottom : theme.Card,
                LinearGradientMode.Vertical);
            using var borderPen = new Pen(_selected ? theme.Accent : theme.Border, _selected ? 1.6f : 1f);
            e.Graphics.FillPath(fillBrush, path);
            e.Graphics.DrawPath(borderPen, path);

            using var font = new Font("Segoe UI", Math.Max(7f, 10f * _uiScale), FontStyle.Bold);
            TextRenderer.DrawText(
                e.Graphics,
                Text,
                font,
                bounds,
                _selected ? theme.ValueText : theme.Text,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }

        private DataViewTheme CurrentTheme => DesignMode
            ? ThemeManager.DesignTime.DataView
            : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

        private int ScalePx(int value)
            => Math.Max(1, (int)Math.Round(value * _uiScale));
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        radius = Math.Max(2, radius);
        var diameter = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}

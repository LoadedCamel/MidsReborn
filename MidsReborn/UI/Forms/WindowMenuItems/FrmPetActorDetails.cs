using System.Drawing.Drawing2D;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Test;
using Mids_Reborn.UI.Theming;
using MRBResourceLib;

namespace Mids_Reborn.UI.Forms.WindowMenuItems;

public sealed class FrmPetActorDetails : Form
{
    private readonly Toon _toon;
    private readonly PetActorRibbon _actorRibbon;
    private readonly MidsVScrollPanel _powerGridScrollPanel;
    private readonly PetActorPowerGrid _powerGrid;
    private readonly MidsDataViewNeo _detailView;
    private readonly Panel _headerPanel;
    private readonly TableLayoutPanel _summaryMetrics;
    private readonly TableLayoutPanel _previewStatePanel;
    private readonly FlowLayoutPanel _upgradeFlow;
    private readonly FlowLayoutPanel _proximityFlow;
    private readonly SplitContainer _bodySplit;
    private readonly SplitContainer _detailTotalsSplit;
    private readonly Panel _emptyStatePanel;
    private readonly Label _nameLabel;
    private readonly Label _subTitleLabel;
    private readonly Label _classLabel;
    private readonly Label _countLabel;
    private readonly Label _tagsLabel;
    private readonly Label _emptyLabel;
    private readonly Panel _totalsScrollPanel;
    private readonly TableLayoutPanel _totalsStack;
    private readonly PowerStatsGrid _coreGrid;
    private readonly PowerStatsGrid _combatGrid;
    private readonly PowerStatsGrid _movementGrid;
    private readonly PowerStatsGrid _defenseGrid;
    private readonly PowerStatsGrid _statusGrid;
    private readonly Label _coreLabel;
    private readonly Label _combatLabel;
    private readonly Label _movementLabel;
    private readonly Label _defenseLabel;
    private readonly Label _statusLabel;
    private readonly Dictionary<string, (Label Title, Label Value)> _metricLabels = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<RealPetActorRosterItem> _roster = [];
    private readonly Dictionary<string, PetActorPreviewState> _previewStatesByActor = new(StringComparer.OrdinalIgnoreCase);

    private PetActorSnapshot? _currentSnapshot;
    private string? _selectedEntityUid;
    private int _selectedSourceHistoryIndex = -1;
    private int _selectedPowerIndex = -1;
    private bool _suppressPreviewStateEvents;

    private DataViewTheme CurrentTheme => DesignMode
        ? ThemeManager.DesignTime.DataView
        : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

    public FrmPetActorDetails(Toon toon, string? initialEntityUid = null, int initialSourceHistoryIndex = -1)
    {
        _toon = toon;
        _selectedEntityUid = initialEntityUid;
        _selectedSourceHistoryIndex = initialSourceHistoryIndex;

        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.Black;
        ForeColor = Color.WhiteSmoke;
        FormBorderStyle = FormBorderStyle.SizableToolWindow;
        MinimumSize = new Size(1200, 780);
        Size = new Size(1500, 920);
        StartPosition = FormStartPosition.Manual;
        Text = "Pet Actor Sheet";
        Icon = Resources.MRB_Icon_Concept;

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            BackColor = Color.Black,
            Padding = new Padding(8)
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 102f));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 84f));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 82f));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 86f));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        _headerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 8),
            Padding = new Padding(16, 12, 16, 12)
        };
        _headerPanel.Paint += HeaderPanelOnPaint;

        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            BackColor = Color.Transparent
        };
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 74f));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26f));
        headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34f));
        headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24f));
        headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        _nameLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 16f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        _subTitleLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft
        };
        _tagsLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9f, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft
        };
        _classLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight
        };
        _countLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight
        };

        headerLayout.Controls.Add(_nameLabel, 0, 0);
        headerLayout.Controls.Add(_countLabel, 1, 0);
        headerLayout.Controls.Add(_subTitleLabel, 0, 1);
        headerLayout.Controls.Add(_classLabel, 1, 1);
        headerLayout.Controls.Add(_tagsLabel, 0, 2);
        _headerPanel.Controls.Add(headerLayout);

        _summaryMetrics = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 1,
            Margin = new Padding(0, 0, 0, 8),
            BackColor = Color.Black
        };
        for (var index = 0; index < 5; index++)
        {
            _summaryMetrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
        }

        AddMetric("hp", "Hit Points");
        AddMetric("regen", "Regen");
        AddMetric("recovery", "Recovery");
        AddMetric("defense", "Peak Def");
        AddMetric("resistance", "Peak Res");

        _previewStatePanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 8),
            BackColor = Color.Black
        };
        _previewStatePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        _previewStatePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _previewStatePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _upgradeFlow = CreatePreviewStateFlow();
        _proximityFlow = CreatePreviewStateFlow();
        _previewStatePanel.Controls.Add(CreatePreviewStateGroup("Applied Upgrades", _upgradeFlow), 0, 0);
        _previewStatePanel.Controls.Add(CreatePreviewStateGroup("Pet Context", _proximityFlow), 0, 1);

        var ribbonHost = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 8),
            BackColor = Color.Black
        };
        ribbonHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 24f));
        ribbonHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        ribbonHost.Controls.Add(CreateSectionLabel("Active Real Pet Actors"), 0, 0);

        _actorRibbon = new PetActorRibbon
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Height = 58
        };
        _actorRibbon.ItemSelected += ActorRibbonOnItemSelected;
        ribbonHost.Controls.Add(_actorRibbon, 0, 1);

        _bodySplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Black
        };

        _detailView = new MidsDataViewNeo
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 8, 0)
        };
        _detailView.SetGraphType(Enums.MDmgGraphType.Layered, Enums.MDmgDisplayStyle.TextUnderGraph);
        _detailView.SetPresentationMode(MidsDataViewNeoPresentationMode.ActorReadOnly);
        _detailView.EntityDetails += DetailViewOnEntityDetails;
        _bodySplit.Panel1.Controls.Add(_detailView);

        var powerPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Black,
            Padding = new Padding(8, 0, 0, 0)
        };
        powerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));
        powerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        powerPanel.Controls.Add(CreateSectionLabel("Powers"), 0, 0);

        _powerGridScrollPanel = new MidsVScrollPanel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            BackColor = Color.Black
        };
        _powerGrid = new PetActorPowerGrid
        {
            Dock = DockStyle.Top,
            Margin = new Padding(0)
        };
        _powerGrid.PowerSelected += PowerGridOnPowerSelected;
        _powerGrid.PreviewToggleClicked += PowerGridOnPreviewToggleClicked;
        _powerGridScrollPanel.ContentPanel.Controls.Add(_powerGrid);
        powerPanel.Controls.Add(_powerGridScrollPanel, 0, 1);
        _bodySplit.Panel2.Controls.Add(powerPanel);

        _detailTotalsSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            BackColor = Color.Black,
            FixedPanel = FixedPanel.Panel2
        };

        var totalsHost = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Black,
            Padding = new Padding(0, 8, 0, 0)
        };
        totalsHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));
        totalsHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        totalsHost.Controls.Add(CreateSectionLabel("Actor Totals"), 0, 0);

        _totalsScrollPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.Black,
            Padding = new Padding(0)
        };
        _totalsStack = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            ColumnCount = 1,
            RowCount = 10,
            BackColor = Color.Black
        };
        _totalsStack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        _coreLabel = CreateSectionLabel("Core and Sustain");
        _combatLabel = CreateSectionLabel("Combat and Utility");
        _movementLabel = CreateSectionLabel("Movement");
        _defenseLabel = CreateSectionLabel("Defense and Resistance");
        _statusLabel = CreateSectionLabel("Status and Debuff Resistances");

        _coreGrid = CreateTotalsGrid();
        _combatGrid = CreateTotalsGrid();
        _movementGrid = CreateTotalsGrid();
        _defenseGrid = CreateTotalsGrid();
        _statusGrid = CreateTotalsGrid();

        AddTotalsSection(_coreLabel, _coreGrid);
        AddTotalsSection(_combatLabel, _combatGrid);
        AddTotalsSection(_movementLabel, _movementGrid);
        AddTotalsSection(_defenseLabel, _defenseGrid);
        AddTotalsSection(_statusLabel, _statusGrid);

        _totalsScrollPanel.Controls.Add(_totalsStack);
        totalsHost.Controls.Add(_totalsScrollPanel, 0, 1);
        _detailTotalsSplit.Panel2.Controls.Add(totalsHost);

        _emptyStatePanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Black,
            Visible = false
        };
        _emptyLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = "No real pet actors are active in this build.\r\nOnce a build summons a persistent pet actor, its powers and totals will appear here.",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold)
        };
        _emptyStatePanel.Controls.Add(_emptyLabel);

        var contentHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Black
        };
        contentHost.Controls.Add(_emptyStatePanel);
        contentHost.Controls.Add(_bodySplit);

        rootLayout.Controls.Add(_headerPanel, 0, 0);
        rootLayout.Controls.Add(_summaryMetrics, 0, 1);
        rootLayout.Controls.Add(_previewStatePanel, 0, 2);
        rootLayout.Controls.Add(ribbonHost, 0, 3);
        rootLayout.Controls.Add(contentHost, 0, 4);
        Controls.Add(rootLayout);

        Load += OnLoad;
        SizeChanged += OnFormSizeChanged;
        FormClosed += OnFormClosed;
    }

    public void UpdateData(bool refresh = false)
    {
        UpdateData(_selectedEntityUid, _selectedSourceHistoryIndex);
    }

    public void UpdateData(string? entityUid, int sourceHistoryIndex = -1)
    {
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

        foreach (var panel in _summaryMetrics.Controls.OfType<Panel>())
        {
            panel.BackColor = theme.Card;
            panel.Invalidate();
        }

        foreach (var panel in _previewStatePanel.Controls.OfType<Panel>())
        {
            panel.BackColor = theme.Card;
            panel.Invalidate();
        }

        foreach (var checkBox in _previewStatePanel.Controls.OfType<Panel>().SelectMany(panel => panel.Controls.OfType<FlowLayoutPanel>()).SelectMany(flow => flow.Controls.OfType<CheckBox>()))
        {
            checkBox.ForeColor = theme.Text;
            checkBox.BackColor = theme.Card;
        }

        _actorRibbon.Invalidate();
        _powerGrid.Invalidate();
        _detailView.Invalidate(true);
        Invalidate(true);
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        if (MidsContext.Config?.EntityDetailsLocation != null)
        {
            Location = (Point)MidsContext.Config.EntityDetailsLocation;
        }
        else if (Owner != null)
        {
            Location = new Point(Owner.Left + 32, Owner.Top + 32);
        }

        if (MidsContext.Character != null)
        {
            MidsContext.Character.AlignmentChanged += CharacterOnAlignmentChanged;
        }

        ThemeManager.ThemeChanged += OnThemeChanged;
        UpdateColorTheme(MidsContext.Character?.Alignment ?? Enums.Alignment.Hero);
        BeginInvoke(new Action(ApplySplitLayout));
        UpdateData(_selectedEntityUid, _selectedSourceHistoryIndex);
    }

    private void OnFormSizeChanged(object? sender, EventArgs e)
    {
        ApplySplitLayout();
    }

    private void OnFormClosed(object? sender, EventArgs e)
    {
        if (MidsContext.Config != null)
        {
            MidsContext.Config.EntityDetailsLocation = Location;
        }

        if (MidsContext.Character != null)
        {
            MidsContext.Character.AlignmentChanged -= CharacterOnAlignmentChanged;
        }

        ThemeManager.ThemeChanged -= OnThemeChanged;
    }

    private void OnThemeChanged()
    {
        UpdateColorTheme(MidsContext.Character?.Alignment ?? Enums.Alignment.Hero);
    }

    private void CharacterOnAlignmentChanged(object? sender, Enums.Alignment alignment)
    {
        UpdateColorTheme(alignment);
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

        using var brush = new LinearGradientBrush(bounds, theme.HeaderTop, theme.HeaderBottom, LinearGradientMode.Vertical);
        using var borderPen = new Pen(theme.Border);
        using var innerPen = new Pen(Color.FromArgb(110, theme.GridHeaderBorder));
        g.FillRectangle(brush, bounds);
        g.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

        var inner = Rectangle.Inflate(bounds, -6, -6);
        if (inner.Width > 0 && inner.Height > 0)
        {
            g.DrawRectangle(innerPen, inner.X, inner.Y, inner.Width - 1, inner.Height - 1);
        }
    }

    private void ApplySplitLayout()
    {
        ApplySplitLayout(_bodySplit, preferredDistance: 620, panel1Min: 500, panel2Min: 420);
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

        using var brush = new SolidBrush(theme.Card);
        using var borderPen = new Pen(theme.Border);
        using var accentPen = new Pen(Color.FromArgb(130, theme.GridHeaderBorder));
        e.Graphics.FillRectangle(brush, bounds);
        e.Graphics.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        e.Graphics.DrawLine(accentPen, bounds.X + 1, 24, bounds.Right - 2, 24);
    }

    private void AddMetric(string key, string title)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 8, 0),
            Padding = new Padding(10, 6, 10, 6)
        };
        panel.Paint += SummaryMetricPanelOnPaint;

        var titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 18,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            Text = title,
            TextAlign = ContentAlignment.MiddleLeft
        };
        var valueLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
            Text = "--",
            TextAlign = ContentAlignment.MiddleLeft
        };

        panel.Controls.Add(valueLabel);
        panel.Controls.Add(titleLabel);
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

    private Panel CreatePreviewStateGroup(string title, FlowLayoutPanel flow)
    {
        var host = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 8, 0),
            Padding = new Padding(10, 6, 10, 8)
        };
        host.Paint += SummaryMetricPanelOnPaint;

        var label = CreateSectionLabel(title);
        label.Dock = DockStyle.Top;
        label.Height = 18;
        label.Font = new Font("Segoe UI", 8.75f, FontStyle.Bold);

        flow.Dock = DockStyle.Fill;
        host.Controls.Add(flow);
        host.Controls.Add(label);
        return host;
    }

    private static FlowLayoutPanel CreatePreviewStateFlow()
    {
        return new FlowLayoutPanel
        {
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0),
            Padding = new Padding(0, 4, 0, 0)
        };
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

    private void DetailViewOnEntityDetails(string entityUid, HashSet<string> powers, int basePowerHistoryIdx, PetInfo petInfo)
    {
        var rosterMatch = _roster.FirstOrDefault(actor =>
            actor.EntityUid.Equals(entityUid, StringComparison.OrdinalIgnoreCase)
            && (basePowerHistoryIdx < 0 || actor.SourceHistoryIndex == basePowerHistoryIdx));

        if (rosterMatch != null)
        {
            SetSelectedActor(rosterMatch);
            LoadActorSnapshot(rosterMatch);
            return;
        }

        using var details = new FrmEntityDetails(entityUid, powers, petInfo);
        details.ShowDialog(this);
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
        _currentSnapshot = _toon.GeneratePetActorSnapshot(actor, GetPreviewState(actor));
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
        PopulateTotals();
        RefreshPowerSurface();
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

        var sourceDescription = PetActorPowerResolver.DescribePowerSource(_currentSnapshot.ResolvedPowers[_selectedPowerIndex]);
        _detailView.SetActorDataInternal(
            _currentSnapshot.BasePowers[_selectedPowerIndex],
            _currentSnapshot.BuffedPowers[_selectedPowerIndex],
            _currentSnapshot.Recipient.ClassName,
            _currentSnapshot.Totals,
            _currentSnapshot.ResolvedPowers[_selectedPowerIndex].SourceHistoryIndex,
            sourceDescription,
            _currentSnapshot.AppliedBonusEntries,
            _currentSnapshot.CalculationSnapshot);
        _detailView.DisplayTotals();
        _detailView.DisplayBonuses();
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
        var checkBox = new CheckBox
        {
            AutoSize = true,
            Appearance = Appearance.Normal,
            Checked = isChecked,
            FlatStyle = FlatStyle.Standard,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = theme.Text,
            BackColor = theme.Card,
            Margin = new Padding(0, 0, 14, 4),
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

        RefreshCurrentActorSnapshot();
    }

    private void PetInRangeToggleOnCheckedChanged(object? sender, EventArgs e)
    {
        if (_suppressPreviewStateEvents || _currentSnapshot == null || sender is not CheckBox checkBox)
        {
            return;
        }

        var state = GetMutablePreviewState(_currentSnapshot.RosterItem);
        state.InRange = checkBox.Checked;
        RefreshCurrentActorSnapshot();
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

        RefreshCurrentActorSnapshot();
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
        _currentSnapshot = null;
        _selectedPowerIndex = -1;
        _emptyStatePanel.Visible = true;
        _bodySplit.Visible = false;
        _nameLabel.Text = "No real pet actors";
        _subTitleLabel.Text = "This build does not currently have any persistent summon actors.";
        _classLabel.Text = string.Empty;
        _countLabel.Text = string.Empty;
        _tagsLabel.Text = string.Empty;
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
}

using System.Text.RegularExpressions;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Forms
{
    public partial class FrmCombatContext : Form
    {
        private enum CombatSection
        {
            Context,
            Player,
            Target,
            Assassination,
            Opportunity,
            Defiance,
            Vigilance,
            CosmicBalance,
            DarkSustenance
        }

        private sealed record RelativeLevelOption(int Value)
        {
            public override string ToString() => Value == 0 ? "Default (0)" : $"{(Value > 0 ? "+" : string.Empty)}{Value}";
        }

        private sealed record TargetProfileOption(CombatTargetProfileId Id, string DisplayName)
        {
            public override string ToString() => DisplayName;
        }

        private sealed record TeammateArchetypeOption(string Value, string DisplayName)
        {
            public override string ToString() => DisplayName;
        }

        private sealed record DefianceCountOption(int Value)
        {
            public override string ToString() => Value.ToString();
        }

        private sealed class TeamCountRowControls
        {
            public required NumericUpDown CountUpDown { get; init; }
        }

        private sealed class VigilanceRowControls
        {
            public required Label NameLabel { get; init; }
            public required MidsTrackBar HpTrackBar { get; init; }
            public required Label HpValueLabel { get; init; }
            public required CheckBox InRangeCheckBox { get; init; }
        }

        private const int MaxMembers = TeamContextDefaults.MaxTeammates;

        private readonly Action _refreshInfo;
        private readonly Dictionary<CombatSection, MidsVectorButton> _sectionButtons = new();
        private readonly Dictionary<CombatSection, Panel> _sectionPanels = new();
        private readonly List<TeammateArchetypeOption> _teamArchetypeOptions = [];
        private readonly Dictionary<string, TeamCountRowControls> _teamCountRows = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, VigilanceRowControls> _vigilanceRows = new();
        private readonly ToolTip _toolTip = new();

        private CombatSection _selectedSection = CombatSection.Context;
        private bool _suppressUiEvents;
        private bool _isShuttingDown;
        private int? _previousPlayerHpValue = 100;

        private TableLayoutPanel? _rootLayout;
        private TableLayoutPanel? _mainLayout;
        private Panel? _navigationSurface;
        private FlowLayoutPanel? _navigationRail;
        private Panel? _contentHost;
        private FlowLayoutPanel? _actionBar;

        private MidsVectorButton? _contextButton;
        private MidsVectorButton? _playerButton;
        private MidsVectorButton? _targetButton;
        private MidsVectorButton? _assassinationButton;
        private MidsVectorButton? _opportunityButton;
        private MidsVectorButton? _defianceButton;
        private MidsVectorButton? _vigilanceButton;
        private MidsVectorButton? _cosmicBalanceButton;
        private MidsVectorButton? _darkSustenanceButton;

        private Panel? _contextPage;
        private Panel? _playerPage;
        private Panel? _targetPage;
        private Panel? _assassinationPage;
        private Panel? _opportunityPage;
        private Panel? _defiancePage;
        private Panel? _vigilancePage;
        private Panel? _cosmicBalancePage;
        private Panel? _darkSustenancePage;

        private Label? _selectedRelativeLevelValue;
        private MidsDropDownList? _enemyRelativeLevelCombo;
        private Label? _combatSupportStatusLabel;

        private MidsTrackBar? _playerHpTrackBar;
        private Label? _playerHpValueLabel;
        private MidsTrackBar? _playerEndTrackBar;
        private Label? _playerEndValueLabel;
        private MidsVectorButton? _playerAliveButton;
        private MidsVectorButton? _playerDefeatedButton;

        private MidsTrackBar? _targetHpTrackBar;
        private Label? _targetHpValueLabel;
        private MidsTrackBar? _targetEndTrackBar;
        private Label? _targetEndValueLabel;
        private MidsDropDownList? _targetProfileCombo;
        private Label? _targetProfileSummaryValue;
        private Label? _targetClassSummaryValue;
        private CheckBox? _targetHeldCheckBox;
        private CheckBox? _targetImmobilizedCheckBox;
        private CheckBox? _targetStunnedCheckBox;
        private CheckBox? _targetTerrorizedCheckBox;
        private CheckBox? _targetSleptRecentlyCheckBox;

        private MidsTrackBar? _opportunityMeterTrackBar;
        private Label? _opportunityMeterValueLabel;
        private Label? _opportunityCurrentMeterValue;
        private Label? _opportunityReadyValue;

        private MidsTrackBar? _assassinationStacksTrackBar;
        private Label? _assassinationStacksValueLabel;
        private Label? _assassinationCurrentStacksValue;
        private Label? _assassinationCritBonusValue;

        private Label? _defianceTotalBonusValue;
        private Label? _defianceActiveSourcesValue;
        private Panel? _defianceCard;
        private TableLayoutPanel? _defianceGrid;

        private Panel? _sharedTeamCountsCard;
        private Panel? _assassinationTeamCountsHost;
        private Panel? _vigilanceTeamCountsHost;
        private Panel? _cosmicBalanceTeamCountsHost;
        private Panel? _darkSustenanceTeamCountsHost;
        private Label? _teamTotalMembersValue;
        private Label? _teamRemainingSlotsValue;
        private TableLayoutPanel? _teamMembersGrid;
        private Panel? _vigilanceCard;
        private TableLayoutPanel? _vigilanceGrid;
        private MidsVectorButton? _topMostButton;
        private MidsVectorButton? _resetSectionButton;
        private MidsVectorButton? _resetAllButton;
        private MidsVectorButton? _closeButton;

        public FrmCombatContext(Action refreshInfo)
        {
            _refreshInfo = refreshInfo ?? (() => { });

            InitializeComponent();
            BuildInterface();
            HookEvents();
        }

        private void HookEvents()
        {
            Load += FrmCombatContext_Load;
            FormClosing += FrmCombatContext_FormClosing;
            VisibleChanged += FrmCombatContext_VisibleChanged;
            Disposed += FrmCombatContext_Disposed;
        }

        private void FrmCombatContext_Load(object? sender, EventArgs e)
        {
            Icon = MRBResourceLib.Resources.MRB_Icon_Concept;

            if (MidsContext.Character != null)
            {
                MidsContext.Character.AlignmentChanged += CharacterOnAlignmentChanged;
            }

            if (!DesignMode)
            {
                ThemeManager.ThemeChanged += OnThemeChanged;
            }

            ApplyTheme();
            RefreshFromConfig();
            SetSelectedSection(CombatSection.Context);
        }

        private void FrmCombatContext_Disposed(object? sender, EventArgs e)
        {
            _isShuttingDown = true;
            if (MidsContext.Character != null)
            {
                MidsContext.Character.AlignmentChanged -= CharacterOnAlignmentChanged;
            }

            if (!DesignMode)
            {
                ThemeManager.ThemeChanged -= OnThemeChanged;
            }

            try
            {
                _toolTip.Active = false;
                _toolTip.RemoveAll();
                _toolTip.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        private void FrmCombatContext_VisibleChanged(object? sender, EventArgs e)
        {
            if (_isShuttingDown)
            {
                return;
            }

            if (Visible)
            {
                ResumeToolTips();
                CenterToOwner();
                RefreshFromConfig();
            }
            else
            {
                SuppressToolTips();
            }
        }

        private void FrmCombatContext_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_isShuttingDown)
            {
                return;
            }

            if (e.CloseReason == CloseReason.UserClosing)
            {
                SuppressToolTips();
                e.Cancel = true;
                Hide();
            }
        }

        public void PrepareForOwnerShutdown()
        {
            if (_isShuttingDown)
            {
                return;
            }

            _isShuttingDown = true;
            SuppressToolTips();
        }

        public void SelectContextSection()
        {
            SetSelectedSection(CombatSection.Context);
        }

        public void RefreshFromConfig()
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            _suppressUiEvents = true;
            try
            {
                UpdateSharedTeamCountsPlacement();
                UpdateContextControls();
                UpdatePlayerControls();
                UpdateTargetControls();
                UpdateAssassinationControls();
                UpdateOpportunityControls();
                UpdateDefianceControls();
                UpdateTeamControls();
                UpdateSectionVisibility();
                UpdateTopMostButtonState();
                UpdateResetButtonTooltip();
            }
            finally
            {
                _suppressUiEvents = false;
            }
        }

        public void FeedbackUpdate(string settingName, float val)
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            switch (settingName.ToLowerInvariant())
            {
                case "cfg.player.hp":
                    MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent = (int)Math.Round(val);
                    if (val > 0)
                    {
                        _previousPlayerHpValue = (int)Math.Round(val);
                        MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive = true;
                    }
                    else
                    {
                        MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive = false;
                    }

                    break;

                case "cfg.player.end":
                    MidsContext.Config.CombatContextSettings.PlayerSettings.EndPercent = (int)Math.Round(val);
                    break;

                case "cfg.target.hp":
                    MidsContext.Config.CombatContextSettings.TargetSettings.HpPercent = (int)Math.Round(val);
                    break;

                case "cfg.target.end":
                    MidsContext.Config.CombatContextSettings.TargetSettings.EndPercent = (int)Math.Round(val);
                    break;

                case "cfg.target.profileid":
                    MidsContext.Config.CombatContextSettings.TargetSettings.ProfileId = (int)Math.Round(val);
                    break;
                case "cfg.target.held":
                    MidsContext.Config.CombatContextSettings.TargetSettings.Held = val > 0.5f;
                    break;
                case "cfg.target.immobilized":
                    MidsContext.Config.CombatContextSettings.TargetSettings.Immobilized = val > 0.5f;
                    break;
                case "cfg.target.stunned":
                    MidsContext.Config.CombatContextSettings.TargetSettings.Stunned = val > 0.5f;
                    break;
                case "cfg.target.terrorized":
                    MidsContext.Config.CombatContextSettings.TargetSettings.Terrorized = val > 0.5f;
                    break;
                case "cfg.target.sleptrecently":
                    MidsContext.Config.CombatContextSettings.TargetSettings.SleptRecently = val > 0.5f;
                    break;
                case "cfg.target.vulnerabilityactive":
                    MidsContext.Config.CombatContextSettings.TargetSettings.VulnerabilityActive = val > 0.5f;
                    break;

                case "cfg.target.opportunitystate":
                    MidsContext.Config.CombatContextSettings.TargetSettings.OpportunityState = (int)Math.Round(val);
                    MidsContext.Config.CombatContextSettings.TargetSettings.VulnerabilityActive =
                        MidsContext.Config.CombatContextSettings.TargetSettings.OpportunityState != (int)CombatTargetOpportunityState.None;
                    break;
            }

            RefreshFromConfig();
        }

        private void BuildInterface()
        {
            SuspendLayout();

            _rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(16, 16, 16, 12),
                BackColor = Color.Transparent
            };
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _navigationSurface = CreateCardPanel();
            _navigationSurface.Dock = DockStyle.Fill;
            _navigationSurface.Padding = new Padding(10);

            _navigationRail = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };

            _contextButton = CreateSectionButton("Context", CombatSection.Context);
            _playerButton = CreateSectionButton("Player", CombatSection.Player);
            _targetButton = CreateSectionButton("Target", CombatSection.Target);
            _assassinationButton = CreateSectionButton("Assassination", CombatSection.Assassination);
            _opportunityButton = CreateSectionButton("Opportunity", CombatSection.Opportunity);
            _defianceButton = CreateSectionButton("Defiance", CombatSection.Defiance);
            _vigilanceButton = CreateSectionButton("Vigilance", CombatSection.Vigilance);
            _cosmicBalanceButton = CreateSectionButton("Cosmic Balance", CombatSection.CosmicBalance);
            _darkSustenanceButton = CreateSectionButton("Dark Sustenance", CombatSection.DarkSustenance);

            _navigationRail.Controls.AddRange([
                _contextButton,
                _playerButton,
                _targetButton,
                _assassinationButton,
                _opportunityButton,
                _defianceButton,
                _vigilanceButton,
                _cosmicBalanceButton,
                _darkSustenanceButton
            ]);
            _navigationSurface.Controls.Add(_navigationRail);

            _contentHost = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(14, 0, 0, 0),
                BackColor = Color.Transparent
            };

            _contextPage = CreateContextPage();
            _playerPage = CreatePlayerPage();
            _targetPage = CreateTargetPage();
            _assassinationPage = CreateAssassinationPage();
            _opportunityPage = CreateOpportunityPage();
            _defiancePage = CreateDefiancePage();
            _vigilancePage = CreateVigilancePage();
            _cosmicBalancePage = CreateCosmicBalancePage();
            _darkSustenancePage = CreateDarkSustenancePage();

            _sectionPanels[CombatSection.Context] = _contextPage;
            _sectionPanels[CombatSection.Player] = _playerPage;
            _sectionPanels[CombatSection.Target] = _targetPage;
            _sectionPanels[CombatSection.Assassination] = _assassinationPage;
            _sectionPanels[CombatSection.Opportunity] = _opportunityPage;
            _sectionPanels[CombatSection.Defiance] = _defiancePage;
            _sectionPanels[CombatSection.Vigilance] = _vigilancePage;
            _sectionPanels[CombatSection.CosmicBalance] = _cosmicBalancePage;
            _sectionPanels[CombatSection.DarkSustenance] = _darkSustenancePage;

            foreach (var panel in _sectionPanels.Values)
            {
                panel.Dock = DockStyle.Fill;
                panel.Visible = false;
                _contentHost.Controls.Add(panel);
            }

            _mainLayout.Controls.Add(_navigationSurface, 0, 0);
            _mainLayout.Controls.Add(_contentHost, 1, 0);

            _actionBar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0, 12, 0, 0),
                Margin = Padding.Empty,
                BackColor = Color.Transparent
            };

            _closeButton = CreateActionButton("Close", CloseButtonOnClick);
            _resetAllButton = CreateActionButton("Reset All", ResetAllButtonOnClick);
            _resetSectionButton = CreateActionButton("Reset Section", ResetSectionButtonOnClick);
            _topMostButton = CreateToggleButton("Top Most", TopMostButtonOnClick);

            _actionBar.Controls.AddRange([_closeButton, _resetAllButton, _resetSectionButton, _topMostButton]);

            _rootLayout.Controls.Add(_mainLayout, 0, 0);
            _rootLayout.Controls.Add(_actionBar, 0, 1);

            Controls.Add(_rootLayout);

            ResumeLayout(true);
        }

        private Panel CreateContextPage()
        {
            var page = CreatePageHost();
            var layout = CreatePageLayout();
            page.ContentPanel.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Combat Context",
                "Set the encounter assumptions Mids should use while it plans your build."));

            var summaryLayout = CreateSummaryLayout(1);
            summaryLayout.Controls.Add(CreateSummaryCard("Selected Relative Level", out _selectedRelativeLevelValue), 0, 0);
            layout.Controls.Add(summaryLayout);

            var encounterCard = CreateAutoSizeCardPanel();
            encounterCard.Dock = DockStyle.Top;
            encounterCard.Padding = new Padding(18);
            encounterCard.Margin = new Padding(0, 0, 0, 14);

            var encounterLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                BackColor = Color.Transparent
            };
            encounterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            encounterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var relativeLevelLabel = CreateFieldLabel("Enemy Relative Level");
            _enemyRelativeLevelCombo = new MidsDropDownList
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 10),
                Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold),
                PlaceholderText = "Select a relative level"
            };

            PopulateEnemyRelativeLevelOptions();

            _enemyRelativeLevelCombo.SelectedIndexChanged += EnemyRelativeLevelComboOnSelectedIndexChanged;
            SetToolTipSafe(_enemyRelativeLevelCombo, "Choose the enemy-relative-level assumption used for planner combat math.");

            encounterLayout.Controls.Add(relativeLevelLabel, 0, 0);
            encounterLayout.Controls.Add(_enemyRelativeLevelCombo, 1, 0);

            _combatSupportStatusLabel = CreateMutedLabel(string.Empty);
            _combatSupportStatusLabel.AutoSize = true;
            _combatSupportStatusLabel.MaximumSize = new Size(560, 0);
            _combatSupportStatusLabel.Margin = new Padding(0, 2, 0, 10);

            encounterLayout.Controls.Add(CreateFieldLabel("Server Profile"), 0, 1);
            encounterLayout.Controls.Add(_combatSupportStatusLabel, 1, 1);
            encounterCard.Controls.Add(encounterLayout);
            layout.Controls.Add(encounterCard);

            return page;
        }

        private Panel CreatePlayerPage()
        {
            var page = CreatePageHost();
            var layout = CreatePageLayout();
            page.ContentPanel.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Player State",
                "These values update live so linked combat-setting powers and planner displays stay in sync."));

            var statsCard = CreateAutoSizeCardPanel();
            statsCard.Dock = DockStyle.Top;
            statsCard.Padding = new Padding(18);
            statsCard.Margin = new Padding(0, 0, 0, 14);

            var statsLayout = CreateFieldGridLayout();
            statsCard.Controls.Add(statsLayout);

            statsLayout.Controls.Add(CreateFieldLabel("HP"), 0, 0);
            statsLayout.Controls.Add(CreateSliderRow(out _playerHpTrackBar, out _playerHpValueLabel, PlayerHpTrackBarOnValueChanged), 1, 0);

            statsLayout.Controls.Add(CreateFieldLabel("Endurance"), 0, 1);
            statsLayout.Controls.Add(CreateSliderRow(out _playerEndTrackBar, out _playerEndValueLabel, PlayerEndTrackBarOnValueChanged), 1, 1);

            layout.Controls.Add(statsCard);

            var statusCard = CreateAutoSizeCardPanel();
            statusCard.Dock = DockStyle.Top;
            statusCard.Padding = new Padding(18);
            statusCard.Margin = new Padding(0, 0, 0, 14);

            var statusLayout = CreateFieldGridLayout();
            statusCard.Controls.Add(statusLayout);

            _playerAliveButton = CreateToggleButton("Alive", PlayerAliveButtonOnClick);
            _playerAliveButton.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            _playerAliveButton.ToggleText.ToggledOff = "Alive";
            _playerAliveButton.ToggleText.ToggledOn = "Alive";
            _playerAliveButton.Width = 120;

            _playerDefeatedButton = CreateToggleButton("Defeated", PlayerDefeatedButtonOnClick);
            _playerDefeatedButton.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            _playerDefeatedButton.ToggleText.ToggledOff = "Defeated";
            _playerDefeatedButton.ToggleText.ToggledOn = "Defeated";
            _playerDefeatedButton.Width = 120;

            var statusButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = false,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            statusButtons.Controls.AddRange([_playerAliveButton, _playerDefeatedButton]);

            statusLayout.Controls.Add(CreateFieldLabel("Status"), 0, 0);
            statusLayout.Controls.Add(statusButtons, 1, 0);

            layout.Controls.Add(statusCard);

            return page;
        }

        private Panel CreateTargetPage()
        {
            var page = CreatePageHost();
            var layout = CreatePageLayout();
            page.ContentPanel.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Target State",
                "Target state now includes an explicit enemy profile, so planner expressions can reason about critter class tables and boss-grade tags."));

            var summaryLayout = CreateSummaryLayout(2);
            summaryLayout.Controls.Add(CreateSummaryCard("Target Profile", out _targetProfileSummaryValue), 0, 0);
            summaryLayout.Controls.Add(CreateSummaryCard("Class Table", out _targetClassSummaryValue), 1, 0);
            layout.Controls.Add(summaryLayout);

            var profileCard = CreateAutoSizeCardPanel();
            profileCard.Dock = DockStyle.Top;
            profileCard.Padding = new Padding(18);
            profileCard.Margin = new Padding(0, 0, 0, 14);

            var profileLayout = CreateFieldGridLayout();
            profileCard.Controls.Add(profileLayout);

            _targetProfileCombo = new MidsDropDownList
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 10),
                Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold),
                PlaceholderText = "Select a target profile"
            };
            PopulateTargetProfileOptions();
            _targetProfileCombo.SelectedIndexChanged += TargetProfileComboOnSelectedIndexChanged;
            SetToolTipSafe(_targetProfileCombo, "Choose the enemy profile used for target class-table and target-tag planner expressions.");

            profileLayout.Controls.Add(CreateFieldLabel("Target Profile"), 0, 0);
            profileLayout.Controls.Add(_targetProfileCombo, 1, 0);

            layout.Controls.Add(profileCard);

            var statsCard = CreateAutoSizeCardPanel();
            statsCard.Dock = DockStyle.Top;
            statsCard.Padding = new Padding(18);
            statsCard.Margin = new Padding(0, 0, 0, 14);

            var statsLayout = CreateFieldGridLayout();
            statsCard.Controls.Add(statsLayout);

            statsLayout.Controls.Add(CreateFieldLabel("HP"), 0, 0);
            statsLayout.Controls.Add(CreateSliderRow(out _targetHpTrackBar, out _targetHpValueLabel, TargetHpTrackBarOnValueChanged), 1, 0);

            statsLayout.Controls.Add(CreateFieldLabel("Endurance"), 0, 1);
            statsLayout.Controls.Add(CreateSliderRow(out _targetEndTrackBar, out _targetEndValueLabel, TargetEndTrackBarOnValueChanged), 1, 1);

            layout.Controls.Add(statsCard);

            var stateCard = CreateAutoSizeCardPanel();
            stateCard.Dock = DockStyle.Top;
            stateCard.Padding = new Padding(18);
            stateCard.Margin = new Padding(0, 0, 0, 14);

            var stateLayout = CreateFieldGridLayout();
            stateCard.Controls.Add(stateLayout);

            var statusPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Margin = new Padding(0, 0, 0, 6),
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            statusPanel.Controls.AddRange(
            [
                CreateTargetStateCheckBox("Held", TargetHeldCheckBoxOnCheckedChanged, out _targetHeldCheckBox),
                CreateTargetStateCheckBox("Immobilized", TargetImmobilizedCheckBoxOnCheckedChanged, out _targetImmobilizedCheckBox),
                CreateTargetStateCheckBox("Stunned", TargetStunnedCheckBoxOnCheckedChanged, out _targetStunnedCheckBox),
                CreateTargetStateCheckBox("Terrorized", TargetTerrorizedCheckBoxOnCheckedChanged, out _targetTerrorizedCheckBox),
                CreateTargetStateCheckBox("Slept Recently", TargetSleptRecentlyCheckBoxOnCheckedChanged, out _targetSleptRecentlyCheckBox)
            ]);

            stateLayout.Controls.Add(CreateFieldLabel("Target Status"), 0, 0);
            stateLayout.Controls.Add(statusPanel, 1, 0);

            layout.Controls.Add(stateCard);

            return page;
        }

        private Panel CreateVigilancePage()
        {
            var page = CreatePageHost();
            var layout = CreatePageLayout();
            page.ContentPanel.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Vigilance",
                "For Defenders, nearby teammate assumptions live here. Team counts feed Vigilance, and each teammate expands into its own HP and range snapshot row."));

            _vigilanceTeamCountsHost = CreateSharedCardHost();
            layout.Controls.Add(_vigilanceTeamCountsHost);

            _vigilanceCard = CreateAutoSizeCardPanel();
            _vigilanceCard.Dock = DockStyle.Top;
            _vigilanceCard.Padding = new Padding(18);
            _vigilanceCard.Margin = new Padding(0, 0, 0, 14);

            var vigilanceLayout = CreatePageLayout();
            _vigilanceCard.Controls.Add(vigilanceLayout);
            vigilanceLayout.Controls.Add(CreatePageHeader(
                "Vigilance",
                "For Defenders, each teammate can contribute endurance discount based on missing HP while they are in range."));

            _vigilanceGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            _vigilanceGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            vigilanceLayout.Controls.Add(_vigilanceGrid);

            layout.Controls.Add(_vigilanceCard);

            return page;
        }

        private Panel CreateCosmicBalancePage()
        {
            var page = CreatePageHost();
            var layout = CreatePageLayout();
            page.ContentPanel.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Cosmic Balance",
                "For Peacebringers, nearby teammate assumptions live here. The counts below feed Cosmic Balance based on who is close enough to contribute."));

            _cosmicBalanceTeamCountsHost = CreateSharedCardHost();
            layout.Controls.Add(_cosmicBalanceTeamCountsHost);

            return page;
        }

        private Panel CreateDarkSustenancePage()
        {
            var page = CreatePageHost();
            var layout = CreatePageLayout();
            page.ContentPanel.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Dark Sustenance",
                "For Warshades, nearby teammate assumptions live here. The counts below feed Dark Sustenance based on who is close enough to contribute."));

            _darkSustenanceTeamCountsHost = CreateSharedCardHost();
            layout.Controls.Add(_darkSustenanceTeamCountsHost);

            return page;
        }

        private Panel CreateOpportunityPage()
        {
            var page = CreatePageHost();
            var layout = CreatePageLayout();
            page.ContentPanel.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Opportunity",
                "For Homecoming Sentinels, set the assumed current Opportunity meter here. The imported Opportunity powers continue to handle the real meter-based math."));

            var summaryLayout = CreateSummaryLayout(2);
            summaryLayout.Controls.Add(CreateSummaryCard("Current Meter", out _opportunityCurrentMeterValue), 0, 0);
            summaryLayout.Controls.Add(CreateSummaryCard("Ready (>= 50%)", out _opportunityReadyValue), 1, 0);
            layout.Controls.Add(summaryLayout);

            var meterCard = CreateAutoSizeCardPanel();
            meterCard.Dock = DockStyle.Top;
            meterCard.Padding = new Padding(18);
            meterCard.Margin = new Padding(0, 0, 0, 14);

            var meterLayout = CreateFieldGridLayout();
            meterCard.Controls.Add(meterLayout);

            meterLayout.Controls.Add(CreateFieldLabel("Opportunity Meter"), 0, 0);
            meterLayout.Controls.Add(CreateSliderRow(out _opportunityMeterTrackBar, out _opportunityMeterValueLabel, OpportunityMeterTrackBarOnValueChanged), 1, 0);
            SetToolTipSafe(_opportunityMeterTrackBar, "Set the current Opportunity meter percent for Homecoming Sentinel planner math.");

            layout.Controls.Add(meterCard);

            return page;
        }

        private Panel CreateAssassinationPage()
        {
            var page = CreatePageHost();
            var layout = CreatePageLayout();
            page.ContentPanel.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Assassination",
                "For Stalkers, set the assumed current Assassin's Focus stacks here. The visible Assassination inherent remains passive, From Hide still controls the hidden attack branch in the inherent grid, and nearby teammate assumptions drive ASTeamCrit on regular attacks."));

            var summaryLayout = CreateSummaryLayout(2);
            summaryLayout.Controls.Add(CreateSummaryCard("Current Focus", out _assassinationCurrentStacksValue), 0, 0);
            summaryLayout.Controls.Add(CreateSummaryCard("Assassin's Strike Crit Bonus", out _assassinationCritBonusValue), 1, 0);
            layout.Controls.Add(summaryLayout);

            var focusCard = CreateAutoSizeCardPanel();
            focusCard.Dock = DockStyle.Top;
            focusCard.Padding = new Padding(18);
            focusCard.Margin = new Padding(0, 0, 0, 14);

            var focusLayout = CreateFieldGridLayout();
            focusCard.Controls.Add(focusLayout);

            focusLayout.Controls.Add(CreateFieldLabel("Assassin's Focus Stacks"), 0, 0);
            focusLayout.Controls.Add(CreateSliderRow(out _assassinationStacksTrackBar, out _assassinationStacksValueLabel, AssassinationStacksTrackBarOnValueChanged), 1, 0);
            if (_assassinationStacksTrackBar != null)
            {
                _assassinationStacksTrackBar.Minimum = 0;
                _assassinationStacksTrackBar.Maximum = 3;
                _assassinationStacksTrackBar.SmallChange = 1;
                _assassinationStacksTrackBar.LargeChange = 1;
                _assassinationStacksTrackBar.Value = 0;
                SetToolTipSafe(_assassinationStacksTrackBar, "Set the assumed current Assassin's Focus stacks for non-hidden Assassin's Strike planner math.");
            }

            layout.Controls.Add(focusCard);

            _assassinationTeamCountsHost = CreateSharedCardHost();
            layout.Controls.Add(_assassinationTeamCountsHost);

            return page;
        }

        private Panel CreateSharedCardHost()
        {
            return new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
        }

        private void EnsureSharedTeamCountsCard()
        {
            if (_sharedTeamCountsCard != null)
            {
                return;
            }

            _sharedTeamCountsCard = CreateAutoSizeCardPanel();
            _sharedTeamCountsCard.Dock = DockStyle.Top;
            _sharedTeamCountsCard.Padding = new Padding(18);
            _sharedTeamCountsCard.Margin = new Padding(0, 0, 0, 14);

            var cardLayout = CreatePageLayout();
            _sharedTeamCountsCard.Controls.Add(cardLayout);

            cardLayout.Controls.Add(CreatePageHeader(
                "Nearby Teammates",
                "These shared team assumptions are used by inherents that care about nearby teammates, such as Stalker ASTeamCrit, Defender Vigilance, and Kheldian team-based bonuses."));

            var summaryLayout = CreateSummaryLayout(2);
            summaryLayout.Controls.Add(CreateSummaryCard("Total Members", out _teamTotalMembersValue), 0, 0);
            summaryLayout.Controls.Add(CreateSummaryCard("Remaining Slots", out _teamRemainingSlotsValue), 1, 0);
            cardLayout.Controls.Add(summaryLayout);

            _teamMembersGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            _teamMembersGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _teamMembersGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            BuildTeamRows();
            cardLayout.Controls.Add(_teamMembersGrid);
        }

        private void UpdateSharedTeamCountsPlacement()
        {
            EnsureSharedTeamCountsCard();

            if (_sharedTeamCountsCard == null)
            {
                return;
            }

            Panel? targetHost = null;
            if (SupportsAssassinationCombatSection())
            {
                targetHost = _assassinationTeamCountsHost;
            }
            else if (SupportsVigilanceCombatSection())
            {
                targetHost = _vigilanceTeamCountsHost;
            }
            else if (SupportsCosmicBalanceCombatSection())
            {
                targetHost = _cosmicBalanceTeamCountsHost;
            }
            else if (SupportsDarkSustenanceCombatSection())
            {
                targetHost = _darkSustenanceTeamCountsHost;
            }

            ClearSharedCardHostIfInactive(_assassinationTeamCountsHost, targetHost);
            ClearSharedCardHostIfInactive(_vigilanceTeamCountsHost, targetHost);
            ClearSharedCardHostIfInactive(_cosmicBalanceTeamCountsHost, targetHost);
            ClearSharedCardHostIfInactive(_darkSustenanceTeamCountsHost, targetHost);

            if (targetHost == null)
            {
                _sharedTeamCountsCard.Parent?.Controls.Remove(_sharedTeamCountsCard);
                return;
            }

            if (!targetHost.Controls.Contains(_sharedTeamCountsCard))
            {
                targetHost.SuspendLayout();
                try
                {
                    _sharedTeamCountsCard.Parent?.Controls.Remove(_sharedTeamCountsCard);
                    targetHost.Controls.Clear();
                    targetHost.Controls.Add(_sharedTeamCountsCard);
                }
                finally
                {
                    targetHost.ResumeLayout(true);
                }
            }
        }

        private static void ClearSharedCardHostIfInactive(Panel? host, Panel? activeHost)
        {
            if (host != null && !ReferenceEquals(host, activeHost))
            {
                host.Controls.Clear();
            }
        }

        private Panel CreateDefiancePage()
        {
            var page = CreatePageHost();
            var layout = CreatePageLayout();
            page.ContentPanel.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Defiance",
                "For Blasters, choose which taken powers are currently feeding Defiance. The total bonus is computed from the real imported grant rows on the active payload path."));

            var summaryLayout = CreateSummaryLayout(2);
            summaryLayout.Controls.Add(CreateSummaryCard("Current Bonus", out _defianceTotalBonusValue), 0, 0);
            summaryLayout.Controls.Add(CreateSummaryCard("Active Sources", out _defianceActiveSourcesValue), 1, 0);
            layout.Controls.Add(summaryLayout);

            _defianceCard = CreateAutoSizeCardPanel();
            _defianceCard.Dock = DockStyle.Top;
            _defianceCard.Padding = new Padding(18);
            _defianceCard.Margin = new Padding(0, 0, 0, 14);

            var defianceLayout = CreatePageLayout();
            _defianceCard.Controls.Add(defianceLayout);
            defianceLayout.Controls.Add(CreatePageHeader(
                "Active Contributors",
                "Each row represents one taken build power after redirect resolution. Set how many live Defiance grants from that power you want the planner to assume are currently active."));

            _defianceGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            _defianceGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            defianceLayout.Controls.Add(_defianceGrid);

            layout.Controls.Add(_defianceCard);

            return page;
        }

        private void BuildTeamRows()
        {
            if (_teamMembersGrid == null || MidsContext.Config == null)
            {
                return;
            }

            _teamArchetypeOptions.Clear();
            _teamArchetypeOptions.AddRange(GetTeamMemberDefinitions());
            _teamCountRows.Clear();
            _teamMembersGrid.Controls.Clear();
            _teamMembersGrid.ColumnStyles.Clear();
            _teamMembersGrid.RowStyles.Clear();
            _teamMembersGrid.ColumnCount = 2;
            _teamMembersGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _teamMembersGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            var rowCount = (int)Math.Ceiling(_teamArchetypeOptions.Count / 2f);
            _teamMembersGrid.RowCount = rowCount;
            for (var row = 0; row < rowCount; row++)
            {
                _teamMembersGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            for (var index = 0; index < _teamArchetypeOptions.Count; index++)
            {
                var row = index / 2;
                var column = index % 2;
                _teamMembersGrid.Controls.Add(CreateTeamCountRow(_teamArchetypeOptions[index]), column, row);
            }

            RebuildVigilanceRows();
        }

        private void UpdateTeamGridLayout()
        {
            _teamMembersGrid?.PerformLayout();
            _sharedTeamCountsCard?.PerformLayout();
            _assassinationTeamCountsHost?.PerformLayout();
            _vigilanceTeamCountsHost?.PerformLayout();
            _cosmicBalanceTeamCountsHost?.PerformLayout();
            _darkSustenanceTeamCountsHost?.PerformLayout();
            _vigilanceGrid?.PerformLayout();
            _vigilanceCard?.PerformLayout();
            _assassinationPage?.PerformLayout();
            _vigilancePage?.PerformLayout();
            _cosmicBalancePage?.PerformLayout();
            _darkSustenancePage?.PerformLayout();
        }

        private Panel CreateTeamCountRow(TeammateArchetypeOption definition)
        {
            var rowPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 0, 10, 8),
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            rowPanel.Paint += CardBorderPaint;

            var rowLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Margin = Padding.Empty,
                Padding = new Padding(10, 5, 10, 5),
                BackColor = Color.Transparent
            };
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            rowLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var nameLabel = CreateFieldLabel(definition.DisplayName);
            nameLabel.Dock = DockStyle.Fill;
            nameLabel.TextAlign = ContentAlignment.MiddleLeft;
            nameLabel.AutoSize = false;
            nameLabel.Margin = new Padding(0, 2, 12, 2);
            nameLabel.MinimumSize = new Size(0, 26);

            var countUpDown = new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 0, 0, 0),
                Minimum = 0,
                Maximum = MaxMembers,
                DecimalPlaces = 0,
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Noto Sans SemiBold", 10F, FontStyle.Bold),
                Tag = definition.Value,
                Height = 26
            };
            countUpDown.ValueChanged += TeamCountUpDownOnValueChanged;
            SetToolTipSafe(countUpDown, $"How many {definition.DisplayName} teammates to assume are currently on the team.");

            rowLayout.Controls.Add(nameLabel, 0, 0);
            rowLayout.Controls.Add(countUpDown, 1, 0);

            rowPanel.Controls.Add(rowLayout);
            _teamCountRows[definition.Value] = new TeamCountRowControls
            {
                CountUpDown = countUpDown
            };

            return rowPanel;
        }

        private void RebuildVigilanceRows()
        {
            if (_vigilanceCard == null || _vigilanceGrid == null || MidsContext.Config == null)
            {
                return;
            }

            EnsureTeamRosterSlots();

            var activeSlots = MidsContext.Config.TeamRoster
                .Select((slot, index) => new { Slot = slot, Index = index })
                .Where(entry => !string.IsNullOrWhiteSpace(entry.Slot.Archetype))
                .ToArray();

            var shouldShow = IsDefenderArchetype() && activeSlots.Length > 0;
            if (!shouldShow)
            {
                _vigilanceCard.Visible = false;
                if (_vigilanceRows.Count == 0 && _vigilanceGrid.Controls.Count == 0 && _vigilanceGrid.RowCount == 0)
                {
                    return;
                }

                _vigilanceRows.Clear();
                _vigilanceGrid.Controls.Clear();
                _vigilanceGrid.RowStyles.Clear();
                _vigilanceGrid.RowCount = 0;
                return;
            }

            var activeIndexes = activeSlots.Select(entry => entry.Index).ToArray();
            var existingIndexes = _vigilanceRows.Keys.OrderBy(index => index).ToArray();
            _vigilanceCard.Visible = true;

            if (existingIndexes.SequenceEqual(activeIndexes))
            {
                return;
            }

            _vigilanceGrid.SuspendLayout();
            try
            {
                _vigilanceRows.Clear();
                _vigilanceGrid.Controls.Clear();
                _vigilanceGrid.RowStyles.Clear();
                _vigilanceGrid.RowCount = activeSlots.Length;
                for (var row = 0; row < activeSlots.Length; row++)
                {
                    _vigilanceGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    _vigilanceGrid.Controls.Add(CreateVigilanceRow(activeSlots[row].Index), 0, row);
                }
            }
            finally
            {
                _vigilanceGrid.ResumeLayout(true);
            }
        }

        private Panel CreateVigilanceRow(int slotIndex)
        {
            var rowPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 0, 0, 8),
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            rowPanel.Paint += CardBorderPaint;

            var rowLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 3,
                Margin = Padding.Empty,
                Padding = new Padding(10, 5, 10, 5),
                BackColor = Color.Transparent
            };
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            rowLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var nameLabel = CreateFieldLabel($"Teammate {slotIndex + 1}");
            nameLabel.Dock = DockStyle.Fill;
            nameLabel.TextAlign = ContentAlignment.MiddleLeft;
            nameLabel.AutoSize = false;
            nameLabel.Margin = new Padding(0, 2, 12, 2);
            nameLabel.MinimumSize = new Size(0, 28);

            var hpRow = CreateSliderRow(out var hpTrackBar, out var hpValueLabel, VigilanceHpTrackBarOnValueChanged, compact: true);
            hpTrackBar.Tag = slotIndex;
            hpValueLabel.Text = "100%";

            var inRangeCheckBox = new CheckBox
            {
                AutoSize = true,
                Text = "In Range",
                Margin = new Padding(0, 5, 0, 0),
                Padding = Padding.Empty,
                BackColor = Color.Transparent,
                Tag = slotIndex
            };
            inRangeCheckBox.CheckedChanged += VigilanceInRangeCheckBoxOnCheckedChanged;

            rowLayout.Controls.Add(nameLabel, 0, 0);
            rowLayout.Controls.Add(hpRow, 1, 0);
            rowLayout.Controls.Add(inRangeCheckBox, 2, 0);

            rowPanel.Controls.Add(rowLayout);
            _vigilanceRows[slotIndex] = new VigilanceRowControls
            {
                NameLabel = nameLabel,
                HpTrackBar = hpTrackBar,
                HpValueLabel = hpValueLabel,
                InRangeCheckBox = inRangeCheckBox
            };

            return rowPanel;
        }

        private void RebuildDefianceRows(DefianceResolution resolution)
        {
            if (_defianceGrid == null)
            {
                return;
            }

            _defianceGrid.Controls.Clear();
            _defianceGrid.RowStyles.Clear();
            _defianceGrid.RowCount = 0;

            if (resolution.Contributors.Count == 0)
            {
                var emptyLabel = CreateMutedLabel("No taken build powers currently grant modern Defiance.");
                emptyLabel.Margin = new Padding(0, 0, 0, 8);
                _defianceGrid.RowCount = 1;
                _defianceGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                _defianceGrid.Controls.Add(emptyLabel, 0, 0);
                return;
            }

            _defianceGrid.RowCount = resolution.Contributors.Count;
            for (var row = 0; row < resolution.Contributors.Count; row++)
            {
                _defianceGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                _defianceGrid.Controls.Add(CreateDefianceContributorRow(resolution.Contributors[row]), 0, row);
            }
        }

        private Panel CreateDefianceContributorRow(DefianceContributorState contributor)
        {
            var rowPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                Margin = new Padding(0, 0, 0, 8),
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            rowPanel.Paint += CardBorderPaint;

            var rowLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Margin = Padding.Empty,
                Padding = new Padding(10, 6, 10, 6),
                BackColor = Color.Transparent
            };
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96F));

            var descriptor = contributor.Descriptor;
            var titleLabel = CreateFieldLabel(descriptor.SourceDisplayName);
            titleLabel.Margin = new Padding(0, 0, 0, 2);

            var summaryLabel = CreateMutedLabel(BuildDefianceContributorSummary(descriptor));
            summaryLabel.Margin = new Padding(0);
            summaryLabel.MaximumSize = new Size(480, 0);

            var textPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            textPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            textPanel.Controls.Add(titleLabel, 0, 0);
            textPanel.Controls.Add(summaryLabel, 0, 1);

            var countCombo = new MidsDropDownList
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(12, 4, 0, 0),
                Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold),
                Tag = descriptor
            };
            for (var count = 0; count <= descriptor.MaxActiveCount; count++)
            {
                countCombo.Items.Add(new DefianceCountOption(count));
            }

            var selectedCount = countCombo.Items
                .OfType<DefianceCountOption>()
                .FirstOrDefault(option => option.Value == contributor.ActiveCount)
                ?? new DefianceCountOption(0);
            countCombo.SelectedItem = selectedCount;
            countCombo.SelectedIndexChanged += DefianceCountComboOnSelectedIndexChanged;
            SetToolTipSafe(countCombo, "How many active Defiance grants from this power should the planner assume are currently contributing.");

            rowLayout.Controls.Add(textPanel, 0, 0);
            rowLayout.Controls.Add(countCombo, 1, 0);

            rowPanel.Controls.Add(rowLayout);
            return rowPanel;
        }

        private static string BuildDefianceContributorSummary(DefianceContributorDescriptor descriptor)
        {
            var payloadText = string.Equals(
                descriptor.SourceDisplayName,
                descriptor.ResolvedDisplayName,
                StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : $"{descriptor.ResolvedDisplayName} · ";

            return $"{payloadText}+{DisplayValueFormatter.FormatPercentFromScale(descriptor.Magnitude, 1)}% for {DisplayValueFormatter.FormatNumber(descriptor.Duration, 2)}s";
        }

        private static TableLayoutPanel CreatePageLayout()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            return layout;
        }

        private static TableLayoutPanel CreateFieldGridLayout()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            return layout;
        }

        private static TableLayoutPanel CreateSummaryLayout(int columns)
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = columns,
                Margin = new Padding(0, 0, 0, 14),
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };

            for (var i = 0; i < columns; i++)
            {
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / columns));
            }

            return layout;
        }

        private static MidsVScrollPanel CreatePageHost()
        {
            return new MidsVScrollPanel
            {
                BackColor = Color.Transparent,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
        }

        private Panel CreatePageHeader(string title, string subtitle)
        {
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 14),
                Padding = Padding.Empty
            };

            var titleLabel = new Label
            {
                AutoSize = true,
                Text = title,
                Font = new Font("Noto Sans SemiBold", 16F, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 4),
                Tag = "title"
            };

            var subtitleLabel = CreateMutedLabel(subtitle);
            subtitleLabel.MaximumSize = new Size(620, 0);
            subtitleLabel.Margin = new Padding(0);

            headerPanel.Controls.Add(subtitleLabel);
            headerPanel.Controls.Add(titleLabel);
            subtitleLabel.Dock = DockStyle.Top;
            titleLabel.Dock = DockStyle.Top;

            return headerPanel;
        }

        private Panel CreateSummaryCard(string title, out Label valueLabel)
        {
            var card = CreateCardPanel();
            card.Dock = DockStyle.Fill;
            card.Height = 92;
            card.Margin = new Padding(0, 0, 12, 0);
            card.Padding = new Padding(16, 14, 16, 14);

            var titleLabel = CreateMutedLabel(title);
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Margin = new Padding(0, 0, 0, 6);

            valueLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Noto Sans SemiBold", 18F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Tag = "value"
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);
            return card;
        }

        private Panel CreateSliderRow(
            out MidsTrackBar trackBar,
            out Label valueLabel,
            EventHandler valueChangedHandler,
            bool compact = false)
        {
            var rowLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64F));
            rowLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            trackBar = new MidsTrackBar
            {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = 100,
                SmallChange = 1,
                LargeChange = 10,
                Height = 30,
                Margin = compact ? new Padding(0, 0, 12, 0) : new Padding(0, 0, 12, 10),
                ShowText = false,
                ShowValue = false
            };
            trackBar.ValueChanged += valueChangedHandler;

            valueLabel = CreateChipLabel("100%");
            valueLabel.Margin = compact ? Padding.Empty : new Padding(0, 0, 0, 10);

            rowLayout.Controls.Add(trackBar, 0, 0);
            rowLayout.Controls.Add(valueLabel, 1, 0);
            return rowLayout;
        }

        private Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Noto Sans SemiBold", 10F, FontStyle.Bold),
                Margin = new Padding(0, 4, 12, 10),
                Tag = "text"
            };
        }

        private Label CreateMutedLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Noto Sans", 9F, FontStyle.Regular),
                Tag = "muted"
            };
        }

        private Label CreateChipLabel(string text)
        {
            return new Label
            {
                AutoSize = false,
                Text = text,
                Width = 60,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Noto Sans SemiBold", 10F, FontStyle.Bold),
                Margin = Padding.Empty,
                Tag = "chip"
            };
        }

        private CheckBox CreateTargetStateCheckBox(string text, EventHandler handler, out CheckBox checkBox)
        {
            checkBox = new CheckBox
            {
                AutoSize = true,
                Text = text,
                Margin = new Padding(0, 0, 12, 8),
                Padding = Padding.Empty,
                BackColor = Color.Transparent,
                Tag = "text"
            };
            checkBox.CheckedChanged += handler;
            SetToolTipSafe(checkBox, $"Assume the target is currently {text.ToLowerInvariant()} for planner-only combat math.");
            return checkBox;
        }

        private Panel CreateCardPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            panel.Paint += CardBorderPaint;
            return panel;
        }

        private Panel CreateAutoSizeCardPanel()
        {
            var panel = CreateCardPanel();
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            return panel;
        }

        private void CardBorderPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel)
            {
                return;
            }

            var theme = GetCurrentTheme();
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using var fillBrush = new SolidBrush(theme.Card);
            using var borderPen = new Pen(theme.Border);

            var rect = panel.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;
            e.Graphics.FillRectangle(fillBrush, rect);
            e.Graphics.DrawRectangle(borderPen, rect);
        }

        private MidsVectorButton CreateSectionButton(string text, CombatSection section)
        {
            var button = new MidsVectorButton
            {
                ButtonType = MidsVectorButton.ButtonTypes.Toggle,
                ToggleText =
                {
                    ToggledOff = text,
                    ToggledOn = text
                },
                ToggleState = MidsVectorButton.States.ToggledOff,
                Text = text,
                Width = 134,
                Height = 34,
                Margin = new Padding(0, 0, 0, 8),
                Font = new Font("Noto Sans SemiBold", 10F, FontStyle.Bold),
                CornerRadius = 6
            };

            button.Click += (_, _) => SetSelectedSection(section);
            _sectionButtons[section] = button;
            return button;
        }

        private MidsVectorButton CreateActionButton(string text, EventHandler onClick)
        {
            var button = new MidsVectorButton
            {
                Text = text,
                Width = 118,
                Height = 32,
                Margin = new Padding(8, 0, 0, 0),
                Font = new Font("Noto Sans SemiBold", 9.5F, FontStyle.Bold),
                CornerRadius = 6
            };
            button.Click += onClick;
            return button;
        }

        private MidsVectorButton CreateToggleButton(string text, EventHandler onClick)
        {
            var button = CreateActionButton(text, onClick);
            button.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            button.ToggleText.ToggledOff = text;
            button.ToggleText.ToggledOn = text;
            return button;
        }

        private void SetSelectedSection(CombatSection section)
        {
            if (section == CombatSection.Assassination && !SupportsAssassinationCombatSection())
            {
                section = CombatSection.Context;
            }

            if (section == CombatSection.Opportunity && !SupportsOpportunityCombatSection())
            {
                section = CombatSection.Context;
            }

            if (section == CombatSection.Defiance && !IsBlasterArchetype())
            {
                section = CombatSection.Context;
            }

            if (section == CombatSection.Vigilance && !SupportsVigilanceCombatSection())
            {
                section = CombatSection.Context;
            }

            if (section == CombatSection.CosmicBalance && !SupportsCosmicBalanceCombatSection())
            {
                section = CombatSection.Context;
            }

            if (section == CombatSection.DarkSustenance && !SupportsDarkSustenanceCombatSection())
            {
                section = CombatSection.Context;
            }

            _selectedSection = section;

            foreach (var (currentSection, button) in _sectionButtons)
            {
                button.ToggleState = currentSection == section
                    ? MidsVectorButton.States.ToggledOn
                    : MidsVectorButton.States.ToggledOff;
            }

            foreach (var (currentSection, panel) in _sectionPanels)
            {
                panel.Visible = currentSection == section;
            }

            if (section is CombatSection.Vigilance or CombatSection.CosmicBalance or CombatSection.DarkSustenance)
            {
                UpdateTeamGridLayout();
            }
            else if (section == CombatSection.Assassination)
            {
                UpdateAssassinationControls();
                UpdateTeamGridLayout();
            }
            else if (section == CombatSection.Opportunity)
            {
                UpdateOpportunityControls();
            }
            else if (section == CombatSection.Defiance)
            {
                UpdateDefianceControls();
            }

            UpdateResetButtonTooltip();
        }

        private void UpdateResetButtonTooltip()
        {
            if (_resetSectionButton == null)
            {
                return;
            }

            var sectionName = _selectedSection switch
            {
                CombatSection.Context => "Context",
                CombatSection.Player => "Player",
                CombatSection.Target => "Target",
                CombatSection.Assassination => "Assassination",
                CombatSection.Opportunity => "Opportunity",
                CombatSection.Defiance => "Defiance",
                CombatSection.Vigilance => "Vigilance",
                CombatSection.CosmicBalance => "Cosmic Balance",
                CombatSection.DarkSustenance => "Dark Sustenance",
                _ => "Current"
            };

            SetToolTipSafe(_resetSectionButton, $"Reset the {sectionName} section back to its planner defaults.");
        }

        private void UpdateContextControls()
        {
            if (_selectedRelativeLevelValue == null || _enemyRelativeLevelCombo == null || _combatSupportStatusLabel == null)
            {
                return;
            }

            var selectedRelativeLevel = GetSelectedEnemyRelativeLevel();
            _selectedRelativeLevelValue.Text = FormatSignedValue(selectedRelativeLevel);
            PopulateEnemyRelativeLevelOptions();

            for (var i = 0; i < _enemyRelativeLevelCombo.Items.Count; i++)
            {
                if (_enemyRelativeLevelCombo.Items[i] is RelativeLevelOption option && option.Value == selectedRelativeLevel)
                {
                    _enemyRelativeLevelCombo.SelectedIndex = i;
                    break;
                }
            }

            var (min, max) = ConfigData.GetEnemyRelativeLevelBounds();
            _combatSupportStatusLabel.Text = $"Available range for {DatabaseAPI.DatabaseName}: {FormatSignedValue(min)} to {FormatSignedValue(max)}.";
        }

        private void UpdatePlayerControls()
        {
            if (_playerHpTrackBar == null || _playerEndTrackBar == null || _playerHpValueLabel == null ||
                _playerEndValueLabel == null || _playerAliveButton == null || _playerDefeatedButton == null)
            {
                return;
            }

            var playerSettings = MidsContext.Config.CombatContextSettings.PlayerSettings;
            _playerHpTrackBar.Value = ClampPercent(playerSettings.HpPercent);
            _playerEndTrackBar.Value = ClampPercent(playerSettings.EndPercent);
            _playerHpValueLabel.Text = $"{ClampPercent(playerSettings.HpPercent)}%";
            _playerEndValueLabel.Text = $"{ClampPercent(playerSettings.EndPercent)}%";

            if (playerSettings.HpPercent > 0)
            {
                _previousPlayerHpValue = playerSettings.HpPercent;
            }

            _playerAliveButton.ToggleState = playerSettings.IsAlive
                ? MidsVectorButton.States.ToggledOn
                : MidsVectorButton.States.ToggledOff;
            _playerDefeatedButton.ToggleState = playerSettings.IsAlive
                ? MidsVectorButton.States.ToggledOff
                : MidsVectorButton.States.ToggledOn;
        }

        private void UpdateTargetControls()
        {
            if (_targetHpTrackBar == null || _targetEndTrackBar == null || _targetHpValueLabel == null || _targetEndValueLabel == null)
            {
                return;
            }

            var targetSettings = MidsContext.Config.CombatContextSettings.TargetSettings;
            _targetHpTrackBar.Value = ClampPercent(targetSettings.HpPercent);
            _targetEndTrackBar.Value = ClampPercent(targetSettings.EndPercent);
            _targetHpValueLabel.Text = $"{ClampPercent(targetSettings.HpPercent)}%";
            _targetEndValueLabel.Text = $"{ClampPercent(targetSettings.EndPercent)}%";

            var profile = CombatTargetProfiles.Get(targetSettings.ProfileId);
            if (_targetProfileCombo != null)
            {
                var selected = _targetProfileCombo.Items
                    .OfType<TargetProfileOption>()
                    .FirstOrDefault(option => option.Id == profile.Id);
                if (selected != null && !ReferenceEquals(_targetProfileCombo.SelectedItem, selected))
                {
                    _targetProfileCombo.SelectedItem = selected;
                }
            }

            if (_targetProfileSummaryValue != null)
            {
                _targetProfileSummaryValue.Text = profile.DisplayName;
            }

            if (_targetClassSummaryValue != null)
            {
                _targetClassSummaryValue.Text = profile.ClassName;
            }

            if (_targetHeldCheckBox != null)
            {
                _targetHeldCheckBox.Checked = targetSettings.Held;
            }

            if (_targetImmobilizedCheckBox != null)
            {
                _targetImmobilizedCheckBox.Checked = targetSettings.Immobilized;
            }

            if (_targetStunnedCheckBox != null)
            {
                _targetStunnedCheckBox.Checked = targetSettings.Stunned;
            }

            if (_targetTerrorizedCheckBox != null)
            {
                _targetTerrorizedCheckBox.Checked = targetSettings.Terrorized;
            }

            if (_targetSleptRecentlyCheckBox != null)
            {
                _targetSleptRecentlyCheckBox.Checked = targetSettings.SleptRecently;
            }
        }

        private void UpdateOpportunityControls()
        {
            if (_opportunityMeterTrackBar == null || _opportunityMeterValueLabel == null ||
                _opportunityCurrentMeterValue == null || _opportunityReadyValue == null ||
                MidsContext.Config == null)
            {
                return;
            }

            var meterPercent = OpportunityPlanner.NormalizeMeterPercent(
                MidsContext.Config.CombatContextSettings.Opportunity.MeterPercent);
            MidsContext.Config.CombatContextSettings.Opportunity.MeterPercent = meterPercent;

            if (_opportunityMeterTrackBar.Value != meterPercent)
            {
                _opportunityMeterTrackBar.Value = meterPercent;
            }

            var meterText = $"{meterPercent}%";
            _opportunityMeterValueLabel.Text = meterText;
            _opportunityCurrentMeterValue.Text = meterText;
            _opportunityReadyValue.Text = OpportunityPlanner.IsReady(meterPercent) ? "Yes" : "No";
        }

        private void UpdateAssassinationControls()
        {
            if (_assassinationStacksTrackBar == null || _assassinationStacksValueLabel == null ||
                _assassinationCurrentStacksValue == null || _assassinationCritBonusValue == null ||
                MidsContext.Config == null)
            {
                return;
            }

            var focusStacks = AssassinationPlanner.NormalizeFocusStacks(
                MidsContext.Config.CombatContextSettings.Assassination.FocusStacks);
            MidsContext.Config.CombatContextSettings.Assassination.FocusStacks = focusStacks;

            if (_assassinationStacksTrackBar.Value != focusStacks)
            {
                _assassinationStacksTrackBar.Value = focusStacks;
            }

            var bonusMagnitude = AssassinationPlanner.GetFocusChanceBonusMagnitude(focusStacks);
            _assassinationStacksValueLabel.Text = focusStacks.ToString();
            _assassinationCurrentStacksValue.Text = focusStacks.ToString();
            _assassinationCritBonusValue.Text = $"+{DisplayValueFormatter.FormatPercentFromScale(bonusMagnitude, 1)}%";
        }

        private void UpdateTeamControls()
        {
            if (_teamTotalMembersValue == null || _teamRemainingSlotsValue == null || MidsContext.Config == null)
            {
                return;
            }

            _assassinationPage?.SuspendLayout();
            _vigilancePage?.SuspendLayout();
            _cosmicBalancePage?.SuspendLayout();
            _darkSustenancePage?.SuspendLayout();
            _teamMembersGrid?.SuspendLayout();
            _vigilanceCard?.SuspendLayout();
            _vigilanceGrid?.SuspendLayout();
            try
            {
                EnsureTeamRosterSlots();

                foreach (var option in _teamArchetypeOptions)
                {
                    if (!_teamCountRows.TryGetValue(option.Value, out var row))
                    {
                        continue;
                    }

                    var count = MidsContext.Config.TeamMembers.TryGetValue(option.Value, out var configured)
                        ? configured
                        : 0;
                    if ((int)row.CountUpDown.Value != count)
                    {
                        row.CountUpDown.Value = count;
                    }
                }

                RebuildVigilanceRows();

                var countsByArchetype = CombatContextState.NormalizeTeamMembers(MidsContext.Config.TeamMembers);
                var ordinalByArchetype = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (var index = 0; index < MidsContext.Config.TeamRoster.Count; index++)
                {
                    var slot = MidsContext.Config.TeamRoster[index];
                    if (_vigilanceRows.TryGetValue(index, out var vigilanceRow))
                    {
                        var clampedHp = ClampPercent(slot.HpPercent);
                        var ordinal = ordinalByArchetype.TryGetValue(slot.Archetype, out var currentOrdinal)
                            ? currentOrdinal + 1
                            : 1;
                        ordinalByArchetype[slot.Archetype] = ordinal;
                        var totalForArchetype = countsByArchetype.TryGetValue(slot.Archetype, out var count)
                            ? count
                            : 1;
                        vigilanceRow.NameLabel.Text = BuildVigilanceLabel(slot.Archetype, ordinal, totalForArchetype);
                        if (vigilanceRow.HpTrackBar.Value != clampedHp)
                        {
                            vigilanceRow.HpTrackBar.Value = clampedHp;
                        }

                        vigilanceRow.HpValueLabel.Text = $"{clampedHp}%";
                        if (vigilanceRow.InRangeCheckBox.Checked != slot.InRange)
                        {
                            vigilanceRow.InRangeCheckBox.Checked = slot.InRange;
                        }
                    }
                }

                var totalMembers = countsByArchetype.Values.Sum();

                _teamTotalMembersValue.Text = totalMembers.ToString();
                _teamRemainingSlotsValue.Text = Math.Max(0, MaxMembers - totalMembers).ToString();
            }
            finally
            {
                _vigilanceGrid?.ResumeLayout(true);
                _vigilanceCard?.ResumeLayout(true);
                _teamMembersGrid?.ResumeLayout(true);
                _darkSustenancePage?.ResumeLayout(true);
                _cosmicBalancePage?.ResumeLayout(true);
                _vigilancePage?.ResumeLayout(true);
                _assassinationPage?.ResumeLayout(true);
            }
        }

        private void UpdateDefianceControls()
        {
            if (_defianceTotalBonusValue == null || _defianceActiveSourcesValue == null || _defianceCard == null || _defianceGrid == null)
            {
                return;
            }

            var resolution = DefiancePlanner.Resolve(
                MidsContext.Character?.CurrentBuild,
                MidsContext.Config?.CombatContextSettings.Defiance);

            UpdateDefianceSummary(resolution);
            _defianceCard.Visible = IsBlasterArchetype();
            if (_defianceCard.Visible)
            {
                _defianceGrid.SuspendLayout();
                try
                {
                    RebuildDefianceRows(resolution);
                }
                finally
                {
                    _defianceGrid.ResumeLayout(true);
                }
            }
            else
            {
                _defianceGrid.Controls.Clear();
                _defianceGrid.RowStyles.Clear();
                _defianceGrid.RowCount = 0;
            }
        }

        private void UpdateDefianceSummary(DefianceResolution resolution)
        {
            if (_defianceTotalBonusValue == null || _defianceActiveSourcesValue == null)
            {
                return;
            }

            _defianceTotalBonusValue.Text = $"+{DisplayValueFormatter.FormatPercentFromScale(resolution.TotalMagnitude, 1)}%";
            _defianceActiveSourcesValue.Text = resolution.ActiveContributorCount.ToString();
        }

        private void UpdateSectionVisibility()
        {
            var showAssassination = SupportsAssassinationCombatSection();
            if (_assassinationButton != null)
            {
                _assassinationButton.Visible = showAssassination;
            }

            var showOpportunity = SupportsOpportunityCombatSection();
            if (_opportunityButton != null)
            {
                _opportunityButton.Visible = showOpportunity;
            }

            var showDefiance = IsBlasterArchetype();
            if (_defianceButton != null)
            {
                _defianceButton.Visible = showDefiance;
            }

            var showVigilance = SupportsVigilanceCombatSection();
            if (_vigilanceButton != null)
            {
                _vigilanceButton.Visible = showVigilance;
            }

            var showCosmicBalance = SupportsCosmicBalanceCombatSection();
            if (_cosmicBalanceButton != null)
            {
                _cosmicBalanceButton.Visible = showCosmicBalance;
            }

            var showDarkSustenance = SupportsDarkSustenanceCombatSection();
            if (_darkSustenanceButton != null)
            {
                _darkSustenanceButton.Visible = showDarkSustenance;
            }

            if (_assassinationTeamCountsHost != null)
            {
                _assassinationTeamCountsHost.Visible = showAssassination;
            }

            if (_vigilanceTeamCountsHost != null)
            {
                _vigilanceTeamCountsHost.Visible = showVigilance;
            }

            if (_cosmicBalanceTeamCountsHost != null)
            {
                _cosmicBalanceTeamCountsHost.Visible = showCosmicBalance;
            }

            if (_darkSustenanceTeamCountsHost != null)
            {
                _darkSustenanceTeamCountsHost.Visible = showDarkSustenance;
            }

            UpdateSharedTeamCountsPlacement();

            if (!showAssassination && _selectedSection == CombatSection.Assassination)
            {
                SetSelectedSection(CombatSection.Context);
                return;
            }

            if (!showOpportunity && _selectedSection == CombatSection.Opportunity)
            {
                SetSelectedSection(CombatSection.Context);
                return;
            }

            if (!showDefiance && _selectedSection == CombatSection.Defiance)
            {
                SetSelectedSection(CombatSection.Context);
                return;
            }

            if (!showVigilance && _selectedSection == CombatSection.Vigilance)
            {
                SetSelectedSection(CombatSection.Context);
                return;
            }

            if (!showCosmicBalance && _selectedSection == CombatSection.CosmicBalance)
            {
                SetSelectedSection(CombatSection.Context);
                return;
            }

            if (!showDarkSustenance && _selectedSection == CombatSection.DarkSustenance)
            {
                SetSelectedSection(CombatSection.Context);
                return;
            }
        }

        private void UpdateTopMostButtonState()
        {
            if (_topMostButton == null)
            {
                return;
            }

            _topMostButton.ToggleState = TopMost
                ? MidsVectorButton.States.ToggledOn
                : MidsVectorButton.States.ToggledOff;
        }

        private void ApplyTheme()
        {
            var theme = GetCurrentTheme();

            BackColor = theme.Background;
            ForeColor = theme.Text;

            if (_rootLayout != null)
            {
                _rootLayout.BackColor = theme.Background;
            }

            if (_mainLayout != null)
            {
                _mainLayout.BackColor = theme.Background;
            }

            if (_contentHost != null)
            {
                _contentHost.BackColor = theme.Background;
            }

            ApplyThemeToControlTree(this, theme);
            Invalidate(true);
        }

        private static void ApplyThemeToControlTree(Control root, DataViewTheme theme)
        {
            foreach (Control child in root.Controls)
            {
                switch (child.Tag as string)
                {
                    case "title":
                    case "text":
                        child.ForeColor = theme.Text;
                        break;
                    case "muted":
                        child.ForeColor = theme.Muted;
                        break;
                    case "value":
                        child.ForeColor = theme.ValueText;
                        break;
                    case "chip":
                        child.BackColor = theme.Chip;
                        child.ForeColor = theme.Text;
                        break;
                }

                if (child is NumericUpDown numericUpDown)
                {
                    numericUpDown.BackColor = theme.Background;
                    numericUpDown.ForeColor = theme.Text;
                }

                ApplyThemeToControlTree(child, theme);
            }
        }

        private DataViewTheme GetCurrentTheme()
        {
            return DesignMode
                ? ThemeManager.DesignTime.DataView
                : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;
        }

        private void OnThemeChanged()
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke((Action)ApplyTheme);
                return;
            }

            ApplyTheme();
        }

        private void CharacterOnAlignmentChanged(object? sender, Enums.Alignment e)
        {
            ApplyTheme();
        }

        private void EnemyRelativeLevelComboOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || _enemyRelativeLevelCombo?.SelectedItem is not RelativeLevelOption option)
            {
                return;
            }

            MidsContext.Config.EnemyRelativeLevel = option.Value;
            UpdateContextControls();
            _refreshInfo();
        }

        private void PlayerHpTrackBarOnValueChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || _playerHpTrackBar == null)
            {
                return;
            }

            var value = ClampPercent(_playerHpTrackBar.Value);
            MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent = value;

            if (value > 0)
            {
                _previousPlayerHpValue = value;
                MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive = true;
            }
            else
            {
                MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive = false;
            }

            UpdatePlayerControls();
            BuildUpdate("cfg.player.hp", value);
        }

        private void PlayerEndTrackBarOnValueChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || _playerEndTrackBar == null)
            {
                return;
            }

            var value = ClampPercent(_playerEndTrackBar.Value);
            MidsContext.Config.CombatContextSettings.PlayerSettings.EndPercent = value;
            UpdatePlayerControls();
            BuildUpdate("cfg.player.end", value);
        }

        private void TargetHpTrackBarOnValueChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || _targetHpTrackBar == null)
            {
                return;
            }

            var value = ClampPercent(_targetHpTrackBar.Value);
            MidsContext.Config.CombatContextSettings.TargetSettings.HpPercent = value;
            UpdateTargetControls();
            BuildUpdate("cfg.target.hp", value);
        }

        private void TargetEndTrackBarOnValueChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || _targetEndTrackBar == null)
            {
                return;
            }

            var value = ClampPercent(_targetEndTrackBar.Value);
            MidsContext.Config.CombatContextSettings.TargetSettings.EndPercent = value;
            UpdateTargetControls();
            BuildUpdate("cfg.target.end", value);
        }

        private void TargetProfileComboOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || _targetProfileCombo?.SelectedItem is not TargetProfileOption option)
            {
                return;
            }

            MidsContext.Config.CombatContextSettings.TargetSettings.ProfileId = (int)option.Id;
            UpdateTargetControls();
            BuildUpdate("cfg.target.profileid", (int)option.Id);
        }

        private void TargetHeldCheckBoxOnCheckedChanged(object? sender, EventArgs e)
        {
            UpdateTargetStateToggle("cfg.target.held", _targetHeldCheckBox?.Checked == true, settings => settings.Held = _targetHeldCheckBox?.Checked == true);
        }

        private void TargetImmobilizedCheckBoxOnCheckedChanged(object? sender, EventArgs e)
        {
            UpdateTargetStateToggle("cfg.target.immobilized", _targetImmobilizedCheckBox?.Checked == true, settings => settings.Immobilized = _targetImmobilizedCheckBox?.Checked == true);
        }

        private void TargetStunnedCheckBoxOnCheckedChanged(object? sender, EventArgs e)
        {
            UpdateTargetStateToggle("cfg.target.stunned", _targetStunnedCheckBox?.Checked == true, settings => settings.Stunned = _targetStunnedCheckBox?.Checked == true);
        }

        private void TargetTerrorizedCheckBoxOnCheckedChanged(object? sender, EventArgs e)
        {
            UpdateTargetStateToggle("cfg.target.terrorized", _targetTerrorizedCheckBox?.Checked == true, settings => settings.Terrorized = _targetTerrorizedCheckBox?.Checked == true);
        }

        private void TargetSleptRecentlyCheckBoxOnCheckedChanged(object? sender, EventArgs e)
        {
            UpdateTargetStateToggle("cfg.target.sleptrecently", _targetSleptRecentlyCheckBox?.Checked == true, settings => settings.SleptRecently = _targetSleptRecentlyCheckBox?.Checked == true);
        }

        private void OpportunityMeterTrackBarOnValueChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || _opportunityMeterTrackBar == null)
            {
                return;
            }

            var meterPercent = OpportunityPlanner.NormalizeMeterPercent(_opportunityMeterTrackBar.Value);
            MidsContext.Config.CombatContextSettings.Opportunity.MeterPercent = meterPercent;
            OpportunityPlanner.Synchronize(MidsContext.Character?.CurrentBuild, MidsContext.Config.CombatContextSettings.Opportunity);
            UpdateOpportunityControls();
            _refreshInfo();
        }

        private void AssassinationStacksTrackBarOnValueChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || _assassinationStacksTrackBar == null)
            {
                return;
            }

            var focusStacks = AssassinationPlanner.NormalizeFocusStacks(_assassinationStacksTrackBar.Value);
            MidsContext.Config.CombatContextSettings.Assassination.FocusStacks = focusStacks;
            AssassinationPlanner.Synchronize(MidsContext.Character?.CurrentBuild, MidsContext.Config.CombatContextSettings.Assassination);
            UpdateAssassinationControls();
            _refreshInfo();
        }

        private void PlayerAliveButtonOnClick(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null)
            {
                return;
            }

            MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive = true;
            MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent = _previousPlayerHpValue.GetValueOrDefault(100);
            UpdatePlayerControls();
            BuildUpdate("cfg.player.hp", MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent);
        }

        private void PlayerDefeatedButtonOnClick(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null)
            {
                return;
            }

            if (MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent > 0)
            {
                _previousPlayerHpValue = MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent;
            }

            MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive = false;
            MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent = 0;
            UpdatePlayerControls();
            BuildUpdate("cfg.player.hp", 0);
        }

        private void TeamCountUpDownOnValueChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || sender is not NumericUpDown countUpDown || countUpDown.Tag is not string archetype)
            {
                return;
            }

            var count = (int)countUpDown.Value;
            if (count <= 0)
            {
                MidsContext.Config.TeamMembers.Remove(archetype);
            }
            else
            {
                MidsContext.Config.TeamMembers[archetype] = count;
            }

            MidsContext.Config.SynchronizeTeamRosterFromTeamMembers();
            UpdateTeamControls();
            _refreshInfo();
        }

        private void VigilanceHpTrackBarOnValueChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || sender is not MidsTrackBar trackBar || trackBar.Tag is not int slotIndex)
            {
                return;
            }

            EnsureTeamRosterSlots();
            if (slotIndex < 0 || slotIndex >= MidsContext.Config.TeamRoster.Count)
            {
                return;
            }

            MidsContext.Config.TeamRoster[slotIndex].HpPercent = ClampPercent(trackBar.Value);
            UpdateTeamControls();
            _refreshInfo();
        }

        private void VigilanceInRangeCheckBoxOnCheckedChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents || MidsContext.Config == null || sender is not CheckBox checkBox || checkBox.Tag is not int slotIndex)
            {
                return;
            }

            EnsureTeamRosterSlots();
            if (slotIndex < 0 || slotIndex >= MidsContext.Config.TeamRoster.Count)
            {
                return;
            }

            MidsContext.Config.TeamRoster[slotIndex].InRange = checkBox.Checked;
            UpdateTeamControls();
            _refreshInfo();
        }

        private void DefianceCountComboOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_suppressUiEvents ||
                MidsContext.Config == null ||
                sender is not MidsDropDownList combo ||
                combo.Tag is not DefianceContributorDescriptor descriptor ||
                combo.SelectedItem is not DefianceCountOption option)
            {
                return;
            }

            DefiancePlanner.SetActiveCount(
                MidsContext.Config.CombatContextSettings.Defiance,
                descriptor,
                option.Value);

            var resolution = DefiancePlanner.Resolve(
                MidsContext.Character?.CurrentBuild,
                MidsContext.Config.CombatContextSettings.Defiance);
            UpdateDefianceSummary(resolution);
            _refreshInfo();
        }

        private void UpdateTargetStateToggle(string settingName, bool enabled, Action<ConfigData.CombatContext.Target> updateAction)
        {
            if (_suppressUiEvents || MidsContext.Config == null)
            {
                return;
            }

            updateAction(MidsContext.Config.CombatContextSettings.TargetSettings);
            UpdateTargetControls();
            BuildUpdate(settingName, enabled ? 1 : 0);
        }

        private void TopMostButtonOnClick(object? sender, EventArgs e)
        {
            TopMost = !TopMost;
            UpdateTopMostButtonState();
        }

        private void ResetSectionButtonOnClick(object? sender, EventArgs e)
        {
            switch (_selectedSection)
            {
                case CombatSection.Context:
                    ResetContext();
                    break;
                case CombatSection.Player:
                    ResetPlayer();
                    break;
                case CombatSection.Target:
                    ResetTarget();
                    break;
                case CombatSection.Assassination:
                    ResetAssassination();
                    ResetTeam();
                    return;
                case CombatSection.Opportunity:
                    ResetOpportunity();
                    break;
                case CombatSection.Defiance:
                    ResetDefiance();
                    break;
                case CombatSection.Vigilance:
                case CombatSection.CosmicBalance:
                case CombatSection.DarkSustenance:
                    ResetTeam();
                    break;
            }
        }

        private void ResetAllButtonOnClick(object? sender, EventArgs e)
        {
            ResetContext();
            ResetPlayer();
            ResetTarget();
            ResetAssassination();
            ResetOpportunity();
            ResetDefiance();
            ResetTeam();
            RefreshFromConfig();
            _refreshInfo();
        }

        private void CloseButtonOnClick(object? sender, EventArgs e)
        {
            SuppressToolTips();
            Hide();
        }

        private void CenterToOwner()
        {
            if (Owner == null)
            {
                return;
            }

            var ownerBounds = Owner.Bounds;
            var x = ownerBounds.Left + Math.Max(0, (ownerBounds.Width - Width) / 2);
            var y = ownerBounds.Top + Math.Max(0, (ownerBounds.Height - Height) / 2);
            var workingArea = Screen.FromControl(Owner).WorkingArea;

            x = Math.Max(workingArea.Left, Math.Min(x, workingArea.Right - Width));
            y = Math.Max(workingArea.Top, Math.Min(y, workingArea.Bottom - Height));

            StartPosition = FormStartPosition.Manual;
            Location = new Point(x, y);
        }

        private void SuppressToolTips()
        {
            if (_toolTip == null)
            {
                return;
            }

            try
            {
                _toolTip.Active = false;
                if (IsHandleCreated)
                {
                    _toolTip.Hide(this);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        private void ResumeToolTips()
        {
            if (_isShuttingDown || IsDisposed || Disposing)
            {
                return;
            }

            try
            {
                _toolTip.Active = true;
            }
            catch (ObjectDisposedException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        private void SetToolTipSafe(Control? control, string text)
        {
            if (_isShuttingDown || control == null || control.IsDisposed || IsDisposed || Disposing)
            {
                return;
            }

            try
            {
                _toolTip.SetToolTip(control, text);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        private void ResetContext()
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            MidsContext.Config.EnemyRelativeLevel = 0;
            RefreshFromConfig();
            _refreshInfo();
        }

        private void ResetPlayer()
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent = 100;
            MidsContext.Config.CombatContextSettings.PlayerSettings.EndPercent = 100;
            MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive = true;
            _previousPlayerHpValue = 100;
            RefreshFromConfig();
            BuildUpdate("cfg.player.hp", 100);
            BuildUpdate("cfg.player.end", 100);
        }

        private void ResetTarget()
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            MidsContext.Config.CombatContextSettings.TargetSettings.HpPercent = 100;
            MidsContext.Config.CombatContextSettings.TargetSettings.EndPercent = 100;
            MidsContext.Config.CombatContextSettings.TargetSettings.ProfileId = (int)CombatTargetProfileId.Boss;
            MidsContext.Config.CombatContextSettings.TargetSettings.Held = false;
            MidsContext.Config.CombatContextSettings.TargetSettings.Immobilized = false;
            MidsContext.Config.CombatContextSettings.TargetSettings.Stunned = false;
            MidsContext.Config.CombatContextSettings.TargetSettings.Terrorized = false;
            MidsContext.Config.CombatContextSettings.TargetSettings.SleptRecently = false;
            MidsContext.Config.CombatContextSettings.TargetSettings.VulnerabilityActive = false;
            MidsContext.Config.CombatContextSettings.TargetSettings.OpportunityState = (int)CombatTargetOpportunityState.None;
            RefreshFromConfig();
            BuildUpdate("cfg.target.hp", 100);
            BuildUpdate("cfg.target.end", 100);
            BuildUpdate("cfg.target.profileid", (int)CombatTargetProfileId.Boss);
            BuildUpdate("cfg.target.held", 0);
            BuildUpdate("cfg.target.immobilized", 0);
            BuildUpdate("cfg.target.stunned", 0);
            BuildUpdate("cfg.target.terrorized", 0);
            BuildUpdate("cfg.target.sleptrecently", 0);
            BuildUpdate("cfg.target.vulnerabilityactive", 0);
        }

        private void ResetTeam()
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            MidsContext.Config.TeamMembers.Clear();
            MidsContext.Config.TeamRoster.Clear();
            MidsContext.Config.SynchronizeTeamRosterFromTeamMembers();
            RefreshFromConfig();
            _refreshInfo();
        }

        private void ResetDefiance()
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            MidsContext.Config.CombatContextSettings.Defiance.Contributors.Clear();
            RefreshFromConfig();
            _refreshInfo();
        }

        private void ResetAssassination()
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            MidsContext.Config.CombatContextSettings.Assassination.FocusStacks = 0;
            AssassinationPlanner.Synchronize(MidsContext.Character?.CurrentBuild, MidsContext.Config.CombatContextSettings.Assassination);
            RefreshFromConfig();
            _refreshInfo();
        }

        private void ResetOpportunity()
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            MidsContext.Config.CombatContextSettings.Opportunity.MeterPercent = 0;
            OpportunityPlanner.Synchronize(MidsContext.Character?.CurrentBuild, MidsContext.Config.CombatContextSettings.Opportunity);
            RefreshFromConfig();
            _refreshInfo();
        }

        private void BuildUpdate(string settingName, int value)
        {
            if (MidsContext.Character?.CurrentBuild?.Powers == null)
            {
                _refreshInfo();
                return;
            }

            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                var power = powerEntry?.Power;
                if (power == null)
                {
                    continue;
                }

                var formattedDesc = power.DescLong?.Replace("  ", " ").Trim();
                if (string.IsNullOrWhiteSpace(formattedDesc))
                {
                    continue;
                }

                var matches = Regex.Matches(formattedDesc, @"\{link\:([a-zA-Z0-9\.\-_]+)\}");
                var linkedSettings = matches
                    .Select(match => match.Groups[1].Value)
                    .Where(link => string.Equals(link, settingName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (linkedSettings.Count == 0 || !power.VariableEnabled)
                {
                    continue;
                }

                powerEntry.VariableValue = value;
            }

            _refreshInfo();
        }

        private List<TeammateArchetypeOption> GetTeamMemberDefinitions()
        {
            return CombatContextState.GetAvailableTeammateArchetypes()
                .Select(definition => new TeammateArchetypeOption(definition.Key, definition.DisplayName))
                .ToList();
        }

        private static int ClampPercent(int value)
        {
            return Math.Max(0, Math.Min(100, value));
        }

        private int GetSelectedEnemyRelativeLevel()
        {
            return ConfigData.NormalizeEnemyRelativeLevel(
                MidsContext.Config.EnemyRelativeLevel,
                MidsContext.Config.ScalingToHit);
        }

        private void PopulateTargetProfileOptions()
        {
            if (_targetProfileCombo == null)
            {
                return;
            }

            _targetProfileCombo.Items.Clear();
            foreach (var profile in CombatTargetProfiles.GetAll())
            {
                _targetProfileCombo.Items.Add(new TargetProfileOption(profile.Id, profile.DisplayName));
            }
        }

        private void PopulateEnemyRelativeLevelOptions()
        {
            if (_enemyRelativeLevelCombo == null)
            {
                return;
            }

            var options = MidsContext.Config.RelativeLevels
                .Select(x => x.Value)
                .ToArray();

            if (_enemyRelativeLevelCombo.Items.Count == options.Length)
            {
                var matches = true;
                for (var i = 0; i < options.Length; i++)
                {
                    if (_enemyRelativeLevelCombo.Items[i] is not RelativeLevelOption option || option.Value != options[i])
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    return;
                }
            }

            _enemyRelativeLevelCombo.Items.Clear();
            foreach (var value in options)
            {
                _enemyRelativeLevelCombo.Items.Add(new RelativeLevelOption(value));
            }
        }

        private void EnsureTeamRosterSlots()
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            MidsContext.Config.SynchronizeTeamRosterFromTeamMembers();
        }

        private bool IsDefenderArchetype()
        {
            var archetype = MidsContext.Character?.Archetype ?? MidsContext.Archetype;
            if (archetype == null)
            {
                return false;
            }

            return archetype.DisplayName.Equals("Defender", StringComparison.OrdinalIgnoreCase) ||
                   archetype.ClassName.Equals("Class_Defender", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsBlasterArchetype()
        {
            var archetype = MidsContext.Character?.Archetype ?? MidsContext.Archetype;
            if (archetype == null)
            {
                return false;
            }

            return archetype.DisplayName.Equals("Blaster", StringComparison.OrdinalIgnoreCase) ||
                   archetype.ClassName.Equals("Class_Blaster", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsSentinelArchetype()
        {
            var archetype = MidsContext.Character?.Archetype ?? MidsContext.Archetype;
            if (archetype == null)
            {
                return false;
            }

            return archetype.DisplayName.Equals("Sentinel", StringComparison.OrdinalIgnoreCase) ||
                   archetype.ClassName.Equals("Class_Sentinel", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsStalkerArchetype()
        {
            var archetype = MidsContext.Character?.Archetype ?? MidsContext.Archetype;
            if (archetype == null)
            {
                return false;
            }

            return archetype.DisplayName.Equals("Stalker", StringComparison.OrdinalIgnoreCase) ||
                   archetype.ClassName.Equals("Class_Stalker", StringComparison.OrdinalIgnoreCase);
        }

        private bool SupportsOpportunityCombatSection()
        {
            return DatabaseAPI.GetDataProviderId() == OmniDataProviderId.OmniHomecoming &&
                   IsSentinelArchetype();
        }

        private bool SupportsAssassinationCombatSection()
        {
            return IsStalkerArchetype();
        }

        private bool SupportsVigilanceCombatSection()
        {
            return IsDefenderArchetype();
        }

        private bool SupportsCosmicBalanceCombatSection()
        {
            var archetype = MidsContext.Character?.Archetype ?? MidsContext.Archetype;
            if (archetype == null)
            {
                return false;
            }

            return archetype.DisplayName.Equals("Peacebringer", StringComparison.OrdinalIgnoreCase) ||
                   archetype.ClassName.Equals("Class_Peacebringer", StringComparison.OrdinalIgnoreCase) ||
                   archetype.ClassName.Equals("Class_Kheldian", StringComparison.OrdinalIgnoreCase);
        }

        private bool SupportsDarkSustenanceCombatSection()
        {
            var archetype = MidsContext.Character?.Archetype ?? MidsContext.Archetype;
            if (archetype == null)
            {
                return false;
            }

            return archetype.DisplayName.Equals("Warshade", StringComparison.OrdinalIgnoreCase) ||
                   archetype.ClassName.Equals("Class_Warshade", StringComparison.OrdinalIgnoreCase) ||
                   archetype.ClassName.Equals("Class_Shade", StringComparison.OrdinalIgnoreCase);
        }

        private string GetTeammateDisplayName(string? archetype)
        {
            if (string.IsNullOrWhiteSpace(archetype))
            {
                return string.Empty;
            }

            return _teamArchetypeOptions
                .FirstOrDefault(option => option.Value.Equals(archetype, StringComparison.OrdinalIgnoreCase))
                ?.DisplayName ?? archetype;
        }

        private string BuildVigilanceLabel(string? archetype, int ordinal, int totalForArchetype)
        {
            var displayName = GetTeammateDisplayName(archetype);
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return $"Teammate {ordinal}";
            }

            return totalForArchetype > 1
                ? $"{displayName} {ordinal}"
                : displayName;
        }

        private static string FormatSignedValue(int value)
        {
            return value > 0 ? $"+{value}" : value.ToString();
        }
    }
}

using System.Text.RegularExpressions;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
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
            Team
        }

        private sealed record RelativeLevelOption(int Value)
        {
            public override string ToString() => Value == 0 ? "Default (0)" : $"{(Value > 0 ? "+" : string.Empty)}{Value}";
        }

        private sealed record TeamMemberDefinition(string Key, string DisplayName);

        private sealed class TeamRowControls
        {
            public required Label ValueLabel { get; init; }
        }

        private const int MaxMembers = 7;

        private readonly Action _refreshInfo;
        private readonly Dictionary<CombatSection, MidsVectorButton> _sectionButtons = new();
        private readonly Dictionary<CombatSection, Panel> _sectionPanels = new();
        private readonly Dictionary<string, TeamRowControls> _teamRows = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<TeamMemberDefinition> _teamDefinitions = [];
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
        private MidsVectorButton? _teamButton;

        private Panel? _contextPage;
        private Panel? _playerPage;
        private Panel? _targetPage;
        private Panel? _teamPage;

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

        private Label? _teamTotalMembersValue;
        private Label? _teamRemainingSlotsValue;
        private TableLayoutPanel? _teamMembersGrid;
        private int _teamGridColumnCount = 2;

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
                UpdateContextControls();
                UpdatePlayerControls();
                UpdateTargetControls();
                UpdateTeamControls();
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
            _teamButton = CreateSectionButton("Team", CombatSection.Team);

            _navigationRail.Controls.AddRange([_contextButton, _playerButton, _targetButton, _teamButton]);
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
            _teamPage = CreateTeamPage();
            _teamPage.Resize += (_, _) => UpdateTeamGridLayout();

            _sectionPanels[CombatSection.Context] = _contextPage;
            _sectionPanels[CombatSection.Player] = _playerPage;
            _sectionPanels[CombatSection.Target] = _targetPage;
            _sectionPanels[CombatSection.Team] = _teamPage;

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
            page.Controls.Add(layout);

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
            page.Controls.Add(layout);

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
            page.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Target State",
                "Target HP and endurance stay live too, which keeps combat-setting conditionals honest while you plan."));

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

            return page;
        }

        private Panel CreateTeamPage()
        {
            var page = CreatePageHost();
            var layout = CreatePageLayout();
            page.Controls.Add(layout);

            layout.Controls.Add(CreatePageHeader(
                "Team Context",
                "Adjust teammate counts here. The planner uses these counts for conditional effects and team-size-sensitive combat math."));

            var summaryLayout = CreateSummaryLayout(2);
            summaryLayout.Controls.Add(CreateSummaryCard("Total Members", out _teamTotalMembersValue), 0, 0);
            summaryLayout.Controls.Add(CreateSummaryCard("Remaining Slots", out _teamRemainingSlotsValue), 1, 0);
            layout.Controls.Add(summaryLayout);

            var teamCard = CreateAutoSizeCardPanel();
            teamCard.Dock = DockStyle.Top;
            teamCard.Padding = new Padding(18);
            teamCard.Margin = new Padding(0, 0, 0, 14);

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

            teamCard.Controls.Add(_teamMembersGrid);
            layout.Controls.Add(teamCard);

            return page;
        }

        private void BuildTeamRows()
        {
            if (_teamMembersGrid == null)
            {
                return;
            }

            _teamDefinitions.Clear();
            _teamDefinitions.AddRange(GetTeamMemberDefinitions());
            var availableWidth = _teamPage?.ClientSize.Width > 0 ? _teamPage.ClientSize.Width : ClientSize.Width;
            _teamGridColumnCount = availableWidth >= 720 ? 2 : 1;

            _teamRows.Clear();
            _teamMembersGrid.Controls.Clear();
            _teamMembersGrid.ColumnStyles.Clear();
            _teamMembersGrid.RowStyles.Clear();

            _teamMembersGrid.ColumnCount = _teamGridColumnCount;
            for (var column = 0; column < _teamGridColumnCount; column++)
            {
                _teamMembersGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / _teamGridColumnCount));
            }

            var rowCount = (int)Math.Ceiling(_teamDefinitions.Count / (float)_teamGridColumnCount);
            _teamMembersGrid.RowCount = rowCount;

            for (var row = 0; row < rowCount; row++)
            {
                _teamMembersGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            for (var index = 0; index < _teamDefinitions.Count; index++)
            {
                var row = index / _teamGridColumnCount;
                var column = index % _teamGridColumnCount;
                var definition = _teamDefinitions[index];
                var rowPanel = CreateTeamRow(definition);
                _teamMembersGrid.Controls.Add(rowPanel, column, row);
            }
        }

        private Panel CreateTeamRow(TeamMemberDefinition definition)
        {
            var rowPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                Margin = new Padding(0, 0, 10, 8),
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };

            rowPanel.Paint += CardBorderPaint;

            var rowLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                Margin = Padding.Empty,
                Padding = new Padding(10, 5, 10, 5),
                BackColor = Color.Transparent
            };
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 34F));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46F));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 34F));

            var nameLabel = CreateFieldLabel(definition.DisplayName);
            nameLabel.Dock = DockStyle.Fill;
            nameLabel.TextAlign = ContentAlignment.MiddleLeft;

            var decrementButton = CreateTinyTeamButton("-");
            decrementButton.Tag = definition.Key;
            decrementButton.Click += TeamDecrementButtonOnClick;

            var incrementButton = CreateTinyTeamButton("+");
            incrementButton.Tag = definition.Key;
            incrementButton.Click += TeamIncrementButtonOnClick;

            var valueLabel = CreateChipLabel("0");
            valueLabel.Dock = DockStyle.Fill;

            rowLayout.Controls.Add(nameLabel, 0, 0);
            rowLayout.Controls.Add(decrementButton, 1, 0);
            rowLayout.Controls.Add(valueLabel, 2, 0);
            rowLayout.Controls.Add(incrementButton, 3, 0);

            rowPanel.Controls.Add(rowLayout);

            _teamRows[definition.Key] = new TeamRowControls
            {
                ValueLabel = valueLabel
            };

            return rowPanel;
        }

        private void UpdateTeamGridLayout()
        {
            if (_teamMembersGrid == null)
            {
                return;
            }

            var availableWidth = _teamPage?.ClientSize.Width > 0 ? _teamPage.ClientSize.Width : ClientSize.Width;
            var desiredColumnCount = availableWidth >= 720 ? 2 : 1;
            if (desiredColumnCount == _teamGridColumnCount && _teamRows.Count > 0)
            {
                return;
            }

            BuildTeamRows();
            UpdateTeamControls();
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

        private static Panel CreatePageHost()
        {
            return new Panel
            {
                AutoScroll = true,
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

        private Panel CreateSliderRow(out MidsTrackBar trackBar, out Label valueLabel, EventHandler valueChangedHandler)
        {
            var rowLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64F));

            trackBar = new MidsTrackBar
            {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = 100,
                SmallChange = 1,
                LargeChange = 10,
                Height = 30,
                Margin = new Padding(0, 0, 12, 10),
                ShowText = false,
                ShowValue = false
            };
            trackBar.ValueChanged += valueChangedHandler;

            valueLabel = CreateChipLabel("100%");
            valueLabel.Margin = new Padding(0, 0, 0, 10);

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

        private static MidsVectorButton CreateTinyTeamButton(string text)
        {
            return new MidsVectorButton
            {
                Text = text,
                Width = 28,
                Height = 24,
                Margin = Padding.Empty,
                Font = new Font("Noto Sans SemiBold", 10F, FontStyle.Bold),
                CornerRadius = 6
            };
        }

        private void SetSelectedSection(CombatSection section)
        {
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

            if (section == CombatSection.Team)
            {
                UpdateTeamGridLayout();
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
                CombatSection.Team => "Team",
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
        }

        private void UpdateTeamControls()
        {
            if (_teamTotalMembersValue == null || _teamRemainingSlotsValue == null)
            {
                return;
            }

            foreach (var (key, row) in _teamRows)
            {
                row.ValueLabel.Text = GetTeamMemberCount(key).ToString();
            }

            var totalMembers = MidsContext.Config.TeamMembers.Values.Sum();
            MidsContext.Config.TeamSize = Math.Max(1, totalMembers + 1);

            _teamTotalMembersValue.Text = totalMembers.ToString();
            _teamRemainingSlotsValue.Text = Math.Max(0, MaxMembers - totalMembers).ToString();
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

        private void TeamIncrementButtonOnClick(object? sender, EventArgs e)
        {
            if (MidsContext.Config == null || sender is not MidsVectorButton button || button.Tag is not string key)
            {
                return;
            }

            var totalMembers = MidsContext.Config.TeamMembers.Values.Sum();
            if (totalMembers >= MaxMembers)
            {
                return;
            }

            SetTeamMemberCount(key, GetTeamMemberCount(key) + 1);
            UpdateTeamControls();
            _refreshInfo();
        }

        private void TeamDecrementButtonOnClick(object? sender, EventArgs e)
        {
            if (MidsContext.Config == null || sender is not MidsVectorButton button || button.Tag is not string key)
            {
                return;
            }

            var current = GetTeamMemberCount(key);
            if (current <= 0)
            {
                return;
            }

            SetTeamMemberCount(key, current - 1);
            UpdateTeamControls();
            _refreshInfo();
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
                case CombatSection.Team:
                    ResetTeam();
                    break;
            }
        }

        private void ResetAllButtonOnClick(object? sender, EventArgs e)
        {
            ResetContext();
            ResetPlayer();
            ResetTarget();
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
            RefreshFromConfig();
            BuildUpdate("cfg.target.hp", 100);
            BuildUpdate("cfg.target.end", 100);
        }

        private void ResetTeam()
        {
            if (MidsContext.Config == null)
            {
                return;
            }

            MidsContext.Config.TeamMembers.Clear();
            MidsContext.Config.TeamSize = 1;
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
                power.Stacks = value;
            }

            _refreshInfo();
        }

        private List<TeamMemberDefinition> GetTeamMemberDefinitions()
        {
            var definitions = new List<TeamMemberDefinition>
            {
                new("Any", "Any"),
                new("Blaster", "Blaster"),
                new("Controller", "Controller"),
                new("Defender", "Defender"),
                new("Scrapper", "Scrapper"),
                new("Tanker", "Tanker"),
                new("Peacebringer", "Peacebringer"),
                new("Warshade", "Warshade")
            };

            switch (DatabaseAPI.DatabaseName)
            {
                case "Homecoming":
                    definitions.Add(new TeamMemberDefinition("Sentinel", "Sentinel"));
                    break;
                case "Rebirth":
                    definitions.Add(new TeamMemberDefinition("Guardian", "Guardian"));
                    break;
            }

            definitions.AddRange(
            [
                new TeamMemberDefinition("Brute", "Brute"),
                new TeamMemberDefinition("Stalker", "Stalker"),
                new TeamMemberDefinition("Mastermind", "Mastermind"),
                new TeamMemberDefinition("Dominator", "Dominator"),
                new TeamMemberDefinition("Corruptor", "Corruptor"),
                new TeamMemberDefinition("Arachnos Soldier", "Arachnos Soldier"),
                new TeamMemberDefinition("Arachnos Widow", "Arachnos Widow")
            ]);

            return definitions;
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

        private int GetTeamMemberCount(string key)
        {
            return MidsContext.Config.TeamMembers.TryGetValue(key, out var value)
                ? value
                : 0;
        }

        private void SetTeamMemberCount(string key, int value)
        {
            if (value <= 0)
            {
                MidsContext.Config.TeamMembers.Remove(key);
                return;
            }

            MidsContext.Config.TeamMembers[key] = value;
        }

        private static string FormatSignedValue(int value)
        {
            return value > 0 ? $"+{value}" : value.ToString();
        }
    }
}

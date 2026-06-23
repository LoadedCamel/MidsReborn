using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Test;
using MRBResourceLib;

namespace Mids_Reborn.UI.Forms.WindowMenuItems;

public sealed partial class FrmPetActorDetails
{
    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _headerPanel = new Panel();
        _headerLayout = new TableLayoutPanel();
        _headerIcon = new PetActorIconView();
        _nameLabel = new Label();
        _countLabel = new Label();
        _subTitleLabel = new Label();
        _classLabel = new Label();
        _tagsLabel = new Label();
        _summaryMetrics = new TableLayoutPanel();
        _previewStatePanel = new TableLayoutPanel();
        _upgradeFlow = new FlowLayoutPanel();
        _proximityFlow = new FlowLayoutPanel();
        _ribbonHost = new TableLayoutPanel();
        _actorRibbon = new PetActorRibbon();
        _contentHost = new Panel();
        _emptyStatePanel = new Panel();
        _emptyLabel = new Label();
        _bodySplit = new SplitContainer();
        _detailView = new PetActorDetailView();
        _powerPanel = new TableLayoutPanel();
        _powerGridScrollPanel = new MidsVScrollPanel();
        _powerGrid = new PetActorPowerGrid();
        _detailTotalsSplit = new SplitContainer();
        _totalsHost = new TableLayoutPanel();
        _totalsScrollPanel = new Panel();
        _totalsStack = new TableLayoutPanel();
        _coreLabel = new Label();
        _combatLabel = new Label();
        _movementLabel = new Label();
        _defenseLabel = new Label();
        _statusLabel = new Label();
        _coreGrid = new PowerStatsGrid();
        _combatGrid = new PowerStatsGrid();
        _movementGrid = new PowerStatsGrid();
        _defenseGrid = new PowerStatsGrid();
        _statusGrid = new PowerStatsGrid();
        var ribbonSectionLabel = new Label();
        var powerSectionLabel = new Label();
        var totalsSectionLabel = new Label();
        var metricHpLabel = new Label();
        var metricRegenLabel = new Label();
        var metricRecoveryLabel = new Label();
        var metricDefenseLabel = new Label();
        var metricResistanceLabel = new Label();
        var upgradesPreviewLabel = new Label();
        var contextPreviewLabel = new Label();

        SuspendLayout();
        _rootLayout.SuspendLayout();
        _headerPanel.SuspendLayout();
        _headerLayout.SuspendLayout();
        _previewStatePanel.SuspendLayout();
        _ribbonHost.SuspendLayout();
        _contentHost.SuspendLayout();
        _emptyStatePanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_bodySplit).BeginInit();
        _bodySplit.Panel1.SuspendLayout();
        _bodySplit.Panel2.SuspendLayout();
        _bodySplit.SuspendLayout();
        _powerPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_detailTotalsSplit).BeginInit();
        _detailTotalsSplit.Panel2.SuspendLayout();
        _detailTotalsSplit.SuspendLayout();
        _totalsHost.SuspendLayout();
        _totalsStack.SuspendLayout();

        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.Black;
        ForeColor = Color.WhiteSmoke;
        FormBorderStyle = FormBorderStyle.Sizable;
        Icon = Resources.MRB_Icon_Concept;
        Margin = Padding.Empty;
        MaximizeBox = true;
        MinimizeBox = true;
        MinimumSize = new Size(1080, 700);
        Name = "FrmPetActorDetails";
        ShowIcon = false;
        Size = new Size(1280, 800);
        StartPosition = FormStartPosition.Manual;
        Text = "Pet Actor Sheet";
        Load += OnLoad;
        SizeChanged += OnFormSizeChanged;
        FormClosed += OnFormClosed;

        _rootLayout.BackColor = Color.Black;
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.Margin = Padding.Empty;
        _rootLayout.Padding = new Padding(8);
        _rootLayout.RowCount = 5;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 84F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 82F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 86F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _headerPanel.Dock = DockStyle.Fill;
        _headerPanel.Margin = new Padding(0, 0, 0, 8);
        _headerPanel.Padding = new Padding(16, 12, 16, 12);
        _headerPanel.Paint += HeaderPanelOnPaint;

        _headerLayout.BackColor = Color.Transparent;
        _headerLayout.ColumnCount = 3;
        _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136F));
        _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320F));
        _headerLayout.Dock = DockStyle.Fill;
        _headerLayout.Margin = Padding.Empty;
        _headerLayout.RowCount = 3;
        _headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        _headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        _headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _headerIcon.Dock = DockStyle.Fill;
        _headerIcon.FallbackText = "Alpha Howler Wolf";
        _headerIcon.Margin = new Padding(0, 0, 14, 0);

        _nameLabel.Dock = DockStyle.Fill;
        _nameLabel.Font = new Font("Segoe UI", 19F, FontStyle.Bold, GraphicsUnit.Point);
        _nameLabel.Text = "Alpha Howler Wolf";
        _nameLabel.TextAlign = ContentAlignment.MiddleLeft;

        _countLabel.Dock = DockStyle.Fill;
        _countLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        _countLabel.Text = "1 Active";
        _countLabel.TextAlign = ContentAlignment.MiddleRight;

        _subTitleLabel.Dock = DockStyle.Fill;
        _subTitleLabel.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
        _subTitleLabel.Text = "Summoned from Summon Wolves";
        _subTitleLabel.TextAlign = ContentAlignment.MiddleLeft;

        _classLabel.Dock = DockStyle.Fill;
        _classLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        _classLabel.Text = "Actor Class: Class Henchman Minion";
        _classLabel.TextAlign = ContentAlignment.MiddleRight;

        _tagsLabel.Dock = DockStyle.Fill;
        _tagsLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
        _tagsLabel.Text = "Actor Tags: Beast | Detonator | FullHenchman | SetBonusShare";
        _tagsLabel.TextAlign = ContentAlignment.MiddleLeft;

        _headerLayout.Controls.Add(_headerIcon, 0, 0);
        _headerLayout.Controls.Add(_nameLabel, 1, 0);
        _headerLayout.Controls.Add(_countLabel, 2, 0);
        _headerLayout.Controls.Add(_subTitleLabel, 1, 1);
        _headerLayout.Controls.Add(_classLabel, 2, 1);
        _headerLayout.Controls.Add(_tagsLabel, 1, 2);
        _headerLayout.SetRowSpan(_headerIcon, 3);
        _headerLayout.SetColumnSpan(_tagsLabel, 2);
        _headerPanel.Controls.Add(_headerLayout);

        _summaryMetrics.BackColor = Color.Black;
        _summaryMetrics.ColumnCount = 5;
        _summaryMetrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        _summaryMetrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        _summaryMetrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        _summaryMetrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        _summaryMetrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        _summaryMetrics.Dock = DockStyle.Fill;
        _summaryMetrics.Margin = new Padding(0, 0, 0, 8);
        _summaryMetrics.RowCount = 1;
        _summaryMetrics.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        metricHpLabel.Dock = DockStyle.Fill;
        metricHpLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        metricHpLabel.Padding = new Padding(10, 6, 10, 6);
        metricHpLabel.Text = "Hit Points\r\n578";
        metricHpLabel.TextAlign = ContentAlignment.MiddleLeft;
        _summaryMetrics.Controls.Add(metricHpLabel, 0, 0);

        metricRegenLabel.Dock = DockStyle.Fill;
        metricRegenLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        metricRegenLabel.Padding = new Padding(10, 6, 10, 6);
        metricRegenLabel.Text = "Regen\r\n4.82 HP/s";
        metricRegenLabel.TextAlign = ContentAlignment.MiddleLeft;
        _summaryMetrics.Controls.Add(metricRegenLabel, 1, 0);

        metricRecoveryLabel.Dock = DockStyle.Fill;
        metricRecoveryLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        metricRecoveryLabel.Padding = new Padding(10, 6, 10, 6);
        metricRecoveryLabel.Text = "Recovery\r\n1.67 End/s";
        metricRecoveryLabel.TextAlign = ContentAlignment.MiddleLeft;
        _summaryMetrics.Controls.Add(metricRecoveryLabel, 2, 0);

        metricDefenseLabel.Dock = DockStyle.Fill;
        metricDefenseLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        metricDefenseLabel.Padding = new Padding(10, 6, 10, 6);
        metricDefenseLabel.Text = "Peak Def\r\n0% Smashing";
        metricDefenseLabel.TextAlign = ContentAlignment.MiddleLeft;
        _summaryMetrics.Controls.Add(metricDefenseLabel, 3, 0);

        metricResistanceLabel.Dock = DockStyle.Fill;
        metricResistanceLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        metricResistanceLabel.Padding = new Padding(10, 6, 10, 6);
        metricResistanceLabel.Text = "Peak Res\r\n28.68% Smashing";
        metricResistanceLabel.TextAlign = ContentAlignment.MiddleLeft;
        _summaryMetrics.Controls.Add(metricResistanceLabel, 4, 0);

        _previewStatePanel.BackColor = Color.Black;
        _previewStatePanel.ColumnCount = 2;
        _previewStatePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _previewStatePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _previewStatePanel.Dock = DockStyle.Fill;
        _previewStatePanel.Margin = new Padding(0, 0, 0, 8);
        _previewStatePanel.RowCount = 1;
        _previewStatePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        upgradesPreviewLabel.Dock = DockStyle.Fill;
        upgradesPreviewLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        upgradesPreviewLabel.Padding = new Padding(10, 6, 10, 6);
        upgradesPreviewLabel.Text = "Applied Upgrades\r\nTrain Beasts";
        upgradesPreviewLabel.TextAlign = ContentAlignment.MiddleLeft;
        _previewStatePanel.Controls.Add(upgradesPreviewLabel, 0, 0);

        contextPreviewLabel.Dock = DockStyle.Fill;
        contextPreviewLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        contextPreviewLabel.Padding = new Padding(10, 6, 10, 6);
        contextPreviewLabel.Text = "Pet Context\r\nIn Range of Owner";
        contextPreviewLabel.TextAlign = ContentAlignment.MiddleLeft;
        _previewStatePanel.Controls.Add(contextPreviewLabel, 1, 0);

        _ribbonHost.BackColor = Color.Black;
        _ribbonHost.ColumnCount = 1;
        _ribbonHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _ribbonHost.Dock = DockStyle.Fill;
        _ribbonHost.Margin = new Padding(0, 0, 0, 8);
        _ribbonHost.RowCount = 2;
        _ribbonHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        _ribbonHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        ribbonSectionLabel.Dock = DockStyle.Fill;
        ribbonSectionLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        ribbonSectionLabel.Padding = new Padding(2, 0, 0, 0);
        ribbonSectionLabel.Text = "Active Real Pet Actors";
        ribbonSectionLabel.TextAlign = ContentAlignment.MiddleLeft;
        _ribbonHost.Controls.Add(ribbonSectionLabel, 0, 0);

        _actorRibbon.BackColor = Color.Black;
        _actorRibbon.Dock = DockStyle.Fill;
        _actorRibbon.Height = 58;
        _actorRibbon.Margin = Padding.Empty;
        _actorRibbon.ItemSelected += ActorRibbonOnItemSelected;
        _ribbonHost.Controls.Add(_actorRibbon, 0, 1);

        _contentHost.BackColor = Color.Black;
        _contentHost.Dock = DockStyle.Fill;
        _contentHost.Margin = Padding.Empty;

        _emptyStatePanel.BackColor = Color.Black;
        _emptyStatePanel.Dock = DockStyle.Fill;
        _emptyStatePanel.Margin = Padding.Empty;
        _emptyStatePanel.Visible = false;

        _emptyLabel.Dock = DockStyle.Fill;
        _emptyLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        _emptyLabel.Text = "No real pet actors are active in this build.\r\nOnce a build summons a persistent pet actor, its powers and totals will appear here.";
        _emptyLabel.TextAlign = ContentAlignment.MiddleCenter;
        _emptyStatePanel.Controls.Add(_emptyLabel);

        _bodySplit.BackColor = Color.Black;
        _bodySplit.Dock = DockStyle.Fill;
        _bodySplit.Margin = Padding.Empty;

        _detailView.Dock = DockStyle.Fill;
        _detailView.Margin = new Padding(0, 0, 8, 0);
        _bodySplit.Panel1.Controls.Add(_detailView);

        _powerPanel.BackColor = Color.Black;
        _powerPanel.ColumnCount = 1;
        _powerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _powerPanel.Dock = DockStyle.Fill;
        _powerPanel.Margin = Padding.Empty;
        _powerPanel.Padding = new Padding(8, 0, 0, 0);
        _powerPanel.RowCount = 2;
        _powerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        _powerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        powerSectionLabel.Dock = DockStyle.Fill;
        powerSectionLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        powerSectionLabel.Padding = new Padding(2, 0, 0, 0);
        powerSectionLabel.Text = "Powers";
        powerSectionLabel.TextAlign = ContentAlignment.MiddleLeft;
        _powerPanel.Controls.Add(powerSectionLabel, 0, 0);

        _powerGridScrollPanel.BackColor = Color.Black;
        _powerGridScrollPanel.Dock = DockStyle.Fill;
        _powerGridScrollPanel.Margin = Padding.Empty;

        _powerGrid.BackColor = Color.Black;
        _powerGrid.Dock = DockStyle.Top;
        _powerGrid.Margin = Padding.Empty;
        _powerGrid.PowerSelected += PowerGridOnPowerSelected;
        _powerGrid.PreviewToggleClicked += PowerGridOnPreviewToggleClicked;
        _powerGridScrollPanel.ContentPanel.Controls.Add(_powerGrid);
        _powerPanel.Controls.Add(_powerGridScrollPanel, 0, 1);
        _bodySplit.Panel2.Controls.Add(_powerPanel);

        _detailTotalsSplit.BackColor = Color.Black;
        _detailTotalsSplit.Dock = DockStyle.Fill;
        _detailTotalsSplit.FixedPanel = FixedPanel.Panel2;
        _detailTotalsSplit.Margin = Padding.Empty;
        _detailTotalsSplit.Orientation = Orientation.Horizontal;

        _totalsHost.BackColor = Color.Black;
        _totalsHost.ColumnCount = 1;
        _totalsHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _totalsHost.Dock = DockStyle.Fill;
        _totalsHost.Margin = Padding.Empty;
        _totalsHost.Padding = new Padding(0, 8, 0, 0);
        _totalsHost.RowCount = 2;
        _totalsHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        _totalsHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        totalsSectionLabel.Dock = DockStyle.Fill;
        totalsSectionLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        totalsSectionLabel.Padding = new Padding(2, 0, 0, 0);
        totalsSectionLabel.Text = "Actor Totals";
        totalsSectionLabel.TextAlign = ContentAlignment.MiddleLeft;
        _totalsHost.Controls.Add(totalsSectionLabel, 0, 0);

        _totalsScrollPanel.AutoScroll = true;
        _totalsScrollPanel.BackColor = Color.Black;
        _totalsScrollPanel.Dock = DockStyle.Fill;
        _totalsScrollPanel.Margin = Padding.Empty;
        _totalsScrollPanel.Padding = Padding.Empty;

        _totalsStack.AutoSize = true;
        _totalsStack.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _totalsStack.BackColor = Color.Black;
        _totalsStack.ColumnCount = 1;
        _totalsStack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _totalsStack.Dock = DockStyle.Top;
        _totalsStack.Margin = Padding.Empty;
        _totalsStack.RowCount = 10;

        _totalsScrollPanel.Controls.Add(_totalsStack);
        _totalsHost.Controls.Add(_totalsScrollPanel, 0, 1);
        _detailTotalsSplit.Panel2.Controls.Add(_totalsHost);

        _contentHost.Controls.Add(_emptyStatePanel);
        _contentHost.Controls.Add(_bodySplit);

        _rootLayout.Controls.Add(_headerPanel, 0, 0);
        _rootLayout.Controls.Add(_summaryMetrics, 0, 1);
        _rootLayout.Controls.Add(_previewStatePanel, 0, 2);
        _rootLayout.Controls.Add(_ribbonHost, 0, 3);
        _rootLayout.Controls.Add(_contentHost, 0, 4);
        Controls.Add(_rootLayout);

        _totalsStack.ResumeLayout(false);
        _totalsStack.PerformLayout();
        _totalsHost.ResumeLayout(false);
        _detailTotalsSplit.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_detailTotalsSplit).EndInit();
        _detailTotalsSplit.ResumeLayout(false);
        _powerPanel.ResumeLayout(false);
        _bodySplit.Panel2.ResumeLayout(false);
        _bodySplit.Panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_bodySplit).EndInit();
        _bodySplit.ResumeLayout(false);
        _emptyStatePanel.ResumeLayout(false);
        _contentHost.ResumeLayout(false);
        _ribbonHost.ResumeLayout(false);
        _previewStatePanel.ResumeLayout(false);
        _headerLayout.ResumeLayout(false);
        _headerPanel.ResumeLayout(false);
        _rootLayout.ResumeLayout(false);
        ResumeLayout(false);
    }
}

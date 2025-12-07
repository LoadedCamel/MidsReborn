using Mids_Reborn.Core;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls
{
    partial class MidsDataViewNeo
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (!_isDocked)
                {
                    try { Redock(); } catch { /* swallow on dispose */ }
                }
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            headerPanel = new Panel();
            LockButton = new FontAwesome.Sharp.IconButton();
            DockButton = new FontAwesome.Sharp.IconButton();
            titlePanel = new Panel();
            procToggle = new Mids_Reborn.UI.Controls.Test.MidsToggleSwitch();
            title = new Label();
            dvPages = new FormPages();
            infoView = new Page();
            powerStatsGrid = new Mids_Reborn.UI.Controls.Test.PowerStatsGrid();
            infoDamageDisplay = new ModernDamageDisplay();
            sliderHost = new Panel();
            midsTrackBar1 = new Mids_Reborn.UI.Controls.Test.MidsTrackBar();
            effectView = new Page();
            effectsGrid = new Mids_Reborn.UI.Controls.Test.PowerEffectsGrid();
            totalView = new Page();
            totalViewScrollPanel = new MidsVScrollPanel();
            coreDataList = new PairedListEx();
            totalsHeader3 = new Label();
            totalsResistLayoutPanel = new TableLayoutPanel();
            resistGraph2 = new Mids_Reborn.UI.Controls.Test.MultiStatGraph();
            resistGraph1 = new Mids_Reborn.UI.Controls.Test.MultiStatGraph();
            totalsHeader2 = new Label();
            totalsDefenseLayoutPanel = new TableLayoutPanel();
            defenseGraph2 = new Mids_Reborn.UI.Controls.Test.MultiStatGraph();
            defenseGraph1 = new Mids_Reborn.UI.Controls.Test.MultiStatGraph();
            totalsHeader1 = new Label();
            enhanceView = new Page();
            pnlEnhActive = new Panel();
            pnlEnhInactive = new Panel();
            enhDataList = new PairedListEx();
            enhanceSubtitlePanel = new Panel();
            subTitle = new Label();
            headerPanel.SuspendLayout();
            titlePanel.SuspendLayout();
            dvPages.SuspendLayout();
            infoView.SuspendLayout();
            sliderHost.SuspendLayout();
            effectView.SuspendLayout();
            totalView.SuspendLayout();
            totalViewScrollPanel.ContentPanel.SuspendLayout();
            totalViewScrollPanel.SuspendLayout();
            totalsResistLayoutPanel.SuspendLayout();
            totalsDefenseLayoutPanel.SuspendLayout();
            enhanceView.SuspendLayout();
            enhanceSubtitlePanel.SuspendLayout();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.Transparent;
            headerPanel.Controls.Add(LockButton);
            headerPanel.Controls.Add(DockButton);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(400, 26);
            headerPanel.TabIndex = 0;
            headerPanel.Paint += HeaderPanel_Paint;
            headerPanel.MouseDown += HeaderPanel_MouseDown;
            headerPanel.MouseLeave += HeaderPanel_MouseLeave;
            headerPanel.MouseMove += HeaderPanel_MouseMove;
            // 
            // LockButton
            // 
            LockButton.Dock = DockStyle.Right;
            LockButton.FlatAppearance.BorderSize = 0;
            LockButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(55, 115, 220);
            LockButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 60);
            LockButton.FlatStyle = FlatStyle.Flat;
            LockButton.ForeColor = Color.LimeGreen;
            LockButton.IconChar = FontAwesome.Sharp.IconChar.LockOpen;
            LockButton.IconColor = Color.LimeGreen;
            LockButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LockButton.IconSize = 24;
            LockButton.Location = new Point(342, 0);
            LockButton.Name = "LockButton";
            LockButton.Size = new Size(29, 26);
            LockButton.TabIndex = 3;
            LockButton.Tag = "KeepColors";
            LockButton.UseVisualStyleBackColor = true;
            // 
            // DockButton
            // 
            DockButton.Dock = DockStyle.Right;
            DockButton.FlatAppearance.BorderSize = 0;
            DockButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(55, 115, 220);
            DockButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 60);
            DockButton.FlatStyle = FlatStyle.Flat;
            DockButton.IconChar = FontAwesome.Sharp.IconChar.AnchorLock;
            DockButton.IconColor = Color.Silver;
            DockButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            DockButton.IconSize = 24;
            DockButton.Location = new Point(371, 0);
            DockButton.Name = "DockButton";
            DockButton.Size = new Size(29, 26);
            DockButton.TabIndex = 1;
            DockButton.Tag = "KeepColors";
            DockButton.UseVisualStyleBackColor = true;
            DockButton.Click += DockButton_Click;
            // 
            // titlePanel
            // 
            titlePanel.BackColor = Color.FromArgb(150, 0, 0, 0);
            titlePanel.Controls.Add(procToggle);
            titlePanel.Controls.Add(title);
            titlePanel.Dock = DockStyle.Top;
            titlePanel.ForeColor = Color.WhiteSmoke;
            titlePanel.Location = new Point(0, 26);
            titlePanel.Name = "titlePanel";
            titlePanel.Size = new Size(400, 32);
            titlePanel.TabIndex = 4;
            // 
            // procToggle
            // 
            procToggle.BackColor = Color.Transparent;
            procToggle.ContentGap = 4;
            procToggle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            procToggle.Location = new Point(271, 1);
            procToggle.Name = "procToggle";
            procToggle.Size = new Size(126, 28);
            procToggle.TabIndex = 1;
            procToggle.Text = "Proc Toggle";
            procToggle.Visible = false;
            // 
            // title
            // 
            title.BackColor = Color.Transparent;
            title.Dock = DockStyle.Left;
            title.FlatStyle = FlatStyle.Flat;
            title.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            title.Location = new Point(0, 0);
            title.Name = "title";
            title.Size = new Size(252, 32);
            title.TabIndex = 0;
            title.Text = "Title";
            title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dvPages
            // 
            dvPages.BackColor = Color.Transparent;
            dvPages.Controls.Add(infoView);
            dvPages.Controls.Add(effectView);
            dvPages.Controls.Add(totalView);
            dvPages.Controls.Add(enhanceView);
            dvPages.Dock = DockStyle.Fill;
            dvPages.ForeColor = Color.WhiteSmoke;
            dvPages.Location = new Point(0, 58);
            dvPages.Name = "dvPages";
            dvPages.Pages.Add(infoView);
            dvPages.Pages.Add(effectView);
            dvPages.Pages.Add(totalView);
            dvPages.Pages.Add(enhanceView);
            dvPages.SelectedIndex = 0;
            dvPages.Size = new Size(400, 351);
            dvPages.TabIndex = 5;
            // 
            // infoView
            // 
            infoView.AccessibleRole = AccessibleRole.None;
            infoView.Anchor = AnchorStyles.None;
            infoView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            infoView.BackColor = Color.Transparent;
            infoView.Controls.Add(powerStatsGrid);
            infoView.Controls.Add(infoDamageDisplay);
            infoView.Controls.Add(sliderHost);
            infoView.Dock = DockStyle.Fill;
            infoView.ForeColor = Color.WhiteSmoke;
            infoView.Location = new Point(0, 0);
            infoView.Name = "infoView";
            infoView.Size = new Size(400, 351);
            infoView.TabIndex = 0;
            infoView.Title = "My First Page";
            // 
            // powerStatsGrid
            // 
            powerStatsGrid.BackColor = Color.FromArgb(1, 7, 15);
            powerStatsGrid.Dock = DockStyle.Fill;
            powerStatsGrid.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            powerStatsGrid.GridPadding = 6;
            powerStatsGrid.Location = new Point(0, 31);
            powerStatsGrid.Name = "powerStatsGrid";
            powerStatsGrid.Size = new Size(400, 243);
            powerStatsGrid.TabIndex = 79;
            powerStatsGrid.TabStop = false;
            // 
            // infoDamageDisplay
            // 
            infoDamageDisplay.BackColor = Color.Transparent;
            infoDamageDisplay.BackgroundGradientEnd = Color.Transparent;
            infoDamageDisplay.BackgroundGradientStart = Color.Transparent;
            infoDamageDisplay.BarCornerRadius = 4;
            infoDamageDisplay.BarHeight = 26;
            infoDamageDisplay.BaseGradientEnd = Color.FromArgb(38, 252, 45);
            infoDamageDisplay.BaseGradientStart = Color.Green;
            infoDamageDisplay.Dock = DockStyle.Bottom;
            infoDamageDisplay.EnhancedGradientEnd = Color.FromArgb(252, 52, 38);
            infoDamageDisplay.EnhancedGradientStart = Color.Red;
            infoDamageDisplay.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            infoDamageDisplay.ForeColor = Color.WhiteSmoke;
            infoDamageDisplay.Location = new Point(0, 274);
            infoDamageDisplay.Name = "infoDamageDisplay";
            infoDamageDisplay.PaddingV = 3;
            infoDamageDisplay.Size = new Size(400, 77);
            infoDamageDisplay.TabIndex = 78;
            infoDamageDisplay.TextColor = Color.WhiteSmoke;
            infoDamageDisplay.ToolTipText = "";
            // 
            // sliderHost
            // 
            sliderHost.Controls.Add(midsTrackBar1);
            sliderHost.Dock = DockStyle.Top;
            sliderHost.Location = new Point(0, 0);
            sliderHost.Name = "sliderHost";
            sliderHost.Size = new Size(400, 31);
            sliderHost.TabIndex = 82;
            sliderHost.Visible = false;
            // 
            // midsTrackBar1
            // 
            midsTrackBar1.BackColor = Color.Transparent;
            midsTrackBar1.ForeColor = Color.WhiteSmoke;
            midsTrackBar1.Location = new Point(91, 6);
            midsTrackBar1.Name = "midsTrackBar1";
            midsTrackBar1.ShowValue = true;
            midsTrackBar1.Size = new Size(218, 18);
            midsTrackBar1.TabIndex = 81;
            midsTrackBar1.Text = "Targets:";
            midsTrackBar1.TextAlign = ContentAlignment.MiddleRight;
            midsTrackBar1.TextGap = 8;
            midsTrackBar1.TrackGap = 2;
            // 
            // effectView
            // 
            effectView.AccessibleRole = AccessibleRole.None;
            effectView.Anchor = AnchorStyles.None;
            effectView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            effectView.BackColor = Color.Transparent;
            effectView.Controls.Add(effectsGrid);
            effectView.Dock = DockStyle.Fill;
            effectView.ForeColor = Color.WhiteSmoke;
            effectView.Location = new Point(0, 0);
            effectView.Name = "effectView";
            effectView.Size = new Size(400, 351);
            effectView.TabIndex = 1;
            effectView.Title = "My Page Title";
            // 
            // effectsGrid
            // 
            effectsGrid.Dock = DockStyle.Fill;
            effectsGrid.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            effectsGrid.Location = new Point(0, 0);
            effectsGrid.Name = "effectsGrid";
            effectsGrid.Size = new Size(400, 351);
            effectsGrid.TabIndex = 0;
            effectsGrid.Text = "powerEffectsGrid1";
            // 
            // totalView
            // 
            totalView.AccessibleRole = AccessibleRole.None;
            totalView.Anchor = AnchorStyles.None;
            totalView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            totalView.BackColor = Color.Transparent;
            totalView.Controls.Add(totalViewScrollPanel);
            totalView.Dock = DockStyle.Fill;
            totalView.ForeColor = Color.WhiteSmoke;
            totalView.Location = new Point(0, 0);
            totalView.Name = "totalView";
            totalView.Size = new Size(400, 351);
            totalView.TabIndex = 2;
            totalView.Title = "My Page Title";
            // 
            // totalViewScrollPanel
            // 
            // 
            // totalViewScrollPanel.ContentPanel
            // 
            totalViewScrollPanel.ContentPanel.BackColor = Color.Transparent;
            totalViewScrollPanel.ContentPanel.Controls.Add(coreDataList);
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsHeader3);
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsResistLayoutPanel);
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsHeader2);
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsDefenseLayoutPanel);
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsHeader1);
            totalViewScrollPanel.ContentPanel.Location = new Point(0, 0);
            totalViewScrollPanel.ContentPanel.Margin = new Padding(0);
            totalViewScrollPanel.ContentPanel.Name = "ContentPanel";
            totalViewScrollPanel.ContentPanel.Size = new Size(400, 351);
            totalViewScrollPanel.ContentPanel.TabIndex = 0;
            totalViewScrollPanel.Dock = DockStyle.Fill;
            totalViewScrollPanel.Location = new Point(0, 0);
            totalViewScrollPanel.MinimumSize = new Size(64, 64);
            totalViewScrollPanel.Name = "totalViewScrollPanel";
            totalViewScrollPanel.Size = new Size(400, 351);
            totalViewScrollPanel.TabIndex = 0;
            totalViewScrollPanel.TabStop = true;
            // 
            // coreDataList
            // 
            coreDataList.BackColor = Color.FromArgb(1, 7, 15);
            coreDataList.Dock = DockStyle.Top;
            coreDataList.Font = new Font("Segoe UI Semibold", 9.25F, FontStyle.Bold);
            coreDataList.HighlightColor = Color.CornflowerBlue;
            coreDataList.HighlightTextColor = Color.Black;
            coreDataList.ItemColor = Color.Silver;
            coreDataList.Location = new Point(0, 247);
            coreDataList.Name = "coreDataList";
            coreDataList.SetItemsBold = false;
            coreDataList.ShowRuntimeSamples = true;
            coreDataList.Size = new Size(400, 78);
            coreDataList.TabIndex = 79;
            coreDataList.UseHighlighting = false;
            coreDataList.ValueAlternateColor = Color.Chartreuse;
            coreDataList.ValueColor = Color.WhiteSmoke;
            coreDataList.ValueConditionColor = Color.Firebrick;
            coreDataList.ValueSpecialColor = Color.SlateBlue;
            // 
            // totalsHeader3
            // 
            totalsHeader3.AutoSize = true;
            totalsHeader3.Dock = DockStyle.Top;
            totalsHeader3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            totalsHeader3.Location = new Point(0, 230);
            totalsHeader3.Name = "totalsHeader3";
            totalsHeader3.Size = new Size(36, 17);
            totalsHeader3.TabIndex = 78;
            totalsHeader3.Text = "Core";
            // 
            // totalsResistLayoutPanel
            // 
            totalsResistLayoutPanel.ColumnCount = 2;
            totalsResistLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            totalsResistLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            totalsResistLayoutPanel.Controls.Add(resistGraph2, 1, 0);
            totalsResistLayoutPanel.Controls.Add(resistGraph1, 0, 0);
            totalsResistLayoutPanel.Dock = DockStyle.Top;
            totalsResistLayoutPanel.Location = new Point(0, 135);
            totalsResistLayoutPanel.Name = "totalsResistLayoutPanel";
            totalsResistLayoutPanel.RowCount = 1;
            totalsResistLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            totalsResistLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            totalsResistLayoutPanel.Size = new Size(400, 95);
            totalsResistLayoutPanel.TabIndex = 0;
            // 
            // resistGraph2
            // 
            resistGraph2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            resistGraph2.BackColor = Color.Black;
            resistGraph2.Border = false;
            resistGraph2.ColorAbsorbed = Color.Gainsboro;
            resistGraph2.ColorBase = Color.FromArgb(0, 192, 192);
            resistGraph2.ColorEnh = Color.FromArgb(255, 128, 128);
            resistGraph2.ColorFadeEnd = Color.Teal;
            resistGraph2.ColorHighlight = Color.FromArgb(56, 255, 255, 255);
            resistGraph2.ColorLines = Color.Black;
            resistGraph2.ColorMarkerInner = Color.Black;
            resistGraph2.ColorMarkerOuter = Color.Yellow;
            resistGraph2.ColorOvercap = Color.LightSteelBlue;
            resistGraph2.DesignerSampleCount = 4;
            resistGraph2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            resistGraph2.ForeColor = Color.WhiteSmoke;
            resistGraph2.Location = new Point(203, 3);
            resistGraph2.Max = 100F;
            resistGraph2.Name = "resistGraph2";
            resistGraph2.SingleLineLabels = true;
            resistGraph2.Size = new Size(194, 89);
            resistGraph2.Style = Test.MultiStatGraph.GraphStyle.BaseOnly;
            resistGraph2.TabIndex = 1;
            resistGraph2.Text = "multiStatGraph1";
            resistGraph2.TextWidth = 100;
            // 
            // resistGraph1
            // 
            resistGraph1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            resistGraph1.BackColor = Color.Black;
            resistGraph1.Border = false;
            resistGraph1.ColorAbsorbed = Color.Gainsboro;
            resistGraph1.ColorBase = Color.FromArgb(0, 192, 192);
            resistGraph1.ColorEnh = Color.FromArgb(255, 128, 128);
            resistGraph1.ColorFadeEnd = Color.Teal;
            resistGraph1.ColorHighlight = Color.FromArgb(56, 255, 255, 255);
            resistGraph1.ColorLines = Color.Black;
            resistGraph1.ColorMarkerInner = Color.Black;
            resistGraph1.ColorMarkerOuter = Color.Yellow;
            resistGraph1.ColorOvercap = Color.LightSteelBlue;
            resistGraph1.DesignerSampleCount = 4;
            resistGraph1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            resistGraph1.ForeColor = Color.WhiteSmoke;
            resistGraph1.Location = new Point(3, 3);
            resistGraph1.Max = 100F;
            resistGraph1.Name = "resistGraph1";
            resistGraph1.SingleLineLabels = true;
            resistGraph1.Size = new Size(194, 89);
            resistGraph1.Style = Test.MultiStatGraph.GraphStyle.BaseOnly;
            resistGraph1.TabIndex = 0;
            resistGraph1.Text = "multiStatGraph1";
            resistGraph1.TextWidth = 100;
            resistGraph1.UseParentBackColor = true;
            // 
            // totalsHeader2
            // 
            totalsHeader2.AutoSize = true;
            totalsHeader2.Dock = DockStyle.Top;
            totalsHeader2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            totalsHeader2.Location = new Point(0, 118);
            totalsHeader2.Name = "totalsHeader2";
            totalsHeader2.Size = new Size(72, 17);
            totalsHeader2.TabIndex = 76;
            totalsHeader2.Text = "Resistance";
            // 
            // totalsDefenseLayoutPanel
            // 
            totalsDefenseLayoutPanel.ColumnCount = 2;
            totalsDefenseLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            totalsDefenseLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            totalsDefenseLayoutPanel.Controls.Add(defenseGraph2, 1, 0);
            totalsDefenseLayoutPanel.Controls.Add(defenseGraph1, 0, 0);
            totalsDefenseLayoutPanel.Dock = DockStyle.Top;
            totalsDefenseLayoutPanel.Location = new Point(0, 17);
            totalsDefenseLayoutPanel.Name = "totalsDefenseLayoutPanel";
            totalsDefenseLayoutPanel.RowCount = 1;
            totalsDefenseLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            totalsDefenseLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            totalsDefenseLayoutPanel.Size = new Size(400, 101);
            totalsDefenseLayoutPanel.TabIndex = 0;
            // 
            // defenseGraph2
            // 
            defenseGraph2.BackColor = Color.Black;
            defenseGraph2.Border = false;
            defenseGraph2.ColorAbsorbed = Color.Gainsboro;
            defenseGraph2.ColorBase = Color.FromArgb(192, 0, 192);
            defenseGraph2.ColorEnh = Color.Yellow;
            defenseGraph2.ColorFadeEnd = Color.Purple;
            defenseGraph2.ColorFadeStart = Color.FromArgb(25, 0, 0, 0);
            defenseGraph2.ColorHighlight = Color.FromArgb(56, 255, 255, 255);
            defenseGraph2.ColorLines = Color.FromArgb(96, 255, 255, 255);
            defenseGraph2.ColorMarkerInner = Color.Black;
            defenseGraph2.ColorMarkerOuter = Color.Yellow;
            defenseGraph2.ColorOvercap = Color.Indigo;
            defenseGraph2.DesignerSampleCount = 5;
            defenseGraph2.Dock = DockStyle.Fill;
            defenseGraph2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            defenseGraph2.ForeColor = Color.WhiteSmoke;
            defenseGraph2.ItemHeight = 15;
            defenseGraph2.Location = new Point(203, 3);
            defenseGraph2.Max = 100F;
            defenseGraph2.Name = "defenseGraph2";
            defenseGraph2.OuterBorder = false;
            defenseGraph2.SingleLineLabels = true;
            defenseGraph2.Size = new Size(194, 95);
            defenseGraph2.Style = Test.MultiStatGraph.GraphStyle.BaseOnly;
            defenseGraph2.TabIndex = 3;
            defenseGraph2.Text = "multiStatGraph1";
            defenseGraph2.TextWidth = 100;
            // 
            // defenseGraph1
            // 
            defenseGraph1.BackColor = Color.Black;
            defenseGraph1.Border = false;
            defenseGraph1.ColorAbsorbed = Color.Gainsboro;
            defenseGraph1.ColorBase = Color.FromArgb(192, 0, 192);
            defenseGraph1.ColorEnh = Color.Yellow;
            defenseGraph1.ColorFadeEnd = Color.Purple;
            defenseGraph1.ColorFadeStart = Color.FromArgb(1, 7, 15);
            defenseGraph1.ColorHighlight = Color.FromArgb(56, 255, 255, 255);
            defenseGraph1.ColorLines = Color.FromArgb(96, 255, 255, 255);
            defenseGraph1.ColorMarkerInner = Color.Black;
            defenseGraph1.ColorMarkerOuter = Color.Yellow;
            defenseGraph1.ColorOvercap = Color.Indigo;
            defenseGraph1.DesignerSampleCount = 5;
            defenseGraph1.Dock = DockStyle.Fill;
            defenseGraph1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            defenseGraph1.ForeColor = Color.WhiteSmoke;
            defenseGraph1.ItemHeight = 15;
            defenseGraph1.Location = new Point(3, 3);
            defenseGraph1.Max = 100F;
            defenseGraph1.Name = "defenseGraph1";
            defenseGraph1.OuterBorder = false;
            defenseGraph1.SingleLineLabels = true;
            defenseGraph1.Size = new Size(194, 95);
            defenseGraph1.Style = Test.MultiStatGraph.GraphStyle.BaseOnly;
            defenseGraph1.TabIndex = 2;
            defenseGraph1.Text = "multiStatGraph1";
            defenseGraph1.TextWidth = 100;
            // 
            // totalsHeader1
            // 
            totalsHeader1.AutoSize = true;
            totalsHeader1.Dock = DockStyle.Top;
            totalsHeader1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            totalsHeader1.Location = new Point(0, 0);
            totalsHeader1.Name = "totalsHeader1";
            totalsHeader1.Size = new Size(58, 17);
            totalsHeader1.TabIndex = 7;
            totalsHeader1.Text = "Defense";
            // 
            // enhanceView
            // 
            enhanceView.AccessibleRole = AccessibleRole.None;
            enhanceView.Anchor = AnchorStyles.None;
            enhanceView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            enhanceView.BackColor = Color.Transparent;
            enhanceView.Controls.Add(pnlEnhActive);
            enhanceView.Controls.Add(pnlEnhInactive);
            enhanceView.Controls.Add(enhDataList);
            enhanceView.Controls.Add(enhanceSubtitlePanel);
            enhanceView.Dock = DockStyle.Fill;
            enhanceView.ForeColor = Color.WhiteSmoke;
            enhanceView.Location = new Point(0, 0);
            enhanceView.Name = "enhanceView";
            enhanceView.Size = new Size(400, 351);
            enhanceView.TabIndex = 3;
            enhanceView.Title = "My Page Title";
            // 
            // pnlEnhActive
            // 
            pnlEnhActive.BackColor = Color.FromArgb(150, 0, 0, 0);
            pnlEnhActive.Location = new Point(0, 242);
            pnlEnhActive.Name = "pnlEnhActive";
            pnlEnhActive.Size = new Size(400, 50);
            pnlEnhActive.TabIndex = 84;
            // 
            // pnlEnhInactive
            // 
            pnlEnhInactive.BackColor = Color.FromArgb(150, 0, 0, 0);
            pnlEnhInactive.Location = new Point(0, 298);
            pnlEnhInactive.Name = "pnlEnhInactive";
            pnlEnhInactive.Size = new Size(400, 50);
            pnlEnhInactive.TabIndex = 0;
            // 
            // enhDataList
            // 
            enhDataList.BackColor = Color.FromArgb(150, 26, 28, 36);
            enhDataList.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            enhDataList.HighlightColor = Color.CornflowerBlue;
            enhDataList.HighlightTextColor = Color.Black;
            enhDataList.ItemColor = Color.Silver;
            enhDataList.Location = new Point(0, 20);
            enhDataList.Name = "enhDataList";
            enhDataList.SampleRowsPerColumn = 10;
            enhDataList.SetItemsBold = false;
            enhDataList.ShowRuntimeSamples = true;
            enhDataList.Size = new Size(397, 216);
            enhDataList.TabIndex = 80;
            enhDataList.UseHighlighting = false;
            enhDataList.ValueAlternateColor = Color.Chartreuse;
            enhDataList.ValueColor = Color.WhiteSmoke;
            enhDataList.ValueConditionColor = Color.Firebrick;
            enhDataList.ValueSpecialColor = Color.SlateBlue;
            // 
            // enhanceSubtitlePanel
            // 
            enhanceSubtitlePanel.BackColor = Color.FromArgb(150, 26, 28, 36);
            enhanceSubtitlePanel.Controls.Add(subTitle);
            enhanceSubtitlePanel.Dock = DockStyle.Top;
            enhanceSubtitlePanel.ForeColor = Color.WhiteSmoke;
            enhanceSubtitlePanel.Location = new Point(0, 0);
            enhanceSubtitlePanel.Name = "enhanceSubtitlePanel";
            enhanceSubtitlePanel.Size = new Size(400, 19);
            enhanceSubtitlePanel.TabIndex = 5;
            // 
            // subTitle
            // 
            subTitle.BackColor = Color.Transparent;
            subTitle.Dock = DockStyle.Fill;
            subTitle.FlatStyle = FlatStyle.Flat;
            subTitle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            subTitle.Location = new Point(0, 0);
            subTitle.Name = "subTitle";
            subTitle.Size = new Size(400, 19);
            subTitle.TabIndex = 0;
            subTitle.Text = "Subtitle";
            subTitle.TextAlign = ContentAlignment.TopCenter;
            // 
            // MidsDataViewNeo
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(1, 7, 15);
            Controls.Add(dvPages);
            Controls.Add(titlePanel);
            Controls.Add(headerPanel);
            Font = new Font("Segoe UI", 9F);
            Name = "MidsDataViewNeo";
            Size = new Size(400, 409);
            Resize += MidsDataView_Resize;
            headerPanel.ResumeLayout(false);
            titlePanel.ResumeLayout(false);
            dvPages.ResumeLayout(false);
            infoView.ResumeLayout(false);
            sliderHost.ResumeLayout(false);
            effectView.ResumeLayout(false);
            totalView.ResumeLayout(false);
            totalViewScrollPanel.ContentPanel.ResumeLayout(false);
            totalViewScrollPanel.ContentPanel.PerformLayout();
            totalViewScrollPanel.ResumeLayout(false);
            totalsResistLayoutPanel.ResumeLayout(false);
            totalsDefenseLayoutPanel.ResumeLayout(false);
            enhanceView.ResumeLayout(false);
            enhanceSubtitlePanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private FontAwesome.Sharp.IconButton DockButton;
        private Panel titlePanel;
        private Label title;
        private FormPages dvPages;
        private Page infoView;
        private Page effectView;
        private Page totalView;
        private Page enhanceView;
        private MidsVScrollPanel totalViewScrollPanel;
        private Label totalsHeader1;
        private Label totalsHeader2;
        private PairedListEx coreDataList;
        private Label totalsHeader3;
        private PairedListEx enhDataList;
        private Panel enhanceSubtitlePanel;
        private Label subTitle;
        private Panel pnlEnhActive;
        private Panel pnlEnhInactive;
        private FontAwesome.Sharp.IconButton LockButton;
        private Test.PowerStatsGrid powerStatsGrid;
        internal ModernDamageDisplay infoDamageDisplay;
        private Test.MidsTrackBar midsTrackBar1;
        private Test.MidsToggleSwitch procToggle;
        private Panel sliderHost;
        private TableLayoutPanel totalsDefenseLayoutPanel;
        private Test.MultiStatGraph defenseGraph2;
        private Test.MultiStatGraph defenseGraph1;
        private TableLayoutPanel totalsResistLayoutPanel;
        private Test.MultiStatGraph resistGraph1;
        private Test.MultiStatGraph resistGraph2;
        private Test.PowerEffectsGrid effectsGrid;
    }
}

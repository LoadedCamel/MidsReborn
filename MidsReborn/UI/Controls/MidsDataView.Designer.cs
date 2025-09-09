using Mids_Reborn.Core;

namespace Mids_Reborn.UI.Controls
{
    partial class MidsDataView
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MidsDataView));
            headerPanel = new Panel();
            LockButton = new FontAwesome.Sharp.IconButton();
            DockButton = new FontAwesome.Sharp.IconButton();
            titlePanel = new Panel();
            title = new Label();
            dvPages = new FormPages();
            infoView = new Page();
            infoLayoutPanel = new TableLayoutPanel();
            infoDataList = new PairedListEx();
            infoLDesc = new MidsRichTextView();
            infoSDesc = new RichTextBox();
            powerScaler = new CtlMultiGraph();
            damageHeader = new Label();
            infoDamageDisplay = new ModernDamageDisplay();
            effectView = new Page();
            effectViewScrollPanel = new MidsVScrollPanel();
            effectDataList3 = new PairedListEx();
            effectsHeader3 = new Label();
            effectDataList2 = new PairedListEx();
            effectsHeader2 = new Label();
            effectDataList1 = new PairedListEx();
            effectsHeader1 = new Label();
            totalView = new Page();
            totalViewScrollPanel = new MidsVScrollPanel();
            totalsFooter = new Label();
            coreDataList = new PairedListEx();
            totalsHeader3 = new Label();
            totalsResistPanel = new Panel();
            resistGraph2 = new CtlMultiGraph();
            resistGraph1 = new CtlMultiGraph();
            totalsHeader2 = new Label();
            totalsDefensePanel = new Panel();
            defenseGraph2 = new CtlMultiGraph();
            defenseGraph1 = new CtlMultiGraph();
            totalsHeader1 = new Label();
            enhanceView = new Page();
            spacer = new Panel();
            pnlEnhActive = new Panel();
            spacer2 = new Panel();
            pnlEnhInactive = new Panel();
            enhDataList = new PairedListEx();
            enhanceSubtitlePanel = new Panel();
            subTitle = new Label();
            dvToolTip = new ToolTip(components);
            headerPanel.SuspendLayout();
            titlePanel.SuspendLayout();
            dvPages.SuspendLayout();
            infoView.SuspendLayout();
            infoLayoutPanel.SuspendLayout();
            effectView.SuspendLayout();
            effectViewScrollPanel.ContentPanel.SuspendLayout();
            effectViewScrollPanel.SuspendLayout();
            totalView.SuspendLayout();
            totalViewScrollPanel.ContentPanel.SuspendLayout();
            totalViewScrollPanel.SuspendLayout();
            totalsResistPanel.SuspendLayout();
            totalsDefensePanel.SuspendLayout();
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
            LockButton.TabIndex = 2;
            LockButton.Tag = "KeepColors";
            LockButton.UseVisualStyleBackColor = true;
            LockButton.Click += LockButton_Click;
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
            titlePanel.Controls.Add(title);
            titlePanel.Dock = DockStyle.Top;
            titlePanel.ForeColor = Color.WhiteSmoke;
            titlePanel.Location = new Point(0, 26);
            titlePanel.Name = "titlePanel";
            titlePanel.Size = new Size(400, 20);
            titlePanel.TabIndex = 4;
            // 
            // title
            // 
            title.BackColor = Color.Transparent;
            title.Dock = DockStyle.Fill;
            title.FlatStyle = FlatStyle.Flat;
            title.Font = new Font("Noto Sans SemiBold", 10.25F, FontStyle.Bold);
            title.Location = new Point(0, 0);
            title.Name = "title";
            title.Size = new Size(400, 20);
            title.TabIndex = 0;
            title.Text = "Title";
            title.TextAlign = ContentAlignment.TopCenter;
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
            dvPages.Location = new Point(0, 46);
            dvPages.Name = "dvPages";
            dvPages.Pages.Add(infoView);
            dvPages.Pages.Add(effectView);
            dvPages.Pages.Add(totalView);
            dvPages.Pages.Add(enhanceView);
            dvPages.SelectedIndex = 0;
            dvPages.Size = new Size(400, 363);
            dvPages.TabIndex = 5;
            // 
            // infoView
            // 
            infoView.AccessibleRole = AccessibleRole.None;
            infoView.Anchor = AnchorStyles.None;
            infoView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            infoView.BackColor = Color.Transparent;
            infoView.Controls.Add(infoLayoutPanel);
            infoView.Dock = DockStyle.Fill;
            infoView.ForeColor = Color.WhiteSmoke;
            infoView.Location = new Point(0, 0);
            infoView.Name = "infoView";
            infoView.Size = new Size(400, 363);
            infoView.TabIndex = 0;
            infoView.Title = "My First Page";
            // 
            // infoLayoutPanel
            // 
            infoLayoutPanel.ColumnCount = 1;
            infoLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            infoLayoutPanel.Controls.Add(infoDataList, 0, 3);
            infoLayoutPanel.Controls.Add(infoLDesc, 0, 1);
            infoLayoutPanel.Controls.Add(infoSDesc, 0, 0);
            infoLayoutPanel.Controls.Add(powerScaler, 0, 2);
            infoLayoutPanel.Controls.Add(damageHeader, 0, 4);
            infoLayoutPanel.Controls.Add(infoDamageDisplay, 0, 5);
            infoLayoutPanel.Dock = DockStyle.Fill;
            infoLayoutPanel.Location = new Point(0, 0);
            infoLayoutPanel.Name = "infoLayoutPanel";
            infoLayoutPanel.RowCount = 6;
            infoLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            infoLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 19.3756542F));
            infoLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
            infoLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 51.83498F));
            infoLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
            infoLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 28.7893639F));
            infoLayoutPanel.Size = new Size(400, 363);
            infoLayoutPanel.TabIndex = 0;
            // 
            // infoDataList
            // 
            infoDataList.AutoScroll = true;
            infoDataList.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            infoDataList.BackColor = Color.FromArgb(150, 26, 28, 36);
            infoDataList.Dock = DockStyle.Fill;
            infoDataList.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            infoDataList.HighlightColor = Color.FromArgb(128, 128, 255);
            infoDataList.HighlightTextColor = Color.Black;
            infoDataList.ItemColor = Color.White;
            infoDataList.Location = new Point(4, 116);
            infoDataList.Margin = new Padding(4);
            infoDataList.Name = "infoDataList";
            infoDataList.SampleRowsPerColumn = 5;
            infoDataList.SetItemsBold = false;
            infoDataList.ShowRuntimeSamples = true;
            infoDataList.Size = new Size(392, 141);
            infoDataList.TabIndex = 73;
            infoDataList.UseHighlighting = true;
            infoDataList.ValueAlternateColor = Color.Chartreuse;
            infoDataList.ValueColor = Color.WhiteSmoke;
            infoDataList.ValueConditionColor = Color.Firebrick;
            infoDataList.ValueSpecialColor = Color.SlateBlue;
            // 
            // infoLDesc
            // 
            infoLDesc.BackColor = Color.FromArgb(15, 16, 21);
            infoLDesc.DetectUrls = false;
            infoLDesc.Dock = DockStyle.Fill;
            infoLDesc.Font = new Font("Noto Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            infoLDesc.ForeColor = Color.White;
            infoLDesc.Location = new Point(3, 43);
            infoLDesc.Name = "infoLDesc";
            infoLDesc.Rtf = resources.GetString("infoLDesc.Rtf");
            infoLDesc.Size = new Size(394, 49);
            infoLDesc.TabIndex = 74;
            infoLDesc.Text = resources.GetString("infoLDesc.Text");
            // 
            // infoSDesc
            // 
            infoSDesc.BackColor = Color.FromArgb(15, 16, 21);
            infoSDesc.BorderStyle = BorderStyle.None;
            infoSDesc.Dock = DockStyle.Fill;
            infoSDesc.Font = new Font("Noto Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            infoSDesc.ForeColor = Color.White;
            infoSDesc.Location = new Point(3, 3);
            infoSDesc.Name = "infoSDesc";
            infoSDesc.ReadOnly = true;
            infoSDesc.ScrollBars = RichTextBoxScrollBars.None;
            infoSDesc.Size = new Size(394, 34);
            infoSDesc.TabIndex = 75;
            infoSDesc.Text = "PBAoE, Minor DMG (Energy), Minor DoT (Toxic), Foe -Regen, Self HP, +End, Res (-Regen)";
            // 
            // powerScaler
            // 
            powerScaler.BackColor = Color.FromArgb(45, 45, 48);
            powerScaler.BackgroundImage = (Image)resources.GetObject("powerScaler.BackgroundImage");
            powerScaler.BaseBarColors = (List<Color>)resources.GetObject("powerScaler.BaseBarColors");
            powerScaler.Border = true;
            powerScaler.BorderColor = Color.Black;
            powerScaler.Clickable = true;
            powerScaler.ColorAbsorbed = Color.Gainsboro;
            powerScaler.ColorBase = Color.FromArgb(64, 255, 64);
            powerScaler.ColorEnh = Color.Yellow;
            powerScaler.ColorFadeEnd = Color.FromArgb(0, 192, 0);
            powerScaler.ColorFadeStart = Color.FromArgb(150, 0, 0, 0);
            powerScaler.ColorHighlight = Color.Gray;
            powerScaler.ColorLines = Color.Black;
            powerScaler.ColorMarkerInner = Color.Red;
            powerScaler.ColorMarkerOuter = Color.Black;
            powerScaler.ColorOvercap = Color.Black;
            powerScaler.DifferentiateColors = false;
            powerScaler.Dock = DockStyle.Fill;
            powerScaler.DrawRuler = false;
            powerScaler.Dual = false;
            powerScaler.EnhBarColors = (List<Color>)resources.GetObject("powerScaler.EnhBarColors");
            powerScaler.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold, GraphicsUnit.Pixel);
            powerScaler.ForcedMax = 0F;
            powerScaler.ForeColor = Color.WhiteSmoke;
            powerScaler.Highlight = false;
            powerScaler.ItemFontSizeOverride = 0F;
            powerScaler.ItemHeight = 10;
            powerScaler.Lines = true;
            powerScaler.Location = new Point(3, 98);
            powerScaler.MarkerValue = 0F;
            powerScaler.Max = 100F;
            powerScaler.MaxItems = 1;
            powerScaler.MinimumSize = new Size(0, 20);
            powerScaler.Name = "powerScaler";
            powerScaler.OuterBorder = false;
            powerScaler.Overcap = false;
            powerScaler.OvercapColors = (List<Color>)resources.GetObject("powerScaler.OvercapColors");
            powerScaler.PaddingX = 2F;
            powerScaler.PaddingY = 2F;
            powerScaler.PerItemScales = (List<float>)resources.GetObject("powerScaler.PerItemScales");
            powerScaler.RulerPos = CtlMultiGraph.RulerPosition.Top;
            powerScaler.ScaleHeight = 32;
            powerScaler.ScaleIndex = 8;
            powerScaler.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            powerScaler.ShowScale = false;
            powerScaler.SingleLineLabels = true;
            powerScaler.Size = new Size(394, 20);
            powerScaler.Style = Enums.GraphStyle.baseOnly;
            powerScaler.TabIndex = 76;
            powerScaler.TextWidth = 80;
            // 
            // damageHeader
            // 
            damageHeader.BackColor = Color.FromArgb(150, 0, 0, 0);
            damageHeader.Dock = DockStyle.Fill;
            damageHeader.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            damageHeader.ForeColor = Color.White;
            damageHeader.Location = new Point(3, 261);
            damageHeader.Name = "damageHeader";
            damageHeader.Size = new Size(394, 17);
            damageHeader.TabIndex = 73;
            damageHeader.Text = "Damage (Green = Base | Red = Enhanced)";
            damageHeader.TextAlign = ContentAlignment.TopCenter;
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
            infoDamageDisplay.Dock = DockStyle.Fill;
            infoDamageDisplay.EnhancedGradientEnd = Color.FromArgb(252, 52, 38);
            infoDamageDisplay.EnhancedGradientStart = Color.Red;
            infoDamageDisplay.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            infoDamageDisplay.ForeColor = Color.WhiteSmoke;
            infoDamageDisplay.Location = new Point(3, 281);
            infoDamageDisplay.Name = "infoDamageDisplay";
            infoDamageDisplay.PaddingV = 3;
            infoDamageDisplay.Size = new Size(394, 79);
            infoDamageDisplay.TabIndex = 78;
            infoDamageDisplay.TextColor = Color.WhiteSmoke;
            infoDamageDisplay.ToolTipText = "";
            // 
            // effectView
            // 
            effectView.AccessibleRole = AccessibleRole.None;
            effectView.Anchor = AnchorStyles.None;
            effectView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            effectView.BackColor = Color.Transparent;
            effectView.Controls.Add(effectViewScrollPanel);
            effectView.Dock = DockStyle.Fill;
            effectView.ForeColor = Color.WhiteSmoke;
            effectView.Location = new Point(0, 0);
            effectView.Name = "effectView";
            effectView.Size = new Size(400, 364);
            effectView.TabIndex = 1;
            effectView.Title = "My Page Title";
            // 
            // effectViewScrollPanel
            // 
            // 
            // effectViewScrollPanel.ContentPanel
            // 
            effectViewScrollPanel.ContentPanel.BackColor = Color.Transparent;
            effectViewScrollPanel.ContentPanel.Controls.Add(effectDataList3);
            effectViewScrollPanel.ContentPanel.Controls.Add(effectsHeader3);
            effectViewScrollPanel.ContentPanel.Controls.Add(effectDataList2);
            effectViewScrollPanel.ContentPanel.Controls.Add(effectsHeader2);
            effectViewScrollPanel.ContentPanel.Controls.Add(effectDataList1);
            effectViewScrollPanel.ContentPanel.Controls.Add(effectsHeader1);
            effectViewScrollPanel.ContentPanel.Location = new Point(0, 0);
            effectViewScrollPanel.ContentPanel.Margin = new Padding(0);
            effectViewScrollPanel.ContentPanel.Name = "ContentPanel";
            effectViewScrollPanel.ContentPanel.Size = new Size(384, 526);
            effectViewScrollPanel.ContentPanel.TabIndex = 0;
            effectViewScrollPanel.Dock = DockStyle.Fill;
            effectViewScrollPanel.Location = new Point(0, 0);
            effectViewScrollPanel.MinimumSize = new Size(64, 64);
            effectViewScrollPanel.Name = "effectViewScrollPanel";
            effectViewScrollPanel.Size = new Size(400, 364);
            effectViewScrollPanel.TabIndex = 0;
            effectViewScrollPanel.TabStop = true;
            // 
            // effectDataList3
            // 
            effectDataList3.BackColor = Color.FromArgb(150, 26, 28, 36);
            effectDataList3.Dock = DockStyle.Top;
            effectDataList3.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            effectDataList3.HighlightColor = Color.CornflowerBlue;
            effectDataList3.HighlightTextColor = Color.Black;
            effectDataList3.ItemColor = Color.Silver;
            effectDataList3.Location = new Point(0, 366);
            effectDataList3.Name = "effectDataList3";
            effectDataList3.SetItemsBold = false;
            effectDataList3.ShowRuntimeSamples = true;
            effectDataList3.Size = new Size(384, 150);
            effectDataList3.TabIndex = 11;
            effectDataList3.UseHighlighting = false;
            effectDataList3.ValueAlternateColor = Color.Chartreuse;
            effectDataList3.ValueColor = Color.WhiteSmoke;
            effectDataList3.ValueConditionColor = Color.Firebrick;
            effectDataList3.ValueSpecialColor = Color.SlateBlue;
            // 
            // effectsHeader3
            // 
            effectsHeader3.AutoSize = true;
            effectsHeader3.Dock = DockStyle.Top;
            effectsHeader3.Font = new Font("Noto Sans SemiBold", 10.25F, FontStyle.Bold);
            effectsHeader3.Location = new Point(0, 344);
            effectsHeader3.Name = "effectsHeader3";
            effectsHeader3.Size = new Size(91, 22);
            effectsHeader3.TabIndex = 10;
            effectsHeader3.Text = "Misc Effects";
            // 
            // effectDataList2
            // 
            effectDataList2.BackColor = Color.FromArgb(150, 26, 28, 36);
            effectDataList2.Dock = DockStyle.Top;
            effectDataList2.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold);
            effectDataList2.HighlightColor = Color.CornflowerBlue;
            effectDataList2.HighlightTextColor = Color.Black;
            effectDataList2.ItemColor = Color.Silver;
            effectDataList2.Location = new Point(0, 194);
            effectDataList2.Name = "effectDataList2";
            effectDataList2.SampleRowsPerColumn = 5;
            effectDataList2.SetItemsBold = false;
            effectDataList2.ShowRuntimeSamples = true;
            effectDataList2.Size = new Size(384, 150);
            effectDataList2.TabIndex = 9;
            effectDataList2.UseHighlighting = false;
            effectDataList2.ValueAlternateColor = Color.Chartreuse;
            effectDataList2.ValueColor = Color.WhiteSmoke;
            effectDataList2.ValueConditionColor = Color.Firebrick;
            effectDataList2.ValueSpecialColor = Color.SlateBlue;
            // 
            // effectsHeader2
            // 
            effectsHeader2.AutoSize = true;
            effectsHeader2.Dock = DockStyle.Top;
            effectsHeader2.Font = new Font("Noto Sans SemiBold", 10.25F, FontStyle.Bold);
            effectsHeader2.Location = new Point(0, 172);
            effectsHeader2.Name = "effectsHeader2";
            effectsHeader2.Size = new Size(132, 22);
            effectsHeader2.TabIndex = 8;
            effectsHeader2.Text = "Secondary Effects";
            // 
            // effectDataList1
            // 
            effectDataList1.BackColor = Color.FromArgb(150, 26, 28, 36);
            effectDataList1.Dock = DockStyle.Top;
            effectDataList1.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold);
            effectDataList1.HighlightColor = Color.CornflowerBlue;
            effectDataList1.HighlightTextColor = Color.Black;
            effectDataList1.ItemColor = Color.Silver;
            effectDataList1.Location = new Point(0, 22);
            effectDataList1.Name = "effectDataList1";
            effectDataList1.SampleRowsPerColumn = 5;
            effectDataList1.SetItemsBold = false;
            effectDataList1.ShowRuntimeSamples = true;
            effectDataList1.Size = new Size(384, 150);
            effectDataList1.TabIndex = 7;
            effectDataList1.UseHighlighting = false;
            effectDataList1.ValueAlternateColor = Color.Chartreuse;
            effectDataList1.ValueColor = Color.WhiteSmoke;
            effectDataList1.ValueConditionColor = Color.Firebrick;
            effectDataList1.ValueSpecialColor = Color.SlateBlue;
            // 
            // effectsHeader1
            // 
            effectsHeader1.AutoSize = true;
            effectsHeader1.Dock = DockStyle.Top;
            effectsHeader1.Font = new Font("Noto Sans SemiBold", 10.25F, FontStyle.Bold);
            effectsHeader1.Location = new Point(0, 0);
            effectsHeader1.Name = "effectsHeader1";
            effectsHeader1.Size = new Size(114, 22);
            effectsHeader1.TabIndex = 6;
            effectsHeader1.Text = "Primary Effects";
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
            totalView.Size = new Size(400, 364);
            totalView.TabIndex = 2;
            totalView.Title = "My Page Title";
            // 
            // totalViewScrollPanel
            // 
            // 
            // totalViewScrollPanel.ContentPanel
            // 
            totalViewScrollPanel.ContentPanel.BackColor = Color.Transparent;
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsFooter);
            totalViewScrollPanel.ContentPanel.Controls.Add(coreDataList);
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsHeader3);
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsResistPanel);
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsHeader2);
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsDefensePanel);
            totalViewScrollPanel.ContentPanel.Controls.Add(totalsHeader1);
            totalViewScrollPanel.ContentPanel.Location = new Point(0, 0);
            totalViewScrollPanel.ContentPanel.Margin = new Padding(0);
            totalViewScrollPanel.ContentPanel.Name = "ContentPanel";
            totalViewScrollPanel.ContentPanel.Size = new Size(400, 364);
            totalViewScrollPanel.ContentPanel.TabIndex = 0;
            totalViewScrollPanel.Dock = DockStyle.Fill;
            totalViewScrollPanel.Location = new Point(0, 0);
            totalViewScrollPanel.MinimumSize = new Size(64, 64);
            totalViewScrollPanel.Name = "totalViewScrollPanel";
            totalViewScrollPanel.Size = new Size(400, 364);
            totalViewScrollPanel.TabIndex = 0;
            totalViewScrollPanel.TabStop = true;
            // 
            // totalsFooter
            // 
            totalsFooter.Dock = DockStyle.Top;
            totalsFooter.Font = new Font("Noto Sans SemiBold", 10.25F, FontStyle.Bold);
            totalsFooter.Location = new Point(0, 323);
            totalsFooter.Name = "totalsFooter";
            totalsFooter.Size = new Size(400, 22);
            totalsFooter.TabIndex = 80;
            totalsFooter.Text = "Use 'View Totals' for more info.";
            totalsFooter.TextAlign = ContentAlignment.TopCenter;
            // 
            // coreDataList
            // 
            coreDataList.BackColor = Color.FromArgb(150, 26, 28, 36);
            coreDataList.Dock = DockStyle.Top;
            coreDataList.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            coreDataList.HighlightColor = Color.CornflowerBlue;
            coreDataList.HighlightTextColor = Color.Black;
            coreDataList.ItemColor = Color.Silver;
            coreDataList.Location = new Point(0, 242);
            coreDataList.Name = "coreDataList";
            coreDataList.SetItemsBold = false;
            coreDataList.ShowRuntimeSamples = true;
            coreDataList.Size = new Size(400, 81);
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
            totalsHeader3.Font = new Font("Noto Sans SemiBold", 9.25F, FontStyle.Bold);
            totalsHeader3.Location = new Point(0, 222);
            totalsHeader3.Name = "totalsHeader3";
            totalsHeader3.Size = new Size(39, 20);
            totalsHeader3.TabIndex = 78;
            totalsHeader3.Text = "Core";
            // 
            // totalsResistPanel
            // 
            totalsResistPanel.Controls.Add(resistGraph2);
            totalsResistPanel.Controls.Add(resistGraph1);
            totalsResistPanel.Dock = DockStyle.Top;
            totalsResistPanel.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            totalsResistPanel.Location = new Point(0, 147);
            totalsResistPanel.Name = "totalsResistPanel";
            totalsResistPanel.Size = new Size(400, 75);
            totalsResistPanel.TabIndex = 77;
            // 
            // resistGraph2
            // 
            resistGraph2.BackColor = Color.Black;
            resistGraph2.BackgroundImage = (Image)resources.GetObject("resistGraph2.BackgroundImage");
            resistGraph2.BaseBarColors = (List<Color>)resources.GetObject("resistGraph2.BaseBarColors");
            resistGraph2.Border = true;
            resistGraph2.BorderColor = Color.Black;
            resistGraph2.Clickable = false;
            resistGraph2.ColorAbsorbed = Color.Gainsboro;
            resistGraph2.ColorBase = Color.FromArgb(0, 192, 192);
            resistGraph2.ColorEnh = Color.FromArgb(255, 128, 128);
            resistGraph2.ColorFadeEnd = Color.Teal;
            resistGraph2.ColorFadeStart = Color.Black;
            resistGraph2.ColorHighlight = Color.Gray;
            resistGraph2.ColorLines = Color.Black;
            resistGraph2.ColorMarkerInner = Color.Black;
            resistGraph2.ColorMarkerOuter = Color.Yellow;
            resistGraph2.ColorOvercap = Color.Black;
            resistGraph2.DifferentiateColors = false;
            resistGraph2.Dock = DockStyle.Right;
            resistGraph2.DrawRuler = false;
            resistGraph2.Dual = true;
            resistGraph2.EnhBarColors = (List<Color>)resources.GetObject("resistGraph2.EnhBarColors");
            resistGraph2.Font = new Font("Microsoft Sans Serif", 9.25F);
            resistGraph2.ForcedMax = 0F;
            resistGraph2.ForeColor = Color.FromArgb(192, 192, 255);
            resistGraph2.Highlight = true;
            resistGraph2.ItemFontSizeOverride = 11.25F;
            resistGraph2.ItemHeight = 13;
            resistGraph2.Lines = true;
            resistGraph2.Location = new Point(211, 0);
            resistGraph2.MarkerValue = 0F;
            resistGraph2.Max = 100F;
            resistGraph2.MaxItems = 4;
            resistGraph2.Name = "resistGraph2";
            resistGraph2.OuterBorder = false;
            resistGraph2.Overcap = false;
            resistGraph2.OvercapColors = (List<Color>)resources.GetObject("resistGraph2.OvercapColors");
            resistGraph2.PaddingX = 2F;
            resistGraph2.PaddingY = 4F;
            resistGraph2.PerItemScales = (List<float>)resources.GetObject("resistGraph2.PerItemScales");
            resistGraph2.RulerPos = CtlMultiGraph.RulerPosition.Top;
            resistGraph2.ScaleHeight = 32;
            resistGraph2.ScaleIndex = 8;
            resistGraph2.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            resistGraph2.ShowScale = false;
            resistGraph2.SingleLineLabels = true;
            resistGraph2.Size = new Size(189, 75);
            resistGraph2.Style = Enums.GraphStyle.Stacked;
            resistGraph2.TabIndex = 76;
            resistGraph2.TextWidth = 100;
            // 
            // resistGraph1
            // 
            resistGraph1.BackColor = Color.Black;
            resistGraph1.BackgroundImage = (Image)resources.GetObject("resistGraph1.BackgroundImage");
            resistGraph1.BaseBarColors = (List<Color>)resources.GetObject("resistGraph1.BaseBarColors");
            resistGraph1.Border = true;
            resistGraph1.BorderColor = Color.Black;
            resistGraph1.Clickable = false;
            resistGraph1.ColorAbsorbed = Color.Gainsboro;
            resistGraph1.ColorBase = Color.FromArgb(0, 192, 192);
            resistGraph1.ColorEnh = Color.FromArgb(255, 128, 128);
            resistGraph1.ColorFadeEnd = Color.Teal;
            resistGraph1.ColorFadeStart = Color.Black;
            resistGraph1.ColorHighlight = Color.Gray;
            resistGraph1.ColorLines = Color.Black;
            resistGraph1.ColorMarkerInner = Color.Black;
            resistGraph1.ColorMarkerOuter = Color.Yellow;
            resistGraph1.ColorOvercap = Color.Black;
            resistGraph1.DifferentiateColors = false;
            resistGraph1.Dock = DockStyle.Left;
            resistGraph1.DrawRuler = false;
            resistGraph1.Dual = true;
            resistGraph1.EnhBarColors = (List<Color>)resources.GetObject("resistGraph1.EnhBarColors");
            resistGraph1.Font = new Font("Segoe UI", 9.25F);
            resistGraph1.ForcedMax = 0F;
            resistGraph1.ForeColor = Color.FromArgb(192, 192, 255);
            resistGraph1.Highlight = true;
            resistGraph1.ItemFontSizeOverride = 11.25F;
            resistGraph1.ItemHeight = 13;
            resistGraph1.Lines = true;
            resistGraph1.Location = new Point(0, 0);
            resistGraph1.MarkerValue = 0F;
            resistGraph1.Max = 100F;
            resistGraph1.MaxItems = 4;
            resistGraph1.Name = "resistGraph1";
            resistGraph1.OuterBorder = false;
            resistGraph1.Overcap = false;
            resistGraph1.OvercapColors = (List<Color>)resources.GetObject("resistGraph1.OvercapColors");
            resistGraph1.PaddingX = 2F;
            resistGraph1.PaddingY = 4F;
            resistGraph1.PerItemScales = (List<float>)resources.GetObject("resistGraph1.PerItemScales");
            resistGraph1.RulerPos = CtlMultiGraph.RulerPosition.Top;
            resistGraph1.ScaleHeight = 32;
            resistGraph1.ScaleIndex = 8;
            resistGraph1.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            resistGraph1.ShowScale = false;
            resistGraph1.SingleLineLabels = true;
            resistGraph1.Size = new Size(196, 75);
            resistGraph1.Style = Enums.GraphStyle.Stacked;
            resistGraph1.TabIndex = 75;
            resistGraph1.TextWidth = 100;
            // 
            // totalsHeader2
            // 
            totalsHeader2.AutoSize = true;
            totalsHeader2.Dock = DockStyle.Top;
            totalsHeader2.Font = new Font("Noto Sans SemiBold", 9.25F, FontStyle.Bold);
            totalsHeader2.Location = new Point(0, 127);
            totalsHeader2.Name = "totalsHeader2";
            totalsHeader2.Size = new Size(77, 20);
            totalsHeader2.TabIndex = 76;
            totalsHeader2.Text = "Resistance";
            // 
            // totalsDefensePanel
            // 
            totalsDefensePanel.Controls.Add(defenseGraph2);
            totalsDefensePanel.Controls.Add(defenseGraph1);
            totalsDefensePanel.Dock = DockStyle.Top;
            totalsDefensePanel.Location = new Point(0, 20);
            totalsDefensePanel.Name = "totalsDefensePanel";
            totalsDefensePanel.Size = new Size(400, 107);
            totalsDefensePanel.TabIndex = 75;
            // 
            // defenseGraph2
            // 
            defenseGraph2.BackColor = Color.Black;
            defenseGraph2.BackgroundImage = (Image)resources.GetObject("defenseGraph2.BackgroundImage");
            defenseGraph2.BaseBarColors = (List<Color>)resources.GetObject("defenseGraph2.BaseBarColors");
            defenseGraph2.Border = true;
            defenseGraph2.BorderColor = Color.Black;
            defenseGraph2.Clickable = false;
            defenseGraph2.ColorAbsorbed = Color.Gainsboro;
            defenseGraph2.ColorBase = Color.FromArgb(192, 0, 192);
            defenseGraph2.ColorEnh = Color.Yellow;
            defenseGraph2.ColorFadeEnd = Color.Purple;
            defenseGraph2.ColorFadeStart = Color.Black;
            defenseGraph2.ColorHighlight = Color.Gray;
            defenseGraph2.ColorLines = Color.Black;
            defenseGraph2.ColorMarkerInner = Color.Black;
            defenseGraph2.ColorMarkerOuter = Color.Yellow;
            defenseGraph2.ColorOvercap = Color.Black;
            defenseGraph2.DifferentiateColors = false;
            defenseGraph2.Dock = DockStyle.Right;
            defenseGraph2.DrawRuler = false;
            defenseGraph2.Dual = true;
            defenseGraph2.EnhBarColors = (List<Color>)resources.GetObject("defenseGraph2.EnhBarColors");
            defenseGraph2.Font = new Font("Segoe UI", 9.25F);
            defenseGraph2.ForcedMax = 0F;
            defenseGraph2.ForeColor = Color.FromArgb(192, 192, 255);
            defenseGraph2.Highlight = true;
            defenseGraph2.ItemFontSizeOverride = 11.25F;
            defenseGraph2.ItemHeight = 13;
            defenseGraph2.Lines = true;
            defenseGraph2.Location = new Point(211, 0);
            defenseGraph2.MarkerValue = 0F;
            defenseGraph2.Max = 100F;
            defenseGraph2.MaxItems = 6;
            defenseGraph2.Name = "defenseGraph2";
            defenseGraph2.OuterBorder = false;
            defenseGraph2.Overcap = true;
            defenseGraph2.OvercapColors = (List<Color>)resources.GetObject("defenseGraph2.OvercapColors");
            defenseGraph2.PaddingX = 2F;
            defenseGraph2.PaddingY = 4F;
            defenseGraph2.PerItemScales = (List<float>)resources.GetObject("defenseGraph2.PerItemScales");
            defenseGraph2.RulerPos = CtlMultiGraph.RulerPosition.Top;
            defenseGraph2.ScaleHeight = 32;
            defenseGraph2.ScaleIndex = 8;
            defenseGraph2.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            defenseGraph2.ShowScale = false;
            defenseGraph2.SingleLineLabels = true;
            defenseGraph2.Size = new Size(189, 107);
            defenseGraph2.Style = Enums.GraphStyle.baseOnly;
            defenseGraph2.TabIndex = 74;
            defenseGraph2.TextWidth = 100;
            // 
            // defenseGraph1
            // 
            defenseGraph1.BackColor = Color.Black;
            defenseGraph1.BackgroundImage = (Image)resources.GetObject("defenseGraph1.BackgroundImage");
            defenseGraph1.BaseBarColors = (List<Color>)resources.GetObject("defenseGraph1.BaseBarColors");
            defenseGraph1.Border = true;
            defenseGraph1.BorderColor = Color.Black;
            defenseGraph1.Clickable = false;
            defenseGraph1.ColorAbsorbed = Color.Gainsboro;
            defenseGraph1.ColorBase = Color.FromArgb(192, 0, 192);
            defenseGraph1.ColorEnh = Color.Yellow;
            defenseGraph1.ColorFadeEnd = Color.Purple;
            defenseGraph1.ColorFadeStart = Color.Black;
            defenseGraph1.ColorHighlight = Color.Gray;
            defenseGraph1.ColorLines = Color.Black;
            defenseGraph1.ColorMarkerInner = Color.Black;
            defenseGraph1.ColorMarkerOuter = Color.Yellow;
            defenseGraph1.ColorOvercap = Color.Black;
            defenseGraph1.DifferentiateColors = false;
            defenseGraph1.Dock = DockStyle.Left;
            defenseGraph1.DrawRuler = false;
            defenseGraph1.Dual = true;
            defenseGraph1.EnhBarColors = (List<Color>)resources.GetObject("defenseGraph1.EnhBarColors");
            defenseGraph1.Font = new Font("Segoe UI", 9.25F);
            defenseGraph1.ForcedMax = 0F;
            defenseGraph1.ForeColor = Color.FromArgb(192, 192, 255);
            defenseGraph1.Highlight = true;
            defenseGraph1.ItemFontSizeOverride = 11.25F;
            defenseGraph1.ItemHeight = 13;
            defenseGraph1.Lines = true;
            defenseGraph1.Location = new Point(0, 0);
            defenseGraph1.MarkerValue = 0F;
            defenseGraph1.Max = 100F;
            defenseGraph1.MaxItems = 6;
            defenseGraph1.Name = "defenseGraph1";
            defenseGraph1.OuterBorder = false;
            defenseGraph1.Overcap = true;
            defenseGraph1.OvercapColors = (List<Color>)resources.GetObject("defenseGraph1.OvercapColors");
            defenseGraph1.PaddingX = 2F;
            defenseGraph1.PaddingY = 4F;
            defenseGraph1.PerItemScales = (List<float>)resources.GetObject("defenseGraph1.PerItemScales");
            defenseGraph1.RulerPos = CtlMultiGraph.RulerPosition.Top;
            defenseGraph1.ScaleHeight = 32;
            defenseGraph1.ScaleIndex = 8;
            defenseGraph1.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            defenseGraph1.ShowScale = false;
            defenseGraph1.SingleLineLabels = true;
            defenseGraph1.Size = new Size(196, 107);
            defenseGraph1.Style = Enums.GraphStyle.baseOnly;
            defenseGraph1.TabIndex = 73;
            defenseGraph1.TextWidth = 100;
            // 
            // totalsHeader1
            // 
            totalsHeader1.AutoSize = true;
            totalsHeader1.Dock = DockStyle.Top;
            totalsHeader1.Font = new Font("Noto Sans SemiBold", 9.25F, FontStyle.Bold);
            totalsHeader1.Location = new Point(0, 0);
            totalsHeader1.Name = "totalsHeader1";
            totalsHeader1.Size = new Size(62, 20);
            totalsHeader1.TabIndex = 7;
            totalsHeader1.Text = "Defense";
            // 
            // enhanceView
            // 
            enhanceView.AccessibleRole = AccessibleRole.None;
            enhanceView.Anchor = AnchorStyles.None;
            enhanceView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            enhanceView.BackColor = Color.Transparent;
            enhanceView.Controls.Add(spacer);
            enhanceView.Controls.Add(pnlEnhActive);
            enhanceView.Controls.Add(spacer2);
            enhanceView.Controls.Add(pnlEnhInactive);
            enhanceView.Controls.Add(enhDataList);
            enhanceView.Controls.Add(enhanceSubtitlePanel);
            enhanceView.Dock = DockStyle.Fill;
            enhanceView.ForeColor = Color.WhiteSmoke;
            enhanceView.Location = new Point(0, 0);
            enhanceView.Name = "enhanceView";
            enhanceView.Size = new Size(400, 363);
            enhanceView.TabIndex = 3;
            enhanceView.Title = "My Page Title";
            // 
            // spacer
            // 
            spacer.Dock = DockStyle.Bottom;
            spacer.Location = new Point(0, 243);
            spacer.Name = "spacer";
            spacer.Size = new Size(400, 10);
            spacer.TabIndex = 83;
            // 
            // pnlEnhActive
            // 
            pnlEnhActive.BackColor = Color.FromArgb(150, 0, 0, 0);
            pnlEnhActive.Dock = DockStyle.Bottom;
            pnlEnhActive.Location = new Point(0, 253);
            pnlEnhActive.Name = "pnlEnhActive";
            pnlEnhActive.Size = new Size(400, 50);
            pnlEnhActive.TabIndex = 84;
            // 
            // spacer2
            // 
            spacer2.Dock = DockStyle.Bottom;
            spacer2.Location = new Point(0, 303);
            spacer2.Name = "spacer2";
            spacer2.Size = new Size(400, 10);
            spacer2.TabIndex = 85;
            // 
            // pnlEnhInactive
            // 
            pnlEnhInactive.BackColor = Color.FromArgb(150, 0, 0, 0);
            pnlEnhInactive.Dock = DockStyle.Bottom;
            pnlEnhInactive.Location = new Point(0, 313);
            pnlEnhInactive.Name = "pnlEnhInactive";
            pnlEnhInactive.Size = new Size(400, 50);
            pnlEnhInactive.TabIndex = 0;
            // 
            // enhDataList
            // 
            enhDataList.BackColor = Color.FromArgb(150, 26, 28, 36);
            enhDataList.Dock = DockStyle.Fill;
            enhDataList.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            enhDataList.HighlightColor = Color.CornflowerBlue;
            enhDataList.HighlightTextColor = Color.Black;
            enhDataList.ItemColor = Color.Silver;
            enhDataList.Location = new Point(0, 20);
            enhDataList.Name = "enhDataList";
            enhDataList.SampleRowsPerColumn = 10;
            enhDataList.SetItemsBold = false;
            enhDataList.ShowRuntimeSamples = true;
            enhDataList.Size = new Size(400, 343);
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
            enhanceSubtitlePanel.Size = new Size(400, 20);
            enhanceSubtitlePanel.TabIndex = 5;
            // 
            // subTitle
            // 
            subTitle.BackColor = Color.Transparent;
            subTitle.Dock = DockStyle.Fill;
            subTitle.FlatStyle = FlatStyle.Flat;
            subTitle.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            subTitle.Location = new Point(0, 0);
            subTitle.Name = "subTitle";
            subTitle.Size = new Size(400, 20);
            subTitle.TabIndex = 0;
            subTitle.Text = "Subtitle";
            subTitle.TextAlign = ContentAlignment.TopCenter;
            // 
            // MidsDataView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(26, 28, 36);
            Controls.Add(dvPages);
            Controls.Add(titlePanel);
            Controls.Add(headerPanel);
            Font = new Font("Segoe UI", 9F);
            Name = "MidsDataView";
            Size = new Size(400, 409);
            Resize += MidsDataView_Resize;
            headerPanel.ResumeLayout(false);
            titlePanel.ResumeLayout(false);
            dvPages.ResumeLayout(false);
            infoView.ResumeLayout(false);
            infoLayoutPanel.ResumeLayout(false);
            effectView.ResumeLayout(false);
            effectViewScrollPanel.ContentPanel.ResumeLayout(false);
            effectViewScrollPanel.ContentPanel.PerformLayout();
            effectViewScrollPanel.ResumeLayout(false);
            totalView.ResumeLayout(false);
            totalViewScrollPanel.ContentPanel.ResumeLayout(false);
            totalViewScrollPanel.ContentPanel.PerformLayout();
            totalViewScrollPanel.ResumeLayout(false);
            totalsResistPanel.ResumeLayout(false);
            totalsDefensePanel.ResumeLayout(false);
            enhanceView.ResumeLayout(false);
            enhanceSubtitlePanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private FontAwesome.Sharp.IconButton DockButton;
        private FontAwesome.Sharp.IconButton LockButton;
        private Panel titlePanel;
        private Label title;
        private FormPages dvPages;
        private Page infoView;
        private Page effectView;
        private Page totalView;
        private Page enhanceView;
        private TableLayoutPanel infoLayoutPanel;
        private PairedListEx infoDataList;
        internal MidsRichTextView infoLDesc;
        private RichTextBox infoSDesc;
        private CtlMultiGraph powerScaler;
        private Label damageHeader;
        private MidsVScrollPanel effectViewScrollPanel;
        private PairedListEx effectDataList3;
        private Label effectsHeader3;
        private PairedListEx effectDataList2;
        private Label effectsHeader2;
        private PairedListEx effectDataList1;
        private Label effectsHeader1;
        private MidsVScrollPanel totalViewScrollPanel;
        private Label totalsHeader1;
        private Panel totalsDefensePanel;
        private CtlMultiGraph defenseGraph2;
        private CtlMultiGraph defenseGraph1;
        private Panel totalsResistPanel;
        private Label totalsHeader2;
        private PairedListEx coreDataList;
        private Label totalsHeader3;
        private CtlMultiGraph resistGraph2;
        private CtlMultiGraph resistGraph1;
        private Label totalsFooter;
        private PairedListEx enhDataList;
        private Panel enhanceSubtitlePanel;
        private Label subTitle;
        private Panel spacer;
        private Panel pnlEnhActive;
        private Panel spacer2;
        private Panel pnlEnhInactive;
        private ToolTip dvToolTip;
        internal ModernDamageDisplay infoDamageDisplay;
    }
}

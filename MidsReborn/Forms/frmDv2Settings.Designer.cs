namespace Mids_Reborn.Forms
{
    partial class frmDv2Settings
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
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabPageColors = new System.Windows.Forms.TabControl();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.checkEndDetail = new System.Windows.Forms.CheckBox();
            this.checkIgnoreConst = new System.Windows.Forms.CheckBox();
            this.numAnimSpeed = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageFonts = new System.Windows.Forms.TabPage();
            this.numGraphRefFontSize = new System.Windows.Forms.NumericUpDown();
            this.numGraphValueFontSize = new System.Windows.Forms.NumericUpDown();
            this.numTotalsValueFontSize = new System.Windows.Forms.NumericUpDown();
            this.numTotalsBarHeight = new System.Windows.Forms.NumericUpDown();
            this.numGridItemFontSize = new System.Windows.Forms.NumericUpDown();
            this.numDmgFontSize = new System.Windows.Forms.NumericUpDown();
            this.numInfoPanelFontSize = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.cbThemeAlignmentOverride = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.checkAutoSwitchAlignmentTheme = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cbColorTheme = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tabPageColors.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAnimSpeed)).BeginInit();
            this.tabPageFonts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGraphRefFontSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGraphValueFontSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTotalsValueFontSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTotalsBarHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGridItemFontSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDmgFontSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInfoPanelFontSize)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageColors
            // 
            this.tabPageColors.Controls.Add(this.tabPageSettings);
            this.tabPageColors.Controls.Add(this.tabPageFonts);
            this.tabPageColors.Controls.Add(this.tabPage3);
            this.tabPageColors.Location = new System.Drawing.Point(0, 0);
            this.tabPageColors.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageColors.Name = "tabPageColors";
            this.tabPageColors.Padding = new System.Drawing.Point(0, 0);
            this.tabPageColors.SelectedIndex = 0;
            this.tabPageColors.Size = new System.Drawing.Size(311, 296);
            this.tabPageColors.TabIndex = 0;
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.checkEndDetail);
            this.tabPageSettings.Controls.Add(this.checkIgnoreConst);
            this.tabPageSettings.Controls.Add(this.numAnimSpeed);
            this.tabPageSettings.Controls.Add(this.label4);
            this.tabPageSettings.Controls.Add(this.label3);
            this.tabPageSettings.Controls.Add(this.label2);
            this.tabPageSettings.Controls.Add(this.label1);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(303, 270);
            this.tabPageSettings.TabIndex = 0;
            this.tabPageSettings.Text = "Settings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // checkEndDetail
            // 
            this.checkEndDetail.AutoSize = true;
            this.checkEndDetail.Location = new System.Drawing.Point(259, 47);
            this.checkEndDetail.Name = "checkEndDetail";
            this.checkEndDetail.Size = new System.Drawing.Size(15, 14);
            this.checkEndDetail.TabIndex = 10;
            this.checkEndDetail.UseMnemonic = false;
            this.checkEndDetail.UseVisualStyleBackColor = true;
            this.checkEndDetail.CheckedChanged += new System.EventHandler(this.checkEndDetail_CheckedChanged);
            // 
            // checkIgnoreConst
            // 
            this.checkIgnoreConst.AutoSize = true;
            this.checkIgnoreConst.Location = new System.Drawing.Point(259, 128);
            this.checkIgnoreConst.Name = "checkIgnoreConst";
            this.checkIgnoreConst.Size = new System.Drawing.Size(15, 14);
            this.checkIgnoreConst.TabIndex = 9;
            this.checkIgnoreConst.UseMnemonic = false;
            this.checkIgnoreConst.UseVisualStyleBackColor = true;
            this.checkIgnoreConst.CheckedChanged += new System.EventHandler(this.checkIgnoreConst_CheckedChanged);
            // 
            // numAnimSpeed
            // 
            this.numAnimSpeed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numAnimSpeed.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numAnimSpeed.Location = new System.Drawing.Point(232, 82);
            this.numAnimSpeed.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numAnimSpeed.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numAnimSpeed.Name = "numAnimSpeed";
            this.numAnimSpeed.Size = new System.Drawing.Size(65, 20);
            this.numAnimSpeed.TabIndex = 8;
            this.numAnimSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numAnimSpeed.ThousandsSeparator = true;
            this.numAnimSpeed.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numAnimSpeed.ValueChanged += new System.EventHandler(this.numAnimSpeed_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 26);
            this.label4.TabIndex = 3;
            this.label4.Text = "Ignore non-variable values\r\n(Scales graph)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Flip animation speed (ms per tick)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Show endurance/recovery detailed info";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Damage Graph format";
            // 
            // tabPageFonts
            // 
            this.tabPageFonts.Controls.Add(this.numGraphRefFontSize);
            this.tabPageFonts.Controls.Add(this.numGraphValueFontSize);
            this.tabPageFonts.Controls.Add(this.numTotalsValueFontSize);
            this.tabPageFonts.Controls.Add(this.numTotalsBarHeight);
            this.tabPageFonts.Controls.Add(this.numGridItemFontSize);
            this.tabPageFonts.Controls.Add(this.numDmgFontSize);
            this.tabPageFonts.Controls.Add(this.numInfoPanelFontSize);
            this.tabPageFonts.Controls.Add(this.label11);
            this.tabPageFonts.Controls.Add(this.label10);
            this.tabPageFonts.Controls.Add(this.label9);
            this.tabPageFonts.Controls.Add(this.label8);
            this.tabPageFonts.Controls.Add(this.label7);
            this.tabPageFonts.Controls.Add(this.label6);
            this.tabPageFonts.Controls.Add(this.label5);
            this.tabPageFonts.Location = new System.Drawing.Point(4, 22);
            this.tabPageFonts.Name = "tabPageFonts";
            this.tabPageFonts.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFonts.Size = new System.Drawing.Size(303, 270);
            this.tabPageFonts.TabIndex = 1;
            this.tabPageFonts.Text = "Fonts";
            this.tabPageFonts.UseVisualStyleBackColor = true;
            // 
            // numGraphRefFontSize
            // 
            this.numGraphRefFontSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numGraphRefFontSize.DecimalPlaces = 2;
            this.numGraphRefFontSize.Location = new System.Drawing.Point(224, 226);
            this.numGraphRefFontSize.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numGraphRefFontSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numGraphRefFontSize.Name = "numGraphRefFontSize";
            this.numGraphRefFontSize.Size = new System.Drawing.Size(65, 20);
            this.numGraphRefFontSize.TabIndex = 13;
            this.numGraphRefFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numGraphRefFontSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numGraphRefFontSize.ValueChanged += new System.EventHandler(this.numGraphRefFontSize_ValueChanged);
            // 
            // numGraphValueFontSize
            // 
            this.numGraphValueFontSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numGraphValueFontSize.DecimalPlaces = 2;
            this.numGraphValueFontSize.Location = new System.Drawing.Point(224, 190);
            this.numGraphValueFontSize.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numGraphValueFontSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numGraphValueFontSize.Name = "numGraphValueFontSize";
            this.numGraphValueFontSize.Size = new System.Drawing.Size(65, 20);
            this.numGraphValueFontSize.TabIndex = 12;
            this.numGraphValueFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numGraphValueFontSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numGraphValueFontSize.ValueChanged += new System.EventHandler(this.numGraphValueFontSize_ValueChanged);
            // 
            // numTotalsValueFontSize
            // 
            this.numTotalsValueFontSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numTotalsValueFontSize.DecimalPlaces = 2;
            this.numTotalsValueFontSize.Location = new System.Drawing.Point(224, 154);
            this.numTotalsValueFontSize.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numTotalsValueFontSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numTotalsValueFontSize.Name = "numTotalsValueFontSize";
            this.numTotalsValueFontSize.Size = new System.Drawing.Size(65, 20);
            this.numTotalsValueFontSize.TabIndex = 11;
            this.numTotalsValueFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTotalsValueFontSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numTotalsValueFontSize.ValueChanged += new System.EventHandler(this.numTotalsValueFontSize_ValueChanged);
            // 
            // numTotalsBarHeight
            // 
            this.numTotalsBarHeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numTotalsBarHeight.DecimalPlaces = 2;
            this.numTotalsBarHeight.Location = new System.Drawing.Point(224, 118);
            this.numTotalsBarHeight.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numTotalsBarHeight.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numTotalsBarHeight.Name = "numTotalsBarHeight";
            this.numTotalsBarHeight.Size = new System.Drawing.Size(65, 20);
            this.numTotalsBarHeight.TabIndex = 10;
            this.numTotalsBarHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTotalsBarHeight.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numTotalsBarHeight.ValueChanged += new System.EventHandler(this.numTotalsBarHeight_ValueChanged);
            // 
            // numGridItemFontSize
            // 
            this.numGridItemFontSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numGridItemFontSize.DecimalPlaces = 2;
            this.numGridItemFontSize.Location = new System.Drawing.Point(224, 82);
            this.numGridItemFontSize.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numGridItemFontSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numGridItemFontSize.Name = "numGridItemFontSize";
            this.numGridItemFontSize.Size = new System.Drawing.Size(65, 20);
            this.numGridItemFontSize.TabIndex = 9;
            this.numGridItemFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numGridItemFontSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numGridItemFontSize.ValueChanged += new System.EventHandler(this.numGridItemFontSize_ValueChanged);
            // 
            // numDmgFontSize
            // 
            this.numDmgFontSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numDmgFontSize.DecimalPlaces = 2;
            this.numDmgFontSize.Location = new System.Drawing.Point(224, 46);
            this.numDmgFontSize.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numDmgFontSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numDmgFontSize.Name = "numDmgFontSize";
            this.numDmgFontSize.Size = new System.Drawing.Size(65, 20);
            this.numDmgFontSize.TabIndex = 8;
            this.numDmgFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numDmgFontSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numDmgFontSize.ValueChanged += new System.EventHandler(this.numDmgFontSize_ValueChanged);
            // 
            // numInfoPanelFontSize
            // 
            this.numInfoPanelFontSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numInfoPanelFontSize.DecimalPlaces = 2;
            this.numInfoPanelFontSize.Location = new System.Drawing.Point(224, 10);
            this.numInfoPanelFontSize.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numInfoPanelFontSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numInfoPanelFontSize.Name = "numInfoPanelFontSize";
            this.numInfoPanelFontSize.Size = new System.Drawing.Size(65, 20);
            this.numInfoPanelFontSize.TabIndex = 7;
            this.numInfoPanelFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numInfoPanelFontSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numInfoPanelFontSize.ValueChanged += new System.EventHandler(this.numInfoPanelFontSize_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 228);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(125, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "Graph ref. sheet font size";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 192);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(112, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Graph values font size";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 156);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(125, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Totals bar value font size";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 120);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Totals bar height (px)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 84);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Grid view item font size";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Damage graph font size";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Info panel font size";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cbThemeAlignmentOverride);
            this.tabPage3.Controls.Add(this.label14);
            this.tabPage3.Controls.Add(this.checkAutoSwitchAlignmentTheme);
            this.tabPage3.Controls.Add(this.label13);
            this.tabPage3.Controls.Add(this.cbColorTheme);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(303, 270);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Colors";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // cbThemeAlignmentOverride
            // 
            this.cbThemeAlignmentOverride.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbThemeAlignmentOverride.FormattingEnabled = true;
            this.cbThemeAlignmentOverride.Items.AddRange(new object[] {
            "Hero Theme",
            "Villain Theme"});
            this.cbThemeAlignmentOverride.Location = new System.Drawing.Point(164, 82);
            this.cbThemeAlignmentOverride.Name = "cbThemeAlignmentOverride";
            this.cbThemeAlignmentOverride.Size = new System.Drawing.Size(121, 21);
            this.cbThemeAlignmentOverride.TabIndex = 5;
            this.cbThemeAlignmentOverride.SelectedIndexChanged += new System.EventHandler(this.cbThemeAlignmentOverride_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(16, 84);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(43, 13);
            this.label14.TabIndex = 4;
            this.label14.Text = "Stick to";
            // 
            // checkAutoSwitchAlignmentTheme
            // 
            this.checkAutoSwitchAlignmentTheme.AutoSize = true;
            this.checkAutoSwitchAlignmentTheme.Location = new System.Drawing.Point(259, 47);
            this.checkAutoSwitchAlignmentTheme.Name = "checkAutoSwitchAlignmentTheme";
            this.checkAutoSwitchAlignmentTheme.Size = new System.Drawing.Size(15, 14);
            this.checkAutoSwitchAlignmentTheme.TabIndex = 3;
            this.checkAutoSwitchAlignmentTheme.UseMnemonic = false;
            this.checkAutoSwitchAlignmentTheme.UseVisualStyleBackColor = true;
            this.checkAutoSwitchAlignmentTheme.CheckedChanged += new System.EventHandler(this.checkAutoSwitchAlignmentTheme_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 48);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(149, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Auto-switch hero/villain theme";
            // 
            // cbColorTheme
            // 
            this.cbColorTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbColorTheme.FormattingEnabled = true;
            this.cbColorTheme.Items.AddRange(new object[] {
            "Dimmed (default)",
            "Old School Shiny",
            "Morally Flexible",
            "Paragon Blue",
            "Granville\'s Red",
            "Nova Gold",
            "Imperial Purple"});
            this.cbColorTheme.Location = new System.Drawing.Point(164, 10);
            this.cbColorTheme.Name = "cbColorTheme";
            this.cbColorTheme.Size = new System.Drawing.Size(125, 21);
            this.cbColorTheme.TabIndex = 1;
            this.cbColorTheme.SelectedIndexChanged += new System.EventHandler(this.cbColorTheme_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 12);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(89, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Tabs color theme";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(172, 307);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(64, 307);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 27);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // frmDv2Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 353);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabPageColors);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDv2Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DataView Options";
            this.Load += new System.EventHandler(this.frmDv2Settings_Load);
            this.tabPageColors.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAnimSpeed)).EndInit();
            this.tabPageFonts.ResumeLayout(false);
            this.tabPageFonts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGraphRefFontSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGraphValueFontSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTotalsValueFontSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTotalsBarHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGridItemFontSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDmgFontSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInfoPanelFontSize)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabPageColors;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.TabPage tabPageFonts;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkEndDetail;
        private System.Windows.Forms.CheckBox checkIgnoreConst;
        private System.Windows.Forms.NumericUpDown numAnimSpeed;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numGraphRefFontSize;
        private System.Windows.Forms.NumericUpDown numGraphValueFontSize;
        private System.Windows.Forms.NumericUpDown numTotalsValueFontSize;
        private System.Windows.Forms.NumericUpDown numTotalsBarHeight;
        private System.Windows.Forms.NumericUpDown numGridItemFontSize;
        private System.Windows.Forms.NumericUpDown numDmgFontSize;
        private System.Windows.Forms.NumericUpDown numInfoPanelFontSize;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbColorTheme;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cbThemeAlignmentOverride;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox checkAutoSwitchAlignmentTheme;
        private System.Windows.Forms.Label label13;
    }
}
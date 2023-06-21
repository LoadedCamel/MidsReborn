namespace Mids_Reborn.Forms.ImportExportItems
{
    partial class frmForumExport
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
            panel1 = new System.Windows.Forms.Panel();
            chkCustomThemeDark = new System.Windows.Forms.CheckBox();
            chkOptLongFormat = new System.Windows.Forms.CheckBox();
            chkOptIncarnates = new System.Windows.Forms.CheckBox();
            chkOptAccolades = new System.Windows.Forms.CheckBox();
            label7 = new System.Windows.Forms.Label();
            btnCancel = new Controls.ImageButtonEx();
            btnExport = new Controls.ImageButtonEx();
            lbFormatCodeType = new System.Windows.Forms.ListBox();
            label6 = new System.Windows.Forms.Label();
            rbAllThemes = new System.Windows.Forms.RadioButton();
            rbDarkThemes = new System.Windows.Forms.RadioButton();
            rbLightThemes = new System.Windows.Forms.RadioButton();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            panelColorSlots = new System.Windows.Forms.Panel();
            panelColorLevels = new System.Windows.Forms.Panel();
            panelColorHeadings = new System.Windows.Forms.Panel();
            panelColorTitle = new System.Windows.Forms.Panel();
            lbColorTheme = new System.Windows.Forms.ListBox();
            label1 = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(chkCustomThemeDark);
            panel1.Controls.Add(chkOptLongFormat);
            panel1.Controls.Add(chkOptIncarnates);
            panel1.Controls.Add(chkOptAccolades);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnExport);
            panel1.Controls.Add(lbFormatCodeType);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(rbAllThemes);
            panel1.Controls.Add(rbDarkThemes);
            panel1.Controls.Add(rbLightThemes);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(panelColorSlots);
            panel1.Controls.Add(panelColorLevels);
            panel1.Controls.Add(panelColorHeadings);
            panel1.Controls.Add(panelColorTitle);
            panel1.Controls.Add(lbColorTheme);
            panel1.Controls.Add(label1);
            panel1.ForeColor = System.Drawing.Color.WhiteSmoke;
            panel1.Location = new System.Drawing.Point(12, 11);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(776, 400);
            panel1.TabIndex = 0;
            // 
            // chkCustomThemeDark
            // 
            chkCustomThemeDark.AutoSize = true;
            chkCustomThemeDark.Location = new System.Drawing.Point(289, 165);
            chkCustomThemeDark.Name = "chkCustomThemeDark";
            chkCustomThemeDark.Size = new System.Drawing.Size(89, 19);
            chkCustomThemeDark.TabIndex = 22;
            chkCustomThemeDark.Text = "Dark Theme";
            chkCustomThemeDark.UseVisualStyleBackColor = true;
            chkCustomThemeDark.Visible = false;
            chkCustomThemeDark.CheckedChanged += chkCustomThemeDark_CheckedChanged;
            // 
            // chkOptLongFormat
            // 
            chkOptLongFormat.AutoSize = true;
            chkOptLongFormat.Location = new System.Drawing.Point(28, 349);
            chkOptLongFormat.Name = "chkOptLongFormat";
            chkOptLongFormat.Size = new System.Drawing.Size(218, 19);
            chkOptLongFormat.TabIndex = 21;
            chkOptLongFormat.Text = "Include Bonus Totals, Set Breakdown";
            chkOptLongFormat.UseVisualStyleBackColor = true;
            // 
            // chkOptIncarnates
            // 
            chkOptIncarnates.AutoSize = true;
            chkOptIncarnates.Location = new System.Drawing.Point(28, 324);
            chkOptIncarnates.Name = "chkOptIncarnates";
            chkOptIncarnates.Size = new System.Drawing.Size(122, 19);
            chkOptIncarnates.TabIndex = 19;
            chkOptIncarnates.Text = "Include Incarnates";
            chkOptIncarnates.UseVisualStyleBackColor = true;
            // 
            // chkOptAccolades
            // 
            chkOptAccolades.AutoSize = true;
            chkOptAccolades.Location = new System.Drawing.Point(28, 300);
            chkOptAccolades.Name = "chkOptAccolades";
            chkOptAccolades.Size = new System.Drawing.Size(122, 19);
            chkOptAccolades.TabIndex = 18;
            chkOptAccolades.Text = "Include Accolades";
            chkOptAccolades.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label7.Location = new System.Drawing.Point(28, 271);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(91, 16);
            label7.TabIndex = 17;
            label7.Text = "Export options:";
            // 
            // btnCancel
            // 
            btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnCancel.CurrentText = "Cancel";
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnCancel.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnCancel.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnCancel.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnCancel.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnCancel.Location = new System.Drawing.Point(622, 342);
            btnCancel.Lock = false;
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(100, 28);
            btnCancel.TabIndex = 16;
            btnCancel.Text = "Cancel";
            btnCancel.TextOutline.Color = System.Drawing.Color.Black;
            btnCancel.TextOutline.Width = 2;
            btnCancel.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            btnCancel.ToggleText.Indeterminate = "Indeterminate State";
            btnCancel.ToggleText.ToggledOff = "ToggledOff State";
            btnCancel.ToggleText.ToggledOn = "ToggledOn State";
            btnCancel.UseAlt = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnExport
            // 
            btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnExport.CurrentText = "Export";
            btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnExport.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnExport.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnExport.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnExport.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnExport.Location = new System.Drawing.Point(479, 342);
            btnExport.Lock = false;
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(100, 28);
            btnExport.TabIndex = 15;
            btnExport.Text = "Export";
            btnExport.TextOutline.Color = System.Drawing.Color.Black;
            btnExport.TextOutline.Width = 2;
            btnExport.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            btnExport.ToggleText.Indeterminate = "Indeterminate State";
            btnExport.ToggleText.ToggledOff = "ToggledOff State";
            btnExport.ToggleText.ToggledOn = "ToggledOn State";
            btnExport.UseAlt = false;
            btnExport.Click += btnExport_Click;
            // 
            // lbFormatCodeType
            // 
            lbFormatCodeType.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            lbFormatCodeType.ForeColor = System.Drawing.Color.WhiteSmoke;
            lbFormatCodeType.FormattingEnabled = true;
            lbFormatCodeType.ItemHeight = 15;
            lbFormatCodeType.Location = new System.Drawing.Point(440, 42);
            lbFormatCodeType.Name = "lbFormatCodeType";
            lbFormatCodeType.Size = new System.Drawing.Size(314, 199);
            lbFormatCodeType.TabIndex = 14;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(440, 13);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(135, 16);
            label6.TabIndex = 13;
            label6.Text = "Formatting Code Type:";
            // 
            // rbAllThemes
            // 
            rbAllThemes.AutoSize = true;
            rbAllThemes.Location = new System.Drawing.Point(263, 261);
            rbAllThemes.Name = "rbAllThemes";
            rbAllThemes.Size = new System.Drawing.Size(46, 19);
            rbAllThemes.TabIndex = 12;
            rbAllThemes.TabStop = true;
            rbAllThemes.Text = "Any";
            rbAllThemes.UseVisualStyleBackColor = true;
            rbAllThemes.CheckedChanged += rbAllThemes_CheckedChanged;
            // 
            // rbDarkThemes
            // 
            rbDarkThemes.AutoSize = true;
            rbDarkThemes.Location = new System.Drawing.Point(263, 236);
            rbDarkThemes.Name = "rbDarkThemes";
            rbDarkThemes.Size = new System.Drawing.Size(93, 19);
            rbDarkThemes.TabIndex = 11;
            rbDarkThemes.TabStop = true;
            rbDarkThemes.Text = "Dark Themes";
            rbDarkThemes.UseVisualStyleBackColor = true;
            rbDarkThemes.CheckedChanged += rbDarkThemes_CheckedChanged;
            // 
            // rbLightThemes
            // 
            rbLightThemes.AutoSize = true;
            rbLightThemes.Location = new System.Drawing.Point(263, 212);
            rbLightThemes.Name = "rbLightThemes";
            rbLightThemes.Size = new System.Drawing.Size(96, 19);
            rbLightThemes.TabIndex = 10;
            rbLightThemes.TabStop = true;
            rbLightThemes.Text = "Light Themes";
            rbLightThemes.UseVisualStyleBackColor = true;
            rbLightThemes.CheckedChanged += rbLightThemes_CheckedChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(256, 45);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(56, 15);
            label5.TabIndex = 9;
            label5.Text = "Title Text:";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(277, 129);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(35, 15);
            label4.TabIndex = 8;
            label4.Text = "Slots:";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(270, 101);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(42, 15);
            label3.TabIndex = 7;
            label3.Text = "Levels:";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(252, 73);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(60, 15);
            label2.TabIndex = 6;
            label2.Text = "Headings:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelColorSlots
            // 
            panelColorSlots.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelColorSlots.Location = new System.Drawing.Point(318, 127);
            panelColorSlots.Name = "panelColorSlots";
            panelColorSlots.Size = new System.Drawing.Size(60, 23);
            panelColorSlots.TabIndex = 5;
            panelColorSlots.Click += panelColorSlots_Click;
            // 
            // panelColorLevels
            // 
            panelColorLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelColorLevels.Location = new System.Drawing.Point(318, 98);
            panelColorLevels.Name = "panelColorLevels";
            panelColorLevels.Size = new System.Drawing.Size(60, 23);
            panelColorLevels.TabIndex = 4;
            panelColorLevels.Click += panelColorLevels_Click;
            // 
            // panelColorHeadings
            // 
            panelColorHeadings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelColorHeadings.Location = new System.Drawing.Point(318, 70);
            panelColorHeadings.Name = "panelColorHeadings";
            panelColorHeadings.Size = new System.Drawing.Size(60, 23);
            panelColorHeadings.TabIndex = 3;
            panelColorHeadings.Click += panelColorHeadings_Click;
            // 
            // panelColorTitle
            // 
            panelColorTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelColorTitle.Location = new System.Drawing.Point(318, 42);
            panelColorTitle.Name = "panelColorTitle";
            panelColorTitle.Size = new System.Drawing.Size(60, 23);
            panelColorTitle.TabIndex = 2;
            panelColorTitle.Click += panelColorTitle_Click;
            // 
            // lbColorTheme
            // 
            lbColorTheme.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            lbColorTheme.ForeColor = System.Drawing.Color.WhiteSmoke;
            lbColorTheme.FormattingEnabled = true;
            lbColorTheme.ItemHeight = 15;
            lbColorTheme.Location = new System.Drawing.Point(13, 42);
            lbColorTheme.Name = "lbColorTheme";
            lbColorTheme.Size = new System.Drawing.Size(199, 199);
            lbColorTheme.TabIndex = 1;
            lbColorTheme.SelectedIndexChanged += lbColorTheme_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(13, 13);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(83, 16);
            label1.TabIndex = 0;
            label1.Text = "Color Theme:";
            // 
            // frmForumExport
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(800, 422);
            Controls.Add(panel1);
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "frmForumExport";
            ShowInTaskbar = false;
            Text = "Form1";
            Load += frmForumExport_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelColorSlots;
        private System.Windows.Forms.Panel panelColorLevels;
        private System.Windows.Forms.Panel panelColorHeadings;
        private System.Windows.Forms.Panel panelColorTitle;
        private System.Windows.Forms.ListBox lbColorTheme;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbAllThemes;
        private System.Windows.Forms.RadioButton rbDarkThemes;
        private System.Windows.Forms.RadioButton rbLightThemes;
        private System.Windows.Forms.ListBox lbFormatCodeType;
        private System.Windows.Forms.Label label6;
        private Controls.ImageButtonEx btnCancel;
        private Controls.ImageButtonEx btnExport;
        private System.Windows.Forms.CheckBox chkOptSetBreakdown;
        private System.Windows.Forms.CheckBox chkOptLongFormat;
        private System.Windows.Forms.CheckBox chkOptIncarnates;
        private System.Windows.Forms.CheckBox chkOptAccolades;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkCustomThemeDark;
    }
}
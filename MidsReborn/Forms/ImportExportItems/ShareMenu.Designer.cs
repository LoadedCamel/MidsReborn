using MetaControls;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.ImportExportItems
{
    partial class ShareMenu
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
            formPages1 = new FormPages();
            page1 = new Page();
            label9 = new System.Windows.Forms.Label();
            cbHtmlIncludeExtras = new System.Windows.Forms.CheckBox();
            cbHtmlIncludeInc = new System.Windows.Forms.CheckBox();
            cbHtmlIncludeAcc = new System.Windows.Forms.CheckBox();
            label8 = new System.Windows.Forms.Label();
            ibExCancelHtml = new Controls.ImageButtonEx();
            ibExExportHtml = new Controls.ImageButtonEx();
            page2 = new Page();
            label11 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
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
            navStrip1 = new NavStrip();
            formPages1.SuspendLayout();
            page1.SuspendLayout();
            page2.SuspendLayout();
            SuspendLayout();
            // 
            // formPages1
            // 
            formPages1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            formPages1.Controls.Add(page1);
            formPages1.Controls.Add(page2);
            formPages1.Location = new System.Drawing.Point(12, 62);
            formPages1.Name = "formPages1";
            formPages1.Pages.Add(page1);
            formPages1.Pages.Add(page2);
            formPages1.SelectedIndex = 1;
            formPages1.Size = new System.Drawing.Size(776, 393);
            formPages1.TabIndex = 45;
            // 
            // page1
            // 
            page1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page1.Anchor = System.Windows.Forms.AnchorStyles.None;
            page1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page1.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            page1.Controls.Add(label9);
            page1.Controls.Add(cbHtmlIncludeExtras);
            page1.Controls.Add(cbHtmlIncludeInc);
            page1.Controls.Add(cbHtmlIncludeAcc);
            page1.Controls.Add(label8);
            page1.Controls.Add(ibExCancelHtml);
            page1.Controls.Add(ibExExportHtml);
            page1.Dock = System.Windows.Forms.DockStyle.Fill;
            page1.ForeColor = System.Drawing.Color.WhiteSmoke;
            page1.Location = new System.Drawing.Point(0, 0);
            page1.Name = "page1";
            page1.Size = new System.Drawing.Size(774, 391);
            page1.TabIndex = 0;
            page1.Title = "Mobile Friendly (Link)";
            // 
            // label9
            // 
            label9.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            label9.Location = new System.Drawing.Point(10, 12);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(758, 51);
            label9.TabIndex = 50;
            label9.Text = "Generates a link from character data with optional extras that are exported to a multi-column card to summarize build statistics.\r\nThe output will be copied into the clipboard.";
            // 
            // cbHtmlIncludeExtras
            // 
            cbHtmlIncludeExtras.AutoSize = true;
            cbHtmlIncludeExtras.Location = new System.Drawing.Point(20, 188);
            cbHtmlIncludeExtras.Name = "cbHtmlIncludeExtras";
            cbHtmlIncludeExtras.Size = new System.Drawing.Size(218, 19);
            cbHtmlIncludeExtras.TabIndex = 49;
            cbHtmlIncludeExtras.Text = "Include Bonus Totals, Set Breakdown";
            cbHtmlIncludeExtras.UseVisualStyleBackColor = true;
            cbHtmlIncludeExtras.CheckedChanged += cbHtmlIncludeExtras_CheckedChanged;
            // 
            // cbHtmlIncludeInc
            // 
            cbHtmlIncludeInc.AutoSize = true;
            cbHtmlIncludeInc.Location = new System.Drawing.Point(20, 163);
            cbHtmlIncludeInc.Name = "cbHtmlIncludeInc";
            cbHtmlIncludeInc.Size = new System.Drawing.Size(122, 19);
            cbHtmlIncludeInc.TabIndex = 48;
            cbHtmlIncludeInc.Text = "Include Incarnates";
            cbHtmlIncludeInc.UseVisualStyleBackColor = true;
            cbHtmlIncludeInc.CheckedChanged += cbHtmlIncludeInc_CheckedChanged;
            // 
            // cbHtmlIncludeAcc
            // 
            cbHtmlIncludeAcc.AutoSize = true;
            cbHtmlIncludeAcc.Location = new System.Drawing.Point(20, 139);
            cbHtmlIncludeAcc.Name = "cbHtmlIncludeAcc";
            cbHtmlIncludeAcc.Size = new System.Drawing.Size(122, 19);
            cbHtmlIncludeAcc.TabIndex = 47;
            cbHtmlIncludeAcc.Text = "Include Accolades";
            cbHtmlIncludeAcc.UseVisualStyleBackColor = true;
            cbHtmlIncludeAcc.CheckedChanged += cbHtmlIncludeAcc_CheckedChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Segoe UI Variable Display", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label8.Location = new System.Drawing.Point(20, 110);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(99, 19);
            label8.TabIndex = 46;
            label8.Text = "Extra options:";
            // 
            // ibExCancelHtml
            // 
            ibExCancelHtml.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ibExCancelHtml.CurrentText = "Cancel";
            ibExCancelHtml.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ibExCancelHtml.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibExCancelHtml.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibExCancelHtml.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibExCancelHtml.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibExCancelHtml.Location = new System.Drawing.Point(634, 312);
            ibExCancelHtml.Lock = false;
            ibExCancelHtml.Name = "ibExCancelHtml";
            ibExCancelHtml.Size = new System.Drawing.Size(100, 28);
            ibExCancelHtml.TabIndex = 45;
            ibExCancelHtml.Text = "Cancel";
            ibExCancelHtml.TextOutline.Color = System.Drawing.Color.Black;
            ibExCancelHtml.TextOutline.Width = 2;
            ibExCancelHtml.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            ibExCancelHtml.ToggleText.Indeterminate = "Indeterminate State";
            ibExCancelHtml.ToggleText.ToggledOff = "ToggledOff State";
            ibExCancelHtml.ToggleText.ToggledOn = "ToggledOn State";
            ibExCancelHtml.UseAlt = false;
            ibExCancelHtml.Click += btnCancel_Click;
            // 
            // ibExExportHtml
            // 
            ibExExportHtml.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ibExExportHtml.CurrentText = "Generate";
            ibExExportHtml.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ibExExportHtml.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibExExportHtml.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibExExportHtml.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibExExportHtml.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibExExportHtml.Location = new System.Drawing.Point(491, 312);
            ibExExportHtml.Lock = false;
            ibExExportHtml.Name = "ibExExportHtml";
            ibExExportHtml.Size = new System.Drawing.Size(100, 28);
            ibExExportHtml.TabIndex = 44;
            ibExExportHtml.Text = "Generate";
            ibExExportHtml.TextOutline.Color = System.Drawing.Color.Black;
            ibExExportHtml.TextOutline.Width = 2;
            ibExExportHtml.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            ibExExportHtml.ToggleText.Indeterminate = "Indeterminate State";
            ibExExportHtml.ToggleText.ToggledOff = "ToggledOff State";
            ibExExportHtml.ToggleText.ToggledOn = "ToggledOn State";
            ibExExportHtml.UseAlt = false;
            ibExExportHtml.Click += ibExExportHtml_Click;
            // 
            // page2
            // 
            page2.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page2.Anchor = System.Windows.Forms.AnchorStyles.None;
            page2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page2.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            page2.Controls.Add(label11);
            page2.Controls.Add(label10);
            page2.Controls.Add(chkCustomThemeDark);
            page2.Controls.Add(chkOptLongFormat);
            page2.Controls.Add(chkOptIncarnates);
            page2.Controls.Add(chkOptAccolades);
            page2.Controls.Add(label7);
            page2.Controls.Add(btnCancel);
            page2.Controls.Add(btnExport);
            page2.Controls.Add(lbFormatCodeType);
            page2.Controls.Add(label6);
            page2.Controls.Add(rbAllThemes);
            page2.Controls.Add(rbDarkThemes);
            page2.Controls.Add(rbLightThemes);
            page2.Controls.Add(label5);
            page2.Controls.Add(label4);
            page2.Controls.Add(label3);
            page2.Controls.Add(label2);
            page2.Controls.Add(panelColorSlots);
            page2.Controls.Add(panelColorLevels);
            page2.Controls.Add(panelColorHeadings);
            page2.Controls.Add(panelColorTitle);
            page2.Controls.Add(lbColorTheme);
            page2.Controls.Add(label1);
            page2.Dock = System.Windows.Forms.DockStyle.Fill;
            page2.ForeColor = System.Drawing.Color.WhiteSmoke;
            page2.Location = new System.Drawing.Point(0, 0);
            page2.Name = "page2";
            page2.Size = new System.Drawing.Size(774, 391);
            page2.TabIndex = 1;
            page2.Title = "Forum Formats (Text)";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label11.Location = new System.Drawing.Point(293, 254);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(80, 15);
            label11.TabIndex = 68;
            label11.Text = "Theme filter:";
            // 
            // label10
            // 
            label10.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            label10.Location = new System.Drawing.Point(10, 12);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(746, 52);
            label10.TabIndex = 67;
            label10.Text = "Exports character data and optional extras to a compatible output format including {formats}.\r\nThe output will be copied into the clipboard.";
            // 
            // chkCustomThemeDark
            // 
            chkCustomThemeDark.AutoSize = true;
            chkCustomThemeDark.Location = new System.Drawing.Point(293, 216);
            chkCustomThemeDark.Name = "chkCustomThemeDark";
            chkCustomThemeDark.Size = new System.Drawing.Size(89, 19);
            chkCustomThemeDark.TabIndex = 66;
            chkCustomThemeDark.Text = "Dark Theme";
            chkCustomThemeDark.UseVisualStyleBackColor = true;
            chkCustomThemeDark.Visible = false;
            chkCustomThemeDark.CheckedChanged += chkCustomThemeDark_CheckedChanged;
            // 
            // chkOptLongFormat
            // 
            chkOptLongFormat.AutoSize = true;
            chkOptLongFormat.Location = new System.Drawing.Point(21, 331);
            chkOptLongFormat.Name = "chkOptLongFormat";
            chkOptLongFormat.Size = new System.Drawing.Size(218, 19);
            chkOptLongFormat.TabIndex = 65;
            chkOptLongFormat.Text = "Include Bonus Totals, Set Breakdown";
            chkOptLongFormat.UseVisualStyleBackColor = true;
            // 
            // chkOptIncarnates
            // 
            chkOptIncarnates.AutoSize = true;
            chkOptIncarnates.Location = new System.Drawing.Point(21, 306);
            chkOptIncarnates.Name = "chkOptIncarnates";
            chkOptIncarnates.Size = new System.Drawing.Size(122, 19);
            chkOptIncarnates.TabIndex = 64;
            chkOptIncarnates.Text = "Include Incarnates";
            chkOptIncarnates.UseVisualStyleBackColor = true;
            // 
            // chkOptAccolades
            // 
            chkOptAccolades.AutoSize = true;
            chkOptAccolades.Location = new System.Drawing.Point(21, 282);
            chkOptAccolades.Name = "chkOptAccolades";
            chkOptAccolades.Size = new System.Drawing.Size(122, 19);
            chkOptAccolades.TabIndex = 63;
            chkOptAccolades.Text = "Include Accolades";
            chkOptAccolades.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Segoe UI Variable Display", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label7.Location = new System.Drawing.Point(21, 253);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(109, 19);
            label7.TabIndex = 62;
            label7.Text = "Export options:";
            // 
            // btnCancel
            // 
            btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnCancel.CurrentText = "Cancel";
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnCancel.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnCancel.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnCancel.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnCancel.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnCancel.Location = new System.Drawing.Point(634, 312);
            btnCancel.Lock = false;
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(100, 28);
            btnCancel.TabIndex = 61;
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
            btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnExport.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnExport.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnExport.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnExport.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnExport.Location = new System.Drawing.Point(491, 312);
            btnExport.Lock = false;
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(100, 28);
            btnExport.TabIndex = 60;
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
            lbFormatCodeType.Location = new System.Drawing.Point(442, 83);
            lbFormatCodeType.Name = "lbFormatCodeType";
            lbFormatCodeType.Size = new System.Drawing.Size(314, 139);
            lbFormatCodeType.TabIndex = 59;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(442, 64);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(135, 16);
            label6.TabIndex = 58;
            label6.Text = "Formatting Code Type:";
            // 
            // rbAllThemes
            // 
            rbAllThemes.AutoSize = true;
            rbAllThemes.Location = new System.Drawing.Point(291, 330);
            rbAllThemes.Name = "rbAllThemes";
            rbAllThemes.Size = new System.Drawing.Size(46, 19);
            rbAllThemes.TabIndex = 57;
            rbAllThemes.TabStop = true;
            rbAllThemes.Text = "Any";
            rbAllThemes.UseVisualStyleBackColor = true;
            rbAllThemes.CheckedChanged += rbAllThemes_CheckedChanged;
            // 
            // rbDarkThemes
            // 
            rbDarkThemes.AutoSize = true;
            rbDarkThemes.Location = new System.Drawing.Point(291, 305);
            rbDarkThemes.Name = "rbDarkThemes";
            rbDarkThemes.Size = new System.Drawing.Size(93, 19);
            rbDarkThemes.TabIndex = 56;
            rbDarkThemes.TabStop = true;
            rbDarkThemes.Text = "Dark Themes";
            rbDarkThemes.UseVisualStyleBackColor = true;
            rbDarkThemes.CheckedChanged += rbDarkThemes_CheckedChanged;
            // 
            // rbLightThemes
            // 
            rbLightThemes.AutoSize = true;
            rbLightThemes.Location = new System.Drawing.Point(291, 281);
            rbLightThemes.Name = "rbLightThemes";
            rbLightThemes.Size = new System.Drawing.Size(96, 19);
            rbLightThemes.TabIndex = 55;
            rbLightThemes.TabStop = true;
            rbLightThemes.Text = "Light Themes";
            rbLightThemes.UseVisualStyleBackColor = true;
            rbLightThemes.CheckedChanged += rbLightThemes_CheckedChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(260, 96);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(56, 15);
            label5.TabIndex = 54;
            label5.Text = "Title Text:";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(281, 180);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(35, 15);
            label4.TabIndex = 53;
            label4.Text = "Slots:";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(274, 152);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(42, 15);
            label3.TabIndex = 52;
            label3.Text = "Levels:";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(256, 124);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(60, 15);
            label2.TabIndex = 51;
            label2.Text = "Headings:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelColorSlots
            // 
            panelColorSlots.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelColorSlots.Location = new System.Drawing.Point(322, 178);
            panelColorSlots.Name = "panelColorSlots";
            panelColorSlots.Size = new System.Drawing.Size(60, 23);
            panelColorSlots.TabIndex = 50;
            panelColorSlots.Click += panelColorSlots_Click;
            // 
            // panelColorLevels
            // 
            panelColorLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelColorLevels.Location = new System.Drawing.Point(322, 149);
            panelColorLevels.Name = "panelColorLevels";
            panelColorLevels.Size = new System.Drawing.Size(60, 23);
            panelColorLevels.TabIndex = 49;
            panelColorLevels.Click += panelColorLevels_Click;
            // 
            // panelColorHeadings
            // 
            panelColorHeadings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelColorHeadings.Location = new System.Drawing.Point(322, 121);
            panelColorHeadings.Name = "panelColorHeadings";
            panelColorHeadings.Size = new System.Drawing.Size(60, 23);
            panelColorHeadings.TabIndex = 48;
            panelColorHeadings.Click += panelColorHeadings_Click;
            // 
            // panelColorTitle
            // 
            panelColorTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelColorTitle.Location = new System.Drawing.Point(322, 93);
            panelColorTitle.Name = "panelColorTitle";
            panelColorTitle.Size = new System.Drawing.Size(60, 23);
            panelColorTitle.TabIndex = 47;
            panelColorTitle.Click += panelColorTitle_Click;
            // 
            // lbColorTheme
            // 
            lbColorTheme.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            lbColorTheme.ForeColor = System.Drawing.Color.WhiteSmoke;
            lbColorTheme.FormattingEnabled = true;
            lbColorTheme.ItemHeight = 15;
            lbColorTheme.Location = new System.Drawing.Point(17, 83);
            lbColorTheme.Name = "lbColorTheme";
            lbColorTheme.Size = new System.Drawing.Size(199, 139);
            lbColorTheme.TabIndex = 46;
            lbColorTheme.SelectedIndexChanged += lbColorTheme_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(17, 64);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(83, 16);
            label1.TabIndex = 45;
            label1.Text = "Color Theme:";
            // 
            // navStrip1
            // 
            navStrip1.ActiveTabColor = System.Drawing.Color.Goldenrod;
            navStrip1.DataSource = formPages1;
            navStrip1.DimmedColor = System.Drawing.Color.FromArgb(21, 61, 93);
            navStrip1.DisabledTabColor = System.Drawing.Color.DarkGray;
            navStrip1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            navStrip1.InactiveTabColor = System.Drawing.Color.FromArgb(30, 85, 130);
            navStrip1.InactiveTabHoverColor = System.Drawing.Color.FromArgb(43, 122, 187);
            navStrip1.Location = new System.Drawing.Point(14, 15);
            navStrip1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            navStrip1.Name = "navStrip1";
            navStrip1.OutlineColor = System.Drawing.Color.Black;
            navStrip1.Size = new System.Drawing.Size(774, 45);
            navStrip1.TabIndex = 46;
            // 
            // ShareMenu
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(802, 467);
            Controls.Add(navStrip1);
            Controls.Add(formPages1);
            DoubleBuffered = true;
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "ShareMenu";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Share Menu";
            Load += frmForumExport_Load;
            formPages1.ResumeLayout(false);
            page1.ResumeLayout(false);
            page1.PerformLayout();
            page2.ResumeLayout(false);
            page2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.CheckBox chkOptSetBreakdown;
        private FormPages formPages1;
        private Page page1;
        private Page page2;
        private ctlTotalsTabStrip tabStrip1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cbHtmlIncludeExtras;
        private System.Windows.Forms.CheckBox cbHtmlIncludeInc;
        private System.Windows.Forms.CheckBox cbHtmlIncludeAcc;
        private System.Windows.Forms.Label label8;
        private Controls.ImageButtonEx ibExCancelHtml;
        private Controls.ImageButtonEx ibExExportHtml;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkCustomThemeDark;
        private System.Windows.Forms.CheckBox chkOptLongFormat;
        private System.Windows.Forms.CheckBox chkOptIncarnates;
        private System.Windows.Forms.CheckBox chkOptAccolades;
        private System.Windows.Forms.Label label7;
        private Controls.ImageButtonEx btnCancel;
        private Controls.ImageButtonEx btnExport;
        private System.Windows.Forms.ListBox lbFormatCodeType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbAllThemes;
        private System.Windows.Forms.RadioButton rbDarkThemes;
        private System.Windows.Forms.RadioButton rbLightThemes;
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
        private System.Windows.Forms.Label label11;
        private NavStrip navStrip1;
    }
}
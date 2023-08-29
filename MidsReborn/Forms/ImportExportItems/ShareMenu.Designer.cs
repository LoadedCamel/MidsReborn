using MetaControls;
using Mids_Reborn.Controls;
using Mids_Reborn.Forms.Controls;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShareMenu));
            formPages1 = new FormPages();
            buildData = new Page();
            bdCancel = new ImageButtonEx();
            bdExport = new ImageButtonEx();
            label12 = new System.Windows.Forms.Label();
            bdChunkBox = new System.Windows.Forms.TextBox();
            forumFormats = new Page();
            resetThemeBtn = new System.Windows.Forms.Button();
            remThemeBtn = new System.Windows.Forms.Button();
            addThemeBtn = new System.Windows.Forms.Button();
            label11 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            chkCustomThemeDark = new System.Windows.Forms.CheckBox();
            chkOptLongFormat = new System.Windows.Forms.CheckBox();
            chkOptIncarnates = new System.Windows.Forms.CheckBox();
            chkOptAccolades = new System.Windows.Forms.CheckBox();
            label7 = new System.Windows.Forms.Label();
            ffCancel = new ImageButtonEx();
            ffExport = new ImageButtonEx();
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
            lbColorTheme = new FilterableListBox();
            label1 = new System.Windows.Forms.Label();
            infoGraphic = new Page();
            chkUseAltIg = new System.Windows.Forms.CheckBox();
            igCancel = new ImageButtonEx();
            igExport = new ImageButtonEx();
            label13 = new System.Windows.Forms.Label();
            igPictureBox = new System.Windows.Forms.PictureBox();
            mobileFriendly = new Page();
            label14 = new System.Windows.Forms.Label();
            borderPanel1 = new BorderPanel();
            previewLabel = new System.Windows.Forms.Label();
            webViewPreview = new Microsoft.Web.WebView2.WinForms.WebView2();
            cbInclSetBreakdown = new System.Windows.Forms.CheckBox();
            label9 = new System.Windows.Forms.Label();
            cbInclSetBonus = new System.Windows.Forms.CheckBox();
            cbInclIncarnate = new System.Windows.Forms.CheckBox();
            cbInclAccolade = new System.Windows.Forms.CheckBox();
            label8 = new System.Windows.Forms.Label();
            mbfCancel = new ImageButtonEx();
            mbfExport = new ImageButtonEx();
            navStrip1 = new NavStrip();
            formPages1.SuspendLayout();
            buildData.SuspendLayout();
            forumFormats.SuspendLayout();
            infoGraphic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)igPictureBox).BeginInit();
            mobileFriendly.SuspendLayout();
            borderPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webViewPreview).BeginInit();
            SuspendLayout();
            // 
            // formPages1
            // 
            formPages1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            formPages1.Controls.Add(buildData);
            formPages1.Controls.Add(forumFormats);
            formPages1.Controls.Add(infoGraphic);
            formPages1.Controls.Add(mobileFriendly);
            formPages1.Location = new System.Drawing.Point(12, 62);
            formPages1.Name = "formPages1";
            formPages1.Pages.Add(buildData);
            formPages1.Pages.Add(forumFormats);
            formPages1.Pages.Add(infoGraphic);
            formPages1.Pages.Add(mobileFriendly);
            formPages1.SelectedIndex = 3;
            formPages1.Size = new System.Drawing.Size(776, 393);
            formPages1.TabIndex = 45;
            // 
            // buildData
            // 
            buildData.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            buildData.Anchor = System.Windows.Forms.AnchorStyles.None;
            buildData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            buildData.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            buildData.Controls.Add(bdCancel);
            buildData.Controls.Add(bdExport);
            buildData.Controls.Add(label12);
            buildData.Controls.Add(bdChunkBox);
            buildData.Dock = System.Windows.Forms.DockStyle.Fill;
            buildData.ForeColor = System.Drawing.Color.WhiteSmoke;
            buildData.Location = new System.Drawing.Point(0, 0);
            buildData.Name = "buildData";
            buildData.Size = new System.Drawing.Size(774, 391);
            buildData.TabIndex = 2;
            buildData.Title = "Build Data (Chunk)";
            // 
            // bdCancel
            // 
            bdCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            bdCancel.CurrentText = "Close";
            bdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            bdCancel.Images.Background = MRBResourceLib.Resources.HeroButton;
            bdCancel.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            bdCancel.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            bdCancel.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            bdCancel.Location = new System.Drawing.Point(656, 342);
            bdCancel.Lock = false;
            bdCancel.Name = "bdCancel";
            bdCancel.Size = new System.Drawing.Size(100, 28);
            bdCancel.TabIndex = 70;
            bdCancel.Text = "Close";
            bdCancel.TextOutline.Color = System.Drawing.Color.Black;
            bdCancel.TextOutline.Width = 3;
            bdCancel.ToggleState = ImageButtonEx.States.ToggledOff;
            bdCancel.ToggleText.Indeterminate = "Indeterminate State";
            bdCancel.ToggleText.ToggledOff = "ToggledOff State";
            bdCancel.ToggleText.ToggledOn = "ToggledOn State";
            bdCancel.UseAlt = false;
            bdCancel.Click += BtnCancel_Click;
            // 
            // bdExport
            // 
            bdExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            bdExport.CurrentText = "Export";
            bdExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            bdExport.Images.Background = MRBResourceLib.Resources.HeroButton;
            bdExport.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            bdExport.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            bdExport.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            bdExport.Location = new System.Drawing.Point(550, 342);
            bdExport.Lock = false;
            bdExport.Name = "bdExport";
            bdExport.Size = new System.Drawing.Size(100, 28);
            bdExport.TabIndex = 69;
            bdExport.Text = "Export";
            bdExport.TextOutline.Color = System.Drawing.Color.Black;
            bdExport.TextOutline.Width = 3;
            bdExport.ToggleState = ImageButtonEx.States.ToggledOff;
            bdExport.ToggleText.Indeterminate = "Indeterminate State";
            bdExport.ToggleText.ToggledOff = "ToggledOff State";
            bdExport.ToggleText.ToggledOn = "ToggledOn State";
            bdExport.UseAlt = false;
            bdExport.Click += btnExport_Click;
            // 
            // label12
            // 
            label12.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label12.Location = new System.Drawing.Point(10, 12);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(755, 37);
            label12.TabIndex = 68;
            label12.Text = "A compressed data chunk generated from the character data that can be shared with friends and imported into another instance. The output will be copied into the clipboard.";
            // 
            // bdChunkBox
            // 
            bdChunkBox.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            bdChunkBox.ForeColor = System.Drawing.Color.WhiteSmoke;
            bdChunkBox.Location = new System.Drawing.Point(10, 52);
            bdChunkBox.Multiline = true;
            bdChunkBox.Name = "bdChunkBox";
            bdChunkBox.ReadOnly = true;
            bdChunkBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            bdChunkBox.Size = new System.Drawing.Size(746, 271);
            bdChunkBox.TabIndex = 0;
            bdChunkBox.WordWrap = true;
            // 
            // forumFormats
            // 
            forumFormats.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            forumFormats.Anchor = System.Windows.Forms.AnchorStyles.None;
            forumFormats.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            forumFormats.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            forumFormats.Controls.Add(resetThemeBtn);
            forumFormats.Controls.Add(remThemeBtn);
            forumFormats.Controls.Add(addThemeBtn);
            forumFormats.Controls.Add(label11);
            forumFormats.Controls.Add(label10);
            forumFormats.Controls.Add(chkCustomThemeDark);
            forumFormats.Controls.Add(chkOptLongFormat);
            forumFormats.Controls.Add(chkOptIncarnates);
            forumFormats.Controls.Add(chkOptAccolades);
            forumFormats.Controls.Add(label7);
            forumFormats.Controls.Add(ffCancel);
            forumFormats.Controls.Add(ffExport);
            forumFormats.Controls.Add(lbFormatCodeType);
            forumFormats.Controls.Add(label6);
            forumFormats.Controls.Add(rbAllThemes);
            forumFormats.Controls.Add(rbDarkThemes);
            forumFormats.Controls.Add(rbLightThemes);
            forumFormats.Controls.Add(label5);
            forumFormats.Controls.Add(label4);
            forumFormats.Controls.Add(label3);
            forumFormats.Controls.Add(label2);
            forumFormats.Controls.Add(panelColorSlots);
            forumFormats.Controls.Add(panelColorLevels);
            forumFormats.Controls.Add(panelColorHeadings);
            forumFormats.Controls.Add(panelColorTitle);
            forumFormats.Controls.Add(lbColorTheme);
            forumFormats.Controls.Add(label1);
            forumFormats.Dock = System.Windows.Forms.DockStyle.Fill;
            forumFormats.ForeColor = System.Drawing.Color.WhiteSmoke;
            forumFormats.Location = new System.Drawing.Point(0, 0);
            forumFormats.Name = "forumFormats";
            forumFormats.Size = new System.Drawing.Size(774, 391);
            forumFormats.TabIndex = 1;
            forumFormats.Title = "Forum Formats (Text)";
            // 
            // resetThemeBtn
            // 
            resetThemeBtn.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            resetThemeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSlateGray;
            resetThemeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            resetThemeBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            resetThemeBtn.Location = new System.Drawing.Point(220, 112);
            resetThemeBtn.Name = "resetThemeBtn";
            resetThemeBtn.Size = new System.Drawing.Size(30, 89);
            resetThemeBtn.TabIndex = 71;
            resetThemeBtn.Text = "R\r\nE\r\nS\r\nE\r\nT";
            resetThemeBtn.UseVisualStyleBackColor = true;
            resetThemeBtn.Click += resetThemeBtn_Click;
            // 
            // remThemeBtn
            // 
            remThemeBtn.BackColor = System.Drawing.Color.FromArgb(50, 115, 124, 161);
            remThemeBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            remThemeBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            remThemeBtn.Location = new System.Drawing.Point(120, 228);
            remThemeBtn.Name = "remThemeBtn";
            remThemeBtn.Size = new System.Drawing.Size(61, 23);
            remThemeBtn.TabIndex = 70;
            remThemeBtn.Text = "Remove";
            remThemeBtn.UseVisualStyleBackColor = false;
            remThemeBtn.Click += remThemBtn_Click;
            // 
            // addThemeBtn
            // 
            addThemeBtn.BackColor = System.Drawing.Color.FromArgb(100, 115, 124, 161);
            addThemeBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            addThemeBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            addThemeBtn.Location = new System.Drawing.Point(52, 228);
            addThemeBtn.Name = "addThemeBtn";
            addThemeBtn.Size = new System.Drawing.Size(61, 23);
            addThemeBtn.TabIndex = 69;
            addThemeBtn.Text = "Add";
            addThemeBtn.UseVisualStyleBackColor = false;
            addThemeBtn.Click += addThemeBtn_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Segoe UI Variable Display", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label11.Location = new System.Drawing.Point(291, 274);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(85, 17);
            label11.TabIndex = 68;
            label11.Text = "Theme filter:";
            // 
            // label10
            // 
            label10.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label10.Location = new System.Drawing.Point(10, 12);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(755, 52);
            label10.TabIndex = 67;
            label10.Text = resources.GetString("label10.Text");
            // 
            // chkCustomThemeDark
            // 
            chkCustomThemeDark.AutoSize = true;
            chkCustomThemeDark.Location = new System.Drawing.Point(293, 216);
            chkCustomThemeDark.Name = "chkCustomThemeDark";
            chkCustomThemeDark.Size = new System.Drawing.Size(94, 19);
            chkCustomThemeDark.TabIndex = 66;
            chkCustomThemeDark.Text = "Dark Theme?";
            chkCustomThemeDark.UseVisualStyleBackColor = true;
            chkCustomThemeDark.CheckedChanged += chkCustomThemeDark_CheckedChanged;
            // 
            // chkOptLongFormat
            // 
            chkOptLongFormat.AutoSize = true;
            chkOptLongFormat.Location = new System.Drawing.Point(17, 353);
            chkOptLongFormat.Name = "chkOptLongFormat";
            chkOptLongFormat.Size = new System.Drawing.Size(218, 19);
            chkOptLongFormat.TabIndex = 65;
            chkOptLongFormat.Text = "Include Bonus Totals, Set Breakdown";
            chkOptLongFormat.UseVisualStyleBackColor = true;
            chkOptLongFormat.CheckedChanged += chkOptLongFormat_CheckChanged;
            // 
            // chkOptIncarnates
            // 
            chkOptIncarnates.AutoSize = true;
            chkOptIncarnates.Location = new System.Drawing.Point(17, 328);
            chkOptIncarnates.Name = "chkOptIncarnates";
            chkOptIncarnates.Size = new System.Drawing.Size(122, 19);
            chkOptIncarnates.TabIndex = 64;
            chkOptIncarnates.Text = "Include Incarnates";
            chkOptIncarnates.UseVisualStyleBackColor = true;
            chkOptIncarnates.CheckedChanged += chkOptIncarnates_CheckChanged;
            // 
            // chkOptAccolades
            // 
            chkOptAccolades.AutoSize = true;
            chkOptAccolades.Location = new System.Drawing.Point(17, 304);
            chkOptAccolades.Name = "chkOptAccolades";
            chkOptAccolades.Size = new System.Drawing.Size(122, 19);
            chkOptAccolades.TabIndex = 63;
            chkOptAccolades.Text = "Include Accolades";
            chkOptAccolades.UseVisualStyleBackColor = true;
            chkOptAccolades.CheckedChanged += chkOptAccolades_CheckChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Segoe UI Variable Display", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label7.Location = new System.Drawing.Point(17, 281);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(101, 17);
            label7.TabIndex = 62;
            label7.Text = "Export options:";
            // 
            // ffCancel
            // 
            ffCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ffCancel.CurrentText = "Close";
            ffCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ffCancel.Images.Background = MRBResourceLib.Resources.HeroButton;
            ffCancel.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ffCancel.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ffCancel.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ffCancel.Location = new System.Drawing.Point(656, 342);
            ffCancel.Lock = false;
            ffCancel.Name = "ffCancel";
            ffCancel.Size = new System.Drawing.Size(100, 28);
            ffCancel.TabIndex = 61;
            ffCancel.Text = "Close";
            ffCancel.TextOutline.Color = System.Drawing.Color.Black;
            ffCancel.TextOutline.Width = 3;
            ffCancel.ToggleState = ImageButtonEx.States.ToggledOff;
            ffCancel.ToggleText.Indeterminate = "Indeterminate State";
            ffCancel.ToggleText.ToggledOff = "ToggledOff State";
            ffCancel.ToggleText.ToggledOn = "ToggledOn State";
            ffCancel.UseAlt = false;
            ffCancel.Click += BtnCancel_Click;
            // 
            // ffExport
            // 
            ffExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ffExport.CurrentText = "Export";
            ffExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ffExport.Images.Background = MRBResourceLib.Resources.HeroButton;
            ffExport.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ffExport.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ffExport.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ffExport.Location = new System.Drawing.Point(550, 342);
            ffExport.Lock = false;
            ffExport.Name = "ffExport";
            ffExport.Size = new System.Drawing.Size(100, 28);
            ffExport.TabIndex = 60;
            ffExport.Text = "Export";
            ffExport.TextOutline.Color = System.Drawing.Color.Black;
            ffExport.TextOutline.Width = 3;
            ffExport.ToggleState = ImageButtonEx.States.ToggledOff;
            ffExport.ToggleText.Indeterminate = "Indeterminate State";
            ffExport.ToggleText.ToggledOff = "ToggledOff State";
            ffExport.ToggleText.ToggledOn = "ToggledOn State";
            ffExport.UseAlt = false;
            ffExport.Click += btnExport_Click;
            // 
            // lbFormatCodeType
            // 
            lbFormatCodeType.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            lbFormatCodeType.ForeColor = System.Drawing.Color.WhiteSmoke;
            lbFormatCodeType.ItemHeight = 15;
            lbFormatCodeType.Location = new System.Drawing.Point(442, 83);
            lbFormatCodeType.Name = "lbFormatCodeType";
            lbFormatCodeType.Size = new System.Drawing.Size(314, 139);
            lbFormatCodeType.TabIndex = 59;
            lbFormatCodeType.SelectedIndexChanged += lbFormatCodeType_SelectedIndexChanged;
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
            rbAllThemes.Location = new System.Drawing.Point(291, 343);
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
            rbDarkThemes.Location = new System.Drawing.Point(291, 318);
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
            rbLightThemes.Location = new System.Drawing.Point(291, 294);
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
            // infoGraphic
            // 
            infoGraphic.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            infoGraphic.Anchor = System.Windows.Forms.AnchorStyles.None;
            infoGraphic.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            infoGraphic.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            infoGraphic.Controls.Add(chkUseAltIg);
            infoGraphic.Controls.Add(igCancel);
            infoGraphic.Controls.Add(igExport);
            infoGraphic.Controls.Add(label13);
            infoGraphic.Controls.Add(igPictureBox);
            infoGraphic.Dock = System.Windows.Forms.DockStyle.Fill;
            infoGraphic.ForeColor = System.Drawing.Color.WhiteSmoke;
            infoGraphic.Location = new System.Drawing.Point(0, 0);
            infoGraphic.Name = "infoGraphic";
            infoGraphic.Size = new System.Drawing.Size(774, 391);
            infoGraphic.TabIndex = 3;
            infoGraphic.Title = "InfoGraphic (Image)";
            // 
            // chkUseAltIg
            // 
            chkUseAltIg.AutoSize = true;
            chkUseAltIg.Location = new System.Drawing.Point(10, 88);
            chkUseAltIg.Name = "chkUseAltIg";
            chkUseAltIg.Size = new System.Drawing.Size(140, 19);
            chkUseAltIg.TabIndex = 73;
            chkUseAltIg.Text = "Use Alternate Graphic";
            chkUseAltIg.UseVisualStyleBackColor = true;
            chkUseAltIg.CheckedChanged += chkUseAltIg_CheckChanged;
            // 
            // igCancel
            // 
            igCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            igCancel.CurrentText = "Close";
            igCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            igCancel.Images.Background = MRBResourceLib.Resources.HeroButton;
            igCancel.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            igCancel.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            igCancel.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            igCancel.Location = new System.Drawing.Point(656, 342);
            igCancel.Lock = false;
            igCancel.Name = "igCancel";
            igCancel.Size = new System.Drawing.Size(100, 28);
            igCancel.TabIndex = 72;
            igCancel.Text = "Close";
            igCancel.TextOutline.Color = System.Drawing.Color.Black;
            igCancel.TextOutline.Width = 3;
            igCancel.ToggleState = ImageButtonEx.States.ToggledOff;
            igCancel.ToggleText.Indeterminate = "Indeterminate State";
            igCancel.ToggleText.ToggledOff = "ToggledOff State";
            igCancel.ToggleText.ToggledOn = "ToggledOn State";
            igCancel.UseAlt = false;
            igCancel.Click += BtnCancel_Click;
            // 
            // igExport
            // 
            igExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            igExport.CurrentText = "Export";
            igExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            igExport.Images.Background = MRBResourceLib.Resources.HeroButton;
            igExport.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            igExport.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            igExport.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            igExport.Location = new System.Drawing.Point(550, 342);
            igExport.Lock = false;
            igExport.Name = "igExport";
            igExport.Size = new System.Drawing.Size(100, 28);
            igExport.TabIndex = 71;
            igExport.Text = "Export";
            igExport.TextOutline.Color = System.Drawing.Color.Black;
            igExport.TextOutline.Width = 3;
            igExport.ToggleState = ImageButtonEx.States.ToggledOff;
            igExport.ToggleText.Indeterminate = "Indeterminate State";
            igExport.ToggleText.ToggledOff = "ToggledOff State";
            igExport.ToggleText.ToggledOn = "ToggledOn State";
            igExport.UseAlt = false;
            // 
            // label13
            // 
            label13.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label13.Location = new System.Drawing.Point(10, 12);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(755, 46);
            label13.TabIndex = 69;
            label13.Text = "An infographic generated from character data that can be used on social media or shared with friends thats shows a preview of build stats. The output will be copied into the clipboard.";
            // 
            // igPictureBox
            // 
            igPictureBox.Location = new System.Drawing.Point(159, 61);
            igPictureBox.Name = "igPictureBox";
            igPictureBox.Size = new System.Drawing.Size(597, 275);
            igPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            igPictureBox.TabIndex = 1;
            igPictureBox.TabStop = false;
            // 
            // mobileFriendly
            // 
            mobileFriendly.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            mobileFriendly.Anchor = System.Windows.Forms.AnchorStyles.None;
            mobileFriendly.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            mobileFriendly.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            mobileFriendly.Controls.Add(label14);
            mobileFriendly.Controls.Add(borderPanel1);
            mobileFriendly.Controls.Add(cbInclSetBreakdown);
            mobileFriendly.Controls.Add(label9);
            mobileFriendly.Controls.Add(cbInclSetBonus);
            mobileFriendly.Controls.Add(cbInclIncarnate);
            mobileFriendly.Controls.Add(cbInclAccolade);
            mobileFriendly.Controls.Add(label8);
            mobileFriendly.Controls.Add(mbfCancel);
            mobileFriendly.Controls.Add(mbfExport);
            mobileFriendly.Dock = System.Windows.Forms.DockStyle.Fill;
            mobileFriendly.ForeColor = System.Drawing.Color.WhiteSmoke;
            mobileFriendly.Location = new System.Drawing.Point(0, 0);
            mobileFriendly.Name = "mobileFriendly";
            mobileFriendly.Size = new System.Drawing.Size(774, 391);
            mobileFriendly.TabIndex = 0;
            mobileFriendly.Title = "Mobile Friendly (Link)";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label14.ForeColor = System.Drawing.Color.DimGray;
            label14.Location = new System.Drawing.Point(3, 370);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(173, 21);
            label14.TabIndex = 55;
            label14.Text = "Experimental Feature";
            // 
            // borderPanel1
            // 
            borderPanel1.Border.Color = System.Drawing.Color.FromArgb(12, 56, 100);
            borderPanel1.Border.Style = System.Windows.Forms.ButtonBorderStyle.Inset;
            borderPanel1.Border.Thickness = 2;
            borderPanel1.Border.Which = BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel1.Controls.Add(previewLabel);
            borderPanel1.Controls.Add(webViewPreview);
            borderPanel1.Location = new System.Drawing.Point(199, 61);
            borderPanel1.Name = "borderPanel1";
            borderPanel1.Size = new System.Drawing.Size(569, 275);
            borderPanel1.TabIndex = 54;
            // 
            // previewLabel
            // 
            previewLabel.AutoSize = true;
            previewLabel.BackColor = System.Drawing.Color.Firebrick;
            previewLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            previewLabel.Location = new System.Drawing.Point(15, 15);
            previewLabel.Name = "previewLabel";
            previewLabel.Size = new System.Drawing.Size(52, 17);
            previewLabel.TabIndex = 53;
            previewLabel.Text = "Preview";
            previewLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            previewLabel.Visible = false;
            // 
            // webViewPreview
            // 
            webViewPreview.AllowExternalDrop = false;
            webViewPreview.CreationProperties = null;
            webViewPreview.DefaultBackgroundColor = System.Drawing.Color.FromArgb(44, 47, 51);
            webViewPreview.Location = new System.Drawing.Point(3, 3);
            webViewPreview.Name = "webViewPreview";
            webViewPreview.Size = new System.Drawing.Size(563, 269);
            webViewPreview.TabIndex = 51;
            webViewPreview.ZoomFactor = 1D;
            // 
            // cbInclSetBreakdown
            // 
            cbInclSetBreakdown.AutoSize = true;
            cbInclSetBreakdown.Location = new System.Drawing.Point(20, 213);
            cbInclSetBreakdown.Name = "cbInclSetBreakdown";
            cbInclSetBreakdown.Size = new System.Drawing.Size(146, 19);
            cbInclSetBreakdown.TabIndex = 52;
            cbInclSetBreakdown.Text = "Include Set Breakdown";
            cbInclSetBreakdown.UseVisualStyleBackColor = true;
            cbInclSetBreakdown.CheckedChanged += cbInclSetBreakdown_CheckedChanged;
            // 
            // label9
            // 
            label9.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label9.Location = new System.Drawing.Point(10, 12);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(758, 46);
            label9.TabIndex = 50;
            label9.Text = resources.GetString("label9.Text");
            // 
            // cbInclSetBonus
            // 
            cbInclSetBonus.AutoSize = true;
            cbInclSetBonus.Location = new System.Drawing.Point(20, 188);
            cbInclSetBonus.Name = "cbInclSetBonus";
            cbInclSetBonus.Size = new System.Drawing.Size(153, 19);
            cbInclSetBonus.TabIndex = 49;
            cbInclSetBonus.Text = "Include Set Bonus Totals";
            cbInclSetBonus.UseVisualStyleBackColor = true;
            cbInclSetBonus.CheckedChanged += cbInclSetBonus_CheckedChanged;
            // 
            // cbInclIncarnate
            // 
            cbInclIncarnate.AutoSize = true;
            cbInclIncarnate.Location = new System.Drawing.Point(20, 138);
            cbInclIncarnate.Name = "cbInclIncarnate";
            cbInclIncarnate.Size = new System.Drawing.Size(122, 19);
            cbInclIncarnate.TabIndex = 48;
            cbInclIncarnate.Text = "Include Incarnates";
            cbInclIncarnate.UseVisualStyleBackColor = true;
            cbInclIncarnate.CheckedChanged += cbInclIncarnate_CheckedChanged;
            // 
            // cbInclAccolade
            // 
            cbInclAccolade.AutoSize = true;
            cbInclAccolade.Location = new System.Drawing.Point(20, 163);
            cbInclAccolade.Name = "cbInclAccolade";
            cbInclAccolade.Size = new System.Drawing.Size(122, 19);
            cbInclAccolade.TabIndex = 47;
            cbInclAccolade.Text = "Include Accolades";
            cbInclAccolade.UseVisualStyleBackColor = true;
            cbInclAccolade.CheckedChanged += cbInclAccolade_CheckedChanged;
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
            // mbfCancel
            // 
            mbfCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            mbfCancel.CurrentText = "Close";
            mbfCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            mbfCancel.Images.Background = MRBResourceLib.Resources.HeroButton;
            mbfCancel.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            mbfCancel.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            mbfCancel.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            mbfCancel.Location = new System.Drawing.Point(656, 342);
            mbfCancel.Lock = false;
            mbfCancel.Name = "mbfCancel";
            mbfCancel.Size = new System.Drawing.Size(100, 28);
            mbfCancel.TabIndex = 45;
            mbfCancel.Text = "Close";
            mbfCancel.TextOutline.Color = System.Drawing.Color.Black;
            mbfCancel.TextOutline.Width = 3;
            mbfCancel.ToggleState = ImageButtonEx.States.ToggledOff;
            mbfCancel.ToggleText.Indeterminate = "Indeterminate State";
            mbfCancel.ToggleText.ToggledOff = "ToggledOff State";
            mbfCancel.ToggleText.ToggledOn = "ToggledOn State";
            mbfCancel.UseAlt = false;
            mbfCancel.Click += BtnCancel_Click;
            // 
            // mbfExport
            // 
            mbfExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            mbfExport.CurrentText = "Export";
            mbfExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            mbfExport.Images.Background = MRBResourceLib.Resources.HeroButton;
            mbfExport.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            mbfExport.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            mbfExport.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            mbfExport.Location = new System.Drawing.Point(550, 342);
            mbfExport.Lock = false;
            mbfExport.Name = "mbfExport";
            mbfExport.Size = new System.Drawing.Size(100, 28);
            mbfExport.TabIndex = 44;
            mbfExport.Text = "Export";
            mbfExport.TextOutline.Color = System.Drawing.Color.Black;
            mbfExport.TextOutline.Width = 3;
            mbfExport.ToggleState = ImageButtonEx.States.ToggledOff;
            mbfExport.ToggleText.Indeterminate = "Indeterminate State";
            mbfExport.ToggleText.ToggledOff = "ToggledOff State";
            mbfExport.ToggleText.ToggledOn = "ToggledOn State";
            mbfExport.UseAlt = false;
            mbfExport.Click += btnExport_Click;
            // 
            // navStrip1
            // 
            navStrip1.ActiveTabColor = System.Drawing.Color.Goldenrod;
            navStrip1.DataSource = formPages1;
            navStrip1.DimmedColor = System.Drawing.Color.FromArgb(21, 61, 93);
            navStrip1.DisabledTabColor = System.Drawing.Color.DarkGray;
            navStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            navStrip1.InactiveTabColor = System.Drawing.Color.FromArgb(30, 85, 130);
            navStrip1.InactiveTabHoverColor = System.Drawing.Color.FromArgb(43, 122, 187);
            navStrip1.Location = new System.Drawing.Point(12, 18);
            navStrip1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            navStrip1.Name = "navStrip1";
            navStrip1.OutlineColor = System.Drawing.Color.Black;
            navStrip1.Size = new System.Drawing.Size(775, 38);
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
            formPages1.ResumeLayout(false);
            buildData.ResumeLayout(false);
            buildData.PerformLayout();
            forumFormats.ResumeLayout(false);
            forumFormats.PerformLayout();
            infoGraphic.ResumeLayout(false);
            infoGraphic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)igPictureBox).EndInit();
            mobileFriendly.ResumeLayout(false);
            mobileFriendly.PerformLayout();
            borderPanel1.ResumeLayout(false);
            borderPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webViewPreview).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.CheckBox chkOptSetBreakdown;
        private FormPages formPages1;
        private Page mobileFriendly;
        private Page forumFormats;
        private ctlTotalsTabStrip tabStrip1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cbInclSetBonus;
        private System.Windows.Forms.CheckBox cbInclIncarnate;
        private System.Windows.Forms.CheckBox cbInclAccolade;
        private System.Windows.Forms.Label label8;
        private Controls.ImageButtonEx mbfCancel;
        private Controls.ImageButtonEx mbfExport;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkCustomThemeDark;
        private System.Windows.Forms.CheckBox chkOptLongFormat;
        private System.Windows.Forms.CheckBox chkOptIncarnates;
        private System.Windows.Forms.CheckBox chkOptAccolades;
        private System.Windows.Forms.Label label7;
        private Controls.ImageButtonEx ffCancel;
        private Controls.ImageButtonEx ffExport;
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
        private Controls.FilterableListBox lbColorTheme;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private NavStrip navStrip1;
        private System.Windows.Forms.Button resetThemeBtn;
        private System.Windows.Forms.Button remThemeBtn;
        private System.Windows.Forms.Button addThemeBtn;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewPreview;
        private System.Windows.Forms.CheckBox cbInclSetBreakdown;
        private System.Windows.Forms.Label previewLabel;
        private Page buildData;
        private Page infoGraphic;
        private Controls.ImageButtonEx bdExport;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox bdChunkBox;
        private Controls.ImageButtonEx bdCancel;
        private Controls.BorderPanel borderPanel1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.PictureBox igPictureBox;
        private Controls.ImageButtonEx igCancel;
        private Controls.ImageButtonEx igExport;
        private System.Windows.Forms.CheckBox chkUseAltIg;
        private System.Windows.Forms.Label label14;
    }
}
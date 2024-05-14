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
            components = new System.ComponentModel.Container();
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
            linkBased = new Page();
            btnSaveChanges = new System.Windows.Forms.Button();
            ibUpdate = new ImageButtonEx();
            ibSubmit = new ImageButtonEx();
            label25 = new System.Windows.Forms.Label();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            advGroupBox2 = new AdvGroupBox();
            txtExpiresOn = new System.Windows.Forms.TextBox();
            label20 = new System.Windows.Forms.Label();
            txtSchemaUrl = new System.Windows.Forms.TextBox();
            label21 = new System.Windows.Forms.Label();
            txtImageUrl = new System.Windows.Forms.TextBox();
            label22 = new System.Windows.Forms.Label();
            txtDownloadUrl = new System.Windows.Forms.TextBox();
            label23 = new System.Windows.Forms.Label();
            txtId = new System.Windows.Forms.TextBox();
            label24 = new System.Windows.Forms.Label();
            advGroupBox1 = new AdvGroupBox();
            chkEditDesc = new System.Windows.Forms.CheckBox();
            chkEditName = new System.Windows.Forms.CheckBox();
            txtDesc = new System.Windows.Forms.TextBox();
            label19 = new System.Windows.Forms.Label();
            txtSec = new System.Windows.Forms.TextBox();
            label18 = new System.Windows.Forms.Label();
            txtPri = new System.Windows.Forms.TextBox();
            label17 = new System.Windows.Forms.Label();
            txtArchetype = new System.Windows.Forms.TextBox();
            label16 = new System.Windows.Forms.Label();
            txtBuildName = new System.Windows.Forms.TextBox();
            label15 = new System.Windows.Forms.Label();
            infoGraphic = new Page();
            chkUseAltIg = new System.Windows.Forms.CheckBox();
            igCancel = new ImageButtonEx();
            igExport = new ImageButtonEx();
            label13 = new System.Windows.Forms.Label();
            igPictureBox = new System.Windows.Forms.PictureBox();
            navStrip1 = new NavStrip();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            formPages1.SuspendLayout();
            buildData.SuspendLayout();
            forumFormats.SuspendLayout();
            linkBased.SuspendLayout();
            statusStrip1.SuspendLayout();
            advGroupBox2.SuspendLayout();
            advGroupBox1.SuspendLayout();
            infoGraphic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)igPictureBox).BeginInit();
            SuspendLayout();
            // 
            // formPages1
            // 
            formPages1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            formPages1.Controls.Add(buildData);
            formPages1.Controls.Add(forumFormats);
            formPages1.Controls.Add(linkBased);
            formPages1.Controls.Add(infoGraphic);
            formPages1.Location = new System.Drawing.Point(12, 62);
            formPages1.Name = "formPages1";
            formPages1.Pages.Add(buildData);
            formPages1.Pages.Add(forumFormats);
            formPages1.Pages.Add(linkBased);
            formPages1.Pages.Add(infoGraphic);
            formPages1.SelectedIndex = 2;
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
            buildData.Title = "Build (Datachunk)";
            // 
            // bdCancel
            // 
            bdCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            bdCancel.CurrentText = "Close";
            bdCancel.DisplayVertically = false;
            bdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
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
            bdExport.DisplayVertically = false;
            bdExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
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
            label12.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            label12.Location = new System.Drawing.Point(10, 12);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(755, 37);
            label12.TabIndex = 68;
            label12.Text = "A compressed data chunk generated from the character data that can be shared with friends and imported into another instance. The output will be copied into the clipboard.";
            // 
            // bdChunkBox
            // 
            bdChunkBox.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            bdChunkBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            bdChunkBox.ForeColor = System.Drawing.Color.WhiteSmoke;
            bdChunkBox.Location = new System.Drawing.Point(10, 52);
            bdChunkBox.Multiline = true;
            bdChunkBox.Name = "bdChunkBox";
            bdChunkBox.ReadOnly = true;
            bdChunkBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            bdChunkBox.ShortcutsEnabled = false;
            bdChunkBox.Size = new System.Drawing.Size(746, 271);
            bdChunkBox.TabIndex = 0;
            bdChunkBox.WordWrap = false;
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
            forumFormats.Title = "Forum Post (Formatted)";
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
            label11.Font = new System.Drawing.Font("Segoe UI Variable Display", 9.75F, System.Drawing.FontStyle.Bold);
            label11.Location = new System.Drawing.Point(291, 274);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(85, 17);
            label11.TabIndex = 68;
            label11.Text = "Theme filter:";
            // 
            // label10
            // 
            label10.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
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
            label7.Font = new System.Drawing.Font("Segoe UI Variable Display", 9.75F, System.Drawing.FontStyle.Bold);
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
            ffCancel.DisplayVertically = false;
            ffCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
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
            ffExport.DisplayVertically = false;
            ffExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
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
            label6.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Bold);
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
            label1.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Bold);
            label1.Location = new System.Drawing.Point(17, 64);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(83, 16);
            label1.TabIndex = 45;
            label1.Text = "Color Theme:";
            // 
            // linkBased
            // 
            linkBased.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            linkBased.Anchor = System.Windows.Forms.AnchorStyles.None;
            linkBased.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            linkBased.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            linkBased.Controls.Add(btnSaveChanges);
            linkBased.Controls.Add(ibUpdate);
            linkBased.Controls.Add(ibSubmit);
            linkBased.Controls.Add(label25);
            linkBased.Controls.Add(statusStrip1);
            linkBased.Controls.Add(advGroupBox2);
            linkBased.Controls.Add(advGroupBox1);
            linkBased.Dock = System.Windows.Forms.DockStyle.Fill;
            linkBased.ForeColor = System.Drawing.Color.WhiteSmoke;
            linkBased.Location = new System.Drawing.Point(0, 0);
            linkBased.Name = "linkBased";
            linkBased.Size = new System.Drawing.Size(774, 391);
            linkBased.TabIndex = 4;
            linkBased.Title = "Build Links (API)";
            linkBased.Visible = false;
            // 
            // btnSaveChanges
            // 
            btnSaveChanges.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnSaveChanges.ForeColor = System.Drawing.Color.Black;
            btnSaveChanges.Location = new System.Drawing.Point(112, 339);
            btnSaveChanges.Name = "btnSaveChanges";
            btnSaveChanges.Size = new System.Drawing.Size(101, 22);
            btnSaveChanges.TabIndex = 12;
            btnSaveChanges.Text = "Save Changes";
            btnSaveChanges.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            btnSaveChanges.UseVisualStyleBackColor = true;
            btnSaveChanges.Click += btnSaveChanges_Click;
            // 
            // ibUpdate
            // 
            ibUpdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ibUpdate.CurrentText = "Update";
            ibUpdate.DisplayVertically = false;
            ibUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            ibUpdate.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibUpdate.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibUpdate.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibUpdate.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibUpdate.Location = new System.Drawing.Point(311, 207);
            ibUpdate.Lock = false;
            ibUpdate.Name = "ibUpdate";
            ibUpdate.Size = new System.Drawing.Size(133, 35);
            ibUpdate.TabIndex = 73;
            ibUpdate.Text = "Update";
            ibUpdate.TextOutline.Color = System.Drawing.Color.Black;
            ibUpdate.TextOutline.Width = 3;
            ibUpdate.ToggleState = ImageButtonEx.States.ToggledOff;
            ibUpdate.ToggleText.Indeterminate = "Indeterminate State";
            ibUpdate.ToggleText.ToggledOff = "ToggledOff State";
            ibUpdate.ToggleText.ToggledOn = "ToggledOn State";
            ibUpdate.UseAlt = false;
            ibUpdate.Click += IbUpdateButton_Click;
            // 
            // ibSubmit
            // 
            ibSubmit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ibSubmit.CurrentText = "Submit";
            ibSubmit.DisplayVertically = false;
            ibSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            ibSubmit.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibSubmit.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibSubmit.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibSubmit.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibSubmit.Location = new System.Drawing.Point(311, 157);
            ibSubmit.Lock = false;
            ibSubmit.Name = "ibSubmit";
            ibSubmit.Size = new System.Drawing.Size(133, 35);
            ibSubmit.TabIndex = 72;
            ibSubmit.Text = "Submit";
            ibSubmit.TextOutline.Color = System.Drawing.Color.Black;
            ibSubmit.TextOutline.Width = 3;
            ibSubmit.ToggleState = ImageButtonEx.States.ToggledOff;
            ibSubmit.ToggleText.Indeterminate = "Indeterminate State";
            ibSubmit.ToggleText.ToggledOff = "ToggledOff State";
            ibSubmit.ToggleText.ToggledOn = "ToggledOn State";
            ibSubmit.UseAlt = false;
            ibSubmit.Click += IbSubmitButton_Click;
            // 
            // label25
            // 
            label25.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            label25.Location = new System.Drawing.Point(20, 10);
            label25.Name = "label25";
            label25.Size = new System.Drawing.Size(736, 25);
            label25.TabIndex = 68;
            label25.Text = "Submits your build to the API for link generation. If successful, links will be provided in the information boxes.";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new System.Drawing.Point(0, 369);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(774, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 3;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.BackColor = System.Drawing.Color.Transparent;
            toolStripStatusLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.Adjust;
            toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripStatusLabel1.ForeColor = System.Drawing.Color.Black;
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(194, 17);
            toolStripStatusLabel1.Text = "Please submit or update your build.";
            // 
            // advGroupBox2
            // 
            advGroupBox2.BorderColor = System.Drawing.Color.DodgerBlue;
            advGroupBox2.Controls.Add(txtExpiresOn);
            advGroupBox2.Controls.Add(label20);
            advGroupBox2.Controls.Add(txtSchemaUrl);
            advGroupBox2.Controls.Add(label21);
            advGroupBox2.Controls.Add(txtImageUrl);
            advGroupBox2.Controls.Add(label22);
            advGroupBox2.Controls.Add(txtDownloadUrl);
            advGroupBox2.Controls.Add(label23);
            advGroupBox2.Controls.Add(txtId);
            advGroupBox2.Controls.Add(label24);
            advGroupBox2.Location = new System.Drawing.Point(450, 44);
            advGroupBox2.Name = "advGroupBox2";
            advGroupBox2.RoundedCorners.Enabled = true;
            advGroupBox2.RoundedCorners.Radius = 10;
            advGroupBox2.Size = new System.Drawing.Size(306, 293);
            advGroupBox2.TabIndex = 2;
            advGroupBox2.TabStop = false;
            advGroupBox2.Text = "Links / Info";
            advGroupBox2.TitleColor = System.Drawing.Color.WhiteSmoke;
            advGroupBox2.TitleFont = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            // 
            // txtExpiresOn
            // 
            txtExpiresOn.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtExpiresOn.Location = new System.Drawing.Point(20, 254);
            txtExpiresOn.Name = "txtExpiresOn";
            txtExpiresOn.ReadOnly = true;
            txtExpiresOn.Size = new System.Drawing.Size(254, 23);
            txtExpiresOn.TabIndex = 9;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new System.Drawing.Point(22, 236);
            label20.Name = "label20";
            label20.Size = new System.Drawing.Size(66, 15);
            label20.TabIndex = 8;
            label20.Text = "Expires On:";
            // 
            // txtSchemaUrl
            // 
            txtSchemaUrl.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtSchemaUrl.Location = new System.Drawing.Point(20, 201);
            txtSchemaUrl.Name = "txtSchemaUrl";
            txtSchemaUrl.ReadOnly = true;
            txtSchemaUrl.Size = new System.Drawing.Size(254, 23);
            txtSchemaUrl.TabIndex = 7;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new System.Drawing.Point(20, 183);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(199, 15);
            label21.TabIndex = 6;
            label21.Text = "Schema URL (For use in HTML only):";
            // 
            // txtImageUrl
            // 
            txtImageUrl.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtImageUrl.Location = new System.Drawing.Point(20, 151);
            txtImageUrl.Name = "txtImageUrl";
            txtImageUrl.ReadOnly = true;
            txtImageUrl.Size = new System.Drawing.Size(254, 23);
            txtImageUrl.TabIndex = 5;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new System.Drawing.Point(20, 133);
            label22.Name = "label22";
            label22.Size = new System.Drawing.Size(67, 15);
            label22.TabIndex = 4;
            label22.Text = "Image URL:";
            // 
            // txtDownloadUrl
            // 
            txtDownloadUrl.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtDownloadUrl.Location = new System.Drawing.Point(20, 101);
            txtDownloadUrl.Name = "txtDownloadUrl";
            txtDownloadUrl.ReadOnly = true;
            txtDownloadUrl.Size = new System.Drawing.Size(254, 23);
            txtDownloadUrl.TabIndex = 3;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new System.Drawing.Point(20, 83);
            label23.Name = "label23";
            label23.Size = new System.Drawing.Size(88, 15);
            label23.TabIndex = 2;
            label23.Text = "Download URL:";
            // 
            // txtId
            // 
            txtId.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtId.Location = new System.Drawing.Point(20, 49);
            txtId.Name = "txtId";
            txtId.ReadOnly = true;
            txtId.Size = new System.Drawing.Size(254, 23);
            txtId.TabIndex = 1;
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new System.Drawing.Point(20, 31);
            label24.Name = "label24";
            label24.Size = new System.Drawing.Size(61, 15);
            label24.TabIndex = 0;
            label24.Text = "Unique Id:";
            // 
            // advGroupBox1
            // 
            advGroupBox1.BorderColor = System.Drawing.Color.DodgerBlue;
            advGroupBox1.Controls.Add(chkEditDesc);
            advGroupBox1.Controls.Add(chkEditName);
            advGroupBox1.Controls.Add(txtDesc);
            advGroupBox1.Controls.Add(label19);
            advGroupBox1.Controls.Add(txtSec);
            advGroupBox1.Controls.Add(label18);
            advGroupBox1.Controls.Add(txtPri);
            advGroupBox1.Controls.Add(label17);
            advGroupBox1.Controls.Add(txtArchetype);
            advGroupBox1.Controls.Add(label16);
            advGroupBox1.Controls.Add(txtBuildName);
            advGroupBox1.Controls.Add(label15);
            advGroupBox1.Location = new System.Drawing.Point(20, 44);
            advGroupBox1.Name = "advGroupBox1";
            advGroupBox1.RoundedCorners.Enabled = true;
            advGroupBox1.RoundedCorners.Radius = 10;
            advGroupBox1.Size = new System.Drawing.Size(285, 293);
            advGroupBox1.TabIndex = 1;
            advGroupBox1.TabStop = false;
            advGroupBox1.Text = "Build Details (Review)";
            advGroupBox1.TitleColor = System.Drawing.Color.WhiteSmoke;
            advGroupBox1.TitleFont = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            // 
            // chkEditDesc
            // 
            chkEditDesc.AutoSize = true;
            chkEditDesc.Location = new System.Drawing.Point(256, 248);
            chkEditDesc.Name = "chkEditDesc";
            chkEditDesc.Size = new System.Drawing.Size(15, 14);
            chkEditDesc.TabIndex = 11;
            chkEditDesc.UseVisualStyleBackColor = true;
            chkEditDesc.CheckedChanged += chkEditDesc_CheckedChanged;
            // 
            // chkEditName
            // 
            chkEditName.AutoSize = true;
            chkEditName.Location = new System.Drawing.Point(256, 53);
            chkEditName.Name = "chkEditName";
            chkEditName.Size = new System.Drawing.Size(15, 14);
            chkEditName.TabIndex = 10;
            chkEditName.UseVisualStyleBackColor = true;
            chkEditName.CheckedChanged += chkEditName_CheckedChanged;
            // 
            // txtDesc
            // 
            txtDesc.Enabled = false;
            txtDesc.Location = new System.Drawing.Point(20, 227);
            txtDesc.Multiline = true;
            txtDesc.Name = "txtDesc";
            txtDesc.ReadOnly = true;
            txtDesc.Size = new System.Drawing.Size(226, 57);
            txtDesc.TabIndex = 9;
            txtDesc.TextChanged += BuildInfo_Changed;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(20, 209);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(70, 15);
            label19.TabIndex = 8;
            label19.Text = "Description:";
            // 
            // txtSec
            // 
            txtSec.Enabled = false;
            txtSec.Location = new System.Drawing.Point(20, 183);
            txtSec.Name = "txtSec";
            txtSec.ReadOnly = true;
            txtSec.Size = new System.Drawing.Size(226, 23);
            txtSec.TabIndex = 7;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new System.Drawing.Point(20, 164);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(116, 15);
            label18.TabIndex = 6;
            label18.Text = "Secondary Powerset:";
            // 
            // txtPri
            // 
            txtPri.Enabled = false;
            txtPri.Location = new System.Drawing.Point(20, 138);
            txtPri.Name = "txtPri";
            txtPri.ReadOnly = true;
            txtPri.Size = new System.Drawing.Size(226, 23);
            txtPri.TabIndex = 5;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(20, 120);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(102, 15);
            label17.TabIndex = 4;
            label17.Text = "Primary Powerset:";
            // 
            // txtArchetype
            // 
            txtArchetype.Enabled = false;
            txtArchetype.Location = new System.Drawing.Point(20, 94);
            txtArchetype.Name = "txtArchetype";
            txtArchetype.ReadOnly = true;
            txtArchetype.Size = new System.Drawing.Size(226, 23);
            txtArchetype.TabIndex = 3;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(20, 75);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(64, 15);
            label16.TabIndex = 2;
            label16.Text = "Archetype:";
            // 
            // txtBuildName
            // 
            txtBuildName.Enabled = false;
            txtBuildName.Location = new System.Drawing.Point(20, 49);
            txtBuildName.Name = "txtBuildName";
            txtBuildName.ReadOnly = true;
            txtBuildName.Size = new System.Drawing.Size(226, 23);
            txtBuildName.TabIndex = 1;
            txtBuildName.TextChanged += BuildInfo_Changed;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(20, 31);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(72, 15);
            label15.TabIndex = 0;
            label15.Text = "Build Name:";
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
            igCancel.DisplayVertically = false;
            igCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
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
            igExport.DisplayVertically = false;
            igExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
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
            label13.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
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
            // navStrip1
            // 
            navStrip1.ActiveTabColor = System.Drawing.Color.Goldenrod;
            navStrip1.DataSource = formPages1;
            navStrip1.DimmedColor = System.Drawing.Color.FromArgb(21, 61, 93);
            navStrip1.DisabledTabColor = System.Drawing.Color.DarkGray;
            navStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            navStrip1.InactiveTabColor = System.Drawing.Color.FromArgb(30, 85, 130);
            navStrip1.InactiveTabHoverColor = System.Drawing.Color.FromArgb(43, 122, 187);
            navStrip1.Location = new System.Drawing.Point(12, 18);
            navStrip1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            navStrip1.Name = "navStrip1";
            navStrip1.OutlineColor = System.Drawing.Color.Black;
            navStrip1.Size = new System.Drawing.Size(775, 38);
            navStrip1.TabIndex = 46;
            // 
            // toolTip1
            // 
            toolTip1.IsBalloon = true;
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
            linkBased.ResumeLayout(false);
            linkBased.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            advGroupBox2.ResumeLayout(false);
            advGroupBox2.PerformLayout();
            advGroupBox1.ResumeLayout(false);
            advGroupBox1.PerformLayout();
            infoGraphic.ResumeLayout(false);
            infoGraphic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)igPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.CheckBox chkOptSetBreakdown;
        private FormPages formPages1;
        private Page forumFormats;
        private ctlTotalsTabStrip tabStrip1;
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
        private Page buildData;
        private Page infoGraphic;
        private Controls.ImageButtonEx bdExport;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox bdChunkBox;
        private Controls.ImageButtonEx bdCancel;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.PictureBox igPictureBox;
        private Controls.ImageButtonEx igCancel;
        private Controls.ImageButtonEx igExport;
        private System.Windows.Forms.CheckBox chkUseAltIg;
        private Page linkBased;
        private AdvGroupBox advGroupBox1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtPri;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtArchetype;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtBuildName;
        private System.Windows.Forms.TextBox txtSec;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.Label label19;
        private AdvGroupBox advGroupBox2;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtSchemaUrl;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtImageUrl;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtDownloadUrl;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtExpiresOn;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label25;
        private ImageButtonEx ibUpdate;
        private ImageButtonEx ibSubmit;
        private System.Windows.Forms.CheckBox chkEditName;
        private System.Windows.Forms.CheckBox chkEditDesc;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnSaveChanges;
    }
}